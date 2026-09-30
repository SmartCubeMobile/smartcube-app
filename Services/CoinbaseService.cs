using Newtonsoft.Json.Linq;
using SmartCubeMobile.MockData;
using System.Security.Cryptography;
using System.Text;

namespace SmartCubeMobile.Services
{
    public static class CoinbaseService
    {
        private static readonly HttpClient _http = new() { Timeout = TimeSpan.FromSeconds(20) };

        public static async Task<List<MockCryptoHolding>> GetHoldingsAsync(string apiKeyName, string privateKeyPem, string label = "Coinbase")
        {
            var keyBytes = ParseKey(privateKeyPem);

            var accounts = await CallApi(apiKeyName, keyBytes, "GET",
                "/api/v3/brokerage/accounts", "?limit=250");

            var acctArray = accounts?["accounts"] as JArray;
            if (acctArray == null) return new();

            var parsed = new List<(string currency, string uuid, string name, decimal total, List<MockCryptoTransaction> txs)>();

            foreach (var acct in acctArray)
            {
                var currency = acct["currency"]?.ToString() ?? "";
                var uuid = acct["uuid"]?.ToString() ?? "";
                var availStr = acct["available_balance"]?["value"]?.ToString() ?? "0";
                var holdStr = acct["hold"]?["value"]?.ToString() ?? "0";

                if (!decimal.TryParse(availStr, System.Globalization.NumberStyles.Any,
                    System.Globalization.CultureInfo.InvariantCulture, out var available))
                    continue;
                if (!decimal.TryParse(holdStr, System.Globalization.NumberStyles.Any,
                    System.Globalization.CultureInfo.InvariantCulture, out var hold))
                    hold = 0;

                var total = available + hold;
                if (currency == "GBP" || currency == "USD" || currency == "EUR") continue;

                var transactions = new List<MockCryptoTransaction>();
                try
                {
                    transactions = await GetTransactionsAsync(apiKeyName, keyBytes, uuid, currency);
                }
                catch { }

                if (total <= 0 && transactions.Count == 0) continue;

                var name = acct["name"]?.ToString() ?? currency;
                if (name.EndsWith(" Wallet")) name = name.Replace(" Wallet", "");
                parsed.Add((currency, uuid, name, total, transactions));
            }

            var allSymbols = parsed.Select(p => p.currency).Distinct();
            var prices = await CryptoMarketService.GetBatchPricesAsync(allSymbols);

            var holdings = new List<MockCryptoHolding>();
            foreach (var (currency, uuid, name, total, transactions) in parsed)
            {
                prices.TryGetValue(currency, out var pricePair);
                var price = pricePair.Price;
                var change = pricePair.Change24h;

                // Coinbase gives the GBP value of most transactions; the rest are priced by date.
                await HistoricalPriceService.FillPricesAsync(transactions, price);

                holdings.Add(new MockCryptoHolding
                {
                    Symbol = currency,
                    Name = name,
                    Quantity = total,
                    PriceGBP = price,
                    PriceUSD = price * 1.27m,
                    Change24h = change,
                    WalletLabel = label,
                    WalletAddress = $"{label} account",
                    Network = "coinbase",
                    Transactions = transactions,
                });
            }

            return holdings;
        }

