using SmartCubeMobile.Dashboard;
using SmartCubeMobile.MockData;
using SmartCubeMobile.Services;
using System.Globalization;
using System.Text.RegularExpressions;

namespace SmartCubeMobile.Dashboard.Faces
{
    public partial class UtilityFace : ContentView, IAnimatedFace
    {
        private readonly List<ParsedBill> _parsedBills = new();
        private List<AuditFinding> _lastAuditFindings;

        public UtilityFace()
        {
            InitializeComponent();
            LoadData();
#if WINDOWS
            RegisterNativeDrop();
#endif
        }

        private async void OnHelpClicked(object sender, EventArgs e)
        {
            var page = Application.Current?.Windows.FirstOrDefault()?.Page;
            if (page != null)
                await page.DisplayAlert("Utilities",
                    "This section tracks your energy, water and broadband suppliers and bills. Use Compare to shop energy tariffs for your postcode, and + Add to add a supplier. Drop PDF bills into the box on the right and SmartCube reads the amounts, dates and usage automatically, or click Run Audit to check them for pricing errors and savings. All bill and supplier data is stored locally on this PC; tariff comparisons come from EnergyLinx via the SmartCube server.",
                    "OK");
        }

#if WINDOWS
        private void RegisterNativeDrop()
        {
            DropZone.HandlerChanged += (s, e) =>
            {
                if (DropZone.Handler?.PlatformView is Microsoft.UI.Xaml.UIElement nativeView)
                {
                    nativeView.AllowDrop = true;
                    nativeView.DragOver += (ds, de) =>
                    {
                        de.AcceptedOperation = Windows.ApplicationModel.DataTransfer.DataPackageOperation.Copy;
                        de.DragUIOverride.Caption = "Drop to import";
                        de.DragUIOverride.IsCaptionVisible = true;
                        de.DragUIOverride.IsGlyphVisible = true;

                        MainThread.BeginInvokeOnMainThread(() =>
                        {
                            DropZone.BackgroundColor = Color.FromArgb("#1A1E3A");
                            DropZone.Stroke = Color.FromArgb("#C084FC");
                            DropIcon.Text = "📥";
                            DropTitle.Text = "Drop to import";
                            DropTitle.TextColor = Color.FromArgb("#C084FC");
                        });
                    };

                    nativeView.DragLeave += (ds, de) =>
                    {
                        MainThread.BeginInvokeOnMainThread(ResetDropZone);
                    };

                    nativeView.Drop += async (ds, de) =>
                    {
                        if (de.DataView.Contains(Windows.ApplicationModel.DataTransfer.StandardDataFormats.StorageItems))
                        {
                            var items = await de.DataView.GetStorageItemsAsync();
                            var pdfPaths = new List<string>();
                            foreach (var item in items)
                            {
                                if (item is Windows.Storage.StorageFile file &&
                                    file.FileType.Equals(".pdf", StringComparison.OrdinalIgnoreCase))
                                {
                                    pdfPaths.Add(file.Path);
                                }
                            }

                            MainThread.BeginInvokeOnMainThread(() => ProcessDroppedFiles(pdfPaths));
                        }
                        else
                        {
                            MainThread.BeginInvokeOnMainThread(async () =>
                            {
                                ResetDropZone();
                                await Application.Current.Windows[0].Page.DisplayAlert(
                                    "No PDFs", "No PDF files found. Please drag PDF bills.", "OK");
                            });
                        }
                    };
                }
            };
        }
#endif

        private async void ProcessDroppedFiles(List<string> pdfPaths)
        {
            ResetDropZone();

            if (pdfPaths.Count == 0)
            {
                await Application.Current.Windows[0].Page.DisplayAlert(
                    "No PDFs", "No PDF files were found. Please drag PDF bills.", "OK");
                return;
            }

            DropIcon.Text = "⏳";
            DropTitle.Text = $"Parsing {pdfPaths.Count} bill(s)...";
            DropTitle.TextColor = Color.FromArgb("#F59E0B");

            _parsedBills.Clear();
            int failed = 0;

            await Task.Run(() =>
            {
                foreach (var path in pdfPaths)
                {
                    try
                    {
                        var parsed = BillPdfParser.ParseFromFile(path);
                        if (parsed != null)
                            _parsedBills.Add(parsed);
                        else
                            failed++;
                    }
                    catch
                    {
                        failed++;
                    }
                }
            });

            if (_parsedBills.Count == 0)
            {
                ResetDropZone();
                await Application.Current.Windows[0].Page.DisplayAlert(
                    "Parse Failed", "Could not extract data from the dropped PDFs. They may be image-based or encrypted.", "OK");
                return;
            }

            ShowParsedResults(failed);
            ResetDropZone();
        }

        private static List<MockUtilityBill> FilterByProperty(List<MockUtilityBill> bills)
        {
            var prop = MockDataService.GetProperty();
            if (string.IsNullOrWhiteSpace(prop.Postcode)) return bills;
            var pc = prop.Postcode.Replace(" ", "").ToUpper();
            return bills.Where(b =>
            {
                if (string.IsNullOrEmpty(b.Address)) return true;
                return b.Address.Replace(" ", "").ToUpper().Contains(pc);
            }).ToList();
        }

        private void LoadData()
        {
            var bills = FilterByProperty(MockDataService.GetUtilityBills());
            var readings = MockDataService.GetMeterReadings();
            var suppliers = MockDataService.GetSuppliers();
            var culture = new CultureInfo("en-GB");

            BuildSupplierList(suppliers, culture);
            LoadReadings(readings, bills);
            LoadCurrentCosts(bills, culture);
            LoadBillHistory(bills, culture);

            AuditCard.IsVisible = true;
            var pdfBillCount = bills.Count(b => b.IsFromPdf);
            AuditSubtitle.Text = pdfBillCount > 0
                ? $"{pdfBillCount} uploaded bill(s) available to audit"
                : "Drop your PDF bills below to get started";
        }

