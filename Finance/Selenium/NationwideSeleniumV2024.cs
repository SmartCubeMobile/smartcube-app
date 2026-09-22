// https://stackoverflow.com/questions/78797531/cannot-use-javascriptexecutor-in-htmlunit-android-package
using System.Text.RegularExpressions;
using MoreLinq;
using static SmartCubeMobile.MainViewModel;



#if WINFORMS
using SmartDashboard;
using System.Windows.Threading;
using Windows.UI.Notifications.Management;
using Windows.UI.Notifications;
using OpenQA.Selenium;
#endif

#if WPF
using Windows.UI.Notifications.Management;
using Windows.UI.Notifications;
using OpenQA.Selenium;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Windows.Controls;
using System.Collections.Generic;
using static iText.StyledXmlParser.Jsoup.Select.Evaluator;
using System.Windows;
using static SmartCubeMobile.MainViewModel;
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
using OpenQA.Selenium;
#endif

#if ANDROID
using Android.Content;
using Android.Provider;
//using OpenQA.Selenium;
#endif


namespace SmartCubeMobile
{
#pragma warning disable CA1416
#if ANDROID

    // Should think about moving this somewhere Central??


    // https://learn.microsoft.com/en-us/dotnet/api/android.manifest.permission.receivesms?view=xamarin-android-sdk-12
    // https://stackoverflow.com/questions/61451407/creating-default-sms-app-in-xamarin-forms
    [BroadcastReceiver(Exported = true)]
    [IntentFilter(new[] { "android.provider.Telephony.SMS_RECEIVED" }, Priority = (int)IntentFilterPriority.HighPriority)]
    public class MyReceiver : BroadcastReceiver
    {
        public static readonly string INTENT_ACTION = "android.provider.Telephony.SMS_RECEIVED";
        protected string message, address = "";

