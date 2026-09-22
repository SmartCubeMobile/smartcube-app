using SmartCubeMobile.MockData;
using SmartCubeMobile.Services;
using System.Globalization;

namespace SmartCubeMobile.Dashboard
{
    public partial class WalletDetailPage : ContentPage
    {
        private readonly MockCryptoHolding _holding;
        private readonly CultureInfo _culture;
        private readonly HashSet<string> _activeFilters = new();
        private readonly Dictionary<string, Border> _filterButtons = new();

        public WalletDetailPage(MockCryptoHolding holding, CultureInfo culture)
        {
            InitializeComponent();
            _holding = holding;
            _culture = culture;
            PopulateData();
        }

        private void PopulateData()
        {
            var h = _holding;
            var value = h.Quantity * h.PriceGBP;
            var txs = h.Transactions ?? new();

            HeaderTitle.Text = h.WalletLabel ?? h.Name;
            HeaderSubtitle.Text = h.WalletAddress ?? h.Symbol;

            TotalValue.Text = CryptoFormatHelper.FormatValue(value);
            QuantityLabel.Text = $"{h.Quantity:G} {h.Symbol}";
            PriceLabel.Text = CryptoFormatHelper.FormatPrice(h.PriceGBP);

            ChangeLabel.Text = $"{(h.Change24h >= 0 ? "+" : "")}{h.Change24h:F2}%";
            ChangeLabel.TextColor = h.Change24h >= 0 ? Color.FromArgb("#22C55E") : Color.FromArgb("#EF4444");

            var costBasis = txs.Where(t => (t.Type == "Receive" || t.Type == "Buy") && t.PriceAtTime > 0)
                .Sum(t => t.Quantity * t.PriceAtTime);
            var receivedQty = txs.Where(t => t.Type == "Receive" || t.Type == "Buy").Sum(t => t.Quantity);
            var avgCost = receivedQty > 0 ? costBasis / receivedQty : 0;
            var currentValue = receivedQty > 0 ? receivedQty * h.PriceGBP : value;
            var pnl = costBasis > 0 ? currentValue - costBasis : 0;
            var pnlPct = costBasis > 0 ? (pnl / costBasis) * 100 : 0;

            PnlLabel.Text = costBasis > 0
                ? $"{CryptoFormatHelper.FormatSignedValue(pnl)} ({pnlPct:+0.0;-0.0}%)"
                : "—";
            PnlLabel.TextColor = pnl >= 0 ? Color.FromArgb("#22C55E") : Color.FromArgb("#EF4444");

            AvgCostLabel.Text = avgCost > 0 ? CryptoFormatHelper.FormatPrice(avgCost) : "—";
            CostBasisLabel.Text = costBasis > 0 ? CryptoFormatHelper.FormatValue(costBasis) : "—";

            NetworkLabel.Text = h.Network ?? "";
            AddressLabel.Text = h.WalletAddress ?? "—";
            WalletLabelText.Text = h.WalletLabel ?? "—";

            TxCountLabel.Text = $"{txs.Count} transactions";

            var received = txs.Where(t => t.Type == "Receive" || t.Type == "Buy").Sum(t => t.Quantity);
            var sent = txs.Where(t => t.Type == "Send" || t.Type == "Sell").Sum(t => t.Quantity);
            var net = received - sent;
            var receivedValue = txs.Where(t => (t.Type == "Receive" || t.Type == "Buy") && t.PriceAtTime > 0).Sum(t => t.Quantity * t.PriceAtTime);
            var sentValue = txs.Where(t => (t.Type == "Send" || t.Type == "Sell") && t.PriceAtTime > 0).Sum(t => t.Quantity * t.PriceAtTime);

            TotalReceived.Text = $"+{received:G} {h.Symbol}";
            ReceivedValue.Text = receivedValue > 0 ? $"({CryptoFormatHelper.FormatValue(receivedValue)} at time)" : "";
            TotalSent.Text = $"-{sent:G} {h.Symbol}";
            SentValue.Text = sentValue > 0 ? $"({CryptoFormatHelper.FormatValue(sentValue)} at time)" : "";
            NetFlow.Text = $"{(net >= 0 ? "+" : "")}{net:G} {h.Symbol}";
            NetFlow.TextColor = net >= 0 ? Color.FromArgb("#22C55E") : Color.FromArgb("#EF4444");

            BuildPnlChart(txs);
            BuildFilterButtons();
            BuildTransactionRows(txs);
        }

