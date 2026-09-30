using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SmartCubeMobile.MockData;
using System.Globalization;
using System.Net.Http.Headers;

namespace SmartCubeMobile.Services
{
    public class MonzoService
    {
        private const string ApiBase = "https://api.monzo.com";

        private readonly HttpClient _http;
        private string _accessToken;
        private string _refreshToken;
        private string _clientId;
        private string _clientSecret;

        public bool IsConnected => !string.IsNullOrEmpty(_accessToken);
        public string LastError { get; private set; }

        public MonzoService()
        {
            _http = new HttpClient();
            _http.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public void SetCredentials(string clientId, string clientSecret)
        {
            _clientId = clientId;
            _clientSecret = clientSecret;
        }

        public void SetAccessToken(string accessToken, string refreshToken = null)
        {
            _accessToken = accessToken;
            _refreshToken = refreshToken;
            _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);
        }

        public async Task<bool> ExchangeAuthCode(string authCode, string redirectUri = "http://localhost:3000/callback")
        {
            if (string.IsNullOrEmpty(_clientId) || string.IsNullOrEmpty(_clientSecret))
            {
                LastError = "Client ID and Client Secret are required to exchange an auth code.";
                return false;
            }

            try
            {
                var content = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("grant_type", "authorization_code"),
                    new KeyValuePair<string, string>("client_id", _clientId),
                    new KeyValuePair<string, string>("client_secret", _clientSecret),
                    new KeyValuePair<string, string>("redirect_uri", redirectUri),
                    new KeyValuePair<string, string>("code", authCode),
                });

                var response = await _http.PostAsync($"{ApiBase}/oauth2/token", content);
                var body = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    var err = TryParseError(body);
                    LastError = $"Auth code exchange failed ({response.StatusCode}): {err}";
                    return false;
                }

