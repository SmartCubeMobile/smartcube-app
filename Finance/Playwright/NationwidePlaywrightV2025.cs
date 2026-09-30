
using System.Text.RegularExpressions;
using System.Windows;
using Microsoft.Playwright;

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
using Android.Content;
using Android.Provider;
#endif


namespace SmartCubeMobile
{
    internal class NationwidePlaywrightV2025
    {
        internal static async Task<bool> RunNationwidePlaywright(
#if WINFORMS
                                                    MainProcess components,
                                                    RichTextBox textBoxConsole,
#endif
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
            var playwright = await Playwright.CreateAsync();
            IBrowser browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
            {
                Headless = true
            });


            financeviewmodel.notificationHandledSource = new TaskCompletionSource<bool>();

            IBrowserContext context = await browser.NewContextAsync();
            //var context = await browser.NewContextAsync(new BrowserNewContextOptions
            //{
            //    ViewportSize = new ViewportSize { Width = 1280, Height = 720 },
            //    UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 Chrome/114.0 Safari/537.36"
            //});
            IPage page = await context.NewPageAsync();

            await page.EvaluateAsync(@"() => 
            {
                    document.body.style.zoom = '0.75';
            }");
            // Extract DOB parts
            string Day = Parameter2.Substring(0, 2);
            string Month = SmartParametersV2016.months[Convert.ToInt16(Parameter2.Substring(2, 2)) - 1];
            string Year = Parameter2.Substring(4, 4);

            try
            {
                await page.GotoAsync(startUrl);
#if WPF || UWP || WINUI
                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Retrieved: " + startUrl);
#endif

                var loginLink = await page.QuerySelectorAsync("[class^='LoginLinks__LoginLink']");
                var loginHref = await loginLink.GetAttributeAsync("href");

                await page.GotoAsync(loginHref);

#if WPF || UWP || WINUI
                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Navigated to: " + loginHref);

#endif
                Uri uri = new Uri(page.Url);
                var baseUrl = @"https://" + uri.Authority;

#if WPF || UWP || WINUI
                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Base URL: " + baseUrl);
#endif
                if (!await NationwideFindInputs(ourviewmodel,
                                            financeviewmodel,
                                            page,
                                            Parameter1,
                                            Day,
                                            Year))
                {
#if WPF || UWP || WINUI
                    await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Couldn't fill main inputs");
#endif
                    return false;
                }
#if WPF || UWP || WINUI
                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Filled: Customer, Day and Year");
#endif
                try
                {
                    await page.SelectOptionAsync("select[name='DateOfBirthMonth']", new[] { Month });
                }
                catch (Exception ex)
                {
                    await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Filled: Dropdown " + ex.Message);
                    return false;
                }
#if WPF || UWP || WINUI
                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Filled: Dropdown " + Month);
#endif
                // Click first "Continue" button (which is 2nd in the list)

                // Get all matching buttons
                var buttons = await page.QuerySelectorAllAsync("button:has-text(\"Continue\")");

                if (buttons.Count >= 2)
                {
                    // Use JavaScript to click the second one
                    await buttons[1].EvaluateAsync("el => el.click()");
                    //    await page.WaitForTimeoutAsync(3000); // Wait 0.5s


                }
                else
                {
                    System.Console.WriteLine("Less than 2 'Continue' buttons found.");
                }


#if WPF || UWP || WINUI
                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Continue(1) button clicked");
#endif

                // Select radio button
                var selector = "input[type='radio'][value='PassNumberAndSMSOTP']";
                await page.WaitForSelectorAsync(selector, new PageWaitForSelectorOptions
                {
                    State = WaitForSelectorState.Visible
                });
                var radio = await page.QuerySelectorAsync(selector);
                if (radio != null)
                {
                    await radio.ScrollIntoViewIfNeededAsync();
                    await radio.EvaluateAsync("el => el.click()");

                    //await radio.CheckAsync(new() { Force = true });
#if WPF || UWP || WINUI
                    await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Radio button clicked");
#endif
                }
                else
                {
#if WPF || UWP || WINUI
                    await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Radio button cannot be clicked");
#endif
                    return false;
                }

                // Passcode fill logic placeholder
                if (!await NationwideFindPasscodes(
#if WINFORMS
                                                    components,
                                                    textBoxConsole,
#endif
                                                    ourviewmodel,
                                                    financeviewmodel,
                                                    page,
                                                    Parameter3))
                {
#if WPF || UWP || WINUI
                    await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Problem with Passcodes: ");
#endif
                    return false;
                }
#if WPF || UWP || WINUI
                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Passcodes: Filled");
#endif

                // Final "Continue" click
                var finalContinue = page.Locator("button", new PageLocatorOptions { HasTextString = "Continue" }).Nth(1);
                if (finalContinue != null)
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
                            string Text = $"{k}: {(reason == MainViewModel.TimerFinishReason.Completed ? "Finished" : "Cancelled")}";
                            if (Text.Contains("Finished"))
                            {
#if WPF || UWP || WINUI
                                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Completed: " + Text);
#endif
                                // Shutdown - we timed out without an SMSOTP
                                if (await WebDriverDead(ourviewmodel, financeviewmodel, page))
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
                                                                                                                                context,
                                                                                                                                page,
                                                                                                                                baseUrl,
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
                    // I will forever be in the debt of BART WOJTALA whose solution at
                    // https://stackoverflow.com/questions/11908249/debugging-element-is-not-clickable-at-point-error
                    // saved my ass, my bacon and my sanity to allow me
                    // to 'click' through this last button and get this entire
                    // 15-year project back on track.  Thanks a million, Bart
                    //

                    // And press the Button to say 'Send me an OTP' please

                    //await finalContinue.ClickAsync(new() { Force = true });
                    await finalContinue.EvaluateAsync("el => el.click()");
#if WPF || UWP || WINUI
                    await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Continue(2) button clicked");
#endif

                    // Wait for something indicating successful login / trigger
#if WPF || UWP || WINUI
                    await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Waiting for a notification...");
#endif
                    // Keep the User informed
#if WPF || UWP || WINUI
                    await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Returning...");
#endif
                }
                else
                {
#if WPF || UWP || WINUI
                    await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Continue(2) button not clicked");
#endif
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Problem: " + ex.Message);
#if WPF || UWP || WINUI
                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, ex.Message);
#endif
                financeviewmodel.errorMessage = ex.Message;
                return false;
            }
            return true;
        }

#if !ANDROID
        internal static async void ListenerNotificationChanged(

            UserNotificationListener sender, UserNotificationChangedEventArgs args,

#if WINFORMS
                                                                    MainProcess components, 
                                                                    RichTextBox textBoxConsole,                                                                    
#endif
                                                                    MainViewModel ourviewmodel,
                                                                    FinanceViewModel financeviewmodel,
                                                                    IBrowserContext context,
                                                                    IPage page,
                                                                    string baseUrl,
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
                bodyText = bodyText.Replace(notificationPrefix, "").Trim(); //Trim gets rid of any spaces
                if (bodyText.Length <= 6)
                {
                    continue;
                }
                string notificationContent = bodyText.Substring(0, notificationTagLength);

                string oneTimePasscode = notificationContent;
#if WPF || UWP || WINUI
                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "OneTimePasscode: " + oneTimePasscode);
#endif
                // Process Accounts
                if (!await Part2(
#if WINFORMS
                                    components,
                                    textBoxConsole,
#endif
                                    ourviewmodel,
                                    financeviewmodel,
                                    context,
                                    page,
                                    baseUrl,
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
                                    page))
                    {
                        return;
                    }
                    if (await WebDriverDead(ourviewmodel, financeviewmodel, page))
                    {
                        break;
                    }
                }
                break;
            }
            // Mark the task as complete so the calling method continues
            //financeviewmodel.notificationHandledSource.TrySetResult(true);
            return;
        }
