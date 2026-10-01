using SmartCubeMobile.Dashboard;
using SmartCubeMobile.MockData;
using System.Globalization;

namespace SmartCubeMobile.Dashboard.Faces
{
    public partial class InvestmentsFace : ContentView, IAnimatedFace
    {
        private readonly CultureInfo culture = new("en-GB");

        public InvestmentsFace()
        {
            InitializeComponent();
            LoadData();
        }

        private void LoadData()
        {
            var investments = MockDataService.GetInvestments();
            var totalValue = MockDataService.GetTotalInvestmentValueGBP();
            var totalCost = investments.Sum(i => i.Shares * i.CostBasisPence / 100m);
            var totalGain = totalValue - totalCost;
            var totalGainPct = totalCost > 0 ? totalGain / totalCost * 100 : 0;

            BuildKpiCards(investments, totalValue, totalGain, totalGainPct);

            HoldingsCount.Text = $"{investments.Count} holdings";
            HoldingsList.ItemsSource = investments.OrderByDescending(i => i.Shares * i.PricePence).Select(i =>
            {
                var value = i.Shares * i.PricePence / 100m;
                var cost = i.Shares * i.CostBasisPence / 100m;
                var gain = value - cost;
                var gainPct = cost > 0 ? gain / cost * 100 : 0;

                return new InvestmentDisplayItem
                {
                    Source = i,
                    Symbol = i.Symbol,
                    Name = i.Name,
                    TypeBadge = i.Type.ToUpper(),
                    TypeColour = i.Type switch
                    {
                        "ETF" => Color.FromArgb("#3B82F6"),
                        "Investment Trust" => Color.FromArgb("#8B5CF6"),
                        _ => Color.FromArgb("#06B6D4"),
                    },
                    ValueFormatted = value.ToString("C", culture),
                    SharesFormatted = $"{i.Shares} shares @ £{(i.PricePence / 100m):F2}",
                    GainFormatted = $"{(gain >= 0 ? "+" : "")}{gain.ToString("C", culture)} ({gainPct:F1}%)",
                    GainColour = gain >= 0 ? Color.FromArgb("#22C55E") : Color.FromArgb("#EF4444"),
                    ChangeFormatted = $"{(i.Change1D >= 0 ? "+" : "")}{i.Change1D:F2}%",
                    ChangeColour = i.Change1D >= 0 ? Color.FromArgb("#22C55E") : Color.FromArgb("#EF4444"),
                };
            }).ToList();

            HoldingsList.SelectionMode = SelectionMode.Single;
            HoldingsList.SelectionChanged += OnHoldingSelected;

            BuildSectorAllocation(investments);
            BuildDividendList(investments);

            var platform = investments.FirstOrDefault()?.Platform ?? "—";
            PlatformLabel.Text = platform;

            if (investments.Count == 0)
            {
                HoldingsCount.Text = "No holdings yet — tap + Add to record your first investment";
                SectorList.Children.Add(new Label { Text = "No holdings", TextColor = Color.FromArgb("#64748B"), FontSize = 12 });
                DividendList.Children.Add(new Label { Text = "No holdings", TextColor = Color.FromArgb("#64748B"), FontSize = 12 });
            }
        }

        private void BuildKpiCards(List<MockInvestment> investments, decimal totalValue, decimal totalGain, decimal totalGainPct)
        {
            var topEarner = investments
                .OrderByDescending(i => i.Shares * i.PricePence / 100m - i.Shares * i.CostBasisPence / 100m)
                .FirstOrDefault();
            var topGain = topEarner == null ? 0
                : topEarner.Shares * topEarner.PricePence / 100m - topEarner.Shares * topEarner.CostBasisPence / 100m;
            var avgYield = investments.Count == 0 ? 0 : investments.Average(i => i.DividendYield);

            var kpis = new[]
            {
                ("Portfolio Value", totalValue.ToString("C", culture), "", Color.FromArgb("#3B82F6")),
                ("Total Gain/Loss", $"{(totalGain >= 0 ? "+" : "")}{totalGain.ToString("C", culture)}",
                    $"{(totalGainPct >= 0 ? "+" : "")}{totalGainPct:F1}%",
                    totalGain >= 0 ? Color.FromArgb("#22C55E") : Color.FromArgb("#EF4444")),
                ("Top Earner", topEarner?.Symbol ?? "—", topEarner == null ? "" : $"+{topGain.ToString("C", culture)}", Color.FromArgb("#06B6D4")),
                ("Holdings", investments.Count.ToString(), $"{investments.Count(i => i.Type == "ETF")} ETFs · {investments.Count(i => i.Type == "Share")} Shares", Color.FromArgb("#8B5CF6")),
                ("Avg Yield", $"{avgYield:F1}%", "dividend", Color.FromArgb("#F59E0B")),
            };

            foreach (var (title, value, sub, accent) in kpis)
            {
                var card = new Border
                {
                    BackgroundColor = Color.FromArgb("#131B2E"),
                    Stroke = Color.FromArgb("#1E2D4A"),
                    StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 16 },
                    Padding = new Thickness(20, 16),
                    WidthRequest = 200,
                    Content = new VerticalStackLayout
                    {
                        Spacing = 4,
                        Children =
                        {
                            new Label { Text = title, TextColor = Color.FromArgb("#94A3B8"), FontSize = 12 },
                            new Label { Text = value, TextColor = accent, FontSize = 22, FontAttributes = FontAttributes.Bold },
                            new Label { Text = sub, TextColor = Color.FromArgb("#64748B"), FontSize = 11,
                                        IsVisible = !string.IsNullOrEmpty(sub) },
                        }
                    }
                };
                KpiCards.Children.Add(card);
            }
        }

