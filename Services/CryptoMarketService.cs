using Newtonsoft.Json;
using System.Globalization;

namespace SmartCubeMobile.Services
{
    public static class CryptoFormatHelper
    {
        private static readonly CultureInfo _gb = new("en-GB");

        public static string FormatPrice(decimal value)
        {
            var abs = Math.Abs(value);
            if (abs == 0) return "£0.00";
            if (abs >= 1m) return value.ToString("C", _gb);
            if (abs >= 0.01m) return value.ToString("C4", _gb);
            int dp = 2;
            var test = abs;
            while (test < 0.01m && dp < 10)
            {
                test *= 10;
                dp++;
            }
            return value.ToString($"C{dp}", _gb);
        }

        public static string FormatValue(decimal value)
        {
            if (Math.Abs(value) >= 0.01m) return value.ToString("C", _gb);
            return FormatPrice(value);
        }

        public static string FormatSignedValue(decimal value)
        {
            var prefix = value >= 0 ? "+" : "";
            return $"{prefix}{FormatValue(value)}";
        }
    }

    public class MarketCoin
    {
        public string Id { get; set; }
        public string Symbol { get; set; }
        public string Name { get; set; }
        public int Rank { get; set; }
        public decimal PriceGBP { get; set; }
        public decimal MarketCapGBP { get; set; }
        public decimal ChangePercent24h { get; set; }
        public decimal VolumeGBP { get; set; }
    }

    public static class CryptoMarketService
    {
        private static readonly HttpClient _http = new() { Timeout = TimeSpan.FromSeconds(15) };
        private static readonly HttpClient _fallbackHttp = new() { Timeout = TimeSpan.FromSeconds(8) };

        static CryptoMarketService()
        {
            _http.DefaultRequestHeaders.UserAgent.ParseAdd("SmartCubeMobile/1.0");
            _fallbackHttp.DefaultRequestHeaders.UserAgent.ParseAdd("SmartCubeMobile/1.0");
        }
        private static List<MarketCoin> _cache;
        private static DateTime _cacheTime = DateTime.MinValue;

        public static void ClearCache() => _cacheTime = DateTime.MinValue;

        #region CoinGecko throttle

        // CoinGecko's free API allows only a handful of calls a minute per IP. Every call goes through here:
        // calls are spaced out, and after a "429 Too Many Requests" we stop asking until the cool-down ends
        // instead of waiting and retrying (which made wallet refreshes look like they'd hung).
        private static readonly SemaphoreSlim _gate = new(1, 1);
        private static DateTime _lastCall = DateTime.MinValue;
        private static DateTime _coolDownUntil = DateTime.MinValue;
        private static readonly TimeSpan MinSpacing = TimeSpan.FromSeconds(2.5);

        public static bool IsRateLimited => DateTime.UtcNow < _coolDownUntil;

        private static async Task<string> CoinGeckoGetAsync(string url)
        {
            if (IsRateLimited) throw new HttpRequestException("CoinGecko rate limit - cooling down");
            await _gate.WaitAsync();
            try
            {
                if (IsRateLimited) throw new HttpRequestException("CoinGecko rate limit - cooling down");
                var wait = _lastCall + MinSpacing - DateTime.UtcNow;
                if (wait > TimeSpan.Zero) await Task.Delay(wait);
                _lastCall = DateTime.UtcNow;

                using var resp = await _http.GetAsync(url);
                if ((int)resp.StatusCode == 429)
                {
                    var retry = resp.Headers.RetryAfter?.Delta ?? TimeSpan.FromSeconds(60);
                    _coolDownUntil = DateTime.UtcNow + (retry < TimeSpan.FromSeconds(20) ? TimeSpan.FromSeconds(20) : retry);
                    throw new HttpRequestException("CoinGecko rate limit (429)");
                }
                resp.EnsureSuccessStatusCode();
                return await resp.Content.ReadAsStringAsync();
            }
            finally { _gate.Release(); }
        }

        #endregion

        #region Price cache

        private static readonly Dictionary<string, (decimal Price, decimal Change, DateTime At)> _prices = new(StringComparer.OrdinalIgnoreCase);
        private static readonly TimeSpan PriceFresh = TimeSpan.FromMinutes(3);

        private static void Remember(string symbol, decimal price, decimal change)
        {
            if (price > 0) lock (_prices) _prices[symbol] = (price, change, DateTime.UtcNow);
        }

        // Coinbase's public spot price (no key, generous limits) for when CoinGecko is rate limited.
        private static async Task<decimal> CoinbaseSpotAsync(string symbol)
        {
            try
            {
                var json = await _fallbackHttp.GetStringAsync($"https://api.coinbase.com/v2/prices/{Uri.EscapeDataString(symbol)}-GBP/spot");
                var amount = Newtonsoft.Json.Linq.JObject.Parse(json)?["data"]?["amount"]?.ToString();
                return decimal.TryParse(amount, NumberStyles.Any, CultureInfo.InvariantCulture, out var p) ? p : 0;
            }
            catch { return 0; }
        }