#endif

        internal static async Task<bool> WebDriverDead(MainViewModel ourviewmodel,
                                                FinanceViewModel financeviewmodel,
                                                IPage page)
        {
            await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "WebDriver: " + "Quit completed");
            return true;
        }

        internal static async Task<bool> Part2(
#if WINFORMS
                                            MainProcess components,
                                            RichTextBox textBoxConsole,
#endif
                                            MainViewModel ourviewmodel,
                                            FinanceViewModel financeviewmodel,
                                            IBrowserContext context,
                                            IPage page,
                                            string baseUrl,
                                            SmartFinance.Logins loginInfo,
                                            string oneTimePasscode,
                                            short institutionCode,
                                            short brandCode,
                                            List<SmartFinance.Transaction_Groups> transaction_groupsFound,
                                            List<SmartFinance.Transaction_Types> transaction_typesFound)
        {
            if (string.IsNullOrEmpty(oneTimePasscode))
            {
#if WPF || UWP || WINUI
                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Notification content: Empty");
#endif
                financeviewmodel.errorMessage = "Notification content is empty";
                return false;
            }

            try
            {
                await page.FillAsync("input[name='OneTimePasscode']", oneTimePasscode);
#if WPF || UWP || WINUI
                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, $"Input Field: {oneTimePasscode} filled");
#endif

                financeviewmodel.loggedIn = false;

                var buttons = await page.QuerySelectorAllAsync("button");
                if (buttons == null || buttons.Count == 0)
                {
#if WPF || UWP || WINUI
                    await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "No buttons found");
#endif
                    financeviewmodel.errorMessage = "Couldn't find any buttons";
                    return false;
                }

                IElementHandle loginButton = null;
                foreach (var button in buttons)
                {
                    var buttonText = await button.InnerTextAsync();
                    if (buttonText.Trim() == "Log in")
                    {
                        loginButton = button;
                        break;
                    }
                }
                if (loginButton == null)
                {
#if WPF || UWP || WINUI
                    await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Can't find Login button");
#endif
                    return false;
                }

                //await loginButton.ClickAsync();
                await loginButton.EvaluateAsync("el => el.click()");
