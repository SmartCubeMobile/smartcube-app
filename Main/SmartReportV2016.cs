using System;
using System.Collections.Generic;
using System.Linq;

namespace SmartCubeMobile
{
    public class SmartReportV2016
    {
        
        internal static string Fill_Institution_Report(FinanceViewModel financeviewmodel,
                                                    string institution_name)
        {
            string report_box = "";
            report_box += Environment.NewLine;

            List<SmartFinance.InstitutionInfo> institution_info_found = new List<SmartFinance.InstitutionInfo>(from Institution_Information
                                                                    in financeviewmodel.PLO.institution_infoList
                                                                                                               where (Institution_Information.INSTITUTION_NAME == institution_name)
                                                                                                               select Institution_Information);
            if (institution_info_found.Count > 0)
            {
                foreach (SmartFinance.InstitutionInfo institution_information_row in institution_info_found)
                {
                    if (!string.IsNullOrEmpty(institution_information_row.INSTITUTION_NAME))
                    {
                        report_box += institution_information_row.DETAILS.Replace("|", Environment.NewLine);
                        break;
                    }
                }
            }
            else
            {
                report_box += "SmartSwitch has no information about this Institution at this present time";
            }
            return report_box;
        }

        internal static string Fill_Transaction_Report(string brand_name,
                                                string sortcode,
                                                string accountno,
                                                string transaction_date,
                                                string description,
                                                int credit_debit,          // Credit = 1 true, Debit = 0 false
                                                string amount,
                                                //string currency,
                                                string balance)
        {
            string newline = Environment.NewLine;
            string report_box = "";

            //report_box += "Area\t\t: " + Report_Lookup_Area_Name(area_code, financeviewmodel.PLO.supply_areasList) + " (" + area_code.ToString("00") + ")" + newline;
            report_box += "Institution\t\t: " + brand_name + newline;
            report_box += "Sort Code\t\t: " + sortcode + newline;
            report_box += "Account No\t: " + accountno + newline;
            report_box += "Date\t\t: " + transaction_date + newline;
            report_box += "Description\t: " + description + newline;
            report_box += "Amount\t\t: " + amount;
            if (credit_debit == 1)
            {
                report_box += " CREDIT ";
            }
            else
            {
                report_box += " DEBIT ";
            }
            //report_box += "(" + currency + ")" + newline;
            report_box += "Balance\t\t: " + balance + newline;
            report_box += newline;
            return report_box;
        }
        internal static string Fill_Email_Report(DateTime time_now,
                                                char resource_code,
                                                string brand_name,
                                                string tariff_name,
                                                string payment_method,
                                                List<SmartUtility.Templates> templatesList,
                                                string my_name,
                                                string my_address,
                                                string my_postcode,
                                                string my_contact_phone,
                                                string my_email_address,
                                                string old_supplier_name,
                                                string old_tariff_name,
                                                string old_payment_plan,
                                                string mpan_mprn,
                                                string elec_account_no,
                                                string elec_meter_type,
                                                string gas_account_no,
                                                string meter_serial_no,
                                                string READINGS_PERIOD_END,
                                                string d_this_read,
                                                string n_this_read,
                                                string adminEmailAddress)
        {
            string email_box = "",
                    electricity_box = "",
                    gas_box = "";
            
            email_box += Environment.NewLine;

            List<SmartUtility.Templates> templates_found = new List<SmartUtility.Templates>(from Template in templatesList
                                                                                            where (Template.WHAT == "EMAIL")
                                                                                            select Template);
            if (templates_found.Count > 0)
            {
                if (!string.IsNullOrEmpty(templates_found[0].TEXT))
                {
                    // 27-May-2017 ... She is a 'WAG'  (Without-A-Grunt!!!)
                    email_box = templates_found[0].TEXT;
                    email_box = email_box.Replace("<new_supplier_name>", brand_name);
                    email_box = email_box.Replace("<new_tariff_name>", tariff_name);
                    email_box = email_box.Replace("<new_payment_method>", payment_method);
                    email_box = email_box.Replace("<my_name>", my_name);
                    email_box = email_box.Replace("<my_address>", my_address.Replace(SmartParametersV2016.bar.ToString(), "  "));
                    email_box = email_box.Replace("<my_postcode>", my_postcode);
                    email_box = email_box.Replace("<my_contact_phone>", my_contact_phone);
                    email_box = email_box.Replace("<my_email_address>", my_email_address);
                    email_box = email_box.Replace("<old_supplier_name>", old_supplier_name);
                    email_box = email_box.Replace("<old_tariff_name>", old_tariff_name);
                    email_box = email_box.Replace("<old_payment_method>", old_payment_plan);
                    email_box = email_box.Replace("<todays_date>", time_now.ToString(SmartParametersV2016.ddmmmyyyyFormat));
                    email_box = email_box.Replace("<adminEmailAddress>", adminEmailAddress);
                }
            }

            switch (resource_code)
            {
                case SmartParametersV2016.Electricity:
                    templates_found = new List<SmartUtility.Templates>(from Template
                                        in templatesList
                                                                       where (Template.WHAT == "ELECTRICITY")
                                                                       select Template);
                    if (templates_found.Count > 0)
                    {
                        if (!string.IsNullOrEmpty(templates_found[0].TEXT))
                        {
                            // 27-May-2017 ... She is a 'WAG'  (Without-A-Grunt!!!)
                            electricity_box = templates_found[0].TEXT;
                            electricity_box = electricity_box.Replace("<elec_account_no>", elec_account_no);
                            electricity_box = electricity_box.Replace("<elec_mpan>", mpan_mprn);
                            electricity_box = electricity_box.Replace("<elec_meter_serial_no>", meter_serial_no);
                            electricity_box = electricity_box.Replace("<elec_meter_type>", elec_meter_type);
                            electricity_box = electricity_box.Replace("<elec_reading>", d_this_read);
                            if (!string.IsNullOrEmpty(n_this_read))
                            {
                                electricity_box = electricity_box.Replace("<elec_reading_night>", n_this_read);

                            }
                            electricity_box = electricity_box.Replace("<elec_date>", READINGS_PERIOD_END);
                            email_box = email_box.Replace("<ELECTRICITY>", electricity_box);
                            email_box = email_box.Replace("<GAS>", "");
                        }
                    }
                    break;
                case SmartParametersV2016.Gas:
                    templates_found = new List<SmartUtility.Templates>(from Template
                                        in templatesList
                                                                       where (Template.WHAT == "GAS")
                                                                       select Template);
                    if (templates_found.Count > 0)
                    {
                        if (!string.IsNullOrEmpty(templates_found[0].TEXT))
                        {
                            // 27-May-2017 ... She is a 'WAG'  (Without-A-Grunt!!!)
                            gas_box = templates_found[0].TEXT;
                            gas_box = gas_box.Replace("<gas_account_no>", gas_account_no);
                            gas_box = gas_box.Replace("<gas_mprn>", mpan_mprn);
                            gas_box = gas_box.Replace("<gas_meter_serial_no>", meter_serial_no);
                            gas_box = gas_box.Replace("<gas_reading>", d_this_read);
                            gas_box = gas_box.Replace("<gas_date>", READINGS_PERIOD_END);
                            email_box = email_box.Replace("<GAS>", gas_box);
                            email_box = email_box.Replace("<ELECTRICITY>", "");
                        }
                    }
                    break;
                default:
                    break;
            }
            return email_box;
        }

