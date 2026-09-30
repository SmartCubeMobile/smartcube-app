using Microsoft.Maui.Controls;

using System.Text.RegularExpressions;
using static System.Net.WebRequestMethods;

namespace SmartCubeMobile

{
    public class CustomNationwideClient
    {
        
        internal string logClass = "Nationwide";

        internal MainViewModel ourviewmodel;
        internal FinanceViewModel financeviewmodel;
        internal bool StepInProgress;
        internal int Step;
        internal string customerNumber,
                        birthDay,
                        birthMonth,
                        birthYear,
                        passcode;
        internal string notificationPrefix;
        internal short notificationTagLength;

        internal short institutionCode;
        internal short brandCode;

        internal List<SmartFinance.Transaction_Groups> transaction_groupsFound;
        internal List<SmartFinance.Transaction_Types> transaction_typesFound;

        internal string loginHref = "";
        internal bool loggedIn = false;
        internal List<AccountLink> AccountLinks { get; set; } = new();
        internal int CurrentAccountIndex { get; set; } = 0;
        internal TaskCompletionSource<bool> tcs;

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

        private Dictionary<short, string> javascriptTemplates;

        internal CustomNationwideClient(MainViewModel ourviewmodel,
                                        FinanceViewModel financeviewmodel,
                                        TaskCompletionSource<bool> tcs,
                                        short institutionCode,
                                        short brandCode,
                                        string customerNumber,
                                        string birthDay,
                                        string birthMonth,
                                        string birthYear,
                                        string passcode,
                                        string notificationPrefix,
                                        short notificationTagLength,
                                        List<SmartFinance.Transaction_Groups> transactionGroupsFound,
                                        List<SmartFinance.Transaction_Types> transactionTypesFound)
        {
            // Initialize your scripts dictionary
            javascriptTemplates = new Dictionary<short, string>
            {
                { 01, @"(function() {{ var meta = document.createElement('meta'); meta.name = 'viewport'; meta.content = 'width=device-width, initial-scale=0.3'; document.head.appendChild(meta); }})();" },
                { 10, @"(function() {{ try {{ let scripts = document.querySelectorAll('script'); for (let script of scripts) {{ if (script.textContent.includes('logInLink')) {{ let match = script.textContent.match(/""logInLink""\s*:\s*\{{[^}}]+\}}/); if (match && match[0]) {{ let jsonText = '{{' + match[0] + '}}'; let json = JSON.parse(jsonText); return json.logInLink.url; }} }} }} }} catch (e) {{ return ''; }} return ''; }})();" },
                { 21, @"(function() {{ var input = document.querySelector('input[name=""CustomerNumber""]'); if (input) {{ input.focus(); input.value = '{0}'; input.dispatchEvent(new Event('input', {{bubbles: true }})); input.dispatchEvent(new Event('change', {{bubbles: true }})); input.blur(); return 'Input filled successfully'; }} return 'Input not found'; }})();" },
                { 22, @"(function() {{ var input = document.querySelector('input[name=""DateOfBirthDay""]'); if (input) {{ input.focus(); input.value = '{0}'; input.dispatchEvent(new Event('input', {{ bubbles: true }})); input.dispatchEvent(new Event('change', {{ bubbles: true }})); input.blur(); return 'Input filled successfully'; }} return 'Input not found'; }})();" },
                { 23, @"(function() {{ var input = document.querySelector('input[name=""DateOfBirthYear""]'); if (input) {{ input.focus(); input.value = '{0}'; input.dispatchEvent(new Event('input', {{ bubbles: true }})); input.dispatchEvent(new Event('change', {{ bubbles: true }})); input.blur(); return 'Input filled successfully'; }} return 'Input not found'; }})();" },
                { 24, @"(function() {{ var select = document.querySelector('select[name=""DateOfBirthMonth""]'); if (select) {{ select.value = '{0}'; var event = new Event('change', {{ bubbles: true }}); select.dispatchEvent(event); return 'Month selected'; }} return 'Month dropdown not found'; }})();" },
                { 25, @"(function() {{ var btns = Array.from(document.querySelectorAll('button')); var btn = btns.find(b => b.textContent.trim().toLowerCase() === 'continue'); if (btn) {{ btn.click(); return 'Continue clicked'; }} return 'Button not found'; }})();" },
                { 31, @"(() => {{ const radio = document.querySelector(""input[type='radio'][value='PassNumberAndSMSOTP']""); if (radio && radio.offsetParent !== null) {{ radio.scrollIntoView({{ behavior: 'smooth', block: 'center' }}); radio.click(); return 'Clicked'; }} else {{ return 'NotFound'; }} }})()" },
                { 33, @"(function() {{ var btns = Array.from(document.querySelectorAll('button')); var btn = btns.find(b => b.textContent.trim().toLowerCase() === 'continue'); if (btn) {{ btn.click(); return 'ClickedContinue'; }} return 'ButtonNotFound'; }})();" },
                { 41, @"(function() {{ const welcomeElem = document.getElementById('welcome-message'); return welcomeElem && welcomeElem.innerText ? welcomeElem.innerText : ''; }})();" },
                { 42, @"(() => {{ const link = Array.from(document.querySelectorAll('a[href]')) .find(a => a.href.includes('{0}')); if (link) {{ link.click(); return 'ClickedAccountList'; }} return 'NotFound'; }})();" },
                { 43, @"(function() {{ let results = []; let activeElem = document.querySelector('.active'); if (!activeElem) return JSON.stringify(results); let anchors = activeElem.getElementsByTagName('a'); for(let i=0; i<anchors.length; i++) {{ let href = anchors[i].href || ''; if (href.includes('/AccountList/Account/RedirectToDefaultPage')) {{ results.push({{ href: href, innerHTML: anchors[i].innerHTML }}); }} }} return JSON.stringify(results);}})();" },
                { 44, @"(() => {{ const link = Array.from(document.querySelectorAll('a[href]')) .find(a => a.href.includes('{0}')); if (link) {{ link.click(); return 'ClickedPhoneAddress'; }} return 'NotFound'; }})();" },
                { 45, @"(function() {{ let addresses = document.getElementsByTagName('address'); if (addresses.length === 0) return ''; let addrText = addresses[0].innerText || ''; addrText = addrText.replace(/\r?\n/g, ' ').trim(); addrText = addrText.replace(/\s{2,}/g, ' '); return addrText; }})();" },
                { 46, @"(function() {{ let el = document.getElementById('CurrentEmailAddress'); return el ? el.value : ''; }})();" },
                { 47, @"(function() {{ let el = document.getElementById('MobilePhoneNumber'); return el ? el.value : ''; }})();" }

            };
            this.ourviewmodel = ourviewmodel;
            this.financeviewmodel = financeviewmodel;
            this.tcs = tcs;

            this.customerNumber = customerNumber;
            this.birthDay = birthDay;
            this.birthMonth = birthMonth;
            this.birthYear = birthYear;
            this.passcode = passcode;

            this.notificationPrefix = notificationPrefix;
            this.notificationTagLength = notificationTagLength;

            this.institutionCode = institutionCode;
            this.brandCode = brandCode;
            transaction_groupsFound = transactionGroupsFound;
            transaction_typesFound = transactionTypesFound;

            // Subscribe to OTP received event
            //com.keasdon.messagereceiver.SmartReceiver.OnOtpReceived += (ourviewmodel, financeviewmodel, webView, otp) =>
            //{
            //    // Fire-and-forget the async method since it's an async void
            //    NationwideCommon.HandleOtpReceived(webView, ourviewmodel, financeviewmodel, otp);
            //};

            financeviewmodel.financeWebView.Navigated += (s,e) => OnNavigated(s, e, financeviewmodel);

            Step = 2;
            StepInProgress = true;

            financeviewmodel.financeWebView.Source =
        "https://onlinebanking.nationwide.co.uk/AccessManagement/IdentifyCustomer/IdentifyCustomer";
        }

