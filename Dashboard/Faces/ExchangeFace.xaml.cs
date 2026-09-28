using SmartCubeMobile.Dashboard;
using SmartCubeMobile.MockData;
using System.Globalization;

namespace SmartCubeMobile.Dashboard.Faces
{
    public partial class ExchangeFace : ContentView, IAnimatedFace
    {
        private readonly CultureInfo culture = new("en-GB");

        private static readonly Dictionary<string, CurrencyInfo> AllCurrencies = new()
        {
            { "GBP", new("British Pound", "£", "fiat") },
            { "EUR", new("Euro", "€", "fiat") },
            { "USD", new("US Dollar", "$", "fiat") },
            { "JPY", new("Japanese Yen", "¥", "fiat") },
            { "CHF", new("Swiss Franc", "CHF", "fiat") },
            { "AUD", new("Australian Dollar", "A$", "fiat") },
            { "CAD", new("Canadian Dollar", "C$", "fiat") },
            { "SEK", new("Swedish Krona", "kr", "fiat") },
            { "NOK", new("Norwegian Krone", "kr", "fiat") },
            { "DKK", new("Danish Krone", "kr", "fiat") },
            { "NZD", new("New Zealand Dollar", "NZ$", "fiat") },
            { "ZAR", new("South African Rand", "R", "fiat") },
            { "INR", new("Indian Rupee", "₹", "fiat") },
            { "SGD", new("Singapore Dollar", "S$", "fiat") },
            { "HKD", new("Hong Kong Dollar", "HK$", "fiat") },
            { "PLN", new("Polish Zloty", "zł", "fiat") },
            { "TRY", new("Turkish Lira", "₺", "fiat") },
            { "MXN", new("Mexican Peso", "MX$", "fiat") },
            { "BRL", new("Brazilian Real", "R$", "fiat") },
            { "THB", new("Thai Baht", "฿", "fiat") },
            { "BTC", new("Bitcoin", "₿", "crypto") },
            { "ETH", new("Ethereum", "Ξ", "crypto") },
            { "XRP", new("Ripple", "XRP", "crypto") },
            { "SOL", new("Solana", "SOL", "crypto") },
            { "ADA", new("Cardano", "ADA", "crypto") },
            { "DOGE", new("Dogecoin", "DOGE", "crypto") },
            { "DOT", new("Polkadot", "DOT", "crypto") },
            { "MATIC", new("Polygon", "MATIC", "crypto") },
            { "USDT", new("Tether", "USDT", "crypto") },
            { "USDC", new("USD Coin", "USDC", "crypto") },
        };

        private static readonly Dictionary<string, decimal> RatesToGBP = new()
        {
            { "GBP", 1m },
            { "EUR", 0.8412m }, { "USD", 0.7865m }, { "JPY", 0.005234m },
            { "CHF", 0.8821m }, { "AUD", 0.5124m }, { "CAD", 0.5732m },
            { "SEK", 0.0742m }, { "NOK", 0.0718m }, { "DKK", 0.1128m },
            { "NZD", 0.4698m }, { "ZAR", 0.0432m }, { "INR", 0.00932m },
            { "SGD", 0.5890m }, { "HKD", 0.1012m }, { "PLN", 0.1980m },
            { "TRY", 0.0230m }, { "MXN", 0.0458m }, { "BRL", 0.1520m },
            { "THB", 0.0224m },
            { "BTC", 56230m }, { "ETH", 2480.50m }, { "XRP", 1.82m },
            { "SOL", 145m }, { "ADA", 0.42m }, { "DOGE", 0.125m },
            { "DOT", 5.80m }, { "MATIC", 0.65m }, { "USDT", 0.79m }, { "USDC", 0.79m },
        };

        private static readonly Dictionary<string, decimal> PrevRatesToGBP = new()
        {
            { "GBP", 1m },
            { "EUR", 0.8398m }, { "USD", 0.7891m }, { "JPY", 0.005218m },
            { "CHF", 0.8805m }, { "AUD", 0.5141m }, { "CAD", 0.5718m },
            { "SEK", 0.0738m }, { "NOK", 0.0721m }, { "DKK", 0.1125m },
            { "NZD", 0.4712m }, { "ZAR", 0.0428m }, { "INR", 0.00928m },
            { "SGD", 0.5870m }, { "HKD", 0.1008m }, { "PLN", 0.1965m },
            { "TRY", 0.0232m }, { "MXN", 0.0462m }, { "BRL", 0.1505m },
            { "THB", 0.0221m },
            { "BTC", 54100m }, { "ETH", 2380m }, { "XRP", 1.78m },
            { "SOL", 138m }, { "ADA", 0.40m }, { "DOGE", 0.118m },
            { "DOT", 5.60m }, { "MATIC", 0.62m }, { "USDT", 0.79m }, { "USDC", 0.79m },
        };

        private static List<string> _tracked = new()
        {
            "EUR", "USD", "JPY", "CHF", "AUD", "CAD",
            "BTC", "ETH", "XRP"
        };

        private List<string> converterCodes;

        public ExchangeFace()
        {
            InitializeComponent();
            LoadData();
        }

        private void LoadData()
        {
            converterCodes = AllCurrencies.Keys.OrderBy(k =>
            {
                var info = AllCurrencies[k];
                return info.Type == "fiat" ? "0" + k : "1" + k;
            }).ToList();

            FromCurrency.ItemsSource = converterCodes;
            ToCurrency.ItemsSource = converterCodes;
            FromCurrency.SelectedItem = "GBP";
            ToCurrency.SelectedItem = "USD";

            RateDate.Text = $"Updated: {DateTime.Now:dd MMM yyyy HH:mm}";

            RefreshRatesList();
            DoConvert();

            AddCurrencyBtn.GestureRecognizers.Add(new TapGestureRecognizer
            {
                Command = new Command(OnAddCurrency)
            });
        }

