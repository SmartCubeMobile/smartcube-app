using SmartCubeMobile.MockData;
using SmartCubeMobile.Services;
using System.Globalization;

namespace SmartCubeMobile.Dashboard
{
    public partial class FullReportPage : ContentPage
    {
        private readonly CultureInfo culture = new("en-GB");

        public FullReportPage()
        {
            InitializeComponent();
            ReportDate.Text = $"Generated {DateTime.Now:dd MMM yyyy HH:mm}";

            _ = LoadAsync();
        }

        private async Task LoadAsync()
        {
            await BuildNetWorth();
            await BuildBanking();
            BuildUtility();
            BuildCrypto();
            BuildInvestments();
        }

        private async Task BuildNetWorth()
        {
            var accounts = await SmartDataService.GetAccounts();
            var bankTotal = accounts.Sum(a => a.TotalBalance);
            var cryptoTotal = SmartDataService.GetTotalCryptoValueGBP();
            var investmentTotal = SmartDataService.GetTotalInvestmentValueGBP();
            var netWorth = bankTotal + cryptoTotal + investmentTotal;

            AddMetric(NetWorthGrid, 0, 0, "Total Net Worth", netWorth.ToString("C", culture), "#3B82F6", 22);
            AddMetric(NetWorthGrid, 0, 1, "Banking", bankTotal.ToString("C", culture), "#22C55E");
            AddMetric(NetWorthGrid, 0, 2, "Crypto", cryptoTotal.ToString("C", culture), "#F59E0B");
            AddMetric(NetWorthGrid, 0, 3, "Investments", investmentTotal.ToString("C", culture), "#8B5CF6");

            var bankPct = netWorth > 0 ? bankTotal / netWorth * 100 : 0;
            var cryptoPct = netWorth > 0 ? cryptoTotal / netWorth * 100 : 0;
            var invPct = netWorth > 0 ? investmentTotal / netWorth * 100 : 0;

            var bills = SmartDataService.GetUtilityBills();
            var monthlyBills = bills.Where(b => b.BillDate >= DateTime.Now.AddMonths(-1)).Sum(b => b.Amount);

            AddMetric(NetWorthBreakdown, 0, 0, "Monthly Outgoings (Utilities)", monthlyBills.ToString("C", culture), "#EF4444");
            AddMetric(NetWorthBreakdown, 0, 1, "Banking %", $"{bankPct:F1}%", "#22C55E");
            AddMetric(NetWorthBreakdown, 0, 2, "Crypto %", $"{cryptoPct:F1}%", "#F59E0B");
            AddMetric(NetWorthBreakdown, 0, 3, "Investments %", $"{invPct:F1}%", "#8B5CF6");
        }

        private async Task BuildBanking()
        {
            var accounts = await SmartDataService.GetAccounts();
            var transactions = await SmartDataService.GetTransactions();

            foreach (var acc in accounts.OrderByDescending(a => a.TotalBalance))
            {
                AddDataRow(BankAccountsList,
                    acc.Institution,
                    acc.AccountName,
                    acc.TotalBalance.ToString("C", culture),
                    acc.AccountType == "Current" ? "#06B6D4" : acc.AccountType == "Savings" ? "#22C55E" : "#8B5CF6",
                    acc.AccountType);
            }

            var spending = transactions
                .Where(t => t.Amount < 0 && t.Category != "Transfer")
                .GroupBy(t => t.Category)
                .Select(g => new { Category = g.Key, Total = Math.Abs(g.Sum(t => t.Amount)) })
                .OrderByDescending(s => s.Total)
                .ToList();

            var colours = new[] { "#EF4444", "#F59E0B", "#3B82F6", "#8B5CF6", "#06B6D4", "#22C55E", "#EC4899" };
            for (int i = 0; i < spending.Count; i++)
            {
                var s = spending[i];
                var colour = colours[i % colours.Length];
                AddBarRow(SpendingList, s.Category, s.Total.ToString("C", culture), colour, s.Total, spending.Max(x => x.Total));
            }
        }

        private void BuildUtility()
        {
            var bills = SmartDataService.GetUtilityBills();

            var monthly = bills
                .GroupBy(b => b.Period)
                .Select(g => new { Period = g.Key, Total = g.Sum(b => b.Amount), Date = g.First().BillDate })
                .OrderByDescending(m => m.Date)
                .Take(6)
                .ToList();

            foreach (var m in monthly)
            {
                AddBarRow(UtilityMonthlyCosts, m.Period, m.Total.ToString("C", culture), "#F59E0B", m.Total, monthly.Max(x => x.Total));
            }

            var suppliers = bills
                .GroupBy(b => b.Supplier)
                .Select(g => new { Supplier = g.Key, Total = g.Sum(b => b.Amount), FuelTypes = string.Join(", ", g.Select(b => b.FuelType).Distinct()) })
                .OrderByDescending(s => s.Total)
                .ToList();

            foreach (var s in suppliers)
            {
                AddDataRow(UtilitySupplierList, s.Supplier, s.FuelTypes, s.Total.ToString("C", culture), "#F59E0B", "6 months");
            }
        }

