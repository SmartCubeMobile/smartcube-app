using System.Globalization;
using SmartCubeMobile.MockData;
using SmartCubeMobile.Services;

namespace SmartCubeMobile.Dashboard.Faces
{
    public partial class InsuranceFace : ContentView, IAnimatedFace
    {
        private static readonly CultureInfo _gbp = new("en-GB");
        public InsuranceFace()
        {
            InitializeComponent();
            LoadData();
            SetupDropZones();
        }

        private async void OnHelpClicked(object sender, EventArgs e)
        {
            var page = Application.Current?.Windows.FirstOrDefault()?.Page;
            if (page != null)
                await page.DisplayAlert("Insurance",
                    "This page tracks your car, house, life and pet insurance policies and what they cost. Use + Manual on any card to type in a policy's details yourself, or drop a policy PDF or photo onto the dashed box and Smart Scan reads the provider, premium and cover automatically. Find Payment on a policy searches your connected bank transactions for a matching payment, and Upcoming Renewals lists policies due soon. All policy data is stored locally on this PC.",
                    "OK");
        }

        private void SetupDropZones()
        {
            SetupDropZone(CarDropZone, "Car");
            SetupDropZone(HouseDropZone, "House");
            SetupDropZone(LifeDropZone, "Life");
            SetupDropZone(PetDropZone, "Pet");
        }

