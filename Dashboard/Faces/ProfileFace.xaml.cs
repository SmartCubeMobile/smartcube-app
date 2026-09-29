using System.Globalization;
using SmartCubeMobile.Dashboard;
using SmartCubeMobile.MockData;
using SmartCubeMobile.Services;

namespace SmartCubeMobile.Dashboard.Faces
{
    public partial class ProfileFace : ContentView, IAnimatedFace
    {
        private static readonly CultureInfo _gbp = new("en-GB");

        private VehicleInfo _currentMaintVehicle;
        private PropertyInfo _currentMaintProperty;
        private readonly Dictionary<string, string> _advisoryParts = new();
        private readonly Label[] _odometerLabels = new Label[6];
        private bool _odometerBuilt;

        public ProfileFace()
        {
            InitializeComponent();
            LoadData();
            UpdateBankStatus();
            Loaded += OnProfileLoaded;
        }

        private async void OnProfileLoaded(object sender, EventArgs e)
        {
            Loaded -= OnProfileLoaded;
            var p = UserProfileDataService.GetProfile();
            if (!string.IsNullOrWhiteSpace(p.SmartScanLicence) || p.SmartScanPromptShown) return;

            await Task.Delay(1200);
            p.SmartScanPromptShown = true;
            UserProfileDataService.SaveProfile();

            var page = Application.Current?.Windows.FirstOrDefault()?.Page;
            if (page == null) return;
            var wants = await page.DisplayAlert("Smart Scan",
                "Do you have a Smart Scan licence key? It lets SmartCube read insurance policies and documents automatically.\n\nYou can also enter it later from the Profile page.",
                "Enter Key", "Not Now");
            if (wants) await PromptSmartScanKey();
        }

        private void LoadSmartScan()
        {
            var key = UserProfileDataService.GetProfile().SmartScanLicence;
            if (string.IsNullOrWhiteSpace(key))
            {
                SmartScanStatusLabel.Text = "Not entered";
                SmartScanStatusLabel.TextColor = Color.FromArgb("#94A3B8");
                SmartScanKeyBtn.Text = "Enter Key";
            }
            else
            {
                SmartScanStatusLabel.Text = key;
                SmartScanStatusLabel.TextColor = Color.FromArgb("#22C55E");
                SmartScanKeyBtn.Text = "Change Key";
            }
        }

        private async void OnHelpClicked(object sender, EventArgs e)
        {
            var page = Application.Current?.Windows.FirstOrDefault()?.Page;
            if (page != null)
                await page.DisplayAlert("Profile & Household",
                    "This page holds your personal details, properties, vehicles, session and account settings. Upload a licence, passport or payslip photo and SmartCube reads the details for you; add properties and vehicles by postcode or registration to look them up automatically. Use Session to change your password, sign out or set up two-factor authentication, and Smart Scan to enter the licence key that lets SmartCube read documents automatically. All of this data is stored locally on this PC.",
                    "OK");
        }

        private async void OnSmartScanKeyClicked(object sender, EventArgs e) => await PromptSmartScanKey();

        private async void OnChangePasswordClicked(object sender, EventArgs e)
        {
            var nav = Application.Current?.Windows.FirstOrDefault()?.Page?.Navigation;
            if (nav != null) await nav.PushAsync(new ChangePasswordPage(forced: false));
        }

        private async void OnTwoFactorClicked(object sender, EventArgs e)
        {
            var nav = Application.Current?.Windows.FirstOrDefault()?.Page?.Navigation;
            if (nav == null) return;
            var page = new TwoFactorPage();
            page.Disappearing += (_, _) => RefreshTwoFactor();
            await nav.PushAsync(page);
        }

        private async void RefreshTwoFactor()
        {
            if (SessionService.Current == null)
            {
                TwoFactorLabel.Text = "Sign in to manage two-factor.";
                TwoFactorBtn.IsEnabled = false;
                return;
            }
            var (ok, error, enabled, codesLeft) = await SessionService.GetTwoFactorStatus();
            if (!ok)
            {
                TwoFactorLabel.Text = error;
                TwoFactorBtn.IsEnabled = false;
                return;
            }
            TwoFactorLabel.Text = enabled
                ? $"On · {codesLeft} recovery code{(codesLeft == 1 ? "" : "s")} left"
                : "Off · protect your account with an authenticator app";
            TwoFactorLabel.TextColor = enabled ? Color.FromArgb("#22C55E") : Color.FromArgb("#94A3B8");
            TwoFactorBtn.Text = enabled ? "Manage" : "Set Up";
            TwoFactorBtn.IsEnabled = true;
        }

        private async void OnChangePinClicked(object sender, EventArgs e)
        {
            var nav = Application.Current?.Windows.FirstOrDefault()?.Page?.Navigation;
            if (nav != null) await nav.PushAsync(new PinPage(PinPage.Mode.Change));
        }

        private async void OnSignOutClicked(object sender, EventArgs e)
        {
            var window = Application.Current?.Windows.FirstOrDefault();
            var page = window?.Page;
            if (page == null) return;
            var confirm = await page.DisplayAlert("Sign Out",
                "Sign out of SmartCube? This device will no longer be remembered.", "Sign Out", "Cancel");
            if (!confirm) return;

            SignOutBtn.IsEnabled = false;
            await SessionService.Logout();
            window.Page = new NavigationPage(new LoginPage());
        }

        private async Task PromptSmartScanKey()
        {
            var page = Application.Current?.Windows.FirstOrDefault()?.Page;
            if (page == null) return;
            var p = UserProfileDataService.GetProfile();
            var key = await page.DisplayPromptAsync("Smart Scan Licence",
                "Enter the licence key from your SmartCube account (Pro plan).",
                placeholder: "SC-XXXX-XXXX-XXXX", initialValue: p.SmartScanLicence ?? "", maxLength: 40);
            if (key == null) return;
            p.SmartScanLicence = string.IsNullOrWhiteSpace(key) ? null : key.Trim().ToUpperInvariant();
            UserProfileDataService.SaveProfile();
            LoadSmartScan();
        }

        private void LoadData()
        {
            var profile = MockDataService.GetUserProfile();

            ProfileName.Text = profile.DisplayName;
            ProfileEmail.Text = profile.Email;
            AvatarInitials.Text = SessionService.InitialsFor(profile.DisplayName);
            ProfileUsername.Text = profile.Username;
            ProfileCulture.Text = profile.CultureCode;
            ProfileCurrency.Text = profile.CurrencyCode;
            ProfileSince.Text = profile.AccountCreated.ToString("MMMM yyyy");
            LastLoginLabel.Text = profile.LastLogin.ToString("dd MMM yyyy HH:mm");
            TraceLabel.Text = profile.TraceEnabled ? "Enabled" : "Disabled";
            TraceLabel.TextColor = profile.TraceEnabled ? Color.FromArgb("#22C55E") : Color.FromArgb("#EF4444");
            RefreshTwoFactor();
            ChangePinBtn.IsVisible = !string.IsNullOrEmpty(SessionService.DeviceToken);

            LoadPersonalData();
            LoadProperties();
            LoadVehicles();
            LoadCubeFaces(profile);
            LoadMortgageData();
            LoadEmailStatus();
            LoadDocuments();
            LoadCreditScore();
            LoadSmartScan();
        }

        private void LoadPersonalData()
        {
            var p = UserProfileDataService.GetProfile();

            PersonalNameLabel.Text = string.IsNullOrEmpty(p.FullName) ? "—" : p.FullName;
            PersonalDobLabel.Text = string.IsNullOrEmpty(p.DateOfBirth) ? "—" : p.DateOfBirth;
            PersonalPhoneLabel.Text = string.IsNullOrEmpty(p.Phone) ? "—" : p.Phone;
            PersonalNiLabel.Text = string.IsNullOrEmpty(p.NiNumber) ? "—" : p.NiNumber;
            PersonalEmploymentLabel.Text = string.IsNullOrEmpty(p.EmploymentStatus) ? "—" : p.EmploymentStatus;
            PersonalEmployerLabel.Text = string.IsNullOrEmpty(p.Employer) ? "—" : p.Employer;
            PersonalIncomeLabel.Text = p.AnnualIncome > 0 ? p.AnnualIncome.ToString("C0", _gbp) : "—";
            PersonalTaxCodeLabel.Text = string.IsNullOrEmpty(p.TaxCode) ? "—" : p.TaxCode;
            PersonalGenderLabel.Text = string.IsNullOrEmpty(p.Gender) ? "—" : p.Gender;
            PersonalLicenceLabel.Text = string.IsNullOrEmpty(p.DrivingLicenceNumber) ? "—" : p.DrivingLicenceNumber;
            PersonalLicenceExpiryLabel.Text = string.IsNullOrEmpty(p.LicenceExpiryDate) ? "—" : p.LicenceExpiryDate;
            PersonalCategoriesLabel.Text = string.IsNullOrEmpty(p.DrivingCategories) ? "—" : p.DrivingCategories;
            PersonalPassportLabel.Text = string.IsNullOrEmpty(p.PassportNumber) ? "—" : p.PassportNumber;
            PersonalPassportExpiryLabel.Text = string.IsNullOrEmpty(p.PassportExpiry) ? "—" : p.PassportExpiry;
            PersonalNationalityLabel.Text = string.IsNullOrEmpty(p.Nationality) ? "—" : p.Nationality;
            PersonalPobLabel.Text = string.IsNullOrEmpty(p.PlaceOfBirth) ? "—" : p.PlaceOfBirth;
            PersonalCreditLabel.Text = string.IsNullOrEmpty(p.CreditScore) ? "—" : p.CreditScore;
            PersonalCreditProviderLabel.Text = string.IsNullOrEmpty(p.CreditScoreProvider) ? "—" : p.CreditScoreProvider;

            if (!string.IsNullOrEmpty(p.FullName))
            {
                var parts = p.FullName.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length >= 2)
                    AvatarInitials.Text = $"{parts[0][0]}{parts[^1][0]}".ToUpper();
                else if (parts.Length == 1)
                    AvatarInitials.Text = parts[0][..Math.Min(2, parts[0].Length)].ToUpper();
            }
        }

        private async void OnUploadLicenceClicked(object sender, EventArgs e)
        {
            var page = Application.Current.Windows[0].Page;

            try
            {
                var result = await FilePicker.PickAsync(new PickOptions
                {
                    PickerTitle = "Select driving licence photo",
                    FileTypes = FilePickerFileType.Images,
                });

                if (result == null) return;

                UploadLicenceBtn.IsEnabled = false;
                UploadLicenceBtn.Text = "Reading...";

                var (data, rawText, error) = await DrivingLicenceService.ExtractFromImage(result.FullPath);

                UploadLicenceBtn.IsEnabled = true;
                UploadLicenceBtn.Text = "Upload Licence";

                if (data == null)
                {
                    await page.DisplayAlert("Extraction Failed", error ?? "Unknown error", "OK");
                    return;
                }

                var summary = "";
                if (!string.IsNullOrEmpty(data.FullName)) summary += $"Name: {data.FullName}\n";
                else if (!string.IsNullOrEmpty(data.Surname)) summary += $"Surname: {data.Surname}\n";
                if (!string.IsNullOrEmpty(data.DateOfBirth)) summary += $"DOB: {data.DateOfBirth}\n";
                if (!string.IsNullOrEmpty(data.Gender)) summary += $"Gender: {data.Gender}\n";
                if (!string.IsNullOrEmpty(data.LicenceNumber)) summary += $"Licence: {data.LicenceNumber}\n";
                if (!string.IsNullOrEmpty(data.IssueDate)) summary += $"Issued: {data.IssueDate}\n";
                if (!string.IsNullOrEmpty(data.ExpiryDate)) summary += $"Expires: {data.ExpiryDate}\n";
                if (!string.IsNullOrEmpty(data.Address)) summary += $"Address: {data.Address}\n";
                if (!string.IsNullOrEmpty(data.Categories)) summary += $"Categories: {data.Categories}\n";

                if (string.IsNullOrWhiteSpace(summary))
                {
                    var preview = rawText?.Length > 400 ? rawText[..400] : rawText;
                    await page.DisplayAlert("No Data Found",
                        $"Could not extract licence fields.\n\nRaw OCR text:\n{preview}", "OK");
                    return;
                }

                var rawPreview = rawText?.Length > 300 ? rawText[..300] : rawText;
                summary += $"\n--- Raw OCR ---\n{rawPreview}";

                var confirm = await page.DisplayAlert("Licence Data Found", summary, "Apply to Profile", "Cancel");
                if (!confirm) return;

                DocumentStorageService.AddDocument(result.FullPath, "Licence", result.FileName);

                var p = UserProfileDataService.GetProfile();

                if (!string.IsNullOrEmpty(data.FullName))
                    p.FullName = data.FullName;
                else if (!string.IsNullOrEmpty(data.Surname))
                    p.FullName = data.Surname;

                if (!string.IsNullOrEmpty(data.DateOfBirth))
                    p.DateOfBirth = data.DateOfBirth;
                if (!string.IsNullOrEmpty(data.Gender))
                    p.Gender = data.Gender;
                if (!string.IsNullOrEmpty(data.LicenceNumber))
                    p.DrivingLicenceNumber = data.LicenceNumber;
                if (!string.IsNullOrEmpty(data.IssueDate))
                    p.LicenceIssueDate = data.IssueDate;
                if (!string.IsNullOrEmpty(data.ExpiryDate))
                    p.LicenceExpiryDate = data.ExpiryDate;
                if (!string.IsNullOrEmpty(data.Categories))
                    p.DrivingCategories = data.Categories;

                UserProfileDataService.SaveProfile();
                LoadPersonalData();
                LoadDocuments();
            }
            catch (Exception ex)
            {
                await page.DisplayAlert("Error", ex.Message, "OK");
            }
        }

