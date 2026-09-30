using System.Globalization;
using SmartCubeMobile.Services;

namespace SmartCubeMobile.Dashboard
{
    // Opens UK comparison sites inside SmartCube with the current policy's details to hand,
    // so the user can get like-for-like quotes before renewal.
    public partial class InsuranceComparePage : ContentPage
    {
        private readonly InsurancePolicy _policy;
        private static readonly CultureInfo Gbp = new("en-GB");

        private static readonly Dictionary<string, (string Name, string Url)[]> Sites = new(StringComparer.OrdinalIgnoreCase)
        {
            ["Car"] = new[]
            {
                ("MoneySuperMarket", "https://www.moneysupermarket.com/car-insurance/"),
                ("Compare the Market", "https://www.comparethemarket.com/car-insurance/"),
                ("Confused.com", "https://www.confused.com/car-insurance"),
                ("GoCompare", "https://www.gocompare.com/car-insurance/"),
            },
            ["House"] = new[]
            {
                ("MoneySuperMarket", "https://www.moneysupermarket.com/home-insurance/"),
                ("Compare the Market", "https://www.comparethemarket.com/home-insurance/"),
                ("Confused.com", "https://www.confused.com/home-insurance"),
                ("GoCompare", "https://www.gocompare.com/home-insurance/"),
            },
            ["Life"] = new[]
            {
                ("MoneySuperMarket", "https://www.moneysupermarket.com/life-insurance/"),
                ("Compare the Market", "https://www.comparethemarket.com/life-insurance/"),
                ("Confused.com", "https://www.confused.com/life-insurance"),
                ("GoCompare", "https://www.gocompare.com/life-insurance/"),
            },
            ["Pet"] = new[]
            {
                ("MoneySuperMarket", "https://www.moneysupermarket.com/pet-insurance/"),
                ("Compare the Market", "https://www.comparethemarket.com/pet-insurance/"),
                ("Confused.com", "https://www.confused.com/pet-insurance"),
                ("GoCompare", "https://www.gocompare.com/pet-insurance/"),
            },
        };

        public InsuranceComparePage(InsurancePolicy policy)
        {
            InitializeComponent();
            _policy = policy;

            var category = Sites.ContainsKey(policy.Category ?? "") ? policy.Category : "Car";
            PageTitle.Text = $"Compare {category} Insurance";

            var current = new List<string>();
            if (!string.IsNullOrWhiteSpace(policy.Provider)) current.Add(policy.Provider);
            if (policy.AnnualPremium > 0) current.Add($"{policy.AnnualPremium.ToString("C2", Gbp)}/yr");
            else if (policy.MonthlyPremium > 0) current.Add($"{policy.MonthlyPremium.ToString("C2", Gbp)}/mo");
            if (policy.RenewalDate.HasValue) current.Add($"renews {policy.RenewalDate:dd MMM yyyy}");
            PageSubtitle.Text = current.Count > 0 ? "Current: " + string.Join(" · ", current) : "Get quotes to compare with your current policy";

            BuildDetailChips();

            foreach (var (name, url) in Sites[category])
            {
                var btn = new Button
                {
                    Text = name,
                    FontSize = 11,
                    FontAttributes = FontAttributes.Bold,
                    BackgroundColor = Color.FromArgb("#1E3A5F"),
                    TextColor = Color.FromArgb("#60A5FA"),
                    CornerRadius = 8,
                    Padding = new Thickness(10, 2),
                    HeightRequest = 28,
                };
                var target = url;
                btn.Clicked += (_, _) => CompareWebView.Source = new UrlWebViewSource { Url = target };
                ToolTipProperties.SetText(btn, $"Open {name}'s {category.ToLower()} insurance quotes in this window.");
                SiteButtons.Add(btn);
            }

            CompareWebView.Source = new UrlWebViewSource { Url = Sites[category][0].Url };
        }

        // Small clickable chips with the details a quote form will ask for; click copies to clipboard.
        private void BuildDetailChips()
        {
            void Chip(string label, string value)
            {
                if (string.IsNullOrWhiteSpace(value)) return;
                var chip = new Border
                {
                    BackgroundColor = Color.FromArgb("#0D1322"),
                    Stroke = Color.FromArgb("#2A3F6B"),
                    StrokeThickness = 1,
                    StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 6 },
                    Padding = new Thickness(8, 3),
                    Content = new Label
                    {
                        Text = $"{label}: {value}",
                        FontSize = 11,
                        TextColor = Color.FromArgb("#E2E8F0"),
                    },
                };
                chip.GestureRecognizers.Add(new TapGestureRecognizer
                {
                    Command = new Command(async () =>
                    {
                        try
                        {
                            await Clipboard.Default.SetTextAsync(value);
                            ((Label)chip.Content).Text = $"{label}: {value} ✓";
                            await Task.Delay(1200);
                            ((Label)chip.Content).Text = $"{label}: {value}";
                        }
                        catch { }
                    })
                });
                ToolTipProperties.SetText(chip, $"Click to copy \"{value}\" so you can paste it into the quote form.");
                DetailChips.Add(chip);
            }