#if WPF || UWP || WINUI
                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Log in button: Clicked");
#endif

                financeviewmodel.loggedIn = true;
                financeviewmodel.accountItems = new List<FinanceViewModel.AccountItem>();
                financeviewmodel.currentUrl = page.Url;

                string owner = "";
                financeviewmodel.UDPRN = "";

                var selector = "#welcome-message";
                await page.WaitForSelectorAsync(selector, new PageWaitForSelectorOptions
                {
                    State = WaitForSelectorState.Visible
                });
                var welcome = await page.QuerySelectorAsync(selector);
                if (welcome != null)
                {
                    var welcomeText = await welcome.InnerTextAsync();
                    if (!string.IsNullOrEmpty(welcomeText))
                    {
                        owner = welcomeText.Replace("Welcome back,", "").Trim();
#if WPF || UWP || WINUI
                        await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Welcome back: " + owner);
#endif
                    }
                }

                bool foundAddress = await FindAddressAsync(ourviewmodel,
                                                            financeviewmodel,
                                                            context,
                                                            page,
                                                            baseUrl,
                                                            loginInfo,
                                                            owner);
                if (!foundAddress)
                {
#if WPF || UWP || WINUI
                    await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Find address: Failed");
#endif
                    return false;
                }
                else
                {
#if WPF || UWP || WINUI
                    await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Owner: " + loginInfo.OWNER);
                    await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Contact no.: " + loginInfo.CONTACT_PHONENO);
                    await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Email address: " + loginInfo.CONTACT_EMAIL);
#endif
                }

                financeviewmodel.account_id = 0;
                financeviewmodel.AccountLinks = new List<KeyValuePair<string, string>>();

                bool builtAccounts = await BuildAccountListAsync(
#if WINFORMS
                                                                components,
                                                                textBoxConsole,
#endif
                                                                ourviewmodel,
                                                                financeviewmodel,
                                                                page,
                                                                context,
                                                                baseUrl,
                                                                institutionCode,
                                                                brandCode,
                                                                financeviewmodel.UDPRN,
                                                                transaction_groupsFound,
                                                                transaction_typesFound);
                if (!builtAccounts)
                {
#if WPF || UWP || WINUI
                    await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, financeviewmodel.errorMessage);
#endif
                    return false;
                }
#if WPF || UWP || WINUI
                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Transactions inserted: " + financeviewmodel.transactions_count);
#endif
                return true;
            }
            catch (Exception ex)
            {
#if WPF || UWP || WINUI
                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, ex.Message);
