using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using iText.Kernel.Pdf;
using System.Text.Json;
using System.Diagnostics.CodeAnalysis;


#if ANDROIDX
using AndroidX.AppCompat.App;
#endif
namespace SmartCubeMobile
{
    public class NPowerV2016
    {
        [RequiresUnreferencedCode("Calls System.Text.Json.JsonSerializer.Serialize<TValue>(TValue, JsonSerializerOptions)")]
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
            // LEAVE THIS IN AND SEE IF THIS BOLLOCKS MAKES ANY DIFFERENCE (I don't think it does)

            //
            //********************************************************************
            // YES IT FUCKING DOES!!!!!   The PDF POST doesn't work without it!!!!
            // *******************************************************************
            //
            //string user_agent = "Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.2; WOW64; Trident/6.0)";

            //
            // THINK WE CAN GET AWAY WITHOUT THIS USER AGENT BOLLOCKS
            //
            //UrlMkSetSessionOption(URLMON_OPTION_USERAGENT, user_agent, user_agent.Length, 0);

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
            // Timer will tick evert second
            // FUCKING HELL - this is never Enabled????
            // You have 60 Seconds to stop
            // the timer on a good login!
            //string //midata_pathname = "",
            utilityviewmodel.youraccounts_pathname = ""; // "Your_Account/Account_Details/";

            List<SmartUtility.Bills> bills_tempList = new List<SmartUtility.Bills>();

            // There are two parts to this - and FOUR account numbers
            // The old part has THREE account nos - one Account and two
            // for each of the resources (Electricity and Gas)  We ignore
            // these last two, and use the new Main account for both
            // Tariff is DUAL FUEL
            // The new part has ONE account no - its the same for
            // both Electricity and Gas  We use this old Main account for both
            //
            // Good test this if TWO Utility.Accounts for the same MPAN/MPRN albeit
            // with the same Supplier (Brand).  We should be able to cope with
            // this ... (should)
            // Tariff is not DUAL FUEL, that's just their way of saying

            string next_routine = "",
                    //exit_test = "",
                    //yes_routine = "",
                    //no_routine = "",
                    //comments = "",
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
                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Comments: " + utilityviewmodel.comments + Environment.NewLine.ToString());

                await SmartUtilityV2022.First_Throw(ourviewmodel,
                                                    utilityviewmodel);
                                                   // DateTime.Now + ourviewmodel.utcOffset);   // Local time
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
                        utilityviewmodel.keyValues.Add(new KeyValuePair<string, string>("username", utilityviewmodel.user_id));
#if WINFORMS
                        if (utilityviewmodel.console)
                        {
                            await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Username: " + utilityviewmodel.user_id + Environment.NewLine.ToString());
                        }
#endif
#if WPF 
                        if (TextBox_Active)
                        {

                            await SmartRoutinesV2018.TextBlockUpdate(
                                ourviewmodel,
                                "UserId" +
                                SmartParametersV2016.space +
                                utilityviewmodel.user_id);

                        }
#endif
#if ANDROIDX
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
#endif
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

                        // Should be going onto "Login"

                        break;
                    case "Login":
                        // Try to login
                        utilityviewmodel.login_attempted = true;

                        if (await NP_Login(
#if ANDROIDX
                                        meterActivity,
#endif
                                        ourviewmodel,
                                        utilityviewmodel,
                                        utilityviewmodel.htmlDocument,
                                        TextBox_Active))
                        {
                            utilityviewmodel.target_pathname = "/at_home/Applications/Npower.Web.Login/Login.aspx/Login";
                            //await SmartBobV2017.Scraper_Generic_Post(keyValues, utilityviewmodel);
                            // Should now be going on to HOME
                        }
                        else
                        {
                            ourviewmodel.errorMessage = utilityviewmodel.current_routine;
                            utilityviewmodel.next_routine = "LOGOUT";
                            utilityviewmodel.login_finished = true;          // Escape route on Login failure
                        }
                        break;

                    case "VWALLBLL":
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
                        utilityviewmodel.current_page = 0;

                        string RequestVerificationToken = "";
                        IList<HtmlAgilityPack.HtmlNode> HtmlCol2 = YetAnotherFuckingHoop.SelectNodesAsList(utilityviewmodel.htmlDocument.DocumentNode, "//input");
                        foreach (HtmlAgilityPack.HtmlNode element2 in HtmlCol2)
                        {
                            string name = element2.GetAttributeValue("name", "");
                            if (name == SmartParametersV2016.requestVerificationToken)
                            {
                                // Need to add this as a cookie??
                                RequestVerificationToken = name +
                                                            SmartParametersV2016.space +
                                                            element2.GetAttributeValue("value", "");

                                break;
                            }
                        }

                        while (utilityviewmodel.next_routine == "VWALLBLL")
                        {

                            if (await NP_ViewAllBillsPayments(
#if ANDROIDX
                                                        meterActivity,
#endif
                                                        ourviewmodel,
                                                        utilityviewmodel,
                                                        utilityviewmodel.htmlDocument,
                                                        TextBox_Active,
                                                        utilityviewmodel.youraccounts_pathname))

                            {
                                // This ***DOESN'T*** GO AT THE END IN CASE THE
                                // NAVIGATE DOESN'T WORK - I don't know if this is still
                                // true, but I am too tired and too scared to care
                                // 26th August 2012 ... well I HAVE now put it at the end
                                // in order to try and fix the problem where Rick and Bitch's
                                // bill reading sometimes 'hangs'.  I have also put in code
                                // in the NAVIGATING to filter out Urls which just do a 
                                // javascript postback - all we are interested in are the Urls
                                // which allow us to View Bills ....
                                // element.InvokeMember("Click"); // <= Now put at the end
                                // Hook up the next routine to 'collect' the 'view' click on the 
                                // PDF file link.  There doesn't appear to be a 'Save' link as yet
                                // to click on (which would be much more preferable .. )
                                // But this way WORKS!
                                if (utilityviewmodel.next_routine != "VWALLBLL")
                                {
                                    // Should go to "ACCNTLST" to work through list
                                }
                                else
                                {
                                    utilityviewmodel.target_pathname = "/at_home/Applications/atlas.web/BillsandPayments.aspx/GetBillsAndPaymentHistory";

                                    object jsonpage = new { contractAccountNumber = utilityviewmodel.account_no, historyPageIndex = utilityviewmodel.current_page, displayPeriod = "0", billsAndPaymentsHistoryType = "All" };
                                    // ✅ System.Text.Json replacement
                                    string jsonString = JsonSerializer.Serialize(jsonpage);

                                    string postData = RequestVerificationToken;

                                    string answer = await SmartBobV2017.Scraper_Generic_Post_Json(ourviewmodel, utilityviewmodel.utilityToken, jsonString, utilityviewmodel, postData);
                                    if (!string.IsNullOrEmpty(ourviewmodel.errorMessage))
                                    {
                                        return false;
                                    }
                                    utilityviewmodel.htmlDocument.LoadHtml(answer);
                                }
                            }
                        }
                        break;
                    case "ACCNTSUM":            // We cycle round here LOTS

                        await NP_AccountsSummary(ourviewmodel,
                                                 utilityviewmodel,
                                                 utilityviewmodel.htmlDocument);

                        break;
                    // case "VWOLDBLL":
                    // Fuck me did THIS BASTARD NEARLY KILL ME OR WHAT?
                    // I spent ***THREE FUCKING WEEKS*** ON THIS SON OF A BITCH
                    // and it nigh on killed me.  Those FUCKING, FUCKING NP Foreigners
                    // Those FUCKING Foreigners .... all because the POST PDF needed
                    // a fucking USer-Agent string to make it work.  Those fucking
                    // Foreigners ....

                    case "LOGOUT":
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
                        NP_Do_Intermediates(ourviewmodel,
                                            utilityviewmodel,
                                            utilityviewmodel.next_routine,
                                            utilityviewmodel.htmlDocument);
                        break;
                }
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

        private static void NP_Do_Intermediates(MainViewModel ourviewmodel,
                                                UtilityViewModel utilityviewmodel,
                                                string next_routine,
                                                HtmlAgilityPack.HtmlDocument htmlDocument)
        {
            switch (next_routine)
            {
                case "HOME":                // Same as Account Summary
                    NP_YourAccount(htmlDocument,
                                   utilityviewmodel);
                    // Should go to "ACCNTSET" to get Name and Address
                    break;
                case "ACCNTSET":
                    NP_Accountsettings(htmlDocument,
                                            utilityviewmodel);
                    // Should go to "ACCNTPREF" to get Name and Address
                    break;
                case "ACCNTPREF":
                    NP_AccountPreferences(ourviewmodel,
                                                utilityviewmodel,
                                                htmlDocument);
                    // Should go to "ACCNTSUM" (same as start page My Account)
                    break;
                default:
                    break;
            }
            return;
        }

        private static async Task<bool> NP_Login(
#if ANDROIDX
                                            AppCompatActivity meterActivity,
#endif
                                        MainViewModel ourviewmodel,
                                        UtilityViewModel utilityviewmodel,
                                        HtmlAgilityPack.HtmlDocument document,
                                        bool TextBox_Active)
        {
            // However!  The npower fuckers re-direct us off to a 'secure' URL
            // which we have to cater for by unhooking this routine (i.e. wb_DocumentCompleted_Login)
            // and hooking on the routine to handle the 'secure' URL (i.e. wb_DocumentCompleted_SecureLogin)

            // Reset this

            IList<HtmlAgilityPack.HtmlNode>
                     HtmlCol;
            bool clicked = false;
            List<KeyValuePair<string, string>> keyValues = new List<KeyValuePair<string, string>>();

            HtmlCol = YetAnotherFuckingHoop.SelectNodesAsList(document.DocumentNode, "//input");
            foreach (HtmlAgilityPack.HtmlNode element1 in HtmlCol)
            {
                switch (element1.Id)
                {
                    case "Username":
                        // Set the user name in the username text box
                        string name = element1.GetAttributeValue("name", "");
                        keyValues.Add(new KeyValuePair<string, string>(name, utilityviewmodel.user_id));
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

                    case "Password":
                        //Type the password in the password text box
                        name = element1.GetAttributeValue("name", "");
                        keyValues.Add(new KeyValuePair<string, string>(name, utilityviewmodel.password));
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
                        // This next line may not be needed but I am so fucked and fraught with
                        // all this shit that I daren't take a chance               
                        //                    case "submit":

                        utilityviewmodel.login_attempted = true;
                        // Hook up the next routine - if the login is true - for the next 'Document Completed' delivery
                        utilityviewmodel.next_routine = "HOME";

                        if (TextBox_Active)
                        {
                            await SmartRoutinesV2018.TextBlockUpdate(
#if ANDROIDX
                                    meterActivity,
#endif
                                ourviewmodel,
                                "LogOnAttempted");
                        }
                        clicked = true;
                        break;
                    default:
                        break;
                }
                if (clicked)
                {
                    break;
                }
            }
            utilityviewmodel.keyValues = keyValues;
            // If our login was successful a 'secure' Document
            // will be delivered.
            // If our login was UNSUCCESSFUL, then no Document
            // will be delivered and the timer will tick away
            // the 60 seconds and exit the timer loop when it
            // reaches  0
            return clicked;
        }

        private static bool NP_YourAccount(HtmlAgilityPack.HtmlDocument document,
                                            UtilityViewModel utilityviewmodel)
        {
            // Fuck me!  It fucking WORKED!!!! See the re-direction to this 'secure'
            // routine from the initial fucking Login routine above!!!!!!!!!!!!
            // You ARE a fucking genius Ray!!  An absolute bravest of the brave
            // fucking genius.  Those visionless cunts you saw last Thursday have
            // ABSOLUTELY no idea what they are missing out on ....

            IList<HtmlAgilityPack.HtmlNode>
                HtmlCol1,
                HtmlCol2;
            bool clicked = false;


            // Is account closed ?
            HtmlCol1 = YetAnotherFuckingHoop.SelectNodesAsList(document.DocumentNode, "//div");
            foreach (HtmlAgilityPack.HtmlNode element1 in HtmlCol1)
            {
                if (string.IsNullOrEmpty(element1.Id))
                {
                    string class_name = element1.GetAttributeValue("class", "");
                    if (SmartNibbyV2016.Check_Classname(class_name, "wrapper form_message_box error", true))
                    {
                        HtmlCol2 = YetAnotherFuckingHoop.SelectNodesAsList(element1, "//h3");
                        foreach (HtmlAgilityPack.HtmlNode element2 in HtmlCol2)
                        {
                            if (!string.IsNullOrEmpty(element2.InnerText))
                            {
                                if (element2.InnerText == "This account is now closed")
                                {
                                    utilityviewmodel.account_status = 'N';
                                    clicked = true;
                                    break;
                                }
                            }
                        }
                    }
                }
                if (clicked)
                {
                    break;
                }
            }
            // Reset
            clicked = false;

            HtmlCol1 = YetAnotherFuckingHoop.SelectNodesAsList(document.DocumentNode, "//a");
            foreach (HtmlAgilityPack.HtmlNode element1 in HtmlCol1)
            {
                if (string.IsNullOrEmpty(element1.Id))
                {
                    if (!string.IsNullOrEmpty(element1.InnerText))
                    {
                        string inner_text = element1.InnerText.Trim();
                        if (inner_text == "YOUR ACCOUNT")
                        {
                            // At this point we should be logged in so ...
                            // We wouldn't be HERE unless we were logged in successfully
                            // Set this flag as were are 99.9% sure that we are logged-in
                            // NO - when we get a good SUPPLY ADDRESS 
                            utilityviewmodel.target_pathname = element1.GetAttributeValue(SmartParametersV2016.href, "");

                            utilityviewmodel.next_routine = "ACCNTSET";
                            if (!string.IsNullOrEmpty(utilityviewmodel.logout_pathname))
                            {
                                clicked = true;
                                break;
                            }
                        }
                        if (inner_text == "LOG OUT")
                        {
                            // At this point we should be logged in so ...
                            // We wouldn't be HERE unless we were logged in successfully
                            // Set this flag as were are 99.9% sure that we are logged-in
                            // NO - when we get a good SUPPLY ADDRESS 
                            utilityviewmodel.logout_pathname = element1.GetAttributeValue(SmartParametersV2016.href, "");
                            if (clicked)
                            {
                                break;
                            }
                        }
                    }
                }
            }
            if (!clicked)
            {
                utilityviewmodel.next_routine = "LOGOUT";
            }
            return clicked;
        }


        private static bool NP_Accountsettings(HtmlAgilityPack.HtmlDocument document,
                                                UtilityViewModel utilityviewmodel)
        {
            // Fuck me!  It fucking WORKED!!!! See the re-direction to this 'secure'
            // routine from the initial fucking Login routine above!!!!!!!!!!!!
            // You ARE a fucking genius Ray!!  An absolute bravest of the brave
            // fucking genius.  Those visionless cunts you saw last Thursday have
            // ABSOLUTELY no idea what they are missing out on ....

            IList<HtmlAgilityPack.HtmlNode>
                HtmlCol1;
            bool clicked = false;

            HtmlCol1 = YetAnotherFuckingHoop.SelectNodesAsList(document.DocumentNode, "//a");
            foreach (HtmlAgilityPack.HtmlNode element1 in HtmlCol1)
            {
                if (string.IsNullOrEmpty(element1.Id))
                {
                    if (!string.IsNullOrEmpty(element1.InnerText))
                    {
                        string inner_text = element1.InnerText.Trim();
                        if (inner_text == "Account settings")
                        {
                            // At this point we should be logged in so ...
                            // We wouldn't be HERE unless we were logged in successfully
                            // Set this flag as were are 99.9% sure that we are logged-in
                            // NO - when we get a good SUPPLY ADDRESS 
                            utilityviewmodel.target_pathname = element1.GetAttributeValue(SmartParametersV2016.href, "");
                            utilityviewmodel.next_routine = "ACCNTPREF";
                            clicked = true;
                            break;
                        }
                    }
                }
            }
            if (!clicked)
            {
                utilityviewmodel.next_routine = "LOGOUT";
            }
            return clicked;
        }

        private static bool NP_AccountPreferences(MainViewModel ourviewmodel,
                                                UtilityViewModel utilityviewmodel,
                                                HtmlAgilityPack.HtmlDocument document)
        {
            // Fuck me!  It fucking WORKED!!!! See the re-direction to this 'secure'
            // routine from the initial fucking Login routine above!!!!!!!!!!!!
            // You ARE a fucking genius Ray!!  An absolute bravest of the brave
            // fucking genius.  Those visionless cunts you saw last Thursday have
            // ABSOLUTELY no idea what they are missing out on ....

            IList<HtmlAgilityPack.HtmlNode>
                HtmlCol1;
            bool clicked = false;
            //bool postcode_found = false;

            HtmlCol1 = YetAnotherFuckingHoop.SelectNodesAsList(document.DocumentNode, "//input");
            foreach (HtmlAgilityPack.HtmlNode element1 in HtmlCol1)
            {
                if (!string.IsNullOrEmpty(element1.Id))
                {
                    NP_Account_Details(ourviewmodel,
                                    utilityviewmodel,
                                    element1);
                }
            }

            HtmlCol1 = YetAnotherFuckingHoop.SelectNodesAsList(document.DocumentNode, "//a");
            foreach (HtmlAgilityPack.HtmlNode element1 in HtmlCol1)
            {
                if (string.IsNullOrEmpty(element1.Id))
                {
                    if (!string.IsNullOrEmpty(element1.InnerText))
                    {
                        string inner_text = element1.InnerText.Trim();
                        if (inner_text == "Account summary")
                        {
                            utilityviewmodel.target_pathname = element1.GetAttributeValue(SmartParametersV2016.href, "");
                            utilityviewmodel.next_routine = "ACCNTSUM";
                            utilityviewmodel.youraccounts_pathname = utilityviewmodel.target_pathname;
                            clicked = true;
                            break;
                        }
                    }
                }
            }
            if (!clicked)
            {
                utilityviewmodel.next_routine = "LOGOUT";
            }
            return clicked;
        }

        //    // Fuck me!  It fucking WORKED!!!! See the re-direction to this 'secure'
        //    // routine from the initial fucking Login routine above!!!!!!!!!!!!
        //    // You ARE a fucking genius Ray!!  An absolute bravest of the brave
        //    // fucking genius.  Those visionless cunts you saw last Thursday have
        //    // ABSOLUTELY no idea what they are missing out on ....

        private static void NP_Account_Details(MainViewModel ourviewmodel,
                                                UtilityViewModel utilityviewmodel,
                                                HtmlAgilityPack.HtmlNode element1)
        {
            string value;// = "";
            switch (element1.Id)
            {
                case "PersonalInformation_Title":
                    value = element1.GetAttributeValue("value", "");
                    utilityviewmodel.account_name = NP_Process_Value(value, utilityviewmodel.account_name, SmartParametersV2016.space);
                    break;
                case "PersonalInformation_FirstName":
                    value = element1.GetAttributeValue("value", "");
                    utilityviewmodel.account_name = NP_Process_Value(value, utilityviewmodel.account_name, SmartParametersV2016.space);
                    break;
                case "PersonalInformation_LastName":
                    value = element1.GetAttributeValue("value", "");
                    utilityviewmodel.account_name = NP_Process_Value(value, utilityviewmodel.account_name);
                    break;

                case "PersonalInformation_DateOfBirthLimit":
                    value = element1.GetAttributeValue("value", "");
                    utilityviewmodel.account_dob = NP_Process_Value(value, utilityviewmodel.account_dob);
                    break;
                case "ContactInformation_HomeTelephoneNumber":
                    value = element1.GetAttributeValue("value", "");
                    utilityviewmodel.account_phone_no = NP_Process_Value(value, utilityviewmodel.account_phone_no);
                    break;
                case "ContactInformation_MobileTelephoneNumber":
                    if (string.IsNullOrEmpty(utilityviewmodel.account_phone_no))
                    {
                        // No home no = try to get the mobile
                        value = element1.GetAttributeValue("value", "");
                        utilityviewmodel.account_phone_no = NP_Process_Value(value, utilityviewmodel.account_phone_no);
                    }
                    break;
                case "ContactInformation_EmailAddress":
                    value = element1.GetAttributeValue("value", "");
                    utilityviewmodel.account_email = NP_Process_Value(value, utilityviewmodel.account_email);
                    break;
                default:
                    NP_Address(ourviewmodel,
                                utilityviewmodel,
                                element1);
                    break;
            }
            return;
        }

        private static void NP_Address(MainViewModel ourviewmodel,
                                        UtilityViewModel utilityviewmodel,
                                        HtmlAgilityPack.HtmlNode element1)

