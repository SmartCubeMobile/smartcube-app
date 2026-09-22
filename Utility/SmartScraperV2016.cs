// Same File?
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

#if WINFORMS
using SmartDashboard;
#endif


#if ANDROIDX
using AndroidX.AppCompat.App;
#endif

namespace SmartCubeMobile
{
    public class SmartScraperV2016
    {
        [RequiresUnreferencedCode("Calls SmartCubeMobile.SmartScraperV2016.Do_The_Reads(AppCompatActivity, MainViewModel, UtilityViewModel, Int16, Boolean)")]
        internal static async Task<bool> General_Meter(
#if WINFORMS
                                                        bool decode,
#endif
#if ANDROIDX
                                                        AppCompatActivity meterActivity,
#endif
                                                        MainViewModel ourviewmodel,
                                                        UtilityViewModel utilityviewmodel,
                                                        List<SmartUsers.VatRates> vatRatesList,
                                                        List<SmartUsers.ExchangeRates> exchangeRatesList,
                                                        List<SmartUsers.InternalPostcodes> postcodesList,
                                                        DateTime time_now,
                                                        int[] analysisCost,
                                                        bool submit_button,
                                                        short supplier_code,
                                                        short brand_code,
                                                        bool download_bills)
        {
            // Check the timeout once - here
            string http_message = SmartNibbyV2016.Check_Timeout(ourviewmodel);
            if (!string.IsNullOrEmpty(http_message))
            {
                // The timeout has to be 0 < timeout <= 120 for us to continue ... and its not!
                if (!await SmartRoutinesV2018.CheckTrace(ourviewmodel, utilityviewmodel.utilityToken, supplier_code, brand_code, SmartParametersV2016.Utility.ToString() + " " + http_message))
                {
                    return false;
                }
                return false;
            }

            download_bills = false;
#if WINFORMS
            if (true) //checkboxDownload_Bills.Checked)
            {
                download_bills = true;
            }
#endif
            Guid guid = Guid.NewGuid();

            Initialize_Utility_MES(ourviewmodel,
                                        utilityviewmodel,
                                        postcodesList,
#if WINFORMS
                                        decode,
#endif
                                        utilityviewmodel.UserId,
                                        utilityviewmodel.Password,
                                        utilityviewmodel.volumecorrection,
                                        utilityviewmodel.kwhconversion,
                                        utilityviewmodel.target_supplier_prfix,
                                        utilityviewmodel.target_supplier_xxx,
                                        utilityviewmodel.target_bill_token,
                                        utilityviewmodel.target_bill_currency_symbol,
                                        utilityviewmodel.target_bill_denomination_symbol,
                                        utilityviewmodel.target_bill_currency_separator,
                                        utilityviewmodel.target_bill_thousands_separator,
                                        utilityviewmodel.target_bill_prfix,
                                        utilityviewmodel.target_useProxy,
                                        download_bills,
                                        supplier_code,
                                        brand_code,
                                        SmartParametersV2016.defaultResourceCode,
                                        "",   // Default resource_type
                                              //utilityviewmodel.target_udprn,
                                        guid);

            // General controlling parameters
            bool TextBox_Active = true;                     // Turn it on
            if (!FrontEndGUI.GetBorderVisible(ourviewmodel))
            {
                FrontEndGUI.SetBorderScrollVisible(ourviewmodel);
            }

            // in it from last time
            // Get ready to Rock 'n Roll !!!
            //#if PRODUCTION
            //            if (useProxy == "Y")
            //            {
            //                proxy = true;
            //            }
            //#endif
            bool read_success = false;

            read_success = await Do_The_Reads(
#if WINFORMS
                                        //Scraper,                            
#endif
#if ANDROIDX
                                        meterActivity,
#endif
                                        ourviewmodel,
                                        utilityviewmodel,
                                        supplier_code,
                                        TextBox_Active);

            if (!read_success)
            {
#if WINFORMS
                // Do some analysis

                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Account status:" + utilityviewmodel.account_status + Environment.NewLine.ToString());

                System.Windows.Forms.CheckBox checkBoxStopOnError = new System.Windows.Forms.CheckBox();
                System.Windows.Forms.CheckBox checkBoxQuiet = new System.Windows.Forms.CheckBox();
                await SmartResultsV2016.MES_contents(checkBoxStopOnError,
                                                checkBoxQuiet,
                                                ourviewmodel,
                                                utilityviewmodel.Hezbollah.ameliaList,
                                                utilityviewmodel.Hezbollah.utility_accountsList,
                                                utilityviewmodel.Hezbollah.account_charges_creditsList,
                                                ourviewmodel.SmartProfile.profilecubefacesList,
                                                ourviewmodel.SmartProfile.addressesview_changesList,
                                                //utilityviewmodel.Hezbollah.utility_addressesList,
                                                utilityviewmodel.Hezbollah.utility_bankdetails_changesList,
                                                utilityviewmodel.Hezbollah.bills_changesList,
                                                utilityviewmodel.Hezbollah.bills_resource_changesList,
                                                utilityviewmodel.Hezbollah.payments_changesList,
                                                utilityviewmodel.Hezbollah.unallocated_changesList,
                                                utilityviewmodel.Hezbollah.e_readings_changesList,
                                                utilityviewmodel.Hezbollah.e_unit_charges_changesList,
                                                utilityviewmodel.Hezbollah.e_standing_charges_changesList,
                                                utilityviewmodel.Hezbollah.e_discounts_changesList,
                                                utilityviewmodel.Hezbollah.e_usage_changesList,
                                                utilityviewmodel.Hezbollah.g_readings_changesList,
                                                utilityviewmodel.Hezbollah.g_unit_charges_changesList,
                                                utilityviewmodel.Hezbollah.g_standing_charges_changesList,
                                                utilityviewmodel.Hezbollah.g_discounts_changesList,
                                                utilityviewmodel.Hezbollah.g_usage_changesList,
                                                utilityviewmodel.Hezbollah.utility_meters_changesList,
                                                utilityviewmodel.Hezbollah.utility_resources_changesList,

                                                utilityviewmodel.Hezbollah.utility_switches_changesList,
                                                utilityviewmodel.Hezbollah.supply_charges_credits_changesList,
                                                utilityviewmodel.Hezbollah.tariff_details_changesList);
#endif

                // We don't have the Login Attempted flag here, in case the
                // first web screen of the supplier is all wrong (i.e. no fields)
                if (!await SmartRoutinesV2018.CheckTrace(ourviewmodel, utilityviewmodel.utilityToken, supplier_code, brand_code, SmartParametersV2016.Utility.ToString() + " Attempted:" + utilityviewmodel.login_attempted))
                {
                    return false;
                }
                if (!await SmartRoutinesV2018.CheckTrace(ourviewmodel, utilityviewmodel.utilityToken, supplier_code, brand_code, SmartParametersV2016.Utility.ToString() + " Finished:" + utilityviewmodel.login_finished))
                {
                    return false;
                }
                // We never made it through ...
#if WINFORMS
                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Failed on: " + ourviewmodel.errorMessage + Environment.NewLine.ToString());
#endif
                return false;
            }
            // Don't send output to the TextBox anymore
            if (FrontEndGUI.GetBorderVisible(ourviewmodel))
            {
                FrontEndGUI.SetBorderScrollInvisible(ourviewmodel);
            }

            // MES is a mini-database
            // There may or may not be is AN account - do the general stuff

#if WINFORMS
            System.Windows.Forms.CheckBox insert = new System.Windows.Forms.CheckBox();
#endif
            bool result = await General_Stuff(
#if WINFORMS
                                                    insert, //checkBoxInsert_SmartSwitch,
#endif
                                                    ourviewmodel,
                                                    utilityviewmodel,// The 'outs' are in here
                                                    vatRatesList,
                                                    exchangeRatesList,
                                                    time_now,
                                                    submit_button);


            if (!result || !read_success)
            {
                if (!await SmartRoutinesV2018.CheckTrace(ourviewmodel, utilityviewmodel.utilityToken, supplier_code, brand_code, SmartParametersV2016.Utility.ToString() + " General Stuff problem"))
                {
                    return false;
                }
            }
            return result;
        }

