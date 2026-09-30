using System.Text.RegularExpressions;
using System.Windows;
using System.Threading;

#if WINFORMS
using System.Text.Json;
using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.WinForms;
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
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml;
using Microsoft.Web.WebView2.Core;
using System.Text.Json;
#endif

#if WINUI
using Microsoft.UI.Dispatching;
#endif

#if ANDROID
using Android.Content;
using Android.Provider;
#endif

#if !ANDROID
namespace SmartCubeMobile
{
    internal class NatWestWebView2
    {
        // Well after spending 4? 5? days trying to oversome the fucking Modal
        // popup and the OTP bollocks (and I think I succeeded) the NatWest chimps
        // have now decided that as I'm a 'good boy' they now recognize my WebView2
        // 'device' (!!!! you couldn't make this up) and I no longer have to expect
        // either the Modal OR to intercept and pick up an OTP !!!!
        // So obviously I've left the code in to still test for this, but (sigh)
        // I now login to NatWest without either of these all the time ...
        // I'll ask ChatGPT how I can make it ask for an OTP again, as deleting
        // all the cookies ... ooo I wonder if I have to delete WebView2 cookies?
        // Nope that didn't make any difference (perhaps I have to reboot the Virgin
        // server?) 
        // Anyhow - when I'm logged in, the Chimps don't direct me to a new navigation
        // page so I don't really know where the fuck I am.....
        // Cuppa teat-time tho, Ray.  Well done in any case!

        internal static async Task RunNatWestWebView2(
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
                await FinanceWebView.CoreWebView2.Profile.ClearBrowsingDataAsync(CoreWebView2BrowsingDataKinds.DiskCache);
            }
            catch (Exception ex)
            {
                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Ensure WebView2 failed: " + ex.Message);
                await Task.CompletedTask;
                return;// false;
            }

            financeviewmodel.notificationHandledSource = new TaskCompletionSource<bool>();
            FinanceWebView.CoreWebView2.NewWindowRequested += (sender, args) => args.Handled = true;
            FinanceWebView.CoreWebView2.OpenDevToolsWindow();
            

