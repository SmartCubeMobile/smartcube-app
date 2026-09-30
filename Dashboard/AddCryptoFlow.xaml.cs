using SmartCubeMobile.MockData;
using SmartCubeMobile.Services;
using System.Globalization;

namespace SmartCubeMobile.Dashboard
{
    public partial class AddCryptoFlow : ContentPage
    {
        public event Action CryptoAdded;

        private readonly CultureInfo culture = new("en-GB");
        private bool isWallet;
        private string selectedNetwork;
        private string selectedExchange;
        private Border selectedPickerCard;
        private string _fullWalletAddress;

        private static readonly List<NetworkInfo> WalletNetworks = new()
        {
            new("Bitcoin", "BTC", "₿", "#F7931A"),
            new("Ethereum", "ETH", "Ξ", "#627EEA"),
            new("XRP Ledger", "XRP", "✕", "#00AAE4"),
            new("Solana", "SOL", "◎", "#9945FF"),
            new("Hedera", "HBAR", "ℏ", "#00ACED"),
            new("Cardano", "ADA", "₳", "#0033AD"),
            new("Polygon", "MATIC", "⬡", "#8247E5"),
            new("Stellar", "XLM", "✦", "#000000"),
        };

        private static readonly List<ExchangeInfo> Exchanges = new()
        {
            new("Coinbase", "🟦", "#0052FF", "API Key Name", "Private Key"),
            new("Binance", "🟨", "#F0B90B", "API Key", "Secret Key"),
            new("Kraken", "🟪", "#5741D9", "API Key", "Private Key"),
            new("Crypto.com", "🔵", "#002D74", "API Key", "Secret Key"),
            new("Gemini", "🟩", "#00DCFA", "API Key", "API Secret"),
            new("Bitstamp", "🟢", "#4BA53E", "Customer ID", "API Key"),
        };

        private readonly string _prefillLabel;

        public AddCryptoFlow(string prefillLabel = null)
        {
            _prefillLabel = prefillLabel;
            InitializeComponent();
            SetupTypeCards();

            if (!string.IsNullOrEmpty(_prefillLabel))
                SelectType(true);
        }

        private void SetupTypeCards()
        {
            WalletOption.GestureRecognizers.Add(new TapGestureRecognizer
            {
                Command = new Command(async () =>
                {
                    await WalletOption.ScaleTo(0.97, 80, Easing.CubicOut);
                    await WalletOption.ScaleTo(1.0, 80, Easing.CubicOut);
                    SelectType(true);
                })
            });
            ExchangeOption.GestureRecognizers.Add(new TapGestureRecognizer
            {
                Command = new Command(async () =>
                {
                    await ExchangeOption.ScaleTo(0.97, 80, Easing.CubicOut);
                    await ExchangeOption.ScaleTo(1.0, 80, Easing.CubicOut);
                    SelectType(false);
                })
            });
        }

        private async void SelectType(bool wallet)
        {
            isWallet = wallet;
            PageTitle.Text = wallet ? "Add Wallet" : "Add Exchange";
            PageSubtitle.Text = wallet ? "Select blockchain network" : "Select exchange";
            StepIndicator.Text = "Step 2 of 3";

            FormIcon.Text = wallet ? "🔑" : "🏛";
            FormTitle.Text = wallet ? "Wallet Address" : "Exchange API";
            PickerLabel.Text = wallet ? "Network" : "Exchange";

            PickerList.Children.Clear();
            FieldsSection.IsVisible = false;
            selectedPickerCard = null;

            if (wallet)
            {
                foreach (var net in WalletNetworks)
                    AddPickerCard(net.Name, net.Symbol, net.Icon, net.Colour, net.Symbol);
            }
            else
            {
                foreach (var ex in Exchanges)
                    AddPickerCard(ex.Name, "", ex.Icon, ex.Colour, ex.Name);
            }

            Step1.IsVisible = false;
            Step2.IsVisible = true;
            Step2.Opacity = 0;
            await Step2.FadeTo(1, 250, Easing.CubicOut);
        }

        private void AddPickerCard(string name, string subtitle, string icon, string colour, string key)
        {
            var card = new Border
            {
                BackgroundColor = Color.FromArgb("#1C2744"),
                Stroke = Color.FromArgb("#1E2D4A"),
                StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 10 },
                Padding = new Thickness(12, 10),
            };

