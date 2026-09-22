using System.Text.RegularExpressions;

#if WINFORMS
using Microsoft.Web.WebView2.Core;
using System.Text.Json;
#endif

#if WPF
using Microsoft.Web.WebView2.Core;
using System.Text.Json;
using System.Windows;
#endif

#if UWP || WINUI
using System.Threading.Tasks;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Text.Json;
using Microsoft.UI.Xaml.Controls;
using Microsoft.Web.WebView2.Core;
using Windows.Foundation;
#endif


#if ANDROID
using Android.Content;
using Android.Provider;
#endif

#if !ANDROID
namespace SmartCubeMobile
{
    // Well, I have Dan to thank for showing me chatGPT because
    // it certainly saved my sorry arse at 20:20 today 8th August 2025
    // as I have been struggling for an entire FIVE DAYS to get this
    // bitch working ... but here it is: a routine that finds
    // the anchor containing the href ... clicks it and WAITS
    // for the fucking page to load!  Now .. can I use this bastard
    // anywhere else??  Time for a cuppa, Ray! Time for a cuppa ...

    internal class NationwideNewWebView2V2025
    {
        internal static string UDPRN = "";
        // Some 'necessary evil' globals
        internal static int step = 0;
        internal static string owner = "";
        internal static List<AccountLink> AccountLinks { get; set; } = new List<AccountLink>();
        internal static int CurrentAccountIndex { get; set; } = 0;
        internal static bool IsProcessingAccounts { get; set; } = false;

#if WINFORMS || WPF || UWP
        internal static EventHandler<CoreWebView2NavigationCompletedEventArgs> InitialHandler = null;
#endif
#if WINUI
        internal static TypedEventHandler<WebView2, CoreWebView2NavigationCompletedEventArgs> InitialHandler = null;
#endif
        internal static async Task RunNationwideNewWebView2(
#if WINFORMS
                                                    SmartDashboard.MainProcess components,
                                                    RichTextBox textBoxConsole,
#endif
                                                    TaskCompletionSource<bool> tcs,
                                                    MainViewModel ourviewmodel,
                                                    FinanceViewModel financeviewmodel,
                                                    SmartFinance.Logins loginInfo,
                                                    string startUrl,
                                                    string notificationTitle,
                                                    string notificationPrefix,
                                                    short notificationTagLength,
                                                    short institution_code,
                                                    short brand_code,
                                                    List<SmartFinance.Transaction_Groups> transaction_groupsFound,
                                                    List<SmartFinance.Transaction_Types> transaction_typesFound,
                                                    string Parameter1,
                                                    string Parameter2,
                                                    string Parameter3,
                                                    short loginMethod)
        {
#if WINFORMS
            financeviewmodel.FinanceWebView.Visible = true;            
#endif
#if WPF
            financeviewmodel.FinanceWebView.Visibility = Visibility.Visible;
            financeviewmodel.FinanceWebView.HorizontalAlignment = HorizontalAlignment.Stretch;
            financeviewmodel.FinanceWebView.VerticalAlignment = VerticalAlignment.Stretch;
#endif
#if WINFORMS
            // See chatGPT standard setup for WebView2 Forms
#endif
            try
            {
                await financeviewmodel.FinanceWebView.EnsureCoreWebView2Async();
                await financeviewmodel.FinanceWebView.CoreWebView2.Profile.ClearBrowsingDataAsync(CoreWebView2BrowsingDataKinds.DiskCache);
            }
            catch (Exception ex)
            {
                financeviewmodel.errorMessage = "Ensure WebView2 failed: " + ex.Message;
                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, financeviewmodel.errorMessage);
                await Task.CompletedTask;
            }

            financeviewmodel.notificationHandledSource = new TaskCompletionSource<bool>();
            financeviewmodel.FinanceWebView.CoreWebView2.NewWindowRequested += (sender, args) => args.Handled = true;
#if WPF || UWP || WINUI
            ourviewmodel.webviewLogging = false;
            if (ourviewmodel.webviewLogging)
            {
                financeviewmodel.FinanceWebView.CoreWebView2.OpenDevToolsWindow();
                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "WebView2 logging: Enabled");
            }
#endif
            // Is this really needed again??
            await financeviewmodel.FinanceWebView.CoreWebView2.Profile.ClearBrowsingDataAsync();
            
            //int step = 0;
            string logAction = string.Empty;
            string errorMessage = string.Empty;

            // Don't forget this shit!!! It's really useful!!!!
            //financeviewmodel.FinanceWebView.CoreWebView2.OpenDevToolsWindow();
            
            // Set up to receive messages
#if WINFORMS || WPF
            financeviewmodel.FinanceWebView.CoreWebView2.WebMessageReceived +=
                    new EventHandler<CoreWebView2WebMessageReceivedEventArgs>((s, e) => CoreWebView2_WebMessageReceived(s, e,
#if WINFORMS
                                                    components,
                                                    textBoxConsole,
#endif
                                                    tcs,
                                                    ourviewmodel,
                                                    financeviewmodel,
                                                    loginInfo,
                                                    notificationTitle,
                                                    notificationPrefix,
                                                    notificationTagLength,
                                                    institution_code,
                                                    brand_code,
                                                    transaction_groupsFound,
                                                    transaction_typesFound,
                                                    Parameter1,
                                                    Parameter2));
#endif
#if WINUI
            financeviewmodel.FinanceWebView.CoreWebView2.WebMessageReceived +=
                    (s, e) => CoreWebView2_WebMessageReceived(s, e,
                                        tcs,
                                        ourviewmodel,
                                        financeviewmodel,
                                        loginInfo,
                                        notificationTitle,
                                        notificationPrefix,
                                        notificationTagLength,
                                        institution_code,
                                        brand_code,
                                        transaction_groupsFound,
                                        transaction_typesFound,
                                        Parameter1,
                                        Parameter2);
#endif
            // Set up to deal with Navigation, we have to do it this
            // way so we can uncouple it later to do the accounts

#if WINFORMS || WPF
            InitialHandler = InitialHandlerZ(
#if WINFORMS
                                            components,
                                            textBoxConsole,
#endif
                                            tcs,
                                            ourviewmodel,
                                            financeviewmodel,
                                            loginInfo,
                                            startUrl,
                                            notificationTitle,
                                            notificationPrefix,
                                            notificationTagLength,
                                            institution_code,
                                            brand_code,
                                            transaction_groupsFound,
                                            transaction_typesFound,
                                            Parameter1,
                                            Parameter2,
                                            Parameter3,
                                            "",                 // Sort Code
                                            "",                 // Account No
                                            SmartParametersV2016.defaultChar,   // Category
                                            "",                // Currency Symbol
                                            loginMethod);
            financeviewmodel.FinanceWebView.CoreWebView2.NavigationCompleted += InitialHandler; //
#endif
#if WINUI
            InitialHandler = CreateNavigationHandler(tcs,
                                            ourviewmodel,
                                            financeviewmodel,
                                            loginInfo,
                                            startUrl,
                                            notificationTitle,
                                            notificationPrefix,
                                            notificationTagLength,
                                            institution_code,
                                            brand_code,
                                            transaction_groupsFound,
                                            transaction_typesFound,
                                            Parameter1,
                                            Parameter2,
                                            Parameter3,
                                            "",             // Sort Code
                                            "",             // Account No
                                            SmartParametersV2016.defaultChar,   // Category
                                            "",              // Currency Symbol
                                            loginMethod);
            financeviewmodel.FinanceWebView.NavigationCompleted += InitialHandler;
#endif

            financeviewmodel.FinanceWebView.Source = new Uri(startUrl);
            //await financeviewmodel.tcs1X.Task;
        }
#if WINUI
        private static TypedEventHandler<WebView2, CoreWebView2NavigationCompletedEventArgs> CreateNavigationHandler(
                                                    TaskCompletionSource<bool> tcs,
                                                    MainViewModel ourviewmodel,
                                                    FinanceViewModel financeviewmodel,
                                                    SmartFinance.Logins loginInfo,
                                                    string startUrl,
                                                    string notificationTitle,
                                                    string notificationPrefix,
                                                    short notificationTagLength,
                                                    short institution_code,
                                                    short brand_code,
                                                    List<SmartFinance.Transaction_Groups> transaction_groupsFound,
                                                    List<SmartFinance.Transaction_Types> transaction_typesFound,
                                                    string Parameter1,
                                                    string Parameter2,
                                                    string Parameter3,
                                                    string sortCode,
                                                    string accountNo,
                                                    char categoryCode,
                                                    string symbol,
                                                    short loginMethod)
        {
            return async (WebView2 sender, CoreWebView2NavigationCompletedEventArgs args) =>
            {
                await InitialHandlerZ(sender, args,
                                                tcs,
                                                ourviewmodel,
                                                financeviewmodel,
                                                loginInfo,
                                                startUrl,
                                                notificationTitle,
                                                notificationPrefix,
                                                notificationTagLength,
                                                institution_code,
                                                brand_code,
                                                transaction_groupsFound,
                                                transaction_typesFound,
                                                Parameter1,
                                                Parameter2,
                                                Parameter3,
                                                sortCode, 
                                                accountNo,
                                                categoryCode,
                                                symbol,
                                                loginMethod);
            };
        }