        internal static void Initialize_Utility_MES(MainViewModel ourviewmodel,
                                            UtilityViewModel utilityviewmodel,
                                            List<SmartUsers.InternalPostcodes> postcodesList,

#if WINFORMS
                                            bool decode,
#endif
                                            string user_id,
                                            string password,
                                            decimal volumeCorrection,
                                            decimal kwhConversion,
                                            string prfix_xxx,
                                            string target_pathname,
                                            string bill_token,
                                            string bill_currency_symbol,
                                            char bill_denomination_symbol,
                                            char bill_currency_separator,
                                            char bill_thousands_separator,
                                            char bill_prfix,
                                            bool useProxy,
                                            bool download_bills,
                                            short supplier_code,
                                            short brand_code,
                                            char resource_code,
                                            string resource_type,
                                            Guid guid)
        {
            utilityviewmodel.scraperlast_usage_date = new DateTime[2];
            utilityviewmodel.scraperlast_usage_index = new short[2];

            Do_AmeliaLists(utilityviewmodel);

            MES_Do_HamasLists_Part2(utilityviewmodel);

            //MES_Do_FatahLists(ourviewmodel);
            MES_Do_HezbollahLists(ourviewmodel, utilityviewmodel, brand_code);

            utilityviewmodel.KWHCONVERSION = kwhConversion.ToString();
            utilityviewmodel.VOLUMECORRECTION = volumeCorrection.ToString();

            // MES is a Minnie database!
            utilityviewmodel.examine.scrape = ourviewmodel.trace;                    // 1
#if WINFORMS
            utilityviewmodel.examine.decode = decode;                    // 1
            utilityviewmodel.console = ourviewmodel.trace || decode;                          // 2
#endif
            utilityviewmodel.download_bills = download_bills;            // 3

            // Categories
            //utilityviewmodel.category_code = ourviewmodel.category_code;              // 24
            utilityviewmodel.face_active = SmartParametersV2016.defaultChar;
            utilityviewmodel.face_last_display = "";

            // These come from Utility.AccountsList
            utilityviewmodel.supplier_code = supplier_code;              // 7
            utilityviewmodel.brand_code = brand_code;                    // 6
            utilityviewmodel.account_no = "";// account_no;                  // 34
            utilityviewmodel.created = SmartParametersV2016.defaultDate; // created;

            // In case we have no Bills
            utilityviewmodel.ACCOUNT_NO = utilityviewmodel.account_no;
            utilityviewmodel.CREATED = utilityviewmodel.created;
            utilityviewmodel.BILL_DATE = SmartParametersV2016.defaultDates;

            if (string.IsNullOrEmpty(utilityviewmodel.bill_token))
            {
                utilityviewmodel.bill_token = bill_token;                // 8
            }
            utilityviewmodel.bill_currency_symbol = bill_currency_symbol;// 9
            utilityviewmodel.bill_denomination_symbol = bill_denomination_symbol;// 9
            utilityviewmodel.bill_currency_separator = bill_currency_separator;// 10
            utilityviewmodel.bill_thousands_separator = bill_thousands_separator;// 10
            utilityviewmodel.bill_prfix = bill_prfix;// 10
            utilityviewmodel.prfix_xxx = prfix_xxx;                    // 11
            utilityviewmodel.target_pathname = target_pathname;                    // 12
            utilityviewmodel.connection_timeout = ourviewmodel.timespanTimeout.Milliseconds;   // 14 - default one minute
            utilityviewmodel.last_usage_index = new short[2]           // 20
            {
                utilityviewmodel.scraperlast_usage_index[0],
                utilityviewmodel.scraperlast_usage_index[1]
            };
            utilityviewmodel.last_usage_date = new DateTime[2]           // 20
            {
                utilityviewmodel.scraperlast_usage_date[0],
                utilityviewmodel.scraperlast_usage_date[1]
            };
            // Read/Write internally section  
            utilityviewmodel.login_attempted = false;                    // 22
            utilityviewmodel.login_finished = false;                     // 23

            // All this bollocks so we can get the NEXT_CONNECTION updated if the Login Fails
            foreach (SmartUtility.Meters meters_row in utilityviewmodel.Hezbollah.utility_metersList)
            {
                switch (meters_row.RESOURCE_CODE)
                {
                    case SmartParametersV2016.Electricity:
                        utilityviewmodel.sparks.resource_type = meters_row.RESOURCE_TYPE;
                        utilityviewmodel.sparks.MPAN = meters_row.MPAN_MPRN;
                        utilityviewmodel.sparks.meter_serial_no = meters_row.METER_SERIAL_NO;
                        //utilityviewmodel.sparks.udprn = meters_row.UDPRN;
                        break;
                    case SmartParametersV2016.Gas:
                        utilityviewmodel.smell.resource_type = meters_row.RESOURCE_TYPE;
                        utilityviewmodel.smell.MPRN = meters_row.MPAN_MPRN;
                        utilityviewmodel.smell.meter_serial_no = meters_row.METER_SERIAL_NO;
                        //utilityviewmodel.smell.udprn = meters_row.UDPRN;
                        break;
                    default:
                        break;
                }
            }
            utilityviewmodel.sparks.payment_method = SmartParametersV2016.defaultChar;
            utilityviewmodel.sparks.tariff_code = 0;
            //if (string.IsNullOrEmpty(utilityviewmodel.sparks.udprn))
            //{
            //    utilityviewmodel.sparks.udprn = target_udprn;
            //}
            utilityviewmodel.smell.payment_method = SmartParametersV2016.defaultChar;
            utilityviewmodel.smell.tariff_code = 0; ;
            //if (string.IsNullOrEmpty(utilityviewmodel.smell.udprn))
            //{
            //    utilityviewmodel.smell.udprn = target_udprn;
            //}
            utilityviewmodel.area_code = 0;                              // 28 This should be set on return ...?
            utilityviewmodel.postcode = "";                    // 29
            utilityviewmodel.account_type = "";                // 30
            utilityviewmodel.payment_name = "";                // 33
            utilityviewmodel.fuelList = "";
            utilityviewmodel.account_name = "";
            utilityviewmodel.account_address = "";
            utilityviewmodel.bank_account_name = "";
            utilityviewmodel.bank_sort_code = "";
            utilityviewmodel.bank_account_number = "";
            utilityviewmodel.bank_payment_day = 0;
            utilityviewmodel.headers = new List<string>();

            // Addresses
            utilityviewmodel.ADDRESS = new GenericAddress()
            {
                building_name = "",
                building_number = "",
                code = "",
                country = "",
                county = "",
                dependant_locality = "",
                dependant_thoroughfare = "",
                double_dependant_locality = "",
                electricity = new Electricity
                {
                    MPAN = "",
                    tariff_code = 0,
                    contact_end_date = "",
                    energy_used = "",
                    personal_projection = "",
                    TCR = "",
                    payment_method = SmartParametersV2016.defaultChar
                },
                gas = new Gas
                {
                    MPRN = "",
                    tariff_code = 0,
                    contact_end_date = "",
                    energy_used = "",
                    personal_projection = "",
                    TCR = "",
                    payment_method = SmartParametersV2016.defaultChar
                },
                organization = "",
                pobox = "",
                postcode = "",
                thoroughfare = "",
                town = "",
                udprn = ""
            };
            // Utility.Accounts
            utilityviewmodel.bank_institution_code = 0;
            utilityviewmodel.bank_brand_code = 0;
            utilityviewmodel.account_status = 'Y';                     // OPen = 'Y' (assumed), Switching = 'S', Closed = 'N'
            utilityviewmodel.TARIFF_CODE = 0;
            utilityviewmodel.PAYMENT_PLAN = SmartParametersV2016.defaultChar;

            // Consumer
            utilityviewmodel.user_id = user_id;                          // 4
            utilityviewmodel.password = password;                        // 5
            utilityviewmodel.resource_code = resource_code;              // 24   Which may be the default ''
            utilityviewmodel.resource_type = resource_type;

            // Meters
            utilityviewmodel.ADDRESS.electricity.resource_type = "";                  // 44 Set to "SR" usually for E
            //utilityviewmodel.ADDRESS.electricity.udprn = "";
            utilityviewmodel.ADDRESS.electricity.meter_serial_no = "";
            utilityviewmodel.ADDRESS.gas.resource_type = "";                  // 44 Set to "SR" usually for E
            //utilityviewmodel.ADDRESS.gas.udprn = "";
            utilityviewmodel.ADDRESS.gas.meter_serial_no = "";

            //Resources
            utilityviewmodel.autoswitch = SmartParametersV2016.defaultAutoSwitch;
            utilityviewmodel.last_display = String.Empty;
            utilityviewmodel.age_60plus = "";
            utilityviewmodel.last_datetime = SmartParametersV2016.defaultDate;
            utilityviewmodel.next_connection = SmartParametersV2016.defaultDate;
            utilityviewmodel.last_update = SmartParametersV2016.defaultDate;
            utilityviewmodel.expiry_date = SmartParametersV2016.defaultDate;
            utilityviewmodel.autoswitch_destination = "";
            utilityviewmodel.autoswitch_supplier_code = 0;
            utilityviewmodel.autoswitch_brand_code = 0;
            utilityviewmodel.autoswitch_tariff_code = 0;
            utilityviewmodel.autoswitch_email_sent = SmartParametersV2016.defaultDate;


            utilityviewmodel.bill_date = SmartParametersV2016.defaultDate;                      // 35
            utilityviewmodel.statement_id = "";                 // 36  Usually a date ..
            utilityviewmodel.bill_version = 0;                           // 37  Start off with version set to earliest

            utilityviewmodel.Hezbollah.bills_row = new SmartUtility.Bills();                    // 38
            utilityviewmodel.Hezbollah.bills_resource_row = new SmartUtility.BillsResource();  // 39

            ourviewmodel.errorMessage = "";              // 74
            // Remaining fields may be 'null' but all of the field names should be in UPPERCASE
            // (because they are set during a pdf decode)
            utilityviewmodel.guid = guid;

            ourviewmodel.TargetUrl = new Uri(SmartParametersV2016.localWebsite);
            utilityviewmodel.current_routine = "";
            utilityviewmodel.next_routine = "LOGIN";
            //utilityviewmodel.http_message = "";
            utilityviewmodel.logout_pathname = "";
            utilityviewmodel.current_page = 0;           // Only used for NPower at the moment ..

            return;
        }

        internal static void Do_AmeliaLists(UtilityViewModel utilityviewmodel)
        {
            // Build Amelia list for resources we might find
            utilityviewmodel.Hezbollah.ameliaList = new List<Amelia>();
            return;
        }

        //internal static void MES_Do_HamasLists_Part1(UtilityViewModel utilityviewmodel,
        //                                            MainViewModel ourviewmodel)
        //{

        //    // Build Amelia list for resources we might find
        //    utilityviewmodel.ameliaList = new List<Amelia>();

        //    // Build a 'local' version of Utility.Accounts table in order to check
        //    // whether the get any new SmartUtility.Accounts
        //    ourviewmodel.Hamas.consumersChangesList = new List<SmartUsers.Consumers>();

        //    ourviewmodel.Hamas.addresses_changesList = new List<SmartUsers.Addresses>();
        //    ourviewmodel.Hamas.cubefacesChangesList = new List<SmartProfile.Cubefaces>();
        //    utilityviewmodel.bank_details_changesList = new List<SmartUtility.BankDetails>();

        //    return;
        //}



