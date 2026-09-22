using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

#if ANDROIDX
using AndroidX.AppCompat.App;
#endif
namespace SmartCubeMobile
{
    public class ScottishPowerV2016
    {
        // ONe day this fucking busybody fucking noisy bitch will shut the fuck up
        // forever about her stupid family and what she fucking thinks which is
        // of no interest to anybody.  She is a shallow spiteful rude, ignorant
        // arrogant two-faced sanctimonious pompous gossip without an iota of intelligence
        // or common sense or intellect - in short - a fucking moron

        // I really, really, really am at the end of my tether over this stuff
        // These Scottish Power cunts have 'SUDDENLY' decided to put a 'front;
        // Login screen in front of the 'normal' Login screen.  What ARE these
        // fucking Foreigners up to??  They don't have a fucking clue.  Every time
        // I hit a problem like this my nerves are torn to shreds.  THEN they
        // decide that sometimes their host has to be www.scottishpower.co.uk'
        // and other times it has to be 'www.spenergy.co.uk' and if you get the
        // wrong host?  They close the fucking connection on you .... I have had
        // it with these fucking, fucking, fucking, fucking paki sweaty morons
        // Absolutely - I spent A WHOLE SAY sorting their shit out AGAIN (or is that
        // the 3rd or 4th time??)

        // These Scottish Power FUCKERS have decided to make their PDF files
        // completely image only without any fucking texts or anything.
        // They are absolute fucking numbskulls.
        //          ======== ======= ==========
        // Then, a couple of years ago, they changed all this and they
        // became readable.  However they are ALWAYS fucking about and tinkering
        // with their fucking website making it almost unscraepable

        // SO WHERE HAVE ALL THE ENTRIES FOR UNIT_CHARGES, STANDING CHARGES AND CONSUMPTION GONE?
        // They are never added in, because they are not displayed on the website AND we
        // can't download the bill to analyze them ...
        // But all we need are the READINGS (which we have) in order to make a tariff
        // comparison.  So the UNit_Charges, Standing_Charges and COnsumption are a "nice to
        // have", they are not a "must have". We can live with this until those fucking
        // sweaty paki cunts do a proper website ..

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

            // Because Sometimes we are returned a Document
            utilityviewmodel.htmlDocument = new HtmlAgilityPack.HtmlDocument();

            utilityviewmodel.keyValues = new List<KeyValuePair<string, string>>();

            // Everytime timer ticks, timer_Tick will be called
            // Timer will tick evert second
            // FUCKING HELL - this is never Enabled????
            // You have 60 Seconds to stop
            // the timer on a good login!
            bool download_done = false;
            string account_overview_pathname = "",
                    //account_pathname = "",
                    edit_personal_details_pathname = "",
                    payment_plan_pathname = "",
                    energy_usage_pathname = "",
                    view_my_balance_pathname = "";

            utilityviewmodel.gotomyaccount_pathname = "";
            utilityviewmodel.url_addon = "";
            utilityviewmodel.sessionFormUID = "";
            utilityviewmodel.login_post_action = "";

            utilityviewmodel.eventID = "";

            utilityviewmodel.keep_looping = true;
            List<SmartUtility.Payments> payments_tempList = new List<SmartUtility.Payments>();

            // SP_Login1
            //      -> SP_Login2
            //          -> SP_MyAccount ( set: resource(E, G or E+G)
            //                            set: my_energy_usage link
            //                            set: enter_meter_reading link)
            //                            set: view_my_balance link)
            //              -> SP_MyDetails                 (no resource -> Logout)
            //                  -> SP_EnterMeterReading     (with: resource = 'E' link = enter_meter_readings)
            //              -> SP_ViewMyBalance             (with: resource = 'E' <= until no more pages)
            //
            //              -> SP_MyDetails                 (no resource -> Logout)
            //                  -> SP_EnterMeterReading     (with: resource = 'G' link = enter_meter_readings)
            //              -> SP_ViewMyBalance             (with: resource = 'G' <= until no more pages)
            //
            // Yes... we read the same fucking Bill TWICE whenits dual fuel, but this really is the
            // easiest way of doing it believe me (and I KNOW for all the fucking PAIN I have suffered)
            // Its ... cup of tea time Raymond!
            //

            // It appears that after every POST, the SP Foreigners require you ro do another
            // POST with a submit.  Its only these Foreigners that run their stupid website like this ...

            utilityviewmodel.sessionID_name = "";
            utilityviewmodel.sessionID_value = "";

            while (utilityviewmodel.keep_looping)
            {
#if WINFORMS
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
                if (!string.IsNullOrEmpty(ourviewmodel.errorMessage) ||
                    utilityviewmodel.next_routine == "LOGOUT")
                {
                    utilityviewmodel.target_pathname = utilityviewmodel.logout_pathname;
                    utilityviewmodel.keep_looping = false;
                    utilityviewmodel.next_routine = "LOGOUT";
                }
                if ((utilityviewmodel.next_routine != "HOME"))
                {
                    if (utilityviewmodel.next_routine == utilityviewmodel.next_routine.ToUpper())
                    {
                        // UPPER CASE its a GET

                        utilityviewmodel.htmlDocument = await SmartBobV2017.Scraper_Generic_Get(ourviewmodel, utilityviewmodel.utilityToken, utilityviewmodel);
                        if (!string.IsNullOrEmpty(ourviewmodel.errorMessage))
                        {
                            return false;
                        }
                    }
                    else
                    {
                        // Mixed case its a POST
                        // Notice how for SP (!!!)we are interested in the returned document from the POST

                        utilityviewmodel.htmlDocument = await SmartBobV2017.Scraper_Generic_Post(ourviewmodel, utilityviewmodel.utilityToken, utilityviewmodel.keyValues, utilityviewmodel);
                        if (!string.IsNullOrEmpty(ourviewmodel.errorMessage))
                        {
                            return false;
                        }
                    }
                }
                utilityviewmodel.current_routine = utilityviewmodel.next_routine;
                switch (utilityviewmodel.next_routine)
                {
                    // These fucking sweaty fuckers have got TWO fucking Logins
                    case "LOGIN":   // here from a GET
                        // The timer is re-started when the
                        // first Login Document has been completed
                        utilityviewmodel.keyValues.Clear();
                        if (await SP_Login1(
#if ANDROIDX
                                        meterActivity,
#endif
                                        ourviewmodel,
                                        utilityviewmodel,
                                        utilityviewmodel.htmlDocument,
                                        TextBox_Active))
                        {
                            // Add the 'special' query string
                            utilityviewmodel.target_pathname = "account/login.process";
                            // Should now be going on to Login2 i.e. on a POST
                        }
                        else
                        {
                            ourviewmodel.errorMessage = utilityviewmodel.current_routine;
                            utilityviewmodel.next_routine = "LOGOUT";
                            utilityviewmodel.login_finished = true;          // Escape route on Login failure
                        }
                        break;
                    case "LOGIN2":
                    case "Login2":      // Here from a POST
                        // The timer is re-started when the
                        // first Login Document has been completed
                        if (SP_Login2(utilityviewmodel,
                                        utilityviewmodel.htmlDocument))
                        {
                            // Add the normal query string
                            // Fucking SweatyPower paki cunts have changed their login YET AGAIN
                            // is this the 5th or 6th time??? and now want the sessionID found
                            // in SP_Login1.
                            // Fucking brainless sweaty paki fuckers
                            utilityviewmodel.keyValues.Clear();
                            utilityviewmodel.keyValues.Add(new KeyValuePair<string, string>("sessionFormUID", utilityviewmodel.sessionFormUID)); // From SP_Login1

                            // Add the 'special' query string
                            utilityviewmodel.target_pathname = utilityviewmodel.login_post_action;
                            account_overview_pathname = utilityviewmodel.target_pathname;
                            utilityviewmodel.target_pathname = utilityviewmodel.target_pathname + "&" + utilityviewmodel.url_addon;
                            utilityviewmodel.next_routine = "MyAccountOverview"; // Lower case its a POST
                        }
                        else
                        {
                            ourviewmodel.errorMessage = utilityviewmodel.current_routine;
                            utilityviewmodel.next_routine = "LOGOUT";
                            utilityviewmodel.login_finished = true;          // Escape route on Login failure
                        }
                        break;
                    case "MyAccountOverview":   // Here from a POST
                    case "MYACCOUNTOVERVIEW":   // Here from a GET
                        // Is where we set these two
                        string account_post_action = "";
                        if (SP_MyAccountOverview(utilityviewmodel,
                                                utilityviewmodel.htmlDocument))
                        {
                            if (!string.IsNullOrEmpty(utilityviewmodel.eventID))
                            {
                                // Pick off the LAST one
                                string[] comp = utilityviewmodel.fuelList.Split(SmartParametersV2016.bar);
                                int count = comp.Length;
                                if (count > 0)
                                {
                                    utilityviewmodel.resource_code = Convert.ToChar(comp[count - 1]);// We are doing this resource
                                    SmartUtilityV2022.Fix_Resource_Type(utilityviewmodel, SmartUtilityV2022.Determine_Resource_Type(utilityviewmodel)); // For the time being 

                                    List<Amelia> amelia_found = SmartUtilityV2022.Amelia_Lookup(ourviewmodel, utilityviewmodel, false);
                                    if (amelia_found.Count == 0)
                                    {

                                        if (!await SP_Amelia_Stuff(ourviewmodel,
                                                                 utilityviewmodel,
                                                                 utilityviewmodel.account_address))


                                        // Rebuild the fuel list to exclude the one we are 'on'
                                        {
                                            return false;
                                        }
                                    }

                                    utilityviewmodel.keyValues.Clear();
                                    // From SP_Login1
                                    utilityviewmodel.keyValues.Add(new KeyValuePair<string, string>("sessionFormUID", utilityviewmodel.sessionFormUID));
                                    utilityviewmodel.keyValues.Add(new KeyValuePair<string, string>(utilityviewmodel.eventID, "View"));

                                    utilityviewmodel.target_pathname = account_post_action;
                                    utilityviewmodel.next_routine = "Submit"; //Lower case its a POST
                                }
                            }
                            else
                            {
                                ////https://www.scottishpower.co.uk/my-account/energyusage.process?execution=e2s1
                                //utilityviewmodel.next_routine = "MYENERGYUSAGE";
                                //utilityviewmodel.target_pathname = energy_usage_pathname; // login_post_action.Replace("choosecontract", "energyusage");
                                utilityviewmodel.next_routine = "LOGOUT";
                            }
                        }
                        else
                        {
                            ourviewmodel.errorMessage = utilityviewmodel.current_routine;
                        }
                        break;
                    case "Submit":  // Here from a POST
                        string submit_post_action = "";
                        if (SP_Submit(utilityviewmodel,
                                        utilityviewmodel.htmlDocument))
                        {
                            utilityviewmodel.keyValues.Clear();
                            // From SP_Login1
                            utilityviewmodel.keyValues.Add(new KeyValuePair<string, string>("sessionFormUID", utilityviewmodel.sessionFormUID));
                            // Add the 'special' query string
                            utilityviewmodel.target_pathname = submit_post_action;
                            utilityviewmodel.target_pathname = utilityviewmodel.target_pathname + "&" + utilityviewmodel.url_addon;
                            utilityviewmodel.next_routine = "MyAccount"; // Lower case its a POST
                        }
                        break;
                    case "GETPATHS":        // Here on a GET
                                            // Is where we set these two
                        utilityviewmodel.my_energy_usage_path = "";
                        utilityviewmodel.view_my_balance_path = "";
                        utilityviewmodel.edit_personal_details_path = "";
                        utilityviewmodel.payment_plan_path = "";
                        if (SP_MyAccount(ourviewmodel,
                                                utilityviewmodel,
                                                utilityviewmodel.htmlDocument))
                        {
                            // Now we can look up the Tariff Name because we have a Resource Code and
                            // an Area Code
                            if (!string.IsNullOrEmpty(utilityviewmodel.TARIFF_NAME) &&
                                utilityviewmodel.TARIFF_CODE == 0 &&
                                utilityviewmodel.area_code != 0)
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
                            if (!string.IsNullOrEmpty(edit_personal_details_pathname) &&
                                !string.IsNullOrEmpty(payment_plan_pathname))
                            {
                                utilityviewmodel.next_routine = "EDITPERSONALDETAILS";
                                utilityviewmodel.target_pathname = edit_personal_details_pathname;
                            }
                            else
                            {
                                if (!string.IsNullOrEmpty(view_my_balance_pathname))
                                {
                                    utilityviewmodel.next_routine = "VIEWMYBAL";
                                    utilityviewmodel.target_pathname = view_my_balance_pathname;
                                }
                            }
                        }
                        else
                        {
                            utilityviewmodel.next_routine = "LOGOUT";
                        }
                        // Should now be going on to EDITPERSONALDETAILS
                        break;

                    case "PAYMENTPLAN":
                        if (SP_PaymentPlan(utilityviewmodel,
                                           utilityviewmodel.htmlDocument))
                        {
                            utilityviewmodel.target_pathname = view_my_balance_pathname;
                            utilityviewmodel.next_routine = "VIEWMYBAL"; // Upper case its a GET
                        }
                        else
                        {
                            utilityviewmodel.next_routine = "LOGOUT";
                        }
                        // Should now be going on to ENTER or LOGOUT
                        break;
                    case "VIEWMYBAL":

                        if (await SP_ViewMyBalance(
#if ANDROIDX
                                                    meterActivity,
#endif
                                                    ourviewmodel,
                                                    utilityviewmodel,
                                                    utilityviewmodel.htmlDocument,
                                                        TextBox_Active,
                                                        //payments_tempList,
                                                        account_overview_pathname))

                        {
                            utilityviewmodel.keyValues.Clear();
                            //// Add the 'special' query string
                            //utilityviewmodel.target_pathname = account_overview_pathname;
                            //utilityviewmodel.next_routine = "LOGIN2";    // Upper case its a GET

                            //https://www.scottishpower.co.uk/my-account/energyusage.process?execution=e2s1
                            utilityviewmodel.next_routine = "MYENERGYUSAGE";
                            utilityviewmodel.target_pathname = energy_usage_pathname; // login_post_action.Replace("choosecontract", "energyusage");

                        }
                        break;
                    case "DOWNLOAD":
                        utilityviewmodel.target_pathname = account_overview_pathname;
                        utilityviewmodel.next_routine = "LOGIN2";    // Upper case its a GET
                        if (!download_done)
                        {

                            if (!await SP_Download(ourviewmodel,
                                                utilityviewmodel,
                                                utilityviewmodel.htmlDocument))
                            {
                                if (!string.IsNullOrEmpty(ourviewmodel.errorMessage))
                                {
                                    return false;
                                }
                                utilityviewmodel.next_routine = "LOGOUT";
                            }
                            else
                            {
                                download_done = true;
                            }
                        }
                        break;
                    case "LOGOUT":
                        // This is where we build up one Utility.Accounts_out from the other



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
                        SP_Do_Intermediates(utilityviewmodel,
                                            utilityviewmodel.next_routine,
                                            utilityviewmodel.htmlDocument);
                        break;
                }
            }
            await SmartUtilityV2022.Last_Throw(
#if ANDROIDX
                                                meterActivity,
#endif
                                                ourviewmodel, utilityviewmodel);

            // Yay!
            return utilityviewmodel.login_finished;
        }

        private static void SP_Do_Intermediates(UtilityViewModel utilityviewmodel,
                                                string next_routine,
                                                HtmlAgilityPack.HtmlDocument htmlDocument)
        {
            switch (next_routine)
            {
                case "MyAccount":   // here from a POST
                    if (SP_GoToMyAccount(utilityviewmodel,
                                htmlDocument))
                    {
                        utilityviewmodel.target_pathname = utilityviewmodel.gotomyaccount_pathname;
                        utilityviewmodel.next_routine = "GETPATHS";  // Upper case its a GET
                    }
                    else
                    {
                        utilityviewmodel.next_routine = "LOGOUT";
                    }
                    break;
                case "EDITPERSONALDETAILS":
                    if (SP_EditPersonalDetails(utilityviewmodel,
                                            htmlDocument))
                    {
                        utilityviewmodel.target_pathname = utilityviewmodel.payment_plan_pathname;
                        utilityviewmodel.next_routine = "PAYMENTPLAN";
                    }
                    else
                    {
                        utilityviewmodel.next_routine = "LOGOUT";
                    }
                    // Should now be going on to PAYMENTPLAN or LOGOUT
                    break;
                case "MYENERGYUSAGE":
                    SP_MyEnergyUsage(htmlDocument,
                                                utilityviewmodel);
                    break;
            }
            return;
        }

