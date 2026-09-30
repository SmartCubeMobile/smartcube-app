// ©Principal Research Corp Ltd 2010,2011,2012,2013,2014,2015,2016,2017,2018,2019
// No portion of this code may be copied or modified in any way, shape or form by
// any means whatsoever without the express written permission of the author.
using Newtonsoft.Json.Linq;
using SmartCubeMobile;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
//using Bridge;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SmartDashboard
{
    class USwitchV2019
    {
        internal static async Task<bool> DoGet_Utility(
#if WINFORMS
                                            System.Windows.Forms.RichTextBox textBoxConsole,
#endif
#if WPF
                                            TextBlock textBoxBrowser,
                                            ScrollViewer scrollViewer,
#endif
#if WINDOWS_UWP || __ANDROID__
                                            ScrollView scrollViewer,
#endif
                                            MainViewModel ourviewmodel,
                                            DashboardModel dashboardmodel,
                                            CancellationToken cancellation_token,
                                            Uri url,
                                            string url_base,
                                            bool log,
                                            string operation,
                                            string referer = "")
        {
            string FinanceLog = string.Empty;
            TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();

            StringContent empty = new StringContent(string.Empty);
            //XMLHttpRequest request = new XMLHttpRequest();

            //request.Open(operation, url.ToString());
            //request.WithCredentials = true;
            switch (operation)
            {
                case "GET":
                    //request.SetRequestHeader("Accept", "text/html,application/xhtml+xml,application/xml;q= 0.9,image/webp,image/apng,*/*;q=0.8");
                    //request.SetRequestHeader("Accept-Encoding", "gzip,deflate,br");
                    //request.SetRequestHeader("Accept-Language", "en-GB,en-US;q=0.9,en;q=0.8");
                    //// Boy! do we need this one ...
                    //request.SetRequestHeader("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/73.0.3683.103 Safari/537.36");
                    //request.SetRequestHeader("Upgrade-Insecure-Requests", "1");
                    //if (!string.IsNullOrEmpty(referer))
                    //{
                    //    request.SetRequestHeader("Referer", referer);
                    //}
                    break;
            }
            //string empty1 = "test";
            //request.Send(empty, ourviewmodel.cookies, url_base, cancellation_token);
            //            request.Onreadystatechange += () =>
            //            {
            //                if (request.ReadyState == 4)
            //                {
            //                    // If request.responseType is 'null' then there will be an error message in request.responseText
            //                    if (request.ResponseType == "uri")
            //                    {
            //                        dashboardmodel.response = request.ResponseText;
            //                    }
            //                    else
            //                    {
            //                        HtmlAgilityPack.HtmlDocument document = new HtmlAgilityPack.HtmlDocument();
            //                        document.LoadHtml(request.ResponseText);

            //                        dashboardmodel.htmlDocument = document;
            //                    }   // do some shit
            //                    dashboardmodel.type = request.ResponseType;
            //                    if (log)
            //                    {
            //#if WINFORMS
            //                        FinanceLog = FinanceLog + operation + " " + url.ToString() + Environment.NewLine.ToString();
            //                        //FinanceLog.ScrollToCaret();
            //#endif
            //                        FinanceLog = FinanceLog + operation + " " + url.ToString();
            //                    }
            //                    tcs.SetResult(true);
            //                }
            //            };
#pragma warning disable CA2007 // Do not directly await a Task
            if (await tcs.Task)
            {
                //request.Onreadystatechange -= () => { };
            }
#pragma warning restore CA2007 // Do not directly await a Task
#if !WINFORMS
            await SmartRoutinesV2018.TextBlockUpdate(
#if WINFORMS
                                                            textBoxConsole,
#endif
#if WPF
                                                            scrollViewer,
                                                            textBoxBrowser,
#endif
#if WINDOWS_UWP || __ANDROID__
                                                            scrollViewer,
#endif
                                                        ourviewmodel,
                                                        FinanceLog);
#endif
            return true;
        }

        public static async Task<bool> Read_Website(SqlConnection SmartUtilityConnection,
                                                    MainProcess components,
                                                    SignInViewModel signinviewmodel,
                                                    MainViewModel ourviewmodel,
                                                    DashboardModel dashboardmodel,
                                                    System.Windows.Forms.RichTextBox Finance_Log,
                                                    string start_routine,
                                                    bool multiple_shot,       // False = only do current, True = update index
                                                    bool special,           // True - loop round all suppliers collecting tariffs
                                                    string usw_filepath,
                                                    string usw_website,
                                                    string usw_website_resource,
                                                    DashBored Mally,
                                                    System.Windows.Forms.RichTextBox textBoxConsole,
                                                    string yymmdd_format,
                                                    char category_code,
                                                    char resource_code,
                                                    string resource_type,
                                                    string defaultDates,
                                                    DateTime defaultDate,
                                                    string brand_namex,
                                                    string dashboard_namex,
                                                    string scraper_name,
                                                    short supplier_code,
                                                    short brand_code,
                                                    short version_code,
                                                    DateTime final_date,
                                                    string tariff_name,
                                                    string alternative_tariff_name,
                                                    int tariff_code,
                                                    DateTime prices_valid_from,
                                                    decimal uplift,
                                                    string usw_area,
                                                    int target_row,         // The line we change re: its colour
                                                    int colourindex,        // The colour we change a line to
                                                    int defaultindex,       // Default line colour
                                                    int sam_count,          // Count of brand_matrix for each plan
                                                    SmartUtility.BrandMatrix brand_matrix_row,
                                                    CheckedListBox Brand_Matrix,
                                                    CheckedListBox Tariffs_Areas_Matrix,
                                                    CheckBox EnergyUpdates,
                                                    ListView Conditions_Groups_Tariff_Plans,
                                                    ListView Conditions_Plans_Limits,
                                                    ListView Payments_Tariff_Plans,
                                                    ComboBox comboBoxUSW,
                                                    ComboBox comboBoxUSW_Tariffs,
                                                    Label labelUSW_VATRate,
                                                    CheckBox CheckBoxDF,
                                                    string last_target)
        {
            string routine = MethodBase.GetCurrentMethod().Name.ToUpper();
            // The gurgling fawning laugh ... Mundae ...!!!

            List<string> headers = new List<string>();

            // To Find out what might have gone wrong
            string http_message = SmartNibbyV2016.Check_Timeout(ourviewmodel);
            if (http_message != string.Empty)
            {
                // The timeout has to be 0 < timeout <= 120 for us to continue ... and its not!
                MainProcess.Output_Message(textBoxConsole,
                                    http_message,
                                    Mally.examine.scrape, Mally.console);
                return false;
            }
            else
            {
                http_message = string.Empty;
            }

            dashboardmodel.connection_timeout = SmartParametersV2016.serverTimeoutSecs;
            dashboardmodel.brand_code = brand_code;
            dashboardmodel.supplier_code = supplier_code;

            bool use_proxy = false;
            TimeSpan time_difference = new TimeSpan(0, 0, 0, 0);
            //bool TextBox_Active = false;

            // Yes, as per usual ... my miserable, churning, insufferable 'wife'
            // spoilt yet another family outing with her petulant, selfish behaviour
            // yesterday 18th June 'Father's Day'.  She simply CANNOT ABIDE not being
            // the centre of attention.  She decided she would have a 'cob' on and
            // that is what she did.  Too rude to go round to Anna's and just slumped
            // in the chair facing away from me.  Then she came in the kitched and
            // spent the rest of the evening slamming doors.  She is such a BITCH
            // you wouldn't believe it.  No charm, no style, no manners, no grace,
            // no elegance, no feminism, just her own raw ugly self-aggrandisement.
            // How ditzy I am! How wacky I am! How funny I am!  How everyone at work
            // hangs on my every word!  How they all laugh with (at?) me at work!
            // IF ONLY THEY KNEW!

            // 12 April 2018 ... now (!!!) the stupid bitch was going out for a meal with her new friend
            // and her Zumba 'pals' when the silly bitches got the wrong date and turned up 24 hours too late!
            // Too stupid to check the date and the day!!!  Par for the course, eh, numbskull??

            // To carry cookies across all GET and POST calls
            // ... so the Cookies ARE IMPORTANT because clearing them down after
            // the login, loses the fact that you are Logged-in i.e. ACCSUMMY keeps
            // returning the Login page  YOU NEED THE FUCKING COOKIES
            //                           ============================

            // Because Sometimes we are returned a Document
            dashboardmodel.htmlDocument = new HtmlAgilityPack.HtmlDocument();
            HtmlAgilityPack.HtmlDocument html_ajax_document = new HtmlAgilityPack.HtmlDocument();

            List<KeyValuePair<string, string>> keyValues = new List<KeyValuePair<string, string>>();

            // Everytime timer ticks, timer_Tick will be called
            // Timer will tick evert second
            // FUCKING HELL - this is never Enabled????
            // You have 60 Seconds to stop
            // the timer on a good login!
            string next_routine = start_routine,
                    pathname = string.Empty,            // Used to target the next page
                    account_number = string.Empty,
                    page_number = string.Empty,
                    target_string = string.Empty;

            string midata_pathname = string.Empty;

            bool keep_looping = true;
            DateTime time_now = DateTime.Now;   // VERY IMPORTANT NOT TO VARY THIS IF USE_PROXT = TRUE
            string user_agent = string.Empty;

            while (keep_looping)
            {
                if (!use_proxy)
                {
                    // DON'T CHANGE THIS IF WE ARE SCRAPING BY PROXY - IT NEEDS RO BE CONSTANT
                    time_now = DateTime.Now.Add(time_difference);
                }

                // Could that fucking bitch be ANY noisier?  DOes she HAVE to turn the fucking
                // radio on?  Is she SO RUDE, THICK or INCONSIDERATE to ***ASK*** me BEFORE
                // she turns the fucking thing on???  ODSBD and IWBSN
                MainProcess.Output_Message(textBoxConsole,
                                    "Next routine " + next_routine + " at " + time_now.ToString(yymmdd_format) + " URL: " + usw_website_resource,
                                    Mally.examine.scrape, Mally.console);
                if (string.IsNullOrEmpty(usw_website_resource))
                {
                    // We can gt away with lots .. but not without this!!!
                    MainProcess.Output_Message(textBoxConsole,
                                       "Cannot continue with website null or empty ",
                                       Mally.examine.scrape, Mally.console);
                }

                // Over the top ... "IYA!!!!"  The word is HELLO you idiot
                // This fucking dumb thick idiotic moron has the cheek to stand in the kitchen
                // 13-Dec-2015 8pm and make faces at me behind my back - and then too stupid
                // to admit it she tries to laugh it off.  She couldn't even come up to the soles
                // of my feet intellectually.  This moron who can't read, write or even speak properly
                // has the gall, the impudence, the temerity to mock me?  She is STUPID BEYOND STUPID
                // and ODIBF.  ODSBD and IWHTPUWHS.

                string url_base = "https://www.uswitch.com";
                dashboardmodel.response = string.Empty;
                dashboardmodel.type = string.Empty;
                string referer = string.Empty;


                // Over the top ... "IYERRRRR!!!!"  The word is HELLO you idiot
                switch (next_routine)
                {
                    case "SETUP":
                        // The last piece of the jigsaw!!!!

                        target_string = usw_website;
                        Uri target_url = new Uri(target_string);

                        headers.Clear();

                        headers.Add("Accept" + SmartParametersV2016.unitSeparator + "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8");
                        //DNT: 1
                        //headers.Add("Accept-Encoding" + SmartParametersV2016.unitSeparator + "gzip, deflate, br");
                        headers.Add("Accept-Language" + SmartParametersV2016.unitSeparator + "en-GB,en-US;q=0.9,en;q=0.8");
                        headers.Add("Upgrade-Insecure-Requests" + SmartParametersV2016.unitSeparator + "1");
                        //headers.Add("User-Agent" + SmartParametersV2016.unitSeparator + "Mozilla/5.0(Windows NT 10.0; Win64; x64) AppleWebKit/537.36(KHTML, like Gecko) Chrome/65.0.3325.181 Safari/537.36");

                        // More fucking hoops
                        if (Mally.console)
                        {
                            // Synchronous for the Console application - use a lambda expression to
                            // help us return the 'void' type declared for HTTP_GET_ASYNC
                            dashboardmodel.htmlDocument = SmartBobV2017.HTTPCLIENT_GET_SYNC_SPECIAL(ourviewmodel,
                                                                                                        dashboardmodel.utilityToken,
                                                                                                        target_url);
                        }
                        else
                        {
                            Uri next_url = new Uri(url_base);  // Response_itself shouldn't be empty
                            bool log = true;
                            // Asynchronous for the Form application
                            if (await DoGet_Utility(
                                        textBoxConsole,
                                        ourviewmodel,
                                        dashboardmodel,
                                        dashboardmodel.utilityToken,
                                        next_url,
                                        url_base,
                                        log,
                                        "GET",
                                        referer))
                            {
                                textBoxConsole.AppendText("First strike!" + Environment.NewLine.ToString());
                                textBoxConsole.ScrollToCaret();
                            }
                        }
                        if (!string.IsNullOrEmpty(http_message))
                        {
                            MainProcess.Output_Message(textBoxConsole,
                                         http_message,
                                         Mally.examine.scrape, Mally.console);
                            keep_looping = false;
                        }
                        if (dashboardmodel.htmlDocument.RemainderOffset == 0)
                        {
                            keep_looping = false;
                        }
                        else
                        {
                            // Find the authenticity token

                            dashboardmodel.login_finished = true;
                            next_routine = "LOGOUT";    // true or false we logout
                            //}
                        }
                        break;
                    case "LOGIN":

                        dashboardmodel.login_finished = true;
                        bool prices_include_vat = false;    // <==== ||||||||| Its different for Uswitch !!!!!!

                        // ==========> THESE ARE DEFAULTED here <===========
                        short tier_count = 1;
                        short tier_level = 1;

                        // Does this fucking cow EVER???? SHUT THE FUCK UP ABOUT HER FUCKING JOB???
                        // She brings her stupid fucking job into EVERY fucking conversation.
                        // Every fucking one

                        // This program is little short of brilliant.  I have just loaded THREE
                        // payment types for each of Electricity, Economy7 and Gas (so that's NINE
                        // in total) ... and they all appear without error!  
                        // What a program!  What a PROGRAMMER!!!!

                        // Find the vat rate
                        dashboardmodel.VAT_RATE = 0.0M;
                        bool foundIt = false;
                        // A copy of the version in SmartParseV2016.Lookup_Vat_Rate
                        foreach (SmartMain.VatRates vat_rates_row in ourviewmodel.Blanche.vatRatesList)
                        {
                            if (vat_rates_row.VAT_CODE == SmartParametersV2016.currentVatCode)
                            {
                                DateTime valid_from = vat_rates_row.VALID_FROM;
                                DateTime valid_to = vat_rates_row.VALID_TO;

                                // Find a VAT rate which covers 'from' and 'to'
                                if ((SmartRoutinesV2018.DateTimeCompare(valid_from, prices_valid_from) <= 0) &&
                                    (SmartRoutinesV2018.DateTimeCompare(prices_valid_from, valid_to) <= 0))
                                {
                                    dashboardmodel.VAT_RATE = SmartRoutinesV2018.ConvertDecimal(vat_rates_row.VAT_RATE);
                                    foundIt = true;
                                    break;
                                }
                            }
                        }
                        //if (!SmartParseV2016.Lookup_Vat_Rate(SmartParametersV2016.currentVatCode,
                        //                                    prices_valid_from, 
                        //                                    prices_valid_from, 
                        //                                    ourviewmodel.Blanche.vatRatesList, 
                        //                                    utilityviewmodel))
                        if (!foundIt)
                        {
                            MainProcess.Output_Message(textBoxConsole,
                                                "==> Vat Rate: " + dashboardmodel.VAT_RATE + " is zero!",
                                                Mally.examine.scrape, Mally.console);
                            return false;
                        }
                        if (dashboardmodel.VAT_RATE == 0.0M)
                        {
                            labelUSW_VATRate.ForeColor = System.Drawing.Color.Red;
                        }
                        else
                        {
                            labelUSW_VATRate.ForeColor = System.Drawing.Color.LawnGreen;
                        }
                        labelUSW_VATRate.Text = dashboardmodel.VAT_RATE.ToString() + "%";

                        MainProcess.Output_Message(textBoxConsole,
                            "Supplier name " + brand_namex,
                            Mally.examine.scrape, Mally.console);

                        MainProcess.Output_Message(textBoxConsole,
                                           "Tariff: " + tariff_name,
                                            Mally.examine.scrape, Mally.console);

                        // This is the tough part ... how do we know the Tariff method is SC??
                        // Think about this Ray, its either SC or 2Tier ... but how do you tell which?

                        string path = Path.Combine(usw_filepath, brand_namex);
                        // When resource_code = "D" that means Dual-Rate i.e. E7
                        if (!File.Exists(path))
                        {
                            // Create a file to write to. 
                            Directory.CreateDirectory(path);
                        }
                        dashboardmodel.errorMessage = string.Empty;
                        string file_name = tariff_name.Replace("/", "-");   // slashes "/" are bad
                        file_name = file_name.Replace(":", "_");            // as are colons ":" 
                        string resource = string.Empty;
                        switch (resource_code)
                        {
                            case SmartParametersV2016.Electricity:
                                if (resource_type == "VR")
                                {
                                    path = Path.Combine(path, resource_code + "7" + "_" + file_name + ".txt");
                                    resource = "E7";
                                }
                                else
                                {
                                    path = Path.Combine(path, resource_code + "_" + file_name + ".txt");
                                    resource = "E";
                                }
                                break;
                            case SmartParametersV2016.Gas:
                                path = Path.Combine(path, resource_code + "_" + file_name + ".txt");
                                resource = "G";
                                break;
                            default:
                                break;
                        }

                        StreamWriter stream_writer = null;
                        if (!File.Exists(path))
                        {
                            // Create a file to write to. 
                            stream_writer = File.CreateText(path);
                            stream_writer.WriteLine("// USwitch Created: " + DateTime.Now.ToString("dd-MMM-yyyy HH:mm:ss") + " ..you ARE a fucking genius, Ray!");
                            stream_writer.Close();
                        }
                        stream_writer = new StreamWriter(path, true);

                        stream_writer.WriteLine("Supplier Name: " + brand_namex);
                        stream_writer.WriteLine("Tariff Name: " + tariff_name);
                        stream_writer.WriteLine("Prices valid from: " + prices_valid_from.ToString());
                        stream_writer.WriteLine("Prices exclude VAT");

                        // And people aren't OBEAST you dick-head they are OBESE
                        // And it was JOHN Atkinson who had dementia not fucking RON Atkinson you moron
                        //
                        // The pain of this program is UNRELENTING .. but its not a DAMP SQUID you
                        // stupid fucking bitch. Squids ARE ALWAYS FUCKING DAMP BECAUSE THEY FUCKING
                        // LIVE IN THE FUCKING SEA....   YOu mean a **DAMP S-Q-U-I-B *** You Stupid
                        // idiotic moronic fucking cow

                        string payment_type = string.Empty,
                                payment_name = string.Empty;
                        string payment_plan = "A";  // Might go in as "A" and come out as "C"

                        string derived_payment_plan = payment_plan;  // Might go in as "A" and come out as "C"

                        ObservableCollection<SmartUtility.PaymentPlans> payment_plans_found = DashboardUtilityV2018.Lookup_Payment_Plan(textBoxConsole, SmartUtilityConnection, payment_plan);
                        if (payment_plans_found.Count == 0)
                        {
                            MainProcess.Output_Message(textBoxConsole,
                                                resource + " Payment Plan: " + payment_plan + " " + "not found error",
                                                Mally.examine.scrape, Mally.console);
                            return false;
                        }
                        else
                        {
                            payment_type = payment_plans_found.First().PAYMENT_CODE;
                            payment_name = payment_plans_found.First().PAYMENT_NAME;
                        }

                        stream_writer.WriteLine();
                        stream_writer.WriteLine("Payment Plan: " + payment_name);
                        //bool at_the_top = true;

                        keyValues.Clear();

                        decimal[,,] unit_rates = new decimal[14, 2, 5]; // Tier 1 and Tier 2

                        bool payment_type_found = false;

                        string[] fields = new string[0];
                        ObservableCollection<SmartUtility.SupplyAreas> supply_areas_found = SmartDatabaseV2016.READ_Records<SmartUtility.SupplyAreas>(textBoxConsole,
                                                                                            SmartUtilityConnection,
                                                                                            MainProcess.global_utility_tablesList,
                                                                                            fields,
                                                                                            string.Empty,
                                                                                            new SmartUtility.SupplyAreas());
                        foreach (SmartUtility.SupplyAreas supply_areas_row in supply_areas_found)
                        {
                            // Leave this IN HERE otherwise the scrape for E SR and E VR
                            // gets confused!!!  If this IS IN, then we have separate
                            // cookie containers for EACH of the scrapes and that WOEKS!!

                            string authenticity_token = string.Empty,
                                    uscc = string.Empty;

                            string postcode = supply_areas_row.POSTCODE;

                            if (postcode != string.Empty)
                            {
                                short area_code = supply_areas_row.AREA_CODE;

                                target_string = SmartParametersV2016.uswitchWebsite + "gas-electricity/";
                                target_url = new Uri(target_string);

                                headers.Clear();

                                // Leave these in here in case we ever need 'em
                                // For reasons I DO understand, the "User-Agent" header is read-only so I can't set it
                                // For reasons I DON'T understand, the "Accept-Encoding" header ensures Fiddler DOESN'T decode
                                // the incoming result; however if I leave the "Accept-ENcoding" OUT, then it does get decoded!!  Go figure!
                                headers.Add("Accept" + SmartParametersV2016.unitSeparator + "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8");
                                headers.Add("Accept-Language" + SmartParametersV2016.unitSeparator + "en-GB,en-US;q=0.9,en;q=0.8");
                                headers.Add("Upgrade-Insecure-Requests" + SmartParametersV2016.unitSeparator + "1");
                                headers.Add("Referer" + SmartParametersV2016.unitSeparator + SmartParametersV2016.uswitchWebsite);
                                headers.Add("Cache-Control" + SmartParametersV2016.unitSeparator + "no-cache, no-store, max-age=0, must-revalidate");

                                if (Mally.console)
                                {
                                    // Synchronous for the Console application - use a lambda expression to
                                    // help us return the 'void' type declared for HTTP_GET_ASYNC
                                    dashboardmodel.htmlDocument = SmartBobV2017.HTTPCLIENT_GET_SYNC_SPECIAL(ourviewmodel,
                                                                                                                dashboardmodel.utilityToken,
                                                                                                                target_url);
                                }
                                else
                                {
                                    bool log = true;
                                    Uri next_url = new Uri(target_string);
                                    // Asynchronous for the Form application
                                    if (await DoGet_Utility(textBoxConsole,
                                        ourviewmodel,
                                        dashboardmodel,
                                        dashboardmodel.utilityToken,
                                        next_url,
                                        url_base,
                                        log,
                                        "GET",
                                        referer))
                                    {
                                        textBoxConsole.AppendText("Second strike!" + Environment.NewLine.ToString());
                                        textBoxConsole.ScrollToCaret();

                                    }

                                }
                                if (!string.IsNullOrEmpty(http_message))
                                {
                                    MainProcess.Output_Message(textBoxConsole,
                                           "Error 1: " + http_message,
                                            Mally.examine.scrape, Mally.console);
                                    keep_looping = false;
                                }
                                if (dashboardmodel.htmlDocument.RemainderOffset == 0)
                                {
                                    keep_looping = false;
                                }
                                else
                                {
                                    if (!Find_USCC(Mally,
                                                     textBoxConsole,
                                                     dashboardmodel.htmlDocument,
                                                     ref uscc))
                                    {
                                        MainProcess.Output_Message(textBoxConsole,
                                                        "Error 2: " + "Can't find uscc",
                                                        Mally.examine.scrape, Mally.console);
                                        return false;
                                    }
                                    else
                                    {
                                        string token = string.Empty;
                                        if (!Find_Token(Mally,
                                                    textBoxConsole,
                                                    dashboardmodel.htmlDocument,
                                                    ref token))
                                        {
                                            MainProcess.Output_Message(textBoxConsole,
                                                            "Error 3: " + "Can't find authorization token",
                                                            Mally.examine.scrape, Mally.console);
                                            return false;
                                        }
                                        else
                                        {
                                            target_string = SmartParametersV2016.uswitchWebsite + "gas-electricity/beta-api/resources/v1/regions?postcode=" +
                                                                Uri.EscapeDataString(supply_areas_row.POSTCODE) +
                                                                "&" + Uri.EscapeDataString("identity[uscc]") + "=" + Uri.EscapeDataString(uscc) +
                                                                "&" + Uri.EscapeDataString("identity[userAgent]") + "=" +
                                                                Uri.EscapeDataString("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/63.0.3239.132 Safari/537.36");
                                            target_url = new Uri(target_string);

                                            if (Mally.console)
                                            {
                                                // Synchronous for the Console application - use a lambda expression to
                                                // help us return the 'void' type declared for HTTP_GET_ASYNC
                                                dashboardmodel.htmlDocument = SmartBobV2017.HTTPCLIENT_GET_SYNC_SPECIAL(ourviewmodel,
                                                                                                                            dashboardmodel.utilityToken,
                                                                                                                            target_url);
                                            }
                                            else
                                            {
                                                // Asynchronous for the Form application
                                                bool log = true;
                                                Uri next_url = new Uri(target_string);
                                                // Asynchronous for the Form application
                                                if (await DoGet_Utility(textBoxConsole,
                                                    ourviewmodel,
                                                    dashboardmodel,
                                                    dashboardmodel.utilityToken,
                                                    next_url,
                                                    url_base,
                                                    log,
                                                    "GET",

                                                    referer))
                                                {
                                                    textBoxConsole.AppendText("Third strike!" + Environment.NewLine.ToString());
                                                    textBoxConsole.ScrollToCaret();

                                                }

                                            }
                                            if (!string.IsNullOrEmpty(http_message))
                                            {
                                                MainProcess.Output_Message(textBoxConsole,
                                                       "Error 4: " + http_message,
                                                        Mally.examine.scrape, Mally.console);
                                                keep_looping = false;
                                            }
                                            if (dashboardmodel.htmlDocument.RemainderOffset == 0)
                                            {

                                                keep_looping = false;
                                            }
                                            string json_string = string.Empty;
                                            target_string = SmartParametersV2016.uswitchWebsite + "gas-electricity/beta-api/resources/v1/suppliers?regionId=" + supply_areas_row.AREA_CODE.ToString();

                                            switch (resource_code)
                                            {
                                                case SmartParametersV2016.Electricity:
                                                    if (CheckBoxDF.Checked)
                                                    {
                                                        target_string = target_string + "&fuel=dual_fuel";
                                                    }
                                                    else
                                                    {
                                                        target_string = target_string + "&fuel=electricity";
                                                    }
                                                    break;
                                                case SmartParametersV2016.Gas:
                                                    target_string = target_string + "&fuel=dual_fuel"; // "&fuel=gas";
                                                    break;
                                                default:
                                                    break;
                                            }
                                            target_url = new Uri(target_string);

                                            if (Mally.console)
                                            {
                                                // Synchronous for the Console application - use a lambda expression to
                                                // help us return the 'void' type declared for HTTP_GET_ASYNC
                                                dashboardmodel.htmlDocument = SmartBobV2017.HTTPCLIENT_GET_SYNC_SPECIAL(ourviewmodel,
                                                                                                                            dashboardmodel.utilityToken,
                                                                                                                            target_url);
                                            }
                                            else
                                            {
                                                bool log = true;
                                                Uri next_url = new Uri(target_string);
                                                // Asynchronous for the Form application
                                                if (await DoGet_Utility(textBoxConsole,
                                                    ourviewmodel,
                                                    dashboardmodel,
                                                    dashboardmodel.utilityToken,
                                                    next_url,
                                                    url_base,
                                                    log,
                                                    "GET",
                                                    referer))
                                                {
                                                    textBoxConsole.AppendText("Fourth strike!" + Environment.NewLine.ToString());
                                                    textBoxConsole.ScrollToCaret();
                                                }

                                                dynamic jsonResponse = JObject.Parse(dashboardmodel.response);
                                                // Asynchronous for the Form application
                                                //json_string = await SmartBobV2017.HTTPCLIENT_GET_ASYNC_JSON(timespanTimeout,
                                                //                                            target_url,
                                                //                                            hm => http_message = hm,
                                                //                                            false,
                                                //                                            string.Empty,
                                                //                                            string.Empty,
                                                //                                            token);
                                                json_string = jsonResponse;
                                            }
                                            if (!string.IsNullOrEmpty(http_message))
                                            {
                                                MainProcess.Output_Message(textBoxConsole,
                                                        "Error 17: " + http_message,
                                                        Mally.examine.scrape, Mally.console);
                                                keep_looping = false;
                                            }
                                            if (string.IsNullOrEmpty(json_string))
                                            {
                                                keep_looping = false;
                                            }
                                            else
                                            {
                                                string keyName = string.Empty;
                                                if (USW_Decipher_Supplier_Key(json_string,
                                                                            Mally,
                                                                        textBoxConsole,
                                                                        scraper_name,
                                                                        ref keyName))

                                                {
                                                    target_string = SmartParametersV2016.uswitchWebsite + "gas-electricity/beta-api/resources/v1/plans";
                                                    target_string = target_string + "?regionId=" + supply_areas_row.AREA_CODE.ToString();

                                                    switch (resource_code)
                                                    {
                                                        case SmartParametersV2016.Electricity:
                                                            if (CheckBoxDF.Checked)
                                                            {
                                                                target_string = target_string + "&fuel=dual_fuel";
                                                            }
                                                            else
                                                            {
                                                                target_string = target_string + "&fuel=electricity";
                                                            }
                                                            break;
                                                        case SmartParametersV2016.Gas:
                                                            target_string = target_string + "&fuel=dual_fuel"; //  "&fuel=gas";
                                                            break;
                                                        default:
                                                            break;
                                                    }

                                                    target_string = target_string + "&paymentMethod=" + Uri.EscapeDataString("Monthly Direct Debit");

                                                    target_string = target_string + "&supplierKey=" + keyName;
                                                    //target_string = target_string + "&variant=Standard";

                                                    switch (resource_type)
                                                    {
                                                        case "SR":
                                                            target_string = target_string + "&variant=Standard";
                                                            break;
                                                        case "VR":
                                                            target_string = target_string + "&variant=" + Uri.EscapeDataString("Economy 7");
                                                            break;
                                                        default:
                                                            break;
                                                    }
                                                    target_url = new Uri(target_string);

                                                    if (Mally.console)
                                                    {
                                                        // Synchronous for the Console application - use a lambda expression to
                                                        // help us return the 'void' type declared for HTTP_GET_ASYNC
                                                        dashboardmodel.htmlDocument = SmartBobV2017.HTTPCLIENT_GET_SYNC_SPECIAL(ourviewmodel,
                                                                                                                                    dashboardmodel.utilityToken,
                                                                                                                                    target_url);
                                                    }
                                                    else
                                                    {
                                                        // Asynchronous for the Form application
                                                        bool log = true;
                                                        Uri next_url = new Uri(target_string);
                                                        // Asynchronous for the Form application
                                                        if (await DoGet_Utility(textBoxConsole,
                                                            ourviewmodel,
                                                            dashboardmodel,
                                                            dashboardmodel.utilityToken,
                                                            next_url,
                                                            url_base,
                                                            log,
                                                            "GET",
                                                            referer))
                                                        {
                                                            textBoxConsole.AppendText("Fourth strike!" + Environment.NewLine.ToString());
                                                            textBoxConsole.ScrollToCaret();

                                                        }

                                                        dynamic jsonResponse = JObject.Parse(dashboardmodel.response);

                                                        //json_string = await SmartBobV2017.HTTPCLIENT_GET_ASYNC_JSON(timespanTimeout,
                                                        //                target_url,
                                                        //                hm => http_message = hm,
                                                        //                false,
                                                        //                string.Empty,
                                                        //                string.Empty,
                                                        //                token);

                                                        json_string = jsonResponse;

                                                        if (!string.IsNullOrEmpty(http_message) ||
                                                            (string.IsNullOrEmpty(json_string)))
                                                        {
                                                            // <= Contains the status in 'urgent_message'
                                                            // Reset this
                                                            http_message = string.Empty;
                                                        }
                                                    }

                                                    if (string.IsNullOrEmpty(json_string) ||
                                                        (json_string == "[]"))  // <= Sometimes this comes back when USwitch has nothing
                                                    {
                                                        MainProcess.Output_Message(textBoxConsole,
                                                                    "Error 18: " + http_message,
                                                                    Mally.examine.scrape, Mally.console);
                                                        keep_looping = false;
                                                    }
                                                    else
                                                    {
                                                        string elec_standing_charge = "0.0",
                                                                    elec_day_rate = "0.0",
                                                                    elec_night_rate = "0.0",
                                                                    gas_standing_charge = "0.0",
                                                                    gas_day_rate = "0.0",
                                                                    gas_night_rate = "0.0";

                                                        if (USW_Decipher(json_string,
                                                                        Mally,
                                                                        textBoxConsole,
                                                                        tariff_name,
                                                                        ref payment_type_found,
                                                                        ref elec_standing_charge,
                                                                        ref elec_day_rate,
                                                                        ref elec_night_rate,
                                                                        ref gas_standing_charge,
                                                                        ref gas_day_rate,
                                                                        ref gas_night_rate))
                                                        {

                                                            string rhs = string.Empty;
                                                            string tcr = "0.0";

                                                            switch (resource_code)
                                                            {
                                                                case SmartParametersV2016.Electricity:
                                                                    MainProcess.Output_Message(textBoxConsole,
                                                                    supply_areas_row.POSTCODE + " " +
                                                                    "Area: " +
                                                                    area_code + " " +
                                                                    elec_standing_charge + " " +
                                                                    elec_day_rate + " " +
                                                                    elec_night_rate + " " +
                                                                    tcr,
                                                                    Mally.examine.scrape, Mally.console);
                                                                    stream_writer.WriteLine(area_code + "\t" +
                                                                                        elec_standing_charge + "\t" +
                                                                                        elec_day_rate + "\t" +
                                                                                        elec_night_rate + "\t" +
                                                                                        tcr);
                                                                    rhs = area_code.ToString() + " " +
                                                                            elec_standing_charge + " " +
                                                                            elec_day_rate + " " +
                                                                            elec_night_rate + " " +
                                                                            tcr;      // <= TCR
                                                                    if (CheckBoxDF.Checked)
                                                                    {
                                                                        MainProcess.Output_Message(textBoxConsole,
                                                                        supply_areas_row.POSTCODE + " " +
                                                                        "Area: " +
                                                                        area_code + " " +
                                                                        gas_standing_charge + " " +
                                                                        gas_day_rate + " " +
                                                                        gas_night_rate + " " +
                                                                        tcr,
                                                                        Mally.examine.scrape, Mally.console);
                                                                        stream_writer.WriteLine(area_code + "\t" +
                                                                                        gas_standing_charge + "\t" +
                                                                                        gas_day_rate + "\t" +
                                                                                        gas_night_rate + "\t" +
                                                                                        tcr);
                                                                    }
                                                                    break;
                                                                case SmartParametersV2016.Gas:
                                                                    MainProcess.Output_Message(textBoxConsole,
                                                                        supply_areas_row.POSTCODE + " " +
                                                                        "Area: " +
                                                                        area_code + " " +
                                                                        gas_standing_charge + " " +
                                                                        gas_day_rate + " " +
                                                                        gas_night_rate + " " +
                                                                        tcr,
                                                                        Mally.examine.scrape, Mally.console);
                                                                    stream_writer.WriteLine(area_code + "\t" +
                                                                                        gas_standing_charge + "\t" +
                                                                                        gas_day_rate + "\t" +
                                                                                        gas_night_rate + "\t" +
                                                                                        tcr);
                                                                    rhs = area_code.ToString() + " " +
                                                                            gas_standing_charge + " " +
                                                                            gas_day_rate + " " +
                                                                            gas_night_rate + " " +
                                                                            tcr;      // <= TCR
                                                                    break;
                                                                default:
                                                                    break;
                                                            }



                                                            // We need to update the Tariffs Matrix for all areas found
                                                            // and then the Suppliers Matrix
                                                            // Here
                                                            if (payment_type_found)
                                                            {
                                                                // CHECK
                                                                string[] comp = rhs.Split(' ');
                                                                if (comp.Count() != 5)
                                                                {
                                                                    MainProcess.Output_Message(textBoxConsole,
                                                                            resource + " Not enough components to proceed: " + rhs + " " + dashboardmodel.errorMessage + " continuing ...",
                                                                            Mally.examine.scrape, Mally.console);
                                                                    comp = rhs.Split(',');
                                                                    if (comp.Count() != 5)
                                                                    {
                                                                        MainProcess.Output_Message(textBoxConsole,
                                                                            resource + " Not enough components to proceed: " + rhs + " " + dashboardmodel.errorMessage,
                                                                            Mally.examine.scrape, Mally.console);
                                                                        return false;
                                                                    }
                                                                    else
                                                                    {
                                                                        rhs = rhs.Replace(",", " ");
                                                                    }
                                                                }
                                                                // true ... but we might not have found the payment type
                                                                if (energylinxV2016.Blah(rhs, ref unit_rates, area_code, prices_include_vat, dashboardmodel.VAT_RATE, ref tier_count))
                                                                {
                                                                    // If E7 then say we found at least one
                                                                    if (resource == "E")
                                                                    {
                                                                        dashboardmodel.one_e_found = true;
                                                                    }
                                                                    else
                                                                    {
                                                                        if (resource == "E7")
                                                                        {
                                                                            dashboardmodel.one_e7_found = true;
                                                                        }
                                                                    }
                                                                }
                                                                else
                                                                {
                                                                    // Blah failed - set payment code not found
                                                                    payment_type_found = false;
                                                                    break;
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }

                                // Well W-A-G waddled off to bed lst night as per usual.
                                // Truculent, petulent, sexless, unfeminine, ugly, smelly ..
                            }
                        }  // End of USwitch Regions loop

                        stream_writer.Close();


                        // Does this fucking cow EVER???? SHUT THE FUCK UP ABOUT HER FUCKING JOB???
                        // She brings her stupid fucking job into EVERY fucking conversation.
                        // Every fucking one

                        // This program is little short of brilliant.  I have just loaded THREE
                        // payment types for each of Electricity, Economy7 and Gas (so that's NINE
                        // in total) ... and they all appear without error!  
                        // What a program!  What a PROGRAMMER!!!!

                        // Now lets see if I can store them away
                        if (payment_type_found)
                        {
                            dashboardmodel.errorMessage = string.Empty;
                            // You have to check for INDIVIDUAL payment plans because with EL
                            // we cannot know (in advance) which prices are combined into which
                            // payment plans (unlike before when I could look ar an Excel spreadsheet
                            // and read off which ones apply to a set of prices)
                            // So:
                            //      If an individual plan DOESN'T exist - add it
                            //      If an individual plan DOES exist - then use it, its the Date
                            //          in the Groupings table which then applies
                            //
                            // Fix up the name if we found a different plam
                            if (derived_payment_plan != payment_plan)
                            {
                                ObservableCollection<SmartUtility.PaymentPlans> derived_payment_plans_found = DashboardUtilityV2018.Lookup_Payment_Plan(textBoxConsole, SmartUtilityConnection, derived_payment_plan);
                                if (derived_payment_plans_found.Count == 0)
                                {
                                    MainProcess.Output_Message(textBoxConsole,
                                                        resource + " Supplier: " + payment_name + " " + "derived Payment Plan error",
                                                        Mally.examine.scrape, Mally.console);
                                    return false;
                                }
                                else
                                {
                                    payment_name = derived_payment_plans_found.First().PAYMENT_NAME;
                                }
                            }

                            dashboardmodel.tariff_plans_row = new SmartUtility.TariffPlans();
                            if (!DashboardUtilityV2018.Add_Supplier_Plans(dashboardmodel,
                                                                        textBoxConsole,
                                                                        SmartUtilityConnection,
                                                                        true,
                                                                        category_code,
                                                                        supplier_code,
                                                                        derived_payment_plan,
                                                                        payment_name,
                                                                        defaultDates,
                                                                        Payments_Tariff_Plans))
                            {
                                MainProcess.Output_Message(textBoxConsole,
                                                        resource + " Supplier: " + brand_namex + " " + dashboardmodel.errorMessage,
                                                        Mally.examine.scrape, Mally.console);
                                dashboardmodel.login_finished = false;
                                break;
                            }
                            else
                            {
                                MainProcess.Output_Message(textBoxConsole,
                                                        resource + " Supplier: " + brand_namex + " " + dashboardmodel.errorMessage,
                                                        Mally.examine.scrape, Mally.console);
                                // Create the new Plans (clears down Conditions_Groups_Tariff_Plans)
                                DashboardUtilityV2018.Add_Tariff_Plans(dashboardmodel,
                                                                    textBoxConsole,
                                                                    SmartUtilityConnection,
                                                                    supplier_code,
                                                                    brand_namex,
                                                                    Conditions_Groups_Tariff_Plans);

                                dashboardmodel.drop_down_text = string.Empty;
                                // 'Fix up' the plans we have chosen
                                if (Set_Tariff_Plans(derived_payment_plan,
                                                        payment_name,
                                                        Payments_Tariff_Plans,
                                                        Conditions_Groups_Tariff_Plans))
                                {
                                    // This is the name IF IT doesn't exist - if it DOES EXIST then use the existing name
                                    dashboardmodel.selection_name = tariff_name + " (" + derived_payment_plan + ")";
                                    dashboardmodel.errorMessage = string.Empty;
                                    dashboardmodel.payment_code = 0;
                                    dashboardmodel.payment_plans = string.Empty;

                                    // First parameter means PAYMENT code additions are restricted
                                    dashboardmodel.conditions_plans_row = new SmartUtility.ConditionsPlans();
                                    if (!DashboardUtilityV2018.Add_Conditions_Plans(dashboardmodel,
                                                                                    textBoxConsole,
                                                                                    SmartUtilityConnection,
                                                                                    false,
                                                                                    supplier_code,
                                                                                    resource_code,
                                                                                    resource_type,
                                                                                    tariff_code,
                                                                                    version_code,
                                                                                    defaultDates,
                                                                                    Conditions_Groups_Tariff_Plans,
                                                                                    string.Empty))
                                    //rf payment_codex,
                                    //rf selection_name,
                                    //rf payment_plans,
                                    //rf drop_down_text,
                                    //rf conditions_plans_row))
                                    {
                                        MainProcess.Output_Message(textBoxConsole,
                                                                resource + " Tariff: " + tariff_name + " " + dashboardmodel.errorMessage,
                                                                Mally.examine.scrape, Mally.console);
                                        dashboardmodel.login_finished = false;
                                        break;
                                    }
                                    else
                                    {
                                        DashboardUtilityV2018.Build_Supplier_Limits(textBoxConsole,
                                                                    SmartUtilityConnection,
                                                                    supplier_code,
                                                                    resource_code,
                                                                    resource_type,
                                                                    Conditions_Plans_Limits);
                                        string conditions_limits_name = string.Empty;
                                        if (tier_count == 1)
                                        {
                                            conditions_limits_name = "Standing Charge";
                                        }
                                        else
                                        {
                                            conditions_limits_name = "Will never be found";
                                        }
                                        int limits_index = Conditions_Plans_Limits.Items.Count;
                                        if (limits_index == 0)
                                        {
                                            dashboardmodel.errorMessage = string.Empty;
                                            dashboardmodel.conditions_limit_row = new SmartUtility.ConditionsLimits();
                                            if (!DashboardUtilityV2018.Add_Limit(dashboardmodel,
                                                                                textBoxConsole,
                                                                                SmartUtilityConnection,
                                                                                resource_code,
                                                                                resource_type,
                                                                                supplier_code,
                                                                                defaultDates,
                                                                                conditions_limits_name,
                                                                                "SC",
                                                                                "0",
                                                                                "<=",
                                                                                "DAYS",
                                                                                "",
                                                                                "",
                                                                                "Days"))
                                            {
                                                string text = dashboardmodel.conditions_limit_row.LIMIT_CODE.ToString("000") +
                                                                " - " +
                                                                dashboardmodel.conditions_limit_row.LIMIT_NAME;
                                                MainProcess.Output_Message(textBoxConsole,
                                                                        resource + " Limit exists: " + text,
                                                                        Mally.examine.scrape, Mally.console);
                                                dashboardmodel.login_finished = false;
                                                break;
                                            }
                                            else
                                            {
                                                string text = dashboardmodel.conditions_limit_row.LIMIT_CODE.ToString("000") +
                                                                " - " +
                                                                dashboardmodel.conditions_limit_row.LIMIT_NAME;

                                                // Add it to the drop-down
                                                Conditions_Plans_Limits.Items.Add(text);
                                                Conditions_Plans_Limits.Items[limits_index].Checked = true;
                                                MainProcess.Output_Message(textBoxConsole,
                                                                    resource + " Limit added: " + text,
                                                                    Mally.examine.scrape, Mally.console);
                                            }
                                        }
                                        else
                                        {
                                            for (limits_index = 0; limits_index < Conditions_Plans_Limits.Items.Count; limits_index++)
                                            {
                                                if (Conditions_Plans_Limits.Items[limits_index].Text.IndexOf(conditions_limits_name) >= 0)
                                                {
                                                    Conditions_Plans_Limits.Items[limits_index].Checked = true;
                                                    break;
                                                }
                                            }
                                        }
                                        dashboardmodel.errorMessage = string.Empty;
                                        dashboardmodel.drop_down_text = string.Empty;
                                        // FAWNING OLEAGENOUS SUCKING-UP BITCH TO HER
                                        // FAUX reLATIONS.  SHE IS BRIGHT!  HAPPY!  SPARKLING!
                                        // BUT IN REALITY SHE IS FOUL-MOUTHED AND FOUL-
                                        // TEMPERED BITCH.  sHE IS SUCH A fraud.  sHE IS AN ARCHETYPAL sham
                                        // tHE FALSE laugh the staccato high-pitched put-on joy
                                        // The gushing obsequious fawning false personality.
                                        // From a bitch who never had fucking sex in her life.
                                        // From a barren, vituperative absolute bitch
                                        dashboardmodel.conditions_dates_row = new SmartUtility.ConditionsDates();
                                        if (!DashboardUtilityV2018.Add_Conditions_Dates(dashboardmodel,
                                                                                        textBoxConsole,
                                                                                        SmartUtilityConnection,
                                                                                        resource_code,
                                                                                        supplier_code,
                                                                                        tariff_code,
                                                                                        version_code,
                                                                                        dashboardmodel.payment_code,
                                                                                        tier_count,
                                                                                        tier_level,
                                                                                        dashboardmodel.selection_name,
                                                                                        resource_type,
                                                                                        payment_plan,
                                                                                        defaultDates,
                                                                                        prices_valid_from,
                                                                                        defaultDate,
                                                                                        string.Empty,
                                                                                        Conditions_Plans_Limits))
                                        {
                                            MainProcess.Output_Message(textBoxConsole,
                                                                    resource + " Tariff: " + tariff_name + " " + dashboardmodel.errorMessage,
                                                                    Mally.examine.scrape, Mally.console);
                                            dashboardmodel.login_finished = false;
                                            break;
                                        }
                                        else
                                        {
                                            string area_info = string.Empty;

                                            for (int row = 0; row < 14; row++)
                                            {
                                                short area_code = Convert.ToInt16(unit_rates[row, 0, 0]); // Tier1
                                                if (area_code > 0)  // If we are only doing Area 12, then it might be 0 for all the others
                                                {
                                                    string standing_charge1 = string.Format("{0:0.000}", unit_rates[row, 0, 1]),    // Tier1
                                                            standing_charge2 = string.Format("{0:0.000}", unit_rates[row, 1, 1]),   // Tier2
                                                            standing_charge3 = 0.0M.ToString(),
                                                            day_rate1 = string.Format("{0:0.000}", unit_rates[row, 0, 2]),          // Tier1
                                                            day_rate2 = string.Format("{0:0.000}", unit_rates[row, 1, 2]),          // Tier2
                                                            day_rate3 = 0.0M.ToString(),
                                                            night_rate1 = string.Format("{0:0.000}", unit_rates[row, 0, 3]),        // Tier1
                                                            night_rate2 = string.Format("{0:0.000}", unit_rates[row, 1, 3]),        // Tier2
                                                            night_rate3 = 0.0M.ToString(),
                                                            tariff_comparison_rate = string.Format("{0:0.000}", unit_rates[row, 0, 4]); // Tier1
                                                    standing_charge1 = DashboardUtilityV2018.Cleanup(standing_charge1);
                                                    day_rate1 = DashboardUtilityV2018.Cleanup(day_rate1);
                                                    night_rate1 = DashboardUtilityV2018.Cleanup(night_rate1);
                                                    standing_charge2 = DashboardUtilityV2018.Cleanup(standing_charge2);
                                                    day_rate2 = DashboardUtilityV2018.Cleanup(day_rate2);
                                                    night_rate2 = DashboardUtilityV2018.Cleanup(night_rate2);
                                                    tariff_comparison_rate = DashboardUtilityV2018.Cleanup(tariff_comparison_rate);
                                                    bool dual_fuel = false;
                                                    dashboardmodel.errorMessage = string.Empty;
                                                    bool lock_records = true;               // Lets try it with this 'on'
                                                    if (DashboardUtilityV2018.Update_Values(dashboardmodel,
                                                                        textBoxConsole,
                                                                        SmartUtilityConnection,
                                                                        area_code,
                                                                        supplier_code,
                                                                        resource_code,
                                                                        tariff_code,
                                                                        version_code,
                                                                        dashboardmodel.payment_plans,
                                                                        false,
                                                                        resource_type,
                                                                        dual_fuel,
                                                                        lock_records,
                                                                        standing_charge1,
                                                                        standing_charge2,
                                                                        standing_charge3,
                                                                        day_rate1,
                                                                        day_rate2,
                                                                        day_rate3,
                                                                        night_rate1,
                                                                        night_rate2,
                                                                        night_rate3,
                                                                        tariff_comparison_rate,
                                                                        prices_valid_from,
                                                                        defaultDate,
                                                                        uplift))
                                                    {
                                                        area_info = area_info + area_code.ToString("00") + " ";
                                                        MainProcess.Output_Message(textBoxConsole,
                                                                                resource + " " + dashboardmodel.errorMessage,
                                                                                Mally.examine.scrape, Mally.console);
                                                    }
                                                    else
                                                    {
                                                        // Here? Is where we see if its locked
                                                        MainProcess.Output_Message(textBoxConsole,
                                                                            resource + " Data: " + dashboardmodel.errorMessage,
                                                                            Mally.examine.scrape, Mally.console);
                                                    }
                                                }
                                            }
                                        }

                                    }
                                }
                                else
                                {
                                    MainProcess.Output_Message(textBoxConsole,
                                                        resource + " Payment Plan: " + derived_payment_plan + " " + "not found",
                                                        Mally.examine.scrape, Mally.console);
                                }
                            }
                        }
                        next_routine = "LOGOUT";
                        break;
                    case "LOGOUT":
                        keep_looping = false;
                        break;
                    default:
                        break;
                }
            }
            return dashboardmodel.login_finished;
        }

        internal static bool USW_Decipher_Supplier_Key(string json,
                                            DashBored Mally,
                                            RichTextBox textBoxConsole,
                                            string brand_name,
                                            ref string key_name)
        {

            string value = string.Empty;

            key_name = string.Empty;

            // Look at the JSON

            dynamic jsonArray = JArray.Parse(json);
            foreach (dynamic array_entry in jsonArray)
            {
                bool found_it = false;

                dynamic jsonItem = JObject.Parse(Convert.ToString(array_entry));
                foreach (dynamic entry in jsonItem)
                {
                    string entry_item = entry.Name;
                    switch (entry_item)
                    {
                        case "name":
                            //MainProcess.Output_Message(textBoxConsole,
                            //                            entry.Name + " " + entry.Value,
                            //                            Mally.examine.scrape, Mally.console);
                            if (entry.Value == brand_name)
                            {
                                found_it = true;
                            }
                            break;
                        case "key":
                            //MainProcess.Output_Message(textBoxConsole,
                            //                            entry.Name + " " + entry.Value,
                            //                            Mally.examine.scrape, Mally.console);
                            if (found_it)
                            {
                                key_name = entry.Value;

                            }
                            break;

                        default:
                            break;
                    }
                    if (!string.IsNullOrEmpty(key_name))
                    {
                        return true;
                    }
                }

            }
            return false;
        }

        internal static bool USW_Decipher(string json,
                    DashBored Mally,
                    RichTextBox textBoxConsole,
                    string tariff_name,
                    ref bool payment_type_found,
                    ref string elec_standing_charge,
                    ref string elec_day_rate,
                    ref string elec_night_rate,
                    ref string gas_standing_charge,
                    ref string gas_day_rate,
                    ref string gas_night_rate)
        {

            string value = string.Empty,
                    addressasline = string.Empty;

            // Look at the JSON

            dynamic jsonArray = JArray.Parse(json);
            foreach (dynamic array_entry in jsonArray)
            {
                bool found_it = false;

                dynamic jsonItem = JObject.Parse(Convert.ToString(array_entry));
                foreach (dynamic entry in jsonItem)
                {
                    string entry_item = entry.Name;
                    switch (entry_item)
                    {
                        case "cancellationFeeNoLongerApplies":
                            //MainProcess.Output_Message(textBoxConsole,
                            //                            entry.Name + " " + entry.Value,
                            //                            Mally.examine.scrape, Mally.console);
                            break;
                        case "cancellationFeeTotal":
                            //MainProcess.Output_Message(textBoxConsole,
                            //                            entry.Name + " " + entry.Value,
                            //                            Mally.examine.scrape, Mally.console);
                            break;
                        case "endDate":
                            //MainProcess.Output_Message(textBoxConsole,
                            //                            entry.Name + " " + entry.Value,
                            //                            Mally.examine.scrape, Mally.console);
                            break;
                        case "hasCancellationFee":
                            //MainProcess.Output_Message(textBoxConsole,
                            //                            entry.Name + " " + entry.Value,
                            //                            Mally.examine.scrape, Mally.console);
                            break;
                        case "name":
                            //MainProcess.Output_Message(textBoxConsole,
                            //                            entry.Name + " " + entry.Value,
                            //                            Mally.examine.scrape, Mally.console);
                            if (entry.Value == tariff_name)
                            {
                                found_it = true;
                                payment_type_found = true;
                            }
                            else
                            {
                                if (entry.Value == tariff_name.Replace("-", "–")) // Hyphens (sometimes) must be replaced with minus signs!!!!
                                {
                                    found_it = true;
                                    payment_type_found = true;
                                }
                            }
                            break;
                        case "nameKey":
                            //MainProcess.Output_Message(textBoxConsole,
                            //                            entry.Name + " " + entry.Value,
                            //                            Mally.examine.scrape, Mally.console);
                            break;
                        case "needsEndDate":
                            //MainProcess.Output_Message(textBoxConsole,
                            //                            entry.Name + " " + entry.Value,
                            //                            Mally.examine.scrape, Mally.console);
                            break;
                        case "rates":
                            //MainProcess.Output_Message(textBoxConsole,
                            //                                                        entry.Name + " " + entry.Value,
                            //                                                        Mally.examine.scrape, Mally.console);
                            if (found_it)
                            {
                                // Now this self-promoting moron shows me photos from her self-promoting family
                                // When I (rightly) pointed out that Sophie's belt is the incorrect length .. all I got was a grunt
                                // She never wants to understand the correct way of doing things, she's not interested in being exact;
                                // all she ever wants to do is guess or estimate (wrongly) or accept an incorrect or invalid argument.
                                // She has no concept of doing ANYTHING properly ...
                                dynamic resource_jsonItem = JObject.Parse(Convert.ToString(entry.Value));
                                foreach (dynamic resource_entry in resource_jsonItem)
                                {
                                    string resource_entry_item = resource_entry.Name;
                                    switch (resource_entry_item)
                                    {
                                        case "electricity":
                                            string e_rate_item = Convert.ToString(resource_entry.Value);
                                            e_rate_item = e_rate_item.Replace(Environment.NewLine, string.Empty);
                                            dynamic e_rate_jsonItem = JArray.Parse(e_rate_item);
                                            foreach (dynamic e_rate_entry in e_rate_jsonItem)
                                            {
                                                string price = string.Empty;
                                                foreach (dynamic e_actual_entry in e_rate_entry)
                                                {
                                                    switch (e_actual_entry.Name)
                                                    {
                                                        case "price":
                                                            //MainProcess.Output_Message(textBoxConsole,
                                                            //                     "Day rate: " + e_actual_entry.Value,
                                                            //                     Mally.examine.scrape, Mally.console);
                                                            price = e_actual_entry.Value;
                                                            break;
                                                        case "threshold":
                                                            if (e_actual_entry.Value != null)
                                                            {
                                                                //MainProcess.Output_Message(textBoxConsole,
                                                                //                 "Threshold: " + e_actual_entry.Value,
                                                                //                 Mally.examine.scrape, Mally.console);
                                                            }
                                                            break;
                                                        case "nightRate":
                                                            if (e_actual_entry.Value != null)
                                                            {
                                                                //MainProcess.Output_Message(textBoxConsole,
                                                                //                 "Night rate: " + e_actual_entry.Value,
                                                                //                 Mally.examine.scrape, Mally.console);
                                                                try
                                                                {
                                                                    decimal price_dec = Convert.ToDecimal(price.Trim());
                                                                    price = price_dec.ToString("#.###", CultureInfo.CurrentCulture);
                                                                    // For some reason 0 converts  to 'empty'
                                                                    if (string.IsNullOrEmpty(price))
                                                                    {
                                                                        price = "0.000";
                                                                    }
                                                                    string e_night = e_actual_entry.Value.ToString();
                                                                    if (e_night == "True")
                                                                    {
                                                                        elec_night_rate = price;
                                                                    }
                                                                    else
                                                                    {
                                                                        elec_day_rate = price;
                                                                    }
                                                                }
                                                                catch (Exception)
                                                                {
                                                                    MainProcess.Output_Message(textBoxConsole,
                                                                                     "Cannot covnert: " + price + " to decimal",
                                                                                     Mally.examine.scrape, Mally.console);

                                                                    return false;
                                                                }
                                                            }
                                                            break;
                                                        default:
                                                            break;
                                                    }
                                                }
                                            }

                                            break;
                                        case "gas":
                                            string g_rate_item = Convert.ToString(resource_entry.Value);
                                            g_rate_item = g_rate_item.Replace(Environment.NewLine, string.Empty);
                                            dynamic g_rate_jsonItem = JArray.Parse(g_rate_item);
                                            foreach (dynamic g_rate_entry in g_rate_jsonItem)
                                            {
                                                string price = string.Empty;
                                                foreach (dynamic g_actual_entry in g_rate_entry)
                                                {
                                                    switch (g_actual_entry.Name)
                                                    {
                                                        case "price":
                                                            //MainProcess.Output_Message(textBoxConsole,
                                                            //                     "Day rate: " + g_actual_entry.Value,
                                                            //                     Mally.examine.scrape, Mally.console);
                                                            gas_day_rate = g_actual_entry.Value;
                                                            break;
                                                        case "threshold":
                                                            if (g_actual_entry.Value != null)
                                                            {
                                                                //MainProcess.Output_Message(textBoxConsole,
                                                                //                 "Threshold: " + g_actual_entry.Value,
                                                                //                 Mally.examine.scrape, Mally.console);
                                                            }
                                                            break;
                                                        case "nightRate":
                                                            if (g_actual_entry.Value != null)
                                                            {
                                                                //MainProcess.Output_Message(textBoxConsole,
                                                                //                 "Night rate: " + g_actual_entry.Value,
                                                                //                 Mally.examine.scrape, Mally.console);
                                                                string g_night = g_actual_entry.Value.ToString();
                                                                if (g_night == "True")
                                                                {
                                                                    gas_night_rate = price;
                                                                }

                                                            }
                                                            break;
                                                        default:
                                                            break;
                                                    }
                                                }
                                            }
                                            break;
                                        default:
                                            break;
                                    }
                                }
                            }
                            break;
                        case "shouldAskCustomerOnlineQuestion":
                            //MainProcess.Output_Message(textBoxConsole,
                            //                            entry.Name + " " + entry.Value,
                            //                            Mally.examine.scrape, Mally.console);
                            break;
                        case "standard":
                            //MainProcess.Output_Message(textBoxConsole,
                            //                            entry.Name + " " + entry.Value,
                            //                            Mally.examine.scrape, Mally.console);
                            break;
                        case "standingCharge":
                            //MainProcess.Output_Message(textBoxConsole,
                            //                            entry.Name + " " + entry.Value,
                            //                            Mally.examine.scrape, Mally.console);
                            if (found_it)
                            {
                                // Now this self-promoting moron shows me photos from her self-promoting family
                                // When I (rightly) pointed out that Sophie's belt is the incorrect length .. all I got was a grunt
                                // She never wants to understand the correct way of doing things, she's not interested in being exact;
                                // all she ever wants to do is guess or estimate (wrongly) or accept an incorrect or invalid argument.
                                // She has no concept of doing ANYTHING properly ...
                                dynamic resource_jsonItem = JObject.Parse(Convert.ToString(entry.Value));
                                foreach (dynamic resource_entry in resource_jsonItem)
                                {
                                    string resource_entry_item = resource_entry.Name;
                                    switch (resource_entry_item)
                                    {
                                        case "electricity":
                                            string e_charge_item = resource_entry.Value;
                                            // Work out daily rate
                                            decimal e_yearly_charge = Convert.ToDecimal(e_charge_item);
                                            decimal e_daily_charge = (e_yearly_charge * 100.0M) / 365;
                                            //MainProcess.Output_Message(textBoxConsole,
                                            //                            "Standing charge: " + e_daily_charge,
                                            //                            Mally.examine.scrape, Mally.console);
                                            elec_standing_charge = e_daily_charge.ToString("#.###", CultureInfo.CurrentCulture);
                                            // For some reason 0 converts  to 'empty'
                                            if (string.IsNullOrEmpty(elec_standing_charge))
                                            {
                                                elec_standing_charge = "0.000";
                                            }
                                            break;
                                        case "gas":
                                            string g_charge_item = resource_entry.Value;

                                            // Work out daily rate
                                            decimal g_yearly_charge = Convert.ToDecimal(g_charge_item);
                                            decimal g_daily_charge = (g_yearly_charge * 100.0M) / 365;
                                            //MainProcess.Output_Message(textBoxConsole,
                                            //                            "Standing charge: " + g_daily_charge,
                                            //                            Mally.examine.scrape, Mally.console);
                                            gas_standing_charge = g_daily_charge.ToString("#.###", CultureInfo.CurrentCulture);
                                            // For some reason 0 converts  to 'empty'
                                            if (string.IsNullOrEmpty(gas_standing_charge))
                                            {
                                                gas_standing_charge = "0.000";
                                            }
                                            break;
                                        default:
                                            break;

                                    }
                                }
                            }
                            break;
                        case "status":
                            //MainProcess.Output_Message(textBoxConsole,
                            //                            entry.Name + " " + entry.Value,
                            //                            Mally.examine.scrape, Mally.console);
                            break;
                        default:
                            break;
                    }
                }
            }
            return true;
        }

        internal static bool USW_Decipher_Supplier_Name(string json,
                                            DashBored Mally,
                                            RichTextBox textBoxConsole,
                                            string brand_name,
                                            ref string keyName)
        {

            string value = string.Empty,
                    addressasline = string.Empty;

            // Look at the JSON

            dynamic jsonArray = JArray.Parse(json);
            foreach (dynamic array_entry in jsonArray)
            {
                bool found_it = false;

                dynamic jsonItem = JObject.Parse(Convert.ToString(array_entry));
                foreach (dynamic entry in jsonItem)
                {
                    string entry_item = entry.Name;
                    switch (entry_item)
                    {
                        case "cancellationFeeNoLongerApplies":
                            //MainProcess.Output_Message(textBoxConsole,
                            //                            entry.Name + " " + entry.Value,
                            //                            Mally.examine.scrape, Mally.console);
                            break;

                        case "rates":
                            //MainProcess.Output_Message(textBoxConsole,
                            //                                                        entry.Name + " " + entry.Value,
                            //                                                        Mally.examine.scrape, Mally.console);
                            if (found_it)
                            {
                                // Now this self-promoting moron shows me photos from her self-promoting family
                                // When I (rightly) pointed out that Sophie's belt is the incorrect length .. all I got was a grunt
                                // She never wants to understand the correct way of doing things, she's not interested in being exact;
                                // all she ever wants to do is guess or estimate (wrongly) or accept an incorrect or invalid argument.
                                // She has no concept of doing ANYTHING properly ...
                                dynamic resource_jsonItem = JObject.Parse(Convert.ToString(entry.Value));
                                foreach (dynamic resource_entry in resource_jsonItem)
                                {
                                    string resource_entry_item = resource_entry.Name;
                                    switch (resource_entry_item)
                                    {
                                        case "electricity":
                                            string e_rate_item = Convert.ToString(resource_entry.Value);
                                            e_rate_item = e_rate_item.Replace(Environment.NewLine, string.Empty);
                                            dynamic e_rate_jsonItem = JArray.Parse(e_rate_item);
                                            foreach (dynamic e_rate_entry in e_rate_jsonItem)
                                            {
                                                string price = string.Empty;
                                                foreach (dynamic e_actual_entry in e_rate_entry)
                                                {
                                                    switch (e_actual_entry.Name)
                                                    {
                                                        case "price":
                                                            //MainProcess.Output_Message(textBoxConsole,
                                                            //                     "Day rate: " + e_actual_entry.Value,
                                                            //                     Mally.examine.scrape, Mally.console);
                                                            price = e_actual_entry.Value;
                                                            break;
                                                        case "threshold":
                                                            if (e_actual_entry.Value != null)
                                                            {
                                                                //MainProcess.Output_Message(textBoxConsole,
                                                                //                 "Threshold: " + e_actual_entry.Value,
                                                                //                 Mally.examine.scrape, Mally.console);
                                                            }
                                                            break;
                                                        case "nightRate":
                                                            if (e_actual_entry.Value != null)
                                                            {
                                                                //MainProcess.Output_Message(textBoxConsole,
                                                                //                 "Night rate: " + e_actual_entry.Value,
                                                                //                 Mally.examine.scrape, Mally.console);
                                                                try
                                                                {
                                                                    decimal price_dec = Convert.ToDecimal(price.Trim());


                                                                    price = price_dec.ToString("#.###", CultureInfo.CurrentCulture);



                                                                    string e_night = e_actual_entry.Value.ToString();
                                                                    if (e_night == "True")
                                                                    {
                                                                        // elec_night_rate = price;
                                                                    }
                                                                    else
                                                                    {
                                                                        // elec_day_rate = price;
                                                                    }
                                                                }
                                                                catch (Exception)
                                                                {
                                                                    MainProcess.Output_Message(textBoxConsole,
                                                                                     "Cannot covnert: " + price + " to decimal",
                                                                                     Mally.examine.scrape, Mally.console);

                                                                    return false;
                                                                }
                                                            }
                                                            break;
                                                        default:
                                                            break;
                                                    }
                                                }
                                            }

                                            break;
                                        case "gas":
                                            string g_rate_item = Convert.ToString(resource_entry.Value);
                                            g_rate_item = g_rate_item.Replace(Environment.NewLine, string.Empty);
                                            dynamic g_rate_jsonItem = JArray.Parse(g_rate_item);
                                            foreach (dynamic g_rate_entry in g_rate_jsonItem)
                                            {
                                                string price = string.Empty;
                                                foreach (dynamic g_actual_entry in g_rate_entry)
                                                {
                                                    switch (g_actual_entry.Name)
                                                    {
                                                        case "price":
                                                            //MainProcess.Output_Message(textBoxConsole,
                                                            //                     "Day rate: " + g_actual_entry.Value,
                                                            //                     Mally.examine.scrape, Mally.console);
                                                            //gas_day_rate = g_actual_entry.Value;
                                                            break;
                                                        case "threshold":
                                                            if (g_actual_entry.Value != null)
                                                            {
                                                                //MainProcess.Output_Message(textBoxConsole,
                                                                //                 "Threshold: " + g_actual_entry.Value,
                                                                //                 Mally.examine.scrape, Mally.console);
                                                            }
                                                            break;
                                                        case "nightRate":
                                                            if (g_actual_entry.Value != null)
                                                            {
                                                                //MainProcess.Output_Message(textBoxConsole,
                                                                //                 "Night rate: " + g_actual_entry.Value,
                                                                //                 Mally.examine.scrape, Mally.console);
                                                                string g_night = g_actual_entry.Value.ToString();
                                                                if (g_night == "True")
                                                                {
                                                                    //gas_night_rate = price;
                                                                }

                                                            }
                                                            break;
                                                        default:
                                                            break;
                                                    }
                                                }
                                            }
                                            break;
                                        default:
                                            break;
                                    }
                                }
                            }
                            break;
                        case "shouldAskCustomerOnlineQuestion":
                            //MainProcess.Output_Message(textBoxConsole,
                            //                            entry.Name + " " + entry.Value,
                            //                            Mally.examine.scrape, Mally.console);
                            break;

                        default:
                            break;
                    }
                }
            }
            return true;
        }


        private static bool Authenticity_Token(DashBored Mally,
                                            RichTextBox textBoxConsole,
                                            HtmlAgilityPack.HtmlDocument html_document,
                                            ref string authenticity_token)
        {
            IList<HtmlAgilityPack.HtmlNode> HtmlCol1;

            // go and get the authenticity_token
            // < input name = "authenticity_token" type = "hidden" value = "mPm5Pj8rmTSgrRBGbG2MB3eCXD31CQwm2uQGGMyrCxs=" /></ div >

            HtmlCol1 = YetAnotherFuckingHoop.SelectNodesAsList(html_document.DocumentNode, ".//input");
            foreach (HtmlAgilityPack.HtmlNode element1 in HtmlCol1)
            {
                string name = element1.GetAttributeValue("name", string.Empty);

                if (name == "authenticity_token")
                {
                    string value = element1.GetAttributeValue("value", string.Empty);
                    authenticity_token = value;
                    return true;
                }
            }
            return false;
        }

        private static bool Find_USCC(DashBored Mally,
                                            RichTextBox textBoxConsole,
                                            HtmlAgilityPack.HtmlDocument html_document,
                                            ref string uscc)
        {
            IList<HtmlAgilityPack.HtmlNode> HtmlCol1;

            HtmlCol1 = YetAnotherFuckingHoop.SelectNodesAsList(html_document.DocumentNode, "//script");
            foreach (HtmlAgilityPack.HtmlNode element1 in HtmlCol1)
            {
                //string name = element1.GetAttributeValue("name", string.Empty);

                string name = element1.InnerText;
                string[] comma_separated = name.Split(',');
                foreach (string comma_pair in comma_separated)
                {
                    if (comma_pair.IndexOf("uscc") >= 0)
                    {
                        string value = comma_pair.Replace("\"", "");
                        int colon = value.IndexOf(":");
                        if (colon >= 0)
                        {
                            uscc = value.Substring(colon + 1);
                            return true;
                        }
                    }

                }
            }
            return false;
        }

        private static bool Find_Token(DashBored Mally,
                                            RichTextBox textBoxConsole,
                                            HtmlAgilityPack.HtmlDocument html_document,
                                            ref string token)
        {
            IList<HtmlAgilityPack.HtmlNode> HtmlCol1;

            HtmlCol1 = YetAnotherFuckingHoop.SelectNodesAsList(html_document.DocumentNode, "//script");
            foreach (HtmlAgilityPack.HtmlNode element1 in HtmlCol1)
            {
                //string name = element1.GetAttributeValue("name", string.Empty);

                string name = element1.InnerText;
                string[] comma_separated = name.Split(',');
                foreach (string comma_pair in comma_separated)
                {
                    string[] lines = comma_pair.Split(SmartParametersV2016.newline);
                    foreach (string line in lines)
                    {
                        if (line.IndexOf("token") >= 0)
                        {
                            string value = line.Replace("\"", "");
                            int colon = value.IndexOf(":");
                            if (colon >= 0)
                            {
                                token = value.Substring(colon + 1);
                                token = token.Replace("token:", string.Empty);
                                token = token.TrimStart('{');
                                token = token.TrimEnd('}');
                                return true;
                            }
                        }
                    }
                }
            }
            return false;
        }

        private static bool Find_Authorization_Token(DashBored Mally,
                                            RichTextBox textBoxConsole,
                                            HtmlAgilityPack.HtmlDocument html_document,
                                            ref string authorization_token)
        {
            IList<HtmlAgilityPack.HtmlNode> HtmlCol1;

            HtmlCol1 = YetAnotherFuckingHoop.SelectNodesAsList(html_document.DocumentNode, "//script");
            foreach (HtmlAgilityPack.HtmlNode element1 in HtmlCol1)
            {
                //string name = element1.GetAttributeValue("name", string.Empty);

                string name = element1.InnerText;
                string[] comma_separated = name.Split(',');
                foreach (string comma_pair in comma_separated)
                {
                    if (comma_pair.IndexOf("token") >= 0)
                    {
                        string value = comma_pair.Replace("\"", "");
                        int colon = value.IndexOf(":");
                        if (colon >= 0)
                        {
                            authorization_token = value.Substring(colon + 1);
                            return true;
                        }
                    }

                }
            }
            return false;
        }

        private static bool Find_Supplier_Id(char resource_code,
                                            string brand_name,
                                            DashBored Mally,
                                            RichTextBox textBoxConsole,
                                            HtmlAgilityPack.HtmlDocument html_document,
                                            ref string supplier_id)
        {
            IList<HtmlAgilityPack.HtmlNode> HtmlCol1,
                                            HtmlCol3,
                                            HtmlCol4,
                                            HtmlCol5,
                                            HtmlCol6;

            supplier_id = string.Empty;
            HtmlCol1 = YetAnotherFuckingHoop.SelectNodesAsList(html_document.DocumentNode, "//div");
            foreach (HtmlAgilityPack.HtmlNode element1 in HtmlCol1)
            {
                string classname = element1.GetAttributeValue("class", string.Empty);

                bool found = false;
                switch (resource_code)
                {
                    case 'E':
                        if (SmartNibbyV2016.Check_Classname(classname, "radio-buttons-suppliers electricity", true))
                        {
                            found = true;
                        }
                        break;
                    case 'G':
                        if (SmartNibbyV2016.Check_Classname(classname, "radio-buttons-suppliers gas", true))
                        {
                            found = true;
                        }
                        break;
                    default:
                        break;
                }
                if (found)
                {
                    string value = string.Empty;
                    HtmlCol3 = YetAnotherFuckingHoop.SelectNodesAsList(element1, ".//div");
                    foreach (HtmlAgilityPack.HtmlNode element3 in HtmlCol3)
                    {
                        classname = element3.GetAttributeValue("class", string.Empty);
                        if (SmartNibbyV2016.Check_Classname(classname, "group", true) ||
                            SmartNibbyV2016.Check_Classname(classname, "group show-later", false))
                        {
                            HtmlCol4 = YetAnotherFuckingHoop.SelectNodesAsList(element3, ".//input");
                            foreach (HtmlAgilityPack.HtmlNode element4 in HtmlCol4)
                            {
                                supplier_id = element4.GetAttributeValue("value", string.Empty);
                                break;
                            }
                            HtmlCol5 = YetAnotherFuckingHoop.SelectNodesAsList(element3, ".//label");
                            foreach (HtmlAgilityPack.HtmlNode element5 in HtmlCol5)
                            {
                                HtmlCol6 = YetAnotherFuckingHoop.SelectNodesAsList(element5, ".//span");
                                foreach (HtmlAgilityPack.HtmlNode element6 in HtmlCol6)
                                {
                                    string supplier_name = element6.InnerText.Trim();
                                    if (supplier_name == brand_name)
                                    {
                                        //textBoxConsole.AppendText(" Supplier: " + supplier_name + "Id: " + supplier_id + Environment.NewLine);
                                        return true;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return false;
        }

        private static bool Find_Tariff_Id(string brand_name,
                                            string tariff_name,
                                            DashBored Mally,
                                            RichTextBox textBoxConsole,
                                            HtmlAgilityPack.HtmlDocument html_document,
                                            ref string tariff_id)
        {
            IList<HtmlAgilityPack.HtmlNode> HtmlCol1,
                                            HtmlCol2;

            tariff_id = string.Empty;
            HtmlCol1 = YetAnotherFuckingHoop.SelectNodesAsList(html_document.DocumentNode, "//select");
            foreach (HtmlAgilityPack.HtmlNode element1 in HtmlCol1)
            {
                string classname = element1.GetAttributeValue("class", string.Empty);
                if (SmartNibbyV2016.Check_Classname(classname, "form-control", true))
                {
                    string[] found_names = element1.InnerText.Split(SmartParametersV2016.newline);
                    int count = 0;
                    while (count < found_names.Count())
                    {
                        found_names[count] = found_names[count].Replace(brand_name, string.Empty).Trim();
                        count = count + 1;
                    }

                    string value = string.Empty;
                    HtmlCol2 = YetAnotherFuckingHoop.SelectNodesAsList(element1, ".//option");
                    int name_index = 0;
                    foreach (HtmlAgilityPack.HtmlNode element2 in HtmlCol2)
                    {
                        tariff_id = element2.GetAttributeValue("value", string.Empty);
                        if ((found_names[name_index] == tariff_name) ||
                            (found_names[name_index] == tariff_name.Replace(" - ", " ")) ||
                            (found_names[name_index].IndexOf(tariff_name) >= 0))
                        {
                            //textBoxConsole.AppendText(" Tariff: " + tariff_name + "Id: " + tariff_id + Environment.NewLine);
                            return true;
                        }
                        name_index = name_index + 1;
                    }
                }
            }
            return false;
        }



        static private bool Set_Tariff_Plans(string payment_plan,
                                             string payment_name,
                                             ListView Payments_Tariff_Plans,
                                             ListView Conditions_Groups_Tariff_Plans)
        {
            if (Conditions_Groups_Tariff_Plans.Items.Count == 0)
            {
                for (int plan_code_index = 0; plan_code_index < Payments_Tariff_Plans.Items.Count; plan_code_index++)
                {
                    if (Payments_Tariff_Plans.Items[plan_code_index].Text.Substring(0, 1) == payment_plan)
                    {
                        Conditions_Groups_Tariff_Plans.Items.Add(payment_name);
                        Conditions_Groups_Tariff_Plans.Items[plan_code_index].Text = payment_plan.Substring(0, 1);
                        Conditions_Groups_Tariff_Plans.Items[plan_code_index].Checked = true;
                        return true;
                    }
                }
            }
            else
            {
                for (int plan_code_index = 0; plan_code_index < Conditions_Groups_Tariff_Plans.Items.Count; plan_code_index++)
                {
                    if (Conditions_Groups_Tariff_Plans.Items[plan_code_index].Text.Substring(0, 1) == payment_plan)
                    {
                        Conditions_Groups_Tariff_Plans.Items[plan_code_index].Checked = true;
                        return true;
                    }
                }
            }
            return false;
        }

        private static bool USwitch_Decode(StreamWriter sw,
                                            HtmlAgilityPack.HtmlDocument document,
                                            RichTextBox textBoxConsole)
        {

            IList<HtmlAgilityPack.HtmlNode> HtmlCol1,   // table
                                            HtmlCol2;   // tr

            char delimiter = '\t';

            string supplier = string.Empty;

            HtmlCol1 = YetAnotherFuckingHoop.SelectNodesAsList(document.DocumentNode, "//table");
            foreach (HtmlAgilityPack.HtmlNode element1 in HtmlCol1)
            {
                string classname = element1.GetAttributeValue("class", "");
                if (Check_Classname(classname, "data-table"))
                {
                    if (!(element1.Id == null || element1.Id == string.Empty))
                    {
                        if (element1.Id == "price_updates")
                        {
                            HtmlCol2 = YetAnotherFuckingHoop.SelectNodesAsList(element1, "tr");
                            foreach (HtmlAgilityPack.HtmlNode element2 in HtmlCol2)
                            {
                                int img_index = element2.InnerHtml.IndexOf("<img alt=");
                                int src_index = element2.InnerHtml.IndexOf("src=");
                                if (img_index >= 0 && src_index >= 0)
                                {
                                    supplier = element2.InnerHtml.Substring(img_index + 9,
                                                                            src_index - img_index - 9);
                                    supplier = supplier.Replace("\"", string.Empty).Trim();
                                    supplier = supplier.Replace("&#x27;", "'");
                                    supplier = supplier.Replace("&amp;", "&");



                                    string text = element2.InnerText.Replace("\n", string.Empty);

                                    text = text.Replace("&nbsp;", " ");
                                    text = text.Replace("&#39;", "'");
                                    text = text.Replace("&#x27;", "'");

                                    text = text.Replace("&pound;", "£");
                                    text = text.Replace("Â", string.Empty);

                                    text = text.Replace("&amp;", "&");
                                    text = text.Replace("&rsquo;", "'");
                                    text = text.Replace("&lsquo;", "'");
                                    text = text.Replace("&quot;", "'");
                                    text = SmartParseV2016.Remove_Double_Spaces_V3(text);

                                    text = text.Replace("\t", string.Empty).Trim();
                                    text = text.Replace("\r", string.Empty).Trim();

                                    string day = string.Empty,
                                            month = string.Empty,
                                            year = string.Empty,
                                            rest = string.Empty;

                                    int space1 = text.IndexOf(" ");
                                    if (space1 >= 0)
                                    {
                                        day = text.Substring(0, space1);
                                        int space2 = text.IndexOf(" ", space1 + 1);
                                        if (space2 >= 0)
                                        {
                                            month = text.Substring(space1 + 1, space2 - space1 - 1);
                                            year = text.Substring(space2 + 1, 4);
                                            rest = text.Substring(space2 + 1 + 4);
                                            text = day + " " + month + " " + year + delimiter +
                                                    day + " " + month + " " + year + delimiter +
                                                    "USwitch" + delimiter +
                                                    rest;
                                        }
                                        else
                                        {
                                            textBoxConsole.AppendText(">>CANNOT DECODE<< " + text + Environment.NewLine.ToString());
                                            textBoxConsole.ScrollToCaret();
                                        }
                                    }
                                    else
                                    {
                                        textBoxConsole.AppendText(">>CANNOT DECODE<< " + text + Environment.NewLine.ToString());
                                        textBoxConsole.ScrollToCaret();
                                    }
                                    text = supplier + delimiter + text;

                                    sw.WriteLine(text);

                                    //textBoxConsole.AppendText(text + Environment.NewLine.ToString());
                                    //textBoxConsole.ScrollToCaret();
                                }
                            }
                        }
                    }
                }
            }
            return true;
        }

        private static bool Check_Classname(string classname, string comparison)
        {
            if (classname != null)
            {
                if (classname != string.Empty)
                {
                    if (classname == comparison)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        static public bool USW_Add_Tariff(DashboardModel dashboardmodel,
                                    SqlConnection SmartUtilityConnection,
                                    char category_code,
                                    char resource_code,
                                    string resource_type,
                                    short supplier_code,
                                    short brand_code,
                                    short version_code,
                                    char propogate_tariffs,
                                    string brand_name,
                                    string tariff_name,
                                    DateTime launched,
                                    DateTime issued,
                                    DateTime prices_valid_from,
                                    DateTime valid_to,
                                    DateTime withdrawn,
                                    string dual_fuel_only,
                                    string defaultDates,
                                    string status_flag,
                                    CheckedListBox Tariffs_Areas_Matrix,
                                    DashBored Mally,
                                    RichTextBox textBoxConsole)
        {
            if (status_flag != string.Empty)
            {
                MainProcess.Output_Message(textBoxConsole,
                                    "!!!!!!!!!!****INSERT FLAG IS " + status_flag + " ******!!!!!!",

                                    Mally.examine.scrape, Mally.console);
            }
            string routine = MethodBase.GetCurrentMethod().Name.ToUpper();

            // The TARIFF_NAME is the ***SUPPLIER*** 
            string[] fields = new String[10];
            fields[0] = "BRAND_CODE";
            fields[1] = brand_code.ToString();// Tariffs in the Area Matrix are Brand specific
            fields[2] = "RESOURCE_CODE";
            fields[3] = resource_code.ToString();
            fields[4] = "RESOURCE_TYPE";
            fields[5] = resource_type;
            fields[6] = "TARIFF_NAME";
            fields[7] = tariff_name;
            fields[8] = "VERSION_CODE";
            fields[9] = version_code.ToString();
            ObservableCollection<SmartUtility.TariffMatrix> tariff_matrix_found = SmartDatabaseV2016.READ_Records<SmartUtility.TariffMatrix>(textBoxConsole,
                                                                                            SmartUtilityConnection,
                                                                                            MainProcess.global_utility_tablesList,
                                                                                            fields,
                                                                                            string.Empty,
                                                                                            new SmartUtility.TariffMatrix());
            if (tariff_matrix_found.Count() == 0)
            {
                fields = new string[8];
                fields[0] = "BRAND_CODE";
                fields[1] = brand_code.ToString();// Tariffs in the Area Matrix are Brand specific
                fields[2] = "RESOURCE_CODE";
                fields[3] = resource_code.ToString();
                fields[4] = "RESOURCE_TYPE";
                fields[5] = resource_type;
                fields[6] = "TARIFF_PREVIOUS_NAME";
                fields[7] = tariff_name;
                ObservableCollection<SmartUtility.TariffHistory> tariff_history_found = SmartDatabaseV2016.READ_Records<SmartUtility.TariffHistory>(textBoxConsole,
                                                                                            SmartUtilityConnection,
                                                                                            MainProcess.global_utility_tablesList,
                                                                                            fields,
                                                                                            string.Empty,
                                                                                            new SmartUtility.TariffHistory());
                if (tariff_history_found.Count() == 0)
                {
                    MainProcess.Output_Message(textBoxConsole,
                                    brand_name +
                                    " / " +
                                    tariff_name +
                                    " not found in Tariff Area Matrix or Tariff Name History",
                                    Mally.examine.scrape, Mally.console);

                    string online_option = "N",
                            fallback_tariff = "N",
                            availability = "B",
                            variable = string.Empty,
                            feedin = "N",
                            age = "0",
                            customer = "N";

                    if (tariff_name.IndexOf("Fix") == -1)
                    {
                        variable = "Y";
                    }
                    else
                    {
                        variable = "N";
                    }

                    if (tariff_name.IndexOf("Online") >= 0)
                    {
                        online_option = "Y";
                    }
                    byte paydays = 127;

                    // Default the Tariffs Areas to N
                    for (int i = 0; i < Tariffs_Areas_Matrix.Items.Count; i++)
                    {
                        Tariffs_Areas_Matrix.SetItemCheckState(i, CheckState.Unchecked);
                    }

                    dashboardmodel.tariffs_row = new SmartUtility.Tariffs();
                    dashboardmodel.status_message = string.Empty;
                    if (!DashboardUtilityV2018.Add_Tariff(dashboardmodel,
                                            textBoxConsole,
                                            SmartUtilityConnection,
                                            category_code,
                                            resource_code,
                                            supplier_code,
                                            brand_code,
                                            resource_type,
                                            tariff_name,
                                            defaultDates,
                                            online_option,
                                            dual_fuel_only,
                                            fallback_tariff,
                                            availability,
                                            variable,
                                            feedin,
                                            age,
                                            customer,
                                            paydays,
                                            propogate_tariffs,
                                            launched,
                                            issued,
                                            prices_valid_from,
                                            valid_to,
                                            withdrawn,
                                            Tariffs_Areas_Matrix,
                                            status_flag))
                    {
                        MainProcess.Output_Message(textBoxConsole,
                                            "==>INSERT FAILED: " + brand_name + "/" + tariff_name,
                                            Mally.examine.scrape, Mally.console);
                        return false;
                    }
                    else
                    {
                        MainProcess.Output_Message(textBoxConsole,
                                            "Inserted: " + brand_name + " / " + tariff_name,
                                            Mally.examine.scrape, Mally.console);
                        dashboardmodel.tariff_code = dashboardmodel.tariffs_row.TARIFF_CODE;
                        return true;
                    }
                }
            }
            // Fucking stupid ignorant ranting aggrssive ignorant bitch. 
            // Who ON EARTH would want to fuck you?  I CANNOT BELIEVE I EVER DID, I really can't
            // She doesn't even flush the toilet the stinking dirty bitch
            return true;
        }

        private static bool Find_Sam_Count(RichTextBox textBoxConsole,
                                                SqlConnection SmartUtilityConnection,
                                                short brand_code,
                                                SmartUtility.BrandMatrix brand_matrix_row,
                                                CheckedListBox Brand_Matrix,
                                                ref int sam_count,
                                                ref SmartUtility.BrandMatrix brand_matrix_new)
        {

            // Need to find out from the Supplier Area Matrix how many areas are ticked                
            // Lookup all the Areas for this Brand

            String[] fields = new String[0];
            ObservableCollection<SmartUtility.BrandMatrix> brand_matrix_found = SmartDatabaseV2016.READ_Records<SmartUtility.BrandMatrix>(textBoxConsole,
                                                                SmartUtilityConnection,
                                                                MainProcess.global_utility_tablesList,
                                                                fields,
                                                                string.Empty,
                                                                new SmartUtility.BrandMatrix());
            if (brand_matrix_found.Count() > 0)
            {
                string colname;
                //SHOULD .. only be one ...
                for (int i = 0; i < Brand_Matrix.Items.Count; i++)
                {
                    colname = "AREA_" + (i + 10).ToString("00");
                    switch (colname)
                    {
                        case "AREA_10":
                            if (brand_matrix_found[0].AREA_10 == SmartParametersV2016.brandMatrixValid)
                            {
                                sam_count = sam_count + 1;
                                brand_matrix_row.AREA_10 = SmartParametersV2016.brandMatrixValid;
                            }
                            else
                            {
                                brand_matrix_row.AREA_10 = SmartParametersV2016.brandMatrixInvalid;
                            }
                            break;
                        case "AREA_11":
                            if (brand_matrix_found[0].AREA_11 == SmartParametersV2016.brandMatrixValid)
                            {
                                sam_count = sam_count + 1;
                                brand_matrix_row.AREA_11 = SmartParametersV2016.brandMatrixValid;
                            }
                            else
                            {
                                brand_matrix_row.AREA_11 = SmartParametersV2016.brandMatrixInvalid;
                            }
                            break;
                        case "AREA_12":
                            if (brand_matrix_found[0].AREA_12 == SmartParametersV2016.brandMatrixValid)
                            {
                                sam_count = sam_count + 1;
                                brand_matrix_row.AREA_12 = SmartParametersV2016.brandMatrixValid;
                            }
                            else
                            {
                                brand_matrix_row.AREA_12 = SmartParametersV2016.brandMatrixInvalid;
                            }
                            break;
                        case "AREA_13":
                            if (brand_matrix_found[0].AREA_13 == SmartParametersV2016.brandMatrixValid)
                            {
                                sam_count = sam_count + 1;
                                brand_matrix_row.AREA_13 = SmartParametersV2016.brandMatrixValid;
                            }
                            else
                            {
                                brand_matrix_row.AREA_13 = SmartParametersV2016.brandMatrixInvalid;
                            }
                            break;
                        case "AREA_14":
                            if (brand_matrix_found[0].AREA_14 == SmartParametersV2016.brandMatrixValid)
                            {
                                sam_count = sam_count + 1;
                                brand_matrix_row.AREA_14 = SmartParametersV2016.brandMatrixValid;
                            }
                            else
                            {
                                brand_matrix_row.AREA_14 = SmartParametersV2016.brandMatrixInvalid;
                            }
                            break;
                        case "AREA_15":
                            if (brand_matrix_found[0].AREA_15 == SmartParametersV2016.brandMatrixValid)
                            {
                                sam_count = sam_count + 1;
                                brand_matrix_row.AREA_15 = SmartParametersV2016.brandMatrixValid;
                            }
                            else
                            {
                                brand_matrix_row.AREA_15 = SmartParametersV2016.brandMatrixInvalid;
                            }
                            break;
                        case "AREA_16":
                            if (brand_matrix_found[0].AREA_16 == SmartParametersV2016.brandMatrixValid)
                            {
                                sam_count = sam_count + 1;
                                brand_matrix_row.AREA_16 = SmartParametersV2016.brandMatrixValid;
                            }
                            else
                            {
                                brand_matrix_row.AREA_16 = SmartParametersV2016.brandMatrixInvalid;
                            }
                            break;
                        case "AREA_17":
                            if (brand_matrix_found[0].AREA_17 == SmartParametersV2016.brandMatrixValid)
                            {
                                sam_count = sam_count + 1;
                                brand_matrix_row.AREA_17 = SmartParametersV2016.brandMatrixValid;
                            }
                            else
                            {
                                brand_matrix_row.AREA_17 = SmartParametersV2016.brandMatrixInvalid;
                            }
                            break;
                        case "AREA_18":
                            if (brand_matrix_found[0].AREA_18 == SmartParametersV2016.brandMatrixValid)
                            {
                                sam_count = sam_count + 1;
                                brand_matrix_row.AREA_18 = SmartParametersV2016.brandMatrixValid;
                            }
                            else
                            {
                                brand_matrix_row.AREA_18 = SmartParametersV2016.brandMatrixInvalid;
                            }
                            break;
                        case "AREA_19":
                            if (brand_matrix_found[0].AREA_19 == SmartParametersV2016.brandMatrixValid)
                            {
                                sam_count = sam_count + 1;
                                brand_matrix_row.AREA_19 = SmartParametersV2016.brandMatrixValid;
                            }
                            else
                            {
                                brand_matrix_row.AREA_19 = SmartParametersV2016.brandMatrixInvalid;
                            }
                            break;
                        case "AREA_20":
                            if (brand_matrix_found[0].AREA_20 == SmartParametersV2016.brandMatrixValid)
                            {
                                sam_count = sam_count + 1;
                                brand_matrix_row.AREA_20 = SmartParametersV2016.brandMatrixValid;
                            }
                            else
                            {
                                brand_matrix_row.AREA_20 = SmartParametersV2016.brandMatrixInvalid;
                            }
                            break;
                        case "AREA_21":
                            if (brand_matrix_found[0].AREA_21 == SmartParametersV2016.brandMatrixValid)
                            {
                                sam_count = sam_count + 1;
                                brand_matrix_row.AREA_21 = SmartParametersV2016.brandMatrixValid;
                            }
                            else
                            {
                                brand_matrix_row.AREA_21 = SmartParametersV2016.brandMatrixInvalid;
                            }
                            break;
                        case "AREA_22":
                            if (brand_matrix_found[0].AREA_22 == SmartParametersV2016.brandMatrixValid)
                            {
                                sam_count = sam_count + 1;
                                brand_matrix_row.AREA_22 = SmartParametersV2016.brandMatrixValid;
                            }
                            else
                            {
                                brand_matrix_row.AREA_22 = SmartParametersV2016.brandMatrixInvalid;
                            }
                            break;
                        case "AREA_23":
                            if (brand_matrix_found[0].AREA_23 == SmartParametersV2016.brandMatrixValid)
                            {
                                sam_count = sam_count + 1;
                                brand_matrix_row.AREA_23 = SmartParametersV2016.brandMatrixValid;
                            }
                            else
                            {
                                brand_matrix_row.AREA_23 = SmartParametersV2016.brandMatrixInvalid;
                            }
                            break;
                        default:
                            break;
                    }
                }
                brand_matrix_new = brand_matrix_row;
                return true;
            }
            return false;
        }

        public static bool USW_Suppliers_SIC(MainProcess MainForm,
                                                    SqlConnection SmartUtilityConnection,
                                                    MainViewModel ourviewmodel,
                                                    DashboardModel dashboardmodel,
                                                    DashBored Mally,
                                                    string username,
                                                    string utility_pathname,
                                                    string yymmdd_format,
                                                    string defaultDates,
                                                    DateTime defaultDate,
                                                    DateTime final_date)
        {
            // Set the Supplier based on the index selected
            dashboardmodel.supplier_code = 0;
            if (!DashboardUtilityV2018.Lookup_Supplier_Or_Brand_Code(MainForm.textBoxConsole, SmartUtilityConnection, SmartParametersV2016.Utility, MainForm.comboBoxUSW_Suppliers.Text, dashboardmodel, true))
            {
                MainProcess.Output_Message(MainForm.richTextBoxUSW,
                                   "Brand Code zero for: " + SmartParametersV2016.Utility + " " + MainForm.comboBoxUSW_Suppliers.Text,
                                    Mally.examine.scrape, Mally.console);
                return false;
            }
            dashboardmodel.brand_code = 0;
            if (!DashboardUtilityV2018.Lookup_Supplier_Or_Brand_Code(MainForm.textBoxConsole, SmartUtilityConnection, SmartParametersV2016.Utility, MainForm.comboBoxUSW_Suppliers.Text, dashboardmodel, false))
            {
                MainProcess.Output_Message(MainForm.richTextBoxUSW,
                                   "Brand Code zero for: " + SmartParametersV2016.Utility + " " + MainForm.comboBoxUSW_Suppliers.Text.Trim('~'),
                                    Mally.examine.scrape, Mally.console);
                return false;
            }
            DashboardUtilityV2018.Update_Big4(MainForm.Utility_Area_Code, MainForm.Utility_Supplier_Code, MainForm.Utility_Tariff_Code, MainForm.Utility_Resource_Type, dashboardmodel.area_code, dashboardmodel.supplier_code, dashboardmodel.brand_code, dashboardmodel.tariff_code, dashboardmodel.resource_type);

            // Always talking herself up as per usual - Look At Me!  How fantastic I am!
            //SmartUtilityV2018.Check_Selected_Text(MainForm.comboBoxSuppliers);
            //MainForm.comboBoxSuppliers.Text = MainForm.comboBoxSuppliers.Text.Trim('~');

            DateTime prices_valid_from = defaultDate;

            string dashboard_name = DashboardUtilityV2018.Lookup_Brand_Name(MainForm.textBoxConsole,
                                                                        SmartUtilityConnection,
                                                                        SmartParametersV2016.Utility,
                                                                        dashboardmodel.brand_code,
                                                                        "USWITCH");   // Says it all
            string local_tariff_name = string.Empty;
            SmartUtility.BrandMatrix brand_matrix_row = new SmartUtility.BrandMatrix();

            bool multiple_shot = false;
            if (MainForm.radioButtonUSW_Multiple.Checked)
            {
                multiple_shot = true;
            }
            bool special = false;
            if (MainForm.radioButtonUSW_Special.Checked)
            {
                special = true;
                multiple_shot = true;
            }

            if (multiple_shot || special)
            {
                decimal uplift = 0.0M;
                if (MainForm.textBoxUSW_Uplift.Text.Length != 0)
                {
                    uplift = Convert.ToDecimal(MainForm.textBoxUSW_Uplift.Text);
                    uplift = 1.0M + uplift / 100.0M;
                }

                string usw_area = string.Empty;
                if (MainForm.textBoxUSW_Area.Text.Length != 0)
                {
                    usw_area = MainForm.textBoxUSW_Area.Text;
                }
            }
            // Build the tariffs for this Supplier
            MainForm.comboBoxUSW_Tariffs.Items.Clear();
            DashboardUtilityV2018.Build_Tariff_Dropdown(MainForm.textBoxConsole,
                                                SmartUtilityConnection,
                                                SmartParametersV2016.Utility,
                                                MainForm.comboBoxUSW_Tariffs,
                                                dashboardmodel.area_code,
                                                dashboardmodel.brand_code,
                                                dashboardmodel.resource_code,
                                                dashboardmodel.resource_type,
                                                dashboardmodel.tariff_code,
                                                dashboardmodel.version_code,
                                                false);       // Don't include deactivated items
            return true;
        }

        public static async Task<bool> DoSuck_USwitch(SqlConnection SmartUtilityConnection,
                                                MainProcess components,
                                                SignInViewModel signinviewmodel,
                                                MainViewModel ourviewmodel,
                                                DashboardModel dashboardmodel,
                                                RichTextBox Finance_Log,
                                                string start_routine,
                                                bool multiple_shot,
                                                bool special,
                                                string usw_filepath,
                                                string usw_website,
                                                DashBored Mally,
                                                RichTextBox textBoxConsole,
                                                string yymmdd_format,
                                                char category_code,
                                                char resource_code,
                                                string resource_type,
                                                string defaultDates,
                                                DateTime defaultDate,
                                                string brand_name,
                                                string dashboard_name,
                                                string scraper_name,
                                                short supplier_code,
                                                short brand_code,
                                                short version_code,
                                                string local_tariff_name,
                                                string alternative_tariff_name,
                                                int tariff_code,
                                                DateTime prices_valid_from,
                                                DateTime final_date,
                                                decimal uplift,
                                                string usw_area,
                                                int target_row,
                                                int colourindex,
                                                int blackindex,
                                                int sam_count,
                                                SmartUtility.BrandMatrix brand_matrix_row,
                                                CheckedListBox Brand_Matrix,
                                                CheckedListBox Tariffs_Areas_Matrix,
                                                CheckBox EnergyUpdates,
                                                ListView Conditions_Groups_Tariff_Plans,
                                                ListView Conditions_Plans_Limits,
                                                ListView Payments_Tariff_Plans,
                                                ComboBox comboBoxUSW,
                                                ComboBox comboBoxUSW_Tariffs,
                                                Label labelUSW_VATRate,
                                                CheckBox checkBoxDF,
                                                string last_target)
        {
            bool task = await Read_Website(SmartUtilityConnection,
                                                components,
                                                signinviewmodel,
                                                ourviewmodel,
                                                dashboardmodel,
                                                Finance_Log,
                                                start_routine,
                                                multiple_shot,
                                                special,
                                                usw_filepath,
                                                usw_website,
                                                usw_website,
                                                Mally,
                                                textBoxConsole,
                                                yymmdd_format,
                                                category_code,
                                                resource_code,
                                                resource_type,
                                                defaultDates,
                                                defaultDate,
                                                brand_name,
                                                dashboard_name,
                                                scraper_name,
                                                supplier_code,
                                                brand_code,
                                                version_code,
                                                final_date,
                                                local_tariff_name,
                                                alternative_tariff_name,
                                                tariff_code,
                                                prices_valid_from,
                                                uplift,
                                                usw_area,
                                                target_row,
                                                colourindex,
                                                blackindex,
                                                sam_count,
                                                brand_matrix_row,
                                                Brand_Matrix,
                                                Tariffs_Areas_Matrix,
                                                EnergyUpdates,
                                                Conditions_Groups_Tariff_Plans,
                                                Conditions_Plans_Limits,
                                                Payments_Tariff_Plans,
                                                comboBoxUSW,
                                                comboBoxUSW_Tariffs,
                                                labelUSW_VATRate,
                                                checkBoxDF,
                                                last_target);
            if (!SmartDatabaseV2016.Add_Tick_Or_Cross(textBoxConsole,
                                                task,
                                                "USWITCH      : " + "Finished at " + DateTime.Now.ToString(yymmdd_format),
                                                Mally))
            {
                return false;
            }
            // Now!  Old misery guts is telling me aboout 'how children play'!!!  As if I had
            // never ever been a child myself!!!
            return task;
        }

        public static async Task<bool> USW_Tariffs_SIC(SqlConnection SmartUtilityConnection,
                                                    MainProcess components,
                                                    RichTextBox Finance_Log,

                                                    SignInViewModel signinviewmodel,
                                                    MainViewModel ourviewmodel,
                                                    DashboardModel dashboardmodel,
                                                    string username,
                                                    DashBored Mally,
                                                    RichTextBox textBoxConsole,
                                                    string utility_pathname,
                                                    string yymmdd_format,
                                                    string defaultDates,
                                                    DateTime defaultDate,
                                                    DateTime final_date)
        {
            SmartDatabaseV2016.Build_VAT_List(textBoxConsole, SmartParametersV2016.Utility, ourviewmodel.Blanche.vatRatesList);

            DateTime prices_valid_from = defaultDate;

            string dashboard_name = DashboardUtilityV2018.Lookup_Brand_Name(textBoxConsole,
                                                                        SmartUtilityConnection,
                                                                        SmartParametersV2016.Utility,
                                                                        dashboardmodel.brand_code,
                                                                        "USWITCH");   // Says it all

            string scraper_name = DashboardUtilityV2018.Lookup_Brand_Name(textBoxConsole,
                                                                     SmartUtilityConnection,
                                                                     SmartParametersV2016.Utility,
                                                                     dashboardmodel.brand_code,
                                                                     "SCRAPER");   // Says it all

            // Set the Tariff based on the index selected
            if (!DashboardUtilityV2018.Lookup_New_Tariff_Code(dashboardmodel,
                                                            textBoxConsole,
                                                            SmartUtilityConnection,
                                                            components.TariffsMatrixVersionListBox,
                                                            dashboardmodel.area_code,
                                                            dashboardmodel.supplier_code,
                                                            dashboardmodel.brand_code,
                                                            dashboardmodel.resource_code,
                                                            dashboardmodel.resource_type,
                                                            components.comboBoxUSW_Tariffs.Text.Trim('~').Trim(SmartParametersV2016.placeholderDesignation)))
            {
                return false;
            }
            DashboardUtilityV2018.Update_Big4(components.Utility_Area_Code, components.Utility_Supplier_Code, components.Utility_Tariff_Code, components.Utility_Resource_Type, dashboardmodel.area_code, dashboardmodel.supplier_code, dashboardmodel.brand_code, dashboardmodel.tariff_code, dashboardmodel.resource_type);

            string[] fields;

            string local_tariff_name = string.Empty;

            // Look for all the Tariffs in this Supplier
            fields = new String[8];
            fields[0] = "SUPPLIER_CODE";
            fields[1] = dashboardmodel.supplier_code.ToString();
            fields[2] = "RESOURCE_CODE";
            fields[3] = dashboardmodel.resource_code.ToString();
            fields[4] = "RESOURCE_TYPE";
            fields[5] = dashboardmodel.resource_type;
            fields[6] = "TARIFF_CODE";
            fields[7] = dashboardmodel.tariff_code.ToString();
            ObservableCollection<SmartUtility.Tariffs> tariffs_found = SmartDatabaseV2016.READ_Records<SmartUtility.Tariffs>(textBoxConsole,
                            SmartUtilityConnection,
                            MainProcess.global_utility_tablesList, fields,
                            String.Empty,
                            new SmartUtility.Tariffs());
            // Should only be one?
            if (tariffs_found.Count() == 1)
            {
                prices_valid_from = tariffs_found[0].PRICES_VALID_FROM;

                // Look for all the Tariffs in this Supplier
                fields = new String[10];
                fields[0] = "BRAND_CODE";
                fields[1] = dashboardmodel.brand_code.ToString();
                fields[2] = "RESOURCE_CODE";
                fields[3] = dashboardmodel.resource_code.ToString();
                fields[4] = "RESOURCE_TYPE";
                fields[5] = dashboardmodel.resource_type;
                fields[6] = "TARIFF_CODE";
                fields[7] = dashboardmodel.tariff_code.ToString();
                fields[8] = "VERSION_CODE";
                fields[9] = dashboardmodel.version_code.ToString();
                ObservableCollection<SmartUtility.TariffMatrix> tariff_matrix_found = SmartDatabaseV2016.READ_Records<SmartUtility.TariffMatrix>(textBoxConsole,
                                                SmartUtilityConnection,
                                                MainProcess.global_utility_tablesList,
                                                fields,
                                                String.Empty,
                                                new SmartUtility.TariffMatrix());
                if (tariff_matrix_found.Count() == 1)
                {
                    local_tariff_name = tariff_matrix_found[0].TARIFF_NAME;
                    components.textBoxUSW_Tariff_Name.Text = local_tariff_name;
                }
            }

            components.labelUSW_Prices_Valid_From.Text = "Prices valid from: " + prices_valid_from.ToString();

            // Need to find out from the Brand Matrix how many areas are ticked                
            // Lookup all the Areas for this Brand
            // BUT If the Brand = Supplier, then fix it so that the Areas are ticked for ALL brands
            int sam_count = 0;
            SmartUtility.BrandMatrix brand_matrix_row = new SmartUtility.BrandMatrix();
            if (dashboardmodel.supplier_code != dashboardmodel.brand_code)
            {
                // We really are doing a Brand here
                if (!Find_Sam_Count(textBoxConsole,
                                    SmartUtilityConnection,
                                    dashboardmodel.brand_code,
                                    brand_matrix_row,
                                    components.Utility_Brands_Areas_Matrix,
                                    ref sam_count,
                                    ref brand_matrix_row))
                {
                    // Need to put an error in here
                    return false;
                }
            }
            else
            {
                if (!Find_Sam_Count(textBoxConsole,
                                SmartUtilityConnection,
                                dashboardmodel.supplier_code,
                                brand_matrix_row,
                                components.Utility_Brands_Areas_Matrix,
                                ref sam_count,
                                ref brand_matrix_row))
                {
                    // And here
                    return false;
                }
            }

            decimal uplift = 0.0M;
            if (components.textBoxUSW_Uplift.Text.Length != 0)
            {
                uplift = Convert.ToDecimal(components.textBoxUSW_Uplift.Text);
                uplift = 1.0M + uplift / 100.0M;
            }

            string usw_area = string.Empty;
            if (components.textBoxUSW_Area.Text.Length != 0)
            {
                usw_area = components.textBoxUSW_Area.Text;
            }

            dashboardmodel.target_count = Convert.ToInt32(components.textBoxUSW_Limit.Text);
            bool multiple_shot = false;
            if (components.radioButtonUSW_Single.Checked)
            {
                multiple_shot = false;
            }
            if (components.radioButtonUSW_Multiple.Checked)
            {
                multiple_shot = true;
            }
            bool special = false;       // Don't do this one here

            string last_target = string.Empty;
            dashboardmodel.one_e_found = false;
            dashboardmodel.one_e7_found = false;

            // For Suppleirs GET SmartParametersV2016.uswitchWebsite + gas-electricity/beta-api/suppliers?regionId=16&fuel=electricity HTTP/1.1

            string usw_filepath = Path.Combine(utility_pathname, "USwitch");
            string usw_website = SmartParametersV2016.uswitchWebsite + "gas-electricity/beta-api/plans?"; // <== where are /resources/v1 in this??

            TimeSpan timespanTimeout = SmartParametersV2016.timespanTimeout;

            bool result = await DoSuck_USwitch(SmartUtilityConnection,
                                            components,
                                        signinviewmodel,
                                        ourviewmodel,
                                        dashboardmodel,
                                        Finance_Log,
                                        "LOGIN",
                                        multiple_shot,
                                        special,
                                        usw_filepath,
                                        usw_website,
                                        Mally,
                                        textBoxConsole,
                                        yymmdd_format,
                                        SmartParametersV2016.Utility,
                                        dashboardmodel.resource_code,
                                        dashboardmodel.resource_type,
                                        defaultDates,
                                        defaultDate,
                                        components.comboBoxUSW_Suppliers.Text.Trim('~'),
                                        dashboard_name,
                                        scraper_name,
                                        dashboardmodel.supplier_code,
                                        dashboardmodel.brand_code,
                                        dashboardmodel.version_code,
                                        components.comboBoxUSW_Tariffs.Text.Trim('~').Trim(SmartParametersV2016.placeholderDesignation),
                                        local_tariff_name,
                                        dashboardmodel.tariff_code,
                                        prices_valid_from,
                                        final_date,
                                        uplift,
                                        usw_area,
                                        0,                          // target_row,
                                        0,                          // colourindex,
                                        0,                          // blackindex
                                        sam_count,                  // brand_matrix count
                                        brand_matrix_row,   // brand_matrix_Row
                                        components.Utility_Brands_Areas_Matrix,
                                        components.Utility_Tariffs_Areas_Matrix,
                                        components.checkBoxUSW_EnergyUpdates,
                                        components.Conditions_Groups_Tariff_Plans,
                                        components.Conditions_Plans_Limits,
                                        components.Payments_Tariff_Plans,
                                        components.comboBoxUSW_List,
                                        components.comboBoxUSW_Tariffs,
                                        components.labelUSW_VATRate,
                                        components.checkBoxDF,
                                        last_target);

            return true;
        }

        public static void USW_SIC(SqlConnection SmartUtilityConnection,
                                    MainProcess components,
                                    SignInViewModel signinviewmodel,
                                    MainViewModel ourviewmodel,
                                    DashboardModel dashboardmodel,
                                    System.Windows.Forms.RichTextBox Finance_Log,
                                    string[] targets,
                                    string username,
                                    DashBored Mally,
                                    RichTextBox textBoxConsole,
                                    string yymmdd_format,
                                    DateTime defaultDate,
                                    string defaultDates,
                                    int blackindex,
                                    int colourindex,
                                    string utility_pathname,
                                    string smartutility_updates_name,
                                    string smartdatabase_directory_path,
                                    int target_row,
                                    DateTime final_date,
                                    DataTable energyupdates_table,
                                    DataTable newupdates_table)
        {
            int action = Convert.ToInt32(targets[1]);

            dashboardmodel.supplier_code = Convert.ToInt16(targets[2]);
            dashboardmodel.brand_code = Convert.ToInt16(targets[3]);

            string brand_name = targets[4];
            string fuel_type = targets[5];
            string local_tariff_name = targets[6];

            DateTime launched = defaultDate,
                    issued = defaultDate,
                    prices_valid_from = defaultDate,
                    tariff_valid_to = defaultDate,
                    withdrawn = defaultDate;
            string[] fields = new string[2];
            switch (action)
            {
                case 1:
                    launched = Convert.ToDateTime(targets[7]);
                    issued = Convert.ToDateTime(targets[8]);
                    prices_valid_from = Convert.ToDateTime(targets[9]);
                    tariff_valid_to = Convert.ToDateTime(targets[10]);
                    withdrawn = Convert.ToDateTime(targets[11]);
                    break;
                case 3:
                    issued = Convert.ToDateTime(targets[7]);
                    withdrawn = Convert.ToDateTime(targets[8]);
                    break;
                case 2:
                    issued = Convert.ToDateTime(targets[7]);
                    prices_valid_from = Convert.ToDateTime(targets[8]);
                    break;
                // SHe is CONSTANTLY fucking and farting about and giving me the usual
                // unwanted commentary on every fucking churning aspect of her life
                // CHURN CHURN CHURN CHURN CHURN CHURN CHURN CHURN mutter mutter mutter
                // drivel drivel drivel drivel .....
                // Does she EVER just FUCK OFF??????!!!!!!
                default:
                    break;
            }
            // Don't forget this you fucking IMBECILE
            SmartDatabaseV2016.Build_VAT_List(textBoxConsole, SmartParametersV2016.Utility, ourviewmodel.Blanche.vatRatesList);

            // Need to find out from the Supplier Area Matrix how many areas are ticked                
            // Lookup all the Areas for this Brand
            int sam_count = 0;
            SmartUtility.BrandMatrix brand_matrix_row = new SmartUtility.BrandMatrix();

            if (dashboardmodel.supplier_code != dashboardmodel.brand_code)
            {
                // We really are doing a Brand here
                if (!Find_Sam_Count(textBoxConsole,
                                    SmartUtilityConnection,
                                    dashboardmodel.brand_code,
                                    brand_matrix_row,
                                    components.Utility_Brands_Areas_Matrix,
                                    ref sam_count,
                                    ref brand_matrix_row))
                {
                    // Check if sam_count is zero - not worth doing ANYTHING if it is
                    MainProcess.Output_Message(components.richTextBoxUSW,
                                        "BRAND_MATRIX is **EMPTY** - aborting",
                                        Mally.examine.scrape, Mally.console);
                    return;
                }
            }
            else
            {
                if (!Find_Sam_Count(textBoxConsole,
                                    SmartUtilityConnection,
                                    dashboardmodel.supplier_code,
                                    brand_matrix_row,
                                    components.Utility_Brands_Areas_Matrix,
                                    ref sam_count,
                                    ref brand_matrix_row))
                {
                    // Check if sam_count is zero - not worth doing ANYTHING if it is
                    MainProcess.Output_Message(components.richTextBoxUSW,
                                        "BRAND_MATRIX is **EMPTY** - aborting",
                                        Mally.examine.scrape, Mally.console);
                    return;
                }
            }

            // The meaning of life
            dashboardmodel.target_count = 0;  // 14 * 3 (E, E7 and G) * 3 payment types

            // We have a 'good' line (or ar least - one with DATES.
            // Now!!!! Can we automate it????
            // First - can we automatically insert the Tariffs?
            char[] resources = new char[2] { SmartParametersV2016.Electricity, SmartParametersV2016.Gas };
            string[,] meter_type = new string[2, 2] {{ "N", "Y"},   // Only applies for 'E'
                                                    { "N", " "}};  // Only applies for 'G'

            string dual_fuel_only = string.Empty;
            if (fuel_type != "All")
            {
                if (fuel_type.IndexOf("Dual") >= 0)
                {
                    dual_fuel_only = "Y";
                }
                else
                {
                    dual_fuel_only = "N";
                    switch (fuel_type)
                    {
                        case "Electricity":
                            resources[1] = ' ';
                            meter_type[1, 0] = " ";  // Clear down the G E7
                            meter_type[1, 1] = " ";
                            break;
                        case "Gas":
                            resources[0] = ' ';
                            meter_type[0, 0] = " ";  // Clear down the E E7
                            meter_type[0, 1] = " ";
                            break;
                        default:
                            break;
                    }
                }
            }
            else
            {
                dual_fuel_only = "N";
            }

            string target_list = string.Empty;
            // Work out the target list
            foreach (char resource_code in resources)
            {
                if (resource_code != ' ')
                {
                    int meter_index = (resource_code == SmartParametersV2016.Electricity) ? 0 : 1;
                    for (int i = 0; i < 2; i++) // Kludge City!
                    {
                        string meter = meter_type[meter_index, i];
                        if (meter != " ")
                        {
                            if (string.IsNullOrEmpty(target_list))
                            {
                                target_list = resource_code.ToString();
                            }
                            else
                            {
                                target_list = target_list + "|" + resource_code.ToString();
                            }
                            if (meter == "Y")
                            {
                                target_list = target_list + "7";
                            }
                        }
                    }
                }
            }
            string[] target_lists = target_list.Split('|');

            // Reset this ready for any rebuild
            dashboardmodel.target_count = 0;

            if (target_lists.Count() > 0)
            {
                // Only if we have something to do ...
                string last_target = target_lists[target_lists.Count() - 1];   // Get the last - whatever
                dashboardmodel.one_e_found = false;
                dashboardmodel.one_e7_found = false;

                foreach (string target in target_lists)
                {
                    string local_resource_type = "SR";
                    dashboardmodel.resource_code = Convert.ToChar(target.Substring(0, 1));   // Kludge City!
                    MainProcess.Output_Message(textBoxConsole,
                                    "Energy code " + target,
                                    Mally.examine.scrape, Mally.console);

                    if (target.IndexOf("7") != -1)
                    {
                        local_resource_type = "VR";
                    }
                    // 23-Oct-2014 at 10:27am
                    // Who gives a shit about your stupid job??
                    // I HAVE AUTOMATED THE TARIFF LOADING PROCESS AND ITS
                    // NIGH ON FUCKING BRILLIANT!!!  IT DOES **ALL THREE**
                    // RESOURCES 'AT ONCE' !!!!!!!!!!  ITS THE FUCKING DOGS BOLLOCKS!!!
                    // Who needs you cunts???????

                    // Default this here
                    dashboardmodel.version_code = SmartParametersV2016.defaultVersionCode;
                    dashboardmodel.tariff_code = 0;
                    if (!DashboardUtilityV2018.Lookup_New_Tariff_Code(dashboardmodel,
                                                                        textBoxConsole,
                                                                        SmartUtilityConnection,
                                                                        components.TariffsMatrixVersionListBox,
                                                                        dashboardmodel.area_code,
                                                                        dashboardmodel.supplier_code,
                                                                        dashboardmodel.brand_code,
                                                                        dashboardmodel.resource_code,
                                                                        local_resource_type,
                                                                        local_tariff_name))
                    {
                        switch (action)
                        {
                            case 1:
                                // LAUNCH - No Tariff?  We are only allowed this code for a Launch                           
                                dashboardmodel.tariff_code = 0;

                                //dashboardmodel.propogate_tariffs = SmartDatabaseV2016.lookup_supplier_propogate(SmartParametersV2016.Utility, brand_name, spants);
                                if (!USW_Add_Tariff(dashboardmodel,
                                                    SmartUtilityConnection,
                                                    SmartParametersV2016.Utility,
                                                    dashboardmodel.resource_code,
                                                    local_resource_type,
                                                    dashboardmodel.supplier_code,
                                                    dashboardmodel.brand_code,
                                                    dashboardmodel.version_code,
                                                    dashboardmodel.propogate_tariffs,
                                                    brand_name,
                                                    local_tariff_name,
                                                    launched,
                                                    issued,
                                                    prices_valid_from,
                                                    tariff_valid_to,
                                                    withdrawn,
                                                    dual_fuel_only,
                                                    defaultDates,
                                                    SmartParametersV2016.activeStatus,
                                                    components.Utility_Tariffs_Areas_Matrix,
                                                    Mally,
                                                    textBoxConsole))
                                {
                                    MainProcess.Output_Message(textBoxConsole,
                                                target + " " + "Tariff NOT added: " + local_tariff_name,
                                                Mally.examine.scrape, Mally.console);
                                    return;
                                }
                                MainProcess.Output_Message(textBoxConsole,
                                                target + " " + "Tariff added: " + local_tariff_name,
                                                Mally.examine.scrape, Mally.console);
                                break;
                            case 3:
                                // REMOVAL - 
                                MainProcess.Output_Message(textBoxConsole,
                                                target + " " + "Tariff code must exist for Removal or Update " + action,
                                                Mally.examine.scrape, Mally.console);
                                break;

                            case 2:
                                // UPDATE - 
                                // Must have a tariff code if we are Removing or Updating
                                MainProcess.Output_Message(textBoxConsole,
                                                target + " " + "Tariff code must exist for Removal or Update " + action,
                                                Mally.examine.scrape, Mally.console);
                                break;
                        }
                    }
                    else
                    {
                        switch (action)
                        {
                            case 1:
                                // LAUNCH - Cannot have a Launch if it already exists
                                MainProcess.Output_Message(textBoxConsole,
                                            "Action 1: Tariff code already exists " + dashboardmodel.tariff_code + " for Action " + action,
                                            Mally.examine.scrape, Mally.console);
                                // Just in case we have had an update AFTER  the Updates Table
                                DateTime tariff_prices_valid_from = DashboardUtilityV2018.Lookup_Tariff_Prices_Valid_From(textBoxConsole,
                                                                                                                SmartUtilityConnection,
                                                                                                                dashboardmodel.area_code,
                                                                                                                dashboardmodel.supplier_code,
                                                                                                                dashboardmodel.resource_code,
                                                                                                                local_resource_type,
                                                                                                                dashboardmodel.tariff_code,
                                                                                                                dashboardmodel.version_code,
                                                                                                                defaultDate);

                                if (DateTime.Compare(prices_valid_from, tariff_prices_valid_from) > 0)
                                {
                                    MainProcess.Output_Message(textBoxConsole,
                                                "Action 1: Prices valid from altered to " + prices_valid_from + " for Action " + action,
                                                Mally.examine.scrape, Mally.console);
                                }

                                if (((dashboardmodel.resource_code == SmartParametersV2016.Electricity) && (fuel_type == "Electricity") && (target == "E")) ||
                                    ((dashboardmodel.resource_code == SmartParametersV2016.Gas) && (fuel_type == "Gas")))
                                {
                                    MainProcess.Output_Message(textBoxConsole,
                                                "Returning: Energy code " + dashboardmodel.resource_code + " Target " + target + " Fuel Type " + fuel_type,
                                                Mally.examine.scrape, Mally.console);
                                }

                                // We are carrying on - modify the tariff dates where necessary
                                dashboardmodel.tariffs_row = new SmartUtility.Tariffs();
                                if (!DashboardUtilityV2018.Modify_Tariff(dashboardmodel,
                                                                    textBoxConsole,
                                                                    SmartUtilityConnection,
                                                                    false,
                                                                    dashboardmodel.resource_code,
                                                                    dashboardmodel.supplier_code,
                                                                    dashboardmodel.brand_code,
                                                                    local_resource_type,
                                                                    dashboardmodel.tariff_code,
                                                                    dashboardmodel.version_code,
                                                                    local_tariff_name,
                                                                    string.Empty,   // Online_Option
                                                                    string.Empty,   // Dual_Fuel_Only,
                                                                    string.Empty,   // Fallback_Tariff
                                                                    string.Empty,   // Availability
                                                                    string.Empty,   // Variable
                                                                    string.Empty,   // age
                                                                    string.Empty,   // Customer
                                                                    0,              // Paydays
                                                                    string.Empty,   // Status Flag
                                                                    defaultDate,
                                                                    launched,
                                                                    issued,
                                                                    prices_valid_from,
                                                                    tariff_valid_to,
                                                                    withdrawn))
                                {
                                    MainProcess.Output_Message(textBoxConsole,
                                            "Action " + action + ": Tariff NOT FOUND " + local_tariff_name + " for Action " + action,
                                            Mally.examine.scrape, Mally.console);
                                    //return false;
                                }
                                break;
                            case 3:
                                // REMOVAL - Can only have a Removal if it already exists
                                MainProcess.Output_Message(textBoxConsole,
                                            "Action " + action + ": Tariff already exists " + local_tariff_name + " for Action " + action,
                                            Mally.examine.scrape, Mally.console);
                                // We are carrying on - modify the tariff dates where necessary
                                dashboardmodel.tariffs_row = new SmartUtility.Tariffs();
                                if (!DashboardUtilityV2018.Modify_Tariff(dashboardmodel,
                                                                    textBoxConsole,
                                                                    SmartUtilityConnection,
                                                                    false,
                                                                    dashboardmodel.resource_code,
                                                                    dashboardmodel.supplier_code,
                                                                    dashboardmodel.brand_code,
                                                                    local_resource_type,
                                                                    dashboardmodel.tariff_code,
                                                                    dashboardmodel.version_code,
                                                                    string.Empty,   // So it doesn't get updated
                                                                    string.Empty,   // Online_Option
                                                                    string.Empty,   // Dual_Fuel_Only,
                                                                    string.Empty,   // Fallback_Tariff
                                                                    string.Empty,   // Availability
                                                                    string.Empty,   // Variable
                                                                    string.Empty,   // age
                                                                    string.Empty,   // Customer
                                                                    0,              // Paydays
                                                                    string.Empty,   // Status Flag
                                                                    defaultDate,
                                                                    launched,           // does not change
                                                                    issued,             // valid
                                                                    prices_valid_from,  // valid
                                                                    tariff_valid_to,    // does not change
                                                                    withdrawn))          // changes                                                                    
                                {
                                    MainProcess.Output_Message(textBoxConsole,
                                            "Action " + action + ": Tariff NOT FOUND " + dashboardmodel.supplier_code + " " + dashboardmodel.resource_code + " " + local_resource_type + " " + dashboardmodel.tariff_code + " for Action " + action,
                                            Mally.examine.scrape, Mally.console);
                                    //return false;
                                }
                                else
                                {
                                    energyupdates_table.Rows[target_row]["COLOURS"] = colourindex.ToString() + "|" + blackindex.ToString();
                                    MainProcess.Output_Message(textBoxConsole,
                                                        "Updating Energy Updates table",
                                                        Mally.examine.scrape, Mally.console);
                                    MainProcess.Output_Message(textBoxConsole,
                                                    dashboardmodel.resource_code + " Processing next resource",
                                                    Mally.examine.scrape, Mally.console);
                                }

                                break;     // Not break! 'A-cos we are done here
                            case 2:
                                // UPDATE - Can only have an update if it already exists
                                MainProcess.Output_Message(textBoxConsole,
                                            "Action " + action + ": Tariff code already exists " + dashboardmodel.tariff_code + " for Action " + action,
                                            Mally.examine.scrape, Mally.console);
                                // Just in case we have had an update AFTER  the Updates Table
                                tariff_prices_valid_from = DashboardUtilityV2018.Lookup_Tariff_Prices_Valid_From(textBoxConsole,
                                                                                                                SmartUtilityConnection,
                                                                                                                dashboardmodel.area_code,
                                                                                                                dashboardmodel.supplier_code,
                                                                                                                dashboardmodel.resource_code,
                                                                                                                local_resource_type,
                                                                                                                dashboardmodel.tariff_code,
                                                                                                                dashboardmodel.version_code,
                                                                                                                defaultDate);

                                if (DateTime.Compare(prices_valid_from, tariff_prices_valid_from) > 0)
                                {
                                    MainProcess.Output_Message(textBoxConsole,
                                                "Action " + action + ": Prices valid from altered to " + prices_valid_from + " for Action " + action,
                                                Mally.examine.scrape, Mally.console);
                                }

                                // We are carrying on - modify the tariff dates where necessary
                                dashboardmodel.tariffs_row = new SmartUtility.Tariffs();
                                if (!DashboardUtilityV2018.Modify_Tariff(dashboardmodel,
                                                                textBoxConsole,
                                                                SmartUtilityConnection,
                                                                false,
                                                                dashboardmodel.resource_code,
                                                                dashboardmodel.supplier_code,
                                                                dashboardmodel.brand_code,
                                                                local_resource_type,
                                                                dashboardmodel.tariff_code,
                                                                dashboardmodel.version_code,
                                                                local_tariff_name,
                                                                string.Empty,   // Online_Option
                                                                string.Empty,   // Dual_Fuel_Only,
                                                                string.Empty,   // Fallback_Tariff
                                                                string.Empty,   // Availability
                                                                string.Empty,   // Variable
                                                                string.Empty,   // age
                                                                string.Empty,   // Customer
                                                                0,              // Paydays
                                                                string.Empty,   // Status Flag
                                                                defaultDate,
                                                                launched,           // does not change
                                                                issued,             // valid
                                                                prices_valid_from,  // valid
                                                                tariff_valid_to,    // does not change
                                                                withdrawn))          // does not change
                                {
                                    MainProcess.Output_Message(textBoxConsole,
                                            "Action " + action + ": Tariff NOT FOUND " + local_tariff_name + " for Action " + action,
                                            Mally.examine.scrape, Mally.console);
                                    //return false;
                                }
                                break;
                            default:
                                // Nuffink
                                break;
                        }
                    }

                    if (action != 3)
                    {
                        decimal uplift = 0.0M;

                        string usw_area = string.Empty;

                        DashboardUtilityV2018.Update_Big4(components.Utility_Area_Code, components.Utility_Supplier_Code, components.Utility_Tariff_Code, components.Utility_Resource_Type, dashboardmodel.area_code, dashboardmodel.supplier_code, dashboardmodel.brand_code, dashboardmodel.tariff_code, local_resource_type);

                        string meter = string.Empty;
                        if (target.Length > 1)
                        {
                            meter = target.Substring(1, 1);
                        }
                        dashboardmodel.resource_type = (meter == "7") ? "VR" : "SR";
                        //dashboardmodel.version_code = dashboardmodel.version_code;

                        string usw_filepath = Path.Combine(utility_pathname, "USwitch");
                        string usw_website = "https://www.uswitch.co.uk/home_energy";

                        // Make sure we look up 'Telecom Plus' for 'Utility Warehouse'
                        string dashboard_name = DashboardUtilityV2018.Lookup_Brand_Name(textBoxConsole,
                                                                                    SmartUtilityConnection,
                                                                                    SmartParametersV2016.Utility,
                                                                                    dashboardmodel.brand_code,
                                                                                    "USWITCH");    // Return dashboard name

                        string scraper_name = DashboardUtilityV2018.Lookup_Brand_Name(textBoxConsole,
                                                                                    SmartUtilityConnection,
                                                                                    SmartParametersV2016.Utility,
                                                                                    dashboardmodel.brand_code,
                                                                                    "SCRAPER");   // Says it all

                        bool multiple_shot = true;
                        bool special = false;

                        TimeSpan timespanTimeout = SmartParametersV2016.timespanTimeout;


                        var result = DoSuck_USwitch(SmartUtilityConnection,
                                        components,
                                        signinviewmodel,
                                        ourviewmodel,
                                        dashboardmodel,
                                        Finance_Log,
                                        "LOGIN",
                                        multiple_shot,           // always do multiples for these
                                        special,
                                        usw_filepath,
                                        usw_website,
                                        Mally,
                                        textBoxConsole,
                                        yymmdd_format,
                                        SmartParametersV2016.Utility,
                                        dashboardmodel.resource_code,
                                        dashboardmodel.resource_type,
                                        defaultDates,
                                        defaultDate,
                                        brand_name,
                                        dashboard_name,
                                        scraper_name,
                                        dashboardmodel.supplier_code,
                                        dashboardmodel.brand_code,
                                        dashboardmodel.version_code,
                                        local_tariff_name,
                                        local_tariff_name,
                                        dashboardmodel.tariff_code,
                                        prices_valid_from,
                                        final_date,
                                        uplift,
                                        usw_area,
                                        target_row,
                                        colourindex,
                                        blackindex,
                                        sam_count,
                                        brand_matrix_row,
                                        components.Utility_Brands_Areas_Matrix,
                                        components.Utility_Tariffs_Areas_Matrix,
                                        components.checkBoxUSW_EnergyUpdates,
                                        components.Conditions_Groups_Tariff_Plans,
                                        components.Conditions_Plans_Limits,
                                        components.Payments_Tariff_Plans,
                                        components.comboBoxUSW_List,
                                        components.comboBoxUSW_Tariffs,
                                        components.labelUSW_VATRate,
                                        components.checkBoxDF,
                                        last_target);
                        // if YOU PUT CHECKS IN HERE FOR SUCCESS OR FAILUE
                        // THE THE WHOLE FUCKING THING STALLS

                        //if (!result.Result)
                        //{
                        //    MainProcess.Output_Message(MainForm.textBoxUSW,
                        //            "LOGIN failed ....",
                        //            Mally.examine.scrape, Mally.console);
                        //    //return false;
                        //}
                    }
                }
                if (action == 3)
                {
                    MainProcess.Output_Message(textBoxConsole,
                                    "Processing next tariff",
                                    Mally.examine.scrape, Mally.console);
                    components.comboBoxUSW_List.SelectedIndex = components.comboBoxUSW_List.SelectedIndex + 1;
                }
            }
            return;
        }
    }
}