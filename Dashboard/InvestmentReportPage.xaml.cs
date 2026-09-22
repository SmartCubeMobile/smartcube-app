using SmartCubeMobile.MockData;
using System.Globalization;

namespace SmartCubeMobile.Dashboard
{
    public partial class InvestmentReportPage : ContentPage
    {
        private readonly CultureInfo culture = new("en-GB");
        private readonly List<MockInvestment> investments;

        public InvestmentReportPage()
        {
            InitializeComponent();
            investments = MockDataService.GetInvestments();
            ReportDate.Text = $"Generated {DateTime.Now:dd MMM yyyy HH:mm}";
            BuildSummary();
            BuildPerformance();
            BuildHoldingsTable();
            BuildSectorAllocation();
            BuildTypeBreakdown();
            BuildPerformers();
            BuildDividendIncome();
            BuildPlatformSummary();
            BuildRiskMetrics();
        }

        private void BuildSummary()
        {
            var totalValue = investments.Sum(i => i.Shares * i.PricePence / 100m);
            var totalCost = investments.Sum(i => i.Shares * i.CostBasisPence / 100m);
            var totalGain = totalValue - totalCost;
            var totalGainPct = totalCost > 0 ? totalGain / totalCost * 100 : 0;
            var holdings = investments.Count;
            var avgYield = investments.Average(i => i.DividendYield);

            AddMetric(SummaryGrid, 0, 0, "Total Value", totalValue.ToString("C", culture), "#3B82F6");
            AddMetric(SummaryGrid, 0, 1, "Total Cost", totalCost.ToString("C", culture), "#94A3B8");
            AddMetric(SummaryGrid, 0, 2, "Total P&L",
                $"{(totalGain >= 0 ? "+" : "")}{totalGain.ToString("C", culture)} ({totalGainPct:F1}%)",
                totalGain >= 0 ? "#22C55E" : "#EF4444");
            AddMetric(SummaryGrid, 1, 0, "Holdings", holdings.ToString(), "#8B5CF6");
            AddMetric(SummaryGrid, 1, 1, "Avg Yield", $"{avgYield:F1}%", "#F59E0B");
            AddMetric(SummaryGrid, 1, 2, "Est. Annual Income",
                (investments.Sum(i => i.Shares * i.PricePence / 100m * i.DividendYield / 100m)).ToString("C", culture),
                "#22C55E");
        }

        private void BuildPerformance()
        {
            var day1Change = investments.Sum(i =>
                i.Shares * i.PricePence / 100m * i.Change1D / 100m);
            var avgChange = investments.Average(i => i.Change1D);
            var winners = investments.Count(i => i.PricePence > i.CostBasisPence);
            var losers = investments.Count(i => i.PricePence < i.CostBasisPence);

            AddMetric(PerformanceGrid, 0, 0, "Today's Change",
                $"{(day1Change >= 0 ? "+" : "")}{day1Change.ToString("C", culture)}",
                day1Change >= 0 ? "#22C55E" : "#EF4444");
            AddMetric(PerformanceGrid, 0, 1, "Avg Daily %",
                $"{(avgChange >= 0 ? "+" : "")}{avgChange:F2}%",
                avgChange >= 0 ? "#22C55E" : "#EF4444");
            AddMetric(PerformanceGrid, 0, 2, "Winners", winners.ToString(), "#22C55E");
            AddMetric(PerformanceGrid, 0, 3, "Losers", losers.ToString(), "#EF4444");
        }

