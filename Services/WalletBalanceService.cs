using Newtonsoft.Json.Linq;

namespace SmartCubeMobile.Services
{
    public static class WalletBalanceService
    {
        private static readonly HttpClient _http = new() { Timeout = TimeSpan.FromSeconds(60) };

        public static async Task<decimal?> GetBalanceAsync(string symbol, string address)
        {
            try
            {
                return symbol switch
                {
                    "BTC" => await GetBtcBalance(address),
                    "ETH" => await GetEthBalance(address),
                    "XRP" => await GetXrpBalance(address),
                    "SOL" => await GetSolBalance(address),
                    "ADA" => await GetAdaBalance(address),
                    "MATIC" => await GetMaticBalance(address),
                    "HBAR" => await GetHbarBalance(address),
                    "XLM" => await GetXlmBalance(address),
                    _ => null,
                };
            }
            catch
            {
                return null;
            }
        }

        private static async Task<decimal?> GetBtcBalance(string address)
        {
            var text = await _http.GetStringAsync($"https://blockchain.info/q/addressbalance/{address}");
            if (long.TryParse(text.Trim(), out var satoshis))
                return satoshis / 100_000_000m;
            return null;
        }

        public class TokenBalance
        {
            public string Symbol { get; set; }
            public string Name { get; set; }
            public decimal Balance { get; set; }
            public int Decimals { get; set; }
            public string ContractAddress { get; set; }
        }

        private static async Task<string> EthplorerGetAsync(string url)
        {
            for (int attempt = 0; attempt < 5; attempt++)
            {
                if (attempt > 0)
                    await Task.Delay((attempt + 1) * 6000);
                var resp = await _http.GetAsync(url);
                if ((int)resp.StatusCode == 429)
                    continue;
                resp.EnsureSuccessStatusCode();
                return await resp.Content.ReadAsStringAsync();
            }
            throw new HttpRequestException("Ethplorer rate limit exceeded after retries");
        }

        public static async Task<List<TokenBalance>> GetEthTokenBalancesAsync(string address)
        {
            var results = new List<TokenBalance>();
            var json = await EthplorerGetAsync($"https://api.ethplorer.io/getAddressInfo/{address}?apiKey=freekey");
            var obj = JObject.Parse(json);

            var ethBal = obj?["ETH"]?["balance"]?.Value<decimal>() ?? 0;
            if (ethBal > 0)
            {
                results.Add(new TokenBalance
                {
                    Symbol = "ETH",
                    Name = "Ethereum",
                    Balance = ethBal,
                    Decimals = 18,
                });
            }

            var tokens = obj?["tokens"] as JArray;
            if (tokens != null)
            {
                foreach (var tok in tokens)
                {
                    var info = tok["tokenInfo"];
                    if (info == null) continue;
                    var symbol = info["symbol"]?.ToString()?.ToUpper() ?? "";
                    var name = info["name"]?.ToString() ?? symbol;
                    var contract = info["address"]?.ToString() ?? "";
                    var decStr = info["decimals"]?.ToString() ?? "18";
                    if (!int.TryParse(decStr, out var decimals)) decimals = 18;
                    var rawBal = tok["rawBalance"]?.ToString() ?? tok["balance"]?.ToString() ?? "0";

                    decimal balance = 0;
                    if (decimal.TryParse(rawBal, System.Globalization.NumberStyles.Any,
                        System.Globalization.CultureInfo.InvariantCulture, out var raw))
                    {
                        balance = raw / (decimal)Math.Pow(10, decimals);
                    }
                    else if (double.TryParse(rawBal, System.Globalization.NumberStyles.Any,
                        System.Globalization.CultureInfo.InvariantCulture, out var rawD))
                    {
                        balance = (decimal)(rawD / Math.Pow(10, decimals));
                    }

                    if (balance <= 0) continue;

                    results.Add(new TokenBalance
                    {
                        Symbol = symbol,
                        Name = name,
                        Balance = balance,
                        Decimals = decimals,
                        ContractAddress = contract,
                    });
                }
            }

            return results;
        }

