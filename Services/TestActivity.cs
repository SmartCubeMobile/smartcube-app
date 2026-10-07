using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace SmartCubeMobile.Services
{
    // Test accounts only. When the signed-in account has been flagged as a test user by an admin, this
    // reports what the account adds, changes, removes and uploads to the SmartCube server so it can be
    // watched on the admin page. It does nothing for any other account, and a banner shows in the app
    // while it is on. Secrets (passwords, tokens, keys, PINs) are removed before anything leaves the PC.
    //
    // How it sees changes: SecureFile calls BeforeWrite/AfterWrite around every data-file save; for each
    // .json file it compares the new contents with the previous ones and queues one event per item added,
    // removed or edited. Documents are reported (and a copy uploaded) from DocumentStorageService.
    public static class TestActivity
    {
        private static readonly object _lock = new();
        private static readonly List<EventDto> _queue = new();
        private static readonly Dictionary<string, JToken> _last = new(StringComparer.OrdinalIgnoreCase);
        private static Timer _timer;
        private static bool _sending;
        private const int MaxQueue = 1000;
        private const long MaxUploadBytes = 20 * 1024 * 1024;

        public static bool Active { get; private set; }
        public static event Action ActiveChanged;

        // Files that hold secrets, caches that change constantly, or the vault itself: never reported.
        private static readonly Regex SkipFile = new(@"(_cache|supplier_logins|device|keys)\.json$", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private static readonly Regex Secret = new(@"pass(word|wd)?|secret|token|api_?key|licen[cs]e_?key|private_?key|credential|\botp\b|cookie|authori[sz]ation|^pin$|_pin$", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        // Fields that change on every refresh and would only be noise.
        private static readonly Regex Noise = new(@"^(last(sync|refresh|check|updated|seen)\w*|updatedat|lastlogin|connectedat)$", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        public static void SetActive(bool on)
        {
            if (Active == on) return;
            Active = on;
            lock (_lock) { _queue.Clear(); _last.Clear(); }
            if (on) _timer = new Timer(_ => Flush(), null, TimeSpan.FromSeconds(4), TimeSpan.FromSeconds(4));
            else { _timer?.Dispose(); _timer = null; }
            try { ActiveChanged?.Invoke(); } catch { }
        }

        // ---------------- Data files ----------------

        private static bool Tracked(string path) =>
            Active && path != null && path.EndsWith(".json", StringComparison.OrdinalIgnoreCase) && !SkipFile.IsMatch(path);

        // Remember what the file held before it is overwritten (first write of the session only).
        public static void BeforeWrite(string path)
        {
            if (!Tracked(path)) return;
            try
            {
                lock (_lock) { if (_last.ContainsKey(path)) return; }
                JToken before = null;
                if (File.Exists(path))
                {
                    var text = SecureFile.ReadAllText(path);
                    if (!string.IsNullOrWhiteSpace(text)) before = Clean(JToken.Parse(text));
                }
                lock (_lock) { _last[path] = before; }
            }
            catch { }
        }

        public static void AfterWrite(string path, string contents)
        {
            if (!Tracked(path)) return;
            try
            {
                if (string.IsNullOrWhiteSpace(contents)) return;
                var after = Clean(JToken.Parse(contents));
                JToken before;
                lock (_lock) { _last.TryGetValue(path, out before); _last[path] = after; }
                var area = Path.GetFileNameWithoutExtension(path);
                var events = new List<EventDto>();
                Diff(area, before, after, events, 0);
                if (area.Equals("documents", StringComparison.OrdinalIgnoreCase))
                    events.RemoveAll(e => e.kind == "add");   // reported as uploads with the file
                Enqueue(events);
            }
            catch { }
        }

        // Secrets and noisy fields out, before comparing and before sending.
        private static JToken Clean(JToken t)
        {
            if (t is JObject o)
            {
                foreach (var p in o.Properties().ToList())
                {
                    if (Noise.IsMatch(p.Name)) { p.Remove(); continue; }
                    if (Secret.IsMatch(p.Name)) { p.Value = "[hidden]"; continue; }
                    Clean(p.Value);
                }
            }
            else if (t is JArray a) foreach (var item in a) Clean(item);
            return t;
        }

        private static string KeyOf(JToken item, int index)
        {
            if (item is JObject o)
            {
                foreach (var name in new[] { "Id", "id", "Key", "Provider", "Name", "Title" })
                    if (o[name] is JValue v && v.Type != JTokenType.Null && !string.IsNullOrWhiteSpace(v.ToString())) return v.ToString();
                return "#" + Math.Abs(o.ToString(Formatting.None).GetHashCode());
            }
            return item is JValue jv ? jv.ToString() : "#" + index;
        }

        private static string Label(JToken item)
        {
            if (item is JObject o)
                foreach (var name in new[] { "Name", "Title", "DisplayName", "Provider", "Supplier", "SupplierName", "Description", "Address", "Postcode", "Id" })
                    if (o[name] is JValue v && v.Type != JTokenType.Null && !string.IsNullOrWhiteSpace(v.ToString())) return v.ToString();
            return item is JValue jv ? jv.ToString() : "item";
        }

        public class EventDto
        {
            public DateTime at { get; set; }
            public string area { get; set; }
            public string kind { get; set; }
            public string key { get; set; }
            public string summary { get; set; }
            public JToken data { get; set; }
        }

        private static EventDto Ev(string area, string kind, string key, string summary, JToken data) => new()
        {
            at = DateTime.UtcNow,
            area = area,
            kind = kind,
            key = key,
            summary = summary.Length > 380 ? summary[..380] : summary,
            data = data == null ? null : Trim(data),
        };

        private static JToken Trim(JToken d) => d.ToString(Formatting.None).Length > 15000 ? new JValue(d.ToString(Formatting.None)[..15000] + "â€¦") : d;

        private static void Diff(string area, JToken before, JToken after, List<EventDto> events, int depth)
        {
            if (events.Count >= 100) return;
            if (after is JArray na)
            {
                var oa = before as JArray ?? new JArray();
                var oldMap = Index(oa); var newMap = Index(na);
                foreach (var kv in newMap)
                {
                    if (!oldMap.TryGetValue(kv.Key, out var old)) events.Add(Ev(area, "add", kv.Key, $"Added: {Label(kv.Value)}", kv.Value));
                    else if (!JToken.DeepEquals(old, kv.Value)) events.Add(Ev(area, "edit", kv.Key, $"Changed: {Label(kv.Value)}{ChangedFields(old, kv.Value)}", Changes(old, kv.Value)));
                }
                foreach (var kv in oldMap.Where(k => !newMap.ContainsKey(k.Key)))
                    events.Add(Ev(area, "remove", kv.Key, $"Removed: {Label(kv.Value)}", kv.Value));
            }
            else if (after is JObject no)
            {
                var oo = before as JObject ?? new JObject();
                foreach (var p in no.Properties())
                {
                    var old = oo[p.Name];
                    if (old == null) events.Add(Ev(area, "add", p.Name, $"Set {p.Name}", p.Value));
                    else if (JToken.DeepEquals(old, p.Value)) continue;
                    else if (depth < 2 && (p.Value is JArray || (p.Value is JObject && old is JObject) ) && old.Type == p.Value.Type)
                        Diff(area + "/" + p.Name, old, p.Value, events, depth + 1);
                    else events.Add(Ev(area, "edit", p.Name, $"Changed {p.Name}", new JObject { ["from"] = old, ["to"] = p.Value }));
                }
                foreach (var p in oo.Properties().Where(p => no[p.Name] == null))
                    events.Add(Ev(area, "remove", p.Name, $"Cleared {p.Name}", p.Value));
            }
        }

        private static Dictionary<string, JToken> Index(JArray arr)
        {
            var map = new Dictionary<string, JToken>();
            for (var i = 0; i < arr.Count; i++)
            {
                var k = KeyOf(arr[i], i);
                var n = 2; var key = k;
                while (map.ContainsKey(key)) key = k + "~" + n++;
                map[key] = arr[i];
            }
            return map;
        }

        private static JObject Changes(JToken old, JToken now)
        {
            var result = new JObject();
            if (old is JObject o && now is JObject n)
                foreach (var name in o.Properties().Select(p => p.Name).Union(n.Properties().Select(p => p.Name)).Distinct())
                    if (!JToken.DeepEquals(o[name], n[name])) result[name] = new JObject { ["from"] = o[name], ["to"] = n[name] };
            return result;
        }

        private static string ChangedFields(JToken old, JToken now)
        {
            var names = Changes(old, now).Properties().Select(p => p.Name).Take(4).ToList();
            return names.Count == 0 ? "" : " (" + string.Join(", ", names) + ")";
        }

        // ---------------- Documents ----------------

        // Called by DocumentStorageService.AddDocument. The source file may be deleted straight after
        // (supplier downloads are), so it is read here, then uploaded in the background.
        public static void DocumentAdded(string sourcePath, StoredDocument doc)
        {
            if (!Active) return;
            try
            {
                var size = new FileInfo(sourcePath).Length;
                Enqueue(new List<EventDto> { Ev("documents", "upload", doc.Id,
                    $"Uploaded document: {doc.Name} ({doc.Category}, {(size < 1048576 ? (size / 1024) + " KB" : (size / 1048576.0).ToString("0.0") + " MB")})",
                    JObject.FromObject(new { doc.Name, doc.Category, doc.OriginalFileName, sizeBytes = size })) });
                if (size > MaxUploadBytes) return;
                var bytes = File.ReadAllBytes(sourcePath);
                var name = doc.OriginalFileName ?? doc.Name; var category = doc.Category;
                _ = Task.Run(() => SessionService.UploadTestFile(bytes, name, "documents", category));
            }
            catch { }
        }

        // ---------------- Sending ----------------

        private static void Enqueue(List<EventDto> events)
        {
            if (events.Count == 0) return;
            lock (_lock)
            {
                _queue.AddRange(events);
                if (_queue.Count > MaxQueue) _queue.RemoveRange(0, _queue.Count - MaxQueue);
            }
        }

        private static async void Flush()
        {
            if (_sending || !Active || !SessionService.IsSignedIn) return;
            List<EventDto> batch;
            lock (_lock)
            {
                if (_queue.Count == 0) return;
                batch = _queue.Take(100).ToList();
                _queue.RemoveRange(0, batch.Count);
            }
            _sending = true;
            try
            {
                var (ok, _, json) = await SessionService.ApiPost("/api/account/test/events", new { events = batch, appVersion = AppInfo.Current.VersionString });
                if (ok && json?["active"]?.Value<bool>() == false) { SetActive(false); return; }   // the flag was removed
                if (!ok) lock (_lock) { _queue.InsertRange(0, batch); if (_queue.Count > MaxQueue) _queue.RemoveRange(MaxQueue, _queue.Count - MaxQueue); }
            }
            catch { lock (_lock) { _queue.InsertRange(0, batch); } }
            finally { _sending = false; }
        }
    }
}
