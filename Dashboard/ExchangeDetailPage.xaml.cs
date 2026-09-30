using SmartCubeMobile.MockData;
using SmartCubeMobile.Services;
using System.Globalization;

namespace SmartCubeMobile.Dashboard
{
    public partial class ExchangeDetailPage : ContentPage
    {
        private readonly List<MockCryptoHolding> _holdings;
        private readonly CultureInfo _culture;
        private readonly string _sourceName;

        public event Action<MockCryptoHolding> AssetRemoved;

        public ExchangeDetailPage(string sourceName, List<MockCryptoHolding> holdings, CultureInfo culture)
        {
            InitializeComponent();
            _holdings = holdings;
            _culture = culture;
            _sourceName = sourceName;

            HeaderTitle.Text = sourceName;
            HeaderSubtitle.Text = $"{holdings.Count} asset{(holdings.Count == 1 ? "" : "s")}";

            BuildSummary();
            BuildTransactionHistory();
            BuildAssetCards();
        }

        private readonly HashSet<string> _activeFilters = new();
        private readonly Dictionary<string, Border> _filterButtons = new();
        private Dictionary<MockCryptoTransaction, CryptoPnlService.TransferLeg> _transfers = new(ReferenceEqualityComparer.Instance);

        private void BuildSummary()
        {
            var totalValue = _holdings.Sum(h => h.Quantity * h.PriceGBP);
            TotalValue.Text = CryptoFormatHelper.FormatValue(totalValue);
            AssetCount.Text = _holdings.Count.ToString();

            // P&L is only worked out for the whole portfolio (transfers between your own wallets and this
            // exchange aren't buys or sells), so this page shows the transfers instead.
            _transfers = CryptoPnlService.FindTransfers(MockDataService.GetCryptoHoldings().Concat(_holdings).Distinct());
            var transferCount = _holdings.Sum(h => (h.Transactions ?? new()).Count(t => _transfers.ContainsKey(t)));
            TotalPnl.Text = transferCount > 0 ? $"🔗 {transferCount}" : "None";
            TotalPnl.TextColor = Color.FromArgb("#A78BFA");
        }

        private void BuildAssetCards()
        {
            var colors = new[] { "#06B6D4", "#8B5CF6", "#F59E0B", "#EC4899", "#10B981", "#F97316" };
            var sorted = _holdings.OrderByDescending(h => h.Transactions?.Count ?? 0).ToList();

            for (int i = 0; i < sorted.Count; i++)
            {
                var h = sorted[i];
                var value = h.Quantity * h.PriceGBP;
                var accentColor = Color.FromArgb(colors[i % colors.Length]);


                var txCount = h.Transactions?.Count ?? 0;

                var refreshBtn = new Button
                {
                    Text = "↻",
                    TextColor = Color.FromArgb("#60A5FA"),
                    BackgroundColor = Color.FromArgb("#1E3A5F"),
                    FontSize = 11,
                    CornerRadius = 6,
                    Padding = new Thickness(4, 0),
                    HeightRequest = 24,
                    WidthRequest = 24,
                    VerticalOptions = LayoutOptions.Start,
                    HorizontalOptions = LayoutOptions.End,
                };
                ToolTipProperties.SetText(refreshBtn, $"Re-fetch the latest price and balance for {h.Symbol}.");
                var capturedForRefresh = h;
                var capturedRefreshBtn = refreshBtn;
                refreshBtn.Clicked += async (s, e) => await RefreshAssetAsync(capturedForRefresh, capturedRefreshBtn);

                var removeBtn = new Button
                {
                    Text = "✕",
                    TextColor = Color.FromArgb("#EF4444"),
                    BackgroundColor = Color.FromArgb("#3B1C1C"),
                    FontSize = 11,
                    CornerRadius = 6,
                    Padding = new Thickness(4, 0),
                    HeightRequest = 24,
                    WidthRequest = 24,
                    VerticalOptions = LayoutOptions.Start,
                    HorizontalOptions = LayoutOptions.End,
                };
                ToolTipProperties.SetText(removeBtn, $"Remove {h.Symbol} from {_sourceName}. Asks for confirmation first.");
                var capturedForRemove = h;
                removeBtn.Clicked += async (s, e) => await RemoveAssetAsync(capturedForRemove);

                var btnRow = new HorizontalStackLayout
                {
                    Spacing = 4,
                    HorizontalOptions = LayoutOptions.End,
                    Children = { refreshBtn, removeBtn },
                };

                var valueCol = BuildAssetValue(value, 2);

                var rightStack = new VerticalStackLayout
                {
                    Spacing = 4,
                    HorizontalOptions = LayoutOptions.End,
                    Children = { btnRow, valueCol },
                };
                Grid.SetColumn(rightStack, 2);

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
                            new ColumnDefinition(new GridLength(3)),
                            new ColumnDefinition(GridLength.Star),
                            new ColumnDefinition(GridLength.Auto),
                        },
                        ColumnSpacing = 14,
                        Children =
                        {
                            new BoxView { Color = accentColor, CornerRadius = 2 },
                            BuildAssetInfo(h, txCount, 1),
                            rightStack,
                        }
                    }
                };

