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
        public bool TwoFactorEnabled { get; set; }
    }

    public static class SessionService
    {
        private static readonly string _deviceFile = Path.Combine(
            SmartCubeMobile.Services.AppPaths.Local,
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

        // Demo mode: a made-up signed-in user, no server involved.
        public static void StartDemoSession()
        {
            if (!AppPaths.IsDemo) return;
            Current = new SessionUser
            {
                Username = DemoData.Username, Email = DemoData.Email, FullName = DemoData.FullName,
                Plan = "Premium", Subscriber = true, LastLogin = DateTime.UtcNow, TwoFactorEnabled = true,
            };
        }

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
                    return JObject.Parse(SecureFile.ReadAllText(_deviceFile))["token"]?.ToString();
                }
                catch { return null; }
            }
        }

        public static async Task<(bool Ok, string Error, bool TwoFactorRequired)> Login(string login, string password, bool rememberDevice, string code = null)
        {
            var (ok, error, json) = await Post("/api/account/login", new { login, password, remember = rememberDevice, code });
            if (!ok) return (false, error, json?["twoFactorRequired"]?.Value<bool>() ?? false);

            ApplySession(json);
            // Remembering the device now happens separately, once the user has chosen a PIN (RegisterDevice).
            return (true, null, false);
        }

        // ---- Remembered device + PIN ----

        public static bool DeviceHasPin
        {
            get
            {
                try
                {
                    if (!File.Exists(_deviceFile)) return false;
                    return JObject.Parse(SecureFile.ReadAllText(_deviceFile))["pin"]?.Value<bool>() ?? false;
                }
                catch { return false; }
            }
        }

        // Same rules as the server, so the user gets the message before anything is sent.
        public static string PinProblem(string pin)
        {
            if (string.IsNullOrEmpty(pin) || pin.Length < 4 || pin.Length > 6 || !pin.All(char.IsDigit))
                return "Your PIN must be 4 to 6 digits.";
            if (pin.Distinct().Count() == 1) return "Choose a PIN that isn't the same digit repeated.";
            const string up = "0123456789012345", down = "9876543210987654";
            if (up.Contains(pin) || down.Contains(pin)) return "Choose a PIN that isn't a simple sequence like 1234.";
            return null;
        }

        // Remember this device, protected by a PIN the server checks. Call after a password sign-in.
        public static async Task<(bool Ok, string Error)> RegisterDevice(string pin)
        {
            var (ok, error, json) = await Post("/api/account/device/register", new { deviceName = Environment.MachineName, pin });
            if (!ok || json?["token"] == null) return (false, error ?? "Could not remember this device.");
            SaveDeviceToken(json["token"].ToString(), hasPin: true);
            TrySetDeviceSlot(json["deviceKey"]?.ToString());
            return (true, null);
        }

        // Set or change the PIN on the device already remembered (user must be signed in).
        public static async Task<(bool Ok, string Error)> SetDevicePin(string pin)
        {
            var token = DeviceToken;
            if (string.IsNullOrEmpty(token)) return (false, "This device isn't remembered.");
            var (ok, error, json) = await Post("/api/account/device/pin", new { token, pin });
            if (ok)
            {
                SaveDeviceToken(token, hasPin: true);
                TrySetDeviceSlot(json?["deviceKey"]?.ToString());
            }
            return (ok, error);
        }

        // Stop remembering this PC locally (the saved token is deleted, so it can't be used again).
        public static void ForgetThisDevice()
        {
            ClearDeviceToken();
            try { KeyVault.RemoveDeviceSlot(); } catch { }
        }

        // ---- Key vault (one lock, three keys) ----

        private static void TrySetDeviceSlot(string deviceKey)
        {
            try { if (KeyVault.IsUnlocked && !string.IsNullOrEmpty(deviceKey)) KeyVault.SetDeviceSlot(deviceKey); }
            catch { }
        }

        // After the vault is unlocked: make sure a remembered PC with a PIN also has its "this PC" slot.
        public static async Task EnsureDeviceSlot()
        {
            try
            {
                if (!KeyVault.IsUnlocked || KeyVault.HasDeviceSlot) return;
                var token = DeviceToken;
                if (string.IsNullOrEmpty(token) || !DeviceHasPin) return;
                var (ok, _, json) = await Post("/api/account/device/key", new { token });
                if (ok) TrySetDeviceSlot(json?["deviceKey"]?.ToString());
            }
            catch { }
        }

        public class DeviceLoginResult
        {
            public bool Ok { get; set; }
            public string Error { get; set; }
            public bool PinRequired { get; set; }   // server wants the PIN (first try, or a wrong one)
            public bool NeedsPin { get; set; }      // signed in, but this device has no PIN yet
            public bool Forgotten { get; set; }     // device no longer remembered: use the password
            public int AttemptsLeft { get; set; }
            public string DeviceKey { get; set; }   // server half of the "this PC" vault slot (after a correct PIN)
        }

        // ---- Two-factor (authenticator app) ----

        public static async Task<(bool Ok, string Error, bool Enabled, int RecoveryCodesLeft)> GetTwoFactorStatus()
        {
            var (ok, error, json) = await Get("/api/account/2fa");
            if (!ok) return (false, error, false, 0);
            return (true, null, json["enabled"]?.Value<bool>() ?? false, json["recoveryCodesLeft"]?.Value<int>() ?? 0);
        }

        public static async Task<(bool Ok, string Error, string Key, byte[] QrPng)> StartTwoFactorSetup()
        {
            var (ok, error, json) = await Post("/api/account/2fa/setup", new { });
            if (!ok) return (false, error, null, null);
            byte[] png = null;
            try { png = await _http.GetByteArrayAsync($"{ServerBase}/api/account/2fa/qr"); } catch { }
            return (true, null, json["key"]?.ToString(), png);
        }

        public static async Task<(bool Ok, string Error, string[] RecoveryCodes)> EnableTwoFactor(string code)
        {
            var (ok, error, json) = await Post("/api/account/2fa/enable", new { code });
            if (!ok) return (false, error, null);
            if (Current != null) Current.TwoFactorEnabled = true;
            return (true, null, (json["recoveryCodes"] as JArray)?.Select(c => c.ToString()).ToArray() ?? Array.Empty<string>());
        }

        public static async Task<(bool Ok, string Error)> DisableTwoFactor(string code)
        {
            var (ok, error, _) = await Post("/api/account/2fa/disable", new { code });
            if (ok && Current != null) Current.TwoFactorEnabled = false;
            return (ok, error);
        }

        private static async Task<(bool Ok, string Error, JObject Json)> Get(string path)
        {
            try
            {
                using var response = await _http.GetAsync($"{ServerBase}{path}");
                var text = await response.Content.ReadAsStringAsync();
                JObject json;
                try { json = JObject.Parse(text); }
                catch { return (false, $"Unexpected server response ({(int)response.StatusCode})", null); }
                if (!response.IsSuccessStatusCode || json["ok"]?.Value<bool>() != true)
                    return (false, json["error"]?.ToString() ?? $"Request failed ({(int)response.StatusCode})", json);
                return (true, null, json);
            }
            catch (Exception ex)
            {
                return (false, $"Could not reach the SmartCube server: {ex.Message}", null);
            }
        }

        public static async Task<DeviceLoginResult> TryDeviceLogin(string pin = null)
        {
            var token = DeviceToken;
            if (string.IsNullOrEmpty(token)) return new DeviceLoginResult { Forgotten = true };

            var (ok, error, json) = await Post("/api/account/device/login", new { token, pin });
            if (!ok)
            {
                var pinRequired = json?["pinRequired"]?.Value<bool>() ?? false;
                var unreachable = error != null && error.StartsWith("Could not reach");
                if (!pinRequired && !unreachable)
                    ClearDeviceToken();   // revoked, too many wrong PINs, or unknown
                return new DeviceLoginResult
                {
                    Error = error,
                    PinRequired = pinRequired,
                    Forgotten = !pinRequired && !unreachable,
                    AttemptsLeft = json?["attemptsLeft"]?.Value<int>() ?? 0,
                };
            }
            ApplySession(json);
            return new DeviceLoginResult
            {
                Ok = true,
                NeedsPin = json["needsPin"]?.Value<bool>() ?? false,
                DeviceKey = json["deviceKey"]?.Type == JTokenType.String ? json["deviceKey"].ToString() : null,
            };
        }

        public static async Task<(bool Ok, string Error)> ChangePassword(string currentPassword, string newPassword)
        {
            var (ok, error, _) = await Post("/api/account/change-password", new { currentPassword, newPassword });
            if (ok && Current != null) Current.MustChangePassword = false;
            // Re-lock the data key with the new password so it keeps opening the data on this PC.
            if (ok && KeyVault.IsUnlocked)
                try { await Task.Run(() => KeyVault.SetPassword(Current?.Username, newPassword)); } catch { }
            return (ok, error);
        }

        public static async Task Logout()
        {
            var token = DeviceToken;
            try { await Post("/api/account/device/revoke", new { token }); } catch { }
            ClearDeviceToken();
            try { KeyVault.RemoveDeviceSlot(); } catch { }
            KeyVault.Lock();   // the data key leaves memory; the password (or recovery key) is needed again
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
                TwoFactorEnabled = json["twoFactorEnabled"]?.Value<bool>() ?? false,
            };

            SyncProfileFromSession();
        }

        // Copies the Smart Scan licence and name from the account into the local profile. Must not run
        // while the data is locked: the profile would read as empty and could be saved over the real one.
        public static void SyncProfileFromSession()
        {
            if (Current == null) return;
            if (KeyVault.Exists && !KeyVault.IsUnlocked) return;

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

        // Shared, signed-in calls to the SmartCube server (same cookies as the login).
        public static Task<(bool Ok, string Error, JObject Json)> ApiGet(string path) => Get(path);
        public static Task<(bool Ok, string Error, JObject Json)> ApiPost(string path, object body) => Post(path, body);

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

        private static void SaveDeviceToken(string token, bool hasPin)
        {
            try
            {
                var dir = Path.GetDirectoryName(_deviceFile);
                if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
                // Needed before sign-in, so it stays on the Windows lock rather than the vault key.
                SecureFile.WriteAllTextDpapi(_deviceFile, JsonConvert.SerializeObject(new { token, pin = hasPin, saved = DateTime.UtcNow }));
            }
            catch { }
        }

        private static void ClearDeviceToken()
        {
            try { if (File.Exists(_deviceFile)) File.Delete(_deviceFile); } catch { }
        }
    }
}
