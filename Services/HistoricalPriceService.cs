using System.Globalization;
using System.Text.Json;
using Newtonsoft.Json.Linq;
using SmartCubeMobile.MockData;

namespace SmartCubeMobile.Services
{
    // Price of a coin on the day of each wallet transaction, in GBP, so P&L uses what it was worth then.
    //   1. Network price: Ethplorer gives the USD price at the time of every native ETH transaction.
    //   2. Otherwise the coin's daily close on Binance ({SYMBOL}USDT), which covers the tokens the
    //      blockchain APIs don't price (ERC-20 transfers, BTC, SOL, XRP, ...).
    // USD -> GBP uses that day's USDT/GBP close (Coinbase, then Kraken). Daily closes are cached on disk,
    // so only new days are fetched. If nothing is available the current price is used and marked as an
    // estimate (PriceSource "current").
    public static class HistoricalPriceService
    {
        public const string SourceNetwork = "network";
        public const string SourceDaily = "daily";
        public const string SourceExchange = "exchange";
        public const string SourceCurrent = "current";

        private static readonly HttpClient _http = new() { Timeout = TimeSpan.FromSeconds(15) };
        static HistoricalPriceService() => _http.DefaultRequestHeaders.UserAgent.ParseAdd("SmartCubeMobile/1.0");

        private static readonly SemaphoreSlim _lock = new(1, 1);
        private static Dictionary<string, SortedDictionary<string, decimal>> _cache;   // series -> yyyy-MM-dd -> close
        private static readonly HashSet<string> _unavailable = new(StringComparer.OrdinalIgnoreCase);
        // Each series is downloaded at most once per app session (older dates may simply not exist).
        private static readonly HashSet<string> _triedThisSession = new(StringComparer.OrdinalIgnoreCase);
        private static string CachePath => Path.Combine(MockDataService.DataDir, "price_history.json");
        private const string FxSeries = "FX:USDTGBP";

        private static readonly Dictionary<string, string> Aliases = new(StringComparer.OrdinalIgnoreCase)
        {
            ["WETH"] = "ETH", ["STETH"] = "ETH", ["WSTETH"] = "ETH", ["WBTC"] = "BTC", ["RENDER"] = "RNDR",
        };
        private static readonly HashSet<string> UsdStable = new(StringComparer.OrdinalIgnoreCase) { "USDT", "USDC", "DAI", "BUSD", "TUSD", "FDUSD", "USDP" };

        // True when the transaction still needs a proper historical price.
        public static bool NeedsPrice(MockCryptoTransaction t) =>
            t != null && (t.PriceAtTime <= 0 || t.PriceSource == null || t.PriceSource == SourceCurrent);

        // Prices wallet transactions by date. currentPrice is only a last resort for coins with no history.
        public static async Task FillPricesAsync(IEnumerable<MockCryptoTransaction> transactions, decimal currentPrice = 0)
        {
            var list = (transactions ?? Enumerable.Empty<MockCryptoTransaction>()).Where(NeedsPrice).ToList();
            if (list.Count == 0) return;

            await _lock.WaitAsync();
            try
            {
                LoadCache();
                var from = list.Min(t => t.Date).Date;
                var to = list.Max(t => t.Date).Date;
                await EnsureFx(from, to);

                foreach (var group in list.GroupBy(t => t.Symbol ?? ""))
                {
                    var sym = Aliases.TryGetValue(group.Key, out var a) ? a : group.Key.ToUpperInvariant();
                    var needDaily = group.Any(t => t.UsdPriceAtTime <= 0) && !UsdStable.Contains(sym);
                    if (needDaily) await EnsureDaily(sym, group.Min(t => t.Date).Date, group.Max(t => t.Date).Date);

                    foreach (var t in group)
                    {
                        var day = t.Date.ToUniversalTime().Date;
                        var fx = Close(FxSeries, day, 10);
                        decimal usd = t.UsdPriceAtTime > 0 ? t.UsdPriceAtTime
                            : UsdStable.Contains(sym) ? 1m
                            : Close("BN:" + sym, day, 3);

                        if (usd > 0 && fx > 0)
                        {
                            t.PriceAtTime = Math.Round(usd * fx, 10);
                            t.PriceSource = t.UsdPriceAtTime > 0 ? SourceNetwork : SourceDaily;
                        }
                        else if (currentPrice > 0 && t.PriceAtTime <= 0)
                        {
                            t.PriceAtTime = currentPrice;
                            t.PriceSource = SourceCurrent;
                        }
                    }
                }
                SaveCache();
            }
            catch { }
            finally { _lock.Release(); }
        }

        #region Series

        private static string Key(DateTime d) => d.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);

        // Close on that day, or the nearest day within maxGapDays (weekends for FX, listing gaps for coins).
        private static decimal Close(string series, DateTime day, int maxGapDays)
        {
            if (!_cache.TryGetValue(series, out var s) || s.Count == 0) return 0;
            for (int gap = 0; gap <= maxGapDays; gap++)
            {
                if (s.TryGetValue(Key(day.AddDays(-gap)), out var v) && v > 0) return v;
                if (gap > 0 && s.TryGetValue(Key(day.AddDays(gap)), out v) && v > 0) return v;
            }
            return 0;
        }