        public override async void OnReceive(Context context, Intent intent)
        {
            if (intent.HasExtra("pdus"))
            {
                Java.Lang.Object[] smsArray = (Java.Lang.Object[])intent.Extras.Get("pdus");
                foreach (Java.Lang.Object item in smsArray)
                {
                    Android.Telephony.SmsMessage[] msgs = Telephony.Sms.Intents.GetMessagesFromIntent(intent);
                    List<Android.Telephony.SmsMessage> nwmsgs = new List<Android.Telephony.SmsMessage>();
                    foreach (Android.Telephony.SmsMessage msg in msgs)
                    {
                        if (msg.OriginatingAddress == "NATIONWIDE")
                        {
                            nwmsgs.Add(msg);
                        }
                    }
                    if (nwmsgs.Count > 0)
                    {
                        string smsotp = nwmsgs.Last().MessageBody.Replace("Use one-time code ", ""); // 779RCJ to log into the Internet Bank. Never share this code with anyone, only a fr "
                        smsotp = smsotp.Substring(0, 6).Trim();
                        Toast.MakeText(MainMeter.ourviewmodel.activity,
                                        "SMSOTP: " + smsotp,  // So show previous View 
                                        ToastLength.Short).Show();
                        await SmartRoutinesV2018.TextBlockUpdate(MainMeter.ourviewmodel, "SMSOTP" + smsotp, true);
                        // Process Accounts
                        //if (!await NationwideSeleniumV2024.Part2(MainMeter.ourviewmodel,
                        //            MainMeter.financeviewmodel,
                        //            smsotp,
                        //            MainMeter.financeviewmodel.temp_institution_code,
                        //            MainMeter.financeviewmodel.temp_brand_code,
                        //            MainMeter.financeviewmodel.transaction_types_found))
                        //{
                        //    return;
                        //}
                        //else
                        //{
                        //    // Post Process and Logout
                        //    if (!await NationwideSeleniumV2024.Part3(MainMeter.ourviewmodel,
                        //                    MainMeter.financeviewmodel))
                        //    {
                        //        return;
                        //    }
                        //}
                        break;
                        //MainMeter.ShowNotification(smsotp);
                    }
                    //Toast.MakeText(context, "Number: " + address + " Message: " + message, ToastLength.Short).Show();
                }
                return;
            }            
        }
    }
#endif

#if WINFORMS || WPF || UWP || WINUI
    public class NationwideSeleniumV2024
    {       
        internal static async Task<bool> RunNationwideSelenium(//object sender, RoutedEventArgs e,
#if WINFORMS
                                                MainProcess components,
                                                RichTextBox textBoxConsole,
#endif
                                                MainViewModel ourviewmodel,
                                                FinanceViewModel financeviewmodel,
                                                SmartFinance.Logins loginInfo,
                                                string start_url,
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
            // Warning about how *NOT* to waste your life!!!
            //await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "WARNING!!!!: " + "Google Chrome Zoom value must be < 80% for this to work!!!");
            try
            {
                WebDriver webDriver = SmartFinanceScrapeV2023.GetDriver();

                string Day = Parameter2.Substring(0, 2);
                string Month = SmartParametersV2016.months[Convert.ToInt16(Parameter2.Substring(2, 2)) - 1];
                string Year = Parameter2.Substring(4, 4);

                financeviewmodel.notificationHandledSource = new TaskCompletionSource<bool>();

                //Navigate to Nationwide
                webDriver.Url = start_url;

#if WPF || UWP || WINUI
                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Retrieved: " + start_url);
#endif
#if WINFORMS
                MainProcess.Output_Message(textBoxConsole, "Retrieved: " + start_url, false, false);
#endif
//#if ANDROID
//                      await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Retrieved: " + start_url);
//#endif
                //string title = financeviewmodel.webDriver.Title;
                //await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Title: " + title);

                // Find online banking link
                IWebElement link = webDriver.FindElement(By.CssSelector("[class^='LoginLinks__LoginLink']"));

                webDriver.Url = link.GetAttribute(SmartParametersV2016.href);

#if WPF || UWP || WINUI
                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Navigated to: " + webDriver.Url);

#endif
#if WINFORMS
                MainProcess.Output_Message(textBoxConsole, "Navigated to: " + webDriver.Url, false, false);
#endif
//#if ANDROID
//                      await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Navigated to: " + webDriver.Url);
//#endif
                // Use these later when we've logged-in (fingers crossed)
                Uri currentUri = new Uri(webDriver.Url); // create a Uri instance of it

                financeviewmodel.baseUrl = currentUri.Authority; // just get the "base" bit of the URL

                if (!NationwideFindInputs(webDriver,
                                                Parameter1,
                                                Day,
                                                Year,
                                                financeviewmodel))
                {
#if WPF || UWP || WINUI
                    await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Couldn't find main inputs");
#endif
#if WINFORMS
                    MainProcess.Output_Message(textBoxConsole, "Couldn't find main inputs", false, false);
#endif
//#if ANDROID
//                      await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Couldn't find main inputs");
//#endif
                    return false;
                }
                        
#if WPF || UWP || WINUI
                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Filled: Customer, Day and Year");
#endif
#if WINFORMS
                MainProcess.Output_Message(textBoxConsole, "Filled: Customer, Day and Year", false, false);
#endif
//#if ANDROID
//                   await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Filled: Customer, Day and Year");
//#endif
                // Month dropdown
                webDriver.FindElement(By.Name("DateOfBirthMonth")).SendKeys(Month);

#if WPF || UWP || WINUI
                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Filled: Dropdown " + Month);
#endif
#if WINFORMS
                MainProcess.Output_Message(textBoxConsole, "Filled: Dropdown " + Month, false, false);
#endif
//#if ANDROID
//                  await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Filled: Dropdown " + Month);
//#endif
                // Find all the buttons
                IReadOnlyCollection<IWebElement> firstbuttons = webDriver.FindElements(By.TagName("button"));

                foreach (IWebElement button in firstbuttons)
                {
                    if (button.Text != null)
                    {
                        string text = button.Text;
                        if (text.Trim() == "Continue")
                        {
                            // Left in - we might needs these one day! We need ALL the help we can get!
                            //Actions actions = new Actions(financeviewmodel.webDriver);
                            //actions.MoveToElement(button).Click().Perform();
                            IJavaScriptExecutor ex = (IJavaScriptExecutor)webDriver.ExecuteScript("arguments[0].click();", button);
                            break;
                        }
                    }
                }
#if WPF || UWP || WINUI
                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Coninue(1) button clicked");
#endif
#if WINFORMS
                MainProcess.Output_Message(textBoxConsole, "Coninue(1) button clicked", false, false);
#endif
//#if ANDROID
//                  await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Coninue(1) button clicked");
//#endif
                IReadOnlyList<IWebElement> radiobuttons = webDriver.FindElements(By.CssSelector("input[type='radio']"));
                foreach (IWebElement radiobutton in radiobuttons)
                {
                    string name = radiobutton.GetAttribute("value");
                    switch (name)
                    {
                        case "PassNumberAndSMSOTP":
                            if (!radiobutton.Selected)
                            {
                                IJavaScriptExecutor ex = (IJavaScriptExecutor)webDriver.ExecuteScript("arguments[0].click();", radiobutton);
                            }
                            break;
                        default:
                            break;
                    }
                }
#if WPF || UWP || WINUI
                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Radio button clicked");
#endif
#if WINFORMS
                MainProcess.Output_Message(textBoxConsole, "Radio button clicked", false, false);
#endif
//#if ANDROID
//                  await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Radio button clicked");
//#endif
                // Find and fill Passcodes
                if (!await NationwideFindPasscodes(
#if WINFORMS
                                        components,
                                        textBoxConsole,
#endif
                                                    ourviewmodel,
                                                    financeviewmodel,
                                                    webDriver,
                                                    Parameter3))
                {
#if WPF || UWP || WINUI
                    await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Problem with Passcodes: ");
#endif
#if WINFORMS
                    MainProcess.Output_Message(textBoxConsole, "Problem with Passcodes: ", false, false);
#endif
//#if ANDROID
//                      await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Problem with Passcodes: ");
//#endif
                    return false;
                }
#if WPF || UWP || WINUI
                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Passcodes: Filled");
#endif
#if WINFORMS
                MainProcess.Output_Message(textBoxConsole, "Passcodes: Filled", false, false);
#endif
//#if ANDROID
//                  await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Passcodes: Filled");
//#endif
                // Find all the buttons
                IReadOnlyCollection<IWebElement> finalbuttons = webDriver.FindElements(By.TagName("button"));
                foreach (IWebElement button in finalbuttons)
                {
                    if (button.Text != null)
                    {
                        string text = button.Text;
                        if (text.Trim() == "Continue")
                        {
                            // At this point we've found - possibly - the Continue
                            // button which, when pressed will get Nationwide to send
                            // us an SMSOTP.  THEREFORE we have to be able to receive it
                            // and to this effect we create the event to listen for it:
#if WINFORMS || WPF || UWP || WINUI
                            // Make up a key string
                            string key = $"Timer_{financeviewmodel.timerIdCounter++}";
                            // Add a two-minute timer
                            AddWaitTimer(ourviewmodel, financeviewmodel, key, SmartParametersV2016.serverTimeoutSecs,
                                onTick: k =>
                                {
                                    int remaining = GetRemainingSeconds(financeviewmodel, k);
                                    //label.Text = $"{k}: {remaining} second(s) remaining";
                                },
                                onComplete: async (k, reason) =>
                                {
                                    // Should only ever come here for normal timeout
                                    string Text = $"{k}: {(reason == TimerFinishReason.Completed ? "Finished" : "Cancelled")}";
                                    if (Text.Contains("Finished"))
                                    {
#if WPF || UWP || WINUI
                                        await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Completed: " + Text);
#endif
#if WINFORMS
                                        MainProcess.Output_Message(textBoxConsole, "Completed: " + Text, false, false);
#endif
//#if ANDROID
//                                          await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Completed: " + Text);
//#endif

                                        // Shutdown - we timed out without an SMSOTP
                                        if (await WebDriverDead(ourviewmodel, financeviewmodel, webDriver))
                                        // Mark the task as complete so the calling method continues
                                        {
                                            financeviewmodel.notificationHandledSource.TrySetResult(true);
                                        }
                                        return;
                                    }

                                    return;
                                }
                            );
                            ourviewmodel.notificationListener.NotificationChanged += (sender, e) => ListenerNotificationChanged(sender, e,
#if WINFORMS
                                                                                                                                components,
                                                                                                                                textBoxConsole,
#endif
                                                                                                                                ourviewmodel,
                                                                                                                                financeviewmodel,
                                                                                                                                webDriver,
                                                                                                                                key,
                                                                                                                                loginInfo,
                                                                                                                                notificationTitle,
                                                                                                                                notificationPrefix,
                                                                                                                                notificationTagLength,
                                                                                                                                institution_code,
                                                                                                                                brand_code,
                                                                                                                                transaction_groupsFound,
                                                                                                                                transaction_typesFound);
#endif
#if WPF || UWP || WINUI
                            await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Handler: Created");
#endif
#if WINFORMS
                            MainProcess.Output_Message(textBoxConsole, "Handler: Created", false, false);
#endif
                            //#if ANDROID
                            //                  await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Handler: Created");
                            //#endif
                            //
                            // I will forever be in the debt of BART WOJTALA whose solution at
                            // https://stackoverflow.com/questions/11908249/debugging-element-is-not-clickable-at-point-error
                            // saved my ass, my bacon and my sanity to allow me
                            // to 'click' through this last button and get this entire
                            // 15-year project back on track.  Thanks a million, Bart
                            //

                            // And press the Button to say 'Send me an OTP' please
                            IJavaScriptExecutor ex = (IJavaScriptExecutor)webDriver.ExecuteScript("arguments[0].click();", button);
                            break;
                        }
                    }
                }
#if WPF || UWP || WINUI
                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Continue(2) button clicked");
#endif
#if WINFORMS
                MainProcess.Output_Message(textBoxConsole, "Continue(2) button clicked", false, false);
#endif
//#if ANDROID
//                  await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Continue(2) button clicked");
//#endif
#if WPF || UWP || WINUI
                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Setting timer for 2 mins");
#endif
#if WINFORMS
                MainProcess.Output_Message(textBoxConsole, "Setting timer for 2 mins", false, false);
#endif
//#if ANDROID
//                  await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Setting timer for 2 mins");
//#endif                
#if WPF || UWP || WINUI
                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Waiting for a notification ...");
#endif
#if WINFORMS
                MainProcess.Output_Message(textBoxConsole, "Waiting for a notification ...", false, false);
#endif
//#if ANDROID
//                  await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Waiting for a notification ...");
//#endif
                 await financeviewmodel.notificationHandledSource.Task;
             }
            // So ... at this point we leave the main calling routine
            // because we are waiting for the SMSOTP to be delivered
            // to the Listener event, which, when it has received the
            // SMS does all the heavy lifting
            catch (Exception ex)
            {
#if WPF || UWP || WINUI
                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Problem: " + ex.Message);
#endif
#if WINFORMS
                MainProcess.Output_Message(textBoxConsole, "Problem: " + ex.Message, false, false);
#endif
//#if ANDROID
//                  await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Problem: " + ex.Message);
//#endif
                return false;
            }
#if WPF || UWP || WINUI
            await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Returning ... ");
#endif
#if WINFORMS
            MainProcess.Output_Message(textBoxConsole, "Returning ... ", false, false);
#endif
//#if ANDROID
//           await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Returning ... ");
//#endif
            financeviewmodel.errorMessage = ""; // Because failing in SmartPhyll!
            return true;
        }

#if WINFORMS || WPF || UWP || WINUI
        public static async void AddWaitTimer(MainViewModel ourviewmodel, FinanceViewModel financeviewmodel, string key, int seconds, Action<string> onTick, Action<string, TimerFinishReason> onComplete)
        {
            if (IsRunning(financeviewmodel, key))
            {
                MessageBox.Show($"Timer {key} is already running.");
#if WPF || UWP || WINUI
                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, $"Timer {key} is already running");
#endif
//#if WINFORMS
//                MainProcess.Output_Message(textBoxConsole, $"Timer {key} is already running", false, false);
//#endif
//#if ANDROID
//              await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, $"Timer {key} is already running");
//#endif
                return;
            }
            financeviewmodel.timers[key] = new TimerEntry
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
                timer.OnComplete.Invoke(key, TimerFinishReason.Cancelled);
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
        internal static async void ListenerNotificationChanged(

            UserNotificationListener sender, UserNotificationChangedEventArgs args,

#if WINFORMS
                                                                    MainProcess components, 
                                                                    RichTextBox textBoxConsole,
                                                                    
#endif
                                                                    MainViewModel ourviewmodel,
                                                                    FinanceViewModel financeviewmodel,
                                                                    WebDriver webDriver,
                                                                    string key,
                                                                    SmartFinance.Logins loginInfo,
                                                                    string notificationTitle,
                                                                    string notificationPrefix,
                                                                    short notificationTagLength,
                                                                    short institution_code,
                                                                    short brand_code,
                                                                    List<SmartFinance.Transaction_Groups> transaction_groupsFound,
                                                                    List<SmartFinance.Transaction_Types> transaction_typesFound) // Relevant to 1001/1001