            var grid = new Grid
            {
                ColumnDefinitions = new ColumnDefinitionCollection
                {
                    new ColumnDefinition(new GridLength(36)),
                    new ColumnDefinition(GridLength.Star),
                },
                ColumnSpacing = 12,
            };

            var iconBorder = new Border
            {
                BackgroundColor = Color.FromArgb(colour + "30"),
                StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 8 },
                Stroke = Colors.Transparent,
                WidthRequest = 36, HeightRequest = 36,
                Content = new Label
                {
                    Text = icon, FontSize = 18,
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center,
                }
            };
            Grid.SetColumn(iconBorder, 0);

            var info = new VerticalStackLayout { Spacing = 1, VerticalOptions = LayoutOptions.Center };
            info.Children.Add(new Label
            {
                Text = name, TextColor = Color.FromArgb("#F1F5F9"),
                FontSize = 14, FontAttributes = FontAttributes.Bold,
            });
            if (!string.IsNullOrEmpty(subtitle))
            {
                info.Children.Add(new Label
                {
                    Text = subtitle, TextColor = Color.FromArgb("#64748B"), FontSize = 11,
                });
            }
            Grid.SetColumn(info, 1);

            grid.Children.Add(iconBorder);
            grid.Children.Add(info);
            card.Content = grid;

            ToolTipProperties.SetText(card, $"Select {name} to continue with this network or exchange.");

            var k = key;
            var c = card;
            card.GestureRecognizers.Add(new TapGestureRecognizer
            {
                Command = new Command(() => SelectPickerItem(k, c))
            });