        // Last price SmartCube saw for a coin (this session, or the saved holdings from last time).
        private static (decimal Price, decimal Change) LastKnown(string symbol)
        {
            lock (_prices)
                if (_prices.TryGetValue(symbol, out var c)) return (c.Price, c.Change);
            try
            {
                var h = MockData.MockDataService.GetCryptoHoldings()
                    .FirstOrDefault(x => string.Equals(x.Symbol, symbol, StringComparison.OrdinalIgnoreCase) && x.PriceGBP > 0);
                if (h != null) return (h.PriceGBP, h.Change24h);
            }
            catch { }
            return (0, 0);
        }

        #endregion

        public static async Task<List<MarketCoin>> GetTop50Async()
        {
            if (_cache != null && DateTime.UtcNow - _cacheTime < TimeSpan.FromMinutes(2))
                return _cache;

            var url = "https://api.coingecko.com/api/v3/coins/markets"
                    + "?vs_currency=gbp&order=market_cap_desc&per_page=50&page=1&sparkline=false";
            string json;
            try { json = await CoinGeckoGetAsync(url); }
            catch when (_cache != null) { return _cache; }   // rate limited: keep showing the last list
            var coins = JsonConvert.DeserializeObject<List<CoinGeckoMarket>>(json);

            _cache = (coins ?? new List<CoinGeckoMarket>()).Select(c => new MarketCoin
            {
                Id = c.id ?? "",
                Symbol = c.symbol?.ToUpper() ?? "",
                Name = c.name ?? "",
                Rank = c.market_cap_rank,
                PriceGBP = c.current_price,
                MarketCapGBP = c.market_cap,
                ChangePercent24h = c.price_change_percentage_24h ?? 0,
                VolumeGBP = c.total_volume,
            }).ToList();

            _cacheTime = DateTime.UtcNow;
            foreach (var c in _cache) Remember(c.Symbol, c.PriceGBP, Math.Round(c.ChangePercent24h, 2));
            return _cache;
        }

