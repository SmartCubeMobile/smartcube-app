using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using com.keasdon.messagereceiver;



#if WINFORMS
using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.WinForms;
#endif

#if WPF
using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.Wpf;
#endif

#if WINUI
using Microsoft.Web.WebView2.Core;
using Microsoft.UI.Xaml.Controls;
#endif

namespace SmartCubeMobile
{
    public class CustomNationwideClient
    {
        internal readonly WebView2 webView;

        internal string logClass = "Nationwide";

        //public Context context;
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
        internal CustomNationwideClient(WebView2 WebView,
                                    MainViewModel Ourviewmodel,
                                    FinanceViewModel Financeviewmodel,
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
            webView = WebView;
            ourviewmodel = Ourviewmodel;
            financeviewmodel = Financeviewmodel;
            tcs = TCS;
            customerNumber = CustomerNumber;
            birthDay = BirthDay;
            birthMonth = BirthMonth;
            birthYear = BirthYear;
            passcode = Passcode;
            institutionCode = InstitutionCode;
            brandCode = BrandCode;
            transaction_groupsFound = TransactionGroupsFound;
            transaction_typesFound = TransactionTypesFound;

            // Subscribe to OTP received event
            SmartReceiver.OnOtpReceived += (ourviewmodel, financeviewmodel, webView, otp) =>
            {
                // Fire-and-forget the async method since it's an async void
                NationwideCommon.HandleOtpReceived(webView, ourviewmodel, financeviewmodel, otp);
            };
            webView.NavigationCompleted += OnNavigationCompleted;

        }

