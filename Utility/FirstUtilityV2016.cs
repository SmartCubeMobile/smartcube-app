using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iText.Kernel.Pdf;
using System.Text.Json;
using System.Text.Json.Serialization;
using HtmlAgilityPack;
using System.Diagnostics.CodeAnalysis;



#if ANDROIDX
using AndroidX.AppCompat.App;
#endif
namespace SmartCubeMobile
{
    public class FirstUtilityV2016
    {
        [RequiresUnreferencedCode("Calls SmartCubeMobile.FirstUtilityV2016.FU_AccountOverview(MainViewModel, UtilityViewModel, HtmlDocument)")]
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
            // One day I will be free of all of this ... one day ....

            // *******************************  NOTE   *********************
            // First Utility is fairly unusual in the fact that the Url prfix ends WITHOUT a slash ('/')
            // This is because all the targets scraped START with a slash ('/') and I don't want Urls
            // such as 'www.first-utility.co.uk//login' to appear when I join them.  So there is NO TEST
            // for targets which are scraped to see if they have leading slashes ('/') as I want to keep
            // the code as simple as possible. i.e. the philosophy behind this is 'if I can make the change
            // in the data, then that's better than making changes in the code ...'  45+ years of experience
            // has taught me SOMETHING!!
            // *******************************  NOTE   *********************

            // We ONLY need this to convert HTML documents ... and when - one
            // day - we find a better way, this fucking useless object is BINNED
            // It is now fucking binned WebBrowser wb = new WebBrowser();
            // ========================

            // Because Sometimes we are returned a Document
            utilityviewmodel.htmlDocument = new HtmlAgilityPack.HtmlDocument();

            utilityviewmodel.keyValues = new List<KeyValuePair<string, string>>();

            // Everytime timer ticks, timer_Tick will be called
            // Timer will tick evert second
            // FUCKING HELL - this is never Enabled????
            // You have 60 Seconds to stop
            // the timer on a good login!
            string myaccount_energy = "myaccount/energy",  // <= this is the default page, which we never decode via an href!
                    bills_pathname = "",
                    payments_pathname = "",
                    details_pathname = "";
            utilityviewmodel.graph_electricity_pathname = "";
            utilityviewmodel.graph_gas_pathname = "";

            utilityviewmodel.keep_looping = true;
            List<SmartUtility.Payments> payments_tempList = new List<SmartUtility.Payments>();

