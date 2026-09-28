using SmartCubeMobile.MockData;
using System.Globalization;

namespace SmartCubeMobile.Dashboard
{
    public partial class SwitchSupplierFlow : ContentPage
    {
        public event Action SupplierChanged;

        private readonly MockSupplier currentSupplier;
        private readonly CultureInfo culture = new("en-GB");

        private static readonly Dictionary<string, List<AlternativeSupplier>> Alternatives = new()
        {
            ["Electricity"] = new()
            {
                new("Octopus Energy", "⚡", "Flexible Octopus", "24.03p/kWh · 46.36p/day standing", 89.20m, "Save ~£5/mo", true),
                new("OVO Energy", "⚡", "Better Energy", "24.89p/kWh · 49.52p/day standing", 91.80m, "Save ~£3/mo", false),
                new("EDF Energy", "⚡", "Simply Fixed", "23.15p/kWh · 51.84p/day standing", 87.50m, "Save ~£7/mo · Fixed 12 months", true),
                new("E.ON Next", "⚡", "Next Flex", "25.10p/kWh · 48.93p/day standing", 93.40m, "Similar cost", false),
                new("Scottish Power", "⚡", "Standard Variable", "25.85p/kWh · 53.00p/day standing", 96.10m, "+£2/mo more", false),
                new("Shell Energy", "⚡", "Flexible", "24.50p/kWh · 50.18p/day standing", 90.60m, "Save ~£4/mo", false),
            },
            ["Gas"] = new()
            {
                new("Octopus Energy", "🔥", "Flexible Octopus", "5.98p/kWh · 29.85p/day standing", 38.50m, "Save ~£4/mo", true),
                new("OVO Energy", "🔥", "Better Energy", "6.10p/kWh · 30.12p/day standing", 40.20m, "Save ~£2/mo", false),
                new("EDF Energy", "🔥", "Simply Fixed", "5.80p/kWh · 28.62p/day standing", 36.90m, "Save ~£5/mo · Fixed 12 months", true),
                new("E.ON Next", "🔥", "Next Flex", "6.30p/kWh · 31.50p/day standing", 41.80m, "Similar cost", false),
                new("Scottish Power", "🔥", "Standard Variable", "6.55p/kWh · 32.10p/day standing", 43.50m, "+£1/mo more", false),
            },
            ["Water"] = new()
            {
                new("South Staffs Water", "💧", "Measured Tariff", "£1.6820/m³ · 19.80p/day standing", 39.50m, "Save ~£3/mo", false),
                new("Severn Trent", "💧", "WaterSure", "Capped rate for qualifying households", 38.00m, "Save ~£4/mo if eligible", true),
                new("Severn Trent", "💧", "Unmeasured Tariff", "Fixed annual charge · no meter", 44.50m, "+£3/mo · predictable bills", false),
            },
            ["Broadband"] = new()
            {
                new("BT", "📡", "Fibre 2 (73Mbps)", "24 month contract", 32.99m, "Save ~£3/mo", false),
                new("Virgin Media", "📡", "M250 (264Mbps)", "18 month contract", 34.00m, "Save ~£2/mo · faster speed", true),
                new("Hyperoptic", "📡", "150Mbps", "12 month contract", 29.00m, "Save ~£7/mo", true),
                new("Plusnet", "📡", "Full Fibre 66", "24 month contract", 27.99m, "Save ~£8/mo", false),
                new("Vodafone", "📡", "Pro Xtra (900Mbps)", "24 month contract", 38.00m, "+£2/mo · ultrafast", false),
                new("TalkTalk", "📡", "Fibre 65 (67Mbps)", "18 month contract", 26.50m, "Save ~£10/mo", false),
            },
            ["Mobile"] = new()
            {
                new("Three", "📱", "Unlimited 30GB", "Unlimited mins & texts · 30GB data", 20.00m, "Save ~£8/mo", true),
                new("Vodafone", "📱", "Xtra 50GB", "Unlimited mins & texts · 50GB data", 25.00m, "Save ~£3/mo · more data", false),
                new("O2", "📱", "Airtime 40GB", "Unlimited mins & texts · 40GB data", 26.00m, "Save ~£2/mo", false),
                new("giffgaff", "📱", "Golden Goodybag 35GB", "Unlimited mins & texts · 35GB data", 18.00m, "Save ~£10/mo · no contract", true),
                new("Tesco Mobile", "📱", "SIM Only 25GB", "Unlimited mins & texts · 25GB data", 15.00m, "Save ~£13/mo", false),
                new("VOXI", "📱", "Endless Social 45GB", "Unlimited mins & texts · 45GB + social", 22.00m, "Save ~£6/mo", false),
            },
        };

