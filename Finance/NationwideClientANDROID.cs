using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using Android.Content;
using Android.Util;
using Android.Views;
using Android.Webkit;
using AndroidX.AppCompat.App;
using com.keasdon.messagereceiver;
using Newtonsoft.Json;

namespace SmartCubeMobile
{
    internal class CustomWebViewClient : WebViewClient
    {
        internal static string logClass = "Nationwide";

        internal MainViewModel ourviewmodel;
        internal FinanceViewModel financeviewmodel;
        internal bool StepInProgress;
        internal int Step;
        internal string customerNumber,
                        birthDay,
                        birthMonth,
                        birthYear,
                        passcode;
        internal short institutionCode;
        internal short brandCode;
        
        internal static List<SmartFinance.Transaction_Groups> transaction_groupsFound;
        internal static List<SmartFinance.Transaction_Types> transaction_typesFound;

        internal static string loginHref = "";
        internal bool loggedIn = false;
        internal static List<AccountLink> AccountLinks { get; set; } = new();
        internal static int CurrentAccountIndex { get; set; } = 0;
        internal static TaskCompletionSource<bool> tcs;

        internal static string SortCode = "";
        internal static string AccountNo = "";
        internal static char categoryCode = SmartParametersV2016.defaultChar;
        internal static string symbol = "";

        // The vitally important UDPRN!!
        // You CANNOT add ACCOUNTS, TRANSACTIONS or
        // TRANSACTIONSCATEGORIES without a valid
        //     ====> UDPRN <==== 
        // It is **all important**
        internal static string UDPRN = "";
        internal string OWETP = "";
        
        public CustomWebViewClient(
#if ANDROIDX
                                    AppCompatActivity meterActivity,
#endif
                                    MainViewModel ourviewmodel, 
                                    FinanceViewModel financeviewmodel,
                                    TaskCompletionSource<bool> TCS,
                                    short InstitutionCode,
                                    short BrandCode,
                                    string CustomerNumber,
                                    string BirthDay,
                                    string BirthMonth,
                                    string BirthYear,
                                    string Passcode,
                                    List<SmartFinance.Transaction_Groups> TransactionGroupsFound,
                                    List<SmartFinance.Transaction_Types> TransactionTypesFound)
        {
            this.ourviewmodel = ourviewmodel;
            this.financeviewmodel = financeviewmodel;
            tcs = TCS;
            institutionCode = InstitutionCode;
            brandCode = BrandCode;
            customerNumber = CustomerNumber;
            birthDay = BirthDay;
            birthMonth = BirthMonth;
            birthYear = BirthYear;
            passcode = Passcode;
            transaction_groupsFound = TransactionGroupsFound;
            transaction_typesFound = TransactionTypesFound;
            // ? Subscribe to OTP received event
            SmartReceiver.OnOtpReceived += (ourviewmodel, financeviewmodel, webView, otp) =>
            {
                // Fire-and-forget the async method since it's an async void
                //NationwideCommon.HandleOtpReceived(webView, meterActivity, ourviewmodel, financeviewmodel, otp);
            };

        }