        private void BuildFilterButtons()
        {
            var txs = _holding.Transactions ?? new();
            var hasBuy = txs.Any(t => t.Type == "Buy");
            var hasSell = txs.Any(t => t.Type == "Sell");
            var hasSwap = txs.Any(t => t.Type == "Swap");
            var hasWithdraw = txs.Any(t => t.Type == "Withdraw");
            var hasFees = txs.Any(t => t.Quantity < 0.01m);
            var filterList = new List<string> { "All", "Receive", "Send" };
            if (hasBuy) filterList.Add("Buy");
            if (hasSell) filterList.Add("Sell");
            if (hasSwap) filterList.Add("Swap");
            if (hasWithdraw) filterList.Add("Withdraw");
            if (hasFees) filterList.Add("No Fees");
            var filters = filterList.ToArray();
            FilterBar.Children.Clear();
            _filterButtons.Clear();

            foreach (var filter in filters)
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
            pdfBtn.GestureRecognizers.Add(new TapGestureRecognizer { Command = new Command(async () => await ExportPdf()) });
            FilterBar.Children.Add(pdfBtn);
        }

        private async Task ExportPdf()
        {
            var allTxs = _holding.Transactions?.ToList() ?? new();
            var typeFilters = _activeFilters.Where(f => f != "No Fees").ToHashSet();
            if (typeFilters.Count > 0) allTxs = allTxs.Where(t => typeFilters.Contains(t.Type)).ToList();
            if (_activeFilters.Contains("No Fees")) allTxs = allTxs.Where(t => t.Quantity >= 0.01m).ToList();

            var walletLabel = _holding.WalletLabel ?? $"{_holding.Symbol} Wallet";
            var addressMap = TransactionHelper.BuildAddressMap();
            var rows = allTxs.OrderByDescending(t => t.Date)
                .Select(t => TransactionHelper.ToReportRow(t, walletLabel, _holding.PriceGBP, addressMap))
                .ToList();

            var filters = _activeFilters.Count > 0 ? _activeFilters.ToList() : new List<string> { "All" };
            await PdfReportService.GenerateAndOpenAsync(walletLabel, $"Wallet: {walletLabel} ({_holding.Symbol})", filters, rows);
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

            var txs = _holding.Transactions ?? new();
            var typeFilters = _activeFilters.Where(f => f != "No Fees").ToHashSet();
            if (typeFilters.Count > 0)
                txs = txs.Where(t => typeFilters.Contains(t.Type)).ToList();
            if (_activeFilters.Contains("No Fees"))
                txs = txs.Where(t => t.Quantity >= 0.01m).ToList();

            BuildTransactionRows(txs);
        }