        {
            //
            // Part 1
            //

            // Cancel the waiting timer FIRST!
            CancelTimer(financeviewmodel, key);

#if WPF || UWP || WINUI
            await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Handler Event: " + "Delivered");
#endif
#if WINFORMS
            await Task.Run(() => components.BeginInvoke(() => MainProcess.Output_Message(textBoxConsole, "Handler Event: " + "Delivered", false, false)));
#endif
//#if ANDROID
//            await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Handler Event: " + "Delivered");
//#endif
            if (!await SmartRoutinesV2018.CheckTrace(ourviewmodel, financeviewmodel.financeToken, institution_code, brand_code, "Handler: event delivered"))
            {
                return;
            }

            // Get the toast notifications
            //IAsyncOperation<UserNotification> notifsx = (IAsyncOperation<UserNotification>)await sender.GetNotificationsAsync(NotificationKinds.Toast);
            //await sender.GetNotificationsAsync(Windows.UI.Notifications.NotificationKinds.Toast);

            IReadOnlyList<UserNotification> notifs = await sender.GetNotificationsAsync(NotificationKinds.Toast);

            // Try and make sure the latest notification is First
            IReadOnlyList<UserNotification> notifs_sorted = (from NT in notifs
                                                             orderby NT.CreationTime descending
                                                             select NT).ToList();

            // Select the first notification
            if (notifs_sorted.Count == 0)
            {
#if WPF || UWP || WINUI
                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "No User Notifications found ");
#endif
#if WINFORMS
                await Task.Run(() => components.BeginInvoke(() => MainProcess.Output_Message(textBoxConsole, "No User Notifications found ", false, false)));
#endif
//#if ANDROID
//                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "No User Notifications found ");
//#endif
                financeviewmodel.errorMessage = "No User Notifications found";
                return;
            }
            foreach (UserNotification usernot in notifs_sorted)
            {
                UserNotification notif = usernot;
                // Get the toast binding, if present
                NotificationBinding toastBinding = notif.Notification.Visual.GetBinding(KnownNotificationBindings.ToastGeneric);
                if (toastBinding == null)
                {
                    continue;
                }
                // And then get the text elements from the toast binding
                IReadOnlyList<AdaptiveNotificationText> textElements = toastBinding.GetTextElements();

                // Treat the first text element as the title text
                string titleText = textElements.FirstOrDefault().Text;
                if (notificationTitle != titleText)
                {
                    continue;
                }

#if WPF || UWP || WINUI
                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Found: " + titleText);
#endif
#if WINFORMS
                await Task.Run(() => components.BeginInvoke(() => MainProcess.Output_Message(textBoxConsole, "Found: " + titleText, false, false)));
#endif
//#if ANDROID
//                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Found: " + titleText);
//#endif
                // We'll treat all subsequent text elements as body text,
                // joining them together via newlines.
                string bodyText = string.Join("\n", textElements.Skip(1).Select(t => t.Text));
                if (!bodyText.Contains(notificationPrefix))
                {
                    continue;
                }

                // Ok, we've found a NATIONWIDE, so say we don't want to invoke
                // this event handler any more!
                ourviewmodel.notificationListener.NotificationChanged -= null;
#if WPF || UWP || WINUI
                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Handler Event: " + "Removed");
#endif
#if WINFORMS
                await Task.Run(() => components.BeginInvoke(() => MainProcess.Output_Message(textBoxConsole, "Handler Event: " + "Removed", false, false)));
#endif
//#if ANDROID
//                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Handler Event: " + "Removed");
//#endif
                bodyText = bodyText.Replace(notificationPrefix, "").Trim(); //Trim gets rid of any spaces
                if (bodyText.Length <= 6)
                {
                    continue;
                }
                string notificationContent = bodyText.Substring(0, notificationTagLength);

                string oneTimePasscode = notificationContent;

                // Process Accounts
                if (!await Part2(
#if WINFORMS
                                    components,
                                    textBoxConsole,
#endif
                                    ourviewmodel,
                                    financeviewmodel,
                                    webDriver,
                                    loginInfo,
                                    oneTimePasscode,
                                    institution_code,
                                    brand_code,
                                    transaction_groupsFound,
                                    transaction_typesFound))
                {
                    return;
                }
                else
                {
                    // Post Process and Logout
                    if (!await Part3(
#if WINFORMS
                                    components,
                                    textBoxConsole,
#endif
                                    ourviewmodel,
                                    financeviewmodel,
                                    webDriver))
                    {
                        return;
                    }
                    if (await WebDriverDead(ourviewmodel, financeviewmodel, webDriver))
                    {
                        break;
                    }
                }
                break;
            }
            // Mark the task as complete so the calling method continues
            financeviewmodel.notificationHandledSource.TrySetResult(true);
            return;
        }
#endif

        internal static async Task<bool> Part2(
#if WINFORMS
                                            MainProcess components,
                                            RichTextBox textBoxConsole,
#endif
                                            MainViewModel ourviewmodel,
                                            FinanceViewModel financeviewmodel,
                                            WebDriver webDriver,
                                            SmartFinance.Logins loginInfo,
                                            string oneTimePasscode,
                                            short institution_code,
                                            short brand_code,
                                            List<SmartFinance.Transaction_Groups> transaction_groupsFound,
                                            List<SmartFinance.Transaction_Types> transaction_typesFound)
        {
            if (string.IsNullOrEmpty(oneTimePasscode))
            {
                // Hardly likely to happen
#if WPF || UWP || WINUI
                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Notification content: " + "Empty");
#endif
#if WINFORMS
                await Task.Run(() => components.BeginInvoke(() => MainProcess.Output_Message(textBoxConsole, "Notification content: " + "Empty", false, false)));
#endif
//#if ANDROID
//              await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Notification content: " + "Empty");
//#endif
                financeviewmodel.errorMessage = "Notification content is empty";
                return false;
            }

            try
            {
                IWebElement smsotp = webDriver.FindElement(By.CssSelector("input[name='OneTimePasscode']"));
                if (smsotp == null)
                {
                    // Hardly likely to happen, either
#if WPF || UWP || WINUI
                    await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "SMSOTP input field: " + "Not found");
#endif
#if WINFORMS
                    await Task.Run(() => components.BeginInvoke(() => MainProcess.Output_Message(textBoxConsole, "SMSOTP input field: " + "Not found", false, false)));
#endif
//#if ANDROID
//                    await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "SMSOTP input field: " + "Not found");
//#endif
                    financeviewmodel.errorMessage = "Couldn't find the SMSOTP input";
                    return false;
                }

#if WPF || UWP || WINUI
                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Inserting: " + oneTimePasscode);
#endif
#if WINFORMS
                await Task.Run(() => components.BeginInvoke(() => MainProcess.Output_Message(textBoxConsole, "Inserting: " + oneTimePasscode, false, false)));
#endif
//#if ANDROID
//                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Inserting: " + oneTimePasscode);
//#endif
                smsotp.SendKeys(oneTimePasscode);
#if WPF || UWP || WINUI
                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Input Field: " + oneTimePasscode + " filled");
#endif
#if WINFORMS
                await Task.Run(() => components.BeginInvoke(() => MainProcess.Output_Message(textBoxConsole, "Input Field: " + oneTimePasscode + " filled", false, false)));
#endif
//#if ANDROID
//                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Input Field: " + oneTimePasscode + " filled");
//#endif
                financeviewmodel.loggedIn = false;

                IReadOnlyList<IWebElement> buttons = webDriver.FindElements(By.TagName("button"));
                if (buttons == null)
                {
#if WPF || UWP || WINUI
                    await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "No buttons found");
#endif
#if WINFORMS
                    await Task.Run(() => components.BeginInvoke(() => MainProcess.Output_Message(textBoxConsole, "No buttons found", false, false)));
#endif
//#if ANDROID
//                    await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "No buttons found");
//#endif
                    financeviewmodel.errorMessage = "Couldn't find any buttons";
                    return false;
                }

