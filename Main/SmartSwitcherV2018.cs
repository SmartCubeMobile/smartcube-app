using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;


#if WINFORMS
using SmartDashboard;
#endif

#if ANDROIDX
using AndroidX.AppCompat.App;
#endif
namespace SmartCubeMobile
{
    public class SmartSwitcherV2018
    {
        //
        // All these calls happen when *NOT* logged-in!
        //
        internal static async Task<bool> Utility_Check_Switch(
#if ANDROIDX
                                            AppCompatActivity meterActivity,
#endif
                                            MainViewModel ourviewmodel,
                                            UtilityViewModel utilityviewmodel,
                                            char resource_code,
                                            SmartUtility.AnalysisCostsView analysis_costs_row,
                                            SmartUtility.SwitchInfo RAY,
                                            Action<string> set_err_message)
        {
            int switch_count = 0;
            string report = "";
            StringBuilder bollocks = new StringBuilder();
            List<SmartProfile.Profiles> details_found = new List<SmartProfile.Profiles>();

            bool switch_possible = false;

            List<SmartUtility.Resources> resources_found =
                SmartSpikeUtilityV2017.UtilityFindResourcesList(ourviewmodel,
                                                                 utilityviewmodel,
                                                                 SmartParametersV2016.defaultResourceCode);
            foreach (SmartUtility.Resources consumer_row in resources_found)
            {
                char local_resource_code = consumer_row.RESOURCE_CODE;
                if (((local_resource_code == resource_code) ||
                     (resource_code == SmartParametersV2016.defaultResourceCode)) &&
                    (consumer_row.AUTOSWITCH == SmartParametersV2016.activeFlag))
                {
                    // Re-do this RAY!!!
                    // Does the Resource entry permit a switch?

                    //if (string.IsNullOrEmpty(consumer_row.AUTOSWITCH_DESTINATION) &&
                    //        (consumer_row.AUTOSWITCH_SUPPLIER_CODE == 0) &&
                    //        (consumer_row.AUTOSWITCH_BRAND_CODE == 0) &&
                    //        (consumer_row.AUTOSWITCH_TARIFF_CODE == 0))
                    //{
                    //    switch_possible = true;
                    //}
                }
            }

            if (switch_possible)
            {
                //List<SmartUtility.ConsumersView> reducedList = SmartSpikeUtilityV2017.Reduced_Accounts(ourviewmodel,
                //                                                                                        utilityviewmodel);
                //if (reducedList.Count > 1)
                //{
                details_found = SmartSpikeUtilityV2017.Utility_Find_PersonalDetails(ourviewmodel);
                                                                            //SmartParametersV2016.Utility);
                if (details_found.Count > 0)
                {
                    if (string.IsNullOrEmpty(details_found[0].LASTNAME) ||
                        string.IsNullOrEmpty(details_found[0].EMAIL_ADDRESS) ||
                        string.IsNullOrEmpty(details_found[0].CONTACT_NO) ||
                        string.IsNullOrEmpty(details_found[0].DATE_OF_BIRTH))
                    {
                        switch_possible = false;
                    }
                }
                //}
            }

            if (switch_possible)
            {
                List<SmartUtility.Accounts> accounts_found = SmartSpikeUtilityV2017.Utility_Lookup_Resources(ourviewmodel,
                                                                                                                            utilityviewmodel,
                                                                                                                            SmartParametersV2016.defaultResourceCode);

                foreach (SmartUtility.Accounts consumerview_row in accounts_found)
                {
                    string BRAND_NAME = SmartSpikeUtilityV2017.Utility_Lookup_SupplierName(ourviewmodel,
                                                    utilityviewmodel,
                                                    consumerview_row.SUPPLIER_CODE,
                                                    consumerview_row.BRAND_CODE);
                    List<SmartUtility.ResourcesTypes> resourcestypes_found =
                        SmartSpikeUtilityV2017.Utility_Find_ResourcesTypes(ourviewmodel,
                                                                                utilityviewmodel,
                                                                                consumerview_row.RESOURCE_CODE);

                    if (resourcestypes_found.Count > 0)
                    {
                        string TARIFF_NAME = SmartSpikeUtilityV2017.Utility_Lookup_TariffName(utilityviewmodel,
                                                                        consumerview_row.RESOURCE_CODE,
                                                                        consumerview_row.SUPPLIER_CODE,
                                                                        consumerview_row.BRAND_CODE,
                                                                        resourcestypes_found[0].RESOURCE_TYPE,
                                                                        consumerview_row.TARIFF_CODE);
                        string PAYMENT_NAME = "";
                        string[] payment_names = new string[3] { "", "", "" };
                        payment_names = SmartSpikeUtilityV2017.Utility_Lookup_PaymentName(ourviewmodel,
                                                                            utilityviewmodel,

                                                                            consumerview_row.PAYMENT_PLAN);
                        foreach (string payment_name in payment_names)
                        {
                            if (!string.IsNullOrEmpty(payment_name))
                            {
                                PAYMENT_NAME = payment_name;
                                break;
                            }
                        }

                        if (string.IsNullOrEmpty(BRAND_NAME) ||
                            string.IsNullOrEmpty(TARIFF_NAME) ||
                            string.IsNullOrEmpty(PAYMENT_NAME))
                        {

                            //await SmartBobV2017.ListenerAsync(SignIn.signinviewmodel.Username,timespanTimeout, cookies, username, website,
                            set_err_message(consumerview_row.BRAND_CODE.ToString() + SmartParametersV2016.space +
                                            consumerview_row.SUPPLIER_CODE.ToString() + SmartParametersV2016.space +
                                            "Switch failed because " +
                                            BRAND_NAME + "/" + TARIFF_NAME + "/" + PAYMENT_NAME);
                            return false;
                        }
                        else
                        {
                            List<SmartUtility.Suppliers> suppliers_found = SmartSpikeUtilityV2017.Utility_Just_Lookup_Supplier(ourviewmodel,
                                                                                    utilityviewmodel,
                                                                                    consumerview_row.RESOURCE_CODE,
                                                                                    analysis_costs_row.SUPPLIER_CODE);
                            if (suppliers_found.Count == 0)
                            {

                                //await SmartBobV2017.ListenerAsync(SignIn.signinviewmodel.Username,timespanTimeout, cookies, username, website,
                                set_err_message(consumerview_row.BRAND_CODE.ToString() + SmartParametersV2016.space +
                                            consumerview_row.SUPPLIER_CODE.ToString() + SmartParametersV2016.space +
                                            "Switch failed because Suppliers found = 0");
                                return false;
                            }
                            else
                            {
                                List<SmartProfile.AddressesView> addresses_found =
                                        SmartSpikeV2017.Users_Lookup_AddressesUDPRN(ourviewmodel,
                                                                                    ourviewmodel.UserName,
                                                                                    "123456");  // Fill this in later!!
                                if (addresses_found.Count == 1)
                                {
                                    //string title = "";
                                    string first_name = "";
                                    string last_name = "";

                                    List<SmartUtility.Accounts> utility_accounts_found = SmartSpikeUtilityV2017.Utility_Lookup_Accounts(ourviewmodel,
                                                                                                                    utilityviewmodel,
                                                                                                                    consumerview_row.ACCOUNT_CREATED,
                                                                                                                    consumerview_row.SUPPLIER_CODE,
                                                                                                                    consumerview_row.BRAND_CODE,
                                                                                                                    consumerview_row.ACCOUNT_NO);
                                    short payday = SmartParametersV2016.defaultPaymentDay;
                                    if (utility_accounts_found.Count > 0)
                                    {
                                        List<SmartUtility.BankDetails> bank_details_found = SmartSpikeUtilityV2017.Utility_Lookup_BankDetailsX(ourviewmodel,
                                                                                                                    utilityviewmodel,
                                                                                                                    consumerview_row.SUPPLIER_CODE,
                                                                                                                    consumerview_row.BRAND_CODE);
                                        if (bank_details_found.Count > 0)
                                        {
                                            payday = bank_details_found[0].BANK_PAYDAY;
                                        }

                                    }
                                    // They might genuinely have no bank account details
                                    if (utility_accounts_found.Count == 1)
                                    {
                                        if (!string.IsNullOrEmpty(details_found[0].LASTNAME))
                                        {
                                            string[] names = details_found[0].LASTNAME.Split(SmartParametersV2016.spaceSplit);
                                            int last = names.Length - 1;
                                            if (last >= 0)
                                            {
                                                last_name = names[last];
                                            }
                                            last--;
                                            if (last >= 0)
                                            {
                                                first_name = names[last];
                                            }
                                        }
                                    }

                                    RAY.postcode = addresses_found[0].POSTCODE;
                                    RAY.address = addresses_found[0].BASIC;
                                    //RAY.telephone = accounts_row.Spare3;
                                    //RAY.email = accounts_row.Spare4;
                                    RAY.telephone = details_found[0].CONTACT_NO;
                                    RAY.email = details_found[0].EMAIL_ADDRESS;
                                    RAY.title = "";
                                    RAY.first_name = first_name;
                                    RAY.last_name = last_name;
                                    short bank_institution_code = 0;
                                    short bank_brand_code = 0;
                                    //if (financeviewmodel.PLO.finance_accountsList != nll)
                                    //{
                                    //    foreach (SmartFinance.Accounts accounts_row in financeviewmodel.PLO.finance_accountsList)
                                    //    {
                                    //        if (accounts_row.ACCOUNT_NO == utility_accounts_found[0].ACCOUNT_NO)
                                    //        {
                                    //            bank_institution_code = accounts_row.INSTITUTION_CODE;
                                    //            bank_brand_code = accounts_row.BRAND_CODE;
                                    //            RAY.bank_account_name = accounts_row.TITLE;
                                    //            RAY.bank_account_sort_code = accounts_row.SORTCODE;
                                    //            RAY.bank_account_number = accounts_row.ACCOUNT_NO;
                                    //            break;
                                    //        }
                                    //    }
                                    //}
                                    if (bank_institution_code == 0 &&
                                        bank_brand_code == 0)
                                    {
                                        RAY.bank_account_name = details_found[0].LASTNAME; // accounts_row.Spare1;
                                    }
                                    if (RAY.bank_account_name.Length > SmartParametersV2016.maximumBankAccountName)
                                    {
                                        RAY.bank_account_name = RAY.bank_account_name.Substring(SmartParametersV2016.maximumBankAccountName);
                                    }
                                    //string[] split = finance_accounts_found[0].ACCOUNT_NO.Split(SmartParametersV2016.sortcode);
                                    //if (split.Count > 1)
                                    //{

                                    //}
                                    // Account name, sort code and account_no mught still be empty at this point
                                    RAY.bank_account_payday = payday; // == 0 ? SmartParametersV2016.defaultPaymentDay : accounts_row.Spare9;
                                    if (RAY.bank_account_payday == 0)
                                    {
                                        RAY.bank_account_payday = 1;
                                    }
                                    RAY.proposed_brand_name = analysis_costs_row.SUPPLIER_NAME;
                                    RAY.proposed_tariff_name = analysis_costs_row.TARIFF_NAME;
                                    RAY.proposed_supplier_code = analysis_costs_row.SUPPLIER_CODE;
                                    RAY.proposed_brand_code = analysis_costs_row.BRAND_CODE;
                                    RAY.proposed_tariff_code = analysis_costs_row.TARIFF_CODE;
                                    RAY.proposed_payment_plan = Convert.ToChar(analysis_costs_row.PAYMENT_PLAN);
                                    RAY.proposed_payment_name = analysis_costs_row.PAYMENT_NAME;
                                    RAY.url_prfix = suppliers_found[0].URL_SWITCH; // Usually energylinx?
                                    RAY.username = consumerview_row.USERNAME;
                                    RAY.cubeface_code = consumerview_row.CUBEFACE_CODE;
                                    RAY.present_supplier_code = consumerview_row.SUPPLIER_CODE;
                                    RAY.present_brand_code = consumerview_row.BRAND_CODE;
                                    RAY.present_tariff_code = consumerview_row.TARIFF_CODE;
                                    RAY.present_payment_plan = consumerview_row.PAYMENT_PLAN;
                                    RAY.present_resource_type = resourcestypes_found[0].RESOURCE_TYPE;
                                    RAY.created = consumerview_row.ACCOUNT_CREATED;
                                    RAY.account_no = consumerview_row.ACCOUNT_NO;
                                    RAY.udprn = addresses_found[0].UDPRN;
                                    // For DUAL FUEL you need BOTH of these filled!!
                                    string mpan_mprn = "",
                                            meter_serial_no = "",
                                            elec_account_no = "",
                                            elec_meter_type = "",
                                            gas_account_no = "";
                                    utilityviewmodel.e_readings_found = new List<SmartUtility.EReadings>();
                                    utilityviewmodel.g_readings_found = new List<SmartUtility.GReadings>();
                                    utilityviewmodel.d_readings_found = new List<SmartUtility.EReadings>();
                                    SmartSpikeUtilityV2017.Utility_Find_Readings(utilityviewmodel,
                                                                    consumerview_row.RESOURCE_CODE);
                                    string READINGS_PERIOD_END = "",
                                            d_this_read = "",
                                            n_this_read = "";
                                    int readings_count = 0;


                                    List<SmartUtility.Meters> meters_found =
                                        SmartSpikeUtilityV2017.Utility_Lookup_Meters(ourviewmodel,
                                                                            utilityviewmodel,
                                                                            resources_found[0].RESOURCE_CODE,
                                                                            //resourcestypes_found[0].RESOURCE_TYPE,
                                                                            "");
                                    switch (consumerview_row.RESOURCE_CODE)
                                    {
                                        case SmartParametersV2016.Electricity:
                                            readings_count = utilityviewmodel.e_readings_found.Count - 1;
                                            if (readings_count >= 0)
                                            {
                                                READINGS_PERIOD_END = utilityviewmodel.e_readings_found[readings_count].READINGS_PERIOD_END.ToString(SmartParametersV2016.ddmmmyyyyFormat);
                                                d_this_read = utilityviewmodel.e_readings_found[readings_count].D_THIS_READ.ToString();
                                                n_this_read = utilityviewmodel.e_readings_found[readings_count].N_THIS_READ.ToString();
                                            }

                                            if (meters_found.Count > 0)
                                            {
                                                RAY.mpan = meters_found[0].MPAN_MPRN;
                                                RAY.e_meter_serial_no = meters_found[0].METER_SERIAL_NO;
                                            }
                                            mpan_mprn = RAY.mpan;
                                            meter_serial_no = RAY.e_meter_serial_no;
                                            elec_account_no = consumerview_row.ACCOUNT_NO;
                                            switch (resourcestypes_found[0].RESOURCE_TYPE) //(accounts_row.RESOURCE_TYPE) ! No!!!
                                            {
                                                case "SR":
                                                    elec_meter_type = "Single-rate";
                                                    break;
                                                case "VR":
                                                    elec_meter_type = "Dual-rate (Economy7)";
                                                    break;
                                                default:
                                                    break;
                                            }
                                            break;
                                        case SmartParametersV2016.Gas:
                                            readings_count = utilityviewmodel.g_readings_found.Count - 1;
                                            if (readings_count >= 0)
                                            {
                                                READINGS_PERIOD_END = utilityviewmodel.g_readings_found[readings_count].READINGS_PERIOD_END.ToString(SmartParametersV2016.ddmmmyyyyFormat);
                                                d_this_read = utilityviewmodel.g_readings_found[readings_count].D_THIS_READ.ToString();
                                            }

                                            if (meters_found.Count > 0)
                                            {
                                                RAY.mprn = meters_found[0].MPAN_MPRN;
                                                RAY.g_meter_serial_no = meters_found[0].METER_SERIAL_NO;
                                            }
                                            mpan_mprn = RAY.mprn;
                                            meter_serial_no = RAY.g_meter_serial_no;
                                            gas_account_no = consumerview_row.ACCOUNT_NO;
                                            break;
                                        default:
                                            // Dual Switch
                                            break;
                                    }

                                    RAY.resource_code = consumerview_row.RESOURCE_CODE;
                                    RAY.resource_type = resourcestypes_found[0].RESOURCE_TYPE;
                                    RAY.new_account_password = ourviewmodel.UserName; //!!! should be consumerview_row.USER_PASSWORD;

                                    switch_count++;

                                    if (!string.IsNullOrEmpty(report))
                                    {
                                        report += Environment.NewLine;
                                    }
                                    report += SmartReportV2016.Fill_Email_Report(DateTime.Now + ourviewmodel.utcOffset, // Local time

                                                            resource_code,
                                                            RAY.proposed_brand_name,
                                                            RAY.proposed_tariff_name,
                                                            RAY.proposed_payment_name,
                                                            utilityviewmodel.Hezbollah.templateList,
                                                            details_found[0].LASTNAME,
                                                            RAY.address = addresses_found[0].BASIC,
                                                            RAY.postcode = addresses_found[0].POSTCODE,
                                                            details_found[0].CONTACT_NO,
                                                            details_found[0].EMAIL_ADDRESS,
                                                            BRAND_NAME,
                                                            TARIFF_NAME,
                                                            PAYMENT_NAME,
                                                            mpan_mprn,
                                                            elec_account_no,
                                                            elec_meter_type,
                                                            gas_account_no,
                                                            meter_serial_no,
                                                            READINGS_PERIOD_END,
                                                            d_this_read,
                                                            n_this_read,
                                                            SmartParametersV2016.adminEmailAddress); // "admin@Keasdon.co.uk";);

                                    // GetNextRandomNew()
                                    int randomkey = SmartRoutinesV2018.GetNextRandomNew(ourviewmodel.randomR);

                                    SmartUtility.Switches switches_row = new SmartUtility.Switches()
                                    {
                                        USERNAME = RAY.username,
                                        CUBEFACE_CODE = RAY.cubeface_code,
                                        SUPPLIER_CODE = RAY.proposed_supplier_code,
                                        BRAND_CODE = RAY.proposed_brand_code,
                                        RESOURCE_CODE = RAY.resource_code,
                                        SWITCH_CREATED = RAY.created,
                                        ACCOUNT_NO = RAY.account_no,
                                        MPAN_MPRN = RAY.mpan, //or mprn
                                                              // Some in the middle are left alone
                                                              //AUTOSWITCH = SmartParametersV2016.activeDefault, // Turn this OFF now
                                        AUTOSWITCH_DESTINATION = RAY.email,
                                        AUTOSWITCH_EMAIL_SENT = DateTime.Now + ourviewmodel.utcOffset,  // Local time
                                        AUTOSWITCH_SUPPLIER_CODE = analysis_costs_row.SUPPLIER_CODE,
                                        AUTOSWITCH_BRAND_CODE = analysis_costs_row.BRAND_CODE,
                                        AUTOSWITCH_TARIFF_CODE = analysis_costs_row.TARIFF_CODE,
                                        RANDOMKEY = randomkey,
                                        Updated = true              // Its a new record
                                    };
                                    utilityviewmodel.Hezbollah.utility_switches_changesList.Add(switches_row);

                                    // At about this point we CAN'T pass the login_info BACK because
                                    // this Popup has been called and the calling routine has moved on!
                                    // If Updated = true, then the record already exists
                                    // If Updated = false then we need to create a new record ..

                                    if (utilityviewmodel.Hezbollah.utility_switches_changesList.Count > 0)
                                    {
                                        // Record the switch
                                        SmartUtilityScrapeV2022.DoAllUtility_Switches(ourviewmodel,
                                                                                utilityviewmodel,
                                                                                SmartParametersV2016.sqliteformat);
                                        if (bollocks.Length == 0)
                                        {
                                            set_err_message("Problem 78:  DoAllSmartSwitches");
                                            return false;
                                        }
                                        else
                                        {
                                            if (switches_row.Updated == false)
                                            {
                                                // Its a new record
                                                SmartUtility.Switches switches_new = new SmartUtility.Switches()
                                                {
                                                    USERNAME = switches_row.USERNAME,
                                                    CUBEFACE_CODE = switches_row.CUBEFACE_CODE,
                                                    SUPPLIER_CODE = switches_row.SUPPLIER_CODE,
                                                    BRAND_CODE = switches_row.BRAND_CODE,
                                                    RESOURCE_CODE = switches_row.RESOURCE_CODE,
                                                    SWITCH_CREATED = switches_row.SWITCH_CREATED,
                                                    ACCOUNT_NO = switches_row.ACCOUNT_NO,
                                                    MPAN_MPRN = switches_row.MPAN_MPRN,
                                                    RANDOMKEY = switches_row.RANDOMKEY,
                                                    AGE_60PLUS = switches_row.AGE_60PLUS,
                                                    AUTOSWITCH_DESTINATION = switches_row.AUTOSWITCH_DESTINATION,
                                                    AUTOSWITCH_EMAIL_SENT = DateTime.Now + ourviewmodel.utcOffset,  // Local time
                                                    AUTOSWITCH_SUPPLIER_CODE = switches_row.AUTOSWITCH_SUPPLIER_CODE,
                                                    AUTOSWITCH_BRAND_CODE = switches_row.AUTOSWITCH_BRAND_CODE,
                                                    AUTOSWITCH_TARIFF_CODE = switches_row.AUTOSWITCH_TARIFF_CODE

                                                };
                                                utilityviewmodel.Hezbollah.utility_switchesList.Add(switches_new);
                                            }
                                            else
                                            {
                                                // Update the existing Login record
                                                foreach (SmartUtility.Switches switches in utilityviewmodel.Hezbollah.utility_switchesList)
                                                {
                                                    if (switches.USERNAME == RAY.username &&
                                                        switches.CUBEFACE_CODE == RAY.cubeface_code &&
                                                        switches.SUPPLIER_CODE == RAY.present_supplier_code &&
                                                        switches.BRAND_CODE == RAY.present_brand_code &&
                                                        //switches_row.SWITCHES_CREATED == RAY.SWITCHES_CREATED &&
                                                        switches.ACCOUNT_NO == RAY.account_no)
                                                    {
                                                        //switches.AUTOSWITCH = SmartParametersV2016.activeDefault;                           // Turn this OFF now
                                                        switches.AUTOSWITCH_DESTINATION = switches_row.AUTOSWITCH_DESTINATION;
                                                        switches.AUTOSWITCH_EMAIL_SENT = DateTime.Now + ourviewmodel.utcOffset;  // Local time
                                                        switches.AUTOSWITCH_SUPPLIER_CODE = switches_row.AUTOSWITCH_SUPPLIER_CODE;
                                                        switches.AUTOSWITCH_BRAND_CODE = switches_row.AUTOSWITCH_BRAND_CODE;
                                                        switches.AUTOSWITCH_TARIFF_CODE = switches_row.AUTOSWITCH_TARIFF_CODE;

                                                        break; // Keys should be unique
                                                    }
                                                }
                                            }
                                            
                                            // Tell the console we have switched
                                            await SmartRoutinesV2018.TextBlockUpdate(
#if ANDROIDX
                                                meterActivity,
#endif
                                                ourviewmodel, "Switch info changed for: ");// + oldlogin_info.BANK_NAME);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            if (switch_count > 0)
            {
                // Merge the two?
                utilityviewmodel.autoswitch_pending = true;
                utilityviewmodel.report = report;
                utilityviewmodel.bollocks = bollocks;
                return true;
            }
            return false;
        }

        internal static async Task<bool> General_Switcher(
#if ANDROIDX
                                                        AppCompatActivity meterActivity,
#endif
                                                        MainViewModel ourviewmodel,
                                                        UtilityViewModel utilityviewmodel,
                                                        char resource_code,
                                                        bool switch_on,
#if WINFORMS
                                                        bool decode,
#endif
                                                        SmartUtility.SwitchInfo RAY)
        {
            // Check the timeout once - here
            string http_message = SmartNibbyV2016.Check_Timeout(ourviewmodel);
            if (!string.IsNullOrEmpty(http_message))
            {
                // The timeout has to be 0 < timeout <= 120 for us to continue ... and its not!
                if (switch_on)
                {
                    if (!await SmartRoutinesV2018.CheckTrace(ourviewmodel, utilityviewmodel.utilityToken, RAY.proposed_supplier_code, RAY.proposed_brand_code, SmartParametersV2016.Utility.ToString() + " " + http_message))
                    {
                        return false;
                    }
                    return false;
                }
            }

            string target_supplier_name = SmartSpikeUtilityV2017.Utility_Lookup_SupplierName(ourviewmodel,
                                                                                utilityviewmodel,
                                                                                RAY.present_supplier_code,
                                                                                RAY.present_brand_code);
            string target_tariff_name = SmartSpikeUtilityV2017.Utility_Lookup_TariffName(utilityviewmodel,
                                                                    resource_code,
                                                                    RAY.present_supplier_code,
                                                                    RAY.present_brand_code,
                                                                    RAY.present_resource_type, //accounts_row.RESOURCE_TYPE,
                                                                    RAY.present_tariff_code);
            string target_payment_name = "";
            List<SmartUtility.PaymentPlans> payment_plans_found = SmartSpikeUtilityV2017.Utility_Find_PaymentPlansY(ourviewmodel,
                                                                                                                                    utilityviewmodel,
                                                                                                                                    RAY.present_payment_plan);
            if (payment_plans_found.Count > 0)
            {
                target_payment_name = payment_plans_found[0].PAYMENT_NAME;
            }



#if WINFORMS
            utilityviewmodel.console = true;
#endif
            utilityviewmodel.resource_code = resource_code;
            utilityviewmodel.next_routine = "";
            utilityviewmodel.prfix_xxx = RAY.url_prfix; //"https://www.energylinx.co.uk/"
            utilityviewmodel.supplier_name = target_supplier_name;
            utilityviewmodel.TARIFF_NAME = target_tariff_name;
            utilityviewmodel.payment_name = target_payment_name;
            utilityviewmodel.headers = new List<string>();//List<string>();


            // General controlling parameters
            bool TextBox_Active = true;                     // Turn it on
            if (!FrontEndGUI.GetBorderVisible(ourviewmodel))
            {
                FrontEndGUI.SetBorderScrollVisible(ourviewmodel);
            }

            // in it from last time
            // Get ready to Rock 'n Roll !!!
            bool task = false;

            task = await Do_The_Switches(
#if ANDROIDX
                                        meterActivity,
#endif
                                        ourviewmodel,
                                        utilityviewmodel,
                                        TextBox_Active,
                                        RAY);



#if WINFORMS
            // Do some analysis

            await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Account status:" + utilityviewmodel.account_status + Environment.NewLine.ToString());
            System.Windows.Forms.CheckBox stop = new System.Windows.Forms.CheckBox();
            System.Windows.Forms.CheckBox quiet = new System.Windows.Forms.CheckBox();

            await SmartResultsV2016.MES_contents(stop,
                                                quiet,
                                                ourviewmodel,
                                                utilityviewmodel.Hezbollah.ameliaList,
                                                utilityviewmodel.Hezbollah.utility_accountsList,
                                                utilityviewmodel.Hezbollah.account_charges_creditsList,
                                                ourviewmodel.SmartProfile.profilecubefacesList,
                                                ourviewmodel.SmartProfile.addressesviewList,
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
            // Don't send output to the TextBox anymore
            if (FrontEndGUI.GetBorderVisible(ourviewmodel))
            {
                FrontEndGUI.SetBorderScrollInvisible(ourviewmodel);
            }

            // We don't have the Login Attempted flag here, in case the
            // first web screen of the supplier is all wrong (i.e. no fields)
            if (!task)
            {
                if (switch_on)
                {
                    if (!await SmartRoutinesV2018.CheckTrace(ourviewmodel, utilityviewmodel.utilityToken, utilityviewmodel.supplier_code, utilityviewmodel.brand_code, SmartParametersV2016.Utility.ToString() + " Attempted: " + " Switch"))
                    {
                        return false;
                    }
                    if (!await SmartRoutinesV2018.CheckTrace(ourviewmodel, utilityviewmodel.utilityToken, utilityviewmodel.supplier_code, utilityviewmodel.brand_code, SmartParametersV2016.Utility.ToString() + " Finished: " + " Switch"))
                    {
                        return false;
                    }
                }
                // We never made it through ...
                FrontEndGUI.SetLedColour(ourviewmodel, 7, ourviewmodel.redColour);
                await SmartRoutinesV2018.TextBlockUpdate(
#if ANDROIDX
                    meterActivity,
#endif
                    ourviewmodel,
                    "Failed on: " + ourviewmodel.errorMessage + Environment.NewLine.ToString());
                return task;
            }
            return true;
        }

        // We always do a scrape on a SUPPLIER, never on a BRAND
        internal static async Task<bool> Do_The_Switches(
#if ANDROIDX
                                                        AppCompatActivity meterActivity,
#endif
                                                        MainViewModel ourviewmodel,
                                                        UtilityViewModel utilityviewmodel,
                                                        bool TextBox_Active,
                                                        SmartUtility.SwitchInfo RAY)
        {
            bool result = false;

            switch (RAY.proposed_supplier_code)
            {
                case 45:
                    // Out for the time being so I can get WebView working
//                    result = await EdFV2016.DoTheSwitch(
//#if ANDROIDX
//                                                        meterActivity,
//#endif
//                                                        ourviewmodel,
//                                                        utilityviewmodel,
//                                                        TextBox_Active,
//                                                        RAY);

                    break;

                case 133:   // Outbid the Fox
                            // Angel
                            // Other dumb fuckers
                    break;
                default:
                    // These are (or should be) all energylinx switches
                    //if (true)
                    //{
                    return true;
                    //}
                    //result = await DoTheSwitch(MES,
                    //                                TextBox_Active,
                    //                                                    textBoxBrowser,
                    //                                                    scrollViewer,
                    //                                                    RAY);
                    //                    break;
            }
            return result;
        }

        internal static async Task<bool> Switch_Email_New(MainViewModel ourviewmodel,
                                                        UtilityViewModel utilityviewmodel,
                                                        SmartUtility.SwitchInfo RAY,
                                                        string email_report,
                                                        StringBuilder bollocks)
        {
            // Send an e-mail to Anna
            // We only send ONE e-mail - if we have switch E then we send an E e-mail,
            // if we have switched G then we send a G e-mail, if we have switched D then we send an E/G e-mail


            if (!string.IsNullOrEmpty(RAY.email))
            {
                string to = RAY.email;
                //to = "ray_chapman48@hotmail.com";


                // http://www.codeforwin.in/2015/03/sending-emails-in-c-sharp.html
                //Smpt server
                string REG123_SERVER = "smtp.123-reg.co.uk";
                //Connecting port
                int PORT = 587;
                try
                {

#if WINFORMS || WPF  || WINUI
                    SmtpClient mailServer = new SmtpClient(REG123_SERVER, PORT)
                    {
                        EnableSsl = false,

                        //Provide your email id with your password.
                        //Enter the app-specfic password if two-step authentication is enabled.
                        Credentials = new System.Net.NetworkCredential("support@keasdon.co.uk", SmartParametersV2016.myPassword)
                    };
#endif
#if ANDROIDX 
                    // Define the MailServer
                    System.Net.Mail.SmtpClient mailServer = new System.Net.Mail.SmtpClient()
                    {
                        Host = REG123_SERVER,
                        Port = PORT,
                        EnableSsl = false,
                        //    //Provide your email id with your password.
                        //    //Enter the app-specfic password if two-step authentication is enabled.
                        Credentials = new System.Net.NetworkCredential("support@keasdon.co.uk", SmartParametersV2016.myPassword)
                    };
#endif
#if SMARTMAUI
                    SmtpClient mailServer = new SmtpClient(REG123_SERVER, PORT) //, false, "support@keasdon.co.uk", SmartParametersV2016.myPassword);
                    {
                        EnableSsl = false,
                        //Provide your email id with your password.
                        //Enter the app-specfic password if two-step authentication is enabled.
                        Credentials = new System.Net.NetworkCredential("support@keasdon.co.uk", SmartParametersV2016.myPassword)
                    };
#endif
                    //Senders email.
                    System.Net.Mail.MailAddress from = new System.Net.Mail.MailAddress(SmartParametersV2016.adminEmailAddress);  // "admin@Keasdon.co.uk";
                                                                                                                                 //Receiver email
                                                                                                                                 //string to = "atmc_21@hotmail.com";

#if WINFORMS || WPF  || WINUI
                    MailMessage msg = new MailMessage();

                    // Who should receive it
                    msg.To.Add(to);
                    msg.From = from;  // <= Check to ensure this actually works

                    //Subject of the email.
                    msg.Subject = "The First Automated Switching E-Mail In The Universe!";
                    //Specify the body of the email here.
                    msg.Body = email_report;
#endif
#if ANDROIDX
                    // Construct the message
                    System.Net.Mail.MailMessage msg = new System.Net.Mail.MailMessage();

                    // Who should receive it
                    msg.To.Add(to);
                    msg.From = from;  // <= Check to ensure this actually works
                    //Subject of the email.
                    msg.Subject = "The First Automated Switching E-Mail In The Universe!";
                    //Specify the body of the email here.
                    msg.Body = email_report;
#endif

                    // YES WE FUCKING DO
#if WINFORMS || WPF
                    mailServer.Send(msg);
#endif
#if WINUI 
                    await mailServer.SendMailAsync(msg);
#endif
#if ANDROIDX
                    await mailServer.SendMailAsync(msg);
#endif
                    // AND YES WE FUCKING CAN!!!!!!

                    // Get rid
#if WINFORMS || WPF  || WINUI
                    msg.Dispose();
#endif
#if ANDROIDX 
                    msg.Dispose();
                    mailServer.Dispose();
#endif
                    if (bollocks.Length > 0 &&
                        (ourviewmodel.UserNameColour == ourviewmodel.greenColour))
                    {
                        // This might return true or false

                        if (!await SmartBobV2017.Insert_COMMON_Async(ourviewmodel, utilityviewmodel.utilityToken, //"SmartSwitch",
                                                                                                                   bollocks))
                        {
                            if (!await SmartRoutinesV2018.CheckTrace(ourviewmodel, utilityviewmodel.utilityToken, RAY.present_supplier_code, RAY.present_brand_code, SmartParametersV2016.Utility.ToString() + " " + ourviewmodel.errorMessage))
                            {
                                return false;
                            }
                            return false;
                        }
                    }
                    if (!await SmartRoutinesV2018.CheckTrace(ourviewmodel, utilityviewmodel.utilityToken, RAY.present_supplier_code, RAY.present_brand_code, SmartParametersV2016.Utility.ToString() + " " + "Switch e-mail sent to " + to + " for " + RAY.proposed_brand_name + "/" + RAY.proposed_tariff_name))
                    {
                        return false;
                    }
                }
                catch (Exception exception)
                {
                    if (!await SmartRoutinesV2018.CheckTrace(ourviewmodel, utilityviewmodel.utilityToken, RAY.present_supplier_code, RAY.present_brand_code, SmartParametersV2016.Utility.ToString() + " " + "Failed send to " + to + " for " + RAY.proposed_brand_name + "/" + RAY.proposed_tariff_code + ": " + exception.Message))
                    {
                        return false;
                    }
                    return false;
                }
            }
            return true;
        }

        //internal static async Task<bool> Switch_Email(TimeSpan utcOffset,
        //                                                TimeSpan timespanTimeout,
        //                                                string username,
        //                                                string website,
        //                                                Switch RAY,
        //                                                AnalysisCostsView analysis_costs_row,
        //                                                CookieContainer cookies)
        //{
        //    // Send an e-mail to Anna
        //    // We only send ONE e-mail - if we have switch E then we send an E e-mail,
        //    // if we have switched G then we send a G e-mail, if we have switched D then we send an E/G e-mail


        //    if (!string.IsNullOrEmpty(RAY.email))
        //    {
        //        string to = RAY.email;
        //        to = "ray_chapman48@hotmail.com";

        //        List<SmartUtility.Meters> meters_foundx = SmartSpikeUtilityV2017.Find_MPAN_MPRN(RAY.cubeface_code,
        //                                                                    RAY.udprn,
        //                                                                    RAY.resource_code,
        //                                                                    Hamas.metersList);

        //        List<E_Readings> e_readings_found = new List<E_Readings>();
        //        List<G_Readings> g_readings_found = new List<G_Readings>();
        //        List<E_Readings> d_readings_found = new List<E_Readings>();
        //        SmartSpikeUtilityV2017.Find_Readings(RAY.resource_code,
        //                                        Hamas.consumers_viewList,
        //                                        Hamas.working_billsList,
        //                                        Hamas.bills_resourceList,
        //                                        Hamas.e_readingsList,
        //                                        Hamas.g_readingsList,
        //                                        rf e_readings_found,
        //                                        rf g_readings_found,
        //                                        rf d_readings_found);
        //        string meter_serial_no = "",
        //                READINGS_PERIOD_END = "",
        //                d_this_read = "",
        //                n_this_read = "";
        //        int readings_count = 0;

        //        switch (RAY.resource_code)
        //        {
        //            case SmartParametersV2016.Electricity:
        //                readings_count = e_readings_found.Count - 1;
        //                if (readings_count >= 0)
        //                {
        //                    meter_serial_no = e_readings_found[readings_count].METER_SERIAL_NO;
        //                    READINGS_PERIOD_END = e_readings_found[readings_count].READINGS_PERIOD_END.ToString(SmartParametersV2016.ddmmmyyyyFormat);
        //                    d_this_read = e_readings_found[readings_count].D_THIS_READ.ToString();
        //                    n_this_read = e_readings_found[readings_count].N_THIS_READ.ToString();
        //                }
        //                break;
        //            case SmartParametersV2016.Gas:
        //                readings_count = g_readings_found.Count - 1;
        //                if (readings_count >= 0)
        //                {
        //                    meter_serial_no = g_readings_found[readings_count].METER_SERIAL_NO;
        //                    READINGS_PERIOD_END = g_readings_found[readings_count].READINGS_PERIOD_END.ToString(SmartParametersV2016.ddmmmyyyyFormat);
        //                    d_this_read = g_readings_found[readings_count].D_THIS_READ.ToString();
        //                }
        //                break;
        //            default:
        //                // Dual Switch
        //                break;
        //        }

        //        string mpan_mprn = "";
        //        //if (meters_found.Length == 1)
        //        //{
        //        //    mpan_mprn = meters_found[0].MPAN_MPRN;
        //        //}

        //        string adminEmailAddress = SmartParametersV2016.adminEmailAddress; // "admin@Keasdon.co.uk";
        //        string email_report = SmartSwitcherV2018.Build_Email(username,
        //                RAY.cubeface_code,
        //                RAY.resource_code,
        //                RAY.resource_type,
        //                RAY.proposed_supplier_code,
        //                RAY.proposed_brand_code,
        //                RAY.created,
        //                RAY.account_no,
        //                RAY.postcode,
        //                analysis_costs_row.SupplierName,
        //                analysis_costs_row.TariffName,
        //                analysis_costs_row.PaymentName,
        //                Hamas.consumersList,
        //                Hamas.Utility.AccountsList,
        //                Hamas.resourcesList,
        //                Hezbollah.categoriesList,
        //                Hezbollah.suppliersList,
        //                Hezbollah.brandsList,
        //                Hezbollah.templateList,
        //                Hezbollah.tariffsList,
        //                Hezbollah.tariff_matrixList,
        //                Hezbollah.payment_plansList,
        //                READINGS_PERIOD_END,
        //                d_this_read,
        //                n_this_read,
        //                adminEmailAddress);
        //        // http://www.codeforwin.in/2015/03/sending-emails-in-c-sharp.html
        //        //Smpt server
        //        string REG123_SERVER = "smtp.123-reg.co.uk";
        //        //Connecting port
        //        int PORT = 587;
        //        try
        //        {
        //            SmtpClient mailServer = new SmtpClient(REG123_SERVER, PORT)
        //            {
        //                EnableSsl = false,

        //                //Provide your email id with your password.
        //                //Enter the app-specfic password if two-step authentication is enabled.
        //                Credentials = new System.Net.NetworkCredential("support@keasdon.co.uk", SmartParametersV2016.myPassword)
        //            };

        //            //Senders email.
        //            string from = adminEmailAddress;  // "admin@Keasdon.co.uk";
        //                                                //Receiver email
        //                                                //string to = "atmc_21@hotmail.com";

        //            MailMessage msg = new MailMessage(from, to)
        //            {
        //                //Subject of the email.
        //                Subject = "The First Automated Switching E-Mail In The Universe!",
        //                //Specify the body of the email here.
        //                Body = email_report
        //            };

        //            // YES WE FUCKING DO
        //            mailServer.Send(msg);
        //            // AND YES WE FUCKING CAN!!!!!!

        //            Resources resources_row = new Resources()
        //            {
        //                CUBEFACE_CODE = RAY.cubeface_code,
        //                SUPPLIER_CODE = RAY.proposed_supplier_code,
        //                BRAND_CODE = RAY.proposed_brand_code,
        //                CREATED = RAY.created,
        //                ACCOUNT_NO = RAY.account_no,
        //                MPAN_MPRN = mpan_mprn,
        //                // Some in the middle are left alone
        //                AUTOSWITCH = 'N',                           // Turn this OFF now
        //                AUTOSWITCH_DESTINATION = to,
        //                AUTOSWITCH_EMAIL_SENT = ourviewmodel.time_now,
        //                AUTOSWITCH_SUPPLIER_CODE = analysis_costs_row.SupplierCode,
        //                AUTOSWITCH_TARIFF_CODE = analysis_costs_row.TariffCode,
        //                Updated = true              // Its a change
        //            };
        //            utilityviewmodel.resources_changesList = new List<SmartUtility.Resources>()   // Make sure the count here is zero
        //
        //            utilityviewmodel.resources_changesList.Add(resources_row);

        //            StringBuilder bollocks = new StringBuilder();

        //            SmartNibbyV2016.DoAll_Resources(RAY.cubeface_code,
        //                                            RAY.proposed_supplier_code,
        //                                            RAY.proposed_brand_code,
        //                                            RAY.account_no,
        //                                            RAY.created,
        //                                            mpan_mprn,
        //                                            SmartParametersV2016.yymmddShortFormat,
        //                                            MES,
        //                                            Hamas);
        //            if (bollocks.Length > 0 &&
        //                  (ourviewmodel.UserNameColour == ourviewmodel.greenColour))
        //            {
        //                // This might return true or false
        //                string urgent_message = "";
        //                if (!await SmartBobV2017.Insert_COMMON_Async(timespanTimeout, cookies, username, website, bollocks, p => urgent_message = p))
        //                {
        //                    await SmartBobV2017.ListenerAsync(SignIn.signinviewmodel.Username,timespanTimeout, cookies, username, website, analysis_costs_row.BrandCode, analysis_costs_row.SupplierCode, urgent_message);
        //                    return false;
        //                }
        //            }
        //            await SmartBobV2017.ListenerAsync(SignIn.signinviewmodel.Username,timespanTimeout, cookies, username, website, analysis_costs_row.BrandCode, analysis_costs_row.SupplierCode, "Switch e-mail sent to " + to + " for " + analysis_costs_row.SupplierName + "/" + analysis_costs_row.TariffName);
        //        }
        //        catch (Exception exception)
        //        {
        //            await SmartBobV2017.ListenerAsync(SignIn.signinviewmodel.Username,timespanTimeout, cookies, username, website, analysis_costs_row.BrandCode, analysis_costs_row.SupplierCode, "Failed send to " + to + " for " + analysis_costs_row.SupplierName + "/" + analysis_costs_row.TariffName + ": " + exception.Message);
        //            return false;
        //        }
        //    }
        //    return true;
        //}

        //internal static string Build_Email(string username,
        //                                    char cubeface_code,
        //                                    char resource_code,
        //                                    string resource_type,
        //                                    short supplier_code,
        //                                    short brand_code,
        //                                    DateTime created,
        //                                    //string mpan_mprn,
        //                                    string AccountNo,
        //                                    string Postcode,
        //                                    string SupplierName,
        //                                    string PaymentName,
        //                                    List<SmartUtility.Consumers> consumersList,
        //                                    List<SmartUtility.Accounts> accountsList,
        //                                    List<SmartUtility.Resources> resourcesList,
        //                                    List<Categories> categoriesList,
        //                                    List<SmartUtility.Suppliers> suppliersList,
        //                                    List<SmartUtility.Brands> brandsList,
        //                                    List<SmartUtility.Tariffs> tariffsList,
        //                                    List<SmartUtility.TariffMatrix> tariff_matrixList,
        //                                    List<SmartUtility.PaymentPlans> payment_plansList,
        //                                    //string meter_serial_no,
        //                                    string READINGS_PERIOD_END,
        //                                    string d_this_read,
        //                                    string n_this_read,
        //                                    string adminEmailAddress)
        //{
        //    string my_name = "",
        //                            my_address = "",
        //                            //my_postcode = Postcode,
        //                            my_contact_phone = "",
        //                            my_email_address = "",
        //                            old_supplier_name = "",
        //                            old_tariff_name = "",
        //                            old_payment_plan = "",
        //                            elec_account_no = "",
        //                            elec_meter_type = "",
        //                            gas_account_no = "";
        //    Lookup_Account_Details(username,
        //                    cubeface_code,
        //                    resource_code,
        //                    resource_type,
        //                    supplier_code,
        //                    brand_code,
        //                    created,
        //                    AccountNo,
        //                    consumersList,
        //                    accountsList,
        //                    resourcesList,
        //                    rf my_name,
        //                    rf my_address,
        //                    rf my_contact_phone,
        //                    rf my_email_address,
        //                    rf old_supplier_name,
        //                    rf old_tariff_name,
        //                    rf old_payment_plan,
        //                    rf elec_account_no,
        //                    rf elec_meter_type,
        //                    rf gas_account_no,
        //                    categoriesList,
        //                    suppliersList,
        //                    brandsList,
        //                    tariffsList,
        //                    tariff_matrixList,
        //                    payment_plansList);
        //    //return SmartReportV2016.fill_email_report(resource_code,
        //    //                                                SupplierName,
        //    //                                                TariffName,
        //    //                                                PaymentName,
        //    //                                                templatesList,
        //    //                                                my_name,
        //    //                                                my_address,
        //    //                                                my_postcode,
        //    //                                                my_contact_phone,
        //    //                                                my_email_address,
        //    //                                                old_supplier_name,
        //    //                                                old_tariff_name,
        //    //                                                old_payment_plan,
        //    //                                                //mpan_mprn,
        //    //                                                elec_account_no,
        //    //                                                elec_meter_type,
        //    //                                                gas_account_no,
        //    //                                                //meter_serial_no,
        //    //                                                READINGS_PERIOD_END,
        //    //                                                d_this_read,
        //    //                                                n_this_read,
        //    //                                                adminEmailAddress);
        //    return "";

        //}

        //internal static bool Lookup_Account_Details(string username,
        //                                            char cubeface_code,
        //                                            char resource_code,
        //                                            string resource_type,
        //                                            short supplier_code,
        //                                            short brand_code,
        //                                            DateTime created,
        //                                            string account_no,
        //                                            List<SmartUtility.Consumers> consumersList,
        //                                            List<SmartUtility.Accounts> accountsList,
        //                                            List<SmartUtility.Resources> resourcesList,
        //                                            rf string my_name,
        //                                            rf string my_address,
        //                                            rf string my_contact_phone,
        //                                            rf string my_email_address,
        //                                            rf string old_supplier_name,
        //                                            rf string old_tariff_name,
        //                                            rf string old_payment_plan,
        //                                            rf string elec_account_no,
        //                                            rf string elec_meter_type,
        //                                            rf string gas_account_no,
        //                                            List<Categories> categoriesList,
        //                                            List<SmartUtility.Suppliers> suppliersList,
        //                                            List<SmartUtility.Brands> brandsList,
        //                                            List<SmartUtility.Tariffs> tariffsList,
        //                                            List<SmartUtility.TariffMatrix> tariff_matrixList,
        //                                            List<SmartUtility.PaymentPlans> payment_plansList)
        //{
        //    List<SmartUtility.Accounts> accounts_found = SmartSpikeUtilityV2017.Lookup_Accounts(username,
        //                                                                    cubeface_code,
        //                                                                    supplier_code,
        //                                                                    brand_code,
        //                                                                    created,
        //                                                                    account_no,
        //                                                                    accountsList);
        //    if (accounts_found.Count > 0)
        //    {
        //        foreach (SmartUtility.Accounts accounts_row in accounts_found)
        //        {
        //            if (!string.IsNullOrEmpty(accounts_row.Spare1))
        //            {
        //                my_name = accounts_row.Spare1;
        //            }
        //            //if (!string.IsNullOrEmpty(accounts_row.Spare2))
        //            //{
        //            //    my_address = accounts_row.Spare2;
        //            //}
        //            if (!string.IsNullOrEmpty(accounts_row.Spare3))
        //            {
        //                my_contact_phone = accounts_row.Spare3;
        //            }
        //            if (!string.IsNullOrEmpty(accounts_row.Spare4))
        //            {
        //                my_email_address = accounts_row.Spare4;
        //            }
        //            old_supplier_name = SmartSpikeUtilityV2017.Utility_Lookup_SupplierName(cubeface_code,
        //                                                                            accounts_row.SUPPLIER_CODE,
        //                                                                            brand_code,
        //                                                                            categoriesList,
        //                                                                            suppliersList,
        //                                                                            brandsList);
        //            old_tariff_name = SmartSpikeUtilityV2017.Utility_Lookup_TariffName(resource_code,
        //                                                                    accounts_row.SUPPLIER_CODE,
        //                                                                    brand_code,
        //                                                                    resource_type, //accounts_row.RESOURCE_TYPE,
        //                                                                    accounts_row.TARIFF_CODE,
        //                                                                    tariffsList,
        //                                                                    tariff_matrixList);
        //            List<SmartUtility.PaymentPlans> payment_plans_found = SmartSpikeUtilityV2017.Find_Payment_Plans(accounts_row.PAYMENT_PLAN,
        //                                                                                          payment_plansList);
        //            if (payment_plans_found.Count > 0)
        //            {
        //                old_payment_plan = payment_plans_found[0].PAYMENT_NAME;
        //            }
        //            switch (resource_code) //(accounts_row.RESOURCE_CODE)
        //            {
        //                case SmartParametersV2016.Electricity:
        //                    elec_account_no = accounts_row.ACCOUNT_NO;
        //                    switch (resource_type) //(accounts_row.RESOURCE_TYPE)
        //                    {
        //                        case "SR":
        //                            elec_meter_type = "Single-rate";
        //                            break;
        //                        case "VR":
        //                            elec_meter_type = "Dual-rate (Economy7)";
        //                            break;
        //                        default:
        //                            break;
        //                    }
        //                    break;
        //                case SmartParametersV2016.Gas:
        //                    gas_account_no = accounts_row.ACCOUNT_NO;
        //                    break;
        //                default:
        //                    break;
        //            }

        //        }
        //    }
        //    return true;
        //}


        internal static async Task<bool> DoTheSwitchRay(
#if ANDROIDX
                                                        AppCompatActivity    meterActivity,
#endif
                                                        MainViewModel ourviewmodel,
                                                        UtilityViewModel utilityviewmodel,
                                                        bool TextBox_Active,
                                                        SmartUtility.SwitchInfo RAY)
        {
            // Because Sometimes we are returned a Document
            HtmlAgilityPack.HtmlDocument htmlDocument = new HtmlAgilityPack.HtmlDocument();

            List<KeyValuePair<string, string>> keyValues = new List<KeyValuePair<string, string>>();

            bool status = false;

            bool keep_looping = true;
            //string target_string = "";

            utilityviewmodel.wskey = "";
            utilityviewmodel.wskey_name = "";

            bool expect_json = false;
            string json = "";
            //string authenticity_token = "",   // Varies at each step
            //        signup_code = "",
            //        vanity_address_udprn = "";

            //string switch_now_path = "";

            utilityviewmodel.proceed_online_path = "";

            utilityviewmodel.udprn = "";
            utilityviewmodel.property_value = "";
            utilityviewmodel.House_No = "";
            utilityviewmodel.House_Name = "";
            utilityviewmodel.Street = "";
            utilityviewmodel.City_Town = "";

            GenericAddress Address = new GenericAddress();

            RAY.address = RAY.address.Replace(SmartParametersV2016.bar.ToString(), "");
            RAY.address = SmartParseV2016.Remove_Double_Spaces_V3(RAY.address).Trim();

            utilityviewmodel.logout_pathname = "";
            utilityviewmodel.next_routine = "HOME";
            utilityviewmodel.target_pathname = ""; //Cos the first call is a POST
            expect_json = true;

            while (keep_looping)
            {
                if (!string.IsNullOrEmpty(ourviewmodel.errorMessage) ||
                    utilityviewmodel.next_routine == "LOGOUT")
                {
                    utilityviewmodel.target_pathname = utilityviewmodel.logout_pathname;
                    keep_looping = false;
                    utilityviewmodel.next_routine = "LOGOUT";
                }
                if (!string.IsNullOrEmpty(utilityviewmodel.target_pathname))
                {
                    if (utilityviewmodel.next_routine.ToUpper() == utilityviewmodel.next_routine)
                    {
                        // We are doing GETs
                        if (!expect_json)
                        {
                            htmlDocument = await SmartBobV2017.Scraper_Generic_Get(ourviewmodel, utilityviewmodel.utilityToken, utilityviewmodel);
                            if (!string.IsNullOrEmpty(ourviewmodel.errorMessage))
                            {
                                return false;
                            }
                        }
                        else
                        {
                            json = await SmartBobV2017.Scraper_Generic_Get_Json(ourviewmodel, utilityviewmodel.utilityToken, utilityviewmodel);
                            if (!string.IsNullOrEmpty(ourviewmodel.errorMessage))
                            {
                                return false;
                            }
                        }
                    }
                    else
                    {
                        // We are doing POSTs
                        htmlDocument = await SmartBobV2017.Scraper_Generic_Post(ourviewmodel, utilityviewmodel.utilityToken, keyValues, utilityviewmodel);
                        if (!string.IsNullOrEmpty(ourviewmodel.errorMessage))
                        {
                            return false;
                        }
                    }
                }

                // Can't miss out a STEP!!  Have to do all of them!!!
                utilityviewmodel.current_routine = utilityviewmodel.next_routine;
                switch (utilityviewmodel.next_routine)
                {
                    case "HOME":
#if WINFORMS
                        if (utilityviewmodel.console)
                        {
                            await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel,
                                "Switching now: " + ourviewmodel.TargetUrl + Environment.NewLine.ToString());
                        }
#endif
                        if (TextBox_Active)
                        {
                            await SmartRoutinesV2018.TextBlockUpdate(
#if ANDROIDX
                                meterActivity,
#endif
                                ourviewmodel,
                                SmartParametersV2016.space +

                                "SwitchingNow" +

                                SmartParametersV2016.space +
                                ourviewmodel.TargetUrl +
#if ANDROIDX
                                SmartParametersV2016.space);
#else
                                "HeavyTick");
#endif
                        }

                        keyValues.Clear();

                        keyValues.Add(new KeyValuePair<string, string>("values[postcode]", ""));
                        keyValues.Add(new KeyValuePair<string, string>("values[email]", ""));
                        keyValues.Add(new KeyValuePair<string, string>("values[phoneMobile]", ""));

                        utilityviewmodel.target_pathname = "https://www.energylinx.co.uk/energycalc.html?db=dual&values[postcode]=" + Uri.EscapeDataString(RAY.postcode);
                        utilityviewmodel.next_routine = "Find_Address";  // Lower case? Then next scrape is a POST
                        break;
                    case "Find_Address":
                        // Look for the Web Service Key which is required for every POST
                        if (!Find_Wskey(utilityviewmodel, htmlDocument))//, rf wskey, rf wskey_name))
                        {
#if WINFORMS
                            if (utilityviewmodel.console)
                            {
                                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel,
                                    "Cannot find Web Service Key" + Environment.NewLine.ToString());
                            }
#endif
                            if (TextBox_Active)
                            {
                                await SmartRoutinesV2018.TextBlockUpdate(
#if ANDROIDX
                                    meterActivity,
#endif
                                    ourviewmodel,
#if ANDROIDX
                                    SmartParametersV2016.space);
#else
                                    "CannotFindWebServiceKey" +
                                    "HeavyTick"); ;
#endif

                            }

                            ourviewmodel.errorMessage = utilityviewmodel.current_routine;
                            return false;
                        }
                        else
                        {
#if WINFORMS
                            if (utilityviewmodel.console)
                            {
                                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel,
                                        "Web Service Key: " + utilityviewmodel.wskey + Environment.NewLine.ToString());
                            }
#endif
                            if (TextBox_Active)
                            {
                                await SmartRoutinesV2018.TextBlockUpdate(
#if ANDROIDX
                                    meterActivity,
#endif
                                    ourviewmodel,

                                    "WebServiceKey" +

                                    SmartParametersV2016.space +
                                    utilityviewmodel.wskey +
#if ANDROIDX
                                    SmartParametersV2016.space);
#else
                                    "HeavyTick");
#endif
                            }

                            utilityviewmodel.target_pathname = "ws/?callback=none&fields=" + Uri.EscapeDataString(RAY.postcode) + "&wsFunction=GetAddressListJson";  // Find address
                            utilityviewmodel.next_routine = "FIND_JSON";  // Upper case? Then next scrape is a GET
                        }
                        break;
                    case "FIND_JSON":
                        expect_json = false;
                        if (!SmartNibbyV2016.PS_GetAddress(utilityviewmodel,
                                                            json,
                                                            RAY.postcode,
                                                            RAY.address,
                                                            Address))
                        {
#if WINFORMS
                            if (utilityviewmodel.console)
                            {
                                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel,
                                        "Cannot decode JSON" + Environment.NewLine.ToString());
                            }
#endif
                            if (TextBox_Active)
                            {
                                await SmartRoutinesV2018.TextBlockUpdate(
#if ANDROIDX
                                    meterActivity,
#endif
                                    ourviewmodel,
                                    "CannotDecodeJson" +
                                    "HeavyTick");
                            }
                            ourviewmodel.errorMessage = utilityviewmodel.current_routine;
                            return false;
                        }
                        else
                        {
                            switch (utilityviewmodel.resource_code)
                            {
                                case SmartParametersV2016.Electricity:
                                    utilityviewmodel.target_pathname = "energycalc.html?db=electric&values[postcode]=";
                                    break;
                                case SmartParametersV2016.Gas:
                                    utilityviewmodel.target_pathname = "energycalc.html?db=gas&values[postcode]=";
                                    break;
                                default:
                                    utilityviewmodel.target_pathname = "energycalc.html?db=dual&values[postcode]=";
                                    break;
                            }
                            utilityviewmodel.target_pathname += RAY.postcode;
#if WINFORMS
                            if (utilityviewmodel.console)
                            {
                                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel,
                                        "Address : " + RAY.address + Environment.NewLine.ToString());
                                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel,
                                        "Address Code: " + utilityviewmodel.udprn + Environment.NewLine.ToString());
                                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel,
                                        "House No: " + utilityviewmodel.House_No + Environment.NewLine.ToString());
                                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel,
                                        "House Name: " + utilityviewmodel.House_Name + Environment.NewLine.ToString());
                                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel,
                                        "Street: " + utilityviewmodel.Street + Environment.NewLine.ToString());
                                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel,
                                        "City/Town: " + utilityviewmodel.City_Town + Environment.NewLine.ToString());
                            }
#endif
                            if (TextBox_Active)
                            {

                                await SmartRoutinesV2018.TextBlockUpdate(
#if ANDROIDX
                                    meterActivity,
#endif
                                    ourviewmodel,
                                    "SwitchAddress" +
                                    SmartParametersV2016.space +
                                    RAY.address);
                                await SmartRoutinesV2018.TextBlockUpdate(
#if ANDROIDX
                                    meterActivity,
#endif
                                    ourviewmodel,
                                    "SwitchAddressCode" +
                                    SmartParametersV2016.space +
                                    utilityviewmodel.udprn);
                                await SmartRoutinesV2018.TextBlockUpdate(
#if ANDROIDX
                                    meterActivity,
#endif
                                    ourviewmodel,
                                    "SwitchHouseNo" +
                                    SmartParametersV2016.space +
                                    utilityviewmodel.House_No);
                                await SmartRoutinesV2018.TextBlockUpdate(
#if ANDROIDX
                                    meterActivity,
#endif
                                    ourviewmodel,
                                    "SwitchHouseName" +
                                    SmartParametersV2016.space +
                                    utilityviewmodel.House_Name);
                                await SmartRoutinesV2018.TextBlockUpdate(
#if ANDROIDX
                                    meterActivity,
#endif
                                    ourviewmodel,
                                    "SwitchStreet" +
                                    SmartParametersV2016.space +
                                    utilityviewmodel.Street);
                                await SmartRoutinesV2018.TextBlockUpdate(
#if ANDROIDX
                                    meterActivity,
#endif
                                    ourviewmodel,
                                   "SwitchCityTown" +
                                    SmartParametersV2016.space +
                                    utilityviewmodel.City_Town);
                            }