#endif
                financeviewmodel.errorMessage = ex.Message;
                return false;
            }
        }

        // Placeholder for your existing logic
        private static async Task<bool> FindAddressAsync(MainViewModel ourviewmodel,
                                                        FinanceViewModel financeviewmodel,
                                                        IBrowserContext context,
                                                        IPage page,
                                                        string baseUrl,
                                                        SmartFinance.Logins loginInfo,
                                                        string owner)
        {
            financeviewmodel.UDPRN = ""; // Everything depends on finding this!

            try
            {
                // A kludge ... but life's too short ...
                string href = @"CustomerIB/MaintainTelephoneAndAddress";
                var absoluteUrl = new Uri(new Uri(baseUrl), href).ToString();
                var page2 = await context.NewPageAsync();
                await page2.GotoAsync(absoluteUrl);
                financeviewmodel.UDPRN = await FindDetailsAsync(page2,
                                                                ourviewmodel,
                                                                financeviewmodel,
                                                                loginInfo,
                                                                owner);
            }
            catch (Exception ex)
            {
#if WPF || UWP || WINUI
                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Can't navigate: " + ex.Message);
#endif
                financeviewmodel.errorMessage = "Can't navigate to address page: " + ex.Message;
                return false;
            }

            if (string.IsNullOrEmpty(financeviewmodel.UDPRN))
            {
#if WPF || UWP || WINUI
                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "UDPRN: <empty>");
#endif
                financeviewmodel.errorMessage = "UDPRN is empty";
                return false;
            }
#if WPF || UWP || WINUI
            await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "UDPRN good: " + financeviewmodel.UDPRN);
