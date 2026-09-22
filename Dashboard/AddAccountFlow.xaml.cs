using SmartCubeMobile.MockData;

namespace SmartCubeMobile.Dashboard
{
    public partial class AddAccountFlow : ContentPage
    {
        public event Action AccountAdded;

        private static readonly List<BankInfo> Banks = new()
        {
            new("Monzo", "💳", "#FF5A5F", "Email", "Password", "PIN"),
            new("Starling Bank", "🐦", "#7433FF", "Email", "Password", "Passcode"),
            new("Revolut", "🔄", "#0075EB", "Email / Phone", "Password", "Passcode"),
            new("Chase UK", "🏛️", "#117ACA", "Email", "Password", "Passcode"),
            new("Nationwide", "🏠", "#1E3A5F", "Customer Number", "Date of Birth (ddMMyyyy)", "Passcode"),
            new("Santander", "🔴", "#EC0000", "Personal ID", "Date of Birth", "Security Number (5 digits)"),
            new("Barclays", "🦅", "#00AEEF", "Membership Number", "Passcode", "Memorable Word"),
            new("HSBC UK", "🔺", "#DB0011", "Username", "Password", "Secure Key Code"),
            new("Lloyds", "🐴", "#006A4D", "Username", "Password", "Memorable Word"),
            new("Halifax", "🏦", "#0059A3", "Username", "Password", "Memorable Word"),
            new("NatWest", "🟣", "#5B2D8E", "Customer Number", "PIN", "Password"),
            new("Bank of Scotland", "🏴", "#003DA5", "Username", "Password", "Memorable Word"),
            new("TSB", "🔵", "#0047BB", "User ID", "Password", "Memorable Information"),
            new("First Direct", "⚫", "#000000", "Username", "Password", "Secure Key Code"),
            new("Virgin Money", "❤️", "#E10A0A", "Username", "Password", "Security Code"),
            new("Metro Bank", "🏪", "#003087", "Username", "Password", "One-Time Code"),
            new("Atom Bank", "⚛️", "#6B28C6", "Email", "Password", "Passcode"),
            new("Kroo", "🟢", "#00D09C", "Email", "Password", "PIN"),
            new("AJ Bell", "📈", "#00A651", "Username", "Password", "Security Code"),
        };

        private BankInfo selectedBank;

        public AddAccountFlow()
        {
            InitializeComponent();
            BuildBankList();
        }

        private void BuildBankList()
        {
            foreach (var bank in Banks)
            {
                var card = new Border
                {
                    BackgroundColor = Color.FromArgb("#131B2E"),
                    Stroke = Color.FromArgb("#1E2D4A"),
                    StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 14 },
                    Padding = new Thickness(16, 14),
                    Content = new Grid
                    {
                        ColumnDefinitions = new ColumnDefinitionCollection
                        {
                            new ColumnDefinition(new GridLength(44)),
                            new ColumnDefinition(GridLength.Star),
                            new ColumnDefinition(new GridLength(28)),
                        },
                        ColumnSpacing = 14,
                        Children =
                        {
                            CreateBankIcon(bank),
                            CreateBankLabel(bank),
                            CreateChevron(),
                        }
                    }
                };

                var b = bank;
                card.GestureRecognizers.Add(new TapGestureRecognizer
                {
                    Command = new Command(async () =>
                    {
                        await card.ScaleTo(0.97, 80, Easing.CubicOut);
                        await card.ScaleTo(1.0, 80, Easing.CubicOut);
                        SelectBank(b);
                    })
                });

                BankList.Children.Add(card);
            }
        }