                            keyValues.Clear();

                            keyValues.Add(new KeyValuePair<string, string>(utilityviewmodel.wskey_name, utilityviewmodel.wskey)); // Not sure whether this is position dependant?

                            keyValues.Add(new KeyValuePair<string, string>("done", "profile"));      // Should be profile Yes
                            keyValues.Add(new KeyValuePair<string, string>("values[referral]", "1631"));


                            keyValues.Add(new KeyValuePair<string, string>("values[affCustomField]", ""));
                            keyValues.Add(new KeyValuePair<string, string>("values[affCustomField1]", ""));
                            keyValues.Add(new KeyValuePair<string, string>("values[affCustomField2]", ""));
                            keyValues.Add(new KeyValuePair<string, string>("values[affCustomField3]", ""));
                            keyValues.Add(new KeyValuePair<string, string>("values[affCustomField4]", ""));
                            keyValues.Add(new KeyValuePair<string, string>("values[affCustomField5]", ""));
                            keyValues.Add(new KeyValuePair<string, string>("values[affCustomField6]", ""));
                            keyValues.Add(new KeyValuePair<string, string>("values[referIDcust]", ""));
                            keyValues.Add(new KeyValuePair<string, string>("values[email]", ""));
                            keyValues.Add(new KeyValuePair<string, string>("values[gasOrElecOrDual]", ""));
                            keyValues.Add(new KeyValuePair<string, string>("values[greenRatingSearch]", ""));
                            keyValues.Add(new KeyValuePair<string, string>("values[unknownUsageQuestions]", ""));
                            keyValues.Add(new KeyValuePair<string, string>("values[profileIncomplete]", ""));
                            keyValues.Add(new KeyValuePair<string, string>("values[noQuoteLimits]", ""));
                            keyValues.Add(new KeyValuePair<string, string>("callcenter", ""));
                            keyValues.Add(new KeyValuePair<string, string>("values[callCentreVersion]", ""));
                            keyValues.Add(new KeyValuePair<string, string>("values[CCinboundOrOutbound]", ""));
                            keyValues.Add(new KeyValuePair<string, string>("values[CCactivity]", ""));
                            keyValues.Add(new KeyValuePair<string, string>("values[testSignup]", ""));
                            keyValues.Add(new KeyValuePair<string, string>("values[fieldsForResults]", ""));
                            keyValues.Add(new KeyValuePair<string, string>("values[fieldsExtra]", ""));
                            keyValues.Add(new KeyValuePair<string, string>("values[houseNameOrNumber]", ""));
                            keyValues.Add(new KeyValuePair<string, string>("flags[templateType]", "results_body-desktop-full"));
                            keyValues.Add(new KeyValuePair<string, string>("values[postcode]", RAY.postcode));
                            keyValues.Add(new KeyValuePair<string, string>("values[lookupAddress]", Address.value));
                            keyValues.Add(new KeyValuePair<string, string>("supplier[current]", "1"));
                            keyValues.Add(new KeyValuePair<string, string>("values[singleOrSeparate]", "single"));

