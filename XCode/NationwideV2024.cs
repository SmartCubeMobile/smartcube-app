using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using Newtonsoft.Json.Linq;
using System.Text.RegularExpressions;
using System.Linq;

#if WINFORMS
using SmartDashboard;
//using com.gargoylesoftware.htmlunit;
//using WebClient = com.gargoylesoftware.htmlunit.WebClient;
//using com.gargoylesoftware.htmlunit.html;
using Windows.UI.Notifications.Management;
using Windows.UI.Notifications;
//using HtmlElement = com.gargoylesoftware.htmlunit.html.HtmlElement;
//using HttpMethod = com.gargoylesoftware.htmlunit.HttpMethod;
#endif

#if WPF
//using com.gargoylesoftware.htmlunit;
//using WebClient = com.gargoylesoftware.htmlunit.WebClient;
//using com.gargoylesoftware.htmlunit.html;
using Windows.UI.Notifications.Management;
using Windows.UI.Notifications;
//using HtmlElement = com.gargoylesoftware.htmlunit.html.HtmlElement;
//using HttpMethod = com.gargoylesoftware.htmlunit.HttpMethod;
#endif

#if UWP
using com.gargoylesoftware.htmlunit;
using WebClient = com.gargoylesoftware.htmlunit.WebClient;
using com.gargoylesoftware.htmlunit.html;
using Windows.UI.Notifications.Management;
using Windows.UI.Notifications;
using HtmlElement = com.gargoylesoftware.htmlunit.html.HtmlElement;
using HttpMethod = com.gargoylesoftware.htmlunit.HttpMethod;
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

    public class NationwideV2024
    {
#if (WINFORMS || WPF || UWP) && NHTMLUNIT
        internal static bool NationwideFindInputs(
                                        string CustomerNumber,
                                        string Day,
                                        string Year,
                                        HtmlPage page)
        {
            bool status = false;

            bool customernumber_found = false,
                dateofbirthday_found = false,
                dateofbirthyear_found = false;

            DomNodeList domInputs = (DomNodeList)page.getElementsByTagName("input"); //as List<DomNode>;
            for (int i = 0; i < domInputs.getLength(); i++)
            {
                HtmlInput pageInput = (HtmlInput)domInputs.get(i);// as HtmlInput;
                if (pageInput != null)
                {
                    if (pageInput.getId() == "CustIdentDetails_CustomerNumber")
                    {
                        pageInput.setValueAttribute(CustomerNumber);

                    }
                    if (pageInput.getId().Contains("text-input-control"))
                    {
                        switch (pageInput.getNameAttribute())
                        {
                            case "CustomerNumber":
                                customernumber_found = true;
                                pageInput.setValueAttribute(CustomerNumber);
                                break;
                            case "DateOfBirthDay":
                                dateofbirthday_found = true;
                                pageInput.setValueAttribute(Day);
                                break;
                            case "DateOfBirthYear":
                                dateofbirthyear_found = true;
                                pageInput.setValueAttribute(Year);
                                break;
                            default:
                                break;
                        }
                    }
                }
                if (customernumber_found &&
                    dateofbirthday_found &&
                    dateofbirthyear_found)
                {
                    status = true;
                    break;
                }
            }
            return status;
        }

        internal static bool FillInputMonth(HtmlPage mainScreen, string Month)
        {
            bool dateofbirthmonth_found = false;
            DomNodeList domSelects = mainScreen.getElementsByTagName("select");//as List<DomNode>;
            for (int i = 0; i < domSelects.getLength(); i++)
            {
                HtmlSelect mainSelect = (HtmlSelect)domSelects.get(i);// as HtmlSelect;
                if (mainSelect != null)
                {
                    if (mainSelect.getId().Contains("selection-list-control"))
                    {
                        string name = mainSelect.getAttribute("Name");
                        switch (name)
                        {
                            case "DateOfBirthMonth":
                                dateofbirthmonth_found = true;
                                HtmlOption option = mainSelect.getOptionByValue(Month);
                                mainSelect.setSelectedAttribute(option, true);
                                break;
                            default:
                                break;
                        }
                    }
                }
                if (dateofbirthmonth_found)
                {
                    break;
                }
            }
            return dateofbirthmonth_found;
        }

        internal static bool FindPassNumbers(FinanceViewModel financeviewmodel,
                                            HtmlPage page)//, rf int[] zzz)
        {
            // Look for which pass numbers they want
            //int[] zzz = new int[3];   // So we have 0, 1 and 2
            DomNodeList domSpans = page.getElementsByTagName("span");
            for (int i = 0; i < domSpans.getLength(); i++)
            {
                HtmlSpan mainSpan = (HtmlSpan)domSpans.get(i);// as HtmlSpan;
                if (mainSpan != null)
                {
                    string classname = mainSpan.getAttribute("class");
                    if (classname == "control__label__title")
                    {
                        int index = 0;
                        string text = mainSpan.getTextContent();
                        if (text.Contains("digits from your passnumber"))
                        {
                            if (text.Contains("1st"))
                            {
                                financeviewmodel.zzz[index] = 1; // 0;
                                index++;
                            };
                            if (text.Contains("2nd"))
                            {
                                financeviewmodel.zzz[index] = 2; // 1;
                                index++;
                            };
                            if (text.Contains("3rd"))
                            {
                                financeviewmodel.zzz[index] = 3; // 2;
                                index++;
                            };
                            if (text.Contains("4th"))
                            {
                                financeviewmodel.zzz[index] = 4; // 3;
                                index++;
                            };
                            if (text.Contains("5th"))
                            {
                                financeviewmodel.zzz[index] = 5; // 4;
                                index++;
                            };
                            if (text.Contains("6th"))
                            {
                                financeviewmodel.zzz[index] = 6; // 5;
                                //index++; Redundant?
                            };
                            break;
                        }
                    }
                }
            }
            if (financeviewmodel.zzz[0] == 0 ||
                financeviewmodel.zzz[1] == 0 ||
                financeviewmodel.zzz[2] == 0)
            {
                return false;
            }
            return true;
        }

        internal static bool SetPassNumbers(HtmlPage page, string Passcode, int[] zzz)
        {
            // Why zzz[0] - 1? Because we record the first passcode
            // as 1 and not 0.  The -1 is to make the offset into
            // the passcode string 'right'
            bool firstpassnumber_found = false,
                secondpassnumber_found = false,
                thirdpassnumber_found = false;

            DomNodeList domSelects = page.getElementsByTagName("select");
            for (int i = 0; i < domSelects.getLength(); i++)
            {
                HtmlSelect mainSelect = (HtmlSelect)domSelects.get(i);// as HtmlSelect;
                if (mainSelect != null)
                {
                    if (mainSelect.getId().Contains("selection-list-control"))
                    {
                        string name = mainSelect.getAttribute("Name");
                        switch (name)
                        {
                            case "FirstPassnumberValue":
                                firstpassnumber_found = true;
                                HtmlOption option1 = mainSelect.getOptionByValue(Passcode.Substring(zzz[0] - 1, 1)); // CHANGE
                                mainSelect.setSelectedAttribute(option1, true);
                                break;
                            case "SecondPassnumberValue":
                                secondpassnumber_found = true;
                                HtmlOption option2 = mainSelect.getOptionByValue(Passcode.Substring(zzz[1] - 1, 1)); // CHANGE
                                mainSelect.setSelectedAttribute(option2, true);
                                break;
                            case "ThirdPassnumberValue":
                                thirdpassnumber_found = true;
                                HtmlOption option3 = mainSelect.getOptionByValue(Passcode.Substring(zzz[2] - 1, 1)); // CHANGE
                                mainSelect.setSelectedAttribute(option3, true);
                                break;
                            default:
                                break;
                        }
                    }
                }
                if (firstpassnumber_found &&
                    secondpassnumber_found &&
                    thirdpassnumber_found)
                {
                    return true;
                }
            }
            return false;
        }
#endif
#if WINFORMS
        internal static async void Listener_NotificationChangedDEV(UserNotificationListener sender,
                                                                    UserNotificationChangedEventArgs args,
                                                                    MainProcess components,
                                                                    MainViewModel ourviewmodel,
                                                                    FinanceViewModel financeviewmodel)
#endif
#if WPF || UWP
        internal static async void Listener_NotificationChangedDEV(UserNotificationListener sender,
                                                                    UserNotificationChangedEventArgs args,
                                                                    MainViewModel ourviewmodel,
                                                                    FinanceViewModel financeviewmodel)
#endif
        {
            bool ray = true;
            if (ray) //await Listener_NotificationChanged_Actual(sender,
//                                                        args,
//#if WINFORMS
//                                                        components,
//                                                        ourviewmodel,
//                                                        financeviewmodel
//#else
//                                                        MainMeter.ourviewmodel,
//                                                        MainMeter.financeviewmodel
//#endif
//                                                        ))
            {
                if (await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Notification actual: Succeeded "))
                {
                    if (!await SmartFinanceV2021.DisplayFinanceMeterAsync(
#if WINFORMS
                                                                    components,
                                                                    ourviewmodel,
                                                                    financeviewmodel,
#else
                                                                    ourviewmodel,
                                                                    financeviewmodel,
#endif
                                                                    SmartParametersV2016.activeFlag,
                                                                    SmartParametersV2016.defaultDate))
                    {
                        // We failed because of a Cancellation ... but which one? Check
                        if (!(ourviewmodel.quitCts.IsCancellationRequested ||
                            financeviewmodel.financeToken.IsCancellationRequested))
                        {
                            // If we DIDN'T request a cancellation
                            // We do nothing if there is a failure .. !  SmartSwitch DOESN'T fail!!
                            FrontEndGUI.SetLedColour(ourviewmodel, 8, ourviewmodel.redColour);
                        }
                    }
                    else
                    {
                        SmartFinanceV2021.TurnOnFinanceStatus(
#if WINFORMS
                                                        components,
#endif
                                                        financeviewmodel);
                    }
                }
            }
            else

            {
                if (await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Notification actual: Failed "))
                {
                    SmartFinanceV2021.TurnOnFinanceStatus(
#if WINFORMS
                                                        components,
#endif
                                                        financeviewmodel);
                }
            }
            return;
        }

//#if NHTMLUNIT
        internal static async Task<bool> Listener_NotificationChanged_Actual(UserNotificationListener sender,
                                                        UserNotificationChangedEventArgs args,
#if WINFORMS
                                                        MainProcess components,
#endif
                                                        MainViewModel ourviewmodel,
                                                        FinanceViewModel financeviewmodel)
        {

            string onetimepasscode = string.Empty;


#if WINFORMS
            //components.textBoxConsole.Invoke(new Action(() =>
            //{
            /* HERE YOU ARE ON GUI */
            components.textBoxConsole.AppendText("Notification event: Received! " +
                System.Environment.NewLine.ToString());
            components.textBoxConsole.ScrollToCaret();
            //}));
#endif
            //Update UI here
            await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Notification event: Received! ");

            // Get the toast notifications
            IReadOnlyList<UserNotification> notifs = await sender.GetNotificationsAsync(NotificationKinds.Toast);

            // Try and make sure the latest notification is First
            IReadOnlyList<UserNotification> notifs_sorted = (from NT in notifs
                                                             orderby NT.CreationTime descending
                                                             select NT).ToList();

            // Check to see how many notifications we have
            if (notifs_sorted.Count == 0)
            {
#if WINFORMS
                //textBoxConsole.Invoke(new Action(() =>
                //{
                //    /* HERE YOU ARE ON GUI */
                //    textBoxConsole.AppendText("Notification event: None found =:-[ " +
                //    System.Environment.NewLine.ToString());
                //    textBoxConsole.ScrollToCaret();
                //}));
#endif
                //Update UI here
                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Notification event: None found =:-[ ");

                return false;
            }
            // Select the first notification
            foreach (UserNotification usernot in notifs_sorted)
            {
                UserNotification notif = usernot;

                // Get the toast binding, if present
                NotificationBinding toastBinding = notif.Notification.Visual.GetBinding(KnownNotificationBindings.ToastGeneric);

                if (toastBinding != null)
                {
                    // And then get the text elements from the toast binding
                    IReadOnlyList<AdaptiveNotificationText> textElements = toastBinding.GetTextElements();

                    // Treat the first text element as the title text
                    string titleText = textElements.FirstOrDefault().Text;
                    if (titleText == financeviewmodel.notificationTitle)
                    {
#if WINFORMS
                        //textBoxConsole.Invoke(new Action(() =>
                        //{
                        //    /* HERE YOU ARE ON GUI */
                        //    textBoxConsole.AppendText("Notification event: Found - " + titleText +
                        //    System.Environment.NewLine.ToString());
                        //    textBoxConsole.ScrollToCaret();
                        //}));

#endif
                        //Update UI here
                        await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Notification event: Found - " + titleText);

                        // We'll treat all subsequent text elements as body text,
                        // joining them together via newlines.
                        string bodyText = string.Join("\n", textElements.Skip(1).Select(t => t.Text));
                        if (bodyText.Contains(financeviewmodel.notificationText)) //notification_prfix))
                        {
                            bodyText = bodyText.Replace(financeviewmodel.notificationText, string.Empty).Trim(); //Trim gets rid of any spaces
                            if (bodyText.Length > financeviewmodel.notificationLength)
                            {
                                onetimepasscode = bodyText.Substring(0, financeviewmodel.notificationLength);
#if WINFORMS
                                //textBoxConsole.Invoke(new Action(() =>
                                //{
                                //    /* HERE YOU ARE ON GUI */
                                //    textBoxConsole.AppendText("Notification event: Content - " + onetimepasscode +
                                //        System.Environment.NewLine.ToString());
                                //    textBoxConsole.ScrollToCaret();
                                //}));
#endif
                                //Update UI here
                                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Notification event: Content - " + onetimepasscode);
                                break;
                            }
                        }
                    }
                }
            }
            // Out of loop
            if (onetimepasscode == string.Empty)
            {
#if WINFORMS
                //textBoxConsole.Invoke(new Action(() =>
                //{
                //    /* HERE YOU ARE ON GUI */
                //    textBoxConsole.AppendText("Notification event: Content - " + "is empty" +
                //            System.Environment.NewLine.ToString());
                //    textBoxConsole.ScrollToCaret();
                //}));

#endif
                //Update UI here
                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Notification event: Content - " + "is empty");
                return false;
            }

            // Carry on with entering the SMSOTP to NW!!
            // Put it into the page
            bool smsotp = false;
            DomNodeList domInputs = financeviewmodel.NWpage.getElementsByTagName("input");
            for (int j = 0; j < domInputs.getLength(); j++)
            {
                HtmlInput smsInput = (HtmlInput)domInputs.get(j);// as HtmlInput;
                if (smsInput != null)
                {
                    string name = smsInput.getAttribute("name");
                    if (name == "OneTimePasscode")
                    {
                        smsInput.setAttribute("value", onetimepasscode);
                        smsotp = true;
                        break;
                    }
                }
            }
            if (!smsotp)
            {
#if WINFORMS
                //textBoxConsole.Invoke(new Action(() =>
                //{
                //    /* HERE YOU ARE ON GUI */
                //    textBoxConsole.AppendText("SMSOTP set failed:" + onetimepasscode +
                //        System.Environment.NewLine.ToString());
                //    textBoxConsole.ScrollToCaret();
                //}));

#endif
                //Update UI here
                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "SMSOTP set failed:" + onetimepasscode);
                return false;
            }

            bool buttonClicked = false;
            DomNodeList domButtons = financeviewmodel.NWpage.getElementsByTagName("button");
            for (int j = 0; j < domButtons.getLength(); j++)
            {
                DomElement buttonLogin = (DomElement)domButtons.get(j);// as DomElement;
                if (buttonLogin != null)
                {
                    // (Sigh) there is one? other button
                    // with this text, but it's 'type' is 'button'
                    if (buttonLogin.getTextContent().Trim() == "Log in")
                    {
                        // However OBVIOUSLY the type has got changed
                        if (buttonLogin.getAttribute("type") == "submit")
                        {
#if WINFORMS
                            //textBoxConsole.Invoke(new Action(() =>
                            //{
                            //    /* HERE YOU ARE ON GUI */
                            //    textBoxConsole.AppendText("Logging in ..." +
                            //    System.Environment.NewLine.ToString());
                            //    textBoxConsole.ScrollToCaret();
                            //}));

#endif
                            //Update UI here
                            await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Logging in ...");
                            financeviewmodel.NWpage = (HtmlPage)buttonLogin.click();// as HtmlPage;
                            buttonClicked = true;
                            break;
                        }
                    }
                }
                if (buttonClicked)
                {
                    break;
                }
            }
            if (!buttonClicked)
            {
#if WINFORMS
                //textBoxConsole.Invoke(new Action(() =>
                //{
                //    /* HERE YOU ARE ON GUI */
                //    textBoxConsole.AppendText("Button not clicked!" +
                //                System.Environment.NewLine.ToString());
                //    textBoxConsole.ScrollToCaret();
                //}));

#endif
                //Update UI here
                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Button not clicked!");
                return false;
            }

            // Try and find some links to get these fuckers
            //https://onlinebanking.nationwide.co.uk/Customisation/Customisation/MyDetailsAndSettings
            //https://onlinebanking.nationwide.co.uk/CustomerIB/ViewAndMaintainCustomerDetails/Index
            //https://onlinebanking.nationwide.co.uk/CustomerIB/MaintainTelephoneAndAddress

            // Owner
            string owner = string.Empty;
            DomNodeList divNodes1 = financeviewmodel.NWpage.getElementsByTagName("div");
            bool foundWelcome = false;
            for (int i = 0; i < divNodes1.getLength(); i++)
            {
                HtmlDivision mainDivision = (HtmlDivision)divNodes1.get(i);// as HtmlDivision;
                if (mainDivision != null)
                {
                    string claass = mainDivision.getAttribute("class");
                    if (claass == "welcome-util-container")
                    {
                        //<div id="welcome-message">Welcome back, Genius</div>
                        DomNodeList divWelcome = mainDivision.getElementsByTagName("div");
                        for (int j = 0; j < divWelcome.getLength(); j++)
                        {
                            HtmlDivision welcomeDivision = (HtmlDivision)divWelcome.get(j);// as HtmlDivision;
                            if (welcomeDivision != null)
                            {
                                if (welcomeDivision.getId() == "welcome-message")
                                {
                                    owner = welcomeDivision.getTextContent().Replace("Welcome back,", string.Empty).Trim();
                                    foundWelcome = true;
                                    break;
                                }
                            }
                        }
                    }
                }
                if (foundWelcome)
                {
                    break;
                }
            }
            if (!foundWelcome)
            {
                financeviewmodel.errorMessage = "Couldn't find 'welcome'";
                //Update UI here
                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Couldn't find 'welcome'");
            }

            string anchorhref = string.Empty;
            string addresshref = string.Empty;
            string UDPRN = string.Empty;    // Everything depends on this!

            DomNodeList domATags = financeviewmodel.NWpage.getElementsByTagName("a");
            for (int j = 0; j < domATags.getLength(); j++)
            {
                HtmlAnchor anchor = (HtmlAnchor)domATags.get(j);// as HtmlAnchor;
                if (anchor != null)
                {
                    anchorhref = anchor.getAttribute(SmartParametersV2016.href);
                    if (anchorhref.Contains("MaintainTelephoneAndAddress"))// "Customisation/MyDetailsAndSettings"))
                    {
                        // We're on the href!! So fix it up ...
                        addresshref = anchorhref.Trim('/');
                        // Now try and get the address page
                        Uri addressUri = new Uri(financeviewmodel.baseUri + addresshref);
                        HtmlPage addressPage = (HtmlPage)ourviewmodel.webClient.getPage(@addressUri.ToString());// as HtmlPage;
                        if (addressPage != null)
                        {
                            UDPRN = await FindDetails(
#if WINFORMS
                                                //textBoxConsole,
#endif
                                                ourviewmodel,
                                                financeviewmodel,
                                                //ourviewmodel.webClient,
                                                //financeviewmodel.baseUri,
                                                addressPage,
                                                financeviewmodel.login_info,
                                                owner);
                            break;
                        }
                    }
                }
            }
            if (UDPRN == "")
            {
#if WINFORMS
                //textBoxConsole.Invoke(new Action(() =>
                //{
                //    /* HERE YOU ARE ON GUI */
                //    textBoxConsole.AppendText("UDPRN: " + "<empty>" +
                //                    System.Environment.NewLine.ToString());
                //    textBoxConsole.ScrollToCaret();
                //}));

#endif
                //Update UI here
                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "UDPRN: " + "<empty>");

                // We cannot continue here.
                // The UDPRN *cannot* be empty as EVERY
                // bank account has an Address ... every one
                return false;
            }

#if WINFORMS
            //textBoxConsole.Invoke(new Action(() =>
            //{
            //    /* HERE YOU ARE ON GUI */
            //    textBoxConsole.AppendText("UDPRN good: " + UDPRN +
            //                    System.Environment.NewLine.ToString());
            //    textBoxConsole.ScrollToCaret();
            //}));

#endif
            //Update UI here
            await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "UDPRN good: " + UDPRN);

            // Step 9 continued!!

            // See if we can find the owner first
            financeviewmodel.these_accounts.Clear();
            financeviewmodel.account_id = 0;

            if (!await Nationwide_Build_AccountList(
#if WINFORMS
                                                        //textBoxConsole,
#endif
                                                        ourviewmodel.webClient,
                                                        ourviewmodel,
                                                        financeviewmodel,
                                                        financeviewmodel.NWpage,
                                                        financeviewmodel.institution_code,
                                                        financeviewmodel.brand_code,
                                                        UDPRN,
                                                        financeviewmodel.transaction_types_found))
            {
#if WINFORMS
                //textBoxConsole.Invoke(new Action(() =>
                //{
                //    /* HERE YOU ARE ON GUI */
                //    textBoxConsole.AppendText("Error: " + financeviewmodel.errorMessage +
                //                System.Environment.NewLine.ToString());
                //    textBoxConsole.ScrollToCaret();
                //}));           
#endif
                //Update UI here
                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, financeviewmodel.errorMessage);
                return false;
            }
            // Should have got all the transactions? by now??


            // accountslist MUST / SHOULD be good here
            //foreach (string account in financeviewmodel.accountslist)
            //{
            //    string[] details = account.Split(SmartParametersV2016.fieldSeparator);
            //#if WINFORMS
            //    textBoxConsole.Invoke(new Action(() =>
            //    {
            //        /* HERE YOU ARE ON GUI */
            //        textBoxConsole.AppendText("Account name: " + details[0] +
            //        System.Environment.NewLine.ToString());
            //        textBoxConsole.ScrollToCaret();
            //    })); 
            //#else
            //    Application.Current.Dispatcher.Invoke(DispatcherPriority.Normal, new ThreadStart(async delegate
            //    {
            //        //Update UI here
            //        await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Account name: " + details[0]);// +
            //    }));
            //#endif

            //" baseuri " + details[1] +
            //" href: " + details[2]);
            //DecodeAccount_New(financeviewmodel, financeviewmodel.accountslist, "£");
            //}

            //            int index = 0;
            //            foreach (FinanceViewModel.AccountItem accountItem in financeviewmodel.FinanceAccountsList)
            //            {
            //#if WINFORMS
            //                textBoxConsole.Invoke(new Action(() =>
            //                {
            //                    /* HERE YOU ARE ON GUI */
            //                    textBoxConsole.AppendText("Account ID: " + accountItem.Account_ID +
            //                                                " Name: " + accountItem.Content +
            //                                                " transactions" +
            //                    System.Environment.NewLine.ToString());
            //                    textBoxConsole.ScrollToCaret();
            //                }));

            //#else
            //                Application.Current.Dispatcher.Invoke(DispatcherPriority.Normal, new ThreadStart(async delegate
            //                {
            //                    //Update UI here
            //                    await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Account ID: " + accountItem.Account_ID +
            //                                                " Name: " + accountItem.Content +
            //                                                " transactions");
            //                }));
            //#endif
            //                string[] itemArray = accountItem.Value.Split(SmartParametersV2016.bar);
            //                if (itemArray.Length > 2)
            //                {
            //                    string sortcode = itemArray[0];
            //                    string account_no = itemArray[1];
            //                    string udprn = itemArray[2];
            //                    short currencyOrdinal = Convert.ToInt16(itemArray[3]);
            //                    string href = itemArray[4];
            //                    Uri gotcha = new Uri(baseUri.Replace("/AccountList", string.Empty) + itemArray[4]);
            //#if WINFORMS
            //                    textBoxConsole.Invoke(new Action(() =>
            //                    {
            //                        /* HERE YOU ARE ON GUI */
            //                        textBoxConsole.AppendText("SortCode: " + sortcode +
            //                                                   " Account: " + account_no +
            //                                                   " Udprn: " + udprn +
            //                                                   " Uri: " + gotcha +
            //                    System.Environment.NewLine.ToString());
            //                        textBoxConsole.ScrollToCaret();
            //                    }));

            //#else
            //                    Application.Current.Dispatcher.Invoke(DispatcherPriority.Normal, new ThreadStart(async delegate
            //                    {
            //                        //Update UI here
            //                        await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "SortCode: " + sortcode +
            //                                                   " Account: " + account_no +
            //                                                   " Uri: " + gotcha);
            //                    }));
            //#endif
            //                }
            //                index++;
            //            }

#if WINFORMS
            //textBoxConsole.Invoke(new Action(() =>
            //{
            //    /* HERE YOU ARE ON GUI */
            //    textBoxConsole.AppendText("Stopping clock" +
            //    System.Environment.NewLine.ToString());
            //    textBoxConsole.ScrollToCaret();
            //}));

            //MainMeter.ourviewmodel.timerClock.Stop();
#endif
            // Here is where you Log Out
            // Find the Logout button
            DomNodeList domBTags = financeviewmodel.NWpage.getElementsByTagName("a");
            for (int j = 0; j < domBTags.getLength(); j++)
            {
                HtmlAnchor logoutAnchor = (HtmlAnchor)domBTags.get(j);// as HtmlAnchor;
                string logouthref = logoutAnchor.getAttribute(SmartParametersV2016.href);
                if (logouthref.Contains("/AccessManagement/Logout/Logout"))
                {


                    financeviewmodel.NWpage = (HtmlPage)logoutAnchor.click();// as HtmlPage;
#if WINFORMS
                    //textBoxConsole.AppendText("Logout button: Clicked" +
                    //        System.Environment.NewLine.ToString());
                    //textBoxConsole.ScrollToCaret();
#endif
                    // Update the UI
                    await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Logout button: Clicked");

                    if (!await SmartRoutinesV2018.DoAllSmartUsers(ourviewmodel,
                                                                   SmartParametersV2016.sqliteformat))
                    {

                        // Tell the console we have added an address
#if WINFORMS
                        //textBoxConsole.AppendText("All Users: NOT updated" +
                        //        System.Environment.NewLine.ToString());
                        //textBoxConsole.ScrollToCaret();
#endif
                        // Update the UI
                        await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "All Users: NOT updated");

                        financeviewmodel.errorMessage = "Cannot update Users - cannot continue";
                        return false;
                    }
                    else
                    {
#if WINFORMS
                        //textBoxConsole.AppendText("All users: updated" +
                        //    System.Environment.NewLine.ToString());
                        //textBoxConsole.ScrollToCaret();
#endif
                        // Update the UI
                        await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "All users: updated");

                        // This SHOULD get all the Inserts and Updates in one fell swoop
                        if (!await SmartFinanceScrapeV2023.DoAllSmartFinance(ourviewmodel,
                                                                                financeviewmodel,
                                                                                true,
                                                                                SmartParametersV2016.sqliteformat))
                        {
#if WINFORMS
                            //textBoxConsole.AppendText("All Finance: NOT updated" +
                            //    System.Environment.NewLine.ToString());
                            //textBoxConsole.ScrollToCaret();
#endif
                            // Update the UI
                            await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "All Finance: NOT updated");
                            financeviewmodel.errorMessage = "Cannot update Finance - cannot continue";

                            return false;
                        }
                        else
                        {
#if WINFORMS
                            //textBoxConsole.AppendText("All finance: processed" +
                            //    System.Environment.NewLine.ToString());
                            //textBoxConsole.ScrollToCaret();
#endif
                            // Update the UI
                            await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "All Finance: processed");
                        }
                    }
                    break;
                }
            }
            return true;
        }

        // You shouldn't really create/store Addresses
        // until you've found a UDPRN!
        // Address records should ALWAYS have a valid UDPRN!
        internal static async Task<string> FindDetails(
#if WINFORMS
                                            //RichTextBox textBoxConsole,
#endif
                                            MainViewModel ourviewmodel,
                                            FinanceViewModel financeviewmodel,
                                            //WebClient webClient,
                                            //string baseURI,
                                            HtmlPage detailsPage,
                                            SmartFinance.Logins login_info,
                                            string owner)
        {
            string udprn = "";
            // Step 1 - Find address embedded in Html page
            string bankaddress = "";
            try
            {
                DomNodeList domAddress = detailsPage.getElementsByTagName("address");
                for (int j = 0; j < domAddress.getLength(); j++)
                {
                    HtmlElement address = (HtmlElement)domAddress.get(j);// as HtmlElement;
                    if (address != null)
                    {
                        bankaddress = address.getTextContent();
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
                    SmartSpikeV2017.Users_Lookup_TextAddress(ourviewmodel,
                                                            bankaddress);
                if (addresses_found.Count() > 0)
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
                    List<SmartUsers.Working_Postcodes> abc =
                        SmartSpikeV2017.Find_Working_Postcodes(ourviewmodel.Blanche.workingPostcodesList,
                                                               ideal_address.POSTCODE_OUTWARD);
                    short area_code = 0;
                    if (abc.Count() > 0)
                    {
                        area_code = abc.First().AREA_CODE;
                    }
                    SmartFinance.Addresses new_address = new SmartFinance.Addresses()
                    {
                        USERNAME = ourviewmodel.UserName,
                        CUBEFACE_CODE = SmartParametersV2016.Finance,
                        UDPRN = ideal_address.UDPRN,
                        RANDOM_KEY = SmartRoutinesV2018.Get_Next_Random(ourviewmodel.randomR),
                        ADDRESS_CREATED = DateTime.UtcNow,  // UTC time
                        Updated = false     // Its an I(nsert)
                    };
                    financeviewmodel.PLO.finance_addresses_changesList.Add(new_address);
                    SmartUsers.AddressesView new_addressview = new SmartUsers.AddressesView()
                    {
                        USERNAME = ourviewmodel.UserName,
                        UDPRN = ideal_address.UDPRN,
                        RANDOM_KEY1 = SmartRoutinesV2018.Get_Next_Random(ourviewmodel.randomR),
                        AREA_CODE = area_code,
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
                        RANDOM_KEY2 = SmartRoutinesV2018.Get_Next_Random(ourviewmodel.randomR),
                        Updated = false     // Its an I(nsert)
                    };
                    ourviewmodel.Hamas.addressesview_changesList.Add(new_addressview);
                }

                // These are not essential but 'nice to haves'
                // Email and a (any) Phone number
                // But ... where do they go????
                HtmlElement email = (HtmlElement)detailsPage.getElementById("CurrentEmailAddress");// as HtmlElement;
                if (email != null)
                {
                    string abc = email.getAttribute("value");
                    if (login_info.EMAIL != abc)
                    {
                        login_info.EMAIL = abc;
                        login_info.Updated = true;
                    }

                }
                // Can do HomePhoneNumber and/or WorkPhoneNumber here as well
                HtmlElement aphoneno = (HtmlElement)detailsPage.getElementById("MobilePhoneNumber");// as HtmlElement;
                if (aphoneno != null)
                {
                    string xyz = aphoneno.getAttribute("value");
                    if (login_info.PHONENO != xyz)
                    {
                        login_info.PHONENO = xyz;
                        login_info.Updated = true;
                    }
                }
            }
            catch (System.Exception ex)
            {
                // Catch any timeouts or failures
                // Errors here are FATAL as we always
                // need to get a text address
                financeviewmodel.errorMessage = ex.Message;
                return udprn;
            }
            // Update the OWNER if needs be
            if (owner != login_info.OWNER)
            {
                login_info.OWNER = owner;
                login_info.Updated = true;
            }
            if (login_info.Updated)
            {
                financeviewmodel.PLO.finance_logins_changesList.Add(login_info);
            }
            // Step 8 - the UDPRN here is valid
            return udprn;
        }

        internal static async Task<bool> Nationwide_Build_AccountList(
#if WINFORMS
                                                            //RichTextBox textBoxConsole,
#endif
                                                            WebClient webClient,
                                                            MainViewModel ourviewmodel,
                                                            FinanceViewModel financeviewmodel,
                                                            HtmlPage accountsPage,
                                                            short institution_code,
                                                            short brand_code,
                                                            string UDPRN,
                                                            List<SmartFinance.Transaction_Types> transaction_types_found)
        {
            bool foundActive = false;
            string accountInfo;
            string href = string.Empty;
            string id = string.Empty;
            string accName = string.Empty;
            string sortcode = string.Empty;
            string account_no = string.Empty;
            string balance = "";
            string symbol = "";

            // Accounts List
            HtmlPage transactionsPage;
            DomNodeList divNodes2 = accountsPage.getElementsByTagName("a");
            for (int i = 0; i < divNodes2.getLength(); i++)
            {
                HtmlAnchor mainAnchor = (HtmlAnchor)divNodes2.get(i);// as HtmlAnchor;
                if (mainAnchor != null)
                {
                    href = mainAnchor.getAttribute(SmartParametersV2016.href);
                    if (href.Contains("/AccountList/Account/RedirectToDefaultPage"))
                    {
                        string anchorClass = mainAnchor.getAttribute("class");
                        if (anchorClass == "")
                        {
                            string text = mainAnchor.getTextContent();
                            // First!!! Work backwards until you hit
                            // a currency symbol i.e. one that isnt
                            // 0,1,2,3,4,5,6,7,8,9 or , or .
                            int len = text.Length;
                            while (len > 0)
                            {
                                len--;
                                char abc = Convert.ToChar(text.Substring(len, 1));
                                if (abc == SmartParametersV2016.defaultCurrencySeparator ||
                                    abc == SmartParametersV2016.defaultThousandsSeparator ||
                                    Char.IsDigit(abc))
                                {
                                    continue;
                                }
                                else
                                {
                                    // Found a currency symbol but for £ we
                                    // might well be overdrawn!!
                                    symbol = abc.ToString();
                                    if (len > 0)
                                    {
                                        if (text.Substring(len - 1, 1) == "-")
                                        {
                                            // We are!
                                            len--;
                                        }
                                    }
                                    balance = text.Substring(len);
                                    text = text.Substring(0, len);
                                    break;
                                }
                            }

                            // Now ... split the text up.
                            // assume the FIRST is an account name (or part of one)
                            // assume the LAST is always the balance ... good luck!
                            string[] items = text.Split(SmartParametersV2016.spaceSplit);
                            int itemCount = items.Count();
                            accName = "";
                            sortcode = "";
                            account_no = "";
                            if (itemCount < 2)
                            {
                                // Not enough items
                                continue;
                            }
                            int itemPosLow = 0;
                            int itemPosHigh = itemCount - 1;
                            if (items[itemPosLow] == "")
                            {
                                // Can't find account name
                                continue;
                            }
                            accName = items[itemPosLow];
                            itemPosLow++;
                            // Now ... everything is betwwen these two
                            if (itemPosLow >= itemPosHigh)
                            {
                                // Can't find sort code and or acc num
                                continue;
                            }
                            // Should always have ONE acc number
                            account_no = items[itemPosHigh];
                            itemPosHigh--;
                            // Now ... everything is between these two
                            while (itemPosLow <= itemPosHigh)
                            {
                                // Is first character a digit?
                                char abc = Convert.ToChar(items[itemPosLow].Substring(0, 1));
                                if (Char.IsDigit(abc))
                                {
                                    sortcode = items[itemPosLow];
                                }
                                else
                                {
                                    accName = accName + SmartParametersV2016.space + items[itemPosLow];
                                }
                                itemPosLow++;
                            }
                            if (sortcode == "")
                            {
                                id = account_no;
                            }
                            else
                            {
                                id = sortcode +
                                SmartParametersV2016.space +
                                account_no;
                            }
                            accountInfo = accName +
                                SmartParametersV2016.fieldSeparator +
                                id +
                                SmartParametersV2016.fieldSeparator +
                                href;

                            // Processing ...

                            short currency_ordinal =
                                SmartRoutinesV2018.GetCurrencyOrdinal(ourviewmodel, symbol);
                            DecodeAccount(ourviewmodel,
                                        financeviewmodel,
                                        accountInfo,
                                        currency_ordinal,
                                        institution_code,
                                        brand_code,
                                        UDPRN);
                            foundActive = true;

                            //HtmlElement classAnchor = accountsPage.getHtmlElementById(id) as HtmlElement;

                            HtmlAnchor classAnchor = (HtmlAnchor)accountsPage.getHtmlElementById(id);// as HtmlAnchor;
                            string anchClass = classAnchor.getAttribute("class");
                            if (anchClass == "acLink")
                            {
                                transactionsPage = (HtmlPage)classAnchor.click();// as HtmlPage;
                                if (transactionsPage != null)
                                {
                                    string tokenValue = FindTokenValue(transactionsPage);
                                    if (tokenValue != "")
                                    {
                                        // DoTheDates CAN return False,
                                        // However, sometimes we can't find any
                                        // Transactions so it returns true in that case
                                        // It only returns False if some WebClient
                                        // based shit goes wrong e.g. page = null
                                        if (!await DoTheDatesBusiness(webClient,
                                                                tokenValue,
                                                                ourviewmodel,
                                                                financeviewmodel,
                                                                transactionsPage,
                                                                institution_code,
                                                                brand_code,
                                                                sortcode,
                                                                account_no,
                                                                UDPRN,
                                                                currency_ordinal,
                                                                transaction_types_found))
                                        {
                                            financeviewmodel.errorMessage = "Problem with dates" + accName;
                                            //return false;
                                        }
                                        else
                                        {
#if WINFORMS
                                            //textBoxConsole.Invoke(new Action(() =>
                                            //{
                                            //    /* HERE YOU ARE ON GUI */
                                            //    textBoxConsole.AppendText("Processed: " + id +
                                            //                                "(" + financeviewmodel.transactions_count.ToString() + ")" +
                                            //                                System.Environment.NewLine.ToString());
                                            //    textBoxConsole.ScrollToCaret();
                                            //}));                    
#endif
                                            // Update the UI
                                            await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Processed: " + id +
                                                "(" + financeviewmodel.transactions_count.ToString() + ")");
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            if (!foundActive)
            {
                financeviewmodel.errorMessage = "Couldn't find 'active'";
                return false;
            }
            return true;
        }

        internal static bool DecodeAccount(MainViewModel ourviewmodel,
                                    FinanceViewModel financeviewmodel,
                                    string accountInfo,
                                    short currencyOrdinal,
                                    short institution_code,
                                    short brand_code,
                                    string udprn)
        {
            // We pick up the Balance from the very last transaction item
            char category_code = SmartParametersV2016.Banks; //default
            string account_name,
                        account_no = string.Empty,
                        sortcode = string.Empty;
            //remainder;


            string[] details = accountInfo.Split(SmartParametersV2016.fieldSeparator);
            if (details.Length > 1)
            {
                account_name = details[0].Trim();
                if (account_name.Contains("Savings") ||
                    account_name.Contains("savings"))
                {
                    category_code = SmartParametersV2016.Savings;
                }
                if (details.Length > 2)
                {
                    string[] accstuff = details[1].Split(Convert.ToChar(SmartParametersV2016.space));
                    if (accstuff.Length > 1)
                    {
                        sortcode = accstuff[0];
                        // Be Bold!!!
                        if (sortcode != "")
                        {
                            sortcode.Replace("-", string.Empty);
                        }
                        account_no = accstuff[1];
                    }
                    else
                    {
                        account_no = accstuff[0];
                    }
                }

                if (account_name == string.Empty)
                {
                    return false;
                }
                // 0 = sortcode
                // 1 = account_no
                // 2 = currency
                // 3 = href
                string vaalue = sortcode + SmartParametersV2016.fieldSeparator +
                                account_no + SmartParametersV2016.fieldSeparator +
                                udprn + SmartParametersV2016.fieldSeparator +
                                currencyOrdinal + SmartParametersV2016.fieldSeparator +    // Assume we can find it in Banking
                                                                                           // Safety check
                                details[2].Replace("&amp;", "&");
                string[] itemArray = vaalue.Split(SmartParametersV2016.fieldSeparator);

                financeviewmodel.account_id++;

                FinanceViewModel.AccountItem account_item = new FinanceViewModel.AccountItem
                {
                    AccountID = financeviewmodel.account_id,
                    Content = account_name,
                    // Here we have [0] = sort_code
                    //              [1] = account_no
                    //              [2] = udprn
                    //              [3] = currency ordinal
                    //              [4] = href
                    Value = string.Join(SmartParametersV2016.bar.ToString(), itemArray)
                };
                financeviewmodel.these_accounts.Add(account_item);

                // We've found one, now do we need to add it to Accounts?
                List<SmartFinance.Accounts> accounts_found =
                    SmartSpikeFinanceV2017.Finance_Lookup_AccountsCategory(ourviewmodel,
                                                                    financeviewmodel,
                                                                    institution_code,
                                                                    brand_code,
                                                                    category_code,
                                                                    sortcode,
                                                                    account_no,
                                                                    udprn,
                                                                    currencyOrdinal);
                if (accounts_found.Count() == 0)
                {
                    // Dummy these two up
                    int balance = 0;
                    char status = 'O';
                    // Its not in the Accounts List so add it in
                    SmartFinance.Accounts account_record =
                        SmartFinanceV2021.Account_Template(ourviewmodel,
                                                            financeviewmodel,
                                                            institution_code,
                                                            brand_code,
                                                            category_code,
                                                            sortcode,
                                                            account_no,
                                                            udprn,
                                                            account_name,   // Title
                                                            currencyOrdinal,
                                                            balance,
                                                            status);
                    financeviewmodel.PLO.finance_accounts_changesList.Add(account_record);
                }
            }
            //financeviewmodel.FinanceAccountsList = these_accounts;
            return true;
        }

        internal static string FindTokenValue(HtmlPage transactionsPage)
        {
            string tokenValue = string.Empty;

            DomNodeList scriptNodes = transactionsPage.getElementsByTagName("script");
            for (int j = 0; j < scriptNodes.getLength(); j++)
            {
                HtmlScript script = (HtmlScript)scriptNodes.get(j);// as HtmlScript;
                if (script != null)
                {
                    string wotsin = script.ToString();
                    int tokenStart = wotsin.IndexOf("token:");
                    if (tokenStart >= 0)
                    {
                        tokenValue = wotsin.Substring(tokenStart + 6).Trim();
                        int commaEnd = tokenValue.IndexOf(',');
                        if (commaEnd >= 0)
                        {
                            tokenValue = tokenValue.Substring(0, commaEnd - 1);
                            tokenValue = tokenValue.Trim('\'');
                            break;
                        }
                    }
                }
            }
            if (tokenValue == "")
            {
                // You possibly have no transactions
                // visible for this account, although there
                // may be some in your paper statements
                //financeviewmodel.errorMessage = "Token value: " + "is empty =:-{";
                //await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, errorMessage);
                //await SmartRoutinesV2018.CheckTrace(ourviewmodel, financeviewmodel.financeToken, institution_code, brand_code, errorMessage);
            }
            return tokenValue;
        }

#if NHTMLUNIT
        internal static async Task<bool> DoTheDatesBusiness(WebClient webClient,
                                                    string tokenValue,
                                                    MainViewModel ourviewmodel,
                                                    FinanceViewModel financeviewmodel,
                                                    HtmlPage transactionsPage,
                                                    short institution_code,
                                                    short brand_code,
                                                    string sortcode,
                                                    string account_no,
                                                    string UDPRN,
                                                    short currencyOrdinal,
                                                    List<SmartFinance.Transaction_Types> transaction_types_found)
        {

            HtmlAnchor dateReveal = (HtmlAnchor)transactionsPage.getElementById("enter-date-reveal-link");// as HtmlAnchor;//  QuerySelector("input[name^='Start']") as HtmlTextInput;
            if (dateReveal == null)
            {
                return false;
            }

            // DATES!!  Wherever you are in the world, Nationwide
            // dates and times are always local to the UK, so
            // you always need to add the 'utcOffset' to the UTC 'time now'
            // in order to get 'Local UK time'
            try
            {
                DateTime time_now = DateTime.Now + ourviewmodel.utcOffset;   // Local time
                string end_datex = time_now.ToString(SmartParametersV2016.standardFormat);
                DateTime startTime = time_now.AddMonths(-15);
                // Adjustment bollocks! To see if it works!!
                string start_datex = startTime.AddDays(1).ToString(SmartParametersV2016.standardFormat);

                // The way these date buttons work ... is weird!  You can
                // fix the 'end' date button more or less easily ...
                // although we really want it to be 'today' in all cases.
                // However, the 'start' button is far more difficult to fix,
                // and the website ALWAYS wants to try and fix it to
                // 'today' minus 15 months, so if 'today' is 05/03/2022
                // (5th March 2022) then the website won't allow you to use
                // a date EARLIER then 05/12/2020 (5th December 2020)
                // Even if I enter '01/01/1900' then that date gets adjusted
                // (by some poxy Javafuckingscript?) to '05/12/2020'.
                // So I have two choices: either always enter a bonkers
                // '01/01/1900' date and get it adjusted OR calculate
                // the 15 months difference between 'today' and a 'start'
                // date.  But what if NW adjust the start date at some point?
                // I would have to amend my calculation and change the program
                // OR I could define it as a parameter... (more work).
                // Decision: I'm going to stick it in as '01/01/1900' and
                // let the website do the work ....

                //HtmlInput start_date = transactionsPage.getElementById("statement-from-date") as HtmlInput;//  QuerySelector("input[name^='Start']") as HtmlTextInput;
                //if (start_date == null)
                //{
                //    financeviewmodel.errorMessage = "DoTheDates: " + "No start date";
                //    //await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, errorMessage);
                //    //await SmartRoutinesV2018.CheckTrace(ourviewmodel, financeviewmodel.financeToken, institution_code, brand_code, errorMessage);
                //    return false;
                //}

                //string from_date = "01/01/1900";    // This will be adjusted to the minimum by the website (I hope)
                //start_date.setValueAttribute(start_datex); // from_date;

                string to_date = end_datex;


                // This ONLY took me an ENTIRE FUCKING WEEK-END
                // to get going ....
                java.net.URL url = transactionsPage.getWebResponse().getWebRequest().getUrl();

                java.net.URL urlWithout = new java.net.URL(url.ToString().Replace("/0", string.Empty));

                HttpMethod method = com.gargoylesoftware.htmlunit.HttpMethod.POST;
                com.gargoylesoftware.htmlunit.WebRequest requestSettings = new com.gargoylesoftware.htmlunit.WebRequest(urlWithout, method);

                requestSettings.setAdditionalHeader("Accept", "application/json, text/javascript, */*; q=0.01");
                requestSettings.setAdditionalHeader("Content-Type", "application/x-www-form-urlencoded; charset=UTF-8");
                requestSettings.setAdditionalHeader("Referer", url.ToString());
                requestSettings.setAdditionalHeader("Accept-Language", "en-US,en;q=0.8");
                requestSettings.setAdditionalHeader("Accept-Encoding", "gzip, deflate, br");
                requestSettings.setAdditionalHeader("X-Requested-With", "XMLHttpRequest");
                requestSettings.setAdditionalHeader("ADRUM", "isAjax:true");

                java.util.List paramsx = new java.util.ArrayList();

                paramsx.add(new com.gargoylesoftware.htmlunit.util.NameValuePair("start", start_datex));
                paramsx.add(new com.gargoylesoftware.htmlunit.util.NameValuePair("end", end_datex));
                paramsx.add(new com.gargoylesoftware.htmlunit.util.NameValuePair("__token", tokenValue));

                paramsx.add(new com.gargoylesoftware.htmlunit.util.NameValuePair("filters", "TransactionFilterMapping.AllMoneyPaidOut"));
                paramsx.add(new com.gargoylesoftware.htmlunit.util.NameValuePair("filters", "TransactionFilterMapping.CashWithdrawal"));
                paramsx.add(new com.gargoylesoftware.htmlunit.util.NameValuePair("filters", "TransactionFilterMapping.ChequeWithdrawal"));
                paramsx.add(new com.gargoylesoftware.htmlunit.util.NameValuePair("filters", "TransactionFilterMapping.DirectDebits"));
                paramsx.add(new com.gargoylesoftware.htmlunit.util.NameValuePair("filters", "TransactionFilterMapping.ChargesOut"));
                paramsx.add(new com.gargoylesoftware.htmlunit.util.NameValuePair("filters", "TransactionFilterMapping.DebitCardPayments"));
                paramsx.add(new com.gargoylesoftware.htmlunit.util.NameValuePair("filters", "TransactionFilterMapping.RegularPaymentsOutside"));
                paramsx.add(new com.gargoylesoftware.htmlunit.util.NameValuePair("filters", "TransactionFilterMapping.SinglePaymentsOutside"));
                paramsx.add(new com.gargoylesoftware.htmlunit.util.NameValuePair("filters", "TransactionFilterMapping.TransfersToNationwide"));
                paramsx.add(new com.gargoylesoftware.htmlunit.util.NameValuePair("filters", "TransactionFilterMapping.ReturnedItems"));
                paramsx.add(new com.gargoylesoftware.htmlunit.util.NameValuePair("filters", "TransactionFilterMapping.MiscellaneousDebits"));
                paramsx.add(new com.gargoylesoftware.htmlunit.util.NameValuePair("filters", "TransactionFilterMapping.AllMoneyPaidIn"));
                paramsx.add(new com.gargoylesoftware.htmlunit.util.NameValuePair("filters", "TransactionFilterMapping.CashCredits"));
                paramsx.add(new com.gargoylesoftware.htmlunit.util.NameValuePair("filters", "TransactionFilterMapping.ChequeCredits"));
                paramsx.add(new com.gargoylesoftware.htmlunit.util.NameValuePair("filters", "TransactionFilterMapping.BankCredits"));
                paramsx.add(new com.gargoylesoftware.htmlunit.util.NameValuePair("filters", "TransactionFilterMapping.TransfersFromNationwideAccount"));
                paramsx.add(new com.gargoylesoftware.htmlunit.util.NameValuePair("filters", "TransactionFilterMapping.DebitCardTransactions"));
                paramsx.add(new com.gargoylesoftware.htmlunit.util.NameValuePair("filters", "TransactionFilterMapping.UnpaidItems"));
                paramsx.add(new com.gargoylesoftware.htmlunit.util.NameValuePair("filters", "TransactionFilterMapping.ChargesIn"));
                paramsx.add(new com.gargoylesoftware.htmlunit.util.NameValuePair("filters", "TransactionFilterMapping.MiscellaneousCredits"));

                requestSettings.setRequestParameters(paramsx);
                // Its taken me a FUCKING WEEK and
                // I was looking in the WRONG PLACE!!
                // I was expecting it to return me an
                // HtmlPage when ALL I NEEDED was the
                // fucking WEB-FUCKING-RESPONSE!!!!

                WebResponse rays = webClient.getPage(requestSettings).getWebResponse();// as WebResponse;
                if (rays.getContentType() == "application/json")
                {
                    string responseData = rays.getContentAsString();
                    if (responseData != "")
                    {
                        if (!await Nationwide_Find_Transactions(responseData,
                                                                    ourviewmodel,
                                                                    financeviewmodel,
                                                                    institution_code,
                                                                    brand_code,
                                                                    sortcode,
                                                                    account_no,
                                                                    UDPRN,
                                                                    currencyOrdinal,
                                                                    transaction_types_found))
                        {
                            return false;
                        }
                    }
                }
            }
            catch (java.lang.Exception ex)
            {
                // Probably a timeout
                //status = false;
                //await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "DoTheDates: " + ex.Message);
                //await SmartRoutinesV2018.CheckTrace(ourviewmodel, financeviewmodel.financeToken, institution_code, brand_code, "DoTheDates: " + ex.Message);
                financeviewmodel.errorMessage = ex.Message;
                return false;
            }
            return true;    // Although there may well have been no transactions ..
        }

        //Scrape the webpage version
        internal static async Task<bool> Nationwide_Find_Transactions(string responseData,
                                                                        MainViewModel ourviewmodel,
                                                                        FinanceViewModel financeviewmodel,
                                                                        short institution_code,
                                                                        short brand_code,
                                                                        string sortcode,
                                                                        string account_no,
                                                                        string UDPRN,
                                                                        short currencyOrdinal,
                                                                        List<SmartFinance.Transaction_Types> transaction_types_found)
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
                                                                                            transaction_types_found,
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
            catch (java.lang.Exception ex)
            {
                await SmartRoutinesV2018.CheckTrace(ourviewmodel, financeviewmodel.financeToken, institution_code, brand_code, ex.Message + " " + "General failure");
                return false;
            }
            return true;
        }
#endif
    }
}