                var json = JObject.Parse(body);
                _accessToken = json["access_token"]?.ToString();
                _refreshToken = json["refresh_token"]?.ToString();
                _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);
                return true;
            }
            catch (Exception ex)
            {
                LastError = $"Auth code exchange error: {ex.Message}";
                return false;
            }
        }

        public async Task<bool> RefreshAccessToken()
        {
            if (string.IsNullOrEmpty(_refreshToken) || string.IsNullOrEmpty(_clientId))
            {
                LastError = "No refresh token or client credentials available.";
                return false;
            }

            try
            {
                var content = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("grant_type", "refresh_token"),
                    new KeyValuePair<string, string>("client_id", _clientId),
                    new KeyValuePair<string, string>("client_secret", _clientSecret),
                    new KeyValuePair<string, string>("refresh_token", _refreshToken),
                });

                var response = await _http.PostAsync($"{ApiBase}/oauth2/token", content);
                if (!response.IsSuccessStatusCode)
                {
                    LastError = $"Token refresh failed: {response.StatusCode}";
                    return false;
                }

                var json = JObject.Parse(await response.Content.ReadAsStringAsync());
                _accessToken = json["access_token"]?.ToString();
                _refreshToken = json["refresh_token"]?.ToString() ?? _refreshToken;
                _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);
                return true;
            }
            catch (Exception ex)
            {
                LastError = $"Token refresh error: {ex.Message}";
                return false;
            }
        }

        public async Task<bool> TestConnection()
        {
            try
            {
                var json = await GetJson("/ping/whoami");
                if (json == null) return false;

                var authenticated = json["authenticated"]?.Value<bool>() ?? false;
                if (!authenticated)
                {
                    LastError = "Token is valid but not authenticated. Check your Monzo app — you may need to approve access.";
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                LastError = $"Connection test error: {ex.Message}";
                return false;
            }
        }

        public async Task<List<MockAccount>> GetAccounts()
        {
            var result = new List<MockAccount>();
            try
            {
                var json = await GetJson("/accounts?account_type=uk_retail");
                if (json == null) return result;

                var accounts = json["accounts"] as JArray;
                if (accounts == null) return result;

                foreach (var acc in accounts)
                {
                    if (acc["closed"]?.Value<bool>() == true) continue;

                    var accountId = acc["id"]?.ToString();
                    var balance = await GetBalance(accountId);
                    var sortCode = acc["sort_code"]?.ToString();
                    var accNum = acc["account_number"]?.ToString();

                    result.Add(new MockAccount
                    {
                        Institution = "Monzo",
                        AccountName = acc["description"]?.ToString() ?? MapAccountType(acc["type"]?.ToString()),
                        SortCode = FormatSortCode(sortCode),
                        AccountNumber = MaskAccountNumber(accNum),
                        TotalBalance = balance.Total,
                        AvailableBalance = balance.Available,
                        AccountType = MapAccountType(acc["type"]?.ToString()),
                        CurrencyCode = "GBP",
                    });
                }
            }
            catch (Exception ex)
            {
                LastError = $"GetAccounts error: {ex.Message}";
            }
            return result;
        }

        public async Task<List<MockTransaction>> GetTransactions(DateTime? from = null, DateTime? to = null)
        {
            var result = new List<MockTransaction>();
            try
            {
                var accountIds = await GetAccountIds();
                var since = (from ?? DateTime.Now.AddDays(-30)).ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ");

                foreach (var accountId in accountIds)
                {
                    var json = await GetJson($"/transactions?account_id={accountId}&since={since}&expand[]=merchant");
                    if (json == null) continue;

                    var txns = json["transactions"] as JArray;
                    if (txns == null) continue;

                    foreach (var t in txns)
                    {
                        var amount = t["amount"]?.Value<decimal>() ?? 0;
                        var amountGbp = amount / 100m;

                        if (amountGbp == 0 && t["decline_reason"] != null) continue;

                        var merchant = t["merchant"] as JObject;
                        var description = merchant?["name"]?.ToString()
                            ?? t["description"]?.ToString()
                            ?? t["notes"]?.ToString()
                            ?? "Unknown";

                        var category = t["category"]?.ToString() ?? "general";

                        result.Add(new MockTransaction
                        {
                            Date = DateTime.Parse(t["created"]?.ToString() ?? DateTime.Now.ToString()),
                            Description = description,
                            Category = MapCategory(category),
                            Amount = amountGbp,
                            Balance = (t["account_balance"]?.Value<decimal>() ?? 0) / 100m,
                            AccountName = "Monzo",
                            Type = MapTransactionType(t["scheme"]?.ToString(), amountGbp),
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                LastError = $"GetTransactions error: {ex.Message}";
            }
            return result.OrderByDescending(t => t.Date).ToList();
        }

        public string GetAccessToken() => _accessToken;
        public string GetRefreshToken() => _refreshToken;

        public event Action TokensRefreshed;

        private async Task<JObject> GetJson(string endpoint)
        {
            var response = await _http.GetAsync($"{ApiBase}{endpoint}");

            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                if (await RefreshAccessToken())
                {
                    TokensRefreshed?.Invoke();
                    response = await _http.GetAsync($"{ApiBase}{endpoint}");
                }
                else
                {
                    LastError = "Monzo access token expired. Refresh failed — re-authorise via Monzo developer portal.";
                    return null;
                }
            }

            if (!response.IsSuccessStatusCode)
            {
                var body = await response.Content.ReadAsStringAsync();
                var err = TryParseError(body);

                if (response.StatusCode == System.Net.HttpStatusCode.Forbidden)
                    LastError = "Monzo requires you to approve API access in the Monzo app. Check for a notification.";
                else if ((int)response.StatusCode == 429)
                    LastError = "Rate limited by Monzo. Wait a moment and try again.";
                else
                    LastError = $"Monzo API error {(int)response.StatusCode}: {err}";

                return null;
            }

            return JObject.Parse(await response.Content.ReadAsStringAsync());
        }

        private async Task<(decimal Total, decimal Available)> GetBalance(string accountId)
        {
            try
            {
                var json = await GetJson($"/balance?account_id={accountId}");
                if (json == null) return (0, 0);

                var balance = (json["balance"]?.Value<decimal>() ?? 0) / 100m;
                var spendToday = (json["spend_today"]?.Value<decimal>() ?? 0) / 100m;
                return (balance, balance);
            }
            catch
            {
                return (0, 0);
            }
        }

        private async Task<List<string>> GetAccountIds()
        {
            var json = await GetJson("/accounts?account_type=uk_retail");
            var accounts = json?["accounts"] as JArray;
            return accounts?
                .Where(a => a["closed"]?.Value<bool>() != true)
                .Select(a => a["id"]?.ToString())
                .Where(id => id != null)
                .ToList() ?? new List<string>();
        }

        private static string FormatSortCode(string raw)
        {
            if (string.IsNullOrEmpty(raw) || raw.Length != 6) return raw ?? "";
            return $"{raw[..2]}-{raw[2..4]}-{raw[4..6]}";
        }

        private static string MaskAccountNumber(string raw)
        {
            if (string.IsNullOrEmpty(raw)) return "****";
            return raw.Length > 4 ? $"****{raw[^4..]}" : raw;
        }

        private static string MapAccountType(string monzoType)
        {
            return monzoType switch
            {
                "uk_retail" => "Current",
                "uk_retail_joint" => "Joint Current",
                "uk_prepaid" => "Prepaid",
                _ => "Current",
            };
        }

        private static string MapCategory(string monzoCategory)
        {
            return monzoCategory switch
            {
                "groceries" => "Groceries",
                "eating_out" => "Eating Out",
                "transport" => "Transport",
                "shopping" => "Shopping",
                "entertainment" => "Entertainment",
                "bills" => "Bills",
                "expenses" => "Expenses",
                "finances" => "Finance",
                "cash" => "Cash",
                "holidays" => "Holidays",
                "personal_care" => "Health",
                "family" => "Family",
                "general" => "General",
                "income" => "Income",
                "savings" => "Savings",
                "transfers" => "Transfer",
                _ => monzoCategory ?? "General",
            };
        }

        private static string MapTransactionType(string scheme, decimal amount)
        {
            if (amount > 0) return "BACS";
            return scheme switch
            {
                "mastercard" => "Card",
                "p2p_payment" => "TFR",
                "payport_faster_payments" => "FP",
                "bacs" => "DD",
                _ => "Card",
            };
        }

        private static string TryParseError(string body)
        {
            try
            {
                var obj = JObject.Parse(body);
                return obj["message"]?.ToString() ?? obj["error_description"]?.ToString() ?? obj["error"]?.ToString() ?? body;
            }
            catch
            {
                return body?.Length > 200 ? body[..200] : body ?? "Unknown error";
            }
        }
    }
}