        {
            utilityviewmodel.postcode_found = false;
            string value;// = "";
            switch (element1.Id)
            {
                case "CommunicationAddress_HouseName":
                //value = element1.GetAttributeValue("value", "");
                //utilityviewmodel.account_address = NP_Process_Value(value, utilityviewmodel.account_address, SmartParametersV2016.bar.ToString());
                //break;
                case "CommunicationAddress_HouseNumber":
                    //value = element1.GetAttributeValue("value", "");
                    //utilityviewmodel.account_address = NP_Process_Value(value, utilityviewmodel.account_address, SmartParametersV2016.bar.ToString());
                    break;
                case "CommunicationAddress_AddressLine1":
                    //value = element1.GetAttributeValue("value", "");
                    //utilityviewmodel.account_address = NP_Process_Value(value, utilityviewmodel.account_address, SmartParametersV2016.bar.ToString());
                    break;
                case "CommunicationAddress_AddressLine2":
                    //value = element1.GetAttributeValue("value", "");
                    //utilityviewmodel.account_address = NP_Process_Value(value, utilityviewmodel.account_address, SmartParametersV2016.bar.ToString());
                    break;
                case "CommunicationAddress_Town":
                    value = element1.GetAttributeValue("value", "");
                    utilityviewmodel.account_address = NP_Process_Value(value, utilityviewmodel.account_address, SmartParametersV2016.bar.ToString());
                    break;
                case "CommunicationAddress_PostCode":
                    value = element1.GetAttributeValue("value", "");
                    // Note no trailing "|"
                    utilityviewmodel.account_address = NP_Process_Value(value, utilityviewmodel.account_address);
                    if (!utilityviewmodel.postcode_found)
                    {
                        if (SmartNibbyV2016.Derive_Postcode_New(value,   // Should be easy!
                                                    ourviewmodel.Blanche.workingPostcodesList,
                                                    utilityviewmodel))
                        {
                            utilityviewmodel.login_finished = true;
                            utilityviewmodel.postcode_found = true;
                        }
                    }
                    break;
                case "CommunicationAddress_Country":
                    // (not) Unsuprisingly - I couldn't give a shit about this
                    break;
            }
            return;
        }
        private static string NP_Process_Value(string value,
                                            string field_name,
                                            string trailer = "")
        {
            if (!string.IsNullOrEmpty(value))
            {
                field_name += value;
                if (!string.IsNullOrEmpty(trailer))
                {
                    field_name += trailer;
                }
            }
            return field_name;
        }