        private void SetupDropZone(Border dropZone, string category)
        {
            var tap = new TapGestureRecognizer();
            tap.Tapped += async (s, e) => await PickAndProcessFile(category);
            dropZone.GestureRecognizers.Add(tap);

#if WINDOWS
            dropZone.HandlerChanged += (s, e) =>
            {
                if (dropZone.Handler?.PlatformView is Microsoft.UI.Xaml.UIElement nativeElement)
                {
                    nativeElement.AllowDrop = true;
                    nativeElement.DragOver += (ds, de) =>
                    {
                        de.AcceptedOperation = Windows.ApplicationModel.DataTransfer.DataPackageOperation.Copy;
                        de.DragUIOverride.Caption = $"Add {category} Policy";
                        de.DragUIOverride.IsCaptionVisible = true;
                        de.DragUIOverride.IsGlyphVisible = false;
                        MainThread.BeginInvokeOnMainThread(() =>
                        {
                            dropZone.StrokeThickness = 2.5;
                            dropZone.Opacity = 1;
                        });
                    };
                    nativeElement.DragLeave += (ds, de) =>
                    {
                        MainThread.BeginInvokeOnMainThread(() =>
                        {
                            dropZone.StrokeThickness = 1.5;
                            dropZone.Opacity = 1;
                        });
                    };
                    nativeElement.Drop += async (ds, de) =>
                    {
                        var deferral = de.GetDeferral();
                        try
                        {
                            MainThread.BeginInvokeOnMainThread(() =>
                            {
                                dropZone.StrokeThickness = 1.5;
                                dropZone.Opacity = 1;
                            });

                            if (de.DataView.Contains(Windows.ApplicationModel.DataTransfer.StandardDataFormats.StorageItems))
                            {
                                var items = await de.DataView.GetStorageItemsAsync();
                                foreach (var item in items)
                                {
                                    if (item is Windows.Storage.StorageFile file)
                                    {
                                        var ext = file.FileType.ToLower();
                                        if (ext == ".jpg" || ext == ".jpeg" || ext == ".png" || ext == ".bmp" || ext == ".gif" || ext == ".tiff" || ext == ".pdf")
                                        {
                                            await MainThread.InvokeOnMainThreadAsync(() => ProcessPolicyFile(file.Path, category));
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                        finally
                        {
                            deferral.Complete();
                        }
                    };
                }
            };
#endif
        }

        private async Task PickAndProcessFile(string category)
        {
            try
            {
                var policyFileTypes = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
                {
                    { DevicePlatform.WinUI, new[] { ".jpg", ".jpeg", ".png", ".bmp", ".gif", ".tiff", ".pdf" } },
                });
                var result = await FilePicker.Default.PickAsync(new PickOptions
                {
                    PickerTitle = $"Select {category} Insurance Policy Document",
                    FileTypes = policyFileTypes
                });

                if (result != null)
                    await ProcessPolicyFile(result.FullPath, category);
            }
            catch { }
        }

        private async Task<bool> EnsureSmartScanLicence(string reason = null)
        {
            var page = Application.Current.Windows[0].Page;
            var profile = UserProfileDataService.GetProfile();
            var prompt = reason ?? "Enter your Smart Scan licence key to read policy documents automatically.";
            var key = await page.DisplayPromptAsync("Smart Scan Licence", prompt,
                placeholder: "SC-XXXX-XXXX-XXXX", initialValue: profile.SmartScanLicence ?? "", maxLength: 40);
            if (string.IsNullOrWhiteSpace(key)) return false;
            profile.SmartScanLicence = key.Trim().ToUpperInvariant();
            UserProfileDataService.SaveProfile();
            return true;
        }

        private async Task ProcessPolicyFile(string filePath, string category)
        {
            var page = Application.Current.Windows[0].Page;

            // Let the user choose: Smart Scan (server, needs a licence) or the basic built-in reader.
            const string smartOption = "Smart Scan (AI, uses your licence)";
            const string basicOption = "Basic read (no licence, for testing)";
            const string simOption = "Simulated Smart Scan (response.json, for testing)";
            var choice = await page.DisplayActionSheet($"How should SmartCube read this {category.ToLower()} policy?",
                "Cancel", null, smartOption, basicOption, simOption);
            var mode = choice switch
            {
                smartOption => InsurancePolicyOcrService.ScanMode.SmartScan,
                basicOption => InsurancePolicyOcrService.ScanMode.Basic,
                simOption => InsurancePolicyOcrService.ScanMode.Simulated,
                _ => (InsurancePolicyOcrService.ScanMode?)null,
            };
            if (mode == null) return;
            var useSmartScan = mode == InsurancePolicyOcrService.ScanMode.SmartScan;

            if (useSmartScan && string.IsNullOrWhiteSpace(UserProfileDataService.GetProfile().SmartScanLicence))
            {
                if (!await EnsureSmartScanLicence()) return;
            }

            InsurancePolicyOcrData data;
            string rawText, error;
            while (true)
            {
                try
                {
                    (data, rawText, error) = await InsurancePolicyOcrService.ExtractFromImage(filePath, category, mode.Value);
                    break;
                }
                catch (SmartScanLicenceException ex)
                {
                    var retry = await EnsureSmartScanLicence($"{ex.Message}.\n\nEnter a valid Smart Scan licence key, or cancel to use the basic reader:");
                    if (!retry) mode = InsurancePolicyOcrService.ScanMode.Basic;
                }
            }

            if (error != null || data == null)
            {
                var title = mode == InsurancePolicyOcrService.ScanMode.Simulated && rawText != null ? "Simulated Smart Scan" : "OCR Failed";
                await page.DisplayAlert(title, error ?? "Could not read document", "OK");
                return;
            }

            var details = "";
            if (!string.IsNullOrEmpty(data.Provider)) details += $"Provider: {data.Provider}\n";
            if (!string.IsNullOrEmpty(data.PolicyNumber)) details += $"Policy No: {data.PolicyNumber}\n";
            if (!string.IsNullOrEmpty(data.PolicyType)) details += $"Type: {data.PolicyType}\n";
            if (!string.IsNullOrEmpty(data.VehicleReg)) details += $"Reg: {data.VehicleReg}\n";
            if (!string.IsNullOrEmpty(data.VehicleMakeModel)) details += $"Vehicle: {data.VehicleMakeModel}\n";
            if (!string.IsNullOrEmpty(data.PropertyAddress)) details += $"Property: {data.PropertyAddress}\n";
            if (!string.IsNullOrEmpty(data.PetName)) details += $"Pet: {data.PetName}\n";
            if (!string.IsNullOrEmpty(data.PetBreed)) details += $"Breed: {data.PetBreed}\n";
            if (data.MonthlyPremium > 0) details += $"Monthly: £{data.MonthlyPremium:F2}\n";
            if (data.AnnualPremium > 0) details += $"Annual: £{data.AnnualPremium:F2}\n";
            if (data.CoverAmount > 0) details += $"Cover: £{data.CoverAmount:N0}\n";
            if (data.Excess > 0) details += $"Excess: £{data.Excess:F2}\n";
            if (!string.IsNullOrEmpty(data.StartDate)) details += $"Start: {data.StartDate}\n";
            if (!string.IsNullOrEmpty(data.EndDate)) details += $"Renewal: {data.EndDate}\n";
            if (!string.IsNullOrEmpty(data.NamedInsured)) details += $"Insured: {data.NamedInsured}\n";

            if (string.IsNullOrWhiteSpace(details))
                details = "No fields extracted.\n";

            if (!string.IsNullOrEmpty(data.Warning))
                details = $"⚠ {data.Warning}\n\n" + details;

            details += $"\n--- Raw OCR Text ---\n{rawText?[..Math.Min(rawText?.Length ?? 0, 500)]}";

            var confirm = await page.DisplayAlert(
                $"{category} Policy Found",
                details,
                "Save Policy", "Cancel");

            if (!confirm) return;

            if (string.IsNullOrEmpty(data.Provider))
            {
                data.Provider = await page.DisplayPromptAsync("Provider", "Insurance provider name:", placeholder: "e.g. Admiral, Aviva");
                if (string.IsNullOrWhiteSpace(data.Provider)) return;
            }

            if (string.IsNullOrEmpty(data.PolicyType))
            {
                string policyType;
                if (category == "Car")
                    policyType = await page.DisplayActionSheet("Policy Type", "Cancel", null,
                        "Comprehensive", "Third Party", "Third Party Fire & Theft", "Breakdown", "GAP");
                else if (category == "House")
                    policyType = await page.DisplayActionSheet("Policy Type", "Cancel", null,
                        "Buildings", "Contents", "Buildings & Contents", "Landlord");
                else if (category == "Pet")
                    policyType = await page.DisplayActionSheet("Policy Type", "Cancel", null,
                        "Lifetime", "Annual", "Accident Only", "Time Limited");
                else
                    policyType = await page.DisplayActionSheet("Policy Type", "Cancel", null,
                        "Term Life", "Whole Life", "Critical Illness", "Income Protection");
                if (string.IsNullOrWhiteSpace(policyType) || policyType == "Cancel") return;
                data.PolicyType = policyType;
            }

            if (data.MonthlyPremium == 0)
            {
                string premium = await page.DisplayPromptAsync(
                    "Monthly Premium", "Amount per month (£):", keyboard: Keyboard.Numeric);
                if (!string.IsNullOrWhiteSpace(premium) && decimal.TryParse(premium, out var amt))
                    data.MonthlyPremium = amt;
            }

            DateTime? renewalDate = null;
            if (!string.IsNullOrEmpty(data.EndDate))
            {
                var parts = data.EndDate.Split('/');
                if (parts.Length == 3 && int.TryParse(parts[0], out var d) && int.TryParse(parts[1], out var m) && int.TryParse(parts[2], out var y))
                {
                    try { renewalDate = new DateTime(y, m, d); } catch { }
                }
            }

            DateTime startDate = DateTime.Now;
            if (!string.IsNullOrEmpty(data.StartDate))
            {
                var parts = data.StartDate.Split('/');
                if (parts.Length == 3 && int.TryParse(parts[0], out var d) && int.TryParse(parts[1], out var m) && int.TryParse(parts[2], out var y))
                {
                    try { startDate = new DateTime(y, m, d); } catch { }
                }
            }

            var policy = new InsurancePolicy
            {
                Id = Guid.NewGuid().ToString("N")[..8],
                Category = category,
                PolicyType = data.PolicyType ?? "Unknown",
                Provider = data.Provider.Trim(),
                MonthlyPremium = data.MonthlyPremium,
                AnnualPremium = data.AnnualPremium,
                CoverAmount = data.CoverAmount,
                Excess = data.Excess,
                PolicyNumber = data.PolicyNumber?.Trim(),
                StartDate = startDate,
                RenewalDate = renewalDate ?? startDate.AddYears(1),
                VehicleReg = data.VehicleReg?.Trim(),
                VehicleMakeModel = data.VehicleMakeModel?.Trim(),
                PropertyAddress = data.PropertyAddress?.Trim(),
                PetName = data.PetName?.Trim(),
                PetBreed = data.PetBreed?.Trim(),
                NamedInsured = data.NamedInsured?.Trim(),
            };

            InsuranceDataService.AddPolicy(policy);
            LoadData();
        }

        private void LoadData()
        {
            var policies = InsuranceDataService.GetPolicies();

            var car = policies.Where(p => p.Category == "Car").ToList();
            var house = policies.Where(p => p.Category == "House").ToList();
            var life = policies.Where(p => p.Category == "Life").ToList();
            var pet = policies.Where(p => p.Category == "Pet").ToList();

            var totalMonthly = policies.Sum(p => p.MonthlyPremium);
            var totalAnnual = policies.Sum(p => p.AnnualPremium > 0 ? p.AnnualPremium : p.MonthlyPremium * 12);
            PolicyCountLabel.Text = policies.Count.ToString();
            TotalMonthlyLabel.Text = totalMonthly.ToString("C2", _gbp);
            TotalYearlyLabel.Text = totalAnnual.ToString("C2", _gbp);

            LoadCategoryPolicies(CarPolicies, CarEmptyLabel, car, "#1E3A5F");
            LoadCategoryPolicies(HousePolicies, HouseEmptyLabel, house, "#0A3D2E");
            LoadCategoryPolicies(LifePolicies, LifeEmptyLabel, life, "#2E1A5F");
            LoadCategoryPolicies(PetPolicies, PetEmptyLabel, pet, "#5F3A1E");

            LoadRenewals(policies);
        }

        private void LoadCategoryPolicies(VerticalStackLayout list, Label emptyLabel,
            List<InsurancePolicy> policies, string accentColor)
        {
            list.Children.Clear();
            emptyLabel.IsVisible = policies.Count == 0;

            foreach (var policy in policies)
                list.Children.Add(CreatePolicyCard(policy, accentColor));
        }

        private void LoadRenewals(List<InsurancePolicy> policies)
        {
            RenewalsList.Children.Clear();
            var upcoming = policies
                .Where(p => p.RenewalDate.HasValue && p.RenewalDate.Value > DateTime.Now)
                .OrderBy(p => p.RenewalDate)
                .Take(5)
                .ToList();

            RenewalsEmptyLabel.IsVisible = upcoming.Count == 0;

            foreach (var policy in upcoming)
            {
                var daysLeft = (policy.RenewalDate.Value - DateTime.Now).Days;
                var urgencyColor = daysLeft <= 30 ? "#EF4444" : daysLeft <= 60 ? "#F59E0B" : "#22C55E";

                var row = new Grid
                {
                    ColumnDefinitions = new ColumnDefinitionCollection
                    {
                        new ColumnDefinition(GridLength.Star),
                        new ColumnDefinition(GridLength.Auto),
                    },
                    Padding = new Thickness(0, 4),
                };

                row.Add(new Label
                {
                    Text = $"{policy.Provider} — {policy.Category}",
                    TextColor = Color.FromArgb("#CBD5E1"),
                    FontSize = 12,
                    VerticalOptions = LayoutOptions.Center,
                }, 0);

                var badge = new Border
                {
                    BackgroundColor = Color.FromArgb(urgencyColor + "20"),
                    StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 6 },
                    Stroke = Colors.Transparent,
                    Padding = new Thickness(8, 2),
                    Content = new Label
                    {
                        Text = daysLeft <= 0 ? "Due" : $"{daysLeft}d — {policy.RenewalDate.Value:dd MMM}",
                        TextColor = Color.FromArgb(urgencyColor),
                        FontSize = 11,
                        FontAttributes = FontAttributes.Bold,
                    }
                };
                row.Add(badge, 1);

                RenewalsList.Children.Add(row);
            }
        }

        private View CreatePolicyCard(InsurancePolicy policy, string accentColor)
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
                    new ColumnDefinition(GridLength.Star),
                    new ColumnDefinition(GridLength.Auto),
                    new ColumnDefinition(GridLength.Auto),
                },
                ColumnSpacing = 10,
            };

            var details = new VerticalStackLayout { Spacing = 2, VerticalOptions = LayoutOptions.Center };
            details.Add(new Label
            {
                Text = policy.Provider,
                TextColor = Color.FromArgb("#E2E8F0"),
                FontSize = 13,
                FontAttributes = FontAttributes.Bold,
            });

            var subText = policy.PolicyType;
            if (!string.IsNullOrEmpty(policy.PolicyNumber))
                subText += $" · #{policy.PolicyNumber}";
            details.Add(new Label
            {
                Text = subText,
                TextColor = Color.FromArgb("#64748B"),
                FontSize = 11,
            });

            // Named insured
            if (!string.IsNullOrEmpty(policy.NamedInsured))
            {
                details.Add(new Label
                {
                    Text = policy.NamedInsured,
                    TextColor = Color.FromArgb("#CBD5E1"),
                    FontSize = 11,
                });
            }

            // Category-specific details line
            var extraLine = "";
            if (policy.Category == "Car")
            {
                if (!string.IsNullOrEmpty(policy.VehicleReg))
                    extraLine += policy.VehicleReg;
                if (!string.IsNullOrEmpty(policy.VehicleMakeModel))
                    extraLine += (extraLine.Length > 0 ? "  ·  " : "") + policy.VehicleMakeModel;
            }
            else if (policy.Category == "House" && !string.IsNullOrEmpty(policy.PropertyAddress))
                extraLine = policy.PropertyAddress;
            else if (policy.Category == "Pet")
            {
                if (!string.IsNullOrEmpty(policy.PetName))
                    extraLine += policy.PetName;
                if (!string.IsNullOrEmpty(policy.PetBreed))
                    extraLine += (extraLine.Length > 0 ? " · " : "") + policy.PetBreed;
            }
            if (!string.IsNullOrEmpty(extraLine))
            {
                details.Add(new Label
                {
                    Text = extraLine,
                    TextColor = Color.FromArgb("#06B6D4"),
                    FontSize = 11,
                });
            }

            var infoLine = "";
            if (policy.CoverAmount > 0)
                infoLine += $"Cover: £{policy.CoverAmount:N0}";
            if (policy.Excess > 0)
                infoLine += (infoLine.Length > 0 ? "  ·  " : "") + $"Excess: £{policy.Excess:N0}";
            if (!string.IsNullOrEmpty(infoLine))
            {
                details.Add(new Label
                {
                    Text = infoLine,
                    TextColor = Color.FromArgb("#94A3B8"),
                    FontSize = 11,
                });
            }

            var costStack = new VerticalStackLayout { Spacing = 2, VerticalOptions = LayoutOptions.Center };

            if (policy.MonthlyPremium > 0)
                costStack.Add(new Label
                {
                    Text = policy.MonthlyPremium.ToString("C2", _gbp) + "/mo",
                    TextColor = Color.FromArgb("#F59E0B"),
                    FontSize = 14,
                    FontAttributes = FontAttributes.Bold,
                    HorizontalTextAlignment = TextAlignment.End,
                });
            if (policy.AnnualPremium > 0)
                costStack.Add(new Label
                {
                    Text = policy.AnnualPremium.ToString("C2", _gbp) + "/yr",
                    TextColor = Color.FromArgb("#94A3B8"),
                    FontSize = 10,
                    HorizontalTextAlignment = TextAlignment.End,
                });

            var btnStack = new VerticalStackLayout { Spacing = 4, VerticalOptions = LayoutOptions.Center };

            var findPayBtn = new Button
            {
                Text = "Find Payment",
                BackgroundColor = Color.FromArgb("#1A2F1A"),
                TextColor = Color.FromArgb("#22C55E"),
                FontSize = 10,
                CornerRadius = 6,
                Padding = new Thickness(8, 2),
                HeightRequest = 26,
            };
            findPayBtn.Clicked += (s, e) => OnFindPaymentClicked(policy, costStack);
            ToolTipProperties.SetText(findPayBtn, "Search your bank transactions for a payment that matches this policy.");

            var removeBtn = new Button
            {
                Text = "Remove",
                BackgroundColor = Color.FromArgb("#3B1A1A"),
                TextColor = Color.FromArgb("#EF4444"),
                FontSize = 10,
                CornerRadius = 6,
                Padding = new Thickness(8, 2),
                HeightRequest = 26,
            };
            removeBtn.Clicked += (s, e) => OnRemoveClicked(policy);
            ToolTipProperties.SetText(removeBtn, "Remove this policy.");

            var compareBtn = new Button
            {
                Text = "Compare",
                BackgroundColor = Color.FromArgb("#1E3A5F"),
                TextColor = Color.FromArgb("#60A5FA"),
                FontSize = 10,
                CornerRadius = 6,
                Padding = new Thickness(8, 2),
                HeightRequest = 26,
            };
            compareBtn.Clicked += async (s, e) =>
            {
                try
                {
                    var nav = Application.Current?.Windows.FirstOrDefault()?.Page?.Navigation;
                    if (nav != null) await nav.PushAsync(new InsuranceComparePage(policy));
                }
                catch (Exception ex)
                {
                    var host = Application.Current?.Windows.FirstOrDefault()?.Page;
                    if (host != null) await host.DisplayAlert("Compare", ex.Message, "OK");
                }
            };
            ToolTipProperties.SetText(compareBtn, "Get quotes from UK comparison sites, with this policy's details ready to copy.");

            btnStack.Add(compareBtn);
            btnStack.Add(findPayBtn);
            btnStack.Add(removeBtn);

            grid.Add(details, 0);
            grid.Add(costStack, 1);
            grid.Add(btnStack, 2);

            card.Content = grid;
            return card;
        }