        private static async Task<List<MockCryptoTransaction>> GetTransactionsAsync(
            string apiKeyName, byte[] keyBytes, string accountId, string symbol)
        {
            var all = new List<MockCryptoTransaction>();
            string nextUri = null;

            do
            {
                var path = nextUri ?? $"/v2/accounts/{accountId}/transactions";
                var query = nextUri != null ? "" : "?limit=100&order=desc";

                var data = await CallApi(apiKeyName, keyBytes, "GET", path, query);
                var txArray = data?["data"] as JArray;
                if (txArray == null) break;

                foreach (var tx in txArray)
                {
                    var type = tx["type"]?.ToString() ?? "";
                    var amountStr = tx["amount"]?["amount"]?.ToString() ?? "0";
                    var dateStr = tx["created_at"]?.ToString();

                    if (!decimal.TryParse(amountStr, System.Globalization.NumberStyles.Any,
                        System.Globalization.CultureInfo.InvariantCulture, out var amount))
                        continue;

                    DateTime date = DateTime.UtcNow;
                    if (dateStr != null)
                        DateTime.TryParse(dateStr, System.Globalization.CultureInfo.InvariantCulture,
                            System.Globalization.DateTimeStyles.RoundtripKind, out date);

                    var nativeStr = tx["native_amount"]?["amount"]?.ToString() ?? "0";
                    decimal.TryParse(nativeStr, System.Globalization.NumberStyles.Any,
                        System.Globalization.CultureInfo.InvariantCulture, out var nativeAmount);

                    var txType = type switch
                    {
                        "buy" => "Buy",
                        "sell" => "Sell",
                        "fiat_deposit" or "interest" or "staking_reward" => "Receive",
                        "fiat_withdrawal" => "Withdraw",
                        "send" => amount < 0 ? "Send" : "Receive",
                        "trade" or "exchange_deposit" or "exchange_withdrawal" => "Swap",
                        _ => amount >= 0 ? "Receive" : "Send",
                    };

                    string swapFor = null;
                    if (type == "trade")
                    {
                        var tradeId = tx["trade"]?["id"]?.ToString();
                        if (!string.IsNullOrEmpty(tradeId))
                            swapFor = tx["trade"]?["display_input_currency"]?.ToString()
                                ?? tx["trade"]?["input_amount"]?["currency"]?.ToString();
                        if (string.IsNullOrEmpty(swapFor))
                            swapFor = amount > 0 ? "GBP" : null;
                    }

                    var fromAddr = tx["from"]?["resource"]?.ToString() ?? tx["network"]?["from"]?.ToString() ?? "";
                    var toAddr = tx["to"]?["resource"]?.ToString() ?? tx["network"]?["to"]?.ToString() ?? "";
                    if (type == "send" && amount < 0)
                        toAddr = tx["to"]?["address"]?.ToString() ?? toAddr;
                    if (type == "send" && amount > 0)
                        fromAddr = tx["from"]?["address"]?.ToString() ?? fromAddr;

                    all.Add(new MockCryptoTransaction
                    {
                        Symbol = symbol,
                        Type = txType,
                        Quantity = Math.Abs(amount),
                        Date = date,
                        Hash = tx["network"]?["hash"]?.ToString() ?? tx["id"]?.ToString() ?? "",
                        PriceAtTime = nativeAmount != 0 && amount != 0
                            ? Math.Abs(nativeAmount / amount) : 0,
                        PriceSource = nativeAmount != 0 && amount != 0 ? HistoricalPriceService.SourceExchange : null,
                        FromAddress = string.IsNullOrEmpty(fromAddr) ? null : fromAddr,
                        ToAddress = string.IsNullOrEmpty(toAddr) ? null : toAddr,
                        SwapFor = swapFor,
                    });
                }

                var nextPath = data?["pagination"]?["next_uri"]?.ToString();
                nextUri = string.IsNullOrEmpty(nextPath) ? null : nextPath;

            } while (nextUri != null);

            return all;
        }

        private static async Task<JObject> CallApi(string apiKeyName, byte[] keyBytes,
            string method, string path, string query = "")
        {
            var jwt = CreateJwt(apiKeyName, keyBytes, method, path);
            var url = $"https://api.coinbase.com{path}{query}";

            var request = new HttpRequestMessage(
                method == "GET" ? HttpMethod.Get : HttpMethod.Post, url);
            request.Headers.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", jwt);

            var resp = await _http.SendAsync(request);
            var json = await resp.Content.ReadAsStringAsync();

            if (!resp.IsSuccessStatusCode)
                throw new Exception($"Coinbase API error {resp.StatusCode}: {json}");

            return JObject.Parse(json);
        }

        private static byte[] ParseKey(string rawKey)
        {
            var b64 = ExtractBase64(rawKey);
            return Convert.FromBase64String(b64);
        }

        private static string CreateJwt(string keyName, byte[] keyBytes, string method, string path)
        {
            using var ecdsa = ECDsa.Create();
            ecdsa.ImportECPrivateKey(keyBytes, out _);

            var now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            var uri = $"{method} api.coinbase.com{path}";

            var headerJson = $"{{\"alg\":\"ES256\",\"typ\":\"JWT\",\"kid\":\"{keyName}\",\"nonce\":\"{Guid.NewGuid():N}\"}}";
            var payloadJson = $"{{\"iss\":\"coinbase-cloud\",\"sub\":\"{keyName}\",\"nbf\":{now},\"exp\":{now + 120},\"uri\":\"{uri}\"}}";

            var headerB64 = Base64UrlEncode(Encoding.UTF8.GetBytes(headerJson));
            var payloadB64 = Base64UrlEncode(Encoding.UTF8.GetBytes(payloadJson));

            var dataToSign = Encoding.UTF8.GetBytes($"{headerB64}.{payloadB64}");
            var signature = ecdsa.SignData(dataToSign, HashAlgorithmName.SHA256);

            return $"{headerB64}.{payloadB64}.{Base64UrlEncode(signature)}";
        }

        private static string ExtractBase64(string raw)
        {
            var s = raw.Replace(@"\n", " ").Replace("\r\n", " ").Replace("\n", " ").Replace("\r", " ");

            var b64 = new string(s.Where(c =>
                (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') ||
                (c >= '0' && c <= '9') || c == '+' || c == '/' || c == '='
            ).ToArray());

            if (string.IsNullOrEmpty(b64))
                throw new Exception("No key data found. Paste the full private key.");

            if (b64.StartsWith("BEGIN"))
                b64 = b64.Substring(b64.IndexOf("KEY") + 3);
            if (b64.Contains("END"))
                b64 = b64.Substring(0, b64.IndexOf("END"));

            return b64.Trim();
        }

        private static string Base64UrlEncode(byte[] data)
        {
            return Convert.ToBase64String(data)
                .TrimEnd('=')
                .Replace('+', '-')
                .Replace('/', '_');
        }
    }
}
