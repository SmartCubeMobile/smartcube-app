using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace SmartCubeMobile.Services
{
    public class NewsItem
    {
        public string Title { get; set; }
        public string Summary { get; set; }
        public string Url { get; set; }
        public string Source { get; set; }
        public DateTime Published { get; set; }
        public string Sentiment { get; set; }
        public string Icon { get; set; }
    }

    public static class FinanceNewsService
    {
        private static readonly HttpClient _http = new();
        private static List<NewsItem> _cache;
        private static DateTime _cacheTime;

        private static readonly (string Url, string Source)[] _feeds =
        {
            ("https://cointelegraph.com/rss", "CoinTelegraph"),
            ("https://feeds.bbci.co.uk/news/business/rss.xml", "BBC Business"),
        };

        private static readonly string[] _bullish = {
            "surge", "soar", "rally", "gain", "jump", "rise", "bull", "record high",
            "all-time high", "boost", "recover", "rebound", "profit", "growth",
            "upgrade", "approve", "adopt", "launch", "partner", "expand",
            "beat", "outperform", "optimis", "confidence", "strong", "positive"
        };

        private static readonly string[] _bearish = {
            "crash", "plunge", "drop", "fall", "decline", "slump", "bear", "sell",
            "loss", "warn", "fear", "risk", "crisis", "inflation", "recession",
            "hack", "fraud", "ban", "restrict", "fine", "lawsuit", "investig",
            "scandal", "debt", "default", "bankrupt", "layoff", "cut", "downgrade",
            "sanction", "war", "tariff", "negative", "struggle", "worst"
        };

        private static readonly Dictionary<string, string> _topicIcons = new(StringComparer.OrdinalIgnoreCase)
        {
            { "bitcoin", "₿" }, { "btc", "₿" },
            { "ethereum", "Ξ" }, { "eth", "Ξ" },
            { "crypto", "🪙" },
            { "oil", "🛢" }, { "energy", "⚡" }, { "gas", "⚽" },
            { "bank", "🏦" }, { "interest rate", "🏦" }, { "fed ", "🏦" },
            { "stock", "📈" }, { "share", "📈" }, { "market", "📈" },
            { "gold", "🥇" },
            { "property", "🏠" }, { "house", "🏠" }, { "mortgage", "🏠" },
            { "tax", "💰" }, { "budget", "💰" },
            { "trade", "🌍" }, { "tariff", "🌍" }, { "export", "🌍" },
            { "tech", "💻" }, { "ai ", "🤖" },
            { "inflation", "📉" },
            { "job", "👔" }, { "employ", "👔" }, { "wage", "👔" },
        };

        static FinanceNewsService()
        {
            _http.DefaultRequestHeaders.UserAgent.ParseAdd("SmartCube/1.0");
            _http.Timeout = TimeSpan.FromSeconds(15);
        }

        public static void ClearCache() => _cache = null;

        public static async Task<List<NewsItem>> GetNewsAsync(int count = 12)
        {
            if (_cache != null && DateTime.UtcNow - _cacheTime < TimeSpan.FromMinutes(10))
                return _cache.Take(count).ToList();

            var all = new List<NewsItem>();

            foreach (var (url, source) in _feeds)
            {
                try
                {
                    var xml = await _http.GetStringAsync(url);
                    var doc = XDocument.Parse(xml);
                    var items = doc.Descendants("item").Take(10);

                    foreach (var item in items)
                    {
                        var title = item.Element("title")?.Value?.Trim();
                        var desc = item.Element("description")?.Value?.Trim();
                        var link = item.Element("link")?.Value?.Trim();
                        var pubDate = item.Element("pubDate")?.Value;

                        if (string.IsNullOrEmpty(title)) continue;

                        DateTime.TryParse(pubDate, out var published);

                        desc = StripHtml(desc);
                        if (!string.IsNullOrEmpty(desc) && desc.Length > 160)
                            desc = desc[..157] + "...";

                        var combined = $"{title} {desc}".ToLower();
                        var sentiment = ClassifySentiment(combined);
                        var icon = PickIcon(combined);

                        all.Add(new NewsItem
                        {
                            Title = title,
                            Summary = desc ?? "",
                            Url = link ?? "",
                            Source = source,
                            Published = published,
                            Sentiment = sentiment,
                            Icon = icon,
                        });
                    }
                }
                catch { }
            }

            _cache = all.OrderByDescending(n => n.Published).ToList();
            _cacheTime = DateTime.UtcNow;
            return _cache.Take(count).ToList();
        }

        private static string StripHtml(string html)
        {
            if (string.IsNullOrEmpty(html)) return "";
            var text = Regex.Replace(html, "<[^>]+>", " ");
            text = System.Net.WebUtility.HtmlDecode(text);
            text = Regex.Replace(text, @"\s+", " ").Trim();
            return text;
        }

        private static string ClassifySentiment(string text)
        {
            int bull = 0, bear = 0;
            foreach (var w in _bullish) if (text.Contains(w)) bull++;
            foreach (var w in _bearish) if (text.Contains(w)) bear++;
            if (bull > bear) return "bullish";
            if (bear > bull) return "bearish";
            return "neutral";
        }

        private static string PickIcon(string text)
        {
            foreach (var (keyword, icon) in _topicIcons)
                if (text.Contains(keyword)) return icon;
            return "💰";
        }
    }
}