        private async void OnAddCarClicked(object sender, EventArgs e) => await AddPolicy("Car");
        private async void OnAddHouseClicked(object sender, EventArgs e) => await AddPolicy("House");
        private async void OnAddLifeClicked(object sender, EventArgs e) => await AddPolicy("Life");
        private async void OnAddPetClicked(object sender, EventArgs e) => await AddPolicy("Pet");

        private async Task AddPolicy(string category)
        {
            var page = Application.Current.Windows[0].Page;

            string policyType;
            if (category == "Car")
            {
                policyType = await page.DisplayActionSheet("Policy Type", "Cancel", null,
                    "Comprehensive", "Third Party", "Third Party Fire & Theft", "Breakdown", "GAP");
            }
            else if (category == "House")
            {
                policyType = await page.DisplayActionSheet("Policy Type", "Cancel", null,
                    "Buildings", "Contents", "Buildings & Contents", "Landlord");
            }
            else if (category == "Pet")
            {
                policyType = await page.DisplayActionSheet("Policy Type", "Cancel", null,
                    "Lifetime", "Annual", "Accident Only", "Time Limited");
            }
            else
            {
                policyType = await page.DisplayActionSheet("Policy Type", "Cancel", null,
                    "Term Life", "Whole Life", "Critical Illness", "Income Protection");
            }
            if (string.IsNullOrWhiteSpace(policyType) || policyType == "Cancel") return;

            string provider = await page.DisplayPromptAsync(
                "Provider", "Insurance provider name:", placeholder: "e.g. Aviva, Admiral");
            if (string.IsNullOrWhiteSpace(provider)) return;

            string premium = await page.DisplayPromptAsync(
                "Monthly Premium", "Amount per month (£):", keyboard: Keyboard.Numeric);
            if (string.IsNullOrWhiteSpace(premium) || !decimal.TryParse(premium, out var amount)) return;

            string cover = await page.DisplayPromptAsync(
                "Cover Amount", "Total cover amount (£), or leave blank:",
                keyboard: Keyboard.Numeric, initialValue: "");
            decimal.TryParse(cover, out var coverAmount);

            string policyNum = await page.DisplayPromptAsync(
                "Policy Number", "Policy number (optional):", initialValue: "");

            var policy = new InsurancePolicy
            {
                Id = Guid.NewGuid().ToString("N")[..8],
                Category = category,
                PolicyType = policyType,
                Provider = provider.Trim(),
                MonthlyPremium = amount,
                CoverAmount = coverAmount,
                PolicyNumber = policyNum?.Trim(),
                StartDate = DateTime.Now,
                RenewalDate = DateTime.Now.AddYears(1),
            };

            InsuranceDataService.AddPolicy(policy);
            LoadData();
        }

