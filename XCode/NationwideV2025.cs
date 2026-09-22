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
using System.Windows.Controls;
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

    public class NationwideV2025
    {
        internal static async Task<bool> NationwideProcess(
#if WINFORMS
                                                    RichTextBox textBoxConsole,
#endif
                                                    MainViewModel ourviewmodel,
                                                    FinanceViewModel financeviewmodel,
                                                    List<SmartFinance.Transaction_Types> transactionTypesFound,
                                                    string anchor_text,
                                                    string start_url,
                                                    string notification_title,
                                                    string notification_prefix,
                                                    short notification_tag_length,
                                                    float browser_timeout,
                                                    SmartFinance.Logins login_info,
                                                    short institution_code,
                                                    short brand_code)
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
                financeviewmodel.webPage = await ourviewmodel.webBrowser.NewPageAsync();

                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, " Navigating: " + start_url);
                await SmartRoutinesV2018.CheckTrace(ourviewmodel, financeviewmodel.financeToken, institution_code, brand_code, "Navigating: " + start_url);

                //Navigate to Nationwide
                IResponse document = await financeviewmodel.webPage.GotoAsync(start_url);
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
                var all_https = await financeviewmodel.webPage.QuerySelectorAllAsync("a[href^='https://onlinebanking']");
                if (all_https != null)
                {
                    foreach (var each_https in all_https)
                    {
                        string dataref = await each_https.GetAttributeAsync("data-ref");
                        href = await each_https.GetAttributeAsync("href");
                        if (dataref == "link")
                        {
                            found_it = true;
                            await financeviewmodel.webPage.GotoAsync(url: href);
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
                try
                {
                    IElementHandle button = await financeviewmodel.webPage.QuerySelectorAsync("button[class='service-availability-continue-button']"); //, { WaitFor: "visible" });
                    if (button != null)
                    {
                        await button.ClickAsync();
                        await financeviewmodel.webPage.WaitForLoadStateAsync(LoadState.DOMContentLoaded);

                    };
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    return false;
                }

                if (!await NationwideFindInputs(
#if WINFORMS
                                            textBoxConsole,
#endif
                                            login_info.CUSTOMER_NO,
                                            Day,
                                            Year,
                                            financeviewmodel.webPage))
                {
                    financeviewmodel.errorMessage = "Couldn't find main inputs";
                    return false;
                }
                await financeviewmodel.webPage.WaitForLoadStateAsync(LoadState.DOMContentLoaded);

                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, " Found inputs: " + login_info.CUSTOMER_NO + " " + Day + " " + Year);
                await SmartRoutinesV2018.CheckTrace(ourviewmodel, financeviewmodel.financeToken, institution_code, brand_code, "Found inputs: " + login_info.CUSTOMER_NO + " " + Day + " " + Year);

#if WINFORMS
                textBoxConsole.AppendText("Found all three" +
                                        Environment.NewLine.ToString());
                textBoxConsole.ScrollToCaret();
#endif
                // Now do the month dropdown

                IReadOnlyList<string> dateofbirth_month = await financeviewmodel.webPage.SelectOptionAsync("select[name='DateOfBirthMonth']", Month);
                if (dateofbirth_month == null)
                {
                    financeviewmodel.errorMessage = "Couldn't find 'month' drop-down input";
                    return false;
                }
                await financeviewmodel.webPage.WaitForLoadStateAsync(LoadState.DOMContentLoaded);

                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, " Found dropdown: " + Month);
                await SmartRoutinesV2018.CheckTrace(ourviewmodel, financeviewmodel.financeToken, institution_code, brand_code, "Found dropdown " + Month);

                try
                {
                    // Press the fucking equivalent of 'Submit'!!!! (You couldn't make this shit up ...)

                    await financeviewmodel.webPage.GetByRole(AriaRole.Button, new() { Name = "Continue" }).ClickAsync();
                    await financeviewmodel.webPage.WaitForLoadStateAsync(LoadState.DOMContentLoaded);

                    //await financeviewmodel.webPage.Keyboard.PressAsync("Enter");
                    //await financeviewmodel.webPage.WaitForLoadStateAsync(LoadState.DOMContentLoaded);

                    //await financeviewmodel.webPage.WaitForNavigationAsync();                    
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    return false;
                }
                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, " Passcodes: pressed");
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
                if (!await NationwideFindPasscodes(ourviewmodel,
                                                financeviewmodel,
                                                institution_code,
                                                brand_code,
                                                login_info))
                {
                    return false;
                }
                else
                {
                    await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, " After passcodes");

                    // One-time Pass Code
                    await financeviewmodel.webPage.WaitForLoadStateAsync(LoadState.DOMContentLoaded);
                    await financeviewmodel.webPage.WaitForLoadStateAsync(LoadState.Load);
                    await financeviewmodel.webPage.WaitForRequestFinishedAsync();
                    
                    //await financeviewmodel.webPage.ReloadAsync();



                    try
                    {
                        // Press the fucking equivalent of 'Submit'!!!! (You couldn't make this shit up ...)
                        //var locator = financeviewmodel.webPage.GetByText("Continue");
                        //await locator.HoverAsync();
                        //await locator.ClickAsync();
                        

                        await financeviewmodel.webPage.GetByRole(AriaRole.Button, new() { Name = "Continue" }).ClickAsync();
                        //await financeviewmodel.webPage.Keyboard.PressAsync("Continue");
                        //await financeviewmodel.webPage.WaitForURLAsync("https://onlinebanking.nationwide.co.uk/AccessManagement/Login/SendSMSOTP");
                        //await financeviewmodel.webPage.WaitForLoadStateAsync(LoadState.DOMContentLoaded);

                        //await financeviewmodel.webPage.WaitForNavigationAsync(new() {Timeout = 0 });
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        return false;
                    }

                    //DomNodeList domButtons = financeviewmodel.webPage.getElementsByTagName("button");

                    //IReadOnlyList<IElementHandle> rolist = await financeviewmodel.webPage.QuerySelectorAllAsync("button[class='action__button'][title='']");
                    //if (rolist == null)
                    //{
                    //    financeviewmodel.errorMessage = "Couldn't find any buttons to push";
                    //    return false;
                    //}

                    //await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, " Button: found");
                    //await SmartRoutinesV2018.CheckTrace(ourviewmodel, financeviewmodel.financeToken, institution_code, brand_code, "Button: found");

                    //// And press the Button to say 'Send me a OTP' please
                    //foreach (IElementHandle each_button1 in rolist)
                    //{
                    //    string text22 = await each_button1.TextContentAsync();
                    //    if (text22.Trim() == "Continue")
                    //    {
                    //        try
                    //        {
                    //            await each_button1.ClickAsync();
                    //            // Wait for the fucker to get its act together
                    //            //await financeviewmodel.webPage.WaitForLoadStateAsync(LoadState.DOMContentLoaded);

                    //            await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, " Waiting 202");


                    //        }
                    //        catch (Exception ex)
                    //        {
                    //            Console.WriteLine(ex.Message);
                    //            return false;
                    //        }
                    //        break;
                    //    }
                    //}


                    await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, " SendOTP: pressed");
                    await SmartRoutinesV2018.CheckTrace(ourviewmodel, financeviewmodel.financeToken, institution_code, brand_code, "Submit: pressed");
                    
                    if (ourviewmodel.listenerenabled)
                    {
                        if (ourviewmodel.accessStatus.ToString() != "Denied")
                        {
                            // Subscribe to Notification event
                            try
                            {
#if ANDROID
                                await Task.Run(() => Device.BeginInvokeOnMainThread(() => DependencyService.Get<ISMSOTP>().MyReceiver()));
#endif
                                ourviewmodel.notificationListener.NotificationChanged += async (sender, e) => await Listener_NotificationChanged(sender,
                                                                                                                            e,
#if WINFORMS
                                                                                                        textBoxConsole,
#endif
                                                                                                                            ourviewmodel,
                                                                                                                            financeviewmodel,
                                                                                                                            transactionTypesFound,
                                                                                                                            financeviewmodel.webPage,
                                                                                                                            //rolist,
                                                                                                                            notification_title,
                                                                                                                            notification_prefix,
                                                                                                                            notification_tag_length,
                                                                                                                            login_info,             // To collect the address
                                                                                                                            institution_code,
                                                                                                                            brand_code,
                                                                                                                            //set_error_message,
                                                                                                                            em => smsotp = em);


                                //await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, " Notifications: configured");
                                //await SmartRoutinesV2018.CheckTrace(ourviewmodel, financeviewmodel.financeToken, institution_code, brand_code, "Notifications : configured");

                                // And press the Button to say 'Send me a OTP' please
                                //await financeviewmodel.webPage.Keyboard.PressAsync("Enter");
                                // Wait for the fucker to get its act together
                                //await financeviewmodel.webPage.WaitForLoadStateAsync(LoadState.DOMContentLoaded);
                                //await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, " SendOTP: pressed");
                                //await SmartRoutinesV2018.CheckTrace(ourviewmodel, financeviewmodel.financeToken, institution_code, brand_code, "Submit: pressed");

                                // This next line won't work becuase the 'Continue' has line breaks in it
                                //IElementHandle cont_button = await financeviewmodel.webPage.QuerySelectorAsync("button[text=Continue]");
                                //if (cont_button != null)
                                //{
                                //    Console.WriteLine("here");
                                //}                        
                                //foreach (IElementHandle each_button in rolist)
                                //{
                                //    string text = await each_button.TextContentAsync();
                                //    if (text.Trim() == "Continue")
                                //    {
                                //        try
                                //        {
                                //            //await financeviewmodel.webPage.RunAndWaitForNavigationAsync(async () =>
                                //            //{
                                //                // Triggers a navigation after a timeout
                                //                await each_button.ClickAsync();
                                //                await financeviewmodel.webPage.WaitForLoadStateAsync(LoadState.DOMContentLoaded);
                                //            Console.WriteLine("here");
                                //            //});
                                //        }
                                //        catch (Exception ex)
                                //        {

                                //            await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Scrape aborted - no OTP recevied");
                                //            await SmartRoutinesV2018.CheckTrace(ourviewmodel, financeviewmodel.financeToken, institution_code, brand_code, ex.Message);
                                //            financeviewmodel.errorMessage = ex.Message;
                                //            return false;
                                //        }
                                //        break;
                                //    }
                                //}
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex.Message);
                            }
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

#if (WINFORMS || WPF || WINDOWS_UWP)
#if !ANDROID
        internal static async Task<bool> Listener_NotificationChanged(

                                                        UserNotificationListener sender,
                                                        UserNotificationChangedEventArgs args,
#if WINFORMS
                                                        RichTextBox textBoxConsole,
#endif
                                                        MainViewModel ourviewmodel,
                                                        FinanceViewModel financeviewmodel,
                                                        List<SmartFinance.Transaction_Types> transactionTypesFound,
                                                        IPage webPage,
                                                        //IReadOnlyList<IElementHandle> rolist,
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

                IElementHandle smsotp = await financeviewmodel.webPage.QuerySelectorAsync("input[name='OneTimePasscode']");
                if (smsotp == null)
                {
                    // Hardly likely to happen, either
                    financeviewmodel.errorMessage = "Couldn't find the SMSOTP input";
                    return false;
                }

                await smsotp.FillAsync(MainMeter.financeviewmodel.onetimepasscode);

                //rolist = await financeviewmodel.webPage.QuerySelectorAllAsync("button");
                //if (rolist.Count == 0)
                //{
                //    financeviewmodel.errorMessage = "Couldn't find any buttons";
                ///    return false;
                //}

                try
                {
                    // Press the fucking equivalent of 'Submit'!!!! (You couldn't make this shit up ...)
                    await financeviewmodel.webPage.Keyboard.PressAsync("Enter");
                    await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, " Log-in: pressed???");
                    await SmartRoutinesV2018.CheckTrace(ourviewmodel, financeviewmodel.financeToken, institution_code, brand_code, "Submit: pressed");

                    // Find the Address <a> before all this shit goes haywire ..
                    //ILocator managerows = financeviewmodel.webPage.Locator("a[class^='nav-link-unique-']"); // ("a[id^='primary-nav-title-level-1-sibling-1-unique-']");
                    ILocator managerows = financeviewmodel.webPage.Locator("a[id^='primary-nav-link-level-3-sibling-1-unique-']");

                    if (!await FuckingLoopRound(ourviewmodel,
                                        financeviewmodel,
                                        transactionTypesFound,
                                        webPage,
                                        //Bollocks,
                                        institution_code,
                                        brand_code))
                    {
                        await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, " Loop round: Failed");
                        await SmartRoutinesV2018.CheckTrace(ourviewmodel, financeviewmodel.financeToken, institution_code, brand_code, "Loop round: failed");

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
                                webPage,
                                institution_code,
                                brand_code))
                    {
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    return false;
                }

