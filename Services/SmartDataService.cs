using Newtonsoft.Json;
using SmartCubeMobile.MockData;

namespace SmartCubeMobile.Services
{
    public class BankConnection
    {
        public string Provider { get; set; }
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public DateTime ConnectedAt { get; set; }
        public string Status { get; set; }
    }

    public static class SmartDataService
    {
        private static readonly Dictionary<string, TrueLayerService> _trueLayerInstances = new();
        private static readonly MonzoService _monzo = new(); // kept for legacy connection loading
        private static readonly List<BankConnection> _connections = new();
        private static string _lastError;
        private static readonly string _configPath = Path.Combine(
            SmartCubeMobile.Services.AppPaths.Local,
            "SmartCube", "connections.json");

        private static readonly string _transactionFilePath = @"D:\TransactionsCategories.txt";
        private static readonly string _accountsCachePath = Path.Combine(
            SmartCubeMobile.Services.AppPaths.Local,
            "SmartCube", "accounts_cache.json");
        private static readonly string _transactionsCachePath = Path.Combine(
            SmartCubeMobile.Services.AppPaths.Local,
            "SmartCube", "transactions_cache.json");

        public static bool HasLiveConnection => _trueLayerInstances.Values.Any(t => t.IsConnected) || _monzo.IsConnected;
        public static bool HasFileData => TransactionDataLoader.IsLoaded;
        public static string LastError => _lastError ?? _monzo.LastError ?? _trueLayerInstances.Values.FirstOrDefault(t => t.LastError != null)?.LastError;
        public static IReadOnlyList<BankConnection> Connections => _connections.AsReadOnly();

        private static bool _initialized;

        static SmartDataService()
        {
            _monzo.TokensRefreshed += () => UpdateSavedTokens("Monzo", _monzo.GetAccessToken(), _monzo.GetRefreshToken());
            LoadConnections();
            if (!AppPaths.IsDemo)
                try { TransactionDataLoader.LoadFromFile(_transactionFilePath); } catch { }
        }

        public static void ResetInitialized() => _initialized = false;

        public static async Task EnsureTokensFresh()
        {
            if (_initialized) return;
            _initialized = true;

            foreach (var kvp in _trueLayerInstances.ToList())
            {
                if (kvp.Value.IsConnected)
                {
                    var refreshed = await kvp.Value.RefreshAccessToken();
                    if (refreshed)
                        UpdateSavedTokens(kvp.Key, kvp.Value.GetAccessToken(), kvp.Value.GetRefreshToken());
                }
            }

            await ResolveBankNames();
            await CheckTokenExpiry();
        }

        private static async Task ResolveBankNames()
        {
            var toRename = _connections.Where(c => c.Provider == "TrueLayer").ToList();
            foreach (var conn in toRename)
            {
                if (!_trueLayerInstances.TryGetValue(conn.Provider, out var tl)) continue;

                string bankName = null;

                try
                {
                    var accounts = await tl.GetAccounts();
                    bankName = accounts.FirstOrDefault()?.Institution;
                }
                catch { }

                if (string.IsNullOrEmpty(bankName) || bankName == "Unknown")
                    bankName = ExtractBankFromToken(conn.AccessToken);

                if (!string.IsNullOrEmpty(bankName) && bankName != "Unknown")
                {
                    var newKey = bankName;
                    _trueLayerInstances.Remove(conn.Provider);
                    _trueLayerInstances[newKey] = tl;
                    tl.TokensRefreshed += () => UpdateSavedTokens(newKey, tl.GetAccessToken(), tl.GetRefreshToken());
                    conn.Provider = newKey;
                    SaveConnections();
                }
            }
        }

        private static string ExtractBankFromToken(string token)
        {
            if (string.IsNullOrEmpty(token)) return null;
            try
            {
                var parts = token.Split('.');
                if (parts.Length < 2) return null;
                var payload = parts[1];
                payload = payload.Replace('-', '+').Replace('_', '/');
                switch (payload.Length % 4)
                {
                    case 2: payload += "=="; break;
                    case 3: payload += "="; break;
                }
                var json = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(payload));
                var obj = Newtonsoft.Json.Linq.JObject.Parse(json);
                var connectorId = obj["connector_id"]?.ToString();
                if (string.IsNullOrEmpty(connectorId)) return null;
                var name = connectorId.Replace("ob-", "").Replace("-", " ");
                return System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(name);
            }
            catch { return null; }
        }