        internal static string Fill_Supplier_Report(UtilityViewModel utilityviewmodel,
                                                    string brand_name)
        {
            string report_box = "";
            report_box += Environment.NewLine;

            List<SmartUtility.SupplierInfo> supplier_info_found = new List<SmartUtility.SupplierInfo>(from Supplier_Information
                                                                    in utilityviewmodel.Hezbollah.supplier_infoList
                                                                                                      where (Supplier_Information.SUPPLIER_NAME == brand_name)
                                                                                                      select Supplier_Information);
            if (supplier_info_found.Count > 0)
            {
                foreach (SmartUtility.SupplierInfo supplier_information_row in supplier_info_found)
                {
                    if (!string.IsNullOrEmpty(supplier_information_row.SUPPLIER_ID))
                    {
                        report_box += supplier_information_row.DETAILS.Replace("|", Environment.NewLine);
                        break;
                    }
                }
            }
            else
            {
                report_box += "SmartSwitch has no information about this Supplier at this present time";
            }
            return report_box;
        }




        internal static string Fill_Tariff_Report(MainViewModel ourviewmodel,
                                                UtilityViewModel utilityviewmodel,
                                                char resource_code,
                                                string brand_name,
                                                short supplier_code,
                                                int tariff_code,
                                                string resource_type,
#if WINFORMS
                                                DateTime prices_valid_at)
#endif
#if WPF  || WINUI || SMARTMAUI
                                                DateTime prices_valid_at)
#endif
#if ANDROIDX
                                                DateTime prices_valid_at)
