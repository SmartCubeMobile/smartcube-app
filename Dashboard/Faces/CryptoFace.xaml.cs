using SmartCubeMobile.Dashboard;
using SmartCubeMobile.MockData;
using SmartCubeMobile.Services;
using System.Globalization;

namespace SmartCubeMobile.Dashboard.Faces
{
    public partial class CryptoFace : ContentView, IAnimatedFace
    {
        private readonly CultureInfo _culture = new("en-GB");
        private readonly HashSet<string> _txFilters = new();
        private readonly Dictionary<string, Border> _txFilterButtons = new();
        private List<MockCryptoHolding> _lastHoldings;

        public CryptoFace()
        {
            InitializeComponent();
            LoadData();
            CryptoPriceMonitor.Start();
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

        private void LoadData()
        {
            var holdings = MockDataService.GetCryptoHoldings();
            if (holdings.Count == 0)
            {
                var cached = CryptoStorageService.LoadHoldingsCache();
                if (cached.Count > 0)
                {
                    MockDataService.AddCryptoHoldings(cached);
                    holdings = cached;
                }
            }
            _extraHoldings.Clear();
            _extraHoldings.AddRange(holdings);
            DisplayHoldings(holdings);
        }

        private async Task RefreshFromSavedConnectionsAsync()
        {
            var connections = CryptoStorageService.LoadConnections();
            if (connections.Count == 0)
            {
                _ = UpdateLivePricesAsync();
                return;
            }

            var dashboard = FindDashboard();
            if (dashboard != null)
                await dashboard.ShowLoading("Loading crypto wallets...", "Connecting to exchanges");

            var hasCache = _extraHoldings.Count > 0 || MockDataService.GetCryptoHoldings().Count > 0;
            if (hasCache)
                MainThread.BeginInvokeOnMainThread(() => SetAllCardsLoading(true));
            else
                MainThread.BeginInvokeOnMainThread(() => ShowLoadingPlaceholders(connections));

            var allHoldings = new List<MockCryptoHolding>();

            var walletConns = connections.Where(c => c.Type == "wallet" && c.Symbol != "ETH").ToList();
            var ethConns = connections.Where(c => c.Type == "wallet" && c.Symbol == "ETH").ToList();
            var exchangeConns = connections.Where(c => c.Type == "exchange").ToList();

            // Exchanges use cached data on startup — fetch only if cache is empty
            var exchangeLabels = new HashSet<string>(exchangeConns
                .Select(c => c.Label ?? c.ExchangeName ?? "")
                .Where(n => !string.IsNullOrEmpty(n)));
            if (exchangeLabels.Count > 0)
            {
                var cachedAll = MockDataService.GetCryptoHoldings();
                var exchangeCached = cachedAll.Where(h =>
                    !string.IsNullOrEmpty(h.WalletLabel) && exchangeLabels.Contains(h.WalletLabel)).ToList();
                if (exchangeCached.Count > 0)
                {
                    allHoldings.AddRange(exchangeCached);
                }
                else
                {
                    foreach (var conn in exchangeConns)
                    {
                        try
                        {
                            if (conn.ExchangeName == "Coinbase")
                            {
                                var holdings = await CoinbaseService.GetHoldingsAsync(conn.ApiKey, conn.Secret, conn.Label ?? conn.ExchangeName ?? "Coinbase");
                                allHoldings.AddRange(holdings);
                            }
                        }
                        catch { }
                    }
                }
            }

            if (dashboard != null)
                await dashboard.UpdateLoading("Fetching wallet balances...", "Querying blockchain data");

            // Fetch non-ETH wallet balances and transactions (chain APIs, not CoinGecko)
            foreach (var conn in walletConns)
            {
                try
                {
                    var displayAddr = conn.Address;
                    if (displayAddr.Length > 10)
                        displayAddr = displayAddr[..6] + "..." + displayAddr[^4..];
                    var label = string.IsNullOrEmpty(conn.Label) ? null : conn.Label;

                    var balance = await WalletBalanceService.GetBalanceAsync(conn.Symbol, conn.Address) ?? 0;
                    var transactions = new List<MockCryptoTransaction>();
                    try { transactions = await WalletTransactionService.GetTransactionsAsync(conn.Symbol, conn.Address); }
                    catch { }

                    allHoldings.Add(new MockCryptoHolding
                    {
                        Symbol = conn.Symbol,
                        Name = conn.Symbol,
                        Quantity = balance,
                        WalletLabel = label,
                        WalletAddress = displayAddr,
                        Network = conn.Symbol.ToLower(),
                        Transactions = transactions,
                    });
                }
                catch { }
            }

            // One batch CoinGecko call for all non-ETH wallet symbols
            if (walletConns.Count > 0)
            {
                try
                {
                    var symbols = walletConns.Select(c => c.Symbol).Distinct();
                    var prices = await CryptoMarketService.GetBatchPricesAsync(symbols);
                    foreach (var h in allHoldings)
                    {
                        if (prices.TryGetValue(h.Symbol, out var pp))
                        {
                            h.PriceGBP = pp.Price;
                            h.PriceUSD = pp.Price * 1.27m;
                            h.Change24h = pp.Change24h;
                            if (h.Transactions != null)
                                await HistoricalPriceService.FillPricesAsync(h.Transactions, pp.Price);
                        }
                    }
                    await Task.Delay(3000);
                }
                catch { }
            }

            if (dashboard != null)
                await dashboard.UpdateLoading("Fetching ETH wallets...", "Scanning tokens and transactions");

            // ETH wallets processed last with delays between each
            foreach (var conn in ethConns)
            {
                try
                {
                    await Task.Delay(5000);
                    var displayAddr = conn.Address;
                    if (displayAddr.Length > 10)
                        displayAddr = displayAddr[..6] + "..." + displayAddr[^4..];
                    var label = string.IsNullOrEmpty(conn.Label) ? null : conn.Label;

                    var ethHoldings = await FetchEthMultiTokenAsync(conn.Address, label, displayAddr);
                    allHoldings.AddRange(ethHoldings);
                }
                catch { }
            }

            if (allHoldings.Count > 0)
            {
                _extraHoldings.Clear();
                _extraHoldings.AddRange(allHoldings);
                MockDataService.ClearCryptoHoldings();
                MockDataService.AddCryptoHoldings(allHoldings);
                CryptoStorageService.SaveHoldingsCache(allHoldings);
                MainThread.BeginInvokeOnMainThread(() => DisplayHoldings(allHoldings));
            }

            if (dashboard != null)
                await dashboard.HideLoading();
        }

        private static readonly List<MockCryptoHolding> _extraHoldings = new();
        // Legs of transfers between the user's own wallets/exchanges (rebuilt on every display).
        private Dictionary<MockCryptoTransaction, CryptoPnlService.TransferLeg> _transfers = new(ReferenceEqualityComparer.Instance);

        private void DisplayHoldings(List<MockCryptoHolding> holdings)
        {
            var totalValue = holdings.Sum(h => h.Quantity * h.PriceGBP);
            TotalCryptoValue.Text = CryptoFormatHelper.FormatValue(totalValue);

            if (totalValue > 0)
            {
                var weightedChange = holdings.Sum(h => h.Change24h * (h.Quantity * h.PriceGBP)) / totalValue;
                TotalChange.Text = $"{(weightedChange >= 0 ? "+" : "")}{weightedChange:F1}%";
                TotalChange.TextColor = weightedChange >= 0 ? Color.FromArgb("#22C55E") : Color.FromArgb("#EF4444");
            }

            // Portfolio P&L: transfers between your own wallets/exchanges are not buys or sells.
            _transfers = CryptoPnlService.FindTransfers(holdings);
            var portfolio = CryptoPnlService.Calculate(holdings, _transfers);
            var totalPnl = portfolio.Pnl;
            var totalPnlPct = portfolio.PnlPct;
            TotalBought.Text = CryptoFormatHelper.FormatValue(portfolio.Cost);
            TotalSold.Text = CryptoFormatHelper.FormatValue(portfolio.Proceeds);
            TotalPnL.Text = $"{CryptoFormatHelper.FormatSignedValue(totalPnl)} ({totalPnlPct:+0.0;-0.0}%)";
            TotalPnL.TextColor = totalPnl >= 0 ? Color.FromArgb("#22C55E") : Color.FromArgb("#EF4444");

            HoldingCards.Children.Clear();
            BuildHoldingCards(holdings);
            foreach (var child in HoldingCards.Children.OfType<VisualElement>())
                child.Opacity = 1;
            BuildTopAssets(holdings);
            BuildPortfolioTransactions(holdings);
            BuildFearGreedGauge();
            BuildPriceAlerts();
        }

        private static readonly string _logFile = Path.Combine(FileSystem.AppDataDirectory, "eth_debug.log");

        private static void EthLog(string msg)
        {
            try { File.AppendAllText(_logFile, $"[{DateTime.Now:HH:mm:ss}] {msg}\n"); } catch { }
        }

        private async Task<List<MockCryptoHolding>> FetchEthMultiTokenAsync(string address, string label, string displayAddr)
        {
            label ??= $"ETH Wallet ({displayAddr})";
            var results = new List<MockCryptoHolding>();
            EthLog($"--- FetchEthMultiToken START for {displayAddr} ---");
            try
            {
                EthLog("Calling GetEthTokenBalancesAsync...");
                var tokenBalances = await WalletBalanceService.GetEthTokenBalancesAsync(address);
                EthLog($"Got {tokenBalances.Count} token balances: {string.Join(", ", tokenBalances.Select(t => $"{t.Symbol}={t.Balance}"))}");
                await Task.Delay(4000);

                Dictionary<string, List<MockCryptoTransaction>> allTxsBySymbol;
                try
                {
                    EthLog("Calling GetEthAllTransactionsAsync...");
                    allTxsBySymbol = await WalletTransactionService.GetEthAllTransactionsAsync(address);
                    EthLog($"Got txs for {allTxsBySymbol.Count} symbols: {string.Join(", ", allTxsBySymbol.Select(kv => $"{kv.Key}={kv.Value.Count}"))}");
                }
                catch (Exception ex)
                {
                    EthLog($"GetEthAllTransactionsAsync FAILED: {ex.Message}");
                    allTxsBySymbol = new();
                }

                var symbols = tokenBalances.Select(t => t.Symbol)
                    .Union(allTxsBySymbol.Keys).Distinct().ToList();
                EthLog($"Combined symbols ({symbols.Count}): {string.Join(", ", symbols)}");

                Dictionary<string, (decimal Price, decimal Change24h)> prices;
                try
                {
                    EthLog("Calling GetBatchPricesAsync...");
                    prices = await CryptoMarketService.GetBatchPricesAsync(symbols);
                    EthLog($"Got prices for {prices.Count} symbols");
                }
                catch (Exception ex)
                {
                    EthLog($"GetBatchPricesAsync FAILED: {ex.Message}");
                    prices = new();
                }

                foreach (var sym in symbols)
                {
                    var tb = tokenBalances.FirstOrDefault(t => t.Symbol == sym);
                    var balance = tb?.Balance ?? 0;
                    allTxsBySymbol.TryGetValue(sym, out var transactions);
                    transactions ??= new();
                    if (balance <= 0 && transactions.Count == 0)
                    {
                        EthLog($"Skipping {sym}: balance={balance}, txCount={transactions.Count}");
                        continue;
                    }

                    prices.TryGetValue(sym, out var pp);
                    if (sym != "ETH" && pp.Price <= 0)
                    {
                        EthLog($"Skipping {sym}: no CoinGecko price (spam/unknown)");
                        continue;
                    }
                    if (pp.Price > 0 && balance * pp.Price > 1_000_000m)
                    {
                        EthLog($"Skipping {sym}: value £{balance * pp.Price:N0} exceeds cap (fake token)");
                        continue;
                    }

                    await HistoricalPriceService.FillPricesAsync(transactions, pp.Price);

                    EthLog($"Adding holding: {sym} balance={balance} price={pp.Price}");
                    results.Add(new MockCryptoHolding
                    {
                        Symbol = sym,
                        Name = tb?.Name ?? sym,
                        Quantity = balance,
                        PriceGBP = pp.Price,
                        PriceUSD = pp.Price * 1.27m,
                        Change24h = pp.Change24h,
                        WalletLabel = label,
                        WalletAddress = displayAddr,
                        Network = "eth",
                        Transactions = transactions,
                    });
                }

                EthLog($"Results count: {results.Count}");
                if (results.Count == 0)
                {
                    EthLog("No results, falling back to simple ETH balance");
                    var balance = await WalletBalanceService.GetBalanceAsync("ETH", address) ?? 0;
                    decimal price = 0, change = 0;
                    try { (price, change) = await CryptoMarketService.GetSinglePriceAsync("ETH"); } catch { }
                    results.Add(new MockCryptoHolding
                    {
                        Symbol = "ETH", Name = "Ethereum", Quantity = balance,
                        PriceGBP = price, PriceUSD = price * 1.27m, Change24h = change,
                        WalletLabel = label, WalletAddress = displayAddr, Network = "eth",
                    });
                }
            }
            catch (Exception ex)
            {
                EthLog($"OUTER CATCH: {ex.GetType().Name}: {ex.Message}\n{ex.StackTrace}");
                decimal price = 0, change = 0;
                try { (price, change) = await CryptoMarketService.GetSinglePriceAsync("ETH"); } catch { }
                var balance = await WalletBalanceService.GetBalanceAsync("ETH", address) ?? 0;
                var transactions = new List<MockCryptoTransaction>();
                try { transactions = await WalletTransactionService.GetTransactionsAsync("ETH", address); } catch { }
                await HistoricalPriceService.FillPricesAsync(transactions, price);
                results.Add(new MockCryptoHolding
                {
                    Symbol = "ETH", Name = "Ethereum", Quantity = balance,
                    PriceGBP = price, PriceUSD = price * 1.27m, Change24h = change,
                    WalletLabel = label, WalletAddress = displayAddr, Network = "eth",
                    Transactions = transactions,
                });
            }
            return results;
        }

        private async Task UpdateLivePricesAsync()
        {
            try
            {
                var liveCoins = await CryptoMarketService.GetTop50Async();
                if (liveCoins == null || liveCoins.Count == 0) return;

                var lookup = liveCoins.ToDictionary(c => c.Symbol, c => c);
                var holdings = MockDataService.GetCryptoHoldings();
                bool updated = false;

                foreach (var h in holdings)
                {
                    if (lookup.TryGetValue(h.Symbol, out var live))
                    {
                        h.PriceGBP = live.PriceGBP;
                        h.Change24h = live.ChangePercent24h;
                        updated = true;
                    }
                }

                if (updated)
                    MainThread.BeginInvokeOnMainThread(() => DisplayHoldings(holdings));
            }
            catch { }
        }

        private static string GetConnectionDisplayName(SavedConnection conn)
        {
            if (conn.Type == "exchange") return conn.Label ?? conn.ExchangeName;
            if (!string.IsNullOrEmpty(conn.Label)) return conn.Label;
            if (conn.Symbol == "ETH")
            {
                var da = conn.Address;
                if (da.Length > 10) da = da[..6] + "..." + da[^4..];
                return $"ETH Wallet ({da})";
            }
            return $"{conn.Symbol} Wallet";
        }

        private void ShowLoadingPlaceholders(List<SavedConnection> connections)
        {
            HoldingCards.Children.Clear();
            foreach (var conn in connections)
            {
                var name = GetConnectionDisplayName(conn);
                var logo = new Image
                {
                    Source = "smartcube_mobile_logo_cube.png",
                    WidthRequest = 44,
                    HeightRequest = 44,
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center,
                };
                StartSpinAnimation(logo);

                var card = new Border
                {
                    BackgroundColor = Color.FromArgb("#131B2E"),
                    Stroke = Color.FromArgb("#1E2D4A"),
                    StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 16 },
                    Padding = new Thickness(20),
                    WidthRequest = 260,
                    HeightRequest = 160,
                    Opacity = 1,
                    Content = new VerticalStackLayout
                    {
                        Spacing = 12,
                        VerticalOptions = LayoutOptions.Center,
                        HorizontalOptions = LayoutOptions.Center,
                        Children =
                        {
                            new Label { Text = name, TextColor = Color.FromArgb("#F1F5F9"), FontSize = 15, FontAttributes = FontAttributes.Bold, HorizontalOptions = LayoutOptions.Center },
                            logo,
                            new Label { Text = "Loading...", TextColor = Color.FromArgb("#64748B"), FontSize = 12, HorizontalOptions = LayoutOptions.Center },
                        }
                    }
                };
                HoldingCards.Children.Add(card);
            }
        }

