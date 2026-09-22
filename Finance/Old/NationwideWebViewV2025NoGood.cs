
using System.Text.RegularExpressions;
using System.Windows;

using iText.Layout.Element;
using iText.Commons.Actions.Contexts;
using System.Runtime.InteropServices;

#if WINFORMS
using SmartDashboard;
using System.Windows.Threading;
using static SmartCubeMobile.MainViewModel;
using Windows.UI.Notifications.Management;
using Windows.UI.Notifications;
#endif

#if WPF
using Windows.UI.Notifications.Management;
using Windows.UI.Notifications;
using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.Wpf;
using Newtonsoft.Json;
using static iText.Forms.PdfSigFieldLock;
using static SmartCubeMobile.SmartFinance;
#endif

#if UWP || WINUI
using System.Threading.Tasks;
using System.Collections.Generic;
using System;
using System.Threading;
using System.Linq;
using Windows.UI.Core;
using Windows.UI.Notifications.Management;
using Windows.UI.Notifications;
#endif

#if ANDROID
using Android.App;
using Android.OS;
using Android.Webkit;
using Android.Widget;
using System;
using System.Timers;
using System.Threading.Tasks;
using Android.Views;
using Xamarin.KotlinX.Coroutines;
#endif

#if ANDROID
namespace SmartCubeMobile
{
    internal class NationwideWebView2V2025
    {
        internal static WebView webView;
        System.Timers.Timer otpTimer;
        internal static int step = 0;

        string loginUrl = "https://nationwide.co.uk";
        static string customerNumber = "4293636201";
        static string dobDay = "19";
        static string dobMonth = "September";
        static string dobYear = "1948";

        internal class CustomWebViewClient : WebViewClient
        {
            private readonly string _expectedUrl;
            private readonly TaskCompletionSource<bool> _tcs;

            public CustomWebViewClient(string expectedUrl, TaskCompletionSource<bool> tcs)
            {
                _expectedUrl = expectedUrl;
                _tcs = tcs;
            }

            public override void OnPageFinished(WebView view, string url)
            {
                if (step == 0)
                {
                    // Find login link and navigate
                    string js = @"
                    (function() {
                        var el = document.querySelector('[class^=""LoginLinks__LoginLink""]');
                        return el ? el.href : null;
                    })();";

                    //view.EvaluateJavascript(js, href =>
                    //{
                    //    href = href?.Trim('"');
                    //    if (!string.IsNullOrEmpty(href))
                    //    {
                    //        step = 1;
                    //        view.LoadUrl(href);
                    //    }
                    //});
                }
                else if (step == 1)
                {
                    // Fill login form and click Continue
                    string day = dobDay;
                    string month = dobMonth;
                    string year = dobYear;

                    string js = $@"
                    document.querySelector('input[name=""CustomerNumber""]').value = '{customerNumber}';
                    document.querySelector('input[name=""DateOfBirthDay""]').value = '{day}';
                    document.querySelector('input[name=""DateOfBirthYear""]').value = '{year}';
                    var select = document.querySelector('select[name=""DateOfBirthMonth""]');
                    if (select) {{
                        select.value = '{month}';
                        select.dispatchEvent(new Event('change', {{ bubbles: true }}));
                    }}
                    var btns = Array.from(document.querySelectorAll('button'));
                    var btn = btns.find(b => b.textContent.trim().toLowerCase() === 'continue');
                    if (btn) btn.click();";

                    view.EvaluateJavascript(js, null);
                    step = 2;
                }
                else if (step == 2)
                {
                    // Simulate passcode input and clicking Continue again
                    string js = @"
                    var btns = Array.from(document.querySelectorAll('button'));
                    var btn = btns.find(b => b.textContent.trim().toLowerCase() === 'continue');
                    if (btn) btn.click();";

                    view.EvaluateJavascript(js, null);
                    //Toast.MakeText(this, "Passcode handled (simulated). Waiting for OTP...", ToastLength.Short).Show();
                    step = 3;
                }

            }

            public override void OnReceivedError(WebView view, IWebResourceRequest request, WebResourceError error)
            {
                base.OnReceivedError(view, request, error);
                _tcs.TrySetResult(false); // or .TrySetException(error) if you prefer
            }
        }
        internal static Task<bool> WaitForPageLoadAsync(WebView webView, string expectedUrl = null)
        {
            var tcs = new TaskCompletionSource<bool>();

            WebViewClient customClient = new CustomWebViewClient(expectedUrl, tcs);
            webView.SetWebViewClient(customClient);

            return tcs.Task;
        }
        internal static async Task<bool> RunNationwideWebView2(

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

            webView = new WebView(ourviewmodel.context);
            webView.Settings.JavaScriptEnabled = true;
            webView.Visibility = ViewStates.Invisible;

            WebViewClient abc = new WebViewClient();
            webView.SetWebViewClient(abc);
            webView.Settings.DomStorageEnabled = true;
            webView.SetWebChromeClient(new WebChromeClient());
            //webView.SetWebViewClient(new MyWebViewClient(OnPageFinished));

            webView.LoadUrl("https://www.nationwide.co.uk/");



            financeviewmodel.notificationHandledSource = new TaskCompletionSource<bool>();

            //webView.WebViewClient.NewWindowRequested += (sender, args) =>
            //{
            //    args.Handled = true; // Block the popup
            //};

            // Extract DOB parts
            string Day = Parameter2.Substring(0, 2);
            string Month = SmartParametersV2016.months[Convert.ToInt16(Parameter2.Substring(2, 2)) - 1];
            string Year = Parameter2.Substring(4, 4);

            await WaitForPageLoadAsync(webView, "nationwide.co.uk");
            // Wait for the page to load and inject the script
            
            //webView.NavigationCompleted += async (s, args) =>
            //{
            //    try
            //    {
            //        // 1. Find login link and follow it
            //        string jsGetLoginHref = @"
            //                            (function() {
            //                                const el = document.querySelector('[class^=""LoginLinks__LoginLink""]');
            //                                return el ? el.href : null;
            //                            })();";
            //        string loginHrefJson = await webView.ExecuteScriptAsync(jsGetLoginHref);
            //        string loginHref = System.Text.Json.JsonSerializer.Deserialize<string>(loginHrefJson);

            //        if (!string.IsNullOrEmpty(loginHref))
            //        {
            //            webView.CoreWebView2.Navigate(loginHref);
            //        }
            //    }
            //    catch (Exception ex)
            //    {
            //        MessageBox.Show("Navigation failed: " + ex.Message);
            //    }
            //};

            string logAction;
            string errorMessage;
            // After login page

            await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Retrieved: " + startUrl);

            //webView.CoreWebView2.NavigationCompleted += async (s2, args2) =>
            //{
            //    try
            //    {
            //        // 2. Fill inputs
            //        await webView.ExecuteScriptAsync("document.querySelector('input[name=\"CustomerNumber\"]').value = '4293636201';");
            //        await webView.ExecuteScriptAsync("document.querySelector('input[name=\"DateOfBirthDay\"]').value = '19';");
            //        await webView.ExecuteScriptAsync("document.querySelector('input[name=\"DateOfBirthYear\"]').value = '1948';");
            //        //await MyWebView.ExecuteScriptAsync("document.querySelector('select[name=\"DateOfBirthMonth\"]').value = 'September';");

            //        await webView.ExecuteScriptAsync(@"
            //                                        (function() {
            //                                            const select = document.querySelector('select[name=""DateOfBirthMonth""]');
            //                                            if (select) {
            //                                                select.value = 'September';  // or whatever month you want
            //                                                // Dispatch change event so any JS listeners notice
            //                                                const event = new Event('change', { bubbles: true });
            //                                                select.dispatchEvent(event);
            //                                            } else {
            //                                                console.log('Month dropdown not found');
            //                                            }
            //                                        })();
            //                                    ");

            //        await webView.ExecuteScriptAsync(@"
            //                (function() {
            //                    // Find all buttons
            //                    const buttons = Array.from(document.querySelectorAll('button'));
            //                    // Find button(s) with text containing 'Continue' (case-insensitive)
            //                    const continueButton = buttons.find(b => b.textContent.trim().toLowerCase() === 'continue');
            //                    if (continueButton) {
            //                        continueButton.click();
            //                    } else {
            //                        console.log('Continue button not found');
            //                    }
            //                })();
            //            ");



            //        // 3. Click the second Continue button
            //        //string clickContinue = @"
            //        //                        (function() {
            //        //                            const buttons = Array.from(document.querySelectorAll('button')).filter(b => b.innerText.includes('Continue'));
            //        //                            if (buttons.length >= 2) buttons[1].click();
            //        //                        })();";
            //        //await MyWebView.ExecuteScriptAsync(clickContinue);
            //    }
            //    catch (Exception ex)
            //    {
            //        MessageBox.Show("Form interaction failed: " + ex.Message);

            //    }


            //    await WaitForNavigationAsync(webView.CoreWebView2);
            //    if (!await SelectRadioButtonAsync(webView.CoreWebView2,
            //        la => logAction = la))
            //    {
            //        return;
            //    }

            //    if (!await NationwideFindPasscodesAsync(

            //                                webView.CoreWebView2,
            //                                "750504",
            //                                la => logAction = la,
            //                                em => errorMessage = em))
            //    {

            //        await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Problem with Passcodes: ");
            //        return;
            //    }

            //    await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Passcodes: Filled");

            //    //await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Filled: Customer, Day and Year");





            //    await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Continue(1) button clicked");




            //    await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Filled: Dropdown " + Month);


            //    // At this point we've found - possibly - the Continue
            //    // button which, when pressed will get Nationwide to send
            //    // us an SMSOTP.  THEREFORE we have to be able to receive it
            //    // and to this effect we create the event to listen for it:

            //    // Make up a key string
            //    string key = $"Timer_{financeviewmodel.timerIdCounter++}";
            //    // Add a two-minute timer
            //    AddWaitTimer(ourviewmodel, financeviewmodel, key, SmartParametersV2016.serverTimeoutSecs,
            //        onTick: k =>
            //        {
            //            int remaining = GetRemainingSeconds(financeviewmodel, k);
            //            //label.Text = $"{k}: {remaining} second(s) remaining";
            //        },
            //        onComplete: async (k, reason) =>
            //        {
            //            // Should only ever come here for normal timeout
            //            string Text = $"{k}: {(reason == MainViewModel.TimerFinishReason.Completed ? "Finished" : "Cancelled")}";
            //            if (Text.Contains("Finished"))
            //            {
            //                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Completed: " + Text);

            //                // Shutdown - we timed out without an SMSOTP
            //                if (await WebDriverDead(ourviewmodel, financeviewmodel, webView.CoreWebView2))
            //                // Mark the task as complete so the calling method continues
            //                {
            //                    financeviewmodel.notificationHandledSource.TrySetResult(true);
            //                }
            //                return;
            //            }
            //            return;
            //        }
            //    );
            //    ourviewmodel.notificationListener.NotificationChanged += (sender, e) => Application.Current.Dispatcher.Invoke(() =>
            //    {
            //                                                                                                                ListenerNotificationChanged(sender, e,
            //                                                                                                                ourviewmodel,
            //                                                                                                                financeviewmodel,
            //                                                                                                                webView.CoreWebView2,
            //                                                                                                                key,
            //                                                                                                                loginInfo,
            //                                                                                                                notificationTitle,
            //                                                                                                                notificationPrefix,
            //                                                                                                                notificationTagLength,
            //                                                                                                                institution_code,
            //                                                                                                                brand_code,
            //                                                                                                                transaction_groupsFound,
            //                                                                                                                transaction_typesFound);
            //        return Task.CompletedTask;
            //    });


            //    await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Handler: Created");

            //    // I will forever be in the debt of BART WOJTALA whose solution at
            //    // https://stackoverflow.com/questions/11908249/debugging-element-is-not-clickable-at-point-error
            //    // saved my ass, my bacon and my sanity to allow me
            //    // to 'click' through this last button and get this entire
            //    // 15-year project back on track.  Thanks a million, Bart
            //    //

            //    // And press the Button to say 'Send me an OTP' please
            //    await webView.ExecuteScriptAsync(@"
            //                (function() {
            //                    // Find all buttons
            //                    const buttons = Array.from(document.querySelectorAll('button'));
            //                    // Find button(s) with text containing 'Continue' (case-insensitive)
            //                    const continueButton = buttons.find(b => b.textContent.trim().toLowerCase() === 'continue');
            //                    if (continueButton) {
            //                        continueButton.click();
            //                    } else {
            //                        console.log('Continue button not found');
            //                    }
            //                })();
            //            ");

            //    await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Continue(2) button clicked");


            //    // Wait for something indicating successful login / trigger

            //    await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Waiting for a notification...");

            //    // Keep the User informed

            //    await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Returning...");

            //    return;
            //};
            return true;
        }

#if WPF || UWP || WINUI
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

