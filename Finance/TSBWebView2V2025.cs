using System.Text.RegularExpressions;
using System.Windows;
using System.Threading;

#if WINFORMS
using static SmartCubeMobile.MainViewModel;
using Windows.UI.Notifications.Management;
using Windows.UI.Notifications;
using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.WinForms;
using Windows.Foundation;
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
using Windows.UI.Core;
using Windows.UI.Notifications.Management;
using Windows.UI.Notifications;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml;
using Microsoft.Web.WebView2.Core;
using Windows.Foundation;
using System.Text.Json;


#endif

#if WINUI
using Microsoft.UI.Dispatching;
#endif

#if ANDROIDX
using Android.Content;
using Android.Provider;
#endif

#if !ANDROID
namespace SmartCubeMobile
{
    // Not sure this works fully without breakpoints  <======


    internal class TSBWebView2
    {
        //internal static TypedEventHandler<UserNotificationListener, UserNotificationChangedEventArgs> notificationHandler;

        //internal static UserNotificationListener notificationListener = UserNotificationListener.Current;

        internal static string PARAMETER2 = "";
        internal static string PARAMETER3 = "";
        internal static async Task RunTSBWebView2(
#if WINFORMS
                                                    WebView2 FinanceWebView,
                                                    SmartDashboard.MainProcess components,
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
            PARAMETER2 = Parameter2;
            PARAMETER3 = Parameter3;
#if WINFORMS
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
                //await FinanceWebView.CoreWebView2.Profile.ClearBrowsingDataAsync(CoreWebView2BrowsingDataKinds.DiskCache);
            }
            catch (Exception ex)
            {
                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Ensure WebView2 failed: " + ex.Message);
                await Task.CompletedTask;
                return;// false;
            }

            financeviewmodel.notificationHandledSource = new TaskCompletionSource<bool>();
            FinanceWebView.CoreWebView2.NewWindowRequested += (sender, args) => args.Handled = true;
#if WPF || UWP || WINUI
            if (ourviewmodel.webviewLogging)
            {
                FinanceWebView.CoreWebView2.OpenDevToolsWindow();
            }
#endif
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
#if UWP || WINUI || SMARTMAUI
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
                            string jsCode = @"
                            (() => {
                                return Array.from(document.querySelectorAll('a'))
                                    .filter(a => a.textContent.trim().includes('Login'))
                                    .map(a => a.href);
                            })()
                            ";
                            string loginHref = "";
                            // Run the JavaScript and get the result
                            string rawResult = await FinanceWebView.CoreWebView2.ExecuteScriptAsync(jsCode);

                            // Parse JSON string result to C# list
                            // The result is a JSON string like: "[\"https://example.com/signin\"]"
                            try
                            {
                                var hrefs = System.Text.Json.JsonSerializer.Deserialize<List<string>>(rawResult);

                                foreach (var href in hrefs)
                                {
                                    loginHref = href;   
                                    await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Found href: " + href);
                                }
                            }
                            catch (Exception ex)
                            {
                                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Error parsing hrefs: " + ex.Message);
                                return;                                
                            }
                            step++;
                            // Its the last href so it appears!!
                            FinanceWebView.CoreWebView2.Navigate(loginHref);
                            break;
                        
                        case 1:
                            // Force the one input value needed                            
                            string scriptUserId = $@"
                            (function() {{
                                function setInputValue(id, value) {{
                                    const el = document.getElementById(id);
                                    if (el) {{
                                        el.value = value;
                                        el.dispatchEvent(new Event('input', {{ bubbles: true }}));
                                        el.dispatchEvent(new Event('change', {{ bubbles: true }}));
                                    }}
                                }}
                                setInputValue('userId','{Parameter1}');
                            }})();";

                            await FinanceWebView.ExecuteScriptAsync(scriptUserId);

