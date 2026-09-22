using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
//using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.ObjectModel;



#if WINFORMS
using System.Windows.Forms;
using Microsoft.Playwright;
using Windows.UI.Notifications.Management;
using Windows.UI.Notifications;
#endif

#if WPF
using Windows.UI.Notifications.Management;
using Windows.UI.Notifications;
using Windows.ApplicationModel;
using Microsoft.Playwright;
#endif

#if WINDOWS_UWP
using Microsoft.Playwright;
using Windows.ApplicationModel;
using Windows.UI.Notifications;
using Windows.UI.Notifications.Management;
using Windows.Foundation;

#endif
#if XAMARIN
using Microsoft.Playwright;
using Windows.UI.Notifications.Management;
using Windows.UI.Notifications;
using Windows.Foundation;
using Xamarin.Forms;
#endif

#if MAUI
using SmartCubeMobile.Droid;
using Microsoft.Playwright;
#if !ANDROID
using Windows.UI.Notifications.Management;
using Windows.UI.Notifications;
#endif
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
        //public static IBrowser browser = null;
        internal static async Task<bool> NationwideProcess(
#if WINFORMS
                                                    RichTextBox textBoxConsole,
#endif
                                                    //SignInViewModel signinviewmodel,
                                                    MainViewModel ourviewmodel,
                                                    FinanceViewModel financeviewmodel,
                                                    IPlaywright playwright,
                                                    List<SmartFinance.BankTransactions> Bollocks,
                                                    string anchor_text,
                                                    string start_url,
                                                    string notification_title,
                                                    string notification_prefix,
                                                    short notification_tag_length,
                                                    float browser_timeout,
                                                    SmartFinance.Logins login_info,
                                                    short institution_code,
                                                    short brand_code)
                                                    //Action<string> set_error_message)
        {
            //var exitCode = Microsoft.Playwright.Program.Main(new[] { "install" });
            //if (exitCode != 0)
            //{
            //    Console.WriteLine("Failed to install browsers");
            //    //Environment.Exit(exitCode);
            //}

            if (login_info.PASSCODE.Length < 6)
            {
                financeviewmodel.errorMessage = "Invalid Passcode " + login_info.PASSCODE;
                return false;
            }
            if (login_info.DOB.Length < 8)
            {
                financeviewmodel.errorMessage = "Invalid DOB " + login_info.DOB;
                return false;
            }
            // Split the DOB into int's components
            string Day = login_info.DOB.Substring(0, 2);
            string Month = SmartParametersV2016.months[Convert.ToInt16(login_info.DOB.Substring(2, 2)) - 1];
            string Year = login_info.DOB.Substring(4, 4);

            string smsotp = string.Empty;

            // Wow! This fucking thing WORKS!!! THAT's a *FIRST* !!!!
            // https://www.meziantou.net/distributing-applications-that-depend-on-microsoft-playwright.htm
            //int exitCode = Microsoft.Playwright.Program.Main(new[] { "install" });
            //if (exitCode != 0)
            //{
            //    Console.WriteLine("Failed to install browsers");
            //    Environment.Exit(exitCode);
            //}
#if WINFORMS
            textBoxConsole.AppendText("Commencing SCRAPE section " +
                                            Environment.NewLine.ToString());
            textBoxConsole.ScrollToCaret();
#endif
            try
            {
                playwright = await Playwright.CreateAsync();
                //await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, " Playwright: created");
                //await SmartRoutinesV2018.CheckTrace(ourviewmodel, ourviewmodel.user_token, institution_code, brand_code, " Playwright: created");
                Microsoft.Playwright.IBrowser browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
                {
#if WINFORMS
                    ExecutablePath = @"C:\Users\Ray\AppData\Local\ms-playwright\chromium-1019\chrome-win\",
#endif
                    Headless = true,
                    Timeout = browser_timeout   // Nationwide specific
                });

                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, " Browser: headless " + browser_timeout.ToString());
                await SmartRoutinesV2018.CheckTrace(ourviewmodel, financeviewmodel.financeToken, institution_code, brand_code, "Browser: headless " + browser_timeout.ToString());

                //IBrowserContext context = await browser.NewContextAsync();

                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, " Browser: context");
                await SmartRoutinesV2018.CheckTrace(ourviewmodel, financeviewmodel.financeToken, institution_code, brand_code, "Browser: context");

                financeviewmodel.nwpage = await browser.NewPageAsync();

                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, " Navigating: " + start_url);
                await SmartRoutinesV2018.CheckTrace(ourviewmodel, financeviewmodel.financeToken, institution_code, brand_code, "Navigating: " + start_url);

                //Navigate to Nationwide
                IResponse document = await financeviewmodel.nwpage.GotoAsync(start_url);
                if (document == null)
                {
                    financeviewmodel.errorMessage = "Couldn't find url " + start_url;
                    return false;
                }

                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, " Retrieved: " + start_url);
                await SmartRoutinesV2018.CheckTrace(ourviewmodel, financeviewmodel.financeToken, institution_code, brand_code, "Retrieved: " + start_url);

#if WINFORMS
                textBoxConsole.AppendText("Retrieved page: " + start_url + Environment.NewLine.ToString());
                textBoxConsole.ScrollToCaret();