        private async void OnFindPaymentClicked(InsurancePolicy policy, VerticalStackLayout costStack)
        {
            var page = Application.Current.Windows[0].Page;
            var transactions = SmartDataService.GetCachedTransactions();
            if (transactions == null || transactions.Count == 0)
            {
                await page.DisplayAlert("No Transactions", "No bank transactions available. Connect your bank first.", "OK");
                return;
            }

            var searchTerms = new List<string>();
            if (!string.IsNullOrEmpty(policy.Provider))
            {
                searchTerms.Add(policy.Provider.ToUpper());
                var parts = policy.Provider.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length > 1)
                    searchTerms.Add(parts[0].ToUpper());
            }
            if (!string.IsNullOrEmpty(policy.PolicyNumber))
                searchTerms.Add(policy.PolicyNumber.ToUpper());

            var matches = transactions
                .Where(t => t.Amount < 0 && searchTerms.Any(term =>
                    t.Description.ToUpper().Contains(term)))
                .OrderByDescending(t => t.Date)
                .Take(12)
                .ToList();

            if (matches.Count == 0)
            {
                await page.DisplayAlert("No Matches",
                    $"No payments found matching \"{policy.Provider}\" in your bank transactions.\n\n" +
                    "Try refreshing your bank data on the Banking page.",
                    "OK");
                return;
            }

