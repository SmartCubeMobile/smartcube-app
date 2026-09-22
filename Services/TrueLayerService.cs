using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SmartCubeMobile.MockData;
using System.Globalization;
using System.Net.Http.Headers;

namespace SmartCubeMobile.Services
{
    public class TrueLayerService
    {
        private const string DataApiBase = "https://api.truelayer.com/data/v1";
        private const string AuthBase = "https://auth.truelayer.com";

        private readonly HttpClient _http;
        private string _accessToken;
        private string _refreshToken;
        private string _clientId;
        private string _clientSecret;

        public bool IsConnected => !string.IsNullOrEmpty(_accessToken);
        public string LastError { get; private set; }

        public TrueLayerService()
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

                var response = await _http.PostAsync($"{AuthBase}/connect/token", content);
                var body = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    LastError = $"Auth code exchange failed ({response.StatusCode}): {body}";
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

        public string GetAccessToken() => _accessToken;
        public string GetRefreshToken() => _refreshToken;

        public async Task<bool> RefreshAccessToken()
        {
            if (string.IsNullOrEmpty(_refreshToken) || string.IsNullOrEmpty(_clientId))
                return false;

            try
            {
                var content = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("grant_type", "refresh_token"),
                    new KeyValuePair<string, string>("client_id", _clientId),
                    new KeyValuePair<string, string>("client_secret", _clientSecret),
                    new KeyValuePair<string, string>("refresh_token", _refreshToken),
                });

                var response = await _http.PostAsync($"{AuthBase}/connect/token", content);
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

        public async Task<List<MockAccount>> GetAccounts()
        {
            var result = new List<MockAccount>();
            try
            {
                var json = await GetJson("/accounts");
                if (json == null) return result;

                var accounts = json["results"] as JArray;
                if (accounts == null) return result;

                foreach (var acc in accounts)
                {
                    var accountId = acc["account_id"]?.ToString();
                    var balance = await GetBalance(accountId);

                    result.Add(new MockAccount
                    {
                        Institution = acc["provider"]?["display_name"]?.ToString() ?? "Unknown",
                        AccountName = acc["display_name"]?.ToString() ?? acc["account_type"]?.ToString() ?? "Account",
                        SortCode = FormatSortCode(acc["account_number"]?["sort_code"]?.ToString()),
                        AccountNumber = MaskAccountNumber(acc["account_number"]?["number"]?.ToString()),
                        TotalBalance = balance.Total,
                        AvailableBalance = balance.Available,
                        AccountType = MapAccountType(acc["account_type"]?.ToString()),
                        CurrencyCode = acc["currency"]?.ToString() ?? "GBP",
                    });
                }
            }
            catch (Exception ex)
            {
                LastError = $"GetAccounts error: {ex.Message}";
            }
            return result;
        }

