using SmartCubeMobile.MockData;
using System.Globalization;

namespace SmartCubeMobile.Dashboard
{
    public partial class AddInvestmentFlow : ContentPage
    {
        public event Action InvestmentAdded;

        private readonly CultureInfo culture = new("en-GB");

        private static readonly string[] Types = { "Share", "ETF", "Investment Trust" };
        private static readonly string[] Sectors = { "Technology", "Financials", "Healthcare", "Energy", "Telecoms", "Global Equity", "US Equity", "UK Equity Income", "Global Growth", "Consumer", "Industrials", "Real Estate" };
        private static readonly string[] Platforms = { "AJ Bell", "Hargreaves Lansdown", "Interactive Investor", "Vanguard", "Freetrade", "Trading 212", "eToro" };

        public AddInvestmentFlow()
        {
            InitializeComponent();
            SetupPickers();
        }

        private void SetupPickers()
        {
            foreach (var t in Types) TypePicker.Items.Add(t);
            TypePicker.SelectedIndex = 0;

            foreach (var s in Sectors) SectorPicker.Items.Add(s);
            SectorPicker.SelectedIndex = 0;

            foreach (var p in Platforms) PlatformPicker.Items.Add(p);
            PlatformPicker.SelectedIndex = 0;

            PurchaseDatePicker.MaximumDate = DateTime.Now;
            PurchaseDatePicker.Date = DateTime.Now.AddDays(-30);
        }

        private async void OnReviewClicked(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(SymbolEntry.Text))
            {
                await DisplayAlert("Missing", "Please enter a symbol / ticker.", "OK");
                return;
            }
            if (string.IsNullOrWhiteSpace(NameEntry.Text))
            {
                await DisplayAlert("Missing", "Please enter the company or fund name.", "OK");
                return;
            }
            if (!decimal.TryParse(SharesEntry.Text, out var shares) || shares <= 0)
            {
                await DisplayAlert("Invalid", "Please enter a valid number of shares.", "OK");
                return;
            }
            if (!decimal.TryParse(PriceEntry.Text, out var price) || price <= 0)
            {
                await DisplayAlert("Invalid", "Please enter a valid price per share.", "OK");
                return;
            }

            decimal.TryParse(FeeEntry.Text, out var fee);

            var totalCost = shares * price + fee;

            SummaryList.Children.Clear();
            AddSummaryRow("Type", Types[TypePicker.SelectedIndex]);
            AddSummaryRow("Symbol", SymbolEntry.Text.ToUpper());
            AddSummaryRow("Name", NameEntry.Text);
            AddSummaryRow("Sector", Sectors[SectorPicker.SelectedIndex]);
            AddSummaryRow("Date", PurchaseDatePicker.Date.ToString("dd MMM yyyy"));
            AddSummaryRow("Shares", shares.ToString("N0"));
            AddSummaryRow("Price", price.ToString("C", culture));
            AddSummaryRow("Fee", fee.ToString("C", culture));
            AddSummaryRow("Total Cost", totalCost.ToString("C", culture));
            AddSummaryRow("Platform", Platforms[PlatformPicker.SelectedIndex]);

            StepLabel.Text = "Step 2 of 2";
            PageSubtitle.Text = "Review and confirm";

            Step1.IsVisible = false;
            Step2.IsVisible = true;
            Step2.Opacity = 0;
            await Step2.FadeTo(1, 250, Easing.CubicOut);
        }

        private void AddSummaryRow(string label, string value)
        {
            var row = new Grid
            {
                ColumnDefinitions = new ColumnDefinitionCollection
                {
                    new ColumnDefinition(new GridLength(120)),
                    new ColumnDefinition(GridLength.Star),
                },
                ColumnSpacing = 12,
                Padding = new Thickness(0, 4),
            };

            var lbl = new Label
            {
                Text = label,
                TextColor = Color.FromArgb("#64748B"),
                FontSize = 13,
                VerticalOptions = LayoutOptions.Center,
            };

            var val = new Label
            {
                Text = value,
                TextColor = Color.FromArgb("#F1F5F9"),
                FontSize = 14,
                FontAttributes = FontAttributes.Bold,
                VerticalOptions = LayoutOptions.Center,
            };
            Grid.SetColumn(val, 1);

            row.Children.Add(lbl);
            row.Children.Add(val);
            SummaryList.Children.Add(row);
        }

        private async void OnEditClicked(object sender, EventArgs e)
        {
            StepLabel.Text = "Step 1 of 2";
            PageSubtitle.Text = "Enter purchase details";

            Step2.IsVisible = false;
            Step1.IsVisible = true;
            Step1.Opacity = 0;
            await Step1.FadeTo(1, 250, Easing.CubicOut);
        }

        private async void OnConfirmClicked(object sender, EventArgs e)
        {
            decimal.TryParse(SharesEntry.Text, out var shares);
            decimal.TryParse(PriceEntry.Text, out var price);
            decimal.TryParse(FeeEntry.Text, out var fee);

            var pricePence = price * 100m;
            var rng = new Random();
            var drift = (decimal)(rng.NextDouble() * 10 - 3);
            var currentPricePence = Math.Round(pricePence * (1 + drift / 100m), 0);

            var inv = new MockInvestment
            {
                Symbol = SymbolEntry.Text.Trim().ToUpper(),
                Name = NameEntry.Text.Trim(),
                Type = Types[TypePicker.SelectedIndex],
                Shares = (int)shares,
                PricePence = currentPricePence,
                CostBasisPence = pricePence,
                Change1D = Math.Round((decimal)(rng.NextDouble() * 4 - 2), 2),
                DividendYield = Math.Round((decimal)(rng.NextDouble() * 3), 1),
                Sector = Sectors[SectorPicker.SelectedIndex],
                Platform = Platforms[PlatformPicker.SelectedIndex],
            };

            MockDataService.AddInvestment(inv);
            InvestmentAdded?.Invoke();
            await Navigation.PopAsync();
        }

        private async void OnBackClicked(object sender, EventArgs e)
        {
            if (Step2.IsVisible)
            {
                await OnEditClicked_Async();
                return;
            }
            await Navigation.PopAsync();
        }

        private async Task OnEditClicked_Async()
        {
            StepLabel.Text = "Step 1 of 2";
            PageSubtitle.Text = "Enter purchase details";
            Step2.IsVisible = false;
            Step1.IsVisible = true;
            Step1.Opacity = 0;
            await Step1.FadeTo(1, 250, Easing.CubicOut);
        }

        private async void OnHelpClicked(object sender, EventArgs e)
        {
            await DisplayAlert("Add Investment",
                "This screen adds a share, ETF or investment trust you already own to your portfolio. Fill in the symbol, name and sector, then the purchase date, shares, price and fee, and tap Review to check the summary. 'Add to Portfolio' saves it and returns you to the dashboard. Everything is stored locally on this PC, not in the cloud.",
                "OK");
        }
    }
}
