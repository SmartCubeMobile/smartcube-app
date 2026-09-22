// YOU CANNOT TEST THIS FROM THE SILVERLIGH OUT-OF-BROWSER VERSION
// BECAUSE IT ADDS IN A REFERER WHICH IS NOT ACCEPTED BY THE REMOTE HOST
//
// YOU HAVE TO RUN IT AS EITHER A NON-LIGHTSILVER APPLICATION OR AS
// A LIGHTSILVER IN-BROWSER APPLICATION WITH ALL THE HTTP GETs AND POSTs
// SCRAPED AT THE BACK-END NOT AT THE METER FRONT-END I.E UNDER THE 'USEPROXY'
// SETTING.  EVEN IF THE PROGRAM IS SIGNED AND TRUSTED THE REFERER SHIT IS
// *STILL* ATTACHED WHETHER YOU LIKE IT OR NOT.  YOU HAVE BEEN WARNED!
//                                               ===================== 
// It all goes to rat-shit if you are not either:
// 1. Non-Silverligh stand-alone or
// 2. Silverligh out-of-browser and non-trusted
// 3. Silverligh in-browser with 'USEPROXY' flag set
//

//  British Gas accounting is a complete dog’s breakfast.

//  It is executed from THEIR point of view and not the Consumer’s.If, for example the Outstanding Balance is in credit then it means the Consumer owes them and not the other way around.  And if your Outstanding Balance is in debit, then it means that British Gas owes the Consumer and not the other way round!  Could you – would you – ever fucking believe it?  This is a statement FOR the Consumer and most of them won’t have a fucking Scooby-doo about how British Gas are ‘accounting’ their payments and usage.
//  Here is a classic example from Bob’s Gas bill of 3rd March 2012:
//  Balance brought forward:		£105.63	in credit   A
//  What you paid:				    £141.00			    B
//  Gas you’ve used this period:	£202.83			    C
//  Your direct debit discount:		£  6.59	credit      D
//  Your adjustment:				£105.00	refund      E
//  VAT at 5%:					    £  9.81			    F
//  Your new account balance:		£ 64.42	in debit    G
//  Here’s how they work it out:
//  +A + B - C + D - E - F = G  .. and because G > 0 its tagged as 'in credit'
//                                 which is good for the Consumer because it means
//                                 BG owe them
//  Notice how the refund E which we should hold as a PLUS figure is SUBTRACTED because
//  in double-entry book-keeping its send to your Bank Account where it shows as a CREDIT
//  so it must be shown as a DEBIT on the BG side

// BG are ABSOLUTE FUCKERS because in January 2013 they change the signs around!!
// So if you are IN CREDIT to the tune of £50 then that shows on your bill as -£50!!

//using System.Net.Http;  // No more of this fucking shit ... YES this fucking shit IS BACK
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;


#if WINFORMS || WPF
//using iText7;
#endif


#if WINFORMS
#endif
#if WPF
#endif

#if ANDROIDX
using AndroidX.AppCompat.App;
#endif
namespace SmartCubeMobile
{
    public class BritishGasV2016
    {
        // Jesus H. Christ - what a complete fucking pain in the arse this was -
        // there appeared NO WAY British Gas was going to let me cycle
        // round the Utility.Accounts and do them all in one fell swoop, so I had to do
        // them on two separate Logins.  What a complete and utter fucking pain
        // this was.  And there is no logical reason for it that I can see ...
        // The HTTP headers looked fine, but BG just would not return the right
        // data.  Was I missing something in the cookies???

        internal static async Task<bool> Read_Meter(
#if WINFORMS
                                            //WebBrowser Scraper,
#endif
#if ANDROIDX
                                            AppCompatActivity meterActivity,
#endif

                                            MainViewModel ourviewmodel,
                                            UtilityViewModel utilityviewmodel,
                                            bool TextBox_Active)

        {

            // We ONLY need this to convert HTML documents ... and when - one
            // day - we find a better way, this fucking useless object is BINNED
            // It is now fucking binned WebBrowser wb = new WebBrowser(); is BINNED
            // ========================                                   =========


            // To carry cookies across all GET and POST calls
            // ... so the Cookies ARE IMPORTANT because clearing them down after
            // the login, loses the fact that you are Logged-in i.e. ACCSUMMY keeps
            // returning the Login page  YOU NEED THE FUCKING COOKIES
            //                           ============================



            // Plan of Attack


            // THis is THE LAST FUCKING TIME I am re-writing British Gas ...
            string[][] tibs =
                // Common for Versions 1 & 2
                {
                        //Next Routine  Exit Test           Yes         No        Comments    Relative Pathname
            new string[] {"START",      "Success",          "Login",    "LOGOUT",   "Start",                            "identity/", "Finding the correct cookies" },
            new string[] {"Login",      "Success",          "YOURACCS", "LOGOUT",   "Login",                            "uaa/login", "" },  // A POST
            new string[] {"YOURACCS",   "Success",          "TESTLIST", "LOGOUT",   "Landing page - fix Amelia",        "Your_Account/Account_Details/", ""},
            new string[] {"TESTLIST",   "Fuel List Empty",  "LOGOUT",   "ACCSUMMY", "Determine first FLR from list",    "", "" },
            new string[] {"ACCSUMMY",   "Success",          "PERSONAL", "LOGOUT",   "Account Summary- fix address",     "", ""},
            new string[] {"PERSONAL",   "Success",          "MIDATA",   "LOGOUT",   "Determine personal data for FLR",  "Self-Service/Personal-Details-Entry/", "" },
            new string[] {"MIDATA",     "Success",          "VIEWPAYM", "LOGOUT",   "To find MIDATA info for reaource", "Account-History/Midata-result/?accountnumber=", "Account_No"},
            new string[] {"VIEWPAYM",   "Success",          "VIEWBILLS", "LOGOUT",  "Builds Bills list for FLR",        "Utility.Accountsummary/getPaymentHistoryNavigationDetails/", "" },
            new string[] {"VIEWBILLS",  "Success",          "TESTLIST", "LOGOUT",   "Works through Bills list for FLR", "", "" },
            new string[] {"LOGOUT",     "Success",          "<Caller>", "<Caller>", "Finish",                           "", ""},
            };

            // Because Sometimes we are returned a Document
            utilityviewmodel.htmlDocument = new HtmlAgilityPack.HtmlDocument();

            utilityviewmodel.keyValues = new List<KeyValuePair<string, string>>();

            // Everytime timer ticks, timer_Tick will be called
            // Timer will tick every second
            // FUCKING HELL - this is never Enabled????
            // You have 60 Seconds to stop
            // the timer on a good login!
            //string midata_pathname = "",
            //        youraccounts_pathname = ""; // "Your_Account/Account_Details/";
            utilityviewmodel.bills_tempList = new List<SmartUtility.Bills>();

            string next_routine = "",
                    //exit_test = "",
                    yes_routine = "",
                    no_routine = "",
                    comments = "",
                    relative_pathname = "";

            utilityviewmodel.keep_looping = true;
            utilityviewmodel.payments_tempList = new List<SmartUtility.Payments>();

            utilityviewmodel.next_routine = "";

            while (utilityviewmodel.keep_looping)
            {
                utilityviewmodel.next_routine = SmartParseV2016.Find_This_Routine(tibs,
                                                                        next_routine,
                                                                        utilityviewmodel);
                //rf exit_test,
                //rf yes_routine,
                //rf no_routine,
                //rf comments,
                //rf relative_pathname,
                //utilityviewmodel);
                utilityviewmodel.target_pathname = relative_pathname;    // Might be empty sometimes ...
#if WINFORMS
                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel,
                    "Comments: " + comments + Environment.NewLine.ToString());

                await SmartUtilityV2022.First_Throw(ourviewmodel,
                                                    utilityviewmodel);
                                                    //DateTime.Now + ourviewmodel.utcOffset); // Local time
#endif
#if WPF  || SMARTMAUI
                await SmartUtilityV2022.First_Throw(ourviewmodel, utilityviewmodel);
#endif
#if ANDROIDX
                await SmartUtilityV2022.First_Throw(ourviewmodel, utilityviewmodel);
#endif
                // Key values only apply to POSTs
                // "LOGIN" = GET
                // "Login" = POST

                if (!string.IsNullOrEmpty(utilityviewmodel.target_pathname))
                {
                    utilityviewmodel.keep_looping = await SmartUtilityV2022.Prelude(ourviewmodel, utilityviewmodel, utilityviewmodel.keyValues);
                }
#if WINFORMS
                //Scraper.DocumentText = utilityviewmodel.htmlDocument.DocumentNode.OuterHtml;
                //Scraper.Document.Body.ScrollRectangle.Size;
#endif
                utilityviewmodel.current_routine = utilityviewmodel.next_routine;
                switch (utilityviewmodel.next_routine)
                {

                    case "START":
                        // The timer is re-started when the
                        // first Login Document is completed
                        utilityviewmodel.keyValues.Clear();
                        string cookie_name = "CSRF-TOKEN";
                        AntiXsrfToken anti_token = new AntiXsrfToken();
                        if (SmartRoutinesV2018.DecodeCookies(ourviewmodel.website, //"m_domainTable",
                                    cookie_name,
                                    ourviewmodel.cookies,
                                    anti_token))
                        {
                            if (!string.IsNullOrEmpty(anti_token.cookie_value))
                            {
                                utilityviewmodel.keyValues.Add(new KeyValuePair<string, string>("_csrf", anti_token.cookie_value));
                                utilityviewmodel.keyValues.Add(new KeyValuePair<string, string>("username", utilityviewmodel.user_id));

                                if (TextBox_Active)
                                {
                                    await SmartRoutinesV2018.TextBlockUpdate(
#if ANDROIDX
                                        meterActivity,
#endif
                                        ourviewmodel,
                                        "UserId" +
                                        SmartParametersV2016.space +
                                        utilityviewmodel.user_id);
                                }

                                //Type the password in the password text box
                                utilityviewmodel.keyValues.Add(new KeyValuePair<string, string>("password", utilityviewmodel.password));

                                if (TextBox_Active)
                                {
                                    await SmartRoutinesV2018.TextBlockUpdate(
#if ANDROIDX
                                    meterActivity,
#endif
                                    ourviewmodel,
                                    "Password" +
                                    SmartParametersV2016.space +
                                    "Asterisks");
                                    // Should be going onto "Login"
                                }
                            }
                            else
                            {
                                ourviewmodel.errorMessage = comments;
                                yes_routine = no_routine;
                            }
                        }
                        else
                        {
                            yes_routine = no_routine;
                        }
                        break;
                    case "Login":
                        // Try to login
                        utilityviewmodel.login_attempted = true;
                        // But here, we don't know if its successful
                        // Should be going onto YOURACCS
                        break;
                    case "YOURACCS":
                        // **  DON'T clear the Cookie container here **
                        // The timer is re-started when the
                        // first Login Document is completed
                        if (BG_YourAccounts(utilityviewmodel,
                                                utilityviewmodel.htmlDocument))
                        {
                            // No we can set this because we have a good SUPPLY ADDRESS
                            utilityviewmodel.login_finished = true;

                            utilityviewmodel.href = "";
                            // Find the Logout path
                            SmartNibbyV2016.Find_A_Tag_Simple(utilityviewmodel,
                                                            utilityviewmodel.htmlDocument,
                                                            "//a",          // Tag to look for
                                                            true,           // Only consider null elements
                                                            "",   // remove from inner text
                                                            "Log out");     // inner_text_comparison,
                            SmartParseV2016.Update_Tibs(tibs,
                                                        "LOGOUT",
                                                        utilityviewmodel.href);

#if WINFORMS
                            if (utilityviewmodel.console)
                            {
                                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel,
                                    "Account No: " + utilityviewmodel.account_no + Environment.NewLine.ToString());
                                //await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel,
                                //    "UDPRN: " + utilityviewmodel.sparks.udprn + SmartParametersV2016.space +
                                //                            utilityviewmodel.smell.udprn + Environment.NewLine.ToString());
                                //
                                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel,
                                    "Target pathname: " + utilityviewmodel.target_pathname + Environment.NewLine.ToString());
                            }
#endif
                            SmartParseV2016.Update_Tibs(tibs, "ACCSUMMY", utilityviewmodel.target_pathname);
                        }
                        else
                        {
                            yes_routine = no_routine;

                        }
                        break;
                    case "TESTLIST":
                        if (!string.IsNullOrEmpty(utilityviewmodel.fuelList))
                        {
                            string resource_type = "";
                            SmartParseV2016.Strip_FuelList(utilityviewmodel);
                            SmartUtilityV2022.Fix_Resource_Type(utilityviewmodel, resource_type);
                            yes_routine = no_routine;   // Should go to PERSONAL
                        }
                        break;
                    case "ACCSUMMY":
                        if (!await BG_Accountsummary(ourviewmodel,
                                                    utilityviewmodel,
                                                    utilityviewmodel.htmlDocument))
                        {
                            yes_routine = no_routine;
                        }
                        break;
                    case "PERSONAL":
                        BG_Personal(utilityviewmodel,
                                            utilityviewmodel.htmlDocument);
                        // Should be going on to MIDATA
                        break;
                    case "MIDATA":
                        BG_Midata(ourviewmodel,
                                    utilityviewmodel,
                                    utilityviewmodel.htmlDocument);
                        // Should be going back on to PAYMENTS
                        break;
                    case "VIEWBILLS":
                        // Loop round the ViewBills list
                        if (!await BG_Read_Bills(
#if ANDROIDX
                                            meterActivity,
#endif
                                            ourviewmodel,
                                            utilityviewmodel,
                                            TextBox_Active,
                                            utilityviewmodel.bills_tempList))
                        {
                            yes_routine = no_routine;
                        }
                        break;
                    case "LOGOUT":
                        utilityviewmodel.login_finished = true;          // Escape route on Login failure
                        // Combine the Bills_In and Bills_Out lists and
                        // only include unallocated payments past the last Bill Period end

                        // This will work because Payments_Out contains payments DECODED from any
                        // new Bills PLUS payments added from the SCRAPE which is stored in Payments_Temp
                        SmartUtilityV2022.Generic_Payment_Done(utilityviewmodel, utilityviewmodel.payments_tempList);

