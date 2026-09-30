using System.Globalization;
using SmartCubeMobile.MockData;
using SmartCubeMobile.Services;

namespace SmartCubeMobile.Dashboard
{
    public partial class TariffComparePage : ContentPage
    {
        private static readonly CultureInfo _gbp = new("en-GB");
        private string _postcode;
        private string _postcodeSource;
        private decimal? _elecKwh;
        private decimal? _gasKwh;
        private string _usageSource;
        private decimal? _currentSpend;
        private string _spendSource;

        private bool _started;
        private string _supplierFilter;
        private string _fuel = "dual";   // "dual", "elec" or "gas"

        private void ApplyFuelHeader()
        {
            (FuelBtn.Text, var bg, var fg) = _fuel switch
            {
                "elec" => ("Electricity only", "#3B2A0A", "#FBBF24"),
                "gas" => ("Gas only", "#0B2A3B", "#38BDF8"),
                _ => ("Gas & electricity", "#0A3D2E", "#34D399"),
            };
            FuelBtn.BackgroundColor = Color.FromArgb(bg);
            FuelBtn.TextColor = Color.FromArgb(fg);
        }

        // Cycles Gas & electricity -> Electricity only -> Gas only
        private async void OnFuelClicked(object sender, EventArgs e)
        {
            _fuel = _fuel switch { "dual" => "elec", "elec" => "gas", _ => "dual" };
            ApplyFuelHeader();
            await Load();
        }

        public TariffComparePage() : this(null) { }

        // supplierFilter: show only this supplier's tariffs (opened from a supplier card)
        public TariffComparePage(string supplierFilter)
        {
            InitializeComponent();
            _supplierFilter = string.IsNullOrWhiteSpace(supplierFilter) ? null : supplierFilter;
            ApplyFilterHeader();
        }

        private void ApplyFilterHeader()
        {
            TitleLabel.Text = _supplierFilter == null ? "Energy Tariffs" : $"{_supplierFilter} tariffs";
            AllSuppliersBtn.IsVisible = _supplierFilter != null;
        }

        private async void OnShowAllClicked(object sender, EventArgs e)
        {
            _supplierFilter = null;
            ApplyFilterHeader();
            await Load();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            if (_started) return;
            _started = true;

            ShowLoading("Checking your details…", "Looking up your postcode and energy usage");
            try
            {
                (_postcode, _postcodeSource) = TariffService.ResolvePostcode();
                (_elecKwh, _gasKwh) = TariffService.EstimateAnnualKwh();
                _usageSource = _elecKwh.HasValue || _gasKwh.HasValue ? "from your bills" : "typical household (no bills yet)";
                _currentSpend = TariffService.EstimateAnnualSpend();
                _spendSource = _currentSpend.HasValue ? "from your bills" : null;
                // No gas anywhere in the user's data: compare electricity-only tariffs by default.
                _fuel = _gasKwh > 0 || TariffService.UserHasGas() ? "dual" : "elec";
                ApplyFuelHeader();
            }
            catch (Exception ex)
            {
                HideLoading();
                ShowStatus("Could not read your details: " + ex.Message);
                return;
            }

            if (string.IsNullOrEmpty(_postcode))
            {
                HideLoading();
                ShowStatus("We need your postcode to find tariffs for your area.");
                if (!await PromptPostcode()) return;
            }
            await Load();
        }

        private CancellationTokenSource _loadingCts;

        private void ShowLoading(string title, string detail)
        {
            LoadingTitle.Text = title;
            LoadingDetail.Text = detail;
            if (LoadingOverlay.IsVisible) return;

            LoadingBar.WidthRequest = 0;
            LoadingOverlay.Opacity = 0;
            LoadingOverlay.IsVisible = true;
            _ = LoadingOverlay.FadeTo(1, 200, Easing.CubicOut);

            _loadingCts?.Cancel();
            _loadingCts = new CancellationTokenSource();
            _ = PulseLoadingImage(_loadingCts.Token);
            _ = AnimateLoadingBar(_loadingCts.Token);
        }