        internal static async void ListenerNotificationChanged(
                                UserNotificationListener sender,
                                UserNotificationChangedEventArgs args,

                                MainViewModel ourviewmodel,
                                FinanceViewModel financeviewmodel,
                                CoreWebView2 webView,
                                string key,
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
                CancelTimer(financeviewmodel, key);

                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Handler Event: Delivered");

                if (!await SmartRoutinesV2018.CheckTrace(ourviewmodel, financeviewmodel.financeToken, institution_code, brand_code, "Handler: event delivered"))
                {
                    return;
                }
                var notifs = await sender.GetNotificationsAsync(NotificationKinds.Toast);
                var notifsSorted = notifs.OrderByDescending(n => n.CreationTime).ToList();

                if (notifsSorted.Count == 0)
                {

                    await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "No User Notifications found");

                    financeviewmodel.errorMessage = "No User Notifications found";
                    return;
                }

                foreach (var notif in notifsSorted)
                {
                    var toastBinding = notif.Notification.Visual.GetBinding(KnownNotificationBindings.ToastGeneric);
                    if (toastBinding == null) continue;

                    var textElements = toastBinding.GetTextElements();
                    string titleText = textElements.FirstOrDefault()?.Text;
                    if (notificationTitle != titleText) continue;


                    await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Found: " + titleText);


                    string bodyText = string.Join("\n", textElements.Skip(1).Select(t => t.Text));
                    if (!bodyText.Contains(notificationPrefix)) continue;

                    // Unsubscribe listener to avoid duplicate processing
                    ourviewmodel.notificationListener.NotificationChanged -= null;

                    await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Handler Event: Removed");


                    bodyText = bodyText.Replace(notificationPrefix, "").Trim();
                    if (bodyText.Length <= 6) continue;

                    string oneTimePasscode = bodyText.Substring(0, notificationTagLength);

                    await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "OneTimePasscode: " + oneTimePasscode);


                    // === Start WebView2 login & account process ===
                    bool part2Success = await Part2Async(

                                                        ourviewmodel,
                                                        financeviewmodel,
                                                       //baseUrl: "", // You might want to pass this into the method!
                                                        webView,
                                                        loginInfo,
                                                        oneTimePasscode,
                                                        institution_code,
                                                        brand_code,
                                                        transaction_groupsFound,
                                                        transaction_typesFound
                                                        //msg => SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, msg),
                                                        //err => financeviewmodel.errorMessage = err
                    );

                    if (!part2Success)
                    {
                        return;
                    }
                    bool part3Success = await Part3Async(

                                                        ourviewmodel,
                                                        financeviewmodel,
                                                        webView);

                    if (!part3Success)
                        return;

                    bool dead = await WebDriverDead(ourviewmodel, financeviewmodel, webView);
                    if (dead) break;

