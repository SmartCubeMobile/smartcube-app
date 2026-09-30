using System.Text.RegularExpressions;
using System.Windows;
using System.Threading;

#if WINFORMS
using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.WinForms;
using System.Text.Json;
#endif

#if WPF
using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.Wpf;
using System.Text.Json;
#endif

#if UWP || WINUI
using System.Threading.Tasks;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Text.Json;
using Microsoft.Web.WebView2.Core;
using Microsoft.UI.Xaml.Controls;
#endif


#if ANDROID
using Android.Content;
using Android.Provider;
#endif

#if !ANDROID
namespace SmartCubeMobile
{
    internal class SantanderWebView2V2025
    {
        //internal static TypedEventHandler<UserNotificationListener, UserNotificationChangedEventArgs> notificationHandler;

        //internal static UserNotificationListener notificationListener = UserNotificationListener.Current;

        internal static async Task RunSantanderWebView2(
#if WINFORMS
                                                    WebView2 FinanceWebView,
                                                    SmartDashboard.MainProcess components,
                                                    RichTextBox textBoxConsole,
#endif
#if SMARTMAUI
                                                    WebView FinanceWebView,
#endif
#if WPF || WINUI || UWP
                                                    WebView2 FinanceWebView,
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
                                                    string Parameter3)
        {
            
#if WPF || WINUI
            //    Visibility = Visibility.Visible,
            //    HorizontalAlignment = HorizontalAlignment.Stretch,
            //    VerticalAlignment = VerticalAlignment.Stretch
#endif
#if SMARTMAUI
            FinanceWebView.Visible = true;            
#endif
#if WPF
            FinanceWebView.Visibility = Visibility.Visible;
            FinanceWebView.HorizontalAlignment = HorizontalAlignment.Stretch;
            FinanceWebView.VerticalAlignment = VerticalAlignment.Stretch;
#endif

#if WINFORMS
            // See chatGPT standard setup for WebView2 Forms
#endif
            try
            {
                await FinanceWebView.EnsureCoreWebView2Async();
                await FinanceWebView.CoreWebView2.Profile.ClearBrowsingDataAsync(CoreWebView2BrowsingDataKinds.DiskCache);
            }
            catch (Exception ex)
            {
                financeviewmodel.errorMessage = "Ensure WebView2 failed: " + ex.Message;
                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, financeviewmodel.errorMessage);
                await Task.CompletedTask;
            }

            financeviewmodel.notificationHandledSource = new TaskCompletionSource<bool>();
            FinanceWebView.CoreWebView2.NewWindowRequested += (sender, args) => args.Handled = true;
#if WPF || UWP || WINUI
            if (ourviewmodel.webviewLogging)
            {
                FinanceWebView.CoreWebView2.OpenDevToolsWindow();
                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "WebView2 logging: Enabled");
            }
#endif
            await FinanceWebView.CoreWebView2.Profile.ClearBrowsingDataAsync();

            int step = 0;
            string logAction = string.Empty;
            string errorMessage = string.Empty;

            // Set up to receive messages
            FinanceWebView.CoreWebView2.WebMessageReceived +=
#if WINFORMS || WPF
                    new EventHandler<CoreWebView2WebMessageReceivedEventArgs>((s, e) => CoreWebView2_WebMessageReceived(s, e,
#endif
#if WINUI || UWP || SMARTMAUI
                    (s, e) => CoreWebView2_WebMessageReceived(s, e,
#endif
#if WINFORMS
                                                    FinanceWebView,
                                                    components,
                                                    textBoxConsole,
#endif
#if SMARTMAUI
                                                    FinanceWebView,
#endif
#if WPF || UWP || WINUI
                                                    FinanceWebView,
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
                                                    transaction_typesFound
#if WINFORMS || WPF
                                                    ));
#endif
#if WINUI || UWP || SMARTMAUI

                                                    );
#endif

