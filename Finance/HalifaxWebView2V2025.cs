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
using System.Text.Json;
using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.Wpf;
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
    internal class HalifaxWebView2
    {
        //internal static TypedEventHandler<UserNotificationListener, UserNotificationChangedEventArgs> notificationHandler;

        //internal static UserNotificationListener notificationListener = UserNotificationListener.Current;

        internal static async Task RunHalifaxWebView2(
                                                    WebView2 FinanceWebView,
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
                                                    string Parameter3)
        {
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
            FinanceWebView.CoreWebView2.OpenDevToolsWindow();

            int step = 0;
            string logAction = string.Empty;
            string errorMessage = string.Empty;

#if WINFORMS || WPF
            FinanceWebView.CoreWebView2.WebMessageReceived +=
                    new EventHandler<CoreWebView2WebMessageReceivedEventArgs>((s, e) => CoreWebView2_WebMessageReceived(s, e,
                                                    FinanceWebView,
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
                                    .filter(a => a.textContent.trim().includes('Sign in'))
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
                            // Force the two input values needed
                            string script = @"
                                (function() {
                                    function setInputValue(id, value) {
                                        const el = document.getElementById(id);
                                        if (el) {
                                            el.value = value;
                                            el.dispatchEvent(new Event('input', { bubbles: true }));
                                            el.dispatchEvent(new Event('change', { bubbles: true }));
                                        }
                                    }
                                    setInputValue('frmLogin:strCustomerLogin_userID','" + Parameter1 + "');" +
                                    "setInputValue('frmLogin:strCustomerLogin_pwd','" + Parameter2 + "');" +
                                "})();";

                            await FinanceWebView.ExecuteScriptAsync(script);


                            // JavaScript code to find the button, scroll into view, enable it, and click
                            string scriptLogOn = @"
                              (function() {
                                const btn = document.getElementById('frmLogin:btnLogin1');
                                if (btn) {
                                  btn.scrollIntoView({ behavior: 'smooth', block: 'center' });
                                  btn.click();           // Click the button
                                  return 'Button clicked';
                                } else {
                                  return 'Button not found';
                                }
                              })();
                            ";
                            try
                            {
                                string resultx = await FinanceWebView.CoreWebView2.ExecuteScriptAsync(scriptLogOn);
                                resultx = System.Text.Json.JsonSerializer.Deserialize<string>(resultx); // remove extra quotes
                                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Log on: " + resultx);
                                
                            }
                            catch (Exception ex)
                            {
                                financeviewmodel.errorMessage = "Script error: " + ex.Message;
                                await Task.CompletedTask;
                            }
                            step++;
                            break;
                        case 2:
                            await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Step 2: ");

                            // Find the memorable characters
                            string choice = "abcdefghijklmnopqrstuvwxyz0123456789";


                            string jsCode1 = @"
                            (() => {
                                const label = document.querySelector('label[for=""frmentermemorableinformation1:strEnterMemorableInformation_memInfo1""]');
                                return label ? label.textContent : null;
                            })()
                            ";
                            string result1 = await FinanceWebView.CoreWebView2.ExecuteScriptAsync(jsCode1);
                            // Clean JSON quotes
                            string labelText1 = result1.Replace("Character", string.Empty).Trim('"');
                            labelText1 = labelText1.Replace(" ", "");
                            labelText1 = labelText1.Replace(":", ""); 
                            labelText1 = Regex.Replace(labelText1, @"\s+", "");
                            string jsCode2 = @"
                            (() => {
                                const label = document.querySelector('label[for=""frmentermemorableinformation1:strEnterMemorableInformation_memInfo2""]');
                                return label ? label.textContent : null;
                            })()
                            ";
                            string result2 = await FinanceWebView.CoreWebView2.ExecuteScriptAsync(jsCode2);
                            // Clean JSON quotes
                            string labelText2 = result2.Replace("Character", string.Empty).Trim('"');
                            labelText2 = labelText2.Replace(" ", "");
                            labelText2 = labelText2.Replace(":", ""); 
                            labelText2 = Regex.Replace(labelText2, @"\s+", "");
                            string jsCode3 = @"
                            (() => {
                                const label = document.querySelector('label[for=""frmentermemorableinformation1:strEnterMemorableInformation_memInfo3""]');
                                return label ? label.textContent : null;
                            })()
                            ";
                            string result3 = await FinanceWebView.CoreWebView2.ExecuteScriptAsync(jsCode3);
                            // Clean JSON quotes
                            string labelText3 = result3.Replace("Character", string.Empty).Trim('"');
                            labelText3 = labelText3.Replace(" ", "");
                            labelText3 = labelText3.Replace(":", ""); 
                            labelText3 = Regex.Replace(labelText3, @"\s+", "");


                            System.Console.WriteLine("Label text: " + labelText1 + " " + labelText2 + " " + labelText3);

                            await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, labelText1 + " " + labelText2 + " " + labelText3);

                            string char1 = Parameter3.Substring(Convert.ToInt32(labelText1) - 1, 1);
                            // Seems the choices don't start from 0 as you would expect
                            int index1 = choice.IndexOf(char1) + 1;
                            await FinanceWebView.ExecuteScriptAsync(@"
                            document.getElementById('frmentermemorableinformation1:strEnterMemorableInformation_memInfo1').selectedIndex = " + index1 + @";
                            document.getElementById('frmentermemorableinformation1:strEnterMemorableInformation_memInfo1').dispatchEvent(new Event('change'));
                            ");

                            string char2 = Parameter3.Substring(Convert.ToInt32(labelText2) - 1, 1);
                            // Seems the choices don't start from 0 as you would expect
                            int index2 = choice.IndexOf(char2) + 1; 
                            await FinanceWebView.ExecuteScriptAsync(@"
                            document.getElementById('frmentermemorableinformation1:strEnterMemorableInformation_memInfo2').selectedIndex = " + index2 + @";
                            document.getElementById('frmentermemorableinformation1:strEnterMemorableInformation_memInfo2').dispatchEvent(new Event('change'));
                            ");

                            string char3 = Parameter3.Substring(Convert.ToInt32(labelText3) - 1, 1);
                            // Seems the choices don't start from 0 as you would expect
                            int index3 = choice.IndexOf(char3) + 1;
                            await FinanceWebView.ExecuteScriptAsync(@"
                            document.getElementById('frmentermemorableinformation1:strEnterMemorableInformation_memInfo3').selectedIndex = " + index3 + @";
                            document.getElementById('frmentermemorableinformation1:strEnterMemorableInformation_memInfo3').dispatchEvent(new Event('change'));
                            ");

                            // JavaScript code to find the button, scroll into view, enable it, and click
                            string scriptContinue = @"
                              (function() {
                                const btn = document.getElementById('frmentermemorableinformation1:btnContinue');
                                if (btn) {
                                  btn.scrollIntoView({ behavior: 'smooth', block: 'center' });
                                  btn.click();           // Click the button
                                  return 'Button clicked';
                                } else {
                                  return 'Button not found';
                                }
                              })();
                            ";
                            try
                            {
                                string resulty = await FinanceWebView.CoreWebView2.ExecuteScriptAsync(scriptContinue);
                                //resulty = System.Text.Json.JsonSerializer.Deserialize<string>(resulty); // remove extra quotes
                                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Continue: " + resulty);

                            }
                            catch (Exception ex)
                            {
                                financeviewmodel.errorMessage = "Script error: " + ex.Message;
                                await Task.CompletedTask;
                            }
                            step++;
                            break;
                        case 3:
                            await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Step 3: ");

                            SmartBanksV2023.SetupOtpListener(
                                            FinanceWebView,

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
                            // JavaScript code to find the button, scroll into view, enable it, and click



                            
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
                            })();
                            ";

                            try
                            {
                                string resultyy = await FinanceWebView.CoreWebView2.ExecuteScriptAsync(scriptGetOTP);
                                //resulty = System.Text.Json.JsonSerializer.Deserialize<string>(resulty); // remove extra quotes
                                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Continue: " + resultyy);

                            }
                            catch (Exception ex)
                            {
                                financeviewmodel.errorMessage = "Script error: " + ex.Message;
                                await Task.CompletedTask;
                            }
                            step++;
                            break;                            
                        case 4:
                            await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Step 4: ");

                            if (!financeviewmodel.loggedIn)
                            {
                                // Task completed set in here
                                FinanceWebView = new WebView2();
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
                                            WebView2 FinanceWebView,
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
                                            List<SmartFinance.Transaction_Types> transaction_typesFound)
        {
            try
            {
                string messageJson = args.WebMessageAsJson;
                WebMessage message1 = JsonSerializer.Deserialize<WebMessage>(messageJson);

                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Status: " + message1.status);

                switch (message1.status)
                {
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

                                const input1 = await waitForElement('passcode', 10000);

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

                        string scriptClickContinueButton = @"
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
                            })();
                            ";

                        try
                        {
                            string resultyy = await FinanceWebView.CoreWebView2.ExecuteScriptAsync(scriptClickContinueButton);
                            //resulty = System.Text.Json.JsonSerializer.Deserialize<string>(resulty); // remove extra quotes
                            await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Continue: " + resultyy);
                        }
                        catch (Exception ex)
                        {
                            financeviewmodel.errorMessage = "Script error: " + ex.Message;
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
                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "JS message: " + ex.Message);

            }
            return;
        }

        internal static async Task<bool> Part2Async(
#if WINFORMS
                                                SmartDashboard.MainProcess components,
                                                RichTextBox textBoxConsole,
#endif
                                                TaskCompletionSource<bool> tcs,
                                                MainViewModel ourviewmodel,
                                                FinanceViewModel financeviewmodel,
                                                WebView2 FinanceWebView,
                                                SmartFinance.Logins loginInfo,
                                                string oneTimePasscode,
                                                short institution_code,
                                                short brand_code,
                                                List<SmartFinance.Transaction_Groups> transaction_groupsFound,
                                                List<SmartFinance.Transaction_Types> transaction_typesFound)
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


                string setOtpResult = await FinanceWebView.ExecuteScriptAsync(setOtpScript);
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

                string clickResult = await FinanceWebView.ExecuteScriptAsync(waitForLoginAndClick);
                //await WaitForNavigationAsync(webView);
                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Log in button: Clicked");

                financeviewmodel.loggedIn = true;
                financeviewmodel.accountItems = new List<FinanceViewModel.AccountItem>();
                financeviewmodel.currentUrl = FinanceWebView.Source?.ToString() ?? "";

                string getWelcomeScript = @"
                                        (function() {
                                            const welcomeElem = document.getElementById('welcome-message');
                                            return welcomeElem && welcomeElem.innerText ? welcomeElem.innerText : '';
                                        })();";

                string welcomeTextRaw = await FinanceWebView.ExecuteScriptAsync(getWelcomeScript);
                string welcomeText = System.Text.Json.JsonDocument.Parse(welcomeTextRaw).RootElement.GetString() ?? "";
                string owner = welcomeText.Replace("Welcome back,", "").Trim();

                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Owner: " + loginInfo.OWNER);

                //financeviewmodel.UDPRN = "";

                
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