        private static async Task CheckTokenExpiry()
        {
            foreach (var conn in _connections)
            {
                if (!_trueLayerInstances.TryGetValue(conn.Provider, out var tl)) continue;
                if (!tl.IsConnected) continue;
                try
                {
                    var accounts = await tl.GetAccounts();
                    if (accounts.Count == 0)
                        conn.Status = "Expired";
                }
                catch
                {
                    conn.Status = "Expired";
                }
            }
        }

        private static void UpdateSavedTokens(string provider, string accessToken, string refreshToken)
        {
            var conn = _connections.FirstOrDefault(c => c.Provider == provider);
            if (conn == null) return;
            conn.AccessToken = accessToken;
            conn.RefreshToken = refreshToken ?? conn.RefreshToken;
            SaveConnections();
        }

        public static async Task<bool> ConnectMonzo(string authCode, string clientId, string clientSecret, string redirectUri = "http://localhost:3000/callback")
        {
            _lastError = null;
            _monzo.SetCredentials(clientId, clientSecret);

            var exchanged = await _monzo.ExchangeAuthCode(authCode, redirectUri);
            if (!exchanged)
            {
                _lastError = _monzo.LastError;
                return false;
            }

            var ok = await _monzo.TestConnection();
            if (!ok)
            {
                _lastError = _monzo.LastError;
                return false;
            }

            var existing = _connections.FirstOrDefault(c => c.Provider == "Monzo");
            if (existing != null)
                _connections.Remove(existing);

            _connections.Add(new BankConnection
            {
                Provider = "Monzo",
                AccessToken = _monzo.GetAccessToken(),
                RefreshToken = _monzo.GetRefreshToken(),
                ClientId = clientId,
                ClientSecret = clientSecret,
                ConnectedAt = DateTime.Now,
                Status = "Connected",
            });

            SaveConnections();
            return true;
        }

        public static async Task<bool> ConnectMonzoWithToken(string accessToken, string refreshToken = null,
            string clientId = null, string clientSecret = null)
        {
            _lastError = null;
            _monzo.SetAccessToken(accessToken, refreshToken);
            if (clientId != null)
                _monzo.SetCredentials(clientId, clientSecret);

            var ok = await _monzo.TestConnection();
            if (!ok)
            {
                _lastError = _monzo.LastError;
                return false;
            }

            var existing = _connections.FirstOrDefault(c => c.Provider == "Monzo");
            if (existing != null)
                _connections.Remove(existing);

            _connections.Add(new BankConnection
            {
                Provider = "Monzo",
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                ClientId = clientId,
                ClientSecret = clientSecret,
                ConnectedAt = DateTime.Now,
                Status = "Connected",
            });

            SaveConnections();
            return true;
        }

        public static async Task<bool> ConnectTrueLayerWithCode(string authCode, string redirectUri = "http://localhost:3000/callback")
        {
            _lastError = null;
            var tl = new TrueLayerService();

            var exchanged = await tl.ExchangeAuthCode(authCode, redirectUri);
            if (!exchanged)
            {
                _lastError = tl.LastError;
                return false;
            }

            var ok = await tl.TestConnection();
            if (!ok)
            {
                _lastError = tl.LastError;
                return false;
            }

            var accounts = await tl.GetAccounts();
            var institution = accounts.FirstOrDefault()?.Institution ?? "Bank";

            var providerKey = $"TrueLayer:{institution}";
            int suffix = 2;
            while (_trueLayerInstances.ContainsKey(providerKey) || _connections.Any(c => c.Provider == providerKey))
            {
                providerKey = $"TrueLayer:{institution}-{suffix}";
                suffix++;
            }

            tl.TokensRefreshed += () => UpdateSavedTokens(providerKey, tl.GetAccessToken(), tl.GetRefreshToken());
            _trueLayerInstances[providerKey] = tl;

            _connections.Add(new BankConnection
            {
                Provider = providerKey,
                AccessToken = tl.GetAccessToken(),
                RefreshToken = tl.GetRefreshToken(),
                ConnectedAt = DateTime.Now,
                Status = "Connected",
            });

            SaveConnections();
            return true;
        }