            while (utilityviewmodel.keep_looping)
            {
#if WINFORMS
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

                utilityviewmodel.keep_looping = await SmartUtilityV2022.Prelude(ourviewmodel, utilityviewmodel, utilityviewmodel.keyValues);


                utilityviewmodel.current_routine = utilityviewmodel.next_routine;
                switch (utilityviewmodel.next_routine)
                {
                    case "LOGIN":
                        // The timer is re-started when the
                        // first Login Document is completed
                        utilityviewmodel.keyValues.Clear();
                        if (await FU_Login(
#if ANDROIDX
                                        meterActivity,
#endif
                                        ourviewmodel,
                                        utilityviewmodel,
                                        utilityviewmodel.htmlDocument,
                                        TextBox_Active))
                        {
                            utilityviewmodel.target_pathname = "/login";

                            utilityviewmodel.htmlDocument = await SmartBobV2017.Scraper_Generic_Post(ourviewmodel,
                                                                    utilityviewmodel.utilityToken,
                                                                    utilityviewmodel.keyValues,
                                                                    utilityviewmodel);
                            if (!string.IsNullOrEmpty(ourviewmodel.errorMessage))
                            {
                                return false;
                            }
                            // Hook up the next routine - if the login is true - for the next 'Document Completed' delivery
                            utilityviewmodel.next_routine = "HOME";
                            // Should now be going on to HOME
                        }
                        else
                        {
                            // We can't login for some reason (site is being maintained??)
                            // But WE didn't fail so mark status as success and quit
                            ourviewmodel.errorMessage = utilityviewmodel.current_routine;
                            utilityviewmodel.next_routine = "LOGOUT";
                            utilityviewmodel.login_finished = true;          // Escape route on Login failure
                        }
                        break;
                    case "HOME":

                        if (!await FU_AccountOverview(ourviewmodel,
                                        utilityviewmodel,
                                        utilityviewmodel.htmlDocument))
                        {
                            if (!await SmartRoutinesV2018.CheckTrace(ourviewmodel, utilityviewmodel.utilityToken, utilityviewmodel.supplier_code, utilityviewmodel.brand_code, SmartParametersV2016.Utility.ToString() + " " + ourviewmodel.errorMessage))
                            {
                                return false;
                            }
                            utilityviewmodel.next_routine = "LOGOUT";
                        }

                        // If its already been done then click on the View Usage button and
                        // come back to this screen ... so we cycle round here a few times
                        break;
                    case "BASICDETAILS":

                        if (!await FU_BasicDetails(ourviewmodel,
                                                 utilityviewmodel,
                                                utilityviewmodel.htmlDocument))

                        {
#if WINFORMS
                            if (utilityviewmodel.console)
                            {
                                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel,
                                    (DateTime.Now + ourviewmodel.utcOffset).ToString(SmartParametersV2016.sqliteformat) + // Local time
                                    SmartParametersV2016.space + utilityviewmodel.brand_code + SmartParametersV2016.space + utilityviewmodel.supplier_code + SmartParametersV2016.space + ourviewmodel.errorMessage);
                            }
#endif
                            if (utilityviewmodel.examine.scrape)
                            {
                                if (!await SmartRoutinesV2018.CheckTrace(ourviewmodel, utilityviewmodel.utilityToken, utilityviewmodel.supplier_code, utilityviewmodel.brand_code, SmartParametersV2016.Utility.ToString() + " " + ourviewmodel.errorMessage))
                                {
                                    return false;
                                }
                            }
                            // Probably going on to LOGOUT from here
                        }
                        break;
                    case "USAGE":
                        //
                        // THIS SHIT IS NEARLY KILLING ME
                        // I CANNOT TAKE MUCH MORE - I AM ON THE EDGE WITH THIS FUCKING STUFF
                        // ...and yet still I carried on .. year
                        // after year of unrelenting pain ...
                        await FU_Usage(ourviewmodel,
                                        utilityviewmodel,
                                        utilityviewmodel.htmlDocument,
                                        bills_pathname,
                                        details_pathname);

                        // Should now be going on to VIEWBILLS
                        break;
                    case "VIEWBILLS":
                        
                        await FU_ViewBills(
#if ANDROIDX
                                            meterActivity,
#endif
                                            ourviewmodel,
                                            utilityviewmodel,
                                            utilityviewmodel.htmlDocument,
                                            TextBox_Active,
                                            payments_pathname);

                        // Should now be going on to VIEWPAYMENTS
                        break;
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
                        break;
                    default:
                        // A small price to pay ...
#if WINFORMS
                        await FU_Do_Intermediates(ourviewmodel,
#endif
#if WPF  || WINUI || SMARTMAUI
                        FU_Do_Intermediates(ourviewmodel,
#endif
#if ANDROIDX
                        FU_Do_Intermediates(ourviewmodel,
#endif
                        utilityviewmodel,
                                                utilityviewmodel.next_routine,
                                                utilityviewmodel.htmlDocument,
                                                myaccount_energy,
                                                payments_tempList);
                        break;
                }
            }

            // Now .. there is no way of telling whether we are logged in or
            // not as we don't ever test for the "Login failed" message.
            // HOWEVER, if you look further down into the program you will
            // see that I cancel the timer in about the 3rd routine because
            // to have got THAT FAR! we must have been logged in.
            // Cannot assume that just because we don't find any Utility.Accounts,
            // we are not logged in ...
            await SmartUtilityV2022.Last_Throw(
#if ANDROIDX
                                    meterActivity,
#endif
                                    ourviewmodel,
                                    utilityviewmodel);

            // Yay!
            return utilityviewmodel.login_finished;
        }

#if WINFORMS
        private static async Task<bool> FU_Do_Intermediates(MainViewModel ourviewmodel,
#endif
#if WPF  || WINUI || SMARTMAUI
        private static bool FU_Do_Intermediates(MainViewModel ourviewmodel,
#endif
#if ANDROIDX
        private static bool FU_Do_Intermediates(MainViewModel ourviewmodel,
#endif
                                                UtilityViewModel utilityviewmodel,
                                                string next_routine,
                                                HtmlAgilityPack.HtmlDocument htmlDocument,
                                                string myaccount_energy,
                                                List<SmartUtility.Payments> payments_tempList)
        {
            switch (next_routine)
            {
                case "PROCRESOURCE":
#if WINFORMS
                    if (!await FU_Process_Resource(ourviewmodel, utilityviewmodel))
#endif
#if WPF  || WINUI || SMARTMAUI
                    if (!FU_Process_Resource(ourviewmodel, utilityviewmodel))
#endif
#if ANDROIDX
                    if (!FU_Process_Resource(ourviewmodel, utilityviewmodel))
#endif
                    {
                        return false;
                    }
                    break;

                case "ACCSUMMARY":
                    FU_Accountsummary(utilityviewmodel,
                                        htmlDocument);
                    break;
                case "ACCDETAILS":
                    FU_AccountDetails(utilityviewmodel,
                                        htmlDocument);
                    // Should now be going on to USAGE  DETAILS
                    break;
                case "VIEWPAYMENTS":
                    FU_ViewPayments(ourviewmodel,
                                        utilityviewmodel,
                                        payments_tempList,
                                        htmlDocument,
                                        myaccount_energy);
                    // Should now be going back to PROCRESOURCE
                    break;
            }
            return true;
        }

        //private static T ConvertJsonToClass<T>(this string json)
        //{
        //    System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
        //    return serializer.Deserialize<T>(json);
        //}


        private static async Task<bool> FU_Login(
#if ANDROIDX
                                        AppCompatActivity meterActivity,
#endif
                                        MainViewModel ourviewmodel,
                                        UtilityViewModel utilityviewmodel,
                                        HtmlAgilityPack.HtmlDocument document,
                                        bool TextBox_Active)
        {
            // Reset this
            IList<HtmlAgilityPack.HtmlNode> HtmlCol;
            string name;// = "";
            bool clicked = false;

            HtmlCol = YetAnotherFuckingHoop.SelectNodesAsList(document.DocumentNode, "//input");
            foreach (HtmlAgilityPack.HtmlNode element in HtmlCol)
            {
                if (!string.IsNullOrEmpty(element.Id))
                {
                    switch (element.Id)
                    {
                        case "LogonForm_uid":
                            // Set the user name in the username text box
                            name = element.GetAttributeValue("name", "");
                            utilityviewmodel.keyValues.Add(new KeyValuePair<string, string>(name, utilityviewmodel.user_id));
                            if (TextBox_Active)
                            {
                                await SmartRoutinesV2018.TextBlockUpdate(
#if ANDROIDX
                                                                meterActivity,
#endif
                                                                ourviewmodel,
                                                                "UserId" +
                                                                SmartParametersV2016.space +
                                                                utilityviewmodel.user_id +
                                                                Environment.NewLine.ToString());
                            }
                            break;
                        case "LogonForm_pwd":
                            //Type the password in the password text box
                            name = element.GetAttributeValue("name", "");
                            utilityviewmodel.keyValues.Add(new KeyValuePair<string, string>(name, utilityviewmodel.password));
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
                                                               "Asterisks" +
                                                               Environment.NewLine.ToString());
                            }
                            utilityviewmodel.keyValues.Add(new KeyValuePair<string, string>("LogonForm[remember]", "0"));
                            utilityviewmodel.keyValues.Add(new KeyValuePair<string, string>("logon", "Log in"));

                            utilityviewmodel.login_attempted = true;

                            if (TextBox_Active)
                            {
                                await SmartRoutinesV2018.TextBlockUpdate(
#if ANDROIDX
                                                                meterActivity,
#endif
                                                                ourviewmodel,
                                                                "LogOnAttempted" +
                                                                Environment.NewLine.ToString());
                            }

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

            // If our login was successful a 'secure' Document
            // will be delivered and we have told the handler
            // to call the SecureLogin routine below when that
            // happens
            // If our login was UNSUCCESSFUL, then a 'Standard Screen'
            // Document will STILL be delivered and we have told the handler
            // to call the routine below when that happens.  So we
            // can't tell the difference between a good login and a bad
            // login until we try and get the Account No from the page
            // delivered.  If we don't get ANYTHING, then the timer will
            // tick away the 60 seconds and exit the timer loop

            // We didn't find any Login info to get to grips with...
            return clicked;
        }

        // Fuck me!  It fucking WORKED!!!! See the re-direction to this 'secure'
        // routine from the initial fucking Login routine above!!!!!!!!!!!!
        // You ARE a fucking genius Ray!!  An absolute bravest of the brave
        // fucking genius.  Those visionless cunts you saw last Thursday have
        // ABSOLUTELY no idea what they are missing out on ....

        // WHERE ARE WE ???

        ///  The FU cunts have FU-ed ME big time as some of
        ///  the elements I use to 'Click' on have now got class=disabled
        ///  so the link can't be used. I COULD Navigate straight to the
        ///  page I want, but I can't seem to see the navigate link on
        ///  the page I am looking at .  I will re-visit these cunts next
        ///  week when I have finished contemplating suicide ...
        ///  Maybe its just a cock-up on their side to disable links??
        // 13-Oct-2012
        // No it isn't - they mark the class as 'disabled' whilst their "lazy loader"
        // shit gets its act together. Once this garbage has finished
        // THEn they refresh the page and the link is enabled (i.e. the class is no
        // longer 'disabled'.  So what I have to do is not go straight from
        // Login to Utility.Accounts Overview, I have to do this one here and Navigate to
        // Manage Utility.Accounts which should give me a refreshed age and THEn go to
        // Utility.Accounts Overview.  What a Bag of Shit!!  How CAN these fuckers jepoardize
        // 18 months of hard work with their shit??  How can they???
        //
        // In all EIGHTEEN MONTHS this is the **ONLY** solution that ever worked first time
        // The ONLY ONE
        //
        // Unhook THIS routine for the next 'Document Completed' delivery 
        //wb.DocumentCompleted -= new WebBrowserDocumentCompletedEventHandler(FU_DocumentCompleted_ManageAccount);
        //// Hook up the next routine - if the login is true - for the next 'Document Completed' delivery
        //wb.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(FU_DocumentCompleted_Utility.AccountsOverview);
        //wb.Navigate("https://www.first-utility.com/portal/me/account");



        //private static async Task<bool> FU_AccountOverview(
        //    MainViewModel ourviewmodel,
        //    UtilityViewModel utilityviewmodel,
        //    HtmlAgilityPack.HtmlDocument document)
        //    {
        //        bool clicked = false;

        //#if WINFORMS
        //    if (utilityviewmodel.console)
        //    {
        //        await SmartRoutinesV2018.TextBlockUpdate(
        //            ourviewmodel, 
        //            "Fuel List in: " + utilityviewmodel.fuelList + Environment.NewLine);
        //    }
        //    clicked = await Find_Account_No(ourviewmodel, document, utilityviewmodel);
        //#endif
        //#if WPF  || ANDROIDX
        //        clicked = Find_Account_No(ourviewmodel, document, utilityviewmodel);
        //#endif

        //        if (!clicked)
        //        {
        //            ourviewmodel.errorMessage = "Cannot determine account number";
        //            return false;
        //        }
        //        else
        //        {
        //            var payload = new
        //            {
        //                jsonrpc = "2.0",
        //                method = "customersTariffInformationLabels",
        //                @params = new[] { Convert.ToInt32(utilityviewmodel.account_no) },
        //                id = 4
        //            };

        //            string jsonString = JsonSerializer.Serialize(payload);
        //            utilityviewmodel.target_pathname = "json-rpc";

        //            string answer = await SmartBobV2017.Scraper_Generic_Post_Json(
        //                ourviewmodel,
        //                utilityviewmodel.utilityToken,
        //                jsonString,
        //                utilityviewmodel,
        //                "");

        //            if (!string.IsNullOrEmpty(ourviewmodel.errorMessage))
        //                return false;

        //            // --- Deserialize with System.Text.Json ---
        //            using JsonDocument jsonDoc = JsonDocument.Parse(answer);
        //            var tariffs = jsonDoc.RootElement
        //                                  .GetProperty("result")
        //                                  .GetProperty("data")
        //                                  .GetProperty("tariffs");

        //            foreach (var resource in tariffs.EnumerateArray())
        //            {
        //                foreach (var target in resource.GetProperty("fuels").EnumerateArray())
        //                {
        //#if WINFORMS
        //                if (utilityviewmodel.console)
        //                {
        //                    await SmartRoutinesV2018.TextBlockUpdate(
        //                        ourviewmodel, 
        //                        "Actual Fuel " + target.GetProperty("fuel").GetString() + Environment.NewLine);
        //                }
        //#endif
        //                    string this_un = target.GetProperty("fuel").GetString();
        //                    if (!string.IsNullOrEmpty(this_un))
        //                    {
        //                        char resource_code = Convert.ToChar(this_un.ToUpper());
        //                        SmartParseV2016.Update_FuelList(
        //                            SmartParametersV2016.Utility,
        //                            resource_code,
        //                            utilityviewmodel.brand_code,
        //                            utilityviewmodel.Hezbollah.supplier_typesList,
        //                            utilityviewmodel);
        //                    }
        //                }

        //                utilityviewmodel.TARIFF_NAME = resource.GetProperty("name").GetString()!
        //                                                  .Replace("ebill", "", StringComparison.OrdinalIgnoreCase)
        //                                                  .Trim();

        //                string PAYMENT_TYPE = resource.GetProperty("paymentMethod").GetString()!;
        //                SmartSpikeUtilityV2017.Utility_Lookup_PaymentPlan(
        //                    ourviewmodel,
        //                    utilityviewmodel,
        //                    PAYMENT_TYPE);
        //            }
        //        }

        //        if (string.IsNullOrEmpty(utilityviewmodel.logout_pathname))
        //        {
        //#if WINFORMS
        //        await SmartNibbyV2016.Find_A_Tag(ourviewmodel,
        //#endif
        //#if WPF  || WINUI || ANDROIDX
        //            SmartNibbyV2016.Find_A_Tag(ourviewmodel,
        //#endif
        //                document,
        //                utilityviewmodel,
        //                "//a",
        //                true,
        //                "",
        //                "Log out",
        //                "");
        //        }

        //        utilityviewmodel.next_routine = clicked ? "PROCRESOURCE" : "LOGOUT";
        //        if (clicked)
        //            utilityviewmodel.target_pathname = "";

        //#if WINFORMS
        //    if (utilityviewmodel.console)
        //    {
        //        await SmartRoutinesV2018.TextBlockUpdate(
        //            ourviewmodel, 
        //            "Fuel List out: " + utilityviewmodel.fuelList + Environment.NewLine);
        //    }
        //#endif

        //        return clicked;
        //    }

        [RequiresUnreferencedCode("Calls System.Text.Json.JsonSerializer.Serialize<TValue>(TValue, JsonSerializerOptions)")]
        private static async Task<bool> FU_AccountOverview(
        MainViewModel ourviewmodel,
        UtilityViewModel utilityviewmodel,
        HtmlAgilityPack.HtmlDocument document)
        {
            bool clicked = false;

#if WINFORMS
        if (utilityviewmodel.console)
        {
            await SmartRoutinesV2018.TextBlockUpdate(
                ourviewmodel,
                "Fuel List in: " + utilityviewmodel.fuelList + Environment.NewLine);
        }
        clicked = await Find_Account_No(ourviewmodel, document, utilityviewmodel);
#endif
#if WPF  || ANDROIDX || SMARTMAUI
            clicked = Find_Account_No(ourviewmodel, document, utilityviewmodel);
    #endif

            if (!clicked)
            {
                ourviewmodel.errorMessage = "Cannot determine account number";
                return false;
            }

            // --- Build payload and serialize with System.Text.Json ---
            var payload = new
            {
                jsonrpc = "2.0",
                method = "customersTariffInformationLabels",
                @params = new[] { Convert.ToInt32(utilityviewmodel.account_no) },
                id = 4
            };

            string jsonString = JsonSerializer.Serialize(payload);
            utilityviewmodel.target_pathname = "json-rpc";

            string answer = await SmartBobV2017.Scraper_Generic_Post_Json(
                ourviewmodel,
                utilityviewmodel.utilityToken,
                jsonString,
                utilityviewmodel,
                "");

            if (!string.IsNullOrEmpty(ourviewmodel.errorMessage))
                return false;

            // --- Deserialize JSON using System.Text.Json ---
            using JsonDocument jsonDoc = JsonDocument.Parse(answer);
            JsonElement tariffs = jsonDoc.RootElement
                                         .GetProperty("result")
                                         .GetProperty("data")
                                         .GetProperty("tariffs");

            foreach (JsonElement resource in tariffs.EnumerateArray())
            {
                foreach (JsonElement target in resource.GetProperty("fuels").EnumerateArray())
                {
    #if WINFORMS
                if (utilityviewmodel.console)
                {
                    await SmartRoutinesV2018.TextBlockUpdate(
                        ourviewmodel,
                        "Actual Fuel " + target.GetProperty("fuel").GetString() + Environment.NewLine);
                }
    #endif
                    string this_un = target.GetProperty("fuel").GetString();
                    if (!string.IsNullOrEmpty(this_un))
                    {
                        char resource_code = Convert.ToChar(this_un.ToUpper());
                        SmartParseV2016.Update_FuelList(
                            SmartParametersV2016.Utility,
                            resource_code,
                            utilityviewmodel.brand_code,
                            utilityviewmodel.Hezbollah.supplier_typesList,
                            utilityviewmodel);
                    }
                }

                utilityviewmodel.TARIFF_NAME = resource.GetProperty("name").GetString()!
                    .Replace("ebill", "", StringComparison.OrdinalIgnoreCase)
                    .Trim();

                string PAYMENT_TYPE = resource.GetProperty("paymentMethod").GetString()!;
                SmartSpikeUtilityV2017.Utility_Lookup_PaymentPlan(
                    ourviewmodel,
                    utilityviewmodel,
                    PAYMENT_TYPE);
            }

            if (string.IsNullOrEmpty(utilityviewmodel.logout_pathname))
            {
#if WINFORMS
            await SmartNibbyV2016.Find_A_Tag(ourviewmodel,
#endif
#if WPF  || WINUI || ANDROIDX || SMARTMAUI
                SmartNibbyV2016.Find_A_Tag(ourviewmodel,
    #endif
                    document,
                    utilityviewmodel,
                    "//a",
                    true,
                    "",
                    "Log out",
                    "");
            }

            utilityviewmodel.next_routine = clicked ? "PROCRESOURCE" : "LOGOUT";
            if (clicked)
                utilityviewmodel.target_pathname = "";

    #if WINFORMS
        if (utilityviewmodel.console)
        {
            await SmartRoutinesV2018.TextBlockUpdate(
                ourviewmodel,
                "Fuel List out: " + utilityviewmodel.fuelList + Environment.NewLine);
        }
    #endif

            return clicked;
        }

#if WINFORMS
        internal static async Task<bool> Find_Account_No(MainViewModel ourviewmodel,
#endif
#if WPF  || WINUI || SMARTMAUI
        internal static bool Find_Account_No(MainViewModel ourviewmodel,
#endif
#if ANDROIDX
        internal static bool Find_Account_No(MainViewModel ourviewmodel,
#endif
        HtmlAgilityPack.HtmlDocument document,
                                            UtilityViewModel utilityviewmodel)
        {
            IList<HtmlAgilityPack.HtmlNode>
                        HtmlCol1;
            //HtmlCol2;

            string Account_Number = "Your Account Number:";
            bool clicked = false;
            HtmlCol1 = YetAnotherFuckingHoop.SelectNodesAsList(document.DocumentNode, "//p");
            foreach (HtmlAgilityPack.HtmlNode element1 in HtmlCol1)
            {
                if (string.IsNullOrEmpty(element1.Id))
                {
                    if (!string.IsNullOrEmpty(element1.InnerText))
                    {
                        if (element1.InnerText.IndexOf(Account_Number) >= 0)
                        {
                            utilityviewmodel.account_no = element1.InnerText.Replace(Account_Number, "").Trim();
                            //HtmlCol2 = YetAnotherFuckingHoop.SelectNodesAsList(element1, "//strong");
                            //foreach (HtmlAgilityPack.HtmlNode element2 in HtmlCol2)
                            //{
                            //    string classname = element2.GetAttributeValue("class", "");
                            //    if (classname == "userId")
                            //    {
                            //        if (!string.IsNullOrEmpty(element2.InnerText))
                            //        {
                            //            utilityviewmodel.account_no = element2.InnerText.Trim();

#if WINFORMS
                            if (utilityviewmodel.console)
                            {
                                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, Account_Number + SmartParametersV2016.space + utilityviewmodel.account_no + Environment.NewLine.ToString());
                            }
#endif
                            clicked = true;
                            break;
                        }
                    }
                }
                if (clicked)
                {
                    break;
                }
            }
            return clicked;
        }

#if WINFORMS
        private static async Task<bool> FU_Process_Resource(MainViewModel ourviewmodel,
#endif
#if WPF  || WINUI || SMARTMAUI
        private static bool FU_Process_Resource(MainViewModel ourviewmodel,
#endif
#if ANDROIDX
        private static bool FU_Process_Resource(MainViewModel ourviewmodel,
#endif
        UtilityViewModel utilityviewmodel)

        {
            utilityviewmodel.update_account = false;

            bool clicked = false;
#if WINFORMS
            if (utilityviewmodel.console)
            {
                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Fuel List in: " + utilityviewmodel.fuelList + Environment.NewLine.ToString());
            }
#endif
            if (string.IsNullOrEmpty(utilityviewmodel.fuelList))
            {
                utilityviewmodel.resource_code = SmartParametersV2016.defaultResourceCode;
            }
            else
            {
                string resource_type = "";
                if (SmartParseV2016.Strip_FuelList(utilityviewmodel))
                {
                    SmartUtilityV2022.Fix_Resource_Type(utilityviewmodel, resource_type);
                    utilityviewmodel.found_resource = true;

                    utilityviewmodel.target_pathname = "";
                    clicked = true;
                }
                //    // Don't click it if its already been done
                //    List<Amelia> amelia_found = SmartUtilityV2022.Amelia_Lookup(utilityviewmodel, false);
                //    if (amelia_found.Count == 0)
                //    {
                //        SmartUtilityV2022.Amelia_Add(utilityviewmodel);
                //        // Now - we might never know the MPAN if its a new account and
                //        // we don't have any information or Bills to update it
                //        // But we DO have an account so we need to check:
                //        // a) if its already known in the Utility.Accounts_In list
                //        // b) if it isn't then b) check if its in the Utility.Accounts_Out list
                //        // If it isn't then add it into Utility.Accounts_Out
                //        // ...and!  Set the MPAN if it goes in as 'Unknown' to something we have
                //        SmartUtilityV2022.Check_Everything(utilityviewmodel, Hamas);

                //        // Other bits and pieces
                //        utilityviewmodel.TARIFF_NAME = "";
                //        utilityviewmodel.TARIFF_CODE = 0;
                //        utilityviewmodel.statement_id = "";

                //        // No - clear this down
                //        utilityviewmodel.target_pathname = "";
                //        clicked = true;
                //    }
                //}
            }

            if (!clicked)
            {
                utilityviewmodel.next_routine = "LOGOUT";
            }
            else
            {
                utilityviewmodel.next_routine = "BASICDETAILS";
            }
#if WINFORMS
            if (utilityviewmodel.console)
            {
                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Processing: " + utilityviewmodel.resource_code + Environment.NewLine.ToString());
                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Fuel List out: " + utilityviewmodel.fuelList + Environment.NewLine.ToString());
            }
#endif
            return clicked;
        }


        //    private static async Task<bool> FU_BasicDetails(
        //                                MainViewModel ourviewmodel,
        //                                UtilityViewModel utilityviewmodel,
        //                                HtmlAgilityPack.HtmlDocument document)
        //    {
        //        bool clicked = false;

        //        if (!string.IsNullOrEmpty(utilityviewmodel.account_no))
        //        {
        //            // Build JSON request
        //            var payload = new
        //            {
        //                jsonrpc = "2.0",
        //                method = "customersDetails",
        //                @params = new[] { Convert.ToInt32(utilityviewmodel.account_no) },
        //                id = 3
        //            };

        //            string jsonString = JsonSerializer.Serialize(payload);
        //            utilityviewmodel.target_pathname = "json-rpc";

        //            // Call API
        //            string answer = await SmartBobV2017.Scraper_Generic_Post_Json(
        //                ourviewmodel,
        //                utilityviewmodel.utilityToken,
        //                jsonString,
        //                utilityviewmodel,
        //                "");

        //            if (!string.IsNullOrEmpty(ourviewmodel.errorMessage))
        //            {
        //                return false;
        //            }

        //            // --- Parse response using System.Text.Json ---
        //            using JsonDocument jsonDoc = JsonDocument.Parse(answer);
        //            JsonElement data = jsonDoc.RootElement
        //                              .GetProperty("result")
        //                              .GetProperty("data");

        //#if WINFORMS
        //        if (utilityviewmodel.console)
        //        {
        //            await SmartRoutinesV2018.TextBlockUpdate(
        //                ourviewmodel, "Name " + data.GetProperty("fullName").GetString() + Environment.NewLine);
        //            await SmartRoutinesV2018.TextBlockUpdate(
        //                ourviewmodel, "Phone " + data.GetProperty("homeTelephoneNumber").GetString() + Environment.NewLine);
        //            await SmartRoutinesV2018.TextBlockUpdate(
        //                ourviewmodel, "Email " + data.GetProperty("primaryEmailAddress").GetString() + Environment.NewLine);
        //        }
        //#endif

        //            clicked = await Decode_Json_Response(ourviewmodel, utilityviewmodel, data);
        //        }

        //        if (clicked)
        //        {
        //#if WINFORMS
        //        clicked = await SmartNibbyV2016.Find_A_Tag(ourviewmodel,
        //#endif
        //#if WPF  || WINUI
        //        clicked = SmartNibbyV2016.Find_A_Tag(ourviewmodel,
        //#endif
        //#if ANDROIDX
        //            clicked = SmartNibbyV2016.Find_A_Tag(ourviewmodel,
        //#endif
        //                document,
        //                utilityviewmodel,
        //                ".//a",          // Tag to look for
        //                true,            // Only consider null elements
        //                "",              // Remove from inner text
        //                "Account summary", // inner_text_comparison
        //                "ACCSUMMARY");   // next routine

        //            utilityviewmodel.target_pathname = "/myaccount/summary";
        //            utilityviewmodel.next_routine = "ACCSUMMARY";
        //            clicked = true;
        //        }

        //        if (!clicked)
        //        {
        //            utilityviewmodel.next_routine = "LOGOUT";
        //        }

        //        return clicked;
        //    }

        [RequiresUnreferencedCode("Calls System.Text.Json.JsonSerializer.Serialize<TValue>(TValue, JsonSerializerOptions)")]
        private static async Task<bool> FU_BasicDetails(
    MainViewModel ourviewmodel,
    UtilityViewModel utilityviewmodel,
    HtmlAgilityPack.HtmlDocument document)
        {
            bool clicked = false;

            if (!string.IsNullOrEmpty(utilityviewmodel.account_no))
            {
                // Build JSON request
                var payload = new
                {
                    jsonrpc = "2.0",
                    method = "customersDetails",
                    @params = new[] { Convert.ToInt32(utilityviewmodel.account_no) },
                    id = 3
                };

                string jsonString = JsonSerializer.Serialize(payload);
                utilityviewmodel.target_pathname = "json-rpc";

                // Call API
                string answer = await SmartBobV2017.Scraper_Generic_Post_Json(
                    ourviewmodel,
                    utilityviewmodel.utilityToken,
                    jsonString,
                    utilityviewmodel,
                    "");

                if (!string.IsNullOrEmpty(ourviewmodel.errorMessage))
                {
                    return false;
                }

                // --- Parse response using System.Text.Json ---
                using JsonDocument jsonDoc = JsonDocument.Parse(answer);
                JsonElement data = jsonDoc.RootElement
                                          .GetProperty("result")
                                          .GetProperty("data");

#if WINFORMS
        if (utilityviewmodel.console)
        {
            await SmartRoutinesV2018.TextBlockUpdate(
                ourviewmodel, "Name " + data.GetProperty("fullName").GetString() + Environment.NewLine);
            await SmartRoutinesV2018.TextBlockUpdate(
                ourviewmodel, "Phone " + data.GetProperty("homeTelephoneNumber").GetString() + Environment.NewLine);
            await SmartRoutinesV2018.TextBlockUpdate(
                ourviewmodel, "Email " + data.GetProperty("primaryEmailAddress").GetString() + Environment.NewLine);
        }
#endif

                // Call Decode_Json_Response using JsonElement
                clicked = await Decode_Json_Response(ourviewmodel, utilityviewmodel, data);
            }

            if (clicked)
            {
#if WINFORMS
                clicked = await SmartNibbyV2016.Find_A_Tag(
#elif WPF  || WINUI || SMARTMAUI
                clicked = SmartNibbyV2016.Find_A_Tag(
#elif ANDROIDX
                clicked = SmartNibbyV2016.Find_A_Tag(
#endif
                    ourviewmodel,
                    document,
                    utilityviewmodel,
                    ".//a",          // Tag to look for
                    true,            // Only consider null elements
                    "",              // Remove from inner text
                    "Account summary", // inner_text_comparison
                    "ACCSUMMARY");   // next routine

                utilityviewmodel.target_pathname = "/myaccount/summary";
                utilityviewmodel.next_routine = "ACCSUMMARY";
                clicked = true;
            }

            if (!clicked)
            {
                utilityviewmodel.next_routine = "LOGOUT";
            }

            return clicked;
        }
        
        private static async Task<bool> Decode_Json_Response(
                        MainViewModel ourviewmodel,
                        UtilityViewModel utilityviewmodel,
                        JsonElement data) // Accept JsonElement directly
        {
            utilityviewmodel.account_name = data.GetProperty("fullName").GetString() ?? "";
            utilityviewmodel.account_phone_no = data.GetProperty("homeTelephoneNumber").GetString() ?? "";
            utilityviewmodel.account_email = data.GetProperty("primaryEmailAddress").GetString() ?? "";
            utilityviewmodel.account_dob = "";

            foreach (JsonElement address in data.GetProperty("addresses").EnumerateArray())
            {
                string this_un = address.GetProperty("addressType").GetString() ?? "";
                if (await Determine_Address(ourviewmodel, utilityviewmodel, address, this_un))
                {
                    return true;
                }
            }

            return false;
        }
        
        private static async Task<bool> Determine_Address(
                                MainViewModel ourviewmodel,
                                UtilityViewModel utilityviewmodel,
                                System.Text.Json.JsonElement address,
                                string this_un)
        {
            string account_address = "";

            SmartUtility.Accounts account_row = new SmartUtility.Accounts();

            if (this_un == "CONTACT")
            {
                // Updated to use JsonElement
                account_address = Fix_The_Address(address, account_address);

                string account_postcode = address.GetProperty("postcode").GetString() ?? "";

#if WINFORMS
        if (utilityviewmodel.console)
        {
            await SmartRoutinesV2018.TextBlockUpdate(
                ourviewmodel, 
                "Contact Address: " + account_address + Environment.NewLine);

            await SmartRoutinesV2018.TextBlockUpdate(
                ourviewmodel, 
                "Contact Postcode: " + account_postcode + Environment.NewLine);
        }
#endif

                if (SmartNibbyV2016.Derive_Postcode_New(
                        account_postcode,
                        ourviewmodel.Blanche.workingPostcodesList,
                        utilityviewmodel))
                {
                    SmartUtilityV2022.Set_UDPRN_MPAN_MPRN(utilityviewmodel);

                    // If we don't have a UDPRN or the MPAN_MPRN is empty,
                    // then go and look it up.
                    if (string.IsNullOrEmpty(utilityviewmodel.mpan_mprn))
                    {
                        if (!await SmartUtilityV2022.Determine_UDPRNZ(
                                ourviewmodel,
                                utilityviewmodel,
                                account_address))
                        {
                            return false;
                        }
                    }

                    utilityviewmodel.login_finished = true;

                    if (utilityviewmodel.TARIFF_CODE == 0)
                    {
                        await SmartUtilityV2022.Lookup_Remote_Tariff_Code(
                            ourviewmodel,
                            utilityviewmodel);
                    }

                    return true;
                }
            }

            return false;
        }
        
        private static string Fix_The_Address(JsonElement address, string account_address)
        {
            // Loop through each line
            for (int i = 1; i <= 9; i++)
            {
                string lineName = $"addressLine{i}";
                if (address.TryGetProperty(lineName, out JsonElement lineElement))
                {
                    string lineValue = lineElement.GetString() ?? "";
                    account_address = FU_Address(lineValue, account_address);
                }
            }

            account_address = account_address.Trim(SmartParametersV2016.bar);
            return account_address;
        }

        private static string FU_Address(string address_line, string account_address)
        {
            if (!string.IsNullOrEmpty(address_line))
            {
                account_address += address_line + SmartParametersV2016.space;
            }
            return account_address;
        }
        private static bool FU_Accountsummary(UtilityViewModel utilityviewmodel,
                                                HtmlAgilityPack.HtmlDocument document)
        {
            // Fuck me!  It fucking WORKED!!!! See the re-direction to this 'secure'
            // routine from the initial fucking Login routine above!!!!!!!!!!!!
            // You ARE a fucking genius Ray!!  An absolute bravest of the brave
            // fucking genius.  Those visionless cunts you saw last Thursday have
            // ABSOLUTELY no idea what they are missing out on .... 

            // If its already been done then click on the View Usage button and
            // come back to this screen ... so we cycle round here a few times
            IList<HtmlAgilityPack.HtmlNode>
                        HtmlCol1;
            bool clicked = false;

            utilityviewmodel.graph_electricity_pathname = "";
            //graph_gas_pathname = "";
            utilityviewmodel.bills_pathname = "";
            utilityviewmodel.payments_pathname = "";

            utilityviewmodel.target_pathname = "";

            // Go and set the ViewBills and ViewPayments)
            HtmlCol1 = YetAnotherFuckingHoop.SelectNodesAsList(document.DocumentNode, "//a");
            foreach (HtmlAgilityPack.HtmlNode element1 in HtmlCol1)
            {
                string href = element1.GetAttributeValue(SmartParametersV2016.href, "");
                if (!string.IsNullOrEmpty(href))
                {
                    if (href.IndexOf("/myaccount/graph/electricity") >= 0)
                    {
                        utilityviewmodel.graph_electricity_pathname = href;
                        switch (utilityviewmodel.resource_code)
                        {
                            case SmartParametersV2016.Electricity:
                                if (string.IsNullOrEmpty(utilityviewmodel.target_pathname))
                                {
                                    if (!string.IsNullOrEmpty(utilityviewmodel.graph_electricity_pathname))
                                    {
                                        utilityviewmodel.target_pathname = utilityviewmodel.graph_electricity_pathname;
                                        // Hook up the next routine for the next delivery
                                        utilityviewmodel.next_routine = "USAGE";
                                        clicked = true;
                                    }
                                }
                                break;
                            case SmartParametersV2016.Gas:
                                if (string.IsNullOrEmpty(utilityviewmodel.target_pathname))
                                {
                                    if (!string.IsNullOrEmpty(utilityviewmodel.graph_gas_pathname))
                                    {
                                        utilityviewmodel.target_pathname = utilityviewmodel.graph_gas_pathname;
                                        // Hook up the next routine for the next delivery
                                        utilityviewmodel.next_routine = "USAGE";
                                        clicked = true;
                                    }
                                }
                                break;
                            default:
                                break;
                        }
                    }
                    if (href.IndexOf("/myaccount/bills/viewbills") >= 0)
                    {
                        utilityviewmodel.bills_pathname = href;
                        if (string.IsNullOrEmpty(utilityviewmodel.target_pathname))
                        {
                            utilityviewmodel.target_pathname = href;
                            // Hook up the next routine for the next delivery
                            utilityviewmodel.next_routine = "VIEWBILLS";
                            clicked = true;
                        }
                    }
                    if (href.IndexOf("/myaccount/details") >= 0)
                    {
                        utilityviewmodel.details_pathname = href;
                        if (string.IsNullOrEmpty(utilityviewmodel.target_pathname))
                        {
                            utilityviewmodel.target_pathname = href;
                            // Hook up the next routine for the next delivery
                            utilityviewmodel.next_routine = "ACCDETAILS";
                            clicked = true;
                        }
                    }
                    if (href.IndexOf("/myaccount/viewpayments") >= 0)
                    {
                        utilityviewmodel.payments_pathname = href;
                        if (string.IsNullOrEmpty(utilityviewmodel.target_pathname))
                        {
                            utilityviewmodel.target_pathname = href;
                            // Hook up the next routine for the next delivery
                            utilityviewmodel.next_routine = "VIEWPAYMENTS";
                            clicked = true;
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

        private static bool FU_AccountDetails(UtilityViewModel utilityviewmodel,
                                                HtmlAgilityPack.HtmlDocument document)
        {
            // Fuck me!  It fucking WORKED!!!! See the re-direction to this 'secure'
            // routine from the initial fucking Login routine above!!!!!!!!!!!!
            // You ARE a fucking genius Ray!!  An absolute bravest of the brave
            // fucking genius.  Those visionless cunts you saw last Thursday have
            // ABSOLUTELY no idea what they are missing out on ....
            // UtilityViewModel utilityviewmodel = getMES();

            // If its already been done then click on the View Usage button and
            // come back to this screen ... so we cycle round here a few times

            // If there were no 'View whatever buttons' then assume it is
            // Electricity only showing because FU don't do Gas only

            // Assume FU ALWAYS check for Electricity because I don't think
            // you can have FU with ONLY Gas - you can have Electricity only
            // or Electricity and Gas (so I am led to believe)

            // Do it the fucking hard way!
            // NO!  Now we do it the EASy way!!!

            IList<HtmlAgilityPack.HtmlNode>
                        HtmlCol1;

            bool clicked = false;

            // Go and set the ViewBills and ViewPayments)
            HtmlCol1 = YetAnotherFuckingHoop.SelectNodesAsList(document.DocumentNode, "//input");
            foreach (HtmlAgilityPack.HtmlNode element1 in HtmlCol1)
            {
                string name = element1.GetAttributeValue("name", "");
                if (!string.IsNullOrEmpty(name))
                {
                    string value = element1.GetAttributeValue("value", "");
                    switch (name)
                    {
                        case "BankDetailsForm[bankAccountNumber]":
                            utilityviewmodel.bank_account_number = value;
                            break;
                        case "BankDetailsForm[bankSortCode]":
                            utilityviewmodel.bank_sort_code = value;
                            break;
                        case "BankDetailsForm[bankAccountHolder]":
                            utilityviewmodel.bank_account_name = value;
                            break;
                        default:
                            break;
                    }
                    if (!string.IsNullOrEmpty(utilityviewmodel.bank_account_number) &&
                        !string.IsNullOrEmpty(utilityviewmodel.bank_sort_code) &&
                        !string.IsNullOrEmpty(utilityviewmodel.bank_account_name))
                    {
                        utilityviewmodel.bank_payment_day = Convert.ToInt16(1);
                        clicked = true;
                        break;
                    }
                }
            }

            if (clicked)
            {
                //SmartUtilityV2022.Utility_Check_Everything(utilityviewmodel, ourviewmodel, utilityviewmodel);
            }

            utilityviewmodel.next_routine = "VIEWBILLS"; // But at what path??
            utilityviewmodel.target_pathname = utilityviewmodel.bills_pathname;   // <= This one

            return clicked;
        }

        private static async Task<bool> FU_Usage(MainViewModel ourviewmodel,
                                                UtilityViewModel utilityviewmodel,
                                                HtmlAgilityPack.HtmlDocument document,
                                                string bills_pathname,
                                                string details_pathname)
        {
            // Fuck me!  It fucking WORKED!!!! See the re-direction to this 'secure'
            // routine from the initial fucking Login routine above!!!!!!!!!!!!
            // You ARE a fucking genius Ray!!  An absolute bravest of the brave
            // fucking genius.  Those visionless cunts you saw last Thursday have
            // ABSOLUTELY no idea what they are missing out on ....

            // If its already been done then click on the View Usage button and
            // come back to this screen ... so we cycle round here a few times
            IList<HtmlAgilityPack.HtmlNode>
                        HtmlCol1,
                        HtmlCol2;
            string interval = "monthly";

            if (utilityviewmodel.resource_code == SmartParametersV2016.Electricity)
            {
                HtmlCol1 = YetAnotherFuckingHoop.SelectNodesAsList(document.DocumentNode, "//a");
                foreach (HtmlAgilityPack.HtmlNode element1 in HtmlCol1)
                {
                    string href = element1.GetAttributeValue(SmartParametersV2016.href, "");
                    if (!string.IsNullOrEmpty(href))
                    {
                        if (href.IndexOf("/myaccount/graph/gas") >= 0)
                        {
                            utilityviewmodel.graph_gas_pathname = href;
                            break;
                        }
                    }
                }
            }

            // Go and do the csv download
            HtmlCol1 = YetAnotherFuckingHoop.SelectNodesAsList(document.DocumentNode, "//div");
            foreach (HtmlAgilityPack.HtmlNode element1 in HtmlCol1)
            {
                if (!string.IsNullOrEmpty(element1.Id))
                {
                    if (element1.Id.IndexOf("dl-usage-data") >= 0)
                    {
                        HtmlCol2 = YetAnotherFuckingHoop.SelectNodesAsList(element1, ".//a");
                        foreach (HtmlAgilityPack.HtmlNode element2 in HtmlCol2)
                        {
                            string data_href = element2.GetAttributeValue("data-href", "");
                            if (data_href.IndexOf("/myaccount/down/usage") >= 0)
                            {
                                // Get rid of the preamble
                                if (!await Do_The_Downloads(ourviewmodel,
                                                        utilityviewmodel,
                                                        data_href,
                                                        interval))

                                {
                                    // We failed to download some Usage data ...
                                    if (!string.IsNullOrEmpty(ourviewmodel.errorMessage))
                                    {
                                        return false;
                                    }
                                    // However! Sometimes the FU Foreigners just don't fucking show the usage data on the page!
                                    // therfore, we have no choice but to give up ...
                                    goto like_old_days;
                                }
                                break;
                            }
                        }
                    }
                }
            }

        // Go and do the Bills and Payments)
        like_old_days:
            // Paki Fuckers keep changing it
            // Hook up the next routine for the next delivery
            utilityviewmodel.next_routine = "VIEWBILLS"; // But at what path??
            utilityviewmodel.target_pathname = bills_pathname;   // <= This one
            utilityviewmodel.next_routine = "ACCDETAILS"; // But at what path??
            utilityviewmodel.target_pathname = details_pathname;   // <= This one
            return true;
        }

        private static async Task<bool> Do_The_Downloads(MainViewModel ourviewmodel,
                                                        UtilityViewModel utilityviewmodel,
                                                        string data_href,
                                                        string interval)
        {
            // Get rid of the preamble
            string target_string = utilityviewmodel.prfix_xxx + data_href;
            target_string += "?accountId=" + utilityviewmodel.account_no + "&interval=";
            target_string += interval;
            target_string += "&type=";

            utilityviewmodel.resource_description = "";
            if (SmartSpikeUtilityV2017.Utility_Lookup_ResourceDescription(ourviewmodel,
                                                                utilityviewmodel))
            {
                target_string += utilityviewmodel.resource_description.ToLower();
                // Local time
                string this_year = (DateTime.Now + ourviewmodel.utcOffset).Year.ToString(); // Local time
                target_string = target_string + "&enddate=" + this_year + "-12-31" +
                                                "&startdate=2010-01-01";

                Uri TargetUrl = new Uri(target_string);
                // Get the fucking File

                string[] task = await SmartBobV2017.HTTPCLIENT_GET_FILE_ASYNC(ourviewmodel,
                                                                                TargetUrl,
                                                                                utilityviewmodel.utilityToken,
                                                                                utilityviewmodel.guid);

                if (task.Length == 0 ||
                    !string.IsNullOrEmpty(ourviewmodel.errorMessage))
                {
                    return false;
                }
                else
                {
                    if (task.ToArray().Length != 0)
                    {
                        if (await Do_The_Updates(ourviewmodel,
                                                    utilityviewmodel,
                                                    task))
                        {
#if WINFORMS
                            if (utilityviewmodel.console)
                            {
                                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Updated usage: " + task.ToArray().Length + Environment.NewLine.ToString());
                            }
#endif
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        private static async Task<bool> Do_The_Updates(MainViewModel ourviewmodel,
                                                        UtilityViewModel utilityviewmodel,
                                                        string[] task)
        {
            switch (utilityviewmodel.resource_code)
            {
                case SmartParametersV2016.Electricity:

                    if (!await Store_Electricity_Readings(ourviewmodel,
                                            utilityviewmodel,
                                            task.ToArray(),
                                            utilityviewmodel.brand_code,
                                            utilityviewmodel.supplier_code,
                                            //utilityviewmodel.account_no,
                                            //utilityviewmodel.mpan,   // Should be meter serial number
                                            utilityviewmodel.bill_currency_separator,
                                            utilityviewmodel.last_usage_index[0],
                                            utilityviewmodel.last_usage_date[0],
                                            utilityviewmodel.Hezbollah.e_usage_changesList))
                    {
                        return false;
                    }
                    break; //goto like_old_days;
                case SmartParametersV2016.Gas:

                    if (!await Store_Gas_Readings(ourviewmodel,
                                    utilityviewmodel,
                                    task.ToArray(),
                                    utilityviewmodel.brand_code,
                                    utilityviewmodel.supplier_code,
                                    //utilityviewmodel.account_no,
                                    //utilityviewmodel.mprn,       // should be meter serial no
                                    utilityviewmodel.bill_currency_separator,
                                    utilityviewmodel.last_usage_index[1],
                                    utilityviewmodel.last_usage_date[1],
                                    utilityviewmodel.Hezbollah.g_usage_changesList))
                    {
                        return false;
                    }
                    break; //goto like_old_days;
                default:
                    break;
            }
            return true;
        }
        private static async Task<bool> FU_ViewBills(
#if ANDROIDX
                                                AppCompatActivity meterActivity,
#endif
                                                MainViewModel ourviewmodel,
                                                UtilityViewModel utilityviewmodel,
                                                HtmlAgilityPack.HtmlDocument document,
                                                bool TextBox_Active,
                                                string payments_pathname)
        {
            // For some reason ... and I think its due to a combination of First Utility's
            // paki-coded website and timing issues, when I do:
            // Gas -> Bills -> Electricity -> Bills, the Gas Bills never seem to load
            // correctly, i.e. I just get the Bills in the panel on the right
            // (If I do Electricity -> Bills -> Gas -> Bills both sets of Bills load
            //  correctly)

            IList<HtmlAgilityPack.HtmlNode>
                        HtmlCol1,
                        HtmlCol2,
                        HtmlCol3,
                        HtmlCol4;

            bool clicked = true;

            HtmlCol1 = YetAnotherFuckingHoop.SelectNodesAsList(document.DocumentNode, "//table");
            foreach (HtmlAgilityPack.HtmlNode element1 in HtmlCol1)
            {
                if (string.IsNullOrEmpty(element1.Id))
                {
                    string classname = element1.GetAttributeValue("class", "");
                    if (SmartNibbyV2016.Check_Classname(classname, "my-account-table", true))
                    {
                        HtmlCol2 = YetAnotherFuckingHoop.SelectNodesAsList(element1, ".//tbody");
                        foreach (HtmlAgilityPack.HtmlNode element2 in HtmlCol2)
                        {
                            classname = element2.GetAttributeValue("class", "");
                            // Get the visible and hidden tables
                            if (SmartNibbyV2016.Check_Classname(classname, "my-account-table", false))
                            {
                                HtmlCol3 = YetAnotherFuckingHoop.SelectNodesAsList(element2, ".//tr");
                                foreach (HtmlAgilityPack.HtmlNode element3 in HtmlCol3)
                                {
                                    if (!string.IsNullOrEmpty(element3.Id))
                                    {
                                        HtmlCol4 = YetAnotherFuckingHoop.SelectNodesAsList(element3, ".//td");
                                        if (!await Viewbills_Case(
#if ANDROIDX
                                                                    meterActivity,
#endif
                                                                    ourviewmodel,
                                                                    utilityviewmodel,
                                                                    TextBox_Active,
                                                                    HtmlCol4))
                                        {
                                            clicked = false;
                                            goto quit_viewbills;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            // Assume we have scraped all the outstanding bills

            // Amazingly (.. or not so considering what a bag of shit Microsoft,
            // Http, HtmlAgilityPack and all that LightSilver combined) oh and the paki
            // fuckers at FuckUtility are, this "always" goes back to "Your gas usage"
            // We can **ONLY** get to "Your electricity usage" by cycling round
            // inside FU_MyMeter.  What a crock of shit it all is ...

            // Hook up the next routine for the next delivery
            utilityviewmodel.next_routine = "VIEWPAYMENTS";
            utilityviewmodel.target_pathname = payments_pathname;
        quit_viewbills:
            return clicked;
        }

        private static string Check_Href(string classname,
                                        HtmlAgilityPack.HtmlNode element3,
                                        string href)
        {
            IList<HtmlAgilityPack.HtmlNode>
                       HtmlCol4;
            if (SmartNibbyV2016.Check_Classname(classname, "text-right", false))
            {
                HtmlCol4 = YetAnotherFuckingHoop.SelectNodesAsList(element3, ".//a");
                foreach (HtmlAgilityPack.HtmlNode element4 in HtmlCol4)
                {
                    if (string.IsNullOrEmpty(element4.Id))
                    {
                        href = element4.GetAttributeValue(SmartParametersV2016.href, "");
                        break;
                    }
                }
            }
            return href;
        }

        private static async Task<bool> Viewbills_Case(
#if ANDROIDX
                                                AppCompatActivity meterActivity,
#endif
                                                MainViewModel ourviewmodel,
                                                UtilityViewModel utilityviewmodel,
                                                bool TextBox_Active,
                                                IList<HtmlAgilityPack.HtmlNode> HtmlCol4)
        {
            DateTime bill_date = SmartParametersV2016.defaultDate;
            string webpage_id = "",
                    BILL_AMOUNT;// = "";

            string BILL_DATE;// = "";

            int count = 0;

            foreach (HtmlAgilityPack.HtmlNode element4 in HtmlCol4)
            {
                if (string.IsNullOrEmpty(element4.Id))
                {
                    string classname = element4.GetAttributeValue("class", "");
                    // Go and gind the Link
                    string href = "";
                    // It IS check for empty in further routine
                    href = Check_Href(classname, element4, href);

                    string inner_text = element4.InnerText.Trim();

                    if (!string.IsNullOrEmpty(inner_text))
                    {
                        switch (count)
                        {
                            case 0:
                                // Just for compatability with British Gas
                                BILL_DATE = inner_text.Trim();
                                if (!SmartParseV2016.Generic_Parse_Datetime(BILL_DATE, utilityviewmodel))
                                {
                                    // This should make sure we ignore this Date / this line on a date conversion failure
                                    return false;
                                }
                                else
                                {
                                    bill_date = utilityviewmodel.genericTargetDate;
                                }
                                break;
                            case 1:
                                webpage_id = inner_text;
                                break;
                            case 2:
                                BILL_AMOUNT = inner_text.Replace("&pound;", "");
                                BILL_AMOUNT = BILL_AMOUNT.Replace(utilityviewmodel.bill_currency_separator.ToString(), "");
                                BILL_AMOUNT = BILL_AMOUNT.Replace(utilityviewmodel.bill_thousands_separator.ToString(), "");
                                break;
                            case 3:
                                // We may or may not already done this bill in this current Read Meter
                                if (!string.IsNullOrEmpty(href))
                                {
                                    utilityviewmodel.statement_id = webpage_id;

                                    if (!await FU_Do_Actual_Effing_Read(
#if ANDROIDX
                                                                        meterActivity,
#endif
                                                                        ourviewmodel,
                                                                        utilityviewmodel,
                                                                        TextBox_Active,
                                                                        bill_date,
                                                                        href))
                                    {
                                        return false;
                                    }
                                }
                                break;
                            default:
                                break;
                        }
                    }
                }
                count++;
            }
            return true;
        }

        private static async Task<bool> FU_Do_Actual_Effing_Read(
#if ANDROIDX
                                                                    AppCompatActivity meterActivity,
#endif
                                                                    MainViewModel ourviewmodel,
                                                                    UtilityViewModel utilityviewmodel,
                                                                    bool TextBox_Active,
                                                                    DateTime bill_date,
                                                                    string href)
        {
            // Check the Bill's doesn't exist using the Date and Number
            // Start of COMMON PART
            utilityviewmodel.bill_date = bill_date;
            if (SmartUtilityV2022.Check_Bill_Date(utilityviewmodel))
            {
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
                // We may or may not already done this bill in this current Read Meter
                if (!SmartUtilityV2022.Bill_Already_Done(utilityviewmodel, mpan_mprn))
                {
                    // Statement Date is the one that appears in 'Processed:' line
                    // FU is special 'cos it has the Bill Number trailing the date
                    //string statement_date = utilityviewmodel.bill_date.ToString(SmartParametersV2016.bill_number_format) + SmartParametersV2016.space + utilityviewmodel.statement_id;

                    //
                    // GET https://www.first-utility.com/myaccount/bill/6951766 HTTP/1.1
                    //
                    if (!SmartNibbyV2016.Create_Uri(ourviewmodel,
                                                    utilityviewmodel.prfix_xxx,
                                                    href))
                    {
                        ourviewmodel.errorMessage = utilityviewmodel.current_routine + "|" + ourviewmodel.errorMessage;
                        return false;
                    }
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
                                                                    ourviewmodel.TargetUrl,
                                                                    utilityviewmodel.guid);

                    if (!string.IsNullOrEmpty(ourviewmodel.errorMessage) || !string.IsNullOrEmpty(ourviewmodel.pdfMessage))
                    {
                        return false;
                    }

                    if (await SmartPDFV2019.Generic_Close_1(ourviewmodel,
                                                                utilityviewmodel,
#if ANDROIDX
                                                                meterActivity,
#endif
                                                                pdfreader,
                                                                TextBox_Active))

                    {
                        if (!await FU_parse_bill(ourviewmodel,
                                                   utilityviewmodel,// utilityviewmodel.statement_id is already set in here
                                                   pdfreader))
                        {
#if WINFORMS
                            await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel,
                                " Parse failed: " + utilityviewmodel.account_no + SmartParametersV2016.space +
                                                                                                            utilityviewmodel.bill_date + SmartParametersV2016.space +
                                                                                                            utilityviewmodel.statement_id + SmartParametersV2016.space +
                                                                                                            utilityviewmodel.account_type + SmartParametersV2016.space +
                                                                                                            Environment.NewLine.ToString());
#endif
#if WPF  || SMARTMAUI
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

                    await SmartPDFV2019.Generic_Close_2(ourviewmodel,
                                                            utilityviewmodel,
                                                            pdfreader,
#if ANDROIDX
                                                            meterActivity,
#endif
                                                            TextBox_Active);

                    // Close the reader down (at last) here
                    if (pdfreader != null)
                    {
#if ANDROIDX
                        pdfreader.Close();
#endif
                    }
                    // End of COMMON PART
                }
            }
            return true;
        }
        private static bool FU_ViewPayments(MainViewModel ourviewmodel,
                                                UtilityViewModel utilityviewmodel,
                                                List<SmartUtility.Payments> payments_tempList,
                                                HtmlAgilityPack.HtmlDocument document,
                                                string myaccount_energy)
        {
            // For some reason ... and I think its due to a combination of First Utility's
            // paki-coded website and timing issues, when I do:
            // Gas -> Bills -> Electricity -> Bills, the Gas Bills never seem to load
            // correctly, i.e. I just get the Bills in the panel on the right
            // (If I do Electricity -> Bills -> Gas -> Bills both sets of Bills load
            //  correctly)

            string classname;// = "";
            int payment_amount = 0,
                    payment_balance = 0;

            IList<HtmlAgilityPack.HtmlNode>
                        HtmlCol1,
                        HtmlCol2,
                        HtmlCol3,
                        HtmlCol4;

            HtmlCol1 = YetAnotherFuckingHoop.SelectNodesAsList(document.DocumentNode, "//table");
            foreach (HtmlAgilityPack.HtmlNode element1 in HtmlCol1)
            {
                if (string.IsNullOrEmpty(element1.Id))
                {
                    classname = element1.GetAttributeValue("class", "");
                    if (SmartNibbyV2016.Check_Classname(classname, "my-account-table", true))
                    {
                        HtmlCol2 = YetAnotherFuckingHoop.SelectNodesAsList(element1, ".//tbody");
                        foreach (HtmlAgilityPack.HtmlNode element2 in HtmlCol2)
                        {
                            classname = element2.GetAttributeValue("class", "");
                            // Get the visible and hidden tables
                            if (SmartNibbyV2016.Check_Classname(classname, "my-account-table", false))
                            {
                                HtmlCol3 = YetAnotherFuckingHoop.SelectNodesAsList(element2, ".//tr");
                                foreach (HtmlAgilityPack.HtmlNode element3 in HtmlCol3)
                                {
                                    int count = 0;
                                    DateTime payment_date = SmartParametersV2016.defaultDate;

                                    short payment_item = 0,
                                            payment_code = 0;
                                    string PAYMENT_METHOD = "",
                                            PAYMENT_AMOUNT = "";

                                    if (string.IsNullOrEmpty(element3.Id))
                                    {
                                        HtmlCol4 = YetAnotherFuckingHoop.SelectNodesAsList(element3, ".//td");
                                        foreach (HtmlAgilityPack.HtmlNode element4 in HtmlCol4)
                                        {
                                            if (string.IsNullOrEmpty(element4.Id))
                                            {
                                                string inner_text = element4.InnerText.Trim();
                                                if (!string.IsNullOrEmpty(inner_text))
                                                {
                                                    switch (count)
                                                    {
                                                        case 0:
                                                            if (!SmartParseV2016.Generic_Parse_Datetime(inner_text, utilityviewmodel))
                                                            {
                                                                return false;
                                                            }
                                                            else
                                                            {
                                                                payment_date = utilityviewmodel.genericTargetDate;
                                                            }
                                                            break;
                                                        case 1:
                                                            PAYMENT_METHOD = inner_text.Trim();
                                                            if (!string.IsNullOrEmpty(PAYMENT_METHOD))
                                                            {
                                                                if (!SmartSpikeUtilityV2017.Utility_Lookup_PaymentCode(ourviewmodel,
                                                                                                                utilityviewmodel,
                                                                                                                PAYMENT_METHOD))
                                                                {
                                                                    return false;
                                                                }
                                                                PAYMENT_METHOD = utilityviewmodel.PAYMENT_METHOD;
                                                            }
                                                            break;
                                                        case 2:
                                                            PAYMENT_AMOUNT = inner_text.Replace("&pound;", "");
                                                            PAYMENT_AMOUNT = PAYMENT_AMOUNT.Replace(utilityviewmodel.bill_currency_separator.ToString(), "");
                                                            PAYMENT_AMOUNT = PAYMENT_AMOUNT.Replace(utilityviewmodel.bill_thousands_separator.ToString(), "");
                                                            if (!SmartParseV2016.Generic_Parse_Integer(PAYMENT_AMOUNT, utilityviewmodel))
                                                            {
                                                                return false;
                                                            }
                                                            else
                                                            {
                                                                payment_amount = utilityviewmodel.genericTransactionValue;
                                                            }
                                                            break;
                                                        default:
                                                            break;
                                                    }
                                                    count++;
                                                }
                                            }
                                        }
                                        if ((payment_date != SmartParametersV2016.defaultDate) &&
                                            (payment_amount != 0))
                                        {
                                            payment_item = (short)(payment_item + 1);
                                            if (!SmartParseV2016.Check_TempList(ourviewmodel,
                                                                                    utilityviewmodel,
                                                                                    payment_date,
                                                                                    payment_item,
                                                                                    payment_code,
                                                                                    payment_amount,
                                                                                    payment_balance,
                                                                                    payments_tempList))
                                            {
                                                // Sometimes ... we haven't had any bills and this field
                                                // is just ... empty;
                                                //SmartUtilityV2022.PaymentsList_Add(payments_tempList,
                                                //                                        SmartParametersV2016.Utility,
                                                //                                        utilityviewmodel.supplier_code,
                                                //                                        utilityviewmodel.brand_code,
                                                //                                        utilityviewmodel.ACCOUNT_NO,
                                                //                                        utilityviewmodel.CREATED,
                                                //                                        utilityviewmodel.STATEMENT_ID,
                                                //                                        utilityviewmodel.BILL_DATE,
                                                //                                        payment_date,
                                                //                                        payment_item,
                                                //                                        payment_code,
                                                //                                        payment_amount,
                                                //                                        payment_balance);
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
            }

            // Assume we have scraped all the outstanding bills

            // Amazingly (.. or not so considering what a bag of shit Microsoft,
            // Http, HtmlagilityPack and LightSilver combined) oh and the paki
            // fuckers at FuckUtility are, this "always" goes back to "Your gas usage"
            // We can **ONLY** get to "Your electricity usage" by cycling round
            // inside FU_MyMeter.  What a crock of shit it all is ...
            //pathname = "/login";
            // Hook up the next routine for the next delivery
            utilityviewmodel.next_routine = "PROCRESOURCE";
            utilityviewmodel.target_pathname = myaccount_energy;
            return true;
        }

        private static async Task<bool> Store_Gas_Readings(MainViewModel ourviewmodel,
                                                UtilityViewModel utilityviewmodel,
                                                string[] reader,
                                                short brand_code,
                                                short supplier_code,
                                                char bill_currency_separator,
                                                short last_index,
                                                DateTime last_usage_datetime,
                                                List<SmartUtility.GUsage> g_usageList)
        {
            string urgent_message = "",
                        entry_date, // = "",
                        entry_total, // = "",
                        entry_value; // = "";
            int counter = 0;
            int this_value;
            decimal this_total;
            DateTime this_datetime;// = SmartParametersV2016.defaultDate;

            string[] values;// = new string[] { };

            foreach (string input_line in reader)
            {
                // Skip the heading line(s)?
                if (counter > 1)
                {
                    // Get rid of all " characters which seem to be endemic
                    values = input_line.Replace("\"", "").Split(',');
                    if (values.Length >= 3)     // One less column for Gas
                    {
                        if (string.IsNullOrEmpty(values[0]))
                        {
                            // Give up on empty first value
                            break;
                        }

                        // For Gas readings there should be no Time ... but we don't want it to be 12pm!
                        entry_date = values[0];
                        entry_total = values[1];
                        entry_value = values[2];
                        if (!SmartParseV2016.Generic_Parse_Datetime(entry_date, utilityviewmodel))
                        {
                            // One of the very few occasions when we need to tell HQ something
                            if (!await SmartRoutinesV2018.CheckTrace(ourviewmodel, utilityviewmodel.utilityToken, supplier_code, brand_code, SmartParametersV2016.Utility.ToString() + " " + urgent_message))
                            {
                                return false;
                            }
                            return false;
                        }
                        else
                        {
                            this_datetime = utilityviewmodel.genericTargetDate;
                        }
                        // Is the current date/time greater than the last one stored?
                        if (SmartRoutinesV2018.DateTimeCompare(this_datetime, last_usage_datetime) > 0)
                        {
                            if (string.IsNullOrEmpty(entry_total))
                            {
                                entry_total = "0.0";
                            }
                            // All we get back now are the MONTHLY ACCUMULATIONS of usage as decimals ..
                            if (!SmartParseV2016.Generic_Parse_Decimal(entry_total, utilityviewmodel))
                            {
                                // One of the very few occasions when we need to tell HQ something
                                if (!await SmartRoutinesV2018.CheckTrace(ourviewmodel, utilityviewmodel.utilityToken, supplier_code, brand_code, SmartParametersV2016.Utility.ToString() + " " + urgent_message))
                                {
                                    return false;
                                }
                                return false;
                            }
                            else
                            {
                                this_total = utilityviewmodel.genericDecimalValue;
                            }
                            if (string.IsNullOrEmpty(entry_value))
                            {
                                entry_value = "0.00";
                            }
                            entry_value = entry_value.Replace(bill_currency_separator.ToString(), "");
                            if (!SmartParseV2016.Generic_Parse_Integer(entry_value, utilityviewmodel))
                            {
                                // One of the very few occasions when we need to tell HQ something
                                if (!await SmartRoutinesV2018.CheckTrace(ourviewmodel, utilityviewmodel.utilityToken, supplier_code, brand_code, SmartParametersV2016.Utility.ToString() + " " + urgent_message))
                                {
                                    return false;
                                }
                                return false;
                            }
                            else
                            {
                                this_value = utilityviewmodel.genericTransactionValue;
                                last_index = (short)(last_index + 1);
                                last_usage_datetime = this_datetime;
                                SmartUtility.GUsage g_usage_row = new SmartUtility.GUsage()
                                {
                                    CUBEFACE_CODE = SmartParametersV2016.Utility,
                                    MPAN_MPRN = utilityviewmodel.smell.MPRN,
                                    UNIQUE_INDEX = last_index,
                                    USAGE_DATETIME = this_datetime,
                                    USAGE_TOTAL = this_total,
                                    USAGE_VALUE = this_value
                                };
                                g_usageList.Add(g_usage_row);
                            }
                        }
                    }
                }
                counter++;
            }
            return true;
        }

        private static async Task<bool> Store_Electricity_Readings(MainViewModel ourviewmodel,
                                                            UtilityViewModel utilityviewmodel,
                                                            string[] reader,
                                                            short brand_code,
                                                            short supplier_code,
                                                            char bill_currency_separator,
                                                            short last_index,
                                                            DateTime last_usage_datetime,
                                                            List<SmartUtility.EUsage> e_usageList)
        {
            // Trim off the last reading if its 0  this is because sometimes The supplier
            // hasn't updated the reading for the previous day, and as we record units AND dates
            // of reading them, there is a chance we would miss a 'true' value by recording a 'false' 0.00 instead

            // Group the half-hourly readings into days ... I am extremely reluctant to do this
            // as it shows EXACTLY how much is being used each 30minutes (and when during the
            // day) ... but they display graphically like shit.  The gas usage displays so much better,
            // so its pretty pointless having all the overhead of recording these fuckers ...
            // and then not being able to show them in their true light (excuse the pun)

            string urgent_message = "",
                    entry_date, // = "",
                    entry_total, // = "",
                    entry_value; // = "";
            int counter = 0;
            int this_value;// = 0;
            decimal this_total;// = 0.0M;
            DateTime this_datetime;// = defaultDate;

            // What a fuck up this stuff is .. First Utility have TWO
            // fucking midnights for each day and NO entry at 2pm
            // This must be to do with daylight saving?
            // In any case we need to smooth this out and make:
            //    19-Sep-2015 23:30 -> 19-Sep-2015 23:30
            //    19-Sep-2015 00:00 -> 20-Sep-2015 00:00
            //    20-Sep-2015 00:00 -> 20-Sep-2015 00:30
            //    20-Sep-2015 00:30 -> 20-Sep-2015 01:00
            //    20-Sep-2015 01:00 -> 20-Sep-2015 01:30
            //    20-Sep-2015 01:30 -> 20-Sep-2015 02:00
            //                                            <== Note no entry at 2am!!!
            //    20-Sep-2015 02:30 -> 20-Sep-2015 02:30
            //
            //    But we dont care because we are looking for a midnight on
            //    a day AFTER the midnight on a previous day.  Its real simple:
            //    1. Compare the dates.  If this_date > last_date then do 2.
            //    2. Is this date a 'midnight?  Yes, then store it.  End of

            //    Otherwise the Chart3 is going to look really stupid

            string[] values;// = new string[] { };

            foreach (string input_line in reader)
            {
                // Skip the heading line(s)?
                if (counter > 1)
                {
                    // Get rid of all " characters which seem to be endemic
                    values = input_line.Replace("\"", "").Split(',');
                    if (values.Length >= 3)
                    {
                        if (string.IsNullOrEmpty(values[0]))
                        {
                            // Give up on empty first value
                            break;
                        }
                        // For Electricity readings, there SHOULD be a Time Which we can convert.
                        // No, there isn't anymore
                        entry_date = values[0];
                        entry_total = values[1];
                        entry_value = values[2];
                        if (!SmartParseV2016.Generic_Parse_Datetime(entry_date, utilityviewmodel))
                        {
                            // One of the very few occasions when we need to tell HQ something
                            if (!await SmartRoutinesV2018.CheckTrace(ourviewmodel, utilityviewmodel.utilityToken, supplier_code, brand_code, SmartParametersV2016.Utility.ToString() + " " + urgent_message))
                            {
                                return false;
                            }
                            return false;
                        }
                        else
                        {
                            this_datetime = utilityviewmodel.genericTargetDate;
                        }
                        // Is the current date/time greater than the last one stored?
                        if (SmartRoutinesV2018.DateTimeCompare(this_datetime, last_usage_datetime) > 0)
                        {
                            if (string.IsNullOrEmpty(entry_total))
                            {
                                entry_total = "0.0";
                            }
                            // All we get back now are the MONTHLY ACCUMULATIONS of usage as decimals ..
                            if (!SmartParseV2016.Generic_Parse_Decimal(entry_total, utilityviewmodel))
                            {
                                // One of the very few occasions when we need to tell HQ something
                                if (!await SmartRoutinesV2018.CheckTrace(ourviewmodel, utilityviewmodel.utilityToken, supplier_code, brand_code, SmartParametersV2016.Utility.ToString() + " " + urgent_message))
                                {
                                    return false;
                                }
                                return false;
                            }
                            else
                            {
                                this_total = utilityviewmodel.genericDecimalValue;
                            }
                            if (string.IsNullOrEmpty(entry_value))
                            {
                                entry_value = "0.00";
                            }
                            entry_value = entry_value.Replace(bill_currency_separator.ToString(), "");
                            if (!SmartParseV2016.Generic_Parse_Integer(entry_value, utilityviewmodel))
                            {
                                // One of the very few occasions when we need to tell HQ something
                                if (!await SmartRoutinesV2018.CheckTrace(ourviewmodel, utilityviewmodel.utilityToken, supplier_code, brand_code, SmartParametersV2016.Utility.ToString() + " " + urgent_message))
                                {
                                    return false;
                                }
                                return false;
                            }
                            else
                            {
                                this_value = utilityviewmodel.genericTransactionValue;
                                last_index = (short)(last_index + 1);
                                last_usage_datetime = this_datetime;
                                SmartUtility.EUsage e_usage_row = new SmartUtility.EUsage()
                                {
                                    CUBEFACE_CODE = SmartParametersV2016.Utility,
                                    MPAN_MPRN = utilityviewmodel.sparks.MPAN,
                                    UNIQUE_INDEX = last_index,
                                    USAGE_DATETIME = this_datetime,
                                    USAGE_TOTAL = this_total,
                                    USAGE_VALUE = this_value
                                };
                                e_usageList.Add(e_usage_row);
                            }
                        }
                    }
                }
                counter++;
            }
            return true;
        }

        internal static async Task<bool> FU_parse_bill(MainViewModel ourviewmodel,
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

                utilityviewmodel.sections = new string[][] {
                                                 new string[] { "A", "0", "", "", "0", "0", ""},
                                                 new string[] { "B", "0", "", "YourElectricityandGasBill" + "|" +
                                                                                "YourElectricityBill", "0", "-1", "" },
                                                 new string[] { "C", "0", "", "Aboutyourtariff", "-1", "-1", "" },
                                                 new string[] { "C", "1", "", "AboutyourTCR", "-1", "-1", "" },
                                                 new string[] { "C", "2", "", "Aboutyourusage", "-1", "-1", "" },
                                                 new string[] { "D", "0", "", "PaymentsandRefunds" + "|" +
                                                                                "Yourpayments", "-1", "-1", "" },
                                                 new string[] { "E", "0", "", "OtherChargesandCredits", "-1", "-1", "" },
                                                 // Can do F0 for 'E' only because FU don't do Gas only contracts
                                                 new string[] { "F", "0", "E", "AccountChargesandCredits", "-1", "-1", "" },
                                                 // Can do G0 for 'E' only because FU don't do Gas only contracts
                                                 new string[] { "G", "0", "E", "SupplyChargesandCredits", "-1", "-1", "" },
                                                 // Can do G1 for 'E' only because FU don't do Gas only contracts
                                                 new string[] { "G", "1", "E", "CreditNotes" + "|" +
                                                                                "Cancelledbills" + "|" +
                                                                                "CancelledBills", "-1", "-1", SmartParametersV2016.sectionsExactMatch },
                                                 new string[] { "H", "0", "", "YourTariff", "-1", "-1", "" },
                                                 new string[] { "I", "0", "", "YourUsageSummary", "-1", "-1", "" },
                                                 new string[] { "J", "1", "E", "ElectricityStatement" + "|" +
                                                                                "Electricitystatement", "-1", "-1", "" },
                                                 new string[] { "J", "2", "G", "GasStatement" + "|" +
                                                                                "Gasstatement", "-1", "-1", "" },
                                                 };
                // This is always needed anyway because we always do Page1 with this
                // Sometimes ... the Bill Date is never extracted from the PDF text!
                // But when (if) we evert find it - we substitute the Bill Number for it
                // IF we haven't found the Bill number ...
                int line_count = 0;
                bool success = Parse_Page0(ourviewmodel,
                                                utilityviewmodel,
                                                line_count,
                                                lines);
                if (!success ||
                    !SmartParseV2016.Check_Big_Four(ourviewmodel, utilityviewmodel, true))
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

        // Fucking bustling and disturbing me and being fucking noisy as usual
        // Constant fucking running commentary about what she has done.  Not so much
        // verbal diary as verbal diahorrea

        private static bool Parse_Page0(MainViewModel ourviewmodel,
                                UtilityViewModel utilityviewmodel,
                                int line_count,
                                string[] lines)
        {
            // Not perfect but not a bad start
            bool status = true;

            // Very Important Routine!!!
            SmartParseV2016.Sort_Sections(false, utilityviewmodel, lines, true);  // Not having 'true' scared the shit out of me!!

            // Pass 1 - look for the Big Three in Section 0
            if (!Parse_SectionA0(utilityviewmodel, utilityviewmodel.sections, "A", "0", lines))
            {
                ourviewmodel.errorMessage = "A0" + SmartParametersV2016.bar + ourviewmodel.errorMessage;
                return status;
            }
            if (!Parse_SectionB0(ourviewmodel, utilityviewmodel, line_count, utilityviewmodel.sections, "B", "0", lines))
            {
                ourviewmodel.errorMessage = "B0" + SmartParametersV2016.bar + ourviewmodel.errorMessage;
                return status;
            }
            return status;
        }

        private static async Task<bool> Parse_Pages(MainViewModel ourviewmodel,
                                        UtilityViewModel utilityviewmodel,
                                        string[] lines)
        {
            // Not perfect but not a bad start
            bool status = true;

            if (!await Parse_SectionC0(ourviewmodel, utilityviewmodel, utilityviewmodel.sections, "C", "0", lines)) // About your tariff

            {
                ourviewmodel.errorMessage = "C0" + SmartParametersV2016.bar + ourviewmodel.errorMessage;
                return false;
            }
            if (!Parse_SectionC1(utilityviewmodel, utilityviewmodel.sections, "C", "1", lines)) // About Your TCR
            {
                ourviewmodel.errorMessage = "C1" + SmartParametersV2016.bar + ourviewmodel.errorMessage;
                return false;
            }
            if (!Parse_SectionD0(ourviewmodel, utilityviewmodel, utilityviewmodel.sections, "D", "0", lines)) // Payments and Refunds
            {
                ourviewmodel.errorMessage = "D0" + SmartParametersV2016.bar + ourviewmodel.errorMessage;
                return false;
            }
            if (!Parse_SectionE0(ourviewmodel, utilityviewmodel, utilityviewmodel.sections, "E", "0", lines)) // Other Charges and Credits
            {
                ourviewmodel.errorMessage = "E0" + SmartParametersV2016.bar + ourviewmodel.errorMessage;
                return false;
            }
            if (!Parse_SectionF0(ourviewmodel, utilityviewmodel, utilityviewmodel.sections, "F", "0", lines)) // Account Charges Credits
            {
                ourviewmodel.errorMessage = "F0" + SmartParametersV2016.bar + ourviewmodel.errorMessage;
                return false;
            }
            if (!Parse_SectionG0(ourviewmodel, utilityviewmodel, utilityviewmodel.sections, "G", "0", lines)) // Supply Charges Credits
            {
                ourviewmodel.errorMessage = "G0" + SmartParametersV2016.bar + ourviewmodel.errorMessage;
                return false;
            }
            if (!Parse_SectionG1(ourviewmodel, utilityviewmodel, utilityviewmodel.sections, "G", "1", lines)) // Credit Notes
            {
                ourviewmodel.errorMessage = "G1" + SmartParametersV2016.bar + ourviewmodel.errorMessage;
                return false;
            }

            if (!await Parse_SectionH0(ourviewmodel, utilityviewmodel, utilityviewmodel.sections, "H", "0", lines)) // Your tariff

            {
                ourviewmodel.errorMessage = "H0" + SmartParametersV2016.bar + ourviewmodel.errorMessage;
                return false;
            }
            // Just to find the Supply Number!!
            if (!Parse_SectionI0(utilityviewmodel, utilityviewmodel.sections, "I", "0"))
            {
                ourviewmodel.errorMessage = "I0" + SmartParametersV2016.bar + ourviewmodel.errorMessage;
                return false;
            }
            int line_count = 0;
            if (!Parse_SectionJ1(ourviewmodel, utilityviewmodel, ref line_count, utilityviewmodel.sections, "J", "1", lines))
            {
                ourviewmodel.errorMessage = "J1" + SmartParametersV2016.bar + ourviewmodel.errorMessage;
                return false;
            }
            if (!Parse_SectionJ2(ourviewmodel, utilityviewmodel, utilityviewmodel.sections, "J", "2", lines))
            {
                ourviewmodel.errorMessage = "J2" + SmartParametersV2016.bar + ourviewmodel.errorMessage;
                return false;
                // Fucking silly cow cannot add 3 to 2014!!
            }
            return status;
        }
        private static bool Parse_SectionA0(UtilityViewModel utilityviewmodel,
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

            string token;// = "";
            // Not perfect but not a bad start
            char PAYMENT_PLAN = SmartParametersV2016.defaultChar;
            bool RESOURCE_BALANCES = false; // Default
            string PAYMENTS_RECEIVED = "0",
                    ACCOUNT_CHARGES_CREDITS = "0",
                    ACCOUNT_CHARGES_CREDITS_VAT_AMOUNT = "0",
                    SUPPLY_CHARGES_CREDITS = "0",
                    SUPPLY_CHARGES_CREDITS_VAT_AMOUNT = "0",
                    PREVIOUS_BALANCE = "0",
                    BILL_VAT_AMOUNT = "0",
                    OUTSTANDING_BALANCE = "0",
                    TOTAL_NOW_DUE = "0",
                    MONTHLY_PAYMENT = "0",
                    PAYMENT_DUE_DATE = SmartParametersV2016.defaultDates,
                    PAYMENT_TYPE = "",
                    DIRECT_DEBIT_DATE = SmartParametersV2016.defaultDates,
                    LOYALTY_BONUS = "0",
                    FIRST_YEAR_DISCOUNT = "0",
                    REWARDS = "n/a";
            short BILL_VAT_CODE = SmartParametersV2016.zeroRateVatCode,
                    RESOURCE_VAT_CODE = SmartParametersV2016.zeroRateVatCode;
            int TARIFF_CODE = 0,
                    NEW_CHARGES = 0,
                    RESOURCE_DISCOUNTS = 0,
                    RESOURCE_VAT_AMOUNT = 0;

            string cust_account_no = "CustomerAccountNumber",
                    account_number = "AccountNumber",
                    Bill_No = "BillNumber",
                    Bill_no = "Billnumber",
                    Bill_Date = "BillDate",
                    Bill_date = "Billdate",
                    Bill_Period = "BillPeriod",
                    Bill_period = "Billperiod";
            //dash = "-";

            utilityviewmodel.DISCOUNT_CREDIT_DATE = SmartParametersV2016.defaultDates;

            utilityviewmodel.BILL_PERIOD_START = SmartParametersV2016.defaultDates;
            utilityviewmodel.BILL_PERIOD_END = SmartParametersV2016.defaultDates;

            // One day I will be free of that numbskull never putting the lid
            // back properly on anything
            //DateTime temp_date = SmartParametersV2016.defaultDate;
            //string temp_urgent_message = "";

            // Pass 0 can we determine which type of Bill it is?  A (the first) or B the second .. or K the third???
            // This is useless as we have a Maria bill from 03-Jun-2014 in the old format
            // and a Markus bill from 13-May-2014 in the new format!!
            // All we can do is look for "Could you pay less?" which appears on the new bills
            // and not on the old bills ... lets try it
            int line_count = 0;
            while (line_count < lines.Length)
            {
                utilityviewmodel.token = lines[line_count].Replace(SmartParametersV2016.space, "");
                if (Check_SectionB0_Payless(utilityviewmodel))
                {
                    break;
                }
                //if (SmartParseV2016.Token_Identify(utilityviewmodel, "Couldyoupayless?" +
                //                                                    SmartParametersV2016.bar +
                //                                                    "YourPersonalProjection", true))
                //{
                //    utilityviewmodel.bill_version = 1;
                //    break;
                //}
                line_count++;
            }

            // Pass 1 get the Big Three (or four)
            //line_count = 0;
            for (int line_count1 = Convert.ToInt32(sections[utilityviewmodel.currentIndex][4]); line_count1 <= Convert.ToInt32(sections[utilityviewmodel.currentIndex][5]); line_count1++)
            {
                token = lines[line_count1].Replace(SmartParametersV2016.space, "");

                if (SmartParseV2016.Token_Identify(utilityviewmodel, cust_account_no, true) ||
                    SmartParseV2016.Token_Identify(utilityviewmodel, account_number, true))
                {
                    if (!Check_SectionA0_Account_Number(utilityviewmodel,
                                                            sections,
                                                            line_count,
                                                            utilityviewmodel.currentIndex,
                                                            lines,
                                                            token))
                    {
                        return false;
                    }

                    goto the_old_daysA0;
                }

                if (SmartParseV2016.Token_Identify(utilityviewmodel, Bill_No, true) ||
                    SmartParseV2016.Token_Identify(utilityviewmodel, Bill_no, true))
                {
                    //if (!check_sectionA0_bill_number(utilityviewmodel,
                    //                                        token,
                    //                                        sections,
                    //                                        section_index,
                    //                                        lines,
                    //                                        rf line_count))
                    //{
                    //    return false;
                    //}
                    goto the_old_daysA0;
                }

                if (SmartParseV2016.Token_Identify(utilityviewmodel, Bill_Date, true) ||
                    SmartParseV2016.Token_Identify(utilityviewmodel, Bill_date, true))
                {
                    if (!Check_SectionA0_Bill_Date(utilityviewmodel,
                                                    token,
                                                    ref line_count,
                                                    sections,
                                                    utilityviewmodel.currentIndex,
                                                    lines))
                    {
                        return false;
                    }
                    goto the_old_daysA0;
                }

                if (SmartParseV2016.Token_Identify(utilityviewmodel, Bill_Period, true) ||
                    SmartParseV2016.Token_Identify(utilityviewmodel, Bill_period, true))
                {
                    if (!Check_SectionA0_Bill_Period(utilityviewmodel,
                                                        token))
                    {
                        return false;
                    }

                    // This is a special for FU because the Foreigners don't have Bill periods
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
                                                        utilityviewmodel.DISCOUNT_CREDIT_DATE,
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
                    goto the_old_daysA0;
                }
            the_old_daysA0:
                continue;
            }
            return true;
        }

        private static bool Check_SectionA0_Account_Number(UtilityViewModel utilityviewmodel,
                                                            string[][] sections,
                                                            int line_count1,
                                                            int section_index,
                                                            string[] lines,
                                                            string token)
        {
            //string temp_urgent_message;// = "";

            if (string.IsNullOrEmpty(utilityviewmodel.ACCOUNT_NO))
            {
                utilityviewmodel.ACCOUNT_NO = token;
                // Version 1 code
                if (string.IsNullOrEmpty(utilityviewmodel.ACCOUNT_NO))
                {
                    // Try the previous line!!!
                    int sub_line_count = line_count1;
                    while (sub_line_count > Convert.ToInt32(sections[section_index][4]))
                    {
                        sub_line_count--;
                        string temp_account = lines[sub_line_count].Replace(SmartParametersV2016.space, "").Trim(); // Look backwards
                        if (SmartParseV2016.Generic_Parse_Digits(temp_account, utilityviewmodel))
                        {
                            utilityviewmodel.ACCOUNT_NO = temp_account;
                            break;
                        }
                    }
                }
                if (string.IsNullOrEmpty(utilityviewmodel.account_no) &&
                    !string.IsNullOrEmpty(utilityviewmodel.ACCOUNT_NO))   // If we are parsing a Bill from SmartTest
                {                                            // Then we clear this when we go in, so
                    utilityviewmodel.account_no = utilityviewmodel.ACCOUNT_NO;         // we need to re-establish it!
                }

                // Version 2 code
                //if (!string.IsNullOrEmpty(utilityviewmodel.ACCOUNT_NO) &&
                //    string.IsNullOrEmpty(utilityviewmodel.account_no))
                //{
                //    utilityviewmodel.account_no = utilityviewmodel.ACCOUNT_NO;
                //}
                // SmartUtilityV2022.Check_Utility.Accounts(utilityviewmodel, Hamas);
            }
            return true;
        }

        //private static bool check_sectionA0_bill_number(UtilityViewModel utilityviewmodel,
        //                                                    string token,
        //                                                    string[][] sections,
        //                                                    int section_index,
        //                                                    string[] lines,
        //                                                    rf int line_count)
        //{
        //    string temp_urgent_message = "";
        //
        //    utilityviewmodel.statementid = token;
        //    if (string.IsNullOrEmpty(utilityviewmodel.statementid))
        //    {
        //        // Try the previous line!!!
        //        int sub_line_count = line_count;
        //        while (sub_line_count > Convert.ToInt32(sections[section_index][4]))
        //        {
        //            sub_line_count = sub_line_count - 1;
        //            string temp_bill_number = lines[sub_line_count].Replace(SmartParametersV2016.space, "").Trim(); // Look backwards
        //            if (SmartParseV2016.Generic_Parse_Digits(temp_bill_number, rf temp_urgent_message))
        //            {
        //                utilityviewmodel.statementid = temp_bill_number;
        //                break;
        //            }
        //        }
        //    }
        //    // Version 2 code
        //    if (!string.IsNullOrEmpty(utilityviewmodel.statementid))
        //    {
        //        //  Fix the Bill Number with an extra '0' after the dash,
        //        // to make it look like the on-screen Bill Number
        //        int dash_index = utilityviewmodel.statementid.IndexOf(SmartParametersV2016.dash);
        //        if (dash_index >= 0)
        //        {
        //            // Fix up the Bill Number
        //            utilityviewmodel.statementid = utilityviewmodel.statementid.Replace(SmartParametersV2016.dash, SmartParametersV2016.dash + "0");
        //        }
        //    }
        //    utilityviewmodel.statementid = utilityviewmodel.statementid.TrimStart('0');
        //    //if (!string.IsNullOrEmpty(utilityviewmodel.statementid) &&
        //    //    string.IsNullOrEmpty(utilityviewmodel.statementid))
        //    //{
        //    //    utilityviewmodel.statementid = utilityviewmodel.statementid;  // A-cos we are desperate
        //    //}
        //    return true;
        //}

        private static bool Check_SectionA0_Bill_Date(UtilityViewModel utilityviewmodel,
                                                        string token,
                                                        ref int line_count,
                                                        string[][] sections,
                                                        int section_index,
                                                        string[] lines)
        {
            //DateTime temp_date = SmartParametersV2016.defaultDate;

            // Version 2
            utilityviewmodel.BILL_DATE = token;
            if (!string.IsNullOrEmpty(utilityviewmodel.BILL_DATE))
            {
                if (!SmartParseV2016.Generic_Parse_Datetime(utilityviewmodel.BILL_DATE, utilityviewmodel))
                {
                    return false;
                }
                //else
                //{
                //    temp_date = utilityviewmodel.genericTargetDate;
                //}
                return true;
            }
            // Version 1
            // Try the previous line!!!
            int sub_line_count = line_count;
            //string temp_urgent_message = "";
            while (sub_line_count > Convert.ToInt32(sections[section_index][4]))
            {
                sub_line_count--;
                string temp_bill_date = lines[sub_line_count].Replace(SmartParametersV2016.space, "").Trim(); // Look backwards
                if (!SmartParseV2016.Generic_Parse_Digits(temp_bill_date, utilityviewmodel))
                {
                    return false;
                }
                else
                {
                    utilityviewmodel.BILL_DATE = temp_bill_date;
                    break;
                }
            }
            if (!SmartParseV2016.Generic_Parse_Datetime(utilityviewmodel.BILL_DATE, utilityviewmodel))
            {
                return false;
            }
            //else
            //{
            //    temp_date = utilityviewmodel.genericTargetDate;
            //}
            return true;
        }

        private static bool Check_SectionA0_Bill_Period(UtilityViewModel utilityviewmodel,
                                                        string token)
        {
            //DateTime temp_date = SmartParametersV2016.defaultDate;

            string bill_range = token;
            string[] components = bill_range.Split('-');
            int component_count = 0;
            foreach (string component in components)
            {
                switch (component_count)
                {
                    case 0:
                        utilityviewmodel.BILL_PERIOD_START = component.Trim();
                        if (!SmartParseV2016.Generic_Parse_Datetime(utilityviewmodel.BILL_PERIOD_START, utilityviewmodel))
                        {
                            return false;
                        }
                        //else
                        //{
                        //    temp_date = utilityviewmodel.genericTargetDate;
                        //}
                        break;
                    case 1:
                        utilityviewmodel.BILL_PERIOD_END = component.Trim();
                        if (!SmartParseV2016.Generic_Parse_Datetime(utilityviewmodel.BILL_PERIOD_END, utilityviewmodel))
                        {
                            return false;
                        }
                        //else
                        //{
                        //    temp_date = utilityviewmodel.genericTargetDate;
                        //}
                        utilityviewmodel.DISCOUNT_CREDIT_DATE = utilityviewmodel.BILL_PERIOD_END;
                        break;
                    default:
                        break;
                }
                component_count++;
            }
            return true;
        }
        private static bool Parse_SectionB0(MainViewModel ourviewmodel,
                                            UtilityViewModel utilityviewmodel,
                                            int line_count,
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

            // Not perfect but not a bad start
            utilityviewmodel.OUTSTANDING_BALANCE = "0";
            string previous_bal = "Previousbalance",
                    payments_received = "Paymentsreceived",
                    new_charges = "NewCharges",
                    outstanding_bal = "Outstandingbalance";
            bool postcode_found = false;

            // One day I will be free of that numbskull never putting the lid
            // back properly on anything

            // Pass 0 can we determine which type of Bill it is?  A (the first) or B the second .. or K the third???
            // This is useless as we have a Maria bill from 03-Jun-2014 in the old format
            // and a Markus bill from 13-May-2014 in the new format!!
            // All we can do is look for "Could you pay less?" which appears on the new bills
            // and not on the old bills ... lets try it
            while (line_count < lines.Length)
            {
                utilityviewmodel.token = lines[line_count].Replace(SmartParametersV2016.space, "");
                if (Check_SectionB0_Payless(utilityviewmodel))
                {
                    break;
                }
                line_count++;
            }

            bool acc_no = string.IsNullOrEmpty(utilityviewmodel.ACCOUNT_NO) ||
                                utilityviewmodel.BILL_DATE == SmartParametersV2016.defaultDates;
            // Part 1 - have we 'made' a Bill?
            if (acc_no)
            {
                // Haven't found a Bill yet, so do Section A ... but with "B" tags !!!
                if (!Parse_SectionA0(utilityviewmodel, sections, "B", "0", lines))
                {
                    ourviewmodel.errorMessage = "A0" + SmartParametersV2016.space + ourviewmodel.errorMessage;
                    return false;
                }
            }

            acc_no = !string.IsNullOrEmpty(utilityviewmodel.ACCOUNT_NO) &&
                        utilityviewmodel.BILL_DATE != SmartParametersV2016.defaultDates;
            // Part 2
            if (acc_no)
            {
                //line_count = 0;
                for (int line_count1 = Convert.ToInt32(sections[utilityviewmodel.currentIndex][4]); line_count1 <= Convert.ToInt32(sections[utilityviewmodel.currentIndex][5]); line_count1++)
                {
                    utilityviewmodel.token = lines[line_count1].Replace(SmartParametersV2016.space, "");

                    if (SmartParseV2016.Token_Identify_Middle(utilityviewmodel, "Thisbillisforinformationonly." +
                                                                            SmartParametersV2016.bar +
                                                                            "Yourpaymentof", true, ""))
                    {
                        if (!Check_SectionB0_This_Bill(ourviewmodel,
                                                            utilityviewmodel,
                                                            ref line_count,
                                                            sections,
                                                            utilityviewmodel.currentIndex,
                                                            lines))
                        {
                            return false;
                        }
                        goto the_old_daysB0;
                    }

                    if (SmartParseV2016.Token_Identify(utilityviewmodel, previous_bal, true))
                    {
                        if (!Check_SectionB0_Previous_Balance(ourviewmodel,
                                                            utilityviewmodel,
                                                            ref line_count,
                                                            sections,
                                                            utilityviewmodel.currentIndex,
                                                            lines))
                        {
                            return false;
                        }
                        goto the_old_daysB0;
                    }

                    if (SmartParseV2016.Token_Identify(utilityviewmodel, payments_received, true))
                    {
                        if (!Check_SectionB0_Payments_Received(ourviewmodel,
                                                                utilityviewmodel))
                        {
                            return false;
                        }
                        goto the_old_daysB0;
                    }

                    //string NEW_CHARGES = "0";
                    if (SmartParseV2016.Token_Identify(utilityviewmodel, new_charges, true))
                    {
                        if (!Check_SectionB0_New_Charges(ourviewmodel,
                                                            utilityviewmodel,
                                                            ref line_count,
                                                            sections,
                                                            utilityviewmodel.currentIndex,
                                                            lines))
                        {
                            return false;
                        }
                        goto the_old_daysB0;
                    }

                    if (SmartParseV2016.Token_Identify(utilityviewmodel, outstanding_bal, true))
                    {
                        if (!Check_SectionB0_Outstanding_Balance(ourviewmodel,
                                                                utilityviewmodel))
                        {
                            return false;
                        }
                        goto the_old_daysB0;
                    }

                    if (SmartParseV2016.Token_Identify(utilityviewmodel, "TOTALNOWDUE", true))
                    {
                        if (!Check_SectionB0_Total_Now_Due(ourviewmodel,
                                                            utilityviewmodel))
                        {
                            return false;
                        }
                        goto the_old_daysB0;
                    }

                    if (!postcode_found)
                    {
                        postcode_found = Check_SectionB0_Postcode_Found(ourviewmodel,
                                                            utilityviewmodel,
                                                            lines[line_count].Trim());
                        goto the_old_daysB0;
                    }

                the_old_daysB0:
                    continue;
                }
            }
            return true;
        }

        private static bool Check_SectionB0_Postcode_Found(MainViewModel ourviewmodel,
                                                            UtilityViewModel utilityviewmodel,
                                                            string account_postcode)
        {
            if (SmartNibbyV2016.Derive_Postcode_New(account_postcode,
                                                    ourviewmodel.Blanche.workingPostcodesList,
                                                    utilityviewmodel))
            {
                return true;
            }
            return false;
        }

        private static bool Check_SectionB0_Payless(UtilityViewModel utilityviewmodel)
        {
            bool status = false;
            if (SmartParseV2016.Token_Identify(utilityviewmodel, "Couldyoupayless?" +
                                                            SmartParametersV2016.bar +
                                                            "YourPersonalProjection", false))
            {
                utilityviewmodel.bill_version = 1;
                status = true;
            }
            return status;
        }

        private static bool Check_SectionB0_This_Bill(MainViewModel ourviewmodel,
                                                        UtilityViewModel utilityviewmodel,
                                                        ref int line_count,
                                                        string[][] sections,
                                                        int section_index,
                                                        string[] lines)
        {
            string your_payment_will_be_collected_by = "Yourpaymentwillbecollectedby",
                    on_or_after = "onorafter";

            if (utilityviewmodel.token.IndexOf(on_or_after) == -1)
            {
                // Try the NEXT line!!!
                int sub_line_count = line_count + 1;
                while (sub_line_count <= Convert.ToInt32(sections[section_index][5]))
                {
                    utilityviewmodel.token += lines[sub_line_count].Replace(SmartParametersV2016.space, "").Trim();
                    if (utilityviewmodel.token.IndexOf(on_or_after) >= 0)
                    {
                        break;
                    }
                    sub_line_count++;
                }
                line_count = sub_line_count;

            }
            utilityviewmodel.token = utilityviewmodel.token.Replace(your_payment_will_be_collected_by, "");

            int after_index = utilityviewmodel.token.IndexOf(on_or_after);
            if (after_index >= 0)
            {
                string PAYMENT_TYPE = utilityviewmodel.token.Substring(0, after_index); // Good - don't change
                // Now try and lookup the 'temporary plan' and the uncompressed payment type
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

                after_index += on_or_after.Length;
                if (after_index == utilityviewmodel.token.Length)
                {
                    line_count++;
                    utilityviewmodel.token = lines[line_count];
                    after_index = 0;
                }
                string PAYMENT_DUE_DATE = utilityviewmodel.token.Substring(after_index);
                DateTime temp_date;
                if (!SmartParseV2016.Generic_Parse_Datetime(PAYMENT_DUE_DATE, utilityviewmodel))
                {
                    return false;
                }
                else
                {
                    temp_date = utilityviewmodel.genericTargetDate;
                }
                SmartParseV2016.Update_Bill(ourviewmodel, utilityviewmodel, "PAYMENT_DUE_DATE", PAYMENT_DUE_DATE);
                // Best we can do ..
                if (temp_date != SmartParametersV2016.defaultDate)
                {
                    temp_date = temp_date.AddMonths(1);
                    SmartParseV2016.Update_Bill(ourviewmodel, utilityviewmodel, "DIRECT_DEBIT_DATE", temp_date.ToString(SmartParametersV2016.defaultCulture));
                }
            }
            return true;
        }

        private static bool Check_SectionB0_Previous_Balance(MainViewModel ourviewmodel,
                                                            UtilityViewModel utilityviewmodel,
                                                            ref int line_count,
                                                            string[][] sections,
                                                            int section_index,
                                                            string[] lines)
        {
            string PREVIOUS_BALANCE;// = "0";

            int temp;// = 0;
            //string temp_urgent_message;// = "";

            // Version 1
            if (string.IsNullOrEmpty(utilityviewmodel.token))
            {
                // Try the NEXT line 
                int sub_line_count = line_count + 1;
                while (sub_line_count <= Convert.ToInt32(sections[section_index][5]))
                {
                    utilityviewmodel.token = lines[sub_line_count].Replace(SmartParametersV2016.space, "").Trim();
                    if (!string.IsNullOrEmpty(utilityviewmodel.token))
                    {
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
                        else
                        {
                            temp = utilityviewmodel.genericTransactionValue;
                        }
                        SmartParseV2016.Update_Bill(ourviewmodel, utilityviewmodel, "PREVIOUS_BALANCE", temp.ToString()); // Here Version 1
                        break;
                    }
                    sub_line_count++;
                }
                line_count = sub_line_count;
            }
            else
            {
                // Version 2
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
                else
                {
                    temp = utilityviewmodel.genericTransactionValue;
                }
                SmartParseV2016.Update_Bill(ourviewmodel, utilityviewmodel, "PREVIOUS_BALANCE", temp.ToString()); // Here Version 2
            }
            return true;
        }

        private static bool Check_SectionB0_Payments_Received(MainViewModel ourviewmodel,
                                                                UtilityViewModel utilityviewmodel)
        {
            // Version 1
            int currency_index = utilityviewmodel.token.IndexOf(utilityviewmodel.bill_currency_symbol);
            if (currency_index >= 0)
            {
                utilityviewmodel.token = utilityviewmodel.token.Substring(currency_index);
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
            //int temp = 0;
            if (!SmartParseV2016.Generic_Parse_Integer(PAYMENTS_RECEIVED, utilityviewmodel))
            {
                return false;
            }
            //else
            //{
            //    temp = utilityviewmodel.genericTransactionValue;
            //}
            SmartParseV2016.Update_Bill(ourviewmodel, utilityviewmodel, "PAYMENTS_RECEIVED", PAYMENTS_RECEIVED);
            return true;
        }

        private static bool Check_SectionB0_New_Charges(MainViewModel ourviewmodel,
                                                        UtilityViewModel utilityviewmodel,
                                                            ref int line_count,
                                                            string[][] sections,
                                                            int section_index,
                                                            string[] lines)
        {
            string NEW_CHARGES;// = "0";
            //int temp = 0;

            // Try the NEXT line!!!
            int sub_line_count = line_count + 1;
            bool finished = false;
            while (sub_line_count <= Convert.ToInt32(sections[section_index][5]))
            {
                utilityviewmodel.token = lines[sub_line_count].Replace(SmartParametersV2016.space, "").Trim();
                switch (utilityviewmodel.resource_code)
                {
                    case SmartParametersV2016.Electricity:
                        if (utilityviewmodel.token.IndexOf("Electricity") >= 0)
                        {
                            int currency_index = utilityviewmodel.token.IndexOf(utilityviewmodel.bill_currency_symbol);
                            if (currency_index >= 0)
                            {
                                utilityviewmodel.token = utilityviewmodel.token.Substring(currency_index);
                            }
                            if (!SmartParseV2016.Currency_Strip(utilityviewmodel.bill_currency_symbol, utilityviewmodel.bill_currency_separator, utilityviewmodel.bill_thousands_separator, utilityviewmodel.token, utilityviewmodel))
                            {
                                return false;
                            }
                            else
                            {
                                NEW_CHARGES = utilityviewmodel.value;
                            }
                            if (!SmartParseV2016.Generic_Parse_Integer(NEW_CHARGES, utilityviewmodel))
                            {
                                return false;
                            }
                            //else
                            //{
                            //    temp = utilityviewmodel.genericTransactionValue;
                            //}
                            SmartParseV2016.Update_Bill(ourviewmodel, utilityviewmodel, "RESOURCE_NEW_CHARGES", NEW_CHARGES);
                            finished = true;
                            break;
                        }
                        break;
                    case SmartParametersV2016.Gas:
                        if (utilityviewmodel.token.IndexOf("Gas") >= 0)
                        {
                            int currency_index = utilityviewmodel.token.IndexOf(utilityviewmodel.bill_currency_symbol);
                            if (currency_index >= 0)
                            {
                                utilityviewmodel.token = utilityviewmodel.token.Substring(currency_index);
                            }
                            if (!SmartParseV2016.Currency_Strip(utilityviewmodel.bill_currency_symbol, utilityviewmodel.bill_currency_separator, utilityviewmodel.bill_thousands_separator, utilityviewmodel.token, utilityviewmodel))
                            {
                                return false;
                            }
                            else
                            {
                                NEW_CHARGES = utilityviewmodel.value;
                            }
                            if (!SmartParseV2016.Generic_Parse_Integer(NEW_CHARGES, utilityviewmodel))
                            {
                                return false;
                            }
                            //else
                            //{
                            //    temp = utilityviewmodel.genericTransactionValue;
                            //}
                            SmartParseV2016.Update_Bill(ourviewmodel, utilityviewmodel, "RESOURCE_NEW_CHARGES", NEW_CHARGES);
                            finished = true;
                            break;
                        }
                        break;
                    default:
                        break;
                }
                if (finished)
                {
                    break;
                }
                sub_line_count++;
            }
            line_count = sub_line_count;
            return true;
        }

        private static bool Check_SectionB0_Outstanding_Balance(MainViewModel ourviewmodel,
                                                                UtilityViewModel utilityviewmodel)
        {
            // Version 1utilityviewmodel.
            utilityviewmodel.token = utilityviewmodel.token.Replace(SmartParametersV2016.equivalent, "");

            if (!SmartParseV2016.Currency_Strip(utilityviewmodel.bill_currency_symbol, utilityviewmodel.bill_currency_separator, utilityviewmodel.bill_thousands_separator, utilityviewmodel.token, utilityviewmodel))
            {
                return false;
            }
            else
            {
                utilityviewmodel.OUTSTANDING_BALANCE = utilityviewmodel.value;
            }
            //int temp = 0;
            if (!SmartParseV2016.Generic_Parse_Integer(utilityviewmodel.OUTSTANDING_BALANCE, utilityviewmodel))
            {
                return false;
            }
            //else
            //{
            //    temp = utilityviewmodel.genericTransactionValue;
            //}
            SmartParseV2016.Update_Bill(ourviewmodel, utilityviewmodel, "OUTSTANDING_BALANCE", utilityviewmodel.OUTSTANDING_BALANCE);
            return true;
        }

        private static bool Check_SectionB0_Total_Now_Due(MainViewModel ourviewmodel,
                                                            UtilityViewModel utilityviewmodel)
        {

            // Version 1
            utilityviewmodel.token = utilityviewmodel.token.Replace(SmartParametersV2016.equivalent, "");
            string TOTAL_NOW_DUE;// = "";
            if (!SmartParseV2016.Currency_Strip(utilityviewmodel.bill_currency_symbol, utilityviewmodel.bill_currency_separator, utilityviewmodel.bill_thousands_separator, utilityviewmodel.token, utilityviewmodel))
            {
                return false;
            }
            else
            {
                TOTAL_NOW_DUE = utilityviewmodel.value;
            }
            int temp;// = 0;
            if (!SmartParseV2016.Generic_Parse_Integer(TOTAL_NOW_DUE, utilityviewmodel))
            {
                return false;
            }
            else
            {
                temp = utilityviewmodel.genericTransactionValue;
            }
            SmartParseV2016.Update_Bill(ourviewmodel, utilityviewmodel, "TOTAL_NOW_DUE", utilityviewmodel.OUTSTANDING_BALANCE);
            SmartParseV2016.Update_Bill(ourviewmodel, utilityviewmodel, "OUTSTANDING_BALANCE", temp.ToString());
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

            string tariff_title1 = "TariffName-",
                    tariff_title2 = "Tariffname:",
                    tariff_title3 = "Tariffname";

            //int line_count;// = 0;

            for (int line_count = Convert.ToInt32(sections[utilityviewmodel.currentIndex][4]); line_count <= Convert.ToInt32(sections[utilityviewmodel.currentIndex][5]); line_count++)
            {
                utilityviewmodel.token = lines[line_count].Replace(SmartParametersV2016.space, "");
                if (SmartParseV2016.Token_Identify(utilityviewmodel, tariff_title1, false) ||
                        SmartParseV2016.Token_Identify(utilityviewmodel, tariff_title2, false) ||
                        SmartParseV2016.Token_Identify(utilityviewmodel, tariff_title3, false))
                {
                    // Well, I WAS THINKING but now I have been interrupted by CHURNING
                    if (string.IsNullOrEmpty(utilityviewmodel.TARIFF_NAME))
                    {
                        // Try and clean up 'multiple' Tariffs on a line
                        // We must assume that for Elec and Gas the tariff is the same ... (gulp)
                        if (!Clean_Token_Tariffs(ourviewmodel,
                                                    utilityviewmodel,
                                                    tariff_title1,
                                                    tariff_title2,
                                                    tariff_title3))
                        {
                            return false;
                        }

                        // New designator from the FU cunts
                        // Sometimes the title2 on the rhs is title3 on the lhs!!!
                        utilityviewmodel.token = utilityviewmodel.token.Replace(tariff_title3, "");
                        utilityviewmodel.token = utilityviewmodel.token.Replace("ebill", "");
                        utilityviewmodel.TARIFF_NAME = utilityviewmodel.token;

                        if (string.IsNullOrEmpty(utilityviewmodel.TARIFF_NAME))
                        {
                            // Try the NEXT line 
                            int sub_line_count = line_count;
                            while (sub_line_count <= Convert.ToInt32(sections[utilityviewmodel.currentIndex][5]))
                            {
                                utilityviewmodel.token = lines[sub_line_count].Replace(SmartParametersV2016.space, "");
                                if (Set_Tariff_Name(utilityviewmodel, utilityviewmodel.token, tariff_title2))
                                {
                                    break;
                                }
                                sub_line_count++;
                            }
                            line_count = sub_line_count;
                        }
                        utilityviewmodel.TARIFF_NAME = utilityviewmodel.TARIFF_NAME.Replace(SmartParametersV2016.space, "");

                        if (!await Check_Tariff_Name(ourviewmodel, utilityviewmodel))

                        {
                            return false;
                        }
                    }
                    goto the_old_daysC0;
                }
                // Well, I WAS THINKING but now I have been interrupted by CHURNING

                // Now ignore this 'Supply Number' line
                // I don't see the point of this switch anymore ...?
                // Well .. that's because you're a DICKHEAD
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
                //    line_count = Convert.ToInt32(sections[section_index][5]);
                //    goto the_old_daysC0;
                //}

                if (SmartParseV2016.Token_Identify(utilityviewmodel, "Paymentmethod:", true))
                {
                    string PAYMENT_TYPE = utilityviewmodel.token; // Good - don't change
                    // Now try and lookup the 'temporary plan' and the uncompressed payment type
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
            // Her name ISN'T Billie Wilder you fucking numbskull its Bille WHITELAW  .. what a fucking moron
            // AND there is NO 'T' at the end of the word D-E-L-I-C-A-T-E-S-S-A-N  <= there is NO FUCKING T HERE
            the_old_daysC0:
                continue;
            }
            return true;
        }

        private static async Task<bool> Check_Tariff_Name(MainViewModel ourviewmodel,
                                                        UtilityViewModel utilityviewmodel)
        {
            bool status = true;
            if (!string.IsNullOrEmpty(utilityviewmodel.TARIFF_NAME))
            {
                int paren_index = utilityviewmodel.TARIFF_NAME.IndexOf(")");
                if (paren_index >= 0)
                {
                    utilityviewmodel.TARIFF_NAME = utilityviewmodel.TARIFF_NAME.Substring(0, paren_index + 1);
                }
                if (utilityviewmodel.TARIFF_CODE == 0)
                {

                    if (await SmartUtilityV2022.Lookup_Remote_Tariff_Code(ourviewmodel, utilityviewmodel))

                    {
                        SmartParseV2016.Update_Bill(ourviewmodel, utilityviewmodel, "TARIFF_CODE", utilityviewmodel.TARIFF_CODE.ToString());
                    }
                    else
                    {
                        // ourviewmodel.errorMessage should be set here
                        status = false;
                    }
                }
            }
            return status;
        }

        private static bool Set_Tariff_Name(UtilityViewModel utilityviewmodel,
                                            string token,
                                            string tariff_title2)
        {
            if (!string.IsNullOrEmpty(token))
            {
                if (token.IndexOf(tariff_title2) == -1)
                {
                    utilityviewmodel.TARIFF_NAME = token;
                    return true;
                }
            }
            return false;
        }

        private static bool Clean_Token_Tariffs(MainViewModel ourviewmodel,
                                                UtilityViewModel utilityviewmodel,
                                                string tariff_title1,
                                                string tariff_title2,
                                                string tariff_title3)
        {
            if (!string.IsNullOrEmpty(utilityviewmodel.token))
            {
                int colon_index = utilityviewmodel.token.IndexOf(tariff_title1);
                if (colon_index >= 0)
                {
                    utilityviewmodel.token = utilityviewmodel.token.Replace(tariff_title1, "|");
                }
                else
                {
                    colon_index = utilityviewmodel.token.IndexOf(tariff_title2);
                    if (colon_index >= 0)
                    {
                        utilityviewmodel.token = utilityviewmodel.token.Replace(tariff_title2, "|");
                    }
                    else
                    {
                        colon_index = utilityviewmodel.token.IndexOf(tariff_title3);
                        if (colon_index >= 0)
                        {
                            utilityviewmodel.token = utilityviewmodel.token.Replace(tariff_title3, "|");
                        }
                        else
                        {
                            ourviewmodel.errorMessage = "Colon index is -1";
                            return false;
                        }
                    }
                }

                utilityviewmodel.token = utilityviewmodel.token.Trim('|');
                int bar_index = utilityviewmodel.token.IndexOf("|");
                if (bar_index >= 0)
                {
                    utilityviewmodel.token = utilityviewmodel.token.Substring(0, bar_index);
                }
                // otherwise leave token as it is
            }
            return true;
        }
        private static bool Parse_SectionC1(UtilityViewModel utilityviewmodel,
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
            // Not perfect but not a bad start

            string Tariff_Comparison_Rate = "TariffComparisonRate",
                    Electricity = "Electricity",
                    Gas = "Gas",
                    perkWh = "perkWh";

            //int line_count;// = 0;
            for (int line_count = Convert.ToInt32(sections[utilityviewmodel.currentIndex][4]); line_count <= Convert.ToInt32(sections[utilityviewmodel.currentIndex][5]); line_count++)
            {
                utilityviewmodel.token = lines[line_count].Replace(SmartParametersV2016.space, "");
                if (SmartParseV2016.Token_Identify(utilityviewmodel, Tariff_Comparison_Rate, false))
                {
                    if (utilityviewmodel.token.IndexOf(Tariff_Comparison_Rate) >= 0)
                    {
                        utilityviewmodel.token = utilityviewmodel.token.Replace(Tariff_Comparison_Rate, "");
                        utilityviewmodel.token = utilityviewmodel.token.Replace(":", "");
                        utilityviewmodel.token = utilityviewmodel.token.Replace(perkWh, "|");
                        utilityviewmodel.token = utilityviewmodel.token.Trim('|');
                        string[] components = utilityviewmodel.token.Split('|');
                        switch (utilityviewmodel.resource_code)
                        {
                            case SmartParametersV2016.Electricity:
                                if (components.Length > 0)
                                {
                                    utilityviewmodel.TCR = components[0];
                                }
                                break;
                            case SmartParametersV2016.Gas:
                                if (components.Length > 1)
                                {
                                    utilityviewmodel.TCR = components[1];
                                }
                                break;
                            default:
                                break;
                        }
                    }
                    goto the_old_days_C1;
                }

                if (SmartParseV2016.Token_Identify(utilityviewmodel, Electricity, true))
                {
                    utilityviewmodel.token = utilityviewmodel.token.Replace(perkWh, "");
                    utilityviewmodel.TCR = utilityviewmodel.token;
                    goto the_old_days_C1;
                }
                if (SmartParseV2016.Token_Identify(utilityviewmodel, Gas, true))
                {
                    utilityviewmodel.token = utilityviewmodel.token.Replace(perkWh, "");
                    utilityviewmodel.TCR = utilityviewmodel.token;
                    goto the_old_days_C1;
                }
            the_old_days_C1:
                continue;
            }
            return true;
        }

        private static bool Parse_SectionD0(MainViewModel ourviewmodel,
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

            // Not perfect but not a bad start
            int currency_index = 0;

            string ACCOUNT_TYPE;// = "";
            DateTime ACCOUNT_DATE;
            int ACCOUNT_AMOUNT;// = 0;
            short ACCOUNT_VAT_CODE = SmartParametersV2016.zeroRateVatCode;

            string payments_and_refunds = "PaymentsandRefunds",
                    your_payments = "Yourpayments",
                    refunds = "Refunds";

            //int line_count;// = 0;
            for (int line_count = Convert.ToInt32(sections[utilityviewmodel.currentIndex][4]); line_count <= Convert.ToInt32(sections[utilityviewmodel.currentIndex][5]); line_count++)
            {
                utilityviewmodel.token = lines[line_count].Replace(SmartParametersV2016.space, "");
                if (SmartParseV2016.Token_Identify(utilityviewmodel, payments_and_refunds, false) ||
                    SmartParseV2016.Token_Identify(utilityviewmodel, your_payments, false))
                {
                    // Try the NEXT line 
                    int sub_line_count = line_count + 1;
                    if (!Check_SectionD0_Payments(ourviewmodel,
                                                    utilityviewmodel,
                                                    sections,
                                                    utilityviewmodel.currentIndex,
                                                    lines,
                                                    sub_line_count))
                    {
                        return false;
                    }
                    line_count = sub_line_count;
                    goto the_old_daysD0;
                }

                if (SmartParseV2016.Token_Identify(utilityviewmodel, refunds, true) ||
                    SmartParseV2016.Token_Identify(utilityviewmodel, "CreditNotes", true))
                {
                    string temps;// = "";
                    if (!SmartParseV2016.Currency_Strip(utilityviewmodel.bill_currency_symbol, utilityviewmodel.bill_currency_separator, utilityviewmodel.bill_thousands_separator, utilityviewmodel.token, utilityviewmodel))
                    {
                        return false;
                    }
                    else
                    {
                        temps = utilityviewmodel.value;
                    }
                    if (!SmartParseV2016.Generic_Parse_Integer(temps, utilityviewmodel))
                    {
                        return false;
                    }
                    else
                    {
                        ACCOUNT_AMOUNT = utilityviewmodel.genericTransactionValue;
                    }
                    utilityviewmodel.token = utilityviewmodel.token.Substring(0, currency_index);
                    if (!SmartParseV2016.Generic_Parse_Datetime(utilityviewmodel.token, utilityviewmodel))
                    {
                        return false;
                    }
                    else
                    {
                        ACCOUNT_DATE = utilityviewmodel.genericTargetDate;
                    }
                    if ((ACCOUNT_DATE != SmartParametersV2016.defaultDate) &&
                        (ACCOUNT_AMOUNT != 0))
                    {
                        // Add it in
                        ACCOUNT_TYPE = utilityviewmodel.token.Substring(0, currency_index - 9);

                        string INCLUDE_BILLS = SmartParametersV2016.yesFlag;
                        utilityviewmodel.ACCOUNT_CHARGES_CREDITS_ITEM = (short)(utilityviewmodel.ACCOUNT_CHARGES_CREDITS_ITEM + 1);
                        bool is_it_there = false;
                        if (string.IsNullOrEmpty(sections[utilityviewmodel.currentIndex][2]))
                        {
                            is_it_there = SmartParseV2016.Check_Adjustment_Is_There(ourviewmodel,
                                                                                        utilityviewmodel,
                                                                                        ACCOUNT_DATE,
                                                                                        utilityviewmodel.ACCOUNT_CHARGES_CREDITS_ITEM,
                                                                                        ACCOUNT_TYPE,
                                                                                        ACCOUNT_VAT_CODE,
                                                                                        ACCOUNT_AMOUNT);
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
                                                                utilityviewmodel.ACCOUNT_CHARGES_CREDITS_ITEM,
                                                                ACCOUNT_TYPE,
                                                                ACCOUNT_VAT_CODE,
                                                                ACCOUNT_AMOUNT,
                                                                INCLUDE_BILLS);
                            SmartParseV2016.Update_Bill(ourviewmodel, utilityviewmodel, "ACCOUNT_CHARGES_CREDITS", ACCOUNT_AMOUNT.ToString());
                            // Note: ACCOUNT_AMOUNT has already been checked as an Integer}
                        }
                    }
                    goto the_old_daysD0;
                }

            // Now ignore this 'Supply Number' line
            // I don't see the point of this switch anymore ...?
            // Well .. that's because you're a DICKHEAD
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
            //    line_count = Convert.ToInt32(sections[section_index][5]);
            //    goto the_old_daysD0;
            //}
            the_old_daysD0:
                continue;
            }
            return true;
        }

        private static bool Check_SectionD0_Payments(MainViewModel ourviewmodel,
                                                    UtilityViewModel utilityviewmodel,
                                                    string[][] sections,
                                                    int section_index,
                                                    string[] lines,
                                                    int sub_line_count)
        {
            string payments = "Payments",
                    dash_payment = "-Payment";

            while (sub_line_count <= Convert.ToInt32(sections[section_index][5]))
            {
                utilityviewmodel.token = lines[sub_line_count].Replace(SmartParametersV2016.space, "").Trim();
                if ((utilityviewmodel.token == "supplynumber") ||
                    (utilityviewmodel.token == "SupplyNumber") ||
                    (utilityviewmodel.token == "CreditNotes"))
                {
                    // Here we should be able to extract the TOTAL PAYMENTS
                    // Not any more - FU Foreigners have put the TOTAL **BEFORE* the list of payments
                    // Fucking cunts ...
                    break;
                }
                if (SmartParseV2016.Token_Identify(utilityviewmodel, payments, false) ||
                    SmartParseV2016.Token_Identify(utilityviewmodel, "TOTAL", false))
                {
                    goto nextsub;
                }
                if (utilityviewmodel.token.IndexOf(utilityviewmodel.bill_currency_symbol) >= 0)
                {
                    string payment_amount = "0";
                    int currency_index = utilityviewmodel.token.IndexOf(utilityviewmodel.bill_currency_symbol);
                    if (currency_index >= 0)
                    {
                        payment_amount = utilityviewmodel.token.Substring(currency_index);
                    }
                    utilityviewmodel.PAYMENT_AMOUNT = "0";
                    if (!SmartParseV2016.Currency_Strip(utilityviewmodel.bill_currency_symbol, utilityviewmodel.bill_currency_separator, utilityviewmodel.bill_thousands_separator, payment_amount, utilityviewmodel))
                    {
                        return false;
                    }
                    utilityviewmodel.PAYMENT_AMOUNT = utilityviewmodel.value;
                    int temp;// = 0;
                    if (!SmartParseV2016.Generic_Parse_Integer(utilityviewmodel.PAYMENT_AMOUNT, utilityviewmodel))
                    {
                        return false;
                    }
                    else
                    {
                        temp = utilityviewmodel.genericTransactionValue;
                    }
                    if (temp > 0)
                    {
                        utilityviewmodel.value = temp.ToString();
                        SmartParseV2016.Add_Minus_Sign(utilityviewmodel);
                        utilityviewmodel.PAYMENT_AMOUNT = utilityviewmodel.value;
                    }
                    else
                    {
                        utilityviewmodel.value = temp.ToString();
                        SmartParseV2016.Remove_Minus_SignX(utilityviewmodel);
                        utilityviewmodel.PAYMENT_AMOUNT = utilityviewmodel.value;
                    }
                    utilityviewmodel.token = utilityviewmodel.token.Substring(0, currency_index);
                    // Just in case for 'A' and 'B' versions ...
                    utilityviewmodel.token = utilityviewmodel.token.Replace(dash_payment, "");
                    utilityviewmodel.PAYMENT_METHOD = utilityviewmodel.token;
                    utilityviewmodel.PAYMENT_CODE = "";
                    if (!string.IsNullOrEmpty(utilityviewmodel.PAYMENT_METHOD))
                    {
                        // Now try and lookup the 'temporary plan' and the uncompressed payment type
                        if (!SmartSpikeUtilityV2017.Utility_Lookup_PaymentCode(ourviewmodel,
                                                                        utilityviewmodel,
                                                                        utilityviewmodel.PAYMENT_METHOD)) //rf PAYMENT_CODE
                        {
                            return false;
                        }
                        //utilityviewmodel.PAYMENT_METHOD = utilityviewmodel.PAYMENT_METHOD;
                    }
                    utilityviewmodel.token = utilityviewmodel.token.Replace(utilityviewmodel.PAYMENT_METHOD.Replace(SmartParametersV2016.space, ""), "");
                    utilityviewmodel.PAYMENT_DATE = utilityviewmodel.token.Trim();
                    // Now check the (compressed) date!!!
                    if (!SmartParseV2016.Generic_Parse_Datetime(utilityviewmodel.PAYMENT_DATE, utilityviewmodel))
                    {
                        return false;
                    }
                    else
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
            nextsub:
                sub_line_count++;
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

            string token;// = "";

            // Not perfect but not a bad start
            string ACCOUNT_TYPE;// = "";
            DateTime ACCOUNT_DATE;// = SmartParametersV2016.defaultDate;
            int ACCOUNT_AMOUNT;// = 0;
            string INCLUDE_BILLS;// = "";
            short ACCOUNT_VAT_CODE = SmartParametersV2016.zeroRateVatCode;

            string none = "None";

            //int line_count = 0;
            for (int line_count = Convert.ToInt32(sections[utilityviewmodel.currentIndex][4]); line_count <= Convert.ToInt32(sections[utilityviewmodel.currentIndex][5]); line_count++)
            {
                token = lines[line_count].Replace(SmartParametersV2016.space, "");
                if (SmartParseV2016.Token_Identify(utilityviewmodel, "Credits", false))
                {
                    // Try the NEXT line 
                    int sub_line_count = line_count + 1;
                    while (sub_line_count <= Convert.ToInt32(sections[utilityviewmodel.currentIndex][5]))
                    {
                        token = lines[sub_line_count].Replace(SmartParametersV2016.space, "").Trim();
                        if (!string.IsNullOrEmpty(token))
                        {
                            if (SmartParseV2016.Token_Identify(utilityviewmodel, "TOTAL", false) ||
                                SmartParseV2016.Token_Identify(utilityviewmodel, none, false))
                            {
                                break;
                            }

                            if (token.IndexOf(utilityviewmodel.bill_currency_symbol) >= 0)
                            {
                                string account_amount = "0";
                                int currency_index = token.IndexOf(utilityviewmodel.bill_currency_symbol);
                                if (currency_index >= 0)
                                {
                                    account_amount = token.Substring(currency_index);
                                }
                                string temps;// = "";
                                if (!SmartParseV2016.Currency_Strip(utilityviewmodel.bill_currency_symbol, utilityviewmodel.bill_currency_separator, utilityviewmodel.bill_thousands_separator, account_amount, utilityviewmodel))
                                {
                                    return false;
                                }
                                else
                                {
                                    temps = utilityviewmodel.value;
                                }
                                if (!SmartParseV2016.Generic_Parse_Integer(temps, utilityviewmodel))
                                {
                                    return false;
                                }
                                else
                                {
                                    ACCOUNT_AMOUNT = utilityviewmodel.genericTransactionValue;
                                }
                                if (!SmartParseV2016.Generic_Parse_Datetime(utilityviewmodel.BILL_PERIOD_END, utilityviewmodel))
                                {
                                    return false;
                                }
                                else
                                {
                                    ACCOUNT_DATE = utilityviewmodel.genericTargetDate;
                                }
                                // Add it in unless its zero
                                if ((ACCOUNT_DATE != SmartParametersV2016.defaultDate) &&
                                    (ACCOUNT_AMOUNT != 0))
                                {
                                    token = token.Substring(0, currency_index);
                                    ACCOUNT_TYPE = "";
                                    int len = 0;
                                    while (len < token.Length)
                                    {
                                        string alpha = token.Substring(len, 1);
                                        if (alpha.ToUpper() == alpha)
                                        {
                                            ACCOUNT_TYPE = ACCOUNT_TYPE + SmartParametersV2016.space + alpha;
                                        }
                                        else
                                        {
                                            ACCOUNT_TYPE += alpha;
                                        }
                                        len++;
                                    }
                                    ACCOUNT_TYPE = ACCOUNT_TYPE.Trim();
                                    INCLUDE_BILLS = SmartParametersV2016.yesFlag;
                                    // Add it in
                                    // I couldn't give a shit what you did. And you are like a DOG WITH A FUCKING
                                    // BONE over that FUCKING TREE.  SHUT THE FUCK UP ABOUT THAT FUCKING TREE

                                    utilityviewmodel.ACCOUNT_CHARGES_CREDITS_ITEM = (short)(utilityviewmodel.ACCOUNT_CHARGES_CREDITS_ITEM + 1);
                                    bool is_it_there = false;
                                    {
                                        if (string.IsNullOrEmpty(sections[utilityviewmodel.currentIndex][2]))
                                        {
                                            is_it_there = SmartParseV2016.Check_Adjustment_Is_There(ourviewmodel,
                                                                                                    utilityviewmodel,
                                                                                                    ACCOUNT_DATE,
                                                                                                    utilityviewmodel.ACCOUNT_CHARGES_CREDITS_ITEM,
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
                                                                                            utilityviewmodel.ACCOUNT_CHARGES_CREDITS_ITEM,
                                                                                            ACCOUNT_TYPE,
                                                                                            ACCOUNT_VAT_CODE,
                                                                                            ACCOUNT_AMOUNT,
                                                                                            INCLUDE_BILLS);
                                        SmartParseV2016.Update_Bill(ourviewmodel, utilityviewmodel, "ACCOUNT_CHARGES_CREDITS", ACCOUNT_AMOUNT.ToString());
                                    }
                                }
                            }
                            sub_line_count++;
                        }
                    }
                    line_count = sub_line_count;
                    goto the_old_daysE0;
                }
            the_old_daysE0:
                continue;
            }
            return true;
        }

        private static bool Currency_Integer_Check(UtilityViewModel utilityviewmodel,
                                                    string token)
        {
            //int temp = 0;
            if (!SmartParseV2016.Currency_Strip(utilityviewmodel.bill_currency_symbol, utilityviewmodel.bill_currency_separator, utilityviewmodel.bill_thousands_separator, token, utilityviewmodel))
            {
                return false;
            }
            else
            {
                utilityviewmodel.TEMPS = utilityviewmodel.value;
            }
            if (!SmartParseV2016.Generic_Parse_Integer(utilityviewmodel.TEMPS, utilityviewmodel))
            {
                return false;
            }
            //else
            //{
            //    temp = utilityviewmodel.genericTransactionValue;
            //}
            return true;
        }
        private static bool Parse_SectionF0(MainViewModel ourviewmodel,
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
            // Not perfect but not a bad start
            string ACCOUNT_TYPE;// = "";
            DateTime ACCOUNT_DATE;
            utilityviewmodel.ACCOUNT_AMOUNT = "0";
            string ACCOUNT_VAT_AMOUNT,// = "0";
                    ACCOUNT_CHARGES_AMOUNT,// = "0";
                    INCLUDE_BILLS,// = "",
                    ACCOUNT_VAT_RATE;// = "";
            short ACCOUNT_VAT_CODE = SmartParametersV2016.zeroRateVatCode;

            string vat = "VAT@",
                    account_charges_and_credits = "AccountChargesandCredits",
                    total_account_charges_and_credits = "Totalaccountchargesandcredits",
                    total_cost_account_charges_and_credits = "Totalcostaccountchargesandcredits",
                    total_value_account_charges_and_credits = "Totalvalueaccountchargesandcredits",
                    none = "None";


            //int line_count;// = 0;
            for (int line_count = Convert.ToInt32(sections[utilityviewmodel.currentIndex][4]); line_count <= Convert.ToInt32(sections[utilityviewmodel.currentIndex][5]); line_count++)
            {
                utilityviewmodel.token = lines[line_count].Replace(SmartParametersV2016.space, "");

                if (SmartParseV2016.Token_Identify(utilityviewmodel, account_charges_and_credits, true))
                {
                    // Try the NEXT line 
                    int sub_line_count = line_count + 1;
                    while (sub_line_count <= Convert.ToInt32(sections[utilityviewmodel.currentIndex][5]))
                    {
                        utilityviewmodel.token = lines[sub_line_count].Replace(SmartParametersV2016.space, "").Trim();
                        if (!string.IsNullOrEmpty(utilityviewmodel.token))
                        {
                            if (SmartParseV2016.Token_Identify(utilityviewmodel, none, false) ||
                               SmartParseV2016.Token_Identify(utilityviewmodel, total_value_account_charges_and_credits, false))
                            {
                                break;
                            }
                            if (utilityviewmodel.token.IndexOf(total_cost_account_charges_and_credits) >= 0)
                            {
                                utilityviewmodel.token = utilityviewmodel.token.Replace(total_cost_account_charges_and_credits, "");
                                if (!Currency_Integer_Check(utilityviewmodel,
                                                            utilityviewmodel.token))
                                {
                                    return false;
                                }
                                ACCOUNT_CHARGES_AMOUNT = utilityviewmodel.TEMPS;
                                SmartParseV2016.Update_Bill(ourviewmodel, utilityviewmodel, "ACCOUNT_CHARGES_CREDITS", ACCOUNT_CHARGES_AMOUNT);
                                break;
                            }

                            if (utilityviewmodel.token.IndexOf(vat) >= 0)
                            {
                                ACCOUNT_VAT_RATE = utilityviewmodel.token.Replace(vat, "").Trim();
                                int percent = ACCOUNT_VAT_RATE.IndexOf("%");
                                if (percent >= 0)
                                {
                                    ACCOUNT_VAT_RATE = ACCOUNT_VAT_RATE.Substring(0, percent) + ".00";
                                    //ACCOUNT_VAT_CODE = SmartParametersV2016.zeroRateVatCode;
                                    if (!SmartParseV2016.Lookup_Vat_Code(ACCOUNT_VAT_RATE, SmartTimeV2016.ConvertDateTime(utilityviewmodel.BILL_PERIOD_END), ourviewmodel.Blanche.vatRatesList, utilityviewmodel))
                                    {
                                        return false;
                                    }
                                    else
                                    {
                                        ACCOUNT_VAT_CODE = utilityviewmodel.vat_code;
                                    }
                                    foreach (SmartUtility.AccChargesCredits account_charges_credits_row in utilityviewmodel.Hezbollah.account_charges_credits_changesList)
                                    {
                                        account_charges_credits_row.ACCOUNT_VAT_CODE = ACCOUNT_VAT_CODE;
                                    }

                                    int currency_index = utilityviewmodel.token.IndexOf(utilityviewmodel.bill_currency_symbol);
                                    if (currency_index >= 0)
                                    {
                                        utilityviewmodel.token = utilityviewmodel.token.Substring(currency_index);
                                    }
                                    if (!Currency_Integer_Check(utilityviewmodel,
                                                                utilityviewmodel.token))
                                    {
                                        return false;
                                    }
                                    ACCOUNT_VAT_AMOUNT = utilityviewmodel.TEMPS;
                                    SmartParseV2016.Update_Bill(ourviewmodel, utilityviewmodel, "ACCOUNT_CHARGES_CREDITS_VAT_AMOUNT", ACCOUNT_VAT_AMOUNT);
                                }
                                goto update;
                            }

                            if (utilityviewmodel.token.IndexOf(total_account_charges_and_credits) == -1)
                            {
                                utilityviewmodel.token = Do_Last_Spaces(lines, sub_line_count);//, utilityviewmodel.token);

                                string[] components = utilityviewmodel.token.Split(SmartParametersV2016.bar);
                                if (components.Length == 2)
                                {
                                    ACCOUNT_TYPE = components[0].Trim();
                                    utilityviewmodel.token = utilityviewmodel.bill_currency_symbol + components[1];
                                    if (!Currency_Integer_Check(utilityviewmodel,
                                                                utilityviewmodel.token))
                                    {
                                        return false;
                                    }
                                    utilityviewmodel.ACCOUNT_AMOUNT = utilityviewmodel.TEMPS;
                                    if (!SmartParseV2016.Generic_Parse_Datetime(utilityviewmodel.BILL_PERIOD_END, utilityviewmodel))
                                    {
                                        return false;
                                    }
                                    else
                                    {
                                        ACCOUNT_DATE = utilityviewmodel.genericTargetDate;
                                    }
                                    // Add it in unless its zero
                                    if ((ACCOUNT_DATE != SmartParametersV2016.defaultDate) &&
                                        (utilityviewmodel.ACCOUNT_AMOUNT != "0"))
                                    {
                                        INCLUDE_BILLS = SmartParametersV2016.yesFlag;

                                        // I couldn't give a shit what you did. And you are like a DOG WITH A FUCKING
                                        // BONE over that FUCKING TREE.  SHUT THE FUCK UP ABOUT THAT FUCKING TREE

                                        utilityviewmodel.ACCOUNT_CHARGES_CREDITS_ITEM = (short)(utilityviewmodel.ACCOUNT_CHARGES_CREDITS_ITEM + 1);
                                        bool is_it_there = false;
                                        {
                                            if (string.IsNullOrEmpty(sections[utilityviewmodel.currentIndex][2]))
                                            {
                                                is_it_there = SmartParseV2016.Check_Adjustment_Is_There(ourviewmodel,
                                                                                                        utilityviewmodel,
                                                                                                        ACCOUNT_DATE,
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
                                                                                        ACCOUNT_DATE,
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
                        }
                    update:
                        sub_line_count++;
                    }
                    line_count = sub_line_count;
                    goto the_old_daysF0;
                }
            the_old_daysF0:
                continue;
            }
            return true;
        }

        private static string Do_Last_Spaces(string[] lines, int sub_line_count)//, string token)
        {
            int last_space;// = 0;
            bool credit = false;
            bool debit = false;
            string token = lines[sub_line_count].Trim();
            last_space = token.LastIndexOf(SmartParametersV2016.space);
            if ((last_space >= 0) &&
                (last_space + 3 <= token.Length))
            {
                if (token.Substring(last_space + 1, 2) == "CR")
                {
                    credit = true;
                    token = token.Substring(0, last_space);
                }
                else
                {
                    if (token.Substring(last_space + 1, 2) == "DR")
                    {
                        debit = true;
                        token = token.Substring(0, last_space);
                    }
                }
                if (credit || debit)
                {
                    last_space = token.LastIndexOf(SmartParametersV2016.space);
                    if (last_space >= 0)
                    {
                        StringBuilder sb = new StringBuilder(token);
                        sb[last_space] = SmartParametersV2016.bar;
                        token = sb.ToString();
                        if (credit)
                        {
                            token = token + SmartParametersV2016.space + "CR";
                        }
                        else
                        {
                            if (debit)
                            {
                                token = token + SmartParametersV2016.space + "DR";
                            }
                        }
                    }
                }
            }
            return token;
        }

        private static bool Parse_SectionG0(MainViewModel ourviewmodel,
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
            string SUPPLY_TYPE,// = "",
                    SUPPLY_CREDIT_BILL,// = "",
                    SUPPLY_VAT_RATE,// = "",
                    SUPPLY_VAT_AMOUNT,// = "0",
                    SUPPLY_CHARGES_AMOUNT;// = "0";
            int SUPPLY_AMOUNT;// = 0;
            DateTime SUPPLY_DATE,
                        SUPPLY_DUE_DATE;
            short SUPPLY_VAT_CODE = SmartParametersV2016.zeroRateVatCode;

            string vat = "VAT@",
                    supply_charges_and_credits = "SupplyChargesandCredits",
                    total_supply_charges_and_credits = "Totalsupplychargesandcredits",
                    total_value_supply_charges_and_credits = "Totalvaluesupplychargesandcredits",
                    none = "None";
            //int TEMP = 0;

            //int line_count;// = 0;
            for (int line_count = Convert.ToInt32(sections[utilityviewmodel.currentIndex][4]); line_count <= Convert.ToInt32(sections[utilityviewmodel.currentIndex][5]); line_count++)
            {
                utilityviewmodel.token = lines[line_count].Replace(SmartParametersV2016.space, "");

                if (SmartParseV2016.Token_Identify(utilityviewmodel, supply_charges_and_credits, true))
                {
                    // Try the NEXT line 
                    int sub_line_count = line_count + 1;
                    while (sub_line_count <= Convert.ToInt32(sections[utilityviewmodel.currentIndex][5]))
                    {
                        utilityviewmodel.token = lines[sub_line_count].Replace(SmartParametersV2016.space, "").Trim();
                        if (!string.IsNullOrEmpty(utilityviewmodel.token))
                        {
                            if (utilityviewmodel.token == none)
                            {
                                break;
                            }
                            if (utilityviewmodel.token.IndexOf(total_value_supply_charges_and_credits) >= 0)
                            {
                                utilityviewmodel.token = utilityviewmodel.token.Replace(total_value_supply_charges_and_credits, "");
                                if (!SmartParseV2016.Currency_Strip(utilityviewmodel.bill_currency_symbol, utilityviewmodel.bill_currency_separator, utilityviewmodel.bill_thousands_separator, utilityviewmodel.token, utilityviewmodel))
                                {
                                    return false;
                                }
                                else
                                {
                                    SUPPLY_CHARGES_AMOUNT = utilityviewmodel.value;
                                }
                                if (!SmartParseV2016.Generic_Parse_Integer(SUPPLY_CHARGES_AMOUNT, utilityviewmodel))
                                {
                                    return false;
                                }
                                //else
                                //{
                                //    TEMP = utilityviewmodel.genericTransactionValue;
                                //}
                                SmartParseV2016.Update_Bill(ourviewmodel, utilityviewmodel, "SUPPLY_CHARGES_CREDITS", SUPPLY_CHARGES_AMOUNT);
                                break;
                            }
                            if (utilityviewmodel.token.IndexOf(vat) >= 0)
                            {
                                SUPPLY_VAT_RATE = utilityviewmodel.token.Replace(vat, "").Trim();
                                int percent = SUPPLY_VAT_RATE.IndexOf("%");
                                if (percent >= 0)
                                {
                                    SUPPLY_VAT_RATE = SUPPLY_VAT_RATE.Substring(0, percent) + ".00";
                                    //SUPPLY_VAT_CODE = SmartParametersV2016.zeroRateVatCode;
                                    if (!SmartParseV2016.Lookup_Vat_Code(SUPPLY_VAT_RATE, SmartTimeV2016.ConvertDateTime(utilityviewmodel.BILL_PERIOD_END), ourviewmodel.Blanche.vatRatesList, utilityviewmodel))
                                    {
                                        return false;
                                    }
                                    else
                                    {
                                        SUPPLY_VAT_CODE = utilityviewmodel.vat_code;
                                    }
                                    foreach (SmartUtility.SupChargesCredits supply_charges_credits_row in utilityviewmodel.Hezbollah.supply_charges_credits_changesList)
                                    {
                                        supply_charges_credits_row.SUPPLY_VAT_CODE = SUPPLY_VAT_CODE;
                                    }

                                    int currency_index = utilityviewmodel.token.IndexOf(utilityviewmodel.bill_currency_symbol);
                                    if (currency_index >= 0)
                                    {
                                        utilityviewmodel.token = utilityviewmodel.token.Substring(currency_index);
                                    }
                                    if (!SmartParseV2016.Currency_Strip(utilityviewmodel.bill_currency_symbol, utilityviewmodel.bill_currency_separator, utilityviewmodel.bill_thousands_separator, utilityviewmodel.token, utilityviewmodel))
                                    {
                                        return false;
                                    }
                                    else
                                    {
                                        SUPPLY_VAT_AMOUNT = utilityviewmodel.value;
                                    }
                                    if (!SmartParseV2016.Generic_Parse_Integer(SUPPLY_VAT_AMOUNT, utilityviewmodel))
                                    {
                                        return false;
                                    }
                                    //else
                                    //{
                                    //    TEMP = utilityviewmodel.genericTransactionValue;
                                    //}
                                    SmartParseV2016.Update_Bill(ourviewmodel, utilityviewmodel, "SUPPLY_CHARGES_CREDITS_VAT_AMOUNT", SUPPLY_VAT_AMOUNT);
                                }
                                goto update;
                            }

                            if (utilityviewmodel.token.IndexOf(total_supply_charges_and_credits) == -1)
                            {
                                utilityviewmodel.token = Do_Last_Spaces(lines, sub_line_count);//, utilityviewmodel.token);

                                string[] components = utilityviewmodel.token.Split(SmartParametersV2016.bar);
                                if (components.Length == 2)
                                {
                                    SUPPLY_TYPE = components[0].Trim();
                                    utilityviewmodel.token = utilityviewmodel.bill_currency_symbol + components[1];
                                    string temps;// = "";
                                    if (!SmartParseV2016.Currency_Strip(utilityviewmodel.bill_currency_symbol, utilityviewmodel.bill_currency_separator, utilityviewmodel.bill_thousands_separator, utilityviewmodel.token, utilityviewmodel))
                                    {
                                        return false;
                                    }
                                    else
                                    {
                                        temps = utilityviewmodel.value;
                                    }
                                    if (!SmartParseV2016.Generic_Parse_Integer(temps, utilityviewmodel))
                                    {
                                        return false;
                                    }
                                    else
                                    {
                                        SUPPLY_AMOUNT = utilityviewmodel.genericTransactionValue;
                                    }
                                    if (!SmartParseV2016.Generic_Parse_Datetime(utilityviewmodel.BILL_PERIOD_END, utilityviewmodel))
                                    {
                                        return false;
                                    }
                                    else
                                    {
                                        SUPPLY_DATE = utilityviewmodel.genericTargetDate;
                                    }
                                    if ((SUPPLY_DATE != SmartParametersV2016.defaultDate) &&
                                        (SUPPLY_AMOUNT != 0))
                                    {
                                        // Add it in
                                        // I couldn't give a shit what you did. And you are like a DOG WITH A FUCKING
                                        // BONE over that FUCKING TREE.  SHUT THE FUCK UP ABOUT THAT FUCKING TREE
                                        SUPPLY_DUE_DATE = SUPPLY_DATE;
                                        SUPPLY_CREDIT_BILL = utilityviewmodel.STATEMENT_ID;

                                        utilityviewmodel.SUPPLY_CHARGES_CREDITS_ITEM = (short)(utilityviewmodel.SUPPLY_CHARGES_CREDITS_ITEM + 1);
                                        if (!SmartParseV2016.Check_Supply_Is_There(ourviewmodel,
                                                                                utilityviewmodel,
                                                                                SUPPLY_DATE,
                                                                                utilityviewmodel.SUPPLY_CHARGES_CREDITS_ITEM,
                                                                                SUPPLY_TYPE,
                                                                                SUPPLY_VAT_CODE,
                                                                                SUPPLY_AMOUNT))
                                        {
                                            SmartUtilityV2022.Supply_Charges_CreditsList_Add(utilityviewmodel,
                                                                                            SmartParametersV2016.Utility,
                                                                                            utilityviewmodel.supplier_code,
                                                                                            utilityviewmodel.brand_code,
                                                                                            utilityviewmodel.ACCOUNT_NO,
                                                                                            utilityviewmodel.CREATED,
                                                                                            utilityviewmodel.STATEMENT_ID,
                                                                                            utilityviewmodel.BILL_DATE,
                                                                                            SUPPLY_DATE,
                                                                                            utilityviewmodel.SUPPLY_CHARGES_CREDITS_ITEM,
                                                                                            SUPPLY_TYPE,
                                                                                            SUPPLY_VAT_CODE,
                                                                                            SUPPLY_AMOUNT,
                                                                                            SUPPLY_DUE_DATE,
                                                                                            SUPPLY_CREDIT_BILL);
                                        }
                                    }
                                }
                            }
                        }
                    update:
                        sub_line_count++;
                    }
                    line_count = sub_line_count;
                    goto the_old_daysG0;
                }
            the_old_daysG0:
                continue;
            }
            return true;
        }

        private static bool Parse_SectionG1(MainViewModel ourviewmodel,
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
            DateTime ACCOUNT_DATE;
            int ACCOUNT_AMOUNT;// = 0;
            string ACCOUNT_TYPE,// = "",
                    INCLUDE_BILLS,// = "",
                    CREDIT_DATE = "";
            short ACCOUNT_VAT_CODE = SmartParametersV2016.zeroRateVatCode;

            string date_product_amount = "DateProductAmount";

            //int line_count;// = 0;
            for (int line_count = Convert.ToInt32(sections[utilityviewmodel.currentIndex][4]); line_count <= Convert.ToInt32(sections[utilityviewmodel.currentIndex][5]); line_count++)
            {
                utilityviewmodel.token = lines[line_count].Replace(SmartParametersV2016.space, "");

                if (SmartParseV2016.Token_Identify(utilityviewmodel, date_product_amount, true))
                {
                    // Try the NEXT line 
                    int sub_line_count = line_count + 1;
                    while (sub_line_count <= Convert.ToInt32(sections[utilityviewmodel.currentIndex][5]))
                    {
                        utilityviewmodel.token = lines[sub_line_count].Replace(SmartParametersV2016.space, "").Trim();
                        if (!string.IsNullOrEmpty(utilityviewmodel.token))
                        {
                            if (utilityviewmodel.token.Length >= 5)
                            {
                                if (utilityviewmodel.token.Substring(0, 5) == "TOTAL")
                                {
                                    break;
                                }
                            }
                            // Can we make a date from the first 10 characters?
                            if (utilityviewmodel.token.Length >= 9)
                            {
                                CREDIT_DATE = utilityviewmodel.token.Substring(0, 9);
                                if (!SmartParseV2016.Generic_Parse_Datetime(CREDIT_DATE, utilityviewmodel))
                                {
                                    // In the event that we DON'T find 'TOTAL' (which now fucking appears at
                                    // the top, instead of at the end (sigh)) we have to break when we can't find a date
                                    // within the first 9 characters
                                    //
                                    // Maybe one day when we have time, we can re-write this to
                                    // find out the TOTAL 123.45CR and then read lines up to that
                                    // total?
                                    ourviewmodel.errorMessage = "";
                                    break;
                                }
                                //else
                                //{
                                //    temp_date = utilityviewmodel.genericTargetDate;
                                //}
                                utilityviewmodel.token = utilityviewmodel.token.Substring(9);
                            }

                            int currency_index = utilityviewmodel.token.IndexOf(utilityviewmodel.bill_currency_symbol);
                            if (currency_index >= 0)
                            {
                                ACCOUNT_TYPE = utilityviewmodel.token.Substring(0, currency_index);
                                ACCOUNT_TYPE = ACCOUNT_TYPE.Replace("C", " C");
                                ACCOUNT_TYPE = ACCOUNT_TYPE.Replace("S", " S");
                                ACCOUNT_TYPE = ACCOUNT_TYPE.Replace("R", " R");
                                ACCOUNT_TYPE = ACCOUNT_TYPE.Replace("-", " - ");
                                ACCOUNT_TYPE = CREDIT_DATE + SmartParametersV2016.space + ACCOUNT_TYPE;
                                string account_amount = utilityviewmodel.token.Substring(currency_index);
                                string temps;// = "";
                                if (!SmartParseV2016.Currency_Strip(utilityviewmodel.bill_currency_symbol, utilityviewmodel.bill_currency_separator, utilityviewmodel.bill_thousands_separator, account_amount, utilityviewmodel))
                                {
                                    return false;
                                }
                                else
                                {
                                    temps = utilityviewmodel.value;
                                }
                                if (!SmartParseV2016.Generic_Parse_Integer(temps, utilityviewmodel))
                                {
                                    return false;
                                }
                                else
                                {
                                    ACCOUNT_AMOUNT = utilityviewmodel.genericTransactionValue;
                                }
                                if (!SmartParseV2016.Generic_Parse_Datetime(utilityviewmodel.BILL_PERIOD_END, utilityviewmodel))
                                {
                                    return false;
                                }
                                else
                                {
                                    ACCOUNT_DATE = utilityviewmodel.genericTargetDate;
                                }
                                // add it in unless its zero
                                if ((ACCOUNT_DATE != SmartParametersV2016.defaultDate) &&
                                    (ACCOUNT_AMOUNT != 0))
                                {
                                    INCLUDE_BILLS = SmartParametersV2016.yesFlag;

                                    utilityviewmodel.ACCOUNT_CHARGES_CREDITS_ITEM = (short)(utilityviewmodel.ACCOUNT_CHARGES_CREDITS_ITEM + 1);
                                    bool is_it_there = false;
                                    {
                                        if (string.IsNullOrEmpty(sections[utilityviewmodel.currentIndex][2]))
                                        {
                                            is_it_there = SmartParseV2016.Check_Adjustment_Is_There(ourviewmodel,
                                                                                                    utilityviewmodel,
                                                                                                    ACCOUNT_DATE,
                                                                                                    utilityviewmodel.ACCOUNT_CHARGES_CREDITS_ITEM,
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
                                                                                utilityviewmodel.ACCOUNT_CHARGES_CREDITS_ITEM,
                                                                                ACCOUNT_TYPE,
                                                                                ACCOUNT_VAT_CODE,
                                                                                ACCOUNT_AMOUNT,
                                                                                INCLUDE_BILLS);
                                        SmartParseV2016.Update_Bill(ourviewmodel, utilityviewmodel, "ACCOUNT_CHARGES_CREDITS", ACCOUNT_AMOUNT.ToString());
                                    }
                                }
                            }
                        }
                        sub_line_count++;
                    }
                    line_count = sub_line_count;
                    goto the_old_daysG1;
                }
            the_old_daysG1:
                continue;
            }
            return true;
        }
        private static async Task<bool> Parse_SectionH0(MainViewModel ourviewmodel,
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
            string tariff_title1 = "TariffName-",
                    tariff_title2 = "Tariffname:",
                    tariff_title3 = "Tariffname";

            //int line_count;// = 0;
            for (int line_count = Convert.ToInt32(sections[utilityviewmodel.currentIndex][4]); line_count <= Convert.ToInt32(sections[utilityviewmodel.currentIndex][5]); line_count++)
            {
                utilityviewmodel.token = lines[line_count].Replace(SmartParametersV2016.space, "");
                if (SmartParseV2016.Token_Identify(utilityviewmodel, tariff_title1, false) ||
                    SmartParseV2016.Token_Identify(utilityviewmodel, tariff_title2, false) ||
                    SmartParseV2016.Token_Identify(utilityviewmodel, tariff_title3, false))
                {
                    // Well, I WAS THINKING but now I have been interrupted by CHURNING
                    if (string.IsNullOrEmpty(utilityviewmodel.TARIFF_NAME))
                    {
                        // Try and clean up 'multiple' Tariffs on a line
                        // We must assume that for Elec and Gas the tariff is the same ... (gulp)
                        if (!Clean_Token_Tariffs(ourviewmodel,
                                                    utilityviewmodel,
                                                    tariff_title1,
                                                    tariff_title2,
                                                    tariff_title3))
                        {
                            return false;
                        }

                        // New designator from the FU cunts
                        // Sometimes the title2 on the rhs is title3 on the lhs!!!
                        utilityviewmodel.token = utilityviewmodel.token.Replace(tariff_title3, "");
                        utilityviewmodel.token = utilityviewmodel.token.Replace("ebill", "");
                        utilityviewmodel.TARIFF_NAME = utilityviewmodel.token;

                        if (string.IsNullOrEmpty(utilityviewmodel.TARIFF_NAME))
                        {
                            // Try the NEXT line 
                            int sub_line_count = line_count;
                            while (sub_line_count <= Convert.ToInt32(sections[utilityviewmodel.currentIndex][5]))
                            {
                                utilityviewmodel.token = lines[sub_line_count].Replace(SmartParametersV2016.space, "");
                                if (Set_Tariff_Name(utilityviewmodel, utilityviewmodel.token, tariff_title2))
                                {
                                    break;
                                }
                                sub_line_count++;
                            }
                            line_count = sub_line_count;
                        }
                        utilityviewmodel.TARIFF_NAME = utilityviewmodel.TARIFF_NAME.Replace(SmartParametersV2016.space, "");

                        if (!await Check_Tariff_Name(ourviewmodel, utilityviewmodel))
                        {
                            return false;
                        }
                    }
                    goto the_old_daysH0;
                }
                // Well, I WAS THINKING but now I have been interrupted by CHURNING

                // Now ignore this 'Supply Number' line
                // I don't see the point of this switch anymore ...?
                // Well .. that's because you're a DICKHEAD
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
                //    line_count = Convert.ToInt32(sections[section_index][5]);
                //    goto the_old_daysH0;
                //}

                if (!Check_Bill_Version(ourviewmodel, utilityviewmodel, utilityviewmodel.token))
                {
                    return false;
                }
            //utilityviewmodel.token = utilityviewmodel.token;
            // Her name ISN'T Billie Wilder you fucking numbskull its Bille WHITELAW  .. what a fucking moron
            // AND there is NO 'T' at the end of the word D-E-L-I-C-A-T-E-S-S-A-N  <= there is NO FUCKING T HERE

            // AND THERE IS NO FUCKING 'T' at the end of the word TERRACED you fucking moron 
            the_old_daysH0:
                continue;
            }
            return true;
        }

        private static bool Check_Bill_Version(MainViewModel ourviewmodel,
                                                UtilityViewModel utilityviewmodel,
                                                string token)
        {
            switch (utilityviewmodel.bill_version)
            {
                case 0:
                    break;
                case 1:
                    if (SmartParseV2016.Token_Identify(utilityviewmodel, "Paymentmethod:", true))
                    {
                        string PAYMENT_TYPE = token; // Good - don't change
                                                     // Now try and lookup the 'temporary plan' and the uncompressed payment type
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
                    break;
                default:
                    break;
            }
            utilityviewmodel.token = token;
            return true;
        }
        private static bool Parse_SectionI0(UtilityViewModel utilityviewmodel,
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
            // Not perfect but not a bad start

            //int line_count;// = 0;
            for (int line_count = Convert.ToInt32(sections[utilityviewmodel.currentIndex][4]); line_count <= Convert.ToInt32(sections[utilityviewmodel.currentIndex][5]); line_count++)
            {
                //    token = lines[line_count].Replace(SmartParametersV2016.space, "");


                // Now ignore this 'Supply Number' line
                // I don't see the point of this switch anymore ...?
                // Well .. that's because you're a DICKHEAD
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
                //    line_count = Convert.ToInt32(sections[section_index][5]);
                //    goto the_old_daysI0;
                //}
                //the_old_daysI0:
                //continue;
            }
            return true;
        }
        private static bool Parse_SectionJ1(MainViewModel ourviewmodel,
                                            UtilityViewModel utilityviewmodel,
                                            ref int line_count,
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

            // Not perfect but not a bad start
            string
                    total_units = "Totalunits",
                    electricity_charges = "ElectricityCharges",     // Stop with "Total supply charges"
                    Your_total_new_electricity_charges = "Yourtotalnewelectricitycharges",  // Stops with "Total supply charges"
                    vat = "VAT@",
                    plus_vat_at = "PlusVATat",
                    Electricity_Statement = "ElectricityStatement",
                    Electricity_statement = "Electricitystatement",
                    Electricity_Readings = "ElectricityReadings",   // Stops with "Usage Charge"
                    Electricity_readings = "Electricityreadings";   // Stops with "Usage charge"

            char UNITS_TIME = SmartParametersV2016.daytimeUnit;

            // Now I can't think because she wont fucking shut up
            //int line_count;// = 0;
            for (int line_count1 = Convert.ToInt32(sections[utilityviewmodel.currentIndex][4]); line_count1 <= Convert.ToInt32(sections[utilityviewmodel.currentIndex][5]); line_count1++)
            {
                utilityviewmodel.token = lines[line_count1].Replace(SmartParametersV2016.space, "");

                if (SmartParseV2016.Token_Identify(utilityviewmodel, Electricity_Statement, true) ||
                    SmartParseV2016.Token_Identify(utilityviewmodel, Electricity_statement, true))
                {
                    if (string.IsNullOrEmpty(utilityviewmodel.token))
                    {
                        // No account number!
                        SmartParseV2016.Update_Bill(ourviewmodel, utilityviewmodel, "RESOURCE_ACCOUNT_NO", utilityviewmodel.ACCOUNT_NO);
                    }
                    goto the_old_daysJ1A;
                }

                if (SmartParseV2016.Token_Identify(utilityviewmodel, Electricity_Readings, true) ||
                    SmartParseV2016.Token_Identify(utilityviewmodel, Electricity_readings, true))
                {
                    string finish1 = "",
                            finish2 = "";
                    switch (utilityviewmodel.bill_version)
                    {
                        case 0:
                            finish1 = total_units;    // Look for a line with 'Total units' in it
                            finish2 = total_units;
                            break;
                        case 1:
                            utilityviewmodel.token = utilityviewmodel.token.Replace("forMeter", "");
                            utilityviewmodel.token = utilityviewmodel.token.Replace("formeter", "");
                            utilityviewmodel.METER_SERIAL_NO = utilityviewmodel.token;
                            finish1 = "UsageCharge";   // Look for a line with 'Usage Charge' in it
                            finish2 = "Usagecharge";    // Look for a line with 'Usage charge' in it
                            utilityviewmodel.UNITS_TYPE = "Single Rate";
                            break;
                        default:
                            break;
                    }

                    if (!Check_SectionJ1_First_Part(ourviewmodel,
                                                    utilityviewmodel,
                                                    ref line_count,
                                                    sections,
                                                    utilityviewmodel.currentIndex,
                                                    lines,
                                                    finish1,
                                                    finish2,
                                                    UNITS_TIME))
                    {
                        return false;
                    }
                    goto the_old_daysJ1A;
                }
            the_old_daysJ1A:
                continue;
            }

            // How the fuck does THIS work?  can I modify the index in a for loop
            for (int line_count1 = Convert.ToInt32(sections[utilityviewmodel.currentIndex][4]); line_count1 <= Convert.ToInt32(sections[utilityviewmodel.currentIndex][5]); line_count1++)
            {
                utilityviewmodel.token = lines[line_count1].Replace(SmartParametersV2016.space, "");

                if (SmartParseV2016.Token_Identify(utilityviewmodel, electricity_charges, true) ||
                    SmartParseV2016.Token_Identify(utilityviewmodel, Your_total_new_electricity_charges, true))
                {
                    if (!Check_SectionJ1_Electricity_Charges(ourviewmodel,
                                                            utilityviewmodel,
                                                            ref line_count,
                                                            sections,
                                                            utilityviewmodel.currentIndex,
                                                            lines,
                                                            UNITS_TIME))
                    {
                        return false;
                    }
                    //// I am SO zany and witty and funny!
                    goto the_old_daysJ1B;
                }

                // Last Tuesday, the petulant aggressive ignorant charmless cow was stomping about
                // the bedroom effing and blinding as usual.  And then she lays on the bed 'thumping'
                // her fucking leg like 'Thumper' ... what the fuck is she doing?  I don't know and I don't care
                // I just think she is a rude mannerless uneducated moron who does her best to wake me up
                // ODIBF  ODSBD RTB3

                // Now try and get all the Readings Lines
                if (SmartParseV2016.Token_Identify(utilityviewmodel, vat, true) ||
                    SmartParseV2016.Token_Identify(utilityviewmodel, plus_vat_at, true))
                {
                    if (!Check_SectionJ1_VAT(ourviewmodel, utilityviewmodel, utilityviewmodel.token))
                    {
                        return false;
                    }
                    //utilityviewmodel.token = utilityviewmodel.token;
                    goto the_old_daysJ1B;
                }

                if (SmartParseV2016.Token_Identify_Middle(utilityviewmodel, "Yourelectricityfueldiscountfortheperiodupto", true, ""))
                {
                    if (!Check_SectionJ1_Electricity_Discount(ourviewmodel,
                                                            utilityviewmodel,
                                                            ref line_count,
                                                            sections,
                                                            utilityviewmodel.currentIndex,
                                                            lines))
                    {
                        return false;
                    }
                    goto the_old_daysJ1B;
                }

            // Now .. the stupid bitch after BEING TOLD that 2kg is about 4.5lbs
            // went and cooked the fucking ham based on a time for 4kg !!!!
            // MAFFS isn't her strongest suit!!!

            the_old_daysJ1B:
                continue;
                // Last Tuesday, the petulant aggressive ignorant charmless cow was stomping about
                // the bedroom effing and blinding as usual.  And then she lays on the bed 'thumping'
                // her fucking leg like 'Thumper' ... what the fuck is she doing?  I don't know and I don't care
                // I just think she is a rude mannerless uneducated moron who does her best to wake me up
                // ODIBF  ODSBD RTB3
            }
            // And after stupidly filling her diesel car up with petrol for the 3rd? 4th? time she then goes
            // and fills it up without paying for it!  Is she FUCKING DUMB or what?
            return true;
        }

        private static bool Check_SectionJ1_First_Part(MainViewModel ourviewmodel,
                                                            UtilityViewModel utilityviewmodel,
                                                            ref int line_count,
                                                            string[][] sections,
                                                            int section_index,
                                                            string[] lines,
                                                            string finish1,
                                                            string finish2,
                                                            char UNITS_TIME)
        {
            string kWh = "kWh";
            int sub_line_count = line_count;
            while (sub_line_count <= Convert.ToInt32(sections[section_index][5]))
            {
                utilityviewmodel.token = lines[sub_line_count].Replace(SmartParametersV2016.space, "").Trim();
                if ((utilityviewmodel.token.IndexOf(finish1) >= 0) ||
                    (utilityviewmodel.token.IndexOf(finish2) >= 0))
                {
                    break;
                }
                if (utilityviewmodel.token.IndexOf(kWh) >= 0)
                {
                    utilityviewmodel.UNIT_OF_MEASURE = kWh;
                }
                else
                {
                    // I WAS RIGHT - DUMB BITCH CHURNED ABOUT THE BINS!!! She came ALL THE WAY donwstairs for a churn!
                    // This is a potential readings line
                    if (utilityviewmodel.token.IndexOf("/") >= 0)    // Look for a line with 'dates' in it
                    {
                        utilityviewmodel.READINGS_PERIOD_START = SmartParametersV2016.defaultDates;
                        utilityviewmodel.READINGS_PERIOD_END = SmartParametersV2016.defaultDates;
                        utilityviewmodel.D_THIS_READ = "0";
                        utilityviewmodel.D_LAST_READ = "0";
                        utilityviewmodel.D_UNITS_USED = "0";
                        string N_THIS_READ = "0",
                                N_LAST_READ = "0",
                                N_UNITS_USED = "0";
                        utilityviewmodel.METER_TYPE = "";
                        utilityviewmodel.FROM_TYPE = "";
                        utilityviewmodel.READ_TYPE = "";
                        utilityviewmodel.UNITS_RATE = "0";
                        utilityviewmodel.UNITS_COST = "0";
                        // Assume its a line with 'dates' in it
                        utilityviewmodel.token = lines[sub_line_count];
                        utilityviewmodel.token = SmartParseV2016.Remove_Double_Spaces_V3(utilityviewmodel.token);
                        switch (utilityviewmodel.bill_version)
                        {
                            case 0:
                                if (!VersionA_line_split(utilityviewmodel))
                                {
                                    return false;
                                }
                                if (utilityviewmodel.READINGS_PERIOD_START == SmartParametersV2016.defaultDates)
                                {
                                    utilityviewmodel.READINGS_PERIOD_START = utilityviewmodel.BILL_PERIOD_START;
                                }
                                break;
                            case 1:
                                utilityviewmodel.token = utilityviewmodel.token.Replace(utilityviewmodel.bill_currency_symbol + SmartParametersV2016.space, utilityviewmodel.bill_currency_symbol); // V2 So there is no space after the pound
                                utilityviewmodel.M3 = "";
                                if (!VersionB_line_split(utilityviewmodel))             // Not set for E
                                {
                                    return false;
                                }
                                break;
                            default:
                                break;
                        }
                        // Version A: Well ... we could wait until the "Total units" line
                        // and then update all the records with "kWh" but ... what the fuck?
                        // Electricity is ALWAYS measured in kWh so default it in
                        if (string.IsNullOrEmpty(utilityviewmodel.UNIT_OF_MEASURE))
                        {
                            utilityviewmodel.UNIT_OF_MEASURE = kWh;
                        }
                        if (SmartParseV2016.Add_Readings_Test(SmartParametersV2016.defaultDate,
                                                                SmartTimeV2016.ConvertDateTime(utilityviewmodel.READINGS_PERIOD_START),
                                                                SmartTimeV2016.ConvertDateTime(utilityviewmodel.READINGS_PERIOD_END),
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
                                                                    utilityviewmodel.READINGS_PERIOD_END,  // Needed!!
                                                                    utilityviewmodel.METER_SERIAL_NO,
                                                                    utilityviewmodel.READ_TYPE,
                                                                    utilityviewmodel.D_LAST_READ,
                                                                    utilityviewmodel.D_THIS_READ,
                                                                    utilityviewmodel.D_UNITS_USED,
                                                                    N_LAST_READ,
                                                                    N_THIS_READ,
                                                                    N_UNITS_USED,
                                                                    utilityviewmodel.UNIT_OF_MEASURE); // kWh, m3 or ft3
                        }
                        switch (utilityviewmodel.bill_version)
                        {
                            case 0:
                                break;
                            case 1:
                                //int temp = 0;
                                if (!SmartParseV2016.Generic_Parse_Integer(utilityviewmodel.UNITS_COST, utilityviewmodel))
                                {
                                    return false;
                                }
                                //else
                                //{
                                //    temp = utilityviewmodel.genericTransactionValue;
                                //}

                                // Well this used to say 'Always Single Rate'
                                //                       'Version B'
                                //                       units_band = 1
                                // But now there is a situation like this for Bill 29-Apr-2015
                                // for the Unit Charges:
                                // 
                                // 27/03/15 S 20478.000 28/03/15 E 20491.969 13.969 0.21242 £ 2.97
                                // 28/03/15 E 20491.969 31/03/15 E 20520.631 28.662 0.21242 £ 6.09
                                // 31/03/15 E 20520.631 28/04/15 S 20902.000 381.369 0.20682 £ 78.87
                                //
                                // with the Standing Charges thus:
                                //
                                // Standing Charge Single Rate - Elec 29 days @ 4.76p per day £ 1.38
                                // Standing Charge Single Rate - Elec 2 days @ 1.69p per day £ 0.03
                                //
                                // Whilst it is pretty obvious that the Standing Charges are in the wrong order
                                // (just by looking at the days they cover) there are other problems viz:
                                // the Unit Charges days cover 27-Mar-2015 to 28-Apr-2015 which (according to
                                // Excel) is 32 days and not 31 days (from 29 + 2).
                                //
                                // So I have to think of UNITS_BAND differently and now it has to be based on
                                // whether the UNITS_RATE changes and *not* on how many lines I have for
                                // the UNITS_RATE.
                                //
                                // So I need to record the Unit Charges thus:
                                //
                                // 27/03/15 S 20478.000 28/03/15 E 20491.969 13.969 0.21242 £ 2.97   UNITS_BAND 1 (first entry)
                                // 28/03/15 E 20491.969 31/03/15 E 20520.631 28.662 0.21242 £ 6.09   UNITS_BAND 1 (no rate change)
                                // 31/03/15 E 20520.631 28/04/15 S 20902.000 381.369 0.20682 £ 78.87 UNITS_BAND 2 (rate has changed)
                                //
                                // With the Standing Charges now done in REVERSE ORDER (a kludge)
                                //
                                // Standing Charge Single Rate - Elec 2 days @ 1.69p per day £ 0.03  CHARGES_ITEM 1
                                // Standing Charge Single Rate - Elec 29 days @ 4.76p per day £ 1.38 CHARGES_ITEM 2
                                // 
                                // Now when I come to form the TariffDetails I wil be able to match
                                // a Standing Charge for a Unit Rate ...


                                // Check to see if we use the previous UNITS_BAND or make a new one
                                utilityviewmodel.units_band = SmartParseV2016.Determine_Units_Band(ourviewmodel,
                                                                                    utilityviewmodel,
                                                                                    UNITS_TIME,
                                                                                    utilityviewmodel.UNITS_RATE,
                                                                                    utilityviewmodel.units_band);
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
                                                                        utilityviewmodel.UNITS_TYPE,
                                                                        utilityviewmodel.D_UNITS_USED,
                                                                        utilityviewmodel.UNITS_RATE,
                                                                        utilityviewmodel.UNIT_OF_MEASURE,
                                                                        utilityviewmodel.UNITS_COST);
                                break;
                            default:
                                break;
                        }
                        utilityviewmodel.READINGS_PERIOD_END = SmartTimeV2016.ConvertDateTime(utilityviewmodel.READINGS_PERIOD_END).AddDays(1).ToString();
                    }
                }
                sub_line_count++;
            }
            line_count = sub_line_count;
            return true;
        }

        private static bool Check_SectionJ1_Electricity_Charges(MainViewModel ourviewmodel,
                                                            UtilityViewModel utilityviewmodel,
                                                            ref int line_count,
                                                            string[][] sections,
                                                            int section_index,
                                                            string[] lines,
                                                            char UNITS_TIME)
        {
            string READINGS_PERIOD_START = SmartParametersV2016.defaultDates,
                        READINGS_PERIOD_END = SmartParametersV2016.defaultDates;
            string total_supply_charges = "Totalsupplycharges",
                    total_suppy_charges = "Totalsuppycharges",      // !!!!
                    vB_resource = "-Elec",
                    standing_charge = "StandingCharge",
                    VA_electricity_supply_standing_charge = "Electricitysupplystandingcharge",
                    VA_electricity_total_unit_charge = "Electricitytotalunitcharge";

            short supplier_code = utilityviewmodel.supplier_code;
            short brand_code = utilityviewmodel.brand_code;
            string account_no = utilityviewmodel.ACCOUNT_NO;
            DateTime created = utilityviewmodel.CREATED;
            string bill_number = utilityviewmodel.STATEMENT_ID;
            DateTime bill_date = SmartTimeV2016.ConvertDateTime(utilityviewmodel.BILL_DATE);

            List<SmartUtility.EReadings> e_readings_found =
                    new List<SmartUtility.EReadings>(from E_Reading
                    in utilityviewmodel.Hezbollah.e_readings_changesList
                                                     where ((E_Reading.CUBEFACE_CODE == SmartParametersV2016.Utility) &&
                                                             (E_Reading.SUPPLIER_CODE == supplier_code) &&
                                                             (E_Reading.BRAND_CODE == brand_code) &&
                                                             (E_Reading.ACCOUNT_NO == account_no) &&
                                                             (E_Reading.ACCOUNT_CREATED == created) &&
                                                             (E_Reading.STATEMENT_ID == bill_number) &&
                                                             (E_Reading.BILL_DATE == bill_date))
                                                     orderby E_Reading.READINGS_PERIOD_END ascending
                                                     select E_Reading);

            // THIS COULD BE WRONG!!!!

            // Extract the date range (if you can)
            if (!VAB_charges_split(utilityviewmodel, utilityviewmodel.token))
            {
                // Uh - oh FU Foreigners now don't put the date range in the heading
                // We will have to use the readings ...
                if (e_readings_found.Count > 0)
                {
                    utilityviewmodel.READINGS_PERIOD_START = e_readings_found[0].READINGS_PERIOD_START.ToString();
                    utilityviewmodel.READINGS_PERIOD_END = e_readings_found[e_readings_found.Count - 1].READINGS_PERIOD_END.ToString();
                }
            }
            if ((utilityviewmodel.READINGS_PERIOD_START == SmartParametersV2016.defaultDates) ||
                (utilityviewmodel.READINGS_PERIOD_END == SmartParametersV2016.defaultDates))
            {
                ourviewmodel.errorMessage = "Cannot find or fix Standing Charge dates";
                return false;
            }

            // The Closing Date is also the Discount Date IF we can't find it

            // Now .. the OPENING_READ_DATE (!!) is ONE day ahead of what it should be
            // A-cos 01May2012 - 31May2012 is 31 days and *not* 30.
            // If you subtract 31 - 1 you get 30 ...but the Standing Charges days are 31!
            // So adjust the OPENING_READ_DATE 'back' by 1 day
            // No don't - use Excel to find out the days between dates: 01/05/2012 to 31/05/2012 is 30 days!

            // Update any Readings we have with missing info
            foreach (SmartUtility.EReadings e_readings_row in e_readings_found)
            {
                if (string.IsNullOrEmpty(e_readings_row.UNIT_OF_MEASURE))
                {
                    e_readings_row.UNIT_OF_MEASURE = utilityviewmodel.UNIT_OF_MEASURE;
                }
            }

            int sub_line_count = line_count;
            while (sub_line_count <= Convert.ToInt32(sections[section_index][5]))
            {
                utilityviewmodel.token = lines[sub_line_count].Replace(SmartParametersV2016.space, "").Trim();
                if ((utilityviewmodel.token.IndexOf(total_supply_charges) >= 0) ||
                    (utilityviewmodel.token.IndexOf(total_suppy_charges) >= 0))    // <= Useless paki tit-heads
                {
                    break;
                }

                string trigger = "";
                switch (utilityviewmodel.bill_version)
                {
                    case 0:
                        trigger = VA_electricity_supply_standing_charge;
                        break;
                    case 1:
                        trigger = standing_charge;
                        break;
                    default:
                        break;
                }

                if (utilityviewmodel.token.IndexOf(trigger) >= 0)
                {
                    utilityviewmodel.token = utilityviewmodel.token.Replace(trigger, "");

                    if (utilityviewmodel.token.IndexOf("-Elec.") >= 0)
                    {
                        utilityviewmodel.token = utilityviewmodel.token.Replace("-Elec.", "");
                    }
                    if (utilityviewmodel.token.IndexOf("Elec") >= 0)
                    {
                        utilityviewmodel.token = utilityviewmodel.token.Replace("Elec", "");
                    }
                    string CHARGES_TYPE = "Standing Charge";
                    utilityviewmodel.CHARGES_PERIOD = "";

                    utilityviewmodel.CHARGES_DAYS = "0";
                    utilityviewmodel.STANDING_CHARGE = "0";
                    utilityviewmodel.CHARGES_COST = "0";
                    if (!VersionAB_line_split(utilityviewmodel,
                                                vB_resource))
                    {
                        return false;
                    }
                    SmartParseV2016.Update_Charges_Item(utilityviewmodel);
                    utilityviewmodel.charges_item = 1;   // Always safe because of the above line

                    string STANDING_CHARGES_PERIOD_START = READINGS_PERIOD_START;
                    string STANDING_CHARGES_PERIOD_END = READINGS_PERIOD_END;

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
                                                                    utilityviewmodel.charges_item.ToString(),
                                                                    CHARGES_TYPE,
                                                                    utilityviewmodel.STANDING_CHARGE,
                                                                    utilityviewmodel.CHARGES_DAYS,
                                                                    utilityviewmodel.CHARGES_COST);
                }

                // Cutting down on cakes, biscuits ... CHURNING and WITTERING
                if (utilityviewmodel.token.IndexOf(VA_electricity_total_unit_charge) >= 0)
                {
                    utilityviewmodel.token = utilityviewmodel.token.Replace(VA_electricity_total_unit_charge, "");
                    // Version 1
                    utilityviewmodel.UNITS_TYPE = "";
                    utilityviewmodel.UNITS = "0";
                    utilityviewmodel.UNITS_RATE = "0";
                    utilityviewmodel.UNITS_COST = "0";
                    if (!VersionAB_units_split(utilityviewmodel))
                    {
                        return false;
                    }

                    if (!string.IsNullOrEmpty(READINGS_PERIOD_START) &&
                        !string.IsNullOrEmpty(READINGS_PERIOD_END))
                    {
                        // Check to see if we use the previous UNITS_BAND or make a new one
                        utilityviewmodel.units_band = SmartParseV2016.Determine_Units_Band(ourviewmodel,
                                                                            utilityviewmodel,
                                                                            UNITS_TIME,
                                                                            utilityviewmodel.UNITS_RATE,
                                                                            utilityviewmodel.units_band);

                        string UNIT_CHARGES_PERIOD_START = READINGS_PERIOD_START;
                        string UNIT_CHARGES_PERIOD_END = READINGS_PERIOD_END;

                        // Version 1
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
                                                                    UNIT_CHARGES_PERIOD_END,
                                                                    UNITS_TIME,
                                                                    utilityviewmodel.units_band.ToString(),
                                                                    utilityviewmodel.UNITS_TYPE,
                                                                    utilityviewmodel.UNITS,
                                                                    utilityviewmodel.UNITS_RATE,
                                                                    utilityviewmodel.UNIT_OF_MEASURE,
                                                                    utilityviewmodel.UNITS_COST);
                    }
                }
                sub_line_count++;
            }
            line_count = sub_line_count;
            return true;
        }

        private static bool Check_SectionJ1_VAT(MainViewModel ourviewmodel,
                                                UtilityViewModel utilityviewmodel,
                                                string token)
        {
            int percent = token.IndexOf("%");
            if (percent >= 0)
            {
                string VAT_RATE = token.Substring(0, percent).Trim();
                token = token.Substring(percent + 1);
                string VAT_AMOUNT;// = "";
                if (!SmartParseV2016.Currency_Strip(utilityviewmodel.bill_currency_symbol, utilityviewmodel.bill_currency_separator, utilityviewmodel.bill_thousands_separator, token, utilityviewmodel))
                {
                    return false;
                }
                else
                {
                    VAT_AMOUNT = utilityviewmodel.value;
                }
                //int temp = 0;
                if (!SmartParseV2016.Generic_Parse_Integer(VAT_AMOUNT, utilityviewmodel))
                {
                    return false;
                }
                //else
                //{
                //    temp = utilityviewmodel.genericTransactionValue;
                //}
                SmartParseV2016.Update_Bill(ourviewmodel, utilityviewmodel, "RESOURCE_VAT_AMOUNT", VAT_AMOUNT);

                string target_rate = VAT_RATE;
                int has_decimal = target_rate.IndexOf(SmartParametersV2016.decimalPoint);
                if (has_decimal < 0)
                {
                    target_rate += ".00";
                }
                short VAT_CODE;// = SmartParametersV2016.zeroRateVatCode;
                if (!SmartParseV2016.Lookup_Vat_Code(target_rate, SmartTimeV2016.ConvertDateTime(utilityviewmodel.BILL_DATE), ourviewmodel.Blanche.vatRatesList, utilityviewmodel))
                {
                    return false;
                }
                else
                {
                    VAT_CODE = utilityviewmodel.vat_code;
                }
                SmartParseV2016.Update_Bill(ourviewmodel, utilityviewmodel, "RESOURCE_VAT_CODE", VAT_CODE.ToString());
            }
            utilityviewmodel.token = token;
            return true;
        }

        private static bool Check_SectionJ1_Electricity_Discount(MainViewModel ourviewmodel,
                                                            UtilityViewModel utilityviewmodel,
                                                            ref int line_count,
                                                            string[][] sections,
                                                            int section_index,
                                                            string[] lines)
        {
            string on_your = "onyour";

            if (utilityviewmodel.token.IndexOf(SmartParametersV2016.period) >= 0)
            {
                // Find the next line to end in period
                // Try the NEXT line!!!
                int sub_line_count = line_count + 1;
                while (sub_line_count <= Convert.ToInt32(sections[section_index][5]))
                {
                    utilityviewmodel.token += lines[sub_line_count].Replace(SmartParametersV2016.space, "").Trim();
                    // Is the last character a period ?
                    if (utilityviewmodel.token.Substring(utilityviewmodel.token.Length - 1, 1) == SmartParametersV2016.period)
                    {
                        utilityviewmodel.token = utilityviewmodel.token.Replace(SmartParametersV2016.period, "");
                        break;
                    }
                    sub_line_count++;
                }
                line_count = sub_line_count;
            }

            int on_your_index = utilityviewmodel.token.IndexOf(on_your);
            if (on_your_index >= 0)
            {
                utilityviewmodel.token = utilityviewmodel.token.Substring(0, on_your_index);
                utilityviewmodel.token = utilityviewmodel.token.Replace(on_your, "");

                int bill_index = utilityviewmodel.token.IndexOf("bill");
                if (bill_index >= 0)
                {
                    string DISCOUNT_CREDIT_DATE = utilityviewmodel.token.Substring(0, bill_index).Trim();
                    DISCOUNT_CREDIT_DATE = "01" + DISCOUNT_CREDIT_DATE;
                    //DateTime temp_date = SmartParametersV2016.defaultDate;
                    if (!SmartParseV2016.Generic_Parse_Datetime(DISCOUNT_CREDIT_DATE, utilityviewmodel))
                    {
                        return false;
                    }
                    //else
                    //{
                    //    temp_date = utilityviewmodel.genericTargetDate;
                    //}
                    SmartParseV2016.Update_Bill(ourviewmodel, utilityviewmodel, "DISCOUNT_CREDIT_DATE", DISCOUNT_CREDIT_DATE);
                }
            }
            return true;
        }
        private static bool Parse_SectionJ2(MainViewModel ourviewmodel,
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

            // Now I can't think because she wont fucking shut up
            //string token = "";

            // "M Metric meter - Units are measured in cubic meters (m3)."
            // "I Imperial meter - Units are measured in 100's of cubic feet (ft3).  To convert to m3 multiply by 2.83."

            // Not perfect but not a bad start
            string UNITS_TYPE = "";
            utilityviewmodel.UNIT_OF_MEASURE = "";
            utilityviewmodel.units_band = 0;
            utilityviewmodel.charges_item = 0;
            // I am going to do a bit of 'cleansing' here for Gas ...
            // If the last character on a line is lowercase 'm' and the next AND ONLY character
            // on the following line is a '3' then assume this is 'm3' and move the '3' up to
            // the previous line at the end, and clear the line it is on
            //int line_count;// = 0;
            utilityviewmodel.lines = lines;
            for (int line_count = Convert.ToInt32(sections[utilityviewmodel.currentIndex][4]); line_count <= Convert.ToInt32(sections[utilityviewmodel.currentIndex][5]); line_count++)
            {
                SectionJ0_Preamble(utilityviewmodel, line_count);
            }
            lines = utilityviewmodel.lines;
            // Now I can't think because she wont fucking shut up
            // First Pass - do the Supply Number
            if (!SectionJ2_First_Pass(ourviewmodel,
                                        utilityviewmodel,
                                        sections,
                                        utilityviewmodel.currentIndex,
                                        lines))
            {
                return false;
            }

            // Second Pass - do the Supply Number
            if (!SectionJ2_Second_Pass(ourviewmodel,
                                        utilityviewmodel,
                                        sections,
                                        utilityviewmodel.currentIndex,
                                        lines))
            {
                return false;
            }

            // Third Pass - do the Supply Number
            if (!SectionJ2_Third_Pass(ourviewmodel,
                                      utilityviewmodel,
                                      sections,
                                      utilityviewmodel.currentIndex,
                                      lines,
                                      utilityviewmodel.units_band,
                                      UNITS_TYPE,
                                      utilityviewmodel.UNIT_OF_MEASURE))
            {
                return false;
            }
            // And after stupidly filling her diesel car up with petrol for the 3rd? 4th? time she then goes
            // and fills it up without paying for it!  Is she FUCKING DUMB or what?
            return true;
        }

        private static bool SectionJ2_First_Pass(MainViewModel ourviewmodel,
                                                    UtilityViewModel utilityviewmodel,
                                                    string[][] sections,
                                                    int section_index,
                                                    string[] lines)
        {
            string m3 = "m3";

            utilityviewmodel.READINGS_PERIOD_START = SmartParametersV2016.defaultDates;
            utilityviewmodel.READINGS_PERIOD_END = SmartParametersV2016.defaultDates;
            utilityviewmodel.METER_TYPE = "";
            utilityviewmodel.FROM_TYPE = "";
            utilityviewmodel.READ_TYPE = "";
            utilityviewmodel.D_LAST_READ = "0";
            utilityviewmodel.D_THIS_READ = "0";
            utilityviewmodel.D_UNITS_USED_M3 = "0";
            utilityviewmodel.D_UNITS_USED_KWH = "0";
            utilityviewmodel.UNIT_OF_MEASURE = "";
            utilityviewmodel.UNITS_COST = "0";
            utilityviewmodel.UNITS_RATE = "0";

            string total_units = "Totalm3",
                    Gas_Statement = "GasStatement",
                    Gas_statement = "Gasstatement",
                    Gas_Readings = "GasReadings",
                    Gas_readings = "Gasreadings";

            //int line_count;// = 0;
            // Second Pass - do the Supply Number
            for (int line_count = Convert.ToInt32(sections[section_index][4]); line_count <= Convert.ToInt32(sections[section_index][5]); line_count++)
            {
                utilityviewmodel.token = lines[line_count].Replace(SmartParametersV2016.space, "");
                if (SmartParseV2016.Token_Identify(utilityviewmodel, Gas_Statement +
                                                                SmartParametersV2016.bar +
                                                                Gas_statement, true))
                {
                    if (string.IsNullOrEmpty(utilityviewmodel.token))
                    {
                        // No account number!
                        SmartParseV2016.Update_Bill(ourviewmodel, utilityviewmodel, "RESOURCE_ACCOUNT_NO", utilityviewmodel.ACCOUNT_NO);
                    }
                    goto the_old_daysJ2A;
                }

                // Now ignore this 'Supply Number' line
                // I don't see the point of this switch anymore ...?
                // Well .. that's because you're a DICKHEAD
                //if (SmartParseV2016.Token_Identify(utilityviewmodel, "SupplyNumber" +
                //                                                SmartParametersV2016.bar +
                //                                                "Gassupplynumber", true))
                //{
                //    if (SmartParseV2016.fix_mprn(line_count, lines, sections[section_index][5], utilityviewmodel))
                //    {
                //        SmartUtilityV2022.Amelia_Update_Mpan(utilityviewmodel, utilityviewmodel.mprn);
                //    }
                //    goto the_old_daysJ2A;
                //}

                if (SmartParseV2016.Token_Identify(utilityviewmodel, Gas_Readings +
                                                                SmartParametersV2016.bar +
                                                                Gas_readings, true))
                {
                    string finish = "";
                    switch (utilityviewmodel.bill_version)
                    {
                        case 0:
                            finish = total_units;    // Look for a line with 'Total m3' in it
                            break;
                        case 1:
                            utilityviewmodel.token = utilityviewmodel.token.Replace("forMeter", "");
                            utilityviewmodel.token = utilityviewmodel.token.Replace("formeter", "");
                            utilityviewmodel.METER_SERIAL_NO = utilityviewmodel.token;
                            finish = total_units;
                            utilityviewmodel.UNITS_TYPE = "Single Rate";
                            break;
                        default:
                            break;
                    }
                    int sub_line_count = line_count;
                    while (sub_line_count <= Convert.ToInt32(sections[section_index][5]))
                    {
                        utilityviewmodel.token = lines[sub_line_count].Replace(SmartParametersV2016.space, "").Trim();
                        if (utilityviewmodel.token.IndexOf(finish) >= 0)
                        {
                            break;
                        }
                        if (utilityviewmodel.token.IndexOf(m3) >= 0)
                        {
                            utilityviewmodel.UNIT_OF_MEASURE = m3;
                        }
                        else
                        {
                            // I WAS RIGHT - DUMB BITCH CHURNED ABOUT THE BINS!!! She came ALL THE WAY donwstairs for a churn!
                            // This is a potential readings line
                            if (utilityviewmodel.token.IndexOf("/") >= 0)    // Look for a line with 'dates' in it
                            {
                                utilityviewmodel.READINGS_PERIOD_START =
                                    utilityviewmodel.READINGS_PERIOD_END;
                                utilityviewmodel.D_THIS_READ =
                                    utilityviewmodel.D_LAST_READ =
                                    utilityviewmodel.D_UNITS_USED_M3 =
                                    utilityviewmodel.D_UNITS_USED_KWH = "0";

                                utilityviewmodel.M3 = "";

                                //CUBIC_METRES_USED = VOLUMECORRECTION = KWHCONVERSION = "0";

                                // Assume its a line with 'dates' in it
                                utilityviewmodel.token = lines[sub_line_count];
                                utilityviewmodel.token = SmartParseV2016.Remove_Double_Spaces_V3(utilityviewmodel.token);
                                switch (utilityviewmodel.bill_version)
                                {
                                    case 0:
                                        if (!VersionA_line_split(utilityviewmodel))
                                        {
                                            return false;
                                        }
                                        if (utilityviewmodel.READINGS_PERIOD_START == SmartParametersV2016.defaultDates)
                                        {
                                            utilityviewmodel.READINGS_PERIOD_START = utilityviewmodel.BILL_PERIOD_START;
                                        }
                                        break;
                                    case 1:
                                        utilityviewmodel.token = utilityviewmodel.token.Replace(utilityviewmodel.bill_currency_symbol + SmartParametersV2016.space, utilityviewmodel.bill_currency_symbol); // V2 So there is no space after the pound
                                        if (!VersionB_line_split(utilityviewmodel)) // Not set for G
                                        {
                                            return false;
                                        }
                                        break;
                                    default:
                                        break;
                                }

                                // The Closing Date is also the Discount Date IF we can't find it
                                //  DISCOUNT_DATE = READINGS_PERIOD_END;

                                //  Now .. the OPENING_READ_DATE (!!) is ONE day ahead of what it should be
                                //  A-cos 01May2012 - 31May2012 is 31 days and *not* 30.
                                //  If you subtract 31 - 1 you get 30 ...but the Standing Charges days are 31!
                                //  So adjust the OPENING_READ_DATE 'back' by 1 day

                                // For Version A, the READINGS_PERIOD_START might well be SmartParametersV2016.defaultDates ...
                                if (SmartParseV2016.Add_Readings_Test(SmartParametersV2016.defaultDate, SmartTimeV2016.ConvertDateTime(utilityviewmodel.READINGS_PERIOD_START),
                                                                        SmartTimeV2016.ConvertDateTime(utilityviewmodel.READINGS_PERIOD_END),
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
                                                                            utilityviewmodel.READINGS_PERIOD_END,      // Needed
                                                                            utilityviewmodel.METER_SERIAL_NO,
                                                                            utilityviewmodel.READ_TYPE,
                                                                            utilityviewmodel.D_THIS_READ,
                                                                            utilityviewmodel.D_LAST_READ,
                                                                            utilityviewmodel.D_UNITS_USED_M3,
                                                                            utilityviewmodel.UNIT_OF_MEASURE, // kWh, m3 or ft3
                                                                            utilityviewmodel.D_UNITS_USED_KWH,
                                                                            utilityviewmodel.CALORIFIC_VALUE);    // Zero here for now
                                }
                                utilityviewmodel.READINGS_PERIOD_END = SmartTimeV2016.ConvertDateTime(utilityviewmodel.READINGS_PERIOD_END).AddDays(1).ToString();
                            }
                        }
                        sub_line_count++;
                    }
                    line_count = sub_line_count;
                    goto the_old_daysJ2A;
                }
            the_old_daysJ2A:
                continue;
            }
            return true;
        }

        private static bool SectionJ2_Second_Pass(MainViewModel ourviewmodel,
                                                    UtilityViewModel utilityviewmodel,
                                                    string[][] sections,
                                                    int section_index,
                                                    string[] lines)
        {
            string vat = "VAT@",
                    plus_vat_at = "PlusVATat",
                    standing_charge = "StandingCharge",
                    vA_gas_supply_standing_charge = "Gassupplystandingcharge",
                    gas_charges = "GasCharges",
                    Your_total_new_gas_charges = "Yourtotalnewgascharges"; // Stops with "Total supply charges"
            // Second Pass - do the Supply Number
            for (int line_count = Convert.ToInt32(sections[section_index][4]); line_count <= Convert.ToInt32(sections[section_index][5]); line_count++)
            {
                utilityviewmodel.token = lines[line_count].Replace(SmartParametersV2016.space, "");

                if (SmartParseV2016.Token_Identify(utilityviewmodel, gas_charges +
                                                                SmartParametersV2016.bar +
                                                                Your_total_new_gas_charges, true))
                {
                    // I am SO zany and witty and funny!
                    short supplier_code = utilityviewmodel.supplier_code;
                    short brand_code = utilityviewmodel.brand_code;
                    string account_no = utilityviewmodel.ACCOUNT_NO;
                    DateTime created = utilityviewmodel.CREATED;
                    string bill_number = utilityviewmodel.STATEMENT_ID;
                    DateTime bill_date = SmartTimeV2016.ConvertDateTime(utilityviewmodel.BILL_DATE);

                    List<SmartUtility.GReadings> g_readings_found = new List<SmartUtility.GReadings>(from G_Reading
                                        in utilityviewmodel.Hezbollah.g_readings_changesList
                                                                                                     where ((G_Reading.CUBEFACE_CODE == SmartParametersV2016.Utility) &&
                                                                                                             (G_Reading.SUPPLIER_CODE == supplier_code) &&
                                                                                                             (G_Reading.BRAND_CODE == brand_code) &&
                                                                                                             (G_Reading.ACCOUNT_NO == account_no) &&
                                                                                                             (G_Reading.ACCOUNT_CREATED == created) &&
                                                                                                             (G_Reading.STATEMENT_ID == bill_number) &&
                                                                                                             (G_Reading.BILL_DATE == bill_date))
                                                                                                     orderby G_Reading.READINGS_PERIOD_END ascending
                                                                                                     select G_Reading);
                    // Extract the date range (if you can)
                    if (!VAB_charges_split(utilityviewmodel, utilityviewmodel.token))
                    {
                        // Uh - oh FU Foreigners now don't put the date range in the heading
                        // We will have to use the readings ...
                        if (g_readings_found.Count > 0)
                        {
                            utilityviewmodel.READINGS_PERIOD_START = g_readings_found[0].READINGS_PERIOD_START.ToString();
                            utilityviewmodel.READINGS_PERIOD_END = g_readings_found[g_readings_found.Count - 1].READINGS_PERIOD_END.ToString();
                        }
                    }
                    if ((utilityviewmodel.READINGS_PERIOD_START == SmartParametersV2016.defaultDates) ||
                        (utilityviewmodel.READINGS_PERIOD_END == SmartParametersV2016.defaultDates))
                    {
                        ourviewmodel.errorMessage = "Cannot find or fix Standing Charge dates";
                        return false;
                    }

                    // Now .. the OPENING_READ_DATE (!!) is ONE day ahead of what it should be
                    // A-cos 01May2012 - 31May2012 is 31 days and *not* 30.
                    // If you subtract 31 - 1 you get 30 ...but the Standing Charges days are 31!
                    // So adjuest the OPENING_READ_DATE 'back' by 1 day

                    // Update any Readings we have with missing info
                    foreach (SmartUtility.GReadings g_readings_row in g_readings_found)
                    {
                        if (string.IsNullOrEmpty(g_readings_row.UNIT_OF_MEASURE))
                        {
                            g_readings_row.UNIT_OF_MEASURE = utilityviewmodel.UNIT_OF_MEASURE;
                        }
                    }

                    string trigger = "";
                    switch (utilityviewmodel.bill_version)
                    {
                        case 0:
                            trigger = vA_gas_supply_standing_charge;
                            break;
                        case 1:
                            trigger = standing_charge;
                            break;
                        default:
                            break;
                    }

                    int sub_line_count = line_count;
                    if (!SectionJ2_Readings_Pass2(ourviewmodel,
                                                utilityviewmodel,
                                                sections,
                                                section_index,
                                                lines,
                                                trigger,
                                                utilityviewmodel.charges_item,
                                                utilityviewmodel.units_band,
                                                utilityviewmodel.READINGS_PERIOD_START,
                                                utilityviewmodel.READINGS_PERIOD_END,
                                                ref sub_line_count))
                    {
                        return false;
                    }
                    line_count = sub_line_count;
                    goto the_old_daysJ2B;
                }

                // Last Tuesday, the petulant aggressive ignorant charmless cow was stomping about
                // the bedroom effing and blinding as usual.  And then she lays on the bed 'thumping'
                // her fucking leg like 'Thumper' ... what the fuck is she doing?  I don't know and I don't care
                // I just think she is a rude mannerless uneducated moron who does her best to wake me up
                // ODIBF  ODSBD RTB3

                // Now try and get all the VAT Lines
                if (SmartParseV2016.Token_Identify(utilityviewmodel, vat +
                                                    SmartParametersV2016.bar +
                                                    plus_vat_at, true))
                {
                    int percent = utilityviewmodel.token.IndexOf("%");
                    if (!SectionJ2_VAT_Percent(ourviewmodel,
                                                    utilityviewmodel,
                                                    utilityviewmodel.token,
                                                    percent))
                    {
                        return false;
                    }
                    goto the_old_daysJ2B;
                }
                if (SmartParseV2016.Token_Identify_Middle(utilityviewmodel, "Yourgasfueldiscountfortheperiodupto", true, ""))
                {
                    if (!SectionJ2_Gas_Discount(ourviewmodel,
                                                    utilityviewmodel,
                                                    ref line_count,
                                                    utilityviewmodel.token,
                                                    sections,
                                                    section_index,
                                                    lines))
                    {
                        return false;
                    }
                    goto the_old_daysJ2B;
                }
            // Now .. the stupid bitch after BEING TOLD that 2kg is about 4.5lbs
            // went and cooked the fucking ham based on a time for 4kg !!!!
            // MAFFS isn't her strongest suit!!!

            the_old_daysJ2B:
                continue;
                // Last Tuesday, the petulant aggressive ignorant charmless cow was stomping about
                // the bedroom effing and blinding as usual.  And then she lays on the bed 'thumping'
                // her fucking leg like 'Thumper' ... what the fuck is she doing?  I don't know and I don't care
                // I just think she is a rude mannerless uneducated moron who does her best to wake me up
                // ODIBF  ODSBD RTB3
            }
            return true;
        }

        private static bool SectionJ2_Third_Pass(MainViewModel ourviewmodel,
                                                    UtilityViewModel utilityviewmodel,
                                                    string[][] sections,
                                                    int section_index,
                                                    string[] lines,
                                                    short units_band,
                                                    string UNITS_TYPE,
                                                    string UNIT_OF_MEASURE)
        {
            string vB_How_We_Calculate_your_Gas_Charges = "HowWeCalculateyourGasCharges",
                    vB_How_we_calculate_your_gas_charges = "Howwecalculateyourgascharges",
                    v1_how_we_calculate_your_gas_consumption = "HowWeCalculateyourGasConsumption";

            //int line_count;// = 0;
            // Third Pass - do the Supply Number
            for (int line_count = Convert.ToInt32(sections[section_index][4]); line_count <= Convert.ToInt32(sections[section_index][5]); line_count++)
            {
                utilityviewmodel.token = lines[line_count].Replace(SmartParametersV2016.space, "");
                // VERSION A - CONSUMPTION
                if (SmartParseV2016.Token_Identify(utilityviewmodel, v1_how_we_calculate_your_gas_consumption, true))
                {
                    int sub_line_count = line_count;
                    if (!SectionJ0_VA_Consumption(utilityviewmodel,
                                                    utilityviewmodel.token,
                                                    sections,
                                                    section_index,
                                                    lines,
                                                    ref sub_line_count))
                    {
                        return false;
                    }
                    //    Pompous Bitch                    
                    line_count = sub_line_count;
                    goto the_old_daysJ2C;
                }

                // VERSION B - CONSUMPTION
                if (SmartParseV2016.Token_Identify(utilityviewmodel, vB_How_We_Calculate_your_Gas_Charges +
                                                                SmartParametersV2016.bar +
                                                                vB_How_we_calculate_your_gas_charges, true))
                {
                    int sub_line_count = line_count;
                    if (!SectionJ0_VB_Consumption(ourviewmodel,
                                                    utilityviewmodel,
                                                    utilityviewmodel.token,
                                                    sections,
                                                    section_index,
                                                    lines,
                                                    units_band,
                                                    UNITS_TYPE,
                                                    UNIT_OF_MEASURE,
                                                    ref sub_line_count))
                    {
                        return false;
                    }
                    line_count = sub_line_count;
                    goto the_old_daysJ2C;
                }
            the_old_daysJ2C:
                continue;
            }
            utilityviewmodel.units_band = units_band;
            return true;
        }

        private static bool SectionJ0_VB_Consumption(MainViewModel ourviewmodel,
                                                    UtilityViewModel utilityviewmodel,
                                                    string token,
                                                    string[][] sections,
                                                    int section_index,
                                                    string[] lines,
                                                    short units_band,
                                                    string UNITS_TYPE,
                                                    string UNIT_OF_MEASURE,
                                                    ref int sub_line_count)
        {
            string kWh = "kWh";

            while (sub_line_count <= Convert.ToInt32(sections[section_index][5]))
            {
                // Pompous Bitch
                // Now try and get all the Conversion Lines
                token = lines[sub_line_count].Replace(SmartParametersV2016.space, "");
                if (token == "Total")
                {
                    break;
                }
                if (token.IndexOf(kWh) >= 0)
                {
                    UNIT_OF_MEASURE = kWh;
                }
                else
                {
                    if (token.IndexOf("÷") >= 0)
                    {
                        utilityviewmodel.VOLUMECORRECTION = "0";
                        utilityviewmodel.KWHCONVERSION = "0";
                        token = lines[sub_line_count].Trim();

                        token = token.Replace("÷", "");
                        token = token.Replace("x", "");
                        token = token.Replace("=", "");
                        token = token.Replace(utilityviewmodel.bill_currency_symbol, "");
                        token = SmartParseV2016.Remove_Double_Spaces_V3(token);
                        string[] components_B = token.Split(' ');

                        if (!SectionJ0_VB_Bits_And_Pieces(utilityviewmodel,
                                                        components_B))

                        {
                            return false;
                        }

                        foreach (SmartUtility.GReadings g_readings_row in utilityviewmodel.Hezbollah.g_readings_changesList)
                        {
                            if ((g_readings_row.CALORIFIC_VALUE == 0) &&
                                (g_readings_row.READINGS_PERIOD_END == SmartTimeV2016.ConvertDateTime(utilityviewmodel.CLOSING_DATE)))
                            {
                                // These three should have been already checked previously
                                g_readings_row.CALORIFIC_VALUE = SmartRoutinesV2018.ConvertDecimal(utilityviewmodel.CALORIFIC_VALUE);
                                g_readings_row.D_UNITS_USED_KWH = SmartRoutinesV2018.ConvertDecimal(utilityviewmodel.UNITS);
                                // Check to see if we use the previous UNITS_BAND or make a new one
                                units_band = SmartParseV2016.Determine_Units_Band(ourviewmodel,
                                                                                    utilityviewmodel,
                                                                                    SmartParametersV2016.defaultChar,
                                                                                    utilityviewmodel.UNITS_RATE,
                                                                                    units_band);
                                SmartUtilityV2022.G_Unit_ChargesList_Add(utilityviewmodel,
                                                                            SmartParametersV2016.Utility,
                                                                            utilityviewmodel.supplier_code,
                                                                            utilityviewmodel.brand_code,
                                                                            utilityviewmodel.ACCOUNT_NO,
                                                                            utilityviewmodel.CREATED,
                                                                            utilityviewmodel.STATEMENT_ID,
                                                                            utilityviewmodel.BILL_DATE,
                                                                            utilityviewmodel.smell.MPRN,
                                                                            g_readings_row.READINGS_PERIOD_START.ToString(),
                                                                            g_readings_row.READINGS_PERIOD_END.ToString(),
                                                                            units_band.ToString(),
                                                                            UNITS_TYPE,
                                                                            utilityviewmodel.UNITS,
                                                                            utilityviewmodel.UNITS_RATE,
                                                                            UNIT_OF_MEASURE,
                                                                            utilityviewmodel.UNITS_COST);
                                break;
                            }
                        }
                    }
                }
                sub_line_count++;
            }
            utilityviewmodel.token = token;
            utilityviewmodel.units_band = units_band;
            return true;
        }

        private static bool SectionJ0_VB_Bits_And_Pieces(UtilityViewModel utilityviewmodel,
                                                        string[] components_B)
        {
            int item = 0;
            //DateTime temp_date = SmartParametersV2016.defaultDate;
            //decimal temp_decimal;
            //int temp;

            while (item < components_B.Length)
            {
                switch (item)
                {
                    case 0:
                        utilityviewmodel.CLOSING_DATE = components_B[item];
                        if (!SmartParseV2016.Generic_Parse_Datetime(utilityviewmodel.CLOSING_DATE, utilityviewmodel))
                        {
                            return false;
                        }
                        break;
                    case 1:
                        // m3 0
                        utilityviewmodel.CUBIC_METRES_USED = components_B[item];
                        if (!SmartParseV2016.Generic_Parse_Decimal(utilityviewmodel.CUBIC_METRES_USED, utilityviewmodel))
                        {
                            return false;
                        }
                        break;
                    case 2:
                        // Volume Correction Factor
                        utilityviewmodel.VOLUMECORRECTION = components_B[item];
                        if (!SmartParseV2016.Generic_Parse_Decimal(utilityviewmodel.VOLUMECORRECTION, utilityviewmodel))
                        {
                            return false;
                        }
                        break;
                    case 3:
                        // Calorific Value Multiplier
                        utilityviewmodel.CALORIFIC_VALUE = components_B[item];
                        if (!SmartParseV2016.Generic_Parse_Decimal(utilityviewmodel.CALORIFIC_VALUE, utilityviewmodel))
                        {
                            return false;
                        }
                        break;
                    case 4:
                        // Kwh Conversion Calorific Value Divider
                        utilityviewmodel.KWHCONVERSION = components_B[item];
                        if (!SmartParseV2016.Generic_Parse_Decimal(utilityviewmodel.KWHCONVERSION, utilityviewmodel))
                        {
                            return false;
                        }
                        break;
                    case 5:
                        // Units - for Gas we make this a decimal
                        utilityviewmodel.UNITS = components_B[item];
                        if (!SmartParseV2016.Generic_Parse_Decimal(utilityviewmodel.UNITS, utilityviewmodel))
                        {
                            return false;
                        }
                        break;
                    case 6:
                        // kWh gets stored in Units Rate
                        utilityviewmodel.UNITS_RATE = components_B[item];
                        if (!SmartParseV2016.Generic_Parse_Decimal(utilityviewmodel.UNITS_RATE, utilityviewmodel))
                        {
                            return false;
                        }
                        // Rate - comes in pounds so we have to convert to pence
                        // by moving the decimal point 2 places to the right
                        utilityviewmodel.UNITS_RATE = SmartParseV2016.Refactor_Unit_Rate(components_B[item]);
                        // Check again
                        if (!SmartParseV2016.Generic_Parse_Decimal(utilityviewmodel.UNITS_RATE, utilityviewmodel))
                        {
                            return false;
                        }
                        // Enough!! NO!!!
                        break;  // How can I work with THUMPER clomping around upstairs??
                    case 7:
                        utilityviewmodel.UNITS_COST = components_B[item].Trim();
                        utilityviewmodel.UNITS_COST = utilityviewmodel.UNITS_COST.Replace(utilityviewmodel.bill_currency_separator.ToString(), "");
                        utilityviewmodel.UNITS_COST = utilityviewmodel.UNITS_COST.Replace(utilityviewmodel.bill_thousands_separator.ToString(), "");
                        if (!SmartParseV2016.Generic_Parse_Integer(utilityviewmodel.UNITS_COST, utilityviewmodel))
                        {
                            return false;
                        }
                        break;
                    default:
                        break;
                }
                item++;
            }
            return true;
        }

        private static bool SectionJ0_VA_Consumption(UtilityViewModel utilityviewmodel,
                                                    string token,
                                                    string[][] sections,
                                                    int section_index,
                                                    string[] lines,
                                                    ref int sub_line_count)
        {
            string D_UNITS_USED_KWH,
                    CUBIC_METRES_USED,
                    VOLUMECORRECTION,
                    KWHCONVERSION;

            while (sub_line_count <= Convert.ToInt32(sections[section_index][5]))
            {
                // Pompous Bitch
                // Now try and get all the Conversion Lines
                token = lines[sub_line_count].Replace(SmartParametersV2016.space, "");
                if (token.IndexOf("Acorrectionfactorof") >= 0)
                {
                    break;
                }
                if (!string.IsNullOrEmpty(token))
                {
                    if (token.IndexOf("÷") >= 0)
                    {
                        //VOLUMECORRECTION = KWHCONVERSION = "0";
                        token = lines[sub_line_count];

                        token = token.Replace("to kWh Conversion", "").Trim();
                        token = token.Replace("÷", "");
                        token = token.Replace("x", "");
                        token = SmartParseV2016.Remove_Double_Spaces_V3(token);
                        string[] components = token.Split(' ');
                        int item = 0;

                        //decimal temp_decimal;

                        while (item < components.Length)
                        {
                            switch (item)
                            {
                                case 0:
                                    // m3 0
                                    CUBIC_METRES_USED = components[item];
                                    if (!SmartParseV2016.Generic_Parse_Decimal(CUBIC_METRES_USED, utilityviewmodel))
                                    {
                                        return false;
                                    }
                                    break;
                                case 1:
                                    // Volume Correction Factor
                                    VOLUMECORRECTION = components[item];
                                    if (!SmartParseV2016.Generic_Parse_Decimal(VOLUMECORRECTION, utilityviewmodel))
                                    {
                                        return false;
                                    }
                                    break;
                                case 2:
                                    // Calorific Value Multiplier
                                    utilityviewmodel.CALORIFIC_VALUE = components[item];
                                    if (!SmartParseV2016.Generic_Parse_Decimal(utilityviewmodel.CALORIFIC_VALUE, utilityviewmodel))
                                    {
                                        return false;
                                    }
                                    break;
                                case 3:
                                    // Kwh Conversion Calorific Value Divider
                                    KWHCONVERSION = components[item];
                                    if (!SmartParseV2016.Generic_Parse_Decimal(KWHCONVERSION, utilityviewmodel))
                                    {
                                        return false;
                                    }
                                    break;
                                //case 4:
                                //    // Kwh Units Used
                                //    D_UNITS_USED_KWH = components[item];
                                //    if (!SmartParseV2016.Generic_Parse_Decimal(D_UNITS_USED_KWH, rf temp_decimal, rf ourviewmodel.errorMessage))
                                //    {
                                //        return false;
                                //    }
                                //    break;
                                default:
                                    break;
                            }
                            item++;
                        }
                        // Can't do the rates because there is no rates info for Version A
                        foreach (SmartUtility.GReadings g_readings_row in utilityviewmodel.Hezbollah.g_readings_changesList)
                        {
                            if (g_readings_row.CALORIFIC_VALUE == 0)
                            {
                                // These three should have been already checked previously
                                g_readings_row.CALORIFIC_VALUE = SmartRoutinesV2018.ConvertDecimal(utilityviewmodel.CALORIFIC_VALUE);
                                D_UNITS_USED_KWH = ((g_readings_row.D_UNITS_USED_M3 *
                                                                    Convert.ToDecimal(utilityviewmodel.VOLUMECORRECTION) *
                                                                    g_readings_row.CALORIFIC_VALUE) /
                                                                    Convert.ToDecimal(utilityviewmodel.KWHCONVERSION)).ToString("#.###");

                                g_readings_row.D_UNITS_USED_KWH = SmartRoutinesV2018.ConvertDecimal(D_UNITS_USED_KWH);
                            }
                        }
                    }
                }
                sub_line_count++;
            }
            utilityviewmodel.token = token;
            return true;
        }

        private static bool SectionJ2_Gas_Discount(MainViewModel ourviewmodel,
                                                    UtilityViewModel utilityviewmodel,
                                                    ref int line_count,
                                                    string token,
                                                    string[][] sections,
                                                    int section_index,
                                                    string[] lines)
        {
            string on_your = "onyour";

            if (token.IndexOf(SmartParametersV2016.period) >= 0)
            {
                // Find the next line to end with a period
                // Try the NEXT line!!!
                int sub_line_count = line_count + 1;
                while (sub_line_count <= Convert.ToInt32(sections[section_index][5]))
                {
                    token += lines[sub_line_count].Replace(SmartParametersV2016.space, "").Trim();
                    // Is the last character a period ?
                    if (token.Substring(token.Length - 1, 1) == SmartParametersV2016.period)
                    {
                        token = token.Replace(SmartParametersV2016.period, "");
                        break;
                    }
                    sub_line_count++;
                }
                line_count = sub_line_count;
            }

            int on_your_index = token.IndexOf(on_your);
            if (on_your_index >= 0)
            {
                token = token.Substring(0, on_your_index);
                token = token.Replace(on_your, "");

                int bill_index = token.IndexOf("bill");
                if (bill_index >= 0)
                {
                    string DISCOUNT_CREDIT_DATE = token.Substring(0, bill_index).Trim();
                    //DateTime temp_date = SmartParametersV2016.defaultDate;

                    DISCOUNT_CREDIT_DATE = "01" + DISCOUNT_CREDIT_DATE;
                    if (!SmartParseV2016.Generic_Parse_Datetime(DISCOUNT_CREDIT_DATE, utilityviewmodel))
                    {
                        return false;
                    }
                    //else
                    //{
                    //    temp_date = utilityviewmodel.genericTargetDate;
                    //}
                    SmartParseV2016.Update_Bill(ourviewmodel, utilityviewmodel, "DISCOUNT_CREDIT_DATE", DISCOUNT_CREDIT_DATE);
                }
            }
            utilityviewmodel.token = token;
            // line_count = line_count;
            return true;
        }
        private static bool SectionJ2_VAT_Percent(MainViewModel ourviewmodel,
                                                    UtilityViewModel utilityviewmodel,
                                                    string token,
                                                    int percent)
        {
            if (percent >= 0)
            {
                string VAT_RATE = token.Substring(0, percent).Trim();
                token = token.Substring(percent + 1);
                string VAT_AMOUNT;// = "";
                if (!SmartParseV2016.Currency_Strip(utilityviewmodel.bill_currency_symbol, utilityviewmodel.bill_currency_separator, utilityviewmodel.bill_thousands_separator, token, utilityviewmodel))
                {
                    return false;
                }
                else
                {
                    VAT_AMOUNT = utilityviewmodel.value;
                }
                //int temp = 0;
                if (!SmartParseV2016.Generic_Parse_Integer(VAT_AMOUNT, utilityviewmodel))
                {
                    return false;
                }
                //else
                //{
                //    temp = utilityviewmodel.genericTransactionValue;
                //}
                SmartParseV2016.Update_Bill(ourviewmodel, utilityviewmodel, "RESOURCE_VAT_AMOUNT", VAT_AMOUNT);
                string target_rate = VAT_RATE;
                int has_decimal = target_rate.IndexOf(SmartParametersV2016.decimalPoint);
                if (has_decimal < 0)
                {
                    target_rate += ".00";
                }
                short VAT_CODE;// = SmartParametersV2016.zeroRateVatCode;
                if (!SmartParseV2016.Lookup_Vat_Code(target_rate, SmartTimeV2016.ConvertDateTime(utilityviewmodel.BILL_DATE), ourviewmodel.Blanche.vatRatesList, utilityviewmodel))
                {
                    return false;
                }
                else
                {
                    VAT_CODE = utilityviewmodel.vat_code;
                }
                SmartParseV2016.Update_Bill(ourviewmodel, utilityviewmodel, "RESOURCE_VAT_CODE", VAT_CODE.ToString());
            }
            utilityviewmodel.token = token;
            return true;
        }

        private static bool SectionJ2_Readings_Pass2(MainViewModel ourviewmodel,
                                                UtilityViewModel utilityviewmodel,
                                                string[][] sections,
                                                int section_index,
                                                string[] lines,
                                                string trigger,
                                                short charges_item,
                                                short units_band,
                                                string UNIT_CHARGES_PERIOD_START,
                                                string UNIT_CHARGES_PERIOD_END,
                                                ref int sub_line_count)
        {
            utilityviewmodel.UNIT_OF_MEASURE = "";
            utilityviewmodel.STANDING_CHARGE = "0";
            utilityviewmodel.UNITS_COST = "0";
            utilityviewmodel.CHARGES_COST = "0";
            utilityviewmodel.CHARGES_DAYS = "0";
            utilityviewmodel.UNITS = "0";
            utilityviewmodel.UNITS_RATE = "0";
            utilityviewmodel.UNITS_TYPE = "";

            string total_supply_charges = "Totalsupplycharges",
                    total_suppy_charges = "Totalsuppycharges",
                    vA_gas_total_unit_charge = "Gastotalunitcharge",
                    vB_resource = "-Gas";

            while (sub_line_count <= Convert.ToInt32(sections[section_index][5]))
            {
                utilityviewmodel.token = lines[sub_line_count].Replace(SmartParametersV2016.space, "").Trim();
                if ((utilityviewmodel.token.IndexOf(total_supply_charges) >= 0) ||
                    (utilityviewmodel.token.IndexOf(total_suppy_charges) >= 0))  // Would you fucking believe it??!!??
                {
                    break;
                }

                if (utilityviewmodel.token.IndexOf(trigger) >= 0)
                {
                    utilityviewmodel.token = utilityviewmodel.token.Replace(trigger, "");
                    if (utilityviewmodel.token.IndexOf("-Gas.") >= 0)
                    {
                        utilityviewmodel.token = utilityviewmodel.token.Replace("-Gas.", "");
                    }
                    if (utilityviewmodel.token.IndexOf("-Gas") >= 0)
                    {
                        utilityviewmodel.token = utilityviewmodel.token.Replace("-Gas", "");
                    }
                    if (utilityviewmodel.token.IndexOf("Gas") >= 0)
                    {
                        utilityviewmodel.token = utilityviewmodel.token.Replace("Gas", "");
                    }
                    string CHARGES_TYPE = "Standing Charge";
                    utilityviewmodel.CHARGES_PERIOD = "";
                    if (!VersionAB_line_split(utilityviewmodel,
                                                vB_resource))
                    {
                        return false;
                    }
                    SmartParseV2016.Update_Charges_Item(utilityviewmodel);
                    charges_item = 1;   // Always safe because of the above line

                    SmartUtilityV2022.G_Standing_ChargesList_Add(utilityviewmodel,
                                                                    SmartParametersV2016.Utility,
                                                                    utilityviewmodel.supplier_code,
                                                                    utilityviewmodel.brand_code,
                                                                    utilityviewmodel.ACCOUNT_NO,
                                                                    utilityviewmodel.CREATED,
                                                                    utilityviewmodel.STATEMENT_ID,
                                                                    utilityviewmodel.BILL_DATE,
                                                                    utilityviewmodel.smell.MPRN,
                                                                    UNIT_CHARGES_PERIOD_START,
                                                                    UNIT_CHARGES_PERIOD_END,
                                                                    charges_item.ToString(),
                                                                    CHARGES_TYPE,
                                                                    utilityviewmodel.STANDING_CHARGE,
                                                                    utilityviewmodel.CHARGES_DAYS,
                                                                    utilityviewmodel.CHARGES_COST);
                }

                if (utilityviewmodel.token.IndexOf(vA_gas_total_unit_charge) >= 0)
                {
                    utilityviewmodel.token = utilityviewmodel.token.Replace(vA_gas_total_unit_charge, "");
                    // Version 1
                    if (!VersionAB_units_split(utilityviewmodel))
                    {
                        return false;
                    }
                    if (!string.IsNullOrEmpty(UNIT_CHARGES_PERIOD_START) &&
                        !string.IsNullOrEmpty(UNIT_CHARGES_PERIOD_END))
                    {
                        // Check to see if we use the previous UNITS_BAND or make a new one
                        units_band = SmartParseV2016.Determine_Units_Band(ourviewmodel,
                                                                            utilityviewmodel,
                                                                            SmartParametersV2016.defaultChar,
                                                                            utilityviewmodel.UNITS_RATE,
                                                                            units_band);
                        SmartUtilityV2022.G_Unit_ChargesList_Add(utilityviewmodel,
                                                                    SmartParametersV2016.Utility,
                                                                    utilityviewmodel.supplier_code,
                                                                    utilityviewmodel.brand_code,
                                                                    utilityviewmodel.ACCOUNT_NO,
                                                                    utilityviewmodel.CREATED,
                                                                    utilityviewmodel.STATEMENT_ID,
                                                                    utilityviewmodel.BILL_DATE,
                                                                    utilityviewmodel.smell.MPRN,
                                                                    UNIT_CHARGES_PERIOD_START,
                                                                    UNIT_CHARGES_PERIOD_END,
                                                                    units_band.ToString(),
                                                                    utilityviewmodel.UNITS_TYPE,
                                                                    utilityviewmodel.UNITS,
                                                                    utilityviewmodel.UNITS_RATE,
                                                                    utilityviewmodel.UNIT_OF_MEASURE,
                                                                    utilityviewmodel.UNITS_COST);
                    }
                }
                sub_line_count++;
            }
            utilityviewmodel.charges_item = charges_item;
            utilityviewmodel.units_band = units_band;
            return true;
        }
        //private static bool sectionJ0_readings_pass1(UtilityViewModel utilityviewmodel,

        private static void SectionJ0_Preamble(UtilityViewModel utilityviewmodel, int line_count)
        {
            utilityviewmodel.token = utilityviewmodel.lines[line_count].Replace(SmartParametersV2016.space, "");
            if (!string.IsNullOrEmpty(utilityviewmodel.token))
            {
                if (utilityviewmodel.token.Substring(utilityviewmodel.token.Length - 1, 1) == "m")
                {
                    int temp_count = line_count + 1;
                    if (temp_count < utilityviewmodel.lines.Length)
                    {
                        if (!string.IsNullOrEmpty(utilityviewmodel.lines[temp_count]))
                        {
                            if (utilityviewmodel.lines[temp_count] == "3")
                            {
                                utilityviewmodel.lines[line_count] = utilityviewmodel.lines[line_count] + utilityviewmodel.lines[temp_count];
                                utilityviewmodel.lines[temp_count] = "";
                            }
                        }
                    }
                }
            }
            return;
        }

        
        private static bool VAB_charges_split(UtilityViewModel utilityviewmodel,
                                                string token)
        {
            //DateTime temp_date = SmartParametersV2016.defaultDate;
            string dash = "-";
            string charges_range = token;
            int dash_index = charges_range.IndexOf(dash);
            if (dash_index >= 0)
            {
                utilityviewmodel.READINGS_PERIOD_END = charges_range.Substring(dash_index + dash.Length);
                if (!SmartParseV2016.Generic_Parse_Datetime(utilityviewmodel.READINGS_PERIOD_END, utilityviewmodel))
                {
                    return false;
                }
                //else
                //{
                //    temp_date = utilityviewmodel.genericTargetDate;
                //}
                utilityviewmodel.READINGS_PERIOD_START = charges_range.Substring(0, dash_index);
                // Note: use the temp_date below in the next section!
                if (!SmartParseV2016.Generic_Parse_Datetime(utilityviewmodel.READINGS_PERIOD_START, utilityviewmodel))
                {
                    return false;
                }
                //else
                //{
                //    temp_date = utilityviewmodel.genericTargetDate;
                //}
            }
            else
            {
                return false;
            }
            return true;
        }

        private static string Check_Read_Type(string utility_type)
        {
            // C = Customer
            // E = Estimate
            // D = Deemed
            // R = Routine
            // S = Smart
            // I = Initial
            // F = Final
            if (!string.IsNullOrEmpty(utility_type))
            {
                if (utility_type.Length == 1)
                {
                    switch (utility_type)
                    {
                        case "C":
                            return "Customer";
                        case "E":
                            return "Estimate";
                        case "D":
                            return "Deemed";
                        case "R":
                            return "Routine";
                        case "S":
                            return "Smart";
                        case "I":
                            return "Initial";
                        case "F":
                            return "Final";
                        default:
                            break;
                    }
                }
                // Embedded commas fuck up SmartDBServer
                return utility_type;
            }
            return utility_type;
        }

        private static bool VersionAB_units_split(UtilityViewModel utilityviewmodel)
        {
            // Version 1
            utilityviewmodel.token = utilityviewmodel.token.Replace("kWh", "kWh ");
            utilityviewmodel.token = utilityviewmodel.token.Replace("pper", "p per");
            utilityviewmodel.token = utilityviewmodel.token.Replace("p perkWh ", SmartParametersV2016.space);
            utilityviewmodel.token = utilityviewmodel.token.Replace("kWh", " kWh");

            utilityviewmodel.UNITS_TYPE = "";
            string[] components = utilityviewmodel.token.Split(' ');
            int item = 0;
            while (item < components.Length)
            {
                switch (item)
                {
                    case 0:
                        // Units - for Electricity we make this an Int32 <= No its not always true
                        utilityviewmodel.UNITS = components[item].Trim();
                        if (!SmartParseV2016.Generic_Parse_Decimal(utilityviewmodel.UNITS, utilityviewmodel))
                        {
                            return false;
                        }
                        //else
                        //{
                        //    temp_decimal = utilityviewmodel.genericDecimalValue;
                        //}
                        utilityviewmodel.UNITS_TYPE = utilityviewmodel.UNITS;
                        break;
                    case 1:
                        // Unit of Measure (usually kWh hours)
                        utilityviewmodel.UNIT_OF_MEASURE = components[item].Trim();
                        utilityviewmodel.UNITS_TYPE = utilityviewmodel.UNITS_TYPE + SmartParametersV2016.space + utilityviewmodel.UNIT_OF_MEASURE;
                        break;
                    case 2:
                        utilityviewmodel.UNITS_RATE = components[item].Trim();
                        // Enough!! NO!!!
                        if (!SmartParseV2016.Generic_Parse_Decimal(utilityviewmodel.UNITS_RATE, utilityviewmodel))
                        {
                            return false;
                        }
                        //else
                        //{
                        //    temp_decimal = utilityviewmodel.genericDecimalValue;
                        //}
                        utilityviewmodel.UNITS_TYPE = utilityviewmodel.UNITS_TYPE + " @" + utilityviewmodel.UNITS_RATE + utilityviewmodel.bill_denomination_symbol.ToString();
                        break;  // How can I work with THUMPER clomping around upstairs??
                    case 3:
                        if (!SmartParseV2016.Currency_Strip(utilityviewmodel.bill_currency_symbol, utilityviewmodel.bill_currency_separator, utilityviewmodel.bill_thousands_separator, components[item].Trim(), utilityviewmodel))
                        {
                            return false;
                        }
                        else
                        {
                            utilityviewmodel.UNITS_COST = utilityviewmodel.value;
                        }
                        //int temp = 0;
                        if (!SmartParseV2016.Generic_Parse_Integer(utilityviewmodel.UNITS_COST, utilityviewmodel))
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
                item++;
            }
            return true;
        }

        private static bool VersionAB_line_split(UtilityViewModel utilityviewmodel,
                                                    string VB_resource)
        {
            switch (utilityviewmodel.bill_version)
            {
                case 0:
                    break;
                case 1:
                    utilityviewmodel.token = utilityviewmodel.token.Replace(VB_resource + SmartParametersV2016.period, "");
                    utilityviewmodel.token = utilityviewmodel.token.Replace(VB_resource, "");
                    utilityviewmodel.token = utilityviewmodel.token.Replace("SingleRate", "");
                    break;
                default:
                    break;
            }
            utilityviewmodel.token = utilityviewmodel.token.Replace("@", "");
            utilityviewmodel.token = utilityviewmodel.token.Replace("x", "");   // <== Fucking Foreigners

            utilityviewmodel.token = utilityviewmodel.token.Replace("days", " days ");
            utilityviewmodel.token = utilityviewmodel.token.Replace("pperday", SmartParametersV2016.space);

            string[] components = utilityviewmodel.token.Split(' ');
            int item = 0;
            while (item < components.Length)
            {
                switch (item)
                {
                    case 0:
                        // Days = should be an integer
                        utilityviewmodel.CHARGES_DAYS = components[item].Trim();
                        if (!SmartParseV2016.Generic_Parse_Integer(utilityviewmodel.CHARGES_DAYS, utilityviewmodel))
                        {
                            return false;
                        }
                        //else
                        //{
                        //    temp_int = utilityviewmodel.genericTransactionValue;
                        //}
                        break;
                    case 1:
                        // Period (usually 'days' as a WORD you moron!)
                        utilityviewmodel.CHARGES_PERIOD = components[item].Trim();
                        break;
                    case 2:
                        // Convert to an integer
                        utilityviewmodel.STANDING_CHARGE = components[item].Trim();
                        if (!SmartParseV2016.Generic_Parse_Decimal(utilityviewmodel.STANDING_CHARGE, utilityviewmodel))
                        {
                            return false;
                        }
                        //else
                        //{
                        //    temp_decimal = utilityviewmodel.genericDecimalValue;
                        //}
                        break;
                    case 3:
                        // Convert to an integer
                        if (!SmartParseV2016.Currency_Strip(utilityviewmodel.bill_currency_symbol, utilityviewmodel.bill_currency_separator, utilityviewmodel.bill_thousands_separator, components[item].Trim(), utilityviewmodel))
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
                        //else
                        //{
                        //    temp_int = utilityviewmodel.genericTransactionValue;
                        //}
                        break;
                    default:
                        break;
                }
                item++;
            }
            return true;
        }

        private static bool VersionA_line_split(UtilityViewModel utilityviewmodel)
        {
            // Should be same as DateTime  .Today
            //DateTime temp_date = SmartParametersV2016.defaultDate;
            //decimal temp_decimal = 0.0M;

            string[] components_A = utilityviewmodel.token.Split(' ');
            int item_A;// = 0;
            int item = 0;
            while (item < components_A.Length)
            {
                item_A = item;
                if ((utilityviewmodel.resource_code == SmartParametersV2016.Electricity) &&
                    (item > 0))
                {
                    // So we skip the METER_TYPE for Electricity
                    item_A = item + 1;
                }

                switch (item_A)
                {
                    case 0:
                        // Meter Serial No.
                        utilityviewmodel.METER_SERIAL_NO = components_A[item];
                        break;
                    case 1:
                        // Meter Type
                        utilityviewmodel.METER_TYPE = Check_Read_Type(components_A[item]);
                        break;
                    case 2:
                        // Closing Read Date
                        utilityviewmodel.READ_DATE = components_A[item];
                        // CHURN CHURN CHURN CHURN CHURN witter witter witter witter witter
                        if (!SmartParseV2016.Generic_Parse_Datetime(utilityviewmodel.READ_DATE, utilityviewmodel))
                        {
                            return false;
                        }
                        //else
                        //{
                        //    temp_date = utilityviewmodel.genericTargetDate;
                        //}
                        break;
                    case 3:
                        utilityviewmodel.READ_TYPE = Check_Read_Type(components_A[item]);
                        // I apologise for that one, I didn't really mean it
                        break;
                    case 4:
                        // Closing Read
                        utilityviewmodel.D_THIS_READ = components_A[item]; // In case there is an 'Estimated' dec point
                        if (!SmartParseV2016.Generic_Parse_Decimal(utilityviewmodel.D_THIS_READ, utilityviewmodel))
                        {
                            return false;
                        }
                        //else
                        //{
                        //    temp_decimal = utilityviewmodel.genericDecimalValue;
                        //}
                        break;
                    case 5:
                        // Opening read
                        utilityviewmodel.D_LAST_READ = components_A[item]; // In case there is an 'Estimated' dec point
                        if (!SmartParseV2016.Generic_Parse_Decimal(utilityviewmodel.D_LAST_READ, utilityviewmodel))
                        {
                            return false;
                        }
                        //else
                        //{
                        //    temp_decimal = utilityviewmodel.genericDecimalValue;
                        //}
                        break;
                    case 6:
                        utilityviewmodel.D_UNITS_USED = components_A[item]; // In case there is an 'Estimated' dec point
                        if (!SmartParseV2016.Generic_Parse_Decimal(utilityviewmodel.D_UNITS_USED, utilityviewmodel))
                        {
                            return false;
                        }
                        //else
                        //{
                        //    temp_decimal = utilityviewmodel.genericDecimalValue;
                        //}
                        break;
                    default:
                        break;
                }
                item++;
            }
            return true;
        }

        private static bool VersionB_line_split(UtilityViewModel utilityviewmodel)
        {
            string[] components_B = utilityviewmodel.token.Split(' ');
            int item_B = 0;
            while (item_B < components_B.Length)
            {
                switch (item_B)
                {
                    case 0:
                        // Opening Read Date
                        utilityviewmodel.READINGS_PERIOD_START = components_B[item_B];
                        if (!SmartParseV2016.Generic_Parse_Datetime(utilityviewmodel.READINGS_PERIOD_START, utilityviewmodel))
                        {
                            return false;
                        }
                        //else
                        //{
                        //    temp_date = utilityviewmodel.genericTargetDate;
                        //}
                        break;
                    case 1:
                        utilityviewmodel.FROM_TYPE = Check_Read_Type(components_B[item_B]);
                        // I apologise for that one, I didn't really mean it
                        break;
                    case 2:
                        // Opening Read
                        utilityviewmodel.D_LAST_READ = components_B[item_B]; // In case there is an 'Estimated' dec point
                        if (!SmartParseV2016.Generic_Parse_Decimal(utilityviewmodel.D_LAST_READ, utilityviewmodel))
                        {
                            return false;
                        }
                        //else
                        //{
                        //    temp_decimal = utilityviewmodel.genericDecimalValue;
                        //}
                        break;
                    case 3:
                        // Closing Read Date
                        utilityviewmodel.READINGS_PERIOD_END = components_B[item_B];
                        // CHURN CHURN CHURN CHURN CHURN witter witter witter witter witter
                        if (!SmartParseV2016.Generic_Parse_Datetime(utilityviewmodel.READINGS_PERIOD_END, utilityviewmodel))
                        {
                            return false;
                        }
                        //else
                        //{
                        //    temp_date = utilityviewmodel.genericTargetDate;
                        //}
                        break;
                    case 4:
                        utilityviewmodel.READ_TYPE = Check_Read_Type(components_B[item_B]);
                        break;
                    case 5:
                        // Closing read
                        utilityviewmodel.D_THIS_READ = components_B[item_B]; // In case there is an 'Estimated' dec point
                        if (!SmartParseV2016.Generic_Parse_Decimal(utilityviewmodel.D_THIS_READ, utilityviewmodel))
                        {
                            return false;
                        }
                        //else
                        //{
                        //    temp_decimal = utilityviewmodel.genericDecimalValue;
                        //}
                        break;
                    case 6:
                        utilityviewmodel.D_UNITS_USED = components_B[item_B]; // In case there is an 'Estimated' dec point
                        if (!SmartParseV2016.Generic_Parse_Decimal(utilityviewmodel.D_UNITS_USED, utilityviewmodel))
                        {
                            return false;
                        }
                        //else
                        //{
                        //    temp_decimal = utilityviewmodel.genericDecimalValue;
                        //}
                        break;
                    case 7:
                        switch (utilityviewmodel.resource_code)
                        {
                            case SmartParametersV2016.Electricity:
                                if (!SmartParseV2016.Generic_Parse_Decimal(utilityviewmodel.UNITS_RATE, utilityviewmodel))
                                {
                                    return false;
                                }
                                //else
                                //{
                                //    temp_decimal = utilityviewmodel.genericDecimalValue;
                                //}
                                // Rate - comes in pounds so we have to convert to pence
                                // by moving the decimal point 2 places to the right
                                utilityviewmodel.UNITS_RATE = SmartParseV2016.Refactor_Unit_Rate(components_B[item_B]);
                                // Check again!
                                if (!SmartParseV2016.Generic_Parse_Decimal(utilityviewmodel.UNITS_RATE, utilityviewmodel))
                                {
                                    return false;
                                }
                                //else
                                //{
                                //    temp_decimal = utilityviewmodel.genericDecimalValue;
                                //}
                                break;
                            case SmartParametersV2016.Gas:
                                // m3
                                utilityviewmodel.M3 = components_B[item_B];
                                break;
                            default:
                                break;
                        }
                        break;
                    case 8:
                        if (!SmartParseV2016.Currency_Strip(utilityviewmodel.bill_currency_symbol, utilityviewmodel.bill_currency_separator, utilityviewmodel.bill_thousands_separator, components_B[item_B], utilityviewmodel))
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
                        //else
                        //{
                        //    temp_int = utilityviewmodel.genericTransactionValue;
                        //}
                        break;
                    default:
                        break;
                }
                item_B++;
            }
            return true;
        }
    }
}