using SmartCubeMobile.Dashboard;
using SmartCubeMobile.MockData;
using SmartCubeMobile.Services;
using System.Globalization;

namespace SmartCubeMobile.Dashboard.Faces
{
    public partial class ChartsFace : ContentView, IAnimatedFace
    {
        private static readonly string[] PieColours =
            { "#3B82F6", "#8B5CF6", "#06B6D4", "#F59E0B", "#22C55E", "#EF4444", "#EC4899", "#14B8A6", "#F97316" };

        private readonly CultureInfo culture = new("en-GB");
        private List<MockTransaction> _allTransactions;
        private List<MockAccount> _allAccounts;
        private List<MockCryptoHolding> _cryptoHoldings;
        private List<string> _accountNames;

        public ChartsFace()
        {
            InitializeComponent();
            _ = LoadDataAsync();
        }

        private async Task LoadDataAsync()
        {
            _allTransactions = await SmartDataService.GetTransactions();
            _allAccounts = await SmartDataService.GetAccounts();
            _cryptoHoldings = SmartDataService.GetCryptoHoldings();

            _accountNames = new List<string> { "All Accounts" };
            _accountNames.AddRange(_allAccounts.Select(a => $"{a.Institution} — {a.AccountName}"));
            AccountPicker.ItemsSource = _accountNames;
            AccountPicker.SelectedIndex = 0;
        }

        private void OnAccountChanged(object sender, EventArgs e)
        {
            if (AccountPicker.SelectedIndex < 0 || _allTransactions == null) return;
            RefreshCharts();
        }

        private void RefreshCharts()
        {
            var idx = AccountPicker.SelectedIndex;
            List<MockTransaction> filtered;
            List<MockAccount> filteredAccounts;

            if (idx <= 0)
            {
                filtered = _allTransactions;
                filteredAccounts = _allAccounts;
            }
            else
            {
                var acct = _allAccounts[idx - 1];
                filtered = _allTransactions.Where(t => t.AccountName == acct.AccountName).ToList();
                filteredAccounts = new List<MockAccount> { acct };
            }

            var netWorthHistory = MockDataService.GetNetWorthHistory();
            var bankTotal = filteredAccounts.Sum(a => a.TotalBalance);
            var cryptoTotal = SmartDataService.GetTotalCryptoValueGBP();

            LoadKpis(filtered, filteredAccounts, _cryptoHoldings, bankTotal, cryptoTotal);
            LoadSpendingByCategory(filtered);
            LoadNetWorthTrend(netWorthHistory);
            BuildBankPieChart(filteredAccounts, bankTotal);
            BuildCryptoPieChart(_cryptoHoldings, cryptoTotal);
            BuildBudgetAnalysis(filtered);
            BuildSavingsSuggestions(filtered, filteredAccounts);
        }

        private void LoadKpis(List<MockTransaction> transactions, List<MockAccount> accounts,
            List<MockCryptoHolding> holdings, decimal bankTotal, decimal cryptoTotal)
        {
            var totalNw = bankTotal + cryptoTotal;
            NetWorthLabel.Text = totalNw.ToString("C0", culture);

            var now = DateTime.Now;
            var thisMonthSpend = transactions
                .Where(t => t.Amount < 0 && t.Date.Month == now.Month && t.Date.Year == now.Year)
                .Sum(t => Math.Abs(t.Amount));

            var lastMonth = now.AddMonths(-1);
            var lastMonthSpend = transactions
                .Where(t => t.Amount < 0 && t.Date.Month == lastMonth.Month && t.Date.Year == lastMonth.Year)
                .Sum(t => Math.Abs(t.Amount));

            if (thisMonthSpend == 0 && lastMonthSpend == 0)
            {
                var latestDate = transactions.Where(t => t.Amount < 0).Select(t => t.Date).DefaultIfEmpty(now).Max();
                thisMonthSpend = transactions
                    .Where(t => t.Amount < 0 && t.Date.Month == latestDate.Month && t.Date.Year == latestDate.Year)
                    .Sum(t => Math.Abs(t.Amount));
                var prevMonth = latestDate.AddMonths(-1);
                lastMonthSpend = transactions
                    .Where(t => t.Amount < 0 && t.Date.Month == prevMonth.Month && t.Date.Year == prevMonth.Year)
                    .Sum(t => Math.Abs(t.Amount));
                MonthlySpendLabel.Text = thisMonthSpend.ToString("C", culture);
                MonthlySpendCompare.Text = lastMonthSpend > 0
                    ? $"vs {lastMonthSpend.ToString("C0", culture)} prev month ({latestDate:MMM yy})"
                    : $"{latestDate:MMM yyyy}";
            }
            else
            {
                MonthlySpendLabel.Text = thisMonthSpend.ToString("C", culture);
                MonthlySpendCompare.Text = lastMonthSpend > 0
                    ? $"vs {lastMonthSpend.ToString("C0", culture)} last month"
                    : "current month to date";
            }

            if (lastMonthSpend > 0 && thisMonthSpend > 0)
            {
                var pctChange = (thisMonthSpend - lastMonthSpend) / lastMonthSpend * 100;
                NetWorthChangeLabel.Text = pctChange >= 0
                    ? $"Spending up {pctChange:F1}% vs last month"
                    : $"Spending down {Math.Abs(pctChange):F1}% vs last month";
                NetWorthChangeLabel.TextColor = pctChange >= 0
                    ? Color.FromArgb("#EF4444")
                    : Color.FromArgb("#22C55E");
            }
            else
            {
                NetWorthChangeLabel.Text = $"{transactions.Count:N0} transactions loaded";
            }

            BankTotalLabel.Text = bankTotal.ToString("C0", culture);
            BankCountLabel.Text = $"{accounts.Count} accounts";

            CryptoTotalLabel.Text = cryptoTotal.ToString("C0", culture);
            CryptoCountLabel.Text = $"{holdings.Count} assets";
        }