                    break; // only process one
                }
            }
            catch (Exception ex)
            {
                financeviewmodel.errorMessage = ex.Message;

                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Error: " + ex.Message);

            }
        }


        internal static async Task<bool> Part2Async(
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
            if (string.IsNullOrEmpty(oneTimePasscode))
            {
                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Notification content: Empty");
                financeviewmodel.errorMessage = "Notification content is empty";
                return false;
            }

            try
            {
                

                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Inserting: " + oneTimePasscode);

                // Set OTP value
                string setOtpScript = $@"
                                    (function() {{
                                        const input = document.querySelector('input[name=""OneTimePasscode""]');
                                        if(input) {{
                                            input.value = '{oneTimePasscode}';
                                            return 'ok';
                                        }} else {{
                                            return 'not found';
                                        }}
                                    }})();
                                ";
                var setOtpResult = await webView.ExecuteScriptAsync(setOtpScript);
                if (!setOtpResult.Contains("ok"))
                {
                    financeviewmodel.errorMessage = "Failed to set OTP input field.";
                    return false;
                }

                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, $"Input Field: {oneTimePasscode} filled");

                financeviewmodel.loggedIn = false;

                // Wait and click Log in button
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
                                            });
                                        ";
                string clickResult = await webView.ExecuteScriptAsync(waitForLoginAndClick);
                await WaitForNavigationAsync(webView);
                Console.WriteLine(clickResult);
                //if (!clickResult.Contains("clicked"))
                //{
                //    await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Can't find Login button");
                //    return false;
                //}

                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Log in button: Clicked");

                financeviewmodel.loggedIn = true;
                financeviewmodel.accountItems = new List<FinanceViewModel.AccountItem>();

                financeviewmodel.currentUrl = webView.Source?.ToString() ?? "";

                // Get welcome message text and parse owner
                string getWelcomeScript = @"
                                            (function() {
                                                const welcomeElem = document.getElementById('welcome-message');
                                                return welcomeElem && welcomeElem.innerText ? welcomeElem.innerText : '';
                                            })();
                                        ";
                string welcomeTextRaw = await webView.ExecuteScriptAsync(getWelcomeScript);
                string welcomeText = System.Text.Json.JsonDocument.Parse(welcomeTextRaw).RootElement.GetString() ?? "";
                string owner = welcomeText.Replace("Welcome back,", "").Trim();
                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Owner: " + loginInfo.OWNER);

                financeviewmodel.UDPRN = "";

                if (!await FindAddressAsync(
                        ourviewmodel,
                        financeviewmodel,
                        webView,
                        loginInfo,
                        owner))
                {
                    await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Find address: Failed");
                    return false;
                }
                else
                {
                    await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Contact no.: " + loginInfo.CONTACT_PHONENO);
                    await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Email address: " + loginInfo.CONTACT_EMAIL);
                }

                financeviewmodel.account_id = 0;
                //financeviewmodel.AccountLinks = new List<KeyValuePair<string, string>>();

                if (!await NationwideBuildAccountList(
                        ourviewmodel,
                        financeviewmodel,
                        webView,
                        institution_code,
                        brand_code,
                        financeviewmodel.UDPRN,
                        transaction_groupsFound,
                        transaction_typesFound))
                {
                    await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, financeviewmodel.errorMessage);
                    return false;
                }

                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Transactions inserted: " + financeviewmodel.transactions_count);
            }
            catch (Exception ex)
            {
                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Exception: " + ex.Message);
                financeviewmodel.errorMessage = "Exception occurred: " + ex.Message;
                return false;
            }

            return true;
        }

        internal static async Task<bool> Part3Async(
                                                    MainViewModel ourviewmodel,
                                                    FinanceViewModel financeviewmodel,
                                                    CoreWebView2 webView)
        {
            try
            {
                var tcsNavigation = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);

                void NavigationCompletedHandler(object sender, Microsoft.Web.WebView2.Core.CoreWebView2NavigationCompletedEventArgs e)
                {
                    tcsNavigation.TrySetResult(e.IsSuccess);
                    webView.NavigationCompleted -= NavigationCompletedHandler;
                }

                webView.NavigationCompleted += NavigationCompletedHandler;
                webView.Navigate(financeviewmodel.currentUrl);

                // Wait for navigation to complete (no delay, purely event based)
                bool success = await tcsNavigation.Task;
                if (!success)
                {
                    financeviewmodel.errorMessage = "Page navigation failed";
                    await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Page navigation failed");
                    return false;
                }

                // JavaScript to find and click logout button with timeout (all inside JS, no Task.Delay here)
                string jsScript = @"
            new Promise((resolve, reject) => {
                let attempts = 0;
                const maxAttempts = 100;
                const interval = setInterval(() => {
                    const logoutBtn = document.querySelector('.log-out-link');
                    if (logoutBtn) {
                        logoutBtn.click();
                        clearInterval(interval);
                        resolve('clicked');
                    } else if (++attempts >= maxAttempts) {
                        clearInterval(interval);
                        resolve('not found');
                    }
                }, 100);
            });
        ";

                string result = await webView.ExecuteScriptAsync(jsScript);
                if (!result.Contains("clicked"))
                {
                    financeviewmodel.errorMessage = "Couldn't find logout button";
                    await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "No buttons found");
                    return false;
                }

                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Logout button: Clicked");
                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "WebView2: Dead and buried ..");

            }
            catch (Exception ex)
            {
                financeviewmodel.errorMessage = ex.Message;
                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Logout button click: " + ex.Message);
                return false;
            }

            return true;
        }

        internal static async Task<bool> FindAddressAsync(
                                                        MainViewModel ourviewmodel,
                                                        FinanceViewModel financeviewmodel,
                                                        CoreWebView2 webView,
                                                        SmartFinance.Logins loginInfo,
                                                        string owner)
        {
            financeviewmodel.UDPRN = "";

            try
            {
                // JS to find href containing "MaintainTelephoneAndAddress"
                string findHrefScript = @"
            (function() {
                const anchors = document.querySelectorAll('a[href*=""MaintainTelephoneAndAddress""]');
                if (anchors.length > 0) {
                    return anchors[0].href;
                }
                return '';
            })();
        ";

                string hrefJson = await webView.ExecuteScriptAsync(findHrefScript);
                // The result is JSON string, e.g. "\"https://example.com/path\""
                string href = System.Text.Json.JsonSerializer.Deserialize<string>(hrefJson);

                if (string.IsNullOrEmpty(href))
                {
                    await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "MaintainTelephoneAndAddress link not found");
                    financeviewmodel.errorMessage = "Address link not found";
                    return false;
                }

                // Navigate to the href and wait for navigation complete event
                var tcsNav = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
                void NavCompleted(object sender, Microsoft.Web.WebView2.Core.CoreWebView2NavigationCompletedEventArgs e)
                {
                    tcsNav.TrySetResult(e.IsSuccess);
                    webView.NavigationCompleted -= NavCompleted;
                }
                webView.NavigationCompleted += NavCompleted;
                webView.Navigate(href);

                bool navSuccess = await tcsNav.Task;
                if (!navSuccess)
                {
                    await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Navigation failed to address page");
                    financeviewmodel.errorMessage = "Navigation to address page failed";
                    return false;
                }

                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Navigated to: " + href);

                // Call FindDetails - you must have a WebView2 version of this method
                financeviewmodel.UDPRN = await FindDetailsAsync(ourviewmodel, financeviewmodel, webView, loginInfo, owner);

                if (string.IsNullOrEmpty(financeviewmodel.UDPRN))
                {
                    await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "UDPRN: <empty>");
                    financeviewmodel.errorMessage = "UDPRN is empty";
                    return false;
                }

                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "UDPRN good: " + financeviewmodel.UDPRN);
                return true;
            }
            catch (Exception ex)
            {
                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Exception: " + ex.Message);
                financeviewmodel.errorMessage = ex.Message;
                return false;
            }
        }

        internal static async Task<string> FindDetailsAsync(
                                                            MainViewModel ourviewmodel,
                                                            FinanceViewModel financeviewmodel,
                                                            CoreWebView2 webView,
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
            })();
        ";

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
                    var ideal_address = await SmartRoutinesV2018.UsersUDPRNLookup(ourviewmodel, bankaddress);
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
            })();
        ";
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
            public string href { get; set; }
            public string innerHTML { get; set; }
        }

        internal static async Task<bool> NationwideBuildAccountList(

                                                                    MainViewModel ourviewmodel,
                                                                    FinanceViewModel financeviewmodel,
                                                                    CoreWebView2 webView,
                                                                    short institution_code,
                                                                    short brand_code,
                                                                    string UDPRN,
                                                                    List<SmartFinance.Transaction_Groups> transaction_groupsFound,
                                                                    List<SmartFinance.Transaction_Types> transaction_typesFound)
        {
            try
            {
                var tcsNavigation = new TaskCompletionSource<bool>();
                void NavigationCompletedHandler(object sender, Microsoft.Web.WebView2.Core.CoreWebView2NavigationCompletedEventArgs e)
                {
                    tcsNavigation.TrySetResult(e.IsSuccess);
                    webView.NavigationCompleted -= NavigationCompletedHandler;
                }

                webView.NavigationCompleted += NavigationCompletedHandler;
                webView.Navigate(financeviewmodel.currentUrl);

                bool navigated = await tcsNavigation.Task;
                if (!navigated)
                {
                    financeviewmodel.errorMessage = "Navigation to accounts list failed.";
                    return false;
                }

                // Run JavaScript to find the links under element with class 'active' with href containing "/AccountList/Account/RedirectToDefaultPage"
                string script = @"
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
            })();
        ";

                string jsonResult = await webView.ExecuteScriptAsync(script);

                // Deserialize JSON string to a list of link info objects
                var accounts = System.Text.Json.JsonSerializer.Deserialize<List<AccountLink>>(jsonResult);

                if (accounts == null || accounts.Count == 0)
                {
                    financeviewmodel.errorMessage = "No accounts found.";
                    return false;
                }

                foreach (var account in accounts)
                {
                    string accountInfo1 = "";
                    string accountInfo2;
                    string accountName = "";
                    string balance = "";
                    string symbol = "";
                    char categoryCode = SmartParametersV2016.defaultChar;

                    // Parse account details from innerHTML
                    NewAccountBalance(financeviewmodel, account.innerHTML, ref accountInfo1, ref balance, ref symbol);

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

                    short currency_ordinal = SmartSpikeV2017.GetCurrencyOrdinal(ourviewmodel, symbol);

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
                        return false;

                    bool processed = await ProcessAccountAsync(

                        ourviewmodel,
                        financeviewmodel,
                        webView,
                        categoryCode,
                        account.href,
                        institution_code,
                        brand_code,
                        sortcode,
                        account_no,
                        financeviewmodel.UDPRN,
                        0, // accountOrdinal (not used here, adjust if needed)
                        symbol,
                        transaction_groupsFound,
                        transaction_typesFound);

                    if (!processed)
                    {

                        await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Process Account: " + financeviewmodel.errorMessage);

                    }

                    financeviewmodel.foundActiveX = true;
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
#endif
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
                    if (!await SmartFinanceScrapeV2023.DoAllSmartFinanceX(ourviewmodel,
                                                                        financeviewmodel,
                                                                        true,
                                                                        SmartParametersV2016.sqliteformat))
                    {

                        MainMeter.financeviewmodel.errorMessage = "Cannot update accounts(3) - cannot continue";
                        await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, financeviewmodel.errorMessage);
                        // This is a bit fatal, because if we can't do this, there's
                        // every chance we can't do lots of things we need to further on
                        return false;

                    }
                }
            }
            catch (Exception ex)
            {
                financeviewmodel.errorMessage = ex.Message;
            }
            return true;
        }