            // Set up to deal with Navigation
            FinanceWebView.CoreWebView2.NavigationCompleted += async (s, args) =>
            {
                if (!args.IsSuccess)
                {
                    await Task.CompletedTask;
                }
                try
                {
                    switch (step)
                    {
                        case 0:
                            await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Status: Step 0");
                            string loginHrefJson = await FinanceWebView.ExecuteScriptAsync(@"
                                        (function() {
                                            const el = document.querySelector('[class^=""bp-button log-on""]');
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
                            FinanceWebView.CoreWebView2.Navigate(loginHref);
                            break;
                        
                        case 1:
                            await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Status: Step 1");

                            // This is an async function - don't expect anything sensible back!!
                            string scriptPersonalId = $@"
                            (async function() {{
                                function sleep(ms) {{
                                    return new Promise(resolve => setTimeout(resolve, ms));
                                }}

                                async function waitForElement(id, timeout = 10000) {{
                                    const interval = 200;
                                    let elapsed = 0;
                                    while (elapsed < timeout) {{
                                        const el = document.getElementById(id);
                                        if (el) return el;
                                        await sleep(interval);
                                        elapsed += interval;
                                    }}
                                    return null;
                                }}

                                function setInputValue(el, value) {{
                                    el.focus();
                                    el.value = value;
                                    el.dispatchEvent(new Event('input', {{ bubbles: true }}));
                                    el.dispatchEvent(new Event('change', {{ bubbles: true }}));
                                    el.blur();
                                }}

                                const pidInput = await waitForElement('pid');
                                const secInput = await waitForElement('securityNumber');

                                if (!pidInput || !secInput) {{
                                    return false;
                                }}

                                setInputValue(pidInput, '{Parameter1}');
                                setInputValue(secInput, '{Parameter3}');

                                return true;
                            }})();";

                            try
                            {
                                string result = await FinanceWebView.CoreWebView2.ExecuteScriptAsync(scriptPersonalId);
                                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Status: Personal Id(s) entered");
                            }
                            catch (Exception ex)
                            {
                                financeviewmodel.errorMessage = "Script error 3: " + ex.Message;
                                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, financeviewmodel.errorMessage);
                                await Task.CompletedTask;
                            }
                            
                            // Insert the Modal Sniffer One
                            await FinanceWebView.ExecuteScriptAsync(@"
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

                            string scriptLogOn = @"
                            (async function() {
                                function sleep(ms) {
                                    return new Promise(resolve => setTimeout(resolve, ms));
                                }

                                // Wait for DOM to be fully loaded
                                if (document.readyState !== 'complete') {
                                    await new Promise(resolve => window.addEventListener('load', resolve));
                                }

                                // Wait up to 10 seconds for the button to appear
                                let timeout = 10000;
                                let interval = 250;
                                let elapsed = 0;

                                while (elapsed < timeout) {
                                    const btn = document.getElementById('submitbtn');
                                    if (btn && typeof btn.click === 'function' && !btn.disabled) {
                                        btn.scrollIntoView({ behavior: 'smooth', block: 'center' });
                                        btn.click();
                                        return true;
                                    }
                                    await sleep(interval);
                                    elapsed += interval;
                                }

                                return false;
                            })();";
                            try
                            {
                                string result = await FinanceWebView.CoreWebView2.ExecuteScriptAsync(scriptLogOn);
                                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Status: Submit button clicked");
                            }
                            catch (Exception ex)
                            {
                                financeviewmodel.errorMessage = "Script error 4: " + ex.Message;
                                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, financeviewmodel.errorMessage);
                                await Task.CompletedTask;
                            }
                            step++;
                            break;
                        case 2:
                            await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Step 2: Waiting for OTP...");
                            step++;
                            break;
                        case 3:
                            await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Step 3: Cancelling timer");

                            if (!financeviewmodel.loggedIn)
                            {
                                // Task completed set in here
                                await SmartBanksV2023.WebDriverDead(FinanceWebView, tcs, ourviewmodel, financeviewmodel);
                                await Task.CompletedTask;
                            }
                            string key = @"Timer_" + notificationTitle;
                            if (SmartBanksV2023.IsRunning(financeviewmodel, key))
                            {
                                SmartBanksV2023.CancelTimer(financeviewmodel, key);
                                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Status : Timer " + key + " cancelled");

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
            };

            FinanceWebView.Source = new Uri(startUrl);
            //await financeviewmodel.tcs1X.Task;
        }

        internal static async void CoreWebView2_WebMessageReceived(object sender,
                                                        CoreWebView2WebMessageReceivedEventArgs args,
#if WINFORMS
                                                        WebView2 FinanceWebView,
                                                        SmartDashboard.MainProcess  components,
                                                        RichTextBox textBoxConsole,
#endif
#if SMARTMAUI
                                                        WebView FinanceWebView,
#endif
#if WPF || UWP || WINUI
                                                        WebView2 FinanceWebView,
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
                                                        List<SmartFinance.Transaction_Types> transaction_typesFound)
        {
            try
            {
                if (args.WebMessageAsJson == null)
                {
                    await Task.CompletedTask;
                }
                string messageJson = args.WebMessageAsJson;
                WebMessage message1 = JsonSerializer.Deserialize<WebMessage>(messageJson);

                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "WMStatus: " + message1.status);

                switch (message1.status)
                {
                    case "success_modalsniffer1":
                        // Before we tell the Modal we want an OTP,
                        // setup the Notification shit
                        await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "WMStatus: Modal detected");

                        SmartBanksV2023.SetupOtpListener(
#if WINFORMS
                                                FinanceWebView,
                                                components,
                                                textBoxConsole,
#endif
#if SMARTMAUI
                                                FinanceWebView,
#endif
#if WPF || UWP || WINUI
                                                FinanceWebView,
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
                        await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "WMStatus: Called OTP Listener");

                        // Now ask to be sent the code
                        string scriptPleaseSendCode = @"
                            (async function() {
                                function sleep(ms) {
                                    return new Promise(resolve => setTimeout(resolve, ms));
                                }

                                // Wait for DOM to be fully loaded
                                if (document.readyState !== 'complete') {
                                    await new Promise(resolve => window.addEventListener('load', resolve));
                                }

                                // Wait up to 10 seconds for the button to appear
                                let timeout = 10000;
                                let interval = 250;
                                let elapsed = 0;

                                while (elapsed < timeout) {
                                    const btn = document.getElementById('sendcode');
                                    if (btn && typeof btn.click === 'function' && !btn.disabled) {
                                        btn.scrollIntoView({ behavior: 'smooth', block: 'center' });
                                        btn.click();
                                        return true;
                                    }
                                    await sleep(interval);
                                    elapsed += interval;
                                }

                                return false;
                            })();";

                        try
                        {
                            string result = await FinanceWebView.CoreWebView2.ExecuteScriptAsync(scriptPleaseSendCode);
                            //result = System.Text.Json.JsonSerializer.Deserialize<string>(result); // remove extra quotes
                            await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Status: Asked to be sent code");
                        }
                        catch (Exception ex)
                        {
                            financeviewmodel.errorMessage = "WMStatus: Script error1 " + ex.Message;
                            await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, financeviewmodel.errorMessage);
                            await Task.CompletedTask;
                        }
                        break;

                    case "success_passcode":
                        //
                        // Oh and I'm not sure this work at all without any break-points
                        //
                        string oneTimePasscode = message1.text;
                        if (oneTimePasscode.Length != notificationTagLength)
                        {
                            financeviewmodel.errorMessage = "WMStatus: OneTimePasscode has incorrect length";
                            await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, financeviewmodel.errorMessage);
                            await Task.CompletedTask;
                        }

                        string scriptSetOtpInput = $@"
                        (async function() {{
                            function sleep(ms) {{
                                return new Promise(resolve => setTimeout(resolve, ms));
                            }}

                            async function waitForElement(id, timeout = 10000) {{
                                const interval = 200;
                                let elapsed = 0;
                                while (elapsed < timeout) {{
                                    const el = document.getElementById(id);
                                    if (el) return el;
                                    await sleep(interval);
                                    elapsed += interval;
                                }}
                                return null;
                            }}

                            function typeIntoField(input, value) {{
                                input.focus();
                                input.value = '';

                                for (let i = 0; i < value.length; i++) {{
                                    const char = value[i];
                                    input.value += char;

                                    const keyEvent = new KeyboardEvent('keydown', {{
                                        key: char,
                                        bubbles: true
                                    }});
                                    input.dispatchEvent(keyEvent);

                                    const inputEvent = new Event('input', {{ bubbles: true }});
                                    input.dispatchEvent(inputEvent);
                                }}

                                input.dispatchEvent(new Event('change', {{ bubbles: true }}));
                                input.dispatchEvent(new Event('blur', {{ bubbles: true }}));

                                // console.log(`✅ Typed into ${{input.id}}:`, value);
                                return true;
                            }}

                            const input1 = await waitForElement('pwd', 10000);

                            if (!input1) {{
                                // console.log('❌ OTP input not found');
                                window.chrome.webview.postMessage({{ status: 'failure', text: 'OTP input not found' }});
                                return;
                            }}

                            typeIntoField(input1, '" + oneTimePasscode + $@"');    

                            // console.log('✅ Filled in oneTimePasscode');
                            
                            const sendCodeButton = await waitForElement('btnsubmit');

                            if (sendCodeButton) {{
                                sendCodeButton.focus();
                                sendCodeButton.click();
                                // console.log('✅ Clicked Log in button');
                                if (window.chrome && window.chrome.webview) {{
                                    //window.chrome.webview.postMessage({{ status: 'submit_otp' }});
                                }}
                            }} else {{
                                // console.log('❌ Send code button not found');
                                if (window.chrome && window.chrome.webview) {{
                                    window.chrome.webview.postMessage({{ status: 'failure', text: 'Button sendcode not found' }});
                                }}
                            }}
                            }})();";

                        // Execute the script in WebView2
                        try
                        {
                            string result = await FinanceWebView.CoreWebView2.ExecuteScriptAsync(scriptSetOtpInput);
                            //result = System.Text.Json.JsonSerializer.Deserialize<string>(result); // remove extra quotes
                            await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "WMStatus: Set OTP input");
                        }
                        catch (Exception ex)
                        {
                            financeviewmodel.errorMessage = "WMScript error2: " + ex.Message;
                            await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, financeviewmodel.errorMessage);
                            await Task.CompletedTask;
                        }
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

        internal static Task WaitForNavigationAsync(CoreWebView2 webView)
        {
            var tcs = new TaskCompletionSource<bool>();

            void handler(object sender, CoreWebView2NavigationCompletedEventArgs e)
            {
                webView.NavigationCompleted -= handler;
                tcs.TrySetResult(true);
            }
            webView.NavigationCompleted += handler;
            return tcs.Task;
        }


        internal static async Task<bool> Part2Async(
#if WINFORMS
                                                SmartDashboard.MainProcess components,
                                                RichTextBox textBoxConsole,
#endif
                                                TaskCompletionSource<bool> tcs,
                                                MainViewModel ourviewmodel,
                                                FinanceViewModel financeviewmodel,
                                                CoreWebView2 webView,
                                                SmartFinance.Logins loginInfo,
                                                string oneTimePasscode,
                                                short institution_code,
                                                short brand_code,
                                                List<SmartFinance.Transaction_Groups> transaction_groupsFound,
                                                List<SmartFinance.Transaction_Types> transaction_typesFound)
        {
            await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "In: Part2");

            try
            {
//                if (string.IsNullOrEmpty(oneTimePasscode))
//                {
//                    await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Notification content: Empty");
//                    financeviewmodel.errorMessage = "Notification content is empty";
//                    await Task.CompletedTask;
//                }

//                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Inserting: " + oneTimePasscode);

//                string setOtpScript = $@"
//                    (function() {{
//                        const input = document.querySelector('input[name=""OneTimePasscode""]');
//                        if(input) {{
//                            input.value = '{oneTimePasscode}';
//                            return 'ok';
//                        }} else {{
//                            return 'not found';
//                        }}
//                    }})();";


//                string setOtpResult = await webView.ExecuteScriptAsync(setOtpScript);
//                if (!setOtpResult.Contains("ok"))
//                {
//                    await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Failed to set OTP input field");
//                    financeviewmodel.errorMessage = "Failed to set OTP input field";
//                    return false;
//                }

//                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, $"Input Field: {oneTimePasscode} filled");

//                financeviewmodel.loggedIn = false;

//                string waitForLoginAndClick = @"
//            new Promise(resolve => {
//                const checkExist = setInterval(() => {
//                    const buttons = Array.from(document.getElementsByTagName('button'));
//                    const loginBtn = buttons.find(b => b.innerText.trim().toLowerCase() === 'log in');
//                    if (loginBtn) {
//                        loginBtn.click();
//                        clearInterval(checkExist);
//                        resolve('clicked');
//                    }
//                }, 100);
//                setTimeout(() => { clearInterval(checkExist); resolve('not found'); }, 10000);
//            });";

//                string clickResult = await webView.ExecuteScriptAsync(waitForLoginAndClick);
//                await WaitForNavigationAsync(webView);
//                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Log in button: Clicked");

//                financeviewmodel.loggedIn = true;
//                financeviewmodel.accountItems = new List<FinanceViewModel.AccountItem>();
//                financeviewmodel.currentUrl = webView.Source?.ToString() ?? "";

//                string getWelcomeScript = @"
//                                        (function() {
//                                            const welcomeElem = document.getElementById('welcome-message');
//                                            return welcomeElem && welcomeElem.innerText ? welcomeElem.innerText : '';
//                                        })();";

//                string welcomeTextRaw = await webView.ExecuteScriptAsync(getWelcomeScript);
//                string welcomeText = System.Text.Json.JsonDocument.Parse(welcomeTextRaw).RootElement.GetString() ?? "";
//                string owner = welcomeText.Replace("Welcome back,", "").Trim();

//                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Owner: " + loginInfo.OWNER);

//                financeviewmodel.UDPRN = "";

//                if (!await FindAddressAsync(ourviewmodel, financeviewmodel, webView, loginInfo, owner))
//                {
//                    await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Find address: Failed");
//                    return false;
//                }

//                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Contact no.: " + loginInfo.CONTACT_PHONENO);
//                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Email address: " + loginInfo.CONTACT_EMAIL);

//                financeviewmodel.account_id = 0;

//                if (!await NationwideBuildAccountList(
//#if WINFORMS
//                    components, textBoxConsole,
//#endif
//                    ourviewmodel, financeviewmodel, webView,
//                    institution_code, brand_code,
//                    financeviewmodel.UDPRN,
//                    transaction_groupsFound,
//                    transaction_typesFound
//                ))
//                {
//                    await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, financeviewmodel.errorMessage);
//                    return false;
//                }

//                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Transactions inserted: " + financeviewmodel.transactions_count);
//                return true;
            }
            catch (Exception ex)
            {
                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Exception: " + ex.Message);
                financeviewmodel.errorMessage = "Exception occurred: " + ex.Message;
                tcs.TrySetResult(true);
                return false;
            }
            return true;
        }