#endif


#if WINFORMS || WPF || UWP
        internal static EventHandler<CoreWebView2NavigationCompletedEventArgs> InitialHandlerZ(
#if WINFORMS
                                                    SmartDashboard.MainProcess components,
                                                    RichTextBox textBoxConsole,
#endif
                                                    TaskCompletionSource<bool> tcs,MainViewModel ourviewmodel,
                                                    FinanceViewModel financeviewmodel,
                                                    SmartFinance.Logins loginInfo,
                                                    string startUrl,
                                                    string notificationTitle,
                                                    string notificationPrefix,
                                                    short notificationTagLength,
                                                    short institution_code,
                                                    short brand_code,
                                                    List<SmartFinance.Transaction_Groups> transaction_groupsFound,
                                                    List<SmartFinance.Transaction_Types> transaction_typesFound,
                                                    string Parameter1,
                                                    string Parameter2,
                                                    string Parameter3,
                                                    string sortCode,
                                                    string accountNo,
                                                    char categoryCode,
                                                    string symbol,
                                                    short loginMethod,
                                                    Action<string, string> onFailure = null)
        {
            return async (object sender, CoreWebView2NavigationCompletedEventArgs args) =>
            {
#endif
#if WINUI
        private static async Task InitialHandlerZ(object sender,
                                                    CoreWebView2NavigationCompletedEventArgs args,
                                                    TaskCompletionSource<bool> tcs,
                                                    MainViewModel ourviewmodel,
                                                    FinanceViewModel financeviewmodel,
                                                    SmartFinance.Logins loginInfo,
                                                    string startUrl,
                                                    string notificationTitle,
                                                    string notificationPrefix,
                                                    short notificationTagLength,
                                                    short institution_code,
                                                    short brand_code,
                                                    List<SmartFinance.Transaction_Groups> transaction_groupsFound,
                                                    List<SmartFinance.Transaction_Types> transaction_typesFound,
                                                    string Parameter1,
                                                    string Parameter2,
                                                    string Parameter3,
                                                    string sortCode,
                                                    string accountNo,
                                                    char categoryCode,
                                                    string symbol,
                                                    short loginMethod)
        {
#endif
                if (!args.IsSuccess)
                {
                    // Perform custom logic (e.g. log, callback)
                    await Task.CompletedTask;
                }
                try
                {
                    switch (step)
                    {
                        case 0:
                            await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Status: Step 0");
                            string loginHrefJson = await financeviewmodel.FinanceWebView.ExecuteScriptAsync(@"
                            (function() {
                                const el = Array.from(document.querySelectorAll('a')).find(a => a.href.includes('AccessManagement'));
                                return el ? el.href : null;
                            })();");

                            string loginHref = System.Text.Json.JsonSerializer.Deserialize<string>(loginHrefJson);
                            if (string.IsNullOrEmpty(loginHref))
                            {
                                financeviewmodel.errorMessage = "Status: href could not be found";
                                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, financeviewmodel.errorMessage);
                                await Task.CompletedTask;
                            }
                            await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Status: Found href");
                            step++;
                            financeviewmodel.FinanceWebView.CoreWebView2.Navigate(loginHref);
                            break;
                        case 1:
                            await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Status: Step 1");

                            // Insert the Modal Sniffer One
                            await financeviewmodel.FinanceWebView.ExecuteScriptAsync(@"
                            (function() {
                            const observer = new MutationObserver(mutations => {
                            const modal = document.querySelector(
                                '.modal.show, .popup, .dialog, .overlay, [role=dialog], [aria-modal=true]'
                            );

                                if (modal)
                                {
                                    if (window.chrome && window.chrome.webview)
                                    {
                                        window.chrome.webview.postMessage({ status: 'success_modalsniffer1', text: 'Modal appeared' }); 
                                    }
                                    // Stop observing after the first match
                                    observer.disconnect();                                    
                                }
                            });

                            if (document.body)
                            {
                                observer.observe(document.body, { childList: true, subtree: true });
                                
                            }
                            else
                            {
                                console.warn('[ModalSniffer] document.body not ready');
                            }
                            })();");

                            await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Status: Modal sniffer inserted");
                            step++;
                            break;
                        case 2:
                            await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Status: Step 2");

                            string script = @"
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
                            try
                            {
                                string resultRadio = await financeviewmodel.FinanceWebView.ExecuteScriptAsync(script);
                                if (resultRadio.Contains("Clicked"))
                                {
                                    await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Step 2: Radio button clicked");
                                }
                                else
                                {
                                    financeviewmodel.errorMessage = "Step2: Radio button not found";
                                    await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, financeviewmodel.errorMessage);
                                    await Task.CompletedTask;   // Give up
                                }
                            }
                            catch (Exception ex)
                            {
                                financeviewmodel.errorMessage = "Failure: Select radio button " + ex.Message;
                                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, financeviewmodel.errorMessage);
                                await Task.CompletedTask;
                            }

                            // Step 1: Get spans with class 'control__label__title'
                            string scriptPasscode = @"
                            (() => {
                                const spans = Array.from(document.querySelectorAll('span.control__label__title'));
                                const result = [];
                                for (let span of spans) {
                                    if (span.innerText.includes('digits from your passnumber')) {
                                        result.push(span.innerText);
                                    }
                                }
                                return result;
                            })()";
                            try
                            {
                                var resultJson = await financeviewmodel.FinanceWebView.ExecuteScriptAsync(scriptPasscode);
                                var spanTexts = System.Text.Json.JsonSerializer.Deserialize<List<string>>(resultJson);

                                if (spanTexts == null || spanTexts.Count == 0)
                                {
                                    financeviewmodel.errorMessage = "Step2: Couldn't find 'passcodes' span class";
                                    await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, financeviewmodel.errorMessage);
                                    await Task.CompletedTask;   // Give up
                                }

                                string text = spanTexts[0]; // Use the first match
                                int index = 0;
                                int[] zzz = new int[3];

                                if (text.Contains("1st")) zzz[index++] = 0;
                                if (text.Contains("2nd")) zzz[index++] = 1;
                                if (text.Contains("3rd")) zzz[index++] = 2;
                                if (text.Contains("4th")) zzz[index++] = 3;
                                if (text.Contains("5th")) zzz[index++] = 4;
                                if (text.Contains("6th")) zzz[index++] = 5;


                                string[] dropdownNames = { "FirstPassnumberValue", "SecondPassnumberValue", "ThirdPassnumberValue" };

                                for (int j = 0; j < 3; j++)
                                {
                                    int digitIndex = zzz[j];
                                    string digit = Parameter3.Substring(digitIndex, 1);
                                    string selectScript = $@"
                                    (() => {{
                                        let select = document.querySelector('select[name=""{dropdownNames[j]}""]');
                                        if (!select) return 'Missing';
                                        for (let option of select.options) {{
                                            if (option.label === '{digit}') {{
                                                select.value = option.value;
                                                select.dispatchEvent(new Event('change'));
                                                return 'Success';
                                            }}
                                        }}
                                        return 'NotFound';
                                    }})()";
                                    string selectionResult = await financeviewmodel.FinanceWebView.ExecuteScriptAsync(selectScript);
                                    if (selectionResult.Contains("Missing"))
                                    {
                                        financeviewmodel.errorMessage = $"Couldn't find '{dropdownNames[j]}' drop-down input";
                                        await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, financeviewmodel.errorMessage);
                                        await Task.CompletedTask;
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                financeviewmodel.errorMessage = "Step 2: Insert Passcode(s) failure " + ex.Message;
                                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, financeviewmodel.errorMessage);
                                await Task.CompletedTask;
                            }

                            await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Step 2: Passcodes set");

                            SmartBanksV2023.SetupOtpListener(
#if WINFORMS
                                                components,
                                                textBoxConsole,
#endif
                                                tcs,
                                                ourviewmodel,
                                                financeviewmodel,
                                                loginInfo,
                                                notificationTitle,
                                                notificationPrefix,
                                                notificationTagLength,
                                                institution_code,
                                                brand_code,
                                                transaction_groupsFound,
                                                transaction_typesFound);
                            await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Step 2: OTP Listener set");
                            // Not an async so strings should return
                            string scriptContinue2 = @"
                            (function() {
                                function getSecondContinueButton() {
                                    const buttons = Array.from(document.querySelectorAll('button'))
                                        .filter(btn => btn.innerText.trim() === 'Continue');
                                    return buttons.length >= 1 ? buttons[0] : null;
                                }

                                function waitForButtonAndClick() {
                                    return new Promise((resolve, reject) => {
                                        const interval = setInterval(() => {
                                            const button = getSecondContinueButton();
                                            if (button) {
                                                clearInterval(interval);
                                                button.scrollIntoView({ behavior: 'smooth', block: 'center' });
                                                button.click();
                                                resolve('Clicked');
                                            }
                                        }, 500);

                                        // Timeout after 15 seconds
                                        setTimeout(() => {
                                            clearInterval(interval);
                                            reject('Not found');
                                        }, 15000);
                                    });
                                }

                                return waitForButtonAndClick();
                            })();";

                            try
                            {
                                // I think you are getting a Promise back here
                                // So don't try to deserialize it
                                string resultContinue2 = await financeviewmodel.FinanceWebView.CoreWebView2.ExecuteScriptAsync(scriptContinue2);
                                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Continue button(2): Clicked");
                            }
                            catch (Exception ex)
                            {
                                financeviewmodel.errorMessage = "Step 2: Continue(2) " + ex.Message;
                                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, financeviewmodel.errorMessage);
                                await Task.CompletedTask;
                            }
                            step++;
                            break;
                        case 3:
                            await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Step 3: Cancelling timer");

                            if (!financeviewmodel.loggedIn)
                            {
                                // Task completed set in here
                                await SmartBanksV2023.WebDriverDead(tcs, ourviewmodel, financeviewmodel);
                                await Task.CompletedTask;
                            }
                            string key = @"Timer_" + notificationTitle;
                            if (SmartBanksV2023.IsRunning(financeviewmodel, key))
                            {
                                SmartBanksV2023.CancelTimer(financeviewmodel, key);
                                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Status : Timer " + key + " cancelled");

                            }

                            try
                            {
                                financeviewmodel.accountItems = new List<FinanceViewModel.AccountItem>();
                                financeviewmodel.currentUrl = financeviewmodel.FinanceWebView.Source.ToString() ?? "";

                                string getWelcomeScript = @"
                                (function() {
                                    const welcomeElem = document.getElementById('welcome-message');
                                    return welcomeElem && welcomeElem.innerText ? welcomeElem.innerText : '';
                                })();";

                                string welcomeTextRaw = await financeviewmodel.FinanceWebView.ExecuteScriptAsync(getWelcomeScript);
                                string welcomeText = System.Text.Json.JsonDocument.Parse(welcomeTextRaw).RootElement.GetString() ?? "";
                                owner = welcomeText.Replace("Welcome back,", "").Trim();

                                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Owner: " + owner);

                                step = 100;

                                //financeviewmodel.UDPRN = "";

                                string hrefMaintains = "MaintainTelephoneAndAddress";
                                string maintainHref = hrefMaintains.Replace("'", "\\'");
                                string scriptMaintain = $@"
                                (() => {{
                                    const link = Array.from(document.querySelectorAll('a[href]')).find(a => a.href.includes('{maintainHref}'));
                                    if (link) {{
                                        link.click();
                                        return true;
                                    }} else {{
                                        return false;
                                    }}
                                }})()";
                                // Navigate to the href and wait for navigation complete event

                                string resultMaintain = await financeviewmodel.FinanceWebView.ExecuteScriptAsync(scriptMaintain);
                                // Now we should be going to 100 !!
                                // ...and we do!
                            }
                            catch (Exception ex)
                            {
                                financeviewmodel.errorMessage = "Exception occurred: " + ex.Message;
                                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, financeviewmodel.errorMessage);
                                await Task.CompletedTask;
                            }
                            break;
                        case 100:
                            // Find Address
                            await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Step 100: Finding details");

                            await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Contact no.: " + loginInfo.CONTACT_PHONENO);
                            await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Email address: " + loginInfo.CONTACT_EMAIL);

                            // Call FindDetails - you must have a WebView2 version of this method
                            UDPRN = await FindDetailsAsync(ourviewmodel,
                                                                            financeviewmodel,
                                                                            loginInfo,
                                                                            owner);

                            if (string.IsNullOrEmpty(UDPRN))
                            {
                                financeviewmodel.errorMessage = "UDPRN is empty";
                                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, financeviewmodel.errorMessage);
                                await Task.CompletedTask;
                            }
                            await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "UDPRN good: " + UDPRN);

                            financeviewmodel.account_id = 0;

                            step = 200;

                            string hrefContains = "AccountList/AccountList";
                            string escapedHref = hrefContains.Replace("'", "\\'");

                            // Navigate to the href and wait for navigation complete event
                            string scriptAccountList = $@"
                            (() => {{
                                const link = Array.from(document.querySelectorAll('a[href]')).find(a => a.href.includes('{escapedHref}'));
                                if (link) {{
                                    link.click();
                                    return true;
                                }} else {{
                                    return false;
                                }}
                            }})()";
                            // Navigate to the href and wait for navigation complete event

                            string result = await financeviewmodel.FinanceWebView.ExecuteScriptAsync(scriptAccountList);
                            // we should be going to 200 now!

                            break;
                        case 200:
                            //UDPRN = "227273";
                            // Find Accounts List - you only ever hit 200 ONCE
                            await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Step 200: Finding accounts");

                            // Run JavaScript to find the links under element with class 'active' with href containing "/AccountList/Account/RedirectToDefaultPage"
                            // No async so can process the returned string
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

                            string jsonResult = await financeviewmodel.FinanceWebView.ExecuteScriptAsync(scriptList);


                            // Step 1: Trim leading/trailing quotes from JS result because we have a string not an array
                            if (jsonResult.StartsWith("\"") && jsonResult.EndsWith("\""))
                            {
                                jsonResult = jsonResult.Substring(1, jsonResult.Length - 2);
                            }
                            // Step 2: Unescape the JSON string
                            string unescapedJson = System.Text.RegularExpressions.Regex.Unescape(jsonResult);

                            // Step 3: Deserialize JSON string to a list of link info objects
                            AccountLinks = JsonSerializer.Deserialize<List<AccountLink>>(unescapedJson);

                            if (AccountLinks == null || AccountLinks.Count == 0)
                            {
                                // Not an error message as such, this might well be the case
                                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "No accounts found...");
                            }
                            else
                            {
                                // ✅ Store list and setup for account-by-account processing
                                CurrentAccountIndex = 0;
                                IsProcessingAccounts = true;
                                string SortCode = "";
                                string AccountNo = "";
                                char CategoryCode = SmartParametersV2016.defaultChar;
                                string Symbol = "";

#if WINFORMS || WPF
                                // ✅ Remove the current handler (you’ll need to name it)
                                financeviewmodel.FinanceWebView.CoreWebView2.NavigationCompleted -= InitialHandler;

                                // ✅ Add the new processing handler
                                financeviewmodel.AccountNavHandler = CreateAccountHandler(
#if WINFORMS
                                                                                        components,
                                                                                        textBoxConsole,
#endif
                                                                                        ourviewmodel, 
                                                                                        financeviewmodel, 
                                                                                        SortCode, 
                                                                                        AccountNo, 
                                                                                        CategoryCode, 
                                                                                        Symbol);
                                financeviewmodel.FinanceWebView.CoreWebView2.NavigationCompleted += financeviewmodel.AccountNavHandler;
#endif
#if WINUI
                                // ✅ Remove the current handler (you’ll need to name it)
                                if (financeviewmodel.FinanceWebView.CoreWebView2 != null && 
                                    financeviewmodel.AccountNavHandler != null)
                                {
                                    financeviewmodel.FinanceWebView.NavigationCompleted -= InitialHandler;
                                }

                                // ✅ Add the new processing handler
                                financeviewmodel.AccountNavHandler = CreateAccountHandler(ourviewmodel, financeviewmodel, SortCode, AccountNo, CategoryCode, Symbol);
                                financeviewmodel.FinanceWebView.NavigationCompleted += financeviewmodel.AccountNavHandler;
#endif
                                // ✅ Start first account processing
                                await ProcessNextAccountAsync(
#if WINFORMS
                                                            components,
                                                            textBoxConsole,
#endif
                                                            ourviewmodel,
                                                            financeviewmodel,
                                                            UDPRN,
                                                            SortCode,
                                                            AccountNo);
                            }
                            break;
                        default:
                            break;
                    }
                }
                catch (Exception ex)
                {
                    financeviewmodel.errorMessage = $"Step {step} failed: " + ex.Message;
                    await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, financeviewmodel.errorMessage);
                    await Task.CompletedTask;
                }
