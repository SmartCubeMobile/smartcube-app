using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
//using System.Net.Http;
using System.Threading.Tasks;

using System.Reflection;
using System.Collections;
using System.Net;

using PuppeteerSharp;
using SmartCubeMobile;
using System.Collections.ObjectModel;
using System.Xml;
using PuppeteerSharp.Input;

#if WINFORMS
using System.Windows.Forms;
using Windows.UI.Notifications;
using Windows.ApplicationModel;
using Windows.UI.Notifications.Management;
using java.time;

#endif
#if WPF
using System.Windows.Controls;
using NHtmlUnit.Html;
using Windows.UI.Notifications;
using Windows.ApplicationModel;
using Windows.UI.Notifications.Management;
using java.time;

#endif
#if UWP
using NHtmlUnit.Html;
using Windows.UI.Notifications;
using Windows.ApplicationModel;
using Windows.UI.Notifications.Management;
using java.time;

#endif
#if XAMARIN
//using Windows.UI.Notifications;
//using Windows.ApplicationModel;
//using Windows.UI.Notifications.Management;
using NHtmlUnit.Html;

#endif

#if MAUI
using Microsoft.Playwright;
#endif

namespace SmartCubeMobile
{
    // This entire Microshit system is such utter bollocks you couldn't make it up ...
    // I got SO FAR!!  Down to the post to login in, but NW is returning a 302 after I post all the data
    // and - if you check on the actual website, then it really does think I have logged in!
    // But the 302 with the Customerized webpage shit has a bug?  or the NW website has a bug? 
    // and it keeps returning 10 re-directs to /Login/Login instead of AccountsList
    // So I am trying to control the 302 re-directs down to 1 (!!) but can I fucking do it?  Can I as fuck;
    // This UWP shit picks up a hard-coded (yes!!!- unbelievable I know) value of 10 from the Wininet (whatever the fuck
    // that is) and then - wait for it - won't let me specify a value of ... anything!  I can't set it to 0, 1, or 2
    // it HAS TO BE fucking 10!!!  So I can't then wait for 1 re-direct and then try and go and get the data
    // So its eithe 10 re-directs or none (if I turn auto-redirect off) WHAT AN ABSOLUTE PILE OF FUCKING BOLLOCKS
    // You need to  look at this https://github.com/dotnet/corefx/issues/17986