        private static bool Covers(string series, DateTime from, DateTime to)
        {
            if (!_cache.TryGetValue(series, out var s) || s.Count == 0) return false;
            var today = DateTime.UtcNow.Date;
            if (to >= today) to = today.AddDays(-1);   // today's close isn't final yet
            return string.CompareOrdinal(s.Keys.First(), Key(from)) <= 0 && string.CompareOrdinal(s.Keys.Last(), Key(to)) >= 0;
        }

        private static SortedDictionary<string, decimal> Series(string name)
        {
            if (!_cache.TryGetValue(name, out var s)) _cache[name] = s = new SortedDictionary<string, decimal>(StringComparer.Ordinal);
            return s;
        }

        // Daily USD closes from Binance, fetched forward from the earliest date needed.
        private static async Task EnsureDaily(string sym, DateTime from, DateTime to)
        {
            var name = "BN:" + sym;
            if (_unavailable.Contains(name) || Covers(name, from, to) || !_triedThisSession.Add(name)) return;
            var series = Series(name);
            var start = new DateTimeOffset(DateTime.SpecifyKind(from.AddDays(-3), DateTimeKind.Utc)).ToUnixTimeMilliseconds();
            var end = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds();
            try
            {
                for (int page = 0; page < 10 && start < end; page++)
                {
                    var json = await _http.GetStringAsync($"https://api.binance.com/api/v3/klines?symbol={sym}USDT&interval=1d&limit=1000&startTime={start}");
                    var rows = JArray.Parse(json);
                    if (rows.Count == 0) break;
                    foreach (var r in rows)
                    {
                        var open = DateTimeOffset.FromUnixTimeMilliseconds(r[0]!.Value<long>()).UtcDateTime.Date;
                        if (decimal.TryParse(r[4]!.ToString(), NumberStyles.Any, CultureInfo.InvariantCulture, out var close))
                            series[Key(open)] = close;
                    }
                    start = rows.Last![0]!.Value<long>() + 86_400_000;
                    if (rows.Count < 1000) break;
                }
            }
            catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                _unavailable.Add(name);   // not listed on Binance (spam or very small token)
            }
            catch { }
        }

        // USDT -> GBP daily closes: Coinbase candles (300 days per request), then Kraken for recent days.
        private static async Task EnsureFx(DateTime from, DateTime to)
        {
            if (Covers(FxSeries, from, to) || !_triedThisSession.Add(FxSeries)) return;
            var series = Series(FxSeries);
            try
            {
                var start = from.AddDays(-5);
                var end = DateTime.UtcNow.Date;
                while (start < end)
                {
                    var chunkEnd = start.AddDays(299) < end ? start.AddDays(299) : end;
                    var url = $"https://api.exchange.coinbase.com/products/USDT-GBP/candles?granularity=86400&start={start:yyyy-MM-dd}T00:00:00Z&end={chunkEnd:yyyy-MM-dd}T00:00:00Z";
                    var rows = JArray.Parse(await _http.GetStringAsync(url));
                    foreach (var r in rows)
                    {
                        var day = DateTimeOffset.FromUnixTimeSeconds(r[0]!.Value<long>()).UtcDateTime.Date;
                        series[Key(day)] = r[4]!.Value<decimal>();
                    }
                    start = chunkEnd.AddDays(1);
                }
            }
            catch { }

            if (Covers(FxSeries, from, to)) return;
            try
            {
                var obj = JObject.Parse(await _http.GetStringAsync("https://api.kraken.com/0/public/OHLC?pair=USDTGBP&interval=1440"));
                if (obj["result"]?["USDTGBP"] is JArray rows)
                    foreach (var r in rows)
                    {
                        var day = DateTimeOffset.FromUnixTimeSeconds(r[0]!.Value<long>()).UtcDateTime.Date;
                        if (decimal.TryParse(r[4]!.ToString(), NumberStyles.Any, CultureInfo.InvariantCulture, out var close))
                            series.TryAdd(Key(day), close);
                    }
            }
            catch { }
        }

        #endregion

        #region Disk cache

        private static void LoadCache()
        {
            if (_cache != null) return;
            try
            {
                if (File.Exists(CachePath))
                {
                    var raw = JsonSerializer.Deserialize<Dictionary<string, Dictionary<string, decimal>>>(SecureFile.ReadAllText(CachePath));
                    _cache = raw?.ToDictionary(kv => kv.Key, kv => new SortedDictionary<string, decimal>(kv.Value, StringComparer.Ordinal));
                }
            }
            catch { }
            _cache ??= new();
        }

        private static void SaveCache()
        {
            try { SecureFile.WriteAllText(CachePath, JsonSerializer.Serialize(_cache)); } catch { }
        }

        #endregion
    }
}