        public SwitchSupplierFlow(MockSupplier supplier)
        {
            currentSupplier = supplier;
            InitializeComponent();
            LoadCurrentSupplier();
            LoadAlternatives();
        }

        private void LoadCurrentSupplier()
        {
            PageTitle.Text = $"Switch {currentSupplier.Type}";
            PageSubtitle.Text = $"Compare {currentSupplier.Type.ToLower()} deals";

            CurrentIcon.Text = currentSupplier.Icon;
            CurrentName.Text = $"{currentSupplier.Name} — {currentSupplier.Type}";
            CurrentTariff.Text = currentSupplier.Tariff;
            CurrentDetail.Text = currentSupplier.TariffDetail;
            CurrentCost.Text = $"{currentSupplier.MonthlyCost.ToString("C", culture)}/mo";
            CurrentRef.Text = $"Ref: {currentSupplier.AccountRef}";
        }

        private void LoadAlternatives()
        {
            AlternativesList.Children.Clear();

            if (!Alternatives.TryGetValue(currentSupplier.Type, out var alts))
                return;

            foreach (var alt in alts.OrderBy(a => a.MonthlyCost))
            {
                var savings = currentSupplier.MonthlyCost - alt.MonthlyCost;
                bool isCheaper = savings > 0;

                var card = new Border
                {
                    BackgroundColor = Color.FromArgb("#131B2E"),
                    Stroke = Color.FromArgb(alt.Recommended ? "#3B82F640" : "#1E2D4A"),
                    StrokeThickness = alt.Recommended ? 2 : 1,
                    StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 14 },
                    Padding = new Thickness(18, 14),
                };

                var grid = new Grid
                {
                    ColumnDefinitions = new ColumnDefinitionCollection
                    {
                        new ColumnDefinition(new GridLength(36)),
                        new ColumnDefinition(GridLength.Star),
                        new ColumnDefinition(GridLength.Auto),
                    },
                    ColumnSpacing = 14,
                };

                var icon = new Label
                {
                    Text = alt.Icon,
                    FontSize = 22,
                    VerticalOptions = LayoutOptions.Center,
                };
                Grid.SetColumn(icon, 0);

                var info = new VerticalStackLayout { Spacing = 2, VerticalOptions = LayoutOptions.Center };
                var nameRow = new HorizontalStackLayout { Spacing = 8 };
                nameRow.Children.Add(new Label
                {
                    Text = alt.Name,
                    TextColor = Color.FromArgb("#F1F5F9"),
                    FontSize = 14,
                    FontAttributes = FontAttributes.Bold,
                });
                if (alt.Recommended)
                {
                    nameRow.Children.Add(new Border
                    {
                        BackgroundColor = Color.FromArgb("#22C55E20"),
                        Stroke = Colors.Transparent,
                        StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 4 },
                        Padding = new Thickness(6, 2),
                        Content = new Label
                        {
                            Text = "Recommended",
                            TextColor = Color.FromArgb("#22C55E"),
                            FontSize = 9,
                            FontAttributes = FontAttributes.Bold,
                        }
                    });
                }
                info.Children.Add(nameRow);
                info.Children.Add(new Label { Text = alt.Tariff, TextColor = Color.FromArgb("#3B82F6"), FontSize = 12 });
                info.Children.Add(new Label { Text = alt.Detail, TextColor = Color.FromArgb("#64748B"), FontSize = 11 });
                info.Children.Add(new Label
                {
                    Text = alt.Comparison,
                    TextColor = Color.FromArgb(isCheaper ? "#22C55E" : savings < 0 ? "#EF4444" : "#94A3B8"),
                    FontSize = 11,
                    FontAttributes = FontAttributes.Bold,
                });
                Grid.SetColumn(info, 1);