    public class NationwideV2023
    {
        internal static async Task<bool> Nationwide_Find_Transactions(
#if WINFORMS
                                                    RichTextBox textBoxConsole,
#endif
                                                    SignInViewModel signinviewmodel,
                                                    MainViewModel ourviewmodel,
                                                    FinanceViewModel financeviewmodel,
                                                    SmartFinance.Logins login_info,
                                                    ObservableCollection<SmartFinance.BankTransactions> Bollocks,
                                                    //string anchor_text,
                                                    string start_url,
                                                    string notification_title,
                                                    string notification_prefix,
                                                    short notification_tag_length,
                                                    string CustomerNumber,
                                                    string DOB,
                                                    string Passcode, //string[] PassNumbers)
                                                    short institution_code,
                                                    short brand_code,
                                                    Action<string> set_error_message)
        {
            // Check these important parameters
            if (Passcode.Length < 6)
            {
                set_error_message("Invalid Passcode " + Passcode);
                return false;
            }
            if (DOB.Length < 8)
            {
                set_error_message("Invalid DOB " + DOB);
                return false;
            }
            // Split the DOB into int's components
            string Day = DOB.Substring(0, 2);
            string Month = SmartParametersV2016.months[Convert.ToInt16(DOB.Substring(2, 2)) - 1];
            string Year = DOB.Substring(4, 4);


            //Create a new instance in code
            //CefSharp.Wpf.ChromiumWebBrowser cefbrowser = new CefSharp.Wpf.ChromiumWebBrowser("www.google.com");

            //Load a different url
            //cefbrowser.LoadUrl("https://github.com");


            IBrowser browser = null;
            try
            {
#if UWP
                string downloadPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

                BrowserFetcherOptions browserFetcherOptions = new BrowserFetcherOptions { Path = downloadPath };

#endif
                BrowserFetcher browserFetcher = new BrowserFetcher(new BrowserFetcherOptions()
                {                    
                    Path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Puppeteer")
                });
#if WINFORMS
                textBoxConsole.AppendText("BrowserFetcher creation completed" + Environment.NewLine.ToString());
                textBoxConsole.ScrollToCaret();
#endif


                await browserFetcher.DownloadAsync(BrowserFetcher.DefaultChromiumRevision );
#if WINFORMS
                textBoxConsole.AppendText("BrowserFletcher download completed" + Environment.NewLine.ToString());
                textBoxConsole.ScrollToCaret();
#endif

                //string exepath = @"Puppeteer\Win64-970485\chrome-win\chrome.exe";
                //exepath = System.IO.Path.Combine(downloadPath, exepath);
                
                browser = await Puppeteer.LaunchAsync(new LaunchOptions 
                { Headless = true, 
                  ExecutablePath = browserFetcher.RevisionInfo(BrowserFetcher.DefaultChromiumRevision).ExecutablePath,
                  Args = new string[] { "--no-sandbox" }
                });
#if WINFORMS
                textBoxConsole.AppendText("Puppeteer LaunchAync completed" + Environment.NewLine.ToString());
                textBoxConsole.ScrollToCaret();
#endif                
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }

            //Navigate to Nationwide
            IPage page = await browser.NewPageAsync();
            IResponse document = await page.GoToAsync("https://onlinebanking.nationwide.co.uk"); // In case of fonts being loaded from a CDN, use WaitUntilNavigation.Networkidle0 as a second param.
            if (document == null)
            {
                set_error_message("Couldn't find url " + start_url);
                return false;
            }
#if WINFORMS
            textBoxConsole.AppendText("Retrieved page: " + start_url + Environment.NewLine.ToString());
            textBoxConsole.ScrollToCaret();
#endif

            // Could never get the fucking button to work ... ever
            //await page.ClickAsync("button[text='Log in']");
            // Lets see if I can find the fucking Log in button ...
            //            string jsSelectAllButtons = @"Array.from(document.querySelectorAll('button'.map(button => button);";
            //            string[] buttons = await page.EvaluateExpressionAsync<string[]>(jsSelectAllButtons);
            //            foreach (string button in buttons)
            //            {
            //                //if (button == @"https://onlinebanking.nationwide.co.uk/Registration/GetCustomerNumber/Begin")
            //                //{
            //#if WINFORMS
            //                    textBoxConsole.AppendText("Button: " + button + Environment.NewLine.ToString());
            //                    textBoxConsole.ScrollToCaret();
            //#endif
            //                    break;
            //                //}
            //            }
            //            if (buttons.Count() > 0)
            //            {
            //                return true;
            //            }


            //            string jsSelectAllAnchors = @"Array.from(document.querySelectorAll('a')).map(a => a.href);";
            //            string[] urls = await page.EvaluateExpressionAsync<string[]>(jsSelectAllAnchors);
            //            string onlineUrl = String.Empty;
            //            foreach (string url in urls)
            //            {
            //                if (url.IndexOf("GetCustomerNumber") > 0)
            //                {
            //#if WINFORMS
            //                    textBoxConsole.AppendText("Url: " + url + Environment.NewLine.ToString());
            //                    textBoxConsole.ScrollToCaret();
            //#endif
            //                    break;
            //                }
            //            }
            //if (urls.Count() > 0)
            //{
            //    return true;
            //}
            //Console.WriteLine("Press any key to continue...");
            //Console.ReadLine();
            //}


            string onlineUrl = @"https://onlinebanking.nationwide.co.uk/AccessManagement/Login/Login";

            document = await page.GoToAsync(onlineUrl);
            if (document == null)
            {
                set_error_message("Couldn't find 'online' link");
                return false;
            }

            if (!await NationwideFindInputs(
#if WINFORMS
                                            textBoxConsole,
#endif
                                            CustomerNumber, Day, Year, page))
            {
                set_error_message("Couldn't find main inputs");
                return false;
            }
#if WINFORMS
            textBoxConsole.AppendText("Found all three" +
                                    Environment.NewLine.ToString());
            textBoxConsole.ScrollToCaret();
#endif
            // Now do the month dropdown
            var dateofbirth_month = await page.SelectAsync("select[name='DateOfBirthMonth']", Month);
            if (dateofbirth_month == null)
            {
                set_error_message("Couldn't find 'month' drop-down input");
                return false;
            }
            else
            {
#if WINFORMS
                textBoxConsole.AppendText("Birth month: " + Month +
                                           Environment.NewLine.ToString());
                textBoxConsole.ScrollToCaret();
#endif
            }


            //            bool raygo = false;
            //            string jsSelectAllButtons = @"Array.from(document.querySelectorAll('button')).map(button => button.class);";
            //            string[] buttonz = await page.EvaluateExpressionAsync<string[]>(jsSelectAllButtons);
            //            foreach (string button in buttonz)
            //            {
            //#if WINFORMS
            //                textBoxConsole.AppendText("Button0: " + button +
            //                                           Environment.NewLine.ToString());
            //                textBoxConsole.ScrollToCaret();
            //#endif
            //            }

            await page.Keyboard.PressAsync("Enter");// ClickAsync("button[class='action__button']");
            await page.WaitForNavigationAsync();

#if WINFORMS
            textBoxConsole.AppendText("Passnumbers: " +
                                            Passcode.Substring(0, 1) +
                                            " " +
                                            Passcode.Substring(1, 1) +
                                            " " +
                                            Passcode.Substring(2, 1) +
                                            " " +
                                            Passcode.Substring(3, 1) +
                                            " " +
                                            Passcode.Substring(4, 1) +
                                            " " +
                                            Passcode.Substring(5, 1) +
                                            Environment.NewLine.ToString());
            textBoxConsole.ScrollToCaret();
#endif
            //
            // Passcodes - NOTHING is fucking easy with this shit - ABSOLUTELY NOTHING!
            //
            int passnumbersCount = 0;
            IElementHandle[] all_select = await page.QuerySelectorAllAsync("select");
            if (all_select != null)
            {
                foreach (IElementHandle each_select in all_select)
                {
                    IJSHandle fooname = await each_select.GetPropertyAsync("name");
                    string name = fooname.ToString().Replace("JSHandle:", string.Empty);
                    switch (name)
                    {
                        case "FirstPassnumberValue":
                            await each_select.SelectAsync(Passcode.Substring(0, 1));
                            passnumbersCount++;
                            break;
                        case "SecondPassnumberValue":
                            await each_select.SelectAsync(Passcode.Substring(1, 1));
                            passnumbersCount++;
                            break;
                        case "ThirdPassnumberValue":
                            await each_select.SelectAsync(Passcode.Substring(2, 1));
                            passnumbersCount++;
                            break;
                        case "FourthPassnumberValue":
                            await each_select.SelectAsync(Passcode.Substring(3, 1));
                            passnumbersCount++;
                            break;
                        case "FifthPassnumberValue":
                            await each_select.SelectAsync(Passcode.Substring(4, 1));
                            passnumbersCount++;
                            break;
                        case "SixthPassnumberValue":
                            await each_select.SelectAsync(Passcode.Substring(5, 1));
                            passnumbersCount++;
                            break;
                    }
                }
            }


#if WINFORMS
            textBoxConsole.AppendText("Passnumbers count: " + passnumbersCount.ToString() +
                                            Environment.NewLine.ToString());
            textBoxConsole.ScrollToCaret();
#endif
            // Subscribe to Notification event
            try
            {

                //IReadOnlyList<IElementHandle> rolist = null;
                IElementHandle[] passcode_buttons = await page.MainFrame.QuerySelectorAllAsync("button");

                //ourviewmodel.notification_listener.NotificationChanged += async (sender, e) => await Listener_NotificationChangedX(sender,
                //                                                                                                    e);
                //
                //
                string smsotp = string.Empty;
#if WINFORMS || WPF || UWP
                ourviewmodel.notification_listener.NotificationChanged += async (sender, e) => await Listener_NotificationChanged(sender,
                                                                                                                    e,
#if WINFORMS
                                                                                                                    textBoxConsole,
#endif
                                                                                                                    signinviewmodel,
                                                                                                                    ourviewmodel,
                                                                                                                    financeviewmodel,
                                                                                                                    page,
                                                                                                                    passcode_buttons,
                                                                                                                    Bollocks,
                                                                                                                    notification_title,
                                                                                                                    notification_prefix,
                                                                                                                    notification_tag_length,
                                                                                                                    login_info,             // To collect the address
                                                                                                                    institution_code,
                                                                                                                    brand_code,
                                                                                                                    set_error_message,
                                                                                                                    em => smsotp = em);

#endif
#if WINFORMS
                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, " Notifications: configured");
                await SmartRoutinesV2018.Check_Trace(ourviewmodel, financeviewmodel.finance_token, institution_code, brand_code, "Notifications : configured");
#endif

#if WINFORMS
                ourviewmodel.timerClock.Interval = SmartParametersV2016.clockInt; // 1 Second 
                                                                                  //ourviewmodel.timerClock.Tick += new EventHandler(TimerClock_Tick);   // Won't happen until we either Open/Close

                ourviewmodel.timerClock.Tick += new EventHandler((s, e) => NotificationClock_Tick(s, e, textBoxConsole));


                ourviewmodel.timerClock.Disposed += NotificationClock_Disposed;    // Has to be static for some obscure reason
#endif

                bool continueFound = false;
                // One-time Pass Code
                //IElementHandle[] passcode_buttons = await page.MainFrame.QuerySelectorAllAsync("button");
                if (passcode_buttons != null)
                {
                    foreach (IElementHandle button in passcode_buttons)
                    {
                        IJSHandle footitle = await button.GetPropertyAsync("textContent");
                        string title = footitle.ToString().Replace("JSHandle:", string.Empty).Trim();

                        if (title == "Continue")
                        {
                            continueFound = true;
                            //#if WINFORMS
                            //                        textBoxConsole.AppendText("Button: " + title +
                            //                                                  Environment.NewLine.ToString());
                            //                        textBoxConsole.ScrollToCaret();
                            //#endif
                            try
                            {
#if WINFORMS
                                ourviewmodel.timerClock.Enabled = true;
                                ourviewmodel.timerClock.Start();
                                textBoxConsole.AppendText("Clock: " + " started" +
                                                          Environment.NewLine.ToString());
                                textBoxConsole.ScrollToCaret();
#endif
                                //continueFound = true;
                                await button.ClickAsync();
                                //await page.WaitForNavigationAsync();
                                //break;
#if WINFORMS
                                textBoxConsole.AppendText("Button: " + title + " clicked" +
                                                          Environment.NewLine.ToString());
                                textBoxConsole.ScrollToCaret();
#endif
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex.Message);
                                break;
                            }
                        }
                    }
                }


