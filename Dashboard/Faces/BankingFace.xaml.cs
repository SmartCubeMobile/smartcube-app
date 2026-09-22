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
            addCard.GestureRecognizers.Add(new TapGestureRecognizer
            {
                Command = new Command(OnAddAccount)
            });
            AccountCards.Children.Add(addCard);

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
            addCard.GestureRecognizers.Add(new TapGestureRecognizer
            {
                Command = new Command(OnAddAccount)
            });
            AccountCards.Children.Add(addCard);

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
            var filtered = accountName == null
                ? allTransactions
                : allTransactions.Where(t => t.AccountName == accountName).ToList();

            var label = accountName == null
                ? $"{filtered.Count} transactions"
                : $"{filtered.Count} transactions — {accountName}";

            TransactionCount.Text = label;
            TransactionsList.ItemsSource = filtered.OrderByDescending(t => t.Date)
                .Select(t => new TransactionDisplayItem
                {
                    Description = t.Description,
                    Date = t.Date,
                    Category = t.Category,
                    AccountName = t.AccountName,
                    Type = t.Type,
                    AmountFormatted = t.Amount.ToString("C", culture),
                    AmountColour = t.Amount >= 0 ? Color.FromArgb("#22C55E") : Color.FromArgb("#EF4444"),
                }).ToList();
        }

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
        public string Description { get; set; }
        public DateTime Date { get; set; }
        public string Category { get; set; }
        public string AccountName { get; set; }
        public string Type { get; set; }
        public string AmountFormatted { get; set; }
        public Color AmountColour { get; set; }
    }
}