        private async void OnUploadPassportClicked(object sender, EventArgs e)
        {
            var page = Application.Current.Windows[0].Page;

            try
            {
                var result = await FilePicker.PickAsync(new PickOptions
                {
                    PickerTitle = "Select passport photo",
                    FileTypes = FilePickerFileType.Images,
                });

                if (result == null) return;

                UploadPassportBtn.IsEnabled = false;
                UploadPassportBtn.Text = "Reading...";

                var (data, rawText, error) = await PassportService.ExtractFromImage(result.FullPath);

                UploadPassportBtn.IsEnabled = true;
                UploadPassportBtn.Text = "Passport";

                if (data == null)
                {
                    await page.DisplayAlert("Extraction Failed", error ?? "Unknown error", "OK");
                    return;
                }

                var summary = "";
                if (!string.IsNullOrEmpty(data.FullName)) summary += $"Name: {data.FullName}\n";
                if (!string.IsNullOrEmpty(data.DateOfBirth)) summary += $"DOB: {data.DateOfBirth}\n";
                if (!string.IsNullOrEmpty(data.Gender)) summary += $"Gender: {data.Gender}\n";
                if (!string.IsNullOrEmpty(data.Nationality)) summary += $"Nationality: {data.Nationality}\n";
                if (!string.IsNullOrEmpty(data.PassportNumber)) summary += $"Passport No: {data.PassportNumber}\n";
                if (!string.IsNullOrEmpty(data.IssueDate)) summary += $"Issued: {data.IssueDate}\n";
                if (!string.IsNullOrEmpty(data.ExpiryDate)) summary += $"Expires: {data.ExpiryDate}\n";
                if (!string.IsNullOrEmpty(data.PlaceOfBirth)) summary += $"Place of Birth: {data.PlaceOfBirth}\n";

                var rawPreview = rawText?.Length > 300 ? rawText[..300] : rawText;
                summary += $"\n--- Raw OCR ---\n{rawPreview}";

                if (string.IsNullOrWhiteSpace(summary.Split("--- Raw OCR ---")[0]))
                {
                    await page.DisplayAlert("No Data Found",
                        $"Could not extract passport fields.\n\nRaw OCR text:\n{rawPreview}", "OK");
                    return;
                }

                var confirm = await page.DisplayAlert("Passport Data Found", summary, "Apply to Profile", "Cancel");
                if (!confirm) return;

                DocumentStorageService.AddDocument(result.FullPath, "Passport", result.FileName);

                var p = UserProfileDataService.GetProfile();

                if (!string.IsNullOrEmpty(data.FullName))
                    p.FullName = data.FullName;
                if (!string.IsNullOrEmpty(data.DateOfBirth))
                    p.DateOfBirth = data.DateOfBirth;
                if (!string.IsNullOrEmpty(data.Gender))
                    p.Gender = data.Gender;
                if (!string.IsNullOrEmpty(data.PassportNumber))
                    p.PassportNumber = data.PassportNumber;
                if (!string.IsNullOrEmpty(data.ExpiryDate))
                    p.PassportExpiry = data.ExpiryDate;
                if (!string.IsNullOrEmpty(data.Nationality))
                    p.Nationality = data.Nationality;
                if (!string.IsNullOrEmpty(data.PlaceOfBirth))
                    p.PlaceOfBirth = data.PlaceOfBirth;

                UserProfileDataService.SaveProfile();
                LoadPersonalData();
                LoadDocuments();
            }
            catch (Exception ex)
            {
                await page.DisplayAlert("Error", ex.Message, "OK");
            }
        }

        private async void OnUploadPayslipClicked(object sender, EventArgs e)
        {
            var page = Application.Current.Windows[0].Page;

            try
            {
                var result = await FilePicker.PickAsync(new PickOptions
                {
                    PickerTitle = "Select payslip photo or scan",
                    FileTypes = FilePickerFileType.Images,
                });

                if (result == null) return;

                UploadPayslipBtn.IsEnabled = false;
                UploadPayslipBtn.Text = "Reading...";

                var (data, rawText, error) = await PayslipService.ExtractFromImage(result.FullPath);

                UploadPayslipBtn.IsEnabled = true;
                UploadPayslipBtn.Text = "Upload Payslip";

                if (data == null)
                {
                    await page.DisplayAlert("Extraction Failed", error ?? "Unknown error", "OK");
                    return;
                }

                var summary = "";
                if (!string.IsNullOrEmpty(data.NiNumber)) summary += $"NI Number: {data.NiNumber}\n";
                if (!string.IsNullOrEmpty(data.TaxCode)) summary += $"Tax Code: {data.TaxCode}\n";
                if (data.GrossPay > 0) summary += $"Gross Pay: {data.GrossPay:C2}\n";
                if (data.NetPay > 0) summary += $"Net Pay: {data.NetPay:C2}\n";
                if (data.TaxDeducted > 0) summary += $"Tax Deducted: {data.TaxDeducted:C2}\n";
                if (data.NiDeducted > 0) summary += $"NI Deducted: {data.NiDeducted:C2}\n";
                if (data.AnnualGross > 0) summary += $"Annual Gross: {data.AnnualGross:C0}\n";
                if (!string.IsNullOrEmpty(data.Employer)) summary += $"Employer: {data.Employer}\n";

                var rawPreview = rawText?.Length > 300 ? rawText[..300] : rawText;
                summary += $"\n--- Raw OCR ---\n{rawPreview}";

                if (string.IsNullOrWhiteSpace(summary.Split("--- Raw OCR ---")[0]))
                {
                    await page.DisplayAlert("No Data Found",
                        $"Could not extract payslip fields.\n\nRaw OCR text:\n{rawPreview}", "OK");
                    return;
                }

                var confirm = await page.DisplayAlert("Payslip Data Found", summary, "Apply to Profile", "Cancel");
                if (!confirm) return;

                DocumentStorageService.AddDocument(result.FullPath, "Payslip", result.FileName);

                var p = UserProfileDataService.GetProfile();

                if (!string.IsNullOrEmpty(data.NiNumber))
                    p.NiNumber = data.NiNumber;
                if (!string.IsNullOrEmpty(data.TaxCode))
                    p.TaxCode = data.TaxCode;
                if (data.AnnualGross > 0)
                    p.AnnualIncome = data.AnnualGross;
                else if (data.GrossPay > 0)
                    p.AnnualIncome = data.GrossPay * 12;
                if (!string.IsNullOrEmpty(data.Employer))
                    p.Employer = data.Employer;

                UserProfileDataService.SaveProfile();
                LoadPersonalData();
                LoadDocuments();
            }
            catch (Exception ex)
            {
                await page.DisplayAlert("Error", ex.Message, "OK");
            }
        }

        private async void OnEditPersonalClicked(object sender, EventArgs e)
        {
            var page = Application.Current.Windows[0].Page;
            var p = UserProfileDataService.GetProfile();

            string name = await page.DisplayPromptAsync("Full Name", "Your full name:", initialValue: p.FullName ?? "");
            if (name == null) return;
            p.FullName = name.Trim();

            string dob = await page.DisplayPromptAsync("Date of Birth", "DD/MM/YYYY:", initialValue: p.DateOfBirth ?? "");
            if (dob == null) return;
            p.DateOfBirth = dob.Trim();

            string phone = await page.DisplayPromptAsync("Phone", "Phone number:", initialValue: p.Phone ?? "", keyboard: Keyboard.Telephone);
            if (phone == null) return;
            p.Phone = phone.Trim();

            string ni = await page.DisplayPromptAsync("NI Number", "National Insurance number:", initialValue: p.NiNumber ?? "");
            if (ni == null) return;
            p.NiNumber = ni.Trim().ToUpper();

            string employment = await page.DisplayActionSheet("Employment Status", "Cancel", null,
                "Employed", "Self-Employed", "Part-Time", "Contractor", "Retired", "Student", "Unemployed");
            if (string.IsNullOrWhiteSpace(employment) || employment == "Cancel") return;
            p.EmploymentStatus = employment;

            string employer = await page.DisplayPromptAsync("Employer", "Employer name (if applicable):", initialValue: p.Employer ?? "");
            if (employer == null) return;
            p.Employer = employer.Trim();

            string income = await page.DisplayPromptAsync("Annual Income", "Gross annual income (£):", initialValue: p.AnnualIncome > 0 ? p.AnnualIncome.ToString("0") : "", keyboard: Keyboard.Numeric);
            if (income == null) return;
            decimal.TryParse(income, out var inc);
            p.AnnualIncome = inc;

            string taxCode = await page.DisplayPromptAsync("Tax Code", "HMRC tax code:", initialValue: p.TaxCode ?? "");
            if (taxCode == null) return;
            p.TaxCode = taxCode.Trim().ToUpper();

            string credit = await page.DisplayPromptAsync("Credit Score", "Your credit score:", initialValue: p.CreditScore ?? "", keyboard: Keyboard.Numeric);
            if (credit == null) return;
            p.CreditScore = credit.Trim();

            string creditProv = await page.DisplayActionSheet("Credit Score Provider", "Cancel", null,
                "Experian", "Equifax", "TransUnion", "ClearScore", "Credit Karma");
            if (string.IsNullOrWhiteSpace(creditProv) || creditProv == "Cancel") return;
            p.CreditScoreProvider = creditProv;

            UserProfileDataService.SaveProfile();
            LoadPersonalData();
        }