                if (!continueFound)
                {

#if WINFORMS
                    textBoxConsole.AppendText("Continue button: " + continueFound.ToString() +
                                               Environment.NewLine.ToString());
                    textBoxConsole.ScrollToCaret();
#endif
                    return false;
                }
                else
                {
                    // Press the fucking equivalent of 'Submit'!!!! (You couldn't make this shit up ...)

                    //await page.WaitForNavigationAsync();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }


            
            // Read the TOKEN HERE!!!

            //TextReader txt = new StreamReader(@"C:\Users\Ray\AppData\Local\SMSOTP.txt");
            //string onetimepasscode = txt.ReadToEnd();
            //txt.Close();

            //if (!string.IsNullOrEmpty(onetimepasscode))
            //{
            //    var smsotp = await page.QuerySelectorAsync("input[name='OneTimePasscode']");
            //    if (smsotp == null)
            //    {
            //        set_error_message("Couldn't find the SMSOTP input");
            //        return false;
            //    }

            //    await smsotp.TypeAsync(onetimepasscode);

            //    foreach (IElementHandle each_button in buttons)
            //    {
            //        string text = each_button.ToString();
            //        if (text.Trim() == "Log in")
            //        {
            //            // Press the fucking equivalent of 'Submit'!!!! (You couldn't make this shit up ...)
            //            await each_button.ClickAsync();
            //            // Wait for the fucker to get its act together
            //            await page.WaitForNavigationAsync();
            //            break;
            //        }
            //    }


#if WINFORMS
            textBoxConsole.AppendText("You should have received an SMSOTP!" + 
                                            Environment.NewLine.ToString());
            textBoxConsole.ScrollToCaret();
#endif

            // Subscribe to Notification event
//            try
//            {
//                IReadOnlyList<IElementHandle> rolist = null;
//                string smsotp = string.Empty;
//                ourviewmodel.notification_listener.NotificationChanged += async (sender, e) => await Listener_NotificationChanged(sender,
//                                                                                                                    e,
//#if WINFORMS
//                                                                                                                    textBoxConsole,
//#endif
//                                                                                                                    signinviewmodel,
//                                                                                                                    ourviewmodel,
//                                                                                                                    financeviewmodel,
//                                                                                                                    page,
//                                                                                                                    rolist,
//                                                                                                                    Bollocks,
//                                                                                                                    notification_title,
//                                                                                                                    notification_prefix,
//                                                                                                                    notification_tag_length,
//                                                                                                                    login_info,             // To collect the address
//                                                                                                                    institution_code,
//                                                                                                                    brand_code,
//                                                                                                                    set_error_message,
//                                                                                                                    em => smsotp = em);

//#if WINFORMS
//                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, " Notifications: configured");
//                await SmartRoutinesV2018.Check_Trace(ourviewmodel, financeviewmodel.finance_token, institution_code, brand_code, "Notifications : configured");
//#endif
//                // And press the Button to say 'Send me a OTP' please
//                // This next line won't work becuase the 'Continue' has line breaks in it
//                //IElementHandle cont_button = await financeviewmodel.nwpage.QuerySelectorAsync("button[text=Continue]");
//                //if (cont_button != null)
//                //{
//                //    Console.WriteLine("here");
//                //}                        
//                foreach (IElementHandle button in rolist)
//                {
//                    IJSHandle footext = await button.GetPropertyAsync("textContent");
//                    string text = footext.ToString().Replace("JSHandle:", string.Empty).Trim();

//                    if (text == "Continue")
//                    {
//                        try
//                        {
//                            await button.ClickAsync();
//                            await page.WaitForNavigationAsync();
//                            //async () =>
//                            //{
//                                // Triggers a navigation after a timeout
//                                //await financeviewmodel.nwpage.WaitForLoadStateAsync(LoadState.DOMContentLoaded);

//                            //});
//                        }
//                        catch (Exception ex)
//                        {

//                            await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Scrape aborted - no OTP recevied");
//                            await SmartRoutinesV2018.Check_Trace(ourviewmodel, financeviewmodel.finance_token, institution_code, brand_code, ex.Message);
//                            set_error_message(ex.Message);
//                            return false;
//                        }
//                        break;
//                    }
//                }
//            }
//            catch (Exception ex)
//            {
//                set_error_message(ex.Message);
//                return false;
//            }

#if WINFORMS
            textBoxConsole.AppendText("Finished SCRAPE section" +
                                            Environment.NewLine.ToString());
            textBoxConsole.ScrollToCaret();
#endif
            await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Finished: SCRAPE section");
            await SmartRoutinesV2018.Check_Trace(ourviewmodel, financeviewmodel.finance_token, institution_code, brand_code, "Finished: SCRAPE section");
            return true;
        }
#if WINFORMS
        internal static void NotificationClock_Tick(object sender, EventArgs e, RichTextBox textBoxConsole)
        {

            textBoxConsole.AppendText(DateTime.Now.ToString(SmartParametersV2016.militaryFormat) +
                                      Environment.NewLine.ToString());
            textBoxConsole.ScrollToCaret();

            return;      
        }
#endif