                            //if (payment_plan != "K")
                            //{
                            //    keyValues.Add(new KeyValuePair<string, string>("values[existingPayType]", payment_type));
                            //}
                            //else
                            //{
                            //    keyValues.Add(new KeyValuePair<string, string>("values[existingPayType]", payment_type));
                            //}
                            switch (utilityviewmodel.resource_code)
                            {
                                case SmartParametersV2016.Electricity:

                                    if (RAY.resource_type == "VR")  // This should be utilityviewmodel.resource_type
                                    {
                                        keyValues.Add(new KeyValuePair<string, string>("values[existingElecTariff]", "2"));
                                        keyValues.Add(new KeyValuePair<string, string>("values[nightUsePercent]", "55"));
                                    }
                                    else
                                    {
                                        keyValues.Add(new KeyValuePair<string, string>("values[existingElecTariff]", "1"));
                                        keyValues.Add(new KeyValuePair<string, string>("values[nightUsePercent]", "0"));
                                    }
                                    keyValues.Add(new KeyValuePair<string, string>("values[elecInputType]", "pounds"));
                                    keyValues.Add(new KeyValuePair<string, string>("values[elecBill]", "1000"));
                                    keyValues.Add(new KeyValuePair<string, string>("values[elecBillInterval]", "year"));

                                    keyValues.Add(new KeyValuePair<string, string>("values[elecUsage]", ""));
                                    keyValues.Add(new KeyValuePair<string, string>("values[elecUsageInterval]", "year"));
                                    break;
                                case SmartParametersV2016.Gas:
                                    keyValues.Add(new KeyValuePair<string, string>("values[existingGasTariff]", "1"));

                                    keyValues.Add(new KeyValuePair<string, string>("values[gasInputType]", "pounds"));
                                    keyValues.Add(new KeyValuePair<string, string>("values[gasBill]", "1000"));
                                    keyValues.Add(new KeyValuePair<string, string>("values[gasBillInterval]", "year"));

                                    keyValues.Add(new KeyValuePair<string, string>("values[gasUsage]", ""));
                                    keyValues.Add(new KeyValuePair<string, string>("values[gasUsageInterval]", "year"));
                                    break;
                                default:

                                    // Dual Fuel - Electric part
                                    keyValues.Add(new KeyValuePair<string, string>("values[existingSupplierName2]", ""));
                                    // Always the ALTERNATIVE which should be one-to-one with EL
                                    keyValues.Add(new KeyValuePair<string, string>("values[existingSupplierName]", utilityviewmodel.supplier_name));
                                    // Always the ALTERNATIVE which should be one-to-one with EL
                                    keyValues.Add(new KeyValuePair<string, string>("values[existingTariff]", utilityviewmodel.TARIFF_NAME));
                                    keyValues.Add(new KeyValuePair<string, string>("values[existingElecTariff]", "1"));
                                    keyValues.Add(new KeyValuePair<string, string>("values[nightUsePercent]", "42"));
                                    keyValues.Add(new KeyValuePair<string, string>("values[existingPayTypeELEC]", "4;11"));
                                    keyValues.Add(new KeyValuePair<string, string>("values[elecInputType]", "estimate"));
                                    keyValues.Add(new KeyValuePair<string, string>("values[elecBill]", ""));
                                    keyValues.Add(new KeyValuePair<string, string>("values[elecBillInterval]", ""));
                                    keyValues.Add(new KeyValuePair<string, string>("values[elecUsage]", ""));
                                    keyValues.Add(new KeyValuePair<string, string>("values[elecUsageInterval]", "year"));
                                    keyValues.Add(new KeyValuePair<string, string>("values[unknownUsageTypeElec]", "Use OFGEM averages"));
                                    keyValues.Add(new KeyValuePair<string, string>("values[unknownUsageElec]", "medium user"));

                                    // Dual Fuel - Gas part
                                    keyValues.Add(new KeyValuePair<string, string>("values[existingSupplierName1]", ""));
                                    keyValues.Add(new KeyValuePair<string, string>("values[existingPayTypeGAS]", "4;11"));
                                    keyValues.Add(new KeyValuePair<string, string>("values[gasInputType]", "estimate"));
                                    keyValues.Add(new KeyValuePair<string, string>("values[gasBill]", ""));
                                    keyValues.Add(new KeyValuePair<string, string>("values[gasBillInterval]", ""));
                                    keyValues.Add(new KeyValuePair<string, string>("values[gasUsage]", ""));
                                    keyValues.Add(new KeyValuePair<string, string>("values[gasUsageInterval]", "year"));
                                    keyValues.Add(new KeyValuePair<string, string>("values[unknownUsageTypeGas]", "Use OFGEM averages"));
                                    keyValues.Add(new KeyValuePair<string, string>("values[unknownUsageGas]", "medium user"));

                                    break;
                            }