        internal async void OnNavigated(object sender,
                                        WebNavigatedEventArgs e,
                                        FinanceViewModel financeviewmodel)
        {
            if (e.Result != WebNavigationResult.Success)
                return;
            try
            {
                // Inject viewport scaling
                string js = GetJavaScript(financeviewmodel, 01, javascriptTemplates, "");

                await financeviewmodel.financeWebView.EvaluateJavaScriptAsync(js);

                if (StepInProgress)
                    return;

                StepInProgress = true;

                switch (Step)
                {
                    case 0:
                        await RunStep0(
                            ourviewmodel,
                            financeviewmodel);
                        break;

                    case 1:
                        // Not needed/used in WPF
                        //RunStep1(_webView);
                        break;

                    case 2:
                        await RunStep2(
                            ourviewmodel,
                            financeviewmodel,
                            customerNumber,
                            birthDay,
                            birthMonth,
                            birthYear);
                        break;

                    case 3:
                        await RunStep3(
                            ourviewmodel,
                            financeviewmodel,
                            passcode,
                            notificationPrefix,
                            notificationTagLength);
                        break;

                    case 4:
                        await RunStep4(
                            ourviewmodel,
                            financeviewmodel);
                        break;

                    case 45:
                        await RunStep45(
                            ourviewmodel,
                            financeviewmodel);
                        break;

                    case 5:
                        bool result5 = await RunStep5(ourviewmodel,
                                                        financeviewmodel,
                                                        institutionCode,
                                                        brandCode);
                        if (!result5)
                        {
                            await CleanupAndExitAsync(financeviewmodel);
                            return;
                        }
                        break;

                    case 6:
                        bool result6 = await RunStep6(ourviewmodel,
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
                            await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Finished processing accounts.");
                            await CleanupAndExitAsync(financeviewmodel);
                            return;
                        }
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

        internal async Task CleanupAndExitAsync(FinanceViewModel financeviewmodel)
        {
            financeviewmodel.financeWebView.Navigated -= (s, e) => OnNavigated(s, e, financeviewmodel);
            StepInProgress = false;

            await NationwideCommon.LogoutAsync(ourviewmodel, financeviewmodel, tcs, loggedIn);
            return;
        }

        internal string GetJavaScript(FinanceViewModel financeviewmodel, short key, Dictionary<short, string> templates, params object[] args)
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

        private async Task<bool> RunStep0(MainViewModel ourviewmodel, FinanceViewModel financeviewmodel)
        {

            
            //Console.WriteLine(html);
            //string script = GetJavaScript(financeviewmodel, 10, javascriptTemplates, "");
            try
            {
                
                string result = await financeviewmodel.financeWebView
                    .EvaluateJavaScriptAsync("document.readyState");

                string result1 = await financeviewmodel.financeWebView
    .EvaluateJavaScriptAsync(
        "document.querySelectorAll('button')[1].innerText");
                if (result1 == "Log in")
                {
                    await financeviewmodel.financeWebView.EvaluateJavaScriptAsync(@"
const btn = document.querySelectorAll('button')[1];
if (btn) {
    btn.scrollIntoView({ block: 'center' });

    btn.dispatchEvent(new MouseEvent('pointerdown', { bubbles: true }));
    btn.dispatchEvent(new MouseEvent('mousedown', { bubbles: true }));
    btn.dispatchEvent(new MouseEvent('mouseup', { bubbles: true }));
    btn.dispatchEvent(new MouseEvent('click', { bubbles: true }));
}
");
                    var result2 = await financeviewmodel.financeWebView.EvaluateJavaScriptAsync(@"
document.querySelectorAll('button')[1] ? 'Found' : 'Not found'
");
                    Console.WriteLine(result2);

                    var test = await financeviewmodel.financeWebView.EvaluateJavaScriptAsync("1+1");
                    Console.WriteLine(test);

                    var result3 = await financeviewmodel.financeWebView.EvaluateJavaScriptAsync(@"
({
    url: window.location.href,
    frames: window.length,
    buttons: document.querySelectorAll('button').length,
    ready: document.readyState
})
");
                    Console.WriteLine(result3);

                    var test2 = await financeviewmodel.financeWebView.EvaluateJavaScriptAsync(@"
document.body.innerHTML.length.toString()
");
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
                    financeviewmodel.financeWebView.Source = new UrlWebViewSource
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

        internal async Task<bool> RunStep2(MainViewModel ourviewmodel,
                                            FinanceViewModel financeviewmodel,
                                            string customerNumber,
                                            string birthDay,
                                            string birthMonth,
                                            string birthYear)
        {
            try
            {
                string jsCustomer = GetJavaScript(financeviewmodel, 21, javascriptTemplates, customerNumber);

                // Fill Customer Number            
                string customerResult = await financeviewmodel.financeWebView.EvaluateJavaScriptAsync(jsCustomer);
                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Customer Result: " + NationwideCommon.TrimQuotes(customerResult));
                // Fill Day
                string jsDay = GetJavaScript(financeviewmodel, 22, javascriptTemplates, birthDay);
                string dayResult = await financeviewmodel.financeWebView.EvaluateJavaScriptAsync(jsDay);
                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Day Result: " + NationwideCommon.TrimQuotes(dayResult));
                // Fill Year
                string jsYear = GetJavaScript(financeviewmodel, 23, javascriptTemplates, birthYear);
                string yearResult = await financeviewmodel.financeWebView.EvaluateJavaScriptAsync(jsYear);
                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Year Result: " + NationwideCommon.TrimQuotes(yearResult));
                // Select Month
                string jsMonth = GetJavaScript(financeviewmodel, 24, javascriptTemplates, birthMonth);
                string monthResult = await financeviewmodel.financeWebView.EvaluateJavaScriptAsync(jsMonth);
                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Month Result: " + NationwideCommon.TrimQuotes(monthResult));
                // Click "Continue" button
                string jsClick = GetJavaScript(financeviewmodel, 25, javascriptTemplates, 0);
                string clickResult = await financeviewmodel.financeWebView.EvaluateJavaScriptAsync(jsClick);
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

        internal async Task<bool> RunStep3(MainViewModel ourviewmodel,
                                                FinanceViewModel financeviewmodel,
                                                string passcode,
                                                string notificationPrefix,
                                                short notificationTagLength)
        {
            await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Status: Step 3");
            string jsscript = GetJavaScript(financeviewmodel, 31, javascriptTemplates, 0);

            try
            {
                string resultRadio = await financeviewmodel.financeWebView.EvaluateJavaScriptAsync(jsscript);
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
            if (!await NationwideCommon.NationwideFindPasscodes(ourviewmodel,
                                        financeviewmodel,
                                        passcode,
                                        la => logAction = la,
                                        em => errorMessage = em))
            {
                // Some error
                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Step 3: Find passcodes error");
                return false;
            }
            await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Step 3: Passcodes set");
            // Click Continue
            string jsClickContinue = GetJavaScript(financeviewmodel, 33, javascriptTemplates, 0);
            try
            {
                string clickResult = await financeviewmodel.financeWebView.EvaluateJavaScriptAsync(jsClickContinue);
                clickResult = NationwideCommon.TrimQuotes(clickResult);
                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, $"Step3 Click Continue result: {clickResult}");
            }
            catch (Exception ex)
            {
                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, $"Step3 Error executing continue JS: {ex}");
                return false;
            }

            //com.keasdon.messagereceiver.SmartReceiver.OnTimeout += (ourviewmodel, financeviewmodel, webView, tcs, loggedIn) =>
            //{
            //    // Fire-and-forget the async method since it's an async void
                NationwideCommon.HandleTimeout(ourviewmodel, financeviewmodel, tcs, loggedIn);
            //};

            // Start HTTP server to receive OTP etc.
            // But make sure you update the step first!!!
            Step = 4;
            StepInProgress = false;
            var listener = new NotificationListener(8080);

            var packet = await listener.WaitForNotificationAsync(TimeSpan.FromMinutes(3), "NATIONWIDE");
            await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, $"Step3 Waiting for OTP...");
            if (packet == null)
            {
                NationwideCommon.HandleTimeout(ourviewmodel, financeviewmodel, tcs, loggedIn);
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

                NationwideCommon.HandleOtpReceived(ourviewmodel, financeviewmodel, otp);
            }




            //await com.keasdon.messagereceiver.SmartReceiver.StartHttpServer(webView,
            //                                    ourviewmodel,
            //                                    financeviewmodel,
            //                                    tcs,
            //                                    loggedIn /*, no Context needed in WPF */);
            //await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, $"Step3 Waiting for OTP...");
            return true;
        }

        internal async Task<bool> RunStep4(MainViewModel ourviewmodel,
                                                FinanceViewModel financeviewmodel)
        {
            string jsWelcome = GetJavaScript(financeviewmodel, 41, javascriptTemplates, 0);
            try
            {
                string raw = await financeviewmodel.financeWebView.EvaluateJavaScriptAsync(jsWelcome);
                raw = NationwideCommon.TrimQuotes(raw);
                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, $"Step4 Welcome raw: {raw}");

                string welcomeText = raw; // since raw is already the innerText
                if (!string.IsNullOrEmpty(welcomeText))
                {
                    loggedIn = true;
                }
                string owner = welcomeText.Replace("Welcome back,", "").Trim();
                string ownerDisplay = "Owner: " + owner;
                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, $"Step4 {ownerDisplay}");

                string hrefAccContains = "AccountList/AccountList";
                string accountListHref = hrefAccContains.Replace("'", "\\'");
                string scriptAccountList = GetJavaScript(financeviewmodel, 42, javascriptTemplates, hrefAccContains);
                //string scriptAccountList = $@"
                //        (() => {{
                //            const link = Array.from(document.querySelectorAll('a[href]')).find(a => a.href.includes('{accountListHref}'));
                //            if (link) {{
                //                link.click();
                //                return true;
                //            }} else {{
                //                return false;
                //            }}
                //        }})()";


                //string scriptAccountList = @"(() => {{ const link = Array.from(document.querySelectorAll('a[href]')) .find(a => a.href.includes('{hrefAccContains}')); if (link) {{ link.click(); return 'ClickedAccountList'; }} return 'NotFound'; }})();";


                string accountClickResult = await financeviewmodel.financeWebView.EvaluateJavaScriptAsync(scriptAccountList);
                accountClickResult = NationwideCommon.TrimQuotes(accountClickResult);
                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, $"Step4 Click AccountList result: {accountClickResult}");
                Step = 45;
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

        internal async Task<bool> RunStep45(MainViewModel ourviewmodel,
                                            FinanceViewModel financeviewmodel)
        {
            string jsList = GetJavaScript(financeviewmodel, 43, javascriptTemplates, "");
            try
            {
                //string jsList = @"
                //    (function() {
                //        let results = [];
                //        let activeElem = document.querySelector('.active');
                //        if (!activeElem) return JSON.stringify(results);
                //        let anchors = activeElem.getElementsByTagName('a');
                //        for(let i=0; i<anchors.length; i++) {
                //            let href = anchors[i].href || '';
                //            if (href.includes('/AccountList/Account/RedirectToDefaultPage')) {
                //                results.push({
                //                    href: href,
                //                    innerHTML: anchors[i].innerHTML
                //                });
                //            }
                //        }
                //        return JSON.stringify(results);
                //    })();";


                string jsonResult = await financeviewmodel.financeWebView.EvaluateJavaScriptAsync(jsList);

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
                        await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, $"Step4 Link: {acc.href} – {acc.innerHTML}");
                    }
                }

                string hrefContains2 = "MaintainTelephoneAndAddress";
                string clickPhoneAddress = GetJavaScript(financeviewmodel, 44, javascriptTemplates, hrefContains2);

                string phoneAddressResult = await financeviewmodel.financeWebView.EvaluateJavaScriptAsync(clickPhoneAddress);
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
        internal async Task<bool> RunStep5(MainViewModel ourviewmodel,
                                                FinanceViewModel financeviewmodel,
                                                short institutionCode,
                                                short brandCode)
        {

            await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "RunStep5: started");
            UDPRN = "";

            try
            {
                // Step 4: get the address
                //string getAddressScript = GetJavaScript(financeviewmodel, 45, javascriptTemplates, "");

                string getAddressScript = @"
                (function() {
                    let addresses = document.getElementsByTagName('address');
                    if (addresses.length === 0) return '';
                    let addrText = addresses[0].innerText || '';
                    addrText = addrText.replace(/\r?\n/g, ' ').trim();
                    addrText = addrText.replace(/\s{2,}/g, ' ');
                    return addrText;
                })();";
                string addressJson = await financeviewmodel.financeWebView.EvaluateJavaScriptAsync(getAddressScript);
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
                string getEmailScript = GetJavaScript(financeviewmodel, 46, javascriptTemplates, "");
                //string getEmailScript = @"
                //    (function() {
                //        let el = document.getElementById('CurrentEmailAddress');
                //        return el ? el.value : '';
                //    })();";
                string emailJson = await financeviewmodel.financeWebView.EvaluateJavaScriptAsync(getEmailScript);
                emailJson = NationwideCommon.TrimQuotes(emailJson);
                string email = "Email: " + emailJson;
                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, $"Step5 {email}");

                // Extract phone via JS
                string getPhoneScript = GetJavaScript(financeviewmodel, 47, javascriptTemplates, "");
                //string getPhoneScript = @"
                //    (function() {
                //        let el = document.getElementById('MobilePhoneNumber');
                //        return el ? el.value : '';
                //    })();";
                string phoneJson = await financeviewmodel.financeWebView.EvaluateJavaScriptAsync(getPhoneScript);
                phoneJson = NationwideCommon.TrimQuotes(phoneJson);
                string phone = "Phone: " + phoneJson;
                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, $"Step5 {phone}");

                // If no account links, bail
                if (AccountLinks == null || AccountLinks.Count == 0)
                {
                    await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Step5 No account links, logging out or fail");
                    return false;
                }

                bool processed = await NationwideCommon.ProcessNextAccount(ourviewmodel,
                                                                            financeviewmodel,
                                                                            institutionCode,
                                                                            brandCode,
                                                                            CurrentAccountIndex,
                                                                            AccountLinks,
                                                                            UDPRN,
                                                                            logClass,
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

        internal async Task<bool> RunStep6(MainViewModel ourviewmodel,
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
            await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, $"RunStep6: Starting (UDPRN = {UDPRN})");

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

            string clickButton = await financeviewmodel.financeWebView.EvaluateJavaScriptAsync(clickButtonScript);
            clickButton = NationwideCommon.TrimQuotes(clickButton);
            if (clickButton != "clicked")
            {
                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, $"Step6 clickButton result = {clickButton}, assuming no transactions");
            }
            else
            {
                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Step6 clicked, waiting a bit for page to update");
                await Task.Delay(1500);

                string readTableScript = @"
        (() => {
            try {
                const tbody = document.querySelector('tbody');
                if (!tbody) return 'no_tbody';

                const rows = Array.from(tbody.querySelectorAll('tr')).map(row => {
                    return Array.from(row.querySelectorAll('td'))
                                .map(cell => cell.innerText.trim());
                });
                return JSON.stringify(rows);
            } catch (e) {
                return 'script_error: ' + e.message;
            }
        })();";

                string rawJson = await financeviewmodel.financeWebView.EvaluateJavaScriptAsync(readTableScript);
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
                                                                            transaction_groupsFound,
                                                                            transaction_typesFound,
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

            bool nextOk = await NationwideCommon.ProcessNextAccount(ourviewmodel,
                                                                    financeviewmodel,
                                                                    institutionCode,
                                                                    brandCode,
                                                                    CurrentAccountIndex,
                                                                    AccountLinks,
                                                                    UDPRN,
                                                                    logClass,
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
        //internal static async Task<string> EvaluateJavaScriptAsync(WebView webView, string script)
        //{
        //    try
        //    {
        //        return await webView.ExecuteScriptAsync(script);
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine("EvaluateJavaScriptAsync error: " + ex.Message);
        //        return string.Empty;
        //    }
        //}
        internal static async Task<string> EvaluateJavaScriptAsync(WebView webView,
                                                                    string script)
        {
            try
            {
                return await webView.EvaluateJavaScriptAsync(script);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return string.Empty;
            }
        }
    }
}