        private void SetAllCardsLoading(bool loading)
        {
            foreach (var child in HoldingCards.Children.OfType<Border>())
            {
                if (child.AutomationId?.StartsWith("source_") != true) continue;
                SetCardOverlay(child, loading);
            }
        }

        private void SetSourceLoading(string sourceName, bool loading)
        {
            foreach (var child in HoldingCards.Children.OfType<Border>())
            {
                if (child.AutomationId == $"source_{sourceName}")
                {
                    SetCardOverlay(child, loading);
                    break;
                }
            }
        }

        private void SetCardOverlay(Border card, bool loading)
        {
            if (card.Content is not Grid wrapper || wrapper.Children.Count < 2) return;
            var overlay = wrapper.Children.OfType<Grid>().FirstOrDefault(g => g.AutomationId == "loadingOverlay");
            if (overlay == null) return;

            overlay.IsVisible = loading;
            if (loading)
            {
                var stack = overlay.Children.OfType<VerticalStackLayout>().FirstOrDefault();
                var logo = stack?.Children.OfType<Image>().FirstOrDefault();
                if (logo != null) StartSpinAnimation(logo);
            }
        }

        private static async void StartSpinAnimation(Image logo)
        {
            try
            {
                await Task.Delay(50);
                while (logo.IsVisible && logo.Handler != null)
                {
                    await logo.RotateTo(360, 1500, Easing.Linear);
                    logo.Rotation = 0;
                }
            }
            catch { }
        }

