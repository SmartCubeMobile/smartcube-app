//using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
//using System.Net.Http;
using System.Threading.Tasks;

using System.Reflection;
using System.Collections;
//using System.Net;

//using com.gargoylesoftware.htmlunit;
//using com.gargoylesoftware.htmlunit.html;
using System.Text;
//using Newtonsoft.Json.Linq;
//using com.sun.xml.@internal.bind.v2.model.core;
//using org.omg.PortableInterceptor;
using System.Collections.ObjectModel;
//using com.sun.beans.decoder;
//using com.sun.tools.javac.comp;
//using static iText.StyledXmlParser.Jsoup.Select.Evaluator;
//using System.Windows.Documents;
//using java.lang;
//using com.gargoylesoftware.htmlunit.util;
//using java.util;
//using com.sun.tools.javac.api;
//using com.sun.corba.se.impl.oa.toa;
//using System.Net;
//using WebResponse = com.gargoylesoftware.htmlunit.WebResponse;
//using WebClient = com.gargoylesoftware.htmlunit.WebClient;
//using static SmartCubeMobile.SmartFinance;
using SmartCubeMobile;
using Newtonsoft.Json.Linq;




#if WINFORMS
//using System.Windows.Forms;
//using NHtmlUnit.Html;
#endif
#if WPF
using System.Windows.Controls;
using IKVM.Java;
using NHtmlUnit.Util;
using NHtmlUnit.Html;
using NHtmlUnit;
//using System.Windows.UI.Notifications.Management;
//using System.Windows.UI.Notifications;
//using System.Windows.ApplicationModel;
//using Windows.UI.Notifications;
//using Windows.ApplicationModel;
//using Windows.UI.Notifications.Management;
//using SmartCubeV2023;
#endif
#if WINDOWS_UWP
//using NHtmlUnit.Html;
#endif
#if XAMARIN
//using Windows.UI.Notifications;
//using Windows.ApplicationModel;
//using Windows.UI.Notifications.Management;
using NHtmlUnit.Html;

#endif

