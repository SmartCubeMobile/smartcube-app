using Newtonsoft.Json.Linq;
using SmartCubeMobile.MockData;

namespace SmartCubeMobile.Services
{
    public static class WalletTransactionService
    {
        private static readonly HttpClient _http = new() { Timeout = TimeSpan.FromSeconds(60) };

        static WalletTransactionService()
        {
            _http.DefaultRequestHeaders.UserAgent.ParseAdd("SmartCubeMobile/1.0");
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

        public static async Task<List<MockCryptoTransaction>> GetTransactionsAsync(string symbol, string address)
        {
            try
            {
                return symbol switch
                {
                    "BTC" => await GetBtcTransactions(address),
                    "ETH" => await GetEthTransactions(address),
                    "XRP" => await GetXrpTransactions(address),
                    "SOL" => await GetSolTransactions(address),
                    "HBAR" => await GetHbarTransactions(address),
                    "XLM" => await GetXlmTransactions(address),
                    _ => new(),
                };
            }
            catch
            {
                return new();
            }
        }

        private static async Task<List<MockCryptoTransaction>> GetBtcTransactions(string address)
        {
            var results = new List<MockCryptoTransaction>();
            int offset = 0;
            const int pageSize = 50;

            while (true)
            {
                var json = await _http.GetStringAsync(
                    $"https://blockchain.info/rawaddr/{address}?limit={pageSize}&offset={offset}");
                var obj = JObject.Parse(json);
                var txs = obj?["txs"] as JArray;
                if (txs == null || txs.Count == 0) break;

                foreach (var tx in txs)
                {
                    var hash = tx["hash"]?.ToString() ?? "";
                    var time = tx["time"]?.Value<long>() ?? 0;
                    var date = DateTimeOffset.FromUnixTimeSeconds(time).LocalDateTime;

                    decimal received = 0, sent = 0;
                    string firstSender = null, firstReceiver = null;
                    if (tx["out"] is JArray outputs)
                        foreach (var o in outputs)
                        {
                            var outAddr = o["addr"]?.ToString() ?? "";
                            if (outAddr == address)
                                received += (o["value"]?.Value<long>() ?? 0) / 100_000_000m;
                            else if (firstReceiver == null && !string.IsNullOrEmpty(outAddr))
                                firstReceiver = outAddr;
                        }

                    if (tx["inputs"] is JArray inputs)
                        foreach (var inp in inputs)
                        {
                            var inAddr = inp["prev_out"]?["addr"]?.ToString() ?? "";
                            if (inAddr == address)
                                sent += (inp["prev_out"]?["value"]?.Value<long>() ?? 0) / 100_000_000m;
                            else if (firstSender == null && !string.IsNullOrEmpty(inAddr))
                                firstSender = inAddr;
                        }

                    var net = received - sent;
                    if (net == 0) continue;

                    results.Add(new MockCryptoTransaction
                    {
                        Date = date,
                        Symbol = "BTC",
                        Type = net > 0 ? "Receive" : "Send",
                        Quantity = Math.Abs(net),
                        Hash = TruncateHash(hash),
                        FromAddress = net > 0 ? (firstSender ?? "") : address,
                        ToAddress = net > 0 ? address : (firstReceiver ?? ""),
                    });
                }

                var totalTx = obj?["n_tx"]?.Value<int>() ?? 0;
                offset += txs.Count;
                if (offset >= totalTx) break;
            }
            return results;
        }

        public static async Task<Dictionary<string, List<MockCryptoTransaction>>> GetEthAllTransactionsAsync(string address)
        {
            var bySymbol = new Dictionary<string, List<MockCryptoTransaction>>();

            var ethTxs = await GetEthNativeTransactions(address);
            if (ethTxs.Count > 0)
                bySymbol["ETH"] = ethTxs;

            await Task.Delay(4000);
            var tokenTxs = await GetEthTokenTransactions(address);
            foreach (var kv in tokenTxs)
            {
                if (!bySymbol.ContainsKey(kv.Key))
                    bySymbol[kv.Key] = new();
                bySymbol[kv.Key].AddRange(kv.Value);
            }

            return bySymbol;
        }

        private static async Task<List<MockCryptoTransaction>> GetEthNativeTransactions(string address)
        {
            var results = new List<MockCryptoTransaction>();
            int offset = 0;
            const int pageSize = 50;
            var addrLower = address.ToLower();

            while (true)
            {
                var url = $"https://api.ethplorer.io/getAddressTransactions/{address}?apiKey=freekey&limit={pageSize}";
                if (offset > 0) url += $"&offset={offset}";
                var json = await EthplorerGetAsync(url);
                var arr = JArray.Parse(json);
                if (arr == null || arr.Count == 0) break;

                foreach (var tx in arr)
                {
                    var hash = tx["hash"]?.ToString() ?? "";
                    var timestamp = tx["timestamp"]?.Value<long>() ?? 0;
                    var date = DateTimeOffset.FromUnixTimeSeconds(timestamp).LocalDateTime;
                    var value = tx["value"]?.Value<decimal>() ?? 0;
                    var from = tx["from"]?.ToString() ?? "";
                    var to = tx["to"]?.ToString() ?? "";

                    results.Add(new MockCryptoTransaction
                    {
                        Date = date,
                        Symbol = "ETH",
                        Type = from.ToLower() == addrLower ? "Send" : "Receive",
                        Quantity = Math.Abs(value),
                        Hash = TruncateHash(hash),
                        FromAddress = from,
                        ToAddress = to,
                    });
                }

                if (arr.Count < pageSize) break;
                offset += arr.Count;
                await Task.Delay(3000);
            }
            return results;
        }

        private static async Task<Dictionary<string, List<MockCryptoTransaction>>> GetEthTokenTransactions(string address)
        {
            var bySymbol = new Dictionary<string, List<MockCryptoTransaction>>();
            var addrLower = address.ToLower();
            string lastTimestamp = null;

            for (int page = 0; page < 10; page++)
            {
                var url = $"https://api.ethplorer.io/getAddressHistory/{address}?apiKey=freekey&limit=100&type=transfer";
                if (lastTimestamp != null) url += $"&timestamp={lastTimestamp}";
                var json = await EthplorerGetAsync(url);
                var obj = JObject.Parse(json);
                var ops = obj?["operations"] as JArray;
                if (ops == null || ops.Count == 0) break;

                foreach (var op in ops)
                {
                    var info = op["tokenInfo"];
                    if (info == null) continue;
                    var symbol = info["symbol"]?.ToString()?.ToUpper() ?? "";
                    if (string.IsNullOrEmpty(symbol)) continue;
                    var decStr = info["decimals"]?.ToString() ?? "18";
                    if (!int.TryParse(decStr, out var decimals)) decimals = 18;

                    var hash = op["transactionHash"]?.ToString() ?? "";
                    var timestamp = op["timestamp"]?.Value<long>() ?? 0;
                    var date = DateTimeOffset.FromUnixTimeSeconds(timestamp).LocalDateTime;
                    var rawValue = op["value"]?.ToString() ?? "0";
                    var from = op["from"]?.ToString()?.ToLower() ?? "";

                    decimal quantity = 0;
                    if (decimal.TryParse(rawValue, System.Globalization.NumberStyles.Any,
                        System.Globalization.CultureInfo.InvariantCulture, out var raw))
                    {
                        quantity = raw / (decimal)Math.Pow(10, decimals);
                    }
                    else if (double.TryParse(rawValue, System.Globalization.NumberStyles.Any,
                        System.Globalization.CultureInfo.InvariantCulture, out var rawD))
                    {
                        quantity = (decimal)(rawD / Math.Pow(10, decimals));
                    }

                    if (!bySymbol.ContainsKey(symbol))
                        bySymbol[symbol] = new();

                    var to = op["to"]?.ToString() ?? "";
                    bySymbol[symbol].Add(new MockCryptoTransaction
                    {
                        Date = date,
                        Symbol = symbol,
                        Type = from == addrLower ? "Send" : "Receive",
                        Quantity = Math.Abs(quantity),
                        Hash = TruncateHash(hash),
                        FromAddress = op["from"]?.ToString() ?? "",
                        ToAddress = to,
                    });
                }

                if (ops.Count < 100) break;
                lastTimestamp = ops.Last?["timestamp"]?.ToString();
                if (lastTimestamp == null) break;
                await Task.Delay(3000);
            }
            return bySymbol;
        }

        private static async Task<List<MockCryptoTransaction>> GetEthTransactions(string address)
        {
            var all = await GetEthAllTransactionsAsync(address);
            return all.TryGetValue("ETH", out var ethTxs) ? ethTxs : new();
        }

        private static async Task<List<MockCryptoTransaction>> GetXrpTransactions(string address)
        {
            var results = new List<MockCryptoTransaction>();
            JToken marker = null;

            while (true)
            {
                var paramsObj = new JObject
                {
                    ["account"] = address,
                    ["ledger_index_min"] = -1,
                    ["ledger_index_max"] = -1,
                    ["limit"] = 200,
                };
                if (marker != null)
                    paramsObj["marker"] = marker;

                var reqObj = new JObject
                {
                    ["method"] = "account_tx",
                    ["params"] = new JArray { paramsObj },
                };

                var content = new StringContent(reqObj.ToString(), System.Text.Encoding.UTF8, "application/json");
                var resp = await _http.PostAsync("https://xrplcluster.com/", content);
                var json = await resp.Content.ReadAsStringAsync();
                var obj = JObject.Parse(json);
                var txs = obj?["result"]?["transactions"] as JArray;
                if (txs == null || txs.Count == 0) break;

                foreach (var wrapper in txs)
                {
                    var tx = wrapper["tx"];
                    if (tx == null) continue;
                    if (tx["TransactionType"]?.ToString() != "Payment") continue;

                    var hash = tx["hash"]?.ToString() ?? "";
                    var date = ParseRippleDate(tx["date"]?.Value<long>() ?? 0);
                    var account = tx["Account"]?.ToString() ?? "";
                    var dest = tx["Destination"]?.ToString() ?? "";
                    var amount = tx["Amount"];

                    decimal xrpAmount = 0;
                    if (amount?.Type == JTokenType.String)
                    {
                        if (long.TryParse(amount.ToString(), out var drops))
                            xrpAmount = drops / 1_000_000m;
                    }
                    else continue;

                    results.Add(new MockCryptoTransaction
                    {
                        Date = date,
                        Symbol = "XRP",
                        Type = dest == address ? "Receive" : "Send",
                        Quantity = xrpAmount,
                        Hash = TruncateHash(hash),
                        FromAddress = account,
                        ToAddress = dest,
                    });
                }

                marker = obj?["result"]?["marker"];
                if (marker == null || marker.Type == JTokenType.Null) break;
            }
            return results;
        }

        private static DateTime ParseRippleDate(long rippleEpoch)
        {
            var unixTime = rippleEpoch + 946684800;
            return DateTimeOffset.FromUnixTimeSeconds(unixTime).LocalDateTime;
        }

        private static async Task<List<MockCryptoTransaction>> GetSolTransactions(string address)
        {
            var sigBody = $"{{\"jsonrpc\":\"2.0\",\"id\":1,\"method\":\"getSignaturesForAddress\",\"params\":[\"{address}\",{{\"limit\":50}}]}}";
            var sigContent = new StringContent(sigBody, System.Text.Encoding.UTF8, "application/json");
            var sigResp = await _http.PostAsync("https://api.mainnet-beta.solana.com", sigContent);
            var sigJson = await sigResp.Content.ReadAsStringAsync();
            var sigObj = JObject.Parse(sigJson);
            var sigs = sigObj?["result"] as JArray;
            if (sigs == null || sigs.Count == 0) return new();

            var results = new List<MockCryptoTransaction>();
            foreach (var sig in sigs)
            {
                var hash = sig["signature"]?.ToString() ?? "";
                var err = sig["err"];
                if (err != null && err.Type != JTokenType.Null) continue;

                try
                {
                    var txBody = $"{{\"jsonrpc\":\"2.0\",\"id\":1,\"method\":\"getTransaction\",\"params\":[\"{hash}\",{{\"encoding\":\"jsonParsed\",\"maxSupportedTransactionVersion\":0}}]}}";
                    var txContent = new StringContent(txBody, System.Text.Encoding.UTF8, "application/json");
                    var txResp = await _http.PostAsync("https://api.mainnet-beta.solana.com", txContent);
                    var txJson = await txResp.Content.ReadAsStringAsync();
                    var txObj = JObject.Parse(txJson);
                    var txResult = txObj?["result"];
                    if (txResult == null || txResult.Type == JTokenType.Null) continue;

                    var blockTime = txResult["blockTime"]?.Value<long>() ?? 0;
                    var date = DateTimeOffset.FromUnixTimeSeconds(blockTime).LocalDateTime;

                    var meta = txResult["meta"];
                    var preBalances = meta?["preBalances"] as JArray;
                    var postBalances = meta?["postBalances"] as JArray;
                    var accountKeys = txResult["transaction"]?["message"]?["accountKeys"] as JArray;

                    if (preBalances == null || postBalances == null || accountKeys == null) continue;

                    int addrIdx = -1;
                    for (int i = 0; i < accountKeys.Count; i++)
                    {
                        var key = accountKeys[i]?["pubkey"]?.ToString() ?? accountKeys[i]?.ToString() ?? "";
                        if (key == address) { addrIdx = i; break; }
                    }
                    if (addrIdx < 0 || addrIdx >= preBalances.Count) continue;

                    var pre = preBalances[addrIdx]?.Value<long>() ?? 0;
                    var post = postBalances[addrIdx]?.Value<long>() ?? 0;
                    var diff = post - pre;
                    if (diff == 0) continue;

                    var solAmount = Math.Abs(diff) / 1_000_000_000m;
                    var type = diff > 0 ? "Receive" : "Send";

                    string from = null, to = null;
                    if (accountKeys.Count >= 2)
                    {
                        var key0 = accountKeys[0]?["pubkey"]?.ToString() ?? accountKeys[0]?.ToString() ?? "";
                        var key1 = accountKeys[1]?["pubkey"]?.ToString() ?? accountKeys[1]?.ToString() ?? "";
                        from = type == "Send" ? address : key0 == address ? key1 : key0;
                        to = type == "Receive" ? address : key0 == address ? key1 : key0;
                    }

                    results.Add(new MockCryptoTransaction
                    {
                        Date = date,
                        Symbol = "SOL",
                        Type = type,
                        Quantity = solAmount,
                        Hash = TruncateHash(hash),
                        FromAddress = from ?? "",
                        ToAddress = to ?? "",
                    });
                }
                catch { }
            }
            return results;
        }

        private static async Task<List<MockCryptoTransaction>> GetHbarTransactions(string address)
        {
            var results = new List<MockCryptoTransaction>();
            string nextLink = null;

            for (int page = 0; page < 10; page++)
            {
                var url = nextLink ?? $"https://mainnet-public.mirrornode.hedera.com/api/v1/transactions?account.id={address}&limit=100&order=desc&transactiontype=CRYPTOTRANSFER";
                var json = await _http.GetStringAsync(url);
                var obj = JObject.Parse(json);
                var txs = obj?["transactions"] as JArray;
                if (txs == null || txs.Count == 0) break;

                foreach (var tx in txs)
                {
                    var txId = tx["transaction_id"]?.ToString() ?? "";
                    var consensusTs = tx["consensus_timestamp"]?.ToString() ?? "";
                    DateTime date = DateTime.UtcNow;
                    if (double.TryParse(consensusTs, System.Globalization.NumberStyles.Any,
                        System.Globalization.CultureInfo.InvariantCulture, out var ts))
                        date = DateTimeOffset.FromUnixTimeSeconds((long)ts).LocalDateTime;

                    var transfers = tx["transfers"] as JArray;
                    if (transfers == null) continue;

                    decimal myAmount = 0;
                    string otherAddr = null;
                    foreach (var tr in transfers)
                    {
                        var acct = tr["account"]?.ToString() ?? "";
                        var amt = tr["amount"]?.Value<long>() ?? 0;
                        if (acct == address)
                            myAmount += amt;
                        else if (otherAddr == null && amt != 0 && !acct.StartsWith("0.0.98"))
                            otherAddr = acct;
                    }

                    if (myAmount == 0) continue;
                    var hbarAmount = myAmount / 100_000_000m;

                    results.Add(new MockCryptoTransaction
                    {
                        Date = date,
                        Symbol = "HBAR",
                        Type = hbarAmount > 0 ? "Receive" : "Send",
                        Quantity = Math.Abs(hbarAmount),
                        Hash = TruncateHash(txId),
                        FromAddress = hbarAmount > 0 ? otherAddr : address,
                        ToAddress = hbarAmount > 0 ? address : otherAddr,
                    });
                }

                var links = obj?["links"]?["next"]?.ToString();
                if (string.IsNullOrEmpty(links)) break;
                nextLink = $"https://mainnet-public.mirrornode.hedera.com{links}";
            }
            return results;
        }

        private static async Task<List<MockCryptoTransaction>> GetXlmTransactions(string address)
        {
            var results = new List<MockCryptoTransaction>();
            var url = $"https://horizon.stellar.org/accounts/{address}/payments?order=desc&limit=100";

            while (url != null && results.Count < 200)
            {
                var json = await _http.GetStringAsync(url);
                var obj = JObject.Parse(json);
                var records = obj?["_embedded"]?["records"] as JArray;
                if (records == null || records.Count == 0) break;

                foreach (var rec in records)
                {
                    try
                    {
                        var type = rec["type"]?.ToString();
                        if (type != "payment" && type != "create_account") continue;

                        var dateStr = rec["created_at"]?.ToString();
                        if (!DateTime.TryParse(dateStr, System.Globalization.CultureInfo.InvariantCulture,
                            System.Globalization.DateTimeStyles.RoundtripKind, out var date))
                            continue;

                        var from = rec["from"]?.ToString() ?? rec["source_account"]?.ToString() ?? "";
                        var to = rec["to"]?.ToString() ?? rec["account"]?.ToString() ?? "";

                        decimal amount = 0;
                        if (type == "create_account")
                            decimal.TryParse(rec["starting_balance"]?.ToString(), System.Globalization.NumberStyles.Any,
                                System.Globalization.CultureInfo.InvariantCulture, out amount);
                        else
                        {
                            var assetType = rec["asset_type"]?.ToString();
                            if (assetType != "native") continue;
                            decimal.TryParse(rec["amount"]?.ToString(), System.Globalization.NumberStyles.Any,
                                System.Globalization.CultureInfo.InvariantCulture, out amount);
                        }

                        if (amount <= 0) continue;
                        var isReceive = to.Equals(address, StringComparison.OrdinalIgnoreCase);

                        results.Add(new MockCryptoTransaction
                        {
                            Date = date,
                            Symbol = "XLM",
                            Type = isReceive ? "Receive" : "Send",
                            Quantity = amount,
                            Hash = TruncateHash(rec["transaction_hash"]?.ToString() ?? rec["id"]?.ToString() ?? ""),
                            FromAddress = from,
                            ToAddress = to,
                        });
                    }
                    catch { }
                }

                var next = obj?["_links"]?["next"]?["href"]?.ToString();
                url = string.IsNullOrEmpty(next) ? null : next;
            }
            return results;
        }

        private static string TruncateHash(string hash)
        {
            if (hash.Length > 16)
                return hash[..8] + "..." + hash[^4..];
            return hash;
        }
    }
}