        public async Task<List<MockTransaction>> GetTransactions(string accountId = null, DateTime? from = null, DateTime? to = null)
        {
            var result = new List<MockTransaction>();
            try
            {
                var accounts = accountId != null
                    ? new List<string> { accountId }
                    : await GetAccountIds();

                var fromDate = (from ?? DateTime.Now.AddDays(-30)).ToString("yyyy-MM-dd");
                var toDate = (to ?? DateTime.Now).ToString("yyyy-MM-dd");

                foreach (var accId in accounts)
                {
                    var json = await GetJson($"/accounts/{accId}/transactions?from={fromDate}&to={toDate}");
                    if (json == null) continue;

                    var txns = json["results"] as JArray;
                    if (txns == null) continue;

                    var accName = await GetAccountName(accId);

                    foreach (var t in txns)
                    {
                        result.Add(new MockTransaction
                        {
                            Date = DateTime.Parse(t["timestamp"]?.ToString() ?? DateTime.Now.ToString()),
                            Description = t["description"]?.ToString() ?? t["merchant_name"]?.ToString() ?? "Unknown",
                            Category = MapCategory(t["transaction_category"]?.ToString()),
                            Amount = decimal.Parse(t["amount"]?.ToString() ?? "0", CultureInfo.InvariantCulture),
                            Balance = decimal.Parse(t["running_balance"]?["amount"]?.ToString() ?? "0", CultureInfo.InvariantCulture),
                            AccountName = accName,
                            Type = MapTransactionType(t["transaction_type"]?.ToString()),
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

        public async Task<List<MockTransaction>> GetCardTransactions(DateTime? from = null, DateTime? to = null)
        {
            var result = new List<MockTransaction>();
            try
            {
                var cardIds = await GetCardIds();
                var fromDate = (from ?? DateTime.Now.AddDays(-30)).ToString("yyyy-MM-dd");
                var toDate = (to ?? DateTime.Now).ToString("yyyy-MM-dd");

                foreach (var cardId in cardIds)
                {
                    var json = await GetJson($"/cards/{cardId}/transactions?from={fromDate}&to={toDate}");
                    if (json == null) continue;

                    var txns = json["results"] as JArray;
                    if (txns == null) continue;

                    var cardName = await GetCardName(cardId);

                    foreach (var t in txns)
                    {
                        result.Add(new MockTransaction
                        {
                            Date = DateTime.Parse(t["timestamp"]?.ToString() ?? DateTime.Now.ToString()),
                            Description = t["description"]?.ToString() ?? t["merchant_name"]?.ToString() ?? "Unknown",
                            Category = MapCategory(t["transaction_category"]?.ToString()),
                            Amount = decimal.Parse(t["amount"]?.ToString() ?? "0", CultureInfo.InvariantCulture),
                            Balance = decimal.Parse(t["running_balance"]?["amount"]?.ToString() ?? "0", CultureInfo.InvariantCulture),
                            AccountName = cardName,
                            Type = MapTransactionType(t["transaction_type"]?.ToString()),
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                LastError = $"GetCardTransactions error: {ex.Message}";
            }
            return result.OrderByDescending(t => t.Date).ToList();
        }

        private async Task<List<string>> GetCardIds()
        {
            var json = await GetJson("/cards");
            var cards = json?["results"] as JArray;
            return cards?.Select(c => c["account_id"]?.ToString()).Where(id => id != null).ToList()
                   ?? new List<string>();
        }

        private async Task<string> GetCardName(string cardId)
        {
            var json = await GetJson("/cards");
            var cards = json?["results"] as JArray;
            var card = cards?.FirstOrDefault(c => c["account_id"]?.ToString() == cardId);
            return card?["display_name"]?.ToString() ?? "Credit Card";
        }

        public async Task<List<MockAccount>> GetCards()
        {
            var result = new List<MockAccount>();
            try
            {
                var json = await GetJson("/cards");
                if (json == null) return result;

                var cards = json["results"] as JArray;
                if (cards == null) return result;

                foreach (var card in cards)
                {
                    var cardId = card["account_id"]?.ToString();
                    var balance = await GetCardBalance(cardId);

                    result.Add(new MockAccount
                    {
                        Institution = card["provider"]?["display_name"]?.ToString() ?? "Unknown",
                        AccountName = card["display_name"]?.ToString() ?? "Credit Card",
                        SortCode = "",
                        AccountNumber = MaskAccountNumber(card["partial_card_number"]?.ToString()),
                        TotalBalance = balance.Total,
                        AvailableBalance = balance.Available,
                        AccountType = "Credit Card",
                        CurrencyCode = card["currency"]?.ToString() ?? "GBP",
                    });
                }
            }
            catch (Exception ex)
            {
                LastError = $"GetCards error: {ex.Message}";
            }
            return result;
        }

        public async Task<bool> TestConnection()
        {
            try
            {
                var json = await GetJson("/accounts");
                if (json != null) return true;

                json = await GetJson("/me");
                if (json != null) return true;

                var body = await _http.GetAsync($"{DataApiBase}/accounts");
                var raw = await body.Content.ReadAsStringAsync();
                LastError = $"Auth failed ({body.StatusCode}). Your access token may have expired — generate a fresh one from TrueLayer and try again.";
                return false;
            }
            catch (HttpRequestException ex)
            {
                LastError = $"Network error: {ex.Message}";
                return false;
            }
            catch (Exception ex)
            {
                LastError = $"Connection error: {ex.Message}";
                return false;
            }
        }

        public event Action TokensRefreshed;

        private async Task<JObject> GetJson(string endpoint)
        {
            var response = await _http.GetAsync($"{DataApiBase}{endpoint}");

            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                if (await RefreshAccessToken())
                {
                    TokensRefreshed?.Invoke();
                    response = await _http.GetAsync($"{DataApiBase}{endpoint}");
                }
                else
                {
                    LastError = "Access token expired and no refresh token available. Generate a fresh access token from TrueLayer.";
                    return null;
                }
            }

            if (!response.IsSuccessStatusCode)
            {
                var body = await response.Content.ReadAsStringAsync();
                LastError = response.StatusCode == System.Net.HttpStatusCode.Forbidden
                    ? "Access denied — your token may not have the required permissions (accounts, balance, transactions)."
                    : $"API error {(int)response.StatusCode}: {response.ReasonPhrase}";
                return null;
            }

            return JObject.Parse(await response.Content.ReadAsStringAsync());
        }

        private async Task<(decimal Total, decimal Available)> GetBalance(string accountId)
        {
            try
            {
                var json = await GetJson($"/accounts/{accountId}/balance");
                var balance = (json?["results"] as JArray)?.FirstOrDefault();
                if (balance == null) return (0, 0);

                var current = decimal.Parse(balance["current"]?.ToString() ?? "0", CultureInfo.InvariantCulture);
                var available = decimal.Parse(balance["available"]?.ToString() ?? current.ToString(), CultureInfo.InvariantCulture);
                return (current, available);
            }
            catch
            {
                return (0, 0);
            }
        }

        private async Task<(decimal Total, decimal Available)> GetCardBalance(string cardId)
        {
            try
            {
                var json = await GetJson($"/cards/{cardId}/balance");
                var balance = (json?["results"] as JArray)?.FirstOrDefault();
                if (balance == null) return (0, 0);

                var current = decimal.Parse(balance["current"]?.ToString() ?? "0", CultureInfo.InvariantCulture);
                var available = decimal.Parse(balance["available"]?.ToString() ?? "0", CultureInfo.InvariantCulture);
                return (current, available);
            }
            catch
            {
                return (0, 0);
            }
        }

        private async Task<List<string>> GetAccountIds()
        {
            var json = await GetJson("/accounts");
            var accounts = json?["results"] as JArray;
            return accounts?.Select(a => a["account_id"]?.ToString()).Where(id => id != null).ToList()
                   ?? new List<string>();
        }

        private async Task<string> GetAccountName(string accountId)
        {
            var json = await GetJson("/accounts");
            var accounts = json?["results"] as JArray;
            var acc = accounts?.FirstOrDefault(a => a["account_id"]?.ToString() == accountId);
            return acc?["display_name"]?.ToString() ?? "Account";
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

        private static string MapAccountType(string tlType)
        {
            return tlType?.ToUpper() switch
            {
                "TRANSACTION" => "Current",
                "SAVINGS" => "Savings",
                "BUSINESS_TRANSACTION" => "Business",
                "BUSINESS_SAVINGS" => "Business Savings",
                _ => tlType ?? "Current",
            };
        }

        private static string MapCategory(string tlCategory)
        {
            return tlCategory?.ToUpper() switch
            {
                "PURCHASE" => "Shopping",
                "ATM" => "Cash",
                "BILL_PAYMENT" => "Bills",
                "DIRECT_DEBIT" => "Bills",
                "STANDING_ORDER" => "Bills",
                "TRANSFER" => "Transfer",
                "INTEREST" => "Income",
                "DIVIDEND" => "Income",
                "CREDIT" => "Income",
                "DEBIT" => "General",
                _ => tlCategory ?? "General",
            };
        }

        private static string MapTransactionType(string tlType)
        {
            return tlType?.ToUpper() switch
            {
                "DEBIT" => "Card",
                "CREDIT" => "BACS",
                "SDD" => "DD",
                "FPI" => "FP",
                _ => tlType ?? "Card",
            };
        }
    }
}
