using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace SmartCubeMobile
{
    public class Account
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("balance")]
        public string Balance { get; set; }

        [JsonPropertyName("sortCode")]
        public string SortCode { get; set; }
        [JsonPropertyName("accountNumber")]
        public string AccountNumber { get; set; }
        [JsonPropertyName("href")]
        public string Href { get; set; }
    }

    internal class NationwideMAUI
    {
        private readonly WebView FinanceWebView;
        private readonly MainViewModel ourviewmodel;
        private readonly FinanceViewModel financeviewmodel;
        private readonly TaskCompletionSource<bool> tcs1001;
        private readonly string startUrl;
        private readonly string notificationTitle;
        private readonly string notificationPrefix;
        private readonly short notificationTagLength;
        private readonly short institutionCode;
        private readonly short brandCode;
        private readonly List<SmartFinance.Transaction_Groups>transactionGroups;
        private readonly List<SmartFinance.Transaction_Types> transactionTypes;

        public NationwideMAUI(
                                WebView financeWebView,
                                MainViewModel ourviewmodel,
                                FinanceViewModel financeviewmodel,
                                TaskCompletionSource<bool> tcs1001,
                                string StartUrl,
                                string NotificationTitle,
                                string NotificationPrefix,
                                short NotificationTagLength,
                                short InstitutionCode,
                                short BrandCode,
                                List<SmartFinance.Transaction_Groups> TransactionGroups,
                                List<SmartFinance.Transaction_Types> TransactionTypes)
        {
            this.FinanceWebView = financeWebView;
            this.ourviewmodel = ourviewmodel;
            this.financeviewmodel = financeviewmodel;
            this.tcs1001 = tcs1001;
            this.startUrl = StartUrl;
            this.notificationTitle = NotificationTitle;
            this.notificationPrefix = NotificationPrefix;
            this.notificationTagLength = NotificationTagLength;
            this.institutionCode = InstitutionCode;
            this.brandCode = BrandCode;
            this.transactionGroups = TransactionGroups;
            this.transactionTypes = TransactionTypes;
        }

        internal string owner = "";

        internal bool StepInProgress;
        internal int Step;
        internal string CustomerNumber = "",
                        BirthDay = "",
                        BirthMonth = "",
                        BirthYear = "",
                        Passcode = "";
        internal string loginHref = "";
        internal bool loggedIn = false;
        internal List<AccountLink> AccountLinks { get; set; } = new();
        internal int CurrentAccountIndex { get; set; } = 0;
        //internal TaskCompletionSource<bool> tcs;

        internal string SortCode = "";
        internal string AccountNo = "";
        internal char categoryCode = SmartParametersV2016.defaultChar;
        internal string symbol = "";

        // The vitally important UDPRN!!
        // You CANNOT add ACCOUNTS, TRANSACTIONS or
        // TRANSACTIONSCATEGORIES without a valid
        //     ====> UDPRN <==== 
        // It is **all important**
        internal string UDPRN = "";
        internal string OWETP = "";

        private bool pageLoaded = false;

        private bool fuckers = false;
        private string lastUrl = "";

        private List<Account> accountsList = new();
        private int currentAccount = 0;

        private Dictionary<short, string> 
        // Initialize your scripts dictionary
        javascriptTemplates = new Dictionary<short, string>
            {
                { 01, @"(function () {let meta = document.querySelector('meta[name=""viewport""]');if (!meta) {meta = document.createElement('meta');meta.name = 'viewport';document.head.appendChild(meta);}meta.content = 'width=device-width, initial-scale=1.0';})();" },
                { 10, @"(function() {{ try {{ let scripts = document.querySelectorAll('script'); for (let script of scripts) {{ if (script.textContent.includes('logInLink')) {{ let match = script.textContent.match(/""logInLink""\s*:\s*\{{[^}}]+\}}/); if (match && match[0]) {{ let jsonText = '{{' + match[0] + '}}'; let json = JSON.parse(jsonText); return json.logInLink.url; }} }} }} }} catch (e) {{ return ''; }} return ''; }})();" },
                { 21, @"(function() {{ var input = document.querySelector('input[name=""CustomerNumber""]'); if (input) {{ input.focus(); input.value = '{0}'; input.dispatchEvent(new Event('input', {{bubbles: true }})); input.dispatchEvent(new Event('change', {{bubbles: true }})); input.blur(); return 'Input filled successfully'; }} return 'Input not found'; }})();" },
                { 22, @"(function() {{ var input = document.querySelector('input[name=""DateOfBirthDay""]'); if (input) {{ input.focus(); input.value = '{0}'; input.dispatchEvent(new Event('input', {{ bubbles: true }})); input.dispatchEvent(new Event('change', {{ bubbles: true }})); input.blur(); return 'Input filled successfully'; }} return 'Input not found'; }})();" },
                { 23, @"(function() {{ var input = document.querySelector('input[name=""DateOfBirthYear""]'); if (input) {{ input.focus(); input.value = '{0}'; input.dispatchEvent(new Event('input', {{ bubbles: true }})); input.dispatchEvent(new Event('change', {{ bubbles: true }})); input.blur(); return 'Input filled successfully'; }} return 'Input not found'; }})();" },
                { 24, @"(function() {{ var select = document.querySelector('select[name=""DateOfBirthMonth""]'); if (select) {{ select.value = '{0}'; var event = new Event('change', {{ bubbles: true }}); select.dispatchEvent(event); return 'Month selected'; }} return 'Month dropdown not found'; }})();" },
                { 25, @"(function() {{ var btns = Array.from(document.querySelectorAll('button')); var btn = btns.find(b => b.textContent.trim().toLowerCase() === 'continue'); if (btn) {{ btn.click(); return 'Continue clicked'; }} return 'Button not found'; }})();" },
                { 31, @"(() => {{ const radio = document.querySelector(""input[type='radio'][value='PassNumberAndSMSOTP']""); if (radio && radio.offsetParent !== null) {{ radio.scrollIntoView({{ behavior: 'smooth', block: 'center' }}); radio.click(); return 'Clicked'; }} else {{ return 'NotFound'; }} }})()" },
                { 33, @"(function() {{ var btns = Array.from(document.querySelectorAll('button')); var btn = btns.find(b => b.textContent.trim().toLowerCase() === 'continue'); if (btn) {{ btn.click(); return 'Clicked'; }} return 'ButtonNotFound'; }})();" },
                { 41, @"(function() {{ const welcomeElem = document.getElementById('welcome-message'); return welcomeElem && welcomeElem.innerText ? welcomeElem.innerText : ''; }})();" },
                { 42, @"(() => {{ const link = Array.from(document.querySelectorAll('a[href]')) .find(a => a.href.includes('{0}')); if (link) {{ link.click(); return 'ClickedAccountList'; }} return 'NotFound'; }})();" },
                { 43, @"(function() {{ let results = []; let activeElem = document.querySelector('.active'); if (!activeElem) return JSON.stringify(results); let anchors = activeElem.getElementsByTagName('a'); for(let i=0; i<anchors.length; i++) {{ let href = anchors[i].href || ''; if (href.includes('/AccountList/Account/RedirectToDefaultPage')) {{ results.push({{ href: href, innerHTML: anchors[i].innerHTML }}); }} }} return JSON.stringify(results);}})();" },
                { 44, @"(() => {{ const link = Array.from(document.querySelectorAll('a[href]')) .find(a => a.href.includes('{0}')); if (link) {{ link.click(); return 'ClickedPhoneAddress'; }} return 'NotFound'; }})();" },
                { 45, @"(function() {{ let addresses = document.getElementsByTagName('address'); if (addresses.length === 0) return ''; let addrText = addresses[0].innerText || ''; addrText = addrText.replace(/\r?\n/g, ' ').trim(); addrText = addrText.replace(/\s{2,}/g, ' '); return addrText; }})();" },
                { 46, @"(function() {{ let el = document.getElementById('CurrentEmailAddress'); return el ? el.value : ''; }})();" },
                { 47, @"(function() {{ let el = document.getElementById('MobilePhoneNumber'); return el ? el.value : ''; }})();" }

            };

        internal async Task RunNationwide(SmartFinance.Logins loginInfo,
                                            string Parameter1,
                                            string Parameter2,
                                            string Parameter3,
                                            int loginMethod)
        {            
            try
            {
                FinanceWebView.IsVisible = true;
                CustomerNumber = Parameter1;
                BirthDay = Parameter2.Substring(0, 2);
                BirthMonth = SmartParametersV2016.months[int.Parse(Parameter2.Substring(2, 2)) - 1];
                BirthYear = Parameter2.Substring(4, 4);
                Passcode = Parameter3;
                //financeviewmodel.financeWebView.Source = startUrl;
                FinanceWebView.Navigating += OnNavigating;
                FinanceWebView.Navigated += OnNavigated;                
                Step = 2;
                StepInProgress = false;
                FinanceWebView.Source =
                "https://onlinebanking.nationwide.co.uk/AccessManagement/IdentifyCustomer/IdentifyCustomer";
            }
            catch (Exception ex)
            {
                financeviewmodel.errorMessage =
                    $"WebView initialization failed: {ex.Message}";

                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel,
                                                        financeviewmodel.errorMessage);
            }
        }