        private static Border CreateBankIcon(BankInfo bank)
        {
            var icon = new Border
            {
                BackgroundColor = Color.FromArgb(bank.BrandColor),
                StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 10 },
                Stroke = Colors.Transparent,
                WidthRequest = 44,
                HeightRequest = 44,
                Content = new Label
                {
                    Text = bank.Icon,
                    FontSize = 22,
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center,
                }
            };
            Grid.SetColumn(icon, 0);
            return icon;
        }

        private static VerticalStackLayout CreateBankLabel(BankInfo bank)
        {
            var stack = new VerticalStackLayout
            {
                Spacing = 2,
                VerticalOptions = LayoutOptions.Center,
                Children =
                {
                    new Label
                    {
                        Text = bank.Name,
                        TextColor = Color.FromArgb("#F1F5F9"),
                        FontSize = 15,
                        FontAttributes = FontAttributes.Bold,
                    },
                    new Label
                    {
                        Text = $"{bank.Param1Label} · {bank.Param2Label} · {bank.Param3Label}",
                        TextColor = Color.FromArgb("#64748B"),
                        FontSize = 11,
                    }
                }
            };
            Grid.SetColumn(stack, 1);
            return stack;
        }

        private static Label CreateChevron()
        {
            var label = new Label
            {
                Text = "›",
                TextColor = Color.FromArgb("#475569"),
                FontSize = 22,
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Center,
            };
            Grid.SetColumn(label, 2);
            return label;
        }

        private async void SelectBank(BankInfo bank)
        {
            selectedBank = bank;

            SelectedBankIcon.Text = bank.Icon;
            SelectedBankName.Text = bank.Name;
            Param1Label.Text = bank.Param1Label;
            Param2Label.Text = bank.Param2Label;
            Param3Label.Text = bank.Param3Label;
            Param3Entry.IsPassword = bank.Param3Label.Contains("Pass") || bank.Param3Label.Contains("Security") || bank.Param3Label.Contains("PIN");

            Param1Entry.Text = "";
            Param2Entry.Text = "";
            Param3Entry.Text = "";

            StepTitle.Text = bank.Name;
            StepSubtitle.Text = "Enter your credentials";
            StepIndicator.Text = "Step 2 of 3";

            Step1.IsVisible = false;
            Step2.IsVisible = true;
            Step2.Opacity = 0;
            await Step2.FadeTo(1, 250, Easing.CubicOut);
        }

        private async void OnConnectClicked(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(Param1Entry.Text) ||
                string.IsNullOrWhiteSpace(Param2Entry.Text) ||
                string.IsNullOrWhiteSpace(Param3Entry.Text))
            {
                await DisplayAlert("Missing Fields", "Please fill in all credential fields.", "OK");
                return;
            }

            StepTitle.Text = "Connecting";
            StepSubtitle.Text = selectedBank.Name;
            StepIndicator.Text = "Step 3 of 3";
            BackButton.IsVisible = false;

            ConnectingLabel.Text = $"Connecting to {selectedBank.Name}...";
            ConnectingDetail.Text = "Logging in securely...";
            ConnectingState.IsVisible = true;
            SuccessState.IsVisible = false;

            Step2.IsVisible = false;
            Step3.IsVisible = true;
            Step3.Opacity = 0;
            await Step3.FadeTo(1, 250, Easing.CubicOut);

            await Task.Delay(1200);
            ConnectingDetail.Text = "Retrieving account details...";
            await Task.Delay(1000);
            ConnectingDetail.Text = "Fetching transactions...";
            await Task.Delay(800);

            ShowSuccess();
        }

        private void ShowSuccess()
        {
            var mockAccounts = GenerateMockAccounts(selectedBank);

            ConnectingState.IsVisible = false;
            SuccessState.IsVisible = true;
            SuccessState.Opacity = 0;

            SuccessTitle.Text = $"{mockAccounts.Count} account{(mockAccounts.Count == 1 ? "" : "s")} found";
            FoundAccountsList.Children.Clear();

            foreach (var acct in mockAccounts)
            {
                var card = new Border
                {
                    BackgroundColor = Color.FromArgb("#131B2E"),
                    Stroke = Color.FromArgb("#1E2D4A"),
                    StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 12 },
                    Padding = new Thickness(16, 12),
                    Content = new Grid
                    {
                        ColumnDefinitions = new ColumnDefinitionCollection
                        {
                            new ColumnDefinition(GridLength.Star),
                            new ColumnDefinition(GridLength.Auto),
                        },
                        Children =
                        {
                            new VerticalStackLayout
                            {
                                Spacing = 2,
                                Children =
                                {
                                    new Label { Text = acct.AccountName, TextColor = Color.FromArgb("#F1F5F9"), FontSize = 14, FontAttributes = FontAttributes.Bold },
                                    new Label { Text = $"{acct.AccountType} · {acct.AccountNumber}", TextColor = Color.FromArgb("#64748B"), FontSize = 12 },
                                }
                            },
                            CreateBalanceLabel(acct),
                        }
                    }
                };
                FoundAccountsList.Children.Add(card);
            }

            BackButton.IsVisible = true;
            StepTitle.Text = "Success";
            StepSubtitle.Text = $"Connected to {selectedBank.Name}";

            _ = SuccessState.FadeTo(1, 350, Easing.CubicOut);

            MockDataService.AddAccounts(mockAccounts);
            var now = DateTime.Now;
            foreach (var acct in mockAccounts)
            {
                MockDataService.AddTransactions(GenerateMockTransactions(acct, now));
            }
        }

        private static Label CreateBalanceLabel(MockAccount acct)
        {
            var label = new Label
            {
                Text = acct.TotalBalance.ToString("C", new System.Globalization.CultureInfo("en-GB")),
                TextColor = Color.FromArgb("#22C55E"),
                FontSize = 16,
                FontAttributes = FontAttributes.Bold,
                VerticalOptions = LayoutOptions.Center,
            };
            Grid.SetColumn(label, 1);
            return label;
        }

        private async void OnAddToCubeClicked(object sender, EventArgs e)
        {
            AccountAdded?.Invoke();
            await Navigation.PopAsync();
        }

        private async void OnBackClicked(object sender, EventArgs e)
        {
            if (Step3.IsVisible && SuccessState.IsVisible)
            {
                AccountAdded?.Invoke();
                await Navigation.PopAsync();
                return;
            }

            if (Step2.IsVisible)
            {
                Step2.IsVisible = false;
                Step1.IsVisible = true;
                StepTitle.Text = "Add Bank Account";
                StepSubtitle.Text = "Select your bank";
                StepIndicator.Text = "Step 1 of 3";
                return;
            }

            await Navigation.PopAsync();
        }

        private static List<MockAccount> GenerateMockAccounts(BankInfo bank)
        {
            var accounts = new List<MockAccount>();
            var rng = new Random();

            accounts.Add(new MockAccount
            {
                Institution = bank.Name,
                AccountName = bank.Name == "AJ Bell" ? "Investment ISA" : "Current Account",
                SortCode = bank.Name == "AJ Bell" ? "" : $"{rng.Next(10, 99):D2}-{rng.Next(10, 99):D2}-{rng.Next(10, 99):D2}",
                AccountNumber = bank.Name == "AJ Bell" ? $"AJB-{rng.Next(10000, 99999)}" : $"****{rng.Next(1000, 9999)}",
                TotalBalance = Math.Round((decimal)(rng.NextDouble() * 4000 + 500), 2),
                AvailableBalance = 0,
                AccountType = bank.Name == "AJ Bell" ? "Investment" : "Current",
            });
            accounts[0].AvailableBalance = accounts[0].TotalBalance;

            if (bank.Name != "AJ Bell" && rng.Next(100) > 30)
            {
                var savings = new MockAccount
                {
                    Institution = bank.Name,
                    AccountName = "Savings Account",
                    SortCode = accounts[0].SortCode,
                    AccountNumber = $"****{rng.Next(1000, 9999)}",
                    TotalBalance = Math.Round((decimal)(rng.NextDouble() * 15000 + 1000), 2),
                    AccountType = "Savings",
                };
                savings.AvailableBalance = savings.TotalBalance;
                accounts.Add(savings);
            }

            return accounts;
        }

        private static List<MockTransaction> GenerateMockTransactions(MockAccount account, DateTime now)
        {
            var txns = new List<MockTransaction>();
            var rng = new Random();
            string[] merchants = { "Tesco", "ASDA", "Sainsburys", "Amazon", "PayPal", "TfL", "Uber", "Costa", "Argos" };
            string[] categories = { "Groceries", "Shopping", "Transport", "Eating Out", "Bills" };

            int count = rng.Next(3, 7);
            decimal balance = account.TotalBalance;

            for (int i = 0; i < count; i++)
            {
                var amount = -Math.Round((decimal)(rng.NextDouble() * 80 + 5), 2);
                txns.Add(new MockTransaction
                {
                    Date = now.AddDays(-(i + 1)),
                    Description = merchants[rng.Next(merchants.Length)],
                    Category = categories[rng.Next(categories.Length)],
                    Amount = amount,
                    Balance = balance,
                    AccountName = account.AccountName,
                    Type = "Card",
                });
                balance -= amount;
            }

            return txns;
        }

        private record BankInfo(string Name, string Icon, string BrandColor,
            string Param1Label, string Param2Label, string Param3Label);
    }
}