        internal static void MES_Do_HamasLists_Part2(UtilityViewModel utilityviewmodel)
        //rf short[] last_usage_index,
        //rf DateTime[] last_usage_date)
        {
            //utilityviewmodel.Hezbollah.account_charges_credits_inList = utilityviewmodel.Hezbollah.account_charges_creditsList;
            //utilityviewmodel.Hezbollah.supply_charges_credits_inList = utilityviewmodel.Hezbollah.supply_charges_creditsList;

            //utilityviewmodel.Hezbollah.supply_charges_credits_changesList = new List<SmartUtility.SupChargesCredits>();
            //utilityviewmodel.Hezbollah.account_charges_credits_changesList = new List<SmartUtility.AccChargesCredits>();

            // Build a 'local' version of Bills table in order to check whether the Bills have already been processed
            //utilityviewmodel.Hezbollah.bills_inList = utilityviewmodel.Hezbollah.billsList;
            //utilityviewmodel.Hezbollah.bills_resource_inList = utilityviewmodel.Hezbollah.bills_resourceList;
            //utilityviewmodel.Hezbollah.bills_changesList = new List<SmartUtility.Bills>();
            //ut//ilityviewmodel.Hezbollah.bills_resource_changesList = new List<SmartUtility.BillsResource>();
            utilityviewmodel.Hezbollah.bills_tempList = new List<SmartUtility.Bills>();
            utilityviewmodel.Hezbollah.bills_resource_tempList = new List<SmartUtility.BillsResource>();

            //utilityviewmodel.Hezbollah.payments_inList = utilityviewmodel.Hezbollah.paymentsList;
            //utilityviewmodel.Hezbollah.payments_changesList = new List<SmartUtility.Payments>();

            // May also contain last Payment_Item for last Payment_Date for each Account which
            // may or may not contain a Bill Number
            //utilityviewmodel.Hezbollah.unallocated_inList = utilityviewmodel.Hezbollah.unallocatedList;
            //utilityviewmodel.Hezbollah.unallocated_changesList = new List<SmartUtility.Unallocated>();

            //utilityviewmodel.Hezbollah.e_readings_changesList = new List<SmartUtility.EReadings>();
            //utilityviewmodel.Hezbollah.e_standing_charges_changesList = new List<SmartUtility.EStandingCharges>();
            //utilityviewmodel.Hezbollah.e_unit_charges_changesList = new List<SmartUtility.EUnitCharges>();
            //utilityviewmodel.Hezbollah.e_discounts_changesList = new List<SmartUtility.EDiscounts>();
            if (utilityviewmodel.Hezbollah.e_usageList.Count > 0)
            {
                utilityviewmodel.scraperlast_usage_index[0] = utilityviewmodel.Hezbollah.e_usageList.Last().UNIQUE_INDEX;
                utilityviewmodel.scraperlast_usage_date[0] = utilityviewmodel.Hezbollah.e_usageList.Last().USAGE_DATETIME;
            }
            else
            {
                utilityviewmodel.scraperlast_usage_index[0] = 0;
                utilityviewmodel.scraperlast_usage_date[0] = SmartParametersV2016.defaultDate;
            }
            //utilityviewmodel.Hezbollah.e_usage_changesList = new List<SmartUtility.EUsage>();
            //utilityviewmodel.Hezbollah.e_costsList = new List<SmartUtility.ECosts>();

            //utilityviewmodel.Hezbollah.g_readings_changesList = new List<SmartUtility.GReadings>();
            //utilityviewmodel.Hezbollah.g_standing_charges_changesList = new List<SmartUtility.GStandingCharges>();
            //utilityviewmodel.Hezbollah.g_unit_charges_changesList = new List<SmartUtility.GUnitCharges>();
            //utilityviewmodel.Hezbollah.g_discounts_changesList = new List<SmartUtility.GDiscounts>();
            // Only read the first to get the internal table column names
            if (utilityviewmodel.Hezbollah.g_usageList.Count > 0)
            {
                utilityviewmodel.scraperlast_usage_index[1] = utilityviewmodel.Hezbollah.g_usageList.Last().UNIQUE_INDEX;
                utilityviewmodel.scraperlast_usage_date[1] = utilityviewmodel.Hezbollah.g_usageList.Last().USAGE_DATETIME;
            }
            else
            {
                utilityviewmodel.scraperlast_usage_index[1] = 0;
                utilityviewmodel.scraperlast_usage_date[1] = SmartParametersV2016.defaultDate;
            }
            //utilityviewmodel.Hezbollah.g_usage_changesList = new List<SmartUtility.GUsage>();
            //utilityviewmodel.Hezbollah.g_costsList = new List<SmartUtility.GCosts>();

            //utilityviewmodel.Hezbollah.tariff_details_changesList = new List<SmartUtility.TariffDetails>();
            return;
        }

        internal static void MES_Do_HezbollahLists(MainViewModel ourviewmodel,
                                            UtilityViewModel utilityviewmodel,
                                            short brand_code)
        {
            // Build a 'local' version of Tariffs table
            // However - tariffs are attached to Suppliers (and separated by the
            // Tariff Area Matrix) so we need to find the Supplier attached to the 'brand'
            utilityviewmodel.tariff_codes_names_subsetList = SmartSpikeUtilityV2017.Utility_TCNSubsetList(ourviewmodel,
                                                                                                utilityviewmodel,
                                                                                                brand_code);
            //utilityviewmodel.Hezbollah.tariff_codes_names_tempList = new List<SmartUtility.TariffCodesNamesView>();

            // These three are built outside and passed in
            //utilityviewmodel.conditions_plansList = Hezbollah.conditions_plansList;
            //utilityviewmodel.payment_plansList = utilityviewmodel.Hezbollah.payment_plansList;
            //utilityviewmodel.payment_methodsList = utilityviewmodel.Hezbollah.payment_methodsList;

            //utilityviewmodel.utility_typesList = utilityviewmodel.Hezbollah.utility_typesList;
            //utilityviewmodel.Hezbollah.supplier_typesList = utilityviewmodel.Hezbollah.supplier_typesList;

            return;
        }

        // We always do a scrape on a SUPPLIER, never on a BRAND
        [RequiresUnreferencedCode("Calls SmartCubeMobile.FirstUtilityV2016.Read_Meter(AppCompatActivity, MainViewModel, UtilityViewModel, Boolean)")]
        internal static async Task<bool> Do_The_Reads(
#if WINFORMS
                                                    //System.Windows.Forms.WebBrowser Scraper,
#endif
#if ANDROIDX
                                                    AppCompatActivity meterActivity,
#endif
                                                    MainViewModel ourviewmodel,
                                                    UtilityViewModel utilityviewmodel,
                                                    short supplier_code,
                                                    bool TextBox_Active)
        {
            bool result = false;

            //            Dictionary<short, TimeSpan,
            //                                utilityviewmodel,
            //                                bool,
            //                                TextBlock,
            //                                ScrollViewer>> functions =
            //                         new Dictionary<short,
            //                                TimeSpan,
            //                                utilityviewmodel,
            //                                bool,
            //                                TextBlock,
            //                                ScrollViewer>>();

            //            Dictionary<short, Task<bool>> tasks =
            //                         new Dictionary<short, Task<bool>>();

            //            tasks[77] = await BritishGas_V2016.Read_Meter();
            //#endif
            //            Task abc = tasks[77](utcOffset,
            //                                                    MES,
            //                                                    TextBox_Active,
            //                                                    textBoxBrowser,
            //                                                    scrollViewer);

            //            functions["subtract"] = this.subtract;

            switch (supplier_code)
            {
                case 77:

                    result = await FirstUtilityV2016.Read_Meter(
#if WINFORMS
                                                    //Scraper,
#endif
#if ANDROIDX
                                                    meterActivity,
#endif

                                                    ourviewmodel,
                                                    utilityviewmodel,
                                                    TextBox_Active);


                    break;
                case 05:
                    // Do not fuck with this ... it WORKS.  It may not be brilliant
                    // OR what you would have coded OR even pleasing to your eye OR EVEN effiient -
                    // But it FUCKING WORKS
                    // ====================
                    // This idiot is SO FULL OF HERSELF.  Who cares about her
                    // fucking stupid boring irrelevant life. 'Just in case'
                    // oh FUCK OFF
                    // Back onto herself again - SO conceited, SO up her own arse!!
                    // She just can't see how boring she is!!
                    // Shouting down the telephone as usual ...
                    //
                    // Do not fuck with this ... it WORKS.  It may not be brilliant
                    // OR what you would have coded OR even pleasing to your eye OR EVEN effiient -
                    // But it FUCKING WORKS
                    // ====================

                    result = await BritishGasV2016.Read_Meter(
#if WINFORMS
                                                    //Scraper,
#endif
#if ANDROIDX
                                                    meterActivity,
#endif
                                                    ourviewmodel,
                                                    utilityviewmodel,
                                                    TextBox_Active);

                    break;
                case 40:

                    result = await NPowerV2016.Read_Meter(
#if WINFORMS
                                                    //Scraper,
#endif
#if ANDROIDX
                                                    meterActivity,
#endif
                                                    ourviewmodel,
                                                    utilityviewmodel,
                                                    TextBox_Active);

                    break;
                case 45:

                    result = await EdFV2016.Read_Meter(
#if WINFORMS
                                                    //Scraper,
#endif
#if ANDROIDX
                                                    meterActivity,
#endif
                                                    ourviewmodel,
                                                    utilityviewmodel,
                                                    TextBox_Active);

                    break;

                case 39:

                    result = await EOnV2016.Read_Meter(
#if WINFORMS
                                                    //Scraper,
#endif
#if ANDROIDX
                                                    meterActivity,
#endif

                                                    ourviewmodel,
                                                    utilityviewmodel,
                                                    TextBox_Active);

                    break;

                case 21:
                    // For the avoidance of doubt:
                    // ScottishPower are cunts.  Their website does not display the
                    // unit rates that a Customer (such as Anna) pays.  therfore,
                    // it is impossible to work out from theit bills (which are also
                    // fucking unreadable AND don't give the proper breakdown of rates
                    //  EITHER) exactly how much is being paid.  So £total_cost , when
                    // returned from build_tariff_engine will always be £0.0.  We
                    // therfore have to use a brilliant product like SmartSwitch
                    // to work out - WHAT THE COST WOULD HAVE BEEN (which is totally
                    // different from what Anna has paid btw) and display that.
                    // therfore the Meter text value shown will always match the
                    // figure in the Costs grid where Scottish Power is concerned.
                    // We will always find a cheaper supplier, though ('cos SmartSwitch
                    // IS brilliant!!)

                    result = await ScottishPowerV2016.Read_Meter(
#if WINFORMS
                                                    //Scraper,
#endif
#if ANDROIDX
                                                    meterActivity,
#endif

                                                    ourviewmodel,
                                                    utilityviewmodel,
                                                    TextBox_Active);

                    break;

                case 24:
                // Scottish Hydro
                case 26:
                // SWALEC
                case 43:
                // Southern Electric
                case 81:
                    // SSE

                    result = await SSEV2016.Read_Meter(
#if WINFORMS
                                                    //Scraper,
#endif
#if ANDROIDX
                                                    meterActivity,
#endif

                                                    ourviewmodel,
                                                    utilityviewmodel,
                                                    TextBox_Active);

                    break;
                default:
                    // Unsupported
                    break;
            }
            return result;
        }

        //internal static string Build_String<T>(FieldInfo[] myFields, T target_row, string yymmdd_format)
        //{
        //    //FieldInfo[] myFields = typeof(T).GetFields(SmartParametersV2016.bindingFlags);

        //    string row_as_string = "";
        //    for (int i = 0; i < myFields.Length; i++)
        //    {
        //        // Filter out the Username (its 'set' on the other side in DbServer
        //        if (myFields[i].Name != "USERNAME")
        //        {
        //            if (!string.IsNullOrEmpty(row_as_string))
        //            {
        //                row_as_string = row_as_string + ",";
        //            }
        //            if (myFields[i].FieldType.FullName == "System.DateTime")
        //            {
        //                // This SHOULD be in the right format ..
        //                DateTime ray_date = (DateTime)myFields[i].GetValue(target_row);
        //                row_as_string = row_as_string + ray_date.ToString(yymmdd_format);
        //            }
        //            else
        //            {
        //                row_as_string = row_as_string + myFields[i].GetValue(target_row);
        //            }
        //        }
        //    }
        //    return row_as_string;
        //}

        //internal static string Build_String_packed<T>(FieldInfo[] myFields, T target_row, string yymmdd_format, rf string last_date)
        //{
        //    //FieldInfo[] myFields = typeof(T).GetFields(SmartParametersV2016.bindingFlags);