        private static readonly Dictionary<string, string> _coinIds = new()
        {
            ["BTC"] = "bitcoin", ["ETH"] = "ethereum", ["XRP"] = "ripple",
            ["SOL"] = "solana", ["ADA"] = "cardano", ["MATIC"] = "matic-network",
            ["DOGE"] = "dogecoin", ["DOT"] = "polkadot", ["AVAX"] = "avalanche-2",
            ["LINK"] = "chainlink", ["USDT"] = "tether", ["USDC"] = "usd-coin",
            ["SHIB"] = "shiba-inu", ["LTC"] = "litecoin", ["UNI"] = "uniswap",
            ["ATOM"] = "cosmos", ["FIL"] = "filecoin", ["NEAR"] = "near",
            ["APT"] = "aptos", ["ARB"] = "arbitrum", ["OP"] = "optimism",
            ["ALGO"] = "algorand", ["ICP"] = "internet-computer", ["GRT"] = "the-graph",
            ["AAVE"] = "aave", ["MKR"] = "maker", ["CRV"] = "curve-dao-token",
            ["SAND"] = "the-sandbox", ["MANA"] = "decentraland", ["AXS"] = "axie-infinity",
            ["COMP"] = "compound-governance-token", ["SNX"] = "havven",
            ["BAT"] = "basic-attention-token", ["ZRX"] = "0x", ["ENJ"] = "enjincoin",
            ["1INCH"] = "1inch", ["SUSHI"] = "sushi", ["YFI"] = "yearn-finance",
            ["LRC"] = "loopring", ["BNB"] = "binancecoin", ["TRX"] = "tron",
            ["ETC"] = "ethereum-classic", ["XLM"] = "stellar", ["VET"] = "vechain",
            ["HBAR"] = "hedera-hashgraph", ["FTM"] = "fantom", ["THETA"] = "theta-token",
            ["XTZ"] = "tezos", ["EOS"] = "eos", ["FLOW"] = "flow",
            ["CHZ"] = "chiliz", ["GALA"] = "gala", ["IMX"] = "immutable-x",
            ["JASMY"] = "jasmycoin", ["RNDR"] = "render-token", ["INJ"] = "injective-protocol",
            ["SUI"] = "sui", ["SEI"] = "sei-network", ["TIA"] = "celestia",
            ["PEPE"] = "pepe", ["BONK"] = "bonk", ["WIF"] = "dogwifcoin",
            ["FLOKI"] = "floki", ["RENDER"] = "render-token", ["FET"] = "fetch-ai",
            ["AGIX"] = "singularitynet", ["OCEAN"] = "ocean-protocol",
            ["QNT"] = "quant-network", ["EGLD"] = "elrond-erd-2",
            ["ZEC"] = "zcash", ["DASH"] = "dash", ["XMR"] = "monero",
            ["NEO"] = "neo", ["IOTA"] = "iota", ["CELO"] = "celo",
            ["ROSE"] = "oasis-network", ["KDA"] = "kadena", ["MINA"] = "mina-protocol",
            ["ONE"] = "harmony", ["ZIL"] = "zilliqa", ["ICX"] = "icon",
            ["ANKR"] = "ankr", ["SKL"] = "skale", ["STORJ"] = "storj",
            ["BAND"] = "band-protocol", ["NMR"] = "numeraire", ["RLC"] = "iexec-rlc",
            ["CLV"] = "clover-finance", ["CTSI"] = "cartesi", ["MASK"] = "mask-network",
            ["API3"] = "api3", ["ACH"] = "alchemy-pay", ["RAD"] = "radicle",
            ["POLY"] = "polymath", ["REN"] = "republic-protocol",
            ["DNT"] = "district0x", ["CVC"] = "civic", ["REQ"] = "request-network",
            ["NKN"] = "nkn", ["OGN"] = "origin-protocol", ["SPELL"] = "spell-token",
            ["BICO"] = "biconomy", ["SUPER"] = "superfarm", ["MEDIA"] = "media-network",
            ["LOOM"] = "loom-network-new", ["ASM"] = "assemble-protocol",
            ["PRO"] = "propy", ["FARM"] = "harvest-finance",
            ["FORT"] = "forta", ["ARPA"] = "arpa", ["PERP"] = "perpetual-protocol",
            ["SYLO"] = "sylo", ["AERGO"] = "aergo", ["PLU"] = "pluton",
            ["IRYS"] = "irys",
            ["TURBO"] = "turbo", ["TRUMP"] = "official-trump",
            ["AIOZ"] = "aioz-network", ["ILV"] = "illuvium",
            ["ONDO"] = "ondo-finance", ["WOLF"] = "landwolf-on-avax",
            ["DAI"] = "dai", ["WBTC"] = "wrapped-bitcoin",
            ["STETH"] = "staked-ether", ["WETH"] = "weth",
            ["CRO"] = "crypto-com-chain", ["LEO"] = "leo-token",
            ["TON"] = "the-open-network", ["KAS"] = "kaspa",
            ["TAO"] = "bittensor", ["STX"] = "blockstack",
            ["ENA"] = "ethena", ["PENDLE"] = "pendle",
            ["LDO"] = "lido-dao", ["RPL"] = "rocket-pool",
            ["SSV"] = "ssv-network", ["BLUR"] = "blur",
            ["ENS"] = "ethereum-name-service", ["DYDX"] = "dydx-chain",
            ["WLD"] = "worldcoin-wld", ["ARK"] = "ark",
            ["PYTH"] = "pyth-network", ["JTO"] = "jito-governance-token",
            ["W"] = "wormhole", ["STRK"] = "starknet",
            ["ZK"] = "zksync", ["ZRO"] = "layerzero",
            ["EIGEN"] = "eigenlayer", ["SAFE"] = "safe",
            ["CAKE"] = "pancakeswap-token", ["CRV"] = "curve-dao-token",
            ["AERO"] = "aerodrome-finance", ["BRETT"] = "brett",
        };

        public static string GetCoinId(string symbol) =>
            _coinIds.TryGetValue(symbol, out var id) ? id : null;