//                foreach (IElementHandle each_button in rolist)
//                {
//                    string text = await each_button.TextContentAsync();
//                    if (text.Trim() == "Log in")
//                    {
//                        await each_button.ClickAsync();
//                        await financeviewmodel.webPage.WaitForLoadStateAsync(LoadState.DOMContentLoaded);

//                        // Find the Address <a> before all this shit goes haywire ..
//                        //ILocator managerows = financeviewmodel.webPage.Locator("a[class^='nav-link-unique-']"); // ("a[id^='primary-nav-title-level-1-sibling-1-unique-']");
//                        ILocator managerows = financeviewmodel.webPage.Locator("a[id^='primary-nav-link-level-3-sibling-1-unique-']");

//                        if (!await FuckingLoopRound(//signinviewmodel,
//                                            ourviewmodel,
//                                            financeviewmodel,
//                                            transactionTypesFound,
//                                            webPage,
//                                            //Bollocks,
//                                            institution_code,
//                                            brand_code))
//                                            //set_error_message))
//                        {
//                            return false;
//                        }
//                        // Find address here?
//                        // Its here
//                        if (!await FindAddress(
//#if WINFORMS
//                                    textBoxConsole,
//#endif
//                                    //signinviewmodel,
//                                    ourviewmodel,
//                                    financeviewmodel,
//                                    webPage,
//                                    institution_code,
//                                    brand_code))
//                        {
//                            return false;
//                        }
//                    }
//                }
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
                                                    IPage webPage,
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

                IReadOnlyList< IElementHandle> manage_list = await financeviewmodel.webPage.QuerySelectorAllAsync("a[id^='primary-nav-link-level-3-sibling-1-unique-']");
                foreach (IElementHandle manage_link in manage_list)
                {
                    string href1 = await manage_link.GetAttributeAsync("href");
                    if (href1.IndexOf("myaddress") > 0)
                    {
                        href1 = "https://onlinebanking.nationwide.co.uk" + href1;
                        await financeviewmodel.webPage.GotoAsync(href1);
                        await financeviewmodel.webPage.WaitForLoadStateAsync(LoadState.DOMContentLoaded);

                        IElementHandle address = await financeviewmodel.webPage.QuerySelectorAsync("address");
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
                            //{
                            //}
                            //else
                            //{
                            //    return false;
                            //}

                            //                            string UDPRN = await FindDetails(
                            //#if WINFORMS
                            //                                                //textBoxConsole,
                            //#endif
                            //                                                ourviewmodel,
                            //                                                financeviewmodel,
                            //                                                //ourviewmodel.webClient,
                            //                                                //financeviewmodel.baseUri,
                            //                                                addressPage,
                            //                                                financeviewmodel.login_info,
                            //                                                owner);
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
                        IElementHandle phone_id = await financeviewmodel.webPage.QuerySelectorAsync("a[id^='current-phone-number']");
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
                        IReadOnlyList<IElementHandle> email_details = await financeviewmodel.webPage.QuerySelectorAllAsync("div[class^='details-value']");
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
                                                                IPage webPage,
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
                IReadOnlyList<IElementHandle> dls = await financeviewmodel.webPage.QuerySelectorAllAsync("dl");
                foreach (IElementHandle dl_row in dls)
                {
                    IReadOnlyList<IElementHandle> dds = await financeviewmodel.webPage.QuerySelectorAllAsync("dd");
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
                                                            List<SmartFinance.Transaction_Types> transactionTypesFound,
                                                            IPage webPage,
                                                            //List<SmartFinance.BankTransactions> Bollocks,
                                                            short institution_code,
                                                            short brand_code)
                                                            //Action<string> set_error_message)
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
                ILocator rows = financeviewmodel.webPage.Locator("a[class^='acLink']");
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
                            if (details.Length > 1)
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
                        await financeviewmodel.webPage.WaitForLoadStateAsync(LoadState.DOMContentLoaded);
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
                                //await financeviewmodel.webPage.RunAndWaitForNavigationAsync(async () =>
                                //{
                                    await each_account.ClickAsync();
                                //});
                                string currency = string.Empty;

                                if (!await FindBalanceAvailable(//signinviewmodel,
                                                            ourviewmodel,
                                                            financeviewmodel,
                                                            webPage,
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


                                IElementHandle reveal_date = await financeviewmodel.webPage.QuerySelectorAsync("a[id^='enter-date-reveal-link']");
                                // Sometimes .. .NW accounts simply don't have a 'reveal date' (sigh)
                                if (reveal_date == null)
                                {
                                    await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "LoopRound: " + "No reveal date");
                                    await SmartRoutinesV2018.CheckTrace(ourviewmodel, financeviewmodel.financeToken, institution_code, brand_code, "LoopRound: " + "No reveal date");

                                }
                                else
                                {
                                    await reveal_date.ClickAsync();
                                    await financeviewmodel.webPage.WaitForLoadStateAsync(LoadState.DOMContentLoaded);

                                    // DATES!!  Wherever you are in the world, Nationwide
                                    // dates and times are always local to the UK, so
                                    // you always need to add the 'gmtoffset' to the 'time now'
                                    // in order to get 'UK time'

                                    string error_message = await DoTheDatesBusiness(ourviewmodel,
                                                            financeviewmodel,
                                                            transactionTypesFound,
                                                            webPage,
                                                            //Bollocks,
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
                        await financeviewmodel.webPage.GoBackAsync();
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
                                                            List<SmartFinance.Transaction_Types> transactionTypesFound,
                                                            IPage webPage,
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
                //DateTime time_now = SmartEncryptionV2016.DateTimeNow(SmartParametersV2016.gmtdefaultOffset);
                string end_datex = DateTime.Now.ToString(SmartParametersV2016.standardFormat);

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

                IElementHandle start_date = await financeviewmodel.webPage.QuerySelectorAsync("input[name^='Start']");
                if (start_date == null)
                {
                    error_message = "DoTheDates: " + "No start date";

                    await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, error_message);
                    await SmartRoutinesV2018.CheckTrace(ourviewmodel, financeviewmodel.financeToken, institution_code, brand_code, error_message);

                    return error_message;
                }
                string from_date = "01/01/1900";    // This will be adjusted to the minimum by the website (I hope)
                await start_date.FillAsync(from_date);

                IElementHandle end_date = await financeviewmodel.webPage.QuerySelectorAsync("input[name^='End']");
                if (end_date == null)
                {
                    error_message = "DoTheDates: " + "No end date";
                    await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, error_message);
                    await SmartRoutinesV2018.CheckTrace(ourviewmodel, financeviewmodel.financeToken, institution_code, brand_code, error_message);
                    return error_message;
                }
                string to_date = end_datex;
                await end_date.FillAsync(to_date);

                IElementHandle date_button = await financeviewmodel.webPage.QuerySelectorAsync("a[id^='date-filter-update']");
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
                IResponse document = await financeviewmodel.webPage.RunAndWaitForResponseAsync(async () =>
                {
                    await financeviewmodel.webPage.ClickAsync("[id^='date-filter-update']", opt);

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
                            //em => error_message = em,
                            institution_code,
                            brand_code);
                    if (Scraped.Count > 0)
                    {
                        financeviewmodel.bank_transactions.AddRange(Scraped);
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

        // scrape the old NW version
        internal static async Task<bool> Nationwide_Find_Transactions(string responseData,
                                                                        MainViewModel ourviewmodel,
                                                                        FinanceViewModel financeviewmodel,
                                                                        List<SmartFinance.Transaction_Types> transactionTypesFound,
                                                                        short institution_code,
                                                                        short brand_code,
                                                                        string sortcode,
                                                                        string account_no,
                                                                        string UDPRN,
                                                                        short currencyOrdinal,
                                                                        ObservableCollection<SmartFinance.Transaction_Types> transaction_types_found)
        {
            // This selects the ones we want (!!) with an EXTRA FUCKER which has no 'id' so
            // the Chimps don't let us filter that one out with a selector - we
            // have to FUCK ABOUT and do it ourselves .. I don't know how to
            // do 'multiple selectors' i.e. acLink AND href=/AccountList

            string amounts,
                    date = "",
                    sequence_nos,
                    description = "",
                    balances,
                    transaction_type = "";
            bool isPaidIn = false;
            int amount = 0,
                balance = 0;
            DateTime booking_date = SmartParametersV2016.defaultDate;

            // I'm desperate to pass this back ...
            financeviewmodel.transactions_count = 0;

            long sequence_no = SmartFinanceV2021.SequenceNo();

            // These transactions come out in ASCENDING date order (I think!)
            try
            {
                dynamic jsonResponse = JObject.Parse(responseData);
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
                                            // Debits are returned as NEGATIVES!!
                                            // So we display all amounts as Positive
                                            // (NW shows Negative amounts as (positive) values)
                                            amounts = individual.Value;
                                            int sep1 = amounts.IndexOf(SmartParametersV2016.defaultCurrencySeparator);
                                            if (sep1 == -1)
                                            {
                                                // Add trailing pennies/cents when required (the Currency sep isn't really required..)
                                                amounts = amounts + SmartParametersV2016.defaultCurrencySeparator.ToString() + "00";
                                            }
                                            else
                                            {
                                                // Add trailing 0 when required
                                                if (sep1 + 2 == amounts.Length)
                                                {
                                                    amounts += "0";
                                                }
                                            }
                                            amount = Convert.ToInt32(amounts.Replace(SmartParametersV2016.defaultCurrencySeparator.ToString(), string.Empty));
                                            break;
                                        case "Date":
                                            try
                                            {
                                                date = individual.Value.ToString();
                                                booking_date = SmartRoutinesV2018.DateTimeParse(date).ToLocalTime();
                                            }
                                            catch (System.Exception ex)
                                            {
                                                await SmartBobV2017.ListenerAsync(ourviewmodel, financeviewmodel.financeToken, institution_code, brand_code, "NW date", ex.Message + ": " + date);
                                                return false;
                                            }
                                            break;
                                        case "Description":
                                            description = individual.Value;
                                            break;
                                        case "IsPaidIn":
                                            isPaidIn = Convert.ToBoolean(individual.Value);
                                            break;
                                        case "RunningBalance":
                                            balances = individual.Value;
                                            int sep2 = balances.IndexOf(SmartParametersV2016.defaultCurrencySeparator);
                                            if (sep2 == -1)
                                            {
                                                // Add trailing pennies/cents when required (the Currency separator isn't really necessary here
                                                balances = balances + SmartParametersV2016.defaultCurrencySeparator.ToString() + "00";
                                            }
                                            else
                                            {
                                                // Add trailing 0 when required
                                                if (sep2 + 2 == balances.Length)
                                                {
                                                    balances += "0";
                                                }
                                            }
                                            balance = Convert.ToInt32(balances.Replace(SmartParametersV2016.defaultCurrencySeparator.ToString(), string.Empty));
                                            break;
                                        case "Sequence":        // Ignored
                                            sequence_nos = individual.Value;
                                            break;
                                        case "TransactionType":
                                            transaction_type = individual.Value;
                                            break;
                                        default:
                                            break;
                                    }
                                }

                                short[] transCodes = SmartSpikeFinanceV2017.Finance_Lookup_TransactionCode(financeviewmodel,
                                                                                            transactionTypesFound,
                                                                                            isPaidIn,
                                                                                            transaction_type,
                                                                                            false,
                                                                                            institution_code,
                                                                                            brand_code);
                                if (transCodes.Length != 2)
                                {
                                    await SmartBobV2017.ListenerAsync(ourviewmodel, financeviewmodel.financeToken, institution_code, brand_code, "NW Trans Lookup", "Trans type  failed: " + transaction_type);
                                    //return false;
                                }
                                else
                                {
                                    if (transCodes[0] == 0 || transCodes[1] == 0)
                                    {
                                        await SmartBobV2017.ListenerAsync(ourviewmodel, financeviewmodel.financeToken, institution_code, brand_code, "NW Trans Lookup", "Trans type  failed: " + transaction_type);
                                        //return false;
                                    }

                                }

                                // Have we already got this one? (Ignore sequence_no)
                                List<SmartFinance.BankTransactions>
                                    bt_found = SmartSpikeFinanceV2017.Finance_Lookup_BankTransaction(financeviewmodel,
                                    ourviewmodel.UserName,
                                    financeviewmodel.cubeface_code,
                                    institution_code,
                                    brand_code,
                                    sortcode,
                                    account_no,
                                    UDPRN,
                                    booking_date,
                                    isPaidIn,
                                    transCodes,
                                    description,
                                    amount,
                                    currencyOrdinal,
                                    balance);
                                // Only add it in if we can't find it
                                if (bt_found.Count == 0)
                                {
                                    // Still want to add it in even if we can't analyze the transaction ...
                                    int random1 = SmartRoutinesV2018.Get_Next_Random(ourviewmodel.randomR);
                                    int random2 = SmartRoutinesV2018.Get_Next_Random(ourviewmodel.randomR);
                                    // Use common template
                                    SmartFinance.BankTransactions bankTransaction =
                                        SmartFinanceV2021.Transaction_Template(ourviewmodel,
                                                                                financeviewmodel,
                                                                                institution_code,
                                                                                brand_code,
                                                                                sortcode,
                                                                                account_no,
                                                                                UDPRN,
                                                                                random1,
                                                                                booking_date,
                                                                                sequence_no,
                                                                                isPaidIn,
                                                                                transCodes,
                                                                                description,
                                                                                amount,
                                                                                currencyOrdinal,
                                                                                SmartParametersV2016.totalBalance,
                                                                                balance,
                                                                                currencyOrdinal,
                                                                                balance >= 0 ? true : false,
                                                                                random2,
                                                                                false);
                                    financeviewmodel.PLO.bank_transactions_changesList.Add(bankTransaction);
                                    financeviewmodel.transactions_count++;
                                    sequence_no++;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                await SmartRoutinesV2018.CheckTrace(ourviewmodel, financeviewmodel.financeToken, institution_code, brand_code, ex.Message + " " + "General failure");
                return false;
            }
            return true;
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
                                                                        //Action<string> set_error_message,
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

            DateTime transaction_created = DateTime.Now.Add(ourviewmodel.utcOffset);   // UTC time

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
                                List<SmartFinance.Transaction_Types> transaction_types_found = new List<SmartFinance.Transaction_Types> ();
                                short[] transCodes = SmartSpikeFinanceV2017.Finance_Lookup_TransactionCode(financeviewmodel,
                                                                                            transaction_types_found,
                                                                                            is_paid_in,
                                                                                            transaction_type,
                                                                                            false,
                                                                                            institution_code,
                                                                                            brand_code);
                                if (transCodes.Length != 2)
                                {
                                    await SmartBobV2017.ListenerAsync(ourviewmodel, financeviewmodel.financeToken, institution_code, brand_code, "NW Trans Lookup", "Trans type  failed: " + transaction_type);
                                    //return false;
                                }
                                else
                                {
                                    if (transCodes[0] == 0 || transCodes[1] == 0)
                                    {
                                        await SmartBobV2017.ListenerAsync(ourviewmodel, financeviewmodel.financeToken, institution_code, brand_code, "NW Trans Lookup", "Trans type  failed: " + transaction_type);
                                        //return false;
                                    }

                                }
                                //if (!SmartSpikeFinanceV2017.FinanceLookupTransactionSubCode(ourviewmodel, 
                                //                                                        financeviewmodel,
                                //                                                                            is_paid_in,
                                //                                                                            ref transaction_type,
                                //                                                                            ref transaction_code,
                                //                                                                            ref transaction_sub_code,
                                //                                                                            false,
                                //                                                                            institution_code,
                                //                                                                            brand_code))
                                //{
                                //    await SmartRoutinesV2018.CheckTrace(ourviewmodel, financeviewmodel.financeToken, institution_code, brand_code, "Trans type lookup failed: " + transaction_type);
                                //}

                                // Still want to add it in even if we can't analyze the transaction ...
                                int random1 = SmartRoutinesV2018.Get_Next_Random(ourviewmodel.randomR);
                                int random2 = SmartRoutinesV2018.Get_Next_Random(ourviewmodel.randomR);

                                SmartFinance.BankTransactions abc = new SmartFinance.BankTransactions()
                                {
                                    USERNAME = ourviewmodel.UserName,
                                    CUBEFACE_CODE = financeviewmodel.cubeface_code,
                                    INSTITUTION_CODE = institution_code,
                                    BRAND_CODE = brand_code,
                                    SORTCODE = sort_code,
                                    ACCOUNT_NO = account_no,
                                    UDPRN = string.Empty,
                                    RANDOMKEY1 = random1,
                                    BOOKING_DATE = converted_date,
                                    SEQUENCE_NO = new DateTimeOffset(DateTime.Now + ourviewmodel.utcOffset).ToUnixTimeMilliseconds(), // Unique AND increasing
                                    CREDITDEBIT_INDICATOR = is_paid_in,
                                    PTC_CODE = 0,
                                    TRANSGROUP_CODE = transaction_sub_code,
                                    TRANSACTION_CODE = transaction_code,
                                    DESCRIPTION = description,
                                    AMOUNT = Convert.ToInt32(amount.Replace(".", string.Empty)),
                                    CURRENCY_ORDINAL = 0,//currency,     // Pull this in from the Account
                                    BALANCE_TYPE = false,
                                    BALANCE_AMOUNT = Convert.ToInt32(balance.Replace(".", string.Empty)),
                                    BALANCE_CURRENCY_ORDINAL = 0,
                                    BALANCE_CREDITDEBIT_INDICATOR = false,
                                    RANDOMKEY2 = random2,
                                    EXCHANGE_RATES = new decimal[3],
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
                financeviewmodel.errorMessage = ex.Message;
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
        //                                                                IPage webPage,
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
        //                IReadOnlyList<IElementHandle> full_container = await financeviewmodel.webPage.QuerySelectorAllAsync("div[id^='full-statement-container']");
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
        //                string account_value = sort_code + SmartParametersV2016.unitSeparator +
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

#if WINFORMS || WPF || WINDOWS_UWP
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

        internal static async Task<bool> NationwideFindPasscodes(MainViewModel ourviewmodel,
                                                                FinanceViewModel financeviewmodel,
                                                                short institution_code,
                                                                short brand_code,
                                                                SmartFinance.Logins login_info)
        {
            bool status = false;

            IReadOnlyList<IElementHandle> passcodes = await financeviewmodel.webPage.QuerySelectorAllAsync("span[class='control__label__title']");
            //await financeviewmodel.webPage.WaitForLoadStateAsync(LoadState.DOMContentLoaded);
            if (passcodes == null)
            {
                financeviewmodel.errorMessage = "Couldn't find 'passcodes' span class";
                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, " Passcodes: not found");

                return false;
            }

            //await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, " Passcodes: found");
            //await SmartRoutinesV2018.CheckTrace(ourviewmodel, financeviewmodel.financeToken, institution_code, brand_code, "Passcodes: found");
            foreach (IElementHandle each_passcode in passcodes)
            {
                string text = await each_passcode.TextContentAsync();
                if (text.Contains("digits from your passnumber"))
                {
                    //Console.WriteLine(text.ToString());

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
                    IReadOnlyList<string> firstpassnumber = await financeviewmodel.webPage.SelectOptionAsync("select[name='FirstPassnumberValue']", login_info.PASSCODE.Substring(zzz[0], 1));
                    //await financeviewmodel.webPage.WaitForLoadStateAsync(LoadState.DOMContentLoaded);
                    if (firstpassnumber == null)
                    {
                        financeviewmodel.errorMessage = "Couldn't find 'First pass number' drop-down input";
                        return false;
                    }

                    IReadOnlyList<string> secondpassnumber = await financeviewmodel.webPage.SelectOptionAsync("select[name='SecondPassnumberValue']", login_info.PASSCODE.Substring(zzz[1], 1));
                    //await financeviewmodel.webPage.WaitForLoadStateAsync(LoadState.DOMContentLoaded);
                    if (secondpassnumber == null)
                    {
                        financeviewmodel.errorMessage = "Couldn't find 'Second pass number' drop-down input";
                        return false;
                    }

                    IReadOnlyList<string> thirdpassnumber = await financeviewmodel.webPage.SelectOptionAsync("select[name='ThirdPassnumberValue']", login_info.PASSCODE.Substring(zzz[2], 1));
                    if (thirdpassnumber == null)
                    {
                        financeviewmodel.errorMessage = "Couldn't find 'Third pass number' drop-down input";
                        return false;
                    }
                    await financeviewmodel.webPage.WaitForLoadStateAsync(LoadState.DOMContentLoaded);

                    await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, " Passcodes: set");
                    await SmartRoutinesV2018.CheckTrace(ourviewmodel, financeviewmodel.financeToken, institution_code, brand_code, "Passcodes: set");
                    status = true;
                    break;
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