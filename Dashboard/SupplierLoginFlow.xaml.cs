using SmartCubeMobile.MockData;
using System.Globalization;
using System.Text.RegularExpressions;
#if WINDOWS
using Microsoft.Web.WebView2.Core;
#endif

namespace SmartCubeMobile.Dashboard
{
    public partial class SupplierLoginFlow : ContentPage
    {
        public event Action SupplierAdded;

        private readonly CultureInfo culture = new("en-GB");
        private SupplierInfo _selected;
        private bool _loggedIn;
        private List<ExtractedBill> _extractedBills = new();
        private readonly List<string> _capturedApiResponses = new();

        private static readonly List<SupplierInfo> Suppliers = new()
        {
            // Energy
            new("British Gas", "⚡", "#0072CE", "Energy", "https://www.britishgas.co.uk/identity/login"),
            new("EDF Energy", "⚡", "#FF6B00", "Energy", "https://www.edfenergy.com/myaccount/login"),
            new("Octopus Energy", "⚡", "#E51A5E", "Energy", "https://octopus.energy/dashboard/"),
            new("OVO Energy", "⚡", "#30BF54", "Energy", "https://my.ovoenergy.com/login"),
            new("E.ON Next", "⚡", "#EA1B2D", "Energy", "https://www.eonnext.com/login"),
            new("Scottish Power", "⚡", "#003DA5", "Energy", "https://www.scottishpower.co.uk/login/"),
            new("Shell Energy", "⚡", "#FFD500", "Energy", "https://shellenergy.co.uk/my-account/login"),
            new("Bulb", "⚡", "#79D17C", "Energy", "https://account.bulb.co.uk/login"),
            new("Utility Warehouse", "⚡", "#5C2D91", "Energy", "https://www.utilitywarehouse.co.uk/login"),
            new("So Energy", "⚡", "#00B4D8", "Energy", "https://www.so.energy/account/login"),

            // Water
            new("Thames Water", "💧", "#0072CE", "Water", "https://myaccount.thameswater.co.uk/"),
            new("Severn Trent", "💧", "#00A6D6", "Water", "https://myaccount.stwater.co.uk/login"),
            new("United Utilities", "💧", "#003865", "Water", "https://www.unitedutilities.com/my-account/sign-in/"),
            new("Yorkshire Water", "💧", "#0077C8", "Water", "https://www.yorkshirewater.com/my-account/"),
            new("Anglian Water", "💧", "#00447C", "Water", "https://myaccount.anglianwater.co.uk/"),
            new("South West Water", "💧", "#006B77", "Water", "https://www.southwestwater.co.uk/my-account/"),
            new("Welsh Water", "💧", "#003DA5", "Water", "https://www.dwrcymru.com/en/my-account"),
            new("Southern Water", "💧", "#005A9C", "Water", "https://myaccount.southernwater.co.uk/"),
            new("Northumbrian Water", "💧", "#0077B6", "Water", "https://www.nwl.co.uk/your-account/"),

            // Telecom
            new("BT", "📡", "#5514B4", "Telecom", "https://www.bt.com/mybt/login"),
            new("Sky", "📡", "#0072C9", "Telecom", "https://www.sky.com/signin"),
            new("Virgin Media", "📡", "#ED1C24", "Telecom", "https://www.virginmedia.com/my-virgin-media/sign-in"),
            new("EE", "📱", "#007B85", "Telecom", "https://id.ee.co.uk/id/login"),
            new("Three", "📱", "#FF7B7B", "Telecom", "https://www.three.co.uk/my3account"),
            new("Vodafone", "📱", "#E60000", "Telecom", "https://www.vodafone.co.uk/myvodafone/"),
            new("O2", "📱", "#0019A5", "Telecom", "https://accounts.o2.co.uk/signin"),
            new("Plusnet", "📡", "#FA6632", "Telecom", "https://www.plus.net/member-centre/login/"),
            new("TalkTalk", "📡", "#6C2D82", "Telecom", "https://www.talktalk.co.uk/myaccount"),

            // Other
            new("Council Tax", "🏛", "#4A5568", "Other", ""),
            new("TV Licence", "📺", "#2D3748", "Other", "https://www.tvlicensing.co.uk/cs/account/index.app"),
        };