        internal static void NotificationClock_Disposed(object sender, object e)
        {
            return;
        }
            internal static async Task<bool> NationwideFindInputs(
#if WINFORMS
                                                            RichTextBox textBoxConsole,
#endif
                                                            string CustomerNumber,
                                                            string Day,
                                                            string Year,
                                                            IPage page)
        {
#if WINFORMS
            textBoxConsole.AppendText("Finding inputs: " + Environment.NewLine.ToString());
            textBoxConsole.ScrollToCaret();
#endif

            bool status = false;

            bool customernumber_found = false,
                dateofbirthday_found = false,
                dateofbirthyear_found = false;

//            string jsSelectAllInputs = @"Array.from(document.querySelectorAll('input')).map(input => input.name);";
//            string[] inputs = await page.EvaluateExpressionAsync<string[]>(jsSelectAllInputs);
//            foreach (string input in inputs)
//            {
                
//#if WINFORMS
//                textBoxConsole.AppendText("Input: " + input + Environment.NewLine.ToString());
//                textBoxConsole.ScrollToCaret();
//#endif

//            }
            //if (inputs.Count() > 0)
            //{
            //    return true;
            //}


            IElementHandle[] all_input = await page.QuerySelectorAllAsync("input");
            if (all_input != null)
            {
                foreach (IElementHandle each_input in all_input)
                {
                    IJSHandle fooname = await each_input.GetPropertyAsync("name");
                    string name = fooname.ToString().Replace("JSHandle:", string.Empty);

                    switch (name)
                    {
                        case "CustomerNumber":
                            if (!customernumber_found)
                            {
                                customernumber_found = true;
                                await each_input.TypeAsync(CustomerNumber);
#if WINFORMS
                                textBoxConsole.AppendText("Customer: " + CustomerNumber +
                                                           Environment.NewLine.ToString());
                                textBoxConsole.ScrollToCaret();
#endif
                            }
                            break;
                        case "DateOfBirthDay":
                            if (!dateofbirthday_found)
                            {
                                dateofbirthday_found = true;
                                await each_input.TypeAsync(Day);
#if WINFORMS
                                textBoxConsole.AppendText("Birth day: " + Day +
                                                           Environment.NewLine.ToString());
                                textBoxConsole.ScrollToCaret();
#endif
                            }
                            break;
                        case "DateOfBirthYear":
                            if (!dateofbirthyear_found)
                            {
                                dateofbirthyear_found = true;
                                await each_input.TypeAsync(Year);
#if WINFORMS
                                textBoxConsole.AppendText("Birth year: " + Year +
                                                           Environment.NewLine.ToString());
                                textBoxConsole.ScrollToCaret();
#endif
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
            return status;
        }

        //internal static async Task<bool> Listener_NotificationChangedX(
        //
        //                                                UserNotificationListener sender,
        //                                                UserNotificationChangedEventArgs args)
        //{
        //    Console.WriteLine(args.ToString());
        //    return true;
        //}


#if WINFORMS || WPF || UWP
        internal static async Task<bool> Listener_NotificationChanged(

                                                        UserNotificationListener sender,
                                                        UserNotificationChangedEventArgs args,
#if WINFORMS
                                                        RichTextBox textBoxConsole,
#endif
                                                        SignInViewModel signinviewmodel,
                                                        MainViewModel ourviewmodel,
                                                        FinanceViewModel financeviewmodel,
                                                        IPage nwpage,
                                                        IElementHandle[] passcode_buttons,
                                                        ObservableCollection<SmartFinance.BankTransactions> Bollocks,
                                                        string notification_title,
                                                        string notification_prefix,
                                                        short notification_tag_length,
                                                        SmartFinance.Logins logins_info,    // To collect address
                                                        short institution_code,
                                                        short brand_code,
                                                        Action<string> set_error_message,
                                                        Action<string> set_smsotp)
        {

            await SmartRoutinesV2018.Check_Trace(ourviewmodel, financeviewmodel.finance_token, institution_code, brand_code, " Handler: event delivered");

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
                set_error_message("No User Notifications found");
                return false;
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
                string titleText = textElements.FirstOrDefault()?.Text;
                if (notification_title != titleText)
                {
                    continue;
                }
#if WINFORMS
                textBoxConsole.AppendText("Found: " + titleText +
                                           Environment.NewLine.ToString());
                textBoxConsole.ScrollToCaret();
#else
                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, " Found: " + titleText);
                await SmartRoutinesV2018.Check_Trace(ourviewmodel, financeviewmodel.finance_token, institution_code, brand_code, "Found: " + titleText);
#endif
                // We'll treat all subsequent text elements as body text,
                // joining them together via newlines.
                string bodyText = string.Join("\n", textElements.Skip(1).Select(t => t.Text));
                if (!bodyText.Contains(notification_prefix))
                {
                    continue;
                }

                // Ok, we've found a NATIONWIDE, so say we don't want to invoke
                // this event handler any more!
                ourviewmodel.notification_listener.NotificationChanged -= null;

                bodyText = bodyText.Replace(notification_prefix, string.Empty).Trim(); //Trim gets rid of any spaces
                if (bodyText.Length <= 6)
                {
                    continue;
                }
                string notificationContent = bodyText.Substring(0, notification_tag_length);

                MainMeter.financeviewmodel.onetimepasscode = notificationContent;

#if WINFORMS
                textBoxConsole.AppendText("Inserting: " + notificationContent +
                                           Environment.NewLine.ToString());
                textBoxConsole.ScrollToCaret();
#else
                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, " Inserting: " + notificationContent);
                await SmartRoutinesV2018.Check_Trace(ourviewmodel, financeviewmodel.finance_token, institution_code, brand_code, "Inserting: " + notificationContent);
#endif
                //string onetimepasscode = notificationContent;
                if (string.IsNullOrEmpty(MainMeter.financeviewmodel.onetimepasscode))
                {
                    // Hardly likely to happen
                    set_error_message("Notification content is empty");
                    return false;
                }
                set_smsotp(MainMeter.financeviewmodel.onetimepasscode);

                IElementHandle smsotp = await nwpage.QuerySelectorAsync("input[name='OneTimePasscode']");
                if (smsotp == null)
                {
                    // Hardly likely to happen, either
                    set_error_message("Couldn't find the SMSOTP input");
                    return false;
                }

                await smsotp.TypeAsync(MainMeter.financeviewmodel.onetimepasscode);

                passcode_buttons = await nwpage.QuerySelectorAllAsync("button");
                if (passcode_buttons.Count() == 0)
                {
                    set_error_message("Couldn't find any buttons");
                    return false;
                }

                foreach (IElementHandle each_button in passcode_buttons)
                {
                    IJSHandle foobutton = await each_button.GetPropertyAsync("textContent");

                    string footext = each_button.ToString().Replace("JSHandle", string.Empty).Trim();
                    if (footext == "Log in")
                    {
                        await each_button.ClickAsync();
                        await nwpage.WaitForNavigationAsync();//  WaitForLoadStateAsync(LoadState.DOMContentLoaded);

                        // Find the Address <a> befo re all this shit goes haywire ..
                        //ILocator managerows = financeviewmodel.nwpage.Locator("a[class^='nav-link-unique-']"); // ("a[id^='primary-nav-title-level-1-sibling-1-unique-']");
                        IElementHandle managerows = await nwpage.QuerySelectorAsync("a[id^='primary-nav-link-level-3-sibling-1-unique-']");

                        if (!await FuckingLoopRound(signinviewmodel,
                                            ourviewmodel,
                                            financeviewmodel,
                                            nwpage,
                                            Bollocks,
                                            institution_code,
                                            brand_code,
                                            set_error_message))
                        {
                            return false;
                        }
                        // Find address here?
                        // Its here
                        if (!await FindAddress(
#if WINFORMS
                                    textBoxConsole,
#endif
                                    signinviewmodel,
                                    ourviewmodel,
                                    financeviewmodel,
                                    nwpage,
                                    institution_code,
                                    brand_code))
                        {
                            return false;
                        }
                    }
                }
                // We only ever do ONE notification
                Console.WriteLine("Break4");
                break;
            }
            return true;
        }
#endif

        internal static async Task<bool> FindAddress(
#if WINFORMS
                                                    RichTextBox textBoxConsole,
#endif
                                                    SignInViewModel signinviewmodel,
                                                    MainViewModel ourviewmodel,
                                                    FinanceViewModel financeviewmodel,
                                                    IPage nwpage,
                                                    //ILocator managerows,
                                                    short institution_code,
                                                    short brand_code)
        {
            bool status = true;
            // Find address here?
            // Its here
            try
            {
                SmartFinance.Accounts account_info = new SmartFinance.Accounts();

                IReadOnlyList<IElementHandle> manage_list = await nwpage.QuerySelectorAllAsync("a[id^='primary-nav-link-level-3-sibling-1-unique-']");
                foreach (IElementHandle manage_link in manage_list)
                {
                    IJSHandle foohref1 = await manage_link.GetPropertyAsync("href");
                    string href1 = foohref1.ToString().Replace("JSHandle:", string.Empty).Trim();
                    if (href1.IndexOf("myaddress") > 0)
                    {
                        href1 = "https://onlinebanking.nationwide.co.uk" + href1;
                        await nwpage.GoToAsync(href1);
                        await nwpage.WaitForNavigationAsync();//  WaitForLoadStateAsync(LoadState.DOMContentLoaded);

                        IElementHandle address = await nwpage.QuerySelectorAsync("address");
                        if (address != null)
                        {
                            IJSHandle foomyaddress = await address.GetPropertyAsync("innerText");
                            string myaddress = foomyaddress.ToString().Replace("JSHandle:", string.Empty).Trim();
                            SmartUsers.Addresses ideal_address = new SmartUsers.Addresses();
                            if (!await SmartFinanceV2021.UDPRNLookup(  
#if WINFORMS
                                                    textBoxConsole,
#endif
                                                    ourviewmodel,
                                                    financeviewmodel,
                                                    xyz => ideal_address = xyz,
                                                    myaddress))
                            {
                                return false;
                            }

                            string udprn = ideal_address.UDPRN; // "12345678"; // "100011448863";
                            if (string.IsNullOrEmpty(account_info.UDPRN) ||
                                account_info.UDPRN != udprn)   // Change of address
                            {
                                account_info.UDPRN = udprn;
                                account_info.Updated = true; // Don't forget to update the Logins in DoAll_SmartFinance

                                ObservableCollection<SmartUsers.Addresses> addresses_list = SmartSpikeFinanceV2017.Finance_Find_Addresses(ourviewmodel,
                                                                                                                                          financeviewmodel,
                                                                                                                                          udprn);
                                if (addresses_list.Count == 0)
                                {
                                    // Its not there
                                    //SmartUsers.Addresses new_address = new SmartUsers.Addresses()
                                    //{                                                
                                    //    UDPRN = udprn,
                                    //    POSTCODE = "SK8 3JH",
                                    //    Updated = false     // Its an I(nsert)
                                    //};
                                    ideal_address.Updated = false; // Its an I(nsert)
                                    ourviewmodel.Hamas.addresses_changes_list.Add(ideal_address);
                                    if (await SmartRoutinesV2018.DoAll_SmartUsers(ourviewmodel,
                                                SmartParametersV2016.sqliteformat))
                                    {
                                        // Tell the console we have added an address
                                        if (!await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Added new address: " + ideal_address.UDPRN))
                                        {
                                            return false;
                                        }
                                    }
                                }
                            }
                        }
                        IElementHandle phone_id = await nwpage.QuerySelectorAsync("a[id^='current-phone-number']");
                        if (phone_id != null)
                        {
                            IElementHandle phone_dl = await phone_id.QuerySelectorAsync("dl");
                            if (phone_dl != null)
                            {
                                IReadOnlyList<IElementHandle> phone_dds_dts = await phone_dl.QuerySelectorAllAsync("d^");
                                foreach (IElementHandle phone_dd_dt in phone_dds_dts)
                                {


                                }
                            }
                        }
                        string myemail = string.Empty;
                        IReadOnlyList<IElementHandle> email_details = await nwpage.QuerySelectorAllAsync("div[class^='details-value']");
                        foreach (IElementHandle email_div in email_details)
                        {
                            IJSHandle foomyemail = await email_div.GetPropertyAsync("innerText");
                            myemail = foomyemail.ToString().Replace("JSHandle:", string.Empty).Trim();
                            if (myemail.IndexOf('@') > 0)
                            {
                                // we found it (Kludge City rules)
                                break;
                            }
                        }
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                // Catch any timeouts
                status = false;
                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "FindAddress: " + ex.Message);
                await SmartRoutinesV2018.Check_Trace(ourviewmodel, financeviewmodel.finance_token, institution_code, brand_code, "FindAddress : " + ex.Message);
            }
            return status;
        }

        internal static async Task<bool> FindBalanceAvailable(SignInViewModel signinviewmodel,
                                                                MainViewModel ourviewmodel,
                                                                FinanceViewModel financeviewmodel,
                                                                IPage nwpage,
                                                                short institution_code,
                                                                short brand_code,
                                                                Action<string> set_balance,
                                                                Action<string> set_available,
                                                                Action<string> set_currency)
        {
            bool status = false;
            string balance = String.Empty;
            string available = string.Empty;
            string currency = string.Empty;

            try
            {
                IReadOnlyList<IElementHandle> dls = await nwpage.QuerySelectorAllAsync("dl");
                foreach (IElementHandle dl_row in dls)
                {
                    IReadOnlyList<IElementHandle> dds = await nwpage.QuerySelectorAllAsync("dd");
                    int dd_count = 0;
                    foreach (IElementHandle dd_row in dds)
                    {
                        switch (dd_count)
                        {
                            case 0:
                                balance = dd_row.ToString(); // TextContentAsync();
                                balance = balance.Trim();
                                string currency_symbol = balance.Substring(0, 1);
                                currency = SmartSpikeFinanceV2017.Finance_Lookup_Account_Currency(signinviewmodel.Fatah.culture_view_list,
                                                                                        currency_symbol);
                                break;
                            case 1:
                                available = dd_row.ToString(); // TextContentAsync();
                                available = available.Trim();
                                break;
                            default: break;
                        }
                        dd_count++;
                    }
                }
                if (!string.IsNullOrEmpty(balance) &&
                    !string.IsNullOrEmpty(available) &&
                    !string.IsNullOrEmpty(currency))
                {
                    set_balance(balance);
                    set_available(available);
                    set_currency(currency);
                    status = true;
                }
            }
            catch (Exception ex)
            {
                // Catch any timeouts
                status = false;
                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "FindBalance: " + ex.Message);
                await SmartRoutinesV2018.Check_Trace(ourviewmodel, financeviewmodel.finance_token, institution_code, brand_code, "FindBalance: " + ex.Message);
            }
            return status;
        }

        internal static void ScrapedAccounts(ObservableCollection<FinanceViewModel.AccountItem> scraped_accounts,
                                            string sort_code,
                                            string account_no,
                                            string balance,
                                            string href,
                                            int account_id,
                                            string account_name)
        {
            string account_value = sort_code + SmartParametersV2016.unitSeparator +
                                                    account_no +
                                                    //":" +
                                                    //"GBP" +     // Assume we can find it in OpenBanking
                                                    SmartParametersV2016.ourSeparator +
                                                    balance +
                                                    SmartParametersV2016.ourSeparator +
                                                    href;    // Safety check
                                                             //string[] array = val.Split(':');
                                                             //account_id++;
            FinanceViewModel.AccountItem account_item = new FinanceViewModel.AccountItem()
            {
                Account_ID = account_id,
                Content = account_name,
                Value = account_value // Here we have    [0] = sort_code and account_no
                                      //                 [1] = balance
                                      //                 [2] = href
            };
            scraped_accounts.Add(account_item);
            return;
        }

        internal static async Task<bool> FuckingLoopRound(SignInViewModel signinviewmodel,
                                                            MainViewModel ourviewmodel,
                                                            FinanceViewModel financeviewmodel,
                                                            IPage nwpage,
                                                            ObservableCollection<SmartFinance.BankTransactions> Bollocks,
                                                            short institution_code,
                                                            short brand_code,
                                                            Action<string> set_error_message)
        {
            bool status = true;
            // AT THIS POINT WE SHOULD BE ABLE TO READ THE FUCKING ACCOUNT??
            // HAHAHAHHAHAHAHA!!  You have GOT to be FUCKING JOKING MATE!
            // YOU NEED TO SPEND FUCKING HOURS AND HOURS AND COUNTLESS TEST
            // in order to get this fucking bollocks to loop round the account
            //
            // Hundreds of 'Element is not attached to the DOM' applies
            // in Chimp-spades
            try
            {
                IElementHandle[] rows = await nwpage.QuerySelectorAllAsync("a[class^='acLink']");
                int no_of_rows = rows.Count();
                int account_id = -1;
                for (int row = 0; row < no_of_rows; row++)
                {
                    try
                    {
                        IElementHandle each_account = rows[row]; ////await rows.Nth(row).IElementHandleAsync();
                          
                        IJSHandle foonwid = await each_account.GetPropertyAsync("id"); //each_account.GetAttributeAsync("id");
                        string nwid = foonwid.ToString().Replace("JSHandle:", string.Empty).Trim();
                        await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, " Processing: " + nwid);
                        await SmartRoutinesV2018.Check_Trace(ourviewmodel, financeviewmodel.finance_token, institution_code, brand_code, "Processing : " + nwid);

                        IJSHandle foohref = await each_account.GetPropertyAsync("href");
                        string href = foohref.ToString().Replace("JSHandle:", string.Empty).Trim();

                        account_id++;

                        string sort_code = string.Empty;
                        string account_no = string.Empty;
                        string balance = string.Empty;
                        string available = string.Empty;

                        string account_name = string.Empty;

                        ObservableCollection<FinanceViewModel.AccountItem> scraped_accounts = new ObservableCollection<FinanceViewModel.AccountItem>();

                        if (!string.IsNullOrEmpty(nwid))
                        {
                            string[] details = nwid.Split(Convert.ToChar(SmartParametersV2016.space));
                            if (details.Count() > 1)
                            {
                                sort_code = details[0];
                                account_no = details[1];
                            }
                            else
                            {
                                account_no = details[0];
                            }
                        }

                        IReadOnlyList<IElementHandle> all_text = await each_account.QuerySelectorAllAsync("b");
                        
                        //await financeviewmodel.nwpage.WaitForLoadStateAsync(LoadState.DOMContentLoaded);
                        
                        if (all_text != null)
                        {
                            foreach (IElementHandle text_item in all_text)
                            {
                                IJSHandle fooaccount_name = await text_item.GetPropertyAsync("innerText");
                                account_name = fooaccount_name.ToString().Replace("JSHandle:", string.Empty).Trim();
                                break;
                            }

                            if (!string.IsNullOrEmpty(account_name) &&  // sort_code might indeed be deliberately empty here ...
                                !string.IsNullOrEmpty(account_no) &&
                                !string.IsNullOrEmpty(href))
                            {
                                await each_account.ClickAsync();
                                await nwpage.WaitForNavigationAsync(); // async () =>
                                //{
                                //    await each_account.ClickAsync();
                                //});
                                string currency = string.Empty;

                                if (!await FindBalanceAvailable(signinviewmodel,
                                                            ourviewmodel,
                                                            financeviewmodel,
                                                            nwpage,
                                                            institution_code,
                                                            brand_code,
                                                            bal => balance = bal,
                                                            ava => available = ava,
                                                            cur => currency = cur))
                                {
                                    return false;
                                }

                                ScrapedAccounts(scraped_accounts,
                                                sort_code,
                                                account_no,
                                                balance,
                                                href,
                                                account_id,
                                                account_name);


                                IElementHandle reveal_date = await nwpage.QuerySelectorAsync("a[id^='enter-date-reveal-link']");
                                // Sometimes .. .NW accounts simply don't have a 'reveal date' (sigh)
                                if (reveal_date == null)
                                {
                                    await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "LoopRound: " + "No reveal date");
                                    await SmartRoutinesV2018.Check_Trace(ourviewmodel, financeviewmodel.finance_token, institution_code, brand_code, "LoopRound: " + "No reveal date");

                                }
                                else
                                {
                                    await reveal_date.ClickAsync();
                                    await nwpage.WaitForNavigationAsync();

                                    // DATES!!  Wherever you are in the world, Nationwide
                                    // dates and times are always local to the UK, so
                                    // you always need to add the 'gmtoffset' to the 'time now'
                                    // in order to get 'UK time'

                                    string error_message = await DoTheDatesBusiness(ourviewmodel,
                                                            financeviewmodel,
                                                            nwpage,
                                                            Bollocks,
                                                            sort_code,
                                                            account_no,
                                                            currency,
                                                            institution_code,
                                                            brand_code);
                                    if (!string.IsNullOrEmpty(error_message))
                                    {
                                        set_error_message(error_message);
                                        return false;
                                    }
                                }
                            }
                        }
                        await nwpage.GoBackAsync();
                    }
                    catch (Exception ex)
                    {
                        await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "LoopRound nwid: " + ex.Message + account_id.ToString());
                        await SmartRoutinesV2018.Check_Trace(ourviewmodel, financeviewmodel.finance_token, institution_code, brand_code, "LoopRound nwid : " + ex.Message + account_id.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                // Probably a timeout
                status = false;
                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "LoopRound: " + ex.Message);
                await SmartRoutinesV2018.Check_Trace(ourviewmodel, financeviewmodel.finance_token, institution_code, brand_code, "LoopRound: " + ex.Message);
            }
            return status;
        }

        internal static async Task<string> DoTheDatesBusiness(MainViewModel ourviewmodel,
                                                            FinanceViewModel financeviewmodel,
                                                        IPage nwpage,
                                                        ObservableCollection<SmartFinance.BankTransactions> Bollocks,
                                                        string sort_code,
                                                        string account_no,
                                                        string currency,
                                                        short institution_code,
                                                        short brand_code)
        {
        //bool status = true;

            string error_message = string.Empty;

            // DATES!!  Wherever you are in the world, Nationwide
            // dates and times are always local to the UK, so
            // you always need to add the 'gmtoffset' to the 'time now'
            // in order to get 'UK time'
            try
            {
                DateTime time_now = SmartEncryptionV2016.DateTimeNow(SmartParametersV2016.gmtdefaultOffset);
                string end_datex = time_now.ToString(SmartParametersV2016.standardFormat);

                // The way these date buttons work ... is weird!  You can
                // fix the 'end' date button more or less easily ...
                // although we really want it to be 'today' in all cases.
                // However, te 'start' button is far more difficult to fix,
                // and the website ALWAYS wants to try and fix it to
                // 'today' minus 15 months, so if 'today' is 05/03/2022
                // (5th March 2022) then the website won't allow you to use
                // a date EARLIER then 05/12/2020 (5th December 2020)
                // Even if I enter '01/01/1900' then that date gets adjusted
                // (by some poxy Javafuckingscript?) to '05/12/2020'.
                // So I have two choices: either always enter a bonkers
                // '01/01/1900' date and gert it adjusted OR calculate
                // the 15 months difference between 'today' and a 'start'
                // date.  But what is NW adjust the start date at some point?
                // I would have to amend my calculation and change the program
                // OR I could define it as a parameter... (more work).
                // Decision: I'm going to stick it in as '01/01/1900' and
                // let the website do the work ....

                IElementHandle start_date = await nwpage.QuerySelectorAsync("input[name^='Start']");
                if (start_date == null)
                {
                    error_message = "DoTheDates: " + "No start date";

                    await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, error_message);
                    await SmartRoutinesV2018.Check_Trace(ourviewmodel, financeviewmodel.finance_token, institution_code, brand_code, error_message);

                    return error_message;
                }
                string from_date = "01/01/1900";    // This will be adjusted to the minimum by the website (I hope)
                await start_date.TypeAsync(from_date);

                IElementHandle end_date = await nwpage.QuerySelectorAsync("input[name^='End']");
                if (end_date == null)
                {
                    error_message = "DoTheDates: " + "No end date";
                    await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, error_message);
                    await SmartRoutinesV2018.Check_Trace(ourviewmodel, financeviewmodel.finance_token, institution_code, brand_code, error_message);
                    return error_message;
                }
                string to_date = end_datex;
                await end_date.TypeAsync(to_date);

                IElementHandle date_button = await nwpage.QuerySelectorAsync("a[id^='date-filter-update']");
                if (date_button == null)
                {
                    error_message = "DoTheDates: " + "No date button";
                    await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, error_message);
                    await SmartRoutinesV2018.Check_Trace(ourviewmodel, financeviewmodel.finance_token, institution_code, brand_code, error_message);
                    return error_message;
                }