            var grouped = matches
                .GroupBy(t => Math.Abs(Math.Round(t.Amount, 2)))
                .OrderByDescending(g => g.Count())
                .ThenByDescending(g => g.Max(t => t.Date))
                .ToList();

            var mostCommon = grouped.First();
            var amount = mostCommon.Key;
            var count = mostCommon.Count();
            var lastDate = mostCommon.Max(t => t.Date);
            var desc = mostCommon.First().Description;

            var message = $"Found {matches.Count} payment(s) to \"{desc}\"\n\n";
            message += $"Most frequent amount: £{amount:F2} ({count} payments)\n";
            message += $"Last payment: {lastDate:dd MMM yyyy}\n\n";

            if (count >= 2)
            {
                var dates = mostCommon.OrderBy(t => t.Date).Select(t => t.Date).ToList();
                var intervals = new List<double>();
                for (int i = 1; i < dates.Count; i++)
                    intervals.Add((dates[i] - dates[i - 1]).TotalDays);
                var avgInterval = intervals.Average();

                if (avgInterval >= 25 && avgInterval <= 35)
                    message += $"Payment frequency: Monthly (every ~{avgInterval:F0} days)\n";
                else if (avgInterval >= 350 && avgInterval <= 380)
                    message += "Payment frequency: Annual\n";
                else
                    message += $"Payment frequency: Every ~{avgInterval:F0} days\n";
            }

