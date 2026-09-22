using System.Net;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace SmartCubeMobile.Services
{
    public class SessionUser
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
        public string Plan { get; set; }
        public bool Subscriber { get; set; }
        public DateTime? Expires { get; set; }
        public DateTime? LastLogin { get; set; }
        public string SmartScanLicence { get; set; }
        public bool MustChangePassword { get; set; }
    }

    public static class SessionService
    {
        private static readonly string _deviceFile = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "SmartCube", "device.json");

        private static readonly HttpClient _http = CreateClient();

        private static HttpClient CreateClient()
        {
            var client = new HttpClient(new HttpClientHandler
            {
                UseCookies = true,
                CookieContainer = new CookieContainer(),
            })
            { Timeout = TimeSpan.FromSeconds(30) };
            try { client.DefaultRequestHeaders.Add("X-SmartCube-Version", AppInfo.Current.VersionString); }
            catch { }
            return client;
        }

        public static SessionUser Current { get; private set; }
        public static bool IsSignedIn => Current != null;

        public static string DisplayName =>
            Current?.FullName ?? Current?.Username ?? "";

        public static string InitialsFor(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) return "?";
            var parts = name.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            return parts.Length >= 2
                ? $"{parts[0][0]}{parts[^1][0]}".ToUpperInvariant()
                : parts[0][..Math.Min(2, parts[0].Length)].ToUpperInvariant();
        }

        public const string DefaultServer = "https://api.smartcubemobile.com";
        private const string ServerPrefKey = "ServerUrl";

        public static string ServerBase
        {
            get
            {
                var s = Microsoft.Maui.Storage.Preferences.Default.Get(ServerPrefKey, "");
                if (string.IsNullOrWhiteSpace(s)) s = DefaultServer;
                return s.Trim().TrimEnd('/');
            }
            set
            {
                var v = (value ?? "").Trim().TrimEnd('/');
                if (string.IsNullOrEmpty(v) || v == DefaultServer)
                    Microsoft.Maui.Storage.Preferences.Default.Remove(ServerPrefKey);
                else
                    Microsoft.Maui.Storage.Preferences.Default.Set(ServerPrefKey, v);
            }
        }

        public static string DeviceToken
        {
            get
            {
                try
                {
                    if (!File.Exists(_deviceFile)) return null;
                    return JObject.Parse(File.ReadAllText(_deviceFile))["token"]?.ToString();
                }
                catch { return null; }
            }
        }

        public static async Task<(bool Ok, string Error)> Login(string login, string password, bool rememberDevice)
        {
            var (ok, error, json) = await Post("/api/account/login", new { login, password, remember = rememberDevice });
            if (!ok) return (false, error);

            ApplySession(json);

            if (rememberDevice)
            {
                var (dOk, _, dJson) = await Post("/api/account/device/register", new { deviceName = Environment.MachineName });
                if (dOk && dJson["token"] != null)
                    SaveDeviceToken(dJson["token"].ToString());
            }
            return (true, null);
        }

        public static async Task<(bool Ok, string Error)> TryDeviceLogin()
        {
            var token = DeviceToken;
            if (string.IsNullOrEmpty(token)) return (false, null);

            var (ok, error, json) = await Post("/api/account/device/login", new { token });
            if (!ok)
            {
                if (error != null && !error.StartsWith("Could not reach"))
                    ClearDeviceToken();
                return (false, error);
            }
            ApplySession(json);
            return (true, null);
        }

        public static async Task<(bool Ok, string Error)> ChangePassword(string currentPassword, string newPassword)
        {
            var (ok, error, _) = await Post("/api/account/change-password", new { currentPassword, newPassword });
            if (ok && Current != null) Current.MustChangePassword = false;
            return (ok, error);
        }

        public static async Task Logout()
        {
            var token = DeviceToken;
            try { await Post("/api/account/device/revoke", new { token }); } catch { }
            ClearDeviceToken();
            Current = null;
        }

        private static void ApplySession(JObject json)
        {
            Current = new SessionUser
            {
                Username = json["username"]?.ToString(),
                Email = json["email"]?.ToString(),
                FullName = json["fullName"]?.Type == JTokenType.Null ? null : json["fullName"]?.ToString(),
                Plan = json["plan"]?.Type == JTokenType.Null ? null : json["plan"]?.ToString(),
                Subscriber = json["subscriber"]?.Value<bool>() ?? false,
                Expires = json["expires"]?.Type == JTokenType.Date ? json["expires"].Value<DateTime>() : null,
                LastLogin = json["lastLogin"]?.Type == JTokenType.Date ? json["lastLogin"].Value<DateTime>() : null,
                SmartScanLicence = json["smartScanLicence"]?.Type == JTokenType.Null ? null : json["smartScanLicence"]?.ToString(),
                MustChangePassword = json["mustChangePassword"]?.Value<bool>() ?? false,
            };

            var profile = UserProfileDataService.GetProfile();
            var changed = false;
            if (!string.IsNullOrEmpty(Current.SmartScanLicence) && string.IsNullOrEmpty(profile.SmartScanLicence))
            {
                profile.SmartScanLicence = Current.SmartScanLicence;
                profile.SmartScanPromptShown = true;
                changed = true;
            }
            if (!string.IsNullOrEmpty(Current.FullName) && string.IsNullOrEmpty(profile.FullName))
            {
                profile.FullName = Current.FullName;
                changed = true;
            }
            if (changed) UserProfileDataService.SaveProfile();
        }

        private static async Task<(bool Ok, string Error, JObject Json)> Post(string path, object body)
        {
            try
            {
                var content = new StringContent(JsonConvert.SerializeObject(body), Encoding.UTF8, "application/json");
                using var response = await _http.PostAsync($"{ServerBase}{path}", content);
                var text = await response.Content.ReadAsStringAsync();
                JObject json;
                try { json = JObject.Parse(text); }
                catch { return (false, $"Unexpected server response ({(int)response.StatusCode})", null); }

                if (!response.IsSuccessStatusCode || json["ok"]?.Value<bool>() != true)
                {
                    var err = json["error"]?.ToString()
                              ?? (json["errors"] as JArray)?.Select(e => e.ToString()).FirstOrDefault()
                              ?? $"Request failed ({(int)response.StatusCode})";
                    return (false, err, json);
                }
                return (true, null, json);
            }
            catch (Exception ex)
            {
                return (false, $"Could not reach the SmartCube server: {ex.Message}", null);
            }
        }

        private static void SaveDeviceToken(string token)
        {
            try
            {
                var dir = Path.GetDirectoryName(_deviceFile);
                if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
                File.WriteAllText(_deviceFile, JsonConvert.SerializeObject(new { token, saved = DateTime.UtcNow }));
            }
            catch { }
        }

        private static void ClearDeviceToken()
        {
            try { if (File.Exists(_deviceFile)) File.Delete(_deviceFile); } catch { }
        }
    }
}