        // Took out 'override' from here
        [RequiresUnreferencedCode("Calls SmartCubeMobile.CustomWebViewClient.RunStep3(AppCompatActivity, MainViewModel, FinanceViewModel, WebView, String)")]
        public async void OnPageFinished(
#if ANDROIDX
                                                    AppCompatActivity meterActivity,
#endif
                                                    WebView webView, string url)
        {
            base.OnPageFinished(webView, url);

            string js = @"
                    (function() {
                        var meta = document.createElement('meta');
                        meta.name = 'viewport';
                        meta.content = 'width=device-width, initial-scale=0.3';
                        document.getElementsByTagName('head')[0].appendChild(meta);
                    })();
                ";

            webView.EvaluateJavascript(js, null);

            Log.Debug(logClass, $"Step: {Step}, InProgress: {StepInProgress}");

            // Continue existing logic...
            if (StepInProgress)
            {
                return;
            }
            StepInProgress = true;

            try
            {
                switch (Step)
                {
                    case 0:
                        RunStep0(webView);
                        break;
                    case 1:
                        RunStep1(webView);
                        break;
                    case 2:
                        RunStep2(webView,
                            customerNumber,
                                        birthDay,
                                        birthMonth,
                                        birthYear);
                        break;
                    case 3:
                        _ = RunStep3(meterActivity,
                                    ourviewmodel,
                                    financeviewmodel,
                                    webView,
                                    passcode);
                        break;
                    case 4:
                        _ = RunStep4(webView,
                                    meterActivity,
                                    ourviewmodel,
                                    financeviewmodel);
                        break;
                    case 5:
                        bool result5 = await RunStep5(webView,
                                                        meterActivity,
                                                        ourviewmodel,
                                                        financeviewmodel,
                                                        institutionCode,
                                                        brandCode);
                        if (!result5)
                        {
                            await SmartRoutinesV2018.TextBlockUpdate(
#if ANDROIDX
                                                                meterActivity,
#endif
                                                                ourviewmodel, "Finished processing accounts.");
                            Log.Debug(logClass, "Logout attempt: " + AccountLinks.Count());

                            await NationwideCommon.LogoutAsync(webView,
                                                meterActivity,
                                                ourviewmodel,
                                                financeviewmodel,
                                                tcs,
                                                loggedIn);
                            return;
                        }
                        break;
                    case 6:
                        bool result6 = await RunStep6(webView, 
                                                        meterActivity,              
                                                        ourviewmodel,
                                                                financeviewmodel,
                                                                institutionCode,
                                                                brandCode,
                                                                SortCode,
                                                                AccountNo,
                                                                categoryCode,
                                                                symbol,
                                                                transaction_groupsFound,
                                                                transaction_typesFound);
                        if (!result6)
                        {
                            await SmartRoutinesV2018.TextBlockUpdate(
#if ANDROIDX
                                                                meterActivity,
#endif
                                                                ourviewmodel, "Finished processing accounts.");
                            Log.Debug(logClass, "Logout attempt: " + CurrentAccountIndex.ToString());

                            await NationwideCommon.LogoutAsync(webView,
                                                meterActivity,
                                                ourviewmodel,
                                                financeviewmodel,
                                                tcs,
                                                loggedIn);
                            return;
                        }
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                Log.Error("WebViewClient", $"Step failed: {ex.Message}");
            }
        }

        internal void RunStep0(WebView webView)
        {
            webView.EvaluateJavascript(@"
        (function() {
        try {
            let scripts = document.querySelectorAll('script');
            for (let script of scripts) {
                if (script.textContent.includes('logInLink')) {
                    let match = script.textContent.match(/""logInLink""\s*:\s*\{[^}]+\}/);                                    
                    if (match && match[0])
                    {
                        let jsonText = '{' + match[0] + '}';
                        let json = JSON.parse(jsonText);
                        return json.logInLink.url;
                    }
                }
            }
        } catch (e) {
            return '';
        }
        return '';
    })();", new ValueCallback(result =>
            {
                loginHref = NationwideCommon.TrimQuotes(result);
                if (!string.IsNullOrEmpty(loginHref))
                {
                    if (loginHref != "null")    // Yes! The WORD null
                    {
                        Step = 1;
                    }
                    StepInProgress = false;
                }
            }));
            return;
        }
        internal void RunStep1(WebView webView)
        {
            if (!string.IsNullOrEmpty(loginHref))
            {
                if (loginHref != "null")    // Yes! The WORD null
                {
                    webView.LoadUrl(loginHref);
                }
                Step = 2;
                StepInProgress = false;
            }
            return;
        }