        private void LoadProperties()
        {
            var props = UserProfileDataService.GetProfile().Properties;
            PropertiesList.Children.Clear();
            PropertiesEmptyLabel.IsVisible = props.Count == 0;

            foreach (var prop in props)
            {
                var card = new Border
                {
                    BackgroundColor = Color.FromArgb("#1A2332"),
                    Stroke = Color.FromArgb("#1E293B"),
                    StrokeThickness = 1,
                    Padding = new Thickness(12, 10),
                    StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 8 },
                };

                var grid = new Grid
                {
                    ColumnDefinitions = new ColumnDefinitionCollection
                    {
                        new ColumnDefinition(new GridLength(32)),
                        new ColumnDefinition(GridLength.Star),
                        new ColumnDefinition(GridLength.Auto),
                        new ColumnDefinition(GridLength.Auto),
                    },
                    ColumnSpacing = 8,
                };

                var icon = new Label
                {
                    Text = prop.IsPrimary ? "\U0001F3E0" : "\U0001F3E2",
                    FontSize = 20,
                    VerticalOptions = LayoutOptions.Center,
                };

                var details = new VerticalStackLayout { Spacing = 2, VerticalOptions = LayoutOptions.Center };
                details.Add(new Label
                {
                    Text = prop.Address ?? "No address",
                    TextColor = Color.FromArgb("#E2E8F0"),
                    FontSize = 13,
                    FontAttributes = FontAttributes.Bold,
                });

                var subParts = new List<string>();
                if (!string.IsNullOrEmpty(prop.Postcode)) subParts.Add(prop.Postcode);
                if (!string.IsNullOrEmpty(prop.PropertyType)) subParts.Add(prop.PropertyType);
                if (prop.Bedrooms > 0) subParts.Add($"{prop.Bedrooms} bed");
                if (!string.IsNullOrEmpty(prop.Tenure)) subParts.Add(prop.Tenure);
                details.Add(new Label
                {
                    Text = subParts.Count > 0 ? string.Join(" · ", subParts) : "—",
                    TextColor = Color.FromArgb("#64748B"),
                    FontSize = 11,
                });

                var valParts = new List<string>();
                if (prop.EstimatedValue > 0) valParts.Add($"Value: {prop.EstimatedValue:C0}");
                if (!string.IsNullOrEmpty(prop.CouncilTaxBand)) valParts.Add($"Band {prop.CouncilTaxBand}");
                if (!string.IsNullOrEmpty(prop.EpcRating)) valParts.Add($"EPC {prop.EpcRating}");
                if (valParts.Count > 0)
                {
                    details.Add(new Label
                    {
                        Text = string.Join(" · ", valParts),
                        TextColor = Color.FromArgb("#94A3B8"),
                        FontSize = 11,
                    });
                }

                var removeBtn = new Button
                {
                    Text = "Remove",
                    BackgroundColor = Color.FromArgb("#3B1A1A"),
                    TextColor = Color.FromArgb("#EF4444"),
                    FontSize = 10,
                    CornerRadius = 6,
                    Padding = new Thickness(8, 2),
                    HeightRequest = 26,
                    VerticalOptions = LayoutOptions.Center,
                };
                ToolTipProperties.SetText(removeBtn, "Remove this property from your profile.");
                var propId = prop.Id;
                removeBtn.Clicked += async (s, ev) =>
                {
                    var confirm = await Application.Current.Windows[0].Page.DisplayAlert(
                        "Remove Property", $"Remove {prop.Address}?", "Remove", "Cancel");
                    if (confirm)
                    {
                        UserProfileDataService.RemoveProperty(propId);
                        LoadProperties();
                    }
                };

                var propMaintBtn = new Button
                {
                    Text = "Maint",
                    BackgroundColor = Color.FromArgb("#1E3A5F"),
                    TextColor = Color.FromArgb("#60A5FA"),
                    FontSize = 10,
                    CornerRadius = 6,
                    Padding = new Thickness(8, 2),
                    HeightRequest = 26,
                    VerticalOptions = LayoutOptions.Center,
                };
                ToolTipProperties.SetText(propMaintBtn, "Open maintenance details for this property: financials, maintenance log and contractors.");
                var propRef = prop;
                propMaintBtn.Clicked += (s, ev) => OnPropertyMaintenanceClicked(propRef);

                grid.Add(icon, 0);
                grid.Add(details, 1);
                grid.Add(propMaintBtn, 2);
                grid.Add(removeBtn, 3);
                card.Content = grid;
                PropertiesList.Children.Add(card);
            }
        }

        private async void OnAddPropertyClicked(object sender, EventArgs e)
        {
            var page = Application.Current.Windows[0].Page;

            string postcode = await page.DisplayPromptAsync("Add Property", "Enter postcode:", placeholder: "e.g. SK8 3HT");
            if (string.IsNullOrWhiteSpace(postcode)) return;
            postcode = postcode.Trim().ToUpper();

            LookupStatusLabel.Text = $"Looking up {postcode}...";
            LookupOverlay.IsVisible = true;
            AddPropertyBtn.IsEnabled = false;

            await Task.Delay(200);
            var (results, error) = await PropertyLookupService.SearchByPostcode(postcode, LookupWebView);

            LookupOverlay.IsVisible = false;
            AddPropertyBtn.IsEnabled = true;

            if (results == null || results.Count == 0)
            {
                var manual = await page.DisplayAlert("No Results", error ?? "No properties found. Add manually?", "Add Manually", "Cancel");
                if (!manual) return;

                string manualAddr = await page.DisplayPromptAsync("Address", "First line of address:", placeholder: "e.g. 5 Baron Green");
                if (string.IsNullOrWhiteSpace(manualAddr)) return;

                UserProfileDataService.AddProperty(new PropertyInfo
                {
                    Id = Guid.NewGuid().ToString("N")[..8],
                    Address = manualAddr.Trim(),
                    Postcode = postcode,
                    IsPrimary = UserProfileDataService.GetProfile().Properties.Count == 0,
                });
                LoadProperties();
                return;
            }

            var addressOptions = results.Select(r =>
                string.IsNullOrEmpty(r.EpcRating) ? r.Address : $"{r.Address} (EPC: {r.EpcRating})").ToArray();

            var picked = await page.DisplayActionSheet($"Select address ({results.Count} found)", "Cancel", null, addressOptions);
            if (string.IsNullOrWhiteSpace(picked) || picked == "Cancel") return;

            var selectedIndex = Array.IndexOf(addressOptions, picked);
            if (selectedIndex < 0) return;

            var selected = results[selectedIndex];

            UserProfileDataService.AddProperty(new PropertyInfo
            {
                Id = Guid.NewGuid().ToString("N")[..8],
                Address = selected.Address,
                Postcode = postcode,
                EpcRating = selected.EpcRating,
                CertificateUrl = selected.CertificateUrl,
                IsPrimary = UserProfileDataService.GetProfile().Properties.Count == 0,
            });

            LoadProperties();
        }

        private void LoadVehicles()
        {
            var vehicles = UserProfileDataService.GetProfile().Vehicles;
            VehiclesList.Children.Clear();
            VehiclesEmptyLabel.IsVisible = vehicles.Count == 0;

            foreach (var v in vehicles)
            {
                var card = new Border
                {
                    BackgroundColor = Color.FromArgb("#1A2332"),
                    Stroke = Color.FromArgb("#1E293B"),
                    StrokeThickness = 1,
                    Padding = new Thickness(12, 10),
                    StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 8 },
                };

                var grid = new Grid
                {
                    ColumnDefinitions = new ColumnDefinitionCollection
                    {
                        new ColumnDefinition(new GridLength(32)),
                        new ColumnDefinition(GridLength.Star),
                        new ColumnDefinition(GridLength.Auto),
                        new ColumnDefinition(GridLength.Auto),
                    },
                    ColumnSpacing = 8,
                };

                var icon = new Label
                {
                    Text = "\U0001F697",
                    FontSize = 20,
                    VerticalOptions = LayoutOptions.Center,
                };

                var details = new VerticalStackLayout { Spacing = 2, VerticalOptions = LayoutOptions.Center };
                details.Add(new Label
                {
                    Text = $"{v.Make} {v.Model}",
                    TextColor = Color.FromArgb("#E2E8F0"),
                    FontSize = 13,
                    FontAttributes = FontAttributes.Bold,
                });

                var subParts = new List<string>();
                if (!string.IsNullOrEmpty(v.Registration)) subParts.Add(v.Registration);
                if (v.Year > 0) subParts.Add(v.Year.ToString());
                if (!string.IsNullOrEmpty(v.FuelType)) subParts.Add(v.FuelType);
                if (!string.IsNullOrEmpty(v.Colour)) subParts.Add(v.Colour);
                details.Add(new Label
                {
                    Text = subParts.Count > 0 ? string.Join(" · ", subParts) : "—",
                    TextColor = Color.FromArgb("#64748B"),
                    FontSize = 11,
                });

                var infoParts = new List<string>();
                if (v.Mileage > 0) infoParts.Add($"{v.Mileage:N0} miles");
                if (!string.IsNullOrEmpty(v.MotExpiry)) infoParts.Add($"MOT: {v.MotExpiry}");
                if (!string.IsNullOrEmpty(v.TaxExpiry)) infoParts.Add($"Tax: {v.TaxExpiry}");
                if (v.EstimatedValue > 0) infoParts.Add($"Value: {v.EstimatedValue:C0}");
                if (infoParts.Count > 0)
                {
                    details.Add(new Label
                    {
                        Text = string.Join(" · ", infoParts),
                        TextColor = Color.FromArgb("#94A3B8"),
                        FontSize = 11,
                    });
                }

                var maintBtn = new Button
                {
                    Text = "Maint",
                    BackgroundColor = Color.FromArgb("#1E3A5F"),
                    TextColor = Color.FromArgb("#60A5FA"),
                    FontSize = 10,
                    CornerRadius = 6,
                    Padding = new Thickness(8, 2),
                    HeightRequest = 26,
                    VerticalOptions = LayoutOptions.Center,
                };
                var vehicleRef = v;
                maintBtn.Clicked += (s, ev) => OnMaintenanceClicked(vehicleRef);
                ToolTipProperties.SetText(maintBtn, "Open maintenance details for this vehicle: MOT status, advisories, mileage and garage.");

                var removeBtn = new Button
                {
                    Text = "Remove",
                    BackgroundColor = Color.FromArgb("#3B1A1A"),
                    TextColor = Color.FromArgb("#EF4444"),
                    FontSize = 10,
                    CornerRadius = 6,
                    Padding = new Thickness(8, 2),
                    HeightRequest = 26,
                    VerticalOptions = LayoutOptions.Center,
                };
                ToolTipProperties.SetText(removeBtn, "Remove this vehicle from your profile.");
                var vId = v.Id;
                removeBtn.Clicked += async (s, ev) =>
                {
                    var confirm = await Application.Current.Windows[0].Page.DisplayAlert(
                        "Remove Vehicle", $"Remove {v.Make} {v.Model}?", "Remove", "Cancel");
                    if (confirm)
                    {
                        UserProfileDataService.RemoveVehicle(vId);
                        LoadVehicles();
                    }
                };

                grid.Add(icon, 0);
                grid.Add(details, 1);
                grid.Add(maintBtn, 2);
                grid.Add(removeBtn, 3);
                card.Content = grid;
                VehiclesList.Children.Add(card);
            }
        }

        private async void OnAddVehicleClicked(object sender, EventArgs e)
        {
            var page = Application.Current.Windows[0].Page;

            string reg = await page.DisplayPromptAsync("Add Vehicle", "Enter registration number:", placeholder: "e.g. AB12 CDE");
            if (string.IsNullOrWhiteSpace(reg)) return;
            reg = reg.Replace(" ", "").Trim().ToUpper();

            LookupStatusLabel.Text = $"Looking up {reg}...";
            LookupOverlay.IsVisible = true;
            AddVehicleBtn.IsEnabled = false;

            await Task.Delay(200);
            var (data, error) = await VehicleLookupService.LookupRegistration(reg, LookupWebView);

            LookupOverlay.IsVisible = false;
            AddVehicleBtn.IsEnabled = true;

            if (data == null)
            {
                await page.DisplayAlert("Lookup Failed", error, "OK");
                return;
            }

            var summary = $"Make: {data.Make}\n" +
                $"Model: {data.Model}\n" +
                $"Year: {data.Year}\n" +
                $"Colour: {data.Colour}\n" +
                $"Fuel: {data.FuelType}\n" +
                (data.EngineCC > 0 ? $"Engine: {data.EngineCC}cc\n" : "") +
                (data.Mileage > 0 ? $"Mileage: {data.Mileage:N0}\n" : "") +
                (!string.IsNullOrEmpty(data.MotExpiry) ? $"MOT Expires: {data.MotExpiry}\n" : "") +
                (!string.IsNullOrEmpty(data.FirstRegistered) ? $"Registered: {data.FirstRegistered}" : "");

            var proceed = await page.DisplayAlert($"{reg} Found", summary, "Add Vehicle", "Cancel");
            if (!proceed) return;

            UserProfileDataService.AddVehicle(new VehicleInfo
            {
                Id = Guid.NewGuid().ToString("N")[..8],
                Registration = reg,
                Make = data.Make ?? "",
                Model = data.Model ?? "",
                Year = data.Year,
                FuelType = data.FuelType ?? "",
                Colour = data.Colour ?? "",
                Mileage = data.Mileage,
                MotExpiry = data.MotExpiry ?? "",
                TaxExpiry = "",
                EstimatedValue = 0,
            });

            LoadVehicles();
        }


        private void OnMaintenanceClicked(VehicleInfo vehicle)
        {
            _currentMaintVehicle = vehicle;
            MaintenanceOverlay.IsVisible = true;
            MaintTitle.Text = $"{vehicle.Make} {vehicle.Model} — {vehicle.Registration}";
            LoadMaintenanceData();
        }

        private void OnCloseMaintenanceClicked(object sender, EventArgs e)
        {
            MaintenanceOverlay.IsVisible = false;
            _currentMaintVehicle = null;
        }

        private void OnPropertyMaintenanceClicked(PropertyInfo property)
        {
            _currentMaintProperty = property;
            PropMaintOverlay.IsVisible = true;
            PropMaintTitle.Text = $"Property — {property.Address}";
            LoadPropertyMaintenanceData();
        }

        private void OnClosePropMaintClicked(object sender, EventArgs e)
        {
            PropMaintOverlay.IsVisible = false;
            _currentMaintProperty = null;
        }

        private void LoadPropertyMaintenanceData()
        {
            var p = _currentMaintProperty;
            if (p == null) return;

            PropMaintAddress.Text = string.IsNullOrEmpty(p.Address) ? "—" : p.Address;
            PropMaintPostcode.Text = string.IsNullOrEmpty(p.Postcode) ? "—" : p.Postcode;
            PropMaintType.Text = string.IsNullOrEmpty(p.PropertyType) ? "—" : p.PropertyType;
            PropMaintBedrooms.Text = p.Bedrooms > 0 ? p.Bedrooms.ToString() : "—";
            PropMaintTenure.Text = string.IsNullOrEmpty(p.Tenure) ? "—" : p.Tenure;
            PropMaintPrimary.Text = p.IsPrimary ? "Yes" : "No";

            PropMaintPurchasePrice.Text = p.PurchasePrice > 0 ? p.PurchasePrice.ToString("C0", _gbp) : "—";
            PropMaintPurchaseDate.Text = string.IsNullOrEmpty(p.PurchaseDate) ? "—" : p.PurchaseDate;
            PropMaintEstValue.Text = p.EstimatedValue > 0 ? p.EstimatedValue.ToString("C0", _gbp) : "—";
            PropMaintCouncilTax.Text = string.IsNullOrEmpty(p.CouncilTaxBand) ? "—" : $"Band {p.CouncilTaxBand}";
            PropMaintLastSale.Text = p.LastSalePrice > 0
                ? p.LastSalePrice.ToString("C0", _gbp) + (string.IsNullOrEmpty(p.LastSaleDate) ? "" : $" ({p.LastSaleDate})")
                : "—";

            var epc = string.IsNullOrEmpty(p.EpcRating) ? "—" : p.EpcRating;
            PropMaintEpcLabel.Text = $"EPC Rating: {epc}";
            PropMaintEpcExpiry.Text = string.IsNullOrEmpty(p.EpcExpiry) ? "" : $"Valid until {p.EpcExpiry}";
            var epcColor = epc switch
            {
                "A" => "#0A3D2E",
                "B" => "#0A3D2E",
                "C" => "#1A3D1A",
                "D" => "#3B3B1A",
                "E" => "#3B2E1A",
                "F" => "#3B1A1A",
                "G" => "#3B1A1A",
                _ => "#1A2332",
            };
            PropMaintEpcBadge.BackgroundColor = Color.FromArgb(epcColor);

            var hasBuildingDetails = !string.IsNullOrEmpty(p.FloorArea) || !string.IsNullOrEmpty(p.WallsType)
                || !string.IsNullOrEmpty(p.HeatingType);
            BuildingDetailsCard.IsVisible = hasBuildingDetails;
            if (hasBuildingDetails)
            {
                PropMaintFloorArea.Text = string.IsNullOrEmpty(p.FloorArea) ? "—" : p.FloorArea;
                PropMaintWalls.Text = string.IsNullOrEmpty(p.WallsType) ? "—" : p.WallsType;
                PropMaintRoof.Text = string.IsNullOrEmpty(p.RoofType) ? "—" : p.RoofType;
                PropMaintHeating.Text = string.IsNullOrEmpty(p.HeatingType) ? "—" : p.HeatingType;
                PropMaintWindows.Text = string.IsNullOrEmpty(p.WindowsType) ? "—" : p.WindowsType;
                PropMaintHotWater.Text = string.IsNullOrEmpty(p.HotWater) ? "—" : p.HotWater;
            }

            PropMaintLogList.Children.Clear();
            PropMaintLogEmpty.IsVisible = p.MaintenanceHistory.Count == 0;

            foreach (var entry in p.MaintenanceHistory.OrderByDescending(e => e.Date))
            {
                var row = new Grid
                {
                    ColumnDefinitions = new ColumnDefinitionCollection
                    {
                        new ColumnDefinition(GridLength.Auto),
                        new ColumnDefinition(GridLength.Star),
                        new ColumnDefinition(GridLength.Auto),
                        new ColumnDefinition(GridLength.Auto),
                    },
                    ColumnSpacing = 8,
                    Padding = new Thickness(0, 4),
                };
                row.Add(new Label
                {
                    Text = entry.Date,
                    TextColor = Color.FromArgb("#94A3B8"),
                    FontSize = 11,
                    VerticalOptions = LayoutOptions.Center,
                }, 0);
                row.Add(new Label
                {
                    Text = $"{entry.Description}" + (string.IsNullOrEmpty(entry.Category) ? "" : $" ({entry.Category})"),
                    TextColor = Color.FromArgb("#E2E8F0"),
                    FontSize = 12,
                    VerticalOptions = LayoutOptions.Center,
                }, 1);
                row.Add(new Label
                {
                    Text = entry.Cost > 0 ? entry.Cost.ToString("C0", _gbp) : "",
                    TextColor = Color.FromArgb("#34D399"),
                    FontSize = 12,
                    FontAttributes = FontAttributes.Bold,
                    VerticalOptions = LayoutOptions.Center,
                }, 2);

                var removeBtn = new Button
                {
                    Text = "X",
                    BackgroundColor = Color.FromArgb("#3B1A1A"),
                    TextColor = Color.FromArgb("#EF4444"),
                    FontSize = 9,
                    CornerRadius = 4,
                    Padding = new Thickness(6, 0),
                    HeightRequest = 22,
                    VerticalOptions = LayoutOptions.Center,
                };
                ToolTipProperties.SetText(removeBtn, "Remove this maintenance entry.");
                var entryId = entry.Id;
                removeBtn.Clicked += async (s, ev) =>
                {
                    var confirm = await Application.Current.Windows[0].Page.DisplayAlert(
                        "Remove Entry", $"Remove \"{entry.Description}\"?", "Remove", "Cancel");
                    if (confirm)
                    {
                        _currentMaintProperty.MaintenanceHistory.RemoveAll(e => e.Id == entryId);
                        UserProfileDataService.SaveProfile();
                        LoadPropertyMaintenanceData();
                    }
                };
                row.Add(removeBtn, 3);
                PropMaintLogList.Children.Add(row);
            }

            ContractorList.Children.Clear();
            ContractorEmpty.IsVisible = p.Contractors.Count == 0;

            foreach (var c in p.Contractors.OrderBy(c => c.Trade))
            {
                var row = new Grid
                {
                    ColumnDefinitions = new ColumnDefinitionCollection
                    {
                        new ColumnDefinition(GridLength.Auto),
                        new ColumnDefinition(GridLength.Star),
                        new ColumnDefinition(GridLength.Auto),
                        new ColumnDefinition(GridLength.Auto),
                    },
                    ColumnSpacing = 8,
                    Padding = new Thickness(0, 4),
                };
                row.Add(new Border
                {
                    BackgroundColor = Color.FromArgb("#1E3A5F"),
                    StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 4 },
                    Stroke = Colors.Transparent,
                    Padding = new Thickness(6, 2),
                    Content = new Label
                    {
                        Text = c.Trade ?? "",
                        TextColor = Color.FromArgb("#60A5FA"),
                        FontSize = 10,
                        FontAttributes = FontAttributes.Bold,
                    },
                    VerticalOptions = LayoutOptions.Center,
                }, 0);
                row.Add(new VerticalStackLayout
                {
                    Children =
                    {
                        new Label { Text = c.Name ?? "", TextColor = Color.FromArgb("#E2E8F0"), FontSize = 12 },
                        new Label { Text = string.Join("  ", new[] { c.Phone, c.Email }.Where(s => !string.IsNullOrEmpty(s))),
                                    TextColor = Color.FromArgb("#94A3B8"), FontSize = 10 },
                    },
                    VerticalOptions = LayoutOptions.Center,
                }, 1);

                var phoneBtn = new Button
                {
                    Text = "Call",
                    BackgroundColor = Color.FromArgb("#0A3D2E"),
                    TextColor = Color.FromArgb("#34D399"),
                    FontSize = 9,
                    CornerRadius = 4,
                    Padding = new Thickness(6, 0),
                    HeightRequest = 22,
                    VerticalOptions = LayoutOptions.Center,
                    IsVisible = !string.IsNullOrEmpty(c.Phone),
                };
                ToolTipProperties.SetText(phoneBtn, "Call this contractor.");
                var phone = c.Phone;
                phoneBtn.Clicked += async (s, ev) =>
                {
                    try { await Launcher.Default.OpenAsync(new Uri($"tel:{phone}")); } catch { }
                };
                row.Add(phoneBtn, 2);

                var removeBtn = new Button
                {
                    Text = "X",
                    BackgroundColor = Color.FromArgb("#3B1A1A"),
                    TextColor = Color.FromArgb("#EF4444"),
                    FontSize = 9,
                    CornerRadius = 4,
                    Padding = new Thickness(6, 0),
                    HeightRequest = 22,
                    VerticalOptions = LayoutOptions.Center,
                };
                ToolTipProperties.SetText(removeBtn, "Remove this contractor.");
                var cId = c.Id;
                removeBtn.Clicked += async (s, ev) =>
                {
                    var confirm = await Application.Current.Windows[0].Page.DisplayAlert(
                        "Remove Contractor", $"Remove \"{c.Name}\"?", "Remove", "Cancel");
                    if (confirm)
                    {
                        _currentMaintProperty.Contractors.RemoveAll(x => x.Id == cId);
                        UserProfileDataService.SaveProfile();
                        LoadPropertyMaintenanceData();
                    }
                };
                row.Add(removeBtn, 3);
                ContractorList.Children.Add(row);
            }
        }

        private async void OnAddContractorClicked(object sender, EventArgs e)
        {
            if (_currentMaintProperty == null) return;
            var page = Application.Current.Windows[0].Page;

            var trade = await page.DisplayActionSheet("Trade", "Cancel", null,
                "Electrician", "Plumber", "Painter/Decorator", "Roofer", "Builder",
                "Joiner/Carpenter", "Heating Engineer", "Gardener/Landscaper",
                "Locksmith", "Plasterer", "Tiler", "Cleaner", "General Handyman");
            if (string.IsNullOrWhiteSpace(trade) || trade == "Cancel") return;

            var name = await page.DisplayPromptAsync("Contractor Name", $"Name of {trade.ToLower()}:",
                placeholder: "e.g. John Smith");
            if (string.IsNullOrWhiteSpace(name)) return;

            var phone = await page.DisplayPromptAsync("Phone", "Phone number (optional):",
                keyboard: Keyboard.Telephone, placeholder: "", initialValue: "");

            var email = await page.DisplayPromptAsync("Email", "Email address (optional):",
                keyboard: Keyboard.Email, placeholder: "", initialValue: "");

            _currentMaintProperty.Contractors.Add(new ContractorInfo
            {
                Id = Guid.NewGuid().ToString("N")[..8],
                Name = name.Trim(),
                Trade = trade,
                Phone = phone?.Trim(),
                Email = email?.Trim(),
            });
            UserProfileDataService.SaveProfile();
            LoadPropertyMaintenanceData();
        }

        private async void OnRefreshPropDetailsClicked(object sender, EventArgs e)
        {
            if (_currentMaintProperty == null) return;
            var p = _currentMaintProperty;

            RefreshPropBtn.IsEnabled = false;
            PropRefreshSpinner.IsVisible = true;
            PropRefreshSpinner.IsRunning = true;

            var (valResult, valErr) = await PropertyLookupService.FetchPropertyValue(p.Address, p.Postcode, PropLookupWebView,
                showCaptcha => MainThread.BeginInvokeOnMainThread(() =>
                {
                    PropLookupPanel.IsVisible = showCaptcha;
                    PropMaintStatus.Text = "Please complete the verification below...";
                }));
            if (valResult != null)
            {
                if (valResult.EstimatedValue > 0) p.EstimatedValue = valResult.EstimatedValue;
                if (valResult.LastSalePrice > 0) p.LastSalePrice = valResult.LastSalePrice;
                if (!string.IsNullOrEmpty(valResult.LastSaleDate)) p.LastSaleDate = valResult.LastSaleDate;
                if (!string.IsNullOrEmpty(valResult.PropertyType)) p.PropertyType = valResult.PropertyType;
                if (!string.IsNullOrEmpty(valResult.Bedrooms) && int.TryParse(valResult.Bedrooms, out var beds)) p.Bedrooms = beds;
                if (!string.IsNullOrEmpty(valResult.Tenure)) p.Tenure = valResult.Tenure;
                if (!string.IsNullOrEmpty(valResult.CouncilTaxBand)) p.CouncilTaxBand = valResult.CouncilTaxBand;
                if (!string.IsNullOrEmpty(valResult.EpcRating)) p.EpcRating = valResult.EpcRating;
                if (!string.IsNullOrEmpty(valResult.FloorArea) && string.IsNullOrEmpty(p.FloorArea)) p.FloorArea = valResult.FloorArea;
                if (valResult.PriceHistory.Count > 0)
                {
                    var first = valResult.PriceHistory.Last();
                    if (p.PurchasePrice == 0 && first.Price > 0)
                    {
                        p.PurchasePrice = first.Price;
                        p.PurchaseDate = first.Date;
                    }
                }
                UserProfileDataService.SaveProfile();
            }

            if (!string.IsNullOrEmpty(p.CertificateUrl))
            {
                var (details, err) = await PropertyLookupService.FetchCertificateDetails(p.CertificateUrl, PropLookupWebView);
                if (details != null)
                    ApplyEpcDetails(p, details);
            }
            else
            {
                var (results, searchErr) = await PropertyLookupService.SearchByPostcode(p.Postcode, PropLookupWebView);
                if (results != null)
                {
                    var match = results.FirstOrDefault(r =>
                        r.Address.Contains(p.Address.Split(',')[0].Trim(), StringComparison.OrdinalIgnoreCase));
                    if (match == null) match = results.FirstOrDefault();
                    if (match != null)
                    {
                        p.CertificateUrl = match.CertificateUrl;
                        if (string.IsNullOrEmpty(p.EpcRating) && !string.IsNullOrEmpty(match.EpcRating))
                            p.EpcRating = match.EpcRating;

                        if (!string.IsNullOrEmpty(match.CertificateUrl))
                        {
                            var (details, detErr) = await PropertyLookupService.FetchCertificateDetails(match.CertificateUrl, PropLookupWebView);
                            if (details != null)
                                ApplyEpcDetails(p, details);
                        }
                        UserProfileDataService.SaveProfile();
                    }
                }
            }

            PropLookupPanel.IsVisible = false;
            PropRefreshSpinner.IsRunning = false;
            PropRefreshSpinner.IsVisible = false;
            RefreshPropBtn.IsEnabled = true;
            LoadPropertyMaintenanceData();
            LoadProperties();
        }

        private void ApplyEpcDetails(PropertyInfo p, PropertyDetailResult details)
        {
            if (!string.IsNullOrEmpty(details.EpcRating)) p.EpcRating = details.EpcRating;
            if (string.IsNullOrEmpty(p.PropertyType) && !string.IsNullOrEmpty(details.PropertyType))
                p.PropertyType = details.PropertyType;
            if (string.IsNullOrEmpty(p.FloorArea) && !string.IsNullOrEmpty(details.TotalFloorArea))
                p.FloorArea = details.TotalFloorArea;
            if (string.IsNullOrEmpty(p.WallsType) && !string.IsNullOrEmpty(details.WallsType))
                p.WallsType = details.WallsType;
            if (string.IsNullOrEmpty(p.RoofType) && !string.IsNullOrEmpty(details.RoofType))
                p.RoofType = details.RoofType;
            if (string.IsNullOrEmpty(p.HeatingType) && !string.IsNullOrEmpty(details.HeatingType))
                p.HeatingType = details.HeatingType;
            if (string.IsNullOrEmpty(p.WindowsType) && !string.IsNullOrEmpty(details.WindowsType))
                p.WindowsType = details.WindowsType;
            if (string.IsNullOrEmpty(p.HotWater) && !string.IsNullOrEmpty(details.HotWater))
                p.HotWater = details.HotWater;
            if (string.IsNullOrEmpty(p.Lighting) && !string.IsNullOrEmpty(details.Lighting))
                p.Lighting = details.Lighting;
            if (string.IsNullOrEmpty(p.EpcExpiry) && !string.IsNullOrEmpty(details.EpcExpiry))
                p.EpcExpiry = details.EpcExpiry;
            UserProfileDataService.SaveProfile();
        }

        private async void OnAddPropMaintEntryClicked(object sender, EventArgs e)
        {
            if (_currentMaintProperty == null) return;
            var page = Application.Current.Windows[0].Page;

            var category = await page.DisplayActionSheet("Category", "Cancel", null,
                "Plumbing", "Electrical", "Roofing", "Heating", "Garden",
                "Decorating", "Structural", "Appliance", "General");
            if (string.IsNullOrWhiteSpace(category) || category == "Cancel") return;

            var desc = await page.DisplayPromptAsync("Description", "What was done?",
                placeholder: "e.g. Boiler serviced");
            if (string.IsNullOrWhiteSpace(desc)) return;

            var costStr = await page.DisplayPromptAsync("Cost", "How much? (optional)",
                keyboard: Keyboard.Numeric, placeholder: "0", initialValue: "");
            decimal.TryParse(costStr, out var cost);

            _currentMaintProperty.MaintenanceHistory.Add(new PropertyMaintenanceEntry
            {
                Id = Guid.NewGuid().ToString("N")[..8],
                Date = DateTime.Now.ToString("dd/MM/yyyy"),
                Description = desc.Trim(),
                Cost = cost,
                Category = category,
            });
            UserProfileDataService.SaveProfile();
            LoadPropertyMaintenanceData();
        }

        private void LoadMaintenanceData()
        {
            var v = _currentMaintVehicle;
            if (v == null) return;

            // MOT status
            if (!string.IsNullOrEmpty(v.MotExpiry))
            {
                MotExpiryLabel.Text = $"MOT expires: {v.MotExpiry}";
                if (DateTime.TryParse(v.MotExpiry, out var motDate) ||
                    DateTime.TryParseExact(v.MotExpiry, new[] { "dd MMMM yyyy", "d MMMM yyyy", "dd/MM/yyyy" },
                        null, System.Globalization.DateTimeStyles.None, out motDate))
                {
                    var daysLeft = (motDate - DateTime.Now).Days;
                    if (daysLeft < 0)
                    {
                        MotCountdownLabel.Text = $"EXPIRED {Math.Abs(daysLeft)} days ago";
                        MotCountdownBadge.BackgroundColor = Color.FromArgb("#3B1A1A");
                        MotStatusIcon.Text = "⚠️";
                    }
                    else if (daysLeft <= 30)
                    {
                        MotCountdownLabel.Text = $"{daysLeft} days remaining — book now";
                        MotCountdownBadge.BackgroundColor = Color.FromArgb("#3B2E1A");
                        MotStatusIcon.Text = "⚠️";
                    }
                    else
                    {
                        MotCountdownLabel.Text = $"{daysLeft} days remaining";
                        MotCountdownBadge.BackgroundColor = Color.FromArgb("#0A3D2E");
                        MotStatusIcon.Text = "✅";
                    }
                }
                else
                {
                    MotCountdownLabel.Text = "";
                    MotStatusIcon.Text = "";
                }
            }
            else
            {
                MotExpiryLabel.Text = "MOT expires: —";
                MotCountdownLabel.Text = "Use Refresh MOT to fetch from DVSA";
                MotCountdownBadge.BackgroundColor = Color.FromArgb("#1C2744");
                MotStatusIcon.Text = "";
            }

            MaintTestDateLabel.Text = string.IsNullOrEmpty(v.MotTestDate) ? "—" : v.MotTestDate;
            MaintTestResultLabel.Text = string.IsNullOrEmpty(v.MotResult) ? "—" : v.MotResult;
            if (!string.IsNullOrEmpty(v.MotResult))
            {
                MaintTestResultLabel.TextColor = v.MotResult.Contains("Pass", StringComparison.OrdinalIgnoreCase)
                    ? Color.FromArgb("#22C55E") : Color.FromArgb("#EF4444");
            }

            // Reminder status
            if (!string.IsNullOrEmpty(v.MotReminderDate))
            {
                MotReminderLabel.Text = $"Reminder set for {v.MotReminderDate}";
                MotReminderLabel.IsVisible = true;
            }
            else
            {
                MotReminderLabel.IsVisible = false;
            }

            // Advisories
            AdvisoriesList.Children.Clear();
            AdvisoriesEmptyLabel.IsVisible = v.MotAdvisories.Count == 0;
            GetAllEstimatesBtn.IsVisible = v.MotAdvisories.Count > 0;
            foreach (var adv in v.MotAdvisories)
            {
                var row = new Border
                {
                    BackgroundColor = Color.FromArgb("#1C2744"),
                    StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 6 },
                    Stroke = Colors.Transparent,
                    Padding = new Thickness(10, 8),
                };
                var btnStack = new VerticalStackLayout { Spacing = 3 };
                var estimateBtn = new Button
                {
                    Text = "Get Estimate",
                    BackgroundColor = Color.FromArgb("#1E3A5F"),
                    TextColor = Color.FromArgb("#60A5FA"),
                    FontSize = 9, FontAttributes = FontAttributes.Bold,
                    CornerRadius = 4, Padding = new Thickness(6, 2),
                    HeightRequest = 22,
                    CommandParameter = adv,
                };
                estimateBtn.Clicked += OnGetEstimateClicked;
                ToolTipProperties.SetText(estimateBtn, "Email your preferred garage asking for an estimate to fix this advisory.");
                var getPartBtn = new Button
                {
                    Text = _advisoryParts.ContainsKey(adv) ? _advisoryParts[adv] : "Get Part",
                    BackgroundColor = _advisoryParts.ContainsKey(adv) ? Color.FromArgb("#0A3D2E") : Color.FromArgb("#3B2E1A"),
                    TextColor = _advisoryParts.ContainsKey(adv) ? Color.FromArgb("#22C55E") : Color.FromArgb("#F59E0B"),
                    FontSize = 9, FontAttributes = FontAttributes.Bold,
                    CornerRadius = 4, Padding = new Thickness(6, 2),
                    HeightRequest = 22,
                    CommandParameter = adv,
                };
                getPartBtn.Clicked += OnGetPartClicked;
                ToolTipProperties.SetText(getPartBtn, "Search online for a part number matching this advisory.");
                btnStack.Children.Add(estimateBtn);
                btnStack.Children.Add(getPartBtn);
                row.Content = new Grid
                {
                    ColumnDefinitions = new ColumnDefinitionCollection
                    {
                        new ColumnDefinition(GridLength.Auto),
                        new ColumnDefinition(GridLength.Star),
                        new ColumnDefinition(GridLength.Auto),
                    },
                    ColumnSpacing = 8,
                    Children =
                    {
                        new Label { Text = "⚠️", FontSize = 14, VerticalOptions = LayoutOptions.Start },
                    }
                };
                var advLabel = new Label { Text = adv, TextColor = Color.FromArgb("#F59E0B"), FontSize = 11,
                            VerticalOptions = LayoutOptions.Center, LineBreakMode = LineBreakMode.WordWrap };
                Grid.SetColumn(advLabel, 1);
                Grid.SetColumn(btnStack, 2);
                ((Grid)row.Content).Children.Add(advLabel);
                ((Grid)row.Content).Children.Add(btnStack);
                AdvisoriesList.Children.Add(row);
            }

            // Vehicle diagram
            UpdateDiagramMarkers(v.MotAdvisories);

            // Mileage
            MileageList.Children.Clear();
            var currentMiles = v.Mileage;
            if (v.MileageHistory.Count > 0)
            {
                var latest = v.MileageHistory.OrderByDescending(m => m.Miles).First();
                currentMiles = latest.Miles;
            }
            BuildOdometer();
            SetOdometerValue((int)currentMiles);
            MileageEmptyLabel.IsVisible = v.MileageHistory.Count == 0;

            foreach (var entry in v.MileageHistory.OrderByDescending(m => m.Miles))
            {
                var row = new Grid
                {
                    ColumnDefinitions = new ColumnDefinitionCollection
                    {
                        new ColumnDefinition(GridLength.Auto),
                        new ColumnDefinition(GridLength.Star),
                        new ColumnDefinition(GridLength.Auto),
                    },
                    ColumnSpacing = 10,
                    Padding = new Thickness(0, 4),
                };
                row.Add(new Label
                {
                    Text = entry.Date,
                    TextColor = Color.FromArgb("#94A3B8"),
                    FontSize = 12,
                    VerticalOptions = LayoutOptions.Center,
                }, 0);
                row.Add(new Label
                {
                    Text = string.IsNullOrEmpty(entry.Notes) ? "" : entry.Notes,
                    TextColor = Color.FromArgb("#64748B"),
                    FontSize = 11,
                    VerticalOptions = LayoutOptions.Center,
                }, 1);
                row.Add(new Label
                {
                    Text = $"{entry.Miles:N0} mi",
                    TextColor = Color.FromArgb("#E2E8F0"),
                    FontSize = 13,
                    FontAttributes = FontAttributes.Bold,
                    VerticalOptions = LayoutOptions.Center,
                }, 2);
                MileageList.Children.Add(row);
            }

            // Garages
            GarageList.Children.Clear();
            GarageEmptyLabel.IsVisible = v.PreferredGarages.Count == 0;
            foreach (var g in v.PreferredGarages)
            {
                var gRow = new Border
                {
                    BackgroundColor = Color.FromArgb("#1C2744"),
                    StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 6 },
                    Stroke = Colors.Transparent,
                    Padding = new Thickness(10, 8),
                };
                var editBtn = new Button
                {
                    Text = "Edit",
                    BackgroundColor = Color.FromArgb("#1E3A5F"),
                    TextColor = Color.FromArgb("#60A5FA"),
                    FontSize = 9, FontAttributes = FontAttributes.Bold,
                    CornerRadius = 4, Padding = new Thickness(8, 2),
                    HeightRequest = 22,
                    VerticalOptions = LayoutOptions.Start,
                    CommandParameter = g.Id,
                };
                editBtn.Clicked += OnEditGarageClicked;
                ToolTipProperties.SetText(editBtn, "Edit this garage's name, address, phone and contact details.");
                var removeBtn = new Button
                {
                    Text = "✕",
                    BackgroundColor = Colors.Transparent,
                    TextColor = Color.FromArgb("#EF4444"),
                    FontSize = 12,
                    Padding = new Thickness(4, 0),
                    HeightRequest = 22, WidthRequest = 22,
                    VerticalOptions = LayoutOptions.Start,
                    CommandParameter = g.Id,
                };
                removeBtn.Clicked += OnRemoveGarageClicked;
                ToolTipProperties.SetText(removeBtn, "Remove this garage.");
                var details = new VerticalStackLayout { Spacing = 2 };
                details.Children.Add(new Label { Text = g.Name, TextColor = Color.FromArgb("#E2E8F0"), FontSize = 13, FontAttributes = FontAttributes.Bold });
                details.Children.Add(new Label { Text = g.Address, TextColor = Color.FromArgb("#94A3B8"), FontSize = 11, LineBreakMode = LineBreakMode.WordWrap });
                if (!string.IsNullOrEmpty(g.Phone))
                    details.Children.Add(new Label { Text = g.Phone, TextColor = Color.FromArgb("#60A5FA"), FontSize = 11 });
                if (!string.IsNullOrEmpty(g.ContactName))
                    details.Children.Add(new Label { Text = $"Contact: {g.ContactName}", TextColor = Color.FromArgb("#94A3B8"), FontSize = 10 });
                if (!string.IsNullOrEmpty(g.ContactEmail))
                    details.Children.Add(new Label { Text = g.ContactEmail, TextColor = Color.FromArgb("#60A5FA"), FontSize = 10 });
                var btnStack = new VerticalStackLayout { Spacing = 4 };
                btnStack.Children.Add(editBtn);
                btnStack.Children.Add(removeBtn);
                var gGrid = new Grid
                {
                    ColumnDefinitions = new ColumnDefinitionCollection
                    {
                        new ColumnDefinition(GridLength.Star),
                        new ColumnDefinition(GridLength.Auto),
                    },
                    ColumnSpacing = 6,
                };
                gGrid.Add(details, 0);
                gGrid.Add(btnStack, 1);
                gRow.Content = gGrid;
                GarageList.Children.Add(gRow);
            }
        }

        private void UpdateDiagramMarkers(List<string> advisories)
        {
            MarkerNF.IsVisible = MarkerOF.IsVisible = MarkerNR.IsVisible = MarkerOR.IsVisible = false;
            MarkerNFIcon.IsVisible = MarkerOFIcon.IsVisible = MarkerNRIcon.IsVisible = MarkerORIcon.IsVisible = false;
            WheelNF.Stroke = WheelOF.Stroke = WheelNR.Stroke = WheelOR.Stroke = Color.FromArgb("#2A3A5C");

            if (advisories == null || advisories.Count == 0)
            {
                DiagramCard.IsVisible = false;
                return;
            }

            DiagramCard.IsVisible = true;
            foreach (var adv in advisories)
            {
                var upper = adv.ToUpperInvariant();
                bool nearside = upper.Contains("NEARSIDE") || upper.Contains("N/S");
                bool offside = upper.Contains("OFFSIDE") || upper.Contains("O/S");
                bool front = upper.Contains("FRONT");
                bool rear = upper.Contains("REAR");
                if (!front && !rear) rear = true;

                var amber = Color.FromArgb("#F59E0B");

                if (nearside && front) { MarkerNF.IsVisible = true; MarkerNFIcon.IsVisible = true; WheelNF.Stroke = amber; }
                if (offside && front) { MarkerOF.IsVisible = true; MarkerOFIcon.IsVisible = true; WheelOF.Stroke = amber; }
                if (nearside && rear) { MarkerNR.IsVisible = true; MarkerNRIcon.IsVisible = true; WheelNR.Stroke = amber; }
                if (offside && rear) { MarkerOR.IsVisible = true; MarkerORIcon.IsVisible = true; WheelOR.Stroke = amber; }
            }
        }

        private static string ExtractPartTerms(string advisory)
        {
            var stripped = System.Text.RegularExpressions.Regex.Replace(advisory, @"\([^)]*\)", "").Trim();
            stripped = System.Text.RegularExpressions.Regex.Replace(stripped, @"^(Advisory|Dangerous|Major|Minor)\s*-\s*", "", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            var stopWords = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "has","have","had","is","are","was","were","the","a","an","to","of","in","on","at","for","and","or","but",
                "not","with","from","by","its","it","be","been","being","this","that","which","who","whom","whose",
                "slight","slightly","minor","major","dangerous","advisory","worn","close","legal","limit","edge",
                "excessive","play","movement","deteriorated","corroded",
                "contaminated","insecure","leaking","cracked","broken","missing","loose","noisy","binding",
                "operating","incorrectly","condition","such","does","may","could","would","should","very"
            };
            var words = stripped.Split(new[] { ' ', ',', '/', '-' }, StringSplitOptions.RemoveEmptyEntries);
            var parts = new List<string>();
            foreach (var w in words)
            {
                var clean = w.Trim();
                if (clean.Length < 3) continue;
                if (stopWords.Contains(clean)) continue;
                if (System.Text.RegularExpressions.Regex.IsMatch(clean, @"^\d")) continue;
                parts.Add(clean);
            }
            return string.Join(" ", parts.Take(4));
        }

        private async void OnGetPartClicked(object sender, EventArgs e)
        {
            if (_currentMaintVehicle == null || sender is not Button btn) return;
            var advisory = btn.CommandParameter as string ?? "";
            var v = _currentMaintVehicle;

            var searchTerms = ExtractPartTerms(advisory);
            if (string.IsNullOrWhiteSpace(searchTerms)) return;

            btn.IsEnabled = false;
            btn.Text = "Searching...";

            FindPartEntry.Text = searchTerms;
            await DoPartSearch();

            var foundParts = new List<string>();
            foreach (var child in PartSearchResults.Children)
            {
                if (child is Border b && b.Content is VerticalStackLayout st && st.Children.Count > 0 &&
                    st.Children[0] is Label lbl)
                {
                    foundParts.Add(lbl.Text);
                }
            }

            btn.IsEnabled = true;

            if (foundParts.Count > 0)
            {
                var page = Application.Current.Windows[0].Page;
                string selected;
                if (foundParts.Count == 1)
                {
                    bool use = await page.DisplayAlert("Part Found", $"Part number: {foundParts[0]}\n\nUse this part number?", "Yes", "No");
                    selected = use ? foundParts[0] : null;
                }
                else
                {
                    selected = await page.DisplayActionSheet("Select Part Number", "Cancel", null, foundParts.ToArray());
                    if (selected == "Cancel") selected = null;
                }

                if (!string.IsNullOrEmpty(selected))
                {
                    _advisoryParts[advisory] = selected;
                    LoadMaintenanceData();
                    return;
                }
            }

            btn.Text = _advisoryParts.ContainsKey(advisory) ? _advisoryParts[advisory] : "Get Part";
        }

        private async void OnGetAllEstimatesClicked(object sender, EventArgs e)
        {
            if (_currentMaintVehicle == null) return;
            var v = _currentMaintVehicle;
            var page = Application.Current.Windows[0].Page;

            if (v.PreferredGarages.Count == 0)
            {
                await page.DisplayAlert("Get All Estimates",
                    "No preferred garage set. Add a garage first.", "OK");
                return;
            }
            var garage = v.PreferredGarages[0];
            if (string.IsNullOrEmpty(garage.ContactEmail))
            {
                await page.DisplayAlert("Get All Estimates",
                    $"No email set for {garage.Name}. Edit the garage to add a contact email.", "OK");
                return;
            }

            await SendEstimateEmail(garage, v, v.MotAdvisories);
        }

        private async void OnGetEstimateClicked(object sender, EventArgs e)
        {
            if (_currentMaintVehicle == null || sender is not Button btn) return;
            var advisory = btn.CommandParameter as string ?? "";
            var v = _currentMaintVehicle;
            var page = Application.Current.Windows[0].Page;

            if (v.PreferredGarages.Count == 0)
            {
                await page.DisplayAlert("Get Estimate",
                    "No preferred garage set. Add a garage first, then request an estimate.", "OK");
                return;
            }
            var garage = v.PreferredGarages[0];
            if (string.IsNullOrEmpty(garage.ContactEmail))
            {
                await page.DisplayAlert("Get Estimate",
                    $"No email set for {garage.Name}. Edit the garage to add a contact email.", "OK");
                return;
            }

            await SendEstimateEmail(garage, v, new List<string> { advisory });
        }

        private async Task SendEstimateEmail(GarageInfo garage, VehicleInfo v, List<string> advisories)
        {
            var page = Application.Current.Windows[0].Page;
            var single = advisories.Count == 1;
            var subject = single
                ? $"Estimate Request - {v.Registration} {v.Make} {v.Model}"
                : $"Estimate Request ({advisories.Count} items) - {v.Registration} {v.Make} {v.Model}";

            var body = $"Hi {(string.IsNullOrEmpty(garage.ContactName) ? "" : garage.ContactName)},\n\n" +
                       $"I'd like to request {(single ? "an estimate" : "estimates")} for the following MOT advisor{(single ? "y" : "ies")} on my vehicle:\n\n" +
                       $"Vehicle: {v.Registration} - {v.Make} {v.Model} ({v.Year})\n" +
                       $"Mileage: {v.Mileage:N0}\n\n";

            for (int i = 0; i < advisories.Count; i++)
            {
                var adv = advisories[i];
                body += $"{(single ? "Advisory" : $"{i + 1})")}: {adv}\n";
                if (_advisoryParts.TryGetValue(adv, out var partNo))
                    body += $"   Part Number: {partNo}\n";
                body += "\n";
            }

            body += $"Could you please provide {(single ? "an estimate" : "estimates")} for the repair{(single ? "" : "s")}?\n\nMany thanks";

            var mailto = $"mailto:{Uri.EscapeDataString(garage.ContactEmail)}" +
                         $"?subject={Uri.EscapeDataString(subject)}" +
                         $"&body={Uri.EscapeDataString(body)}";
            try
            {
                await Launcher.Default.OpenAsync(new Uri(mailto));
            }
            catch
            {
                await Clipboard.Default.SetTextAsync($"{subject}\n\n{body}");
                await page.DisplayAlert("Email Unavailable",
                    "Could not open email client. Estimate details copied to clipboard.", "OK");
            }
        }

        private void OnFindPartSearch(object sender, EventArgs e) => _ = DoPartSearch();
        private void OnFindPartSearchClicked(object sender, EventArgs e) => _ = DoPartSearch();

        private async Task DoPartSearch()
        {
            var query = FindPartEntry?.Text?.Trim();
            if (string.IsNullOrWhiteSpace(query) || _currentMaintVehicle == null) return;

            var v = _currentMaintVehicle;
            FindPartBtn.IsEnabled = false;
            FindPartBtn.Text = "Searching...";
            PartSearchResults.Children.Clear();
            PartSearchResults.Children.Add(new Label { Text = "Looking up parts...", TextColor = Color.FromArgb("#94A3B8"), FontSize = 11 });

            var fullQuery = $"{v.Make} {v.Model} {v.Year} {query} OEM part number";
            var url = $"https://www.google.com/search?q={Uri.EscapeDataString(fullQuery)}";

            try
            {
                var navTcs = new TaskCompletionSource<bool>();
                void onNav(object s, WebNavigatedEventArgs e2) => navTcs.TrySetResult(true);
                LookupWebView.Navigated += onNav;
                LookupWebView.Source = new UrlWebViewSource { Url = url };
                using var cts = new CancellationTokenSource(20000);
                try { await navTcs.Task.WaitAsync(cts.Token); } catch (OperationCanceledException) { }
                LookupWebView.Navigated -= onNav;
                await Task.Delay(3000);

                var extractJs = @"(function(){
    var body = document.body.innerText;
    if (!body || body.length < 100) return 'WAITING';
    var partNums = [];
    var seen = {};
    var pnRe = /\b([A-Z]{2,6}[\-\s]?[A-Z0-9]{3,15}(?:[\-\/][A-Z0-9]{1,8})?)\b/g;
    var skipWords = /^(http|https|www|com|org|html|Search|Google|About|More|Next|Back|View|Sign|Free|Home|Cart|Page|Images|Maps|News|Shop|Books|None|Your|This|That|From|With|What|Have|Just|Will|Also|Some|Than|When|They|Each|Find|Like|Only|Over|Such|Into|Year|Many|Then|Make|Made|Used|Part|Parts|Type|Best|Good|High|Sale|Cost|Price|Click|Here)$/i;
    var lines = body.split('\n');
    var results = [];
    for (var i = 0; i < lines.length; i++) {
        var line = lines[i].trim();
        if (line.length < 10) continue;
        var m;
        pnRe.lastIndex = 0;
        while ((m = pnRe.exec(line)) !== null) {
            var pn = m[1].replace(/\s+/g, '');
            if (pn.length < 5 || pn.length > 20) continue;
            if (skipWords.test(pn)) continue;
            if (!/\d/.test(pn)) continue;
            if (!/[A-Z]/i.test(pn)) continue;
            var key = pn.toUpperCase();
            if (seen[key]) continue;
            seen[key] = true;
            var ctx = line.substring(Math.max(0, m.index - 60), Math.min(line.length, m.index + pn.length + 60)).trim();
            if (ctx.length > 120) ctx = ctx.substring(0, 120) + '...';
            results.push(pn + '|||' + ctx);
            if (results.length >= 8) break;
        }
        if (results.length >= 8) break;
    }
    return results.length > 0 ? results.join('###') : 'NONE';
})()";

                string resultStr = null;