        private async void HideLoading()
        {
            if (!LoadingOverlay.IsVisible) return;
            _loadingCts?.Cancel();
            await LoadingOverlay.FadeTo(0, 300, Easing.CubicIn);
            LoadingOverlay.IsVisible = false;
        }

        private async Task PulseLoadingImage(CancellationToken ct)
        {
            try
            {
                while (!ct.IsCancellationRequested)
                {
                    await LoadingImage.ScaleTo(1.05, 800, Easing.SinInOut);
                    if (ct.IsCancellationRequested) break;
                    await LoadingImage.ScaleTo(0.95, 800, Easing.SinInOut);
                }
            }
            catch (TaskCanceledException) { }
            LoadingImage.Scale = 1.0;
        }

        private async Task AnimateLoadingBar(CancellationToken ct)
        {
            try
            {
                while (!ct.IsCancellationRequested)
                {
                    for (double w = 0; w <= 260 && !ct.IsCancellationRequested; w += 2.6)
                    {
                        LoadingBar.WidthRequest = w;
                        await Task.Delay(30, ct);
                    }
                    LoadingBar.WidthRequest = 0;
                }
            }
            catch (TaskCanceledException) { }
        }

        private void ShowStatus(string text)
        {
            StatusLabel.Text = text ?? "";
            StatusLabel.IsVisible = !string.IsNullOrEmpty(text);
        }