        private void BuildCrypto()
        {
            var holdings = SmartDataService.GetCryptoHoldings();
            var totalValue = holdings.Sum(h => h.Quantity * h.PriceGBP);

            foreach (var h in holdings.OrderByDescending(h => h.Quantity * h.PriceGBP))
            {
                var value = h.Quantity * h.PriceGBP;
                var weight = totalValue > 0 ? value / totalValue * 100 : 0;
                var colour = h.Change24h >= 0 ? "#22C55E" : "#EF4444";

                var row = new Border
                {
                    BackgroundColor = Color.FromArgb("#1C2744"),
                    StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 8 },
                    Stroke = Colors.Transparent,
                    Padding = new Thickness(12, 10),
                };

                var grid = new Grid
                {
                    ColumnDefinitions = new ColumnDefinitionCollection
                    {
                        new ColumnDefinition(new GridLength(50)),
                        new ColumnDefinition(GridLength.Star),
                        new ColumnDefinition(GridLength.Auto),
                        new ColumnDefinition(GridLength.Auto),
                    },
                    ColumnSpacing = 10,
                };

                var sym = new Label { Text = h.Symbol, TextColor = Color.FromArgb("#F59E0B"), FontSize = 13, FontAttributes = FontAttributes.Bold, VerticalOptions = LayoutOptions.Center };
                var info = new VerticalStackLayout
                {
                    Spacing = 1, VerticalOptions = LayoutOptions.Center,
                    Children =
                    {
                        new Label { Text = h.Name, TextColor = Color.FromArgb("#94A3B8"), FontSize = 12 },
                        new Label { Text = $"{h.Quantity:G} {h.Symbol} · {weight:F1}%", TextColor = Color.FromArgb("#64748B"), FontSize = 10 },
                    }
                };
                var val = new Label { Text = value.ToString("C", culture), TextColor = Color.FromArgb("#F1F5F9"), FontSize = 14, FontAttributes = FontAttributes.Bold, VerticalOptions = LayoutOptions.Center };
                var change = new Label
                {
                    Text = $"{(h.Change24h >= 0 ? "+" : "")}{h.Change24h:F1}%",
                    TextColor = Color.FromArgb(colour), FontSize = 12, VerticalOptions = LayoutOptions.Center,
                };

                Grid.SetColumn(info, 1);
                Grid.SetColumn(val, 2);
                Grid.SetColumn(change, 3);

                grid.Children.Add(sym);
                grid.Children.Add(info);
                grid.Children.Add(val);
                grid.Children.Add(change);
                row.Content = grid;
                CryptoHoldingsList.Children.Add(row);
            }

            var assets = holdings.Count;
            var avg24h = holdings.Average(h => h.Change24h);

            AddMetric(CryptoKpiGrid, 0, 0, "Total Value", totalValue.ToString("C", culture), "#F59E0B");
            AddMetric(CryptoKpiGrid, 0, 1, "Assets", assets.ToString(), "#8B5CF6");
            AddMetric(CryptoKpiGrid, 1, 0, "Avg 24h Change", $"{(avg24h >= 0 ? "+" : "")}{avg24h:F1}%", avg24h >= 0 ? "#22C55E" : "#EF4444");
            AddMetric(CryptoKpiGrid, 1, 1, "Top Holding",
                holdings.OrderByDescending(h => h.Quantity * h.PriceGBP).First().Symbol,
                "#06B6D4");
        }