        private void RefreshRatesList()
        {
            RatesList.ItemsSource = _tracked.Select(code =>
            {
                var info = AllCurrencies[code];
                var rate = RatesToGBP[code];
                var prev = PrevRatesToGBP.GetValueOrDefault(code, rate);
                decimal gbpToForeign, changePct;

                if (info.Type == "crypto")
                {
                    gbpToForeign = 1m / rate;
                    changePct = prev > 0 ? (rate - prev) / prev * 100 : 0;
                }
                else
                {
                    gbpToForeign = 1m / rate;
                    changePct = prev > 0 ? (rate - prev) / prev * 100 : 0;
                }

                return new RateDisplayItem
                {
                    Code = code,
                    CodeColour = Color.FromArgb(info.Type == "crypto" ? "#F59E0B" : "#06B6D4"),
                    TypeBadge = info.Type == "crypto" ? "CRYPTO" : "FIAT",
                    TypeColour = Color.FromArgb(info.Type == "crypto" ? "#F59E0B" : "#3B82F6"),
                    Name = info.Name,
                    RateFormatted = info.Type == "crypto"
                        ? $"£{rate:N2}"
                        : gbpToForeign.ToString("F4"),
                    ChangeFormatted = $"{(changePct >= 0 ? "+" : "")}{changePct:F2}%",
                    ChangeColour = changePct >= 0 ? Color.FromArgb("#22C55E") : Color.FromArgb("#EF4444"),
                };
            }).ToList();
        }

        private void OnAmountChanged(object sender, TextChangedEventArgs e) => DoConvert();
        private void OnCurrencyChanged(object sender, EventArgs e) => DoConvert();

        private void OnSwapClicked(object sender, EventArgs e)
        {
            var fromIdx = FromCurrency.SelectedIndex;
            var toIdx = ToCurrency.SelectedIndex;
            FromCurrency.SelectedIndex = toIdx;
            ToCurrency.SelectedIndex = fromIdx;

            if (decimal.TryParse(ToAmount.Text, out var val))
                FromAmount.Text = val.ToString("F2");
        }

        private void DoConvert()
        {
            if (FromCurrency.SelectedIndex < 0 || ToCurrency.SelectedIndex < 0) return;
            if (!decimal.TryParse(FromAmount.Text, out var amount)) return;

            var fromCode = converterCodes[FromCurrency.SelectedIndex];
            var toCode = converterCodes[ToCurrency.SelectedIndex];

            if (!RatesToGBP.ContainsKey(fromCode) || !RatesToGBP.ContainsKey(toCode)) return;

            var fromToGbp = RatesToGBP[fromCode];
            var toToGbp = RatesToGBP[toCode];

            var gbpAmount = amount * fromToGbp;
            var converted = gbpAmount / toToGbp;

            var rate = fromToGbp / toToGbp;
            var reverseRate = toToGbp / fromToGbp;

            var toInfo = AllCurrencies[toCode];
            ToAmount.Text = converted.ToString(toInfo.Type == "crypto" ? "G8" : "F2");

            RateLabel.Text = $"1 {fromCode} = {rate:G6} {toCode}";
            ReverseRateLabel.Text = $"1 {toCode} = {reverseRate:G6} {fromCode}";
        }

        private async void OnAddCurrency()
        {
            var available = AllCurrencies.Keys
                .Where(k => k != "GBP" && !_tracked.Contains(k))
                .OrderBy(k => AllCurrencies[k].Type == "fiat" ? "0" : "1")
                .ThenBy(k => k)
                .ToList();

            if (!available.Any())
            {
                await Application.Current.MainPage.DisplayAlert("All Added", "All available currencies are already tracked.", "OK");
                return;
            }

            var labels = available.Select(k =>
            {
                var info = AllCurrencies[k];
                var tag = info.Type == "crypto" ? "₿" : "💱";
                return $"{tag} {k} — {info.Name}";
            }).ToArray();

            var result = await Application.Current.MainPage.DisplayActionSheet(
                "Add Currency to Track", "Cancel", null, labels);

            if (string.IsNullOrEmpty(result) || result == "Cancel") return;

            var code = result.Split(' ')[1];
            if (AllCurrencies.ContainsKey(code) && !_tracked.Contains(code))
            {
                _tracked.Add(code);
                RefreshRatesList();
            }
        }

        private async void OnHelpClicked(object sender, EventArgs e)
        {
            var page = Application.Current?.Windows.FirstOrDefault()?.Page;
            if (page != null)
                await page.DisplayAlert("Exchange", "This section converts between currencies and cryptocurrencies using live exchange rates. Enter an amount and pick your From and To currencies, or tap the swap button to reverse them. Tracked Rates vs GBP lists live rates against the pound; use Add Currency to track more. Rates come from the European Central Bank and CoinGecko and are cached locally on this PC.", "OK");
        }

        public async Task PlayEntryAnimation()
        {
            AnimationHelper.PrepareForEntry(ConverterCard, SourceCard);
            _ = AnimationHelper.AnimateEntry(ConverterCard, 0, 400);
            _ = AnimationHelper.AnimateEntry(SourceCard, 150, 400);
            await AnimationHelper.SlideInFromRight(RatesCard, 400);
        }
    }

    public class RateDisplayItem
    {
        public string Code { get; set; }
        public Color CodeColour { get; set; }
        public string TypeBadge { get; set; }
        public Color TypeColour { get; set; }
        public string Name { get; set; }
        public string RateFormatted { get; set; }
        public string ChangeFormatted { get; set; }
        public Color ChangeColour { get; set; }
    }

    internal record CurrencyInfo(string Name, string Symbol, string Type);
}