#if MAUI
//using Microsoft.Playwright;
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
        internal static bool FindAddress(MainViewModel ourviewmodel,
                                            FinanceViewModel financeviewmodel,
                                            NHtmlUnit.WebClient webClient,
                                            HtmlPage page,
                                            string baseURI)
        {
            bool status = false;
            SmartFinance.Accounts account_info = new SmartFinance.Accounts();

            try
            {
                IReadOnlyList<HtmlElement> manage_list = page.QuerySelectorAll("primary-nav-link-level-3-sibling-1-unique-") as IReadOnlyList<HtmlElement>;
                foreach (HtmlElement manage_link in manage_list)
                {
                    string href1 = manage_link.GetAttribute("href");
                    if (href1.IndexOf("myaddress") > 0)
                    {
                        href1 = baseURI + href1;
                        page = webClient.GetHtmlPage(href1) as HtmlPage;

                        HtmlElement address = page.QuerySelector("address") as HtmlElement;
                        if (address != null)
                        {
                            string myaddress = address.GetAttribute("text");
                            myaddress = myaddress.Trim();
                            SmartUsers.AddressesView ideal_address = new SmartUsers.AddressesView();
                            //                        if (!await SmartFinanceV2021.UDPRNLookup(
                            //#if WINFORMS
                            //                                                            textBoxConsole,
                            //#endif
                            //                                                            ourviewmodel,
                            //                                                            financeviewmodel,
                            //                                                            xyz => ideal_address = xyz,
                            //                                                            myaddress))
                            //                        {
                            //                            return false;
                            //                        }

                            string udprn = ideal_address.UDPRN; // "12345678"; // "100011448863";
                            if (string.IsNullOrEmpty(account_info.UDPRN) ||
                                account_info.UDPRN != udprn)   // Change of address
                            {
                                account_info.UDPRN = udprn;
                                account_info.Updated = true; // Don't forget to update the Logins in DoAll_SmartFinance

                                //ObservableCollection<SmartUsers.Addresses> addresses_list = 
                                //    SmartSpikeFinanceV2017.Finance_Find_Addresses(ourviewmodel,
                                //                                                financeviewmodel,
                                //                                                udprn);
                                //if (addresses_list.Count == 0)
                                //{
                                    // Its not there
                                    //SmartUsers.Addresses new_address = new SmartUsers.Addresses()
                                    //{                                                
                                    //    UDPRN = udprn,
                                    //    POSTCODE = "SK8 3JH",
                                    //    Updated = false     // Its an I(nsert)
                                    //};
                                    ideal_address.Updated = false; // Its an I(nsert)
                                    
                                //ourviewmodel.Hamas.addresses_changes_list.Add(ideal_address);
                                    
                                
                                    //if (await SmartRoutinesV2018.DoAll_SmartUsers(signinviewmodel,
                                    //            ourviewmodel,
                                    //            SmartParametersV2016.sqliteformat,
                                    //            em => ourviewmodel.error_message = em))
                                    //{
                                    //    // Tell the console we have added an address
                                    //    if (!await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Added new address: " + ideal_address.UDPRN))
                                    //    {
                                    //        return false;
                                    //    }
                                    //}
                                //}
                            }
                        }
                        HtmlElement phone_id = page.QuerySelector("a[id^='current-phone-number']") as HtmlElement;
                        if (phone_id != null)
                        {
                            HtmlElement phone_dl = phone_id.QuerySelector("dl") as HtmlElement;
                            if (phone_dl != null)
                            {
                                IReadOnlyList<HtmlElement> phone_dds_dts = phone_dl.QuerySelectorAll("d^") as IReadOnlyList<HtmlElement>;
                                foreach (HtmlElement phone_dd_dt in phone_dds_dts)
                                {


                                }
                            }
                        }
                        string myemail = string.Empty;
                        IReadOnlyList<HtmlElement> email_details = page.QuerySelectorAll("div[class^='details-value']") as IReadOnlyList<HtmlElement>;
                        foreach (HtmlElement email_div in email_details)
                        {
                            myemail = email_div.GetAttribute("text");
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
            catch (System.Exception ex)
            {
                // Catch any timeouts
                status = false;
                financeviewmodel.errorMessage = ex.Message;
                //await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "FindAddress: " + ex.Message);
                //await SmartRoutinesV2018.Check_Trace(ourviewmodel, financeviewmodel.finance_token, institution_code, brand_code, "FindAddress : " + ex.Message);
            }
            return status;
        }

        internal static bool DecodeAccount_New(FinanceViewModel financeviewmodel,
                                    string[] accounts_list,
                                    string currencySymbol)
        {
            // We pick up the Balance from the very last transaction item

            List<FinanceViewModel.AccountItem> these_accounts =
                new List<FinanceViewModel.AccountItem>();

            int account_id = 0;

            foreach (string account in accounts_list)
            {
                string account_name = string.Empty,
                        account_no = string.Empty,
                        sortcode = string.Empty,
                        remainder = string.Empty;

                account_id++;
                string[] details = account.Split(SmartParametersV2016.fieldSeparator);
                if (details.Length > 1)
                {
                    account_name = details[0].Trim();
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
                                    "GBP" + SmartParametersV2016.fieldSeparator +    // Assume we can find it in OpenBanking
                                    // Safety check
                                    details[2].Replace("&amp;", "&");
                    string[] itemArray = vaalue.Split(SmartParametersV2016.fieldSeparator);
#if WINFORMS
                    FinanceViewModel.AccountItem account_item = new FinanceViewModel.AccountItem
#else
                    FinanceViewModel.AccountItem account_item = new FinanceViewModel.AccountItem
#endif
                    {
                        AccountID = account_id,
                        Content = account_name,
                        // Here we have [0] = sort_code
                        //              [1] = account_no
                        //              [2] = currency
                        //              [3] = href
                        Value = string.Join(SmartParametersV2016.bar.ToString(), itemArray)
                    };
                    these_accounts.Add(account_item);
                }
            }
            financeviewmodel.FinanceAccountsList = these_accounts;
            return true;
        }

//        internal bool DecodeAccount(FinanceViewModel financeviewmodel,
//                                    string[] accounts_list,
//                                    string currencySymbol)
//        {
//            ObservableCollection<FinanceViewModel.AccountItem> these_accounts =
//                new ObservableCollection<FinanceViewModel.AccountItem>();

//            int account_id = 0;

//            foreach (string account in accounts_list)
//            {
//                string account_name = string.Empty,
//                        account_no = string.Empty,
//                        sortcode = string.Empty,
//                        balance = string.Empty,
//                        remainder = string.Empty;

//                account_id++;
//                string[] details = account.Split(SmartParametersV2016.fieldSeparator);
//                if (details.Length > 1)
//                {
//                    string account_token = details[0].Trim();
//                    // Work backwards until we find a '£'
//                    for (int pos = account_token.Length; pos > 0; pos--)
//                    {
//                        string thisun = account_token.Substring(pos - 1, 1);
//                        if (thisun == currencySymbol)
//                        {
//                            balance = account_token.Substring(pos - 1); // To inlcude '£'
//                            remainder = account_token.Substring(0, pos - 1);
//                            break;
//                        }
//                    }
//                    // There MUST be SOMETHING in the Balance, even if it is =:-[ only £0.00 ...
//                    if (balance == string.Empty ||
//                        remainder == string.Empty)
//                    {
//                        return false;
//                    }

//                    // Now work backwards from the remainder until we find a space?
//                    for (int pos = remainder.Length; pos > 0; pos--)
//                    {
//                        string thisun = remainder.Substring(pos - 1, 1);
//                        if (thisun == SmartParametersV2016.space)
//                        {
//                            account_no = remainder.Substring(pos);
//                            remainder = remainder.Substring(0, pos - 1);
//                            break;
//                        }
//                    }

//                    if (account_no == string.Empty ||
//                        remainder == string.Empty)
//                    {
//                        return false;
//                    }

//                    // Now the remainder MIGHT contain a Sort Code .. or not
//                    // Lets assume it does
//                    // Now work backwards from the remainder until we find a space?
//                    for (int pos = remainder.Length; pos > 0; pos--)
//                    {
//                        string thisun = remainder.Substring(pos - 1, 1);
//                        if (thisun == SmartParametersV2016.space)
//                        {
//                            sortcode = remainder.Substring(pos);
//                            account_name = remainder.Substring(0, pos - 1);
//                            // At this point, the sort_code SHOULD be all digits
//                            // If it's not the tack it on the end of the account_name
//                            for (int digpos = 0; digpos < sortcode.Length; digpos++)
//                            {
//                                if (!Char.IsDigit(Convert.ToChar(sortcode.Substring(digpos, 1))))
//                                {
//                                    account_name = account_name +
//                                                    SmartParametersV2016.space +
//                                                    sortcode;
//                                    sortcode = string.Empty;
//                                    break;
//                                }
//                            }
//                            break;
//                        }
//                    }

//                    if (account_name == string.Empty)
//                    {
//                        return false;
//                    }
//                    // Be Bold!!!
//                    if (sortcode != "")
//                    {
//                        sortcode.Replace("-", string.Empty);
//                    }
//                    // 0 = sortcode
//                    // 1 = account_no
//                    // 2 = currency
//                    // 3 = balance
//                    // 4 = baseUri
//                    // 5 = href
//                    string vaalue = sortcode + SmartParametersV2016.fieldSeparator +
//                                    account_no + SmartParametersV2016.fieldSeparator +
//                                    "GBP" + SmartParametersV2016.fieldSeparator +    // Assume we can find it in OpenBanking
//                                    balance + SmartParametersV2016.fieldSeparator +
//                                    // Safety check
//                                    details[1].Replace("&amp;", "&") + SmartParametersV2016.fieldSeparator +
//                                    details[2].Replace("&amp;", "&");
//                    string[] itemArray = vaalue.Split(SmartParametersV2016.fieldSeparator);
//#if WINFORMS
//                    FinanceViewModel.AccountItem account_item = new FinanceViewModel.AccountItem
//#else
//                    FinanceViewModel.AccountItem account_item = new FinanceViewModel.AccountItem
//#endif
//                    {
//                        Account_ID = account_id,
//                        Content = account_name,
//                        // Here we have [0] = sort_code
//                        //              [1] = account_no
//                        //              [2] = currency
//                        //              [3] = balance
//                        //              [4] = baseUri
//                        //              [5] = href
//                        Value = string.Join(SmartParametersV2016.bar.ToString(), itemArray)
//                    };
//                    these_accounts.Add(account_item);
//                }
//            }
//            financeviewmodel.FinanceAccounts_List = these_accounts;
//            return true;
//        }
        //internal static WebClient Create(MainViewModel ourviewmodel, bool withProxy)
        //{
        //    WebClient webClient = null;
        //    try
        //    {
        //        if (!withProxy)
        //        {
        //            webClient = new WebClient(BrowserVersion.CHROME); //, "127.0.0.0", 8888);
        //        }
        //        else
        //        {
        //            // At the moment - we need Fiddler running
        //            // for this to work! (You couldn't make this shit up - you really couldn't ...
        //            webClient = new WebClient(BrowserVersion.CHROME, "127.0.0.1", 8888);
        //        }
        //    }
        //    catch (System.Exception ex)
        //    {
        //        ourviewmodel.errorMessage   = ex.Message;
        //    }
        //    return webClient;
        //}

        //internal static WebClient Options(WebClient webClient)
        //{
        //    // Leaving this next one OUT might fix 'failed handshake' problem?
        //    // I am assuming it defaults to 'false' ???
        //    // No - that had nothing to do with it!!

        //    webClient.getOptions().setThrowExceptionOnScriptError(false);
        //    webClient.getOptions().setUseInsecureSSL(true);
        //    webClient.getOptions().setJavaScriptEnabled(true);
        //    webClient.getOptions().setCssEnabled(true); // false;
        //    webClient.getOptions().setActiveXNative(true);
        //    webClient.getOptions().setRedirectEnabled(true);
        //    webClient.getOptions().setThrowExceptionOnScriptError(false);
        //    webClient.getOptions().setThrowExceptionOnFailingStatusCode(false);
        //    string[] prot = new string[3] { "TLSv1.2", "TLSv1.1", "TLSv1" };// give protocol alert
        //    webClient.getOptions().setSSLClientProtocols(prot);
        //    webClient.getOptions().setPopupBlockerEnabled(true);
        //    webClient.getOptions().setAppletEnabled(true);
        //    webClient.getOptions().setConnectionTimeToLive(20000);
        //    webClient.getOptions().setTimeout(60000);
        //    //webClient.getOptions().wait(10000); No!!!          
        //    return webClient;
        //}

        internal static bool NationwideFindInputs(
#if WINFORMS
                                                //RichTextBox textBoxConsole,
                                                MainViewModel ourviewmodel,
#endif
                                        string CustomerNumber,
                                        string Day,
                                        string Year,
                                        HtmlPage page)
        {
#if WINFORMS
            //textBoxConsole.AppendText("Finding inputs: " + Environment.NewLine.ToString());
            //textBoxConsole.ScrollToCaret();
            MainWindow.TextBlockUpdate(ourviewmodel, "Finding inputs: " + Environment.NewLine.ToString());
#endif

            bool status = false;

            bool customernumber_found = false,
                dateofbirthday_found = false,
                dateofbirthyear_found = false;

            var domInputs = page.GetElementsByTagName("input"); //as List<DomNode>;
            for (int i = 0; i < domInputs.Count; i++)
            {
                HtmlInput pageInput = domInputs[i] as HtmlInput;
                if (pageInput != null)
                {
                    if (pageInput.GetAttribute("id") == "CustIdentDetails_CustomerNumber")
                    {
                        pageInput.SetAttribute("text", CustomerNumber);

                    }
                    if (pageInput.GetAttribute("id").Contains("text-input-control"))
                    {
#if WINFORMS 
                        MainWindow.TextBlockUpdate(ourviewmodel,
                                        "Text: " + pageInput.getTextContent() +
                                        " Name: " + pageInput.getNameAttribute() +
                                        Environment.NewLine.ToString());
                        //textBoxConsole.ScrollToCaret();
#endif
                        switch (pageInput.GetAttribute("name"))
                        {
                            case "CustomerNumber":
                                customernumber_found = true;
                                pageInput.SetAttribute("text",CustomerNumber);
                                break;
                            case "DateOfBirthDay":
                                dateofbirthday_found = true;
                                pageInput.SetAttribute("text", Day);
                                break;
                            case "DateOfBirthYear":
                                dateofbirthyear_found = true;
                                pageInput.SetAttribute("text", Year);
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

        internal static bool FillInputMonth(MainViewModel ourviewmodel, HtmlPage mainScreen, string Month)
        {
            bool dateofbirthmonth_found = false;
            var domSelects = mainScreen.GetElementsByTagName("select");//as List<DomNode>;
            for (int i = 0; i < domSelects.Count; i++)
            {
                HtmlSelect mainSelect = domSelects[i] as HtmlSelect;
                if (mainSelect != null)
                {
                    if (mainSelect.GetAttribute("id").Contains("selection-list-control"))
                    {
                        string name = mainSelect.GetAttribute("name"); // Name?
                        switch (name)
                        {
                            case "DateOfBirthMonth":
                                dateofbirthmonth_found = true;
                                HtmlOption option = mainSelect.GetOptionByValue(Month);
                                mainSelect.SetSelectedAttribute(option, true);
#if WINFORMS
                                MainWindow.TextBlockUpdate(ourviewmodel,
                                                    "Name: " + name +
                                                    //" Text: " + mainSelect.TextContent +
                                                    " Set to: " + Month +
                                                    Environment.NewLine.ToString());
                                //textBoxConsole.ScrollToCaret();
#endif
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

        internal static bool FindPassNumbers(HtmlPage page, ref int[] zzz)
        {
            // Look for which pass numbers they want
            //int[] zzz = new int[3];   // So we have 0, 1 and 2
            var domSpans = page.GetElementsByTagName("span");
            for (int i = 0; i < domSpans.Count; i++)
            {
                HtmlSpan mainSpan = domSpans[i] as HtmlSpan;
                if (mainSpan != null)
                {
                    string classname = mainSpan.GetAttribute("class");
                    if (classname == "control__label__title")
                    {
                        int index = 0;
                        string text = mainSpan.GetAttribute("text");
                        if (text.Contains("digits from your passnumber"))
                        {
                            if (text.Contains("1st"))
                            {
                                zzz[index] = 1; // 0;
                                index++;
                            };
                            if (text.Contains("2nd"))
                            {
                                zzz[index] = 2; // 1;
                                index++;
                            };
                            if (text.Contains("3rd"))
                            {
                                zzz[index] = 3; // 2;
                                index++;
                            };
                            if (text.Contains("4th"))
                            {
                                zzz[index] = 4; // 3;
                                index++;
                            };
                            if (text.Contains("5th"))
                            {
                                zzz[index] = 5; // 4;
                                index++;
                            };
                            if (text.Contains("6th"))
                            {
                                zzz[index] = 6; // 5;
                                index++;
                            };
                            break;
                        }
                    }
                }
            }
            if (zzz[0] == 0 ||
                zzz[1] == 0 ||
                zzz[2] == 0)
            {
                return false;
            }
            return true;
        }

        internal static bool SetPassNumbers(ref HtmlPage page, string Passcode, int[] zzz)
        {
            // Why zzz[0] - 1? Because we record the first passcode
            // as 1 and not 0.  The -1 is to make the offset into
            // the passcode string 'right'
            bool firstpassnumber_found = false,
                secondpassnumber_found = false,
                thirdpassnumber_found = false;

            var domSelects = page.GetElementsByTagName("select");
            for (int i = 0; i < domSelects.Count; i++)
            {
                HtmlSelect mainSelect = domSelects[i] as HtmlSelect;
                if (mainSelect != null)
                {
                    if (mainSelect.GetAttribute("id").Contains("selection-list-control"))
                    {
                        string name = mainSelect.GetAttribute("name"); // Name?
                        switch (name)
                        {
                            case "FirstPassnumberValue":
                                firstpassnumber_found = true;
                                HtmlOption option1 = mainSelect.GetOptionByValue(Passcode.Substring(zzz[0] - 1, 1)); // CHANGE
                                mainSelect.SetSelectedAttribute(option1, true);
                                break;
                            case "SecondPassnumberValue":
                                secondpassnumber_found = true;
                                HtmlOption option2 = mainSelect.GetOptionByValue(Passcode.Substring(zzz[1] - 1, 1)); // CHANGE
                                mainSelect.SetSelectedAttribute(option2, true);
                                break;
                            case "ThirdPassnumberValue":
                                thirdpassnumber_found = true;
                                HtmlOption option3 = mainSelect.GetOptionByValue(Passcode.Substring(zzz[2] - 1, 1)); // CHANGE
                                mainSelect.SetSelectedAttribute(option3, true);
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

        internal static bool Nationwide_Build_Account_List(NHtmlUnit.WebClient webClient,
                                                            MainViewModel ourviewmodel,
                                                            FinanceViewModel financeviewmodel,
                                                            HtmlPage accountsPage,
                                                            short institution_code,
                                                            short brand_code)
        {
            // Owner
            var divNodes1 = accountsPage.GetElementsByTagName("div");
            bool foundWelcome = false;
            for (int i = 0; i < divNodes1.Count; i++)
            {
                HtmlDivision mainDivision = divNodes1[i] as HtmlDivision;
                if (mainDivision != null)
                {
                    string claass = mainDivision.GetAttribute("class");
                    if (claass == "welcome-util-container")
                    {
                        //<div id="welcome-message">Welcome back, Genius</div>
                        var divWelcome = mainDivision.GetElementsByTagName("div");
                        for (int j = 0; j < divWelcome.Count; j++)
                        {
                            HtmlDivision welcomeDivision = divWelcome[j] as HtmlDivision;
                            if (welcomeDivision != null)
                            {
                                if (welcomeDivision.GetAttribute("id") == "welcome-message")
                                {
                                    financeviewmodel.owner = welcomeDivision.GetAttribute("text").Replace("Welcome back,", string.Empty).Trim();
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
                //return false;
            }
            // Accounts List
            HtmlPage transactionsPage;
            var divNodes2 = accountsPage.GetElementsByTagName("a");

            bool foundActive = false;
            string acclist = string.Empty;
            string href = string.Empty;
            string id = string.Empty;
            string accName = string.Empty;
            string sortcode = string.Empty;
            string account_no = string.Empty;
            string currency = "GBP";
            for (int i = 0; i < divNodes2.Count; i++)
            {
                HtmlAnchor mainAnchor = divNodes2[i] as HtmlAnchor;
                if (mainAnchor != null)
                {
                    string claass = mainAnchor.GetAttribute("class");
                    if (claass == "acLink")
                    {
                        href = mainAnchor.GetAttribute("href");
                        if (href.Contains("/AccountList"))
                        {
                            id = mainAnchor.GetAttribute("text");//  GetId(); // Poss Sort and Account                    
                            string[] split = id.Split(' ');
                            if (split.Length > 1)
                            {
                                sortcode = split[0];
                                account_no = split[1];

                            }
                            else
                            {
                                account_no = split[0];
                            }

                            var bNodes = mainAnchor.GetElementsByTagName("b");
                            for (int j = 0; j < bNodes.Count; j++)
                            {
                                HtmlElement accountName = bNodes[j] as HtmlElement;
                                if (accountName != null)
                                {
                                    accName = accountName.GetAttribute("text");//    GetTextContent();
                                    if (acclist != "")
                                    {
                                        acclist = acclist + SmartParametersV2016.recordSeparator;
                                    }
                                    acclist = acclist + accName +
                                                SmartParametersV2016.fieldSeparator +
                                                id +
                                                SmartParametersV2016.fieldSeparator +
                                                href;
                                    foundActive = true;
                                }
                            }

                            transactionsPage = mainAnchor.Click() as HtmlPage;
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
                                    if (!DoTheDatesBusiness(webClient,
                                                            tokenValue,
                                                            ourviewmodel,
                                                            financeviewmodel,
                                                            transactionsPage,
                                                            sortcode,
                                                            account_no,
                                                            currency,
                                                            institution_code,
                                                            brand_code))
                                    {
                                        financeviewmodel.errorMessage = "Problem with dates" + accName;
                                        //return false;
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
            if (acclist == "")
            {
                financeviewmodel.errorMessage = "Couldn't split acclist";
                return false;
            }
            else
            {
                financeviewmodel.accounts = acclist.Split(SmartParametersV2016.recordSeparator);
            }
            return true;
        }

        internal static string FindTokenValue(HtmlPage transactionsPage)
        {
            string tokenValue = string.Empty;

            var scriptNodes = transactionsPage.GetElementsByTagName("script");
            for (int j = 0; j < scriptNodes.Count; j++)
            {
                HtmlScript script = scriptNodes[j] as HtmlScript;
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
                // You possibly have no ttransactions
                // visible for this account, although there
                // may be some in your paper statements
                //financeviewmodel.error_message = "Token value: " + "is empty =:-{";
                //await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, error_message);
                //await SmartRoutinesV2018.Check_Trace(ourviewmodel, financeviewmodel.finance_token, institution_code, brand_code, error_message);
            }
            return tokenValue;
        }
        
        internal static bool DoTheDatesBusiness(NHtmlUnit.WebClient webClient,
                                                    string tokenValue,
                                                    MainViewModel ourviewmodel,
                                                    FinanceViewModel financeviewmodel,
                                                    HtmlPage transactionsPage,
                                                    string sortcode,
                                                    string account_no,
                                                    string currency,
                                                    short institution_code,
                                                    short brand_code)
        {
            
            HtmlAnchor dateReveal = transactionsPage.GetElementById("enter-date-reveal-link") as HtmlAnchor;//  QuerySelector("input[name^='Start']") as HtmlTextInput;
            if (dateReveal == null)
            {
                return false;
            }
            // NO! Not needed??
            //transactionsPage = dateReveal.click() as HtmlPage;


            // DATES!!  Wherever you are in the world, Nationwide
            // dates and times are always local to the UK, so
            // you always need to add the 'gmtoffset' to the 'time now'
            // in order to get 'UK time'
            try
            {
                DateTime time_now = DateTime.Now.Add(ourviewmodel.utcOffset);
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
                //    financeviewmodel.error_message = "DoTheDates: " + "No start date";
                //    //await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, error_message);
                //    //await SmartRoutinesV2018.Check_Trace(ourviewmodel, financeviewmodel.finance_token, institution_code, brand_code, error_message);
                //    return false;
                //}

                string from_date = "01/01/1900";    // This will be adjusted to the minimum by the website (I hope)
                //start_date.setValueAttribute(start_datex); // from_date;

                string to_date = end_datex;


                // This ONLY took me an ENTIRE FUCKING WEEK-END
                // to get going ....
                //java.net.URL url = null;// transactionsPage.GetWebResponse().getWebRequest().getUrl();

                //java.net.URL urlWithout = new java.net.URL(url.ToString().Replace("/0", string.Empty));

                HttpMethod method = null; // HttpMethod.POST;
                //com.gargoylesoftware.htmlunit.WebRequest requestSettings = new com.gargoylesoftware.htmlunit.WebRequest(urlWithout, method);

                //requestSettings.setAdditionalHeader("Accept", "application/json, text/javascript, */*; q=0.01");
                //requestSettings.setAdditionalHeader("Content-Type", "application/x-www-form-urlencoded; charset=UTF-8");
                //requestSettings.setAdditionalHeader("Referer", url.ToString());
                //requestSettings.setAdditionalHeader("Accept-Language", "en-US,en;q=0.8");
                //requestSettings.setAdditionalHeader("Accept-Encoding", "gzip, deflate, br");
                //requestSettings.setAdditionalHeader("X-Requested-With", "XMLHttpRequest");
                //requestSettings.setAdditionalHeader("ADRUM", "isAjax:true");

                //java.util.List paramsx = new java.util.ArrayList();

                //paramsx.add(new NameValuePair("start", start_datex));
                //paramsx.add(new NameValuePair("end", end_datex));
                //paramsx.add(new NameValuePair("__token", tokenValue));

                //paramsx.add(new NameValuePair("filters", "TransactionFilterMapping.AllMoneyPaidOut"));
                //paramsx.add(new NameValuePair("filters", "TransactionFilterMapping.CashWithdrawal"));
                //paramsx.add(new NameValuePair("filters", "TransactionFilterMapping.ChequeWithdrawal"));
                //paramsx.add(new NameValuePair("filters", "TransactionFilterMapping.DirectDebits"));
                //paramsx.add(new NameValuePair("filters", "TransactionFilterMapping.ChargesOut"));
                //paramsx.add(new NameValuePair("filters", "TransactionFilterMapping.DebitCardPayments"));
                //paramsx.add(new NameValuePair("filters", "TransactionFilterMapping.RegularPaymentsOutside"));
                //paramsx.add(new NameValuePair("filters", "TransactionFilterMapping.SinglePaymentsOutside"));
                //paramsx.add(new NameValuePair("filters", "TransactionFilterMapping.TransfersToNationwide"));
                //paramsx.add(new NameValuePair("filters", "TransactionFilterMapping.ReturnedItems"));
                //paramsx.add(new NameValuePair("filters", "TransactionFilterMapping.MiscellaneousDebits"));
                //paramsx.add(new NameValuePair("filters", "TransactionFilterMapping.AllMoneyPaidIn"));
                //paramsx.add(new NameValuePair("filters", "TransactionFilterMapping.CashCredits"));
                //paramsx.add(new NameValuePair("filters", "TransactionFilterMapping.ChequeCredits"));
                //paramsx.add(new NameValuePair("filters", "TransactionFilterMapping.BankCredits"));
                //paramsx.add(new NameValuePair("filters", "TransactionFilterMapping.TransfersFromNationwideAccount"));
                //paramsx.add(new NameValuePair("filters", "TransactionFilterMapping.DebitCardTransactions"));
                //paramsx.add(new NameValuePair("filters", "TransactionFilterMapping.UnpaidItems"));
                //paramsx.add(new NameValuePair("filters", "TransactionFilterMapping.ChargesIn"));
                //paramsx.add(new NameValuePair("filters", "TransactionFilterMapping.MiscellaneousCredits"));

                //requestSettings.setRequestParameters(paramsx);
                // Its taken me a FUCKING WEEK and
                // I was looking in the WRONG PLACE!!
                // I was expecting it to return me an
                // HtmlPage when ALL I NEEDED was the
                // fucking WEB-FUCKING-RESPONSE!!!!

                NHtmlUnit.WebResponse rays = null; // webClient.GetPage(requestSettings).getWebResponse() as WebResponse;
                if (rays.ContentType == "application/json")
                {
                    string responseData = null;// rays.GetContentAsString(rays.ContentType);
                    if (responseData != "")
                    {
                        List<SmartFinance.BankTransactions> Scraped =
                            NationwideV2023.Nationwide_Find_Transactions(responseData,
                                                                    ourviewmodel,
                                                                    financeviewmodel,
                                                                    institution_code,
                                                                    brand_code,
                                                                    sortcode,
                                                                    account_no);
                        if (Scraped.Count > 0)
                        {
                            financeviewmodel.bank_transactions.AddRange(Scraped);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Probably a timeout
                //status = false;
                //await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "DoTheDates: " + ex.Message);
                //await SmartRoutinesV2018.Check_Trace(ourviewmodel, financeviewmodel.finance_token, institution_code, brand_code, "DoTheDates: " + ex.Message);
                financeviewmodel.errorMessage = ex.Message;
                return false;
            }            
            return true;    // Although there may well have been no transactions ..
        }

        //Scrape the webpage version
        internal static List<SmartFinance.BankTransactions> Nationwide_Find_Transactions(string responseData,
                                                                        MainViewModel ourviewmodel,
                                                                        FinanceViewModel financeviewmodel,
                                                                        short institution_code,
                                                                        short brand_code,
                                                                        string sortcode,
                                                                        string account_no)
        {
            string accounts = string.Empty;

            List<SmartFinance.BankTransactions> Scraped = new List<SmartFinance.BankTransactions>();

            // This selects the ones we want (!!) with an EXTRA FUCKER which has no 'id' so
            // the Chimps don't let us filter that one out with a selector - we
            // have to FUCK ABOUT and do it ourselves .. I don't know how to
            // do 'multiple selectors' i.e. acLink AND href=/AccountList

            string currency = "GBP",    // Pull this in from the account
                    amount =  string.Empty,
                    date,
                    description = string.Empty,
                    balance =  string.Empty,
                    sequence_no,
                    transaction_type;
            bool isPaidIn = false;
            short transaction_code,
                    transaction_sub_code;

            DateTime converted_date = SmartParametersV2016.defaultDate;

            //ONE DATE for all ofs Batch of transactions!!
            DateTime transaction_created = DateTime.Now;// SmartEncryptionV2016.DateTimeNow(ourviewmodel.gmt_offset);   // GMT time
            try
            {   
                dynamic jsonResponse = JObject.Parse(responseData);
                //Console.WriteLine(jsonResponse);
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
                                                //converted_date = SmartRoutinesV2018.DateTimeParse(date).ToLocalTime();
                                            }
                                            catch (System.Exception ex)
                                            {
                                                // CheckTrace ??
                                                //await SmartBobV2017.ListenerAsync(ourviewmodel, financeviewmodel.finance_token, financeviewmodel.institution_code, financeviewmodel.brand_code, ex.Message + ": " + date);

                                            }
                                            break;
                                        case "Description":
                                            description = individual.Value;
                                            break;
                                        case "IsPaidIn":
                                            isPaidIn = Convert.ToBoolean(individual.Value);
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

                                //if (!SmartSpikeFinanceV2017.Lookup_Transaction_SubCode(financeviewmodel,
                                //                                                            is_paid_in,
                                //                                                            ref transaction_type,
                                //                                                            ref transaction_code,
                                //                                                            ref transaction_sub_code,
                                //                                                            false))
                                //{

                                //    await SmartBobV2017.ListenerAsync(ourviewmodel, financeviewmodel.finance_token, financeviewmodel.institution_code, financeviewmodel.brand_code, "Trans type  failed: " + transaction_type);

                                //}

                                // Still want to add it in even if we can't analyze the transaction ...
                                //                int random1 = SmartRoutinesV2018.Get_Next_Random(ourviewmodel.random_r);
                                //int random2 = SmartRoutinesV2018.Get_Next_Random(ourviewmodel.random_r);
                                int random1 = 1;
                                int random2 = 2;
                                transaction_code = 1;
                                transaction_sub_code = 2;
                                SmartFinance.BankTransactions bankTransaction = new SmartFinance.BankTransactions()
                                {
                                    USERNAME = "RAY", //ourviewmodel.UserName,
                                    INSTITUTION_CODE = institution_code,
                                    BRAND_CODE = brand_code,
                                    SORTCODE = sortcode,
                                    ACCOUNT_NO = account_no,
                                    RANDOMKEY1 = random1,
                                    BOOKING_DATE = converted_date,
                                    //SEQUENCE_NO = new DateTimeOffset(DateTime.Now) + ourviewmodel.gmt_offset).ToUnixTimeMilliseconds(), // Unique AND increasing
                                    CREDITDEBIT_INDICATOR = isPaidIn,
                                    TRANSACTION_CODE = transaction_code,
                                    //TRANSACTION_SUB_CODE = transaction_sub_code,
                                    DESCRIPTION = description,
                                    AMOUNT = Convert.ToInt32(amount.Replace(".", string.Empty)),
                                    //CURRENCY = currency,     // Pull this in from the Account
                                    BALANCE_AMOUNT = Convert.ToInt32(balance.Replace(".", string.Empty)),
                                    RANDOMKEY2 = random2,
                                    Updated = false
                                };
                                Scraped.Add(bankTransaction);
                            }
                        }
                    }
                }
               
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                //await SmartRoutinesV2018.Check_Trace(ourviewmodel, financeviewmodel.finance_token, institution_code, brand_code, ex.Message + " " + "General failure");
            }
            return Scraped;
        }
    }
}

//    HtmlElement fullContainer = transactionsPage.GetElementById("full-statement-container");
//    if (fullContainer != null)
//    {
//        List<HtmlTableBody> tbodies = fullContainer.HtmlElementDescendants
//                        .OfType<NHtmlUnit.Html.HtmlTableBody>().ToList();
//        if (tbodies.Length > 0)
//        {
//            List<HtmlTableRow> trows = tbodies[0].HtmlElementDescendants
//                    .OfType<NHtmlUnit.Html.HtmlTableRow>().ToList();
//            if (trows.Length > 0)
//            {
//                foreach (HtmlTableRow tr_row in trows)
//                {
//                    List<HtmlTableDataCell> td_rows = tr_row.HtmlElementDescendants
//                            .OfType<NHtmlUnit.Html.HtmlTableDataCell>().ToList();
//                    int col = 0;
//                    datex = string.Empty;
//                    transaction_type = string.Empty;
//                    description = string.Empty;
//                    paid_in = string.Empty;
//                    paid_out = string.Empty;
//                    is_paid_in = false;
//                    transaction_code = 0;
//                    transaction_sub_code = 0;
//                    amount = string.Empty;
//                    balance = string.Empty;
//                    currency = string.Empty;
//                    foreach (HtmlTableDataCell td_row in td_rows)
//                    {
//                        switch (col)
//                        {
//                            case 0: // Date
//                                datex = td_row.TextContent;
//                                datex = datex.Trim();
//                                datex = datex.Substring(0, 11).Trim();
//                                try
//                                {
//                                    converted_date = DateTime.Now;//   SmartRoutinesV2018.DateTimeParse(datex).ToLocalTime();
//                                }
//                                catch (Exception ex)
//                                {
//                                    Console.WriteLine(ex.Message + datex);
//                                    //await SmartRoutinesV2018.Check_Trace(ourviewmodel, financeviewmodel.finance_token, institution_code, brand_code, ex.Message + " " + datex);
//                                }
//                                break;
//                            case 1: // Type
//                                transaction_type = td_row.TextContent;
//                                transaction_type = transaction_type.Trim();
//                                break;
//                            case 2: // Description
//                                description = td_row.TextContent;
//                                description = description.Trim();
//                                description = description.Replace("&amp;", "&");
//                                break;
//                            case 3: // Paid In
//                                paid_in = td_row.TextContent;
//                                paid_in = paid_in.Trim();
//                                break;
//                            case 4: // Paid out
//                                paid_out = td_row.TextContent;
//                                paid_out = paid_out.Trim();
//                                break;
//                            case 5: // Balance
//                                balance = td_row.TextContent;
//                                balance = balance.Trim();
//                                if (balance.IndexOf('£') != -1)
//                                {
//                                    balance = balance.Replace("£", string.Empty);
//                                }
//                                break;
//                            default:
//                                break;
//                        }
//                        col++;
//                    }
//                    // Should have a full line here ....
//                    if (string.IsNullOrEmpty(paid_out))
//                    {
//                        is_paid_in = true;
//                        amount = paid_in;
//                    }
//                    else
//                    {
//                        amount = paid_out;
//                    }
//                    if (amount.IndexOf('£') != -1)
//                    {
//                        currency = "GBP";
//                        amount = amount.Replace("£", string.Empty);
//                    }

//                    //if (!SmartSpikeFinanceV2017.Finance_Lookup_TransactionSubCode(ourviewmodel,
//                    //                                        financeviewmodel,
//                    //                                        is_paid_in,
//                    //                                        ref transaction_type,
//                    //                                        ref transaction_code,
//                    //                                        ref transaction_sub_code,
//                    //                                        false,
//                    //                                        institution_code,
//                    //                                        brand_code))
//                    //{
//                    //    await SmartRoutinesV2018.Check_Trace(ourviewmodel, financeviewmodel.finance_token, institution_code, brand_code, "Trans type lookup failed: " + transaction_type);
//                    //}

//                    // Still want to add it in even if we can't analyze the transaction ...
//                    int random1 = 1; // SmartRoutinesV2018.Get_Next_Random(ourviewmodel.random_r);
//                    int random2 = 2; // SmartRoutinesV2018.Get_Next_Random(ourviewmodel.random_r);

//                    try
//                    {
//                        SmartFinance.BankTransactions banktransaction = new SmartFinance.BankTransactions()
//                        {
//                            USERNAME = ourviewmodel.UserName,
//                            CUBEFACE_CODE = SmartParametersV2016.Finance,
//                            INSTITUTION_CODE = institution_code,
//                            BRAND_CODE = brand_code,
//                            SORTCODE = sort_code,
//                            ACCOUNT_NO = account_no,
//                            RANDOMKEY1 = random1,
//                            BOOKING_DATE = converted_date,
//                            SEQUENCE_NO = new DateTimeOffset(DateTime.Now + ourviewmodel.gmt_offset).ToUnixTimeMilliseconds(), // Unique AND increasing
//                            //SEQUENCE_NO = new DateTimeOffset(DateTime.Now).ToUnixTimeMilliseconds(), // Unique AND increasing
//                            CREDITDEBIT_INDICATOR = is_paid_in,
//                            TRANSACTION_CODE = transaction_code,
//                            TRANSACTION_SUB_CODE = transaction_sub_code,
//                            DESCRIPTION = description,
//                            AMOUNT = Convert.ToInt32(amount.Replace(".", string.Empty)),
//                            CURRENCY_ORDINAL = 1,     // Pull this in from the Account
//                            BALANCE_TYPE = string.Empty,
//                            BALANCE_AMOUNT = Convert.ToInt32(balance.Replace(".", string.Empty)),
//                            BALANCE_CURRENCY_ORDINAL = 1,     // Pull this in from the Account
//                            BALANCE_CREDITDEBIT_INDICATOR = is_paid_in,
//                            PTC_CODE = 0,
//                            EXCHANGE_RATES = new decimal[4] { 0, 0, 0, 0 },
//                            RANDOMKEY2 = random2,
//                            Updated = false
//                        };
//                        Scraped.Add(banktransaction);
//                    }
//                    catch (Exception ex)
//                    {
//                        Console.WriteLine(ex.Message);

//                        //await SmartRoutinesV2018.Check_Trace(ourviewmodel, financeviewmodel.finance_token, institution_code, brand_code, ex.Message + " " + "Cannot add Transaction record");
//                        //
//                    }
//                }
//            }
//        }
//    }    

//internal static StringBuilder Nationwide_Find_TransactionsOlder(HtmlPage transactionsPage,
//                                                    TestViewModel ourviewmodel,
//                                                    //FinanceViewModel financeviewmodel,
//                                                    short institution_code,
//                                                    short brand_code,
//                                                    string sortcode,
//                                                    string account_no)
//{
//    //< div id = "full-statement-container"
//    string amount = "";
//    string date = "";
//    DateTime converted_date = SmartParametersV2016.default_date;
//    string description = "";
//    bool is_paid_in = false;
//    string balance = "";
//    string sequence_no = "";
//    string transaction_type = "";

//    StringBuilder Bollocks = new StringBuilder();


//    // Owner
//    HtmlElement transContainer = transactionsPage.GetElementById("full-statement-container");
//    if (transContainer != null)
//    {
//        List<HtmlTable> bodies = transContainer.HtmlElementDescendants
//            .OfType<NHtmlUnit.Html.HtmlTable>().ToList();
//        foreach (HtmlTable body in bodies)
//        {
//            var rows = body.GetElementsByTagName("tr");
//            foreach (HtmlElement row in rows)
//            {
//                var columns = body.GetElementsByTagName("td");
//                {
//                    string line = string.Empty;
//                    foreach (HtmlElement col in columns)
//                    {
//                        string vaalue = col.GetAttribute("value");
//                        line = line + " " + vaalue;
//                    }
//                    Bollocks.Append(line);
//                }
//            }
//        }
//    }



//        internal static async Task<bool> Nationwide_FindAccounts_PW(
//#if WINFORMS
//                                                    RichTextBox textBoxConsole,
//#endif
//                                                    string anchor_text,
//                                                    string start_url,
//                                                    string notification_title,
//                                                    string notification_prefix,
//                                                    short notification_tag_length,
//                                                    string CustomerNumber,
//                                                    string DOB,
//                                                    string Passcode, //string[] PassNumbers)
//                                                    Action<string> set_error_message)
//        {


//            if (Passcode.Length < 6)
//            {
//                set_error_message("Invalid Passcode " + Passcode);
//                return false;
//            }
//            if (DOB.Length < 8)
//            {
//                set_error_message("Invalid DOB " + DOB);
//                return false;
//            }
//            // Split the DOB into int's components
//            string Day = DOB.Substring(0, 2);
//            string Month = SmartParametersV2016.months[Convert.ToInt16(DOB.Substring(2, 2)) - 1];
//            string Year = DOB.Substring(4, 4);

//            Console.WriteLine("Hello World!");
//#if SCRAPE_PLAYWRIGHT
//            var playwright = await Playwright.CreateAsync();
//            var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
//            {
//                Headless = true
//            });
//            var context = await browser.NewContextAsync();
//            var page = await context.NewPageAsync();
//            //Navigate to Nationwide
//            //IResponse document = await page.GotoAsync(start_url);
//            if (document == null)
//            {
//                set_error_message("Couldn't find url " + start_url);
//                return false;
//            }
//#if WINFORMS
//            textBoxConsole.AppendText("Retrieved page: " + start_url + Environment.NewLine.ToString());
//            textBoxConsole.ScrollToCaret();
//#endif

//            // Could never get the fucking button to work ... ever
//            //await page.ClickAsync("button[text='Log in']");

//            var all_https = await page.QuerySelectorAllAsync("a[href^='https://onlinebanking']");
//            if (all_https != null)
//            {
//                foreach (var each_https in all_https)
//                {
//                    string dataref = await each_https.GetAttributeAsync("data-ref");
//                    string href = await each_https.GetAttributeAsync("href");
//                    Console.WriteLine(dataref + " " + href.ToString());
//                    if (dataref == "link")
//                    {
//                        document = await page.GotoAsync(href);
//                        break;
//                    }
//                }
//            }
//            if (document == null)
//            {
//                set_error_message("Couldn't find 'online' link");
//                return false;
//            }

//            if (!await NationwideFindInputs(
//#if WINFORMS
//                                            textBoxConsole,
//#endif
//                                            CustomerNumber, Day, Year, page))
//            {
//                set_error_message("Couldn't find main inputs");
//                return false;
//            }
//#if WINFORMS
//            textBoxConsole.AppendText("Found all three" +
//                                    Environment.NewLine.ToString());
//            textBoxConsole.ScrollToCaret();
//#endif
//            // Now do the month dropdown

//            var dateofbirth_month = await page.SelectOptionAsync("select[name='DateOfBirthMonth']", Month);
//            if (dateofbirth_month == null)
//            {
//                set_error_message("Couldn't find 'month' drop-down input");
//                return false;
//            }

//            // Press the fucking equivalent of 'Submit'!!!! (You couldn't make this shit up ...)
//            await page.Keyboard.PressAsync("Enter");
//            // Wait for the fucker to get its act together
//            await page.WaitForNavigationAsync();
//#if WINFORMS
//            textBoxConsole.AppendText("Passnumbers: " +
//                                            Passcode.Substring(0, 1) +
//                                            " " +
//                                            Passcode.Substring(1, 1) +
//                                            " " +
//                                            Passcode.Substring(2, 1) +
//                                            " " +
//                                            Passcode.Substring(3, 1) +
//                                            " " +
//                                            Passcode.Substring(4, 1) +
//                                            " " +
//                                            Passcode.Substring(5, 1) +
//                                            Environment.NewLine.ToString());
//            textBoxConsole.ScrollToCaret();
//#endif
//            //
//            // Passcodes
//            //
//            var passcodes = await page.QuerySelectorAllAsync("span[class='control__label__title']");
//            if (passcodes == null)
//            {
//                set_error_message("Couldn't find 'passcodes' span class");
//                return false;
//            }
//            foreach (var each_passcode in passcodes)
//            {
//                string text = await each_passcode.TextContentAsync();
//                if (text.Contains("digits from your passnumber"))
//                {
//                    Console.WriteLine(text.ToString());

//                    int index = 0;

//                    // Look for which pass numbers they want
//                    int[] zzz = new int[3];   // So we have 0, 1 and 2                                
//#if WINFORMS
//                    textBoxConsole.AppendText("Passnumbers reqd: " +
//                                        text +
//                                        Environment.NewLine.ToString());
//                    textBoxConsole.ScrollToCaret();
//#endif
//                    if (text.Contains("1st"))
//                    {
//                        zzz[index] = 0;
//                        index++;
//                    };
//                    if (text.Contains("2nd"))
//                    {
//                        zzz[index] = 1;
//                        index++;
//                    };
//                    if (text.Contains("3rd"))
//                    {
//                        zzz[index] = 2;
//                        index++;
//                    };
//                    if (text.Contains("4th"))
//                    {
//                        zzz[index] = 3;
//                        index++;
//                    };
//                    if (text.Contains("5th"))
//                    {
//                        zzz[index] = 4;
//                        index++;
//                    };
//                    if (text.Contains("6th"))
//                    {
//                        zzz[index] = 5;
//                        index++;
//                    };
//#if WINFORMS
//                    textBoxConsole.AppendText("Passnumbers to be sent: " +
//                                        Passcode.Substring(zzz[0], 1) +
//                                        " " +
//                                        Passcode.Substring(zzz[1], 1) +
//                                        " " +
//                                        Passcode.Substring(zzz[2], 1) +
//                                        Environment.NewLine.ToString());
//                    textBoxConsole.ScrollToCaret();
//#endif
//                    var firstpassnumber = await page.SelectOptionAsync("select[name='FirstPassnumberValue']", Passcode.Substring(zzz[0], 1));
//                    if (firstpassnumber == null)
//                    {
//                        set_error_message("Couldn't find 'First pass number' drop-down input");
//                        return false;
//                    }

//                    var secondpassnumber = await page.SelectOptionAsync("select[name='SecondPassnumberValue']", Passcode.Substring(zzz[1], 1));
//                    if (secondpassnumber == null)
//                    {
//                        set_error_message("Couldn't find 'Second pass number' drop-down input");
//                        return false;
//                    }

//                    var thirdpassnumber = await page.SelectOptionAsync("select[name='ThirdPassnumberValue']", Passcode.Substring(zzz[2], 1));
//                    if (thirdpassnumber == null)
//                    {
//                        set_error_message("Couldn't find 'Third pass number' drop-down input");
//                        return false;
//                    }
//                    break;
//                }
//            }

//            // One-time Pass Code
//            var buttons = await page.QuerySelectorAllAsync("button");
//            if (buttons == null)
//            {
//                set_error_message("Couldn't find any buttons to push");
//                return false;
//            }

//            foreach (var each_button in buttons)
//            {
//                string text = await each_button.TextContentAsync();
//                if (text.Trim() == "Continue")
//                {
//                    // Press the fucking equivalent of 'Submit'!!!! (You couldn't make this shit up ...)
//                    await each_button.ClickAsync();
//                    // Wait for the fucker to get its act together
//                    //await page.WaitForNavigationAsync();
//                    break;
//                }
//            }

//            // Read the TOKEN HERE!!!
//            TextReader txt = new StreamReader(@"C:\Users\Ray\AppData\Local\SMSOTP.txt");
//            string onetimepasscode = txt.ReadToEnd();
//            txt.Close();

//            if (!string.IsNullOrEmpty(onetimepasscode))
//            {
//                var smsotp = await page.QuerySelectorAsync("input[name='OneTimePasscode']");
//                if (smsotp == null)
//                {
//                    set_error_message("Couldn't find the SMSOTP input");
//                    return false;
//                }

//                await smsotp.FillAsync(onetimepasscode);

//                foreach (var each_button in buttons)
//                {
//                    string text = await each_button.TextContentAsync();
//                    if (text.Trim() == "Log in")
//                    {
//                        // Press the fucking equivalent of 'Submit'!!!! (You couldn't make this shit up ...)
//                        await each_button.ClickAsync();
//                        // Wait for the fucker to get its act together
//                        await page.WaitForNavigationAsync();
//                        break;
//                    }
//                }

//            }
//#endif
//            return true;
//        }


//        internal static async Task<bool> NationwideFindInputs(
//#if WINFORMS
//                                                RichTextBox textBoxConsole,
//#endif
//                                                string CustomerNumber,
//                                                string Day,
//                                                string Year
//#if SCRAPE_PLAYWRIGHT
//                                                ,Microsoft.Playwright.IPage page
//#endif
//                                                )
//        {
//#if WINFORMS
//            textBoxConsole.AppendText("Finding inputs: " + Environment.NewLine.ToString());
//            textBoxConsole.ScrollToCaret();
//#endif

//            bool status = false;

//            bool customernumber_found = false,
//                dateofbirthday_found = false,
//                dateofbirthyear_found = false;
//#if SCRAPE_PLAYWRIGHT
//            var all_input = await page.QuerySelectorAllAsync("input");
//            if (all_input != null)
//            {
//                foreach (var each_input in all_input)
//                {
//                    string name = await each_input.GetAttributeAsync("name");
//#if WINFORMS
//                    //textBoxConsole.AppendText("Text: " + pageInput.TextContent +
//                    //                           " Name: " + pageInput.NameAttribute +
//                    //                            Environment.NewLine.ToString());
//                    //textBoxConsole.ScrollToCaret();
//#endif
//                    switch (name)
//                    {
//                        case "CustomerNumber":
//                            if (!customernumber_found)
//                            {
//                                customernumber_found = true;
//                                await each_input.FillAsync(CustomerNumber);
//                            }
//                            break;
//                        case "DateOfBirthDay":
//                            if (!dateofbirthday_found)
//                            {
//                                dateofbirthday_found = true;
//                                await each_input.FillAsync(Day);
//                            }
//                            break;
//                        case "DateOfBirthYear":
//                            if (!dateofbirthyear_found)
//                            {
//                                dateofbirthyear_found = true;
//                                await each_input.FillAsync(Year);
//                            }
//                            break;
//                        default:
//                            break;
//                    }
//                    if (customernumber_found &&
//                        dateofbirthday_found &&
//                        dateofbirthyear_found)
//                    {
//                        status = true;
//                        break;
//                    }
//                }
//            }
//#endif
//            return status;
//        }

//#if NHTML
//        internal static NHtmlUnit.WebClient webClient;
//
//        internal static async Task<bool> Nationwide_Find_TransactionsOld(
//#if WINFORMS
//                                                    RichTextBox textBoxConsole,
//#endif
//                                                    //SqlConnection SmartFinanceConnection,
//                                                    TestViewModel ourviewmodel,
//                                                    //FinanceViewModel financeviewmodel,
//                                                    //UserNotificationListener listener,
//                                                    string start_url,
//                                                    string notification_title,
//                                                    string notification_prefix,
//                                                    short notification_tag_length,
//                                                    string CustomerNumber,
//                                                    string DOB,
//                                                    string Passcode,
//                                                    Action<string> set_error_message)
//        {
//            if (Passcode.Length < 6)
//            {
//                set_error_message("Invalid Passcode " + Passcode);
//                return false;
//            }
//            if (DOB.Length < 8)
//            {
//                set_error_message("Invalid DOB " + DOB);
//                return false;
//            }
//            string Day = DOB.Substring(0, 2);
//            string Month = SmartParametersV2016.months[Convert.ToInt16(DOB.Substring(2, 2)) - 1];
//            string Year = DOB.Substring(4, 4);

//            //PuppeteerSharp.IBrowser browser = null;
//            try
//            {
//                //using var browserFetcher = new BrowserFetcher();
//#if WINFORMS
//                textBoxConsole.AppendText("BrowserFetcher creation completed" + Environment.NewLine.ToString());
//                textBoxConsole.ScrollToCaret();
//#endif
//                //await browserFetcher.DownloadAsync();
//#if WINFORMS
//                textBoxConsole.AppendText("BrowserFletcher download completed" + Environment.NewLine.ToString());
//                textBoxConsole.ScrollToCaret();
//#endif
//                //browser = await Puppeteer.LaunchAsync(new LaunchOptions { Headless = true });
//#if WINFORMS
//                textBoxConsole.AppendText("Puppeteer LaunchAync completed" + Environment.NewLine.ToString());
//                textBoxConsole.ScrollToCaret();
//#endif

//                //await using var pagex = await browser.NewPageAsync();
//                //await pagex.GoToAsync("https://onlinebanking.nationwide.co.uk"); // In case of fonts being loaded from a CDN, use WaitUntilNavigation.Networkidle0 as a second param.

//                //ScraperClass sc = new ScraperClass();
//                //webClient = ScraperClass.Create();
//                Console.WriteLine("here");
//                //webClient = Class1.Create();
//                if (webClient != null)
//                {
//                    Console.WriteLine("here");
//                }

//                webClient =  new NHtmlUnit.WebClient(BrowserVersion.CHROME, "", 8888);
//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine(ex.Message);
//            }

//            //IPage pagex = await browser.NewPageAsync();
//            //await pagex.GoToAsync("https://onlinebanking.nationwide.co.uk"); // In case of fonts being loaded from a CDN, use WaitUntilNavigation.Networkidle0 as a second param.
//#if WINFORMS
//            textBoxConsole.AppendText("Page goto completed: " + Environment.NewLine.ToString());
//            textBoxConsole.ScrollToCaret();
//#endif

//            //IElementHandle abc = await pagex.WaitForSelectorAsync("#Log in");
//            //if (abc != null)
//            //{
//            //    Console.WriteLine("here");
//            //}
//#if WINFORMS
//            textBoxConsole.AppendText("Looking for : " + Environment.NewLine.ToString());
//            textBoxConsole.ScrollToCaret();
//#endif
//            //await pagex.ClickAsync("#Log in");
//#if WINFORMS
//            textBoxConsole.AppendText("Clicked : " + Environment.NewLine.ToString());
//            textBoxConsole.ScrollToCaret();
//#endif
//            //await pagex.WaitForSelectorAsync(".searchbox input");
//            //await pagex.FocusAsync(".searchbox input");

//            //await pagex.WaitForNavigationAsync();
//#if WINFORMS
//            textBoxConsole.AppendText("Navigation : " + Environment.NewLine.ToString());
//            textBoxConsole.ScrollToCaret();
//#endif
//            //IPage pagey = await browser.NewPageAsync();
//            //var seven = await pagey.EvaluateExpressionAsync<int>("4 + 3");
//            //var someObject = await pagey.EvaluateFunctionAsync<dynamic>("(value) => ({a: value})", 5);
//            //Console.WriteLine(someObject.a);



//            bool cookies_enabled = webClient.CookieManager.IsCookiesEnabled();

//#if WINFORMS
//            textBoxConsole.AppendText("Cookies enabled: " + cookies_enabled + Environment.NewLine.ToString());
//            textBoxConsole.ScrollToCaret();
//#endif
//#if XAMARIN
//            //await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Cookies enabled: " + cookies_enabled);
//#endif
//            webClient.Options.UseInsecureSsl = true; // <= Should be false?
//            webClient.Options.JavaScriptEnabled = true;
//            webClient.Options.CssEnabled = false;
//            webClient.Options.ActiveXNative = true;
//            webClient.Options.RedirectEnabled = true;
//            webClient.Options.ThrowExceptionOnScriptError = false;
//            webClient.Options.ThrowExceptionOnFailingStatusCode = false;


//            HtmlPage page = webClient.GetHtmlPage(start_url);
//            if (page == null)
//            {
//                set_error_message("Failed to retrieve page: " + start_url);
//                return false;
//            }
//#if WINFORMS
//            textBoxConsole.AppendText("Retrieved page: " + start_url + Environment.NewLine.ToString());
//            textBoxConsole.ScrollToCaret();
//#endif
//#if XAMARIN
//            await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Retrieved page: " + start_url);
//#endif
//            // Lets see if I can find all the anchors ...
//#if WINFORMS
//            textBoxConsole.AppendText("Finding anchors... " + Environment.NewLine.ToString());
//            textBoxConsole.ScrollToCaret();
//#endif
//#if XAMARIN
//            await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Finding anchors... ");
//#endif

//            string href = string.Empty;
//            bool customernumber_found = false,
//                 dateofbirthday_found = false,
//                 dateofbirthmonth_found = false,
//                 dateofbirthyear_found = false;

//            // "//iframe[@name='view']");
//            NHtmlUnit.Html.HtmlAnchor mainAnchor = page.GetAnchorByText("Log in") as NHtmlUnit.Html.HtmlAnchor;
//            if (mainAnchor == null)
//            {
//                set_error_message("Failed to retrieve anchor: " + "Log in");
//                return false;
//            }
//#if WINFORMS
//            textBoxConsole.AppendText("Text: " + mainAnchor.TextContent +
//                                    " Id: " + mainAnchor.Id +
//                                    " Href: " + mainAnchor.HrefAttribute +
//                                    Environment.NewLine.ToString());
//            textBoxConsole.ScrollToCaret();
//#endif

//            // Can't 'click' on something that has an href !!!!
//            HtmlPage mainScreen = webClient.GetHtmlPage(mainAnchor.HrefAttribute);
//#if WINFORMS
//            textBoxConsole.AppendText("Retrieved page: " +
//                                        mainAnchor.HrefAttribute +
//                                        Environment.NewLine.ToString());
//            textBoxConsole.ScrollToCaret();
//#endif

//            // This bollocks IS complete and utter fucking twaddle
//            //webClient.WaitForBackgroundJavaScript(1 * 1000); // Experimental API: May be changed in next release and may not yet work perfectly!

//#if XAMARIN
//            await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Finding inputs... ");
//#endif
//            if (!NationwideFindInputs(
//#if WINFORMS
//            textBoxConsole,
//#endif
//                                                CustomerNumber,
//                                                Day,
//                                                Year,
//                                                mainScreen))
//            {
//                set_error_message("Failed to find: Inputs");
//                return false;
//            }
//#if WINFORMS
//            textBoxConsole.AppendText("Finding inputs: " + Environment.NewLine.ToString());
//            textBoxConsole.ScrollToCaret();
//#endif
//#if WINFORMS
//            textBoxConsole.AppendText("Finding 1 selects: " + Environment.NewLine.ToString());
//            textBoxConsole.ScrollToCaret();
//#endif
//            var mainSelects1 = mainScreen.HtmlElementDescendants
//                .OfType<NHtmlUnit.Html.HtmlSelect>().ToList();
//            foreach (NHtmlUnit.Html.HtmlSelect mainSelect in mainSelects1)
//            {
//                if (mainSelect != null)
//                {
//                    if (mainSelect.Id.Contains("selection-list-control"))
//                    {

//                        string name = mainSelect.GetAttribute("Name");
//                        switch (name)
//                        {
//                            case "DateOfBirthMonth":
//                                dateofbirthmonth_found = true;
//                                HtmlOption option = mainSelect.GetOptionByValue(Month);
//                                mainSelect.SetSelectedAttribute(option, true);
//#if WINFORMS
//                                textBoxConsole.AppendText("Name: " + name +
//                                                  //" Text: " + mainSelect.TextContent +
//                                                  " Set to: " + Month +
//                                                    Environment.NewLine.ToString());
//                                textBoxConsole.ScrollToCaret();
//#endif
//                                break;
//                            default:
//                                break;
//                        }

//                    }
//                }
//                if (dateofbirthmonth_found)
//                {
//                    break;
//                }
//            }

//            if (customernumber_found &&
//                dateofbirthday_found &&
//                dateofbirthmonth_found &&
//                dateofbirthyear_found)
//            {
//#if WINFORMS
//                textBoxConsole.AppendText("All good!" +
//                                                            Environment.NewLine.ToString());
//                textBoxConsole.ScrollToCaret();
//#endif
//#if XAMARIN
//                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "All good... ");
//#endif
//                NHtmlUnit.Html.HtmlElement button = (NHtmlUnit.Html.HtmlElement)mainScreen.CreateElement("button");
//                button.SetAttribute("type", "submit");

//                bool post_done = false;

//                var mainForms = mainScreen.HtmlElementDescendants
//                .OfType<NHtmlUnit.Html.HtmlForm>().ToList();
//                foreach (NHtmlUnit.Html.HtmlForm mainForm in mainForms)
//                {
//                    if (mainForm != null)
//                    {
//                        string act = mainForm.GetAttribute("action");
//                        if (act == "/AccessManagement/IdentifyCustomer/EnterCustomerIdentificationDetail")
//                        {
//                            // Submit the form with our 'mocked up' button

//                            mainForm.AppendChild(button);
//                            //NHtmlUnit.Javascript.Host.Events.Event ev = new NHtmlUnit.Javascript.Host.Events.Event. //  ("Submit");
//                            //mainForm.FireEvent(ev);
//                            page = button.Click() as HtmlPage;
//                            post_done = true;
//                            break;
//                        }
//                    }
//                }

//                if (!post_done)
//                {
//                    set_error_message("Post not done");
//                    return false;
//                }

//#if WINFORMS
//                textBoxConsole.AppendText("Passnumbers: " +
//                                                        Passcode.Substring(0, 1) +
//                                                        " " +
//                                                        Passcode.Substring(1, 1) +
//                                                        " " +
//                                                        Passcode.Substring(2, 1) +
//                                                        " " +
//                                                        Passcode.Substring(3, 1) +
//                                                        " " +
//                                                        Passcode.Substring(4, 1) +
//                                                        " " +
//                                                        Passcode.Substring(5, 1) +
//                                                        Environment.NewLine.ToString());
//                textBoxConsole.ScrollToCaret();
//#endif
//#if XAMARIN
//                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Finding passnumbers... ");
//#endif
//                // Look for which pass numbers they want
//                int[] zzz = new int[3];   // So we have 0, 1 and 2
//                var mainSpans = page.HtmlElementDescendants
//                    .OfType<NHtmlUnit.Html.HtmlSpan>().ToList();
//                foreach (NHtmlUnit.Html.HtmlSpan mainSpan in mainSpans)
//                {
//                    if (mainSpan != null)
//                    {
//                        string classname = mainSpan.GetAttribute("class");
//                        if (classname == "control__label__title")
//                        {
//                            int index = 0;
//                            string text = mainSpan.TextContent;
//                            if (text.Contains("digits from your passnumber"))
//                            {
//#if WINFORMS
//                                textBoxConsole.AppendText("Passnumbers reqd: " +
//                                                        text +
//                                                        Environment.NewLine.ToString());
//                                textBoxConsole.ScrollToCaret();
//#endif
//                                if (text.Contains("1st"))
//                                {
//                                    zzz[index] = 0;
//                                    index++;
//                                };
//                                if (text.Contains("2nd"))
//                                {
//                                    zzz[index] = 1;
//                                    index++;
//                                };
//                                if (text.Contains("3rd"))
//                                {
//                                    zzz[index] = 2;
//                                    index++;
//                                };
//                                if (text.Contains("4th"))
//                                {
//                                    zzz[index] = 3;
//                                    index++;
//                                };
//                                if (text.Contains("5th"))
//                                {
//                                    zzz[index] = 4;
//                                    index++;
//                                };
//                                if (text.Contains("6th"))
//                                {
//                                    zzz[index] = 5;
//                                    index++;
//                                };
//                                break;
//                            }
//                        }
//                    }
//                }

//#if WINFORMS
//                textBoxConsole.AppendText("Passnumbers to be sent: " +
//                                                        Passcode.Substring(zzz[0], 1) +
//                                                        " " +
//                                                        Passcode.Substring(zzz[1], 1) +
//                                                        " " +
//                                                        Passcode.Substring(zzz[2], 1) +
//                                                        Environment.NewLine.ToString());
//                textBoxConsole.ScrollToCaret();
//#endif
//#if XAMARIN
//                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Need passnumbers: " +
//                                                        Passcode.Substring(zzz[0], 1) +
//                                                        " " +
//                                                        Passcode.Substring(zzz[1], 1) +
//                                                        " " +
//                                                        Passcode.Substring(zzz[2], 1));
//#endif

//                bool firstpassnumber_found = false,
//                secondpassnumber_found = false,
//                thirdpassnumber_found = false;

//#if WINFORMS
//                textBoxConsole.AppendText("Finding 2 selects: " + Environment.NewLine.ToString());
//                textBoxConsole.ScrollToCaret();
//#endif
//                var mainSelects2 = page.HtmlElementDescendants
//                    .OfType<NHtmlUnit.Html.HtmlSelect>().ToList();
//                foreach (NHtmlUnit.Html.HtmlSelect mainSelect in mainSelects2)
//                {
//                    if (mainSelect != null)
//                    {
//                        if (mainSelect.Id.Contains("selection-list-control"))
//                        {
//                            string name = mainSelect.GetAttribute("Name");
//                            switch (name)
//                            {
//                                case "FirstPassnumberValue":
//                                    firstpassnumber_found = true;
//                                    HtmlOption option1 = mainSelect.GetOptionByValue(Passcode.Substring(zzz[0], 1)); // CHANGE
//                                    mainSelect.SetSelectedAttribute(option1, true);
//#if WINFORMS
//                                    textBoxConsole.AppendText("Name: " + name +
//                                                      //" Text: " + mainSelect.TextContent +
//                                                      " Set to: " + Passcode.Substring(zzz[0], 1) +                // Change
//                                                        Environment.NewLine.ToString());
//                                    textBoxConsole.ScrollToCaret();
//#endif
//                                    break;
//                                case "SecondPassnumberValue":
//                                    secondpassnumber_found = true;
//                                    HtmlOption option2 = mainSelect.GetOptionByValue(Passcode.Substring(zzz[1], 1)); // CHANGE
//                                    mainSelect.SetSelectedAttribute(option2, true);
//#if WINFORMS
//                                    textBoxConsole.AppendText("Name: " + name +
//                                                      //" Text: " + mainSelect.TextContent +
//                                                      " Set to: " + Passcode.Substring(zzz[1], 1) +                // Change
//                                                        Environment.NewLine.ToString());
//                                    textBoxConsole.ScrollToCaret();
//#endif
//                                    break;
//                                case "ThirdPassnumberValue":
//                                    thirdpassnumber_found = true;
//                                    HtmlOption option3 = mainSelect.GetOptionByValue(Passcode.Substring(zzz[2], 1)); // CHANGE
//                                    mainSelect.SetSelectedAttribute(option3, true);
//#if WINFORMS
//                                    textBoxConsole.AppendText("Name: " + name +
//                                                      //" Text: " + mainSelect.TextContent +
//                                                      " Set to: " + Passcode.Substring(zzz[2], 1) +                // Change
//                                                        Environment.NewLine.ToString());
//                                    textBoxConsole.ScrollToCaret();
//#endif
//                                    break;
//                                default:
//                                    break;
//                            }
//                        }
//                    }
//                    if (firstpassnumber_found &&
//                        secondpassnumber_found &&
//                        thirdpassnumber_found)
//                    {
//                        break;
//                    }
//                }
//#if XAMARIN
//                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Passnumbers set... ");
//#endif
//                var passButtons = page.HtmlElementDescendants
//                    .OfType<NHtmlUnit.Html.HtmlButton>().ToList();
//                foreach (NHtmlUnit.Html.HtmlButton passButton in passButtons)
//                {
//                    if (passButton != null)
//                    {
//                        if (passButton.Id == string.Empty &&
//                            passButton.TextContent.Trim() == "Continue")
//                        {
//                            passButton.SetAttribute("type", "submit");
//                            page = passButton.Click() as HtmlPage;
//#if WINFORMS
//                            textBoxConsole.AppendText("Button: " +
//                                                        passButton.TextContent.Trim() +
//                                                        Environment.NewLine.ToString());
//                            textBoxConsole.ScrollToCaret();
//#endif
//                            break;
//                        }
//                    }
//                }

//#if UWP
//                ourviewmodel.notification_listener.NotificationChanged += (sender, e) => Listener_NotificationChanged(sender, e,
//                                                                            ourviewmodel,
//                                                                            notification_title,
//                                                                            notification_prefix,
//                                                                            notification_tag_length,
//                                                                            page,
//                                                                            button);
//#endif


//                // Read the TOKEN HERE!!!
//                TextReader txt = new StreamReader(@"C:\Users\Ray\AppData\Local\SMSOTP.txt");
//                string onetimepasscode = txt.ReadToEnd();
//                txt.Close();

//#if XAMARIN
//                                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Inserting OTP ");
//#endif
//                // Put it into the page
//                var smsInputs = page.HtmlElementDescendants
//                        .OfType<NHtmlUnit.Html.HtmlInput>().ToList();
//                foreach (NHtmlUnit.Html.HtmlInput smsInput in smsInputs)
//                {
//                    if (smsInput != null)
//                    {
//                        string name = smsInput.GetAttribute("name");
//                        if (name == "OneTimePasscode")
//                        {
//                            smsInput.SetAttribute("value", onetimepasscode);
//                            break;
//                        }
//                    }
//                }


//                var pageForms2 = page.HtmlElementDescendants
//                        .OfType<NHtmlUnit.Html.HtmlForm>().ToList();
//                foreach (NHtmlUnit.Html.HtmlForm mainForm2 in pageForms2)
//                {
//                    if (mainForm2 != null)
//                    {
//                        string act2 = mainForm2.GetAttribute("action");
//                        if (act2 == "/AccessManagement/Login/LoginViaPassnumberAndSmsOtp")
//                        {
//                            // Submit the form with our 'mocked up' button

//                            mainForm2.AppendChild(button);
//                            //NHtmlUnit.Javascript.Host.Events.Event ev = new NHtmlUnit.Javascript.Host.Events.Event. //  ("Submit");
//                            //mainForm.FireEvent(ev);
//                            HtmlPage lastpage = button.Click() as HtmlPage;
//                            //string newstringx = lastpage.AsNormalizedText();

//                            break;
//                        }
//                    }
//                }
//            }

//            //NHtmlUnit.Javascript.Host.Events.Event ev = new NHtmlUnit.Javascript.Host.Events.Event. //  ("Submit");
//            //mainForm.FireEvent(ev);

//            // Thanks to mirovarga
//            // https://stackoverflow.com/questions/7573558/htmlunit-how-to-post-form-without-clicking-submit-button



//            // Lets try and get all the other shit ...

//            return true;
//        }
//#endif

//#if WINFORMS || WINDOWS_UWP || XAMARIN
//        internal static async void Listener_NotificationChanged(UserNotificationListener sender, 
//                                                        UserNotificationChangedEventArgs args, 
//                                                        MainViewModel ourviewmodel,
//                                                        string notification_title,
//                                                        string notification_prefix,
//                                                        short notification_tag_length,
//                                                        HtmlPage page,
//                                                        NHtmlUnit.Html.HtmlElement button)
//        {
//            // Get the toast notifications
//            IReadOnlyList<UserNotification> notifs = await sender.GetNotificationsAsync(NotificationKinds.Toast);

//            // Select the first notification
//            foreach (UserNotification usernot in notifs)
//            {
//                UserNotification notif = usernot;

//                // Get the toast binding, if present
//                NotificationBinding toastBinding = notif.Notification.Visual.GetBinding(KnownNotificationBindings.ToastGeneric);

//                if (toastBinding != null)
//                {
//                    // And then get the text elements from the toast binding
//                    IReadOnlyList<AdaptiveNotificationText> textElements = toastBinding.GetTextElements();

//                    // Treat the first text element as the title text
//                    string titleText = textElements.FirstOrDefault()?.Text;
//                    if (notification_title == titleText)
//                    {
//#if XAMARIN
//                        await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, " Found! " + titleText);
//#endif
//                        // We'll treat all subsequent text elements as body text,
//                        // joining them together via newlines.
//                        string bodyText = string.Join("\n", textElements.Skip(1).Select(t => t.Text));
//                        if (bodyText.Contains(notification_prefix))
//                        {
//                            bodyText = bodyText.Replace(notification_prefix, string.Empty).Trim(); //Trim gets rid of any spaces
//                            if (bodyText.Length > 6)
//                            {
//                                string notificationContent = bodyText.Substring(0, notification_tag_length);

//#if XAMARIN
//                                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Inserting OTP ");
//#endif
//                                // Put it into the page
//                                var smsInputs = page.HtmlElementDescendants
//                                        .OfType<NHtmlUnit.Html.HtmlInput>().ToList();
//                                foreach (NHtmlUnit.Html.HtmlInput smsInput in smsInputs)
//                                {
//                                    if (smsInput != null)
//                                    {
//                                        string name = smsInput.GetAttribute("name");
//                                        if (name == "OneTimePasscode")
//                                        {
//                                            smsInput.SetAttribute("value", notificationContent);
//                                            break;
//                                        }
//                                    }
//                                }


//                                var pageForms2 = page.HtmlElementDescendants
//                                        .OfType<NHtmlUnit.Html.HtmlForm>().ToList();
//                                foreach (NHtmlUnit.Html.HtmlForm mainForm2 in pageForms2)
//                                {
//                                    if (mainForm2 != null)
//                                    {
//                                        string act2 = mainForm2.GetAttribute("action");
//                                        if (act2 == "/AccessManagement/Login/LoginViaPassnumberAndSmsOtp")
//                                        {
//                                            // Submit the form with our 'mocked up' button

//                                            mainForm2.AppendChild(button);
//                                            //NHtmlUnit.Javascript.Host.Events.Event ev = new NHtmlUnit.Javascript.Host.Events.Event. //  ("Submit");
//                                            //mainForm.FireEvent(ev);
//                                            HtmlPage lastpage = button.Click() as HtmlPage;
//                                            //string newstringx = lastpage.AsNormalizedText();

//                                            break;
//                                        }
//                                    }
//                                }
//                            }

//                            //NHtmlUnit.Javascript.Host.Events.Event ev = new NHtmlUnit.Javascript.Host.Events.Event.   //  ("Submit");
//                            //mainForm.FireEvent(ev);

//                            // Thanks to mirovarga
//                            // https://stackoverflow.com/questions/7573558/htmlunit-how-to-post-form-without-clicking-submit-button


//                            // AT THIS POINT WE SHOULD BE ABLE TO READ THE FUCKING ACCOUNT??
//#if XAMARIN
//                            await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Have we got an Accounts List??? ");
//#endif
//                            // Lets try and get all the other shit ...

//                        }
//                    }
//                    break;                    
//                }
//            }
//        }

//        public sealed class UserNotifications
//        {
//            public AppInfo AppInfo { get; }
//            public DateTimeOffset CreationTime { get; }
//            public uint Id { get; }
//            public Notification Notification { get; }
//        }

//#endif




//        internal static async Task<bool> NationWide_Find_Accounts(
//#if WINFORMS
//                                            RichTextBox textBoxConsole,
//#endif
//                                            MainViewModel ourviewmodel,
//                                            FinanceViewModel financeviewmodel,
//                                            string url_first,   // nationwide.co.uk
//                                            bool log,
//                                            string referer,
//                                            string customer_no,
//                                            string dob,
//                                            string passcode,
//                                            string memorable_data)
//        {
//            string url_base = string.Empty;     // Which we try to deduce!

//            string inner_text = string.Empty,
//                        acct_type_tag = string.Empty,
//                        account_address = string.Empty,
//                        classname = string.Empty,
//                        href = string.Empty;
//            bool found_it = false;

//            financeviewmodel.type = string.Empty;
//            financeviewmodel.response = string.Empty;

//            Uri url = new Uri(url_first);   // nationwide.co.uk

//            financeviewmodel.html_document = new HtmlAgilityPack.HtmlDocument();

//#if WINFORMS
//            textBoxConsole.AppendText("Sending: " + url +
//                                            Environment.NewLine.ToString());
//            textBoxConsole.ScrollToCaret();
//#endif
//            if (await SmartBanksV2019.DoGet_Finance(
//                                                ourviewmodel,
//                                                financeviewmodel,
//                                                financeviewmodel.finance_token,
//                                                url,        // nationwide.co.uk
//                                                url_first,  // nationwide.co.uk
//                                                log,
//                                                "GET",
//                                                referer))

//            {
//                //< nav aria - label = "Log in" class="DropdownNav-fzf12b-0 ihYshF LoginFlyout__UnposedLoginFlyout-uiiphf-1 kUBlCj LoginFlyout__MobileTabletLoginFlyout-uiiphf-2 eNaqE" 
//                //  data-analytics-identifier="login flyout" 
//                // data-analytics-context="true">
//                // <div class="VStack-sc-186pbzy-0 LoginFlyout__UnposedNavItem-uiiphf-0 cqeycc">
//                // <div class="TopRow__TopRowFlexContainer-x2n9g4-0 dpiQRV">
//                // <h2 class="NelComponents__Heading-vsly48-4 FYxCH nel-Typography-454 nel-Typography-449" data-ref="heading">Internet Banking</h2>
//                // <a class="LoginLinks__ShortLoginLink-myoff6-1 iBIaJC nel-Link-6 nel-Link-5" 
//                // data-ref="link" 
//                // href="https://onlinebanking.nationwide.co.uk/AccessManagement/IdentifyCustomer/IdentifyCustomer">
//#if WINFORMS
//                textBoxConsole.AppendText("Checking: " + "LoginLink" +
//                                            Environment.NewLine.ToString());
//                textBoxConsole.ScrollToCaret();
//#endif

//                if (!Look_For_LoginLink(financeviewmodel, ref href, ref url_base))
//                {
//#if WINFORMS
//                    textBoxConsole.AppendText("Problem: " + "LoginLink" +
//                                            Environment.NewLine.ToString());
//                    textBoxConsole.ScrollToCaret();
//#endif
//                    return false;
//                }

//                // Now try to extract the REAL url_base i.e.      
//                if (string.IsNullOrEmpty(href) ||
//                    string.IsNullOrEmpty(url_base))
//                {
//#if WINFORMS
//                    textBoxConsole.AppendText("Problem: " + "href or url_base" +
//                                            Environment.NewLine.ToString());
//                    textBoxConsole.ScrollToCaret();
//#endif
//                    return false;
//                }

//#if WINFORMS
//                textBoxConsole.AppendText("Found href: " + href +
//                                        Environment.NewLine.ToString());
//                textBoxConsole.ScrollToCaret();
//                textBoxConsole.AppendText("Setting url_base: " + url_base +
//                                    Environment.NewLine.ToString());
//                textBoxConsole.ScrollToCaret();
//#endif
//                found_it = true;
//                Uri next_url = new Uri(href);   // "https://onlinebanking.nationwide.co.uk/AccessManagement/IdentifyCustomer/IdentifyCustomer"


//#if WINFORMS
//                textBoxConsole.AppendText("Sending: " + next_url +
//                                    Environment.NewLine.ToString());
//                textBoxConsole.ScrollToCaret();
//#endif
//                if (!await SmartBanksV2019.DoGet_Finance(
//                                        ourviewmodel,
//                                        financeviewmodel,
//                                        financeviewmodel.finance_token,
//                                        next_url,
//                                        url_base,
//                                        log,
//                                        "GET",
//                                        referer))
//                {
//#if WINFORMS
//                    textBoxConsole.AppendText("Problem: " + next_url +
//                                            Environment.NewLine.ToString());
//                    textBoxConsole.ScrollToCaret();
//#endif
//                    return false;
//                }
//#if WINFORMS
//                textBoxConsole.AppendText("Checking: " + "__token" +
//                                            Environment.NewLine.ToString());
//                textBoxConsole.ScrollToCaret();
//#endif
//                string customerid_url = string.Empty;
//                string customerid_token = Nationwide_Find_Form(financeviewmodel.html_document,
//                                                "accessmanagementidentifycustomerentercustomeridentificationdetail",
//                                                "__token",
//                                                ref customerid_url);
//                if (string.IsNullOrEmpty(customerid_token) ||
//                    string.IsNullOrEmpty(customerid_url))
//                {
//#if WINFORMS
//                    textBoxConsole.AppendText("Problem: " + "token or url_access_management" +
//                                            Environment.NewLine.ToString());
//                    textBoxConsole.ScrollToCaret();
//#endif
//                    return false;
//                }
//#if WINFORMS
//                textBoxConsole.AppendText("Found token: " + customerid_token +
//                                        Environment.NewLine.ToString());
//                textBoxConsole.ScrollToCaret();
//                textBoxConsole.AppendText("Setting url: " + url_base +
//                                    customerid_url +
//                                    Environment.NewLine.ToString());
//                textBoxConsole.ScrollToCaret();
//#endif
//                // Now try and POST with the Customer Number, Date of Birth and token
//                string urgent_message = string.Empty;
//                if (!SmartLoginV2016.Create_Uri(url_base, customerid_url,
//                    ref next_url,
//                    ref urgent_message))
//                {
//#if WINFORMS
//                    // Bad news ...
//                    textBoxConsole.AppendText("Error: " + urgent_message +
//                            Environment.NewLine.ToString());
//                    textBoxConsole.ScrollToCaret();
//#endif
//                    return false;
//                }

//                string day = string.Empty,
//                       month = string.Empty,
//                       year = string.Empty;
//                if (!SplitDob(dob, ref day, ref month, ref year))
//                {
//#if WINFORMS
//                    // Bad Birthday news ...
//                    textBoxConsole.AppendText("Error: " + dob +
//                            Environment.NewLine.ToString());
//                    textBoxConsole.ScrollToCaret();
//#endif
//                    return false;
//                }

//                string now = SmartEncryptionV2016.DateTimeNow(ourviewmodel.gmt_offset).ToString("ddd MMM dd yyyy hh:mm:ss"); // Doesn't appear to be 24 hr time - problem?
//                                                                                                                             //now = now + (" GMT+0000 (GMT Standard Time)");
//                now = now + " GMT+0000 " + "(" + "GMT Standard Time" + ")";

//                string parameters = "__token" + "|" + customerid_token + SmartParametersV2016.record_separator +
//                                    "PersistentCookiesEnabled" + "|" + "true" + SmartParametersV2016.record_separator +
//                                    "ScreenResolution" + "|" + "1536 x 864" + SmartParametersV2016.record_separator +
//                                    "TimeZoneOffset" + "|" + "1" + SmartParametersV2016.record_separator +
//                                    "LocalTime" + "|" + now + SmartParametersV2016.record_separator +
//                                    "CustomerNumber" + "|" + customer_no + SmartParametersV2016.record_separator +
//                                    "RememberMe" + "|" + "false" + SmartParametersV2016.record_separator +
//                                    "SpecifiedStartPage" + "|" + "" + SmartParametersV2016.record_separator +
//                                    "DateOfBirthDay" + "|" + day + SmartParametersV2016.record_separator +
//                                    "DateOfBirthMonth" + "|" + month + SmartParametersV2016.record_separator +
//                                    "DateOfBirthYear" + "|" + year;
//                var not_empty1 = SmartBanksV2019.Common_Build_Form_Strings_New(parameters);
//                financeviewmodel.response = string.Empty;
//#if WINFORMS
//                textBoxConsole.AppendText("Sending: CustomerId parameters" +
//                            Environment.NewLine.ToString());
//                textBoxConsole.ScrollToCaret();
//#endif
//                if (!await SmartBanksV2019.DoPost_NewX(
//                                ourviewmodel,
//                                financeviewmodel,
//                                financeviewmodel.finance_token,
//                                next_url,
//                                url_base,
//                                log,
//                                0,
//                                "POST",
//                                not_empty1,
//                                referer))
//                {
//#if WINFORMS
//                    textBoxConsole.AppendText("Problem: " + "Post CustomerId and DOB" +
//                                            Environment.NewLine.ToString());
//                    textBoxConsole.ScrollToCaret();
//#endif
//                    return false;
//                }

//                //PrintCookies(ourviewmodel.cookies, textBoxConsole);
//                //if (ourviewmodel.cookies.Count > 0)
//                //{
//                //    return true;
//                //}
//                string servicing = WebUtility.UrlEncode("ib:servicing:login:login method:passnumber and code by text");
//                servicing = servicing.Replace("+", "%20");
//                Cookie gpv_v19 = new Cookie()
//                {
//                    Name = "gpv_v19",
//                    Value = servicing,
//                    Path = "/",
//                    Domain = "onlinebanking.nationwide.co.uk"
//                };
//                ourviewmodel.cookies.Add(gpv_v19);
//                // Now try and find the OTP token which is hidden in the previous response!
//                string passnumberandsmsotp_url = string.Empty;
//                string passnumberandsmsotp_token = Nationwide_Find_Form(financeviewmodel.html_document,
//                                                "accessmanagementloginloginviapassnumberandsmsotp", // Id
//                                                "__token",                          // name
//                                                ref passnumberandsmsotp_url);
//                if (string.IsNullOrEmpty(passnumberandsmsotp_token) ||
//                    string.IsNullOrEmpty(passnumberandsmsotp_url))
//                {
//#if WINFORMS
//                    textBoxConsole.AppendText("Problem: " + "PassNumbers token or url" +
//                                            Environment.NewLine.ToString());
//                    textBoxConsole.ScrollToCaret();
//#endif
//                    return false;
//                }

//                string SendSmsCanaryTokenValue_url = string.Empty;
//                string sendsmscanary_token = Nationwide_Find_Form(financeviewmodel.html_document, "SendSmsCanaryTokenValue",
//                                                "SendSmsCanaryTokenValue",
//                                                ref SendSmsCanaryTokenValue_url);
//                if (string.IsNullOrEmpty(sendsmscanary_token) ||
//                    string.IsNullOrEmpty(SendSmsCanaryTokenValue_url))
//                {
//#if WINFORMS
//                    textBoxConsole.AppendText("Problem: " + "SendSmsCanary token or url" +
//                                            Environment.NewLine.ToString());
//                    textBoxConsole.ScrollToCaret();
//#endif
//                    return false;
//                }

//                // Cassie Speck <= Sissy Spacek!!! ODIWHTLTTB

//                // Jennifer Anson < Aniston

//                string passnumbers_token = Nationwide_Find_Token(financeviewmodel.html_document,
//                                                                "SmsOtp_passNoEntryPositions");
//                if (string.IsNullOrEmpty(passnumbers_token))
//                {
//#if WINFORMS
//                    textBoxConsole.AppendText("Problem: " + "Passnumbers token" +
//                                            Environment.NewLine.ToString());
//                    textBoxConsole.ScrollToCaret();
//#endif
//                    return false;
//                }
//                passnumbers_token = passnumbers_token.TrimEnd(SmartParametersV2016.commachar);
//                string[] passnumbers = passnumbers_token.Split(SmartParametersV2016.commachar);
//                if (passnumbers.Length != 3)
//                {
//#if WINFORMS
//                    // Bad news ...
//                    textBoxConsole.AppendText("Error: " + passnumbers_token +
//                            Environment.NewLine.ToString());
//                    textBoxConsole.ScrollToCaret();
//#endif
//                    return false;
//                }
//                string firstpassnumber = passcode.Substring(Convert.ToInt16(passnumbers[0]) - 1, 1);
//                string secondpassnumber = passcode.Substring(Convert.ToInt16(passnumbers[1]) - 1, 1);
//                string thirdpassnumber = passcode.Substring(Convert.ToInt16(passnumbers[2]) - 1, 1);

//                // Build the parameters to send the token
//                parameters = "__token" + "|" + sendsmscanary_token;
//                not_empty1 = SmartBanksV2019.Common_Build_Form_Strings_New(parameters);
//                financeviewmodel.response = string.Empty;

//                if (!SmartLoginV2016.Create_Uri(url_base, "AccessManagement/Login/SendSMSOTP",
//                    ref next_url,
//                    ref urgent_message))
//                {
//#if WINFORMS
//                    // Bad news ...
//                    textBoxConsole.AppendText("Error: " + urgent_message +
//                            Environment.NewLine.ToString());
//                    textBoxConsole.ScrollToCaret();
//#endif
//                    return false;
//                }
//                // Now send the token to get the OTP on your phone
//                if (!await SmartBanksV2019.DoPost_NewX(
//                                ourviewmodel,
//                                financeviewmodel,
//                                financeviewmodel.finance_token,
//                                next_url,   // https://onlinebanking.nationwide.co.uk/AccessManagement/Login/SendSMSOTP
//                                url_base,
//                                log,
//                                1,
//                                "POST",
//                                not_empty1,
//                                "https://onlinebanking.nationwide.co.uk/AccessManagement/Login/IdentifyCustomerForLogin"
//                                ))//"https://onlinebanking.nationwide.co.uk"))
//                {
//#if WINFORMS
//                    textBoxConsole.AppendText("Problem: " + "Access Token" +
//                                            Environment.NewLine.ToString());
//                    textBoxConsole.ScrollToCaret();
//#endif
//                    return false;
//                }

//                TextReader txt1 = new StreamReader(@"C:\Users\Ray\AppData\Local\SMSOTPEXPIRED.txt");
//                string one = txt1.ReadToEnd();
//                txt1.Close();

//                txt1 = new StreamReader(@"C:\Users\Ray\AppData\Local\OTPRESENDSLIMITREACHEDHELPHINTSTITLE.txt");
//                string two = txt1.ReadToEnd();
//                txt1.Close();

//                txt1 = new StreamReader(@"C:\Users\Ray\AppData\Local\OTPRESENDSLIMITREACHEDHELPHINTS.txt");
//                string three = txt1.ReadToEnd();
//                txt1.Close();

//                txt1 = new StreamReader(@"C:\Users\Ray\AppData\Local\OTPEXPIREDCONTENT.txt");
//                string four = txt1.ReadToEnd();
//                txt1.Close();
//                string SmsResendsRemaining = string.Empty;
//                string NemOtpExpiryTime = string.Empty;
//                if (!NW_Get_SMSOTP_String(financeviewmodel.response,
//                                                    ref SmsResendsRemaining,
//                                                    ref NemOtpExpiryTime))
//                {
//#if WINFORMS
//                    textBoxConsole.AppendText("Problem: " + "SMSOTP response" +
//                                            Environment.NewLine.ToString());
//                    textBoxConsole.ScrollToCaret();
//#endif
//                    return false;
//                }

//                TextReader txt = new StreamReader(@"C:\Users\Ray\AppData\Local\SMSOTP.txt");
//                string onetimepasscode = txt.ReadToEnd();
//                txt.Close();

//                now = SmartEncryptionV2016.DateTimeNow(ourviewmodel.gmt_offset).ToString("ddd MMM dd yyyy hh:mm:ss"); // Doesn't appear to be 24 hr time - problem?                                                                                                                      //now = now + (" GMT+0000 (GMT Standard Time)");
//                // ?????? 0100 and not 0000??
//                now = now + " GMT+0100";// + "(" + "GMT Standard Time" + ")";



//                parameters = "__token" + "|" + passnumberandsmsotp_token + SmartParametersV2016.record_separator +
//                            "PersistentCookiesEnabled" + "|" + "true" + SmartParametersV2016.record_separator +
//                            "ScreenResolution" + "|" + "1536 x 864" + SmartParametersV2016.record_separator +
//                            "TimeZoneOffset" + "|" + "1" + SmartParametersV2016.record_separator +
//                            "LocalTime" + "|" + now + SmartParametersV2016.record_separator +
//                            "IsOneTimePasscodeRequired" + "|" + "true" + SmartParametersV2016.record_separator +
//                            "SmsResendsRemaining" + "|" + SmsResendsRemaining + SmartParametersV2016.record_separator +
//                            "NemOtpExpiryTime" + "|" + NemOtpExpiryTime + SmartParametersV2016.record_separator +
//                            "SendSmsCanaryTokenValue" + "|" + sendsmscanary_token + SmartParametersV2016.record_separator +
//                            "SmsOtpCreated" + "|" + "False" + SmartParametersV2016.record_separator +
//                            "SmsOtpExpired" + "|" + one + SmartParametersV2016.record_separator +
//                            "otp-resends-limit-reached-help-hints-title" + "|" + two + SmartParametersV2016.record_separator +
//                            "otp-resends-limit-reached-help-hints" + "|" + three + SmartParametersV2016.record_separator +
//                            "OtpExpiredContent" + "|" + four + SmartParametersV2016.record_separator +
//                            "FirstPassnumberValue" + "|" + firstpassnumber + SmartParametersV2016.record_separator +
//                            "SecondPassnumberValue" + "|" + secondpassnumber + SmartParametersV2016.record_separator +
//                            "ThirdPassnumberValue" + "|" + thirdpassnumber + SmartParametersV2016.record_separator +
//                            "OneTimePasscode" + "|" + onetimepasscode;

//                // Now try and POST to get the digits
//                Uri login_url = new Uri(url_base + passnumberandsmsotp_url);


//                not_empty1 = SmartBanksV2019.Common_Build_Form_Strings_New(parameters);
//                financeviewmodel.response = string.Empty;


//                if (!await SmartBanksV2019.DoPost_NewX(
//                                ourviewmodel,
//                                financeviewmodel,
//                                financeviewmodel.finance_token,
//                                login_url,
//                                url_base,
//                                log,
//                                1,
//                                "POST",
//                                not_empty1,
//                                "https://onlinebanking.nationwide.co.uk/AccessManagement/Login/IdentifyCustomerForLogin",
//                                "https://onlinebanking.nationwide.co.uk"))

//                {
//#if WINFORMS
//                    textBoxConsole.AppendText("Problem: " + "Login" +
//                                            Environment.NewLine.ToString());
//                    textBoxConsole.ScrollToCaret();
//#endif
//                }

//#if WINFORMS
//                textBoxConsole.AppendText("Success: " + "Login" +
//                                            Environment.NewLine.ToString());
//                textBoxConsole.ScrollToCaret();
//#endif
//                //if (NW_Get_Json_String(ref financeviewmodel.response))
//                //{
//                //    await SmartRoutinesV2018.TextBlockUpdate(
//                //                                        ourviewmodel,
//                //                                        "Received passnumber positions");

//                //    string FirstPassnumberValue = string.Empty,
//                //            SecondPassnumberValue = string.Empty,
//                //            ThirdPassnumberValue = string.Empty;
//                //    financeviewmodel.response = financeviewmodel.response.Replace("{", string.Empty);
//                //    financeviewmodel.response = financeviewmodel.response.Replace("}", string.Empty);
//                //    financeviewmodel.response = financeviewmodel.response.Replace("\"", string.Empty);
//                //    financeviewmodel.response = financeviewmodel.response.Replace(":", string.Empty);
//                //    financeviewmodel.response = financeviewmodel.response.Replace(" ", string.Empty);
//                //    financeviewmodel.response = financeviewmodel.response.Replace("st", string.Empty);
//                //    financeviewmodel.response = financeviewmodel.response.Replace("nd", string.Empty);
//                //    financeviewmodel.response = financeviewmodel.response.Replace("rd", string.Empty);
//                //    financeviewmodel.response = financeviewmodel.response.Replace("th", string.Empty);
//                //    int pass_count = 0;
//                //    string[] positions = financeviewmodel.response.Split(',');
//                //    foreach (string position in positions)
//                //    {
//                //        short index = Convert.ToInt16(position);
//                //        if (index > 0)
//                //        {
//                //            index = (short)(index - 1);
//                //            switch (pass_count)
//                //            {
//                //                case 0:
//                //                    FirstPassnumberValue = passcode.Substring(index, 1);
//                //                    break;
//                //                case 1:
//                //                    SecondPassnumberValue = passcode.Substring(index, 1);
//                //                    break;
//                //                case 2:
//                //                    ThirdPassnumberValue = passcode.Substring(index, 1);
//                //                    break;
//                //                default:
//                //                    break;
//                //            }
//                //        }
//                //        pass_count++;
//                //    }
//                //    now = SmartEncryptionV2016.DateTimeNow(ourviewmodel.gmt_offset).ToString("ddd MMM dd yyyy hh:mm:ss"); // Doesn't appear to be 24 hr time - problem?
//                //                                                                                                          //now = now + (" GMT+0000 (GMT Standard Time)");
//                //    now = now + " GMT+0000 " + "(" + "GMT Standard Time" + ")";

//                //string data = "__token" + "|" + token + SmartParametersV2016.record_separator +
//                //            "PersistentCookiesEnabled" + "|" + "true" + SmartParametersV2016.record_separator +
//                //            "ScreenResolution" + "|" + "1280 x 768" + SmartParametersV2016.record_separator +
//                //            "TimeZoneOffset" + "|" + "0" + SmartParametersV2016.record_separator +
//                //            "LocalTime" + "|" + now + SmartParametersV2016.record_separator +
//                //            "SpecifiedStartPage" + "|" + "" + SmartParametersV2016.record_separator +
//                //            "PassnumberDigitsLoaded" + "|" + "true" + SmartParametersV2016.record_separator +
//                //            "CustomerNumber" + "|" + customer_no + SmartParametersV2016.record_separator +
//                //            "RememberCustomerNumber" + "|" + "false" + SmartParametersV2016.record_separator +
//                //            "MemorableData" + "|" + memorable_data + SmartParametersV2016.record_separator +
//                //            "FirstPassnumberValue" + "|" + FirstPassnumberValue + SmartParametersV2016.record_separator +
//                //            "SecondPassnumberValue" + "|" + SecondPassnumberValue + SmartParametersV2016.record_separator +
//                //            "ThirdPassnumberValue" + "|" + ThirdPassnumberValue;
//                //StringContent not_empty2 = SmartBanksV2019.Common_Build_Form_Strings(data);
//                //financeviewmodel.response = string.Empty;
//                //Uri login_url = new Uri(url_base + "/AccessManagement/Login/LoginViaMemorableDataAndPassnumber");

//                //await SmartRoutinesV2018.TextBlockUpdate(
//                //                                    ourviewmodel,
//                //                                    "Sending passnumbers and memorable data");


//                //if (await SmartBanksV2019.DoPost_NewX(
//                //        ourviewmodel,
//                //        financeviewmodel,
//                //        financeviewmodel.finance_token,
//                //        login_url,
//                //        url_base,
//                //        log,
//                //        1,
//                //        "POST",
//                //        not_empty2,
//                //        referer))

//                //{

//                await SmartRoutinesV2018.TextBlockUpdate(
//                                                    ourviewmodel,
//                                                    "Receiving accounts list response");


//                // Should be able to get the list of accounts NOW!!
//                // And I fucking DID!!  (Now, if only I could find my fucking keys ...)
//                string accounts = string.Empty;
//#if WINFORMS
//                List<FinanceViewModel.AccountItem> these_accounts = new List<FinanceViewModel.AccountItem>();
//#else
//                List<FinanceViewModel.AccountItem> these_accounts = new List<FinanceViewModel.AccountItem>();
//#endif

//                string owner = string.Empty;
//                Nationwide_Build_Account_List(financeviewmodel.html_document, ref owner, ref accounts);
//                financeviewmodel.owner = owner;

//                if (!string.IsNullOrEmpty(accounts))
//                {
//                    string[] accounts_list = accounts.Split(SmartParametersV2016.record_separator);
//                    if (accounts_list.Count > 0)
//                    {
//                        string account_name = string.Empty,
//                                account_number = string.Empty,
//                                sort_code = string.Empty,
//                                balance = string.Empty;

//                        await SmartRoutinesV2018.TextBlockUpdate(
//                                                            ourviewmodel,
//                                                            "Found " + accounts_list.Length + " accounts");
//                        int account_id = 0;
//                        foreach (string account in accounts_list)
//                        {
//                            account_id++;
//                            string[] comp = account.Split(SmartParametersV2016.unit_separator);
//                            if (comp.Count > 1)
//                            {
//                                string account_token = comp[0];
//                                if (SmartFinanceV2021.Backwards_Split(ref account_token, SmartParametersV2016.space, ref balance))
//                                {
//                                    if (SmartFinanceV2021.Backwards_Split(ref account_token, SmartParametersV2016.space, ref account_number))
//                                    {
//                                        // Well .. there may not be a Sort Code!!!
//                                        // Is the last character a digit?
//                                        char last_charx = Convert.ToChar(account_token.Substring(account_token.Length - 1, 1));
//                                        if (Char.IsDigit(last_charx))
//                                        {
//                                            if (SmartFinanceV2021.Backwards_Split(ref account_token, SmartParametersV2016.space, ref sort_code))
//                                            {
//                                                // Does this set the Account Type?????
//                                                last_charx = SmartParametersV2016.singleaccounttype;
//                                            }
//                                        }
//                                        account_name = account_token;
//                                        // Be Bold!!!
//                                        sort_code = sort_code.Replace("-", string.Empty);
//                                        string val = sort_code + SmartParametersV2016.unit_separator +
//                                                        account_number +
//                                                        ":" +
//                                                        "GBP" +     // Assume we can find it in OpenBanking
//                                                        ":" +
//                                                        balance +
//                                                        ":" +
//                                                        comp[1].Replace("&amp;", "&");    // Safety check
//                                        string[] array = val.Split(':');
//#if WINFORMS
//                                        FinanceViewModel.AccountItem account_item = new FinanceViewModel.AccountItem
//#else
//                                        FinanceViewModel.AccountItem account_item = new FinanceViewModel.AccountItem
//#endif
//                                        {
//                                            Account_ID = account_id,
//                                            Content = account_name,
//                                            Value = array.ToString() // Here we have [0] = sort_code and accoount_no
//                                                                     //             [1] = balance
//                                                                     //              [2] = href
//                                        };
//                                        these_accounts.Add(account_item);

//                                    }
//                                }
//                            }
//                        }
//                        financeviewmodel.FinanceAccounts_List = these_accounts;
//                    }
//                }



//                if (!found_it)
//                {

//                    await SmartRoutinesV2018.TextBlockUpdate(
//                                                        ourviewmodel,
//                                                        "Not found");
//                }
//            }
//            return true;
//        }

//        // Paste this dependencies in your class
//        /// <summary>
//        /// It prints all cookies in a CookieContainer. Only for testing.
//        /// </summary>
//        /// <param name="cookieJar">A cookie container</param>
//        public static void PrintCookies(
//#if WINFORMS
//                                        RichTextBox textBoxConsole,
//#endif
//                                        CookieContainer cookieJar
//                                        )
//        {
//            try
//            {
//                var cookies = new List<Cookie>();

//                var table = (Hashtable)cookieJar.GetType().InvokeMember("m_domainTable",
//                                                                        BindingFlags.NonPublic |
//                                                                        BindingFlags.GetField |
//                                                                        BindingFlags.Instance,
//                                                                        null,
//                                                                        cookieJar,
//                                                                        new object[] { });

//#if WINFORMS
//                textBoxConsole.AppendText(cookieJar.Count + " HTTP COOKIES FOUND:" +
//                Environment.NewLine.ToString());
//                textBoxConsole.ScrollToCaret();
//                textBoxConsole.AppendText("----------------------------------" +
//                Environment.NewLine.ToString());
//                textBoxConsole.ScrollToCaret();
//#endif

//                foreach (var key in table.Keys)
//                {

//                    Uri uri = null;

//                    var domain = key as string;

//                    if (domain == null)
//                        continue;

//                    if (domain.StartsWith("."))
//                        domain = domain.Substring(1);

//                    var address = string.Format("http://{0}/", domain);

//                    if (Uri.TryCreate(address, UriKind.RelativeOrAbsolute, out uri) == false)
//                        continue;

//                    foreach (Cookie cookie in cookieJar.GetCookies(uri))
//                    {
//                        //cookies.Add(cookie);
//#if WINFORMS
//                        textBoxConsole.AppendText(cookie.Name + " " + cookie.Domain + " " + cookie.Path +
//                                Environment.NewLine.ToString());
//                        textBoxConsole.ScrollToCaret();
//#endif
//                    }
//                }
//            }
//            catch (Exception e)
//            {
//#if WINFORMS
//                //Console.WriteLine(e);
//                textBoxConsole.AppendText(e.Message +
//                        Environment.NewLine.ToString());
//                textBoxConsole.ScrollToCaret();
//#endif
//            }



//            //try
//            //{
//            //    Hashtable table = (Hashtable)cookieJar
//            //        .GetType().InvokeMember("m_domainTable",
//            //        BindingFlags.NonPublic |
//            //        BindingFlags.GetField |
//            //        BindingFlags.Instance,
//            //        null,
//            //        cookieJar,
//            //        new object[] { });


//            //    foreach (var key in table.Keys)
//            //    {
//            //        // Look for http cookies.
//            //        string url = string.Format("http://{0}/", key);
//            //        //string rays = key.ToString();
//            //        //Console.WriteLine(rays);
//            //        if (cookieJar.GetCookies(
//            //            new Uri(url)).Count > 0)
//            //        {
//            //            //Console.WriteLine(cookieJar.Count + " HTTP COOKIES FOUND:");
//            //            //Console.WriteLine("----------------------------------");
//            //            textBoxConsole.AppendText(cookieJar.Count + " HTTP COOKIES FOUND:" +
//            //            Environment.NewLine.ToString());
//            //            textBoxConsole.ScrollToCaret();
//            //            textBoxConsole.AppendText("----------------------------------" +
//            //            Environment.NewLine.ToString());
//            //            textBoxConsole.ScrollToCaret();

//            //            foreach (Cookie cookie in cookieJar.GetCookies(new Uri("http://onlinebanking.nationwide.co.uk")))
//            //                //new Uri(string.Format("http://{0}/", key))))
//            //            {
//            //                //Console.WriteLine(
//            //                //    "Name = {0} ; Value = {1} ; Domain = {2}",
//            //                //    cookie.Name, cookie.Value, cookie.Domain);
//            //                string details = cookie.Name + " " + cookie.Domain + " " + cookie.Path;
//            //                textBoxConsole.AppendText(details +
//            //                        Environment.NewLine.ToString());
//            //                textBoxConsole.ScrollToCaret();
//            //            }
//            //        }

//            //        // Look for https cookies
//            //        if (cookieJar.GetCookies(
//            //            new Uri(string.Format("https://{0}/", key))).Count > 0)
//            //        {
//            //            //Console.WriteLine(cookieJar.Count + " HTTPS COOKIES FOUND:");
//            //            //Console.WriteLine("----------------------------------");
//            //            textBoxConsole.AppendText(cookieJar.Count + " HTTPS COOKIES FOUND:" +
//            //            Environment.NewLine.ToString());
//            //            textBoxConsole.ScrollToCaret();
//            //            textBoxConsole.AppendText("----------------------------------" +
//            //            Environment.NewLine.ToString());
//            //            textBoxConsole.ScrollToCaret();

//            //            foreach (Cookie cookie in cookieJar.GetCookies(
//            //                new Uri(string.Format("https://{0}/", key))))
//            //            {
//            //                //Console.WriteLine(
//            //                //   "Name = {0} ; Value = {1} ; Domain = {2}",
//            //                //   cookie.Name, cookie.Value, cookie.Domain);

//            //                string details = cookie.Name;// + " " + cookie.Value + " " + cookie.Domain;
//            //                    textBoxConsole.AppendText(details +
//            //                            Environment.NewLine.ToString());
//            //                    textBoxConsole.ScrollToCaret();

//            //            }
//            //        }
//            //    }
//            //}

//        }
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
//        }
//    }
//    return false;
//}

//internal static bool Look_For_LoginLink(FinanceViewModel financeviewmodel,
//                                        ref string href,
//                                        ref string url_base)
//{
//    bool result = false;

//    IList<HtmlAgilityPack.HtmlNode> HtmlCol1;

//    string classname;

//    href = url_base = string.Empty;

//    HtmlCol1 = YetAnotherFuckingHoop.SelectNodesAsList(financeviewmodel.html_document.DocumentNode, "//a");
//    foreach (HtmlAgilityPack.HtmlNode element1 in HtmlCol1)
//    {
//        classname = element1.GetAttributeValue("class", string.Empty);
//        if (!string.IsNullOrEmpty(classname))
//        {
//            if (classname.Length >= 9)
//            {
//                if (classname.Substring(0, 9) == "LoginLink")
//                {
//                    href = element1.GetAttributeValue("href", string.Empty);
//                    href = href.Replace("&amp;", "&");

//                    int index = href.IndexOf(SmartParametersV2016.https);
//                    if (index >= 0)
//                    {
//                        index = index + SmartParametersV2016.https.Length;
//                        if (href.Length > index)
//                        {
//                            int slash = href.IndexOf("/", index);
//                            if (slash >= 0)
//                            {
//                                url_base = href.Substring(0, slash);
//                                result = true;
//                            }
//                        }
//                    }
//                    break;
//                }
//            }
//        }
//    }
//    return result;
//}

//internal static async Task<List<SmartFinance.BankTransactions>> Nationwide_Find_Transactions(
//                                                                MainViewModel ourviewmodel,
//                                                                FinanceViewModel financeviewmodel,
//                                                                bool log,
//                                                                string referer,
//                                                                string url_base,
//                                                                string url_href,
//                                                                string sortcode,
//                                                                string account_no)
//{
//    List<SmartFinance.TransactionTypes> transaction_types_list,
//    List<SmartFinance.TransactionSubTypes> transaction_sub_types_list)

//    string //inner_text,
//              acct_type_tag,
//            account_address,
//            classname,
//           href,
//                token;

//    string //response_type,
//            response_itself = url_href;

//    string currency = "GBP",    // Pull this in from the account
//            amount = string.Empty,
//            date = string.Empty,
//            description = string.Empty,
//            balance = string.Empty,
//            sequence_no,
//            transaction_type = string.Empty;
//    bool is_paid_in = false;
//    short transaction_code = 0,
//            transaction_sub_code = 0;

//    HtmlAgilityPack.HtmlDocument document = new HtmlAgilityPack.HtmlDocument();

//    List<SmartFinance.BankTransactions> Bollocks = new List<SmartFinance.BankTransactions>();

//    Uri next_url = new Uri(url_base + response_itself);  // Response_itself shouldn't be empty

//    if (await SmartBanksV2019.DoGet_Finance(
//                                        ourviewmodel,
//                                        financeviewmodel,
//                                        financeviewmodel.finance_token,
//                                        next_url,
//                                        url_base,
//                                        log,
//                                        "GET",
//                                        referer))

//    {
//        token = Nationwide_Find_Script_Token(document);
//         Now you need to try and POST some dates and see what you get back
//         which will be in JSON so it will need decoding?  You will need to
//         keep their SDf (stupid Date Format) as a field so you can compare
//         Good luck old son - nearly there!!
//        if (!string.IsNullOrEmpty(token))
//        {
//             Fix local time
//            DateTime time_now = SmartEncryptionV2016.DateTimeNow(SmartParametersV2016.gmtdefaultOffset);
//            string end_date = time_now.ToString("dd/MM/yyyy");
//            string start_date = time_now.AddMonths(-12).ToString("dd/MM/yyyy");
//            string data = "__token" + "|" + token + SmartParametersV2016.record_separator +
//                            "start" + "|" + start_date + SmartParametersV2016.record_separator +
//                            "end" + "|" + end_date + SmartParametersV2016.record_separator +
//                            "filters" + "|" + "ConfirmInfoRead" + SmartParametersV2016.record_separator +
//                            "filters" + "|" + "TransactionFilterMapping.AllMoneyPaidOut" + SmartParametersV2016.record_separator +
//                            "filters" + "|" + "TransactionFilterMapping.CashWithdrawal" + SmartParametersV2016.record_separator +
//                            "filters" + "|" + "TransactionFilterMapping.ChequeWithdrawal" + SmartParametersV2016.record_separator +
//                            "filters" + "|" + "TransactionFilterMapping.DirectDebits" + SmartParametersV2016.record_separator +
//                            "filters" + "|" + "TransactionFilterMapping.ChargesOut" + SmartParametersV2016.record_separator +
//                            "filters" + "|" + "TransactionFilterMapping.DebitCardPayments" + SmartParametersV2016.record_separator +
//                            "filters" + "|" + "TransactionFilterMapping.RegularPaymentsOutside" + SmartParametersV2016.record_separator +
//                            "filters" + "|" + "TransactionFilterMapping.SinglePaymentsOutside" + SmartParametersV2016.record_separator +
//                            "filters" + "|" + "TransactionFilterMapping.TransfersToNationwide" + SmartParametersV2016.record_separator +
//                            "filters" + "|" + "TransactionFilterMapping.ReturnedItems" + SmartParametersV2016.record_separator +
//                            "filters" + "|" + "TransactionFilterMapping.MiscellaneousDebits" + SmartParametersV2016.record_separator +
//                            "filters" + "|" + "TransactionFilterMapping.AllMoneyPaidIn" + SmartParametersV2016.record_separator +
//                            "filters" + "|" + "TransactionFilterMapping.CashCredits" + SmartParametersV2016.record_separator +
//                            "filters" + "|" + "TransactionFilterMapping.ChequeCredits" + SmartParametersV2016.record_separator +
//                            "filters" + "|" + "TransactionFilterMapping.BankCredits" + SmartParametersV2016.record_separator +
//                            "filters" + "|" + "TransactionFilterMapping.TransfersFromNationwideAccount" + SmartParametersV2016.record_separator +
//                            "filters" + "|" + "TransactionFilterMapping.DebitCardTransactions" + SmartParametersV2016.record_separator +
//                            "filters" + "|" + "TransactionFilterMapping.UnpaidItems" + SmartParametersV2016.record_separator +
//                            "filters" + "|" + "TransactionFilterMapping.ChargesIn" + SmartParametersV2016.record_separator +
//                            "filters" + "|" + "TransactionFilterMapping.MiscellaneousCredits";
//            var not_empty2 = SmartBanksV2019.Common_Build_Form_Strings_New(data);
//            response_itself = string.Empty;
//            Uri transactions_url = new Uri(url_base + "/Transactions/FullStatement/FullStatement");

//            await SmartRoutinesV2018.TextBlockUpdate(
//                                                    ourviewmodel,
//                                                    "Requesting transactions list");

//            if (await SmartBanksV2019.DoPost_NewX(
//                                                ourviewmodel,
//                                                financeviewmodel,
//                                                financeviewmodel.finance_token,
//                                                transactions_url,
//                                                url_base,
//                                                log,
//                                                0,
//                                                "POST",
//                                                not_empty2,
//                                                referer))

//            {

//                await SmartRoutinesV2018.TextBlockUpdate(
//                                                    ourviewmodel,
//                                                    "Received transactions list");

//                 ONE DATE for all of this Batch of transactions!!
//                DateTime transaction_created = SmartEncryptionV2016.DateTimeNow(ourviewmodel.gmt_offset);   // GMT time

//                DateTime converted_date = SmartParametersV2016.default_date;

//                dynamic jsonResponse = JObject.Parse(response_itself);

//                foreach (dynamic statement in jsonResponse)
//                {
//                    string statement_name = statement.Name;
//                    if (statement_name == "Transactions")
//                    {
//                        foreach (dynamic transactions in statement)
//                        {
//                            foreach (dynamic transaction in transactions)
//                            {
//                                foreach (dynamic individual in transaction)
//                                {
//                                    string column = individual.Name;
//                                    switch (column)
//                                    {
//                                        case "Amount":
//                                            amount = individual.Value;
//                                            int sep = amount.IndexOf(SmartParametersV2016.defaultCurrencySeparator);
//                                            if (sep == -1)
//                                            {
//                                                amount = amount + SmartParametersV2016.defaultCurrencySeparator + "00";
//                                            }
//                                            else
//                                            {
//                                                if (sep + 2 == amount.Length)
//                                                {
//                                                    amount += "0";
//                                                }
//                                            }
//                                            break;
//                                        case "Date":
//                                            try
//                                            {
//                                                date = individual.Value.ToString();
//                                                converted_date = SmartRoutinesV2018.DateTimeParse(date).ToLocalTime();
//                                            }
//                                            catch (Exception ex)
//                                            {

//                                                await SmartBobV2017.ListenerAsync(ourviewmodel, financeviewmodel.finance_token, financeviewmodel.institution_code, financeviewmodel.brand_code, ex.Message + ": " + date);

//                                            }
//                                            break;
//                                        case "Description":
//                                            description = individual.Value;
//                                            break;
//                                        case "IsPaidIn":
//                                            is_paid_in = Convert.ToBoolean(individual.Value);
//                                            break;
//                                        case "RunningBalance":
//                                            balance = individual.Value;
//                                            int sep2 = balance.IndexOf(SmartParametersV2016.defaultCurrencySeparator);
//                                            if (sep2 == -1)
//                                            {
//                                                balance = balance + SmartParametersV2016.defaultCurrencySeparator + "00";
//                                            }
//                                            else
//                                            {
//                                                if (sep2 + 2 == balance.Length)
//                                                {
//                                                    balance += "0";
//                                                }
//                                            }
//                                            break;
//                                        case "Sequence":
//                                            sequence_no = individual.Value;
//                                            break;
//                                        case "TransactionType":
//                                            transaction_type = individual.Value;
//                                            break;
//                                        default:
//                                            break;
//                                    }
//                                }

//                                if (!SmartSpikeFinanceV2017.Lookup_Transaction_SubCode(financeviewmodel,
//                                                                                            is_paid_in,
//                                                                                            ref transaction_type,
//                                                                                            ref transaction_code,
//                                                                                            ref transaction_sub_code,
//                                                                                            false))
//                                {

//                                    await SmartBobV2017.ListenerAsync(ourviewmodel, financeviewmodel.finance_token, financeviewmodel.institution_code, financeviewmodel.brand_code, "Trans type  failed: " + transaction_type);

//                                }

//                                 Still want to add it in even if we can't analyze the transaction ...
//                                int random1 = SmartRoutinesV2018.Get_Next_Random(ourviewmodel.random_r);
//                                int random2 = SmartRoutinesV2018.Get_Next_Random(ourviewmodel.random_r);

//                                SmartFinance.BankTransactions abc = new SmartFinance.BankTransactions()
//                                {
//                                    USERNAME = ourviewmodel.UserName,
//                                    INSTITUTION_CODE = financeviewmodel.institution_code,
//                                    BRAND_CODE = financeviewmodel.brand_code,
//                                    SORTCODE = sortcode,
//                                    ACCOUNT_NO = account_no,
//                                    RANDOMKEY1 = random1,
//                                    BOOKING_DATE = converted_date,
//                                    SEQUENCE_NO = new DateTimeOffset(DateTime.Now + ourviewmodel.gmt_offset).ToUnixTimeMilliseconds(), // Unique AND increasing
//                                    CREDITDEBIT_INDICATOR = is_paid_in,
//                                    TRANSACTION_CODE = transaction_code,
//                                    TRANSACTION_SUB_CODE = transaction_sub_code,
//                                    DESCRIPTION = description,
//                                    AMOUNT = Convert.ToInt32(amount.Replace(".", string.Empty)),
//                                    CURRENCY = currency,     // Pull this in from the Account
//                                    BALANCE_AMOUNT = Convert.ToInt32(balance.Replace(".", string.Empty)),
//                                    RANDOMKEY2 = random2
//                                    Updated = false
//                                };

//                                Bollocks.Add(abc);

//                            }
//                        }
//                    }
//                }

//                await SmartRoutinesV2018.TextBlockUpdate(
//                                                    ourviewmodel,
//                                                    "Found: " + Bollocks.Count + " transactions");
//            }
//            else
//            {

//                await SmartRoutinesV2018.TextBlockUpdate(
//                                                    ourviewmodel,
//                                                     "Failed: Asked for transactions");
//            }
//        }
//    }
//    return Bollocks;    // Which may be empty ...
//}

//internal static bool NW_Get_SMSOTP_String(string reply,
//                                            ref string SmsResendsRemaining,
//                                            ref string NemOtpExpiryTime)
//{
//    string FinanceLog = string.Empty;

//    try
//    {
//        //string reply = "{\"SmsResendsRemaining\":\"\",\"NemOtpExpiryTime\":\"\\/Date(1630783419027)\\/\"}";
//        dynamic jsonResponse = JObject.Parse(reply);
//        foreach (dynamic statement in jsonResponse)
//        {
//            string statement_name = statement.Name;
//            switch (statement_name)
//            {
//                case "SmsResendsRemaining":
//                    SmsResendsRemaining = statement.Value;
//                    //Console.WriteLine(amount);
//                    break;
//                case "NemOtpExpiryTime":
//                    NemOtpExpiryTime = statement.ToString();
//                    NemOtpExpiryTime = NemOtpExpiryTime.Replace("NemOtpExpiryTime", string.Empty);
//                    NemOtpExpiryTime = NemOtpExpiryTime.Replace("\"", string.Empty).TrimStart(':').Trim();
//                    //Console.WriteLine(NemOtpExpiryTime);
//                    //if (NemOtpExpiryTime == comp)
//                    //{
//                    //    Console.WriteLine("They match!");
//                    //}
//                    break;
//                default:
//                    break;
//            }
//        }
//        return true;
//    }
//    catch (Exception exception)
//    {
//        FinanceLog = FinanceLog + exception.Message + Environment.NewLine.ToString();

//        //reply = FinanceLog;
//        //return false;
//    }
//    //reply = string.Empty;
//    return false;
//}

//internal static bool NW_Get_Json_String(ref string reply)
//{
//    string FinanceLog = string.Empty;
//    try
//    {
//        reply = reply.Replace("[", "{");
//        reply = reply.Replace("]", "}");
//        // adding 'empty' values to all the variables .. what a bag of shit this stuff is ...
//        reply = reply.Replace(" digit\"", "\":\"\"");
//        JObject jsonResponse = JObject.Parse(reply);
//        if (jsonResponse != null)
//        {
//            reply = jsonResponse.ToString().Replace(Environment.NewLine, string.Empty);
//            return true;
//        }
//    }
//    catch (Exception exception)
//    {
//        FinanceLog = FinanceLog + exception.Message + Environment.NewLine.ToString();

//        reply = FinanceLog;
//        return false;
//    }
//    reply = string.Empty;
//    return false;
//}



//internal static void Nationwide_Build_Account_List(HtmlAgilityPack.HtmlDocument document,
//                                        ref string owner,
//                                        ref string accounts)
//{
//    IList<HtmlAgilityPack.HtmlNode> HtmlCol1,
//            HtmlCol2,
//            HtmlCol3,
//            HtmlCol4,
//            HtmlCol5,
//            HtmlCol6;

//    // < div id = "header" >
//    //< header >
//    //< div class="welcome-util-container">
//    //  <div id = "welcome-message" > Welcome back, Genius</div>

//    HtmlCol1 = YetAnotherFuckingHoop.SelectNodesAsList(document.DocumentNode, ".//div");
//    foreach (HtmlAgilityPack.HtmlNode element1 in HtmlCol1)
//    {
//        if (element1.Id == "header")
//        {
//            HtmlCol2 = YetAnotherFuckingHoop.SelectNodesAsList(element1, ".//div");
//            foreach (HtmlAgilityPack.HtmlNode element2 in HtmlCol2)
//            {
//                string classname = element2.GetAttributeValue("class", string.Empty);
//                if (classname == "welcome-util-container")
//                {
//                    HtmlCol3 = YetAnotherFuckingHoop.SelectNodesAsList(element2, ".//div");
//                    foreach (HtmlAgilityPack.HtmlNode element3 in HtmlCol3)
//                    {
//                        if (element3.Id == "welcome-message")
//                        {
//                            owner = element3.InnerText.Replace("Welcome back,", string.Empty).Trim();
//                            break;
//                        }
//                    }
//                }
//                if (!string.IsNullOrEmpty(owner))
//                {
//                    break;
//                }
//            }
//        }
//        if (!string.IsNullOrEmpty(owner))
//        {
//            break;
//        }
//    }

//    HtmlCol1 = YetAnotherFuckingHoop.SelectNodesAsList(document.DocumentNode, "//nav");
//    foreach (HtmlAgilityPack.HtmlNode element1 in HtmlCol1)
//    {
//        if (element1.Id == "primary-nav")
//        {
//            HtmlCol2 = YetAnotherFuckingHoop.SelectNodesAsList(element1, ".//ul");
//            foreach (HtmlAgilityPack.HtmlNode element2 in HtmlCol2)
//            {
//                HtmlCol3 = YetAnotherFuckingHoop.SelectNodesAsList(element2, ".//li");
//                foreach (HtmlAgilityPack.HtmlNode element3 in HtmlCol3)
//                {
//                    string classname = element3.GetAttributeValue("class", string.Empty);
//                    if (classname == "active")
//                    {
//                        HtmlCol4 = YetAnotherFuckingHoop.SelectNodesAsList(element3, ".//ul");
//                        foreach (HtmlAgilityPack.HtmlNode element4 in HtmlCol4)
//                        {
//                            HtmlCol5 = YetAnotherFuckingHoop.SelectNodesAsList(element4, ".//li");
//                            foreach (HtmlAgilityPack.HtmlNode element5 in HtmlCol5)
//                            {
//                                HtmlCol6 = YetAnotherFuckingHoop.SelectNodesAsList(element5, ".//a");
//                                foreach (HtmlAgilityPack.HtmlNode element6 in HtmlCol6)
//                                {
//                                    string href = element6.GetAttributeValue("href", string.Empty);
//                                    href = href.Replace("&amp;", "&");
//                                    if (!string.IsNullOrEmpty(element6.InnerText))
//                                    {
//                                        string inner_text = element6.InnerText;
//                                        if (inner_text.IndexOf("Overview") != 0)
//                                        {
//                                            string html = element6.InnerHtml;
//                                            html = html.Replace("<br>", SmartParametersV2016.space).Trim();
//                                            if (string.IsNullOrEmpty(accounts))
//                                            {
//                                                accounts = html +
//                                                            SmartParametersV2016.unit_separator +
//                                                            href;
//                                            }
//                                            else
//                                            {
//                                                accounts = accounts +
//                                                            SmartParametersV2016.record_separator +
//                                                             html +
//                                                            SmartParametersV2016.unit_separator +
//                                                             href;
//                                            }
//                                        }
//                                    }
//                                }
//                            }
//                        }
//                    }
//                }
//            }
//        }
//    }
//    return;
//}

//internal static string Nationwide_Find_Token(HtmlAgilityPack.HtmlDocument document, string id)
//{
//    IList<HtmlAgilityPack.HtmlNode> HtmlCol1;
//    string token = string.Empty;

//    HtmlCol1 = YetAnotherFuckingHoop.SelectNodesAsList(document.DocumentNode, "//input");
//    foreach (HtmlAgilityPack.HtmlNode element1 in HtmlCol1)
//    {
//        if (!string.IsNullOrEmpty(element1.Id))
//        {
//            if (element1.Id == id) // SmsOtp_passNoEntryPositions
//            {
//                //string name = element1.GetAttributeValue("name", string.Empty);
//                //if (name == "__token")
//                //{
//                token = element1.GetAttributeValue("value", string.Empty);
//                return token;
//                //}
//            }
//        }
//    }
//    return token;
//}

//internal static string Nationwide_Find_Form(HtmlAgilityPack.HtmlDocument document, string id,
//                                                string target_name,
//                                                ref string urlval)
//{
//    IList<HtmlAgilityPack.HtmlNode> HtmlCol1,
//                                    HtmlCol2;
//    string token = string.Empty;
//    string name;

//    HtmlCol1 = YetAnotherFuckingHoop.SelectNodesAsList(document.DocumentNode, "//form");
//    foreach (HtmlAgilityPack.HtmlNode element1 in HtmlCol1)
//    {
//        urlval = element1.GetAttributeValue("action", string.Empty);
//        HtmlCol2 = YetAnotherFuckingHoop.SelectNodesAsList(element1, ".//input");
//        foreach (HtmlAgilityPack.HtmlNode element2 in HtmlCol2)
//        {
//            if (!string.IsNullOrEmpty(element2.Id))
//            {
//                if (element2.Id == id) //"accessmanagementloginloginviamemorabledataandpassnumber")
//                {
//                    name = element2.GetAttributeValue("name", string.Empty);
//                    if (name == target_name) // "__token")
//                    {
//                        token = element2.GetAttributeValue("value", string.Empty);
//                        return token;
//                    }
//                }
//            }
//        }
//    }
//    return token;
//}
//internal static string Nationwide_Find_Script_Token(HtmlAgilityPack.HtmlDocument document)
//{
//    IList<HtmlAgilityPack.HtmlNode> HtmlCol1;
//    string token = string.Empty;
//    bool found_url = false;

//    HtmlCol1 = YetAnotherFuckingHoop.SelectNodesAsList(document.DocumentNode, "//script");
//    foreach (HtmlAgilityPack.HtmlNode element1 in HtmlCol1)
//    {
//        string tipe = element1.GetAttributeValue("type", string.Empty);
//        if (tipe == "text/javascript")
//        {
//            if (!string.IsNullOrEmpty(element1.InnerText))
//            {
//                string[] components = element1.InnerText.Split(',');
//                foreach (string component in components)
//                {
//                    if (component.IndexOf("url:") >= 0)
//                    {
//                        string text = component.Replace("url:", string.Empty).Trim();
//                        text = text.Trim('\'');
//                        if (text == "/Transactions/FullStatement/FullStatement")
//                        {
//                            found_url = true;
//                        }
//                    }
//                    else
//                    {
//                        if (component.IndexOf("token:") >= 0)
//                        {
//                            string text = component.Replace("token:", string.Empty).Trim();
//                            text = text.Trim('\'');
//                            if (found_url)
//                            {
//                                token = text;
//                                return token;
//                            }
//                        }
//                    }
//                }
//            }
//        }
//    }
//    return token;
//}
//}
//}