                        await SmartUtilityV2022.Common_Logout(
#if ANDROIDX
                                                meterActivity,
#endif
                                                ourviewmodel,
                                                utilityviewmodel,
                                                TextBox_Active);
                        if (!string.IsNullOrEmpty(ourviewmodel.errorMessage))
                        {
                            return false;
                        }
                        break;
                    default:
                        // A small price to pay ...
                        BG_Do_Intermediates(ourviewmodel,
                                            utilityviewmodel,
                                            utilityviewmodel.next_routine,
                                            utilityviewmodel.htmlDocument,
                                            no_routine);
                        break;
                }
                next_routine = utilityviewmodel.yes_routine;
            }
            // Now .. there is no way of telling whether we are logged in or
            // not as we don't ever test for the "Login failed" message.
            // HOWEVER, if we can't find any Utility.Accounts in the list then we can
            // safely assume that the login has failed because we won't have been
            // presented with the "Your Account" screen to find any Utility.Accounts.
            // It is a logical contradiction that we COULD have successfully logged
            // in AND then found no Utility.Accounts.  We can safely assume that this
            // combination of events can never happen, it makes logical sense to
            // safely assume that NO Utility.Accounts = LOGIN FAILED.
            await SmartUtilityV2022.Last_Throw(
#if ANDROIDX
                                            meterActivity,
#endif
                                            ourviewmodel, utilityviewmodel);

            // Yay!
            return utilityviewmodel.login_finished;
        }

        private static void BG_Do_Intermediates(MainViewModel ourviewmodel,
                                                UtilityViewModel utilityviewmodel,
                                                string next_routine,
                                                HtmlAgilityPack.HtmlDocument htmlDocument,
                                                string no_routine)
        {
            switch (next_routine)
            {

                //case "PAYMENTS":
                //    BG_Payments(htmlDocument,
                //                    utilityviewmodel);
                //    break;
                case "VIEWPAYM":
                    utilityviewmodel.bills_tempList.Clear();
                    utilityviewmodel.payments_tempList.Clear();
                    // This goes down the list and does Bills AND Payments
                    if (!BG_ViewPayments(ourviewmodel,
                                            utilityviewmodel,
                                            htmlDocument))
                    {
                        utilityviewmodel.yes_routine = no_routine;
                    }
                    // Next routine should be VIEWBILLS
                    // This problem brought me near to suicide ...
                    // This program works normally but in FUCKING SILVERLIGH
                    // WRITTEN BY FUCKING KIDS WHO HAVE **never** WORKED IN A COMMERCIAL
                    // ENVIRONMENT THESE FUCKERS ACTUALLY CONFIGURED SILVERLIGH TO CACHE
                    // URLS IT HAD ALREADY HHTP-ed.  THE RESULT IS THAT I WAS COMPLETELY
                    // FUCKED BY THESE STUPID IDIOTIC DUMB JUVENILE SPOTTY THICK STUPID
                    // KID FUCKING WANKHERS WHO **automatically**  I.E. DIDN'T EVEN GIVE
                    // ME A FUCKING0 GHOST OF A CHANCE TO CIRCUMEVENT IT, TO FUCKING
                    // CACHE URLS.  So View/Bills/Consumption_Details always went to
                    // the first (cached) bill despite me looking at other fucking bills
                    // What a bunch of pathetic immature syupid ignorant arrogant fucking
                    // cunts these fuckers at Microsoft are
                    break;
            }
            return;
        }

        private static string Determine_Bill_Number(string STATEMENT_ID)
        {
            // The temp Bill Id contains the entire path, the number after the =
            // is the bill id number
            string[] components = STATEMENT_ID.Split(SmartParametersV2016.equalsSplit);
            if (components.Length > 1)
            {
                // Should do this BEFORE the row is stored away, really
                components[1] = components[1].Replace("&accountNumber", "").Trim();
            }
            return components[1];
        }

        private static bool BG_YourAccounts(UtilityViewModel utilityviewmodel,
                                            HtmlAgilityPack.HtmlDocument document)
        {
            // Fuck me!  It fucking WORKED!!!! See the re-direction to this 'secure'
            // routine from the initial fucking Login routine above!!!!!!!!!!!!
            // You ARE a fucking genius Ray!!  An absolute bravest of the brave
            // fucking genius.  Those visionless cunts you saw last Thursday have
            // ABSOLUTELY no idea what they are missing out on ....

            IList<HtmlAgilityPack.HtmlNode> HtmlCol1,
                        HtmlCol2,
                        HtmlCol3;
            string inner_text,
                        classname,
                        href;
            bool clicked = false;

            utilityviewmodel.acct_type_tag = "";

            //bool update_meters = false,
            //        update_addresses = false,
            //        update_account = false;

            HtmlCol1 = YetAnotherFuckingHoop.SelectNodesAsList(document.DocumentNode, "//div");
            foreach (HtmlAgilityPack.HtmlNode element1 in HtmlCol1)
            {
                classname = element1.GetAttributeValue("class", "");
                if (SmartNibbyV2016.Check_Classname(classname, "cust-info fleft", true))
                {
                    // we wouldn't be HERE unless we were logged in successfully
                    // Set this flag as were are 99.9% sure that we are logged-in 
                    // NO its not until we get a SUPPLYA DDRESS

                    // ** So this gets fixed properly on each 'loop' through when
                    //    we finally figure out how to do a DUAL look-up
                    //    I struggled for over 2 days in Feb 2013 trying to make
                    //    it loop, but eventually had to code it up for a 'double
                    //    login' technique.  What a bunch of cunts BG are .. it simply
                    //    won't accept that I am clicking on different Utility.Accounts ..
                    //    Is it all to do with cookies and/or session login parameters?
                    //    Don't know ... might never know ...
                    //
                    //
                    //utilityviewmodel.resource_code = SmartParametersV2016.defaultResourceCode;  // Can always leave this in for single login or double login

                    HtmlCol2 = YetAnotherFuckingHoop.SelectNodesAsList(element1, "*");
                    foreach (HtmlAgilityPack.HtmlNode element2 in HtmlCol2)
                    {
                        if (string.IsNullOrEmpty(element2.Id))
                        {
                            classname = element2.GetAttributeValue("class", "");
                            inner_text = element2.InnerText.Trim();
                            Heading_Or_Small(utilityviewmodel,
                                                    classname,
                                                    inner_text);
                        }
                        if (!string.IsNullOrEmpty(element2.Id))// &&
                                                               //(utilityviewmodel.resource_code != SmartParametersV2016.defaultResourceCode))
                        {
                            AccountIDs(element2, utilityviewmodel);

                        }
                    }



                    // Clicking it is enough to set the latest value (nope)
                    HtmlCol3 = YetAnotherFuckingHoop.SelectNodesAsList(document.DocumentNode, "//a");
                    foreach (HtmlAgilityPack.HtmlNode element3 in HtmlCol3)
                    {
                        classname = element3.GetAttributeValue("class", "");
                        if (SmartNibbyV2016.Check_Classname(classname, "primary-link", true))
                        {
                            href = element3.GetAttributeValue(SmartParametersV2016.href, "");
                            if (!string.IsNullOrEmpty(href))
                            {
                                if (href.Contains("acctType=" + utilityviewmodel.acct_type_tag))
                                {
                                    utilityviewmodel.target_pathname = href;
                                    clicked = true;
                                    goto the_old_days;
                                }
                            }
                        }
                    }
                }
            }

        the_old_days:

            // All I can hear all the time is bashing and crashing around the house.
            // No wonder I never get any piece and quiet - no wonder I never
            // get anything done ..
            return clicked;
        }

        private static void AccountIDs(HtmlAgilityPack.HtmlNode element2,
                                        UtilityViewModel utilityviewmodel)
        {
            if (string.IsNullOrEmpty(utilityviewmodel.account_no))
            {
                // Stay away from "accountID3" <= fuck knows what that is ...
                if ((element2.Id.Contains("accountID1")) ||
                    (element2.Id.Contains("accountID2")))
                {
                    // Careful! This is also in class "small" ....
                    utilityviewmodel.account_no = element2.InnerText.Trim();
                }
                else
                {
                    if (element2.Id.Contains("accountNumberID0"))
                    {
                        string value = element2.GetAttributeValue("value", "");
                        utilityviewmodel.account_no = value;
                    }
                }
            }
            return;
        }

        private static void Heading_Or_Small(UtilityViewModel utilityviewmodel,
                                            string classname,
                                            string inner_text)
        {
            char resource_code;
            switch (classname)
            {
                case "heading":
                    if (inner_text.Length > 0)
                    {
                        if (inner_text == "Energy")
                        {
                            // Then we can assume this means Gas AND Electricity
                            // because if it was ONLY Gas then it would say 'Gas' and
                            // if it was ONLY Electricity then it would say 'Electricity'
                            resource_code = SmartParametersV2016.Electricity;
                            SmartParseV2016.Update_FuelList(SmartParametersV2016.Utility,
                                                                resource_code,
                                                                utilityviewmodel.brand_code,
                                                                utilityviewmodel.Hezbollah.supplier_typesList,
                                                                utilityviewmodel);
                            resource_code = SmartParametersV2016.Gas;
                            SmartParseV2016.Update_FuelList(SmartParametersV2016.Utility,
                                                                resource_code,
                                                                utilityviewmodel.brand_code,
                                                                utilityviewmodel.Hezbollah.supplier_typesList,
                                                                utilityviewmodel);

                        }
                        else
                        {
                            resource_code = Convert.ToChar(inner_text);
                            SmartParseV2016.Update_FuelList(SmartParametersV2016.Utility,
                                                                resource_code,
                                                                utilityviewmodel.brand_code,
                                                                utilityviewmodel.Hezbollah.supplier_typesList,
                                                                utilityviewmodel);

                        }
                        utilityviewmodel.acct_type_tag = inner_text;

                    }
                    break;
                case "small":
                    //inner_text = element2.InnerText.Trim();
                    if (!string.IsNullOrEmpty(inner_text))
                    {
                        //if (!inner_text.Contains("Customer reference"))
                        //{
                        //    // She THUMP THUMP THUMP THUMPS across the bathroom floor.
                        //    // Walking on her fucking heels all the fucking time
                        //    // So FUCKING NOISY she gives me a headache
                        //    // WALK ON YOUR FUCKING TOES for Christ's sake

                        //    // This logic finds the SAME address for both Gas AND Electricity !!!
                        //    // Something to fix when you have a few rainy days Ray!
                        //    switch (utilityviewmodel.resource_code)
                        //    {
                        //        case SmartParametersV2016.Gas:
                        //        case SmartParametersV2016.Electricity:
                        //            // Remove all 's which cock up the SELECT
                        //            if (!string.IsNullOrEmpty(account_address))
                        //            {
                        //                account_address = account_address + SmartParametersV2016.space;
                        //            }
                        //            account_address = account_address + inner_text.Replace(Environment.NewLine, "");
                        //            account_address = account_address.Replace("'", "").Trim();
                        //            account_address = account_address.TrimEnd(Convert.ToChar(SmartParametersV2016.period));
                        //            SmartParseV2016.Remove_Double_Spaces_V3(rf account_address);
                        //            break;
                        //        default:
                        //            break;
                        //    }
                        //}
                    }
                    break;
                default:
                    break;
            }
            return;
        }

        // I spent OVER FOUR DAYS trying to get this fucking thing to go view the
        // "View Account" button onto the Your Utility.Accounts page and try and scrape off
        // the Tariff and the Payment Plan, but all to no avail.  Fucking Webbrowser
        // categorically refused to load in all the bytes for the page; IE would
        // do it, and I could see 80000 bytes loaded in Fiddler, but would the
        // Webbrowser shit do it?  WOuld it fuck.  It only ever loaded in 65000
        // bytes and that - of course - didn't include the Tariff and the Payment
        // Plan.
        // So I gave up after a day amd resorted to scraping them off the bill ..
        // Fucking British Gas fucking cunts - all they had to do was list this
        // info and not put it in a fucking tab control.  Fucking numbskulls
        //
        // The problem is to do with the fact that the Tariff and Payment
        // are OVERLAYS (fuck knows why) and I think IE browser has a
        // capability to process these (Javascripts?) but the Webbrowser
        // doesn't (or at least I can't find a switch to make it)
        // In the HTML source look for 'tariff overlay'
        // British Gas Cunts ...

        // Fuck me!  It fucking WORKED!!!! See the re-direction to this 'secure'
        // routine from the initial fucking Login routine above!!!!!!!!!!!!
        // You ARE a fucking genius Ray!!  An absolute bravest of the brave
        // fucking genius.  Those visionless cunts you saw last Thursday have
        // ABSOLUTELY no idea what they are missing out on ....

        //wb.DocumentCompleted -= new WebBrowserDocumentCompletedEventHandler(BG_DocumentCompleted_Utility.Accountsummary1);
        //wb.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(BG_DocumentCompleted_Utility.Accountsummary2);

        // All you need to do is tell the FUCKING IE 7.0 browser component
        // which is fucking picked up automatically instead of IE 9.0
        // to accept these document types in order to get the fucking
        // 'Your Tariff' field returned.  Fucking, fucking, fucking Microsoft cunts

        private static async Task<bool> BG_Accountsummary(MainViewModel ourviewmodel,
                                                UtilityViewModel utilityviewmodel,
                                                HtmlAgilityPack.HtmlDocument document)
        {
            // This one should return the Tariff fields we need

            // IList<HtmlAgilityPack.HtmlNode> HtmlCol1,
            //                                HtmlCol2;
            //string classname = "",
            //        inner_text = "";
            //bool clicked = false;

            IList<HtmlAgilityPack.HtmlNode> HtmlCol1,
                       HtmlCol2;
            //HtmlCol3;
            utilityviewmodel.token = "";
            string account_address = "",
                        classname;
            bool clicked = false;


            HtmlCol1 = YetAnotherFuckingHoop.SelectNodesAsList(document.DocumentNode, "//div");
            foreach (HtmlAgilityPack.HtmlNode element1 in HtmlCol1)
            {
                classname = element1.GetAttributeValue("class", "");
                if (SmartNibbyV2016.Check_Classname(classname, "details-lft-space fleft address", true))
                {
                    HtmlCol2 = YetAnotherFuckingHoop.SelectNodesAsList(element1, "p");
                    foreach (HtmlAgilityPack.HtmlNode element2 in HtmlCol2)
                    {

                        classname = element2.GetAttributeValue("class", "");

                        // Other bits and pieces
                        //      utilityviewmodel.TARIFF_NAME = "";
                        //      utilityviewmodel.TARIFF_CODE = 0;
                        //      utilityviewmodel.statement_id = "";
                        if (!string.IsNullOrEmpty(element2.InnerText))
                        {
                            // For BG One Bill customers there are still - in fact - TWO cutomer
                            // references accountID1 and accountID2 !!!  The second one, though is
                            // hidden even though SmartSwitch is clever enough to find it!
                            // Lets proceed as if they really do just have 'one bill'


                            utilityviewmodel.token = element2.InnerText.Replace(Environment.NewLine, "").Trim();
                            utilityviewmodel.token = utilityviewmodel.token.Replace("&nbsp;", "");
                            utilityviewmodel.token = SmartParseV2016.Remove_Double_Spaces_V3(utilityviewmodel.token);
                            switch (classname)
                            {
                                case "small":
                                    break;
                                case "small fleft":
                                    // Remove possible trailing full stop
                                    account_address = account_address + SmartParametersV2016.space + utilityviewmodel.token.TrimEnd(Convert.ToChar(SmartParametersV2016.period));
                                    clicked = true;
                                    break;
                                case "":
                                    account_address = utilityviewmodel.token;
                                    break;
                                default:
                                    break;
                            }
                        }
                    }
                    if (clicked)
                    {
                        break;
                    }
                }
            }

            if (!string.IsNullOrEmpty(account_address))
            {
                if (SmartNibbyV2016.Derive_Postcode_New(account_address,
                                                    ourviewmodel.Blanche.workingPostcodesList,
                                                    utilityviewmodel))
                {
                    SmartUtilityV2022.Set_UDPRN_MPAN_MPRN(utilityviewmodel);

                    // If we don't have a UDPRN or the MPAN_MPRN is empty,
                    // then go and look it up.
                    if (string.IsNullOrEmpty(utilityviewmodel.mpan_mprn))
                    {
                        if (!await SmartUtilityV2022.Determine_UDPRNZ(ourviewmodel, utilityviewmodel, account_address))
                        {
                            return false;
                        }
                    }
                    utilityviewmodel.login_finished = true;
                    clicked = true;
                }
            }

            return clicked;
        }

        //private static bool BG_Payments(HtmlAgilityPack.HtmlDocument document,
        //                                UtilityViewModel utilityviewmodel)
        //{
        //    // Into the 'new'!!  More slog, eh, Ray??
        //     IList<HtmlAgilityPack.HtmlNode> HtmlCol1;
        //    string inner_text = "";
        //    bool clicked = false;

        //    HtmlCol1 = YetAnotherFuckingHoop.SelectNodesAsList(document.DocumentNode, "//a");
        //    foreach (HtmlAgilityPack.HtmlNode element1 in HtmlCol1)
        //    {
        //        if (string.IsNullOrEmpty(element1.Id) &&
        //            !string.IsNullOrEmpty(element1.InnerText))
        //        {
        //            inner_text = element1.InnerText.Trim();
        //            if (inner_text == "Payment history")
        //            {
        //                // Perform the 'click' on the Payment History (Fucking Cunts)
        //                utilityviewmodel.target_pathname = element1.GetAttributeValue(SmartParametersV2016.href, "");
        //                utilityviewmodel.next_routine = "VIEWPAYM";
        //                clicked = true;
        //                break;
        //            }
        //        }
        //    }
        //    if (!clicked)
        //    {
        //        utilityviewmodel.next_routine = "LOGOUT";
        //    }
        //    return true;
        //}

        private static bool BG_ViewPayments(MainViewModel ourviewmodel,
                                            UtilityViewModel utilityviewmodel,
                                            HtmlAgilityPack.HtmlDocument document)
        {
            // Fuck me!  It fucking WORKED!!!! See the re-direction to this 'secure'
            // routine from the initial fucking Login routine above!!!!!!!!!!!!
            // You ARE a fucking genius Ray!!  An absolute bravest of the brave
            // fucking genius.  Those visionless cunts you saw last Thursday have
            // ABSOLUTELY no idea what they are missing out on ....

            IList<HtmlAgilityPack.HtmlNode>
                        HtmlCol1,
                        HtmlCol2,
                        HtmlCol3,
                        HtmlCol4,
                        HtmlCol5;

            string classname,
                    href;
            short payment_item = 0,
                    payment_code = 0;
            int span_index;

            bool clicked = false;

            // Store 'un-billed'/unallocated transactions in
            // a temp list unless they are already there ..

            // This 'click' approach doesn't work!  We don't get a document delivered
            // so we can't intercept it; this 'tab' is changed IN PLACE so we have to
            // use 'POST' as the 'click' and 'GET' as the document delivery ...

            HtmlCol1 = YetAnotherFuckingHoop.SelectNodesAsList(document.DocumentNode, "//table");
            foreach (HtmlAgilityPack.HtmlNode element1 in HtmlCol1)
            {
                if (!string.IsNullOrEmpty(element1.Id))
                {
                    classname = element1.GetAttributeValue("class", "");
                    if (SmartNibbyV2016.Check_Classname(classname, "table-history fleft", true))
                    {
                        HtmlCol2 = YetAnotherFuckingHoop.SelectNodesAsList(element1, "tbody");
                        foreach (HtmlAgilityPack.HtmlNode element2 in HtmlCol2)
                        {
                            HtmlCol3 = YetAnotherFuckingHoop.SelectNodesAsList(element2, "tr");
                            foreach (HtmlAgilityPack.HtmlNode element3 in HtmlCol3)
                            {
                                span_index = 0;
                                utilityviewmodel.transaction_date = ""; // transaction_value = account_balance = "";
                                utilityviewmodel.description = "";
                                utilityviewmodel.payment_date = SmartParametersV2016.defaultDate;
                                utilityviewmodel.payment_amount = 0;
                                utilityviewmodel.payment_balance = 0;

                                HtmlCol4 = YetAnotherFuckingHoop.SelectNodesAsList(element3, "td");
                                foreach (HtmlAgilityPack.HtmlNode element4 in HtmlCol4)
                                {
                                    // At least this Agility shit finds this A link
                                    // which is something the fucking WebBrowser never did ...
                                    HtmlCol5 = YetAnotherFuckingHoop.SelectNodesAsList(element4, "a");
                                    foreach (HtmlAgilityPack.HtmlNode element5 in HtmlCol5)
                                    {
                                        href = element5.GetAttributeValue(SmartParametersV2016.href, "");
                                        Process_Href(utilityviewmodel,
                                                    href,
                                                    utilityviewmodel.transaction_date,
                                                    utilityviewmodel.bills_tempList);
                                    }

                                    if (element4.Name == "td")
                                    {
                                        if (!Process_TD(utilityviewmodel,
                                                        span_index,
                                                        element4))
                                        {
                                            goto quit;
                                        }
                                        span_index++;
                                    }
                                }
                                // a) Is this a payment and
                                // b) Can we put this in the Payments temp list?
                                if ((utilityviewmodel.payment_date != SmartParametersV2016.defaultDate) &&
                                    (utilityviewmodel.payment_amount != 0) &&
                                    (utilityviewmodel.description.Contains("Payment") ||
                                      utilityviewmodel.description.Contains("payment")))
                                {
                                    string PAYMENT_METHOD = utilityviewmodel.description.Trim();
                                    if (!string.IsNullOrEmpty(PAYMENT_METHOD))
                                    {
                                        if (!SmartSpikeUtilityV2017.Utility_Lookup_PaymentCode(ourviewmodel,
                                                                                        utilityviewmodel,
                                                                                        PAYMENT_METHOD))
                                        {
                                            goto quit;
                                        }
                                        PAYMENT_METHOD = utilityviewmodel.PAYMENT_METHOD;
                                        payment_item = (short)(payment_item + 1);
                                        if (!SmartParseV2016.Check_TempList(ourviewmodel,
                                                                            utilityviewmodel,
                                                                            utilityviewmodel.payment_date,
                                                                            payment_item,
                                                                            payment_code,
                                                                            utilityviewmodel.payment_amount,
                                                                            utilityviewmodel.payment_balance,
                                                                            utilityviewmodel.payments_tempList))
                                        {
                                            //SmartUtilityV2022.PaymentsList_Add(payments_tempList,
                                            //                                    SmartParametersV2016.Utility,
                                            //                                    utilityviewmodel.supplier_code,
                                            //                                    utilityviewmodel.brand_code,
                                            //                                    utilityviewmodel.ACCOUNT_NO,
                                            //                                    utilityviewmodel.CREATED,
                                            //                                    utilityviewmodel.STATEMENT_ID,
                                            //                                    utilityviewmodel.BILL_DATE,
                                            //                                    PAYMENT_DATE,
                                            //                                    payment_item,
                                            //                                    utilityviewmodel.PAYMENT_CODE,
                                            //                                    PAYMENT_AMOUNT,
                                            //                                    PAYMENT_BALANCE);
                                            // Sometiutilityviewmodel...we haven't had any bills and this Statement Id field
                                            // is just ... empty;
                                            foreach (SmartUtility.Payments payment_row in utilityviewmodel.payments_tempList)
                                            {
                                                SmartUtilityV2022.PaymentsList_Add(utilityviewmodel,
                                                                                SmartParametersV2016.Utility,
                                                                                payment_row.SUPPLIER_CODE,
                                                                                payment_row.BRAND_CODE,
                                                                                payment_row.ACCOUNT_NO,
                                                                                payment_row.ACCOUNT_CREATED,
                                                                                utilityviewmodel.STATEMENT_ID,       // Because HERE we don't have a Bill Number (yet)
                                                                                payment_row.BILL_DATE.ToString(),
                                                                                payment_row.PAYMENT_DATE,
                                                                                payment_row.PAYMENT_ITEM,
                                                                                payment_row.PAYMENT_CODE,
                                                                                payment_row.PAYMENT_AMOUNT,
                                                                                payment_row.PAYMENT_BALANCE);

                                            }
                                            // BESPOKE not BESTOKE you numbskull
                                            // And its TAUT not TAUNT you dumb bitch. The rope was TAUT
                                            // Noisy fucking, moaning idiotic SeeLon its CELINE you fucking thick bitch
                                        }
                                    }
                                }
                            }
                        }
                        clicked = true;
                        break;
                    }
                }
            }
        quit:
            return clicked;
        }

        // Fiddle, fart and fuck about ... ad nauseam ...
        private static void Process_Href(UtilityViewModel utilityviewmodel,
                                            string href,
                                            string transaction_date,
                                            List<SmartUtility.Bills> bills_tempList)
        {
            if (!string.IsNullOrEmpty(href))
            {
                SmartUtility.Bills bills_row = new SmartUtility.Bills()
                {
                    ACCOUNT_NO = utilityviewmodel.account_no,
                    STATEMENT_ID = href
                };

                // The temp Bill Id contains the entire path, the number after the =
                // is the bill id number
                string[] components = bills_row.STATEMENT_ID.Split(SmartParametersV2016.equalsSplit);
                string account_number = "&accountNumber";
                if (components.Length > 1)
                {
                    if (components[1].Contains(account_number))
                    {
                        components[1] = components[1].Replace(account_number, "").Trim();
                        string webpage_id = components[1];
                        if (!SmartParseV2016.Generic_Parse_Datetime(transaction_date, utilityviewmodel))
                        {
                            // Ensure the next if statement fails
                            utilityviewmodel.bill_date = SmartParametersV2016.defaultDate;
                        }
                        else
                        {
                            utilityviewmodel.bill_date = utilityviewmodel.genericTargetDate;
                        }
                        utilityviewmodel.statement_id = webpage_id;
                        if (SmartUtilityV2022.Check_Bill_Date(utilityviewmodel))
                        {
                            // Build a list of all the bills we find ..
                            // We decide which ones to process later on in VIEWBILLS
                            bills_tempList.Add(bills_row);
                        }
                    }
                }
            }
            return;
        }

        private static bool Process_TD(UtilityViewModel utilityviewmodel,
                                        int span_index,
                                        HtmlAgilityPack.HtmlNode element4)
        {
            string transaction_value,
                   account_balance;

            string inner_text = element4.InnerText.Trim();
            if (!string.IsNullOrEmpty(inner_text))
            {
                inner_text = inner_text.Replace("&nbsp;", "");
                inner_text = inner_text.Trim();
            }
            switch (span_index)
            {
                case 0:
                    // Date
                    if (!string.IsNullOrEmpty(inner_text))
                    {
                        utilityviewmodel.transaction_date = inner_text.Trim();
                        if (!SmartParseV2016.Generic_Parse_Datetime(utilityviewmodel.transaction_date, utilityviewmodel))
                        {
                            return false;
                        }
                        else
                        {
                            utilityviewmodel.payment_date = utilityviewmodel.genericTargetDate;
                        }
                    }
                    break;
                case 1:
                    // Type
                    if (!string.IsNullOrEmpty(inner_text))
                    {
                        utilityviewmodel.description = inner_text;
                    }
                    break;
                case 2:
                    // Debits
                    if (string.IsNullOrEmpty(inner_text))
                    {
                        transaction_value = "0";
                    }
                    else
                    {
                        transaction_value = inner_text.Replace(utilityviewmodel.bill_currency_symbol, "");
                        transaction_value = transaction_value.Replace(utilityviewmodel.bill_currency_separator.ToString(), "");
                        transaction_value = transaction_value.Replace(utilityviewmodel.bill_thousands_separator.ToString(), "");
                    }
                    if (!SmartParseV2016.Generic_Parse_Integer(transaction_value, utilityviewmodel))
                    {
                        return false;
                    }
                    utilityviewmodel.payment_amount = utilityviewmodel.genericTransactionValue;
                    break;
                case 3:
                    // Credits
                    if (string.IsNullOrEmpty(inner_text))
                    {
                        transaction_value = "0";
                    }
                    else
                    {
                        transaction_value = inner_text.Replace(utilityviewmodel.bill_currency_symbol, SmartParametersV2016.minus);
                        transaction_value = transaction_value.Replace(utilityviewmodel.bill_currency_separator.ToString(), "");
                        transaction_value = transaction_value.Replace(utilityviewmodel.bill_thousands_separator.ToString(), "");
                    }
                    if (!SmartParseV2016.Generic_Parse_Integer(transaction_value, utilityviewmodel))
                    {
                        return false;
                    }
                    utilityviewmodel.payment_amount = utilityviewmodel.genericTransactionValue;
                    break;
                case 4:
                    // Balance
                    if (string.IsNullOrEmpty(inner_text))
                    {
                        account_balance = "0";
                    }
                    else
                    {
                        account_balance = inner_text;
                    }
                    if (account_balance.Contains(" DR"))
                    {
                        account_balance = account_balance.Replace(" DR", "");
                    }
                    // Cannot do this with all the noise and interruptions ...
                    else
                    {
                        if (account_balance.Contains(" CR"))
                        {
                            account_balance = account_balance.Replace(" CR", "");
                        }
                        // If its not a Debit then it must be a Credit so mark it as -ve
                    }
                    account_balance = account_balance.Replace(utilityviewmodel.bill_currency_symbol, "");
                    account_balance = account_balance.Replace(utilityviewmodel.bill_currency_separator.ToString(), "");
                    account_balance = account_balance.Replace(utilityviewmodel.bill_thousands_separator.ToString(), "");
                    if (!SmartParseV2016.Generic_Parse_Integer(account_balance, utilityviewmodel))
                    {
                        return false;
                    }
                    utilityviewmodel.payment_balance = utilityviewmodel.genericTransactionValue;
                    break;
                default:
                    break;
            }
            return true;
        }

        private static bool BG_ViewBills(UtilityViewModel utilityviewmodel,
                                            HtmlAgilityPack.HtmlDocument htmlDocument,
                                            char resource_code)
        {
            // Fuck me!  It fucking WORKED!!!! See the re-direction to this 'secure'
            // routine from the initial fucking Login routine above!!!!!!!!!!!!
            // You ARE a fucking genius Ray!!  An absolute bravest of the brave
            // fucking genius.  Those visionless cunts you saw last Thursday have
            // ABSOLUTELY no idea what they are missing out on ....

            IList<HtmlAgilityPack.HtmlNode>
                        HtmlCol1,
                        HtmlCol2;

            string classname;

            // Store 'un-billed'/unallocated transactions in
            // a temp list unless they are already there ..

            HtmlCol1 = YetAnotherFuckingHoop.SelectNodesAsList(htmlDocument.DocumentNode, "//div");
            foreach (HtmlAgilityPack.HtmlNode element1 in HtmlCol1)
            {
                classname = element1.GetAttributeValue("class", "");
                if (!string.IsNullOrEmpty(classname))
                {
                    if (classname.Contains("downloadPdfLink"))
                    {
                        HtmlCol2 = YetAnotherFuckingHoop.SelectNodesAsList(element1, "p");
                        foreach (HtmlAgilityPack.HtmlNode element2 in HtmlCol2)
                        {
                            if (!string.IsNullOrEmpty(element2.InnerText))
                            {
                                utilityviewmodel.webpage_id = element2.InnerText;
                                switch (resource_code)
                                {
                                    case SmartParametersV2016.Gas:
                                        utilityviewmodel.webpage_id = utilityviewmodel.webpage_id.Replace("Gas statement", "");
                                        utilityviewmodel.webpage_id = utilityviewmodel.webpage_id.Replace("Gas bill", "");
                                        break;
                                    case SmartParametersV2016.Electricity:
                                        utilityviewmodel.webpage_id = utilityviewmodel.webpage_id.Replace("Electricity statement", "");
                                        utilityviewmodel.webpage_id = utilityviewmodel.webpage_id.Replace("Electricity bill", "");
                                        break;
                                    default:
                                        break;
                                }
                                utilityviewmodel.webpage_id = utilityviewmodel.webpage_id.Trim();
                                return true;
                            }
                        }
                    }
                }
            }
            utilityviewmodel.webpage_id = "";    // We didn't find any Bill statement info =:-O(
            return false;
        }

        private static bool BG_Midata(MainViewModel ourviewmodel,
                                        UtilityViewModel utilityviewmodel,
                                        HtmlAgilityPack.HtmlDocument document)
        {
            IList<HtmlAgilityPack.HtmlNode>
                            HtmlCol2,
                            HtmlCol3,
                            HtmlCol4;

            // Sometimes ... there is NO Midata i.e. no table - we just have to carry on ...
            HtmlCol2 = YetAnotherFuckingHoop.SelectNodesAsList(document.DocumentNode, "//table");
            foreach (HtmlAgilityPack.HtmlNode element2 in HtmlCol2)
            {
                if (element2.Id == utilityviewmodel.account_no)
                {
                    HtmlCol3 = YetAnotherFuckingHoop.SelectNodesAsList(element2, "tr");
                    foreach (HtmlAgilityPack.HtmlNode element3 in HtmlCol3)
                    {
                        int loop = 0;
                        utilityviewmodel.clicked = false;
                        utilityviewmodel.resource_code = SmartParametersV2016.defaultResourceCode;

                        HtmlCol4 = YetAnotherFuckingHoop.SelectNodesAsList(element3, "td");
                        foreach (HtmlAgilityPack.HtmlNode element4 in HtmlCol4)
                        {
                            if (!Midata_Process(ourviewmodel,
                                                    utilityviewmodel,
                                                    loop,
                                                    element4))
                            {
                                goto quit;
                            }
                            loop++;
                        }
                        if (utilityviewmodel.clicked)
                        {
                            break;
                        }
                    }
                }
                if (utilityviewmodel.clicked)
                {
                    break;
                }
            }


        quit:
            return utilityviewmodel.clicked;
        }

        internal static bool Midata_Process(MainViewModel ourviewmodel,
                                            UtilityViewModel utilityviewmodel,
                                            int loop,
                                            HtmlAgilityPack.HtmlNode element4)
        {
            utilityviewmodel.clicked = false;
            string inner_text = element4.InnerText.Trim();
            switch (loop)
            {
                case 0:
                    // Should be the resource
                    if (inner_text.Length >= 1)
                    {
                        utilityviewmodel.resource_code = Convert.ToChar(inner_text);
                    }
                    break;
                case 1:
                    // Should be the MPAN/MPRN
                    //if (utilityviewmodel.resource_code == utilityviewmodel.resource_code)
                    //{
                    //bool update_meters = false;
                    switch (utilityviewmodel.resource_code)
                    {
                        case SmartParametersV2016.Electricity:
                            if (string.IsNullOrEmpty(utilityviewmodel.sparks.MPAN) ||
                                (utilityviewmodel.sparks.MPAN == SmartParametersV2016.unknownMPAN))
                            {
                                utilityviewmodel.sparks.MPAN = inner_text;
                            }
                            break;
                        case SmartParametersV2016.Gas:
                            if (string.IsNullOrEmpty(utilityviewmodel.smell.MPRN) ||
                                (utilityviewmodel.smell.MPRN == SmartParametersV2016.unknownMPRN))
                            {
                                utilityviewmodel.smell.MPRN = inner_text;
                            }
                            break;
                        default:
                            break;
                    }
                    //}
                    break;
                case 2:
                    //if (utilityviewmodel.resource_code == utilityviewmodel.resource_code)
                    //{
                    utilityviewmodel.TARIFF_CODE = 0;
                    string resource_type = "";
                    switch (utilityviewmodel.resource_code)
                    {
                        case SmartParametersV2016.Electricity:
                            resource_type = utilityviewmodel.sparks.resource_type;
                            break;
                        case SmartParametersV2016.Gas:
                            resource_type = utilityviewmodel.smell.resource_type;
                            break;
                        default:
                            break;
                    }
                    if (SmartSpikeUtilityV2017.Utility_Lookup_TariffCode(utilityviewmodel,
                                                        utilityviewmodel.brand_code,
                                                        utilityviewmodel.resource_code,
                                                        resource_type))
                    {
                        //inner_text = utilityviewmodel.TARIFF_NAME;
                        // Should be the Product name (Tariff name)
                        switch (utilityviewmodel.resource_code)
                        {
                            case SmartParametersV2016.Electricity:
                                utilityviewmodel.sparks.tariff_code = utilityviewmodel.TARIFF_CODE;
                                break;
                            case SmartParametersV2016.Gas:
                                utilityviewmodel.smell.tariff_code = utilityviewmodel.TARIFF_CODE;
                                break;
                            default:
                                break;
                        }
                    }
                    //}
                    break;
                case 3:
                    //if (utilityviewmodel.resource_code == utilityviewmodel.resource_code)
                    //{
                    // Should be the Contract End Date - could be a dash ? '-'
                    switch (utilityviewmodel.resource_code)
                    {
                        case SmartParametersV2016.Electricity:
                            utilityviewmodel.sparks.contact_end_date = inner_text;
                            break;
                        case SmartParametersV2016.Gas:
                            utilityviewmodel.smell.contact_end_date = inner_text;
                            break;
                        default:
                            break;
                    }
                    //}
                    break;
                case 4:
                    //if (utilityviewmodel.resource_code == utilityviewmodel.resource_code)
                    //{
                    // Should be the Energy Used
                    inner_text = inner_text.Replace("kWh", "").Trim();
                    switch (utilityviewmodel.resource_code)
                    {
                        case SmartParametersV2016.Electricity:
                            utilityviewmodel.sparks.energy_used = inner_text;
                            break;
                        case SmartParametersV2016.Gas:
                            utilityviewmodel.smell.energy_used = inner_text;
                            break;
                        default:
                            break;
                    }
                    //}
                    break;
                case 5:
                    //if (utilityviewmodel.resource_code == utilityviewmodel.resource_code)
                    //{
                    // Should be the Forecasted Cost
                    inner_text = inner_text.Replace("&pound;", "");
                    switch (utilityviewmodel.resource_code)
                    {
                        case SmartParametersV2016.Electricity:
                            utilityviewmodel.sparks.personal_projection = inner_text;
                            break;
                        case SmartParametersV2016.Gas:
                            utilityviewmodel.smell.personal_projection = inner_text;
                            break;
                        default:
                            break;
                    }
                    //}
                    break;
                case 6:
                    //if (utilityviewmodel.resource_code == utilityviewmodel.resource_code)
                    //{
                    // Should be the TCR rate per kWh
                    inner_text = inner_text.Replace("*", "");
                    inner_text = inner_text.Replace("kWh", "");
                    inner_text = inner_text.Replace("per", "");
                    inner_text = inner_text.Replace("p", "");
                    if (inner_text == "N/A")
                    {
                        inner_text = "0.0";
                    }
                    switch (utilityviewmodel.resource_code)
                    {
                        case SmartParametersV2016.Electricity:
                            utilityviewmodel.sparks.TCR = inner_text;
                            break;
                        case SmartParametersV2016.Gas:
                            utilityviewmodel.smell.TCR = inner_text;
                            break;
                        default:
                            break;
                    }
                    //}
                    break;
                case 7:
                    //if (utilityviewmodel.resource_code == utilityviewmodel.resource_code)
                    //{
                    // Should be the Payment Method (Payment Plan)
                    int dash = inner_text.IndexOf("-");
                    if (dash >= 0)
                    {
                        inner_text = inner_text.Substring(0, dash).Trim();
                    }
                    string PAYMENT_TYPE = inner_text;
                    char payment_method;// = SmartParametersV2016.defaultChar;
                    if (!SmartSpikeUtilityV2017.Utility_Lookup_PaymentPlan(ourviewmodel, utilityviewmodel, PAYMENT_TYPE))
                    {
                        // utilityviewmodel.urgent message should be set here
                        return false;
                    }
                    else
                    {
                        payment_method = utilityviewmodel.PAYMENT_PLAN;
                    }
                    switch (utilityviewmodel.resource_code)
                    {
                        case SmartParametersV2016.Electricity:
                            utilityviewmodel.sparks.payment_method = payment_method;
                            break;
                        case SmartParametersV2016.Gas:
                            utilityviewmodel.smell.payment_method = payment_method;
                            break;
                        default:
                            break;
                    }
                    utilityviewmodel.clicked = true;
                    //}
                    break;
                default:
                    // Everything else (for now)
                    break;
            }
            return true;
        }

        private static bool BG_Personal(UtilityViewModel utilityviewmodel,
                                        HtmlAgilityPack.HtmlDocument document)
        {
            IList<HtmlAgilityPack.HtmlNode> HtmlCol1,
                        HtmlCol2,
                        HtmlCol3;
            string email_address = "Email address:",
                    current_home_number = "Current home number:";
            utilityviewmodel.token = "";
            string classname;

            HtmlCol1 = YetAnotherFuckingHoop.SelectNodesAsList(document.DocumentNode, "//div");
            foreach (HtmlAgilityPack.HtmlNode element1 in HtmlCol1)
            {
                classname = element1.GetAttributeValue("class", "");
                if (classname == "content-area")
                {
                    HtmlCol2 = YetAnotherFuckingHoop.SelectNodesAsList(element1, "p");
                    foreach (HtmlAgilityPack.HtmlNode element2 in HtmlCol2)
                    {
                        HtmlCol3 = YetAnotherFuckingHoop.SelectNodesAsList(element2, "strong");
                        foreach (HtmlAgilityPack.HtmlNode element3 in HtmlCol3)
                        {
                            utilityviewmodel.token = element3.InnerText.Trim();
                            // Take a chance the first strong is the name
                            utilityviewmodel.token = utilityviewmodel.token.Replace(SmartParametersV2016.newline.ToString(), "");
                            utilityviewmodel.token = SmartParseV2016.Remove_Double_Spaces_V3(utilityviewmodel.token);
                            utilityviewmodel.account_name = utilityviewmodel.token;
                            break;
                        }
                    }
                }
                if (classname == "text")
                {
                    HtmlCol2 = YetAnotherFuckingHoop.SelectNodesAsList(element1, "ul");
                    foreach (HtmlAgilityPack.HtmlNode element2 in HtmlCol2)
                    {
                        HtmlCol3 = YetAnotherFuckingHoop.SelectNodesAsList(element2, "li");
                        foreach (HtmlAgilityPack.HtmlNode element3 in HtmlCol3)
                        {
                            utilityviewmodel.token = element3.InnerText.Trim();
                            if (utilityviewmodel.token.Contains(email_address))
                            {
                                utilityviewmodel.account_email = utilityviewmodel.token.Replace(email_address, "").Trim();
                            }
                            else
                            {
                                if (utilityviewmodel.token.Contains(current_home_number))
                                {
                                    utilityviewmodel.account_phone_no = utilityviewmodel.token.Replace(current_home_number, "").Trim();
                                }
                            }
                        }
                    }
                }
            }
            if (!string.IsNullOrEmpty(utilityviewmodel.account_name) ||
                !string.IsNullOrEmpty(utilityviewmodel.account_email) ||
                !string.IsNullOrEmpty(utilityviewmodel.account_phone_no))
            {
                //SmartUtilityV2022.Utility_Check_Everything(utilityviewmodel, ourviewmodel, utilityviewmodel);
            }
            // Well ... we either find stuff or we don't no matter either way ....
            return true;
        }

        private static async Task<bool> BG_Read_Bills(
#if ANDROIDX
                                        AppCompatActivity meterActivity,
#endif
                                        MainViewModel ourviewmodel,
                                        UtilityViewModel utilityviewmodel,
                                        bool TextBox_Active,
                                        List<SmartUtility.Bills> bills_tempList)
        {
            // Loop round the ViewBills list
            HtmlAgilityPack.HtmlDocument htmlDocument = new HtmlAgilityPack.HtmlDocument();

            foreach (SmartUtility.Bills bills_temp_row in bills_tempList)
            {
                // The temp Bill Id contains the entire path, the number after the =
                // is the bill id number
                string bill_id = Determine_Bill_Number(bills_temp_row.STATEMENT_ID);
#if WINFORMS
                if (utilityviewmodel.console)
                {
                    await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel,
                                                (DateTime.Now + ourviewmodel.utcOffset).ToString(SmartParametersV2016.sqliteformat) + SmartParametersV2016.space + // Local time
                                                utilityviewmodel.supplier_code + SmartParametersV2016.space +
                                                utilityviewmodel.brand_code);
                }
#endif
#if WPF 
                if (utilityviewmodel.examine.scrape)
                {
                    if (!await SmartRoutinesV2018.CheckTrace(ourviewmodel, utilityviewmodel.utilityToken, utilityviewmodel.supplier_code, utilityviewmodel.brand_code, SmartParametersV2016.Utility.ToString() + " " + utilityviewmodel.next_routine))
                    {
                        return false;
                    }
                }
#endif
#if ANDROIDX
                if (utilityviewmodel.examine.scrape)
                {
                    await SmartRoutinesV2018.CheckTrace(ourviewmodel, utilityviewmodel.utilityToken, utilityviewmodel.supplier_code, utilityviewmodel.brand_code, SmartParametersV2016.Utility.ToString() + " " + utilityviewmodel.next_routine);
                }
#endif
                if (!SmartNibbyV2016.Create_Uri(ourviewmodel,
                                                utilityviewmodel.prfix_xxx,
                                                bills_temp_row.STATEMENT_ID))
                {
                    ourviewmodel.errorMessage = utilityviewmodel.current_routine + "|" + ourviewmodel.errorMessage;
                    return false;
                }
                List<string> headers = new List<string>();
                htmlDocument = await SmartBobV2017.HTTPCLIENT_GET_ASYNC(ourviewmodel,
                                                                   ourviewmodel.TargetUrl,
                                                                   utilityviewmodel.utilityToken,
                                                                    headers,
                                                                    "",   // Authenticity token
                                                                    utilityviewmodel.guid);
                if (!string.IsNullOrEmpty(ourviewmodel.errorMessage) ||
                    (htmlDocument.RemainderOffset == 0))
                {
                    ourviewmodel.errorMessage = utilityviewmodel.current_routine + SmartParametersV2016.space + ourviewmodel.errorMessage;
                    return false;
                }
                utilityviewmodel.webpage_id = "";
                // This just gets the Statement Date from the document
                if (BG_ViewBills(utilityviewmodel, htmlDocument, utilityviewmodel.resource_code))
                {
                    utilityviewmodel.webpage_id = utilityviewmodel.webpage_id.Replace("Energy bill", "").Trim();
                    DateTime bill_date = SmartParametersV2016.defaultDate;
                    if (!SmartParseV2016.Generic_Parse_Datetime(utilityviewmodel.webpage_id, utilityviewmodel))
                    {
#if WINFORMS
                        // Date doesn't convert - try the next
                        await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel,
                                                (DateTime.Now + ourviewmodel.utcOffset).ToString(SmartParametersV2016.sqliteformat) + // Local time
                                                SmartParametersV2016.space +
                                                ourviewmodel.errorMessage);
#endif
                        continue;
                    }
                    else
                    {
                        bill_date = utilityviewmodel.genericTargetDate;
                    }
                    utilityviewmodel.statement_id = utilityviewmodel.webpage_id;
                    await BG_Do_Actual_Read(
#if ANDROIDX
                                        meterActivity,
#endif
                                        ourviewmodel,
                                         utilityviewmodel,
                                         TextBox_Active,
                                        bill_date,
                                        bill_id);
                }
            }
            return true;
        }

        private static async Task<bool> BG_Do_Actual_Read(
#if ANDROIDX
                                        AppCompatActivity meterActivity,
#endif
                                        MainViewModel ourviewmodel,
                                         UtilityViewModel utilityviewmodel,
                                         bool TextBox_Active,
                                        DateTime bill_date,
                                        string bill_id)
        {
            utilityviewmodel.bill_date = bill_date;
            utilityviewmodel.bills_out = true;
            utilityviewmodel.bills_resource_out = true;
            string mpan_mprn = "";
            switch (utilityviewmodel.resource_code)
            {
                case SmartParametersV2016.Electricity:
                    mpan_mprn = utilityviewmodel.sparks.MPAN;
                    break;
                case SmartParametersV2016.Gas:
                    mpan_mprn = utilityviewmodel.smell.MPRN;
                    break;
                default:
                    break;
            }
            if (!SmartUtilityV2022.Bill_Already_Done(utilityviewmodel, mpan_mprn))
            {
                // We haven't already done this bill in this current Read Meter
                if (!SmartNibbyV2016.Create_Uri(ourviewmodel,
                                                utilityviewmodel.prfix_xxx,
                                                "apps/britishgas/components/downloadBill/GET.servlet" +
                                                "?accountNumber=" + utilityviewmodel.account_no +
                                                "&billDate=" + Uri.EscapeDataString(utilityviewmodel.statement_id) +
                                                "&billId=" + bill_id +
                                                "&acctType=" + utilityviewmodel.account_type +
                                                "&billType=statement"))
                {
                    ourviewmodel.errorMessage = utilityviewmodel.current_routine + "|" + ourviewmodel.errorMessage;
                    return false;
                }
                // Start with a clean sheet ...
                ourviewmodel.pdfMessage = "";
                // Get the fucking PDF
#if WINFORMS || WPF  || WINUI || SMARTMAUI
                iText.Kernel.Pdf.PdfReader pdfreader =
#endif
#if ANDROIDX
                iText.Kernel.Pdf.PdfReader pdfreader =
#endif
                await SmartBobV2017.HTTPCLIENT_GET_PDF_ASYNC(ourviewmodel,
                                                            utilityviewmodel.utilityToken,
                                                            ourviewmodel.TargetUrl,
                                                            utilityviewmodel.guid); // Possible pdf_error here
                if (!string.IsNullOrEmpty(ourviewmodel.errorMessage) ||
                    !string.IsNullOrEmpty(ourviewmodel.pdfMessage))
                {
                    return false;
                }
                if (!await SmartPDFV2019.Generic_Close_1(ourviewmodel,
                                                        utilityviewmodel,
#if ANDROIDX
                                                        meterActivity,
#endif
                                                        pdfreader,
                                                        TextBox_Active))
                {
                    if (!string.IsNullOrEmpty(ourviewmodel.errorMessage))
                    {
                        return false;
                    }
                }
                else
                {
                    if (!await BG_parse_bill(ourviewmodel,
                                                utilityviewmodel,
                                                pdfreader))
                    {
#if WINFORMS
                        await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, " Parse failed: " + utilityviewmodel.account_no + SmartParametersV2016.space +
                                                                                        utilityviewmodel.statement_id + SmartParametersV2016.space +
                                                                                        bill_id + SmartParametersV2016.space +
                                                                                        utilityviewmodel.account_type + SmartParametersV2016.space +
                                                                                        Environment.NewLine.ToString());
#endif
#if WPF 
                        if (utilityviewmodel.examine.scrape)
                        {
                            if (!await SmartRoutinesV2018.CheckTrace(ourviewmodel, utilityviewmodel.utilityToken, utilityviewmodel.supplier_code, utilityviewmodel.brand_code, SmartParametersV2016.Utility.ToString() + " " + ourviewmodel.errorMessage))
                            {
                                return false;
                            }
                        }
#endif
#if ANDROIDX
                        if (utilityviewmodel.examine.scrape)
                        {
                            await SmartRoutinesV2018.CheckTrace(ourviewmodel, utilityviewmodel.utilityToken, utilityviewmodel.supplier_code, utilityviewmodel.brand_code, SmartParametersV2016.Utility.ToString() + " " + ourviewmodel.errorMessage);
                        }
#endif
                    }
                    else
                    {
                        // Only add it in if we successfully created it
                        SmartScraperV2016.Do_The_Bills(utilityviewmodel);
                    }
                }

                // After parsing, decide what to do with the PDF
                await SmartPDFV2019.Generic_Close_2(ourviewmodel,
                                                        utilityviewmodel,
                                                        pdfreader,
#if ANDROIDX
                                                        meterActivity,
#endif
                                                        TextBox_Active);
                if (!string.IsNullOrEmpty(ourviewmodel.errorMessage))
                {
                    return false;
                }
                // Close the reader down (at last) here
                if (pdfreader != null)
                {
#if ANDROIDX
                    pdfreader.Close();
#endif
                }
            }
            // End of COMMON PART
            return true;
        }

        internal static async Task<bool> BG_parse_bill(MainViewModel ourviewmodel,
                                                     UtilityViewModel utilityviewmodel,
#if WINFORMS || WPF  || WINUI || SMARTMAUI
                                                    iText.Kernel.Pdf.PdfReader pdfreader)