            PickerList.Children.Add(card);
        }

        private void SelectPickerItem(string key, Border card)
        {
            if (selectedPickerCard != null)
            {
                selectedPickerCard.Stroke = Color.FromArgb("#1E2D4A");
                selectedPickerCard.StrokeThickness = 1;
            }

            selectedPickerCard = card;
            card.Stroke = Color.FromArgb("#3B82F6");
            card.StrokeThickness = 2;

            if (isWallet)
            {
                selectedNetwork = key;
                Field1Label.Text = $"{key} Wallet Address";
                Field1Entry.Placeholder = $"Enter {key} address";
                Field1Entry.Text = "";
                Field2Section.IsVisible = false;
                Field3Section.IsVisible = true;
                Field3Label.Text = "Label (optional)";
                Field3Entry.Text = _prefillLabel ?? "";
            }
            else
            {
                selectedExchange = key;
                var ex = Exchanges.First(e => e.Name == key);
                Field1Label.Text = ex.Field1;
                Field1Entry.Placeholder = $"Enter {ex.Field1.ToLower()}";
                Field1Entry.Text = "";

                Field3Section.IsVisible = true;
                Field3Label.Text = "Account Label";
                var existingConns = CryptoStorageService.LoadConnections()
                    .Count(c => c.Type == "exchange" && c.ExchangeName == key);
                Field3Entry.Text = existingConns > 0 ? $"{key} {existingConns + 1}" : key;

                if (key == "Coinbase")
                {
                    Field2Section.IsVisible = false;
                    PrivateKeySection.IsVisible = true;
                    PrivateKeyEditor.Text = "";
                }
                else
                {
                    Field2Section.IsVisible = true;
                    PrivateKeySection.IsVisible = false;
                    Field2Label.Text = ex.Field2;
                    Field2Entry.Placeholder = $"Enter {ex.Field2.ToLower()}";
                    Field2Entry.Text = "";
                    Field2Entry.IsPassword = true;
                    Field2Entry.Keyboard = Keyboard.Default;
                }
            }

            FieldsSection.IsVisible = true;
        }

        private async void OnConnectClicked(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(Field1Entry.Text))
            {
                await DisplayAlert("Missing", $"Please enter the {Field1Label.Text.ToLower()}.", "OK");
                return;
            }
            if (!isWallet)
            {
                if (selectedExchange == "Coinbase" && string.IsNullOrWhiteSpace(PrivateKeyEditor.Text))
                {
                    await DisplayAlert("Missing", "Please paste your private key.", "OK");
                    return;
                }
                if (selectedExchange != "Coinbase" && string.IsNullOrWhiteSpace(Field2Entry.Text))
                {
                    await DisplayAlert("Missing", $"Please enter the {Field2Label.Text.ToLower()}.", "OK");
                    return;
                }
            }

            PageTitle.Text = "Connecting";
            StepIndicator.Text = "Step 3 of 3";
            BackButton.IsVisible = false;

            ConnectingLabel.Text = isWallet
                ? $"Scanning {selectedNetwork} blockchain..."
                : $"Connecting to {selectedExchange}...";
            ConnectingDetail.Text = isWallet ? "Looking up address..." : "Authenticating...";
            ConnectingState.IsVisible = true;
            SuccessState.IsVisible = false;

            Step2.IsVisible = false;
            Step3.IsVisible = true;
            Step3.Opacity = 0;
            await Step3.FadeTo(1, 250, Easing.CubicOut);

            try
            {
                var holdings = await FetchLiveHoldingsAsync();
                ShowSuccess(holdings);
            }
            catch (Exception ex)
            {
                ConnectingState.IsVisible = false;
                await DisplayAlert("Error", $"Failed to connect: {ex.Message}", "OK");
                BackButton.IsVisible = true;
            }
        }

        private async Task<List<MockCryptoHolding>> FetchLiveHoldingsAsync()
        {
            var address = Field1Entry.Text.Trim();

            if (isWallet)
            {
                var net = WalletNetworks.First(n => n.Symbol == selectedNetwork);
                _fullWalletAddress = address;
                var displayAddr = address;
                if (address.Length > 10)
                    displayAddr = address[..6] + "..." + address[^4..];
                var label = Field3Entry.Text?.Trim();

                if (selectedNetwork == "ETH")
                {
                    ConnectingDetail.Text = "Scanning ETH & token balances...";
                    var tokenBalances = await WalletBalanceService.GetEthTokenBalancesAsync(address);

                    var symbols = tokenBalances.Select(t => t.Symbol).Distinct().ToList();

                    if (symbols.Count > 0)
                    {
                        ConnectingDetail.Text = $"Found {symbols.Count} tokens, fetching prices...";
                        await Task.Delay(3000);
                        var prices = await CryptoMarketService.GetBatchPricesAsync(symbols);

                        var results = new List<MockCryptoHolding>();
                        foreach (var sym in symbols)
                        {
                            var tb = tokenBalances.FirstOrDefault(t => t.Symbol == sym);
                            var balance = tb?.Balance ?? 0;
                            if (balance <= 0) continue;

                            prices.TryGetValue(sym, out var pp);
                            if (pp.Price <= 0) continue;
                            if (balance * pp.Price > 1_000_000m) continue;

                            results.Add(new MockCryptoHolding
                            {
                                Symbol = sym, Name = tb?.Name ?? sym, Quantity = balance,
                                PriceGBP = pp.Price, PriceUSD = pp.Price * 1.27m, Change24h = pp.Change24h,
                                WalletLabel = string.IsNullOrEmpty(label) ? null : label,
                                WalletAddress = displayAddr, Network = "eth",
                            });
                        }
                        if (results.Count > 0) return results;
                    }

                    ConnectingState.IsVisible = false;
                    var qtyStr = await DisplayPromptAsync("Enter Balance",
                        $"No tokens found. Enter your ETH quantity:", keyboard: Keyboard.Numeric);
                    ConnectingState.IsVisible = true;
                    decimal manualBal = 0;
                    if (qtyStr != null) decimal.TryParse(qtyStr, out manualBal);
                    decimal ep = 0, ec = 0;
                    try { (ep, ec) = await CryptoMarketService.GetSinglePriceAsync("ETH"); } catch { }
                    return new List<MockCryptoHolding>
                    {
                        new()
                        {
                            Symbol = "ETH", Name = "Ethereum", Quantity = manualBal,
                            PriceGBP = ep, PriceUSD = ep * 1.27m, Change24h = ec,
                            WalletLabel = string.IsNullOrEmpty(label) ? null : label,
                            WalletAddress = displayAddr, Network = "eth",
                        }
                    };
                }

                ConnectingDetail.Text = "Fetching live price...";
                decimal livePrice = 0;
                decimal change = 0;
                try
                {
                    (livePrice, change) = await CryptoMarketService.GetSinglePriceAsync(selectedNetwork);
                }
                catch { }

                ConnectingDetail.Text = "Reading wallet balance...";
                decimal walletBalance = 0;
                string lookupError = null;
                try
                {
                    var result = await WalletBalanceService.GetBalanceAsync(selectedNetwork, address);
                    if (result.HasValue && result.Value > 0)
                        walletBalance = result.Value;
                    else
                        lookupError = $"API returned: {result?.ToString() ?? "null"}";
                }
                catch (Exception ex)
                {
                    lookupError = ex.Message;
                }

                if (walletBalance == 0)
                {
                    ConnectingState.IsVisible = false;
                    var msg = lookupError != null
                        ? $"Auto-detect failed ({lookupError}).\nEnter your {net.Symbol} quantity:"
                        : $"Enter your {net.Symbol} quantity:";
                    var qtyStr = await DisplayPromptAsync("Enter Balance", msg, keyboard: Keyboard.Numeric);
                    ConnectingState.IsVisible = true;
                    if (qtyStr != null)
                        decimal.TryParse(qtyStr, out walletBalance);
                }

                ConnectingDetail.Text = "Fetching transaction history...";
                var txList = new List<MockCryptoTransaction>();
                try
                {
                    txList = await WalletTransactionService.GetTransactionsAsync(selectedNetwork, address);

                    if (txList.Count > 0)
                    {
                        ConnectingDetail.Text = "Fetching historical prices...";
                        await HistoricalPriceService.FillPricesAsync(txList, livePrice);
                    }
                }
                catch
                {
                    await HistoricalPriceService.FillPricesAsync(txList, livePrice);
                }

                ConnectingDetail.Text = "Calculating values...";

                return new List<MockCryptoHolding>
                {
                    new()
                    {
                        Symbol = net.Symbol, Name = net.Name, Quantity = walletBalance,
                        PriceGBP = livePrice, PriceUSD = livePrice * 1.27m,
                        Change24h = change,
                        WalletLabel = string.IsNullOrEmpty(label) ? null : label,
                        WalletAddress = displayAddr,
                        Network = net.Name.ToLower(),
                        Transactions = txList,
                    }
                };
            }
            else
            {
                var apiKey = Field1Entry.Text.Trim();

                var exchangeLabel = Field3Entry.Text?.Trim();
                if (string.IsNullOrEmpty(exchangeLabel)) exchangeLabel = selectedExchange;

                if (selectedExchange == "Coinbase")
                {
                    var privateKey = PrivateKeyEditor.Text.Trim();
                    ConnectingDetail.Text = "Authenticating with Coinbase...";
                    var holdings = await CoinbaseService.GetHoldingsAsync(apiKey, privateKey, exchangeLabel);
                    ConnectingDetail.Text = $"Found {holdings.Count} assets";
                    return holdings;
                }

                var secret = Field2Entry.Text.Trim();

                ConnectingDetail.Text = "Enter your holdings...";
                ConnectingState.IsVisible = false;

                var manualHoldings = new List<MockCryptoHolding>();
                var defaultSymbols = new[] { "BTC", "ETH", "XRP", "SOL", "ADA", "DOGE", "DOT", "AVAX", "LINK" };

                while (true)
                {
                    var symbol = await DisplayActionSheet("Select asset", "Done", null, defaultSymbols);
                    if (symbol == null || symbol == "Done") break;

                    decimal coinPrice = 0;
                    decimal coinChange = 0;
                    try { (coinPrice, coinChange) = await CryptoMarketService.GetSinglePriceAsync(symbol); }
                    catch { }

                    var qtyStr = await DisplayPromptAsync($"Enter {symbol} quantity",
                        $"Current price: {coinPrice.ToString("C", culture)}",
                        keyboard: Keyboard.Numeric);
                    if (qtyStr != null && decimal.TryParse(qtyStr, out var qty) && qty > 0)
                    {
                        manualHoldings.Add(new MockCryptoHolding
                        {
                            Symbol = symbol,
                            Name = symbol,
                            Quantity = qty,
                            PriceGBP = coinPrice,
                            PriceUSD = coinPrice * 1.27m,
                            Change24h = coinChange,
                            WalletLabel = exchangeLabel,
                            WalletAddress = $"{exchangeLabel} account",
                            Network = selectedExchange.ToLower(),
                        });
                    }
                }

                ConnectingState.IsVisible = true;
                return manualHoldings;
            }
        }

        private void ShowSuccess(List<MockCryptoHolding> holdings)
        {
            ConnectingState.IsVisible = false;
            SuccessState.IsVisible = true;
            SuccessState.Opacity = 0;

            if (holdings.Count == 0)
            {
                SuccessTitle.Text = "No assets added";
            }
            else
            {
                SuccessTitle.Text = $"{holdings.Count} asset{(holdings.Count == 1 ? "" : "s")} found";
            }

            FoundAssetsList.Children.Clear();

            foreach (var h in holdings)
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
                                    new Label { Text = $"{h.Name} ({h.Symbol})", TextColor = Color.FromArgb("#F1F5F9"), FontSize = 14, FontAttributes = FontAttributes.Bold },
                                    new Label { Text = $"{h.Quantity:G} {h.Symbol} @ {h.PriceGBP.ToString("C", culture)}", TextColor = Color.FromArgb("#64748B"), FontSize = 12 },
                                }
                            },
                            CreateValueLabel(h),
                        }
                    }
                };
                FoundAssetsList.Children.Add(card);
            }

            BackButton.IsVisible = true;
            PageTitle.Text = holdings.Count > 0 ? "Success" : "Done";
            PageSubtitle.Text = isWallet ? "Wallet connected" : "Exchange linked";

            if (holdings.Count > 0)
            {
                MockDataService.AddCryptoHoldings(holdings);

                if (isWallet)
                {
                    CryptoStorageService.SaveConnection(new SavedConnection
                    {
                        Type = "wallet",
                        Symbol = selectedNetwork,
                        Address = _fullWalletAddress,
                        Label = Field3Entry.Text?.Trim(),
                    });
                }
                else
                {
                    CryptoStorageService.SaveConnection(new SavedConnection
                    {
                        Type = "exchange",
                        ExchangeName = selectedExchange,
                        Label = Field3Entry.Text?.Trim(),
                        ApiKey = Field1Entry.Text.Trim(),
                        Secret = selectedExchange == "Coinbase"
                            ? PrivateKeyEditor.Text.Trim()
                            : Field2Entry.Text.Trim(),
                    });
                }

                CryptoStorageService.SaveHoldingsCache(MockDataService.GetCryptoHoldings());
            }

            _ = SuccessState.FadeTo(1, 350, Easing.CubicOut);
        }

        private static Label CreateValueLabel(MockCryptoHolding h)
        {
            var culture = new CultureInfo("en-GB");
            var val = h.Quantity * h.PriceGBP;
            var lbl = new Label
            {
                Text = val.ToString("C", culture),
                TextColor = Color.FromArgb("#22C55E"),
                FontSize = 16, FontAttributes = FontAttributes.Bold,
                VerticalOptions = LayoutOptions.Center,
            };
            Grid.SetColumn(lbl, 1);
            return lbl;
        }

        private async void OnAddClicked(object sender, EventArgs e)
        {
            CryptoAdded?.Invoke();
            await Navigation.PopAsync();
        }

        private async void OnBackClicked(object sender, EventArgs e)
        {
            if (Step3.IsVisible && SuccessState.IsVisible)
            {
                CryptoAdded?.Invoke();
                await Navigation.PopAsync();
                return;
            }
            if (Step2.IsVisible)
            {
                Step2.IsVisible = false;
                Step1.IsVisible = true;
                PageTitle.Text = "Add Crypto";
                PageSubtitle.Text = "Choose connection type";
                StepIndicator.Text = "Step 1 of 3";
                return;
            }
            await Navigation.PopAsync();
        }

        private async void OnHelpClicked(object sender, EventArgs e)
        {
            await DisplayAlert("Add Crypto",
                "This screen adds a crypto wallet or exchange account so its balances appear on your dashboard. Choose 'Wallet Address' to look up a public blockchain address, or 'Exchange Account' to link an exchange using an API key. SmartCube fetches live prices and balances, then lets you review them before adding. Your addresses and keys are stored locally on this PC.",
                "OK");
        }

        private record NetworkInfo(string Name, string Symbol, string Icon, string Colour);
        private record ExchangeInfo(string Name, string Icon, string Colour, string Field1, string Field2);
    }
}