                var rightCol = new VerticalStackLayout
                {
                    Spacing = 8,
                    VerticalOptions = LayoutOptions.Center,
                    HorizontalOptions = LayoutOptions.End,
                };
                rightCol.Children.Add(new Label
                {
                    Text = $"{alt.MonthlyCost.ToString("C", culture)}/mo",
                    TextColor = Color.FromArgb(isCheaper ? "#22C55E" : "#F59E0B"),
                    FontSize = 16,
                    FontAttributes = FontAttributes.Bold,
                    HorizontalTextAlignment = TextAlignment.End,
                });

                if (isCheaper)
                {
                    var annualSaving = savings * 12;
                    rightCol.Children.Add(new Label
                    {
                        Text = $"{annualSaving.ToString("C0", culture)}/yr saved",
                        TextColor = Color.FromArgb("#22C55E"),
                        FontSize = 11,
                        HorizontalTextAlignment = TextAlignment.End,
                    });
                }

                var switchBtn = new Button
                {
                    Text = "Switch",
                    FontSize = 12,
                    CornerRadius = 8,
                    HeightRequest = 34,
                    Padding = new Thickness(16, 0),
                };
                ToolTipProperties.SetText(switchBtn, $"Switch your {currentSupplier.Type.ToLower()} to {alt.Name} ({alt.Tariff}).");
                var altRef = alt;
                switchBtn.Clicked += async (s, e) => await OnSwitchClicked(altRef);
                rightCol.Children.Add(switchBtn);
                Grid.SetColumn(rightCol, 2);

                grid.Children.Add(icon);
                grid.Children.Add(info);
                grid.Children.Add(rightCol);
                card.Content = grid;

                ToolTipProperties.SetText(card, $"{alt.Name} — {alt.Tariff}, {alt.Comparison.ToLower()}.");
                card.GestureRecognizers.Add(new TapGestureRecognizer
                {
                    Command = new Command(async () =>
                    {
                        await card.ScaleTo(0.98, 60, Easing.CubicOut);
                        await card.ScaleTo(1.0, 60, Easing.CubicOut);
                    })
                });

                AlternativesList.Children.Add(card);
            }
        }

        private async Task OnSwitchClicked(AlternativeSupplier alt)
        {
            bool confirm = await DisplayAlert(
                "Switch Supplier",
                $"Switch your {currentSupplier.Type.ToLower()} to {alt.Name} ({alt.Tariff}) at {alt.MonthlyCost.ToString("C", culture)}/mo?",
                "Switch", "Cancel");

            if (!confirm) return;

            MockDataService.UpdateSupplier(currentSupplier.Type, alt.Name, alt.Tariff, alt.Detail, alt.MonthlyCost);
            SupplierChanged?.Invoke();
            await Navigation.PopAsync();
        }

        private async void OnBackClicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }

        private async void OnHelpClicked(object sender, EventArgs e)
        {
            await DisplayAlert("Switch Supplier",
                "This page compares your current supplier against alternative deals for the same service. Each card shows the tariff, price and how it compares to what you pay now; click Switch to confirm and update your supplier record. Prices shown here are example deals, not live quotes, and your supplier details are stored locally on this PC.",
                "OK");
        }

        private record AlternativeSupplier(string Name, string Icon, string Tariff, string Detail, decimal MonthlyCost, string Comparison, bool Recommended);
    }
}