        private async Task Load()
        {
            PostcodeLabel.Text = _postcode;
            PostcodeSourceLabel.Text = _postcodeSource ?? "";
            UsageLabel.Text = FormatUsage(_elecKwh, _gasKwh);
            UsageSourceLabel.Text = _usageSource;
            ShowStatus(null);
            TariffList.Children.Clear();
            ShowLoading($"Finding tariffs for {_postcode}…", "Fetching the latest prices for your supply region");

            TariffResult r;
            var started = DateTime.UtcNow;
            try
            {
                r = await TariffService.GetTariffs(_postcode, _fuel == "gas" ? null : _elecKwh, _fuel == "elec" ? null : _gasKwh, _fuel);
                var remaining = TimeSpan.FromMilliseconds(1200) - (DateTime.UtcNow - started);
                if (remaining > TimeSpan.Zero) await Task.Delay(remaining);
            }
            finally
            {
                HideLoading();
            }

            if (!r.Ok)
            {
                ShowStatus(r.Error ?? "Could not load tariffs.");
                return;
            }

            RegionLabel.Text = string.IsNullOrEmpty(r.RegionName) ? "—" : r.RegionName;
            RegionSourceLabel.Text = r.RegionSource == "energylinx" ? "confirmed for this postcode" : "based on postcode area";
            SubtitleLabel.Text = _fuel switch
            {
                "elec" => $"Electricity-only tariffs available in {r.RegionName}",
                "gas" => $"Gas-only tariffs available in {r.RegionName}",
                _ => $"Dual-fuel tariffs available in {r.RegionName}",
            };

            if (!r.Available)
            {
                ShowStatus(r.Error ?? "No tariff data for this region yet.");
                FetchedLabel.Text = "—";
                return;
            }

            FetchedLabel.Text = r.FetchedAt.HasValue ? r.FetchedAt.Value.ToLocalTime().ToString("dd MMM yyyy") : "—";
            CountLabel.Text = $"{r.Tariffs.Count} tariffs";
            if (!_elecKwh.HasValue && !_gasKwh.HasValue)
            {
                UsageLabel.Text = FormatUsage(r.AssumedElecKwh, r.AssumedGasKwh);
            }

            ShowStatus(null);
            var current = MockDataService.GetSuppliers().Select(s => s.Name).Where(n => !string.IsNullOrEmpty(n)).Distinct().ToList();
            var all = r.Tariffs.OrderBy(t => t.CostForYou).ToList();
            var ordered = _supplierFilter == null ? all : all.Where(t => Matches(_supplierFilter, t.SupplierName)).ToList();
            var cheapest = all.FirstOrDefault();

            if (_supplierFilter != null && ordered.Count == 0)
            {
                ShowStatus($"{_supplierFilter} has no tariffs listed for {r.RegionName} on EnergyLinx. Tap 'Show all suppliers' to compare everyone.");
            }

            // Baseline for savings: what the bills say you pay now; otherwise your supplier's dearest
            // listed tariff (usually the standard variable rate).
            var baseline = _currentSpend;
            var baselineNote = _spendSource;
            if (baseline == null)
            {
                var mine = ordered.Where(t => current.Any(c => Matches(c, t.SupplierName))).ToList();
                if (mine.Count > 0)
                {
                    baseline = mine.Max(t => t.CostForYou);
                    baselineNote = $"{mine.First().SupplierName} standard rate (add bills for your real spend)";
                }
            }
            SpendLabel.Text = baseline.HasValue ? baseline.Value.ToString("C0", _gbp) + "/yr" : "—";
            SpendSourceLabel.Text = baselineNote ?? "add energy bills to see savings";

            var quickSwitch = ordered.Count(t => current.Any(c => Matches(c, t.SupplierName)));
            foreach (var t in ordered)
                TariffList.Children.Add(BuildCard(t, t == cheapest, current.Any(c => Matches(c, t.SupplierName)), baseline));

            UpdateChart(ordered, baseline, baselineNote);

            var bestInList = ordered.FirstOrDefault();
            var best = bestInList != null && baseline.HasValue ? baseline.Value - bestInList.CostForYou : 0;
            if (_supplierFilter != null)
            {
                var cheaperElsewhere = cheapest != null && bestInList != null && cheapest.CostForYou < bestInList.CostForYou - 1
                    ? $" · {cheapest.SupplierName} is cheaper ({cheapest.CostForYou.ToString("C0", _gbp)}/yr)" : "";
                SubtitleLabel.Text = $"{ordered.Count} tariffs you can switch to without changing supplier"
                    + (best > 0 ? $" · save up to {best.ToString("C0", _gbp)}/yr" : "") + cheaperElsewhere;
            }
            else
            {
                SubtitleLabel.Text = $"{ordered.Count} dual-fuel tariffs in {r.RegionName}"
                    + (best > 0 ? $" · save up to {best.ToString("C0", _gbp)}/yr" : "")
                    + (quickSwitch > 0 ? $" · {quickSwitch} quick switches with your current supplier" : "");
            }

            FootnoteLabel.Text = (_elecKwh.HasValue || _gasKwh.HasValue
                ? $"Costs are estimated for your usage ({FormatUsage(_elecKwh, _gasKwh)}) from each tariff's unit rates and standing charges, including VAT as quoted. "
                : $"Costs are EnergyLinx estimates for a typical household ({FormatUsage(r.AssumedElecKwh, r.AssumedGasKwh)}). Add your energy bills to see prices for your own usage. ")
                + (baseline.HasValue ? $"Savings are against {SpendLabel.Text} ({baselineNote}). " : "")
                + "Blue cards are tariffs from your current supplier — switching to these is usually quickest, with no change of supplier. "
                + $"Prices collected from EnergyLinx for {r.RegionName}; the supplier's own quote may differ.";
        }