        public static async Task<bool> ConnectBank(string accessToken, string refreshToken = null,
            string clientId = null, string clientSecret = null, string provider = "TrueLayer")
        {
            _lastError = null;
            var tl = new TrueLayerService();
            tl.SetAccessToken(accessToken, refreshToken);

            var ok = await tl.TestConnection();
            if (!ok)
            {
                _lastError = tl.LastError;
                return false;
            }

            var bankName = provider;
            try
            {
                var accounts = await tl.GetAccounts();
                if (accounts.Count > 0 && !string.IsNullOrEmpty(accounts[0].Institution) && accounts[0].Institution != "Unknown")
                    bankName = accounts[0].Institution;
            }
            catch { }

            var providerKey = bankName;
            if (_trueLayerInstances.ContainsKey(providerKey))
            {
                int suffix = 2;
                while (_trueLayerInstances.ContainsKey($"{bankName}-{suffix}"))
                    suffix++;
                providerKey = $"{bankName}-{suffix}";
            }

            tl.TokensRefreshed += () => UpdateSavedTokens(providerKey, tl.GetAccessToken(), tl.GetRefreshToken());
            _trueLayerInstances[providerKey] = tl;

            _connections.Add(new BankConnection
            {
                Provider = providerKey,
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                ClientId = clientId,
                ConnectedAt = DateTime.Now,
                Status = "Connected",
            });

            SaveConnections();
            return ok;
        }

        public static void DisconnectBank(string provider)
        {
            _connections.RemoveAll(c => c.Provider == provider);
            _trueLayerInstances.Remove(provider);
            SaveConnections();
        }

        public static List<MockAccount> GetCachedAccounts()
        {
            try
            {
                if (File.Exists(_accountsCachePath))
                {
                    var json = SecureFile.ReadAllText(_accountsCachePath);
                    var cached = JsonConvert.DeserializeObject<List<MockAccount>>(json);
                    if (cached != null && cached.Count > 0)
                    {
                        var dupes = cached.GroupBy(a => a.AccountName).Where(g => g.Count() > 1);
                        foreach (var group in dupes)
                        {
                            foreach (var acc in group)
                                acc.AccountName = $"{acc.AccountName} ({acc.AccountType})";
                        }
                        return cached;
                    }
                }
            }
            catch { }
            return null;
        }

        public static List<MockTransaction> GetCachedTransactions()
        {
            try
            {
                if (File.Exists(_transactionsCachePath))
                {
                    var json = SecureFile.ReadAllText(_transactionsCachePath);
                    var cached = JsonConvert.DeserializeObject<List<MockTransaction>>(json);
                    if (cached != null && cached.Count > 0)
                        return cached;
                }
            }
            catch { }
            return null;
        }

        private static void SaveAccountsCache(List<MockAccount> accounts)
        {
            try
            {
                var dir = Path.GetDirectoryName(_accountsCachePath);
                if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
                SecureFile.WriteAllText(_accountsCachePath, JsonConvert.SerializeObject(accounts, Formatting.Indented));
            }
            catch { }
        }

        private static void SaveTransactionsCache(List<MockTransaction> transactions)
        {
            try
            {
                var dir = Path.GetDirectoryName(_transactionsCachePath);
                if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
                SecureFile.WriteAllText(_transactionsCachePath, JsonConvert.SerializeObject(transactions, Formatting.Indented));
            }
            catch { }
        }

        public static async Task<List<MockAccount>> GetAccounts()
        {
            var live = new List<MockAccount>();

            if (_monzo.IsConnected)
            {
                var monzoAccounts = await _monzo.GetAccounts();
                live.AddRange(monzoAccounts);
            }

            foreach (var tl in _trueLayerInstances.Values)
            {
                if (!tl.IsConnected) continue;
                var tlAccounts = await tl.GetAccounts();
                var tlCards = await tl.GetCards();
                live.AddRange(tlAccounts);
                live.AddRange(tlCards);
            }

            if (live.Count > 0)
            {
                var result = live
                    .GroupBy(a => $"{a.Institution}|{a.AccountName}|{a.AccountNumber}|{a.SortCode}")
                    .Select(g => g.First())
                    .ToList();

                var dupes = result.GroupBy(a => a.AccountName).Where(g => g.Count() > 1);
                foreach (var group in dupes)
                {
                    foreach (var acc in group)
                        acc.AccountName = $"{acc.AccountName} ({acc.AccountType})";
                }

                SaveAccountsCache(result);
                return result;
            }

            if (TransactionDataLoader.IsLoaded)
                return TransactionDataLoader.GetAccounts();

            return MockDataService.GetAccounts();
        }