        private List<MockTransaction> _filteredTransactions;

        private void LoadSpendingByCategory(List<MockTransaction> transactions)
        {
            _filteredTransactions = transactions;
            SpendingBackBtn.IsVisible = false;
            SpendingTitle.Text = "Spending by Category";
            SpendingHint.Text = "Tap a category to drill down";
            SpendingHint.IsVisible = true;

            var expenses = transactions
                .Where(t => t.Amount < 0)
                .GroupBy(t => t.Category)
                .Where(g => g.Key != "Transfer")
                .Select(g => new { Category = g.Key, Total = g.Sum(x => Math.Abs(x.Amount)), Count = g.Count() })
                .OrderByDescending(x => x.Total)
                .ToList();

            if (!expenses.Any()) return;

            var maxCat = expenses.Max(c => c.Total);
            var grandTotal = expenses.Sum(c => c.Total);

            SpendingStack.Children.Clear();

            foreach (var (cat, idx) in expenses.Select((c, i) => (c, i)))
            {
                var colour = PieColours[idx % PieColours.Length];
                var pct = grandTotal > 0 ? cat.Total / grandTotal * 100 : 0;

                var row = new Border
                {
                    BackgroundColor = Color.FromArgb("#1C2744"),
                    StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 6 },
                    Stroke = Colors.Transparent,
                    Padding = new Thickness(10, 7),
                };

                var grid = new Grid
                {
                    ColumnDefinitions = new ColumnDefinitionCollection
                    {
                        new ColumnDefinition(new GridLength(4)),
                        new ColumnDefinition(GridLength.Star),
                        new ColumnDefinition(GridLength.Auto),
                        new ColumnDefinition(GridLength.Auto),
                    },
                    ColumnSpacing = 10,
                };

                var accent = new BoxView { Color = Color.FromArgb(colour), CornerRadius = 2 };
                Grid.SetColumn(accent, 0);

                var info = new VerticalStackLayout
                {
                    Spacing = 2,
                    Children =
                    {
                        new Label { Text = cat.Category, TextColor = Color.FromArgb("#F1F5F9"), FontSize = 13, FontAttributes = FontAttributes.Bold },
                        new Label { Text = $"{cat.Count} transactions · {pct:F0}%", TextColor = Color.FromArgb("#64748B"), FontSize = 10 },
                    }
                };
                Grid.SetColumn(info, 1);

                var amtLabel = new Label
                {
                    Text = cat.Total.ToString("C", culture),
                    TextColor = Color.FromArgb(colour),
                    FontSize = 13,
                    FontAttributes = FontAttributes.Bold,
                    VerticalOptions = LayoutOptions.Center,
                };
                Grid.SetColumn(amtLabel, 2);

                var chevron = new Label
                {
                    Text = "›",
                    TextColor = Color.FromArgb("#64748B"),
                    FontSize = 18,
                    VerticalOptions = LayoutOptions.Center,
                };
                Grid.SetColumn(chevron, 3);

                grid.Children.Add(accent);
                grid.Children.Add(info);
                grid.Children.Add(amtLabel);
                grid.Children.Add(chevron);
                row.Content = grid;

                var category = cat.Category;
                row.GestureRecognizers.Add(new TapGestureRecognizer
                {
                    Command = new Command(() => DrillDownCategory(category))
                });

                SpendingStack.Children.Add(row);
            }
        }