        internal void RunStep2(WebView webView,
                                string customerNumber,
                                string birthDay,
                                string birthMonth,
                                string birthYear)
        {
            // Fill login form and click Continue

            string jsCustomer = $@"
    (function() {{
        var input = document.querySelector('input[name=""CustomerNumber""]');
        if (input) {{
            input.focus();
            input.value = '{customerNumber}';
            input.dispatchEvent(new Event('input', {{bubbles: true }}));
            input.dispatchEvent(new Event('change', {{bubbles: true }}));
            input.blur();
            return 'Input filled successfully';
        }}
        return 'Input not found';
    }})();";
            webView.EvaluateJavascript(jsCustomer, new ValueCallback(result =>
            {
                Log.Debug(logClass, "Customer Result: " + result);
            }));

            string jsDay = $@"
    (function() {{
        var input = document.querySelector('input[name=""DateOfBirthDay""]');
        if (input) {{
            input.focus();
            input.value = '{birthDay}';
            input.dispatchEvent(new Event('input', {{ bubbles: true }}));
            input.dispatchEvent(new Event('change', {{ bubbles: true }}));
            input.blur();
            return 'Input filled successfully';
        }}
        return 'Input not found';
    }})();";
            webView.EvaluateJavascript(jsDay, new ValueCallback(result =>
            {
                Log.Debug(logClass, "Day Result: " + result);
            }));

            string jsYear = $@"
        (function() {{var input = document.querySelector('input[name=""DateOfBirthYear""]');
            if (input) {{
                input.focus();
                input.value = '{birthYear}';
                input.dispatchEvent(new Event('input', {{ bubbles: true }}));
                input.dispatchEvent(new Event('change', {{ bubbles: true }}));
                input.blur();
                return 'Input filled successfully';
            }}
            return 'Input not found';
        }})();";
            webView.EvaluateJavascript(jsYear, new ValueCallback(result =>
            {
                Log.Debug(logClass, "Year Result: " + result);
            }));

            string jsMonth = $@"
                (function() {{
                    var select = document.querySelector('select[name=""DateOfBirthMonth""]');
                    if (select) {{
                        select.value = '{birthMonth}';
                        var event = new Event('change', {{ bubbles: true }});
                        select.dispatchEvent(event);
                        return 'Month selected';
                    }}
                    return 'Month dropdown not found';
                }})();";
            webView.EvaluateJavascript(jsMonth, new ValueCallback(result =>
            {
                Log.Debug(logClass, "Month Selection Result: " + result);
            }));

            // Click Continue
            string script = $@"
            var btns = Array.from(document.querySelectorAll('button'));
            var btn = btns.find(b => b.textContent.trim().toLowerCase() === 'continue');
            if (btn) btn.click();";

            webView.EvaluateJavascript(script, null);
            Step = 3;
            StepInProgress = false;
            return;
        }

        [RequiresUnreferencedCode("Calls SmartCubeMobile.NationwideCommon.NationwideFindPasscodes(WebView, AppCompatActivity, MainViewModel, FinanceViewModel, String, Action<String>, Action<String>)")]
        internal async Task<bool> RunStep3(
#if ANDROIDX
                                            AppCompatActivity meterActivity,
#endif
                                            MainViewModel ourviewmodel,
                                            FinanceViewModel financeviewmodel,
                                            WebView webView,
                                            string passcode)
        {
            string jsPasscode = @"
                        (() => {
                            const radio = document.querySelector(""input[type='radio'][value='PassNumberAndSMSOTP']"");
                            if (radio && radio.offsetParent !== null) {  // Checks for visibility
                                radio.scrollIntoView({ behavior: 'smooth', block: 'center' });
                                radio.click();
                                return 'Clicked';
                            } else {
                                return 'NotFound';
                            }
                        })()";
            webView.EvaluateJavascript(jsPasscode, new ValueCallback(result =>
            {
                Log.Debug(logClass, "Passcode Selection Result: " + result);
            }));

            string logAction = "";
            string errorMessage = "";
            if (!await NationwideCommon.NationwideFindPasscodes(webView,
                                                meterActivity,
                                                ourviewmodel,
                                                financeviewmodel,
                                                passcode,
                                                la => logAction = la,
                                                em => errorMessage = em))
            {
                // Some error
                Log.Debug(logClass, "Passcode Find error: " + errorMessage);
                return false;
            }
            // Click Continue
            string script = $@"
                var btns = Array.from(document.querySelectorAll('button'));
                var btn = btns.find(b => b.textContent.trim().toLowerCase() === 'continue');
                if (btn) btn.click();";
            webView.EvaluateJavascript(script, null);

            Log.Debug(logClass, "Continue button clicked");


            //SmartReceiver.OnTimeout += HandleTimeout;
            SmartReceiver.OnTimeout += (ourviewmodel, financeviewmodel, webView, tcs, loggedIn) =>
            {
                // Fire-and-forget the async method since it's an async void
                NationwideCommon.HandleTimeout(webView, meterActivity, ourviewmodel, financeviewmodel, tcs, loggedIn);
            };            
            // Start HTTP server
            // But make sure you update the step first!!!
            Step = 4;
            StepInProgress = false;
            SmartReceiver.StartHttpServer(webView,
                                            ourviewmodel,
                                            financeviewmodel,
                                            tcs,
                                            loggedIn);
            Log.Debug(logClass, "Waiting for OTP: ");            
            return true;
        }