        private void BuildSectorAllocation(List<MockInvestment> investments)
        {
            var sectors = investments
                .GroupBy(i => i.Sector)
                .Select(g => new { Sector = g.Key, Value = g.Sum(i => i.Shares * i.PricePence / 100m) })
                .OrderByDescending(s => s.Value)
                .ToList();

            var total = sectors.Sum(s => s.Value);
            var colours = new[] { "#3B82F6", "#06B6D4", "#8B5CF6", "#F59E0B", "#22C55E", "#EF4444", "#EC4899", "#6366F1" };

            for (int i = 0; i < sectors.Count; i++)
            {
                var s = sectors[i];
                var pct = total > 0 ? s.Value / total * 100 : 0;
                var colour = Color.FromArgb(colours[i % colours.Length]);

                var row = new Grid
                {
                    ColumnDefinitions = new ColumnDefinitionCollection
                    {
                        new ColumnDefinition(new GridLength(10)),
                        new ColumnDefinition(GridLength.Star),
                        new ColumnDefinition(GridLength.Auto),
                    },
                    ColumnSpacing = 8,
                };

                var dot = new BoxView { Color = colour, CornerRadius = 5, WidthRequest = 10, HeightRequest = 10, VerticalOptions = LayoutOptions.Center };
                var label = new Label { Text = s.Sector, TextColor = Color.FromArgb("#94A3B8"), FontSize = 12, VerticalOptions = LayoutOptions.Center };
                var val = new Label { Text = $"{pct:F1}%", TextColor = Color.FromArgb("#F1F5F9"), FontSize = 12, FontAttributes = FontAttributes.Bold, VerticalOptions = LayoutOptions.Center };

                Grid.SetColumn(dot, 0);
                Grid.SetColumn(label, 1);
                Grid.SetColumn(val, 2);

                row.Children.Add(dot);
                row.Children.Add(label);
                row.Children.Add(val);

                SectorList.Children.Add(row);
            }
        }

        private void BuildDividendList(List<MockInvestment> investments)
        {
            var top = investments.OrderByDescending(i => i.DividendYield).Take(5);

            foreach (var inv in top)
            {
                var row = new Grid
                {
                    ColumnDefinitions = new ColumnDefinitionCollection
                    {
                        new ColumnDefinition(new GridLength(50)),
                        new ColumnDefinition(GridLength.Star),
                        new ColumnDefinition(GridLength.Auto),
                    },
                    ColumnSpacing = 8,
                };

                var sym = new Label { Text = inv.Symbol, TextColor = Color.FromArgb("#06B6D4"), FontSize = 12, FontAttributes = FontAttributes.Bold, VerticalOptions = LayoutOptions.Center };
                var name = new Label { Text = inv.Name, TextColor = Color.FromArgb("#94A3B8"), FontSize = 11, VerticalOptions = LayoutOptions.Center, LineBreakMode = LineBreakMode.TailTruncation };
                var yield = new Label { Text = $"{inv.DividendYield:F1}%", TextColor = Color.FromArgb("#22C55E"), FontSize = 13, FontAttributes = FontAttributes.Bold, VerticalOptions = LayoutOptions.Center };

                Grid.SetColumn(sym, 0);
                Grid.SetColumn(name, 1);
                Grid.SetColumn(yield, 2);

                row.Children.Add(sym);
                row.Children.Add(name);
                row.Children.Add(yield);

                DividendList.Children.Add(row);
            }
        }

        public void RefreshData()
        {
            KpiCards.Children.Clear();
            SectorList.Children.Clear();
            DividendList.Children.Clear();
            LoadData();
        }

        private async void OnAddInvestmentClicked(object sender, EventArgs e)
        {
            var flow = new AddInvestmentFlow();
            flow.InvestmentAdded += () => MainThread.BeginInvokeOnMainThread(RefreshData);
            await Navigation.PushAsync(flow);
        }

        private async void OnHoldingSelected(object sender, SelectionChangedEventArgs e)
        {
            if (e.CurrentSelection.FirstOrDefault() is InvestmentDisplayItem item)
            {
                HoldingsList.SelectedItem = null;
                var page = new InvestmentDetailPage(item.Source);
                await Navigation.PushAsync(page);
            }
        }

        private async void OnHelpClicked(object sender, EventArgs e)
        {
            var page = Application.Current?.Windows.FirstOrDefault()?.Page;
            if (page != null) await page.DisplayAlert("Investments", "This section lists your shares, ETFs, trusts and funds, with their current value, cost and gain or loss. Tap + Add to record a new holding, or tap a holding in the list to see more detail. The panel on the right shows your sector allocation, top dividend yields and platform. All of this data is stored locally on this PC.", "OK");
        }

        public async Task PlayEntryAnimation()
        {
            AnimationHelper.PrepareForEntry(HoldingsCard, SectorCard, DividendCard, PlatformCard);
            _ = AnimationHelper.AnimateEntry(HoldingsCard, 0, 400);
            _ = AnimationHelper.AnimateEntry(SectorCard, 100, 400);
            _ = AnimationHelper.AnimateEntry(DividendCard, 200, 400);
            _ = AnimationHelper.AnimateEntry(PlatformCard, 300, 400);
        }
    }

    public class InvestmentDisplayItem
    {
        public MockInvestment Source { get; set; }
        public string Symbol { get; set; }
        public string Name { get; set; }
        public string TypeBadge { get; set; }
        public Color TypeColour { get; set; }
        public string ValueFormatted { get; set; }
        public string SharesFormatted { get; set; }
        public string GainFormatted { get; set; }
        public Color GainColour { get; set; }
        public string ChangeFormatted { get; set; }
        public Color ChangeColour { get; set; }
    }
}