#if WINFORMS || WPF || UWP
            };
#endif
        }

        internal static async void CoreWebView2_WebMessageReceived(object sender,
                                                        CoreWebView2WebMessageReceivedEventArgs args,
#if WINFORMS
                                                        SmartDashboard.MainProcess  components,
                                                        RichTextBox textBoxConsole,
#endif
                                                        TaskCompletionSource<bool> tcs,
                                                        MainViewModel ourviewmodel,
                                                        FinanceViewModel financeviewmodel,
                                                        SmartFinance.Logins loginInfo,
                                                        string notificationTitle,
                                                        string notificationPrefix,
                                                        short notificationTagLength,
                                                        short institution_code,
                                                        short brand_code,
                                                        List<SmartFinance.Transaction_Groups> transaction_groupsFound,
                                                        List<SmartFinance.Transaction_Types> transaction_typesFound,
                                                        string Parameter1,
                                                        string Parameter2)
        {
            string SortCode = "";
            string AccountNo = "";
            try
            {
                if (args.WebMessageAsJson == null)
                {
                    await Task.CompletedTask;
                }
                string messageJson = args.WebMessageAsJson;
                WebMessage message1 = JsonSerializer.Deserialize<WebMessage>(messageJson);

                //await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "WMStatus: " + message1.status);

                switch (message1.status)
                {
                    case "elements_found":
                        // Process each Account
                        //await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Finding transactions: " + message1.text.Replace(SmartParametersV2016.fieldSeparator.ToString(), " "));

                        string oneYearAgo = DateTime.Now.AddYears(-1).ToString("dd/MM/yyyy"); // ISO format for input[type='date']

                        string message1Json = JsonSerializer.Serialize(message1.text);
                        string script = $@"
                        (() => {{
                            try {{
                                const message1Json = {message1Json}; // Injected safely from C#

                                const revealLink = document.getElementById('enter-date-reveal-link');
                                const inputSelector = '#statement-from-date';
                                const buttonSelector = '#date-filter-update';
                                const tbodySelector = 'tbody';

                                // Click the reveal link if it exists
                                if (revealLink) {{
                                    revealLink.click();
                                }}

                                setTimeout(() => {{
                                    const input = document.querySelector(inputSelector);
                                    const button = document.querySelector(buttonSelector);

                                    if (!input || !button) {{
                                        window.chrome.webview.postMessage({{ status: 'missing_inputs' }});
                                        return;
                                    }}

                                    input.value = '{oneYearAgo}';
                                    const event = new Event('change', {{ bubbles: true }});
                                    input.dispatchEvent(event);
                                    button.click();

                                    let sent = false;
                                    let retries = 50;
                                    let previousHTML = document.querySelector(tbodySelector)?.innerHTML || '';

                                    function notify(status) {{
                                        if (sent) return;
                                        sent = true;
                                        if (status === 'table_updated') {{
                                            window.chrome.webview.postMessage({{ status, text: message1Json }});
                                        }} else {{
                                            window.chrome.webview.postMessage({{ status }});
                                        }}
                                    }}

                                    const poller = setInterval(() => {{
                                        const tbody = document.querySelector(tbodySelector);
                                        if (tbody && tbody.innerHTML !== previousHTML && tbody.children.length > 0) {{
                                            notify('table_updated');
                                            clearInterval(poller);
                                        }} else if (--retries <= 0) {{
                                            notify('table_timeout');
                                            clearInterval(poller);
                                        }}
                                    }}, 200);
                                }}, 300);
                            }} catch (err) {{
                                //console.error('Error in script:', err);
                                window.chrome.webview.postMessage({{ status: 'script_error', message: err.toString() }});
                            }}
                        }})();";
                        await financeviewmodel.FinanceWebView.ExecuteScriptAsync(script);
                        break;
                    case "table_updated":
                        string[] codes = message1.text.Split(SmartParametersV2016.fieldSeparator);
                        SortCode = codes[0];
                        AccountNo = codes[1];

                        string symbol = "";
                        char categoryCode = SmartParametersV2016.defaultChar;

                        string[] transactions = new string[0];
                        // // Wait for table body to be updated – use a polling loop with timeout
                        string tableScript = @"
                                (function(){
                                    let tbody = document.querySelector('tbody');
                                    return tbody ? tbody.innerText : '';
                                })();";
                        string tableResult = await financeviewmodel.FinanceWebView.ExecuteScriptAsync(tableScript);
                        if (tableResult.StartsWith("\"") && tableResult.EndsWith("\""))
                        {
                            tableResult = tableResult.Substring(1, tableResult.Length - 2);
                        }
                        tableResult = Regex.Unescape(tableResult);
                        transactions = tableResult.Split('\n');
                        if (transactions.Length == 0)
                        {
                            // Not a killer as such -
                            await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "No transactions found ...");
                        }
                        else
                        {
#if WPF || UWP || WINUI
                            await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Transactions found: " + transactions.Length);
#endif
                            DateTime accountCreated = SmartParametersV2016.defaultDate;
                            string UDPRN = "221723";
                            List<SmartFinance.Accounts> accounts_found = financeviewmodel.PLO.finance_accountsList
                                .Where(a => a.INSTITUTION_CODE == institution_code &&
                                            a.BRAND_CODE == brand_code &&
                                            a.SORTCODE == SortCode &&
                                            a.ACCOUNT_NO == AccountNo &&
                                            a.UDPRN == UDPRN)
                                .OrderByDescending(a => a.ACCOUNT_CREATED)
                                .ToList();

                            if (accounts_found.Count > 0)
                            {
                                accountCreated = accounts_found.First().ACCOUNT_CREATED;
                            }
                            financeviewmodel.sequence_no = SmartFinanceV2025.UnixSequenceNo();

                            if (!await NationwideTransactions(
#if WINFORMS
                                                        components,
                                                        textBoxConsole,
#endif
                                                    ourviewmodel,
                                                    financeviewmodel,
                                                    categoryCode,
                                                    transactions,
                                                    institution_code,
                                                    brand_code,
                                                    SortCode,
                                                    AccountNo,
                                                    UDPRN,
                                                    symbol,
                                                    transaction_groupsFound,
                                                    transaction_typesFound,
                                                    accountCreated))
                            {
                                financeviewmodel.errorMessage = "Transactions failed: " + financeviewmodel.errorMessage;
                                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, financeviewmodel.errorMessage);
                                await Task.CompletedTask;
                            }
                            else
                            {
                                // Continue to next account
                                CurrentAccountIndex++;
                                await ProcessNextAccountAsync(
#if WINFORMS
                                                            components,
                                                            textBoxConsole,
#endif
                                                            ourviewmodel,
                                                            financeviewmodel,
                                                            UDPRN,
                                                            SortCode,
                                                            AccountNo);
                            }
                        }
                        break;
                    case "success_modalsniffer1":
                        // Before we tell the Modal we want an OTP,
                        // setup the Notification shit
                        await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "WMStatus: Modal detected");
                        string CustomerNumber = Parameter1;
                        string Day = Parameter2.Substring(0, 2);
                        string Month = SmartParametersV2016.months[int.Parse(Parameter2.Substring(2, 2)) - 1];
                        string Year = Parameter2.Substring(4, 4);

                        await financeviewmodel.FinanceWebView.ExecuteScriptAsync($"document.querySelector('input[name=\"CustomerNumber\"]').value = '{CustomerNumber}';");
                        await financeviewmodel.FinanceWebView.ExecuteScriptAsync($"document.querySelector('input[name=\"DateOfBirthDay\"]').value = '{Day}';");
                        await financeviewmodel.FinanceWebView.ExecuteScriptAsync($"document.querySelector('input[name=\"DateOfBirthYear\"]').value = '{Year}';");

                        await financeviewmodel.FinanceWebView.ExecuteScriptAsync(@"
                       (function() {
                            const select = document.querySelector('select[name=""DateOfBirthMonth""]');
                            if (select) {
                                select.value = 'TargetMonth';  // or whatever month you want
                                // Dispatch change event so any JS listeners notice
                                const event = new Event('change', { bubbles: true });
                                select.dispatchEvent(event);
                            } else {
                                console.log('Month dropdown not found');
                            }
                        })();
                        ".Replace("TargetMonth", Month));


                        await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Inputs filled in");

                        // Async here so the function WON'T return a string
                        // So don't try and deserialize one!
                        string scriptContinue1 = @"
                        (async function () {
                            function sleep(ms) {
                                return new Promise(resolve => setTimeout(resolve, ms));
                            }
                            function isVisibleAndEnabled(el) {
                                const rect = el.getBoundingClientRect();
                                return (
                                    el.offsetParent !== null &&
                                    rect.width > 0 &&
                                    rect.height > 0 &&
                                    !el.disabled
                                );
                            }
                            async function waitForButton(timeout = 10000) {
                                const interval = 200;
                                let elapsed = 0;

                                while (elapsed < timeout) {
                                    const buttons = Array.from(document.querySelectorAll('button.action__button'));
                                    const button = buttons.find(b => b.textContent.trim().toLowerCase() === 'continue');

                                    if (button && isVisibleAndEnabled(button)) {
                                        return button;
                                    }
                                    await sleep(interval);
                                    elapsed += interval;
                                }
                                return null;
                            }
                            const btn = await waitForButton();
                            if (btn) {
                                btn.scrollIntoView({ behavior: 'smooth', block: 'center' });
                                btn.click();
                                //window.chrome.webview.postMessage({ status: 'success', text: 'Clicked continue button' });
                            } else {
                                //window.chrome.webview.postMessage({ status: 'error', text: 'Button not found' });
                            }
                        })();";

                        try
                        {
                            string result = await financeviewmodel.FinanceWebView.CoreWebView2.ExecuteScriptAsync(scriptContinue1);
                            await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Continue button(1): requested");
                        }
                        catch (Exception ex)
                        {
                            financeviewmodel.errorMessage = "Script error 3: " + ex.Message;
                            await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, financeviewmodel.errorMessage);
                            await Task.CompletedTask;
                        }
                        // Should end up at Step 2 hopefully
                        // to choose Passcode option and enter it
                        break;

                    case "success_passcode":
                        string oneTimePasscode = message1.text;
                        // No async so I can accept return string
                        string setOtpScript = $@"
                        (function() {{
                            const input = document.querySelector('input[name=""OneTimePasscode""]');
                            if(input) {{
                                input.value = '{oneTimePasscode}';
                                return true;
                            }} else {{
                                return false;
                            }}
                        }})();";

                        try
                        {
                            string setOtpResult = await financeviewmodel.FinanceWebView.ExecuteScriptAsync(setOtpScript);
                            if (setOtpResult == "true")
                            {
                                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "WM success: OTP filled");
                            }
                            else
                            {
                                financeviewmodel.errorMessage = "WM failed OTP: " + setOtpResult;
                                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, financeviewmodel.errorMessage);
                                await Task.CompletedTask;
                            }
                        }
                        catch (Exception ex)
                        {
                            financeviewmodel.errorMessage = "WM failed OTP: " + ex.Message;
                            await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, financeviewmodel.errorMessage);
                            await Task.CompletedTask;
                        }
                        // Assume the worst ... but hope for the best!
                        financeviewmodel.loggedIn = false;

                        // Async so we cannot deserialize the returned string
                        string waitForLoginAndClick = @"
                        (async function () {
                            function sleep(ms) {
                                return new Promise(resolve => setTimeout(resolve, ms));
                            }

                            async function waitForButton(label, timeout = 10000) {
                                const interval = 200;
                                let elapsed = 0;

                                while (elapsed < timeout) {
                                    const buttons = Array.from(document.getElementsByTagName('button'));
                                    const loginBtn = buttons.find(b => b.innerText.trim().toLowerCase() === label);
                                    if (loginBtn) {
                                        loginBtn.click();
                                        return true;
                                    }
                                    await sleep(interval);
                                    elapsed += interval;
                                }
                                return false;
                            }
                            return await waitForButton('log in');
                        })();";
                        try
                        {
                            string resultClick = await financeviewmodel.FinanceWebView.ExecuteScriptAsync(waitForLoginAndClick);
                            // Oooh its an async so no result decoding!
                            await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Log in: " + "Attempted");
                        }
                        catch (Exception ex)
                        {
                            financeviewmodel.errorMessage = "WM failed Login button: " + ex.Message;
                            await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, financeviewmodel.errorMessage);
                            await Task.CompletedTask;
                        }
                        financeviewmodel.loggedIn = true;   // Should be!
                        // And we should end up at Step 3? 
                        break;
                    case "timeout_fallback":
                        // Not a killer - something didn't happen in time
                        // Continue to next account
                        CurrentAccountIndex++;
                        await ProcessNextAccountAsync(

#if WINFORMS
                                                    components,
                                                    textBoxConsole,
#endif
                                                    ourviewmodel,
                                                    financeviewmodel,
                                                    UDPRN,
                                                    SortCode,
                                                    AccountNo);
                        break;
                    default:
                        await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Failure: " + message1.status);
                        break;
                }
            }
            catch (Exception ex)
            {
                financeviewmodel.errorMessage = "JS message: " + ex.Message;
                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, financeviewmodel.errorMessage);
                await Task.CompletedTask;
            }
            return;
        }

        internal static async Task<bool> LogoutAsync(
#if WINFORMS
                                                    SmartDashboard.MainProcess components,
                                                    RichTextBox textBoxConsole,
#endif
                                                    MainViewModel ourviewmodel,
                                                    FinanceViewModel financeviewmodel)
        {
            // No async so we can process the strings returned
            const string scriptLogout = @"
            (function() {
                const anchors = Array.from(document.querySelectorAll('a'));
                const logoutLink = anchors.find(el =>
                    el.textContent.trim().toLowerCase() === 'log out');
                if (logoutLink) {
                    logoutLink.click();
                    return true;
                } else {
                    return false;
                }
            })();";
            try
            {
                string resultLogout = await financeviewmodel.FinanceWebView.ExecuteScriptAsync(scriptLogout);
                // Logout
                financeviewmodel.loggedIn = false;
                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Logout : " + resultLogout);
                // Completion gets set in WebDead routine
            }
            catch (Exception ex)
            {
                financeviewmodel.errorMessage = "Logout failed: " + ex.Message;
                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, financeviewmodel.errorMessage);
                await Task.CompletedTask;
            }
            return true;
        }

        // Internal??
