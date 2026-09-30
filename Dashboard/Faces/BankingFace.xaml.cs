using SmartCubeMobile.Dashboard;
using SmartCubeMobile.MockData;
using SmartCubeMobile.Services;
using System.Globalization;

namespace SmartCubeMobile.Dashboard.Faces
{
    public partial class BankingFace : ContentView, IAnimatedFace
    {
        private List<MockTransaction> allTransactions;
        private CultureInfo culture = new CultureInfo("en-GB");
        private string selectedAccountName;
        private Border selectedCard;

        private bool _hasLoaded;

        public BankingFace()
        {
            InitializeComponent();
        }

        private SmartCubeDashboard FindDashboard()
        {
            Element el = this;
            while (el != null)
            {
                if (el is SmartCubeDashboard d) return d;
                el = el.Parent;
            }
            return null;
        }

        public async Task ReloadAsync()
        {
            if (!_hasLoaded)
            {
                var cachedAccounts = SmartDataService.GetCachedAccounts();
                var cachedTransactions = SmartDataService.GetCachedTransactions();
                if (cachedAccounts != null && cachedAccounts.Count > 0)
                {
                    if (cachedTransactions != null)
                    {
                        var currentAcct = cachedAccounts
                            .Where(a => a.AccountName.EndsWith($" ({a.AccountType})") && a.AccountType != "Credit Card")
                            .GroupBy(a => a.AccountName[..^(a.AccountType.Length + 3)])
                            .ToDictionary(g => g.Key, g => g.First().AccountName);

                        foreach (var t in cachedTransactions)
                        {
                            if (currentAcct.TryGetValue(t.AccountName, out var mapped))
                                t.AccountName = mapped;
                        }
                    }

                    LoadFromCache(cachedAccounts, cachedTransactions ?? new List<MockTransaction>());
                    _hasLoaded = true;
                    return;
                }
            }

            await LoadLiveData();
        }

        private async Task LoadLiveData()
        {
            var dashboard = FindDashboard();

            if (dashboard != null)
                await dashboard.ShowLoading("Connecting to your bank...", "Refreshing access tokens");

            await SmartDataService.EnsureTokensFresh();

            if (dashboard != null)
                await dashboard.UpdateLoading("Fetching accounts...", "Retrieving balances and cards");

            await LoadDataAsync();
            _hasLoaded = true;

            if (dashboard != null)
                await dashboard.HideLoading();
        }

        private void LoadFromCache(List<MockAccount> accounts, List<MockTransaction> transactions)
        {
            AccountCards.Children.Clear();
            allTransactions = transactions;

            var totalBalance = accounts.Sum(a => a.TotalBalance);
            var totalCard = new Border
            {
                StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 16 },
                Stroke = Color.FromArgb("#6366F180"),
                StrokeThickness = 2,
                Padding = new Thickness(20),
                WidthRequest = 230,
                Opacity = 0,
                Background = new LinearGradientBrush
                {
                    StartPoint = new Point(0, 0),
                    EndPoint = new Point(1, 1),
                    GradientStops = new GradientStopCollection
                    {
                        new GradientStop(Color.FromArgb("#3B82F6"), 0),
                        new GradientStop(Color.FromArgb("#7C3AED"), 1),
                    }
                },
                Content = new VerticalStackLayout
                {
                    Spacing = 8,
                    Children =
                    {
                        new Label { Text = "Total Balance", TextColor = Color.FromArgb("#E0E7FF"), FontSize = 12 },
                        new Label { Text = totalBalance.ToString("C", culture), TextColor = Colors.White, FontSize = 24, FontAttributes = FontAttributes.Bold },
                        new Label { Text = $"{accounts.Count} accounts", TextColor = Color.FromArgb("#C7D2FE"), FontSize = 12 },
                    }
                }
            };
            ToolTipProperties.SetText(totalCard, "Shows your combined balance across all accounts; tap to see all transactions.");
            totalCard.GestureRecognizers.Add(new TapGestureRecognizer
            {
                Command = new Command(() => SelectAccount(null, totalCard))
            });
            AccountCards.Children.Add(totalCard);
            selectedCard = totalCard;