#if WPF || UWP || WINUI
        internal static async Task<bool> ProcessAccountAsync(

                                                        MainViewModel ourviewmodel,
                                                        FinanceViewModel financeviewmodel,
                                                        CoreWebView2 webView,
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
                var tcsNavigation = new TaskCompletionSource<bool>();
                void NavigationCompletedHandler(object sender, Microsoft.Web.WebView2.Core.CoreWebView2NavigationCompletedEventArgs e)
                {
                    webView.NavigationCompleted -= NavigationCompletedHandler;
                    tcsNavigation.TrySetResult(e.IsSuccess);
                }

                webView.NavigationCompleted += NavigationCompletedHandler;
                webView.Navigate(href);

                bool navigated = await tcsNavigation.Task;
                if (!navigated)
                {

                    await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Failed to navigate to: " + href);

                    return false;
                }

                // Click the "Last 12 months" button
                string clickScript = @"
            (function(){
                let btn = document.getElementById('last-12-month-btn');
                if (btn) {
                    btn.click();
                    return true;
                }
                return false;
            })();";
                string clickResult = await webView.ExecuteScriptAsync(clickScript);
                if (!clickResult.Contains("true"))
                {

                    await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Class anchor: is null" + href);

                    return false;
                }

                // Wait for table body to be updated – use a polling loop with timeout
                string tableScript = @"
            (function(){
                let tbody = document.querySelector('tbody');
                return tbody ? tbody.innerText : '';
            })();";

                string responseData = "";
                for (int i = 0; i < 20; i++) // try for up to ~2 seconds
                {
                    responseData = await webView.ExecuteScriptAsync(tableScript);
                    responseData = System.Text.Json.JsonSerializer.Deserialize<string>(responseData); // remove quotes
                    if (!string.IsNullOrWhiteSpace(responseData)) break;
                    await Task.Delay(100);
                }

                if (string.IsNullOrWhiteSpace(responseData))
                {

                    await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "No transaction data loaded");

                    return false;
                }

                string[] transactions = responseData.Split(SmartParametersV2016.newline);



                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Transactions found: " + transactions.Length);


                if (transactions.Length > 0)
                {
                    DateTime accountCreated = SmartParametersV2016.defaultDate;
                    var accounts_found = financeviewmodel.PLO.finance_accountsList
                        .Where(a => a.INSTITUTION_CODE == institution_code &&
                                    a.BRAND_CODE == brand_code &&
                                    a.SORTCODE == sortcode &&
                                    a.ACCOUNT_NO == account_no &&
                                    a.UDPRN == UDPRN)
                        .OrderByDescending(a => a.ACCOUNT_CREATED)
                        .ToList();

                    if (accounts_found.Count > 0)
                        accountCreated = accounts_found.First().ACCOUNT_CREATED;

                    financeviewmodel.sequence_no = SmartFinanceV2025.UnixSequenceNo();

                    await NationwideTransactions(

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
                        accountCreated);
                }
            }
            catch (Exception ex)
            {

                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Exception in ProcessAccount: " + ex.Message);

                return false;
            }
            return true;
        }
#endif
//        internal static async void ListenerNotificationChanged(

//            UserNotificationListener sender, UserNotificationChangedEventArgs args,

//#if WINFORMS
//                                                                    MainProcess components, 
//                                                                    RichTextBox textBoxConsole,                                                                    
//#endif
//                                                                    MainViewModel ourviewmodel,
//                                                                    FinanceViewModel financeviewmodel,
//                                                                    CoreWebView2 webView,
//                                                                    string key,
//                                                                    SmartFinance.Logins loginInfo,
//                                                                    string notificationTitle,
//                                                                    string notificationPrefix,
//                                                                    short notificationTagLength,
//                                                                    short institution_code,
//                                                                    short brand_code,
//                                                                    List<SmartFinance.Transaction_Groups> transaction_groupsFound,
//                                                                    List<SmartFinance.Transaction_Types> transaction_typesFound) // Relevant to 1001/1001


//        {
//            //
//            // Part 1
//            //

//            // Cancel the waiting timer FIRST!
//            CancelTimer(financeviewmodel, key);

//#if WPF || UWP || WINUI
//            await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Handler Event: " + "Delivered");
//#endif
//            if (!await SmartRoutinesV2018.CheckTrace(ourviewmodel, financeviewmodel.financeToken, institution_code, brand_code, "Handler: event delivered"))
//            {
//                return;
//            }

//            // Get the toast notifications
//            //IAsyncOperation<UserNotification> notifsx = (IAsyncOperation<UserNotification>)await sender.GetNotificationsAsync(NotificationKinds.Toast);
//            //await sender.GetNotificationsAsync(Windows.UI.Notifications.NotificationKinds.Toast);

//            IReadOnlyList<UserNotification> notifs = await sender.GetNotificationsAsync(NotificationKinds.Toast);

//            // Try and make sure the latest notification is First
//            IReadOnlyList<UserNotification> notifs_sorted = (from NT in notifs
//                                                             orderby NT.CreationTime descending
//                                                             select NT).ToList();

//            // Select the first notification
//            if (notifs_sorted.Count == 0)
//            {
//#if WPF || UWP || WINUI
//                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "No User Notifications found ");
//#endif
//                financeviewmodel.errorMessage = "No User Notifications found";
//                return;
//            }
//            foreach (UserNotification usernot in notifs_sorted)
//            {
//                UserNotification notif = usernot;
//                // Get the toast binding, if present
//                NotificationBinding toastBinding = notif.Notification.Visual.GetBinding(KnownNotificationBindings.ToastGeneric);
//                if (toastBinding == null)
//                {
//                    continue;
//                }
//                // And then get the text elements from the toast binding
//                IReadOnlyList<AdaptiveNotificationText> textElements = toastBinding.GetTextElements();

//                // Treat the first text element as the title text
//                string titleText = textElements.FirstOrDefault().Text;
//                if (notificationTitle != titleText)
//                {
//                    continue;
//                }

//#if WPF || UWP || WINUI
//                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Found: " + titleText);
//#endif
//                // We'll treat all subsequent text elements as body text,
//                // joining them together via newlines.
//                string bodyText = string.Join("\n", textElements.Skip(1).Select(t => t.Text));
//                if (!bodyText.Contains(notificationPrefix))
//                {
//                    continue;
//                }

//                // Ok, we've found a NATIONWIDE, so say we don't want to invoke
//                // this event handler any more!
//                ourviewmodel.notificationListener.NotificationChanged -= null;
//#if WPF || UWP || WINUI
//                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Handler Event: " + "Removed");
//#endif
//                bodyText = bodyText.Replace(notificationPrefix, "").Trim(); //Trim gets rid of any spaces
//                if (bodyText.Length <= 6)
//                {
//                    continue;
//                }
//                string notificationContent = bodyText.Substring(0, notificationTagLength);

//                string oneTimePasscode = notificationContent;
//#if WPF || UWP || WINUI
//                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "OneTimePasscode: " + oneTimePasscode);
//#endif
//                // Process Accounts
//                if (!await Part2(
//#if WINFORMS
//                                    components,
//                                    textBoxConsole,
//#endif
//                                    ourviewmodel,
//                                    financeviewmodel,
//                                    webView,
//                                    loginInfo,
//                                    oneTimePasscode,
//                                    institution_code,
//                                    brand_code,
//                                    transaction_groupsFound,
//                                    transaction_typesFound))
//                {
//                    return;
//                }
//                else
//                {
//                    // Post Process and Logout
//                    if (!await Part3(
//#if WINFORMS
//                                    components,
//                                    textBoxConsole,
//#endif
//                                    ourviewmodel,
//                                    financeviewmodel,
//                                    webView))
//                    {
//                        return;
//                    }
//                    if (await WebDriverDead(ourviewmodel, financeviewmodel, page))
//                    {
//                        break;
//                    }
//                }
//                break;
//            }
//            // Mark the task as complete so the calling method continues
//            //financeviewmodel.notificationHandledSource.TrySetResult(true);
//            return;
//        }
//#endif

#if WPF || UWP || WINUI
        internal static async Task<bool> WebDriverDead(MainViewModel ourviewmodel,
                                                FinanceViewModel financeviewmodel,
                                                CoreWebView2 webView)
        {

            await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "WebDriver: " + "Quit completed");

            return true;
        }