#endif
            return true;
        }

        internal static async Task<string> FindDetailsAsync(IPage page2,
                                                            MainViewModel ourviewmodel,
                                                            FinanceViewModel financeviewmodel,
                                                            SmartFinance.Logins loginInfo,
                                                            string owner)
        {
            string udprn = "";
            string bankAddress = "";

            try
            {
                // Step 1 - Find <address> tag
                var addressElements = await page2.QuerySelectorAllAsync("address");
                foreach (var addressElement in addressElements)
                {
                    string rawText = await addressElement.InnerTextAsync();
                    if (!string.IsNullOrEmpty(rawText))
                    {
                        bankAddress = rawText.Replace(SmartParametersV2016.newline, SmartParametersV2016.spaceSplit).Trim();
                        bankAddress = Regex.Replace(bankAddress, @"\s{2,}", " ");
                        break;
                    }
                }

                if (string.IsNullOrWhiteSpace(bankAddress))
                {
#if WPF || UWP || WINUI
                    await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Bank address empty - cannot continue");
#endif
                    financeviewmodel.errorMessage = "Bank address empty - cannot continue";
                    return udprn;
                }

                // Step 2 - Check for existing address
                var addressesFound = SmartSpikeV2017.UsersLookupTextAddress(ourviewmodel, bankAddress);
                if (addressesFound.Any())
                {
                    if (string.IsNullOrEmpty(addressesFound.First().UDPRN))
                    {
#if WPF || UWP || WINUI
                        await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Address record has no UDPRN - cannot continue");
#endif
                        financeviewmodel.errorMessage = "Address record has no UDPRN - cannot continue";
                        return udprn;
                    }

                    udprn = addressesFound.First().UDPRN;
                }
                else
                {
                    // Step 5 - Use Ideal Postcodes lookup
                    var idealAddress = await SmartRoutinesV2018.UsersUDPRNLookup(ourviewmodel, bankAddress);
                    if (string.IsNullOrEmpty(idealAddress.UDPRN))
                    {
#if WPF || UWP || WINUI
                        await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Cannot determine UDPRN - cannot continue");
#endif
                        financeviewmodel.errorMessage = "Cannot determine UDPRN - cannot continue";
                        return udprn;
                    }

                    udprn = idealAddress.UDPRN;

                    // Determine Area Code
                    var postcodes = SmartSpikeV2017.Find_Working_Postcodes(ourviewmodel.Blanche.workingPostcodesList, idealAddress.POSTCODE_OUTWARD);
                    short areaCode = postcodes.FirstOrDefault()?.AREA_CODE ?? 0;

                    var newAddress = new SmartUsers.AddressesView
                    {
                        USERNAME = ourviewmodel.UserName,
                        UDPRN = idealAddress.UDPRN,
                        RANDOM_KEY1 = SmartRoutinesV2018.GetNextRandomNew(ourviewmodel.randomR),
                        ADDRESS_CREATED = DateTime.Now,
                        BASIC = bankAddress,
                        BUILDING_NAME = idealAddress.BUILDING_NAME,
                        BUILDING_NUMBER = idealAddress.BUILDING_NUMBER,
                        COUNTY = idealAddress.COUNTY,
                        DOUBLE_DEPENDANT_LOCALITY = idealAddress.DOUBLE_DEPENDANT_LOCALITY,
                        DEPENDANT_THOROUGHFARE = idealAddress.DEPENDANT_THOROUGHFARE,
                        DEPENDANT_LOCALITY = idealAddress.DEPENDANT_LOCALITY,
                        ORGANIZATION = idealAddress.ORGANIZATION,
                        POSTCODE_OUTWARD = idealAddress.POSTCODE_OUTWARD,
                        POSTCODE = idealAddress.POSTCODE,
                        POBOX = idealAddress.POBOX,
                        SUB_BUILDING_NAME = idealAddress.SUB_BUILDING_NAME,
                        THOROUGHFARE = idealAddress.THOROUGHFARE,
                        TOWN = idealAddress.TOWN,
                        RANDOM_KEY2 = SmartRoutinesV2018.GetNextRandomNew(ourviewmodel.randomR),
                        Updated = false
                    };

                    ourviewmodel.Hamas.addressesview_changesList.Add(newAddress);
                }

                // Step 6 - Optional fields: Email and Mobile
                var emailElement = await page2.QuerySelectorAsync("#CurrentEmailAddress");
                if (emailElement != null)
                {
                    string emailVal = await emailElement.GetAttributeAsync("value");
                    if (loginInfo.CONTACT_EMAIL != emailVal)
                    {
                        loginInfo.CONTACT_EMAIL = emailVal;
                        loginInfo.Updated = true;
                    }
                }

                var mobileElement = await page2.QuerySelectorAsync("#MobilePhoneNumber");
                if (mobileElement != null)
                {
                    string phoneVal = await mobileElement.GetAttributeAsync("value");
                    if (loginInfo.CONTACT_PHONENO != phoneVal)
                    {
                        loginInfo.CONTACT_PHONENO = phoneVal;
                        loginInfo.Updated = true;
                    }
                }
            }
            catch (Exception ex)
            {
#if WPF || UWP || WINUI
                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, ex.Message);
#endif
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


        private static async Task<bool> BuildAccountListAsync(
#if WINFORMS
                                                            MainProcess components,
                                                            RichTextBox textBoxConsole,
#endif
                                                            MainViewModel ourviewmodel,
                                                            FinanceViewModel financeviewmodel,
                                                            IPage page,
                                                            IBrowserContext context,
                                                            string baseUrl,
                                                            short institutionCode,
                                                            short brandCode,
                                                            string UDPRN,
                                                            List<SmartFinance.Transaction_Groups> transactionGroupsFound,
                                                            List<SmartFinance.Transaction_Types> transactionTypesFound)
        {
            // Implement actual logic here
            string accountInfo1 = "";
            string accountInfo2;
            string accountName = "";
            string balance = "";
            string symbol = "";
            char categoryCode = SmartParametersV2016.defaultChar;

            try
            {
                var offs = await page.QuerySelectorAsync("[class='active']");
                if (offs != null)
                {
                    var links = await offs.QuerySelectorAllAsync("a");
                    foreach (var link in links)
                    {
                        string href = await link.GetAttributeAsync(SmartParametersV2016.href);
                        if (href != null &&
                            href.Contains("/AccountList/Account/RedirectToDefaultPage"))
                        {
                            string innerHtml = await link.InnerHTMLAsync();
                            if (!string.IsNullOrEmpty(innerHtml))
                            {
                                short accountOrdinal = 0;
                                categoryCode = SmartParametersV2016.defaultChar;

                                NewAccountBalance(financeviewmodel, innerHtml, ref accountInfo1, ref balance, ref symbol);
                                if (string.IsNullOrEmpty(accountInfo1))
                                {
#if WPF || UWP || WINUI
                                    await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Couldn't parse account info: " + accountInfo1);
#endif
                                    financeviewmodel.errorMessage = "Couldn't parse account info: " + accountInfo1;
                                    return false;
                                }

                                accountInfo2 = SplitAccountName(financeviewmodel, accountInfo1, ref accountName);
                                if (string.IsNullOrEmpty(accountName))
                                {
#if WPF || UWP || WINUI
                                    await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Account name empty");
#endif
                                    financeviewmodel.errorMessage = "Account name empty";
                                    return false;
                                }

                                if (string.IsNullOrEmpty(accountInfo2))
                                {
#if WPF || UWP || WINUI
                                    await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Account sortcode/account empty");
#endif
                                    financeviewmodel.errorMessage = "Account sortcode/account empty";
                                    return false;
                                }

                                short currencyOrdinal = SmartSpikeV2017.GetCurrencyOrdinal(ourviewmodel, symbol);
                                categoryCode = SmartParametersV2016.Banks;

                                string sortcode = "", account_no = "";
                                if (accountName.Contains("Savings", StringComparison.OrdinalIgnoreCase))
                                {
                                    categoryCode = SmartParametersV2016.Savings;
                                }

                                var accstuff = accountInfo2.Split(SmartParametersV2016.spacechar);
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
                                                                    currencyOrdinal,
                                                                    institutionCode,
                                                                    brandCode,
                                                                    UDPRN,
                                                                    categoryCode);

                                if (!decoded)
                                {
                                    return false;
                                }
                                bool processed = await ProcessAccount(
#if WINFORMS
                                                                    components, 
                                                                    textBoxConsole,
#endif
                                                                    ourviewmodel,
                                                                    financeviewmodel,
                                                                    page,
                                                                    context,
                                                                    baseUrl,
                                                                    categoryCode,
                                                                    href,
                                                                    institutionCode,
                                                                    brandCode,
                                                                    sortcode,
                                                                    account_no,
                                                                    financeviewmodel.UDPRN,
                                                                    accountOrdinal,
                                                                    symbol,
                                                                    transactionGroupsFound,
                                                                    transactionTypesFound);

                                if (!processed)
                                {
#if WPF || UWP || WINUI
                                    await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Process Account: " + financeviewmodel.errorMessage);
#endif
                                }
                                financeviewmodel.foundActiveX = true;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
#if WPF || UWP || WINUI
                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, ex.Message);
#endif
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

        internal static async Task<bool> ProcessAccount(
#if WINFORMS
                                                        MainProcess components,
                                                        RichTextBox textBoxConsole,
#endif
                                                        MainViewModel ourviewmodel,
                                                        FinanceViewModel financeviewmodel,
                                                        IPage page, // changed from WebDriver to Playwright IPage
                                                        IBrowserContext context,
                                                        string baseUrl,
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
                href = href.Replace("/AccountList/Account/RedirectToDefaultPage", "/Transactions/FullStatement/FullStatement");

                var absoluteUrl = new Uri(new Uri(baseUrl), href).ToString();
                var page3 = await context.NewPageAsync();
                await page3.GotoAsync(absoluteUrl);

                var classAnchor = await page3.QuerySelectorAsync("[id='last-12-month-btn']");
                if (classAnchor == null)
                {
#if WPF || UWP || WINUI
                    await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Class anchor: is null");
#endif
                    return false;
                }
                else
                {
                    await classAnchor.EvaluateAsync("el => el.click()");

                    var tbody = await page3.QuerySelectorAsync("tbody");
                    if (tbody == null)
                    {
#if WPF || UWP || WINUI
                        await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Table body not found");
#endif
                        return false;
                    }

                    string responseData = await tbody.InnerTextAsync();
                    string[] transactions = responseData.Split(SmartParametersV2016.newline);

#if WPF || UWP || WINUI
                    await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Transactions found: " + transactions.Length);
#endif
                    if (transactions.Length > 0)
                    {
                        DateTime accountCreated = SmartParametersV2016.defaultDate;

                        var accounts_found = financeviewmodel.PLO.finance_accountsList
                            .Where(acc => acc.INSTITUTION_CODE == institution_code &&
                                          acc.BRAND_CODE == brand_code &&
                                          acc.SORTCODE == sortcode &&
                                          acc.ACCOUNT_NO == account_no &&
                                          acc.UDPRN == UDPRN)
                            .OrderByDescending(acc => acc.ACCOUNT_CREATED)
                            .ToList();

                        if (accounts_found.Count > 0)
                        {
                            accountCreated = accounts_found.First().ACCOUNT_CREATED;
                        }

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
                }
            }
            catch (Exception)
            {
#if WPF || UWP || WINUI
                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "No Last 12 months button");
#endif
                return false;
            }
            return true;
        }

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

        internal static async Task<bool> Part3(
#if WINFORMS
                                    MainProcess components,
                                    RichTextBox textBoxConsole,
#endif
                                    MainViewModel ourviewmodel,
                                    FinanceViewModel financeviewmodel,
                                    IPage page)
        {
            // Reset this - just to make sure we return to
            // "AccountsList" every time!!
            try
            {
                //webDriver.Url = financeviewmodel.currentUrl;

                // Because we are logged in,
                // No need to process any more buttons!
                IElementHandle logoutButton = await page.QuerySelectorAsync(".log-out-link");
                if (logoutButton == null)
                {
#if WPF || UWP || WINUI
                    await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "No buttons found");
#endif
                    financeviewmodel.errorMessage = "Couldn't find any buttons";
                    return false;
                }
                // Try and depart gracefully
                await logoutButton.ClickAsync(); // equivalent to JS click

#if WPF || UWP || WINUI
                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Logout button: " + "Clicked");
#endif
            }
            catch (Exception ex)
            {
                financeviewmodel.errorMessage = ex.Message;
#if WPF || UWP || WINUI
                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Logout button click: " + ex.Message);
#endif
                return false;
            }
            return true;
        }

        internal static async Task<bool> NationwideFindInputs(MainViewModel ourviewmodel,
                                                        FinanceViewModel financeviewmodel,
                                                        IPage page,
                                                        string CustomerNumber,
                                                        string Day,
                                                        string Year)
        {
            bool status = false;

            bool customernumber_found = false,
                dateofbirthday_found = false,
                dateofbirthyear_found = false;
            // Fill inputs (You should adjust selectors based on actual input names/ids)
            try
            {
                await page.FillAsync("input[name='CustomerNumber']", CustomerNumber);
                customernumber_found = true;
            }
            catch (Exception ex)
            {
                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Not filled: CustomerId " + ex.Message);
                return false;
            }
            try
            {
                await page.FillAsync("input[name='DateOfBirthDay']", Day);
                dateofbirthday_found = true;
            }
            catch (Exception ex)
            {
                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Not filled: DateOfBirthDay " + ex.Message);
                return false;
            }
            try
            {
                await page.FillAsync("input[name='DateOfBirthYear']", Year);
                dateofbirthyear_found = true;
            }
            catch (Exception ex)
            {
                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Not filled: DateOfBirthYear " + ex.Message);
                return false;
            }
            if (customernumber_found &&
                    dateofbirthday_found &&
                    dateofbirthyear_found)
            {
                status = true;
            }
            return status;
        }

        internal static async Task<bool> NationwideFindPasscodes(
#if WINFORMS
                                                                        MainProcess components,
                                                                        RichTextBox textBoxConsole,
#endif
                                                                        MainViewModel ourviewmodel,
                                                                        FinanceViewModel financeviewmodel,
                                                                        IPage page,
                                                                        string PASSCODE)
        {
            bool status = false;

            try
            {
                var passcodes = page.Locator("span.control__label__title");
                int count = await passcodes.CountAsync();
                if (count == 0)
                {
                    await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Couldn't find 'passcodes' span class");
                    return false;
                }

                for (int i = 0; i < count; i++)
                {
                    var eachPasscode = passcodes.Nth(i);
                    string text = await eachPasscode.InnerTextAsync();
                    if (text.Contains("digits from your passnumber"))
                    {
                        int index = 0;
                        int[] zzz = new int[3];

                        if (text.Contains("1st")) zzz[index++] = 0;
                        if (text.Contains("2nd")) zzz[index++] = 1;
                        if (text.Contains("3rd")) zzz[index++] = 2;
                        if (text.Contains("4th")) zzz[index++] = 3;
                        if (text.Contains("5th")) zzz[index++] = 4;
                        if (text.Contains("6th")) zzz[index++] = 5;

                        // Fill dropdowns
                        string[] dropdownNames = { "FirstPassnumberValue", "SecondPassnumberValue", "ThirdPassnumberValue" };

                        for (int j = 0; j < 3; j++)
                        {
                            var selector = $"select[name='{dropdownNames[j]}']";
                            var dropdown = page.Locator(selector);

                            if (await dropdown.CountAsync() == 0)
                            {
                                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, $"Couldn't find '{dropdownNames[j]}' drop-down input");
                                return false;
                            }
                            await dropdown.SelectOptionAsync(new SelectOptionValue { Label = PASSCODE.Substring(zzz[j], 1) });
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

#if WINFORMS || WPF || UWP || WINUI
        public static async void AddWaitTimer(MainViewModel ourviewmodel, FinanceViewModel financeviewmodel, string key, int seconds, Action<string> onTick, Action<string, MainViewModel.TimerFinishReason> onComplete)
        {
            if (IsRunning(financeviewmodel, key))
            {
#if WINFORMS
                System.Windows.Forms.MessageBox.Show($"Timer {key} is already running.");
#endif
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
#endif
    }
}