                IWebElement loginButton = FindButton(financeviewmodel,
                                                        buttons,
                                                        "Log in");
                if (loginButton == null)
                {
#if WPF || UWP || WINUI
                    await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Can't find Login button");
#endif
#if WINFORMS
                    await Task.Run(() => components.BeginInvoke(() => MainProcess.Output_Message(textBoxConsole,  "Can't find Login button", false, false)));
#endif
//#if ANDROID
//                    await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel,  "Can't find Login button");
//#endif
                    return false;
                }

                try
                {
                    IJavaScriptExecutor ex = (IJavaScriptExecutor)webDriver.ExecuteScript("arguments[0].click();", loginButton);
#if WPF || UWP || WINUI
                    await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Log in button: " + "Clicked");
#endif
#if WINFORMS
                    await Task.Run(() => components.BeginInvoke(() => MainProcess.Output_Message(textBoxConsole, "Log in button: " + "Clicked", false, false)));
#endif
//#if ANDROID
//                  await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Log in button: " + "Clicked");
//#endif
                }
                catch (Exception ex)
                {
                    financeviewmodel.errorMessage = ex.Message;
                    return false;
                }
                financeviewmodel.loggedIn = true;
                financeviewmodel.accountItems = new List<FinanceViewModel.AccountItem>();

                financeviewmodel.currentUrl = webDriver.Url;

                //      Find the Address <a> before all this shit goes haywire ..
                //      ILocator managerows = financeviewmodel.nwpage.Locator("a[class^='nav-link-unique-']"); // ("a[id^='primary-nav-title-level-1-sibling-1-unique-']");
                //      ILocator managerows = financeviewmodel.webPage.Locator("a[id^='primary-nav-link-level-3-sibling-1-unique-']");
                // Find owner/Welcome here?
                string owner = "";
                financeviewmodel.UDPRN = "";
                IWebElement welcome = webDriver.FindElement(By.Id("welcome-message"));
                if (welcome != null)
                {
                    if (welcome.Text != null)
                    {
                        owner = welcome.Text.Replace("Welcome back,", "").Trim();
                    }
                }
                // Find address here?
                if (!await FindAddress(
#if WINFORMS
                                        components,
                                        textBoxConsole,
#endif
                                        ourviewmodel,
                                        financeviewmodel,
                                        webDriver,
                                        loginInfo,
                                        owner))
                {
#if WPF || UWP || WINUI
                    await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Find address: " + "Failed");
#endif
#if WINFORMS
                    await Task.Run(() => components.BeginInvoke(() => MainProcess.Output_Message(textBoxConsole, "Find address: " + "Failed", false, false)));
#endif
//#if ANDROID
//                   await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Find address: " + "Failed");
//#endif
                    return false;
                }
                else
                {
                    // Even though we find the Owner first, the UDPRN is far more important
#if WPF || UWP || WINUI
                    await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Owner: " + loginInfo.OWNER);
#endif
#if WINFORMS
                    await Task.Run(() => components.BeginInvoke(() => MainProcess.Output_Message(textBoxConsole, "Owner: " + loginInfo.OWNER, false, false)));
#endif
//#if ANDROID
//                   await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Owner: " + financeviewmodel.loginInfo.OWNER);
//#endif
                    // These are 'nice to haves'
#if WPF || UWP || WINUI
                    await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Contact no.: " + loginInfo.CONTACT_PHONENO);
#endif
#if WINFORMS
                    await Task.Run(() => components.BeginInvoke(() => MainProcess.Output_Message(textBoxConsole, "Contact no.: " + loginInfo.CONTACT_PHONENO, false, false)));
#endif
//#if ANDROID
//                    await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Contact no.: " + financeviewmodel.loginInfo.CONTACT_PHONENO);
//#endif
#if WPF || UWP || WINUI
                    await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Email address: " + loginInfo.CONTACT_EMAIL);
#endif
#if WINFORMS
                    await Task.Run(() => components.BeginInvoke(() => MainProcess.Output_Message(textBoxConsole, "Email address: " + loginInfo.CONTACT_EMAIL, false, false)));
#endif
//#if ANDROID
//                   await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Email address: " + financeviewmodel.loginInfo.CONTACT_EMAIL);
//#endif
                    // Check the contents of the LoginInfo
                }

                financeviewmodel.account_id = 0;
                financeviewmodel.AccountLinks = new List<KeyValuePair<string, string>>();

                if (!await NationwideBuildAccountList(
#if WINFORMS
                                                    components,
                                                    textBoxConsole,
#endif
                                                    ourviewmodel,
                                                    financeviewmodel,
                                                    webDriver,
                                                    institution_code,
                                                    brand_code,
                                                    financeviewmodel.UDPRN,
                                                    transaction_groupsFound,
                                                    transaction_typesFound))
                {
                    //Update UI here
#if WPF || UWP || WINUI
                    await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, financeviewmodel.errorMessage);
#endif
#if WINFORMS
                    await Task.Run(() => components.BeginInvoke(() => MainProcess.Output_Message(textBoxConsole, financeviewmodel.errorMessage, false, false)));
#endif
//#if ANDROID
//                   await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, financeviewmodel.errorMessage);
//#endif
                    return false;
                }
                // We only ever do ONE Login for ONE notification
#if WPF || UWP || WINUI
                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Transactions inserted: " + financeviewmodel.transactions_count);
#endif
#if WINFORMS
                await Task.Run(() => components.BeginInvoke(() => MainProcess.Output_Message(textBoxConsole, "Transactions inserted: " + financeviewmodel.transactions_count, false, false)));
#endif
//#if ANDROID
//                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Transactions inserted: " + financeviewmodel.transactions_count);
//#endif
            }
            catch (Exception ex)
            {
                // Hardly likely to happen, either
#if WPF || UWP || WINUI
                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "SMSOTP input field: " + "Not found");
#endif
#if WINFORMS
                await Task.Run(() => components.BeginInvoke(() => MainProcess.Output_Message(textBoxConsole, "SMSOTP input field: " + "Not found", false, false)));
#endif
//#if ANDROID
//                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "SMSOTP input field: " + "Not found");
//#endif
                financeviewmodel.errorMessage = "Couldn't find the SMSOTP input " + ex.Message;
            }
            return true;
        }

        internal static async Task<bool> Part3(
#if WINFORMS
                                    MainProcess components,
                                    RichTextBox textBoxConsole,
#endif
                                    MainViewModel ourviewmodel,
                                    FinanceViewModel financeviewmodel,
                                    WebDriver webDriver)
        {
            // Reset this - just to make sure we return to
            // "AccountsList" every time!!
            try
            {
                webDriver.Url = financeviewmodel.currentUrl;

                // Because we are logged in,
                // No need to process any more buttons!
                IWebElement logoutButton = webDriver.FindElement(By.CssSelector("[class='log-out-link']"));
                if (logoutButton == null)
                {
#if WPF || UWP || WINUI
                    await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "No buttons found");
#endif
#if WINFORMS
                    await Task.Run(() => components.BeginInvoke(() => MainProcess.Output_Message(textBoxConsole, "No buttons found", false, false)));
#endif
//#if ANDROID
//                  await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "No buttons found");
//#endif
                    financeviewmodel.errorMessage = "Couldn't find any buttons";
                    return false;
                }
            
                IJavaScriptExecutor ex = (IJavaScriptExecutor)webDriver.ExecuteScript("arguments[0].click();", logoutButton);

#if WPF || UWP || WINUI
                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Logout button: " + "Clicked");
#endif
#if WINFORMS
                await Task.Run(() => components.BeginInvoke(() => MainProcess.Output_Message(textBoxConsole, "Logout button: " + "Clicked", false, false)));
#endif
//#if ANDROID
//                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Logout button: " + "Clicked");
//#endif
                

//#if WPF || UWP || WINUI
//                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Webdriver: " + "Dead and buried ..");
//#endif
#if WINFORMS
                await Task.Run(() => components.BeginInvoke(() => MainProcess.Output_Message(textBoxConsole, "Webdriver: " + "Dead and buried ..", false, false)));
#endif
//#if ANDROID
//               await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Webdriver: " + "Dead and buried ..");
//#endif
            }
            catch (Exception ex)
            {
                financeviewmodel.errorMessage = ex.Message;
#if WPF || UWP || WINUI
                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Logout button click: " + ex.Message);
#endif
#if WINFORMS
                await Task.Run(() => components.BeginInvoke(() => MainProcess.Output_Message(textBoxConsole, "Logout button click: " + ex.Message, false, false)));
#endif
//#if ANDROID
//               await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Logout button click: " + ex.Message);
//#endif
                return false;
            }
            return true;
        }


        internal static async Task<bool> WebDriverDead(MainViewModel ourviewmodel,
                                                FinanceViewModel financeviewmodel,
                                                WebDriver webDriver)
        {
            //webDriver.Close();

#if WPF || UWP || WINUI
            await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "WebDriver: " + "Quit completed");