#endif
    //    internal static async Task<bool> Part2Async(
    //CoreWebView2 webView,
    //MainViewModel ourviewmodel,
    //FinanceViewModel financeviewmodel,
    //string baseUrl,
    //SmartFinance.Logins loginInfo,
    //string oneTimePasscode,
    //short institutionCode,
    //short brandCode,
    //List<SmartFinance.Transaction_Groups> transactionGroupsFound,
    //List<SmartFinance.Transaction_Types> transactionTypesFound,
    //Action<string> log,
    //Action<string> setError)
    //    {
    //        if (string.IsNullOrWhiteSpace(oneTimePasscode))
    //        {
    //            setError("Notification content is empty");
    //            log("Notification content: Empty");
    //            return false;
    //        }

    //        try
    //        {
    //            // Fill OneTimePasscode (using safe JSON string encoding)
    //            var passcodeJson = JsonSerializer.Serialize(oneTimePasscode);
    //            await webView.ExecuteScriptAsync($@"
    //        (() => {{
    //            const input = document.querySelector('input[name=OneTimePasscode]');
    //            if (input) input.value = {passcodeJson};
    //        }})()");
    //            log($"Input Field: {oneTimePasscode} filled");

    //            financeviewmodel.loggedIn = false;

    //            // Click the "Log in" button
    //            string clickScript = @"
    //        (() => {
    //            const buttons = Array.from(document.querySelectorAll('button'));
    //            const login = buttons.find(b => b.innerText.trim() === 'Log in');
    //            if (login) {
    //                login.click();
    //                return 'clicked';
    //            }
    //            return 'not found';
    //        })()";
    //            string rawClickResult = await webView.ExecuteScriptAsync(clickScript);
    //            string clickResult = JsonSerializer.Deserialize<string>(rawClickResult);

    //            if (clickResult != "clicked")
    //            {
    //                setError("Can't find Login button");
    //                return false;
    //            }

    //            log("Log in button: Clicked");
    //            financeviewmodel.loggedIn = true;
    //            financeviewmodel.accountItems = new List<FinanceViewModel.AccountItem>();
    //            financeviewmodel.currentUrl = webView.Source;

    //            // Wait for welcome message
    //            if (!await WaitForElementVisibleAsync(webView, "#welcome-message", timeoutMs: 15000))
    //            {
    //                setError("Welcome message not found");
    //                return false;
    //            }

    //            string welcomeRaw = await webView.ExecuteScriptAsync(@"
    //        (() => {
    //            const el = document.querySelector('#welcome-message');
    //            return el ? el.innerText : '';
    //        })()");
    //            string welcomeText = JsonSerializer.Deserialize<string>(welcomeRaw)?.Replace("Welcome back,", "").Trim();

    //            if (!string.IsNullOrEmpty(welcomeText))
    //            {
    //                log("Welcome back: " + welcomeText);
    //            }

    //            bool foundAddress = await FindAddressAsync(
    //                webView,
    //                ourviewmodel,
    //                financeviewmodel,
    //                baseUrl,
    //                loginInfo,
    //                welcomeText);

    //            if (!foundAddress)
    //            {
    //                log("Find address: Failed");
    //                return false;
    //            }

    //            log("Owner: " + loginInfo.OWNER);
    //            log("Contact no.: " + loginInfo.CONTACT_PHONENO);
    //            log("Email address: " + loginInfo.CONTACT_EMAIL);

    //            financeviewmodel.account_id = 0;
    //            financeviewmodel.AccountLinks = new List<KeyValuePair<string, string>>();

    //            bool builtAccounts = await BuildAccountListWebView2Async(
    //                webView,
    //                ourviewmodel,
    //                financeviewmodel,
    //                baseUrl,
    //                institutionCode,
    //                brandCode,
    //                financeviewmodel.UDPRN,
    //                transactionGroupsFound,
    //                transactionTypesFound,
    //                log,
    //                setError);

    //            if (!builtAccounts)
    //            {
    //                log(financeviewmodel.errorMessage);
    //                return false;
    //            }

    //            log("Transactions inserted: " + financeviewmodel.transactions_count);
    //            return true;
    //        }
    //        catch (Exception ex)
    //        {
    //            setError(ex.Message);
    //            return false;
    //        }
    //    }

        //internal static async Task<bool> WaitForElementVisibleAsync(CoreWebView2 webView, string selector, int timeoutMs = 10000, int intervalMs = 500)
        //{
        //    int elapsed = 0;

        //    while (elapsed < timeoutMs)
        //    {
        //        try
        //        {
        //            string script = $@"
        //        (() => {{
        //            const el = document.querySelector('{selector}');
        //            return el && el.offsetParent !== null;
        //        }})()";

        //            string result = await webView.ExecuteScriptAsync(script);
        //            bool isVisible = JsonSerializer.Deserialize<bool>(result);

        //            if (isVisible)
        //                return true;
        //        }
        //        catch
        //        {
        //            // Ignore errors and keep retrying
        //        }

        //        await Task.Delay(intervalMs);
        //        elapsed += intervalMs;
        //    }

        //    return false;
        //}

        // Placeholder for your existing logic
        //private static async Task<bool> FindAddressAsync(
        //                                                        CoreWebView2 webView,
        //                                                        MainViewModel ourviewmodel,
        //                                                        FinanceViewModel financeviewmodel,
        //                                                        string baseUrl,
        //                                                        SmartFinance.Logins loginInfo,
        //                                                        string owner)
        //{
        //    financeviewmodel.UDPRN = ""; // Everything depends on finding this!

        //    try
        //    {
        //        // Navigate to address page
        //        string href = "CustomerIB/MaintainTelephoneAndAddress";
        //        string addressUrl = new Uri(new Uri(baseUrl), href).ToString();
        //        webView.Navigate(addressUrl);

        //        // Wait for navigation to complete
        //        var navTcs = new TaskCompletionSource<bool>();
        //        void Handler(object sender, CoreWebView2NavigationCompletedEventArgs e)
        //        {
        //            webView.NavigationCompleted -= Handler;
        //            navTcs.SetResult(true);
        //        }
        //        webView.NavigationCompleted += Handler;
        //        await navTcs.Task;

        //        // Now call FindDetails using WebView2
        //        financeviewmodel.UDPRN = await FindDetailsAsync(webView, ourviewmodel, financeviewmodel, loginInfo, owner);
        //    }
        //    catch (Exception ex)
        //    {
        //        financeviewmodel.errorMessage = "Can't navigate to address page: " + ex.Message;
        //        return false;
        //    }

        //    if (string.IsNullOrEmpty(financeviewmodel.UDPRN))
        //    {
        //        financeviewmodel.errorMessage = "UDPRN is empty";
        //        return false;
        //    }

        //    return true;
        //}

        //internal static async Task<string> FindDetailsAsync(
        //                                                    CoreWebView2 webView,
        //                                                    MainViewModel ourviewmodel,
        //                                                    FinanceViewModel financeviewmodel,
        //                                                    SmartFinance.Logins loginInfo,
        //                                                    string owner)
        //{
        //    string udprn = "";
        //    string bankAddress = "";

        //    try
        //    {
        //        // Step 1 - Find <address> tag text
        //        string script = @"
        //    (() => {
        //        const address = document.querySelector('address');
        //        return address ? address.innerText : '';
        //    })();";
        //        string result = await webView.ExecuteScriptAsync(script);
        //        string rawText = JsonSerializer.Deserialize<string>(result);

        //        if (!string.IsNullOrEmpty(rawText))
        //        {
        //            bankAddress = rawText.Replace(SmartParametersV2016.newline, SmartParametersV2016.spaceSplit).Trim();
        //            bankAddress = Regex.Replace(bankAddress, @"\s{2,}", " ");
        //        }

        //        if (string.IsNullOrWhiteSpace(bankAddress))
        //        {
        //            financeviewmodel.errorMessage = "Bank address empty - cannot continue";
        //            return udprn;
        //        }

        //        var addressesFound = SmartSpikeV2017.UsersLookupTextAddress(ourviewmodel, bankAddress);
        //        if (addressesFound.Any())
        //        {
        //            if (string.IsNullOrEmpty(addressesFound.First().UDPRN))
        //            {
        //                financeviewmodel.errorMessage = "Address record has no UDPRN - cannot continue";
        //                return udprn;
        //            }

        //            udprn = addressesFound.First().UDPRN;
        //        }
        //        else
        //        {
        //            var idealAddress = await SmartRoutinesV2018.UsersUDPRNLookup(ourviewmodel, bankAddress);
        //            if (string.IsNullOrEmpty(idealAddress.UDPRN))
        //            {
        //                financeviewmodel.errorMessage = "Cannot determine UDPRN - cannot continue";
        //                return udprn;
        //            }

        //            udprn = idealAddress.UDPRN;

        //            short areaCode = SmartSpikeV2017.Find_Working_Postcodes(
        //                ourviewmodel.Blanche.workingPostcodesList,
        //                idealAddress.POSTCODE_OUTWARD).FirstOrDefault()?.AREA_CODE ?? 0;

        //            var newAddress = new SmartUsers.AddressesView
        //            {
        //                USERNAME = ourviewmodel.UserName,
        //                UDPRN = idealAddress.UDPRN,
        //                RANDOM_KEY1 = SmartRoutinesV2018.GetNextRandomNew(ourviewmodel.randomR),
        //                ADDRESS_CREATED = DateTime.Now,
        //                BASIC = bankAddress,
        //                BUILDING_NAME = idealAddress.BUILDING_NAME,
        //                BUILDING_NUMBER = idealAddress.BUILDING_NUMBER,
        //                COUNTY = idealAddress.COUNTY,
        //                DOUBLE_DEPENDANT_LOCALITY = idealAddress.DOUBLE_DEPENDANT_LOCALITY,
        //                DEPENDANT_THOROUGHFARE = idealAddress.DEPENDANT_THOROUGHFARE,
        //                DEPENDANT_LOCALITY = idealAddress.DEPENDANT_LOCALITY,
        //                ORGANIZATION = idealAddress.ORGANIZATION,
        //                POSTCODE_OUTWARD = idealAddress.POSTCODE_OUTWARD,
        //                POSTCODE = idealAddress.POSTCODE,
        //                POBOX = idealAddress.POBOX,
        //                SUB_BUILDING_NAME = idealAddress.SUB_BUILDING_NAME,
        //                THOROUGHFARE = idealAddress.THOROUGHFARE,
        //                TOWN = idealAddress.TOWN,
        //                RANDOM_KEY2 = SmartRoutinesV2018.GetNextRandomNew(ourviewmodel.randomR),
        //                Updated = false
        //            };

        //            ourviewmodel.Hamas.addressesview_changesList.Add(newAddress);
        //        }

        //        // Step 6 - Optional fields
        //        async Task<string> GetValue(string selector)
        //        {
        //            string raw = await webView.ExecuteScriptAsync($@"
        //        (() => {{
        //            const el = document.querySelector('{selector}');
        //            return el ? el.value : '';
        //        }})();");
        //            return JsonSerializer.Deserialize<string>(raw);
        //        }

        //        string emailVal = await GetValue("#CurrentEmailAddress");
        //        if (!string.IsNullOrEmpty(emailVal) && loginInfo.CONTACT_EMAIL != emailVal)
        //        {
        //            loginInfo.CONTACT_EMAIL = emailVal;
        //            loginInfo.Updated = true;
        //        }

        //        string phoneVal = await GetValue("#MobilePhoneNumber");
        //        if (!string.IsNullOrEmpty(phoneVal) && loginInfo.CONTACT_PHONENO != phoneVal)
        //        {
        //            loginInfo.CONTACT_PHONENO = phoneVal;
        //            loginInfo.Updated = true;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        financeviewmodel.errorMessage = ex.Message;
        //        return udprn;
        //    }

        //    if (owner != loginInfo.OWNER)
        //    {
        //        loginInfo.OWNER = owner;
        //        loginInfo.Updated = true;
        //    }

        //    if (loginInfo.Updated)
        //    {
        //        financeviewmodel.PLO.finance_logins_changesList.Add(loginInfo);
        //    }

        //    return udprn;
        //}


//        internal static async Task<string> FindDetailsAsync(CoreWebView2 webView,
//                                                            MainViewModel ourviewmodel,
//                                                            FinanceViewModel financeviewmodel,
//                                                            SmartFinance.Logins loginInfo,
//                                                            string owner)
//        {
//            string udprn = "";
//            string bankAddress = "";

