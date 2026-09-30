using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

#if ANDROIDX
using AndroidX.AppCompat.App;
#endif
namespace SmartCubeMobile
{
    public class EOnV2016
    {
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
            // It is now fucking binned WebBrowser wb = new WebBrowser();
            // ========================

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
            new string[] {"START",      "Success",          "Login",    "LOGOUT",   "Start",                            "for-your-home/your-account", "Finding the correct cookies" },
            new string[] {"Login",      "Success",          "YOURACCS", "LOGOUT",   "Login",                            "for-your-home/your-account", "" },  // A POST
            new string[] {"YOURACCS",   "Success",          "TESTLIST", "LOGOUT",   "Landing page - fix Amelia",        "for-your-home/your-account", ""},
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

            utilityviewmodel.fuelList = "";
            utilityviewmodel.view_your_statement_path = "";
            utilityviewmodel.meter_readings_path = "";

            //string your_account_summary_path = "";



            // Everytime timer ticks, timer_Tick will be called
            // Timer will tick every second
            // FUCKING HELL - this is never Enabled????
            // You have 60 Seconds to stop
            // the timer on a good login!
            //string midata_pathname = "",
            //        youraccounts_pathname = ""; // "Your_Account/Account_Details/";
            List<SmartUtility.Bills> bills_tempList = new List<SmartUtility.Bills>();

            string next_routine = "",
                    //exit_test = "",
                    yes_routine = "",
                    no_routine = "",
                    //comments = "",
                    relative_pathname = "";

            utilityviewmodel.keep_looping = true;
            List<SmartUtility.Payments> payments_tempList = new List<SmartUtility.Payments>();

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
                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Comments: " + utilityviewmodel.comments + Environment.NewLine.ToString());
                await SmartUtilityV2022.First_Throw(ourviewmodel,
                                                    utilityviewmodel);
                                                    //DateTime.Now + ourviewmodel.utcOffset);   // Local time
#endif
#if WPF 
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
                //Scraper.DocumentText = htmlDocument.DocumentNode.OuterHtml;
                //Scraper.Document.Body.ScrollRectangle.Size;
#endif
                utilityviewmodel.current_routine = utilityviewmodel.next_routine;
                switch (utilityviewmodel.next_routine)
                {
                    case "START":
                        // The timer is re-started when the
                        // first Login Document is completed
                        utilityviewmodel.keyValues.Clear();
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
                        }
                        if (!await EON_Login(
#if ANDROIDX
                                        meterActivity,
#endif
                                        ourviewmodel,
                                        utilityviewmodel,
                                        utilityviewmodel.htmlDocument,
                                        TextBox_Active))
                        {
                            yes_routine = no_routine;
                        }

                        // Should be going onto "Login"
                        break;
                    case "Login":
                        // Try to login
                        utilityviewmodel.login_attempted = true;
                        break;
                    case "YOURACCS":
                        // The timer is re-started when the
                        // first Login Document is completed
                        if (await EON_AccountDetails(ourviewmodel,
                                                utilityviewmodel,
                                                utilityviewmodel.htmlDocument))
                        {
#if WINFORMS
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

                            if (utilityviewmodel.console)
                            {
                                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Account No: " + utilityviewmodel.account_no + Environment.NewLine.ToString());
                                //await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "UDPRN: " + utilityviewmodel.sparks.udprn + SmartParametersV2016.space +
                                //                            utilityviewmodel.smell.udprn + Environment.NewLine.ToString());
                                //
                                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Target pathname: " + utilityviewmodel.target_pathname + Environment.NewLine.ToString());
                            }
#endif
                            yes_routine = utilityviewmodel.view_your_statement_path;
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
                    case "VIEWSTATEMENT":
                        await EON_ViewYourStatement(
#if ANDROIDX
                                                meterActivity,
#endif
                                                ourviewmodel,
                                                utilityviewmodel,
                                                utilityviewmodel.htmlDocument,
                                                TextBox_Active);

                        break;
                    // Thank absolute fuck for that, this works!
                    // I have been struggling since Friday 12th Oct 2012 to get a
                    // solution to this and have only just succeeded (10am Wednesday 17th Oct 2012)
                    // This has - again - almost killed me.  I had to re-write the ENTIRE NPower site
                    // access - throw away all the 'DocumentCompleted' WebBrowser approach
                    // ***SIMPLY BECAUSE*** the fucking WebBrowser did not have the
                    // capability of trapping or notifying me of Ajax updates to the Payments
                    // screen. I have WASTED ALMOST A YEAR OF DEVELOPMENT because of this, but
                    // now - fingers, toes and testicles crossed, I should be able to cope with
                    // anything that any website (including EDF and E-ON's PDF which I couldn't read)
                    // can throw at me.  You ARE an ABSOLUTE FUCKING GENIUS, Ray, NO DOUBT ABOUT IT!
                    //                       ===    ================================================
                    // Well, done, Son!  You WILL find her!

                    // This routine always returns 'true', its ACCHIST which gets us down to 'LOGOUT'

                    case "LOGOUT":
                        // Combine the Bills_In and Bills_Out lists and
                        // only include unallocated payments past the last Bill Period end

                        // This will work because Payments_Out contains payments DECODED from any
                        // new Bills PLUS payments added from the SCRAPE which is stored in Payments_Temp
                        SmartUtilityV2022.Generic_Payment_Done(utilityviewmodel, payments_tempList);

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
                        EON_Do_Intermediates(ourviewmodel,
                                            utilityviewmodel,
                                            utilityviewmodel.next_routine,
                                            utilityviewmodel.htmlDocument,
                                            payments_tempList);
                        break;
                }
                next_routine = yes_routine;
            }
            if (!utilityviewmodel.login_attempted)
            {
                // We can assume the login failed
#if WINFORMS
                if (utilityviewmodel.console)
                {
                    await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, " Login failed");
                }
#endif
            }
            else
            {
                // We can assume the login succeeded
#if WINFORMS
                if (utilityviewmodel.console)
                {
                    await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, " Logged out");
                }