#endif

                // Could never get the fucking button to work ... ever
                //await page.ClickAsync("button[text='Log in']");

                string href = string.Empty;
                bool found_it = false;
                var all_https = await financeviewmodel.nwpage.QuerySelectorAllAsync("a[href^='https://onlinebanking']");
                if (all_https != null)
                {
                    foreach (var each_https in all_https)
                    {
                        string dataref = await each_https.GetAttributeAsync("data-ref");
                        href = await each_https.GetAttributeAsync("href");
                        if (dataref == "link")
                        {
                            //await nationwide_page.RunAndWaitForNavigationAsync(async () =>
                            //{
                            //document = 
                            found_it = true;
                            await financeviewmodel.nwpage.GotoAsync(url: href);
                            //});
                            break;
                        }
                    }
                }
                if (!found_it)
                {
                    financeviewmodel.errorMessage = "Couldn't find 'online' link";
                    return false;
                }
                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, " Found link: " + href);
                await SmartRoutinesV2018.CheckTrace(ourviewmodel, financeviewmodel.financeToken, institution_code, brand_code, "Found link: " + href);

                // Nationwide fuckers ... made me waste half a day ...
                IElementHandle button = await financeviewmodel.nwpage.QuerySelectorAsync("button[class='service-availability-continue-button']"); //, { WaitFor: "visible" });
                if (button != null)
                {
                    await button.ClickAsync();
                    await financeviewmodel.nwpage.WaitForLoadStateAsync(LoadState.DOMContentLoaded);

                };

                if (!await NationwideFindInputs(
#if WINFORMS
                                                            textBoxConsole,
#endif
                                                            login_info.CUSTOMER_NO,
                                            Day,
                                            Year,
                                            financeviewmodel.nwpage))
                {
                    financeviewmodel.errorMessage = "Couldn't find main inputs";
                    return false;
                }

                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, " Found inputs: " + login_info.CUSTOMER_NO + " " + Day + " " + Year);
                await SmartRoutinesV2018.CheckTrace(ourviewmodel, financeviewmodel.financeToken, institution_code, brand_code, "Found inputs: " + login_info.CUSTOMER_NO + " " + Day + " " + Year);

#if WINFORMS
                textBoxConsole.AppendText("Found all three" +
                                        Environment.NewLine.ToString());
                textBoxConsole.ScrollToCaret();
#endif
                // Now do the month dropdown

                var dateofbirth_month = await financeviewmodel.nwpage.SelectOptionAsync("select[name='DateOfBirthMonth']", Month);
                if (dateofbirth_month == null)
                {
                    financeviewmodel.errorMessage = "Couldn't find 'month' drop-down input";
                    return false;
                }

                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, " Found dropdown: " + Month);
                await SmartRoutinesV2018.CheckTrace(ourviewmodel, financeviewmodel.financeToken, institution_code, brand_code, "Found dropdown " + Month);

                // Press the fucking equivalent of 'Submit'!!!! (You couldn't make this shit up ...)
                await financeviewmodel.nwpage.Keyboard.PressAsync("Enter");
                // Wait for the fucker to get its act together
                await financeviewmodel.nwpage.WaitForNavigationAsync();

                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, " Submit: pressed");
                await SmartRoutinesV2018.CheckTrace(ourviewmodel, financeviewmodel.financeToken, institution_code, brand_code, "Submit: pressed");

#if WINFORMS
                textBoxConsole.AppendText("Passnumbers: " +
                                                login_info.PASSCODE.Substring(0, 1) +
                                                " " +
                                                login_info.PASSCODE.Substring(1, 1) +
                                                " " +
                                                login_info.PASSCODE.Substring(2, 1) +
                                                " " +
                                                login_info.PASSCODE.Substring(3, 1) +
                                                " " +
                                                login_info.PASSCODE.Substring(4, 1) +
                                                " " +
                                                login_info.PASSCODE.Substring(5, 1) +
                                                Environment.NewLine.ToString());
                textBoxConsole.ScrollToCaret();