        //    string  this_usage_datetime = "",
        //            this_date = "",
        //            this_time = "";
        //    string row_as_string = "";
        //    for (int i = 0; i < myFields.Length; i++)
        //    {
        //        if (!string.IsNullOrEmpty(row_as_string))
        //        {
        //            row_as_string = row_as_string + ",";
        //        }
        //        if (myFields[i].FieldType.FullName == "System.DateTime")
        //        {
        //            // This SHOULD be in the right format ..
        //            DateTime ray_date = (DateTime)myFields[i].GetValue(target_row);
        //            this_usage_datetime = ray_date.ToString(yymmdd_format);
        //            int space = this_usage_datetime.IndexOf(space);
        //            if (space >= 0)
        //            {
        //                this_date = this_usage_datetime .Substring(0, space);
        //                this_time = this_usage_datetime .Substring(space, this_usage_datetime .Length - space);
        //                //this_time = this_time.Replace(":00.000", "");
        //            }
        //            if (this_date == last_date)
        //            {
        //                row_as_string = row_as_string + "=" + this_time;
        //            }
        //            else
        //            {
        //                row_as_string = row_as_string + this_date + this_time;
        //            }
        //            last_date = this_date;
        //        }
        //        else
        //        {
        //            // Does tha last_row contain something?
        //            row_as_string = row_as_string + myFields[i].GetValue(target_row).ToString();
        //        }
        //    }
        //    return row_as_string;
        //}

        internal static async Task<bool> Pass_1_And_2(
                                        MainViewModel ourviewmodel,
                                        UtilityViewModel utilityviewmodel,
                                        DateTime time_now,
#if WINFORMS
                                        System.Windows.Forms.CheckBox checkBoxInsert_SmartSwitch,
#endif
                                        char last_resource_code,
                                        bool submit_button)
        //DateTime EFirstDate,
        //DateTime ELastDate,
        //decimal EFirstRead,
        //decimal ELastRead,
        //DateTime GFirstDate,
        //DateTime GLastDate,
        //decimal GFirstRead,
        //decimal GLastRead)
        {
            utilityviewmodel.failure_message = "";

            // This fucking moron bitch **NEVER** shuts the fuck up.
            // Its one continual, continuous stream of fucking verbal diahorrea
            // Every fucking boring aspect of her stupid fucking trivial banal life
            // And its ROSAMUND pike you thick cow, not Rosalind Pike.
            // And its a QashQAI not a QUASI you thick moron
            //string space = SmartParametersV2016.space;
            //bool new_record = false; // Always assume we will find an existing record

            // These should be in 'old' / 'new' account order
            // We should get just ONE each of SmartParametersV2016.Electricity or 'G' ....
            List<SmartUtility.AmeliasView> amelia_descendingList = SmartSpikeUtilityV2017.Utility_AmeliaDesc();
            foreach (SmartUtility.AmeliasView amelia_row in amelia_descendingList)
            {
                // The only one we DON'T check is BANK_BRAND_CODE
                if ((amelia_row.CUBEFACE_CODE != SmartParametersV2016.Utility) ||
                    (amelia_row.RESOURCE_CODE == SmartParametersV2016.defaultResourceCode) ||
                    string.IsNullOrEmpty(amelia_row.RESOURCE_TYPE) ||
                    (amelia_row.SUPPLIER_CODE == 0) ||
                    (amelia_row.BRAND_CODE == 0) ||
                    (amelia_row.ACCOUNT_CREATED == SmartParametersV2016.defaultDate) ||
                    string.IsNullOrEmpty(amelia_row.ACCOUNT_NO) ||
                    string.IsNullOrEmpty(amelia_row.POSTCODE) ||
                    (amelia_row.AREA_CODE == 0) ||
                    string.IsNullOrEmpty(amelia_row.UDPRN) ||
                    string.IsNullOrEmpty(amelia_row.MPAN_MPRN))
                //string.IsNullOrEmpty(amelia_row.METER_SERIAL_NO)) Not having this ISN'T a killer 'cos its not a key
                //                                                  The reason its not here is because its not lookup
                //                                                  looked up by Powershop AND its not in MIDATA and
                //                                                  we couldn't find it on any Bills!  So we have tried
                //                                                  hard and there is EVERY chance we can scrape it and
                //                                                  fill it up when things get good
                {
                    utilityviewmodel.failure_message = "Pass 1 problem: " + amelia_row.CUBEFACE_CODE + SmartParametersV2016.space +
                                                            amelia_row.RESOURCE_CODE + SmartParametersV2016.space +
                                                            amelia_row.RESOURCE_TYPE + SmartParametersV2016.space +
                                                            amelia_row.SUPPLIER_CODE + SmartParametersV2016.space +
                                                            amelia_row.BRAND_CODE + SmartParametersV2016.space +
                                                            amelia_row.ACCOUNT_NO + SmartParametersV2016.space +
                                                            amelia_row.ACCOUNT_CREATED.ToString("dd-MMM-yyyy") + SmartParametersV2016.space +
                                                            amelia_row.POSTCODE + SmartParametersV2016.space +
                                                            amelia_row.AREA_CODE.ToString() + SmartParametersV2016.space +
                                                            amelia_row.UDPRN + SmartParametersV2016.space +
                                                            amelia_row.MPAN_MPRN + SmartParametersV2016.space +
                                                            amelia_row.METER_SERIAL_NO;
                    // No BANK_BRAND_CODE
                    goto quit;
                }


                // Always assume the record is there
                //new_record = false;
                if ((last_resource_code == ' ') ||
                    (last_resource_code != amelia_row.RESOURCE_CODE))
                {
                    // Should be unique Energy Code with most recent Account No

                    utilityviewmodel.scraperlast_display = "";
                    Check_Submit_Button(utilityviewmodel,
                                        submit_button,
                                        last_resource_code);
                    //amelia_row,
                    //next_resources_row,
                    //rf last_display,
                    //utilityviewmodel);


                    // Consumer row has been updated successfully
                    // Only do the Switch Over if we came in from a SUBMIT
                    if (submit_button)
                    {
                        // There are 3 scenarios:
                        // 1    E    <blank>  <blank>
                        //      G    <blank>  <blank>
                        //
                        // 2    E     X        X
                        //      G    <blank>  <blank>
                        //    or
                        //      E    <blank>  <blank>
                        //      G     X        X
                        //
                        // 3    E     X        X
                        //      G              X
                        //    or
                        //      E              X
                        //      G     X        X
                        //


                        // So ... we are going to make the FIRST
                        // round the loop in E => G order (if there
                        // are two) the X X and the second round
                        // the loop (if there are two) the X one
                        // Make everything else <blank> even if it
                        // is THIS one we are just about to make
                        // the best
                        utilityviewmodel.resource_code = amelia_row.RESOURCE_CODE;
                        utilityviewmodel.last_update = time_now;
                        utilityviewmodel.next_connection = utilityviewmodel.last_update.AddDays(1);
                        utilityviewmodel.last_display = utilityviewmodel.scraperlast_display;
                        //SmartUtilityV2022.Utility_Check_Everything(MES, ourviewmodel, utilityviewmodel);
                    }
                    else
                    {

                        if (Nailed_It(last_resource_code, amelia_row.RESOURCE_CODE))
                        {
                            // Should be unique Energy Code with most recent Account No  
                            SmartUtility.Resources resources_row = SmartSpikeUtilityV2017.Utility_Extract_MatchingSingleResource(ourviewmodel,
                                                                                                                            utilityviewmodel,
                                                                                                                            //amelia_row.USERNAME,
                                                                                                                            amelia_row.SUPPLIER_CODE,
                                                                                                                            amelia_row.BRAND_CODE,
                                                                                                                            amelia_row.ACCOUNT_CREATED,
                                                                                                                            amelia_row.ACCOUNT_NO,
                                                                                                                            amelia_row.MPAN_MPRN);

                            // Should only be ONE?? - Yes we break at the end of this loop
                            string startup_udprn = "";

                            //// Update the last of the Resource figures 
                            //if (amelia_row.STATUS != 'N')
                            //{
                            //    amelia_row.NEXT_CONNECTION = SmartTimeV2016.ConvertDateTime(SmartParametersV2016.maximum_date); // defaultDate;
                            //                                                                                        //date_set = true;
                            //}
                            //else
                            //{
                            //    amelia_row.NEXT_CONNECTION = next_conn;
                            //}
                            utilityviewmodel.resource_code = amelia_row.RESOURCE_CODE;       // Important key!!
                            utilityviewmodel.next_connection = amelia_row.NEXT_CONNECTION;
                            //SmartUtilityV2022.Utility_Check_Everything(MES, ourviewmodel, utilityviewmodel);
                            startup_udprn = amelia_row.UDPRN;

                            if (!string.IsNullOrEmpty(startup_udprn))
                            {
                                // I have no idea about this THIS HAS TO BE WRONG
                                List<SmartProfile.AddressesView> addresses_found = SmartSpikeUtilityV2017.Utility_Analyze_Addresses(ourviewmodel,
                                                                                                    utilityviewmodel);
                                //amelia_descendingList);
                                //utilityviewmodel.Hezbollah.utility_addressesList);
                                foreach (SmartProfile.AddressesView address_row in addresses_found)
                                {

                                    if (!SmartUtilityV2022.Build_Utility_UDPRN_Dropdown(ourviewmodel,
                                                                                    utilityviewmodel,
                                                                                    startup_udprn,
                                                                                    address_row.BASIC))
                                    {
                                        utilityviewmodel.UtilityAddressesList.Clear();
                                    }
                                    else
                                    {
                                        utilityviewmodel.AddressSelectedIndex = utilityviewmodel.udprn_index;
                                    }
                                }
                            }
                        }
                    }

                    switch (amelia_row.RESOURCE_CODE)
                    {
                        case SmartParametersV2016.Electricity:
                            // Do the e stuff
                            Find_E_Values(utilityviewmodel,
                                            amelia_row.CUBEFACE_CODE,
                                            amelia_row.MPAN_MPRN,
                                            SmartParametersV2016.defaultDate);
                            //rf EFirstDate,
                            //rf ELastDate,
                            //rf EFirstRead,
                            //rf ELastRead);
                            break;
                        case SmartParametersV2016.Gas:
                            // Do the e stuff
                            Find_G_Values(utilityviewmodel,
                                            amelia_row.CUBEFACE_CODE,
                                            amelia_row.MPAN_MPRN,
                                            SmartParametersV2016.defaultDate);
                            //rf GFirstDate,
                            //rf GLastDate,
                            //rf GFirstRead,
                            //rf GLastRead);
                            break;
                        default:
                            break;
                    }
                    last_resource_code = amelia_row.RESOURCE_CODE;
                }
            }

            // This is where we update Resource records

            if (!await Pass2(ourviewmodel,
#if WINFORMS
                                   checkBoxInsert_SmartSwitch,
#endif
                                   utilityviewmodel))

            {
                utilityviewmodel.failure_message = "Pass 2 failed " + utilityviewmodel.failure_message;
            }

        quit:
            if (!string.IsNullOrEmpty(utilityviewmodel.failure_message))
            {
                return false;
            }
            // RE-CONSTITUTE THE VIEW HERE!!
            return true;
        }