#if !WINFORMS
        
        internal async void OnNavigating(object sender,
                                        WebNavigatingEventArgs e)
        {
            Console.WriteLine(e.Url.ToString());
            
        }
        internal async void OnNavigated(object sender,
                                        WebNavigatedEventArgs e)
        {
            Console.WriteLine(e.Url.ToString());
            pageLoaded = true;
            //if (e.Result != WebNavigationResult.Success)
            //{
            //    if (e.Url.ToString() == "https://onlinebanking.nationwide.co.uk/CustomerIB/MaintainTelephoneAndAddress") 
            //    {
            //        FinanceWebView.Navigated -= OnNavigated;

            //        await RunStep4();
            //    }
            //    return;
            //}
            //if (fuckers)// && !e.Url.ToString().Contains("/ib/"))
            //{
            //    if (e.Url.ToString() == "https://onlinebanking.nationwide.co.uk/CustomerIB/MaintainTelephoneAndAddress")  //   "https://onlinebanking.nationwide.co.uk/Customisation/Customisation/MyDetailsAndSettings")
            //    {
            //        FinanceWebView.Navigated -= OnNavigated;

            //        await RunStep4();
            //        //FinanceWebView.Source = "https://onlinebanking.nationwide.co.uk/ib/settings";
            //        return;
            //    }
            //    //if (e.Url.ToString().Contains("ib/settings"))
            //    //{
            //    //    //FinanceWebView.Source = "https://onlinebanking.nationwide.co.uk/Customisation/Customisation/MyDetailsAndSettings";

            //    //    FinanceWebView.Source = "https://onlinebanking.nationwide.co.uk/CustomerIB/MaintainTelephoneAndAddress";

            //    //    return;


            //    //   //                 await RunStep4();
            //    //    //Console.WriteLine("here");
            //    //}
            //    //Console.WriteLine(e.Url.ToString());
            //}
            try
            {
                if (StepInProgress)
                {
                Console.WriteLine(e.Url.ToString());
                return;
                }

                    StepInProgress = true;

                    switch (Step)
                    {
                        case 0:
                            await RunStep0();
                            break;

                        case 1:
                            // Not needed/used in WPF
                            //RunStep1(_webView);
                            break;

                        case 2:
                            await RunStep2(CustomerNumber,
                                            BirthDay,
                                            BirthMonth,
                                            BirthYear);
                            break;

                        case 3:
                            await RunStep3();
                            break;

                        case 35:
                            await RunStep35();
                            if (accountsList.Count == 0)
                            {
                                await CleanupAndExitAsync();
                                return;
                            }
                            // Start looping
                            //currentAccount = 0;
                            //Step = 6;
                            //FinanceWebView.Source = accountsList[currentAccount].Href;
                        break;

                    //case 36:
                    //    await RunStep36();
                    //    break;
                    case 4:
                            await RunStep4();
                            break;

                        case 45:
                            await RunStep45();
                            break;

                        case 5:
                            bool result5 = await RunStep5();
                            if (!result5)
                            {
                                await CleanupAndExitAsync();
                                return;
                            }
                            break;

                        case 6:
                            //bool result6 = await RunStep6(SortCode,
                            //                                AccountNo,
                            //                                categoryCode,
                            //                                symbol);
                            //if (!result6)
                            //{
                            //    await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Finished processing accounts.");
                            //    await CleanupAndExitAsync();
                            //    return;
                            //}

                            var account = accountsList[currentAccount];

                            bool result6 = await RunStep6(
                                account.SortCode,
                                account.AccountNumber,
                                categoryCode,
                                symbol);

                            if (!result6)
                            {
                                await CleanupAndExitAsync();
                                return;
                            }

                            currentAccount++;

                            if (currentAccount >= accountsList.Count)
                            {
                                await SmartRoutinesV2018.TextBlockUpdate(
                                    ourviewmodel,
                                    "Finished processing accounts.");

                                await CleanupAndExitAsync();
                                return;
                            }

                            FinanceWebView.Source = accountsList[currentAccount].Href;
                            // Wait for Navigated event again
                            break;
                        default:
                            break;
                    }
                }
                catch (Exception ex)
                {
                    await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, $"Navigation Error: {ex.Message}");
                }
                finally
                {
                    StepInProgress = false;
                }
            return;
        }

        internal async Task CleanupAndExitAsync()
        {
            FinanceWebView.Navigated -= OnNavigated;
            StepInProgress = false;

            await NationwideCommon.LogoutAsync(FinanceWebView, ourviewmodel, financeviewmodel, tcs1001, loggedIn);
            return;
        }

        internal string GetJavaScript(short key, Dictionary<short, string> templates, params object[] args)
        {
            if (!templates.TryGetValue(key, out var template))
            {
                throw new ArgumentException($"JavaScript template for key '{key}' not found.");
            }
            string wot = "";
            try
            {
                wot = string.Format(template, args);
            }
            catch (Exception ex)
            {
                // Should never happen
                financeviewmodel.errorMessage = ex.Message;
            }
            return wot;
        }

        private async Task<bool> RunStep0()
        {
            //string script = GetJavaScript(financeviewmodel, 10, javascriptTemplates, "");
            try
            {
                string result = await FinanceWebView.EvaluateJavaScriptAsync(@"document.readyState");

                string result1 = await FinanceWebView.EvaluateJavaScriptAsync(@"document.querySelectorAll('button')[1].innerText");
                if (result1 == "Log in")
                {
                    string resultx = await FinanceWebView.EvaluateJavaScriptAsync(@"const btn = document.querySelectorAll('button')[1];if (btn) {btn.scrollIntoView({ block: 'center' });btn.dispatchEvent(new MouseEvent('pointerdown', { bubbles: true }));btn.dispatchEvent(new MouseEvent('mousedown', { bubbles: true }));btn.dispatchEvent(new MouseEvent('mouseup', { bubbles: true }));btn.dispatchEvent(new MouseEvent('click', { bubbles: true }));}");
                    Console.WriteLine(resultx);
                    
                    var result2 = await FinanceWebView.EvaluateJavaScriptAsync(@"document.querySelectorAll('button')[1] ? 'Found' : 'Not found'");
                    Console.WriteLine(result2);

                    var test = await FinanceWebView.EvaluateJavaScriptAsync("1+1");
                    Console.WriteLine(test);

                    var result3 = await FinanceWebView.EvaluateJavaScriptAsync(@"({url: window.location.href,frames: window.length,buttons: document.querySelectorAll('button').length,ready: document.readyState})");
                    Console.WriteLine(result3);

                    var test2 = await FinanceWebView.EvaluateJavaScriptAsync(@"document.body.innerHTML.length.toString()");
                    Console.WriteLine(test2);
                }
            }
            catch (Exception ex)
            {
                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Script error: " + ex.Message);
                StepInProgress = false;
                return false;
            }
            if (!string.IsNullOrEmpty(loginHref) && loginHref != "null")
            {
                try
                {
                    FinanceWebView.Source = new UrlWebViewSource
                    {
                        Url = loginHref
                    };
                }
                catch (UriFormatException ex)
                {
                    await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Invalid URI: " + ex.Message);
                    return false;
                }
            }
            Step = 2;
            StepInProgress = false;
            await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Step 0 executed");
            return true;
        }

        internal async Task<bool> RunStep2(string CustomerNumber,
                                            string BirthDay,
                                            string BirthMonth,
                                            string BirthYear)
        {
            try
            {
                // Fill Customer Number            
                string jsCustomer = GetJavaScript(21, javascriptTemplates, CustomerNumber);
                string customerResult = await FinanceWebView.EvaluateJavaScriptAsync(jsCustomer);
                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Customer Result: " + NationwideCommon.TrimQuotes(customerResult));
                // Fill Day
                string jsDay = GetJavaScript(22, javascriptTemplates, BirthDay);
                string dayResult = await FinanceWebView.EvaluateJavaScriptAsync(jsDay);
                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Day Result: " + NationwideCommon.TrimQuotes(dayResult));
                // Fill Year
                string jsYear = GetJavaScript(23, javascriptTemplates, BirthYear);
                string yearResult = await FinanceWebView.EvaluateJavaScriptAsync(jsYear);
                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Year Result: " + NationwideCommon.TrimQuotes(yearResult));
                // Select Month
                string jsMonth = GetJavaScript(24, javascriptTemplates, BirthMonth);
                string monthResult = await FinanceWebView.EvaluateJavaScriptAsync(jsMonth);
                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Month Result: " + NationwideCommon.TrimQuotes(monthResult));
                // Click "Continue" button
                string jsClick = GetJavaScript(25, javascriptTemplates, 0);
                string clickResult = await FinanceWebView.EvaluateJavaScriptAsync(jsClick);
                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Click Result: " + NationwideCommon.TrimQuotes(clickResult));
            }
            catch (Exception ex)
            {
                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "RunStep2 Error: " + ex.Message);
                return false;
            }
            Step = 3;
            StepInProgress = false;
            return true;
        }

        internal async Task<bool> RunStep3()
        {
            await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Status: Step 3");
            string jsscript = GetJavaScript(31, javascriptTemplates, 0);
            // Move on
            try
            {
                string resultRadio = await FinanceWebView.EvaluateJavaScriptAsync(jsscript);
                if (resultRadio.Contains("Clicked"))
                {
                    await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Step 3: Radio button clicked");
                }
                else
                {
                    financeviewmodel.errorMessage = "Step3: Radio button not found";
                    await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, financeviewmodel.errorMessage);
                    return false;   // Give up
                }
            }
            catch (Exception ex)
            {
                financeviewmodel.errorMessage = "Failure: Select radio button " + ex.Message;
                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, financeviewmodel.errorMessage);
                return false;
            }

            string logAction = "";
            string errorMessage = "";
            if (!await NationwideCommon.NationwideFindPasscodes(FinanceWebView,
                                                                ourviewmodel,
                                                                financeviewmodel,
                                                                Passcode,
                                                                la => logAction = la,
                                                                em => errorMessage = em))
            {
                // Some error
                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Step 3: Find passcodes error");
                return false;
            }
            await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Step 3: Passcodes set");

            NotificationListener listener = new NotificationListener(8080);

            // Start DCP to receive OTP etc. and start listening
            Task <NotificationPacket> waitTask = listener.WaitForNotificationAsync(
                                                        TimeSpan.FromMinutes(3),
                                                        "NATIONWIDE");
            await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, $"Step3 Waiting for OTP...");

            // Click Continue
            string jsClickContinue = GetJavaScript(33, javascriptTemplates, 0);
            try
            {
                string clickResult = await FinanceWebView.EvaluateJavaScriptAsync(jsClickContinue);
                clickResult = NationwideCommon.TrimQuotes(clickResult);
                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, $"Step3 Continue result: {clickResult}");
            }
            catch (Exception ex)
            {
                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, $"Step3 Error executing continue JS: {ex}");
                return false;
            }

            // Start  server to receive OTP etc.
            // But make sure you update the step first!!!
            Step = 35;
            StepInProgress = false;

            // Now wait for whichever happens next
            NotificationPacket packet = await waitTask;
            if (packet == null)
            {
                NationwideCommon.HandleTimeout(FinanceWebView, ourviewmodel, financeviewmodel, tcs1001, loggedIn);
            }
            else
            {
                // Received notification
                if (!packet.Text.Contains(notificationPrefix))
                {
                    return false;
                }
                packet.Text = packet.Text.Replace(notificationPrefix, "").Trim();
                if (packet.Text.Length <= notificationTagLength)
                {
                    return false;
                }
                // Get first 6?
                string otp = packet.Text.Substring(0, notificationTagLength);
                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "OTP Received: " + otp);
                // ? Fill OTP in WebView
                string jsotp = $@"(function() {{var input = document.querySelector('input[name=""OneTimePasscode""]');if (input) {{input.focus();input.value = '{otp}';input.dispatchEvent(new Event('input', {{bubbles: true }}));input.dispatchEvent(new Event('change', {{bubbles: true }}));input.blur();return 'Input filled';}}return 'Input not found';}})();";
                string resultotp = await FinanceWebView.EvaluateJavaScriptAsync(jsotp);
                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "OTP: " + resultotp);
                // ? Click "log in" button
                string clickit = @"(() => {const btn = Array.from(document.querySelectorAll('button')).find(b => b.textContent.trim().toLowerCase() === 'log in');if (!btn) return 'NotFound';btn.click();return 'Clicked';})();";
                string resultlogin = await FinanceWebView.EvaluateJavaScriptAsync(clickit);
                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Login: " + resultlogin);
            }
            return true;
        }

        private async Task WaitForAccountTileAsync(WebView FinanceWebView)
        {
            for (int i = 0; i < 40; i++) // 40 × 250 ms = 10 seconds
            {
                var ready = await FinanceWebView.EvaluateJavaScriptAsync(@"document.querySelector('[data-testid=""account-tile""]') != null");

                if (ready == "true")
                {
                    return;
                }
                await Task.Delay(250);
            }
            throw new TimeoutException("Timed out waiting for accounts");
        }

       internal async Task<bool> RunStep35()
        {
            try
            {
                await WaitForAccountTileAsync(FinanceWebView);

                // Greeting
                string js = @"document.querySelector('p[data-testid=""greetingMessage""]').textContent.trim();";
                string owner = await FinanceWebView.EvaluateJavaScriptAsync(js);
                //string[] greeting = owner;//.Split(',');
                if (!string.IsNullOrEmpty(owner))
                {
                    loggedIn = true;
                }

                string ownerDisplay = "Owner: " + owner;
                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, $"Step35 {ownerDisplay}");

                //Accounts
                string jsonAccounts = @"(() => {const tiles = document.querySelectorAll('[data-testid=""account-tile""]');const data = Array.from(tiles).map(tile => {const name = tile.querySelector('h3')?.innerText?.trim() || null;const accountSpans = tile.querySelectorAll('.product-family__account-number span[aria-hidden=""true""]');let sortCode = accountSpans[0]?.innerText?.trim() || null;let accountNumber = accountSpans[1]?.innerText?.trim() || null;if (name?.includes('Credit Card')) {sortCode = null;accountNumber = tile.querySelector('[data-testid=""accountNumber""]')?.innerText?.trim() || null;}return {name,balance: tile.querySelector('[data-testid=""card-amount""]')?.innerText?.trim() || null,sortCode,accountNumber};});return JSON.stringify(data);})()";

                string jsonResult = await FinanceWebView.EvaluateJavaScriptAsync(jsonAccounts);
                jsonResult = jsonResult.Replace("\\\"", "\"");

                accountsList = JsonSerializer.Deserialize<List<Account>>(jsonResult);
                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, $"Step35 Accounts: " + accountsList.Count());
                if (accountsList.Count == 0)
                {
                    return false;
                }
                
                int statementIndex = 0;
                foreach (Account account in accountsList)
                {
                    if (string.IsNullOrEmpty(account.Href))
                    {
                        string href = "https://onlinebanking.nationwide.co.uk/ib/accounts/statement/" + statementIndex.ToString();
                    }
                    statementIndex++;
                }

                //bool foundit = false;
                //while (!foundit)
                //{
                //    var resulty = await FinanceWebView.EvaluateJavaScriptAsync(@"(() => {const toggle = document.querySelector('button[role=""switch""]');if (!toggle)return ""NOT_FOUND"";const isOn = toggle.getAttribute(""aria-checked"") === ""true"";if (isOn){toggle.click();return ""TURNED_OFF"";}return ""ALREADY_OFF"";})()");
                //    if (resulty == "TURNED_OFF" || resulty == "ALREADY_OFF")
                //    {
                //        foundit = true;
                //        await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Step4: New look is OFF");
                //    }
                //    else
                //    {
                //        await Task.Delay(200);
                //    }
                //}
                Step = 4;
                StepInProgress = false;
                pageLoaded = false;
                // Go and get all the bits and pieces
                //FinanceWebView.Source = "https://onlinebanking.nationwide.co.uk/Customisation/Customisation/MyDetailsAndSettings";

                //FinanceWebView.Source = "https://onlinebanking.nationwide.co.uk/ib/settings";


                //FinanceWebView.Source = "https://onlinebanking.nationwide.co.uk/ib/CustomerIB/MaintainTelephoneAndAddress";
                //FinanceWebView.Source = "https://onlinebanking.nationwide.co.uk/CustomerIB/MaintainTelephoneAndAddress";
                //lastUrl = "https://onlinebanking.nationwide.co.uk/CustomerIB/MaintainTelephoneAndAddress";
                fuckers = true;
                //FinanceWebView.Source = "https://onlinebanking.nationwide.co.uk/CustomerIB/ViewAndMaintainCustomerDetails";

                FinanceWebView.Source = "https://onlinebanking.nationwide.co.uk/ib/settings/personal-details";
                //GET https://onlinebanking.nationwide.co.uk/CustomerIB/MaintainTelephoneAndAddress HTTP/1.1

            }
            catch (Exception ex)
            {
                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, $"Step35 Error: {ex.Message}");
                financeviewmodel.errorMessage = ex.Message;
                return false;
            }
            return true;
        }

        private async Task WaitForStep4Async(WebView FinanceWebView)
        {
            for (int i = 0; i < 40; i++) // 40 × 250 ms = 10 seconds
            {
                var ready = await FinanceWebView.EvaluateJavaScriptAsync(@"document.querySelector('.details-value address') != null");

                if (ready == "true")
                {
                    return;
                }
                await Task.Delay(250);
            }
            throw new TimeoutException("Timed out waiting for details");
        }

        internal async Task<bool> RunStep4()
        {
            try
            {
                await WaitForStep4Async(FinanceWebView);
                
                Console.WriteLine(pageLoaded);
                
                string jsx = @"(() => document.querySelectorAll('.details-value').length)()";
                var result = await FinanceWebView.EvaluateJavaScriptAsync(jsx);
                Console.WriteLine(result);
                string jsonAddress = @"(() => {const addr = document.querySelector('.details-value address');if (!addr) return JSON.stringify(null);const text = addr.innerText.split('\n').map(x => x.trim()).filter(x => x.length > 0).join(', ');return JSON.stringify(text);})()";
                string jsonResult = await FinanceWebView.EvaluateJavaScriptAsync(jsonAddress);
                jsonResult = jsonResult.Replace("\\\"", "\"");
                string address = JsonSerializer.Deserialize<string>(jsonResult);
                Console.WriteLine(address);
                string jsonPhones = @"(() => {const result = {home: null,work: null,mobile: null};const dts = document.querySelectorAll('.details-value dl dt');for (const dt of dts) {const label = dt.innerText.replace(':', '').trim().toLowerCase();const dd = dt.nextElementSibling;if (!dd) continue;const value = dd.innerText.trim();switch (label) {case 'home':result.home = value;break;case 'work':result.work = value;break;case 'mobile':result.mobile = value;break;}}return JSON.stringify(result);})()";
                jsonResult = await FinanceWebView.EvaluateJavaScriptAsync(jsonPhones);
                jsonResult = jsonResult.Replace("\\\"", "\"");
                //string phones = JsonSerializer.Deserialize<string>(jsonResult);
                Console.WriteLine(jsonResult);
                string jsonEmail = @"(() => {const nodes = document.querySelectorAll('.details-value');for (const node of nodes) {const text = (node.innerText || '').trim();if (text.includes('@')) {return JSON.stringify(text);}}return JSON.stringify(null);})()";
                jsonResult = await FinanceWebView.EvaluateJavaScriptAsync(jsonEmail);
                jsonResult = jsonResult.Replace("\\\"", "\"");
                string emailaddress = JsonSerializer.Deserialize<string>(jsonResult);
                Console.WriteLine(emailaddress);
                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, $"Step4 Email: {emailaddress}");
            }
            catch (Exception ex)
            {
                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, $"Step4 Error: {ex.Message}");
                financeviewmodel.errorMessage = ex.Message;
                return false;
            }
            return true;
        }

        
        internal async Task<bool> RunStep45()
        {
            string jsList = GetJavaScript(43, javascriptTemplates, "");
            try
            {
                string jsonResult = await FinanceWebView.EvaluateJavaScriptAsync(jsList);

                // IMPORTANT: WebView returns escaped JSON string
                jsonResult = NationwideCommon.TrimQuotes(jsonResult);
                jsonResult = Regex.Unescape(jsonResult);

                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, $"Step4 Account list JSON: {jsonResult}");

                List<AccountLink> accountLinks =
                    System.Text.Json.JsonSerializer.Deserialize<List<AccountLink>>(jsonResult);

                AccountLinks = accountLinks ?? new List<AccountLink>();
                if (AccountLinks == null || AccountLinks.Count == 0)
                {
                    await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "No accounts found...");
                }
                else
                {
                    foreach (var acc in AccountLinks)
                    {
                        await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, $"Step4 Link: {acc.Href}");// – {acc.innerHTML}");
                    }
                }

                string hrefContains2 = "MaintainTelephoneAndAddress";
                string clickPhoneAddress = GetJavaScript(44, javascriptTemplates, hrefContains2);

                string phoneAddressResult = await FinanceWebView.EvaluateJavaScriptAsync(clickPhoneAddress);
                phoneAddressResult = NationwideCommon.TrimQuotes(phoneAddressResult);
                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, $"Step4 Phone Address click result: {phoneAddressResult}");

                Step = 5;
                StepInProgress = false;
                return true;
            }
            catch (Exception ex)
            {
                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, $"Step4 Error: {ex}");
                financeviewmodel.errorMessage = ex.Message;
                return false;
            }
        }
        internal async Task<bool> RunStep5()
        {
            await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "RunStep5: started");
            UDPRN = "";

            try
            {
                // Step 4: get the address
                //string getAddressScript = GetJavaScript(financeviewmodel, 45, javascriptTemplates, "");

                string getAddressScript = @"(function() {let addresses = document.getElementsByTagName('address');if (addresses.length === 0) return '';let addrText = addresses[0].innerText || '';addrText = addrText.replace(/\r?\n/g, ' ').trim();addrText = addrText.replace(/\s{2,}/g, ' ');return addrText;})();";
                string addressJson = await FinanceWebView.EvaluateJavaScriptAsync(getAddressScript);
                addressJson = NationwideCommon.TrimQuotes(addressJson);
                string bankAddress = "Address: " + addressJson;
                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, $"Step5 bankAddress = {bankAddress}");
                if (string.IsNullOrWhiteSpace(addressJson))
                {
                    financeviewmodel.errorMessage = "Bank address empty - cannot continue";
                    return false;
                }
                else
                {
                    await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, bankAddress);
                }

                // Lookup existing address in your local cache
                var addresses_found = SmartSpikeV2017.UsersLookupTextAddress(ourviewmodel, addressJson.Trim());
                if (addresses_found.Count > 0)
                {
                    var first = addresses_found.First();
                    if (string.IsNullOrEmpty(first.UDPRN))
                    {
                        financeviewmodel.errorMessage = "Address record has no UDPRN - cannot continue";
                        return false;
                    }
                    UDPRN = first.UDPRN;
                }
                else
                {
#if PRODUCTION
                // Fallback: lookup via external service
                var ideal_address = await SmartRoutinesV2018.UsersUDPRNLookup(ourviewmodel, addressJson.Trim());
                if (ideal_address == null || string.IsNullOrEmpty(ideal_address.UDPRN))
                {
                    financeviewmodel.errorMessage = "Cannot determine UDPRN - cannot continue";
                    return false;
                }
                UDPRN = ideal_address.UDPRN;

                // Optionally insert new address record logic...
#endif
                }

                // Extract email via JS
                string getEmailScript = GetJavaScript(46, javascriptTemplates, "");
                string emailJson = await FinanceWebView.EvaluateJavaScriptAsync(getEmailScript);
                emailJson = NationwideCommon.TrimQuotes(emailJson);
                string email = "Email: " + emailJson;
                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, $"Step5 {email}");

                // Extract phone via JS
                string getPhoneScript = GetJavaScript(47, javascriptTemplates, "");
                string phoneJson = await FinanceWebView.EvaluateJavaScriptAsync(getPhoneScript);
                phoneJson = NationwideCommon.TrimQuotes(phoneJson);
                string phone = "Phone: " + phoneJson;
                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, $"Step5 {phone}");
                // If no account links, bail
                if (AccountLinks == null || AccountLinks.Count == 0)
                {
                    await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Step5 No account links, logging out or fail");
                    return false;
                }

                bool processed = await NationwideCommon.ProcessNextAccount(FinanceWebView,
                                                                            ourviewmodel,
                                                                            financeviewmodel,
                                                                            institutionCode,
                                                                            brandCode,
                                                                            CurrentAccountIndex,
                                                                            AccountLinks,
                                                                            UDPRN,
                                                                            //logClass,
                                                                            st => Step = st,
                                                                            sip => StepInProgress = sip
                );
                if (!processed)
                {
                    await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Step5 ProcessNextAccount failed");
                    return false;
                }
            }
            catch (Exception ex)
            {
                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, $"Step5 Exception: {ex.Message}");
                financeviewmodel.errorMessage = ex.Message;
                return false;
            }
            if (string.IsNullOrEmpty(UDPRN))
            {
                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Step5 UDPRN is empty on exit");
                return false;
            }
            Step = 6;
            StepInProgress = false;
            await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "RunStep5: completed, moving to Step 6");
            return true;
        }

        internal async Task<bool> RunStep6(string SortCode,
                                            string AccountNo,
                                            char categoryCode,
                                            string symbol)
        {
            await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, $"RunStep6: Starting (UDPRN = {UDPRN})");

            string oneYearAgo = DateTime.Now.AddYears(-1).ToString("dd/MM/yyyy");
            string clickButtonScript = $@"(() => {{try {{const revealLink = document.getElementById('enter-date-reveal-link');if (!revealLink) {{return 'error: reveal_link_not_found';}}revealLink.click();const input = document.querySelector('#statement-from-date');const button = document.querySelector('#date-filter-update');if (!input || !button) {{return 'error: missing_input_or_button';}}const nativeSetter = Object.getOwnPropertyDescriptor(window.HTMLInputElement.prototype, 'value').set;nativeSetter.call(input, '{oneYearAgo}');input.dispatchEvent(new Event('input', {{ bubbles: true }}));input.dispatchEvent(new Event('change', {{ bubbles: true }}));button.click();return 'clicked';}} catch (e) {{return 'script_error: ' + e.message;}}}})();";

            string clickButton = await FinanceWebView.EvaluateJavaScriptAsync(clickButtonScript);
            clickButton = NationwideCommon.TrimQuotes(clickButton);
            if (clickButton != "clicked")
            {
                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, $"Step6 clickButton result = {clickButton}, assuming no transactions");
            }
            else
            {
                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Step6 clicked, waiting a bit for page to update");
                await Task.Delay(1500);

                string readTableScript = @"(() => {try {const tbody = document.querySelector('tbody');if (!tbody) return 'no_tbody';const rows = Array.from(tbody.querySelectorAll('tr')).map(row => {return Array.from(row.querySelectorAll('td')).map(cell => cell.innerText.trim());});return JSON.stringify(rows);} catch (e) {return 'script_error: ' + e.message;}})();";

                string rawJson = await FinanceWebView.EvaluateJavaScriptAsync(readTableScript);
                rawJson = NationwideCommon.TrimQuotes(rawJson);
                string json = Regex.Unescape(rawJson);

                // Convert from Newtonsoft.Json to System.Text.Json
                var tableRows = System.Text.Json.JsonSerializer.Deserialize<List<List<string>>>(json);
                string[] transactions = tableRows?
                    .Select(inner => string.Join(SmartParametersV2016.tab, inner))
                    .ToArray() ?? Array.Empty<string>();

                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, $"Step6 Transactions count: {transactions.Length}");

                // If you have Android-specific code under ANDROIDX, wrap or remove it
                DateTime accountCreated = SmartParametersV2016.defaultDate;
                var accounts_found = financeviewmodel.PLO.finance_accountsList
                    .Where(a => a.INSTITUTION_CODE == institutionCode &&
                                a.BRAND_CODE == brandCode &&
                                a.SORTCODE == SortCode &&
                                a.ACCOUNT_NO == AccountNo &&
                                a.UDPRN == UDPRN)
                    .OrderByDescending(a => a.ACCOUNT_CREATED)
                    .ToList();

                if (accounts_found.Count > 0)
                    accountCreated = accounts_found.First().ACCOUNT_CREATED;

                bool success = await NationwideCommon.NationwideTransactions(ourviewmodel,
                                                                            financeviewmodel,
                                                                            categoryCode,
                                                                            transactions,
                                                                            institutionCode,
                                                                            brandCode,
                                                                            SortCode,
                                                                            AccountNo,
                                                                            UDPRN,
                                                                            symbol,
                                                                            transactionGroups,
                                                                            transactionTypes,
                                                                            accountCreated
                );
                if (!success)
                {
                    financeviewmodel.errorMessage = "Transactions failed: " + financeviewmodel.errorMessage;
                    await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, financeviewmodel.errorMessage);
                    return false;
                }
            }

            // Advance account index
            CurrentAccountIndex++;

            bool nextOk = await NationwideCommon.ProcessNextAccount(FinanceWebView, 
                                                                    ourviewmodel,
                                                                    financeviewmodel,
                                                                    institutionCode,
                                                                    brandCode,
                                                                    CurrentAccountIndex,
                                                                    AccountLinks,
                                                                    UDPRN,
                                                                    //logClass,
                                                                    st => Step = st,
                                                                    stip => StepInProgress = stip
            );
            if (!nextOk)
            {
                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Step6 ProcessNextAccount failed");
                return false;
            }
            await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "RunStep6: completed");
            return true;
        }        
#endif
    }

}