        private void UpdateChart(List<TariffOffer> tariffs, decimal? baseline, string baselineNote)
        {
            if (tariffs.Count == 0)
            {
                ChartCard.IsVisible = false;
                return;
            }
            var cheapest = tariffs[0];
            var dearest = tariffs[^1];
            var bars = new List<TariffBar>();
            if (baseline.HasValue)
                bars.Add(new TariffBar("You pay now", baseline.Value, "#F59E0B", baselineNote != null && baselineNote.StartsWith("from") ? "from your bills" : "standard rate"));
            bars.Add(new TariffBar("Cheapest", cheapest.CostForYou, "#22C55E", $"{cheapest.SupplierName} · {cheapest.TariffName}"));
            bars.Add(new TariffBar("Most expensive", dearest.CostForYou, "#EF4444", $"{dearest.SupplierName} · {dearest.TariffName}"));

            ChartView.HeightRequest = 20 + bars.Count * 38;
            ChartView.Drawable = new TariffBarsDrawable(bars);
            ChartView.Invalidate();
            ChartCaption.Text = baseline.HasValue
                ? $"cheapest saves {(baseline.Value - cheapest.CostForYou).ToString("C0", _gbp)}/yr · spread {(dearest.CostForYou - cheapest.CostForYou).ToString("C0", _gbp)}/yr"
                : $"spread {(dearest.CostForYou - cheapest.CostForYou).ToString("C0", _gbp)}/yr between cheapest and dearest";
            ChartCard.IsVisible = true;
        }

        private record TariffBar(string Label, decimal Value, string Colour, string Detail);

        private class TariffBarsDrawable : IDrawable
        {
            private readonly List<TariffBar> _bars;
            public TariffBarsDrawable(List<TariffBar> bars) { _bars = bars; }

            public void Draw(ICanvas canvas, RectF rect)
            {
                if (_bars.Count == 0) return;
                var max = (float)_bars.Max(b => b.Value);
                if (max <= 0) return;

                const float labelW = 110, valueW = 80, rowH = 38, barH = 16;
                var barLeft = rect.Left + labelW;
                var barMaxW = Math.Max(20, rect.Width - labelW - valueW);
                canvas.FontSize = 12;

                for (int i = 0; i < _bars.Count; i++)
                {
                    var b = _bars[i];
                    var top = rect.Top + 10 + i * rowH;
                    var colour = Color.FromArgb(b.Colour);

                    canvas.FontColor = Color.FromArgb("#CBD5E1");
                    canvas.FontSize = 12;
                    canvas.DrawString(b.Label, rect.Left, top - 2, labelW - 8, barH + 4, HorizontalAlignment.Left, VerticalAlignment.Center);

                    canvas.FillColor = Color.FromArgb("#1E2D4A");
                    canvas.FillRoundedRectangle(barLeft, top, barMaxW, barH, 5);

                    var w = Math.Max(6, barMaxW * (float)b.Value / max);
                    canvas.FillColor = colour;
                    canvas.FillRoundedRectangle(barLeft, top, w, barH, 5);

                    canvas.FontColor = colour;
                    canvas.FontSize = 13;
                    canvas.DrawString(b.Value.ToString("C0", new CultureInfo("en-GB")) + "/yr", barLeft + barMaxW + 8, top - 2, valueW - 8, barH + 4, HorizontalAlignment.Left, VerticalAlignment.Center);

                    canvas.FontColor = Color.FromArgb("#64748B");
                    canvas.FontSize = 10;
                    canvas.DrawString(b.Detail ?? "", barLeft, top + barH + 1, barMaxW, 14, HorizontalAlignment.Left, VerticalAlignment.Top);
                }
            }
        }

        private static bool Matches(string a, string b)
        {
            if (string.IsNullOrEmpty(a) || string.IsNullOrEmpty(b)) return false;
            a = a.ToLowerInvariant().Replace(" energy", "").Replace(".", "").Trim();
            b = b.ToLowerInvariant().Replace(" energy", "").Replace(".", "").Trim();
            return a == b || a.Contains(b) || b.Contains(a);
        }

        private static string FormatUsage(decimal? elec, decimal? gas)
        {
            var parts = new List<string>();
            if (elec > 0) parts.Add($"{elec:N0} kWh elec");
            if (gas > 0) parts.Add($"{gas:N0} kWh gas");
            return parts.Count == 0 ? "—" : string.Join(" · ", parts);
        }