        internal static void Check_Submit_Button(UtilityViewModel utilityviewmodel,
                                        bool submit_button,
                                        char last_resource_code)
        //SmartUtility.ConsumersView amelia_row,
        //rf string last_display)
        {
            if (submit_button)
            {
                char radio_button_resource_code = SmartUtilityV2022.Utility_Find_Resource_Code(utilityviewmodel);

                if (utilityviewmodel.resource_code == radio_button_resource_code)
                {
                    // Fix *this one* as 'the one' - this ONLY WORKS if one of the Elec or Gas buttons is CHECKED!!!
                    utilityviewmodel.scraperlast_display = SmartParametersV2016.lastChecked;
                }
                else
                {
                    // Fix *this one* as 'the other one!'
                    if ((radio_button_resource_code == ' ') &&
                        (last_resource_code == ' '))
                    {
                        // Nothing is checked - we are at the refresh of the Universe (which wasn't created
                        // by a 'big bang' after all, but is cyclical and is forever with time
                        // going forward (as the Universe expands) and going backwards (as the Universe contracts)
                        // because TIME has a MASS, which gives 'Gravity' its ability to 'push'.  For the Universe
                        // to exist - you only need TIME (which has a mass) and MASS itself. These are 'must haves'!!
                        // Everything else - particles, matter, w.h.y are 'nice to haves'
                        //
                        // Are we the 'last one' of the loop?
                        utilityviewmodel.scraperlast_display = SmartParametersV2016.lastChecked;
                    }
                }
            }
            return;
        }

        //internal static Resources Desperation(DateTime time_now,
        //                                                char cubeface_code,
        //                                                DateTime[] expiration,
        //                                                UtilityViewModel utilityviewmodel,
        //                                                Amelia amelia_row,
        //                                                List<Consumers_View> consumers_viewList,
        //                                                List<SmartUtility.Resources> resourcesList,
        //                                                rf bool new_record,
        //                                                rf short variation)
        //{
        //    // The UDPRN **ALWAYS** contains something! Its a MUST HAVE
        //    // Sometimes the UDPRN might be 'Unknown' if we can't find it or
        //    // its the first time through and there are no Bills at all ...
        //    // But its **NEVER** the account number ...

        //    Resources resources_row = new Resources();
        //    List<SmartUtility.Resources> resources_found = (from Resource in resourcesList
        //                                       where ((Resource.CUBEFACE_CODE == cubeface_code) &&
        //                                               (Resource.MPAN_MPRN == amelia_row.MPAN_MPRN))
        //                                       select Resource);

        //    variation = 1;
        //    List<SmartUtility.Resources> resources_found1 = (from Resource in resources_found    // <= Note the difference
        //                                        where ((Resource.CUBEFACE_CODE == amelia_row.CUBEFACE_CODE) &&
        //                                                (Resource.SUPPLIER_CODE == amelia_row.SUPPLIER_CODE) &&
        //                                                (Resource.BRAND_CODE == amelia_row.BRAND_CODE) &&
        //                                                (Resource.MPAN_MPRN == amelia_row.MPAN_MPRN))
        //                                        select Resource);
        //    if (resources_found1.Count > 0)
        //    {
        //        resources_row = resources_found1[0];
        //        return resources_row;
        //    }

        //    variation = 2;
        //    List<SmartUtility.Resources> resources_found2 = (from Resource in resources_found    // <= Not the difference
        //                                        where ((Resource.CUBEFACE_CODE == amelia_row.CUBEFACE_CODE) &&
        //                                                (Resource.MPAN_MPRN == amelia_row.MPAN_MPRN))
        //                                        orderby Resource.LAST_UPDATE descending
        //                                        select Resource);
        //    if (resources_found2.Count > 0)
        //    {
        //        // Well we found a record ... but does the Supplier Code match?
        //        // If the number found is > 1 then there are two many Suppliers (with the
        //        // matching UDPRN to decide which is the current ..
        //        if ((amelia_row.SUPPLIER_CODE == resources_found2[0].SUPPLIER_CODE) &&
        //            (amelia_row.BRAND_CODE == resources_found2[0].BRAND_CODE))
        //        {
        //            resources_row = resources_found2[0];
        //            return resources_row;
        //        }
        //        goto new_record;
        //    }

        //    // Get any Account matching Resource
        //    variation = 3;
        //    // We assume now that the Amelia / ACCOUNT_NO are entered in ascending order of Bill Date
        //    // so earlier rows will refer to earlier Bills (and possibly earlier ACCOUNT_NOs) whilst
        //    // later Bills will refer to newer or updated ACCOUNT_NOs.  See PAT's Npower Bills and
        //    // how the account_no changed 
        //    List<SmartUtility.Resources> resources_found3 = (from Resource in resources_found    // <= Note the difference
        //                                        where ((Resource.CUBEFACE_CODE == amelia_row.CUBEFACE_CODE) &&
        //                                                (Resource.SUPPLIER_CODE == amelia_row.SUPPLIER_CODE) &&
        //                                                (Resource.BRAND_CODE == amelia_row.BRAND_CODE))
        //                                        select Resource);
        //    // Has this failed??
        //    if (resources_found3.Count > 0)
        //    {
        //        resources_row = resources_found3[resources_found3.Count - 1]; // Get the last (Amelia / ACCOUNT_NO
        //        return resources_row;
        //    }

        //    // How do we do VARIATION 4 now??
        //    variation = 4;
        //    List<SmartUtility.Resources> resources_found4 = (from Resource in resources_found    //  <= Note the difference
        //                                        where ((Resource.CUBEFACE_CODE == amelia_row.CUBEFACE_CODE) &&
        //                                            (Resource.SUPPLIER_CODE == amelia_row.SUPPLIER_CODE) &&
        //                                            (Resource.BRAND_CODE == amelia_row.BRAND_CODE)) // &&
        //                                                                                            //      (Consumer_Vie.User_Id == utilityviewmodel.user_id))
        //                                        select Resource);
        //    if (resources_found4.Count > 0)
        //    {
        //        resources_row = resources_found4[0];
        //        return resources_row;
        //    }

        //    variation = 5;
        //    new_record:
        //    resources_row = Setup_Resource_Record(time_now,
        //                                            amelia_row,
        //                                            MES,
        //                                            expiration,
        //                                            rf variation,
        //                                            rf new_record);
        //    return resources_row;
        //}


        //internal static Resources Desperation_Newer(DateTime time_now,
        //                                                char cubeface_code,
        //                                                DateTime[] expiration,
        //                                                UtilityViewModel utilityviewmodel,
        //                                                Amelia amelia_row,
        //                                                List<SmartUtility.Resources> resourcesList,
        //                                                rf bool new_record,
        //                                                rf short variation)
        //{
        //    // The UDPRN **ALWAYS** contains something! Its a MUST HAVE
        //    // Sometimes the UDPRN might be 'Unknown' if we can't find it or
        //    // its the first time through and there are no Bills at all ...
        //    // But its **NEVER** the account number ...

        //    Resources resources_row = new Resources();
        //    List<SmartUtility.Resources> resources_found = (from Resource in resourcesList
        //                                        where ((Resource.CUBEFACE_CODE == cubeface_code) &&
        //                                                (Resource.MPAN_MPRN == amelia_row.MPAN_MPRN))
        //                                        select Resource);

        //    variation = 1;
        //    List<SmartUtility.Resources> resources_found1 = (from Resource in resources_found    // <= Note the difference
        //                                        where ((Resource.CUBEFACE_CODE == amelia_row.CUBEFACE_CODE) &&
        //                                                (Resource.SUPPLIER_CODE == amelia_row.SUPPLIER_CODE) &&
        //                                                (Resource.BRAND_CODE == amelia_row.BRAND_CODE) &&
        //                                                (Resource.MPAN_MPRN == amelia_row.MPAN_MPRN))
        //                                        select Resource);
        //    if (resources_found1.Count > 0)
        //    {
        //        resources_row = resources_found1[0];
        //        return resources_row;
        //    }

        //    variation = 2;
        //    List<SmartUtility.Resources> resources_found2 = (from Resource in resources_found    // <= Not the difference
        //                                        where ((Resource.CUBEFACE_CODE == amelia_row.CUBEFACE_CODE) &&
        //                                                (Resource.MPAN_MPRN == amelia_row.MPAN_MPRN))
        //                                        orderby Resource.LAST_UPDATE descending
        //                                        select Resource);
        //    if (resources_found2.Count > 0)
        //    {
        //        // Well we found a record ... but does the Supplier Code match?
        //        // If the number found is > 1 then there are two many Suppliers (with the
        //        // matching UDPRN to decide which is the current ..
        //        if ((amelia_row.SUPPLIER_CODE == resources_found2[0].SUPPLIER_CODE) &&
        //            (amelia_row.BRAND_CODE == resources_found2[0].BRAND_CODE))
        //        {
        //            resources_row = resources_found2[0];
        //            return resources_row;
        //        }
        //        goto new_record;
        //    }

        //    // Get any Account matching Resource
        //    variation = 3;
        //    // We assume now that the Amelia / ACCOUNT_NO are entered in ascending order of Bill Date
        //    // so earlier rows will refer to earlier Bills (and possibly earlier ACCOUNT_NOs) whilst
        //    // later Bills will refer to newer or updated ACCOUNT_NOs.  See PAT's Npower Bills and
        //    // how the account_no changed 
        //    List<SmartUtility.Resources> resources_found3 = (from Resource in resources_found    // <= Note the difference
        //                                        where ((Resource.CUBEFACE_CODE == amelia_row.CUBEFACE_CODE) &&
        //                                                (Resource.SUPPLIER_CODE == amelia_row.SUPPLIER_CODE) &&
        //                                                (Resource.BRAND_CODE == amelia_row.BRAND_CODE))
        //                                        select Resource).ToList();
        //    // Has this failed??
        //    if (resources_found3.Count > 0)
        //    {
        //        resources_row = resources_found3[resources_found3.Count - 1]; // Get the last (Amelia / ACCOUNT_NO
        //        return resources_row;
        //    }