#endif
#if ANDROIDX
                                                    iText.Kernel.Pdf.PdfReader pdfreader)
#endif
        {
            SmartParseV2016.Initialize_Bill_Parse(ourviewmodel, utilityviewmodel);
            bool status = true;
            string strText_simple = "";
#if WINFORMS || WPF
            strText_simple = SmartPDFV2019.TurnPdfToText(ourviewmodel, utilityviewmodel, pdfreader);
#endif
#if ANDROIDX
            strText_simple = SmartPDFV2019.TurnPdfToText(ourviewmodel, utilityviewmodel, pdfreader);
#endif
            if (!string.IsNullOrEmpty(ourviewmodel.errorMessage))
            {
                return false;
            }

            if (!SmartParseV2016.Check_Big_Four(ourviewmodel, utilityviewmodel, false))
            {
                string[] lines = strText_simple.Split(SmartParametersV2016.newline);

                // THis is THE LAST FUCKING TIME I am re-writing British Gas ...
                string[][] sections = 
                                        // Common for Versions 1 & 2
                                      {
                                        new string[] { "A", "0", "", "24 hour emergency", "0", "-1", ""},
                                        new string[] { "A", "1", "", "Statement date:", "-1", "-1", ""},
                                        new string[] { "A", "2", "", "Your customer number:", "-1", "-1", ""},
                                        new string[] { "B", "1", "", "Before this statement" + "|" +
                                                                        "Last period", "-1", "-1", "" },
                                        new string[] { "B", "2", "", "This statement" + "|" +
                                                                        "This period", "-1", "-1", "" },
                                        new string[] { "B", "4", "", "What next" + "|" +
                                                                        "Next steps", "-1", "-1", "" },
                                        new string[] { "C", "0", "", "What you paid - thank you" + "|" +
                                                                     "What you paid – thank you", "-1", "-1", "" },

                                        new string[] { "C", "1", "E", "Electricity you’ve used this period", "-1", "-1", "" },
                                        new string[] { "C", "2", "E", "Total cost of electricity used", "-1", "-1", "" },
                                        new string[] { "C", "3", "G", "Gas you’ve used this period", "-1", "-1", "" },
                                        new string[] { "C", "4", "G", "Total cost of gas used", "-1", "-1", "" },
                                        new string[] { "C", "5", "", "Your adjustment" + "|" +
                                                                        "Adjustments after VAT" + "|" +
                                                                        "Adjustments", "-1", "-1", SmartParametersV2016.sectionsExactMatch },  // <= Exact match!
                                        new string[] { "D", "0", "", "About your tariff" + "|" +
                                                                        "You’re on " + "|" +
                                                                        "You're on " + "|" +
                                                                        "You're on our ", "-1", "-1", "" },
                                        new string[] { "E", "0", "", "Your balance was in credit by" +"|" +
                                                                        "Your balance was in debit by" + "|" +
                                                                        "Your account balance before", "-1", "-1", "" },
                                        // Version 2
                                        new string[] { "H", "0", "", "Energy charges this period", "-1", "-1", "" },
                                        new string[] { "H", "1", "E", "Your electricity use in detail", "-1", "-1", "" },
                                        new string[] { "H", "2", "G", "Your gas use in detail", "-1", "-1", "" },
                                        new string[] { "L", "1", "E", "Total electricity used", "-1", "-1", "" },
                                        new string[] { "L", "2", "G", "Total gas used", "-1", "-1", "" },
                                   };
                // This is always needed anyway because we always do Page1 with this
                // Sometimes ... the Bill Date is never extracted from the PDF text!
                // But when (if) we ever find it - we substitute the Bill Number for it
                // IF we haven't found the Bill number ...
                // Very Important Routine!!!
                SmartParseV2016.Sort_Sections(false, utilityviewmodel, lines);
                if (!await Parse_Page0(ourviewmodel,
                                                utilityviewmodel,
                                                sections,
                                                lines))
                {
                    if (!SmartParseV2016.Check_Big_Four(ourviewmodel, utilityviewmodel, false))
                    {
                        status = false;
                    }
                }
                else
                {
                    //int line_count = 0;
                    if (!await Parse_Pages(ourviewmodel,
                                             utilityviewmodel,
                                             //line_count,
                                             sections,
                                             lines))
                    {
                        status = false;
                    }
                }
            }
            if (status)
            {
                // This is the only sensible place to do this because sometimes
                // they come BEFORE a total ... and sometime they come AFTER...
                SmartParseV2016.Update_Tariff_Details(ourviewmodel, utilityviewmodel);
            }
            return status;
        }

        private static async Task<bool> Parse_Page0(MainViewModel ourviewmodel,
                                UtilityViewModel utilityviewmodel,
                                string[][] sections,
                                string[] lines)
        {
            // Not perfect but not a bad start
            bool status = true;

            // Very Important Routine!!!
            //SmartParseV2016.Sort_Sections(false, rf sections, lines);

            // Pass 1 - look for the Big Three in Section 0
            if (!await Parse_SectionA0(ourviewmodel, utilityviewmodel, sections, "A", "0", lines))
            {
                ourviewmodel.errorMessage = "A0" + SmartParametersV2016.bar + ourviewmodel.errorMessage;
                return status;
            }
            // Look for the Statement Date
            if (!await Parse_SectionA1(ourviewmodel, utilityviewmodel, sections, "A", "1", lines))
            {
                ourviewmodel.errorMessage = "A1" + SmartParametersV2016.bar + ourviewmodel.errorMessage;
                return status;
            }
            // Look for the Customer Number
            if (!Parse_SectionA2(ourviewmodel, utilityviewmodel, sections, "A", "2", lines))
            {
                ourviewmodel.errorMessage = "A2" + SmartParametersV2016.bar + ourviewmodel.errorMessage;
                return status;
            }
            return status;
        }

        private static async Task<bool> Parse_Pages(MainViewModel ourviewmodel,
                                         UtilityViewModel utilityviewmodel,
                                         //int line_count,
                                         string[][] sections,
                                         string[] lines)
        {
            // Not perfect but not a bad start
            bool status = true;

            if (!Parse_SectionB1(ourviewmodel, utilityviewmodel, sections, "B", "1", lines))
            {
                ourviewmodel.errorMessage = "B1" + SmartParametersV2016.bar + ourviewmodel.errorMessage;
                return false;
            }
            if (!Parse_SectionB2(ourviewmodel, utilityviewmodel, sections, "B", "2", lines))
            {
                ourviewmodel.errorMessage = "B2" + SmartParametersV2016.bar + ourviewmodel.errorMessage;
                return false;
            }
            if (!Parse_SectionB4(utilityviewmodel, sections, "B", "4"))
            {
                ourviewmodel.errorMessage = "B4" + SmartParametersV2016.bar + ourviewmodel.errorMessage;
                return false;
            }
            if (!Parse_SectionC0(ourviewmodel, utilityviewmodel, sections, "C", "0", lines))
            {
                ourviewmodel.errorMessage = "C0" + SmartParametersV2016.bar + ourviewmodel.errorMessage;
                return false;
            }
            if (!Parse_SectionC1(ourviewmodel, utilityviewmodel, sections, "C", "1", lines))
            {
                ourviewmodel.errorMessage = "C1" + SmartParametersV2016.bar + ourviewmodel.errorMessage;
                return false;
            }
            if (!Parse_SectionC2(ourviewmodel, utilityviewmodel, sections, "C", "2", lines))
            {
                ourviewmodel.errorMessage = "C2" + SmartParametersV2016.bar + ourviewmodel.errorMessage;
                return false;
            }
            if (!Parse_SectionC3(ourviewmodel, utilityviewmodel, sections, "C", "3", lines))
            {
                ourviewmodel.errorMessage = "C3" + SmartParametersV2016.bar + ourviewmodel.errorMessage;
                return false;
            }
            if (!Parse_SectionC4(ourviewmodel, utilityviewmodel, sections, "C", "4", lines))
            {
                ourviewmodel.errorMessage = "C4" + SmartParametersV2016.bar + ourviewmodel.errorMessage;
                return false;
            }
            if (!Parse_SectionC5(ourviewmodel, utilityviewmodel, sections, "C", "5", lines))     // Adjustments
            {
                ourviewmodel.errorMessage = "C5" + SmartParametersV2016.bar + ourviewmodel.errorMessage;
                return false;
            }
            // Tariff
            if (!await Parse_SectionD0(ourviewmodel, utilityviewmodel, sections, "D", "0", lines))
            {
                ourviewmodel.errorMessage = "D0" + SmartParametersV2016.bar + ourviewmodel.errorMessage;
                return false;
            }
            // Payments
            if (!Parse_SectionE0(ourviewmodel, utilityviewmodel, sections, "E", "0", lines))
            {
                ourviewmodel.errorMessage = "E0" + SmartParametersV2016.bar + ourviewmodel.errorMessage;
                return false;
            }
            // Schweppes <= Schwartz !!!
            if (!Parse_SectionH1(ourviewmodel, utilityviewmodel, sections, "H", "1", lines))
            {
                ourviewmodel.errorMessage = "H1" + SmartParametersV2016.bar + ourviewmodel.errorMessage;
                return false;
            }
            // Gas Readings
            if (!Parse_SectionH2(ourviewmodel, utilityviewmodel, sections, "H", "2", lines)) // G Readings
            {
                ourviewmodel.errorMessage = "H2" + SmartParametersV2016.bar + ourviewmodel.errorMessage;
                return false;
                // Fucking silly cow cannot add 3 to 2014!!
            }
            if (!Parse_SectionL1(ourviewmodel, utilityviewmodel, sections, "L", "1", lines)) // Discounts
            {
                ourviewmodel.errorMessage = "L1" + SmartParametersV2016.bar + ourviewmodel.errorMessage;
                return false;
                // Fucking silly cow cannot add 3 to 2014!!
            }
            if (!Parse_SectionL2(ourviewmodel, utilityviewmodel, sections, "L", "2", lines)) // Discounts
            {
                ourviewmodel.errorMessage = "L2" + SmartParametersV2016.bar + ourviewmodel.errorMessage;
                return false;
                // Fucking silly cow cannot add 3 to 2014!!
            }
            return status;
        }

        private static async Task<bool> Parse_SectionA0(MainViewModel ourviewmodel,
                                            UtilityViewModel utilityviewmodel,
                                            string[][] sections,
                                            string section,
                                            string sub_section,
                                            string[] lines)
        {
            // Not perfect but not a bad start
            utilityviewmodel.currentIndex = 0;
            if (!SmartParseV2016.Decode_Section(section, sub_section, sections, utilityviewmodel.resource_code.ToString(), utilityviewmodel))
            {
                return true;
            }

            // Pass 0 - Clear out some shit
            utilityviewmodel.token = "";

            bool postcode_found = false;

            char PAYMENT_PLAN = SmartParametersV2016.defaultChar;
            bool RESOURCE_BALANCES = false; // Default
            int TARIFF_CODE = 0,
                    NEW_CHARGES = 0,
                    RESOURCE_DISCOUNTS = 0,
                    RESOURCE_VAT_AMOUNT = 0;
            string PREVIOUS_BALANCE = "0",
                    PAYMENTS_RECEIVED = "0";
            string ACCOUNT_CHARGES_CREDITS = "0",
                    ACCOUNT_CHARGES_CREDITS_VAT_AMOUNT = "0",
                    SUPPLY_CHARGES_CREDITS = "0",
                    SUPPLY_CHARGES_CREDITS_VAT_AMOUNT = "0",
                    BILL_VAT_AMOUNT = "0",
                    OUTSTANDING_BALANCE = "0",
                    TOTAL_NOW_DUE = "0",
                    MONTHLY_PAYMENT = "0",
                    DIRECT_DEBIT_DATE = SmartParametersV2016.defaultDates,
                    PAYMENT_DUE_DATE = SmartParametersV2016.defaultDates,
                    PAYMENT_TYPE = "",
                    FIRST_YEAR_DISCOUNT = "0",
                    LOYALTY_BONUS = "0",
                    DISCOUNT_CREDIT_DATE = SmartParametersV2016.defaultDates,
                    REWARDS = "n/a";
            short BILL_VAT_CODE = SmartParametersV2016.zeroRateVatCode,
                    RESOURCE_VAT_CODE = SmartParametersV2016.zeroRateVatCode;
            DateTime temp_date;// = SmartParametersV2016.defaultDate;
            string account_address;// = "";

            int rayStart = Convert.ToInt32(sections[utilityviewmodel.currentIndex][4]);
            int rayEnd = Convert.ToInt32(sections[utilityviewmodel.currentIndex][5]);
            // Pass 1 get the Big Three (or four)
            for (int line_count = rayStart; line_count <= rayEnd; line_count++)
            {
                utilityviewmodel.token = lines[line_count].Trim();
                if (!postcode_found)
                {
                    string account_postcode = lines[line_count];
                    if (SmartNibbyV2016.Derive_Postcode_New(account_postcode,
                                                    ourviewmodel.Blanche.workingPostcodesList,
                                                    utilityviewmodel))
                    {
                        postcode_found = true;

                        account_address = account_postcode;
                        int my_line_count = line_count - 1;
                        while (my_line_count >= 0)
                        {
                            if (lines[my_line_count].Length <= 1)
                            {
                                // Remove possible name from front
                                account_address = account_address.Replace(lines[my_line_count + 1] + ",", "");
                                break;
                            }
                            account_address = lines[my_line_count] + "," + account_address;
                            my_line_count--;
                        }

                        SmartUtilityV2022.Set_UDPRN_MPAN_MPRN(utilityviewmodel);

                        // If we don't have a UDPRN or the MPAN_MPRN is empty,
                        // then go and look it up.
                        if (string.IsNullOrEmpty(utilityviewmodel.mpan_mprn))
                        {
                            if (!await SmartUtilityV2022.Determine_UDPRNZ(ourviewmodel, utilityviewmodel, account_address))
                            {
                                return false;
                            }
                        }
                    }
                }

                if (Accounts_At_Last(ourviewmodel,
                                    utilityviewmodel,
                                    line_count,
                                    lines))
                {
                    // So we have an Account No
                    if (postcode_found)
                    {
                        //SmartUtilityV2022.Utility_Check_Everything(utilityviewmodel, ourviewmodel, utilityviewmodel);
                    }
                    goto the_old_daysA0;
                }
                else
                {
                    if (!string.IsNullOrEmpty(ourviewmodel.errorMessage))
                    {
                        return false;
                    }
                }

                if (Bill_Dates(ourviewmodel, utilityviewmodel, line_count, lines))
                {
                    goto the_old_daysA0;
                }
                else
                {
                    if (!string.IsNullOrEmpty(ourviewmodel.errorMessage))
                    {
                        return false;
                    }
                }

                if (Bill_Periods(ourviewmodel, utilityviewmodel, line_count, lines))
                {
                    goto the_old_daysA0;
                }
                else
                {
                    if (!string.IsNullOrEmpty(ourviewmodel.errorMessage))
                    {
                        return false;
                    }
                }
            the_old_daysA0:
                continue;
            }
            SmartUtilityV2022.Bills_Row_Update(utilityviewmodel.Hezbollah.bills_row,
                                                SmartParametersV2016.Utility,
                                                utilityviewmodel.supplier_code,
                                                utilityviewmodel.brand_code,
                                                utilityviewmodel.ACCOUNT_NO,
                                                utilityviewmodel.CREATED,
                                                utilityviewmodel.STATEMENT_ID,
                                                utilityviewmodel.BILL_DATE,
                                                utilityviewmodel.BILL_PERIOD_START,
                                                utilityviewmodel.BILL_PERIOD_END,
                                                PAYMENT_PLAN,
                                                RESOURCE_BALANCES,
                                                PREVIOUS_BALANCE,
                                                PAYMENTS_RECEIVED,
                                                ACCOUNT_CHARGES_CREDITS,
                                                ACCOUNT_CHARGES_CREDITS_VAT_AMOUNT,
                                                SUPPLY_CHARGES_CREDITS,
                                                SUPPLY_CHARGES_CREDITS_VAT_AMOUNT,
                                                BILL_VAT_AMOUNT,
                                                BILL_VAT_CODE,
                                                OUTSTANDING_BALANCE,
                                                TOTAL_NOW_DUE,
                                                MONTHLY_PAYMENT,
                                                PAYMENT_DUE_DATE,
                                                PAYMENT_TYPE,
                                                DIRECT_DEBIT_DATE,
                                                LOYALTY_BONUS,
                                                DISCOUNT_CREDIT_DATE,
                                                FIRST_YEAR_DISCOUNT,
                                                REWARDS);
            if (!SmartParseV2016.Generic_Parse_Datetime(PAYMENT_DUE_DATE, utilityviewmodel))
            {
                return false;
            }
            else
            {
                temp_date = utilityviewmodel.genericTargetDate;
            }
            // Best we can do ..
            if (temp_date != SmartParametersV2016.defaultDate)
            {
                temp_date = temp_date.AddMonths(1);
                SmartParseV2016.Update_Bill(ourviewmodel, utilityviewmodel, "DIRECT_DEBIT_DATE", temp_date.ToString(SmartParametersV2016.defaultCulture));
            }
            string RESOURCE_ACCOUNT_NO = utilityviewmodel.ACCOUNT_NO;
            //string resource_type;
            string MPAN_MPRN = "";
            switch (utilityviewmodel.resource_code)
            {
                case SmartParametersV2016.Electricity:
                    //resource_type = utilityviewmodel.sparks.resource_type;
                    MPAN_MPRN = utilityviewmodel.sparks.MPAN;
                    break;
                case SmartParametersV2016.Gas:
                    //resource_type = utilityviewmodel.smell.resource_type;
                    MPAN_MPRN = utilityviewmodel.smell.MPRN;
                    break;
                default:
                    break;
            }

            SmartUtilityV2022.Bills_Resource_Row_Update(utilityviewmodel.Hezbollah.bills_resource_row,
                                                    utilityviewmodel.cubeface_code,
                                                    utilityviewmodel.supplier_code,
                                                    utilityviewmodel.brand_code,
                                                    utilityviewmodel.ACCOUNT_NO,
                                                    utilityviewmodel.CREATED,
                                                    utilityviewmodel.STATEMENT_ID,
                                                    utilityviewmodel.BILL_DATE,
                                                    MPAN_MPRN,
                                                    RESOURCE_ACCOUNT_NO,
                                                    TARIFF_CODE,        // WE don't check for TARIFF_CODE = 0 here ... Well we fucking should
                                                    NEW_CHARGES,
                                                    RESOURCE_DISCOUNTS,
                                                    RESOURCE_VAT_AMOUNT,
                                                    RESOURCE_VAT_CODE);
            return true;
        }

        private static bool Bill_Periods(MainViewModel ourviewmodel,
                                            UtilityViewModel utilityviewmodel,
                                            int line_count,
                                            string[] lines)
        {
            string bill_period = "Bill period:",
                    statement_period = "Statement period:";

            if (SmartParseV2016.Token_Identify(utilityviewmodel, statement_period, false) ||
                SmartParseV2016.Token_Identify(utilityviewmodel, bill_period, false))
            {
                if ((utilityviewmodel.BILL_PERIOD_START == SmartParametersV2016.defaultDates) ||
                     (utilityviewmodel.BILL_PERIOD_END == SmartParametersV2016.defaultDates))
                {
                    //DateTime temp_date = SmartParametersV2016.defaultDate;

                    SmartParseV2016.Find_Bill_Periods(utilityviewmodel,
                                                        line_count,
                                                        lines,
                                                        //utilityviewmodel.BILL_PERIOD_START,
                                                        //utilityviewmodel.BILL_PERIOD_END,
                                                        SmartParametersV2016.defaultDates);
                    if (!SmartParseV2016.Generic_Parse_Datetime(utilityviewmodel.BILL_PERIOD_START, utilityviewmodel))
                    {
                        return false;
                    }
                    //else
                    //{
                    //    temp_date = utilityviewmodel.genericTargetDate;
                    //}
                    if (!SmartParseV2016.Generic_Parse_Datetime(utilityviewmodel.BILL_PERIOD_END, utilityviewmodel))
                    {
                        return false;
                    }
                    //else
                    //{
                    //    temp_date = utilityviewmodel.genericTargetDate;
                    //}
                    SmartParseV2016.Update_Bill(ourviewmodel, utilityviewmodel, "BILL_PERIOD_START", utilityviewmodel.BILL_PERIOD_START);
                    SmartParseV2016.Update_Bill(ourviewmodel, utilityviewmodel, "BILL_PERIOD_END", utilityviewmodel.BILL_PERIOD_END);

                    string DISCOUNT_CREDIT_DATE = utilityviewmodel.BILL_PERIOD_END;
                    SmartParseV2016.Update_Bill(ourviewmodel, utilityviewmodel, "DISCOUNT_CREDIT_DATE", DISCOUNT_CREDIT_DATE);
                    // Matched the token and everything ok
                    return true;
                }
            }
            // Didn't match the token
            return false;
        }

        private static bool Bill_Dates(MainViewModel ourviewmodel,
                                        UtilityViewModel utilityviewmodel,
                                        int line_count,
                                        string[] lines)
        {
            string bill_date = "Bill date:",
                    statement_date_colon = "Statement date:";

            if ((SmartParseV2016.Token_Identify(utilityviewmodel, bill_date, true) ||
                        (utilityviewmodel.token.Contains(bill_date))) ||
                    (SmartParseV2016.Token_Identify(utilityviewmodel, statement_date_colon, true) ||
                        (utilityviewmodel.token.Contains(statement_date_colon))))
            {
                //DateTime temp_date = SmartParametersV2016.defaultDate;

                if (utilityviewmodel.BILL_DATE == SmartParametersV2016.defaultDates)
                {
                    utilityviewmodel.BILL_DATE = SmartParseV2016.Find_Bill_Date(utilityviewmodel, line_count, lines, SmartParametersV2016.defaultDates, utilityviewmodel.token);
                    if (!SmartParseV2016.Generic_Parse_Datetime(utilityviewmodel.BILL_DATE, utilityviewmodel))
                    {
                        return false;
                    }
                    //else
                    //{
                    //    temp_date = utilityviewmodel.genericTargetDate;
                    //}
                    SmartParseV2016.Update_Bill(ourviewmodel, utilityviewmodel, "BILL_DATE", utilityviewmodel.BILL_DATE);
                    return true;
                }
            }
            return false;
        }

        private static async Task<bool> Parse_SectionA1(MainViewModel ourviewmodel,
                                            UtilityViewModel utilityviewmodel,
                                            string[][] sections,
                                            string section,
                                            string sub_section,
                                            string[] lines)
        {
            // Not perfect but not a bad start
            utilityviewmodel.currentIndex = 0;
            if (!SmartParseV2016.Decode_Section(section, sub_section, sections, utilityviewmodel.resource_code.ToString(), utilityviewmodel))
            {
                return true;
            }

            // Pass 0 - Clear out some shit
            utilityviewmodel.token = "";

            bool postcode_found = false;

            // Pass 1 get the Big Three (or four)
            for (int line_count = Convert.ToInt32(sections[utilityviewmodel.currentIndex][4]); line_count <= Convert.ToInt32(sections[utilityviewmodel.currentIndex][5]); line_count++)
            {
                utilityviewmodel.token = lines[line_count].Trim();

                if (!postcode_found &&
                    string.IsNullOrEmpty(utilityviewmodel.postcode))
                {
                    string account_postcode = lines[line_count];
                    if (SmartNibbyV2016.Derive_Postcode_New(account_postcode,
                                                    ourviewmodel.Blanche.workingPostcodesList,
                                                    utilityviewmodel))
                    {
                        postcode_found = true;
                        string account_address = account_postcode;
                        int my_line_count = line_count - 1;
                        while (my_line_count >= 0)
                        {
                            if (lines[my_line_count].Length <= 1 ||
                                lines[my_line_count].Contains("See step 4"))
                            {
                                break;
                            }
                            account_address = lines[my_line_count] + "," + account_address;
                            my_line_count--;
                        }

                        SmartUtilityV2022.Set_UDPRN_MPAN_MPRN(utilityviewmodel);

                        // If we don't have a UDPRN or the MPAN_MPRN is empty,
                        // then go and look it up.
                        if (string.IsNullOrEmpty(utilityviewmodel.mpan_mprn))
                        {
                            if (!await SmartUtilityV2022.Determine_UDPRNZ(ourviewmodel, utilityviewmodel, account_address))
                            {
                                return false;
                            }
                        }
                    }
                }

                if (Accounts_At_Last(ourviewmodel,
                                    utilityviewmodel,
                                    line_count,
                                    lines))
                {
                    goto the_old_daysA1;
                }
                else
                {
                    if (!string.IsNullOrEmpty(ourviewmodel.errorMessage))
                    {
                        return false;
                    }
                }

                if (Bill_Dates(ourviewmodel, utilityviewmodel, line_count, lines))
                {
                    goto the_old_daysA1;
                }
                else
                {
                    if (!string.IsNullOrEmpty(ourviewmodel.errorMessage))
                    {
                        return false;
                    }
                }

                if (Bill_Periods(ourviewmodel, utilityviewmodel, line_count, lines))
                {
                    goto the_old_daysA1;
                }
                else
                {
                    if (!string.IsNullOrEmpty(ourviewmodel.errorMessage))
                    {
                        return false;
                    }
                }

            the_old_daysA1:
                continue;
            }
            return true;
        }

        private static bool Accounts_At_Last(MainViewModel ourviewmodel,
                                            UtilityViewModel utilityviewmodel,
                                            int line_count,
                                            string[] lines)
        {
            string customer_reference_number = "Customer reference number",
                    your_customer_number = "Your customer number";

            if (SmartParseV2016.Token_Identify(utilityviewmodel, customer_reference_number, false) ||
                SmartParseV2016.Token_Identify(utilityviewmodel, your_customer_number, false))
            {
                if (string.IsNullOrEmpty(utilityviewmodel.ACCOUNT_NO))
                {
                    string temp_account = SmartParseV2016.Find_Account(utilityviewmodel, line_count, lines);
                    if (!string.IsNullOrEmpty(temp_account))
                    {
                        utilityviewmodel.ACCOUNT_NO = temp_account;
                        SmartParseV2016.Update_Bill(ourviewmodel, utilityviewmodel, "ACCOUNT_NO", temp_account);
                        SmartParseV2016.Update_Bill(ourviewmodel, utilityviewmodel, "RESOURCE_ACCOUNT_NO", temp_account);
                        // This is for when we just Decode bills and don't Scrape the website
                        if (string.IsNullOrEmpty(utilityviewmodel.account_no))
                        {
                            utilityviewmodel.account_no = utilityviewmodel.ACCOUNT_NO;
                        }
                        return true;
                    }
                }
            }
            return false;
        }

        private static bool Parse_SectionA2(MainViewModel ourviewmodel,
                                            UtilityViewModel utilityviewmodel,
                                            string[][] sections,
                                            string section,
                                            string sub_section,
                                            string[] lines)
        {
            // Not perfect but not a bad start

            utilityviewmodel.currentIndex = 0;
            if (!SmartParseV2016.Decode_Section(section, sub_section, sections, utilityviewmodel.resource_code.ToString(), utilityviewmodel))
            {
                return true;
            }

            // Clear out some shit
            utilityviewmodel.token = "";
            bool postcode_found = false;

            // Pass 1 get the Big Three (or four)
            for (int line_count = Convert.ToInt32(sections[utilityviewmodel.currentIndex][4]); line_count <= Convert.ToInt32(sections[utilityviewmodel.currentIndex][5]); line_count++)
            {
                utilityviewmodel.token = lines[line_count].Trim();

                if (!postcode_found &&
                    string.IsNullOrEmpty(utilityviewmodel.postcode))
                {
                    string account_postcode = lines[line_count];
                    if (SmartNibbyV2016.Derive_Postcode_New(account_postcode,
                                                    ourviewmodel.Blanche.workingPostcodesList,
                                                    utilityviewmodel))
                    {
                        postcode_found = true;
                    }
                }
                if (Accounts_At_Last(ourviewmodel,
                                    utilityviewmodel,
                                    line_count,
                                    lines))
                {
                    goto the_old_daysA2;
                }
                else
                {
                    if (!string.IsNullOrEmpty(ourviewmodel.errorMessage))
                    {
                        return false;
                    }
                }

            the_old_daysA2:
                continue;
            }
            return true;
        }

        private static bool Parse_SectionB1(MainViewModel ourviewmodel,
                                            UtilityViewModel utilityviewmodel,
                                            string[][] sections,
                                            string section,
                                            string sub_section,
                                            string[] lines)
        {
            utilityviewmodel.currentIndex = 0;
            if (!SmartParseV2016.Decode_Section(section, sub_section, sections, utilityviewmodel.resource_code.ToString(), utilityviewmodel))
            {
                return true;
            }

            utilityviewmodel.token = "";

            for (int line_count = Convert.ToInt32(sections[utilityviewmodel.currentIndex][4]); line_count <= Convert.ToInt32(sections[utilityviewmodel.currentIndex][5]); line_count++)
            {
                utilityviewmodel.token = lines[line_count].Trim();

                if (Previous_Balance(ourviewmodel, utilityviewmodel, line_count, lines, sections, utilityviewmodel.currentIndex))
                {
                    goto the_old_daysB1;
                }
                else
                {
                    // Didn't find the token
                    if (!string.IsNullOrEmpty(ourviewmodel.errorMessage))
                    {
                        // But found a problem
                        return false;
                    }
                }
            the_old_daysB1:
                continue;
            }
            return true;
        }

        private static bool Decode_VAT(MainViewModel ourviewmodel,
                                       UtilityViewModel utilityviewmodel)
        {
            string VAT_at = "VAT at";

            if (SmartParseV2016.Token_Identify(utilityviewmodel, VAT_at, false))
            {
                //int TEMP;

                short VAT_CODE = SmartParametersV2016.zeroRateVatCode;
                if (!SmartParseV2016.Determine_Vat_Code(ourviewmodel,
                                                        utilityviewmodel,
                                                        utilityviewmodel.token,
                                                        VAT_at))
                {
                    return false;
                }
                SmartParseV2016.Update_Bill(ourviewmodel, utilityviewmodel, "RESOURCE_VAT_CODE", VAT_CODE.ToString());

                utilityviewmodel.token = utilityviewmodel.token.Replace(VAT_at, "").Trim();

                string VAT_AMOUNT = "";
                string[] components = utilityviewmodel.token.Split(SmartParametersV2016.spaceSplit);
                int component_count = 0;
                while (component_count < components.Length)
                {
                    switch (component_count)
                    {
                        case 0:
                            break;
                        case 1:
                            VAT_AMOUNT = components[component_count].Trim();
                            break;
                        default:
                            break;
                    }
                    component_count++;
                }
                if (!SmartParseV2016.Currency_Strip(utilityviewmodel.bill_currency_symbol, utilityviewmodel.bill_currency_separator, utilityviewmodel.bill_thousands_separator, VAT_AMOUNT, utilityviewmodel))
                {
                    return false;
                }
                else
                {
                    VAT_AMOUNT = utilityviewmodel.value;
                }
                if (!SmartParseV2016.Generic_Parse_Integer(VAT_AMOUNT, utilityviewmodel))
                {
                    return false;
                }
                //else
                //{
                //    TEMP = utilityviewmodel.genericTransactionValue;
                //}
                SmartParseV2016.Update_Bill(ourviewmodel, utilityviewmodel, "RESOURCE_VAT_AMOUNT", VAT_AMOUNT);
                return true;
            }
            return false;
        }

        private static bool Parse_SectionB2(MainViewModel ourviewmodel,
                                            UtilityViewModel utilityviewmodel,
                                            string[][] sections,
                                            string section,
                                            string sub_section,
                                            string[] lines)
        {
            utilityviewmodel.currentIndex = 0;
            if (!SmartParseV2016.Decode_Section(section, sub_section, sections, utilityviewmodel.resource_code.ToString(), utilityviewmodel))
            {
                return true;
            }

            utilityviewmodel.token = "";
            string what_you_paid = "What you paid";
            utilityviewmodel.discount_item = 0;
            //int temp = 0;

            for (int line_count = Convert.ToInt32(sections[utilityviewmodel.currentIndex][4]); line_count <= Convert.ToInt32(sections[utilityviewmodel.currentIndex][5]); line_count++)
            {
                utilityviewmodel.token = lines[line_count].Trim();

                if (SmartParseV2016.Token_Identify(utilityviewmodel, what_you_paid, true))
                {
                    // She is such a FUCKING NOISY BITCH ON THE TELEPHONE - SHOUTING DOWN IT ALL THE TIME...
                    // Now wonder I can never think when she's around - so FUKCING noisy
                    utilityviewmodel.token = SmartParseV2016.Remove_Double_Spaces_V3(utilityviewmodel.token);
                    if (string.IsNullOrEmpty(utilityviewmodel.token))
                    {
                        // Loop round until we find a currency symbol
                        int outer_index = line_count + 1;
                        while (outer_index <= Convert.ToInt32(sections[utilityviewmodel.currentIndex][5]))
                        {
                            utilityviewmodel.token = lines[outer_index].Trim();
                            if (utilityviewmodel.token.Contains(utilityviewmodel.bill_currency_symbol))
                            {
                                line_count = outer_index;
                                break;
                            }
                            outer_index++;
                        }
                    }
                    string PAYMENTS_RECEIVED;// = "0";

                    if (!SmartParseV2016.Currency_Strip(utilityviewmodel.bill_currency_symbol, utilityviewmodel.bill_currency_separator, utilityviewmodel.bill_thousands_separator, utilityviewmodel.token, utilityviewmodel))
                    {
                        return false;
                    }
                    else
                    {
                        PAYMENTS_RECEIVED = utilityviewmodel.value;
                    }
                    if (SmartParseV2016.Add_Minus_Sign(utilityviewmodel))
                    {
                        PAYMENTS_RECEIVED = utilityviewmodel.value;
                    }
                    if (!SmartParseV2016.Generic_Parse_Integer(PAYMENTS_RECEIVED, utilityviewmodel))
                    {
                        return false;
                    }
                    //else
                    //{
                    //    temp = utilityviewmodel.genericTransactionValue;
                    //}
                    SmartParseV2016.Update_Bill(ourviewmodel, utilityviewmodel, "PAYMENTS_RECEIVED", PAYMENTS_RECEIVED);
                    goto the_old_daysB2;
                }

                switch (utilityviewmodel.resource_code)
                {
                    case SmartParametersV2016.Electricity:

                        if (E_Total(ourviewmodel, utilityviewmodel, ref line_count, sections, utilityviewmodel.currentIndex, lines))
                        {
                            goto the_old_daysB2;
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(ourviewmodel.errorMessage))
                            {
                                return false;
                            }
                        }
                        break;
                    case SmartParametersV2016.Gas:
                        if (G_Total(ourviewmodel, utilityviewmodel, ref line_count, sections, utilityviewmodel.currentIndex, lines))
                        {
                            goto the_old_daysB2;
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(ourviewmodel.errorMessage))
                            {
                                return false;
                            }
                        }
                        break;
                    default:
                        break;
                }
                // She is such a FUCKING NOISY BITCH ON THE TELEPHONE - SHOUTING DOWN IT ALL THE TIME...
                // Now wonder I can never think when she's around - so FUKCING noisy

                // and I thought YOU were thick!!!!
                // This is someone who thinks Gt Budworth is near Macclesfield
                // Basically - none of her dumb family have a CLUE where
                // anywhere in the country is
                if (!Direct_Debit_Discount1(ourviewmodel, utilityviewmodel, ref line_count, lines))
                {
                    if (!string.IsNullOrEmpty(ourviewmodel.errorMessage))
                    {
                        return false;
                    }
                }

                if (Decode_VAT(ourviewmodel, utilityviewmodel))
                {
                    goto the_old_daysB2;
                }
                else
                {
                    if (!string.IsNullOrEmpty(ourviewmodel.errorMessage))
                    {
                        return false;
                    }
                }

                if (Outstanding_Balance(ourviewmodel, utilityviewmodel, line_count, lines, sections, utilityviewmodel.currentIndex))
                {
                    goto the_old_daysB2;
                }
                else
                {
                    // Didn't find the token
                    if (!string.IsNullOrEmpty(ourviewmodel.errorMessage))
                    {
                        // But found a problem
                        return false;
                    }
                }
            the_old_daysB2:
                continue;
            }
            return true;
        }

        private static bool Parse_SectionB4(UtilityViewModel utilityviewmodel,
                                            string[][] sections,
                                            string section,
                                            string sub_section)
        {
            utilityviewmodel.currentIndex = 0;
            if (!SmartParseV2016.Decode_Section(section, sub_section, sections, utilityviewmodel.resource_code.ToString(), utilityviewmodel))
            {
                return true;
            }

            for (int line_count = Convert.ToInt32(sections[utilityviewmodel.currentIndex][4]); line_count <= Convert.ToInt32(sections[utilityviewmodel.currentIndex][5]); line_count++)
            {
                //token = lines[line_count].Trim();

                //if (SmartParseV2016.Token_Identify(utilityviewmodel, "S", true, true))
                //{
                //    if (SmartParseV2016.fix_supply_number(line_count,
                //                            lines,
                //                            sections[section_index][5],
                //                            sections[section_index][4],
                //                            utilityviewmodel,
                //                            false))
                //    {
                //        SmartUtilityV2022.Amelia_Update_Mpan(utilityviewmodel, utilityviewmodel.mpan);
                //    }
                //    line_count = Convert.ToInt32(sections[section_index][5]);
                //    goto the_old_daysB4;
                //}
                //the_old_daysB4:
                //continue;
            }
            return true;
        }

        private static bool Parse_SectionC0(MainViewModel ourviewmodel,
                                            UtilityViewModel utilityviewmodel,
                                            string[][] sections,
                                            string section,
                                            string sub_section,
                                            string[] lines)
        {
            utilityviewmodel.currentIndex = 0;
            if (!SmartParseV2016.Decode_Section(section, sub_section, sections, utilityviewmodel.resource_code.ToString(), utilityviewmodel))
            {
                return true;
            }

            utilityviewmodel.token = "";

            for (int line_count = Convert.ToInt32(sections[utilityviewmodel.currentIndex][4]); line_count <= Convert.ToInt32(sections[utilityviewmodel.currentIndex][5]); line_count++)
            {
                utilityviewmodel.token = lines[line_count].Trim();

                if (Payment_Received(ourviewmodel,
                                            utilityviewmodel,
                                            ref line_count,
                                            sections,
                                            lines,
                                            utilityviewmodel.currentIndex))
                {
                    goto the_old_daysC0;
                }
                else
                {
                    if (!string.IsNullOrEmpty(ourviewmodel.errorMessage))
                    {
                        return false;
                    }
                }
            the_old_daysC0:
                continue;
            }
            return true;
        }

        private static bool Parse_SectionC1(MainViewModel ourviewmodel,
                                            UtilityViewModel utilityviewmodel,
                                            string[][] sections,
                                            string section,
                                            string sub_section,
                                            string[] lines)
        {
            utilityviewmodel.currentIndex = 0;
            if (!SmartParseV2016.Decode_Section(section, sub_section, sections, utilityviewmodel.resource_code.ToString(), utilityviewmodel))
            {
                return true;
            }

            utilityviewmodel.token = "";
            utilityviewmodel.METER_SERIAL_NO = utilityviewmodel.sparks.meter_serial_no;
            utilityviewmodel.units_band = 0;

            utilityviewmodel.READINGS_PERIOD_START = SmartParametersV2016.defaultDates;
            utilityviewmodel.READINGS_PERIOD_END = SmartParametersV2016.defaultDates;

            utilityviewmodel.UNITS = "0";
            utilityviewmodel.UNITS_RATE = "0";
            utilityviewmodel.UNITS_COST = "0";
            utilityviewmodel.UNIT_OF_MEASURE = "";
            char UNITS_TIME = SmartParametersV2016.daytimeUnit;    // Assume initially its D ays i.e. not E7


            for (int line_count = Convert.ToInt32(sections[utilityviewmodel.currentIndex][4]); line_count <= Convert.ToInt32(sections[utilityviewmodel.currentIndex][5]); line_count++)
            {
                utilityviewmodel.token = lines[line_count].Trim();
                if (string.IsNullOrEmpty(utilityviewmodel.sparks.meter_serial_no))
                {
                    if (E_Meter_Serial_Number(utilityviewmodel))
                    {
                        goto the_old_daysC1;
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(ourviewmodel.errorMessage))
                        {
                            return false;
                        }
                    }
                }

                if (E_Readings_Units_New(utilityviewmodel,
                                    ref line_count,
                                    sections,
                                    lines,
                                    utilityviewmodel.currentIndex,
                                    utilityviewmodel.METER_SERIAL_NO))
                {
                    goto the_old_daysC1;
                }
                else
                {
                    if (!string.IsNullOrEmpty(ourviewmodel.errorMessage))
                    {
                        return false;
                    }
                }

                if (Kwh_Used(utilityviewmodel,
                                UNITS_TIME))
                {
                    goto the_old_daysC1;
                }
                else
                {
                    if (!string.IsNullOrEmpty(ourviewmodel.errorMessage))
                    {
                        return false;
                    }
                }


                if (E_Standing_Charges(utilityviewmodel,
                                    ref line_count,
                                    sections,
                                    lines,
                                    utilityviewmodel.currentIndex,
                                    utilityviewmodel.units_band))
                {
                    goto the_old_daysC1;
                }
                else
                {
                    if (!string.IsNullOrEmpty(ourviewmodel.errorMessage))
                    {
                        return false;
                    }
                }
            the_old_daysC1:
                continue;
            }
            return true;
        }

        private static bool Parse_SectionC2(MainViewModel ourviewmodel,
                                            UtilityViewModel utilityviewmodel,
                                            string[][] sections,
                                            string section,
                                            string sub_section,
                                            string[] lines)
        {
            utilityviewmodel.currentIndex = 0;
            if (!SmartParseV2016.Decode_Section(section, sub_section, sections, utilityviewmodel.resource_code.ToString(), utilityviewmodel))
            {
                return true;
            }

            // Dribble  <= Drizzle!!
            // Ergonomically  <= Aerod eye namically!!!
            utilityviewmodel.token = "";

            string total_cost_of_electricity_used = "Total cost of electricity used";
            utilityviewmodel.discount_item = 0;

            for (int line_count = Convert.ToInt32(sections[utilityviewmodel.currentIndex][4]); line_count <= Convert.ToInt32(sections[utilityviewmodel.currentIndex][5]); line_count++)
            {
                utilityviewmodel.token = lines[line_count].Trim();
                if (SmartParseV2016.Token_Identify(utilityviewmodel, total_cost_of_electricity_used, true))
                {
                    //int temp = 0;

                    // She is such a FUCKING NOISY BITCH ON THE TELEPHONE - SHOUTING DOWN IT ALL THE TIME...
                    // Now wonder I can never think when she's around - so FUKCING noisy
                    utilityviewmodel.token = SmartParseV2016.Remove_Double_Spaces_V3(utilityviewmodel.token);
                    if (string.IsNullOrEmpty(utilityviewmodel.token))
                    {
                        // Loop round until we find a currency symbol
                        int outer_index = line_count + 1;
                        while (outer_index <= Convert.ToInt32(sections[utilityviewmodel.currentIndex][5]))
                        {
                            utilityviewmodel.token = lines[outer_index].Trim();
                            if (utilityviewmodel.token.Contains(utilityviewmodel.bill_currency_symbol))
                            {
                                // and I thought YOU were thick!!!!
                                // This is someone who thinks Gt Budworth is near Macclesfield
                                // Basically - none of her dumb family have a CLUE where
                                // anything in the country is
                                line_count = outer_index;
                                break;
                            }
                            outer_index++;
                        }
                    }
                    string E_NEW_CHARGES;// = "0";

                    if (!SmartParseV2016.Currency_Strip(utilityviewmodel.bill_currency_symbol, utilityviewmodel.bill_currency_separator, utilityviewmodel.bill_thousands_separator, utilityviewmodel.token, utilityviewmodel))
                    {
                        return false;
                    }
                    else
                    {
                        E_NEW_CHARGES = utilityviewmodel.value;
                    }
                    if (!SmartParseV2016.Generic_Parse_Integer(E_NEW_CHARGES, utilityviewmodel))
                    {
                        return false;
                    }
                    //else
                    //{
                    //    temp = utilityviewmodel.genericTransactionValue;
                    //}
                    SmartParseV2016.Update_Bill(ourviewmodel, utilityviewmodel, "RESOURCE_NEW_CHARGES", E_NEW_CHARGES);
                    goto the_old_daysC2;
                }

                if (!Direct_Debit_Discount1(ourviewmodel, utilityviewmodel, ref line_count, lines))
                {
                    if (!string.IsNullOrEmpty(ourviewmodel.errorMessage))
                    {
                        return false;
                    }
                }

                if (!Direct_Debit_Discount2(ourviewmodel, utilityviewmodel))
                {
                    if (!string.IsNullOrEmpty(ourviewmodel.errorMessage))
                    {
                        return false;
                    }
                }

                if (!Dual_Fuel_Discount(ourviewmodel, utilityviewmodel))
                {
                    if (!string.IsNullOrEmpty(ourviewmodel.errorMessage))
                    {
                        return false;
                    }
                }
            the_old_daysC2:
                continue;
            }
            return true;
        }

        private static bool Parse_SectionC3(MainViewModel ourviewmodel,
                                            UtilityViewModel utilityviewmodel,
                                            string[][] sections,
                                            string section,
                                            string sub_section,
                                            string[] lines)
        {
            utilityviewmodel.currentIndex = 0;
            if (!SmartParseV2016.Decode_Section(section, sub_section, sections, utilityviewmodel.resource_code.ToString(), utilityviewmodel))
            {
                return true;
            }

            utilityviewmodel.token = "";
            utilityviewmodel.METER_SERIAL_NO = utilityviewmodel.sparks.meter_serial_no;
            utilityviewmodel.units_band = 0;

            for (int line_count = Convert.ToInt32(sections[utilityviewmodel.currentIndex][4]); line_count <= Convert.ToInt32(sections[utilityviewmodel.currentIndex][5]); line_count++)
            {
                utilityviewmodel.token = lines[line_count].Trim();
                if (string.IsNullOrEmpty(utilityviewmodel.METER_SERIAL_NO))
                {
                    if (G_Meter_Serial_Number(utilityviewmodel))
                    {
                        goto the_old_daysC3;
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(ourviewmodel.errorMessage))
                        {
                            return false;
                        }
                    }
                }

                if (G_Readings_Units(utilityviewmodel,
                                    ref line_count,
                                    sections,
                                    lines,
                                    utilityviewmodel.currentIndex,
                                    utilityviewmodel.METER_SERIAL_NO))
                {
                    goto the_old_daysC3;
                }
                else
                {
                    if (!string.IsNullOrEmpty(ourviewmodel.errorMessage))
                    {
                        return false;
                    }
                }

                if (G_Standing_Charges(utilityviewmodel,
                                    ref line_count,
                                    sections,
                                    lines,
                                    utilityviewmodel.currentIndex,
                                    utilityviewmodel.units_band))
                {
                    goto the_old_daysC3;
                }
                else
                {
                    if (!string.IsNullOrEmpty(ourviewmodel.errorMessage))
                    {
                        return false;
                    }
                }
            the_old_daysC3:
                continue;
            }
            return true;
        }

        private static bool Check_Calorific_Value(UtilityViewModel utilityviewmodel)
        {
            string VOLUMECORRECTION = "x " + SmartParametersV2016.VOLUMECORRECTION;
            if (SmartParseV2016.Token_Identify_Middle(utilityviewmodel, VOLUMECORRECTION, false, ""))
            {
                // Real kludge! for the earlier bills
                int volume = utilityviewmodel.token.IndexOf(VOLUMECORRECTION);
                if (volume >= 0)
                {
                    decimal TEMPDEC;

                    utilityviewmodel.token = utilityviewmodel.token.Substring(0, volume).Trim();
                    if (SmartParseV2016.Look_Backwards(utilityviewmodel, SmartParametersV2016.space, SmartParametersV2016.bar.ToString()))
                    {
                        string[] components = utilityviewmodel.token.Split(SmartParametersV2016.bar);
                        if (components.Length == 2)
                        {
                            utilityviewmodel.CALORIFIC_VALUE = components[1];

                            if (!SmartParseV2016.Generic_Parse_Decimal(utilityviewmodel.CALORIFIC_VALUE, utilityviewmodel))
                            {
                                return false;
                            }
                            else
                            {
                                TEMPDEC = utilityviewmodel.genericDecimalValue;
                            }
                            foreach (SmartUtility.GReadings g_readings_row in utilityviewmodel.Hezbollah.g_readings_changesList)
                            {
                                if (g_readings_row.CALORIFIC_VALUE == 0.0M)
                                {
                                    g_readings_row.CALORIFIC_VALUE = TEMPDEC;
                                }
                            }
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        private static bool Parse_SectionC4(MainViewModel ourviewmodel,
                                            UtilityViewModel utilityviewmodel,
                                            string[][] sections,
                                            string section,
                                            string sub_section,
                                            string[] lines)
        {
            utilityviewmodel.currentIndex = 0;
            if (!SmartParseV2016.Decode_Section(section, sub_section, sections, utilityviewmodel.resource_code.ToString(), utilityviewmodel))
            {
                return true;
            }

            // Dribble  <= Drizzle!!
            // Ergonomically  <= Aerod eye namically!!!
            utilityviewmodel.token = "";

            string total_cost_of_gas_used = "Total cost of gas used";

            for (int line_count = Convert.ToInt32(sections[utilityviewmodel.currentIndex][4]); line_count <= Convert.ToInt32(sections[utilityviewmodel.currentIndex][5]); line_count++)
            {
                utilityviewmodel.token = lines[line_count].Trim();
                if (SmartParseV2016.Token_Identify(utilityviewmodel, total_cost_of_gas_used, true))
                {
                    //int temp = 0;

                    // She is such a FUCKING NOISY BITCH ON THE TELEPHONE - SHOUTING DOWN IT ALL THE TIME...
                    // Now wonder I can never think when she's around - so FUKCING noisy
                    utilityviewmodel.token = SmartParseV2016.Remove_Double_Spaces_V3(utilityviewmodel.token);
                    if (string.IsNullOrEmpty(utilityviewmodel.token))
                    {
                        // Loop round until we find a currency symbol
                        int outer_index = line_count + 1;
                        while (outer_index <= Convert.ToInt32(sections[utilityviewmodel.currentIndex][5]))
                        {
                            utilityviewmodel.token = lines[outer_index].Trim();
                            if (utilityviewmodel.token.Contains(utilityviewmodel.bill_currency_symbol))
                            {
                                // and I thought YOU were thick!!!!
                                // This is someone who thinks Gt Budworth is near Macclesfield
                                // Basically - none of her dumb family have a CLUE where
                                // anything in the country is
                                line_count = outer_index;
                                break;
                            }
                            outer_index++;
                        }
                    }
                    string G_NEW_CHARGES;// = "0";

                    if (!SmartParseV2016.Currency_Strip(utilityviewmodel.bill_currency_symbol, utilityviewmodel.bill_currency_separator, utilityviewmodel.bill_thousands_separator, utilityviewmodel.token, utilityviewmodel))
                    {
                        return false;
                    }
                    else
                    {
                        G_NEW_CHARGES = utilityviewmodel.value;
                    }
                    if (!SmartParseV2016.Generic_Parse_Integer(G_NEW_CHARGES, utilityviewmodel))
                    {
                        return false;
                    }
                    //else
                    //{
                    //    temp = utilityviewmodel.genericTransactionValue;
                    //}
                    SmartParseV2016.Update_Bill(ourviewmodel, utilityviewmodel, "RESOURCE_NEW_CHARGES", G_NEW_CHARGES);
                    goto the_old_daysC4;
                }

                if (Check_Calorific_Value(utilityviewmodel))
                {
                    goto the_old_daysC4;
                }
                else
                {
                    if (!string.IsNullOrEmpty(ourviewmodel.errorMessage))
                    {
                        return false;
                    }
                }
            the_old_daysC4:
                continue;
            }
            return true;
        }

        private static bool Parse_SectionC5(MainViewModel ourviewmodel,
                                            UtilityViewModel utilityviewmodel,
                                            string[][] sections,
                                            string section,
                                            string sub_section,
                                            string[] lines)
        {
            utilityviewmodel.currentIndex = 0;
            if (!SmartParseV2016.Decode_Section(section, sub_section, sections, utilityviewmodel.resource_code.ToString(), utilityviewmodel))
            {
                return true;
            }

            // Dribble  <= Drizzle!!
            // Ergonomically  <= Aerod eye namically!!!
            utilityviewmodel.token = "";

            string your_adjustment = "Your adjustment",
                    adjustments_after_VAT = "Adjustments after VAT",
                    total_adjustments = "Total adjustments",
                    adjustments = "Adjustments",
                    total = "Total";

            bool start_looking = false;
            for (int line_count = Convert.ToInt32(sections[utilityviewmodel.currentIndex][4]); line_count <= Convert.ToInt32(sections[utilityviewmodel.currentIndex][5]); line_count++)
            {
                utilityviewmodel.token = lines[line_count].Trim();
                if (SmartParseV2016.Token_Identify(utilityviewmodel, your_adjustment, true) ||
                    SmartParseV2016.Token_Identify(utilityviewmodel, adjustments_after_VAT, true) ||
                    SmartParseV2016.Token_Identify(utilityviewmodel, adjustments, true))
                {
                    start_looking = true;
                    goto the_old_daysC5;
                }

                if (SmartParseV2016.Token_Identify(utilityviewmodel, total_adjustments, true) ||
                    SmartParseV2016.Token_Identify(utilityviewmodel, total, true))
                {
                    break;
                }

                if (start_looking)
                {
                    // Fuck this is complicated

                    utilityviewmodel.ACCOUNT_TYPE = "";
                    utilityviewmodel.ACCOUNT_AMOUNT = "";
                    short ACCOUNT_VAT_CODE = SmartParametersV2016.zeroRateVatCode;      // 0% VAT
                    utilityviewmodel.ACCOUNT_DATE = SmartParametersV2016.defaultDates;

                    // She is such a FUCKING NOISY BITCH ON THE TELEPHONE - SHOUTING DOWN IT ALL THE TIME...
                    // Now wonder I can never think when she's around - so FUKCING noisy

                    // I am going to have to bite the bullet here ...
                    // Need to find the last space once I have a £ sign on the line (not a £ sign at the beginning)
                    // So from 'last space' to end is our REFUND AMOUNT eg |-£30.00 or |£30.00
                    // Then work BACKWARDS to find the date  e.g. - 19 Sep 48|-£30.00
                    //                                             ^  ^   ^
                    //                                             3rd2nd 1st
                    bool found_it = false;
                    while (!found_it)
                    {
                        int len = utilityviewmodel.token.Length;
                        if (len > 0)
                        {
                            // Sometimes ... it all comes on one line with a 'dr' (or a 'cr') at the end ..
                            string stripped_token = utilityviewmodel.token.Replace("dr", "");
                            stripped_token = stripped_token.Replace("cr", "");
                            char last_char = Convert.ToChar(stripped_token.Substring(stripped_token.Length - 1, 1));
                            if (Char.IsDigit(last_char))
                            {
                                if (utilityviewmodel.token.Contains(utilityviewmodel.bill_currency_symbol))
                                {
                                    //found_it = true;
                                    break;
                                }
                            }
                            line_count++;
                            if (line_count > Convert.ToInt32(sections[utilityviewmodel.currentIndex][5]))
                            {
                                goto the_old_daysC5;
                            }
                            utilityviewmodel.token = utilityviewmodel.token + SmartParametersV2016.space + lines[line_count].Trim();
                        }
                    }

                    if (SmartParseV2016.Look_Backwards(utilityviewmodel, SmartParametersV2016.space, SmartParametersV2016.bar.ToString()))
                    {
                        // Then work BACKWARDS to find the last space  e.g. - 19 Sep 48|-£30.00
                        // Do Year
                        if (SmartParseV2016.Look_Backwards(utilityviewmodel, SmartParametersV2016.space, "?"))
                        {
                            // Then work BACKWARDS to find the last space  e.g. - 19 Sep?48|-£30.00
                            // Do Month
                            if (SmartParseV2016.Look_Backwards(utilityviewmodel, SmartParametersV2016.space, "?"))
                            {
                                // Then work BACKWARDS to find the last space  e.g. - 19?Sep?48|-£30.00
                                if (SmartParseV2016.Look_Backwards(utilityviewmodel, SmartParametersV2016.space, SmartParametersV2016.bar.ToString()))
                                {
                                    // Now we have  e.g. -|19?Sep?48|-£30.00 and can split on |
                                    if (!Reduce_C5(ourviewmodel,
                                                    utilityviewmodel))
                                    {
                                        return false;
                                    }

                                    if ((utilityviewmodel.ACCOUNT_DATE != SmartParametersV2016.defaultDates) &&
                                        !string.IsNullOrEmpty(utilityviewmodel.ACCOUNT_TYPE) &&
                                        !string.IsNullOrEmpty(utilityviewmodel.ACCOUNT_AMOUNT))
                                    {
                                        string INCLUDE_BILLS = SmartParametersV2016.yesFlag;
                                        // Need to increment this for multiple inserts on the same Bill date
                                        utilityviewmodel.ACCOUNT_CHARGES_CREDITS_ITEM = (short)(utilityviewmodel.ACCOUNT_CHARGES_CREDITS_ITEM + 1);
                                        if (!SmartParseV2016.Check_Adjustment_Is_There(ourviewmodel,
                                                    utilityviewmodel,
                                                    Convert.ToDateTime(utilityviewmodel.ACCOUNT_DATE),
                                                    utilityviewmodel.ACCOUNT_CHARGES_CREDITS_ITEM,
                                                    utilityviewmodel.ACCOUNT_TYPE,
                                                    ACCOUNT_VAT_CODE,
                                                    Convert.ToInt32(utilityviewmodel.ACCOUNT_AMOUNT)))
                                        {
                                            SmartUtilityV2022.Account_Charges_CreditsList_Add(utilityviewmodel,
                                                                            SmartParametersV2016.Utility,
                                                                            utilityviewmodel.supplier_code,
                                                                            utilityviewmodel.brand_code,
                                                                            utilityviewmodel.ACCOUNT_NO,
                                                                            utilityviewmodel.CREATED,
                                                                            utilityviewmodel.STATEMENT_ID,
                                                                            utilityviewmodel.BILL_DATE,
                                                                            Convert.ToDateTime(utilityviewmodel.ACCOUNT_DATE),
                                                                            utilityviewmodel.ACCOUNT_CHARGES_CREDITS_ITEM,
                                                                            utilityviewmodel.ACCOUNT_TYPE,
                                                                            ACCOUNT_VAT_CODE,
                                                                            Convert.ToInt32(utilityviewmodel.ACCOUNT_AMOUNT),
                                                                            INCLUDE_BILLS);
                                            SmartParseV2016.Update_Bill(ourviewmodel, utilityviewmodel, "ACCOUNT_CHARGES_CREDITS", utilityviewmodel.ACCOUNT_AMOUNT);
                                            // Note: ACCOUNT_AMOUNT has already been checked as an Integer}
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

            the_old_daysC5:
                continue;
            }
            return true;
        }

        private static bool Reduce_C5(MainViewModel ourviewmodel,
                                UtilityViewModel utilityviewmodel)
        {
            string[] components = utilityviewmodel.token.Split(SmartParametersV2016.bar);
            int component_count = 0;
            while (component_count < components.Length)
            {
                switch (component_count)
                {
                    case 0:
                        utilityviewmodel.ACCOUNT_TYPE = components[component_count].Trim(SmartParametersV2016.dashSplit).Trim();
                        break;
                    case 1:
                        string TEMP_DATE = components[component_count].Replace("?", SmartParametersV2016.space).Trim();
                        if (!SmartParseV2016.Generic_Parse_Datetime(TEMP_DATE, utilityviewmodel))
                        {
                            return false;
                        }
                        else
                        {
                            utilityviewmodel.ACCOUNT_DATE = TEMP_DATE;
                        }
                        break;
                    case 2:
                        if (!SmartParseV2016.Currency_Strip(utilityviewmodel.bill_currency_symbol, utilityviewmodel.bill_currency_separator, utilityviewmodel.bill_thousands_separator, components[component_count].Trim(), utilityviewmodel))
                        {
                            return false;
                        }
                        else
                        {
                            TEMP_DATE = utilityviewmodel.value;
                        }
                        if (!SmartParseV2016.Generic_Parse_Integer(TEMP_DATE, utilityviewmodel))
                        {
                            return false;
                        }
                        else
                        {
                            utilityviewmodel.ACCOUNT_AMOUNT = TEMP_DATE;
                        }
                        SmartParseV2016.Update_Bill(ourviewmodel, utilityviewmodel, "ACCOUNT_CHARGES_CREDITS", utilityviewmodel.ACCOUNT_AMOUNT.ToString());
                        break;
                    default:
                        break;
                }
                component_count++;
            }
            return true;
        }

        private static async Task<bool> Parse_SectionD0(MainViewModel ourviewmodel,
                                             UtilityViewModel utilityviewmodel,
                                             string[][] sections,
                                            string section,
                                            string sub_section,
                                            string[] lines)
        {
            // Not perfect but not a bad start            

            utilityviewmodel.currentIndex = 0;
            if (!SmartParseV2016.Decode_Section(section, sub_section, sections, utilityviewmodel.resource_code.ToString(), utilityviewmodel))
            {
                return true;
            }

            utilityviewmodel.token = "";

            string youre_on1 = "You’re on ",
                    youre_on2 = "You're on ",
                    youre_on3 = "You're on our ",
                    energy_tariff = " tariff:",
                    Tariff_name = "Tariff name",
                    Payment_method = "Payment method";

            for (int line_count = Convert.ToInt32(sections[utilityviewmodel.currentIndex][4]); line_count <= Convert.ToInt32(sections[utilityviewmodel.currentIndex][5]); line_count++)
            {
                utilityviewmodel.token = lines[line_count].Trim();
                // Not sure about these !!! with Token_Identify
                if (utilityviewmodel.token.Length >= youre_on1.Length)
                {
                    if ((utilityviewmodel.token.Substring(0, youre_on1.Length) == youre_on1) ||
                        (utilityviewmodel.token.Substring(0, youre_on2.Length) == youre_on2))
                    {
                        utilityviewmodel.token = utilityviewmodel.token.Replace(youre_on3, "");
                        utilityviewmodel.token = utilityviewmodel.token.Replace(youre_on1, "");
                        utilityviewmodel.token = utilityviewmodel.token.Replace(youre_on2, "");
                        if (!await D0_One(ourviewmodel, utilityviewmodel))
                        {
                            return false;
                        }

                        goto the_old_daysD0;
                    }
                }

                if (SmartParseV2016.Token_Identify(utilityviewmodel, energy_tariff, true) ||
                    SmartParseV2016.Token_Identify(utilityviewmodel, Tariff_name, true))
                {
                    if (!await Check_Token_First(ourviewmodel, utilityviewmodel))
                    {
                        return false;
                    }
                    goto the_old_daysD0;
                }

                if (SmartParseV2016.Token_Identify(utilityviewmodel, Payment_method, true))
                {
                    if (!Check_Token_Second(ourviewmodel, utilityviewmodel))
                    {
                        return false;
                    }
                    goto the_old_daysD0;
                }
            the_old_daysD0:
                continue;
            }
            return true;
        }

        private static bool Check_Token_Second(MainViewModel ourviewmodel,
                                                UtilityViewModel utilityviewmodel)
        {
            if (!string.IsNullOrEmpty(utilityviewmodel.token))
            {
                // Make sure the Tariff name doesn't contain any commas (this will fuck up SmartDBServer
                // which isn't expecting any!

                string PAYMENT_TYPE = utilityviewmodel.token;
                if (!SmartSpikeUtilityV2017.Utility_Lookup_PaymentPlan(ourviewmodel, utilityviewmodel, PAYMENT_TYPE))
                {
                    // utilityviewmodel.urgent message should be set here
                    return false;
                }
                else
                {
                    PAYMENT_TYPE = utilityviewmodel.PAYMENT_PLAN.ToString();
                }
                SmartParseV2016.Update_Bill(ourviewmodel, utilityviewmodel, "PAYMENT_PLAN", utilityviewmodel.PAYMENT_PLAN.ToString());
                SmartParseV2016.Update_Bill(ourviewmodel, utilityviewmodel, "PAYMENT_TYPE", PAYMENT_TYPE);
            }

            return true;
        }

        private static async Task<bool> Check_Token_First(MainViewModel ourviewmodel,
                                                            UtilityViewModel utilityviewmodel)
        {
            if (!string.IsNullOrEmpty(utilityviewmodel.token))
            {
                // Make sure the Tariff name doesn't contain any commas (this will fuck up SmartDBServer
                // which isn't expecting any!
                utilityviewmodel.TARIFF_NAME = utilityviewmodel.token;
                if (!await SmartUtilityV2022.Lookup_Remote_Tariff_Code(ourviewmodel, utilityviewmodel))
                {
                    return false;
                }
                else
                {
                    SmartParseV2016.Update_Bill(ourviewmodel, utilityviewmodel, "TARIFF_CODE", utilityviewmodel.TARIFF_CODE.ToString());
                }
            }
            return true;
        }

        private static async Task<bool> D0_One(MainViewModel ourviewmodel,
                                                UtilityViewModel utilityviewmodel)
        {
            int tariff_index = utilityviewmodel.token.IndexOf("tariff");
            if (tariff_index == -1)
            {
                tariff_index = utilityviewmodel.token.IndexOf("Tariff");
            }
            if (tariff_index >= 0)
            {
                utilityviewmodel.token = utilityviewmodel.token.Substring(0, tariff_index).Trim();
            }
            if (utilityviewmodel.token.Length >= 2)
            {
                // Fucking clumsy irritating useless meddling oaf
                // Make sure the Tariff name doesn't contain any commas (this will fuck up SmartDBServer
                // which isn't expecting any!
                utilityviewmodel.TARIFF_NAME = char.ToUpper(utilityviewmodel.token[0]) + utilityviewmodel.token.Substring(0, 1);
                if (!await SmartUtilityV2022.Lookup_Remote_Tariff_Code(ourviewmodel, utilityviewmodel))
                {
                    return false;
                }
                else
                {
                    SmartParseV2016.Update_Bill(ourviewmodel, utilityviewmodel, "TARIFF_CODE", utilityviewmodel.TARIFF_CODE.ToString());
                }
            }
            return true;
        }

        private static void Examine_Next_Line(UtilityViewModel utilityviewmodel,
                                                int outer_index,
                                                string[][] sections,
                                                int section_index,
                                                string[] lines)
        {
            // Can we look at the next line?
            int temp_sub = outer_index + 1;
            if (temp_sub <= Convert.ToInt32(sections[section_index][5]))
            {
                // Yes ... is it "in debit" (which means Consumer owes BG)
                if ((lines[temp_sub].Trim() == "in debit") ||
                    (lines[temp_sub].Trim() == "debit"))
                {
                    // Tack it after the token
                    utilityviewmodel.token = utilityviewmodel.token + SmartParametersV2016.space + lines[temp_sub].Trim();
                }
                if ((lines[temp_sub].Trim() == "in credit") ||
                    (lines[temp_sub].Trim() == "credit"))
                {
                    // Tack it after the token
                    utilityviewmodel.token = utilityviewmodel.token + SmartParametersV2016.space + lines[temp_sub].Trim();
                }
            }
            return;
        }

        private static bool Previous_Balance(MainViewModel ourviewmodel,
                                            UtilityViewModel utilityviewmodel,
                                            int line_count,
                                            string[] lines,
                                            string[][] sections,
                                            int section_index)
        {
            string balance_of_your_last = "Balance of your last",
                    your_balance_was_in = "Your balance was in",
                    your_account_balance_before = "Your account balance before";

            if (SmartParseV2016.Token_Identify(utilityviewmodel, balance_of_your_last, true) ||
                    SmartParseV2016.Token_Identify(utilityviewmodel, your_balance_was_in, true) ||
                    SmartParseV2016.Token_Identify(utilityviewmodel, your_account_balance_before, true))
            {
                string PREVIOUS_BALANCE;// = "0";
                //int TEMP = 0;

                utilityviewmodel.token = utilityviewmodel.token.Replace("by", "").Trim();
                utilityviewmodel.token = SmartParseV2016.Remove_Double_Spaces_V3(utilityviewmodel.token);
                int currency_index = utilityviewmodel.token.IndexOf(utilityviewmodel.bill_currency_symbol);
                if (currency_index == -1)
                {
                    // Loop round until we find a currency symbol
                    int outer_index = line_count;
                    while (outer_index <= Convert.ToInt32(sections[section_index][5]))
                    {
                        if (utilityviewmodel.token.Contains(utilityviewmodel.bill_currency_symbol))
                        {
                            Examine_Next_Line(utilityviewmodel, outer_index, sections, section_index, lines);
                            break;
                        }
                        outer_index++;
                        utilityviewmodel.token = lines[outer_index].Trim();
                    }
                }
                if (!SmartParseV2016.Currency_Strip(utilityviewmodel.bill_currency_symbol, utilityviewmodel.bill_currency_separator, utilityviewmodel.bill_thousands_separator, utilityviewmodel.token, utilityviewmodel))
                {
                    return false;
                }
                else
                {
                    PREVIOUS_BALANCE = utilityviewmodel.value;
                }
                if (!SmartParseV2016.Generic_Parse_Integer(PREVIOUS_BALANCE, utilityviewmodel))
                {
                    return false;
                }
                // else
                //{
                //   TEMP = utilityviewmodel.genericTransactionValue;
                //}
                SmartParseV2016.Update_Bill(ourviewmodel, utilityviewmodel, "PREVIOUS_BALANCE", PREVIOUS_BALANCE);
                // Matched the token and everything ok
                return true;
            }
            // Didn't match the token
            return false;
        }

        private static bool Outstanding_Balance(MainViewModel ourviewmodel,
                                                UtilityViewModel utilityviewmodel,
                                                int line_count,
                                                string[] lines,
                                                string[][] sections,
                                                int section_index)
        {
            string your_account_balance_now = "Your account balance now",
                    your_account_balance_is_in = "Your account balance is in",
                    your_new = "Your new";

            utilityviewmodel.working_token = lines[line_count + 1];

            if (SmartParseV2016.Token_Identify(utilityviewmodel, your_account_balance_is_in, true) ||
                SmartParseV2016.Token_Identify(utilityviewmodel, your_account_balance_now, true) ||
                (SmartParseV2016.Token_Identify(utilityviewmodel, your_new, true) &&
                SmartParseV2016.Token_IdentifyWorking(utilityviewmodel, "account balance", false)))
            {
                string OUTSTANDING_BALANCE;// = "0";
                //int TEMP = 0;

                utilityviewmodel.token = utilityviewmodel.token.Replace("by", "").Trim();
                utilityviewmodel.token = SmartParseV2016.Remove_Double_Spaces_V3(utilityviewmodel.token);
                // Loop round until we find a currency symbol
                if (utilityviewmodel.token.IndexOf(utilityviewmodel.bill_currency_symbol) == -1)
                {
                    int outer_index = line_count;
                    while (outer_index <= Convert.ToInt32(sections[section_index][5]))
                    {
                        if (utilityviewmodel.token.Contains(utilityviewmodel.bill_currency_symbol))
                        {
                            Examine_Next_Line(utilityviewmodel, outer_index, sections, section_index, lines);
                            break;
                        }
                        outer_index++;
                        utilityviewmodel.token = lines[outer_index].Trim();
                    }
                }
                if (!SmartParseV2016.Currency_Strip(utilityviewmodel.bill_currency_symbol, utilityviewmodel.bill_currency_separator, utilityviewmodel.bill_thousands_separator, utilityviewmodel.token, utilityviewmodel))
                {
                    return false;
                }
                else
                {
                    OUTSTANDING_BALANCE = utilityviewmodel.value;
                }
                if (!SmartParseV2016.Generic_Parse_Integer(OUTSTANDING_BALANCE, utilityviewmodel))
                {
                    return false;
                }
                //else
                //{
                //    TEMP = utilityviewmodel.genericTransactionValue;
                //}
                SmartParseV2016.Update_Bill(ourviewmodel, utilityviewmodel, "OUTSTANDING_BALANCE", OUTSTANDING_BALANCE);
                // Matched the token and everything ok
                return true;
            }
            // Didn't match the token
            return false;
        }

        private static bool Payment_Received(MainViewModel ourviewmodel,
                                            UtilityViewModel utilityviewmodel,
                                            ref int line_count,
                                            string[][] sections,
                                            string[] lines,
                                            int section_index)
        {
            // All PAYMENTS received are naturally - amounts because they CREDIT
            // your bill ... but in some of the earlier BG Bills the lazy fucking Foreigners
            // didn't bother to put the minus sign in front of the value

            string what_youve_paid1 = "What you've paid",
                    what_you_paid2 = "What you paid - thank you",
                    what_you_paid3 = "What you paid – thank you";

            if (SmartParseV2016.Token_Identify(utilityviewmodel, what_youve_paid1, true) ||
                SmartParseV2016.Token_Identify(utilityviewmodel, what_you_paid2, true) ||
                SmartParseV2016.Token_Identify(utilityviewmodel, what_you_paid3, true))
            {
                string your_account_balance_is_in = "Your account balance is in",
                        your_account_balance_now = "Your account balance now";
                string PAYMENTS_RECEIVED;// = "0";
                //int TEMP = 0;
                DateTime temp_date = SmartParametersV2016.defaultDate;

                utilityviewmodel.PAYMENT_BALANCE = "";
                int currency_index;// = 0;
                // Do we have a value on the same line?  i.e. as a "total"??
                if (!string.IsNullOrEmpty(utilityviewmodel.token))
                {
                    if (!SmartParseV2016.Currency_Strip(utilityviewmodel.bill_currency_symbol, utilityviewmodel.bill_currency_separator, utilityviewmodel.bill_thousands_separator, utilityviewmodel.token, utilityviewmodel))
                    {
                        return false;
                    }
                    else
                    {
                        PAYMENTS_RECEIVED = utilityviewmodel.value;
                    }
                    // Some bills don't flag PAYMENTS with a preceding -
                    if (SmartParseV2016.Add_Minus_Sign(utilityviewmodel))
                    {
                        PAYMENTS_RECEIVED = utilityviewmodel.value;
                    }
                    if (!SmartParseV2016.Generic_Parse_Integer(PAYMENTS_RECEIVED, utilityviewmodel))
                    {
                        return false;
                    }
                    SmartParseV2016.Update_Bill(ourviewmodel, utilityviewmodel, "PAYMENTS_RECEIVED", PAYMENTS_RECEIVED);
                }
                int outer_index = line_count + 1;
                while (outer_index <= Convert.ToInt32(sections[section_index][5]))
                {
                    utilityviewmodel.token = lines[outer_index].Trim();
                    if (utilityviewmodel.token.Contains(your_account_balance_is_in) ||
                        utilityviewmodel.token.Contains(your_account_balance_now))
                    {
                        break;
                    }
                    utilityviewmodel.last_date_index = -1;
                    // A payment line ... can we make out the date?
                    if (SmartNibbyV2016.Make_Out_Date(utilityviewmodel))
                    {
                        currency_index = utilityviewmodel.token.IndexOf(utilityviewmodel.bill_currency_symbol, utilityviewmodel.last_date_index);
                        if (currency_index == -1)
                        {
                            // Cant find the amount? Maybe its on the next line ...
                            if (outer_index + 1 <= Convert.ToInt32(sections[section_index][5]))
                            {
                                currency_index = lines[outer_index + 1].IndexOf(utilityviewmodel.bill_currency_symbol);
                                if (currency_index >= 0)
                                {
                                    // Yes
                                    // Move the line_count on
                                    outer_index++;
                                    // Adjust the token
                                    utilityviewmodel.token = utilityviewmodel.token + SmartParametersV2016.space + lines[outer_index];
                                    // Re-calc currency_index
                                    currency_index = utilityviewmodel.token.IndexOf(utilityviewmodel.bill_currency_symbol);
                                    // Remove the line
                                    lines[outer_index] = "";
                                }
                                else
                                {
                                    // No - give up
                                    goto update_sub;
                                }
                            }
                        }

                        // Look for space after currency symbol if poss
                        if (utilityviewmodel.token.Contains("Balance carried forward"))
                        {
                            goto update_sub;
                        }
                        int space_index = utilityviewmodel.token.IndexOf(SmartParametersV2016.space, currency_index); // currency_index + 1?
                        if (space_index >= 0)
                        {
                            utilityviewmodel.token = utilityviewmodel.token.Substring(0, space_index).Trim();
                        }
                        utilityviewmodel.PAYMENT_AMOUNT = "";
                        utilityviewmodel.PAYMENT_DATE = "";
                        utilityviewmodel.PAYMENT_METHOD = "";
                        if (!Payments_XYZ(utilityviewmodel))
                        {
                            return false;
                        }

                        utilityviewmodel.PAYMENT_METHOD = utilityviewmodel.PAYMENT_METHOD.Trim();
                        //short PAYMENT_CODE = 0;
                        if (!string.IsNullOrEmpty(utilityviewmodel.PAYMENT_METHOD))
                        {
                            if (!SmartSpikeUtilityV2017.Utility_Lookup_PaymentCode(ourviewmodel,
                                                                            utilityviewmodel,
                                                                            utilityviewmodel.PAYMENT_METHOD)) //rf PAYMENT_CODE
                            {
                                return false;
                            }
                            //utilityviewmodel.PAYMENT_METHOD = utilityviewmodel.PAYMENT_METHOD;
                        }
                        utilityviewmodel.payment_date = SmartNibbyV2016.ConvertDate(utilityviewmodel.PAYMENT_DATE,
                            SmartParametersV2016.defaultDate, ourviewmodel);
                        if (ourviewmodel.errorMessage != "")
                        {
                            return false;
                        }
                        if (!string.IsNullOrEmpty(utilityviewmodel.PAYMENT_DATE) &&
                            !string.IsNullOrEmpty(utilityviewmodel.PAYMENT_AMOUNT))
                        {
                            // Add it in ... perhaps
                            // If this section is E(lectricity) and G(as) then we might be doing it twice!
                            // Check if its there first
                            utilityviewmodel.PAYMENT_BALANCE = utilityviewmodel.PAYMENTS_BALANCE.ToString();
                            if (!SmartParseV2016.Check_Payment_Is_There(ourviewmodel,
                                                                        utilityviewmodel))      // Which IS the PAYMENT_DATE as a DateTime
                            {                                                               //utilityviewmodel.PAYMENTS_ITEM,

                                //SmartUtilityV2022.PaymentsList_Add(utilityviewmodel.Hezbollah.payments_changesList,
                                //                                    SmartParametersV2016.Utility,
                                //                                    utilityviewmodel.supplier_code,
                                //                                    utilityviewmodel.brand_code,
                                //                                    utilityviewmodel.ACCOUNT_NO,
                                //                                    utilityviewmodel.CREATED,
                                //                                    utilityviewmodel.STATEMENT_ID,
                                //                                    utilityviewmodel.BILL_DATE,
                                //                                    payment_date,
                                //                                    utilityviewmodel.PAYMENTS_ITEM,
                                //                                    PAYMENT_CODE,
                                //                                    Convert.ToInt32(PAYMENT_AMOUNT),
                                //                                    utilityviewmodel.PAYMENTS_BALANCE);

                                foreach (SmartUtility.Payments payment_row in utilityviewmodel.Hezbollah.payments_changesList)
                                {
                                    SmartUtilityV2022.PaymentsList_Add(utilityviewmodel,
                                                                    SmartParametersV2016.Utility,
                                                                    payment_row.SUPPLIER_CODE,
                                                                    payment_row.BRAND_CODE,
                                                                    payment_row.ACCOUNT_NO,
                                                                    payment_row.ACCOUNT_CREATED,
                                                                    payment_row.STATEMENT_ID,       // Because HERE we DO have a Bill Number (yet)
                                                                    payment_row.BILL_DATE.ToString(),
                                                                    payment_row.PAYMENT_DATE,
                                                                    payment_row.PAYMENT_ITEM,
                                                                    payment_row.PAYMENT_CODE,
                                                                    payment_row.PAYMENT_AMOUNT,
                                                                    payment_row.PAYMENT_BALANCE);

                                }
                            }
                            SmartParseV2016.Update_Bill(ourviewmodel, utilityviewmodel, "PAYMENT_AMOUNT", utilityviewmodel.PAYMENT_AMOUNT);

                            string PAYMENT_TYPE = utilityviewmodel.PAYMENT_METHOD;
                            if (!SmartSpikeUtilityV2017.Utility_Lookup_PaymentPlan(ourviewmodel, utilityviewmodel, PAYMENT_TYPE))
                            {
                                // This is because some early Bills don't have a Payment Method for us
                                // to deduce the pyment method e.g.:                                // About your tariff
                                //
                                //      This information will help you to compare your current tariff
                                //      with others available.
                                //      Your electricity tariff
                                //      Tariff name Standard
                                //      Payment method Monthly Direct Debit
                                //      Tariff ends on No end date
                                //      Exit fee(if you cancel this tariff before end date) Not applicable
                                //      Annual consumption
                                //      (based on your actual use in the last 12 months)
                                //      1709.00 kWh
                                // 
                                // So when we DON'T have this info we have to find the payment plan from
                                // the way they pay r.g:
                                //      What you've paid -£96.00
                                //      Direct Debit 6 Jun 2016 -£24.00
                                //      Direct Debit 5 Jul 2016 -£24.00
                                //      Direct Debit 5 Aug 2016 -£24.00
                                //      Pending 5 Sep 2016 -£24.00
                                //
                                // ...but this last line is going to fuck us because its a PAYMENT_TYPE
                                // and **NOT** a PAYMENT_PLAN.  So we cannot stop if we hit lines like these ...
                                //
                                // return false; <== We cannot do this
                            }
                            else
                            {
                                SmartParseV2016.Update_Bill(ourviewmodel, utilityviewmodel, "PAYMENT_PLAN", utilityviewmodel.PAYMENT_PLAN.ToString());
                                SmartParseV2016.Update_Bill(ourviewmodel, utilityviewmodel, "PAYMENT_TYPE", PAYMENT_TYPE);
                            }

                            if (temp_date != SmartParametersV2016.defaultDate)
                            {
                                temp_date = temp_date.AddMonths(1);
                                SmartParseV2016.Update_Bill(ourviewmodel, utilityviewmodel, "PAYMENT_DUE_DATE", temp_date.ToString(SmartParametersV2016.defaultCulture));
                                // Best we can do ..
                                SmartParseV2016.Update_Bill(ourviewmodel, utilityviewmodel, "DIRECT_DEBIT_DATE", temp_date.ToString(SmartParametersV2016.defaultCulture));
                            }
                        }
                    }
                update_sub:
                    outer_index++;
                }
                line_count = outer_index;
                return true;
            }
            return false;
        }

        private static bool Payments_XYZ(UtilityViewModel utilityviewmodel)
        {
            //int TEMP;
            string[] components = utilityviewmodel.token.Split(SmartParametersV2016.spaceSplit);
            // Note how this one goes IN REVERSE
            int item = components.Length - 1;
            while (item >= 0)
            {
                if (item == components.Length - 1)
                {
                    if (!SmartParseV2016.Currency_Strip(utilityviewmodel.bill_currency_symbol, utilityviewmodel.bill_currency_separator, utilityviewmodel.bill_thousands_separator, components[item], utilityviewmodel))
                    {
                        return false;
                    }
                    utilityviewmodel.PAYMENT_AMOUNT = utilityviewmodel.value;

                    // Some Bills don't flag PAYMENTS with a preceding -
                    if (SmartParseV2016.Add_Minus_Sign(utilityviewmodel))
                    {
                        utilityviewmodel.PAYMENT_AMOUNT = utilityviewmodel.value;
                    }
                    if (!SmartParseV2016.Generic_Parse_Integer(utilityviewmodel.PAYMENT_AMOUNT, utilityviewmodel))
                    {
                        return false;
                    }
                }
                else
                {
                    if (item == components.Length - 2)
                    {
                        utilityviewmodel.PAYMENT_DATE = components[item];
                    }
                    else
                    {
                        utilityviewmodel.PAYMENT_METHOD = components[item] + SmartParametersV2016.space + utilityviewmodel.PAYMENT_METHOD;
                    }
                }
                item--;// = item - 1;
            }
            return true;
        }

        private static bool Parse_SectionE0(MainViewModel ourviewmodel,
                                            UtilityViewModel utilityviewmodel,
                                            string[][] sections,
                                            string section,
                                            string sub_section,
                                            string[] lines)
        {
            utilityviewmodel.currentIndex = 0;
            if (!SmartParseV2016.Decode_Section(section, sub_section, sections, utilityviewmodel.resource_code.ToString(), utilityviewmodel))
            {
                return true;
            }

            utilityviewmodel.token = "";

            for (int line_count = Convert.ToInt32(sections[utilityviewmodel.currentIndex][4]); line_count <= Convert.ToInt32(sections[utilityviewmodel.currentIndex][5]); line_count++)
            {
                utilityviewmodel.token = lines[line_count].Trim();
                if (Previous_Balance(ourviewmodel, utilityviewmodel, line_count, lines, sections, utilityviewmodel.currentIndex))
                {
                    goto the_old_daysE0;
                }
                else
                {
                    // Didn't find the token
                    if (!string.IsNullOrEmpty(ourviewmodel.errorMessage))
                    {
                        // But found a problem
                        return false;
                    }
                }

                if (!Payment_Received(ourviewmodel,
                                            utilityviewmodel,
                                            ref line_count,
                                            sections,
                                            lines,
                                            utilityviewmodel.currentIndex))
                {
                    if (!string.IsNullOrEmpty(ourviewmodel.errorMessage))
                    {
                        return false;
                    }
                }

                if (Outstanding_Balance(ourviewmodel, utilityviewmodel, line_count, lines, sections, utilityviewmodel.currentIndex))
                {
                    goto the_old_daysE0;
                }
                else
                {
                    // Didn't find the token
                    if (!string.IsNullOrEmpty(ourviewmodel.errorMessage))
                    {
                        // But found a problem
                        return false;
                    }
                }
            the_old_daysE0:
                continue;
            }
            return true;
        }

        private static bool E_Meter_Serial_Number(UtilityViewModel utilityviewmodel)
        {
            string meter_number_lcase = "Meter number",
                    new_meter_number = "New meter number",
                    previous_meter_number = "Previous meter number";

            if (SmartParseV2016.Token_Identify(utilityviewmodel, meter_number_lcase, true) ||
                SmartParseV2016.Token_Identify(utilityviewmodel, new_meter_number, true) ||
                SmartParseV2016.Token_Identify(utilityviewmodel, previous_meter_number, true))
            {
                utilityviewmodel.METER_SERIAL_NO = Find_Meter_Serial_No_V3(utilityviewmodel);
                if (string.IsNullOrEmpty(utilityviewmodel.sparks.meter_serial_no))
                {
                    utilityviewmodel.sparks.meter_serial_no = utilityviewmodel.METER_SERIAL_NO;
                }
                return true;
            }
            return false;
        }

        private static bool E_This_Is_Fucking_Tough(bool finished, UtilityViewModel utilityviewmodel)
        {
            while (!finished)
            {
                int last_space = utilityviewmodel.working_token.LastIndexOf(SmartParametersV2016.space);
                if (last_space >= 0)
                {
                    StringBuilder sb = new StringBuilder(utilityviewmodel.working_token);
                    sb[last_space] = SmartParametersV2016.bar;
                    utilityviewmodel.working_token = sb.ToString();
                    last_space++;
                    if (last_space < sb.Length)
                    {
                        if (Char.IsDigit(sb[last_space]))
                        {
                            finished = true;
                        }
                    }
                }
                else
                {
                    break;
                }
            }
            return finished;
        }

        //private static bool E_Readings_Units(string[][] sections,
        //                                    string[] lines,
        //                                    UtilityViewModel utilityviewmodel,
        //                                    int section_index,
        //                                    string METER_SERIAL_NO,
        //                                    rf short units_band)
        //{
        //    int     dash_index,
        //            discount_index;

        //    // Try for a real dash first ... (sometimes they just don't come through)
        //    dash_index = token.IndexOf(SmartParametersV2016.dash);
        //    discount_index = token.IndexOf(SmartParametersV2016.defaultConvertToSymbol.ToString());

        //    // And she's a fucking RADIOGRAPHER not a fucking STENOGRAPHER you stupid bitch
        //    // A STENOGRAPHER records court proceedings you idiotic cow
        //    if (dash_index >= 0 &&
        //        discount_index == -1)
        //    {
        //        // Obeast -Obese
        //        // Ashbergers - Aspbergers
        //        // Terraced - Terrace
        //        // Delicatessant - Delicatessan

        //        // Work FORWARDS until we find all the Reading Dates   
        //        DateTime READINGS_PERIOD_START = SmartParametersV2016.defaultDate,
        //                 READINGS_PERIOD_END = SmartParametersV2016.defaultDate;
        //        //UNIT_CHARGES_PERIOD_START = SmartParametersV2016.defaultDate,
        //        //UNIT_CHARGES_PERIOD_END = SmartParametersV2016.defaultDate;
        //        string UNITS = "0",
        //                UNITS_RATE = "0",
        //                UNITS_COST = "0";
        //        string D_THIS_READ = "0",
        //                D_LAST_READ = "0",
        //                D_UNITS_USED = "0";
        //        string N_THIS_READ = "0",
        //                N_LAST_READ = "0",
        //                N_UNITS_USED = "0";
        //        string READ_TYPE = "",
        //                UNIT_OF_MEASURE = "";
        //        //string UNITS_TYPE = ""; Hmmm ... this was in the original but appears unused??
        //        string TEMP_DATE = "";
        //        int TEMP = 0;


        //        // BASHING,, CRASHING THUMPING is this bitch E-V-E-R quiet???????????????????
        //        char UNITS_TIME = SmartParametersV2016.daytimeUnit;    // Assume initially its D ays i.e. not E7

        //        // Now sometimes ... we don't get all of the line i.e. its split over three
        //        // So now we do a bit of jiggery-pokery - if the NEXT line contains a dash,
        //        // then we can assume its a date, but if not ... then we keep building THIS line
        //        // until the next line IS a date .. Understand?
        //        bool we_have_all_we_need = false;
        //        while (!we_have_all_we_need)
        //        {
        //            if (lines[line_count + 1].Contains(SmartParametersV2016.dash))
        //            {
        //                we_have_all_we_need = true;
        //            }
        //            else
        //            {
        //                token = token + SmartParametersV2016.space + lines[line_count + 1];
        //                line_count = line_count + 1;
        //            }
        //        }

        //        //if (token.Contains("Cost of electricity used this period") ||
        //        //        token.Contains("Cost of electricity"))
        //        //{
        //        //    //line_count = inner_index;
        //        //    goto go_back;
        //        //}

        //        TEMP_DATE = token.Substring(0, dash_index).Trim();
        //        TEMP_DATE = Fillout_Year(TEMP_DATE, SmartParametersV2016.spaceSplit, SmartParametersV2016.dash);
        //        if (!SmartParseV2016.Generic_Parse_Datetime(TEMP_DATE, rf READINGS_PERIOD_START, rf ourviewmodel.errorMessage))
        //        {
        //            return false;
        //        }
        //        string remainder_token = token.Substring(dash_index).Trim(SmartParametersV2016.dashSplit).Trim();
        //        // Now we are looking for TWO components which - eventually - are separated by a space
        //        // So we need to find the LAST SPACE on the line, so we want an IndexOf which goes BACKWARDS
        //        utilityviewmodel.working_token = remainder_token;
        //        bool finished = false;
        //        while (!finished)
        //        {
        //            if (E_This_Is_Fucking_Tough(finished, utilityviewmodel))
        //            {
        //                break;
        //            }
        //            line_count = line_count + 1;
        //            remainder_token = remainder_token + SmartParametersV2016.space + lines[line_count].Trim();
        //            utilityviewmodel.working_token = remainder_token;
        //        }
        //        string[] components = utilityviewmodel.working_token.Split(SmartParametersV2016.bar);
        //        if (components.Length >= 2)
        //        {
        //            D_LAST_READ = components[1].Trim();
        //            if (!SmartParseV2016.Generic_Parse_Integer(D_LAST_READ, rf TEMP, rf ourviewmodel.errorMessage))
        //            {
        //                return false;
        //            }
        //            READ_TYPE = components[0].Trim();  // For completeness
        //            READ_TYPE = char.ToUpper(READ_TYPE[0]) + READ_TYPE.Substring(0, 1);

        //            // Andrea LEADSOM not Andrea LEVESON you fucking moron
        //            int outer_index = line_count + 1;
        //            while (outer_index <= Convert.ToInt32(sections[section_index][5]))
        //            {
        //                token = lines[outer_index].Trim();
        //                if (token.Contains("Cost of electricity used this period") ||
        //                    token.Contains("Cost of electricity") ||
        //                    token.Contains("Standing Charge"))
        //                {
        //                    //line_count = inner_index;
        //                    goto go_back;
        //                }
        //                dash_index = token.IndexOf(SmartParametersV2016.dash);
        //                if (dash_index >= 0)
        //                {
        //                    TEMP_DATE = token.Substring(0, dash_index).Trim();
        //                    TEMP_DATE = Fillout_Year(TEMP_DATE, SmartParametersV2016.spaceSplit, SmartParametersV2016.dash);
        //                    if (!SmartParseV2016.Generic_Parse_Datetime(TEMP_DATE, rf READINGS_PERIOD_END, rf ourviewmodel.errorMessage))
        //                    {
        //                        return false;
        //                    }
        //                    remainder_token = token.Substring(dash_index).Trim(SmartParametersV2016.dashSplit).Trim();
        //                    utilityviewmodel.working_token = remainder_token;
        //                    finished = false;
        //                    while (!finished)
        //                    {
        //                        if (E_This_Is_Fucking_Tough(finished, utilityviewmodel))
        //                        {
        //                            break;
        //                        }
        //                        outer_index = outer_index + 1;
        //                        remainder_token = remainder_token + SmartParametersV2016.space + lines[outer_index].Trim();
        //                        utilityviewmodel.working_token = remainder_token;
        //                    }
        //                    components = utilityviewmodel.working_token.Split(SmartParametersV2016.bar);
        //                    if (components.Length >= 2)
        //                    {
        //                        D_THIS_READ = components[1].Trim();
        //                        if (!SmartParseV2016.Generic_Parse_Integer(D_THIS_READ, rf TEMP, rf ourviewmodel.errorMessage))
        //                        {
        //                            return false;
        //                        }
        //                        READ_TYPE = components[0].Trim();
        //                        READ_TYPE = char.ToUpper(READ_TYPE[0]) + READ_TYPE.Substring(0, 1);
        //                    }
        //                }
        //                if (token.Contains("kWh used over"))
        //                {
        //                    UNIT_OF_MEASURE = "kWh";
        //                    // Both D_LAST_READ and D_THIS_READ have already been tested as Ints
        //                    D_UNITS_USED = (Convert.ToInt32(D_THIS_READ) - Convert.ToInt32(D_LAST_READ)).ToString();
        //                    if (SmartParseV2016.Add_Readings_Test(SmartParametersV2016.defaultDate,
        //                                                            READINGS_PERIOD_START,
        //                                                            READINGS_PERIOD_END,
        //                                                            D_LAST_READ,
        //                                                            D_THIS_READ,
        //                                                            UNIT_OF_MEASURE))
        //                    {
        //                        SmartUtilityV2022.E_ReadingsList_Add(utilityviewmodel.e_readings_outList,
        //                                                                    utilityviewmodel.cubeface_code,
        //                                                                    utilityviewmodel.supplier_code,
        //                                                                    utilityviewmodel.brand_code,
        //                                                                    utilityviewmodel.ACCOUNT_NO,
        //                                                                    utilityviewmodel.created,
        //                                                                    utilityviewmodel.STATEMENT_ID,
        //                                                                    utilityviewmodel.BILL_DATE,
        //                                                                    utilityviewmodel.sparks.MPAN,
        //                                                                    READINGS_PERIOD_START.ToString(),
        //                                                                    READINGS_PERIOD_END.ToString(),
        //                                                                    METER_SERIAL_NO,
        //                                                                    READ_TYPE,
        //                                                                    D_LAST_READ,
        //                                                                    D_THIS_READ,
        //                                                                    D_UNITS_USED,
        //                                                                    N_LAST_READ,
        //                                                                    N_THIS_READ,
        //                                                                    N_UNITS_USED,
        //                                                                    UNIT_OF_MEASURE);
        //                    }

        //                    line_count = outer_index + 1;    // <= Point at the next one to process

        //                    int inner_index = line_count;
        //                    while (inner_index <= Convert.ToInt32(sections[section_index][5]))
        //                    {
        //                        token = lines[inner_index].Trim();
        //                        if (!Kwh_Used(utilityviewmodel,
        //                                        rf units_band,
        //                                        rf UNITS,
        //                                        rf UNIT_OF_MEASURE,
        //                                        rf UNITS_RATE,
        //                                        rf UNITS_COST,
        //                                        UNITS_TIME,
        //                                        READINGS_PERIOD_START,
        //                                        READINGS_PERIOD_END))
        //                        {
        //                            return false;
        //                        }
        //                        inner_index = inner_index + 1;
        //                    }
        //                    //outer_index = inner_index;
        //                }
        //                outer_index = outer_index + 1;
        //            }
        //        }
        //        go_back:
        //        return true;
        //    }
        //    return false;
        //}


        private static bool E_Readings_Units_New(UtilityViewModel utilityviewmodel,
                                            ref int line_count,
                                            string[][] sections,
                                            string[] lines,
                                            int section_index,
                                            string METER_SERIAL_NO)
        {
            int dash_index,
                    discount_index;

            // Try for a real dash first ... (sometimes they just don't come through)
            dash_index = utilityviewmodel.token.IndexOf(SmartParametersV2016.dash);
            discount_index = utilityviewmodel.token.IndexOf(SmartParametersV2016.defaultConvertToSymbol);

            // And she's a fucking RADIOGRAPHER not a fucking STENOGRAPHER you stupid bitch
            // A STENOGRAPHER records court proceedings you idiotic cow
            if (dash_index >= 0 &&
                discount_index == -1)
            {
                // Obeast -Obese
                // Ashbergers - Aspbergers
                // Terraced - Terrace
                // Delicatessant - Delicatessan

                // Work FORWARDS until we find all the Reading Dates   
                //DateTime READINGS_PERIOD_START = SmartParametersV2016.defaultDate,
                //         READINGS_PERIOD_END = SmartParametersV2016.defaultDate;
                //UNIT_CHARGES_PERIOD_START = SmartParametersV2016.defaultDate,
                //UNIT_CHARGES_PERIOD_END = SmartParametersV2016.defaultDate;
                //string UNITS = "0",
                //        UNITS_RATE = "0",
                //        UNITS_COST = "0",
                //        UNIT_OF_MEASURE = "";
                string D_THIS_READ = "0",
                        D_LAST_READ,// = "0",
                        D_UNITS_USED;// = "0";
                string N_THIS_READ = "0",
                        N_LAST_READ = "0",
                        N_UNITS_USED = "0";
                string READ_TYPE;
                //string UNITS_TYPE = ""; Hmmm ... this was in the original but appears unused??
                string TEMP_DATE;
                int TEMP;

                //string First = "First",
                //        Next = "Next";

                // BASHING,, CRASHING THUMPING is this bitch E-V-E-R quiet???????????????????
                //char UNITS_TIME = SmartParametersV2016.daytimeUnit;    // Assume initially its D ays i.e. not E7

                // Now sometimes ... we don't get all of the line i.e. its split over three
                // So now we do a bit of jiggery-pokery - if the NEXT line contains a dash,
                // then we can assume its a date, but if not ... then we keep building THIS line
                // until the next line IS a date .. Understand?
                bool we_have_all_we_need = false;
                while (!we_have_all_we_need)
                {
                    if (lines[line_count + 1].Contains(SmartParametersV2016.dash))
                    {
                        we_have_all_we_need = true;
                    }
                    else
                    {
                        utilityviewmodel.token = utilityviewmodel.token + SmartParametersV2016.space + lines[line_count + 1];
                        line_count++;
                    }
                }

                TEMP_DATE = utilityviewmodel.token.Substring(0, dash_index).Trim();
                TEMP_DATE = Fillout_Year(TEMP_DATE, SmartParametersV2016.spaceSplit, SmartParametersV2016.dash);
                if (!SmartParseV2016.Generic_Parse_Datetime(TEMP_DATE, utilityviewmodel))
                {
                    return false;
                }
                utilityviewmodel.READINGS_PERIOD_START = TEMP_DATE;

                string remainder_token = utilityviewmodel.token.Substring(dash_index).Trim(SmartParametersV2016.dashSplit).Trim();
                // Now we are looking for TWO components which - eventually - are separated by a space
                // So we need to find the LAST SPACE on the line, so we want an IndexOf which goes BACKWARDS
                utilityviewmodel.working_token = remainder_token;
                bool finished = false;
                while (!finished)
                {
                    if (E_This_Is_Fucking_Tough(finished, utilityviewmodel))
                    {
                        break;
                    }
                    line_count++;
                    remainder_token = remainder_token + SmartParametersV2016.space + lines[line_count].Trim();
                    utilityviewmodel.working_token = remainder_token;
                }
                string[] components = utilityviewmodel.working_token.Split(SmartParametersV2016.bar);
                if (components.Length >= 2)
                {
                    D_LAST_READ = components[1].Trim();
                    if (!SmartParseV2016.Generic_Parse_Integer(D_LAST_READ, utilityviewmodel))
                    {
                        return false;
                    }
                    else
                    {
                        TEMP = utilityviewmodel.genericTransactionValue;
                    }
                    READ_TYPE = components[0].Trim();  // For completeness
                    READ_TYPE = char.ToUpper(READ_TYPE[0]) + READ_TYPE.Substring(0, 1);
                    // Andrea LEADSOM not Andrea LEVESON you fucking moron
                    int outer_index = line_count + 1;
                    while (outer_index <= Convert.ToInt32(sections[section_index][5]))
                    {
                        utilityviewmodel.token = lines[outer_index].Trim();
                        if (utilityviewmodel.token.Contains("Cost of electricity used this period") ||
                            utilityviewmodel.token.Contains("Cost of electricity") ||
                            utilityviewmodel.token.Contains("Standing Charge") ||
                            utilityviewmodel.token.Contains("Standing charge"))
                        {
                            goto go_back;
                        }
                        dash_index = utilityviewmodel.token.IndexOf(SmartParametersV2016.dash);
                        if (dash_index >= 0)
                        {
                            TEMP_DATE = utilityviewmodel.token.Substring(0, dash_index).Trim();
                            TEMP_DATE = Fillout_Year(TEMP_DATE, SmartParametersV2016.spaceSplit, SmartParametersV2016.dash);
                            if (!SmartParseV2016.Generic_Parse_Datetime(TEMP_DATE, utilityviewmodel))
                            {
                                return false;
                            }
                            utilityviewmodel.READINGS_PERIOD_END = TEMP_DATE;
                            remainder_token = utilityviewmodel.token.Substring(dash_index).Trim(SmartParametersV2016.dashSplit).Trim();
                            utilityviewmodel.working_token = remainder_token;
                            finished = false;
                            while (!finished)
                            {
                                if (E_This_Is_Fucking_Tough(finished, utilityviewmodel))
                                {
                                    break;
                                }
                                outer_index++;
                                remainder_token = remainder_token + SmartParametersV2016.space + lines[outer_index].Trim();
                                utilityviewmodel.working_token = remainder_token;
                            }
                            components = utilityviewmodel.working_token.Split(SmartParametersV2016.bar);
                            if (components.Length >= 2)
                            {
                                D_THIS_READ = components[1].Trim();
                                if (!SmartParseV2016.Generic_Parse_Integer(D_THIS_READ, utilityviewmodel))
                                {
                                    return false;
                                }
                                else
                                {
                                    TEMP = utilityviewmodel.genericTransactionValue;
                                }
                                READ_TYPE = components[0].Trim();
                                READ_TYPE = char.ToUpper(READ_TYPE[0]) + READ_TYPE.Substring(0, 1);
                            }
                        }
                        if (utilityviewmodel.token.Contains("kWh used over"))
                        {
                            string UNIT_OF_MEASURE = "kWh";
                            // Both D_LAST_READ and D_THIS_READ have already been tested as Ints
                            D_UNITS_USED = (Convert.ToInt32(D_THIS_READ) - Convert.ToInt32(D_LAST_READ)).ToString();
                            if (SmartParseV2016.Add_Readings_Test(SmartParametersV2016.defaultDate,
                                                                    Convert.ToDateTime(utilityviewmodel.READINGS_PERIOD_START),
                                                                    Convert.ToDateTime(utilityviewmodel.READINGS_PERIOD_END),
                                                                    D_LAST_READ,
                                                                    D_THIS_READ,
                                                                    UNIT_OF_MEASURE))
                            {
                                SmartUtilityV2022.E_ReadingsList_Add(utilityviewmodel,

                                                                            SmartParametersV2016.Utility,
                                                                            utilityviewmodel.supplier_code,
                                                                            utilityviewmodel.brand_code,
                                                                            utilityviewmodel.ACCOUNT_NO,
                                                                            utilityviewmodel.CREATED,
                                                                            utilityviewmodel.STATEMENT_ID,
                                                                            utilityviewmodel.BILL_DATE,
                                                                            utilityviewmodel.sparks.MPAN,
                                                                            utilityviewmodel.READINGS_PERIOD_START,
                                                                            utilityviewmodel.READINGS_PERIOD_END,
                                                                            METER_SERIAL_NO,
                                                                            READ_TYPE,
                                                                            D_LAST_READ,
                                                                            D_THIS_READ,
                                                                            D_UNITS_USED,
                                                                            N_LAST_READ,
                                                                            N_THIS_READ,
                                                                            N_UNITS_USED,
                                                                            UNIT_OF_MEASURE);
                            }
                            line_count = outer_index;
                            outer_index = Convert.ToInt32(sections[section_index][5]);
                        }
                        outer_index++;
                    }
                }
            go_back:
                return true;
            }
            return false;
        }

        private static bool Kwh_Used(UtilityViewModel utilityviewmodel,
                                        char UNITS_TIME)
        {
            string First = "First",
                        Next = "Next";

            if (utilityviewmodel.token.Contains("kWh used at") ||
                                    utilityviewmodel.token.Contains("kWh x"))
            {
                string UNITS_TYPE = utilityviewmodel.token;
                int curr = UNITS_TYPE.IndexOf(utilityviewmodel.bill_currency_symbol);
                if (curr >= 0)
                {
                    UNITS_TYPE = UNITS_TYPE.Substring(0, curr).Trim();
                }
                utilityviewmodel.token = utilityviewmodel.token.Replace("used at", "x").Trim();
                if (utilityviewmodel.token.Contains("Cost of"))
                {
                    utilityviewmodel.token = utilityviewmodel.token.Replace("Cost of", "").Trim();
                }
                if (utilityviewmodel.token.Contains("electricity"))
                {
                    utilityviewmodel.token = utilityviewmodel.token.Replace("electricity", "").Trim();
                }
                utilityviewmodel.token = utilityviewmodel.token.Replace("first", First);
                utilityviewmodel.token = utilityviewmodel.token.Replace("next", Next);
                if (utilityviewmodel.token.IndexOf(First) == -1)
                {
                    if (utilityviewmodel.token.IndexOf(Next) == -1)
                    {
                        utilityviewmodel.token = "Filler " + utilityviewmodel.token;
                    }
                }
                if (!E_Components(utilityviewmodel))
                {
                    return false;
                }
                SmartUtilityV2022.E_Unit_ChargesList_Add(utilityviewmodel,
                                                            SmartParametersV2016.Utility,
                                                            utilityviewmodel.supplier_code,
                                                            utilityviewmodel.brand_code,
                                                            utilityviewmodel.ACCOUNT_NO,
                                                            utilityviewmodel.CREATED,
                                                            utilityviewmodel.STATEMENT_ID,
                                                            utilityviewmodel.BILL_DATE,
                                                            utilityviewmodel.sparks.MPAN,
                                                            utilityviewmodel.READINGS_PERIOD_START,
                                                            utilityviewmodel.READINGS_PERIOD_END,
                                                            UNITS_TIME,
                                                            utilityviewmodel.units_band.ToString(),
                                                            UNITS_TYPE,
                                                            utilityviewmodel.UNITS,
                                                            utilityviewmodel.UNITS_RATE,
                                                            utilityviewmodel.UNIT_OF_MEASURE,
                                                            utilityviewmodel.UNITS_COST);
                return true;
            }
            return false;
        }

        private static bool E_Components(UtilityViewModel utilityviewmodel)
        {
            string[] components = utilityviewmodel.token.Split(SmartParametersV2016.spaceSplit);
            int component = 0;
            while (component < components.Length)
            {
                switch (component)
                {
                    case 0:
                        // first or next 
                        utilityviewmodel.units_band = (short)(utilityviewmodel.units_band + 1);
                        break;
                    case 1:
                        // Units
                        utilityviewmodel.UNITS = components[component].Replace("(", "").Trim();
                        if (!SmartParseV2016.Generic_Parse_Integer(utilityviewmodel.UNITS, utilityviewmodel))
                        {
                            return false;
                        }
                        break;
                    case 2:
                        // kWh
                        utilityviewmodel.UNIT_OF_MEASURE = components[component];
                        break;
                    case 3:
                        // x times
                        break;
                    case 4:
                        // Unit Rate
                        if (components[component].IndexOf(utilityviewmodel.bill_denomination_symbol.ToString()) >= 0)
                        {
                            utilityviewmodel.UNITS_RATE = components[component].Replace(utilityviewmodel.bill_denomination_symbol.ToString(), "").Trim();
                            utilityviewmodel.UNITS_RATE = utilityviewmodel.UNITS_RATE.Replace(")", "").Trim();
                            if (!SmartParseV2016.Generic_Parse_Decimal(utilityviewmodel.UNITS_RATE, utilityviewmodel))
                            {
                                return false;
                            }
                        }
                        break;
                    case 5:
                        // Cost
                        if (!SmartParseV2016.Currency_Strip(utilityviewmodel.bill_currency_symbol, utilityviewmodel.bill_currency_separator, utilityviewmodel.bill_thousands_separator, components[component].Trim(), utilityviewmodel))
                        {
                            return false;
                        }
                        else
                        {
                            utilityviewmodel.UNITS_COST = utilityviewmodel.value;
                        }
                        if (!SmartParseV2016.Generic_Parse_Integer(utilityviewmodel.UNITS_COST, utilityviewmodel))
                        {
                            return false;
                        }
                        break;
                    default:
                        break;
                }
                component++;
            }
            return true;
        }

        private static bool E_Standing_Charges(UtilityViewModel utilityviewmodel,
                                            ref int line_count,
                                            string[][] sections,
                                            string[] lines,
                                            int section_index,
                                            short charges_item)
        {
            string Standing_Charge = "Standing Charge",
                    Standing_charge = "Standing charge";

            if (SmartParseV2016.Token_Identify(utilityviewmodel, Standing_Charge, false) ||
                SmartParseV2016.Token_Identify(utilityviewmodel, Standing_charge, false))
            {
                DateTime PERIOD_START = SmartParametersV2016.defaultDate,
                            PERIOD_END = SmartParametersV2016.defaultDate;

                short CHARGES_DAYS = 0;
                string CHARGES_TYPE,// = "",
                        CHARGES_COST = "0",
                        STANDING_CHARGE = "0";  // There IS no 'STANDING_CHARGE' for British Gas
                                                // There is now ... these cunts can never make up their fucking minds

                string TEMP_DATE;// = "";
                int dash_index;

                int outer_index = line_count + 1;
                while (outer_index <= Convert.ToInt32(sections[section_index][5]))
                {
                    utilityviewmodel.token = lines[outer_index].Trim();
                    dash_index = utilityviewmodel.token.IndexOf(SmartParametersV2016.dash);
                    if (dash_index >= 0)
                    {
                        TEMP_DATE = utilityviewmodel.token.Substring(0, dash_index).Trim();
                        TEMP_DATE = Fillout_Year(TEMP_DATE, SmartParametersV2016.spaceSplit, SmartParametersV2016.dash);
                        if (!SmartParseV2016.Generic_Parse_Datetime(TEMP_DATE, utilityviewmodel))
                        {
                            return false;
                        }
                        else
                        {
                            PERIOD_START = utilityviewmodel.genericTargetDate;
                        }
                        TEMP_DATE = utilityviewmodel.token.Substring(dash_index).Trim(SmartParametersV2016.dashSplit).Trim();
                        TEMP_DATE = TEMP_DATE.Trim();
                        TEMP_DATE = Fillout_Year(TEMP_DATE, SmartParametersV2016.spaceSplit, SmartParametersV2016.dash);
                        if (!SmartParseV2016.Generic_Parse_Datetime(TEMP_DATE, utilityviewmodel))
                        {
                            return false;
                        }
                        else
                        {
                            PERIOD_END = utilityviewmodel.genericTargetDate;
                        }
                    }
                    if (utilityviewmodel.token.Contains("days at"))
                    {
                        CHARGES_TYPE = utilityviewmodel.token;
                        utilityviewmodel.token = utilityviewmodel.token.Replace("days at", "").Trim();
                        utilityviewmodel.token = utilityviewmodel.token.Replace("per day", "");
                        utilityviewmodel.token = utilityviewmodel.token.Replace("/day", "");
                        utilityviewmodel.token = SmartParseV2016.Remove_Double_Spaces_V3(utilityviewmodel.token);
                        utilityviewmodel.token = utilityviewmodel.token.Trim();

                        int temp;// = 0;

                        string[] components = utilityviewmodel.token.Split(SmartParametersV2016.spaceSplit);
                        int component_count = 0;
                        while (component_count < components.Length)
                        {
                            switch (component_count)
                            {
                                case 0:
                                    string charges_days = components[component_count].Trim();
                                    charges_days = charges_days.Replace(".00", "");   // No of days is always a whole number is it not?
                                                                                      // Sometimes ... days has a decimal point e.g. 99.00
                                                                                      // Sometimes Gas values have these embedded
                                                                                      //UNITS = UNITS.Replace("comma", "");
                                    if (!SmartParseV2016.Generic_Parse_Integer(charges_days, utilityviewmodel))
                                    {
                                        return false; // goto quit_C2;
                                    }
                                    else
                                    {
                                        temp = utilityviewmodel.genericTransactionValue;
                                    }
                                    CHARGES_DAYS = Convert.ToInt16(temp);
                                    break;
                                case 1:
                                    // Charges Rate
                                    components[component_count] = components[component_count].Replace(SmartParametersV2016.decimalPoint + utilityviewmodel.bill_denomination_symbol, utilityviewmodel.bill_denomination_symbol.ToString());
                                    STANDING_CHARGE = components[component_count].Replace(utilityviewmodel.bill_denomination_symbol.ToString(), "").Trim();
                                    //decimal temp_decimal = 0.0M;
                                    if (!SmartParseV2016.Generic_Parse_Decimal(STANDING_CHARGE, utilityviewmodel))
                                    {
                                        return false;
                                    }
                                    //else
                                    //{
                                    //    temp_decimal = utilityviewmodel.genericDecimalValue;
                                    //}
                                    break;
                                case 2:
                                    // Charges Cost
                                    if (!SmartParseV2016.Currency_Strip(utilityviewmodel.bill_currency_symbol, utilityviewmodel.bill_currency_separator, utilityviewmodel.bill_thousands_separator, components[component_count], utilityviewmodel))
                                    {
                                        return false;
                                    }
                                    else
                                    {
                                        CHARGES_COST = utilityviewmodel.value;
                                    }
                                    if (!SmartParseV2016.Generic_Parse_Integer(CHARGES_COST, utilityviewmodel))
                                    {
                                        return false;
                                    }
                                    //else
                                    //{
                                    //    temp = utilityviewmodel.genericTransactionValue;
                                    //}
                                    break;
                                default:
                                    break;
                            }
                            component_count++;
                        }

                        // This does both Electricity and Gas ....
                        // Add in a Standing Charge record WHEN WE GET THEM
                        // DON'T ADD IN STANDING CHARGE RECORDS WHEN THERE ARE NONE!!!!!
                        switch (utilityviewmodel.resource_code)
                        {
                            case SmartParametersV2016.Electricity:
                                SmartUtilityV2022.E_Standing_ChargesList_Add(utilityviewmodel,

                                                                            SmartParametersV2016.Utility,
                                                                            utilityviewmodel.supplier_code,
                                                                            utilityviewmodel.brand_code,
                                                                            utilityviewmodel.ACCOUNT_NO,
                                                                            utilityviewmodel.CREATED,
                                                                            utilityviewmodel.STATEMENT_ID,
                                                                            utilityviewmodel.BILL_DATE,
                                                                            utilityviewmodel.sparks.MPAN,
                                                                            PERIOD_START.ToString(),
                                                                            PERIOD_END.ToString(),
                                                                            charges_item.ToString(),
                                                                            CHARGES_TYPE,
                                                                            STANDING_CHARGE,
                                                                            CHARGES_DAYS.ToString(),
                                                                            CHARGES_COST);
                                break;
                            default:
                                break;
                        }
                        line_count = outer_index;
                        goto go_back;
                    }
                    outer_index++;
                }
            go_back:
                return true;
            }
            return false;
        }

        private static bool Parse_SectionH1(MainViewModel ourviewmodel,
                                            UtilityViewModel utilityviewmodel,
                                            string[][] sections,
                                            string section,
                                            string sub_section,
                                            string[] lines)
        {
            utilityviewmodel.currentIndex = 0;
            if (!SmartParseV2016.Decode_Section(section, sub_section, sections, utilityviewmodel.resource_code.ToString(), utilityviewmodel))
            {
                return true;
            }

            int version = 3;
            utilityviewmodel.token = "";

            utilityviewmodel.units_band = 0;

            utilityviewmodel.READINGS_PERIOD_START = SmartParametersV2016.defaultDates;
            utilityviewmodel.READINGS_PERIOD_END = SmartParametersV2016.defaultDates;

            utilityviewmodel.UNITS = "0";
            utilityviewmodel.UNITS_RATE = "0";
            utilityviewmodel.UNITS_COST = "0";
            utilityviewmodel.UNIT_OF_MEASURE = "";
            // BASHING,, CRASHING THUMPING is this bitch E-V-E-R quiet???????????????????
            char UNITS_TIME = SmartParametersV2016.daytimeUnit;    // Assume initially its D ays i.e. not E7

            // There is now ... these cunts can never make up their fucking minds

            utilityviewmodel.METER_SERIAL_NO = utilityviewmodel.smell.meter_serial_no;

            switch (version)
            {
                case 1:
                    // WE DON'T DO VERSION 1 ANYMORE
                    // (Versions2 and 3 are hard enough ...)
                    //parse_page2_v1(page,
                    //                strText,
                    //                utilityviewmodel,
                    //                BILL_PERIOD_END,
                    //                SUPPLY_VAT);
                    break;
                case 2:
                case 3:
                    // Separate just to be on the safe side
                    for (int line_count = Convert.ToInt32(sections[utilityviewmodel.currentIndex][4]); line_count <= Convert.ToInt32(sections[utilityviewmodel.currentIndex][5]); line_count++)
                    {
                        utilityviewmodel.token = lines[line_count].Trim();

                        if (string.IsNullOrEmpty(utilityviewmodel.METER_SERIAL_NO))
                        {
                            if (E_Meter_Serial_Number(utilityviewmodel))
                            {
                                goto the_old_daysH1;
                            }
                            else
                            {
                                if (!string.IsNullOrEmpty(ourviewmodel.errorMessage))
                                {
                                    return false;
                                }
                            }
                        }

                        if (E_Readings_Units_New(utilityviewmodel,
                                            ref line_count,
                                            sections,
                                            lines,
                                            utilityviewmodel.currentIndex,
                                            utilityviewmodel.METER_SERIAL_NO))
                        {
                            goto the_old_daysH1;
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(ourviewmodel.errorMessage))
                            {
                                return false;
                            }
                        }

                        if (Kwh_Used(utilityviewmodel,
                                    UNITS_TIME))
                        {
                            goto the_old_daysH1;
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(ourviewmodel.errorMessage))
                            {
                                return false;
                            }
                        }

                        if (E_Standing_Charges(utilityviewmodel,
                                            ref line_count,
                                            sections,
                                            lines,
                                            utilityviewmodel.currentIndex,
                                            utilityviewmodel.units_band))
                        {
                            goto the_old_daysH1;
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(ourviewmodel.errorMessage))
                            {
                                return false;
                            }
                        }

                    the_old_daysH1:
                        continue;
                    }
                    break;
                default:
                    break;
            }
            return true;
        }

        // This is why I cannot work, why this project is years late
        // Mrs Meddling Fucking Farting about has started dropping plastic boxes FOR THE UMPTEENTH TIME
        // in the kitchen.  She is SUCH AS FUCKING NOISY BITCH
        //                  ==================================

        private static bool G_Meter_Serial_Number(UtilityViewModel utilityviewmodel)
        {
            string meter_number_lcase = "Meter number",
                    new_meter_number = "New meter number",
                    previous_meter_number = "Previous meter number";

            if (SmartParseV2016.Token_Identify(utilityviewmodel, meter_number_lcase, true) ||
                SmartParseV2016.Token_Identify(utilityviewmodel, new_meter_number, true) ||
                SmartParseV2016.Token_Identify(utilityviewmodel, previous_meter_number, true))
            {
                utilityviewmodel.METER_SERIAL_NO = Find_Meter_Serial_No_V3(utilityviewmodel);
                if (string.IsNullOrEmpty(utilityviewmodel.smell.meter_serial_no))
                {
                    utilityviewmodel.smell.meter_serial_no = utilityviewmodel.METER_SERIAL_NO;
                }
                return true;
            }
            return false;
        }

        private static bool G_This_Is_Fucking_Tough(bool finished, UtilityViewModel utilityviewmodel)
        {
            while (!finished)
            {
                int last_space = utilityviewmodel.working_token.LastIndexOf(SmartParametersV2016.space);
                if (last_space >= 0)
                {
                    StringBuilder sb = new StringBuilder(utilityviewmodel.working_token);
                    sb[last_space] = SmartParametersV2016.bar;
                    utilityviewmodel.working_token = sb.ToString();
                    last_space++;
                    if (last_space < sb.Length)
                    {
                        if (Char.IsDigit(sb[last_space]))
                        {
                            finished = true;
                        }
                    }
                }
                else
                {
                    break;
                }
            }
            return finished;
        }

        private static bool G_Readings_Units(UtilityViewModel utilityviewmodel,
                                            ref int line_count,
                                            string[][] sections,
                                            string[] lines,
                                            int section_index,
                                            string METER_SERIAL_NO)
        {
            int dash_index,
                    discount_index;

            // Try for a real dash first ... (sometimes they just don't come through)
            dash_index = utilityviewmodel.token.IndexOf(SmartParametersV2016.dash);
            discount_index = utilityviewmodel.token.IndexOf(SmartParametersV2016.defaultConvertToSymbol);

            if (dash_index >= 0 &&
                discount_index == -1)
            {
                utilityviewmodel.READINGS_PERIOD_START = SmartParametersV2016.defaultDates;
                utilityviewmodel.READINGS_PERIOD_END = SmartParametersV2016.defaultDates;
                utilityviewmodel.READ_TYPE = "";
                string TEMP_DATE;// = "";

                // Now sometimes ... we don't get all of the line i.e. its split over three
                // So now we do a bit of jiggery-pokery - if the NEXT line contains a dash,
                // then we can assume its a date, but if not ... then we keep building THIS line
                // until the next line IS a date .. Understand?
                bool we_have_all_we_need = false;
                while (!we_have_all_we_need)
                {
                    if (lines[line_count + 1].Contains(SmartParametersV2016.dash))
                    {
                        we_have_all_we_need = true;
                    }
                    else
                    {
                        utilityviewmodel.token = utilityviewmodel.token + SmartParametersV2016.space + lines[line_count + 1];
                        line_count++;
                    }
                }

                TEMP_DATE = utilityviewmodel.token.Substring(0, dash_index).Trim();
                TEMP_DATE = Fillout_Year(TEMP_DATE, SmartParametersV2016.spaceSplit, SmartParametersV2016.dash);
                if (!SmartParseV2016.Generic_Parse_Datetime(TEMP_DATE, utilityviewmodel))
                {
                    return false;
                }
                else
                {
                    utilityviewmodel.READINGS_PERIOD_START = TEMP_DATE;
                }
                string remainder_token = utilityviewmodel.token.Substring(dash_index).Trim(SmartParametersV2016.dashSplit).Trim();
                // Now we are looking for TWO components which - eventually - are separated by a space
                // So we need to find the LAST SPACE on the line, so we want an IndexOF which goes BACKWARDS
                utilityviewmodel.working_token = remainder_token;
                bool finished = false;
                while (!finished)
                {
                    if (G_This_Is_Fucking_Tough(finished, utilityviewmodel))
                    {
                        break;
                    }
                    line_count++;
                    remainder_token = remainder_token + SmartParametersV2016.space + lines[line_count].Trim();
                    utilityviewmodel.working_token = remainder_token;
                }
                string[] components = utilityviewmodel.working_token.Split(SmartParametersV2016.bar);
                if (components.Length >= 2)
                {
                    string D_LAST_READ = components[1].Trim();
                    if (!SmartParseV2016.Generic_Parse_Integer(D_LAST_READ, utilityviewmodel))
                    {
                        return false;
                    }
                    D_LAST_READ = utilityviewmodel.genericTransactionValue.ToString();
                    utilityviewmodel.READ_TYPE = components[0].Trim();  // For completeness
                    utilityviewmodel.READ_TYPE = char.ToUpper(utilityviewmodel.READ_TYPE[0]) + utilityviewmodel.READ_TYPE.Substring(0, 1);
                    // Andrea LEADSOM not Andrea LEVESON you fucking moron
                    utilityviewmodel.outer_index = line_count + 1;
                    bool go_back_flag = false;
                    if (!G_Readings_Units_Outer_Index(utilityviewmodel,
                                                        ref line_count,
                                                        sections,
                                                        section_index,
                                                        lines,
                                                        METER_SERIAL_NO,
                                                        D_LAST_READ))
                    {
                        return false;
                    }
                    else
                    {
                        if (go_back_flag)
                        {
                            goto go_back;
                        }
                    }
                }
            go_back:
                return true;
            }
            return false;
        }

        private static bool G_Readings_Units_Outer_Index(UtilityViewModel utilityviewmodel,
                                                        ref int line_count,
                                                        string[][] sections,
                                                        int section_index,
                                                        string[] lines,
                                                        string METER_SERIAL_NO,
                                                        string D_LAST_READ)
        {
            string D_THIS_READ = "0",
                        D_UNITS_USED_M3 = "0",
                        D_UNITS_USED_KWH = "0";
            string READ_TYPE = "",
                        UNIT_OF_MEASURE = "";
            string unit_calorific_value_for_this_period = "Unit calorific value for this period";

            utilityviewmodel.temp_decimal = 0.0M;         // Ditto above, but we are CLEVER!

            while (utilityviewmodel.outer_index <= Convert.ToInt32(sections[section_index][5]))
            {
                utilityviewmodel.token = lines[utilityviewmodel.outer_index].Trim();
                int dash_index = utilityviewmodel.token.IndexOf(SmartParametersV2016.dash, 0);
                if (dash_index >= 0)
                {
                    string TEMP_DATE = utilityviewmodel.token.Substring(0, dash_index).Trim();
                    TEMP_DATE = Fillout_Year(TEMP_DATE, SmartParametersV2016.spaceSplit, SmartParametersV2016.dash);
                    if (!SmartParseV2016.Generic_Parse_Datetime(TEMP_DATE, utilityviewmodel))
                    {
                        return false;
                    }
                    utilityviewmodel.READINGS_PERIOD_END = TEMP_DATE;
                    string remainder_token = utilityviewmodel.token.Substring(dash_index).Trim(SmartParametersV2016.dashSplit).Trim();
                    utilityviewmodel.working_token = remainder_token;
                    bool finished = false;
                    while (!finished)
                    {
                        if (G_This_Is_Fucking_Tough(finished, utilityviewmodel))
                        {
                            break;
                        }
                        utilityviewmodel.outer_index++;
                        remainder_token = remainder_token + SmartParametersV2016.space + lines[utilityviewmodel.outer_index].Trim();
                        utilityviewmodel.working_token = remainder_token;
                        //if (!g_this_is_fucking_tough(finished, rf working_token))
                        //{
                        //    outer_index = outer_index + 1;
                        //    remainder_token = remainder_token + SmartParametersV2016.space + lines[outer_index].Trim();
                        //    working_token = remainder_token;
                        //}
                        //else
                        //{
                        //    break;
                        //}
                    }
                    string[] components = utilityviewmodel.working_token.Split(SmartParametersV2016.bar);
                    if (components.Length >= 2)
                    {
                        D_THIS_READ = components[1].Trim();
                        if (!SmartParseV2016.Generic_Parse_Integer(D_THIS_READ, utilityviewmodel))
                        {
                            return false;
                        }
                        D_THIS_READ = utilityviewmodel.genericTransactionValue.ToString();
                        READ_TYPE = components[0].Trim();
                        READ_TYPE = char.ToUpper(READ_TYPE[0]) + READ_TYPE.Substring(0, 1);
                    }
                }

                if (utilityviewmodel.token.Contains("metric units used over"))
                {
                    utilityviewmodel.token = utilityviewmodel.token.Replace("metric units used over", "").Trim();
                    utilityviewmodel.token = utilityviewmodel.token.Replace("=", "").Trim();
                    utilityviewmodel.token = utilityviewmodel.token.Replace(SmartParametersV2016.space, SmartParametersV2016.bar.ToString());
                    string[] components = utilityviewmodel.token.Split(SmartParametersV2016.bar);
                    if (components.Length > 0)
                    {
                        D_UNITS_USED_M3 = components[0].Trim();
                        if (!SmartParseV2016.Generic_Parse_Decimal(D_UNITS_USED_M3, utilityviewmodel))
                        {
                            return false;
                        }
                        else
                        {
                            utilityviewmodel.temp_decimal = utilityviewmodel.genericDecimalValue;
                        }
                        UNIT_OF_MEASURE = "m3";
                    }
                }
                if ((utilityviewmodel.token.Contains("Estimated units used over")) ||
                    utilityviewmodel.token.Contains("Actual units used over"))
                {
                    utilityviewmodel.token = utilityviewmodel.token.Replace("Estimated units used over", "").Trim();
                    utilityviewmodel.token = utilityviewmodel.token.Replace("Actual units used over", "").Trim();
                    utilityviewmodel.token = utilityviewmodel.token.Replace(SmartParametersV2016.space, SmartParametersV2016.bar.ToString());
                    string[] components = utilityviewmodel.token.Split(SmartParametersV2016.bar);
                    if (components.Length == 3)
                    {
                        D_UNITS_USED_M3 = components[2].Trim();
                        if (!SmartParseV2016.Generic_Parse_Decimal(D_UNITS_USED_M3, utilityviewmodel))
                        {
                            return false;
                        }
                        else
                        {
                            utilityviewmodel.temp_decimal = utilityviewmodel.genericDecimalValue;
                        }
                        UNIT_OF_MEASURE = "m3";
                    }
                }

                if (utilityviewmodel.token.Contains(unit_calorific_value_for_this_period))
                {
                    utilityviewmodel.token = utilityviewmodel.token.Replace(unit_calorific_value_for_this_period, "");
                    utilityviewmodel.token = utilityviewmodel.token.Trim('(');
                    utilityviewmodel.token = utilityviewmodel.token.Trim(')');
                    if (!SmartParseV2016.Generic_Parse_Decimal(utilityviewmodel.token.Trim(), utilityviewmodel))
                    {
                        return false;
                    }
                    else
                    {
                        utilityviewmodel.temp_decimal = utilityviewmodel.genericDecimalValue;
                    }
                    utilityviewmodel.CALORIFIC_VALUE = utilityviewmodel.temp_decimal.ToString();
                }
                if (utilityviewmodel.token.Contains("Gas units converted"))
                {
                    utilityviewmodel.token = utilityviewmodel.token.Replace("Gas units converted", "").Trim();
                    utilityviewmodel.token = utilityviewmodel.token.Replace("into kWh", "").Trim();
                    if (utilityviewmodel.token.Contains("used over"))
                    {
                        utilityviewmodel.token = utilityviewmodel.token.Replace(SmartParametersV2016.space, SmartParametersV2016.bar.ToString());
                        string[] components = utilityviewmodel.token.Split(SmartParametersV2016.bar);
                        if (components.Length > 0)
                        {
                            D_UNITS_USED_KWH = components[1].Trim();
                        }
                    }
                    else
                    {
                        D_UNITS_USED_KWH = utilityviewmodel.token.Trim();
                    }
                    if (!SmartParseV2016.Generic_Parse_Decimal(D_UNITS_USED_M3, utilityviewmodel))
                    {
                        return false;
                    }
                    else
                    {
                        utilityviewmodel.temp_decimal = utilityviewmodel.genericDecimalValue;
                    }
                    // Both D_LAST_READ and D_THIS_READ have already been tested as Ints
                    if (SmartParseV2016.Add_Readings_Test(SmartParametersV2016.defaultDate,
                                                            Convert.ToDateTime(utilityviewmodel.READINGS_PERIOD_START),
                                                            Convert.ToDateTime(utilityviewmodel.READINGS_PERIOD_END),
                                                            D_LAST_READ,
                                                            D_THIS_READ,
                                                            UNIT_OF_MEASURE))
                    {
                        switch (utilityviewmodel.resource_code)
                        {
                            case SmartParametersV2016.Gas:
                                SmartUtilityV2022.G_ReadingsList_Add(utilityviewmodel,

                                                                        SmartParametersV2016.Utility,
                                                                        utilityviewmodel.supplier_code,
                                                                        utilityviewmodel.brand_code,
                                                                        utilityviewmodel.ACCOUNT_NO,
                                                                        utilityviewmodel.CREATED,
                                                                        utilityviewmodel.STATEMENT_ID,
                                                                        utilityviewmodel.BILL_DATE,
                                                                        utilityviewmodel.smell.MPRN,
                                                                        utilityviewmodel.READINGS_PERIOD_START,
                                                                        utilityviewmodel.READINGS_PERIOD_END,
                                                                        METER_SERIAL_NO,
                                                                        READ_TYPE,
                                                                        D_LAST_READ,
                                                                        D_THIS_READ,
                                                                        D_UNITS_USED_M3,
                                                                        UNIT_OF_MEASURE,
                                                                        D_UNITS_USED_KWH,
                                                                        utilityviewmodel.CALORIFIC_VALUE);
                                break;
                            default:
                                break;
                        }
                    }

                    line_count = utilityviewmodel.outer_index + 1;    // <= Point at the next one to process

                    utilityviewmodel.inner_index = line_count;

                    utilityviewmodel.go_back_flag = false;
                    if (!G_Readings_Unit_Inner_Index(utilityviewmodel,
                                                ref line_count,
                                                sections,
                                                section_index,
                                                lines))
                    {
                        return false;
                    }
                    else
                    {
                        if (utilityviewmodel.go_back_flag)
                        {
                            return true;
                        }
                    }

                    utilityviewmodel.outer_index = utilityviewmodel.inner_index;
                }
                utilityviewmodel.outer_index++;
            }
            return true;
        }

        private static bool G_Readings_Unit_Inner_Index(UtilityViewModel utilityviewmodel,
                                                        ref int line_count,
                                                        string[][] sections,
                                                        int section_index,
                                                        string[] lines)
        {
            string First = "First",
                        Next = "Next";

            utilityviewmodel.go_back_flag = false;

            while (utilityviewmodel.inner_index <= Convert.ToInt32(sections[section_index][5]))
            {
                utilityviewmodel.token = lines[utilityviewmodel.inner_index].Trim();

                // If its a Date - we want to break!!
                int dash_index = utilityviewmodel.token.IndexOf(SmartParametersV2016.dash, 0);
                if (dash_index >= 0)
                {
                    string TEMP_DATE = utilityviewmodel.token.Substring(0, dash_index).Trim();
                    TEMP_DATE = Fillout_Year(TEMP_DATE, SmartParametersV2016.spaceSplit, SmartParametersV2016.dash);
                    if (!SmartParseV2016.Generic_Parse_Datetime(TEMP_DATE, utilityviewmodel))
                    {
                        return false;
                    }
                    else
                    {
                        utilityviewmodel.READINGS_PERIOD_START = TEMP_DATE;
                        // Wind back one line
                        utilityviewmodel.inner_index--;
                        break;
                    }
                }

                if (utilityviewmodel.token.Contains("Cost of gas used this period"))
                {
                    line_count = utilityviewmodel.inner_index;
                    utilityviewmodel.go_back_flag = true;
                    break;
                }

                if (!Check_Kwh(utilityviewmodel,
                                First,
                                Next))
                {
                    return false;
                }
                //    Everything that moron does, involves having
                //    background noise which I find SO distracting
                utilityviewmodel.inner_index++;
            }
            return true;
        }

        private static bool Check_Kwh(UtilityViewModel utilityviewmodel,
                                        string First,
                                        string Next)
        {
            if ((utilityviewmodel.token.Contains("kWh used at")) ||
                (utilityviewmodel.token.Contains("kWh x")))
            {
                string UNITS_TYPE = utilityviewmodel.token;
                int curr = UNITS_TYPE.IndexOf(utilityviewmodel.bill_currency_symbol);
                if (curr >= 0)
                {
                    UNITS_TYPE = UNITS_TYPE.Substring(0, curr).Trim();
                }
                utilityviewmodel.token = utilityviewmodel.token.Replace("used at", "x").Trim();
                if (utilityviewmodel.token.Contains("Cost of"))
                {
                    utilityviewmodel.token = utilityviewmodel.token.Replace("Cost of", "").Trim();
                }
                if (utilityviewmodel.token.Contains("gas"))
                {
                    utilityviewmodel.token = utilityviewmodel.token.Replace("gas", "").Trim();
                }

                utilityviewmodel.token = utilityviewmodel.token.Replace("first", First);
                utilityviewmodel.token = utilityviewmodel.token.Replace("next", Next);
                if (utilityviewmodel.token.IndexOf(First) == -1)
                {
                    if (utilityviewmodel.token.IndexOf(Next) == -1)
                    {
                        utilityviewmodel.token = "Filler " + utilityviewmodel.token;
                    }
                }

                string UNITS = "0",
                        UNIT_OF_MEASURE = "",
                        UNITS_RATE = "0",
                        UNITS_COST = "";
                decimal temp_decimal;

                //Everything that moron does, involves having
                // background noise which I find SO distracting
                string[] components = utilityviewmodel.token.Split(SmartParametersV2016.spaceSplit);
                int component = 0;
                while (component < components.Length)
                {
                    switch (component)
                    {
                        case 0:
                            // first or next 
                            utilityviewmodel.units_band = (short)(utilityviewmodel.units_band + 1);
                            break;
                        case 1:
                            // Units
                            UNITS = components[component].Replace("(", "").Trim();
                            if (!SmartParseV2016.Generic_Parse_Decimal(UNITS, utilityviewmodel))
                            {
                                return false;
                            }
                            break;
                        case 2:
                            // kWh
                            UNIT_OF_MEASURE = components[component];
                            break;
                        case 3:
                            // x times
                            break;
                        case 4:
                            // Unit Rate
                            if (components[component].IndexOf(utilityviewmodel.bill_denomination_symbol.ToString()) >= 0)
                            {
                                UNITS_RATE = components[component].Replace(utilityviewmodel.bill_denomination_symbol.ToString(), "").Trim();
                                UNITS_RATE = UNITS_RATE.Replace(")", "").Trim();
                                if (!SmartParseV2016.Generic_Parse_Decimal(UNITS_RATE, utilityviewmodel))
                                {
                                    return false;
                                }
                                else
                                {
                                    temp_decimal = utilityviewmodel.genericDecimalValue;
                                }
                                UNITS_RATE = temp_decimal.ToString();
                            }
                            break;
                        case 5:
                            // Cost
                            //UNITS_COST = "";
                            if (!SmartParseV2016.Currency_Strip(utilityviewmodel.bill_currency_symbol, utilityviewmodel.bill_currency_separator, utilityviewmodel.bill_thousands_separator, components[component].Trim(), utilityviewmodel))
                            {
                                return false;
                            }
                            else
                            {
                                UNITS_COST = utilityviewmodel.value;
                            }
                            //int temp = 0;
                            if (!SmartParseV2016.Generic_Parse_Integer(UNITS_COST, utilityviewmodel))
                            {
                                return false;
                            }
                            //else
                            //{
                            //    temp = utilityviewmodel.genericTransactionValue;
                            //}
                            break;
                        default:
                            break;
                    }
                    component++;
                }

                switch (utilityviewmodel.resource_code)
                {
                    case SmartParametersV2016.Gas:
                        SmartUtilityV2022.G_Unit_ChargesList_Add(utilityviewmodel,

                                                                    SmartParametersV2016.Utility,
                                                                    utilityviewmodel.supplier_code,
                                                                    utilityviewmodel.brand_code,
                                                                    utilityviewmodel.ACCOUNT_NO,
                                                                    utilityviewmodel.CREATED,
                                                                    utilityviewmodel.STATEMENT_ID,
                                                                    utilityviewmodel.BILL_DATE,
                                                                    utilityviewmodel.smell.MPRN,
                                                                    utilityviewmodel.READINGS_PERIOD_START,
                                                                    utilityviewmodel.READINGS_PERIOD_END,
                                                                    utilityviewmodel.units_band.ToString(),
                                                                    UNITS_TYPE,
                                                                    UNITS,
                                                                    UNITS_RATE,
                                                                    UNIT_OF_MEASURE,
                                                                    UNITS_COST);
                        break;
                    default:
                        break;
                }
                return true;
            }
            return false;
        }

        private static bool G_Standing_Charges(UtilityViewModel utilityviewmodel,
                                            ref int line_count,
                                            string[][] sections,
                                            string[] lines,
                                            int section_index,
                                            short charges_item)
        {
            string Standing_Charge = "Standing Charge",
                    Standing_charge = "Standing charge";

            if (SmartParseV2016.Token_Identify(utilityviewmodel, Standing_Charge, false) ||
                SmartParseV2016.Token_Identify(utilityviewmodel, Standing_charge, false))
            {
                DateTime FROM_DATE = SmartParametersV2016.defaultDate,
                        READ_DATE = SmartParametersV2016.defaultDate;

                short CHARGES_DAYS = 0;
                string CHARGES_TYPE,// = "",
                        CHARGES_COST = "0",
                        STANDING_CHARGE = "0";  // There IS no 'STANDING_CHARGE' for British Gas
                                                // There is now ... these cunts can never make up their fucking minds

                string TEMP_DATE;// = "";
                int dash_index;

                int outer_index = line_count + 1;
                while (outer_index <= Convert.ToInt32(sections[section_index][5]))
                {
                    utilityviewmodel.token = lines[outer_index].Trim();
                    dash_index = utilityviewmodel.token.IndexOf(SmartParametersV2016.dash);
                    if (dash_index >= 0)
                    {
                        TEMP_DATE = utilityviewmodel.token.Substring(0, dash_index).Trim();
                        TEMP_DATE = Fillout_Year(TEMP_DATE, SmartParametersV2016.spaceSplit, SmartParametersV2016.dash);
                        if (!SmartParseV2016.Generic_Parse_Datetime(TEMP_DATE, utilityviewmodel))
                        {
                            return false;
                        }
                        else
                        {
                            FROM_DATE = utilityviewmodel.genericTargetDate;
                        }
                        TEMP_DATE = utilityviewmodel.token.Substring(dash_index).Trim(SmartParametersV2016.dashSplit).Trim();
                        TEMP_DATE = TEMP_DATE.Trim();
                        TEMP_DATE = Fillout_Year(TEMP_DATE, SmartParametersV2016.spaceSplit, SmartParametersV2016.dash);
                        if (!SmartParseV2016.Generic_Parse_Datetime(TEMP_DATE, utilityviewmodel))
                        {
                            return false;
                        }
                        else
                        {
                            READ_DATE = utilityviewmodel.genericTargetDate;
                        }
                    }
                    if (utilityviewmodel.token.Contains("days at"))
                    {
                        CHARGES_TYPE = utilityviewmodel.token;
                        utilityviewmodel.token = utilityviewmodel.token.Replace("days at", "").Trim();
                        utilityviewmodel.token = utilityviewmodel.token.Replace("per day", "");
                        utilityviewmodel.token = utilityviewmodel.token.Replace("/day", "");
                        utilityviewmodel.token = SmartParseV2016.Remove_Double_Spaces_V3(utilityviewmodel.token);
                        utilityviewmodel.token = utilityviewmodel.token.Trim();

                        int temp;// = 0;

                        string[] components = utilityviewmodel.token.Split(SmartParametersV2016.spaceSplit);
                        int component_count = 0;
                        while (component_count < components.Length)
                        {
                            switch (component_count)
                            {
                                case 0:
                                    string charges_days = components[component_count].Trim();
                                    charges_days = charges_days.Replace(".00", "");   // No of days is always a whole number is it not?
                                                                                      // Sometimes ... days has a decimal point e.g. 99.00
                                                                                      // Sometimes Gas values have these embedded
                                                                                      //UNITS = UNITS.Replace("comma", "");
                                    if (!SmartParseV2016.Generic_Parse_Integer(charges_days, utilityviewmodel))
                                    {
                                        return false; // goto quit_C2;
                                    }
                                    else
                                    {
                                        temp = utilityviewmodel.genericTransactionValue;
                                    }
                                    CHARGES_DAYS = Convert.ToInt16(temp);
                                    break;
                                case 1:
                                    // Charges Rate
                                    components[component_count] = components[component_count].Replace(SmartParametersV2016.decimalPoint + utilityviewmodel.bill_denomination_symbol, utilityviewmodel.bill_denomination_symbol.ToString());
                                    STANDING_CHARGE = components[component_count].Replace(utilityviewmodel.bill_denomination_symbol.ToString(), "").Trim();
                                    //decimal temp_decimal = 0.0M;
                                    if (!SmartParseV2016.Generic_Parse_Decimal(STANDING_CHARGE, utilityviewmodel))
                                    {
                                        return false;
                                    }
                                    //else
                                    //{
                                    //    temp_decimal = utilityviewmodel.genericDecimalValue;
                                    //}
                                    break;
                                case 2:
                                    // Charges Cost
                                    if (!SmartParseV2016.Currency_Strip(utilityviewmodel.bill_currency_symbol, utilityviewmodel.bill_currency_separator, utilityviewmodel.bill_thousands_separator, components[component_count], utilityviewmodel))
                                    {
                                        return false;
                                    }
                                    else
                                    {
                                        CHARGES_COST = utilityviewmodel.value;
                                    }
                                    if (!SmartParseV2016.Generic_Parse_Integer(CHARGES_COST, utilityviewmodel))
                                    {
                                        return false;
                                    }
                                    //else
                                    //{
                                    //    temp = utilityviewmodel.genericTransactionValue;
                                    //}
                                    break;
                                default:
                                    break;
                            }
                            component_count++;
                        }

                        // Add in a Standing Charge record WHEN WE GET THEM
                        // DON'T ADD IN STANDING CHARGE RECORDS WHEN THERE ARE NONE!!!!!
                        switch (utilityviewmodel.resource_code)
                        {
                            case SmartParametersV2016.Gas:
                                SmartUtilityV2022.G_Standing_ChargesList_Add(utilityviewmodel,
                                                                            SmartParametersV2016.Utility,
                                                                            utilityviewmodel.supplier_code,
                                                                            utilityviewmodel.brand_code,
                                                                            utilityviewmodel.ACCOUNT_NO,
                                                                            utilityviewmodel.CREATED,
                                                                            utilityviewmodel.STATEMENT_ID,
                                                                            utilityviewmodel.BILL_DATE,
                                                                            utilityviewmodel.smell.MPRN,
                                                                            FROM_DATE.ToString(SmartParametersV2016.defaultCulture),
                                                                            READ_DATE.ToString(SmartParametersV2016.defaultCulture),
                                                                            charges_item.ToString(),
                                                                            CHARGES_TYPE,
                                                                            STANDING_CHARGE,
                                                                            CHARGES_DAYS.ToString(),
                                                                            CHARGES_COST);
                                break;
                            default:
                                break;
                        }
                        line_count = outer_index;
                        goto go_back;
                    }
                    outer_index++;
                }
            go_back:
                return true;
            }
            return false;
        }

        private static bool Parse_SectionH2(MainViewModel ourviewmodel,
                                            UtilityViewModel utilityviewmodel,
                                            string[][] sections,
                                            string section,
                                            string sub_section,
                                            string[] lines)
        {
            utilityviewmodel.currentIndex = 0;
            if (!SmartParseV2016.Decode_Section(section, sub_section, sections, utilityviewmodel.resource_code.ToString(), utilityviewmodel))
            {
                return true;
            }

            int version = 3;
            utilityviewmodel.token = "";

            short units_band = 0;

            // There is now ... these cunts can never make up their fucking minds

            utilityviewmodel.METER_SERIAL_NO = utilityviewmodel.smell.meter_serial_no;

            switch (version)
            {
                case 1:
                    // WE DON'T DO VERSION 1 ANYMORE
                    // (Versions2 and 3 are hard enough ...)
                    //parse_page2_v1(page,
                    //                strText,
                    //                utilityviewmodel,
                    //                BILL_PERIOD_END,
                    //                SUPPLY_VAT);
                    break;
                case 2:
                case 3:
                    // Separate just to be on the safe side
                    for (int line_count = Convert.ToInt32(sections[utilityviewmodel.currentIndex][4]); line_count <= Convert.ToInt32(sections[utilityviewmodel.currentIndex][5]); line_count++)
                    {
                        utilityviewmodel.token = lines[line_count].Trim();

                        if (string.IsNullOrEmpty(utilityviewmodel.METER_SERIAL_NO))
                        {
                            if (G_Meter_Serial_Number(utilityviewmodel))
                            {
                                goto the_old_daysH2;
                            }
                            else
                            {
                                if (!string.IsNullOrEmpty(ourviewmodel.errorMessage))
                                {
                                    return false;
                                }
                            }
                        }

                        if (G_Readings_Units(utilityviewmodel,
                                            ref line_count,
                                            sections,
                                            lines,
                                            utilityviewmodel.currentIndex,
                                            utilityviewmodel.METER_SERIAL_NO))
                        {
                            goto the_old_daysH2;
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(ourviewmodel.errorMessage))
                            {
                                return false;
                            }
                        }

                        if (G_Standing_Charges(utilityviewmodel,
                                            ref line_count,
                                            sections,
                                            lines,
                                            utilityviewmodel.currentIndex,
                                            units_band))
                        {
                            goto the_old_daysH2;
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(ourviewmodel.errorMessage))
                            {
                                return false;
                            }
                        }
                    the_old_daysH2:
                        continue;
                    }
                    break;
                default:
                    break;
            }
            return true;
        }

        private static bool Parse_SectionL1(MainViewModel ourviewmodel,
                                            UtilityViewModel utilityviewmodel,
                                            string[][] sections,
                                            string section,
                                            string sub_section,
                                            string[] lines)
        {

            utilityviewmodel.currentIndex = 0;
            if (!SmartParseV2016.Decode_Section(section, sub_section, sections, utilityviewmodel.resource_code.ToString(), utilityviewmodel))
            {
                return true;
            }

            utilityviewmodel.token = "";

            utilityviewmodel.discount_item = 0;

            for (int line_count = Convert.ToInt32(sections[utilityviewmodel.currentIndex][4]); line_count <= Convert.ToInt32(sections[utilityviewmodel.currentIndex][5]); line_count++)
            {
                utilityviewmodel.token = lines[line_count].Trim();

                if (E_Total(ourviewmodel, utilityviewmodel, ref line_count, sections, utilityviewmodel.currentIndex, lines))
                {
                    goto the_old_daysL1;
                }
                else
                {
                    if (!string.IsNullOrEmpty(ourviewmodel.errorMessage))
                    {
                        return false;
                    }
                }

                if (!Direct_Debit_Discount1(ourviewmodel, utilityviewmodel, ref line_count, lines))
                {
                    if (!string.IsNullOrEmpty(ourviewmodel.errorMessage))
                    {
                        return false;
                    }
                }

                if (!Direct_Debit_Discount2(ourviewmodel, utilityviewmodel))
                {
                    if (!string.IsNullOrEmpty(ourviewmodel.errorMessage))
                    {
                        return false;
                    }
                }

                if (!Dual_Fuel_Discount(ourviewmodel, utilityviewmodel))
                {
                    if (!string.IsNullOrEmpty(ourviewmodel.errorMessage))
                    {
                        return false;
                    }
                }

                //if (SmartParseV2016.Token_Identify(utilityviewmodel, "S", true, true))
                //{
                //    if (SmartParseV2016.fix_supply_number(line_count,
                //                            lines,
                //                            sections[section_index][5],
                //                            sections[section_index][4],
                //                            utilityviewmodel,
                //                            false))
                //    {
                //        SmartUtilityV2022.Amelia_Update_Mpan(utilityviewmodel, utilityviewmodel.mpan);
                //    }
                //    line_count = Convert.ToInt32(sections[section_index][5]);
                //    goto the_old_daysL1;
                //}

                if (Decode_VAT(ourviewmodel, utilityviewmodel))
                {
                    goto the_old_daysL1;
                }
                else
                {
                    if (!string.IsNullOrEmpty(ourviewmodel.errorMessage))
                    {
                        return false;
                    }
                }
            the_old_daysL1:
                continue;
            }
            return true;
        }

        private static bool Parse_SectionL2(MainViewModel ourviewmodel,
                                            UtilityViewModel utilityviewmodel,
                                            string[][] sections,
                                            string section,
                                            string sub_section,
                                            string[] lines)
        {
            utilityviewmodel.currentIndex = 0;
            if (!SmartParseV2016.Decode_Section(section, sub_section, sections, utilityviewmodel.resource_code.ToString(), utilityviewmodel))
            {
                return true;
            }

            utilityviewmodel.token = "";
            utilityviewmodel.discount_item = 0;

            for (int line_count = Convert.ToInt32(sections[utilityviewmodel.currentIndex][4]); line_count <= Convert.ToInt32(sections[utilityviewmodel.currentIndex][5]); line_count++)
            {
                utilityviewmodel.token = lines[line_count].Trim();

                if (G_Total(ourviewmodel, utilityviewmodel, ref line_count, sections, utilityviewmodel.currentIndex, lines))
                {
                    goto the_old_daysL2;
                }
                else
                {
                    if (!string.IsNullOrEmpty(ourviewmodel.errorMessage))
                    {
                        return false;
                    }
                }

                if (!Direct_Debit_Discount1(ourviewmodel, utilityviewmodel, ref line_count, lines))
                {
                    if (!string.IsNullOrEmpty(ourviewmodel.errorMessage))
                    {
                        return false;
                    }
                }

                if (!Direct_Debit_Discount2(ourviewmodel, utilityviewmodel))
                {
                    if (!string.IsNullOrEmpty(ourviewmodel.errorMessage))
                    {
                        return false;
                    }
                }

                if (!Dual_Fuel_Discount(ourviewmodel, utilityviewmodel))
                {
                    if (!string.IsNullOrEmpty(ourviewmodel.errorMessage))
                    {
                        return false;
                    }
                }

                if (Decode_VAT(ourviewmodel, utilityviewmodel))
                {
                    goto the_old_daysL2;
                }
                else
                {
                    if (!string.IsNullOrEmpty(ourviewmodel.errorMessage))
                    {
                        return false;
                    }
                }
            the_old_daysL2:
                continue;
            }
            return true;
        }

        private static bool E_Total(MainViewModel ourviewmodel,
                                    UtilityViewModel utilityviewmodel,
                                    ref int line_count,
                                    string[][] sections,
                                    int section_index,
                                    string[] lines)
        {
            string total_electricity_used = "Total electricity used",
                    electricity_youve_used = "Electricity you’ve used";
            if (SmartParseV2016.Token_Identify(utilityviewmodel, electricity_youve_used, true) ||
                SmartParseV2016.Token_Identify(utilityviewmodel, total_electricity_used, true))
            {
                //int temp = 0;

                // She is such a FUCKING NOISY BITCH ON THE TELEPHONE - SHOUTING DOWN IT ALL THE TIME...
                // Now wonder I can never think when she's around - so FUKCING noisy
                utilityviewmodel.token = SmartParseV2016.Remove_Double_Spaces_V3(utilityviewmodel.token);
                utilityviewmodel.token = utilityviewmodel.token.Replace("this", "").Trim();
                utilityviewmodel.token = utilityviewmodel.token.Replace("period", "").Trim();
                if (string.IsNullOrEmpty(utilityviewmodel.token))
                {
                    // Loop round until we find a currency symbol
                    int outer_index = line_count + 1;
                    while (outer_index <= Convert.ToInt32(sections[section_index][5]))
                    {
                        utilityviewmodel.token = lines[outer_index].Trim();
                        if (utilityviewmodel.token.Contains(utilityviewmodel.bill_currency_symbol))
                        {
                            // and I thought YOU were thick!!!!
                            // This is someone who thinks Gt Budworth is near Macclesfield
                            // Basically - none of her dumb family have a CLUE where
                            // anything in the country is
                            line_count = outer_index;
                            break;
                        }
                        outer_index++;
                    }
                }
                string E_NEW_CHARGES;// = "0";

                if (!SmartParseV2016.Currency_Strip(utilityviewmodel.bill_currency_symbol, utilityviewmodel.bill_currency_separator, utilityviewmodel.bill_thousands_separator, utilityviewmodel.token, utilityviewmodel))
                {
                    return false;
                }
                else
                {
                    E_NEW_CHARGES = utilityviewmodel.value;
                }
                if (!SmartParseV2016.Generic_Parse_Integer(E_NEW_CHARGES, utilityviewmodel))
                {
                    return false;
                }
                //else
                //{
                //    temp = utilityviewmodel.genericTransactionValue;
                //}
                SmartParseV2016.Update_Bill(ourviewmodel, utilityviewmodel, "RESOURCE_NEW_CHARGES", E_NEW_CHARGES);
                return true;
            }
            return false;
        }

        private static bool G_Total(MainViewModel ourviewmodel,
                                    UtilityViewModel utilityviewmodel,
                                    ref int line_count,
                                    string[][] sections,
                                    int section_index,
                                    string[] lines)
        {
            string total_gas_used = "Total gas used",
                    gas_youve_used = "Gas you’ve used";
            if (SmartParseV2016.Token_Identify(utilityviewmodel, gas_youve_used, true) ||
                SmartParseV2016.Token_Identify(utilityviewmodel, total_gas_used, true))
            {
                //int temp = 0;

                // She is such a FUCKING NOISY BITCH ON THE TELEPHONE - SHOUTING DOWN IT ALL THE TIME...
                // Now wonder I can never think when she's around - so FUKCING noisy
                utilityviewmodel.token = SmartParseV2016.Remove_Double_Spaces_V3(utilityviewmodel.token);
                utilityviewmodel.token = utilityviewmodel.token.Replace("this", "").Trim();
                utilityviewmodel.token = utilityviewmodel.token.Replace("period", "").Trim();
                if (string.IsNullOrEmpty(utilityviewmodel.token))
                {
                    // Loop round until we find a currency symbol
                    int outer_index = line_count + 1;
                    while (outer_index <= Convert.ToInt32(sections[section_index][5]))
                    {
                        utilityviewmodel.token = lines[outer_index].Trim();
                        if (utilityviewmodel.token.Contains(utilityviewmodel.bill_currency_symbol))
                        {
                            // and I thought YOU were thick!!!!
                            // This is someone who thinks Gt Budworth is near Macclesfield
                            // Basically - none of her dumb family have a CLUE where
                            // anything in the country is
                            line_count = outer_index;
                            break;
                        }
                        outer_index++;
                    }
                }
                string G_NEW_CHARGES;// = "0";

                if (!SmartParseV2016.Currency_Strip(utilityviewmodel.bill_currency_symbol, utilityviewmodel.bill_currency_separator, utilityviewmodel.bill_thousands_separator, utilityviewmodel.token, utilityviewmodel))
                {
                    return false;
                }
                else
                {
                    G_NEW_CHARGES = utilityviewmodel.value;
                }
                if (!SmartParseV2016.Generic_Parse_Integer(G_NEW_CHARGES, utilityviewmodel))
                {
                    return false;
                }
                //else
                //{
                //    temp = utilityviewmodel.genericTransactionValue;
                //}
                SmartParseV2016.Update_Bill(ourviewmodel, utilityviewmodel, "RESOURCE_NEW_CHARGES", G_NEW_CHARGES);
                return true;
            }
            return false;
        }

        private static bool Direct_Debit_Discount1(MainViewModel ourviewmodel,
                                                    UtilityViewModel utilityviewmodel,
                                                    ref int line_count,
                                                    string[] lines)
        {
            string your_direct_debit = "Your Direct Debit",
                    discount = "discount";

            // Direct Debit discount
            if (SmartParseV2016.Token_Identify(utilityviewmodel, your_direct_debit, true))
            {
                string DISCOUNT_AMOUNT,// = "0",
                        DISCOUNT_CREDIT_DATE,// = "",
                        DISCOUNT_TYPE,// = "",
                        credit = "credit",
                        debit = "debit";
                //int TEMP = 0;
                short DISCOUNT_VAT_CODE = SmartParametersV2016.zeroRateVatCode;
                // Get some kind of discount date
                DateTime DISCOUNT_DATE = SmartNibbyV2016.ConvertDate(utilityviewmodel.BILL_PERIOD_END,
                    SmartParametersV2016.defaultDate, ourviewmodel);
                if (ourviewmodel.errorMessage != "")
                {
                    return false;
                }
                DISCOUNT_TYPE = your_direct_debit.Replace("Your", "").Trim();
                while (line_count + 1 < lines.Length)
                {
                    line_count++;
                    utilityviewmodel.token = utilityviewmodel.token + SmartParametersV2016.space + lines[line_count];
                    if ((lines[line_count].Contains(credit)) ||
                        (lines[line_count].Contains(debit)))
                    {
                        break;
                    }
                }
                if (utilityviewmodel.token.Contains(discount))
                {
                    utilityviewmodel.token = utilityviewmodel.token.Replace(discount, "").Trim();
                    DISCOUNT_TYPE = DISCOUNT_TYPE + SmartParametersV2016.space + discount;
                }

                if (!SmartParseV2016.Currency_Strip(utilityviewmodel.bill_currency_symbol, utilityviewmodel.bill_currency_separator, utilityviewmodel.bill_thousands_separator, utilityviewmodel.token, utilityviewmodel))
                {
                    return false;
                }
                else
                {
                    DISCOUNT_AMOUNT = utilityviewmodel.value;
                }
                if (!SmartParseV2016.Generic_Parse_Integer(DISCOUNT_AMOUNT, utilityviewmodel))
                {
                    return false;
                }
                //else
                //{
                //    TEMP = utilityviewmodel.genericTransactionValue;
                //}
                DISCOUNT_CREDIT_DATE = utilityviewmodel.BILL_DATE.Substring(0, 2);
                utilityviewmodel.discount_item = (short)(utilityviewmodel.discount_item + 1);
                if (!SmartParseV2016.Check_Discount_Is_There(utilityviewmodel,
                                                                DISCOUNT_DATE,
                                                                utilityviewmodel.discount_item,
                                                                DISCOUNT_TYPE,
                                                                DISCOUNT_VAT_CODE,
                                                                DISCOUNT_AMOUNT))
                {
                    SmartParseV2016.No_Wonder_I_Cant_Fucking_Work(utilityviewmodel,
                                                                    DISCOUNT_DATE,
                                                                    utilityviewmodel.discount_item,
                                                                    DISCOUNT_TYPE,
                                                                    DISCOUNT_VAT_CODE,
                                                                    DISCOUNT_AMOUNT);
                    SmartParseV2016.Update_Bill(ourviewmodel, utilityviewmodel, "DISCOUNTS", DISCOUNT_AMOUNT);

                    SmartParseV2016.Update_Bill(ourviewmodel, utilityviewmodel, "DISCOUNT_CREDIT_DATE", DISCOUNT_CREDIT_DATE);
                }
                return true;
            }
            return false;
        }

        private static bool Direct_Debit_Discount2(MainViewModel ourviewmodel,
                                                    UtilityViewModel utilityviewmodel)
        {
            string direct_debit_discount = "Direct Debit discount",
                    direct_debit = "Direct Debit";

            // Direct Debit discount
            if (SmartParseV2016.Token_Identify(utilityviewmodel, direct_debit_discount, true) ||
                SmartParseV2016.Token_Identify(utilityviewmodel, direct_debit, true))
            {
                string DISCOUNT_AMOUNT,// = "",
                   DISCOUNT_CREDIT_DATE,// = "",
                   DISCOUNT_TYPE;// = "";
                //int TEMP = 0;
                short DISCOUNT_VAT_CODE = SmartParametersV2016.zeroRateVatCode;
                // Get some kind of discount date
                DateTime DISCOUNT_DATE = SmartNibbyV2016.ConvertDate(utilityviewmodel.BILL_PERIOD_END,
                    SmartParametersV2016.defaultDate, ourviewmodel);
                if (ourviewmodel.errorMessage != "")
                {
                    return false;
                }
                if (!SmartParseV2016.Currency_Strip(utilityviewmodel.bill_currency_symbol, utilityviewmodel.bill_currency_separator, utilityviewmodel.bill_thousands_separator, utilityviewmodel.token, utilityviewmodel))
                {
                    return false;
                }
                else
                {
                    DISCOUNT_AMOUNT = utilityviewmodel.value;
                }
                if (!SmartParseV2016.Generic_Parse_Integer(DISCOUNT_AMOUNT, utilityviewmodel))
                {
                    return false;
                }
                //else
                //{
                //    TEMP = utilityviewmodel.genericTransactionValue;
                //}
                DISCOUNT_TYPE = direct_debit_discount;
                utilityviewmodel.DISCOUNTCREDITDATE = utilityviewmodel.BILL_DATE.Substring(0, 2);
                SmartParseV2016.Reformat_Discount_Credit_Bill(utilityviewmodel);
                DISCOUNT_CREDIT_DATE = utilityviewmodel.DISCOUNTCREDITDATE;
                utilityviewmodel.discount_item = (short)(utilityviewmodel.discount_item + 1);
                if (!SmartParseV2016.Check_Discount_Is_There(utilityviewmodel,
                                                            DISCOUNT_DATE,
                                                            utilityviewmodel.discount_item,
                                                            DISCOUNT_TYPE,
                                                            DISCOUNT_VAT_CODE,
                                                            DISCOUNT_AMOUNT))
                {
                    SmartParseV2016.No_Wonder_I_Cant_Fucking_Work(utilityviewmodel,
                                                                    DISCOUNT_DATE,
                                                                    utilityviewmodel.discount_item,
                                                                    DISCOUNT_TYPE,
                                                                    DISCOUNT_VAT_CODE,
                                                                    DISCOUNT_AMOUNT);
                    SmartParseV2016.Update_Bill(ourviewmodel, utilityviewmodel, "DISCOUNTS", DISCOUNT_AMOUNT);
                    SmartParseV2016.Update_Bill(ourviewmodel, utilityviewmodel, "DISCOUNT_CREDIT_DATE", DISCOUNT_CREDIT_DATE);
                }
                return true;
            }
            return false;
        }

        private static bool Dual_Fuel_Discount(MainViewModel ourviewmodel,
                                                  UtilityViewModel utilityviewmodel)
        {
            string dual_fuel_discount = "Dual Fuel discount",
                    dual_fuel = "Dual Fuel";

            // Dual Fuel discount
            if (SmartParseV2016.Token_Identify(utilityviewmodel, dual_fuel_discount, true) ||
                SmartParseV2016.Token_Identify(utilityviewmodel, dual_fuel, true))
            {
                string DISCOUNT_AMOUNT,// = "",
                DISCOUNT_CREDIT_DATE,// = "",
                DISCOUNT_TYPE;// = "";
                // string   dual_fuel = "Dual Fuel";
                //int TEMP = 0;
                short DISCOUNT_VAT_CODE = SmartParametersV2016.zeroRateVatCode;
                // Get some kind of discount date
                DateTime DISCOUNT_DATE = SmartNibbyV2016.ConvertDate(utilityviewmodel.BILL_PERIOD_END,
                    SmartParametersV2016.defaultDate, ourviewmodel);
                if (ourviewmodel.errorMessage != "")
                {
                    return false;
                }
                if (!SmartParseV2016.Currency_Strip(utilityviewmodel.bill_currency_symbol, utilityviewmodel.bill_currency_separator, utilityviewmodel.bill_thousands_separator, utilityviewmodel.token, utilityviewmodel))
                {
                    return false;
                }
                else
                {
                    DISCOUNT_AMOUNT = utilityviewmodel.value;
                }
                if (!SmartParseV2016.Generic_Parse_Integer(DISCOUNT_AMOUNT, utilityviewmodel))
                {
                    return false;
                }
                //else
                //{
                //    TEMP = utilityviewmodel.genericTransactionValue;
                //}
                DISCOUNT_TYPE = dual_fuel_discount;
                utilityviewmodel.DISCOUNTCREDITDATE = utilityviewmodel.BILL_DATE.Substring(0, 2);
                SmartParseV2016.Reformat_Discount_Credit_Bill(utilityviewmodel);
                DISCOUNT_CREDIT_DATE = utilityviewmodel.DISCOUNTCREDITDATE;
                utilityviewmodel.discount_item = (short)(utilityviewmodel.discount_item + 1);     // <= Can put this in MES if needs be to carry ot through
                if (!SmartParseV2016.Check_Discount_Is_There(utilityviewmodel,
                                                            DISCOUNT_DATE,
                                                            utilityviewmodel.discount_item,
                                                            DISCOUNT_TYPE,
                                                            DISCOUNT_VAT_CODE,
                                                            DISCOUNT_AMOUNT))
                {
                    SmartParseV2016.No_Wonder_I_Cant_Fucking_Work(utilityviewmodel,
                                                                    DISCOUNT_DATE,
                                                                    utilityviewmodel.discount_item,
                                                                    DISCOUNT_TYPE,
                                                                    DISCOUNT_VAT_CODE,
                                                                    DISCOUNT_AMOUNT);
                    SmartParseV2016.Update_Bill(ourviewmodel, utilityviewmodel, "DISCOUNTS", DISCOUNT_AMOUNT);
                    SmartParseV2016.Update_Bill(ourviewmodel, utilityviewmodel, "DISCOUNT_CREDIT_DATE", DISCOUNT_CREDIT_DATE);
                }
                // IF we have a Direct Debit then we must pay by DD
                return true;
                // IF we have a Dual Fuel discount then we must be on it .. and modify the Bill to say so
                //SmartParseV2016.Update_Bill("DUAL_FUEL", "Y");    // And tell SMartSwitch
            }
            return false;
        }

        private static string Find_Meter_Serial_No_V3(UtilityViewModel utilityviewmodel)
        {
            string METER_SERIAL_NO;// = "";

            METER_SERIAL_NO = utilityviewmodel.token.Replace(":", "").Trim();
            METER_SERIAL_NO = METER_SERIAL_NO.Substring(0, METER_SERIAL_NO.Length <= 14 ? METER_SERIAL_NO.Length : 14); //Enforce 14 char max
            return METER_SERIAL_NO;
        }

        private static string Fillout_Year(string some_date, char split, string combine)
        {
            string[] components = some_date.Split(split);
            if (components.Length > 2)
            {
                if (components[0].Length == 1)
                {
                    components[0] = "0" + components[0];
                }
                if (components[1].Length > 3)
                {
                    components[1] = components[1].Substring(0, 3);
                }
                if (components[2].Length == 2)
                {
                    components[2] = "20" + components[2];
                }
                return components[0] + combine + components[1] + combine + components[2];
            }
            else
            {
                return some_date;
            }
        }
    }
}