        private static async Task<decimal?> GetEthBalance(string address)
        {
            var json = await EthplorerGetAsync($"https://api.ethplorer.io/getAddressInfo/{address}?apiKey=freekey");
            var obj = JObject.Parse(json);
            var balance = obj?["ETH"]?["balance"];
            if (balance != null)
                return balance.Value<decimal>();
            return null;
        }

        private static async Task<decimal?> GetXrpBalance(string address)
        {
            var body = "{\"method\":\"account_info\",\"params\":[{\"account\":\"" + address + "\",\"ledger_index\":\"validated\"}]}";
            var content = new StringContent(body, System.Text.Encoding.UTF8, "application/json");
            var resp = await _http.PostAsync("https://xrplcluster.com/", content);
            var json = await resp.Content.ReadAsStringAsync();
            var obj = JObject.Parse(json);
            var drops = obj?["result"]?["account_data"]?["Balance"];
            if (drops != null && long.TryParse(drops.ToString(), out var d))
                return d / 1_000_000m;
            return null;
        }

        private static async Task<decimal?> GetSolBalance(string address)
        {
            var body = "{\"jsonrpc\":\"2.0\",\"id\":1,\"method\":\"getBalance\",\"params\":[\"" + address + "\"]}";
            var content = new StringContent(body, System.Text.Encoding.UTF8, "application/json");
            var resp = await _http.PostAsync("https://api.mainnet-beta.solana.com", content);
            var json = await resp.Content.ReadAsStringAsync();
            var obj = JObject.Parse(json);
            var lamports = obj?["result"]?["value"];
            if (lamports != null)
                return lamports.Value<long>() / 1_000_000_000m;
            return null;
        }

        private static async Task<decimal?> GetAdaBalance(string address)
        {
            var body = "{\"_addresses\":[\"" + address + "\"]}";
            var content = new StringContent(body, System.Text.Encoding.UTF8, "application/json");
            var resp = await _http.PostAsync("https://api.koios.rest/api/v1/address_info", content);
            var json = await resp.Content.ReadAsStringAsync();
            var arr = JArray.Parse(json);
            var lovelace = arr?.FirstOrDefault()?["balance"];
            if (lovelace != null && long.TryParse(lovelace.ToString(), out var l))
                return l / 1_000_000m;
            return null;
        }

        private static async Task<decimal?> GetMaticBalance(string address)
        {
            var body = "{\"jsonrpc\":\"2.0\",\"id\":1,\"method\":\"eth_getBalance\",\"params\":[\"" + address + "\",\"latest\"]}";
            var content = new StringContent(body, System.Text.Encoding.UTF8, "application/json");
            var resp = await _http.PostAsync("https://polygon-rpc.com/", content);
            var json = await resp.Content.ReadAsStringAsync();
            var obj = JObject.Parse(json);
            var hex = obj?["result"]?.ToString();
            if (hex != null && hex.StartsWith("0x"))
            {
                var wei = System.Numerics.BigInteger.Parse("0" + hex[2..], System.Globalization.NumberStyles.HexNumber);
                return (decimal)wei / 1_000_000_000_000_000_000m;
            }
            return null;
        }

        private static async Task<decimal?> GetHbarBalance(string address)
        {
            var json = await _http.GetStringAsync($"https://mainnet-public.mirrornode.hedera.com/api/v1/balances?account.id={address}&limit=1");
            var obj = JObject.Parse(json);
            var balances = obj?["balances"] as JArray;
            var bal = balances?.FirstOrDefault()?["balance"];
            if (bal != null && long.TryParse(bal.ToString(), out var tinybars))
                return tinybars / 100_000_000m;
            return null;
        }

        private static async Task<decimal?> GetXlmBalance(string address)
        {
            var json = await _http.GetStringAsync($"https://horizon.stellar.org/accounts/{address}");
            var obj = JObject.Parse(json);
            var balances = obj?["balances"] as JArray;
            if (balances == null) return null;
            var native = balances.FirstOrDefault(b => b["asset_type"]?.ToString() == "native");
            if (native != null && decimal.TryParse(native["balance"]?.ToString(),
                System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var bal))
                return bal;
            return null;
        }
    }
}