                            // JavaScript code to find the button, scroll into view, enable it, and click
                            string scriptContinue1 = @"
                                (function() {
                                    const buttons = Array.from(document.querySelectorAll('button'));
                                    const continueButton = buttons.find(b => b.textContent.trim().toLowerCase() === 'continue');
                                    if (continueButton) 
                                    {
                                       continueButton.scrollIntoView({ behavior: 'smooth', block: 'center' });
                                       continueButton.click();
                                       return 'Button clicked';
                                    } 
                                    else 
                                    {
                                        return 'Button not found';
                                    }
                                })();";
                            try
                            {
                                string resultContinue = await FinanceWebView.CoreWebView2.ExecuteScriptAsync(scriptContinue1);
                                resultContinue = System.Text.Json.JsonSerializer.Deserialize<string>(resultContinue); // remove extra quotes
                                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Continue button(1): " + resultContinue);
                            }
                            catch (Exception ex)
                            {
                                financeviewmodel.errorMessage = "ScriptContinue error: " + ex.Message;
                                //await Task.CompletedTask;
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
                            })();
                            ");

                            // You need to change this!!
                            // SO THAT YOU DON'T RETURN ANYTHING!
                            // Change it to 'return true;' or return false;
                            // DON'T RETURN TEXT IT DOESN'T WORK!!!!

                            string scriptPassword = $@"
                            (async function() {{
                                const timeout = 10000; // 10 seconds max
                                const interval = 100;  // Check every 100ms
                                const maxTries = timeout / interval;
                                let tries = 0;

                                function setInputValue(el, value) {{
                                    el.value = value;
                                    el.dispatchEvent(new Event('input', {{ bubbles: true }}));
                                    el.dispatchEvent(new Event('change', {{ bubbles: true }}));
                                }}

                                while (tries < maxTries) {{
                                    const el = document.getElementById('loginPassword');
                                    if (el) {{
                                        setInputValue(el, '{Parameter2}');
                                        return true;
                                    }}
                                    await new Promise(resolve => setTimeout(resolve, interval));
                                    tries++;
                                }}

                                return false;
                            }})();
                            ";