#if WINDOWS
                var nativeWebView = LookupWebView.Handler?.PlatformView as Microsoft.UI.Xaml.Controls.WebView2;
                if (nativeWebView?.CoreWebView2 == null)
                    try { await nativeWebView?.EnsureCoreWebView2Async(); } catch { }
                if (nativeWebView?.CoreWebView2 != null)
                {
                    for (int i = 0; i < 10; i++)
                    {
                        await Task.Delay(1500);
                        try
                        {
                            var raw = await nativeWebView.CoreWebView2.ExecuteScriptAsync(extractJs);
                            if (!string.IsNullOrEmpty(raw) && raw != "null" && raw != "\"\"")
                            {
                                resultStr = raw.Trim('"').Replace("\\u0027", "'");
                                if (resultStr == "WAITING") continue;
                                break;
                            }
                        }
                        catch { }
                    }
                }
#endif
                try { LookupWebView.Source = new UrlWebViewSource { Url = "about:blank" }; } catch { }

                PartSearchResults.Children.Clear();

                if (string.IsNullOrEmpty(resultStr) || resultStr == "NONE" || resultStr == "WAITING")
                {
                    PartSearchResults.Children.Add(new Label { Text = "No part numbers found. Try a different search term.", TextColor = Color.FromArgb("#94A3B8"), FontSize = 11 });
                }
                else
                {
                    var parts = resultStr.Split("###", StringSplitOptions.RemoveEmptyEntries);
                    foreach (var part in parts)
                    {
                        var fields = part.Split("|||");
                        var partNo = fields.Length > 0 ? fields[0] : "";
                        var context = fields.Length > 1 ? fields[1] : "";

                        var partRow = new Border
                        {
                            BackgroundColor = Color.FromArgb("#1C2744"),
                            StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 6 },
                            Stroke = Colors.Transparent,
                            Padding = new Thickness(10, 6),
                        };
                        var stack = new VerticalStackLayout { Spacing = 2 };
                        stack.Children.Add(new Label { Text = partNo, TextColor = Color.FromArgb("#22C55E"), FontSize = 12, FontAttributes = FontAttributes.Bold });
                        if (!string.IsNullOrEmpty(context))
                            stack.Children.Add(new Label { Text = context, TextColor = Color.FromArgb("#94A3B8"), FontSize = 10, LineBreakMode = LineBreakMode.WordWrap });
                        partRow.Content = stack;
                        PartSearchResults.Children.Add(partRow);
                    }
                }
            }
            catch
            {
                PartSearchResults.Children.Clear();
                PartSearchResults.Children.Add(new Label { Text = "Search failed. Please try again.", TextColor = Color.FromArgb("#EF4444"), FontSize = 11 });
            }

            FindPartBtn.IsEnabled = true;
            FindPartBtn.Text = "Search";
        }

        private async void OnAddGarageClicked(object sender, EventArgs e)
        {
            if (_currentMaintVehicle == null) return;
            var page = Application.Current.Windows[0].Page;

            var postcode = await page.DisplayPromptAsync("Add Garage", "Enter garage postcode:",
                placeholder: "e.g. SK8 3PD");
            if (string.IsNullOrWhiteSpace(postcode)) return;

            string addressList = null;
            try
            {
                using var http = new HttpClient();
                var json = await http.GetStringAsync($"https://api.postcodes.io/postcodes/{Uri.EscapeDataString(postcode.Trim())}");
                var obj = Newtonsoft.Json.Linq.JObject.Parse(json);
                if ((int?)obj["status"] == 200)
                {
                    var result = obj["result"];
                    var ward = (string)result?["admin_ward"] ?? "";
                    var parish = (string)result?["parish"] ?? "";
                    var district = (string)result?["admin_district"] ?? "";
                    addressList = string.Join(", ", new[] { ward, parish, district }.Where(s => !string.IsNullOrEmpty(s)));
                }
            }
            catch { }

            var name = await page.DisplayPromptAsync("Garage Name", "Enter garage name:",
                placeholder: "e.g. Kwik Fit Cheadle");
            if (string.IsNullOrWhiteSpace(name)) return;

            var address = await page.DisplayPromptAsync("Garage Address",
                !string.IsNullOrEmpty(addressList) ? $"Area: {addressList}\nEnter full address:" : "Enter full address:",
                placeholder: "e.g. 123 High Street");
            if (string.IsNullOrWhiteSpace(address)) return;

            var phone = await page.DisplayPromptAsync("Phone (optional)", "Enter phone number:",
                keyboard: Keyboard.Telephone, placeholder: "e.g. 0161 123 4567");

            _currentMaintVehicle.PreferredGarages.Add(new GarageInfo
            {
                Id = Guid.NewGuid().ToString("N")[..8],
                Name = name.Trim(),
                Address = $"{address.Trim()}, {postcode.Trim().ToUpper()}",
                Postcode = postcode.Trim().ToUpper(),
                Phone = phone?.Trim(),
            });
            UserProfileDataService.SaveProfile();
            LoadMaintenanceData();
        }

        private async void OnEditGarageClicked(object sender, EventArgs e)
        {
            if (_currentMaintVehicle == null || sender is not Button btn) return;
            var id = btn.CommandParameter as string;
            var garage = _currentMaintVehicle.PreferredGarages.FirstOrDefault(g => g.Id == id);
            if (garage == null) return;
            var page = Application.Current.Windows[0].Page;

            var name = await page.DisplayPromptAsync("Edit Garage", "Garage name:", initialValue: garage.Name);
            if (name != null) garage.Name = name.Trim();

            var address = await page.DisplayPromptAsync("Edit Garage", "Address:", initialValue: garage.Address);
            if (address != null) garage.Address = address.Trim();

            var phone = await page.DisplayPromptAsync("Edit Garage", "Phone:", initialValue: garage.Phone ?? "", keyboard: Keyboard.Telephone);
            if (phone != null) garage.Phone = phone.Trim();

            var contactName = await page.DisplayPromptAsync("Edit Garage", "Contact name:", initialValue: garage.ContactName ?? "");
            if (contactName != null) garage.ContactName = contactName.Trim();

            var contactEmail = await page.DisplayPromptAsync("Edit Garage", "Contact email:", initialValue: garage.ContactEmail ?? "", keyboard: Keyboard.Email);
            if (contactEmail != null) garage.ContactEmail = contactEmail.Trim();

            UserProfileDataService.SaveProfile();
            LoadMaintenanceData();
        }

        private void OnRemoveGarageClicked(object sender, EventArgs e)
        {
            if (_currentMaintVehicle == null || sender is not Button btn) return;
            var id = btn.CommandParameter as string;
            _currentMaintVehicle.PreferredGarages.RemoveAll(g => g.Id == id);
            UserProfileDataService.SaveProfile();
            LoadMaintenanceData();
        }

        private void BuildOdometer()
        {
            if (_odometerBuilt) return;
            _odometerBuilt = true;

            OdometerGrid.ColumnDefinitions.Clear();
            OdometerGrid.Children.Clear();

            for (int i = 0; i < 6; i++)
            {
                OdometerGrid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Auto));

                var lbl = new Label
                {
                    Text = "0",
                    FontSize = 22,
                    FontFamily = "Consolas",
                    FontAttributes = FontAttributes.Bold,
                    TextColor = Color.FromArgb("#E2E8F0"),
                    HorizontalTextAlignment = TextAlignment.Center,
                    VerticalTextAlignment = TextAlignment.Center,
                    WidthRequest = 26,
                    HeightRequest = 34,
                };

                var digitBorder = new Border
                {
                    BackgroundColor = Color.FromArgb("#1A2744"),
                    Stroke = Color.FromArgb("#2A3A5C"),
                    StrokeThickness = 1,
                    StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 4 },
                    Padding = new Thickness(0),
                    Content = lbl,
                };

                _odometerLabels[i] = lbl;
                OdometerGrid.Add(digitBorder, i, 0);
            }

