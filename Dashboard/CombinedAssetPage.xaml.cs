using SmartCubeMobile.MockData;
using SmartCubeMobile.Services;
using System.Globalization;

namespace SmartCubeMobile.Dashboard
{
    public partial class CombinedAssetPage : ContentPage
    {
        private readonly string _symbol;
        private readonly List<MockCryptoHolding> _holdings;
        private readonly CultureInfo _culture;
        private readonly HashSet<string> _activeFilters = new();
        private readonly Dictionary<string, Border> _filterButtons = new();

        public CombinedAssetPage(string symbol, List<MockCryptoHolding> holdings, CultureInfo culture)
        {
            InitializeComponent();
            _symbol = symbol;
            _holdings = holdings;
            _culture = culture;
            PopulateData();
        }

        private void PopulateData()
        {
            var price = _holdings.FirstOrDefault(h => h.PriceGBP > 0)?.PriceGBP ?? 0;
            var change = _holdings.FirstOrDefault(h => h.PriceGBP > 0)?.Change24h ?? 0;
            var totalQty = _holdings.Sum(h => h.Quantity);
            var totalValue = totalQty * price;
            var name = _holdings.FirstOrDefault()?.Name ?? _symbol;

            var allTxs = _holdings
                .Where(h => h.Transactions != null)
                .SelectMany(h => h.Transactions)
                .ToList();

            HeaderTitle.Text = $"{_symbol} — {name}";
            HeaderSubtitle.Text = $"Across {_holdings.Count} source{(_holdings.Count == 1 ? "" : "s")}";

            TotalValue.Text = CryptoFormatHelper.FormatValue(totalValue);
            QuantityLabel.Text = $"{totalQty:G} {_symbol}";
            PriceLabel.Text = CryptoFormatHelper.FormatPrice(price);

            ChangeLabel.Text = $"{(change >= 0 ? "+" : "")}{change:F2}%";
            ChangeLabel.TextColor = change >= 0 ? Color.FromArgb("#22C55E") : Color.FromArgb("#EF4444");

            var costBasis = allTxs.Where(t => (t.Type == "Receive" || t.Type == "Buy") && t.PriceAtTime > 0)
                .Sum(t => t.Quantity * t.PriceAtTime);
            var receivedQty = allTxs.Where(t => t.Type == "Receive" || t.Type == "Buy").Sum(t => t.Quantity);
            var avgCost = receivedQty > 0 ? costBasis / receivedQty : 0;
            var pnl = costBasis > 0 ? totalValue - costBasis : 0;
            var pnlPct = costBasis > 0 ? (pnl / costBasis) * 100 : 0;

            PnlLabel.Text = costBasis > 0
                ? $"{CryptoFormatHelper.FormatSignedValue(pnl)} ({pnlPct:+0.0;-0.0}%)"
                : "—";
            PnlLabel.TextColor = pnl >= 0 ? Color.FromArgb("#22C55E") : Color.FromArgb("#EF4444");

            AvgCostLabel.Text = avgCost > 0 ? CryptoFormatHelper.FormatPrice(avgCost) : "—";
            CostBasisLabel.Text = costBasis > 0 ? CryptoFormatHelper.FormatValue(costBasis) : "—";

            BuildSourcesBreakdown(price);
            BuildTxSummary(allTxs);
            BuildPnlChart(allTxs);
            BuildFilterButtons(allTxs);
            BuildTransactionRows(allTxs);
        }