                            try
                            {
                                await FinanceWebView.ExecuteScriptAsync(scriptPassword);
                                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Password set");
                            }
                            catch (Exception ex)
                            {
                                financeviewmodel.errorMessage = "ScriptPassword error: " + ex.Message;
                                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, financeviewmodel.errorMessage);
                                await Task.CompletedTask;
                            }
                            step++;
                            break;
                        case 2:
                            
                           

                            
                            // 18-Jul2025 07:09
                            // *** This took me A WHOLE DAY TO FIGURE OUT!! ***

                            string scriptGetOTP = @"
                            (function observeAndClickContinueButton() {
                                return new Promise((resolve, reject) => {
                                    try {
                                        const checkAndClick = () => {
                                            const buttons = Array.from(document.querySelectorAll('button'));
                                            const target = buttons.find(b => b.textContent.trim().toLowerCase() === 'continue');

                                            if (!target) return false;

                                            const style = window.getComputedStyle(target);
                                            const isVisible = !!(target.offsetWidth || target.offsetHeight || target.getClientRects().length);
                                            const isDisabled = target.disabled;

                                            if (!isVisible || isDisabled || style.display === 'none' || style.visibility === 'hidden' || parseFloat(style.opacity) === 0) {
                                                return false;
                                            }

                                            // Button found and ready — click it like a human
                                            target.scrollIntoView({ behavior: 'smooth', block: 'center' });
                                            target.focus();

                                            const rect = target.getBoundingClientRect();
                                            const x = rect.left + rect.width / 2;
                                            const y = rect.top + rect.height / 2;

                                            ['mouseover', 'mousemove', 'mousedown', 'mouseup', 'click'].forEach(type => {
                                                const evt = new MouseEvent(type, {
                                                    view: window,
                                                    bubbles: true,
                                                    cancelable: true,
                                                    clientX: x,
                                                    clientY: y,
                                                    button: 0
                                                });
                                                target.dispatchEvent(evt);
                                            });

                                            resolve({ success: true, message: 'Button clicked' });
                                            return true;
                                        };

                                        // Try immediately in case it's already there
                                        if (checkAndClick()) return;

                                        const observer = new MutationObserver(() => {
                                            if (checkAndClick()) {
                                                observer.disconnect();
                                            }
                                        });

                                        observer.observe(document.body, { childList: true, subtree: true });

                                        // Timeout in case it never appears
                                        setTimeout(() => {
                                            observer.disconnect();
                                            resolve({ success: false, message: 'Button not found in time' });
                                        }, 10000); // 10 seconds
                                    } catch (err) {
                                        reject({ success: false, message: 'Error: ' + err.message });
                                    }
                                });
                            })();";

                            try
                            {
                                // THERE IS NO RESULT TO DESERIALIZE!!
                                string resultyy = await FinanceWebView.CoreWebView2.ExecuteScriptAsync(scriptGetOTP);
                                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Continue: " + resultyy);
                            }
                            catch (Exception ex)
                            {
                                financeviewmodel.errorMessage = "Script error: " + ex.Message;
                                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, financeviewmodel.errorMessage);
                                await Task.CompletedTask;
                            }
                            step++;
                            break;                            
                        case 4:
                            await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Step 4: ");

                            if (!financeviewmodel.loggedIn)
                            {
                                // Task completed set in here
                                await SmartBanksV2023.WebDriverDead(FinanceWebView, tcs, ourviewmodel, financeviewmodel);
                                await Task.CompletedTask;
                            }
                            break;
                        case 99:
                            await Task.CompletedTask;
                            return;
                        default:
                            break;
                    }
                }
                catch (Exception ex)
                {
                    await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, $"Step {step} failed: " + ex.Message);
                    await Task.CompletedTask;
                }
            };

            FinanceWebView.Source = new Uri(startUrl);
            //await financeviewmodel.tcs1X.Task;
        }

        private static async void CoreWebView2_WebMessageReceived(object sender,
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
                                            TaskCompletionSource<bool> tcs,MainViewModel ourviewmodel,
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
                    return;
                }
                string messageJson = args.WebMessageAsJson;
                WebMessage message1 = JsonSerializer.Deserialize<WebMessage>(messageJson);

                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Status: " + message1.status);

                switch (message1.status)
                {
                    // Modal Sniffer One
                    case "success_modalsniffer1":
                        // Do the Characters (the Password has already been done)
                        string charPosX = "",
                                charPosY = "",
                                charPosZ = "";
                        string scriptGetCharTextX = @"
                        (() => {
                          const el = document.querySelector('span.text-bold.text-std[translate=""RETAIL_CREDENTIALS_PUBLIC_LOGON_MEMORABLEINFORMATION_CHARACTER1""]');
                          return el ? el.textContent.trim() : null;
                        })()";
                        try
                        {
                            string resultX = await FinanceWebView.ExecuteScriptAsync(scriptGetCharTextX);
                            string charLabelX = System.Text.Json.JsonSerializer.Deserialize<string>(resultX);
                            charLabelX = charLabelX.Replace("Character", "");
                            charLabelX = charLabelX.Replace(":", "").Trim();
                            int posX = Convert.ToInt32(charLabelX) - 1;
                            if (posX <= PARAMETER3.Length)
                            {
                                charPosX = PARAMETER3.Substring(posX, 1);
                            }
                            await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Char Label: " + charLabelX + " " + charPosX);
                        }
                        catch (Exception ex)
                        {
                            financeviewmodel.errorMessage = "ScriptCharX error: " + ex.Message;
                        }
                        // Force the first input value needed                            
                        string scriptcharXPos = $@"
                            (function() {{
                                function setInputValue(id, value) {{
                                    const el = document.getElementById(id);
                                    if (el) {{
                                        el.value = value;
                                        el.dispatchEvent(new Event('input', {{ bubbles: true }}));
                                        el.dispatchEvent(new Event('change', {{ bubbles: true }}));
                                    }}
                                }}
                                setInputValue('charXPos','{charPosX}');
                            }})();";

                        await FinanceWebView.ExecuteScriptAsync(scriptcharXPos);

                        string scriptGetCharTextY = @"
                        (() => {
                          const el = document.querySelector('span.text-bold.text-std[translate=""RETAIL_CREDENTIALS_PUBLIC_LOGON_MEMORABLEINFORMATION_CHARACTER2""]');
                          return el ? el.textContent.trim() : null;
                        })()";
                        try
                        {
                            string resultY = await FinanceWebView.ExecuteScriptAsync(scriptGetCharTextY);
                            string charLabelY = System.Text.Json.JsonSerializer.Deserialize<string>(resultY);
                            charLabelY = charLabelY.Replace("Character", "");
                            charLabelY = charLabelY.Replace(":", "").Trim();
                            int posY = Convert.ToInt32(charLabelY) - 1;
                            if (posY <= PARAMETER3.Length)
                            {
                                charPosY = PARAMETER3.Substring(posY, 1);
                            }
                            await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Char Label: " + charLabelY + " " + charPosY);
                        }
                        catch (Exception ex)
                        {
                            financeviewmodel.errorMessage = "ScriptCharY error: " + ex.Message;
                        }

                        // Force the second input value needed                            
                        string scriptcharYPos = $@"
                            (function() {{
                                function setInputValue(id, value) {{
                                    const el = document.getElementById(id);
                                    if (el) {{
                                        el.value = value;
                                        el.dispatchEvent(new Event('input', {{ bubbles: true }}));
                                        el.dispatchEvent(new Event('change', {{ bubbles: true }}));
                                    }}
                                }}
                                setInputValue('charYPos','{charPosY}');
                            }})();";

                        await FinanceWebView.ExecuteScriptAsync(scriptcharYPos);


                        string scriptGetCharTextZ = @"
                        (() => {
                          const el = document.querySelector('span.text-bold.text-std[translate=""RETAIL_CREDENTIALS_PUBLIC_LOGON_MEMORABLEINFORMATION_CHARACTER3""]');
                          return el ? el.textContent.trim() : null;
                        })()";
                        try
                        {
                            string resultZ = await FinanceWebView.ExecuteScriptAsync(scriptGetCharTextZ);
                            string charLabelZ = System.Text.Json.JsonSerializer.Deserialize<string>(resultZ);
                            charLabelZ = charLabelZ.Replace("Character", "");
                            charLabelZ = charLabelZ.Replace(":", "").Trim();
                            int posZ = Convert.ToInt32(charLabelZ) - 1;
                            if (posZ <= PARAMETER3.Length)
                            {
                                charPosZ = PARAMETER3.Substring(posZ, 1);
                            }
                            await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Char Label: " + charLabelZ + " " + charPosZ);
                        }
                        catch (Exception ex)
                        {
                            financeviewmodel.errorMessage = "ScriptCharZ error: " + ex.Message;
                        }

                        // Force the third input value needed                            
                        string scriptcharZPos = $@"
                            (function() {{
                                function setInputValue(id, value) {{
                                    const el = document.getElementById(id);
                                    if (el) {{
                                        el.value = value;
                                        el.dispatchEvent(new Event('input', {{ bubbles: true }}));
                                        el.dispatchEvent(new Event('change', {{ bubbles: true }}));
                                    }}
                                }}
                                setInputValue('charZPos','{charPosZ}');
                            }})();";

                        await FinanceWebView.ExecuteScriptAsync(scriptcharZPos);

                        // Set up Modal Sniffer TWO
                        // Insert the Modal Sniffer Two
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
                                        window.chrome.webview.postMessage({ status: 'success_modalsniffer2', text: 'Modal appeared' }); 
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
                            })();
                            ");

                        // Continue button!! 
                        // JavaScript code to find the button, scroll into view, enable it, and click
                        string scriptContinue = @"
                                (function() {
                                    const buttons = Array.from(document.querySelectorAll('button'));
                                    const continueButton = buttons.find(b => b.textContent.trim().toLowerCase() === 'continue');
                                    if (continueButton) 
                                    {
                                       continueButton.scrollIntoView({ behavior: 'smooth', block: 'center' });
                                       continueButton.click();
                                       return 'Button clicked';
                                    } 
                                    else 
                                    {
                                        return 'Button not found';
                                    }
                                })();";
                        try
                        {
                            string resultContinue = await FinanceWebView.CoreWebView2.ExecuteScriptAsync(scriptContinue);
                            resultContinue = System.Text.Json.JsonSerializer.Deserialize<string>(resultContinue); // remove extra quotes
                            await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Continue button(1): " + resultContinue);
                        }
                        catch (Exception ex)
                        {
                            await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "ScriptContinue error: " + ex.Message);
                            return;
                        }                        
                        break;
                    case "success_modalsniffer2":
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


                        // Force the third input value needed                            
                        string scriptradioButton = @"
                        (function () {
                            const radioButton = document.getElementById('authenticationPanelPhonesRadio2');

                            if (!radioButton) {
                                return false;
                            }

                            if (radioButton.disabled) {
                                return false;
                            }

                            radioButton.click();
                            return true;
                        })();";

                        await FinanceWebView.ExecuteScriptAsync(scriptradioButton);

                        // JavaScript code to find the button, scroll into view, enable it, and click
                        // Continue button!! 
                        // JavaScript code to find the button, scroll into view, enable it, and click


                        string scriptContinue1 = @"
                                (function() {
                                    const buttons = Array.from(document.querySelectorAll('button'));
                                    const continueButton = buttons.find(b => b.textContent.trim().toLowerCase() === 'continue');
                                    if (continueButton) 
                                    {
                                       continueButton.scrollIntoView({ behavior: 'smooth', block: 'center' });
                                       continueButton.click();
                                       return 'Button clicked';
                                    } 
                                    else 
                                    {
                                        return 'Button not found';
                                    }
                                })();";
                        try
                        {
                            string resultContinue = await FinanceWebView.CoreWebView2.ExecuteScriptAsync(scriptContinue1);
                            resultContinue = System.Text.Json.JsonSerializer.Deserialize<string>(resultContinue); // remove extra quotes
                            await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Continue button(1): " + resultContinue);
                        }
                        catch (Exception ex)
                        {
                            financeviewmodel.errorMessage = "ScriptContinue error: " + ex.Message;
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
                            financeviewmodel.errorMessage = "OneTimePasscode: " + "Incorrect length";
                            await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, financeviewmodel.errorMessage);
                            await Task.CompletedTask;
                        }

                        string scriptSetOtpInput = $@"
                            (function () {{
                                const input = document.getElementById('OTPInput');

                                if (!input) {{
                                    return false;
                                }}

                                if (input.disabled) {{
                                    return false;
                                }}

                                // Set the value
                                input.value = '{oneTimePasscode}';

                                // Trigger input and change events (important for Angular)
                                const inputEvent = new Event('input', {{ bubbles: true }});
                                const changeEvent = new Event('change', {{ bubbles: true }});
                                input.dispatchEvent(inputEvent);
                                input.dispatchEvent(changeEvent);

                                return true;
                            }})();
                            ";

                        
                        // Execute the script in WebView2
                        await FinanceWebView.CoreWebView2.ExecuteScriptAsync(scriptSetOtpInput);

                        string scriptContinue2 = @"
                                (function() {
                                    const buttons = Array.from(document.querySelectorAll('button'));
                                    const continueButton = buttons.find(b => b.textContent.trim().toLowerCase() === 'continue');
                                    if (continueButton) 
                                    {
                                       continueButton.scrollIntoView({ behavior: 'smooth', block: 'center' });
                                       continueButton.click();
                                       return 'Button clicked';
                                    } 
                                    else 
                                    {
                                        return 'Button not found';
                                    }
                                })();";
                        try
                        {
                            string resultContinue = await FinanceWebView.CoreWebView2.ExecuteScriptAsync(scriptContinue2);
                            resultContinue = System.Text.Json.JsonSerializer.Deserialize<string>(resultContinue); // remove extra quotes
                            await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Continue button(2): " + resultContinue);
                        }
                        catch (Exception ex)
                        {
                            financeviewmodel.errorMessage = "ScriptContinue error: " + ex.Message;
                            //await Task.CompletedTask;
                        }
                        break;                    
                    default:
                        await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Failure: " + message1.status);
                        break;
                }
            }
            catch (Exception ex)
            {
                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "JS message: " + ex.Message);
            }
            return;
        }

        internal static async Task<bool> Part2Async(
#if WINFORMS
                                                SmartDashboard.MainProcess components,
                                                RichTextBox textBoxConsole,
#endif
                                                TaskCompletionSource<bool> tcs,MainViewModel ourviewmodel,
                                                FinanceViewModel financeviewmodel,
                                                CoreWebView2 webView,
                                                SmartFinance.Logins loginInfo,
                                                string oneTimePasscode,
                                                short institution_code,
                                                short brand_code,
                                                List<SmartFinance.Transaction_Groups> transaction_groupsFound,
                                                List<SmartFinance.Transaction_Types> transaction_typesFound)
        {
            try
            {
                
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
    }
}
#endif