        private void BuildSupplierList(List<MockSupplier> suppliers, CultureInfo culture)
        {
            SupplierList.Children.Clear();
            if (suppliers.Count == 0)
            {
                SupplierList.Children.Add(new Label
                {
                    Text = "No suppliers added yet. Tap '+ Add' to add your energy, water or broadband supplier.",
                    TextColor = Color.FromArgb("#64748B"),
                    FontSize = 12,
                    HorizontalTextAlignment = TextAlignment.Center,
                    Margin = new Thickness(0, 12),
                });
                return;
            }
            var hints = new Dictionary<string, Label>(StringComparer.OrdinalIgnoreCase);
            foreach (var s in suppliers)
            {
                var info = CreateSupplierInfo(s);
                var isEnergy = IsEnergy(s.Type);
                if (isEnergy)
                {
                    var hint = new Label { Text = "Tap to see this supplier's tariffs", TextColor = Color.FromArgb("#22C55E"), FontSize = 10 };
                    info.Children.Add(hint);
                    if (!string.IsNullOrEmpty(s.Name)) hints[s.Name] = hint;
                }

                var card = new Border
                {
                    BackgroundColor = Color.FromArgb("#1C2744"),
                    StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 10 },
                    Stroke = Colors.Transparent,
                    Padding = new Thickness(12, 10),
                    Content = new Grid
                    {
                        ColumnDefinitions = new ColumnDefinitionCollection
                        {
                            new ColumnDefinition(new GridLength(28)),
                            new ColumnDefinition(GridLength.Star),
                            new ColumnDefinition(GridLength.Auto),
                            new ColumnDefinition(GridLength.Auto),
                        },
                        ColumnSpacing = 10,
                        Children =
                        {
                            CreateSupplierBadge(s.Name),
                            info,
                            CreateSupplierCost(s, culture),
                        }
                    }
                };
                var supplier = s;
                ToolTipProperties.SetText(card, isEnergy
                    ? $"View tariffs for {s.Name}."
                    : $"Switch supplier for {s.Name}.");

                var removeBtn = new Button
                {
                    Text = "✕",
                    FontSize = 12,
                    TextColor = Color.FromArgb("#94A3B8"),
                    BackgroundColor = Color.FromArgb("#0D1322"),
                    CornerRadius = 8,
                    Padding = new Thickness(0),
                    WidthRequest = 28, HeightRequest = 28,
                    VerticalOptions = LayoutOptions.Center,
                };
                ToolTipProperties.SetText(removeBtn, $"Remove {s.Name}");
                removeBtn.Clicked += async (_, _) => await RemoveSupplier(supplier);
                Grid.SetColumn(removeBtn, 3);
                ((Grid)card.Content).Children.Add(removeBtn);
                card.GestureRecognizers.Add(new TapGestureRecognizer
                {
                    Command = new Command(async () =>
                    {
                        await card.ScaleTo(0.97, 80, Easing.CubicOut);
                        await card.ScaleTo(1.0, 80, Easing.CubicOut);
                        if (IsEnergy(supplier.Type))
                            await OpenSupplierTariffs(supplier);
                        else
                            await OpenSwitchSupplier(supplier);
                    })
                });
                SupplierList.Children.Add(card);
            }

            if (hints.Count > 0)
                _ = AnnotateSupplierCards(hints);
        }

        private async Task RemoveSupplier(MockSupplier supplier)
        {
            var host = Application.Current?.Windows.FirstOrDefault()?.Page;
            if (host == null) return;
            var billCount = MockDataService.CountBillsFor(supplier);
            var billsNote = billCount == 0
                ? ""
                : $"\n\n{billCount} bill{(billCount == 1 ? "" : "s")} from {supplier.Name} will be removed from your bill history too.";
            var ok = await host.DisplayAlert("Remove supplier",
                $"Remove {supplier.Name} ({supplier.Type}) from your suppliers?{billsNote}",
                "Remove", "Cancel");
            if (!ok) return;
            MockDataService.RemoveSupplier(supplier);
            RefreshData();
            RunAudit();
        }

        private static bool IsEnergy(string type) =>
            string.Equals(type, "Electricity", StringComparison.OrdinalIgnoreCase) ||
            string.Equals(type, "Gas", StringComparison.OrdinalIgnoreCase);

        private async Task OpenSupplierTariffs(MockSupplier supplier)
        {
            try
            {
                var nav = Application.Current?.Windows.FirstOrDefault()?.Page?.Navigation ?? Navigation;
                await nav.PushAsync(new TariffComparePage(supplier.Name));
            }
            catch (Exception ex)
            {
                var host = Application.Current?.Windows.FirstOrDefault()?.Page;
                if (host != null) await host.DisplayAlert("Tariffs", ex.Message, "OK");
            }
        }