        private static async Task<bool> NP_AccountsSummary(MainViewModel ourviewmodel,
                                             UtilityViewModel utilityviewmodel,
                                             HtmlAgilityPack.HtmlDocument document)
        {
            // There should be one 'new account
            //      The new account may be dual fuel or not
            //      If the tariff is "Dual Fuel" then mock up 'E' and 'G'
            //          If 'E' not done do 'E'
            //          If 'G' not done do 'G'
            // There may or may not be 'old' Utility.Accounts
            // This whole thing is a fucking bitch ....

            IList<HtmlAgilityPack.HtmlNode>
                HtmlCol1,
                HtmlCol2;
            bool clicked = false;
            string class_name = "",
                   new_account_no = "",
                    account_address = "";
            SmartUtility.Accounts account_row = new SmartUtility.Accounts();

            utilityviewmodel.np_tariff_name = "";
            // It all depends on this!
            utilityviewmodel.target_pathname = "";

            // This is complicated shit, but it does the job for the structure above
            // AND its reasonably fast.  Basically we come back to this routine
            // twice for 'new' Utility.Accounts

            HtmlCol1 = YetAnotherFuckingHoop.SelectNodesAsList(document.DocumentNode, "//div");
            foreach (HtmlAgilityPack.HtmlNode element1 in HtmlCol1)
            {
                if (string.IsNullOrEmpty(element1.Id))
                {
                    class_name = element1.GetAttributeValue("class", "");
                    if (SmartNibbyV2016.Check_Classname(class_name, "summary-panel__heading", true))
                    {
                        HtmlCol2 = YetAnotherFuckingHoop.SelectNodesAsList(element1, ".//*");
                        foreach (HtmlAgilityPack.HtmlNode element2 in HtmlCol2)
                        {
                            if (element2.Name == "p")
                            {
                                class_name = element2.GetAttributeValue("class", "");
                                if (!string.IsNullOrEmpty(class_name))
                                {
                                    if (!string.IsNullOrEmpty(element2.InnerText))
                                    {
                                        if (element2.InnerText.IndexOf("Account number:") >= 0)
                                        {
                                            new_account_no = element2.InnerText.Replace("Account number:", "").Trim();
                                            char[] resource_code = new char[2];
                                            resource_code[0] = SmartParametersV2016.Electricity;
                                            resource_code[1] = SmartParametersV2016.Gas;
                                            // Its a 'new' account
                                            // Assume we do BOTH Gas and Electricity
                                            // although we never find either for 'new' Utility.Accounts on this page
                                            // Is the Electricity entry there?  For this account?

                                            clicked = NP_Do_This_Part(ourviewmodel,
                                                                        utilityviewmodel,
                                                                        resource_code,
                                                                        new_account_no);
                                        }
                                    }
                                }
                            }
                        }
                    }

                    if (SmartNibbyV2016.Check_Classname(class_name, "summary-panel__col", true))
                    {
                        HtmlCol2 = YetAnotherFuckingHoop.SelectNodesAsList(element1, ".//*");
                        utilityviewmodel.np_tariff_name = NP_Find_Tariff(HtmlCol2, utilityviewmodel);
                    }

                    if (SmartNibbyV2016.Check_Classname(class_name, "icon_inline", true))
                    {
                        HtmlCol2 = YetAnotherFuckingHoop.SelectNodesAsList(element1, ".//*");
                        foreach (HtmlAgilityPack.HtmlNode element2 in HtmlCol2)
                        {
                            if (element2.Name == "p")
                            {
                                if (!string.IsNullOrEmpty(element2.InnerText))
                                {
                                    account_address = element2.InnerText.Trim();
                                    account_address = account_address.Replace(SmartParametersV2016.tab.ToString(), "");
                                    account_address = account_address.Replace(SmartParametersV2016.comma, SmartParametersV2016.bar.ToString());
                                    utilityviewmodel.token = account_address;
                                    utilityviewmodel.token = SmartParseV2016.Remove_Double_Spaces_V3(utilityviewmodel.token);
                                    account_address = utilityviewmodel.token;

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

                                        // No we can set this because we have a good SUPPLY ADDRESS
                                        utilityviewmodel.login_finished = true;

                                        // Can do this now we have the area_code
                                        if (utilityviewmodel.TARIFF_CODE == 0)
                                        {
                                            await SmartUtilityV2022.Lookup_Remote_Tariff_Code(ourviewmodel, utilityviewmodel);
                                        }
                                    }
                                    if (clicked)
                                    {
                                        // i.e. we have an Account Number
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            if (clicked)
            {
#if WINFORMS
                clicked = await SmartNibbyV2016.Find_A_Tag(ourviewmodel,
#endif
#if WPF  || WINUI || SMARTMAUI
                clicked = SmartNibbyV2016.Find_A_Tag(ourviewmodel,
#endif
#if ANDROIDX
                clicked = SmartNibbyV2016.Find_A_Tag(ourviewmodel,
#endif
                                                        document,
                                                        utilityviewmodel,
                                                        "//a",
                                                        true,               // bool only_consider_null_elements,
                                                        "amp;",             // remove,
                                                        "Bills & payments", // inner_text_comparison,
                                                        "VWALLBLL");        // next_routine
            }

            if (!clicked ||
                string.IsNullOrEmpty(utilityviewmodel.target_pathname))
            {
                utilityviewmodel.next_routine = "LOGOUT";    // <= because we should already have the pathname
            }
            return clicked;
        }

        private static string NP_Find_Tariff(IList<HtmlAgilityPack.HtmlNode> HtmlCol2,
                                            UtilityViewModel utilityviewmodel)
        {
            foreach (HtmlAgilityPack.HtmlNode element2 in HtmlCol2)
            {
                if (element2.Name == "p")
                {
                    string class_name = element2.GetAttributeValue("class", "");
                    if (!string.IsNullOrEmpty(class_name))
                    {
                        if (!string.IsNullOrEmpty(element2.InnerText))
                        {
                            utilityviewmodel.np_tariff_name = element2.InnerText.Trim();
                            int carriageReturn = utilityviewmodel.np_tariff_name.IndexOf(SmartParametersV2016.carriageReturn);
                            if (carriageReturn >= 0)
                            {
                                utilityviewmodel.np_tariff_name = utilityviewmodel.np_tariff_name.Substring(carriageReturn);
                            }
                            break;
                        }
                    }
                }
            }
            return utilityviewmodel.np_tariff_name.Trim();
        }

        private static bool NP_Do_This_Part(MainViewModel ourviewmodel,
                                            UtilityViewModel utilityviewmodel,
                                            char[] resource_code,
                                            string new_account_no)
        {
            bool clicked = false;

            //List<Amelia> amelia_found = new List<Amelia>();

            bool found_resource = false;

            int resource_code_count = 0;
            while (resource_code_count < resource_code.Length)
            {
                utilityviewmodel.resource_code = resource_code[resource_code_count];
                utilityviewmodel.np_resource_type = "";
                switch (utilityviewmodel.resource_code)
                {
                    case SmartParametersV2016.Electricity:
                        if (SmartSpikeUtilityV2017.Utility_Lookup_ResourceName(ourviewmodel, utilityviewmodel, "Electricity"))
                        {
                            //utilityviewmodel.resource_code = utilityviewmodel.resource_code;
                            if (SmartSpikeUtilityV2017.Utility_Lookup_ResourceType(ourviewmodel, utilityviewmodel, utilityviewmodel.resource_code, ""))
                            {
                                SmartUtilityV2022.Fix_Resource_Type(utilityviewmodel, utilityviewmodel.np_resource_type);
                                found_resource = true;
                            }
                        }
                        break;
                    case SmartParametersV2016.Gas:
                        if (SmartSpikeUtilityV2017.Utility_Lookup_ResourceName(ourviewmodel, utilityviewmodel, "Gas"))
                        {
                            //utilityviewmodel.resource_code = utilityviewmodel.resource_code;
                            if (SmartSpikeUtilityV2017.Utility_Lookup_ResourceType(ourviewmodel, utilityviewmodel, utilityviewmodel.resource_code, ""))
                            {
                                SmartUtilityV2022.Fix_Resource_Type(utilityviewmodel, utilityviewmodel.np_resource_type);
                                found_resource = true;
                            }
                        }
                        break;
                    default:
                        break;
                }
                if (!found_resource)
                {
                    return false;
                }
                utilityviewmodel.account_no = new_account_no;
                // Doesn't matter about the MPAN/MPRN - only ever do one of each 'E' or 'G'
                // Ignore the MPAN/MPRN even though the original code includes it?
                List<Amelia> amelia_found = SmartUtilityV2022.Amelia_Lookup(ourviewmodel, utilityviewmodel, false);
                if (amelia_found.Count == 0)
                {
                    SmartUtilityV2022.Amelia_Add(utilityviewmodel);
                    clicked = true;
                    resource_code_count = resource_code.Length;    // Gets us out of the loop
                }
                resource_code_count++;
            }
            return clicked;
        }

#if WINFORMS
        private static async Task<bool> NP_Trailer(MainViewModel ourviewmodel,
                                        HtmlAgilityPack.HtmlDocument htmlDocument,
                                        UtilityViewModel utilityviewmodel)

#endif
#if WPF  || WINUI || SMARTMAUI
        private static bool NP_Trailer(MainViewModel ourviewmodel,
                                        HtmlAgilityPack.HtmlDocument htmlDocument,
                                        UtilityViewModel utilityviewmodel)

#endif
#if ANDROIDX
        private static bool NP_Trailer(MainViewModel ourviewmodel,
                                        HtmlAgilityPack.HtmlDocument htmlDocument,
                                        UtilityViewModel utilityviewmodel)
        
#endif
        {
            IList<HtmlAgilityPack.HtmlNode>
                HtmlCol1;

            bool clicked = false;

            HtmlCol1 = YetAnotherFuckingHoop.SelectNodesAsList(htmlDocument.DocumentNode, "//a");
            foreach (HtmlAgilityPack.HtmlNode element1 in HtmlCol1)
            {
                if (string.IsNullOrEmpty(element1.Id))
                {
                    string class_name = element1.GetAttributeValue("class", "");
                    if (SmartNibbyV2016.Check_Classname(class_name, "page-number", true))
                    {
                        // We have found a page - is it greater than the one we are on?
                        if (!string.IsNullOrEmpty(element1.InnerText))
                        {
                            string inner_text = element1.InnerText.Replace("&lt;", "").Trim();
                            inner_text = inner_text.Replace("&gt;", "");
                            inner_text = inner_text.Replace("<", "");
                            inner_text = inner_text.Replace(">", "");
                            if (!string.IsNullOrEmpty(inner_text))
                            {
                                short page_no = Convert.ToInt16(inner_text);
                                if (page_no > utilityviewmodel.current_page)
                                {
                                    // Click on this page
                                    utilityviewmodel.current_page = page_no;
#if WINFORMS
                                    await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Next page: " + page_no + Environment.NewLine.ToString());
#endif
                                    clicked = true;
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            return clicked;
        }
        private static async Task<bool> NP_ViewAllBillsPayments(
#if ANDROIDX
                                                    AppCompatActivity meterActivity,
#endif
                                                    MainViewModel ourviewmodel,
                                                    UtilityViewModel utilityviewmodel,
                                                    HtmlAgilityPack.HtmlDocument document,
                                                    bool TextBox_Active,
                                                    string accountsum_pathname)
        {
            // I have to say Ray, 26-Feb-2016 07:03 that you are an ABSOLUTELY FABULOUSLY TOUGH
            // SUPERB SYSTEM PROGRAMMER.  You are the BEST OF THE ABSOLUTE BEST.  No-one can touch
            // you when it comes to this stuff.  This has worked and worked and worked and worked
            // this morning and you only started it at 11pm last night!  What a CODER!!

            IList<HtmlAgilityPack.HtmlNode>
                HtmlCol1,
                HtmlCol2,
                HtmlCol3;

            bool clicked;

            HtmlCol1 = YetAnotherFuckingHoop.SelectNodesAsList(document.DocumentNode, "//div");
            foreach (HtmlAgilityPack.HtmlNode element1 in HtmlCol1)
            {
                if (string.IsNullOrEmpty(element1.Id))
                {
                    // Fucking NP Foreigners may have MORE THAN ONE TABLE WITH THIS class name!!
                    // Would you fucking believe it??
                    string class_name = element1.GetAttributeValue("class", "");
                    if (SmartNibbyV2016.Check_Classname(class_name, "bill_payment_history", false))
                    {
                        HtmlCol2 = YetAnotherFuckingHoop.SelectNodesAsList(element1, ".//table");
                        foreach (HtmlAgilityPack.HtmlNode element2 in HtmlCol2)
                        {
                            if (string.IsNullOrEmpty(element2.Id))
                            {
                                // Fucking NP Foreigners may have MORE THAN ONE TABLE WITH THIS class name!!
                                // Would you fucking believe it??
                                class_name = element2.GetAttributeValue("class", "");
                                if (SmartNibbyV2016.Check_Classname(class_name, "full_width striped", false))
                                {
                                    HtmlCol3 = YetAnotherFuckingHoop.SelectNodesAsList(element2, ".//tbody");

                                    if (!await NPBillsShit(
#if ANDROIDX
                                                            meterActivity,
#endif
                                                            ourviewmodel,
                                                            utilityviewmodel,
                                                            TextBox_Active,
                                                            HtmlCol3))

                                    {
                                        goto the_old_days;
                                    }
                                    break;  // Only do 1st table of this name!  In case there are
                                            // MORE THAN ONE!!  Which there are!!  Foreigners ...!!!
                                }
                            }
                        }
                    }
                }
            }
        the_old_days:
#if WINFORMS
            clicked = await NP_Trailer(ourviewmodel, document, utilityviewmodel);
#endif
#if WPF  || WINUI || SMARTMAUI
            clicked = NP_Trailer(ourviewmodel, document, utilityviewmodel);
#endif
#if ANDROIDX
            clicked = NP_Trailer(ourviewmodel, document, utilityviewmodel);
#endif
            if (!clicked)
            {
#if WINFORMS
                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "No more pages" + Environment.NewLine.ToString());
#endif
                // No more pages to do - go back to the Utility.Accounts Summary
                // Mock this up until we find out how to do it properly. We now know!
                utilityviewmodel.target_pathname = accountsum_pathname;
                utilityviewmodel.next_routine = "ACCNTSUM";    // Choose from old and new SmartUtility.Accounts
                clicked = true;
            }
            return clicked;
        }

        private static async Task<bool> NPBillsShit(
#if ANDROIDX
                                                    AppCompatActivity meterActivity,
#endif
                                                    MainViewModel ourviewmodel,
                                                    UtilityViewModel utilityviewmodel,
                                                    bool TextBox_Active,
                                                    IList<HtmlAgilityPack.HtmlNode> HtmlCol3)
        {
            IList<HtmlAgilityPack.HtmlNode>
                            HtmlCol4,
                            HtmlCol5;

            short payment_code = 0;
            utilityviewmodel.payment_item = 0;


            utilityviewmodel.PAYMENT_DATE = SmartParametersV2016.defaultDates;
            utilityviewmodel.PAYMENT_METHOD = "";
            utilityviewmodel.PAYMENT_AMOUNT = "";
            utilityviewmodel.PAYMENT_BALANCE = "";

            utilityviewmodel.href = "";
            DateTime payment_date = SmartParametersV2016.defaultDate;


#if WINFORMS
            if (utilityviewmodel.console)
            {
                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Page: " + utilityviewmodel.current_page + Environment.NewLine.ToString());
            }
#endif
            // This fucking clumsy oaf
            foreach (HtmlAgilityPack.HtmlNode element3 in HtmlCol3)
            {
                HtmlCol4 = YetAnotherFuckingHoop.SelectNodesAsList(element3, ".//tr");
                foreach (HtmlAgilityPack.HtmlNode element4 in HtmlCol4)
                {
                    int column_count = 0;
                    utilityviewmodel.href = "";
                    string out_line = "";
                    HtmlCol5 = YetAnotherFuckingHoop.SelectNodesAsList(element4, ".//td");
                    foreach (HtmlAgilityPack.HtmlNode element5 in HtmlCol5)
                    {
                        if (!string.IsNullOrEmpty(element5.InnerText))
                        {
                            string inner_text = element5.InnerText.Replace(Environment.NewLine, "").Trim();
                            if (!string.IsNullOrEmpty(inner_text))
                            {
                                out_line = out_line + inner_text + SmartParametersV2016.space;
                                switch (column_count)
                                {
                                    case 4:
                                        // Account Balance
                                        if (!Check_Account_Balance(utilityviewmodel,
                                                                    inner_text))
                                        {
                                            return false;
                                        }

                                        if (string.IsNullOrEmpty(utilityviewmodel.href))
                                        {
                                            NP_Update_Payments(ourviewmodel,
                                                                utilityviewmodel,
                                                                payment_date,
                                                                Convert.ToInt32(utilityviewmodel.PAYMENT_AMOUNT),
                                                                payment_code,
                                                                Convert.ToInt32(utilityviewmodel.PAYMENT_BALANCE));
                                        }
                                        else
                                        {

                                            // Is this an Invoice (a bill?)
                                            // The href contains the entire path, the value after the = is the bill date

                                            await NP_Read_Bill(
#if ANDROIDX
                                                                meterActivity,
#endif
                                                                ourviewmodel,
                                                                utilityviewmodel,
                                                                TextBox_Active,
                                                                utilityviewmodel.href);

                                        }
                                        break;
                                    default:
                                        if (!NP_Do_All_Other_Columns(ourviewmodel,
                                                    utilityviewmodel,
                                                    column_count,
                                                    inner_text,
                                                    element4))
                                        {
                                            return false;
                                        }
                                        break;
                                }
                            }
                            column_count++;
                        }
                    }
#if WINFORMS
                    if (utilityviewmodel.console)
                    {
                        await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Line: " + out_line + Environment.NewLine.ToString());
                    }
#endif
                }
            }
            return true;
        }

        private static bool NP_Do_All_Other_Columns(MainViewModel ourviewmodel,
                                                    UtilityViewModel utilityviewmodel,
                                                    int column_count,
                                                    string inner_text,
                                                    HtmlAgilityPack.HtmlNode element4)
        {

            switch (column_count)
            {
                case 0:
                    // Date
                    if (!Check_Date(utilityviewmodel,
                                    inner_text))
                    {
                        return false;
                    }
                    if (utilityviewmodel.current_page == 0)
                    {
                        // We have found a table of rows - assume it is page 1
                        utilityviewmodel.current_page = 1;
                    }
                    break;
                case 1:
                    // Description
                    if (!Check_Description(ourviewmodel,
                                    utilityviewmodel,
                                    inner_text,
                                    element4))
                    {
                        return false;
                    }
                    utilityviewmodel.PAYMENT_METHOD = inner_text;
                    break;
                case 2:
                    // Credits
                    if (!Check_Credit(utilityviewmodel,
                                    inner_text))
                    {
                        return false;
                    }
                    break;
                case 3:
                    // Debits / Account Balance
                    if (!Check_Debit(utilityviewmodel,
                                    inner_text))
                    {
                        return false;
                    }
                    break;
                default:
                    break;
            }
            return true;
        }

        private static void NP_Update_Payments(MainViewModel ourviewmodel,
                                                UtilityViewModel utilityviewmodel,
                                                DateTime payment_date,
                                                int payment_amount,
                                                short payment_code,
                                                int payment_balance)
        {
            if ((payment_date != SmartParametersV2016.defaultDate) &&
                 (payment_amount != 0))
            {
                utilityviewmodel.payment_item = (short)(utilityviewmodel.payment_item + 1);
                if (!SmartParseV2016.Check_TempList(ourviewmodel,
                                                    utilityviewmodel,
                                                    payment_date,
                                                    utilityviewmodel.payment_item,
                                                    payment_code,
                                                    payment_amount,
                                                    payment_balance,
                                                    utilityviewmodel.payments_tempList))
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
                    //                                    payment_date,
                    //                                    payment_item,
                    //                                    payment_code,
                    //                                    payment_amount,
                    //                                    payment_balance);
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

                }
            }
            return;
        }
        private static bool NP_Test_Bill(UtilityViewModel utilityviewmodel,
                                            string href)
        {
            // Is this an Invoice (a bill?)
            // The href contains the entire path, the value after the = is the bill date
            utilityviewmodel.statement_id = "";
            string billDate = "billDate=";
            int billdate_index = href.IndexOf(billDate);
            if (billdate_index >= 0)
            {
                string webpage_id = href.Substring(billdate_index, href.Length - billdate_index);
                webpage_id = webpage_id.Replace(billDate, "");
                webpage_id = Uri.UnescapeDataString(webpage_id);
                webpage_id = SmartParseV2016.Date_Delimiters(webpage_id);
                if (!SmartParseV2016.Generic_Parse_Datetime(webpage_id, utilityviewmodel))
                {
                    return false;
                }
                else
                {
                    utilityviewmodel.bill_date = utilityviewmodel.genericTargetDate;
                }
                utilityviewmodel.statement_id = webpage_id;
                if (SmartUtilityV2022.Check_Bill_Date(utilityviewmodel))
                {
                    // Statement Date is the one that appears in 'Processed:' line
                    //statement_date = utilityviewmodel.bill_date.ToString(SmartParametersV2016.bill_number_format);
                    // Have we already done this bill in a previous Read Meter?
                    utilityviewmodel.bills_out = true;
                    utilityviewmodel.bills_resource_out = true;
                    string mpan_mprn = SmartUtilityV2022.Determine_MPAN_MPRN(utilityviewmodel);
                    if (SmartUtilityV2022.Bill_Already_Done(utilityviewmodel, mpan_mprn))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        private static async Task<bool> NP_Read_Bill(
#if ANDROIDX
                                                    AppCompatActivity meterActivity,
#endif
                                                    MainViewModel ourviewmodel,
                                                    UtilityViewModel utilityviewmodel,
                                                    bool TextBox_Active,
                                                    string href)
        {
            utilityviewmodel.bills_out = false;
            utilityviewmodel.bills_resource_out = false;
            if (NP_Test_Bill(utilityviewmodel, href))
            {
                // We haven't already done this bill in this current Read Meter
                Uri TargetUrl = new Uri(SmartParametersV2016.localWebsite);
                if (!SmartNibbyV2016.Create_Uri(ourviewmodel,
                                                utilityviewmodel.prfix_xxx,
                                                href))
                {
                    ourviewmodel.errorMessage = utilityviewmodel.current_routine + SmartParametersV2016.bar + ourviewmodel.errorMessage;
                    return false;
                }
                TargetUrl = ourviewmodel.TargetUrl;
                // Start with a clean sheet ...
                ourviewmodel.pdfMessage = "";
                // Get the fucking PDF

#if WINFORMS || WPF  || WINUI || SMARTMAUI
                PdfReader pdfreader =
#endif
#if ANDROIDX
                iText.Kernel.Pdf.PdfReader pdfreader =
#endif
                await SmartBobV2017.HTTPCLIENT_GET_PDF_ASYNC(ourviewmodel,
                                                            utilityviewmodel.utilityToken,
                                                            TargetUrl,
                                                            utilityviewmodel.guid);

                if (!string.IsNullOrEmpty(ourviewmodel.errorMessage) || !string.IsNullOrEmpty(ourviewmodel.pdfMessage))
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
                    if (!await NP_parse_bill(ourviewmodel,
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
                // End of COMMON PART
            }
            return true;
        }

        private static bool Check_Date(UtilityViewModel utilityviewmodel,
                                        string inner_text)
        {
            //DateTime payment_date = SmartParametersV2016.defaultDate;

            utilityviewmodel.PAYMENT_DATE = inner_text;
            utilityviewmodel.PAYMENT_DATE = SmartParseV2016.Date_Delimiters(utilityviewmodel.PAYMENT_DATE);
            if (!SmartParseV2016.Generic_Parse_Datetime(utilityviewmodel.PAYMENT_DATE, utilityviewmodel))
            {
                return false;
            }
            return true;
        }

        private static bool Check_Description(MainViewModel ourviewmodel,
                                        UtilityViewModel utilityviewmodel,
                                        string inner_text,
                                        HtmlAgilityPack.HtmlNode element4)
        {
            IList<HtmlAgilityPack.HtmlNode>
                            HtmlCol6;

            utilityviewmodel.PAYMENT_METHOD = inner_text;
            if ((utilityviewmodel.PAYMENT_METHOD.IndexOf("Your bill") >= 0) ||
                (utilityviewmodel.PAYMENT_METHOD.IndexOf("Your final bill") >= 0) ||
                (utilityviewmodel.PAYMENT_METHOD.IndexOf("Your invoice") >= 0) ||
                (utilityviewmodel.PAYMENT_METHOD.IndexOf("Your final invoice") >= 0))
            {
                HtmlCol6 = YetAnotherFuckingHoop.SelectNodesAsList(element4, ".//a");
                foreach (HtmlAgilityPack.HtmlNode element6 in HtmlCol6)
                {
                    utilityviewmodel.href = element6.GetAttributeValue(SmartParametersV2016.href, "");
                    utilityviewmodel.PAYMENT_METHOD = "";
                    break;
                }
            }
            if (!string.IsNullOrEmpty(utilityviewmodel.PAYMENT_METHOD))
            {
                if (!SmartSpikeUtilityV2017.Utility_Lookup_PaymentCode(ourviewmodel,
                                                                        utilityviewmodel,
                                                                        utilityviewmodel.PAYMENT_METHOD))
                {
                    return false;
                }
                //utilityviewmodel.PAYMENT_METHOD = utilityviewmodel.PAYMENT_METHOD;
            }
            return true;
        }

        private static bool Check_Credit(UtilityViewModel utilityviewmodel,
                                        string inner_text)
        {
            utilityviewmodel.PAYMENT_AMOUNT = "";

            string payment_amount = inner_text;

            payment_amount = payment_amount.Replace(utilityviewmodel.bill_currency_separator.ToString(), "");
            payment_amount = payment_amount.Replace(utilityviewmodel.bill_thousands_separator.ToString(), "");
            utilityviewmodel.value = payment_amount;
            SmartParseV2016.Add_Minus_Sign(utilityviewmodel);
            payment_amount = utilityviewmodel.value;
            if (!SmartParseV2016.Generic_Parse_Integer(payment_amount, utilityviewmodel))
            {
                return false;
            }
            utilityviewmodel.PAYMENT_AMOUNT = payment_amount;
            return true;
        }

        private static bool Check_Debit(UtilityViewModel utilityviewmodel,
                                        string inner_text)
        {
            utilityviewmodel.PAYMENT_AMOUNT = "";

            string payment_amount = inner_text;
            payment_amount = payment_amount.Replace(utilityviewmodel.bill_currency_separator.ToString(), "");
            payment_amount = payment_amount.Replace(utilityviewmodel.bill_thousands_separator.ToString(), "");
            if (!SmartParseV2016.Generic_Parse_Integer(payment_amount, utilityviewmodel))
            {
                return false;
            }
            utilityviewmodel.PAYMENT_AMOUNT = payment_amount;
            return true;
        }

        private static bool Check_Account_Balance(UtilityViewModel utilityviewmodel,
                                                    string inner_text)
        {
            string payment_balance = inner_text.Trim();
            if (payment_balance.IndexOf("CR") >= 0)
            {
                payment_balance = payment_balance.Replace("CR", "");
                utilityviewmodel.value = payment_balance;
                SmartParseV2016.Add_Minus_Sign(utilityviewmodel);
                payment_balance = utilityviewmodel.value;
            }
            else
            {
                if (payment_balance.IndexOf("DR") >= 0)
                {
                    payment_balance = payment_balance.Replace("DR", "");
                }
            }
            utilityviewmodel.PAYMENT_BALANCE = "";
            payment_balance = payment_balance.Replace(utilityviewmodel.bill_currency_separator.ToString(), "").Trim();
            payment_balance = payment_balance.Replace(utilityviewmodel.bill_thousands_separator.ToString(), "");
            if (!SmartParseV2016.Generic_Parse_Integer(payment_balance, utilityviewmodel))
            {
                return false;
            }
            utilityviewmodel.PAYMENT_BALANCE = payment_balance;
            return true;
        }
        //
        // This fucking fucking shit made by those fucking fucking Foreigners
        // These fucking NP Foreigners have made me OLD
        //                          =================
        // These FUCKING FUCKING Foreigners have done my head in
        // We need to build a fucking table ...  No - we need to get a life!!!

        internal static async Task<bool> NP_parse_bill(MainViewModel ourviewmodel,
                                                    UtilityViewModel utilityviewmodel,
#if WINFORMS || WPF  || WINUI  || SMARTMAUI
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

                utilityviewmodel.sections = new[] { new string[] { "A", "0", "", "", "0", "0", "" },
                                        new string[] { "B", "0", "", "Your electricity and gas statement", "-1", "-1", "" },
                                        new string[] { "B", "1", "", "Your electricity statement" + SmartParametersV2016.bar +
                                                                                "Your electricity statement - estimated" + SmartParametersV2016.bar +
                                                                                "Your final electricity bill", "-1", "-1", "" },
                                        new string[] { "B", "2", "", "Your gas statement" + SmartParametersV2016.bar +
                                                                                "Your gas statement - estimated" + SmartParametersV2016.bar +
                                                                                "Your final gas bill", "-1", "-1", "" },
                                        new string[] { "H", "1", "E", "Electricity summary", "-1", "-1", "" },
                                        new string[] { "H", "2", "G", "Gas summary", "-1", "-1", "" },
                                        new string[] { "I", "0", "", "How your Direct Debit account adds up", "-1", "-1", "" }
                                      };
                // This is always needed anyway because we always do Page1 with this
                // Sometimes ... the Bill Date is never extracted from the PDF text!
                // But when (if) we evert find it - we substitute the Bill Number for it
                // IF we haven't found the Bill number ...
                bool success = Parse_Page0(ourviewmodel,
                                                utilityviewmodel,
                                                lines);
                if (!success ||
                    !SmartParseV2016.Check_Big_Four(ourviewmodel, utilityviewmodel, true))
                {
                    status = false;
                    // Noisy fucking bitch is banging and crashing and thumping and
                    // stomping around the kitchen again.  She is NEVER quiet. NEVER
                    //                                                         =====
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
                // Yes Sophie and her partner should go to Australia ...
                // they're young they should take their chance and go for it ..
                // Stupid bitch FORGETS as ALWAYS how she wasted my time AND
                // over £3,000 of my money arranging for US to go to Canada
                // and then the stupid bitch got cold feet and changed what
                // little piece of a fucking mind she has ... stupid useless cow


                // This is the only sensible place to do this because sometimes
                // they come BEFORE a total ... and sometime they come AFTER...
                SmartParseV2016.Update_Tariff_Details(ourviewmodel, utilityviewmodel);
                // fucking bitch is talking about her fucking job AGAIN
                // i don't fucking care about fucking verity or fucking aileish
            }
            return status;
        }


        //        // 27-Jan-2016  My ignorant, rude, petulant, insufferable bitch of a 'wife'
        //        // got her arse well and truly out last night stomping and thumping around the bedroom
        //        // in her usual thick, stupid, obnoxious way.  ODSBD and I wont have to put up with
        //        // her tantrums anymore ... ODSBD ... I can't wait ...

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
            if (!Parse_SectionB0(ourviewmodel, utilityviewmodel, utilityviewmodel.sections, "B", "0", lines))
            {
                ourviewmodel.errorMessage = "B0" + SmartParametersV2016.bar + ourviewmodel.errorMessage;
                return false;
            }
            if (!Parse_SectionB0(ourviewmodel, utilityviewmodel, utilityviewmodel.sections, "B", "1", lines))
            {
                ourviewmodel.errorMessage = "B1" + SmartParametersV2016.bar + ourviewmodel.errorMessage;
                return false;
            }
            if (!Parse_SectionB0(ourviewmodel, utilityviewmodel, utilityviewmodel.sections, "B", "2", lines))
            {
                ourviewmodel.errorMessage = "B2" + SmartParametersV2016.bar + ourviewmodel.errorMessage;
                return false;
            }
            return status;
        }
        private static async Task<bool> Parse_Pages(MainViewModel ourviewmodel,
                                        UtilityViewModel utilityviewmodel,
                                        string[] lines)
        {
            // Not perfect but not a bad start
            bool status = true;


            if (!await Parse_SectionH1(ourviewmodel, utilityviewmodel, utilityviewmodel.sections, "H", "1", lines)) // Electricity

            {
                ourviewmodel.errorMessage = "H1" + SmartParametersV2016.bar + ourviewmodel.errorMessage;
                return false;
            }

            if (!await Parse_SectionH2(ourviewmodel, utilityviewmodel, utilityviewmodel.sections, "H", "2", lines)) // Gas

            {
                ourviewmodel.errorMessage = "H2" + SmartParametersV2016.bar + ourviewmodel.errorMessage;
                return false;
            }

            // Fucking silly cow cannot add 3 to 2014!!
            if (!Parse_SectionI0(ourviewmodel, utilityviewmodel, utilityviewmodel.sections, "I", "0", lines)) // Total charges
            {
                ourviewmodel.errorMessage = "I0" + SmartParametersV2016.bar + ourviewmodel.errorMessage;
                return false;
            }

            return status;
        }

        private static bool Parse_SectionA0(MainViewModel ourviewmodel,
                                            UtilityViewModel utilityviewmodel,
                                            string[][] sections,
                                            string section,
                                            string sub_section,
                                            string[] lines)
        {
            utilityviewmodel.token = "";
            bool status = true;
            bool postcode_found = false;

            utilityviewmodel.currentIndex = 0;
            if (!SmartParseV2016.Decode_Section(section, sub_section, sections, utilityviewmodel.resource_code.ToString(), utilityviewmodel))
            {
                // We are here because we don't want (or need) to do this section
                return true;    // Always - because we haven't had a CONVERSION error
            }

            // Pass 1 - look for the Big One (!!)
            //int line_count = 0;
            for (int line_count = Convert.ToInt32(sections[utilityviewmodel.currentIndex][4]); line_count <= Convert.ToInt32(sections[utilityviewmodel.currentIndex][5]); line_count++)
            {
                utilityviewmodel.token = lines[line_count].Trim();

                if (!postcode_found &&
                    (utilityviewmodel.token.Length <= 8))
                {
                    string account_postcode = utilityviewmodel.token;
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
            return status;
        }

        private static bool Parse_SectionB0(MainViewModel ourviewmodel,
                                            UtilityViewModel utilityviewmodel,
                                            string[][] sections,
                                            string section,
                                            string sub_section,
                                            string[] lines)
        {
            utilityviewmodel.currentIndex = 0;
            if (!SmartParseV2016.Decode_Section(section, sub_section, sections, utilityviewmodel.resource_code.ToString(), utilityviewmodel))
            {
                // We are here because we don't want (or need) to do this section
                return true;    // Always - because we haven't had a CONVERSION error
            }

            utilityviewmodel.token = "";

            string your_account_number = "Your account number",
                    this_is_your_account_balance = "This is your account balance";
            char PAYMENT_PLAN = SmartParametersV2016.defaultChar;
            bool RESOURCE_BALANCES = false; // Default
            int NEW_CHARGES = 0,
                    RESOURCE_DISCOUNTS = 0,
                    RESOURCE_VAT_AMOUNT = 0,
                    TARIFF_CODE = 0;
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
            //int TEMP = 0;
            bool status = true;
            //string temp_urgent_message = "";

            //DateTime temp_date = SmartParametersV2016.defaultDate;
            // Pass 1 - look for the Big Three
            // int line_count = 0;
            for (int line_count = Convert.ToInt32(sections[utilityviewmodel.currentIndex][4]); line_count <= Convert.ToInt32(sections[utilityviewmodel.currentIndex][5]); line_count++)
            {
                utilityviewmodel.token = lines[line_count].Trim();

                if (SmartParseV2016.Token_Identify(utilityviewmodel, your_account_number, true))
                {
                    if (string.IsNullOrEmpty(utilityviewmodel.ACCOUNT_NO))
                    {
                        string temp_account = "";
                        int sub_line_count = line_count;
                        while (sub_line_count < Convert.ToInt32(sections[utilityviewmodel.currentIndex][5]))
                        {
                            utilityviewmodel.token = lines[sub_line_count].Replace(SmartParametersV2016.space, "").Trim();
                            if (SmartParseV2016.Generic_Parse_Digits(utilityviewmodel.token, utilityviewmodel))
                            {
                                temp_account = utilityviewmodel.token;
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
                            if (string.IsNullOrEmpty(utilityviewmodel.account_no) &&
                                !string.IsNullOrEmpty(utilityviewmodel.ACCOUNT_NO))
                            {
                                utilityviewmodel.account_no = utilityviewmodel.ACCOUNT_NO;
                            }
                        }
                    }
                    goto the_old_daysB0;
                }

                if (!string.IsNullOrEmpty(utilityviewmodel.ACCOUNT_NO))
                {
                    if (utilityviewmodel.BILL_DATE == SmartParametersV2016.defaultDates)
                    {
                        foreach (string month in SmartParametersV2016.months)
                        {
                            if (utilityviewmodel.token.IndexOf(SmartParametersV2016.space + month + SmartParametersV2016.space) >= 0)
                            {
                                Parse_SectionB0_dates_shit(utilityviewmodel);
                                //utilityviewmodel.token = utilityviewmodel.token.Replace("st", "");
                                //utilityviewmodel.token = utilityviewmodel.token.Replace("nd", "");
                                //utilityviewmodel.token = utilityviewmodel.token.Replace("rd", "");
                                //utilityviewmodel.token = utilityviewmodel.token.Replace("th", "");
                                utilityviewmodel.BILL_DATE = utilityviewmodel.token;
                                if (!SmartParseV2016.Generic_Parse_Datetime(utilityviewmodel.BILL_DATE, utilityviewmodel))
                                {
                                    return false;
                                }
                                //else
                                //{
                                //    temp_date = utilityviewmodel.genericTargetDate;
                                //}
                                SmartParseV2016.Update_Bill(ourviewmodel, utilityviewmodel, "BILL_DATE", utilityviewmodel.BILL_DATE);
                                goto the_old_daysB0;
                            }
                        }
                    }
                }

                if (SmartParseV2016.Token_Identify(utilityviewmodel, this_is_your_account_balance, true))
                {
                    int sub_line_count = line_count;
                    while (sub_line_count < Convert.ToInt32(sections[utilityviewmodel.currentIndex][5]))
                    {
                        utilityviewmodel.token = lines[sub_line_count].Trim();
                        int currency_index = utilityviewmodel.token.IndexOf(utilityviewmodel.bill_currency_symbol);
                        if (currency_index >= 0)
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
                            break;
                        }
                        sub_line_count++;
                    }
                    line_count = sub_line_count;
                    goto the_old_daysB0;
                }

            the_old_daysB0:
                continue;
            }

            // This is a special for NPower because the Foreigners don't have Bill periods
            // decribed at the top of their statements
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

        private static void Parse_SectionB0_dates_shit(UtilityViewModel utilityviewmodel)
        {
            utilityviewmodel.token = utilityviewmodel.token.Replace("st", "");
            utilityviewmodel.token = utilityviewmodel.token.Replace("nd", "");
            utilityviewmodel.token = utilityviewmodel.token.Replace("rd", "");
            utilityviewmodel.token = utilityviewmodel.token.Replace("th", "");
            return;
        }

        private static async Task<bool> Parse_SectionH1(MainViewModel ourviewmodel,
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
                // We are here because we don't want (or need) to do this section
                return true;    // Always - because we haven't had a CONVERSION error
            }

            utilityviewmodel.meter_serial_no = "";

            char UNITS_TIME = SmartParametersV2016.daytimeUnit;

            utilityviewmodel.token = "";
            utilityviewmodel.READINGS_PERIOD_START = SmartParametersV2016.defaultDates;
            utilityviewmodel.READINGS_PERIOD_END = SmartParametersV2016.defaultDates;
            utilityviewmodel.UNIT_CHARGES_PERIOD_START = SmartParametersV2016.defaultDates;
            utilityviewmodel.UNIT_CHARGES_PERIOD_END = SmartParametersV2016.defaultDates;
            utilityviewmodel.STANDING_CHARGES_PERIOD_START = SmartParametersV2016.defaultDates;
            utilityviewmodel.STANDING_CHARGES_PERIOD_END = SmartParametersV2016.defaultDates;
            short DISCOUNT_VAT_CODE = SmartParametersV2016.zeroRateVatCode;
            utilityviewmodel.units_band = 0;
            utilityviewmodel.charges_item = 0;
            utilityviewmodel.discount_item = 0;

            string electricity_summary = "Electricity summary",
                    electricity_account = "Electricity account",
                    tariff = "Tariff",
                    charges_for_tariff = "Charges for Tariff",
                    meter = "Meter:",
                    cost_of_electricity_used_this_period = "Cost of electricity used this period";
            string tariff_name = "Tariff Name:",
                    payment_plan = "Payment Plan:",
                    meter_number = "Meter Number:",
                    VAT_at = "VAT at",
                    kWh = "kWh",
                    dual_fuel_discount = "Dual fuel discount",
                    direct_debit_discount = "Direct Debit discount";

            utilityviewmodel.total_mantissa = 0;
            utilityviewmodel.first_amount = "";
            utilityviewmodel.next_amount = "";
            utilityviewmodel.first_line = "";
            utilityviewmodel.next_line = "";
            utilityviewmodel.kwh_line = "";

            string[] fuck = new string[2] { "", "" };

            utilityviewmodel.two_tier_meter = false;

            for (int line_count = Convert.ToInt32(sections[utilityviewmodel.currentIndex][4]); line_count <= Convert.ToInt32(sections[utilityviewmodel.currentIndex][5]); line_count++)
            {
                utilityviewmodel.token = lines[line_count].Trim();

                if (SmartParseV2016.Token_Identify(utilityviewmodel, electricity_account +
                                                                SmartParametersV2016.bar +
                                                                electricity_summary, true))
                {
                    SectionH1_Account_No(ourviewmodel, utilityviewmodel);
                    goto the_old_daysH1;
                }

                if (SmartParseV2016.Token_Identify(utilityviewmodel,
                                                    tariff +
                                                    SmartParametersV2016.bar +
                                                    charges_for_tariff +
                                                    SmartParametersV2016.bar +
                                                    meter,
                                                    false))
                {
                    utilityviewmodel.token = SmartParseV2016.Remove_Double_Spaces_V3(utilityviewmodel.token);
                    utilityviewmodel.token = utilityviewmodel.token.Replace("/", SmartParametersV2016.bar + payment_plan);
                    utilityviewmodel.token = utilityviewmodel.token.Replace("Charges for Tariff - ", tariff_name);
                    utilityviewmodel.token = utilityviewmodel.token.Replace(tariff + " - ", tariff_name);
                    if (utilityviewmodel.token.IndexOf(meter) >= 0)
                    {
                        utilityviewmodel.two_tier_meter = true;
                    }
                    utilityviewmodel.token = utilityviewmodel.token.Replace(meter, meter_number);
                    utilityviewmodel.token = utilityviewmodel.token.Replace(meter_number, SmartParametersV2016.bar + meter_number);
                    string[] components = utilityviewmodel.token.Trim().Split(SmartParametersV2016.bar);

                    if (!await SectionH1_Tariff_Payment_Plan(ourviewmodel, utilityviewmodel, components))
                    {
                        return false;
                    }

                    // Not needed in H1
                    //if (SmartParseV2016.Token_Identify(utilityviewmodel, meter_number, true))
                    //{
                    //    METER_SERIAL_NO = utilityviewmodel.token.Replace(meter_number, "").Trim();
                    //    METER_SERIAL_NO = METER_SERIAL_NO.Substring(0, METER_SERIAL_NO.Length <= SmartParametersV2016.METERSERIALLENGTH ? METER_SERIAL_NO.Length : SmartParametersV2016.METERSERIALLENGTH); //Enforce 14 char max
                    //    goto the_old_daysH1;
                    //    // The fucking Archers is on A FUCKING GAIN
                    //}

                    // Old two-tier Bills
                    if (utilityviewmodel.two_tier_meter)
                    {
                        utilityviewmodel.two_tier_meter = Suck_Out_Two_Tier(utilityviewmodel,
                                                ref line_count,
                                                sections,
                                                lines,
                                                utilityviewmodel.currentIndex);
                        if (lines[line_count].IndexOf(kWh) >= 0)
                        {
                            utilityviewmodel.kwh_line = lines[line_count].Trim();
                        }
                    }
                    goto the_old_daysH1;
                    // The fucking Archers is on A-FUCKING-GAIN
                }

                if (utilityviewmodel.token.IndexOf(SmartParametersV2016.billDateDelimiter) >= 0)
                {
                    int sub_line_count = line_count;

                    utilityviewmodel.READINGS_PERIOD_START = SmartParametersV2016.defaultDates;
                    utilityviewmodel.READINGS_PERIOD_END = SmartParametersV2016.defaultDates;

                    if (!SectionH1_Big_Loop(ourviewmodel,
                                                utilityviewmodel,
                                                    ref line_count,
                                                    sections,
                                                    utilityviewmodel.currentIndex,
                                                    lines,
                                                    fuck,
                                                    UNITS_TIME,
                                                    ref sub_line_count))
                    //utilityviewmodel.meter_serial_no))
                    {
                        return false;
                    }
                    // Stupid cow cant tell the difference between BROWN and BLUE ....
                    // OR between Alisdair Campbell and Nick Clegg!!!!
                    line_count = sub_line_count;
                    goto the_old_daysH1;
                }

                if (SmartParseV2016.Token_Identify(utilityviewmodel, "S", true, true))
                {
                    // 2012 is such a one-off
                    SectionH1_2012_One_Off(utilityviewmodel);
                    goto the_old_daysH1;
                }

                if (SmartParseV2016.Token_Identify(utilityviewmodel, cost_of_electricity_used_this_period, true))
                {
                    utilityviewmodel.token = utilityviewmodel.token.Replace(utilityviewmodel.bill_currency_symbol, "").Trim();
                    utilityviewmodel.token = utilityviewmodel.token.Replace(utilityviewmodel.bill_currency_separator.ToString(), "");
                    utilityviewmodel.token = utilityviewmodel.token.Replace(utilityviewmodel.bill_thousands_separator.ToString(), "");
                    SmartParseV2016.Update_Bill(ourviewmodel, utilityviewmodel, "RESOURCE_NEW_CHARGES", utilityviewmodel.token);
                    goto the_old_daysH1;
                }

                if (SmartParseV2016.Token_Identify(utilityviewmodel, direct_debit_discount, true))
                {
                    if (!SectionH1_Direct_Debit_Discount(ourviewmodel,
                                                            utilityviewmodel,
                                                            direct_debit_discount,
                                                            DISCOUNT_VAT_CODE))
                    {
                        return false;
                    }
                    goto the_old_daysH1;
                }

                if (SmartParseV2016.Token_Identify(utilityviewmodel, dual_fuel_discount, true))
                {
                    if (!SectionH1_Dual_Fuel_Discounts(ourviewmodel,
                                                        utilityviewmodel,
                                                        dual_fuel_discount,
                                                        DISCOUNT_VAT_CODE))
                    {
                        return false;
                    }
                    goto the_old_daysH1;
                }

                if (SmartParseV2016.Token_Identify(utilityviewmodel, VAT_at, true))
                {
                    short VAT_CODE = SmartParametersV2016.zeroRateVatCode;
                    if (!SmartParseV2016.Determine_Vat_Code(ourviewmodel,
                                                            utilityviewmodel,
                                                            utilityviewmodel.token,
                                                            VAT_at))
                    {
                        return false;
                    }
                    SmartParseV2016.Update_Bill(ourviewmodel, utilityviewmodel, "RESOURCE_VAT_CODE", VAT_CODE.ToString());
                    utilityviewmodel.token = utilityviewmodel.token.Replace("on", "");
                    utilityviewmodel.token = utilityviewmodel.token.Replace("+ " + utilityviewmodel.bill_currency_symbol, "+" + utilityviewmodel.bill_currency_symbol);
                    utilityviewmodel.token = SmartParseV2016.Remove_Double_Spaces_V3(utilityviewmodel.token);
                    string[] components = utilityviewmodel.token.Split(SmartParametersV2016.spaceSplit);
                    if (!SectionH1_VAT_At(ourviewmodel, utilityviewmodel, components))
                    {
                        return false;
                    }
                    goto the_old_daysH1;
                }
            the_old_daysH1:
                continue;
            }
            return true;
        }

        private static bool SectionH1_Big_Loop(MainViewModel ourviewmodel,
                                                UtilityViewModel utilityviewmodel,
                                                    ref int line_count,
                                                    string[][] sections,
                                                    int section_index,
                                                    string[] lines,
                                                    string[] fuck,
                                                    char UNITS_TIME,
                                                    ref int sub_line_count)//,
                                                                           //string METER_SERIAL_NO)
        {
            string Standing_Charge = "Standing Charge",
                    VAT_at = "VAT at",
                    kWh = "kWh";

            utilityviewmodel.D_UNITS_USED_KWH = "";
            utilityviewmodel.N_UNITS_USED_KWH = "";
            utilityviewmodel.UNIT_OF_MEASURE = "";
            utilityviewmodel.THIS_READ_TYPE = "";
            utilityviewmodel.D_THIS_READ = "";
            utilityviewmodel.N_THIS_READ = "";
            utilityviewmodel.LAST_READ_TYPE = "";
            utilityviewmodel.D_LAST_READ = "";
            utilityviewmodel.N_LAST_READ = "";
            utilityviewmodel.TOTAL_UNITS_USED = "";
            utilityviewmodel.UNITS_TYPE = "";
            utilityviewmodel.UNITS_RATE = "";
            utilityviewmodel.UNITS = "";
            utilityviewmodel.UNITS_COST = ""; // Never used

            utilityviewmodel.token = "";

            while (sub_line_count < Convert.ToInt32(sections[section_index][5]))
            {
                // The Readings come in 3s 'Last Reading and 'This Reading'
                utilityviewmodel.token = lines[sub_line_count];
                if ((utilityviewmodel.token.IndexOf(Standing_Charge) >= 0) ||
                    (utilityviewmodel.token.IndexOf(VAT_at) >= 0) ||
                    (utilityviewmodel.token == "Hour"))
                {
                    break;
                }

                if ((utilityviewmodel.token.IndexOf(kWh) >= 0) ||
                    ((utilityviewmodel.token == "24") && utilityviewmodel.two_tier_meter))
                {
                    if (SectionH1_Check_Two_Tier(utilityviewmodel.two_tier_meter,
                                                        utilityviewmodel.first_line,
                                                        utilityviewmodel.next_line,
                                                        utilityviewmodel.first_amount,
                                                        utilityviewmodel.next_amount,
                                                        utilityviewmodel.kwh_line))
                    {
                        utilityviewmodel.first_line = utilityviewmodel.first_line.Replace(" at ", SmartParametersV2016.space);
                        utilityviewmodel.next_line = utilityviewmodel.next_line.Replace(" at ", SmartParametersV2016.space);
                        utilityviewmodel.token = utilityviewmodel.first_line + SmartParametersV2016.space +
                                    utilityviewmodel.first_amount +
                                    SmartParametersV2016.newline +
                                    utilityviewmodel.next_line + SmartParametersV2016.space +
                                    utilityviewmodel.next_amount;

                        utilityviewmodel.token = SmartParseV2016.Remove_Double_Spaces_V3(utilityviewmodel.token);
                        utilityviewmodel.token = utilityviewmodel.token.Replace(SmartParametersV2016.space, SmartParametersV2016.bar.ToString());

                        if (utilityviewmodel.kwh_line.IndexOf(kWh) >= 0)
                        {
                            utilityviewmodel.UNIT_OF_MEASURE = kWh;
                            utilityviewmodel.kwh_line = utilityviewmodel.kwh_line.Replace(kWh, "").Trim();
                            utilityviewmodel.D_UNITS_USED_KWH = utilityviewmodel.kwh_line.Trim();
                            if (!SmartParseV2016.Generic_Parse_Integer(utilityviewmodel.D_UNITS_USED_KWH, utilityviewmodel))
                            {
                                return false;
                            }
                        }
                        fuck = utilityviewmodel.token.Split(SmartParametersV2016.newline);
                        utilityviewmodel.units_limit = fuck.Length;
                    }
                    else
                    {
                        utilityviewmodel.token = SmartParseV2016.Remove_Double_Spaces_V3(utilityviewmodel.token);
                        utilityviewmodel.token = utilityviewmodel.token.Replace(" at ", SmartParametersV2016.space);
                        utilityviewmodel.token = utilityviewmodel.token.Replace(SmartParametersV2016.space, SmartParametersV2016.bar.ToString());

                        string[] components = utilityviewmodel.token.Split(SmartParametersV2016.bar);
                        if (!SectionH1_At(utilityviewmodel,
                                            components))
                        {
                            return false;
                        }
                        utilityviewmodel.units_limit = 1;
                    }

                    // Do we have enough for a Read or Units_cost?
                    if (SmartParseV2016.Add_Readings_Test(SmartParametersV2016.defaultDate,
                                                            Convert.ToDateTime(utilityviewmodel.READINGS_PERIOD_START),
                                                            Convert.ToDateTime(utilityviewmodel.READINGS_PERIOD_END),
                                                            utilityviewmodel.D_LAST_READ,
                                                            utilityviewmodel.D_THIS_READ,
                                                            utilityviewmodel.UNIT_OF_MEASURE))
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
                                                                utilityviewmodel.METER_SERIAL_NO,
                                                                utilityviewmodel.THIS_READ_TYPE,
                                                                utilityviewmodel.D_LAST_READ,
                                                                utilityviewmodel.D_THIS_READ,
                                                                utilityviewmodel.D_UNITS_USED_KWH,
                                                                utilityviewmodel.N_LAST_READ,
                                                                utilityviewmodel.N_THIS_READ,
                                                                utilityviewmodel.N_UNITS_USED_KWH,
                                                                utilityviewmodel.UNIT_OF_MEASURE);
                        if (utilityviewmodel.BILL_PERIOD_START == SmartParametersV2016.defaultDates)
                        {
                            utilityviewmodel.BILL_PERIOD_START = utilityviewmodel.READINGS_PERIOD_START;
                            SmartParseV2016.Update_Bill(ourviewmodel, utilityviewmodel, "BILL_PERIOD_START", utilityviewmodel.READINGS_PERIOD_START);
                        }
                        if (Convert.ToDateTime(utilityviewmodel.READINGS_PERIOD_END) >= SmartTimeV2016.ConvertDateTime(utilityviewmodel.BILL_PERIOD_END))
                        {
                            utilityviewmodel.BILL_PERIOD_END = utilityviewmodel.READINGS_PERIOD_END;
                            SmartParseV2016.Update_Bill(ourviewmodel, utilityviewmodel, "BILL_PERIOD_END", utilityviewmodel.READINGS_PERIOD_END);

                            if (utilityviewmodel.READINGS_PERIOD_END != SmartParametersV2016.defaultDates)
                            {
                                DateTime temp_date = Convert.ToDateTime(utilityviewmodel.READINGS_PERIOD_END).AddMonths(1);
                                SmartParseV2016.Update_Bill(ourviewmodel, utilityviewmodel, "PAYMENT_DUE_DATE", temp_date.ToString(SmartParametersV2016.defaultCulture));
                                // Best we can do ..
                                SmartParseV2016.Update_Bill(ourviewmodel, utilityviewmodel, "DIRECT_DEBIT_DATE", temp_date.ToString(SmartParametersV2016.defaultCulture));
                                // Best we can do
                                SmartParseV2016.Update_Bill(ourviewmodel, utilityviewmodel, "DISCOUNT_CREDIT_DATE", temp_date.ToString(SmartParametersV2016.defaultCulture));
                            }
                        }

                        if (!SectionH1_Units_Charges(utilityviewmodel,
                                            utilityviewmodel.units_limit,
                                            utilityviewmodel.two_tier_meter,
                                            fuck,
                                            utilityviewmodel.D_UNITS_USED_KWH,
                                            utilityviewmodel.N_UNITS_USED_KWH,
                                            UNITS_TIME,
                                            utilityviewmodel.UNIT_OF_MEASURE))
                        {
                            return false;
                        }

                        // Clear these down ready for the next '3's
                        utilityviewmodel.READINGS_PERIOD_START = SmartParametersV2016.defaultDates;
                        utilityviewmodel.READINGS_PERIOD_END = SmartParametersV2016.defaultDates;
                        utilityviewmodel.LAST_READ_TYPE = "";
                        utilityviewmodel.THIS_READ_TYPE = "";
                        utilityviewmodel.D_LAST_READ = "";
                        utilityviewmodel.D_THIS_READ = "";
                        utilityviewmodel.UNIT_OF_MEASURE = "";
                        //total_mantissa = 0;
                        utilityviewmodel.units_band = 0;

                        utilityviewmodel.first_amount = "";
                        utilityviewmodel.next_amount = "";
                        utilityviewmodel.first_line = "";
                        utilityviewmodel.next_line = "";
                        utilityviewmodel.kwh_line = "";
                        if (utilityviewmodel.two_tier_meter)
                        {
                            utilityviewmodel.two_tier_meter = Suck_Out_Two_Tier(utilityviewmodel,
                                                                                ref line_count,
                                                                                sections,
                                                                                lines,
                                                                                section_index);
                            if (lines[sub_line_count].IndexOf(kWh) >= 0)
                            {
                                utilityviewmodel.kwh_line = lines[sub_line_count].Trim();
                            }
                        }
                    }
                }

                if (utilityviewmodel.token.IndexOf(SmartParametersV2016.billDateDelimiter) >= 0)
                {
                    // We have found a date Lets check it
                    if (!SectionH1_Bill_Date(utilityviewmodel))
                    {
                        return false;
                    }
                }
                else
                {
                    // Is it a number or text?
                    if (!SectionH1_Number_Or_Text(utilityviewmodel))
                    {
                        return false;
                    }
                }
                sub_line_count++;
            }

            utilityviewmodel.token = lines[sub_line_count];
            if (utilityviewmodel.token.IndexOf(Standing_Charge) >= 0)
            {
                if (!SectionH1_Standing_Charges(utilityviewmodel,
                                                Standing_Charge))
                {
                    return false;
                }
            }
            return true;
        }

        private static bool SectionH1_At(UtilityViewModel utilityviewmodel,
                                        string[] components)
        {
            int component_count = 0;

            while (component_count < components.Length)
            {
                switch (component_count)
                {
                    case 0:
                        // This line Units
                        utilityviewmodel.D_UNITS_USED_KWH = components[component_count].Trim();
                        // In case they ever use 76.0
                        if (!SmartParseV2016.Generic_Parse_Decimal(utilityviewmodel.D_UNITS_USED_KWH, utilityviewmodel))
                        {
                            return false;
                        }
                        break;
                    case 1:
                        // kWh
                        utilityviewmodel.UNIT_OF_MEASURE = components[component_count].Trim();
                        break;
                    case 2:
                        // Total kWh - don't think I ever use this
                        utilityviewmodel.TOTAL_UNITS_USED = components[component_count].Trim();
                        if (!SmartParseV2016.Generic_Parse_Integer(utilityviewmodel.TOTAL_UNITS_USED, utilityviewmodel))
                        {
                            return false;
                        }
                        break;
                    case 3:
                        // Units Rate
                        utilityviewmodel.UNITS_RATE = components[component_count].Replace(utilityviewmodel.bill_denomination_symbol.ToString(), "").Trim();
                        if (!SmartParseV2016.Generic_Parse_Decimal(utilityviewmodel.UNITS_RATE, utilityviewmodel))
                        {
                            return false;
                        }
                        break;
                    case 4:
                        // Total value UNITS_COST
                        break;
                    default:
                        break;
                }
                component_count++;
            }
            return true;
        }

        private static void SectionH1_Account_No(MainViewModel ourviewmodel,
                                                UtilityViewModel utilityviewmodel)
        {
            if (string.IsNullOrEmpty(utilityviewmodel.Hezbollah.bills_resource_row.RESOURCE_ACCOUNT_NO))
            {
                utilityviewmodel.Hezbollah.bills_resource_row.RESOURCE_ACCOUNT_NO = utilityviewmodel.token.Trim();
            }
            if (string.IsNullOrEmpty(utilityviewmodel.Hezbollah.bills_resource_row.RESOURCE_ACCOUNT_NO))
            {
                utilityviewmodel.Hezbollah.bills_resource_row.RESOURCE_ACCOUNT_NO = utilityviewmodel.ACCOUNT_NO;
                SmartParseV2016.Update_Bill(ourviewmodel, utilityviewmodel, "RESOURCE_ACCOUNT_NO", utilityviewmodel.ACCOUNT_NO);
            }
            return;
        }

        private static async Task<bool> SectionH1_Tariff_Payment_Plan(MainViewModel ourviewmodel,
                                                            UtilityViewModel utilityviewmodel,
                                                            string[] components)
        {
            string tariff_name = "Tariff Name:",
                    payment_plan = "Payment Plan:",
                    meter_number = "Meter Number:";

            foreach (string component in components)
            {
                utilityviewmodel.token = component.Trim();
                if (!string.IsNullOrEmpty(utilityviewmodel.token))
                {
                    if (utilityviewmodel.token.IndexOf(tariff_name) >= 0)
                    {
                        utilityviewmodel.TARIFF_NAME = utilityviewmodel.token.Replace(tariff_name, "");
                        // Npower 'special' gets rid of trailing 'Electricity' or 'Gas'
                        SmartParseV2016.Tariff_Ends_With_Resource(utilityviewmodel.resource_code, utilityviewmodel);
                        SmartParseV2016.Expand_Date(utilityviewmodel);

                        if (await SmartUtilityV2022.Lookup_Remote_Tariff_Code(ourviewmodel, utilityviewmodel))
                        {
                            SmartParseV2016.Update_Bill(ourviewmodel, utilityviewmodel, "TARIFF_CODE", utilityviewmodel.TARIFF_CODE.ToString());
                        }
                        else
                        {
                            // ourviewmodel.errorMessage should be set here
                            return false;
                        }
                    }
                    if (utilityviewmodel.token.IndexOf(payment_plan) >= 0)
                    {
                        string PAYMENT_TYPE = utilityviewmodel.token.Replace(payment_plan, "").Trim();
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
                    }
                    if (utilityviewmodel.token.IndexOf(meter_number) >= 0)
                    {
                        string METER_SERIAL_NO = utilityviewmodel.token.Replace(meter_number, "").Trim();
                        METER_SERIAL_NO = METER_SERIAL_NO.Substring(0, METER_SERIAL_NO.Length <= SmartParametersV2016.METERSERIALLENGTH ? METER_SERIAL_NO.Length : SmartParametersV2016.METERSERIALLENGTH); //Enforce 14 char max
                        utilityviewmodel.meter_serial_no = METER_SERIAL_NO;
                    }
                }
            }
            return true;
        }

        private static bool SectionH1_Check_Two_Tier(bool two_tier_meter,
                                                    string first_line,
                                                    string next_line,
                                                    string first_amount,
                                                    string next_amount,
                                                    string kwh_line)
        {
            if (two_tier_meter &&
                !string.IsNullOrEmpty(first_line) &&
                !string.IsNullOrEmpty(next_line) &&
                !string.IsNullOrEmpty(first_amount) &&
                !string.IsNullOrEmpty(next_amount) &&
                !string.IsNullOrEmpty(kwh_line))
            {
                return true;
            }
            return false;
        }

        private static bool SectionH1_Units_Charges(UtilityViewModel utilityviewmodel,
                                                    int units_limit,
                                                    bool two_tier_meter,
                                                    string[] fuck,
                                                    string D_UNITS_USED_KWH,
                                                    string N_UNITS_USED_KWH,
                                                    char UNITS_TIME,
                                                    string UNIT_OF_MEASURE)
        {
            for (int units_line = 0; units_line < units_limit; units_line++)
            {
                if (two_tier_meter)
                {
                    string[] components = fuck[units_line].Split(SmartParametersV2016.bar);
                    int component_count = 0;
                    while (component_count < components.Length)
                    {
                        switch (component_count)
                        {
                            case 0:
                                utilityviewmodel.UNITS_TYPE = components[component_count].Trim();
                                break;
                            case 1:
                                // This line Units
                                utilityviewmodel.UNITS = components[component_count].Trim();
                                // Even though its an INTEGER we use decimal in case they ever do 76.0
                                if (!SmartParseV2016.Generic_Parse_Decimal(utilityviewmodel.UNITS, utilityviewmodel))
                                {
                                    return false;
                                }
                                utilityviewmodel.UNITS_TYPE = utilityviewmodel.UNITS_TYPE + SmartParametersV2016.space + components[component_count];
                                break;
                            case 2:
                                // Units Cost
                                utilityviewmodel.UNITS_RATE = components[component_count].Replace(utilityviewmodel.bill_denomination_symbol.ToString(), "").Trim();
                                if (!SmartParseV2016.Generic_Parse_Decimal(utilityviewmodel.UNITS_RATE, utilityviewmodel))
                                {
                                    return false;
                                }
                                utilityviewmodel.UNITS_TYPE = utilityviewmodel.UNITS_TYPE +
                                                UNIT_OF_MEASURE + SmartParametersV2016.space +
                                                "at" + SmartParametersV2016.space +
                                                components[component_count];
                                break;
                            case 3:
                                // Total value UNITS_COST
                                // Units Cost
                                utilityviewmodel.UNITS_COST = components[component_count].Replace(utilityviewmodel.bill_currency_symbol, "");
                                utilityviewmodel.UNITS_COST = utilityviewmodel.UNITS_COST.Replace(utilityviewmodel.bill_currency_separator.ToString(), "").Trim();
                                utilityviewmodel.UNITS_COST = utilityviewmodel.UNITS_COST.Replace(utilityviewmodel.bill_thousands_separator.ToString(), "").Trim();
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
                }
                else
                {
                    // For the Charges when they happen
                    if (utilityviewmodel.STANDING_CHARGES_PERIOD_START == SmartParametersV2016.defaultDates)
                    {
                        utilityviewmodel.STANDING_CHARGES_PERIOD_START = utilityviewmodel.READINGS_PERIOD_START;
                    }
                    utilityviewmodel.STANDING_CHARGES_PERIOD_END = utilityviewmodel.READINGS_PERIOD_END;

                    decimal temp_units = (SmartRoutinesV2018.ConvertDecimal(D_UNITS_USED_KWH)) +
                                    SmartRoutinesV2018.ConvertDecimal(N_UNITS_USED_KWH);
                    utilityviewmodel.UNITS = temp_units.ToString();
                    decimal temp_units_rate = (SmartRoutinesV2018.ConvertDecimal(utilityviewmodel.UNITS_RATE));
                    utilityviewmodel.UNITS_TYPE = utilityviewmodel.UNITS + SmartParametersV2016.space +
                                    utilityviewmodel.UNIT_OF_MEASURE + SmartParametersV2016.space +
                                    "at" + SmartParametersV2016.space +
                                    utilityviewmodel.UNITS_RATE + utilityviewmodel.bill_denomination_symbol.ToString();
                    decimal units_cost = temp_units * temp_units_rate;
                    string units = units_cost.ToString();
                    int units_cost_int = 0;
                    string[] characteristic = units.Split(Convert.ToChar(SmartParametersV2016.decimalPoint));
                    if (characteristic.Length > 1)
                    {
                        units_cost_int = Convert.ToInt32(characteristic[0]);
                        int this_mantissa = Convert.ToInt32(characteristic[1]);
                        utilityviewmodel.total_mantissa += this_mantissa;
                        if (utilityviewmodel.total_mantissa >= 500)
                        {
                            units_cost_int++;
                            utilityviewmodel.total_mantissa -= 500;
                        }
                    }
                    utilityviewmodel.UNITS_COST = units_cost_int.ToString();
                }

                utilityviewmodel.UNIT_CHARGES_PERIOD_START = utilityviewmodel.READINGS_PERIOD_START;
                utilityviewmodel.UNIT_CHARGES_PERIOD_END = utilityviewmodel.READINGS_PERIOD_END;

                utilityviewmodel.units_band = (short)(utilityviewmodel.units_band + 1);
                SmartUtilityV2022.E_Unit_ChargesList_Add(utilityviewmodel,
                                                            SmartParametersV2016.Utility,
                                                            utilityviewmodel.supplier_code,
                                                            utilityviewmodel.brand_code,
                                                            utilityviewmodel.ACCOUNT_NO,
                                                            utilityviewmodel.CREATED,
                                                            utilityviewmodel.STATEMENT_ID,
                                                            utilityviewmodel.BILL_DATE,
                                                            utilityviewmodel.sparks.MPAN,
                                                            utilityviewmodel.UNIT_CHARGES_PERIOD_START,
                                                            utilityviewmodel.UNIT_CHARGES_PERIOD_END,
                                                            UNITS_TIME,
                                                            utilityviewmodel.units_band.ToString(),
                                                            utilityviewmodel.UNITS_TYPE,
                                                            utilityviewmodel.UNITS,
                                                            utilityviewmodel.UNITS_RATE,
                                                            utilityviewmodel.UNIT_OF_MEASURE,
                                                            utilityviewmodel.UNITS_COST);
            }
            return true;
        }

        private static bool SectionH1_Bill_Date(UtilityViewModel utilityviewmodel)
        {
            utilityviewmodel.token = utilityviewmodel.token.Replace("PC", "");
            utilityviewmodel.token = utilityviewmodel.token.Replace("*", "").Trim();

            // We have found a date Lets check it
            if (!SmartParseV2016.Generic_Parse_Datetime(utilityviewmodel.token, utilityviewmodel))
            {
                return false;
            }
            if (utilityviewmodel.READINGS_PERIOD_START == SmartParametersV2016.defaultDates)
            {
                utilityviewmodel.READINGS_PERIOD_START = utilityviewmodel.token;
                // Keep going until we find another
            }
            else
            {
                if (utilityviewmodel.READINGS_PERIOD_END == SmartParametersV2016.defaultDates)
                {
                    utilityviewmodel.READINGS_PERIOD_END = utilityviewmodel.token;
                }
            }
            return true;
        }

        private static bool SectionH1_Number_Or_Text(UtilityViewModel utilityviewmodel)
        {
            // Is it a number or text?
            int temp;
            if (SmartParseV2016.Generic_Parse_Digits(utilityviewmodel.token, utilityviewmodel))
            {
                // Its a number
                if (!SmartParseV2016.Generic_Parse_Integer(utilityviewmodel.token, utilityviewmodel))
                {
                    return false;
                }
                else
                {
                    temp = utilityviewmodel.genericTransactionValue;
                }
                if (utilityviewmodel.D_LAST_READ == "")
                {
                    utilityviewmodel.D_LAST_READ = temp.ToString();
                }
                else
                {
                    if (utilityviewmodel.D_THIS_READ == "")
                    {
                        utilityviewmodel.D_THIS_READ = temp.ToString();
                    }
                }
            }
            else
            {
                // Its text
                if (string.IsNullOrEmpty(utilityviewmodel.LAST_READ_TYPE))
                {
                    utilityviewmodel.LAST_READ_TYPE = utilityviewmodel.token;
                }
                else
                {
                    if (string.IsNullOrEmpty(utilityviewmodel.THIS_READ_TYPE))
                    {
                        utilityviewmodel.THIS_READ_TYPE = utilityviewmodel.token;
                    }
                }
            }
            return true;
        }
        private static bool SectionH1_Standing_Charges(UtilityViewModel utilityviewmodel,
                                                        string Standing_Charge)
        {
            string CHARGES_TYPE,// = "",
                    CHARGES_DAYS = "0",
                    STANDING_CHARGE = "0",
                    CHARGES_COST = "0";
            //int temp = 0;
            CHARGES_TYPE = utilityviewmodel.token;
            utilityviewmodel.token = utilityviewmodel.token.Replace(Standing_Charge, "").Trim();
            utilityviewmodel.token = utilityviewmodel.token.Replace("at", "");
            utilityviewmodel.token = utilityviewmodel.token.Replace("per day", "");
            utilityviewmodel.token = SmartParseV2016.Remove_Double_Spaces_V3(utilityviewmodel.token);
            string[] components = utilityviewmodel.token.Split(SmartParametersV2016.spaceSplit);
            int component_count = 0;

            while (component_count < components.Length)
            {
                switch (component_count)
                {
                    case 0:
                        if (!SmartParseV2016.Generic_Parse_Integer(components[component_count], utilityviewmodel))
                        {
                            return false;
                        }
                        CHARGES_DAYS = components[component_count];
                        break;
                    case 1:
                        // days;
                        break;
                    case 2:
                        //decimal temp_decimal = 0.0M;
                        STANDING_CHARGE = components[component_count].Replace(utilityviewmodel.bill_denomination_symbol.ToString(), "");
                        if (!SmartParseV2016.Generic_Parse_Decimal(STANDING_CHARGE, utilityviewmodel))
                        {
                            return false;
                        }
                        break;
                    case 3:
                        CHARGES_COST = components[component_count].Replace(utilityviewmodel.bill_currency_symbol, "");
                        CHARGES_COST = CHARGES_COST.Replace(utilityviewmodel.bill_currency_separator.ToString(), "");
                        CHARGES_COST = CHARGES_COST.Replace(utilityviewmodel.bill_thousands_separator.ToString(), "");
                        if (!SmartParseV2016.Generic_Parse_Integer(CHARGES_COST, utilityviewmodel))
                        {
                            return false;
                        }
                        break;
                    default:
                        break;
                }
                component_count++;
            }

            utilityviewmodel.charges_item = (short)(utilityviewmodel.charges_item + 1);
            SmartUtilityV2022.E_Standing_ChargesList_Add(utilityviewmodel,
                                                            SmartParametersV2016.Utility,
                                                            utilityviewmodel.supplier_code,
                                                            utilityviewmodel.brand_code,
                                                            utilityviewmodel.ACCOUNT_NO,
                                                            utilityviewmodel.CREATED,
                                                            utilityviewmodel.STATEMENT_ID,
                                                            utilityviewmodel.BILL_DATE,
                                                            utilityviewmodel.sparks.MPAN,
                                                            utilityviewmodel.STANDING_CHARGES_PERIOD_START, // Not a mistake
                                                            utilityviewmodel.STANDING_CHARGES_PERIOD_END,     // Not a mistake
                                                            utilityviewmodel.charges_item.ToString(),
                                                            CHARGES_TYPE,
                                                            STANDING_CHARGE,
                                                            CHARGES_DAYS,
                                                            CHARGES_COST);
            utilityviewmodel.STANDING_CHARGES_PERIOD_START = SmartParametersV2016.defaultDates;
            utilityviewmodel.STANDING_CHARGES_PERIOD_END = SmartParametersV2016.defaultDates;
            return true;
        }

        private static void SectionH1_2012_One_Off(UtilityViewModel utilityviewmodel)
        //string[][] sections,
        //int section_index,
        //string[] lines,
        //rf int line_count)
        {
            if (utilityviewmodel.BILL_DATE.IndexOf("2012") >= 0)
            {
                //if (SmartParseV2016.fix_supply_number_special(line_count,
                //                            lines,
                //                            sections[section_index][5],
                //                            utilityviewmodel))//,
                //                                     //true))  // Look backwards if not found by looking forwards
                //{
                //    if (utilityviewmodel.mpan.Length >= 22)
                //    {
                //        SmartUtilityV2022.Amelia_Update_Mpan(utilityviewmodel, utilityviewmodel.mpan);
                //    }
                //}
            }
            else
            {
                //if (SmartParseV2016.fix_supply_number(line_count,
                //                            lines,
                //                            sections[section_index][5],
                //                            sections[section_index][4],
                //                            utilityviewmodel,
                //                            true))  // Look backwards if not found by looking forwards
                //{
                //    if (utilityviewmodel.mpan.Length >= 22)
                //    {
                //        SmartUtilityV2022.Amelia_Update_Mpan(utilityviewmodel, utilityviewmodel.mpan);
                //    }
                //}
            }
            return;
        }

        private static bool SectionH1_Direct_Debit_Discount(MainViewModel ourviewmodel,
                                                UtilityViewModel utilityviewmodel,
                                                string direct_debit_discount,
                                                short DISCOUNT_VAT_CODE)
        {
            string DISCOUNT_TYPE = direct_debit_discount;
            //int temp = 0;
            string DISCOUNT_AMOUNT;// = "0";
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
            // Get some kind of discount date
            DateTime DISCOUNT_DATE = SmartNibbyV2016.ConvertDate(utilityviewmodel.BILL_PERIOD_END,
                SmartParametersV2016.defaultDate, ourviewmodel);
            if (ourviewmodel.errorMessage != "")
            {
                return false;
            }
            utilityviewmodel.discount_item = (short)(utilityviewmodel.discount_item + 1);     // <= Can put this in MES if needs be to carry out through
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
            }
            return true;
        }

        private static bool SectionH1_Dual_Fuel_Discounts(MainViewModel ourviewmodel,
                                                UtilityViewModel utilityviewmodel,
                                                string dual_fuel_discount,
                                                short DISCOUNT_VAT_CODE)
        {
            string DISCOUNT_TYPE = dual_fuel_discount;
            utilityviewmodel.DISCOUNT_AMOUNT = "";
            if (!SmartParseV2016.Currency_Strip(utilityviewmodel.bill_currency_symbol, utilityviewmodel.bill_currency_separator, utilityviewmodel.bill_thousands_separator, utilityviewmodel.token, utilityviewmodel))
            {
                return false;
            }
            else
            {
                utilityviewmodel.DISCOUNT_AMOUNT = utilityviewmodel.value;
            }
            if (!SmartParseV2016.Generic_Parse_Integer(utilityviewmodel.DISCOUNT_AMOUNT, utilityviewmodel))
            {
                return false;
            }
            // Get some kind of discount date
            DateTime DISCOUNT_DATE = SmartNibbyV2016.ConvertDate(utilityviewmodel.BILL_PERIOD_END,
                SmartParametersV2016.defaultDate, ourviewmodel);
            if (ourviewmodel.errorMessage != "")
            {
                return false;
            }
            utilityviewmodel.discount_item = (short)(utilityviewmodel.discount_item + 1);     // <= Can put this in MES if needs be to carry ot through
            if (!SmartParseV2016.Check_Discount_Is_There(utilityviewmodel,
                                                        DISCOUNT_DATE,
                                                        utilityviewmodel.discount_item,
                                                        DISCOUNT_TYPE,
                                                        DISCOUNT_VAT_CODE,
                                                        utilityviewmodel.DISCOUNT_AMOUNT))
            {
                SmartParseV2016.No_Wonder_I_Cant_Fucking_Work(utilityviewmodel,
                                                                DISCOUNT_DATE,
                                                                utilityviewmodel.discount_item,
                                                                DISCOUNT_TYPE,
                                                                DISCOUNT_VAT_CODE,
                                                                utilityviewmodel.DISCOUNT_AMOUNT);
            }
            return true;
        }

        private static bool SectionH1_VAT_At(MainViewModel ourviewmodel,
                                            UtilityViewModel utilityviewmodel,
                                            string[] components)
        {
            int components_count = 0;
            while (components_count < components.Length)
            {
                switch (components_count)
                {
                    case 2:
                        //int temp = 0;
                        string VAT_AMOUNT;// = "0";
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
                        SmartParseV2016.Update_Bill(ourviewmodel, utilityviewmodel, "RESOURCE_VAT_AMOUNT", VAT_AMOUNT);
                        break;
                    default:
                        break;
                }
                components_count++;
            }
            return true;
        }
        private static bool Suck_Out_Two_Tier(UtilityViewModel utilityviewmodel,
                                                ref int line_count,
                                                string[][] sections,
                                                string[] lines,
                                                int section_index)
        {
            utilityviewmodel.token = "";

            string first = "first",
                    next = "next";
            int inner_line_count = line_count;
            //char delimiter = Convert.ToChar(SmartParametersV2016.billDateDelimiter);
            while (inner_line_count < Convert.ToInt32(sections[section_index][5]))
            {
                utilityviewmodel.token = lines[inner_line_count].Trim();
                int delimeter = utilityviewmodel.token.IndexOf(SmartParametersV2016.billDateDelimiter);
                if (delimeter >= 0)
                {
                    if (delimeter + 1 <= utilityviewmodel.token.Length)
                    {
                        delimeter = utilityviewmodel.token.IndexOf(SmartParametersV2016.billDateDelimiter, delimeter + 1);
                        if (delimeter >= 0)
                        {
                            // Set this BACK so we re-read the date delimiter line
                            line_count = inner_line_count - 1;
                            break;
                        }
                    }
                }
                if (utilityviewmodel.token.IndexOf(utilityviewmodel.bill_currency_symbol) >= 0)
                {
                    if (string.IsNullOrEmpty(utilityviewmodel.first_amount))
                    {
                        utilityviewmodel.first_amount = utilityviewmodel.token;
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(utilityviewmodel.next_amount))
                        {
                            utilityviewmodel.next_amount = utilityviewmodel.token;
                        }
                    }
                }
                else
                {
                    if (utilityviewmodel.token.IndexOf(first) >= 0)
                    {
                        utilityviewmodel.first_line = utilityviewmodel.token;
                    }
                    else
                    {
                        if (utilityviewmodel.token.IndexOf(next) >= 0)
                        {
                            utilityviewmodel.next_line = utilityviewmodel.token;
                        }
                    }
                }
                inner_line_count++;
            }
            if (!string.IsNullOrEmpty(utilityviewmodel.first_amount) &&
                !string.IsNullOrEmpty(utilityviewmodel.next_amount) &&
                !string.IsNullOrEmpty(utilityviewmodel.first_line) &&
                !string.IsNullOrEmpty(utilityviewmodel.next_line))
            {
                return true;
            }
            return false;
        }
        private static async Task<bool> Parse_SectionH2(MainViewModel ourviewmodel,
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
                // We are here because we don't want (or need) to do this section
                return true;    // Always - because we haven't had a CONVERSION error
            }

            utilityviewmodel.METER_SERIAL_NO = "";
            utilityviewmodel.token = "";

            utilityviewmodel.READINGS_PERIOD_START = SmartParametersV2016.defaultDates;
            utilityviewmodel.READINGS_PERIOD_END = SmartParametersV2016.defaultDates;
            //        UNIT_CHARGES_PERIOD_START = SmartParametersV2016.defaultDate,
            //        UNIT_CHARGES_PERIOD_END = SmartParametersV2016.defaultDate,
            //        STANDING_CHARGES_PERIOD_START = SmartParametersV2016.defaultDate,
            //        STANDING_CHARGES_PERIOD_END = SmartParametersV2016.defaultDate;

            short DISCOUNT_VAT_CODE = SmartParametersV2016.zeroRateVatCode;
            utilityviewmodel.units_band = 0;
            utilityviewmodel.charges_item = 0;

            string gas_summary = "Gas summary",
                    gas_account = "Gas account",
                    tariff = "Tariff",
                    charges_for_tariff = "Charges for Tariff",
                    meter = "Meter:",
                    VAT_at = "VAT at",
                    cost_of_gas_used_this_period = "Cost of gas used this period";

            string tariff_name = "Tariff Name:",
                    payment_plan = "Payment Plan:",
                    meter_number = "Meter Number:",
                    direct_debit_discount = "Direct Debit discount";

            utilityviewmodel.total_mantissa = 0;
            //string M_Number = "'M' Number";
            bool two_tier_meter = false;
            utilityviewmodel.first_amount = "";
            utilityviewmodel.next_amount = "";
            utilityviewmodel.first_line = "";
            utilityviewmodel.next_line = "";
            utilityviewmodel.units_limit = 0;
            string[] fuck = new string[2] { "", "" };

            for (int line_count = Convert.ToInt32(sections[utilityviewmodel.currentIndex][4]); line_count <= Convert.ToInt32(sections[utilityviewmodel.currentIndex][5]); line_count++)
            {
                utilityviewmodel.token = lines[line_count].Trim();

                if (SmartParseV2016.Token_Identify(utilityviewmodel, gas_account +
                                                                SmartParametersV2016.bar +
                                                                gas_summary, true))
                {
                    SectionH2_Account_No(ourviewmodel,
                                        utilityviewmodel);
                    goto the_old_daysH2;
                }

                if (SmartParseV2016.Token_Identify(utilityviewmodel, tariff +
                                                                SmartParametersV2016.bar +
                                                                charges_for_tariff +
                                                                SmartParametersV2016.bar +
                                                                meter, false))
                {
                    utilityviewmodel.token = SmartParseV2016.Remove_Double_Spaces_V3(utilityviewmodel.token);
                    utilityviewmodel.token = utilityviewmodel.token.Replace("/", SmartParametersV2016.bar + payment_plan);
                    utilityviewmodel.token = utilityviewmodel.token.Replace("Charges for Tariff - ", tariff_name);
                    utilityviewmodel.token = utilityviewmodel.token.Replace(tariff + " - ", tariff_name);
                    if (utilityviewmodel.token.IndexOf(meter) >= 0)
                    {
                        two_tier_meter = true;
                    }
                    utilityviewmodel.token = utilityviewmodel.token.Replace(meter, meter_number);
                    utilityviewmodel.token = utilityviewmodel.token.Replace(tariff_name, SmartParametersV2016.bar + tariff_name);
                    string[] components = utilityviewmodel.token.Trim().Split(SmartParametersV2016.bar);
                    // Sometimes this doesn't find/fetch the Meter Serial No so we have to do that later on ...

                    if (!await SectionH2_Tariff_Payment_Plan(ourviewmodel,
                                                            utilityviewmodel,
                                                            components))
                    {
                        return false;
                    }

                    // Old two-tier Bills
                    if (two_tier_meter)
                    {
                        two_tier_meter = Suck_Out_Two_Tier(utilityviewmodel,
                                                ref line_count,
                                                sections,
                                                lines,
                                                utilityviewmodel.currentIndex);
                    }
                    goto the_old_daysH2;
                    // The fucking Archers is on A FUCKING GAIN   ... Brian!!!!
                }

                if (SmartParseV2016.Token_Identify(utilityviewmodel, meter_number, true))
                {
                    utilityviewmodel.METER_SERIAL_NO = utilityviewmodel.token.Replace(meter_number, "").Trim();
                    utilityviewmodel.METER_SERIAL_NO = utilityviewmodel.METER_SERIAL_NO.Substring(0, utilityviewmodel.METER_SERIAL_NO.Length <= SmartParametersV2016.METERSERIALLENGTH ? utilityviewmodel.METER_SERIAL_NO.Length : SmartParametersV2016.METERSERIALLENGTH); //Enforce 14 char max
                    goto the_old_daysH2;
                }

                if (utilityviewmodel.token.IndexOf(SmartParametersV2016.billDateDelimiter) >= 0)
                {
                    int sub_line_count = line_count;

                    utilityviewmodel.READINGS_PERIOD_START = utilityviewmodel.READINGS_PERIOD_END = SmartParametersV2016.defaultDates;

                    if (!SectionH2_Big_Loop(ourviewmodel,
                                            utilityviewmodel,
                                            line_count,
                                            sections,
                                            utilityviewmodel.currentIndex,
                                            lines,
                                            fuck,
                                            ref sub_line_count))
                    {
                        return false;
                    }
                    line_count = sub_line_count;
                }

                // Stupid cow cant tell the difference between BROWN and BLUE ....

                //if (utilityviewmodel.token == M_Number)
                //{
                //    if (SmartParseV2016.fix_mprn(line_count, lines, sections[section_index][5], utilityviewmodel))
                //    {
                //        SmartUtilityV2022.Amelia_Update_Mpan(utilityviewmodel, utilityviewmodel.mprn);
                //    }
                //    goto the_old_daysH2;
                //}

                if (SmartParseV2016.Token_Identify(utilityviewmodel, cost_of_gas_used_this_period, true))
                {
                    utilityviewmodel.token = utilityviewmodel.token.Replace(utilityviewmodel.bill_currency_symbol, "").Trim();
                    utilityviewmodel.token = utilityviewmodel.token.Replace(utilityviewmodel.bill_currency_separator.ToString(), "");
                    utilityviewmodel.token = utilityviewmodel.token.Replace(utilityviewmodel.bill_thousands_separator.ToString(), "");
                    SmartParseV2016.Update_Bill(ourviewmodel, utilityviewmodel, "RESOURCE_NEW_CHARGES", utilityviewmodel.token);
                    goto the_old_daysH2;
                }

                if (SmartParseV2016.Token_Identify(utilityviewmodel, direct_debit_discount, true))
                {
                    if (!SectionH2_Discounts(ourviewmodel,
                                                utilityviewmodel,
                                                DISCOUNT_VAT_CODE,
                                                direct_debit_discount))
                    {
                        return false;
                    }
                    goto the_old_daysH2;
                }

                // DUAL FUEL DISCOUNT ONLY APPLIES TO ELECTRICITY

                if (SmartParseV2016.Token_Identify(utilityviewmodel, VAT_at, true))
                {
                    short VAT_CODE = SmartParametersV2016.zeroRateVatCode;
                    if (!SmartParseV2016.Determine_Vat_Code(ourviewmodel,
                                                            utilityviewmodel,
                                                            utilityviewmodel.token,
                                                            VAT_at))
                    {
                        return false;
                    }
                    SmartParseV2016.Update_Bill(ourviewmodel, utilityviewmodel, "RESOURCE_VAT_CODE", VAT_CODE.ToString());
                    utilityviewmodel.token = utilityviewmodel.token.Replace("on", "");
                    utilityviewmodel.token = utilityviewmodel.token.Replace("+ " + utilityviewmodel.bill_currency_symbol, "+" + utilityviewmodel.bill_currency_symbol);
                    utilityviewmodel.token = SmartParseV2016.Remove_Double_Spaces_V3(utilityviewmodel.token);
                    string[] components = utilityviewmodel.token.Split(SmartParametersV2016.spaceSplit);
                    if (!SectionH2_Vat_Code(ourviewmodel, utilityviewmodel, components))
                    {
                        return false;
                    }
                    goto the_old_daysH2;
                }
            the_old_daysH2:
                continue;
            }
            return true;
        }

        private static void SectionH2_Account_No(MainViewModel ourviewmodel,
                                                    UtilityViewModel utilityviewmodel)
        {
            if (string.IsNullOrEmpty(utilityviewmodel.Hezbollah.bills_resource_row.RESOURCE_ACCOUNT_NO))
            {
                utilityviewmodel.Hezbollah.bills_resource_row.RESOURCE_ACCOUNT_NO = utilityviewmodel.token.Trim();
            }
            if (string.IsNullOrEmpty(utilityviewmodel.Hezbollah.bills_resource_row.RESOURCE_ACCOUNT_NO))
            {
                utilityviewmodel.Hezbollah.bills_resource_row.RESOURCE_ACCOUNT_NO = utilityviewmodel.ACCOUNT_NO;
                SmartParseV2016.Update_Bill(ourviewmodel, utilityviewmodel, "RESOURCE_ACCOUNT_NO", utilityviewmodel.ACCOUNT_NO);
            }
            return;
        }

        private static async Task<bool> SectionH2_Tariff_Payment_Plan(MainViewModel ourviewmodel,
                                                            UtilityViewModel utilityviewmodel,
                                                            string[] components)
        {
            string tariff_name = "Tariff Name:",
                    payment_plan = "Payment Plan:",
                    meter_number = "Meter Number:";

            foreach (string component in components)
            {
                utilityviewmodel.token = component.Trim();
                if (!string.IsNullOrEmpty(utilityviewmodel.token))
                {
                    if (utilityviewmodel.token.IndexOf(tariff_name) >= 0)
                    {
                        utilityviewmodel.TARIFF_NAME = utilityviewmodel.token.Replace(tariff_name, "");
                        // Npower 'special' gets rid of traling 'Electricity' or 'Gas'
                        SmartParseV2016.Tariff_Ends_With_Resource(utilityviewmodel.resource_code, utilityviewmodel);
                        SmartParseV2016.Expand_Date(utilityviewmodel);

                        if (await SmartUtilityV2022.Lookup_Remote_Tariff_Code(ourviewmodel, utilityviewmodel))

                        {
                            SmartParseV2016.Update_Bill(ourviewmodel, utilityviewmodel, "TARIFF_CODE", utilityviewmodel.TARIFF_CODE.ToString());
                        }
                        else
                        {
                            // ourviewmodel.errorMessage should be set here
                            return false;
                        }
                    }
                    if (utilityviewmodel.token.IndexOf(payment_plan) >= 0)
                    {
                        string PAYMENT_TYPE = utilityviewmodel.token.Replace(payment_plan, "").Trim();
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
                    }
                    if (utilityviewmodel.token.IndexOf(meter_number) >= 0)
                    {
                        string METER_SERIAL_NO = utilityviewmodel.token.Replace(meter_number, "").Trim();
                        METER_SERIAL_NO = METER_SERIAL_NO.Substring(0, METER_SERIAL_NO.Length <= SmartParametersV2016.METERSERIALLENGTH ? METER_SERIAL_NO.Length : SmartParametersV2016.METERSERIALLENGTH); //Enforce 14 char max
                        utilityviewmodel.meter_serial_no = METER_SERIAL_NO;
                    }
                }
            }
            return true;
        }

        private static bool SectionH2_Big_Loop(MainViewModel ourviewmodel,
                                                    UtilityViewModel utilityviewmodel,
                                                    int line_count,
                                                    string[][] sections,
                                                    int section_index,
                                                    string[] lines,
                                                    string[] fuck,
                                                    ref int sub_line_count)

        {
            string VAT_at = "VAT at", Standing_Charge = "Standing Charge",
                    kWh = "kWh",
                    Calorific_Value = "Calorific Value",
                    cubic_metres = "(cubic metres)";
            utilityviewmodel.D_UNITS_USED_KWH = "";
            utilityviewmodel.UNIT_OF_MEASURE = "";
            utilityviewmodel.THIS_READ_TYPE = "";
            utilityviewmodel.D_THIS_READ = "0";
            utilityviewmodel.LAST_READ_TYPE = "";
            utilityviewmodel.D_LAST_READ = "";
            utilityviewmodel.D_UNITS_USED_M3 = "";
            utilityviewmodel.TOTAL_UNITS_USED = "";
            utilityviewmodel.UNITS_TYPE = "";
            utilityviewmodel.UNITS_RATE = "";
            utilityviewmodel.UNITS = "";
            utilityviewmodel.UNITS_COST = "";

            utilityviewmodel.token = "";

            while (sub_line_count < Convert.ToInt32(sections[section_index][5]))
            {
                // The Readings come in 3s 'Last Reading and 'This Reading'
                // But sometimes you have the four two-tier lines preceding the dates!
                // Its because the Npower Foreigners haven't made their pdf properly because they
                // are lazy stupid fuckers ...

                utilityviewmodel.token = lines[sub_line_count];

                if ((utilityviewmodel.token.IndexOf(Standing_Charge) >= 0) ||
                    (utilityviewmodel.token.IndexOf(VAT_at) >= 0))
                {
                    break;
                }

                if (utilityviewmodel.token.IndexOf(Calorific_Value) >= 0)
                {
                    utilityviewmodel.token = utilityviewmodel.token.Replace("PC", "");
                    utilityviewmodel.token = utilityviewmodel.token.Replace("*", "").Trim();

                    utilityviewmodel.token = utilityviewmodel.token.Replace(Calorific_Value, "").Trim();
                    utilityviewmodel.CALORIFIC_VALUE = utilityviewmodel.token;
                    if (SectionH2_Check_Two_Tier(utilityviewmodel.two_tier_meter,
                                                        utilityviewmodel.first_line,
                                                        utilityviewmodel.next_line,
                                                        utilityviewmodel.first_amount,
                                                        utilityviewmodel.next_amount))
                    {
                        utilityviewmodel.first_line = utilityviewmodel.first_line.Replace(" at ", SmartParametersV2016.space);
                        utilityviewmodel.next_line = utilityviewmodel.next_line.Replace(" at ", SmartParametersV2016.space);
                        utilityviewmodel.token = utilityviewmodel.first_line + SmartParametersV2016.space +
                                    utilityviewmodel.first_amount +
                                    SmartParametersV2016.newline +
                                    utilityviewmodel.next_line + SmartParametersV2016.space +
                                    utilityviewmodel.next_amount;

                        utilityviewmodel.token = SmartParseV2016.Remove_Double_Spaces_V3(utilityviewmodel.token);
                        utilityviewmodel.token = utilityviewmodel.token.Replace(SmartParametersV2016.space, SmartParametersV2016.bar.ToString());

                        fuck = utilityviewmodel.token.Split(SmartParametersV2016.newline);
                        utilityviewmodel.units_limit = fuck.Length;
                    }
                    else
                    {
                        utilityviewmodel.units_limit = 1;
                    }
                    goto next_sub;
                }

                if (utilityviewmodel.token.IndexOf(cubic_metres) >= 0)
                {
                    utilityviewmodel.UNIT_OF_MEASURE = "m3";
                    utilityviewmodel.token = utilityviewmodel.token.Replace(cubic_metres, "");
                    utilityviewmodel.D_UNITS_USED_M3 = utilityviewmodel.token;
                    if (!SmartParseV2016.Generic_Parse_Integer(utilityviewmodel.D_UNITS_USED_M3, utilityviewmodel))
                    {
                        return false;
                    }
                    goto next_sub;
                }

                if (utilityviewmodel.token.IndexOf(kWh) >= 0)
                {
                    utilityviewmodel.token = utilityviewmodel.token.Replace(kWh, SmartParametersV2016.space).Trim();
                    utilityviewmodel.token = utilityviewmodel.token.Replace("=", SmartParametersV2016.space).Trim();
                    utilityviewmodel.D_UNITS_USED_KWH = utilityviewmodel.token;
                    if (!SmartParseV2016.Generic_Parse_Integer(utilityviewmodel.D_UNITS_USED_KWH, utilityviewmodel))
                    {
                        return false;
                    }
                    // Might not have a UNITS_RATE at this point ...
                    // Do we have enought for a Read or Units_cost?  (without the Rate)?
                    if (SmartParseV2016.Add_Readings_Test(SmartParametersV2016.defaultDate,
                                                            Convert.ToDateTime(utilityviewmodel.READINGS_PERIOD_START),
                                                            Convert.ToDateTime(utilityviewmodel.READINGS_PERIOD_END),
                                                            utilityviewmodel.D_LAST_READ,
                                                            utilityviewmodel.D_THIS_READ,
                                                            utilityviewmodel.UNIT_OF_MEASURE))
                    {
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
                                                                utilityviewmodel.THIS_READ_TYPE,
                                                                utilityviewmodel.D_LAST_READ,
                                                                utilityviewmodel.D_THIS_READ,
                                                                utilityviewmodel.D_UNITS_USED_M3,
                                                                utilityviewmodel.UNIT_OF_MEASURE,
                                                                utilityviewmodel.D_UNITS_USED_KWH,
                                                                utilityviewmodel.CALORIFIC_VALUE);
                        if (utilityviewmodel.BILL_PERIOD_START == SmartParametersV2016.defaultDates)
                        {
                            utilityviewmodel.BILL_PERIOD_START = utilityviewmodel.READINGS_PERIOD_START.ToString();
                            SmartParseV2016.Update_Bill(ourviewmodel, utilityviewmodel, "BILL_PERIOD_START", utilityviewmodel.READINGS_PERIOD_START);
                        }
                        if (Convert.ToDateTime(utilityviewmodel.READINGS_PERIOD_END) >= SmartTimeV2016.ConvertDateTime(utilityviewmodel.BILL_PERIOD_END))
                        {
                            utilityviewmodel.BILL_PERIOD_END = utilityviewmodel.READINGS_PERIOD_END;
                            SmartParseV2016.Update_Bill(ourviewmodel, utilityviewmodel, "BILL_PERIOD_END", utilityviewmodel.READINGS_PERIOD_END);

                            if (utilityviewmodel.READINGS_PERIOD_END != SmartParametersV2016.defaultDates)
                            {
                                DateTime temp_date = Convert.ToDateTime(utilityviewmodel.READINGS_PERIOD_END).AddMonths(1);
                                SmartParseV2016.Update_Bill(ourviewmodel, utilityviewmodel, "PAYMENT_DUE_DATE", temp_date.ToString(SmartParametersV2016.defaultCulture));
                                // Best we can do ..
                                SmartParseV2016.Update_Bill(ourviewmodel, utilityviewmodel, "DIRECT_DEBIT_DATE", temp_date.ToString(SmartParametersV2016.defaultCulture));
                                // Best we can do
                                SmartParseV2016.Update_Bill(ourviewmodel, utilityviewmodel, "DISCOUNT_CREDIT_DATE", temp_date.ToString(SmartParametersV2016.defaultCulture));
                            }
                        }

                        if (!SectionH2_Unit_Charges(utilityviewmodel,
                                                    fuck))
                        {
                            return false;
                        }

                        // Clear these down ready for the next '3's
                        utilityviewmodel.READINGS_PERIOD_START = SmartParametersV2016.defaultDates;
                        utilityviewmodel.READINGS_PERIOD_END = SmartParametersV2016.defaultDates;
                        utilityviewmodel.LAST_READ_TYPE = "";
                        utilityviewmodel.THIS_READ_TYPE = "";
                        utilityviewmodel.D_LAST_READ = "";
                        utilityviewmodel.D_THIS_READ = "0";
                        utilityviewmodel.UNIT_OF_MEASURE = "";
                        utilityviewmodel.total_mantissa = 0;
                        utilityviewmodel.units_band = 0;

                        utilityviewmodel.first_amount = "";
                        utilityviewmodel.next_amount = "";
                        utilityviewmodel.first_line = "";
                        utilityviewmodel.next_line = "";
                        if (utilityviewmodel.two_tier_meter)
                        {
                            utilityviewmodel.two_tier_meter = Suck_Out_Two_Tier(utilityviewmodel,
                                            ref line_count,
                                            sections,
                                            lines,
                                            section_index);
                        }
                        goto next_sub;
                    }
                }

                if (utilityviewmodel.token.IndexOf(" at ") >= 0)
                {
                    utilityviewmodel.token = utilityviewmodel.token.Replace("at", SmartParametersV2016.space).Trim();
                    utilityviewmodel.token = SmartParseV2016.Remove_Double_Spaces_V3(utilityviewmodel.token);
                    utilityviewmodel.token = utilityviewmodel.token.Replace(SmartParametersV2016.space, SmartParametersV2016.bar.ToString());
                    string[] components = utilityviewmodel.token.Split(SmartParametersV2016.bar);
                    if (!SectionH2_At(utilityviewmodel,
                                        components))
                    {
                        return false;
                    }
                }

                if (utilityviewmodel.token.IndexOf(SmartParametersV2016.billDateDelimiter) >= 0)
                {
                    if (!SectionH2_Bill_Date(utilityviewmodel))
                    {
                        return false;
                    }
                }
                else
                {
                    // Is it a number or text?
                    if (!SectionH2_Number_Or_Text(utilityviewmodel))
                    {
                        return false;
                    }
                }
            next_sub:
                sub_line_count++;
            }

            utilityviewmodel.token = lines[sub_line_count];
            if (utilityviewmodel.token.IndexOf(Standing_Charge) >= 0)
            {
                // Build the line up until you find a £
                // Loop round until we find a currency symbol
                if (!SectionH2_Standing_Chargess(utilityviewmodel,
                                                sections,
                                                section_index,
                                                lines,
                                                Standing_Charge,
                                                ref sub_line_count))
                {
                    return false;
                }
            }
            return true;
        }

        private static bool SectionH2_Check_Two_Tier(bool two_tier_meter,
                                                        string first_line,
                                                        string next_line,
                                                        string first_amount,
                                                        string next_amount)
        {
            if (two_tier_meter &&
                !string.IsNullOrEmpty(first_line) &&
                !string.IsNullOrEmpty(next_line) &&
                !string.IsNullOrEmpty(first_amount) &&
                !string.IsNullOrEmpty(next_amount))
            {
                return true;
            }
            return false;
        }

        private static bool SectionH2_Unit_Charges(UtilityViewModel utilityviewmodel,
                                                    string[] fuck)
        {
            for (int units_line = 0; units_line < utilityviewmodel.units_limit; units_line++)
            {
                if (utilityviewmodel.two_tier_meter)
                {
                    string[] components = fuck[units_line].Split(SmartParametersV2016.bar);
                    int component_count = 0;
                    while (component_count < components.Length)
                    {
                        switch (component_count)
                        {
                            case 0:
                                utilityviewmodel.UNITS_TYPE = components[component_count].Trim();
                                break;
                            case 1:
                                // This line Units
                                utilityviewmodel.UNITS = components[component_count].Trim();
                                if (!SmartParseV2016.Generic_Parse_Integer(utilityviewmodel.UNITS, utilityviewmodel))
                                {
                                    return false;
                                }
                                utilityviewmodel.UNITS_TYPE = utilityviewmodel.UNITS_TYPE + SmartParametersV2016.space + components[component_count];
                                break;
                            case 2:
                                // Units Cost
                                utilityviewmodel.UNITS_RATE = components[component_count].Replace(utilityviewmodel.bill_denomination_symbol.ToString(), "").Trim();
                                if (!SmartParseV2016.Generic_Parse_Decimal(utilityviewmodel.UNITS_RATE, utilityviewmodel))
                                {
                                    return false;
                                }
                                utilityviewmodel.UNITS_TYPE = utilityviewmodel.UNITS_TYPE + SmartParametersV2016.space +
                                                "at" + SmartParametersV2016.space +
                                                components[component_count];
                                break;
                            case 3:
                                // Total value UNITS_COST
                                // Units Cost
                                utilityviewmodel.UNITS_COST = components[component_count].Replace(utilityviewmodel.bill_currency_symbol, "");
                                utilityviewmodel.UNITS_COST = utilityviewmodel.UNITS_COST.Replace(utilityviewmodel.bill_currency_separator.ToString(), "").Trim();
                                utilityviewmodel.UNITS_COST = utilityviewmodel.UNITS_COST.Replace(utilityviewmodel.bill_thousands_separator.ToString(), "").Trim();
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
                }
                else
                {
                    // For the Charges when they happen
                    if (utilityviewmodel.STANDING_CHARGES_PERIOD_START == SmartParametersV2016.defaultDates)
                    {
                        utilityviewmodel.STANDING_CHARGES_PERIOD_START = utilityviewmodel.READINGS_PERIOD_START;
                    }
                    utilityviewmodel.STANDING_CHARGES_PERIOD_END = utilityviewmodel.READINGS_PERIOD_END;

                    utilityviewmodel.UNITS = utilityviewmodel.D_UNITS_USED_KWH;
                    utilityviewmodel.UNITS_TYPE = utilityviewmodel.UNITS + SmartParametersV2016.space +
                                    utilityviewmodel.UNIT_OF_MEASURE + SmartParametersV2016.space +
                                    "at" + SmartParametersV2016.space +
                                    utilityviewmodel.UNITS_RATE + utilityviewmodel.bill_denomination_symbol.ToString();

                    if (utilityviewmodel.UNITS_RATE != "0")
                    {
                        decimal units_cost = SmartRoutinesV2018.ConvertDecimal(utilityviewmodel.UNITS) * SmartRoutinesV2018.ConvertDecimal(utilityviewmodel.UNITS_RATE);
                        string units = units_cost.ToString();
                        int units_cost_int = 0;
                        string[] characteristic = units.Split(Convert.ToChar(SmartParametersV2016.decimalPoint));
                        if (characteristic.Length > 1)
                        {
                            units_cost_int = Convert.ToInt32(characteristic[0]);
                            int this_mantissa = Convert.ToInt32(characteristic[1]);
                            utilityviewmodel.total_mantissa += this_mantissa;
                            if (utilityviewmodel.total_mantissa >= 500)
                            {
                                units_cost_int++;
                                utilityviewmodel.total_mantissa -= 500;
                            }
                        }
                        utilityviewmodel.UNITS_COST = units_cost_int.ToString();
                    }
                }

                utilityviewmodel.UNIT_CHARGES_PERIOD_START = utilityviewmodel.READINGS_PERIOD_START;
                utilityviewmodel.UNIT_CHARGES_PERIOD_END = utilityviewmodel.READINGS_PERIOD_END;

                utilityviewmodel.units_band = (short)(utilityviewmodel.units_band + 1);
                SmartUtilityV2022.G_Unit_ChargesList_Add(utilityviewmodel,
                                                            SmartParametersV2016.Utility,
                                                            utilityviewmodel.supplier_code,
                                                            utilityviewmodel.brand_code,
                                                            utilityviewmodel.ACCOUNT_NO,
                                                            utilityviewmodel.CREATED,
                                                            utilityviewmodel.STATEMENT_ID,
                                                            utilityviewmodel.BILL_DATE,
                                                            utilityviewmodel.smell.MPRN,
                                                            utilityviewmodel.UNIT_CHARGES_PERIOD_START,
                                                            utilityviewmodel.UNIT_CHARGES_PERIOD_END,
                                                            utilityviewmodel.units_band.ToString(),
                                                            utilityviewmodel.UNITS_TYPE,
                                                            utilityviewmodel.UNITS,
                                                            utilityviewmodel.UNITS_RATE,
                                                            utilityviewmodel.UNIT_OF_MEASURE,
                                                            utilityviewmodel.UNITS_COST);
            }
            return true;
        }

        private static bool SectionH2_At(UtilityViewModel utilityviewmodel,
                                        string[] components)
        {
            int component_count = 0;
            while (component_count < components.Length)
            {
                switch (component_count)
                {
                    case 0:
                        // This line Units
                        utilityviewmodel.TOTAL_UNITS_USED = components[component_count].Trim();
                        if (!SmartParseV2016.Generic_Parse_Integer(utilityviewmodel.TOTAL_UNITS_USED, utilityviewmodel))
                        {
                            return false;
                        }
                        break;
                    case 1:
                        // Units Rate
                        utilityviewmodel.UNITS_RATE = components[component_count].Replace(utilityviewmodel.bill_denomination_symbol.ToString(), "").Trim();
                        if (!SmartParseV2016.Generic_Parse_Decimal(utilityviewmodel.UNITS_RATE, utilityviewmodel))
                        {
                            return false;
                        }
                        break;
                    case 2:
                        // Total value UNITS_COST
                        utilityviewmodel.UNITS_COST = components[component_count].Replace(utilityviewmodel.bill_denomination_symbol.ToString(), "").Trim();
                        break;
                    default:
                        break;
                }
                component_count++;
            }
            // Update all G Units with a Rate of "0"
            foreach (SmartUtility.GUnitCharges g_unit_charges_row in utilityviewmodel.Hezbollah.g_unit_charges_changesList)
            {
                if (g_unit_charges_row.UNITS_RATE == 0.0M)
                {
                    if (utilityviewmodel.UNITS_RATE != "0")
                    {
                        decimal units_cost = SmartRoutinesV2018.ConvertDecimal(utilityviewmodel.UNITS) * SmartRoutinesV2018.ConvertDecimal(utilityviewmodel.UNITS_RATE);
                        string units = units_cost.ToString();
                        int units_cost_int = 0;
                        string[] characteristic = units.Split(Convert.ToChar(SmartParametersV2016.decimalPoint));
                        if (characteristic.Length > 1)
                        {
                            units_cost_int = Convert.ToInt32(characteristic[0]);
                            int this_mantissa = Convert.ToInt32(characteristic[1]);
                            utilityviewmodel.total_mantissa += this_mantissa;
                            if (utilityviewmodel.total_mantissa >= 500)
                            {
                                units_cost_int++;
                                utilityviewmodel.total_mantissa -= 500;
                            }
                        }
                        utilityviewmodel.UNITS_COST = units_cost_int.ToString();
                        g_unit_charges_row.UNITS_RATE = SmartRoutinesV2018.ConvertDecimal(utilityviewmodel.UNITS_RATE);
                        g_unit_charges_row.UNITS_COST = Convert.ToInt32(utilityviewmodel.UNITS_COST);
                        g_unit_charges_row.UNITS_TYPE = g_unit_charges_row.UNITS_TYPE.Replace("0p", utilityviewmodel.UNITS_RATE + utilityviewmodel.bill_denomination_symbol);
                    }
                }
            }
            return true;
        }
        private static bool SectionH2_Bill_Date(UtilityViewModel utilityviewmodel)
        {
            utilityviewmodel.token = utilityviewmodel.token.Replace("PC", "");
            utilityviewmodel.token = utilityviewmodel.token.Replace("*", "").Trim();

            // We have found a date Lets check it
            if (!SmartParseV2016.Generic_Parse_Datetime(utilityviewmodel.token, utilityviewmodel))
            {
                return false;
            }
            if (utilityviewmodel.READINGS_PERIOD_START == SmartParametersV2016.defaultDates)
            {
                utilityviewmodel.READINGS_PERIOD_START = utilityviewmodel.token;
                // Keep going until we find another
            }
            else
            {
                if (utilityviewmodel.READINGS_PERIOD_END == SmartParametersV2016.defaultDates)
                {
                    utilityviewmodel.READINGS_PERIOD_END = utilityviewmodel.token;
                }
            }
            return true;
        }
        private static bool SectionH2_Number_Or_Text(UtilityViewModel utilityviewmodel)
        {
            if (SmartParseV2016.Generic_Parse_Digits(utilityviewmodel.token, utilityviewmodel))
            {
                if (!SmartParseV2016.Generic_Parse_Integer(utilityviewmodel.token, utilityviewmodel))
                {
                    return false;
                }
                if (utilityviewmodel.D_LAST_READ == "")
                {
                    utilityviewmodel.D_LAST_READ = utilityviewmodel.token;
                }
                else
                {
                    if (utilityviewmodel.D_THIS_READ == "0")
                    {
                        utilityviewmodel.D_THIS_READ = utilityviewmodel.token;
                    }
                }
            }
            else
            {
                // Its text
                if (string.IsNullOrEmpty(utilityviewmodel.LAST_READ_TYPE))
                {
                    utilityviewmodel.LAST_READ_TYPE = utilityviewmodel.token;
                }
                else
                {
                    if (string.IsNullOrEmpty(utilityviewmodel.THIS_READ_TYPE))
                    {
                        utilityviewmodel.THIS_READ_TYPE = utilityviewmodel.token;
                    }
                }
            }
            return true;
        }
        private static bool SectionH2_Standing_Chargess(UtilityViewModel utilityviewmodel,
                                                        string[][] sections,
                                                        int section_index,
                                                        string[] lines,
                                                        string Standing_Charge,
                                                        ref int sub_line_count)
        {
            int outer_index = sub_line_count;

            utilityviewmodel.token = "";
            while (outer_index <= Convert.ToInt32(sections[section_index][5]))
            {
                if (utilityviewmodel.token.IndexOf(utilityviewmodel.bill_currency_symbol) >= 0)
                {
                    Examine_Next_Line(utilityviewmodel, outer_index, sections, section_index, lines);
                    break;
                }
                outer_index++;
                utilityviewmodel.token = utilityviewmodel.token + SmartParametersV2016.space + lines[outer_index].Trim();
            }
            sub_line_count = outer_index;

            utilityviewmodel.token = utilityviewmodel.token.Replace(Standing_Charge, "").Trim();
            utilityviewmodel.token = utilityviewmodel.token.Replace("at", "");
            utilityviewmodel.token = utilityviewmodel.token.Replace("per day", "");
            utilityviewmodel.token = SmartParseV2016.Remove_Double_Spaces_V3(utilityviewmodel.token);
            string[] components = utilityviewmodel.token.Split(SmartParametersV2016.spaceSplit);
            int component_count = 0;
            utilityviewmodel.CHARGES_TYPE = "";
            utilityviewmodel.CHARGES_DAYS = "";
            utilityviewmodel.UNIT_OF_MEASURE = "";
            utilityviewmodel.STANDING_CHARGE = "0";
            utilityviewmodel.CHARGES_COST = "";
            while (component_count < components.Length)
            {
                switch (component_count)
                {
                    case 0:
                        if (!SmartParseV2016.Generic_Parse_Integer(components[component_count], utilityviewmodel))
                        {
                            return false;
                        }
                        utilityviewmodel.CHARGES_DAYS = components[component_count];
                        break;
                    case 1:
                        // days
                        utilityviewmodel.UNIT_OF_MEASURE = components[component_count];
                        break;
                    case 2:
                        //decimal temp_decimal = 0.0M;
                        utilityviewmodel.STANDING_CHARGE = components[component_count].Replace(utilityviewmodel.bill_denomination_symbol.ToString(), "");
                        if (!SmartParseV2016.Generic_Parse_Decimal(utilityviewmodel.STANDING_CHARGE, utilityviewmodel))
                        {
                            return false;
                        }
                        break;
                    case 3:
                        utilityviewmodel.CHARGES_COST = components[component_count].Replace(utilityviewmodel.bill_currency_symbol, "");
                        utilityviewmodel.CHARGES_COST = utilityviewmodel.CHARGES_COST.Replace(utilityviewmodel.bill_currency_separator.ToString(), "");
                        utilityviewmodel.CHARGES_COST = utilityviewmodel.CHARGES_COST.Replace(utilityviewmodel.bill_thousands_separator.ToString(), "");
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

            utilityviewmodel.CHARGES_TYPE = utilityviewmodel.CHARGES_DAYS + SmartParametersV2016.space +
                            utilityviewmodel.UNIT_OF_MEASURE + SmartParametersV2016.space +
                            "at" + SmartParametersV2016.space +
                            utilityviewmodel.STANDING_CHARGE + utilityviewmodel.bill_denomination_symbol + SmartParametersV2016.space +
                            "per day";

            utilityviewmodel.charges_item = (short)(utilityviewmodel.charges_item + 1);
            SmartUtilityV2022.G_Standing_ChargesList_Add(utilityviewmodel,
                                                            SmartParametersV2016.Utility,
                                                            utilityviewmodel.supplier_code,
                                                            utilityviewmodel.brand_code,
                                                            utilityviewmodel.ACCOUNT_NO,
                                                            utilityviewmodel.CREATED,
                                                            utilityviewmodel.STATEMENT_ID,
                                                            utilityviewmodel.BILL_DATE,
                                                            utilityviewmodel.smell.MPRN,
                                                            utilityviewmodel.STANDING_CHARGES_PERIOD_START,
                                                            utilityviewmodel.STANDING_CHARGES_PERIOD_END,
                                                            utilityviewmodel.charges_item.ToString(),
                                                            utilityviewmodel.CHARGES_TYPE,
                                                            utilityviewmodel.STANDING_CHARGE,
                                                            utilityviewmodel.CHARGES_DAYS,
                                                            utilityviewmodel.CHARGES_COST);
            utilityviewmodel.STANDING_CHARGES_PERIOD_START = SmartParametersV2016.defaultDates;
            utilityviewmodel.STANDING_CHARGES_PERIOD_END = SmartParametersV2016.defaultDates;
            return true;
        }

        private static bool SectionH2_Discounts(MainViewModel ourviewmodel,
                                                UtilityViewModel utilityviewmodel,
                                                short DISCOUNT_VAT_CODE,
                                                string direct_debit_discount)
        {
            //int temp = 0;

            utilityviewmodel.DISCOUNT_TYPE = direct_debit_discount;
            utilityviewmodel.DISCOUNT_AMOUNT = "";
            if (!SmartParseV2016.Currency_Strip(utilityviewmodel.bill_currency_symbol, utilityviewmodel.bill_currency_separator, utilityviewmodel.bill_thousands_separator, utilityviewmodel.token, utilityviewmodel))
            {
                return false;
            }
            else
            {
                utilityviewmodel.DISCOUNT_AMOUNT = utilityviewmodel.value;
            }
            if (!SmartParseV2016.Generic_Parse_Integer(utilityviewmodel.DISCOUNT_AMOUNT, utilityviewmodel))
            {
                return false;
            }
            // Get some kind of discount date
            utilityviewmodel.DISCOUNT_DATE = SmartNibbyV2016.ConvertDate(utilityviewmodel.BILL_PERIOD_END,
                SmartParametersV2016.defaultDate, ourviewmodel);
            if (ourviewmodel.errorMessage != "")
            {
                return false;
            }
            utilityviewmodel.discount_item = (short)(utilityviewmodel.discount_item + 1);     // <= Can put this in MES if needs be to carry ot through
            if (!SmartParseV2016.Check_Discount_Is_There(utilityviewmodel,
                                                        utilityviewmodel.DISCOUNT_DATE,
                                                        utilityviewmodel.discount_item,
                                                        utilityviewmodel.DISCOUNT_TYPE,
                                                        DISCOUNT_VAT_CODE,
                                                        utilityviewmodel.DISCOUNT_AMOUNT))
            {
                SmartParseV2016.No_Wonder_I_Cant_Fucking_Work(utilityviewmodel,
                                                                utilityviewmodel.DISCOUNT_DATE,
                                                                utilityviewmodel.discount_item,
                                                                utilityviewmodel.DISCOUNT_TYPE,
                                                                DISCOUNT_VAT_CODE,
                                                                utilityviewmodel.DISCOUNT_AMOUNT);
            }
            return true;
        }
        private static bool SectionH2_Vat_Code(MainViewModel ourviewmodel,
                                                UtilityViewModel utilityviewmodel,
                                                string[] components)
        {
            //int temp = 0;
            int components_count = 0;
            while (components_count < components.Length)
            {
                switch (components_count)
                {
                    case 2:
                        string VAT_AMOUNT;// = "0";
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
                        SmartParseV2016.Update_Bill(ourviewmodel, utilityviewmodel, "RESOURCE_VAT_AMOUNT", VAT_AMOUNT);
                        break;
                    default:
                        break;
                }
                components_count++;
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
                    (lines[temp_sub].Trim() == "debit") ||
                    (lines[temp_sub].Trim() == "in credit") ||
                    (lines[temp_sub].Trim() == "credit"))
                {
                    // Tack it after the utilityviewmodel.token
                    utilityviewmodel.token = utilityviewmodel.token + SmartParametersV2016.space + lines[temp_sub].Trim();
                }
            }
            return;
        }

        private static bool Parse_SectionI0(MainViewModel ourviewmodel,
                                                       UtilityViewModel utilityviewmodel,
                                                       string[][] sections,
                                                       string section,
                                                       string sub_section,
                                                       string[] lines)
        {
            utilityviewmodel.currentIndex = 0;
            if (!SmartParseV2016.Decode_Section(section, sub_section, sections, utilityviewmodel.resource_code.ToString(), utilityviewmodel))
            {
                // We are here because we don't want (or need) to do this section
                return true;    // Always - because we haven't had a CONVERSION error
            }

            // Do what I do, say what I say, live like I live Do what I do do what i do ad nauseam

            utilityviewmodel.token = "";

            string balance_on_last_bill = "Balance on last bill",
                    payment_received = "Payment received",
                    payments_received = "Payments received",
                    account_balance_for_information = "Account balance for information";    // Only want to do the FIRST of these
            bool outstanding_balance = false;
            short ACCOUNT_VAT_CODE = SmartParametersV2016.zeroRateVatCode;
            utilityviewmodel.this_resource = -1;
#if WINFORMS
            if (utilityviewmodel.BILL_DATE == "29 May 2012")
            {
                SmartParseV2016.Update_Bill(ourviewmodel, utilityviewmodel, "RESOURCE_BALANCES", true.ToString());
                switch (utilityviewmodel.resource_code)
                {
                    case SmartParametersV2016.Electricity:
                        SmartParseV2016.Update_Bill(ourviewmodel, utilityviewmodel, "PREVIOUS_RESOURCE_BALANCE", "12584");
                        SmartParseV2016.Update_Bill(ourviewmodel, utilityviewmodel, "OUTSTANDING_RESOURCE_BALANCE", "5812");

                        break;
                    case SmartParametersV2016.Gas:
                        SmartParseV2016.Update_Bill(ourviewmodel, utilityviewmodel, "PREVIOUS_RESOURCE_BALANCE", "4016");
                        SmartParseV2016.Update_Bill(ourviewmodel, utilityviewmodel, "OUTSTANDING_RESOURCE_BALANCE", "4289");
                        break;
                    default:
                        break;
                }
                return true;
            }
#endif
            int sub_line_count = 0;

            for (int line_count = Convert.ToInt32(sections[utilityviewmodel.currentIndex][4]); line_count <= Convert.ToInt32(sections[utilityviewmodel.currentIndex][5]); line_count++)
            {
                utilityviewmodel.token = lines[line_count].Trim();
                // Has to be this way round!
                if (SmartParseV2016.Token_Identify(utilityviewmodel, balance_on_last_bill, false))
                {
                    // Thick bitch has just fucked off without even saying goodbye.
                    // Moronic. Stupid. Ignorant. Arrogant. 
                    sub_line_count = line_count;
                    if (!SectionI0_Previous_Balance(ourviewmodel,
                                            utilityviewmodel,
                                            sections,
                                            utilityviewmodel.currentIndex,
                                            lines,
                                            balance_on_last_bill,
                                            ref sub_line_count))
                    {
                        return false;
                    }
                    line_count = sub_line_count;
                    goto the_old_daysI0;
                }

                if (SmartParseV2016.Token_Identify(utilityviewmodel, payment_received +
                                                                SmartParametersV2016.bar +
                                                                payments_received, true))
                {
                    sub_line_count = line_count;
                    if (!SectionI0_Resource_Payments(ourviewmodel,
                                            utilityviewmodel,
                                            sections,
                                            utilityviewmodel.currentIndex,
                                            lines,
                                            payment_received,
                                            payments_received,
                                            ref sub_line_count))
                    {
                        return false;
                    }
                    line_count = sub_line_count - 1;    // Re-do the line
                    goto the_old_daysI0;
                }


                if (utilityviewmodel.token.IndexOf("Transfer") >= 0)
                {
                    sub_line_count = line_count;
                    if (!SectionI0_Resource_Transfers(ourviewmodel,
                                            utilityviewmodel,
                                            sections,
                                            utilityviewmodel.currentIndex,
                                            lines,
                                            ACCOUNT_VAT_CODE,
                                            ref sub_line_count))
                    {
                        return false;
                    }
                    line_count = sub_line_count;
                    goto the_old_daysI0;
                }

                if (SmartParseV2016.Token_Identify(utilityviewmodel, account_balance_for_information, true))
                {
                    sub_line_count = line_count;
                    if (!outstanding_balance &&              // have we already done this?
                        !SectionI0_Outstanding_Balance(ourviewmodel,
                                                        utilityviewmodel,
                                                        sections,
                                                        utilityviewmodel.currentIndex,
                                                        lines,
                                                        account_balance_for_information,
                                                        ref sub_line_count))
                    {
                        return false;
                    }
                    outstanding_balance = true;
                    line_count = sub_line_count + 1;
                    goto the_old_daysI0;
                }
            the_old_daysI0:
                continue;
            }
            return true;
        }

        private static bool SectionI0_Previous_Balance(MainViewModel ourviewmodel,
                                                        UtilityViewModel utilityviewmodel,
                                                        string[][] sections,
                                                        int section_index,
                                                        string[] lines,
                                                        string balance_on_last_bill,
                                                        ref int sub_line_count)
        {
            while (sub_line_count <= Convert.ToInt32(sections[section_index][5]))
            {
                // THICK as a fucking plank
                utilityviewmodel.token = lines[sub_line_count].Trim();

                utilityviewmodel.token = utilityviewmodel.token.Replace(balance_on_last_bill, "").Trim();
                if (utilityviewmodel.token.IndexOf(utilityviewmodel.bill_currency_symbol) >= 0)
                {
                    utilityviewmodel.token = utilityviewmodel.token.Replace(" credit", "CR");
                    utilityviewmodel.token = utilityviewmodel.token.Replace(" debit", "DR");
                    utilityviewmodel.token = SmartParseV2016.Remove_Double_Spaces_V3(utilityviewmodel.token);

                    utilityviewmodel.token = utilityviewmodel.token.Replace(SmartParametersV2016.space, SmartParametersV2016.bar.ToString());
                    utilityviewmodel.token = utilityviewmodel.token.Trim(SmartParametersV2016.bar);
                    string[] components = utilityviewmodel.token.Split(SmartParametersV2016.bar);

                    if (components.Length == 1)
                    {
                        // Whichever resource we are doing is the 'first'
                        utilityviewmodel.this_resource = 0;
                    }
                    else
                    {
                        if (components.Length == 2)
                        {
                            switch (utilityviewmodel.resource_code)
                            {
                                case SmartParametersV2016.Electricity:
                                    utilityviewmodel.this_resource = 0;
                                    break;
                                case SmartParametersV2016.Gas:
                                    utilityviewmodel.this_resource = 1;
                                    break;
                                default:
                                    break;
                            }
                        }
                    }
                    int temp;// = 0;
                    utilityviewmodel.PREVIOUS_BALANCE = "";
                    if (!SmartParseV2016.Currency_Strip(utilityviewmodel.bill_currency_symbol, utilityviewmodel.bill_currency_separator, utilityviewmodel.bill_thousands_separator, components[utilityviewmodel.this_resource], utilityviewmodel))
                    {
                        return false;
                    }
                    utilityviewmodel.PREVIOUS_BALANCE = utilityviewmodel.value;

                    if (!SmartParseV2016.Generic_Parse_Integer(utilityviewmodel.PREVIOUS_BALANCE, utilityviewmodel))
                    {
                        return false;
                    }
                    else
                    {
                        temp = utilityviewmodel.genericTransactionValue;
                    }
                    utilityviewmodel.PREVIOUS_BALANCE = (temp + utilityviewmodel.Hezbollah.bills_row.PREVIOUS_BALANCE).ToString();   // Which will either be 0 or contain something?
                    SmartParseV2016.Update_Bill(ourviewmodel, utilityviewmodel, "RESOURCE_BALANCES", true.ToString()); // This
                    SmartParseV2016.Update_Bill(ourviewmodel, utilityviewmodel, "PREVIOUS_RESOURCE_BALANCE", utilityviewmodel.PREVIOUS_BALANCE); // This
                    break;
                }
                sub_line_count++;
            }
            return true;
        }
        private static bool SectionI0_Resource_Payments(MainViewModel ourviewmodel,
                                                        UtilityViewModel utilityviewmodel,
                                                        string[][] sections,
                                                        int section_index,
                                                        string[] lines,
                                                        string payment_received,
                                                        string payments_received,
                                                        ref int sub_line_count)
        {
            while (sub_line_count <= Convert.ToInt32(sections[section_index][5]))
            {
                utilityviewmodel.token = lines[sub_line_count].Trim();

                if ((utilityviewmodel.token.IndexOf("Transfer") >= 0) ||
                    (utilityviewmodel.token.IndexOf("Account") >= 0))
                {
                    break;
                }

                if (SmartParseV2016.Token_Identify(utilityviewmodel, payment_received +
                                                                SmartParametersV2016.bar +
                                                                payments_received, true))
                {
                    utilityviewmodel.token = utilityviewmodel.token.Replace("with thanks", "");
                    utilityviewmodel.token = utilityviewmodel.token.Replace("(or transfers)", "");
                    utilityviewmodel.token = utilityviewmodel.token.Replace("on", "").Trim();
                    utilityviewmodel.PAYMENT_AMOUNT = "";// = "0",
                    utilityviewmodel.PAYMENT_METHOD = "";
                    utilityviewmodel.PAYMENT_DATE = SmartParametersV2016.defaultDates;

                    utilityviewmodel.token = SmartParseV2016.Remove_Double_Spaces_V3(utilityviewmodel.token);
                    utilityviewmodel.token = utilityviewmodel.token.Replace(SmartParametersV2016.space, SmartParametersV2016.bar.ToString());
                    if (utilityviewmodel.token.IndexOf(SmartParametersV2016.billDateDelimiter) >= 0)
                    {
                        string[] components = utilityviewmodel.token.Split(SmartParametersV2016.bar);
                        if (components.Length > 0)
                        {
                            utilityviewmodel.PAYMENT_DATE = components[0];
                            if (!SmartParseV2016.Generic_Parse_Datetime(utilityviewmodel.PAYMENT_DATE, utilityviewmodel))
                            {
                                return false;
                            }
                            if (components.Length == 2)
                            {
                                // Whichever resource we are doing is the 'first'
                                utilityviewmodel.this_resource = 1;
                            }
                            else
                            {
                                if (components.Length == 3)
                                {
                                    switch (utilityviewmodel.resource_code)
                                    {
                                        // These are the WRONG WAY ROUND becuase the
                                        // fucking NPOWER BILL returns them the WRONG WAY ROUND
                                        // i.e. with the GAS on the left and the Electricity on the RIGHT
                                        // despite the Bill showing the Electricity on the LEFT and
                                        // the Gas on the RIGHT!!!!
                                        case SmartParametersV2016.Electricity:
                                            utilityviewmodel.this_resource = 1;  // Should be 0
                                            break;
                                        case SmartParametersV2016.Gas:
                                            utilityviewmodel.this_resource = 2;  // Should be 1
                                            break;
                                        default:
                                            break;
                                    }
                                }
                            }
                            //int payment_amount = 0;
                            if (!SmartParseV2016.Currency_Strip(utilityviewmodel.bill_currency_symbol, utilityviewmodel.bill_currency_separator, utilityviewmodel.bill_thousands_separator, components[utilityviewmodel.this_resource], utilityviewmodel))
                            {
                                return false;
                            }
                            utilityviewmodel.PAYMENT_AMOUNT = utilityviewmodel.value;
                            if (!SmartParseV2016.Generic_Parse_Integer(utilityviewmodel.PAYMENT_AMOUNT, utilityviewmodel))
                            {
                                return false;
                            }
                            utilityviewmodel.PAYMENT_METHOD = payment_received;

                            utilityviewmodel.PAYMENT_CODE = "";
                            if (!string.IsNullOrEmpty(utilityviewmodel.PAYMENT_METHOD))
                            {
                                if (!SmartSpikeUtilityV2017.Utility_Lookup_PaymentCode(ourviewmodel,
                                                                                utilityviewmodel,
                                                                                utilityviewmodel.PAYMENT_METHOD))
                                {
                                    return false;
                                }
                                //utilityviewmodel.PAYMENT_METHOD = utilityviewmodel.PAYMENT_METHOD;
                                if (!string.IsNullOrEmpty(utilityviewmodel.PAYMENT_DATE) &&
                                    !string.IsNullOrEmpty(utilityviewmodel.PAYMENT_AMOUNT))
                                {
                                    // Add it in ... perhaps
                                    // If this section is E(lectricity) and G(as) then we might be doing it twice!
                                    // Check if its there first
                                    utilityviewmodel.PAYMENT_BALANCE = utilityviewmodel.PAYMENTS_BALANCE.ToString();
                                    if (!SmartParseV2016.Check_Payment_Is_There(ourviewmodel,
                                                                                utilityviewmodel))
                                    // Which IS the PAYMENT_DATE as a DateTime
                                    //utilityviewmodel.PAYMENTS_ITEM,                                                                                
                                    {
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
                                        //                                    payment_amount,
                                        //                                    utilityviewmodel.PAYMENTS_BALANCE);
                                        foreach (SmartUtility.Payments payment_row in utilityviewmodel.Hezbollah.payments_changesList)
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
                                    SmartParseV2016.Update_Bill(ourviewmodel, utilityviewmodel, "PAYMENT_AMOUNT", utilityviewmodel.PAYMENT_AMOUNT);
                                }
                            }
                        }
                    }
                }
                sub_line_count++;
            }
            return true;
        }
        private static bool SectionI0_Resource_Transfers(MainViewModel ourviewmodel,
                                                        UtilityViewModel utilityviewmodel,
                                                        string[][] sections,
                                                        int section_index,
                                                        string[] lines,
                                                        short ACCOUNT_VAT_CODE,
                                                        ref int sub_line_count)
        {
            while (sub_line_count <= Convert.ToInt32(sections[section_index][5]))
            {
                utilityviewmodel.token = lines[sub_line_count].Trim();

                if (utilityviewmodel.token.IndexOf("Account") >= 0)
                {
                    break;
                }
                if (utilityviewmodel.token.IndexOf("Transfer") >= 0)
                {
                    string ACCOUNT_TYPE = utilityviewmodel.token.Trim();

                    if (sub_line_count + 1 <= Convert.ToInt32(sections[section_index][5]))
                    {
                        sub_line_count++;
                        utilityviewmodel.token = lines[sub_line_count];
                        utilityviewmodel.token = utilityviewmodel.token.Replace(SmartParametersV2016.space, SmartParametersV2016.bar.ToString());

                        string[] components = utilityviewmodel.token.Split(SmartParametersV2016.bar);
                        if (components.Length == 2)    // No date, just values
                        {
                            switch (utilityviewmodel.resource_code)
                            {
                                case SmartParametersV2016.Electricity:
                                    utilityviewmodel.this_resource = 0;
                                    break;
                                case SmartParametersV2016.Gas:
                                    utilityviewmodel.this_resource = 1;
                                    break;
                                default:
                                    break;
                            }
                            int temp;
                            utilityviewmodel.ACCOUNT_AMOUNT = "";
                            if (!SmartParseV2016.Currency_Strip(utilityviewmodel.bill_currency_symbol, utilityviewmodel.bill_currency_separator, utilityviewmodel.bill_thousands_separator, components[utilityviewmodel.this_resource], utilityviewmodel))
                            {
                                return false;
                            }
                            utilityviewmodel.ACCOUNT_AMOUNT = utilityviewmodel.value;
                            if (!SmartParseV2016.Generic_Parse_Integer(utilityviewmodel.ACCOUNT_AMOUNT, utilityviewmodel))
                            {
                                return false;
                            }
                            else
                            {
                                temp = utilityviewmodel.genericTransactionValue;
                            }
                            // Add it in unless its zero
                            if (temp != 0)
                            {
                                // I couldn't give a shit what you did. And you are like a DOG WITH A FUCKING
                                // BONE over that FUCKING TREE.  SHUT THE FUCK UP ABOUT THAT FUCKING TREE
                                string ACCOUNT_DATE = utilityviewmodel.BILL_PERIOD_END;
                                string INCLUDE_BILLS = SmartParametersV2016.yesFlag;

                                utilityviewmodel.ACCOUNT_CHARGES_CREDITS_ITEM = (short)(utilityviewmodel.ACCOUNT_CHARGES_CREDITS_ITEM + 1);
                                bool is_it_there = false;
                                {
                                    if (string.IsNullOrEmpty(sections[section_index][2]))
                                    {
                                        is_it_there = SmartParseV2016.Check_Adjustment_Is_There(ourviewmodel,
                                                                                                utilityviewmodel,
                                                                                                SmartTimeV2016.ConvertDateTime(ACCOUNT_DATE),
                                                                                                utilityviewmodel.ACCOUNT_CHARGES_CREDITS_ITEM,
                                                                                                ACCOUNT_TYPE,
                                                                                                ACCOUNT_VAT_CODE,
                                                                                                Convert.ToInt32(utilityviewmodel.ACCOUNT_AMOUNT));
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
                                                                                        SmartTimeV2016.ConvertDateTime(ACCOUNT_DATE),
                                                                                        utilityviewmodel.ACCOUNT_CHARGES_CREDITS_ITEM,
                                                                                        ACCOUNT_TYPE,
                                                                                        ACCOUNT_VAT_CODE,
                                                                                        Convert.ToInt32(utilityviewmodel.ACCOUNT_AMOUNT),
                                                                                        INCLUDE_BILLS);
                                    SmartParseV2016.Update_Bill(ourviewmodel, utilityviewmodel, "ACCOUNT_CHARGES_CREDITS", utilityviewmodel.ACCOUNT_AMOUNT);
                                }
                            }
                        }
                    }
                    break;
                }
                sub_line_count++;
            }
            return true;
        }
        private static bool SectionI0_Outstanding_Balance(MainViewModel ourviewmodel,
                                                        UtilityViewModel utilityviewmodel,
                                                        string[][] sections,
                                                        int section_index,
                                                        string[] lines,
                                                        string account_balance_for_information,
                                                        ref int sub_line_count)
        {

            while (sub_line_count <= Convert.ToInt32(sections[section_index][5]))
            {
                utilityviewmodel.token = lines[sub_line_count].Trim();

                utilityviewmodel.token = utilityviewmodel.token.Replace(account_balance_for_information, "");
                if (utilityviewmodel.token.IndexOf(utilityviewmodel.bill_currency_symbol) >= 0)
                {
                    utilityviewmodel.token = utilityviewmodel.token.Replace(" credit", "CR");
                    utilityviewmodel.token = utilityviewmodel.token.Replace(" debit", "DR");
                    utilityviewmodel.token = SmartParseV2016.Remove_Double_Spaces_V3(utilityviewmodel.token);

                    utilityviewmodel.token = utilityviewmodel.token.Replace(SmartParametersV2016.space, SmartParametersV2016.bar.ToString());
                    utilityviewmodel.token = utilityviewmodel.token.Trim(SmartParametersV2016.bar);
                    string[] components = utilityviewmodel.token.Split(SmartParametersV2016.bar);
                    if (components.Length == 1)
                    {
                        // Whichever resource we are doing is the 'first'
                        utilityviewmodel.this_resource = 0;
                    }
                    else
                    {
                        if (components.Length == 2)
                        {
                            switch (utilityviewmodel.resource_code)
                            {
                                case SmartParametersV2016.Electricity:
                                    utilityviewmodel.this_resource = 0;
                                    break;
                                case SmartParametersV2016.Gas:
                                    utilityviewmodel.this_resource = 1;
                                    break;
                                default:
                                    break;
                            }
                        }
                    }
                    //int temp;
                    utilityviewmodel.OUTSTANDING_BALANCE = "";
                    if (!SmartParseV2016.Currency_Strip(utilityviewmodel.bill_currency_symbol, utilityviewmodel.bill_currency_separator, utilityviewmodel.bill_thousands_separator, components[utilityviewmodel.this_resource], utilityviewmodel))
                    {
                        return false;
                    }
                    utilityviewmodel.OUTSTANDING_BALANCE = utilityviewmodel.value;
                    if (!SmartParseV2016.Generic_Parse_Integer(utilityviewmodel.OUTSTANDING_BALANCE, utilityviewmodel))
                    {
                        return false;
                    }
                    SmartParseV2016.Update_Bill(ourviewmodel, utilityviewmodel, "RESOURCE_BALANCES", true.ToString()); // Might already be set
                    SmartParseV2016.Update_Bill(ourviewmodel, utilityviewmodel, "OUTSTANDING_RESOURCE_BALANCE", utilityviewmodel.OUTSTANDING_BALANCE);
                    break;
                }
                sub_line_count++;
            }
            return true;
        }
    }
}