            int step = 0;
            string logAction = string.Empty;
            string errorMessage = string.Empty;

#if WINFORMS || WPF
            FinanceWebView.CoreWebView2.WebMessageReceived +=
                    new EventHandler<CoreWebView2WebMessageReceivedEventArgs>((s, e) => CoreWebView2_WebMessageReceived(s, e,
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
                                                    transaction_typesFound));
#endif
#if WINUI
            FinanceWebView.CoreWebView2.WebMessageReceived +=
                (s, e) => CoreWebView2_WebMessageReceived(
                    s, e,
                    FinanceWebView,
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
                );
#endif
            FinanceWebView.CoreWebView2.HistoryChanged += async (sender, args) =>
            {
                string newUrl = FinanceWebView.Source.ToString();
                // System.Console.WriteLine($"URL changed: {newUrl}");
                if (newUrl.Contains("security-details/option-1"))
                {
                    try
                    {

                        // Pin 0
                        string jsResultPIN0 = await FinanceWebView.ExecuteScriptAsync(@"
                    (function() {
                        const el = document.getElementById('input_enterthefollowingnumbersfromyourpin-0_a11yLabel');
                        return el ? el.textContent.trim() : null;
                    })();");

                        if (jsResultPIN0 != null)
                        {
                            //System.Console.WriteLine(jsResultPIN0.ToString());


                            //System.Console.WriteLine(jsResultPIN0X.ToString());
                            // Remove surrounding quotes
                            string pin0Text = System.Text.Json.JsonSerializer.Deserialize<string>(jsResultPIN0);

                            // Output: 1st digit of your PIN
                            pin0Text = pin0Text.Replace("digit of your PIN", "").TrimEnd();
                            int pin0 = Convert.ToInt32(pin0Text.Substring(0, 1));
                            string pinDigit0 = Parameter2.Substring(pin0 - 1, 1);
                            await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, pin0Text + " " + pinDigit0);

                            await FinanceWebView.ExecuteScriptAsync($@"
                        (function() {{
                            const input = document.getElementById('input_enterthefollowingnumbersfromyourpin-0');
                            if (input) {{
                                input.focus();
                                input.value = '{pinDigit0}';

                                // Dispatch input-related events to trigger frameworks like Angular/React
                                input.dispatchEvent(new Event('input', {{ bubbles: true, cancelable: true }}));
                                input.dispatchEvent(new Event('change', {{ bubbles: true, cancelable: true }}));
                                input.dispatchEvent(new KeyboardEvent('keydown', {{ key: '{pinDigit0}', bubbles: true }}));
                                input.dispatchEvent(new KeyboardEvent('keyup', {{ key: '{pinDigit0}', bubbles: true }}));
                                input.dispatchEvent(new Event('blur', {{ bubbles: true }}));

                                return 'Input updated';
                            }}
                            return 'Input field not found';
                        }})();");
                        }

                        // Pin 1
                        string jsResultPIN1 = await FinanceWebView.ExecuteScriptAsync(@"
                                (function() {
                                    const el = document.getElementById('input_enterthefollowingnumbersfromyourpin-1_a11yLabel');
                                    return el ? el.textContent.trim() : null;
                                })();");

                        if (jsResultPIN1 != null)
                        {
                            // Remove surrounding quotes
                            string pin1Text = System.Text.Json.JsonSerializer.Deserialize<string>(jsResultPIN1);

                            // Output: 2nd digit of your PIN
                            pin1Text = pin1Text.Replace("digit of your PIN", "").TrimEnd();
                            int pin1 = Convert.ToInt32(pin1Text.Substring(0, 1));
                            string pinDigit1 = Parameter2.Substring(pin1 - 1, 1);
                            await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, pin1Text + " " + pinDigit1);

                            await FinanceWebView.ExecuteScriptAsync($@"
                                    (function() {{
                                        const input = document.getElementById('input_enterthefollowingnumbersfromyourpin-1');
                                        if (input) {{
                                            input.focus();
                                            input.value = '{pinDigit1}';

                                            // Dispatch input-related events to trigger frameworks like Angular/React
                                            input.dispatchEvent(new Event('input', {{ bubbles: true, cancelable: true }}));
                                            input.dispatchEvent(new Event('change', {{ bubbles: true, cancelable: true }}));
                                            input.dispatchEvent(new KeyboardEvent('keydown', {{ key: '{pinDigit1}', bubbles: true }}));
                                            input.dispatchEvent(new KeyboardEvent('keyup', {{ key: '{pinDigit1}', bubbles: true }}));
                                            input.dispatchEvent(new Event('blur', {{ bubbles: true }}));

                                            return 'Input updated';
                                        }}
                                        return 'Input field not found';
                                    }})();");

                        }
                        //ray = 3;
                        string jsResultPIN2 = await FinanceWebView.ExecuteScriptAsync(@"
                                (function() {
                                    const el = document.getElementById('input_enterthefollowingnumbersfromyourpin-2_a11yLabel');
                                    return el ? el.textContent.trim() : null;
                                })();");

                        if (jsResultPIN2 != null)
                        {
                            // Remove surrounding quotes
                            string pin2Text = System.Text.Json.JsonSerializer.Deserialize<string>(jsResultPIN2);

                            // Output: 3rd digit of your PIN
                            pin2Text = pin2Text.Replace("digit of your PIN", "").TrimEnd();
                            int pin2 = Convert.ToInt32(pin2Text.Substring(0, 1));
                            string pinDigit2 = Parameter2.Substring(pin2 - 1, 1);
                            await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, pin2Text + " " + pinDigit2);


                            await FinanceWebView.ExecuteScriptAsync($@"
                                    (function() {{
                                        const input = document.getElementById('input_enterthefollowingnumbersfromyourpin-2');
                                        if (input) {{
                                            input.focus();
                                            input.value = '{pinDigit2}';

                                            // Dispatch input-related events to trigger frameworks like Angular/React
                                            input.dispatchEvent(new Event('input', {{ bubbles: true, cancelable: true }}));
                                            input.dispatchEvent(new Event('change', {{ bubbles: true, cancelable: true }}));
                                            input.dispatchEvent(new KeyboardEvent('keydown', {{ key: '{pinDigit2}', bubbles: true }}));
                                            input.dispatchEvent(new KeyboardEvent('keyup', {{ key: '{pinDigit2}', bubbles: true }}));
                                            input.dispatchEvent(new Event('blur', {{ bubbles: true }}));

                                            return 'Input updated';
                                        }}
                                        return 'Input field not found';
                                    }})();");
                        }

                        // ===================================================

                        // ray = 4;
                        string jsResultPassword0 = await FinanceWebView.ExecuteScriptAsync(@"
                                    (function() {
                                        const el = document.getElementById('input_enterthefollowingcharactersfromyourpassword-0_a11yLabel');
                                        return el ? el.textContent.trim() : null;
                                    })();");

                        if (jsResultPassword0 != null)
                        {
                            // Remove surrounding quotes
                            string password0Text = System.Text.Json.JsonSerializer.Deserialize<string>(jsResultPassword0);

                            // Output: 1st character of your Password
                            password0Text = password0Text.Replace("character of your password", "").TrimEnd();
                            // strip off last 2 (st, nd, rd, th, etc.)
                            password0Text = password0Text.Length >= 2 ? password0Text.Substring(0, password0Text.Length - 2) : "";
                            int len0 = password0Text.Length;
                            int password0 = Convert.ToInt32(password0Text.Substring(0, len0));
                            string passwordChar0 = Parameter3.Substring(password0 - 1, 1);
                            await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, password0Text + " " + passwordChar0);


                            await FinanceWebView.ExecuteScriptAsync($@"
                                    (function() {{
                                        const input = document.getElementById('input_enterthefollowingcharactersfromyourpassword-0');
                                        if (input) {{
                                            input.focus();
                                            input.value = '{passwordChar0}';

                                            // Dispatch input-related events to trigger frameworks like Angular/React
                                            input.dispatchEvent(new Event('input', {{ bubbles: true, cancelable: true }}));
                                            input.dispatchEvent(new Event('change', {{ bubbles: true, cancelable: true }}));
                                            input.dispatchEvent(new KeyboardEvent('keydown', {{ key: '{passwordChar0}', bubbles: true }}));
                                            input.dispatchEvent(new KeyboardEvent('keyup', {{ key: '{passwordChar0}', bubbles: true }}));
                                            input.dispatchEvent(new Event('blur', {{ bubbles: true }}));

                                            return 'Input updated';
                                        }}
                                        return 'Input field not found';
                                    }})();");
                        }
                        string jsResultPassword1 = await FinanceWebView.ExecuteScriptAsync(@"
                                        (function() {
                                            const el = document.getElementById('input_enterthefollowingcharactersfromyourpassword-1_a11yLabel');
                                            return el ? el.textContent.trim() : null;
                                        })();");

                        if (jsResultPassword1 != null)
                        {
                            // Remove surrounding quotes
                            string password1Text = System.Text.Json.JsonSerializer.Deserialize<string>(jsResultPassword1);

                            // Output: 2nd character of your Password
                            password1Text = password1Text.Replace("character of your password", "").TrimEnd();
                            // strip off last 2 (st, nd, rd, th, etc.)
                            password1Text = password1Text.Length >= 2 ? password1Text.Substring(0, password1Text.Length - 2) : "";
                            int len1 = password1Text.Length;
                            int password1 = Convert.ToInt32(password1Text.Substring(0, len1));
                            string passwordChar1 = Parameter3.Substring(password1 - 1, 1);
                            await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, password1Text + " " + passwordChar1);

                            await FinanceWebView.ExecuteScriptAsync($@"
                                    (function() {{
                                        const input = document.getElementById('input_enterthefollowingcharactersfromyourpassword-1');
                                        if (input) {{
                                            input.focus();
                                            input.value = '{passwordChar1}';

                                            // Dispatch input-related events to trigger frameworks like Angular/React
                                            input.dispatchEvent(new Event('input', {{ bubbles: true, cancelable: true }}));
                                            input.dispatchEvent(new Event('change', {{ bubbles: true, cancelable: true }}));
                                            input.dispatchEvent(new KeyboardEvent('keydown', {{ key: '{passwordChar1}', bubbles: true }}));
                                            input.dispatchEvent(new KeyboardEvent('keyup', {{ key: '{passwordChar1}', bubbles: true }}));
                                            input.dispatchEvent(new Event('blur', {{ bubbles: true }}));

                                            return 'Input updated';
                                        }}
                                        return 'Input field not found';
                                    }})();");

                        }
                        string jsResultPassword2 = await FinanceWebView.ExecuteScriptAsync(@"
                                    (function() {
                                        const el = document.getElementById('input_enterthefollowingcharactersfromyourpassword-2_a11yLabel');
                                        return el ? el.textContent.trim() : null;
                                    })();");

                        if (jsResultPassword2 != null)
                        {
                            // Remove surrounding quotes
                            string password2Text = System.Text.Json.JsonSerializer.Deserialize<string>(jsResultPassword2);

                            // Output: 3rd character of your Password
                            password2Text = password2Text.Replace("character of your password", "").TrimEnd();
                            // strip off last 2 (st, nd, rd, th, etc.)
                            password2Text = password2Text.Length >= 2 ? password2Text.Substring(0, password2Text.Length - 2) : "";
                            int len2 = password2Text.Length;
                            int password2 = Convert.ToInt32(password2Text.Substring(0, len2));
                            string passwordChar2 = Parameter3.Substring(password2 - 1, 1);
                            await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, password2Text + " " + passwordChar2);

                            await FinanceWebView.ExecuteScriptAsync($@"
                                    (function() {{
                                        const input = document.getElementById('input_enterthefollowingcharactersfromyourpassword-2');
                                        if (input) {{
                                            input.focus();
                                            input.value = '{passwordChar2}';

                                            // Dispatch input-related events to trigger frameworks like Angular/React
                                            input.dispatchEvent(new Event('input', {{ bubbles: true, cancelable: true }}));
                                            input.dispatchEvent(new Event('change', {{ bubbles: true, cancelable: true }}));
                                            input.dispatchEvent(new KeyboardEvent('keydown', {{ key: '{passwordChar2}', bubbles: true }}));
                                            input.dispatchEvent(new KeyboardEvent('keyup', {{ key: '{passwordChar2}', bubbles: true }}));
                                            input.dispatchEvent(new Event('blur', {{ bubbles: true }}));

                                            return 'Input updated';
                                        }}
                                        return 'Input field not found';
                                    }})();");
                        }
                    }

                    catch (Exception ex)
                    {
                        System.Console.WriteLine(ex.Message.ToString());// + ray.ToString());
                    }

                    // Insert the Modal Sniffer
                    await FinanceWebView.ExecuteScriptAsync(@"
                    (function() {
                    const observer = new MutationObserver(mutations => {
                    const modal = document.querySelector(
                        '.modal.show, .popup, .dialog, .overlay, [role=dialog], [aria-modal=true]'
                    );

                                if (modal)
                                {
                                    console.log('[ModalSniffer] Modal detected: ' + modal.outerHTML.slice(0, 200));

                                    if (window.chrome && window.chrome.webview)
                                    {
                                        window.chrome.webview.postMessage({ status: 'success_modalsniffer', text: 'Modal appeared' }); 
                                    }

                                    // Stop observing after the first match
                                    observer.disconnect();
                                    console.log('[ModalSniffer] Observer disconnected after first detection');
                                }
                            });

                            if (document.body)
                            {
                                observer.observe(document.body, { childList: true, subtree: true });
                                console.log('[ModalSniffer] Initialized and watching for modals...');
                            }
                            else
                            {
                                console.warn('[ModalSniffer] document.body not ready');
                            }
                        })();
                        ");



                    // Try and press Continue
                    string scriptContinue2 = @"
                            (function() {
                                const allElements = document.querySelectorAll('*');
                                for (let el of allElements) {
                                    if (el.textContent && el.textContent.trim() === 'Continue') {
                                        el.click();
                                        return 'Clicked element with text Continue';
                                    }
                                        }
                                return 'No element with text Continue found';
                                    })();
                                    ";

                    try
                    {
                        string resulty = await FinanceWebView.CoreWebView2.ExecuteScriptAsync(scriptContinue2);
                        resulty = System.Text.Json.JsonSerializer.Deserialize<string>(resulty); // remove extra quotes
                        await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Continue2: " + resulty);

                    }
                    catch (Exception ex)
                    {
                        financeviewmodel.errorMessage = "Script error: " + ex.Message;
                        await Task.CompletedTask;
                    }
                }
            };

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
                                    .filter(a => a.textContent.trim().includes('Log in'))
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
                                    System.Console.WriteLine("Found href: " + href);
                                }
                            }
                            catch (Exception ex)
                            {
                                System.Console.WriteLine("Error parsing hrefs: " + ex.Message);
                            }
                            step++;
                            // Its the last href so it appears!!
                            FinanceWebView.CoreWebView2.Navigate(loginHref);
                            break;
                        
                        case 1:
                            string scriptCustomerNo = $@"
                                    (function waitAndFillInput() {{
                                        const inputId = 'input_customerNumber';
                                        const valueToSet = '{Parameter1}';

                                        function triggerAngularInput(input, value) {{
                                            const nativeInputValueSetter = Object.getOwnPropertyDescriptor(window.HTMLInputElement.prototype, 'value').set;
                                            nativeInputValueSetter.call(input, value);
                                            input.dispatchEvent(new Event('input', {{ bubbles: true }}));
                                            input.dispatchEvent(new Event('change', {{ bubbles: true }}));
                                            input.dispatchEvent(new Event('blur', {{ bubbles: true }}));
                                        }}

                                        function setInputValue() {{
                                            const input = document.getElementById(inputId);
                                            if (input) {{
                                                triggerAngularInput(input, valueToSet);
                                            }} else {{
                                                setTimeout(setInputValue, 100);
                                            }}
                                        }}

                                        setInputValue();
                                    }})();
                                    ";


                            await FinanceWebView.ExecuteScriptAsync(scriptCustomerNo);

                            string scriptContinue = @"
                                (function waitAndClickInnerButton() {
                                    function findAndClickButton() {
                                        const buttons = document.querySelectorAll('ibank-button');

                                        for (const btn of buttons) {
                                            const text = btn.textContent.trim();
                                            if (text === 'Continue') {
                                                if (btn.shadowRoot) {
                                                    const shadowBtn = btn.shadowRoot.querySelector('button, div, span');
                                                    if (shadowBtn) {
                                                        shadowBtn.click();
                                                        return;
                                                    }
                                                }

                                                const innerBtn = btn.querySelector('button, div, span');
                                                if (innerBtn) {
                                                    innerBtn.click();
                                                    return;
                                                }

                                                btn.click();
                                                return;
                                            }
                                        }

                                        setTimeout(findAndClickButton, 100);
                                    }

                                    findAndClickButton();
                                })();";
                            await FinanceWebView.CoreWebView2.ExecuteScriptAsync(scriptContinue);

                            step++;
                            break;
                                
                        case 2:
                            await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Step 4: ");

                            if (!financeviewmodel.loggedIn)
                            {
                                // Task completed set in here
                                await SmartBanksV2023.WebDriverDead(FinanceWebView, tcs, ourviewmodel, financeviewmodel);
                                await Task.CompletedTask;
                            }
                            break;
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
                string messageJson = args.WebMessageAsJson;
                WebMessage message1 = JsonSerializer.Deserialize<WebMessage>(messageJson);

                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Status: " + message1.status);

                switch (message1.status)
                {
                    case "success_modalsniffer":                        
                        // Before we tell the Modal we want an OTP,
                        // setup the Notification shit
                        await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Setup OTP listener");

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

                        await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Modal detected via WebMessage!");

                        // Run your modal interaction script here
                        // MY GOD WAS THIS AN ABSOLUTE BITCH OR WHAT????!!!???
                        // Could *NOT* have done this without chatGPT. absolutely not
                        await FinanceWebView.ExecuteScriptAsync(@"
                    (function waitForRadioAndContinue() {
                        function runWhenReady() {
                        console.log('[ModalInteraction] DOM is ready, waiting for radio and continue button...');

                        const maxWait = 10000;
                        const pollInterval = 100;
                        const startTime = Date.now();

                        const interval = setInterval(() => {
                            const radio = document.querySelector('input[type=radio]#mobileNumberConfirmation2-Yes3');

                            let continueButton = null;
                                    const buttons = document.querySelectorAll('button, ibank-button, [role=button]');
                                    for (let btn of buttons)
                                    {
                                        const label = (btn.innerText || btn.textContent || '').trim().toLowerCase();
                                        if (label === 'continue')
                                        {
                                            continueButton = btn;
                                            break;
                                        }
                                    }

                                    if (radio && continueButton)
                                    {
                                        clearInterval(interval);
                                        console.log('[ModalInteraction] ✅ Radio and Continue button found');

                                        radio.scrollIntoView({ behavior: 'smooth', block: 'center' });
                                        radio.checked = true;
                                        radio.dispatchEvent(new Event('input', { bubbles: true }));
                                        radio.dispatchEvent(new Event('change', { bubbles: true }));
                                        console.log('[ModalInteraction] ✅ Radio button selected');

                                        continueButton.click();
                                        console.log('[ModalInteraction] ✅ Continue button clicked');
                                        }

                                        if (Date.now() - startTime > maxWait)
                                        {
                                            clearInterval(interval);
                                            console.log('[ModalInteraction] ❌ Timed out waiting for radio and button');
                                        }
                                    }, pollInterval);
                                }

                                if (document.readyState === 'complete')
                                {
                                    runWhenReady();
                                }
                                else
                                {
                                    window.addEventListener('load', runWhenReady);
                                }
                            })();");

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

                        string setOTPScript = $@"
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

                                // Use native setter so frameworks like React detect the change
                                const nativeInputValueSetter = Object.getOwnPropertyDescriptor(
                                    window.HTMLInputElement.prototype, 'value'
                                ).set;

                                nativeInputValueSetter.call(input, ''); // Clear first

                                for (let i = 0; i < value.length; i++) {{
                                    const char = value[i];

                                    nativeInputValueSetter.call(input, input.value + char); // Append

                                    input.dispatchEvent(new KeyboardEvent('keydown', {{ key: char, bubbles: true }}));
                                    input.dispatchEvent(new KeyboardEvent('keypress', {{ key: char, bubbles: true }}));
                                    input.dispatchEvent(new Event('input', {{ bubbles: true }}));
                                    input.dispatchEvent(new KeyboardEvent('keyup', {{ key: char, bubbles: true }}));
                                }}

                                input.dispatchEvent(new Event('change', {{ bubbles: true }}));
                                input.dispatchEvent(new Event('blur', {{ bubbles: true }}));

                                console.log(`✅ Typed into #$${{input.id}}:`, value);
                                return true;
                            }}

                            const input1 = await waitForElement('smsActivationCode', 10000);

                            if (!input1) {{
                                console.log('❌ OTP input not found');
                                window.chrome.webview.postMessage({{ status: 'failure', text: 'otp_input_not_found' }});
                                return;
                            }}

                            typeIntoField(input1, '{oneTimePasscode}');    
                            console.log('✅ Filled in oneTimePasscode');

                        }})();";


                        string setOtpResult = await FinanceWebView.ExecuteScriptAsync(setOTPScript);


                        //string text = JsonSerializer.Deserialize<string>(setOtpResult);
                        //System.Console.WriteLine(setOtpResult);

                        // 18-Jul2025 07:09
                        // *** This took me A WHOLE DAY TO FIGURE OUT!! ***

                        string scriptConfirm = @"
                                (function waitAndClickInnerButton() {
                                    function findAndClickButton() {
                                        const buttons = document.querySelectorAll('ibank-button');

                                        for (const btn of buttons) {
                                            const text = btn.textContent.trim();
                                            if (text === 'Confirm') {
                                                if (btn.shadowRoot) {
                                                    const shadowBtn = btn.shadowRoot.querySelector('button, div, span');
                                                    if (shadowBtn) {
                                                        shadowBtn.click();
                                                        return;
                                                    }
                                                }

                                                const innerBtn = btn.querySelector('button, div, span');
                                                if (innerBtn) {
                                                    innerBtn.click();
                                                    return;
                                                }

                                                btn.click();
                                                return;
                                            }
                                        }

                                        setTimeout(findAndClickButton, 100);
                                    }

                                    findAndClickButton();
                                })();";
                        await FinanceWebView.CoreWebView2.ExecuteScriptAsync(scriptConfirm);
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
                                                List<SmartFinance.Transaction_Types> transaction_typesFound,
                                                string UDPRN)
        {
            try
            {
                if (string.IsNullOrEmpty(oneTimePasscode))
                {
                    await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Notification content: Empty");
                    financeviewmodel.errorMessage = "Notification content is empty";
                    return false;
                }

                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Inserting: " + oneTimePasscode);

                string setOtpScript = $@"
                    (function() {{
                        const input = document.querySelector('input[name=""OneTimePasscode""]');
                        if(input) {{
                            input.value = '{oneTimePasscode}';
                            return 'ok';
                        }} else {{
                            return 'not found';
                        }}
                    }})();";


                string setOtpResult = await webView.ExecuteScriptAsync(setOtpScript);
                if (!setOtpResult.Contains("ok"))
                {
                    await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Failed to set OTP input field");
                    financeviewmodel.errorMessage = "Failed to set OTP input field";
                    return false;
                }

                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, $"Input Field: {oneTimePasscode} filled");

                financeviewmodel.loggedIn = false;

                string waitForLoginAndClick = @"
            new Promise(resolve => {
                const checkExist = setInterval(() => {
                    const buttons = Array.from(document.getElementsByTagName('button'));
                    const loginBtn = buttons.find(b => b.innerText.trim().toLowerCase() === 'log in');
                    if (loginBtn) {
                        loginBtn.click();
                        clearInterval(checkExist);
                        resolve('clicked');
                    }
                }, 100);
                setTimeout(() => { clearInterval(checkExist); resolve('not found'); }, 10000);
            });";

                string clickResult = await webView.ExecuteScriptAsync(waitForLoginAndClick);
                //await WaitForNavigationAsync(webView);
                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Log in button: Clicked");

                financeviewmodel.loggedIn = true;
                financeviewmodel.accountItems = new List<FinanceViewModel.AccountItem>();
                financeviewmodel.currentUrl = webView.Source?.ToString() ?? "";

                string getWelcomeScript = @"
                                        (function() {
                                            const welcomeElem = document.getElementById('welcome-message');
                                            return welcomeElem && welcomeElem.innerText ? welcomeElem.innerText : '';
                                        })();";

                string welcomeTextRaw = await webView.ExecuteScriptAsync(getWelcomeScript);
                string welcomeText = System.Text.Json.JsonDocument.Parse(welcomeTextRaw).RootElement.GetString() ?? "";
                string owner = welcomeText.Replace("Welcome back,", "").Trim();

                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Owner: " + loginInfo.OWNER);

                UDPRN = "";

                
                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Contact no.: " + loginInfo.CONTACT_PHONENO);
                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Email address: " + loginInfo.CONTACT_EMAIL);

                financeviewmodel.account_id = 0;

                

                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Transactions inserted: " + financeviewmodel.transactions_count);
                return true;
            }
            catch (Exception ex)
            {
                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Exception: " + ex.Message);
                financeviewmodel.errorMessage = "Exception occurred: " + ex.Message;
                tcs.TrySetResult(true);
                return false;
            }
        }
    }
}
#endif