        private void BuildHoldingsTable()
        {
            var totalValue = investments.Sum(i => i.Shares * i.PricePence / 100m);

            foreach (var inv in investments.OrderByDescending(i => i.Shares * i.PricePence))
            {
                var value = inv.Shares * inv.PricePence / 100m;
                var cost = inv.Shares * inv.CostBasisPence / 100m;
                var pnl = value - cost;
                var pnlPct = cost > 0 ? pnl / cost * 100 : 0;
                var weight = totalValue > 0 ? value / totalValue * 100 : 0;

                var row = new Border
                {
                    BackgroundColor = Color.FromArgb("#1C2744"),
                    StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 6 },
                    Stroke = Colors.Transparent,
                    Padding = new Thickness(12, 8),
                };

                var grid = new Grid
                {
                    ColumnDefinitions = new ColumnDefinitionCollection
                    {
                        new ColumnDefinition(new GridLength(60)),
                        new ColumnDefinition(GridLength.Star),
                        new ColumnDefinition(new GridLength(80)),
                        new ColumnDefinition(new GridLength(90)),
                        new ColumnDefinition(new GridLength(90)),
                        new ColumnDefinition(new GridLength(80)),
                    },
                    ColumnSpacing = 8,
                };

                var sym = new Label { Text = inv.Symbol, TextColor = Color.FromArgb("#06B6D4"), FontSize = 12, FontAttributes = FontAttributes.Bold, VerticalOptions = LayoutOptions.Center };
                var name = new Label { Text = inv.Name, TextColor = Color.FromArgb("#94A3B8"), FontSize = 11, VerticalOptions = LayoutOptions.Center, LineBreakMode = LineBreakMode.TailTruncation };
                var shares = new Label { Text = inv.Shares.ToString("N0"), TextColor = Color.FromArgb("#F1F5F9"), FontSize = 12, HorizontalTextAlignment = TextAlignment.End, VerticalOptions = LayoutOptions.Center };
                var val = new Label { Text = value.ToString("C", culture), TextColor = Color.FromArgb("#F1F5F9"), FontSize = 12, FontAttributes = FontAttributes.Bold, HorizontalTextAlignment = TextAlignment.End, VerticalOptions = LayoutOptions.Center };
                var pnlLabel = new Label
                {
                    Text = $"{(pnl >= 0 ? "+" : "")}{pnl.ToString("C", culture)}",
                    TextColor = pnl >= 0 ? Color.FromArgb("#22C55E") : Color.FromArgb("#EF4444"),
                    FontSize = 12, HorizontalTextAlignment = TextAlignment.End, VerticalOptions = LayoutOptions.Center,
                };
                var weightLabel = new Label { Text = $"{weight:F1}%", TextColor = Color.FromArgb("#94A3B8"), FontSize = 12, HorizontalTextAlignment = TextAlignment.End, VerticalOptions = LayoutOptions.Center };

                Grid.SetColumn(name, 1);
                Grid.SetColumn(shares, 2);
                Grid.SetColumn(val, 3);
                Grid.SetColumn(pnlLabel, 4);
                Grid.SetColumn(weightLabel, 5);

                grid.Children.Add(sym);
                grid.Children.Add(name);
                grid.Children.Add(shares);
                grid.Children.Add(val);
                grid.Children.Add(pnlLabel);
                grid.Children.Add(weightLabel);

                row.Content = grid;
                HoldingsTable.Children.Add(row);
            }
        }