            var useAmount = await page.DisplayAlert("Payment Found", message, "Use This Amount", "Cancel");
            if (useAmount)
            {
                policy.MonthlyPremium = amount;
                if (policy.AnnualPremium == 0)
                    policy.AnnualPremium = Math.Round(amount * 12, 2);
                InsuranceDataService.UpdatePolicy(policy);
                LoadData();
            }
        }

        private async void OnRemoveClicked(InsurancePolicy policy)
        {
            var result = await Application.Current.Windows[0].Page.DisplayAlert(
                "Remove Policy",
                $"Remove {policy.Provider} {policy.PolicyType} policy?",
                "Remove", "Cancel");
            if (result)
            {
                InsuranceDataService.RemovePolicy(policy.Id);
                LoadData();
            }
        }

        public async Task PlayEntryAnimation()
        {
            AnimationHelper.PrepareForEntry(SummaryCard, CarCard, HouseCard, LifeCard, PetCard, RemindersCard);
            _ = AnimationHelper.AnimateEntry(SummaryCard, 0, 400);
            _ = AnimationHelper.AnimateEntry(CarCard, 100, 400);
            _ = AnimationHelper.AnimateEntry(HouseCard, 200, 400);
            _ = AnimationHelper.AnimateEntry(LifeCard, 100, 400);
            _ = AnimationHelper.AnimateEntry(PetCard, 200, 400);
            await AnimationHelper.AnimateEntry(RemindersCard, 300, 400);
        }
    }
}
