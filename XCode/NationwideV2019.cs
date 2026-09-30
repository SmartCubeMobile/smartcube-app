using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

//using System.Linq;
using System.Reflection;
using System.Collections;
using System.Net;
//using System.Collections.Generic;

#if WINFORMS
using System.Windows.Forms;

#endif
#if WPF
using System.Windows.Controls;
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

    public class NationwideV2019
    {
        internal static async Task<bool> NationWide_Find_Accounts(
#if WINFORMS
                                            RichTextBox textBoxConsole,
#endif
                                            MainViewModel ourviewmodel,
                                            FinanceViewModel financeviewmodel,
                                            string url_first,   // nationwide.co.uk
                                            bool log,
                                            string referer,
                                            string customer_no,
                                            string dob,
                                            string passcode,
                                            string memorable_data)
        {
            string url_base = string.Empty;     // Which we try to deduce!

            string inner_text = string.Empty,
                        acct_type_tag = string.Empty,
                        account_address = string.Empty,
                        classname = string.Empty,
                        href = string.Empty;
            bool found_it = false;

            financeviewmodel.type = string.Empty;
            financeviewmodel.response = string.Empty;

            Uri url = new Uri(url_first);   // nationwide.co.uk

            financeviewmodel.html_document = new HtmlAgilityPack.HtmlDocument();

#if WINFORMS
            textBoxConsole.AppendText("Sending: " + url +
                                            Environment.NewLine.ToString());
            textBoxConsole.ScrollToCaret();
#endif
            if (await SmartBanksV2019.DoGet_Finance(
                                                ourviewmodel,
                                                financeviewmodel,
                                                financeviewmodel.finance_token,
                                                url,        // nationwide.co.uk
                                                url_first,  // nationwide.co.uk
                                                log,
                                                "GET",
                                                referer))

            {
                //< nav aria - label = "Log in" class="DropdownNav-fzf12b-0 ihYshF LoginFlyout__UnposedLoginFlyout-uiiphf-1 kUBlCj LoginFlyout__MobileTabletLoginFlyout-uiiphf-2 eNaqE" 
                //  data-analytics-identifier="login flyout" 
                // data-analytics-context="true">
                // <div class="VStack-sc-186pbzy-0 LoginFlyout__UnposedNavItem-uiiphf-0 cqeycc">
                // <div class="TopRow__TopRowFlexContainer-x2n9g4-0 dpiQRV">
                // <h2 class="NelComponents__Heading-vsly48-4 FYxCH nel-Typography-454 nel-Typography-449" data-ref="heading">Internet Banking</h2>
                // <a class="LoginLinks__ShortLoginLink-myoff6-1 iBIaJC nel-Link-6 nel-Link-5" 
                // data-ref="link" 
                // href="https://onlinebanking.nationwide.co.uk/AccessManagement/IdentifyCustomer/IdentifyCustomer">
#if WINFORMS
                textBoxConsole.AppendText("Checking: " + "LoginLink" +
                                            Environment.NewLine.ToString());
                textBoxConsole.ScrollToCaret();
#endif

                if (!Look_For_LoginLink(financeviewmodel, ref href, ref url_base))
                {
#if WINFORMS
                    textBoxConsole.AppendText("Problem: " + "LoginLink" +
                                            Environment.NewLine.ToString());
                    textBoxConsole.ScrollToCaret();
#endif
                    return false;
                }

                // Now try to extract the REAL url_base i.e.      
                if (string.IsNullOrEmpty(href) ||
                    string.IsNullOrEmpty(url_base))
                {
#if WINFORMS
                    textBoxConsole.AppendText("Problem: " + "href or url_base" +
                                            Environment.NewLine.ToString());
                    textBoxConsole.ScrollToCaret();
#endif
                    return false;
                }

#if WINFORMS
                textBoxConsole.AppendText("Found href: " + href +
                                        Environment.NewLine.ToString());
                textBoxConsole.ScrollToCaret();
                textBoxConsole.AppendText("Setting url_base: " + url_base +
                                    Environment.NewLine.ToString());
                textBoxConsole.ScrollToCaret();
#endif
                found_it = true;
                Uri next_url = new Uri(href);   // "https://onlinebanking.nationwide.co.uk/AccessManagement/IdentifyCustomer/IdentifyCustomer"


#if WINFORMS
                textBoxConsole.AppendText("Sending: " + next_url +
                                    Environment.NewLine.ToString());
                textBoxConsole.ScrollToCaret();
#endif
                if (!await SmartBanksV2019.DoGet_Finance(
                                        ourviewmodel,
                                        financeviewmodel,
                                        financeviewmodel.finance_token,
                                        next_url,
                                        url_base,
                                        log,
                                        "GET",
                                        referer))
                {
#if WINFORMS
                    textBoxConsole.AppendText("Problem: " + next_url +
                                            Environment.NewLine.ToString());
                    textBoxConsole.ScrollToCaret();
#endif
                    return false;
                }
#if WINFORMS
                textBoxConsole.AppendText("Checking: " + "__token" +
                                            Environment.NewLine.ToString());
                textBoxConsole.ScrollToCaret();
#endif
                string customerid_url = string.Empty;
                string customerid_token = Nationwide_Find_Form(financeviewmodel.html_document,
                                                "accessmanagementidentifycustomerentercustomeridentificationdetail",
                                                "__token",
                                                ref customerid_url);
                if (string.IsNullOrEmpty(customerid_token) ||
                    string.IsNullOrEmpty(customerid_url))
                {
#if WINFORMS
                    textBoxConsole.AppendText("Problem: " + "token or url_access_management" +
                                            Environment.NewLine.ToString());
                    textBoxConsole.ScrollToCaret();
#endif
                    return false;
                }
#if WINFORMS
                textBoxConsole.AppendText("Found token: " + customerid_token +
                                        Environment.NewLine.ToString());
                textBoxConsole.ScrollToCaret();
                textBoxConsole.AppendText("Setting url: " + url_base +
                                    customerid_url +
                                    Environment.NewLine.ToString());
                textBoxConsole.ScrollToCaret();
#endif
                // Now try and POST with the Customer Number, Date of Birth and token
                string urgent_message = string.Empty;
                if (!SmartLoginV2016.Create_Uri(url_base, customerid_url,
                    ref next_url,
                    ref urgent_message))
                {
#if WINFORMS
                    // Bad news ...
                    textBoxConsole.AppendText("Error: " + urgent_message +
                            Environment.NewLine.ToString());
                    textBoxConsole.ScrollToCaret();
#endif
                    return false;
                }
                
                string day = string.Empty,
                       month = string.Empty,
                       year = string.Empty;
                if (!SplitDob(dob, ref day, ref month, ref year))
                {
#if WINFORMS
                    // Bad Birthday news ...
                    textBoxConsole.AppendText("Error: " + dob +
                            Environment.NewLine.ToString());
                    textBoxConsole.ScrollToCaret();
#endif
                    return false;
                }

                string now = SmartEncryptionV2016.DateTimeNow(ourviewmodel.gmt_offset).ToString("ddd MMM dd yyyy hh:mm:ss"); // Doesn't appear to be 24 hr time - problem?
                                                                                                                             //now = now + (" GMT+0000 (GMT Standard Time)");
                now = now + " GMT+0000 " + "(" + "GMT Standard Time" + ")";

                string parameters = "__token" + "|" + customerid_token + SmartParametersV2016.record_separator +
                                    "PersistentCookiesEnabled" + "|" + "true" + SmartParametersV2016.record_separator +
                                    "ScreenResolution" + "|" + "1536 x 864" + SmartParametersV2016.record_separator +
                                    "TimeZoneOffset" + "|" + "1" + SmartParametersV2016.record_separator +
                                    "LocalTime" + "|" + now + SmartParametersV2016.record_separator +
                                    "CustomerNumber" + "|" + customer_no + SmartParametersV2016.record_separator +
                                    "RememberMe" + "|" + "false" + SmartParametersV2016.record_separator +
                                    "SpecifiedStartPage" + "|" + "" + SmartParametersV2016.record_separator +
                                    "DateOfBirthDay" + "|" + day + SmartParametersV2016.record_separator +
                                    "DateOfBirthMonth" + "|" + month + SmartParametersV2016.record_separator +
                                    "DateOfBirthYear" + "|" + year;
                var not_empty1 = SmartBanksV2019.Common_Build_Form_Strings_New(parameters);
                financeviewmodel.response = string.Empty;
#if WINFORMS
                textBoxConsole.AppendText("Sending: CustomerId parameters" +
                            Environment.NewLine.ToString());
                textBoxConsole.ScrollToCaret();
#endif
                if (!await SmartBanksV2019.DoPost_NewX(
                                ourviewmodel,
                                financeviewmodel,
                                financeviewmodel.finance_token,
                                next_url,
                                url_base,
                                log,
                                0,
                                "POST",
                                not_empty1,
                                referer))
                {
#if WINFORMS
                    textBoxConsole.AppendText("Problem: " + "Post CustomerId and DOB" +
                                            Environment.NewLine.ToString());
                    textBoxConsole.ScrollToCaret();
#endif
                    return false;
                }

                //PrintCookies(ourviewmodel.cookies, textBoxConsole);
                //if (ourviewmodel.cookies.Count > 0)
                //{
                //    return true;
                //}
                string servicing = WebUtility.UrlEncode("ib:servicing:login:login method:passnumber and code by text");
                servicing = servicing.Replace("+", "%20");
                Cookie gpv_v19 = new Cookie()
                {
                    Name = "gpv_v19",
                    Value = servicing,
                    Path ="/",
                    Domain = "onlinebanking.nationwide.co.uk"
            };
                ourviewmodel.cookies.Add(gpv_v19);
                // Now try and find the OTP token which is hidden in the previous response!
                string passnumberandsmsotp_url = string.Empty;
                string passnumberandsmsotp_token = Nationwide_Find_Form(financeviewmodel.html_document,
                                                "accessmanagementloginloginviapassnumberandsmsotp", // Id
                                                "__token",                          // name
                                                ref passnumberandsmsotp_url);
                if (string.IsNullOrEmpty(passnumberandsmsotp_token) ||
                    string.IsNullOrEmpty(passnumberandsmsotp_url))
                {
#if WINFORMS
                    textBoxConsole.AppendText("Problem: " + "PassNumbers token or url" +
                                            Environment.NewLine.ToString());
                    textBoxConsole.ScrollToCaret();
#endif
                    return false;
                }

                string SendSmsCanaryTokenValue_url = string.Empty;
                string sendsmscanary_token = Nationwide_Find_Form(financeviewmodel.html_document, "SendSmsCanaryTokenValue",
                                                "SendSmsCanaryTokenValue",
                                                ref SendSmsCanaryTokenValue_url);
                if (string.IsNullOrEmpty(sendsmscanary_token) ||
                    string.IsNullOrEmpty(SendSmsCanaryTokenValue_url))
                {
#if WINFORMS
                    textBoxConsole.AppendText("Problem: " + "SendSmsCanary token or url" +
                                            Environment.NewLine.ToString());
                    textBoxConsole.ScrollToCaret();
#endif
                    return false;
                }

                // Cassie Speck <= Sissy Spacek!!! ODIWHTLTTB

                // Jennifer Anson < Aniston

                string passnumbers_token = Nationwide_Find_Token(financeviewmodel.html_document,
                                                                "SmsOtp_passNoEntryPositions");                                                
                if (string.IsNullOrEmpty(passnumbers_token))
                {
#if WINFORMS
                    textBoxConsole.AppendText("Problem: " + "Passnumbers token" +
                                            Environment.NewLine.ToString());
                    textBoxConsole.ScrollToCaret();
#endif
                    return false;
                }
                passnumbers_token = passnumbers_token.TrimEnd(SmartParametersV2016.commachar);
                string[] passnumbers = passnumbers_token.Split(SmartParametersV2016.commachar);
                if (passnumbers.Count() != 3)
                {
#if WINFORMS
                    // Bad news ...
                    textBoxConsole.AppendText("Error: " + passnumbers_token +
                            Environment.NewLine.ToString());
                    textBoxConsole.ScrollToCaret();
#endif
                    return false;
                }
                string firstpassnumber = passcode.Substring(Convert.ToInt16(passnumbers[0]) - 1, 1);
                string secondpassnumber = passcode.Substring(Convert.ToInt16(passnumbers[1]) - 1, 1);
                string thirdpassnumber = passcode.Substring(Convert.ToInt16(passnumbers[2]) - 1, 1);

                // Build the parameters to send the token
                parameters = "__token" + "|" + sendsmscanary_token;
                not_empty1 = SmartBanksV2019.Common_Build_Form_Strings_New(parameters);
                financeviewmodel.response = string.Empty;

                if (!SmartLoginV2016.Create_Uri(url_base, "AccessManagement/Login/SendSMSOTP",
                    ref next_url,
                    ref urgent_message))
                {
#if WINFORMS
                    // Bad news ...
                    textBoxConsole.AppendText("Error: " + urgent_message +
                            Environment.NewLine.ToString());
                    textBoxConsole.ScrollToCaret();
#endif
                    return false;
                }
                // Now send the token to get the OTP on your phone
                if (!await SmartBanksV2019.DoPost_NewX(
                                ourviewmodel,
                                financeviewmodel,
                                financeviewmodel.finance_token,
                                next_url,   // https://onlinebanking.nationwide.co.uk/AccessManagement/Login/SendSMSOTP
                                url_base,
                                log,
                                1,
                                "POST",
                                not_empty1,
                                "https://onlinebanking.nationwide.co.uk/AccessManagement/Login/IdentifyCustomerForLogin"
                                ))//"https://onlinebanking.nationwide.co.uk"))
                {
#if WINFORMS
                    textBoxConsole.AppendText("Problem: " + "Access Token" +
                                            Environment.NewLine.ToString());
                    textBoxConsole.ScrollToCaret();
#endif
                    return false;
                }

                TextReader txt1 = new StreamReader(@"C:\Users\Ray\AppData\Local\SMSOTPEXPIRED.txt");
                string one = txt1.ReadToEnd();
                txt1.Close();

                txt1 = new StreamReader(@"C:\Users\Ray\AppData\Local\OTPRESENDSLIMITREACHEDHELPHINTSTITLE.txt");
                string two = txt1.ReadToEnd();
                txt1.Close();

                txt1 = new StreamReader(@"C:\Users\Ray\AppData\Local\OTPRESENDSLIMITREACHEDHELPHINTS.txt");
                string three = txt1.ReadToEnd();
                txt1.Close();

                txt1 = new StreamReader(@"C:\Users\Ray\AppData\Local\OTPEXPIREDCONTENT.txt");
                string four = txt1.ReadToEnd();
                txt1.Close();
                string SmsResendsRemaining = string.Empty;
                string NemOtpExpiryTime = string.Empty;
                if (!NW_Get_SMSOTP_String(financeviewmodel.response,
                                                    ref SmsResendsRemaining,
                                                    ref NemOtpExpiryTime))
                {
#if WINFORMS
                    textBoxConsole.AppendText("Problem: " + "SMSOTP response" +
                                            Environment.NewLine.ToString());
                    textBoxConsole.ScrollToCaret();
#endif
                    return false;
                }

                TextReader txt = new StreamReader(@"C:\Users\Ray\AppData\Local\SMSOTP.txt");
                string onetimepasscode = txt.ReadToEnd();
                txt.Close();

                now = SmartEncryptionV2016.DateTimeNow(ourviewmodel.gmt_offset).ToString("ddd MMM dd yyyy hh:mm:ss"); // Doesn't appear to be 24 hr time - problem?                                                                                                                      //now = now + (" GMT+0000 (GMT Standard Time)");
                // ?????? 0100 and not 0000??
                now = now + " GMT+0100";// + "(" + "GMT Standard Time" + ")";

                

                parameters = "__token" + "|" + passnumberandsmsotp_token + SmartParametersV2016.record_separator +
                            "PersistentCookiesEnabled" + "|" + "true" + SmartParametersV2016.record_separator +
                            "ScreenResolution" + "|" + "1536 x 864" + SmartParametersV2016.record_separator +
                            "TimeZoneOffset" + "|" + "1"  + SmartParametersV2016.record_separator +
                            "LocalTime" + "|" + now + SmartParametersV2016.record_separator +
                            "IsOneTimePasscodeRequired" + "|" + "true" + SmartParametersV2016.record_separator +
                            "SmsResendsRemaining" + "|" + SmsResendsRemaining + SmartParametersV2016.record_separator +
                            "NemOtpExpiryTime" + "|" + NemOtpExpiryTime + SmartParametersV2016.record_separator +
                            "SendSmsCanaryTokenValue" + "|" + sendsmscanary_token + SmartParametersV2016.record_separator +
                            "SmsOtpCreated" + "|" + "False" + SmartParametersV2016.record_separator +
                            "SmsOtpExpired" + "|" + one + SmartParametersV2016.record_separator +
                            "otp-resends-limit-reached-help-hints-title" + "|" + two + SmartParametersV2016.record_separator +
                            "otp-resends-limit-reached-help-hints" + "|" + three + SmartParametersV2016.record_separator +
                            "OtpExpiredContent" + "|" + four + SmartParametersV2016.record_separator +
                            "FirstPassnumberValue" + "|" + firstpassnumber + SmartParametersV2016.record_separator +
                            "SecondPassnumberValue" + "|" + secondpassnumber + SmartParametersV2016.record_separator +
                            "ThirdPassnumberValue" + "|" + thirdpassnumber + SmartParametersV2016.record_separator +
                            "OneTimePasscode" + "|" + onetimepasscode;

                // Now try and POST to get the digits
                Uri login_url = new Uri(url_base + passnumberandsmsotp_url);
                

                not_empty1 = SmartBanksV2019.Common_Build_Form_Strings_New(parameters);
                financeviewmodel.response = string.Empty;

                
                if (!await SmartBanksV2019.DoPost_NewX(
                                ourviewmodel,
                                financeviewmodel,
                                financeviewmodel.finance_token,
                                login_url,
                                url_base,
                                log,
                                1,
                                "POST",
                                not_empty1,
                                "https://onlinebanking.nationwide.co.uk/AccessManagement/Login/IdentifyCustomerForLogin",
                                "https://onlinebanking.nationwide.co.uk"))

                {
#if WINFORMS
                    textBoxConsole.AppendText("Problem: " + "Login" +
                                            Environment.NewLine.ToString());
                    textBoxConsole.ScrollToCaret();
#endif
                }

#if WINFORMS
                textBoxConsole.AppendText("Success: " + "Login" +
                                            Environment.NewLine.ToString());
                textBoxConsole.ScrollToCaret();
#endif
                //if (NW_Get_Json_String(ref financeviewmodel.response))
                //{
                //    await SmartRoutinesV2018.TextBlockUpdate(
                //                                        ourviewmodel,
                //                                        "Received passnumber positions");

                //    string FirstPassnumberValue = string.Empty,
                //            SecondPassnumberValue = string.Empty,
                //            ThirdPassnumberValue = string.Empty;
                //    financeviewmodel.response = financeviewmodel.response.Replace("{", string.Empty);
                //    financeviewmodel.response = financeviewmodel.response.Replace("}", string.Empty);
                //    financeviewmodel.response = financeviewmodel.response.Replace("\"", string.Empty);
                //    financeviewmodel.response = financeviewmodel.response.Replace(":", string.Empty);
                //    financeviewmodel.response = financeviewmodel.response.Replace(" ", string.Empty);
                //    financeviewmodel.response = financeviewmodel.response.Replace("st", string.Empty);
                //    financeviewmodel.response = financeviewmodel.response.Replace("nd", string.Empty);
                //    financeviewmodel.response = financeviewmodel.response.Replace("rd", string.Empty);
                //    financeviewmodel.response = financeviewmodel.response.Replace("th", string.Empty);
                //    int pass_count = 0;
                //    string[] positions = financeviewmodel.response.Split(',');
                //    foreach (string position in positions)
                //    {
                //        short index = Convert.ToInt16(position);
                //        if (index > 0)
                //        {
                //            index = (short)(index - 1);
                //            switch (pass_count)
                //            {
                //                case 0:
                //                    FirstPassnumberValue = passcode.Substring(index, 1);
                //                    break;
                //                case 1:
                //                    SecondPassnumberValue = passcode.Substring(index, 1);
                //                    break;
                //                case 2:
                //                    ThirdPassnumberValue = passcode.Substring(index, 1);
                //                    break;
                //                default:
                //                    break;
                //            }
                //        }
                //        pass_count++;
                //    }
                //    now = SmartEncryptionV2016.DateTimeNow(ourviewmodel.gmt_offset).ToString("ddd MMM dd yyyy hh:mm:ss"); // Doesn't appear to be 24 hr time - problem?
                //                                                                                                          //now = now + (" GMT+0000 (GMT Standard Time)");
                //    now = now + " GMT+0000 " + "(" + "GMT Standard Time" + ")";

                //string data = "__token" + "|" + token + SmartParametersV2016.record_separator +
                //            "PersistentCookiesEnabled" + "|" + "true" + SmartParametersV2016.record_separator +
                //            "ScreenResolution" + "|" + "1280 x 768" + SmartParametersV2016.record_separator +
                //            "TimeZoneOffset" + "|" + "0" + SmartParametersV2016.record_separator +
                //            "LocalTime" + "|" + now + SmartParametersV2016.record_separator +
                //            "SpecifiedStartPage" + "|" + "" + SmartParametersV2016.record_separator +
                //            "PassnumberDigitsLoaded" + "|" + "true" + SmartParametersV2016.record_separator +
                //            "CustomerNumber" + "|" + customer_no + SmartParametersV2016.record_separator +
                //            "RememberCustomerNumber" + "|" + "false" + SmartParametersV2016.record_separator +
                //            "MemorableData" + "|" + memorable_data + SmartParametersV2016.record_separator +
                //            "FirstPassnumberValue" + "|" + FirstPassnumberValue + SmartParametersV2016.record_separator +
                //            "SecondPassnumberValue" + "|" + SecondPassnumberValue + SmartParametersV2016.record_separator +
                //            "ThirdPassnumberValue" + "|" + ThirdPassnumberValue;
                //StringContent not_empty2 = SmartBanksV2019.Common_Build_Form_Strings(data);
                //financeviewmodel.response = string.Empty;
                //Uri login_url = new Uri(url_base + "/AccessManagement/Login/LoginViaMemorableDataAndPassnumber");

                //await SmartRoutinesV2018.TextBlockUpdate(
                //                                    ourviewmodel,
                //                                    "Sending passnumbers and memorable data");


                //if (await SmartBanksV2019.DoPost_NewX(
                //        ourviewmodel,
                //        financeviewmodel,
                //        financeviewmodel.finance_token,
                //        login_url,
                //        url_base,
                //        log,
                //        1,
                //        "POST",
                //        not_empty2,
                //        referer))

                //{

                await SmartRoutinesV2018.TextBlockUpdate(
                                                    ourviewmodel,
                                                    "Receiving accounts list response");


                // Should be able to get the list of accounts NOW!!
                // And I fucking DID!!  (Now, if only I could find my fucking keys ...)
                string accounts = string.Empty;
#if WINFORMS
                List<FinanceViewModel.AccountItem> these_accounts = new List<FinanceViewModel.AccountItem>();
#else
    List<FinanceViewModel.AccountItem> these_accounts = new List<FinanceViewModel.AccountItem>();
#endif

                string owner = string.Empty;
                Nationwide_Build_Account_List(financeviewmodel.html_document, ref owner, ref accounts);
                financeviewmodel.owner = owner;

                if (!string.IsNullOrEmpty(accounts))
                {
                    string[] accounts_list = accounts.Split(SmartParametersV2016.record_separator);
                    if (accounts_list.Count() > 0)
                    {
                        string account_name = string.Empty,
                                account_number = string.Empty,
                                sort_code = string.Empty,
                                balance = string.Empty;

                        await SmartRoutinesV2018.TextBlockUpdate(
                                                            ourviewmodel,
                                                            "Found " + accounts_list.Count() + " accounts");
                        int account_id = 0;
                        foreach (string account in accounts_list)
                        {
                            account_id++;
                            string[] comp = account.Split(SmartParametersV2016.unit_separator);
                            if (comp.Count() > 1)
                            {
                                string account_token = comp[0];
                                if (SmartFinanceV2021.Backwards_Split(ref account_token, SmartParametersV2016.space, ref balance))
                                {
                                    if (SmartFinanceV2021.Backwards_Split(ref account_token, SmartParametersV2016.space, ref account_number))
                                    {
                                        // Well .. there may not be a Sort Code!!!
                                        // Is the last character a digit?
                                        char last_charx = Convert.ToChar(account_token.Substring(account_token.Length - 1, 1));
                                        if (Char.IsDigit(last_charx))
                                        {
                                            if (SmartFinanceV2021.Backwards_Split(ref account_token, SmartParametersV2016.space, ref sort_code))
                                            {
                                                // Does this set the Account Type?????
                                                last_charx = SmartParametersV2016.singleaccounttype;
                                            }
                                        }
                                        account_name = account_token;
                                        // Be Bold!!!
                                        sort_code = sort_code.Replace("-", string.Empty);
                                        string val = sort_code + SmartParametersV2016.unit_separator +
                                                        account_number +
                                                        ":" +
                                                        "GBP" +     // Assume we can find it in OpenBanking
                                                        ":" +
                                                        balance +
                                                        ":" +
                                                        comp[1].Replace("&amp;", "&");    // Safety check
                                        string[] array = val.Split(':');
#if WINFORMS
                                        FinanceViewModel.AccountItem account_item = new FinanceViewModel.AccountItem
#else
                            FinanceViewModel.AccountItem account_item = new FinanceViewModel.AccountItem
#endif
                                        {
                                            Account_ID = account_id,
                                            Content = account_name,
                                            Value = array.ToString() // Here we have [0] = sort_code and accoount_no
                                                                        //             [1] = balance
                                                                            //              [2] = href
                                            };
                                            these_accounts.Add(account_item);

                                        }
                                    }
                                }
                            }
                            financeviewmodel.FinanceAccounts_List = these_accounts;
                        }
                    }
                    
                

                if (!found_it)
                {

                    await SmartRoutinesV2018.TextBlockUpdate(
                                                        ourviewmodel,
                                                        "Not found");
                }
            }
            return true;
        }

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
        internal static bool SplitDob(string dob, 
                                        ref string day,
                                        ref string month,
                                        ref string year)
        {
            if (dob.Length == 8)
            {
                try
                {
                    day = dob.Substring(0, 2);
                    int month_index = Convert.ToInt16(dob.Substring(2, 2)) - 1;
                    month = SmartParametersV2016.months[month_index];
                    year = dob.Substring(4, 4);
                    return true;
                }
                catch
                { 
                    // Anything that goes wrong return false
                }
            }
            return false;
        }

        internal static bool Look_For_LoginLink(FinanceViewModel financeviewmodel,
                                                ref string href,
                                                ref string url_base)
        {
            bool result = false;

            IList<HtmlAgilityPack.HtmlNode> HtmlCol1;

            string classname;

            href = url_base = string.Empty;

            HtmlCol1 = YetAnotherFuckingHoop.SelectNodesAsList(financeviewmodel.html_document.DocumentNode, "//a");
            foreach (HtmlAgilityPack.HtmlNode element1 in HtmlCol1)
            {
                classname = element1.GetAttributeValue("class", string.Empty);
                if (!string.IsNullOrEmpty(classname))
                {
                    if (classname.Length >= 9)
                    {
                        if (classname.Substring(0, 9) == "LoginLink")
                        {
                            href = element1.GetAttributeValue("href", string.Empty);
                            href = href.Replace("&amp;", "&");

                            int index = href.IndexOf(SmartParametersV2016.https);
                            if (index >= 0)
                            {
                                index = index + SmartParametersV2016.https.Length;
                                if (href.Length > index)
                                {
                                    int slash = href.IndexOf("/", index);
                                    if (slash >= 0)
                                    {
                                        url_base = href.Substring(0, slash);
                                        result = true;
                                    }
                                }
                            }
                            break;
                        }
                    }
                }
            }
            return result;
        }

        internal static async Task<List<SmartFinance.BankTransactions>> Nationwide_Find_Transactions(
                                                                        MainViewModel ourviewmodel,
                                                                        FinanceViewModel financeviewmodel,
                                                                        bool log,
                                                                        string referer,
                                                                        string url_base,
                                                                        string url_href,
                                                                        string sortcode,
                                                                        string account_no)
        //List<SmartFinance.TransactionTypes> transaction_types_list,
        //List<SmartFinance.TransactionSubTypes> transaction_sub_types_list)
        {
            string //inner_text,
                   //   acct_type_tag,
                   // account_address,
                   // classname,
                   //href,
                        token;

            string //response_type,
                    response_itself = url_href;

            string currency = "GBP",    // Pull this in from the account
                    amount = string.Empty,
                    date = string.Empty,
                    description = string.Empty,
                    balance = string.Empty,
                    sequence_no,
                    transaction_type = string.Empty;
            bool is_paid_in = false;
            short transaction_code = 0,
                    transaction_sub_code = 0;

            HtmlAgilityPack.HtmlDocument document = new HtmlAgilityPack.HtmlDocument();

            List<SmartFinance.BankTransactions> Bollocks = new List<SmartFinance.BankTransactions>();

            Uri next_url = new Uri(url_base + response_itself);  // Response_itself shouldn't be empty

            if (await SmartBanksV2019.DoGet_Finance(
                                                ourviewmodel,
                                                financeviewmodel,
                                                financeviewmodel.finance_token,
                                                next_url,
                                                url_base,
                                                log,
                                                "GET",
                                                referer))

            {
                token = Nationwide_Find_Script_Token(document);
                // Now you need to try and POST some dates and see what you get back
                // which will be in JSON so it will need decoding?  You will need to
                // keep their SDf (stupid Date Format) as a field so you can compare
                // Good luck old son - nearly there!!
                if (!string.IsNullOrEmpty(token))
                {
                    // Fix local time
                    DateTime time_now = SmartEncryptionV2016.DateTimeNow(SmartParametersV2016.gmtdefaultOffset);
                    string end_date = time_now.ToString("dd/MM/yyyy");
                    string start_date = time_now.AddMonths(-12).ToString("dd/MM/yyyy");
                    string data = "__token" + "|" + token + SmartParametersV2016.record_separator +
                                    "start" + "|" + start_date + SmartParametersV2016.record_separator +
                                    "end" + "|" + end_date + SmartParametersV2016.record_separator +
                                    "filters" + "|" + "ConfirmInfoRead" + SmartParametersV2016.record_separator +
                                    "filters" + "|" + "TransactionFilterMapping.AllMoneyPaidOut" + SmartParametersV2016.record_separator +
                                    "filters" + "|" + "TransactionFilterMapping.CashWithdrawal" + SmartParametersV2016.record_separator +
                                    "filters" + "|" + "TransactionFilterMapping.ChequeWithdrawal" + SmartParametersV2016.record_separator +
                                    "filters" + "|" + "TransactionFilterMapping.DirectDebits" + SmartParametersV2016.record_separator +
                                    "filters" + "|" + "TransactionFilterMapping.ChargesOut" + SmartParametersV2016.record_separator +
                                    "filters" + "|" + "TransactionFilterMapping.DebitCardPayments" + SmartParametersV2016.record_separator +
                                    "filters" + "|" + "TransactionFilterMapping.RegularPaymentsOutside" + SmartParametersV2016.record_separator +
                                    "filters" + "|" + "TransactionFilterMapping.SinglePaymentsOutside" + SmartParametersV2016.record_separator +
                                    "filters" + "|" + "TransactionFilterMapping.TransfersToNationwide" + SmartParametersV2016.record_separator +
                                    "filters" + "|" + "TransactionFilterMapping.ReturnedItems" + SmartParametersV2016.record_separator +
                                    "filters" + "|" + "TransactionFilterMapping.MiscellaneousDebits" + SmartParametersV2016.record_separator +
                                    "filters" + "|" + "TransactionFilterMapping.AllMoneyPaidIn" + SmartParametersV2016.record_separator +
                                    "filters" + "|" + "TransactionFilterMapping.CashCredits" + SmartParametersV2016.record_separator +
                                    "filters" + "|" + "TransactionFilterMapping.ChequeCredits" + SmartParametersV2016.record_separator +
                                    "filters" + "|" + "TransactionFilterMapping.BankCredits" + SmartParametersV2016.record_separator +
                                    "filters" + "|" + "TransactionFilterMapping.TransfersFromNationwideAccount" + SmartParametersV2016.record_separator +
                                    "filters" + "|" + "TransactionFilterMapping.DebitCardTransactions" + SmartParametersV2016.record_separator +
                                    "filters" + "|" + "TransactionFilterMapping.UnpaidItems" + SmartParametersV2016.record_separator +
                                    "filters" + "|" + "TransactionFilterMapping.ChargesIn" + SmartParametersV2016.record_separator +
                                    "filters" + "|" + "TransactionFilterMapping.MiscellaneousCredits";
                    var not_empty2 = SmartBanksV2019.Common_Build_Form_Strings_New(data);
                    response_itself = string.Empty;
                    Uri transactions_url = new Uri(url_base + "/Transactions/FullStatement/FullStatement");

                    await SmartRoutinesV2018.TextBlockUpdate(
                                                            ourviewmodel,
                                                            "Requesting transactions list");

                    if (await SmartBanksV2019.DoPost_NewX(
                                                        ourviewmodel,
                                                        financeviewmodel,
                                                        financeviewmodel.finance_token,
                                                        transactions_url,
                                                        url_base,
                                                        log,
                                                        0,
                                                        "POST",
                                                        not_empty2,
                                                        referer))

                    {

                        await SmartRoutinesV2018.TextBlockUpdate(
                                                            ourviewmodel,
                                                            "Received transactions list");

                        // ONE DATE for all of this Batch of transactions!!
                        //DateTime transaction_created = SmartEncryptionV2016.DateTimeNow(ourviewmodel.gmt_offset);   // GMT time

                        DateTime converted_date = SmartParametersV2016.default_date;

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
                                                    int sep = amount.IndexOf(SmartParametersV2016.default_currency_separator);
                                                    if (sep == -1)
                                                    {
                                                        amount = amount + SmartParametersV2016.default_currency_separator + "00";
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

                                                        await SmartBobV2017.ListenerAsync(ourviewmodel, financeviewmodel.finance_token, financeviewmodel.institution_code, financeviewmodel.brand_code, ex.Message + ": " + date);

                                                    }
                                                    break;
                                                case "Description":
                                                    description = individual.Value;
                                                    break;
                                                case "IsPaidIn":
                                                    is_paid_in = Convert.ToBoolean(individual.Value);
                                                    break;
                                                case "RunningBalance":
                                                    balance = individual.Value;
                                                    int sep2 = balance.IndexOf(SmartParametersV2016.default_currency_separator);
                                                    if (sep2 == -1)
                                                    {
                                                        balance = balance + SmartParametersV2016.default_currency_separator + "00";
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

                                        if (!SmartSpikeFinanceV2017.Lookup_Transaction_SubCode(financeviewmodel,
                                                                                                    is_paid_in,
                                                                                                    ref transaction_type,
                                                                                                    ref transaction_code,
                                                                                                    ref transaction_sub_code,
                                                                                                    false))
                                        {

                                            await SmartBobV2017.ListenerAsync(ourviewmodel, financeviewmodel.finance_token, financeviewmodel.institution_code, financeviewmodel.brand_code, "Trans type  failed: " + transaction_type);

                                        }

                                        // Still want to add it in even if we can't analyze the transaction ...
                                        int random1 = SmartRoutinesV2018.Get_Next_Random(ourviewmodel.random_r);
                                        int random2 = SmartRoutinesV2018.Get_Next_Random(ourviewmodel.random_r);

                                        SmartFinance.BankTransactions abc = new SmartFinance.BankTransactions()
                                        {
                                            USERNAME = ourviewmodel.UserName,
                                            INSTITUTION_CODE = financeviewmodel.institution_code,
                                            BRAND_CODE = financeviewmodel.brand_code,
                                            SORTCODE = sortcode,
                                            ACCOUNT_NO = account_no,
                                            RANDOMKEY1 = random1,
                                            BOOKING_DATE = converted_date,
                                            SEQUENCE_NO = new DateTimeOffset(DateTime.Now + ourviewmodel.gmt_offset).ToUnixTimeMilliseconds(), // Unique AND increasing
                                            CREDITDEBIT_INDICATOR = is_paid_in,
                                            TRANSACTION_CODE = transaction_code,
                                            TRANSACTION_SUB_CODE = transaction_sub_code,
                                            DESCRIPTION = description,
                                            AMOUNT = Convert.ToInt32(amount.Replace(".", string.Empty)),
                                            CURRENCY = currency,     // Pull this in from the Account
                                            BALANCE_AMOUNT = Convert.ToInt32(balance.Replace(".", string.Empty)),
                                            RANDOMKEY2 = random2
                                            //Updated = false
                                        };

                                        Bollocks.Add(abc);

                                    }
                                }
                            }
                        }

                        await SmartRoutinesV2018.TextBlockUpdate(
                                                            ourviewmodel,
                                                            "Found: " + Bollocks.Count + " transactions");
                    }
                    else
                    {

                        await SmartRoutinesV2018.TextBlockUpdate(
                                                            ourviewmodel,
                                                             "Failed: Asked for transactions");
                    }
                }
            }
            return Bollocks;    // Which may be empty ...
        }

        internal static bool NW_Get_SMSOTP_String(string reply,
                                                    ref string SmsResendsRemaining,
                                                    ref string NemOtpExpiryTime)
        {
            string FinanceLog = string.Empty;

            try
            {
                //string reply = "{\"SmsResendsRemaining\":\"\",\"NemOtpExpiryTime\":\"\\/Date(1630783419027)\\/\"}";
                dynamic jsonResponse = JObject.Parse(reply);
                foreach (dynamic statement in jsonResponse)
                {
                    string statement_name = statement.Name;
                    switch (statement_name)
                    {
                        case "SmsResendsRemaining":
                            SmsResendsRemaining = statement.Value;
                            //Console.WriteLine(amount);
                            break;
                        case "NemOtpExpiryTime":
                            NemOtpExpiryTime = statement.ToString();
                            NemOtpExpiryTime = NemOtpExpiryTime.Replace("NemOtpExpiryTime", string.Empty);
                            NemOtpExpiryTime = NemOtpExpiryTime.Replace("\"", string.Empty).TrimStart(':').Trim();
                            //Console.WriteLine(NemOtpExpiryTime);
                            //if (NemOtpExpiryTime == comp)
                            //{
                            //    Console.WriteLine("They match!");
                            //}
                            break;
                        default:
                            break;
                    }
                }
                return true;
            }
            catch (Exception exception)
            {
                FinanceLog = FinanceLog + exception.Message + Environment.NewLine.ToString();

                //reply = FinanceLog;
                //return false;
            }
            //reply = string.Empty;
            return false;
        }

        internal static bool NW_Get_Json_String(ref string reply)
        {
            string FinanceLog = string.Empty;
            try
            {
                reply = reply.Replace("[", "{");
                reply = reply.Replace("]", "}");
                // adding 'empty' values to all the variables .. what a bag of shit this stuff is ...
                reply = reply.Replace(" digit\"", "\":\"\"");
                JObject jsonResponse = JObject.Parse(reply);
                if (jsonResponse != null)
                {
                    reply = jsonResponse.ToString().Replace(Environment.NewLine, string.Empty);
                    return true;
                }
            }
            catch (Exception exception)
            {
                FinanceLog = FinanceLog + exception.Message + Environment.NewLine.ToString();

                reply = FinanceLog;
                return false;
            }
            reply = string.Empty;
            return false;
        }



        internal static void Nationwide_Build_Account_List(HtmlAgilityPack.HtmlDocument document,
                                                ref string owner,
                                                ref string accounts)
        {
            IList<HtmlAgilityPack.HtmlNode> HtmlCol1,
                    HtmlCol2,
                    HtmlCol3,
                    HtmlCol4,
                    HtmlCol5,
                    HtmlCol6;

            // < div id = "header" >
            //< header >
            //< div class="welcome-util-container">
            //  <div id = "welcome-message" > Welcome back, Genius</div>

            HtmlCol1 = YetAnotherFuckingHoop.SelectNodesAsList(document.DocumentNode, ".//div");
            foreach (HtmlAgilityPack.HtmlNode element1 in HtmlCol1)
            {
                if (element1.Id == "header")
                {
                    HtmlCol2 = YetAnotherFuckingHoop.SelectNodesAsList(element1, ".//div");
                    foreach (HtmlAgilityPack.HtmlNode element2 in HtmlCol2)
                    {
                        string classname = element2.GetAttributeValue("class", string.Empty);
                        if (classname == "welcome-util-container")
                        {
                            HtmlCol3 = YetAnotherFuckingHoop.SelectNodesAsList(element2, ".//div");
                            foreach (HtmlAgilityPack.HtmlNode element3 in HtmlCol3)
                            {
                                if (element3.Id == "welcome-message")
                                {
                                    owner = element3.InnerText.Replace("Welcome back,", string.Empty).Trim();
                                    break;
                                }
                            }
                        }
                        if (!string.IsNullOrEmpty(owner))
                        {
                            break;
                        }
                    }
                }
                if (!string.IsNullOrEmpty(owner))
                {
                    break;
                }
            }

            HtmlCol1 = YetAnotherFuckingHoop.SelectNodesAsList(document.DocumentNode, "//nav");
            foreach (HtmlAgilityPack.HtmlNode element1 in HtmlCol1)
            {
                if (element1.Id == "primary-nav")
                {
                    HtmlCol2 = YetAnotherFuckingHoop.SelectNodesAsList(element1, ".//ul");
                    foreach (HtmlAgilityPack.HtmlNode element2 in HtmlCol2)
                    {
                        HtmlCol3 = YetAnotherFuckingHoop.SelectNodesAsList(element2, ".//li");
                        foreach (HtmlAgilityPack.HtmlNode element3 in HtmlCol3)
                        {
                            string classname = element3.GetAttributeValue("class", string.Empty);
                            if (classname == "active")
                            {
                                HtmlCol4 = YetAnotherFuckingHoop.SelectNodesAsList(element3, ".//ul");
                                foreach (HtmlAgilityPack.HtmlNode element4 in HtmlCol4)
                                {
                                    HtmlCol5 = YetAnotherFuckingHoop.SelectNodesAsList(element4, ".//li");
                                    foreach (HtmlAgilityPack.HtmlNode element5 in HtmlCol5)
                                    {
                                        HtmlCol6 = YetAnotherFuckingHoop.SelectNodesAsList(element5, ".//a");
                                        foreach (HtmlAgilityPack.HtmlNode element6 in HtmlCol6)
                                        {
                                            string href = element6.GetAttributeValue("href", string.Empty);
                                            href = href.Replace("&amp;", "&");
                                            if (!string.IsNullOrEmpty(element6.InnerText))
                                            {
                                                string inner_text = element6.InnerText;
                                                if (inner_text.IndexOf("Overview") != 0)
                                                {
                                                    string html = element6.InnerHtml;
                                                    html = html.Replace("<br>", SmartParametersV2016.space).Trim();
                                                    if (string.IsNullOrEmpty(accounts))
                                                    {
                                                        accounts = html +
                                                                    SmartParametersV2016.unit_separator +
                                                                    href;
                                                    }
                                                    else
                                                    {
                                                        accounts = accounts +
                                                                    SmartParametersV2016.record_separator +
                                                                     html +
                                                                    SmartParametersV2016.unit_separator +
                                                                     href;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return;
        }

        internal static string Nationwide_Find_Token(HtmlAgilityPack.HtmlDocument document, string id)
        {
            IList<HtmlAgilityPack.HtmlNode> HtmlCol1;
            string token = string.Empty;

            HtmlCol1 = YetAnotherFuckingHoop.SelectNodesAsList(document.DocumentNode, "//input");
            foreach (HtmlAgilityPack.HtmlNode element1 in HtmlCol1)
            {
                if (!string.IsNullOrEmpty(element1.Id))
                {
                    if (element1.Id == id) // SmsOtp_passNoEntryPositions
                    {
                        //string name = element1.GetAttributeValue("name", string.Empty);
                        //if (name == "__token")
                        //{
                            token = element1.GetAttributeValue("value", string.Empty);
                            return token;
                        //}
                    }
                }
            }
            return token;
        }

        internal static string Nationwide_Find_Form(HtmlAgilityPack.HtmlDocument document, string id,
                                                        string target_name,
                                                        ref string urlval)
        {
            IList<HtmlAgilityPack.HtmlNode> HtmlCol1,
                                            HtmlCol2;
            string token = string.Empty;
            string name;

            HtmlCol1 = YetAnotherFuckingHoop.SelectNodesAsList(document.DocumentNode, "//form");
            foreach (HtmlAgilityPack.HtmlNode element1 in HtmlCol1)
            {
                urlval = element1.GetAttributeValue("action", string.Empty);
                HtmlCol2 = YetAnotherFuckingHoop.SelectNodesAsList(element1, ".//input");
                foreach (HtmlAgilityPack.HtmlNode element2 in HtmlCol2)
                {
                    if (!string.IsNullOrEmpty(element2.Id))
                    {
                        if (element2.Id == id) //"accessmanagementloginloginviamemorabledataandpassnumber")
                        {
                            name = element2.GetAttributeValue("name", string.Empty);
                            if (name == target_name) // "__token")
                            {
                                token = element2.GetAttributeValue("value", string.Empty);
                                return token;
                            }
                        }
                    }
                }
            }
            return token;
        }
        internal static string Nationwide_Find_Script_Token(HtmlAgilityPack.HtmlDocument document)
        {
            IList<HtmlAgilityPack.HtmlNode> HtmlCol1;
            string token = string.Empty;
            bool found_url = false;

            HtmlCol1 = YetAnotherFuckingHoop.SelectNodesAsList(document.DocumentNode, "//script");
            foreach (HtmlAgilityPack.HtmlNode element1 in HtmlCol1)
            {
                string tipe = element1.GetAttributeValue("type", string.Empty);
                if (tipe == "text/javascript")
                {
                    if (!string.IsNullOrEmpty(element1.InnerText))
                    {
                        string[] components = element1.InnerText.Split(',');
                        foreach (string component in components)
                        {
                            if (component.IndexOf("url:") >= 0)
                            {
                                string text = component.Replace("url:", string.Empty).Trim();
                                text = text.Trim('\'');
                                if (text == "/Transactions/FullStatement/FullStatement")
                                {
                                    found_url = true;
                                }
                            }
                            else
                            {
                                if (component.IndexOf("token:") >= 0)
                                {
                                    string text = component.Replace("token:", string.Empty).Trim();
                                    text = text.Trim('\'');
                                    if (found_url)
                                    {
                                        token = text;
                                        return token;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return token;
        }
    }
}