        internal async Task<bool> RunStep4(WebView webView,
#if ANDROIDX
                                            AppCompatActivity meterActivity,
#endif
                                            MainViewModel ourviewmodel,
                                            FinanceViewModel financeviewmodel)
        {
            string welcomeScript = @"
            (function() {
                const welcomeElem = document.getElementById('welcome-message');
                return welcomeElem && welcomeElem.innerText ? welcomeElem.innerText : '';
            })();";

            try
            {
                // Await welcome text from JS
                string welcomeTextRaw = await NationwideCommon.EvaluateJavaScriptAsync(webView, welcomeScript);
                string welcomeText = System.Text.Json.JsonDocument.Parse($"\"{welcomeTextRaw}\"").RootElement.GetString() ?? "";
                // I'm assuming we would never see this UNLESS we were logged in!!
                if (string.IsNullOrEmpty(welcomeText))
                {
                    loggedIn = true;
                }
                string owner = welcomeText.Replace("Welcome back,", "").Trim();

                owner = "Owner: " + owner;
                Log.Error("Nationwide", owner);
                //financeviewmodel.Welcome.Text = "Owner: " + owner;
                await SmartRoutinesV2018.TextBlockUpdate(
#if ANDROIDX
                                                        meterActivity,
#endif
                                                        ourviewmodel, owner);

                string hrefAccContains = "AccountList/AccountList";
                string accountListHref = hrefAccContains.Replace("'", "\\'");

                // Navigate to the href and wait for navigation complete event
                string scriptAccountList = $@"
                        (() => {{
                            const link = Array.from(document.querySelectorAll('a[href]')).find(a => a.href.includes('{accountListHref}'));
                            if (link) {{
                                link.click();
                                return true;
                            }} else {{
                                return false;
                            }}
                        }})()";
                // Navigate to the href and wait for navigation complete event

                string result = await NationwideCommon.EvaluateJavaScriptAsync(webView, scriptAccountList);
                Log.Error("Nationwide", "Accounts href: " + result);

                //we should be going to 200 now!
                string scriptList = @"
                        (function() {
                            let results = [];
                            let activeElement = document.querySelector('.active');
                            if (!activeElement) return JSON.stringify(results);
                            let anchors = activeElement.getElementsByTagName('a');
                            for(let i=0; i<anchors.length; i++) {
                                let href = anchors[i].href || '';
                                if(href.includes('/AccountList/Account/RedirectToDefaultPage')) {
                                    results.push({
                                        href: href,
                                        innerHTML: anchors[i].innerHTML
                                    });
                                }
                            }
                            return JSON.stringify(results);
                        })();";

                string jsonResult = await NationwideCommon.EvaluateJavaScriptAsync(webView, scriptList);
                // Step 1: Trim leading/trailing quotes from JS result because we have a string not an array
                if (jsonResult.StartsWith("\"") && jsonResult.EndsWith("\""))
                {
                    jsonResult = jsonResult.Substring(1, jsonResult.Length - 2);
                }
                // Step 2: Unescape the JSON string
                //string unescapedJson = System.Text.RegularExpressions.Regex.Unescape(jsonResult);

                // Step 3: Deserialize JSON string to a list of link info objects
                //AccountLinks = System.Text.Json.JsonSerializer.Deserialize<List<AccountLink>>(unescapedJson);

                // Most important! Otherwise it falls over!!
                string unescapedJson = Regex.Unescape(jsonResult);

                // Step 3: Deserialize using Newtonsoft.Json
                AccountLinks = JsonConvert.DeserializeObject<List<AccountLink>>(unescapedJson);
                if (AccountLinks == null || AccountLinks.Count == 0)
                {
                    // Not an error message as such, this might well be the case
                    await SmartRoutinesV2018.TextBlockUpdate(
#if ANDROIDX
                                                             meterActivity,
#endif
                                                            ourviewmodel, "No accounts found...");
                    Log.Debug(logClass, "AccountLinks: empty");
                }
                foreach (AccountLink acc in AccountLinks)
                {
                    Log.Debug(logClass, "AccountLinks2: " + acc.href);
                    Log.Debug(logClass, "AccountLinks2: " + acc.innerHTML);
                }

                // Click link to "MaintainTelephoneAndAddress"
                string hrefContains = "MaintainTelephoneAndAddress";
                string escapedHref = hrefContains.Replace("'", "\\'");

                string clickScript = $@"
            (() => {{
                const link = Array.from(document.querySelectorAll('a[href]')).find(a => a.href.includes('{escapedHref}'));
                if (link) {{
                    link.click();
                    return true;
                }} else {{
                    return false;
                }}
            }})();";
                string phoneAddress = await NationwideCommon.EvaluateJavaScriptAsync(webView, clickScript); // You don't even need the result here unless you want to log it
                Log.Error("Nationwide", "PhoneAddress href: " + phoneAddress);
                Step = 5;
                StepInProgress = false;
            }
            catch (Exception ex)
            {
                Log.Error("Nationwide", $"RunStep4 error: {ex.Message}");
                financeviewmodel.errorMessage = ex.Message;
                return false;
            }
            return true;
        }