        private View BuildCard(TariffOffer t, bool cheapest, bool isCurrent, decimal? baseline)
        {
            var card = new Border
            {
                BackgroundColor = Color.FromArgb(isCurrent ? "#152A4A" : "#131B2E"),
                Stroke = Color.FromArgb(isCurrent ? "#3B82F6" : cheapest ? "#22C55E60" : "#1E2D4A"),
                StrokeThickness = cheapest || isCurrent ? 2 : 1,
                StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 12 },
                Padding = new Thickness(16, 12),
            };
            var grid = new Grid
            {
                ColumnDefinitions = { new ColumnDefinition(GridLength.Star), new ColumnDefinition(GridLength.Auto), new ColumnDefinition(GridLength.Auto) },
                ColumnSpacing = 16,
            };

            var info = new VerticalStackLayout { Spacing = 3, VerticalOptions = LayoutOptions.Center };
            var nameRow = new HorizontalStackLayout { Spacing = 8 };
            nameRow.Add(new Label { Text = t.SupplierName, TextColor = Color.FromArgb("#F1F5F9"), FontSize = 15, FontAttributes = FontAttributes.Bold });
            if (cheapest) nameRow.Add(Tag("Cheapest", "#22C55E"));
            if (isCurrent) nameRow.Add(Tag("Your supplier · quick switch", "#3B82F6"));
            if (!t.Switchable) nameRow.Add(Tag("Not switchable online", "#94A3B8"));
            info.Add(nameRow);
            info.Add(new Label { Text = t.TariffName, TextColor = Color.FromArgb("#60A5FA"), FontSize = 12 });

            var terms = new List<string>();
            if (t.FixedTermMonths > 0) terms.Add($"Fixed {t.FixedTermMonths} months");
            else if (!string.IsNullOrEmpty(t.FixedTermEndDate)) terms.Add($"Fixed until {t.FixedTermEndDate[..Math.Min(10, t.FixedTermEndDate.Length)]}");
            else terms.Add("Variable");
            if (!string.IsNullOrEmpty(t.PaymentType)) terms.Add(t.PaymentType.Replace("-", " "));
            var exit = (t.ElecExitFee ?? 0) + (t.GasExitFee ?? 0);
            terms.Add(exit > 0 ? $"Exit fees {exit.ToString("C0", _gbp)}" : "No exit fees");
            info.Add(new Label { Text = string.Join("  ·  ", terms), TextColor = Color.FromArgb("#94A3B8"), FontSize = 11 });
            info.Add(new Label
            {
                Text = _fuel == "gas" || t.ElecUnitRate <= 0
                    ? $"Gas {t.GasUnitRate:0.00}p/kWh + {t.GasStandingCharge:0.00}p/day"
                    : _fuel == "elec" || t.GasUnitRate <= 0
                    ? $"Elec {t.ElecUnitRate:0.00}p/kWh + {t.ElecStandingCharge:0.00}p/day"
                    : $"Elec {t.ElecUnitRate:0.00}p/kWh + {t.ElecStandingCharge:0.00}p/day   Gas {t.GasUnitRate:0.00}p/kWh + {t.GasStandingCharge:0.00}p/day",
                TextColor = Color.FromArgb("#64748B"), FontSize = 10,
            });
            grid.Add(info, 0);

            var cost = new VerticalStackLayout { Spacing = 1, VerticalOptions = LayoutOptions.Center };
            cost.Add(new Label
            {
                Text = (t.CostForYou / 12m).ToString("C2", _gbp) + "/mo",
                TextColor = Color.FromArgb(cheapest ? "#22C55E" : "#F59E0B"), FontSize = 17, FontAttributes = FontAttributes.Bold, HorizontalTextAlignment = TextAlignment.End,
            });
            cost.Add(new Label { Text = t.CostForYou.ToString("C0", _gbp) + "/yr", TextColor = Color.FromArgb("#94A3B8"), FontSize = 11, HorizontalTextAlignment = TextAlignment.End });
            if (baseline.HasValue)
            {
                var diff = baseline.Value - t.CostForYou;
                cost.Add(new Label
                {
                    Text = diff >= 1 ? $"Save {diff.ToString("C0", _gbp)}/yr"
                         : diff <= -1 ? $"{(-diff).ToString("C0", _gbp)}/yr more"
                         : "Same as now",
                    TextColor = Color.FromArgb(diff >= 1 ? "#22C55E" : diff <= -1 ? "#EF4444" : "#94A3B8"),
                    FontSize = 12, FontAttributes = FontAttributes.Bold, HorizontalTextAlignment = TextAlignment.End,
                });
            }
            grid.Add(cost, 1);