        public static async Task<List<MockTransaction>> GetTransactions()
        {
            var live = new List<MockTransaction>();

            var accounts = await GetAccounts();
            var nameMap = new Dictionary<string, string>();
            foreach (var acc in accounts)
            {
                var baseKey = acc.AccountName;
                if (baseKey.EndsWith($" ({acc.AccountType})"))
                {
                    var original = baseKey[..^(acc.AccountType.Length + 3)];
                    nameMap[$"{original}|{acc.AccountType}"] = baseKey;
                }
            }

            if (_monzo.IsConnected)
            {
                var monzoTxns = await _monzo.GetTransactions();
                live.AddRange(monzoTxns);
                TransactionDatabase.SaveTransactions(monzoTxns, "Monzo");
            }

            foreach (var kvp in _trueLayerInstances)
            {
                if (!kvp.Value.IsConnected) continue;
                var tlTxns = await kvp.Value.GetTransactions();
                live.AddRange(tlTxns);
                TransactionDatabase.SaveTransactions(tlTxns, kvp.Key);

                var cardTxns = await kvp.Value.GetCardTransactions();
                foreach (var ct in cardTxns)
                {
                    var cardKey = $"{ct.AccountName}|Credit Card";
                    if (nameMap.TryGetValue(cardKey, out var mapped))
                        ct.AccountName = mapped;
                }
                live.AddRange(cardTxns);
                TransactionDatabase.SaveTransactions(cardTxns, $"{kvp.Key}_Cards");
            }

            foreach (var txn in live)
            {
                var currentKey = $"{txn.AccountName}|Current";
                if (nameMap.TryGetValue(currentKey, out var mapped))
                    txn.AccountName = mapped;
            }

            if (live.Count > 0)
            {
                var stored = TransactionDatabase.GetAllTransactions();
                var merged = new List<MockTransaction>(live);
                foreach (var s in stored)
                {
                    var key = $"{s.Date:yyyyMMdd}|{s.Description}|{s.Amount}|{s.AccountName}";
                    if (!live.Any(l => $"{l.Date:yyyyMMdd}|{l.Description}|{l.Amount}|{l.AccountName}" == key))
                        merged.Add(s);
                }
                var result = merged.OrderByDescending(t => t.Date).ToList();
                SaveTransactionsCache(result);
                return result;
            }

            var dbTransactions = TransactionDatabase.GetAllTransactions();
            if (dbTransactions.Count > 0)
                return dbTransactions;

            if (TransactionDataLoader.IsLoaded)
                return TransactionDataLoader.GetTransactions();

            return MockDataService.GetTransactions();
        }

        public static async Task<decimal> GetTotalBalance()
        {
            var accounts = await GetAccounts();
            return accounts.Sum(a => a.TotalBalance);
        }

        public static List<MockCryptoHolding> GetCryptoHoldings() => MockDataService.GetCryptoHoldings();
        public static decimal GetTotalCryptoValueGBP() => MockDataService.GetTotalCryptoValueGBP();
        public static List<MockInvestment> GetInvestments() => MockDataService.GetInvestments();
        public static decimal GetTotalInvestmentValueGBP() => MockDataService.GetTotalInvestmentValueGBP();
        public static List<MockUtilityBill> GetUtilityBills() => MockDataService.GetUtilityBills();
        public static List<MockMeterReading> GetMeterReadings() => MockDataService.GetMeterReadings();
        public static MockUserProfile GetUserProfile() => MockDataService.GetUserProfile();

        private static void LoadConnections()
        {
            try
            {
                if (!File.Exists(_configPath)) return;

                var json = SecureFile.ReadAllText(_configPath);
                var connections = JsonConvert.DeserializeObject<List<BankConnection>>(json);
                if (connections == null) return;

                _connections.AddRange(connections);

                foreach (var conn in connections.Where(c => c.Provider != "Monzo"))
                {
                    var tl = new TrueLayerService();
                    conn.ClientSecret = null;   // older versions saved the TrueLayer secret here; the server holds it now
                    tl.SetAccessToken(conn.AccessToken, conn.RefreshToken);
                    var key = conn.Provider;
                    tl.TokensRefreshed += () => UpdateSavedTokens(key, tl.GetAccessToken(), tl.GetRefreshToken());
                    _trueLayerInstances[key] = tl;
                }

                var monzo = connections.FirstOrDefault(c => c.Provider == "Monzo");
                if (monzo != null)
                {
                    if (monzo.ClientId != null)
                        _monzo.SetCredentials(monzo.ClientId, monzo.ClientSecret);
                    _monzo.SetAccessToken(monzo.AccessToken, monzo.RefreshToken);
                }
            }
            catch { }
        }

        private static void SaveConnections()
        {
            try
            {
                var dir = Path.GetDirectoryName(_configPath);
                if (!Directory.Exists(dir))
                    Directory.CreateDirectory(dir);

                var json = JsonConvert.SerializeObject(_connections, Formatting.Indented);
                SecureFile.WriteAllText(_configPath, json);
            }
            catch { }
        }
    }
}
