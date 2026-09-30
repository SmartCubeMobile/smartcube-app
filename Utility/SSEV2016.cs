using System;
using System.Collections.Generic;
using System.Threading.Tasks;
#if ANDROIDX
using AndroidX.AppCompat.App;
#endif

namespace SmartCubeMobile
{

    public class SSEV2016
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
            // LEAVE THIS IN AND SEE IF THIS BOLLOCKS MAKES ANY DIFFERENCE (I don't think it does)

            //
            // THINK WE CAN GET AWAY WITHOUT THIS USER AGENT BOLLOCKS
            //

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
            utilityviewmodel.your_products_pathname = "";
            utilityviewmodel.spare1 = "";
            utilityviewmodel.spare2 = "";
            utilityviewmodel.spare3 = "";
            utilityviewmodel.spare4 = "";
            utilityviewmodel.spare5 = "";

            utilityviewmodel.keep_looping = true;
            List<SmartUtility.Payments> payments_tempList = new List<SmartUtility.Payments>();

            while (utilityviewmodel.keep_looping)
            {
                await SmartUtilityV2022.First_Throw(ourviewmodel, utilityviewmodel);
                if (!string.IsNullOrEmpty(ourviewmodel.errorMessage) ||
                    utilityviewmodel.next_routine == "LOGOUT")
                {
                    utilityviewmodel.target_pathname = utilityviewmodel.logout_pathname;
                    utilityviewmodel.keep_looping = false;
                    utilityviewmodel.next_routine = "LOGOUT";
                }
                if (utilityviewmodel.next_routine != "HOME")
                {
                    utilityviewmodel.htmlDocument = await SmartBobV2017.Scraper_Generic_Get(ourviewmodel, utilityviewmodel.utilityToken, utilityviewmodel);
                    if (!string.IsNullOrEmpty(ourviewmodel.errorMessage))
                    {
                        return false;
                    }
                }
                utilityviewmodel.current_routine = utilityviewmodel.next_routine;
                switch (utilityviewmodel.next_routine) // if (next_routine.Length == 0)
                {
                    case "LOGIN":
                        // The timer is re-started when the
                        // first Login Document has been completed
                        utilityviewmodel.keyValues.Clear();
                        if (await SSE_Login(
#if ANDROIDX
                                        meterActivity,
#endif
                                        ourviewmodel,
                                        utilityviewmodel,
                                        utilityviewmodel.htmlDocument,
                                        TextBox_Active))
                        {

                            await SmartBobV2017.Scraper_Generic_Post(ourviewmodel,
                                                                    utilityviewmodel.utilityToken,
                                                                    utilityviewmodel.keyValues,
                                                                    utilityviewmodel);
                            if (!string.IsNullOrEmpty(ourviewmodel.errorMessage))
                            {
                                return false;
                            }
                            // Should now be going on to HOME
                        }
                        else
                        {
                            ourviewmodel.errorMessage = utilityviewmodel.current_routine;
                            utilityviewmodel.next_routine = "LOGOUT";
                            utilityviewmodel.login_finished = true;          // Escape route on Login failure
                        }
                        break;
                    case "YOURPRODUCTS":
                        // The timer is re-started when the
                        // first Login Document is completed

                        await SSE_YourProducts(ourviewmodel,
                                                 utilityviewmodel,
                                                 utilityviewmodel.htmlDocument);

                        // Should now be going on to BILLSPAYMENTS
                        // Boost my brainpower with blueberries? MY brainpower?
                        // Its your fucking brainpower that needs boosting - in fact you should
                        // eat a fucking ton of them every day to even get a basic fucking brain
                        break;
                    case "BILLSPAYMENTS":
                        // Fuck me!  It fucking WORKED!!!! See the re-direction to this 'secure'
                        // routine from the initial fucking Login routine above!!!!!!!!!!!!
                        // You ARE a fucking genius Ray!!  An absolute bravest of the brave
                        // fucking genius.  Those visionless cunts you saw last Thursday have
                        // ABSOLUTELY no idea what they are missing out on ....

                        await SSE_BillsPayments(
#if ANDROIDX
                                                meterActivity,
#endif
                                                ourviewmodel,
                                                 utilityviewmodel,
                                                 utilityviewmodel.htmlDocument,
                                                TextBox_Active,
                                                payments_tempList);

                        // Should now be going back to YOURPRODUCTS
                        utilityviewmodel.target_pathname = utilityviewmodel.your_products_pathname;
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
                        if (!string.IsNullOrEmpty(ourviewmodel.errorMessage))
                        {
                            return false;
                        }
                        break;
                    default:
                        // A small price to pay ...
                        SSE_Do_Intermediates(utilityviewmodel,
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

        private static void SSE_Do_Intermediates(UtilityViewModel utilityviewmodel,
                                                string next_routine,
                                                HtmlAgilityPack.HtmlDocument htmlDocument)

        {
            switch (next_routine)
            {
                case "HOME":
                    SSE_Home(utilityviewmodel,
                            htmlDocument);
                    // Should now be going on to YOURDETAILS

                    // Boost my brainpower with blueberries? MY brainpower?
                    // Its your fucking brainpower that needs boosting - in fact you should
                    // eat a fucking ton of them every day to even get a basic fucking brain
                    break;
                case "YOURDETAILS":
                    SSE_YourDetails(utilityviewmodel,
                                    htmlDocument);
                    // Should now be going on to YOURPRODUCTS
                    utilityviewmodel.your_products_pathname = utilityviewmodel.target_pathname;
                    // Boost my brainpower with blueberries? MY brainpower?
                    // Its your fucking brainpower that needs boosting - in fact you should
                    // eat a fucking ton of them every day to even get a basic fucking brain
                    break;

            }
            return;
        }
        private static async Task<bool> SSE_Login(
#if ANDROIDX
                                    AppCompatActivity meterActivity,
#endif
                                    MainViewModel ourviewmodel,
                                    UtilityViewModel utilityviewmodel,
                                    HtmlAgilityPack.HtmlDocument document,
                                    bool TextBox_Active)
        {
            IList<HtmlAgilityPack.HtmlNode> HtmlCol,
                                            HtmlCol2;
            string name;
            bool clicked = false;
            List<KeyValuePair<string, string>> keyValues = new List<KeyValuePair<string, string>>();


            HtmlCol2 = YetAnotherFuckingHoop.SelectNodesAsList(document.DocumentNode, "//input");
            foreach (HtmlAgilityPack.HtmlNode element2 in HtmlCol2)
            {
                string rvt_name = element2.GetAttributeValue("name", "");
                if (rvt_name == SmartParametersV2016.requestVerificationToken)
                {
                    string value = element2.GetAttributeValue("value", "");
                    keyValues.Add(new KeyValuePair<string, string>(rvt_name, value));
                    break;
                }
            }

            HtmlCol = YetAnotherFuckingHoop.SelectNodesAsList(document.DocumentNode, "//input");
            foreach (HtmlAgilityPack.HtmlNode element in HtmlCol)
            {
                switch (element.Id)
                {
                    case "ShowCaptcha":
                        //Type the password in the password text box
                        name = element.GetAttributeValue("name", "");
                        keyValues.Add(new KeyValuePair<string, string>(name, "False"));
                        break;
                    case "Email":
                        // Set the user name in the username text box
                        name = element.GetAttributeValue("name", "");
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
                        name = element.GetAttributeValue("name", "");
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
            // will be delivered and we have told the handler
            // to call the SecureLogin routine below when that
            // happens
            // If our login was UNSUCCESSFUL, then no Document
            // will be delivered and the timer will tick away
            // the 10 seconds and exit the timer loop
            return clicked;
        }

        private static void SSE_Home(UtilityViewModel utilityviewmodel,
                                        HtmlAgilityPack.HtmlDocument document)
        {
            // Fuck me!  It fucking WORKED!!!! See the re-direction to this 'secure'
            // routine from the initial fucking Login routine above!!!!!!!!!!!!
            // You ARE a fucking genius Ray!!  An absolute bravest of the brave
            // fucking genius.  Those visionless cunts you saw last Thursday have
            // ABSOLUTELY no idea what they are missing out on ....

            IList<HtmlAgilityPack.HtmlNode> HtmlCol1;
            string inner_text;// = "";
            bool clicked = false;

            HtmlCol1 = YetAnotherFuckingHoop.SelectNodesAsList(document.DocumentNode, "//a");
            foreach (HtmlAgilityPack.HtmlNode element1 in HtmlCol1)
            {
                string classname1 = element1.GetAttributeValue("class", "");
                // for reasons I cannot quite fathom ... the <a> element for "sprite loggedin"
                // is not returned by the foreach above!!!  When I look at the returned
                // document using Chrome (i.e. I view the source) its there, but when
                // I look at the returned DOCUMENT from Fiddler, its not there ... don't know why ..
                // Hence the kludge to force a logout_pathname before we exit this routine ...
                if (SmartNibbyV2016.Check_Classname(classname1, "sprite loggedin", true))
                {
                    utilityviewmodel.logout_pathname = element1.GetAttributeValue(SmartParametersV2016.href, "");
                    utilityviewmodel.logout_pathname = utilityviewmodel.logout_pathname.Replace(utilityviewmodel.prfix_xxx, "");
                }
                else
                {
                    inner_text = element1.InnerText;
                    if (!string.IsNullOrEmpty(inner_text))
                    {
                        if (inner_text.IndexOf("Your details") >= 0)
                        {
                            utilityviewmodel.target_pathname = element1.GetAttributeValue(SmartParametersV2016.href, "");
                            if (!string.IsNullOrEmpty(utilityviewmodel.target_pathname))
                            {
                                utilityviewmodel.next_routine = "YOURDETAILS";
                                clicked = true;
                                break;
                            }
                        }
                    }
                }
            }
            if (string.IsNullOrEmpty(utilityviewmodel.logout_pathname))
            {
                utilityviewmodel.logout_pathname = "/profile/loggedout";
            }
            if (!clicked)
            {
                utilityviewmodel.next_routine = "LOGOUT";
            }
            return;
        }

        private static void SSE_YourDetails(UtilityViewModel utilityviewmodel,
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

            string inner_text,
                    name = "Name",
                    phone_number = "Phone number",
                    email_marketing_consent = "Email Marketing Consent",
                    email_address = "Email address";
            bool clicked = false;

            HtmlCol1 = YetAnotherFuckingHoop.SelectNodesAsList(document.DocumentNode, "//section");  // <= First time ever??
            foreach (HtmlAgilityPack.HtmlNode element1 in HtmlCol1)
            {
                string classname1 = element1.GetAttributeValue("class", "");
                if (SmartNibbyV2016.Check_Classname(classname1, "container contentArea", true))
                {
                    HtmlCol2 = YetAnotherFuckingHoop.SelectNodesAsList(element1, ".//p");
                    foreach (HtmlAgilityPack.HtmlNode element2 in HtmlCol2)
                    {
                        inner_text = element2.InnerText;
                        if (!string.IsNullOrEmpty(inner_text))
                        {
                            if (inner_text.Length >= 4)
                            {
                                if (inner_text.Substring(0, 4) == name)
                                {
                                    utilityviewmodel.spare1 = inner_text.Replace(name, "");
                                }
                                else
                                {
                                    if (inner_text.IndexOf(phone_number) >= 0)
                                    {
                                        utilityviewmodel.spare3 = inner_text.Replace(phone_number, "");
                                    }
                                    else
                                    {
                                        if (inner_text.IndexOf(email_marketing_consent) >= 0)
                                        {

                                        }
                                        else
                                        {
                                            if (inner_text.IndexOf(email_address) >= 0)
                                            {
                                                utilityviewmodel.spare4 = inner_text.Replace(email_address, "");
                                                break;
                                            }
                                            else
                                            {
                                                // Address
                                                utilityviewmodel.spare2 = "";
                                                // DoB
                                                utilityviewmodel.spare5 = "";
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            HtmlCol1 = YetAnotherFuckingHoop.SelectNodesAsList(document.DocumentNode, "//a");
            foreach (HtmlAgilityPack.HtmlNode element1 in HtmlCol1)
            {
                inner_text = element1.InnerText;
                if (!string.IsNullOrEmpty(inner_text))
                {
                    if (inner_text.IndexOf("Your products") >= 0)
                    {
                        utilityviewmodel.target_pathname = element1.GetAttributeValue(SmartParametersV2016.href, "");
                        if (!string.IsNullOrEmpty(utilityviewmodel.target_pathname))
                        {
                            utilityviewmodel.next_routine = "YOURPRODUCTS";
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
            return;
        }

        private static async Task<bool> SSE_YourProducts(MainViewModel ourviewmodel,
                                                 UtilityViewModel utilityviewmodel,
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
            bool clicked = false;

            HtmlCol1 = YetAnotherFuckingHoop.SelectNodesAsList(document.DocumentNode, "//div");
            foreach (HtmlAgilityPack.HtmlNode element1 in HtmlCol1)
            {
                string classname = element1.GetAttributeValue("class", "");
                if (SmartNibbyV2016.Check_Classname(classname, "accordionWrap", true))
                {
                    HtmlCol2 = YetAnotherFuckingHoop.SelectNodesAsList(element1, ".//div");
                    foreach (HtmlAgilityPack.HtmlNode element2 in HtmlCol2)
                    {
                        string classname2 = element2.GetAttributeValue("class", "");
                        if (SmartNibbyV2016.Check_Classname(classname2, "accordionButton", true))
                        {
                            clicked = Accordian_Button(utilityviewmodel,
                                                        element2);
                        }

                        //
                        // Second do all the details
                        //
                        if (SmartNibbyV2016.Check_Classname(classname2, "accordionContent", true))
                        {
                            if (clicked)
                            {
                                SSE_accordian_content(ourviewmodel,
                                                        utilityviewmodel,
                                                        element2);
                                if (!string.IsNullOrEmpty(utilityviewmodel.account_address))
                                {
                                    SmartUtility.Accounts account_row = new SmartUtility.Accounts();
                                    string account_address = utilityviewmodel.account_address;
                                    SmartUtilityV2022.Amelia_Add(utilityviewmodel);

                                    // Amazingly - for the first time ever
                                    // we have the whole of the fucking PostCode
                                    // Need to find the AREA_ID from outward_postcodes_table !!!!
                                    if (SmartNibbyV2016.Derive_Postcode_New(account_address,
                                                                            ourviewmodel.Blanche.workingPostcodesList,
                                                                            utilityviewmodel))
                                    {

                                        SmartUtilityV2022.Set_UDPRN_MPAN_MPRN(utilityviewmodel);

                                        // If we don't have a UDPRN or the MPAN_MPRN is empty,
                                        // then go and look it up.
                                        if (//string.IsNullOrEmpty(udprn) ||
                                            string.IsNullOrEmpty(utilityviewmodel.mpan_mprn))
                                        {
                                            if (!await SmartUtilityV2022.Determine_UDPRNZ(ourviewmodel, utilityviewmodel, account_address))
                                            {
                                                return false;
                                            }
                                        }
                                        SmartUtilityV2022.Amelia_Update_Postcode_Area(ourviewmodel, utilityviewmodel);
                                        // Other bits and pieces
                                        // She is FOREVER banging on about being a Social Worker for older people
                                        // She brings it into EVERY fucking conversation.  EVERY fucking one.

                                        // No we can set this because we have a good SUPPLY ADDRESS
                                        utilityviewmodel.login_finished = true;

                                        // Can do this now we have the area_code
                                        if (utilityviewmodel.TARIFF_CODE == 0)
                                        {
                                            await SmartUtilityV2022.Lookup_Remote_Tariff_Code(ourviewmodel, utilityviewmodel);
                                        }
                                        // And its not PA-CIFIC its fucking SPE-CIFIC you idiot bitch
                                        // Only the first one considered
                                    }
                                }

                                HtmlCol3 = YetAnotherFuckingHoop.SelectNodesAsList(element2, ".//a");
                                foreach (HtmlAgilityPack.HtmlNode element3 in HtmlCol3)
                                {
                                    string inner_text = element3.InnerText;
                                    if (!string.IsNullOrEmpty(inner_text))
                                    {
                                        if (inner_text.IndexOf("View your bills") >= 0)
                                        {
                                            utilityviewmodel.target_pathname = element3.GetAttributeValue(SmartParametersV2016.href, "");
                                            if (!string.IsNullOrEmpty(utilityviewmodel.target_pathname))
                                            {
                                                utilityviewmodel.next_routine = "BILLSPAYMENTS";
                                                clicked = true;
                                                goto the_old_days;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        the_old_days:
            if (!clicked)
            {
                utilityviewmodel.next_routine = "LOGOUT";
            }
            return clicked;
        }

        private static void SSE_accordian_content(MainViewModel ourviewmodel,
                                                    UtilityViewModel utilityviewmodel,
                                                    HtmlAgilityPack.HtmlNode element2)
        {
            IList<HtmlAgilityPack.HtmlNode> HtmlCol3,
                                            HtmlCol4;
            string your_account_number = "Your account number",
                   your_tariff = "Your tariff",
                   how_you_paid = "How you paid",
                   on_paperless_billing = "On paperless billing?",
                   the_address_we_supplied = "The address we supplied";
            utilityviewmodel.token = "";

            //Utility.Accounts accounts_row = new SmartUtility.Accounts();
            //bool update_account = false;

            HtmlCol3 = YetAnotherFuckingHoop.SelectNodesAsList(element2, ".//div");
            foreach (HtmlAgilityPack.HtmlNode element3 in HtmlCol3)
            {
                string classname3 = element3.GetAttributeValue("class", "");
                if (SmartNibbyV2016.Check_Classname(classname3, "row", true))
                {
                    HtmlCol4 = YetAnotherFuckingHoop.SelectNodesAsList(element3, ".//p");
                    foreach (HtmlAgilityPack.HtmlNode element4 in HtmlCol4)
                    {
                        utilityviewmodel.token = element4.InnerText;
                        if (!string.IsNullOrEmpty(utilityviewmodel.token))
                        {
                            if (utilityviewmodel.token.IndexOf(your_account_number) >= 0)
                            {
                                utilityviewmodel.token = utilityviewmodel.token.Replace(your_account_number, "");
                                utilityviewmodel.account_no = utilityviewmodel.token;
                            }
                            else
                            {
                                if (utilityviewmodel.token.IndexOf(your_tariff) >= 0)
                                {
                                    utilityviewmodel.token = utilityviewmodel.token.Replace(your_tariff, "");
                                    utilityviewmodel.TARIFF_NAME = utilityviewmodel.token;
                                }
                                else
                                {
                                    if (utilityviewmodel.token.IndexOf(how_you_paid) >= 0)
                                    {
                                        utilityviewmodel.token = utilityviewmodel.token.Replace(how_you_paid, "");
                                        utilityviewmodel.payment_name = utilityviewmodel.token;  // <= Really should be PAYMENT METHOD?
                                    }
                                    else
                                    {
                                        if (utilityviewmodel.token.IndexOf(on_paperless_billing) >= 0)
                                        {
                                            utilityviewmodel.token = utilityviewmodel.token.Replace(on_paperless_billing, "");
                                        }
                                        else
                                        {
                                            if (utilityviewmodel.token.IndexOf(the_address_we_supplied) >= 0)
                                            {
                                                utilityviewmodel.token = utilityviewmodel.token.Replace(the_address_we_supplied, "");
                                                utilityviewmodel.token = utilityviewmodel.token.Replace("\r", "");
                                                utilityviewmodel.token = utilityviewmodel.token.Replace("\n", SmartParametersV2016.space);
                                                utilityviewmodel.account_address = SmartParseV2016.Remove_Double_Spaces_V3(utilityviewmodel.token).Trim();
                                                Update_Address(ourviewmodel,
                                                                utilityviewmodel,
                                                                utilityviewmodel.account_address);
                                                return;
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

        private static void Update_Address(MainViewModel ourviewmodel,
                                                UtilityViewModel utilityviewmodel,
                                                string account_address)
        {
            if (SmartNibbyV2016.Derive_Postcode_New(account_address,
                                                ourviewmodel.Blanche.workingPostcodesList,
                                                utilityviewmodel))
            {
                utilityviewmodel.login_finished = true; // Otherwise the task in General_Meter returns FALSE
                                                        // and we don't send ANYTING to SmartDBserver

                // And its not PA-CIFIC its fucking SPE-CIFIC you idiot bitch
                // Only the first one considered
                SmartUtilityV2022.Amelia_Update_Postcode_Area(ourviewmodel, utilityviewmodel);

            }   // and its FLAHAVANS not FLANNIGANS you thick cow
            return;
        }

        private static bool Accordian_Button(UtilityViewModel utilityviewmodel,
                                            HtmlAgilityPack.HtmlNode element2)
        {
            IList<HtmlAgilityPack.HtmlNode> HtmlCol3,
                                            HtmlCol4;
            //
            // First do the resource
            //
            bool clicked = false;
            HtmlCol3 = YetAnotherFuckingHoop.SelectNodesAsList(element2, ".//div");
            foreach (HtmlAgilityPack.HtmlNode element3 in HtmlCol3)
            {
                string classname3 = element3.GetAttributeValue("class", "");
                if (SmartNibbyV2016.Check_Classname(classname3, "row", true))
                {
                    HtmlCol4 = YetAnotherFuckingHoop.SelectNodesAsList(element3, ".//div");
                    foreach (HtmlAgilityPack.HtmlNode element4 in HtmlCol4)
                    {
                        string inner_text = element4.InnerText;
                        if (!string.IsNullOrEmpty(inner_text))
                        {
                            inner_text = inner_text.Trim();
                            int left_paren = inner_text.IndexOf("(");
                            int right_paren = inner_text.IndexOf(")");
                            if (left_paren >= 0 &&
                                (right_paren >= 0))
                            {
                                inner_text = inner_text.Substring(left_paren);
                                inner_text = inner_text.Replace("(", "");
                                inner_text = inner_text.Replace(")", "");
                                string resource_code = inner_text.Substring(0, 1);
                                // Have we found a resource?
                                if (utilityviewmodel.fuelList.IndexOf(resource_code) == -1)
                                {
                                    // We haven't done this one
                                    if (!string.IsNullOrEmpty(utilityviewmodel.fuelList))
                                    {
                                        // Include a separator
                                        utilityviewmodel.fuelList += SmartParametersV2016.bar;
                                    }
                                    utilityviewmodel.fuelList += resource_code;
                                    utilityviewmodel.resource_code = Convert.ToChar(resource_code);
                                    SmartUtilityV2022.Fix_Resource_Type(utilityviewmodel, SmartUtilityV2022.Determine_Resource_Type(utilityviewmodel)); // For the time being 
                                    clicked = true;
                                    goto finish;
                                }
                            }
                        }
                    }
                }
            }
        finish:
            return clicked;
        }
        private static HtmlAgilityPack.HtmlNode SSE_find_id(HtmlAgilityPack.HtmlNode element1)
        {
            IList<HtmlAgilityPack.HtmlNode>
                        HtmlCol2;
            if (!string.IsNullOrEmpty(element1.Id))
            {
                // The matching criteria ...
                if (element1.Id == "BillsAndPaymentsSection")
                {
                    HtmlCol2 = YetAnotherFuckingHoop.SelectNodesAsList(element1, ".//table");
                    foreach (HtmlAgilityPack.HtmlNode element2 in HtmlCol2)
                    {
                        string classname2 = element2.GetAttributeValue("class", "");
                        if (SmartNibbyV2016.Check_Classname(classname2, "accountTable", true))
                        {
                            return element2;
                        }
                    }
                }
            }
            return null;
        }

        private static async Task<bool> SSE_BillsPayments(
#if ANDROIDX
                                                            AppCompatActivity meterActivity,
#endif
                                                            MainViewModel ourviewmodel,
                                                             UtilityViewModel utilityviewmodel,
                                                             HtmlAgilityPack.HtmlDocument document,
                                                            bool TextBox_Active,
                                                            List<SmartUtility.Payments> payments_tempList)
        {
            // Fuck me!  It fucking WORKED!!!! See the re-direction to this 'secure'
            // routine from the initial fucking Login routine above!!!!!!!!!!!!
            // You ARE a fucking genius Ray!!  An absolute bravest of the brave
            // fucking genius.  Those visionless cunts you saw last Thursday have
            // ABSOLUTELY no idea what they are missing out on ....

            IList<HtmlAgilityPack.HtmlNode>
                        HtmlCol1;

            bool status = true;

            HtmlCol1 = YetAnotherFuckingHoop.SelectNodesAsList(document.DocumentNode, "//div");
            foreach (HtmlAgilityPack.HtmlNode element1 in HtmlCol1)
            {
                HtmlAgilityPack.HtmlNode element2 = SSE_find_id(element1);
                if (element2 != null)
                {
                    string id = element2.GetAttributeValue("id", "");
                    {
                        switch (id)
                        {
                            case "paymentsHistoryTable":
                                if (!SSE_Payments(ourviewmodel,
                                                    utilityviewmodel,
                                                    element2,
                                                    payments_tempList))
                                {
                                    goto quit;
                                }
                                break;
                            case "billingHistoryTable":

                                if (!await SSE_FindBills(
#if ANDROIDX
                                                            meterActivity,
#endif
                                                            ourviewmodel,
                                                            utilityviewmodel,
                                                            element2,
                                                            TextBox_Active))

                                {
                                    goto quit;
                                }
                                break;
                            default:
                                break;
                        }
                    }
                }
            }

            utilityviewmodel.next_routine = "YOURPRODUCTS";
            goto finish;
        quit:
            status = false;
            utilityviewmodel.next_routine = "LOGOUT";
        finish:
            return status;
        }

        private static async Task<bool> SSE_FindBills(
#if ANDROIDX
                                                        AppCompatActivity meterActivity,
#endif
                                                        MainViewModel ourviewmodel,
                                                        UtilityViewModel utilityviewmodel,
                                                        HtmlAgilityPack.HtmlNode element2,
                                                        bool TextBox_Active)
        {
            IList<HtmlAgilityPack.HtmlNode>
                        HtmlCol3,
                        HtmlCol4,
                        HtmlCol5;

            string webpage_id = "";

            HtmlCol3 = YetAnotherFuckingHoop.SelectNodesAsList(element2, ".//tr");
            foreach (HtmlAgilityPack.HtmlNode element3 in HtmlCol3)
            {
                HtmlCol4 = YetAnotherFuckingHoop.SelectNodesAsList(element3, ".//td");
                foreach (HtmlAgilityPack.HtmlNode element4 in HtmlCol4)
                {
                    if (!string.IsNullOrEmpty(element4.InnerText))
                    {
                        if (element4 != null)
                        {
                            string inner_text = element4.InnerText.Trim();     // Get rid of \r and \n 
                            if (inner_text != "Download bill")
                            {
                                webpage_id = inner_text;
                                if (!SmartParseV2016.Generic_Parse_Datetime(webpage_id, utilityviewmodel))
                                {
                                    return false;
                                }
                                // Carry on
                                else
                                {
                                    utilityviewmodel.bill_date = utilityviewmodel.genericTargetDate;
                                }
                            }
                            else
                            {
                                HtmlCol5 = YetAnotherFuckingHoop.SelectNodesAsList(element4, ".//a");
                                foreach (HtmlAgilityPack.HtmlNode element5 in HtmlCol5)
                                {
                                    string href = element5.GetAttributeValue(SmartParametersV2016.href, "");
                                    if (!string.IsNullOrEmpty(href))
                                    {
                                        utilityviewmodel.statement_id = webpage_id;

                                        if (!await SSE_Bills(
#if ANDROIDX
                                                            meterActivity,
#endif
                                                            ourviewmodel,
                                                             utilityviewmodel,
                                                             href,
                                                            TextBox_Active))
                                        {
                                            return false;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return true;
        }
        private static async Task<bool> SSE_Bills(
#if ANDROIDX
                                                    AppCompatActivity meterActivity,
#endif
                                                    MainViewModel ourviewmodel,
                                                    UtilityViewModel utilityviewmodel,
                                                    string href,
                                                    bool TextBox_Active)
        {
            // Fuck me!  It fucking WORKED!!!! See the re-direction to this 'secure'
            // routine from the initial fucking Login routine above!!!!!!!!!!!!
            // You ARE a fucking genius Ray!!  An absolute bravest of the brave
            // fucking genius.  Those visionless cunts you saw last Thursday have
            // ABSOLUTELY no idea what they are missing out on ....

            // Clean up the href a bit ...
            href = href.Replace("&amp;", "&");
            // Have we done this bill (by its date) before??
            if (SmartUtilityV2022.Check_Bill_Date(utilityviewmodel))
            {
                // Statement Date is the one that appears in 'Processed:' line
                //utilityviewmodel.statementid = statementid;
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
                    ourviewmodel.TargetUrl = new Uri(SmartParametersV2016.localWebsite);
                    if (!SmartNibbyV2016.Create_Uri(ourviewmodel,
                                                        utilityviewmodel.prfix_xxx,
                                                        href))           // <= cleaned up

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
                                                                    utilityviewmodel.guid);

                    if (!string.IsNullOrEmpty(ourviewmodel.errorMessage) || !string.IsNullOrEmpty(ourviewmodel.pdfMessage))
                    {
                        return false;
                    }

                    if (!await SmartPDFV2019.Generic_Close_1(

                                                            ourviewmodel,
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
                        if (!await SSE_parse_bill(ourviewmodel,
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
            }
            return true;
        }


        private static bool SSE_Payments(MainViewModel ourviewmodel,
                                            UtilityViewModel utilityviewmodel,
                                            HtmlAgilityPack.HtmlNode element2,
                                            List<SmartUtility.Payments> payments_tempList)
        {
            IList<HtmlAgilityPack.HtmlNode>
                        HtmlCol3,
                        HtmlCol4;

            string inner_text;
            DateTime payment_date = SmartParametersV2016.defaultDate;
            short payment_item = 0,
                    payment_code = 0;
            utilityviewmodel.PAYMENT_METHOD = "";
            utilityviewmodel.PAYMENT_AMOUNT = "";
            int payment_amount = 0,
                    payment_balance = 0;
            HtmlCol3 = YetAnotherFuckingHoop.SelectNodesAsList(element2, ".//tr");
            foreach (HtmlAgilityPack.HtmlNode element3 in HtmlCol3)
            {
                int td_count = 0;
                utilityviewmodel.PAYMENT_METHOD = "Payment received";
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

                HtmlCol4 = YetAnotherFuckingHoop.SelectNodesAsList(element3, ".//td");
                foreach (HtmlAgilityPack.HtmlNode element4 in HtmlCol4)
                {
                    if (!string.IsNullOrEmpty(element4.InnerText))
                    {
                        inner_text = element4.InnerText.Trim();     // Get rid of \r and \n 

                        switch (td_count)
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
                            default:
                                string[] components = inner_text.Split(SmartParametersV2016.semiColon);
                                if (components.Length >= 2)
                                {
                                    utilityviewmodel.PAYMENT_AMOUNT = components[1];
                                    utilityviewmodel.PAYMENT_AMOUNT = utilityviewmodel.PAYMENT_AMOUNT.Replace(utilityviewmodel.bill_currency_separator.ToString(), "");
                                    utilityviewmodel.PAYMENT_AMOUNT = utilityviewmodel.PAYMENT_AMOUNT.Replace(utilityviewmodel.bill_thousands_separator.ToString(), "");
                                    if (!SmartParseV2016.Generic_Parse_Integer(utilityviewmodel.PAYMENT_AMOUNT, utilityviewmodel))
                                    {
                                        return false;
                                    }
                                    else
                                    {
                                        payment_amount = utilityviewmodel.genericTransactionValue;
                                    }
                                }
                                break;
                        }
                        td_count++;
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
            return true;
        }

        internal static async Task<bool> SSE_parse_bill(MainViewModel ourviewmodel,
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
                string[] lines = strText_simple.Split(SmartParametersV2016.newline);

                utilityviewmodel.sections = new[]{
                                        new string[] { "A", "0", "", "", "0", "-1", "" },
                                        new string[] { "B", "0", "", "Here's your statement explained for the period" + "|" +
                                                                    "This statement is for the period" + "|" +
                                                                    "This bill is for the period" + "|" +
                                                                    "For the period:", "-1", "-1", "" },   // Repeat from here
                                        new string[] { "B", "1", "", "Payments" + "|" +
                                                                    "Your payments", "-1", "-1", "X" },
                                        new string[] { "B", "2", "", "Your adjustments", "-1", "-1", "X" },     // Exactly
                                        new string[] { "B", "3", "G", "Converting to kWh", "-1", "-1", "" },
                                        new string[] { "C", "1", "E", "Your electricity use this period" + "|" +
                                                                        "The electricity you've used" +"|" +
                                                                        "Your amended electricity use this period", "-1", "-1", "" },
                                        new string[] { "C", "2", "E", "Your electricity charges this period" +"|" +
                                                                        "Your amended electricity charges this period", "-1", "-1" , ""},
                                        new string[] { "D", "1", "G", "Your gas use this period" + "|" +
                                                                        "The gas you've used" + "|" +
                                                                        "Your amended gas use this period", "-1", "-1", "" },
                                        new string[] { "D", "2", "G", "Your gas charges this period" + "|" +
                                                                        "Your amended gas charges this period", "-1", "-1" , ""},
                                      };
                // This is always needed anyway because we always do Page1 with this
                // Sometimes ... the Bill Date is never extracted from the PDF text!
                // But when (if) we ever find it - we substitute the Bill Number for it
                // IF we haven't found the Bill number ...
                int start_line = 0,
                    matching_line = 0;
                bool success = Parse_Page0(ourviewmodel,
                                                utilityviewmodel,
                                                lines,
                                                start_line);
                if (!success)
                {
                    status = false;
                }
                else
                {
                    bool repeat = true;
                    while (repeat)
                    {

                        if (!await Parse_Pages(ourviewmodel,
                                            utilityviewmodel,
                                            lines))

                        {
                            status = false;
                            break;
                        }
                        else
                        {
                            // Very Important Routine!!!
                            if (matching_line == lines.Length)
                            {
                                break;
                            }
                            start_line = matching_line;
                            SmartParseV2016.Sort_Sections_New(utilityviewmodel, lines, start_line);
                        }
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
                                string[] lines,
                                int start_line)
        {
            // Not perfect but not a bad start
            bool status = true;

            // Very Important Routine!!!
            SmartParseV2016.Sort_Sections_New(utilityviewmodel, lines, start_line);

            // Pass 1 - look for the Big Three in Section 0
            if (!Parse_SectionA0(ourviewmodel, utilityviewmodel, utilityviewmodel.sections, "A", "0", lines))
            {
                ourviewmodel.errorMessage = "A0" + SmartParametersV2016.bar + ourviewmodel.errorMessage;
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

            // Your bill for the period / For the period: (Repeating)
            if (!Parse_SectionB0(ourviewmodel, utilityviewmodel, utilityviewmodel.sections, "B", "0", lines))
            {
                ourviewmodel.errorMessage = "B0" + SmartParametersV2016.bar + ourviewmodel.errorMessage;
                return false;
            }

            // Payments
            if (!Parse_SectionB1(ourviewmodel, utilityviewmodel, utilityviewmodel.sections, "B", "1", lines))
            {
                ourviewmodel.errorMessage = "B1" + SmartParametersV2016.bar + ourviewmodel.errorMessage;
                return false;
            }

            // Adjustments
            if (!Parse_SectionB2(ourviewmodel, utilityviewmodel, utilityviewmodel.sections, "B", "2", lines))
            {
                ourviewmodel.errorMessage = "B2" + SmartParametersV2016.bar + ourviewmodel.errorMessage;
                return false;
            }

            // Converting to kWh
            if (!Parse_SectionB3(utilityviewmodel, utilityviewmodel.sections, "B", "3", lines))
            {
                ourviewmodel.errorMessage = "B3" + SmartParametersV2016.bar + ourviewmodel.errorMessage;
                return false;
            }
            // E Readings
            if (!Parse_SectionC1(ourviewmodel, utilityviewmodel, utilityviewmodel.sections, "C", "1", lines))     // Electricity use
            {
                ourviewmodel.errorMessage = "C1" + SmartParametersV2016.bar + ourviewmodel.errorMessage;
                return false;
            }
            // Tariff and E Unit Charges and E Standing Charges

            if (!await Parse_SectionC2(ourviewmodel, utilityviewmodel, utilityviewmodel.sections, "C", "2", lines)) // Electricity charges

            {
                ourviewmodel.errorMessage = "C2" + SmartParametersV2016.bar + ourviewmodel.errorMessage;
                return false;
            }
            if (!Parse_SectionD1(ourviewmodel, utilityviewmodel, utilityviewmodel.sections, "D", "1", lines))     // Gas usage
            {
                ourviewmodel.errorMessage = "D1" + SmartParametersV2016.bar + ourviewmodel.errorMessage;
                return status;
            }
            // Tariff and G Unit Charges and G Standing Charges

            if (!await Parse_SectionD2(ourviewmodel, utilityviewmodel, utilityviewmodel.sections, "D", "2", lines)) // Electricity charges

            {
                ourviewmodel.errorMessage = "D2" + SmartParametersV2016.bar + ourviewmodel.errorMessage;
                return false;
            }
            return status;
        }

        private static bool Get_Account_No(UtilityViewModel utilityviewmodel,
                                            int line_count,
                                            string[] lines)
        {
            string your_account_number = "Your account number",
                    your_resource_account_number = "";


            switch (utilityviewmodel.resource_code)
            {
                case SmartParametersV2016.Electricity:
                    your_resource_account_number = "Your electricity account number:";
                    break;

                case SmartParametersV2016.Gas:
                    your_resource_account_number = "Your gas account number:";
                    break;
                default:
                    break;
            }

            if (SmartParseV2016.Token_Identify(utilityviewmodel, your_account_number, false) ||
                SmartParseV2016.Token_Identify(utilityviewmodel, your_resource_account_number, false))
            {
                if (string.IsNullOrEmpty(utilityviewmodel.ACCOUNT_NO))
                {
                    string temp_account = SmartParseV2016.Find_Account(utilityviewmodel, line_count, lines);
                    if (!string.IsNullOrEmpty(temp_account))
                    {
                        utilityviewmodel.ACCOUNT_NO = temp_account;
                        if (string.IsNullOrEmpty(utilityviewmodel.account_no))
                        {
                            // Fix up scraped account number if it needs it
                            utilityviewmodel.account_no = utilityviewmodel.ACCOUNT_NO;
                        }
                        return true;
                    }
                }
            }
            return false;
        }

        private static bool Get_Bill_No(MainViewModel ourviewmodel,
                                            UtilityViewModel utilityviewmodel,
                                            string token,
                                            int line_count,
                                            string[] lines)
        {
            string statement_date = "Statement date:",
                    dated = "Dated:";

            if (SmartParseV2016.Token_Identify(utilityviewmodel, statement_date, true) ||
                SmartParseV2016.Token_Identify(utilityviewmodel, dated, true))
            {
                utilityviewmodel.BILL_DATE = SmartParseV2016.Find_Bill_Date(utilityviewmodel, line_count, lines, SmartParametersV2016.defaultDates, token);
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
            return false;
        }

        private static bool Get_Payment_Plan_V2(MainViewModel ourviewmodel,
                                            UtilityViewModel utilityviewmodel,
                                            string token)
        {
            // This method SHOULD be around in 2 years time ...
            string payment_method = "Payment method";

            if (SmartParseV2016.Token_Identify(utilityviewmodel, payment_method, true))
            {
                string PAYMENT_TYPE = token.Trim();
                if (SmartSpikeUtilityV2017.Utility_Lookup_PaymentPlan(ourviewmodel,
                                                        utilityviewmodel,
                                                        PAYMENT_TYPE))
                {
                    SmartParseV2016.Update_Bill(ourviewmodel, utilityviewmodel, "PAYMENT_PLAN", utilityviewmodel.PAYMENT_PLAN.ToString());
                    SmartParseV2016.Update_Bill(ourviewmodel, utilityviewmodel, "PAYMENT_TYPE", PAYMENT_TYPE);
                    return true;
                }
            }
            return false;
        }

        private static bool Get_Payment_Plan_V1(MainViewModel ourviewmodel,
                                            UtilityViewModel utilityviewmodel,
                                            string[] lines,
                                            int upper_limit,
                                            int temp_count)
        {
            // This approach is deprecated and won't be around in 5 years time?
            string your_rewards = "Your rewards";

            if (SmartParseV2016.Token_Identify(utilityviewmodel, your_rewards, true))
            {
                //string temp_message = "";
                int sub_line_count = temp_count;
                while (sub_line_count <= upper_limit)
                {
                    utilityviewmodel.token = lines[sub_line_count].Trim();
                    string PAYMENT_TYPE = utilityviewmodel.token.Trim();
                    if (SmartSpikeUtilityV2017.Utility_Lookup_PaymentPlan(ourviewmodel,
                                                            utilityviewmodel,
                                                            PAYMENT_TYPE))
                    {
                        SmartParseV2016.Update_Bill(ourviewmodel, utilityviewmodel, "PAYMENT_PLAN", utilityviewmodel.PAYMENT_PLAN.ToString());
                        SmartParseV2016.Update_Bill(ourviewmodel, utilityviewmodel, "PAYMENT_TYPE", PAYMENT_TYPE);
                        return true;
                    }
                    sub_line_count++;
                }
            }
            return false;
        }

        private static bool Parse_SectionA0(MainViewModel ourviewmodel,
                                            UtilityViewModel utilityviewmodel,
                                            string[][] sections,
                                            string section,
                                            string sub_section,
                                            string[] lines)
        {
            // Not perfect but not a bad start
            int section_index = 0;
            if (!SmartParseV2016.Decode_Section(section, sub_section, sections, utilityviewmodel.resource_code.ToString(), utilityviewmodel))
            {
                return true;
            }

            // Pass 0 - Clear out some shit
            utilityviewmodel.token = "";

            bool postcode_found = false;

            string tariff_name = "Tariff name";

            char PAYMENT_PLAN = SmartParametersV2016.defaultChar;
            bool RESOURCE_BALANCES = false; // Default
            int TARIFF_CODE = 0,
                    NEW_CHARGES = 0,
                    RESOURCE_DISCOUNTS = 0,
                    RESOURCE_VAT_AMOUNT = 0;
            string PREVIOUS_BALANCE = "0",
                    PAYMENTS_RECEIVED = "0",
                    ACCOUNT_CHARGES_CREDITS = "0",
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

            DateTime temp_date;

            // Pass 1 get the Big Three (or four)
            for (int line_count = Convert.ToInt32(sections[section_index][4]); line_count <= Convert.ToInt32(sections[section_index][5]); line_count++)
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
                        goto the_old_daysA0;
                    }
                }

                if (Get_Account_No(utilityviewmodel, line_count, lines))
                {
                    goto the_old_daysA0;
                }

                //if (SmartParseV2016.Token_Identify(utilityviewmodel, for_customer_service, false))
                //{
                //    index = index + 1;
                //    utilityviewmodel.CONTACT_TEL_NO = lines[index].Trim(' ');
                //}

                if (Get_Bill_No(ourviewmodel, utilityviewmodel, utilityviewmodel.token, line_count, lines))
                {
                    goto the_old_daysA0;
                }

                if (SmartParseV2016.Token_Identify(utilityviewmodel, tariff_name, true))
                {
                    goto the_old_daysA0;
                }

                if (utilityviewmodel.PAYMENT_PLAN == SmartParametersV2016.defaultChar)
                {
                    if (Get_Payment_Plan_V2(ourviewmodel, utilityviewmodel, utilityviewmodel.token))
                    {
                        goto the_old_daysA0;
                    }
                    else
                    {
                        if (Get_Payment_Plan_V1(ourviewmodel, utilityviewmodel, lines, Convert.ToInt32(sections[section_index][5]), line_count))
                        {
                            goto the_old_daysA0;
                        }
                    }
                }

                if (!Outstanding(ourviewmodel, utilityviewmodel, utilityviewmodel.token))
                {
                    return false;
                }
                else
                {
                    //utilityviewmodel.token = utilityviewmodel.token;
                    goto the_old_daysA0;
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
            return true;
        }

        private static bool Outstanding(MainViewModel ourviewmodel,
                                        UtilityViewModel utilityviewmodel,
                                        string token)
        {
            string the_balance_we_owe_you_is = "The balance we owe you is",
                    the_balance_you_owe_us_is = "The balance you owe us is";

            if (SmartParseV2016.Token_Identify(utilityviewmodel, the_balance_we_owe_you_is, false) ||
                SmartParseV2016.Token_Identify(utilityviewmodel, the_balance_you_owe_us_is, false))
            {
                string OUTSTANDING_BALANCE;

                token = SmartParseV2016.Remove_Double_Spaces_V3(token);
                int currency_index = token.IndexOf(utilityviewmodel.bill_currency_symbol);
                if (currency_index >= 0)
                {
                    if (token.IndexOf(the_balance_we_owe_you_is) >= 0)
                    {
                        token = token.Substring(currency_index) + SmartParametersV2016.space + "credit";
                    }
                    else
                    {
                        token = token.Substring(currency_index) + SmartParametersV2016.space + "debit";
                    }
                    if (!SmartParseV2016.Currency_Strip(utilityviewmodel.bill_currency_symbol, utilityviewmodel.bill_currency_separator, utilityviewmodel.bill_thousands_separator, token, utilityviewmodel))
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
                    //    temp = utilityviewmodel.genericTransactionValue;
                    //}
                    SmartParseV2016.Update_Bill(ourviewmodel, utilityviewmodel, "OUTSTANDING_BALANCE", OUTSTANDING_BALANCE);
                }
            }
            utilityviewmodel.token = token;
            return true;
        }

        private static bool Parse_SectionB0(MainViewModel ourviewmodel,
                                            UtilityViewModel utilityviewmodel,
                                            string[][] sections,
                                            string section,
                                            string sub_section,
                                            string[] lines)
        {
            // Not perfect but not a bad start
            int section_index = 0;
            if (!SmartParseV2016.Decode_Section(section, sub_section, sections, utilityviewmodel.resource_code.ToString(), utilityviewmodel))
            {
                return true;
            }

            utilityviewmodel.token = "";

            string this_statement_is_for_the_period = "This statement is for the period",
                    this_bill_is_for_the_period = "This bill is for the period",
                    for_the_period = "For the period:";

            DateTime temp_date;

            for (int line_count = Convert.ToInt32(sections[section_index][4]); line_count <= Convert.ToInt32(sections[section_index][5]); line_count++)
            {
                utilityviewmodel.token = lines[line_count].Trim();

                if (SmartParseV2016.Token_Identify(utilityviewmodel, this_statement_is_for_the_period, true) ||
                    SmartParseV2016.Token_Identify(utilityviewmodel, this_bill_is_for_the_period, true) ||
                    SmartParseV2016.Token_Identify(utilityviewmodel, for_the_period, true) ||
                    SmartParseV2016.Token_Identify(utilityviewmodel, "Here's your statement explained for the period", true))
                {
                    utilityviewmodel.token = utilityviewmodel.token.Replace("and", "");
                    utilityviewmodel.token = SmartParseV2016.Remove_Double_Spaces_V3(utilityviewmodel.token);
                    int to_index = utilityviewmodel.token.IndexOf(" to ");
                    if (to_index >= 0)
                    {
                        utilityviewmodel.token = utilityviewmodel.token.Replace(" to ", SmartParametersV2016.bar.ToString());
                        utilityviewmodel.token = utilityviewmodel.token.Replace(",", SmartParametersV2016.bar.ToString());
                        string[] components = utilityviewmodel.token.Split(SmartParametersV2016.bar);
                        if (components.Length >= 2)
                        {
                            if (!SmartParseV2016.Generic_Parse_Datetime(components[0], utilityviewmodel))
                            {
                                return false;
                            }
                            else
                            {
                                temp_date = utilityviewmodel.genericTargetDate;
                            }
                            if (utilityviewmodel.BILL_PERIOD_START == SmartParametersV2016.defaultDates)    // Only if
                            {
                                SmartParseV2016.Update_Bill(ourviewmodel, utilityviewmodel, "BILL_PERIOD_START", temp_date.ToString(SmartParametersV2016.defaultCulture));
                            }
                            utilityviewmodel.BILL_PERIOD_START = temp_date.ToString(SmartParametersV2016.defaultCulture);
                            if (!SmartParseV2016.Generic_Parse_Datetime(components[1], utilityviewmodel))
                            {
                                return false;
                            }
                            else
                            {
                                temp_date = utilityviewmodel.genericTargetDate;
                            }
                            utilityviewmodel.BILL_PERIOD_END = temp_date.ToString(SmartParametersV2016.defaultCulture);
                            SmartParseV2016.Update_Bill(ourviewmodel, utilityviewmodel, "BILL_PERIOD_END", temp_date.ToString(SmartParametersV2016.defaultCulture));
                        }
                    }
                    goto the_old_daysB0;
                }

                if (Get_Account_No(utilityviewmodel, line_count, lines))
                {
                    SmartParseV2016.Update_Bill(ourviewmodel, utilityviewmodel, "ACCOUNT_NO", utilityviewmodel.ACCOUNT_NO);
                    goto the_old_daysB0;
                }

                if (Get_Bill_No(ourviewmodel, utilityviewmodel, utilityviewmodel.token, line_count, lines))
                {
                    SmartParseV2016.Update_Bill(ourviewmodel, utilityviewmodel, "BILL_DATE", utilityviewmodel.BILL_DATE);
                    goto the_old_daysB0;
                }

                if (SmartParseV2016.Token_Identify(utilityviewmodel, "Total from last statement", true) ||
                    SmartParseV2016.Token_Identify(utilityviewmodel, "Total from last cancelled statement", true) ||
                    SmartParseV2016.Token_Identify(utilityviewmodel, "We owed you", true) ||
                    SmartParseV2016.Token_Identify(utilityviewmodel, "You owed us", true))
                {
                    string PREVIOUS_BALANCE;// = "0";

                    utilityviewmodel.token = SmartParseV2016.Remove_Double_Spaces_V3(utilityviewmodel.token);
                    utilityviewmodel.prfix = "";
                    int sub_line_count = line_count;
                    int currency_index = Bump(utilityviewmodel,
                                                sections,
                                                section_index,
                                                lines,
                                                utilityviewmodel.token,
                                                utilityviewmodel.prfix,
                                                ref sub_line_count);
                    line_count = utilityviewmodel.temp_count;
                    //utilityviewmodel.token = utilityviewmodel.token;
                    //utilityviewmodel.prfix = utilityviewmodel.prfix;
                    if (currency_index >= 0)
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
                        if (utilityviewmodel.Hezbollah.bills_row.PREVIOUS_BALANCE == 0)
                        {
                            SmartParseV2016.Update_Bill(ourviewmodel, utilityviewmodel, "PREVIOUS_BALANCE", PREVIOUS_BALANCE);
                        }
                    }
                    goto the_old_daysB0;
                }

                if (!Outstanding(ourviewmodel, utilityviewmodel, utilityviewmodel.token))
                {
                    return false;
                }
                else
                {
                    //utilityviewmodel.token = utilityviewmodel.token;
                    goto the_old_daysB0;
                }

            the_old_daysB0:
                continue;
            }
            sections[section_index][4] = "-1";  // Reset the switch
            return true;
        }

        private static bool Parse_SectionB1(MainViewModel ourviewmodel,
                                            UtilityViewModel utilityviewmodel,
                                            string[][] sections,
                                            string section,
                                            string sub_section,
                                            string[] lines)
        {
            // Not perfect but not a bad start
            int section_index = 0;
            if (!SmartParseV2016.Decode_Section(section, sub_section, sections, utilityviewmodel.resource_code.ToString(), utilityviewmodel))
            {
                return true;
            }

            utilityviewmodel.token = "";

            string payments = "Payments",
                    your_payments = "Your payments";
            utilityviewmodel.PAYMENT_AMOUNT = "0";
            utilityviewmodel.PAYMENT_METHOD = "";
            utilityviewmodel.PAYMENT_DATE = "";
            utilityviewmodel.PAYMENT_BALANCE = "0";

            for (int line_count = Convert.ToInt32(sections[section_index][4]); line_count <= Convert.ToInt32(sections[section_index][5]); line_count++)
            {
                utilityviewmodel.token = lines[line_count].Trim();

                if (SmartParseV2016.Token_Identify(utilityviewmodel, payments, true) ||
                    SmartParseV2016.Token_Identify(utilityviewmodel, your_payments, true))
                {
                    int sub_line_count = line_count;
                    while (sub_line_count <= Convert.ToInt32(sections[section_index][5]))
                    {
                        utilityviewmodel.token = lines[sub_line_count].Trim();

                        if (utilityviewmodel.token.IndexOf("Total from last statement") >= 0 ||
                            utilityviewmodel.token.IndexOf("Total from last cancelled statement") >= 0)
                        {
                            goto keep_looping;
                        }

                        if ((utilityviewmodel.token.IndexOf("Your total payments") >= 0) ||
                            (utilityviewmodel.token.IndexOf("Less your total payments") >= 0))
                        {
                            break;
                        }

                        if (utilityviewmodel.token.IndexOf(utilityviewmodel.bill_currency_symbol) >= 0)
                        {
                            utilityviewmodel.PAYMENT_AMOUNT = "";
                            utilityviewmodel.PAYMENT_METHOD = "";
                            utilityviewmodel.PAYMENT_DATE = "";

                            utilityviewmodel.token = SmartParseV2016.Remove_Double_Spaces_V3(utilityviewmodel.token);
                            if (utilityviewmodel.token.IndexOf(" credit") == -1)
                            {
                                utilityviewmodel.token += " debit";
                            }
                            utilityviewmodel.token = utilityviewmodel.token.Replace(SmartParametersV2016.space, SmartParametersV2016.bar.ToString());

                            if (!SectionB1_Check_Payments(utilityviewmodel,
                                                            utilityviewmodel.token))
                            {
                                return false;
                            }

                            if (!string.IsNullOrEmpty(utilityviewmodel.PAYMENT_METHOD))
                            {
                                utilityviewmodel.PAYMENT_CODE = "";
                                if (!SmartSpikeUtilityV2017.Utility_Lookup_PaymentCode(ourviewmodel, utilityviewmodel, utilityviewmodel.PAYMENT_METHOD))
                                {
                                    return false;
                                }
                                if (!string.IsNullOrEmpty(utilityviewmodel.PAYMENT_DATE) &&
                                    !string.IsNullOrEmpty(utilityviewmodel.PAYMENT_AMOUNT))
                                {
                                    //utilityviewmodel.PAYMENT_METHOD = utilityviewmodel.PAYMENT_METHOD;
                                    // Add it in ... perhaps
                                    // If this section is E(lectricity) and G(as) then we might be doing it twice!
                                    // Check if its there first
                                    utilityviewmodel.PAYMENT_BALANCE = utilityviewmodel.PAYMENTS_BALANCE.ToString();
                                    if (!SmartParseV2016.Check_Payment_Is_There(ourviewmodel,
                                                                                utilityviewmodel))
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
                        }
                    keep_looping:
                        sub_line_count++;
                    }
                    line_count = sub_line_count - 1;    // Re-do the line
                    goto the_old_daysB1;
                }
            the_old_daysB1:
                continue;
            }
            sections[section_index][4] = "-1";      // Reset the switch
            return true;
        }

        private static bool SectionB1_Check_Payments(UtilityViewModel utilityviewmodel,
                                            string token)
        {
            string[] components = token.Split(SmartParametersV2016.bar);
            // Work backwards ...
            int component_count = components.Length - 1;
            int times_through = 0;
            while (component_count >= 0)
            {
                switch (times_through)
                {
                    case 0:
                        // Credit or Debit
                        utilityviewmodel.PAYMENT_AMOUNT = components[component_count].Trim();
                        break;
                    case 1:
                        // Amount
                        utilityviewmodel.PAYMENT_AMOUNT = components[component_count].Trim() + SmartParametersV2016.space + utilityviewmodel.PAYMENT_AMOUNT;
                        if (!SmartParseV2016.Currency_Strip(utilityviewmodel.bill_currency_symbol, utilityviewmodel.bill_currency_separator, utilityviewmodel.bill_thousands_separator, utilityviewmodel.PAYMENT_AMOUNT, utilityviewmodel))
                        {
                            return false;
                        }
                        utilityviewmodel.PAYMENT_AMOUNT = utilityviewmodel.value;
                        if (!SmartParseV2016.Generic_Parse_Integer(utilityviewmodel.PAYMENT_AMOUNT, utilityviewmodel))
                        {
                            return false;
                        }
                        break;
                    case 2:
                        // Year
                        utilityviewmodel.PAYMENT_DATE = components[component_count].Trim();
                        break;
                    case 3:
                        // Month
                        utilityviewmodel.PAYMENT_DATE = components[component_count].Trim() + SmartParametersV2016.space + utilityviewmodel.PAYMENT_DATE;
                        break;
                    case 4:
                        // Day
                        utilityviewmodel.PAYMENT_DATE = components[component_count].Trim() + SmartParametersV2016.space + utilityviewmodel.PAYMENT_DATE;
                        if (!SmartParseV2016.Generic_Parse_Datetime(utilityviewmodel.PAYMENT_DATE, utilityviewmodel))
                        {
                            return false;
                        }
                        break;
                    default:
                        if (string.IsNullOrEmpty(utilityviewmodel.PAYMENT_METHOD))
                        {
                            utilityviewmodel.PAYMENT_METHOD = components[component_count].Trim();
                        }
                        else
                        {
                            utilityviewmodel.PAYMENT_METHOD = components[component_count].Trim() + SmartParametersV2016.space + utilityviewmodel.PAYMENT_METHOD;
                        }
                        break;

                }
                times_through++;
                component_count--;
            }
            return true;
        }

        // Adjustments
        private static bool Parse_SectionB2(MainViewModel ourviewmodel,
                                            UtilityViewModel utilityviewmodel,
                                            string[][] sections,
                                            string section,
                                            string sub_section,
                                            string[] lines)
        {
            // Not perfect but not a bad start
            int section_index = 0;
            if (!SmartParseV2016.Decode_Section(section, sub_section, sections, utilityviewmodel.resource_code.ToString(), utilityviewmodel))
            {
                return true;
            }

            utilityviewmodel.token = "";

            string ADJUSTMENT_LINE = "";
            for (int line_count = Convert.ToInt32(sections[section_index][4]); line_count <= Convert.ToInt32(sections[section_index][5]); line_count++)
            {
                utilityviewmodel.token = lines[line_count].Trim();
                if (SmartParseV2016.Token_Identify(utilityviewmodel, "Total Adjustments", true))
                {
                    // Now try and find the Payment Plan ...
                    if (utilityviewmodel.PAYMENT_PLAN == SmartParametersV2016.defaultChar)
                    {
                        int sub_line_count = line_count;
                        while (sub_line_count <= Convert.ToInt32(sections[section_index][5]))
                        {
                            utilityviewmodel.token = lines[sub_line_count].Trim();
                            if (Get_Payment_Plan_V2(ourviewmodel, utilityviewmodel, utilityviewmodel.token))
                            {
                                break;
                            }
                            else
                            {
                                if (Get_Payment_Plan_V1(ourviewmodel, utilityviewmodel, lines, Convert.ToInt32(sections[section_index][5]), sub_line_count))
                                {
                                    break;
                                }
                            }
                            sub_line_count++;
                        }
                    }
                    break;
                }

                if (SmartParseV2016.Token_Identify(utilityviewmodel, "Your adjustments", true))
                {
                    goto the_old_daysB2;
                }

                if (SmartParseV2016.Token_Identify(utilityviewmodel, "Your total adjustments", true))
                {
                    goto the_old_daysB2;
                }

                if (utilityviewmodel.token.IndexOf(utilityviewmodel.bill_currency_symbol) == -1)
                {
                    ADJUSTMENT_LINE = ADJUSTMENT_LINE + SmartParametersV2016.space + utilityviewmodel.token;
                }
                else
                {
                    ADJUSTMENT_LINE = ADJUSTMENT_LINE + SmartParametersV2016.space + utilityviewmodel.token;
                    utilityviewmodel.ACCOUNT_TYPE = "";
                    utilityviewmodel.ACCOUNT_AMOUNT = "";
                    utilityviewmodel.ACCOUNT_DATE = SmartParametersV2016.defaultDates;
                    short ACCOUNT_VAT_CODE = SmartParametersV2016.zeroRateVatCode;

                    utilityviewmodel.token = ADJUSTMENT_LINE.Trim();

                    utilityviewmodel.token = SmartParseV2016.Remove_Double_Spaces_V3(utilityviewmodel.token);
                    if (utilityviewmodel.token.IndexOf(" credit") == -1)
                    {
                        utilityviewmodel.token += " debit";
                    }
                    utilityviewmodel.token = utilityviewmodel.token.Replace(SmartParametersV2016.space, SmartParametersV2016.bar.ToString());

                    if (!SectionB2_Check_Account_Charges(utilityviewmodel,
                                                        utilityviewmodel.token))
                    {
                        return false;
                    }


                    if ((utilityviewmodel.ACCOUNT_DATE != SmartParametersV2016.defaultDates) &&
                        (utilityviewmodel.ACCOUNT_AMOUNT != ""))
                    {
                        // Add it in ... perhaps
                        // If this section is E(lectricity) and G(as) then we might be doing it twice!
                        // Check if its there first
                        // Need to increment this for multiple inserts on the same Bill date
                        string INCLUDE_BILLS = SmartParametersV2016.yesFlag;

                        utilityviewmodel.ACCOUNT_CHARGES_CREDITS_ITEM = (short)(utilityviewmodel.ACCOUNT_CHARGES_CREDITS_ITEM + 1);
                        bool is_it_there = false;
                        {
                            // Check if neither Electricity nor Gas
                            if (string.IsNullOrEmpty(sections[section_index][2]))
                            {
                                is_it_there = SmartParseV2016.Check_Adjustment_Is_There(ourviewmodel,
                                                                                utilityviewmodel,
                                                                                Convert.ToDateTime(utilityviewmodel.ACCOUNT_DATE),
                                                                                utilityviewmodel.ACCOUNT_CHARGES_CREDITS_ITEM,
                                                                                utilityviewmodel.ACCOUNT_TYPE,
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
                                                                                Convert.ToDateTime(utilityviewmodel.ACCOUNT_DATE),
                                                                                utilityviewmodel.ACCOUNT_CHARGES_CREDITS_ITEM,
                                                                                utilityviewmodel.ACCOUNT_TYPE,
                                                                                ACCOUNT_VAT_CODE,
                                                                                Convert.ToInt32(utilityviewmodel.ACCOUNT_AMOUNT),
                                                                                INCLUDE_BILLS);
                            SmartParseV2016.Update_Bill(ourviewmodel, utilityviewmodel, "ACCOUNT_CHARGES_CREDITS", utilityviewmodel.ACCOUNT_AMOUNT);
                            // Note: ADJUSTMENT_AMOUNT has already been checked as an Integer
                        }
                    }
                    goto the_old_daysB2;
                }
            the_old_daysB2:
                continue;
            }
            sections[section_index][4] = "-1";  // Reset the switch
            return true;
        }

        private static bool SectionB2_Check_Account_Charges(UtilityViewModel utilityviewmodel,
                                                            string token)
        {
            string[] components = token.Split(SmartParametersV2016.bar);
            // Work backwards ...
            int component_count = components.Length - 1;
            int times_through = 0;

            string temps = "",
                    tempd = "";

            while (component_count >= 0)
            {
                switch (times_through)
                {
                    case 0:
                        // Credit or Debit
                        temps = components[component_count].Trim();
                        break;
                    case 1:
                        // Amount
                        temps = components[component_count].Trim() + SmartParametersV2016.space + temps;
                        if (!SmartParseV2016.Currency_Strip(utilityviewmodel.bill_currency_symbol, utilityviewmodel.bill_currency_separator, utilityviewmodel.bill_thousands_separator, temps, utilityviewmodel))
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
                        utilityviewmodel.ACCOUNT_AMOUNT = temps;
                        break;
                    case 2:
                        // Year
                        tempd = components[component_count].Trim();
                        break;
                    case 3:
                        // Month
                        tempd = components[component_count].Trim() + SmartParametersV2016.space + tempd;
                        break;
                    case 4:
                        // Day
                        tempd = components[component_count].Trim() + SmartParametersV2016.space + tempd;
                        if (!SmartParseV2016.Generic_Parse_Datetime(tempd, utilityviewmodel))
                        {
                            return false;
                        }
                        utilityviewmodel.ACCOUNT_DATE = tempd;
                        break;
                    default:
                        if (string.IsNullOrEmpty(utilityviewmodel.ACCOUNT_TYPE))
                        {
                            utilityviewmodel.ACCOUNT_TYPE = components[component_count].Trim();
                        }
                        else
                        {
                            utilityviewmodel.ACCOUNT_TYPE = components[component_count].Trim() + SmartParametersV2016.space + utilityviewmodel.ACCOUNT_TYPE;
                        }
                        break;
                }
                times_through++;
                component_count--;
            }
            return true;
        }
        // Converting to kWh
        private static bool Parse_SectionB3(UtilityViewModel utilityviewmodel,
                                            string[][] sections,
                                            string section,
                                            string sub_section,
                                            string[] lines)
        {
            // Not perfect but not a bad start
            int section_index = 0;
            if (!SmartParseV2016.Decode_Section(section, sub_section, sections, utilityviewmodel.resource_code.ToString(), utilityviewmodel))
            {
                return true;
            }

            utilityviewmodel.token = "";

            string calorific_value = "calorific value";
            //decimal temp_decimal = 0.0M;

            for (int line_count = Convert.ToInt32(sections[section_index][4]); line_count <= Convert.ToInt32(sections[section_index][5]); line_count++)
            {
                utilityviewmodel.token = lines[line_count].Trim();

                if (SmartParseV2016.Token_Identify_End(utilityviewmodel, calorific_value, true))
                {
                    utilityviewmodel.CALORIFIC_VALUE = utilityviewmodel.token.Replace("x", "").Trim();
                    if (!SmartParseV2016.Generic_Parse_Decimal(utilityviewmodel.CALORIFIC_VALUE, utilityviewmodel))
                    {
                        return false;
                    }
                    else
                    {
                        utilityviewmodel.CALORIFIC_VALUE = utilityviewmodel.genericDecimalValue.ToString();
                    }
                    break;
                }
            }
            sections[section_index][4] = "-1";      // Reset the switch
            return true;
        }
        private static bool Parse_SectionC1(MainViewModel ourviewmodel,
                                            UtilityViewModel utilityviewmodel,
                                            string[][] sections,
                                            string section,
                                            string sub_section,
                                            string[] lines)
        {
            // Not perfect but not a bad start
            int section_index = 0;
            if (!SmartParseV2016.Decode_Section(section, sub_section, sections, utilityviewmodel.resource_code.ToString(), utilityviewmodel))
            {
                return true;
            }

            string token = "";

            DateTime READINGS_PERIOD_START = SmartParametersV2016.defaultDate,
                    READINGS_PERIOD_END = SmartParametersV2016.defaultDate;

            string METER_SERIAL_NO = "",
                    THIS_READ_TYPE = "",
                    UNIT_OF_MEASURE = "";
            string your_electricity_use_this_period = "Your electricity use this period",
                    your_amended_electricity_use_this_period = "Your amended electricity use this period",
                    the_electricity_youve_used = "The electricity you've used",
                    meter_number_colon = "Meter:",
                    units = "units";
            int temp = 0;
            DateTime temp_date = SmartParametersV2016.defaultDate;

            for (int line_count = Convert.ToInt32(sections[section_index][4]); line_count <= Convert.ToInt32(sections[section_index][5]); line_count++)
            {
                token = lines[line_count].Trim();

                if (SmartParseV2016.Token_Identify(utilityviewmodel, your_electricity_use_this_period, true) ||
                    SmartParseV2016.Token_Identify(utilityviewmodel, your_amended_electricity_use_this_period, true) ||
                    SmartParseV2016.Token_Identify(utilityviewmodel, the_electricity_youve_used, true))
                {
                    token = token.Replace(SmartParametersV2016.dash, "").Trim();
                    if (string.IsNullOrEmpty(token))
                    {
                        line_count++;
                        token = lines[line_count].Trim();

                    }
                    if (token.Length > 1)
                    {
                        THIS_READ_TYPE = char.ToUpper(token[0]) + token.Substring(1);
                    }
                    READINGS_PERIOD_START = SmartTimeV2016.ConvertDateTime(utilityviewmodel.BILL_PERIOD_START);
                    READINGS_PERIOD_END = SmartTimeV2016.ConvertDateTime(utilityviewmodel.BILL_PERIOD_END);
                    goto the_old_daysC1;
                }

                if (SmartParseV2016.Token_Identify(utilityviewmodel, meter_number_colon, true))
                {
                    METER_SERIAL_NO = token.Trim();
                    goto the_old_daysC1;
                }

                if (SmartParseV2016.Token_Identify_End(utilityviewmodel, SmartParametersV2016.space + units, false) ||
                    SmartParseV2016.Token_Identify_End(utilityviewmodel, SmartParametersV2016.defaultUoM, false))
                {
                    string D_THIS_READ = "0",
                            D_LAST_READ = "0",
                            D_UNITS_USED_KWH = "0";
                    string N_THIS_READ = "0",
                            N_LAST_READ = "0",
                            N_UNITS_USED_KWH = "0";

                    string UNITS_TYPE = "";
                    int square_bracket = 0;

                    token = SmartParseV2016.Remove_Double_Spaces_V3(token);
                    token = token.Replace(units, SmartParametersV2016.defaultUoM);
                    token = token.Replace(SmartParametersV2016.decimalPoint +
                                            SmartParametersV2016.space +
                                            SmartParametersV2016.defaultUoM,
                                            SmartParametersV2016.space + SmartParametersV2016.defaultUoM);
                    token = token.Replace(SmartParametersV2016.space, SmartParametersV2016.bar.ToString());
                    string[] components = token.Split(SmartParametersV2016.bar);
                    // Work backwards ...
                    int component_count = components.Length - 1;
                    int times_through = 0;
                    while (component_count >= 0)
                    {
                        switch (times_through)
                        {
                            case 0:
                                // UoM
                                UNIT_OF_MEASURE = components[component_count].Trim();
                                break;
                            case 1:
                                // Total Units
                                // This Read
                                D_UNITS_USED_KWH = components[component_count].Trim();
                                if (!SmartParseV2016.Generic_Parse_Integer(D_UNITS_USED_KWH, utilityviewmodel))
                                {
                                    return false;
                                }
                                else
                                {
                                    temp = utilityviewmodel.genericTransactionValue;
                                }
                                D_UNITS_USED_KWH = temp.ToString();
                                break;
                            case 2:
                                // This Read
                                D_THIS_READ = components[component_count].Trim();
                                square_bracket = D_THIS_READ.IndexOf("[");
                                if (square_bracket >= 0)
                                {
                                    D_THIS_READ = D_THIS_READ.Substring(0, square_bracket);
                                }
                                if (!SmartParseV2016.Generic_Parse_Integer(D_THIS_READ, utilityviewmodel))
                                {
                                    return false;
                                }
                                else
                                {
                                    temp = utilityviewmodel.genericTransactionValue;
                                }
                                D_THIS_READ = temp.ToString();
                                break;
                            case 3:
                                // Last Read
                                D_LAST_READ = components[component_count].Trim();
                                square_bracket = D_LAST_READ.IndexOf("[");
                                if (square_bracket >= 0)
                                {
                                    D_LAST_READ = D_LAST_READ.Substring(0, square_bracket);
                                }
                                if (!SmartParseV2016.Generic_Parse_Integer(D_LAST_READ, utilityviewmodel))
                                {
                                    return false;
                                }
                                else
                                {
                                    temp = utilityviewmodel.genericTransactionValue;
                                }
                                D_LAST_READ = temp.ToString();
                                break;
                            default:
                                if (string.IsNullOrEmpty(UNITS_TYPE))
                                {
                                    UNITS_TYPE = components[component_count].Trim();
                                }
                                else
                                {
                                    UNITS_TYPE = components[component_count].Trim() + SmartParametersV2016.space + UNITS_TYPE;
                                }
                                break;
                        }
                        times_through++;
                        component_count--;
                    }

                    THIS_READ_TYPE = THIS_READ_TYPE + SmartParametersV2016.space + UNITS_TYPE;

                    // Sometiutilityviewmodel...even looking througout an ENTIRE BILL, you
                    // cannot find the term 'kWh' in there anywhere ...

                    // Do we have enough for a Read or Units_cost?
                    if (SmartParseV2016.Add_Readings_Test(SmartParametersV2016.defaultDate,
                                                            READINGS_PERIOD_START,
                                                            READINGS_PERIOD_END,
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
                                                                READINGS_PERIOD_START.ToString(),
                                                                READINGS_PERIOD_END.ToString(),
                                                                METER_SERIAL_NO,
                                                                THIS_READ_TYPE,
                                                                D_LAST_READ,
                                                                D_THIS_READ,
                                                                D_UNITS_USED_KWH,
                                                                N_LAST_READ,
                                                                N_THIS_READ,
                                                                N_UNITS_USED_KWH,
                                                                UNIT_OF_MEASURE);

                        if (READINGS_PERIOD_END != SmartParametersV2016.defaultDate)
                        {
                            temp_date = READINGS_PERIOD_END.AddMonths(1);
                            SmartParseV2016.Update_Bill(ourviewmodel, utilityviewmodel, "PAYMENT_DUE_DATE", temp_date.ToString(SmartParametersV2016.defaultCulture));
                            // Best we can do ..
                            SmartParseV2016.Update_Bill(ourviewmodel, utilityviewmodel, "DIRECT_DEBIT_DATE", temp_date.ToString(SmartParametersV2016.defaultCulture));
                            // Best we can do
                            SmartParseV2016.Update_Bill(ourviewmodel, utilityviewmodel, "DISCOUNT_CREDIT_DATE", temp_date.ToString(SmartParametersV2016.defaultCulture));
                        }
                    }
                }
            the_old_daysC1:
                continue;
            }
            sections[section_index][4] = "-1";      // Reset the switch
            return true;
        }

        private static int Bump(UtilityViewModel utilityviewmodel,
                                string[][] sections,
                                int section_index,
                                string[] lines,
                                string token,
                                string prfix,
                                ref int sub_line_count)
        {
            int currency_index = -1;
            //sub_line_count = temp_count;

            while (sub_line_count <= Convert.ToInt32(sections[section_index][5]))
            {
                token = lines[sub_line_count].Trim();
                if (!string.IsNullOrEmpty(token))
                {
                    currency_index = token.IndexOf(utilityviewmodel.bill_currency_symbol);
                    string last_char = token.Substring(token.Length - 1, 1);
                    if (currency_index >= 0 &&
                        last_char != ")")
                    {
                        prfix = prfix + SmartParametersV2016.space + token.Substring(0, currency_index).Trim();
                        token = token.Substring(currency_index);
                        break;
                    }
                    prfix = prfix + SmartParametersV2016.space + token;
                }
                sub_line_count++;
            }
            utilityviewmodel.temp_count = sub_line_count;
            utilityviewmodel.token = token;
            utilityviewmodel.prfix = prfix.Trim();
            return currency_index;
        }

        private static async Task<bool> Parse_SectionC2(MainViewModel ourviewmodel,
                                        UtilityViewModel utilityviewmodel,
                                        string[][] sections,
                                        string section,
                                        string sub_section,
                                        string[] lines)
        {
            // Not perfect but not a bad start            
            int section_index = 0;
            if (!SmartParseV2016.Decode_Section(section, sub_section, sections, utilityviewmodel.resource_code.ToString(), utilityviewmodel))
            {
                return true;
            }

            utilityviewmodel.token = "";
            bool status = false;

            for (int line_count = Convert.ToInt32(sections[section_index][4]); line_count <= Convert.ToInt32(sections[section_index][5]); line_count++)
            {
                utilityviewmodel.token = lines[line_count].Trim();

                if (SmartParseV2016.Token_Identify(utilityviewmodel, "Your tariff is" +
                                                                SmartParametersV2016.bar +
                                                                "Your pricing plan is", true))
                {
                    utilityviewmodel.token = utilityviewmodel.token.Replace("iplan", "").Trim();
                    utilityviewmodel.TARIFF_NAME = char.ToUpper(utilityviewmodel.token[0]) + utilityviewmodel.token.Substring(1);

                    if (!await SmartUtilityV2022.Lookup_Remote_Tariff_Code(ourviewmodel, utilityviewmodel))

                    {
                        goto quit_C2;
                    }
                    SmartParseV2016.Update_Bill(ourviewmodel, utilityviewmodel, "TARIFF_CODE", utilityviewmodel.TARIFF_CODE.ToString());

                    if (!SectionC2_Check_Big_One(ourviewmodel,
                                                utilityviewmodel,
                                                ref line_count,
                                                sections,
                                                section_index,
                                                lines))
                    {
                        goto quit_C2;
                    }
                    goto the_old_daysC2;
                }

                if (utilityviewmodel.PAYMENT_PLAN == SmartParametersV2016.defaultChar)
                {
                    if (Get_Payment_Plan_V2(ourviewmodel, utilityviewmodel, utilityviewmodel.token))
                    {
                        goto the_old_daysC2;
                    }
                }

            the_old_daysC2:
                continue;
            }
            status = true;
            sections[section_index][4] = "-1";      // Reset the switch
        quit_C2:
            return status;
        }

        private static bool SectionC2_Check_Big_One(MainViewModel ourviewmodel,
                                                            UtilityViewModel utilityviewmodel,
                                                            ref int line_count,
                                                            string[][] sections,
                                                            int section_index,
                                                            string[] lines)
        {
            string units = "units",
                        total_charges_before_VAT = "Total charges before VAT",
                        total_charges_this_period_including_VAT = "Total charges this period including VAT",
                        total_electricity_charges_this_period = "Total electricity charges this period",
                        VAT = "VAT";
            string TOTAL_CHARGES;
            char UNITS_TIME = SmartParametersV2016.daytimeUnit;
            utilityviewmodel.charges_item = 0;
            utilityviewmodel.units_band = 0;
            utilityviewmodel.UNIT_CHARGES_PERIOD_START = utilityviewmodel.BILL_PERIOD_START;
            utilityviewmodel.UNIT_CHARGES_PERIOD_END = utilityviewmodel.BILL_PERIOD_END;
            utilityviewmodel.STANDING_CHARGES_PERIOD_START = utilityviewmodel.UNIT_CHARGES_PERIOD_START;
            utilityviewmodel.STANDING_CHARGES_PERIOD_END = utilityviewmodel.UNIT_CHARGES_PERIOD_END;

            utilityviewmodel.sc_tracker = false;    // Maybe there aren't any Standing Charges?
            utilityviewmodel.SC = "";

            int sub_line_count = line_count + 1;
            while (sub_line_count < Convert.ToInt32(sections[section_index][5]))
            {
                utilityviewmodel.token = lines[sub_line_count].Trim();
                utilityviewmodel.token = SmartParseV2016.Remove_Double_Spaces_V3(utilityviewmodel.token);

                // Check the NEXT LINE to see if it has a dash "-" and contains two dates
                if (utilityviewmodel.token.IndexOf(" - ") >= 0)
                {
                    // We possibly have some period dates
                    utilityviewmodel.token = utilityviewmodel.token.Replace(" - ", SmartParametersV2016.bar.ToString());
                    if (!SectionC2_Check_Period_Dates(utilityviewmodel,
                                                    utilityviewmodel.token))
                    {
                        return false;
                    }

                    utilityviewmodel.units_band = 0;
                    utilityviewmodel.charges_item = 0;
                    utilityviewmodel.sc_tracker = false;
                    goto keep_looping;
                }

                if (SmartParseV2016.Token_Identify(utilityviewmodel, total_electricity_charges_this_period +
                                                                SmartParametersV2016.bar +
                                                                total_charges_this_period_including_VAT, true))
                {
                    if (!SectionC2_Check_Total_Charges(ourviewmodel,
                                                    utilityviewmodel,
                                                    utilityviewmodel.token))
                    {
                        return false;
                    }
                    break;
                }

                if (SmartParseV2016.Token_Identify(utilityviewmodel, VAT, true))
                {
                    if (!SectionC2_Check_VAT_Code(ourviewmodel,
                                                    utilityviewmodel,
                                                    sections,
                                                    section_index,
                                                    lines,
                                                    utilityviewmodel.token,
                                                    VAT,
                                                    ref sub_line_count))
                    {
                        return false;
                    }

                    int sub_sub_line_count = sub_line_count;
                    while (sub_sub_line_count < Convert.ToInt32(sections[section_index][5]))
                    {
                        utilityviewmodel.token = lines[sub_sub_line_count].Trim();
                        if (SmartParseV2016.Token_Identify(utilityviewmodel, total_electricity_charges_this_period +
                                                                        SmartParametersV2016.bar +
                                                                        total_charges_this_period_including_VAT, true))
                        {
                            if (utilityviewmodel.token.IndexOf(utilityviewmodel.bill_currency_symbol) >= 0)
                            {
                                if (!SmartParseV2016.Currency_Strip(utilityviewmodel.bill_currency_symbol, utilityviewmodel.bill_currency_separator, utilityviewmodel.bill_thousands_separator, utilityviewmodel.token, utilityviewmodel))
                                {
                                    return false;
                                }
                                else
                                {
                                    TOTAL_CHARGES = utilityviewmodel.value;
                                }
                                if (!SmartParseV2016.Generic_Parse_Integer(TOTAL_CHARGES, utilityviewmodel))
                                {
                                    return false;
                                }
                                SmartParseV2016.Update_Bill(ourviewmodel, utilityviewmodel, "RESOURCE_NEW_CHARGES", TOTAL_CHARGES);
                            }
                            goto stop_looping;
                        }
                        sub_sub_line_count++;
                    }
                    sub_line_count = sub_sub_line_count;
                    goto keep_looping;
                }

                if (SmartParseV2016.Token_Identify(utilityviewmodel, "Less your", true))
                {
                    // Here comes a discount
                    string DISCOUNT_TYPE = "";
                    int currency_index = Bump(utilityviewmodel,
                                                sections,
                                                section_index,
                                                lines,
                                                utilityviewmodel.token,
                                                DISCOUNT_TYPE,
                                                ref sub_line_count);
                    sub_line_count = utilityviewmodel.temp_count;
                    //utilityviewmodel.token = utilityviewmodel.token;
                    DISCOUNT_TYPE = utilityviewmodel.prfix;
                    if (!SectionC2_Check_Discounts(ourviewmodel,
                                                utilityviewmodel,
                                                currency_index,
                                                utilityviewmodel.token,
                                                Convert.ToDateTime(utilityviewmodel.UNIT_CHARGES_PERIOD_END),
                                                DISCOUNT_TYPE))
                    {
                        return false;
                    }
                    goto keep_looping;
                }

                if (SmartParseV2016.Token_Identify(utilityviewmodel, total_charges_before_VAT, true))
                {
                    goto keep_looping;
                }

                if (utilityviewmodel.sc_tracker)
                {
                    SectionC2_Check_Tracker(utilityviewmodel,
                                            utilityviewmodel.token);
                }

                if (utilityviewmodel.token.IndexOf(utilityviewmodel.bill_currency_symbol) >= 0)
                {
                    utilityviewmodel.token = utilityviewmodel.token.Replace("at", "");
                    utilityviewmodel.token = SmartParseV2016.Remove_Double_Spaces_V3(utilityviewmodel.token);
                    if (string.IsNullOrEmpty(utilityviewmodel.SC) ||
                        utilityviewmodel.token.IndexOf("days") == -1)
                    {
                        // We are doing a unit charge ....
                        utilityviewmodel.UNITS_TYPE = "";
                        utilityviewmodel.UNIT_OF_MEASURE = "";

                        // Split the line and work backwards
                        utilityviewmodel.token = utilityviewmodel.token.Replace(SmartParametersV2016.decimalPoint +
                                                SmartParametersV2016.defaultUoM,
                                                SmartParametersV2016.space + SmartParametersV2016.defaultUoM);
                        utilityviewmodel.token = utilityviewmodel.token.Replace(SmartParametersV2016.decimalPoint +
                                                SmartParametersV2016.space +
                                                units,
                                                SmartParametersV2016.space + SmartParametersV2016.defaultUoM);
                        utilityviewmodel.token = utilityviewmodel.token.Replace(SmartParametersV2016.space, SmartParametersV2016.bar.ToString());

                        if (!SectionC2_Check_Units(utilityviewmodel,
                                                    utilityviewmodel.token,
                                                    UNITS_TIME))
                        {
                            return false;
                        }
                    }
                    else
                    {
                        // We are doing a standing charge
                        utilityviewmodel.sc_tracker = false;

                        string CHARGES_TYPE = utilityviewmodel.SC;
                        utilityviewmodel.SC = "";

                        // Split the line and work backwards
                        utilityviewmodel.token = utilityviewmodel.token.Replace(SmartParametersV2016.decimalPoint + SmartParametersV2016.space + "days",
                                               SmartParametersV2016.space + "days");
                        utilityviewmodel.token = utilityviewmodel.token.Replace(SmartParametersV2016.decimalPoint + "days",
                                               SmartParametersV2016.space + "days");
                        utilityviewmodel.token = utilityviewmodel.token.Replace(SmartParametersV2016.space, SmartParametersV2016.bar.ToString());

                        if (!SectionC2_Check_Charges(utilityviewmodel,
                                            utilityviewmodel.token,
                                             Convert.ToDateTime(utilityviewmodel.STANDING_CHARGES_PERIOD_START),
                                             Convert.ToDateTime(utilityviewmodel.STANDING_CHARGES_PERIOD_END),
                                             CHARGES_TYPE))
                        {
                            return false;
                        }
                    }
                }
            keep_looping:
                sub_line_count++;
            }
        stop_looping:
            line_count = sub_line_count;
            return true;
        }

        private static bool SectionC2_Check_Period_Dates(UtilityViewModel utilityviewmodel,
                                                            string token)
        {
            string[] components = token.Split(SmartParametersV2016.bar);
            if (components.Length == 2)
            {
                string TEMP_DATE = components[0].Trim();
                if (!SmartParseV2016.Generic_Parse_Datetime(TEMP_DATE, utilityviewmodel))
                {
                    return false;
                }
                utilityviewmodel.UNIT_CHARGES_PERIOD_START = TEMP_DATE;
                utilityviewmodel.STANDING_CHARGES_PERIOD_START = utilityviewmodel.UNIT_CHARGES_PERIOD_START;
                TEMP_DATE = components[1].Trim();
                if (!SmartParseV2016.Generic_Parse_Datetime(TEMP_DATE, utilityviewmodel))
                {
                    return false;
                }
                utilityviewmodel.UNIT_CHARGES_PERIOD_END = TEMP_DATE;
                utilityviewmodel.STANDING_CHARGES_PERIOD_END = utilityviewmodel.UNIT_CHARGES_PERIOD_END;
            }
            return true;
        }

        private static bool SectionC2_Check_Total_Charges(MainViewModel ourviewmodel,
                                                            UtilityViewModel utilityviewmodel,
                                                            string token)
        {
            if (token.IndexOf(utilityviewmodel.bill_currency_symbol) >= 0)
            {
                string TOTAL_CHARGES;// = "0";
                //int temp = 0;
                if (!SmartParseV2016.Currency_Strip(utilityviewmodel.bill_currency_symbol, utilityviewmodel.bill_currency_separator, utilityviewmodel.bill_thousands_separator, token, utilityviewmodel))
                {
                    return false;
                }
                else
                {
                    TOTAL_CHARGES = utilityviewmodel.value;
                }
                if (!SmartParseV2016.Generic_Parse_Integer(TOTAL_CHARGES, utilityviewmodel))
                {
                    return false;
                }
                //else
                //{
                //    temp = utilityviewmodel.genericTransactionValue;
                //}
                SmartParseV2016.Update_Bill(ourviewmodel, utilityviewmodel, "RESOURCE_NEW_CHARGES", TOTAL_CHARGES);
            }
            return true;
        }

        private static bool SectionC2_Check_VAT_Code(MainViewModel ourviewmodel,
                                                            UtilityViewModel utilityviewmodel,
                                                            string[][] sections,
                                                            int section_index,
                                                            string[] lines,
                                                            string token,
                                                            string VAT,
                                                            ref int sub_line_count)
        {
            short VAT_CODE = SmartParametersV2016.zeroRateVatCode;
            if (!SmartParseV2016.Determine_Vat_Code(ourviewmodel, utilityviewmodel, token, VAT))
            {
                return false;
            }
            SmartParseV2016.Update_Bill(ourviewmodel, utilityviewmodel, "RESOURCE_VAT_CODE", VAT_CODE.ToString());

            utilityviewmodel.prfix = "";
            int currency_index = Bump(utilityviewmodel,
                                        sections,
                                        section_index,
                                        lines,
                                        token,
                                        utilityviewmodel.prfix,
                                        ref sub_line_count);
            sub_line_count = utilityviewmodel.temp_count;
            token = utilityviewmodel.token;
            //utilityviewmodel.prfix = utilityviewmodel.prfix;

            if (currency_index >= 0)
            {
                string VAT_AMOUNT;// = "0";
                if (!SmartParseV2016.Currency_Strip(utilityviewmodel.bill_currency_symbol, utilityviewmodel.bill_currency_separator, utilityviewmodel.bill_thousands_separator, token, utilityviewmodel))
                {
                    return false;
                }
                VAT_AMOUNT = utilityviewmodel.value;
                if (!SmartParseV2016.Generic_Parse_Integer(VAT_AMOUNT, utilityviewmodel))
                {
                    return false;
                }
                SmartParseV2016.Update_Bill(ourviewmodel, utilityviewmodel, "RESOURCE_VAT_AMOUNT", VAT_AMOUNT);
            }
            return true;
        }

        private static bool SectionC2_Check_Discounts(MainViewModel ourviewmodel,
                                                        UtilityViewModel utilityviewmodel,
                                                        int currency_index,
                                                        string token,
                                                        DateTime UNIT_CHARGES_PERIOD_END,
                                                        string DISCOUNT_TYPE)
        {
            if (currency_index >= 0)
            {
                short discount_item = 0;
                string DISCOUNT_AMOUNT,// = "0",
                        DISCOUNT_CREDIT_DATE;// = "";

                short DISCOUNT_VAT_CODE = SmartParametersV2016.zeroRateVatCode;
                // Get some kind of discount date
                DateTime DISCOUNT_DATE = UNIT_CHARGES_PERIOD_END;

                if (!SmartParseV2016.Currency_Strip(utilityviewmodel.bill_currency_symbol, utilityviewmodel.bill_currency_separator, utilityviewmodel.bill_thousands_separator, token, utilityviewmodel))
                {
                    return false;
                }
                else
                {
                    DISCOUNT_AMOUNT = utilityviewmodel.value;
                }
                //int temp = 0;
                if (!SmartParseV2016.Generic_Parse_Integer(DISCOUNT_AMOUNT, utilityviewmodel))
                {
                    return false;
                }
                //else
                //{
                //    temp = utilityviewmodel.genericTransactionValue;
                //}
                DISCOUNT_CREDIT_DATE = utilityviewmodel.BILL_DATE.Substring(2);
                discount_item = (short)(discount_item + 1);
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
                    SmartParseV2016.Update_Bill(ourviewmodel, utilityviewmodel, "DISCOUNTS", DISCOUNT_AMOUNT);
                    SmartParseV2016.Update_Bill(ourviewmodel, utilityviewmodel, "DISCOUNT_CREDIT_DATE", DISCOUNT_CREDIT_DATE);
                }
            }
            return true;
        }
        private static void SectionC2_Check_Tracker(UtilityViewModel utilityviewmodel,
                                                    string token)
        {
            if (token.Substring(0, 1) != "(")
            {
                if (string.IsNullOrEmpty(utilityviewmodel.SC))
                {
                    utilityviewmodel.SC = token;
                    utilityviewmodel.SC = utilityviewmodel.SC.Replace(". days", " days").Trim();
                    utilityviewmodel.SC = utilityviewmodel.SC.Replace(".p", "p").Trim();
                }
                else
                {
                    int currency_index = token.IndexOf(utilityviewmodel.bill_currency_symbol);
                    if (currency_index >= 0)
                    {
                        utilityviewmodel.SC = utilityviewmodel.SC + SmartParametersV2016.space +
                            token.Substring(0, currency_index).Replace(". days", ".days").Replace(".p", "p").Replace(".days", " days").Replace(".00", "").Trim();
                    }
                    else
                    {
                        utilityviewmodel.SC = utilityviewmodel.SC + SmartParametersV2016.space + token;
                    }
                }
            }
            return;
        }
        private static bool SectionC2_Check_Units(UtilityViewModel utilityviewmodel,
                                                    string token,
                                                    char UNITS_TIME)
        {
            string UNITS_COST = "0",
                    UNITS_RATE = "0",
                    UNITS = "0";

            string[] components = token.Split(SmartParametersV2016.bar);
            // Work backwards ...
            int component_count = components.Length - 1;
            int times_through = 0;
            while (component_count >= 0)
            {
                switch (times_through)
                {
                    case 0:
                        // Units Cost
                        if (!SmartParseV2016.Currency_Strip(utilityviewmodel.bill_currency_symbol, utilityviewmodel.bill_currency_separator, utilityviewmodel.bill_thousands_separator, components[component_count], utilityviewmodel))
                        {
                            return false;
                        }
                        UNITS_COST = utilityviewmodel.value;
                        if (!SmartParseV2016.Generic_Parse_Integer(UNITS_COST, utilityviewmodel))
                        {
                            return false;
                        }
                        break;
                    case 1:
                        // Units Rate
                        components[component_count] = components[component_count].Replace(SmartParametersV2016.decimalPoint + utilityviewmodel.bill_denomination_symbol, utilityviewmodel.bill_denomination_symbol.ToString());
                        UNITS_RATE = components[component_count].Replace(utilityviewmodel.bill_denomination_symbol.ToString(), "").Trim();
                        if (!SmartParseV2016.Generic_Parse_Decimal(UNITS_RATE, utilityviewmodel))
                        {
                            return false;
                        }
                        break;
                    case 2:
                        // Units
                        utilityviewmodel.UNIT_OF_MEASURE = components[component_count].Trim();
                        break;
                    case 3:
                        // Units
                        UNITS = components[component_count].Trim();
                        // Sometimes Gas values have these embedded
                        //UNITS = UNITS.Replace("comma", "");
                        if (!SmartParseV2016.Generic_Parse_Integer(UNITS, utilityviewmodel))
                        {
                            return false; // goto quit_C2;
                        }
                        break;
                    default:
                        if (string.IsNullOrEmpty(utilityviewmodel.UNITS_TYPE))
                        {
                            utilityviewmodel.UNITS_TYPE = components[component_count].Trim();
                        }
                        else
                        {
                            utilityviewmodel.UNITS_TYPE = components[component_count].Trim() + SmartParametersV2016.space + utilityviewmodel.UNITS_TYPE;
                        }
                        break;
                }
                times_through++;
                component_count--;
            }

            // first or next 
            utilityviewmodel.units_band = (short)(utilityviewmodel.units_band + 1);

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
                                                                utilityviewmodel.UNIT_CHARGES_PERIOD_START,
                                                                utilityviewmodel.UNIT_CHARGES_PERIOD_END,
                                                                UNITS_TIME,
                                                                utilityviewmodel.units_band.ToString(),
                                                                utilityviewmodel.UNITS_TYPE,
                                                                UNITS,
                                                                UNITS_RATE,
                                                                utilityviewmodel.UNIT_OF_MEASURE,
                                                                UNITS_COST);
                    utilityviewmodel.sc_tracker = true;
                    utilityviewmodel.SC = "";
                    break;
                default:
                    break;
            }
            return true;
        }

        private static bool SectionC2_Check_Charges(UtilityViewModel utilityviewmodel,
                                                    string token,
                                                     DateTime STANDING_CHARGES_PERIOD_START,
                                                     DateTime STANDING_CHARGES_PERIOD_END,
                                                     string CHARGES_TYPE)
        {
            string CHARGES_COST = "0",
                STANDING_CHARGE = "0";
            short CHARGES_DAYS = 0;
            //int temp = 0;
            string[] components = token.Split(SmartParametersV2016.bar);
            // Work backwards ...
            int component_count = components.Length - 1;
            int times_through = 0;
            while (component_count >= 0)
            {
                switch (times_through)
                {
                    case 0:
                        // Charges Cost
                        if (!SmartParseV2016.Currency_Strip(utilityviewmodel.bill_currency_symbol, utilityviewmodel.bill_currency_separator, utilityviewmodel.bill_thousands_separator, components[component_count], utilityviewmodel))
                        {
                            return false;
                        }
                        CHARGES_COST = utilityviewmodel.value;
                        if (!SmartParseV2016.Generic_Parse_Integer(CHARGES_COST, utilityviewmodel))
                        {
                            return false;
                        }
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
                        break;
                    case 2:
                        // Unit of Measure - Not used for Standing Charge
                        //UNIT_OF_MEASURE = components[component_count].Trim();
                        break;
                    case 3:
                        // Days
                        string charges_days = components[component_count].Trim();
                        charges_days = charges_days.Replace(".00", "");   // No of days is always a whole number is it not?
                                                                          // Sometimes ... days has a decimal point e.g. 99.00
                                                                          // Sometimes Gas values have these embedded
                                                                          //UNITS = UNITS.Replace("comma", "");
                        if (!SmartParseV2016.Generic_Parse_Integer(charges_days, utilityviewmodel))
                        {
                            return false; // goto quit_C2;
                        }
                        CHARGES_DAYS = Convert.ToInt16(utilityviewmodel.genericTransactionValue);
                        break;
                    default:
                        break;
                }
                times_through++;
                component_count--;
            }

            // first or next 
            utilityviewmodel.charges_item = (short)(utilityviewmodel.charges_item + 1);

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
                                                            STANDING_CHARGES_PERIOD_START.ToString(),
                                                            STANDING_CHARGES_PERIOD_END.ToString(),
                                                            utilityviewmodel.charges_item.ToString(),
                                                            CHARGES_TYPE,
                                                            STANDING_CHARGE,
                                                            CHARGES_DAYS.ToString(),
                                                            CHARGES_COST);
                    break;
                default:
                    break;
            }
            return true;
        }
        private static bool Parse_SectionD1(MainViewModel ourviewmodel,
                                            UtilityViewModel utilityviewmodel,
                                            string[][] sections,
                                            string section,
                                            string sub_section,
                                            string[] lines)
        {
            // Not perfect but not a bad start
            int section_index = 0;
            if (!SmartParseV2016.Decode_Section(section, sub_section, sections, utilityviewmodel.resource_code.ToString(), utilityviewmodel))
            {
                return true;
            }

            string token;

            DateTime READINGS_PERIOD_START = SmartParametersV2016.defaultDate,
                    READINGS_PERIOD_END = SmartParametersV2016.defaultDate;

            string METER_SERIAL_NO = "",
                    THIS_READ_TYPE = "",
                    UNIT_OF_MEASURE = "";
            string your_gas_use_this_period = "Your gas use this period",
                    your_amended_gas_use_this_period = "Your amended gas use this period",
                    the_gas_youve_used = "The gas you've used",
                    meter_number_colon = "Meter:",
                    units = "units";
            decimal temp_decimal;

            string D_THIS_READ = "0",
                    D_LAST_READ = "0",
                    D_UNITS_USED_M3 = "0";

            string UNITS_TYPE;

            for (int line_count = Convert.ToInt32(sections[section_index][4]); line_count <= Convert.ToInt32(sections[section_index][5]); line_count++)
            {
                token = lines[line_count].Trim();

                if (SmartParseV2016.Token_Identify(utilityviewmodel, your_gas_use_this_period, true) ||
                    SmartParseV2016.Token_Identify(utilityviewmodel, your_amended_gas_use_this_period, true) ||
                    SmartParseV2016.Token_Identify(utilityviewmodel, the_gas_youve_used, true))
                {
                    token = token.Replace(SmartParametersV2016.dash, "").Trim();
                    if (string.IsNullOrEmpty(token))
                    {
                        line_count++;
                        token = lines[line_count].Trim();
                    }
                    if (token.Length > 1)
                    {
                        THIS_READ_TYPE = char.ToUpper(token[0]) + token.Substring(1);
                    }
                    READINGS_PERIOD_START = SmartTimeV2016.ConvertDateTime(utilityviewmodel.BILL_PERIOD_START);
                    READINGS_PERIOD_END = SmartTimeV2016.ConvertDateTime(utilityviewmodel.BILL_PERIOD_END);
                    goto the_old_daysD1;
                }

                if (SmartParseV2016.Token_Identify(utilityviewmodel, meter_number_colon, true))
                {
                    METER_SERIAL_NO = token.Trim();
                    goto the_old_daysD1;
                }

                if (SmartParseV2016.Token_Identify_End(utilityviewmodel, SmartParametersV2016.space + units, false))
                {
                    D_THIS_READ = D_LAST_READ = D_UNITS_USED_M3 = "0";

                    UNITS_TYPE = "";
                    int square_bracket;

                    token = SmartParseV2016.Remove_Double_Spaces_V3(token);
                    token = token.Replace(SmartParametersV2016.decimalPoint +
                                            SmartParametersV2016.space +
                                            units, SmartParametersV2016.space + units);
                    token = token.Replace(SmartParametersV2016.space, SmartParametersV2016.bar.ToString());
                    string[] components = token.Split(SmartParametersV2016.bar);
                    // Work backwards ...
                    int component_count = components.Length - 1;
                    int times_through = 0;
                    while (component_count >= 0)
                    {
                        switch (times_through)
                        {
                            case 0:
                                // Units
                                break;
                            case 1:
                                // Total M3
                                D_UNITS_USED_M3 = components[component_count].Trim();
                                D_UNITS_USED_M3 = D_UNITS_USED_M3.TrimEnd(Convert.ToChar(SmartParametersV2016.decimalPoint));
                                // Don't forget this is decimal
                                if (!SmartParseV2016.Generic_Parse_Decimal(D_UNITS_USED_M3, utilityviewmodel))
                                {
                                    return false;
                                }
                                else
                                {
                                    temp_decimal = utilityviewmodel.genericDecimalValue;
                                }
                                D_UNITS_USED_M3 = temp_decimal.ToString();
                                break;
                            case 2:
                                // This Read
                                D_THIS_READ = components[component_count].Trim();
                                square_bracket = D_THIS_READ.IndexOf("[");
                                if (square_bracket >= 0)
                                {
                                    D_THIS_READ = D_THIS_READ.Substring(0, square_bracket);
                                }
                                if (!SmartParseV2016.Generic_Parse_Decimal(D_THIS_READ, utilityviewmodel))
                                {
                                    return false;
                                }
                                else
                                {
                                    temp_decimal = utilityviewmodel.genericDecimalValue;
                                }
                                D_THIS_READ = temp_decimal.ToString();
                                break;
                            case 3:
                                // Last Read
                                D_LAST_READ = components[component_count].Trim();
                                square_bracket = D_LAST_READ.IndexOf("[");
                                if (square_bracket >= 0)
                                {
                                    D_LAST_READ = D_LAST_READ.Substring(0, square_bracket);
                                }
                                if (!SmartParseV2016.Generic_Parse_Decimal(D_LAST_READ, utilityviewmodel))
                                {
                                    return false;
                                }
                                else
                                {
                                    temp_decimal = utilityviewmodel.genericDecimalValue;
                                }
                                D_LAST_READ = temp_decimal.ToString();
                                break;
                            default:
                                if (string.IsNullOrEmpty(UNITS_TYPE))
                                {
                                    UNITS_TYPE = components[component_count].Trim();
                                }
                                else
                                {
                                    UNITS_TYPE = components[component_count].Trim() + SmartParametersV2016.space + UNITS_TYPE;
                                }
                                break;
                        }
                        times_through++;
                        component_count--;
                    }

                    THIS_READ_TYPE = THIS_READ_TYPE + SmartParametersV2016.space + UNITS_TYPE;
                }

                if (SmartParseV2016.Token_Identify_End(utilityviewmodel, SmartParametersV2016.defaultUoM, false))
                {
                    utilityviewmodel.D_UNITS_USED_KWH = "0";

                    token = SmartParseV2016.Remove_Double_Spaces_V3(token);
                    token = token.Replace(SmartParametersV2016.space, SmartParametersV2016.bar.ToString());
                    if (!Units_Used(utilityviewmodel,
                                    token))
                    {
                        return false;
                    }


                    // Do we have enough for a Read or Units_cost?
                    if (SmartParseV2016.Add_Readings_Test(SmartParametersV2016.defaultDate,
                                                            READINGS_PERIOD_START,
                                                            READINGS_PERIOD_END,
                                                            D_LAST_READ,
                                                            D_THIS_READ,
                                                            UNIT_OF_MEASURE))
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
                                                                READINGS_PERIOD_START.ToString(),
                                                                READINGS_PERIOD_END.ToString(),
                                                                METER_SERIAL_NO,
                                                                THIS_READ_TYPE,
                                                                D_LAST_READ,
                                                                D_THIS_READ,
                                                                D_UNITS_USED_M3,
                                                                UNIT_OF_MEASURE,
                                                                utilityviewmodel.D_UNITS_USED_KWH,
                                                                utilityviewmodel.CALORIFIC_VALUE);
                        if (READINGS_PERIOD_END != SmartParametersV2016.defaultDate)
                        {
                            DateTime temp_date = READINGS_PERIOD_END.AddMonths(1);
                            SmartParseV2016.Update_Bill(ourviewmodel, utilityviewmodel, "PAYMENT_DUE_DATE", temp_date.ToString(SmartParametersV2016.defaultCulture));
                            // Best we can do ..
                            SmartParseV2016.Update_Bill(ourviewmodel, utilityviewmodel, "DIRECT_DEBIT_DATE", temp_date.ToString(SmartParametersV2016.defaultCulture));
                            // Best we can do
                            SmartParseV2016.Update_Bill(ourviewmodel, utilityviewmodel, "DISCOUNT_CREDIT_DATE", temp_date.ToString(SmartParametersV2016.defaultCulture));
                        }
                    }
                }
            the_old_daysD1:
                continue;
            }
            sections[section_index][4] = "-1";      // Reset the switch
            return true;
        }

        private static bool Units_Used(UtilityViewModel utilityviewmodel,
                                        string token)
        {
            string[] components = token.Split(SmartParametersV2016.bar);
            // Work backwards ...
            int component_count = components.Length - 1;
            int times_through = 0;
            while (component_count >= 0)
            {
                switch (times_through)
                {
                    case 0:
                        // Unit of Measure
                        // This Read
                        utilityviewmodel.UNIT_OF_MEASURE = components[component_count].Trim();
                        break;
                    case 1:
                        // Converted Units
                        // Total kWh
                        utilityviewmodel.D_UNITS_USED_KWH = components[component_count].Trim();
                        // Don't forget this is decimal
                        if (!SmartParseV2016.Generic_Parse_Decimal(utilityviewmodel.D_UNITS_USED_KWH, utilityviewmodel))
                        {
                            return false;
                        }
                        break;
                    default:
                        break;
                }
                times_through++;
                component_count--;
            }
            return true;
        }
        private static async Task<bool> Parse_SectionD2(MainViewModel ourviewmodel,
                                         UtilityViewModel utilityviewmodel,
                                         string[][] sections,
                                        string section,
                                        string sub_section,
                                        string[] lines)
        {
            // Not perfect but not a bad start            
            int section_index = 0;
            if (!SmartParseV2016.Decode_Section(section, sub_section, sections, utilityviewmodel.resource_code.ToString(), utilityviewmodel))
            {
                return true;
            }

            utilityviewmodel.token = "";
            bool status = false;

            for (int line_count = Convert.ToInt32(sections[section_index][4]); line_count <= Convert.ToInt32(sections[section_index][5]); line_count++)
            {
                utilityviewmodel.token = lines[line_count].Trim();

                if (SmartParseV2016.Token_Identify(utilityviewmodel, "Your tariff is" +
                                                                SmartParametersV2016.bar +
                                                                "Your pricing plan is", true))
                {
                    utilityviewmodel.token = utilityviewmodel.token.Replace("iplan", "").Trim();
                    utilityviewmodel.TARIFF_NAME = char.ToUpper(utilityviewmodel.token[0]) + utilityviewmodel.token.Substring(1);

                    if (!await SmartUtilityV2022.Lookup_Remote_Tariff_Code(ourviewmodel, utilityviewmodel))

                    {
                        goto quit_D2;
                    }
                    SmartParseV2016.Update_Bill(ourviewmodel, utilityviewmodel, "TARIFF_CODE", utilityviewmodel.TARIFF_CODE.ToString());

                    if (!SectionD2_Check_Big_One(ourviewmodel,
                                                utilityviewmodel,
                                                ref line_count,
                                                sections,
                                                section_index,
                                                lines))
                    {
                        goto quit_D2;
                    }
                    goto the_old_daysD2;
                }

                if (utilityviewmodel.PAYMENT_PLAN == SmartParametersV2016.defaultChar)
                {
                    if (Get_Payment_Plan_V2(ourviewmodel, utilityviewmodel, utilityviewmodel.token))
                    {
                        goto the_old_daysD2;
                    }
                }

            the_old_daysD2:
                continue;
            }
            status = true;
            sections[section_index][4] = "-1";      // Reset the switch
        quit_D2:
            return status;
        }

        private static bool SectionD2_Check_Big_One(MainViewModel ourviewmodel,
                                                    UtilityViewModel utilityviewmodel,
                                                    ref int line_count,
                                                    string[][] sections,
                                                    int section_index,
                                                    string[] lines)
        {
            string units = "units",
                        total_charges_before_VAT = "Total charges before VAT",
                        total_charges_this_period_including_VAT = "Total charges this period including VAT",
                        total_gas_charges_this_period = "Total gas charges this period",
                        VAT = "VAT";
            utilityviewmodel.charges_item = 0;
            utilityviewmodel.units_band = 0;
            utilityviewmodel.UNIT_CHARGES_PERIOD_START = utilityviewmodel.BILL_PERIOD_START;
            utilityviewmodel.UNIT_CHARGES_PERIOD_END = utilityviewmodel.BILL_PERIOD_END;
            utilityviewmodel.STANDING_CHARGES_PERIOD_START = utilityviewmodel.UNIT_CHARGES_PERIOD_START;
            utilityviewmodel.STANDING_CHARGES_PERIOD_END = utilityviewmodel.UNIT_CHARGES_PERIOD_END;

            utilityviewmodel.sc_tracker = false;    // Maybe there aren't any Standing Charges?
            utilityviewmodel.SC = "";

            int sub_line_count = line_count + 1;
            while (sub_line_count < Convert.ToInt32(sections[section_index][5]))
            {
                utilityviewmodel.token = lines[sub_line_count].Trim();
                utilityviewmodel.token = SmartParseV2016.Remove_Double_Spaces_V3(utilityviewmodel.token);

                // Check the NEXT LINE to see if it has a dash "-" and contains two dates
                if (utilityviewmodel.token.IndexOf(" - ") >= 0)
                {
                    // We possibly have some period dates
                    utilityviewmodel.token = utilityviewmodel.token.Replace(" - ", SmartParametersV2016.bar.ToString());
                    if (!SectionD2_Check_Period_Dates(utilityviewmodel,
                                                utilityviewmodel.token))
                    {
                        return false;
                    }

                    utilityviewmodel.units_band = 0;
                    utilityviewmodel.charges_item = 0;
                    utilityviewmodel.sc_tracker = false;
                    goto keep_looping;
                }

                if (SmartParseV2016.Token_Identify(utilityviewmodel, total_gas_charges_this_period +
                                                                SmartParametersV2016.bar +
                                                                total_charges_this_period_including_VAT, true))
                {
                    if (!SectionD2_Check_Total(ourviewmodel,
                                                utilityviewmodel,
                                                utilityviewmodel.token))
                    {
                        return false;
                    }

                    break;
                }

                if (SmartParseV2016.Token_Identify(utilityviewmodel, VAT, true))
                {
                    if (!SectionD2_Check_VAT(ourviewmodel,
                                        utilityviewmodel,
                                        sections,
                                        section_index,
                                        lines,
                                        ref sub_line_count))
                    {
                        return false;
                    }

                    int sub_sub_line_count = sub_line_count;
                    while (sub_sub_line_count < Convert.ToInt32(sections[section_index][5]))
                    {
                        utilityviewmodel.token = lines[sub_sub_line_count].Trim();
                        if (SmartParseV2016.Token_Identify(utilityviewmodel, total_gas_charges_this_period +
                                                                        SmartParametersV2016.bar +
                                                                        total_charges_this_period_including_VAT, true))
                        {
                            if (!SectionD2_Check_Total_Charges(ourviewmodel,
                                                        utilityviewmodel,
                                                        utilityviewmodel.token))
                            {
                                return false;
                            }

                            goto stop_looping;
                        }
                        sub_sub_line_count++;
                    }
                    sub_line_count = sub_sub_line_count;
                    goto keep_looping;
                }

                if (SmartParseV2016.Token_Identify(utilityviewmodel, "Less your", true))
                {
                    // Here comes a discount
                    string DISCOUNT_TYPE = "";
                    int currency_index = Bump(utilityviewmodel,
                                                sections,
                                                section_index,
                                                lines,
                                                utilityviewmodel.token,
                                                DISCOUNT_TYPE,
                                                ref sub_line_count);
                    sub_line_count = utilityviewmodel.temp_count;
                    DISCOUNT_TYPE = utilityviewmodel.prfix;
                    if (!SectionD2_Check_Discounts(ourviewmodel,
                                                utilityviewmodel,
                                                utilityviewmodel.token,
                                                currency_index,
                                                Convert.ToDateTime(utilityviewmodel.UNIT_CHARGES_PERIOD_END),
                                                DISCOUNT_TYPE))
                    {
                        return false;
                    }
                    goto keep_looping;
                }

                if (SmartParseV2016.Token_Identify(utilityviewmodel, total_charges_before_VAT, true))
                {
                    goto keep_looping;
                }

                if (utilityviewmodel.sc_tracker)
                {
                    SectionD2_Check_Tracker(utilityviewmodel,
                                            utilityviewmodel.token);
                }

                if (utilityviewmodel.token.IndexOf(utilityviewmodel.bill_currency_symbol) >= 0)
                {
                    utilityviewmodel.token = utilityviewmodel.token.Replace("at", "");
                    utilityviewmodel.token = SmartParseV2016.Remove_Double_Spaces_V3(utilityviewmodel.token);

                    if (string.IsNullOrEmpty(utilityviewmodel.SC) ||
                        utilityviewmodel.token.IndexOf("days") == -1)
                    {
                        // We are doing a unit charge ....
                        utilityviewmodel.UNITS_TYPE = "";
                        utilityviewmodel.UNIT_OF_MEASURE = "";

                        // Split the line and work backwards
                        utilityviewmodel.token = utilityviewmodel.token.Replace(SmartParametersV2016.decimalPoint +
                                                SmartParametersV2016.space +
                                                SmartParametersV2016.defaultUoM,
                                                SmartParametersV2016.space + SmartParametersV2016.defaultUoM);
                        utilityviewmodel.token = utilityviewmodel.token.Replace(SmartParametersV2016.decimalPoint +
                                                SmartParametersV2016.space +
                                                units,
                                                SmartParametersV2016.space + units);
                        utilityviewmodel.token = utilityviewmodel.token.Replace(SmartParametersV2016.space, SmartParametersV2016.bar.ToString());
                        if (!Section_D2_Units(utilityviewmodel,
                                                utilityviewmodel.token))
                        {
                            return false;
                        }
                    }
                    else
                    {
                        // We are doing a standing charge
                        utilityviewmodel.sc_tracker = false;

                        utilityviewmodel.CHARGES_TYPE = utilityviewmodel.SC;
                        utilityviewmodel.SC = "";

                        // Split the line and work backwards
                        utilityviewmodel.token = utilityviewmodel.token.Replace(SmartParametersV2016.decimalPoint + SmartParametersV2016.space + "days",
                                                SmartParametersV2016.space + "days");
                        utilityviewmodel.token = utilityviewmodel.token.Replace(SmartParametersV2016.decimalPoint + "days",
                                                SmartParametersV2016.space + "days");
                        utilityviewmodel.token = utilityviewmodel.token.Replace(SmartParametersV2016.space, SmartParametersV2016.bar.ToString());

                        if (!SectionD2_Check_Charges(utilityviewmodel,
                                                    utilityviewmodel.token))
                        {
                            return false;
                        }
                    }
                }
            keep_looping:
                sub_line_count++;
            }
        stop_looping:
            line_count = sub_line_count;
            return true;
        }

        private static bool SectionD2_Check_Total_Charges(MainViewModel ourviewmodel,
                                                    UtilityViewModel utilityviewmodel,
                                                    string token)
        {
            if (token.IndexOf(utilityviewmodel.bill_currency_symbol) >= 0)
            {
                string TOTAL_CHARGES;
                if (!SmartParseV2016.Currency_Strip(utilityviewmodel.bill_currency_symbol, utilityviewmodel.bill_currency_separator, utilityviewmodel.bill_thousands_separator, token, utilityviewmodel))
                {
                    return false;
                }
                else
                {
                    TOTAL_CHARGES = utilityviewmodel.value;
                }
                if (!SmartParseV2016.Generic_Parse_Integer(TOTAL_CHARGES, utilityviewmodel))
                {
                    return false;
                }
                SmartParseV2016.Update_Bill(ourviewmodel, utilityviewmodel, "RESOURCE_NEW_CHARGES", TOTAL_CHARGES);
            }
            return true;
        }

        private static bool SectionD2_Check_VAT(MainViewModel ourviewmodel,
                                                UtilityViewModel utilityviewmodel,
                                                string[][] sections,
                                                int section_index,
                                                string[] lines,
                                                ref int sub_line_count)
        {
            string VAT = "VAT";
            short VAT_CODE = SmartParametersV2016.zeroRateVatCode;
            if (!SmartParseV2016.Determine_Vat_Code(ourviewmodel, utilityviewmodel, utilityviewmodel.token, VAT))
            {
                return false;
            }
            SmartParseV2016.Update_Bill(ourviewmodel, utilityviewmodel, "RESOURCE_VAT_CODE", VAT_CODE.ToString());

            utilityviewmodel.prfix = "";
            int currency_index = Bump(utilityviewmodel,
                                       sections,
                                        section_index,
                                        lines,
                                        utilityviewmodel.token,
                                        utilityviewmodel.prfix,
                                        ref sub_line_count);
            sub_line_count = utilityviewmodel.temp_count;
            //utilityviewmodel.token = utilityviewmodel.token;
            //utilityviewmodel.prfix = utilityviewmodel.prfix;
            if (currency_index >= 0)
            {
                string VAT_AMOUNT;
                if (!SmartParseV2016.Currency_Strip(utilityviewmodel.bill_currency_symbol, utilityviewmodel.bill_currency_separator, utilityviewmodel.bill_thousands_separator, utilityviewmodel.token, utilityviewmodel))
                {
                    return false;
                }
                VAT_AMOUNT = utilityviewmodel.value;
                if (!SmartParseV2016.Generic_Parse_Integer(VAT_AMOUNT, utilityviewmodel))
                {
                    return false;
                }
                SmartParseV2016.Update_Bill(ourviewmodel, utilityviewmodel, "RESOURCE_VAT_AMOUNT", VAT_AMOUNT);
            }
            return true;
        }

        private static bool SectionD2_Check_Total(MainViewModel ourviewmodel,
                                                    UtilityViewModel utilityviewmodel,
                                                    string token)
        {
            if (token.IndexOf(utilityviewmodel.bill_currency_symbol) >= 0)
            {
                string TOTAL_CHARGES;
                if (!SmartParseV2016.Currency_Strip(utilityviewmodel.bill_currency_symbol, utilityviewmodel.bill_currency_separator, utilityviewmodel.bill_thousands_separator, token, utilityviewmodel))
                {
                    return false;
                }
                else
                {
                    TOTAL_CHARGES = utilityviewmodel.value;
                }
                if (!SmartParseV2016.Generic_Parse_Integer(TOTAL_CHARGES, utilityviewmodel))
                {
                    return false;
                }
                SmartParseV2016.Update_Bill(ourviewmodel, utilityviewmodel, "RESOURCE_NEW_CHARGES", TOTAL_CHARGES);
            }
            return true;
        }

        private static bool SectionD2_Check_Period_Dates(UtilityViewModel utilityviewmodel,
                                                        string token)
        {
            string[] components = token.Split(SmartParametersV2016.bar);
            if (components.Length == 2)
            {
                string TEMP_DATE = components[0].Trim();
                if (!SmartParseV2016.Generic_Parse_Datetime(TEMP_DATE, utilityviewmodel))
                {
                    return false;
                }
                else
                {
                    utilityviewmodel.UNIT_CHARGES_PERIOD_START = TEMP_DATE;
                }
                utilityviewmodel.STANDING_CHARGES_PERIOD_START = utilityviewmodel.UNIT_CHARGES_PERIOD_START;
                TEMP_DATE = components[1].Trim();
                if (!SmartParseV2016.Generic_Parse_Datetime(TEMP_DATE, utilityviewmodel))
                {
                    return false;
                }
                utilityviewmodel.UNIT_CHARGES_PERIOD_END = TEMP_DATE;
                utilityviewmodel.STANDING_CHARGES_PERIOD_END = utilityviewmodel.UNIT_CHARGES_PERIOD_END;
            }
            return true;
        }

        private static void SectionD2_Check_Tracker(UtilityViewModel utilityviewmodel,
                                                    string token)
        {
            if (token.Substring(0, 1) != "(")
            {
                if (string.IsNullOrEmpty(utilityviewmodel.SC))
                {
                    utilityviewmodel.SC = token;
                    utilityviewmodel.SC = utilityviewmodel.SC.Replace(". days", " days").Trim();
                    utilityviewmodel.SC = utilityviewmodel.SC.Replace(".p", "p").Trim();
                }
                else
                {
                    int currency_index = token.IndexOf(utilityviewmodel.bill_currency_symbol);
                    if (currency_index >= 0)
                    {
                        utilityviewmodel.SC = utilityviewmodel.SC + SmartParametersV2016.space +
                            token.Substring(0, currency_index).Replace(". days", ".days").Replace(".p", "p").Replace(".days", " days").Replace(".00", "").Trim();
                    }
                    else
                    {
                        utilityviewmodel.SC = utilityviewmodel.SC + SmartParametersV2016.space + token;
                    }
                }
            }
            return;
        }
        private static bool SectionD2_Check_Discounts(MainViewModel ourviewmodel,
                                                        UtilityViewModel utilityviewmodel,
                                                        string token,
                                                        int currency_index,
                                                        DateTime UNIT_CHARGES_PERIOD_END,
                                                        string DISCOUNT_TYPE)
        {
            if (currency_index >= 0)
            {
                short discount_item = 0;
                string DISCOUNT_AMOUNT,
                        DISCOUNT_CREDIT_DATE;

                short DISCOUNT_VAT_CODE = SmartParametersV2016.zeroRateVatCode;
                // Get some kind of discount date
                DateTime DISCOUNT_DATE = UNIT_CHARGES_PERIOD_END;

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
                DISCOUNT_CREDIT_DATE = utilityviewmodel.BILL_DATE.Substring(2);
                discount_item = (short)(discount_item + 1);
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
                    SmartParseV2016.Update_Bill(ourviewmodel, utilityviewmodel, "DISCOUNTS", DISCOUNT_AMOUNT);
                    SmartParseV2016.Update_Bill(ourviewmodel, utilityviewmodel, "DISCOUNT_CREDIT_DATE", DISCOUNT_CREDIT_DATE);
                }
            }
            return true;
        }
        private static bool Section_D2_Units(UtilityViewModel utilityviewmodel,
                                                string token)
        {
            utilityviewmodel.UNITS_COST = "";
            utilityviewmodel.UNITS_RATE = "";
            utilityviewmodel.UNITS = "";
            string[] components = token.Split(SmartParametersV2016.bar);
            // Work backwards ...
            int component_count = components.Length - 1;
            int times_through = 0;
            while (component_count >= 0)
            {
                switch (times_through)
                {
                    case 0:
                        // Units Cost
                        if (!SmartParseV2016.Currency_Strip(utilityviewmodel.bill_currency_symbol, utilityviewmodel.bill_currency_separator, utilityviewmodel.bill_thousands_separator, components[component_count], utilityviewmodel))
                        {
                            return false;
                        }
                        utilityviewmodel.UNITS_COST = utilityviewmodel.value;
                        if (!SmartParseV2016.Generic_Parse_Integer(utilityviewmodel.UNITS_COST, utilityviewmodel))
                        {
                            return false;
                        }
                        break;
                    case 1:
                        // Units Rate
                        components[component_count] = components[component_count].Replace(SmartParametersV2016.decimalPoint + utilityviewmodel.bill_denomination_symbol, utilityviewmodel.bill_denomination_symbol.ToString());
                        utilityviewmodel.UNITS_RATE = components[component_count].Replace(utilityviewmodel.bill_denomination_symbol.ToString(), "").Trim();
                        if (!SmartParseV2016.Generic_Parse_Decimal(utilityviewmodel.UNITS_RATE, utilityviewmodel))
                        {
                            return false;
                        }
                        break;
                    case 2:
                        // Unit of Measure
                        utilityviewmodel.UNIT_OF_MEASURE = components[component_count].Trim();
                        break;
                    case 3:
                        // Units
                        utilityviewmodel.UNITS = components[component_count].Trim();
                        // Sometimes Gas values have these embedded
                        //UNITS = UNITS.Replace("comma", "");
                        if (!SmartParseV2016.Generic_Parse_Decimal(utilityviewmodel.UNITS, utilityviewmodel))
                        {
                            return false;
                        }
                        break;
                    default:
                        if (string.IsNullOrEmpty(utilityviewmodel.UNITS_TYPE))
                        {
                            utilityviewmodel.UNITS_TYPE = components[component_count].Trim();
                        }
                        else
                        {
                            utilityviewmodel.UNITS_TYPE = components[component_count].Trim() + SmartParametersV2016.space + utilityviewmodel.UNITS_TYPE;
                        }
                        break;
                }
                times_through++;
                component_count--;
            }

            // first or next 
            utilityviewmodel.units_band = (short)(utilityviewmodel.units_band + 1);

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
                                                                        utilityviewmodel.UNIT_CHARGES_PERIOD_START,
                                                                        utilityviewmodel.UNIT_CHARGES_PERIOD_END,
                                                                        utilityviewmodel.units_band.ToString(),
                                                                        utilityviewmodel.UNITS_TYPE,
                                                                        utilityviewmodel.UNITS,
                                                                        utilityviewmodel.UNITS_RATE,
                                                                        utilityviewmodel.UNIT_OF_MEASURE,
                                                                        utilityviewmodel.UNITS_COST);
                    utilityviewmodel.sc_tracker = true;
                    utilityviewmodel.SC = "";
                    break;
                default:
                    break;
            }
            return true;
        }

        private static bool SectionD2_Check_Charges(UtilityViewModel utilityviewmodel,
                                                    string token)
        {
            int temp = 0;

            string CHARGES_COST = "0",
                    STANDING_CHARGE = "0";
            short CHARGES_DAYS = 0;

            string[] components = token.Split(SmartParametersV2016.bar);
            // Work backwards ...
            int component_count = components.Length - 1;
            int times_through = 0;
            while (component_count >= 0)
            {
                switch (times_through)
                {
                    case 0:
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
                        else
                        {
                            temp = utilityviewmodel.genericTransactionValue;
                        }
                        break;
                    case 1:
                        // Charges Rate
                        components[component_count] = components[component_count].Replace(SmartParametersV2016.decimalPoint + utilityviewmodel.bill_denomination_symbol, utilityviewmodel.bill_denomination_symbol.ToString());
                        STANDING_CHARGE = components[component_count].Replace(utilityviewmodel.bill_denomination_symbol.ToString(), "").Trim();
                        if (!SmartParseV2016.Generic_Parse_Decimal(STANDING_CHARGE, utilityviewmodel))
                        {
                            return false;
                        }
                        break;
                    case 2:
                        // UoM - not used yet for Standing Charges
                        // UNIT_OF_MEASURE = components[component_count].Trim(); 
                        // Sometimes ... days has a decimal point e.g. 99.00
                        break;
                    case 3:
                        // Days
                        string charges_days = components[component_count].Trim();
                        // Sometimes ... days has a decimal point e.g. 99.00

                        if (!SmartParseV2016.Generic_Parse_Decimal(charges_days, utilityviewmodel))
                        {
                            return false;
                        }
                        CHARGES_DAYS = Convert.ToInt16(temp);
                        break;
                    default:
                        break;
                }
                times_through++;
                component_count--;
            }

            // first or next 
            utilityviewmodel.charges_item = (short)(utilityviewmodel.charges_item + 1);

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
                                                            utilityviewmodel.STANDING_CHARGES_PERIOD_START,
                                                            utilityviewmodel.STANDING_CHARGES_PERIOD_END,
                                                            utilityviewmodel.charges_item.ToString(),
                                                            utilityviewmodel.CHARGES_TYPE,
                                                            STANDING_CHARGE,
                                                            CHARGES_DAYS.ToString(),
                                                            CHARGES_COST);
                    break;
                default:
                    break;
            }
            return true;
        }
    }
}