        private void DrillDownCategory(string category)
        {
            if (_filteredTransactions == null) return;

            SpendingBackBtn.IsVisible = true;
            SpendingTitle.Text = category;
            SpendingHint.Text = "Merchant breakdown";
            SpendingStack.Children.Clear();

            var merchants = _filteredTransactions
                .Where(t => t.Amount < 0 && t.Category == category)
                .GroupBy(t => t.Description)
                .Select(g => new
                {
                    Merchant = g.Key,
                    Total = g.Sum(x => Math.Abs(x.Amount)),
                    Count = g.Count(),
                    LastDate = g.Max(x => x.Date),
                    AvgAmount = g.Average(x => Math.Abs(x.Amount)),
                })
                .OrderByDescending(m => m.Total)
                .ToList();

            if (!merchants.Any()) return;

            var maxMerch = merchants.Max(m => m.Total);
            var total = merchants.Sum(m => m.Total);

            // Total header
            SpendingStack.Children.Add(new Border
            {
                BackgroundColor = Color.FromArgb("#1E3A5F"),
                StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 8 },
                Stroke = Colors.Transparent,
                Padding = new Thickness(12, 8),
                Content = new Grid
                {
                    ColumnDefinitions = new ColumnDefinitionCollection
                    {
                        new ColumnDefinition(GridLength.Star),
                        new ColumnDefinition(GridLength.Auto),
                    },
                    Children =
                    {
                        NewLabel($"{merchants.Count} merchants · {merchants.Sum(m => m.Count)} transactions", "#94A3B8", 11, FontAttributes.None, 0, 0),
                        NewLabel(total.ToString("C", culture), "#60A5FA", 14, FontAttributes.Bold, 0, 1),
                    }
                }
            });

            foreach (var m in merchants)
            {
                var pct = total > 0 ? m.Total / total * 100 : 0;
                var barW = maxMerch > 0 ? (double)(m.Total / maxMerch) * 180 : 0;

                var row = new Border
                {
                    BackgroundColor = Color.FromArgb("#1C2744"),
                    StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 6 },
                    Stroke = Colors.Transparent,
                    Padding = new Thickness(10, 7),
                    Content = new Grid
                    {
                        RowDefinitions = new RowDefinitionCollection
                        {
                            new RowDefinition(GridLength.Auto),
                            new RowDefinition(GridLength.Auto),
                        },
                        ColumnDefinitions = new ColumnDefinitionCollection
                        {
                            new ColumnDefinition(GridLength.Star),
                            new ColumnDefinition(GridLength.Auto),
                        },
                        RowSpacing = 3,
                        Children =
                        {
                            NewLabel(m.Merchant, "#F1F5F9", 12, FontAttributes.Bold, 0, 0),
                            NewLabel(m.Total.ToString("C", culture), "#F59E0B", 13, FontAttributes.Bold, 0, 1),
                            NewLabel($"{m.Count}x · avg {m.AvgAmount.ToString("C", culture)} · last {m.LastDate:dd MMM yy} · {pct:F0}%",
                                "#64748B", 10, FontAttributes.None, 1, 0, 2),
                        }
                    }
                };