        // Fills in "N tariffs · save up to £X/yr" on each energy supplier card once the server answers.
        private async Task AnnotateSupplierCards(Dictionary<string, Label> hints)
        {
            try
            {
                var (postcode, _) = TariffService.ResolvePostcode();
                if (string.IsNullOrEmpty(postcode)) return;
                var (elec, gas) = TariffService.EstimateAnnualKwh();
                var spend = TariffService.EstimateAnnualSpend();
                var fuel = gas > 0 || TariffService.UserHasGas() ? "dual" : "elec";
                var r = await TariffService.GetTariffs(postcode, elec, gas, fuel);
                if (!r.Ok || !r.Available) return;

                var culture = new CultureInfo("en-GB");
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    foreach (var (name, label) in hints)
                    {
                        var mine = r.Tariffs.Where(t => SupplierMatches(name, t.SupplierName)).OrderBy(t => t.CostForYou).ToList();
                        if (mine.Count == 0) { label.Text = "No tariffs listed on EnergyLinx · tap to compare all"; label.TextColor = Color.FromArgb("#64748B"); continue; }
                        var best = mine[0].CostForYou;
                        var save = spend.HasValue ? spend.Value - best : 0;
                        label.Text = $"{mine.Count} tariff{(mine.Count == 1 ? "" : "s")} available"
                            + (save >= 1 ? $" · save up to {save.ToString("C0", culture)}/yr" : "")
                            + " · tap to view";
                    }
                });
            }
            catch { }
        }

        private static bool SupplierMatches(string a, string b)
        {
            if (string.IsNullOrEmpty(a) || string.IsNullOrEmpty(b)) return false;
            a = a.ToLowerInvariant().Replace(" energy", "").Replace(".", "").Trim();
            b = b.ToLowerInvariant().Replace(" energy", "").Replace(".", "").Trim();
            return a == b || a.Contains(b) || b.Contains(a);
        }

        private async void OnAddSupplierClicked(object sender, EventArgs e)
        {
            var flow = new SupplierLoginFlow();
            flow.SupplierAdded += () =>
            {
                MainThread.BeginInvokeOnMainThread(RefreshData);
            };
            await Navigation.PushAsync(flow);
        }

        private async void OnCompareTariffsClicked(object sender, EventArgs e)
        {
            try
            {
                var nav = Application.Current?.Windows.FirstOrDefault()?.Page?.Navigation ?? Navigation;
                await nav.PushAsync(new TariffComparePage());
            }
            catch (Exception ex)
            {
                var host = Application.Current?.Windows.FirstOrDefault()?.Page;
                if (host != null)
                    await host.DisplayAlert("Compare Tariffs", "Could not open the tariff comparison:\n" + ex.Message, "OK");
            }
        }

        private async Task OpenSwitchSupplier(MockSupplier supplier)
        {
            var flow = new SwitchSupplierFlow(supplier);
            flow.SupplierChanged += () =>
            {
                MainThread.BeginInvokeOnMainThread(RefreshData);
            };
            await Navigation.PushAsync(flow);
        }

        private void RefreshData()
        {
            var bills = FilterByProperty(MockDataService.GetUtilityBills());
            var readings = MockDataService.GetMeterReadings();
            var suppliers = MockDataService.GetSuppliers();
            var culture = new CultureInfo("en-GB");

            BuildSupplierList(suppliers, culture);
            LoadReadings(readings, bills);
            LoadCurrentCosts(bills, culture);
            LoadBillHistory(bills, culture);
        }

        private static View CreateSupplierBadge(string supplierName)
        {
            var (color, abbrev) = SupplierBranding.Get(supplierName);
            var brandColor = Color.FromArgb(color);

            var badge = new Border
            {
                WidthRequest = 28,
                HeightRequest = 28,
                BackgroundColor = brandColor,
                StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 6 },
                Stroke = Colors.Transparent,
                VerticalOptions = LayoutOptions.Center,
                Content = new Label
                {
                    Text = abbrev,
                    TextColor = Colors.White,
                    FontSize = abbrev.Length > 2 ? 8 : 10,
                    FontAttributes = FontAttributes.Bold,
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center,
                    HorizontalTextAlignment = TextAlignment.Center,
                    VerticalTextAlignment = TextAlignment.Center,
                }
            };
            Grid.SetColumn(badge, 0);
            return badge;
        }

        private static VerticalStackLayout CreateSupplierInfo(MockSupplier s)
        {
            var stack = new VerticalStackLayout
            {
                Spacing = 1,
                VerticalOptions = LayoutOptions.Center,
                Children =
                {
                    new Label { Text = $"{s.Name} — {s.Type}", TextColor = Color.FromArgb("#F1F5F9"), FontSize = 13, FontAttributes = FontAttributes.Bold },
                    new Label { Text = s.Tariff, TextColor = Color.FromArgb("#3B82F6"), FontSize = 11 },
                    new Label { Text = s.TariffDetail, TextColor = Color.FromArgb("#64748B"), FontSize = 10 },
                }
            };
            Grid.SetColumn(stack, 1);
            return stack;
        }

        private static Label CreateSupplierCost(MockSupplier s, CultureInfo culture)
        {
            var lbl = new Label
            {
                Text = $"{s.MonthlyCost.ToString("C", culture)}/mo",
                TextColor = Color.FromArgb("#F59E0B"),
                FontSize = 12,
                FontAttributes = FontAttributes.Bold,
                VerticalOptions = LayoutOptions.Center,
            };
            Grid.SetColumn(lbl, 2);
            return lbl;
        }

        private void LoadReadings(List<MockMeterReading> readings, List<MockUtilityBill> bills = null)
        {
            var latestElec = readings.Where(r => r.MeterType == "Electricity").OrderByDescending(r => r.ReadingDate).FirstOrDefault();
            var latestGas = readings.Where(r => r.MeterType == "Gas").OrderByDescending(r => r.ReadingDate).FirstOrDefault();

            var elecBill = bills?.Where(b => b.FuelType == "Electricity" && b.MeterReadingEnd > 0)
                .OrderByDescending(b => b.BillDate).FirstOrDefault();
            var gasBill = bills?.Where(b => b.FuelType == "Gas" && b.MeterReadingEnd > 0)
                .OrderByDescending(b => b.BillDate).FirstOrDefault();
            var waterBill = bills?.Where(b => b.FuelType == "Water" && b.MeterReadingEnd > 0)
                .OrderByDescending(b => b.BillDate).FirstOrDefault();

            if (latestElec != null)
            {
                ElecReading.Text = $"{latestElec.Reading:N0} kWh";
                ElecUnits.Text = $"{latestElec.UnitsUsed:N0} kWh used";
            }
            else if (elecBill != null)
            {
                ElecReading.Text = $"{elecBill.MeterReadingEnd:N0} kWh";
                ElecUnits.Text = $"{elecBill.UnitsUsed:N0} kWh used";
            }
            else
            {
                ElecReading.Text = "—";
                ElecUnits.Text = "No data";
            }

            if (latestGas != null)
            {
                GasReading.Text = $"{latestGas.Reading:N0} m³";
                GasUnits.Text = $"{latestGas.UnitsUsed:N0} kWh used";
            }
            else if (gasBill != null)
            {
                GasReading.Text = $"{gasBill.MeterReadingEnd:N0} m³";
                GasUnits.Text = $"{gasBill.UnitsUsed:N0} kWh used";
            }
            else
            {
                GasReading.Text = "—";
                GasUnits.Text = "No data";
            }

            if (waterBill != null)
            {
                WaterReading.Text = $"{waterBill.MeterReadingEnd:N0} m³";
                WaterReading.TextColor = (Color)Application.Current.Resources["TextPrimary"];
                WaterReading.FontAttributes = FontAttributes.Bold;
                var totalUsage = waterBill.MeterReadingEnd - waterBill.MeterReadingStart;
                WaterUnits.Text = totalUsage > 0 ? $"{totalUsage:N0} m³ used" : $"{waterBill.UnitsUsed:N0} m³ used";
                WaterUnits.TextColor = (Color)Application.Current.Resources["Warning"];
            }
            else
            {
                WaterReading.Text = "—";
                WaterUnits.Text = "No data";
            }

            var hasAny = latestElec != null || latestGas != null || elecBill != null || gasBill != null || waterBill != null;
            if (hasAny)
            {
                ReadingSourceLabel.Text = $"Source: PDF Bill";
            }
            else
            {
                ReadingSourceLabel.Text = "Upload bills to populate meter readings";
            }
        }

        private void LoadCurrentCosts(List<MockUtilityBill> bills, CultureInfo culture)
        {
            var latestElec = bills.Where(b => b.FuelType == "Electricity").OrderByDescending(b => b.BillDate).FirstOrDefault();
            var latestGas = bills.Where(b => b.FuelType == "Gas").OrderByDescending(b => b.BillDate).FirstOrDefault();
            var latestWater = bills.Where(b => b.FuelType == "Water").OrderByDescending(b => b.BillDate).FirstOrDefault();

            CostsHeading.Text = "Latest Bills";

            ElecCost.Text = latestElec != null ? $"{latestElec.Amount.ToString("C", culture)}  ({latestElec.BillDate:MMM})" : "—";
            GasCost.Text = latestGas != null ? $"{latestGas.Amount.ToString("C", culture)}  ({latestGas.BillDate:MMM})" : "—";
            WaterCost.Text = latestWater != null ? $"{latestWater.Amount.ToString("C", culture)}  ({latestWater.BillDate:MMM})" : "—";

            var energyUnits = (latestElec?.UnitsUsed ?? 0) + (latestGas?.UnitsUsed ?? 0);
            var waterUnits = latestWater?.UnitsUsed ?? 0;
            if (energyUnits > 0 && waterUnits > 0)
                UnitsUsed.Text = $"{energyUnits:N0} kWh · {waterUnits:N1} m³";
            else if (energyUnits > 0)
                UnitsUsed.Text = $"{energyUnits:N0} kWh";
            else if (waterUnits > 0)
                UnitsUsed.Text = $"{waterUnits:N1} m³";
            else
                UnitsUsed.Text = "—";

            var total = (latestElec?.Amount ?? 0) + (latestGas?.Amount ?? 0) + (latestWater?.Amount ?? 0);
            TotalCost.Text = total > 0 ? total.ToString("C", culture) : "—";

            CheckBillsUpToDate(bills);
        }

        private void CheckBillsUpToDate(List<MockUtilityBill> bills)
        {
            if (bills.Count == 0)
            {
                BillsStatusCard.IsVisible = true;
                BillsStatusLabel.Text = "No bills uploaded yet. Drop your PDF bills in the audit section to get started.";
                return;
            }

            var now = DateTime.Now;
            var currentMonth = new DateTime(now.Year, now.Month, 1);
            var fuelTypes = bills.Select(b => $"{b.Supplier}|{b.FuelType}").Distinct().ToList();
            var missing = new List<string>();

            foreach (var key in fuelTypes)
            {
                var parts = key.Split('|');
                var supplier = parts[0];
                var fuel = parts[1];
                var group = bills
                    .Where(b => b.Supplier == supplier && b.FuelType == fuel)
                    .ToList();

                var earliest = new DateTime(group.Min(b => b.BillDate).Year, group.Min(b => b.BillDate).Month, 1);
                var latest = new DateTime(group.Max(b => b.BillDate).Year, group.Max(b => b.BillDate).Month, 1);
                var coveredMonths = group
                    .Select(b => new DateTime(b.BillDate.Year, b.BillDate.Month, 1))
                    .Distinct()
                    .ToHashSet();

                var missingMonths = new List<string>();
                var check = earliest.AddMonths(1);
                while (check <= latest)
                {
                    if (!coveredMonths.Contains(check))
                        missingMonths.Add(check.ToString("MMM yyyy"));
                    check = check.AddMonths(1);
                }

                if (missingMonths.Count > 0)
                    missing.Add($"{supplier} {fuel}: {string.Join(", ", missingMonths)}");
            }

            if (missing.Count > 0)
            {
                BillsStatusCard.IsVisible = true;
                BillsStatusLabel.Text = $"Bills not up to date:\n{string.Join("\n", missing)}";
            }
            else
            {
                BillsStatusCard.IsVisible = false;
            }
        }

        private void LoadBillHistory(List<MockUtilityBill> bills, CultureInfo culture)
        {
            var ordered = bills.OrderByDescending(b => b.BillDate).ThenBy(b => b.FuelType).ToList();

            var items = new List<BillDisplayItem>();
            foreach (var bill in ordered)
            {
                var prevBill = ordered
                    .Where(b => b.Supplier == bill.Supplier && b.FuelType == bill.FuelType && b.BillDate < bill.BillDate)
                    .OrderByDescending(b => b.BillDate)
                    .FirstOrDefault();

                string changeText = "";
                Color changeColour = Color.FromArgb("#64748B");

                if (prevBill != null)
                {
                    var diff = bill.Amount - prevBill.Amount;
                    if (diff > 0)
                    {
                        changeText = $"+{diff.ToString("C", culture)}";
                        changeColour = Color.FromArgb("#EF4444");
                    }
                    else if (diff < 0)
                    {
                        changeText = $"-{Math.Abs(diff).ToString("C", culture)}";
                        changeColour = Color.FromArgb("#22C55E");
                    }
                    else
                    {
                        changeText = "—";
                    }
                }

                var fuelColour = bill.FuelType switch
                {
                    "Electricity" => "#F59E0B",
                    "Gas" => "#06B6D4",
                    "Water" => "#3B82F6",
                    "Broadband" => "#8B5CF6",
                    "Mobile" => "#22C55E",
                    _ => "#94A3B8",
                };

                var unitsText = bill.UnitsUsed > 0 ? $"{bill.UnitsUsed:G} {bill.UoM}" : "";

                var postcode = "";
                if (!string.IsNullOrEmpty(bill.Address))
                {
                    var pcMatch = Regex.Match(bill.Address, @"[A-Z]{1,2}\d[A-Z\d]?\s*\d[A-Z]{2}", RegexOptions.IgnoreCase);
                    postcode = pcMatch.Success ? pcMatch.Value.ToUpper() : "";
                }

                var (brandColor, brandAbbrev) = SupplierBranding.Get(bill.Supplier);

                var meterText = "";
                if (bill.MeterReadingEnd > 0)
                    meterText = $"{bill.MeterReadingEnd:G}";
                else if (bill.MeterReadingStart > 0)
                    meterText = $"{bill.MeterReadingStart:G}";

                var allBills = MockDataService.GetUtilityBills();
                var billIndex = allBills.IndexOf(bill);

                items.Add(new BillDisplayItem
                {
                    Period = FormatPeriod(bill.Period, bill.BillDate),
                    FuelType = $"{bill.Supplier} — {bill.FuelType}",
                    FuelColour = Color.FromArgb(fuelColour),
                    Postcode = postcode,
                    MeterReadingFormatted = meterText,
                    UnitsFormatted = unitsText,
                    AmountFormatted = bill.Amount.ToString("C", culture),
                    ChangeFormatted = changeText,
                    ChangeColour = changeColour,
                    SupplierAbbrev = brandAbbrev,
                    SupplierBrandColor = Color.FromArgb(brandColor),
                    AbbrevFontSize = brandAbbrev.Length > 2 ? 6 : 7,
                    BillIndex = billIndex,
                });
            }

            BillsList.ItemsSource = items;
        }

        private async void OnBillSelected(object sender, SelectionChangedEventArgs e)
        {
            if (e.CurrentSelection.FirstOrDefault() is not BillDisplayItem item) return;
            BillsList.SelectedItem = null;

            var action = await Application.Current.Windows[0].Page.DisplayActionSheet(
                $"{item.FuelType} — {item.Period}", "Cancel", "Remove Bill");
            if (action != "Remove Bill") return;

            var confirm = await Application.Current.Windows[0].Page.DisplayAlert(
                "Remove Bill",
                $"Remove {item.FuelType} ({item.AmountFormatted}) from {item.Period}?",
                "Remove", "Cancel");
            if (!confirm) return;

            MockDataService.RemoveBill(item.BillIndex);
            RefreshData();
        }

        private void ResetDropZone()
        {
            DropZone.BackgroundColor = Color.FromArgb("#0D1322");
            DropZone.Stroke = Color.FromArgb("#2A3F6B");
            DropIcon.Text = "📄";
            DropTitle.Text = "Drop bill PDFs here";
            DropTitle.TextColor = Color.FromArgb("#F1F5F9");
        }

        #region Parsed Results

        private void ShowParsedResults(int failedCount)
        {
            ParsedBillsList.Children.Clear();
            var culture = new CultureInfo("en-GB");

            var title = $"{_parsedBills.Count} bill(s) parsed";
            if (failedCount > 0)
                title += $" ({failedCount} failed)";
            ParsedResultsTitle.Text = title;

            foreach (var parsed in _parsedBills)
            {
                var fuelColor = parsed.FuelType switch
                {
                    "Electricity" => "#F59E0B",
                    "Gas" => "#06B6D4",
                    "Dual Fuel" => "#8B5CF6",
                    "Water" => "#3B82F6",
                    _ => "#94A3B8",
                };

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
                            new ColumnDefinition(GridLength.Auto),
                            new ColumnDefinition(GridLength.Star),
                            new ColumnDefinition(GridLength.Auto),
                            new ColumnDefinition(GridLength.Auto),
                        },
                        ColumnSpacing = 10,
                        Children =
                        {
                            BuildCol(0, CreateParsedBillBadge(parsed.Supplier)),
                            BuildCol(1, new VerticalStackLayout
                            {
                                Spacing = 1,
                                Children =
                                {
                                    new Label { Text = parsed.Supplier, TextColor = Color.FromArgb("#F1F5F9"), FontSize = 13, FontAttributes = FontAttributes.Bold },
                                    new Label { Text = $"{parsed.FuelType} · {parsed.Period}", TextColor = Color.FromArgb(fuelColor), FontSize = 11 },
                                    new Label { Text = parsed.UnitsUsed > 0 ? $"{parsed.UnitsUsed:N0} {parsed.UoM}" : "usage not detected", TextColor = Color.FromArgb("#64748B"), FontSize = 10 },
                                }
                            }),
                            BuildCol(2, new Label
                            {
                                Text = parsed.Amount > 0 ? parsed.Amount.ToString("C", culture) : "£?",
                                TextColor = parsed.Amount > 0 ? Color.FromArgb("#F59E0B") : Color.FromArgb("#EF4444"),
                                FontSize = 15, FontAttributes = FontAttributes.Bold,
                                VerticalOptions = LayoutOptions.Center,
                            }),
                            BuildCol(3, new Label
                            {
                                Text = parsed.BillDate.ToString("dd MMM yy"),
                                TextColor = Color.FromArgb("#94A3B8"),
                                FontSize = 11, VerticalOptions = LayoutOptions.Center,
                            }),
                        }
                    }
                };

                ParsedBillsList.Children.Add(row);
            }

            ParsedResultsCard.IsVisible = true;
        }

        private static string FormatPeriod(string period, DateTime billDate)
        {
            if (DateTime.TryParse(period, out var parsed))
                return parsed.ToString("MMM yyyy");
            if (billDate != default && (string.IsNullOrEmpty(period) || period.Length > 10))
                return billDate.ToString("MMM yyyy");
            if (!string.IsNullOrEmpty(period))
            {
                foreach (var full in new[] { "January", "February", "March", "April", "May", "June",
                    "July", "August", "September", "October", "November", "December" })
                {
                    if (period.Contains(full, StringComparison.OrdinalIgnoreCase))
                        return period.Replace(full, full[..3], StringComparison.OrdinalIgnoreCase);
                }
            }
            return !string.IsNullOrEmpty(period) ? period : billDate.ToString("MMM yyyy");
        }

        private static View BuildCol(int col, View view)
        {
            Grid.SetColumn(view, col);
            return view;
        }

        private static View CreateParsedBillBadge(string supplierName)
        {
            var (color, abbrev) = SupplierBranding.Get(supplierName);
            return new Border
            {
                WidthRequest = 28,
                HeightRequest = 28,
                BackgroundColor = Color.FromArgb(color),
                StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 6 },
                Stroke = Colors.Transparent,
                VerticalOptions = LayoutOptions.Center,
                Content = new Label
                {
                    Text = abbrev,
                    TextColor = Colors.White,
                    FontSize = abbrev.Length > 2 ? 8 : 10,
                    FontAttributes = FontAttributes.Bold,
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center,
                    HorizontalTextAlignment = TextAlignment.Center,
                    VerticalTextAlignment = TextAlignment.Center,
                }
            };
        }

        private async void OnAddAllParsedBills(object sender, EventArgs e)
        {
            if (_parsedBills.Count == 0) return;

            int added = 0;
            int dupes = 0;
            foreach (var parsed in _parsedBills.OrderByDescending(p => p.BillDate))
            {
                var bills = BillPdfParser.SplitDualFuel(parsed);
                foreach (var bill in bills)
                {
                    if (MockDataService.AddBill(bill))
                        added++;
                    else
                        dupes++;
                }
            }

            var justAdded = new List<ParsedBill>(_parsedBills);
            _parsedBills.Clear();
            ParsedResultsCard.IsVisible = false;
            ParsedBillsList.Children.Clear();
            RefreshData();

            var gapWarning = DetectMissingMonths(justAdded);

            RunAudit();

            if (added == 0 && dupes > 0)
            {
                await Application.Current.Windows[0].Page.DisplayAlert(
                    "Already Added",
                    $"These {dupes} bill(s) are already in your history.",
                    "OK");
            }
            else if (!string.IsNullOrEmpty(gapWarning))
            {
                var msg = $"{added} bill record(s) added.";
                if (dupes > 0) msg += $" ({dupes} already existed)";
                msg += $"\n\n{gapWarning}";
                await Application.Current.Windows[0].Page.DisplayAlert(
                    "Bills Added — Gaps Detected", msg, "OK");
            }
            else
            {
                var msg = $"{added} bill record(s) added to your history.";
                if (dupes > 0) msg += $" ({dupes} already existed)";
                await Application.Current.Windows[0].Page.DisplayAlert(
                    "Bills Added", msg, "OK");
            }
        }

        private static string ExtractPostcode(string address)
        {
            if (string.IsNullOrEmpty(address)) return "";
            var m = Regex.Match(address, @"[A-Z]{1,2}\d[A-Z\d]?\s*\d[A-Z]{2}", RegexOptions.IgnoreCase);
            return m.Success ? m.Value.ToUpper() : "";
        }

        private string DetectMissingMonths(List<ParsedBill> justAdded)
        {
            var allBills = MockDataService.GetUtilityBills();
            if (allBills.Count < 2) return "";

            var addedKeys = new HashSet<string>();
            foreach (var p in justAdded)
            {
                var pc = ExtractPostcode(p.Address);
                var types = p.FuelType == "Dual Fuel"
                    ? new[] { "Electricity", "Gas" }
                    : new[] { p.FuelType };
                foreach (var ft in types)
                    addedKeys.Add($"{pc}|{p.Supplier}|{ft}");
            }

            var gaps = new List<string>();
            var thisMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);

            var groups = allBills
                .Where(b => !string.IsNullOrEmpty(b.Supplier))
                .GroupBy(b => new
                {
                    Postcode = ExtractPostcode(b.Address),
                    b.Supplier,
                    b.FuelType,
                });

            foreach (var group in groups)
            {
                var key = $"{group.Key.Postcode}|{group.Key.Supplier}|{group.Key.FuelType}";
                if (!addedKeys.Contains(key)) continue;

                var bills = group.OrderBy(b => b.BillDate).ToList();
                if (bills.Count < 2) continue;

                var months = bills
                    .Select(b => new DateTime(b.BillDate.Year, b.BillDate.Month, 1))
                    .Distinct()
                    .OrderBy(d => d)
                    .ToList();

                var earliest = months.First();
                var latest = months.Last();

                var label = !string.IsNullOrEmpty(group.Key.Postcode)
                    ? $"{group.Key.Supplier} ({group.Key.Postcode}) — {group.Key.FuelType}"
                    : $"{group.Key.Supplier} — {group.Key.FuelType}";

                var current = earliest.AddMonths(1);
                while (current <= latest)
                {
                    if (!months.Contains(current) && current != thisMonth)
                        gaps.Add($"{label}: {current:MMMM yyyy}");
                    current = current.AddMonths(1);
                }
            }

            if (gaps.Count == 0) return "";

            return $"Missing bills detected:\n• {string.Join("\n• ", gaps)}\n\nDrop the missing bill PDFs to fill the gaps.";
        }

        private void OnClearParsedBills(object sender, EventArgs e)
        {
            _parsedBills.Clear();
            ParsedResultsCard.IsVisible = false;
            ParsedBillsList.Children.Clear();
        }

        #endregion

        #region Bill Audit

        private void OnRunAuditClicked(object sender, EventArgs e)
        {
            RunAudit();
        }

        private void RunAudit()
        {
            var allBills = FilterByProperty(MockDataService.GetUtilityBills());
            var bills = allBills.Where(b => b.IsFromPdf).ToList();
            var readings = MockDataService.GetMeterReadings();

            AuditFindingsList.Children.Clear();

            if (bills.Count == 0)
            {
                _lastAuditFindings = null;
                DownloadReportBtn.IsVisible = false;
                AuditSummaryBanner.IsVisible = true;
                AuditSummaryBanner.BackgroundColor = Color.FromArgb("#131B2E");
                AuditSummaryIcon.Text = "📄";
                AuditSummaryText.Text = "Upload PDF bills to run a compliance audit";
                AuditSummaryText.TextColor = Color.FromArgb("#94A3B8");
                AuditSavingsBanner.IsVisible = false;
                AuditSubtitle.Text = "Audits real bills only — drop PDFs above";
                return;
            }

            var findings = BillAuditor.AuditBills(bills, readings);
            _lastAuditFindings = findings;
            DownloadReportBtn.IsVisible = true;

            if (findings.Count == 0)
            {
                AuditSummaryBanner.IsVisible = true;
                AuditSummaryBanner.BackgroundColor = Color.FromArgb("#0D2818");
                AuditSummaryIcon.Text = "✅";
                AuditSummaryText.Text = $"All {bills.Count} bills checked — no issues found";
                AuditSummaryText.TextColor = Color.FromArgb("#22C55E");
                AuditSavingsBanner.IsVisible = false;
                AuditSubtitle.Text = $"Last run: {DateTime.Now:HH:mm} · {bills.Count} bills checked";
                return;
            }

            var criticalCount = findings.Count(f => f.Severity == AuditSeverity.Critical);
            var alertCount = findings.Count(f => f.Severity == AuditSeverity.Alert);
            var totalSavings = findings.Sum(f => f.PotentialSavings);

            if (criticalCount > 0)
            {
                AuditSummaryBanner.IsVisible = true;
                AuditSummaryBanner.BackgroundColor = Color.FromArgb("#3B1A1A");
                AuditSummaryIcon.Text = "🚨";
                AuditSummaryText.Text = $"{criticalCount} critical issue(s) found across {bills.Count} bills";
                AuditSummaryText.TextColor = Color.FromArgb("#EF4444");
            }
            else
            {
                AuditSummaryBanner.IsVisible = true;
                AuditSummaryBanner.BackgroundColor = Color.FromArgb("#2D1F00");
                AuditSummaryIcon.Text = "⚠️";
                AuditSummaryText.Text = $"{findings.Count} issue(s) found across {bills.Count} bills";
                AuditSummaryText.TextColor = Color.FromArgb("#F59E0B");
            }

            foreach (var finding in findings)
            {
                var (borderColor, bgColor) = finding.Severity switch
                {
                    AuditSeverity.Critical => ("#EF4444", "#1A0A0A"),
                    AuditSeverity.Alert => ("#F59E0B", "#1A1400"),
                    AuditSeverity.Warning => ("#3B82F6", "#0A1A2E"),
                    _ => ("#64748B", "#131B2E"),
                };

                var savingsLabel = finding.PotentialSavings > 0
                    ? new Label
                    {
                        Text = $"£{finding.PotentialSavings:F2}",
                        TextColor = Color.FromArgb("#EF4444"),
                        FontSize = 14,
                        FontAttributes = FontAttributes.Bold,
                        VerticalOptions = LayoutOptions.Center,
                    }
                    : null;

                var rightContent = savingsLabel != null
                    ? (View)savingsLabel
                    : new Label
                    {
                        Text = finding.Severity.ToString(),
                        TextColor = Color.FromArgb(borderColor),
                        FontSize = 10,
                        VerticalOptions = LayoutOptions.Center,
                    };

                var row = new Border
                {
                    BackgroundColor = Color.FromArgb(bgColor),
                    StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 8 },
                    Stroke = Color.FromArgb(borderColor),
                    StrokeThickness = 1,
                    Padding = new Thickness(12, 10),
                    Content = new Grid
                    {
                        ColumnDefinitions = new ColumnDefinitionCollection
                        {
                            new ColumnDefinition(new GridLength(24)),
                            new ColumnDefinition(GridLength.Star),
                            new ColumnDefinition(GridLength.Auto),
                        },
                        ColumnSpacing = 10,
                        Children =
                        {
                            BuildCol(0, new Label
                            {
                                Text = finding.Icon,
                                FontSize = 16,
                                VerticalOptions = LayoutOptions.Start,
                                Margin = new Thickness(0, 2, 0, 0),
                            }),
                            BuildCol(1, new VerticalStackLayout
                            {
                                Spacing = 2,
                                Children =
                                {
                                    new Label
                                    {
                                        Text = finding.Title,
                                        TextColor = Color.FromArgb("#F1F5F9"),
                                        FontSize = 12,
                                        FontAttributes = FontAttributes.Bold,
                                    },
                                    new Label
                                    {
                                        Text = finding.Description,
                                        TextColor = Color.FromArgb("#94A3B8"),
                                        FontSize = 10,
                                        LineBreakMode = LineBreakMode.WordWrap,
                                    },
                                    new Label
                                    {
                                        Text = finding.BillReference,
                                        TextColor = Color.FromArgb("#64748B"),
                                        FontSize = 9,
                                    },
                                }
                            }),
                            BuildCol(2, rightContent),
                        }
                    }
                };

                AuditFindingsList.Children.Add(row);
            }

            if (totalSavings > 0)
            {
                AuditSavingsBanner.IsVisible = true;
                AuditTotalSavings.Text = $"£{totalSavings:F2}";
            }
            else
            {
                AuditSavingsBanner.IsVisible = false;
            }

            AuditSubtitle.Text = $"Last run: {DateTime.Now:HH:mm} · {bills.Count} bills · {findings.Count} issue(s)";
        }

        private async void OnDownloadReportClicked(object sender, EventArgs e)
        {
            if (_lastAuditFindings == null)
            {
                await Application.Current.Windows[0].Page.DisplayAlert(
                    "No Audit Data", "Run an audit first before downloading a report.", "OK");
                return;
            }

            try
            {
                DownloadReportBtn.Text = "Generating...";
                DownloadReportBtn.IsEnabled = false;

                var bills = MockDataService.GetUtilityBills().Where(b => b.IsFromPdf).ToList();
                var filePath = await Task.Run(() => AuditReportGenerator.GenerateReport(_lastAuditFindings, bills));

                DownloadReportBtn.Text = "Report";
                DownloadReportBtn.IsEnabled = true;

#if WINDOWS
                var process = new System.Diagnostics.Process();
                process.StartInfo = new System.Diagnostics.ProcessStartInfo
                {
                    FileName = filePath,
                    UseShellExecute = true,
                };
                process.Start();
#endif

                await Application.Current.Windows[0].Page.DisplayAlert(
                    "Report Saved",
                    $"Audit report saved to:\n{filePath}",
                    "OK");
            }
            catch (Exception ex)
            {
                DownloadReportBtn.Text = "Report";
                DownloadReportBtn.IsEnabled = true;

                await Application.Current.Windows[0].Page.DisplayAlert(
                    "Report Error", $"Failed to generate report:\n{ex.GetType().Name}: {ex.Message}\n\n{ex.InnerException?.Message}", "OK");
            }
        }

        #endregion

        #region MAUI Drop Gesture (unused but required by XAML)

        private void OnDragOver(object sender, DragEventArgs e)
        {
            e.AcceptedOperation = DataPackageOperation.Copy;
        }

        private void OnDragLeave(object sender, DragEventArgs e)
        {
            ResetDropZone();
        }

        private void OnDrop(object sender, DropEventArgs e) { }

        #endregion

        public async Task PlayEntryAnimation()
        {
            AnimationHelper.PrepareForEntry(SupplierCard, ReadingsCard, CostsCard, AuditCard);
            _ = AnimationHelper.AnimateEntry(SupplierCard, 0, 400);
            _ = AnimationHelper.AnimateEntry(ReadingsCard, 100, 400);
            _ = AnimationHelper.AnimateEntry(CostsCard, 200, 400);
            _ = AnimationHelper.AnimateEntry(AuditCard, 300, 400);
            _ = AnimationHelper.FadeIn(DropZone, 100, 400);
            await AnimationHelper.FadeIn(BillsCard, 150, 400);
        }
    }

    public class BillDisplayItem
    {
        public string Period { get; set; }
        public string FuelType { get; set; }
        public Color FuelColour { get; set; }
        public string Postcode { get; set; }
        public string UnitsFormatted { get; set; }
        public string AmountFormatted { get; set; }
        public string ChangeFormatted { get; set; }
        public Color ChangeColour { get; set; }
        public string SupplierAbbrev { get; set; }
        public Color SupplierBrandColor { get; set; }
        public double AbbrevFontSize { get; set; }
        public string MeterReadingFormatted { get; set; }
        public int BillIndex { get; set; }
    }
}