            var btn = new Button
            {
                Text = "View",
                BackgroundColor = Color.FromArgb("#1E3A5F"), TextColor = Color.FromArgb("#60A5FA"),
                FontSize = 11, FontAttributes = FontAttributes.Bold, CornerRadius = 8, Padding = new Thickness(12, 2), HeightRequest = 28,
                VerticalOptions = LayoutOptions.Center, IsEnabled = !string.IsNullOrEmpty(t.SignupUrl),
            };
            ToolTipProperties.SetText(btn, $"Open {t.SupplierName}'s website to sign up to this tariff.");
            btn.Clicked += async (s, e) => { try { await Launcher.Default.OpenAsync(new Uri(t.SignupUrl)); } catch { } };
            grid.Add(btn, 2);

            card.Content = grid;
            return card;
        }

        private static Border Tag(string text, string colour) => new()
        {
            BackgroundColor = Color.FromArgb(colour + "20"),
            Stroke = Colors.Transparent,
            StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 4 },
            Padding = new Thickness(6, 1),
            VerticalOptions = LayoutOptions.Center,
            Content = new Label { Text = text, TextColor = Color.FromArgb(colour), FontSize = 9, FontAttributes = FontAttributes.Bold },
        };

        private async Task<bool> PromptPostcode()
        {
            var value = await DisplayPromptAsync("Postcode", "Enter the postcode of the property to compare tariffs for:",
                placeholder: "e.g. SK8 3JH", initialValue: _postcode ?? "", maxLength: 10);
            if (value == null) return false;
            var pc = TariffService.ExtractPostcode(value);
            if (pc == null)
            {
                await DisplayAlert("Postcode", "That doesn't look like a full UK postcode.", "OK");
                return false;
            }
            _postcode = pc;
            _postcodeSource = "entered by you";
            return true;
        }

        private async void OnChangePostcodeClicked(object sender, EventArgs e)
        {
            if (await PromptPostcode()) await Load();
        }

        private async void OnChangeUsageClicked(object sender, EventArgs e)
        {
            var elec = await DisplayPromptAsync("Electricity usage", "Electricity used per year (kWh):", keyboard: Keyboard.Numeric, initialValue: _elecKwh?.ToString("0") ?? "");
            if (elec == null) return;
            var gas = await DisplayPromptAsync("Gas usage", "Gas used per year (kWh), or blank for none:", keyboard: Keyboard.Numeric, initialValue: _gasKwh?.ToString("0") ?? "");
            if (gas == null) return;
            _elecKwh = decimal.TryParse(elec, out var ev) && ev > 0 ? ev : null;
            _gasKwh = decimal.TryParse(gas, out var gv) && gv > 0 ? gv : null;
            _usageSource = _elecKwh.HasValue || _gasKwh.HasValue ? "entered by you" : "typical household";
            await Load();
        }

        private async void OnBackClicked(object sender, EventArgs e) => await Navigation.PopAsync();

        private async void OnHelpClicked(object sender, EventArgs e)
        {
            await DisplayAlert("Energy Tariffs",
                "This page compares energy tariffs for your postcode. Change postcode or Change usage adjust the details used for the comparison, and View opens the supplier's website to sign up. Blue cards are your current supplier's tariffs, for a quick switch with no supplier change. Your bills and usage are read locally; tariff prices come from EnergyLinx via the SmartCube server.",
                "OK");
        }
    }
}