                            keyValues.Add(new KeyValuePair<string, string>("values[desiredPayType]", "1;2;9"));
                            keyValues.Add(new KeyValuePair<string, string>("values[email]", ""));
                            keyValues.Add(new KeyValuePair<string, string>("values[phoneDaytime]", ""));

                            utilityviewmodel.next_routine = "Retrieve_Quote";    // Lower case? Its a POST
                        }
                        break;

                    case "Retrieve_Quote":
                        string signuplink = RetrieveQuote(utilityviewmodel,
                                            htmlDocument,
                                            RAY.proposed_brand_name,
                                            RAY.proposed_tariff_name);
                        if (string.IsNullOrEmpty(signuplink))
                        {
#if WINFORMS
                            if (utilityviewmodel.console)
                            {
                                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel,
                                        "SignUpLink is empty" + Environment.NewLine.ToString());
                            }
#endif
                            if (TextBox_Active)
                            {
                                await SmartRoutinesV2018.TextBlockUpdate(
#if ANDROIDX
                                    meterActivity,
#endif
                                    ourviewmodel,
                                    "SignupLinkEmpty" +
                                    "HeavyTick");

                            }
                            ourviewmodel.errorMessage = utilityviewmodel.current_routine;
                            return false;
                        }
                        else
                        {
                            utilityviewmodel.target_pathname = signuplink.Replace(utilityviewmodel.prfix_xxx, ""); // Done = results here
                            utilityviewmodel.target_pathname = utilityviewmodel.target_pathname.Replace("amp;", "");
                            utilityviewmodel.next_routine = "CONTACT";    //  Upper case? Next scrape is a GET
                        }
                        break;
                    case "CONTACT":
                        if (!Find_Wskey(utilityviewmodel, htmlDocument))//, rf wskey, rf wskey_name))
                        {
#if WINFORMS
                            if (utilityviewmodel.console)
                            {
                                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel,
                                        "Cannot find Web Service Key" + Environment.NewLine.ToString());
                            }