            Chip("Reg", _policy.VehicleReg);
            Chip("Vehicle", _policy.VehicleMakeModel);
            Chip("Address", _policy.PropertyAddress);
            Chip("Pet", string.IsNullOrWhiteSpace(_policy.PetBreed) ? _policy.PetName : $"{_policy.PetName} ({_policy.PetBreed})");
            Chip("Name", _policy.NamedInsured);
            Chip("Cover", _policy.PolicyType);
            if (_policy.Excess > 0) Chip("Excess", _policy.Excess.ToString("C0", Gbp));
            if (_policy.AnnualPremium > 0) Chip("Paying now", _policy.AnnualPremium.ToString("C2", Gbp) + "/yr");
            if (_policy.RenewalDate.HasValue) Chip("Renewal", _policy.RenewalDate.Value.ToString("dd/MM/yyyy"));
            var postcode = MockData.MockDataService.GetProperty()?.Postcode;
            if (!string.IsNullOrWhiteSpace(postcode)) Chip("Postcode", postcode);
        }

        private void OnWebViewNavigating(object sender, WebNavigatingEventArgs e)
        {
            LoadingIndicator.IsRunning = true;
            UrlLabel.Text = e.Url;
        }

        private void OnWebViewNavigated(object sender, WebNavigatedEventArgs e)
        {
            LoadingIndicator.IsRunning = false;
            UrlLabel.Text = e.Url;
            if (e.Result == WebNavigationResult.Success)
                _ = AutoFillAfterDelay();
        }

        private static readonly string[] FriendlyNames =
        {
            "reg=registration", "postcode=postcode", "firstName=first name", "lastName=surname", "fullName=name",
            "email=email", "phone=phone", "dob=date of birth", "start=cover start date", "licenceDate=licence date",
            "house=house number", "address=address", "marital=marital status", "employment=employment", "title=title",
            "mileage=mileage", "make=make", "model=model", "year=year", "petName=pet name", "petBreed=breed",
        };

        private async Task AutoFillAfterDelay()
        {
            // Quote sites build their forms after the page loads; give them a moment.
            await Task.Delay(1500);
            await RunAutoFill(silentIfNothing: true);
        }

        private async void OnAutoFillClicked(object sender, EventArgs e) => await RunAutoFill(silentIfNothing: false);

        private async Task RunAutoFill(bool silentIfNothing)
        {
            try
            {
                var values = WebFormAutoFill.ValuesFor(_policy);
                var script = WebFormAutoFill.BuildScript(values);
                var raw = await CompareWebView.EvaluateJavaScriptAsync(script);
                if (string.IsNullOrEmpty(raw)) { if (!silentIfNothing) PageSubtitle.Text = "Nothing to fill on this page yet."; return; }
                raw = raw.Trim('"').Replace("\\\"", "\"").Replace("\\\\", "\\");
                var json = Newtonsoft.Json.Linq.JObject.Parse(raw);
                var filled = (json["filled"] as Newtonsoft.Json.Linq.JArray)?.Select(t => t.ToString()).ToList() ?? new List<string>();
                if (filled.Count == 0)
                {
                    if (!silentIfNothing) PageSubtitle.Text = "No fields on this page matched your details. Fill this step yourself and try again on the next one.";
                    return;
                }
                var names = filled.Select(f => FriendlyNames.FirstOrDefault(n => n.StartsWith(f + "="))?.Split('=')[1] ?? f);
                PageSubtitle.Text = $"Auto-filled: {string.Join(", ", names)}. Check them, then answer the rest.";
            }
            catch (Exception ex)
            {
                if (!silentIfNothing) PageSubtitle.Text = $"Auto-fill didn't work on this page ({ex.Message}).";
            }
        }

        private void OnBrowserBackClicked(object sender, EventArgs e) { if (CompareWebView.CanGoBack) CompareWebView.GoBack(); }
        private void OnBrowserForwardClicked(object sender, EventArgs e) { if (CompareWebView.CanGoForward) CompareWebView.GoForward(); }
        private async void OnBackClicked(object sender, EventArgs e) => await Navigation.PopAsync();

        private async void OnHelpClicked(object sender, EventArgs e)
        {
            await DisplayAlert("Compare Insurance",
                "Get quotes from the main UK comparison sites without leaving SmartCube. On each step SmartCube fills the fields it knows (registration, postcode, name, email, date of birth, cover start date and so on); press Auto-fill to try again after a page changes. Your policy details are also shown at the top; click any to copy it. " +
                "Quotes come from the comparison site itself, not SmartCube, and nothing you type on their site is stored here. " +
                "If you switch, add the new policy from the Insurance page so your renewal reminders stay right.",
                "OK");
        }
    }
}