#endif
            }
            // Yay!
            return utilityviewmodel.login_finished;
        }

        private static bool EON_Do_Intermediates(MainViewModel ourviewmodel,
                                                UtilityViewModel utilityviewmodel,
                                                string next_routine,
                                                HtmlAgilityPack.HtmlDocument htmlDocument,
                                                List<SmartUtility.Payments> payments_tempList)
        {
            switch (next_routine)
            {
                //                case "PATHS": // "YOURACCOUNT":
                //                             // The timer is re-started when the
                //                             // first Login Document is completed
                //                    EON_YourAccount(htmlDocument,
                //                                    utilityviewmodel,
                //                                    rf your_account_summary_path,
                //                                    rf view_your_statement_path,
                //                                    rf meter_reading_path);
                //                    // Should be going to "METEREAD"  to pick up E7 metere or not 
                //                    break;
                case "METERREAD":
                    EON_MeterReadings(utilityviewmodel,
                                        htmlDocument);
                    // Should be going to "ACCDETLS"
                    break;
                case "PAYHIST":
                    EON_PaymentsHistory(ourviewmodel,
                                        utilityviewmodel,
                                        htmlDocument,
                                        payments_tempList);
                    // Next routine should be ACCDETLS or LOGOUT if fuelList is empty ..
                    break;
            }
            return true;
        }

        private static async Task<bool> EON_Login(
#if ANDROIDX
                                        AppCompatActivity meterActivity,
#endif
                                        MainViewModel ourviewmodel,
                                        UtilityViewModel utilityviewmodel,
                                        HtmlAgilityPack.HtmlDocument document,
                                        bool TextBox_Active)
        {
            IList<HtmlAgilityPack.HtmlNode>
                    HtmlCol;
            bool clicked = false;
            string id = "",
                    viewstate = "";
            List<KeyValuePair<string, string>> keyValues = new List<KeyValuePair<string, string>>();

            HtmlCol = YetAnotherFuckingHoop.SelectNodesAsList(document.DocumentNode, ".//input");
            foreach (HtmlAgilityPack.HtmlNode element1 in HtmlCol)
            {
                if (!string.IsNullOrEmpty(element1.Id))
                {
                    int element_index = -1;
                    if (element1.Id.IndexOf("EonTextBoxUsername") >= 0)
                    {
                        id = element1.GetAttributeValue("name", "");
                        element_index = 0;
                    }
                    else
                    {
                        if (element1.Id.IndexOf("EonTextBoxPassword") >= 0)
                        {
                            id = element1.GetAttributeValue("name", "");
                            element_index = 1;
                        }
                        else
                        {
                            if (element1.Id.IndexOf("LoginButton") >= 0)
                            {
                                id = element1.GetAttributeValue("name", "");
                                element_index = 2;
                            }
                        }
                    }
                    switch (element_index)
                    {
                        case 0:
                            // Set the user name in the username text box
                            keyValues.Add(new KeyValuePair<string, string>(id, utilityviewmodel.user_id));
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
                            break;
                        case 1:
                            //Type the password in the password text box
                            keyValues.Add(new KeyValuePair<string, string>(id, utilityviewmodel.password));
                            // Hide the password in the log
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
                            }
                            break;
                        case 2:
                            utilityviewmodel.login_attempted = true;
                            // Hook up the next routine - if the login is true - for the next 'Document Completed' delivery
                            keyValues.Add(new KeyValuePair<string, string>(id, "Login"));

                            if (TextBox_Active)
                            {
                                await SmartRoutinesV2018.TextBlockUpdate(
#if ANDROIDX
                                                    meterActivity,
#endif
                                                    ourviewmodel,
                                                    "LogOnAttempted");
                            }
                            utilityviewmodel.next_routine = "HOME";
                            clicked = true;
                            break;
                        default:
                            break;
                    }
                }
                if (clicked)
                {
                    break;
                }
            }

            // Get this one to make the POST work ...
            HtmlAgilityPack.HtmlNode __viewstate = document.GetElementbyId("__VIEWSTATE");
            if (__viewstate != null)
            {
                viewstate = __viewstate.GetAttributeValue("value", "");
            }
            keyValues.Add(new KeyValuePair<string, string>("__VIEWSTATE", viewstate));


            utilityviewmodel.keyValues = keyValues;

            // If our login was successful a 'secure' Document
            // will be delivered.
            // If our login was UNSUCCESSFUL, then no Document
            // will be delivered and the timer will tick away
            // the 60 seconds and exit the timer loop when it
            // reaches  0
            if (!clicked)
            {
                utilityviewmodel.next_routine = "LOGOUT";
            }
            return clicked;
        }

        //        private static bool EON_YourAccount(HtmlAgilityPack.HtmlDocument document,
        //                                        UtilityViewModel utilityviewmodel,
        //                                        rf string your_account_summary_path,
        //                                        rf string view_your_statement_path,
        //                                        rf string meter_reading_path)
        //        {
        //            // Fuck me!  It fucking WORKED!!!! See the re-direction to this 'secure'
        //            // routine from the initial fucking Login routine above!!!!!!!!!!!!
        //            // You ARE a fucking genius Ray!!  An absolute bravest of the brave
        //            // fucking genius.  Those visionless cunts you saw last Thursday have
        //            // ABSOLUTELY no idea what they are missing out on ....

        //            IList<HtmlAgilityPack.HtmlNode>
        //                        HtmlCol1;

        //            bool clicked = true;

        //            HtmlCol1 = YetAnotherFuckingHoop.SelectNodesAsList(document.DocumentNode, "//a");

        //            string asp = if (!EON_Find_Paths(HtmlCol1, "Your account summary", rf your_account_summary_path))
        //            {
        //                clicked = false;
        //            }
        //            if (!EON_Find_Paths(HtmlCol1, "View your statement", rf view_your_statement_path))
        //            {
        //                clicked = false;
        //            }
        //            if (!EON_Find_Paths(HtmlCol1, "Give us a meter reading", rf meter_reading_path))
        //            {
        //                clicked = false;
        //            }
        //            else
        //            {
        //                    utilityviewmodel.target_pathname = meter_reading_path;
        //                    // Hook up the next routine - for the next Object Moved 'Document Completed' delivery
        //                    utilityviewmodel.next_routine = "METERREAD";
        //                    clicked = true;                    

        //            }

        //            SmartUtilityV2022.Find_A_Tag(document,
        //                                        utilityviewmodel,
        //                                        "//a",
        //                                        true,           // only_consider_null_elements,,
        //                                        "",   // remove,
        //                                        "Logout",       // inner_text_comparison,
        //                                        "");  // next_routine
        //            //}
        //            if (!clicked)
        //            {
        //                utilityviewmodel.next_routine = "LOGOUT";
        //            }
        //            return clicked;
        //        }

        private static async Task<bool> EON_AccountDetails(MainViewModel ourviewmodel,
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
                        HtmlCol2;

            string class_name,
                    inner_text;// = "";

            utilityviewmodel.eon_account_info = "";
            utilityviewmodel.eon_account_name = "";
            utilityviewmodel.clicked = false;

            //< div id = "login-register" >
            //< p id = "utility_0_pLoggedIn" class="secure">
            // Mr M Waterfall
            //<a id="utility_0_HyperLinkLogout" href="https://www.eonenergy.com/for-your-home/your-account/Logout"> Logout</a>
            //</p>
            //</div>

            // We only ever want to find each resource_code, account_no,
            // post code and the area id ONCE the first time through...
            // Find the button tab
            HtmlCol1 = YetAnotherFuckingHoop.SelectNodesAsList(document.DocumentNode, "//div");
            foreach (HtmlAgilityPack.HtmlNode element1 in HtmlCol1)
            {
                class_name = element1.GetAttributeValue("class", "");
                switch (class_name)
                {
                    case "eon-button-tabs":
                        HtmlCol2 = YetAnotherFuckingHoop.SelectNodesAsList(element1, ".//input");
                        if (!EON_Button_Tabs(HtmlCol2, utilityviewmodel))
                        {
                            break;
                        }
                        break;
                    case "your-account-summary":
                        HtmlCol2 = YetAnotherFuckingHoop.SelectNodesAsList(element1, ".//*");
                        EON_Your_Account_Summary(ourviewmodel, utilityviewmodel, HtmlCol2);
                        break;
                    case "right-column":
                        HtmlCol2 = YetAnotherFuckingHoop.SelectNodesAsList(element1, ".//*");
                        foreach (HtmlAgilityPack.HtmlNode element2 in HtmlCol2)
                        {
                            switch (element2.Name)
                            {
                                case "h4":
                                    EON_Right_Column_H4(element2, utilityviewmodel);
                                    break;
                                case "p":
                                    //<p class="user-info">8 Outwood House, <nobr>SK8 3AN</nobr></p>
                                    if (string.IsNullOrEmpty(element2.Id))
                                    {
                                        if (!string.IsNullOrEmpty(element2.InnerText))
                                        {
                                            inner_text = element2.InnerText.Replace(Environment.NewLine, "").Trim();
                                            class_name = element2.GetAttributeValue("class", "");
                                            if (class_name.Length >= 9) // Length of user-info
                                            {
                                                class_name = class_name.Substring(0, 9);
                                            }

                                            if (!await EON_Account_Bits_Pieces(ourviewmodel,
                                                                        utilityviewmodel,
                                                                        class_name,
                                                                        inner_text,
                                                                        utilityviewmodel.eon_account_info))
                                            {
                                                goto next_button;
                                            }
                                        }
                                    }
                                    break;
                                case "a":
                                    // Try and find "Your plan(s)"
                                    await EON_Right_Column_A(ourviewmodel,
                                                             utilityviewmodel,
                                                             element2);
                                    break;
                                default:
                                    break;
                            }
                        }
                        break;
                    default:
                        break;
                }
            next_button:
                continue;
            }

            if (utilityviewmodel.clicked)
            {

                HtmlCol1 = YetAnotherFuckingHoop.SelectNodesAsList(document.DocumentNode, "//a");
                //if (!EON_Find_Paths(HtmlCol1, "Your account summary", rf your_account_summary_path))
                //{
                //    clicked = false;
                //}
                string match_path;
                match_path = EON_Find_Paths(HtmlCol1, "View your statement");
                if (!string.IsNullOrEmpty(match_path))
                {
                    utilityviewmodel.view_your_statement_path = match_path;
                }
                match_path = EON_Find_Paths(HtmlCol1, "Give us a meter reading");
                if (!string.IsNullOrEmpty(match_path))
                {
                    utilityviewmodel.meter_readings_path = match_path;
                }
                //else
                //{
                //    utilityviewmodel.target_pathname = meter_reading_path;
                //    // Hook up the next routine - for the next Object Moved 'Document Completed' delivery
                //    utilityviewmodel.next_routine = "METERREAD";
                //    clicked = true;
                //}

                //utilityviewmodel.target_pathname = view_your_statement_path;
                //utilityviewmodel.next_routine = "VIEWSTATEMENT";
                utilityviewmodel.clicked = true;
            }
            if (!utilityviewmodel.clicked)
            {
                utilityviewmodel.next_routine = "LOGOUT";
            }
            return utilityviewmodel.clicked;
        }

        private static string EON_Find_Paths(IList<HtmlAgilityPack.HtmlNode> HtmlCol1,
                                            string match_string)
        {
            foreach (HtmlAgilityPack.HtmlNode element1 in HtmlCol1)
            {
                if (string.IsNullOrEmpty(element1.Id))
                {
                    if (!string.IsNullOrEmpty(element1.InnerText))
                    {
                        string inner_text = element1.InnerText.Replace(Environment.NewLine, "").Trim();
                        if (inner_text.IndexOf(match_string) >= 0)
                        {
                            // Get pathname
                            string href = element1.GetAttributeValue(SmartParametersV2016.href, "");
                            if (!string.IsNullOrEmpty(href))
                            {
                                return href;
                            }
                        }
                    }
                }
            }
            return "";
        }

        private static async Task<bool> EON_Account_Bits_Pieces(MainViewModel ourviewmodel,
                                                    UtilityViewModel utilityviewmodel,
                                                    string class_name,
                                                    string inner_text,
                                                    string account_info)
        {
            //SmartUtility.Accounts account_row = new SmartUtility.Accounts();


            switch (class_name)
            {
                case "user-info":
                    switch (account_info)
                    {
                        case "Account number:":
                            // For Npower you have a separate for Electricity and Gas
                            // But now here for EON you have a combined one
                            utilityviewmodel.account_no = inner_text;

                            // Doesn't matter about the MPAN/MPRN - only ever do one of each E(lectricity) or G(as)
                            // Ignore the MPAN/MPRN even though the original code includes it?
                            List<Amelia> amelia_found = SmartUtilityV2022.Amelia_Lookup(ourviewmodel, utilityviewmodel, false);
                            if (amelia_found.Count > 0)
                            {
                                // This cause goto next_button;
                                return false;
                            }
                            SmartUtilityV2022.Amelia_Add(utilityviewmodel);

                            // Now - we might never know the MPAN if its a new account and
                            // we don't have any information or Bills to update it
                            // But we DO have an account so we need to check:
                            // a) if its already known in the Utility.Accounts_In table
                            // b) if it isn't then b) check if its in the Utility.Accounts_Out table
                            // If it isn't then add it into Utility.Accounts_Out
                            // ...and!  Set the MPAN if it goes in as 'Unknown' to something we have

                            // Other bits and pieces
                            utilityviewmodel.statement_id = "";
                            break;
                        case "Account address:":
                            // Both the PostCode AND the Account are in here ...
                            string account_address = inner_text;
                            account_address = account_address.Replace("'", "");
                            // For when we split the return string

                            // Here is where we try to extract the postcode and all the other bits and pieces
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
                                        utilityviewmodel.clicked = false;
                                        return false;
                                    }
                                }
                                SmartUtilityV2022.Amelia_Update_Postcode_Area(ourviewmodel, utilityviewmodel);

                                // No we can set this because we have a good SUPPLY ADDRESS
                                utilityviewmodel.login_finished = true;

                                // Can do this now we have the area_code
                                if (utilityviewmodel.TARIFF_CODE == 0)
                                {
                                    await SmartUtilityV2022.Lookup_Remote_Tariff_Code(ourviewmodel, utilityviewmodel);
                                }
                            }
                            else
                            {
                                return false;
                            }
                            break;
                        case "Email address:":
                            utilityviewmodel.account_email = inner_text;
                            break;
                        default:
                            break;
                    }
                    break;
                default:
                    break;
            }
            return true;
        }

        private static bool EON_Button_Tabs(IList<HtmlAgilityPack.HtmlNode> HtmlCol2,
                                            UtilityViewModel utilityviewmodel)
        {
            bool found_one = false;
            foreach (HtmlAgilityPack.HtmlNode element2 in HtmlCol2)
            {
                if (!string.IsNullOrEmpty(element2.Id))
                {
                    string value = element2.GetAttributeValue("value", "");
                    // Pick off the energy code
                    if (!string.IsNullOrEmpty(value))
                    {
                        switch (value)
                        {
                            case "Electricity":
                                utilityviewmodel.resource_code = Convert.ToChar(value);
                                //utilityviewmodel.resource_type = "SR";   // Should be set to SR or VR after METERREAD
                                if (!string.IsNullOrEmpty(utilityviewmodel.fuelList))
                                {
                                    utilityviewmodel.fuelList += SmartParametersV2016.bar;
                                }
                                utilityviewmodel.fuelList += utilityviewmodel.resource_code;
                                found_one = true;
                                break;
                            case "Gas":
                                utilityviewmodel.resource_code = Convert.ToChar(value);
                                //utilityviewmodel.resource_type = "SR";   // Should be set to SR after METERREAD
                                if (!string.IsNullOrEmpty(utilityviewmodel.fuelList))
                                {
                                    utilityviewmodel.fuelList += SmartParametersV2016.bar;
                                }
                                utilityviewmodel.fuelList += utilityviewmodel.resource_code;
                                found_one = true;
                                break;
                            default:
                                break;
                        }
                        if (found_one)
                        {
                            break;  // Because we have set a resource
                        }
                    }
                }
            }
            return found_one;
        }

        private static void EON_Your_Account_Summary(MainViewModel ourviewmodel,
                                                UtilityViewModel utilityviewmodel,
                                                IList<HtmlAgilityPack.HtmlNode> HtmlCol2)
        {
            string your_account_balance = "Your account balance at your last bill was ",
                    your_last_bill = "Your last bill was sent ",
                    you_currently_receive = "You currently receive bills every ",
                    you_currently_pay = "You currently pay by ";

            foreach (HtmlAgilityPack.HtmlNode element2 in HtmlCol2)
            {
                if (string.IsNullOrEmpty(element2.Id))
                {
                    if (!string.IsNullOrEmpty(element2.InnerText))
                    {
                        string inner_text = element2.InnerText.Replace(Environment.NewLine, "").Trim();
                        switch (element2.Name)
                        {
                            case "h2":
                                utilityviewmodel.eon_account_name = inner_text.Replace("Hi", "").Trim();
                                break;
                            case "p":
                                int summary_index;// = -1;
                                summary_index = inner_text.IndexOf(your_account_balance);
                                if (summary_index >= 0)
                                {
                                    //outstanding_balance = inner_text.Replace(your_account_balance, "");
                                }
                                else
                                {
                                    summary_index = inner_text.IndexOf(your_last_bill);
                                    if (summary_index >= 0)
                                    {
                                        //last_bill_date = inner_text.Replace(your_last_bill, "");
                                    }
                                    else
                                    {
                                        summary_index = inner_text.IndexOf(you_currently_receive);
                                        if (summary_index >= 0)
                                        {
                                            //statement_frequency = inner_text.Replace(you_currently_receive, "");
                                        }
                                        else
                                        {
                                            summary_index = inner_text.IndexOf(you_currently_pay);
                                            if (summary_index >= 0)
                                            {
                                                utilityviewmodel.payment_name = inner_text.Replace(you_currently_pay, "");
                                                string PAYMENT_TYPE = utilityviewmodel.payment_name;
                                                // Find a Payment Plan which contains this Payment Name
                                                if (SmartSpikeUtilityV2017.Utility_Lookup_PaymentPlan(ourviewmodel,
                                                                                            utilityviewmodel,
                                                                                            PAYMENT_TYPE))
                                                {
                                                    SmartParseV2016.Update_Bill(ourviewmodel, utilityviewmodel, "PAYMENT_PLAN", utilityviewmodel.PAYMENT_PLAN.ToString());
                                                    SmartParseV2016.Update_Bill(ourviewmodel, utilityviewmodel, "PAYMENT_TYPE", PAYMENT_TYPE);
                                                }
                                            }
                                        }
                                    }
                                }
                                break;
                            default:
                                break;
                        }
                    }
                }
            }
            return;
        }

        private static void EON_Right_Column_H4(HtmlAgilityPack.HtmlNode element2,
                                                UtilityViewModel utilityviewmodel)
        {
            if (string.IsNullOrEmpty(element2.Id))
            {
                if (!string.IsNullOrEmpty(element2.InnerText))
                {
                    string inner_text = element2.InnerText.Replace(Environment.NewLine, "").Trim();
                    switch (inner_text)
                    {
                        case "Account number:":
                            utilityviewmodel.eon_account_info = inner_text;
                            // we wouldn't be HERE unless we were logged in successfully
                            // Set this flag as were are 99.9% sure that we are logged-in
                            // NO - when we get a good SUPPLY ADDRESS
                            break;
                        case "Account address:":
                            utilityviewmodel.eon_account_info = inner_text;
                            break;
                        case "Email address:":
                            utilityviewmodel.eon_account_info = inner_text;
                            break;
                        default:
                            break;
                    }
                }
            }
            return;
        }

        private static async Task<bool> EON_Right_Column_A(MainViewModel ourviewmodel,
                                                     UtilityViewModel utilityviewmodel,
                                                     HtmlAgilityPack.HtmlNode element2)
        {
            // Try and find "Your plan(s)"
            string class_name = element2.GetAttributeValue("class", "");
            if (!string.IsNullOrEmpty(element2.InnerText))
            {
                bool found_it = false;
                utilityviewmodel.eon_tariff_name = element2.InnerText.Replace(Environment.NewLine, "").Trim();
                switch (class_name)
                {
                    case "electricity":
                        // Plan
                        utilityviewmodel.TARIFF_NAME = SmartUtilityV2022.Filter_Tariffname(true, utilityviewmodel.eon_tariff_name);
                        found_it = true;
                        break;
                    case "gas":
                        // Plan
                        utilityviewmodel.TARIFF_NAME = SmartUtilityV2022.Filter_Tariffname(true, utilityviewmodel.eon_tariff_name);
                        found_it = true;
                        break;
                    default:
                        break;
                }
                if (found_it)
                {
                    if (await SmartUtilityV2022.Lookup_Remote_Tariff_Code(ourviewmodel, utilityviewmodel))
                    {
                        SmartParseV2016.Update_Bill(ourviewmodel, utilityviewmodel, "TARIFF_CODE", utilityviewmodel.TARIFF_CODE.ToString());
                    }
                    else
                    {
                        return false; // ourviewmodel.errorMessage should be set here
                    }
                }
            }
            return true;
        }

        private static bool EON_MeterReadings(UtilityViewModel utilityviewmodel,
                                                HtmlAgilityPack.HtmlDocument document)
        //#endif
        {
            // Fuck me!  It fucking WORKED!!!! See the re-direction to this 'secure'
            // routine from the initial fucking Login routine above!!!!!!!!!!!!
            // You ARE a fucking genius Ray!!  An absolute bravest of the brave
            // fucking genius.  Those visionless cunts you saw last Thursday have
            // ABSOLUTELY no idea what they are missing out on ....

            IList<HtmlAgilityPack.HtmlNode>
                        HtmlCol1,
                        HtmlCol2,
                        HtmlCol3;

            string class_name,
                    inner_text,
                    Meter_number = "Meter number:";
            //account_info = "",
            //account_name = "";

            bool clicked = false;

            //< div id = "login-register" >
            //< p id = "utility_0_pLoggedIn" class="secure">
            // Mr M Waterfall
            //<a id="utility_0_HyperLinkLogout" href="https://www.eonenergy.com/for-your-home/your-account/Logout"> Logout</a>
            //</p>
            //</div>

            // We only ever want to find each resource_code, account_no,
            // post code and the area id ONCE the first time through...
            // Find the button tab
            HtmlCol1 = YetAnotherFuckingHoop.SelectNodesAsList(document.DocumentNode, ".//div");
            foreach (HtmlAgilityPack.HtmlNode element1 in HtmlCol1)
            {
                class_name = element1.GetAttributeValue("class", "");
                if (SmartNibbyV2016.Check_Classname(class_name, "meter-reading-product", true))
                {
                    HtmlCol2 = YetAnotherFuckingHoop.SelectNodesAsList(element1, ".//div");
                    foreach (HtmlAgilityPack.HtmlNode element2 in HtmlCol2)
                    {
                        class_name = element2.GetAttributeValue("class", "");
                        if (SmartNibbyV2016.Check_Classname(class_name, "meter-reading-meter", true))
                        {
                            HtmlCol3 = YetAnotherFuckingHoop.SelectNodesAsList(element2, ".//*");
                            foreach (HtmlAgilityPack.HtmlNode element3 in HtmlCol3)
                            {
                                switch (element3.Name)
                                {
                                    case "p":
                                        class_name = element3.GetAttributeValue("class", "");
                                        if (class_name == "title")
                                        {
                                            inner_text = element3.InnerText;
                                            if (!string.IsNullOrEmpty(inner_text))
                                            {
                                                inner_text = inner_text.Replace(Meter_number, "").Trim();
                                                //#if WINFORMS
                                                //                                                if (utilityviewmodel.console)
                                                //                                                {
                                                //                                                    await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, Meter_number + inner_text);
                                                //                                                }
                                                //#endif
                                            }
                                        }
                                        break;
                                    case "div":
                                        class_name = element3.GetAttributeValue("class", "");
                                        if (class_name == "meter-reading-message")
                                        {
                                            inner_text = element3.InnerText;
                                            if (!string.IsNullOrEmpty(inner_text))
                                            {
                                                if (inner_text.IndexOf("for the low") >= 0)
                                                {
                                                    SmartUtilityV2022.Fix_Resource_Type(utilityviewmodel, "VR");
                                                    clicked = true;
                                                }
                                                else
                                                {
                                                    if (inner_text.IndexOf("for the normal") >= 0)
                                                    {
                                                        SmartUtilityV2022.Fix_Resource_Type(utilityviewmodel, "SR");
                                                        clicked = true;
                                                    }
                                                }
                                            }
                                        }
                                        break;
                                    default:
                                        break;
                                }
                                if (clicked)
                                {
                                    goto the_old_days;
                                }
                            }
                        }
                    }
                }
            }

        the_old_days:
            if (clicked)
            {
                // Hook up the next routine - for the next Object Moved 'Document Completed' delivery
                utilityviewmodel.target_pathname = utilityviewmodel.your_account_summary_path;
                utilityviewmodel.next_routine = "ACCDETLS";
                clicked = true;
            }
            if (!clicked)
            {
                utilityviewmodel.next_routine = "LOGOUT";
            }
            return clicked;
        }

        // We get the bills HERE!!!!
        private static async Task<bool> EON_ViewYourStatement(
#if ANDROIDX
                                                AppCompatActivity meterActivity,
#endif
                                                MainViewModel ourviewmodel,
                                                UtilityViewModel utilityviewmodel,
                                                HtmlAgilityPack.HtmlDocument htmlDocument,
                                                bool TextBox_Active)
        {
            // The way the Eon Foreigners do this, is that they have all the bills in 
            // a drop down and you have to 'Select' one of them (i.e. do a POST)
            // and then you have to 'Download the paper bill' (when it appears)
            // i.e. you have to do a PDF_READ.

            IList<HtmlAgilityPack.HtmlNode>
                        HtmlCol1,
                        HtmlCol2;

            bool clicked = true;

            // We can get the bills HERE!!!!
            HtmlCol1 = YetAnotherFuckingHoop.SelectNodesAsList(htmlDocument.DocumentNode, "//div");
            foreach (HtmlAgilityPack.HtmlNode element1 in HtmlCol1)
            {
                if (string.IsNullOrEmpty(element1.Id))
                {
                    string class_name = element1.GetAttributeValue("class", "");
                    if (SmartNibbyV2016.Check_Classname(class_name, "bill-selector", true))
                    {
                        if (!string.IsNullOrEmpty(element1.InnerText))
                        {
                            // All because the "options" don't have an InnerText !!
                            string dateList = element1.InnerText;
                            // Clean it up
                            dateList = dateList.Replace(Environment.NewLine, "").Trim();
                            dateList = dateList.Replace("View another bill?", "").Trim();
                            //Fucking noisy bitch
                            string[] datesList = dateList.Split(SmartParametersV2016.tab);
                            int date_count = 0;

                            // This is the main 'Select' code so each 'value' is a drop-down selection
                            // In actuality it looks like this:
                            //
                            // < div class="bill-selector">
                            //      <div class="form-question">
                            //          <label id = "rootcontent_0_maincolumn_0_ucBillList_lblBillList" class="accessible-friendly-hide" for="ddlBillList">View another bill?</label>
                            //          <select name = "rootcontent_0$maincolumn_0$ucBillList$ddlBillList" id="ddlBillList">
                            //          <option selected = "selected" value="-1">View another bill?</option>
                            //          <option value = "531482011">04/12/2016</option>
                            //          <option value = "522542697">14/09/2016</option>
                            //          <option value = "510605549">26/05/2016</option>
                            //          <option value = "501775949">09/03/2016</option>
                            //          <option value = "490730393">01/12/2015</option>
                            //          <option value = "481200857">04/09/2015</option>
                            //          <option value = "470596247">02/06/2015</option>
                            //          <option value = "460748155">03/03/2015</option>
                            //          </select>


                            HtmlCol2 = YetAnotherFuckingHoop.SelectNodesAsList(element1, ".//option");
                            foreach (HtmlAgilityPack.HtmlNode element2 in HtmlCol2)
                            {
                                string option_value = element2.GetAttributeValue("value", "");
                                if (option_value != "-1")
                                {
                                    string webpage_id = datesList[date_count];
                                    // Just for compatability with British Gas
                                    if (!SmartParseV2016.Generic_Parse_Datetime(webpage_id, utilityviewmodel))
                                    {
                                        // This should make sure we ignore this Date / this line on a date conversion failure
                                        clicked = false;
                                        ourviewmodel.warningMessage = "Some viewbills DateTime problem occurred";
                                        goto quit_viewbills;
                                    }
                                    else
                                    {
                                        utilityviewmodel.bill_date = utilityviewmodel.genericTargetDate;
                                    }

                                    utilityviewmodel.statement_id = webpage_id;
                                    if (!await Viewbills_Case(
#if ANDROIDX
                                                            meterActivity,
#endif
                                                            ourviewmodel,
                                                             utilityviewmodel,
                                                             htmlDocument,
                                                            TextBox_Active,
                                                            option_value))
                                    {
                                        clicked = false;
                                        ourviewmodel.warningMessage = "Some viewbills problem occurred";
                                        goto quit_viewbills;
                                    }
                                    date_count++;
                                }
                            }
                        }
                    }
                }
            }

            // This bit happens AT THE END
#if WINFORMS
            clicked = await SmartNibbyV2016.Find_A_Tag(ourviewmodel,
#endif
#if WPF  || WINUI || SMARTMAUI
            clicked = SmartNibbyV2016.Find_A_Tag(ourviewmodel,
#endif
#if ANDROIDX
            clicked = SmartNibbyV2016.Find_A_Tag(ourviewmodel,
#endif
                                        htmlDocument,
                                        utilityviewmodel,
                                        "//a",
                                        true,                           // only_consider_null_elements,,
                                        "",                   // remove,
                                        "billing and payments history", // inner_text_comparison,
                                        "PAYHIST");                     // next_routine

        quit_viewbills:
            if (!clicked)
            {
                utilityviewmodel.next_routine = "LOGOUT";
            }
            return clicked;
        }

        private static async Task<bool> Viewbills_Case(
#if ANDROIDX
                                                AppCompatActivity meterActivity,
#endif
                                                MainViewModel ourviewmodel,
                                                UtilityViewModel utilityviewmodel,
                                                HtmlAgilityPack.HtmlDocument htmlDocument,
                                                bool TextBox_Active,
                                                string option_value)
        {
            // Send this for EACH BILL:
            //
            // POST https://www.eonenergy.com/for-your-home/your-account/billing-and-payments/detailed-bill HTTP/1.1
            //
            // Then get this for EACH BILL:
            //
            // GET https://www.eonenergy.com/for-your-home/your-account/billing-and-payments/download-paper-bill HTTP/1.1

            IList<HtmlAgilityPack.HtmlNode>
                        HtmlCol3;

            //string viewstate;// = "";

            if (SmartUtilityV2022.Check_Bill_Date(utilityviewmodel))
            {
                // Have we already done this bill in a previous Read Meter?
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
                    List<KeyValuePair<string, string>> keyValues = new List<KeyValuePair<string, string>>();

                    string viewstate = "";
                    // Get this one to make the POST work ...
                    HtmlAgilityPack.HtmlNode __viewstate = htmlDocument.GetElementbyId("__VIEWSTATE");
                    if (__viewstate != null)
                    {
                        viewstate = __viewstate.GetAttributeValue("value", "");
                    }

                    keyValues.Add(new KeyValuePair<string, string>("__VIEWSTATE", viewstate));
                    keyValues.Add(new KeyValuePair<string, string>("rootcontent_0$maincolumn_0$ucBillList$ddlBillList", option_value));    // value here is the Bill document id
                    keyValues.Add(new KeyValuePair<string, string>("rootcontent_0$maincolumn_0$ucBillList$btnSelectBill", "Select"));

                    utilityviewmodel.target_pathname = utilityviewmodel.target_pathname.Replace(utilityviewmodel.prfix_xxx, "");
                    HtmlAgilityPack.HtmlDocument post_htmlDocument = await SmartBobV2017.Scraper_Generic_Post(ourviewmodel, utilityviewmodel.utilityToken, keyValues, utilityviewmodel);
                    if (!string.IsNullOrEmpty(ourviewmodel.errorMessage) ||
                            (htmlDocument.RemainderOffset == 0))
                    {
                        ourviewmodel.errorMessage = utilityviewmodel.current_routine + SmartParametersV2016.space + ourviewmodel.errorMessage;
                        return false;
                    }

                    HtmlCol3 = YetAnotherFuckingHoop.SelectNodesAsList(post_htmlDocument.DocumentNode, "//a");
                    foreach (HtmlAgilityPack.HtmlNode element3 in HtmlCol3)
                    {
                        string class_name = element3.GetAttributeValue("class", "");
                        if (class_name == "download-bill")
                        {
                            if (!await Download_Determine_UDPRN(
#if ANDROIDX
                                                        meterActivity,
#endif
                                                        ourviewmodel,
                                                        utilityviewmodel,
                                                        element3,
                                                        TextBox_Active))
                            {
                                return false;
                            }
                            // No more <a> tags now please ..
                            break;
                        }
                    }
                }
            }
            return true;
        }

        // POST https://www.eonenergy.com/for-your-home/your-account/billing-and-payments/history HTTP/1.1

        //
        // Then get this for EACH BILL:
        // GET https://www.eonenergy.com/for-your-home/your-account/billing-and-payments/details HTTP/1.1
        // Find the download-bills link in:
        // <a href="https://www.eonenergy.com/for-your-home/your-account/billing-and-payments/download-paper-bill" id="rootcontent_0_maincolumn_0_ucViewBillButtons_lnkDownloadbill" class="download-bill">
        // and then 
        // GET https://www.eonenergy.com/for-your-home/your-account/billing-and-payments/download-paper-bill HTTP/1.1


        private static async Task<bool> Download_Determine_UDPRN(
#if ANDROIDX
                                                        AppCompatActivity meterActivity,
#endif
                                                        MainViewModel ourviewmodel,
                                                        UtilityViewModel utilityviewmodel,
                                                        HtmlAgilityPack.HtmlNode element3,
                                                        bool TextBox_Active)
        {
            bool status = true;

            string href = element3.GetAttributeValue(SmartParametersV2016.href, "");

            // We haven't already done this bill in this current Read Meter
            Uri TargetUrl_pdf = new Uri(href); // DON'T re-use TargetUrl - it doesn't work!!!

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
                                                        TargetUrl_pdf,
                                                        utilityviewmodel.guid);
            if (!string.IsNullOrEmpty(ourviewmodel.errorMessage) || !string.IsNullOrEmpty(ourviewmodel.pdfMessage))
            {
                return false;
            }
            if (await SmartPDFV2019.Generic_Close_1(

                                                    ourviewmodel,
                                                    utilityviewmodel,
#if ANDROIDX
                                                    meterActivity,
#endif
                                                    pdfreader,
                                                    TextBox_Active))
            {
                if (!await EON_parse_bill(ourviewmodel,
                                            utilityviewmodel,
                                            pdfreader))
                {
#if WINFORMS
                    await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, " Parse failed: " + utilityviewmodel.account_no + SmartParametersV2016.space +
                                                                                                    utilityviewmodel.statement_id + SmartParametersV2016.space +
                                                                                                    utilityviewmodel.bill_date + SmartParametersV2016.space +
                                                                                                    utilityviewmodel.account_type + SmartParametersV2016.space +
                                                                                                    ourviewmodel.errorMessage + Environment.NewLine.ToString());
#endif
#if WPF  || SMARTMAUI
                    await SmartUtilityV2022.Check_Examine(ourviewmodel, utilityviewmodel);
#endif
#if ANDROIDX
                    await SmartUtilityV2022.Check_Examine(ourviewmodel, utilityviewmodel);
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
            // Close the reader down (at last) here
#if ANDROIDX
            pdfreader?.Close();
#endif
            return status;
        }

        private static bool EON_PaymentsHistory(MainViewModel ourviewmodel,
                                                        UtilityViewModel utilityviewmodel,
                                                        HtmlAgilityPack.HtmlDocument document,
                                                        List<SmartUtility.Payments> payments_tempList)
        {
            // This routine was an ABSOLUTE AND UTTER BITCH which took me over seven
            // days of pain, torment and hair-tearing before I finally worked
            // out a solution simply by trial and error and 700 or 800? different attempts
            // to get it working ....  And all I wanted to do was 'press' the middle
            // "Bills" tab in the Account History screen and see the list of bills ...
            // This one problem alone almost killed me - I can't take much more of this
            // I cannot keep hitting these brick walls after 2,500 hours of coding; I can
            // hit them after 25 hours, no problem, but not after 11 months ....
            // After 8 days, I eventually gave up trying to work with the 'Bills" tab - it
            // appears that these E-On cunts have used an Ajax UpdatePanel grid in the
            // middle of the screen, which does an update of the information without doing
            // a postback or anything I can trap like a 'Document Completed' event.
            // Finally, on the brink of suicide, I saw that it was possible to click the
            // 'Next' button and after a 'Refresh' of the page - it stayed the same!  Even after
            // a logout and login, it would still show you Page 2 or Page 3 of the Account
            // History if that was the last page you had been looking at.  My feeble attempts
            // at an HttpWebRequest 'POST' all came to naught, so in desperation I used
            // GoBack(-1) to go back to the Account Details page after clicking 'Next;, and
            // letting that take me back onto the Utility.Accounts History page.  What a bitch.  What a
            // fag it was.  What a bunch of cunts E-On are .....

            IList<HtmlAgilityPack.HtmlNode>
                    HtmlCol1,
                    HtmlCol2;

            string class_name;

            //DateTime payment_date = SmartParametersV2016.defaultDate;
            utilityviewmodel.PAYMENT_DATE = SmartParametersV2016.defaultDates;
            short PAYMENT_CODE = 0;
            utilityviewmodel.PAYMENT_AMOUNT = "";
            utilityviewmodel.PAYMENT_BALANCE = "";
            short payment_item = 0;

            bool clicked = false;

            // This 'click' approach doesn't work!  We don't get a document delivered
            // so we can't intercept it; this 'tab' is changed IN PLACE so we have to
            // use 'POST' as the 'click' and 'GET' as the document delivery ...
            HtmlCol1 = YetAnotherFuckingHoop.SelectNodesAsList(document.DocumentNode, "//div");
            foreach (HtmlAgilityPack.HtmlNode element1 in HtmlCol1)
            {
                if (!string.IsNullOrEmpty(element1.Id))
                {
                    if (element1.Id == "allBillingAndPaymentsTransactionsDiv")
                    {
                        clicked = true;
#if WINFORMS
                        //if (utilityviewmodel.console)
                        //{
                        //    await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Date    Credit    Debit    Balance ");
                        //}
#endif
                        HtmlCol2 = YetAnotherFuckingHoop.SelectNodesAsList(element1, ".//tr");
                        foreach (HtmlAgilityPack.HtmlNode element2 in HtmlCol2)
                        {
                            class_name = element2.GetAttributeValue("class", "");
                            if (SmartNibbyV2016.Check_Classname(class_name, "transaction-row", true))
                            {
                                if (!string.IsNullOrEmpty(element2.InnerText))
                                {
                                    if (!EON_Split_Payments(ourviewmodel,
                                                utilityviewmodel,
                                                element2))
                                    {
                                        return false;
                                    }
#if WINFORMS
                                    //if (utilityviewmodel.console)
                                    //{
                                    //    await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, PAYMENT_DATE + "   " + PAYMENT_CODE + "  " + PAYMENT_AMOUNT + "   " + PAYMENT_BALANCE);
                                    //}
#endif
                                    if ((utilityviewmodel.PAYMENT_DATE != SmartParametersV2016.defaultDates) &&
                                        (utilityviewmodel.PAYMENT_AMOUNT != ""))
                                    {
                                        payment_item = (short)(payment_item + 1);
                                        if (!SmartParseV2016.Check_TempList(ourviewmodel,
                                                                            utilityviewmodel,
                                                                            Convert.ToDateTime(utilityviewmodel.PAYMENT_DATE),
                                                                            payment_item,
                                                                            PAYMENT_CODE,
                                                                            Convert.ToInt32(utilityviewmodel.PAYMENT_AMOUNT),
                                                                            Convert.ToInt32(utilityviewmodel.PAYMENT_BALANCE),
                                                                            payments_tempList))
                                        {
                                            // BESPOKE not BESTOKE you numbskull
                                            // Sometiutilityviewmodel...we haven't had any bills and this Statement Id field
                                            // is just ... empty;
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
                                            //                                    PAYMENT_CODE,
                                            //                                    PAYMENT_AMOUNT,
                                            //                                    PAYMENT_BALANCE);

                                            foreach (SmartUtility.Payments payment_row in payments_tempList)
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
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            // If there are no more transactions to do, then we have finished completely
            // and can go back to the Account Details
            if (clicked)
            {
                // Rebuild the fuel list to exclude the one we are 'on'
                utilityviewmodel.fuelList = utilityviewmodel.fuelList.Replace(utilityviewmodel.resource_code.ToString(), "");
                utilityviewmodel.fuelList = utilityviewmodel.fuelList.Trim(SmartParametersV2016.bar);
                if (!string.IsNullOrEmpty(utilityviewmodel.fuelList))
                {
                    utilityviewmodel.target_pathname = utilityviewmodel.your_account_summary_path;
                    // Hook up the next routine - for the next Object Moved 'Document Completed' delivery
                    utilityviewmodel.next_routine = "ACCDETLS";
                }
                else
                {
                    clicked = false;    // TAKE THIS OUT WHEN YOU FIGURE OUT HOW TO CYCLE ROUND ON FUELList
                }
            }
            if (!clicked)
            {
                utilityviewmodel.next_routine = "LOGOUT";
            }
            return clicked;
        }

        private static bool EON_Split_Payments(MainViewModel ourviewmodel,
                                                UtilityViewModel utilityviewmodel,
                                                HtmlAgilityPack.HtmlNode element2)
        {
            string transaction_line = element2.InnerText.Replace(Environment.NewLine, "").Trim();
            string[] transactions = transaction_line.Split(SmartParametersV2016.tab);
            int span_index = 0;
            foreach (string transaction in transactions)
            {
                string inner_text = transaction.Trim();
                switch (span_index)
                {
                    case 0:
                        string payment_date = inner_text.Replace(SmartParametersV2016.space, SmartParametersV2016.dash);
                        if (!SmartParseV2016.Generic_Parse_Datetime(payment_date, utilityviewmodel))
                        {
                            return false;
                        }
                        else
                        {
                            utilityviewmodel.PAYMENT_DATE = payment_date;
                        }
                        break;
                    case 1:
                        string PAYMENT_METHOD = inner_text;
                        if (!string.IsNullOrEmpty(PAYMENT_METHOD))
                        {
                            if (!SmartSpikeUtilityV2017.Utility_Lookup_PaymentCode(ourviewmodel,
                                                                            utilityviewmodel,
                                                                            PAYMENT_METHOD)) //rf PAYMENT_CODE
                            {
                                return false;
                            }
                            PAYMENT_METHOD = utilityviewmodel.PAYMENT_METHOD;
                        }
                        break;
                    case 2:
                        if (inner_text == "-")
                        {
                            utilityviewmodel.PAYMENT_AMOUNT = "";
                        }
                        else
                        {
                            string payment_amount = inner_text.Replace(utilityviewmodel.bill_currency_symbol, "");
                            payment_amount = payment_amount.Replace(utilityviewmodel.bill_currency_separator.ToString(), "");
                            payment_amount = payment_amount.Replace(utilityviewmodel.bill_thousands_separator.ToString(), "");
                            if (!SmartParseV2016.Generic_Parse_Integer(payment_amount, utilityviewmodel))
                            {
                                return false;
                            }
                            else
                            {
                                utilityviewmodel.PAYMENT_AMOUNT = payment_amount;
                            }
                        }
                        break;
                    case 3:
                        if (inner_text == "-")
                        {
                            utilityviewmodel.PAYMENT_AMOUNT = "";
                        }
                        else
                        {
                            string payment_amount = inner_text.Replace(utilityviewmodel.bill_currency_symbol, "");
                            payment_amount = payment_amount.Replace(utilityviewmodel.bill_currency_separator.ToString(), "");
                            payment_amount = payment_amount.Replace(utilityviewmodel.bill_thousands_separator.ToString(), "");
                            if (!SmartParseV2016.Generic_Parse_Integer(payment_amount, utilityviewmodel))
                            {
                                return false;
                            }
                            else
                            {
                                utilityviewmodel.PAYMENT_AMOUNT = payment_amount;
                            }
                        }
                        break;
                    case 4:
                        string payment_balance = inner_text.Replace(utilityviewmodel.bill_currency_symbol, "");
                        payment_balance = payment_balance.Replace(utilityviewmodel.bill_currency_separator.ToString(), "");
                        payment_balance = payment_balance.Replace(utilityviewmodel.bill_thousands_separator.ToString(), "");
                        if (payment_balance.IndexOf(" CR") >= 0)
                        {
                            // A Payment is a Credit which SUBTRACTS from the amount owed ...
                            payment_balance = payment_balance.Replace(" CR", "");
                            payment_balance = "-" + payment_balance;
                        }
                        if (payment_balance.IndexOf(" DR") >= 0)
                        {
                            // A Debit Payment ADDS to the amount owed because the payment bounced ...
                            payment_balance = payment_balance.Replace(" DR", "");
                        }

                        if (!SmartParseV2016.Generic_Parse_Integer(payment_balance, utilityviewmodel))
                        {
                            return false;
                        }
                        else
                        {
                            utilityviewmodel.PAYMENT_BALANCE = payment_balance;
                        }
                        break;
                    default:
                        break;
                }
                span_index++;
            }
            return true;
        }

        internal static async Task<bool> EON_parse_bill(MainViewModel ourviewmodel,
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

            strText_simple = SmartPDFV2019.TurnPdfToText(ourviewmodel, utilityviewmodel, pdfreader);
            if (!string.IsNullOrEmpty(ourviewmodel.errorMessage))
            {
                return false;
            }

            if (!SmartParseV2016.Check_Big_Four(ourviewmodel, utilityviewmodel, false))
            {
                string[] lines = strText_simple.Split(SmartParametersV2016.newline).Where(r => !string.IsNullOrWhiteSpace(r)).ToArray();

                utilityviewmodel.sections = new[]{
                                            new string[]    { "A", "0", "", "", "0", "0", "" },
                                            new string[]    { "B", "0", "", "Electricity statement" + "|" +
                                                                "Gas statement" + "|" +
                                                                "Electricity statement - estimated" + "|" +
                                                                "Gas statement - estimated", "-1", "-1", "" },
                                           new string[]      { "B", "1", "", "Before this statement", "-1", "-1", "" },
                                           new string[]      { "B", "2", "", "On this statement", "-1", "-1", "" },
                                           new string[]      { "B", "3", "", "Your new balance is" + "|" +
                                                                                "Your credit balance is", "-1", "-1", ""},
                                           new string[]      { "C", "0", "", "About your tariff", "-1", "-1", "" },
                                           new string[]      { "H", "0", "", "Meter readings", "-1", "-1", "" },
                                           new string[]      { "H", "1", "E", "Electricity readings", "-1", "-1", "" },
                                           new string[]      { "H", "2", "G", "Gas readings", "-1", "-1", "" },
                                           new string[]      { "H", "3", "E", "Electricity charges", "-1", "-1", SmartParametersV2016.sectionsExactMatch },
                                           new string[]      { "H", "4", "G", "Gas charges", "-1", "-1", SmartParametersV2016.sectionsExactMatch },
                                           new string[]      { "H", "5", "", "Total charges", "-1", "-1", "" }
                                       };
                // This is always needed anyway because we always do Page1 with this
                // Sometimes ... the Bill Date is never extracted from the PDF text!
                // But when (if) we evert find it - we substitute the Bill Number for it
                // IF we haven't found the Bill number ...
                bool success = Parse_Page0(ourviewmodel,
                                                utilityviewmodel,
                                                lines);
                if (!success ||
                    !SmartParseV2016.Check_Big_Four(ourviewmodel, utilityviewmodel, false))
                {
                    status = false;
                }
                else
                {
                    if (!await Parse_Pages(ourviewmodel,
                                            utilityviewmodel,
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
                // fucking bitch is talking about her fucking job AGAIN
                // i don't fucking care about fucking verity or fucking aileish
            }
            return status;
        }
        private static bool Parse_Page0(MainViewModel ourviewmodel,
                                        UtilityViewModel utilityviewmodel,
                                        string[] lines)
        {
            // Not perfect but not a bad start
            bool status = true;

            // Very Important Routine!!!
            SmartParseV2016.Sort_Sections(false, utilityviewmodel, lines);

            // Pass 1 - look for the Big Three in Section 0
            if (!Parse_SectionA0(ourviewmodel, utilityviewmodel, utilityviewmodel.sections, "A", "0", lines))
            {
                ourviewmodel.errorMessage = "A0" + SmartParametersV2016.bar + ourviewmodel.errorMessage;
                return status;
            }
            return status;
        }

        // 27-Jan-2016  My ignorant, rude, petulant, insufferable bitch of a 'wife'
        // got her arse well and truly out last night stomping and thumping around the bedroom
        // in her usual thick, stupid, obnoxious way.  ODSBD and I wont have to put up with
        // her tantrums anymore ... ODSBD ... I can't wait ...

        // 01-Jun-2016  There she was at it again thumping the bed, stomping round the room
        // cursing me under her breath.  I will never, ever, forgive that fucking cow.
        // She is downright ugly and horrible and REVELS in her own stupidity.  She is
        // HAPPY to be stupid!  I cannot believe how she despises knowledge and facts
        // and how she trumpets her own feeble education but wouldn't pay a penny to help
        // anyone 
        private static async Task<bool> Parse_Pages(MainViewModel ourviewmodel,
                                         UtilityViewModel utilityviewmodel,
                                         string[] lines)
        {
            // Not perfect but not a bad start

            bool status = true;

            if (!Parse_SectionB0(utilityviewmodel, utilityviewmodel.sections, "B", "0"))
            {
                ourviewmodel.errorMessage = "B0" + SmartParametersV2016.bar + ourviewmodel.errorMessage;
                return false;
            }
            if (!Parse_SectionB1(ourviewmodel, utilityviewmodel, utilityviewmodel.sections, "B", "1", lines))
            {
                ourviewmodel.errorMessage = "B1" + SmartParametersV2016.bar + ourviewmodel.errorMessage;
                return false;
            }
            if (!Parse_SectionB2(ourviewmodel, utilityviewmodel, utilityviewmodel.sections, "B", "2", lines))
            {
                ourviewmodel.errorMessage = "B2" + SmartParametersV2016.bar + ourviewmodel.errorMessage;
                return false;
            }
            if (!Parse_SectionB3(ourviewmodel, utilityviewmodel, utilityviewmodel.sections, "B", "3", lines))
            {
                ourviewmodel.errorMessage = "B3" + SmartParametersV2016.bar + ourviewmodel.errorMessage;
                return false;
            }
            if (!await Parse_SectionC0(ourviewmodel, utilityviewmodel, utilityviewmodel.sections, "C", "0", lines))
            {
                ourviewmodel.errorMessage = "C0" + SmartParametersV2016.bar + ourviewmodel.errorMessage;
                return false;
            }
            if (!Parse_SectionH0(utilityviewmodel, utilityviewmodel.sections, "H", "0", lines))
            {
                ourviewmodel.errorMessage = "H0" + SmartParametersV2016.bar + ourviewmodel.errorMessage;
                return false;
            }
            if (!Parse_SectionH1(utilityviewmodel, utilityviewmodel.sections, "H", "1", lines)) // E Readings
            {
                ourviewmodel.errorMessage = "H1" + SmartParametersV2016.bar + ourviewmodel.errorMessage;
                return false;
            }
            if (!Parse_SectionH2(ourviewmodel, utilityviewmodel, utilityviewmodel.sections, "H", "2", lines)) // G Readings
            {
                ourviewmodel.errorMessage = "H2" + SmartParametersV2016.bar + ourviewmodel.errorMessage;
                return false;
                // Fucking silly cow cannot add 3 to 2014!!
            }
            if (!await Parse_SectionH3(ourviewmodel, utilityviewmodel, utilityviewmodel.sections, "H", "3", lines)) // E Charges
            {
                ourviewmodel.errorMessage = "H3" + SmartParametersV2016.bar + ourviewmodel.errorMessage;
                return false;
            }
            if (!Parse_SectionH4(utilityviewmodel, utilityviewmodel.sections, "H", "4", lines)) // G Charges
            {
                ourviewmodel.errorMessage = "H4" + SmartParametersV2016.bar + ourviewmodel.errorMessage;
                return false;
            }
            // The supply number might be in here,...
            if (!Parse_SectionH5(utilityviewmodel, utilityviewmodel.sections, "H", "5")) // Total chargconditions_payemes
            {
                ourviewmodel.errorMessage = "H5" + SmartParametersV2016.bar + ourviewmodel.errorMessage;
                return false;
            }
            return status;
        }

        // One day .. no more half-cooked fih, no more sweet potatoes, no more pork and
        // ... no more fucking Archers on ANY fucking radio ... One day ...
        private static bool Parse_SectionA0(MainViewModel ourviewmodel,
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

            string token,
                    your_account_number = "Your account number",
                    statement_date = "Date";
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
                    PAYMENT_DUE_DATE,// = SmartParametersV2016.defaultDates,
                    PAYMENT_TYPE = "",
                    FIRST_YEAR_DISCOUNT = "0",
                    LOYALTY_BONUS = "0",
                    DISCOUNT_CREDIT_DATE,// = SmartParametersV2016.defaultDates,
                    REWARDS = "n/a";
            short BILL_VAT_CODE = SmartParametersV2016.zeroRateVatCode,
                    RESOURCE_VAT_CODE = SmartParametersV2016.zeroRateVatCode;

            bool status = true;
            bool postcode_found = false;

            utilityviewmodel.BILL_PERIOD_START = SmartParametersV2016.defaultDates;
            utilityviewmodel.BILL_PERIOD_END = SmartParametersV2016.defaultDates;

            DateTime temp_date;// = SmartParametersV2016.defaultDate;
            // Pass 1 - look for the Big Three (or the Big Two in the case of those E.On Foreigners)
            //int line_count = 0;
            for (int line_count = Convert.ToInt32(sections[utilityviewmodel.currentIndex][4]); line_count <= Convert.ToInt32(sections[utilityviewmodel.currentIndex][5]); line_count++)
            {
                token = lines[line_count].Trim();

                if (SmartParseV2016.Token_Identify(utilityviewmodel, statement_date, true))
                {
                    utilityviewmodel.BILL_DATE = SmartParseV2016.Find_Bill_Date(utilityviewmodel, line_count, lines, SmartParametersV2016.defaultDates, token);
                    if (!SmartParseV2016.Generic_Parse_Datetime(utilityviewmodel.BILL_DATE, utilityviewmodel))
                    {
                        return false;
                    }
                    SmartParseV2016.Update_Bill(ourviewmodel, utilityviewmodel, "BILL_DATE", utilityviewmodel.BILL_DATE);
                    goto the_old_daysA0;
                }

                if (SmartParseV2016.Token_Identify(utilityviewmodel, your_account_number, true))
                {
                    if (string.IsNullOrEmpty(utilityviewmodel.ACCOUNT_NO))
                    {
                        string temp_account = "";

                        int sub_line_count = line_count;
                        while (sub_line_count < Convert.ToInt32(sections[utilityviewmodel.currentIndex][5]))
                        {
                            token = lines[sub_line_count].Replace(your_account_number, "").Trim();
                            token = token.Replace(SmartParametersV2016.space, "").Trim();
                            if (SmartParseV2016.Generic_Parse_Digits(token, utilityviewmodel))
                            {
                                temp_account = token;
                                break;
                            }
                            sub_line_count++;
                        }
                        line_count = sub_line_count;

                        if (!string.IsNullOrEmpty(temp_account))
                        {
                            utilityviewmodel.ACCOUNT_NO = temp_account;
                            if (string.IsNullOrEmpty(utilityviewmodel.account_no))
                            {
                                // Fix up scraped account number if it needs it
                                utilityviewmodel.account_no = utilityviewmodel.ACCOUNT_NO;
                            }
                        }
                    }
                    goto the_old_daysA0;
                }

                if (!postcode_found)
                {
                    string account_postcode = token;
                    if (SmartNibbyV2016.Derive_Postcode_New(account_postcode,
                                                    ourviewmodel.Blanche.workingPostcodesList,
                                                    utilityviewmodel))
                    {
                        postcode_found = true;
                    }
                    goto the_old_daysA0;
                }

            the_old_daysA0:
                continue;
            }

            // This is a special for E.On because the Foreigners don't have Bill periods
            // decribed at the top of their statements
            utilityviewmodel.BILL_PERIOD_START = utilityviewmodel.BILL_PERIOD_END = utilityviewmodel.BILL_DATE;
            DISCOUNT_CREDIT_DATE = utilityviewmodel.BILL_PERIOD_END;
            PAYMENT_DUE_DATE = utilityviewmodel.BILL_PERIOD_END;

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
            SmartUtilityV2022.Bills_Resource_Row_Update(utilityviewmodel.Hezbollah.bills_resource_row,
                                                        SmartParametersV2016.Utility,
                                                        utilityviewmodel.supplier_code,
                                                        utilityviewmodel.brand_code,
                                                        utilityviewmodel.ACCOUNT_NO,
                                                        utilityviewmodel.CREATED,
                                                        utilityviewmodel.STATEMENT_ID,
                                                        utilityviewmodel.BILL_DATE,
                                                        SmartUtilityV2022.Determine_MPAN_MPRN(utilityviewmodel),
                                                        RESOURCE_ACCOUNT_NO,
                                                        TARIFF_CODE,        // WE don't check for TARIFF_CODE = 0 here ... Well we fucking should
                                                        NEW_CHARGES,
                                                        RESOURCE_DISCOUNTS,
                                                        RESOURCE_VAT_AMOUNT,
                                                        RESOURCE_VAT_CODE);
            return status;
        }

        private static bool Parse_SectionB0(UtilityViewModel utilityviewmodel,
                                            string[][] sections,
                                            string section,
                                            string sub_section)
        {
            utilityviewmodel.currentIndex = 0;
            if (!SmartParseV2016.Decode_Section(section, sub_section, sections, utilityviewmodel.resource_code.ToString(), utilityviewmodel))
            {
                return true;
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

            // Now I have to listen to that dickhead fucking about with the fucking phone
            short adjustment_item = 0;
            string token;


            utilityviewmodel.PAYMENT_METHOD = "";
            utilityviewmodel.PAYMENT_AMOUNT = "";
            utilityviewmodel.PAYMENT_DATE = SmartParametersV2016.defaultDates;
            utilityviewmodel.PAYMENT_BALANCE = "";

            string balance_on_last_statement = "Balance on last statement",
                    credit_balance_from_your_last_statement = "Credit balance from your last statement",
                    your_payments = "Your payments",
                    space_dash_space = " - ";

            for (int line_count = Convert.ToInt32(sections[utilityviewmodel.currentIndex][4]); line_count <= Convert.ToInt32(sections[utilityviewmodel.currentIndex][5]); line_count++)
            {
                token = lines[line_count].Trim();

                if (SmartParseV2016.Token_Identify(utilityviewmodel, balance_on_last_statement, false) ||
                    SmartParseV2016.Token_Identify(utilityviewmodel, credit_balance_from_your_last_statement, false))
                {
                    // Fucking mixer on a-fucking gain...
                    int dash_index = token.IndexOf(space_dash_space);
                    if (dash_index >= 0)
                    {
                        token = token.Replace(space_dash_space, SmartParametersV2016.bar.ToString());
                        int currency_index = token.IndexOf(utilityviewmodel.bill_currency_symbol);
                        if (currency_index >= 0)
                        {
                            token = token.Replace(utilityviewmodel.bill_currency_symbol, SmartParametersV2016.bar.ToString() + utilityviewmodel.bill_currency_symbol);
                            if (!SectionB1_Check_Balance(ourviewmodel,
                                                            utilityviewmodel,
                                                            token))
                            {
                                return false;
                            }
                        }
                    }
                    goto the_old_daysB1;
                }

                if (SmartParseV2016.Token_Identify(utilityviewmodel, your_payments, true))
                {
                    string DEFAULT_PAYMENT_METHOD = "Payment received";
                    //token = "";
                    // Fucking mixer on a-fucking gain...
                    // is it on THIS line?
                    int sub_line_count = line_count;
                    while (sub_line_count < Convert.ToInt32(sections[utilityviewmodel.currentIndex][5]))
                    {
                        token = lines[sub_line_count].Trim();
                        int on_index = token.IndexOf(" on ");
                        if (on_index >= 0)
                        {
                            token = token.Replace(" on ", SmartParametersV2016.bar.ToString());

                            string[] components = token.Split(utilityviewmodel.bill_currency_symbol);
                            int component_count = 0;
                            while (component_count < components.Length)
                            {
                                token = components[component_count].Trim();
                                if (!string.IsNullOrEmpty(token))
                                {
                                    utilityviewmodel.PAYMENT_AMOUNT = "";
                                    utilityviewmodel.PAYMENT_DATE = "";
                                    utilityviewmodel.PAYMENT_METHOD = "";
                                    utilityviewmodel.PAYMENT_DATE = SmartParametersV2016.defaultDates;

                                    string[] sub_components = components[component_count].Split(SmartParametersV2016.bar);
                                    if (sub_components.Length > 0)
                                    {
                                        token = sub_components[0].Trim();
                                        token = utilityviewmodel.bill_currency_symbol + token;
                                        if (!SectionB1_Check_Bits_And_Pieces(utilityviewmodel,
                                                            token,
                                                            sub_components,
                                                            DEFAULT_PAYMENT_METHOD))
                                        {
                                            return false;
                                        }

                                        utilityviewmodel.PAYMENT_CODE = "";
                                        if (!string.IsNullOrEmpty(utilityviewmodel.PAYMENT_METHOD))
                                        {
                                            if (SmartSpikeUtilityV2017.Utility_Lookup_PaymentCode(ourviewmodel,
                                                                                            utilityviewmodel,
                                                                                            utilityviewmodel.PAYMENT_METHOD)) //rf PAYMENT_CODE
                                            {
                                                //utilityviewmodel.PAYMENT_METHOD = utilityviewmodel.PAYMENT_METHOD;
                                                if ((utilityviewmodel.PAYMENT_DATE != SmartParametersV2016.defaultDates) &&
                                                    !string.IsNullOrEmpty(utilityviewmodel.PAYMENT_AMOUNT))
                                                {
                                                    // Put it in Payments
                                                    utilityviewmodel.PAYMENT_BALANCE = utilityviewmodel.PAYMENTS_BALANCE.ToString();
                                                    if (!SmartParseV2016.Check_Payment_Is_There(ourviewmodel,
                                                                                                utilityviewmodel))      // Which IS the PAYMENT_DATE as a DateTime
                                                    {                                                           //utilityviewmodel.PAYMENTS_ITEM, //payment_item,
                                                        //SmartUtilityV2022.PaymentsList_Add(utilityviewmodel.Hezbollah.payments_changesList,
                                                        //                                   SmartParametersV2016.Utility,
                                                        //                                    utilityviewmodel.supplier_code,
                                                        //                                    utilityviewmodel.brand_code,
                                                        //                                    utilityviewmodel.ACCOUNT_NO,
                                                        //                                    utilityviewmodel.CREATED,
                                                        //                                    utilityviewmodel.STATEMENT_ID,
                                                        //                                    utilityviewmodel.BILL_DATE,
                                                        //                                    SmartTimeV2016.ConvertDateTime(PAYMENT_DATE, SmartParametersV2016.defaultCulture),
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
                                                }
                                            }
                                            else
                                            {
                                                if (!string.IsNullOrEmpty(utilityviewmodel.PAYMENT_AMOUNT))
                                                {
                                                    // It failed the PAYMENT_CODE lookup so assume its an Adjustment
                                                    utilityviewmodel.ACCOUNT_CHARGES_CREDITS_ITEM = (short)(utilityviewmodel.ACCOUNT_CHARGES_CREDITS_ITEM + 1);

                                                    DateTime ACCOUNT_DATE = SmartTimeV2016.ConvertDateTime(utilityviewmodel.PAYMENT_DATE);
                                                    string ACCOUNT_TYPE = utilityviewmodel.PAYMENT_METHOD.Length <= SmartParametersV2016.TYPESLENGTH ? utilityviewmodel.PAYMENT_METHOD : utilityviewmodel.PAYMENT_METHOD.Substring(0, SmartParametersV2016.TYPESLENGTH);
                                                    short ACCOUNT_VAT_CODE = SmartParametersV2016.zeroRateVatCode;
                                                    int ACCOUNT_AMOUNT = Convert.ToInt32(utilityviewmodel.PAYMENT_AMOUNT);
                                                    string INCLUDE_BILLS = SmartParametersV2016.yesFlag;

                                                    bool is_it_there = false;
                                                    {
                                                        if (string.IsNullOrEmpty(sections[utilityviewmodel.currentIndex][2]))
                                                        {
                                                            is_it_there = SmartParseV2016.Check_Adjustment_Is_There(ourviewmodel,
                                                                                                                    utilityviewmodel,
                                                                                                                    ACCOUNT_DATE,
                                                                                                                    adjustment_item,
                                                                                                                    ACCOUNT_TYPE,
                                                                                                                    ACCOUNT_VAT_CODE,
                                                                                                                    ACCOUNT_AMOUNT);
                                                        }
                                                    }
                                                    if (!is_it_there)
                                                    {
                                                        SmartUtilityV2022.Account_Charges_CreditsList_Add(utilityviewmodel,
                                                                                                          SmartParametersV2016.Utility,
                                                                                                          utilityviewmodel.supplier_code,
                                                                                                          utilityviewmodel.brand_code,
                                                                                                          utilityviewmodel.ACCOUNT_NO,
                                                                                                          utilityviewmodel.CREATED,
                                                                                                          utilityviewmodel.STATEMENT_ID,
                                                                                                          utilityviewmodel.BILL_DATE,
                                                                                                          ACCOUNT_DATE,
                                                                                                          adjustment_item,
                                                                                                          ACCOUNT_TYPE,
                                                                                                          ACCOUNT_VAT_CODE,
                                                                                                          ACCOUNT_AMOUNT,
                                                                                                          INCLUDE_BILLS);
                                                        SmartParseV2016.Update_Bill(ourviewmodel, utilityviewmodel, "ACCOUNT_CHARGES_CREDITS", ACCOUNT_AMOUNT.ToString());
                                                        // Note: ACCOUNT_AMOUNT has already been checked as an Integer}
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                                component_count++;
                            }
                        }
                        sub_line_count++;
                    }
                    line_count = sub_line_count;
                    goto the_old_daysB1;
                }
            the_old_daysB1:
                continue;
            }
            return true;
        }

        private static bool SectionB1_Check_Bits_And_Pieces(UtilityViewModel utilityviewmodel,
                                                            string token,
                                                            string[] sub_components,
                                                            string DEFAULT_PAYMENT_METHOD)
        {
            int cr_index = token.IndexOf("CR");
            if (cr_index >= 0)
            {
                if (token.Length <= cr_index + 2)
                {
                    utilityviewmodel.PAYMENT_METHOD = DEFAULT_PAYMENT_METHOD;
                }
                else
                {
                    utilityviewmodel.PAYMENT_METHOD = token.Substring(cr_index + 2).Trim();
                    token = token.Replace(utilityviewmodel.PAYMENT_METHOD, "").Trim();
                }
            }

            if (!SmartParseV2016.Currency_Strip(utilityviewmodel.bill_currency_symbol, utilityviewmodel.bill_currency_separator, utilityviewmodel.bill_thousands_separator, token, utilityviewmodel))
            {
                return false;
            }
            utilityviewmodel.PAYMENT_AMOUNT = utilityviewmodel.value;
            if (!SmartParseV2016.Generic_Parse_Integer(utilityviewmodel.PAYMENT_AMOUNT, utilityviewmodel))
            {
                return false;
            }
            utilityviewmodel.PAYMENT_DATE = sub_components[1].Trim();
            if (!SmartParseV2016.Generic_Parse_Datetime(utilityviewmodel.PAYMENT_DATE, utilityviewmodel))
            {
                return false;
            }
            return true;
        }

        private static bool SectionB1_Check_Balance(MainViewModel ourviewmodel,
                                                    UtilityViewModel utilityviewmodel,
                                                    string token)
        {
            string[] components = token.Split(SmartParametersV2016.bar);
            if (components.Length == 3)
            {
                utilityviewmodel.BILL_PERIOD_START = components[1];
                //DateTime temp_date = SmartParametersV2016.defaultDate;
                if (!SmartParseV2016.Generic_Parse_Datetime(utilityviewmodel.BILL_PERIOD_START, utilityviewmodel))
                {
                    return false;
                }
                SmartParseV2016.Update_Bill(ourviewmodel, utilityviewmodel, "BILL_PERIOD_START", utilityviewmodel.BILL_PERIOD_START);

                token = components[2].Trim();
                string PREVIOUS_BALANCE;// = "";
                if (!SmartParseV2016.Currency_Strip(utilityviewmodel.bill_currency_symbol, utilityviewmodel.bill_currency_separator, utilityviewmodel.bill_thousands_separator, token, utilityviewmodel))
                {
                    return false;
                }
                else
                {
                    PREVIOUS_BALANCE = utilityviewmodel.value;
                }
                //int temp = 0;
                if (!SmartParseV2016.Generic_Parse_Integer(PREVIOUS_BALANCE, utilityviewmodel))
                {
                    return false;
                }
                //else
                //{
                //    temp = utilityviewmodel.genericTransactionValue;
                //}
                SmartParseV2016.Update_Bill(ourviewmodel, utilityviewmodel, "PREVIOUS_BALANCE", PREVIOUS_BALANCE);
            }
            return true;
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


            short discount_item = 0;
            string token,
                    ELECTRICITY_CHARGES,// = "0",
                    DISCOUNT_TYPE,
                    DISCOUNT_AMOUNT,
                    electricity_charges = "Electricity charges",
                    discounts = "Discounts",
                    VAT_at = "VAT at",
                    Vat_at = "Vat at";
            short DISCOUNT_VAT_CODE = SmartParametersV2016.zeroRateVatCode;
            DateTime DISCOUNT_DATE;// = SmartParametersV2016.defaultDate;

            for (int line_count = Convert.ToInt32(sections[utilityviewmodel.currentIndex][4]); line_count <= Convert.ToInt32(sections[utilityviewmodel.currentIndex][5]); line_count++)
            {
                token = lines[line_count].Trim();
                if (SmartParseV2016.Token_Identify(utilityviewmodel, electricity_charges, false))
                {
                    int currency_index = token.IndexOf(utilityviewmodel.bill_currency_symbol);
                    if (currency_index >= 0)
                    {
                        token = token.Substring(currency_index);
                        if (!SmartParseV2016.Currency_Strip(utilityviewmodel.bill_currency_symbol, utilityviewmodel.bill_currency_separator, utilityviewmodel.bill_thousands_separator, token, utilityviewmodel))
                        {
                            return false;
                        }
                        else
                        {
                            ELECTRICITY_CHARGES = utilityviewmodel.value;
                        }
                        //int temp;
                        if (!SmartParseV2016.Generic_Parse_Integer(ELECTRICITY_CHARGES, utilityviewmodel))
                        {
                            return false;
                        }
                        //else
                        //{
                        //    temp = utilityviewmodel.genericTransactionValue;
                        //}
                        SmartParseV2016.Update_Bill(ourviewmodel, utilityviewmodel, "RESOURCE_NEW_CHARGES", ELECTRICITY_CHARGES);
                    }
                    goto the_old_daysB2;
                }
                // 29th March 2016.  Last night ignorant charmless brutish bitch
                // slammed her way out of the bedroom with her usual cursing and
                // slutspeak.  She is SUCH a moron.  Stupid.  Ignorant.  Rude.
                if (SmartParseV2016.Token_Identify(utilityviewmodel, discounts, false))
                {
                    // Get some kind of discount date
                    DISCOUNT_DATE = SmartNibbyV2016.ConvertDate(utilityviewmodel.BILL_PERIOD_END,
                        SmartParametersV2016.defaultDate, ourviewmodel);
                    if (ourviewmodel.errorMessage != "")
                    {
                        return false;
                    }

                    // Fucking mixer on a-fucking gain...
                    // is it on THIS next line?
                    int sub_line_count = line_count + 1;
                    while (sub_line_count <= Convert.ToInt32(sections[utilityviewmodel.currentIndex][5]))
                    {
                        token = lines[sub_line_count].Trim();
                        if (token.IndexOf(VAT_at) >= 0)
                        {
                            break;
                        }

                        string[] components = new string[1];
                        if (token.IndexOf("and") >= 0)
                        {
                            token = token.Replace("and", SmartParametersV2016.bar.ToString());
                            components = token.Split(SmartParametersV2016.bar);
                        }
                        else
                        {
                            components[0] = token;
                        }
                        foreach (string component in components)
                        {
                            token = component.Trim();
                            int discount_index = token.IndexOf(utilityviewmodel.bill_currency_symbol);
                            if (discount_index >= 0)
                            {
                                DISCOUNT_TYPE = DISCOUNT_AMOUNT = "";

                                DISCOUNT_TYPE = component.Substring(0, discount_index).Trim();
                                token = token.Substring(discount_index);
                                if (!SmartParseV2016.Currency_Strip(utilityviewmodel.bill_currency_symbol, utilityviewmodel.bill_currency_separator, utilityviewmodel.bill_thousands_separator, token, utilityviewmodel))
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
                                //    temp = utilityviewmodel.genericTransactionValue;
                                //}
                                discount_item = (short)(discount_item + 1);     // <= Can put this in MES if needs be to carry ot through
                                if (!SmartParseV2016.Check_Discount_Is_There(utilityviewmodel,
                                                                            DISCOUNT_DATE,
                                                                            discount_item,
                                                                            DISCOUNT_TYPE,
                                                                            DISCOUNT_VAT_CODE,
                                                                            DISCOUNT_AMOUNT))
                                {
                                    SmartParseV2016.No_Wonder_I_Cant_Fucking_Work(utilityviewmodel,
                                                                                    DISCOUNT_DATE,
                                                                                    discount_item,
                                                                                    DISCOUNT_TYPE,
                                                                                    DISCOUNT_VAT_CODE,
                                                                                    DISCOUNT_AMOUNT);
                                }
                            }
                        }
                        sub_line_count++;
                    }
                    line_count = sub_line_count;
                }

                // So this dumb fucking numbskull I live with cannot figure out that if the
                // police can't travel from Retrovik (!!!) to the place where the crime is being committed
                // .. then that place is *NOT* Retrovik!!!  How THICK can you get???!!!!!!!!????!!!  
                // Why don't the police just put their coffee cups down and walk around the corner?????
                // HA! HA !!!  She is ***SO*** Stupid!!
                if (SmartParseV2016.Token_Identify(utilityviewmodel, VAT_at +
                                                                SmartParametersV2016.bar +
                                                                Vat_at, true))
                {
                    short VAT_CODE = SmartParametersV2016.zeroRateVatCode;
                    if (!SmartParseV2016.Determine_Vat_Code(ourviewmodel,
                                                            utilityviewmodel,
                                                            token,
                                                            VAT_at))
                    {
                        return false;
                    }
                    SmartParseV2016.Update_Bill(ourviewmodel, utilityviewmodel, "BILL_VAT_CODE", VAT_CODE.ToString());
                    SmartParseV2016.Update_Bill(ourviewmodel, utilityviewmodel, "RESOURCE_VAT_CODE", VAT_CODE.ToString());

                    token = token.Replace(VAT_at, "").Trim();
                    token = token.Replace(Vat_at, "").Trim();
                    if (!SectionB2_Check_VAT(ourviewmodel,
                                                utilityviewmodel,
                                                token))
                    {
                        return false;
                    }
                    goto the_old_daysB2;
                }

            //if (SmartParseV2016.Token_Identify(utilityviewmodel, "S", true, true))
            //{
            //    if (SmartParseV2016.fix_supply_number(line_count,
            //                                lines,
            //                                sections[section_index][5],
            //                                sections[section_index][4],
            //                                utilityviewmodel,
            //                                false))
            //    {
            //        if (utilityviewmodel.mpan.Length >= 22)
            //        {
            //            SmartUtilityV2022.Amelia_Update_Mpan(utilityviewmodel, utilityviewmodel.mpan);
            //        }
            //    }
            //    goto the_old_daysB2;
            //}
            the_old_daysB2:
                continue;
            }
            return true;
        }

        private static bool SectionB2_Check_VAT(MainViewModel ourviewmodel,
                                                UtilityViewModel utilityviewmodel,
                                                string token)
        {
            //int temp = 0;
            string[] components = token.Split(SmartParametersV2016.spaceSplit);
            int components_count = 0;
            while (components_count < components.Length)
            {
                switch (components_count)
                {
                    case 3:
                        string VAT_AMOUNT;
                        if (!SmartParseV2016.Currency_Strip(utilityviewmodel.bill_currency_symbol, utilityviewmodel.bill_currency_separator, utilityviewmodel.bill_thousands_separator, components[components_count], utilityviewmodel))
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
                        //    temp = utilityviewmodel.genericTransactionValue;
                        //}
                        SmartParseV2016.Update_Bill(ourviewmodel, utilityviewmodel, "BILL_VAT_AMOUNT", VAT_AMOUNT);
                        SmartParseV2016.Update_Bill(ourviewmodel, utilityviewmodel, "RESOURCE_VAT_AMOUNT", VAT_AMOUNT);
                        break;
                    default:
                        break;
                }
                components_count++;
            }
            return true;
        }

        private static bool Parse_SectionB3(MainViewModel ourviewmodel,
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

            string OUTSTANDING_BALANCE,// = "0",
                    your_new_balance_is = "Your new balance is",
                    your_credit_balance_is = "Your credit balance is";
            //int TEMP = 0;

            for (int line_count = Convert.ToInt32(sections[utilityviewmodel.currentIndex][4]); line_count <= Convert.ToInt32(sections[utilityviewmodel.currentIndex][5]); line_count++)
            {
                utilityviewmodel.token = lines[line_count].Trim();
                if (SmartParseV2016.Token_Identify(utilityviewmodel, your_new_balance_is, false) ||
                    SmartParseV2016.Token_Identify(utilityviewmodel, your_credit_balance_is, false))
                {
                    // Fucking mixer on a-fucking gain...
                    // is it on THIS line?
                    int sub_line_count = line_count + 1;
                    while (sub_line_count < Convert.ToInt32(sections[utilityviewmodel.currentIndex][5]))
                    {
                        utilityviewmodel.token = lines[sub_line_count].Trim();
                        if (utilityviewmodel.token.IndexOf(utilityviewmodel.bill_currency_symbol) >= 0)
                        {
                            string[] components = utilityviewmodel.token.Split(SmartParametersV2016.spaceSplit);
                            utilityviewmodel.token = components[0];
                            if (!string.IsNullOrEmpty(utilityviewmodel.token))
                            {
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
                                break;
                            }
                        }
                        sub_line_count++;
                    }
                    line_count = sub_line_count;
                    goto the_old_daysB3;
                }

            //if (SmartParseV2016.Token_Identify(utilityviewmodel, "S", true, true))
            //{
            //    if (SmartParseV2016.fix_supply_number(line_count,
            //                                lines,
            //                                sections[section_index][5],
            //                                sections[section_index][4],
            //                                utilityviewmodel,
            //                                false))
            //    {
            //        if (utilityviewmodel.mpan.Length >= 22)
            //        {
            //            SmartUtilityV2022.Amelia_Update_Mpan(utilityviewmodel, utilityviewmodel.mpan);
            //        }
            //    }
            //    goto the_old_daysB3;
            //}

            the_old_daysB3:
                continue;
            }
            return true;
        }

        private static async Task<bool> Parse_SectionC0(MainViewModel ourviewmodel,
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

            string token,
                    about_your_tariff = "About your tariff",
                    paying_by = "Paying by";

            for (int line_count = Convert.ToInt32(sections[utilityviewmodel.currentIndex][4]); line_count <= Convert.ToInt32(sections[utilityviewmodel.currentIndex][5]); line_count++)
            {
                token = lines[line_count].Trim();
                if (SmartParseV2016.Token_Identify(utilityviewmodel, about_your_tariff, false))
                {
                    int sub_line_count = line_count; // + 1;
                    while (sub_line_count < Convert.ToInt32(sections[utilityviewmodel.currentIndex][5]))
                    {
                        token = lines[sub_line_count].Trim();
                        if (token.IndexOf("Name") >= 0)
                        {
                            token = token.Replace("Name", "");
                            utilityviewmodel.TARIFF_NAME = token.Trim();
                            if (!await SmartUtilityV2022.Lookup_Remote_Tariff_Code(ourviewmodel, utilityviewmodel))
                            {
                                // ourviewmodel.errorMessage should be set here
                                return false;
                            }
                            SmartParseV2016.Update_Bill(ourviewmodel, utilityviewmodel, "TARIFF_CODE", utilityviewmodel.TARIFF_CODE.ToString());
                            break;
                        }
                        sub_line_count++;
                    }
                    line_count = sub_line_count;
                    goto the_old_daysC0;
                }

                if (SmartParseV2016.Token_Identify(utilityviewmodel, paying_by, true))
                {
                    string PAYMENT_TYPE = token.Trim();
                    if (SmartSpikeUtilityV2017.Utility_Lookup_PaymentPlan(ourviewmodel,
                                                                utilityviewmodel,
                                                                PAYMENT_TYPE))
                    {
                        SmartParseV2016.Update_Bill(ourviewmodel, utilityviewmodel, "PAYMENT_PLAN", utilityviewmodel.PAYMENT_PLAN.ToString());
                        SmartParseV2016.Update_Bill(ourviewmodel, utilityviewmodel, "PAYMENT_TYPE", PAYMENT_TYPE);
                    }
                    else
                    {
                        // utilityviewmodel.urgent message should be set here
                        return false;
                    }
                    goto the_old_daysC0;
                }
            the_old_daysC0:
                continue;
            }
            return true;
        }

        private static bool Parse_SectionH0(UtilityViewModel utilityviewmodel,
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

            string meter_readings = "Meter readings";

            for (int line_count = Convert.ToInt32(sections[utilityviewmodel.currentIndex][4]); line_count <= Convert.ToInt32(sections[utilityviewmodel.currentIndex][5]); line_count++)
            {
                utilityviewmodel.token = lines[line_count].Trim();
                if (SmartParseV2016.Token_Identify(utilityviewmodel, meter_readings, true))
                {
                    goto the_old_daysH0;
                }
            the_old_daysH0:
                continue;
            }
            return true;
        }

        private static string Fix_Read_Type(string read_type)
        {
            if (!string.IsNullOrEmpty(read_type))
            {
                string actual = "Actual",
                        estimated = "Estimated";
                switch (read_type.Substring(0, 1))
                {
                    case "A":
                        return actual;
                    case "E":
                        return estimated;
                    default:
                        break;
                }
            }
            return "";
        }
        private static bool Parse_SectionH1(UtilityViewModel utilityviewmodel,
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

            utilityviewmodel.METER_SERIAL_NO = "";
            DateTime READINGS_PERIOD_START = SmartParametersV2016.defaultDate,
                     READINGS_PERIOD_END = SmartParametersV2016.defaultDate;
            string electricity_readings = "Electricity readings";

            //int readings_count = 0;
            utilityviewmodel.D_THIS_READ = "";
            utilityviewmodel.D_LAST_READ = "";
            utilityviewmodel.D_UNITS_USED = "";
            utilityviewmodel.N_THIS_READ = "";
            utilityviewmodel.N_LAST_READ = "";
            utilityviewmodel.N_UNITS_USED = "";
            utilityviewmodel.UNIT_OF_MEASURE = "";
            string READ_TYPE,// = "",
                    UOM = "";
            string space_to_space = " to ",
                    kilowatt_hours = "kilowatt hours",
                   kilowatt_dash_hours = "kilowatt-hours";
            utilityviewmodel.day_rate = false;

            for (int line_count = Convert.ToInt32(sections[utilityviewmodel.currentIndex][4]); line_count <= Convert.ToInt32(sections[utilityviewmodel.currentIndex][5]); line_count++)
            {
                utilityviewmodel.token = lines[line_count].Trim();
                if (SmartParseV2016.Token_Identify(utilityviewmodel, electricity_readings, false))
                {
                    goto the_good_old_daysH1;
                }

                if (SmartParseV2016.Token_Identify(utilityviewmodel, "The details", false))
                {
                    break;
                }

                utilityviewmodel.token = lines[line_count].Replace(electricity_readings, "").Trim();

                if ((utilityviewmodel.token.IndexOf(kilowatt_hours) >= 0) ||
                    (utilityviewmodel.token.IndexOf(kilowatt_dash_hours) >= 0))
                {
                    UOM = SmartParametersV2016.defaultUoM;
                    utilityviewmodel.day_rate = false;
                    goto the_good_old_daysH1;
                }

                int to_index = utilityviewmodel.token.IndexOf(space_to_space);

                if (to_index >= 0)
                {
                    int sub_line_count = line_count;
                    while (sub_line_count <= Convert.ToInt32(sections[utilityviewmodel.currentIndex][5]))
                    {
                        utilityviewmodel.token = lines[sub_line_count];

                        if (SmartParseV2016.Token_Identify(utilityviewmodel, "The details", false))
                        {
                            // Re-do this line
                            sub_line_count--;
                            break;
                        }

                        to_index = utilityviewmodel.token.IndexOf(space_to_space);
                        if (to_index >= 0)
                        {
                            utilityviewmodel.day_rate = false;
                            utilityviewmodel.D_THIS_READ = "";
                            utilityviewmodel.D_LAST_READ = "";
                            utilityviewmodel.D_UNITS_USED = "";
                            utilityviewmodel.N_THIS_READ = "";
                            utilityviewmodel.N_LAST_READ = "";
                            utilityviewmodel.N_UNITS_USED = "";
                            READ_TYPE = "";
                            utilityviewmodel.READINGS_PERIOD_START = SmartParametersV2016.defaultDates;
                            utilityviewmodel.READINGS_PERIOD_END = SmartParametersV2016.defaultDates;
                            utilityviewmodel.UNIT_OF_MEASURE = "";

                            if (!Fuck_About(utilityviewmodel,
                                                space_to_space))
                            {
                                return false;
                            }
                            else
                            {
                                lines[sub_line_count] = utilityviewmodel.token;
                            }

                            int sub_sub_line_count = sub_line_count;

                            utilityviewmodel.token = lines[sub_sub_line_count];

                            // We are pretty sure we have a 'Day' or a 'Night' line
                            if (!SectionH1_reduced_reading(utilityviewmodel,
                                                    utilityviewmodel.token,
                                                    UOM))
                            {
                                return false;
                            }

                            if (!utilityviewmodel.day_rate)
                            {
                                int e_readings_last = utilityviewmodel.Hezbollah.e_readings_changesList.Count - 1;
                                // We don't do the TYPE because its INCONCEIVABLE that they would
                                // do an 'Actual' for Day units and 'Estimated' for Night units!!
                                // Bet the fuckers DO!!
                                // Same assumption for UOM
                                //
                                // Minor sanity check ...
                                if (e_readings_last >= 0)
                                {
                                    if (utilityviewmodel.Hezbollah.e_readings_changesList[e_readings_last].READINGS_PERIOD_START == READINGS_PERIOD_START &&
                                    utilityviewmodel.Hezbollah.e_readings_changesList[e_readings_last].READINGS_PERIOD_END == READINGS_PERIOD_END)
                                    {
                                        utilityviewmodel.Hezbollah.e_readings_changesList[e_readings_last].N_LAST_READ = SmartRoutinesV2018.ConvertDecimal(utilityviewmodel.D_LAST_READ);
                                        utilityviewmodel.Hezbollah.e_readings_changesList[e_readings_last].N_THIS_READ = SmartRoutinesV2018.ConvertDecimal(utilityviewmodel.D_THIS_READ);
                                        utilityviewmodel.Hezbollah.e_readings_changesList[e_readings_last].N_UNITS_USED = SmartRoutinesV2018.ConvertDecimal(utilityviewmodel.D_UNITS_USED);
                                    }
                                }
                            }
                            else
                            {
                                if (SmartParseV2016.Add_Readings_Test(SmartParametersV2016.defaultDate, READINGS_PERIOD_START, READINGS_PERIOD_END, utilityviewmodel.D_LAST_READ, utilityviewmodel.D_THIS_READ, utilityviewmodel.UNIT_OF_MEASURE))
                                {
                                    switch (utilityviewmodel.resource_code)
                                    {
                                        case SmartParametersV2016.Electricity:
                                            SmartUtilityV2022.E_ReadingsList_Add(utilityviewmodel,
                                                                                SmartParametersV2016.Utility,
                                                                                utilityviewmodel.supplier_code,
                                                                                utilityviewmodel.brand_code,
                                                                                utilityviewmodel.ACCOUNT_NO,
                                                                                utilityviewmodel.CREATED,
                                                                                utilityviewmodel.STATEMENT_ID,
                                                                                utilityviewmodel.BILL_DATE,
                                                                                utilityviewmodel.sparks.MPAN,
                                                                                READINGS_PERIOD_START.ToString(),
                                                                                READINGS_PERIOD_END.ToString(),
                                                                                utilityviewmodel.METER_SERIAL_NO,
                                                                                READ_TYPE,
                                                                                utilityviewmodel.D_LAST_READ,
                                                                                utilityviewmodel.D_THIS_READ,
                                                                                utilityviewmodel.D_UNITS_USED,
                                                                                utilityviewmodel.N_LAST_READ,
                                                                                utilityviewmodel.N_THIS_READ,
                                                                                utilityviewmodel.N_UNITS_USED,
                                                                                utilityviewmodel.UNIT_OF_MEASURE);
                                            break;
                                        default:
                                            break;
                                    }
                                }
                            }
                        }
                        else
                        {
                            // Old Style bills with no ' to ' on the date line
                            if (utilityviewmodel.day_rate &&
                                (utilityviewmodel.token.IndexOf(utilityviewmodel.METER_SERIAL_NO) >= 0) &&
                                (utilityviewmodel.token.IndexOf("Night") >= 0))

                            {
                                // We are pretty sure we have a 'Night' line
                                if (!SectionH1_reduced_reading(utilityviewmodel,
                                                        utilityviewmodel.token,
                                                        UOM))
                                {
                                    return false;
                                }
                                int e_readings_last = utilityviewmodel.Hezbollah.e_readings_changesList.Count - 1;
                                // We don't do the TYPE because its INCONCEIVABLE that they would
                                // do an 'Actual' for Day units and 'Estimated' for Night units!!
                                // Bet the fuckers DO!!
                                // Same assumption for UOM
                                //
                                // Minor sanity check ...
                                if (e_readings_last >= 0)
                                {
                                    if (utilityviewmodel.Hezbollah.e_readings_changesList[e_readings_last].READINGS_PERIOD_START == READINGS_PERIOD_START &&
                                    utilityviewmodel.Hezbollah.e_readings_changesList[e_readings_last].READINGS_PERIOD_END == READINGS_PERIOD_END)
                                    {
                                        utilityviewmodel.Hezbollah.e_readings_changesList[e_readings_last].N_LAST_READ = SmartRoutinesV2018.ConvertDecimal(utilityviewmodel.D_LAST_READ);
                                        utilityviewmodel.Hezbollah.e_readings_changesList[e_readings_last].N_THIS_READ = SmartRoutinesV2018.ConvertDecimal(utilityviewmodel.D_THIS_READ);
                                        utilityviewmodel.Hezbollah.e_readings_changesList[e_readings_last].N_UNITS_USED = SmartRoutinesV2018.ConvertDecimal(utilityviewmodel.D_UNITS_USED);
                                    }
                                }
                            }
                        }
                        sub_line_count++;
                    }
                    line_count = sub_line_count;
                    goto the_good_old_daysH1;
                }
            the_good_old_daysH1:
                continue;
            }
            return true;
        }

        private static bool SectionH1_reduced_reading(UtilityViewModel utilityviewmodel,
                                                        string token,
                                                        string UOM)
        {
            string[] components = token.Split(SmartParametersV2016.spaceSplit);
            int component_count = 0;
            while (component_count < components.Length)
            {
                switch (component_count)
                {
                    case 0:         // Meter Number
                        utilityviewmodel.METER_SERIAL_NO = components[component_count].Trim();
                        utilityviewmodel.METER_SERIAL_NO = utilityviewmodel.METER_SERIAL_NO.Substring(0, utilityviewmodel.METER_SERIAL_NO.Length <= SmartParametersV2016.METERSERIALLENGTH ? utilityviewmodel.METER_SERIAL_NO.Length : SmartParametersV2016.METERSERIALLENGTH); //Enforce 14 char max
                        break;
                    case 1:         // Previous Read
                        utilityviewmodel.D_LAST_READ = components[component_count].Trim();
                        if (!SmartParseV2016.Generic_Parse_Integer(utilityviewmodel.D_LAST_READ, utilityviewmodel))
                        {
                            return false;
                        }
                        break;
                    case 2:         // Actual or Estimate
                        components[component_count] = components[component_count].Trim();
                        utilityviewmodel.READ_TYPE = Fix_Read_Type(components[component_count]);
                        break;
                    case 3:         // Present Read
                        utilityviewmodel.D_THIS_READ = components[component_count].Trim();
                        if (!SmartParseV2016.Generic_Parse_Integer(utilityviewmodel.D_THIS_READ, utilityviewmodel))
                        {
                            return false;
                        }
                        break;
                    case 4:         // Actual or Estimate (overrides previous)
                        components[component_count] = components[component_count].Trim();
                        utilityviewmodel.READ_TYPE = Fix_Read_Type(components[component_count]);
                        break;
                    case 5:        // Rate - Day or Night
                        if (components[component_count].Trim() == "Day")
                        {
                            utilityviewmodel.day_rate = true;
                            utilityviewmodel.N_LAST_READ = "";
                            utilityviewmodel.N_THIS_READ = "";
                            utilityviewmodel.N_UNITS_USED = "";
                        }
                        break;
                    case 6:        // kWh
                        utilityviewmodel.D_UNITS_USED = components[component_count].Trim();
                        if (!SmartParseV2016.Generic_Parse_Integer(utilityviewmodel.D_UNITS_USED, utilityviewmodel))
                        {
                            return false;
                        }
                        // Mock up the Unit of Measure
                        utilityviewmodel.UNIT_OF_MEASURE = UOM;
                        break;
                    default:
                        break;
                }
                component_count++;
            }
            return true;
        }

        //private static bool check_sectionH1_further_readings(string token,
        //                                                UtilityViewModel utilityviewmodel,
        //                                                rf string METER_SERIAL_NO,
        //                                                rf string READ_TYPE,
        //                                                rf bool day_rate,
        //                                                rf string N_LAST_READ,
        //                                                rf string N_THIS_READ,
        //                                                rf string N_UNITS_USED)
        //{
        //    int temp = 0;

        //    string[] components = token.Split(SmartParametersV2016.spaceSplit);
        //    int component_count = 0;
        //    while (component_count < components.Length)
        //    {
        //        switch (component_count)
        //        {
        //            case 0:         // Meter Number
        //                METER_SERIAL_NO = components[component_count].Trim();
        //                METER_SERIAL_NO = METER_SERIAL_NO.Substring(0, METER_SERIAL_NO.Length <= SmartParametersV2016.METERSERIALLENGTH ? METER_SERIAL_NO.Length : SmartParametersV2016.METERSERIALLENGTH); //Enforce 14 char max
        //                break;
        //            case 1:         // Previous Read
        //                N_LAST_READ = components[component_count].Trim();
        //                if (!SmartParseV2016.Generic_Parse_Integer(N_LAST_READ, rf temp, rf ourviewmodel.errorMessage))
        //                {
        //                    return false;
        //                }
        //                break;
        //            case 2:         // Actual or Estimate
        //                components[component_count] = components[component_count].Trim();
        //                READ_TYPE = fix_read_type(components[component_count]);
        //                break;
        //            case 3:         // Present Read
        //                N_THIS_READ = components[component_count].Trim();
        //                if (!SmartParseV2016.Generic_Parse_Integer(N_THIS_READ, rf temp, rf ourviewmodel.errorMessage))
        //                {
        //                    return false;
        //                }
        //                break;
        //            case 4:         // Actual or Estimate (overrides previous)
        //                components[component_count] = components[component_count].Trim();
        //                READ_TYPE = fix_read_type(components[component_count]);
        //                break;
        //            case 5:        // Rate - Day or Night
        //                if (components[component_count].Trim() == "Night")
        //                {
        //                    day_rate = false;
        //                }
        //                break;
        //            case 6:        // kWh
        //                N_UNITS_USED = components[component_count].Trim();
        //                if (!SmartParseV2016.Generic_Parse_Integer(N_UNITS_USED, rf temp, rf ourviewmodel.errorMessage))
        //                {
        //                    return false;
        //                }
        //                break;
        //            default:
        //                break;
        //        }
        //        component_count = component_count + 1;
        //    }
        //    return true;
        //}
        //private static bool check_sectionH1_readings(string token,
        //                                                UtilityViewModel utilityviewmodel,
        //                                                string UOM,
        //                                                rf DateTime READINGS_PERIOD_START,
        //                                                rf DateTime READINGS_PERIOD_END,
        //                                                rf string METER_SERIAL_NO,
        //                                                rf string READ_TYPE,
        //                                                rf string D_LAST_READ,
        //                                                rf string D_THIS_READ,
        //                                                rf string D_UNITS_USED,
        //                                                rf bool day_rate,
        //                                                rf string N_LAST_READ,
        //                                                rf string N_THIS_READ,
        //                                                rf string N_UNITS_USED,
        //                                                rf string UNIT_OF_MEASURE)
        //{
        //    int temp = 0;
        //    string TEMP_DATE = "";

        //    string[] components = token.Split(SmartParametersV2016.spaceSplit);
        //    int component_count = 0;
        //    while (component_count < components.Length)
        //    {
        //        switch (component_count)
        //        {
        //            case 0:         // READINGS_PERIOD_START day
        //                TEMP_DATE = components[component_count].Trim();
        //                break;
        //            case 1:         // READINGS_PERIOD_START month
        //                TEMP_DATE = TEMP_DATE + SmartParametersV2016.space + components[component_count].Trim();
        //                break;
        //            case 2:         // READINGS_PERIOD_START year
        //                TEMP_DATE = TEMP_DATE + SmartParametersV2016.space + components[component_count].Trim();
        //                if (!SmartParseV2016.Generic_Parse_Datetime(TEMP_DATE, rf READINGS_PERIOD_START, rf ourviewmodel.errorMessage))
        //                {
        //                    return false;
        //                }
        //                if (utilityviewmodel.BILL_PERIOD_START == utilityviewmodel.BILL_DATE)
        //                {
        //                    SmartParseV2016.Update_Bill("BILL_PERIOD_START", utilityviewmodel, TEMP_DATE);
        //                    utilityviewmodel.BILL_PERIOD_START = TEMP_DATE;
        //                }
        //                break;
        //            case 3:         // to
        //                if (components[component_count].Trim() != "to")
        //                {
        //                    return false;
        //                }
        //                break;
        //            case 4:         // TO_DATE day
        //                TEMP_DATE = components[component_count].Trim();
        //                break;
        //            case 5:         // TO_DATE month
        //                TEMP_DATE = TEMP_DATE + SmartParametersV2016.space + components[component_count].Trim();
        //                break;
        //            case 6:         // TO_DATE year
        //                TEMP_DATE = TEMP_DATE + SmartParametersV2016.space + components[component_count].Trim();
        //                if (!SmartParseV2016.Generic_Parse_Datetime(TEMP_DATE, rf READINGS_PERIOD_END, rf ourviewmodel.errorMessage))
        //                {
        //                    return false;
        //                }
        //                SmartParseV2016.Update_Bill("BILL_PERIOD_END", utilityviewmodel, TEMP_DATE);
        //                break;
        //            case 7:         // Meter Number
        //                METER_SERIAL_NO = components[component_count].Trim();
        //                METER_SERIAL_NO = METER_SERIAL_NO.Substring(0, METER_SERIAL_NO.Length <= SmartParametersV2016.METERSERIALLENGTH ? METER_SERIAL_NO.Length : SmartParametersV2016.METERSERIALLENGTH); //Enforce 14 char max
        //                break;
        //            case 8:         // Previous Read
        //                D_LAST_READ = components[component_count].Trim();
        //                if (!SmartParseV2016.Generic_Parse_Integer(D_LAST_READ, rf temp, rf ourviewmodel.errorMessage))
        //                {
        //                    return false;
        //                }
        //                break;
        //            case 9:         // Actual or Estimate
        //                components[component_count] = components[component_count].Trim();
        //                READ_TYPE = fix_read_type(components[component_count]);
        //                break;
        //            case 10:         // Present Read
        //                D_THIS_READ = components[component_count].Trim();
        //                if (!SmartParseV2016.Generic_Parse_Integer(D_THIS_READ, rf temp, rf ourviewmodel.errorMessage))
        //                {
        //                    return false;
        //                }
        //                break;
        //            case 11:         // Actual or Estimate (overrides previous)
        //                components[component_count] = components[component_count].Trim();
        //                READ_TYPE = fix_read_type(components[component_count]);
        //                break;
        //            case 12:        // Rate - Day or Night
        //                if (components[component_count].Trim() == "Day")
        //                {
        //                    day_rate = true;
        //                    N_LAST_READ = N_THIS_READ = N_UNITS_USED = "0";
        //                }
        //                break;
        //            case 13:        // kWh
        //                D_UNITS_USED = components[component_count].Trim();
        //                if (!SmartParseV2016.Generic_Parse_Integer(D_UNITS_USED, rf temp, rf ourviewmodel.errorMessage))
        //                {
        //                    return false;
        //                }
        //                // Mock up the Unit of Measure
        //                UNIT_OF_MEASURE = UOM;
        //                break;
        //            default:
        //                break;
        //        }
        //        component_count = component_count + 1;
        //    }
        //    return true;
        //}
        private static bool Parse_SectionH2(MainViewModel ourviewmodel,
                                            UtilityViewModel utilityviewmodel,
                                            string[][] sections, // Gas
                                            string section,
                                            string sub_section,
                                            string[] lines)
        {
            utilityviewmodel.currentIndex = 0;
            if (!SmartParseV2016.Decode_Section(section, sub_section, sections, utilityviewmodel.resource_code.ToString(), utilityviewmodel))
            {
                return true;
            }

            utilityviewmodel.METER_SERIAL_NO = "";
            utilityviewmodel.READINGS_PERIOD_START = SmartParametersV2016.defaultDates;
            utilityviewmodel.READINGS_PERIOD_END = SmartParametersV2016.defaultDates;
            string gas_readings = "Gas readings";
            int readings_count = 0;
            utilityviewmodel.D_THIS_READ = "";
            utilityviewmodel.D_LAST_READ = "";
            utilityviewmodel.D_UNITS_USED_M3 = "";
            utilityviewmodel.D_UNITS_USED_KWH = "";
            utilityviewmodel.UNIT_OF_MEASURE = "";
            string READ_TYPE = "",
                    UOM = "";

            for (int line_count = Convert.ToInt32(sections[utilityviewmodel.currentIndex][4]); line_count <= Convert.ToInt32(sections[utilityviewmodel.currentIndex][5]); line_count++)
            {
                utilityviewmodel.token = lines[line_count].Trim();
                if (SmartParseV2016.Token_Identify(utilityviewmodel, gas_readings, false))
                {
                    int sub_line_count = line_count;
                    while (sub_line_count <= Convert.ToInt32(sections[utilityviewmodel.currentIndex][5]))
                    {
                        utilityviewmodel.token = lines[sub_line_count].Replace(gas_readings, "").Trim();
                        if (utilityviewmodel.token.IndexOf("cubic metres") >= 0)
                        {
                            UOM = "m3";
                        }
                        if (utilityviewmodel.token.IndexOf(" to ") >= 0)
                        {
                            //TEMP_DATE = "";
                            if (readings_count == 0)
                            {
                                utilityviewmodel.D_THIS_READ = "";
                                utilityviewmodel.D_LAST_READ = "";
                                utilityviewmodel.D_UNITS_USED_M3 = "";
                                utilityviewmodel.D_UNITS_USED_KWH = "";
                                READ_TYPE = "";
                                utilityviewmodel.READINGS_PERIOD_START = SmartParametersV2016.defaultDates;
                                utilityviewmodel.READINGS_PERIOD_END = SmartParametersV2016.defaultDates;
                                utilityviewmodel.UNIT_OF_MEASURE = "";
                            }
                            else
                            {
                                utilityviewmodel.D_LAST_READ = "";
                                utilityviewmodel.D_THIS_READ = "";
                                utilityviewmodel.READINGS_PERIOD_START = SmartParametersV2016.defaultDates;
                                utilityviewmodel.READINGS_PERIOD_END = SmartParametersV2016.defaultDates;
                            }

                            if (!SectionH2_Check_Readings(ourviewmodel,
                                                        utilityviewmodel,
                                                        utilityviewmodel.token,
                                                        UOM))
                            {
                                return false;
                            }

                            if (SmartParseV2016.Add_Readings_Test(SmartParametersV2016.defaultDate, Convert.ToDateTime(utilityviewmodel.READINGS_PERIOD_START), Convert.ToDateTime(utilityviewmodel.READINGS_PERIOD_END), utilityviewmodel.D_LAST_READ, utilityviewmodel.D_THIS_READ, utilityviewmodel.UNIT_OF_MEASURE))
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
                                                                            utilityviewmodel.METER_SERIAL_NO,
                                                                            READ_TYPE,
                                                                            utilityviewmodel.D_LAST_READ,
                                                                            utilityviewmodel.D_THIS_READ,
                                                                            utilityviewmodel.D_UNITS_USED_M3,
                                                                            utilityviewmodel.UNIT_OF_MEASURE,
                                                                            utilityviewmodel.D_UNITS_USED_KWH,
                                                                            utilityviewmodel.CALORIFIC_VALUE);
                                        break;
                                    default:
                                        break;
                                }
                                readings_count++;
                            }
                        }
                        sub_line_count++;
                    }
                    line_count = sub_line_count;
                    goto the_good_old_daysH2;
                }
            the_good_old_daysH2:
                continue;
            }
            return true;
        }

        private static bool SectionH2_Check_Readings(MainViewModel ourviewmodel,
                                                        UtilityViewModel utilityviewmodel,
                                                        string token,
                                                        string UOM)
        {
            string TEMP_DATE = "";

            string[] components = token.Split(SmartParametersV2016.spaceSplit);
            int component_count = 0;
            while (component_count < components.Length)
            {
                switch (component_count)
                {
                    case 0:         // READINGS_PERIOD_START day
                        TEMP_DATE = components[component_count].Trim();
                        break;
                    case 1:         // READINGS_PERIOD_START month
                        TEMP_DATE = TEMP_DATE + SmartParametersV2016.space + components[component_count].Trim();
                        break;
                    case 2:         // READINGS_PERIOD_START year
                        TEMP_DATE = TEMP_DATE + SmartParametersV2016.space + components[component_count].Trim();
                        if (!SmartParseV2016.Generic_Parse_Datetime(TEMP_DATE, utilityviewmodel))
                        {
                            return false;
                        }
                        else
                        {
                            utilityviewmodel.READINGS_PERIOD_START = TEMP_DATE;
                        }
                        if (utilityviewmodel.BILL_PERIOD_START == utilityviewmodel.BILL_DATE)
                        {
                            SmartParseV2016.Update_Bill(ourviewmodel, utilityviewmodel, "BILL_PERIOD_START", TEMP_DATE);
                            utilityviewmodel.BILL_PERIOD_START = TEMP_DATE;
                        }
                        break;
                    case 3:         // to
                        if (components[component_count].Trim() != "to")
                        {
                            return false;
                        }
                        break;
                    case 4:         // TO_DATE day
                        TEMP_DATE = components[component_count].Trim();
                        break;
                    case 5:         // TO_DATE month
                        TEMP_DATE = TEMP_DATE + SmartParametersV2016.space + components[component_count].Trim();
                        break;
                    case 6:         // TO_DATE year
                        TEMP_DATE = TEMP_DATE + SmartParametersV2016.space + components[component_count].Trim();
                        if (!SmartParseV2016.Generic_Parse_Datetime(TEMP_DATE, utilityviewmodel))
                        {
                            return false;
                        }
                        else
                        {
                            utilityviewmodel.READINGS_PERIOD_END = TEMP_DATE;
                        }
                        SmartParseV2016.Update_Bill(ourviewmodel, utilityviewmodel, "BILL_PERIOD_END", TEMP_DATE);
                        break;
                    case 7:         // Meter Number
                        utilityviewmodel.METER_SERIAL_NO = components[component_count].Trim();
                        break;
                    case 8:         // Previous Read
                        utilityviewmodel.D_LAST_READ = components[component_count].Trim();
                        if (!SmartParseV2016.Generic_Parse_Integer(utilityviewmodel.D_LAST_READ, utilityviewmodel))
                        {
                            return false;
                        }
                        break;
                    case 9:         // Actual or Estimate
                        components[component_count] = components[component_count].Trim();
                        utilityviewmodel.READ_TYPE = Fix_Read_Type(components[component_count]);
                        break;
                    case 10:         // Present Read
                        utilityviewmodel.D_THIS_READ = components[component_count].Trim();
                        if (!SmartParseV2016.Generic_Parse_Integer(utilityviewmodel.D_THIS_READ, utilityviewmodel))
                        {
                            return false;
                        }
                        break;
                    case 11:         // Actual or Estimate (overrides previous)
                        components[component_count] = components[component_count].Trim();
                        utilityviewmodel.READ_TYPE = Fix_Read_Type(components[component_count]);
                        break;
                    case 12:        // Rate - Day or Night
                        if (components[component_count].Trim() == "Day")
                        {
                            //day_rate = true;
                        }
                        break;
                    case 13:        // kWh
                        utilityviewmodel.D_UNITS_USED_M3 = components[component_count].Trim();
                        if (!SmartParseV2016.Generic_Parse_Integer(utilityviewmodel.D_UNITS_USED_M3, utilityviewmodel))
                        {
                            return false;
                        }
                        // Mock up the Unit of Measure
                        utilityviewmodel.UNIT_OF_MEASURE = UOM;
                        break;
                    default:
                        break;
                }
                component_count++;
            }
            return true;
        }
        private static async Task<bool> Parse_SectionH3(MainViewModel ourviewmodel,
                                                         UtilityViewModel utilityviewmodel,
                                                         string[][] sections,    // Elec Charges
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

            char UNITS_TIME;// = SmartParametersV2016.daytimeUnit;
            utilityviewmodel.READINGS_PERIOD_START = SmartParametersV2016.defaultDates;
            utilityviewmodel.READINGS_PERIOD_END = SmartParametersV2016.defaultDates;
            string UNIT_CHARGES_PERIOD_START,// = SmartParametersV2016.defaultDates,
                        UNIT_CHARGES_PERIOD_END,// = SmartParametersV2016.defaultDates,
                        STANDING_CHARGES_PERIOD_START,// = SmartParametersV2016.defaultDates,
                        STANDING_CHARGES_PERIOD_END;// = SmartParametersV2016.defaultDates;
            utilityviewmodel.UNITS_TYPE = "";
            utilityviewmodel.UNITS = "";
            utilityviewmodel.UNITS_RATE = "";
            utilityviewmodel.UNITS_COST = "";
            utilityviewmodel.UNIT_OF_MEASURE = SmartParametersV2016.defaultUoM;

            utilityviewmodel.STANDING_CHARGE = "";
            utilityviewmodel.CHARGES_DAYS = "";
            utilityviewmodel.CHARGES_COST = "";
            string CHARGES_TYPE;// = "";


            string space_to_space = " to ",
                        Day = "Day",
                        Night = "Night",
                        kilowatt_hours = "kilowatt hours",
                        kilowatt_dash_hours = "kilowatt-hours",
                        Usage_charges = "Usage charges",
                        Standing_Charge = "Standing Charge",
                        Standing_charges = "Standing charges";
            short units_band,// = 0,
                        charges_item = 0; ;
            utilityviewmodel.tariff_line = 0;

            for (int line_count = Convert.ToInt32(sections[utilityviewmodel.currentIndex][4]); line_count <= Convert.ToInt32(sections[utilityviewmodel.currentIndex][5]); line_count++)
            {
                utilityviewmodel.token = lines[line_count].Trim();
                // This is a kludge!  Sometimes the ONLY place in the bill for the tariff is
                // after 'Electricity charges' ... go figure
                if (SectionH3_electricity_charges(utilityviewmodel,
                                                    utilityviewmodel.token,
                                                    line_count))
                {
                    goto the_good_old_daysH3;
                }
                if (!await SectionH3_Tariff_Code(ourviewmodel,
                                                 utilityviewmodel,
                                                 utilityviewmodel.tariff_line,
                                                line_count,
                                                utilityviewmodel.token))
                {
                    return false;
                }

                int to_index = utilityviewmodel.token.IndexOf(space_to_space);

                if (SmartParseV2016.Token_Identify(utilityviewmodel, Usage_charges, false) ||
                                                    to_index >= 0)
                {
                    int sub_line_count = line_count;
                    while (sub_line_count <= Convert.ToInt32(sections[utilityviewmodel.currentIndex][5]))
                    {
                        utilityviewmodel.token = lines[sub_line_count];

                        if (SmartParseV2016.Token_Identify(utilityviewmodel, Standing_Charge +
                                                                        SmartParametersV2016.bar +
                                                                        Standing_charges, false))
                        {
                            // Re-do this line
                            sub_line_count--;
                            break;
                        }

                        to_index = utilityviewmodel.token.IndexOf(space_to_space);
                        if (to_index >= 0)
                        {
                            if (!Fuck_About(utilityviewmodel,
                                                space_to_space))
                            {
                                return false;
                            }
                            else
                            {
                                lines[sub_line_count] = utilityviewmodel.token;
                            }

                            int sub_sub_line_count = sub_line_count;
                            while (sub_sub_line_count <= Convert.ToInt32(sections[utilityviewmodel.currentIndex][5]))
                            {
                                utilityviewmodel.token = lines[sub_sub_line_count].Trim();

                                if (SmartParseV2016.Token_Identify(utilityviewmodel, Standing_Charge +
                                                                        SmartParametersV2016.bar +
                                                                        Standing_charges, false) ||
                                    SmartParseV2016.Token_Identify_Middle(utilityviewmodel, space_to_space, false, ""))
                                {
                                    // Re-do this line
                                    sub_sub_line_count--;
                                    break;
                                }
                                if ((utilityviewmodel.token.IndexOf(Day) >= 0) ||
                                    (utilityviewmodel.token.IndexOf(Night) >= 0))
                                {
                                    UNITS_TIME = Convert.ToChar(utilityviewmodel.token.ToUpper());
                                    SectionH3_Time(utilityviewmodel,
                                                UNITS_TIME,
                                                Day,
                                                Night);

                                    units_band = 1;

                                    if ((utilityviewmodel.token.IndexOf(kilowatt_hours) >= 0) ||
                                        (utilityviewmodel.token.IndexOf(kilowatt_dash_hours) >= 0))
                                    {
                                        utilityviewmodel.token = utilityviewmodel.token.Replace(kilowatt_hours, "").Trim();
                                        utilityviewmodel.token = utilityviewmodel.token.Replace(kilowatt_dash_hours, "").Trim();
                                        utilityviewmodel.UNITS_TYPE = utilityviewmodel.UNITS_TYPE + SmartParametersV2016.space + kilowatt_hours;
                                        utilityviewmodel.UNITS_TYPE = (utilityviewmodel.UNITS_TYPE.Length <= SmartParametersV2016.TYPESLENGTH ? utilityviewmodel.UNITS_TYPE : utilityviewmodel.UNITS_TYPE.Substring(0, SmartParametersV2016.TYPESLENGTH));
                                    }

                                    utilityviewmodel.token = utilityviewmodel.token.Replace("used at", "").Trim();
                                    utilityviewmodel.token = utilityviewmodel.token.Replace("at", "kWh").Trim();
                                    utilityviewmodel.token = utilityviewmodel.token.Replace("each", "").Trim();
                                    utilityviewmodel.token = SmartParseV2016.Remove_Double_Spaces_V3(utilityviewmodel.token);

                                    if (!SectionH3_Units(utilityviewmodel,
                                                utilityviewmodel.token))
                                    {
                                        return false;
                                    }

                                    UNIT_CHARGES_PERIOD_START = utilityviewmodel.READINGS_PERIOD_START;
                                    UNIT_CHARGES_PERIOD_END = utilityviewmodel.READINGS_PERIOD_END;

                                    switch (utilityviewmodel.resource_code)
                                    {
                                        case SmartParametersV2016.Electricity:
                                            SmartUtilityV2022.E_Unit_ChargesList_Add(utilityviewmodel,
                                                                                SmartParametersV2016.Utility,
                                                                                utilityviewmodel.supplier_code,
                                                                                utilityviewmodel.brand_code,
                                                                                utilityviewmodel.ACCOUNT_NO,
                                                                                utilityviewmodel.CREATED,
                                                                                utilityviewmodel.STATEMENT_ID,
                                                                                utilityviewmodel.BILL_DATE,
                                                                                utilityviewmodel.sparks.MPAN,
                                                                                UNIT_CHARGES_PERIOD_START,
                                                                                UNIT_CHARGES_PERIOD_END,   // CHANGE THIS!
                                                                                UNITS_TIME,
                                                                                units_band.ToString(),
                                                                                utilityviewmodel.UNITS_TYPE,
                                                                                utilityviewmodel.UNITS,
                                                                                utilityviewmodel.UNITS_RATE,
                                                                                utilityviewmodel.UNIT_OF_MEASURE,
                                                                                utilityviewmodel.UNITS_COST);
                                            break;
                                        default:
                                            break;
                                    }
                                }
                                sub_sub_line_count++;
                            }
                            sub_line_count = sub_sub_line_count;
                        }
                        sub_line_count++;
                    }
                    line_count = sub_line_count;
                    goto the_good_old_daysH3;
                }

                if (SmartParseV2016.Token_Identify(utilityviewmodel, Standing_Charge, false))
                {
                    int sub_line_count = line_count;
                    while (sub_line_count <= Convert.ToInt32(sections[utilityviewmodel.currentIndex][5]))
                    {
                        utilityviewmodel.token = lines[sub_line_count].Trim();

                        if (SectionH3_check_line(ourviewmodel,
                                        utilityviewmodel,
                                        utilityviewmodel.token,
                                        lines,
                                        ref sub_line_count))
                        {
                            break;
                        }

                        CHARGES_TYPE = Standing_Charge;

                        utilityviewmodel.token = utilityviewmodel.token.Replace(Standing_Charge, "");
                        utilityviewmodel.token = utilityviewmodel.token.Replace("-", "").Trim();
                        utilityviewmodel.token = utilityviewmodel.token.Replace("days at", "").Trim();
                        utilityviewmodel.token = utilityviewmodel.token.Replace("per day", "").Trim();
                        utilityviewmodel.token = SmartParseV2016.Remove_Double_Spaces_V3(utilityviewmodel.token);

                        if (!SectionH3_Charges(utilityviewmodel,
                                                utilityviewmodel.token))
                        {
                            return false;
                        }

                        charges_item = (short)(charges_item + 1);

                        STANDING_CHARGES_PERIOD_START = utilityviewmodel.READINGS_PERIOD_START;
                        STANDING_CHARGES_PERIOD_END = utilityviewmodel.READINGS_PERIOD_END;

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
                                                                                STANDING_CHARGES_PERIOD_START,
                                                                                STANDING_CHARGES_PERIOD_END,
                                                                                charges_item.ToString(),
                                                                                CHARGES_TYPE,
                                                                                utilityviewmodel.STANDING_CHARGE,
                                                                                utilityviewmodel.CHARGES_DAYS,
                                                                                utilityviewmodel.CHARGES_COST);
                                break;
                            default:
                                break;
                        }
                        sub_line_count++;
                    }
                    line_count = sub_line_count;
                    goto the_good_old_daysH3;
                }

                if (SmartParseV2016.Token_Identify(utilityviewmodel, Standing_charges, false))
                {
                    int sub_line_count = line_count;
                    while (sub_line_count <= Convert.ToInt32(sections[utilityviewmodel.currentIndex][5]))
                    {
                        utilityviewmodel.token = lines[sub_line_count].Trim();

                        if (SectionH3_check_line(ourviewmodel,
                                                utilityviewmodel,
                                                utilityviewmodel.token,
                                                lines,
                                                ref sub_line_count))
                        {
                            break;
                        }

                        to_index = utilityviewmodel.token.IndexOf(space_to_space);
                        if (to_index >= 0)
                        {

                            CHARGES_TYPE = Standing_Charge;

                            if (!Fuck_About(utilityviewmodel,
                                        space_to_space))
                            {
                                return false;
                            }
                            else
                            {
                                lines[sub_line_count] = utilityviewmodel.token;
                            }
                            utilityviewmodel.token = utilityviewmodel.token.Replace("days at", "").Trim();
                            utilityviewmodel.token = utilityviewmodel.token.Replace("per day", "").Trim();
                            utilityviewmodel.token = SmartParseV2016.Remove_Double_Spaces_V3(utilityviewmodel.token);

                            if (!SectionH3_Charges(utilityviewmodel,
                                                    utilityviewmodel.token))
                            {
                                return false;
                            }

                            charges_item = (short)(charges_item + 1);

                            STANDING_CHARGES_PERIOD_START = utilityviewmodel.READINGS_PERIOD_START;
                            STANDING_CHARGES_PERIOD_END = utilityviewmodel.READINGS_PERIOD_END;

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
                                                                                    STANDING_CHARGES_PERIOD_START,
                                                                                    STANDING_CHARGES_PERIOD_END,
                                                                                    charges_item.ToString(),
                                                                                    CHARGES_TYPE,
                                                                                    utilityviewmodel.STANDING_CHARGE,
                                                                                    utilityviewmodel.CHARGES_DAYS,
                                                                                    utilityviewmodel.CHARGES_COST);
                                    break;
                                default:
                                    break;
                            }
                        }
                        sub_line_count++;
                    }
                    line_count = sub_line_count;
                    goto the_good_old_daysH3;
                }
            the_good_old_daysH3:
                continue;
            }
            return true;
        }

        private static bool SectionH3_electricity_charges(UtilityViewModel utilityviewmodel,
                                                            string token,
                                                            int line_count)
        {
            if (token.IndexOf("Electricity charges") >= 0)
            {
                if (utilityviewmodel.TARIFF_CODE == 0)
                {
                    utilityviewmodel.tariff_line = line_count + 1;
                }
                return true;
            }
            return false;
        }
        private static bool SectionH3_check_line(MainViewModel ourviewmodel,
                                                    UtilityViewModel utilityviewmodel,
                                                    string token,
                                                    string[] lines,
                                                    ref int sub_line_count)
        {
            string if_you_stop_paying_by = "If you stop paying by",
                    After_price_change = "After price change";
            if (token.IndexOf(if_you_stop_paying_by) >= 0)
            {
                utilityviewmodel.token = token;
                if (!SectionH3_Payment_Plan(ourviewmodel,
                                            utilityviewmodel,
                                            if_you_stop_paying_by))
                {
                    return false;
                }
                return true;    // Payment Plan token was passed by rf! So this doesn't make sense
            }
            if (token.IndexOf("Total charges") >= 0)
            {
                return true;
            }
            if (token.IndexOf(After_price_change) >= 0)
            {
                // Re-parse this line
                token = token.Replace(After_price_change, "");
                int dash_index = token.IndexOf(SmartParametersV2016.dash);
                if (dash_index >= 0)
                {
                    if (dash_index + 1 < token.Length)
                    {
                        token = token.Substring(dash_index + 1).Trim();
                    }
                }
                lines[sub_line_count] = token;
                sub_line_count--;
                return true;
            }
            return false;
        }
        private static async Task<bool> SectionH3_Tariff_Code(MainViewModel ourviewmodel,
                                                                 UtilityViewModel utilityviewmodel,
                                                                 int tariff_line,
                                                                int line_count,
                                                                string token)
        {
            bool status = true;

            if ((tariff_line > 0) &&
                    (line_count == tariff_line))
            {
                // Try to use this line as the tariff
                utilityviewmodel.TARIFF_NAME = token.Trim();
                if (!await SmartUtilityV2022.Lookup_Remote_Tariff_Code(ourviewmodel, utilityviewmodel))
                {
                    // ourviewmodel.errorMessage should be set here
                    status = false;
                }
                else
                {
                    SmartParseV2016.Update_Bill(ourviewmodel, utilityviewmodel, "TARIFF_CODE", utilityviewmodel.TARIFF_CODE.ToString());
                }
            }
            return status;
        }

        private static bool SectionH3_Payment_Plan(MainViewModel ourviewmodel,
                                                    UtilityViewModel utilityviewmodel,
                                                    string if_you_stop_paying_by)
        {
            if (SmartParseV2016.Token_Identify(utilityviewmodel, if_you_stop_paying_by, true))
            {
                int index = utilityviewmodel.token.IndexOf(",");
                if (index >= 0)
                {
                    utilityviewmodel.token = utilityviewmodel.token.Substring(0, index).Trim();
                    string[] components = utilityviewmodel.token.Split(SmartParametersV2016.spaceSplit);
                    utilityviewmodel.token = "";
                    foreach (string component in components)
                    {
                        string first_upper = component.Substring(0, 1).ToUpper();
                        if (component.Length > 1)
                        {
                            first_upper += component.Substring(1);
                        }
                        utilityviewmodel.token = utilityviewmodel.token + first_upper + SmartParametersV2016.space;
                    }
                    string PAYMENT_TYPE = utilityviewmodel.token.Trim();
                    if (!SmartSpikeUtilityV2017.Utility_Lookup_PaymentPlan(ourviewmodel,
                                                                utilityviewmodel,
                                                                PAYMENT_TYPE))
                    {
                        // ourviewmodel.errorMessage should be set here
                        return false;
                    }
                    SmartParseV2016.Update_Bill(ourviewmodel, utilityviewmodel, "PAYMENT_PLAN", utilityviewmodel.PAYMENT_PLAN.ToString());
                    SmartParseV2016.Update_Bill(ourviewmodel, utilityviewmodel, "PAYMENT_TYPE", PAYMENT_TYPE);
                }
            }
            return true;
        }

        private static bool Fuck_About(UtilityViewModel utilityviewmodel,
                                        string space_to_space)
        {
            string TEMP_DATE = "";
            utilityviewmodel.token = utilityviewmodel.token.Replace(space_to_space, SmartParametersV2016.space);
            utilityviewmodel.token = SmartParseV2016.Remove_Double_Spaces_V3(utilityviewmodel.token);

            string[] components = utilityviewmodel.token.Split(SmartParametersV2016.spaceSplit);
            utilityviewmodel.token = "";
            int components_count = 0;
            while (components_count < components.Length)
            {
                switch (components_count)
                {
                    case 0:
                        TEMP_DATE = components[components_count].Trim();
                        break;
                    case 1:
                        TEMP_DATE = TEMP_DATE + SmartParametersV2016.space + components[components_count].Trim();
                        break;
                    case 2:
                        TEMP_DATE = TEMP_DATE + SmartParametersV2016.space + components[components_count].Trim();

                        //dash_index = TEMP_DATE.IndexOf("-");
                        //if (dash_index >= 0)
                        //{
                        //    if (dash_index < TEMP_DATE.Length)
                        //    {
                        //        TEMP_DATE = TEMP_DATE.Substring(dash_index + 1).Trim();
                        //    }
                        //}
                        if (!SmartParseV2016.Generic_Parse_Datetime(TEMP_DATE, utilityviewmodel))
                        {
                            return false;
                        }
                        utilityviewmodel.READINGS_PERIOD_START = TEMP_DATE;
                        break;
                    case 3:
                        TEMP_DATE = components[components_count].Trim();
                        break;
                    case 4:
                        TEMP_DATE = TEMP_DATE + SmartParametersV2016.space + components[components_count].Trim();
                        break;
                    case 5:
                        TEMP_DATE = TEMP_DATE + SmartParametersV2016.space + components[components_count].Trim();

                        //dash_index = TEMP_DATE.IndexOf("-");
                        //if (dash_index >= 0)
                        //{
                        //    if (dash_index < TEMP_DATE.Length)
                        //    {
                        //        TEMP_DATE = TEMP_DATE.Substring(dash_index + 1).Trim();
                        //    }
                        //}
                        if (!SmartParseV2016.Generic_Parse_Datetime(TEMP_DATE, utilityviewmodel))
                        {
                            return false;
                        }
                        utilityviewmodel.READINGS_PERIOD_END = TEMP_DATE;
                        break;
                    default:
                        utilityviewmodel.token = utilityviewmodel.token + SmartParametersV2016.space + components[components_count];
                        break;
                }
                components_count++;
            }
            utilityviewmodel.token = utilityviewmodel.token.Trim();

            return true;
        }

        private static void SectionH3_Time(UtilityViewModel utilityviewmodel,
                                            char UNITS_TIME,
                                            string Day,
                                            string Night)
        {
            switch (UNITS_TIME)
            {
                case SmartParametersV2016.daytimeUnit:
                    utilityviewmodel.UNITS_TYPE = Day;
                    utilityviewmodel.token = utilityviewmodel.token.Replace(Day, "").Trim();
                    break;
                case SmartParametersV2016.nighttimeUnit:
                    utilityviewmodel.UNITS_TYPE = Night;
                    utilityviewmodel.token = utilityviewmodel.token.Replace(Night, "").Trim();
                    break;
                default:
                    break;
            }
            return;
        }
        private static bool SectionH3_Units(UtilityViewModel utilityviewmodel,
                                            string token)
        {
            string[] components = token.Split(SmartParametersV2016.spaceSplit);
            int component_count = 0;
            while (component_count < components.Length)
            {
                switch (component_count)
                {
                    case 0:     // Units
                        utilityviewmodel.UNITS = components[component_count].Replace(",", "").Trim();
                        if (!SmartParseV2016.Generic_Parse_Integer(utilityviewmodel.UNITS, utilityviewmodel))
                        {
                            return false;
                        }
                        break;
                    case 1:     // Unit of Measure
                        utilityviewmodel.UNIT_OF_MEASURE = components[component_count].Trim('(').Trim(')');
                        break;
                    case 2:     // Rate
                        utilityviewmodel.UNITS_RATE = components[component_count].Replace(utilityviewmodel.bill_denomination_symbol.ToString(), "").Trim();
                        if (!SmartParseV2016.Generic_Parse_Decimal(utilityviewmodel.UNITS_RATE, utilityviewmodel))
                        {
                            return false;
                        }
                        break;
                    case 3:     // Consumption_charge
                        if (!SmartParseV2016.Currency_Strip(utilityviewmodel.bill_currency_symbol, utilityviewmodel.bill_currency_separator, utilityviewmodel.bill_thousands_separator, components[component_count].Trim(), utilityviewmodel))
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
                component_count++;
            }
            return true;
        }
        private static bool SectionH3_Charges(UtilityViewModel utilityviewmodel,
                                                string token)
        {
            string[] components = token.Split(SmartParametersV2016.spaceSplit);
            int component_count = 0;
            while (component_count < components.Length)
            {
                switch (component_count)
                {
                    case 0:     // Days
                        utilityviewmodel.CHARGES_DAYS = components[component_count].Trim();
                        if (!SmartParseV2016.Generic_Parse_Integer(utilityviewmodel.CHARGES_DAYS, utilityviewmodel))
                        {
                            return false;
                        }
                        break;
                    case 1:
                        utilityviewmodel.STANDING_CHARGE = components[component_count].Replace(utilityviewmodel.bill_denomination_symbol.ToString(), "").Trim();
                        if (!SmartParseV2016.Generic_Parse_Decimal(utilityviewmodel.STANDING_CHARGE, utilityviewmodel))
                        {
                            return false;
                        }
                        break;
                    case 2:     // Cost
                        if (!SmartParseV2016.Currency_Strip(utilityviewmodel.bill_currency_symbol, utilityviewmodel.bill_currency_separator, utilityviewmodel.bill_thousands_separator, components[component_count].Trim(), utilityviewmodel))
                        {
                            return false;
                        }
                        else
                        {
                            utilityviewmodel.CHARGES_COST = utilityviewmodel.value;
                        }
                        if (!SmartParseV2016.Generic_Parse_Integer(utilityviewmodel.CHARGES_COST, utilityviewmodel))
                        {
                            return false;
                        }
                        break;
                    default:
                        break;
                }
                component_count++;
            }
            return true;
        }

        private static bool Parse_SectionH4(UtilityViewModel utilityviewmodel,
                                            string[][] sections,    // Gas
                                            string section,
                                            string sub_section,
                                            string[] lines)
        {
            utilityviewmodel.currentIndex = 0;
            if (!SmartParseV2016.Decode_Section(section, sub_section, sections, utilityviewmodel.resource_code.ToString(), utilityviewmodel))
            {
                return true;
            }

            char UNITS_TIME = SmartParametersV2016.daytimeUnit;
            DateTime READINGS_PERIOD_START = SmartParametersV2016.defaultDate;
            string TEMP_DATE;

            string UNITS_TYPE = "";
            utilityviewmodel.UNITS = "";
            utilityviewmodel.UNITS_RATE = "";
            utilityviewmodel.UNITS_COST = "";
            string CHARGES_TYPE;
            utilityviewmodel.STANDING_CHARGE = "";
            utilityviewmodel.CHARGES_DAYS = "0";
            utilityviewmodel.CHARGES_COST = "0";
            utilityviewmodel.UNIT_OF_MEASURE = "";
            string token,
                        Day = "Day",
                        kilowatt_hours = "kilowatt hours",
                        Standing_Charge = "Standing Charge";
            short units_band,
                  charges_item = 0;

            for (int line_count = Convert.ToInt32(sections[utilityviewmodel.currentIndex][4]); line_count <= Convert.ToInt32(sections[utilityviewmodel.currentIndex][5]); line_count++)
            {
                token = lines[line_count].Trim();
                int to_index = token.IndexOf(" to ");
                if (to_index >= 0)
                {
                    TEMP_DATE = token.Substring(0, to_index).Trim();
                    if (!SmartParseV2016.Generic_Parse_Datetime(TEMP_DATE, utilityviewmodel))
                    {
                        return false;
                    }
                    else
                    {
                        READINGS_PERIOD_START = utilityviewmodel.genericTargetDate;
                    }
                    int sub_line_count = line_count;
                    while (sub_line_count <= Convert.ToInt32(sections[utilityviewmodel.currentIndex][5]))
                    {
                        token = lines[sub_line_count].Trim();
                        if (token.IndexOf(Day) >= 0)
                        {
                            UNITS_TIME = Convert.ToChar(token.ToUpper());
                            switch (UNITS_TIME)
                            {
                                case SmartParametersV2016.daytimeUnit:
                                    UNITS_TYPE = Day;
                                    token = token.Replace(Day, "").Trim();
                                    break;
                                default:
                                    break;
                            }
                            units_band = 1;

                            if (token.IndexOf(kilowatt_hours) >= 0)
                            {
                                // Haven't done kilowatt-hours here 'cos it might be m3
                                token = token.Replace(kilowatt_hours, "").Trim();
                                UNITS_TYPE = UNITS_TYPE + SmartParametersV2016.space + kilowatt_hours;
                                UNITS_TYPE = UNITS_TYPE.Length <= SmartParametersV2016.TYPESLENGTH ? UNITS_TYPE : UNITS_TYPE.Substring(0, SmartParametersV2016.TYPESLENGTH);
                            }

                            token = token.Replace("used at", "").Trim();
                            token = token.Replace("each", "").Trim();
                            token = SmartParseV2016.Remove_Double_Spaces_V3(token);

                            if (!SectionH4_Check_Consumption_Charge(utilityviewmodel,
                                                                token))
                            {
                                return false;
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
                                                                                        READINGS_PERIOD_START.ToString(),
                                                                                        READINGS_PERIOD_START.ToString(),   // CHANGE THIS
                                                                                        units_band.ToString(),
                                                                                        UNITS_TYPE,
                                                                                        utilityviewmodel.UNITS,
                                                                                        utilityviewmodel.UNITS_RATE,
                                                                                        utilityviewmodel.UNIT_OF_MEASURE,
                                                                                        utilityviewmodel.UNITS_COST);
                                    break;
                                default:
                                    break;
                            }
                        }
                        else
                        {
                            if (token.IndexOf(Standing_Charge) >= 0)
                            {
                                while (sub_line_count <= Convert.ToInt32(sections[utilityviewmodel.currentIndex][5]))
                                {
                                    token = lines[sub_line_count].Trim();
                                    if (token.IndexOf("Total charges") >= 0)
                                    {
                                        break;
                                    }
                                    if (token.IndexOf("After price change") >= 0)
                                    {
                                        // Re-parse this line
                                        sub_line_count--;
                                        break;
                                    }
                                    if (token.IndexOf(Standing_Charge) >= 0)
                                    {
                                        CHARGES_TYPE = Standing_Charge;

                                        token = token.Replace(Standing_Charge, "").Trim();
                                        token = token.Replace("-", "").Trim();
                                        token = token.Replace("days at", "").Trim();
                                        token = token.Replace("per day", "").Trim();
                                        token = SmartParseV2016.Remove_Double_Spaces_V3(token);

                                        if (!SectionH4_Check_Charges(utilityviewmodel,
                                                    token))
                                        {
                                            return false;
                                        }

                                        charges_item = (short)(charges_item + 1);
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
                                                                                                READINGS_PERIOD_START.ToString(),
                                                                                                READINGS_PERIOD_START.ToString(),   // CHANGE THIS!
                                                                                                charges_item.ToString(),
                                                                                                CHARGES_TYPE,
                                                                                                utilityviewmodel.STANDING_CHARGE,
                                                                                                utilityviewmodel.CHARGES_DAYS,
                                                                                                utilityviewmodel.CHARGES_COST);
                                                break;
                                            default:
                                                break;
                                        }
                                    }
                                    sub_line_count++;
                                }
                                line_count = sub_line_count;
                                goto the_good_old_daysH4;
                            }
                        }
                        sub_line_count++;
                    }
                    line_count = sub_line_count;
                    goto the_good_old_daysH4;
                }
            the_good_old_daysH4:
                continue;
            }
            return true;
        }

        private static bool SectionH4_Check_Charges(UtilityViewModel utilityviewmodel,
                                                    string token)
        {
            string[] components = token.Split(SmartParametersV2016.spaceSplit);
            int component_count = 0;
            while (component_count < components.Length)
            {
                switch (component_count)
                {
                    case 0:     // Days
                        utilityviewmodel.CHARGES_DAYS = components[component_count].Trim();
                        if (!SmartParseV2016.Generic_Parse_Integer(utilityviewmodel.CHARGES_DAYS, utilityviewmodel))
                        {
                            return false;
                        }
                        break;
                    case 1:
                        utilityviewmodel.STANDING_CHARGE = components[component_count].Replace(utilityviewmodel.bill_denomination_symbol.ToString(), "").Trim();
                        if (!SmartParseV2016.Generic_Parse_Decimal(utilityviewmodel.STANDING_CHARGE, utilityviewmodel))
                        {
                            return false;
                        }
                        break;
                    case 2:     // Cost
                        if (!SmartParseV2016.Currency_Strip(utilityviewmodel.bill_currency_symbol, utilityviewmodel.bill_currency_separator, utilityviewmodel.bill_thousands_separator, components[component_count].Trim(), utilityviewmodel))
                        {
                            return false;
                        }
                        else
                        {
                            utilityviewmodel.CHARGES_COST = utilityviewmodel.value;
                        }
                        if (!SmartParseV2016.Generic_Parse_Integer(utilityviewmodel.CHARGES_COST, utilityviewmodel))
                        {
                            return false;
                        }
                        break;
                    default:
                        break;
                }
                component_count++;
            }
            return true;
        }

        private static bool SectionH4_Check_Consumption_Charge(UtilityViewModel utilityviewmodel,
                                                                string token)
        {
            string[] components = token.Split(SmartParametersV2016.spaceSplit);
            int component_count = 0;
            while (component_count < components.Length)
            {
                switch (component_count)
                {
                    case 0:     // Units
                        utilityviewmodel.UNITS = components[component_count].Replace(",", "").Trim();
                        if (!SmartParseV2016.Generic_Parse_Integer(utilityviewmodel.UNITS, utilityviewmodel))
                        {
                            return false;
                        }
                        break;
                    case 1:     // Unit of Measure
                        utilityviewmodel.UNIT_OF_MEASURE = components[component_count].Trim('(').Trim(')');
                        break;
                    case 2:     // Rate
                        utilityviewmodel.UNITS_RATE = components[component_count].Replace(utilityviewmodel.bill_denomination_symbol.ToString(), "").Trim();
                        if (!SmartParseV2016.Generic_Parse_Decimal(utilityviewmodel.UNITS_RATE, utilityviewmodel))
                        {
                            return false;
                        }
                        break;
                    case 3:     // Consumption_charge
                        if (!SmartParseV2016.Currency_Strip(utilityviewmodel.bill_currency_symbol, utilityviewmodel.bill_currency_separator, utilityviewmodel.bill_thousands_separator, components[component_count].Trim(), utilityviewmodel))
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
                component_count++;
            }
            return true;
        }
        private static bool Parse_SectionH5(UtilityViewModel utilityviewmodel,
                                            string[][] sections,
                                            string section,
                                            string sub_section)
        {
            utilityviewmodel.currentIndex = 0;
            if (!SmartParseV2016.Decode_Section(section, sub_section, sections, utilityviewmodel.resource_code.ToString(), utilityviewmodel))
            {
                return true;
            }

            //string token = "";

            for (int line_count = Convert.ToInt32(sections[utilityviewmodel.currentIndex][4]); line_count <= Convert.ToInt32(sections[utilityviewmodel.currentIndex][5]); line_count++)
            {
                //    token = lines[line_count].Trim();
                //    if (SmartParseV2016.Token_Identify(utilityviewmodel, "S", true, true))
                //    {
                //        if (SmartParseV2016.fix_supply_number(line_count,
                //                                 lines,
                //                                 sections[section_index][5],
                //                                 sections[section_index][4],
                //                                 utilityviewmodel,
                //                                 false))
                //        {
                //            if (utilityviewmodel.mpan.Length >= 22)
                //            {
                //                SmartUtilityV2022.Amelia_Update_Mpan(utilityviewmodel, utilityviewmodel.mpan);
                //            }
                //        }
                //        goto the_old_daysH5;
                //    }
                //the_old_daysH5:
                //    continue;
            }
            return true;
        }
    }
}