//            try
//            {
//                // Step 1 - Find <address> tag
//                var addressElements = await page2.QuerySelectorAllAsync("address");
//                foreach (var addressElement in addressElements)
//                {
//                    string rawText = await addressElement.InnerTextAsync();
//                    if (!string.IsNullOrEmpty(rawText))
//                    {
//                        bankAddress = rawText.Replace(SmartParametersV2016.newline, SmartParametersV2016.spaceSplit).Trim();
//                        bankAddress = Regex.Replace(bankAddress, @"\s{2,}", " ");
//                        break;
//                    }
//                }

//                if (string.IsNullOrWhiteSpace(bankAddress))
//                {
//#if WPF || UWP || WINUI
//                    await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Bank address empty - cannot continue");
//#endif
//                    financeviewmodel.errorMessage = "Bank address empty - cannot continue";
//                    return udprn;
//                }

//                // Step 2 - Check for existing address
//                var addressesFound = SmartSpikeV2017.UsersLookupTextAddress(ourviewmodel, bankAddress);
//                if (addressesFound.Any())
//                {
//                    if (string.IsNullOrEmpty(addressesFound.First().UDPRN))
//                    {
//#if WPF || UWP || WINUI
//                        await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Address record has no UDPRN - cannot continue");
//#endif
//                        financeviewmodel.errorMessage = "Address record has no UDPRN - cannot continue";
//                        return udprn;
//                    }

//                    udprn = addressesFound.First().UDPRN;
//                }
//                else
//                {
//                    // Step 5 - Use Ideal Postcodes lookup
//                    var idealAddress = await SmartRoutinesV2018.UsersUDPRNLookup(ourviewmodel, bankAddress);
//                    if (string.IsNullOrEmpty(idealAddress.UDPRN))
//                    {
//#if WPF || UWP || WINUI
//                        await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Cannot determine UDPRN - cannot continue");
//#endif
//                        financeviewmodel.errorMessage = "Cannot determine UDPRN - cannot continue";
//                        return udprn;
//                    }

//                    udprn = idealAddress.UDPRN;

//                    // Determine Area Code
//                    var postcodes = SmartSpikeV2017.Find_Working_Postcodes(ourviewmodel.Blanche.workingPostcodesList, idealAddress.POSTCODE_OUTWARD);
//                    short areaCode = postcodes.FirstOrDefault()?.AREA_CODE ?? 0;

//                    var newAddress = new SmartUsers.AddressesView
//                    {
//                        USERNAME = ourviewmodel.UserName,
//                        UDPRN = idealAddress.UDPRN,
//                        RANDOM_KEY1 = SmartRoutinesV2018.GetNextRandomNew(ourviewmodel.randomR),
//                        ADDRESS_CREATED = DateTime.Now,
//                        BASIC = bankAddress,
//                        BUILDING_NAME = idealAddress.BUILDING_NAME,
//                        BUILDING_NUMBER = idealAddress.BUILDING_NUMBER,
//                        COUNTY = idealAddress.COUNTY,
//                        DOUBLE_DEPENDANT_LOCALITY = idealAddress.DOUBLE_DEPENDANT_LOCALITY,
//                        DEPENDANT_THOROUGHFARE = idealAddress.DEPENDANT_THOROUGHFARE,
//                        DEPENDANT_LOCALITY = idealAddress.DEPENDANT_LOCALITY,
//                        ORGANIZATION = idealAddress.ORGANIZATION,
//                        POSTCODE_OUTWARD = idealAddress.POSTCODE_OUTWARD,
//                        POSTCODE = idealAddress.POSTCODE,
//                        POBOX = idealAddress.POBOX,
//                        SUB_BUILDING_NAME = idealAddress.SUB_BUILDING_NAME,
//                        THOROUGHFARE = idealAddress.THOROUGHFARE,
//                        TOWN = idealAddress.TOWN,
//                        RANDOM_KEY2 = SmartRoutinesV2018.GetNextRandomNew(ourviewmodel.randomR),
//                        Updated = false
//                    };

//                    ourviewmodel.Hamas.addressesview_changesList.Add(newAddress);
//                }

//                // Step 6 - Optional fields: Email and Mobile
//                var emailElement = await page2.QuerySelectorAsync("#CurrentEmailAddress");
//                if (emailElement != null)
//                {
//                    string emailVal = await emailElement.GetAttributeAsync("value");
//                    if (loginInfo.CONTACT_EMAIL != emailVal)
//                    {
//                        loginInfo.CONTACT_EMAIL = emailVal;
//                        loginInfo.Updated = true;
//                    }
//                }

//                var mobileElement = await page2.QuerySelectorAsync("#MobilePhoneNumber");
//                if (mobileElement != null)
//                {
//                    string phoneVal = await mobileElement.GetAttributeAsync("value");
//                    if (loginInfo.CONTACT_PHONENO != phoneVal)
//                    {
//                        loginInfo.CONTACT_PHONENO = phoneVal;
//                        loginInfo.Updated = true;
//                    }
//                }
//            }
//            catch (Exception ex)
//            {
//#if WPF || UWP || WINUI
//                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, ex.Message);
//#endif
//                financeviewmodel.errorMessage = ex.Message;
//                return udprn;
//            }

//            // Update owner if changed
//            if (owner != loginInfo.OWNER)
//            {
//                loginInfo.OWNER = owner;
//                loginInfo.Updated = true;
//            }

//            if (loginInfo.Updated)
//            {
//                financeviewmodel.PLO.finance_logins_changesList.Add(loginInfo);
//            }
//            return udprn;
//        }



//        private static async Task<bool> BuildAccountListWebView2Async(
//                                                    CoreWebView2 webView,
//                                                    MainViewModel ourviewmodel,
//                                                    FinanceViewModel financeviewmodel,
//                                                    string baseUrl,
//                                                    short institutionCode,
//                                                    short brandCode,
//                                                    string UDPRN,
//                                                    List<SmartFinance.Transaction_Groups> transactionGroupsFound,
//                                                    List<SmartFinance.Transaction_Types> transactionTypesFound,
//                                                    Action<string> log,
//                                                    Action<string> setError)
//        {
//            try
//            {
//                string script = @"
//                                (() => {
//                                    const results = [];
//                                    const active = document.querySelector('.active');
//                                    if (!active) return results;
//                                    const links = active.querySelectorAll('a');
//                                    for (const link of links) {
//                                        const href = link.getAttribute('href');
//                                        const html = link.innerHTML;
//                                        if (href && href.includes('/AccountList/Account/RedirectToDefaultPage') && html) {
//                                            results.push({ href, html });
//                                        }
//                                    }
//                                    return results;
//                                })()";
//                string resultJson = await webView.ExecuteScriptAsync(script);
//                var linkInfos = JsonSerializer.Deserialize<List<LinkInfo>>(resultJson, new JsonSerializerOptions
//                {
//                    PropertyNameCaseInsensitive = true
//                });

//                if (linkInfos == null || linkInfos.Count == 0)
//                {
//                    log("No valid account links found.");
//                    return false;
//                }

//                foreach (var link in linkInfos)
//                {
//                    string accountInfo1 = "", accountInfo2 = "", accountName = "", balance = "", symbol = "";
//                    char categoryCode = SmartParametersV2016.defaultChar;
//                    short accountOrdinal = 0;

//                    NewAccountBalance(financeviewmodel, link.Html, ref accountInfo1, ref balance, ref symbol);
//                    if (string.IsNullOrEmpty(accountInfo1))
//                    {
//                        log("Couldn't parse account info: " + accountInfo1);
//                        financeviewmodel.errorMessage = "Couldn't parse account info";
//                        return false;
//                    }

//                    accountInfo2 = SplitAccountName(financeviewmodel, accountInfo1, ref accountName);
//                    if (string.IsNullOrEmpty(accountName))
//                    {
//                        log("Account name empty");
//                        financeviewmodel.errorMessage = "Account name empty";
//                        return false;
//                    }

//                    if (string.IsNullOrEmpty(accountInfo2))
//                    {
//                        log("Account sortcode/account empty");
//                        financeviewmodel.errorMessage = "Account sortcode/account empty";
//                        return false;
//                    }

//                    short currencyOrdinal = SmartSpikeV2017.GetCurrencyOrdinal(ourviewmodel, symbol);
//                    categoryCode = accountName.Contains("Savings", StringComparison.OrdinalIgnoreCase)
//                        ? SmartParametersV2016.Savings
//                        : SmartParametersV2016.Banks;

//                    string sortcode = "", account_no = "";
//                    var accstuff = accountInfo2.Split(SmartParametersV2016.spacechar);
//                    if (accstuff.Length > 1)
//                    {
//                        sortcode = accstuff[0].Replace("-", "");
//                        account_no = accstuff[1];
//                    }
//                    else
//                    {
//                        account_no = accstuff[0];
//                    }

//                    bool decoded = await DecodeAccount(
//                        ourviewmodel,
//                        financeviewmodel,
//                        accountName,
//                        sortcode,
//                        account_no,
//                        currencyOrdinal,
//                        institutionCode,
//                        brandCode,
//                        UDPRN,
//                        categoryCode);

//                    if (!decoded)
//                        return false;

//                    string logAction;
//                    string errorMessage;
//                    bool processed = await ProcessAccountAsync(
//#if WINFORMS
//                                                                    components, 
//                                                                    textBoxConsole,
//#endif
//                                                                    ourviewmodel,
//                                                                    financeviewmodel,
//                                                                    webView,
//                                                                    categoryCode,
//                                                                    baseUrl,
//                                                                    href,
//                                                                    institutionCode,
//                                                                    brandCode,
//                                                                    sortcode,
//                                                                    account_no,
//                                                                    financeviewmodel.UDPRN,
//                                                                    accountOrdinal,
//                                                                    symbol,
//                                                                    transactionGroupsFound,
//                                                                    transactionTypesFound,
//                                                                    la => logAction = la,
//                                                                    em => errorMessage = em);