        private void BuildSectorAllocation()
        {
            var totalValue = investments.Sum(i => i.Shares * i.PricePence / 100m);
            var sectors = investments
                .GroupBy(i => i.Sector)
                .Select(g => new { Sector = g.Key, Value = g.Sum(i => i.Shares * i.PricePence / 100m), Count = g.Count() })
                .OrderByDescending(s => s.Value)
                .ToList();

            var colours = new[] { "#3B82F6", "#06B6D4", "#8B5CF6", "#F59E0B", "#22C55E", "#EF4444", "#EC4899", "#6366F1" };

            for (int i = 0; i < sectors.Count; i++)
            {
                var s = sectors[i];
                var pct = totalValue > 0 ? s.Value / totalValue * 100 : 0;
                var colour = Color.FromArgb(colours[i % colours.Length]);

                var row = new Grid
                {
                    ColumnDefinitions = new ColumnDefinitionCollection
                    {
                        new ColumnDefinition(new GridLength(10)),
                        new ColumnDefinition(GridLength.Star),
                        new ColumnDefinition(new GridLength(70)),
                        new ColumnDefinition(new GridLength(50)),
                    },
                    ColumnSpacing = 8,
                    Padding = new Thickness(0, 2),
                };

                var dot = new BoxView { Color = colour, CornerRadius = 5, WidthRequest = 10, HeightRequest = 10, VerticalOptions = LayoutOptions.Center };
                var label = new Label { Text = s.Sector, TextColor = Color.FromArgb("#94A3B8"), FontSize = 12, VerticalOptions = LayoutOptions.Center };
                var val = new Label { Text = s.Value.ToString("C", culture), TextColor = Color.FromArgb("#F1F5F9"), FontSize = 12, HorizontalTextAlignment = TextAlignment.End, VerticalOptions = LayoutOptions.Center };
                var pctLabel = new Label { Text = $"{pct:F1}%", TextColor = colour, FontSize = 12, FontAttributes = FontAttributes.Bold, HorizontalTextAlignment = TextAlignment.End, VerticalOptions = LayoutOptions.Center };

                Grid.SetColumn(label, 1);
                Grid.SetColumn(val, 2);
                Grid.SetColumn(pctLabel, 3);

                row.Children.Add(dot);
                row.Children.Add(label);
                row.Children.Add(val);
                row.Children.Add(pctLabel);

                SectorTable.Children.Add(row);
            }
        }

        private void BuildTypeBreakdown()
        {
            var totalValue = investments.Sum(i => i.Shares * i.PricePence / 100m);
            var types = investments
                .GroupBy(i => i.Type)
                .Select(g => new { Type = g.Key, Value = g.Sum(i => i.Shares * i.PricePence / 100m), Count = g.Count() })
                .OrderByDescending(t => t.Value)
                .ToList();

            var typeColours = new Dictionary<string, string>
            {
                { "ETF", "#3B82F6" },
                { "Share", "#06B6D4" },
                { "Investment Trust", "#8B5CF6" },
            };

            foreach (var t in types)
            {
                var pct = totalValue > 0 ? t.Value / totalValue * 100 : 0;
                var colour = Color.FromArgb(typeColours.GetValueOrDefault(t.Type, "#94A3B8"));

                var row = new Grid
                {
                    ColumnDefinitions = new ColumnDefinitionCollection
                    {
                        new ColumnDefinition(new GridLength(10)),
                        new ColumnDefinition(GridLength.Star),
                        new ColumnDefinition(new GridLength(70)),
                        new ColumnDefinition(new GridLength(50)),
                    },
                    ColumnSpacing = 8,
                    Padding = new Thickness(0, 2),
                };

                var dot = new BoxView { Color = colour, CornerRadius = 5, WidthRequest = 10, HeightRequest = 10, VerticalOptions = LayoutOptions.Center };
                var label = new Label { Text = $"{t.Type} ({t.Count})", TextColor = Color.FromArgb("#94A3B8"), FontSize = 12, VerticalOptions = LayoutOptions.Center };
                var val = new Label { Text = t.Value.ToString("C", culture), TextColor = Color.FromArgb("#F1F5F9"), FontSize = 12, HorizontalTextAlignment = TextAlignment.End, VerticalOptions = LayoutOptions.Center };
                var pctLabel = new Label { Text = $"{pct:F1}%", TextColor = colour, FontSize = 12, FontAttributes = FontAttributes.Bold, HorizontalTextAlignment = TextAlignment.End, VerticalOptions = LayoutOptions.Center };

                Grid.SetColumn(label, 1);
                Grid.SetColumn(val, 2);
                Grid.SetColumn(pctLabel, 3);

                row.Children.Add(dot);
                row.Children.Add(label);
                row.Children.Add(val);
                row.Children.Add(pctLabel);

                TypeTable.Children.Add(row);
            }
        }