        private void BuildSourcesBreakdown(decimal price)
        {
            SourcesList.Children.Clear();
            foreach (var h in _holdings.OrderByDescending(h => h.Quantity * price))
            {
                var srcValue = h.Quantity * price;
                var srcLabel = h.WalletLabel ?? $"{h.Symbol} Wallet";
                var txCount = h.Transactions?.Count ?? 0;

                var row = new Border
                {
                    BackgroundColor = Color.FromArgb("#1C2744"),
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
                            new VerticalStackLayout
                            {
                                Spacing = 2,
                                Children =
                                {
                                    new Label { Text = srcLabel, TextColor = Color.FromArgb("#F1F5F9"), FontSize = 13, FontAttributes = FontAttributes.Bold },
                                    new Label { Text = $"{h.Quantity:G} {_symbol} · {txCount} tx", TextColor = Color.FromArgb("#94A3B8"), FontSize = 11 },
                                }
                            },
                            BuildRightValue(srcValue),
                        }
                    }
                };
                SourcesList.Children.Add(row);
            }
        }

        private static View BuildRightValue(decimal value)
        {
            var lbl = new Label
            {
                Text = CryptoFormatHelper.FormatValue(value),
                TextColor = Color.FromArgb("#F1F5F9"),
                FontSize = 14,
                FontAttributes = FontAttributes.Bold,
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.End,
            };
            Grid.SetColumn(lbl, 1);
            return lbl;
        }

        private void BuildTxSummary(List<MockCryptoTransaction> allTxs)
        {
            TxCountLabel.Text = $"{allTxs.Count} transactions";

            var received = allTxs.Where(t => t.Type == "Receive" || t.Type == "Buy").Sum(t => t.Quantity);
            var sent = allTxs.Where(t => t.Type == "Send" || t.Type == "Sell").Sum(t => t.Quantity);
            var net = received - sent;
            var receivedVal = allTxs.Where(t => (t.Type == "Receive" || t.Type == "Buy") && t.PriceAtTime > 0).Sum(t => t.Quantity * t.PriceAtTime);
            var sentVal = allTxs.Where(t => (t.Type == "Send" || t.Type == "Sell") && t.PriceAtTime > 0).Sum(t => t.Quantity * t.PriceAtTime);

            TotalReceived.Text = $"+{received:G} {_symbol}";
            ReceivedValue.Text = receivedVal > 0 ? $"({CryptoFormatHelper.FormatValue(receivedVal)} at time)" : "";
            TotalSent.Text = $"-{sent:G} {_symbol}";
            SentValue.Text = sentVal > 0 ? $"({CryptoFormatHelper.FormatValue(sentVal)} at time)" : "";
            NetFlow.Text = $"{(net >= 0 ? "+" : "")}{net:G} {_symbol}";
            NetFlow.TextColor = net >= 0 ? Color.FromArgb("#22C55E") : Color.FromArgb("#EF4444");
        }

        private void BuildPnlChart(List<MockCryptoTransaction> allTxs)
        {
            var receiveTxs = allTxs
                .Where(t => (t.Type == "Receive" || t.Type == "Buy") && t.PriceAtTime > 0)
                .OrderBy(t => t.Date)
                .ToList();

            if (receiveTxs.Count == 0)
            {
                PnlChart.IsVisible = false;
                return;
            }

            var price = _holdings.FirstOrDefault(h => h.PriceGBP > 0)?.PriceGBP ?? 0;
            var dataPoints = new List<(DateTime Date, decimal PnL)>();
            decimal runningQty = 0, runningCost = 0;

            foreach (var tx in receiveTxs)
            {
                runningQty += tx.Quantity;
                runningCost += tx.Quantity * tx.PriceAtTime;
                var valueAtTime = runningQty * tx.PriceAtTime;
                dataPoints.Add((tx.Date, valueAtTime - runningCost));
            }

            dataPoints.Add((DateTime.Now, runningQty * price - runningCost));
            PnlChart.Drawable = new PnlChartDrawable(dataPoints, _culture);
        }

        private void BuildFilterButtons(List<MockCryptoTransaction> allTxs)
        {
            var hasBuy = allTxs.Any(t => t.Type == "Buy");
            var hasSell = allTxs.Any(t => t.Type == "Sell");
            var hasSwap = allTxs.Any(t => t.Type == "Swap");
            var hasWithdraw = allTxs.Any(t => t.Type == "Withdraw");
            var hasFees = allTxs.Any(t => t.Quantity < 0.01m);
            var filterList = new List<string> { "All", "Receive", "Send" };
            if (hasBuy) filterList.Add("Buy");
            if (hasSell) filterList.Add("Sell");
            if (hasSwap) filterList.Add("Swap");
            if (hasWithdraw) filterList.Add("Withdraw");
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
            var allTxs = _holdings.Where(h => h.Transactions != null).SelectMany(h => h.Transactions).ToList();
            var typeFilters = _activeFilters.Where(f => f != "No Fees").ToHashSet();
            if (typeFilters.Count > 0) allTxs = allTxs.Where(t => typeFilters.Contains(t.Type)).ToList();
            if (_activeFilters.Contains("No Fees")) allTxs = allTxs.Where(t => t.Quantity >= 0.01m).ToList();

            var price = _holdings.FirstOrDefault(h => h.PriceGBP > 0)?.PriceGBP ?? 0;
            var addressMap = TransactionHelper.BuildAddressMap();
            var txSourceMap = new Dictionary<MockCryptoTransaction, string>();
            foreach (var h in _holdings)
            {
                var src = h.WalletLabel ?? $"{h.Symbol} Wallet";
                if (h.Transactions != null)
                    foreach (var tx in h.Transactions)
                        txSourceMap[tx] = src;
            }

            var rows = allTxs.OrderByDescending(t => t.Date)
                .Select(t => TransactionHelper.ToReportRow(t, txSourceMap.GetValueOrDefault(t, "—"), price, addressMap))
                .ToList();

            var filters = _activeFilters.Count > 0 ? _activeFilters.ToList() : new List<string> { "All" };
            await PdfReportService.GenerateAndOpenAsync($"{_symbol} Combined", $"{_symbol} across {_holdings.Count} sources", filters, rows);
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

            var allTxs = _holdings
                .Where(h => h.Transactions != null)
                .SelectMany(h => h.Transactions)
                .ToList();

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
            new ColumnDefinition(new GridLength(50)),   // 2: type
            new ColumnDefinition(new GridLength(90)),   // 3: source
            new ColumnDefinition(new GridLength(90)),   // 4: from
            new ColumnDefinition(new GridLength(90)),   // 5: to
            new ColumnDefinition(GridLength.Star),      // 6: quantity
            new ColumnDefinition(new GridLength(70)),   // 7: price
            new ColumnDefinition(new GridLength(75)),   // 8: value
            new ColumnDefinition(new GridLength(90)),   // 9: P&L
            new ColumnDefinition(new GridLength(85)),   // 10: hash
        };

        private void BuildTransactionRows(List<MockCryptoTransaction> txs)
        {
            TransactionsList.Children.Clear();
            var price = _holdings.FirstOrDefault(h => h.PriceGBP > 0)?.PriceGBP ?? 0;

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

            var txSourceMap = new Dictionary<MockCryptoTransaction, string>();
            foreach (var h in _holdings)
            {
                var src = h.WalletLabel ?? $"{h.Symbol} Wallet";
                if (h.Transactions != null)
                    foreach (var tx in h.Transactions)
                        txSourceMap[tx] = src;
            }

            var header = new Grid { ColumnDefinitions = TxColumns, ColumnSpacing = 6, Padding = new Thickness(12, 0, 12, 4) };
            AddHeaderCell(header, "", 0);
            AddHeaderCell(header, "Date", 1);
            AddHeaderCell(header, "Type", 2);
            AddHeaderCell(header, "Source", 3);
            AddHeaderCell(header, "From", 4);
            AddHeaderCell(header, "To", 5);
            AddHeaderCell(header, "Quantity", 6);
            AddHeaderCell(header, "Price", 7);
            AddHeaderCell(header, "Value", 8);
            AddHeaderCell(header, "P&L", 9);
            AddHeaderCell(header, "Hash", 10);
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
                var valueNow = tx.Quantity * price;
                var gainLoss = tx.PriceAtTime > 0 ? valueNow - valueAtTime : 0;
                var glPct = valueAtTime > 0 ? (gainLoss / valueAtTime) * 100 : 0;
                var glColor = gainLoss >= 0 ? "#22C55E" : "#EF4444";

                txSourceMap.TryGetValue(tx, out var source);
                source ??= "—";

                var isLinked = linkedGroups.TryGetValue(tx, out var groupIdx);
                var bgColor = isLinked ? TransactionHelper.GetLinkBgColor(groupIdx) : "#131B2E";
                var linkColor = isLinked ? TransactionHelper.GetLinkColor(groupIdx) : null;

                var fromInfo = TransactionHelper.ResolveWithSource(tx.FromAddress, tx.Type, true, source, addressMap, source);
                var toInfo = TransactionHelper.ResolveWithSource(tx.ToAddress, tx.Type, false, source, addressMap, source);

                var fromCell = Cell(fromInfo.Display, fromInfo.Color, 4, 9);
                var toCell = Cell(toInfo.Display, toInfo.Color, 5, 9);

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
                            Cell(isLinked ? "🔗" : icon, isLinked ? linkColor : typeColor, 0, 14),
                            Cell(tx.Date.ToString("dd MMM yy\nHH:mm"), "#94A3B8", 1, 10),
                            Cell(typeLabel, typeColor, 2, 11, FontAttributes.Bold),
                            Cell(source, "#06B6D4", 3, 10),
                            fromCell,
                            toCell,
                            Cell($"{qtyPrefix}{tx.Quantity:G}", typeColor, 6, 12, FontAttributes.Bold),
                            Cell(tx.PriceAtTime > 0 ? CryptoFormatHelper.FormatPrice(tx.PriceAtTime) : "—", "#F1F5F9", 7, 11),
                            Cell(valueAtTime > 0 ? CryptoFormatHelper.FormatValue(valueAtTime) : "—", "#F1F5F9", 8, 11),
                            Cell(tx.PriceAtTime > 0
                                ? $"{CryptoFormatHelper.FormatSignedValue(gainLoss)}\n({glPct:+0.0;-0.0}%)"
                                : "—",
                                tx.PriceAtTime > 0 ? glColor : "#64748B", 9, 10, FontAttributes.Bold),
                            Cell(tx.Hash, "#475569", 10, 9),
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

        private static Label Cell(string text, string color, int col, int size, FontAttributes attrs = FontAttributes.None)
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

        private async void OnRefreshClicked(object sender, EventArgs e)
        {
            RefreshButton.Text = "⟳";
            RefreshButton.IsEnabled = false;
            try
            {
                var prices = await CryptoMarketService.GetBatchPricesAsync(new[] { _symbol });
                if (prices.TryGetValue(_symbol, out var pp) && pp.Price > 0)
                {
                    foreach (var h in _holdings)
                    {
                        h.PriceGBP = pp.Price;
                        h.PriceUSD = pp.Price * 1.27m;
                        h.Change24h = pp.Change24h;
                    }
                }
                PopulateData();
            }
            catch { }
            finally
            {
                RefreshButton.Text = "↻";
                RefreshButton.IsEnabled = true;
            }
        }

        private async void OnBackClicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }

        private async void OnHelpClicked(object sender, EventArgs e)
        {
            await DisplayAlert("Combined Asset",
                "This page combines your holdings of one coin across every wallet and exchange you have connected. It shows the total value, price, profit/loss and a breakdown of where it is held. The transaction list below can be filtered by type and exported to a PDF report. All figures are calculated from data stored locally on this PC.",
                "OK");
        }
    }
}