#endif
                            if (TextBox_Active)
                            {
                                await SmartRoutinesV2018.TextBlockUpdate(
#if ANDROIDX
                                    meterActivity,
#endif
                                    ourviewmodel,
                                    "CannotFindWebServiceKey" +
                                    "HeavyTick");
                            }
                            ourviewmodel.errorMessage = utilityviewmodel.current_routine;
                            return false;
                        }
                        else
                        {
#if WINFORMS
                            if (utilityviewmodel.console)
                            {
                                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel,
                                        "Web Service Key: " + utilityviewmodel.wskey + Environment.NewLine.ToString());
                            }
#endif
                            if (TextBox_Active)
                            {
                                await SmartRoutinesV2018.TextBlockUpdate(
#if ANDROIDX
                                    meterActivity,
#endif
                                    ourviewmodel,
                                    "WebServiceKey" +
                                    SmartParametersV2016.space +
                                    utilityviewmodel.wskey +
                                    "HeavyTick");
                            }


                            switch (utilityviewmodel.resource_code)
                            {
                                case SmartParametersV2016.Electricity:
                                    utilityviewmodel.target_pathname = "energycalc.html?db=electric"; ;
                                    break;
                                case SmartParametersV2016.Gas:
                                    utilityviewmodel.target_pathname = "energycalc.html?db=gas";
                                    break;
                                default:
                                    utilityviewmodel.target_pathname = "energycalc.html?db=dual";
                                    break;
                            }


                            keyValues.Add(new KeyValuePair<string, string>("done", "signup_contact"));
                            keyValues.Add(new KeyValuePair<string, string>("callcenter", ""));
                            keyValues.Add(new KeyValuePair<string, string>("find_address_second", ""));

                            keyValues.Add(new KeyValuePair<string, string>(utilityviewmodel.wskey_name, utilityviewmodel.wskey)); // Different from previous?

                            keyValues.Add(new KeyValuePair<string, string>("results[lookupAddress]", Address.value));
                            keyValues.Add(new KeyValuePair<string, string>("results[title]", RAY.title));
                            keyValues.Add(new KeyValuePair<string, string>("results[firstName]", RAY.first_name));
                            keyValues.Add(new KeyValuePair<string, string>("results[lastName]", RAY.last_name));
                            keyValues.Add(new KeyValuePair<string, string>("values[username]", RAY.email));
                            keyValues.Add(new KeyValuePair<string, string>("results[dayOrEveningPhone]", "Day"));
                            keyValues.Add(new KeyValuePair<string, string>("results[phoneDaytime]", RAY.telephone));
                            keyValues.Add(new KeyValuePair<string, string>("results[dateOfBirthDay]", RAY.birth_day.ToString()));
                            keyValues.Add(new KeyValuePair<string, string>("results[dateOfBirthMonth]", RAY.birth_month.ToString()));
                            keyValues.Add(new KeyValuePair<string, string>("results[dateOfBirthYear]", RAY.birth_year.ToString()));

                            string[] split = RAY.postcode.Split(' ');
                            if (split.Length > 0)
                            {
                                keyValues.Add(new KeyValuePair<string, string>("results[postcodeOutward]", split[0])); //    M22
                            }
                            if (split.Length > 1)
                            {
                                keyValues.Add(new KeyValuePair<string, string>("results[postcodeInward]", split[1])); // 4YD
                            }
                            else
                            {
                                keyValues.Add(new KeyValuePair<string, string>("results[postcodeInward]", ""));
                            }
                            keyValues.Add(new KeyValuePair<string, string>("results[yearsAtCurrentAddress]", "3"));
                            keyValues.Add(new KeyValuePair<string, string>("results[monthsAtCurrentAddress]", "0"));
                            keyValues.Add(new KeyValuePair<string, string>("results[overseasSecondAddress]", "NO"));
                            keyValues.Add(new KeyValuePair<string, string>("results[secondPostcode", ""));
                            keyValues.Add(new KeyValuePair<string, string>("results[yearsAtSecondAddress]", "0"));
                            keyValues.Add(new KeyValuePair<string, string>("results[monthsAtSecondAddress]", "0"));
                            keyValues.Add(new KeyValuePair<string, string>("results[thirdPostcode]", ""));
                            keyValues.Add(new KeyValuePair<string, string>("results[yearsAtThirdAddress]", "0"));
                            keyValues.Add(new KeyValuePair<string, string>("results[monthsAtThirdAddress]", "0"));
                            keyValues.Add(new KeyValuePair<string, string>("results[ownOrRent]", "OWN"));
                            keyValues.Add(new KeyValuePair<string, string>("results[hasSmartMeterElec]", "NO"));
                            keyValues.Add(new KeyValuePair<string, string>("results[hasSmartMeterGas]", "NO"));
                            keyValues.Add(new KeyValuePair<string, string>("results[psrChoiceMain1]", ""));


                            utilityviewmodel.next_routine = "Get_Bank_Details";    //  Lower case? Next scrape is a POST
                        }
                        break;
                    case "Get_Bank_Details":
                        if (!Find_Wskey(utilityviewmodel, htmlDocument))//, rf wskey, rf wskey_name))
                        {
#if WINFORMS
                            if (utilityviewmodel.console)
                            {
                                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel,
                                        "Cannot find Web Service Key" + Environment.NewLine.ToString());
                            }