#if WINUI
        private static TypedEventHandler<WebView2, CoreWebView2NavigationCompletedEventArgs> CreateAccountHandler(MainViewModel ourviewmodel,
                                                                                            FinanceViewModel financeviewmodel,
                                                                                            string sortcode, 
                                                                                            string account_no,
                                                                                            char categoryCode,
                                                                                            string symbol)
        {
            return async (WebView2 sender, CoreWebView2NavigationCompletedEventArgs args) =>
            {
#endif

#if WINFORMS || WPF || UWP
        private static EventHandler<CoreWebView2NavigationCompletedEventArgs> CreateAccountHandler(
#if WINFORMS
                                                                            SmartDashboard.MainProcess components,
                                                                            RichTextBox textBoxConsole,

#endif
                                                                            MainViewModel ourviewmodel,
                                                                            FinanceViewModel financeviewmodel,
                                                                            string sortcode,
                                                                            string account_no,
                                                                            char categoryCode,
                                                                            string symbol)
        {
            return async (sender, e) =>
            {
#endif
                try
                {
                    string message1Json = JsonSerializer.Serialize(sortcode + SmartParametersV2016.fieldSeparator + account_no);

                    string scriptElements = $@"
                    (() => {{
                        try {{
                            const message1 = {message1Json};
                            const selectors = ['#statement-from-date', '#date-filter-update', 'tbody'];
                            let sent = false;

                            function allElementsExist() {{
                                return selectors.every(sel => document.querySelector(sel));
                            }}

                            function notify(status) {{
                                if (sent) return;
                                sent = true;
                                if (window.chrome && window.chrome.webview) {{
                                    window.chrome.webview.postMessage({{ status, text: message1 }});
                                }}
                            }}

                            let retries = 50;
                            const poller = setInterval(() => {{
                                if (allElementsExist()) {{
                                    notify('elements_found');
                                    clearInterval(poller);
                                }} else if (--retries <= 0) {{
                                    notify('timeout_fallback');
                                    clearInterval(poller);
                                }}
                            }}, 100);

                            return true;
                        }} catch (err) {{
                            console.error('Error in polling script:', err);
                            if (window.chrome && window.chrome.webview) {{
                                window.chrome.webview.postMessage({{
                                    status: 'script_error',
                                    text: message1,
                                    message: err.toString()
                                }});
                            }}
                            return false;
                        }}
                    }})();";

                    string resultElements = await financeviewmodel.FinanceWebView.ExecuteScriptAsync(scriptElements);
                    if (resultElements == "false")
                    {
                        await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Elements: " + resultElements);
                    }
                }
                catch (Exception ex)
                {
                    financeviewmodel.errorMessage = ex.Message;
                    await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "AccountsHandler: " + ex.Message);
                }
            };
        }

        internal static async Task ProcessNextAccountAsync(
#if WINFORMS
                                                            SmartDashboard.MainProcess components,
                                                            RichTextBox textBoxConsole,
#endif
                                                            MainViewModel ourviewmodel, 
                                                            FinanceViewModel financeviewmodel, 
                                                            string UDPRN,
                                                            string SortCode, 
                                                            string AccountNo)
        {
            //await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "In ProcessNext: " + CurrentAccountIndex.ToString());

            if (!IsProcessingAccounts || 
                AccountLinks.Count == 0 || 
                CurrentAccountIndex >= AccountLinks.Count)
            {
                IsProcessingAccounts = false;
#if WINFORMS || WPF
                financeviewmodel.FinanceWebView.CoreWebView2.NavigationCompleted -= financeviewmodel.AccountNavHandler;
#endif
#if WINUI
                financeviewmodel.FinanceWebView.NavigationCompleted -= financeviewmodel.AccountNavHandler;
#endif
                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Finished processing accounts.");

                await LogoutAsync(
#if WINFORMS
                                    components,
                                    textBoxConsole,
#endif
                                    ourviewmodel,
                                    financeviewmodel);
                return;
            }

            AccountLink account = AccountLinks[CurrentAccountIndex];

            string accountInfo1 = "", balance = "", symbol = "", accountName = "", accountInfo2 = "";
            string sortcode = "", account_no = "";
            char categoryCode = SmartParametersV2016.defaultChar;

            NewAccountBalance(financeviewmodel, account.innerHTML, ref accountInfo1, ref balance, ref symbol);
            if (string.IsNullOrEmpty(accountInfo1))
            {
                financeviewmodel.errorMessage = "Couldn't parse account info: " + account.innerHTML;
                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, financeviewmodel.errorMessage);
                CurrentAccountIndex++;
                await ProcessNextAccountAsync(
#if WINFORMS
                                            components,
                                            textBoxConsole,
#endif
                                            ourviewmodel,
                                            financeviewmodel,
                                            UDPRN,
                                            SortCode,
                                            AccountNo);
                return;
            }

            accountInfo2 = SplitAccountName(financeviewmodel, accountInfo1, ref accountName);
            if (string.IsNullOrEmpty(accountName))
            {
                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Account name empty");
            }
            if (string.IsNullOrEmpty(accountInfo2))
            {
                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Account sortcode/account empty");
            }
            short currency_ordinal = SmartSpikeV2017.GetCurrencyOrdinal(ourviewmodel, symbol);

            categoryCode = SmartParametersV2016.Banks;
            if (accountName.Contains("Savings", StringComparison.OrdinalIgnoreCase))
                categoryCode = SmartParametersV2016.Savings;

            string[] accstuff = accountInfo2.Split(SmartParametersV2016.spacechar);
            if (accstuff.Length > 1)
            {
                sortcode = accstuff[0].Replace("-", "");
                account_no = accstuff[1];
            }
            else
            {
                account_no = accstuff[0];
            }

            bool decoded = await DecodeAccount(ourviewmodel, financeviewmodel, accountName, sortcode, account_no,
                                               currency_ordinal, 
                                               1001, //institution_code, 
                                               1001, //brand_code, 
                                               UDPRN, categoryCode);

            if (!decoded)
            {
                financeviewmodel.errorMessage = "Cannot decode account";
                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, financeviewmodel.errorMessage);
            }

            await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, $"Processing: {sortcode} {account_no}");

            // ✅ Click link to start navigation
            string accountHref = account.href.Replace("'", "\\'");
            string scriptClick = $@"
            (() => {{
                const link = Array.from(document.querySelectorAll('a[href]')).find(a => a.href.includes('{accountHref}'));
                if (link) {{
                    link.click();
                    return true;
                }}
                return false;
            }})()";

            string resultClick = await financeviewmodel.FinanceWebView.ExecuteScriptAsync(scriptClick);
            if (resultClick == "false")
            {
                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Link not found.");
                await Task.CompletedTask;                
            }            
            return;
        }

        

        internal static async Task<string> FindDetailsAsync(MainViewModel ourviewmodel,
                                                            FinanceViewModel financeviewmodel,
                                                            SmartFinance.Logins loginInfo,
                                                            string owner)
        {
            string udprn = "";
            try
            {
                // Step 1: Find the address text inside <address> tags
                // No async so can deal with the returned string
                string getAddressScript = @"
                (function() {
                    let addresses = document.getElementsByTagName('address');
                    if (addresses.length === 0) return '';
                    let addrText = addresses[0].innerText || '';
                    addrText = addrText.replace(/\r?\n/g, ' ').trim();
                    addrText = addrText.replace(/\s{2,}/g, ' ');
                    return addrText;
                })();";

                string addressJson = await financeviewmodel.FinanceWebView.ExecuteScriptAsync(getAddressScript);
                string bankaddress = JsonSerializer.Deserialize<string>(addressJson);

                if (string.IsNullOrWhiteSpace(bankaddress))
                {
                    financeviewmodel.errorMessage = "Bank address empty - cannot continue";
                    await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, financeviewmodel.errorMessage);
                    return udprn;
                }

                // Step 2: Lookup existing address in your database or cache
                var addresses_found = SmartSpikeV2017.UsersLookupTextAddress(ourviewmodel, bankaddress);
                if (addresses_found.Count > 0)
                {
                    if (string.IsNullOrEmpty(addresses_found.First().UDPRN))
                    {
                        financeviewmodel.errorMessage = "Address record has no UDPRN - cannot continue";
                        await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, financeviewmodel.errorMessage);
                        return udprn;
                    }
                    udprn = addresses_found.First().UDPRN;
                }
                else
                {
                    // Step 5: New address - lookup via external service
                    SmartUsers.AddressesView ideal_address = await SmartRoutinesV2018.UsersUDPRNLookup(ourviewmodel, bankaddress);
                    if (string.IsNullOrEmpty(ideal_address.UDPRN))
                    {
                        financeviewmodel.errorMessage = "Cannot determine UDPRN - cannot continue";
                        await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, financeviewmodel.errorMessage);
                        return udprn;
                    }
                    udprn = ideal_address.UDPRN;
                    //
                    // Add new address record logic here (similar to your original code) ...
                    // ...
                }
                // Step 7: Extract email and phone from page via JS
                string getEmailScript = @"
                (function() {
                    let el = document.getElementById('CurrentEmailAddress');
                    return el ? el.value : '';
                })();";
                string emailJson = await financeviewmodel.FinanceWebView.ExecuteScriptAsync(getEmailScript);
                string emailValue = System.Text.Json.JsonSerializer.Deserialize<string>(emailJson);

                if (loginInfo.CONTACT_EMAIL != emailValue)
                {
                    loginInfo.CONTACT_EMAIL = emailValue;
                    loginInfo.Updated = true;
                }

                string getPhoneScript = @"
                (function() {
                    let el = document.getElementById('MobilePhoneNumber');
                    return el ? el.value : '';
                })();";
                string phoneJson = await financeviewmodel.FinanceWebView.ExecuteScriptAsync(getPhoneScript);
                string phoneValue = System.Text.Json.JsonSerializer.Deserialize<string>(phoneJson);

                if (loginInfo.CONTACT_PHONENO != phoneValue)
                {
                    loginInfo.CONTACT_PHONENO = phoneValue;
                    loginInfo.Updated = true;
                }
            }
            catch (Exception ex)
            {
                financeviewmodel.errorMessage = ex.Message;
                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, financeviewmodel.errorMessage);
                return udprn;
            }

            // Update owner if changed
            if (owner != loginInfo.OWNER)
            {
                loginInfo.OWNER = owner;
                loginInfo.Updated = true;
            }
            if (loginInfo.Updated)
            {
                financeviewmodel.PLO.finance_logins_changesList.Add(loginInfo);
            }
            return udprn;
        }

        internal static void NewAccountBalance(FinanceViewModel financeviewmodel,
                                                string innerHtml,
                                                ref string accountName,
                                                ref string balance,
                                                ref string symbol)
        {
            try
            {
                innerHtml = innerHtml.Replace("<br>", "|");
                string[] innertext = innerHtml.Split("|");
                if (innertext.Length > 0)
                {
                    accountName = innertext[0];
                    if (innertext.Length > 1)
                    {
                        balance = innertext[1];
                        bool negative = false;
                        if (balance.Length > 0)
                        {
                            if (balance.Substring(0, 1) == "-")
                            {
                                balance = balance.Replace("-", "");
                                negative = true;
                            }
                        }
                        if (balance.Length > 0)
                        {
                            symbol = balance.Substring(0, 1);
                            {
                                balance = balance.Replace(symbol, "");
                            }
                            if (negative)
                            {
                                balance = "-" + balance;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                financeviewmodel.errorMessage = ex.Message;
            }
            return;
        }

        internal static string SplitAccountName(FinanceViewModel financeviewmodel,
                                                string accountInfo,
                                                ref string accountName)
        {
            string ourid = "";
            string account_no;
            string sortcode = "";

            // Now ... split the text up.
            // assume the FIRST is an account name (or part of one)
            // assume the LAST is always the balance ... good luck!
            try
            {
                string[] items = accountInfo.Split(SmartParametersV2016.spaceSplit);
                int itemCount = items.Length;
                if (itemCount < 2)
                {
                    // Not enough items  what about "/" accounts?
                    return "";
                }
                int itemPosLow = 0;
                int itemPosHigh = itemCount - 1; // 2
                if (items[itemPosLow] == "")
                {
                    // Can't find account name
                    return "";
                }
                accountName = items[itemPosLow]; // FlexAccount
                itemPosLow++;  // 1
                // Now ... everything is betwwen these two
                if (itemPosLow >= itemPosHigh)  // 1 >= 2
                {
                    // Can't find sort code and or acc num
                    return "";
                }
                // Should always have ONE acc number
                account_no = items[itemPosHigh];    // 10722017
                itemPosHigh--;                      // 1
                // Now ... everything is between these two
                while (itemPosLow <= itemPosHigh)  // 1 < 1
                {
                    // Is first character a digit?
                    char abc = Convert.ToChar(items[itemPosLow][0]); // items[1] = 070116?
                    if (Char.IsDigit(abc))
                    {
                        sortcode = items[itemPosLow];
                    }
                    else
                    {
                        accountName += SmartParametersV2016.space + items[itemPosLow];
                    }
                    itemPosLow++;
                }
                ourid = sortcode +
                    SmartParametersV2016.space +
                    account_no;
            }
            catch (Exception ex)
            {
                financeviewmodel.errorMessage = ex.Message;
            }
            return ourid;
        }

        internal static async Task<bool> DecodeAccount(MainViewModel ourviewmodel,
                                            FinanceViewModel financeviewmodel,
                                            string accountName,
                                            string sortcode,
                                            string account_no,
                                            short accountOrdinal,
                                            short institutionCode,
                                            short brandCode,
                                            string udprn,
                                            char categoryCode)
        {
            // We pick up the Balance from the very last transaction item
            try
            {
                // 0 = sortcode
                // 1 = account_no
                // 2 = udprn
                // 3 = currency
                string vaalue = sortcode + SmartParametersV2016.fieldSeparator +
                                account_no + SmartParametersV2016.fieldSeparator +
                                udprn + SmartParametersV2016.fieldSeparator +
                                accountOrdinal;   // Assume we can find it in Banking
                vaalue = vaalue.Replace("&amp;", "&");                                                                           // Safety check
                string[] itemArray = vaalue.Split(SmartParametersV2016.fieldSeparator);

                financeviewmodel.account_id++;

                FinanceViewModel.AccountItem account_item = new FinanceViewModel.AccountItem
                {
                    AccountID = financeviewmodel.account_id,
                    Content = accountName,
                    // Here we have [0] = sort_code
                    //              [1] = account_no
                    //              [2] = udprn
                    //              [3] = account ordinal 
                    Value = string.Join(SmartParametersV2016.bar.ToString(), itemArray)
                };
                financeviewmodel.accountItems.Add(account_item);

                // We've found one, now do we need to add it to Accounts?
                List<SmartFinance.Accounts> accounts_found =
                    SmartSpikeFinanceV2017.Finance_Lookup_AccountsCategory(ourviewmodel,
                                                                    financeviewmodel,
                                                                    institutionCode,
                                                                    brandCode,
                                                                    categoryCode,
                                                                    sortcode,
                                                                    account_no,
                                                                    udprn,
                                                                    accountOrdinal);
                if (accounts_found.Count == 0)
                {
                    // Dummy these two up
                    int balance = 0;    // For now
                    char status = SmartParametersV2016.AccountStatus;
                    DateTime accountCreated = DateTime.UtcNow;  // UTC time
                    // Its not in the Accounts List so add it in
                    SmartFinance.Accounts account_record =
                        SmartFinanceV2025.Account_Template(ourviewmodel,
                                                            financeviewmodel,
                                                            institutionCode,
                                                            brandCode,
                                                            sortcode,
                                                            account_no,
                                                            udprn,
                                                            accountCreated,
                                                            categoryCode,
                                                            accountOrdinal,
                                                            accountName,   // Title
                                                            balance,
                                                            status);
                    financeviewmodel.PLO.finance_accounts_changesList.Add(account_record);
                    // Do this now so the Account table is up to date
                    if (!await SmartFinanceScrapeV2023.DoAllSmartFinance(ourviewmodel,
                                                                        financeviewmodel,
                                                                        true,
                                                                        SmartParametersV2016.sqliteformat))
                    {
#if WPF || UWP || WINUI
                        MainMeter.financeviewmodel.errorMessage = "Cannot update accounts(3) - cannot continue";
                        await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, financeviewmodel.errorMessage);
                        // This is a bit fatal, because if we can't do this, there's
                        // every chance we can't do lots of things we need to further on
                        return false;
#endif
                    }
                }
            }
            catch (Exception ex)
            {
                financeviewmodel.errorMessage = ex.Message;
            }
            return true;
        }

        internal static async Task<bool> NationwideTransactions(
#if WINFORMS
                                                                SmartDashboard.MainProcess components,
                                                                RichTextBox textBoxConsole,
#endif
                                                                MainViewModel ourviewmodel,
                                                                FinanceViewModel financeviewmodel,
                                                                char categoryCode,
                                                                string[] transactions,
                                                                short institutionCode,
                                                                short brandCode,
                                                                string sortcode,
                                                                string account_no,
                                                                string UDPRN,
                                                                string symbol,
                                                                List<SmartFinance.Transaction_Groups> transaction_groupsFound,
                                                                List<SmartFinance.Transaction_Types> transaction_typesFound,
                                                                DateTime accountCreated)
        {
            // This selects the ones we want (!!) with an EXTRA FUCKER which has no 'id' so
            // the Chimps don't let us filter that one out with a selector - we
            // have to FUCK ABOUT and do it ourselves .. I don't know how to
            // do 'multiple selectors' i.e. acLink AND href=/AccountList
            string date = "",
                    description = "",
                    transaction_type = "";
            bool isPaidIn = false;
            bool balancePaidIn = false;
            string type = "";
            double cryptoAmount = 0;
            short cryptoAmountOrdinal = 0;
            double amount = 0;
            short amountOrdinal = 0;
            double balance = 0;
            short balanceOrdinal = 0;
            DateTime transactionDate = SmartParametersV2016.defaultDate;

            string amountSymbol = "";
            string balanceSymbol = "";
            // These bank transactions are in DESCENDING date order (I think!)            
            // So we find the last, and work backwards down the list to the first
            try
            {
                int howmany = transactions.Length - 1;
                while (howmany >= 0)
                {
                    string[] items = transactions[howmany].Split(SmartParametersV2016.tab);
                    if (items.Length == 6)
                    {
                        int itemIndex = 0;
                        foreach (string item in items)
                        {
                            switch (itemIndex)
                            {
                                case 0: // Date
                                    try
                                    {
                                        date = item.Trim();
                                        transactionDate = SmartRoutinesV2018.DateTimeParse(date);
                                    }
                                    catch (Exception ex)
                                    {
                                        await SmartBobV2017.ListenerAsync(ourviewmodel, financeviewmodel.financeToken, institutionCode, brandCode, "NW date", ex.Message + ": " + date);
                                        return false;
                                    }
                                    break;
                                case 1: // Transaction type
                                    transaction_type = item.Trim();
                                    break;
                                case 2: // Description
                                    description = item.Trim();
                                    break;
                                case 3: // Paid Out
                                    if (item.Length > 0)
                                    {
                                        string AMOUNT = item.Trim();
                                        if (AMOUNT == "")
                                        {
                                            isPaidIn = true;
                                        }
                                        else
                                        {
                                            isPaidIn = false;
                                            // All amounts as Negative
                                            // May need to find first NON-DIGIT here, but for the time being ...
                                            amountSymbol = AMOUNT.Substring(0, 1); // Get LEADING currency symbol

                                            double VALUE = Convert.ToDouble(AMOUNT.Replace(amountSymbol, "").Replace(SmartParametersV2016.defaultCurrencySeparator.ToString(), ""));
                                            amount = VALUE;    // Even Paid out values are positive
                                            //Fiat currencies are legal tender controlled by governments.
                                            //Crypto currencies are digital assets that use blockchain technology.
                                            amountOrdinal = SmartSpikeV2017.GetCurrencyOrdinal(ourviewmodel, amountSymbol); // Er, might work

                                        }
                                    }
                                    else
                                    {
                                        isPaidIn = true;
                                    }
                                    break;
                                case 4: // Paid In
                                    if (item.Length > 0)
                                    {
                                        string AMOUNT = item.Trim();
                                        if (AMOUNT == "")
                                        {
                                            isPaidIn = false;
                                        }
                                        else
                                        {
                                            isPaidIn = true;
                                            // All amounts as Positive
                                            // May need to find first NON-DIGIT here, but for the time being ...
                                            amountSymbol = AMOUNT.Substring(0, 1); // Get LEADING currency symbol

                                            double VALUE = Convert.ToDouble(AMOUNT.Replace(amountSymbol, "").Replace(SmartParametersV2016.defaultCurrencySeparator.ToString(), ""));
                                            amount = VALUE;
                                            //Fiat currencies are legal tender cont rolled by governments.
                                            //Crypto currencies are digital assets that use blockchain technology.
                                            amountOrdinal = SmartSpikeV2017.GetCurrencyOrdinal(ourviewmodel, amountSymbol); // Er, might work
                                        }
                                    }
                                    else
                                    {
                                        isPaidIn = false;
                                    }
                                    break;
                                case 5: // Balance
                                    if (item.Length > 0)
                                    {
                                        string AMOUNT = item.Trim();
                                        // All amounts as Positive
                                        // May need to find first NON-DIGIT here, but for the time being ...
                                        balanceSymbol = AMOUNT.Substring(0, 1); // Get LEADING currency symbol

                                        double VALUE = Convert.ToDouble(AMOUNT.Replace(balanceSymbol, "").Replace(SmartParametersV2016.defaultCurrencySeparator.ToString(), ""));
                                        balance = VALUE; // Might be + or -
                                        if (balance < 0)
                                        {
                                            balancePaidIn = false;
                                        }
                                        else
                                        {
                                            balancePaidIn = true;
                                        }
                                        balanceOrdinal = SmartSpikeV2017.GetCurrencyOrdinal(ourviewmodel, balanceSymbol); // Er, might work
                                    }
                                    break;
                                default:
                                    break;
                            }
                            itemIndex++;
                        }
                        short[] transCodes = SmartSpikeFinanceV2017.Finance_Lookup_TransactionTypes(financeviewmodel,
                                                                                        transaction_groupsFound,
                                                                                        transaction_typesFound,
                                                                                        isPaidIn,
                                                                                        transaction_type,
                                                                                        false,
                                                                                        institutionCode,
                                                                                        brandCode,
                                                                                        categoryCode);
                        if (transCodes.Length != 2)
                        {
#if WPF || UWP || WINUI
                            await SmartBobV2017.ListenerAsync(ourviewmodel, financeviewmodel.financeToken, institutionCode, brandCode, "NW Trans Lookup", "Trans type  failed: " + transaction_type);
#endif
                            // Carry on
                        }
                        else
                        {
                            if (transCodes[0] == 0 || transCodes[1] == 0)
                            {
#if WPF || UWP || WINUI
                                await SmartBobV2017.ListenerAsync(ourviewmodel, financeviewmodel.financeToken, institutionCode, brandCode, "NW Trans Lookup", "Trans type  failed: " + transaction_type);
#endif
                                // Carry on - we have others to do
                            }
                        }
                        // This is ALL WRONG ... hmm not sure it is
                        // need this
                        DateTime startDate = accountCreated;                    // Latest account
                        DateTime endDate = Convert.ToDateTime(transactionDate);     // Transaction Date

                        int diff = SmartDanV2025.GetMonthsDifference(startDate, endDate);

                        short statementNo = Convert.ToInt16(diff + SmartParametersV2016.NWMonthsAdjustment);

                        // Calculate (Kludge City lives on) the statement date and no
                        DateTime statementDate = SmartDanV2025.GetFirstOfNextMonth(transactionDate);

                        // Have we already got this one? (Ignore sequence_no)
                        List<SmartFinance.TransactionsCategories>
                            bt_found = SmartSpikeFinanceV2017.Finance_Lookup_BankTransaction(financeviewmodel,
                                                    ourviewmodel.UserName,
                                                    financeviewmodel.cubeface_code,
                                                    institutionCode,
                                                    brandCode,
                                                    sortcode,
                                                    account_no,
                                                    UDPRN,
                                                    statementDate,
                                                    statementNo,
                                                    transactionDate,
                                                    isPaidIn,
                                                    transCodes,
                                                    description,
                                                    type,               // Crypto Type
                                                    cryptoAmount,       // Crypto
                                                    cryptoAmountOrdinal,// Crypto
                                                    amount,
                                                    amountOrdinal,
                                                    balance,
                                                    balanceOrdinal,
                                                    balancePaidIn);
                        // Only add it in if we can't find it
                        if (bt_found.Count == 0)
                        {
                            // Before we add it in, check to see if we have its header

                            // Now! We are about to add it in, but does it have a header?
                            // Lets go and see in the Transactions list ...
                            SmartFinance.Transactions transHeader = new SmartFinance.Transactions()
                            {
                                USERNAME = ourviewmodel.UserName,
                                CUBEFACE_CODE = SmartParametersV2016.Finance,
                                INSTITUTION_CODE = institutionCode,
                                BRAND_CODE = brandCode,
                                SORTCODE = sortcode,
                                ACCOUNT_NO = account_no,
                                UDPRN = UDPRN,
                                STATEMENT_DATE = statementDate,
                                STATEMENT_NO = statementNo
                            };
                            // Try and find the Header in PLO
                            List<SmartFinance.Transactions> transheaders_found =
                                   new List<SmartFinance.Transactions>
                            (from Header in financeviewmodel.PLO.transactionsList
                             where Header.USERNAME == transHeader.USERNAME &&
                                    Header.CUBEFACE_CODE == transHeader.CUBEFACE_CODE &&
                                    Header.INSTITUTION_CODE == transHeader.INSTITUTION_CODE &&
                                    Header.BRAND_CODE == transHeader.BRAND_CODE &&
                                    Header.SORTCODE == transHeader.SORTCODE &&
                                    Header.ACCOUNT_NO == transHeader.ACCOUNT_NO &&
                                    Header.UDPRN == transHeader.UDPRN &&
                                    Header.STATEMENT_DATE == transHeader.STATEMENT_DATE &&
                                    Header.STATEMENT_NO == transHeader.STATEMENT_NO
                             select Header).ToList();
                            if (transheaders_found.Count == 0)
                            {
                                // Not there? Is it already in our changes?
                                // Don't want to add it in twice!!
                                transheaders_found =
                                   new List<SmartFinance.Transactions>
                                    (from Header in financeviewmodel.PLO.transactions_changesList
                                     where Header.USERNAME == transHeader.USERNAME &&
                                            Header.CUBEFACE_CODE == transHeader.CUBEFACE_CODE &&
                                            Header.INSTITUTION_CODE == transHeader.INSTITUTION_CODE &&
                                            Header.BRAND_CODE == transHeader.BRAND_CODE &&
                                            Header.SORTCODE == transHeader.SORTCODE &&
                                            Header.ACCOUNT_NO == transHeader.ACCOUNT_NO &&
                                            Header.UDPRN == transHeader.UDPRN &&
                                            Header.STATEMENT_DATE == transHeader.STATEMENT_DATE &&
                                            Header.STATEMENT_NO == transHeader.STATEMENT_NO

                                     select Header).ToList();
                                if (transheaders_found.Count == 0)
                                {
                                    // Add it in
                                    transHeader.RANDOMKEY1 = SmartRoutinesV2018.GetNextRandomNew(ourviewmodel.randomR);
                                    transHeader.Updated = false; // Its new
                                    financeviewmodel.PLO.transactions_changesList.Add(transHeader);
                                }
                            }

                            financeviewmodel.sequence_no++;
                            // Still want to add it in even if we can't analyze the transaction ...
                            int random1 = SmartRoutinesV2018.GetNextRandomNew(ourviewmodel.randomR);
                            int random2 = SmartRoutinesV2018.GetNextRandomNew(ourviewmodel.randomR);
                            // Use common template
                            SmartFinance.TransactionsCategories bankTransaction =
                                SmartDanV2025.TransactionCategory_Template(ourviewmodel,
                                                                        financeviewmodel,
                                                                        institutionCode,
                                                                        brandCode,
                                                                        sortcode,
                                                                        account_no,
                                                                        UDPRN,
                                                                        statementDate,
                                                                        statementNo,
                                                                        financeviewmodel.sequence_no,
                                                                        random1,
                                                                        transactionDate,
                                                                        isPaidIn,
                                                                        transCodes,
                                                                        description,
                                                                        type,
                                                                        cryptoAmount,
                                                                        cryptoAmountOrdinal,
                                                                        amount,
                                                                        amountOrdinal,  // Should be 1 for GBP!
                                                                        balance,
                                                                        balanceOrdinal, // Should be 1 for GBP
                                                                        balancePaidIn,
                                                                        random2,
                                                                        false);         // Updated
                            financeviewmodel.PLO.transactionscategories_changesList.Add(bankTransaction);
                            financeviewmodel.transactions_count++;
                        }
                    }
                    howmany--;
                }
            }
            catch (Exception ex)
            {
                if (!await SmartRoutinesV2018.CheckTrace(ourviewmodel, financeviewmodel.financeToken, institutionCode, brandCode, "NationwideTransactions" + ": " + ex.Message))
                {
                    return false;
                }
                return false;
            }
            return true;
        }

        
    }
}
#endif