        private static async Task<bool> SP_Login1(
#if ANDROIDX
                                        AppCompatActivity meterActivity,
#endif
                                        MainViewModel ourviewmodel,
                                        UtilityViewModel utilityviewmodel,
                                        HtmlAgilityPack.HtmlDocument document,
                                        bool TextBox_Active)
        {

            // This is the page which is delivered as "Complete" when we enter
            // the initial Supplier target URL
            // However!  At LEAST these Scottish Power fuckers don't re-direct us off
            // to a 'secure' URL ....
            // We then unhook this routine (i.e. wb_DocumentCompleted_Login)
            // and hook onto the routine to handle the Account details for Annie

            IList<HtmlAgilityPack.HtmlNode>
                    HtmlCol;
            string name,
                    value = "";
            bool clicked = false;
            List<KeyValuePair<string, string>> keyValues = new List<KeyValuePair<string, string>>();

            HtmlCol = YetAnotherFuckingHoop.SelectNodesAsList(document.DocumentNode, "//input");
            foreach (HtmlAgilityPack.HtmlNode element in HtmlCol)
            {
                if (!string.IsNullOrEmpty(element.Id))
                {
                    switch (element.Id)
                    {
                        case "username":
                            // Set the user name in the username text box
                            keyValues.Add(new KeyValuePair<string, string>("email", utilityviewmodel.user_id));

                            if (TextBox_Active)
                            {
#if WINFORMS
                                await SmartRoutinesV2018.TextBlockUpdate(
                                    ourviewmodel,
                                    "UserId" +
                                    SmartParametersV2016.space +
                                    utilityviewmodel.user_id);
#endif
#if WPF 
                                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel,
                                                                                "UserId" +
                                                                                SmartParametersV2016.space +
                                                                                utilityviewmodel.user_id);
#endif
#if ANDROIDX
                                await SmartRoutinesV2018.TextBlockUpdate(
#if ANDROIDX
                                                                            meterActivity,
#endif
                                                                                ourviewmodel,
                                                                                "UserId" +
                                                                                SmartParametersV2016.space +
                                                                                utilityviewmodel.user_id);
#endif
                            }
                            break;
                        case "password":
                            //Type the password in the password text box
                            keyValues.Add(new KeyValuePair<string, string>(element.Id, utilityviewmodel.password));
                            // Hide the password in the log
                            if (TextBox_Active)
                            {
#if WINFORMS
                                await SmartRoutinesV2018.TextBlockUpdate(
                                    ourviewmodel,
                                    "Password" +
                                    SmartParametersV2016.space +
                                    "Asterisks");
#endif
#if WPF 
                                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel,
                                       "Password" +
                                       SmartParametersV2016.space +
                                       "Asterisks");
#endif
#if ANDROIDX
                                await SmartRoutinesV2018.TextBlockUpdate(
#if ANDROIDX
                                       meterActivity,
#endif
                                        ourviewmodel,
                                       "Password" +
                                       SmartParametersV2016.space +
                                       "Asterisks");
#endif
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
            clicked = false;

            utilityviewmodel.keyValues = keyValues;

            HtmlCol = YetAnotherFuckingHoop.SelectNodesAsList(document.DocumentNode, "//input");
            foreach (HtmlAgilityPack.HtmlNode element in HtmlCol)
            {
                if (string.IsNullOrEmpty(element.Id))
                {
                    name = element.GetAttributeValue("name", "");
                    if (name == "sessionFormUID")
                    {
                        value = element.GetAttributeValue("value", "");
                        utilityviewmodel.sessionID_name = name;
                        utilityviewmodel.sessionID_value = value;  // For SP_Login2
                        // Well this has to be utilityviewmodel. ? otherwise its pointless just adding to keyValues
                        // for this routine because the list is lost when we exit it ??
                        utilityviewmodel.keyValues.Add(new KeyValuePair<string, string>(name, value));

                        utilityviewmodel.login_attempted = true;
                        // Hook up the next routine - if the login is true - for the next 'Document Completed' delivery
                        utilityviewmodel.next_routine = "Login2";    // Lower case Its a POST

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
                    }
                }
            }

            // If our login was successful a 'secure' Document
            // will be delivered and we have told the handler
            // to call the SecureLogin routine below when that
            // happens
            // If our login was UNSUCCESSFUL, then no Document
            // will be delivered and the timer will tick away
            // the 10 seconds and exit the timer loop when it
            // reaches  0
            if (!clicked)
            {
                utilityviewmodel.next_routine = "LOGOUT";
            }
            return clicked;
        }

        private static bool SP_Login2(UtilityViewModel utilityviewmodel,
                                        HtmlAgilityPack.HtmlDocument document)
        {
            // This is the page which is delivered as "Complete" when we enter
            // the initial Supplier target URL
            // However!  At LEAST these Scottish Power fuckers don't re-direct us off
            // to a 'secure' URL ....
            // We then unhook this routine (i.e. wb_DocumentCompleted_Login)
            // and hook onto the routine to handle the Account details for Annie

            IList<HtmlAgilityPack.HtmlNode>
                    HtmlCol;
            bool clicked = false;
            string classname,
                    name;


            HtmlCol = YetAnotherFuckingHoop.SelectNodesAsList(document.DocumentNode, ".//form");
            foreach (HtmlAgilityPack.HtmlNode element in HtmlCol)
            {
                string method = element.GetAttributeValue("method", "");

                if (method == "post")
                {
                    utilityviewmodel.post_action = element.GetAttributeValue("action", "");
                    utilityviewmodel.post_action = utilityviewmodel.post_action.Replace(utilityviewmodel.prfix_xxx, "");
                    break;
                }
            }


            HtmlCol = YetAnotherFuckingHoop.SelectNodesAsList(document.DocumentNode, "//input");
            foreach (HtmlAgilityPack.HtmlNode element in HtmlCol)
            {
                name = element.GetAttributeValue("name", "");
                if (name == "sessionFormUID")
                {
                    string value = element.GetAttributeValue("value", "");
                    utilityviewmodel.sessionFormUID = value;
                }

                if (string.IsNullOrEmpty(element.Id))
                {
                    classname = element.GetAttributeValue("class", "");
                    if (classname == "input-btn")
                    {
                        name = element.GetAttributeValue("name", "");
                        if (!string.IsNullOrEmpty(name))
                        {
                            utilityviewmodel.url_addon = "";
                            string[] components = name.Split('_');
                            int component_count = 0;
                            while (component_count < components.Length)
                            {
                                switch (component_count)
                                {
                                    case 1:
                                        utilityviewmodel.url_addon = Uri.EscapeDataString(SmartParametersV2016.underscore + components[component_count]) + SmartParametersV2016.equivalent;
                                        break;
                                    case 2:
                                        utilityviewmodel.url_addon += Uri.EscapeDataString(components[component_count]);
                                        break;
                                    default:
                                        break;
                                }
                                component_count++;
                            }
                        }
                        // Hook up the next routine - if the login is true - for the next 'Document Completed' delivery
                        clicked = true;
                    }
                }
            }
            return clicked;
        }

        private static bool SP_Submit(UtilityViewModel utilityviewmodel,
                                    HtmlAgilityPack.HtmlDocument document)
        {
            // This is the page which is delivered as "Complete" when we enter
            // the initial Supplier target URL
            // However!  At LEAST these Scottish Power fuckers don't re-direct us off
            // to a 'secure' URL ....
            // We then unhook this routine (i.e. wb_DocumentCompleted_Login)
            // and hook onto the routine to handle the Account details for Annie

            IList<HtmlAgilityPack.HtmlNode>
                    HtmlCol;

            HtmlCol = YetAnotherFuckingHoop.SelectNodesAsList(document.DocumentNode, ".//form");
            foreach (HtmlAgilityPack.HtmlNode element in HtmlCol)
            {
                string method = element.GetAttributeValue("method", "");

                if (method == "post")
                {
                    utilityviewmodel.post_action = element.GetAttributeValue("action", "");
                    utilityviewmodel.post_action = utilityviewmodel.post_action.Replace(utilityviewmodel.prfix_xxx, "");
                    break;
                }
            }
            return true;
        }

        private static bool SP_GoToMyAccount(UtilityViewModel utilityviewmodel,
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

            // 'Phone is hashed through' <= phone is patched through!

            HtmlCol1 = YetAnotherFuckingHoop.SelectNodesAsList(document.DocumentNode, ".//a");
            foreach (HtmlAgilityPack.HtmlNode element1 in HtmlCol1)
            {
                // The matching criteria ....
                string classname = element1.GetAttributeValue("class", "");
                if (SmartNibbyV2016.Check_Classname(classname, "button", true))
                {
                    string inner_text = element1.InnerText;
                    if (inner_text == "Go to My Account")
                    {
                        utilityviewmodel.gotomyaccount_pathname = element1.GetAttributeValue(SmartParametersV2016.href, "");
                        utilityviewmodel.gotomyaccount_pathname = utilityviewmodel.gotomyaccount_pathname.Replace(utilityviewmodel.prfix_xxx, "");
                        clicked = true;
                        break;
                    }
                }
            }
            return clicked;
        }

        private static bool SP_MyAccountOverview(UtilityViewModel utilityviewmodel,
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
            string classname1;

            utilityviewmodel.eventID = "";

            HtmlCol1 = YetAnotherFuckingHoop.SelectNodesAsList(document.DocumentNode, ".//form");
            foreach (HtmlAgilityPack.HtmlNode element in HtmlCol1)
            {
                string method = element.GetAttributeValue("method", "");

                if (method == "post")
                {
                    utilityviewmodel.post_action = element.GetAttributeValue("action", "");
                    utilityviewmodel.post_action = utilityviewmodel.post_action.Replace(utilityviewmodel.prfix_xxx, "");
                    break;
                }
            }

            if (string.IsNullOrEmpty(utilityviewmodel.logout_pathname))
            {
                HtmlCol1 = YetAnotherFuckingHoop.SelectNodesAsList(document.DocumentNode, ".//div");
                foreach (HtmlAgilityPack.HtmlNode element1 in HtmlCol1)
                {
                    // The matching criteria ....
                    string name = element1.GetAttributeValue("id", "");
                    if (!string.IsNullOrEmpty(name))
                    {
                        if (name == "loggedIn")
                        {
                            HtmlCol2 = YetAnotherFuckingHoop.SelectNodesAsList(element1, ".//a");
                            foreach (HtmlAgilityPack.HtmlNode element2 in HtmlCol2)
                            {
                                if (!string.IsNullOrEmpty(element2.InnerText))
                                {
                                    if (element2.InnerText == "Logout")
                                    {
                                        utilityviewmodel.logout_pathname = element2.GetAttributeValue(SmartParametersV2016.href, "");
                                        utilityviewmodel.logout_pathname = utilityviewmodel.logout_pathname.Replace(utilityviewmodel.prfix_xxx, "");
                                        break;
                                    }
                                }
                            }
                        }
                    }
                    if (!string.IsNullOrEmpty(utilityviewmodel.logout_pathname))
                    {
                        break;
                    }
                }
            }

            HtmlCol1 = YetAnotherFuckingHoop.SelectNodesAsList(document.DocumentNode, "//fieldset");
            foreach (HtmlAgilityPack.HtmlNode element1 in HtmlCol1)
            {
                // The matching criteria ....
                if (!string.IsNullOrEmpty(element1.Id))
                {
                    classname1 = element1.GetAttributeValue("class", "").Trim();  // Foreigners hae put a fucking space on the end!!!
                    if (SmartNibbyV2016.Check_Classname(classname1, "myaccount-contracts", true))
                    {
                        utilityviewmodel.new_resource = false;

                        HtmlCol2 = YetAnotherFuckingHoop.SelectNodesAsList(element1, ".//div");
                        foreach (HtmlAgilityPack.HtmlNode element2 in HtmlCol2)
                        {
                            SP_Build_FuelList(utilityviewmodel,
                                                element2);
                            if (!string.IsNullOrEmpty(utilityviewmodel.eventID))
                            {
                                break;
                            }
                        }
                    }
                }
                if (!string.IsNullOrEmpty(utilityviewmodel.eventID))
                {
                    break;
                }
            }

            // Hook up THIS one to logout with
            if (string.IsNullOrEmpty(utilityviewmodel.fuelList))
            {
                //HtmlCol1 = YetAnotherFuckingHoop.SelectNodesAsList(document.DocumentNode, ".//a");
                //foreach (HtmlAgilityPack.HtmlNode element1 in HtmlCol1)
                //{
                //    classname1 = element1.GetAttributeValue("class", "").Trim();  // Foreigners hae put a fucking space on the end!!!
                //    if (SmartUtilityV2022.Check_Classname(classname1, "button", true))
                //    {
                //        string href = element1.GetAttributeValue(SmartParametersV2016.href, "");
                //        if (href.Contains("my-account"))
                //        {
                //            utilityviewmodel.target_pathname = href.Replace(utilityviewmodel.prfix_xxx, "");
                //            utilityviewmodel.next_routine = "MYACCOUNT"; // UPPER CASE its a GET
                //            return true; ;
                //        }
                //    }
                //}
                utilityviewmodel.next_routine = "LOGOUT"; // UPPER CASE its a GET
                return false;
            }
            return true;
        }

        private static void SP_Build_FuelList(UtilityViewModel utilityviewmodel,
                                                HtmlAgilityPack.HtmlNode element2)
        {
            IList<HtmlAgilityPack.HtmlNode>
                    HtmlCol3;

            string inner_text;

            // Hmmm .... this wizzes through the /divs and will obliterate previous
            // addresses account nos and account_names with the next fuel resource.
            // Having said that though, I find it difficult to believe that the SP
            // Foreigners would supply on ONE ACCOUNT two different address and names
            // Lets live with this for the time being ....

            string classname = element2.GetAttributeValue("class", "");

            switch (element2.Id)
            {
                case "":
                    if (SmartNibbyV2016.Check_Classname(classname, "heading-section", true))
                    {
                        HtmlCol3 = YetAnotherFuckingHoop.SelectNodesAsList(element2, ".//h3");
                        foreach (HtmlAgilityPack.HtmlNode element3 in HtmlCol3)
                        {
                            if (!string.IsNullOrEmpty(element3.InnerText))
                            {
                                if (string.IsNullOrEmpty(utilityviewmodel.account_address))
                                {
                                    utilityviewmodel.account_address = element3.InnerText.Replace(SmartParametersV2016.newline, SmartParametersV2016.spaceSplit);
                                    break;
                                }
                            }
                        }
                    }
                    else
                    {
                        if (SmartNibbyV2016.Check_Classname(classname, "control-group btn-orange", false))
                        {
                            HtmlCol3 = YetAnotherFuckingHoop.SelectNodesAsList(element2, ".//input");
                            foreach (HtmlAgilityPack.HtmlNode element3 in HtmlCol3)
                            {
                                string value = element3.GetAttributeValue("value", "");
                                if (value == "View")
                                {
                                    string name = element3.GetAttributeValue("name", "");
                                    if (!string.IsNullOrEmpty(name))
                                    {
                                        if (utilityviewmodel.new_resource)
                                        {
                                            utilityviewmodel.eventID = name;
                                            return;
                                        }
                                        break;
                                    }
                                }
                            }
                        }
                    }
                    break;
                case "service-panel":
                    HtmlCol3 = YetAnotherFuckingHoop.SelectNodesAsList(element2, ".//*");
                    foreach (HtmlAgilityPack.HtmlNode element3 in HtmlCol3)
                    {
                        classname = element3.GetAttributeValue("class", "");
                        if (!string.IsNullOrEmpty(classname))
                        {
                            if (!string.IsNullOrEmpty(element3.InnerText))
                            {
                                inner_text = element3.InnerText;
                                if (inner_text.Contains("Gas"))
                                {
                                    if (!utilityviewmodel.fuelList.Contains(SmartParametersV2016.Gas.ToString()))
                                    {
                                        if (!string.IsNullOrEmpty(utilityviewmodel.fuelList))
                                        {
                                            utilityviewmodel.fuelList += SmartParametersV2016.bar;
                                        }
                                        utilityviewmodel.fuelList += SmartParametersV2016.Gas.ToString();
                                        utilityviewmodel.new_resource = true;
                                    }
                                }
                                if (inner_text.Contains("Electricity"))
                                {
                                    if (!utilityviewmodel.fuelList.Contains(SmartParametersV2016.Electricity.ToString()))
                                    {
                                        if (!string.IsNullOrEmpty(utilityviewmodel.fuelList))
                                        {
                                            utilityviewmodel.fuelList += SmartParametersV2016.bar;
                                        }
                                        utilityviewmodel.fuelList += SmartParametersV2016.Electricity.ToString();
                                        utilityviewmodel.new_resource = true;
                                    }
                                }
                                //if (inner_text.Contains("Dual Fuel"))
                                //{
                                //    utilityviewmodel.fuelList = "E|G";
                                //}
                            }
                        }
                    }
                    break;
                case "account-number-panel":
                    HtmlCol3 = YetAnotherFuckingHoop.SelectNodesAsList(element2, ".//*");
                    foreach (HtmlAgilityPack.HtmlNode element3 in HtmlCol3)
                    {
                        classname = element3.GetAttributeValue("class", "");
                        if (!string.IsNullOrEmpty(classname))
                        {
                            if (!string.IsNullOrEmpty(element3.InnerText))
                            {
                                utilityviewmodel.account_no = element3.InnerText;
                                break;
                            }
                        }
                    }
                    break;
                case "account-name-panel":
                    HtmlCol3 = YetAnotherFuckingHoop.SelectNodesAsList(element2, ".//*");
                    foreach (HtmlAgilityPack.HtmlNode element3 in HtmlCol3)
                    {
                        classname = element3.GetAttributeValue("class", "");
                        if (!string.IsNullOrEmpty(classname))
                        {
                            if (!string.IsNullOrEmpty(element3.InnerText))
                            {
                                utilityviewmodel.account_name = element3.InnerText;
                                break;
                            }
                        }
                    }
                    break;
                default:
                    break;
            }
            return;
        }

        private static bool SP_MyAccount(MainViewModel ourviewmodel,
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
                    HtmlCol3;

            bool clicked = false;



            utilityviewmodel.edit_personal_details_path = "";
            utilityviewmodel.payment_plan_path = "";


            if (string.IsNullOrEmpty(utilityviewmodel.fuelList))
            {
                // we may have switched!
                HtmlCol1 = YetAnotherFuckingHoop.SelectNodesAsList(document.DocumentNode, "//H2");
                foreach (HtmlAgilityPack.HtmlNode element1 in HtmlCol1)
                {
                    string classname1 = element1.GetAttributeValue("class", "").Trim();  // Foreigners hae put a fucking space on the end!!!
                    if (SmartNibbyV2016.Check_Classname(classname1, "icon", true))
                    {
                        if (!string.IsNullOrEmpty(element1.InnerText))
                        {
                            if (element1.InnerText.Contains("Next Steps"))
                            {
                                string text = element1.InnerText.Replace("Next Steps", "").Trim();
                                if (text.Contains("&amp;"))
                                {
                                    text = text.Replace("&amp;", SmartParametersV2016.bar.ToString());
                                    utilityviewmodel.fuelList = text.Replace(SmartParametersV2016.space, "");
                                    break;
                                }
                            }
                        }
                    }
                }
            }



            HtmlCol1 = YetAnotherFuckingHoop.SelectNodesAsList(document.DocumentNode, ".//fieldset");
            foreach (HtmlAgilityPack.HtmlNode element1 in HtmlCol1)
            {
                string classname1 = element1.GetAttributeValue("class", "");
                if (SmartNibbyV2016.Check_Classname(classname1, "myaccount-overview", true))
                {
                    HtmlCol2 = YetAnotherFuckingHoop.SelectNodesAsList(element1, ".//div");
                    foreach (HtmlAgilityPack.HtmlNode element2 in HtmlCol2)
                    {
                        string classname2 = element2.GetAttributeValue("class", "");
                        if (SmartNibbyV2016.Check_Classname(classname2, "cell-pair cell-left", true))
                        {
                            // Look for a "span" and look for a "strong"
                            bool lookup_tariff = false,
                                    lookup_payment_name = false;
                            string payment_type = "";
                            HtmlCol3 = YetAnotherFuckingHoop.SelectNodesAsList(element2, ".//*");
                            foreach (HtmlAgilityPack.HtmlNode element3 in HtmlCol3)
                            {
                                if (!string.IsNullOrEmpty(element3.Name))
                                {
                                    switch (element3.Name)
                                    {
                                        case "span":
                                            if (!string.IsNullOrEmpty(element3.InnerText))
                                            {
                                                if (element3.InnerText.Contains("amount"))
                                                {
                                                    payment_type = element3.InnerText.Replace("amount", "").Trim();
                                                    lookup_payment_name = true;
                                                }
                                                else
                                                {
                                                    switch (Convert.ToChar(element3.InnerText.ToUpper()))
                                                    {
                                                        case SmartParametersV2016.Electricity:
                                                            lookup_tariff = true;
                                                            break;
                                                        case SmartParametersV2016.Gas:
                                                            lookup_tariff = true;
                                                            break;
                                                    }
                                                }
                                            }
                                            break;
                                        case "strong":
                                            if (!string.IsNullOrEmpty(element3.InnerText))
                                            {
                                                if (element3.InnerText.Contains(utilityviewmodel.bill_currency_symbol))
                                                {
                                                    if (lookup_payment_name)
                                                    {
                                                        string PAYMENT_TYPE = payment_type;
                                                        // Find a Payment Plan which contains this Payment Name
                                                        if (SmartSpikeUtilityV2017.Utility_Lookup_PaymentPlan(ourviewmodel,
                                                                                                    utilityviewmodel,
                                                                                                    PAYMENT_TYPE))
                                                        {
                                                            goto the_old_days;  // We have all we need
                                                            //SmartParseV2016.Update_Bill("PAYMENT_PLAN", utilityviewmodel.PAYMENT_PLAN.ToString())));
                                                            //SmartParseV2016.Update_Bill("PAYMENT_TYPE", PAYMENT_TYPE);
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    if (lookup_tariff)
                                                    {
                                                        utilityviewmodel.TARIFF_NAME = element3.InnerText;
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
                    }
                }
            }
        the_old_days:
            if (!string.IsNullOrEmpty(utilityviewmodel.account_no))
            {
                HtmlCol1 = YetAnotherFuckingHoop.SelectNodesAsList(document.DocumentNode, ".//a");
                foreach (HtmlAgilityPack.HtmlNode element1 in HtmlCol1)
                {
                    if (string.IsNullOrEmpty(element1.Id))
                    {
                        if (!string.IsNullOrEmpty(element1.InnerText))
                        {
                            SP_Build_Paths(utilityviewmodel,
                                            element1);
                        }
                    }
                }
                if (//!string.IsNullOrEmpty(edit_personal_details_path) &&  // Sometimes these are blank
                    //!string.IsNullOrEmpty(payment_plan_path) &&           // when we are switching
                    !string.IsNullOrEmpty(utilityviewmodel.my_energy_usage_path) &&
                    !string.IsNullOrEmpty(utilityviewmodel.view_my_balance_path) &&
                    !string.IsNullOrEmpty(utilityviewmodel.next_routine))
                {
                    clicked = true;
                }
            }
            return clicked;
        }

        private static void SP_Build_Paths(UtilityViewModel utilityviewmodel,
                                                HtmlAgilityPack.HtmlNode element1)
        {
            string inner_text = element1.InnerText.Trim();
            switch (inner_text)
            {
                case "Edit personal details":
                    utilityviewmodel.edit_personal_details_path = element1.GetAttributeValue(SmartParametersV2016.href, "");
                    utilityviewmodel.edit_personal_details_path = utilityviewmodel.edit_personal_details_path.Replace(utilityviewmodel.prfix_xxx, "");
                    break;
                case "Change bank details &amp; payment date":
                    utilityviewmodel.payment_plan_path = element1.GetAttributeValue(SmartParametersV2016.href, "");
                    utilityviewmodel.payment_plan_path = utilityviewmodel.payment_plan_path.Replace(utilityviewmodel.prfix_xxx, "");
                    break;
                case "My Energy Usage":
                    utilityviewmodel.my_energy_usage_path = element1.GetAttributeValue(SmartParametersV2016.href, "");
                    utilityviewmodel.my_energy_usage_path = utilityviewmodel.my_energy_usage_path.Replace(utilityviewmodel.prfix_xxx, "");
                    break;
                case "My Balance & Bills":
                    utilityviewmodel.view_my_balance_path = element1.GetAttributeValue(SmartParametersV2016.href, "");
                    utilityviewmodel.view_my_balance_path = utilityviewmodel.view_my_balance_path.Replace(utilityviewmodel.prfix_xxx, "");
                    break;
                default:
                    break;
            }
            return;
        }


        private static bool SP_EditPersonalDetails(UtilityViewModel utilityviewmodel,
                                                    HtmlAgilityPack.HtmlDocument document)
        {
            // Fuck me!  It fucking WORKED!!!! See the re-direction to this 'secure'
            // routine from the initial fucking Login routine above!!!!!!!!!!!!
            // You ARE a fucking genius Ray!!  An absolute bravest of the brave
            // fucking genius.  Those visionless cunts you saw last Thursday have
            // ABSOLUTELY no idea what they are missing out on ....
            // She is FOREVER talking herself and her fucking job 'up'
            // **FOREVER**
            IList<HtmlAgilityPack.HtmlNode>
                    HtmlCol1;
            utilityviewmodel.first_name = "";
            utilityviewmodel.last_name = "";
            utilityviewmodel.middle_name = "";

            // The Fuel Label might contain Dual Fuel or Gas or Electricity
            HtmlCol1 = YetAnotherFuckingHoop.SelectNodesAsList(document.DocumentNode, ".//input");
            foreach (HtmlAgilityPack.HtmlNode element1 in HtmlCol1)
            {
                // The matching criteria ....
                SP_Bits_Pieces(utilityviewmodel,
                                element1);
            }
            utilityviewmodel.account_name = utilityviewmodel.first_name + SmartParametersV2016.space;
            if (utilityviewmodel.middle_name.Length != 0)
            {
                utilityviewmodel.account_name += utilityviewmodel.middle_name + SmartParametersV2016.space;
            }
            utilityviewmodel.account_name += utilityviewmodel.last_name;
            return true;
        }

        private static async Task<bool> SP_Amelia_Stuff(MainViewModel ourviewmodel,
                                             UtilityViewModel utilityviewmodel,
                                             string account_address)
        {
            // And then I had to sit there whilst she bored the pants off janice telling her
            // how marvellous she was and how everybody is going to miss her and how zany and wacky
            // and off the wall she was/is and how she was the CENTRE OF ATTENTION which is what
            // she wants to be ALL THE TIME.
            // "...go trumping round Stockport ..." <= " go TRAMPING round Stockport" you thick bitch
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
                if (string.IsNullOrEmpty(utilityviewmodel.mpan_mprn))
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
                utilityviewmodel.statement_id = "";


                SmartUtilityV2022.Amelia_Update_Postcode_Area(ourviewmodel, utilityviewmodel);
                // No we can set this because we have a good SUPPLY ADDRESS
                utilityviewmodel.login_finished = true;

                // Can do this now we have the area_code
                if (utilityviewmodel.TARIFF_CODE == 0)
                {

                    await SmartUtilityV2022.Lookup_Remote_Tariff_Code(ourviewmodel, utilityviewmodel);

                }

                // And its not PA-CIFIC its fucking SPE-CIFIC you idiot bitch
                // Only the first one considered

            }   // and its FLAHAVANS not FLANNIGANS you thick cow
                // and its the SCHENGEN agreement no the Shanghai agreement you thick dumb-cluck D-A-F!!
            return true;
        }

        private static void SP_Bits_Pieces(UtilityViewModel utilityviewmodel,
                                                HtmlAgilityPack.HtmlNode element1)
        {
            // The matching criteria ....
            if (!string.IsNullOrEmpty(element1.Id))
            {
                switch (element1.Id)
                {
                    case "firstName":
                        utilityviewmodel.first_name = element1.GetAttributeValue("value", "");
                        break;
                    case "lastName":
                        utilityviewmodel.last_name = element1.GetAttributeValue("value", "");
                        break;
                    case "middleName":
                        utilityviewmodel.middle_name = element1.GetAttributeValue("value", "");
                        break;
                    case "dob":
                        utilityviewmodel.account_dob = element1.GetAttributeValue("value", "");
                        break;
                    case "phone":
                        utilityviewmodel.account_phone_no = element1.GetAttributeValue("value", "");
                        break;
                    case "mobile":
                        if (string.IsNullOrEmpty(utilityviewmodel.account_phone_no))
                        {
                            utilityviewmodel.account_phone_no = element1.GetAttributeValue("value", "");
                        }
                        break;
                    case "email":
                        utilityviewmodel.account_email = element1.GetAttributeValue("value", "");
                        break;
                    default:
                        break;
                }
            }
            return;
        }

        private static bool SP_PaymentPlan(UtilityViewModel utilityviewmodel,
                                            HtmlAgilityPack.HtmlDocument document)
        {
            // Fuck me!  It fucking WORKED!!!! See the re-direction to this 'secure'
            // routine from the initial fucking Login routine above!!!!!!!!!!!!
            // You ARE a fucking genius Ray!!  An absolute bravest of the brave
            // fucking genius.  Those visionless cunts you saw last Thursday have
            // ABSOLUTELY no idea what they are missing out on ....
            // She is FOREVER talking herself and her fucking job 'up'
            // **FOREVER**
            IList<HtmlAgilityPack.HtmlNode>
                    HtmlCol1;
            string value, // = "",
                    account_number = "",
                    sort_code = "",
                    payment_day = "";

            bool clicked = false;

            HtmlCol1 = YetAnotherFuckingHoop.SelectNodesAsList(document.DocumentNode, "//input");
            foreach (HtmlAgilityPack.HtmlNode element1 in HtmlCol1)
            {
                if (!string.IsNullOrEmpty(element1.Id))
                {
                    switch (element1.Id)
                    {
                        case "bank_acc_no":
                            value = element1.GetAttributeValue("value", "");
                            account_number = value;
                            break;
                        case "sort_code":
                            value = element1.GetAttributeValue("value", "");
                            sort_code = value;
                            break;
                        case "payment_day":
                            value = element1.GetAttributeValue("value", "");
                            payment_day = value;
                            break;
                        default:
                            break;
                    }
                    if (!string.IsNullOrEmpty(account_number) &&
                        !string.IsNullOrEmpty(sort_code) &&
                        !string.IsNullOrEmpty(payment_day))
                    {
                        if (string.IsNullOrEmpty(utilityviewmodel.bank_account_name))
                        {
                            utilityviewmodel.bank_account_name = utilityviewmodel.account_name; // Bank account name max 35 chars
                        }
                        utilityviewmodel.bank_sort_code = sort_code;                 // Bank account sort code max 6 digits
                        utilityviewmodel.bank_account_number = account_number;       // Bank account number max 8 digits
                        utilityviewmodel.bank_payment_day = Convert.ToInt16(payment_day);

                        clicked = true;
                        break;
                    }
                }
            }
            return clicked;
        }

        private static bool SP_MyEnergyUsage(HtmlAgilityPack.HtmlDocument document,
                                                    UtilityViewModel utilityviewmodel)
        {
            IList<HtmlAgilityPack.HtmlNode>
                    HtmlCol1;

            HtmlCol1 = YetAnotherFuckingHoop.SelectNodesAsList(document.DocumentNode, ".//a");
            foreach (HtmlAgilityPack.HtmlNode element1 in HtmlCol1)
            {
                string href = element1.GetAttributeValue(SmartParametersV2016.href, "");
                if (href.Contains("dashboard"))
                {
                    utilityviewmodel.target_pathname = href.Replace("&amp;", "&");
                    utilityviewmodel.next_routine = "DOWNLOAD";
                    return true; ;
                }
            }
            return false;
        }

        private static async Task<bool> SP_Download(MainViewModel ourviewmodel,
                                                UtilityViewModel utilityviewmodel,
                                                HtmlAgilityPack.HtmlDocument document)
        {
            IList<HtmlAgilityPack.HtmlNode>
                    HtmlCol1,
                    HtmlCol2;

            bool clicked = false;

            HtmlCol1 = YetAnotherFuckingHoop.SelectNodesAsList(document.DocumentNode, "//div");
            foreach (HtmlAgilityPack.HtmlNode element1 in HtmlCol1)
            {
                if (string.IsNullOrEmpty(element1.Id))
                {
                    string classname = element1.GetAttributeValue("class", "");
                    if (SmartNibbyV2016.Check_Classname(classname, "btn-link-orange", false))
                    {
                        HtmlCol2 = YetAnotherFuckingHoop.SelectNodesAsList(element1, ".//a");
                        foreach (HtmlAgilityPack.HtmlNode element2 in HtmlCol2)
                        {
                            string href = element2.GetAttributeValue(SmartParametersV2016.href, "");
                            Uri TargetUrl = new Uri(href);
                            // Get the fucking File

                            string[] task = await SmartBobV2017.HTTPCLIENT_GET_FILE_ASYNC(ourviewmodel,
                                                                                        TargetUrl,
                                                                                        utilityviewmodel.utilityToken,
                                                                                        utilityviewmodel.guid);

                            if (task.Length == 0 ||
                                !string.IsNullOrEmpty(ourviewmodel.errorMessage))
                            {
                                return clicked;     // Still false
                            }
                            else
                            {
                                if (task.ToArray().Length != 0)
                                {
                                    if (await Do_The_Updates(ourviewmodel,
                                                            utilityviewmodel,
                                                            task))

                                    {
                                        clicked = true;
#if WINFORMS
                                        if (utilityviewmodel.console)
                                        {
                                            int usage_count = utilityviewmodel.Hezbollah.e_usage_changesList.Count +
                                                                utilityviewmodel.Hezbollah.g_usage_changesList.Count;
                                            await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Updated usage: " + usage_count.ToString() + Environment.NewLine.ToString());
                                        }
#endif
                                        return true;
                                    }
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
            return clicked;
        }

        private static async Task<bool> Do_The_Updates(MainViewModel ourviewmodel,
                                                        UtilityViewModel utilityviewmodel,
                                                        string[] task)
        {
            // The Scottishpaki readings all come in one big chunk ...

            if (!await Store_Readings(ourviewmodel,
                                    utilityviewmodel,
                                    task.ToArray(),
                                    utilityviewmodel.brand_code,
                                    utilityviewmodel.supplier_code,
                                    utilityviewmodel.bill_currency_separator,
                                    utilityviewmodel.last_usage_index,
                                    utilityviewmodel.last_usage_date,
                                    //SmartParametersV2016.defaultDate,
                                    utilityviewmodel.Hezbollah.e_usage_changesList,
                                    utilityviewmodel.Hezbollah.g_usage_changesList))

            {
                return false;
            }
            return true;
        }

        internal static async Task<bool> Store_Readings(MainViewModel ourviewmodel,
                                                        UtilityViewModel utilityviewmodel,
                                                        string[] reader,
                                                        short brand_code,
                                                        short supplier_code,
                                                        char bill_currency_separator,
                                                        short[] last_index,
                                                        DateTime[] last_usage_datetime,
                                                        List<SmartUtility.EUsage> e_usageList,
                                                        List<SmartUtility.GUsage> g_usageList)
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
                    //entry_date = "",
                    entry_total,// = "",
                    entry_value;// = "";
            int counter = 0;
            int this_value;
            decimal this_total;
            DateTime this_datetime;

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
                if (counter > 0)
                {
                    // Get rid of all " characters which seem to be endemic
                    values = input_line.Replace("\"", "").Split(',');
                    if (values.Length >= 9)
                    {
                        if (string.IsNullOrEmpty(values[0]))
                        {
                            // Give up on empty first value
                            break;
                        }
                        // For Electricity readings, there SHOULD be a Time Which we can convert.
                        // No, there isn't anymore
                        if (utilityviewmodel.account_no == values[0])
                        {
                            string type_of_energy = values[1];
                            // Have we got something we can compare with?
                            if (!string.IsNullOrEmpty(type_of_energy))
                            {
                                //string current_product = values[2];
                                //string payment_method = values[3];
                                string bill_period = values[4];
                                string //BILL_PERIOD_START = "",
                                        BILL_PERIOD_END = "";
                                if (!string.IsNullOrEmpty(bill_period))
                                {
                                    string[] period_dates = bill_period.Split('-');
                                    //BILL_PERIOD_START = period_dates[0].Trim();
                                    BILL_PERIOD_END = period_dates[1].Trim();
                                }
                                //string number_of_days = values[5];
                                string bill_amount = values[6];
                                string consumption_kWh = values[7];
                                //string daily_average = values[8];

                                entry_total = consumption_kWh;
                                entry_value = bill_amount;

                                if (!SmartParseV2016.Generic_Parse_Datetime(BILL_PERIOD_END, utilityviewmodel))
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
                                DateTime last_usage = SmartParametersV2016.basicDate;
                                short last_i = 0;
                                switch (type_of_energy)
                                {
                                    case "Electricity":
                                        last_i = last_index[0];
                                        last_usage = last_usage_datetime[0];
                                        break;
                                    case "Gas":
                                        last_i = last_index[1];
                                        last_usage = last_usage_datetime[1];
                                        break;
                                }
                                if (last_usage != SmartParametersV2016.basicDate)
                                {
                                    if (SmartRoutinesV2018.DateTimeCompare(this_datetime, last_usage) > 0)
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
                                            switch (type_of_energy)
                                            {
                                                case "Electricity":
                                                    last_index[0] = (short)(last_i + 1);
                                                    last_usage_datetime[0] = this_datetime;
                                                    SmartUtility.EUsage e_usage_row = new SmartUtility.EUsage()
                                                    {
                                                        CUBEFACE_CODE = SmartParametersV2016.Utility,
                                                        MPAN_MPRN = utilityviewmodel.sparks.MPAN,
                                                        UNIQUE_INDEX = last_index[0],
                                                        USAGE_DATETIME = this_datetime,
                                                        USAGE_TOTAL = this_total,
                                                        USAGE_VALUE = this_value
                                                    };
                                                    e_usageList.Add(e_usage_row);
                                                    break;
                                                case "Gas":
                                                    last_index[1] = (short)(last_i + 1);
                                                    last_usage_datetime[1] = this_datetime;
                                                    SmartUtility.GUsage g_usage_row = new SmartUtility.GUsage()
                                                    {
                                                        CUBEFACE_CODE = SmartParametersV2016.Utility,
                                                        MPAN_MPRN = utilityviewmodel.smell.MPRN,
                                                        UNIQUE_INDEX = last_index[1],
                                                        USAGE_DATETIME = this_datetime,
                                                        USAGE_TOTAL = this_total,
                                                        USAGE_VALUE = this_value
                                                    };
                                                    g_usageList.Add(g_usage_row);
                                                    break;
                                                default:
                                                    break;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                counter++;
            }
            return true;
        }

        private static async Task<bool> SP_ViewMyBalance(
#if ANDROIDX
                                                AppCompatActivity meterActivity,
#endif
                                                MainViewModel ourviewmodel,
                                                 UtilityViewModel utilityviewmodel,
                                                 HtmlAgilityPack.HtmlDocument document,
                                                bool TextBox_Active,
                                                //List<SmartUtility.Payments> payments_tempList,
                                                string accountoveriew_pathname)
        {
            IList<HtmlAgilityPack.HtmlNode>
                    HtmlCol1,
                    HtmlCol2,
                    HtmlCol6;

            utilityviewmodel.href = "";
            int column_index;

            HtmlCol1 = YetAnotherFuckingHoop.SelectNodesAsList(document.DocumentNode, "//table");
            foreach (HtmlAgilityPack.HtmlNode element1 in HtmlCol1)
            {
                // The matching criteria ....
                if (string.IsNullOrEmpty(element1.Id))
                {
                    string classname = element1.GetAttributeValue("class", "");

                    if (classname == "bills")
                    {
                        HtmlCol6 = YetAnotherFuckingHoop.SelectNodesAsList(element1, "tbody");
                        foreach (HtmlAgilityPack.HtmlNode element6 in HtmlCol6)
                        {
                            HtmlCol2 = YetAnotherFuckingHoop.SelectNodesAsList(element6, "tr");
                            foreach (HtmlAgilityPack.HtmlNode element2 in HtmlCol2)
                            {
                                column_index = 0;
                                utilityviewmodel.href = "";

                                SP_Reduce(ourviewmodel,
                                        utilityviewmodel,
                                        element2,
                                        column_index);

                                if (!string.IsNullOrEmpty(utilityviewmodel.href))
                                {
                                    await SP_Read_Bills(
#if ANDROIDX
                                                        meterActivity,
#endif
                                                        ourviewmodel,
                                                         utilityviewmodel,
                                                         utilityviewmodel.href,
                                                        TextBox_Active);
                                }
                            }
                        }
                    }
                }
            }

            // What has happened is that these SweatyPower paki dickheads have not
            // coded in the "Previous" and "Next" buttons properly so you cannot
            // travel up and down the history - one of the reasons that you cannot
            // see any archived bills.   We will have to call it quits HERE I'm afraid
            utilityviewmodel.target_pathname = accountoveriew_pathname;
            utilityviewmodel.next_routine = "MYACCOUNTOVERVIEW";
            return true;
        }

        private static async Task<bool> SP_Read_Bills(
#if ANDROIDX
                                                        AppCompatActivity meterActivity,
#endif
                                                        MainViewModel ourviewmodel,
                                                        UtilityViewModel utilityviewmodel,
                                                        string href,
                                                        bool TextBox_Active)
        {
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
                    Uri TargetUrl = new Uri(href);
                    // Start with a clean sheet ...
                    ourviewmodel.pdfMessage = "";

#if WINFORMS || WPF  || WINUI || SMARTMAUI
                    iText.Kernel.Pdf.PdfReader pdfreader =
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
                        if (!await SP_parse_bill(ourviewmodel,
                                                 utilityviewmodel,// Statement Id is in utilityviewmodel.statement_id
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

        private static bool SP_Reduce(MainViewModel ourviewmodel,
                                        UtilityViewModel utilityviewmodel,
                                        HtmlAgilityPack.HtmlNode element2,
                                        int column_index)
        {
            IList<HtmlAgilityPack.HtmlNode>
                    HtmlCol3,
                    HtmlCol4;

            DateTime payment_date = SmartParametersV2016.defaultDate;
            string PAYMENT_METHOD,// = "",
                    PAYMENT_AMOUNT;

            string TOTAL_CHARGES, // = "0",
                    webpage_id = SmartParametersV2016.defaultDates;

            HtmlCol3 = YetAnotherFuckingHoop.SelectNodesAsList(element2, "td");
            foreach (HtmlAgilityPack.HtmlNode element3 in HtmlCol3)
            {
                string inner_text = element3.InnerText;
                if (!string.IsNullOrEmpty(inner_text))
                {
                    inner_text = inner_text.Replace("\r", "");
                    inner_text = inner_text.Replace("\n", "");
                    inner_text = inner_text.Replace("\t", "");
                }
                inner_text = inner_text.Trim();
                switch (column_index)
                {
                    case 0:     // Date
                        if (!string.IsNullOrEmpty(inner_text))
                        {
                            webpage_id = inner_text;
                        }
                        else
                        {
                            webpage_id = "";
                            column_index = 6;   // No date so don't do any more columns
                        }

                        if (!SmartParseV2016.Generic_Parse_Datetime(webpage_id, utilityviewmodel))
                        {
                            return false;
                        }
                        else
                        {
                            payment_date = utilityviewmodel.bill_date = utilityviewmodel.genericTargetDate;
                        }
                        break;
                    case 1:     // Bills
                        if (!string.IsNullOrEmpty(inner_text))
                        {
                            TOTAL_CHARGES = inner_text.Replace("(Estimated)", "").Trim();
                            TOTAL_CHARGES = TOTAL_CHARGES.Replace(utilityviewmodel.bill_currency_symbol, "");
                            TOTAL_CHARGES = TOTAL_CHARGES.Replace(utilityviewmodel.bill_currency_separator.ToString(), "");
                            TOTAL_CHARGES = TOTAL_CHARGES.Replace(utilityviewmodel.bill_thousands_separator.ToString(), "");
                        }
                        else
                        {
                            TOTAL_CHARGES = "0";
                        }
                        break;
                    case 2:     // Payments
                        if (!string.IsNullOrEmpty(inner_text))
                        {
                            PAYMENT_AMOUNT = inner_text.Replace(utilityviewmodel.bill_currency_symbol, "").Trim();
                            PAYMENT_AMOUNT = PAYMENT_AMOUNT.Replace("(Credit)", "");
                            //int debit_index = PAYMENT_AMOUNT.Contains("(Debit)");
                            if (PAYMENT_AMOUNT.Contains("(Debit)"))
                            {
                                PAYMENT_AMOUNT = PAYMENT_AMOUNT.Replace("(Debit)", "");
                                PAYMENT_AMOUNT = SmartParametersV2016.minus + PAYMENT_AMOUNT;
                            }
                            PAYMENT_AMOUNT = PAYMENT_AMOUNT.Replace(utilityviewmodel.bill_currency_separator.ToString(), "").Trim();
                            PAYMENT_AMOUNT = PAYMENT_AMOUNT.Replace(utilityviewmodel.bill_thousands_separator.ToString(), "").Trim();
                        }
                        else
                        {
                            PAYMENT_AMOUNT = "0";
                        }
                        if (!SmartParseV2016.Generic_Parse_Integer(PAYMENT_AMOUNT, utilityviewmodel))
                        {
                            return false;
                        }
                        PAYMENT_METHOD = "Payment";
                        if (!SmartSpikeUtilityV2017.Utility_Lookup_PaymentCode(ourviewmodel,
                                                                        utilityviewmodel,
                                                                        PAYMENT_METHOD))
                        {
                            return false;
                        }
                        PAYMENT_METHOD = utilityviewmodel.PAYMENT_METHOD;
                        break;
                    case 3:     // Charges
                        if (!string.IsNullOrEmpty(inner_text))
                        {
                            TOTAL_CHARGES = inner_text.Replace(utilityviewmodel.bill_currency_symbol, "").Trim();
                            TOTAL_CHARGES = TOTAL_CHARGES.Replace(utilityviewmodel.bill_currency_separator.ToString(), "");
                            TOTAL_CHARGES = TOTAL_CHARGES.Replace(utilityviewmodel.bill_thousands_separator.ToString(), "");
                        }
                        else
                        {
                            TOTAL_CHARGES = "0";
                        }
                        break;
                    case 4:     // Balance
                        if (!SP_Payments(ourviewmodel,
                                        utilityviewmodel,
                                        inner_text,
                                        payment_date))
                        {
                            return false;
                        }
                        break;
                    case 5: // Bill link
                        HtmlCol4 = YetAnotherFuckingHoop.SelectNodesAsList(element3, ".//a");
                        foreach (HtmlAgilityPack.HtmlNode element4 in HtmlCol4)
                        {
                            utilityviewmodel.href = element4.GetAttributeValue(SmartParametersV2016.href, "");
                            if (!string.IsNullOrEmpty(utilityviewmodel.href))
                            {
                                if (!string.IsNullOrEmpty(element4.InnerText))
                                {
                                    // Have we done this bill (by its date) before??
                                    if (!string.IsNullOrEmpty(webpage_id))
                                    {
                                        utilityviewmodel.statement_id = webpage_id;
                                    }
                                }
                            }
                        }
                        break;
                    default:
                        break;
                }
                column_index++;
            }
            return true;
        }

        private static bool SP_Payments(MainViewModel ourviewmodel,
                                        UtilityViewModel utilityviewmodel,
                                        string inner_text,
                                        DateTime payment_date)
        {
            string PAYMENT_BALANCE;
            int payment_amount = 0,
                    payment_balance;
            short payment_item = 0,
                    payment_code = 0;

            if (!string.IsNullOrEmpty(inner_text))
            {
                // Christian Scientist is NOT A fucking Scientologist you dick-head
                // FUCKING JOB AGAIN
                PAYMENT_BALANCE = inner_text.Replace(utilityviewmodel.bill_currency_symbol, "").Trim();
                PAYMENT_BALANCE = PAYMENT_BALANCE.Replace("in Credit", "");
                //int debit_index = PAYMENT_BALANCE.Contains("in Debit");
                if (PAYMENT_BALANCE.Contains("in Debit"))
                {
                    PAYMENT_BALANCE = PAYMENT_BALANCE.Replace("in Debit", "");
                    PAYMENT_BALANCE = SmartParametersV2016.minus + PAYMENT_BALANCE;
                }
                PAYMENT_BALANCE = PAYMENT_BALANCE.Replace(utilityviewmodel.bill_currency_separator.ToString(), "").Trim();
                PAYMENT_BALANCE = PAYMENT_BALANCE.Replace(utilityviewmodel.bill_thousands_separator.ToString(), "").Trim();
            }
            else
            {
                PAYMENT_BALANCE = "0";
            }
            if (!SmartParseV2016.Generic_Parse_Integer(PAYMENT_BALANCE, utilityviewmodel))
            {
                return false;
            }
            else
            {
                payment_balance = utilityviewmodel.genericTransactionValue;
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
                    // I couldn't give a shit WHAT she fucking does
                    // AND ... I have ABSOLUTELY NO FUCKING IDEA who your 'cousin' Ann(e?) is, was or does
                }
            }
            return true;
        }

        internal static async Task<bool> SP_parse_bill(MainViewModel ourviewmodel,
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

                string[][] sections = {
                                              new string[] { "A", "0", "", "", "0", "0", "" },
                                              new string[] { "B", "0", "", "Your gas and electricity statement" + "|" +
                                                                "Your adjusted gas and electricity statement" + "|" +
                                                                "Your gas statement" + "|" +
                                                                "Your electricity statement", "-1", "-1", "" },
                                              new string[] { "B", "1", "", "Last period", "-1", "-1", "" },
                                              new string[] { "B", "2", "", "This period", "-1", "-1", "" },
                                              new string[] { "B", "3", "", "Next steps", "-1", "-1", "" },
                                              new string[] { "C", "0", "", "Could you pay less?" + "|" +
                                                                            "Could you find a cheaper deal?" +"|" +
                                                                            "Could you pay less on a different ScottishPower tariff?", "-1", "-1", "" },
                                              new string[] { "D", "0", "", "About your tariff", "-1", "-1", "" },
                                              new string[] { "E", "0", "", "Important information about your energy use", "-1", "-1", "" },
                                              new string[] { "F", "0", "G", "Converting gas units to kWh" +"|" +
                                                                            "Calculating your gas charge", "-1", "-1", "" },
                                              new string[] { "G", "0", "", "Supply problems", "-1", "-1", "" },
                                              new string[] { "H", "0", "", "Energy charges this period" + "|" +
                                                                            "How your energy adds up", "-1", "-1", "" },
                                              new string[] { "H", "1", "E", "Electricity" + "|" +
                                                                            "Electricity costs", "-1", "-1", SmartParametersV2016.sectionsExactMatch },
                                              new string[] { "H", "2", "G", "Gas" + "|" +
                                                                            "Gas costs", "-1", "-1", SmartParametersV2016.sectionsExactMatch },
                                              new string[] { "I", "0", "", "Total energy charges this period" +"|" +
                                                                            "Summary", "-1", "-1", "" },
                                              new string[] { "J", "0", "", "Payments received" + "|" +
                                                                            "What you've paid", "-1", "-1", SmartParametersV2016.sectionsExactMatch + SmartParametersV2016.sectionsCanOverride},
                                              new string[] { "K", "0", "", "Discounts", "-1", "-1", SmartParametersV2016.sectionsExactMatch },
                                              new string[] { "L", "0", "", "Other charges" + "|" +
                                                                            "Account adjustments" + "|" +
                                                                            "Other credits and debits", "-1", "-1", "" },
                                              new string[] { "M", "0", "", "VAT", "-1", "-1", ""},
                                             };
                // This is always needed anyway because we always do Page1 with this
                // Sometimes ... the Bill Date is never extracted from the PDF text!
                // But when (if) we ever find it - we substitute the Bill Number for it
                // IF we haven't found the Bill number ...


                bool success = await Parse_Page0(ourviewmodel,
                                             utilityviewmodel,
                                             sections,
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
                                            sections,
                                            lines))
                    {
                        status = false;
                    }
                }
            }
            if (status)
            {
                SmartParseV2016.Update_Tariff_Details(ourviewmodel, utilityviewmodel);
                // fucking bitch is talking about her fucking job AGAIN
                // i don't fucking care about fucking verity or fucking aileish
                SmartParseV2016.Check_Vat_For_Pennies_Difference(ourviewmodel, utilityviewmodel);
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

            // Very Important Routine!!!    SO WHY IS IT COMMENTED OUT?????????????
            SmartParseV2016.Sort_Sections(true, utilityviewmodel, lines);

            // Pass 1 - look for the Big Three in Section 0
            if (!Parse_SectionA0(ourviewmodel, utilityviewmodel, sections, "A", "0", lines))
            {
                ourviewmodel.errorMessage = "A0" + SmartParametersV2016.bar + ourviewmodel.errorMessage;
                return status;
            }

            if (!await Parse_SectionB0(ourviewmodel, utilityviewmodel, sections, "B", "0", lines))

            {
                ourviewmodel.errorMessage = "B0" + SmartParametersV2016.bar + ourviewmodel.errorMessage;
                return false;
            }
            return status;
        }

        // Haddock!!   < Ad-hoc !!!!
        // Aminated    < Animated
        // 27-Jan-2016  My ignorant, rude, petulant, insufferable bitch of a 'wife'
        // got her arese well and truly out last night stomping and thumping around the bedroom
        // in her usual thick, stupid, obnoxious way.  ODSBD and I wont have to put up with
        // her tantrums anymore ... ODSBD ... I can't wait ...

        private static async Task<bool> Parse_Pages(MainViewModel ourviewmodel,
                                        UtilityViewModel utilityviewmodel,
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
            if (!Parse_SectionB3(ourviewmodel, utilityviewmodel, sections, "B", "3", lines))
            {
                ourviewmodel.errorMessage = "B3" + SmartParametersV2016.bar + ourviewmodel.errorMessage;
                return false;
            }
            if (!Parse_SectionC0(utilityviewmodel, sections, "C", "0"))
            {
                ourviewmodel.errorMessage = "C0" + SmartParametersV2016.bar + ourviewmodel.errorMessage;
                return false;
            }

            if (!await Parse_SectionD0(ourviewmodel, utilityviewmodel, sections, "D", "0", lines))
            {
                ourviewmodel.errorMessage = "D0" + SmartParametersV2016.bar + ourviewmodel.errorMessage;
                return false;
            }
            if (!Parse_SectionE0(utilityviewmodel, sections, "E", "0", lines))
            {
                ourviewmodel.errorMessage = "E0" + SmartParametersV2016.bar + ourviewmodel.errorMessage;
                return false;
            }
            if (!Parse_SectionF0(utilityviewmodel, sections, "F", "0", lines))
            {
                ourviewmodel.errorMessage = "F0" + SmartParametersV2016.bar + ourviewmodel.errorMessage;
                return false;
            }
            if (!Parse_SectionG0(utilityviewmodel, sections, "G", "0"))
            {
                ourviewmodel.errorMessage = "G0" + SmartParametersV2016.bar + ourviewmodel.errorMessage;
                return false;
            }
            if (!Parse_SectionM0(ourviewmodel, utilityviewmodel, sections, "M", "0", lines))
            {
                ourviewmodel.errorMessage = "M0" + SmartParametersV2016.bar + ourviewmodel.errorMessage;
                return false;
            }
            if (!Parse_SectionH1(ourviewmodel, utilityviewmodel, sections, "H", "1", lines))
            {
                ourviewmodel.errorMessage = "H1" + SmartParametersV2016.bar + ourviewmodel.errorMessage;
                return false;
            }
            if (!Parse_SectionH2(ourviewmodel, utilityviewmodel, sections, "H", "2", lines))// , CALORIFIC_VALUE))
            {
                ourviewmodel.errorMessage = "H2" + SmartParametersV2016.bar + ourviewmodel.errorMessage;
                return false;
                // Fucking silly cow cannot add 3 to 2014!!
            }
            if (!Parse_SectionJ0(ourviewmodel, utilityviewmodel, sections, "J", "0", lines))
            {
                ourviewmodel.errorMessage = "J0" + SmartParametersV2016.bar + ourviewmodel.errorMessage;
                return false;
            }
            if (!Parse_SectionK0(ourviewmodel, utilityviewmodel, sections, "K", "0", lines))
            {
                ourviewmodel.errorMessage = "K0" + SmartParametersV2016.bar + ourviewmodel.errorMessage;
                return false;
            }
            if (!Parse_SectionL0(ourviewmodel, utilityviewmodel, sections, "L", "0", lines))
            {
                ourviewmodel.errorMessage = "L0" + SmartParametersV2016.bar + ourviewmodel.errorMessage;
                return false;
                // This stupid bitch is talking herself up as usual.  She is SUCH a fucking dick-head.
                // ALWAYS and FOREVER blowing her own trumpet.  Look ar me!  Lok at ME!! Look how
                // funny and ditzy and wacky and clever I am!!!   
                // No, look at this stupid boring fucking idiot ...
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
            int section_index = 0;
            if (!SmartParseV2016.Decode_Section(section, sub_section, sections, utilityviewmodel.resource_code.ToString(), utilityviewmodel))
            {
                return true;
            }

            utilityviewmodel.token = "";
            string your_account_number = "Account number",
                    Date = "Date ",
                    statement_date = "Statement date:",
                    Supply_Address = "Supply Address:",
                    Supply_address = "Supply address:";
            bool RESOURCE_BALANCES = false; // Default
            char PAYMENT_PLAN = SmartParametersV2016.defaultChar;
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
            bool status = true;
            bool postcode_found = false;

            utilityviewmodel.BILL_PERIOD_START = SmartParametersV2016.defaultDates;
            utilityviewmodel.BILL_PERIOD_END = SmartParametersV2016.defaultDates;

            //DateTime temp_date;// = SmartParametersV2016.defaultDate;

            // Pass 1 - look for the Big Three
            //int line_count;// = 0;
            for (int line_count = Convert.ToInt32(sections[section_index][4]); line_count <= Convert.ToInt32(sections[section_index][5]); line_count++)
            {
                utilityviewmodel.token = lines[line_count].Trim();

                if (SmartParseV2016.Token_Identify(utilityviewmodel, your_account_number, false))
                {
                    if (string.IsNullOrEmpty(utilityviewmodel.ACCOUNT_NO))
                    {
                        string temp_account = SmartParseV2016.Find_Account(utilityviewmodel, line_count, lines);
                        if (!string.IsNullOrEmpty(temp_account))
                        {
                            utilityviewmodel.ACCOUNT_NO = temp_account.Trim();
                        }
                    }
                    if (string.IsNullOrEmpty(utilityviewmodel.account_no) &&
                        !string.IsNullOrEmpty(utilityviewmodel.ACCOUNT_NO))
                    {
                        utilityviewmodel.account_no = utilityviewmodel.ACCOUNT_NO;
                    }
                    goto the_old_daysA0;
                }

                if (SmartParseV2016.Token_Identify(utilityviewmodel, Date + SmartParametersV2016.bar +
                                                                statement_date, true))
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
                    goto the_old_daysA0;
                }

                if (SmartParseV2016.Token_Identify(utilityviewmodel, Supply_Address +
                                                                SmartParametersV2016.bar +
                                                                Supply_address, true))
                {
                    string account_postcode = utilityviewmodel.token;
                    if (!postcode_found)
                    {
                        if (SmartNibbyV2016.Derive_Postcode_New(account_postcode,
                                                    ourviewmodel.Blanche.workingPostcodesList,
                                                    utilityviewmodel))
                        {
                            postcode_found = true;
                        }
                    }
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

        private static async Task<bool> Parse_SectionB0(MainViewModel ourviewmodel,
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

            string account_number = "Account number",
                            account_number_colon = "Account number:",
                            Your_gas_and_electricity_statement_for = "Your gas and electricity statement for:",
                            Your_adjusted_gas_and_electricity_statement_for = "Your adjusted gas and electricity statement for:",
                            Your_gas_statement_for = "Your gas statement for:",
                            Your_electricity_statement_for = "Your electricity statement for:",
                            statement_period = "Statement period",
                            Your_current_tariff = "Your current tariff:",
                            your_tariff = "This is based on our",
                            you_are_on_our = "You are on our",
                            tariff = "tariff",
                            product = "product",
                            your_next_payment_will_be_collected_on = "Your next payment will be collected on";
            //DateTime temp_date = SmartParametersV2016.defaultDate;

            string PREVIOUS_BALANCE = "0",
                    OUTSTANDING_BALANCE;
            string starting_balance = "Starting balance",
                    your_account_balance = "Your account balance";

            for (int line_count = Convert.ToInt32(sections[section_index][4]); line_count <= Convert.ToInt32(sections[section_index][5]); line_count++)
            {
                utilityviewmodel.token = lines[line_count].Trim();

                if (SmartParseV2016.Token_Identify(utilityviewmodel, account_number +
                                                                SmartParametersV2016.bar +
                                                                account_number_colon, false))
                {
                    if (string.IsNullOrEmpty(utilityviewmodel.ACCOUNT_NO))
                    {
                        string temp_account = SmartParseV2016.Find_Account(utilityviewmodel, line_count, lines);
                        if (!string.IsNullOrEmpty(temp_account))
                        {
                            utilityviewmodel.ACCOUNT_NO = temp_account.Trim();
                            SmartParseV2016.Update_Bill(ourviewmodel, utilityviewmodel, "ACCOUNT_NO", utilityviewmodel.ACCOUNT_NO);
                        }
                    }
                    if (string.IsNullOrEmpty(utilityviewmodel.account_no) &&
                        !string.IsNullOrEmpty(utilityviewmodel.ACCOUNT_NO))
                    {
                        utilityviewmodel.account_no = utilityviewmodel.ACCOUNT_NO;
                    }
                    goto the_old_daysB0;
                }
                if (SmartParseV2016.Token_Identify(utilityviewmodel, statement_period +
                                                                SmartParametersV2016.bar +
                                                                Your_gas_and_electricity_statement_for +
                                                                SmartParametersV2016.bar +
                                                                Your_adjusted_gas_and_electricity_statement_for +
                                                                SmartParametersV2016.bar +
                                                                Your_gas_statement_for +
                                                                SmartParametersV2016.bar +
                                                                Your_electricity_statement_for, true))
                {
                    utilityviewmodel.token = utilityviewmodel.token.Replace(SmartParametersV2016.dash, SmartParametersV2016.bar.ToString());
                    string[] components = utilityviewmodel.token.Split(SmartParametersV2016.bar);
                    int component_count = 0;
                    while (component_count < components.Length)
                    {
                        switch (component_count)
                        {
                            case 0:     // Bill Period Start
                                utilityviewmodel.BILL_PERIOD_START = components[component_count].Trim();
                                if (!SmartParseV2016.Generic_Parse_Datetime(utilityviewmodel.BILL_PERIOD_START, utilityviewmodel))
                                {
                                    return false;
                                }
                                //else
                                //{
                                //    temp_date = utilityviewmodel.genericTargetDate;
                                //}
                                SmartParseV2016.Update_Bill(ourviewmodel, utilityviewmodel, "BILL_PERIOD_START", utilityviewmodel.BILL_PERIOD_START);
                                break;
                            case 1:     // Bill Period End
                                utilityviewmodel.BILL_PERIOD_END = components[component_count].Trim();
                                if (!SmartParseV2016.Generic_Parse_Datetime(utilityviewmodel.BILL_PERIOD_END, utilityviewmodel))
                                {
                                    return false;
                                }
                                //else
                                //{
                                //    temp_date = utilityviewmodel.genericTargetDate;
                                //}
                                SmartParseV2016.Update_Bill(ourviewmodel, utilityviewmodel, "BILL_PERIOD_END", utilityviewmodel.BILL_PERIOD_END);
                                SmartParseV2016.Update_Bill(ourviewmodel, utilityviewmodel, "DISCOUNT_CREDIT_DATE", utilityviewmodel.BILL_PERIOD_END);
                                break;
                            default:
                                break;
                        }
                        component_count++;
                    }
                    goto the_old_daysB0;
                }

                // These SP Foreigners FUCK ABOUT with this text all the fucking time
                if (SmartParseV2016.Token_Identify(utilityviewmodel, your_tariff +
                                                                SmartParametersV2016.bar +
                                                                you_are_on_our +
                                                                SmartParametersV2016.bar +
                                                                Your_current_tariff, true))
                {
                    utilityviewmodel.token = utilityviewmodel.token.Replace(tariff, "");
                    utilityviewmodel.token = utilityviewmodel.token.Replace(product, "");
                    int parenth_index = utilityviewmodel.token.IndexOf(SmartParametersV2016.leftParenthesis);
                    if (parenth_index >= 0)
                    {
                        utilityviewmodel.token = utilityviewmodel.token.Substring(0, parenth_index).Trim();
                    }
                    utilityviewmodel.token = utilityviewmodel.token.Replace(SmartParametersV2016.dash, SmartParametersV2016.bar.ToString());
                    string[] components = utilityviewmodel.token.Split(SmartParametersV2016.bar);
                    if (components.Length > 0)
                    {
                        utilityviewmodel.TARIFF_NAME = components[0].Trim();

                        if (await SmartUtilityV2022.Lookup_Remote_Tariff_Code(ourviewmodel, utilityviewmodel))

                        {
                            SmartParseV2016.Update_Bill(ourviewmodel, utilityviewmodel, "TARIFF_CODE", utilityviewmodel.TARIFF_CODE.ToString());
                        }
                        else
                        {
                            return false; // ourviewmodel.errorMessage should be set here
                        }
                    }
                    if (components.Length > 1)
                    {
                        string PAYMENT_TYPE = components[1].Trim();
                        // Find a Payment Plan which contains this Payment Name
                        if (SmartSpikeUtilityV2017.Utility_Lookup_PaymentPlan(ourviewmodel,
                                                                    utilityviewmodel,
                                                                    PAYMENT_TYPE))
                        {
                            SmartParseV2016.Update_Bill(ourviewmodel, utilityviewmodel, "PAYMENT_PLAN", utilityviewmodel.PAYMENT_PLAN.ToString());
                            SmartParseV2016.Update_Bill(ourviewmodel, utilityviewmodel, "PAYMENT_TYPE", PAYMENT_TYPE);
                        }
                        else
                        {
                            return false;
                        }
                    }
                    goto the_old_daysB0;
                }

                if (SmartParseV2016.Token_Identify(utilityviewmodel, your_account_balance, true))
                {
                    if (line_count + 1 <= Convert.ToInt32(sections[section_index][5]))
                    {
                        if (lines[line_count + 1].Contains(utilityviewmodel.bill_currency_symbol))
                        {
                            line_count++;
                            utilityviewmodel.token = lines[line_count].Trim();

                            //int temp = 0;
                            OUTSTANDING_BALANCE = utilityviewmodel.token;
                            if (!SmartParseV2016.Currency_Strip(utilityviewmodel.bill_currency_symbol, utilityviewmodel.bill_currency_separator, utilityviewmodel.bill_thousands_separator, OUTSTANDING_BALANCE, utilityviewmodel))
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
                            goto the_old_daysB0;
                        }
                    }
                }

                if (SmartParseV2016.Token_Identify(utilityviewmodel, starting_balance, false))
                {
                    bool found_it = false;
                    while (!found_it)
                    {
                        int len = utilityviewmodel.token.Length;
                        if (len > 0)
                        {
                            if (SmartParseV2016.Token_Identify_Middle(utilityviewmodel, "(in credit)" +
                                                                                    SmartParametersV2016.bar +
                                                                                    "(credit)", true, "cr") ||
                                SmartParseV2016.Token_Identify_Middle(utilityviewmodel, "(in debit)" +
                                                                                    SmartParametersV2016.bar +
                                                                                    "(debit)", true, "dr"))
                            {

                                if (utilityviewmodel.token.Contains(utilityviewmodel.bill_currency_symbol))
                                {
                                    //found_it = true;
                                    break;
                                }
                            }
                            line_count++;
                            if (line_count > Convert.ToInt32(sections[section_index][5]))
                            {
                                goto the_old_daysB0;
                            }
                            utilityviewmodel.token = utilityviewmodel.token + SmartParametersV2016.space + lines[line_count].Trim();
                        }
                    }

                    // Work backwards
                    if (SmartParseV2016.Look_Backwards(utilityviewmodel, SmartParametersV2016.space, SmartParametersV2016.bar.ToString()))
                    {
                        if (SmartParseV2016.Look_Backwards(utilityviewmodel, SmartParametersV2016.space, SmartParametersV2016.bar.ToString()))
                        {
                            string[] components = utilityviewmodel.token.Split(SmartParametersV2016.bar);
                            int components_count = 0;
                            while (components_count < components.Length)
                            {
                                switch (components_count)
                                {
                                    case 0:
                                        break;

                                    case 1:
                                        PREVIOUS_BALANCE = components[components_count].Trim();
                                        break;
                                    case 2:
                                        //int temp = 0;
                                        PREVIOUS_BALANCE += components[components_count].Trim();
                                        if (!SmartParseV2016.Currency_Strip(utilityviewmodel.bill_currency_symbol, utilityviewmodel.bill_currency_separator, utilityviewmodel.bill_thousands_separator, PREVIOUS_BALANCE, utilityviewmodel))
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
                                        //else
                                        //{
                                        //    temp = utilityviewmodel.genericTransactionValue;
                                        //}
                                        SmartParseV2016.Update_Bill(ourviewmodel, utilityviewmodel, "PREVIOUS_BALANCE", PREVIOUS_BALANCE);
                                        break;

                                    default:
                                        break;
                                }
                                components_count++;
                            }
                        }
                    }
                }

                if (SmartParseV2016.Token_Identify(utilityviewmodel, your_next_payment_will_be_collected_on, false))
                {
                    Section_B3_Due_Date(ourviewmodel,
                                            utilityviewmodel,
                                            sections,
                                            //section,
                                            section_index,
                                            lines,
                                            line_count);

                }
            the_old_daysB0:
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
            int section_index = 0;
            if (!SmartParseV2016.Decode_Section(section, sub_section, sections, utilityviewmodel.resource_code.ToString(), utilityviewmodel))
            {
                return true;
            }

            // Now I have to listen to that dickhead fucking about with the fucking phone
            utilityviewmodel.token = "";
            string PAYMENTS_RECEIVED,
                    PREVIOUS_BALANCE,
                    the_balance = "Balance of your last",
                    payments = "Payments received";
            //int temp = 0;

            for (int line_count = Convert.ToInt32(sections[section_index][4]); line_count <= Convert.ToInt32(sections[section_index][5]); line_count++)
            {
                utilityviewmodel.token = lines[line_count].Trim();
                if (SmartParseV2016.Token_Identify(utilityviewmodel, the_balance, true))
                {
                    utilityviewmodel.token = "";
                    // Fucking mixer on a-fucking gain...
                    // is it on THIS line?
                    int sub_line_count = line_count + 1;
                    while (sub_line_count < Convert.ToInt32(sections[section_index][5]))
                    {
                        utilityviewmodel.token = lines[sub_line_count];
                        if (utilityviewmodel.token.Contains(utilityviewmodel.bill_currency_symbol))
                        {
                            string[] components = utilityviewmodel.token.Split(SmartParametersV2016.spaceSplit);
                            if (sub_line_count + 1 < Convert.ToInt32(sections[section_index][5]))
                            {
                                if ((lines[sub_line_count + 1] == "(in debit)") ||
                                    (lines[sub_line_count + 1] == "(in credit"))
                                {
                                    components[0] = components[0] + SmartParametersV2016.space + lines[sub_line_count + 1].Replace("(in ", "");
                                    components[0] = components[0].Replace(")", "");
                                }
                            }
                            utilityviewmodel.token = components[0];
                            break;
                        }
                        sub_line_count++;
                    }
                    line_count = sub_line_count;

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
                        //else
                        //{
                        //    temp = utilityviewmodel.genericTransactionValue;
                        //}
                        SmartParseV2016.Update_Bill(ourviewmodel, utilityviewmodel, "PREVIOUS_BALANCE", PREVIOUS_BALANCE);
                        goto the_old_daysB1;
                    }
                }
                if (SmartParseV2016.Token_Identify(utilityviewmodel, payments, true))
                {
                    utilityviewmodel.token = "";
                    // Fucking mixer on a-fucking gain...
                    // is it on THIS line?
                    int sub_line_count = line_count + 1;
                    while (sub_line_count < Convert.ToInt32(sections[section_index][5]))
                    {
                        utilityviewmodel.token = lines[sub_line_count];
                        if (utilityviewmodel.token.Contains(utilityviewmodel.bill_currency_symbol))
                        {
                            break;
                        }
                        sub_line_count++;
                    }
                    line_count = sub_line_count;
                    if (!string.IsNullOrEmpty(utilityviewmodel.token))
                    {
                        int currency_index = utilityviewmodel.token.IndexOf(utilityviewmodel.bill_currency_symbol);
                        if (currency_index >= 0)
                        {
                            int minus_index = utilityviewmodel.token.IndexOf(SmartParametersV2016.minus);
                            if (minus_index >= 0)
                            {
                                utilityviewmodel.token = utilityviewmodel.token.Substring(minus_index);
                                utilityviewmodel.token = utilityviewmodel.token.Replace(SmartParametersV2016.space, "");
                            }
                            else
                            {
                                utilityviewmodel.token = utilityviewmodel.token.Substring(currency_index);
                            }
                        }
                        if (!SmartParseV2016.Currency_Strip(utilityviewmodel.bill_currency_symbol, utilityviewmodel.bill_currency_separator, utilityviewmodel.bill_thousands_separator, utilityviewmodel.token, utilityviewmodel))
                        {
                            return false;
                        }
                        else
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
                    }
                    goto the_old_daysB1;
                }
            the_old_daysB1:
                continue;
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
            int section_index = 0;
            if (!SmartParseV2016.Decode_Section(section, sub_section, sections, utilityviewmodel.resource_code.ToString(), utilityviewmodel))
            {
                return true;
            }

            utilityviewmodel.token = "";
            string OUTSTANDING_BALANCE = "0";
            //int TEMP = 0;
            string your_new = "Your new";

            for (int line_count = Convert.ToInt32(sections[section_index][4]); line_count <= Convert.ToInt32(sections[section_index][5]); line_count++)
            {
                utilityviewmodel.token = lines[line_count].Trim();

                if (SmartParseV2016.Token_Identify(utilityviewmodel, your_new, false))
                {
                    bool found_it = false;
                    while (!found_it)
                    {
                        int len = utilityviewmodel.token.Length;
                        if (len > 0)
                        {
                            int incredit_index = utilityviewmodel.token.IndexOf("(in credit)");
                            int indebit_index = utilityviewmodel.token.IndexOf("(in debit)");
                            if (incredit_index >= 0)
                            {
                                utilityviewmodel.token = utilityviewmodel.token.Replace("(in credit)", "cr").Trim();
                            }
                            if (indebit_index >= 0)
                            {
                                utilityviewmodel.token = utilityviewmodel.token.Replace("(in debit)", "dr").Trim();
                            }
                            if (incredit_index >= 0 ||
                                indebit_index >= 0)
                            {
                                if (utilityviewmodel.token.Contains(utilityviewmodel.bill_currency_symbol))
                                {
                                    //found_it = true;
                                    break;
                                }
                            }
                            line_count++;
                            if (line_count > Convert.ToInt32(sections[section_index][5]))
                            {
                                goto the_old_daysB2;
                            }
                            utilityviewmodel.token = utilityviewmodel.token + SmartParametersV2016.space + lines[line_count].Trim();
                        }
                    }

                    // Work backwards
                    if (SmartParseV2016.Look_Backwards(utilityviewmodel, SmartParametersV2016.space, SmartParametersV2016.bar.ToString()))
                    {
                        if (SmartParseV2016.Look_Backwards(utilityviewmodel, SmartParametersV2016.space, SmartParametersV2016.bar.ToString()))
                        {
                            string[] components = utilityviewmodel.token.Split(SmartParametersV2016.bar);
                            int components_count = 0;
                            while (components_count < components.Length)
                            {
                                switch (components_count)
                                {
                                    case 0:
                                        break;
                                    case 1:
                                        OUTSTANDING_BALANCE = components[components_count].Trim();
                                        break;
                                    case 2:
                                        OUTSTANDING_BALANCE += components[components_count].Trim();
                                        if (!SmartParseV2016.Currency_Strip(utilityviewmodel.bill_currency_symbol, utilityviewmodel.bill_currency_separator, utilityviewmodel.bill_thousands_separator, OUTSTANDING_BALANCE, utilityviewmodel))
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
                                    default:
                                        break;
                                }
                                components_count++;
                            }
                        }
                    }
                }
            the_old_daysB2:
                continue;
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
            int section_index = 0;
            if (!SmartParseV2016.Decode_Section(section, sub_section, sections, utilityviewmodel.resource_code.ToString(), utilityviewmodel))
            {
                return true;
            }

            bool status = Section_B3_Due_Date(ourviewmodel,
                                            utilityviewmodel,
                                            sections,
                                            //section,
                                            section_index,
                                            lines,
                                            Convert.ToInt32(sections[section_index][4]));
            return status;
        }

        private static bool Section_B3_Due_Date(MainViewModel ourviewmodel,
                                            UtilityViewModel utilityviewmodel,
                                            string[][] sections,
                                            int section_index,
                                            string[] lines,
                                            int sub_line_count)
        {
            string PAYMENT_DUE_DATE;// = SmartParametersV2016.defaultDates;
            //DateTime temp_date = SmartParametersV2016.defaultDate;
            utilityviewmodel.token = "";

            for (int line_count = sub_line_count; line_count <= Convert.ToInt32(sections[section_index][5]); line_count++)
            {
                utilityviewmodel.token = utilityviewmodel.token + SmartParametersV2016.space + lines[line_count];
            }

            utilityviewmodel.token = utilityviewmodel.token.Trim();
            utilityviewmodel.token = SmartParseV2016.Remove_Double_Spaces_V3(utilityviewmodel.token);
            utilityviewmodel.token = utilityviewmodel.token.Replace(SmartParametersV2016.space, SmartParametersV2016.bar.ToString());
            string[] components = utilityviewmodel.token.Split(SmartParametersV2016.bar);
            int component = 0;
            while (component < components.Length)
            {
                if (components[component].Length == 5)
                {
                    if (components[component].Substring(0, 2) == "20")
                    {
                        if (components[component].Substring(4, 1) == SmartParametersV2016.period)
                        {
                            // This is the one
                            components[component] = components[component].Replace(SmartParametersV2016.period, "");
                            if (component >= 2)
                            {
                                PAYMENT_DUE_DATE = components[component - 2].Trim();
                                PAYMENT_DUE_DATE = PAYMENT_DUE_DATE.Replace("st", "");
                                PAYMENT_DUE_DATE = PAYMENT_DUE_DATE.Replace("nd", "");
                                PAYMENT_DUE_DATE = PAYMENT_DUE_DATE.Replace("rd", "");
                                PAYMENT_DUE_DATE = PAYMENT_DUE_DATE.Replace("th", "");


                                PAYMENT_DUE_DATE = PAYMENT_DUE_DATE + SmartParametersV2016.space + components[component - 1].Trim();
                                PAYMENT_DUE_DATE = PAYMENT_DUE_DATE + SmartParametersV2016.space + components[component].Trim();
                                if (!SmartParseV2016.Generic_Parse_Datetime(PAYMENT_DUE_DATE, utilityviewmodel))
                                {
                                    return false;
                                }
                                //else
                                //{
                                //    temp_date = utilityviewmodel.genericTargetDate;
                                //}
                                SmartParseV2016.Update_Bill(ourviewmodel, utilityviewmodel, "PAYMENT_DUE_DATE", PAYMENT_DUE_DATE);
                                SmartParseV2016.Update_Bill(ourviewmodel, utilityviewmodel, "DIRECT_DEBIT_DATE", PAYMENT_DUE_DATE);
                                break;
                            }
                        }
                    }
                }
                component++;
            }
            return true;
        }

        private static bool Parse_SectionC0(UtilityViewModel utilityviewmodel,
                                            string[][] sections,
                                            string section,
                                            string sub_section)
        {
            //int section_index = 0;
            if (!SmartParseV2016.Decode_Section(section, sub_section, sections, utilityviewmodel.resource_code.ToString(), utilityviewmodel))
            {
                return true;
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

            int section_index = 0;
            if (!SmartParseV2016.Decode_Section(section, sub_section, sections, utilityviewmodel.resource_code.ToString(), utilityviewmodel))
            {
                return true;
            }

            // Now the fat ugly bitch is snoring away on the settee ...
            utilityviewmodel.token = "";
            string Gas_O = "Gas", // O",                    // The gas flame becomes an 'O'
                    Electricity_Q = "Electricity", // Q",    // The lectricity bulb becomes a 'Q'
                    tariff_name = "Tariff name",
                    payment_method = "Payment method",
                    TARIFF_NAME,// = "",
                    PAYMENT_TYPE,
                    target_token = "";

            switch (utilityviewmodel.resource_code)
            {
                case SmartParametersV2016.Electricity:
                    target_token = Electricity_Q;
                    break;
                case SmartParametersV2016.Gas:
                    target_token = Gas_O;
                    break;
                default:
                    break;
            }

            for (int line_count = Convert.ToInt32(sections[section_index][4]); line_count <= Convert.ToInt32(sections[section_index][5]); line_count++)
            {
                utilityviewmodel.token = lines[line_count].Trim();
                if (SmartParseV2016.Token_Identify(utilityviewmodel, target_token, false))
                {
                    int sub_line_count = line_count;
                    while (sub_line_count < Convert.ToInt32(sections[section_index][5]))
                    {
                        utilityviewmodel.token = lines[sub_line_count].Trim();
                        if (utilityviewmodel.token.Contains(tariff_name))
                        {
                            TARIFF_NAME = utilityviewmodel.token.Replace(tariff_name, "").Trim();
                            if (string.IsNullOrEmpty(utilityviewmodel.TARIFF_NAME) ||
                                (utilityviewmodel.TARIFF_CODE == 0))
                            {
                                utilityviewmodel.TARIFF_NAME = TARIFF_NAME;

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
                        if (utilityviewmodel.token.Contains(payment_method))
                        {
                            PAYMENT_TYPE = utilityviewmodel.token.Replace(payment_method, "").Trim();

                            // Find a Payment Plan which contains this Payment Name
                            if (SmartSpikeUtilityV2017.Utility_Lookup_PaymentPlan(ourviewmodel,
                                                                        utilityviewmodel,
                                                                        PAYMENT_TYPE))
                            {
                                SmartParseV2016.Update_Bill(ourviewmodel, utilityviewmodel, "PAYMENT_PLAN", utilityviewmodel.PAYMENT_PLAN.ToString());
                                SmartParseV2016.Update_Bill(ourviewmodel, utilityviewmodel, "PAYMENT_TYPE", PAYMENT_TYPE);
                            }
                            else
                            {
                                return false;
                            }
                            break;
                        }
                        sub_line_count++;
                    }
                    line_count = sub_line_count;
                    goto the_good_old_daysD0;
                }

            the_good_old_daysD0:
                continue;
            }
            return true;
        }

        private static bool Parse_SectionE0(UtilityViewModel utilityviewmodel,
                                            string[][] sections,
                                            string section,
                                            string sub_section,
                                            string[] lines)
        {
            int section_index = 0;
            if (!SmartParseV2016.Decode_Section(section, sub_section, sections, utilityviewmodel.resource_code.ToString(), utilityviewmodel))
            {
                return true;
            }

            // Now the fat ugly bitch is snoring away on the settee ...
            utilityviewmodel.token = "";
            string tariff_comparison_rate = "Tariff Comparison Rate (TCR) for",
                    gas_tcr = "",
                    electricity_tcr = "";

            for (int line_count = Convert.ToInt32(sections[section_index][4]); line_count <= Convert.ToInt32(sections[section_index][5]); line_count++)
            {
                utilityviewmodel.token = lines[line_count].Trim();
                if (SmartParseV2016.Token_Identify(utilityviewmodel, tariff_comparison_rate, false))
                {
                    if (line_count + 1 <= Convert.ToInt32(sections[section_index][5]))
                    {
                        line_count++;
                        if (utilityviewmodel.token.Contains("Gas"))
                        {
                            gas_tcr = lines[line_count].Trim();
                        }
                        if (utilityviewmodel.token.Contains("Electricity"))
                        {
                            electricity_tcr = lines[line_count].Trim();
                        }
                        switch (utilityviewmodel.resource_code)
                        {
                            case SmartParametersV2016.Electricity:
                                utilityviewmodel.TCR = electricity_tcr.Replace("per kWh", "").Trim();
                                break;
                            case SmartParametersV2016.Gas:
                                utilityviewmodel.TCR = gas_tcr.Replace("per kWh", "").Trim(); ;
                                break;
                            default:
                                break;
                        }
                    }
                }
            }
            return true;
        }

        private static bool Parse_SectionF0(UtilityViewModel utilityviewmodel,
                                            string[][] sections,
                                            string section,
                                            string sub_section,
                                            string[] lines)
        {
            int section_index = 0;
            if (!SmartParseV2016.Decode_Section(section, sub_section, sections, utilityviewmodel.resource_code.ToString(), utilityviewmodel))
            {
                return true;
            }

            // Now the fat ugly bitch is snoring away on the settee ...
            utilityviewmodel.token = "";
            string expressed_in_kWh = "expressed in kWh";

            for (int line_count = Convert.ToInt32(sections[section_index][4]); line_count <= Convert.ToInt32(sections[section_index][5]); line_count++)
            {
                utilityviewmodel.token = lines[line_count].Trim();
                if (SmartParseV2016.Token_Identify(utilityviewmodel, expressed_in_kWh, true))
                {
                    if (line_count + 1 <= Convert.ToInt32(sections[section_index][5]))
                    {
                        line_count++;
                        utilityviewmodel.token = lines[line_count].Trim();
                        string[] components = utilityviewmodel.token.Split(SmartParametersV2016.spaceSplit);
                        if (components.Length == 2)
                        {
                            if (components[0].Contains(SmartParametersV2016.decimalPoint))
                            {
                                utilityviewmodel.CALORIFIC_VALUE = components[0];
                                // Work out a value
                                decimal TEMPDEC;
                                if (!SmartParseV2016.Generic_Parse_Decimal(utilityviewmodel.CALORIFIC_VALUE, utilityviewmodel))
                                {
                                    return false;
                                }
                                else
                                {
                                    TEMPDEC = utilityviewmodel.genericDecimalValue;
                                }
                                utilityviewmodel.CALORIFIC_VALUE = TEMPDEC.ToString("##.##"); // <= I may live to regret this ...
                            }
                            goto the_good_old_daysF0;
                        }
                    }
                }
            the_good_old_daysF0:
                continue;
            }
            return true;
        }

        private static bool Parse_SectionG0(UtilityViewModel utilityviewmodel,
                                            string[][] sections,
                                           string section,
                                           string sub_section)
        {
            //int section_index = 0;
            if (!SmartParseV2016.Decode_Section(section, sub_section, sections, utilityviewmodel.resource_code.ToString(), utilityviewmodel))
            {
                return true;
            }
            return true;
        }

        private static bool Parse_SectionH1(MainViewModel ourviewmodel,
                                            UtilityViewModel utilityviewmodel,
                                            string[][] sections,
                                            string section,
                                            string sub_section,
                                            string[] lines)
        {
            int section_index = 0;
            if (!SmartParseV2016.Decode_Section(section, sub_section, sections, utilityviewmodel.resource_code.ToString(), utilityviewmodel))
            {
                return true;
            }

            string METER_SERIAL_NO = "";
            char UNITS_TIME = SmartParametersV2016.daytimeUnit;
            //DateTime READINGS_PERIOD_START = SmartParametersV2016.defaultDate,
            //         READINGS_PERIOD_END = SmartParametersV2016.defaultDate,
            //         UNIT_CHARGES_PERIOD_START = SmartParametersV2016.defaultDate,
            //         UNIT_CHARGES_PERIOD_END = SmartParametersV2016.defaultDate,
            //         STANDING_CHARGES_PERIOD_START = SmartParametersV2016.defaultDate,
            //         STANDING_CHARGES_PERIOD_END = SmartParametersV2016.defaultDate;
            string standard_electricity_meter_number = "Standard Electricity Meter number",
                    smart_electricity_meter_number = "Smart Electricity Meter number",
                    start_reading = "Start Reading",
                    meter_number = "Meter number",
                    units_used = "Units used",
                    electricity_units_used = "Electricity units used",
                    consumption_charge = "Consumption charge,",
                    standing_charge_comma = "Standing charge,",
                    Standing_Charge = "Standing Charge",
                    total_electricity_charges = "Total electricity charges",
                    total_electricity_costs = "Total electricity costs";

            utilityviewmodel.token = "";
            int TEMP;// = 0;

            utilityviewmodel.readings_count = 0;
            utilityviewmodel.D_THIS_READ = "";
            utilityviewmodel.D_LAST_READ = "";
            utilityviewmodel.N_THIS_READ = "";
            utilityviewmodel.N_LAST_READ = "";
            string READ_TYPE = "";

            utilityviewmodel.units_band = 0;
            utilityviewmodel.charges_item = 0;

            for (int line_count = Convert.ToInt32(sections[section_index][4]); line_count <= Convert.ToInt32(sections[section_index][5]); line_count++)
            {
                utilityviewmodel.token = lines[line_count].Trim();

                if (SmartParseV2016.Token_Identify(utilityviewmodel, standard_electricity_meter_number +
                                                                SmartParametersV2016.bar +
                                                                smart_electricity_meter_number +
                                                                SmartParametersV2016.bar +
                                                                meter_number, true))
                {
                    METER_SERIAL_NO = utilityviewmodel.sparks.meter_serial_no;
                    goto the_good_old_daysH1;
                }

                if (SmartParseV2016.Token_Identify(utilityviewmodel, start_reading, false) ||
                      SmartParseV2016.Token_Identify_Middle(utilityviewmodel,
                                                                "Actual" +
                                                                SmartParametersV2016.bar +
                                                                "Estimated" +
                                                                SmartParametersV2016.bar +
                                                                "Customer read", false, ""))
                {
                    int sub_line_count = line_count;
                    if (!SectionH1_Units_Read(utilityviewmodel,
                                                sections,
                                                section_index,
                                                lines,
                                                ref sub_line_count))
                    {
                        return false;
                    }
                    line_count = sub_line_count - 1;
                    goto the_good_old_daysH1;
                }

                if (SmartParseV2016.Token_Identify(utilityviewmodel, units_used +
                                                                SmartParametersV2016.bar +
                                                                electricity_units_used, false))
                {
                    int sub_line_count = line_count;
                    if (!SectionH1_Units_Used(utilityviewmodel,
                                                sections,
                                                section_index,
                                                lines,
                                                METER_SERIAL_NO,
                                                READ_TYPE,
                                                utilityviewmodel.D_LAST_READ,
                                                utilityviewmodel.D_THIS_READ,
                                                utilityviewmodel.N_LAST_READ,
                                                utilityviewmodel.N_THIS_READ,
                                                ref sub_line_count))
                    {
                        return false;
                    }
                    line_count = sub_line_count - 1;
                    goto the_good_old_daysH1;
                }

                if (utilityviewmodel.token.Contains(consumption_charge) || // Do I REALLY need for this Elec??
                    utilityviewmodel.token.Contains("Unit rate"))
                {
                    int sub_line_count = line_count;
                    if (!SectionH1_Units_Charges(utilityviewmodel,
                                                        sections,
                                                        section_index,
                                                        lines,
                                                        UNITS_TIME,
                                                        ref sub_line_count))
                    {
                        return false;
                    }
                    line_count = sub_line_count - 1;
                    goto the_good_old_daysH1;
                }

                if (utilityviewmodel.token.Contains(standing_charge_comma) ||
                    utilityviewmodel.token.Contains(Standing_Charge))
                {
                    int sub_line_count = line_count;
                    if (!SectionH1_Standing_Charge(utilityviewmodel,
                                                        sections,
                                                        section_index,
                                                        lines,
                                                        ref sub_line_count))
                    {
                        return false;
                    }
                    line_count = sub_line_count - 1;
                    goto the_good_old_daysH1;
                }

                if (SmartParseV2016.Token_Identify(utilityviewmodel, total_electricity_charges +
                                                                SmartParametersV2016.bar +
                                                                total_electricity_costs, true))
                {
                    string E_NEW_CHARGES;
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
                    SmartParseV2016.Update_Bill(ourviewmodel, utilityviewmodel, "RESOURCE_NEW_CHARGES", E_NEW_CHARGES);

                    decimal TEMPDEC = 0.0M;
                    if (!SmartParseV2016.Lookup_Vat_Rate(utilityviewmodel.Hezbollah.bills_row.BILL_VAT_CODE, utilityviewmodel.Hezbollah.bills_row.BILL_PERIOD_START, utilityviewmodel.Hezbollah.bills_row.BILL_PERIOD_END, ourviewmodel.Blanche.vatRatesList, utilityviewmodel))
                    {
                        return false;
                    }
                    TEMP = Convert.ToInt32((utilityviewmodel.Hezbollah.bills_resource_row.NEW_CHARGES * TEMPDEC) / 100.0M);

                    SmartParseV2016.Update_Bill(ourviewmodel, utilityviewmodel, "RESOURCE_VAT_AMOUNT", TEMP.ToString());
                    SmartParseV2016.Update_Bill(ourviewmodel, utilityviewmodel, "RESOURCE_VAT_CODE", utilityviewmodel.Hezbollah.bills_row.BILL_VAT_CODE.ToString());
                    goto the_good_old_daysH1;
                }

            the_good_old_daysH1:
                continue;
            }
            return true;
        }

        private static bool SectionH1_Units_Read(UtilityViewModel utilityviewmodel,
                                                        string[][] sections,
                                                        int section_index,
                                                        string[] lines,
                                                        ref int sub_line_count)
        {
            string start_reading = "Start Reading",
                    actual = "Actual",
                    estimated = "Estimated",
                    Customer_read = "Customer read",
                    units_used = "Units used",
                    electricity_units_used = "Electricity units used";

            string Customer_read_underscore = Customer_read.Replace(SmartParametersV2016.space, "_");
            while (sub_line_count <= Convert.ToInt32(sections[section_index][5]))
            {
                utilityviewmodel.token = lines[sub_line_count].Replace(start_reading, "").Trim();
                if (utilityviewmodel.token.Contains(units_used) ||
                    utilityviewmodel.token.Contains(electricity_units_used))
                {
                    break;
                }
                if (SmartParseV2016.Token_Identify_Middle(utilityviewmodel,
                                                                actual +
                                                                SmartParametersV2016.bar +
                                                                estimated +
                                                                SmartParametersV2016.bar +
                                                                Customer_read, false, ""))
                {
                    // Now ... for older bills the SP Foreigners put the reading BEFORE the keyword thus:
                    // 27929 Actual 12 Oct 13
                    // 28472.60 Estimated 05 Dec 13
                    // whilst for new bills the Foreigners put the reading AFTER the keyword thus:
                    // 24 Oct 2016 Actual read: 40508
                    // 23 Jan 2017 Estimated read: 41728.9
                    // So do some re-arranging ...
                    utilityviewmodel.token = utilityviewmodel.token.Replace(Customer_read, Customer_read_underscore);
                    int colon = utilityviewmodel.token.IndexOf(SmartParametersV2016.colon);
                    if (colon >= 0)
                    {
                        string reading = utilityviewmodel.token.Substring(colon + 1).Trim();
                        utilityviewmodel.token = utilityviewmodel.token.Replace(reading, "").Trim();
                        int read_type = utilityviewmodel.token.IndexOf(actual);
                        if (read_type == -1)
                        {
                            read_type = utilityviewmodel.token.IndexOf(estimated);
                        }
                        if (read_type == -1)
                        {
                            read_type = utilityviewmodel.token.IndexOf(Customer_read_underscore);
                        }
                        if (read_type >= 0)
                        {
                            string new_string = utilityviewmodel.token.Substring(0, read_type).Trim();
                            string what = utilityviewmodel.token.Substring(read_type).Replace("read:", "").Trim();
                            utilityviewmodel.token = reading + SmartParametersV2016.space + what + SmartParametersV2016.space + new_string;
                        }
                    }

                    string TEMP_DATE = "";
                    if (utilityviewmodel.readings_count == 0)
                    {
                        utilityviewmodel.D_THIS_READ = "";
                        utilityviewmodel.D_LAST_READ = "";
                        utilityviewmodel.N_THIS_READ = "";
                        utilityviewmodel.N_LAST_READ = "";
                        utilityviewmodel.READ_TYPE = "";
                        utilityviewmodel.READINGS_PERIOD_START = SmartParametersV2016.defaultDates;
                        utilityviewmodel.READINGS_PERIOD_END = SmartParametersV2016.defaultDates;
                    }
                    else
                    {
                        utilityviewmodel.D_LAST_READ = utilityviewmodel.D_THIS_READ;
                        utilityviewmodel.N_LAST_READ = utilityviewmodel.N_THIS_READ;
                        utilityviewmodel.READINGS_PERIOD_START = utilityviewmodel.READINGS_PERIOD_END;
                    }

                    string[] components = utilityviewmodel.token.Split(SmartParametersV2016.spaceSplit);

                    int components_count = 0;
                    while (components_count < components.Length)
                    {
                        switch (components_count)
                        {
                            case 0:
                                utilityviewmodel.D_THIS_READ = components[components_count];
                                break;
                            case 1:
                                utilityviewmodel.READ_TYPE = components[components_count].Trim();
                                utilityviewmodel.READ_TYPE = utilityviewmodel.READ_TYPE.Replace("_", SmartParametersV2016.space);
                                break;
                            default:
                                TEMP_DATE = TEMP_DATE + SmartParametersV2016.space + components[components_count];
                                break;
                        }
                        components_count++;
                    }
                    if (!SmartParseV2016.Generic_Parse_Datetime(TEMP_DATE.Trim(), utilityviewmodel))
                    {
                        return false;
                    }
                    else
                    {
                        utilityviewmodel.READINGS_PERIOD_END = TEMP_DATE.Trim();
                    }
                    utilityviewmodel.readings_count++;
                }
                sub_line_count++;
            }
            return true;
        }

        private static bool SectionH1_Units_Used(UtilityViewModel utilityviewmodel,
                                                        string[][] sections,
                                                        int section_index,
                                                        string[] lines,
                                                        string METER_SERIAL_NO,
                                                        string READ_TYPE,
                                                        string D_LAST_READ,
                                                        string D_THIS_READ,
                                                        string N_LAST_READ,
                                                        string N_THIS_READ,
                                                        ref int sub_line_count)
        {
            string start_reading = "Start Reading",
                    units_used = "Units used",
                    electricity_units_used = "Electricity units used",
                    consumption_charge = "Consumption charge,";

            while (sub_line_count <= Convert.ToInt32(sections[section_index][5]))
            {
                utilityviewmodel.token = lines[sub_line_count].Replace(electricity_units_used, "").Trim();
                utilityviewmodel.token = utilityviewmodel.token.Replace(units_used, "").Trim();
                if ((utilityviewmodel.token.Contains(start_reading)) ||
                    (utilityviewmodel.token.Contains("Unit rate")) ||
                    (utilityviewmodel.token.Contains(consumption_charge)))
                {
                    break;
                }
                if (utilityviewmodel.token.Contains(SmartParametersV2016.equivalent))
                {
                    string[] components = utilityviewmodel.token.Split(SmartParametersV2016.spaceSplit);
                    int component_count = 0;
                    string D_UNITS_USED = "0",
                            N_UNITS_USED = "0",
                            UNIT_OF_MEASURE = "";
                    while (component_count < components.Length)
                    {
                        switch (component_count)
                        {
                            case 0:
                                D_UNITS_USED = components[component_count].Trim();
                                break;
                            case 3:
                                UNIT_OF_MEASURE = components[component_count].Trim();
                                if (UNIT_OF_MEASURE == "KWh")
                                {
                                    UNIT_OF_MEASURE = "kWh";
                                }
                                break;
                            case 1:
                            case 2:
                            default:
                                break;
                        }
                        component_count++;
                    }

                    if (SmartParseV2016.Add_Readings_Test(SmartParametersV2016.defaultDate, Convert.ToDateTime(utilityviewmodel.READINGS_PERIOD_START), Convert.ToDateTime(utilityviewmodel.READINGS_PERIOD_END), D_LAST_READ, D_THIS_READ, UNIT_OF_MEASURE))
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
                                break;
                            default:
                                break;
                        }
                    }
                }
                sub_line_count++;
            }
            return true;
        }

        private static bool SectionH1_Units_Charges(UtilityViewModel utilityviewmodel,
                                                        string[][] sections,
                                                        int section_index,
                                                        string[] lines,
                                                        char UNITS_TIME,
                                                        ref int sub_line_count)
        {
            string standard_electricity = "Standard Electricity",
                    consumption_charge = "Consumption charge,",
                    unit_rate = "Unit rate",
                    standing_charge_comma = "Standing charge,",
                    Standing_Charge = "Standing Charge",
                    total_electricity_charges = "Total electricity charges",
                    total_electricity_costs = "Total electricity costs";

            while (sub_line_count <= Convert.ToInt32(sections[section_index][5]))
            {
                utilityviewmodel.token = lines[sub_line_count].Trim();
                if ((utilityviewmodel.token.Contains(total_electricity_charges)) ||
                    (utilityviewmodel.token.Contains(total_electricity_costs)) ||
                    (utilityviewmodel.token.Contains(standing_charge_comma)) ||
                    (utilityviewmodel.token.Contains(Standing_Charge)) ||
                    (utilityviewmodel.token.Contains(standard_electricity)))
                {
                    break;
                }

                utilityviewmodel.UNIT_CHARGES_PERIOD_END = SmartParametersV2016.defaultDates;

                string us_consumption_charge = consumption_charge.Replace(SmartParametersV2016.space, SmartParametersV2016.underscore);
                utilityviewmodel.token = utilityviewmodel.token.Replace(consumption_charge, us_consumption_charge);

                if (utilityviewmodel.token.Contains(unit_rate))
                {
                    string us_unit_rate = unit_rate.Replace(SmartParametersV2016.space, SmartParametersV2016.underscore);
                    utilityviewmodel.token = utilityviewmodel.token.Replace(unit_rate, us_unit_rate);

                    utilityviewmodel.token = SmartTimeV2016.ConvertDateTime(utilityviewmodel.BILL_PERIOD_START).ToString(SmartParametersV2016.ddmmmyyyyFormat) +
                                SmartParametersV2016.space +
                                SmartTimeV2016.ConvertDateTime(utilityviewmodel.BILL_PERIOD_END).ToString(SmartParametersV2016.ddmmmyyyyFormat) +
                                SmartParametersV2016.space +
                                utilityviewmodel.token;
                }
                utilityviewmodel.units_band = 1;
                if (utilityviewmodel.token.Contains("primary"))
                {
                    utilityviewmodel.token = utilityviewmodel.token.Replace("primary", "");
                }
                else
                {
                    if (utilityviewmodel.token.Contains("secondary"))
                    {
                        utilityviewmodel.units_band = 2;
                        utilityviewmodel.token = utilityviewmodel.token.Replace("secondary", "");
                    }
                }
                utilityviewmodel.token = utilityviewmodel.token.Replace("to", "");
                utilityviewmodel.token = utilityviewmodel.token.Replace(SmartParametersV2016.equivalent, "").Trim();
                utilityviewmodel.token = SmartParseV2016.Remove_Double_Spaces_V3(utilityviewmodel.token);
                string[] components = utilityviewmodel.token.Split(SmartParametersV2016.spaceSplit);
                int component_count = 0;
                string TEMP_DATE,// = "",
                        UNITS_TYPE = "",
                        UNITS = "0",
                        UNIT_OF_MEASURE = "",
                        UNITS_RATE = "0",
                        UNITS_COST = "";
                while (component_count < components.Length)
                {
                    switch (component_count)
                    {
                        case 0:     // From date
                            TEMP_DATE = components[component_count].Trim();
                            if (!SmartParseV2016.Generic_Parse_Datetime(TEMP_DATE, utilityviewmodel))
                            {
                                return false;
                            }
                            utilityviewmodel.UNIT_CHARGES_PERIOD_START = TEMP_DATE;
                            break;
                        case 1:     // To Date
                            TEMP_DATE = components[component_count].Trim();
                            if (!SmartParseV2016.Generic_Parse_Datetime(TEMP_DATE, utilityviewmodel))
                            {
                                return false;
                            }
                            utilityviewmodel.UNIT_CHARGES_PERIOD_END = TEMP_DATE;
                            break;
                        case 2:     // Consumption_charge
                            UNITS_TYPE = components[component_count].Replace(SmartParametersV2016.underscore, SmartParametersV2016.space);
                            UNITS_TYPE = UNITS_TYPE.Replace(SmartParametersV2016.comma, "").Trim();
                            UNITS_TYPE = (UNITS_TYPE.Length <= SmartParametersV2016.TYPESLENGTH ? UNITS_TYPE : UNITS_TYPE.Substring(0, SmartParametersV2016.TYPESLENGTH));
                            break;
                        case 3:
                            UNITS = components[component_count].Trim();
                            break;
                        case 4:
                            UNIT_OF_MEASURE = components[component_count].Trim();
                            break;
                        case 5:     // x
                            break;
                        case 6:     // Rate in p
                            UNITS_RATE = components[component_count].Trim();
                            UNITS_RATE = UNITS_RATE.Replace(utilityviewmodel.bill_denomination_symbol.ToString(), "");
                            break;
                        case 7:     // Total Cost
                            //int TEMP = 0;
                            if (!SmartParseV2016.Currency_Strip(utilityviewmodel.bill_currency_symbol, utilityviewmodel.bill_currency_separator, utilityviewmodel.bill_thousands_separator, components[component_count].Trim(), utilityviewmodel))
                            {
                                return false;
                            }
                            else
                            {
                                UNITS_COST = utilityviewmodel.value;
                            }
                            if (!SmartParseV2016.Generic_Parse_Integer(UNITS_COST, utilityviewmodel))
                            {
                                return false;
                            }
                            break;
                        default:
                            break;
                    }
                    component_count++;
                }

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
                                                                    UNITS_TYPE,
                                                                    UNITS,
                                                                    UNITS_RATE,
                                                                    UNIT_OF_MEASURE,
                                                                    UNITS_COST);
                        break;
                    default:
                        break;
                }
                sub_line_count++;
            }
            return true;
        }

        private static bool SectionH1_Standing_Charge(UtilityViewModel utilityviewmodel,
                                                        string[][] sections,
                                                        int section_index,
                                                        string[] lines,
                                                        ref int sub_line_count)
        {
            string standard_electricity = "Standard Electricity",
                    standing_charge_comma = "Standing charge,",
                    Standing_Charge = "Standing Charge",
                    total_electricity_charges = "Total electricity charges",
                    total_electricity_costs = "Total electricity costs",
                    your_electricity_supply_number = "Your electricity supply number";

            while (sub_line_count <= Convert.ToInt32(sections[section_index][5]))
            {
                utilityviewmodel.token = lines[sub_line_count].Trim();
                if ((utilityviewmodel.token.Contains(total_electricity_charges)) ||
                    (utilityviewmodel.token.Contains(total_electricity_costs)) ||
                    (utilityviewmodel.token.Contains(standard_electricity)) ||
                    (utilityviewmodel.token.Contains(your_electricity_supply_number)))
                {
                    break;
                }

                if (SmartParseV2016.Token_Identify(utilityviewmodel, Standing_Charge, false) ||
                    SmartParseV2016.Token_Identify_Middle(utilityviewmodel, standing_charge_comma, false, ""))
                {
                    utilityviewmodel.STANDING_CHARGES_PERIOD_END = SmartParametersV2016.defaultDates;

                    string us_standing_charge = standing_charge_comma.Replace(SmartParametersV2016.space, SmartParametersV2016.underscore);
                    utilityviewmodel.token = utilityviewmodel.token.Replace(standing_charge_comma, us_standing_charge);
                    if (utilityviewmodel.token.Contains(Standing_Charge))
                    {
                        us_standing_charge = Standing_Charge.Replace(SmartParametersV2016.space, SmartParametersV2016.underscore);
                        utilityviewmodel.token = utilityviewmodel.token.Replace(Standing_Charge, us_standing_charge);
                        utilityviewmodel.token = SmartTimeV2016.ConvertDateTime(utilityviewmodel.BILL_PERIOD_START).ToString(SmartParametersV2016.ddmmmyyyyFormat) +
                                    SmartParametersV2016.space +
                                    SmartTimeV2016.ConvertDateTime(utilityviewmodel.BILL_PERIOD_END).ToString(SmartParametersV2016.ddmmmyyyyFormat) +
                                    SmartParametersV2016.space +
                                    utilityviewmodel.token;
                    }
                    utilityviewmodel.token = utilityviewmodel.token.Replace("to", "");
                    utilityviewmodel.token = utilityviewmodel.token.Replace(SmartParametersV2016.equivalent, "").Trim();
                    utilityviewmodel.token = SmartParseV2016.Remove_Double_Spaces_V3(utilityviewmodel.token);
                    string[] components = utilityviewmodel.token.Split(SmartParametersV2016.spaceSplit);
                    if (components.Length < 8)
                    {
                        sub_line_count++;
                        if (lines[sub_line_count].Contains(utilityviewmodel.bill_currency_symbol))
                        {
                            utilityviewmodel.token = utilityviewmodel.token + SmartParametersV2016.space + lines[sub_line_count].Trim();
                            components = utilityviewmodel.token.Split(SmartParametersV2016.spaceSplit);
                        }
                    }
                    int component_count = 0;
                    string TEMP_DATE,
                            CHARGES_TYPE = "",
                            CHARGES_DAYS = "0",
                            STANDING_CHARGE = "0",
                            CHARGES_COST = "0";
                    while (component_count < components.Length)
                    {
                        switch (component_count)
                        {
                            case 0:     // From date
                                TEMP_DATE = components[component_count].Trim();
                                if (!SmartParseV2016.Generic_Parse_Datetime(TEMP_DATE, utilityviewmodel))
                                {
                                    return false;
                                }
                                utilityviewmodel.STANDING_CHARGES_PERIOD_START = TEMP_DATE;
                                break;
                            case 1:     // To Date
                                TEMP_DATE = components[component_count].Trim();
                                if (!SmartParseV2016.Generic_Parse_Datetime(TEMP_DATE, utilityviewmodel))
                                {
                                    return false;
                                }
                                utilityviewmodel.STANDING_CHARGES_PERIOD_END = TEMP_DATE;
                                break;
                            case 2:     // Consumption_charge
                                CHARGES_TYPE = components[component_count].Replace(SmartParametersV2016.underscore, SmartParametersV2016.space);
                                CHARGES_TYPE = CHARGES_TYPE.Replace(SmartParametersV2016.comma, "").Trim();
                                CHARGES_TYPE = (CHARGES_TYPE.Length <= SmartParametersV2016.TYPESLENGTH ? CHARGES_TYPE : CHARGES_TYPE.Substring(0, SmartParametersV2016.TYPESLENGTH));
                                break;
                            case 3:
                                CHARGES_DAYS = components[component_count].Trim();
                                break;
                            case 4:     // days
                                break;
                            case 5:     // x
                                break;
                            case 6:     // Rate in p
                                STANDING_CHARGE = components[component_count].Trim();
                                STANDING_CHARGE = STANDING_CHARGE.Replace(utilityviewmodel.bill_denomination_symbol.ToString(), "");
                                break;
                            case 7:     // Total Cost
                                //int TEMP = 0;
                                if (!SmartParseV2016.Currency_Strip(utilityviewmodel.bill_currency_symbol, utilityviewmodel.bill_currency_separator, utilityviewmodel.bill_thousands_separator, components[component_count].Trim(), utilityviewmodel))
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
                                break;
                            default:
                                break;
                        }
                        component_count++;
                    }
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
                                                                            utilityviewmodel.STANDING_CHARGES_PERIOD_START,
                                                                            utilityviewmodel.STANDING_CHARGES_PERIOD_END,
                                                                            utilityviewmodel.charges_item.ToString(),
                                                                            CHARGES_TYPE,
                                                                            STANDING_CHARGE,
                                                                            CHARGES_DAYS,
                                                                            CHARGES_COST);
                            break;
                        default:
                            break;
                    }
                }
                sub_line_count++;
            }
            return true;
        }

        private static bool Parse_SectionH2(MainViewModel ourviewmodel,
                                            UtilityViewModel utilityviewmodel,
                                            string[][] sections,
                                            string section,
                                            string sub_section,
                                            string[] lines)
        {
            int section_index = 0;
            if (!SmartParseV2016.Decode_Section(section, sub_section, sections, utilityviewmodel.resource_code.ToString(), utilityviewmodel))
            {
                return true;
            }

            utilityviewmodel.READINGS_PERIOD_START = SmartParametersV2016.defaultDates;
            utilityviewmodel.READINGS_PERIOD_END = SmartParametersV2016.defaultDates;
            utilityviewmodel.UNIT_CHARGES_PERIOD_START = SmartParametersV2016.defaultDates;
            utilityviewmodel.UNIT_CHARGES_PERIOD_END = SmartParametersV2016.defaultDates;
            utilityviewmodel.STANDING_CHARGES_PERIOD_START = SmartParametersV2016.defaultDates;
            utilityviewmodel.STANDING_CHARGES_PERIOD_END = SmartParametersV2016.defaultDates;

            string standard_gas_meter_number = "Standard Gas Meter number",
                   smart_gas_meter_number = "Smart Gas Meter number",
                    start_reading = "Start Reading",
                    meter_number = "Meter number",
                    total_gas_charges = "Total gas charges",
                    total_gas_costs = "Total gas costs",
                    units_used = "Units used",
                    gas_units_used = "Gas units used",
                    consumption_charge = "Consumption charge,",
                    standing_charge_comma = "Standing charge,",
                    Standing_Charge = "Standing Charge";

            utilityviewmodel.token = "";
            string METER_SERIAL_NO = "";
            int TEMP;// = 0;

            utilityviewmodel.readings_count = 0;
            string D_THIS_READ = "0",
                    D_LAST_READ = "0";
            string READ_TYPE = "";
            utilityviewmodel.units_band = 0;
            utilityviewmodel.charges_item = 0;

            for (int line_count = Convert.ToInt32(sections[section_index][4]); line_count <= Convert.ToInt32(sections[section_index][5]); line_count++)
            {
                utilityviewmodel.token = lines[line_count].Trim();
                if (SmartParseV2016.Token_Identify(utilityviewmodel, standard_gas_meter_number +
                                                                SmartParametersV2016.bar +
                                                                smart_gas_meter_number +
                                                                SmartParametersV2016.bar +
                                                                meter_number, true))
                {
                    METER_SERIAL_NO = utilityviewmodel.smell.meter_serial_no;
                    goto the_good_old_daysH2;
                }

                if (SmartParseV2016.Token_Identify(utilityviewmodel, start_reading, false) ||
                      SmartParseV2016.Token_Identify_Middle(utilityviewmodel,
                                                                "Actual" +
                                                                SmartParametersV2016.bar +
                                                                "Estimated" +
                                                                SmartParametersV2016.bar +
                                                                "Customer read", false, ""))
                {
                    utilityviewmodel.readings_count = 0;
                    int sub_line_count = line_count;
                    if (!SectionH2_Units_Read(utilityviewmodel,
                                                sections,
                                                section_index,
                                                lines,
                                                ref sub_line_count))
                    {
                        return false;
                    }
                    line_count = sub_line_count - 1;
                    goto the_good_old_daysH2;
                }

                if (SmartParseV2016.Token_Identify(utilityviewmodel, units_used +
                                                                SmartParametersV2016.bar +
                                                                gas_units_used, false))
                {
                    int sub_line_count = line_count;
                    if (!SectionH2_Units_Used(utilityviewmodel,
                                                sections,
                                                section_index,
                                                lines,
                                                METER_SERIAL_NO,
                                                READ_TYPE,
                                                D_LAST_READ,
                                                D_THIS_READ,
                                                ref sub_line_count))
                    {
                        return false;
                    }
                    line_count = sub_line_count - 1;
                    goto the_good_old_daysH2;
                }

                if (utilityviewmodel.token.Contains(consumption_charge) ||
                    utilityviewmodel.token.Contains("Unit rate"))
                {
                    int sub_line_count = line_count;
                    if (!SectionH2_Unit_Charges(utilityviewmodel,
                                                sections,
                                                section_index,
                                                lines,
                                                ref sub_line_count))
                    {
                        return false;
                    }
                    line_count = sub_line_count - 1;
                    goto the_good_old_daysH2;
                }

                if (utilityviewmodel.token.Contains(standing_charge_comma) ||
                    utilityviewmodel.token.Contains(Standing_Charge))
                {
                    int sub_line_count = line_count;
                    if (!SectionH2_Standing_Charges(utilityviewmodel,
                                                        sections,
                                                        section_index,
                                                        lines,
                                                        ref sub_line_count))
                    {
                        return false;
                    }
                    line_count = sub_line_count - 1;
                    goto the_good_old_daysH2;
                }

                if (SmartParseV2016.Token_Identify(utilityviewmodel,
                                                    total_gas_charges +
                                                    SmartParametersV2016.bar +
                                                    total_gas_costs, true))
                {
                    string G_NEW_CHARGES;
                    //int temp = 0;
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

                    decimal TEMPDEC = 0.0M;
                    if (!SmartParseV2016.Lookup_Vat_Rate(utilityviewmodel.Hezbollah.bills_row.BILL_VAT_CODE, utilityviewmodel.Hezbollah.bills_row.BILL_PERIOD_START, utilityviewmodel.Hezbollah.bills_row.BILL_PERIOD_END, ourviewmodel.Blanche.vatRatesList, utilityviewmodel))
                    {
                        return false;
                    }
                    TEMP = Convert.ToInt32((utilityviewmodel.Hezbollah.bills_resource_row.NEW_CHARGES * TEMPDEC) / 100.0M);

                    SmartParseV2016.Update_Bill(ourviewmodel, utilityviewmodel, "RESOURCE_VAT_AMOUNT", TEMP.ToString());
                    SmartParseV2016.Update_Bill(ourviewmodel, utilityviewmodel, "RESOURCE_VAT_CODE", utilityviewmodel.Hezbollah.bills_row.BILL_VAT_CODE.ToString());

                    goto the_good_old_daysH2;
                }
            the_good_old_daysH2:
                continue;
            }
            return true;
        }

        private static bool SectionH2_Units_Read(UtilityViewModel utilityviewmodel,
                                                    string[][] sections,
                                                    int section_index,
                                                    string[] lines,
                                                    ref int sub_line_count)
        {
            string start_reading = "Start Reading",
                    actual = "Actual",
                    estimated = "Estimated",
                    Customer_read = "Customer read",
                    units_used = "Units used",
                    gas_units_used = "Gas units used";
            string Customer_read_underscore = Customer_read.Replace(SmartParametersV2016.space, "_");
            while (sub_line_count <= Convert.ToInt32(sections[section_index][5]))
            {
                utilityviewmodel.token = lines[sub_line_count].Replace(start_reading, "").Trim();
                if (utilityviewmodel.token.Contains(units_used) ||
                    utilityviewmodel.token.Contains(gas_units_used))
                {
                    break;
                }
                if (SmartParseV2016.Token_Identify_Middle(utilityviewmodel,
                                                                actual +
                                                                SmartParametersV2016.bar +
                                                                estimated +
                                                                SmartParametersV2016.bar +
                                                                Customer_read, false, ""))
                {
                    utilityviewmodel.token = utilityviewmodel.token.Replace(Customer_read, Customer_read_underscore);
                    string TEMP_DATE = "";
                    if (utilityviewmodel.readings_count == 0)
                    {
                        utilityviewmodel.D_THIS_READ = "";
                        utilityviewmodel.D_LAST_READ = ""; // D_UNITS_USED_M3 = D_UNITS_USED_KWH = "0";
                        utilityviewmodel.READ_TYPE = "";
                        utilityviewmodel.READINGS_PERIOD_START = SmartParametersV2016.defaultDates;
                        utilityviewmodel.READINGS_PERIOD_END = SmartParametersV2016.defaultDates;
                        //UNIT_OF_MEASURE = "";
                    }
                    else
                    {
                        utilityviewmodel.D_LAST_READ = "";
                        utilityviewmodel.D_THIS_READ = "";
                        utilityviewmodel.READINGS_PERIOD_START = utilityviewmodel.READINGS_PERIOD_END;
                    }

                    // Now ... for older bills the SP Foreigners put the reading BEFORE the keyword thus:
                    // 13206 Actual 12 Oct 13
                    // 13287 Estimated 28 Oct 13
                    // whilst for new bills the Foreigners put the reading AFTER the keyword thus:
                    // 24 Oct 2016 Actual read: 18094
                    // 23 Jan 2017 Estimated read: 18992
                    // So do some re-arranging ...
                    int colon = utilityviewmodel.token.IndexOf(SmartParametersV2016.colon);
                    if (colon >= 0)
                    {
                        string reading = utilityviewmodel.token.Substring(colon + 1).Trim();
                        utilityviewmodel.token = utilityviewmodel.token.Replace(reading, "").Trim();
                        int read_type = utilityviewmodel.token.IndexOf(actual);
                        if (read_type == -1)
                        {
                            read_type = utilityviewmodel.token.IndexOf(estimated);
                        }
                        if (read_type == -1)
                        {
                            read_type = utilityviewmodel.token.IndexOf(Customer_read_underscore);
                        }
                        if (read_type >= 0)
                        {
                            string new_string = utilityviewmodel.token.Substring(0, read_type).Trim();
                            string what = utilityviewmodel.token.Substring(read_type).Replace("read:", "").Trim();
                            utilityviewmodel.token = reading + SmartParametersV2016.space + what + SmartParametersV2016.space + new_string;
                        }
                    }
                    string[] components = utilityviewmodel.token.Split(SmartParametersV2016.spaceSplit);

                    int components_count = 0;
                    while (components_count < components.Length)
                    {
                        switch (components_count)
                        {
                            case 0:
                                utilityviewmodel.D_THIS_READ = components[components_count];
                                break;
                            case 1:
                                utilityviewmodel.READ_TYPE = components[components_count].Trim();
                                utilityviewmodel.READ_TYPE = utilityviewmodel.READ_TYPE.Replace("_", SmartParametersV2016.space);
                                break;
                            default:
                                TEMP_DATE = TEMP_DATE + SmartParametersV2016.space + components[components_count];
                                break;
                        }
                        components_count++;
                    }
                    if (!SmartParseV2016.Generic_Parse_Datetime(TEMP_DATE.Trim(), utilityviewmodel))
                    {
                        return false;
                    }
                    utilityviewmodel.READINGS_PERIOD_END = TEMP_DATE;
                    utilityviewmodel.readings_count++;
                }
                sub_line_count++;
            }
            return true;
        }

        private static bool SectionH2_Units_Used(UtilityViewModel utilityviewmodel,
                                                        string[][] sections,
                                                        int section_index,
                                                        string[] lines,
                                                        string METER_SERIAL_NO,
                                                        string READ_TYPE,
                                                        string D_LAST_READ,
                                                        string D_THIS_READ,
                                                        ref int sub_line_count)
        {
            string start_reading = "Start Reading",
                    units_used = "Units used",
                    gas_units_used = "Gas units used",
                    consumption_charge = "Consumption charge,";

            while (sub_line_count <= Convert.ToInt32(sections[section_index][5]))
            {
                utilityviewmodel.token = lines[sub_line_count].Replace(gas_units_used, "").Trim();
                utilityviewmodel.token = utilityviewmodel.token.Replace(units_used, "").Trim();
                if ((utilityviewmodel.token.Contains(start_reading)) ||
                    (utilityviewmodel.token.Contains("Unit rate")) ||
                     (utilityviewmodel.token.Contains(consumption_charge)))
                {
                    break;
                }
                if (utilityviewmodel.token.Contains(SmartParametersV2016.equivalent))
                {
                    string[] components = utilityviewmodel.token.Split(SmartParametersV2016.spaceSplit);
                    int component_count = 0;
                    string D_UNITS_USED_M3 = "0",
                            D_UNITS_USED_KWH = "0",
                            UNIT_OF_MEASURE = "";
                    while (component_count < components.Length)
                    {
                        switch (component_count)
                        {
                            case 0:
                                D_UNITS_USED_M3 = components[component_count].Trim();
                                break;
                            case 1:
                                // =
                                break;
                            case 2:
                                D_UNITS_USED_KWH = components[component_count].Trim();
                                break;
                            case 3:
                                UNIT_OF_MEASURE = components[component_count].Trim();
                                if (UNIT_OF_MEASURE == "KWh")
                                {
                                    UNIT_OF_MEASURE = "m3";
                                }
                                break;
                            default:
                                break;
                        }
                        component_count++;
                    }

                    if (SmartParseV2016.Add_Readings_Test(SmartParametersV2016.defaultDate, Convert.ToDateTime(utilityviewmodel.READINGS_PERIOD_START), Convert.ToDateTime(utilityviewmodel.READINGS_PERIOD_END), D_LAST_READ, D_THIS_READ, UNIT_OF_MEASURE))
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
                }
                sub_line_count++;
            }
            return true;
        }

        private static bool SectionH2_Unit_Charges(UtilityViewModel utilityviewmodel,
                                                        string[][] sections,
                                                        int section_index,
                                                        string[] lines,
                                                        ref int sub_line_count)
        {
            string standard_gas = "Standard Gas",
                    total_gas_charges = "Total gas charges",
                    total_gas_costs = "Total gas costs",
                    consumption_charge = "Consumption charge,",
                    unit_rate = "Unit rate",
                    standing_charge_comma = "Standing charge,",
                    Standing_Charge = "Standing Charge";

            while (sub_line_count <= Convert.ToInt32(sections[section_index][5]))
            {
                utilityviewmodel.token = lines[sub_line_count].Trim();
                if ((utilityviewmodel.token.Contains(total_gas_charges)) ||
                    (utilityviewmodel.token.Contains(total_gas_costs)) ||
                    (utilityviewmodel.token.Contains(standing_charge_comma)) ||
                    (utilityviewmodel.token.Contains(Standing_Charge)) ||
                    (utilityviewmodel.token.Contains(standard_gas)))
                {
                    break;
                }

                utilityviewmodel.UNIT_CHARGES_PERIOD_END = SmartParametersV2016.defaultDates;

                string us_consumption_charge = consumption_charge.Replace(SmartParametersV2016.space, SmartParametersV2016.underscore);
                utilityviewmodel.token = utilityviewmodel.token.Replace(consumption_charge, us_consumption_charge);
                if (utilityviewmodel.token.Contains(unit_rate))
                {
                    string us_unit_rate = unit_rate.Replace(SmartParametersV2016.space, SmartParametersV2016.underscore);
                    utilityviewmodel.token = utilityviewmodel.token.Replace(unit_rate, us_unit_rate);

                    utilityviewmodel.token = SmartTimeV2016.ConvertDateTime(utilityviewmodel.BILL_PERIOD_START).ToString(SmartParametersV2016.ddmmmyyyyFormat) +
                                SmartParametersV2016.space +
                                SmartTimeV2016.ConvertDateTime(utilityviewmodel.BILL_PERIOD_END).ToString(SmartParametersV2016.ddmmmyyyyFormat) +
                                SmartParametersV2016.space +
                                utilityviewmodel.token;
                }
                utilityviewmodel.units_band = 1;
                if (utilityviewmodel.token.Contains("primary"))
                {
                    utilityviewmodel.token = utilityviewmodel.token.Replace("primary", "");
                }
                else
                {
                    if (utilityviewmodel.token.Contains("secondary"))
                    {
                        utilityviewmodel.units_band = 2;
                        utilityviewmodel.token = utilityviewmodel.token.Replace("secondary", "");
                    }
                }
                utilityviewmodel.token = utilityviewmodel.token.Replace("to", "");
                utilityviewmodel.token = utilityviewmodel.token.Replace(SmartParametersV2016.equivalent, "").Trim();
                utilityviewmodel.token = utilityviewmodel.token.Replace(utilityviewmodel.bill_currency_symbol + SmartParametersV2016.space, utilityviewmodel.bill_currency_symbol);
                utilityviewmodel.token = SmartParseV2016.Remove_Double_Spaces_V3(utilityviewmodel.token);

                string[] components = utilityviewmodel.token.Split(SmartParametersV2016.spaceSplit);
                int component_count = 0;
                string TEMP_DATE,// = "",
                        UNITS_TYPE = "",
                        UNITS = "0",
                        UNIT_OF_MEASURE = "",
                        UNITS_RATE = "0",
                        UNITS_COST = "0";
                while (component_count < components.Length)
                {
                    switch (component_count)
                    {
                        case 0:     // From date
                            TEMP_DATE = components[component_count].Trim();
                            if (!SmartParseV2016.Generic_Parse_Datetime(TEMP_DATE, utilityviewmodel))
                            {
                                return false;
                            }
                            utilityviewmodel.UNIT_CHARGES_PERIOD_START = TEMP_DATE;
                            break;
                        case 1:     // To Date
                            TEMP_DATE = components[component_count].Trim();
                            if (!SmartParseV2016.Generic_Parse_Datetime(TEMP_DATE, utilityviewmodel))
                            {
                                return false;
                            }
                            utilityviewmodel.UNIT_CHARGES_PERIOD_END = TEMP_DATE;
                            break;
                        case 2:     // Consumption_charge
                            UNITS_TYPE = components[component_count].Replace(SmartParametersV2016.underscore, SmartParametersV2016.space);
                            UNITS_TYPE = UNITS_TYPE.Replace(SmartParametersV2016.comma, "").Trim();
                            UNITS_TYPE = (UNITS_TYPE.Length <= SmartParametersV2016.TYPESLENGTH ? UNITS_TYPE : UNITS_TYPE.Substring(0, SmartParametersV2016.TYPESLENGTH));
                            break;
                        case 3:
                            UNITS = components[component_count].Trim();
                            break;
                        case 4:
                            UNIT_OF_MEASURE = components[component_count].Trim();
                            break;
                        case 5:     // x
                            break;
                        case 6:     // Rate in p
                            UNITS_RATE = components[component_count].Trim();
                            UNITS_RATE = UNITS_RATE.Replace(utilityviewmodel.bill_denomination_symbol.ToString(), "");
                            break;
                        case 7:     // Total Cost
                            if (!SmartParseV2016.Currency_Strip(utilityviewmodel.bill_currency_symbol, utilityviewmodel.bill_currency_separator, utilityviewmodel.bill_thousands_separator, components[component_count].Trim(), utilityviewmodel))
                            {
                                return false;
                            }
                            else
                            {
                                UNITS_COST = utilityviewmodel.value;
                            }
                            if (!SmartParseV2016.Generic_Parse_Integer(UNITS_COST, utilityviewmodel))
                            {
                                return false;
                            }
                            break;
                        default:
                            break;
                    }
                    component_count++;
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
                                                                        utilityviewmodel.UNIT_CHARGES_PERIOD_START,
                                                                        utilityviewmodel.UNIT_CHARGES_PERIOD_END,
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
                sub_line_count++;
            }
            return true;
        }

        private static bool SectionH2_Standing_Charges(UtilityViewModel utilityviewmodel,
                                                        string[][] sections,
                                                        int section_index,
                                                        string[] lines,
                                                        ref int sub_line_count)
        {
            string standard_gas = "Standard Gas",
                   total_gas_charges = "Total gas charges",
                   total_gas_costs = "Total gas costs",
                   standing_charge_comma = "Standing charge,",
                   Standing_Charge = "Standing Charge",
                   your_gas_meter_point_reference_number = "Your gas meter point reference number";

            while (sub_line_count <= Convert.ToInt32(sections[section_index][5]))
            {
                utilityviewmodel.token = lines[sub_line_count].Trim();
                if ((utilityviewmodel.token.Contains(total_gas_charges)) ||
                    (utilityviewmodel.token.Contains(total_gas_costs)) ||
                    (utilityviewmodel.token.Contains(standard_gas)) ||
                    (utilityviewmodel.token.Contains(your_gas_meter_point_reference_number)))
                {
                    break;
                }

                if (SmartParseV2016.Token_Identify(utilityviewmodel, Standing_Charge, false) ||
                    SmartParseV2016.Token_Identify_Middle(utilityviewmodel, standing_charge_comma, false, ""))
                {
                    utilityviewmodel.STANDING_CHARGES_PERIOD_END = SmartParametersV2016.defaultDates;

                    string us_standing_charge = standing_charge_comma.Replace(SmartParametersV2016.space, SmartParametersV2016.underscore);
                    utilityviewmodel.token = utilityviewmodel.token.Replace(standing_charge_comma, us_standing_charge);
                    if (utilityviewmodel.token.Contains(Standing_Charge))
                    {
                        us_standing_charge = Standing_Charge.Replace(SmartParametersV2016.space, SmartParametersV2016.underscore);
                        utilityviewmodel.token = utilityviewmodel.token.Replace(Standing_Charge, us_standing_charge);
                        utilityviewmodel.token = SmartTimeV2016.ConvertDateTime(utilityviewmodel.BILL_PERIOD_START).ToString(SmartParametersV2016.ddmmmyyyyFormat) +
                                    SmartParametersV2016.space +
                                    SmartTimeV2016.ConvertDateTime(utilityviewmodel.BILL_PERIOD_END).ToString(SmartParametersV2016.ddmmmyyyyFormat) +
                                    SmartParametersV2016.space +
                                    utilityviewmodel.token;
                    }
                    utilityviewmodel.token = utilityviewmodel.token.Replace("to", "");
                    utilityviewmodel.token = utilityviewmodel.token.Replace(SmartParametersV2016.equivalent, "").Trim();
                    utilityviewmodel.token = SmartParseV2016.Remove_Double_Spaces_V3(utilityviewmodel.token);
                    string[] components = utilityviewmodel.token.Split(SmartParametersV2016.spaceSplit);
                    if (components.Length < 8)
                    {
                        sub_line_count++;
                        if (lines[sub_line_count].Contains(utilityviewmodel.bill_currency_symbol))
                        {
                            utilityviewmodel.token = utilityviewmodel.token + SmartParametersV2016.space + lines[sub_line_count].Trim();
                            components = utilityviewmodel.token.Split(SmartParametersV2016.spaceSplit);
                        }
                    }
                    string TEMP_DATE,// = "",
                            CHARGES_TYPE = "",
                            CHARGES_DAYS = "0",
                            STANDING_CHARGE = "0",
                            CHARGES_COST = "0";
                    int component_count = 0;
                    while (component_count < components.Length)
                    {
                        switch (component_count)
                        {
                            case 0:     // From date
                                TEMP_DATE = components[component_count].Trim();
                                if (!SmartParseV2016.Generic_Parse_Datetime(TEMP_DATE, utilityviewmodel))
                                {
                                    return false;
                                }
                                utilityviewmodel.STANDING_CHARGES_PERIOD_START = TEMP_DATE;
                                break;
                            case 1:     // To Date
                                TEMP_DATE = components[component_count].Trim();
                                if (!SmartParseV2016.Generic_Parse_Datetime(TEMP_DATE, utilityviewmodel))
                                {
                                    return false;
                                }
                                utilityviewmodel.STANDING_CHARGES_PERIOD_END = TEMP_DATE;
                                break;
                            case 2:     // Consumption_charge
                                CHARGES_TYPE = components[component_count].Replace(SmartParametersV2016.underscore, SmartParametersV2016.space);
                                CHARGES_TYPE = CHARGES_TYPE.Replace(SmartParametersV2016.comma, "").Trim();
                                CHARGES_TYPE = (CHARGES_TYPE.Length <= SmartParametersV2016.TYPESLENGTH ? CHARGES_TYPE : CHARGES_TYPE.Substring(0, SmartParametersV2016.TYPESLENGTH));
                                break;
                            case 3:
                                CHARGES_DAYS = components[component_count].Trim();
                                break;
                            case 4:     // days
                                break;
                            case 5:     // x
                                break;
                            case 6:     // Rate in p
                                STANDING_CHARGE = components[component_count].Trim();
                                STANDING_CHARGE = STANDING_CHARGE.Replace(utilityviewmodel.bill_denomination_symbol.ToString(), "");
                                break;
                            case 7:     // Total Cost
                                //int temp = 0;
                                if (!SmartParseV2016.Currency_Strip(utilityviewmodel.bill_currency_symbol, utilityviewmodel.bill_currency_separator, utilityviewmodel.bill_thousands_separator, components[component_count].Trim(), utilityviewmodel))
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
                                break;
                            default:
                                break;
                        }
                        component_count++;
                    }
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
                                                                            CHARGES_TYPE,
                                                                            STANDING_CHARGE,
                                                                            CHARGES_DAYS,
                                                                            CHARGES_COST);
                            break;
                        default:
                            break;
                    }
                }
                sub_line_count++;
            }
            return true;
        }

        private static bool Parse_SectionJ0(MainViewModel ourviewmodel,
                                            UtilityViewModel utilityviewmodel,
                                            string[][] sections,
                                            string section,
                                            string sub_section,
                                            string[] lines)
        {
            int section_index = 0;
            if (!SmartParseV2016.Decode_Section(section, sub_section, sections, utilityviewmodel.resource_code.ToString(), utilityviewmodel))
            {
                return true;
            }

            utilityviewmodel.token = "";
            string total_payments_received = "Total payments received",
                     payment = "Payment";
            utilityviewmodel.PAYMENT_AMOUNT = "";
            utilityviewmodel.PAYMENT_METHOD = "";
            utilityviewmodel.PAYMENT_DATE = SmartParametersV2016.defaultDates;
            utilityviewmodel.PAYMENT_BALANCE = "";

            DateTime temp_date = SmartParametersV2016.defaultDate;

            // Starting line count is a 'special'!!
            for (int line_count = Convert.ToInt32(sections[section_index][4]); line_count <= Convert.ToInt32(sections[section_index][5]); line_count++)
            {
                utilityviewmodel.token = lines[line_count].Trim();
                if (SmartParseV2016.Token_Identify(utilityviewmodel, total_payments_received +
                                                                SmartParametersV2016.bar +
                                                                "Total paid", false))
                {
                    break;
                }

                if (SmartParseV2016.Token_Identify(utilityviewmodel, payment, false) ||
                    SmartParseV2016.Token_Identify_Middle(utilityviewmodel, payment, false, ""))
                {
                    utilityviewmodel.PAYMENT_AMOUNT = "";
                    utilityviewmodel.PAYMENT_DATE = SmartParametersV2016.defaultDates;
                    utilityviewmodel.PAYMENT_METHOD = payment; // Because of the above test
                    utilityviewmodel.token = utilityviewmodel.token.Replace(payment, "").Trim();
                    utilityviewmodel.token = SmartParseV2016.Remove_Double_Spaces_V3(utilityviewmodel.token);

                    string[] components = utilityviewmodel.token.Split(SmartParametersV2016.spaceSplit);
                    int components_count = 0;
                    while (components_count < components.Length)
                    {
                        switch (components_count)
                        {
                            case 0:
                                utilityviewmodel.PAYMENT_DATE = components[components_count].Trim();
                                break;
                            case 1:
                                utilityviewmodel.PAYMENT_DATE = utilityviewmodel.PAYMENT_DATE + SmartParametersV2016.space + components[components_count].Trim();
                                break;
                            case 2:
                                utilityviewmodel.PAYMENT_DATE = utilityviewmodel.PAYMENT_DATE + SmartParametersV2016.space + components[components_count].Trim();
                                if (!SmartParseV2016.Generic_Parse_Datetime(utilityviewmodel.PAYMENT_DATE, utilityviewmodel))
                                {
                                    return false;
                                }
                                break;
                            case 3:
                                if (!SmartParseV2016.Currency_Strip(utilityviewmodel.bill_currency_symbol, utilityviewmodel.bill_currency_separator, utilityviewmodel.bill_thousands_separator, components[components_count], utilityviewmodel))
                                {
                                    return false;
                                }
                                utilityviewmodel.PAYMENT_AMOUNT = utilityviewmodel.value;

                                if (!SmartParseV2016.Generic_Parse_Integer(utilityviewmodel.PAYMENT_AMOUNT, utilityviewmodel))
                                {
                                    return false;
                                }
                                break;
                            default:
                                break;
                        }
                        components_count++;
                    }
                    utilityviewmodel.PAYMENT_CODE = "";
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
                        utilityviewmodel.payment_date = SmartNibbyV2016.ConvertDate(utilityviewmodel.PAYMENT_DATE,
                                                                                    SmartParametersV2016.defaultDate,
                                                                                    ourviewmodel);
                        if (ourviewmodel.errorMessage != "")
                        {
                            return false;
                        }

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

            // Fix the PAYMENT_DUE_DATE (if we can)
            if (temp_date != SmartParametersV2016.defaultDate)
            {
                // The best we can do - add 1 month to (the last) date
                string PAYMENT_DUE_DATE = temp_date.AddMonths(1).ToString();
                SmartParseV2016.Update_Bill(ourviewmodel, utilityviewmodel, "PAYMENT_DUE_DATE", PAYMENT_DUE_DATE);
                // Best we can do
                SmartParseV2016.Update_Bill(ourviewmodel, utilityviewmodel, "DIRECT_DEBIT_DATE", PAYMENT_DUE_DATE);
            }
            return true;
        }

        private static bool Parse_SectionK0(MainViewModel ourviewmodel,
                                            UtilityViewModel utilityviewmodel,
                                            string[][] sections,
                                            string section,
                                            string sub_section,
                                            string[] lines)
        {
            int section_index = 0;
            if (!SmartParseV2016.Decode_Section(section, sub_section, sections, utilityviewmodel.resource_code.ToString(), utilityviewmodel))
            {
                return true;
            }

            utilityviewmodel.token = "";
            string SUPPLY_TYPE = "",
                    SUPPLY_AMOUNT = "0",
                    SUPPLY_DUE_DATE,// = "",
                    SUPPLY_CREDIT_BILL;// = "";
            DateTime SUPPLY_DATE;// = SmartParametersV2016.defaultDate;
            short SUPPLY_VAT_CODE;// = SmartParametersV2016.zeroRateVatCode;
            string total = "Total";

            int TEMP = 0;

            // Starting line count is a 'special'!!
            for (int line_count = Convert.ToInt32(sections[section_index][4]); line_count <= Convert.ToInt32(sections[section_index][5]); line_count++)
            {
                utilityviewmodel.token = lines[line_count];
                if (SmartParseV2016.Token_Identify(utilityviewmodel, sections[section_index][3], true))
                {
                    // We have reached the buffers - just hope there is never a charge
                    // which starts with the word 'Total' !!!
                    goto the_old_daysK0;
                }

                if (SmartParseV2016.Token_Identify(utilityviewmodel, total, true))
                {
                    // We have reached the buffers - just hope there is never a charge
                    // which starts with the word 'Total' !!!
                    break;
                }
                // Discount
                int currency_index = utilityviewmodel.token.IndexOf(utilityviewmodel.bill_currency_symbol);
                if (currency_index >= 0)
                {
                    utilityviewmodel.token = utilityviewmodel.token.Replace(utilityviewmodel.bill_currency_symbol, SmartParametersV2016.bar.ToString() + utilityviewmodel.bill_currency_symbol);
                    string[] components = utilityviewmodel.token.Split(SmartParametersV2016.bar);
                    int components_count = 0;
                    while (components_count < components.Length)
                    {
                        switch (components_count)
                        {
                            case 0:
                                SUPPLY_TYPE = components[components_count].Trim();
                                break;
                            case 1:
                                if (!SmartParseV2016.Currency_Strip(utilityviewmodel.bill_currency_symbol, utilityviewmodel.bill_currency_separator, utilityviewmodel.bill_thousands_separator, components[components_count].Trim(), utilityviewmodel))
                                {
                                    return false;
                                }
                                else
                                {
                                    SUPPLY_AMOUNT = utilityviewmodel.value;
                                }
                                if (!SmartParseV2016.Generic_Parse_Integer(SUPPLY_AMOUNT, utilityviewmodel))
                                {
                                    return false;
                                }
                                else
                                {
                                    TEMP = utilityviewmodel.genericTransactionValue;
                                }
                                break;
                            default:
                                break;
                        }
                        components_count++;
                    }
                    if (TEMP != 0)
                    {
                        SUPPLY_DATE = SmartNibbyV2016.ConvertDate(utilityviewmodel.BILL_PERIOD_END,
                            SmartParametersV2016.defaultDate, ourviewmodel);
                        if (ourviewmodel.errorMessage != "")
                        {
                            return false;
                        }
                        if (SUPPLY_DATE != SmartParametersV2016.defaultDate &&
                            SUPPLY_AMOUNT != "0" &&
                            !string.IsNullOrEmpty(SUPPLY_TYPE))
                        {
                            SUPPLY_VAT_CODE = utilityviewmodel.Hezbollah.bills_row.BILL_VAT_CODE;
                            // Get some kind of discount date
                            SUPPLY_DUE_DATE = utilityviewmodel.BILL_PERIOD_END;
                            SUPPLY_CREDIT_BILL = utilityviewmodel.STATEMENT_ID;

                            utilityviewmodel.SUPPLY_CHARGES_CREDITS_ITEM = (short)(utilityviewmodel.SUPPLY_CHARGES_CREDITS_ITEM + 1);
                            // Why is it Supply Charges Credits and not E_Discouts or G_Discounts?
                            // Because ScottishhPower have a COMBINED Bill for both Electricity and Gas
                            if (!SmartParseV2016.Check_Supply_Is_There(ourviewmodel,
                                                                        utilityviewmodel,
                                                                        SUPPLY_DATE,
                                                                        utilityviewmodel.SUPPLY_CHARGES_CREDITS_ITEM,
                                                                        SUPPLY_TYPE,
                                                                        SUPPLY_VAT_CODE,
                                                                        Convert.ToInt32(SUPPLY_AMOUNT)))
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
                                                                                Convert.ToInt32(SUPPLY_AMOUNT),
                                                                                SmartTimeV2016.ConvertDateTime(SUPPLY_DUE_DATE),
                                                                                SUPPLY_CREDIT_BILL);
                                SmartParseV2016.Update_Bill(ourviewmodel, utilityviewmodel, "SUPPLY_CHARGES_CREDITS", TEMP.ToString());
                            }
                            else
                            {
                                // Something already in with no 'AMOUNT'? Time to check the VAT ...
                                utilityviewmodel.check_vat = true;
                            }
                        }
                    }
                }
            the_old_daysK0:
                continue;
            }
            return true;
        }

        private static bool Parse_SectionL0(MainViewModel ourviewmodel,
                                            UtilityViewModel utilityviewmodel,
                                            string[][] sections,
                                            string section,
                                            string sub_section,
                                            string[] lines)
        {
            int section_index = 0;
            if (!SmartParseV2016.Decode_Section(section, sub_section, sections, utilityviewmodel.resource_code.ToString(), utilityviewmodel))
            {
                return true;
            }

            utilityviewmodel.token = "";
            string total_other_charges = "Total other charges",
                    total_account_adjustments = "Total account adjustments",
                    ACCOUNT_AMOUNT = "0",
                    ACCOUNT_DATE,// = "",
                    ACCOUNT_TYPE,// = "",
                    INCLUDE_BILLS;// = "";
            short ACCOUNT_VAT_CODE = SmartParametersV2016.zeroRateVatCode;          // Default;
            //DateTime TEMP_DATE = SmartParametersV2016.defaultDate;
            int temp = 0;

            bool start_looking = false;
            // Starting line count is a 'special'!!
            for (int line_count = Convert.ToInt32(sections[section_index][4]); line_count <= Convert.ToInt32(sections[section_index][5]); line_count++)
            {
                utilityviewmodel.token = lines[line_count];
                if (SmartParseV2016.Token_Identify(utilityviewmodel, sections[section_index][3], true))
                {
                    start_looking = true;
                    goto the_old_daysL0;
                }

                if (SmartParseV2016.Token_Identify(utilityviewmodel, total_other_charges +
                                                                SmartParametersV2016.bar +
                                                                total_account_adjustments +
                                                                SmartParametersV2016.bar +
                                                                "Total (debit)" +
                                                                SmartParametersV2016.bar +
                                                                "Total (credit)", true))
                {
                    // We have reached the buffers - just hope there is never a charge
                    // which starts with the word 'Total' !!!
                    break;
                }


                if (start_looking)
                {
                    ACCOUNT_TYPE = "";

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
                            if (line_count > Convert.ToInt32(sections[section_index][5]))
                            {
                                goto the_old_daysL0;
                            }
                            utilityviewmodel.token = utilityviewmodel.token + SmartParametersV2016.space + lines[line_count].Trim();
                        }
                    }
                    if (SmartParseV2016.Look_Backwards(utilityviewmodel, SmartParametersV2016.space, SmartParametersV2016.bar.ToString()))
                    {
                        // Now we have  e.g. -|19?Sep?48|-£30.00 and can split on |
                        string[] components = utilityviewmodel.token.Split(SmartParametersV2016.bar);
                        int component_count = 0;
                        while (component_count < components.Length)
                        {
                            switch (component_count)
                            {
                                case 0:
                                    ACCOUNT_TYPE = components[component_count].Trim(SmartParametersV2016.dashSplit).Trim();
                                    break;
                                case 1:
                                    if (!SmartParseV2016.Currency_Strip(utilityviewmodel.bill_currency_symbol, utilityviewmodel.bill_currency_separator, utilityviewmodel.bill_thousands_separator, components[component_count].Trim(), utilityviewmodel))
                                    {
                                        return false;
                                    }
                                    else
                                    {
                                        ACCOUNT_AMOUNT = utilityviewmodel.value;
                                    }
                                    if (!SmartParseV2016.Generic_Parse_Integer(ACCOUNT_AMOUNT, utilityviewmodel))
                                    {
                                        return false;
                                    }
                                    else
                                    {
                                        temp = utilityviewmodel.genericTransactionValue;
                                    }
                                    break;
                                default:
                                    break;
                            }
                            component_count++;
                        }

                        if (temp != 0)
                        {
                            ACCOUNT_TYPE = (ACCOUNT_TYPE.Length <= SmartParametersV2016.TYPESLENGTH ? ACCOUNT_TYPE : ACCOUNT_TYPE.Substring(0, SmartParametersV2016.TYPESLENGTH));
                            ACCOUNT_DATE = utilityviewmodel.BILL_PERIOD_START;
                            if (!SmartParseV2016.Generic_Parse_Datetime(ACCOUNT_DATE, utilityviewmodel))
                            {
                                return false;
                            }
                            if (ACCOUNT_TYPE == "Cancelled VAT")
                            {
                                INCLUDE_BILLS = SmartParametersV2016.noFlag;
                            }
                            else
                            {
                                INCLUDE_BILLS = SmartParametersV2016.yesFlag;

                            }
                            // Need to increment this for multiple inserts on the same Bill date
                            utilityviewmodel.ACCOUNT_CHARGES_CREDITS_ITEM = (short)(utilityviewmodel.ACCOUNT_CHARGES_CREDITS_ITEM + 1);
                            bool is_it_there = false;
                            {
                                // Check if neither Electricity nor Gas
                                if (string.IsNullOrEmpty(sections[section_index][2]))
                                {
                                    is_it_there = SmartParseV2016.Check_Adjustment_Is_There(ourviewmodel,
                                                                                    utilityviewmodel,
                                                                                    SmartTimeV2016.ConvertDateTime(ACCOUNT_DATE),
                                                                                    utilityviewmodel.ACCOUNT_CHARGES_CREDITS_ITEM,
                                                                                    ACCOUNT_TYPE,
                                                                                    ACCOUNT_VAT_CODE,
                                                                                    Convert.ToInt32(ACCOUNT_AMOUNT));
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
                                                                                    Convert.ToInt32(ACCOUNT_AMOUNT),
                                                                                    INCLUDE_BILLS);
                                SmartParseV2016.Update_Bill(ourviewmodel, utilityviewmodel, "ACCOUNT_CHARGES_CREDITS", ACCOUNT_AMOUNT);
                                // Note: ADJUSTMENT_AMOUNT has already been checked as an Integer
                            }
                        }
                    }
                }
            the_old_daysL0:
                continue;
            }
            return true;
        }

        private static bool Parse_SectionM0(MainViewModel ourviewmodel,
                                            UtilityViewModel utilityviewmodel,
                                            string[][] sections,
                                            string section,
                                            string sub_section,
                                            string[] lines)
        {
            int section_index = 0;
            if (!SmartParseV2016.Decode_Section(section, sub_section, sections, utilityviewmodel.resource_code.ToString(), utilityviewmodel))
            {
                return true;
            }

            // Now the fat ugly bitch is snoring away on the settee ...
            utilityviewmodel.token = "";     // In case we can't find it
            string VAT = "VAT ",
                    VAT_at = "VAT at",
                    Vat_at = "Vat at",
                    BILL_VAT_AMOUNT;
            //int TEMP = 0;
            bool status = false;
            for (int line_count = Convert.ToInt32(sections[section_index][4]); line_count <= Convert.ToInt32(sections[section_index][5]); line_count++)
            {
                utilityviewmodel.token = lines[line_count].Trim();
                if (SmartParseV2016.Token_Identify(utilityviewmodel, VAT +
                                                                SmartParametersV2016.bar +
                                                                VAT_at +
                                                                SmartParametersV2016.bar +
                                                                Vat_at, true))
                {
                    int sub_line_count = line_count;
                    while (sub_line_count < Convert.ToInt32(sections[section_index][5]))
                    {
                        utilityviewmodel.token = lines[sub_line_count].Trim();
                        if (utilityviewmodel.token.Contains("Total"))
                        {
                            break;
                        }
                        if (utilityviewmodel.token.Contains(SmartParametersV2016.percent))    // Now shut the fuck up
                        {
                            string target_rate = "";
                            utilityviewmodel.token = utilityviewmodel.token.Replace("at", "");
                            utilityviewmodel.token = utilityviewmodel.token.Replace("on", "");
                            utilityviewmodel.token = SmartParseV2016.Remove_Double_Spaces_V3(utilityviewmodel.token);
                            string[] components = utilityviewmodel.token.Split(SmartParametersV2016.spaceSplit);
                            int components_count = 0;
                            while (components_count < components.Length)
                            {
                                switch (components_count)
                                {
                                    case 1:
                                        target_rate = components[components_count].Replace(SmartParametersV2016.percent, "");
                                        short BILL_VAT_CODE;// = SmartParametersV2016.zeroRateVatCode;
                                        if (!SmartParseV2016.Lookup_Vat_Code(target_rate, SmartTimeV2016.ConvertDateTime(utilityviewmodel.BILL_DATE), ourviewmodel.Blanche.vatRatesList, utilityviewmodel))
                                        {
                                            return false;
                                        }
                                        else
                                        {
                                            BILL_VAT_CODE = utilityviewmodel.vat_code;
                                        }
                                        SmartParseV2016.Update_Bill(ourviewmodel, utilityviewmodel, "BILL_VAT_CODE", BILL_VAT_CODE.ToString());
                                        break;
                                    case 3:
                                        if (!SmartParseV2016.Currency_Strip(utilityviewmodel.bill_currency_symbol, utilityviewmodel.bill_currency_separator, utilityviewmodel.bill_thousands_separator, components[components_count], utilityviewmodel))
                                        {
                                            return false;
                                        }
                                        else
                                        {
                                            BILL_VAT_AMOUNT = utilityviewmodel.value;
                                        }
                                        if (!SmartParseV2016.Generic_Parse_Integer(BILL_VAT_AMOUNT, utilityviewmodel))
                                        {
                                            return false;
                                        }
                                        // So TEMP contains VAT amount for both Electricity and Gas ...

                                        //
                                        // After 30 YEARS OF LIVING IN THE SAME HOUSE ... <= "What's our postcode?"
                                        //

                                        // Work out a value
                                        decimal TEMPDEC;
                                        if (!SmartParseV2016.Generic_Parse_Decimal(target_rate, utilityviewmodel))
                                        {
                                            return false;
                                        }
                                        else
                                        {
                                            TEMPDEC = utilityviewmodel.genericDecimalValue;
                                        }
                                        SmartParseV2016.Update_Bill(ourviewmodel, utilityviewmodel, "BILL_VAT_AMOUNT", TEMPDEC.ToString());
                                        status = true;
                                        break;
                                    default:
                                        break;
                                }
                                components_count++;
                            }
                        }
                        sub_line_count++;
                    }
                    line_count = sub_line_count;
                    goto the_old_days;
                }
            }
        the_old_days:
            return status;
        }
    }
}