#endif
        {
            string newline = Environment.NewLine;

            utilityviewmodel.reportPaymentPlans = "";
            utilityviewmodel.report_box = "";

            Report_Utility_Resource(utilityviewmodel, resource_code, newline);

            utilityviewmodel.report_box += "Area\t\t: " + Report_Lookup_Area_Name(utilityviewmodel.area_code, utilityviewmodel.Hezbollah.supply_areasList) + " (" + utilityviewmodel.area_code.ToString("00") + ")" + newline;

            List<SmartUtility.Suppliers> suppliers_found = SmartSpikeUtilityV2017.Utility_Lookup_Suppliers(ourviewmodel,
                                                                        utilityviewmodel,
                                                                        supplier_code);
            foreach (SmartUtility.Suppliers supplier_row in suppliers_found)
            {
                // Look for all the Tariffs in this Supplier
                // Tariffs are Supplier specific .....
                List<SmartUtility.Tariffs> tariffs_found = SmartSpikeUtilityV2017.Utility_TariffsList(ourviewmodel,
                                                                        utilityviewmodel,
                                                                        supplier_row.SUPPLIER_CODE,
                                                                        resource_code,
                                                                        resource_type,
                                                                        tariff_code);
                foreach (SmartUtility.Tariffs tariff_row in tariffs_found)
                {
                    Report_Tariff_Type(utilityviewmodel, tariff_row, newline);

                    // Pre-Conditions
                    Report_Pre_Conditions(utilityviewmodel,
                                            resource_code,
                                            resource_type,
                                            supplier_row.SUPPLIER_CODE,
                                            tariff_code,
                                            newline,
                                            tariff_row);

                    // Prices valid from
                    utilityviewmodel.report_box += "Prices valid at\t: " + prices_valid_at + newline;

                    // Do any Payment Plans
                    List<SmartUtility.ConditionsPlans> conditions_plans_found = SmartSpikeUtilityV2017.Utility_ConditionsPlans(ourviewmodel,
                                                                                                    utilityviewmodel,
                                                                                                    supplier_row.SUPPLIER_CODE,
                                                                                                    tariff_row.RESOURCE_CODE,
                                                                                                    tariff_row.RESOURCE_TYPE,
                                                                                                    tariff_row.TARIFF_CODE,
                                                                                                    SmartParametersV2016.defaultChar);
                    foreach (SmartUtility.ConditionsPlans conditions_plans_row in conditions_plans_found)
                    {
                        List<SmartUtility.ConditionsView> conditions_view_found = SmartSpikeUtilityV2017.Utility_ConditionsView(ourviewmodel,
                                                                                                        utilityviewmodel,
                                                                                                        conditions_plans_row.SUPPLIER_CODE,
                                                                                                        conditions_plans_row.RESOURCE_CODE,
                                                                                                        conditions_plans_row.RESOURCE_TYPE,
                                                                                                        conditions_plans_row.TARIFF_CODE,
                                                                                                        conditions_plans_row.PAYMENT_CODE,
                                                                                                        conditions_plans_row.VERSION_CODE);
                        string current_plan = "";
                        if (conditions_view_found.Count > 0)
                        {
                            bool price_already_found = false;
                            foreach (SmartUtility.ConditionsView conditions_view_row in conditions_view_found)
                            {
                                if (SmartAnalyzeV2016.Special_Date_Compare(price_already_found,
#if WINFORMS
                                                                            prices_valid_at,
#endif
#if WPF || WINUI || SMARTMAUI
                                                                            prices_valid_at, 
#endif
#if ANDROIDX
                                                                            prices_valid_at,
#endif
                                                                            conditions_view_row.PRICES_VALID_FROM))
                                {
                                    price_already_found = true;
                                    DateTime prices_valid_from = conditions_view_row.PRICES_VALID_FROM;

                                    //char e7 = ' ';
                                    //if (economy7)
                                    //{
                                    //    e7 = 'Y';
                                    //}
                                    //else
                                    //{
                                    //    e7 = 'N';
                                    //}
                                    // SUPPLIER_CODE TARIFF_CODE PAYMENT_CODE GROUP_CODE AREA_CODE ECONOMY7 VALID_FROM DATE
                                    // DR NR SC

                                    // Doesn't matter if no Groups are found
                                    utilityviewmodel.reportStandingCharge = "";
                                    utilityviewmodel.reportTcr = "";
                                    string indent = "    ";

                                    Report_Unit_Rates(utilityviewmodel,
                                                        utilityviewmodel.area_code,
                                                        resource_code,
                                                        conditions_view_row.SUPPLIER_CODE,
                                                        conditions_view_row.RESOURCE_CODE,
                                                        conditions_view_row.RESOURCE_TYPE,
                                                        conditions_view_row.TARIFF_CODE,
                                                        conditions_view_row.PAYMENT_CODE,
                                                        conditions_view_row.TIER_COUNT,
                                                        conditions_view_row.TIER_LEVEL,
                                                        prices_valid_from,
                                                        newline,
                                                        indent,
                                                        conditions_plans_row);

                                    // On to do the conditions and groupings
                                    List<SmartUtility.ConditionsGroups> conditions_groups_found = SmartSpikeUtilityV2017.Utility_Find_ConditionsGroups(ourviewmodel,
                                                                                                                        utilityviewmodel,
                                                                                                                        conditions_view_row.SUPPLIER_CODE,
                                                                                                                        conditions_view_row.TARIFF_CODE,
                                                                                                                        conditions_view_row.RESOURCE_CODE,
                                                                                                                        conditions_view_row.RESOURCE_TYPE,
                                                                                                                        conditions_view_row.VERSION_CODE,
                                                                                                                        conditions_view_row.PAYMENT_CODE,
                                                                                                                        conditions_view_row.TIER_COUNT);
                                    utilityviewmodel.reportBandLimit = false;

                                    // Now do the Standing Charge 10-Jan-2022 but only if we HAVE one
                                    if (!string.IsNullOrEmpty(utilityviewmodel.reportStandingCharge))
                                    {
                                        foreach (SmartUtility.ConditionsGroups conditions_groups_row in conditions_groups_found)
                                        {
                                            // Now do the Groupings Limits .. 10-Jan-2022 but only if we HAVE a standing charge
                                            Report_Condition_Limits(utilityviewmodel,
                                                        utilityviewmodel.area_code,
                                                        conditions_groups_row.SUPPLIER_CODE,
                                                        conditions_groups_row.RESOURCE_CODE,
                                                        conditions_groups_row.RESOURCE_TYPE,
                                                        conditions_groups_row.LIMIT_CODE,
                                                        newline,
                                                        indent,
                                                        utilityviewmodel.reportStandingCharge);
                                        }
                                    }
                                    // Now do the TCR but only if we HAVE one
                                    if (!string.IsNullOrEmpty(utilityviewmodel.reportTcr))
                                    {
                                        utilityviewmodel.report_box += newline +
                                            indent + "TCR: " + utilityviewmodel.reportTcr + "p per kWh";
                                    }
                                }
                            }

                            // I AM GETTING THERE
                            // Still within loop on Payment Plans
                            // Do any Post-Conditions ...  
                            if (!Report_Post_Conditions(utilityviewmodel,
                                                        resource_code,
                                                        brand_name,
                                                        newline,
                                                        current_plan,
                                                        tariff_row))
                            {
                                return utilityviewmodel.report_box;
                            }
                            utilityviewmodel.report_box += newline;
                        }
                    }
                }
            }
            utilityviewmodel.report_box += newline;
            utilityviewmodel.report_box += "(All rates and prices exclude VAT)";
            return utilityviewmodel.report_box;
        }

        internal static string Report_Finance_Resource(char resource_code)
        {
            string report_box = "";
            switch (resource_code)
            {
                case SmartParametersV2016.Banks:
                    report_box = "BANK";
                    break;
                case SmartParametersV2016.Savings:
                    report_box = "SAVINGS";
                    break;
                case SmartParametersV2016.Investments:
                    report_box = "INVESTMENTS";
                    break;
                default:
                    break;
            }
            return report_box;
        }

        internal static void Report_Utility_Resource(UtilityViewModel utilityviewmodel,
                                                    char resource_code,
                                                    string newline)
        {
            switch (resource_code)
            {
                case SmartParametersV2016.Electricity:
                    utilityviewmodel.report_box = "ELECTRICITY";
                    break;
                case SmartParametersV2016.Gas:
                    utilityviewmodel.report_box = "GAS";
                    break;
                default:
                    break;
            }
            utilityviewmodel.report_box += newline;
            return;
        }

        internal static void Report_Tariff_Type(UtilityViewModel utilityviewmodel,
                                                SmartUtility.Tariffs tariff_row,
                                                string newline)
        {
            // Launch date
            utilityviewmodel.report_box += "Launched\t: " + tariff_row.LAUNCHED.ToString(SmartParametersV2016.ddmmmyyyyFormat) + newline;

            // Issued date
            utilityviewmodel.report_box += "Issued\t\t: " + tariff_row.ISSUED.ToString(SmartParametersV2016.ddmmmyyyyFormat) + newline;
            // Validity dates
            utilityviewmodel.report_box += "Valid from\t: " + tariff_row.PRICES_VALID_FROM.ToString(SmartParametersV2016.ddmmmyyyyFormat) + newline;
            utilityviewmodel.report_box += "Valid to\t\t: " + tariff_row.VALID_TO.ToString(SmartParametersV2016.ddmmmyyyyFormat) + newline;
            // Tariff type
            utilityviewmodel.report_box += "Tariff type\t: ";
            switch (tariff_row.TARIFF_TYPE)
            {
                case "Y":
                    utilityviewmodel.report_box += "Variable" + newline;
                    break;
                default:
                    utilityviewmodel.report_box += "Fixed" + newline;
                    break;
            }
            return;
        }
        internal static void Report_Pre_Conditions(UtilityViewModel utilityviewmodel,
                                            char resource_code,
                                            string resource_type,
                                            short supplier_code,
                                            int tariff_code,
                                            string newline,
                                            SmartUtility.Tariffs tariff_row)
        {
            if (resource_code == SmartParametersV2016.Electricity &&
                resource_type == "SR")
            {
                List<SmartUtility.Tariffs> e7_found = new List<SmartUtility.Tariffs>(from Tariff
                                            in utilityviewmodel.Hezbollah.tariffsList
                                                                                     where ((Tariff.SUPPLIER_CODE == supplier_code) &&
                                                                                             (Tariff.RESOURCE_CODE == resource_code) &&
                                                                                             (Tariff.RESOURCE_TYPE == "VR") &&
                                                                                             (Tariff.TARIFF_CODE == tariff_code))
                                                                                     select Tariff);
                if (e7_found.Count > 0)
                {
                    utilityviewmodel.report_box += "Economy7:\t" + "Possible" + newline;
                    //pre_conditions = true;
                }
            }

            utilityviewmodel.report_box += newline + "Conditions\t: ";

            bool pre_conditions = false;

            string conditionList = "";
            switch (tariff_row.AVAILABILITY)
            {
                case "O":
                    conditionList = "Online";
                    break;
                case "F":
                    conditionList = "Offline";
                    break;
                case "B":
                    conditionList = "Online and Offline";
                    break;
                default:
                    break;
            }
            if (!string.IsNullOrEmpty(conditionList))
            {
                pre_conditions = true;
            }
            // Age
            if (tariff_row.AGE != "0")
            {
                if (!string.IsNullOrEmpty(conditionList))
                {
                    conditionList += " / ";
                }
                conditionList += "Age limit " + tariff_row.AGE + " and above";
                pre_conditions = true;
            }
            // Customer
            switch (tariff_row.CUSTOMER)
            {
                case "Y":
                    if (!string.IsNullOrEmpty(conditionList))
                    {
                        conditionList += " / ";
                    }
                    conditionList += "Customer Only";
                    pre_conditions = true;
                    break;
                case "N":
                    break;
                default:
                    break;
            }
            if (!pre_conditions)
            {
                utilityviewmodel.report_box += "None";
            }
            else
            {
                utilityviewmodel.report_box += conditionList;
            }
            utilityviewmodel.report_box += newline;
            return;
        }

        internal static void Report_Condition_Limits(UtilityViewModel utilityviewmodel,
                                                    short area_code,
                                                    short supplier_code,
                                                    char resource_code,
                                                    string resource_type,
                                                    short limit_code,
                                                    string newline,
                                                    string indent,
                                                    string standing_charge)
        {
            List<SmartUtility.ConditionsLimits> conditions_limits_found = new List<SmartUtility.ConditionsLimits>(from Conditions_Limit
                                                               in utilityviewmodel.Hezbollah.conditions_limitsList
                                                                                                                  where ((Conditions_Limit.SUPPLIER_CODE == supplier_code) && //conditions_groups_row.SUPPLIER_CODE) &&
                                                                                                                          (Conditions_Limit.RESOURCE_CODE == resource_code) && //conditions_groups_row.RESOURCE_CODE) &&
                                                                                                                          (Conditions_Limit.RESOURCE_TYPE == resource_type) && //conditions_groups_row.RESOURCE_TYPE) &&
                                                                                                                          (Conditions_Limit.LIMIT_CODE == limit_code))
                                                                                                                  select Conditions_Limit);
            foreach (SmartUtility.ConditionsLimits conditions_limits_row in conditions_limits_found)
            {
                // Find out which ones to tick
                switch (conditions_limits_row.LIMIT_TYPE)
                {
                    case "SC":
                        Report_Case_SC(utilityviewmodel,
                                            newline,
                                            indent,
                                            conditions_limits_row.LOWER_LIMIT,
                                            conditions_limits_row.LOWER_SYMBOL,
                                            conditions_limits_row.UNITS,
                                            standing_charge);
                        break;
                    case "BL":
                        Report_Case_BL(utilityviewmodel,
                                            area_code,
                                            conditions_limits_row.LOWER_LIMIT,
                                            conditions_limits_row.UPPER_LIMIT,
                                            conditions_limits_row.LOWER_SYMBOL,
                                            conditions_limits_row.UPPER_SYMBOL,
                                            conditions_limits_row.UNITS,
                                            conditions_limits_row.FIELD_NAME,
                                            conditions_limits_row);

                        break;
                    case "TL":
                        Report_Case_TL(utilityviewmodel,
                                            conditions_limits_row.LOWER_LIMIT,
                                            conditions_limits_row.UPPER_LIMIT,
                                            conditions_limits_row.LOWER_SYMBOL,
                                            conditions_limits_row.UPPER_SYMBOL,
                                            conditions_limits_row.UNITS);

                        break;
                    default:
                        break;
                }
            }
            return;
        }

        internal static void Report_Case_TL(UtilityViewModel utilityviewmodel,
                                            string lower_limit,
                                            string upper_limit,
                                            string lower_symbol,
                                            string upper_symbol,
                                            string units)
        {
            if (!utilityviewmodel.reportBandLimit)
            {
                utilityviewmodel.report_box += "        thereafter ";
            }
            if ((lower_limit == "0") &&
                (lower_symbol == "<"))
            {
                utilityviewmodel.report_box += " within ";
            }
            if (upper_symbol == "<=")
            {
                utilityviewmodel.report_box += upper_limit;
                switch (units)
                {
                    case "Years":
                        utilityviewmodel.report_box += " year";
                        break;
                    default:
                        break;
                }
            }
            return;
        }
        internal static void Report_Case_BL(UtilityViewModel utilityviewmodel,
                                            short area_code,
                                            string lower_limit,
                                            string upper_limit,
                                            string lower_symbol,
                                            string upper_symbol,
                                            string units,
                                            string field_name,
                                            SmartUtility.ConditionsLimits conditions_limits_row)
        {
            if ((lower_limit == "<AREA_CODE>") ||
                (upper_limit == "<AREA_CODE>"))
            {
                List<SmartUtility.ConditionsAreas> conditions_areas_found = new List<SmartUtility.ConditionsAreas>(from Conditions_Limits_Area
                                                in utilityviewmodel.Hezbollah.conditions_areasList
                                                                                                                   where ((Conditions_Limits_Area.SUPPLIER_CODE == conditions_limits_row.SUPPLIER_CODE) &&
                                                                                                                       (Conditions_Limits_Area.LIMIT_CODE == conditions_limits_row.LIMIT_CODE) &&
                                                                                                                       (Conditions_Limits_Area.RESOURCE_CODE == conditions_limits_row.RESOURCE_CODE) &&
                                                                                                                       (Conditions_Limits_Area.RESOURCE_TYPE == conditions_limits_row.RESOURCE_TYPE) &&
                                                                                                                       (Conditions_Limits_Area.AREA_CODE == area_code))
                                                                                                                   select Conditions_Limits_Area);
                foreach (SmartUtility.ConditionsAreas conditions_areas_row in conditions_areas_found)
                {
                    utilityviewmodel.report_box += "        for ";
                    utilityviewmodel.report_box += conditions_areas_row.LOWER_LIMIT;
                    utilityviewmodel.report_box += "kWh < usage ";
                    if (conditions_areas_row.UPPER_LIMIT != "999999")
                    {
                        utilityviewmodel.report_box += "<= " + conditions_areas_row.UPPER_LIMIT + "kWh";
                    }
                }
            }
            else
            {
                if ((lower_limit == "0") &&
                    (lower_symbol == "<"))
                {
                    utilityviewmodel.report_box += "        for first ";
                }
                if (upper_symbol == "<=")
                {
                    utilityviewmodel.report_box += upper_limit;
                    utilityviewmodel.report_box += SmartParametersV2016.space + units;
                }
                if (field_name == "UNITS_TOTAL")
                {
                    utilityviewmodel.report_box += SmartParametersV2016.space + " total units ";
                }
            }
            utilityviewmodel.reportBandLimit = true;
            return;
        }

        internal static void Report_Case_SC(UtilityViewModel utilityviewmodel,
                                            string newline,
                                            string indent,
                                            string lower_limit,
                                            string lower_symbol,
                                            string units,
                                            string standing_charge)
        {
            if ((lower_limit == "0") &&
                 ((lower_symbol == "<") ||
                  (lower_symbol == "<=")))
            {
                switch (units)
                {
                    case "Days":
                        utilityviewmodel.report_box += newline;
                        utilityviewmodel.report_box += indent + "Daily";
                        break;
                    case "Months":
                        utilityviewmodel.report_box += newline;
                        utilityviewmodel.report_box += indent + "Monthly";
                        break;
                    case "Years":
                        utilityviewmodel.report_box += newline;
                        utilityviewmodel.report_box += indent + "Yearly";
                        break;
                    default:
                        break;
                }
                utilityviewmodel.report_box += " Standing Charge of "; // Limit Name is always this for SC
                switch (units)
                {
                    case "Days":
                        utilityviewmodel.report_box += standing_charge + "p per kWh";
                        break;
                    case "Months":
                        utilityviewmodel.report_box += Report_Sterling_Format(standing_charge, true);
                        break;
                    case "Years":
                        utilityviewmodel.report_box += Report_Sterling_Format(standing_charge, true);
                        break;
                    default:
                        break;
                }
                utilityviewmodel.report_box += " applies";
            }
            return;
        }

        internal static void Report_Unit_Rates(UtilityViewModel utilityviewmodel,
                                            short area_code,
                                            char resource,
                                            short supplier_code,
                                            char resource_code,
                                            string resource_type,
                                            int tariff_code,
                                            short payment_code,
                                            short tier_count,
                                            short tier_level,
                                            DateTime prices_valid_from,
                                            string newline,
                                            string indent,
                                            SmartUtility.ConditionsPlans conditions_plans_row)
        {
            //string current_plan;
            //bool payment_plans_shown = false;

            List<SmartUtility.UnitRates> unit_rates_found = new List<SmartUtility.UnitRates>();
            switch (resource)
            {
                case SmartParametersV2016.Electricity:
                    unit_rates_found = new List<SmartUtility.UnitRates>(from Unit_Rate
                                                in utilityviewmodel.Hezbollah.e_unit_ratesList
                                                                        where ((Unit_Rate.SUPPLIER_CODE == supplier_code) && //conditions_view_row.SUPPLIER_CODE) &&
                                                                                (Unit_Rate.RESOURCE_CODE == resource_code) && //conditions_view_row.RESOURCE_CODE) &&
                                                                                (Unit_Rate.RESOURCE_TYPE == resource_type) && //conditions_view_row.RESOURCE_TYPE) &&
                                                                                (Unit_Rate.TARIFF_CODE == tariff_code) && //conditions_view_row.TARIFF_CODE) &&
                                                                                (Unit_Rate.PAYMENT_CODE == payment_code) && //conditions_view_row.PAYMENT_CODE) &&
                                                                                (Unit_Rate.TIER_COUNT == tier_count) && //conditions_view_row.TIER_COUNT) &&
                                                                                (Unit_Rate.TIER_LEVEL == tier_level) && //conditions_view_row.TIER_LEVEL) &&
                                                                                (Unit_Rate.AREA_CODE == area_code) &&
                                                                                (Unit_Rate.PRICES_VALID_FROM <= prices_valid_from))
                                                                        orderby Unit_Rate.PRICES_VALID_FROM descending
                                                                        select Unit_Rate);
                    break;
                case SmartParametersV2016.Gas:
                    unit_rates_found = new List<SmartUtility.UnitRates>(from Unit_Rate
                                                in utilityviewmodel.Hezbollah.g_unit_ratesList
                                                                        where ((Unit_Rate.SUPPLIER_CODE == supplier_code) && //conditions_view_row.SUPPLIER_CODE) &&
                                                                                (Unit_Rate.RESOURCE_CODE == resource_code) && //conditions_view_row.RESOURCE_CODE) &&
                                                                                (Unit_Rate.RESOURCE_TYPE == resource_type) && //conditions_view_row.RESOURCE_TYPE) &&
                                                                                (Unit_Rate.TARIFF_CODE == tariff_code) && //conditions_view_row.TARIFF_CODE) &&
                                                                                (Unit_Rate.PAYMENT_CODE == payment_code) && //conditions_view_row.PAYMENT_CODE) &&
                                                                                (Unit_Rate.TIER_COUNT == tier_count) && //conditions_view_row.TIER_COUNT) &&
                                                                                (Unit_Rate.TIER_LEVEL == tier_level) && //conditions_view_row.TIER_LEVEL) &&
                                                                                (Unit_Rate.AREA_CODE == area_code) &&
                                                                                (Unit_Rate.PRICES_VALID_FROM <= prices_valid_from))
                                                                        orderby Unit_Rate.PRICES_VALID_FROM descending
                                                                        select Unit_Rate);
                    break;
                default:
                    break;
            }

            Report_DoTheRest(utilityviewmodel,
                            newline,
                             indent,
                             conditions_plans_row,
                             unit_rates_found);

            //// Doesn't matter if no Groups are found
            //standing_charge = "";
            //if (unit_rates_found.Count == 0)
            //{
            //    report_box += newline +
            //                    "No rates found" + newline;
            //}
            //else
            //{
            //    payment_plans = conditions_plans_row.PAYMENT_PLANS.ToString();
            //    current_plan = "";
            //    for (int plan_index = 0; plan_index < payment_plans.Length; plan_index++)
            //    {
            //        string temp_plan = payment_plans.Substring(plan_index, 1);
            //        // Find the text relating to this Payment Plan code
            //        List<SmartUtility.PaymentPlans> payment_plans_found = (from Payment_Plan
            //                                                    in Hezbollah.payment_plansList
            //                                                   where (Payment_Plan.PAYMENT_PLAN == temp_plan)
            //                                                   select Payment_Plan).ToList();
            //        foreach (Payment_Plans payment_plans_row in payment_plans_found)
            //        {
            //            current_plan = payment_plans_row.PAYMENT_NAME;
            //            if (!payment_plans_shown)
            //            {
            //                utilityviewmodel.report_box += newline;
            //                utilityviewmodel.report_box += "Payment Plans: ";
            //            }
            //            else
            //            {
            //                utilityviewmodel.report_box += " / ";
            //            }
            //            utilityviewmodel.report_box += current_plan;
            //            payment_plans_shown = true;
            //            break;
            //        }
            //        payment_plans_shown = false;
            //        // End of Payment Plans
            //    }


            //    utilityviewmodel.report_box += newline +
            //                        //"    Standing Charge: " + unit_rates_row.SC + "p " +
            //                        "    Day Rate: " + unit_rates_found[0].DR + "p per kWh";
            //    if (unit_rates_found[0].SC != "0.0")
            //    {
            //        standing_charge = unit_rates_found[0].SC;
            //    }
            //    if (unit_rates_found[0].NR.ToString() != "0.0")
            //    {
            //        utilityviewmodel.report_box +=
            //                        " Night Rate: " + unit_rates_found[0].NR + "p per kWh";
            //    }
            //    if (unit_rates_found[0].TCR != "0.0")
            //    {
            //        utilityviewmodel.report_box +=
            //                        " TCR: " + unit_rates_found[0].TCR + "p per kWh";
            //    }
            //    utilityviewmodel.report_box += newline;
            //}
            return;
        }

        internal static void Report_DoTheRest(UtilityViewModel utilityviewmodel,
                                            string newline,
                                            string indent,
                                            SmartUtility.ConditionsPlans conditions_plans_row,
                                            List<SmartUtility.UnitRates> unit_rates_found)
        {
            string current_plan;
            bool payment_plans_shown = false;
            // Doesn't matter if no Groups are found
            utilityviewmodel.reportStandingCharge = "";
            if (unit_rates_found.Count == 0)
            {
                utilityviewmodel.report_box += newline +
                                "No rates found" + newline;
            }
            else
            {
                utilityviewmodel.reportPaymentPlans = conditions_plans_row.PAYMENT_PLANS.ToString();
                current_plan = "";
                for (int plan_index = 0; plan_index < utilityviewmodel.reportPaymentPlans.Length; plan_index++)
                {
                    char temp_plan = Convert.ToChar(utilityviewmodel.reportPaymentPlans.Substring(plan_index, 1));
                    // Find the text relating to this Payment Plan code
                    List<SmartUtility.PaymentPlans> payment_plans_found = new List<SmartUtility.PaymentPlans>(from Payment_Plan
                                                                in utilityviewmodel.Hezbollah.payment_plansList
                                                                                                              where (Payment_Plan.PAYMENT_PLAN == temp_plan)
                                                                                                              select Payment_Plan);
                    foreach (SmartUtility.PaymentPlans payment_plans_row in payment_plans_found)
                    {
                        current_plan = payment_plans_row.PAYMENT_NAME;
                        if (!payment_plans_shown)
                        {
                            utilityviewmodel.report_box += newline;
                            utilityviewmodel.report_box += "Payment Plans\t: ";
                        }
                        else
                        {
                            utilityviewmodel.report_box += " / ";
                        }
                        utilityviewmodel.report_box += current_plan;
                        payment_plans_shown = true;
                        break;
                    }
                    payment_plans_shown = false;
                    // End of Payment Plans
                }

                utilityviewmodel.report_box += newline +
                                    indent + "Day Rate: " + unit_rates_found[0].DR + "p per kWh";
                if (unit_rates_found[0].SC != "0.0")
                {
                    utilityviewmodel.reportStandingCharge = unit_rates_found[0].SC;
                }
                if (unit_rates_found[0].NR.ToString() != "0.0")
                {
                    utilityviewmodel.report_box += newline +
                                    indent + "Night Rate: " + unit_rates_found[0].NR + "p per kWh";
                }
                if (unit_rates_found[0].TCR != "0.0")
                {
                    utilityviewmodel.reportTcr = unit_rates_found[0].TCR;
                }
                //utilityviewmodel.report_box += newline;
            }
            return;
        }

        internal static bool Report_Post_Conditions(UtilityViewModel utilityviewmodel,
                                            char resource_code,
                                            string brand_name,
                                            string newline,
                                            string current_plan,
                                            SmartUtility.Tariffs tariff_row)
        {
            List<SmartUtility.Post_Groups> post_groups_found = new List<SmartUtility.Post_Groups>(from Post_Conditions_Group
                                                   in utilityviewmodel.Hezbollah.post_groupsList
                                                                                                  where ((Post_Conditions_Group.SUPPLIER_CODE == tariff_row.SUPPLIER_CODE) &&
                                                                                                          (Post_Conditions_Group.RESOURCE_CODE == tariff_row.RESOURCE_CODE))
                                                                                                  select Post_Conditions_Group);
            foreach (SmartUtility.Post_Groups post_groups_row in post_groups_found)
            {
                // Find the Tariff_Code relating to this Payment Plan code
                List<SmartUtility.Post_Codes> post_codes_found = new List<SmartUtility.Post_Codes>(from Tariff_Code
                                                    in utilityviewmodel.Hezbollah.post_codesList
                                                                                                   where ((Tariff_Code.SUPPLIER_CODE == post_groups_row.SUPPLIER_CODE) &&
                                                                                                           (Tariff_Code.TARIFF_CODE == tariff_row.TARIFF_CODE) &&
                                                                                                           (Tariff_Code.GROUP_CODE == post_groups_row.GROUP_CODE) &&
                                                                                                           (Tariff_Code.PAYMENT_PLANS.Contains(current_plan)))
                                                                                                   select Tariff_Code);
                foreach (SmartUtility.Post_Codes post_conditions_tariff_codes_row in post_codes_found)
                {
                    List<SmartUtility.Post_Groupings> post_groupings_found = new List<SmartUtility.Post_Groupings>(from Post_Conditions_Grouping
                                                                in utilityviewmodel.Hezbollah.post_groupingsList
                                                                                                                   where ((Post_Conditions_Grouping.SUPPLIER_CODE == post_conditions_tariff_codes_row.SUPPLIER_CODE) &&
                                                                                                                           (Post_Conditions_Grouping.GROUP_CODE == post_conditions_tariff_codes_row.GROUP_CODE))
                                                                                                                   select Post_Conditions_Grouping);
                    foreach (SmartUtility.Post_Groupings post_groupings_row in post_groupings_found)
                    {
                        List<SmartUtility.Post_Conditions> post_conditions_found = new List<SmartUtility.Post_Conditions>(from Post_Condition
                                                in utilityviewmodel.Hezbollah.post_conditionsList
                                                                                                                          where ((Post_Condition.SUPPLIER_CODE == post_groupings_row.SUPPLIER_CODE) &&
                                                                                                                              (Post_Condition.RESOURCE_CODE == post_groups_row.RESOURCE_CODE) &&
                                                                                                                              (Post_Condition.SELECTION_CODE == post_groupings_row.SELECTION_CODE))
                                                                                                                          select Post_Condition);
                        foreach (SmartUtility.Post_Conditions post_conditions_row in post_conditions_found)
                        {
                            utilityviewmodel.report_box += "    " + post_conditions_row.SELECTION_NAME + SmartParametersV2016.space;
                            if (!string.IsNullOrEmpty(post_conditions_row.PRICE_EXC_VAT))
                            {
                                if (post_conditions_row.PRICE_EXC_VAT.IndexOf("%") == -1)
                                {
                                    if (post_conditions_row.PRICE_EXC_VAT.IndexOf("-") >= 0)
                                    {
                                        utilityviewmodel.report_box += "discount of ";
                                    }
                                    utilityviewmodel.report_box += Report_Sterling_Format(post_conditions_row.PRICE_EXC_VAT.Replace("-", ""), true);
                                    if (post_conditions_row.PRICE_EXC_VAT.IndexOf("-") == -1)
                                    {
                                        utilityviewmodel.report_box += " applies";
                                    }
                                }
                            }
                            else
                            {
                                if (!string.IsNullOrEmpty(post_conditions_row.MAXIMUM_DISCOUNT))
                                {
                                    if (post_conditions_row.MAXIMUM_DISCOUNT.IndexOf("%") == -1)
                                    {
                                        if (post_conditions_row.MAXIMUM_DISCOUNT.IndexOf("-") >= 0)
                                        {
                                            utilityviewmodel.report_box += "discount of ";
                                        }
                                        string temp = post_conditions_row.MAXIMUM_DISCOUNT.Replace("-", "");
                                        temp = ((Convert.ToInt32(temp) * 100) / 105).ToString();
                                        utilityviewmodel.report_box += Report_Sterling_Format(temp, true);
                                        if (post_conditions_row.MAXIMUM_DISCOUNT.IndexOf("-") == -1)
                                        {
                                            utilityviewmodel.report_box += " applies";
                                        }
                                    }
                                }

                            }

                            if (!Report_Post_Fuckknowswhat(utilityviewmodel,
                                                resource_code,
                                                post_conditions_row.SUPPLIER_CODE,
                                                post_conditions_row.SELECTION_CODE,
                                                brand_name,
                                                current_plan,
                                                newline))
                            {
                                return false;
                            }

                            //    List<SmartUtility.Post_Select> post_select_found = (from Post_Conditions_Select_Group
                            //                                            in Hezbollah.post_selectList
                            //                                           where ((Post_Conditions_Select_Group.SUPPLIER_CODE == post_conditions_row.SUPPLIER_CODE) &&
                            //                                                   (Post_Conditions_Select_Group.SELECTION_CODE == post_conditions_row.SELECTION_CODE))
                            //                                           select Post_Conditions_Select_Group).ToList();
                            //    foreach (Post_Select post_select_row in post_select_found)
                            //    {
                            //        // Make sure you have all the TLs for Standing Charges BEFORE
                            //        // the TL for Paperless Billing
                            //        List<SmartUtility.Post_Limits> post_limits_found = (from Post_Limit
                            //                                            in Hezbollah.post_limitsList
                            //                                               where ((Post_Limit.SUPPLIER_CODE == post_select_row.SUPPLIER_CODE) &&
                            //                                                       (Post_Limit.LIMIT_CODE == post_select_row.LIMIT_CODE))
                            //                                               select Post_Limit).ToList();
                            //        foreach (Post_Limits post_limits_row in post_limits_found)
                            //        {
                            //            switch (post_limits_row.LIMIT_TYPE)
                            //            {
                            //                case "BL": //"Band Limit"
                            //                    if ((post_limits_row.LOWER_LIMIT == "0") &&
                            //                        (post_limits_row.LOWER_SYMBOL == "<"))
                            //                    {
                            //                        utilityviewmodel.report_box += " up to ";
                            //                    }
                            //                    utilityviewmodel.report_box += post_limits_row.UPPER_LIMIT;
                            //                    utilityviewmodel.report_box += post_limits_row.UNITS;
                            //                    utilityviewmodel.report_box += newline;
                            //                    break;
                            //                case "TL": //"Time Limit" - see the table for Lower Symbol explanations below
                            //                           // The STANDING CHARGES are <= because you pay them
                            //                           // EVEN IF you don't complete a full year (i.e. they
                            //                           // are charged 'up front' at the START of the month or year)
                            //                           //
                            //                           // The DIRECT DEBITs, DUAL_FUEL and PAPERLESS Discounts are < because
                            //                           // you only get them IF you have completed 1 month
                            //                           // (i.e. you get them at the END of the month)
                            //                    if ((post_limits_row.LOWER_LIMIT == "0") &&
                            //                        (post_limits_row.LOWER_SYMBOL == "<"))
                            //                    {
                            //                        utilityviewmodel.report_box += " applies every ";
                            //                    }
                            //                    else
                            //                    {
                            //                        if ((post_limits_row.LOWER_LIMIT == "0") &&
                            //                            (post_limits_row.LOWER_SYMBOL == "<="))
                            //                        {
                            //                            utilityviewmodel.report_box += " commencing each ";
                            //                        }

                            //                    }
                            //                    if ((post_limits_row.UPPER_SYMBOL == "<") ||
                            //                        (post_limits_row.UPPER_SYMBOL == "<="))
                            //                    {
                            //                        switch (post_limits_row.UNITS)
                            //                        {
                            //                            case "Months":
                            //                                if (post_limits_row.UPPER_LIMIT == "1")
                            //                                {
                            //                                    utilityviewmodel.report_box += "month";
                            //                                }
                            //                                break;
                            //                            case "Years":
                            //                                if (post_limits_row.UPPER_LIMIT == "1")
                            //                                {
                            //                                    utilityviewmodel.report_box += "year";
                            //                                }
                            //                                break;
                            //                            default:
                            //                                break;
                            //                        }
                            //                    }
                            //                    utilityviewmodel.report_box += newline;
                            //                    break;
                            //                case "TF": //"Dual Fuel Flag / Economy7 / Paperless Billing Flag"
                            //                    if (post_limits_row.LOWER_LIMIT == "T")
                            //                    {
                            //                        switch (post_limits_row.UNITS)
                            //                        {
                            //                            case "Flag":
                            //                                switch (post_limits_row.FIELD_NAME)
                            //                                {
                            //                                    case "DUAL_FUEL":
                            //                                        utilityviewmodel.report_box += "    (If ";
                            //                                        switch (resource_code)
                            //                                        {
                            //                                            case SmartParametersV2016.Electricity:
                            //                                                utilityviewmodel.report_box += "GAS";
                            //                                                break;
                            //                                            case SmartParametersV2016.Gas:
                            //                                                utilityviewmodel.report_box += "ELECTRICITY";
                            //                                                break;
                            //                                            default:
                            //                                                break;
                            //                                        }
                            //                                        utilityviewmodel.report_box += " is also taken with " + brand_name + ")";
                            //                                        break;
                            //                                    case "PAPERLESS_BILLING":
                            //                                        utilityviewmodel.report_box += "    (If ";
                            //                                        utilityviewmodel.report_box += "Paperless Billing";
                            //                                        utilityviewmodel.report_box += " option is chosen)";
                            //                                        break;
                            //                                    case "ECONOMY7":
                            //                                        utilityviewmodel.report_box += "    (For ";
                            //                                        utilityviewmodel.report_box += "Economy7";
                            //                                        utilityviewmodel.report_box += " meters)";
                            //                                        break;
                            //                                    default:
                            //                                        break;
                            //                                }
                            //                                break;
                            //                            default:
                            //                                break;
                            //                        }
                            //                    }
                            //                    utilityviewmodel.report_box += newline;
                            //                    break;
                            //                case "TP": //"Tariff Plan"
                            //                           // I cant' remember but I THINK this was an Npower special which means that some
                            //                           // Post Conditions only apply to certain Payment Plans or a range of them?
                            //                           // Cannot find an example of this in the current database though ...
                            //                           // A Tariff Plan is in POST_LIMITS but nothing refers to it ...
                            //                    string plans_range = "";
                            //                    if (!string.IsNullOrEmpty(post_limits_row.LOWER_LIMIT))
                            //                    {
                            //                        plans_range = plans_range + post_limits_row.LOWER_LIMIT;
                            //                    }
                            //                    if (!string.IsNullOrEmpty(post_limits_row.UPPER_LIMIT))
                            //                    {
                            //                        plans_range = plans_range + post_limits_row.UPPER_LIMIT;
                            //                    }
                            //                    if (plans_range.IndexOf(current_plan) >= 0)
                            //                    {
                            //                        utilityviewmodel.report_box += "        Applies to this Payment Plan";
                            //                        utilityviewmodel.report_box += newline;
                            //                    }
                            //                    break;
                            //                default:
                            //                    // Anything else?
                            //                    utilityviewmodel.report_box += "Problem 77: decoding ... sorry";
                            //                    return false;     // Avoid race condition
                            //            }
                            //        }
                            //    }
                        }
                    }
                }
            }
            return true;
        }

        internal static bool Report_Post_Fuckknowswhat(UtilityViewModel utilityviewmodel,
                                                char resource_code,
                                                short supplier_code,
                                                short selection_code,
                                                string brand_name,
                                                string current_plan,
                                                string newline)
        {
            List<SmartUtility.Post_Select> post_select_found = new List<SmartUtility.Post_Select>(from Post_Conditions_Select_Group
                                                   in utilityviewmodel.Hezbollah.post_selectList
                                                                                                  where ((Post_Conditions_Select_Group.SUPPLIER_CODE == supplier_code) &&
                                                                                                          (Post_Conditions_Select_Group.SELECTION_CODE == selection_code))
                                                                                                  select Post_Conditions_Select_Group);
            foreach (SmartUtility.Post_Select post_select_row in post_select_found)
            {
                List<SmartUtility.Post_Limits> post_limits_found = new List<SmartUtility.Post_Limits>(from Post_Limit
                                      in utilityviewmodel.Hezbollah.post_limitsList
                                                                                                      where ((Post_Limit.SUPPLIER_CODE == post_select_row.SUPPLIER_CODE) &&
                                                                                                              (Post_Limit.LIMIT_CODE == post_select_row.LIMIT_CODE))
                                                                                                      select Post_Limit);
                foreach (SmartUtility.Post_Limits post_limits_row in post_limits_found)
                {
                    switch (post_limits_row.LIMIT_TYPE)
                    {
                        case "BL": //"Band Limit"
                            if ((post_limits_row.LOWER_LIMIT == "0") &&
                                (post_limits_row.LOWER_SYMBOL == "<"))
                            {
                                utilityviewmodel.report_box += " up to ";
                            }
                            utilityviewmodel.report_box += post_limits_row.UPPER_LIMIT;
                            utilityviewmodel.report_box += post_limits_row.UNITS;
                            utilityviewmodel.report_box += newline;
                            break;
                        case "TL": //"Time Limit" - see the table for Lower Symbol explanations below
                                   // The STANDING CHARGES are <= because you pay them
                                   // EVEN IF you don't complete a full year (i.e. they
                                   // are charged 'up front' at the START of the month or year)
                                   //
                                   // The DIRECT DEBITs, DUAL_FUEL and PAPERLESS Discounts are < because
                                   // you only get them IF you have completed 1 month
                                   // (i.e. you get them at the END of the month)
                            if ((post_limits_row.LOWER_LIMIT == "0") &&
                                (post_limits_row.LOWER_SYMBOL == "<"))
                            {
                                utilityviewmodel.report_box += " applies every ";
                            }
                            else
                            {
                                if ((post_limits_row.LOWER_LIMIT == "0") &&
                                    (post_limits_row.LOWER_SYMBOL == "<="))
                                {
                                    utilityviewmodel.report_box += " commencing each ";
                                }

                            }
                            if ((post_limits_row.UPPER_SYMBOL == "<") ||
                                (post_limits_row.UPPER_SYMBOL == "<="))
                            {
                                switch (post_limits_row.UNITS)
                                {
                                    case "Months":
                                        if (post_limits_row.UPPER_LIMIT == "1")
                                        {
                                            utilityviewmodel.report_box += "month";
                                        }
                                        break;
                                    case "Years":
                                        if (post_limits_row.UPPER_LIMIT == "1")
                                        {
                                            utilityviewmodel.report_box += "year";
                                        }
                                        break;
                                    default:
                                        break;
                                }
                            }
                            utilityviewmodel.report_box += newline;
                            break;
                        case "TF": //"Dual Fuel Flag / Economy7 / Paperless Billing Flag"
                            Report_Case_TF(utilityviewmodel,
                                                    post_limits_row.LOWER_LIMIT,
                                                    post_limits_row.UNITS,
                                                    post_limits_row.FIELD_NAME,
                                                    brand_name,
                                                    resource_code);
                            //if (post_limits_row.LOWER_LIMIT == "T")
                            //{

                            //switch (post_limits_row.UNITS)
                            //{
                            //    case "Flag":
                            //        switch (post_limits_row.FIELD_NAME)
                            //        {
                            //            case "DUAL_FUEL":
                            //                report_box += "    (If ";
                            //                switch (resource_code)
                            //                {
                            //                    case SmartParametersV2016.Electricity:
                            //                        report_box += "GAS";
                            //                        break;
                            //                    case SmartParametersV2016.Gas:
                            //                        report_box += "ELECTRICITY";
                            //                        break;
                            //                    default:
                            //                        break;
                            //                }
                            //                utilityviewmodel.report_box += " is also taken with " + brand_name + ")";
                            //                break;
                            //            case "PAPERLESS_BILLING":
                            //                utilityviewmodel.report_box += "    (If ";
                            //                utilityviewmodel.report_box += "Paperless Billing";
                            //                utilityviewmodel.report_box += " option is chosen)";
                            //                break;
                            //            case "ECONOMY7":
                            //                utilityviewmodel.report_box += "    (For ";
                            //                utilityviewmodel.report_box += "Economy7";
                            //                utilityviewmodel.report_box += " meters)";
                            //                break;
                            //            default:
                            //                break;
                            //        }
                            //        break;
                            //    default:
                            //        break;
                            //}
                            //}
                            utilityviewmodel.report_box += newline;
                            break;
                        case "TP": //"Tariff Plan"
                                   // I cant' remember but I THINK this was an Npower special which means that some
                                   // Post Conditions only apply to certain Payment Plans or a range of them?
                                   // Cannot find an example of this in the current database though ...
                                   // A Tariff Plan is in POST_LIMITS but nothing refers to it ...
                            utilityviewmodel.reportPlansRange = "";
                            Report_Case_TP(utilityviewmodel,
                                            post_limits_row.LOWER_LIMIT,
                                            post_limits_row.UPPER_LIMIT,
                                            current_plan,
                                            newline);
                            //if (!string.IsNullOrEmpty(post_limits_row.LOWER_LIMIT))
                            //{
                            //    plans_range = plans_range + post_limits_row.LOWER_LIMIT;
                            //}
                            //if (!string.IsNullOrEmpty(post_limits_row.UPPER_LIMIT))
                            //{
                            //    plans_range = plans_range + post_limits_row.UPPER_LIMIT;
                            //}
                            //if (plans_range.IndexOf(current_plan) >= 0)
                            //{
                            //    utilityviewmodel.report_box += "        Applies to this Payment Plan";
                            //    utilityviewmodel.report_box += newline;
                            //}
                            break;
                        default:
                            // Anything else?
                            utilityviewmodel.report_box += "Problem 77: Decoding ... sorry";
                            return false;     // Avoid race condition
                    }
                }
            }
            return true;
        }

        internal static void Report_Case_TP(UtilityViewModel utilityviewmodel,
                                    string LOWER_LIMIT,
                                    string UPPER_LIMIT,
                                    string current_plan,
                                    string newline)
        {
            if (!string.IsNullOrEmpty(LOWER_LIMIT))
            {
                utilityviewmodel.reportPlansRange += LOWER_LIMIT;
            }
            if (!string.IsNullOrEmpty(UPPER_LIMIT))
            {
                utilityviewmodel.reportPlansRange += UPPER_LIMIT;
            }
            if (utilityviewmodel.reportPlansRange.IndexOf(current_plan) >= 0)
            {
                utilityviewmodel.report_box += "        Applies to this Payment Plan";
                utilityviewmodel.report_box += newline;
            }
            return;
        }

        internal static void Report_Case_TF(UtilityViewModel utilityviewmodel,
                                                    string LOWER_LIMIT,
                                                    string UNITS,
                                                    string FIELD_NAME,
                                                    string brand_name,
                                                    char resource_code)
        {
            if (LOWER_LIMIT == "T")
            {
                switch (UNITS)
                {
                    case "Flag":
                        switch (FIELD_NAME)
                        {
                            case "DUAL_FUEL":
                                utilityviewmodel.report_box += "    (If ";
                                switch (resource_code)
                                {
                                    case SmartParametersV2016.Electricity:
                                        utilityviewmodel.report_box += "GAS";
                                        break;
                                    case SmartParametersV2016.Gas:
                                        utilityviewmodel.report_box += "ELECTRICITY";
                                        break;
                                    default:
                                        break;
                                }
                                utilityviewmodel.report_box += " is also taken with " + brand_name + ")";
                                break;
                            case "PAPERLESS_BILLING":
                                utilityviewmodel.report_box += "    (If ";
                                utilityviewmodel.report_box += "Paperless Billing";
                                utilityviewmodel.report_box += " option is chosen)";
                                break;
                            case "ECONOMY7":
                                utilityviewmodel.report_box += "    (For ";
                                utilityviewmodel.report_box += "Economy7";
                                utilityviewmodel.report_box += " meters)";
                                break;
                            default:
                                break;
                        }
                        break;
                    default:
                        break;
                }
            }
            return;
        }
        internal static string Report_Sterling_Format(string AMOUNT, bool symbol)
        {
            bool negative = false;
            if (AMOUNT.IndexOf("-") >= 0)
            {
                AMOUNT = AMOUNT.Replace("-", "");
                negative = true;
            }
            switch (AMOUNT.Length)
            {
                case 0:
                    AMOUNT = "0";
                    break;
                case 1:
                    AMOUNT = "0.0" + AMOUNT;
                    break;
                case 2:
                    AMOUNT = "0." + AMOUNT;
                    break;
                default:
                    AMOUNT = AMOUNT.Substring(0, AMOUNT.Length - 2) + SmartParametersV2016.decimalPoint + AMOUNT.Substring(AMOUNT.Length - 2, 2);
                    break;
            }
            if (symbol)
            {
                AMOUNT = "£" + AMOUNT;
            }
            if (negative)
            {
                AMOUNT = "-" + AMOUNT;
            }
            return AMOUNT;
        }

        internal static string Report_Lookup_Area_Name(short area_code, List<SmartUtility.SupplyAreas> supply_areasList)
        {
            // Only if its not empty and we have something in the table
            if ((area_code > 0) &&
                (supply_areasList.Count > 0))
            {
                List<SmartUtility.SupplyAreas> supply_areas_found = new List<SmartUtility.SupplyAreas>(from Supply_Area
                                                         in supply_areasList
                                                                                                       where (Supply_Area.AREA_CODE == area_code)
                                                                                                       select Supply_Area);
                if (supply_areas_found.Count == 1)
                {
                    foreach (SmartUtility.SupplyAreas supply_areas_row in supply_areas_found)
                    {
                        return supply_areas_row.AREA_NAME;
                    }
                }
            }
            return "";
        }
    }
}