        private void BuildInvestments()
        {
            var investments = SmartDataService.GetInvestments();
            var totalValue = investments.Sum(i => i.Shares * i.PricePence / 100m);
            var totalCost = investments.Sum(i => i.Shares * i.CostBasisPence / 100m);
            var totalGain = totalValue - totalCost;
            var totalGainPct = totalCost > 0 ? totalGain / totalCost * 100 : 0;
            var avgYield = investments.Average(i => i.DividendYield);
            var estIncome = investments.Sum(i => i.Shares * i.PricePence / 100m * i.DividendYield / 100m);

            AddMetric(InvKpiGrid, 0, 0, "Portfolio Value", totalValue.ToString("C", culture), "#3B82F6");
            AddMetric(InvKpiGrid, 0, 1, "Total P&L",
                $"{(totalGain >= 0 ? "+" : "")}{totalGain.ToString("C", culture)} ({totalGainPct:F1}%)",
                totalGain >= 0 ? "#22C55E" : "#EF4444");
            AddMetric(InvKpiGrid, 0, 2, "Holdings", investments.Count.ToString(), "#8B5CF6");
            AddMetric(InvKpiGrid, 0, 3, "Avg Yield", $"{avgYield:F1}%", "#F59E0B");
            AddMetric(InvKpiGrid, 0, 4, "Est. Income", estIncome.ToString("C", culture), "#22C55E");

            foreach (var inv in investments.OrderByDescending(i => i.Shares * i.PricePence))
            {
                var value = inv.Shares * inv.PricePence / 100m;
                var cost = inv.Shares * inv.CostBasisPence / 100m;
                var pnl = value - cost;
                var pnlPct = cost > 0 ? pnl / cost * 100 : 0;

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
                        new ColumnDefinition(new GridLength(55)),
                        new ColumnDefinition(GridLength.Star),
                        new ColumnDefinition(new GridLength(80)),
                        new ColumnDefinition(new GridLength(90)),
                    },
                    ColumnSpacing = 8,
                };

                var sym = new Label { Text = inv.Symbol, TextColor = Color.FromArgb("#06B6D4"), FontSize = 12, FontAttributes = FontAttributes.Bold, VerticalOptions = LayoutOptions.Center };
                var name = new Label { Text = inv.Name, TextColor = Color.FromArgb("#94A3B8"), FontSize = 11, VerticalOptions = LayoutOptions.Center, LineBreakMode = LineBreakMode.TailTruncation };
                var val = new Label { Text = value.ToString("C", culture), TextColor = Color.FromArgb("#F1F5F9"), FontSize = 12, FontAttributes = FontAttributes.Bold, HorizontalTextAlignment = TextAlignment.End, VerticalOptions = LayoutOptions.Center };
                var pnlLabel = new Label
                {
                    Text = $"{(pnl >= 0 ? "+" : "")}{pnl.ToString("C", culture)}",
                    TextColor = pnl >= 0 ? Color.FromArgb("#22C55E") : Color.FromArgb("#EF4444"),
                    FontSize = 12, HorizontalTextAlignment = TextAlignment.End, VerticalOptions = LayoutOptions.Center,
                };

                Grid.SetColumn(name, 1);
                Grid.SetColumn(val, 2);
                Grid.SetColumn(pnlLabel, 3);