                SpendingStack.Children.Add(row);
            }
        }

        private void OnSpendingBack(object sender, EventArgs e)
        {
            if (_filteredTransactions != null)
                LoadSpendingByCategory(_filteredTransactions);
        }

        private void OnSearchTextChanged(object sender, TextChangedEventArgs e)
        {
            var query = e.NewTextValue?.Trim();
            if (string.IsNullOrEmpty(query))
            {
                SearchResultsCard.IsVisible = false;
                return;
            }
            if (query.Length < 2) return;

            var transactions = _filteredTransactions ?? _allTransactions;
            if (transactions == null) return;

            var matches = transactions
                .Where(t => t.Amount < 0 &&
                    (t.Description?.Contains(query, StringComparison.OrdinalIgnoreCase) == true))
                .GroupBy(t => t.Description)
                .Select(g => new
                {
                    Merchant = g.Key,
                    Category = g.First().Category,
                    Total = g.Sum(x => Math.Abs(x.Amount)),
                    Count = g.Count(),
                    LastDate = g.Max(x => x.Date),
                    FirstDate = g.Min(x => x.Date),
                    AvgAmount = g.Average(x => Math.Abs(x.Amount)),
                })
                .OrderByDescending(m => m.Total)
                .Take(20)
                .ToList();

            SearchResultsTitle.Text = $"Search: \"{query}\" — {matches.Count} merchants found";
            SearchResultsCard.IsVisible = true;
            SearchResultsList.Children.Clear();

            if (!matches.Any())
            {
                SearchResultsList.Children.Add(new Label
                {
                    Text = "No matching merchants found",
                    TextColor = Color.FromArgb("#64748B"),
                    FontSize = 13,
                });
                return;
            }

            var grandTotal = matches.Sum(m => m.Total);
            var totalTxns = matches.Sum(m => m.Count);

            SearchResultsList.Children.Add(new Border
            {
                BackgroundColor = Color.FromArgb("#1E3A5F"),
                StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 8 },
                Stroke = Colors.Transparent,
                Padding = new Thickness(12, 8),
                Content = new Grid
                {
                    ColumnDefinitions = new ColumnDefinitionCollection
                    {
                        new ColumnDefinition(GridLength.Star),
                        new ColumnDefinition(GridLength.Auto),
                    },
                    Children =
                    {
                        NewLabel($"{totalTxns} transactions across {matches.Count} merchants", "#94A3B8", 11, FontAttributes.None, 0, 0),
                        NewLabel(grandTotal.ToString("C", culture), "#60A5FA", 14, FontAttributes.Bold, 0, 1),
                    }
                }
            });

            foreach (var m in matches)
            {
                var row = new Border
                {
                    BackgroundColor = Color.FromArgb("#1C2744"),
                    StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 8 },
                    Stroke = Colors.Transparent,
                    Padding = new Thickness(12, 8),
                    Content = new Grid
                    {
                        RowDefinitions = new RowDefinitionCollection
                        {
                            new RowDefinition(GridLength.Auto),
                            new RowDefinition(GridLength.Auto),
                            new RowDefinition(GridLength.Auto),
                        },
                        ColumnDefinitions = new ColumnDefinitionCollection
                        {
                            new ColumnDefinition(GridLength.Star),
                            new ColumnDefinition(GridLength.Auto),
                        },
                        RowSpacing = 3,
                        Children =
                        {
                            NewLabel(m.Merchant, "#F1F5F9", 13, FontAttributes.Bold, 0, 0),
                            NewLabel(m.Total.ToString("C", culture), "#F59E0B", 14, FontAttributes.Bold, 0, 1),
                            NewLabel($"{m.Category}", "#3B82F6", 11, FontAttributes.None, 1, 0),
                            NewLabel($"{m.Count}x · avg {m.AvgAmount.ToString("C", culture)}", "#94A3B8", 11, FontAttributes.None, 1, 1),
                            NewLabel($"{m.FirstDate:MMM yy} → {m.LastDate:MMM yy}", "#64748B", 10, FontAttributes.None, 2, 0, 2),
                        }
                    }
                };

                var category = m.Category;
                row.GestureRecognizers.Add(new TapGestureRecognizer
                {
                    Command = new Command(() => DrillDownCategory(category))
                });

                SearchResultsList.Children.Add(row);
            }
        }

        private void OnClearSearch(object sender, EventArgs e)
        {
            SearchEntry.Text = "";
            SearchResultsCard.IsVisible = false;
        }

        private void LoadNetWorthTrend(List<MockChartPoint> netWorth)
        {
            if (netWorth.Count == 0)
            {
                NetWorthList.ItemsSource = new List<NetWorthDisplayItem>
                {
                    new() { Month = "—", ValueFormatted = "No history yet", BarWidth = 0 }
                };
                return;
            }
            var maxNw = netWorth.Max(n => n.Value);
            NetWorthList.ItemsSource = netWorth.Select(n => new NetWorthDisplayItem
            {
                Month = n.Date.ToString("MMM yy"),
                ValueFormatted = n.Value.ToString("C0", culture),
                BarWidth = (double)(n.Value / maxNw) * 300,
            }).ToList();
        }

        private void BuildBankPieChart(List<MockAccount> accounts, decimal total)
        {
            if (total == 0)
            {
                var positiveAccounts = accounts.Where(a => a.TotalBalance > 0).ToList();
                total = positiveAccounts.Sum(a => a.TotalBalance);
                if (total == 0) return;
                accounts = positiveAccounts;
            }

            var positiveOnly = accounts.Where(a => a.TotalBalance > 0).ToList();
            var pieTotal = positiveOnly.Sum(a => a.TotalBalance);
            if (pieTotal == 0) return;

            var slices = positiveOnly
                .OrderByDescending(a => a.TotalBalance)
                .Select((a, i) => new PieSlice
                {
                    Label = $"{a.Institution} — {a.AccountName}",
                    Value = a.TotalBalance,
                    Colour = PieColours[i % PieColours.Length],
                }).ToList();

            BankPieView.Drawable = new PieDrawable(slices, pieTotal);
            BuildLegend(BankPieLegend, "Bank Accounts", slices, pieTotal);
        }

        private void BuildCryptoPieChart(List<MockCryptoHolding> holdings, decimal total)
        {
            if (total == 0)
            {
                CryptoPieLegend.Children.Add(new Label
                {
                    Text = "No holdings",
                    TextColor = Color.FromArgb("#64748B"),
                    FontSize = 13,
                });
                return;
            }

            var slices = holdings
                .OrderByDescending(h => h.Quantity * h.PriceGBP)
                .Select((h, i) => new PieSlice
                {
                    Label = $"{h.Name} ({h.Symbol})",
                    Value = h.Quantity * h.PriceGBP,
                    Colour = new[] { "#06B6D4", "#8B5CF6", "#F59E0B" }[i % 3],
                }).ToList();

            CryptoPieView.Drawable = new PieDrawable(slices, total);
            BuildLegend(CryptoPieLegend, "Crypto & Stocks", slices, total);
        }

        private void BuildBudgetAnalysis(List<MockTransaction> transactions)
        {
            var excludeCategories = new HashSet<string> { "Transfer", "Family", "Interest", "Other" };
            var expenses = transactions
                .Where(t => t.Amount < 0 && !excludeCategories.Contains(t.Category))
                .ToList();

            if (!expenses.Any()) return;

            var months = expenses
                .GroupBy(t => new { t.Date.Year, t.Date.Month })
                .Select(g => new { g.Key.Year, g.Key.Month, Total = g.Sum(x => Math.Abs(x.Amount)) })
                .OrderBy(m => m.Year).ThenBy(m => m.Month)
                .ToList();

            var monthCount = Math.Max(months.Count, 1);
            var avgMonthlySpend = months.Average(m => m.Total);

            var categorySummary = expenses
                .GroupBy(t => t.Category)
                .Select(g => new
                {
                    Category = g.Key,
                    TotalSpend = g.Sum(x => Math.Abs(x.Amount)),
                    MonthlyAvg = g.Sum(x => Math.Abs(x.Amount)) / monthCount,
                    TxnCount = g.Count(),
                })
                .OrderByDescending(c => c.MonthlyAvg)
                .ToList();

            var totalMonthlyBudget = categorySummary.Sum(c => c.MonthlyAvg);

            BudgetSubtitle.Text = $"Based on {monthCount} months of data — Avg {avgMonthlySpend.ToString("C0", culture)}/mo";

            BudgetList.Children.Clear();

            foreach (var cat in categorySummary)
            {
                var pct = totalMonthlyBudget > 0 ? cat.MonthlyAvg / totalMonthlyBudget * 100 : 0;
                var barPct = totalMonthlyBudget > 0 ? (double)(cat.MonthlyAvg / categorySummary[0].MonthlyAvg) : 0;
                var colourIdx = categorySummary.IndexOf(cat) % PieColours.Length;

                var row = new Border
                {
                    BackgroundColor = Color.FromArgb("#1C2744"),
                    StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 8 },
                    Stroke = Colors.Transparent,
                    Padding = new Thickness(12, 8),
                    Content = new Grid
                    {
                        RowDefinitions = new RowDefinitionCollection
                        {
                            new RowDefinition(GridLength.Auto),
                            new RowDefinition(GridLength.Auto),
                        },
                        ColumnDefinitions = new ColumnDefinitionCollection
                        {
                            new ColumnDefinition(GridLength.Star),
                            new ColumnDefinition(GridLength.Auto),
                        },
                        RowSpacing = 4,
                        Children =
                        {
                            NewLabel(cat.Category, "#F1F5F9", 13, FontAttributes.Bold, 0, 0),
                            NewLabel($"{cat.MonthlyAvg.ToString("C0", culture)}/mo", PieColours[colourIdx], 13, FontAttributes.Bold, 0, 1),
                            CreateBudgetBar(barPct, PieColours[colourIdx], 1, 0),
                            NewLabel($"{pct:F0}% · {cat.TxnCount} txns", "#64748B", 10, FontAttributes.None, 1, 1),
                        }
                    }
                };
                BudgetList.Children.Add(row);
            }

            var totalRow = new Border
            {
                BackgroundColor = Color.FromArgb("#1E3A5F"),
                StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 8 },
                Stroke = Colors.Transparent,
                Padding = new Thickness(12, 10),
                Content = new Grid
                {
                    ColumnDefinitions = new ColumnDefinitionCollection
                    {
                        new ColumnDefinition(GridLength.Star),
                        new ColumnDefinition(GridLength.Auto),
                    },
                    Children =
                    {
                        NewLabel("Suggested Monthly Budget", "#F1F5F9", 14, FontAttributes.Bold, 0, 0),
                        NewLabel(totalMonthlyBudget.ToString("C0", culture), "#22C55E", 16, FontAttributes.Bold, 0, 1),
                    }
                }
            };
            BudgetList.Children.Add(totalRow);
        }

        private void BuildSavingsSuggestions(List<MockTransaction> transactions, List<MockAccount> accounts)
        {
            var excludeCategories = new HashSet<string> { "Transfer", "Family", "Interest", "Other", "State Pension", "Income" };
            var expenses = transactions.Where(t => t.Amount < 0 && !excludeCategories.Contains(t.Category)).ToList();
            var income = transactions.Where(t => t.Amount > 0 && (t.Category == "State Pension" || t.Category == "Income")).ToList();

            if (!expenses.Any()) return;

            var months = expenses
                .GroupBy(t => new { t.Date.Year, t.Date.Month })
                .ToList();
            var monthCount = Math.Max(months.Count, 1);

            var avgMonthlySpend = expenses.Sum(t => Math.Abs(t.Amount)) / monthCount;
            var avgMonthlyIncome = income.Any() ? income.Sum(t => t.Amount) / monthCount : 0;
            var monthlySurplus = avgMonthlyIncome - avgMonthlySpend;

            SavingsSubtitle.Text = monthlySurplus > 0
                ? $"Potential monthly surplus: {monthlySurplus.ToString("C0", culture)}"
                : $"Average monthly spend: {avgMonthlySpend.ToString("C0", culture)}";

            SavingsList.Children.Clear();

            var catMonthly = expenses
                .GroupBy(t => t.Category)
                .Select(g => new { Category = g.Key, MonthlyAvg = g.Sum(x => Math.Abs(x.Amount)) / monthCount })
                .OrderByDescending(c => c.MonthlyAvg)
                .ToList();

            // Eating Out savings
            var eatingOut = catMonthly.FirstOrDefault(c => c.Category == "Eating Out");
            if (eatingOut != null && eatingOut.MonthlyAvg > 30)
            {
                var saving = Math.Round(eatingOut.MonthlyAvg * 0.3m, 0);
                AddSavingCard("Reduce Eating Out",
                    $"You spend ~{eatingOut.MonthlyAvg.ToString("C0", culture)}/mo on eating out. " +
                    $"Cutting back 30% could save {saving.ToString("C0", culture)}/mo ({(saving * 12).ToString("C0", culture)}/yr).",
                    saving, "#F59E0B");
            }

            // Cash withdrawal analysis
            var cash = catMonthly.FirstOrDefault(c => c.Category == "Cash");
            if (cash != null && cash.MonthlyAvg > 50)
            {
                var saving = Math.Round(cash.MonthlyAvg * 0.4m, 0);
                AddSavingCard("Reduce Cash Withdrawals",
                    $"You withdraw ~{cash.MonthlyAvg.ToString("C0", culture)}/mo in cash. " +
                    $"Cash spending is harder to track. Switching to card could save {saving.ToString("C0", culture)}/mo.",
                    saving, "#EF4444");
            }

            // Shopping savings
            var shopping = catMonthly.FirstOrDefault(c => c.Category == "Shopping");
            if (shopping != null && shopping.MonthlyAvg > 50)
            {
                var saving = Math.Round(shopping.MonthlyAvg * 0.25m, 0);
                AddSavingCard("Review Non-Essential Shopping",
                    $"Retail spending averages {shopping.MonthlyAvg.ToString("C0", culture)}/mo. " +
                    $"Setting a {(shopping.MonthlyAvg * 0.75m).ToString("C0", culture)}/mo cap could save {saving.ToString("C0", culture)}/mo.",
                    saving, "#8B5CF6");
            }

            // Groceries comparison
            var groceries = catMonthly.FirstOrDefault(c => c.Category == "Groceries");
            if (groceries != null)
            {
                var topStores = expenses
                    .Where(t => t.Category == "Groceries")
                    .GroupBy(t => t.Description)
                    .Select(g => new { Store = g.Key, Monthly = g.Sum(x => Math.Abs(x.Amount)) / monthCount })
                    .OrderByDescending(s => s.Monthly)
                    .Take(3)
                    .ToList();

                var storeList = string.Join(", ", topStores.Select(s => $"{s.Store} ({s.Monthly.ToString("C0", culture)}/mo)"));
                var saving = Math.Round(groceries.MonthlyAvg * 0.15m, 0);
                AddSavingCard("Optimise Grocery Spending",
                    $"Top stores: {storeList}. " +
                    $"Price comparison and own-brand switches could save ~{saving.ToString("C0", culture)}/mo.",
                    saving, "#22C55E");
            }

            // Investment suggestions based on surplus
            AddInvestmentSection(monthlySurplus, avgMonthlySpend, accounts);
        }

        private void AddInvestmentSection(decimal monthlySurplus, decimal avgMonthlySpend, List<MockAccount> accounts)
        {
            var header = new Label
            {
                Text = "Investment Options",
                TextColor = Color.FromArgb("#F1F5F9"),
                FontSize = 15,
                FontAttributes = FontAttributes.Bold,
                Margin = new Thickness(0, 8, 0, 4),
            };
            SavingsList.Children.Add(header);

            var totalBalance = accounts.Sum(a => a.TotalBalance);
            var emergencyFund = avgMonthlySpend * 3;

            if (totalBalance < emergencyFund)
            {
                AddInvestmentCard("Build Emergency Fund",
                    $"Aim for 3 months expenses ({emergencyFund.ToString("C0", culture)}). " +
                    $"Current balance: {totalBalance.ToString("C0", culture)}. " +
                    $"Consider a high-interest easy access saver (Chase 4.1%, Chip 4.51%).",
                    "#06B6D4", "Priority: High");
            }

            AddInvestmentCard("Cash ISA",
                "Tax-free savings up to £20,000/yr. Best rates: Trading 212 (5.1%), Plum (5.05%), Chip (4.84%). " +
                "No tax on interest — ideal for basic/higher rate taxpayers.",
                "#3B82F6", monthlySurplus > 100 ? $"Could save {(monthlySurplus * 0.5m).ToString("C0", culture)}/mo" : "When surplus allows");

            AddInvestmentCard("Stocks & Shares ISA",
                "Long-term growth potential. Low-cost index funds (Vanguard FTSE Global, LifeStrategy 60%). " +
                "Average 8-10% annual return over 10+ years. AJ Bell, Vanguard, or InvestEngine.",
                "#8B5CF6", "For 5+ year goals");

            if (monthlySurplus > 200)
            {
                AddInvestmentCard("Premium Bonds",
                    $"NS&I Premium Bonds — currently 4.0% prize rate. " +
                    $"Tax-free, capital guaranteed. Max £50,000. " +
                    $"Could invest {Math.Min(monthlySurplus * 0.25m, 500).ToString("C0", culture)}/mo.",
                    "#F59E0B", "Low risk");
            }

            AddInvestmentCard("Pension Top-Up",
                "SIPP contributions get 25% tax relief (basic rate) or 40% (higher). " +
                "Even small regular contributions compound significantly. AJ Bell, PensionBee, Vanguard.",
                "#14B8A6", "Tax-efficient long term");
        }

        private void AddSavingCard(string title, string detail, decimal monthlySaving, string colour)
        {
            var card = new Border
            {
                BackgroundColor = Color.FromArgb("#1C2744"),
                StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 10 },
                Stroke = Color.FromArgb(colour + "40"),
                StrokeThickness = 1,
                Padding = new Thickness(14, 10),
                Content = new Grid
                {
                    ColumnDefinitions = new ColumnDefinitionCollection
                    {
                        new ColumnDefinition(GridLength.Star),
                        new ColumnDefinition(GridLength.Auto),
                    },
                    RowDefinitions = new RowDefinitionCollection
                    {
                        new RowDefinition(GridLength.Auto),
                        new RowDefinition(GridLength.Auto),
                    },
                    RowSpacing = 4,
                    Children =
                    {
                        NewLabel(title, colour, 13, FontAttributes.Bold, 0, 0),
                        NewLabel($"Save ~{monthlySaving.ToString("C0", culture)}/mo", "#22C55E", 13, FontAttributes.Bold, 0, 1),
                        NewLabel(detail, "#94A3B8", 11, FontAttributes.None, 1, 0, 2),
                    }
                }
            };
            SavingsList.Children.Add(card);
        }

        private void AddInvestmentCard(string title, string detail, string colour, string badge)
        {
            var badgeLabel = new Label
            {
                Text = badge,
                TextColor = Color.FromArgb(colour),
                BackgroundColor = Color.FromArgb(colour + "20"),
                FontSize = 10,
                Padding = new Thickness(6, 2),
            };

            var card = new Border
            {
                BackgroundColor = Color.FromArgb("#131B2E"),
                StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 10 },
                Stroke = Color.FromArgb(colour + "30"),
                StrokeThickness = 1,
                Padding = new Thickness(14, 10),
                Content = new VerticalStackLayout
                {
                    Spacing = 4,
                    Children =
                    {
                        new Grid
                        {
                            ColumnDefinitions = new ColumnDefinitionCollection
                            {
                                new ColumnDefinition(GridLength.Star),
                                new ColumnDefinition(GridLength.Auto),
                            },
                            Children =
                            {
                                NewLabel(title, colour, 13, FontAttributes.Bold, 0, 0),
                            }
                        },
                        badgeLabel,
                        new Label { Text = detail, TextColor = Color.FromArgb("#94A3B8"), FontSize = 11, LineBreakMode = LineBreakMode.WordWrap },
                    }
                }
            };
            SavingsList.Children.Add(card);
        }

        private static Label NewLabel(string text, string colour, double size, FontAttributes attrs, int row, int col, int colSpan = 1)
        {
            var lbl = new Label
            {
                Text = text,
                TextColor = Color.FromArgb(colour),
                FontSize = size,
                FontAttributes = attrs,
                LineBreakMode = LineBreakMode.WordWrap,
            };
            Grid.SetRow(lbl, row);
            Grid.SetColumn(lbl, col);
            if (colSpan > 1) Grid.SetColumnSpan(lbl, colSpan);
            return lbl;
        }

        private static View CreateBudgetBar(double fraction, string colour, int row, int col)
        {
            var bar = new Border
            {
                BackgroundColor = Color.FromArgb("#0A0F1E"),
                StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 3 },
                Stroke = Colors.Transparent,
                HeightRequest = 6,
                HorizontalOptions = LayoutOptions.Fill,
                Content = new Border
                {
                    BackgroundColor = Color.FromArgb(colour),
                    StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 3 },
                    Stroke = Colors.Transparent,
                    HeightRequest = 6,
                    WidthRequest = Math.Max(fraction * 200, 4),
                    HorizontalOptions = LayoutOptions.Start,
                }
            };
            Grid.SetRow(bar, row);
            Grid.SetColumn(bar, col);
            return bar;
        }

        private void BuildLegend(VerticalStackLayout legend, string title,
            List<PieSlice> slices, decimal total)
        {
            legend.Children.Clear();

            legend.Children.Add(new Label
            {
                Text = title,
                TextColor = Color.FromArgb("#F1F5F9"),
                FontSize = 15,
                FontAttributes = FontAttributes.Bold,
                Margin = new Thickness(0, 0, 0, 4),
            });

            foreach (var slice in slices)
            {
                double pct = (double)(slice.Value / total) * 100;
                var row = new HorizontalStackLayout { Spacing = 8 };
                row.Children.Add(new BoxView
                {
                    Color = Color.FromArgb(slice.Colour),
                    WidthRequest = 10,
                    HeightRequest = 10,
                    CornerRadius = 2,
                    VerticalOptions = LayoutOptions.Center,
                });
                row.Children.Add(new Label
                {
                    Text = slice.Label,
                    TextColor = Color.FromArgb("#CBD5E1"),
                    FontSize = 12,
                    VerticalOptions = LayoutOptions.Center,
                    LineBreakMode = LineBreakMode.TailTruncation,
                    MaximumWidthRequest = 200,
                });
                legend.Children.Add(row);

                legend.Children.Add(new Label
                {
                    Text = $"{slice.Value.ToString("C0", culture)} ({pct:F1}%)",
                    TextColor = Color.FromArgb("#64748B"),
                    FontSize = 11,
                    Margin = new Thickness(18, -4, 0, 0),
                });
            }

            legend.Children.Add(new BoxView
            {
                HeightRequest = 1,
                Color = Color.FromArgb("#1E2D4A"),
                Margin = new Thickness(0, 4, 0, 0),
            });
            legend.Children.Add(new Label
            {
                Text = $"Total: {total.ToString("C0", culture)}",
                TextColor = Color.FromArgb("#F1F5F9"),
                FontSize = 13,
                FontAttributes = FontAttributes.Bold,
                Margin = new Thickness(18, 0, 0, 0),
            });
        }

        public async Task PlayEntryAnimation()
        {
            await AnimationHelper.StaggerIn(KpiRow, 100, 400);
            AnimationHelper.PrepareForEntry(SpendingCard, NetWorthCard, BankPieCard, CryptoPieCard, BudgetCard, InvestCard);
            _ = AnimationHelper.AnimateEntry(SpendingCard, 0, 450);
            _ = AnimationHelper.AnimateEntry(NetWorthCard, 100, 450);
            _ = AnimationHelper.AnimateEntry(BankPieCard, 200, 450);
            _ = AnimationHelper.AnimateEntry(CryptoPieCard, 300, 450);
            _ = AnimationHelper.AnimateEntry(BudgetCard, 400, 450);
            await AnimationHelper.AnimateEntry(InvestCard, 500, 450);
        }
    }

    public class PieDrawable : IDrawable
    {
        private readonly List<PieSlice> slices;
        private readonly decimal total;

        public PieDrawable(List<PieSlice> slices, decimal total)
        {
            this.slices = slices;
            this.total = total;
        }

        public void Draw(ICanvas canvas, RectF dirtyRect)
        {
            float cx = dirtyRect.Width / 2;
            float cy = dirtyRect.Height / 2;
            float radius = Math.Min(cx, cy) - 2;
            float currentAngle = -90f;

            foreach (var slice in slices)
            {
                float sweep = (float)((double)(slice.Value / total) * 360.0);
                if (sweep < 0.2f) continue;

                var path = new PathF();
                path.MoveTo(cx, cy);

                int segments = Math.Max(4, (int)(sweep / 2));
                for (int i = 0; i <= segments; i++)
                {
                    float angle = currentAngle + sweep * i / segments;
                    float rad = angle * MathF.PI / 180f;
                    path.LineTo(cx + radius * MathF.Cos(rad), cy + radius * MathF.Sin(rad));
                }
                path.Close();

                canvas.FillColor = Color.FromArgb(slice.Colour);
                canvas.FillPath(path);

                currentAngle += sweep;
            }
        }
    }

    public class PieSlice
    {
        public string Label { get; set; }
        public decimal Value { get; set; }
        public string Colour { get; set; }
    }

    public class SpendingDisplayItem
    {
        public string Category { get; set; }
        public string AmountFormatted { get; set; }
        public Color BarColour { get; set; }
        public double BarWidth { get; set; }
    }

    public class NetWorthDisplayItem
    {
        public string Month { get; set; }
        public string ValueFormatted { get; set; }
        public double BarWidth { get; set; }
    }
}