//                    if (!processed)
//                    {
//                        log("Process Account failed: " + financeviewmodel.errorMessage);
//                    }

//                    financeviewmodel.foundActiveX = true;
//                }
//            }
//            catch (Exception ex)
//            {
//                setError(ex.Message);
//                financeviewmodel.errorMessage = ex.Message;
//                return false;
//            }

//            return true;
//        }

        //private class LinkInfo
        //{
        //    public string Href { get; set; }
        //    public string Html { get; set; }
        //}

//        private static async Task<bool> BuildAccountListAsync(
//#if WINFORMS
//                                                            MainProcess components,
//                                                            RichTextBox textBoxConsole,
//#endif
//                                                            MainViewModel ourviewmodel,
//                                                            FinanceViewModel financeviewmodel,
//                                                            CoreWebView2 webView,
//                                                            string baseUrl,
//                                                            short institutionCode,
//                                                            short brandCode,
//                                                            string UDPRN,
//                                                            List<SmartFinance.Transaction_Groups> transactionGroupsFound,
//                                                            List<SmartFinance.Transaction_Types> transactionTypesFound)
//        {
//            // Implement actual logic here
//            string accountInfo1 = "";
//            string accountInfo2;
//            string accountName = "";
//            string balance = "";
//            string symbol = "";
//            char categoryCode = SmartParametersV2016.defaultChar;

//            try
//            {
//                var offs = await page.QuerySelectorAsync("[class='active']");
//                if (offs != null)
//                {
//                    var links = await offs.QuerySelectorAllAsync("a");
//                    foreach (var link in links)
//                    {
//                        string href = await link.GetAttributeAsync(SmartParametersV2016.href);
//                        if (href != null &&
//                            href.Contains("/AccountList/Account/RedirectToDefaultPage"))
//                        {
//                            string innerHtml = await link.InnerHTMLAsync();
//                            if (!string.IsNullOrEmpty(innerHtml))
//                            {
//                                short accountOrdinal = 0;
//                                categoryCode = SmartParametersV2016.defaultChar;

//                                NewAccountBalance(financeviewmodel, innerHtml, ref accountInfo1, ref balance, ref symbol);
//                                if (string.IsNullOrEmpty(accountInfo1))
//                                {
//#if WPF || UWP || WINUI
//                                    await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Couldn't parse account info: " + accountInfo1);
//#endif
//                                    financeviewmodel.errorMessage = "Couldn't parse account info: " + accountInfo1;
//                                    return false;
//                                }

//                                accountInfo2 = SplitAccountName(financeviewmodel, accountInfo1, ref accountName);
//                                if (string.IsNullOrEmpty(accountName))
//                                {
//#if WPF || UWP || WINUI
//                                    await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Account name empty");
//#endif
//                                    financeviewmodel.errorMessage = "Account name empty";
//                                    return false;
//                                }

//                                if (string.IsNullOrEmpty(accountInfo2))
//                                {
//#if WPF || UWP || WINUI
//                                    await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Account sortcode/account empty");
//#endif
//                                    financeviewmodel.errorMessage = "Account sortcode/account empty";
//                                    return false;
//                                }

//                                short currencyOrdinal = SmartSpikeV2017.GetCurrencyOrdinal(ourviewmodel, symbol);
//                                categoryCode = SmartParametersV2016.Banks;

//                                string sortcode = "", account_no = "";
//                                if (accountName.Contains("Savings", StringComparison.OrdinalIgnoreCase))
//                                {
//                                    categoryCode = SmartParametersV2016.Savings;
//                                }

//                                var accstuff = accountInfo2.Split(SmartParametersV2016.spacechar);
//                                if (accstuff.Length > 1)
//                                {
//                                    sortcode = accstuff[0].Replace("-", "");
//                                    account_no = accstuff[1];
//                                }
//                                else
//                                {
//                                    account_no = accstuff[0];
//                                }

//                                bool decoded = await DecodeAccount(ourviewmodel,
//                                                                    financeviewmodel,
//                                                                    accountName,
//                                                                    sortcode,
//                                                                    account_no,
//                                                                    currencyOrdinal,
//                                                                    institutionCode,
//                                                                    brandCode,
//                                                                    UDPRN,
//                                                                    categoryCode);

//                                if (!decoded)
//                                {
//                                    return false;
//                                }
//                                string logAction;
//                                string errorMessage;
//                                bool processed = await ProcessAccountAsync(
//#if WINFORMS
//                                                                    components, 
//                                                                    textBoxConsole,
//#endif
//                                                                    ourviewmodel,
//                                                                    financeviewmodel,
//                                                                    webView,
//                                                                    categoryCode,
//                                                                    baseUrl,
//                                                                    href,
//                                                                    institutionCode,
//                                                                    brandCode,
//                                                                    sortcode,
//                                                                    account_no,
//                                                                    financeviewmodel.UDPRN,
//                                                                    accountOrdinal,
//                                                                    symbol,
//                                                                    transactionGroupsFound,
//                                                                    transactionTypesFound,
//                                                                    la => logAction = la,
//                                                                    em => errorMessage = em);

//                                if (!processed)
//                                {
//#if WPF || UWP || WINUI
//                                    await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Process Account: " + financeviewmodel.errorMessage);
//#endif
//                                }
//                                financeviewmodel.foundActiveX = true;
//                            }
//                        }
//                    }
//                }
//            }
//            catch (Exception ex)
//            {
//#if WPF || UWP || WINUI
//                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, ex.Message);
//#endif
//                financeviewmodel.errorMessage = ex.Message;
//                return false;
//            }
//            return true;
//        }



        

        //public static async Task<bool> ProcessAccountAsync(MainViewModel ourviewmodel,
        //                                                        FinanceViewModel financeviewmodel,
        //                                                        CoreWebView2 webView,
        //                                                        char categoryCode,
        //                                                        string baseUrl,
        //                                                        string href,
        //                                                        short institutionCode,
        //                                                        short brandCode,
        //                                                        string sortCode,
        //                                                        string accountNo,
        //                                                        string udprn,
        //                                                        short accountOrdinal,
        //                                                        string symbol,
        //                                                        List<SmartFinance.Transaction_Groups> transactionGroupsFound,
        //                                                    List<SmartFinance.Transaction_Types> transactionTypesFound
        //                                                        Action<string> logAction,
        //                                                        Action<string> setError)
        //{
        //    try
        //    {
        //        href = href.Replace("/AccountList/Account/RedirectToDefaultPage", "/Transactions/FullStatement/FullStatement");
        //        string absoluteUrl = new Uri(new Uri(baseUrl), href).ToString();

        //        // --- Wait for initial navigation ---
        //        var navCompleted1 = new TaskCompletionSource<bool>();
        //        void Handler1(object? s, CoreWebView2NavigationCompletedEventArgs e)
        //        {
        //            navCompleted1.TrySetResult(true);
        //            webView.NavigationCompleted -= Handler1;
        //        }

        //        webView.NavigationCompleted += Handler1;
        //        webView.Navigate(absoluteUrl);
        //        await navCompleted1.Task;

        //        // --- Click the "Last 12 months" button ---
        //        string clickScript = @"
        //                            (() => {
        //                                const btn = document.getElementById('last-12-month-btn');
        //                                if (!btn) return 'NotFound';
        //                                btn.click();
        //                                return 'Clicked';
        //                            })()";
        //        string clickResult = await webView.ExecuteScriptAsync(clickScript);
        //        if (clickResult.Contains("NotFound"))
        //        {
        //            log("Class anchor: is null");
        //            return false;
        //        }

        //        // --- Wait for navigation or reload after click (if applicable) ---
        //        var navCompleted2 = new TaskCompletionSource<bool>();
        //        void Handler2(object? s, CoreWebView2NavigationCompletedEventArgs e)
        //        {
        //            navCompleted2.TrySetResult(true);
        //            webView.NavigationCompleted -= Handler2;
        //        }

        //        webView.NavigationCompleted += Handler2;

        //        // Set a short timeout in case there's no navigation — don't hang forever
        //        var completedTask = await Task.WhenAny(navCompleted2.Task, Task.Delay(2000));
        //        if (completedTask != navCompleted2.Task)
        //        {
        //            // No navigation happened — proceed anyway
        //            webView.NavigationCompleted -= Handler2;
        //        }

        //        // --- Extract transactions from the table ---
        //        string getTableScript = @"
        //                                (() => {
        //                                    const tbody = document.querySelector('tbody');
        //                                    return tbody ? tbody.innerText : '';
        //                                })()";
        //        var json = await webView.ExecuteScriptAsync(getTableScript);
        //        var responseData = JsonSerializer.Deserialize<string>(json);

        //        if (string.IsNullOrWhiteSpace(responseData))
        //        {
        //            log("Table body not found");
        //            return false;
        //        }

        //        var transactions = responseData.Split(new[] { '\n' }, StringSplitOptions.None);
        //        log($"Transactions found: {transactions.Length}");

        //        // Continue processing `transactions` here

        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        setError(ex.Message);
        //        return false;
        //    }
        //}