        private void BuildHoldingCards(List<MockCryptoHolding> holdings)
        {
            var sourceColors = new[] { "#06B6D4", "#8B5CF6", "#F59E0B", "#EC4899", "#10B981" };
            var sourceIcons = new Dictionary<string, string>
            {
                ["Coinbase"] = "🟦", ["Binance"] = "🟨", ["Kraken"] = "🟪",
                ["Crypto.com"] = "🔵", ["Gemini"] = "🟩", ["Bitstamp"] = "🟢",
            };

            var groups = holdings
                .GroupBy(h => h.WalletLabel ?? $"{h.Symbol} Wallet")
                .ToList();

            int colorIdx = 0;
            foreach (var group in groups)
            {
                var sourceName = group.Key;
                var assets = group.ToList();
                var totalValue = assets.Sum(a => a.Quantity * a.PriceGBP);
                var assetCount = assets.Count;
                var accentColor = Color.FromArgb(sourceColors[colorIdx % sourceColors.Length]);
                colorIdx++;

                var isSingleAsset = assetCount == 1;
                var icon = sourceIcons.TryGetValue(sourceName, out var ic) ? ic : "🔑";
                var weightedChange = totalValue > 0
                    ? assets.Sum(a => a.Change24h * (a.Quantity * a.PriceGBP)) / totalValue : 0;

                // No P&L per wallet (coins moved in from your other wallets have no cost of their own);
                // show how many transfers link this source to the rest of the portfolio instead.
                var transferCount = assets.Sum(a => (a.Transactions ?? new()).Count(t => _transfers.ContainsKey(t)));

                var assetSymbols = string.Join(", ", assets.Select(a => a.Symbol));
                if (assetSymbols.Length > 25) assetSymbols = assetSymbols[..22] + "...";

                var pnlStack = new VerticalStackLayout { Spacing = 2 };
                pnlStack.Children.Add(new Label { Text = "Transfers", TextColor = Color.FromArgb("#64748B"), FontSize = 10 });
                pnlStack.Children.Add(new Label
                {
                    Text = transferCount > 0 ? $"🔗 {transferCount} with your other wallets" : "None",
                    TextColor = Color.FromArgb(transferCount > 0 ? "#A78BFA" : "#64748B"), FontSize = 12,
                });
                ToolTipProperties.SetText(pnlStack, "P&L is worked out for your whole portfolio, not per wallet: moving coins between your own wallets and exchanges isn't buying or selling.");

                var refreshBtnLocal = new Button
                {
                    Text = "↻",
                    TextColor = Color.FromArgb("#60A5FA"),
                    BackgroundColor = Color.FromArgb("#1E3A5F"),
                    FontSize = 14,
                    CornerRadius = 6,
                    Padding = new Thickness(6, 0),
                    HeightRequest = 26,
                    WidthRequest = 32,
                    VerticalOptions = LayoutOptions.Center,
                };
                ToolTipProperties.SetText(refreshBtnLocal, "Refresh balances and transactions for this wallet or exchange.");
                var capturedSourceName = sourceName;
                var capturedBtn = refreshBtnLocal;
                refreshBtnLocal.Clicked += async (s, e) =>
                {
                    await RefreshSingleSourceAsync(capturedSourceName, capturedBtn);
                };

                var removeBtnLocal = new Button
                {
                    Text = "✕",
                    TextColor = Color.FromArgb("#EF4444"),
                    BackgroundColor = Color.FromArgb("#3B1C1C"),
                    FontSize = 12,
                    CornerRadius = 6,
                    Padding = new Thickness(4, 0),
                    HeightRequest = 26,
                    WidthRequest = 26,
                    VerticalOptions = LayoutOptions.Center,
                };
                ToolTipProperties.SetText(removeBtnLocal, "Remove this wallet or exchange and all of its crypto assets from SmartCube.");
                var capturedRemoveName = sourceName;
                removeBtnLocal.Clicked += async (s, e) => await RemoveSourceAsync(capturedRemoveName);

                var headerRow = new Grid
                {
                    ColumnDefinitions = new ColumnDefinitionCollection
                    {
                        new ColumnDefinition(GridLength.Star),
                        new ColumnDefinition(GridLength.Auto),
                        new ColumnDefinition(GridLength.Auto),
                    },
                    ColumnSpacing = 4,
                };
                headerRow.Children.Add(new HorizontalStackLayout
                {
                    Spacing = 8,
                    Children =
                    {
                        new Label { Text = icon, FontSize = 20, VerticalOptions = LayoutOptions.Center },
                        new Label { Text = sourceName, TextColor = Color.FromArgb("#F1F5F9"), FontSize = 16, FontAttributes = FontAttributes.Bold, VerticalOptions = LayoutOptions.Center },
                    }
                });
                Grid.SetColumn(refreshBtnLocal, 1);
                headerRow.Children.Add(refreshBtnLocal);
                Grid.SetColumn(removeBtnLocal, 2);
                headerRow.Children.Add(removeBtnLocal);

                var content = new VerticalStackLayout
                {
                    Spacing = 5,
                    Children =
                    {
                        headerRow,
                        new Label { Text = CryptoFormatHelper.FormatValue(totalValue), TextColor = Color.FromArgb("#F1F5F9"), FontSize = 22, FontAttributes = FontAttributes.Bold },
                        new HorizontalStackLayout
                        {
                            Spacing = 12,
                            Children =
                            {
                                new Label { Text = $"{assetCount} asset{(assetCount == 1 ? "" : "s")}", TextColor = Color.FromArgb("#94A3B8"), FontSize = 12 },
                                new Label {
                                    Text = $"{(weightedChange >= 0 ? "+" : "")}{weightedChange:F1}%",
                                    TextColor = weightedChange >= 0 ? Color.FromArgb("#22C55E") : Color.FromArgb("#EF4444"),
                                    FontSize = 12
                                },
                            }
                        },
                        new BoxView { HeightRequest = 1, Color = Color.FromArgb("#1E2D4A"), Margin = new Thickness(0, 2) },
                        pnlStack,
                        new Label { Text = assetSymbols, TextColor = Color.FromArgb("#64748B"), FontSize = 11 },
                    }
                };

                var connections = CryptoStorageService.LoadConnections();
                var isExchange = connections.Any(c => c.Type == "exchange" &&
                    (c.Label == sourceName || c.ExchangeName == sourceName));
                if (!isExchange)
                {
                    var addAssetBtn = new Button
                    {
                        Text = "+ Add Asset",
                        TextColor = Color.FromArgb("#60A5FA"),
                        BackgroundColor = Color.FromArgb("#1E3A5F"),
                        FontSize = 11,
                        CornerRadius = 6,
                        Padding = new Thickness(8, 0),
                        HeightRequest = 24,
                        Margin = new Thickness(0, 4, 0, 0),
                    };
                    ToolTipProperties.SetText(addAssetBtn, "Add another crypto asset to this wallet.");
                    var capturedSource = sourceName;
                    addAssetBtn.Clicked += async (s, e) =>
                    {
                        await OnAddAssetToSource(capturedSource);
                    };
                    content.Children.Add(addAssetBtn);
                }

                Grid.SetColumn(content, 1);

                var cardContent = new Grid
                {
                    ColumnDefinitions = new ColumnDefinitionCollection
                    {
                        new ColumnDefinition(new GridLength(3)),
                        new ColumnDefinition(GridLength.Star),
                    },
                    ColumnSpacing = 14,
                    Children =
                    {
                        new BoxView { Color = accentColor, CornerRadius = 2 },
                        content,
                    }
                };

                var loadingLogo = new Image
                {
                    Source = "smartcube_mobile_logo_cube.png",
                    WidthRequest = 40,
                    HeightRequest = 40,
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center,
                };
                var loadingOverlay = new Grid
                {
                    AutomationId = "loadingOverlay",
                    IsVisible = false,
                    BackgroundColor = Color.FromArgb("#131B2EE8"),
                    HorizontalOptions = LayoutOptions.Fill,
                    VerticalOptions = LayoutOptions.Fill,
                    Margin = new Thickness(-20),
                    Children =
                    {
                        new VerticalStackLayout
                        {
                            Spacing = 8,
                            VerticalOptions = LayoutOptions.Center,
                            HorizontalOptions = LayoutOptions.Center,
                            Children =
                            {
                                loadingLogo,
                                new Label { Text = "Loading...", TextColor = Color.FromArgb("#94A3B8"), FontSize = 13, FontAttributes = FontAttributes.Bold, HorizontalOptions = LayoutOptions.Center },
                            }
                        }
                    }
                };

                var wrapper = new Grid { Children = { cardContent, loadingOverlay } };

                var card = new Border
                {
                    AutomationId = $"source_{sourceName}",
                    BackgroundColor = Color.FromArgb("#131B2E"),
                    Stroke = Color.FromArgb("#1E2D4A"),
                    StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 16 },
                    Padding = new Thickness(20),
                    WidthRequest = 260,
                    Opacity = 0,
                    Content = wrapper,
                };

                ToolTipProperties.SetText(card, "Tap to see full details for this wallet or exchange, including its assets and transactions.");
                var capturedAssets = assets;
                var capturedName = sourceName;
                card.GestureRecognizers.Add(new TapGestureRecognizer
                {
                    Command = new Command(async () =>
                    {
                        await card.ScaleTo(0.97, 80, Easing.CubicOut);
                        await card.ScaleTo(1.0, 80, Easing.CubicOut);
                        if (capturedAssets.Count == 1)
                        {
                            await OpenWalletDetail(capturedAssets[0]);
                        }
                        else
                        {
                            var detailPage = new ExchangeDetailPage(capturedName, capturedAssets, _culture);
                            detailPage.AssetRemoved += (h) =>
                            {
                                MainThread.BeginInvokeOnMainThread(() => RemoveHoldingFromData(h));
                            };
                            await Navigation.PushAsync(detailPage);
                        }
                    })
                });
                HoldingCards.Children.Add(card);
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
                        new Label { Text = "Add Wallet", TextColor = Color.FromArgb("#3B82F6"), FontSize = 13, FontAttributes = FontAttributes.Bold, HorizontalOptions = LayoutOptions.Center },
                    }
                }
            };
            ToolTipProperties.SetText(addCard, "Add a wallet or exchange so your crypto balances appear here.");
            addCard.GestureRecognizers.Add(new TapGestureRecognizer { Command = new Command(OnAddCrypto) });
            HoldingCards.Children.Add(addCard);
        }

        private void BuildTopAssets(List<MockCryptoHolding> holdings)
        {
            TopAssetsList.Children.Clear();

            var grouped = holdings
                .GroupBy(h => h.Symbol)
                .Select(g =>
                {
                    var totalQty = g.Sum(h => h.Quantity);
                    var price = g.FirstOrDefault(h => h.PriceGBP > 0)?.PriceGBP ?? 0;
                    var change = g.FirstOrDefault(h => h.PriceGBP > 0)?.Change24h ?? 0;
                    var totalValue = totalQty * price;
                    var txCount = g.Sum(h => h.Transactions?.Count ?? 0);
                    var name = g.FirstOrDefault()?.Name ?? g.Key;
                    return new { Symbol = g.Key, Name = name, TotalValue = totalValue, TxCount = txCount, Change = change, Price = price, TotalQty = totalQty, Holdings = g.ToList() };
                })
                .OrderByDescending(a => a.TotalValue)
                .ThenByDescending(a => a.TxCount)
                .Take(10)
                .ToList();

            if (grouped.Count == 0)
            {
                TopAssetsCard.Opacity = 0;
                return;
            }

            TopAssetsCard.Opacity = 1;

            foreach (var asset in grouped)
            {
                var coinPnl = CryptoPnlService.Calculate(asset.Holdings, _transfers);
                var costBasis = coinPnl.Cost;
                var pnl = coinPnl.HasCost ? coinPnl.Pnl : 0;
                var pnlPct = coinPnl.PnlPct;
                var pnlColor = pnl >= 0 ? Color.FromArgb("#22C55E") : Color.FromArgb("#EF4444");
                var sourceCount = asset.Holdings.Count;

                var pnlText = costBasis > 0
                    ? $"{CryptoFormatHelper.FormatSignedValue(pnl)} ({pnlPct:+0.0;-0.0}%)"
                    : "—";

                var rightStack = new VerticalStackLayout
                {
                    Spacing = 2,
                    HorizontalOptions = LayoutOptions.End,
                    VerticalOptions = LayoutOptions.Center,
                    Children =
                    {
                        new Label { Text = CryptoFormatHelper.FormatValue(asset.TotalValue), TextColor = Color.FromArgb("#F1F5F9"), FontSize = 14, FontAttributes = FontAttributes.Bold, HorizontalOptions = LayoutOptions.End },
                        new Label { Text = pnlText, TextColor = pnlColor, FontSize = 11, HorizontalOptions = LayoutOptions.End },
                    }
                };
                Grid.SetColumn(rightStack, 1);

                var changeColor = asset.Change >= 0 ? "#22C55E" : "#EF4444";

                var row = new Border
                {
                    BackgroundColor = Color.FromArgb("#1C2744"),
                    Stroke = Colors.Transparent,
                    StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 10 },
                    Padding = new Thickness(14, 10),
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
                                    new HorizontalStackLayout
                                    {
                                        Spacing = 8,
                                        Children =
                                        {
                                            new Label { Text = asset.Symbol, TextColor = Color.FromArgb("#F1F5F9"), FontSize = 15, FontAttributes = FontAttributes.Bold },
                                            new Label { Text = asset.Name, TextColor = Color.FromArgb("#94A3B8"), FontSize = 12, VerticalOptions = LayoutOptions.Center },
                                            new Label { Text = $"{(asset.Change >= 0 ? "+" : "")}{asset.Change:F1}%", TextColor = Color.FromArgb(changeColor), FontSize = 11, VerticalOptions = LayoutOptions.Center },
                                        }
                                    },
                                    new Label { Text = $"{asset.TotalQty:G} {asset.Symbol} · {asset.TxCount} tx · {sourceCount} source{(sourceCount == 1 ? "" : "s")}", TextColor = Color.FromArgb("#64748B"), FontSize = 11 },
                                }
                            },
                            rightStack,
                        }
                    }
                };

                ToolTipProperties.SetText(row, "Tap to see this coin's combined balance and transaction history across every source.");
                var capturedHoldings = asset.Holdings;
                var capturedSymbol = asset.Symbol;
                row.GestureRecognizers.Add(new TapGestureRecognizer
                {
                    Command = new Command(async () =>
                    {
                        await row.ScaleTo(0.97, 80, Easing.CubicOut);
                        await row.ScaleTo(1.0, 80, Easing.CubicOut);
                        await Navigation.PushAsync(new CombinedAssetPage(capturedSymbol, capturedHoldings, _culture));
                    })
                });

                TopAssetsList.Children.Add(row);
            }
        }

        private static readonly ColumnDefinitionCollection TxColumns = new()
        {
            new ColumnDefinition(new GridLength(24)),   // 0: icon
            new ColumnDefinition(new GridLength(80)),   // 1: date
            new ColumnDefinition(new GridLength(50)),   // 2: type
            new ColumnDefinition(new GridLength(55)),   // 3: symbol
            new ColumnDefinition(new GridLength(90)),   // 4: source
            new ColumnDefinition(new GridLength(90)),   // 5: from
            new ColumnDefinition(new GridLength(90)),   // 6: to
            new ColumnDefinition(GridLength.Star),      // 7: quantity
            new ColumnDefinition(new GridLength(70)),   // 8: price
            new ColumnDefinition(new GridLength(75)),   // 9: value
            new ColumnDefinition(new GridLength(90)),   // 10: P&L
            new ColumnDefinition(new GridLength(85)),   // 11: hash
        };

        private void BuildPortfolioTransactions(List<MockCryptoHolding> holdings)
        {
            _lastHoldings = holdings;
            TxHistoryHeader.Children.Clear();
            TxHistoryList.Children.Clear();

            var allTxs = GetAllPortfolioTxs(holdings);

            if (allTxs.Count == 0)
            {
                TxHistoryCard.Opacity = 0;
                return;
            }

            TxHistoryCard.Opacity = 1;
            BuildTxFilterButtons(allTxs);

            var header = new Grid { ColumnDefinitions = TxColumns, ColumnSpacing = 6, Padding = new Thickness(12, 0, 12, 4) };
            string[] headers = { "", "Date", "Type", "Asset", "Source", "From", "To", "Quantity", "Price", "Value", "P&L", "Hash" };
            for (int i = 0; i < headers.Length; i++)
                AddTxHeaderCell(header, headers[i], i);
            TxHistoryHeader.Children.Add(header);
            TxHistoryHeader.Children.Add(new BoxView { HeightRequest = 1, Color = Color.FromArgb("#1E2D4A"), Margin = new Thickness(12, 0) });

            BuildTxRows(allTxs);
        }

        private static List<(MockCryptoTransaction Tx, string Source, decimal CurrentPrice)> GetAllPortfolioTxs(List<MockCryptoHolding> holdings)
        {
            var allTxs = new List<(MockCryptoTransaction Tx, string Source, decimal CurrentPrice)>();
            foreach (var h in holdings)
            {
                if (h.Transactions == null) continue;
                var src = h.WalletLabel ?? $"{h.Symbol} Wallet";
                foreach (var tx in h.Transactions)
                    allTxs.Add((tx, src, h.PriceGBP));
            }
            return allTxs;
        }

        private void BuildTxFilterButtons(List<(MockCryptoTransaction Tx, string Source, decimal CurrentPrice)> allTxs)
        {
            TxFilterBar.Children.Clear();
            _txFilterButtons.Clear();

            var hasBuy = allTxs.Any(t => t.Tx.Type == "Buy");
            var hasSell = allTxs.Any(t => t.Tx.Type == "Sell");
            var hasSwap = allTxs.Any(t => t.Tx.Type == "Swap");
            var hasWithdraw = allTxs.Any(t => t.Tx.Type == "Withdraw");
            var hasFees = allTxs.Any(t => t.Tx.Quantity < 0.01m);

            var filterList = new List<string> { "All", "Receive", "Send" };
            if (hasBuy) filterList.Add("Buy");
            if (hasSell) filterList.Add("Sell");
            if (hasSwap) filterList.Add("Swap");
            if (hasWithdraw) filterList.Add("Withdraw");
            if (hasFees) filterList.Add("No Fees");

            foreach (var filter in filterList)
            {
                var isActive = (filter == "All" && _txFilters.Count == 0) || _txFilters.Contains(filter);
                var btn = new Border
                {
                    BackgroundColor = Color.FromArgb(isActive ? "#1E3A5F" : "#131B2E"),
                    Stroke = Color.FromArgb(isActive ? "#3B82F6" : "#1E2D4A"),
                    StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 8 },
                    Padding = new Thickness(16, 6),
                    Content = new Label
                    {
                        Text = filter,
                        TextColor = Color.FromArgb(isActive ? "#60A5FA" : "#94A3B8"),
                        FontSize = 12,
                        FontAttributes = FontAttributes.Bold,
                        HorizontalOptions = LayoutOptions.Center,
                    }
                };
                ToolTipProperties.SetText(btn, filter == "All"
                    ? "Show every transaction type in the list below."
                    : filter == "No Fees"
                        ? "Hide small dust and fee transactions from the list below."
                        : $"Show only {filter} transactions in the list below.");
                var f = filter;
                btn.GestureRecognizers.Add(new TapGestureRecognizer
                {
                    Command = new Command(() => ApplyTxFilter(f, btn))
                });
                _txFilterButtons[filter] = btn;
                TxFilterBar.Children.Add(btn);
            }
        }

        private static void SetTxButtonActive(Border btn, bool active)
        {
            btn.BackgroundColor = Color.FromArgb(active ? "#1E3A5F" : "#131B2E");
            btn.Stroke = Color.FromArgb(active ? "#3B82F6" : "#1E2D4A");
            ((Label)btn.Content).TextColor = Color.FromArgb(active ? "#60A5FA" : "#94A3B8");
        }

        private void ApplyTxFilter(string filter, Border btn)
        {
            if (filter == "All")
            {
                _txFilters.Clear();
                foreach (var kvp in _txFilterButtons)
                    SetTxButtonActive(kvp.Value, kvp.Key == "All");
            }
            else if (filter == "No Fees")
            {
                if (_txFilters.Contains("No Fees"))
                    _txFilters.Remove("No Fees");
                else
                    _txFilters.Add("No Fees");
                SetTxButtonActive(btn, _txFilters.Contains("No Fees"));
                if (_txFilterButtons.TryGetValue("All", out var allBtn))
                    SetTxButtonActive(allBtn, _txFilters.Count == 0);
            }
            else
            {
                if (_txFilters.Contains(filter))
                    _txFilters.Remove(filter);
                else
                    _txFilters.Add(filter);
                SetTxButtonActive(btn, _txFilters.Contains(filter));
                if (_txFilterButtons.TryGetValue("All", out var allBtn))
                    SetTxButtonActive(allBtn, _txFilters.Count == 0);
            }

            var allTxs = GetAllPortfolioTxs(_lastHoldings ?? new());
            var typeFilters = _txFilters.Where(f => f != "No Fees").ToHashSet();
            if (typeFilters.Count > 0)
                allTxs = allTxs.Where(t => typeFilters.Contains(t.Tx.Type)).ToList();
            if (_txFilters.Contains("No Fees"))
                allTxs = allTxs.Where(t => t.Tx.Quantity >= 0.01m).ToList();

            BuildTxRows(allTxs);
        }

        private void BuildTxRows(List<(MockCryptoTransaction Tx, string Source, decimal CurrentPrice)> allTxs)
        {
            TxHistoryList.Children.Clear();

            if (allTxs.Count == 0)
            {
                TxHistoryList.Children.Add(new Label
                {
                    Text = "No transactions found",
                    TextColor = Color.FromArgb("#64748B"),
                    FontSize = 13,
                    HorizontalOptions = LayoutOptions.Center,
                    Margin = new Thickness(0, 20),
                });
                return;
            }

            var txList = allTxs.Select(t => t.Tx).ToList();
            var linkedGroups = TransactionHelper.FindLinkedGroups(txList);
            var addressMap = TransactionHelper.BuildAddressMap();

            foreach (var item in allTxs.OrderByDescending(t => t.Tx.Date))
            {
                var tx = item.Tx;
                var source = item.Source;
                var price = item.CurrentPrice;

                var icon = tx.Type switch { "Receive" => "⬇", "Send" => "⬆", "Buy" => "🛒", "Sell" => "💰", "Swap" => "🔄", "Withdraw" => "🏦", _ => "➡" };
                var typeColor = tx.Type switch { "Receive" => "#22C55E", "Send" => "#EF4444", "Buy" => "#3B82F6", "Sell" => "#F97316", "Swap" => "#F59E0B", "Withdraw" => "#A855F7", _ => "#94A3B8" };
                var typeLabel = tx.Type == "Swap" && !string.IsNullOrEmpty(tx.SwapFor) ? $"Swap -> {tx.SwapFor}" : tx.Type;
                var qtyPrefix = (tx.Type == "Receive" || tx.Type == "Buy") ? "+" : (tx.Type == "Send" || tx.Type == "Sell" || tx.Type == "Withdraw") ? "-" : "";

                var valueAtTime = tx.PriceAtTime > 0 ? tx.Quantity * tx.PriceAtTime : 0;
                var valueNow = tx.Quantity * price;
                var gainLoss = tx.PriceAtTime > 0 ? valueNow - valueAtTime : 0;
                var glPct = valueAtTime > 0 ? (gainLoss / valueAtTime) * 100 : 0;
                var glColor = gainLoss >= 0 ? "#22C55E" : "#EF4444";

                var isLinked = linkedGroups.TryGetValue(tx, out var groupIdx);
                var bgColor = isLinked ? TransactionHelper.GetLinkBgColor(groupIdx) : "#131B2E";
                var linkColor = isLinked ? TransactionHelper.GetLinkColor(groupIdx) : null;
                var transfer = _transfers.TryGetValue(tx, out var leg) ? leg : null;
                if (transfer != null) typeLabel = "Transfer";

                var fromInfo = TransactionHelper.ResolveWithSource(tx.FromAddress, tx.Type, true, source, addressMap, source);
                var toInfo = TransactionHelper.ResolveWithSource(tx.ToAddress, tx.Type, false, source, addressMap, source);

                var fromCell = TxCell(fromInfo.Display, fromInfo.Color, 5, 9);
                var toCell = TxCell(toInfo.Display, toInfo.Color, 6, 9);

                if (fromInfo.CanClaim)
                {
                    var addr = fromInfo.FullAddress;
                    fromCell.TextDecorations = TextDecorations.Underline;
                    ToolTipProperties.SetText(fromCell, "Tap to label this address as one of your own wallets or exchanges.");
                    fromCell.GestureRecognizers.Add(new TapGestureRecognizer
                    {
                        Command = new Command(async () => await TransactionHelper.ClaimAddress(addr, Navigation.NavigationStack.LastOrDefault() ?? Application.Current.Windows[0].Page))
                    });
                }
                if (toInfo.CanClaim)
                {
                    var addr = toInfo.FullAddress;
                    toCell.TextDecorations = TextDecorations.Underline;
                    ToolTipProperties.SetText(toCell, "Tap to label this address as one of your own wallets or exchanges.");
                    toCell.GestureRecognizers.Add(new TapGestureRecognizer
                    {
                        Command = new Command(async () => await TransactionHelper.ClaimAddress(addr, Navigation.NavigationStack.LastOrDefault() ?? Application.Current.Windows[0].Page))
                    });
                }

                var row = new Border
                {
                    BackgroundColor = Color.FromArgb(bgColor),
                    Stroke = isLinked ? Color.FromArgb(TransactionHelper.GetLinkStrokeColor(groupIdx)) : Colors.Transparent,
                    StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 6 },
                    Padding = new Thickness(12, 8),
                    Content = new Grid
                    {
                        ColumnDefinitions = TxColumns,
                        ColumnSpacing = 6,
                        Children =
                        {
                            TxCell(isLinked ? "🔗" : icon, isLinked ? linkColor : typeColor, 0, 14),
                            TxCell(tx.Date.ToString("dd MMM yy\nHH:mm"), "#94A3B8", 1, 10),
                            TxCell(typeLabel, typeColor, 2, 11, FontAttributes.Bold),
                            TxCell(tx.Symbol, "#06B6D4", 3, 12, FontAttributes.Bold),
                            TxCell(source, "#64748B", 4, 10),
                            fromCell,
                            toCell,
                            TxCell($"{qtyPrefix}{tx.Quantity:G}", typeColor, 7, 12, FontAttributes.Bold),
                            TxCell(tx.PriceAtTime > 0 ? CryptoFormatHelper.FormatPrice(tx.PriceAtTime) : "—", "#F1F5F9", 8, 11),
                            TxCell(valueAtTime > 0 ? CryptoFormatHelper.FormatValue(valueAtTime) : "—", "#F1F5F9", 9, 11),
                            transfer != null
                                ? TxCell((transfer.Outgoing ? "→ " : "← ") + transfer.OtherSide, "#A78BFA", 10, 10)
                                : TxCell(tx.PriceAtTime > 0
                                    ? $"{CryptoFormatHelper.FormatSignedValue(gainLoss)}\n({glPct:+0.0;-0.0}%)"
                                    : "—",
                                    tx.PriceAtTime > 0 ? glColor : "#64748B", 10, 10, FontAttributes.Bold),
                            TxCell(tx.Hash, "#475569", 11, 9),
                        }
                    }
                };
                TxHistoryList.Children.Add(row);
            }
        }

        private async void OnExportPortfolioPdf(object sender, EventArgs e)
        {
            var allTxs = GetAllPortfolioTxs(_lastHoldings ?? new());
            var typeFilters = _txFilters.Where(f => f != "No Fees").ToHashSet();
            if (typeFilters.Count > 0)
                allTxs = allTxs.Where(t => typeFilters.Contains(t.Tx.Type)).ToList();
            if (_txFilters.Contains("No Fees"))
                allTxs = allTxs.Where(t => t.Tx.Quantity >= 0.01m).ToList();

            var addressMap = TransactionHelper.BuildAddressMap();
            var rows = allTxs.OrderByDescending(t => t.Tx.Date)
                .Select(t => TransactionHelper.ToReportRow(t.Tx, t.Source, t.CurrentPrice, addressMap))
                .ToList();

            var filters = _txFilters.Count > 0 ? _txFilters.ToList() : new List<string> { "All" };
            await PdfReportService.GenerateAndOpenAsync("Portfolio", "All sources combined", filters, rows);
        }

        private static void AddTxHeaderCell(Grid grid, string text, int col)
        {
            var lbl = new Label
            {
                Text = text,
                TextColor = Color.FromArgb("#64748B"),
                FontSize = 10,
                FontAttributes = FontAttributes.Bold,
                VerticalOptions = LayoutOptions.Center,
            };
            Grid.SetColumn(lbl, col);
            grid.Children.Add(lbl);
        }

        private static Label TxCell(string text, string color, int col, int size, FontAttributes attrs = FontAttributes.None)
        {
            var lbl = new Label
            {
                Text = text,
                TextColor = Color.FromArgb(color),
                FontSize = size,
                FontAttributes = attrs,
                VerticalOptions = LayoutOptions.Center,
                LineBreakMode = LineBreakMode.TailTruncation,
            };
            Grid.SetColumn(lbl, col);
            return lbl;
        }

        private void BuildFearGreedGauge()
        {
            _ = LoadFearGreedAsync();
        }

        private async Task LoadFearGreedAsync()
        {
            try
            {
                var (score, label, description) = await CryptoMarketService.GetFearGreedAsync();
                MainThread.BeginInvokeOnMainThread(() => ApplyFearGreed(score, label, description));
            }
            catch
            {
                MainThread.BeginInvokeOnMainThread(() => ApplyFearGreed(50, "Neutral", "Unable to fetch live sentiment data."));
            }
        }

        private void ApplyFearGreed(int score, string label, string description)
        {
            FearGreedScore.Text = score.ToString();
            FearGreedLabel.Text = label;
            FearGreedDesc.Text = description;

            string color;
            if (score <= 25) color = "#EF4444";
            else if (score <= 45) color = "#F97316";
            else if (score <= 55) color = "#EAB308";
            else if (score <= 75) color = "#22C55E";
            else color = "#10B981";

            FearGreedLabel.TextColor = Color.FromArgb(color);
            FearGreedGauge.Drawable = new FearGreedDrawable(score, color);
        }

        private void BuildPriceAlerts()
        {
            AlertsList.Children.Clear();
            var alerts = MockDataService.GetPriceAlerts();
            var holdings = MockDataService.GetCryptoHoldings();

            if (alerts.Count == 0)
            {
                AlertsList.Children.Add(new Label
                {
                    Text = "No price alerts set. Tap '+ Add Alert' to get notified when a coin hits your target price.",
                    TextColor = Color.FromArgb("#64748B"),
                    FontSize = 12,
                    Margin = new Thickness(0, 8),
                });
                return;
            }

            foreach (var alert in alerts)
            {
                var holding = holdings.FirstOrDefault(h => h.Symbol == alert.Symbol);
                var currentPrice = holding?.PriceGBP ?? 0;
                var isTriggered = alert.Direction == "Above"
                    ? currentPrice >= alert.TargetPrice
                    : currentPrice <= alert.TargetPrice;
                alert.IsTriggered = isTriggered;

                var pctAway = currentPrice > 0
                    ? ((alert.TargetPrice - currentPrice) / currentPrice) * 100
                    : 0;

                var statusColor = isTriggered ? Color.FromArgb("#22C55E") : Color.FromArgb("#64748B");
                var statusText = isTriggered ? "TRIGGERED" : $"{Math.Abs(pctAway):F1}% away";
                var dirIcon = alert.Direction == "Above" ? "↑" : "↓";

                var alertRow = new Border
                {
                    BackgroundColor = isTriggered ? Color.FromArgb("#0D2818") : Color.FromArgb("#1C2744"),
                    StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 8 },
                    Stroke = isTriggered ? Color.FromArgb("#22C55E40") : Colors.Transparent,
                    Padding = new Thickness(12, 10),
                };

                var removeBtn = new Button
                {
                    Text = "✕",
                    BackgroundColor = Colors.Transparent,
                    TextColor = Color.FromArgb("#EF4444"),
                    FontSize = 14,
                    Padding = new Thickness(4, 0),
                    HeightRequest = 28,
                    WidthRequest = 28,
                    VerticalOptions = LayoutOptions.Center,
                };
                ToolTipProperties.SetText(removeBtn, "Delete this price alert.");
                var capturedAlert = alert;
                removeBtn.Clicked += (s, e) =>
                {
                    MockDataService.RemovePriceAlert(capturedAlert);
                    BuildPriceAlerts();
                };

                var grid = new Grid
                {
                    ColumnDefinitions = new ColumnDefinitionCollection
                    {
                        new ColumnDefinition(GridLength.Auto),
                        new ColumnDefinition(GridLength.Star),
                        new ColumnDefinition(GridLength.Auto),
                        new ColumnDefinition(GridLength.Auto),
                    },
                    ColumnSpacing = 10,
                };

                var symbolLabel = new Label
                {
                    Text = $"{alert.Symbol}",
                    TextColor = Color.FromArgb("#06B6D4"),
                    FontSize = 14,
                    FontAttributes = FontAttributes.Bold,
                    VerticalOptions = LayoutOptions.Center,
                };

                var detailStack = new VerticalStackLayout
                {
                    Spacing = 1,
                    Children =
                    {
                        new Label
                        {
                            Text = $"{dirIcon} {alert.Direction} {CryptoFormatHelper.FormatPrice(alert.TargetPrice)}",
                            TextColor = Color.FromArgb("#F1F5F9"),
                            FontSize = 13,
                        },
                        new Label
                        {
                            Text = $"Current: {CryptoFormatHelper.FormatPrice(currentPrice)}",
                            TextColor = Color.FromArgb("#94A3B8"),
                            FontSize = 11,
                        },
                    }
                };

                var statusLabel = new Label
                {
                    Text = statusText,
                    TextColor = statusColor,
                    FontSize = 11,
                    FontAttributes = FontAttributes.Bold,
                    VerticalOptions = LayoutOptions.Center,
                };

                Grid.SetColumn(symbolLabel, 0);
                Grid.SetColumn(detailStack, 1);
                Grid.SetColumn(statusLabel, 2);
                Grid.SetColumn(removeBtn, 3);
                grid.Children.Add(symbolLabel);
                grid.Children.Add(detailStack);
                grid.Children.Add(statusLabel);
                grid.Children.Add(removeBtn);

                alertRow.Content = grid;
                AlertsList.Children.Add(alertRow);
            }
        }

        private async Task RefreshSingleSourceAsync(string sourceName, Button refreshBtn)
        {
            refreshBtn.Text = "⟳";
            refreshBtn.IsEnabled = false;
            SetSourceLoading(sourceName, true);

            var dashboard = FindDashboard();
            if (dashboard != null)
                await dashboard.ShowLoading($"Refreshing {sourceName}...", "Fetching latest balances");

            try
            {
                var connections = CryptoStorageService.LoadConnections();
                var matching = connections.Where(c =>
                {
                    if (c.Type == "exchange") return (c.Label ?? c.ExchangeName) == sourceName;
                    if (!string.IsNullOrEmpty(c.Label)) return c.Label == sourceName;
                    if (c.Symbol == "ETH")
                    {
                        var da = c.Address;
                        if (da.Length > 10) da = da[..6] + "..." + da[^4..];
                        return $"ETH Wallet ({da})" == sourceName;
                    }
                    return $"{c.Symbol} Wallet" == sourceName;
                }).ToList();

                if (matching.Count == 0) return;

                var newHoldings = new List<MockCryptoHolding>();
                foreach (var conn in matching)
                {
                    try
                    {
                        if (conn.Type == "wallet")
                        {
                            var displayAddr = conn.Address;
                            if (displayAddr.Length > 10)
                                displayAddr = displayAddr[..6] + "..." + displayAddr[^4..];
                            var label = string.IsNullOrEmpty(conn.Label) ? null : conn.Label;

                            if (conn.Symbol == "ETH")
                            {
                                var ethHoldings = await FetchEthMultiTokenAsync(conn.Address, label, displayAddr);
                                newHoldings.AddRange(ethHoldings);
                            }
                            else
                            {
                                decimal price = 0, change = 0;
                                try { (price, change) = await CryptoMarketService.GetSinglePriceAsync(conn.Symbol); } catch { }
                                var balance = await WalletBalanceService.GetBalanceAsync(conn.Symbol, conn.Address) ?? 0;
                                var transactions = new List<MockCryptoTransaction>();
                                try { transactions = await WalletTransactionService.GetTransactionsAsync(conn.Symbol, conn.Address); } catch { }
                                await HistoricalPriceService.FillPricesAsync(transactions, price);
                                newHoldings.Add(new MockCryptoHolding
                                {
                                    Symbol = conn.Symbol, Name = conn.Symbol, Quantity = balance,
                                    PriceGBP = price, PriceUSD = price * 1.27m, Change24h = change,
                                    WalletLabel = label, WalletAddress = displayAddr, Network = conn.Symbol.ToLower(),
                                    Transactions = transactions,
                                });
                            }
                        }
                        else if (conn.Type == "exchange" && conn.ExchangeName == "Coinbase")
                        {
                            var holdings = await CoinbaseService.GetHoldingsAsync(conn.ApiKey, conn.Secret, conn.Label ?? conn.ExchangeName ?? "Coinbase");
                            newHoldings.AddRange(holdings);
                        }
                    }
                    catch { }
                }

                if (newHoldings.Count > 0)
                {
                    _extraHoldings.RemoveAll(h =>
                    {
                        var src = h.WalletLabel ?? $"{h.Symbol} Wallet";
                        return src == sourceName;
                    });
                    _extraHoldings.AddRange(newHoldings);

                    MockDataService.ClearCryptoHoldings();
                    MockDataService.AddCryptoHoldings(_extraHoldings);
                    CryptoStorageService.SaveHoldingsCache(_extraHoldings);
                    MainThread.BeginInvokeOnMainThread(() => DisplayHoldings(_extraHoldings));
                }
            }
            finally
            {
                if (dashboard != null)
                    await dashboard.HideLoading();

                MainThread.BeginInvokeOnMainThread(() =>
                {
                    refreshBtn.Text = "↻";
                    refreshBtn.IsEnabled = true;
                    SetSourceLoading(sourceName, false);
                });
            }
        }

        private async void OnAddAlertClicked(object sender, EventArgs e)
        {
            var holdings = MockDataService.GetCryptoHoldings();
            var symbols = holdings.Select(h => h.Symbol).ToArray();

            string symbol = await Application.Current.Windows[0].Page.DisplayActionSheet(
                "Select coin", "Cancel", null, symbols);
            if (symbol == null || symbol == "Cancel") return;

            string direction = await Application.Current.Windows[0].Page.DisplayActionSheet(
                "Alert when price goes...", "Cancel", null, "Above", "Below");
            if (direction == null || direction == "Cancel") return;

            var holding = holdings.FirstOrDefault(h => h.Symbol == symbol);
            string priceStr = await Application.Current.Windows[0].Page.DisplayPromptAsync(
                "Target Price",
                $"Current {symbol}: {(holding != null ? CryptoFormatHelper.FormatPrice(holding.PriceGBP) : "N/A")}\nEnter target price (£):",
                keyboard: Keyboard.Numeric);
            if (priceStr == null) return;

            if (!decimal.TryParse(priceStr, out var target))
            {
                await Application.Current.Windows[0].Page.DisplayAlert("Error", "Invalid price.", "OK");
                return;
            }

            MockDataService.AddPriceAlert(new CryptoPriceAlert
            {
                Symbol = symbol,
                Name = holding?.Name ?? symbol,
                TargetPrice = target,
                Direction = direction,
                CreatedAt = DateTime.Now,
            });

            BuildPriceAlerts();
        }

        private async Task OpenWalletDetail(MockCryptoHolding holding)
        {
            var page = new WalletDetailPage(holding, _culture);
            await Navigation.PushAsync(page);
        }

        private async Task OnAddAssetToSource(string sourceName)
        {
            var flow = new AddCryptoFlow(prefillLabel: sourceName);
            flow.CryptoAdded += () =>
            {
                MainThread.BeginInvokeOnMainThread(RefreshData);
            };
            await Navigation.PushAsync(flow);
        }

        private async Task RemoveSourceAsync(string sourceName)
        {
            var page = Application.Current?.Windows.FirstOrDefault()?.Page;
            if (page == null) return;

            var confirm = await page.DisplayAlert(
                "Remove Source",
                $"Remove \"{sourceName}\" and all its assets?",
                "Remove", "Cancel");
            if (!confirm) return;

            var connections = CryptoStorageService.LoadConnections();
            var toRemove = connections.Where(c =>
            {
                if (c.Type == "exchange") return (c.Label ?? c.ExchangeName) == sourceName;
                if (!string.IsNullOrEmpty(c.Label)) return c.Label == sourceName;
                if (c.Symbol == "ETH")
                {
                    var da = c.Address;
                    if (da.Length > 10) da = da[..6] + "..." + da[^4..];
                    return $"ETH Wallet ({da})" == sourceName;
                }
                return $"{c.Symbol} Wallet" == sourceName;
            }).ToList();

            foreach (var conn in toRemove)
                CryptoStorageService.RemoveConnection(conn);

            _extraHoldings.RemoveAll(h =>
            {
                var src = h.WalletLabel ?? $"{h.Symbol} Wallet";
                return src == sourceName;
            });

            MockDataService.ClearCryptoHoldings();
            if (_extraHoldings.Count > 0)
                MockDataService.AddCryptoHoldings(_extraHoldings);
            CryptoStorageService.SaveHoldingsCache(_extraHoldings);
            DisplayHoldings(_extraHoldings);
        }

        private void RemoveHoldingFromData(MockCryptoHolding holding)
        {
            _extraHoldings.Remove(holding);
            MockDataService.ClearCryptoHoldings();
            if (_extraHoldings.Count > 0)
                MockDataService.AddCryptoHoldings(_extraHoldings);
            CryptoStorageService.SaveHoldingsCache(_extraHoldings);
            DisplayHoldings(_extraHoldings);
        }

        private async void OnAddCrypto()
        {
            var flow = new AddCryptoFlow();
            flow.CryptoAdded += () =>
            {
                MainThread.BeginInvokeOnMainThread(RefreshData);
            };
            await Navigation.PushAsync(flow);
        }

        private async void OnHelpClicked(object sender, EventArgs e)
        {
            var page = Application.Current?.Windows.FirstOrDefault()?.Page;
            if (page != null)
                await page.DisplayAlert("Crypto", "This section tracks your crypto wallets and exchange accounts. Tap Add Wallet to connect a new wallet or exchange, and tap a card to see its full details. Top Assets and Transaction History summarise your holdings and activity across every connected source, and Price Alerts lets you know when a coin crosses a target price.\n\nProfit & loss is worked out for your whole portfolio (in total and per coin in Top Assets), not per wallet. When you move coins between your own wallets and exchanges, for example buying on Coinbase and sending to your own wallet, SmartCube matches the send with the receive and marks both as a Transfer: it isn't a sale or a purchase, so it doesn't change your P&L. The original purchase price stays as your cost. All balances and transactions are fetched live and stored locally on this PC.", "OK");
        }

        private void RefreshData()
        {
            HoldingCards.Children.Clear();
            AlertsList.Children.Clear();
            LoadData();
            foreach (var child in HoldingCards.Children.OfType<VisualElement>())
                child.Opacity = 1;
            _ = UpdateLivePricesAsync();
        }

        public async Task PlayEntryAnimation()
        {
            _ = AnimationHelper.ScaleIn(PortfolioCard, 0, 400);
            await AnimationHelper.StaggerIn(HoldingCards, 100, 450);
            _ = AnimationHelper.FadeIn(TopAssetsCard, 25, 400);
            _ = AnimationHelper.FadeIn(TxHistoryCard, 50, 400);
            _ = AnimationHelper.FadeIn(BubbleCard, 75, 400);
            _ = AnimationHelper.FadeIn(FearGreedCard, 150, 400);
            _ = AnimationHelper.FadeIn(AlertsCard, 200, 400);
        }
    }

    public class CryptoTxDisplayItem
    {
        public string TypeIcon { get; set; }
        public string Description { get; set; }
        public DateTime Date { get; set; }
        public string QuantityFormatted { get; set; }
        public string Hash { get; set; }
    }

    public class SparklineDrawable : IDrawable
    {
        private readonly decimal[] _data;
        private readonly string _color;

        public SparklineDrawable(decimal[] data, string color)
        {
            _data = data;
            _color = color;
        }

        public void Draw(ICanvas canvas, RectF dirtyRect)
        {
            if (_data == null || _data.Length < 2) return;

            var min = _data.Min();
            var max = _data.Max();
            var range = max - min;
            if (range == 0) range = 1;

            var w = dirtyRect.Width;
            var h = dirtyRect.Height;
            var padding = 2f;

            canvas.StrokeColor = Color.FromArgb(_color);
            canvas.StrokeSize = 2;
            canvas.StrokeLineCap = LineCap.Round;
            canvas.StrokeLineJoin = LineJoin.Round;

            var path = new PathF();
            for (int i = 0; i < _data.Length; i++)
            {
                var x = padding + (w - 2 * padding) * i / (_data.Length - 1);
                var y = h - padding - (h - 2 * padding) * (float)((_data[i] - min) / range);
                if (i == 0) path.MoveTo(x, y);
                else path.LineTo(x, y);
            }
            canvas.DrawPath(path);

            canvas.FillColor = Color.FromArgb(_color + "30");
            var fillPath = new PathF(path);
            fillPath.LineTo(w - padding, h - padding);
            fillPath.LineTo(padding, h - padding);
            fillPath.Close();
            canvas.FillPath(fillPath);
        }
    }

    public class FearGreedDrawable : IDrawable
    {
        private readonly int _score;
        private readonly string _color;

        public FearGreedDrawable(int score, string color)
        {
            _score = score;
            _color = color;
        }

        public void Draw(ICanvas canvas, RectF dirtyRect)
        {
            var cx = dirtyRect.Width / 2;
            var cy = dirtyRect.Height / 2 + 10;
            var radius = Math.Min(cx, cy) - 8;
            var strokeWidth = 12f;

            canvas.StrokeSize = strokeWidth;
            canvas.StrokeLineCap = LineCap.Round;

            canvas.StrokeColor = Color.FromArgb("#1E2D4A");
            canvas.DrawArc(cx - radius, cy - radius, radius * 2, radius * 2, 180, 0, false, false);

            var sweepAngle = 180f * _score / 100f;
            canvas.StrokeColor = Color.FromArgb(_color);
            canvas.DrawArc(cx - radius, cy - radius, radius * 2, radius * 2, 180, 180 - sweepAngle, false, false);

            canvas.FontSize = 9;
            canvas.FontColor = Color.FromArgb("#64748B");
            canvas.DrawString("Fear", cx - radius - 2, cy + 6, HorizontalAlignment.Center);
            canvas.DrawString("Greed", cx + radius + 2, cy + 6, HorizontalAlignment.Center);
        }
    }
}