            foreach (var account in accounts)
            {
                var card = new Border
                {
                    BackgroundColor = Color.FromArgb("#131B2E"),
                    Stroke = Color.FromArgb("#1E2D4A"),
                    StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 16 },
                    Padding = new Thickness(20),
                    WidthRequest = 230,
                    Opacity = 0,
                    Content = new Grid
                    {
                        ColumnDefinitions = new ColumnDefinitionCollection
                        {
                            new ColumnDefinition(new GridLength(3)),
                            new ColumnDefinition(GridLength.Star),
                        },
                        ColumnSpacing = 14,
                        Children =
                        {
                            CreateAccentLine(Color.FromArgb("#3B82F6")),
                            CreateAccountContent(account, culture),
                        }
                    }
                };
                var acctName = account.AccountName;
                ToolTipProperties.SetText(card, "Shows this account's balance; tap to filter transactions to just this account.");
                card.GestureRecognizers.Add(new TapGestureRecognizer
                {
                    Command = new Command(() => SelectAccount(acctName, card))
                });
                AccountCards.Children.Add(card);
            }

            var addCard = new Border
            {
                BackgroundColor = Colors.Transparent,
                Stroke = Color.FromArgb("#2A3F6B"),
                StrokeThickness = 2,
                StrokeDashArray = new DoubleCollection { 6, 4 },
                StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 16 },
                Padding = new Thickness(20),
                WidthRequest = 180,
                Opacity = 0,
                Content = new VerticalStackLayout
                {
                    Spacing = 8,
                    VerticalOptions = LayoutOptions.Center,
                    HorizontalOptions = LayoutOptions.Center,
                    Children =
                    {
                        new Label { Text = "+", TextColor = Color.FromArgb("#3B82F6"), FontSize = 32, HorizontalOptions = LayoutOptions.Center },
                        new Label { Text = "Add Account", TextColor = Color.FromArgb("#3B82F6"), FontSize = 13, FontAttributes = FontAttributes.Bold, HorizontalOptions = LayoutOptions.Center },
                    }
                }
            };
            ToolTipProperties.SetText(addCard, "Connect a new bank account through TrueLayer; SmartCube never sees your bank password.");
            addCard.GestureRecognizers.Add(new TapGestureRecognizer
            {
                Command = new Command(OnAddAccount)
            });
            AccountCards.Children.Add(addCard);

            RunAnalysis(accounts);
            ShowTransactions(null);
            _ = PlayEntryAnimation();
        }

        private async Task LoadDataAsync()
        {
            AccountCards.Children.Clear();
            var accounts = await SmartDataService.GetAccounts();
            allTransactions = await SmartDataService.GetTransactions();

            var totalBalance = accounts.Sum(a => a.TotalBalance);
            var totalCard = new Border
            {
                StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 16 },
                Stroke = Color.FromArgb("#6366F180"),
                StrokeThickness = 2,
                Padding = new Thickness(20),
                WidthRequest = 230,
                Opacity = 0,
                Background = new LinearGradientBrush
                {
                    StartPoint = new Point(0, 0),
                    EndPoint = new Point(1, 1),
                    GradientStops = new GradientStopCollection
                    {
                        new GradientStop(Color.FromArgb("#3B82F6"), 0),
                        new GradientStop(Color.FromArgb("#7C3AED"), 1),
                    }
                },
                Content = new VerticalStackLayout
                {
                    Spacing = 8,
                    Children =
                    {
                        new Label { Text = "Total Balance", TextColor = Color.FromArgb("#E0E7FF"), FontSize = 12 },
                        new Label { Text = totalBalance.ToString("C", culture), TextColor = Colors.White, FontSize = 24, FontAttributes = FontAttributes.Bold },
                        new Label { Text = $"{accounts.Count} accounts", TextColor = Color.FromArgb("#C7D2FE"), FontSize = 12 },
                    }
                }
            };
            ToolTipProperties.SetText(totalCard, "Shows your combined balance across all accounts; tap to see all transactions.");
            totalCard.GestureRecognizers.Add(new TapGestureRecognizer
            {
                Command = new Command(() => SelectAccount(null, totalCard))
            });
            AccountCards.Children.Add(totalCard);
            selectedCard = totalCard;

            foreach (var account in accounts)
            {
                var card = new Border
                {
                    BackgroundColor = Color.FromArgb("#131B2E"),
                    Stroke = Color.FromArgb("#1E2D4A"),
                    StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 16 },
                    Padding = new Thickness(20),
                    WidthRequest = 230,
                    Opacity = 0,
                    Content = new Grid
                    {
                        ColumnDefinitions = new ColumnDefinitionCollection
                        {
                            new ColumnDefinition(new GridLength(3)),
                            new ColumnDefinition(GridLength.Star),
                        },
                        ColumnSpacing = 14,
                        Children =
                        {
                            CreateAccentLine(Color.FromArgb("#3B82F6")),
                            CreateAccountContent(account, culture),
                        }
                    }
                };
                var acctName = account.AccountName;
                ToolTipProperties.SetText(card, "Shows this account's balance; tap to filter transactions to just this account.");
                card.GestureRecognizers.Add(new TapGestureRecognizer
                {
                    Command = new Command(() => SelectAccount(acctName, card))
                });
                AccountCards.Children.Add(card);
            }

            var addCard = new Border
            {
                BackgroundColor = Colors.Transparent,
                Stroke = Color.FromArgb("#2A3F6B"),
                StrokeThickness = 2,
                StrokeDashArray = new DoubleCollection { 6, 4 },
                StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 16 },
                Padding = new Thickness(20),
                WidthRequest = 180,
                Opacity = 0,
                Content = new VerticalStackLayout
                {
                    Spacing = 8,
                    VerticalOptions = LayoutOptions.Center,
                    HorizontalOptions = LayoutOptions.Center,
                    Children =
                    {
                        new Label { Text = "+", TextColor = Color.FromArgb("#3B82F6"), FontSize = 32, HorizontalOptions = LayoutOptions.Center },
                        new Label { Text = "Add Account", TextColor = Color.FromArgb("#3B82F6"), FontSize = 13, FontAttributes = FontAttributes.Bold, HorizontalOptions = LayoutOptions.Center },
                    }
                }
            };
            ToolTipProperties.SetText(addCard, "Connect a new bank account through TrueLayer; SmartCube never sees your bank password.");
            addCard.GestureRecognizers.Add(new TapGestureRecognizer
            {
                Command = new Command(OnAddAccount)
            });
            AccountCards.Children.Add(addCard);

            RunAnalysis(accounts);
            ShowTransactions(null);
            _ = PlayEntryAnimation();
        }

        private void SelectAccount(string accountName, Border card)
        {
            if (selectedCard == card) return;

            if (selectedCard != null && selectedCard != AccountCards.Children[0])
            {
                selectedCard.Stroke = Color.FromArgb("#1E2D4A");
                selectedCard.StrokeThickness = 1;
                _ = selectedCard.ScaleTo(1.0, 150, Easing.CubicOut);
            }
            else if (selectedCard == AccountCards.Children[0])
            {
                selectedCard.Stroke = Color.FromArgb("#6366F130");
                selectedCard.StrokeThickness = 1;
                _ = selectedCard.ScaleTo(1.0, 150, Easing.CubicOut);
            }

            selectedCard = card;
            selectedAccountName = accountName;

            if (card == AccountCards.Children[0])
            {
                card.Stroke = Color.FromArgb("#6366F180");
                card.StrokeThickness = 2;
            }
            else
            {
                card.Stroke = Color.FromArgb("#3B82F6");
                card.StrokeThickness = 2;
            }
            _ = card.ScaleTo(1.03, 150, Easing.CubicOut);

            ShowTransactions(accountName);
        }

        private void ShowTransactions(string accountName)
        {
            var source = allTransactions ?? new List<MockTransaction>();
            IEnumerable<MockTransaction> filtered = accountName == null
                ? source
                : source.Where(t => t.AccountName == accountName);

            if (categoryFilter != null)
                filtered = filtered.Where(t => CategoryOf(t) == categoryFilter);
            if (merchantFilter != null)
                filtered = filtered.Where(t => analysis?.For(t)?.MerchantKey == merchantFilter);
            var list = filtered.ToList();

            var label = $"{list.Count} transactions";
            if (accountName != null) label += $" — {accountName}";
            TransactionCount.Text = label;

            FilterBar.IsVisible = merchantFilter != null;
            if (merchantFilter != null)
            {
                var spent = list.Where(t => t.Amount < 0).Sum(t => -t.Amount);
                var received = list.Where(t => t.Amount > 0).Sum(t => t.Amount);
                FilterLabel.Text = $"Payments for {TransactionAnalyzer.DisplayName(merchantFilter)}: "
                    + (spent > 0 ? $"{spent.ToString("C", culture)} out" : "")
                    + (spent > 0 && received > 0 ? ", " : "")
                    + (received > 0 ? $"{received.ToString("C", culture)} in" : "");
            }

            TransactionsList.ItemsSource = list.OrderByDescending(t => t.Date)
                .Select(t =>
                {
                    var a = analysis?.For(t);
                    var r = a?.Recurring;
                    return new TransactionDisplayItem
                    {
                        Source = t,
                        Description = t.Description,
                        Date = t.Date,
                        Category = a?.Category ?? t.Category,
                        AccountName = t.AccountName,
                        Type = t.Type,
                        AmountFormatted = t.Amount.ToString("C", culture),
                        AmountColour = t.Amount >= 0 ? Color.FromArgb("#22C55E") : Color.FromArgb("#EF4444"),
                        HasRecurring = r != null && r.Kind != "Transfer",
                        RecurringText = r == null ? "" : $"↻ {r.Frequency} {r.Kind.ToLowerInvariant()}",
                        RecurringColour = Color.FromArgb(KindColour(r?.Kind)),
                    };
                }).ToList();

            if (AnalysisPanel.IsVisible) BuildAnalysisPanel();
        }

        #region Analysis

        private TransactionAnalysis analysis;
        private List<MockAccount> knownAccounts = new();
        private string categoryFilter;
        private string merchantFilter;
        private bool updatingPicker;

        private string CategoryOf(MockTransaction t) => analysis?.For(t)?.Category ?? t.Category ?? "General";

        private void RunAnalysis(List<MockAccount> accounts)
        {
            if (accounts != null) knownAccounts = accounts;
            analysis = TransactionAnalyzer.Analyse(allTransactions, knownAccounts);
            RefreshCategoryPicker();
        }

        private void RefreshCategoryPicker()
        {
            updatingPicker = true;
            try
            {
                var cats = (allTransactions ?? new List<MockTransaction>())
                    .GroupBy(CategoryOf)
                    .OrderByDescending(g => g.Sum(t => Math.Abs(t.Amount)))
                    .Select(g => g.Key).ToList();
                if (categoryFilter != null && !cats.Contains(categoryFilter)) categoryFilter = null;

                CategoryPicker.Items.Clear();
                CategoryPicker.Items.Add("All categories");
                foreach (var c in cats) CategoryPicker.Items.Add(c);
                CategoryPicker.SelectedIndex = categoryFilter == null ? 0 : CategoryPicker.Items.IndexOf(categoryFilter);
            }
            finally { updatingPicker = false; }
        }

        private void OnCategoryFilterChanged(object sender, EventArgs e)
        {
            if (updatingPicker) return;
            var picked = CategoryPicker.SelectedItem as string;
            categoryFilter = picked == null || picked == "All categories" ? null : picked;
            ShowTransactions(selectedAccountName);
        }

        private void OnClearFilterClicked(object sender, EventArgs e)
        {
            merchantFilter = null;
            categoryFilter = null;
            RefreshCategoryPicker();
            ShowTransactions(selectedAccountName);
        }

        private void OnAnalyseClicked(object sender, EventArgs e)
        {
            AnalysisPanel.IsVisible = !AnalysisPanel.IsVisible;
            AnalyseBtn.Text = AnalysisPanel.IsVisible ? "Hide analysis" : "Analyse";
            if (AnalysisPanel.IsVisible) BuildAnalysisPanel();
        }

        private void ShowMerchant(string key)
        {
            merchantFilter = key;
            categoryFilter = null;
            RefreshCategoryPicker();
            ShowTransactions(selectedAccountName);
        }

        private void ShowCategory(string category)
        {
            merchantFilter = null;
            categoryFilter = category;
            RefreshCategoryPicker();
            ShowTransactions(selectedAccountName);
        }

        private static string KindColour(string kind) => kind switch
        {
            TransactionAnalyzer.KindIncome => "#22C55E",
            TransactionAnalyzer.KindBill => "#F59E0B",
            TransactionAnalyzer.KindVariableBill => "#06B6D4",
            TransactionAnalyzer.KindSubscription => "#A78BFA",
            TransactionAnalyzer.KindStandingOrder => "#60A5FA",
            _ => "#94A3B8",
        };

        private static Label Text(string text, double size, string colour, bool bold = false) => new Label
        {
            Text = text, FontSize = size, TextColor = Color.FromArgb(colour),
            FontAttributes = bold ? FontAttributes.Bold : FontAttributes.None,
        };

        private void BuildAnalysisPanel()
        {
            AnalysisContent.Children.Clear();
            if (analysis == null || allTransactions == null || allTransactions.Count == 0)
            {
                AnalysisContent.Add(Text("No transactions to analyse yet. Connect a bank account or press ⟳.", 13, "#94A3B8"));
                return;
            }

            var scope = selectedAccountName == null ? allTransactions : allTransactions.Where(t => t.AccountName == selectedAccountName).ToList();
            var recurring = analysis.Recurring
                .Where(r => r.Kind != "Transfer" && (selectedAccountName == null || r.AccountName == selectedAccountName))
                .ToList();

            var from = scope.Count > 0 ? scope.Min(t => t.Date) : DateTime.Today;
            var to = scope.Count > 0 ? scope.Max(t => t.Date) : DateTime.Today;
            var months = Math.Max(1.0, (to - from).TotalDays / 30.44);

            AnalysisContent.Add(Text(
                $"Based on {scope.Count} transactions from {from:dd MMM yyyy} to {to:dd MMM yyyy}"
                + (selectedAccountName != null ? $" in {selectedAccountName}" : " across all accounts")
                + ". Same amount on a schedule = subscription; regular but changing amount = utility bill. Tap a transaction below to correct its category.",
                12, "#94A3B8"));

            // Summary tiles: monthly totals per kind of regular payment.
            var tiles = new FlexLayout { Wrap = Microsoft.Maui.Layouts.FlexWrap.Wrap, JustifyContent = Microsoft.Maui.Layouts.FlexJustify.Start };
            void Tile(string title, string kind, IEnumerable<RecurringPayment> items)
            {
                var list = items.ToList();
                if (list.Count == 0) return;
                var tile = new Border
                {
                    BackgroundColor = Color.FromArgb("#131B2E"), Stroke = Color.FromArgb(KindColour(kind)), StrokeThickness = 1,
                    StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 10 },
                    Padding = new Thickness(14, 10), Margin = new Thickness(0, 0, 10, 10), MinimumWidthRequest = 170,
                    Content = new VerticalStackLayout
                    {
                        Spacing = 2,
                        Children =
                        {
                            Text(title, 11, KindColour(kind), true),
                            Text($"{list.Sum(r => r.MonthlyCost).ToString("C", culture)} / month", 18, "#F1F5F9", true),
                            Text($"{list.Count} regular payment{(list.Count == 1 ? "" : "s")}", 11, "#64748B"),
                        }
                    }
                };
                tiles.Add(tile);
            }
            Tile("Income", TransactionAnalyzer.KindIncome, recurring.Where(r => r.Kind == TransactionAnalyzer.KindIncome));
            Tile("Bills", TransactionAnalyzer.KindBill, recurring.Where(r => r.Kind is TransactionAnalyzer.KindBill or TransactionAnalyzer.KindVariableBill));
            Tile("Subscriptions", TransactionAnalyzer.KindSubscription, recurring.Where(r => r.Kind == TransactionAnalyzer.KindSubscription));
            Tile("Standing orders", TransactionAnalyzer.KindStandingOrder, recurring.Where(r => r.Kind == TransactionAnalyzer.KindStandingOrder));
            Tile("Other regular spending", TransactionAnalyzer.KindRegularSpend, recurring.Where(r => r.Kind == TransactionAnalyzer.KindRegularSpend));
            if (tiles.Children.Count > 0) AnalysisContent.Add(tiles);

            // Regular payments list.
            AnalysisContent.Add(Text(recurring.Count == 0 ? "No regular payments found yet (needs at least two or three payments to the same payee)." : "Regular payments", 14, "#F1F5F9", true));
            foreach (var r in recurring)
            {
                var amount = r.FixedAmount || r.MaxAmount - r.MinAmount < 0.01m
                    ? r.TypicalAmount.ToString("C", culture)
                    : $"{r.MinAmount.ToString("C", culture)} – {r.MaxAmount.ToString("C", culture)}";

                var chip = new Border
                {
                    BackgroundColor = Color.FromArgb("#0B1120"), Stroke = Color.FromArgb(KindColour(r.Kind)), StrokeThickness = 1,
                    StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 6 },
                    Padding = new Thickness(8, 2), VerticalOptions = LayoutOptions.Center, WidthRequest = 110,
                    Content = new Label { Text = r.Kind, FontSize = 11, TextColor = Color.FromArgb(KindColour(r.Kind)), HorizontalTextAlignment = TextAlignment.Center },
                };
                var info = new VerticalStackLayout { Spacing = 2 };
                info.Add(Text($"{r.Name}  ·  {r.Category}", 13, "#F1F5F9", true));
                info.Add(Text($"{r.Reason}  ·  {r.Count} payments  ·  {r.AccountName}", 11, "#64748B"));

                var right = new VerticalStackLayout { Spacing = 2, HorizontalOptions = LayoutOptions.End };
                right.Add(new Label { Text = $"{amount} {r.Frequency.ToLowerInvariant()}", FontSize = 13, FontAttributes = FontAttributes.Bold, TextColor = Color.FromArgb(r.IsIncome ? "#22C55E" : "#F1F5F9"), HorizontalTextAlignment = TextAlignment.End });
                right.Add(new Label
                {
                    Text = $"last {r.LastDate:dd MMM}" + (r.NextDue >= DateTime.Today.AddDays(-3) ? $"  ·  next ~{r.NextDue:dd MMM}" : "  ·  overdue / stopped?"),
                    FontSize = 11, TextColor = Color.FromArgb(r.NextDue >= DateTime.Today.AddDays(-3) ? "#64748B" : "#F87171"), HorizontalTextAlignment = TextAlignment.End,
                });

                var grid = new Grid
                {
                    ColumnDefinitions = { new ColumnDefinition(GridLength.Auto), new ColumnDefinition(GridLength.Star), new ColumnDefinition(GridLength.Auto) },
                    ColumnSpacing = 12,
                };
                grid.Add(chip, 0); grid.Add(info, 1); grid.Add(right, 2);

                var row = new Border
                {
                    BackgroundColor = Color.FromArgb("#131B2E"), Stroke = Colors.Transparent,
                    StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 8 },
                    Padding = new Thickness(12, 8), Content = grid,
                };
                var key = r.Key;
                row.GestureRecognizers.Add(new TapGestureRecognizer { Command = new Command(() => ShowMerchant(key)) });
                ToolTipProperties.SetText(row, "Show every payment for this payee.");
                AnalysisContent.Add(row);
            }

            // Spending by category (monthly average, money out only, transfers excluded).
            var spend = scope.Where(t => t.Amount < 0)
                .GroupBy(CategoryOf)
                .Where(g => g.Key != "Transfer")
                .Select(g => new { Category = g.Key, Monthly = g.Sum(t => -t.Amount) / (decimal)months })
                .OrderByDescending(x => x.Monthly).ToList();
            if (spend.Count > 0)
            {
                AnalysisContent.Add(Text("Where your money goes (average per month)", 14, "#F1F5F9", true));
                var top = spend[0].Monthly;
                foreach (var s in spend)
                {
                    var bar = new Grid { ColumnDefinitions = { new ColumnDefinition(new GridLength(150)), new ColumnDefinition(GridLength.Star), new ColumnDefinition(new GridLength(90)) }, ColumnSpacing = 10 };
                    bar.Add(Text(s.Category, 12, "#CBD5E1"), 0);
                    var track = new Grid { HeightRequest = 8, VerticalOptions = LayoutOptions.Center };
                    track.Add(new BoxView { Color = Color.FromArgb("#1E2D4A"), CornerRadius = 4 });
                    track.Add(new BoxView
                    {
                        Color = Color.FromArgb("#7C3AED"), CornerRadius = 4, HorizontalOptions = LayoutOptions.Start,
                        WidthRequest = top > 0 ? Math.Max(4, (double)(s.Monthly / top) * 320) : 4,
                    });
                    bar.Add(track, 1);
                    bar.Add(new Label { Text = s.Monthly.ToString("C", culture), FontSize = 12, TextColor = Color.FromArgb("#F1F5F9"), HorizontalTextAlignment = TextAlignment.End }, 2);
                    var cat = s.Category;
                    bar.GestureRecognizers.Add(new TapGestureRecognizer { Command = new Command(() => ShowCategory(cat)) });
                    ToolTipProperties.SetText(bar, $"Show only {cat} transactions.");
                    AnalysisContent.Add(bar);
                }
            }
        }

        private async void OnTransactionSelected(object sender, SelectionChangedEventArgs e)
        {
            if (e.CurrentSelection.FirstOrDefault() is not TransactionDisplayItem item) return;
            TransactionsList.SelectedItem = null;
            var a = analysis?.For(item.Source);
            if (a == null) return;
            var page = Application.Current?.Windows.FirstOrDefault()?.Page;
            if (page == null) return;

            const string change = "Change category";
            var showAll = $"Show all payments for {a.MerchantName}";
            const string why = "Why this category?";
            var pick = await page.DisplayActionSheet($"{a.MerchantName} · {a.Category}", "Cancel", null, change, showAll, why);

            if (pick == showAll) { ShowMerchant(a.MerchantKey); return; }
            if (pick == why)
            {
                var r = a.Recurring;
                await page.DisplayAlert(a.Category,
                    $"{a.Reason}." + (r != null ? $"\n\n{r.Count} payments to {r.Name}, {r.Frequency.ToLowerInvariant()}, "
                        + (r.FixedAmount ? $"always {r.TypicalAmount.ToString("C", culture)}." : $"between {r.MinAmount.ToString("C", culture)} and {r.MaxAmount.ToString("C", culture)}.") : "")
                    + (TransactionAnalyzer.GetRule(a.MerchantKey) != null ? "\n\nYou set this category yourself." : "\n\nIf it's wrong, choose Change category and SmartCube will remember it for this payee."),
                    "OK");
                return;
            }
            if (pick != change) return;

            const string auto = "Automatic (let SmartCube decide)";
            var options = TransactionAnalyzer.Categories.Append(auto).ToArray();
            var chosen = await page.DisplayActionSheet($"Category for all payments to {a.MerchantName}", "Cancel", null, options);
            if (chosen == null || chosen == "Cancel") return;

            TransactionAnalyzer.SetRule(a.MerchantKey, chosen == auto ? null : chosen);
            RunAnalysis(null);
            ShowTransactions(selectedAccountName);
        }

        #endregion

        private async void OnAddAccount()
        {
            var page = new ConnectBankPage();
            page.BankConnected += () =>
            {
                MainThread.BeginInvokeOnMainThread(() => RefreshData());
            };
            await Navigation.PushAsync(page);
        }

        private async void OnRefreshClicked(object sender, EventArgs e)
        {
            RefreshBtn.IsEnabled = false;
            RefreshBtn.Text = "...";
            SmartDataService.ResetInitialized();
            await LoadLiveData();
            RefreshBtn.Text = "⟳";
            RefreshBtn.IsEnabled = true;
        }

        private async void OnHelpClicked(object sender, EventArgs e)
        {
            var page = Application.Current?.Windows.FirstOrDefault()?.Page;
            if (page != null)
                await page.DisplayAlert("Banking", "This section shows your connected bank accounts and recent transactions. Tap an account card to filter the list below to just that account. Use Add Account to connect a new bank through TrueLayer — SmartCube never sees your bank password. All your account and transaction data is stored locally on this PC.\n\n" +
                    "SmartCube sorts every transaction into a category from who you paid and how often. A payment that repeats on a schedule for the same amount is treated as a subscription; one that repeats but changes a little each time is treated as a utility bill; known payees (energy suppliers, councils, supermarkets and so on) get their own category. Press Analyse to see your regular payments and monthly spending by category, use the category box to filter, and tap a transaction to correct its category — SmartCube remembers your choice for that payee.", "OK");
        }

        private void RefreshData()
        {
            AccountCards.Children.Clear();
            selectedCard = null;
            selectedAccountName = null;
            _ = LoadDataAsync();
        }

        private static BoxView CreateAccentLine(Color color)
        {
            var line = new BoxView { Color = color, CornerRadius = 2 };
            Grid.SetColumn(line, 0);
            return line;
        }

        private static VerticalStackLayout CreateAccountContent(MockAccount account, CultureInfo culture)
        {
            var stack = new VerticalStackLayout
            {
                Spacing = 6,
                Children =
                {
                    new Label { Text = account.Institution, TextColor = Color.FromArgb("#94A3B8"), FontSize = 12 },
                    new Label { Text = account.AccountName, TextColor = Color.FromArgb("#F1F5F9"), FontSize = 15, FontAttributes = FontAttributes.Bold },
                    new Label { Text = account.TotalBalance.ToString("C", culture), TextColor = Color.FromArgb("#22C55E"), FontSize = 20, FontAttributes = FontAttributes.Bold },
                    new HorizontalStackLayout
                    {
                        Spacing = 8,
                        Children =
                        {
                            new Label { Text = account.AccountType, TextColor = Color.FromArgb("#06B6D4"), FontSize = 11 },
                            new Label { Text = account.AccountNumber, TextColor = Color.FromArgb("#64748B"), FontSize = 11 },
                        }
                    }
                }
            };
            Grid.SetColumn(stack, 1);
            return stack;
        }

        public async Task PlayEntryAnimation()
        {
            await AnimationHelper.StaggerIn(AccountCards, 80, 450);
            _ = AnimationHelper.FadeIn(TransactionsCard, 100, 400);
        }
    }

    public class TransactionDisplayItem
    {
        public MockTransaction Source { get; set; }
        public bool HasRecurring { get; set; }
        public string RecurringText { get; set; }
        public Color RecurringColour { get; set; }
        public string Description { get; set; }
        public DateTime Date { get; set; }
        public string Category { get; set; }
        public string AccountName { get; set; }
        public string Type { get; set; }
        public string AmountFormatted { get; set; }
        public Color AmountColour { get; set; }
    }
}