        internal async Task<bool> RunStep5(WebView FinanceWebView,
#if ANDROIDX
                                           AppCompatActivity meterActivity,
#endif
                                            MainViewModel ourviewmodel,
                                            FinanceViewModel financeviewmodel,
                                            short institutionCode,
                                            short brandCode)
        {
            Log.Error("Nationwide", $"RunStep5 started:");
            UDPRN = "";
            try
            {
                // Step 1: Find the address text inside <address> tags
                string getAddressScript = @"
                                (function() {
                                    let addresses = document.getElementsByTagName('address');
                                    if (addresses.length === 0) return '';
                                    let addrText = addresses[0].innerText || '';
                                    addrText = addrText.replace(/\r?\n/g, ' ').trim();
                                    addrText = addrText.replace(/\s{2,}/g, ' ');
                                    return addrText;
                                })();
                            ";
                string addressJson = await NationwideCommon.EvaluateJavaScriptAsync(FinanceWebView, getAddressScript);
                string bankAddress = "Address: " + addressJson.Trim();
                Log.Debug(logClass, bankAddress);
                if (string.IsNullOrWhiteSpace(addressJson.Trim()))
                {
                    financeviewmodel.errorMessage = "Bank address empty - cannot continue";
                    return false;
                }
                else
                {
                    await SmartRoutinesV2018.TextBlockUpdate(
#if ANDROIDX
                                                             meterActivity,
#endif
                                                            ourviewmodel, bankAddress);
                }

                // Lookup existing address in your database or cache
                var addresses_found = SmartSpikeV2017.UsersLookupTextAddress(ourviewmodel, addressJson.Trim());
                if (addresses_found.Count > 0)
                {
                    if (string.IsNullOrEmpty(addresses_found.First().UDPRN))
                    {
                        financeviewmodel.errorMessage = "Address record has no UDPRN - cannot continue";
                        return false;
                    }
                    UDPRN = addresses_found.First().UDPRN;
                }
                else
                {
#if PRODUCTION
                    // New address - lookup via external service
                    var ideal_address = await SmartRoutinesV2018.UsersUDPRNLookup(ourviewmodel, addressJson.Trim());
                    if (string.IsNullOrEmpty(ideal_address.UDPRN))
                    {
                        financeviewmodel.errorMessage = "Cannot determine UDPRN - cannot continue";
                        return false;
                    }
                    UDPRN = ideal_address.UDPRN;

                    // Add new address record logic here (similar to your original code) ...
                    // ...
#endif
                }

                // Extract email and phone from page via JS
                string getEmailScript = @"
                                    (function() {
                                        let el = document.getElementById('CurrentEmailAddress');
                                        return el ? el.value : '';
                                    })();
                                ";
                string emailJson = await NationwideCommon.EvaluateJavaScriptAsync(FinanceWebView, getEmailScript);
                string email = "Email: " + emailJson.Trim();
                Log.Debug(logClass, email);
                await SmartRoutinesV2018.TextBlockUpdate(
#if ANDROIDX
                                                        meterActivity,
#endif
                                                        ourviewmodel, email);

                //if (loginInfo.CONTACT_EMAIL != emailValue)
                //{
                //    loginInfo.CONTACT_EMAIL = emailValue;
                //    loginInfo.Updated = true;
                //}

                string getPhoneScript = @"
                                    (function() {
                                        let el = document.getElementById('MobilePhoneNumber');
                                        return el ? el.value : '';
                                    })();
                                ";
                string phoneJson = await NationwideCommon.EvaluateJavaScriptAsync(FinanceWebView, getPhoneScript);
                string phone = "Phone: " + phoneJson.Trim();
                Log.Debug(logClass, phone);
                await SmartRoutinesV2018.TextBlockUpdate(
#if ANDROIDX
                                                        meterActivity,
#endif
                                                        ourviewmodel, phone);

                //if (loginInfo.CONTACT_PHONENO != phoneValue)
                //{
                //    loginInfo.CONTACT_PHONENO = phoneValue;
                //    loginInfo.Updated = true;
                //}

                await SmartRoutinesV2018.TextBlockUpdate(
#if ANDROIDX
                                                        meterActivity,
#endif
                                                        ourviewmodel, "In ProcessNext: " + CurrentAccountIndex.ToString());

                // First (and only time here ) CurrentAccountIndex = 0
                if (AccountLinks.Count == 0)
                {
                    Log.Debug(logClass, "Step5 : No links ... logging out");
                    return false;
                }
                if (!await NationwideCommon.ProcessNextAccount(
                                                FinanceWebView,
                                                logClass,
                                                ourviewmodel,
                                                financeviewmodel,
                                                institutionCode,

                                                brandCode,
                                                CurrentAccountIndex,
                                                AccountLinks,
                                                UDPRN,
                                                st => Step = st,
                                                sip => StepInProgress = sip))
                {
                    Log.Debug(logClass, "Step5 : Process account failed");
                    return false;
                }
            }
            catch (Exception ex)
            {
                Log.Error("Nationwide", $"RunStep5 error: {ex.Message}");
                financeviewmodel.errorMessage = ex.Message;
                return false;
            }
            if (string.IsNullOrEmpty(UDPRN))
            {
                Log.Error("Nationwide", "Returning from Step5 with UDPRN empty ");
                return false;
            }
            Step = 6;
            StepInProgress = false;
            Log.Error("Nationwide", "Returning from Step5 ");
            return true;
        }