#endif
                            if (TextBox_Active)
                            {
                                await SmartRoutinesV2018.TextBlockUpdate(
#if ANDROIDX
                                    meterActivity,
#endif
                                    ourviewmodel,
                                    "CannotFindWebServiceKey" +
                                    "HeavyTick");

                            }

                            ourviewmodel.errorMessage = utilityviewmodel.current_routine;
                            return false;
                        }
                        else
                        {
#if WINFORMS
                            if (utilityviewmodel.console)
                            {
                                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel,
                                        "Web Service Key: " + utilityviewmodel.wskey + Environment.NewLine.ToString());
                            }
#endif
                            if (TextBox_Active)
                            {
                                await SmartRoutinesV2018.TextBlockUpdate(
#if ANDROIDX
                                    meterActivity,
#endif
                                    ourviewmodel,
                                    "WebServiceKey" +
                                    SmartParametersV2016.space +
                                    utilityviewmodel.wskey +
                                    "HeavyTick");
                            }

                            keyValues.Clear();

                            keyValues.Add(new KeyValuePair<string, string>("done", "signup_bankinfo"));
                            keyValues.Add(new KeyValuePair<string, string>("callcenter", ""));
                            keyValues.Add(new KeyValuePair<string, string>(utilityviewmodel.wskey_name, utilityviewmodel.wskey));
                            // This must not be empty!!
                            if (RAY.bank_account_name.Length > SmartParametersV2016.maximumBankAccountName)
                            {
                                keyValues.Add(new KeyValuePair<string, string>("results[accountName]", RAY.bank_account_name.Substring(0, SmartParametersV2016.maximumBankAccountName)));
                            }
                            else
                            {
                                keyValues.Add(new KeyValuePair<string, string>("results[accountName]", RAY.bank_account_name));
                            }
                            // Neithe must this!!
                            keyValues.Add(new KeyValuePair<string, string>("results[accountNumber]", RAY.bank_account_number.Substring(0, SmartParametersV2016.maximumBankAccountNumber)));
                            // Nor these!!!!
                            keyValues.Add(new KeyValuePair<string, string>("results[sortCode1]", RAY.bank_account_sort_code.Substring(0, 2)));
                            keyValues.Add(new KeyValuePair<string, string>("results[sortCode2]", RAY.bank_account_sort_code.Substring(2, 2)));
                            keyValues.Add(new KeyValuePair<string, string>("results[sortCode3]", RAY.bank_account_sort_code.Substring(4, 2)));
                            keyValues.Add(new KeyValuePair<string, string>("results[paymentDate]", "1"));
                            keyValues.Add(new KeyValuePair<string, string>("results[accountAuthorized]", "YES"));

                            utilityviewmodel.next_routine = "Confirm";   // Lower case? Next scrape is a POST
                                                                         // You ARE a fucking Genius, Ray!!
                        }
                        break;
                    case "Confirm":
                        
                        keyValues.Clear();

                        keyValues.Add(new KeyValuePair<string, string>("done", "signup_confirm"));

                        keyValues.Add(new KeyValuePair<string, string>("callcenter", ""));
                        if (!string.IsNullOrEmpty(RAY.mpan))
                        {
                            string mpan = RAY.mpan;
                            if (mpan.Length == 21)
                            {
                                keyValues.Add(new KeyValuePair<string, string>("results[mpanProfile]", mpan.Substring(0, 2)));
                                mpan = mpan.Substring(2);
                            }
                            else
                            {
                                keyValues.Add(new KeyValuePair<string, string>("results[mpanProfile]", ""));
                            }
                            if (mpan.Length == 19)
                            {
                                keyValues.Add(new KeyValuePair<string, string>("results[mpanTimeSwitch]", mpan.Substring(0, 3)));
                                mpan = mpan.Substring(3);
                            }
                            else
                            {
                                keyValues.Add(new KeyValuePair<string, string>("results[mpanTimeSwitch]", ""));

                            }
                            if (mpan.Length == 16)
                            {
                                keyValues.Add(new KeyValuePair<string, string>("results[mpanDuos]", mpan.Substring(0, 3)));
                                mpan = mpan.Substring(3);
                            }
                            else
                            {
                                keyValues.Add(new KeyValuePair<string, string>("results[mpanDuos]", ""));

                            }
                            if (mpan.Length == 13)
                            {
                                keyValues.Add(new KeyValuePair<string, string>("results[mpanDI]", mpan.Substring(0, 2)));
                                mpan = mpan.Substring(2);
                            }
                            else
                            {
                                keyValues.Add(new KeyValuePair<string, string>("results[mpanDI]", ""));
                            }
                            if (mpan.Length == 11)
                            {
                                keyValues.Add(new KeyValuePair<string, string>("results[mpanRefNo1]", mpan.Substring(0, 4)));
                                mpan = mpan.Substring(4);
                            }
                            else
                            {
                                keyValues.Add(new KeyValuePair<string, string>("results[mpanRefNo1]", ""));
                            }
                            if (mpan.Length == 7)
                            {
                                keyValues.Add(new KeyValuePair<string, string>("results[mpanRefNo2]", mpan.Substring(0, 4)));
                                mpan = mpan.Substring(4);
                            }
                            else
                            {
                                keyValues.Add(new KeyValuePair<string, string>("results[mpanRefNo2]", ""));
                            }
                            if (mpan.Length == 3)
                            {
                                keyValues.Add(new KeyValuePair<string, string>("results[mpanCheckDigits]", mpan.Substring(0, 3)));
                            }
                            else
                            {
                                mpan = "";
                            }
                        }
                        else
                        {
                            keyValues.Add(new KeyValuePair<string, string>("results[mpanProfile]", ""));
                            keyValues.Add(new KeyValuePair<string, string>("results[mpanTimeSwitch]", ""));
                            keyValues.Add(new KeyValuePair<string, string>("results[mpanDuos]", ""));
                            keyValues.Add(new KeyValuePair<string, string>("results[mpanDI]", ""));
                            keyValues.Add(new KeyValuePair<string, string>("results[mpanRefNo1]", ""));
                            keyValues.Add(new KeyValuePair<string, string>("results[mpanRefNo2]", ""));
                            keyValues.Add(new KeyValuePair<string, string>("results[mpanCheckDigits]", ""));

                        }
                        if (!string.IsNullOrEmpty(RAY.mprn))
                        {
                            keyValues.Add(new KeyValuePair<string, string>("results[mprn]", RAY.mprn));
                        }
                        else
                        {
                            keyValues.Add(new KeyValuePair<string, string>("results[mprn]", ""));
                        }
                        keyValues.Add(new KeyValuePair<string, string>("results[supplierTerms]", "YES"));
                        keyValues.Add(new KeyValuePair<string, string>("results[cancellationConsent]", "YES"));
                        keyValues.Add(new KeyValuePair<string, string>("results[obligationConsent]", "YES"));
                        keyValues.Add(new KeyValuePair<string, string>("results[supplierCreditCheck]", "YES"));
                        keyValues.Add(new KeyValuePair<string, string>("results[supplierMailingList]", "NO"));
                        keyValues.Add(new KeyValuePair<string, string>("results[supplierEmailingList]", "NO"));
                        keyValues.Add(new KeyValuePair<string, string>("results[supplierCalls]", "NO"));
                        keyValues.Add(new KeyValuePair<string, string>("results[supplierCallsMobile]", "NO"));
                        keyValues.Add(new KeyValuePair<string, string>("results[supplierText]", "NO"));

                        utilityviewmodel.next_routine = "Check_Signup";    // Upper case so next scrape is a POST
                        break;
                    case "Check_Signup":
                        utilityviewmodel.switcherdone = "";
                        if (Find_Done(utilityviewmodel, htmlDocument) &&
                            (utilityviewmodel.switcherdone == "signup_thankyou_review"))
                        {
#if WINFORMS
                            if (utilityviewmodel.console)
                            {
                                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel,
                                        "Confirmation : " + true.ToString() + Environment.NewLine.ToString());
                            }
