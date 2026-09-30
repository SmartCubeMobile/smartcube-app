// ©Ray Chapman 2010,2011,2012,2013,2014,2015,2016,2017
// No portion of this code may be copied or modified in any way, shape or form by
// any means whatsoever without the express written permission of the author.
// Which you are never going to get so don't ask
using SmartCubeMobile;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using Microsoft.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SmartDashboard
{
    class energylinxV2016

    {
        
        public static async Task<bool> Read_Website(MainProcess MainForm,
                                                    SqlConnection SmartUtilityConnection,
                                                    SignInViewModel signinviewmodel,
                                                    MainViewModel ourviewmodel,
                                                    DashboardModel dashboardmodel,
                                                    CancellationToken cancellation_token,
                                                    string start_routine,
                                                    bool multiple_shot,       // False = only do current, True = update index
                                                    bool missing,           // True - loop round all suppliers collecting tariffs
                                                    string username,
                                                    string el_filepath,
                                                    string website,
                                                    string website_resource,
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
                                                    short supplier_code,
                                                    short brand_code,
                                                    short version_code,
                                                    DateTime final_date,
                                                    string tariff_name,
                                                    string alternative_tariff_name,
                                                    int tariff_code,
                                                    DateTime prices_valid_from,
                                                    decimal uplift,
                                                    string el_area,
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
                                                    ComboBox comboBoxEL,
                                                    ComboBox comboBoxEL_Tariffs,
                                                    Label ELVAT_Rate,
                                                    string last_target)
        {
            string routine = MethodBase.GetCurrentMethod().Name.ToUpper();
            // The gurgling fawning laugh

            // To Find out what might have gone wrong
            ourviewmodel.errorMessage = SmartNibbyV2016.Check_Timeout(ourviewmodel);
            if (ourviewmodel.errorMessage != string.Empty)
            {
                // The timeout has to be 0 < timeout <= 120 for us to continue ... and its not!
                MainProcess.Output_Message(textBoxConsole,
                                    ourviewmodel.errorMessage,
                                    Mally.examine.scrape, Mally.console);
                return false;
            }


            DashboardModel MES = new DashboardModel()
            {
                connection_timeout = SmartParametersV2016.serverTimeoutSecs,
                brand_code = brand_code,
                supplier_code = supplier_code
            };

            bool use_proxy = false;
            TimeSpan time_difference = new TimeSpan(0, 0, 0, 0);
            bool TextBox_Active = false;



            // To carry cookies across all GET and POST calls
            // ... so the Cookies ARE IMPORTANT because clearing them down after
            // the login, loses the fact that you are Logged-in i.e. ACCSUMMY keeps
            // returning the Login page  YOU NEED THE FUCKING COOKIES
            //                           ============================

            // Because Sometimes we are returned a Document
            HtmlAgilityPack.HtmlDocument html_document = new HtmlAgilityPack.HtmlDocument();
            HtmlAgilityPack.HtmlDocument html_ajax_document = new HtmlAgilityPack.HtmlDocument();

            List<string> headers = new List<string>();

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

                MainProcess.Output_Message(textBoxConsole,
                                    "Next routine " + next_routine + " at " + time_now.ToString(yymmdd_format) + " URL: " + website_resource,
                                    Mally.examine.scrape, Mally.console);
                if (string.IsNullOrEmpty(website_resource))
                {
                    // We can gt away with lots .. but not without this!!!
                    MainProcess.Output_Message(textBoxConsole,
                                       "Cannot continue with website null or empty ",
                                       Mally.examine.scrape, Mally.console);
                }

                switch (next_routine)
                {
                    // The last piece of the jigsaw!!!!
                    case "UPDATE":

                        headers.Clear();

                        target_string = website_resource;
                        Uri target_url = new Uri(target_string);
                        // More fucking hoops
                        if (Mally.console)
                        {
                            // Synchronous for the Console application - use a lambda expression to
                            // help us return the 'void' type declared for HTTP_GET_ASYNC
                            html_document = await SmartBobV2017.HTTPCLIENT_GET_SYNC_SPECIAL(ourviewmodel,
                                                                        cancellation_token,
                                                                        target_url);
                        }
                        else
                        {
                            // Asynchronous for the Form application
                            html_document = await SmartBobV2017.HTTPCLIENT_GET_ASYNC(ourviewmodel,
                                                                        target_url,
                                                                        cancellation_token,
                                                                        headers);

                        }
                        if (!string.IsNullOrEmpty(ourviewmodel.errorMessage))
                        {
                            MainProcess.Output_Message(textBoxConsole,
                                         ourviewmodel.errorMessage,
                                         Mally.examine.scrape, Mally.console);
                            keep_looping = false;
                        }
                        if (html_document.RemainderOffset == 0)
                        {
                            keep_looping = false;
                        }
                        else
                        {
                            MES.login_finished = Energy_Updates(dashboardmodel,
                                                                username,
                                                                html_document,
                                                                TextBox_Active,
                                                                Mally,
                                                                textBoxConsole,
                                                                resource_code,
                                                                defaultindex,
                                                                defaultDate,
                                                                EnergyUpdates);
                            next_routine = "LOGOUT";    // true or false we logout
                        }
                        break;
                    case "SUPPLIERS":
                        
                        CookieContainer cookies_suppliers = new CookieContainer();

                        target_string = "https://energysuppliers.energylinx.co.uk/";

                        target_url = new Uri(target_string);
                        headers.Clear();

                        // More fucking hoops
                        html_document = await SmartBobV2017.HTTPCLIENT_GET_ASYNC(ourviewmodel,
                                                                        target_url,
                                                                        cancellation_token,
                                                                        headers);
                        if (!string.IsNullOrEmpty(ourviewmodel.errorMessage))
                        {
                            MainProcess.Output_Message(textBoxConsole,
                                        ourviewmodel.errorMessage,
                                        Mally.examine.scrape, Mally.console);
                            keep_looping = false;
                        }
                        if (html_document.RemainderOffset == 0)
                        {
                            keep_looping = false;
                        }
                        else
                        {
                            MES.login_finished = EL_Find_New_Suppliers(MainForm,
                                                    SmartUtilityConnection,
                                                    username,
                                                    html_document,
                                                    Mally,
                                                    dashboardmodel,        // Because individuals get updated
                                                    textBoxConsole,
                                                    category_code,
                                                    resource_code,
                                                    defaultDates,
                                                    final_date,
                                                    Brand_Matrix);
                            //brands_list);
                            next_routine = "LOGOUT";
                        }



                        target_url = new Uri(target_string);
                        // More fucking hoops
                        headers.Clear();

                        html_document = await SmartBobV2017.HTTPCLIENT_GET_ASYNC(ourviewmodel,
                                                                        target_url,
                                                                        cancellation_token,
                                                                        headers);
                        if (!string.IsNullOrEmpty(ourviewmodel.errorMessage))
                        {
                            MainProcess.Output_Message(textBoxConsole,
                                        ourviewmodel.errorMessage,
                                        Mally.examine.scrape, Mally.console);
                            keep_looping = false;
                        }
                        if (html_document.RemainderOffset == 0)
                        {
                            keep_looping = false;
                        }
                        else
                        {
                            MES.login_finished = EL_Find_Suppliers(MainForm,
                                                    SmartUtilityConnection,
                                                    username,
                                                    html_document,
                                                    Mally,
                                                    dashboardmodel,        // Because individuals get updated
                                                    textBoxConsole,
                                                    category_code,
                                                    resource_code,
                                                    defaultDates,
                                                    final_date,
                                                    Brand_Matrix);
                            next_routine = "LOGOUT";
                        }
                        break;

                    case "SWITCHABLE":

                        target_string = website;
                        target_url = new Uri(target_string);
                        // More fucking hoops
                        headers.Clear();

                        html_document = await SmartBobV2017.HTTPCLIENT_GET_ASYNC(ourviewmodel,
                                                                        target_url,
                                                                        cancellation_token,
                                                                        headers);
                        if (!string.IsNullOrEmpty(ourviewmodel.errorMessage))
                        {
                            MainProcess.Output_Message(textBoxConsole,
                                        ourviewmodel.errorMessage,
                                        Mally.examine.scrape, Mally.console);
                            keep_looping = false;
                        }
                        if (html_document.RemainderOffset == 0)
                        {
                            keep_looping = false;
                        }
                        else
                        {
                            MES.login_finished = EL_Find_Switchable(MainForm,
                                                    SmartUtilityConnection,
                                                    username,
                                                    html_document,
                                                    Mally,
                                                    dashboardmodel,        // Because individuals get updated
                                                    textBoxConsole,
                                                    category_code,
                                                    brand_code,
                                                    defaultDates,
                                                    final_date,
                                                    Brand_Matrix);
                            next_routine = "LOGOUT";
                        }
                        break;

                    case "TARIFFS":
                        CookieContainer cookies_tariffs = new CookieContainer();

                        target_string = website_resource;
                        target_url = new Uri(target_string);
                        // More fucking hoops
                        headers.Clear();

                        html_document = await SmartBobV2017.HTTPCLIENT_GET_ASYNC(ourviewmodel,
                                                                        target_url,
                                                                        cancellation_token,
                                                                        headers);
                        if (!string.IsNullOrEmpty(ourviewmodel.errorMessage))
                        {
                            MainProcess.Output_Message(textBoxConsole,
                                       ourviewmodel.errorMessage,
                                        Mally.examine.scrape, Mally.console);
                            keep_looping = false;
                        }
                        if (html_document.RemainderOffset == 0)
                        {
                            keep_looping = false;
                        }
                        else
                        {
                            MES.login_finished = EL_Find_Tariffs(MainForm,
                                                                SmartUtilityConnection,
                                                                username,
                                                                html_document,
                                                                Mally,
                                                                dashboardmodel,
                                                                textBoxConsole,
                                                                category_code,
                                                                resource_code,
                                                                resource_type,
                                                                defaultDates,
                                                                defaultDate,
                                                                brand_name,  // What we are looking for may not be scrapeable
                                                                Tariffs_Areas_Matrix,
                                                                missing);
                            next_routine = "LOGOUT";
                        }
                        break;
                    case "LOGIN":
                        // Leave this IN HERE otherwise the scrape for E SR and E VR
                        // gets confused!!!  If this IS IN, then we have separate
                        // cookie containers for EACH of the scrapes and that WOEKS!!
                        CookieContainer cookies_login = new CookieContainer();

                        MES.login_finished = true;
                        bool prices_include_vat = true;

                        // ==========> THESE ARE DEFAULTED here <===========
                        short tier_count = 1;
                        short tier_level = 1;

                        
                        // This program is little short of brilliant.  I have just loaded THREE
                        // payment types for each of Electricity, Economy7 and Gas (so that's NINE
                        // in total) ... and they all appear without error!  
                        // What a program!  What a PROGRAMMER!!!!

                        // Find the vat rate
                        dashboardmodel.VAT_RATE = 0.0M;
                        bool foundIt = false;
                        // A copy of the version in SmartParseV2016.Lookup_Vat_Rate
                        foreach (SmartUsers.VatRates vat_rates_row in ourviewmodel.Blanche.vatRatesList)
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
                            ELVAT_Rate.ForeColor = System.Drawing.Color.Red;
                        }
                        else
                        {
                            ELVAT_Rate.ForeColor = System.Drawing.Color.LawnGreen;
                        }
                        ELVAT_Rate.Text = dashboardmodel.VAT_RATE.ToString() + "%";

                        MainProcess.Output_Message(textBoxConsole,
                            "Supplier name " + brand_name,
                            Mally.examine.scrape, Mally.console);

                        MainProcess.Output_Message(textBoxConsole,
                                           "Tariff: " + tariff_name,
                                            Mally.examine.scrape, Mally.console);

                        // This is the tough part ... how do we know the Tariff method is SC??
                        // Think about this Ray, its either SC or 2Tier ... but how do you tell which?
                        //bool single_parse = true;

                        string resource = string.Empty;
                        string path = string.Empty;
                        StreamWriter stream_writer = null;
                        dashboardmodel.errorMessage = string.Empty;

                        path = Path.Combine(el_filepath, brand_name);
                        // When resource_code = "D" that means Dual-Rate i.e. E7
                        if (!File.Exists(path))
                        {
                            // Create a file to write to. 
                            Directory.CreateDirectory(path);
                        }
                        string file_name = tariff_name.Replace("/", "-");   // slashes "/" are bad
                        file_name = file_name.Replace(":", "_");            // as are colons ":" 
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
                        //string path = @"c:\temp\MyTest.txt";

                        if (!File.Exists(path))
                        {
                            // Create a file to write to. 
                            stream_writer = File.CreateText(path);
                            stream_writer.WriteLine("// energylinx Created: " + DateTime.Now.ToString("dd-MMM-yyyy HH:mm:ss") + " ..you ARE a fucking genius, Ray!");
                            stream_writer.Close();
                        }
                        stream_writer = new StreamWriter(path, true);
                        //}
                        //else
                        //{
                        //    path = Path.Combine(el_filepath, DateTime.Now.ToString(SmartParametersV2016.dashes_format));
                        //    // When resource_code = "D" that means Dual-Rate i.e. E7
                        //    switch (resource_code)
                        //    {
                        //        case SmartParametersV2016.Electricity:
                        //            if (resource_type == "VR")
                        //            {
                        //                path = Path.Combine(path, resource_code.ToString() + "7");
                        //                resource = "E7";
                        //            }
                        //            else
                        //            {
                        //                path = Path.Combine(path, resource_code.ToString());
                        //                resource = "E";
                        //            }
                        //            break;
                        //        case SmartParametersV2016.Gas:
                        //            path = Path.Combine(path, resource_code.ToString());
                        //            resource = "G";
                        //            break;
                        //        default:
                        //            break;
                        //    }
                        //    if (!File.Exists(path))
                        //    {
                        //        // Create a file to write to. 
                        //        Directory.CreateDirectory(path);
                        //    }
                        //}

                        
                        bool at_the_top = true;

                        // ===================================================
                        // Energylinx Pay type (go in "existingPayType" field:
                        //
                        // 1	Monthly Direct Debit or Fixed Monthly Direct Debit
                        // 2	Quarterly Direct Debit
                        // 4	Pay on receipt of Bill
                        // 5	Prepayment meter
                        //
                        // The problem is, we don;t know WHICH Monthly DD it is - Variable or Fixed - until we
                        // send "Monthly Direct Debit" to energylink.  If it comes back with a "Fixed" prefix then we need
                        // to change the payment_plans_we_think 'on the fly' to represent not "A Monthly Direct Debit"
                        // but "C Fixed Monthly Direct Debit"
                        // ====================================================

                        string[] plans_to_scrape = new string[5] { "A", "D", "F", "K", "G" };    // The Energylinx Payment codes are 1, 2, 4, 4;11 and 5
                        string payment_name = string.Empty,
                                alternate_name1 = string.Empty,
                                alternate_name2 = string.Empty;
                        foreach (string payment_plan in plans_to_scrape)
                        {
                            string payment_type = string.Empty;
                            string derived_payment_plan = payment_plan;  // Might go in as "A" and come out as "C"

                            List<SmartUtility.PaymentPlans> payment_plans_found = DashboardUtilityV2018.Lookup_Payment_Plan(textBoxConsole, SmartUtilityConnection, payment_plan);
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
                                alternate_name1 = payment_plans_found.First().ALTERNATE_NAME1;
                                alternate_name2 = payment_plans_found.First().ALTERNATE_NAME2;
                                // alternate_name3 only ever used byte SP_Analyze
                                MainProcess.Output_Message(textBoxConsole,
                                                        resource + " Payment Plan: " + payment_name,
                                                        Mally.examine.scrape, Mally.console);
                                if (payment_type != string.Empty)
                                {
                                    bool two_tier = false;  // This MAY NOT BE THE BEST PLACE TO PUT THIS
                                    dashboardmodel.target_count = dashboardmodel.target_count + sam_count;
                                    //set_target_count(get_target_count() + sam_count);

                                    // Clear all the Plans
                                    for (int plan_index = 0; plan_index < Conditions_Groups_Tariff_Plans.Items.Count; plan_index++)
                                    {
                                        Conditions_Groups_Tariff_Plans.Items[plan_index].Checked = false;
                                    }
                                    decimal[,,] unit_rates = new decimal[14, 2, 5]; // Tier 1 and Tier 2

                                    bool payment_type_found = false;
                                    string colname;

                                    for (int i = 0; i < Brand_Matrix.Items.Count; i++)
                                    {
                                        colname = "AREA_" + (i + 10).ToString("00");

                                        //if (!single_parse)
                                        //{
                                        //    string target_path = Path.Combine(path, colname + ".txt");
                                        //    if (!File.Exists(target_path))
                                        //    {
                                        //        // Create a file to write to. 
                                        //        stream_writer = File.CreateText(target_path);
                                        //        stream_writer.WriteLine("// Created: " + DateTime.Now.ToString("dd-MMM-yyyy HH:mm:ss") + " ..you ARE a fucking genius, Ray! SON'T FORGET THESE ARE ALL INC VAT!!");
                                        //        stream_writer.Close();
                                        //    }
                                        //    stream_writer = new StreamWriter(target_path, true);
                                        //}

                                        bool success = false;
                                        switch (colname)
                                        {
                                            case "AREA_10":
                                                if (brand_matrix_row.AREA_10 == SmartParametersV2016.brandMatrixValid)
                                                {
                                                    success = true;
                                                }
                                                break;
                                            case "AREA_11":
                                                if (brand_matrix_row.AREA_11 == SmartParametersV2016.brandMatrixValid)
                                                {
                                                    success = true;
                                                }
                                                break;
                                            case "AREA_12":
                                                if (brand_matrix_row.AREA_12 == SmartParametersV2016.brandMatrixValid)
                                                {
                                                    success = true;
                                                }
                                                break;
                                            case "AREA_13":
                                                if (brand_matrix_row.AREA_13 == SmartParametersV2016.brandMatrixValid)
                                                {
                                                    success = true;
                                                }
                                                break;
                                            case "AREA_14":
                                                if (brand_matrix_row.AREA_14 == SmartParametersV2016.brandMatrixValid)
                                                {
                                                    success = true;
                                                }
                                                break;
                                            case "AREA_15":
                                                if (brand_matrix_row.AREA_15 == SmartParametersV2016.brandMatrixValid)
                                                {
                                                    success = true;
                                                }
                                                break;
                                            case "AREA_16":
                                                if (brand_matrix_row.AREA_16 == SmartParametersV2016.brandMatrixValid)
                                                {
                                                    success = true;
                                                }
                                                break;
                                            case "AREA_17":
                                                if (brand_matrix_row.AREA_17 == SmartParametersV2016.brandMatrixValid)
                                                {
                                                    success = true;
                                                }
                                                break;
                                            case "AREA_18":
                                                if (brand_matrix_row.AREA_18 == SmartParametersV2016.brandMatrixValid)
                                                {
                                                    success = true;
                                                }
                                                break;
                                            case "AREA_19":
                                                if (brand_matrix_row.AREA_19 == SmartParametersV2016.brandMatrixValid)
                                                {
                                                    success = true;
                                                }
                                                break;
                                            case "AREA_20":
                                                if (brand_matrix_row.AREA_20 == SmartParametersV2016.brandMatrixValid)
                                                {
                                                    success = true;
                                                }
                                                break;
                                            case "AREA_21":
                                                if (brand_matrix_row.AREA_21 == SmartParametersV2016.brandMatrixValid)
                                                {
                                                    success = true;
                                                }
                                                break;
                                            case "AREA_22":
                                                if (brand_matrix_row.AREA_22 == SmartParametersV2016.brandMatrixValid)
                                                {
                                                    success = true;
                                                }
                                                break;
                                            case "AREA_23":
                                                if (brand_matrix_row.AREA_23 == SmartParametersV2016.brandMatrixValid)
                                                {
                                                    success = true;
                                                }
                                                break;
                                            default:
                                                break;
                                        }

                                        if (success)
                                        {
                                            if ((el_area != string.Empty) &&
                                                ((i + 10).ToString("00") != el_area))
                                            {
                                                continue;
                                            }

                                            List<KeyValuePair<string, string>> keyValues = new List<KeyValuePair<string, string>>();

                                            String[] fields = new String[0];
                                            List<SmartUtility.SupplyAreas> supply_areas_list = SmartDatabaseV2016.READ_Records<SmartUtility.SupplyAreas>(textBoxConsole,
                                                                        SmartUtilityConnection,
                                                                        MainProcess.global_utility_tablesList,
                                                                        fields,
                                                                        string.Empty,
                                                                        new SmartUtility.SupplyAreas());
                                            string postcode = supply_areas_list[i].POSTCODE.ToString();
                                            if (postcode != string.Empty)
                                            {
                                                short area_code = Convert.ToInt16(i + 10);
                                                switch (resource_code)
                                                {
                                                    case SmartParametersV2016.Electricity:
                                                        keyValues.Add(new KeyValuePair<string, string>("values[WebServiceKey]", "ui282ET066Im"));
                                                        break;
                                                    case SmartParametersV2016.Gas:
                                                        keyValues.Add(new KeyValuePair<string, string>("values[WebServiceKey]", "pUUnPF9roZ5Y"));
                                                        break;
                                                }
                                                keyValues.Add(new KeyValuePair<string, string>("done", "profile"));
                                                keyValues.Add(new KeyValuePair<string, string>("values[referral]", "1631"));


                                                keyValues.Add(new KeyValuePair<string, string>("values[affCustomField]", string.Empty));
                                                keyValues.Add(new KeyValuePair<string, string>("values[affCustomField1]", string.Empty));
                                                keyValues.Add(new KeyValuePair<string, string>("values[affCustomField2]", string.Empty));
                                                keyValues.Add(new KeyValuePair<string, string>("values[affCustomField3]", string.Empty));
                                                keyValues.Add(new KeyValuePair<string, string>("values[affCustomField4]", string.Empty));
                                                keyValues.Add(new KeyValuePair<string, string>("values[affCustomField5]", string.Empty));
                                                keyValues.Add(new KeyValuePair<string, string>("values[affCustomField6]", string.Empty));
                                                keyValues.Add(new KeyValuePair<string, string>("values[referIDcust]", string.Empty));
                                                keyValues.Add(new KeyValuePair<string, string>("values[email]", string.Empty));
                                                keyValues.Add(new KeyValuePair<string, string>("values[gasOrElecOrDual]", string.Empty));
                                                keyValues.Add(new KeyValuePair<string, string>("values[greenRatingSearch]", string.Empty));
                                                keyValues.Add(new KeyValuePair<string, string>("values[unknownUsageQuestions]", string.Empty));
                                                keyValues.Add(new KeyValuePair<string, string>("values[profileIncomplete]", string.Empty));
                                                keyValues.Add(new KeyValuePair<string, string>("values[noQuoteLimits]", string.Empty));
                                                keyValues.Add(new KeyValuePair<string, string>("values[callCentreVersion]", string.Empty));
                                                keyValues.Add(new KeyValuePair<string, string>("values[CCinboundOrOutbound]", string.Empty));
                                                keyValues.Add(new KeyValuePair<string, string>("values[CCactivity]", string.Empty));
                                                keyValues.Add(new KeyValuePair<string, string>("values[testSignup]", string.Empty));
                                                keyValues.Add(new KeyValuePair<string, string>("values[fieldsForResults]", string.Empty));
                                                keyValues.Add(new KeyValuePair<string, string>("values[fieldsExtra]", string.Empty));
                                                keyValues.Add(new KeyValuePair<string, string>("values[houseNameOrNumber]", string.Empty));


                                                // Always the ALTERNATIVE which should be one-to-one with EL
                                                keyValues.Add(new KeyValuePair<string, string>("values[existingSupplierName]", dashboard_name));
                                                // Always the ALTERNATIVE which should be one-to-one with EL
                                                keyValues.Add(new KeyValuePair<string, string>("values[existingTariff]", alternative_tariff_name));

                                                if (payment_plan != "K")
                                                {
                                                    keyValues.Add(new KeyValuePair<string, string>("values[existingPayType]", payment_type));
                                                }
                                                else
                                                {
                                                    keyValues.Add(new KeyValuePair<string, string>("values[existingPayType]", payment_type));
                                                }
                                                switch (resource_code)
                                                {
                                                    case SmartParametersV2016.Electricity:
                                                        //keyValues.Add(new KeyValuePair<string, string>("values[warmHomeDiscountElec]", "NO"));
                                                        //keyValues.Add(new KeyValuePair<string, string>("values[hasSmartMeterElec]", "No"));

                                                        if (resource_type == "VR")
                                                        {
                                                            keyValues.Add(new KeyValuePair<string, string>("values[postcode]", postcode));
                                                            keyValues.Add(new KeyValuePair<string, string>("values[lookupAddress]", supply_areas_list[i].E_ADDRESS.ToString()));

                                                            keyValues.Add(new KeyValuePair<string, string>("values[existingElecTariff]", "2"));
                                                            keyValues.Add(new KeyValuePair<string, string>("values[nightUsePercent]", "55"));
                                                        }
                                                        else
                                                        {
                                                            keyValues.Add(new KeyValuePair<string, string>("values[postcode]", postcode));
                                                            keyValues.Add(new KeyValuePair<string, string>("values[lookupAddress]", supply_areas_list[i].E7_ADDRESS.ToString()));

                                                            keyValues.Add(new KeyValuePair<string, string>("values[existingElecTariff]", "1"));
                                                            keyValues.Add(new KeyValuePair<string, string>("values[nightUsePercent]", "0"));
                                                        }
                                                        keyValues.Add(new KeyValuePair<string, string>("values[elecInputType]", "pounds"));
                                                        keyValues.Add(new KeyValuePair<string, string>("values[elecBill]", "1000"));
                                                        keyValues.Add(new KeyValuePair<string, string>("values[elecBillInterval]", "year"));

                                                        keyValues.Add(new KeyValuePair<string, string>("values[elecUsage]", string.Empty));
                                                        keyValues.Add(new KeyValuePair<string, string>("values[elecUsageInterval]", "year"));
                                                        //keyValues.Add(new KeyValuePair<string, string>("values[unknownUsageTypeElec]", "Use profiling"));
                                                        break;
                                                    case SmartParametersV2016.Gas:
                                                        keyValues.Add(new KeyValuePair<string, string>("values[postcode]", postcode));
                                                        keyValues.Add(new KeyValuePair<string, string>("values[lookupAddress]", supply_areas_list[i].G_ADDRESS.ToString()));
                                                        //keyValues.Add(new KeyValuePair<string, string>("values[warmHomeDiscountGas]", "NO"));
                                                        //keyValues.Add(new KeyValuePair<string, string>("values[hasSmartMeterGas]", "No"));
                                                        keyValues.Add(new KeyValuePair<string, string>("values[existingGasTariff]", "1"));

                                                        keyValues.Add(new KeyValuePair<string, string>("values[gasInputType]", "pounds"));
                                                        keyValues.Add(new KeyValuePair<string, string>("values[gasBill]", "1000"));
                                                        keyValues.Add(new KeyValuePair<string, string>("values[gasBillInterval]", "year"));

                                                        keyValues.Add(new KeyValuePair<string, string>("values[gasUsage]", string.Empty));
                                                        keyValues.Add(new KeyValuePair<string, string>("values[gasUsageInterval]", "year"));
                                                        //keyValues.Add(new KeyValuePair<string, string>("values[unknownUsageTypeGas]", "Use profiling"));
                                                        break;
                                                    default:
                                                        break;
                                                }
                                                keyValues.Add(new KeyValuePair<string, string>("values[email]", string.Empty));
                                                keyValues.Add(new KeyValuePair<string, string>("values[phoneDaytime]", string.Empty));
                                                //keyValues.Add(new KeyValuePair<string, string>("values[hasGreenDeal]", "No"));
                                                //keyValues.Add(new KeyValuePair<string, string>("values[filterOnlineOrOffline]", "ONLINE_AND_OFFLINE"));

                                                if (payment_plan != "K")
                                                {
                                                    keyValues.Add(new KeyValuePair<string, string>("values[desiredPayType]", "1;2;9"));
                                                    //keyValues.Add(new KeyValuePair<string, string>("values[desiredPayIntervalDD]", "All"));
                                                    //keyValues.Add(new KeyValuePair<string, string>("values[desiredPayIntervalBill]", "monthly"));
                                                }
                                                else
                                                {
                                                    keyValues.Add(new KeyValuePair<string, string>("values[desiredPayType]", payment_type));
                                                    //keyValues.Add(new KeyValuePair<string, string>("values[desiredPayIntervalDD]", "Monthly Fixed"));
                                                    //keyValues.Add(new KeyValuePair<string, string>("values[desiredPayIntervalBill]", "monthly"));
                                                }
                                                target_string = website_resource;
                                                target_url = new Uri(target_string);

                                                //FormUrlEncodedContent content = new FormUrlEncodedContent(keyValues);

                                                ourviewmodel.jsonReturned = false;
                                                ourviewmodel.loggedInUser = new Potential();

                                                string json_string = string.Empty;

                                                ourviewmodel.errorMessage = string.Empty;
                                                if (Mally.console)
                                                {
                                                    // Synchronous for the Console application
                                                    html_document = await SmartBobV2017.HTTPCLIENT_POST_SYNC_SPECIAL(ourviewmodel,
                                                                                                        cancellation_token,
                                                                                                        keyValues, //content,
                                                                                                        target_url,
                                                                                                        string.Empty);
                                                }
                                                else
                                                {
                                                    // Asynchronous for the Form application
                                                    html_document = await SmartBobV2017.HTTPCLIENT_POST_ASYNC(ourviewmodel,
                                                                                                            target_url,
                                                                                                            cancellation_token,
                                                                                                            keyValues, //content,
                                                                                                            headers,
                                                                                                            new Guid());
                                                }
                                                if (!string.IsNullOrEmpty(ourviewmodel.errorMessage))
                                                {
                                                    MainProcess.Output_Message(textBoxConsole,
                                                                ourviewmodel.errorMessage,
                                                                Mally.examine.scrape, Mally.console);
                                                    keep_looping = false;
                                                }
                                                if (html_document.RemainderOffset == 0)
                                                {
                                                    keep_looping = false;
                                                }
                                                else
                                                {
                                                    // If EL_Button returns string.Empty, then that means
                                                    // that FOR THIS PAYMENT TYPE, there was no 'More Info' button
                                                    // i.e. this Payment Type is not valid
                                                    //string onclick_value = EL_Button(html_document);
                                                    // Can we match on Supplier and Tariff???
                                                    // If EL_Button returns string.Empty, then that means
                                                    // that FOR THIS PAYMENT TYPE, there was no 'More Info' button
                                                    // i.e. this Payment Type is not valid
                                                    string onclick_value = EL_Button(html_document);
                                                    int echtml = onclick_value.IndexOf("?");
                                                    if (echtml >= 0)
                                                    {
                                                        // Replace the header
                                                        onclick_value = website + onclick_value.Substring(echtml);
                                                        // Remove the trailer
                                                        onclick_value = onclick_value.Replace("');", string.Empty);

                                                        target_string = onclick_value;
                                                        target_url = new Uri(target_string);
                                                        // More fucking hoops
                                                        headers.Clear();

                                                        if (Mally.console)
                                                        {

                                                            // Synchronous for the Console application
                                                            html_ajax_document = await SmartBobV2017.HTTPCLIENT_GET_SYNC_SPECIAL(ourviewmodel,
                                                                                                                            cancellation_token,
                                                                                                                            target_url);

                                                        }
                                                        else
                                                        {
                                                            // Asynchronous for 
                                                            html_ajax_document = await SmartBobV2017.HTTPCLIENT_GET_ASYNC(ourviewmodel,
                                                                                                                           target_url,
                                                                                                                           cancellation_token,
                                                                                                                           headers);
                                                        }
                                                        if (!string.IsNullOrEmpty(ourviewmodel.errorMessage))
                                                        {
                                                            MainProcess.Output_Message(textBoxConsole,
                                                                        ourviewmodel.errorMessage,
                                                                        Mally.examine.scrape, Mally.console);
                                                            keep_looping = false;
                                                        }
                                                        if (html_ajax_document.RemainderOffset == 0)
                                                        {
                                                            keep_looping = false;
                                                        }
                                                        else
                                                        {
                                                            string rhs = string.Empty;

                                                            // This only return 'true' if at least ONE tariff comparison was found
                                                            // otherwise is returns 'false'
                                                            // Also returns false for 'Complex'
                                                            if (EL_Decipher(username,
                                                                            el_filepath,
                                                                            stream_writer,
                                                                            resource_code,
                                                                            brand_name,
                                                                            payment_name,
                                                                            payment_type,
                                                                            alternate_name1,
                                                                            alternate_name2, // Alternate 3 only ever used by SP_Analyze
                                                                            prices_valid_from,
                                                                            html_document,
                                                                            html_ajax_document,
                                                                            Mally,    // Because individuals get updated - well I can't see that anything changes in there now?
                                                                            TextBox_Active,
                                                                            textBoxConsole,
                                                                            time_now,
                                                                            area_code,
                                                                            resource_type,
                                                                            ref rhs,
                                                                            ref at_the_top,
                                                                            ref two_tier,
                                                                            ref payment_type_found,
                                                                            ref derived_payment_plan,   // Might go in as "A" (Variable) and come out as "C" (Fixed)
                                                                            Tariffs_Areas_Matrix,       // This is the CheckBox not the mask
                                                                            Brand_Matrix))    // This is the CheckBox not the mask
                                                            {
                                                                // We came back true
                                                                dashboardmodel.target_count = dashboardmodel.target_count - 1;

                                                                // We need to update the Tariffs Matrix for all areas found
                                                                // and then the Suppliers Matrix
                                                                // Here
                                                                if (payment_type_found)
                                                                {
                                                                    // CHECK
                                                                    string[] comp = rhs.Split(' ');
                                                                    if (comp.Length != 5)
                                                                    {
                                                                        MainProcess.Output_Message(textBoxConsole,
                                                                                resource + " Not enough components to proceed: " + rhs + " " + dashboardmodel.errorMessage + " continuing ...",
                                                                                Mally.examine.scrape, Mally.console);
                                                                        comp = rhs.Split(',');
                                                                        if (comp.Length != 5)
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
                                                                    if (Blah(rhs, ref unit_rates, area_code, prices_include_vat, dashboardmodel.VAT_RATE, ref tier_count))
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
                                                            else
                                                            {
                                                                // We came back false STILL reduce it!!
                                                                dashboardmodel.target_count = dashboardmodel.target_count - 1;
                                                            }
                                                        }
                                                    }
                                                    else
                                                    {
                                                        // There is no button STILL reduce it!!
                                                        dashboardmodel.target_count = dashboardmodel.target_count - 1;
                                                    }
                                                }
                                            }
                                        }
                                    } // End foreach brand_matrix

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
                                            List<SmartUtility.PaymentPlans> derived_payment_plans_found = DashboardUtilityV2018.Lookup_Payment_Plan(textBoxConsole, SmartUtilityConnection, derived_payment_plan);
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
                                                                    resource + " Supplier: " + brand_name + " " + dashboardmodel.errorMessage,
                                                                    Mally.examine.scrape, Mally.console);
                                            MES.login_finished = false;
                                            break;
                                        }
                                        else
                                        {
                                            MainProcess.Output_Message(textBoxConsole,
                                                                    resource + " Supplier: " + brand_name + " " + dashboardmodel.errorMessage,
                                                                    Mally.examine.scrape, Mally.console);
                                            // Create the new Plans (clears down Conditions_Groups_Tariff_Plans)
                                            DashboardUtilityV2018.Add_Tariff_Plans(dashboardmodel,
                                                                                textBoxConsole,
                                                                                SmartUtilityConnection,
                                                                                supplier_code,
                                                                                brand_name,
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
                                                    MES.login_finished = false;
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
                                                            MES.login_finished = false;
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
                                                            if (Conditions_Plans_Limits.Items[limits_index].Text.Contains(conditions_limits_name))
                                                            {
                                                                Conditions_Plans_Limits.Items[limits_index].Checked = true;
                                                                break;
                                                            }
                                                        }
                                                    }
                                                    dashboardmodel.errorMessage = string.Empty;
                                                    dashboardmodel.drop_down_text = string.Empty;
                                                    
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
                                                        MES.login_finished = false;
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
                                }   // payment type 0
                            }
                        }   // End of Payment Plans loop

                        stream_writer.Close();

                        dashboardmodel.errorMessage = string.Empty;
                        if (el_area == string.Empty)
                        {
                            // Any 'deleted' status gets changed to 'active'
                            if (DashboardUtilityV2018.Modify_Tariff_Area(dashboardmodel,
                                                            SmartUtilityConnection,
                                                            textBoxConsole,
                                                            category_code,
                                                            supplier_code,
                                                            0,          // Do all Brands for this Supplier
                                                            resource_code,
                                                            resource_type,
                                                            tariff_code,
                                                            version_code,
                                                            string.Empty,   // Cannot set Placeholder from energylinx!
                                                            Tariffs_Areas_Matrix))
                            {
                                MainProcess.Output_Message(textBoxConsole,
                                                        resource + " Tariff: " + tariff_name + " area matrix updated",
                                                        Mally.examine.scrape, Mally.console);
                            }
                        }
                        else
                        {
                            MainProcess.Output_Message(textBoxConsole,
                                                resource + " Tariff: " + tariff_name + " " + dashboardmodel.errorMessage,
                                                Mally.examine.scrape, Mally.console);
                        }

                        string this_target = resource_code.ToString();
                        if (resource_type == "VR")
                        {
                            this_target = this_target + "7";
                        }

                        // The way this should work is that IF we have a Tariff in the Tariffs combo
                        // then we either do a SINGLE tariff OR we do MULTIPLE tariffs de[ending on the switch
                        if (comboBoxEL_Tariffs.SelectedIndex != -1)
                        {
                            dashboardmodel.target_count = dashboardmodel.target_count - 1;
                            if (dashboardmodel.target_count > 0 || multiple_shot)
                            {
                                MainProcess.Output_Message(textBoxConsole,
                                                            resource + " Processing next tariff",
                                                            Mally.examine.scrape, Mally.console);
                                // Only do the next Tariff if we can
                                if (comboBoxEL_Tariffs.SelectedIndex < comboBoxEL_Tariffs.Items.Count - 1)
                                {
                                    comboBoxEL_Tariffs.SelectedIndex = comboBoxEL_Tariffs.SelectedIndex + 1;
                                }
                            }
                        }
                        else
                        {
                            // There isn't a Tariff in the Tariffs Box - we are doing the EL Box
                            // We ALWAYS do Multiple shots for this box

                            if ((
                                (Mally.console == false) && (dashboardmodel.target_count == 0)) ||
                                (
                                // Cos we should go E, E7 and then G   REDO THIS!!!!  ITS DONE
                                (Mally.console == true) && (last_target == this_target) && (dashboardmodel.target_count == 0)))
                            {
                                DataTable energyupdates_table = dashboardmodel.energyupdates_table;
                                energyupdates_table.Rows[target_row]["COLOURS"] = colourindex.ToString() + "|" + defaultindex.ToString();
                                dashboardmodel.energyupdates_table = energyupdates_table;
                                MainProcess.Output_Message(textBoxConsole,
                                                    "Updating Energy Updates table",
                                                    Mally.examine.scrape, Mally.console);
                                //if ((resource_code == SmartParametersV2016.Electricity) &&     // Don't do this if we are doing 'G' ...
                                //    (get_e_found() == true) &&
                                //    (get_e7_found() == false))
                                //{
                                //    // WE DIDN'T FIND ONE E7!!!!
                                //    SmartUtility.Tariffs tariffs_row = new SmartUtility.Tariffs();
                                //    MainProcess.Output_Message(textBoxConsole,
                                //                    "We didn't find ONE E7 ... is the problem in here?",
                                //                    Mally.examine.scrape, Mally.console);

                                //    if (!SmartUtilityV2018.Modify_Tariff(textBoxConsole,
                                //                                true,
                                //                                resource_code,
                                //                                supplier_code,
                                //                                brand_code,
                                //                                resource_type,
                                //                                tariff_code,
                                //                                version_code,
                                //                                string.Empty,   // Tariff name
                                //                                string.Empty,   // Online_Option
                                //                                string.Empty,   // Dual_Fuel_Only,
                                //                                string.Empty,   // Fallback_Tariff
                                //                                string.Empty,   // Availability
                                //                                string.Empty,   // Variable
                                //                                string.Empty,   // age
                                //                                string.Empty,   // Customer
                                //                                string.Empty,   // Status Flag
                                //                                defaultDate,
                                //                                defaultDate,
                                //                                defaultDate,
                                //                                defaultDate,
                                //                                defaultDate,
                                //                                defaultDate,
                                //                                rf tariffs_row))
                                //    {
                                //        MainProcess.Output_Message(textBoxConsole,
                                //                       resource + " E7 Tariff row not found",
                                //                       Mally.examine.scrape, Mally.console);
                                //    }
                                //    else
                                //    {
                                //        MainProcess.Output_Message(textBoxConsole,
                                //                        resource + " Economy7 reset " + tariff_name,
                                //                        Mally.examine.scrape, Mally.console);
                                //    }
                                //}
                                MainProcess.Output_Message(textBoxConsole,
                                                        resource + " Processing next tariff",
                                                        Mally.examine.scrape, Mally.console);
                                comboBoxEL.SelectedIndex = comboBoxEL.SelectedIndex + 1;
                            }
                        }
                        keep_looping = false;
                        next_routine = "LOGOUT";
                        break;

                    case "LOGOUT":
                        keep_looping = false;
                        break;
                    default:
                        break;
                }
            }
            return MES.login_finished;
        }

        public static bool Blah(string rhs,
                            ref decimal[,,] unit_rates,
                            short area_code,
                            bool prices_include_vat,
                            decimal vat_rate,
                            ref short tier_count)
        {
            // true ... but we might not have found the payment type
            string[] components = rhs.Split(' ');
            int component = 0;
            for (int i = 0; i < 5; i++)     // Surely can 'paramterize' this
            {
                unit_rates[area_code - 10, 0, i] = 0.0M;    // Tier 1
                unit_rates[area_code - 10, 1, i] = 0.0M;    // Tier 2
            }

            string tier1 = string.Empty;
            string tier2 = string.Empty;
            int tier_index = 0;

            while (component < components.Length) //Count())
            {
                tier2 = string.Empty;
                tier_index = components[component].IndexOf("/");
                if (tier_index >= 0)
                {
                    // We have a Two-Tier
                    tier1 = components[component].Substring(0, tier_index);
                    tier2 = components[component].Substring(tier_index + 1);
                    // Tell the outside world
                    tier_count = 2;
                }
                else
                {
                    tier1 = components[component];
                }

                if (component == 0)
                {
                    try
                    {
                        unit_rates[area_code - 10, 0, component] = Convert.ToDecimal(tier1);
                        // For this entry, make Tier2 the same as Tier1
                        unit_rates[area_code - 10, 1, component] = unit_rates[area_code - 10, 0, component];
                    }
                    catch
                    {
                        // Something went wrong with the conversion
                        return false;
                    }
                }
                else
                {
                    if (!prices_include_vat ||
                        (component == components.Length - 1))  // Always ensure TCRs are unaffected by VAT reduction
                    {
                        try
                        {
                            unit_rates[area_code - 10, 0, component] = Convert.ToDecimal(tier1);
                            if (tier_index >= 0)
                            {
                                // Set a 'new' Tier2 value
                                unit_rates[area_code - 10, 1, component] = Convert.ToDecimal(tier2);
                            }
                        }
                        catch
                        {
                            // Conversion problem
                            return false;
                        }
                    }
                    else
                    {
                        try
                        {
                            unit_rates[area_code - 10, 0, component] = (Convert.ToDecimal(tier1) * 100.0M) / (100.0M + vat_rate);
                            if (tier_index >= 0)
                            {
                                // Set a 'new' Tier2 value
                                unit_rates[area_code - 10, 1, component] = (Convert.ToDecimal(tier2) * 100.0M) / (100.0M + vat_rate);
                            }
                        }
                        catch
                        {
                            // Conversion problem
                            return false;
                        }
                    }
                }
                component = component + 1;
            }
            return true;
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

        private static string EL_Button(HtmlAgilityPack.HtmlDocument document)
        {
            IList<HtmlAgilityPack.HtmlNode> HtmlCol1;

            HtmlCol1 = YetAnotherFuckingHoop.SelectNodesAsList(document.DocumentNode, "//button");
            foreach (HtmlAgilityPack.HtmlNode element1 in HtmlCol1)
            {
                string classname = element1.GetAttributeValue("class", string.Empty);
                if (SmartNibbyV2016.Check_Classname(classname, "continue-small down-arrow switch-more-info", false))
                {
                    return element1.GetAttributeValue("onclick", string.Empty);
                }
            }
            return string.Empty;
            // NO WONDER I NEVER GET ANYTHING DONE
        }


        private static bool EL_Decode(string username,
                                        string el_filepath,
                                        StreamWriter stream_writer,
                                        char resource_code,
                                        string brand_name,
                                        string payment_name,
                                        string payment_type,
                                        string alternate_name1,
                                        string alternate_name2, // Alternate3 only ever used by SP_Analyze
                                        DateTime prices_valid_from,
                                        HtmlAgilityPack.HtmlDocument html_documentx,
                                        HtmlAgilityPack.HtmlDocument ajax_documentx,
                                        DashBored Mally,
                                        bool TextBox_Active,
                                        RichTextBox textBoxConsole,
                                        DateTime time_now,
                                        short area_code,
                                        string resource_type,
                                        ref string lhs,
                                        ref string rhs,
                                        ref bool at_the_top,
                                        ref bool two_tier,
                                        ref bool payment_type_found,
                                        ref string derived_payment_plan,
                                        CheckedListBox Tariffs_Areas_Matrix,
                                        CheckedListBox Brand_Matrix)
        {
            IList<HtmlAgilityPack.HtmlNode> HtmlCol1,   // div moreinfo
                                            HtmlCol2,   // div row
                                            HtmlCol3;   // div row existing-single

            char delimiter = ',';

            bool rhs_done = false;

            string case3 = string.Empty,
                    payment_method = string.Empty,
                    complex = "Complex",
                    not_found = "Not found";
            string fixed_payment = " Fixed ",
                    exit_fees = "Exit fees";   // This one has a <span> in the heading
            bool status = false;
            string header = string.Empty,
                           trailer = string.Empty,
                           href = string.Empty;// For Complex - click for details this will return a '#'
            rhs = string.Empty;
            string rhs_limit = string.Empty;

            string local_resource_type = resource_type;
            bool tcr_found = false;

            HtmlCol1 = YetAnotherFuckingHoop.SelectNodesAsList(ajax_documentx.DocumentNode, "//div");
            foreach (HtmlAgilityPack.HtmlNode element1 in HtmlCol1)
            {
                string classname = element1.GetAttributeValue("class", string.Empty);
                if (SmartNibbyV2016.Check_Classname(classname, "moreinfo-container", true))
                {
                    status = true;  // We found the moreinfo entry - need to re-write this to check "Tariffs found"

                    string lhs_temp = string.Empty,
                           rhs_day_temp = string.Empty,
                           rhs_night_temp = string.Empty;   // All because of FUCKING Utilita ..

                    HtmlCol2 = YetAnotherFuckingHoop.SelectNodesAsList(element1, "div");
                    foreach (HtmlAgilityPack.HtmlNode element2 in HtmlCol2)
                    {
                        classname = element2.GetAttributeValue("class", string.Empty);
                        if (SmartNibbyV2016.Check_Classname(classname, "row", true) ||
                            SmartNibbyV2016.Check_Classname(classname, "row tariff-details-single", true)) // Fucking sweaty wankers
                        {
                            // Talking herself up as usual - what a bullshitter
                            int text_no = -1;


                            HtmlCol3 = YetAnotherFuckingHoop.SelectNodesAsList(element2, "div");
                            foreach (HtmlAgilityPack.HtmlNode element3 in HtmlCol3)
                            {
                                int count = -1;
                                classname = element3.GetAttributeValue("class", string.Empty);
                                if (SmartNibbyV2016.Check_Classname(classname, "column tariff-details-single", true) ||
                                    SmartNibbyV2016.Check_Classname(classname, "column", true)) // Fucking FUCKING sweaty cunts
                                {
                                    count = 0;
                                }
                                if (SmartNibbyV2016.Check_Classname(classname, "column new-elec-single new-plan-single", true)) // Go figure why this isn't column-gas-single etc. whatever
                                {
                                    count = 1;  // and then 2
                                }

                                switch (count)
                                {
                                    case 0:
                                        if (!string.IsNullOrEmpty(element3.InnerText))
                                        {
                                            string text0 = element3.InnerText.Replace("&nbsp;", string.Empty).Trim();
                                            if (text0.Contains(exit_fees))
                                            {
                                                text0 = exit_fees;
                                            }
                                            switch (text0)
                                            {
                                                case "Supplier":
                                                case "Supplier Name":
                                                    text_no = 0;
                                                    break;
                                                case "Tariff Name":
                                                    text_no = 1;
                                                    break;
                                                case "Tariff Type":
                                                    text_no = 2;
                                                    break;
                                                case "Payment Method":
                                                    text_no = 3;
                                                    case3 = text0;  // I HAVE FAITH, COURAGE and ENTHUSIASM I WILL WIN
                                                    break;
                                                case "Unit Rate 1":
                                                case "Tier 1 Unit Rate":
                                                    text_no = 4;
                                                    break;
                                                case "Unit Rate 1 Limit":
                                                case "Tier 1 Limit":
                                                    text_no = 5;
                                                    break;
                                                case "Unit Rate 2":
                                                case "Tier 2 Unit Rate":
                                                    text_no = 6;
                                                    break;
                                                case "Standing Charge":
                                                    text_no = 7;
                                                    break;
                                                case "Tariff ends on":
                                                    text_no = 8;
                                                    break;
                                                case "Price guaranteed until":
                                                    text_no = 9;
                                                    break;
                                                case "Exit fees":   // (if you cancel this tariff before the end date)
                                                    text_no = 10;
                                                    break;
                                                case "Discounts and additional charges":
                                                    text_no = 11;
                                                    break;
                                                case "Additional products or services included":
                                                    text_no = 12;
                                                    break;
                                                case "Tariff Comparison Rate (TCR)":
                                                    text_no = 13;
                                                    tcr_found = true;
                                                    break;
                                                default:
                                                    break;
                                            }
                                        }
                                        break;

                                    case 1:
                                        if (!string.IsNullOrEmpty(element3.InnerText))
                                        {
                                            string text1 = element3.InnerText.Replace("&nbsp;", string.Empty).Trim();
                                            switch (text_no)
                                            {
                                                case 0:
                                                    // Supplier Name
                                                    lhs = text1;
                                                    break;
                                                case 1:
                                                    // Tariff Name
                                                    lhs = lhs + delimiter + text1;
                                                    break;
                                                case 2:
                                                    // Tariff Type
                                                    lhs = lhs + delimiter + text1;
                                                    break;
                                                case 3:
                                                    // Payment Method
                                                    lhs = lhs + delimiter + text1;
                                                    break;
                                                case 4:
                                                    // Tier 1 Unit Rate
                                                    text1 = text1.Replace("pence per kWh", string.Empty).Trim();
                                                    lhs_temp = text1;
                                                    break;
                                                case 5:
                                                    // Tier 1 Limit
                                                    break;
                                                case 6:
                                                    // Tier 2 Unit Rate
                                                    text1 = text1.Replace("pence per kWh", string.Empty).Trim();
                                                    lhs_temp = text1;
                                                    break;
                                                case 7:
                                                    // Standing Charge
                                                    text1 = text1.Replace("pence per day", string.Empty).Trim();
                                                    if (string.IsNullOrEmpty(text1))
                                                    {
                                                        text1 = "0.0";  // Sometimes there's a bug and the Standing Charge line just isn't there ...
                                                    }
                                                    lhs = lhs + delimiter + area_code.ToString("00") + delimiter + text1 + delimiter + lhs_temp;
                                                    break;
                                                case 8:
                                                    break;
                                                case 9:
                                                    break;
                                                case 10:
                                                    break;
                                                case 11:
                                                    break;
                                                case 12:
                                                    break;
                                                case 13:
                                                    text1 = text1.Replace("pence per kWh", string.Empty).Trim();
                                                    if (string.IsNullOrEmpty(text1) ||
                                                        Not_Applicable(text1)) // Economy7s don't have TCRs cos Ofgem say they're not required
                                                    {
                                                        text1 = "0.0";
                                                    }
                                                    lhs = lhs + delimiter + "TCR:" + text1;
                                                    break;
                                                default:
                                                    break;
                                            }
                                        }

                                        if (!string.IsNullOrEmpty(element3.InnerText))
                                        {
                                            string text2 = element3.InnerText.Replace("&nbsp;", string.Empty).Trim();
                                            text2 = text2.Replace(SmartParametersV2016.carriageReturn.ToString(), string.Empty);
                                            switch (text_no)
                                            {
                                                case 0:
                                                    header = "Supplier Name: " + text2;
                                                    break;
                                                case 1:
                                                    // Tariff Name
                                                    header = header + Environment.NewLine.ToString() + "Tariff Name: " + text2;
                                                    break;
                                                case 2:
                                                    // Tariff Type
                                                    header = header + Environment.NewLine.ToString() + "Tariff Type: " + text2;
                                                    header = header + Environment.NewLine.ToString() + "Prices valid from: " + prices_valid_from.ToString();
                                                    header = header + Environment.NewLine.ToString() + "Prices include VAT";
                                                    break;
                                                case 3:
                                                    // Payment Method
                                                    // Payment header comes from 'outside'
                                                    payment_method = text2;
                                                    break;
                                                case 4:
                                                    // Tier 1 Unit Rate
                                                    text2 = text2.Replace("pence per kWh", string.Empty).Trim();
                                                    // Tough one this ...
                                                    if ((text2.Contains("Day")) &&
                                                        (text2.Contains("Night")))
                                                    {
                                                        text2 = text2.Replace(":\n", string.Empty);
                                                        string[] components = text2.Split(SmartParametersV2016.newline);
                                                        // Now ... we have always(?) got a Day Rate, but we may or may not have a Night Rate
                                                        int component = 0;
                                                        while (component < components.Length)
                                                        {
                                                            ////(local_resource_type);

                                                            switch (component)
                                                            {
                                                                case 0:
                                                                    rhs_day_temp = components[component].Trim();
                                                                    if (rhs_day_temp.Contains("Day"))   // "Day" may or may not be there ...!
                                                                    {
                                                                        rhs_day_temp = rhs_day_temp.Replace("Day", string.Empty).Trim();
                                                                    }
                                                                    rhs_day_temp = rhs_day_temp.Replace(":", string.Empty).Trim(); // Just in case
                                                                    rhs_night_temp = "0.0";
                                                                    break;
                                                                case 1:
                                                                    rhs_night_temp = components[component].Trim();
                                                                    if (rhs_night_temp.Contains("Night"))
                                                                    {
                                                                        rhs_night_temp = rhs_night_temp.Replace("Night", string.Empty).Trim();
                                                                        rhs_night_temp = rhs_night_temp.Replace(":", string.Empty).Trim(); // Just in case
                                                                    }
                                                                    break;
                                                                default:
                                                                    break;
                                                            }
                                                            component = component + 1;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        rhs_day_temp = text2.Replace(":", string.Empty).Trim(); // Just in case
                                                        rhs_night_temp = "0.0";
                                                    }
                                                    break;
                                                case 5:
                                                    // Tier 1 Limit
                                                    if (Not_Applicable(text2) ||
                                                        (text2.Contains(complex)))
                                                    {
                                                        rhs_limit = string.Empty;
                                                    }
                                                    else
                                                    {
                                                        rhs_limit = " (Limit: " + text2 + "kWh)";
                                                    }
                                                    break;
                                                case 6:
                                                    // Tier 2 Unit Rate
                                                    if (Not_Applicable(text2) ||
                                                        (text2.Contains(complex)))
                                                    {
                                                    }
                                                    else
                                                    {
                                                        text2 = text2.Replace("pence per kWh", string.Empty).Trim();
                                                        rhs_day_temp = rhs_day_temp + "/" + text2;
                                                    }
                                                    break;
                                                case 7:
                                                    // Standing Charge
                                                    // For EbiCo especially
                                                    if (Not_Applicable(text2) ||
                                                        string.IsNullOrEmpty(text2))    // Sometimes there's a bug and the Standing Charge line jusr isn't there...
                                                    {
                                                        text2 = "0.0";   // means this per day
                                                    }
                                                    text2 = text2.Replace("pence per day", string.Empty).Trim();
                                                    rhs = area_code.ToString("00") + delimiter + text2 + delimiter + rhs_day_temp + delimiter + rhs_night_temp;
                                                    href = EL_Complex(element3);
                                                    break;
                                                case 8:
                                                    if (text2.Contains(complex))
                                                    {
                                                        text2 = not_found;
                                                    }
                                                    trailer = "Tariff ends on: " + text2;
                                                    break;
                                                case 9:
                                                    if (text2.Contains(complex))
                                                    {
                                                        text2 = not_found;
                                                    }
                                                    trailer = trailer + Environment.NewLine.ToString() + "Price guaranteed until: " + text2;
                                                    break;
                                                case 10:
                                                    if (text2.Contains(complex))
                                                    {
                                                        text2 = not_found;
                                                    }
                                                    trailer = trailer + Environment.NewLine.ToString() + "Exit fees: " + text2.Replace("&pound;", "£");
                                                    break;
                                                case 11:
                                                    if (text2.Contains(complex))
                                                    {
                                                        text2 = not_found;
                                                    }
                                                    trailer = trailer + Environment.NewLine.ToString() + "Discounts and additional charges: " + text2.Replace("&pound;", "£");
                                                    break;
                                                case 12:
                                                    if (text2.Contains(complex))
                                                    {
                                                        text2 = not_found;
                                                    }
                                                    trailer = trailer + Environment.NewLine.ToString() + "Additional products or services included: " + text2.Replace("&pound;", "£");
                                                    break;
                                                case 13:
                                                    //if (local_resource_type == "SR")
                                                    //{
                                                    //    // If we're not Economy7, we need a 'separator'
                                                    //    // so we always return the AREA_CODE + SC + DR + NR + TCR
                                                    //    rhs = rhs + delimiter + "0.0";
                                                    //}
                                                    text2 = text2.Replace("pence per kWh", string.Empty).Trim();
                                                    if (Not_Applicable(text2) ||
                                                        string.IsNullOrEmpty(text2))    // Economy7s don't have TCRs cos Ofgem say they're not required
                                                    {
                                                        text2 = "0.0";
                                                    }
                                                    rhs = rhs + delimiter + text2;
                                                    //if (text2.Contains(complex))
                                                    //{
                                                    //    text2 = not_found;
                                                    //}
                                                    //trailer = trailer + Environment.NewLine.ToString() + "Tariff Comparison Rate (TCR): " + text2;
                                                    break;
                                                default:
                                                    break;
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

            if (!rhs_done)
            {
                if (at_the_top)
                {
                    // Display headers
                    //stream_writer.WriteLine(header);
                    //stream_writer.WriteLine(trailer);
                    at_the_top = false;
                }

                if (area_code == 10)
                {
                    // Display headers
                    //stream_writer.WriteLine();
                    //stream_writer.WriteLine("Payment Plan: " + payment_method);
                }
                // well there is a problem in Energylinx insofar as EVERY 'existing' or 'input' tariff
                // Payment Method is shown as "Monthly Fixed Direct Debit" no matter what
                // the required 'output' tariff or output Pament Method is.
                // e.g. EdF Blue + Price Promise March 2016 has the same figures for a
                // "Monthly Variable Direct Debit" as for a "Monthly Fixed Direct Debit"
                // (this is irrespective of the tariff itself being Fixed or Variable btw.)
                // ON the basis that Variable is much preferable to Fixed, we are going to default
                // "Monthly Fixed Direct Debit" (type "C") to "Monthly Variable Direct Debit" (type "A")
                // and assume that energylinx have now 'go it slightly wrong'

                // This is the kludge we might have to take out sometime
                if ((payment_type == "1") &&
                    (payment_method.Contains(fixed_payment)))
                {
                    payment_method = payment_method.Replace(fixed_payment, " ");
                }
                if ((payment_method != payment_name) &&
                    (payment_method != alternate_name1) &&
                    (payment_method != alternate_name2))
                {
                    // This is the GENUINE fix-up which works
                    // Try and fix up "A Monthly Direct Debit" to "C Monthly Fixed Direct Debit"
                    if ((payment_type == "1") &&
                        (payment_method.Contains(fixed_payment)))
                    {
                        derived_payment_plan = "C";
                    }
                    else
                    {
                        //MainProcess.Output_Message(textBoxConsole,
                        //                        "Mis-match: want " + payment_name +
                        //                        " or " + alternate_name1 +
                        //                        " or " + alternate_name2 +
                        //                        " got " + payment_method,
                        //                        Mally.examine.scrape, Mally.console);
                        goto skip;
                    }
                }

                if (!string.IsNullOrEmpty(href))
                {
                    // A Non-empty href means we have found 'Complex - click for details'
                    // and those details are stored as a modal dialogue in the ORIGINAL html document
                    // **Not** the one we used to get 'More info' (i.e. the one with 'More info' on it)
                    if (!EL_Decipher_Complex(html_documentx,
                                        Mally,
                                        TextBox_Active,
                                        textBoxConsole,
                                        resource_code,
                                        area_code,
                                        local_resource_type,
                                        ref rhs,
                                        ref two_tier,
                                        Tariffs_Areas_Matrix,
                                        Brand_Matrix))
                    {
                        MainProcess.Output_Message(textBoxConsole,
                                    "Decipher Complex failed: " + resource_code + " " +
                                    local_resource_type + " " +
                                    derived_payment_plan + " " +
                                    rhs,
                                    Mally.examine.scrape, Mally.console);
                    }
                    else
                    {
                        MainProcess.Output_Message(textBoxConsole,
                                            "Complex says :" + rhs,
                                            Mally.examine.scrape, Mally.console);

                    }
                }
                if (!tcr_found)
                {
                    rhs = rhs + ",0.00";
                }
                rhs = rhs.Replace(",", " ");

                // These prices INCLUDE VAT
                //stream_writer.WriteLine(lhs + "\t" + rhs.Replace(" ", "\t"));    // Ensure they come out tab-delimited
                rhs_done = true;

                // Set the Tariff Area Matrix
                if (area_code >= 10 && area_code <= 23)
                {
                    area_code = (short)(area_code - 10);
                    // Set the Tariff Area 10 to 'Y' 
                    Tariffs_Areas_Matrix.SetItemCheckState(area_code, CheckState.Checked);
                    Brand_Matrix.SetItemCheckState(area_code, CheckState.Checked);
                }

                if (!payment_type_found)
                {
                    payment_type_found = true;
                }

                stream_writer.WriteLine(lhs + "\t" +
                                    resource_code + "\t" +
                                    local_resource_type + "\t" +
                                    derived_payment_plan + "\t" +
                                    rhs.Replace(" ", "\t") + "\t" +
                                    rhs_limit);
                //MainProcess.Output_Message(textBoxConsole,
                //"LHS: " + lhs + " " +
                //"RHS: " + resource_code + " " +
                //                    local_resource_type + " " +
                //                    derived_payment_plan + " " +
                //                    rhs + rhs_limit,
                //                    Mally.examine.scrape, Mally.console);
            }
        skip:
            return status;
        }


        private static bool EL_Decipher(string username,
                                        string el_filepath,
                                        StreamWriter stream_writer,
                                        char resource_code,
                                        string brand_name,
                                        string payment_name,
                                        string payment_type,
                                        string alternate_name1,
                                        string alternate_name2, // Alternate3 only ever used by SP_Analyze
                                        DateTime prices_valid_from,
                                        HtmlAgilityPack.HtmlDocument html_documentx,
                                        HtmlAgilityPack.HtmlDocument ajax_documentx,
                                        DashBored Mally,
                                        bool TextBox_Active,
                                        RichTextBox textBoxConsole,
                                        DateTime time_now,
                                        short area_code,
                                        string resource_type,
                                        ref string rhs,
                                        ref bool at_the_top,
                                        ref bool two_tier,
                                        ref bool payment_type_found,
                                        ref string derived_payment_plan,
                                        CheckedListBox Tariffs_Areas_Matrix,
                                        CheckedListBox Brand_Matrix)
        {
            IList<HtmlAgilityPack.HtmlNode> HtmlCol1,   // div moreinfo
                                           HtmlCol2,   // div row
                                           HtmlCol3;   // div row existing-single

            char delimiter = ',';

            bool rhs_done = false;

            string case3 = string.Empty,
                    payment_method = string.Empty,
                    complex = "Complex",
                    not_found = "Not found";
            string fixed_payment = " Fixed ",
                    exit_fees = "Exit fees";   // This one has a <span> in the heading
            bool status = false;
            string header = string.Empty,
                           lhs = string.Empty,
                           trailer = string.Empty,
                           href = string.Empty;// For Complex - click for details this will return a '#'
            rhs = string.Empty;
            string rhs_limit = string.Empty;

            string local_resource_type = resource_type;
            bool tcr_found = false;

            HtmlCol1 = YetAnotherFuckingHoop.SelectNodesAsList(ajax_documentx.DocumentNode, "//div");
            foreach (HtmlAgilityPack.HtmlNode element1 in HtmlCol1)
            {
                string classname = element1.GetAttributeValue("class", string.Empty);
                if (SmartNibbyV2016.Check_Classname(classname, "moreinfo-container", true))
                {
                    status = true;  // We found the moreinfo entry - need to re-write this to check "Tariffs found"

                    string lhs_temp = string.Empty,
                           rhs_day_temp = string.Empty,
                           rhs_night_temp = string.Empty;   // All because of FUCKING Utilita ..

                    HtmlCol2 = YetAnotherFuckingHoop.SelectNodesAsList(element1, "div");
                    foreach (HtmlAgilityPack.HtmlNode element2 in HtmlCol2)
                    {
                        classname = element2.GetAttributeValue("class", string.Empty);
                        if (SmartNibbyV2016.Check_Classname(classname, "row", true) ||
                            SmartNibbyV2016.Check_Classname(classname, "row tariff-details-single", true)) // Fucking sweaty wankers
                        {
                            // Talking herself up as usual - what a bullshitter
                            int text_no = -1;


                            HtmlCol3 = YetAnotherFuckingHoop.SelectNodesAsList(element2, "div");
                            foreach (HtmlAgilityPack.HtmlNode element3 in HtmlCol3)
                            {
                                int count = -1;
                                classname = element3.GetAttributeValue("class", string.Empty);
                                if (SmartNibbyV2016.Check_Classname(classname, "column tariff-details-single", true) ||
                                    SmartNibbyV2016.Check_Classname(classname, "column", true)) // Fucking FUCKING sweaty cunts
                                {
                                    count = 0;
                                }
                                //if (SmartNibbyV2016.Check_Classname(classname, "column new-elec-single new-plan-single", true)) // Go figure why this isn't column-gas-single etc. whatever
                                //{
                                //    count = 1;  // and then 2
                                //}
                                if (SmartNibbyV2016.Check_Classname(classname, "highlight-blue", true)) // Go figure why this isn't column-gas-single etc. whatever
                                {
                                    count = 1;  // and then 2
                                }

                                switch (count)
                                {
                                    case 0:
                                        if (!string.IsNullOrEmpty(element3.InnerText))
                                        {
                                            string text0 = element3.InnerText.Replace("&nbsp;", string.Empty).Trim();
                                            if (text0.Contains(exit_fees))
                                            {
                                                text0 = exit_fees;
                                            }
                                            switch (text0)
                                            {
                                                case "Supplier":
                                                case "Supplier Name":
                                                    text_no = 0;
                                                    break;
                                                case "Tariff Name":
                                                    text_no = 1;
                                                    break;
                                                case "Tariff Type":
                                                    text_no = 2;
                                                    break;
                                                case "Payment Method":
                                                    text_no = 3;
                                                    case3 = text0;  // I HAVE FAITH, COURAGE and ENTHUSIASM I WILL WIN
                                                    break;
                                                case "Unit Rate 1":
                                                case "Tier 1 Unit Rate":
                                                    text_no = 4;
                                                    break;
                                                case "Unit Rate 1 Limit":
                                                case "Tier 1 Limit":
                                                    text_no = 5;
                                                    break;
                                                case "Unit Rate 2":
                                                case "Tier 2 Unit Rate":
                                                    text_no = 6;
                                                    break;
                                                case "Standing Charge":
                                                    text_no = 7;
                                                    break;
                                                case "Tariff ends on":
                                                    text_no = 8;
                                                    break;
                                                case "Price guaranteed until":
                                                    text_no = 9;
                                                    break;
                                                case "Exit fees":   // (if you cancel this tariff before the end date)
                                                    text_no = 10;
                                                    break;
                                                case "Discounts and additional charges":
                                                    text_no = 11;
                                                    break;
                                                case "Additional products or services included":
                                                    text_no = 12;
                                                    break;
                                                case "Tariff Comparison Rate (TCR)":
                                                    text_no = 13;
                                                    tcr_found = true;
                                                    break;
                                                default:
                                                    break;
                                            }
                                        }
                                        break;

                                    case 1:
                                        if (!string.IsNullOrEmpty(element3.InnerText))
                                        {
                                            string text1 = element3.InnerText.Replace("&nbsp;", string.Empty).Trim();
                                            switch (text_no)
                                            {
                                                case 0:
                                                    // Supplier Name
                                                    lhs = text1;
                                                    break;
                                                case 1:
                                                    // Tariff Name
                                                    lhs = lhs + delimiter + text1;
                                                    break;
                                                case 2:
                                                    // Tariff Type
                                                    lhs = lhs + delimiter + text1;
                                                    break;
                                                case 3:
                                                    // Payment Method
                                                    lhs = lhs + delimiter + text1;
                                                    break;
                                                case 4:
                                                    // Tier 1 Unit Rate
                                                    text1 = text1.Replace("pence per kWh", string.Empty).Trim();
                                                    lhs_temp = text1;
                                                    break;
                                                case 5:
                                                    // Tier 1 Limit
                                                    break;
                                                case 6:
                                                    // Tier 2 Unit Rate
                                                    text1 = text1.Replace("pence per kWh", string.Empty).Trim();
                                                    lhs_temp = text1;
                                                    break;
                                                case 7:
                                                    // Standing Charge
                                                    text1 = text1.Replace("pence per day", string.Empty).Trim();
                                                    if (string.IsNullOrEmpty(text1))
                                                    {
                                                        text1 = "0.0";  // Sometimes there's a bug and the Standing Charge line just isn't there ...
                                                    }
                                                    lhs = lhs + delimiter + area_code.ToString("00") + delimiter + text1 + delimiter + lhs_temp;
                                                    break;
                                                case 8:
                                                    break;
                                                case 9:
                                                    break;
                                                case 10:
                                                    break;
                                                case 11:
                                                    break;
                                                case 12:
                                                    break;
                                                case 13:
                                                    text1 = text1.Replace("pence per kWh", string.Empty).Trim();
                                                    if (string.IsNullOrEmpty(text1) ||
                                                        Not_Applicable(text1)) // Economy7s don't have TCRs cos Ofgem say they're not required
                                                    {
                                                        text1 = "0.0";
                                                    }
                                                    lhs = lhs + delimiter + "TCR:" + text1;
                                                    break;
                                                default:
                                                    break;
                                            }
                                        }

                                        if (!string.IsNullOrEmpty(element3.InnerText))
                                        {
                                            string text2 = element3.InnerText.Replace("&nbsp;", string.Empty).Trim();
                                            text2 = text2.Replace(SmartParametersV2016.carriageReturn.ToString(), string.Empty);
                                            switch (text_no)
                                            {
                                                case 0:
                                                    header = "Supplier Name: " + text2;
                                                    break;
                                                case 1:
                                                    // Tariff Name
                                                    header = header + Environment.NewLine.ToString() + "Tariff Name: " + text2;
                                                    break;
                                                case 2:
                                                    // Tariff Type
                                                    header = header + Environment.NewLine.ToString() + "Tariff Type: " + text2;
                                                    header = header + Environment.NewLine.ToString() + "Prices valid from: " + prices_valid_from.ToString();
                                                    header = header + Environment.NewLine.ToString() + "Prices include VAT";
                                                    break;
                                                case 3:
                                                    // Payment Method
                                                    // Payment header comes from 'outside'
                                                    payment_method = text2;
                                                    break;
                                                case 4:
                                                    // Tier 1 Unit Rate
                                                    text2 = text2.Replace("pence per kWh", string.Empty).Trim();
                                                    // Tough one this ...
                                                    if ((text2.Contains("Day")) &&
                                                        (text2.Contains("Night")))
                                                    {
                                                        text2 = text2.Replace(":\n", string.Empty);
                                                        string[] components = text2.Split(SmartParametersV2016.newline);
                                                        // Now ... we have always(?) got a Day Rate, but we may or may not have a Night Rate
                                                        int component = 0;
                                                        while (component < components.Length)
                                                        {
                                                            ////(local_resource_type);

                                                            switch (component)
                                                            {
                                                                case 0:
                                                                    rhs_day_temp = components[component].Trim();
                                                                    if (rhs_day_temp.Contains("Day"))   // "Day" may or may not be there ...!
                                                                    {
                                                                        rhs_day_temp = rhs_day_temp.Replace("Day", string.Empty).Trim();
                                                                    }
                                                                    rhs_day_temp = rhs_day_temp.Replace(":", string.Empty).Trim(); // Just in case
                                                                    rhs_night_temp = "0.0";
                                                                    break;
                                                                case 1:
                                                                    rhs_night_temp = components[component].Trim();
                                                                    if (rhs_night_temp.Contains("Night"))
                                                                    {
                                                                        rhs_night_temp = rhs_night_temp.Replace("Night", string.Empty).Trim();
                                                                        rhs_night_temp = rhs_night_temp.Replace(":", string.Empty).Trim(); // Just in case
                                                                        if (Not_Applicable(rhs_night_temp))
                                                                        {
                                                                            rhs_night_temp = "0.0";
                                                                        }
                                                                    }
                                                                    break;
                                                                default:
                                                                    break;
                                                            }
                                                            component = component + 1;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        rhs_day_temp = text2.Replace(":", string.Empty).Trim(); // Just in case
                                                        rhs_night_temp = "0.0";
                                                    }
                                                    break;
                                                case 5:
                                                    // Tier 1 Limit
                                                    if (Not_Applicable(text2) ||
                                                        (text2.Contains(complex)))
                                                    {
                                                        rhs_limit = string.Empty;
                                                    }
                                                    else
                                                    {
                                                        rhs_limit = " (Limit: " + text2 + "kWh)";
                                                    }
                                                    break;
                                                case 6:
                                                    // Tier 2 Unit Rate
                                                    if (Not_Applicable(text2) ||
                                                        (text2.Contains(complex)))
                                                    {
                                                    }
                                                    else
                                                    {
                                                        text2 = text2.Replace("pence per kWh", string.Empty).Trim();
                                                        rhs_day_temp = rhs_day_temp + "/" + text2;
                                                    }
                                                    break;
                                                case 7:
                                                    // Standing Charge
                                                    // For EbiCo especially
                                                    if (Not_Applicable(text2) ||
                                                        string.IsNullOrEmpty(text2))    // Sometimes there's a bug and the Standing Charge line jusr isn't there...
                                                    {
                                                        text2 = "0.0";   // means this per day
                                                    }
                                                    text2 = text2.Replace("pence per day", string.Empty).Trim();
                                                    rhs = area_code.ToString("00") + delimiter + text2 + delimiter + rhs_day_temp + delimiter + rhs_night_temp;
                                                    href = EL_Complex(element3);
                                                    break;
                                                case 8:
                                                    if (text2.Contains(complex))
                                                    {
                                                        text2 = not_found;
                                                    }
                                                    trailer = "Tariff ends on: " + text2;
                                                    break;
                                                case 9:
                                                    if (text2.IndexOf(complex) >= 0)
                                                    {
                                                        text2 = not_found;
                                                    }
                                                    trailer = trailer + Environment.NewLine.ToString() + "Price guaranteed until: " + text2;
                                                    break;
                                                case 10:
                                                    if (text2.IndexOf(complex) >= 0)
                                                    {
                                                        text2 = not_found;
                                                    }
                                                    trailer = trailer + Environment.NewLine.ToString() + "Exit fees: " + text2.Replace("&pound;", "£");
                                                    break;
                                                case 11:
                                                    if (text2.IndexOf(complex) >= 0)
                                                    {
                                                        text2 = not_found;
                                                    }
                                                    trailer = trailer + Environment.NewLine.ToString() + "Discounts and additional charges: " + text2.Replace("&pound;", "£");
                                                    break;
                                                case 12:
                                                    if (text2.IndexOf(complex) >= 0)
                                                    {
                                                        text2 = not_found;
                                                    }
                                                    trailer = trailer + Environment.NewLine.ToString() + "Additional products or services included: " + text2.Replace("&pound;", "£");
                                                    break;
                                                case 13:
                                                    //if (local_resource_type == "SR")
                                                    //{
                                                    //    // If we're not Economy7, we need a 'separator'
                                                    //    // so we always return the AREA_CODE + SC + DR + NR + TCR
                                                    //    rhs = rhs + delimiter + "0.0";
                                                    //}
                                                    text2 = text2.Replace("pence per kWh", string.Empty).Trim();
                                                    if (Not_Applicable(text2) ||
                                                        string.IsNullOrEmpty(text2))    // Economy7s don't have TCRs cos Ofgem say they're not required
                                                    {
                                                        text2 = "0.0";
                                                    }
                                                    rhs = rhs + delimiter + text2;
                                                    //if (text2.IndexOf(complex) >= 0)
                                                    //{
                                                    //    text2 = not_found;
                                                    //}
                                                    //trailer = trailer + Environment.NewLine.ToString() + "Tariff Comparison Rate (TCR): " + text2;
                                                    break;
                                                default:
                                                    break;
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

            if (!rhs_done)
            {
                if (at_the_top)
                {
                    // Display headers
                    stream_writer.WriteLine(header);
                    stream_writer.WriteLine(trailer);
                    at_the_top = false;
                }

                if (area_code == 10)
                {
                    // Display headers
                    stream_writer.WriteLine();
                    stream_writer.WriteLine("Payment Plan: " + payment_method);
                }
                // well there is a problem in Energylinx insofar as EVERY 'existing' or 'input' tariff
                // Payment Method is shown as "Monthly Fixed Direct Debit" no matter what
                // the required 'output' tariff or output Pament Method is.
                // e.g. EdF Blue + Price Promise March 2016 has the same figures for a
                // "Monthly Variable Direct Debit" as for a "Monthly Fixed Direct Debit"
                // (this is irrespective of the tariff itself being Fixed or Variable btw.)
                // ON the basis that Variable is much preferable to Fixed, we are going to default
                // "Monthly Fixed Direct Debit" (type "C") to "Monthly Variable Direct Debit" (type "A")
                // and assume that energylinx have now 'go it slightly wrong'

                // This is the kludge we might have to take out sometime
                if ((payment_type == "1") &&
                    (payment_method.IndexOf(fixed_payment) >= 0))
                {
                    payment_method = payment_method.Replace(fixed_payment, " ");
                }
                if ((payment_method != payment_name) &&
                    (payment_method != alternate_name1) &&
                    (payment_method != alternate_name2))
                {
                    // This is the GENUINE fix-up which works
                    // Try and fix up "A Monthly Direct Debit" to "C Monthly Fixed Direct Debit"
                    if ((payment_type == "1") &&
                        (payment_method.IndexOf(fixed_payment) >= 0))
                    {
                        derived_payment_plan = "C";
                    }
                    else
                    {
                        MainProcess.Output_Message(textBoxConsole,
                                                "Mis-match: want " + payment_name +
                                                " or " + alternate_name1 +
                                                " or " + alternate_name2 +
                                                " got " + payment_method,
                                                Mally.examine.scrape, Mally.console);
                        goto skip;
                    }
                }

                if (!string.IsNullOrEmpty(href))
                {
                    // A Non-empty href means we have found 'Complex - click for details'
                    // and those details are stored as a modal dialogue in the ORIGINAL html document
                    // **Not** the one we used to get 'More info' (i.e. the one with 'More info' on it)
                    if (!EL_Decipher_Complex(html_documentx,
                                        Mally,
                                        TextBox_Active,
                                        textBoxConsole,
                                        resource_code,
                                        area_code,
                                        local_resource_type,
                                        ref rhs,
                                        ref two_tier,
                                        Tariffs_Areas_Matrix,
                                        Brand_Matrix))
                    {
                        MainProcess.Output_Message(textBoxConsole,
                                    "Decipher Complex failed: " + resource_code + " " +
                                    local_resource_type + " " +
                                    derived_payment_plan + " " +
                                    rhs,
                                    Mally.examine.scrape, Mally.console);
                    }
                    else
                    {
                        MainProcess.Output_Message(textBoxConsole,
                                            "Complex says :" + rhs,
                                            Mally.examine.scrape, Mally.console);

                    }
                }
                if (!tcr_found)
                {
                    rhs = rhs + ",0.00";
                }
                rhs = rhs.Replace(",", " ");

                // These prices INCLUDE VAT
                stream_writer.WriteLine(rhs.Replace(" ", "\t"));    // Ensure they come out tab-delimited
                rhs_done = true;

                // Set the Tariff Area Matrix
                if (area_code >= 10 && area_code <= 23)
                {
                    area_code = (short)(area_code - 10);
                    // Set the Tariff Area 10 to 'Y' 
                    Tariffs_Areas_Matrix.SetItemCheckState(area_code, CheckState.Checked);
                    Brand_Matrix.SetItemCheckState(area_code, CheckState.Checked);
                }

                if (!payment_type_found)
                {
                    payment_type_found = true;
                }
                //MainProcess.Output_Message(textBoxConsole,
                //                    "LHS: " + lhs,
                //                    Mally.examine.scrape, Mally.console);
                MainProcess.Output_Message(textBoxConsole,
                "RHS: " + resource_code + " " +
                                    local_resource_type + " " +
                                    derived_payment_plan + " " +
                                    rhs + rhs_limit,
                                    Mally.examine.scrape, Mally.console);
            }
        skip:
            return status;
        }

        private static bool Not_Applicable(string text2)
        {
            if (!string.IsNullOrEmpty(text2))
            {
                if ((text2 == "N/A") ||
                    (text2 == "Not applicable"))
                {
                    return true;
                }
            }
            return false;
        }

        private static string EL_Complex(HtmlAgilityPack.HtmlNode element)
        {
            IList<HtmlAgilityPack.HtmlNode> HtmlCol1;

            HtmlCol1 = YetAnotherFuckingHoop.SelectNodesAsList(element, "//a");
            foreach (HtmlAgilityPack.HtmlNode element1 in HtmlCol1)
            {
                if (!string.IsNullOrEmpty(element1.InnerText))
                {
                    if (element1.InnerText.IndexOf("Complex") >= 0)
                    {
                        return element1.GetAttributeValue("href", string.Empty);
                    }
                }
            }
            return string.Empty;
        }
        private static bool EL_Decipher_Complex(HtmlAgilityPack.HtmlDocument document,
                                        DashBored Mally,
                                        bool TextBox_Active,
                                        RichTextBox textBoxConsole,
                                        char resource_code,
                                        short area_code,
                                        string resource_type,
                                        ref string rhs,
                                        ref bool two_tier,
                                        CheckedListBox Tariffs_Areas_Matrix,
                                        CheckedListBox Brand_Matrix)
        {

            IList<HtmlAgilityPack.HtmlNode> HtmlCol7,
                                           HtmlCol8,
                                           HtmlCol9,
                                           HtmlCol10,
                                           HtmlCol11;

            char delimiter = ',';

            string classname = string.Empty;

            string standing_charge = string.Empty,
                day_rate1 = string.Empty,
                day_rate2 = string.Empty,
                night_rate = string.Empty;

            // Assume its a Standing Charge tariff
            two_tier = false;
            bool clicked = false;

            // Look for the class="modal-body"
            HtmlCol7 = YetAnotherFuckingHoop.SelectNodesAsList(document.DocumentNode, "//div");
            foreach (HtmlAgilityPack.HtmlNode element7 in HtmlCol7)
            {
                classname = element7.GetAttributeValue("class", string.Empty);

                if (SmartNibbyV2016.Check_Classname(classname, "modal-body", true))
                {
                    HtmlCol8 = YetAnotherFuckingHoop.SelectNodesAsList(element7, "table");
                    foreach (HtmlAgilityPack.HtmlNode element8 in HtmlCol8)
                    {
                        classname = element8.GetAttributeValue("class", string.Empty);
                        if (SmartNibbyV2016.Check_Classname(classname, "table", true))
                        {
                            HtmlCol9 = YetAnotherFuckingHoop.SelectNodesAsList(element8, "tbody");
                            foreach (HtmlAgilityPack.HtmlNode element9 in HtmlCol9)
                            {
                                int lines_count = 0;
                                HtmlCol10 = YetAnotherFuckingHoop.SelectNodesAsList(element9, "tr");
                                foreach (HtmlAgilityPack.HtmlNode element10 in HtmlCol10)
                                {
                                    switch (lines_count)
                                    {
                                        case 0:
                                            HtmlCol11 = YetAnotherFuckingHoop.SelectNodesAsList(element10, "td");
                                            foreach (HtmlAgilityPack.HtmlNode element11 in HtmlCol11)
                                            {
                                                if (!string.IsNullOrEmpty(element11.InnerText))
                                                {
                                                    int at_index = element11.InnerText.IndexOf("@");
                                                    if (at_index >= 0)
                                                    {
                                                        standing_charge = element11.InnerText.Substring(at_index + 1, element11.InnerText.Length - at_index - 2);
                                                        standing_charge = standing_charge.Replace("\t", string.Empty);
                                                        standing_charge = standing_charge.Replace("\n", string.Empty);
                                                        standing_charge = standing_charge.Replace("=", string.Empty);
                                                        standing_charge = standing_charge.Replace("pence", string.Empty).Trim();
                                                        if (string.IsNullOrEmpty(standing_charge))
                                                        {
                                                            // Usually means that the Unit Rates are TWO-TIER
                                                        }
                                                        break;
                                                    }
                                                }
                                            }
                                            break;
                                        case 1:
                                            break;
                                        case 2:         // "Unit rates" and "Unit rates - day"
                                            HtmlCol11 = YetAnotherFuckingHoop.SelectNodesAsList(element10, "td");
                                            foreach (HtmlAgilityPack.HtmlNode element11 in HtmlCol11)
                                            {
                                                if (!string.IsNullOrEmpty(element11.InnerText))
                                                {
                                                    int unit_rates_index = element11.InnerText.IndexOf("Unit rates");
                                                    if (unit_rates_index >= 0)
                                                    {
                                                        break;
                                                    }
                                                }
                                            }
                                            break;
                                        case 3:
                                            HtmlCol11 = YetAnotherFuckingHoop.SelectNodesAsList(element10, "td");
                                            foreach (HtmlAgilityPack.HtmlNode element11 in HtmlCol11)
                                            {
                                                if (!string.IsNullOrEmpty(element11.InnerText))
                                                {
                                                    int at_index = element11.InnerText.IndexOf("@");
                                                    if (at_index >= 0)
                                                    {
                                                        day_rate1 = element11.InnerText.Substring(at_index + 1, element11.InnerText.Length - at_index - 2);
                                                        day_rate1 = day_rate1.Replace("\t", string.Empty);
                                                        day_rate1 = day_rate1.Replace("\n", string.Empty);
                                                        day_rate1 = day_rate1.Replace("=", string.Empty);
                                                        day_rate1 = day_rate1.Replace("pence", string.Empty).Trim();
                                                        if (!string.IsNullOrEmpty(standing_charge))
                                                        {
                                                            rhs = area_code.ToString("00") + delimiter + standing_charge + delimiter + day_rate1;
                                                            if (resource_type == "SR")
                                                            {
                                                                // Mock up the Night Rate
                                                                rhs = rhs + delimiter + "0.00";
                                                                //// Mock up the TCR which isn't in Complex Details
                                                                //rhs = rhs + delimiter + "0.00";
                                                                // |Give up now if we don't have to find a night rate
                                                                clicked = true;
                                                                break;
                                                            }
                                                        }
                                                        else
                                                        {
                                                            if (!string.IsNullOrEmpty(day_rate1))
                                                            {
                                                                rhs = area_code.ToString("00") + delimiter + day_rate1;
                                                                // We have to find either the NIght Rate OR Day Rate 2
                                                                MainProcess.Output_Message(textBoxConsole,
                                                                                    "RHS: " + "may be Two-Tier",
                                                                                    Mally.examine.scrape, Mally.console);
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                            break;
                                        case 4:
                                            HtmlCol11 = YetAnotherFuckingHoop.SelectNodesAsList(element10, "td");
                                            foreach (HtmlAgilityPack.HtmlNode element11 in HtmlCol11)
                                            {
                                                if (!string.IsNullOrEmpty(element11.InnerText))
                                                {
                                                    int at_index = element11.InnerText.IndexOf("@");
                                                    if (at_index >= 0)
                                                    {
                                                        day_rate2 = element11.InnerText.Substring(at_index + 1, element11.InnerText.Length - at_index - 2);
                                                        day_rate2 = day_rate2.Replace("\t", string.Empty);
                                                        day_rate2 = day_rate2.Replace("\n", string.Empty);
                                                        day_rate2 = day_rate2.Replace("=", string.Empty);
                                                        day_rate2 = day_rate2.Replace("pence", string.Empty).Trim();
                                                        if (string.IsNullOrEmpty(standing_charge) &&
                                                            !string.IsNullOrEmpty(day_rate1))
                                                        {
                                                            
                                                            rhs = rhs + delimiter + day_rate2;
                                                            // |Give up now if we have found the night rate or day rate 2
                                                            //// Mock up the TCR which doesn't exist in COmpex Details
                                                            //rhs = rhs + delimiter + "0.00";
                                                            two_tier = true;
                                                            clicked = true;
                                                            MainProcess.Output_Message(textBoxConsole,
                                                                                "RHS: " + "may be returning the STANDARD rate for Rate2",
                                                                                Mally.examine.scrape, Mally.console);
                                                            break;
                                                        }
                                                    }
                                                }
                                            }
                                            break;
                                        case 5: // "Unit rates - night" OR day_rate2
                                            HtmlCol11 = YetAnotherFuckingHoop.SelectNodesAsList(element10, "td");
                                            foreach (HtmlAgilityPack.HtmlNode element11 in HtmlCol11)
                                            {
                                                if (!string.IsNullOrEmpty(element11.InnerText))
                                                {
                                                    int unit_rates_index = element11.InnerText.IndexOf("Unit rates");
                                                    if (unit_rates_index >= 0)
                                                    {
                                                        break;
                                                    }
                                                }
                                            }
                                            break;
                                        case 6:
                                            // Night Rate (if economy7?)
                                            HtmlCol11 = YetAnotherFuckingHoop.SelectNodesAsList(element10, "td");
                                            foreach (HtmlAgilityPack.HtmlNode element11 in HtmlCol11)
                                            {
                                                if (!string.IsNullOrEmpty(element11.InnerText))
                                                {
                                                    int at_index = element11.InnerText.IndexOf("@");
                                                    if (at_index >= 0)
                                                    {
                                                        night_rate = element11.InnerText.Substring(at_index + 1, element11.InnerText.Length - at_index - 2);
                                                        night_rate = night_rate.Replace("\t", string.Empty);
                                                        night_rate = night_rate.Replace("\n", string.Empty);
                                                        night_rate = night_rate.Replace("=", string.Empty);
                                                        night_rate = night_rate.Replace("pence", string.Empty).Trim();
                                                        if (!string.IsNullOrEmpty(standing_charge) &&
                                                            !string.IsNullOrEmpty(day_rate1))
                                                        {
                                                            
                                                            rhs = rhs + delimiter + night_rate;
                                                            if (resource_type == "VR")
                                                            {
                                                                //// Mock up the TCR which doesn't exist in the Complex Details
                                                                //rhs = rhs + delimiter + "0.00";
                                                                // |Give up now if we have found the night rate or day rate 2
                                                                clicked = true;
                                                                break;
                                                            }
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
                                        break;
                                    }
                                    lines_count = lines_count + 1;
                                }
                                if (clicked)
                                {
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
                if (clicked)
                {
                    break;
                }
            }
            return true;
        }

        private static bool Energy_Updates(DashboardModel dashboardmodel,
                                    string username,
                                    HtmlAgilityPack.HtmlDocument document,
                                    bool TextBox_Active,
                                    DashBored Mally,
                                    RichTextBox textBoxConsole,
                                    char resource_code,
                                    int defaultindex,
                                    DateTime defaultDate,
                                    CheckBox EnergyUpdates)
        {
            string routine = MethodBase.GetCurrentMethod().Name.ToUpper();

            IList<HtmlAgilityPack.HtmlNode> HtmlCol1,   // table
                                            HtmlCol2,   // tr
                                            HtmlCol3;   // td

            // Our table contains COLOURS column but we never find that?
            DataTable our_updates_table = dashboardmodel.energyupdates_table;

            string[,] cols = new string[our_updates_table.Columns.Count, 2];

            int col_count = 0;
            foreach (DataColumn column in our_updates_table.Columns)
            {
                cols[col_count, 0] = column.ColumnName;
                cols[col_count, 1] = column.DataType.ToString();
                col_count = col_count + 1;
            }


            int line_count = 0;
            int insert_position = 0;            // The row position into which we insert what isn't there already
                                                // (Working down from the top)
            bool all_done = false;

            HtmlCol1 = YetAnotherFuckingHoop.SelectNodesAsList(document.DocumentNode, "//table");
            foreach (HtmlAgilityPack.HtmlNode element1 in HtmlCol1)
            {
                if (element1.Id == "pricestable")
                {
                    HtmlCol2 = YetAnotherFuckingHoop.SelectNodesAsList(element1, "tr");
                    foreach (HtmlAgilityPack.HtmlNode element2 in HtmlCol2)
                    {
                        int column_count = 0;
                        DataRow possible_insert_row = null;

                        string[] fields = new string[10];

                        HtmlCol3 = YetAnotherFuckingHoop.SelectNodesAsList(element2, "td");
                        foreach (HtmlAgilityPack.HtmlNode element3 in HtmlCol3)
                        {
                            string item = element3.InnerText.Replace(SmartParametersV2016.tab.ToString(), string.Empty).Trim();
                            // Sort out & (e.g. Fix & Save and M&S
                            item = item.Replace("&amp;", SmartParametersV2016.ampersand);
                            string classname = element3.GetAttributeValue("class", string.Empty);
                            if (SmartNibbyV2016.Check_Classname(classname, "tablecontentleft", true))
                            {
                                if ((item.ToUpper() == "SUPPLIER") &&
                                    (line_count == 0))
                                {
                                    goto skip_this; // <= Its the header
                                }
                                if (item.ToUpper().Replace(SmartParametersV2016.space, SmartParametersV2016.underscore) != cols[column_count, 0])
                                {
                                    if (string.IsNullOrEmpty(item.Trim()))
                                    {
                                        MainProcess.Output_Message(textBoxConsole,
                                                            "Item string is empty - giving up?",
                                                            Mally.examine.scrape, Mally.console);
                                        all_done = true;
                                        break;
                                    }
                                    possible_insert_row = our_updates_table.NewRow();
                                    line_count = line_count + 1;
                                    possible_insert_row[cols[column_count, 0]] = item;
                                }
                                
                            }
                            if (SmartNibbyV2016.Check_Classname(classname, "tablecontent", true))
                            {
                                if (item.ToUpper().Replace(SmartParametersV2016.space, SmartParametersV2016.underscore) != cols[column_count, 0])
                                {
                                    if (cols[column_count, 1] == "System.DateTime")
                                    {
                                        // Take energyupdates table row 0 as the last date
                                        DateTime row_update_date = defaultDate;
                                        if (!string.IsNullOrEmpty(item.Trim()))
                                        {
                                            item = item.Replace(SmartParametersV2016.period, SmartParametersV2016.dash);
                                            try
                                            {
                                                row_update_date = Convert.ToDateTime(item);
                                            }
                                            catch
                                            {
                                                MainProcess.Output_Message(textBoxConsole,
                                                                "Cannot convert " + item + " to date at line " + line_count,
                                                                Mally.examine.scrape, Mally.console);
                                            }
                                        }
                                        if (row_update_date == defaultDate)
                                        {
                                            if (column_count == 1)
                                            {
                                                goto skip_this;
                                            }
                                            if (column_count == 2)
                                            {
                                                possible_insert_row[cols[column_count, 0]] = possible_insert_row[cols[column_count - 1, 0]];
                                            }
                                        }
                                        else
                                        {
                                            possible_insert_row[cols[column_count, 0]] = row_update_date;
                                        }
                                    }
                                    else
                                    {
                                        possible_insert_row[cols[column_count, 0]] = item;
                                    }
                                }
                            }
                            if (SmartNibbyV2016.Check_Classname(classname, "tablecontent tablecontentright", true))
                            {
                                if (item.ToUpper().Replace(SmartParametersV2016.space, SmartParametersV2016.underscore) != cols[column_count, 0])
                                {
                                    possible_insert_row[cols[column_count, 0]] = item;
                                }
                            }
                            column_count = column_count + 1;
                        }
                        // Check to see if we add it in
                        bool header = false;
                        if (possible_insert_row != null)
                        {
                            if ((possible_insert_row[0].ToString() == "Supplier") &&
                                    //(possible_insert_row[1].ToString() == SmartParametersV2016.defaultDates) &&
                                    //(possible_insert_row[2].ToString() == SmartParametersV2016.defaultDates) &&
                                    (possible_insert_row[3].ToString() == "Fuel Type") &&
                                    (possible_insert_row[4].ToString() == "Details"))
                            {
                                header = true;
                            }
                        }

                        if (possible_insert_row != null &&
                            !header)
                        {
                            // Default the colours
                            possible_insert_row["COMMENTS"] = string.Empty;  // So its not 'null' (this is ours and not scraped from Energylinx)
                            possible_insert_row["COLOURS"] = defaultindex.ToString() + "|" + defaultindex.ToString();

                            // Check the Supplier
                            string expression1 = "SUPPLIER = '" + possible_insert_row["SUPPLIER"].ToString().Replace("'", "''") + "'";
                            expression1 = expression1 + " AND DATE_UPDATED = '" + possible_insert_row["DATE_UPDATED"] + "'";
                            expression1 = expression1 + " AND DATE_EFFECTIVE = '" + possible_insert_row["DATE_EFFECTIVE"] + "'";
                            expression1 = expression1 + " AND FUEL_TYPE = '" + possible_insert_row["FUEL_TYPE"].ToString() + "'";
                            // Check for "£" sysmbol
                            if (possible_insert_row["DETAILS"].ToString().IndexOf(" ") >= 0)
                            {
                                possible_insert_row["DETAILS"] = possible_insert_row["DETAILS"].ToString().Replace(" ", SmartParametersV2016.defaultISOConvertToSymbol); // Is this default correct??
                            }
                            string details = possible_insert_row["DETAILS"].ToString();
                            details = SmartParseV2016.Remove_Double_Spaces_V3(details);
                            possible_insert_row["DETAILS"] = details;
                            expression1 = expression1 + " AND DETAILS = '" + possible_insert_row["DETAILS"].ToString().Replace("'", "''") + "'";

                            try
                            {
                                DataRow[] already_there = our_updates_table.Select(expression1);
                                switch (already_there.Count())
                                {
                                    case 0:
                                        our_updates_table.Rows.InsertAt(possible_insert_row, insert_position);
                                        insert_position = insert_position + 1;
                                        MainProcess.Output_Message(textBoxConsole,
                                                                "Inserted " + possible_insert_row["SUPPLIER"].ToString() +
                                                                            " " + possible_insert_row["DATE_UPDATED"] +
                                                                            " " + possible_insert_row["DATE_EFFECTIVE"] +
                                                                            " " + possible_insert_row["FUEL_TYPE"].ToString() +
                                                                            " " + possible_insert_row["DETAILS"].ToString(),
                                                                Mally.examine.scrape, Mally.console);
                                        break;
                                    case 1:
                                        // Found once
                                        break;
                                    default:
                                        // Found more than once
                                        MainProcess.Output_Message(textBoxConsole,
                                                                "**MULTIPLE ENTRIES** " + possible_insert_row["SUPPLIER"].ToString() +
                                                                            " " + possible_insert_row["DATE_UPDATED"] +
                                                                            " " + possible_insert_row["DATE_EFFECTIVE"] +
                                                                            " " + possible_insert_row["FUEL_TYPE"].ToString() +
                                                                            " " + possible_insert_row["DETAILS"].ToString(),
                                                                Mally.examine.scrape, Mally.console);

                                        break;
                                }
                            }
                            catch (Exception exception)
                            {
                                MainProcess.Output_Message(textBoxConsole,
                                                        "ERROR " + exception.Message,
                                                        Mally.examine.scrape, Mally.console);
                                return false;
                            }
                        }
                        if (all_done)
                        {
                            break;
                        }
                    skip_this:
                        continue;
                    }

                    break;
                }
            }

            // Fix what we've got
            dashboardmodel.energyupdates_table = our_updates_table;

            // Flick the SWITCH!!!
            if (EnergyUpdates.Checked == true)
            {
                EnergyUpdates.Checked = false;
            }
            else
            {
                EnergyUpdates.Checked = true;
            }

            // You may see the scrape starting BEFORE this message displays... C'est la vie ...
            MainProcess.Output_Message(textBoxConsole,
                                "EnergyUpdates checkbox " + EnergyUpdates.Checked,
                                Mally.examine.scrape, Mally.console);
            return true;
        }

        // This only does suppliers
        private static bool EL_Find_Switchable(MainProcess MainForm,
                                            SqlConnection SmartUtilityConnection,
                                            string username,
                                            HtmlAgilityPack.HtmlDocument document,
                                            DashBored Mally,
                                            DashboardModel dashboardmodel,
                                            RichTextBox textBoxConsole,
                                            char category_code,
                                            short brand_code,
                                            string defaultDates,
                                            DateTime final_date,
                                            CheckedListBox Brand_Matrix)
        {
            string[] lines = document.DocumentNode.InnerText.ToString().Split('\n');

            //  Green Star Energy currently have the following tariffs available for switching to:"
            string chosen = "Tariff name";

            int count = 0;

            string el_tariff_name = string.Empty;

            MainProcess.Output_Message(textBoxConsole,
                        "Switchable: ",
                        Mally.examine.scrape, Mally.console);

            // Check This Out!!
            List<SmartUtility.TariffMatrix> el_tariff_matrix_list = new List<SmartUtility.TariffMatrix>();

            while (count < lines.Count())
            {
                if (lines[count].IndexOf("have previously offered the tariffs for sale.") >= 0)
                {
                    break;
                }
                if (lines[count].IndexOf(chosen) >= 0)
                {
                    count = count + 1;
                    el_tariff_name = lines[count].Trim();
                    el_tariff_name = el_tariff_name.Replace("&pound;", "£");

                    SmartUtility.TariffMatrix el_tariff_matrix_row = new SmartUtility.TariffMatrix()
                    {
                        //el_tariff_matrix_row["TARIFF_NAME"] = el_tariff_name;
                        //el_tariff_matrix_table.Rows.Add(el_tariff_matrix_row);
                        TARIFF_NAME = el_tariff_name
                    };
                    el_tariff_matrix_list.Add(el_tariff_matrix_row);

                    //MainProcess.Output_Message(textBoxConsole,
                    //    el_tariff_name,
                    //    Mally.examine.scrape, Mally.console);


                    Check_Withdrawn_Date(MainForm,
                                            SmartUtilityConnection,
                                            true,      // Switchable
                                            true,   // Display default dates
                                            false,  // Don;t fix max withdrawn dates
                                            brand_code,
                                            el_tariff_name,
                                            Mally,
                                            dashboardmodel,
                                            textBoxConsole);
                }
                count = count + 1;
            }

            MainProcess.Output_Message(textBoxConsole,
                        "No longer available: ",
                        Mally.examine.scrape, Mally.console);
            while (count < lines.Count())
            {
                if (lines[count].IndexOf(chosen) >= 0)
                {
                    count = count + 1;
                    el_tariff_name = lines[count].Trim();
                    el_tariff_name = el_tariff_name.Replace("&pound;", "£");
                    //MainProcess.Output_Message(textBoxConsole,
                    //    el_tariff_name,
                    //    Mally.examine.scrape, Mally.console);
                    Check_Withdrawn_Date(MainForm,
                                            SmartUtilityConnection,
                                            false,         // Not switchable
                                            false,       // Don't Display default dates
                                            false,      // Don't fix max dates
                                            brand_code,
                                            el_tariff_name,
                                            Mally,
                                            dashboardmodel,
                                            textBoxConsole);
                }
                count = count + 1;
            }

            // having built the Switchable table - have we STILL got 
            // entries in OUR table which aren't in there ?
            // i.e. have we got some dates wrong??

            MainProcess.Output_Message(textBoxConsole,
                        "Leftovers: ",
                        Mally.examine.scrape, Mally.console);

            // Get list of Unique tariff matrix names
            List<SmartUtility.TariffMatrix> el_unique_list = new List<SmartUtility.TariffMatrix>();

            String[] fields = new String[2];
            fields[0] = "BRAND_CODE";
            fields[1] = brand_code.ToString();
            List<SmartUtility.TariffMatrix> tariff_matrix_found = SmartDatabaseV2016.READ_Records<SmartUtility.TariffMatrix>(MainForm.textBoxConsole,
                                                                                        SmartUtilityConnection,
                                                                        MainProcess.global_utility_tablesList,
                                                                        fields,
                                                                                        string.Empty,
                                                                                        new SmartUtility.TariffMatrix());
            // May have VERSION_CODE at 1 and 2
            if (tariff_matrix_found.Count() > 0)
            {
                foreach (SmartUtility.TariffMatrix tariff_matrix_row in tariff_matrix_found)
                {
                    List<SmartUtility.TariffMatrix> unique_found = new List<SmartUtility.TariffMatrix>(from Unique in el_unique_list
                                                                                                                                       where (Unique.TARIFF_NAME == tariff_matrix_row.TARIFF_NAME)
                                                                                                                                       select Unique);
                    if (unique_found.Count() == 0)
                    {
                        SmartUtility.TariffMatrix el_unique_row = new SmartUtility.TariffMatrix()
                        {
                            BRAND_CODE = tariff_matrix_row.BRAND_CODE,
                            RESOURCE_CODE = tariff_matrix_row.RESOURCE_CODE,
                            RESOURCE_TYPE = tariff_matrix_row.RESOURCE_TYPE,
                            TARIFF_CODE = tariff_matrix_row.TARIFF_CODE,
                            TARIFF_NAME = tariff_matrix_row.TARIFF_NAME
                        };
                        el_unique_list.Add(el_unique_row);
                    }
                }
            }

            foreach (SmartUtility.TariffMatrix tariff_matrix_row in el_unique_list)
            {
                List<SmartUtility.TariffMatrix> tariff_matrix_found_new = new List<SmartUtility.TariffMatrix>(from TariffM in el_tariff_matrix_list
                                                                                                                                              where (TariffM.TARIFF_NAME == tariff_matrix_row.TARIFF_NAME)
                                                                                                                                              select TariffM);

                if (tariff_matrix_found_new.Count() == 0)
                {
                    Check_Withdrawn_Date(MainForm,
                                        SmartUtilityConnection,
                                        false,         // Not switchable
                                        false,          // Don't display_defaultDates
                                        true,           // Set max dates to default dates
                                        brand_code,
                                        tariff_matrix_row.TARIFF_NAME,
                                        Mally,
                                        dashboardmodel,
                                        textBoxConsole);
                }

            }
            return true;
        }

        private static void Check_Withdrawn_Date(MainProcess MainForm,
                                            SqlConnection SmartUtilityConnection,
                                            bool switchable,
                                            bool display_defaultDates,
                                            bool fix_max_dates,
                                            short brand_code,
                                            string tariff_name,
                                            DashBored Mally,
                                            DashboardModel dashboardmodel,
                                            RichTextBox textBoxConsole)
        {

            bool from_history = false;
            String[] fields = new String[4];
            fields[0] = "BRAND_CODE";
            fields[1] = brand_code.ToString();
            fields[2] = "TARIFF_NAME";
            fields[3] = tariff_name;
            List<SmartUtility.TariffMatrix> tariff_matrix_found = SmartDatabaseV2016.READ_Records<SmartUtility.TariffMatrix>(MainForm.textBoxConsole,
                                SmartUtilityConnection,
                                                                        MainProcess.global_utility_tablesList,
                                                                        fields,
                                string.Empty,
                                new SmartUtility.TariffMatrix());
            // May have VERSION_CODE at 1 and 2
            if (tariff_matrix_found.Count() == 0)
            {
                // Perhaps the name is in the History?
                fields = new String[4];
                fields[0] = "BRAND_CODE";
                fields[1] = brand_code.ToString();
                fields[2] = "TARIFF_PREVIOUS_NAME";
                fields[3] = tariff_name;
                List<SmartUtility.TariffHistory> tariff_history_found = SmartDatabaseV2016.READ_Records<SmartUtility.TariffHistory>(MainForm.textBoxConsole,
                                                                        SmartUtilityConnection,
                                                                        MainProcess.global_utility_tablesList,
                                                                        fields,
                                string.Empty,
                                new SmartUtility.TariffHistory());
                if (tariff_history_found.Count() == 0)
                {
                    SmartDatabaseV2016.Add_Tick_Or_Cross(textBoxConsole,
                                            false,
                                            tariff_name + " " +
                                            brand_code.ToString() + " " +
                                            "not found in Tariffs History either",
                                            Mally);


                }
                else
                {
                    foreach (SmartUtility.TariffHistory tariff_history_row in tariff_history_found)
                    {
                        if (tariff_history_row.WITHDRAWN.ToString() == SmartParametersV2016.defaultDates)
                        {
                            return;
                        }
                    }
                    from_history = true;
                }
            }
            if (tariff_matrix_found.Count() > 0)
            {
                foreach (SmartUtility.TariffMatrix tariff_matrix_row in tariff_matrix_found)
                {

                    fields = new String[8];
                    fields[0] = "SUPPLIER_CODE";
                    fields[1] = tariff_matrix_row.BRAND_CODE.ToString();
                    fields[2] = "RESOURCE_CODE";
                    fields[3] = tariff_matrix_row.RESOURCE_CODE.ToString();
                    fields[4] = "RESOURCE_TYPE";
                    fields[5] = tariff_matrix_row.RESOURCE_TYPE;
                    fields[6] = "TARIFF_CODE";
                    fields[7] = tariff_matrix_row.TARIFF_CODE.ToString();
                    List<SmartUtility.Tariffs> tariffs_found = SmartDatabaseV2016.READ_Records<SmartUtility.Tariffs>(MainForm.textBoxConsole,
                                                                        SmartUtilityConnection,
                                                                        MainProcess.global_utility_tablesList,
                                                                        fields,
                                    string.Empty,
                                    new SmartUtility.Tariffs());

                    // May have VERSION_CODE at 1 and 2
                    if (tariffs_found.Count() > 0)
                    {
                        foreach (SmartUtility.Tariffs tariffs_row in tariffs_found)
                        {
                            string tariff_withdrawn_dates = tariffs_row.WITHDRAWN.ToString();
                            if (switchable)
                            {
                                if (tariff_withdrawn_dates == SmartParametersV2016.maximumDate)
                                {
                                    //SmartDatabaseV2016.Add_Tick_Or_Cross(textBoxConsole,
                                    //            true,
                                    //            tariff_name + " " +
                                    //            tariffs_row["TARIFF_CODE"].ToString() + " " +
                                    //            tariffs_row["RESOURCE_CODE"].ToString() + " " +
                                    //            tariffs_row["RESOURCE_TYPE"].ToString() + " " +
                                    //            tariffs_row["WITHDRAWN"].ToString(),
                                    //            Mally);
                                }
                                else
                                {
                                    SmartDatabaseV2016.Add_Tick_Or_Cross(textBoxConsole,
                                                false,
                                                tariff_name + " " +
                                                tariffs_row.TARIFF_CODE + " " +
                                                tariffs_row.RESOURCE_CODE + " " +
                                                tariffs_row.RESOURCE_TYPE + " " +
                                                tariffs_row.WITHDRAWN,
                                                Mally);
                                }
                            }
                            else
                            {
                                if (DateTime.Compare(Convert.ToDateTime(tariff_withdrawn_dates), DateTime.Now) < 0)
                                //== SmartParametersV2016.maximum_date ||
                                // switch_date == SmartParametersV2016.defaultDates))
                                {
                                    ////(switch_date + SmartParametersV2016.defaultDates);
                                    //SmartDatabaseV2016.Add_Tick_Or_Cross(textBoxConsole,
                                    //            true,
                                    //            tariff_name + " " +
                                    //            tariffs_row["TARIFF_CODE"].ToString() + " " +
                                    //            tariffs_row["RESOURCE_CODE"].ToString() + " " +
                                    //            tariffs_row["RESOURCE_TYPE"].ToString() + " " +
                                    //            tariffs_row["WITHDRAWN"].ToString(),
                                    //            Mally);
                                }
                                else
                                {
                                    if (tariff_withdrawn_dates == SmartParametersV2016.defaultDates &&
                                        !display_defaultDates)
                                    {
                                        continue;
                                    }
                                    SmartDatabaseV2016.Add_Tick_Or_Cross(textBoxConsole,
                                            false,
                                            tariff_name + " " +
                                            tariffs_row.TARIFF_CODE + " " +
                                            tariffs_row.RESOURCE_CODE + " " +
                                            tariffs_row.RESOURCE_TYPE + " " +
                                            tariffs_row.WITHDRAWN,
                                            Mally);
                                    if (fix_max_dates)
                                    {
                                        tariffs_row.WITHDRAWN = SmartParametersV2016.defaultDate;


                                    }
                                    if (from_history)
                                    {
                                        int pre_index = 0;
                                        pre_index = tariff_name.IndexOf("(pre");
                                        if (pre_index >= 0)
                                        {
                                            string hist_dates = tariff_name.Substring(pre_index);
                                            hist_dates = hist_dates.Replace("(pre", string.Empty);
                                            hist_dates = hist_dates.Replace(")", string.Empty);
                                            DateTime hist_date = Convert.ToDateTime(hist_dates);
                                            hist_date = hist_date.AddDays(-1);
                                            tariffs_row.VALID_TO = hist_date;
                                            tariffs_row.WITHDRAWN = SmartParametersV2016.defaultDate;
                                        }
                                        //(pre 10 / 06 / 2016) 
                                    }

                                    string[] tariff_fields = new String[32];
                                    tariff_fields[0] = "SUPPLIER_CODE";
                                    tariff_fields[1] = tariffs_row.SUPPLIER_CODE.ToString();
                                    tariff_fields[2] = "RESOURCE_CODE";
                                    tariff_fields[3] = tariffs_row.RESOURCE_CODE.ToString();
                                    tariff_fields[4] = "RESOURCE_TYPE";
                                    tariff_fields[5] = tariffs_row.RESOURCE_TYPE;
                                    tariff_fields[6] = "TARIFF_CODE";
                                    tariff_fields[7] = tariffs_row.TARIFF_CODE.ToString();
                                    tariff_fields[8] = "ONLINE_OPTION";
                                    tariff_fields[9] = tariffs_row.ONLINE_OPTION;
                                    tariff_fields[10] = "DUAL_FUEL";
                                    tariff_fields[11] = tariffs_row.DUAL_FUEL;
                                    tariff_fields[12] = "FALLBACK";
                                    tariff_fields[13] = tariffs_row.FALLBACK;
                                    tariff_fields[14] = "TARIFF_TYPE";
                                    tariff_fields[15] = tariffs_row.TARIFF_TYPE;
                                    tariff_fields[16] = "AVAILABILITY";
                                    tariff_fields[17] = tariffs_row.AVAILABILITY;
                                    tariff_fields[18] = "AGE";
                                    tariff_fields[19] = tariffs_row.AGE;
                                    tariff_fields[20] = "CUSTOMER";
                                    tariff_fields[21] = tariffs_row.CUSTOMER;
                                    tariff_fields[22] = "LAUNCHED";
                                    tariff_fields[23] = tariffs_row.LAUNCHED.ToString();
                                    tariff_fields[24] = "ISSUED";
                                    tariff_fields[25] = tariffs_row.ISSUED.ToString();
                                    tariff_fields[26] = "PRICES_VALID_FROM";
                                    tariff_fields[27] = tariffs_row.PRICES_VALID_FROM.ToString();
                                    tariff_fields[28] = "VALID_TO";
                                    tariff_fields[29] = tariffs_row.VALID_TO.ToString();
                                    tariff_fields[30] = "WITHDRAWN";
                                    tariff_fields[31] = tariffs_row.WITHDRAWN.ToString();

                                    SmartDatabaseV2016.UPDATE_Records<SmartUtility.Tariffs>(textBoxConsole,
                                                    SmartUtilityConnection,
                                                    tariff_fields,        // Got the keys ...

                                                                        MainProcess.global_utility_tablesList,

                                                                        tariffs_found[0]);
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                MainProcess.Output_Message(textBoxConsole,
                                      tariff_name + " " +
                                      " NOT FOUND",
                                      Mally.examine.scrape, Mally.console);
            }
            return;
        }

        private static bool EL_Find_New_Suppliers(MainProcess MainForm,
                                            SqlConnection SmartUtilityConnection,
                                            string username,
                                            HtmlAgilityPack.HtmlDocument document,
                                            DashBored Mally,
                                            DashboardModel dashboardmodel,
                                            RichTextBox textBoxConsole,
                                            char category_code,
                                            char resource_code,
                                            string defaultDates,
                                            DateTime final_date,
                                            CheckedListBox Brand_Matrix)
        {
            IList<HtmlAgilityPack.HtmlNode> HtmlCol1,
                                           HtmlCol2,
                                           HtmlCol3;


            HtmlCol1 = YetAnotherFuckingHoop.SelectNodesAsList(document.DocumentNode, "//li");
            foreach (HtmlAgilityPack.HtmlNode element1 in HtmlCol1)
            {
                string classname = element1.GetAttributeValue("class", string.Empty);
                if (SmartNibbyV2016.Check_Classname(classname, "grid-container", true))
                {
                    HtmlCol2 = YetAnotherFuckingHoop.SelectNodesAsList(element1, ".//ul");
                    foreach (HtmlAgilityPack.HtmlNode element2 in HtmlCol2)
                    {
                        classname = element2.GetAttributeValue("class", string.Empty);
                        if (SmartNibbyV2016.Check_Classname(classname, "yamm-list", true))
                        {
                            HtmlCol3 = YetAnotherFuckingHoop.SelectNodesAsList(element1, ".//a");
                            foreach (HtmlAgilityPack.HtmlNode element3 in HtmlCol3)
                            {
                                string href = element3.GetAttributeValue("href", string.Empty);
                                if (!string.IsNullOrEmpty(href))
                                {
                                    string brand_name = element3.InnerText.Trim();

                                    String[] fields = new String[4];
                                    fields[0] = "CUBEFACE_CODE";
                                    fields[1] = SmartParametersV2016.Utility.ToString();
                                    fields[2] = "BRAND_NAME";
                                    fields[3] = brand_name;
                                    List<SmartUtility.Brands> brands_found = SmartDatabaseV2016.READ_Records<SmartUtility.Brands>(MainForm.textBoxConsole,
                                                                                                SmartUtilityConnection,
                                                                        MainProcess.global_utility_tablesList,
                                                                        fields,
                                                                                                String.Empty,
                                                                                                new SmartUtility.Brands());
                                    // Suppliers should be in Area only once
                                    if (brands_found.Count() == 1)
                                    {
                                        //if (string.IsNullOrEmpty(brands_found[0]["HREF"].ToString()))
                                        //{
                                        brands_found[0].HREF = "https://energysuppliers.energylinx.co.uk" + href;
                                        //}
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return true;
        }

        // This only does suppliers
        private static bool EL_Find_Suppliers(MainProcess MainForm,
                                            SqlConnection SmartUtilityConnection,
                                            string username,
                                            HtmlAgilityPack.HtmlDocument document,
                                            DashBored Mally,
                                            DashboardModel dashboardmodel,
                                            RichTextBox textBoxConsole,
                                            char category_code,
                                            char resource_code,
                                            string defaultDates,
                                            DateTime final_date,
                                            CheckedListBox Brand_Matrix)
        {
            string function = string.Empty;

            switch (resource_code)
            {
                case SmartParametersV2016.Electricity:
                    function = "function setOptionsElectric(chosen, selbox)";
                    break;
                case SmartParametersV2016.Gas:
                    function = "function setOptionsGas(chosen, selbox)";
                    break;
                default:
                    break;
            }

            if (string.IsNullOrEmpty(function))
            {
                return false;
            }
            else
            {
                string[] lines = document.DocumentNode.InnerText.ToString().Split('\n');

                string chosen = "if (chosen == ";

                int count = 0;
                bool found_start = false;
                try
                {
                    List<SmartUtility.Brands> el_brands_list = new List<SmartUtility.Brands>();
                    List<SmartUtility.Suppliers> el_suppliers_list = new List<SmartUtility.Suppliers>();
                    el_suppliers_list.Clear();

                    string el_brand_name = string.Empty,
                            el_tariff_name = string.Empty;

                    while (count < lines.Length)
                    {
                        if (found_start)
                        {
                            if (lines[count].IndexOf(chosen) >= 0)
                            {
                                el_brand_name = lines[count].Replace(chosen, string.Empty);
                                el_brand_name = el_brand_name.Replace("\\", string.Empty).Trim();
                                el_brand_name = el_brand_name.Replace("\"", string.Empty).Trim();
                                el_brand_name = el_brand_name.Replace(")", string.Empty).Trim();
                                el_brand_name = el_brand_name.Replace("{", string.Empty).Trim();
                                el_brand_name = el_brand_name.Replace("'", string.Empty).Trim(); // For Sainsbury's
                                if (el_brand_name == "E")
                                {
                                    el_brand_name = string.Empty;
                                }
                                if (!string.IsNullOrEmpty(el_brand_name))
                                {
                                    if (el_brand_name.IndexOf("UK plc") >= 0)
                                    {
                                        el_brand_name = el_brand_name + ")";  // For (UK plc)
                                    }
                                    SmartUtility.Brands el_brands_row = new SmartUtility.Brands()
                                    {
                                        BRAND_NAME = el_brand_name
                                    };
                                    el_brands_list.Add(el_brands_row);
                                    SmartUtility.Suppliers el_suppliers_row = new SmartUtility.Suppliers();
                                    el_suppliers_list.Add(el_suppliers_row);
                                    //MainProcess.Output_Message(textBoxConsole,
                                    //                    "Added: " + el_brand_name,
                                    //                    Mally.examine.scrape, Mally.console);
                                }
                            }
                        }
                        else
                        {
                            if (lines[count].IndexOf(function) >= 0)
                            {
                                found_start = true;
                            }
                        }
                        count = count + 1;
                    }
                    MainProcess.Output_Message(textBoxConsole,
                                        "Totals: Suppliers " + el_suppliers_list.Count.ToString(),
                                        Mally.examine.scrape, Mally.console);

                    // New Suppliers in EL?
                    foreach (SmartUtility.Brands el_brands_row in el_brands_list)
                    {
                        string[] fields;
                        fields = new String[2];
                        fields[0] = "BRAND_NAME";
                        fields[1] = el_brands_row.BRAND_NAME;
                        List<SmartUtility.Brands> brands_found = SmartDatabaseV2016.READ_Records<SmartUtility.Brands>(MainForm.textBoxConsole,
                                                                                    SmartUtilityConnection,
                                                                                    MainProcess.global_utility_tablesList,
                                                                                    fields,
                                                                                    String.Empty,
                                                                                    new SmartUtility.Brands());
                        if (brands_found.Count() == 0)
                        {
                            fields[0] = "ALTERNATIVE_NAME1";

                            brands_found = SmartDatabaseV2016.READ_Records<SmartUtility.Brands>(MainForm.textBoxConsole,
                                                                                    SmartUtilityConnection,
                                                                                    MainProcess.global_utility_tablesList,
                                                                                    fields,
                                                                                    String.Empty,
                                                                                    new SmartUtility.Brands());
                            if (brands_found.Count() == 0)
                            {
                                fields[0] = "ALTERNATIVE_NAME";
                                brands_found = SmartDatabaseV2016.READ_Records<SmartUtility.Brands>(MainForm.textBoxConsole,
                                                                                    SmartUtilityConnection,
                                                                                    MainProcess.global_utility_tablesList,
                                                                                    fields,
                                                                                    String.Empty,
                                                                                    new SmartUtility.Brands());
                                if (brands_found.Count() == 0)
                                {
                                    fields[0] = "DASHBOARD_NAME";
                                    brands_found = SmartDatabaseV2016.READ_Records<SmartUtility.Brands>(MainForm.textBoxConsole,
                                                                                    SmartUtilityConnection,
                                                                                    MainProcess.global_utility_tablesList,
                                                                                    fields,
                                                                                    String.Empty,
                                                                                    new SmartUtility.Brands());
                                    if (brands_found.Count() == 0)
                                    {
                                        MainProcess.Output_Message(textBoxConsole,
                                                    "EL: Name " +
                                                    el_brands_row.BRAND_NAME +
                                                    " not found in Brands",
                                                    Mally.examine.scrape, Mally.console);
                                        char propogate_tariffs = SmartParametersV2016.propogateTariffsDefault,
                                                active = SmartParametersV2016.activeDefault;
                                        string bill_token = string.Empty,
                                                    url_prefix = string.Empty,
                                                    url_target = string.Empty;
                                        string bill_currency_symbol = SmartParametersV2016.defaultISOConvertToSymbol;
                                        char bill_denomination_symbol = SmartParametersV2016.defaultDenominationSymbol;
                                        char bill_currency_separator = SmartParametersV2016.defaultCurrencySeparator;
                                        char bill_thousands_separator = SmartParametersV2016.defaultThousandsSeparator;
                                        char bill_prefix = SmartParametersV2016.defaultBillprfix;


                                        // Need to add it in here ....
                                        // If you ARE going to do the job! Do it properly .... make sure
                                        // you don't - GURGLE! - have any (GULP!!) 'code smells'!!!!  Like those
                                        // ignorant rude stupid gingers!!

                                        // Default the Supplier Areas to N
                                        for (int i = 0; i < Brand_Matrix.Items.Count; i++)
                                        {
                                            Brand_Matrix.SetItemCheckState(i, CheckState.Unchecked);
                                        }

                                        dashboardmodel.errorMessage = string.Empty;

                                        dashboardmodel.brands_row = new SmartUtility.Brands();
                                        dashboardmodel.suppliers_row = new SmartUtility.Suppliers();

                                        if (!DashboardUtilityV2018.Add_Supplier(dashboardmodel,
                                                                    textBoxConsole,
                                                                    SmartUtilityConnection,
                                                                    category_code,
                                                                    resource_code,
                                                                    el_brands_row.BRAND_NAME,
                                                                    el_brands_row.BRAND_NAME,  // Alternative1 IS the Supplier name to start off with
                                                                    el_brands_row.BRAND_NAME,  // Dashboard name IS the Supplier name initially
                                                                    el_brands_row.BRAND_NAME,  // USwitch name IS the Supplier name initially
                                                                    el_brands_row.BRAND_NAME,  // USwitch scraper IS the Supplier name initially
                                                                    0,                          // Supplier
                                                                    0,                          // Brand                       
                                                                    DateTime.Today,
                                                                    final_date,
                                                                    defaultDates,
                                                                    propogate_tariffs,
                                                                    active,
                                                                    bill_token,
                                                                    bill_currency_symbol,
                                                                    bill_denomination_symbol,
                                                                    bill_currency_separator,
                                                                    bill_thousands_separator,
                                                                    bill_prefix,
                                                                    url_prefix,
                                                                    url_target,
                                                                    Brand_Matrix,
                                                                    string.Empty))   // Is this correct? For Status Flag blank or 'E' for energylinx??                                                                    
                                        {
                                            MainProcess.Output_Message(textBoxConsole,
                                                                "INSERT FAILED: " + el_brands_row.BRAND_NAME,
                                                                Mally.examine.scrape, Mally.console);
                                            return false;
                                        }
                                        else
                                        {
                                            MainProcess.Output_Message(textBoxConsole,
                                                                "Inserted: " + el_brands_row.BRAND_NAME,
                                                                Mally.examine.scrape, Mally.console);
                                        }
                                    }
                                }
                                //el_suppliers_row["SUPPLIER_CODE"] = el_brands_row["SUPPLIER_CODE"];
                            }
                        }
                    }
                }
                catch (Exception exception)
                {
                    MainProcess.Output_Message(textBoxConsole,
                                        "***EXCEPTION***: " + exception.Message,
                                        Mally.examine.scrape, Mally.console);
                    return false;
                }
            }
            return true;
        }

        private static bool EL_Find_Tariffs(MainProcess MainForm,
                                SqlConnection SmartUtilityConnection,
                                string username,
                                HtmlAgilityPack.HtmlDocument document,
                                DashBored Mally,
                                DashboardModel dashboardmodel,
                                RichTextBox textBoxConsole,
                                char category_code,
                                char resource_code,
                                string resource_type,
                                string defaultDates,
                                DateTime defaultDate,
                                string brand_name,
                                CheckedListBox Tariffs_Areas_Matrix,
                                bool missing)
        {
            string start_function = string.Empty,
                    end_function = "if (selbox.options.length == 0)";

            switch (resource_code)
            {
                case SmartParametersV2016.Electricity:
                    start_function = "function setOptionsElectric(chosen, selbox)";
                    break;
                case SmartParametersV2016.Gas:
                    start_function = "function setOptionsGas(chosen, selbox)";
                    break;
                default:
                    break;
            }

            if (string.IsNullOrEmpty(start_function))
            {
                return false;
            }
            else
            {
                string[] lines = document.DocumentNode.InnerText.ToString().Split('\n');

                string chosen = "if (chosen == ";
                string tequals = "t = ";

                int count = 0;
                bool found_start = false;
                try
                {
                    if (!string.IsNullOrEmpty(brand_name))
                    {
                        List<SmartUtility.Brands> el_brands_list = new List<SmartUtility.Brands>();
                        el_brands_list.Clear();
                        List<SmartUtility.Suppliers> el_suppliers_list = new List<SmartUtility.Suppliers>();
                        el_suppliers_list.Clear();
                        List<SmartUtility.TariffMatrix> el_tariff_matrix_list = new List<SmartUtility.TariffMatrix>();
                        el_tariff_matrix_list.Clear();

                        string el_brand_name = string.Empty,
                                el_tariff_name = string.Empty;

                        while (count < lines.Count())
                        {
                            if (found_start)
                            {
                                if (lines[count].IndexOf(end_function) >= 0)
                                {
                                    break;
                                }
                                if (lines[count].IndexOf(chosen) >= 0)
                                {
                                    el_brand_name = lines[count].Replace(chosen, string.Empty);
                                    el_brand_name = el_brand_name.Replace("\\", string.Empty).Trim();
                                    el_brand_name = el_brand_name.Replace("\"", string.Empty).Trim();
                                    el_brand_name = el_brand_name.Replace(")", string.Empty).Trim();
                                    el_brand_name = el_brand_name.Replace("{", string.Empty).Trim();
                                    el_brand_name = el_brand_name.Replace("'", string.Empty).Trim(); // For Sainsbury's
                                    if (!string.IsNullOrEmpty(el_brand_name))
                                    {
                                        if (el_brand_name.IndexOf("UK plc") >= 0)
                                        {
                                            el_brand_name = el_brand_name + ")";  // For (UK plc)
                                        }

                                        SmartUtility.Brands el_brands_row = new SmartUtility.Brands();

                                        dashboardmodel.brand_code = 0;
                                        dashboardmodel.supplier_code = 0;
                                        if (!DashboardUtilityV2018.Lookup_Supplier_Or_Brand_Code(MainForm.textBoxConsole, SmartUtilityConnection, category_code, el_brand_name, dashboardmodel, false))
                                        {
                                            // No need for this anymore - it confuses the missing display

                                            //MainProcess.Output_Message(textBoxConsole,
                                            //                    "Brand name no longer valid 3: " + category_code + " " +
                                            //                    el_brand_name,
                                            //                    Mally.examine.scrape, Mally.console);
                                        }
                                        else
                                        {
                                            if (!DashboardUtilityV2018.Lookup_Supplier_Or_Brand_Code(MainForm.textBoxConsole, SmartUtilityConnection, category_code, el_brand_name, dashboardmodel, true))
                                            {
                                                MainProcess.Output_Message(textBoxConsole,
                                                                    "Brand name no longer valid 4: " + category_code + " " +
                                                                    el_brand_name,
                                                                    Mally.examine.scrape, Mally.console);
                                            }
                                            else
                                            {
                                                el_brands_row.BRAND_NAME = el_brand_name;
                                                el_brands_row.BRAND_CODE = dashboardmodel.brand_code;
                                                el_brands_row.SUPPLIER_CODE = dashboardmodel.supplier_code;
                                                el_brands_list.Add(el_brands_row);
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    if (lines[count].IndexOf(tequals) >= 0)
                                    {
                                        el_tariff_name = lines[count].Replace(tequals, string.Empty);
                                        el_tariff_name = el_tariff_name.Replace("\\", string.Empty).Trim();
                                        el_tariff_name = el_tariff_name.Replace("\"", string.Empty).Trim();
                                        el_tariff_name = el_tariff_name.Replace(";", string.Empty).Trim();
                                        el_tariff_name = el_tariff_name.Replace("{", string.Empty).Trim();

                                        if (!string.IsNullOrEmpty(el_tariff_name))
                                        {
                                            SmartUtility.TariffMatrix el_tariff_matrix_row = new SmartUtility.TariffMatrix();
                                            dashboardmodel.brand_code = 0;
                                            dashboardmodel.supplier_code = 0;
                                            if (!DashboardUtilityV2018.Lookup_Supplier_Or_Brand_Code(MainForm.textBoxConsole, SmartUtilityConnection, category_code, el_brand_name, dashboardmodel, false))
                                            {
                                                // Not needed anymore

                                                //MainProcess.Output_Message(textBoxConsole,
                                                //                    "Brand name no longer valid 1: " + category_code + " " +
                                                //                    el_brand_name,
                                                //                    Mally.examine.scrape, Mally.console);
                                            }
                                            else
                                            {
                                                if (!DashboardUtilityV2018.Lookup_Supplier_Or_Brand_Code(MainForm.textBoxConsole, SmartUtilityConnection, category_code, el_brand_name, dashboardmodel, true))
                                                {
                                                    MainProcess.Output_Message(textBoxConsole,
                                                                        "Brand name no longer valid 2: " + category_code + " " +
                                                                        el_brand_name,
                                                                        Mally.examine.scrape, Mally.console);
                                                }
                                                else
                                                {
                                                    el_tariff_matrix_row.BRAND_CODE = dashboardmodel.brand_code;
                                                    el_tariff_matrix_row.RESOURCE_CODE = resource_code;
                                                    el_tariff_matrix_row.TARIFF_NAME = el_tariff_name;
                                                    el_tariff_matrix_list.Add(el_tariff_matrix_row);
                                                    //MainProcess.Output_Message(textBoxConsole,
                                                    //                    "Added: " + el_tariff_name,
                                                    //                    Mally.examine.scrape, Mally.console); 
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            else
                            {
                                if (lines[count].IndexOf(start_function) >= 0)
                                {
                                    found_start = true;
                                }
                            }
                            count = count + 1;
                        }
                        MainProcess.Output_Message(textBoxConsole,
                                            "Totals: Suppliers " + el_suppliers_list.Count.ToString() +
                                            " Tariffs " + el_tariff_matrix_list.Count.ToString(),
                                            Mally.examine.scrape, Mally.console);

                        // New Tariffs in EL?  Much more common than new suppliers in EL ...
                        foreach (SmartUtility.Brands el_brands_row in el_brands_list)
                        {
                            if (el_brands_row.BRAND_NAME.ToString().ToUpper() == brand_name.ToUpper()
                                || missing)
                            {
                                short brand_code = Convert.ToInt16(el_brands_row.BRAND_CODE);

                                string[] fields;
                                fields = new String[2];
                                fields[0] = "DASHBOARD_NAME";
                                fields[1] = el_brands_row.BRAND_NAME;
                                List<SmartUtility.Brands> brands_found = SmartDatabaseV2016.READ_Records(MainForm.textBoxConsole,
                                                                                            SmartUtilityConnection,
                                                                        MainProcess.global_utility_tablesList,
                                                                        fields,
                                                                                            string.Empty,
                                                                                            new SmartUtility.Brands());
                                if (brands_found.Count() == 0)
                                {
                                    fields[0] = "ALTERNATIVE_NAME1";
                                    brands_found = SmartDatabaseV2016.READ_Records(MainForm.textBoxConsole,
                                                                                            SmartUtilityConnection,
                                                                        MainProcess.global_utility_tablesList,
                                                                        fields,
                                                                                            string.Empty,
                                                                                            new SmartUtility.Brands());
                                    if (brands_found.Count() == 0)
                                    {
                                        fields[0] = "ALTERNATIVE_NAME";
                                        brands_found = SmartDatabaseV2016.READ_Records(MainForm.textBoxConsole,
                                                                                            SmartUtilityConnection,
                                                                        MainProcess.global_utility_tablesList,
                                                                        fields,
                                                                                            string.Empty,
                                                                                            new SmartUtility.Brands());
                                    }
                                }
                                if (brands_found.Count() > 0)
                                {
                                    // Does the Supplier have a type which matches resource_type??
                                    // Otherwise it flags up false positives
                                    string[] st_fields = new string[8];
                                    st_fields[0] = "CUBEFACE_CODE";
                                    st_fields[1] = category_code.ToString();
                                    st_fields[2] = "RESOURCE_CODE";
                                    st_fields[3] = resource_code.ToString();
                                    st_fields[4] = "RESOURCE_TYPE";
                                    st_fields[5] = resource_type;
                                    st_fields[6] = "SUPPLIER_CODE";
                                    st_fields[7] = brands_found[0].SUPPLIER_CODE.ToString();

                                    List<SmartUtility.SupplierTypes> st_found = SmartDatabaseV2016.READ_Records(MainForm.textBoxConsole,
                                                                                            SmartUtilityConnection,
                                                                                            MainProcess.global_utility_tablesList,
                                                                                            st_fields,
                                                                                            string.Empty,
                                                                                            new SmartUtility.SupplierTypes());

                                    if (st_found.Count() > 0)
                                    {
                                        string new_brand_name = el_brands_row.BRAND_NAME;

                                        char propogate_tariffs = SmartParametersV2016.propogateTariffsDefault;
                                        short version_code = SmartParametersV2016.defaultVersionCode;

                                        foreach (SmartUtility.Brands brands_row in brands_found)
                                        {
                                            brand_code = brands_row.BRAND_CODE;
                                            short supplier_code = brands_row.SUPPLIER_CODE;
                                            propogate_tariffs = brands_row.PROPOGATE_TARIFFS;
                                            if (brand_code > 0)
                                            {
                                                foreach (SmartUtility.TariffMatrix el_tariff_matrix_row in el_tariff_matrix_list)
                                                {
                                                    if (el_tariff_matrix_row.BRAND_CODE == brand_code &&
                                                        brand_code == supplier_code)
                                                    {
                                                        // Was here
                                                        dashboardmodel.tariff_code = 0;
                                                        if (!EL_Add_Tariff(dashboardmodel,
                                                                            MainForm,
                                                                            SmartUtilityConnection,
                                                                            category_code,
                                                                                resource_code,
                                                                                resource_type,
                                                                                supplier_code,
                                                                                brand_code,
                                                                                version_code,
                                                                                propogate_tariffs,
                                                                                new_brand_name,
                                                                                el_tariff_matrix_row.TARIFF_NAME,
                                                                                defaultDate,
                                                                                defaultDate,
                                                                                DateTime.Today,
                                                                                defaultDate,
                                                                                defaultDate,
                                                                                "N",                // Dual Fuel Only
                                                                                defaultDates,
                                                                                SmartParametersV2016.activeStatus,
                                                                                Tariffs_Areas_Matrix,
                                                                                Mally,
                                                                                textBoxConsole,
                                                                                false))  // Dont add Tarifss

                                                        {
                                                            MainProcess.Output_Message(textBoxConsole,
                                                                "Failed to add: Brand " + new_brand_name +
                                                                " Tariff " + el_tariff_matrix_row.TARIFF_NAME,
                                                                Mally.examine.scrape, Mally.console);
                                                            return false;
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
                catch (Exception exception)
                {
                    MainProcess.Output_Message(textBoxConsole,
                                        "***EXCEPTION***: " + exception.Message,
                                        Mally.examine.scrape, Mally.console);
                    return false;
                }
            }
            return true;
        }

        static public bool EL_Add_Tariff(DashboardModel dashboardmodel,
                                    MainProcess MainForm,
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
                                    RichTextBox textBoxConsole,
                                    bool really_add_it)
        {
            if (status_flag != string.Empty)
            {
                MainProcess.Output_Message(textBoxConsole,
                                    "!!!!!!!!!!****INSERT FLAG IS " + status_flag + " ******!!!!!!",

                                    Mally.examine.scrape, Mally.console);
            }
            string routine = MethodBase.GetCurrentMethod().Name.ToUpper();

            // The TARIFF_NAME is the ***SUPPLIER*** 
            string[] fields;
            fields = new String[10];
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
            List<SmartUtility.TariffMatrix> tariff_matrix_found = SmartDatabaseV2016.READ_Records<SmartUtility.TariffMatrix>(MainForm.textBoxConsole,
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
                List<SmartUtility.TariffHistory> tariff_history_found = SmartDatabaseV2016.READ_Records<SmartUtility.TariffHistory>(MainForm.textBoxConsole,
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

                    if (really_add_it)
                    {
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
                    else
                    {
                        MainProcess.Output_Message(textBoxConsole,
                                                "Not Inserted: " + brand_name + " / " + tariff_name,
                                                Mally.examine.scrape, Mally.console);
                        return true;

                    }
                }
            }
            return true;
        }

        public static bool Energylinx_CheckedChanged(MainProcess MainForm,
                                                SqlConnection SmartUtilityConnection,
                                                MainViewModel ourviewmodel,
                                                DashboardModel dashboardmodel,
                                                DashBored Mally,
                                                DateTime defaultDate,
                                                DateTime final_date,
                                                string defaultDates,
                                                string short_format,
                                                int blackindex,
                                                int colourindex)
        {
            string routine = MethodBase.GetCurrentMethod().Name.ToUpper();
            MainForm.comboBoxEL_List.Items.Clear();
            MainForm.richTextBoxEL.Clear();

            // Lets try and open Energy Updates
            if (dashboardmodel.energyupdates_table != null)
            {
                if (dashboardmodel.energyupdates_table.Rows.Count == 0)
                {
                    MainProcess.Output_Message(MainForm.richTextBoxEL,
                                    "Energy Updates table has no rows",
                                    Mally.examine.scrape, Mally.console);
                }
                else
                {
                    MainProcess.Output_Message(MainForm.richTextBoxEL,
                                    "Energy Updates table has " + dashboardmodel.energyupdates_table.Rows.Count + " rows",
                                    Mally.examine.scrape, Mally.console);
                    MainProcess.Output_Message(MainForm.richTextBoxEL,
                                    "New Updates table has " + dashboardmodel.newupdates_table.Rows.Count + " rows",
                                    Mally.examine.scrape, Mally.console);
                    if (dashboardmodel.newupdates_table.Rows.Count > 0)
                    {
                        MainProcess.Output_Message(MainForm.richTextBoxEL,
                                        "New Updates to be added " + dashboardmodel.newupdates_table.Rows.Count + " rows",
                                        Mally.examine.scrape, Mally.console);
                        dashboardmodel.energyupdates_table.Merge(dashboardmodel.newupdates_table);
                        MainProcess.Output_Message(MainForm.richTextBoxEL,
                                         "Energy Updates table merged with New Updates table " + dashboardmodel.energyupdates_table.Rows.Count + " rows",
                                         Mally.examine.scrape, Mally.console);
                        // Just because its got a Primary Key DOESN'T mean its sorted according to it!!
                        DataRow[] foundRows = dashboardmodel.energyupdates_table.Select("", "DATE_EFFECTIVE DESC, DATE_UPDATED DESC");
                        MainProcess.Output_Message(MainForm.richTextBoxEL,
                                        "Energy Updates sorted on " + foundRows.Count() + " rows",
                                        Mally.examine.scrape, Mally.console);
                        dashboardmodel.energyupdates_table = foundRows.CopyToDataTable();
                        MainProcess.Output_Message(MainForm.richTextBoxEL,
                                          "Energy Updates table re-created with " + dashboardmodel.energyupdates_table.Rows.Count + " rows",
                                          Mally.examine.scrape, Mally.console);
                    }
                    else
                    {
                        MainProcess.Output_Message(MainForm.richTextBoxEL,
                                        "No New Updates rows to be added",
                                        Mally.examine.scrape, Mally.console);
                    }

                    int process_count = 0;
                    int target_action = Convert.ToInt32(MainForm.textBoxEL_Action.Text);
                    int target_row = -1;
                    bool preview = false;

                    DateTime start_date = defaultDate;
                    if (Mally.console)
                    {
                        start_date = Convert.ToDateTime("01-Jun-2014 0:00:00");
                    }
                    else
                    {
                        start_date = Convert.ToDateTime(MainForm.dateTimePickerEL_DateTime.Text);
                    }

                    int total_rows = dashboardmodel.energyupdates_table.Rows.Count;
                    int total_index = -1;
                    if (total_rows > 0)
                    {
                        total_index = total_rows - 1;
                    }
                    
                    while (total_index >= 0)
                    {
                        try
                        {
                            DateTime amelia = Convert.ToDateTime(dashboardmodel.energyupdates_table.Rows[total_index]["DATE_EFFECTIVE"]);
                            if (DateTime.Compare(amelia, start_date) >= 0)  // This WAS > 0 originally but I can't remember why I did it like that
                            {
                                target_row = total_index;
                                // If the first column is Green then ignore it - its done
                                string[] colours = new string[2] { string.Empty, string.Empty };
                                colours = dashboardmodel.energyupdates_table.Rows[target_row]["COLOURS"].ToString().Split('|');
                                if (colours[0] != blackindex.ToString())
                                {
                                    // Its already been done
                                    goto update_row;
                                }
                                string brand_name = dashboardmodel.energyupdates_table.Rows[target_row]["SUPPLIER"].ToString();

                                dashboardmodel.supplier_code = 0;

                                if (!DashboardUtilityV2018.Lookup_Supplier_Or_Brand_Code(MainForm.textBoxConsole, SmartUtilityConnection, SmartParametersV2016.Utility, brand_name, dashboardmodel, true))
                                {
                                    MainProcess.Output_Message(MainForm.richTextBoxEL,
                                                       "(1) Brand Code zero for: " + SmartParametersV2016.Utility + " " +
                                                        brand_name,
                                                        Mally.examine.scrape, Mally.console);
                                    return false;
                                }
                                if (dashboardmodel.supplier_code == 0)
                                {
                                    // Supplier can't be found - > add it in??

                                    // If you ARE going to do the job! Do it properly .... make sure
                                    // you don't - GURGLE! - have any (GULP!!) 'code smells'!!!!  Like those
                                    // ignorant rude stupid gingers!!
                                    char propogate_tariffs = SmartParametersV2016.propogateTariffsDefault,
                                            active = SmartParametersV2016.activeDefault;
                                    string bill_token = string.Empty,
                                                url_prefix = string.Empty,
                                                url_target = string.Empty;
                                    string bill_currency_symbol = SmartParametersV2016.defaultISOConvertToSymbol;
                                    char bill_denomination_symbol = SmartParametersV2016.defaultDenominationSymbol;
                                    char bill_currency_separator = SmartParametersV2016.defaultCurrencySeparator;
                                    char bill_thousands_separator = SmartParametersV2016.defaultThousandsSeparator;
                                    char bill_prefix = SmartParametersV2016.defaultBillprfix;

                                    // Default the Supplier Areas to 'Y'
                                    // Well, now here's a thing.  We could default them all to N
                                    // or perhaps just AREA 17 (North of Scotland) but then we might
                                    // never know WHICH Areas the Suppplier really does cover.  So we
                                    // assume 'all of them' which makes the sam_count to 14 and then,
                                    // well, we are just going to have to update this matrix if the
                                    // scrape (for example for Area 17) fails ... its the best we can do!
                                    for (int i = 0; i < MainForm.Utility_Brands_Areas_Matrix.Items.Count; i++)
                                    {
                                        MainForm.Utility_Brands_Areas_Matrix.SetItemCheckState(i, CheckState.Checked);
                                    }
                                    // Clear down Area 17 (North Scotland) Disregard what I have written above!!
                                    MainForm.Utility_Brands_Areas_Matrix.SetItemCheckState(7, CheckState.Unchecked);

                                    dashboardmodel.suppliers_row = new SmartUtility.Suppliers();
                                    dashboardmodel.brands_row = new SmartUtility.Brands();
                                    dashboardmodel.errorMessage = string.Empty;
                                    if (!DashboardUtilityV2018.Add_Supplier(dashboardmodel,
                                                                MainForm.textBoxConsole,
                                                                SmartUtilityConnection,
                                                                SmartParametersV2016.Utility,
                                                                dashboardmodel.resource_code,
                                                                brand_name,
                                                                brand_name,  // Alternative IS the Supplier name to start off with
                                                                brand_name,  // Dashboard IS the Supplier name initially
                                                                brand_name,  // USwitch name IS the Supplier name initially
                                                                brand_name,  // USwitch scraper IS the Supplier name initially
                                                                dashboardmodel.supplier_code,  // When zero, creates a Brand with same Supplier
                                                                dashboardmodel.brand_code,  // When zero, creates a Brand with same Supplier
                                                                DateTime.Today,
                                                                final_date,
                                                                defaultDates,
                                                                propogate_tariffs,
                                                                active,
                                                                bill_token,
                                                                bill_currency_symbol,
                                                                bill_denomination_symbol,
                                                                bill_currency_separator,
                                                                bill_thousands_separator,
                                                                bill_prefix,
                                                                url_prefix,
                                                                url_target,
                                                                MainForm.Utility_Brands_Areas_Matrix,
                                                                string.Empty))
                                    {
                                        MainProcess.Output_Message(MainForm.richTextBoxEL,
                                                        "==> SUPPLIER INSERT FAILED: " + brand_name,
                                                        Mally.examine.scrape, Mally.console);
                                        goto update_row;
                                    }
                                    else
                                    {
                                        MainProcess.Output_Message(MainForm.richTextBoxEL,
                                                        "Supplier inserted: " + brand_name,
                                                        Mally.examine.scrape, Mally.console);
                                        dashboardmodel.supplier_code = dashboardmodel.suppliers_row.SUPPLIER_CODE;
                                    }
                                }

                                // Find the brand code for the Tariffs MATRIX (not the Tariffs - they are based on SUPPLIER_CODE, remember?)
                                dashboardmodel.brand_code = 0;
                                if (!DashboardUtilityV2018.Lookup_Supplier_Or_Brand_Code(MainForm.textBoxConsole, SmartUtilityConnection, SmartParametersV2016.Utility, brand_name, dashboardmodel, false))
                                {
                                    MainProcess.Output_Message(MainForm.richTextBoxEL,
                                                       "(2) Brand Code zero for: " + SmartParametersV2016.Utility + " " + brand_name,
                                                        Mally.examine.scrape, Mally.console);
                                    return false;
                                }

                                dashboardmodel.propogate_tariffs = DashboardUtilityV2018.Lookup_Supplier_Propogate(MainForm.textBoxConsole, SmartUtilityConnection, SmartParametersV2016.Utility, brand_name);

                                // Now ... in the case of Telecom Plus, it has no tariffs, they are all attached to Utility Warehouse
                                // So CHECK if the brand_code == supplier_code if there are any tariffs associated with the Brand?
                                if (dashboardmodel.brand_code == dashboardmodel.supplier_code)
                                {
                                    int sam_count = 0;
                                    SmartUtility.BrandMatrix brand_matrix_row = new SmartUtility.BrandMatrix();
                                    if (!Find_Sam_Count(MainForm,
                                                        SmartUtilityConnection,
                                                        dashboardmodel.brand_code,
                                                        brand_matrix_row,
                                                        MainForm.Utility_Brands_Areas_Matrix,
                                                        ref sam_count,
                                                        ref brand_matrix_row))
                                    {
                                        // There are no Tariffs for this Supplier/Brand
                                        // Look for the first Brand which matches the Supplier
                                        dashboardmodel.brand_code = 0;
                                        if (!DashboardUtilityV2018.Lookup_Special_Brand_Code(dashboardmodel,
                                                                                                MainForm.textBoxConsole,
                                                                                                SmartUtilityConnection,
                                                                                                SmartParametersV2016.Utility,
                                                                                                dashboardmodel.supplier_code))
                                        //rf brand_code_temp))
                                        {
                                            MainProcess.Output_Message(MainForm.richTextBoxEL,
                                                            "Special Brand Code not found: " +
                                                            SmartParametersV2016.Utility + " " +
                                                            dashboardmodel.supplier_code + "/" +
                                                            brand_name + " (",
                                                            Mally.examine.scrape, Mally.console);
                                            return false;
                                        }
                                        if (dashboardmodel.brand_code > 0)
                                        {
                                            //dashboardmodel.brand_code = dashboardmodel.brand_code;
                                        }
                                    }
                                }

                                // Lookup the REAL supplier name in case the one from Energy Updates is the alternative!
                                brand_name = DashboardUtilityV2018.Lookup_Brand_Name(MainForm.textBoxConsole, SmartUtilityConnection, SmartParametersV2016.Utility, dashboardmodel.brand_code, "SUPPLIER");    // Return supplier name
                                if (brand_name == "Telecom Plus")
                                {
                                    dashboardmodel.brand_code = 55;
                                }

                                DateTime launched = Convert.ToDateTime(dashboardmodel.energyupdates_table.Rows[target_row]["DATE_UPDATED"].ToString());
                                DateTime issued = Convert.ToDateTime(dashboardmodel.energyupdates_table.Rows[target_row]["DATE_UPDATED"].ToString());
                                string fuel_type = dashboardmodel.energyupdates_table.Rows[target_row]["FUEL_TYPE"].ToString();
                                DateTime prices_valid_from = Convert.ToDateTime(dashboardmodel.energyupdates_table.Rows[target_row]["DATE_EFFECTIVE"].ToString());
                                // If the Tariffs PRICES VALID FROM is greater than the Updates Spreadsheet, then use that!

                                DateTime tariff_valid_to = final_date;
                                DateTime withdrawn = tariff_valid_to;
                                string details = dashboardmodel.energyupdates_table.Rows[target_row]["DETAILS"].ToString().Replace("Re-pricing", "Repricing");

                                // Don't do this for Green Star Energy - many of their tariffs have dashes
                                // Or GB Energy - the same
                                // Or Co-op - the same ('scuse pun!)
                                if ((dashboardmodel.supplier_code != 115) &&        // Bulb Vari-Fair
                                        (dashboardmodel.supplier_code != 93) &&
                                        (dashboardmodel.supplier_code != 103) &&
                                        (dashboardmodel.supplier_code != 82) &&
                                        (dashboardmodel.supplier_code != 109) &&
                                        (dashboardmodel.supplier_code != 40) &&
                                        (dashboardmodel.supplier_code != 133) && // Powershop
                                        (dashboardmodel.supplier_code != 135) && // Toto
                                        (dashboardmodel.supplier_code != 149))   // Brilliant
                                {
                                    // Only consider the tariff name do up to the first dash
                                    // Only fuck about with the details if its not "Re-pricing"
                                    int dash = details.IndexOf(" - "); // So 'Energy-Free' doesn't get caught but 'Energy - 2017' does..
                                    if (dash >= 0)
                                    {
                                        if (details.Substring(dash + 1, details.Length - dash - 1).Trim() != "PAYG")
                                        {
                                            details = details.Substring(0, dash).Trim();
                                        }
                                    }
                                }

                                int action = 0;
                                // Do nothing
                                if (details.Contains("Launch"))
                                {
                                    action = 1;
                                    details = details.Replace("Launch of", string.Empty).Trim();
                                    details = details.Replace("Launch", string.Empty).Trim();
                                    details = details.Replace("new tariff", string.Empty).Trim();
                                    // Can we find any dates within details?
                                    if (dashboardmodel.supplier_code != 153)
                                    {
                                        if (!Extract_Date_From_Details(MainForm, details, ref tariff_valid_to, final_date))
                                        {
                                            MainProcess.Output_Message(MainForm.richTextBoxEL,
                                                                "Extract date failed: " +
                                                                details,
                                                                Mally.examine.scrape, Mally.console);
                                            return false;
                                        }
                                    }
                                }
                                if (details.Contains("Removal"))
                                {
                                    action = 3;
                                    details = details.Replace("Removal", string.Empty).Trim();
                                    withdrawn = Convert.ToDateTime(dashboardmodel.energyupdates_table.Rows[target_row]["DATE_EFFECTIVE"].ToString());
                                }

                                string[] actions_2 = new string[6] { "Update of",
                                                        "New version of",
                                                        "Price increase of",
                                                        "Price decrease of",
                                                        "Price amendment of",
                                                        "Repricing of"};
                                foreach (string operation in actions_2)
                                {
                                    // No wonder I never get anything done with that idiot CONTINUALLY WITTERING away
                                    if (details.Contains(operation))
                                    {
                                        action = 2;
                                        details = details.Replace(operation, string.Empty).Trim();

                                        string gas_price_decrease = "- Gas price decrease";
                                        if (details.Contains(gas_price_decrease))
                                        {
                                            details = details.Replace(gas_price_decrease, string.Empty).Trim();
                                        }
                                        string electricity_price_decrease = "- Electricity price decrease";
                                        if (details.Contains(electricity_price_decrease))
                                        {
                                            details = details.Replace(electricity_price_decrease, string.Empty).Trim();
                                        }
                                        break;
                                    }
                                }

                                details = details.Replace("of", string.Empty).Trim();
                                details = details.Replace("Elec Only", string.Empty);  //E.On
                                string local_tariff_name = details;

                                if ((action > 0) &&
                                    (action == target_action))
                                {
                                    string target_string = string.Empty;
                                    switch (action)
                                    {
                                        case 1:
                                            MainProcess.Output_Message(MainForm.richTextBoxEL,
                                                            "Processing LAUNCH: " + target_row + " " +
                                                            dashboardmodel.supplier_code + "/" +
                                                            dashboardmodel.brand_code + ")" + " " +
                                                            brand_name + " (" +
                                                            launched + " " +
                                                            issued + " " +
                                                            prices_valid_from + " " +
                                                            tariff_valid_to + " " +
                                                            local_tariff_name,
                                                            Mally.examine.scrape, Mally.console);
                                            if ((preview == false))
                                            {
                                                target_string = target_row.ToString() + ":" +
                                                                action + ":" +
                                                                dashboardmodel.supplier_code + ":" +
                                                                dashboardmodel.brand_code + ":" +
                                                                brand_name + ":" +
                                                                fuel_type + ":" +
                                                                local_tariff_name + ":" +
                                                                launched.ToString(short_format) + ":" +
                                                                issued.ToString(short_format) + ":" +
                                                                prices_valid_from.ToString(short_format) + ":" +
                                                                tariff_valid_to.ToString(short_format) + ":" +
                                                                withdrawn.ToString(short_format);

                                                if (dashboardmodel.propogate_tariffs == 'Y')
                                                {
                                                    short[] brands = DashboardUtilityV2018.Lookup_Brand_Codes(MainForm.textBoxConsole,
                                                                                                           SmartUtilityConnection,
                                                                                                           SmartParametersV2016.Utility,
                                                                                                           dashboardmodel.supplier_code);


                                                    foreach (short brand_code in brands)
                                                    {
                                                        // We did this one in the line above!
                                                        if (brand_code != dashboardmodel.brand_code)
                                                        {
                                                            target_string = target_string +
                                                                        SmartParametersV2016.bar +
                                                                        target_row.ToString() + ":" +
                                                                        action + ":" +
                                                                        dashboardmodel.supplier_code + ":" +
                                                                        brand_code + ":" +
                                                                        brand_name + ":" +
                                                                        fuel_type + ":" +
                                                                        local_tariff_name + ":" +
                                                                        launched.ToString(short_format) + ":" +
                                                                        issued.ToString(short_format) + ":" +
                                                                        prices_valid_from.ToString(short_format) + ":" +
                                                                        tariff_valid_to.ToString(short_format) + ":" +
                                                                        withdrawn.ToString(short_format);
                                                        }
                                                    }
                                                }
                                            }
                                            break;
                                        case 3:
                                            MainProcess.Output_Message(MainForm.richTextBoxEL,
                                                            "Processing REMOVAL: " + target_row + " " +
                                                            dashboardmodel.supplier_code + "/" +
                                                            dashboardmodel.brand_code + ")" + " " +
                                                            brand_name + " (" +
                                                            launched + " " +
                                                            tariff_valid_to + " " +
                                                            withdrawn + " " +
                                                            local_tariff_name,
                                                            Mally.examine.scrape, Mally.console);
                                            if ((preview == false))
                                            {
                                                target_string = target_row.ToString() + ":" +
                                                                action + ":" +
                                                                dashboardmodel.supplier_code + ":" +
                                                                dashboardmodel.brand_code + ":" +
                                                                brand_name + ":" +
                                                                fuel_type + ":" +
                                                                local_tariff_name + ":" +
                                                                issued.ToString(short_format) + ":" +
                                                                withdrawn.ToString(short_format);
                                            }
                                            break;
                                        case 2:
                                            MainProcess.Output_Message(MainForm.richTextBoxEL,
                                                            "Processing UPDATE: " + target_row + " " +
                                                            dashboardmodel.supplier_code + "/" +
                                                            dashboardmodel.brand_code + ")" + " " +
                                                            brand_name + " (" +
                                                            issued + " " +
                                                            prices_valid_from + " " +
                                                            local_tariff_name,
                                                            Mally.examine.scrape, Mally.console);
                                            if ((preview == false))
                                            {
                                                target_string = target_row.ToString() + ":" +
                                                                action + ":" +
                                                                dashboardmodel.supplier_code + ":" +
                                                                dashboardmodel.brand_code + ":" +
                                                                brand_name + ":" +
                                                                fuel_type + ":" +
                                                                local_tariff_name + ":" +
                                                                issued.ToString(short_format) + ":" +
                                                                prices_valid_from.ToString(short_format);
                                                // This has been added in because we weren't doing all the brands for SSE
                                                if (dashboardmodel.propogate_tariffs == 'Y')
                                                {
                                                    short[] brands = DashboardUtilityV2018.Lookup_Brand_Codes(MainForm.textBoxConsole,
                                                                                                            SmartUtilityConnection,
                                                                                                            SmartParametersV2016.Utility,
                                                                                                            dashboardmodel.supplier_code);


                                                    foreach (short brand_code in brands)
                                                    {
                                                        // We did this one in the line above!
                                                        if (brand_code != dashboardmodel.brand_code)
                                                        {
                                                            target_string = target_string +
                                                                        SmartParametersV2016.bar +
                                                                        target_row.ToString() + ":" +
                                                                        action + ":" +
                                                                        dashboardmodel.supplier_code + ":" +
                                                                        brand_code + ":" +
                                                                        brand_name + ":" +
                                                                        fuel_type + ":" +
                                                                        local_tariff_name + ":" +
                                                                        launched.ToString(short_format) + ":" +
                                                                        issued.ToString(short_format) + ":" +
                                                                        prices_valid_from.ToString(short_format) + ":" +
                                                                        tariff_valid_to.ToString(short_format) + ":" +
                                                                        withdrawn.ToString(short_format);
                                                        }
                                                    }
                                                }
                                            }
                                            break;

                                        default:
                                            break;
                                    }
                                    string[] targets = target_string.Split(SmartParametersV2016.bar);
                                    foreach (string target in targets)
                                    {
                                        MainForm.comboBoxEL_List.Items.Add(target);
                                        process_count = process_count + 1;
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                        }
                        if (!string.IsNullOrEmpty(MainForm.textBoxEL_Limit.Text) &&
                            MainForm.textBoxEL_Limit.Text != "0")
                        {
                            if (process_count >= Convert.ToInt32(MainForm.textBoxEL_Limit.Text))
                            {
                                break;
                            }
                        }
                    update_row:
                        total_index = total_index - 1;
                    }
                    MainProcess.Output_Message(MainForm.richTextBoxEL,
                                          "Items to be processed: " + MainForm.comboBoxEL_List.Items.Count,
                                          Mally.examine.scrape, Mally.console);
                    // Add a terminating trailer item
                    MainForm.comboBoxEL_List.Items.Add("-1");

                    if (Mally.console)
                    {
                        if (MainForm.comboBoxEL_List.Items.Count > 1)
                        {
                            // Start on the first?
                            MainForm.comboBoxEL_List.SelectedIndex = 0;
                        }
                    }
                }
            }
            return true;
        }

        public static bool Extract_Date_From_Details(MainProcess MainForm, string details, ref DateTime valid_to, DateTime final_date)
        {
            // Try September 2015 type dates
            int target_month = 0;
            int target_year = 0;
            int index = 0;
            foreach (string month_row in SmartParametersV2016.months)
            {
                string month = month_row;
                index = details.IndexOf(month);
                // Try cut-down month - try Sep
                if (index == -1)
                {
                    index = details.IndexOf(month.Substring(0, 3) + " ");
                    if (index >= 0)
                    {
                        month = month.Substring(0, 3);
                    }
                }
                // Try Sept
                if ((index == -1) &&
                    (target_month != 4))
                {
                    index = details.IndexOf(month.Substring(0, 4) + " ");
                    if (index >= 0)
                    {
                        month = month.Substring(0, 4);
                    }
                }

                if (index >= 0)
                {
                    // All this to get the end of the month found!
                    bool next_year = false;
                    if (target_month >= 11)
                    {
                        target_month = 0;
                        next_year = true;
                    }
                    else
                    {
                        target_month = target_month + 1;
                    }
                    string date_valid_to = "01-" + SmartParametersV2016.months[target_month].Substring(0, 3) + "-";
                    string remainder = details.Substring(index);
                    remainder = remainder.Replace(month, string.Empty).Trim();
                    if (remainder.Contains(")"))
                    {
                        remainder = remainder.Replace(")", string.Empty);
                    }
                    string[] years = remainder.Split(' ');
                    target_year = 0;
                    foreach (string years_row in years)
                    {
                        string year = years_row;
                        int version_index = year.IndexOf("v");
                        if (version_index == -1)
                        {
                            version_index = year.IndexOf("V");
                        }
                        if (version_index >= 0)
                        {
                            year = year.Substring(0, version_index);
                        }
                        // Only consider the first?
                        if (year.Length <= 2)
                        {
                            year = "20" + year;
                        }
                        try
                        {
                            target_year = Convert.ToInt32(year);
                        }
                        catch (Exception exception)
                        {
                            MainForm.richTextBoxEL.AppendText("Can't convert " + years_row + " " + exception.Message + Environment.NewLine.ToString());
                            MainForm.richTextBoxEL.ScrollToCaret();
                            return false;
                        }
                        if (next_year)
                        {
                            target_year = target_year + 1;
                        }
                        date_valid_to = date_valid_to + target_year.ToString("0000");
                        try
                        {
                            valid_to = Convert.ToDateTime(date_valid_to);
                        }
                        catch (Exception exception)
                        {
                            MainForm.richTextBoxEL.AppendText("Can't convert " + date_valid_to + " " + exception.Message + Environment.NewLine.ToString());
                            MainForm.richTextBoxEL.ScrollToCaret();
                            return false;
                        }
                        // Now get the end of the previous month
                        valid_to = valid_to.AddDays(-1);
                        return true;
                    }
                    break;
                }
                else
                {
                    target_month = target_month + 1;
                }
            }

            // Try 201510 type dates for iSupply
            if (DateTime.Compare(valid_to, final_date) == 0)
            {
                index = details.IndexOf("2015");
                if (index >= 0)
                {
                    string remainder = details.Substring(index);
                    if (remainder.Length >= 6)
                    {
                        target_month = Convert.ToInt32(remainder.Substring(4, 2));
                        if (target_month == 12)                 // December becomes
                        {
                            target_month = 0;                   // the January index
                        }
                    }
                    if (remainder.Length >= 4)
                    {
                        target_year = Convert.ToInt32(remainder.Substring(0, 4));
                        if (target_month == 0)
                        {
                            target_year = target_year + 1;
                        }
                    }
                    if (target_month < 12)
                    {
                        string date_valid_to = "01-" + SmartParametersV2016.months[target_month].Substring(0, 3) + "-" + target_year.ToString("0000");
                        try
                        {
                            valid_to = Convert.ToDateTime(date_valid_to);
                        }
                        catch (Exception exception)
                        {
                            MainForm.richTextBoxEL.AppendText("Can't convert " + date_valid_to + " " + exception.Message + Environment.NewLine.ToString());
                            MainForm.richTextBoxEL.ScrollToCaret();
                            return false;
                        }
                        // Now get the end of the previous month
                        valid_to = valid_to.AddDays(-1);
                        return true;
                    }
                }
            }
            return true;
        }


        private static bool Find_Sam_Count(MainProcess MainForm,
                                                SqlConnection SmartUtilityConnection,
                                                short brand_code,
                                                SmartUtility.BrandMatrix brand_matrix_row,
                                                CheckedListBox Brand_Matrix,
                                                ref int sam_count,
                                                ref SmartUtility.BrandMatrix brand_matrix_new)
        {
            // Need to find out from the Supplier Area Matrix how many areas are ticked                
            // Lookup all the Areas for this Brand

            String[] fields = new String[2];
            fields[0] = "BRAND_CODE";
            fields[1] = brand_code.ToString();
            List<SmartUtility.BrandMatrix> brand_matrix_found = SmartDatabaseV2016.READ_Records<SmartUtility.BrandMatrix>(MainForm.textBoxConsole,
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

        public static async Task<bool> EL_Suppliers_SIC(MainProcess MainForm,
                                                        SqlConnection SmartUtilityConnection,
                                                        SignInViewModel signinviewmodel,
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
            if (!DashboardUtilityV2018.Lookup_Supplier_Or_Brand_Code(MainForm.textBoxConsole, SmartUtilityConnection, SmartParametersV2016.Utility, MainForm.comboBoxEL_Suppliers.Text, dashboardmodel, true))
            {
                MainProcess.Output_Message(MainForm.richTextBoxEL,
                                   "(3) Brand Code zero for: " + SmartParametersV2016.Utility + " " + MainForm.comboBoxEL_Suppliers.Text,
                                    Mally.examine.scrape, Mally.console);
                return false;
            }
            dashboardmodel.brand_code = 0;
            if (!DashboardUtilityV2018.Lookup_Supplier_Or_Brand_Code(MainForm.textBoxConsole, SmartUtilityConnection, SmartParametersV2016.Utility, MainForm.comboBoxEL_Suppliers.Text, dashboardmodel, false))
            {
                MainProcess.Output_Message(MainForm.richTextBoxEL,
                                   "(4) Brand Code zero for: " + SmartParametersV2016.Utility + " " + MainForm.comboBoxEL_Suppliers.Text,
                                    Mally.examine.scrape, Mally.console);
                return false;
            }
            DashboardUtilityV2018.Update_Big4(MainForm.Utility_Area_Code, MainForm.Utility_Supplier_Code, MainForm.Utility_Tariff_Code, MainForm.Utility_Resource_Type, dashboardmodel.area_code, dashboardmodel.supplier_code, dashboardmodel.brand_code, dashboardmodel.tariff_code, dashboardmodel.resource_type);


            // Always talking herself up as per usual - Look At Me!  How fantastic I am!
            SmartRoutinesV2018.Check_Selected_Text(MainForm.comboBoxScraperSuppliers);
            MainForm.comboBoxScraperSuppliers.Text = MainForm.comboBoxScraperSuppliers.Text.Trim('~');

            DateTime prices_valid_from = defaultDate;

            string dashboard_name = DashboardUtilityV2018.Lookup_Brand_Name(MainForm.textBoxConsole,
                                                                   SmartUtilityConnection,
                                                                   SmartParametersV2016.Utility,
                                                                   dashboardmodel.brand_code, "DASHBOARD");   // Return DASHBOARD name
            string local_tariff_name = string.Empty;
            int sam_count = 0;
            SmartUtility.BrandMatrix brand_matrix_row = new SmartUtility.BrandMatrix();

            bool multiple_shot = false;
            if (MainForm.radioButtonEL_Multiple.Checked)
            {
                multiple_shot = true;
            }
            bool missing = false;
            if (MainForm.radioButtonEL_Missing.Checked)
            {
                missing = true;
            }

            string el_area = string.Empty;
            decimal uplift = 0.0M;

            dashboardmodel.target_count = Convert.ToInt32(MainForm.textBoxEL_Limit.Text);

            string last_target = string.Empty;
            dashboardmodel.one_e_found = false;
            dashboardmodel.one_e7_found = false;

            string el_filepath = Path.Combine(utility_pathname, "energylinx");
            string el_website = string.Empty;

            if (MainForm.radioButtonEL_Switchable.Checked)
            {
                // Lookup the appropriate Href
                string brand_name = dashboard_name;
                String[] fields = new String[4];
                fields[0] = "CUBEFACE_CODE";
                fields[1] = SmartParametersV2016.Utility.ToString();
                fields[2] = "BRAND_NAME";
                fields[3] = brand_name;
                List<SmartUtility.Brands> brands_found = SmartDatabaseV2016.READ_Records<SmartUtility.Brands>(MainForm.textBoxConsole,
                                                                                SmartUtilityConnection,
                                                                                MainProcess.global_utility_tablesList,
                                                                                fields,
                                                                                string.Empty,
                                                                                new SmartUtility.Brands());
                // Suppliers should be in Area only once
                if (brands_found.Count() == 1)
                {
                    if (!string.IsNullOrEmpty(brands_found[0].HREF))
                    {
                        el_website = brands_found[0].HREF;
                        bool result = await DoSuck_Energylinx(MainForm,
                                                SmartUtilityConnection,
                                                signinviewmodel,
                                                ourviewmodel,
                                                ourviewmodel.quitCts.Token,
                                                dashboardmodel,
                                                "SWITCHABLE",
                                                multiple_shot,
                                                missing,
                                                username,
                                                el_filepath,
                                                el_website,
                                                Mally,
                                                MainForm.richTextBoxEL,
                                                yymmdd_format,
                                                SmartParametersV2016.Utility,
                                                dashboardmodel.resource_code,
                                                dashboardmodel.resource_type,
                                                defaultDates,
                                                defaultDate,
                                                MainForm.comboBoxEL_Suppliers.Text.Trim('~'),
                                                dashboard_name,
                                                dashboardmodel.supplier_code,
                                                dashboardmodel.brand_code,
                                                dashboardmodel.version_code,
                                                MainForm.comboBoxEL_Tariffs.Text.Trim('~').Trim(SmartParametersV2016.placeholderDesignation),
                                                local_tariff_name,
                                                dashboardmodel.tariff_code,
                                                prices_valid_from,
                                                final_date,
                                                uplift,
                                                el_area,
                                                0,                          // target_row,
                                                0,                          // colourindex,
                                                0,                          // blackindex
                                                sam_count,                  // brand_matrix count
                                                brand_matrix_row,   // brand_matrix_Row
                                                MainForm.Utility_Brands_Areas_Matrix,
                                                MainForm.Utility_Tariffs_Areas_Matrix,
                                                MainForm.checkBoxEL_EnergyUpdates,
                                                MainForm.Conditions_Groups_Tariff_Plans,
                                                MainForm.Conditions_Plans_Limits,
                                                MainForm.Payments_Tariff_Plans,
                                                MainForm.comboBoxEL_List,
                                                MainForm.comboBoxEL_Tariffs,
                                                MainForm.textBoxUSW_VATRate,
                                                last_target);
                        if (result)
                        {

                        }
                    }
                }
            }
            if (multiple_shot || missing)
            {
                if (MainForm.textBoxEL_Uplift.Text.Length != 0)
                {
                    uplift = Convert.ToDecimal(MainForm.textBoxEL_Uplift.Text);
                    uplift = 1.0M + uplift / 100.0M;
                }

                if (MainForm.textBoxEL_Area.Text.Length != 0)
                {
                    el_area = MainForm.textBoxEL_Area.Text;
                }

                el_website = "https://www.energylinx.co.uk/energycalc.html";
                bool result = await DoSuck_Energylinx(MainForm,
                                            SmartUtilityConnection,
                                            signinviewmodel,
                                            ourviewmodel,
                                            ourviewmodel.quitCts.Token,
                                            dashboardmodel,
                                            "TARIFFS",
                                            multiple_shot,
                                            missing,
                                            username,
                                            el_filepath,
                                            el_website,
                                            Mally,
                                            MainForm.richTextBoxEL,
                                            yymmdd_format,
                                            SmartParametersV2016.Utility,
                                            dashboardmodel.resource_code,
                                            dashboardmodel.resource_type,
                                            defaultDates,
                                            defaultDate,
                                            MainForm.comboBoxEL_Suppliers.Text.Trim('~'),
                                            dashboard_name,
                                            dashboardmodel.supplier_code,
                                            dashboardmodel.brand_code,
                                            dashboardmodel.version_code,
                                            MainForm.comboBoxEL_Tariffs.Text.Trim('~').Trim(SmartParametersV2016.placeholderDesignation),
                                            local_tariff_name,
                                            dashboardmodel.tariff_code,
                                            prices_valid_from,
                                            final_date,
                                            uplift,
                                            el_area,
                                            0,                          // target_row,
                                            0,                          // colourindex,
                                            0,                          // blackindex
                                            sam_count,                  // brand_matrix count
                                            brand_matrix_row,   // brand_matrix_Row
                                            MainForm.Utility_Brands_Areas_Matrix,
                                            MainForm.Utility_Tariffs_Areas_Matrix,
                                            MainForm.checkBoxEL_EnergyUpdates,
                                            MainForm.Conditions_Groups_Tariff_Plans,
                                            MainForm.Conditions_Plans_Limits,
                                            MainForm.Payments_Tariff_Plans,
                                            MainForm.comboBoxEL_List,
                                            MainForm.comboBoxEL_Tariffs,
                                            MainForm.textBoxUSW_VATRate,
                                            last_target);
                if (result)
                {

                }
            }
            // Clear down current set
            MainForm.comboBoxEL_Tariffs.Items.Clear();
            DashboardUtilityV2018.Build_Tariff_Dropdown(MainForm.textBoxConsole,
                                                SmartUtilityConnection,
                                                SmartParametersV2016.Utility,
                                                MainForm.comboBoxEL_Tariffs,
                                                dashboardmodel.area_code,
                                                dashboardmodel.brand_code,
                                                dashboardmodel.resource_code,
                                                dashboardmodel.resource_type,
                                                dashboardmodel.tariff_code,
                                                dashboardmodel.version_code,
                                                false);       // Don't include deactivated items
            return true;
        }

        public static async Task<bool> DoSuck_Energylinx(MainProcess MainForm,
                                                SqlConnection SmartUtilityConnection,
                                                SignInViewModel signinviewmodel,
                                                MainViewModel ourviewmodel,
                                                CancellationToken cancellation_token,
                                                DashboardModel dashboardmodel,
                                                string start_routine,
                                                bool multiple_shot,
                                                bool missing,
                                                string username,
                                                string el_filepath,
                                                string el_website,
                                                DashBored Mally,
                                                RichTextBox richTextBoxEnergyLinx,
                                                string yymmdd_format,
                                                char category_code,
                                                char resource_code,
                                                string resource_type,
                                                string defaultDates,
                                                DateTime defaultDate,
                                                string brand_name,
                                                string dashboard_name,
                                                short supplier_code,
                                                short brand_code,
                                                short version_code,
                                                string local_tariff_name,
                                                string alternative_tariff_name,
                                                int tariff_code,
                                                DateTime prices_valid_from,
                                                DateTime final_date,
                                                decimal uplift,
                                                string el_area,
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
                                                ComboBox comboBoxEL,
                                                ComboBox comboBoxEL_Tariffs,
                                                Label ELVAT_Rate,
                                                string last_target)
        {
            string db = string.Empty;
            switch (resource_code)
            {
                case SmartParametersV2016.Electricity:
                    db = "?db=electric";
                    break;
                case SmartParametersV2016.Gas:
                    db = "?db=gas";
                    break;
                case 'D':
                    db = "?db=dual";
                    break;
                default:
                    break;
            }

            bool task = await Read_Website(MainForm,
                                            SmartUtilityConnection,
                                            signinviewmodel,
                                            ourviewmodel,
                                            dashboardmodel,
                                            cancellation_token,
                                            start_routine,
                                                multiple_shot,
                                                missing,
                                                username,
                                                el_filepath,
                                                el_website,
                                                el_website + db,
                                                Mally,
                                                richTextBoxEnergyLinx,
                                                yymmdd_format,
                                                category_code,
                                                resource_code,
                                                resource_type,
                                                defaultDates,
                                                defaultDate,
                                                brand_name,
                                                dashboard_name,
                                                supplier_code,
                                                brand_code,
                                                version_code,
                                                final_date,
                                                local_tariff_name,
                                                alternative_tariff_name,
                                                tariff_code,
                                                prices_valid_from,
                                                uplift,
                                                el_area,
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
                                                comboBoxEL,
                                                comboBoxEL_Tariffs,
                                                ELVAT_Rate,
                                                last_target);
            if (!SmartDatabaseV2016.Add_Tick_Or_Cross(richTextBoxEnergyLinx,
                                                task,
                                                "ENERGYLINX      : " + "Finished at " + DateTime.Now.ToString(yymmdd_format),
                                                Mally))
            {
                return false;
            }
            // Now!  Old misery guts is telling me aboout 'how children play'!!!  As if I had
            // never ever been a child myself!!!
            return task;
        }

        public static async Task<bool> EL_Tariffs_SIC(MainProcess MainForm,
                                                    SqlConnection SmartUtilityConnection,
                                                    SignInViewModel signinviewmodel,
                                                    MainViewModel ourviewmodel,
                                                    DashboardModel dashboardmodel,
                                                    string username,
                                                    DashBored Mally,
                                                    string utility_pathname,
                                                    string yymmdd_format,
                                                    string defaultDates,
                                                    DateTime defaultDate,
                                                    DateTime final_date)
        {
            SmartDatabaseV2016.Build_VAT_List(MainForm.textBoxConsole, 'U', ourviewmodel.Blanche.vatRatesList);

            DateTime prices_valid_from = defaultDate;

            string dashboard_name = DashboardUtilityV2018.Lookup_Brand_Name(MainForm.textBoxConsole,
                                                                        SmartUtilityConnection,
                                                                        SmartParametersV2016.Utility,
                                                                        dashboardmodel.brand_code, "DASHBOARD");   // Return DASHBOARD name

            // Set the Tariff based on the index selected
            if (!DashboardUtilityV2018.Lookup_New_Tariff_Code(dashboardmodel,
                                                            MainForm.textBoxConsole,
                                                            SmartUtilityConnection,
                                                            MainForm.TariffsMatrixVersionListBox,
                                                            dashboardmodel.area_code,
                                                            dashboardmodel.supplier_code,
                                                            dashboardmodel.brand_code,
                                                            dashboardmodel.resource_code,
                                                            dashboardmodel.resource_type,
                                                            MainForm.comboBoxEL_Tariffs.Text.Trim('~').Trim(SmartParametersV2016.placeholderDesignation)))
            {
                return false;
            }
            DashboardUtilityV2018.Update_Big4(MainForm.Utility_Area_Code, MainForm.Utility_Supplier_Code, MainForm.Utility_Tariff_Code, MainForm.Utility_Resource_Type, dashboardmodel.area_code, dashboardmodel.supplier_code, dashboardmodel.brand_code, dashboardmodel.tariff_code, dashboardmodel.resource_type);

            string local_tariff_name = string.Empty;


            // Look for all the Tariffs in this Supplier
            String[] fields = new String[8];
            fields[0] = "SUPPLIER_CODE";
            fields[1] = dashboardmodel.supplier_code.ToString();
            fields[2] = "RESOURCE_CODE";
            fields[3] = dashboardmodel.resource_code.ToString();
            fields[4] = "RESOURCE_TYPE";
            fields[5] = dashboardmodel.resource_type;
            fields[6] = "TARIFF_CODE";
            fields[7] = dashboardmodel.tariff_code.ToString();
            List<SmartUtility.Tariffs> tariffs_found = SmartDatabaseV2016.READ_Records<SmartUtility.Tariffs>(MainForm.textBoxConsole,
                                                                        SmartUtilityConnection,
                                                                        MainProcess.global_utility_tablesList,
                                                                        fields,
                                                                        string.Empty,
                                                                        new SmartUtility.Tariffs());
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
                List<SmartUtility.TariffMatrix> tariff_matrix_found = SmartDatabaseV2016.READ_Records<SmartUtility.TariffMatrix>(MainForm.textBoxConsole,
                                                                        SmartUtilityConnection,
                                                                        MainProcess.global_utility_tablesList,
                                                                        fields,
                                                string.Empty,
                                                new SmartUtility.TariffMatrix());
                if (tariff_matrix_found.Count() == 1)
                {
                    local_tariff_name = tariff_matrix_found[0].TARIFF_NAME;
                    MainForm.textBoxEL_Tariff_Name.Text = local_tariff_name;
                }
            }
            

            MainForm.labelEL_Prices_Valid_From.Text = "Prices valid from: " + prices_valid_from.ToString();

            // Need to find out from the Brand Matrix how many areas are ticked                
            // Lookup all the Areas for this Brand
            // BUT If the Brand = Supplier, then fix it so that the Areas are ticked for ALL brands
            int sam_count = 0;

            SmartUtility.BrandMatrix brand_matrix_row = new SmartUtility.BrandMatrix();

            if (dashboardmodel.supplier_code != dashboardmodel.brand_code)
            {
                // We really are doing a Brand here
                if (!Find_Sam_Count(MainForm,
                                    SmartUtilityConnection,
                                    dashboardmodel.brand_code,
                                    brand_matrix_row,
                                    MainForm.Utility_Brands_Areas_Matrix,
                                    ref sam_count,
                                    ref brand_matrix_row))
                {
                    // Need to put an error in here
                    return false;
                }
            }
            else
            {
                if (!Find_Sam_Count(MainForm,
                                    SmartUtilityConnection,
                                    dashboardmodel.supplier_code,
                                brand_matrix_row,
                                MainForm.Utility_Brands_Areas_Matrix,
                                ref sam_count,
                                ref brand_matrix_row))
                {
                    // And here
                    return false;
                }
            }

            decimal uplift = 0.0M;
            if (MainForm.textBoxEL_Uplift.Text.Length != 0)
            {
                uplift = Convert.ToDecimal(MainForm.textBoxEL_Uplift.Text);
                uplift = 1.0M + uplift / 100.0M;
            }

            string el_area = string.Empty;
            if (MainForm.textBoxEL_Area.Text.Length != 0)
            {
                el_area = MainForm.textBoxEL_Area.Text;
            }

            dashboardmodel.target_count = Convert.ToInt32(MainForm.textBoxEL_Limit.Text);
            bool multiple_shot = false;
            //if (MainForm.radioButtonEL_Single.Checked)
            //{
            //    multiple_shot = false;
            //}
            if (MainForm.radioButtonEL_Multiple.Checked)
            {
                multiple_shot = true;
            }
            bool missing = false;       // Don't do this one here

            string last_target = string.Empty;
            dashboardmodel.one_e_found = false;
            dashboardmodel.one_e7_found = false;

            string el_filepath = Path.Combine(utility_pathname, "energylinx");
            string el_website = "https://www.energylinx.co.uk/energycalc.html";

            bool result = await DoSuck_Energylinx(MainForm,
                                        SmartUtilityConnection,
                                        signinviewmodel,
                                        ourviewmodel,
                                        ourviewmodel.quitCts.Token,
                                        dashboardmodel,
                                        "LOGIN",
                                        multiple_shot,
                                        missing,
                                        username,
                                        el_filepath,
                                        el_website,
                                        Mally,
                                        MainForm.richTextBoxEL,
                                        yymmdd_format,
                                        SmartParametersV2016.Utility,
                                        dashboardmodel.resource_code,
                                        dashboardmodel.resource_type,
                                        defaultDates,
                                        defaultDate,
                                        MainForm.comboBoxEL_Suppliers.Text.Trim('~'),
                                        dashboard_name,
                                        dashboardmodel.supplier_code,
                                        dashboardmodel.brand_code,
                                        dashboardmodel.version_code,
                                        MainForm.comboBoxEL_Tariffs.Text.Trim('~').Trim(SmartParametersV2016.placeholderDesignation),
                                        local_tariff_name,
                                        dashboardmodel.tariff_code,
                                        prices_valid_from,
                                        final_date,
                                        uplift,
                                        el_area,
                                        0,                          // target_row,
                                        0,                          // colourindex,
                                        0,                          // blackindex
                                        sam_count,                  // brand_matrix count
                                        brand_matrix_row,   // brand_matrix_Row
                                        MainForm.Utility_Brands_Areas_Matrix,
                                        MainForm.Utility_Tariffs_Areas_Matrix,
                                        MainForm.checkBoxEL_EnergyUpdates,
                                        MainForm.Conditions_Groups_Tariff_Plans,
                                        MainForm.Conditions_Plans_Limits,
                                        MainForm.Payments_Tariff_Plans,
                                        MainForm.comboBoxEL_List,
                                        MainForm.comboBoxEL_Tariffs,
                                        MainForm.textBoxUSW_VATRate,
                                        last_target);
            return true;
        }

        public static void EL_SIC(MainProcess MainForm,
                                    SqlConnection SmartUtilityConnection,
                                    SignInViewModel signinviewmodel,
                                    MainViewModel ourviewmodel,
                                    DashboardModel dashboardmodel,
                                    string[] targets,
                                    string username,
                                    DashBored Mally,
                                    string yymmdd_format,
                                    DateTime defaultDate,
                                    string defaultDates,
                                    int blackindex,
                                    int colourindex,
                                    string utility_pathname,
                                    string smartutility_updates_name,
                                    string smartdatabase_directory_path,
                                    int target_row,
                                    DateTime final_date)
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
                default:
                    break;
            }
            // Don't forget this you fucking IMBECILE
            SmartDatabaseV2016.Build_VAT_List(MainForm.textBoxConsole, 'U', ourviewmodel.Blanche.vatRatesList);

            // Need to find out from the Supplier Area Matrix how many areas are ticked                
            // Lookup all the Areas for this Brand
            int sam_count = 0;

            SmartUtility.BrandMatrix brand_matrix_row = new SmartUtility.BrandMatrix();
            if (dashboardmodel.supplier_code != dashboardmodel.brand_code)
            {
                // We really are doing a Brand here
                if (!Find_Sam_Count(MainForm,
                                    SmartUtilityConnection,
                                    dashboardmodel.brand_code,
                                    brand_matrix_row,
                                    MainForm.Utility_Brands_Areas_Matrix,
                                    ref sam_count,
                                    ref brand_matrix_row))
                {
                    // Check if sam_count is zero - not worth doing ANYTHING if it is
                    MainProcess.Output_Message(MainForm.richTextBoxEL,
                                        "BRAND_MATRIX is **EMPTY** - aborting",
                                        Mally.examine.scrape, Mally.console);
                    return;
                }
            }
            else
            {
                if (!Find_Sam_Count(MainForm,
                                    SmartUtilityConnection,
                                    dashboardmodel.supplier_code,
                                    brand_matrix_row,
                                    MainForm.Utility_Brands_Areas_Matrix,
                                    ref sam_count,
                                    ref brand_matrix_row))
                {
                    // Check if sam_count is zero - not worth doing ANYTHING if it is
                    MainProcess.Output_Message(MainForm.richTextBoxEL,
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
                if (fuel_type.Contains("Dual"))
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
                    dashboardmodel.resource_code = Convert.ToChar(target);   // Kludge City!
                    MainProcess.Output_Message(MainForm.richTextBoxEL,
                                    "Energy code " + target,
                                    Mally.examine.scrape, Mally.console);

                    if (target.IndexOf("7") != -1)
                    {
                        local_resource_type = "VR";
                    }
                    dashboardmodel.resource_type = local_resource_type;
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
                                                                        MainForm.textBoxConsole,
                                                                        SmartUtilityConnection,
                                                                        MainForm.TariffsMatrixVersionListBox,
                                                                        dashboardmodel.area_code,
                                                                        dashboardmodel.supplier_code,
                                                                        dashboardmodel.brand_code,
                                                                        dashboardmodel.resource_code,
                                                                        dashboardmodel.resource_type,
                                                                        local_tariff_name))
                    {
                        switch (action)
                        {
                            case 1:
                                // LAUNCH - No Tariff?  We are only allowed this code for a Launch                           
                                dashboardmodel.tariff_code = 0;

                                //dashboardmodel.propogate_tariffs = SmartDatabaseV2016.lookup_supplier_propogate(SmartParametersV2016.Utility, brand_name);
                                dashboardmodel.resource_type = local_resource_type;
                                if (!EL_Add_Tariff(dashboardmodel,
                                                    MainForm,
                                                    SmartUtilityConnection,
                                                    SmartParametersV2016.Utility,
                                                    dashboardmodel.resource_code,
                                                    dashboardmodel.resource_type,
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
                                                    MainForm.Utility_Tariffs_Areas_Matrix,
                                                    Mally,
                                                    MainForm.richTextBoxEL,
                                                    true))          // Really Add Tariff
                                {
                                    MainProcess.Output_Message(MainForm.richTextBoxEL,
                                                target + " " + "Tariff NOT added: " + local_tariff_name,
                                                Mally.examine.scrape, Mally.console);
                                    return;
                                }
                                MainProcess.Output_Message(MainForm.richTextBoxEL,
                                                target + " " + "Tariff added: " + local_tariff_name,
                                                Mally.examine.scrape, Mally.console);
                                break;
                            case 3:
                                // REMOVAL - 
                                MainProcess.Output_Message(MainForm.richTextBoxEL,
                                                target + " " + "Tariff code must exist for Removal or Update " + action,
                                                Mally.examine.scrape, Mally.console);
                                break;

                            case 2:
                                // UPDATE - 
                                // Must have a tariff code if we are Removing or Updating
                                MainProcess.Output_Message(MainForm.richTextBoxEL,
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
                                MainProcess.Output_Message(MainForm.richTextBoxEL,
                                            "Action 1: Tariff code already exists " + dashboardmodel.tariff_code + " for Action " + action,
                                            Mally.examine.scrape, Mally.console);
                                // Just in case we have had an update AFTER  the Updates Table
                                DateTime tariff_prices_valid_from = DashboardUtilityV2018.Lookup_Tariff_Prices_Valid_From(MainForm.textBoxConsole,
                                                                                                                SmartUtilityConnection,
                                                                                                                dashboardmodel.area_code,
                                                                                                                dashboardmodel.supplier_code,
                                                                                                                dashboardmodel.resource_code,
                                                                                                                dashboardmodel.resource_type,
                                                                                                                dashboardmodel.tariff_code,
                                                                                                                dashboardmodel.version_code,
                                                                                                                defaultDate);

                                if (DateTime.Compare(prices_valid_from, tariff_prices_valid_from) > 0)
                                {
                                    MainProcess.Output_Message(MainForm.richTextBoxEL,
                                                "Action 1: Prices valid from altered to " + prices_valid_from + " for Action " + action,
                                                Mally.examine.scrape, Mally.console);
                                }

                                if (((dashboardmodel.resource_code == SmartParametersV2016.Electricity) && (fuel_type == "Electricity") && (target == "E")) ||
                                    ((dashboardmodel.resource_code == SmartParametersV2016.Gas) && (fuel_type == "Gas")))
                                {
                                    MainProcess.Output_Message(MainForm.richTextBoxEL,
                                                "Returning: Energy code " + dashboardmodel.resource_code + " Target " + target + " Fuel Type " + fuel_type,
                                                Mally.examine.scrape, Mally.console);
                                }

                                // We are carrying on - modify the tariff dates where necessary
                                dashboardmodel.tariffs_row = new SmartUtility.Tariffs();
                                if (!DashboardUtilityV2018.Modify_Tariff(dashboardmodel,
                                                                    MainForm.textBoxConsole,
                                                                    SmartUtilityConnection,
                                                                    false,
                                                                    dashboardmodel.resource_code,
                                                                    dashboardmodel.supplier_code,
                                                                    dashboardmodel.brand_code,
                                                                    dashboardmodel.resource_type,
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
                                                                    0,   // Paydays
                                                                    string.Empty,   // Status Flag
                                                                    defaultDate,
                                                                    launched,
                                                                    issued,
                                                                    prices_valid_from,
                                                                    tariff_valid_to,
                                                                    withdrawn))
                                {
                                    MainProcess.Output_Message(MainForm.richTextBoxEL,
                                            "Action " + action + ": Tariff NOT FOUND " + local_tariff_name + " for Action " + action,
                                            Mally.examine.scrape, Mally.console);
                                    //return false;
                                }
                                break;
                            case 3:
                                // REMOVAL - Can only have a Removal if it already exists
                                MainProcess.Output_Message(MainForm.richTextBoxEL,
                                            "Action " + action + ": Tariff already exists " + local_tariff_name + " for Action " + action,
                                            Mally.examine.scrape, Mally.console);
                                // We are carrying on - modify the tariff dates where necessary
                                dashboardmodel.tariffs_row = new SmartUtility.Tariffs();
                                if (!DashboardUtilityV2018.Modify_Tariff(dashboardmodel,
                                                                    MainForm.textBoxConsole,
                                                                    SmartUtilityConnection,
                                                                    false,
                                                                    dashboardmodel.resource_code,
                                                                    dashboardmodel.supplier_code,
                                                                    dashboardmodel.brand_code,
                                                                    dashboardmodel.resource_type,
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
                                    MainProcess.Output_Message(MainForm.richTextBoxEL,
                                            "Action " + action + ": Tariff NOT FOUND " + dashboardmodel.supplier_code + " " + dashboardmodel.resource_code + " " + dashboardmodel.resource_type + " " + dashboardmodel.tariff_code + " for Action " + action,
                                            Mally.examine.scrape, Mally.console);
                                    //return false;
                                }
                                else
                                {
                                    dashboardmodel.energyupdates_table.Rows[target_row]["COLOURS"] = colourindex.ToString() + "|" + blackindex.ToString();
                                    MainProcess.Output_Message(MainForm.richTextBoxEL,
                                                        "Updating Energy Updates table",
                                                        Mally.examine.scrape, Mally.console);
                                    MainProcess.Output_Message(MainForm.richTextBoxEL,
                                                    dashboardmodel.resource_code + " Processing next resource",
                                                    Mally.examine.scrape, Mally.console);
                                }

                                break;     // Not break! 'A-cos we are done here
                            case 2:
                                // UPDATE - Can only have an update if it already exists
                                MainProcess.Output_Message(MainForm.richTextBoxEL,
                                            "Action " + action + ": Tariff code already exists " + dashboardmodel.tariff_code + " for Action " + action,
                                            Mally.examine.scrape, Mally.console);
                                // Just in case we have had an update AFTER  the Updates Table
                                tariff_prices_valid_from = DashboardUtilityV2018.Lookup_Tariff_Prices_Valid_From(MainForm.textBoxConsole,
                                                                                                                SmartUtilityConnection,
                                                                                                                dashboardmodel.area_code,
                                                                                                                dashboardmodel.supplier_code,
                                                                                                                dashboardmodel.resource_code,
                                                                                                                dashboardmodel.resource_type,
                                                                                                                dashboardmodel.tariff_code,
                                                                                                                dashboardmodel.version_code,
                                                                                                                defaultDate);

                                if (DateTime.Compare(prices_valid_from, tariff_prices_valid_from) > 0)
                                {
                                    MainProcess.Output_Message(MainForm.richTextBoxEL,
                                                "Action " + action + ": Prices valid from altered to " + prices_valid_from + " for Action " + action,
                                                Mally.examine.scrape, Mally.console);
                                }

                                // We are carrying on - modify the tariff dates where necessary
                                dashboardmodel.tariffs_row = new SmartUtility.Tariffs();
                                if (!DashboardUtilityV2018.Modify_Tariff(dashboardmodel,
                                                                MainForm.textBoxConsole,
                                                                SmartUtilityConnection,
                                                                false,
                                                                dashboardmodel.resource_code,
                                                                dashboardmodel.supplier_code,
                                                                dashboardmodel.brand_code,
                                                                dashboardmodel.resource_type,
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
                                                                0,   // Paydays
                                                                string.Empty,   // Status Flag
                                                                defaultDate,
                                                                launched,           // does not change
                                                                issued,             // valid
                                                                prices_valid_from,  // valid
                                                                tariff_valid_to,    // does not change
                                                                withdrawn))          // does not change
                                {
                                    MainProcess.Output_Message(MainForm.richTextBoxEL,
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

                        string el_area = string.Empty;

                        DashboardUtilityV2018.Update_Big4(MainForm.Utility_Area_Code, MainForm.Utility_Supplier_Code, MainForm.Utility_Tariff_Code, MainForm.Utility_Resource_Type, dashboardmodel.area_code, dashboardmodel.supplier_code, dashboardmodel.brand_code, dashboardmodel.tariff_code, dashboardmodel.resource_type);

                        string meter = string.Empty;
                        if (target.Length > 1)
                        {
                            meter = target.Substring(1, 1);
                        }
                        dashboardmodel.resource_type = (meter == "7") ? "VR" : "SR";
                        //dashboardmodel.version_code = dashboardmodel.version_code;

                        string el_filepath = Path.Combine(utility_pathname, "energylinx");
                        string el_website = "https://www.energylinx.co.uk/energycalc.html";

                        // Make sure we look up 'Telecom Plus' for 'Utility Warehouse'
                        string dashboard_name = DashboardUtilityV2018.Lookup_Brand_Name(MainForm.textBoxConsole,
                                                                                    SmartUtilityConnection,
                                                                                    SmartParametersV2016.Utility,
                                                                                    dashboardmodel.brand_code,
                                                                                    "DASHBOARD");    // Return dashboard name

                        bool multiple_shot = true;
                        bool missing = false;
                        var result = DoSuck_Energylinx(MainForm,
                                        SmartUtilityConnection,
                                        signinviewmodel,
                                        ourviewmodel,
                                        ourviewmodel.quitCts.Token,
                                        dashboardmodel,
                                        "LOGIN",
                                        multiple_shot,           // always do multiples for these
                                        missing,
                                        username,
                                        el_filepath,
                                        el_website,
                                        Mally,
                                        MainForm.richTextBoxEL,
                                        yymmdd_format,
                                        SmartParametersV2016.Utility,
                                        dashboardmodel.resource_code,
                                        dashboardmodel.resource_type,
                                        defaultDates,
                                        defaultDate,
                                        brand_name,
                                        dashboard_name,
                                        dashboardmodel.supplier_code,
                                        dashboardmodel.brand_code,
                                        dashboardmodel.version_code,
                                        local_tariff_name,
                                        local_tariff_name,
                                        dashboardmodel.tariff_code,
                                        prices_valid_from,
                                        final_date,
                                        uplift,
                                        el_area,
                                        target_row,
                                        colourindex,
                                        blackindex,
                                        sam_count,
                                        brand_matrix_row,
                                        MainForm.Utility_Brands_Areas_Matrix,
                                        MainForm.Utility_Tariffs_Areas_Matrix,
                                        MainForm.checkBoxEL_EnergyUpdates,
                                        MainForm.Conditions_Groups_Tariff_Plans,
                                        MainForm.Conditions_Plans_Limits,
                                        MainForm.Payments_Tariff_Plans,
                                        MainForm.comboBoxEL_List,
                                        MainForm.comboBoxEL_Tariffs,
                                        MainForm.textBoxUSW_VATRate,
                                        last_target);
                        // if YOU PUT CHECKS IN HERE FOR SUCCESS OR FAILUE
                        // THE THE WHOLE FUCKING THING STALLS

                        //if (!result.Result)
                        //{
                        //    MainProcess.Output_Message(MainForm.textBoxEL,
                        //            "LOGIN failed ....",
                        //            Mally.examine.scrape, Mally.console);
                        //    //return false;
                        //}
                    }
                }
                if (action == 3)
                {
                    MainProcess.Output_Message(MainForm.richTextBoxEL,
                                    "Processing next tariff",
                                    Mally.examine.scrape, Mally.console);
                    MainForm.comboBoxEL_List.SelectedIndex = MainForm.comboBoxEL_List.SelectedIndex + 1;
                }
            }
        }
    }
}