                grid.Children.Add(sym);
                grid.Children.Add(name);
                grid.Children.Add(val);
                grid.Children.Add(pnlLabel);
                row.Content = grid;
                InvHoldingsList.Children.Add(row);
            }

            var sectorColours = new[] { "#3B82F6", "#06B6D4", "#8B5CF6", "#F59E0B", "#22C55E", "#EF4444", "#EC4899", "#6366F1" };
            var sectors = investments
                .GroupBy(i => i.Sector)
                .Select(g => new { Sector = g.Key, Value = g.Sum(i => i.Shares * i.PricePence / 100m) })
                .OrderByDescending(s => s.Value)
                .ToList();

            for (int i = 0; i < sectors.Count; i++)
            {
                var s = sectors[i];
                var pct = totalValue > 0 ? s.Value / totalValue * 100 : 0;
                AddDotRow(InvSectorList, s.Sector, $"{pct:F1}%", sectorColours[i % sectorColours.Length]);
            }

            foreach (var inv in investments.Where(i => i.DividendYield > 0).OrderByDescending(i => i.DividendYield).Take(6))
            {
                var income = inv.Shares * inv.PricePence / 100m * inv.DividendYield / 100m;
                AddDotRow(InvDividendList, $"{inv.Symbol} ({inv.DividendYield:F1}%)", income.ToString("C", culture), "#22C55E");
            }
        }

        private void AddMetric(Grid grid, int row, int col, string title, string value, string colour, int fontSize = 18)
        {
            var stack = new VerticalStackLayout
            {
                Spacing = 4,
                Children =
                {
                    new Label { Text = title, TextColor = Color.FromArgb("#64748B"), FontSize = 11 },
                    new Label { Text = value, TextColor = Color.FromArgb(colour), FontSize = fontSize, FontAttributes = FontAttributes.Bold },
                }
            };
            Grid.SetRow(stack, row);
            Grid.SetColumn(stack, col);
            grid.Children.Add(stack);
        }

        private void AddDataRow(VerticalStackLayout list, string primary, string secondary, string value, string valueColour, string badge)
        {
            var row = new Border
            {
                BackgroundColor = Color.FromArgb("#1C2744"),
                StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 8 },
                Stroke = Colors.Transparent,
                Padding = new Thickness(12, 10),
            };

            var grid = new Grid
            {
                ColumnDefinitions = new ColumnDefinitionCollection
                {
                    new ColumnDefinition(GridLength.Star),
                    new ColumnDefinition(GridLength.Auto),
                    new ColumnDefinition(GridLength.Auto),
                },
                ColumnSpacing = 10,
            };

            var info = new VerticalStackLayout
            {
                Spacing = 1, VerticalOptions = LayoutOptions.Center,
                Children =
                {
                    new Label { Text = primary, TextColor = Color.FromArgb("#F1F5F9"), FontSize = 13, FontAttributes = FontAttributes.Bold },
                    new Label { Text = secondary, TextColor = Color.FromArgb("#64748B"), FontSize = 11 },
                }
            };
            var val = new Label { Text = value, TextColor = Color.FromArgb(valueColour), FontSize = 14, FontAttributes = FontAttributes.Bold, VerticalOptions = LayoutOptions.Center };
            var tag = new Label { Text = badge, TextColor = Color.FromArgb("#64748B"), FontSize = 10, VerticalOptions = LayoutOptions.Center };

            Grid.SetColumn(val, 1);
            Grid.SetColumn(tag, 2);

            grid.Children.Add(info);
            grid.Children.Add(val);
            grid.Children.Add(tag);
            row.Content = grid;
            list.Children.Add(row);
        }

        private void AddBarRow(VerticalStackLayout list, string label, string value, string colour, decimal amount, decimal max)
        {
            var pct = max > 0 ? (double)(amount / max) : 0;

            var row = new VerticalStackLayout { Spacing = 4, Padding = new Thickness(0, 2) };

            var header = new Grid
            {
                ColumnDefinitions = new ColumnDefinitionCollection
                {
                    new ColumnDefinition(GridLength.Star),
                    new ColumnDefinition(GridLength.Auto),
                },
            };
            var lbl = new Label { Text = label, TextColor = Color.FromArgb("#94A3B8"), FontSize = 12 };
            var val = new Label { Text = value, TextColor = Color.FromArgb("#F1F5F9"), FontSize = 12, FontAttributes = FontAttributes.Bold };
            Grid.SetColumn(val, 1);
            header.Children.Add(lbl);
            header.Children.Add(val);

            var barBg = new Border
            {
                BackgroundColor = Color.FromArgb("#1C2744"),
                StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 3 },
                Stroke = Colors.Transparent,
                HeightRequest = 6,
            };
            var barFill = new BoxView
            {
                Color = Color.FromArgb(colour),
                CornerRadius = 3,
                HeightRequest = 6,
                HorizontalOptions = LayoutOptions.Start,
                WidthRequest = Math.Max(4, pct * 280),
            };
            barBg.Content = barFill;

            row.Children.Add(header);
            row.Children.Add(barBg);
            list.Children.Add(row);
        }

        private void AddDotRow(VerticalStackLayout list, string label, string value, string colour)
        {
            var row = new Grid
            {
                ColumnDefinitions = new ColumnDefinitionCollection
                {
                    new ColumnDefinition(new GridLength(10)),
                    new ColumnDefinition(GridLength.Star),
                    new ColumnDefinition(GridLength.Auto),
                },
                ColumnSpacing = 8,
                Padding = new Thickness(0, 2),
            };

            var dot = new BoxView { Color = Color.FromArgb(colour), CornerRadius = 5, WidthRequest = 10, HeightRequest = 10, VerticalOptions = LayoutOptions.Center };
            var lbl = new Label { Text = label, TextColor = Color.FromArgb("#94A3B8"), FontSize = 12, VerticalOptions = LayoutOptions.Center };
            var val = new Label { Text = value, TextColor = Color.FromArgb(colour), FontSize = 12, FontAttributes = FontAttributes.Bold, VerticalOptions = LayoutOptions.Center };

            Grid.SetColumn(lbl, 1);
            Grid.SetColumn(val, 2);

            row.Children.Add(dot);
            row.Children.Add(lbl);
            row.Children.Add(val);
            list.Children.Add(row);
        }

        private async void OnBackClicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }

        private async void OnHelpClicked(object sender, EventArgs e)
        {
            await DisplayAlert("Full Report",
                "This page is a single, printable-style report covering your whole financial picture: net worth, banking, utilities, crypto and investments. Everything here is calculated from the accounts, bills, wallets and holdings you have already set up in SmartCube. There are no buttons to change data on this page, it is read-only. All figures come from data stored locally on this PC.",
                "OK");
        }
    }
}