        internal static async Task<bool> Part3Async(
#if WINFORMS
                                                    SmartDashboard.MainProcess components,
                                                    RichTextBox textBoxConsole,
#endif
                                                    TaskCompletionSource<bool> tcs,
                                                    MainViewModel ourviewmodel,
                                                    FinanceViewModel financeviewmodel,
                                                    WebView2 webView)
        {
            await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "In: Part3");

            //const string script = @"
            //                    (function() {
            //                        const anchors = Array.from(document.querySelectorAll('a'));
            //                        const logoutLink = anchors.find(el =>
            //                            el.textContent.trim().toLowerCase() === 'log out');
            //                        if (logoutLink) {
            //                            logoutLink.click();
            //                            return 'clicked';
            //                        } else {
            //                            return 'not found';
            //                        }
            //                    })();";

            try
            {
                //string result = await webView.ExecuteScriptAsync(script);

                //if (!result.Contains("clicked"))
                //{
                //    financeviewmodel.errorMessage = "Couldn't find logout button";
                //    await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "No logout button found");
                //    tcs.TrySetResult(true); // <-- Signal failure no success whatever
                //}

                //financeviewmodel.loggedIn = false;
                //await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Logout button: Clicked");
                //// Completion gets set in WebDead routine
                //return true;
            }
            catch (Exception)// ex)
            {
                //financeviewmodel.errorMessage = $"Logout failed: {ex.Message}";
                //await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Logout button click fail: " + ex.Message);
                tcs.TrySetResult(true); // <-- Signal failure no success whatever
                return false;
            }
            return true;
        }

        internal static async Task ClickLinkAndWaitAsync(WebView2 webView, string script, string href)
        {
            var tcs = new TaskCompletionSource<bool>();

            void Handler(object sender, CoreWebView2NavigationCompletedEventArgs e)
            {
                webView.NavigationCompleted -= Handler;
                tcs.TrySetResult(true);
            }

            webView.NavigationCompleted += Handler;

            var result = await webView.ExecuteScriptAsync(script);
            if (result == "false")
            {
                webView.NavigationCompleted -= Handler;
                throw new InvalidOperationException("No link found for " + href);
            }
            await tcs.Task; // Wait for navigation to complete
        }

        internal static async Task<bool> FindAddressAsync(
                                                        MainViewModel ourviewmodel,
                                                        FinanceViewModel financeviewmodel,
                                                        WebView2 webView,
                                                        SmartFinance.Logins loginInfo,
                                                        string owner)
        {
            try
            {
                //financeviewmodel.UDPRN = "";

                // Well, I have Dan to thank for showing me chatGPT because
                // it certainly saved my sorry arse at 07:37 today 27th May 2025
                // as I have been struggling for an entire TWO DAYS to get this
                // bitch working ... but here it is: a routine that finds
                // the anchor containing the href ... clicks it and WAITS
                // for the fucking page to load!  Now .. can I use this bastard
                // anywhere else??  Time for a cuppa, Ray! Time for a cuppa ...
                string hrefContains = "MaintainTelephoneAndAddress";
                string escapedHref = hrefContains.Replace("'", "\\'");
                string script = $@"
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
                await ClickLinkAndWaitAsync(webView, script, "MTA");

                // Call FindDetails - you must have a WebView2 version of this method
                string UDPRN = await FindDetailsAsync(ourviewmodel,
                                                                financeviewmodel,
                                                                webView,
                                                                loginInfo,
                                                                owner);

                if (string.IsNullOrEmpty(UDPRN))
                {
                    await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "UDPRN: <empty>");
                    financeviewmodel.errorMessage = "UDPRN is empty";
                    return false;
                }
                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "UDPRN good: " + UDPRN);
            }
            catch (Exception ex)
            {
                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Find Address: " + ex.Message);
            }
            return true;
        }

        internal static async Task<string> FindDetailsAsync(MainViewModel ourviewmodel,
                                                            FinanceViewModel financeviewmodel,
                                                            WebView2 webView,
                                                            SmartFinance.Logins loginInfo,
                                                            string owner)
        {
            string udprn = "";
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
                                    })();";

                string addressJson = await webView.ExecuteScriptAsync(getAddressScript);
                string bankaddress = System.Text.Json.JsonSerializer.Deserialize<string>(addressJson);

                if (string.IsNullOrWhiteSpace(bankaddress))
                {
                    financeviewmodel.errorMessage = "Bank address empty - cannot continue";
                    return udprn;
                }

                // Step 2: Lookup existing address in your database or cache
                var addresses_found = SmartSpikeV2017.UsersLookupTextAddress(ourviewmodel, bankaddress);
                if (addresses_found.Count > 0)
                {
                    if (string.IsNullOrEmpty(addresses_found.First().UDPRN))
                    {
                        financeviewmodel.errorMessage = "Address record has no UDPRN - cannot continue";
                        return udprn;
                    }
                    udprn = addresses_found.First().UDPRN;
                }
                else
                {
                    // Step 5: New address - lookup via external service
                    SmartProfile.AddressesView ideal_address = await SmartRoutinesV2018.UsersUDPRNLookup(ourviewmodel, bankaddress);
                    if (string.IsNullOrEmpty(ideal_address.UDPRN))
                    {
                        financeviewmodel.errorMessage = "Cannot determine UDPRN - cannot continue";
                        return udprn;
                    }
                    udprn = ideal_address.UDPRN;

                    // Add new address record logic here (similar to your original code) ...
                    // ...
                }
                // Step 7: Extract email and phone from page via JS
                string getEmailScript = @"
                                        (function() {
                                            let el = document.getElementById('CurrentEmailAddress');
                                            return el ? el.value : '';
                                        })();";
                string emailJson = await webView.ExecuteScriptAsync(getEmailScript);
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
                                        })();
                                    ";
                string phoneJson = await webView.ExecuteScriptAsync(getPhoneScript);
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

        private class AccountLink
        {
            public string Href { get; set; }
            public string InnerHTML { get; set; }
        }

        internal static async Task<bool> NationwideBuildAccountList(
#if WINFORMS
                                                                    SmartDashboard.MainProcess components,
                                                                    RichTextBox textBoxConsole,
#endif
                                                                    MainViewModel ourviewmodel,
                                                                    FinanceViewModel financeviewmodel,
                                                                    WebView2 webView,
                                                                    short institution_code,
                                                                    short brand_code,
                                                                    string UDPRN,
                                                                    List<SmartFinance.Transaction_Groups> transaction_groupsFound,
                                                                    List<SmartFinance.Transaction_Types> transaction_typesFound)
        {
            try
            {
                string hrefContains = "AccountList/AccountList";
                string escapedHref = hrefContains.Replace("'", "\\'");
                
                // Navigate to the href and wait for navigation complete event
                string script = $@"
                            (() => {{
                                const link = Array.from(document.querySelectorAll('a[href]')).find(a => a.href.includes('{escapedHref}'));
                                if (link) {{
                                    link.click();
                                    return true;
                                }} else {{
                                    return false;
                                }}
                            }})()";
                await ClickLinkAndWaitAsync(webView, script, "AL");                

                List<AccountLink> accounts = new List<AccountLink>();
                // Run JavaScript to find the links under element with class 'active' with href containing "/AccountList/Account/RedirectToDefaultPage"
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

                string jsonResult = await webView.ExecuteScriptAsync(scriptList);


                // Step 1: Trim leading/trailing quotes from JS result because we have a string not an array
                if (jsonResult.StartsWith("\"") && jsonResult.EndsWith("\""))
                {
                    jsonResult = jsonResult.Substring(1, jsonResult.Length - 2);
                }
                // Step 2: Unescape the JSON string
                string unescapedJson = System.Text.RegularExpressions.Regex.Unescape(jsonResult);

                // Step 3: Deserialize JSON string to a list of link info objects
                accounts = System.Text.Json.JsonSerializer.Deserialize<List<AccountLink>>(unescapedJson);

                if (accounts == null || accounts.Count == 0)
                {
                    financeviewmodel.errorMessage = "No accounts found.";
                    return false;
                }

                foreach (AccountLink account in accounts)
                {
                    string accountInfo1 = "";
                    string accountInfo2;
                    string accountName = "";
                    string balance = "";
                    string symbol = "";
                    char categoryCode = SmartParametersV2016.defaultChar;

                    // Parse account details from innerHTML
                    NewAccountBalance(financeviewmodel, account.InnerHTML, ref accountInfo1, ref balance, ref symbol);

                    if (string.IsNullOrEmpty(accountInfo1))
                    {
                        financeviewmodel.errorMessage = "Couldn't parse account info: " + accountInfo1;
                        return false;
                    }

                    accountInfo2 = SplitAccountName(financeviewmodel, accountInfo1, ref accountName);
                    if (string.IsNullOrEmpty(accountName))
                    {
                        financeviewmodel.errorMessage = "Account name empty";
                        return false;
                    }
                    if (string.IsNullOrEmpty(accountInfo2))
                    {
                        financeviewmodel.errorMessage = "Account sortcode/account empty";
                        return false;
                    }

                    short currency_ordinal = SmartSpikeFinanceV2017.GetCurrencyOrdinal(ourviewmodel, symbol);

                    categoryCode = SmartParametersV2016.Banks; // default
                    if (accountName.Contains("Savings", StringComparison.OrdinalIgnoreCase))
                    {
                        categoryCode = SmartParametersV2016.Savings;
                    }

                    string[] accstuff = accountInfo2.Split(SmartParametersV2016.spacechar);
                    string sortcode = "";
                    string account_no = "";

                    if (accstuff.Length > 1)
                    {
                        sortcode = accstuff[0].Replace("-", "");
                        account_no = accstuff[1];
                    }
                    else
                    {
                        account_no = accstuff[0];
                    }

                    bool decoded = await DecodeAccount(ourviewmodel,
                                                        financeviewmodel,
                                                        accountName,
                                                        sortcode,
                                                        account_no,
                                                        currency_ordinal,
                                                        institution_code,
                                                        brand_code,
                                                        UDPRN,
                                                        categoryCode);
                    if (!decoded)
                    {
                        return false;
                    }
                    if (!await ProcessAccountAsync(
#if WINFORMS
                        components,
                        textBoxConsole,
#endif
                        ourviewmodel,
                        financeviewmodel,
                        webView,
                        categoryCode,
                        account.Href,
                        institution_code,
                        brand_code,
                        sortcode,
                        account_no,
                        UDPRN,
                        0, // accountOrdinal (not used here, adjust if needed)
                        symbol,
                        transaction_groupsFound,
                        transaction_typesFound))
                    {
#if WPF || UWP || WINUI
                        await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Process Account: " + financeviewmodel.errorMessage);
#endif
                    }
                }
                
            }
            catch (Exception ex)
            {
                financeviewmodel.errorMessage = ex.Message;

                return false;
            }
            return true;
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
                        financeviewmodel.errorMessage = "Cannot update accounts(3) - cannot continue";
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

        internal static async Task<bool> ProcessAccountAsync(
#if WINFORMS
                                                        SmartDashboard.MainProcess components,
                                                        RichTextBox textBoxConsole,
#endif
                                                        MainViewModel ourviewmodel,
                                                        FinanceViewModel financeviewmodel,
                                                        WebView2 webView,
                                                        char categoryCode,
                                                        string href,
                                                        short institution_code,
                                                        short brand_code,
                                                        string sortcode,
                                                        string account_no,
                                                        string UDPRN,
                                                        short accountOrdinal,
                                                        string symbol,
                                                        List<SmartFinance.Transaction_Groups> transaction_groupsFound,
                                                        List<SmartFinance.Transaction_Types> transaction_typesFound)
        {
            try
            {
                //string hrefContains = "AccountList/AccountList";
                string escapedHref = href.Replace("'", "\\'");
                string script = $@"
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
                await ClickLinkAndWaitAsync(webView, script, "ALD");
                
                string oneyearago = DateTime.Now.AddYears(-1).ToString("dd/MM/yyyy");
                await webView.ExecuteScriptAsync(@"document.getElementById('statement-from-date').value = '" + oneyearago + "';");

                await webView.ExecuteScriptAsync(@"document.getElementById('date-filter-update').click();");

                await Task.Delay(2000);
                string[] transactions = new string[0];
                // // Wait for table body to be updated – use a polling loop with timeout
                string tableScript = @"
                                (function(){
                                    let tbody = document.querySelector('tbody');
                                    return tbody ? tbody.innerText : '';
                                })();";
                string jsonResult = await webView.ExecuteScriptAsync(tableScript);
                if (jsonResult.StartsWith("\"") && jsonResult.EndsWith("\""))
                {
                    jsonResult = jsonResult.Substring(1, jsonResult.Length - 2);
                }
                jsonResult = Regex.Unescape(jsonResult);
                transactions = jsonResult.Split('\n');                
                if (transactions.Length == 0)
                {
                    financeviewmodel.errorMessage = "No accounts found.";
                    return false;
                }
#if WPF || UWP || WINUI
                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Transactions found: " + transactions.Length);
#endif

                DateTime accountCreated = SmartParametersV2016.defaultDate;
                List<SmartFinance.Accounts> accounts_found = financeviewmodel.PLO.finance_accountsList
                    .Where(a => a.INSTITUTION_CODE == institution_code &&
                                a.BRAND_CODE == brand_code &&
                                a.SORTCODE == sortcode &&
                                a.ACCOUNT_NO == account_no &&
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
                                            sortcode,
                                            account_no,
                                            UDPRN,
                                            symbol,
                                            transaction_groupsFound,
                                            transaction_typesFound,
                                            accountCreated))
                {
                    await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Transactions failed ");
                    return false;
                }
            }
            catch (Exception ex)
            {
                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Exception in ProcessAccount: " + ex.Message);
                return false;
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
            int isPaidIn = 0;
            int balancePaidIn = 0;
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
                                            isPaidIn = 1;
                                        }
                                        else
                                        {
                                            isPaidIn = 0;
                                            // All amounts as Negative
                                            // May need to find first NON-DIGIT here, but for the time being ...
                                            amountSymbol = AMOUNT.Substring(0, 1); // Get LEADING currency symbol

                                            double VALUE = Convert.ToDouble(AMOUNT.Replace(amountSymbol, "").Replace(SmartParametersV2016.defaultCurrencySeparator.ToString(), ""));
                                            amount = VALUE;    // Even Paid out values are positive
                                            //Fiat currencies are legal tender controlled by governments.
                                            //Crypto currencies are digital assets that use blockchain technology.
                                            amountOrdinal = SmartSpikeFinanceV2017.GetCurrencyOrdinal(ourviewmodel, amountSymbol); // Er, might work

                                        }
                                    }
                                    else
                                    {
                                        isPaidIn = 1;
                                    }
                                    break;
                                case 4: // Paid In
                                    if (item.Length > 0)
                                    {
                                        string AMOUNT = item.Trim();
                                        if (AMOUNT == "")
                                        {
                                            isPaidIn = 0;
                                        }
                                        else
                                        {
                                            isPaidIn = 1;
                                            // All amounts as Positive
                                            // May need to find first NON-DIGIT here, but for the time being ...
                                            amountSymbol = AMOUNT.Substring(0, 1); // Get LEADING currency symbol

                                            double VALUE = Convert.ToDouble(AMOUNT.Replace(amountSymbol, "").Replace(SmartParametersV2016.defaultCurrencySeparator.ToString(), ""));
                                            amount = VALUE;
                                            //Fiat currencies are legal tender cont rolled by governments.
                                            //Crypto currencies are digital assets that use blockchain technology.
                                            amountOrdinal = SmartSpikeFinanceV2017.GetCurrencyOrdinal(ourviewmodel, amountSymbol); // Er, might work
                                        }
                                    }
                                    else
                                    {
                                        isPaidIn = 0;
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
                                            balancePaidIn = 0;
                                        }
                                        else
                                        {
                                            balancePaidIn = 1;
                                        }
                                        balanceOrdinal = SmartSpikeFinanceV2017.GetCurrencyOrdinal(ourviewmodel, balanceSymbol); // Er, might work
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