#if WINDOWS
            OdometerContainer.Loaded += (s, e) =>
            {
                Dispatcher.Dispatch(async () =>
                {
                    await Task.Delay(200);
                    if (OdometerContainer.Handler?.PlatformView is Microsoft.UI.Xaml.UIElement native)
                    {
                        native.AddHandler(
                            Microsoft.UI.Xaml.UIElement.PointerWheelChangedEvent,
                            new Microsoft.UI.Xaml.Input.PointerEventHandler((sender, args) =>
                            {
                                var pt = args.GetCurrentPoint(native);
                                var x = pt.Position.X;
                                var w = native.ActualSize.X;
                                if (w <= 0) return;
                                var idx = (int)(x / (w / 6));
                                if (idx < 0) idx = 0;
                                if (idx > 5) idx = 5;
                                var delta = pt.Properties.MouseWheelDelta;
                                var cur = int.Parse(_odometerLabels[idx].Text);
                                if (delta > 0) cur = (cur + 1) % 10;
                                else if (delta < 0) cur = (cur + 9) % 10;
                                _odometerLabels[idx].Text = cur.ToString();
                                args.Handled = true;
                            }),
                            true);
                    }
                });
            };
#endif
        }

        private void SetOdometerValue(int value)
        {
            var s = value.ToString().PadLeft(6, '0');
            if (s.Length > 6) s = s[^6..];
            for (int i = 0; i < 6; i++)
                _odometerLabels[i].Text = s[i].ToString();
        }

        private int GetOdometerValue()
        {
            var s = "";
            for (int i = 0; i < 6; i++)
                s += _odometerLabels[i].Text;
            return int.TryParse(s, out var v) ? v : 0;
        }

        private void OnSetMileageClicked(object sender, EventArgs e)
        {
            if (_currentMaintVehicle == null) return;

            var mileage = GetOdometerValue();
            if (mileage <= 0) return;

            _currentMaintVehicle.MileageHistory.Add(new MileageEntry
            {
                Date = DateTime.Now.ToString("dd/MM/yyyy"),
                Miles = mileage,
                Notes = "Odometer",
            });
            _currentMaintVehicle.Mileage = mileage;
            UserProfileDataService.SaveProfile();
            LoadMaintenanceData();
            LoadVehicles();
        }

        private async void OnSetMotReminderClicked(object sender, EventArgs e)
        {
            if (_currentMaintVehicle == null) return;
            var page = Application.Current.Windows[0].Page;

            if (string.IsNullOrEmpty(_currentMaintVehicle.MotExpiry))
            {
                await page.DisplayAlert("MOT Reminder", "No MOT expiry date available. Use Refresh MOT first.", "OK");
                return;
            }

            DateTime motDate;
            if (!DateTime.TryParseExact(_currentMaintVehicle.MotExpiry,
                new[] { "dd/MM/yyyy", "dd MMMM yyyy", "d MMMM yyyy" },
                null, System.Globalization.DateTimeStyles.None, out motDate) &&
                !DateTime.TryParse(_currentMaintVehicle.MotExpiry, out motDate))
            {
                await page.DisplayAlert("MOT Reminder", "Could not parse MOT expiry date.", "OK");
                return;
            }

            var reminderDate = motDate.AddDays(-21);
            if (reminderDate <= DateTime.Now)
            {
                await page.DisplayAlert("MOT Reminder",
                    $"MOT is due within 3 weeks or already expired. Book your MOT now!", "OK");
                return;
            }

            _currentMaintVehicle.MotReminderDate = reminderDate.ToString("dd/MM/yyyy");
            UserProfileDataService.SaveProfile();

#if WINDOWS
            try
            {
                var xml = $@"<toast launch=""mot-reminder"" scenario=""reminder"">
  <visual>
    <binding template=""ToastGeneric"">
      <text>MOT Reminder</text>
      <text>Your MOT for {_currentMaintVehicle.Registration} expires on {_currentMaintVehicle.MotExpiry}. Book your MOT now!</text>
    </binding>
  </visual>
</toast>";
                var doc = new Windows.Data.Xml.Dom.XmlDocument();
                doc.LoadXml(xml);
                var scheduled = new Windows.UI.Notifications.ScheduledToastNotification(doc,
                    new DateTimeOffset(reminderDate.Year, reminderDate.Month, reminderDate.Day, 9, 0, 0, TimeSpan.Zero));
                scheduled.Id = $"mot-{_currentMaintVehicle.Registration}";
                Windows.UI.Notifications.ToastNotificationManager.CreateToastNotifier().AddToSchedule(scheduled);
            }
            catch { }
#endif

            await page.DisplayAlert("Reminder Set",
                $"You'll be reminded on {reminderDate:dd/MM/yyyy} (3 weeks before MOT expires on {_currentMaintVehicle.MotExpiry}).", "OK");
            LoadMaintenanceData();
        }

        private async void OnRefreshMotClicked(object sender, EventArgs e)
        {
            if (_currentMaintVehicle == null) return;

            RefreshMotBtn.IsEnabled = false;
            RefreshMotBtn.Text = "Fetching...";

            var reg = _currentMaintVehicle.Registration;
            var (testDate, result, motExpiry, mileage, advisories, error) =
                await VehicleLookupService.FetchMotDetails(reg, LookupWebView);

            RefreshMotBtn.IsEnabled = true;
            RefreshMotBtn.Text = "Refresh MOT";

            if (error != null)
            {
                await Application.Current.Windows[0].Page.DisplayAlert("MOT Refresh",
                    $"Could not fetch MOT data: {error}", "OK");
                return;
            }

            if (!string.IsNullOrEmpty(testDate))
                _currentMaintVehicle.MotTestDate = testDate;
            if (!string.IsNullOrEmpty(result))
                _currentMaintVehicle.MotResult = result;
            if (!string.IsNullOrEmpty(motExpiry))
                _currentMaintVehicle.MotExpiry = motExpiry;
            if (mileage > 0)
            {
                _currentMaintVehicle.Mileage = mileage;
                var hasExisting = _currentMaintVehicle.MileageHistory.Any(m => m.Notes == "MOT Test" && m.Miles == mileage);
                if (!hasExisting)
                {
                    _currentMaintVehicle.MileageHistory.Add(new MileageEntry
                    {
                        Date = testDate ?? DateTime.Now.ToString("dd/MM/yyyy"),
                        Miles = mileage,
                        Notes = "MOT Test",
                    });
                }
            }
            if (advisories.Count > 0)
                _currentMaintVehicle.MotAdvisories = advisories;

            UserProfileDataService.SaveProfile();
            LoadMaintenanceData();
            LoadVehicles();
        }

        private void LoadCubeFaces(MockUserProfile profile)
        {
            CubeFacesContainer.Children.Clear();
            foreach (var f in profile.CubeFaces.OrderBy(f => f.ScreenOrder))
            {
                var row = new Border
                {
                    BackgroundColor = Color.FromArgb("#1C2744"),
                    StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 8 },
                    Stroke = Colors.Transparent,
                    Padding = new Thickness(14, 10),
                };
                var grid = new Grid
                {
                    ColumnDefinitions = new ColumnDefinitionCollection
                    {
                        new ColumnDefinition(new GridLength(36)),
                        new ColumnDefinition(GridLength.Star),
                        new ColumnDefinition(GridLength.Auto),
                        new ColumnDefinition(GridLength.Auto),
                    },
                    ColumnSpacing = 12,
                };
                grid.Add(new Label { Text = f.Icon, FontSize = 20, VerticalOptions = LayoutOptions.Center, HorizontalOptions = LayoutOptions.Center }, 0);
                var info = new VerticalStackLayout { Spacing = 2, VerticalOptions = LayoutOptions.Center };
                info.Add(new Label { Text = f.Name, TextColor = Color.FromArgb("#F1F5F9"), FontSize = 14, FontAttributes = FontAttributes.Bold });
                info.Add(new Label { Text = $"Face Code: {f.Code}", TextColor = Color.FromArgb("#64748B"), FontSize = 11 });
                grid.Add(info, 1);
                grid.Add(new Label { Text = $"#{f.ScreenOrder}", TextColor = Color.FromArgb("#94A3B8"), FontSize = 12, VerticalOptions = LayoutOptions.Center }, 2);
                grid.Add(new Label
                {
                    Text = f.IsActive ? "Active" : "Inactive",
                    TextColor = Color.FromArgb(f.IsActive ? "#22C55E" : "#64748B"),
                    FontSize = 12,
                    FontAttributes = FontAttributes.Bold,
                    VerticalOptions = LayoutOptions.Center,
                }, 3);
                row.Content = grid;
                CubeFacesContainer.Children.Add(row);
            }
        }

        private void LoadEmailStatus()
        {
            var settings = EmailBillScanner.LoadSettings();
            if (settings != null && !string.IsNullOrEmpty(settings.EmailAddress))
            {
                EmailStatusIcon.Text = "\U0001F7E2";
                EmailStatusLabel.Text = $"Connected — {settings.EmailAddress}";
                EmailStatusLabel.TextColor = Color.FromArgb("#22C55E");
                EmailAddressEntry.Text = settings.EmailAddress;
                EmailPasswordEntry.Text = settings.AppPassword;
                EmailImapEntry.Text = settings.ImapServer;
                EmailFormPanel.IsVisible = false;
                ConnectEmailBtn.IsVisible = false;
                ScanEmailBtn.IsVisible = true;
                DisconnectEmailBtn.IsVisible = true;

                if (settings.LastScanDate.HasValue)
                    EmailStatusLabel.Text += $" · Last scan: {settings.LastScanDate:dd MMM yyyy HH:mm}";
            }
        }

        private void OnEmailAddressChanged(object sender, TextChangedEventArgs e)
        {
            var email = EmailAddressEntry.Text?.Trim();
            if (string.IsNullOrEmpty(email) || !email.Contains('@')) return;
            var (server, _) = EmailBillScanner.DetectImapServer(email);
            if (!string.IsNullOrEmpty(server))
                EmailImapEntry.Text = server;
        }

        private async void OnConnectEmailClicked(object sender, EventArgs e)
        {
            var email = EmailAddressEntry.Text?.Trim();
            var password = EmailPasswordEntry.Text?.Trim();
            var server = EmailImapEntry.Text?.Trim();

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                await Application.Current.Windows[0].Page.DisplayAlert("Missing Info", "Enter your email and app password.", "OK");
                return;
            }

            if (string.IsNullOrEmpty(server))
            {
                var (detected, _) = EmailBillScanner.DetectImapServer(email);
                server = detected;
                EmailImapEntry.Text = server;
            }

            if (string.IsNullOrEmpty(server))
            {
                await Application.Current.Windows[0].Page.DisplayAlert("IMAP Server", "Could not detect your IMAP server. Please enter it manually.", "OK");
                return;
            }

            ConnectEmailBtn.IsEnabled = false;
            ConnectEmailBtn.Text = "Testing...";

            var settings = new EmailSettings
            {
                EmailAddress = email,
                AppPassword = password,
                ImapServer = server,
                ImapPort = 993,
            };

            var (success, error) = await EmailBillScanner.TestConnection(settings);

            if (success)
            {
                EmailBillScanner.SaveSettings(settings);
                LoadEmailStatus();
            }
            else
            {
                await Application.Current.Windows[0].Page.DisplayAlert("Connection Failed", error, "OK");
            }

            ConnectEmailBtn.IsEnabled = true;
            ConnectEmailBtn.Text = "\U0001F4E7 Connect Email";
        }

        private async void OnScanEmailClicked(object sender, EventArgs e)
        {
            var settings = EmailBillScanner.LoadSettings();
            if (settings == null) return;

            ScanEmailBtn.IsEnabled = false;
            ScanEmailBtn.Text = "Scanning...";
            EmailProgressLabel.IsVisible = true;

            var progress = new Progress<string>(msg =>
                MainThread.BeginInvokeOnMainThread(() => EmailProgressLabel.Text = msg));

            var (found, added, error) = await Task.Run(() => EmailBillScanner.ScanForBills(settings, progress));

            EmailProgressLabel.IsVisible = false;
            ScanEmailBtn.IsEnabled = true;
            ScanEmailBtn.Text = "\U0001F50D Scan for Bills";

            if (error != null)
            {
                await Application.Current.Windows[0].Page.DisplayAlert("Scan Error", error, "OK");
            }
            else
            {
                await Application.Current.Windows[0].Page.DisplayAlert("Scan Complete",
                    $"Found {found} bill PDF(s), added {added} new bill(s) to your history.", "OK");
            }

            LoadEmailStatus();
        }

        private async void OnDisconnectEmailClicked(object sender, EventArgs e)
        {
            var confirm = await Application.Current.Windows[0].Page.DisplayAlert(
                "Disconnect Email", "Remove your email connection?", "Yes", "Cancel");
            if (!confirm) return;

            EmailBillScanner.ClearSettings();
            EmailStatusIcon.Text = "\U0001F534";
            EmailStatusLabel.Text = "Not Connected";
            EmailStatusLabel.TextColor = Color.FromArgb("#64748B");
            EmailFormPanel.IsVisible = true;
            ConnectEmailBtn.IsVisible = true;
            ScanEmailBtn.IsVisible = false;
            DisconnectEmailBtn.IsVisible = false;
            EmailAddressEntry.Text = "";
            EmailPasswordEntry.Text = "";
            EmailImapEntry.Text = "";
        }

        private void LoadMortgageData()
        {
            var m = MockDataService.GetMortgage();
            if (m == null)
            {
                MortLender.Text = MortBalance.Text = MortPayment.Text = MortRate.Text = MortLtv.Text = MortProperty.Text = "—";
                MortgageAlert.IsVisible = false;
                SvrImpactCard.IsVisible = false;
                BestRatesCard.IsVisible = false;
                MortFixTitle.Text = "No mortgage added";
                MortFixDetail.Text = "Tap Edit to add your lender, balance and fixed-rate end date. We'll warn you before the fix ends.";
                return;
            }

            MortgageAlert.IsVisible = false;
            SvrImpactCard.IsVisible = false;
            MortLender.Text = m.Lender;
            MortBalance.Text = m.OutstandingBalance.ToString("C0", _gbp);
            MortPayment.Text = m.MonthlyPayment.ToString("C2", _gbp);
            MortRate.Text = $"{m.CurrentRate}% {m.RateType}";
            MortLtv.Text = $"{m.Ltv}%";
            MortProperty.Text = m.PropertyValue.ToString("C0", _gbp);

            if (m.FixExpired)
            {
                MortgageAlert.IsVisible = true;
                MortgageAlert.BackgroundColor = Color.FromArgb("#3B1A1A");
                MortgageAlertText.Text = "⚠ Your fixed rate has expired — you may be on the SVR";
                MortgageAlertText.TextColor = Color.FromArgb("#EF4444");

                MortFixTitle.Text = "Fixed rate expired";
                MortFixDetail.Text = $"Ended {m.FixedRateEndDate:dd MMM yyyy}. Contact {m.Lender} or a broker to remortgage.";

                SvrImpactCard.IsVisible = true;
                SvrTitle.Text = $"SVR impact: +{m.MonthlySvrIncrease:C2}/month";
                SvrDetail.Text = $"At {m.SvrRate}% SVR, estimated payment rises to {m.EstimatedSvrPayment:C2}/month";
            }
            else if (m.InAlertWindow)
            {
                MortgageAlert.IsVisible = true;
                MortgageAlert.BackgroundColor = Color.FromArgb("#3B2E1A");
                MortgageAlertText.Text = $"⏰ {m.MonthsUntilEnd} months until your fix ends — start looking now";
                MortgageAlertText.TextColor = Color.FromArgb("#F59E0B");

                MortFixTitle.Text = $"Fixed rate ends in {m.MonthsUntilEnd} months";
                MortFixDetail.Text = $"Ends {m.FixedRateEndDate:dd MMM yyyy}. Most lenders let you lock in a new rate 3-6 months early.";

                SvrImpactCard.IsVisible = true;
                SvrTitle.Text = $"If you don't remortgage: +{m.MonthlySvrIncrease:C2}/month";
                SvrDetail.Text = $"SVR of {m.SvrRate}% would increase payment to {m.EstimatedSvrPayment:C2}/month";
            }
            else
            {
                MortFixTitle.Text = $"Fixed until {m.FixedRateEndDate:dd MMM yyyy}";
                MortFixDetail.Text = $"{m.MonthsUntilEnd} months remaining. No action needed yet.";
            }

            BestRatesCard.IsVisible = true;
            var ltv = m.Ltv;
            string ltvBand = ltv <= 60 ? "≤60%" : ltv <= 75 ? "≤75%" : ltv <= 85 ? "≤85%" : "≤90%";
            BestRatesDetail.Text = $"LTV band: {ltvBand}\n" +
                $"2yr fixed: ~4.1%  |  5yr fixed: ~3.9%\n" +
                $"Compare at MoneySupermarket, Habito, or L&C";
        }

        private async void OnEditMortgageClicked(object sender, EventArgs e)
        {
            string lender = await Application.Current.Windows[0].Page.DisplayPromptAsync(
                "Mortgage Details", "Lender name:", initialValue: MockDataService.GetMortgage()?.Lender ?? "");
            if (lender == null) return;

            string propVal = await Application.Current.Windows[0].Page.DisplayPromptAsync(
                "Property Value", "Current property value (£):",
                initialValue: MockDataService.GetMortgage()?.PropertyValue.ToString("0") ?? "",
                keyboard: Keyboard.Numeric);
            if (propVal == null) return;

            string balance = await Application.Current.Windows[0].Page.DisplayPromptAsync(
                "Outstanding Balance", "Mortgage balance (£):",
                initialValue: MockDataService.GetMortgage()?.OutstandingBalance.ToString("0") ?? "",
                keyboard: Keyboard.Numeric);
            if (balance == null) return;

            string payment = await Application.Current.Windows[0].Page.DisplayPromptAsync(
                "Monthly Payment", "Monthly payment (£):",
                initialValue: MockDataService.GetMortgage()?.MonthlyPayment.ToString("0.00") ?? "",
                keyboard: Keyboard.Numeric);
            if (payment == null) return;

            string rate = await Application.Current.Windows[0].Page.DisplayPromptAsync(
                "Interest Rate", "Current rate (%):",
                initialValue: MockDataService.GetMortgage()?.CurrentRate.ToString() ?? "",
                keyboard: Keyboard.Numeric);
            if (rate == null) return;

            string svr = await Application.Current.Windows[0].Page.DisplayPromptAsync(
                "SVR Rate", "Lender SVR rate (%):",
                initialValue: MockDataService.GetMortgage()?.SvrRate.ToString() ?? "",
                keyboard: Keyboard.Numeric);
            if (svr == null) return;

            string fixEnd = await Application.Current.Windows[0].Page.DisplayPromptAsync(
                "Fix End Date", "Fixed rate end date (dd/MM/yyyy):",
                initialValue: MockDataService.GetMortgage()?.FixedRateEndDate.ToString("dd/MM/yyyy") ?? "");
            if (fixEnd == null) return;

            if (!decimal.TryParse(propVal, out var pv) ||
                !decimal.TryParse(balance, out var bal) ||
                !decimal.TryParse(payment, out var pay) ||
                !decimal.TryParse(rate, out var r) ||
                !decimal.TryParse(svr, out var sv) ||
                !DateTime.TryParseExact(fixEnd, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out var fe))
            {
                await Application.Current.Windows[0].Page.DisplayAlert("Error", "Invalid input. Please check values.", "OK");
                return;
            }

            var mort = MockDataService.GetMortgage() ?? new MortgageDetails
            {
                RateType = "Fixed",
                MortgageStartDate = DateTime.Now,
                TermYears = 25,
            };
            mort.Lender = lender;
            mort.PropertyValue = pv;
            mort.OutstandingBalance = bal;
            mort.MonthlyPayment = pay;
            mort.CurrentRate = r;
            mort.SvrRate = sv;
            mort.FixedRateEndDate = fe;
            MockDataService.SetMortgage(mort);

            LoadMortgageData();
        }

        private void UpdateBankStatus()
        {
            if (SmartDataService.HasLiveConnection)
            {
                BankStatusIcon.Text = "\U0001F7E2";
                BankStatusLabel.Text = $"Connected — {SmartDataService.Connections.Count} provider(s)";
                BankStatusLabel.TextColor = Color.FromArgb("#22C55E");
                ConnectBankBtn.Text = "\U0001F3E6 Manage Connections";
            }
            else
            {
                BankStatusIcon.Text = "\U0001F534";
                BankStatusLabel.Text = "Not Connected";
                BankStatusLabel.TextColor = Color.FromArgb("#64748B");
                ConnectBankBtn.Text = "\U0001F3E6 Connect Bank";
            }
        }

        private async void OnConnectBankClicked(object sender, EventArgs e)
        {
            var page = new ConnectBankPage();
            page.BankConnected += () => MainThread.BeginInvokeOnMainThread(UpdateBankStatus);
            await Navigation.PushAsync(page);
        }

        private async void OnViewReportClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new FullReportPage());
        }

        private void LoadDocuments()
        {
            DocumentsList.Children.Clear();
            var docs = DocumentStorageService.GetAll();

            DocumentsEmptyLabel.IsVisible = docs.Count == 0;

            foreach (var doc in docs)
            {
                var icon = doc.Category switch
                {
                    "Licence" => "📋",
                    "Passport" => "🛂",
                    "Payslip" => "💰",
                    "Report" => "📄",
                    "Statement" => "🏦",
                    _ => "📁",
                };

                var categoryColor = doc.Category switch
                {
                    "Licence" => "#22C55E",
                    "Passport" => "#2DD4BF",
                    "Payslip" => "#C084FC",
                    "Report" => "#3B82F6",
                    "Statement" => "#F59E0B",
                    _ => "#94A3B8",
                };

                var row = new Border
                {
                    BackgroundColor = Color.FromArgb("#0D1322"),
                    StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 8 },
                    Stroke = Color.FromArgb("#1E2D4A"),
                    Padding = new Thickness(12, 8),
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
                            new Label
                            {
                                Text = icon, FontSize = 18,
                                VerticalOptions = LayoutOptions.Center,
                            },
                            SetCol(new VerticalStackLayout
                            {
                                Spacing = 2,
                                Children =
                                {
                                    new Label { Text = doc.Name, TextColor = Color.FromArgb("#E2E8F0"), FontSize = 12, LineBreakMode = LineBreakMode.TailTruncation },
                                    new HorizontalStackLayout
                                    {
                                        Spacing = 8,
                                        Children =
                                        {
                                            new Label
                                            {
                                                Text = doc.Category,
                                                TextColor = Color.FromArgb(categoryColor),
                                                FontSize = 10, FontAttributes = FontAttributes.Bold,
                                            },
                                            new Label
                                            {
                                                Text = doc.DateAdded.ToString("dd MMM yyyy"),
                                                TextColor = Color.FromArgb("#64748B"), FontSize = 10,
                                            },
                                            new Label
                                            {
                                                Text = DocumentStorageService.FormatSize(doc.FileSizeBytes),
                                                TextColor = Color.FromArgb("#64748B"), FontSize = 10,
                                            },
                                        }
                                    }
                                }
                            }, 1),
                            SetCol(CreateDocButton("Open", "#1E3A5F", "#60A5FA", doc.FilePath, OnOpenDocClicked, "Open this document."), 2),
                            SetCol(CreateDocButton("X", "#3B1A1A", "#EF4444", doc.Id, OnDeleteDocClicked, "Delete this document."), 3),
                        }
                    }
                };

                DocumentsList.Children.Add(row);
            }
        }

        private static View SetCol(View view, int col)
        {
            Grid.SetColumn(view, col);
            return view;
        }

        private static Button CreateDocButton(string text, string bg, string fg, string tag, EventHandler handler, string tooltip = null)
        {
            var btn = new Button
            {
                Text = text,
                BackgroundColor = Color.FromArgb(bg),
                TextColor = Color.FromArgb(fg),
                FontSize = 10,
                CornerRadius = 6,
                Padding = new Thickness(8, 2),
                HeightRequest = 26,
                ClassId = tag,
            };
            btn.Clicked += handler;
            if (!string.IsNullOrEmpty(tooltip))
                ToolTipProperties.SetText(btn, tooltip);
            return btn;
        }

        private async void OnOpenDocClicked(object sender, EventArgs e)
        {
            if (sender is Button btn && !string.IsNullOrEmpty(btn.ClassId))
            {
                try
                {
                    // Stored documents are encrypted; open a temporary decrypted copy.
                    var viewPath = SecureFile.DecryptForViewing(btn.ClassId);
                    await Launcher.OpenAsync(new OpenFileRequest
                    {
                        File = new ReadOnlyFile(viewPath)
                    });
                }
                catch (Exception ex)
                {
                    var page = Application.Current.Windows[0].Page;
                    await page.DisplayAlert("Error", $"Could not open file: {ex.Message}", "OK");
                }
            }
        }

        private async void OnDeleteDocClicked(object sender, EventArgs e)
        {
            if (sender is Button btn && !string.IsNullOrEmpty(btn.ClassId))
            {
                var page = Application.Current.Windows[0].Page;
                var confirm = await page.DisplayAlert("Delete Document", "Remove this document?", "Delete", "Cancel");
                if (!confirm) return;

                DocumentStorageService.RemoveDocument(btn.ClassId);
                LoadDocuments();
            }
        }

        private async void OnAddDocumentClicked(object sender, EventArgs e)
        {
            var page = Application.Current.Windows[0].Page;

            try
            {
                var result = await FilePicker.PickAsync(new PickOptions
                {
                    PickerTitle = "Select a document to upload",
                });

                if (result == null) return;

                var category = await page.DisplayActionSheet("Document Category",
                    "Cancel", null, "Licence", "Passport", "Payslip", "Statement", "Report", "Other");

                if (string.IsNullOrEmpty(category) || category == "Cancel") return;

                DocumentStorageService.AddDocument(result.FullPath, category, result.FileName);
                LoadDocuments();
            }
            catch (Exception ex)
            {
                await page.DisplayAlert("Error", ex.Message, "OK");
            }
        }

        private void LoadCreditScore()
        {
            var p = UserProfileDataService.GetProfile();

            if (int.TryParse(p.CreditScore, out int score) && score > 0)
            {
                CreditScoreValue.Text = score.ToString();

                var (rating, color) = score switch
                {
                    >= 811 => ("Excellent", "#22C55E"),
                    >= 671 => ("Good", "#34D399"),
                    >= 561 => ("Fair", "#F59E0B"),
                    >= 440 => ("Poor", "#F97316"),
                    _ => ("Very Poor", "#EF4444"),
                };

                CreditScoreValue.TextColor = Color.FromArgb(color);
                CreditScoreRating.Text = rating;
                CreditScoreRating.TextColor = Color.FromArgb(color);

                var barMax = 196.0;
                var fraction = Math.Min(score / 999.0, 1.0);
                CreditScoreBar.WidthRequest = barMax * fraction;
                CreditScoreBar.BackgroundColor = Color.FromArgb(color);
            }
            else
            {
                CreditScoreValue.Text = "—";
                CreditScoreValue.TextColor = Color.FromArgb("#64748B");
                CreditScoreRating.Text = "No score recorded";
                CreditScoreRating.TextColor = Color.FromArgb("#94A3B8");
                CreditScoreBar.WidthRequest = 0;
            }

            CreditProviderLabel.Text = string.IsNullOrEmpty(p.CreditScoreProvider) ? "—" : p.CreditScoreProvider;
            CreditLastCheckedLabel.Text = string.IsNullOrEmpty(p.CreditScoreDate) ? "—" : p.CreditScoreDate;

            CreditHistoryList.Children.Clear();
            if (p.CreditScoreHistory.Count > 0)
            {
                foreach (var entry in p.CreditScoreHistory.OrderByDescending(e => e.Date).Take(5))
                {
                    var (_, entryColor) = entry.Score switch
                    {
                        >= 811 => ("Excellent", "#22C55E"),
                        >= 671 => ("Good", "#34D399"),
                        >= 561 => ("Fair", "#F59E0B"),
                        >= 440 => ("Poor", "#F97316"),
                        _ => ("Very Poor", "#EF4444"),
                    };

                    CreditHistoryList.Children.Add(new Grid
                    {
                        ColumnDefinitions = new ColumnDefinitionCollection
                        {
                            new ColumnDefinition(GridLength.Star),
                            new ColumnDefinition(GridLength.Auto),
                            new ColumnDefinition(GridLength.Auto),
                        },
                        ColumnSpacing = 8,
                        Children =
                        {
                            new Label { Text = entry.Date, TextColor = Color.FromArgb("#94A3B8"), FontSize = 11 },
                            SetCol(new Label { Text = entry.Provider, TextColor = Color.FromArgb("#64748B"), FontSize = 11 }, 1),
                            SetCol(new Label { Text = entry.Score.ToString(), TextColor = Color.FromArgb(entryColor), FontSize = 11, FontAttributes = FontAttributes.Bold }, 2),
                        }
                    });
                }
            }
        }

        private async void OnUpdateScoreClicked(object sender, EventArgs e)
        {
            var page = Application.Current.Windows[0].Page;

            string scoreStr = await page.DisplayPromptAsync("Credit Score", "Enter your current credit score (0-999):",
                keyboard: Keyboard.Numeric);
            if (string.IsNullOrWhiteSpace(scoreStr)) return;

            if (!int.TryParse(scoreStr, out int score) || score < 0 || score > 999)
            {
                await page.DisplayAlert("Invalid Score", "Please enter a number between 0 and 999.", "OK");
                return;
            }

            string provider = await page.DisplayActionSheet("Credit Score Provider", "Cancel", null,
                "Experian", "Equifax", "TransUnion", "ClearScore", "Credit Karma");
            if (string.IsNullOrWhiteSpace(provider) || provider == "Cancel") return;

            var p = UserProfileDataService.GetProfile();
            var dateStr = DateTime.Now.ToString("dd MMM yyyy");

            p.CreditScore = score.ToString();
            p.CreditScoreProvider = provider;
            p.CreditScoreDate = dateStr;

            p.CreditScoreHistory.Add(new CreditScoreEntry
            {
                Score = score,
                Provider = provider,
                Date = dateStr,
            });

            UserProfileDataService.SaveProfile();
            LoadCreditScore();
            LoadPersonalData();
        }

        private async void OnCheckClearScoreClicked(object sender, EventArgs e)
        {
            await Launcher.OpenAsync(new Uri("https://www.clearscore.com/"));
        }

        private async void OnCheckCreditKarmaClicked(object sender, EventArgs e)
        {
            await Launcher.OpenAsync(new Uri("https://www.creditkarma.co.uk/"));
        }

        private async void OnCheckMseClicked(object sender, EventArgs e)
        {
            await Launcher.OpenAsync(new Uri("https://www.moneysavingexpert.com/creditclub/"));
        }

        public async Task PlayEntryAnimation()
        {
            AnimationHelper.PrepareForEntry(UserCard, PersonalCard, PropertiesCard, VehiclesCard,
                SessionCard, SmartScanCard, CreditScoreCard, EmailCard, ConnectBankCard, ReportsCard, DocumentsCard, MortgageCard, CubeFacesCard);
            _ = AnimationHelper.AnimateEntry(UserCard, 0, 400);
            _ = AnimationHelper.AnimateEntry(PersonalCard, 100, 400);
            _ = AnimationHelper.AnimateEntry(PropertiesCard, 200, 400);
            _ = AnimationHelper.AnimateEntry(VehiclesCard, 300, 400);
            _ = AnimationHelper.AnimateEntry(SessionCard, 0, 400);
            _ = AnimationHelper.AnimateEntry(SmartScanCard, 50, 400);
            _ = AnimationHelper.AnimateEntry(CreditScoreCard, 100, 400);
            _ = AnimationHelper.AnimateEntry(EmailCard, 200, 400);
            _ = AnimationHelper.AnimateEntry(ConnectBankCard, 300, 400);
            _ = AnimationHelper.AnimateEntry(ReportsCard, 400, 400);
            _ = AnimationHelper.AnimateEntry(DocumentsCard, 500, 400);
            _ = AnimationHelper.AnimateEntry(MortgageCard, 600, 400);
            await AnimationHelper.AnimateEntry(CubeFacesCard, 700, 400);
        }
    }

    public class CubeFaceDisplayItem
    {
        public string Icon { get; set; }
        public string Name { get; set; }
        public string CodeFormatted { get; set; }
        public string OrderLabel { get; set; }
        public string StatusText { get; set; }
        public Color StatusColour { get; set; }
    }
}