#endif
//#if WINFORMS
//            await Task.Run(() => components.BeginInvoke(() => MainProcess.Output_Message(textBoxConsole, "WebDriver: " + "Quit completed", false, false)));
//#endif
//#if ANDROID
//          await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "WebDriver: " + "Quit completed");
//#endif

            //financeviewmodel.service.Dispose(); // Yes, chatGPT says so
#if WPF || UWP || WINUI
            await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Service: " + "Dispose completed");
#endif
//#if WINFORMS
//            await Task.Run(() => components.BeginInvoke(() => MainProcess.Output_Message(textBoxConsole, "Service: " + "Dispose completed", false, false)));
//#endif
//#if ANDROID
//          await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Service: " + "Dispose completed");
//#endif
            return true;
        }

        internal static IWebElement FindButton(FinanceViewModel financeviewmodel,
                                                IReadOnlyList<IWebElement> buttons,
                                                string buttonText)
        {
            foreach (IWebElement button in buttons)
            {
                try
                {
                    if (button.Text != null)
                    {
                        string text = button.Text;
                        if (text.Trim() == buttonText)
                        {
                            return button;
                        }
                    }
                }
                catch (Exception ex)
                {
                    financeviewmodel.errorMessage = "Problem finding button: " + buttonText + ex.Message;
                }
            }
            return null;
        }

        //Scrape the webpage version
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
#if WPF || UWP || WINUI
                            await SmartBobV2017.ListenerAsync(ourviewmodel, financeviewmodel.financeToken, institutionCode, brandCode, "NW Trans Lookup", "Trans type  failed: " + transaction_type);
#endif
#if WINFORMS
                            await Task.Run(() => components.BeginInvoke(() => MainProcess.Output_Message(textBoxConsole, "NW Trans Lookup: Trans type  failed: " + transaction_type, false, false)));