        private void BuildPerformers()
        {
            var ranked = investments
                .Select(i => new
                {
                    Inv = i,
                    Gain = i.Shares * i.PricePence / 100m - i.Shares * i.CostBasisPence / 100m,
                    GainPct = i.CostBasisPence > 0 ? (i.PricePence - i.CostBasisPence) / i.CostBasisPence * 100 : 0,
                })
                .OrderByDescending(x => x.GainPct)
                .ToList();

            foreach (var item in ranked.Take(5))
                AddPerformerRow(TopList, item.Inv.Symbol, item.Inv.Name, item.Gain, item.GainPct);

            foreach (var item in ranked.TakeLast(3).Reverse())
                AddPerformerRow(BottomList, item.Inv.Symbol, item.Inv.Name, item.Gain, item.GainPct);
        }

        private void AddPerformerRow(VerticalStackLayout list, string symbol, string name, decimal gain, decimal gainPct)
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
                Padding = new Thickness(0, 3),
            };

            var sym = new Label { Text = symbol, TextColor = Color.FromArgb("#06B6D4"), FontSize = 12, FontAttributes = FontAttributes.Bold, VerticalOptions = LayoutOptions.Center };
            var nm = new Label { Text = name, TextColor = Color.FromArgb("#94A3B8"), FontSize = 11, VerticalOptions = LayoutOptions.Center, LineBreakMode = LineBreakMode.TailTruncation };
            var val = new Label
            {
                Text = $"{(gain >= 0 ? "+" : "")}{gain.ToString("C", culture)} ({gainPct:F1}%)",
                TextColor = gain >= 0 ? Color.FromArgb("#22C55E") : Color.FromArgb("#EF4444"),
                FontSize = 12, FontAttributes = FontAttributes.Bold, VerticalOptions = LayoutOptions.Center,
            };

            Grid.SetColumn(nm, 1);
            Grid.SetColumn(val, 2);

            row.Children.Add(sym);
            row.Children.Add(nm);
            row.Children.Add(val);
            list.Children.Add(row);
        }

        private void BuildDividendIncome()
        {
            var totalValue = investments.Sum(i => i.Shares * i.PricePence / 100m);
            var totalIncome = investments.Sum(i => i.Shares * i.PricePence / 100m * i.DividendYield / 100m);
            var avgYield = investments.Average(i => i.DividendYield);
            var payers = investments.Count(i => i.DividendYield > 0);

            AddMetric(DividendSummary, 0, 0, "Est. Annual Income", totalIncome.ToString("C", culture), "#22C55E");
            AddMetric(DividendSummary, 0, 1, "Portfolio Yield", $"{avgYield:F1}%", "#F59E0B");
            AddMetric(DividendSummary, 0, 2, "Dividend Payers", $"{payers} of {investments.Count}", "#8B5CF6");

            foreach (var inv in investments.Where(i => i.DividendYield > 0).OrderByDescending(i => i.DividendYield))
            {
                var income = inv.Shares * inv.PricePence / 100m * inv.DividendYield / 100m;

                var row = new Border
                {
                    BackgroundColor = Color.FromArgb("#1C2744"),
                    StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 6 },
                    Stroke = Colors.Transparent,
                    Padding = new Thickness(12, 8),
                };

                var grid = new Grid
                {
                    ColumnDefinitions = new ColumnDefinitionCollection
                    {
                        new ColumnDefinition(new GridLength(50)),
                        new ColumnDefinition(GridLength.Star),
                        new ColumnDefinition(new GridLength(60)),
                        new ColumnDefinition(new GridLength(90)),
                    },
                    ColumnSpacing = 8,
                };

                var sym = new Label { Text = inv.Symbol, TextColor = Color.FromArgb("#06B6D4"), FontSize = 12, FontAttributes = FontAttributes.Bold, VerticalOptions = LayoutOptions.Center };
                var nm = new Label { Text = inv.Name, TextColor = Color.FromArgb("#94A3B8"), FontSize = 11, VerticalOptions = LayoutOptions.Center, LineBreakMode = LineBreakMode.TailTruncation };
                var yld = new Label { Text = $"{inv.DividendYield:F1}%", TextColor = Color.FromArgb("#22C55E"), FontSize = 12, FontAttributes = FontAttributes.Bold, HorizontalTextAlignment = TextAlignment.End, VerticalOptions = LayoutOptions.Center };
                var inc = new Label { Text = income.ToString("C", culture), TextColor = Color.FromArgb("#F1F5F9"), FontSize = 12, HorizontalTextAlignment = TextAlignment.End, VerticalOptions = LayoutOptions.Center };

                Grid.SetColumn(nm, 1);
                Grid.SetColumn(yld, 2);
                Grid.SetColumn(inc, 3);

                grid.Children.Add(sym);
                grid.Children.Add(nm);
                grid.Children.Add(yld);
                grid.Children.Add(inc);
                row.Content = grid;
                DividendTable.Children.Add(row);
            }
        }

        private void BuildPlatformSummary()
        {
            var platforms = investments
                .GroupBy(i => i.Platform)
                .Select(g => new { Platform = g.Key, Value = g.Sum(i => i.Shares * i.PricePence / 100m), Count = g.Count() })
                .OrderByDescending(p => p.Value)
                .ToList();

            foreach (var p in platforms)
            {
                var row = new Grid
                {
                    ColumnDefinitions = new ColumnDefinitionCollection
                    {
                        new ColumnDefinition(GridLength.Auto),
                        new ColumnDefinition(GridLength.Star),
                        new ColumnDefinition(GridLength.Auto),
                        new ColumnDefinition(GridLength.Auto),
                    },
                    ColumnSpacing = 10,
                    Padding = new Thickness(0, 4),
                };

                var icon = new Label { Text = "🏦", FontSize = 16, VerticalOptions = LayoutOptions.Center };
                var name = new Label { Text = p.Platform, TextColor = Color.FromArgb("#F1F5F9"), FontSize = 13, FontAttributes = FontAttributes.Bold, VerticalOptions = LayoutOptions.Center };
                var count = new Label { Text = $"{p.Count} holding{(p.Count == 1 ? "" : "s")}", TextColor = Color.FromArgb("#64748B"), FontSize = 12, VerticalOptions = LayoutOptions.Center };
                var val = new Label { Text = p.Value.ToString("C", culture), TextColor = Color.FromArgb("#3B82F6"), FontSize = 14, FontAttributes = FontAttributes.Bold, VerticalOptions = LayoutOptions.Center };

                Grid.SetColumn(name, 1);
                Grid.SetColumn(count, 2);
                Grid.SetColumn(val, 3);

                row.Children.Add(icon);
                row.Children.Add(name);
                row.Children.Add(count);
                row.Children.Add(val);
                PlatformTable.Children.Add(row);
            }
        }

        private void BuildRiskMetrics()
        {
            var totalValue = investments.Sum(i => i.Shares * i.PricePence / 100m);
            var sectorCount = investments.Select(i => i.Sector).Distinct().Count();
            var typeCount = investments.Select(i => i.Type).Distinct().Count();

            var largest = investments.Max(i => i.Shares * i.PricePence / 100m);
            var concentration = totalValue > 0 ? largest / totalValue * 100 : 0;

            var topName = investments.OrderByDescending(i => i.Shares * i.PricePence).First().Symbol;

            AddMetric(RiskGrid, 0, 0, "Sectors", sectorCount.ToString(), "#3B82F6");
            AddMetric(RiskGrid, 0, 1, "Asset Types", typeCount.ToString(), "#8B5CF6");
            AddMetric(RiskGrid, 0, 2, "Largest Position",
                $"{topName} ({concentration:F1}%)",
                concentration > 30 ? "#EF4444" : "#22C55E");
        }

        private void AddMetric(Grid grid, int row, int col, string title, string value, string colour)
        {
            var stack = new VerticalStackLayout
            {
                Spacing = 4,
                Children =
                {
                    new Label { Text = title, TextColor = Color.FromArgb("#64748B"), FontSize = 11 },
                    new Label { Text = value, TextColor = Color.FromArgb(colour), FontSize = 18, FontAttributes = FontAttributes.Bold },
                }
            };
            Grid.SetRow(stack, row);
            Grid.SetColumn(stack, col);
            grid.Children.Add(stack);
        }

        private async void OnBackClicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }
    }
}