                // So at least we know the "Search" button is there ...                                                                        
                //PageClickOptions opt = new PageClickOptions()
                //{
                //    Timeout = 60000
                //};
                // This ONLY took me an ENTIRE FUCKING WEEK-END
                // to get going ....
                await nwpage.ClickAsync("[id^='date-filter-update']");
                IResponse document = await nwpage.WaitForNavigationAsync();// async () =>


                

                //{
                //    await financeviewmodel.nwpage.ClickAsync("[id^='date-filter-update']", opt);
                //
                //}, resp => resp.Url.Contains("/Transactions/FullStatement/FullStatement") && resp.Status == 200);
                if (document == null)
                {
                    error_message = "Document transactions response is null";
                    return error_message;
                }
                else
                {
                    var response_itself = await document.JsonAsync();
                    ObservableCollection<SmartFinance.BankTransactions> Scraped =
                        await ReadAllFuckingAccounts(ourviewmodel,
                            financeviewmodel,
                            response_itself.ToString(),
                            currency,
                            sort_code,
                            account_no,
                            em => error_message = em,
                            institution_code,
                            brand_code);
                    if (Scraped.Count > 0)
                    {
                        Bollocks.AddRange(Scraped);
                    }
                }
            }
            catch (Exception ex)
            {
                // Probably a timeout
                //status = false;
                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "DoTheDates: " + ex.Message);
                await SmartRoutinesV2018.Check_Trace(ourviewmodel, financeviewmodel.finance_token, institution_code, brand_code, "DoTheDates: " + ex.Message);
                error_message = ex.Message;
            }
            return error_message;    // Although there may well have been no transactions ..
        }

        //
        // Scrape the JSON version
        //

        internal static async Task<ObservableCollection<SmartFinance.BankTransactions>> ReadAllFuckingAccounts(MainViewModel ourviewmodel,
                                                                        FinanceViewModel financeviewmodel,
                                                                        string response_itself,
                                                                        string currency,// Pull this in from the account
                                                                        string sort_code,
                                                                        string account_no,
                                                                        Action<string> set_error_message,
                                                                        short institution_code,
                                                                        short brand_code)
        {
            string accounts = string.Empty;

            ObservableCollection<SmartFinance.BankTransactions> Scraped = new ObservableCollection<SmartFinance.BankTransactions>();

            // This selects the ones we want (!!) with an EXTRA FUCKER which has no 'id' so
            // the Chimps don't let us filter that one out with a selector - we
            // have to FUCK ABOUT and do it ourselves .. I don't know how to
            // do 'multiple selectors' i.e. acLink AND href=/AccountList

            string amount = string.Empty,
                    date = string.Empty,
                    description = string.Empty,
                    ispaidin = string.Empty,
                    balance = string.Empty,
                    sequence_no,
                    transaction_type = string.Empty;
            bool is_paid_in = false;
            short transaction_code = 0,
                    transaction_sub_code = 0;

            DateTime converted_date = SmartParametersV2016.default_date;

            //ONE DATE for all ofs Batch of transactions!!

            DateTime transaction_created = SmartEncryptionV2016.DateTimeNow(ourviewmodel.gmt_offset);   // GMT time

            try
            {
                dynamic jsonResponse = JObject.Parse(response_itself);

                foreach (dynamic statement in jsonResponse)
                {
                    string statement_name = statement.Name;
                    if (statement_name == "Transactions")
                    {
                        foreach (dynamic transactions in statement)
                        {
                            foreach (dynamic transaction in transactions)
                            {
                                foreach (dynamic individual in transaction)
                                {
                                    string column = individual.Name;
                                    switch (column)
                                    {
                                        case "Amount":
                                            amount = individual.Value;
                                            int sep = amount.IndexOf(SmartParametersV2016.defaultCurrencySeparator);
                                            if (sep == -1)
                                            {
                                                amount = amount + SmartParametersV2016.defaultCurrencySeparator + "00";
                                            }
                                            else
                                            {
                                                if (sep + 2 == amount.Length)
                                                {
                                                    amount += "0";
                                                }
                                            }
                                            break;
                                        case "Date":
                                            try
                                            {
                                                date = individual.Value.ToString();
                                                converted_date = SmartRoutinesV2018.DateTimeParse(date).ToLocalTime();
                                            }
                                            catch (Exception ex)
                                            {
                                                await SmartRoutinesV2018.Check_Trace(ourviewmodel, financeviewmodel.finance_token, institution_code, brand_code, "ReadAllTransactions: " + ex.Message + " " + date);
                                            }
                                            break;
                                        case "Description":
                                            description = individual.Value;
                                            break;
                                        case "IsPaidIn":
                                            ispaidin = individual.Value;
                                            is_paid_in = Convert.ToBoolean(ispaidin);
                                            break;
                                        case "RunningBalance":
                                            balance = individual.Value;
                                            int sep2 = balance.IndexOf(SmartParametersV2016.defaultCurrencySeparator);
                                            if (sep2 == -1)
                                            {
                                                balance = balance + SmartParametersV2016.defaultCurrencySeparator + "00";
                                            }
                                            else
                                            {
                                                if (sep2 + 2 == balance.Length)
                                                {
                                                    balance += "0";
                                                }
                                            }
                                            break;
                                        case "Sequence":
                                            sequence_no = individual.Value;
                                            break;
                                        case "TransactionType":
                                            transaction_type = individual.Value;
                                            break;
                                        default:
                                            break;
                                    }
                                }

                                //if (!SmartSpikeFinanceV2017.Finance_Lookup_TransactionSubCode(ourviewmodel,
                                //                                                        financeviewmodel,
                                //                                                                            is_paid_in,
                                //                                                                            ref transaction_type,
                                //                                                                            ref transaction_code,
                                //                                                                            ref transaction_sub_code,
                                //                                                                            false,
                                //                                                                            institution_code,
                                //                                                                            brand_code))
                                //{
                                //    await SmartRoutinesV2018.Check_Trace(ourviewmodel, financeviewmodel.finance_token, institution_code, brand_code, "Trans type lookup failed: " + transaction_type);
                                //}

                                // Still want to add it in even if we can't analyze the transaction ...
                                int random1 = SmartRoutinesV2018.Get_Next_Random(ourviewmodel.random_r);
                                int random2 = SmartRoutinesV2018.Get_Next_Random(ourviewmodel.random_r);

                                SmartFinance.BankTransactions abc = new SmartFinance.BankTransactions()
                                {
                                    USERNAME = ourviewmodel.UserName,
                                    INSTITUTION_CODE = institution_code,
                                    BRAND_CODE = brand_code,
                                    SORTCODE = sort_code,
                                    ACCOUNT_NO = account_no,
                                    RANDOMKEY1 = random1,
                                    BOOKING_DATE = converted_date,
                                    SEQUENCE_NO = new DateTimeOffset(DateTime.Now + ourviewmodel.gmt_offset).ToUnixTimeMilliseconds(), // Unique AND increasing
                                    CREDITDEBIT_INDICATOR = is_paid_in,
                                    TRANSACTION_CODE = transaction_code,
                                    TRANSACTION_SUB_CODE = transaction_sub_code,
                                    DESCRIPTION = description,
                                    AMOUNT = Convert.ToInt32(amount.Replace(".", string.Empty)),
                                    CURRENCY_ORDINAL = 1,//currency,     // Pull this in from the Account
                                    BALANCE_AMOUNT = Convert.ToInt32(balance.Replace(".", string.Empty)),
                                    RANDOMKEY2 = random2,
                                    Updated = false
                                };

                                Scraped.Add(abc);

                            }
                        }
                    }
                }
                if (Scraped.Count > 0)
                {
                    await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Found: " + Scraped.Count.ToString() + " transactions");
                    await SmartRoutinesV2018.Check_Trace(ourviewmodel, financeviewmodel.finance_token, institution_code, brand_code, "Found: " + Scraped.Count.ToString() + " transactions");
                }
                else
                {
                    await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Found: No transactions");
                    await SmartRoutinesV2018.Check_Trace(ourviewmodel, financeviewmodel.finance_token, institution_code, brand_code, "Found: No transactions");
                }
            }
            catch (Exception ex)
            {
                // Probably a timeout
                set_error_message(ex.Message);
                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "ReadAllAccounts: " + ex.Message);
                await SmartRoutinesV2018.Check_Trace(ourviewmodel, financeviewmodel.finance_token, institution_code, brand_code, "ReadAllAccounts: " + ex.Message);
            }
            return Scraped;
        }
    }
}