        private static readonly ColumnDefinitionCollection TxColumns = new()
        {
            new ColumnDefinition(new GridLength(24)),   // 0: icon
            new ColumnDefinition(new GridLength(80)),   // 1: date
            new ColumnDefinition(new GridLength(50)),   // 2: type
            new ColumnDefinition(new GridLength(90)),   // 3: from
            new ColumnDefinition(new GridLength(90)),   // 4: to
            new ColumnDefinition(GridLength.Star),      // 5: quantity
            new ColumnDefinition(new GridLength(70)),   // 6: price
            new ColumnDefinition(new GridLength(75)),   // 7: value
            new ColumnDefinition(new GridLength(90)),   // 8: P&L
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
            var walletLabel = _holding.WalletLabel;

            var header = new Grid { ColumnDefinitions = TxColumns, ColumnSpacing = 6, Padding = new Thickness(12, 0, 12, 4) };
            AddHeaderCell(header, "", 0);
            AddHeaderCell(header, "Date", 1);
            AddHeaderCell(header, "Type", 2);
            AddHeaderCell(header, "From", 3);
            AddHeaderCell(header, "To", 4);
            AddHeaderCell(header, "Quantity", 5);
            AddHeaderCell(header, "Price", 6);
            AddHeaderCell(header, "Value", 7);
            AddHeaderCell(header, "P&L", 8);
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
                var valueNow = tx.Quantity * _holding.PriceGBP;
                var gainLoss = tx.PriceAtTime > 0 ? valueNow - valueAtTime : 0;
                var glPct = valueAtTime > 0 ? (gainLoss / valueAtTime) * 100 : 0;
                var glColor = gainLoss >= 0 ? "#22C55E" : "#EF4444";

                var isLinked = linkedGroups.TryGetValue(tx, out var groupIdx);
                var bgColor = isLinked ? TransactionHelper.GetLinkBgColor(groupIdx) : "#131B2E";
                var linkColor = isLinked ? TransactionHelper.GetLinkColor(groupIdx) : null;

                var fromInfo = TransactionHelper.ResolveWithSource(tx.FromAddress, tx.Type, true, walletLabel, addressMap, walletLabel);
                var toInfo = TransactionHelper.ResolveWithSource(tx.ToAddress, tx.Type, false, walletLabel, addressMap, walletLabel);

                var fromCell = Cell(fromInfo.Display, fromInfo.Color, 3, 9);
                var toCell = Cell(toInfo.Display, toInfo.Color, 4, 9);

                if (fromInfo.CanClaim)
                {
                    var addr = fromInfo.FullAddress;
                    fromCell.TextDecorations = TextDecorations.Underline;
                    fromCell.GestureRecognizers.Add(new TapGestureRecognizer
                    {
                        Command = new Command(async () => await TransactionHelper.ClaimAddress(addr, this))
                    });
                }
                if (toInfo.CanClaim)
                {
                    var addr = toInfo.FullAddress;
                    toCell.TextDecorations = TextDecorations.Underline;
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
                            fromCell,
                            toCell,
                            Cell($"{qtyPrefix}{tx.Quantity:G}", typeColor, 5, 12, FontAttributes.Bold),
                            Cell(tx.PriceAtTime > 0 ? CryptoFormatHelper.FormatPrice(tx.PriceAtTime) : "—", "#F1F5F9", 6, 11),
                            Cell(valueAtTime > 0 ? CryptoFormatHelper.FormatValue(valueAtTime) : "—", "#F1F5F9", 7, 11),
                            Cell(tx.PriceAtTime > 0
                                ? $"{CryptoFormatHelper.FormatSignedValue(gainLoss)}\n({glPct:+0.0;-0.0}%)"
                                : "—",
                                tx.PriceAtTime > 0 ? glColor : "#64748B", 8, 10, FontAttributes.Bold),
                            Cell(tx.Hash, "#475569", 9, 9),
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

        private void BuildPnlChart(List<MockCryptoTransaction> txs)
        {
            var receiveTxs = txs
                .Where(t => (t.Type == "Receive" || t.Type == "Buy") && t.PriceAtTime > 0)
                .OrderBy(t => t.Date)
                .ToList();

            if (receiveTxs.Count == 0)
            {
                PnlChart.IsVisible = false;
                return;
            }

            var dataPoints = new List<(DateTime Date, decimal PnL)>();
            decimal runningQty = 0;
            decimal runningCost = 0;

            foreach (var tx in receiveTxs)
            {
                runningQty += tx.Quantity;
                runningCost += tx.Quantity * tx.PriceAtTime;
                var valueAtTime = runningQty * tx.PriceAtTime;
                var pnlAtTime = valueAtTime - runningCost;
                dataPoints.Add((tx.Date, pnlAtTime));
            }

            var currentPnl = (runningQty * _holding.PriceGBP) - runningCost;
            dataPoints.Add((DateTime.Now, currentPnl));

            PnlChart.Drawable = new PnlChartDrawable(dataPoints, _culture);
        }

        private async void OnRefreshClicked(object sender, EventArgs e)
        {
            RefreshButton.Text = "⟳";
            RefreshButton.IsEnabled = false;
            try
            {
                var prices = await CryptoMarketService.GetBatchPricesAsync(new[] { _holding.Symbol });
                if (prices.TryGetValue(_holding.Symbol, out var pp) && pp.Price > 0)
                {
                    _holding.PriceGBP = pp.Price;
                    _holding.PriceUSD = pp.Price * 1.27m;
                    _holding.Change24h = pp.Change24h;
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
    }

    public class PnlChartDrawable : IDrawable
    {
        private readonly List<(DateTime Date, decimal PnL)> _data;
        private readonly CultureInfo _culture;

        public PnlChartDrawable(List<(DateTime Date, decimal PnL)> data, CultureInfo culture)
        {
            _data = data;
            _culture = culture;
        }

        public void Draw(ICanvas canvas, RectF dirtyRect)
        {
            if (_data.Count < 2) return;

            var w = dirtyRect.Width;
            var h = dirtyRect.Height;
            float padLeft = 60, padRight = 16, padTop = 20, padBottom = 30;
            var chartW = w - padLeft - padRight;
            var chartH = h - padTop - padBottom;

            var minPnl = (float)_data.Min(d => d.PnL);
            var maxPnl = (float)_data.Max(d => d.PnL);
            var range = maxPnl - minPnl;
            if (range == 0) range = 1;

            var minTime = _data[0].Date.Ticks;
            var maxTime = _data[^1].Date.Ticks;
            var timeRange = maxTime - minTime;
            if (timeRange == 0) timeRange = 1;

            float MapX(DateTime date) => padLeft + (float)((date.Ticks - minTime) / (double)timeRange) * chartW;
            float MapY(decimal pnl) => padTop + chartH - ((float)pnl - minPnl) / range * chartH;

            canvas.StrokeColor = Color.FromArgb("#1E2D4A");
            canvas.StrokeSize = 1;
            for (int i = 0; i <= 4; i++)
            {
                var y = padTop + chartH * i / 4f;
                canvas.DrawLine(padLeft, y, w - padRight, y);
                var val = (decimal)(maxPnl - (maxPnl - minPnl) * i / 4f);
                canvas.FontColor = Color.FromArgb("#64748B");
                canvas.FontSize = 9;
                canvas.DrawString(val.ToString("C0", _culture), 0, y - 6, padLeft - 4, 12, HorizontalAlignment.Right, VerticalAlignment.Center);
            }

            var zeroY = MapY(0);
            if (zeroY >= padTop && zeroY <= padTop + chartH)
            {
                canvas.StrokeColor = Color.FromArgb("#475569");
                canvas.StrokeSize = 1;
                canvas.StrokeDashPattern = new float[] { 4, 4 };
                canvas.DrawLine(padLeft, zeroY, w - padRight, zeroY);
                canvas.StrokeDashPattern = null;
            }

            var path = new PathF();
            var fillPath = new PathF();
            for (int i = 0; i < _data.Count; i++)
            {
                var x = MapX(_data[i].Date);
                var y = MapY(_data[i].PnL);
                if (i == 0)
                {
                    path.MoveTo(x, y);
                    fillPath.MoveTo(x, y);
                }
                else
                {
                    path.LineTo(x, y);
                    fillPath.LineTo(x, y);
                }
            }

            var lastX = MapX(_data[^1].Date);
            var firstX = MapX(_data[0].Date);
            fillPath.LineTo(lastX, padTop + chartH);
            fillPath.LineTo(firstX, padTop + chartH);
            fillPath.Close();

            var finalPnl = _data[^1].PnL;
            var lineColor = finalPnl >= 0 ? "#22C55E" : "#EF4444";
            var fillColor = finalPnl >= 0 ? "#22C55E20" : "#EF444420";

            canvas.FillColor = Color.FromArgb(fillColor);
            canvas.FillPath(fillPath);

            canvas.StrokeColor = Color.FromArgb(lineColor);
            canvas.StrokeSize = 2;
            canvas.DrawPath(path);

            var labelCount = Math.Min(_data.Count, 5);
            var step = Math.Max(1, _data.Count / labelCount);
            canvas.FontColor = Color.FromArgb("#64748B");
            canvas.FontSize = 9;
            for (int i = 0; i < _data.Count; i += step)
            {
                var x = MapX(_data[i].Date);
                canvas.DrawString(_data[i].Date.ToString("MMM yy"), x - 20, padTop + chartH + 4, 40, 20,
                    HorizontalAlignment.Center, VerticalAlignment.Top);
            }
        }
    }
}