        public SupplierLoginFlow()
        {
            InitializeComponent();
            ConfigureWebView();
            BuildSupplierList();
        }

        private void ConfigureWebView()
        {
#if WINDOWS
            SupplierWebView.HandlerChanged += (s, e) =>
            {
                if (SupplierWebView.Handler?.PlatformView is Microsoft.UI.Xaml.Controls.WebView2 wv2Ctrl)
                {
                    wv2Ctrl.CoreWebView2Initialized += (sender, args) =>
                    {
                        if (wv2Ctrl.CoreWebView2 != null)
                        {
                            wv2Ctrl.CoreWebView2.Settings.AreDefaultScriptDialogsEnabled = true;
                            wv2Ctrl.CoreWebView2.Settings.IsWebMessageEnabled = true;
                            wv2Ctrl.CoreWebView2.Settings.AreDevToolsEnabled = false;
                            wv2Ctrl.CoreWebView2.Settings.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/126.0.0.0 Safari/537.36";

                            wv2Ctrl.CoreWebView2.ServerCertificateErrorDetected += (s2, certArgs) =>
                            {
                                certArgs.Action = CoreWebView2ServerCertificateErrorAction.AlwaysAllow;
                            };

                            wv2Ctrl.CoreWebView2.WebResourceResponseReceived += async (s2, resArgs) =>
                            {
                                try
                                {
                                    var uri = resArgs.Request.Uri?.ToLower() ?? "";
                                    var ct = resArgs.Response.Headers?.GetHeader("Content-Type") ?? "";
                                    if (ct.Contains("json") && (
                                        uri.Contains("bill") || uri.Contains("payment") ||
                                        uri.Contains("account") || uri.Contains("balance") ||
                                        uri.Contains("usage") || uri.Contains("statement") ||
                                        uri.Contains("graphql") || uri.Contains("api") ||
                                        uri.Contains("energy") || uri.Contains("tariff") ||
                                        uri.Contains("meter") || uri.Contains("charge") ||
                                        uri.Contains("consumption") || uri.Contains("myaccount") ||
                                        uri.Contains("customer")))
                                    {
                                        var stream = await resArgs.Response.GetContentAsync();
                                        if (stream != null)
                                        {
                                            using var netStream = stream.AsStreamForRead();
                                            using var reader = new System.IO.StreamReader(netStream);
                                            var body = await reader.ReadToEndAsync();
                                            if (!string.IsNullOrEmpty(body) && body.Length < 500000)
                                            {
                                                _capturedApiResponses.Add(body);
                                            }
                                        }
                                    }
                                }
                                catch { }
                            };
                        }
                    };
                }
            };
#endif
        }

        private void BuildSupplierList()
        {
            foreach (var s in Suppliers)
            {
                var list = s.Category switch
                {
                    "Energy" => EnergyList,
                    "Water" => WaterList,
                    "Telecom" => TelecomList,
                    _ => OtherList,
                };

                var card = new Border
                {
                    BackgroundColor = Color.FromArgb("#1C2744"),
                    StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 10 },
                    Stroke = Color.FromArgb("#1E2D4A"),
                    Padding = new Thickness(14, 12),
                };

                var grid = new Grid
                {
                    ColumnDefinitions = new ColumnDefinitionCollection
                    {
                        new ColumnDefinition(new GridLength(36)),
                        new ColumnDefinition(GridLength.Star),
                        new ColumnDefinition(GridLength.Auto),
                    },
                    ColumnSpacing = 12,
                };

                var iconBorder = new Border
                {
                    BackgroundColor = Color.FromArgb(s.Colour + "30"),
                    StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 8 },
                    Stroke = Colors.Transparent,
                    WidthRequest = 36, HeightRequest = 36,
                    Content = new Label
                    {
                        Text = s.Icon, FontSize = 18,
                        HorizontalOptions = LayoutOptions.Center,
                        VerticalOptions = LayoutOptions.Center,
                    }
                };
                Grid.SetColumn(iconBorder, 0);