#endif
                            if (TextBox_Active)
                            {
                                await SmartRoutinesV2018.TextBlockUpdate(
#if ANDROIDX
                                    meterActivity,
#endif
                                    ourviewmodel,
                                    "SwitchConfirmation" +
                                    SmartParametersV2016.space +
                                    "True");
                            }

                            status = true;
                            utilityviewmodel.next_routine = "LOGOUT";
                        }
                        else
                        {
#if WINFORMS
                            if (utilityviewmodel.console)
                            {
                                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel,
                                        "Confirmation : " + false.ToString() + Environment.NewLine.ToString());
                            }
#endif
                            if (TextBox_Active)
                            {
                                await SmartRoutinesV2018.TextBlockUpdate(
#if ANDROIDX
                                    meterActivity,
#endif
                                    ourviewmodel,
                                    "SwitchConfirmation" +
                                    SmartParametersV2016.space +
                                    "False");
                            }
                            ourviewmodel.errorMessage = utilityviewmodel.current_routine;
                            return false;
                        }
                        break;
                    case "LOGOUT":
                        keep_looping = false;
                        break;
                    default:
                        break;
                }
            }
            return status;
        }

        internal static bool Find_Wskey(UtilityViewModel utilityviewmodel,
                                        HtmlAgilityPack.HtmlDocument document)
        // rf string wskey,
        // rf string name)
        {
            IList<HtmlAgilityPack.HtmlNode>
                        HtmlCol1;
            utilityviewmodel.wskey = "";

            HtmlCol1 = YetAnotherFuckingHoop.SelectNodesAsList(document.DocumentNode, "//input");
            foreach (HtmlAgilityPack.HtmlNode element1 in HtmlCol1)
            {
                if (!string.IsNullOrEmpty(element1.Id))
                {
                    switch (element1.Id)
                    {
                        case "wsKey":
                            utilityviewmodel.wskey_name = element1.GetAttributeValue("name", "");
                            utilityviewmodel.wskey = element1.GetAttributeValue("value", "");
                            return true;
                        default:
                            break;
                    }
                }
            }
            return false;
        }

        internal static bool Find_Done(UtilityViewModel utilityviewmodel,
                                        HtmlAgilityPack.HtmlDocument document)
        {
            IList<HtmlAgilityPack.HtmlNode>
                        HtmlCol1;

            HtmlCol1 = YetAnotherFuckingHoop.SelectNodesAsList(document.DocumentNode, "//input");
            foreach (HtmlAgilityPack.HtmlNode element1 in HtmlCol1)
            {
                if (string.IsNullOrEmpty(element1.Id))
                {
                    switch (element1.Id)
                    {
                        case "done":
                            utilityviewmodel.switcherdone = element1.GetAttributeValue("value", "");
                            return true;
                        default:
                            break;
                    }
                }
            }
            return false;
        }

        //internal static bool GetSwitchNow(HtmlAgilityPack.HtmlDocument document,
        //                                    rf string switch_now_path)
        //{
        //    IList<HtmlAgilityPack.HtmlNode>
        //                HtmlCol1,
        //                HtmlCol2,
        //                HtmlCol3,
        //                HtmlCol4,
        //                HtmlCol5;
        //    switch_now_path = "";

        //    HtmlCol1 = YetAnotherFuckingHoop.SelectNodesAsList(document.DocumentNode, "//div");
        //    foreach (HtmlAgilityPack.HtmlNode element1 in HtmlCol1)
        //    {
        //        string classname = element1.GetAttributeValue("class", "");
        //        if (SmartNibbyV2016.Check_Classname(classname, "main-menu", true))
        //        {
        //            HtmlCol2 = YetAnotherFuckingHoop.SelectNodesAsList(element1, ".//div");
        //            foreach (HtmlAgilityPack.HtmlNode element2 in HtmlCol2)
        //            {
        //                classname = element2.GetAttributeValue("class", "");
        //                if (SmartNibbyV2016.Check_Classname(classname, "page-links", true))
        //                {
        //                    HtmlCol3 = YetAnotherFuckingHoop.SelectNodesAsList(element2, ".//ul");
        //                    foreach (HtmlAgilityPack.HtmlNode element3 in HtmlCol3)
        //                    {
        //                        HtmlCol4 = YetAnotherFuckingHoop.SelectNodesAsList(element3, ".//li");
        //                        foreach (HtmlAgilityPack.HtmlNode element4 in HtmlCol4)
        //                        {
        //                            if (!string.IsNullOrEmpty(element4.InnerText))
        //                            {
        //                                string inner_text = element4.InnerText;
        //                                switch (inner_text)
        //                                {
        //                                    case "Switch now":
        //                                        HtmlCol5 = YetAnotherFuckingHoop.SelectNodesAsList(element4, ".//a");
        //                                        foreach (HtmlAgilityPack.HtmlNode element5 in HtmlCol5)
        //                                        {
        //                                            string href = element5.GetAttributeValue(SmartParametersV2016.href, "");
        //                                            switch_now_path = href;
        //                                            return true;
        //                                        }
        //                                        break;
        //                                    default:
        //                                        break;
        //                                }
        //                            }
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    return false;
        //}

        internal static string RetrieveQuote(UtilityViewModel utilityviewmodel,
                                                HtmlAgilityPack.HtmlDocument document,
                                                string target_supplier_name,
                                                string target_tariff_name)
        //rf string proceed_online_path)
        {
            IList<HtmlAgilityPack.HtmlNode>
                        HtmlCol1,
                        HtmlCol2,
                        HtmlCol3,
                        HtmlCol4,
                        HtmlCol5,
                        HtmlCol6,
                        HtmlCol7;
            utilityviewmodel.proceed_online_path = "";

            HtmlCol1 = YetAnotherFuckingHoop.SelectNodesAsList(document.DocumentNode, "//div");
            foreach (HtmlAgilityPack.HtmlNode element1 in HtmlCol1)
            {
                if (!string.IsNullOrEmpty(element1.Id))
                {
                    if (element1.Id == "mainbg")
                    {
                        HtmlCol2 = YetAnotherFuckingHoop.SelectNodesAsList(element1, ".//div");
                        foreach (HtmlAgilityPack.HtmlNode element2 in HtmlCol2)
                        {
                            if (!string.IsNullOrEmpty(element2.Id))
                            {
                                if (element2.Id == "accordion") // Not accordian you dick-head
                                {
                                    string classname = element2.GetAttributeValue("class", "");
                                    if (SmartNibbyV2016.Check_Classname(classname, "container", true))
                                    {
                                        HtmlCol3 = YetAnotherFuckingHoop.SelectNodesAsList(element2, ".//div");
                                        foreach (HtmlAgilityPack.HtmlNode element3 in HtmlCol3)
                                        {
                                            if (!string.IsNullOrEmpty(element3.Id))
                                            {
                                                if (element3.Id.Contains("panel"))
                                                {
                                                    string data_supplier_name = element3.GetAttributeValue("data-supplier-name", "");
                                                    if (SmartNibbyV2016.Check_Classname(data_supplier_name, target_supplier_name, true))
                                                    {
                                                        HtmlCol4 = YetAnotherFuckingHoop.SelectNodesAsList(element3, ".//div");
                                                        foreach (HtmlAgilityPack.HtmlNode element4 in HtmlCol4)
                                                        {
                                                            classname = element4.GetAttributeValue("class", "");
                                                            if (SmartNibbyV2016.Check_Classname(classname, "results-wrapper", true))
                                                            {
                                                                bool found_it = false;
                                                                HtmlCol5 = YetAnotherFuckingHoop.SelectNodesAsList(element4, ".//div");
                                                                foreach (HtmlAgilityPack.HtmlNode element5 in HtmlCol5)
                                                                {
                                                                    classname = element5.GetAttributeValue("class", "");
                                                                    switch (classname)
                                                                    {
                                                                        case "tariff-name":
                                                                            if (!string.IsNullOrEmpty(element5.InnerText))
                                                                            {
                                                                                string tariff_name = element5.InnerText.Trim();
                                                                                if (tariff_name == target_tariff_name)
                                                                                {
                                                                                    found_it = true;
                                                                                }
                                                                            }
                                                                            break;
                                                                        case "columns-wrapper":
                                                                            if (found_it)
                                                                            {
                                                                                HtmlCol6 = YetAnotherFuckingHoop.SelectNodesAsList(element5, ".//div");
                                                                                foreach (HtmlAgilityPack.HtmlNode element6 in HtmlCol6)
                                                                                {
                                                                                    // Choose the Online button
                                                                                    classname = element6.GetAttributeValue("class", "");
                                                                                    if (SmartNibbyV2016.Check_Classname(classname, "buttons", true))
                                                                                    {
                                                                                        HtmlCol7 = YetAnotherFuckingHoop.SelectNodesAsList(element6, ".//a");
                                                                                        foreach (HtmlAgilityPack.HtmlNode element7 in HtmlCol7)
                                                                                        {
                                                                                            if (!string.IsNullOrEmpty(element7.InnerText))
                                                                                            {
                                                                                                if (element7.InnerText.Contains("Proceed online"))
                                                                                                {
                                                                                                    string href = element7.GetAttributeValue(SmartParametersV2016.href, "");
                                                                                                    //string data_signuplink = element7.GetAttributeValue("data-signuplink", "");
                                                                                                    if (!string.IsNullOrEmpty(href))
                                                                                                    {
                                                                                                        return href;
                                                                                                    }
                                                                                                }
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
            return "";
        }

        //internal static bool FillDetails(HtmlAgilityPack.HtmlDocument document,
        //                                        rf string authenticity_token,
        //                                        rf string signup_code)
        //{
        //    IList<HtmlAgilityPack.HtmlNode>
        //                HtmlCol1;
        //    authenticity_token = signup_code = "";

        //    HtmlCol1 = YetAnotherFuckingHoop.SelectNodesAsList(document.DocumentNode, "//input");
        //    foreach (HtmlAgilityPack.HtmlNode element1 in HtmlCol1)
        //    {
        //        if (!string.IsNullOrEmpty(element1.Id))
        //        {
        //            switch (element1.Id)
        //            {
        //                case "signup_code":
        //                    signup_code = element1.GetAttributeValue("value", "");
        //                    break;
        //                default:
        //                    break;
        //            }
        //        }
        //        else
        //        {
        //            string name = element1.GetAttributeValue("name", "");
        //            switch (name)
        //            {
        //                case "authenticity_token":
        //                    authenticity_token = element1.GetAttributeValue("value", "");
        //                    break;
        //                default:
        //                    break;
        //            }
        //        }

        //        if (!string.IsNullOrEmpty(authenticity_token) &&
        //            !string.IsNullOrEmpty(signup_code))
        //        {
        //            return true;

        //        }

        //    }
        //    return false;
        //}

        //internal static bool FillDetails2(HtmlAgilityPack.HtmlDocument document,
        //                                        rf string authenticity_token)
        //{
        //    IList<HtmlAgilityPack.HtmlNode>
        //                HtmlCol1;
        //    authenticity_token = "";

        //    HtmlCol1 = YetAnotherFuckingHoop.SelectNodesAsList(document.DocumentNode, "//input");
        //    foreach (HtmlAgilityPack.HtmlNode element1 in HtmlCol1)
        //    {
        //        if (string.IsNullOrEmpty(element1.Id))
        //        {
        //            string name = element1.GetAttributeValue("name", "");
        //            switch (name)
        //            {
        //                case "authenticity_token":
        //                    authenticity_token = element1.GetAttributeValue("value", "");
        //                    return true;
        //                default:
        //                    break;
        //            }
        //        }

        //    }
        //    return false;
        //}

        //internal static bool FillDetails3(HtmlAgilityPack.HtmlDocument document,
        //                                        rf string authenticity_token,
        //                                        rf string vanity_address_sub_building_name,
        //                                        rf string vanity_address_building_number,
        //                                        rf string vanity_address_building_name,
        //                                        rf string vanity_address_thoroughfare,
        //                                        rf string vanity_address_dependant_thoroughfare,
        //                                        rf string vanity_address_dependant_locality,
        //                                        rf string vanity_address_double_dependant_locality,
        //                                        rf string vanity_address_locality,
        //                                        rf string vanity_address_postcode,
        //                                        rf string fm_county,
        //                                        rf string fm_country,
        //                                        rf string vanity_address_udprn)
        //{
        //    IList<HtmlAgilityPack.HtmlNode>
        //                HtmlCol1;
        //    authenticity_token = vanity_address_udprn = "";

        //    HtmlCol1 = YetAnotherFuckingHoop.SelectNodesAsList(document.DocumentNode, "//input");
        //    foreach (HtmlAgilityPack.HtmlNode element1 in HtmlCol1)
        //    {
        //        if (!string.IsNullOrEmpty(element1.Id))
        //        {
        //            switch (element1.Id)
        //            {
        //                case "vanity_address_sub_building_name":
        //                    vanity_address_sub_building_name = element1.GetAttributeValue("value", "");
        //                    break;
        //                case "vanity_address_building_number":
        //                    vanity_address_building_number = element1.GetAttributeValue("value", "");
        //                    break;
        //                case "vanity_address_building_name":
        //                    vanity_address_building_name = element1.GetAttributeValue("value", "");
        //                    break;
        //                case "vanity_address_thoroughfare":
        //                    vanity_address_thoroughfare = element1.GetAttributeValue("value", "");
        //                    break;
        //                case "vanity_address_dependant_thoroughfare":
        //                    vanity_address_dependant_thoroughfare = element1.GetAttributeValue("value", "");
        //                    break;
        //                case "vanity_address_dependant_locality":
        //                    vanity_address_dependant_locality = element1.GetAttributeValue("value", "");
        //                    break;
        //                case "vanity_address_double_dependant_locality":
        //                    vanity_address_double_dependant_locality = element1.GetAttributeValue("value", "");
        //                    break;
        //                case "vanity_address_locality":
        //                    vanity_address_locality = element1.GetAttributeValue("value", "");
        //                    break;
        //                case "vanity_address_postcode":
        //                    vanity_address_postcode = element1.GetAttributeValue("value", "");
        //                    break;
        //                case "fm-county":
        //                    fm_county = element1.GetAttributeValue("value", "");
        //                    break;
        //                case "fm-country":
        //                    fm_country = element1.GetAttributeValue("value", "");
        //                    break;
        //                case "vanity_address_udprn":
        //                    vanity_address_udprn = element1.GetAttributeValue("value", "");
        //                    if (string.IsNullOrEmpty(vanity_address_udprn))
        //                    {
        //                        vanity_address_udprn = "Empty";
        //                    }
        //                    break;
        //                default:
        //                    break;
        //            }
        //        }
        //        else
        //        {
        //            string name = element1.GetAttributeValue("name", "");
        //            switch (name)
        //            {
        //                case "authenticity_token":
        //                    authenticity_token = element1.GetAttributeValue("value", "");
        //                    break;
        //                default:
        //                    break;
        //            }
        //        }

        //        if (!string.IsNullOrEmpty(authenticity_token) &&
        //            !string.IsNullOrEmpty(vanity_address_udprn))
        //        {
        //            if (vanity_address_udprn == "Empty")
        //            {
        //                vanity_address_udprn = "";
        //            }
        //            return true;

        //        }

        //    }
        //    return false;
        //}

        //internal static bool FillDetails4(HtmlAgilityPack.HtmlDocument document,
        //                                        rf string x_csrf_token,
        //                                        rf string authenticity_token)
        //{
        //    IList<HtmlAgilityPack.HtmlNode>
        //                HtmlCol1;
        //    x_csrf_token = authenticity_token = "";

        //    HtmlCol1 = YetAnotherFuckingHoop.SelectNodesAsList(document.DocumentNode, "//meta");
        //    foreach (HtmlAgilityPack.HtmlNode element1 in HtmlCol1)
        //    {
        //        string name = element1.GetAttributeValue("name", "");
        //        switch (name)
        //        {
        //            case "csrf-token":
        //                x_csrf_token = element1.GetAttributeValue("content", "");
        //                break;
        //            default:
        //                break;
        //        }
        //    }

        //    HtmlCol1 = YetAnotherFuckingHoop.SelectNodesAsList(document.DocumentNode, "//input");
        //    foreach (HtmlAgilityPack.HtmlNode element1 in HtmlCol1)
        //    {
        //        if (string.IsNullOrEmpty(element1.Id))
        //        {

        //            string name = element1.GetAttributeValue("name", "");
        //            switch (name)
        //            {
        //                case "authenticity_token":
        //                    authenticity_token = element1.GetAttributeValue("value", "");
        //                    break;
        //                default:
        //                    break;
        //            }
        //        }
        //    }

        //    if (!string.IsNullOrEmpty(x_csrf_token) &&
        //        !string.IsNullOrEmpty(authenticity_token))
        //    {
        //        return true;
        //    }

        //    return false;
        //}

        //internal static bool CheckConfirm(HtmlAgilityPack.HtmlDocument document)
        //{
        //    IList<HtmlAgilityPack.HtmlNode>
        //                HtmlCol1,
        //                HtmlCol2;
        //    bool three_found = false;

        //    HtmlCol1 = YetAnotherFuckingHoop.SelectNodesAsList(document.DocumentNode, "//div");
        //    foreach (HtmlAgilityPack.HtmlNode element1 in HtmlCol1)
        //    {
        //        string classname = element1.GetAttributeValue("class", "");
        //        if (SmartNibbyV2016.Check_Classname(classname, "step", true))
        //        {
        //            HtmlCol2 = YetAnotherFuckingHoop.SelectNodesAsList(element1, ".//div");
        //            foreach (HtmlAgilityPack.HtmlNode element2 in HtmlCol2)
        //            {
        //                classname = element2.GetAttributeValue("class", "");
        //                if (SmartNibbyV2016.Check_Classname(classname, "number", true))
        //                {
        //                    if (!string.IsNullOrEmpty(element2.InnerText))
        //                    {
        //                        string inner_text = element2.InnerText.Trim();
        //                        if (inner_text == "3")
        //                        {
        //                            three_found = true;
        //                        }
        //                    }
        //                }

        //                if (three_found)
        //                {
        //                    if (SmartNibbyV2016.Check_Classname(classname, "content", true))
        //                    {
        //                        if (!string.IsNullOrEmpty(element2.InnerText))
        //                        {
        //                            if (element2.InnerText.IndexOf("You are now supplied by Powershop") >= 0)
        //                            {
        //                                return true;
        //                            }
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    return false;
        //}


    }
}