        //    // How do we do VARIATION 4 now??
        //    variation = 4;
        //    List<SmartUtility.Resources> resources_found4 = (from Resource in resources_found    //  <= Note the difference
        //                                        where ((Resource.CUBEFACE_CODE == amelia_row.CUBEFACE_CODE) &&
        //                                            (Resource.SUPPLIER_CODE == amelia_row.SUPPLIER_CODE) &&
        //                                            (Resource.BRAND_CODE == amelia_row.BRAND_CODE)) // &&
        //                                          //      (Consumer_Vie.User_Id == utilityviewmodel.user_id))
        //                                        select Resource).ToList();
        //    if (resources_found4.Count > 0)
        //    {
        //        resources_row = resources_found4[0];
        //        return resources_row;
        //    }

        //    variation = 5;
        //    new_record:
        //    resources_row = Setup_Resource_Record(time_now,
        //                                            amelia_row,
        //                                            MES,
        //                                            expiration,
        //                                            rf variation,
        //                                            rf new_record);
        //    return resources_row;
        //}

        //internal static Consumers Setup_Consumer_Record(Amelia amelia_row,
        //                                                UtilityViewModel utilityviewmodel,
        //                                                rf short variation,
        //                                                rf bool new_record)
        //{
        //    variation = 0;

        //    Consumers ray_row = new Consumers()
        //    {
        //        // Key starts here
        //        CUBEFACE_CODE = amelia_row.CUBEFACE_CODE,
        //        SUPPLIER_CODE = amelia_row.SUPPLIER_CODE,
        //        BRAND_CODE = amelia_row.BRAND_CODE,
        //        CREATED = amelia_row.CREATED,
        //        User_Id = utilityviewmodel.user_id,
        //        Password = utilityviewmodel.password
        //    };
        //    new_record = true;
        //    return ray_row;
        //}

        internal static bool Nailed_It(char last_resource_code, char resource_code)
        {
            if ((last_resource_code == SmartParametersV2016.defaultResourceCode) ||
                    (last_resource_code != resource_code))
            {
                return true;
            }
            return false;
        }

        internal static async Task<bool> General_Stuff(
#if WINFORMS
                                                    System.Windows.Forms.CheckBox checkBoxInsert_SmartSwitch,
#endif
                                                    MainViewModel ourviewmodel,
                                                    UtilityViewModel utilityviewmodel,
                                                    List<SmartUsers.VatRates> vatRatesList,
                                                    List<SmartUsers.ExchangeRates> exchangeRatesList,
                                                    DateTime time_now,
                                                    bool submit_button)
        {
            // This is SO MUCH BETTER than before -
            //      More readable
            //      More logical
            //      A lot faster
            //      More efficient
            //      More debuggable
            //      More supportable
            //      10 / 10 Ray, you are a fucking ACE programmer

            utilityviewmodel.EFirstDate = SmartParametersV2016.defaultDate;
            utilityviewmodel.ELastDate = SmartParametersV2016.defaultDate;
            utilityviewmodel.GFirstDate = SmartParametersV2016.defaultDate;
            utilityviewmodel.GLastDate = SmartParametersV2016.defaultDate;
            utilityviewmodel.EFirstRead = 0.0M;
            utilityviewmodel.ELastRead = 0.0M;
            utilityviewmodel.GFirstRead = 0.0M;
            utilityviewmodel.GLastRead = 0.0M;

            char last_resource_code = SmartParametersV2016.defaultResourceCode;

            utilityviewmodel.failure_message = "";

            //
            // PASS 1 - Make Sure we have a Resource record and it gets updated
            //

            if (!await Pass_1_And_2(ourviewmodel,
                                        utilityviewmodel,
                                        time_now,
#if WINFORMS
                                         checkBoxInsert_SmartSwitch,
#endif
                                        last_resource_code,
                                        submit_button))
            //EFirstDate,
            //ELastDate,
            //EFirstRead,
            //ELastRead,
            //GFirstDate,
            //GLastDate,
            //GFirstRead,
            //GLastRead))


            {
                goto quit;
            }
            //
            // PASS 3 - Make Sure we build the Tariff Engine and Analyze just once
            //
            List<SmartUtility.Accounts> accounts_found =
            SmartSpikeUtilityV2017.Utility_Lookup_Accounts(ourviewmodel,
                                                        utilityviewmodel,
                                                        SmartParametersV2016.defaultDate);
            if (accounts_found.Count == 0)
            {
                utilityviewmodel.failure_message = "Amelia desc empty Problem";
                goto quit;
            }

            //
            // Consumer View List will have been rebuilt here with NEXT_CONNECTION updated
            //
            foreach (SmartUtility.Accounts accounts_row in accounts_found)
            {
                if (Nailed_It(last_resource_code, accounts_row.RESOURCE_CODE))
                {
                    // Should be unique Energy Code with most recent Account No  
                    //Resources resources_row = SmartSpikeUtilityV2017.Extract_Matching_Single_Resource(amelia_row, Hamas.resourcesList);

                    // Should only be ONE?? - Yes we break at the end of this loop
                    // Assume the default()
                    int age = 0;

                    utilityviewmodel.Hezbollah.working_billsList = SmartUtilityV2022.Working_BillsX(ourviewmodel,
                                                                                    utilityviewmodel,
                                                                                    SmartParametersV2016.defaultDate); // Remove is true here
                                                                                                                       // Change it to E7??
                                                                                                                       // Rebuild the Engine
                                                                                                                       // Username is inserted at DBServer end - possibly ... but not for this SELECT!

                    List<SmartUtility.ResourcesTypes> rt_found =
                   SmartSpikeUtilityV2017.Utility_Find_ResourcesTypes(ourviewmodel,
                                                                           utilityviewmodel,
                                                                           accounts_found.First().RESOURCE_CODE);
                    utilityviewmodel.resource_type = rt_found.First().RESOURCE_TYPE; // Will be (should be) SR or VR

                    //List<SmartUtility.Meters> mt_found =
                    //SmartSpikeUtilityV2017.Utility_Lookup_Meters(ourviewmodel,
                    //                                    utilityviewmodel,
                    //                                    accounts_row.RESOURCE_CODE,
                    //                                    "");
                    //if (mt_found.Count > 0)
                    //{
                    List<SmartProfile.AddressesView> ad_found =
                        SmartSpikeV2017.Users_Lookup_AddressesUDPRN(ourviewmodel,
                                                            ourviewmodel.UserName,
                                                            accounts_row.UDPRN);
                    if (ad_found.Count > 0)
                    {
                        // Should NEVER be zero!!
                        //utilityviewmodel.area_code = ad_found.First().AREA_CODE;
                    }

                    // The ORDER of these parameters is VERY IMPORTANT


                    string load_unit_rates = SmartParametersV2016.Utility.ToString() + SmartParametersV2016.unitSeparator +
                                                utilityviewmodel.area_code.ToString() + SmartParametersV2016.unitSeparator +
                                                accounts_row.RESOURCE_CODE.ToString() + SmartParametersV2016.unitSeparator +
                                                utilityviewmodel.resource_type.ToString() + SmartParametersV2016.unitSeparator +
                                                utilityviewmodel.withdrawn_date.ToString(SmartParametersV2016.sqliteformat);

                    TimeSpan ts = new TimeSpan(12, 0, 0);
                    DateTime engine_from_date = time_now.Date.Add(ts);

                    if (!await Do_Main_Engine(ourviewmodel,
                                            utilityviewmodel,
                                            load_unit_rates,
                                            engine_from_date,
                                            age,
                                            utilityviewmodel.area_code,
                                            accounts_row.RESOURCE_CODE,
                                            utilityviewmodel.resource_type,
                                            accounts_row.SUPPLIER_CODE,
                                            accounts_row.BRAND_CODE,
                                            utilityviewmodel.VOLUMECORRECTION,
                                            utilityviewmodel.KWHCONVERSION))
                    {
                        goto quit;
                    }
                }
                last_resource_code = accounts_row.RESOURCE_CODE;
            }

        quit:
            if (!string.IsNullOrEmpty(utilityviewmodel.failure_message))
            {
                if (utilityviewmodel.examine.scrape)
                {
                    if (!await SmartRoutinesV2018.CheckTrace(ourviewmodel, utilityviewmodel.utilityToken, utilityviewmodel.supplier_code, utilityviewmodel.brand_code, SmartParametersV2016.Utility.ToString() + utilityviewmodel.failure_message))
                    {
                        return false;
                    }
                }
                return false;
            }
            return true;
        }