                ToolTipProperties.SetText(card, $"Open the detail page for {h.Symbol} on {_sourceName}.");
                var captured = h;
                card.GestureRecognizers.Add(new TapGestureRecognizer
                {
                    Command = new Command(async () =>
                    {
                        await card.ScaleTo(0.97, 80, Easing.CubicOut);
                        await card.ScaleTo(1.0, 80, Easing.CubicOut);
                        await Navigation.PushAsync(new WalletDetailPage(captured, _culture));
                    })
                });

                AssetList.Children.Add(card);
            }
        }

        private static VerticalStackLayout BuildAssetInfo(MockCryptoHolding h, int txCount, int col)
        {
            var stack = new VerticalStackLayout
            {
                Spacing = 3,
                Children =
                {
                    new HorizontalStackLayout
                    {
                        Spacing = 8,
                        Children =
                        {
                            new Label { Text = h.Symbol, TextColor = Color.FromArgb("#F1F5F9"), FontSize = 16, FontAttributes = FontAttributes.Bold },
                            new Label { Text = h.Name, TextColor = Color.FromArgb("#94A3B8"), FontSize = 13, VerticalOptions = LayoutOptions.Center },
                        }
                    },
                    new Label
                    {
                        Text = $"{h.Quantity:G} {h.Symbol}",
                        TextColor = Color.FromArgb("#94A3B8"), FontSize = 12,
                    },
                    new Label
                    {
                        Text = $"{txCount} transaction{(txCount == 1 ? "" : "s")}",
                        TextColor = Color.FromArgb("#64748B"), FontSize = 11,
                    },
                }
            };
            Grid.SetColumn(stack, col);
            return stack;
        }

        private VerticalStackLayout BuildAssetValue(decimal value, int col)
        {
            var stack = new VerticalStackLayout
            {
                Spacing = 3,
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.End,
                Children =
                {
                    new Label
                    {
                        Text = CryptoFormatHelper.FormatValue(value),
                        TextColor = Color.FromArgb("#F1F5F9"), FontSize = 16, FontAttributes = FontAttributes.Bold,
                        HorizontalOptions = LayoutOptions.End,
                    },
                }
            };
            Grid.SetColumn(stack, col);
            return stack;
        }

        private async Task RefreshAssetAsync(MockCryptoHolding holding, Button btn)
        {
            btn.Text = "⟳";
            btn.IsEnabled = false;
            try
            {
                var prices = await CryptoMarketService.GetBatchPricesAsync(new[] { holding.Symbol });
                if (prices.TryGetValue(holding.Symbol, out var pp) && pp.Price > 0)
                {
                    holding.PriceGBP = pp.Price;
                    holding.PriceUSD = pp.Price * 1.27m;
                    holding.Change24h = pp.Change24h;
                }

                if (holding.Network == "eth" || holding.Network == "coinbase")
                {
                }
                else
                {
                    var balance = await WalletBalanceService.GetBalanceAsync(holding.Symbol, holding.WalletAddress ?? "");
                    if (balance.HasValue && balance.Value > 0)
                        holding.Quantity = balance.Value;
                }

                AssetList.Children.Clear();
                BuildSummary();
                _activeFilters.Clear();
                BuildTransactionHistory();
                BuildAssetCards();
            }
            catch { }
            finally
            {
                btn.Text = "↻";
                btn.IsEnabled = true;
            }
        }

        private async Task RemoveAssetAsync(MockCryptoHolding holding)
        {
            var confirm = await DisplayAlert(
                "Remove Asset",
                $"Remove {holding.Symbol} ({holding.Name}) from {_sourceName}?",
                "Remove", "Cancel");
            if (!confirm) return;

            _holdings.Remove(holding);
            AssetRemoved?.Invoke(holding);

            if (_holdings.Count == 0)
            {
                await Navigation.PopAsync();
                return;
            }

            HeaderSubtitle.Text = $"{_holdings.Count} asset{(_holdings.Count == 1 ? "" : "s")}";
            AssetList.Children.Clear();
            BuildSummary();
            _activeFilters.Clear();
            BuildTransactionHistory();
            BuildAssetCards();
        }

        private List<MockCryptoTransaction> GetAllTransactions()
        {
            return _holdings
                .Where(h => h.Transactions != null)
                .SelectMany(h => h.Transactions)
                .ToList();
        }

        private void BuildTransactionHistory()
        {
            var allTxs = GetAllTransactions();
            TxCountLabel.Text = $"{allTxs.Count} transactions";
            BuildFilterButtons(allTxs);
            BuildTransactionRows(allTxs);
        }

        private void BuildFilterButtons(List<MockCryptoTransaction> allTxs)
        {
            var types = allTxs.Select(t => t.Type).Distinct().ToList();
            var hasFees = allTxs.Any(t => t.Quantity < 0.01m);
            var filterList = new List<string> { "All" };
            foreach (var t in new[] { "Buy", "Sell", "Send", "Receive", "Swap", "Withdraw" })
                if (types.Contains(t)) filterList.Add(t);
            if (hasFees) filterList.Add("No Fees");

            FilterBar.Children.Clear();
            _filterButtons.Clear();
            foreach (var filter in filterList)
            {
                var isActive = (filter == "All" && _activeFilters.Count == 0) || _activeFilters.Contains(filter);
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
                    ? "Show every transaction below, with no type filter applied."
                    : filter == "No Fees"
                        ? "Hide tiny dust or fee transactions from the list below."
                        : $"Show only {filter} transactions in the list below.");

                var f = filter;
                btn.GestureRecognizers.Add(new TapGestureRecognizer
                {
                    Command = new Command(() => ApplyFilter(f, btn))
                });
                _filterButtons[filter] = btn;
                FilterBar.Children.Add(btn);
            }

            var pdfBtn = new Border
            {
                BackgroundColor = Color.FromArgb("#1E3A5F"),
                Stroke = Color.FromArgb("#3B82F6"),
                StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 8 },
                Padding = new Thickness(12, 6),
                Content = new Label { Text = "Export PDF", TextColor = Color.FromArgb("#60A5FA"), FontSize = 12, FontAttributes = FontAttributes.Bold }
            };
            ToolTipProperties.SetText(pdfBtn, "Export the transactions currently shown to a PDF report file.");
            pdfBtn.GestureRecognizers.Add(new TapGestureRecognizer { Command = new Command(async () => await ExportPdf()) });
            FilterBar.Children.Add(pdfBtn);
        }

        private async Task ExportPdf()
        {
            var allTxs = GetAllTransactions();
            var typeFilters = _activeFilters.Where(f => f != "No Fees").ToHashSet();
            if (typeFilters.Count > 0) allTxs = allTxs.Where(t => typeFilters.Contains(t.Type)).ToList();
            if (_activeFilters.Contains("No Fees")) allTxs = allTxs.Where(t => t.Quantity >= 0.01m).ToList();

            var addressMap = TransactionHelper.BuildAddressMap();
            var rows = allTxs.OrderByDescending(t => t.Date)
                .Select(t =>
                {
                    var h = _holdings.FirstOrDefault(h => h.Symbol == t.Symbol);
                    var price = h?.PriceGBP ?? 0;
                    return TransactionHelper.ToReportRow(t, _sourceName, price, addressMap);
                }).ToList();

            var filters = _activeFilters.Count > 0 ? _activeFilters.ToList() : new List<string> { "All" };
            await PdfReportService.GenerateAndOpenAsync(_sourceName, $"Exchange: {_sourceName}", filters, rows);
        }

        private static void SetButtonActive(Border btn, bool active)
        {
            btn.BackgroundColor = Color.FromArgb(active ? "#1E3A5F" : "#131B2E");
            btn.Stroke = Color.FromArgb(active ? "#3B82F6" : "#1E2D4A");
            ((Label)btn.Content).TextColor = Color.FromArgb(active ? "#60A5FA" : "#94A3B8");
        }

        private void ApplyFilter(string filter, Border btn)
        {
            if (filter == "All")
            {
                _activeFilters.Clear();
                foreach (var kvp in _filterButtons)
                    SetButtonActive(kvp.Value, kvp.Key == "All");
            }
            else if (filter == "No Fees")
            {
                if (_activeFilters.Contains("No Fees"))
                    _activeFilters.Remove("No Fees");
                else
                    _activeFilters.Add("No Fees");
                SetButtonActive(btn, _activeFilters.Contains("No Fees"));
                if (_filterButtons.TryGetValue("All", out var allBtn))
                    SetButtonActive(allBtn, _activeFilters.Count == 0);
            }
            else
            {
                if (_activeFilters.Contains(filter))
                    _activeFilters.Remove(filter);
                else
                    _activeFilters.Add(filter);

                SetButtonActive(btn, _activeFilters.Contains(filter));
                if (_filterButtons.TryGetValue("All", out var allBtn))
                    SetButtonActive(allBtn, _activeFilters.Count == 0);
            }

            var allTxs = GetAllTransactions();
            var typeFilters = _activeFilters.Where(f => f != "No Fees").ToHashSet();
            if (typeFilters.Count > 0)
                allTxs = allTxs.Where(t => typeFilters.Contains(t.Type)).ToList();
            if (_activeFilters.Contains("No Fees"))
                allTxs = allTxs.Where(t => t.Quantity >= 0.01m).ToList();
            BuildTransactionRows(allTxs);
        }

        private static readonly ColumnDefinitionCollection TxColumns = new()
        {
            new ColumnDefinition(new GridLength(24)),   // 0: icon
            new ColumnDefinition(new GridLength(80)),   // 1: date
            new ColumnDefinition(new GridLength(70)),   // 2: type
            new ColumnDefinition(new GridLength(50)),   // 3: symbol
            new ColumnDefinition(new GridLength(90)),   // 4: from
            new ColumnDefinition(new GridLength(90)),   // 5: to
            new ColumnDefinition(GridLength.Star),      // 6: quantity
            new ColumnDefinition(new GridLength(70)),   // 7: price
            new ColumnDefinition(new GridLength(75)),   // 8: value
            new ColumnDefinition(new GridLength(85)),   // 9: hash
        };

        private void BuildTransactionRows(List<MockCryptoTransaction> txs)
        {
            TransactionsList.Children.Clear();

            if (txs.Count == 0)
            {
                TransactionsList.Children.Add(new Label
                {
                    Text = "No transactions found",
                    TextColor = Color.FromArgb("#64748B"),
                    FontSize = 13,
                    HorizontalOptions = LayoutOptions.Center,
                    Margin = new Thickness(0, 20),
                });
                return;
            }

            var linkedGroups = TransactionHelper.FindLinkedGroups(txs);
            var addressMap = TransactionHelper.BuildAddressMap();

            var header = new Grid { ColumnDefinitions = TxColumns, ColumnSpacing = 6, Padding = new Thickness(12, 0, 12, 4) };
            AddHeaderCell(header, "", 0);
            AddHeaderCell(header, "Date", 1);
            AddHeaderCell(header, "Type", 2);
            AddHeaderCell(header, "Asset", 3);
            AddHeaderCell(header, "From", 4);
            AddHeaderCell(header, "To", 5);
            AddHeaderCell(header, "Quantity", 6);
            AddHeaderCell(header, "Price", 7);
            AddHeaderCell(header, "Value", 8);
            AddHeaderCell(header, "Hash", 9);
            TransactionsList.Children.Add(header);

            TransactionsList.Children.Add(new BoxView
            {
                HeightRequest = 1,
                Color = Color.FromArgb("#1E2D4A"),
                Margin = new Thickness(12, 0),
            });

            foreach (var tx in txs.OrderByDescending(t => t.Date))
            {
                var icon = tx.Type switch { "Receive" => "⬇", "Send" => "⬆", "Buy" => "🛒", "Sell" => "💰", "Swap" => "🔄", "Withdraw" => "🏦", _ => "➡" };
                var typeColor = tx.Type switch { "Receive" => "#22C55E", "Send" => "#EF4444", "Buy" => "#3B82F6", "Sell" => "#F97316", "Swap" => "#F59E0B", "Withdraw" => "#A855F7", _ => "#94A3B8" };
                var typeLabel = tx.Type == "Swap" && !string.IsNullOrEmpty(tx.SwapFor) ? $"Swap → {tx.SwapFor}" : tx.Type;
                var qtyPrefix = (tx.Type == "Receive" || tx.Type == "Buy") ? "+" : (tx.Type == "Send" || tx.Type == "Sell" || tx.Type == "Withdraw") ? "-" : "";

                var valueAtTime = tx.PriceAtTime > 0 ? tx.Quantity * tx.PriceAtTime : 0;
                var isLinked = linkedGroups.TryGetValue(tx, out var groupIdx);
                var bgColor = isLinked ? TransactionHelper.GetLinkBgColor(groupIdx) : "#131B2E";
                var linkColor = isLinked ? TransactionHelper.GetLinkColor(groupIdx) : null;

                var fromInfo = TransactionHelper.ResolveWithSource(tx.FromAddress, tx.Type, true, _sourceName, addressMap, _sourceName);
                var toInfo = TransactionHelper.ResolveWithSource(tx.ToAddress, tx.Type, false, _sourceName, addressMap, _sourceName);

                var fromCell = TxCell(fromInfo.Display, fromInfo.Color, 4, 9);
                var toCell = TxCell(toInfo.Display, toInfo.Color, 5, 9);

                if (fromInfo.CanClaim)
                {
                    var addr = fromInfo.FullAddress;
                    fromCell.TextDecorations = TextDecorations.Underline;
                    ToolTipProperties.SetText(fromCell, "Tap to link this address to one of your wallets or exchanges, or give it a custom label.");
                    fromCell.GestureRecognizers.Add(new TapGestureRecognizer
                    {
                        Command = new Command(async () => await TransactionHelper.ClaimAddress(addr, this))
                    });
                }
                if (toInfo.CanClaim)
                {
                    var addr = toInfo.FullAddress;
                    toCell.TextDecorations = TextDecorations.Underline;
                    ToolTipProperties.SetText(toCell, "Tap to link this address to one of your wallets or exchanges, or give it a custom label.");
                    toCell.GestureRecognizers.Add(new TapGestureRecognizer
                    {
                        Command = new Command(async () => await TransactionHelper.ClaimAddress(addr, this))
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
                            fromCell,
                            toCell,
                            TxCell($"{qtyPrefix}{tx.Quantity:G}", typeColor, 6, 12, FontAttributes.Bold),
                            TxCell(tx.PriceAtTime > 0 ? CryptoFormatHelper.FormatPrice(tx.PriceAtTime) : "—", "#F1F5F9", 7, 11),
                            TxCell(valueAtTime > 0 ? CryptoFormatHelper.FormatValue(valueAtTime) : "—", "#F1F5F9", 8, 11),
                            TxCell(tx.Hash, "#475569", 9, 9),
                        }
                    }
                };
                TransactionsList.Children.Add(row);
            }
        }

        private static void AddHeaderCell(Grid grid, string text, int col)
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

        private async void OnBackClicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }

        private async void OnHelpClicked(object sender, EventArgs e)
        {
            await DisplayAlert("Exchange / Wallet Detail",
                "This page shows every asset held on this exchange or wallet, its total value and profit/loss, and the full transaction history. Tap an asset card to see its own detail page, use the small refresh button to re-fetch its price and balance, or the ✕ button to remove it. Transactions can be filtered by type and exported to a PDF report. All data is stored locally on this PC.",
                "OK");
        }
    }
}