#endif
                //
                // Passcodes
                //
                var passcodes = await financeviewmodel.nwpage.QuerySelectorAllAsync("span[class='control__label__title']");
                if (passcodes == null)
                {
                    financeviewmodel.errorMessage = "Couldn't find 'passcodes' span class";
                    return false;
                }

                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, " Passcodes: found");
                await SmartRoutinesV2018.CheckTrace(ourviewmodel, financeviewmodel.financeToken, institution_code, brand_code, "Passcodes: found");
                foreach (var each_passcode in passcodes)
                {
                    string text = await each_passcode.TextContentAsync();
                    if (text.Contains("digits from your passnumber"))
                    {
                        Console.WriteLine(text.ToString());

                        int index = 0;

                        // Look for which pass numbers they want
                        int[] zzz = new int[3];   // So we have 0, 1 and 2                                
#if WINFORMS
                        textBoxConsole.AppendText("Passnumbers reqd: " +
                                            text +
                                            Environment.NewLine.ToString());
                        textBoxConsole.ScrollToCaret();
#endif
                        if (text.Contains("1st"))
                        {
                            zzz[index] = 0;
                            index++;
                        };
                        if (text.Contains("2nd"))
                        {
                            zzz[index] = 1;
                            index++;
                        };
                        if (text.Contains("3rd"))
                        {
                            zzz[index] = 2;
                            index++;
                        };
                        if (text.Contains("4th"))
                        {
                            zzz[index] = 3;
                            index++;
                        };
                        if (text.Contains("5th"))
                        {
                            zzz[index] = 4;
                            index++;
                        };
                        if (text.Contains("6th"))
                        {
                            zzz[index] = 5;
                            index++;
                        };

                        await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, " Passcodes: " +
                                            login_info.PASSCODE.Substring(zzz[0], 1) +
                                            " " +
                                            login_info.PASSCODE.Substring(zzz[1], 1) +
                                            " " +
                                            login_info.PASSCODE.Substring(zzz[2], 1));
                        await SmartRoutinesV2018.CheckTrace(ourviewmodel, financeviewmodel.financeToken, institution_code, brand_code, "Passcodes: " +
                                            login_info.PASSCODE.Substring(zzz[0], 1) +
                                            " " +
                                            login_info.PASSCODE.Substring(zzz[1], 1) +
                                            " " +
                                            login_info.PASSCODE.Substring(zzz[2], 1));
#if WINFORMS
                        textBoxConsole.AppendText("Passnumbers to be sent: " +
                                            login_info.PASSCODE.Substring(zzz[0], 1) +
                                            " " +
                                            login_info.PASSCODE.Substring(zzz[1], 1) +
                                            " " +
                                            login_info.PASSCODE.Substring(zzz[2], 1) +
                                            Environment.NewLine.ToString());
                        textBoxConsole.ScrollToCaret();
#endif
                        var firstpassnumber = await financeviewmodel.nwpage.SelectOptionAsync("select[name='FirstPassnumberValue']", login_info.PASSCODE.Substring(zzz[0], 1));
                        if (firstpassnumber == null)
                        {
                            financeviewmodel.errorMessage = "Couldn't find 'First pass number' drop-down input";
                            return false;
                        }

                        var secondpassnumber = await financeviewmodel.nwpage.SelectOptionAsync("select[name='SecondPassnumberValue']", login_info.PASSCODE.Substring(zzz[1], 1));
                        if (secondpassnumber == null)
                        {
                            financeviewmodel.errorMessage = "Couldn't find 'Second pass number' drop-down input";
                            return false;
                        }

                        var thirdpassnumber = await financeviewmodel.nwpage.SelectOptionAsync("select[name='ThirdPassnumberValue']", login_info.PASSCODE.Substring(zzz[2], 1));
                        if (thirdpassnumber == null)
                        {
                            financeviewmodel.errorMessage = "Couldn't find 'Third pass number' drop-down input";
                            return false;
                        }
                        await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, " Passcodes: set");
                        await SmartRoutinesV2018.CheckTrace(ourviewmodel, financeviewmodel.financeToken, institution_code, brand_code, "Passcodes: set");
                        break;
                    }
                }

                // One-time Pass Code
                IReadOnlyList<IElementHandle> rolist = await financeviewmodel.nwpage.QuerySelectorAllAsync("button");
                if (rolist == null)
                {
                    financeviewmodel.errorMessage = "Couldn't find any buttons to push";
                    return false;
                }
                // Read the TOKEN HERE!!!

                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, " Button: found");
                await SmartRoutinesV2018.CheckTrace(ourviewmodel, financeviewmodel.financeToken, institution_code, brand_code, "Button: found");

                if (financeviewmodel.listener_enabled)
                {
                    if (ourviewmodel.accessStatus.ToString() != "Denied")
                    {
                        // Subscribe to Notification event
                        try
                        {
#if ANDROID
#if MAUI
                            await Task.Run(() => Application.Current.Dispatcher.Dispatch(() => new MyReceiver()));


#else
                            await Task.Run(() => Device.BeginInvokeOnMainThread(() => DependencyService.Get<ISMSOTP>().MyReceiver()));
#endif
#else
                            ourviewmodel.notificationListener.NotificationChanged += async (sender, e) => await Listener_NotificationChanged(sender,
                                                                                                                        e,
#if WINFORMS
                                                                                                                        textBoxConsole,
#endif
                                                                                                                        //signinviewmodel,
                                                                                                                        ourviewmodel,
                                                                                                                        financeviewmodel,
                                                                                                                        financeviewmodel.nwpage,
                                                                                                                        rolist,
                                                                                                                        Bollocks,
                                                                                                                        notification_title,
                                                                                                                        notification_prefix,
                                                                                                                        notification_tag_length,
                                                                                                                        login_info,             // To collect the address
                                                                                                                        institution_code,
                                                                                                                        brand_code,
                                                                                                                        //set_error_message,
                                                                                                                        em => smsotp = em);
#endif


                            await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, " Notifications: configured");
                            await SmartRoutinesV2018.CheckTrace(ourviewmodel, financeviewmodel.financeToken, institution_code, brand_code, "Notifications : configured");

                            // And press the Button to say 'Send me a OTP' please
                            // This next line won't work becuase the 'Continue' has line breaks in it
                            //IElementHandle cont_button = await financeviewmodel.nwpage.QuerySelectorAsync("button[text=Continue]");
                            //if (cont_button != null)
                            //{
                            //    Console.WriteLine("here");
                            //}                        
                            foreach (IElementHandle each_button in rolist)
                            {
                                string text = await each_button.TextContentAsync();
                                if (text.Trim() == "Continue")
                                {
                                    try
                                    {
                                        await financeviewmodel.nwpage.RunAndWaitForNavigationAsync(async () =>
                                        {
                                            // Triggers a navigation after a timeout
                                            await each_button.ClickAsync();
                                            await financeviewmodel.nwpage.WaitForLoadStateAsync(LoadState.DOMContentLoaded);

                                        });
                                    }
                                    catch (Exception ex)
                                    {

                                        await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Scrape aborted - no OTP recevied");
                                        await SmartRoutinesV2018.CheckTrace(ourviewmodel, financeviewmodel.financeToken, institution_code, brand_code, ex.Message);
                                        financeviewmodel.errorMessage = ex.Message;
                                        return false;
                                    }
                                    break;
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                financeviewmodel.errorMessage = ex.Message;
                return false;
            }

#if WINFORMS
            textBoxConsole.AppendText("Finished SCRAPE section" +
                                            Environment.NewLine.ToString());
            textBoxConsole.ScrollToCaret();
#endif
            await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Finished: SCRAPE section");
            await SmartRoutinesV2018.CheckTrace(ourviewmodel, financeviewmodel.financeToken, institution_code, brand_code, "Finished: SCRAPE section");
            return true;
        }

#if (WPF || WINDOWS_UWP || XAMARIN || MAUI) && SCRAPE
        internal sealed class UserNotifications
        {
            public AppInfo AppInfo { get; }
            public DateTimeOffset CreationTime { get; }
            public uint Id { get; }
            public Notification Notification { get; }
        }
#endif

#if (WINFORMS || WPF || WINDOWS_UWP || XAMARIN || MAUI)
#if !ANDROID
        internal static async Task<bool> Listener_NotificationChanged(

                                                        UserNotificationListener sender,
                                                        UserNotificationChangedEventArgs args,
#if WINFORMS
                                                        RichTextBox textBoxConsole,
#endif
                                                        //SignInViewModel signinviewmodel,
                                                        MainViewModel ourviewmodel,
                                                        FinanceViewModel financeviewmodel,
                                                        IPage nwpage,
                                                        IReadOnlyList<IElementHandle> rolist,
                                                        List<SmartFinance.BankTransactions> Bollocks,
                                                        string notification_title,
                                                        string notification_prefix,
                                                        short notification_tag_length,
                                                        SmartFinance.Logins logins_info,    // To collect address
                                                        short institution_code,
                                                        short brand_code,
                                                        //Action<string> set_error_message,
                                                        Action<string> set_smsotp)
        {

            await SmartRoutinesV2018.CheckTrace(ourviewmodel, financeviewmodel.financeToken, institution_code, brand_code, " Handler: event delivered");

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
                financeviewmodel.errorMessage = "No User Notifications found";
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

#if !WINFORMS
                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, " Found: " + titleText);
                await SmartRoutinesV2018.CheckTrace(ourviewmodel, financeviewmodel.financeToken, institution_code, brand_code, "Found: " + titleText);
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
                ourviewmodel.notificationListener.NotificationChanged -= null;

                bodyText = bodyText.Replace(notification_prefix, string.Empty).Trim(); //Trim gets rid of any spaces
                if (bodyText.Length <= 6)
                {
                    continue;
                }
                string notificationContent = bodyText.Substring(0, notification_tag_length);

                MainMeter.financeviewmodel.onetimepasscode = notificationContent;

#if !WINFORMS
                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, " Inserting: " + notificationContent);
                await SmartRoutinesV2018.CheckTrace(ourviewmodel, financeviewmodel.financeToken, institution_code, brand_code, "Inserting: " + notificationContent);
#endif
                //string onetimepasscode = notificationContent;
                if (string.IsNullOrEmpty(MainMeter.financeviewmodel.onetimepasscode))
                {
                    // Hardly likely to happen
                    financeviewmodel.errorMessage = "Notification content is empty";
                    return false;
                }
                set_smsotp(MainMeter.financeviewmodel.onetimepasscode);

                IElementHandle smsotp = await financeviewmodel.nwpage.QuerySelectorAsync("input[name='OneTimePasscode']");
                if (smsotp == null)
                {
                    // Hardly likely to happen, either
                    financeviewmodel.errorMessage = "Couldn't find the SMSOTP input";
                    return false;
                }

                await smsotp.FillAsync(MainMeter.financeviewmodel.onetimepasscode);

                rolist = await financeviewmodel.nwpage.QuerySelectorAllAsync("button");
                if (rolist.Count == 0)
                {
                    financeviewmodel.errorMessage = "Couldn't find any buttons";
                    return false;
                }

                foreach (IElementHandle each_button in rolist)
                {
                    string text = await each_button.TextContentAsync();
                    if (text.Trim() == "Log in")
                    {
                        await each_button.ClickAsync();
                        await financeviewmodel.nwpage.WaitForLoadStateAsync(LoadState.DOMContentLoaded);

                        // Find the Address <a> before all this shit goes haywire ..
                        //ILocator managerows = financeviewmodel.nwpage.Locator("a[class^='nav-link-unique-']"); // ("a[id^='primary-nav-title-level-1-sibling-1-unique-']");
                        ILocator managerows = financeviewmodel.nwpage.Locator("a[id^='primary-nav-link-level-3-sibling-1-unique-']");

                        if (!await FuckingLoopRound(//signinviewmodel,
                                            ourviewmodel,
                                            financeviewmodel,
                                            nwpage,
                                            Bollocks,
                                            institution_code,
                                            brand_code))
                        {
                            return false;
                        }
                        // Find address here?
                        // Its here
                        if (!await FindAddress(
#if WINFORMS
                                    textBoxConsole,
#endif
                                    //signinviewmodel,
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
#endif
        internal static async Task<bool> FindAddress(
#if WINFORMS
                                                    RichTextBox textBoxConsole,
#endif
                                                    //SignInViewModel signinviewmodel,
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

                IReadOnlyList< IElementHandle> manage_list = await financeviewmodel.nwpage.QuerySelectorAllAsync("a[id^='primary-nav-link-level-3-sibling-1-unique-']");
                foreach (IElementHandle manage_link in manage_list)
                {
                    string href1 = await manage_link.GetAttributeAsync("href");
                    if (href1.IndexOf("myaddress") > 0)
                    {
                        href1 = "https://onlinebanking.nationwide.co.uk" + href1;
                        await financeviewmodel.nwpage.GotoAsync(href1);
                        await financeviewmodel.nwpage.WaitForLoadStateAsync(LoadState.DOMContentLoaded);

                        IElementHandle address = await financeviewmodel.nwpage.QuerySelectorAsync("address");
                        if (address != null)
                        {
                            string myaddress = await address.InnerTextAsync();
                            myaddress = myaddress.Trim();
                            SmartUsers.AddressesView ideal_address = new SmartUsers.AddressesView();
                            ideal_address = await SmartRoutinesV2018.UsersUDPRNLookup(
#if WINFORMS
                                                    textBoxConsole,
#endif
                                                    ourviewmodel,
                                                    //financeviewmodel,
                                                    //xyz => ideal_address = xyz,
                                                    myaddress);

                            string udprn = ideal_address.UDPRN; // "12345678"; // "100011448863";
                            if (string.IsNullOrEmpty(account_info.UDPRN) ||
                                account_info.UDPRN != udprn)   // Change of address
                            {
                                account_info.UDPRN = udprn;
                                account_info.Updated = true; // Don't forget to update the Logins in DoAll_SmartFinance

                                List<SmartUsers.AddressesView> addresses_list = SmartSpikeFinanceV2017.Finance_Find_Addresses(ourviewmodel,
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
                                    ourviewmodel.Hamas.addressesview_changesList.Add(ideal_address);
                                    if (await SmartRoutinesV2018.DoAllSmartUsers(ourviewmodel,
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
                        IElementHandle phone_id = await financeviewmodel.nwpage.QuerySelectorAsync("a[id^='current-phone-number']");
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
                        IReadOnlyList<IElementHandle> email_details = await financeviewmodel.nwpage.QuerySelectorAllAsync("div[class^='details-value']");
                        foreach (IElementHandle email_div in email_details)
                        {
                            myemail = await email_div.InnerTextAsync();
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
                await SmartRoutinesV2018.CheckTrace(ourviewmodel, financeviewmodel.financeToken, institution_code, brand_code, "FindAddress : " + ex.Message);
            }
            return status;
        }

        internal static async Task<bool> FindBalanceAvailable(//SignInViewModel signinviewmodel,
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
                IReadOnlyList<IElementHandle> dls = await financeviewmodel.nwpage.QuerySelectorAllAsync("dl");
                foreach (IElementHandle dl_row in dls)
                {
                    IReadOnlyList<IElementHandle> dds = await financeviewmodel.nwpage.QuerySelectorAllAsync("dd");
                    int dd_count = 0;
                    foreach (IElementHandle dd_row in dds)
                    {
                        switch (dd_count)
                        {
                            case 0:
                                balance = await dd_row.TextContentAsync();
                                balance = balance.Trim();                                
                                string currency_symbol = balance.Substring(0, 1);
                                currency = SmartSpikeFinanceV2017.Finance_Lookup_Account_Currency(ourviewmodel.cultureviewList,
                                                                                        currency_symbol);
                                break;
                            case 1:
                                available = await dd_row.TextContentAsync();
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
                await SmartRoutinesV2018.CheckTrace(ourviewmodel, financeviewmodel.financeToken, institution_code, brand_code, "FindBalance: " + ex.Message);
            }
            return status;
        }

        internal static void ScrapedAccounts(List<FinanceViewModel.AccountItem> scraped_accounts,
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
                AccountID = account_id,
                Content = account_name,
                Value = account_value // Here we have    [0] = sort_code and account_no
                                      //                 [1] = balance
                                      //                 [2] = href
            };
            scraped_accounts.Add(account_item);
            return;
        }

        internal static async Task<bool> FuckingLoopRound(//SignInViewModel signinviewmodel,
                                                            MainViewModel ourviewmodel,
                                                            FinanceViewModel financeviewmodel,
                                                            IPage nwpage,
                                                            List<SmartFinance.BankTransactions> Bollocks,
                                                            short institution_code,
                                                            short brand_code)
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
                ILocator rows = financeviewmodel.nwpage.Locator("a[class^='acLink']");
                int no_of_rows = await rows.CountAsync();
                int account_id = -1;
                for (int row = 0; row < no_of_rows; row++)
                {
                    try
                    {
                        IElementHandle each_account = await rows.Nth(row).ElementHandleAsync();

                        string nwid = await each_account.GetAttributeAsync("id");

                        await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, " Processing: " + nwid);
                        await SmartRoutinesV2018.CheckTrace(ourviewmodel, financeviewmodel.financeToken, institution_code, brand_code, "Processing : " + nwid);

                        string href = await each_account.GetAttributeAsync("href");

                        account_id++;

                        string sort_code = string.Empty;
                        string account_no = string.Empty;
                        string balance = string.Empty;
                        string available = string.Empty;

                        string account_name = string.Empty;

                        List<FinanceViewModel.AccountItem> scraped_accounts = new List<FinanceViewModel.AccountItem>();

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
                        await financeviewmodel.nwpage.WaitForLoadStateAsync(LoadState.DOMContentLoaded);
                        if (all_text != null)
                        {
                            foreach (IElementHandle text_item in all_text)
                            {
                                account_name = await text_item.InnerTextAsync();
                                break;
                            }

                            if (!string.IsNullOrEmpty(account_name) &&  // sort_code might indeed be deliberately empty here ...
                                !string.IsNullOrEmpty(account_no) &&
                                !string.IsNullOrEmpty(href))
                            {
                                await financeviewmodel.nwpage.RunAndWaitForNavigationAsync(async () =>
                                {
                                    await each_account.ClickAsync();
                                });
                                string currency = string.Empty;

                                if (!await FindBalanceAvailable(//signinviewmodel,
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


                                IElementHandle reveal_date = await financeviewmodel.nwpage.QuerySelectorAsync("a[id^='enter-date-reveal-link']");
                                // Sometimes .. .NW accounts simply don't have a 'reveal date' (sigh)
                                if (reveal_date == null)
                                {
                                    await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "LoopRound: " + "No reveal date");
                                    await SmartRoutinesV2018.CheckTrace(ourviewmodel, financeviewmodel.financeToken, institution_code, brand_code, "LoopRound: " + "No reveal date");

                                }
                                else
                                {
                                    await reveal_date.ClickAsync();
                                    await financeviewmodel.nwpage.WaitForLoadStateAsync(LoadState.DOMContentLoaded);

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
                                        financeviewmodel.errorMessage = error_message;
                                        return false;
                                    }
                                }
                            }
                        }
                        await financeviewmodel.nwpage.GoBackAsync();
                    }
                    catch (Exception ex)
                    {
                        await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "LoopRound nwid: " + ex.Message + account_id.ToString());
                        await SmartRoutinesV2018.CheckTrace(ourviewmodel, financeviewmodel.financeToken, institution_code, brand_code, "LoopRound nwid : " + ex.Message + account_id.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                // Probably a timeout
                status = false;
                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "LoopRound: " + ex.Message);
                await SmartRoutinesV2018.CheckTrace(ourviewmodel, financeviewmodel.financeToken, institution_code, brand_code, "LoopRound: " + ex.Message);
            }
            return status;
        }

        internal static async Task<string> DoTheDatesBusiness(MainViewModel ourviewmodel,
                                                            FinanceViewModel financeviewmodel,
                                                            IPage nwpage,
                                                            List<SmartFinance.BankTransactions> Bollocks,
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
                DateTime time_now = DateTime.Now.Add(ourviewmodel.utcOffset);
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

                IElementHandle start_date = await financeviewmodel.nwpage.QuerySelectorAsync("input[name^='Start']");
                if (start_date == null)
                {
                    error_message = "DoTheDates: " + "No start date";

                    await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, error_message);
                    await SmartRoutinesV2018.CheckTrace(ourviewmodel, financeviewmodel.financeToken, institution_code, brand_code, error_message);

                    return error_message;
                }
                string from_date = "01/01/1900";    // This will be adjusted to the minimum by the website (I hope)
                await start_date.FillAsync(from_date);

                IElementHandle end_date = await financeviewmodel.nwpage.QuerySelectorAsync("input[name^='End']");
                if (end_date == null)
                {
                    error_message = "DoTheDates: " + "No end date";
                    await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, error_message);
                    await SmartRoutinesV2018.CheckTrace(ourviewmodel, financeviewmodel.financeToken, institution_code, brand_code, error_message);
                    return error_message;
                }
                string to_date = end_datex;
                await end_date.FillAsync(to_date);

                IElementHandle date_button = await financeviewmodel.nwpage.QuerySelectorAsync("a[id^='date-filter-update']");
                if (date_button == null)
                {
                    error_message = "DoTheDates: " + "No date button";
                    await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, error_message);
                    await SmartRoutinesV2018.CheckTrace(ourviewmodel, financeviewmodel.financeToken, institution_code, brand_code, error_message);
                    return error_message;
                }

                // So at least we know the "Search" button is there ...                                                                        
                PageClickOptions opt = new PageClickOptions()
                {
                    Timeout = 60000
                };
                // This ONLY took me an ENTIRE FUCKING WEEK-END
                // to get going ....
                IResponse document = await financeviewmodel.nwpage.RunAndWaitForResponseAsync(async () =>
                {
                    await financeviewmodel.nwpage.ClickAsync("[id^='date-filter-update']", opt);

                }, resp => resp.Url.Contains("/Transactions/FullStatement/FullStatement") && resp.Status == 200);
                if (document == null)
                {
                    error_message = "Document transactions response is null";
                    return error_message;
                }
                else
                {
                    var response_itself = await document.JsonAsync();
                    List<SmartFinance.BankTransactions> Scraped =
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
                await SmartRoutinesV2018.CheckTrace(ourviewmodel, financeviewmodel.financeToken, institution_code, brand_code, "DoTheDates: " + ex.Message);
                error_message = ex.Message;
            }
            return error_message;    // Although there may well have been no transactions ..
        }

        //
        // Scrape the JSON version
        //

        internal static async Task<List<SmartFinance.BankTransactions>> ReadAllFuckingAccounts(MainViewModel ourviewmodel,
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

            List<SmartFinance.BankTransactions> Scraped = new List<SmartFinance.BankTransactions>();

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

            DateTime converted_date = SmartParametersV2016.defaultDate;

            //ONE DATE for all ofs Batch of transactions!!

            DateTime transaction_created = DateTime.Now.Add(ourviewmodel.utcOffset);   // GMT time

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
                                                await SmartRoutinesV2018.CheckTrace(ourviewmodel, financeviewmodel.financeToken, institution_code, brand_code, "ReadAllTransactions: " + ex.Message + " " + date);
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

                                short[] codes = SmartSpikeFinanceV2017.Finance_Lookup_TransactionCode(financeviewmodel,
                                                                                                            financeviewmodel.transaction_types_found,
                                                                                                            is_paid_in,
                                                                                                            transaction_type,
                                                                                                            //ref transaction_code,
                                                                                                            //ref transaction_sub_code,
                                                                                                            false,
                                                                                                            institution_code,
                                                                                                            brand_code);
                                //{
                                //    await SmartRoutinesV2018.CheckTrace(ourviewmodel, financeviewmodel.financeToken, institution_code, brand_code, "Trans type lookup failed: " + transaction_type);
                                //}

                                // Still want to add it in even if we can't analyze the transaction ...
                                int random1 = SmartRoutinesV2018.Get_Next_Random(ourviewmodel.randomR);
                                int random2 = SmartRoutinesV2018.Get_Next_Random(ourviewmodel.randomR);

                                SmartFinance.BankTransactions abc = new SmartFinance.BankTransactions()
                                {
                                    USERNAME = ourviewmodel.UserName,
                                    INSTITUTION_CODE = institution_code,
                                    BRAND_CODE = brand_code,
                                    SORTCODE = sort_code,
                                    ACCOUNT_NO = account_no,
                                    RANDOMKEY1 = random1,
                                    BOOKING_DATE = converted_date,
                                    SEQUENCE_NO = new DateTimeOffset(DateTime.Now + ourviewmodel.utcOffset).ToUnixTimeMilliseconds(), // Unique AND increasing
                                    CREDITDEBIT_INDICATOR = is_paid_in,
                                    TRANSACTION_CODE = transaction_code,
                                    //TRANSACTION_SUB_CODE = transaction_sub_code,
                                    DESCRIPTION = description,
                                    AMOUNT = Convert.ToInt32(amount.Replace(".", string.Empty)),
                                    //CURRENCY = currency,     // Pull this in from the Account
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
                    await SmartRoutinesV2018.CheckTrace(ourviewmodel, financeviewmodel.financeToken, institution_code, brand_code, "Found: " + Scraped.Count.ToString() + " transactions");
                }
                else
                {
                    await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Found: No transactions");
                    await SmartRoutinesV2018.CheckTrace(ourviewmodel, financeviewmodel.financeToken, institution_code, brand_code, "Found: No transactions");
                }
            }
            catch (Exception ex)
            {
                // Probably a timeout
                set_error_message(ex.Message);
                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "ReadAllAccounts: " + ex.Message);
                await SmartRoutinesV2018.CheckTrace(ourviewmodel, financeviewmodel.financeToken, institution_code, brand_code, "ReadAllAccounts: " + ex.Message);
            }
            return Scraped;
        }

        //
        // Scrape the webpage version
        //
        //        internal static async Task<List<SmartFinance.BankTransactions>> ReadAllFuckingAccounts(MainViewModel ourviewmodel,
        //                                                                FinanceViewModel financeviewmodel,
        //                                                                IPage nwpage,
        //                                                                string sort_code,
        //                                                                string account_no,
        //                                                                Action<string> set_error_message,
        //                                                                short institution_code,
        //                                                                short brand_code)
        //        {


        //#if WPF || XAMARIN
        //            string accounts = string.Empty;

        //            List<SmartFinance.BankTransactions> Scraped = new List<SmartFinance.BankTransactions>();

        //            // This selects the ones we want (!!) with an EXTRA FUCKER which has no 'id' so
        //            // the Chimps don't let us filter that one out with a selector - we
        //            // have to FUCK ABOUT and do it ourselves .. I don't know how to
        //            // do 'multiple selectors' i.e. acLink AND href=/AccountList

        //            string currency = "GBP",    // Pull this in from the account
        //                    amount = string.Empty,
        //                    date = string.Empty,
        //                    description = string.Empty,
        //                    balance = string.Empty,
        //                    sequence_no,
        //                    transaction_type = string.Empty;
        //            bool    is_paid_in = false;
        //            short   transaction_code = 0,
        //                    transaction_sub_code = 0;

        //            DateTime converted_date = SmartParametersV2016.default_date;

        //            //ONE DATE for all ofs Batch of transactions!!

        //            DateTime transaction_created = SmartEncryptionV2016.DateTimeNow(ourviewmodel.gmt_offset);   // GMT time

        //            try
        //            {
        //                IReadOnlyList<IElementHandle> full_container = await financeviewmodel.nwpage.QuerySelectorAllAsync("div[id^='full-statement-container']");
        //                if (full_container != null)
        //                {
        //                    foreach (IElementHandle container in full_container)
        //                    {
        //                        IReadOnlyList<IElementHandle> bodies = await container.QuerySelectorAllAsync("tbody");
        //                        foreach (IElementHandle body in bodies)
        //                        {
        //                            IReadOnlyList<IElementHandle> tr_rows = await body.QuerySelectorAllAsync("tr");
        //                            if (tr_rows != null)
        //                            {
        //                                foreach (IElementHandle tr_row in tr_rows)
        //                                {
        //                                    IReadOnlyList<IElementHandle> td_rows = await tr_row.QuerySelectorAllAsync("td");
        //                                    int col = 0;
        //                                    string datex = string.Empty;
        //                                    transaction_type = string.Empty;
        //                                    description = string.Empty;
        //                                    string paid_in = string.Empty;
        //                                    string paid_out = string.Empty;
        //                                    is_paid_in = false;
        //                                    transaction_code = 0;
        //                                    transaction_sub_code = 0;
        //                                    amount = string.Empty;
        //                                    balance = string.Empty;
        //                                    currency = string.Empty;
        //                                    foreach (IElementHandle td_row in td_rows)
        //                                    {
        //                                        switch (col)
        //                                        {
        //                                            case 0: // Date
        //                                                datex = await td_row.InnerTextAsync();
        //                                                datex = datex.Trim();

        //                                                datex = datex.Substring(0, 11).Trim();
        //                                                try
        //                                                {
        //                                                    converted_date = SmartRoutinesV2018.DateTimeParse(datex).ToLocalTime();
        //                                                }
        //                                                catch (Exception ex)
        //                                                {
        //                                                    Console.WriteLine(ex.Message + datex);
        //                                                    await SmartRoutinesV2018.CheckTrace(ourviewmodel, financeviewmodel.financeToken, institution_code, brand_code, ex.Message + " " + datex);
        //                                                }
        //                                                break;
        //                                            case 1: // Type
        //                                                transaction_type = await td_row.InnerTextAsync();
        //                                                transaction_type = transaction_type.Trim();
        //                                                break;
        //                                            case 2: // Description
        //                                                description = await td_row.InnerTextAsync();
        //                                                description = description.Trim();
        //                                                description = description.Replace("&amp;", "&");
        //                                                break;
        //                                            case 3: // Paid In
        //                                                paid_in = await td_row.InnerTextAsync();
        //                                                paid_in = paid_in.Trim();
        //                                                break;
        //                                            case 4: // Paid out
        //                                                paid_out = await td_row.InnerTextAsync();
        //                                                paid_out = paid_out.Trim();
        //                                                break;
        //                                            case 5: // Balance
        //                                                balance = await td_row.InnerTextAsync();
        //                                                balance = balance.Trim();
        //                                                if (balance.IndexOf('£') != -1)
        //                                                {
        //                                                    balance = balance.Replace("£", string.Empty);
        //                                                }
        //                                                break;
        //                                            default:
        //                                                break;
        //                                        }
        //                                        col++;
        //                                    }
        //                                    // Should have a full line here ....
        //                                    if (string.IsNullOrEmpty(paid_out))
        //                                    {
        //                                        is_paid_in = true;
        //                                        amount = paid_in;
        //                                    }
        //                                    else
        //                                    {
        //                                        amount = paid_out;
        //                                    }
        //                                    if (amount.IndexOf('£') != -1)
        //                                    {
        //                                        currency = "GBP";
        //                                        amount = amount.Replace("£", string.Empty);
        //                                    }

        //                                    if (!SmartSpikeFinanceV2017.Lookup_Transaction_SubCode(financeviewmodel,
        //                                                                            is_paid_in,
        //                                                                            ref transaction_type,
        //                                                                            ref transaction_code,
        //                                                                            ref transaction_sub_code,
        //                                                                            false,
        //                                                                            institution_code,
        //                                                                            brand_code))
        //                                    {
        //                                        await SmartRoutinesV2018.CheckTrace(ourviewmodel, financeviewmodel.financeToken, institution_code, brand_code, "Trans type lookup failed: " + transaction_type);
        //                                    }

        //                                    // Still want to add it in even if we can't analyze the transaction ...
        //                                    int random1 = SmartRoutinesV2018.Get_Next_Random(ourviewmodel.random_r);
        //                                    int random2 = SmartRoutinesV2018.Get_Next_Random(ourviewmodel.random_r);

        //                                    try
        //                                    {
        //                                        SmartFinance.BankTransactions banktransaction = new SmartFinance.BankTransactions()
        //                                        {
        //                                            USERNAME = ourviewmodel.UserName,
        //                                            INSTITUTION_CODE = institution_code,
        //                                            BRAND_CODE = brand_code,
        //                                            SORTCODE = sort_code,
        //                                            ACCOUNT_NO = account_no,
        //                                            RANDOMKEY1 = random1,
        //                                            BOOKING_DATE = converted_date,
        //                                            SEQUENCE_NO = new DateTimeOffset(DateTime.Now + ourviewmodel.gmt_offset).ToUnixTimeMilliseconds(), // Unique AND increasing
        //                                            CREDITDEBIT_INDICATOR = is_paid_in,
        //                                            TRANSACTION_CODE = transaction_code,
        //                                            TRANSACTION_SUB_CODE = transaction_sub_code,
        //                                            DESCRIPTION = description,
        //                                            AMOUNT = Convert.ToInt32(amount.Replace(".", string.Empty)),
        //                                            CURRENCY = currency,     // Pull this in from the Account
        //                                            BALANCE_AMOUNT = Convert.ToInt32(balance.Replace(".", string.Empty)),
        //                                            RANDOMKEY2 = random2,
        //                                            Updated = false
        //                                        };
        //                                        Scraped.Add(banktransaction);
        //                                    }
        //                                    catch (Exception ex)
        //                                    {
        //                                        Console.WriteLine(ex.Message);

        //                                        await SmartRoutinesV2018.CheckTrace(ourviewmodel, financeviewmodel.financeToken, institution_code, brand_code, ex.Message + " " + "Cannot add Transaction record");

        //                                    }
        //                                }
        //                            }
        //                        }
        //                    }
        //                }
        //            }
        //            catch(Exception ex)
        //            {
        //                Console.WriteLine(ex.Message);

        //                await SmartRoutinesV2018.CheckTrace(ourviewmodel, financeviewmodel.financeToken, institution_code, brand_code, ex.Message + " " + "General failure");

        //            }
        //            return Scraped;
        //#endif
        //        }



        //internal static async Task<List<FinanceViewModel.AccountItem>> NationwideBuildAccountList(MainViewModel ourviewmodel,
        //                                                        FinanceViewModel financeviewmodel,
        //                                                        IReadOnlyList<IElementHandle> all_accounts)
        //{
        //    List<FinanceViewModel.AccountItem> scraped_accounts = new List<FinanceViewModel.AccountItem>();

        //    string accounts = string.Empty;
        //    int account_id = 0;



        //    foreach (IElementHandle each_account in all_accounts)
        //    {
        //        string sort_code = string.Empty;
        //        string account_no = string.Empty;
        //        string balance = "£0.0";

        //        string href = string.Empty;
        //        string nwid = string.Empty;
        //        string account_name = string.Empty;
        //        try
        //        {
        //            nwid = await each_account.GetAttributeAsync("id");
        //            href = await each_account.GetAttributeAsync("href");

        //            if (!string.IsNullOrEmpty(nwid))
        //            {
        //                string[] details = nwid.Split(SmartParametersV2016.space);
        //                if (details.Count() > 1)
        //                {
        //                    sort_code = details[0];
        //                    account_no = details[1];
        //                }
        //                else
        //                {
        //                    account_no = details[0];
        //                }
        //            }

        //            IReadOnlyList<IElementHandle> all_text = await each_account.QuerySelectorAllAsync("b");
        //            if (all_text != null)
        //            {
        //                foreach (IElementHandle text_item in all_text)
        //                {
        //                    account_name = await text_item.InnerTextAsync();
        //                    break;
        //                }
        //            }
        //            if (!string.IsNullOrEmpty(account_name) &&
        //                !string.IsNullOrEmpty(account_no) &&
        //                !string.IsNullOrEmpty(href))
        //            {
        //                string account_value = sort_code + SmartParametersV2016.unit_separator +
        //                            account_no +
        //                            //":" +
        //                            //"GBP" +     // Assume we can find it in OpenBanking
        //                            SmartParametersV2016.ourSeparator +
        //                            balance +
        //                            SmartParametersV2016.ourSeparator +
        //                            href;    // Safety check
        //                                     //string[] array = val.Split(':');
        //                FinanceViewModel.AccountItem account_item = new FinanceViewModel.AccountItem()
        //                {
        //                    Account_ID = account_id,
        //                    Content = account_name,
        //                    Value = account_value // Here we have    [0] = sort_code and account_no
        //                                          //                 [1] = balance
        //                                          //                 [2] = href
        //                };
        //                scraped_accounts.Add(account_item);
        //                account_id++;
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            Console.WriteLine(ex.Message);
        //        }

        //    }
        //    return scraped_accounts;
        //}

        internal static async Task<bool> NationwideFindInputs(
#if WINFORMS
                                                RichTextBox textBoxConsole,
#endif
                                                string CustomerNumber,
                                                string Day,
                                                string Year
                                                , IPage nationwide_page
                                                )
        {
#if WINFORMS
            textBoxConsole.AppendText("Finding inputs: " + Environment.NewLine.ToString());
            textBoxConsole.ScrollToCaret();
#endif

            bool status = false;

#if WINFORMS || WPF || WINDOWS_UWP || XAMARIN || MAUI
            bool customernumber_found = false,
                dateofbirthday_found = false,
                dateofbirthyear_found = false;
#endif
            IReadOnlyList<IElementHandle> all_input = await nationwide_page.QuerySelectorAllAsync("input");
            if (all_input != null)
            {
                foreach (IElementHandle each_input in all_input)
                {
                    string name = await each_input.GetAttributeAsync("name");
#if WINFORMS
                    //textBoxConsole.AppendText("Text: " + pageInput.TextContent +
                    //                           " Name: " + pageInput.NameAttribute +
                    //                            Environment.NewLine.ToString());
                    //textBoxConsole.ScrollToCaret();
#endif
                    string aha = await each_input.GetAttributeAsync("type");
                    if (aha == "text")
                    {
                        switch (name)
                        {
                            case "CustomerNumber":
                                if (!customernumber_found)
                                {
                                    customernumber_found = true;
                                    await each_input.FillAsync(CustomerNumber);
                                }
                                break;
                            case "DateOfBirthDay":
                                if (!dateofbirthday_found)
                                {
                                    dateofbirthday_found = true;
                                    await each_input.FillAsync(Day);
                                }
                                break;
                            case "DateOfBirthYear":
                                if (!dateofbirthyear_found)
                                {
                                    dateofbirthyear_found = true;
                                    await each_input.FillAsync(Year);
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
            return status;
        }

        //internal static bool SplitDob(string dob,
        //                                ref string day,
        //                                ref string month,
        //                                ref string year)
        //{
        //    if (dob.Length == 8)
        //    {
        //        try
        //        {
        //            day = dob.Substring(0, 2);
        //            int month_index = Convert.ToInt16(dob.Substring(2, 2)) - 1;
        //            month = SmartParametersV2016.months[month_index];
        //            year = dob.Substring(4, 4);
        //            return true;
        //        }
        //        catch
        //        {
        //            // Anything that goes wrong return false
        //            Console.WriteLine("here");
        //        }
        //    }
        //    return false;
        //}
    }
}