        internal static async Task<bool> Do_Main_Engine(MainViewModel ourviewmodel,
                                                        UtilityViewModel utilityviewmodel,
                                                        string load_unit_rates,
                                                        DateTime engine_from_date,
                                                        int age,
                                                        short area_code,
                                                        char resource_code,
                                                        string resource_type,
                                                        short supplier_code,
                                                        short brand_code,
                                                        string VOLUMECORRECTION,
                                                        string KWHCONVERSION)
        {
            bool status = true;

            // Rebuild the Engine
            utilityviewmodel.firstReading = 0.0M;
            utilityviewmodel.totalReadingsTemp = 0.0M;
            utilityviewmodel.lastReading = 0.0M;

            short target_supplier_code = 0;             // For debugging
            int target_tariff_code = 0;                 // For debugging
            char target_payment_plan = SmartParametersV2016.defaultChar;  // For debugging

            DateTime previous_withdrawn_date = utilityviewmodel.withdrawn_date; // SmartParametersV2016.defaultDate;

            //Resources resources_row = get_consumer_energy();

            switch (resource_code)
            {
                case SmartParametersV2016.Electricity:
                    utilityviewmodel.e_engine_dates[0] = engine_from_date;
                    utilityviewmodel.e_engine_dates[1] = engine_from_date;
                    break;
                case SmartParametersV2016.Gas:
                    utilityviewmodel.g_engine_dates[0] = engine_from_date;
                    utilityviewmodel.g_engine_dates[1] = engine_from_date;
                    break;
                default:
                    break;
            }
            utilityviewmodel.EngineToDate = engine_from_date;
            if (!SmartEngineV2016.Build_Tariff_EngineList(utilityviewmodel,
                                                            resource_code,
                                                            engine_from_date,
                                                            VOLUMECORRECTION,
                                                            KWHCONVERSION))
            {
                utilityviewmodel.failure_message = "Build_Tariff_Engine Problem58: " + resource_code;
                status = false;
                goto quit;
            }

            Do_Engine_Dates(utilityviewmodel,
                            resource_code);
            //rf EngineToDate);
            switch (resource_code)
            {
                case SmartParametersV2016.Electricity:
                    utilityviewmodel.e_engine_dates[0] = engine_from_date;
                    utilityviewmodel.e_engine_dates[1] = utilityviewmodel.EngineToDate;
                    break;

                case SmartParametersV2016.Gas:
                    utilityviewmodel.g_engine_dates[0] = engine_from_date;
                    utilityviewmodel.g_engine_dates[1] = utilityviewmodel.EngineToDate;
                    break;
                default:
                    break;
            }
            // Store the Total Readings
            //resources_row.FIRST_READING = first_reading;
            //resources_row.LAST_READING = last_reading;
            //resources_row.TOTAL_READING = total_readings;
            //resources_row.TOTAL_COST = total_cost;

            // Store the Engine dates
            //resources_row.FROM_DATETIME = engine_from_date;
            //resources_row.TO_DATETIME = EngineToDate;

            // Hmmm ... in THEORY, once we have got the Unit Rates for the Area Id
            // in question (i.e from DISPLAY_METER) then that Area Id shouldn't change
            // as we don't support Usernames where people move from Area to Area i.e
            // once you are in an area (for Electricity and/or Gas) then .. you 'stay'
            // in that Area.  So er should only ever load up the Unit Rates for an Area
            // in THIS routine - i.e. the Rows are 0 IF we came in from a Submit
            switch (resource_code)
            {
                case SmartParametersV2016.Electricity:
                    if (utilityviewmodel.Hezbollah.e_unit_ratesList.Count == 0)
                    {
                        // We don't have any so go and get some from SmartDBServer
                        // The GUI should **NEVER** have to worry about the Schema - SmartDBServer should sort that out

                        utilityviewmodel.Hezbollah.e_unit_ratesList = await SmartBobV2017.Load_SingleList_Async<SmartUtility.UnitRates>(ourviewmodel,
                                                                                                            utilityviewmodel,
                                                                                                            utilityviewmodel.utilityToken,
                                                                                                            "SMARTUTILITY",
                                                                                                            "LOAD_UNIT_RATES",
                                                                                                            "P",
                                                                                                            load_unit_rates);

                        if (!string.IsNullOrEmpty(ourviewmodel.errorMessage))
                        {
                            return false;
                        }
                    }
                    break;
                case SmartParametersV2016.Gas:
                    if (utilityviewmodel.Hezbollah.g_unit_ratesList.Count == 0)
                    {
                        // We don't have any so go and get some from SmartDBServer
                        // The GUI should **NEVER** have to worry about the Schema - SmartDBServer should sort that out

                        utilityviewmodel.Hezbollah.g_unit_ratesList = await SmartBobV2017.Load_SingleList_Async<SmartUtility.UnitRates>(ourviewmodel,
                                                                                                            utilityviewmodel,
                                                                                                            utilityviewmodel.utilityToken,
                                                                                                            "SMARTUTILITY",
                                                                                                            "LOAD_UNIT_RATES",
                                                                                                            "P",
                                                                                                            load_unit_rates);


                        if (!string.IsNullOrEmpty(ourviewmodel.errorMessage))
                        {
                            return false;
                        }
                    }
                    break;
                default:
                    break;
            }
            // Check to see if new date is EARLIER?? Shouldn't ever happen
            if (SmartRoutinesV2018.DateTimeCompare(utilityviewmodel.withdrawn_date, previous_withdrawn_date) < 0)
            {
                utilityviewmodel.withdrawn_date = previous_withdrawn_date; // Shouldn't ever happen??
            }

            if ((resource_code == SmartParametersV2016.Electricity &&
                utilityviewmodel.Hezbollah.e_unit_ratesList.Count > 0) ||
                (resource_code == SmartParametersV2016.Gas &&
                utilityviewmodel.Hezbollah.g_unit_ratesList.Count > 0))
            {

                List<SmartUtility.AnalysisConditions> already_doneList = new List<SmartUtility.AnalysisConditions>();
                if (!await SmartAnalyzeV2016.Analyze_Costs(ourviewmodel,
                                                            utilityviewmodel,
                                                            true,     // Will delete from AC
                                                            area_code,
                                                            resource_code,
                                                            resource_type,
                                                            brand_code,
                                                            supplier_code, // Supplier
                                                            0,                      // Supplier we might choose (wildcard or non-wildcard)
                                                            0,                      // Brand
                                                            0,                      // Tariff we might choose (wildcard or non-wildcard)
                                                            SmartParametersV2016.defaultChar,           // Payment Plan we might choose              
                                                            target_supplier_code,
                                                            target_tariff_code,
                                                            target_payment_plan,
                                                            engine_from_date,
                                                            age,
                                                            utilityviewmodel.withdrawn_date,
                                                            already_doneList))

                {
                    utilityviewmodel.failure_message = "Analyze_Costs Problem59:" + resource_code;
                    status = false;
                    goto quit;
                }
            }
        //set_consumer_energy(resources_row);
        quit:
            return status;
        }

        internal static void Do_Engine_Dates(UtilityViewModel utilityviewmodel,
                                            char resource_code)
        //rf DateTime EngineToDate)
        {
            int last_row_count;// = 0;
            switch (resource_code)
            {
                case SmartParametersV2016.Electricity:
                    if (utilityviewmodel.Hezbollah.e_tariff_engineList.Count > 0)
                    {
                        last_row_count = utilityviewmodel.Hezbollah.e_tariff_engineList.Count - 1;
                        utilityviewmodel.EngineToDate = utilityviewmodel.Hezbollah.e_tariff_engineList[last_row_count].PERIOD_END;
                    }
                    break;
                case SmartParametersV2016.Gas:
                    if (utilityviewmodel.Hezbollah.g_tariff_engineList.Count > 0)
                    {
                        last_row_count = utilityviewmodel.Hezbollah.g_tariff_engineList.Count - 1;
                        utilityviewmodel.EngineToDate = utilityviewmodel.Hezbollah.g_tariff_engineList[last_row_count].PERIOD_END;
                    }
                    break;
                default:
                    break;
            }
            return;
        }

        internal static async Task<bool> Pass2(MainViewModel ourviewmodel,
#if WINFORMS
                                                System.Windows.Forms.CheckBox checkBoxInsert_SmartSwitch,
#endif
                                                UtilityViewModel utilityviewmodel)
        {
            // These should be in 'old' / 'new' account order
            //List<SmartUtility.ConsumersView> amelia_ascending = 

            // I am not sure if this logic change fucks everything up??
            List<SmartUtility.AmeliasView> av_found =
                new List<SmartUtility.AmeliasView>();
            // Now do all the Utility.Accounts etc which may be needed for keys?
            //
            // So we can store it in the fucking SQL Server Express database in
            // fucking stupid US "English" format
            //
#if WINFORMS
            if (checkBoxInsert_SmartSwitch.Checked)
            {
#endif
            string urgent_message = "";

            if (!await SmartUtilityScrapeV2022.DoAllSmartUtility(ourviewmodel,
                                                        utilityviewmodel,
                                                        true,
                                                        SmartParametersV2016.sqliteformat))
            // Delete all unallocated payments is passed in as 'true'
            {
                utilityviewmodel.failure_message = "DoAllSmartSwitch Problem " + urgent_message;
                return false;
            }
#if WINFORMS
            }
#endif
            return true;
        }

        internal static void Find_E_Values(UtilityViewModel utilityviewmodel,
                                        char cubeface_code,
                                        string mpan_mprn,
                                        DateTime defaultDate)
        //rf DateTime first_date,
        //rf DateTime last_date,
        //rf decimal first_read,
        //rf decimal last_read)
        {
            DateTime readinGFirstDate = defaultDate,
                     readinGLastDate = defaultDate;
            decimal readinGFirstRead = 0.0M,
                    readinGLastRead = 0.0M;
            if (utilityviewmodel.Hezbollah.e_readingsList.Count > 0)
            {
                // Across ALL Suppliers, Brands and Utility.Accounts ... but only THIS MPAN!!
                List<SmartUtility.EReadings> e_readings_found =
                    new List<SmartUtility.EReadings>(from E_Reading
                    in utilityviewmodel.Hezbollah.e_readingsList
                                                     where E_Reading.CUBEFACE_CODE == cubeface_code &&
                                                         E_Reading.MPAN_MPRN == mpan_mprn
                                                     orderby E_Reading.READINGS_PERIOD_END descending
                                                     select E_Reading);
                if (e_readings_found.Count > 0)
                {
                    int index = e_readings_found.Count - 1;
                    readinGLastRead = e_readings_found[index].D_UNITS_USED; // This is a decimal
                    readinGLastDate = e_readings_found[index].READINGS_PERIOD_END;
                }

                // These don't come back in PERIOD_END order ...
                //foreach (E_Readings e_readings_row in e_readings_found)
                //{
                //    Let duplicate readings bounce off as
                //    sqlexceptions in Insert_Common.  This is a kludge
                //    and I know it.  However ... there will be very few 'normal'
                //    duplicate readings (i.e. when the same one appears on two
                //    different bills) and all the readings returned from
                //    Scottish Power because I cannot read their bills.  I don't
                //    expect this to last forever!  ONE DAY!! SCottish Power will
                //    have readable bills, and I can get more or less unique
                //    readings every time and I won't be trapping exceptions
                //    Live with it Ray, you have done wonders so far ...
                Set_E_Values(utilityviewmodel,
                                defaultDate,
                                readinGFirstDate,
                                readinGLastDate,
                                readinGFirstRead,
                                readinGLastRead);
                //rf first_date,
                //rf last_date,
                //rf first_read,
                //rf last_read);
            }

            if (utilityviewmodel.Hezbollah.e_usageList.Count > 0)
            {
                SmartUtility.EUsage last_row;
                DateTime this_datetime = defaultDate;
                decimal this_reading = 0.0M;
                int count;
                bool fix_first_read = false;

                DateTime usagEFirstDate = defaultDate,
                        usagELastDate = defaultDate;
                decimal usagEFirstRead = 0.0M,
                        usagELastRead = 0.0M;

                // Trim off the last reading if its 0  this is because sometimes The supplier
                // hasn't updated the reading for the previous day, and as we record units AND dates
                // of reading them, there is a chance we would miss a 'true' value by recording a 'false' 0.00 instead
                count = utilityviewmodel.Hezbollah.e_usageList.Count;
                while (count > 0)
                {
                    last_row = utilityviewmodel.Hezbollah.e_usageList.Last();
                    if (last_row.USAGE_TOTAL == 0)
                    {
                        count--;
                    }
                    else
                    {
                        break;
                    }
                }

                foreach (SmartUtility.EUsage e_usage_row in utilityviewmodel.Hezbollah.e_usageList)
                {
                    if (count == 0)
                    {
                        break;
                    }
                    // The reading comes in as Wh so we must divide by 1000 to give Kwh
                    this_reading = e_usage_row.USAGE_TOTAL;
                    this_datetime = e_usage_row.USAGE_DATETIME;
                    if (fix_first_read == false)
                    {
                        usagEFirstRead = this_reading;
                        usagEFirstDate = this_datetime;
                        fix_first_read = true;
                    }
                    count--;
                    usagELastRead = this_reading;
                    usagELastDate = this_datetime;
                }
                Set_E_Values(utilityviewmodel,
                                defaultDate,
                                usagEFirstDate,
                                usagELastDate,
                                usagEFirstRead,
                                usagELastRead);
                //rf first_date,
                //rf last_date,
                //rf first_read,
                //rf last_read);
            }
            return;
        }