                var name = new Label
                {
                    Text = s.Name, TextColor = Color.FromArgb("#F1F5F9"),
                    FontSize = 14, FontAttributes = FontAttributes.Bold,
                    VerticalOptions = LayoutOptions.Center,
                };
                Grid.SetColumn(name, 1);

                var arrow = new Label
                {
                    Text = "→", TextColor = Color.FromArgb("#64748B"),
                    FontSize = 16, VerticalOptions = LayoutOptions.Center,
                };
                Grid.SetColumn(arrow, 2);

                grid.Children.Add(iconBorder);
                grid.Children.Add(name);
                grid.Children.Add(arrow);
                card.Content = grid;

                var supplier = s;
                card.GestureRecognizers.Add(new TapGestureRecognizer
                {
                    Command = new Command(async () =>
                    {
                        await card.ScaleTo(0.97, 80, Easing.CubicOut);
                        await card.ScaleTo(1.0, 80, Easing.CubicOut);
                        SelectSupplier(supplier);
                    })
                });

                list.Children.Add(card);
            }
        }

        private async void SelectSupplier(SupplierInfo supplier)
        {
            _selected = supplier;
            _loggedIn = false;
            _capturedApiResponses.Clear();

            if (string.IsNullOrEmpty(supplier.LoginUrl))
            {
                var flow = new AddSupplierFlow();
                flow.SupplierAdded += () => SupplierAdded?.Invoke();
                await Navigation.PushAsync(flow);
                return;
            }

            PageTitle.Text = supplier.Name;
            PageSubtitle.Text = "Log in to your account";
            ExtractBtn.IsVisible = false;

            SupplierWebView.Source = new UrlWebViewSource { Url = supplier.LoginUrl };

            Step1.IsVisible = false;
            Step2.IsVisible = true;
        }

        private void OnWebViewNavigating(object sender, WebNavigatingEventArgs e)
        {
            LoadingIndicator.IsRunning = true;
            UrlLabel.Text = e.Url;
        }

        private async void OnWebViewNavigated(object sender, WebNavigatedEventArgs e)
        {
            LoadingIndicator.IsRunning = false;
            UrlLabel.Text = e.Url;

            if (e.Result == WebNavigationResult.Success && _selected != null)
            {
                var url = e.Url?.ToLower() ?? "";

                // Always show the extract button once past the initial login URL
                if (!_loggedIn)
                {
                    var loginUrl = (_selected.LoginUrl ?? "").ToLower();
                    var isStillOnLoginPage = !string.IsNullOrEmpty(loginUrl) && url == loginUrl;
                    var hasLoginInUrl = url.Contains("/login") || url.Contains("/signin") || url.Contains("/sign-in")
                                    || url.Contains("/authenticate");
                    // Detect post-login: URL changed away from login, or landed on account/dashboard pages
                    var isPostLogin = url.Contains("my-account") || url.Contains("myaccount") || url.Contains("dashboard")
                                    || url.Contains("overview") || url.Contains("account-summary")
                                    || url.Contains("/home") || url.Contains("/account");

                    if ((!isStillOnLoginPage && !hasLoginInUrl) || isPostLogin)
                    {
                        _loggedIn = true;
                        PageSubtitle.Text = "Logged in — adding supplier...";
                        AddSupplierFromLogin();

                        ExtractBtn.Text = "Scrape Bills";
                        ExtractBtn.IsVisible = true;

                        await Task.Delay(3000);
                        await AutoExtractBills();
                    }
                }
                else
                {
                    // Already logged in — page navigation within the portal
                    // Re-enable scrape button for each new page
                    ExtractBtn.Text = "Scrape Bills";
                    ExtractBtn.IsVisible = true;
                    ExtractBtn.IsEnabled = true;
                }
            }
        }

        private void AddSupplierFromLogin()
        {
            var type = _selected.Category switch
            {
                "Energy" => "Electricity",
                "Water" => "Water",
                "Telecom" => _selected.Icon == "\U0001F4F1" ? "Mobile" : "Broadband",
                _ => "Other",
            };

            var existing = MockDataService.GetSuppliers()
                .Any(s => s.Name == _selected.Name);

            if (!existing)
            {
                MockDataService.AddSupplier(new MockSupplier
                {
                    Name = _selected.Name,
                    Type = type,
                    Icon = _selected.Icon,
                    Tariff = "Connected Account",
                    TariffDetail = $"Linked {DateTime.Now:dd MMM yyyy}",
                    MonthlyCost = 0m,
                    AccountRef = "",
                });
                SupplierAdded?.Invoke();
            }
        }

        private async Task AutoExtractBills()
        {
            PageSubtitle.Text = "Extracting bill data...";

            try
            {
                if (await TryExtractAndSave())
                    return;

                PageSubtitle.Text = "Waiting for page data...";
                await Task.Delay(3000);

                if (await TryExtractAndSave())
                    return;

                PageSubtitle.Text = $"Supplier added — {_capturedApiResponses.Count} API(s) captured. Navigate to your bills page and tap Scrape Bills.";
            }
            catch
            {
                PageSubtitle.Text = "Supplier added — navigate to bills page and tap Scrape Bills";
            }
        }

        private async Task<bool> TryExtractAndSave()
        {
            if (TryParseFromCapturedApi() && _extractedBills.Count > 0)
            {
                SaveExtractedBills();
                PageSubtitle.Text = $"Done — {_extractedBills.Count} bill(s) added";
                await Task.Delay(1500);
                SupplierAdded?.Invoke();
                await Navigation.PopAsync();
                return true;
            }

            var js = GetSupplierScraper(_selected?.Name ?? "");
            var result = await SupplierWebView.EvaluateJavaScriptAsync(js);

            if (!string.IsNullOrEmpty(result))
            {
                result = result.Trim('"').Replace("\\\"", "\"").Replace("\\\\", "\\");
                ParseExtractedData(result);

                if (_extractedBills.Count > 0)
                {
                    SaveExtractedBills();
                    PageSubtitle.Text = $"Done — {_extractedBills.Count} bill(s) added";
                    await Task.Delay(1500);
                    SupplierAdded?.Invoke();
                    await Navigation.PopAsync();
                    return true;
                }
            }

            return false;
        }

        private void SaveExtractedBills()
        {
            var type = _selected.Category switch
            {
                "Energy" => "Electricity",
                "Water" => "Water",
                "Telecom" => _selected.Icon == "\U0001F4F1" ? "Mobile" : "Broadband",
                _ => "Other",
            };

            foreach (var bill in _extractedBills)
            {
                DateTime billDate;
                if (!DateTime.TryParse(bill.DateStr, culture, DateTimeStyles.None, out billDate))
                    DateTime.TryParse(bill.DateStr, CultureInfo.InvariantCulture, DateTimeStyles.None, out billDate);
                if (billDate == default) billDate = DateTime.Now;

                MockDataService.AddBill(new MockUtilityBill
                {
                    BillDate = billDate,
                    Supplier = _selected.Name,
                    FuelType = type,
                    Amount = bill.Amount,
                    UnitsUsed = 0,
                    Period = billDate.ToString("MMM yyyy"),
                    Address = "",
                });
            }

            var suppliers = MockDataService.GetSuppliers();
            var supplier = suppliers.FirstOrDefault(s => s.Name == _selected.Name);
            if (supplier != null && _extractedBills.Count > 0)
            {
                supplier.MonthlyCost = Math.Round(_extractedBills.Average(b => b.Amount), 2);
            }
        }

        private async void OnExtractClicked(object sender, EventArgs e)
        {
            ExtractBtn.IsEnabled = false;
            ExtractBtn.Text = "Scraping...";

            try
            {
                if (TryParseFromCapturedApi() && _extractedBills.Count > 0)
                {
                    SaveExtractedBills();
                    SupplierAdded?.Invoke();
                    await DisplayAlert("Bills Found", $"{_extractedBills.Count} bill(s) added from API data.", "OK");
                    await Navigation.PopAsync();
                    return;
                }

                var js = GetSupplierScraper(_selected?.Name ?? "");
                var result = await SupplierWebView.EvaluateJavaScriptAsync(js);
                var rawResult = result ?? "(null)";

                if (!string.IsNullOrEmpty(result))
                {
                    result = result.Trim('"').Replace("\\\"", "\"").Replace("\\\\", "\\");
                    ParseExtractedData(result);

                    if (_extractedBills.Count > 0)
                    {
                        SaveExtractedBills();
                        SupplierAdded?.Invoke();
                        await DisplayAlert("Bills Found", $"{_extractedBills.Count} bill(s) added from page scrape.", "OK");
                        await Navigation.PopAsync();
                        return;
                    }
                }

                // Dump all captured APIs to a file for analysis
                try
                {
                    var dumpPath = System.IO.Path.Combine(
                        Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                        "edf_api_dump.txt");
                    var sb = new System.Text.StringBuilder();
                    sb.AppendLine($"URL: {UrlLabel.Text}");
                    sb.AppendLine($"APIs captured: {_capturedApiResponses.Count}");
                    sb.AppendLine($"Scraper returned: {rawResult}");
                    sb.AppendLine();
                    for (int i = 0; i < _capturedApiResponses.Count; i++)
                    {
                        sb.AppendLine($"=== API {i + 1} ===");
                        sb.AppendLine(Truncate(_capturedApiResponses[i], 2000));
                        sb.AppendLine();
                    }
                    System.IO.File.WriteAllText(dumpPath, sb.ToString());
                    await DisplayAlert("No Bills Found",
                        $"APIs captured: {_capturedApiResponses.Count}\n" +
                        $"URL: {UrlLabel.Text}\n\n" +
                        $"Scraper: {Truncate(rawResult, 200)}\n\n" +
                        $"Full API dump saved to Desktop:\nedf_api_dump.txt",
                        "OK");
                }
                catch (Exception dumpEx)
                {
                    await DisplayAlert("No Bills Found",
                        $"APIs: {_capturedApiResponses.Count}, Scraper: {Truncate(rawResult, 200)}\nDump error: {dumpEx.Message}", "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Scrape Error", ex.Message, "OK");
            }

            ExtractBtn.IsEnabled = true;
            ExtractBtn.Text = "Scrape Bills";
        }

        private static string Truncate(string s, int max)
        {
            if (string.IsNullOrEmpty(s)) return s;
            return s.Length <= max ? s : s.Substring(0, max) + "...";
        }

        private bool TryParseFromCapturedApi()
        {
            _extractedBills.Clear();

            foreach (var json in _capturedApiResponses)
            {
                try
                {
                    var token = Newtonsoft.Json.Linq.JToken.Parse(json);
                    SearchForBills(token);
                    if (_extractedBills.Count > 0)
                        return true;
                }
                catch { }
            }
            return false;
        }

        private void SearchForBills(Newtonsoft.Json.Linq.JToken token)
        {
            if (_extractedBills.Count >= 24) return;

            if (token is Newtonsoft.Json.Linq.JArray arr)
            {
                // Check if this array contains bill-like objects
                var bills = new List<ExtractedBill>();
                foreach (var item in arr)
                {
                    if (item is Newtonsoft.Json.Linq.JObject itemObj)
                    {
                        var b = TryExtractBillFromObject(itemObj);
                        if (b != null) bills.Add(b);
                    }
                }
                if (bills.Count >= 2)
                {
                    _extractedBills.AddRange(bills);
                    return;
                }

                // Recurse into array items
                foreach (var item in arr)
                    SearchForBills(item);
            }
            else if (token is Newtonsoft.Json.Linq.JObject obj)
            {
                foreach (var prop in obj.Properties())
                {
                    var name = prop.Name.ToLower();
                    // Prioritise properties that sound like bill collections
                    if (prop.Value is Newtonsoft.Json.Linq.JArray propArr &&
                        (name.Contains("bill") || name.Contains("statement") || name.Contains("invoice") ||
                         name.Contains("payment") || name.Contains("transaction") || name.Contains("charge") ||
                         name.Contains("history") || name.Contains("ledger")))
                    {
                        var bills = new List<ExtractedBill>();
                        foreach (var item in propArr)
                        {
                            if (item is Newtonsoft.Json.Linq.JObject itemObj)
                            {
                                var b = TryExtractBillFromObject(itemObj);
                                if (b != null) bills.Add(b);
                            }
                        }
                        if (bills.Count >= 1)
                        {
                            _extractedBills.AddRange(bills);
                            return;
                        }
                    }

                    // Recurse into child objects/arrays
                    if (prop.Value is Newtonsoft.Json.Linq.JObject || prop.Value is Newtonsoft.Json.Linq.JArray)
                        SearchForBills(prop.Value);

                    if (_extractedBills.Count >= 24) return;
                }
            }
        }

        private ExtractedBill TryExtractBillFromObject(Newtonsoft.Json.Linq.JObject obj)
        {
            var amountNames = new[] {
                "amount", "total", "value", "billAmount", "totalAmount", "netAmount",
                "grossAmount", "cost", "charge", "totalGrossAmount", "totalNetAmount",
                "amountOwed", "chargesTotal", "balance", "net", "gross",
                "totalCharges", "totalCost"
            };
            var dateNames = new[] {
                "date", "billDate", "invoiceDate", "statementDate", "createdDate",
                "issueDate", "dueDate", "periodEnd", "billingDate", "issuedDate",
                "fromDate", "toDate", "startDate", "endDate", "periodFrom", "periodTo",
                "billedFrom", "billedTo", "createdAt", "updatedAt"
            };

            decimal? amount = null;
            string dateStr = null;

            foreach (var n in amountNames)
            {
                var val = FindTokenDeep(obj, n);
                if (val == null) continue;
                var s = val.ToString().Replace("£", "").Replace(",", "").Trim();
                if (decimal.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out var a))
                {
                    if (a == 0) continue;
                    // EDF Kraken API uses pence for some values — amounts over 500 are likely pence
                    amount = Math.Abs(a) > 500 ? Math.Round(Math.Abs(a) / 100m, 2) : Math.Abs(a);
                    break;
                }
            }

            foreach (var n in dateNames)
            {
                var val = FindTokenDeep(obj, n);
                if (val == null) continue;
                var s = val.ToString().Trim();
                if (string.IsNullOrEmpty(s) || s == "null") continue;
                if (DateTime.TryParse(s, CultureInfo.InvariantCulture, DateTimeStyles.None, out var d))
                {
                    dateStr = d.ToString("dd MMM yyyy");
                    break;
                }
                if (DateTime.TryParse(s, culture, DateTimeStyles.None, out var d2))
                {
                    dateStr = d2.ToString("dd MMM yyyy");
                    break;
                }
                if (s.Length >= 6)
                {
                    dateStr = s;
                    break;
                }
            }

            if (amount.HasValue && amount.Value > 0)
                return new ExtractedBill { Amount = amount.Value, DateStr = dateStr ?? "" };

            return null;
        }

        private static Newtonsoft.Json.Linq.JToken FindTokenDeep(Newtonsoft.Json.Linq.JObject obj, string name)
        {
            // Direct property first
            var val = obj[name];
            if (val != null && val.Type != Newtonsoft.Json.Linq.JTokenType.Null &&
                val.Type != Newtonsoft.Json.Linq.JTokenType.Object &&
                val.Type != Newtonsoft.Json.Linq.JTokenType.Array)
                return val;

            // Case-insensitive search
            foreach (var prop in obj.Properties())
            {
                if (prop.Name.Equals(name, StringComparison.OrdinalIgnoreCase) &&
                    prop.Value.Type != Newtonsoft.Json.Linq.JTokenType.Null &&
                    prop.Value.Type != Newtonsoft.Json.Linq.JTokenType.Object &&
                    prop.Value.Type != Newtonsoft.Json.Linq.JTokenType.Array)
                    return prop.Value;
            }

            return null;
        }

        private string FindJsonValue(Newtonsoft.Json.Linq.JObject obj, params string[] names)
        {
            foreach (var n in names)
            {
                var val = obj.SelectToken(n) ?? obj.SelectToken($"data.{n}");
                if (val != null)
                {
                    var s = val.ToString().Trim();
                    if (!string.IsNullOrEmpty(s) && s != "0" && s != "null") return s;
                }
            }
            return null;
        }

        private static string GetSupplierScraper(string supplierName)
        {
            return BuildScraper();
        }

        private static string BuildScraper()
        {
            return
                "try{" +
                "var d={bills:[],account:'',balance:'',usage:'',title:document.title||'',len:(document.body.innerText||'').length};" +
                "var t=document.body.innerText||'';" +
                "var am=t.match(/(?:balance|amount\\s*due|you\\s*owe|please\\s*pay)[^£]*£([\\d,]+\\.?\\d*)/i);" +
                "if(am)d.balance=am[1].replace(/,/g,'');" +
                "var um=t.match(/(\\d[\\d,]*\\.?\\d*)\\s*kWh/i);" +
                "if(um)d.usage=um[1].replace(/,/g,'')+' kWh';" +
                "var els=document.querySelectorAll('tr,li,div,article,section,a,span,p');" +
                "for(var i=0;i<els.length&&d.bills.length<24;i++){" +
                "var e=els[i];if(e.children.length>10)continue;" +
                "var x=e.innerText||'';if(x.length>500||x.length<5)continue;" +
                "var a=x.match(/£(\\d[\\d,]*\\.?\\d{0,2})/);" +
                "var dt=x.match(/(\\d{1,2})\\s*(Jan(?:uary)?|Feb(?:ruary)?|Mar(?:ch)?|Apr(?:il)?|May|Jun(?:e)?|Jul(?:y)?|Aug(?:ust)?|Sep(?:tember)?|Oct(?:ober)?|Nov(?:ember)?|Dec(?:ember)?)\\s*(\\d{2,4})/i);" +
                "if(!dt)dt=x.match(/(\\d{1,2})[\\/\\-](\\d{1,2})[\\/\\-](\\d{2,4})/);" +
                "if(a&&dt){" +
                "var v=a[1].replace(/,/g,'');" +
                "var dd=dt[0];" +
                "var dup=d.bills.some(function(b){return b.amount===v&&b.date===dd;});" +
                "if(!dup&&parseFloat(v)>0)d.bills.push({amount:v,date:dd});" +
                "}}" +
                "if(d.bills.length===0){" +
                "var amts=t.match(/£\\d[\\d,]*\\.\\d{2}/g)||[];" +
                "var dts=t.match(/\\d{1,2}\\s+(?:Jan(?:uary)?|Feb(?:ruary)?|Mar(?:ch)?|Apr(?:il)?|May|Jun(?:e)?|Jul(?:y)?|Aug(?:ust)?|Sep(?:tember)?|Oct(?:ober)?|Nov(?:ember)?|Dec(?:ember)?)\\s+\\d{2,4}/gi)||[];" +
                "var c=Math.min(amts.length,dts.length,24);" +
                "for(var j=0;j<c;j++){d.bills.push({amount:amts[j].replace(/[£,]/g,''),date:dts[j]});}}" +
                "JSON.stringify(d);" +
                "}catch(ex){JSON.stringify({error:ex.message,bills:[]});}";
        }

        private void ParseExtractedData(string json)
        {
            try
            {
                var data = Newtonsoft.Json.Linq.JObject.Parse(json);
                var bills = data["bills"] as Newtonsoft.Json.Linq.JArray;

                _extractedBills.Clear();

                if (bills != null && bills.Count > 0)
                {
                    foreach (var b in bills)
                    {
                        if (decimal.TryParse(b["amount"]?.ToString(), NumberStyles.Any, CultureInfo.InvariantCulture, out var amount))
                        {
                            _extractedBills.Add(new ExtractedBill
                            {
                                Amount = amount,
                                DateStr = b["date"]?.ToString() ?? "",
                            });
                        }
                    }
                }
            }
            catch { }
        }

        private async void OnBackClicked(object sender, EventArgs e)
        {
            if (Step2.IsVisible)
            {
                Step2.IsVisible = false;
                Step1.IsVisible = true;
                ExtractBtn.IsVisible = false;
                PageTitle.Text = "Connect Supplier";
                PageSubtitle.Text = "Choose your supplier";
                _loggedIn = false;
                return;
            }
            await Navigation.PopAsync();
        }

        private record SupplierInfo(string Name, string Icon, string Colour, string Category, string LoginUrl);
        private class ExtractedBill
        {
            public decimal Amount { get; set; }
            public string DateStr { get; set; }
        }
    }
}