        internal async Task<bool> RunStep6(WebView FinanceWebView,
#if ANDROIDX
                                            AppCompatActivity meterActivity,
#endif
                                            MainViewModel ourviewmodel,
                                            FinanceViewModel financeviewmodel,
                                            short institutionCode,
                                            short brandCode,
                                            string SortCode,
                                            string AccountNo,
                                            char categoryCode,
                                            string symbol,
                                            List<SmartFinance.Transaction_Groups> transaction_groupsFound,
                                            List<SmartFinance.Transaction_Types> transaction_typesFound)
        {
            // Process each Account
            Log.Warn("Nationwide", "Entering Step6: UDPRN " + UDPRN);
            string oneYearAgo = DateTime.Now.AddYears(-1).ToString("dd/MM/yyyy");
            string clickButtonScript = $@"
            (() => {{
                try {{
                    const revealLink = document.getElementById('enter-date-reveal-link');
                    if (!revealLink) {{
                        return 'error: reveal_link_not_found';
                    }}

                    revealLink.click();

                    const input = document.querySelector('#statement-from-date');
                    const button = document.querySelector('#date-filter-update');

                    if (!input || !button) {{
                        return 'error: missing_input_or_button';
                    }}

                    const nativeSetter = Object.getOwnPropertyDescriptor(window.HTMLInputElement.prototype, 'value').set;
                    nativeSetter.call(input, '{oneYearAgo}');
                    input.dispatchEvent(new Event('input', {{ bubbles: true }}));
                    input.dispatchEvent(new Event('change', {{ bubbles: true }}));

                    button.click();

                    return 'clicked';
                }} catch (e) {{
                    return 'script_error: ' + e.message;
                }}
            }})();";

            string clickButton = await NationwideCommon.EvaluateJavaScriptAsync(FinanceWebView, clickButtonScript);
            if (clickButton != "clicked")
            {
                // There's no drop down
                Log.Warn("Nationwide", "Transactions: 0");
            }
            else
            {
                Log.Debug(logClass, "Click button: " + clickButton);

                await Task.Delay(1500);

                Log.Warn("Nationwide", "Entering into reading table...");

                string readTableScript = @"
                (() => {
                    try {
                        const tbody = document.querySelector('tbody');
                        if (!tbody) return 'no_tbody';

                        const rows = Array.from(tbody.querySelectorAll('tr')).map(row => {
                            return Array.from(row.querySelectorAll('td')).map(cell => cell.innerText.trim());
                        });

                        return JSON.stringify(rows);
                    } catch (e) {
                        return 'script_error: ' + e.message;
                    }
                })();
                ";

                string rawJson = await NationwideCommon.EvaluateJavaScriptAsync(FinanceWebView, readTableScript);

                // Remove surrounding quotes (JSON string returned from JS is doubly quoted)
                string json = System.Text.RegularExpressions.Regex.Unescape(rawJson.Trim('"'));

                // Deserialize to usable C# object
                //List<List<string>> tableRows = System.Text.Json.JsonSerializer.Deserialize<List<List<string>>>(json);
                List<List<string>> tableRows = JsonConvert.DeserializeObject<List<List<string>>>(json);
                string[] transactions = tableRows
                .Select(inner => string.Join(SmartParametersV2016.tab, inner))  // join each inner list into one string
                .ToArray();                                // convert to array

                Log.Warn("Nationwide", "Transactions: " + transactions.Length);

                DateTime accountCreated = SmartParametersV2016.defaultDate;
                List<SmartFinance.Accounts> accounts_found = financeviewmodel.PLO.finance_accountsList
                    .Where(a => a.INSTITUTION_CODE == institutionCode &&
                                a.BRAND_CODE == brandCode &&
                                a.SORTCODE == SortCode &&
                                a.ACCOUNT_NO == AccountNo &&
                                a.UDPRN == UDPRN)
                    .OrderByDescending(a => a.ACCOUNT_CREATED)
                    .ToList();

                if (accounts_found.Count > 0)
                {
                    accountCreated = accounts_found.First().ACCOUNT_CREATED;
                }

                if (!await NationwideCommon.NationwideTransactions(
                                        ourviewmodel,
                                        financeviewmodel,
                                        categoryCode,
                                        transactions,
                                        institutionCode,
                                        brandCode,
                                        SortCode,
                                        AccountNo,
                                        UDPRN,
                                        symbol,
                                        transaction_groupsFound,
                                        transaction_typesFound,
                                        accountCreated))
                {
                    financeviewmodel.errorMessage = "Transactions failed: " + financeviewmodel.errorMessage;
                    await SmartRoutinesV2018.TextBlockUpdate(
#if ANDROIDX
                                                             meterActivity,
#endif
                                                            ourviewmodel, financeviewmodel.errorMessage);
                    return false;
                }
            }

            // Update the Account index
            CurrentAccountIndex++;

            if (!await NationwideCommon.ProcessNextAccount(
                                            FinanceWebView,
                                            logClass,
                                            ourviewmodel,
                                            financeviewmodel,
                                            institutionCode,
                                            brandCode,
                                            CurrentAccountIndex,
                                            AccountLinks,
                                            UDPRN,
                                            st => Step = st,
                                            stip => StepInProgress = stip))
            {
                Log.Debug(logClass, "Process Account: Step6 failed");
                return false;
            }
            Log.Warn("Nationwide", "Returning from Step6");
            return true;
        }
        
        internal class ValueCallback : Java.Lang.Object, IValueCallback
        {
            private readonly Action<string> callback;
            public ValueCallback(Action<string> callback)
            {
                this.callback = callback;
            }

            public void OnReceiveValue(Java.Lang.Object value)
            {
                callback?.Invoke(value?.ToString());
            }
        }
        
        internal class JsResultCallback : Java.Lang.Object, IValueCallback
        {
            private readonly TaskCompletionSource<string> _tcs;

            public JsResultCallback(TaskCompletionSource<string> tcs)
            {
                _tcs = tcs;
            }

            public void OnReceiveValue(Java.Lang.Object result)
            {
                string json = result?.ToString();

                // JS returns a quoted string (e.g., "\"https://example.com\""), so unescape it
                if (!string.IsNullOrEmpty(json) && json != "null")
                {
                    json = json.Trim('"').Replace("\\u002F", "/");
                }
                else
                {
                    json = null;
                }

                _tcs.SetResult(json);
            }
        }
    }
}