        public async void OnNavigationCompleted(object sender, CoreWebView2NavigationCompletedEventArgs e)
        {
            if (!e.IsSuccess) return;

            try
            {
                // Inject viewport scaling
                string js = GetJavaScript(financeviewmodel, 01, javascriptTemplates, "");

                await webView.ExecuteScriptAsync(js);
                
                if (StepInProgress)
                    return;

                StepInProgress = true;

                switch (Step)
                {
                    case 0:
                        await RunStep0(webView,                                        
                                        ourviewmodel,
                                        financeviewmodel);
                        break;
                    case 1:
                        // Not needed/used in WPF
                        //RunStep1(_webView);
                        break;
                    case 2:
                        await RunStep2(webView,
                                    ourviewmodel,
                                    financeviewmodel,
                                    customerNumber,
                                    birthDay,
                                    birthMonth,
                                    birthYear);
                        break;
                   case 3:
                        await RunStep3(ourviewmodel,
                                    financeviewmodel,
                                    webView,
                                    passcode);
                        break;
                    case 4:
                        await RunStep4(webView,
                                    ourviewmodel,
                                    financeviewmodel);
                        break;
                    case 45:
                        await RunStep45(webView,
                                    ourviewmodel,
                                    financeviewmodel);
                        break;
                    case 5:
                        bool result5 = await RunStep5(webView,
                                                        ourviewmodel,
                                                        financeviewmodel,
                                                        institutionCode,
                                                        brandCode);
                        if (!result5)
                        {
#if WPF
                            await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Finished processing accounts.");
#endif

                            await CleanupAndExitAsync();
                            //await NationwideCommon.LogoutAsync(webView,
                            //                                ourviewmodel,
                            //                                financeviewmodel,
                            //                                tcs,
                            //                                loggedIn);
                            //webView.NavigationCompleted -= OnNavigationCompleted;
                            return;
                        }
                        break;
                    case 6:
                        bool result6 = await RunStep6(webView, 
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
                            await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Finished processing accounts.");
                            await CleanupAndExitAsync();
                            //await NationwideCommon.LogoutAsync(webView,
                            //                                    ourviewmodel,
                            //                                    financeviewmodel,
                            //                                    tcs,
                            //                                    loggedIn);

                            //webView.NavigationCompleted -= OnNavigationCompleted;
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

        internal async Task CleanupAndExitAsync()
        {
            webView.NavigationCompleted -= OnNavigationCompleted;
            StepInProgress = false;

            await NationwideCommon.LogoutAsync(webView, ourviewmodel, financeviewmodel, tcs, loggedIn);
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

        private async Task<bool> RunStep0(WebView2 webView, MainViewModel ourviewmodel, FinanceViewModel financeviewmodel)
        {
            string script = GetJavaScript(financeviewmodel, 10, javascriptTemplates, "");
            try
            {
                string result = await webView.ExecuteScriptAsync(script);
                // WebView2 returns JSON-encoded strings (i.e., with double quotes), so trim them
                loginHref = NationwideCommon.TrimQuotes(result);                
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
                    webView.Source = new Uri(loginHref);
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

        internal async Task<bool> RunStep2(WebView2 webView, 
                                            MainViewModel ourviewmodel,
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
                string customerResult = await webView.ExecuteScriptAsync(jsCustomer);
                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Customer Result: " + NationwideCommon.TrimQuotes(customerResult));
                // Fill Day
                string jsDay = GetJavaScript(financeviewmodel, 22, javascriptTemplates, birthDay);
                string dayResult = await webView.ExecuteScriptAsync(jsDay);
                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Day Result: " + NationwideCommon.TrimQuotes(dayResult));
                // Fill Year
                string jsYear = GetJavaScript(financeviewmodel, 23, javascriptTemplates, birthYear);
                string yearResult = await webView.ExecuteScriptAsync(jsYear);
                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Year Result: " + NationwideCommon.TrimQuotes(yearResult));
                // Select Month
                string jsMonth = GetJavaScript(financeviewmodel, 24, javascriptTemplates, birthMonth);
                string monthResult = await webView.ExecuteScriptAsync(jsMonth);
                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Month Result: " + NationwideCommon.TrimQuotes(monthResult));
                // Click "Continue" button
                string jsClick = GetJavaScript(financeviewmodel, 25, javascriptTemplates, 0);
                string clickResult = await webView.ExecuteScriptAsync(jsClick);
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
                                              WebView2 webView,
                                              string passcode)
        {
            await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Status: Step 3");
            string jsscript = GetJavaScript(financeviewmodel, 31, javascriptTemplates, 0);

            try
            {
                string resultRadio = await webView.ExecuteScriptAsync(jsscript);
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
            if (!await NationwideCommon.NationwideFindPasscodes(webView,
                                        ourviewmodel,
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
                string clickResult = await webView.ExecuteScriptAsync(jsClickContinue);
                clickResult = NationwideCommon.TrimQuotes(clickResult);
                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, $"Step3 Click Continue result: {clickResult}");
            }
            catch (Exception ex)
            {
                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, $"Step3 Error executing continue JS: {ex}");
                return false;
            }

            SmartReceiver.OnTimeout += (ourviewmodel, financeviewmodel, webView, tcs, loggedIn) =>
            {
                // Fire-and-forget the async method since it's an async void
                NationwideCommon.HandleTimeout(webView, ourviewmodel, financeviewmodel, tcs, loggedIn);
            };

            // Start HTTP server to receive OTP etc.
            // But make sure you update the step first!!!
            Step = 4;
            StepInProgress = false;

            await SmartReceiver.StartHttpServer(webView,
                                                ourviewmodel, 
                                                financeviewmodel, 
                                                tcs,
                                                loggedIn /*, no Context needed in WPF */);
            await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, $"Step3 Waiting for OTP...");
            return true;
        }

        internal async Task<bool> RunStep4(WebView2 webView,
                                              MainViewModel ourviewmodel,
                                              FinanceViewModel financeviewmodel)
        {
            string jsWelcome = GetJavaScript(financeviewmodel, 41, javascriptTemplates, 0);
            try
            {
                string raw = await webView.ExecuteScriptAsync(jsWelcome);
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


                string accountClickResult = await webView.ExecuteScriptAsync(scriptAccountList);
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

        internal async Task<bool> RunStep45(WebView2 webView,
                                  MainViewModel ourviewmodel,
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


                string jsonResult = await webView.ExecuteScriptAsync(jsList);

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

                string phoneAddressResult = await webView.ExecuteScriptAsync(clickPhoneAddress);
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
        internal async Task<bool> RunStep5(WebView2 webView,
                                              MainViewModel ourviewmodel,
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
                string addressJson = await webView.ExecuteScriptAsync(getAddressScript);
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
                string emailJson = await webView.ExecuteScriptAsync(getEmailScript);
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
                string phoneJson = await webView.ExecuteScriptAsync(getPhoneScript);
                phoneJson = NationwideCommon.TrimQuotes(phoneJson);
                string phone = "Phone: " + phoneJson;
                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, $"Step5 {phone}");

                // If no account links, bail
                if (AccountLinks == null || AccountLinks.Count == 0)
                {
                    await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Step5 No account links, logging out or fail");
                    return false;
                }

                bool processed = await NationwideCommon.ProcessNextAccount(webView,
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

        internal async Task<bool> RunStep6(WebView2 webView,
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

            string clickButton = await webView.ExecuteScriptAsync(clickButtonScript);
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

                string rawJson = await webView.ExecuteScriptAsync(readTableScript);
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

            bool nextOk = await NationwideCommon.ProcessNextAccount(webView,
                                                                    ourviewmodel,
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
        internal static async Task<string> EvaluateJavaScriptAsync(WebView2 webView, string script)
        {
            try
            {
                return await webView.ExecuteScriptAsync(script);
            }
            catch (Exception ex)
            {
                Console.WriteLine("EvaluateJavaScriptAsync error: " + ex.Message);
                return string.Empty;
            }
        }
    }
}

// Also ensure your LogoutAsync, other Steps, etc. are adapted to use WebView2 instead of Android WebView


//SmartReceiver.OnOtpReceived += NationwideCommon.HandleOtpReceived;
//SmartReceiver.OnOtpReceived += HandleOtpReceived;

//        internal static async Task<bool> LogoutAsync(
//                                                    TaskCompletionSource<bool> tcs,
//                                                    MainViewModel ourviewmodel,
//                                                    FinanceViewModel financeviewmodel,
//                                                    WebView2 webView,
//                                                    bool loggedIn)
//        {
//            if (loggedIn)
//            {
//                // No async so we can process the strings returned
//                const string scriptLogout = @"
//            (function() {
//                const anchors = Array.from(document.querySelectorAll('a'));
//                const logoutLink = anchors.find(el =>
//                    el.textContent.trim().toLowerCase() === 'log out');
//                if (logoutLink) {
//                    logoutLink.click();
//                    return true;
//                } else {
//                    return false;
//                }
//            })();";
//                try
//                {
//                    string resultLogout = await EvaluateJavaScriptAsync(webView, scriptLogout);
//                    // Logout
//                    loggedIn = false;
//                    await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Logout : " + resultLogout);
//                }
//                catch (Exception ex)
//                {
//                    financeviewmodel.errorMessage = "Logout failed: " + ex.Message;
//                    await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, financeviewmodel.errorMessage);
//                }
//            }
//            // Completion gets set after this
//            if (webView != null)
//            {
//#if ANDROIDX
//                webView.Visibility = ViewStates.Gone;
//#endif
//                // Don't dispose of it - yet
//                // We may need it again
//            }

//            tcs.TrySetResult(true); // Unblocks Task.WhenAll
//            return true;
//        }




//string js = @"
//    (function() {
//        var meta = document.createElement('meta');
//        meta.name = 'viewport';
//        meta.content = 'width=device-width, initial-scale=0.3';
//        document.head.appendChild(meta);
//    })();";


//string script = @"
//(function() {
//    try {
//        let scripts = document.querySelectorAll('script');
//        for (let script of scripts) {
//            if (script.textContent.includes('logInLink')) {
//                let match = script.textContent.match(/""logInLink""\s*:\s*\{[^}]+\}/);                                    
//                if (match && match[0]) {
//                    let jsonText = '{' + match[0] + '}';
//                    let json = JSON.parse(jsonText);
//                    return json.logInLink.url;
//                }
//            }
//        }
//    } catch (e) {
//        return '';
//    }
//    return '';
//})();";

//    string jsCustomer = $@"
//(function() {{
//    var input = document.querySelector('input[name=""CustomerNumber""]');
//    if (input) {{
//        input.focus();
//        input.value = '{customerNumber}';
//        input.dispatchEvent(new Event('input', {{bubbles: true }}));
//        input.dispatchEvent(new Event('change', {{bubbles: true }}));
//        input.blur();
//        return 'Input filled successfully';
//    }}
//    return 'Input not found';
//}})();";

//    string jsDay = $@"
//(function() {{
//    var input = document.querySelector('input[name=""DateOfBirthDay""]');
//    if (input) {{
//        input.focus();
//        input.value = '{birthDay}';
//        input.dispatchEvent(new Event('input', {{ bubbles: true }}));
//        input.dispatchEvent(new Event('change', {{ bubbles: true }}));
//        input.blur();
//        return 'Input filled successfully';
//    }}
//    return 'Input not found';
//}})();";

//    string jsYear = $@"
//(function() {{
//    var input = document.querySelector('input[name=""DateOfBirthYear""]');
//    if (input) {{
//        input.focus();
//        input.value = '{birthYear}';
//        input.dispatchEvent(new Event('input', {{ bubbles: true }}));
//        input.dispatchEvent(new Event('change', {{ bubbles: true }}));
//        input.blur();
//        return 'Input filled successfully';
//    }}
//    return 'Input not found';
//}})();";

//    string jsMonth = $@"
//(function() {{
//    var select = document.querySelector('select[name=""DateOfBirthMonth""]');
//    if (select) {{
//        select.value = '{birthMonth}';
//        var event = new Event('change', {{ bubbles: true }});
//        select.dispatchEvent(event);
//        return 'Month selected';
//    }}
//    return 'Month dropdown not found';
//}})();";

//    string jsClick = @"
//(function() {
//    var btns = Array.from(document.querySelectorAll('button'));
//    var btn = btns.find(b => b.textContent.trim().toLowerCase() === 'continue');
//    if (btn) {
//        btn.click();
//        return 'Continue clicked';
//    }
//    return 'Button not found';
//})();";

//string script = @"
//                (() => {
//                    const radio = document.querySelector(""input[type='radio'][value='PassNumberAndSMSOTP']"");
//                    if (radio && radio.offsetParent !== null) {  // Checks for visibility
//                        radio.scrollIntoView({ behavior: 'smooth', block: 'center' });
//                        radio.click();
//                        return 'Clicked';
//                    } else {
//                        return 'NotFound';
//                    }
//                })()";


//string jsClickContinue = @"
//        (function() {
//            var btns = Array.from(document.querySelectorAll('button'));
//            var btn = btns.find(b => b.textContent.trim().toLowerCase() === 'continue');
//            if (btn) {
//                btn.click();
//                return 'ClickedContinue';
//            }
//            return 'ButtonNotFound';
//        })();";


//string clickPhoneAddress = $@"
//    (() => {{
//        const link = Array.from(document.querySelectorAll('a[href]'))
//            .find(a => a.href.includes('{hrefContains2}'));
//        if (link) {{
//            link.click();
//            return 'ClickedPhoneAddress';
//        }}
//        return 'NotFound';
//    }})();";


//string scriptAccountList = $@"
//    (() => {{
//        const link = Array.from(document.querySelectorAll('a[href]'))
//            .find(a => a.href.includes('{hrefContains}'));
//        if (link) {{
//            link.click();
//            return 'ClickedAccountList';
//        }}
//        return 'NotFound';
//    }})();";

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

//string jsWelcome = @"
//    (function() {
//        const welcomeElem = document.getElementById('welcome-message');
//        return welcomeElem && welcomeElem.innerText ? welcomeElem.innerText : '';
//    })();";

//private async void HandleTimeout(MainViewModel ourviewmodel, FinanceViewModel financeviewmodel, WebView2 webView, bool loggedIn)
//{
//    await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Timeout: No OTP received in time");
//    await LogoutAsync(tcs, ourviewmodel, financeviewmodel, webView, loggedIn);
//    return;
//}

//private void HandleOtpReceived(MainViewModel ourviewmodel,
//                                FinanceViewModel financeviewmodel,
//                                WebView2 webView,

//                                string otp)
//{
//    Application.Current.Dispatcher.Invoke(async () =>
//    {
//        SmartReceiver.OnOtpReceived -= HandleOtpReceived;
//        //SmartReceiver.OnTimeout -= NationwideCommon.HandleTimeout;
//        SmartReceiver.OnTimeout -= (ourviewmodel, financeviewmodel, webView, loggedIn) =>
//        {
//            // Fire-and-forget the async method since it's an async void
//            NationwideCommon.HandleTimeout(webView, ourviewmodel, financeviewmodel, tcs, loggedIn);
//        };


//        await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "OTP Received: " + otp);

//        string js = $@"
//            (function() {{
//                var input = document.querySelector('input[name=""OneTimePasscode""]');
//                if (input) {{
//                    input.focus();
//                    input.value = '{otp}';
//                    input.dispatchEvent(new Event('input', {{bubbles: true}}));
//                    input.dispatchEvent(new Event('change', {{bubbles: true}}));
//                    input.blur();
//                }}
//            }})();";
//        await _webView.ExecuteScriptAsync(js);

//        string clickScript = @"
//            (function() {
//                var btns = Array.from(document.querySelectorAll('button'));
//                var btn = btns.find(b => b.textContent.trim().toLowerCase() === 'log in');
//                if (btn) btn.click();
//            })();";
//        await _webView.ExecuteScriptAsync(clickScript);
//    });
//    StepInProgress = false;
//}

//string scriptAccountList = $@"
//        (() => {{
//            const link = Array.from(document.querySelectorAll('a[href]'))
//                .find(a => a.href.includes('{hrefAccContains}'));
//            if (link) {{
//                link.click();
//                return 'ClickedAccountList';
//            }}
//            return 'NotFound';
//        }})();";