        // Prices in GBP for known coins. Uses prices fetched in the last few minutes, asks CoinGecko once
        // for the rest, and if CoinGecko is rate limited falls back to Coinbase's spot price and then to
        // the last known price, so it always returns promptly. Unknown (spam) tokens get no price.
        public static async Task<Dictionary<string, (decimal Price, decimal Change24h)>> GetBatchPricesAsync(IEnumerable<string> symbols)
        {
            var result = new Dictionary<string, (decimal, decimal)>();
            var symbolToId = new Dictionary<string, string>();
            foreach (var sym in symbols.Distinct())
            {
                if (_coinIds.TryGetValue(sym, out var id))
                    symbolToId[sym] = id;
            }
            if (symbolToId.Count == 0) return result;

            var stale = new List<KeyValuePair<string, string>>();
            foreach (var kv in symbolToId)
            {
                bool fresh;
                (decimal Price, decimal Change, DateTime At) c;
                lock (_prices) fresh = _prices.TryGetValue(kv.Key, out c) && DateTime.UtcNow - c.At < PriceFresh;
                if (fresh) result[kv.Key] = (c.Price, c.Change);
                else stale.Add(kv);
            }

            foreach (var chunk in stale.Chunk(50))
            {
                var ids = string.Join(",", chunk.Select(kv => kv.Value));
                var url = $"https://api.coingecko.com/api/v3/simple/price?ids={ids}&vs_currencies=gbp&include_24hr_change=true";
                try
                {
                    var obj = Newtonsoft.Json.Linq.JObject.Parse(await CoinGeckoGetAsync(url));
                    foreach (var kv in chunk)
                    {
                        var price = (decimal?)obj?[kv.Value]?["gbp"] ?? 0;
                        var change = Math.Round((decimal?)obj?[kv.Value]?["gbp_24h_change"] ?? 0, 2);
                        if (price <= 0) continue;
                        result[kv.Key] = (price, change);
                        Remember(kv.Key, price, change);
                    }
                }
                catch { break; }   // rate limited or offline: use the fallbacks below
            }

            var missing = symbolToId.Keys.Where(s => !result.ContainsKey(s)).ToList();
            if (missing.Count > 0)
            {
                var spot = await Task.WhenAll(missing.Select(async s => (Symbol: s, Price: await CoinbaseSpotAsync(s))));
                foreach (var (sym, price) in spot)
                {
                    var last = LastKnown(sym);
                    if (price > 0) { result[sym] = (price, last.Change); Remember(sym, price, last.Change); }
                    else if (last.Price > 0) result[sym] = (last.Price, last.Change);
                }
            }
            return result;
        }

        public static async Task<(decimal Price, decimal Change24h)> GetSinglePriceAsync(string symbol)
        {
            if (!_coinIds.ContainsKey(symbol ?? ""))
                return (0, 0);
            var prices = await GetBatchPricesAsync(new[] { symbol });
            return prices.TryGetValue(symbol, out var p) ? p : (0, 0);
        }

        public static async Task<List<(long Timestamp, decimal Price)>> GetHistoricalPricesAsync(string symbol)
        {
            if (!_coinIds.TryGetValue(symbol, out var coinId))
                return new();

            var url = $"https://api.coingecko.com/api/v3/coins/{coinId}/market_chart?vs_currency=gbp&days=365";
            var json = await CoinGeckoGetAsync(url);
            var obj = Newtonsoft.Json.Linq.JObject.Parse(json);
            var prices = obj?["prices"] as Newtonsoft.Json.Linq.JArray;
            if (prices == null) return new();

            return prices.Select(p => (
                Timestamp: (long)p[0] / 1000,
                Price: (decimal)p[1]
            )).ToList();
        }

        public static decimal FindPriceAtTime(List<(long Timestamp, decimal Price)> history, DateTime date)
        {
            if (history.Count == 0) return 0;
            var unix = new DateTimeOffset(date).ToUnixTimeSeconds();
            var closest = history.OrderBy(p => Math.Abs(p.Timestamp - unix)).First();
            return closest.Price;
        }

        private static (int Score, string Label, string Description)? _fgCache;
        private static DateTime _fgCacheTime = DateTime.MinValue;

        public static async Task<(int Score, string Label, string Description)> GetFearGreedAsync()
        {
            if (_fgCache.HasValue && DateTime.UtcNow - _fgCacheTime < TimeSpan.FromMinutes(10))
                return _fgCache.Value;

            var json = await _http.GetStringAsync("https://api.alternative.me/fng/");
            var result = JsonConvert.DeserializeObject<FearGreedResponse>(json);
            var data = result?.data?.FirstOrDefault();

            if (data != null && int.TryParse(data.value, out var score))
            {
                var label = data.value_classification ?? "Unknown";
                var desc = score switch
                {
                    <= 25 => "Extreme fear — investors are very worried, potential buying opportunity.",
                    <= 45 => "Fear — market sentiment is cautious and uncertain.",
                    <= 55 => "Neutral — market sentiment is balanced.",
                    <= 75 => "Greed — investors are confident and optimistic.",
                    _ => "Extreme greed — market may be overheated, caution advised.",
                };
                _fgCache = (score, label, desc);
                _fgCacheTime = DateTime.UtcNow;
                return _fgCache.Value;
            }

            return (50, "Neutral", "Unable to fetch live sentiment data.");
        }

        private class FearGreedResponse
        {
            public List<FearGreedData> data { get; set; }
        }

        private class FearGreedData
        {
            public string value { get; set; }
            public string value_classification { get; set; }
        }

        private class CoinGeckoMarket
        {
            public string id { get; set; }
            public string symbol { get; set; }
            public string name { get; set; }
            public decimal current_price { get; set; }
            public decimal market_cap { get; set; }
            public int market_cap_rank { get; set; }
            public decimal? price_change_percentage_24h { get; set; }
            public decimal total_volume { get; set; }
        }
    }
}