//        internal static async Task<bool> ProcessAccount(
//#if WINFORMS
//                                                        MainProcess components,
//                                                        RichTextBox textBoxConsole,
//#endif
//                                                        MainViewModel ourviewmodel,
//                                                        FinanceViewModel financeviewmodel,
//                                                        CoreWebView2 webView,
//                                                        char categoryCode,
//                                                        string href,
//                                                        short institution_code,
//                                                        short brand_code,
//                                                        string sortcode,
//                                                        string account_no,
//                                                        string UDPRN,
//                                                        short accountOrdinal,
//                                                        string symbol,
//                                                        List<SmartFinance.Transaction_Groups> transaction_groupsFound,
//                                                        List<SmartFinance.Transaction_Types> transaction_typesFound)
//        {
//            try
//            {
//                href = href.Replace("/AccountList/Account/RedirectToDefaultPage", "/Transactions/FullStatement/FullStatement");

//                //var absoluteUrl = new Uri(new Uri(baseUrl), href).ToString();
//                //var page3 = await context.NewPageAsync();
//                await page3.GotoAsync(absoluteUrl);

//                var classAnchor = await page3.QuerySelectorAsync("[id='last-12-month-btn']");
//                if (classAnchor == null)
//                {
//#if WPF || UWP || WINUI
//                    await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Class anchor: is null");
//#endif
//                    return false;
//                }
//                else
//                {
//                    await classAnchor.EvaluateAsync("el => el.click()");

//                    var tbody = await page3.QuerySelectorAsync("tbody");
//                    if (tbody == null)
//                    {
//#if WPF || UWP || WINUI
//                        await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Table body not found");
//#endif
//                        return false;
//                    }

//                    string responseData = await tbody.InnerTextAsync();
//                    string[] transactions = responseData.Split(SmartParametersV2016.newline);

//#if WPF || UWP || WINUI
//                    await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Transactions found: " + transactions.Length);
//#endif
//                    if (transactions.Length > 0)
//                    {
//                        DateTime accountCreated = SmartParametersV2016.defaultDate;

//                        var accounts_found = financeviewmodel.PLO.finance_accountsList
//                            .Where(acc => acc.INSTITUTION_CODE == institution_code &&
//                                          acc.BRAND_CODE == brand_code &&
//                                          acc.SORTCODE == sortcode &&
//                                          acc.ACCOUNT_NO == account_no &&
//                                          acc.UDPRN == UDPRN)
//                            .OrderByDescending(acc => acc.ACCOUNT_CREATED)
//                            .ToList();

//                        if (accounts_found.Count > 0)
//                        {
//                            accountCreated = accounts_found.First().ACCOUNT_CREATED;
//                        }

//                        financeviewmodel.sequence_no = SmartFinanceV2025.UnixSequenceNo();

//                        await NationwideTransactions(
//#if WINFORMS
//                                                components,
//                                                textBoxConsole,
//#endif
//                                                    ourviewmodel,
//                                                    financeviewmodel,
//                                                    categoryCode,
//                                                    transactions,
//                                                    institution_code,
//                                                    brand_code,
//                                                    sortcode,
//                                                    account_no,
//                                                    UDPRN,
//                                                    symbol,
//                                                    transaction_groupsFound,
//                                                    transaction_typesFound,
//                                                    accountCreated);
//                    }
//                }
//            }
//            catch (Exception)
//            {
//#if WPF || UWP || WINUI
//                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "No Last 12 months button");
//#endif
//                return false;
//            }
//            return true;
//        }

        internal static async Task<bool> NationwideTransactions(
#if WINFORMS
                                                                MainProcess components,
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

                            await SmartBobV2017.ListenerAsync(ourviewmodel, financeviewmodel.financeToken, institutionCode, brandCode, "NW Trans Lookup", "Trans type  failed: " + transaction_type);

                            // Carry on
                        }
                        else
                        {
                            if (transCodes[0] == 0 || transCodes[1] == 0)
                            {

                                await SmartBobV2017.ListenerAsync(ourviewmodel, financeviewmodel.financeToken, institutionCode, brandCode, "NW Trans Lookup", "Trans type  failed: " + transaction_type);

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

        //        internal static async Task<bool> Part3(
        //#if WINFORMS
        //                                    MainProcess components,
        //                                    RichTextBox textBoxConsole,
        //#endif
        //                                    MainViewModel ourviewmodel,
        //                                    FinanceViewModel financeviewmodel,
        //                                    CoreWebView2 webView)
        //        {
        //            // Reset this - just to make sure we return to
        //            // "AccountsList" every time!!
        //            try
        //            {
        //                //webDriver.Url = financeviewmodel.currentUrl;

//                // Because we are logged in,
//                // No need to process any more buttons!
//                IElementHandle logoutButton = await page.QuerySelectorAsync(".log-out-link");
//                if (logoutButton == null)
//                {
//#if WPF || UWP || WINUI
//                    await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "No buttons found");
//#endif
//                    financeviewmodel.errorMessage = "Couldn't find any buttons";
//                    return false;
//                }
//                // Try and depart gracefully
//                await logoutButton.ClickAsync(); // equivalent to JS click

//#if WPF || UWP || WINUI
//                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Logout button: " + "Clicked");
//#endif
//            }
//            catch (Exception ex)
//            {
//                financeviewmodel.errorMessage = ex.Message;
//#if WPF || UWP || WINUI
//                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Logout button click: " + ex.Message);
//#endif
//                return false;
//            }
//            return true;
//        }


#if WPF || UWP || WINUI
        internal static async Task<bool> NationwideFindInputs(
                                                        MainViewModel ourviewmodel,
                                                        FinanceViewModel financeviewmodel,
                                                        CoreWebView2 webView,
                                                        string customerNumber,
                                                        string day,
                                                        string year)
        {
            bool status = false;

            bool customernumber_found = false,
                 dateofbirthday_found = false,
                 dateofbirthyear_found = false;

            try
            {
                await webView.ExecuteScriptAsync($"document.querySelector(\"input[name='CustomerNumber']\").value = '{customerNumber}';");
                customernumber_found = true;
            }
            catch (Exception ex)
            {
                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Not filled: CustomerId " + ex.Message);
                return false;
            }

            try
            {
                await webView.ExecuteScriptAsync($"document.querySelector(\"input[name='DateOfBirthDay']\").value = '{day}';");
                dateofbirthday_found = true;
            }
            catch (Exception ex)
            {
                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Not filled: DateOfBirthDay " + ex.Message);
                return false;
            }

            try
            {
                await webView.ExecuteScriptAsync($"document.querySelector(\"input[name='DateOfBirthYear']\").value = '{year}';");
                dateofbirthyear_found = true;
            }
            catch (Exception ex)
            {
                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Not filled: DateOfBirthYear " + ex.Message);
                return false;
            }

            if (customernumber_found && dateofbirthday_found && dateofbirthyear_found)
            {
                status = true;
            }

            return status;
        }

        public static async Task<bool> SelectRadioButtonAsync(
                                        CoreWebView2 webView,
                                        Action<string> logAction)
        {
            try
            {
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

                string result = await webView.ExecuteScriptAsync(script);
                if (result.Contains("Clicked"))
                {
                    logAction("Radio button clicked");
                    return true;
                }
                else
                {
                    logAction("Radio button cannot be clicked");
                    return false;
                }
            }
            catch (Exception ex)
            {
                logAction($"Error clicking radio button: {ex.Message}");
                return false;
            }
        }
        internal static async Task<bool> NationwideFindPasscodesAsync(

                                                                        //MainViewModel ourviewmodel,
                                                                        //FinanceViewModel financeviewmodel,
                                                                        CoreWebView2 webView,
                                                                        string PASSCODE,
                                                                        Action<string> logAction,
                                                                        Action<string> setError)
        {
            try
            {
                // Step 1: Get spans with class 'control__label__title'
                string script = @"
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
                var resultJson = await webView.ExecuteScriptAsync(script);
                var spanTexts = System.Text.Json.JsonSerializer.Deserialize<List<string>>(resultJson);

                if (spanTexts == null || spanTexts.Count == 0)
                {
                    logAction("Couldn't find 'passcodes' span class");
                    return false;
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
                    string digit = PASSCODE.Substring(digitIndex, 1);
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
                    string selectionResult = await webView.ExecuteScriptAsync(selectScript);
                    if (selectionResult.Contains("Missing"))
                    {
                        logAction($"Couldn't find '{dropdownNames[j]}' drop-down input");
                        return false;
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                setError(ex.Message);
                return false;
            }
        }
#endif

        public static async void AddWaitTimer(MainViewModel ourviewmodel, FinanceViewModel financeviewmodel, string key, int seconds, Action<string> onTick, Action<string, MainViewModel.TimerFinishReason> onComplete)
        {
            if (IsRunning(financeviewmodel, key))
            {



                //MessageBox.Show($"Timer {key} is already running.");
                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, $"Timer {key} is already running");

                return;
            }
            financeviewmodel.timers[key] = new MainViewModel.TimerEntry
            {
                RemainingSeconds = seconds,
                OnTick = onTick,
                OnComplete = onComplete
            };

            // Fire initial tick so UI can show full duration immediately            
            //onTick.Invoke(key); <= Don't think I need this here
        }
        public static bool IsRunning(FinanceViewModel financeviewmodel, string key) => financeviewmodel.timers.ContainsKey(key);

        public static void CancelTimer(FinanceViewModel financeviewmodel, string key)
        {
            if (financeviewmodel.timers.TryGetValue(key, out var timer))
            {
                timer.OnComplete.Invoke(key, MainViewModel.TimerFinishReason.Cancelled);
                financeviewmodel.timers.Remove(key);
            }
            return;
        }

        // Not so sure this is ever needed?
        internal static int GetRemainingSeconds(FinanceViewModel financeviewmodel, string key)
        {
            if (financeviewmodel.timers.TryGetValue(key, out var timer))
            {
                return timer.RemainingSeconds;
            }
            return 0;
        }

    }
}
#endif