#endif
//#if ANDROID
//                            await SmartBobV2017.ListenerAsync(ourviewmodel, financeviewmodel.financeToken, institution_code, brand_code, "NW Trans Lookup", "Trans type  failed: " + transaction_type);
//#endif
                            // Carry on
                        }
                        else
                        {
                            if (transCodes[0] == 0 || transCodes[1] == 0)
                            {
#if WPF || UWP || WINUI
                                await SmartBobV2017.ListenerAsync(ourviewmodel, financeviewmodel.financeToken, institutionCode, brandCode, "NW Trans Lookup", "Trans type  failed: " + transaction_type);
#endif
#if WINFORMS
                                await Task.Run(() => components.BeginInvoke(() => MainProcess.Output_Message(textBoxConsole, "NW Trans Lookup: Trans type  failed: " + transaction_type, false, false)));
#endif
//#if ANDROID
//                              await SmartBobV2017.ListenerAsync(ourviewmodel, financeviewmodel.financeToken, institution_code, brand_code, "NW Trans Lookup", "Trans type  failed: " + transaction_type);
//#endif
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
#if WPF || UWP || WINUI
                if (!await SmartRoutinesV2018.CheckTrace(ourviewmodel, financeviewmodel.financeToken, institutionCode, brandCode, "NationwideTransactions" + ": " + ex.Message))
                {
                    return false;
                }
#endif
#if WINFORMS
                await Task.Run(() => components.BeginInvoke(() => MainProcess.Output_Message(textBoxConsole, "NationwideTransactions" + ": " + ex.Message, false, false)));
#endif
//#if ANDROID
//                await SmartRoutinesV2018.CheckTrace(ourviewmodel, financeviewmodel.financeToken, institution_code, brand_code, "NationwideTransactions" + ": " + ex.Message);
//#endif
                return false;
            }
            return true;
        }

        internal static async Task<bool> NationwideBuildAccountList(
#if WINFORMS
                                                    MainProcess components,
                                                    RichTextBox textBoxConsole,
#endif
                                                    MainViewModel ourviewmodel,
                                                    FinanceViewModel financeviewmodel,
                                                    WebDriver webDriver,
                                                    short institution_code,
                                                    short brand_code,
                                                    string UDPRN,
                                                    List<SmartFinance.Transaction_Groups> transaction_groupsFound,
                                                    List<SmartFinance.Transaction_Types> transaction_typesFound)
        {
            string accountInfo1 = "";
            string accountInfo2;
            string accountName = "";
            string balance = "";
            string symbol = "";
            char categoryCode = SmartParametersV2016.defaultChar;

            // Accounts List
            try
            {
                webDriver.Url = financeviewmodel.currentUrl;
                IWebElement offs = webDriver.FindElement(By.CssSelector("[class='active']"));
                if (offs != null)
                {
                    IReadOnlyList<IWebElement> links = offs.FindElements(By.TagName("a"));
                    if (links != null)
                    {
                        foreach (IWebElement link in links)
                        {
                            string href = link.GetAttribute(SmartParametersV2016.href);
                            if (href.Contains("/AccountList/Account/RedirectToDefaultPage"))
                            {
                                string innerHtml = link.GetAttribute("innerHTML");
                                if (innerHtml != null)
                                {
                                    short accountOrdinal = 0;
                                    categoryCode = SmartParametersV2016.defaultChar;

                                    NewAccountBalance(financeviewmodel,
                                                        innerHtml,
                                                        ref accountInfo1,
                                                        ref balance,
                                                        ref symbol);
                                    if (string.IsNullOrEmpty(accountInfo1))
                                    {
                                        financeviewmodel.errorMessage = "Couldn't parse account info: " + accountInfo1;
                                        return false;
                                    }
                                    // ourInfo returns AccountName + fs + SortCode + space + AccountNo
                                    accountInfo2 = SplitAccountName(financeviewmodel,
                                                                accountInfo1,
                                                                ref accountName);
                                    if (accountName == "")
                                    {
                                        financeviewmodel.errorMessage = "Account name empty";
                                        return false;
                                    }
                                    if (accountInfo2 == "")
                                    {
                                        financeviewmodel.errorMessage = "Account sortcode/account empty";
                                        return false;
                                    }


                                    // Processing Account details (not transactions) ...
                                    short currency_ordinal = SmartSpikeV2017.GetCurrencyOrdinal(ourviewmodel, symbol);

                                    categoryCode = SmartParametersV2016.Banks; //default
                                    string sortcode = "",
                                            account_no = "";
                                    if (accountName.Contains("Savings") ||
                                        accountName.Contains("savings"))
                                    {
                                        categoryCode = SmartParametersV2016.Savings;
                                    }

                                    string[] accstuff = accountInfo2.Split(SmartParametersV2016.spacechar);
                                    if (accstuff.Length > 1)
                                    {
                                        sortcode = accstuff[0];
                                        // Be Bold!!!
                                        if (sortcode != "")
                                        {
                                            sortcode.Replace("-", "");
                                        }
                                        account_no = accstuff[1];
                                    }
                                    else
                                    {
                                        account_no = accstuff[0];
                                    }

                                    if (!await DecodeAccount(ourviewmodel,
                                                financeviewmodel,
                                                accountName,
                                                sortcode,
                                                account_no,
                                                currency_ordinal,
                                                institution_code,
                                                brand_code,
                                                UDPRN,
                                                categoryCode))
                                    {
                                        return false;
                                    }

                                    // This is where we get the transactions
                                    if (!await ProcessAccount(
#if WINFORMS
                                                                components,
                                                                textBoxConsole,
#endif
                                                                ourviewmodel,
                                                                financeviewmodel,
                                                                webDriver,
                                                                categoryCode,
                                                                href,
                                                                institution_code,
                                                                brand_code,
                                                                sortcode,
                                                                account_no,
                                                                financeviewmodel.UDPRN,
                                                                accountOrdinal,
                                                                symbol,
                                                                transaction_groupsFound,
                                                                transaction_typesFound))
                                    {
                                        // There may well have been problems, but carry
                                        // on looping round trying to do the best we can
#if WPF || UWP || WINUI
                                        await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Process Account: " + financeviewmodel.errorMessage);
#endif
#if WINFORMS
                                        await Task.Run(() => components.BeginInvoke(() => MainProcess.Output_Message(textBoxConsole, "Process link: " + financeviewmodel.errorMessage, false, false)));
#endif
//#if ANDROID
//                                      await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Process link: " + financeviewmodel.errorMessage);
//#endif
                                    }
                                    financeviewmodel.foundActiveX = true;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                financeviewmodel.errorMessage =   ex.Message;
            }
            return true;
        }

        internal static async Task<bool> ProcessAccount(
#if WINFORMS
                                                        MainProcess components,
                                                        RichTextBox textBoxConsole,
#endif
                                        MainViewModel ourviewmodel,
                                        FinanceViewModel financeviewmodel,
                                        WebDriver webDriver,
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
                // Go to the transactions page
                webDriver.Url = href;

                IWebElement classAnchor = webDriver.FindElement(By.Id("last-12-month-btn"));
                if (classAnchor == null)
                {
#if WPF || UWP || WINUI
                    await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Class anchor: " + "is null");
#endif
#if WINFORMS
                    await Task.Run(() => components.BeginInvoke(() => MainProcess.Output_Message(textBoxConsole, "Class anchor: " + "is null", false, false)));
#endif
//#if ANDROID
//                  await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Class anchor: " + "is null");
//#endif
                    return false;
                }

                IJavaScriptExecutor ex1 = (IJavaScriptExecutor)webDriver.ExecuteScript("arguments[0].click();", classAnchor);
                Thread.Sleep(5000);
                IWebElement body = webDriver.FindElement(By.TagName("tbody"));

                string responseData = body.GetAttribute("innerText");
                // Was carriagereturn, now newline
                string[] transactions = responseData.Split(SmartParametersV2016.newline);
#if WPF || UWP || WINUI
                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Transactions found: " + transactions.Length);
#endif
#if WINFORMS
                await Task.Run(() => components.BeginInvoke(() => MainProcess.Output_Message(textBoxConsole, "Transactions found: " + transactions.Length, false, false)));
#endif
//#if ANDROID
//              await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Transactions found: " + transactions.Length);
//#endif

                if (transactions.Length > 0)
                {
                    DateTime accountCreated = SmartParametersV2016.defaultDate;
                    List<SmartFinance.Accounts> accounts_found =
                        new List<SmartFinance.Accounts>
                        (from Account in financeviewmodel.PLO.finance_accountsList
                         where Account.INSTITUTION_CODE == institution_code &&
                         Account.BRAND_CODE == brand_code &&
                         Account.SORTCODE == sortcode &
                         Account.ACCOUNT_NO == account_no &&
                         Account.UDPRN == UDPRN
                         orderby Account.ACCOUNT_CREATED descending
                         select Account).ToList();
                    if (accounts_found.Count > 0)
                    {
                        accountCreated = accounts_found.First().ACCOUNT_CREATED;
                    }
                    // Fix this then increment it to ensure uniqueness
                    financeviewmodel.sequence_no = SmartFinanceV2025.UnixSequenceNo();
                    await NationwideTransactions(
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
                                            accountCreated);
                }


                // This is the old stuff whereby I try to get ALL the possible
                // transactions I can from 14 months ago.  However ... I am SO
                // TIRED and SO FED UP with trying to make this work, that I
                // have given up and am just going to press the last 12 months
                // button as I do above.  The other reason is the Nationwide
                // chimps might very well CHANGE the from date without telling
                // me so I never know if my from calculations are ALWAYS going
                // to be valid.  At least the 'last 12 months' button should
                // always work ...
                

//                elementX = "Enter element";
//                IWebElement enterdate = financeviewmodel.webDriver[financeviewmodel.brandScrapeIndex].FindElement(By.CssSelector("[id='enter-date-reveal-link'"));// Checked
//                IJavaScriptExecutor ex3 = (IJavaScriptExecutor)financeviewmodel.webDriver[financeviewmodel.brandScrapeIndex].ExecuteScript("arguments[0].click();", enterdate);
//                //ex3.ExecuteScript("arguments[0].click();", enterdate);

//                // Find input with id statement-from-date
//                elementX = "From element";
//                IWebElement fromdate = financeviewmodel.webDriver[financeviewmodel.brandScrapeIndex].FindElement(By.CssSelector("[id='statement-from-date'"));
//                fromdate.Clear();
//                string text1 = fromdate.GetAttribute("value");
                    
//#if WPF || UWP || WINUI
//                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Dates: " + text1 + " to " + DateTime.UtcNow.ToShortDateString());
//#endif
//#if WINFORMS
//                    await Task.Run(() => components.BeginInvoke(() => MainProcess.Output_Message(textBoxConsole, "Dates: " + text1 + " to " + DateTime.UtcNow.ToShortDateString(), false, false)));
//#endif
//                //#if ANDROID
//                //                        await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Dates: " + text1 + " to " + DateTime.UtcNow.ToShortDateString());
//                //#endif
//                elementX = "Update element";
//                IWebElement update = financeviewmodel.webDriver[financeviewmodel.brandScrapeIndex].FindElement(By.CssSelector("[id='date-filter-update'"));
//                IJavaScriptExecutor ex2 = (IJavaScriptExecutor)financeviewmodel.webDriver[financeviewmodel.brandScrapeIndex];
//                ex2.ExecuteScript("arguments[0].click();", update);
                    
                // This is the old JSON stuff. Who knows? One day it may come back
                
                //responseData = body.GetAttribute("innerHTML");
                //dynamic jsonResponse = JObject.Parse(responseData);
                //foreach (dynamic statement in jsonResponse)
                //{
                //    string statement_name = statement.Name;
                //    if (statement_name == "Transactions")
                //    {
                //        foreach (dynamic transactions in statement)
                //        {
                //            foreach (dynamic transaction in transactions)
                //            {
                //                foreach (dynamic individual in transaction)
                //                {
                //                    string column = individual.Name;
                //                    switch (column)
                //                    {
                //                        case "a":
                //                            break;
                //                        default:
                //                            Console.Write(column);
                //                            break;
                //                    }

                //                }
                //            }
                //        }
                //    }
                //}
            }
            catch (Exception)
            {
                // Almost invariably, these are because "FindElements" fails
                // to find a particular id or link
                // DON'T put it on financeviewmodel.errorMessage cos its not fatal!!
#if WPF || UWP || WINUI
                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "No Last 12 months button");
#endif
#if WINFORMS
                await Task.Run(() => components.BeginInvoke(() => MainProcess.Output_Message(textBoxConsole, "No Last 12 months button", false, false)));
#endif
//#if ANDROID
//                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "No Last 12 months button");
//#endif
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
                                            short institution_code,
                                            short brand_code,
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
                                                                    institution_code,
                                                                    brand_code,
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
                                                            institution_code,
                                                            brand_code,
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
                        financeviewmodel.errorMessage = "Cannot update accounts - cannot continue";
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

        internal static async Task<bool> FindAddress(
#if WINFORMS
                                                    MainProcess components,
                                                    RichTextBox textBoxConsole,
#endif
                                                    MainViewModel ourviewmodel,
                                                    FinanceViewModel financeviewmodel,
                                                    WebDriver webDriver,
                                                    SmartFinance.Logins loginInfo,
                                                    string owner)
        {
            string anchorhref;
            financeviewmodel.UDPRN = "";    // Everything depends on finding this!

            IReadOnlyList<IWebElement> domATags = webDriver.FindElements(By.TagName("a"));
            foreach (IWebElement domATag in domATags)
            {
                if (domATag != null)
                {
                    anchorhref = domATag.GetAttribute(SmartParametersV2016.href);
                    if (anchorhref != null)
                    {
                        if (anchorhref.Contains("MaintainTelephoneAndAddress"))
                        {
                            // Now try and get the address page
                            try
                            {
                                webDriver.Url = domATag.GetAttribute(SmartParametersV2016.href);

#if WPF || UWP || WINUI
                                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Navigated to: " + webDriver.Url);
#endif
#if WINFORMS
                                await Task.Run(() => components.BeginInvoke(() => MainProcess.Output_Message(textBoxConsole, "Navigated to: " + webDriver.Url, false, false)));
#endif
//#if ANDROID
//                              await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Navigated to: " + webDriver.Url);
//#endif
                                financeviewmodel.UDPRN = await FindDetails(ourviewmodel,
                                                                            financeviewmodel,
                                                                            webDriver,
                                                                            loginInfo,
                                                                            owner);
                                break;
                            }
                            catch (Exception ex)
                            {
#if WPF || UWP || WINUI
                                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Cant navigate: " + ex.Message);
#endif
#if WINFORMS
                                await Task.Run(() => components.BeginInvoke(() => MainProcess.Output_Message(textBoxConsole, "Cant navigate: " + ex.Message, false, false)));
#endif
//#if ANDROID
//                               await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Cant navigate: " + ex.Message);
//#endif
                                return false;
                            }
                        }
                    }
                }
            }
            if (financeviewmodel.UDPRN == "")
            {
                //Update UI here
#if WPF || UWP || WINUI
                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "UDPRN: " + "<empty>");
#endif
#if WINFORMS
                await Task.Run(() => components.BeginInvoke(() => MainProcess.Output_Message(textBoxConsole, "UDPRN: " + "<empty>", false, false)));
#endif
//#if ANDROID
//               await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "UDPRN: " + "<empty>");
//#endif

                // We cannot continue here.
                // The UDPRN *cannot* be empty as EVERY
                // bank account has an Address ... every one
                // No exceptions (for them AND for us)
                financeviewmodel.errorMessage = "UDPRN is empty";
                return false;
            }
            //Update UI here
#if WPF || UWP || WINUI
            await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "UDPRN good: " + financeviewmodel.UDPRN);
#endif
#if WINFORMS
            await Task.Run(() => components.BeginInvoke(() => MainProcess.Output_Message(textBoxConsole, "UDPRN good: " + financeviewmodel.UDPRN, false, false)));
#endif
//#if ANDROID
//            await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "UDPRN good: " + financeviewmodel.UDPRN);
//#endif
            return true;
        }

        // Until you've found a UDPRN!
        // Address records should ALWAYS have a valid UDPRN!
        internal static async Task<string> FindDetails(
                                            MainViewModel ourviewmodel,
                                            FinanceViewModel financeviewmodel,
                                            WebDriver webDriver,
                                            SmartFinance.Logins loginInfo,
                                            string owner)
        {
            string udprn = "";
            // Step 1 - Find address embedded in Html page
            string bankaddress = "";
            try
            {
                IReadOnlyList<IWebElement> domAddresses = webDriver.FindElements(By.TagName("address"));
                foreach (IWebElement domAddress in domAddresses)
                {
                    if (domAddress.Text != null)
                    {
                        bankaddress = domAddress.Text;
                        bankaddress = bankaddress.Replace(SmartParametersV2016.newline,
                                                    SmartParametersV2016.spaceSplit).Trim();
                        // Here, we have the address from the website, lets clean it up a mo
                        // Reduce multiple spaces to a single space
                        bankaddress = Regex.Replace(bankaddress, @"\s{2,}", " ");
                        break;
                    }
                }

                // If there were no exceptions BUT the address
                // is still blank (empty) then that alone is
                // another FATAL error
                if (bankaddress == "")
                {
                    financeviewmodel.errorMessage = "Bank address empty - cannot continue";
                    return udprn;
                }

                // Step 2
                // Well, its not empty, look it up to see if we already have it
                List<SmartUsers.AddressesView> addresses_found =
                    SmartSpikeV2017.UsersLookupTextAddress(ourviewmodel,
                                                            bankaddress);
                if (addresses_found.Count > 0)
                {
                    // Something was returned
                    // Check to ensure it has a UDPRN
                    if (addresses_found.First().UDPRN == "")
                    {
                        // Step 3 - FATAL error
                        financeviewmodel.errorMessage = "Address record has no UDPRN - cannot continue";
                        return udprn;
                    }
                    // Step 4 - We can use this one's UDPRN
                    udprn = addresses_found.First().UDPRN;
                }
                else
                {
                    // Step 5 - No matching bankaddresses found
                    // We must add a new bankaddress record!
                    // Can we look it up from Ideal Postcodes?
                    SmartUsers.AddressesView ideal_address =
                            await SmartRoutinesV2018.UsersUDPRNLookup(ourviewmodel,
                                                                        bankaddress);
                    if (ideal_address.UDPRN == "")
                    {
                        // Step 6 - FATAL error 
                        // We MUST have a UDPRN to continue
                        financeviewmodel.errorMessage = "Cannot determine UDPRN - cannot continue";
                        return udprn;
                    }
                    // Step 7 - We have a udprn
                    udprn = ideal_address.UDPRN;
                    // Need to create a new Address record with this udprn
                    // Its not there
                    // Find the area based on the POSTCODE_OUTWARD setting
                    List<SmartUsers.Working_Postcodes> postcodes =
                        SmartSpikeV2017.Find_Working_Postcodes(ourviewmodel.Blanche.workingPostcodesList,
                                                               ideal_address.POSTCODE_OUTWARD);
                    short area_code = 0;
                    if (postcodes.Count > 0)
                    {
                        area_code = postcodes.First().AREA_CODE;
                    }
                    //SmartFinance.Addresses new_address = new SmartFinance.Addresses()
                    //{
                    //    USERNAME = ourviewmodel.UserName,
                    //   CUBEFACE_CODE = SmartParametersV2016.Finance,
                    //    UDPRN = ideal_address.UDPRN,
                    //    RANDOM_KEY = SmartRoutinesV2018.GetNextRandomNew(ourviewmodel.randomR),
                    //    ADDRESS_CREATED = DateTime.UtcNow,  // UTC time
                    //    Updated = false     // Its an I(nsert)
                    //};
                    //financeviewmodel.PLO.finance_addresses_changesList.Add(new_address);
                    //ourviewmodel.Hamas.addressesview_changesList.Add(new_address);



                    SmartUsers.AddressesView new_addressview = new SmartUsers.AddressesView()
                    {
                        USERNAME = ourviewmodel.UserName,
                        UDPRN = ideal_address.UDPRN,
                        RANDOM_KEY1 = SmartRoutinesV2018.GetNextRandomNew(ourviewmodel.randomR),
                        ADDRESS_CREATED = DateTime.Now,
                        //AREA_CODE = area_code,
                        BASIC = bankaddress,    // I'm not interested in what Ideal thinks this is
                                                // Add in all the bits and pieces from Ideal
                        BUILDING_NAME = ideal_address.BUILDING_NAME,
                        BUILDING_NUMBER = ideal_address.BUILDING_NUMBER,
                        COUNTY = ideal_address.COUNTY,
                        DOUBLE_DEPENDANT_LOCALITY = ideal_address.DOUBLE_DEPENDANT_LOCALITY,
                        DEPENDANT_THOROUGHFARE = ideal_address.DEPENDANT_THOROUGHFARE,
                        DEPENDANT_LOCALITY = ideal_address.DEPENDANT_LOCALITY,
                        ORGANIZATION = ideal_address.ORGANIZATION,
                        POSTCODE_OUTWARD = ideal_address.POSTCODE_OUTWARD,
                        POSTCODE = ideal_address.POSTCODE,
                        POBOX = ideal_address.POBOX,
                        SUB_BUILDING_NAME = ideal_address.SUB_BUILDING_NAME,
                        THOROUGHFARE = ideal_address.THOROUGHFARE,
                        TOWN = ideal_address.TOWN,
                        RANDOM_KEY2 = SmartRoutinesV2018.GetNextRandomNew(ourviewmodel.randomR),
                        Updated = false     // Its an I(nsert)
                    };
                    ourviewmodel.Hamas.addressesview_changesList.Add(new_addressview);
                }

                // These are not essential but 'nice to haves'
                // Email and a mobile Phone number
                // To do: Work and Home phone nos.
                // But ... where do they go????
                IWebElement email = webDriver.FindElement(By.Id("CurrentEmailAddress"));
                if (email != null)
                {
                    string emailcode = email.GetAttribute("value");
                    if (loginInfo.CONTACT_EMAIL != emailcode)
                    {
                        loginInfo.CONTACT_EMAIL = emailcode;
                        loginInfo.Updated = true;
                    }
                }

                // Can do HomePhoneNumber and/or WorkPhoneNumber here as well
                IWebElement mobilephoneno = webDriver.FindElement(By.Id("MobilePhoneNumber"));
                if (mobilephoneno != null)
                {
                    string phonedigits = mobilephoneno.GetAttribute("value");
                    if (loginInfo.CONTACT_PHONENO != phonedigits)
                    {
                        loginInfo.CONTACT_PHONENO = phonedigits;
                        loginInfo.Updated = true;
                    }
                }
            }
            catch (Exception ex)
            {
                // Catch any timeouts or failures
                // Errors here are FATAL as we always
                // need to get a text address
                financeviewmodel.errorMessage = ex.Message;
                return udprn;
            }
            // Update the OWNER if needs be
            if (owner != loginInfo.OWNER)
            {
                loginInfo.OWNER = owner;
                loginInfo.Updated = true;
            }
            if (loginInfo.Updated)
            {
                financeviewmodel.PLO.finance_logins_changesList.Add(loginInfo);
            }
            // Step 8 - the UDPRN here is valid
            return udprn;
        }


        internal static async Task<bool> NationwideFindPasscodes(
#if WINFORMS
                                                                MainProcess components,
                                                                RichTextBox textBoxConsole,
#endif
                                                                MainViewModel ourviewmodel,
                                                                FinanceViewModel financeviewmodel,
                                                                WebDriver webDriver,
                                                                string PASSCODE)
        {
            bool status = false;

            try
            {
                IReadOnlyList<IWebElement> passcodes = webDriver.FindElements(By.CssSelector("span[class='control__label__title']"));
                if (passcodes == null)
                {
#if WPF || UWP || WINUI
                    await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Couldn't find 'passcodes' span class");
#endif
#if WINFORMS
                    await Task.Run(() => components.BeginInvoke(() => MainProcess.Output_Message(textBoxConsole, "Couldn't find 'passcodes' span class", false, false)));
#endif
//#if ANDROID
//                  await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Couldn't find 'passcodes' span class");
//#endif
                    return false;
                }

                foreach (IWebElement each_passcode in passcodes)
                {
                    string text = each_passcode.Text;
                    if (text.Contains("digits from your passnumber"))
                    {
                        int index = 0;

                        // Look for which pass numbers they want
                        int[] zzz = new int[3];   // So we have 0, 1 and 2                                
                        if (text.Contains("1st"))
                        {
                            zzz[index] = 0;
                            index++;
                        }
                        ;
                        if (text.Contains("2nd"))
                        {
                            zzz[index] = 1;
                            index++;
                        }
                        ;
                        if (text.Contains("3rd"))
                        {
                            zzz[index] = 2;
                            index++;
                        }
                        ;
                        if (text.Contains("4th"))
                        {
                            zzz[index] = 3;
                            index++;
                        }
                        ;
                        if (text.Contains("5th"))
                        {
                            zzz[index] = 4;
                            index++;
                        }
                        ;
                        if (text.Contains("6th"))
                        {
                            zzz[index] = 5;
                            index++;
                        }
                        ;

                        IWebElement firstpassnumber = webDriver.FindElement(By.CssSelector("select[name='FirstPassnumberValue']"));//, PASSCODE.Substring(zzz[0], 1)));
                        if (firstpassnumber == null)
                        {
#if WPF || UWP || WINUI
                            await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Couldn't find 'First pass number' drop-down input");
#endif
#if WINFORMS
                            await Task.Run(() => components.BeginInvoke(() => MainProcess.Output_Message(textBoxConsole, "Couldn't find 'First pass number' drop-down input", false, false)));
#endif
//#if ANDROID
//                          await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Couldn't find 'First pass number' drop-down input");
//#endif
                            return false;
                        }
                        else
                        {
                            firstpassnumber.SendKeys(PASSCODE.Substring(zzz[0], 1));
                        }
                        IWebElement secondpassnumber = webDriver.FindElement(By.CssSelector("select[name='SecondPassnumberValue']"));//, PASSCODE.Substring(zzz[1], 1));
                        if (secondpassnumber == null)
                        {
#if WPF || UWP || WINUI
                            await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Couldn't find 'Second pass number' drop-down input");
#endif
#if WINFORMS
                            await Task.Run(() => components.BeginInvoke(() => MainProcess.Output_Message(textBoxConsole, "Couldn't find 'Second pass number' drop-down input", false, false)));
#endif
//#if ANDROID
//                          await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Couldn't find 'Second pass number' drop-down input");
//#endif
                            return false;
                        }
                        else
                        {
                            secondpassnumber.SendKeys(PASSCODE.Substring(zzz[1], 1));
                        }
                        IWebElement thirdpassnumber = webDriver.FindElement(By.CssSelector("select[name='ThirdPassnumberValue']"));//, PASSCODE.Substring(zzz[2], 1));
                        if (thirdpassnumber == null)
                        {
#if WPF || UWP || WINUI
                            await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Couldn't find 'Third pass number' drop-down input");
#endif
#if WINFORMS
                            await Task.Run(() => components.BeginInvoke(() => MainProcess.Output_Message(textBoxConsole, "Couldn't find 'Third pass number' drop-down input", false, false)));
#endif
//#if ANDROID
//                          await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Couldn't find 'Third pass number' drop-down input");
//#endif
                            return false;
                        }
                        else
                        {
                            thirdpassnumber.SendKeys(PASSCODE.Substring(zzz[2], 1));
                        }
                        status = true;
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                financeviewmodel.errorMessage = ex.Message;
            }
            return status;
        }
        internal static bool NationwideFindInputs(WebDriver webDriver,
                                                    string CustomerNumber,
                                                    string Day,
                                                    string Year,
                                                    FinanceViewModel financeviewmodel)
        {
            bool status = false;

            bool customernumber_found = false,
                dateofbirthday_found = false,
                dateofbirthyear_found = false;

            try
            {
                IReadOnlyCollection<IWebElement> allInputs = webDriver.FindElements(By.TagName("input"));
                if (allInputs != null)
                {
                    foreach (IWebElement each_input in allInputs)
                    {
                        string name = each_input.GetAttribute("name");
                        string aha = each_input.GetAttribute("type");
                        if (aha == "text")
                        {
                            switch (name)
                            {
                                case "CustomerNumber":
                                    if (!customernumber_found)
                                    {
                                        customernumber_found = true;
                                        each_input.SendKeys(CustomerNumber);
                                    }
                                    break;
                                case "DateOfBirthDay":
                                    if (!dateofbirthday_found)
                                    {
                                        dateofbirthday_found = true;
                                        each_input.SendKeys(Day);
                                    }
                                    break;
                                case "DateOfBirthYear":
                                    if (!dateofbirthyear_found)
                                    {
                                        dateofbirthyear_found = true;
                                        each_input.SendKeys(Year);
                                    }
                                    break;
                                default:
                                    break;
                            }
                            if (customernumber_found &&
                                dateofbirthday_found &&
                                dateofbirthyear_found)
                            {
                                status = true;
                                break;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                financeviewmodel.errorMessage = ex.Message;
            }
            return status;
        }
    }
#pragma warning restore CA1416
#endif
}