        internal static void Set_E_Values(UtilityViewModel utilityviewmodel,
                                        DateTime defaultDate,
                                        DateTime comparison_first_date,
                                        DateTime comparison_last_date,
                                        decimal comparison_first_read,
                                        decimal comparison_last_read)
        //rf DateTime first_date,
        //rf DateTime last_date,
        //rf decimal first_read,
        //rf decimal last_read)
        {
            if ((utilityviewmodel.EFirstDate == defaultDate) ||
                (SmartRoutinesV2018.DateTimeCompare(comparison_first_date, utilityviewmodel.EFirstDate) < 0))
            {
                utilityviewmodel.EFirstRead = comparison_first_read;
                utilityviewmodel.EFirstDate = comparison_first_date;
            }
            if ((utilityviewmodel.ELastDate == defaultDate) ||
                (SmartRoutinesV2018.DateTimeCompare(comparison_last_date, utilityviewmodel.ELastDate) > 0))
            {
                utilityviewmodel.ELastRead = comparison_last_read;
                utilityviewmodel.ELastDate = comparison_last_date;
            }
            return;
        }

        internal static void Find_G_Values(UtilityViewModel utilityviewmodel,
                                    char cubeface_code,
                                    string mpan_mprn,
                                    DateTime defaultDate)
        //rf DateTime first_date,
        //rf DateTime last_date,
        //rf decimal first_read,
        //rf decimal last_read)
        {
            DateTime readinGFirstDate = defaultDate,
                     readinGLastDate = defaultDate;
            decimal readinGFirstRead = 0.0M,
                    readinGLastRead = 0.0M;
            if (utilityviewmodel.Hezbollah.g_readingsList.Count > 0)
            {
                // Across ALL Suppliers, Brands and Utility.Accounts ... but only THIS MPRN!!
                // These don't come back in PERIOD_END order ...
                List<SmartUtility.GReadings> g_readings_found = new List<SmartUtility.GReadings>(from G_Reading
                                                    in utilityviewmodel.Hezbollah.g_readingsList
                                                                                                 where G_Reading.CUBEFACE_CODE == cubeface_code &&
                                                                                                        G_Reading.MPAN_MPRN == mpan_mprn
                                                                                                 orderby G_Reading.READINGS_PERIOD_END descending
                                                                                                 select G_Reading);
                if (g_readings_found.Count > 0)
                {
                    int index = g_readings_found.Count - 1;
                    readinGLastRead = g_readings_found[index].D_UNITS_USED_M3; // This is a decimal
                    readinGLastDate = g_readings_found[index].READINGS_PERIOD_END;
                }
                Set_G_Values(utilityviewmodel,
                                defaultDate,
                                readinGFirstDate,
                                readinGLastDate,
                                readinGFirstRead,
                                readinGLastRead);
                //rf first_date,
                //rf last_date,
                //rf first_read,
                //rf last_read);
            }

            if (utilityviewmodel.Hezbollah.g_usageList.Count > 0)
            {
                // Store away everything from g_usageList
                SmartUtility.GUsage last_row;
                DateTime this_datetime = defaultDate;
                decimal this_reading = 0.0M;
                int count;
                bool fix_first_read = false;

                DateTime usagEFirstDate = defaultDate,
                        usagELastDate = defaultDate;
                decimal usagEFirstRead = 0.0M,
                        usagELastRead = 0.0M;

                // Trim off the last reading if its 0.00  this is because sometimes The supplier
                // hasn't updated the reading for the previous day, and as we record units AND dates
                // of reading them, there is a chance we would miss a 'true' value by recording a 'false' 0.00 instead
                count = utilityviewmodel.Hezbollah.g_usageList.Count;
                while (count > 0)
                {
                    last_row = utilityviewmodel.Hezbollah.g_usageList.Last();
                    if (last_row.USAGE_TOTAL == 0M)
                    {
                        count--;
                    }
                    else
                    {
                        break;
                    }
                }
                //FieldInfo[] myFields = typeof(G_Usage).GetFields(SmartParametersV2016.bindingFlags);
                foreach (SmartUtility.GUsage g_usage_row in utilityviewmodel.Hezbollah.g_usageList)
                {
                    if (count == 0)
                    {
                        break;
                    }
                    //                          3
                    // The reading comes in as M  which needs to be convert to Kwh
                    // .. but at least it is to 3 decimal places
                    this_reading = g_usage_row.USAGE_TOTAL;
                    this_datetime = g_usage_row.USAGE_DATETIME;
                    if (fix_first_read == false)
                    {
                        usagEFirstRead = this_reading;
                        usagEFirstDate = this_datetime;
                        fix_first_read = true;
                    }
                    count--;
                    usagELastRead = this_reading;
                    usagELastDate = this_datetime;
                }
                Set_G_Values(utilityviewmodel,
                                defaultDate,
                                usagEFirstDate,
                                usagELastDate,
                                usagEFirstRead,
                                usagELastRead);
                //rf first_date,
                //rf last_date,
                //rf first_read,
                //rf last_read);
            }
        }

        internal static void Set_G_Values(UtilityViewModel utilityviewmodel,
                                        DateTime defaultDate,
                                        DateTime comparison_first_date,
                                        DateTime comparison_last_date,
                                        decimal comparison_first_read,
                                        decimal comparison_last_read)
        //rf DateTime first_date,
        //rf DateTime last_date,
        //rf decimal first_read,
        //rf decimal last_read)
        {
            if ((utilityviewmodel.GFirstDate == defaultDate) ||
                (SmartRoutinesV2018.DateTimeCompare(comparison_first_date, utilityviewmodel.GFirstDate) < 0))
            {
                utilityviewmodel.GFirstRead = comparison_first_read;
                utilityviewmodel.GFirstDate = comparison_first_date;
            }
            if ((utilityviewmodel.GLastDate == defaultDate) ||
                (SmartRoutinesV2018.DateTimeCompare(comparison_last_date, utilityviewmodel.GLastDate) > 0))
            {
                utilityviewmodel.GLastRead = comparison_last_read;
                utilityviewmodel.GLastDate = comparison_last_date;
            }
        }

        //internal static void Fix_First_Last(rf SmartUtility.Resources resources_row,
        //                                    DateTime last_date)
        //{
        //    //if (resources_row.FIRST_DATETIME == defaultDate)
        //    //{
        //    //    resources_row.FIRST_DATETIME = first_date;
        //    //    resources_row.FIRST_READING = first_read;
        //    //}
        //    if (SmartRoutinesV2018.DateTimeCompare(last_date, resources_row.LAST_DATETIME) > 0)
        //    {
        //        resources_row.LAST_DATETIME = last_date;
        //        //resources_row.LAST_READING = last_read;
        //    }
        //}

        //internal static void Fix_Next_Connection(
        //                                        char resource_code,     // AMELIA resource_code
        //                                        List<Consumers_View> consumers_viewList, //Resources resources_row,
        //                                        DateTime defaultDate,
        //                                        DateTime next_conn)
        //{
        //    // NOT SURE ABOUT THIS ROUTINE

        //    //char resource_code = consumer_row.RESOURCE_CODE;
        //    //string UDPRN = resources_row.UDPRN;
        //    //short   brand_code = resources_row.BRAND_CODE;
        //    //bool date_set = false;
        //    //List<SmartUtility.Accounts> accounts_found = SmartSpikeUtilityV2017.Lookup_Utility.Accounts(resources_row.BRAND_CODE,
        //    //                                                                    utilityviewmodel.consumers_inList,
        //    //                                                                    utilityviewmodel.Utility.Accounts_changesList);


        //     //   (from Utility.Accounts
        //     //                               in utilityviewmodel.Utility.Accounts_changesList
        //     //                                where Utility.Accounts.UDPRN == UDPRN && //Utility.Accounts.RESOURCE_CODE == resource_code &&
        //     //                                      Utility.Accounts.SUPPLIER_CODE == brand_code
        //     //                                orderby Utility.Accounts.CREATED descending
        //     //                                select Utility.Accounts).ToList();
        //    //if (accounts_found.Count == 0)
        //    //{
        //    //    SmartSpikeUtilityV2017.Lookup_Utility.Accounts(resources_row.BRAND_CODE,
        //    //                                    utilityviewmodel.consumers_inList,
        //    //                                    utilityviewmodel.Utility.Accounts_inList);
        //        //accounts_found = (from Utility.Accounts
        //        //                  in utilityviewmodel.Utility.Accounts_inList
        //        //                  where Utility.Accounts.UDPRN == UDPRN && //Utility.Accounts.RESOURCE_CODE == resource_code &&
        //        //                        Utility.Accounts.SUPPLIER_CODE == brand_code
        //        //                  orderby Utility.Accounts.CREATED descending
        //        //                  select Utility.Accounts).ToList();
        //    //}
        //    //if (accounts_found.Count > 0)
        //    //{


        //    // SHUT THE FUCK UP
        //    // SHUT UP
        //    // SHUT THE FUCKING FUCK UP
        //    // YOU ARE A **CONSTANT** FUCKING DISTRACTION
        //    // FUCKING SHUT FUCKING UP
        //    // =======================
        //    foreach (Consumers_View consumers_view_row in consumers_viewList)
        //    {
        //        if (consumers_view_row.RESOURCE_CODE == resource_code)
        //        {
        //            if (consumers_view_row.CLOSED)
        //            {
        //                consumers_view_row.NEXT_CONNECTION = SmartTimeV2016.ConvertDateTime(SmartParametersV2016.maximum_date); // defaultDate;
        //                //date_set = true;
        //            }
        //            else
        //            {
        //                consumers_view_row.NEXT_CONNECTION = next_conn;
        //            }
        //            SmartNibbyV2016.Check_Everything(MES, Hamas);
        //        }
        //        //break;  // only first
        //    }
        //    //}
        //    //if (!date_set)
        //    //{
        //    //    // Account ISN'T closed or
        //    //    // No Utility.Accounts!  Can't check status ...
        //    //    // need to set this anyway
        //    //    resources_row.NEXT_CONNECTION = next_conn;
        //    //}
        //}

        internal static void Do_The_Bills(UtilityViewModel utilityviewmodel)
        {
            // Only add it in if we successfully created it
            if (utilityviewmodel.bills_out)
            {
                utilityviewmodel.Hezbollah.bills_changesList.Add(utilityviewmodel.Hezbollah.bills_row);
            }
            if (utilityviewmodel.bills_resource_out)
            {
                utilityviewmodel.Hezbollah.bills_resource_changesList.Add(utilityviewmodel.Hezbollah.bills_resource_row);
            }
            return;
        }
    }
}