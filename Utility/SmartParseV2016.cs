using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SmartCubeMobile
{
    public class SmartParseV2016
    {
        internal static void Update_Tibs(string[][] tibs,
                                                string last_routine,
                                                string relative_pathname)
        {
            foreach (string[] tibs_outer_row in tibs)
            {
                if (tibs_outer_row[0] == last_routine)
                {
                    if (string.IsNullOrEmpty(tibs_outer_row[5]))
                    {
                        tibs_outer_row[5] = relative_pathname;
                    }
                }
            }
            return;
        }

        internal static string Find_This_Routine(string[][] tibs,
                                                string last_routine,
                                                //rf string exit_test,
                                                //rf string yes_routine,
                                                //rf string no_routine,
                                                //rf string comments,
                                                //rf string relative_pathname,
                                                UtilityViewModel utilityviewmodel)
        {
            //int tibs_count = tibs.GetUpperBound(0);

            foreach (string[] tibs_outer_row in tibs)
            {
                if (string.IsNullOrEmpty(last_routine))
                {
                    utilityviewmodel.exit_test = tibs_outer_row[1];
                    utilityviewmodel.yes_routine = tibs_outer_row[2];
                    utilityviewmodel.no_routine = tibs_outer_row[3];
                    utilityviewmodel.comments = tibs_outer_row[4];
                    utilityviewmodel.relative_pathname = tibs_outer_row[5];
                    return tibs_outer_row[0];
                }
                else
                {
                    if (tibs_outer_row[0] == last_routine)
                    {
                        utilityviewmodel.exit_test = tibs_outer_row[1];
                        utilityviewmodel.yes_routine = tibs_outer_row[2];
                        utilityviewmodel.no_routine = tibs_outer_row[3];
                        utilityviewmodel.comments = tibs_outer_row[4];
                        utilityviewmodel.relative_pathname = tibs_outer_row[5];
                        if (!string.IsNullOrEmpty(tibs_outer_row[6]))
                        {
                            switch (tibs_outer_row[6])
                            {
                                case "Account_No":
                                    utilityviewmodel.relative_pathname += utilityviewmodel.account_no;
                                    break;
                                default:
                                    break;
                            }
                        }
                        return tibs_outer_row[0];
                    }
                }
            }
            return "";
        }

        internal static bool Strip_FuelList(UtilityViewModel utilityviewmodel)
        {
            if (!string.IsNullOrEmpty(utilityviewmodel.fuelList))
            {
                string[] components = utilityviewmodel.fuelList.Split(SmartParametersV2016.bar);
                if (components.Length > 0)
                {
                    string[] these = components[0].Split(SmartParametersV2016.colon);
                    if (these.Length > 1)
                    {
                        utilityviewmodel.resource_code = Convert.ToChar(these[0]);
                        utilityviewmodel.resource_type = these[1];
                        utilityviewmodel.fuelList = utilityviewmodel.fuelList.Replace(components[0], "").Trim(SmartParametersV2016.bar);
                        return true;
                    }
                }
            }
            return false;
        }

        internal static void Update_FuelList(char cubeface_code,
                                                char resource_code,
                                                short brand_code,
                                                List<SmartUtility.SupplierTypes> supplier_typesList,
                                                UtilityViewModel utilityviewmodel)
        {
            List<SmartUtility.SupplierTypes> s_types_found = new List<SmartUtility.SupplierTypes>(from STypes in supplier_typesList
                                                                                                  where ((STypes.CUBEFACE_CODE == cubeface_code) &&
                                                                                                          (STypes.RESOURCE_CODE == resource_code) &&
                                                                                                          (STypes.SUPPLIER_CODE == brand_code))
                                                                                                  select STypes);
            if (s_types_found.Count > 0)
            {
                bool found_it = false;
                if (!string.IsNullOrEmpty(utilityviewmodel.fuelList))
                {
                    string[] fuels = utilityviewmodel.fuelList.Split(SmartParametersV2016.bar);

                    foreach (string fuel in fuels)
                    {
                        string[] these = fuel.Split(SmartParametersV2016.colon);
                        if (these.Length > 0)
                        {
                            if (s_types_found[0].RESOURCE_CODE == Convert.ToChar(these[0]))
                            {
                                found_it = true;
                                break;
                            }
                        }
                    }
                }

                if (!found_it)
                {
                    if (!string.IsNullOrEmpty(utilityviewmodel.fuelList))
                    {
                        // Include a separator
                        utilityviewmodel.fuelList += SmartParametersV2016.bar;
                    }
                    utilityviewmodel.fuelList = utilityviewmodel.fuelList + s_types_found[0].RESOURCE_CODE +
                                    SmartParametersV2016.colon + s_types_found[0].RESOURCE_TYPE;
                }
            }
            return;
        }

        internal static bool Lookup_Vat_Rate(short vat_code,
                                                DateTime from_date,
                                                DateTime to_date,
                                                List<SmartUsers.VatRates> vatRatesList,
                                                UtilityViewModel utilityviewmodel)
        {
            // Kludgy at the moment as we assume VAT is constant across the date range,
            // but it mightnot always be so ....
            foreach (SmartUsers.VatRates vat_rates_row in vatRatesList)
            {
                if (vat_rates_row.VAT_CODE == vat_code)
                {
                    DateTime valid_from = vat_rates_row.VALID_FROM;
                    DateTime valid_to = vat_rates_row.VALID_TO;

                    // Find a VAT rate which covers 'from' and 'to'
                    if ((SmartRoutinesV2018.DateTimeCompare(valid_from, from_date) <= 0) &&
                        (SmartRoutinesV2018.DateTimeCompare(to_date, valid_to) <= 0))
                    {
                        utilityviewmodel.VAT_RATE = SmartRoutinesV2018.ConvertDecimal(vat_rates_row.VAT_RATE);
                        return true;
                    }
                }
            }
            return false;
        }

        //internal static bool Add_Readings_Test(UtilityViewModel utilityviewmodel,
        //                                    DateTime READINGS_PERIOD_START,
        //                                    DateTime READINGS_PERIOD_END,
        //                                    string D_LAST_READ,
        //                                    string D_THIS_READ,
        //                                    string UNIT_OF_MEASURE)
        //{
        //    // Do we have enought for a Read or Units_cost?
        //    if ((READINGS_PERIOD_START != SmartParametersV2016.defaultDate) &&
        //        (READINGS_PERIOD_END != SmartParametersV2016.defaultDate) &&
        //        (D_LAST_READ != "0") &&
        //        (D_THIS_READ != "0") &&
        //        // No D_UNITS_USED = 0 test because sometimes it might be!
        //        (!string.IsNullOrEmpty(UNIT_OF_MEASURE)))
        //    {
        //        return true;
        //    }
        //    return false;
        //}
        internal static bool Add_Readings_Test(DateTime defaultDate,
                                            DateTime READINGS_PERIOD_START,
                                            DateTime READINGS_PERIOD_END,
                                            string D_LAST_READ,
                                            string D_THIS_READ,
                                            string UNIT_OF_MEASURE)
        {
            if ((READINGS_PERIOD_START != defaultDate) &&
                (READINGS_PERIOD_END != defaultDate) &&
                (D_LAST_READ != "0") &&
                (D_THIS_READ != "0") &&
                // No D_UNITS_USED = 0 because sometimes it might be!
                (!string.IsNullOrEmpty(UNIT_OF_MEASURE)))
            {
                return true;
            }
            return false;
        }

        internal static void Sort_Sections_New(UtilityViewModel utilityviewmodel,
                                            //rf string[][] sections,
                                            string[] lines,
                                            int startLine,
                                            //rf int matchingLine,
                                            bool compress = false)
        {
            // The previous sort sections would whack through the list looking
            // for a tag AND overwrite a previously found tag if more than one existed!!
            // i.e.  for B 0     line_no 21 "Your electricity statement explained"
            //               and line_no 53 "Your electricity statement explained"
            // it would store line_no 53 and not line_no 21.  Line 21 would be ignored ...
            // So we need to fix it so that if a line_no >= 0 then we DON'T replace
            // We only replace if line_no == -1
            int section_count = utilityviewmodel.sections.GetUpperBound(0);
            //int line_count = 0;
            int total_lines = lines.Length;

            utilityviewmodel.matchingLine = total_lines;

            // Pass 1 - loop through the lines and try and find a matching section name to fill the
            // start parameter
            utilityviewmodel.token = "";

            //int section_index = 0;
            for (int line_count = startLine; line_count < total_lines; line_count++)
            {
                utilityviewmodel.token = lines[line_count].Trim();
                if (compress)
                {
                    utilityviewmodel.token = utilityviewmodel.token.Replace(SmartParametersV2016.space, "").Trim();
                }
                utilityviewmodel.token = Remove_Double_Spaces_V3(utilityviewmodel.token);
                for (int section_index = 0; section_index <= section_count; section_index++)
                {
                    //if ((sections[section_index][4] == "-1") ||
                    //    (sections[section_index][6].IndexOf(SmartParametersV2016.sectionsCanOverride) >= 0))
                    //{
                    bool exact = false;
                    if (utilityviewmodel.sections[section_index][6].IndexOf(SmartParametersV2016.sectionsExactMatch) >= 0)
                    {
                        exact = true;
                    }
                    if (SmartParseV2016.Token_Identify_Array(utilityviewmodel, utilityviewmodel.sections[section_index][3], false, exact))
                    {
                        if ((utilityviewmodel.sections[section_index][4] == "-1") ||
                            (utilityviewmodel.sections[section_index][6].IndexOf(SmartParametersV2016.sectionsCanOverride) >= 0))
                        {
                            utilityviewmodel.sections[section_index][4] = line_count.ToString();
                            break;
                        }
                        else
                        {
                            utilityviewmodel.matchingLine = line_count;
                            goto Pass2;
                        }
                    }
                    //}
                }
            }

        Pass2:
            // Pass 2 - Bubble Sort to arrange the start parameters in ascending order
            bool swap_done = true;
            while (swap_done)
            {
                swap_done = false;
                for (int section_index = 0; section_index < section_count; section_index++)
                {
                    if (Convert.ToInt32(utilityviewmodel.sections[section_index][4]) > Convert.ToInt32(utilityviewmodel.sections[section_index + 1][4]))
                    {
                        for (int ray = 0; ray <= 6; ray++)
                        {
                            (utilityviewmodel.sections[section_index + 1][ray],
                                utilityviewmodel.sections[section_index][ray]) =
                            (utilityviewmodel.sections[section_index][ray],
                                utilityviewmodel.sections[section_index + 1][ray]);
                        }
                        //string temp = utilityviewmodel.sections[section_index][0];
                        //utilityviewmodel.sections[section_index][0] = utilityviewmodel.sections[section_index + 1][0];
                        //utilityviewmodel.sections[section_index + 1][0] = temp;

                        // Using a fucking tuple whatever the fuck that is
                        //(utilityviewmodel.sections[section_index + 1][0], utilityviewmodel.sections[section_index][0]) =
                        //                (utilityviewmodel.sections[section_index][0], utilityviewmodel.sections[section_index + 1][0]);
                        //temp = utilityviewmodel.sections[section_index][1];
                        //utilityviewmodel.sections[section_index][1] = utilityviewmodel.sections[section_index + 1][1];
                        //utilityviewmodel.sections[section_index + 1][1] = temp;

                        // Using a tuple (whatever that is!) ... but we don't have to use a 'temp'
                        //(utilityviewmodel.sections[section_index + 1][1], utilityviewmodel.sections[section_index][1]) =
                        //        (utilityviewmodel.sections[section_index][1], utilityviewmodel.sections[section_index + 1][1]);

                        //temp = utilityviewmodel.sections[section_index][2];
                        //utilityviewmodel.sections[section_index][2] = utilityviewmodel.sections[section_index + 1][2];
                        //utilityviewmodel.sections[section_index + 1][2] = temp;

                        //temp = utilityviewmodel.sections[section_index][3];
                        //utilityviewmodel.sections[section_index][3] = utilityviewmodel.sections[section_index + 1][3];
                        //utilityviewmodel.sections[section_index + 1][3] = temp;

                        //temp = utilityviewmodel.sections[section_index][4];
                        //utilityviewmodel.sections[section_index][4] = utilityviewmodel.sections[section_index + 1][4];
                        //utilityviewmodel.sections[section_index + 1][4] = temp;

                        //temp = utilityviewmodel.sections[section_index][5];
                        //utilityviewmodel.sections[section_index][5] = utilityviewmodel.sections[section_index + 1][5];
                        //utilityviewmodel.sections[section_index + 1][5] = temp;

                        //temp = utilityviewmodel.sections[section_index][6];
                        //utilityviewmodel.sections[section_index][6] = utilityviewmodel.sections[section_index + 1][6];
                        //utilityviewmodel.sections[section_index + 1][6] = temp;

                        swap_done = true;
                    }
                }
            }

            // Pass3 - now work BACKWARDs down the list making sure the finish parameters are one
            // less than the start parameters  of the higher section
            for (int section_index = section_count; section_index >= 0; section_index--)
            {
                // Don't do the -1s
                if (utilityviewmodel.sections[section_index][4] != "-1")
                {
                    if (section_index == section_count)
                    {
                        utilityviewmodel.sections[section_index][5] = (utilityviewmodel.matchingLine - 1).ToString();

                    }
                    else
                    {
                        int finish = Convert.ToInt32(utilityviewmodel.sections[section_index + 1][4]);
                        if (finish > 0)
                        {
                            finish--;
                            utilityviewmodel.sections[section_index][5] = finish.ToString();
                        }
                    }
                }
            }
            return;
        }

        internal static void Sort_Sections(bool special,
                                            UtilityViewModel utilityviewmodel,
                                            string[] lines,
                                            bool compress = false)
        {
            int section_count = utilityviewmodel.sections.GetUpperBound(0);
            //int line_count = 0;
            int total_lines = lines.Length;

            // Pass 1 - loop through the lines and try and find a matching section name to fill the
            // start parameter
            utilityviewmodel.token = "";

            //int section_index = 0;
            for (int line_count = 0; line_count < total_lines; line_count++)
            {
                utilityviewmodel.token = lines[line_count].Trim();
                if (compress)
                {
                    utilityviewmodel.token = utilityviewmodel.token.Replace(SmartParametersV2016.space, "").Trim();
                }
                utilityviewmodel.token = Remove_Double_Spaces_V3(utilityviewmodel.token);
                for (int section_index = 0; section_index <= section_count; section_index++)
                {
                    if ((utilityviewmodel.sections[section_index][4] == "-1") ||
                        (utilityviewmodel.sections[section_index][6].IndexOf(SmartParametersV2016.sectionsCanOverride) >= 0))
                    {
                        bool exact = false;
                        if (utilityviewmodel.sections[section_index][6].IndexOf(SmartParametersV2016.sectionsExactMatch) >= 0)
                        {
                            exact = true;
                        }
                        if (SmartParseV2016.Token_Identify_Array(utilityviewmodel, utilityviewmodel.sections[section_index][3], false, exact))
                        {
                            bool store_it = true;
                            if (special)
                            {
                                if (utilityviewmodel.sections[section_index][1] != "0")
                                {
                                    // Does the master section have settings?
                                    for (int sub_section_index = 0; sub_section_index <= section_count; sub_section_index++)
                                    {
                                        if (utilityviewmodel.sections[sub_section_index][0] == utilityviewmodel.sections[section_index][0] &&
                                            utilityviewmodel.sections[sub_section_index][1] == "0")
                                        {
                                            if (Convert.ToInt32(utilityviewmodel.sections[sub_section_index][4]) == -1 &&
                                                Convert.ToInt32(utilityviewmodel.sections[sub_section_index][5]) == -1)
                                            {
                                                store_it = false;
                                                break;
                                            }
                                        }
                                    }
                                }
                            }
                            if (store_it)
                            {
                                utilityviewmodel.sections[section_index][4] = line_count.ToString();
                            }
                            break;
                        }
                    }
                }
            }

            // Pass 2 - Bubble Sort to arrange the start parameters in ascending order
            bool swap_done = true;
            while (swap_done)
            {
                swap_done = false;
                for (int section_index = 0; section_index < section_count; section_index++)
                {
                    if (Convert.ToInt32(utilityviewmodel.sections[section_index][4]) > Convert.ToInt32(utilityviewmodel.sections[section_index + 1][4]))
                    {
                        for (int ray = 0; ray <= 6; ray++)
                        {
                            (utilityviewmodel.sections[section_index + 1][ray],
                                utilityviewmodel.sections[section_index][ray]) =
                            (utilityviewmodel.sections[section_index][ray],
                                utilityviewmodel.sections[section_index + 1][ray]);
                        }
                        //string temp = utilityviewmodel.sections[section_index][0];
                        //utilityviewmodel.sections[section_index][0] = utilityviewmodel.sections[section_index + 1][0];
                        //utilityviewmodel.sections[section_index + 1][0] = temp;

                        //temp = utilityviewmodel.sections[section_index][1];
                        //utilityviewmodel.sections[section_index][1] = utilityviewmodel.sections[section_index + 1][1];
                        //utilityviewmodel.sections[section_index + 1][1] = temp;

                        //temp = utilityviewmodel.sections[section_index][2];
                        //utilityviewmodel.sections[section_index][2] = utilityviewmodel.sections[section_index + 1][2];
                        //utilityviewmodel.sections[section_index + 1][2] = temp;

                        //temp = utilityviewmodel.sections[section_index][3];
                        //utilityviewmodel.sections[section_index][3] = utilityviewmodel.sections[section_index + 1][3];
                        //utilityviewmodel.sections[section_index + 1][3] = temp;

                        //temp = utilityviewmodel.sections[section_index][4];
                        //utilityviewmodel.sections[section_index][4] = utilityviewmodel.sections[section_index + 1][4];
                        //utilityviewmodel.sections[section_index + 1][4] = temp;

                        //temp = utilityviewmodel.sections[section_index][5];
                        //utilityviewmodel.sections[section_index][5] = utilityviewmodel.sections[section_index + 1][5];
                        //utilityviewmodel.sections[section_index + 1][5] = temp;

                        //temp = utilityviewmodel.sections[section_index][6];
                        //utilityviewmodel.sections[section_index][6] = utilityviewmodel.sections[section_index + 1][6];
                        //utilityviewmodel.sections[section_index + 1][6] = temp;

                        swap_done = true;
                    }
                }
            }

            // Pass3 - now work BACKWARDs down the list making sure the finish parameters are one
            // less than the start parameters  of the higher section
            for (int section_index = section_count; section_index >= 0; section_index--)
            {
                // Don't do the -1s
                if (utilityviewmodel.sections[section_index][4] != "-1")
                {
                    if (section_index == section_count)
                    {
                        utilityviewmodel.sections[section_index][5] = (total_lines - 1).ToString();

                    }
                    else
                    {
                        int finish = Convert.ToInt32(utilityviewmodel.sections[section_index + 1][4]);
                        if (finish > 0)
                        {
                            finish--;
                            utilityviewmodel.sections[section_index][5] = finish.ToString();
                        }
                    }
                }
            }
            return;
        }

        internal static string Date_Delimiters(string token)
        {
            if (!string.IsNullOrEmpty(token))
            {
                token = token.Replace(SmartParametersV2016.billDateDelimiter, SmartParametersV2016.billDateSeparator);
            }
            return token;
        }

        internal static bool Check_Big_Four(MainViewModel ourviewmodel, UtilityViewModel utilityviewmodel, bool update_message)
        {
            if (utilityviewmodel.area_code == 0 ||
                string.IsNullOrEmpty(utilityviewmodel.ACCOUNT_NO) ||
                string.IsNullOrEmpty(utilityviewmodel.statement_id) ||
                utilityviewmodel.BILL_DATE == SmartParametersV2016.defaultDates)
            {
                if (update_message)
                {
                    if (utilityviewmodel.area_code == 0)
                    {
                        ourviewmodel.errorMessage += " Area Code is 0";
                    }
                    if (string.IsNullOrEmpty(utilityviewmodel.ACCOUNT_NO))
                    {
                        ourviewmodel.errorMessage += " ACCOUNT_NO empty";
                    }
                    if (string.IsNullOrEmpty(utilityviewmodel.statement_id))
                    {
                        ourviewmodel.errorMessage += " STATEMENT_ID empty";
                    }
                    if (utilityviewmodel.BILL_DATE == SmartParametersV2016.defaultDates)
                    {
                        ourviewmodel.errorMessage += " BILL_DATE empty";
                    }
                }
                if (!string.IsNullOrEmpty(ourviewmodel.errorMessage))
                {
                    ourviewmodel.errorMessage = "" + SmartParametersV2016.bar + ourviewmodel.errorMessage.Trim();
                }
                return false;
            }
            return true;
        }

        internal static void Tariff_Ends_With_Resource(char resourceCode, UtilityViewModel utilityviewmodel)//rf string tariffName)
        {
            if (!string.IsNullOrEmpty(utilityviewmodel.TARIFF_NAME))
            {
                string resource = "";
                switch (resourceCode)
                {
                    case SmartParametersV2016.Electricity:
                        resource = "Electricity";
                        break;
                    case SmartParametersV2016.Gas:
                        resource = "Gas";
                        break;
                    default:
                        break;
                }
                if (!string.IsNullOrEmpty(resource))
                {
                    int resource_index = utilityviewmodel.TARIFF_NAME.IndexOf(resource);
                    if (resource_index >= 0)
                    {
                        if ((utilityviewmodel.TARIFF_NAME.Length - resource_index) >= 0)
                        {
                            utilityviewmodel.TARIFF_NAME = utilityviewmodel.TARIFF_NAME.Substring(0, resource_index).Trim();
                        }
                    }
                }
            }
            return;
        }

        internal static void Expand_Date(UtilityViewModel utilityviewmodel)//rf string tariffName)
        {
            int date_count = 0;
            foreach (string date in SmartParametersV2016.dates)
            {
                if (utilityviewmodel.TARIFF_NAME.IndexOf(date) >= 0)
                {
                    utilityviewmodel.TARIFF_NAME = utilityviewmodel.TARIFF_NAME.Replace(date, SmartParametersV2016.space + SmartParametersV2016.months[date_count] + SmartParametersV2016.space);
                    break;  // Only the first!
                }
                date_count++;
            }
            return;
        }

        //internal static bool fix_mprn(int line_count,
        //                                string[] lines,
        //                                string index5,
        //                                UtilityViewModel utilityviewmodel)
        //{
        //    string MPRN = "";
        //    bool mprn_found = false;
        //    string temp_urgent_message = "";

        //    int sub_line_count = line_count;
        //    sub_line_count = sub_line_count + 1;
        //    while (sub_line_count <= Convert.ToInt32(index5))
        //    {
        //        MPRN = lines[sub_line_count].Replace(SmartParametersV2016.space, "").Trim();
        //        if (SmartParseV2016.Generic_Parse_Digits(MPRN, rf temp_urgent_message))
        //        {
        //            mprn_found = true;
        //            utilityviewmodel.smell.MPRN = MPRN;
        //            break;
        //        }
        //        sub_line_count = sub_line_count + 1;
        //    }
        //    return mprn_found;
        //}

        //internal static bool fix_mprn_special(int line_count,
        //                        string[] lines,
        //                        string index5,
        //                        string index4,
        //                        UtilityViewModel utilityviewmodel,
        //                        string target,
        //                        bool spaces_are_forbidden,
        //                        bool Look_Backwards = false)
        //{
        //    bool s_found = false;
        //    string MPRN = "";
        //    int sub_line_count = line_count;
        //    string temp_urgent_message = "";
        //    switch (utilityviewmodel.resource_code)
        //    {
        //        case SmartParametersV2016.Gas:
        //            while (sub_line_count <= Convert.ToInt32(index5))
        //            {
        //                // Ray .. you are a fucking genius ... work forwards
        //                if (lines[sub_line_count].Trim() == target)
        //                {
        //                    s_found = true;
        //                }
        //                else
        //                {
        //                    if (s_found)
        //                    {
        //                        if (MPRN.Length < SmartParametersV2016.minimumMPRNLength)
        //                        {
        //                            string test = lines[sub_line_count].Trim();
        //                            if (spaces_are_forbidden)
        //                            {
        //                                if (test.IndexOf(SmartParametersV2016.space) >= 0)
        //                                {
        //                                    goto next_line_forwards;
        //                                }
        //                            }
        //                            //test = test.Replace(SmartParametersV2016.space, "").Trim();

        //                            if (SmartParseV2016.Generic_Parse_Digits(test, rf temp_urgent_message))
        //                            {
        //                                // These go in from left to right
        //                                MPRN = MPRN + test;
        //                                utilityviewmodel.smell.MPRN = MPRN;
        //                            }
        //                        }
        //                        else
        //                        {
        //                            utilityviewmodel.smell.MPRN = MPRN;
        //                            return s_found;
        //                        }
        //                    }
        //                }
        //                next_line_forwards:
        //                sub_line_count = sub_line_count + 1;
        //            }
        //            if (Look_Backwards)
        //            {
        //                if (MPRN.Length < SmartParametersV2016.minimumMPRNLength)
        //                {
        //                    // Now work backwards!!
        //                    MPRN = "";
        //                    s_found = false;
        //                    sub_line_count = line_count;
        //                    temp_urgent_message = "";
        //                    while (sub_line_count > Convert.ToInt32(index4))
        //                    {
        //                        // Ray
        //                        if (lines[sub_line_count].Trim() == target)
        //                        {
        //                            s_found = true;
        //                        }
        //                        else
        //                        {
        //                            if (s_found)
        //                            {
        //                                if (MPRN.Length < SmartParametersV2016.minimumMPRNLength)  // See above for Bob's
        //                                {
        //                                    string test = lines[sub_line_count].Trim();
        //                                    if (spaces_are_forbidden)
        //                                    {
        //                                        if (test.IndexOf(SmartParametersV2016.space) >= 0)
        //                                        {
        //                                            goto next_line_backwards;
        //                                        }
        //                                    }
        //                                    if (SmartParseV2016.Generic_Parse_Digits(test, rf temp_urgent_message))
        //                                    {
        //                                        // These go in from right to left!!
        //                                        MPRN = test + MPRN;
        //                                        utilityviewmodel.smell.MPRN = MPRN;

        //                                    }
        //                                }
        //                                else
        //                                {
        //                                    utilityviewmodel.smell.MPRN = MPRN;
        //                                    return s_found;
        //                                }
        //                            }
        //                        }
        //                        next_line_backwards:
        //                        sub_line_count = sub_line_count - 1;
        //                    }
        //                }
        //            }
        //            break;
        //        case SmartParametersV2016.Electricity:
        //            break;
        //        default:
        //            break;
        //    }
        //    return s_found;
        //}

        // This is for Npower bills circa 2012 only 
        //internal static bool fix_supply_number_special(int line_count,
        //                                string[] lines,
        //                                string index5,
        //                                UtilityViewModel utilityviewmodel)//,
        //                                //bool Look_Backwards = false)
        //{
        //    // https://www.energylinx.co.uk/mpan.htm
        //    string mpan_prfix = "S";
        //    string MPAN = "";
        //    bool s_found = false;
        //    int sub_line_count = line_count;
        //    string temp_urgent_message = "";

        //    int component_count = 0;
        //    switch (utilityviewmodel.resource_code)
        //    {
        //        case SmartParametersV2016.Electricity:
        //            while (sub_line_count <= Convert.ToInt32(index5))
        //            {
        //                // Ray .. you are a fucking genius ... work forwards
        //                if (lines[sub_line_count].Trim() == mpan_prfix)
        //                {
        //                    s_found = true;
        //                }
        //                else
        //                {
        //                    if (s_found)
        //                    {
        //                        if (MPAN.Replace(SmartParametersV2016.bar.ToString(), "").Length < SmartParametersV2016.minimumMPANLength) // Because for BG, Bob's is 01 801 3  then 10 1242 4329 980
        //                        {
        //                            string test = lines[sub_line_count].Replace(SmartParametersV2016.space, "").Trim();
        //                            if (SmartParseV2016.Generic_Parse_Digits(test, rf temp_urgent_message))
        //                            {
        //                                // These go in from left to right
        //                                switch (component_count)
        //                                {
        //                                    case 0:
        //                                        MPAN = test;
        //                                        break;
        //                                    case 1:
        //                                        MPAN = test + SmartParametersV2016.bar + MPAN;
        //                                        break;
        //                                    case 2:
        //                                        MPAN = MPAN.Replace(SmartParametersV2016.bar.ToString(), test);
        //                                        break;
        //                                    default:
        //                                        break;
        //                                }
        //                                component_count = component_count + 1;
        //                            }
        //                        }
        //                        else
        //                        {
        //                            utilityviewmodel.sparks.MPAN = mpan_prfix + MPAN;
        //                            return s_found;
        //                        }
        //                    }
        //                }
        //                sub_line_count = sub_line_count + 1;
        //            }
        //            break;
        //        case SmartParametersV2016.Gas:
        //            break;
        //        default:
        //            break;
        //    }
        //    return s_found;
        //}
        //internal static bool fix_supply_number(int line_count,
        //                                string[] lines,
        //                                string index5,
        //                                string index4,
        //                                UtilityViewModel utilityviewmodel,
        //                                bool spaces_are_a_must,
        //                                bool Look_Backwards = false)
        //{
        //    // https://www.energylinx.co.uk/mpan.htm
        //    string mpan_prfix = "S";
        //    string MPAN = "";
        //    bool s_found = false;
        //    int sub_line_count = line_count;
        //    string temp_urgent_message = "";
        //    switch (utilityviewmodel.resource_code)
        //    {
        //        case SmartParametersV2016.Electricity:
        //            while (sub_line_count <= Convert.ToInt32(index5))
        //            {
        //                // Ray .. you are a fucking genius ... work forwards
        //                if (lines[sub_line_count].Trim() == mpan_prfix)
        //                {
        //                    s_found = true;
        //                }
        //                else
        //                {
        //                    if (s_found)
        //                    {
        //                        if (MPAN.Length < SmartParametersV2016.minimumMPANLength) // Because for BG, Bob's is 01 801 3  then 10 1242 4329 980
        //                        {
        //                            string test = lines[sub_line_count].Trim();
        //                            if (spaces_are_a_must)
        //                            {
        //                                if (test.IndexOf(SmartParametersV2016.space) == -1)
        //                                {
        //                                    goto next_line_forwards;
        //                                }
        //                            }
        //                            test = test.Replace(SmartParametersV2016.space, "").Trim();

        //                            if (SmartParseV2016.Generic_Parse_Digits(test, rf temp_urgent_message))
        //                            {
        //                                // These go in from left to right
        //                                MPAN = MPAN + test;
        //                                utilityviewmodel.sparks.MPAN = mpan_prfix + MPAN;
        //                            }
        //                        }
        //                        else
        //                        {
        //                            utilityviewmodel.sparks.MPAN = mpan_prfix + MPAN;
        //                            return s_found;
        //                        }
        //                    }
        //                }
        //                next_line_forwards:
        //                sub_line_count = sub_line_count + 1;
        //            }
        //            if (Look_Backwards)
        //            {
        //                if (MPAN.Length < SmartParametersV2016.minimumMPANLength + 1)
        //                {
        //                    // Now work backwards!!
        //                    MPAN = "";
        //                    s_found = false;
        //                    sub_line_count = line_count;
        //                    temp_urgent_message = "";
        //                    while (sub_line_count > Convert.ToInt32(index4))
        //                    {
        //                        // Ray
        //                        if (lines[sub_line_count].Trim() == mpan_prfix)
        //                        {
        //                            s_found = true;
        //                        }
        //                        else
        //                        {
        //                            if (s_found)
        //                            {
        //                                if (MPAN.Length < SmartParametersV2016.minimumMPANLength)  // See above for Bob's
        //                                {
        //                                    string test = lines[sub_line_count].Trim();
        //                                    if (spaces_are_a_must)
        //                                    {
        //                                        if (test.IndexOf(SmartParametersV2016.space) == -1)
        //                                        {
        //                                            goto next_line_backwards;
        //                                        }
        //                                    }
        //                                    test = test.Replace(SmartParametersV2016.space, "").Trim();

        //                                    if (SmartParseV2016.Generic_Parse_Digits(test, rf temp_urgent_message))
        //                                    {
        //                                        // These go in from right to left!!
        //                                        MPAN = test + MPAN;
        //                                        utilityviewmodel.sparks.MPAN = mpan_prfix + MPAN;

        //                                    }
        //                                }
        //                                else
        //                                {
        //                                    //MPAN = mpan_prfix + MPAN;
        //                                    utilityviewmodel.sparks.MPAN = mpan_prfix + MPAN;
        //                                    return s_found;
        //                                }
        //                            }
        //                        }
        //                        next_line_backwards:
        //                        sub_line_count = sub_line_count - 1;
        //                    }
        //                }
        //            }
        //            break;
        //        case SmartParametersV2016.Gas:
        //            break;
        //        default:
        //            break;
        //    }
        //    return s_found;
        //}


        // Masha isn't The Bear you dumb fucker its Masha AND The Bear

        internal static bool Decode_Section(string section,
                                            string subSection,
                                            string[][] sections,
                                            string resourceCode,
                                            UtilityViewModel utilityviewmodel)//rf int currentIndex)
        {
            // What ARE we trying to say here?
            // If there is NO resource code then see if its >=0 and return true otherwise return false
            // If there IS a Resource Code then if see if it matches [2] and return true or false
            //  

            for (int section_index = 0; section_index <= sections.GetUpperBound(0); section_index++)
            {
                if ((section == sections[section_index][0]) &&
                    (subSection == sections[section_index][1]))
                {
                    if ((Convert.ToInt32(sections[section_index][4]) >= 0) &&
                        (Convert.ToInt32(sections[section_index][5]) >= 0))
                    {
                        if (!string.IsNullOrEmpty(sections[section_index][2]))
                        {
                            if (resourceCode != sections[section_index][2])
                            {
                                break;
                            }
                        }
                        utilityviewmodel.currentIndex = section_index;
                        return true;
                    }
                    break;
                }
            }
            return false;
        }

        internal static bool Token_Identify(UtilityViewModel utilityviewmodel, string comparison, bool replace, bool sameLength = false)
        {
            string[] identifiers = comparison.Split(SmartParametersV2016.bar);
            foreach (string identifier in identifiers)
            {
                if (!string.IsNullOrEmpty(utilityviewmodel.token) &&
                    !string.IsNullOrEmpty(identifier))
                {
                    bool something_to_compare = false;
                    if (!sameLength)
                    {
                        if (utilityviewmodel.token.Length >= identifier.Length)
                        {
                            something_to_compare = true;
                        }
                    }
                    else
                    {
                        if (utilityviewmodel.token.Length == identifier.Length)
                        {
                            something_to_compare = true;
                        }
                    }
                    if (something_to_compare)
                    {
                        if (utilityviewmodel.token.Substring(0, identifier.Length) == identifier)
                        {
                            if (replace)
                            {
                                utilityviewmodel.token = utilityviewmodel.token.Replace(identifier, "").Trim();
                            }
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        internal static bool Token_IdentifyWorking(UtilityViewModel utilityviewmodel, string comparison, bool replace, bool sameLength = false)
        {
            string[] identifiers = comparison.Split(SmartParametersV2016.bar);
            foreach (string identifier in identifiers)
            {
                if (!string.IsNullOrEmpty(utilityviewmodel.working_token) &&
                    !string.IsNullOrEmpty(identifier))
                {
                    bool something_to_compare = false;
                    if (!sameLength)
                    {
                        if (utilityviewmodel.working_token.Length >= identifier.Length)
                        {
                            something_to_compare = true;
                        }
                    }
                    else
                    {
                        if (utilityviewmodel.working_token.Length == identifier.Length)
                        {
                            something_to_compare = true;
                        }
                    }
                    if (something_to_compare)
                    {
                        if (utilityviewmodel.working_token.Substring(0, identifier.Length) == identifier)
                        {
                            if (replace)
                            {
                                utilityviewmodel.working_token = utilityviewmodel.working_token.Replace(identifier, "").Trim();
                            }
                            return true;
                        }
                    }
                }
            }
            return false;
        }
        internal static bool Token_Identify_End(UtilityViewModel utilityviewmodel, string identifier, bool replace)
        {
            if (!string.IsNullOrEmpty(utilityviewmodel.token) &&
                !string.IsNullOrEmpty(identifier))
            {
                if (utilityviewmodel.token.Length >= identifier.Length)
                {
                    if (utilityviewmodel.token.Substring(utilityviewmodel.token.Length - identifier.Length, identifier.Length) == identifier)
                    {
                        if (replace)
                        {
                            utilityviewmodel.token = utilityviewmodel.token.Substring(0, utilityviewmodel.token.Length - identifier.Length).Trim();
                        }
                        return true;
                    }
                }
            }
            return false;
        }

        internal static bool Token_Identify_Middle(UtilityViewModel utilityviewmodel, string comparison, bool replace, string withWhat)
        {
            string[] identifiers = comparison.Split(SmartParametersV2016.bar);
            foreach (string identifier in identifiers)
            {
                if (!string.IsNullOrEmpty(utilityviewmodel.token) &&
                !string.IsNullOrEmpty(identifier))
                {
                    if (utilityviewmodel.token.Length >= identifier.Length)
                    {
                        if (utilityviewmodel.token.IndexOf(identifier) >= 0)
                        {
                            if (replace)
                            {
                                utilityviewmodel.token = utilityviewmodel.token.Replace(identifier, withWhat).Trim();
                            }
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        internal static bool Token_Identify_Array(UtilityViewModel utilityviewmodel, string identifier, bool replace, bool sameLength = false)
        {
            if (!string.IsNullOrEmpty(utilityviewmodel.token))
            {
                string[] components = identifier.Split('|');
                foreach (string component in components)
                {
                    if (!string.IsNullOrEmpty(component))
                    {
                        bool something_to_compare = false;
                        if (!sameLength)
                        {
                            if (utilityviewmodel.token.Length >= component.Length)
                            {
                                something_to_compare = true;
                            }
                        }
                        else
                        {
                            if (utilityviewmodel.token.Length == component.Length)
                            {
                                something_to_compare = true;
                            }
                        }
                        if (something_to_compare)
                        {
                            if (utilityviewmodel.token.Substring(0, component.Length) == component)
                            {
                                if (replace)
                                {
                                    utilityviewmodel.token = utilityviewmodel.token.Replace(component, "").Trim();
                                }
                                return true;
                            }
                        }
                    }
                }
            }
            return false;
        }

        internal static void Initialize_Bill_Parse(MainViewModel ourviewmodel, UtilityViewModel utilityviewmodel)
        {
            utilityviewmodel.bill_version = 0;                   // Starting off value in case we don't find it
            utilityviewmodel.ACCOUNT_NO = utilityviewmodel.account_no;        // Starting off value in case we don't find it
            utilityviewmodel.BILL_DATE = SmartParametersV2016.defaultDates;      // Starting off value in case we don't find it
            // Because for SP the Bill Number IS the Bill Date, we need to re-format it
            // so we can work it out 'externally' (i.e. before downloading and opening the Bill_
            // from the Bill Date.  So we can't keep it as '1 March 2014' we have to 'normalize'
            // it to something like 01 Mar 2014 (at least)
            //
            // THE STATEMENT_ID is **always** fixed EXTERNALLY and not INTERNALLY from the Bill
            //
            utilityviewmodel.STATEMENT_ID = utilityviewmodel.statement_id;     // Starting off value - must never be empty
            utilityviewmodel.BILL_PERIOD_START = SmartParametersV2016.defaultDates;
            utilityviewmodel.BILL_PERIOD_END = SmartParametersV2016.defaultDates;
            utilityviewmodel.TARIFF_CODE = 0;
            utilityviewmodel.TARIFF_NAME = "";
            utilityviewmodel.PAYMENT_PLAN = SmartParametersV2016.defaultChar;
            utilityviewmodel.TCR = "n/a";
            ourviewmodel.errorMessage = "";
            utilityviewmodel.PAYMENTS_BALANCE = 0;
            utilityviewmodel.PAYMENTS_ITEM = 0;
            utilityviewmodel.ACCOUNT_CHARGES_CREDITS_ITEM = 0;
            utilityviewmodel.SUPPLY_CHARGES_CREDITS_ITEM = 0;
            utilityviewmodel.CALORIFIC_VALUE = "0";
            // Now, instead of doing my program I have to listen to - and waste time with -
            // this stupid fucking cow
        }

        internal static void Reformat_Discount_Credit_Bill(UtilityViewModel utilityviewmodel)
        {
            int discount_credit_date = 0;
            while (discount_credit_date < utilityviewmodel.DISCOUNTCREDITDATE.Length)
            {
                if (Char.IsDigit(Convert.ToChar(utilityviewmodel.DISCOUNTCREDITDATE.Substring(discount_credit_date, 1))))
                {
                    utilityviewmodel.DISCOUNTCREDITDATE = utilityviewmodel.DISCOUNTCREDITDATE.Substring(0, discount_credit_date) +
                                            SmartParametersV2016.space +
                                            utilityviewmodel.DISCOUNTCREDITDATE.Substring(discount_credit_date);
                    break;
                }
                discount_credit_date++;
            }
            return;
        }

        internal static string Remove_Double_Spaces_V3(string token)
        {
            // How stupid can you get??  GIVE ME THE ENTIRE FUCKING JOURNEY YOU DICK-HEAD not what
            // your dumb mate told you, but THE FUCKING JOURNEY
            if (!string.IsNullOrEmpty(token))
            {
                while (token.IndexOf("  ") >= 0)
                {
                    token = token.Replace("  ", SmartParametersV2016.space);
                }
            }
            // Drops out when there are no more double spaces
            return token;
        }

        internal static short Determine_Units_Band(MainViewModel ourviewmodel,
                                                    UtilityViewModel utilityviewmodel,
                                                    char UNITSTIME,
                                                    string UNITSRATE,
                                                    short unitsBand)
        {
            // Check to see if we use the previous UNITS_BAND or make a new one
            short temp_supplier_code = utilityviewmodel.supplier_code;
            short temp_brand_code = utilityviewmodel.brand_code;
            string temp_account_no = utilityviewmodel.ACCOUNT_NO;
            DateTime temp_created = utilityviewmodel.CREATED;
            string temp_statement_id = utilityviewmodel.statement_id;
            DateTime temp_bill_date = SmartTimeV2016.ConvertDateTime(utilityviewmodel.BILL_DATE);
            switch (utilityviewmodel.resource_code)
            {
                case SmartParametersV2016.Electricity:

                    List<SmartUtility.EUnitCharges> e_unit_charges_out_found = new List<SmartUtility.EUnitCharges>(from E_Unit_Charges
                                                                in utilityviewmodel.Hezbollah.e_unit_charges_changesList
                                                                                                                   where ((E_Unit_Charges.USERNAME == ourviewmodel.UserName) &&
                                                                                                                           (E_Unit_Charges.CUBEFACE_CODE == SmartParametersV2016.Utility) &&
                                                                                                                           (E_Unit_Charges.SUPPLIER_CODE == temp_supplier_code) &&
                                                                                                                           (E_Unit_Charges.BRAND_CODE == temp_brand_code) &&
                                                                                                                           (E_Unit_Charges.ACCOUNT_NO == temp_account_no) &&
                                                                                                                           (E_Unit_Charges.ACCOUNT_CREATED == temp_created) &&
                                                                                                                           (E_Unit_Charges.STATEMENT_ID == temp_statement_id) &&
                                                                                                                           (E_Unit_Charges.BILL_DATE == temp_bill_date) &&
                                                                                                                           (E_Unit_Charges.UNITS_TIME == UNITSTIME) &&
                                                                                                                           (E_Unit_Charges.UNITS_RATE == SmartRoutinesV2018.ConvertDecimal(UNITSRATE)))
                                                                                                                   select E_Unit_Charges);
                    if (e_unit_charges_out_found.Count == 0)
                    {
                        unitsBand = (short)(unitsBand + 1);
                    }
                    break;
                case SmartParametersV2016.Gas:
                    List<SmartUtility.GUnitCharges> g_unit_charges_out_found = new List<SmartUtility.GUnitCharges>(from G_Unit_Charges
                                                                in utilityviewmodel.Hezbollah.g_unit_charges_changesList
                                                                                                                   where ((G_Unit_Charges.USERNAME == ourviewmodel.UserName) &&
                                                                                                                           (G_Unit_Charges.CUBEFACE_CODE == SmartParametersV2016.Utility) &&
                                                                                                                           (G_Unit_Charges.SUPPLIER_CODE == temp_brand_code) &&
                                                                                                                           (G_Unit_Charges.BRAND_CODE == temp_brand_code) &&
                                                                                                                           (G_Unit_Charges.ACCOUNT_NO == temp_account_no) &&
                                                                                                                           (G_Unit_Charges.ACCOUNT_CREATED == temp_created) &&
                                                                                                                           (G_Unit_Charges.STATEMENT_ID == temp_statement_id) &&
                                                                                                                           (G_Unit_Charges.BILL_DATE == temp_bill_date) &&
                                                                                                                           (G_Unit_Charges.UNITS_RATE == SmartRoutinesV2018.ConvertDecimal(UNITSRATE)))
                                                                                                                   select G_Unit_Charges);
                    if (g_unit_charges_out_found.Count == 0)
                    {
                        unitsBand = (short)(unitsBand + 1);
                    }
                    break;
                default:
                    break;

            }
            return unitsBand;
        }

        internal static void Update_Charges_Item(UtilityViewModel utilityviewmodel)
        {
            // Check to see if we use the previous UNITS_BAND or make a new one
            short temp_supplier_code = utilityviewmodel.supplier_code;
            short temp_brand_code = utilityviewmodel.brand_code;
            string temp_account_no = utilityviewmodel.ACCOUNT_NO;
            DateTime temp_created = utilityviewmodel.CREATED;
            string temp_statement_id = utilityviewmodel.statement_id;
            DateTime temp_bill_date = SmartTimeV2016.ConvertDateTime(utilityviewmodel.BILL_DATE);
            switch (utilityviewmodel.resource_code)
            {
                case SmartParametersV2016.Electricity:
                    List<SmartUtility.EStandingCharges> e_standing_charges_out_found = new List<SmartUtility.EStandingCharges>(from E_Standing_Charges
                                                                        in utilityviewmodel.Hezbollah.e_standing_charges_changesList
                                                                                                                               where ((E_Standing_Charges.CUBEFACE_CODE == SmartParametersV2016.Utility) &&
                                                                                                                                       (E_Standing_Charges.SUPPLIER_CODE == temp_supplier_code) &&
                                                                                                                                       (E_Standing_Charges.BRAND_CODE == temp_brand_code) &&
                                                                                                                                       (E_Standing_Charges.ACCOUNT_NO == temp_account_no) &&
                                                                                                                                       (E_Standing_Charges.ACCOUNT_CREATED == temp_created) &&
                                                                                                                                       (E_Standing_Charges.STATEMENT_ID == temp_statement_id) &&
                                                                                                                                       (E_Standing_Charges.BILL_DATE == temp_bill_date))
                                                                                                                               select E_Standing_Charges);
                    if (e_standing_charges_out_found.Count > 0)
                    {
                        foreach (SmartUtility.EStandingCharges e_standing_charges_row in e_standing_charges_out_found)
                        {
                            e_standing_charges_row.CHARGES_ITEM = (short)(e_standing_charges_row.CHARGES_ITEM + 1);
                        }
                    }
                    break;
                case SmartParametersV2016.Gas:
                    List<SmartUtility.GStandingCharges> g_standing_charges_out_found = new List<SmartUtility.GStandingCharges>(from G_Standing_Charges
                                                                        in utilityviewmodel.Hezbollah.g_standing_charges_changesList
                                                                                                                               where ((G_Standing_Charges.CUBEFACE_CODE == SmartParametersV2016.Utility) &&
                                                                                                                                       (G_Standing_Charges.SUPPLIER_CODE == temp_supplier_code) &&
                                                                                                                                       (G_Standing_Charges.BRAND_CODE == temp_brand_code) &&
                                                                                                                                       (G_Standing_Charges.ACCOUNT_NO == temp_account_no) &&
                                                                                                                                       (G_Standing_Charges.ACCOUNT_CREATED == temp_created) &&
                                                                                                                                       (G_Standing_Charges.STATEMENT_ID == temp_statement_id) &&
                                                                                                                                       (G_Standing_Charges.BILL_DATE == temp_bill_date))
                                                                                                                               select G_Standing_Charges);
                    if (g_standing_charges_out_found.Count > 0)
                    {
                        foreach (SmartUtility.GStandingCharges g_standing_charges_row in g_standing_charges_out_found)
                        {
                            // This wouldn't compile because I had G_Standing_Charges defined
                            // as a 'struct' instead of a Class !!!
                            g_standing_charges_row.CHARGES_ITEM = (short)(g_standing_charges_row.CHARGES_ITEM + 1);
                        }
                    }
                    break;
                default:
                    break;
            }
            return;
        }

        internal static void Update_Bill(MainViewModel ourviewmodel,
                                            UtilityViewModel utilityviewmodel,
                                           string field_name,
                                           string parameter)
        {
            if (!string.IsNullOrEmpty(parameter))
            {
                // IF we have a Dual Fuel discount then we must be on it .. and modify the Bill to say so
                //char temp_resource_code = utilityviewmodel.resource_code;
                short temp_supplier_code = utilityviewmodel.supplier_code;
                short temp_brand_code = utilityviewmodel.brand_code;
                string temp_account_no = utilityviewmodel.ACCOUNT_NO;
                DateTime temp_created = utilityviewmodel.CREATED;
                string temp_statement_id = utilityviewmodel.STATEMENT_ID;
                DateTime temp_bill_date = SmartTimeV2016.ConvertDateTime(utilityviewmodel.BILL_DATE);
                int temp = 0;

                switch (field_name)
                {
                    case "ACCOUNT_CHARGES_CREDITS":
                        utilityviewmodel.Hezbollah.bills_row.ACCOUNT_CHARGES_CREDITS += Convert.ToInt32(parameter); // This is a TOTAL
                        break;
                    case "ACCOUNT_CHARGES_CREDITS_VAT_AMOUNT":
                        utilityviewmodel.Hezbollah.bills_row.ACCOUNT_CHARGES_CREDITS_VAT_AMOUNT += Convert.ToInt32(parameter); // This is a TOTAL
                        break;
                    case "ACCOUNT_NO":
                        if (string.IsNullOrEmpty(utilityviewmodel.Hezbollah.bills_row.ACCOUNT_NO))
                        {
                            utilityviewmodel.Hezbollah.bills_row.ACCOUNT_NO = parameter;
                        }
                        if (string.IsNullOrEmpty(utilityviewmodel.Hezbollah.bills_resource_row.ACCOUNT_NO))
                        {
                            utilityviewmodel.Hezbollah.bills_resource_row.ACCOUNT_NO = parameter;
                        }
                        if (string.IsNullOrEmpty(utilityviewmodel.Hezbollah.bills_resource_row.RESOURCE_ACCOUNT_NO))
                        {
                            utilityviewmodel.Hezbollah.bills_resource_row.RESOURCE_ACCOUNT_NO = parameter;
                        }
                        break;
                    case "CREATED":
                        if (utilityviewmodel.Hezbollah.bills_row.ACCOUNT_CREATED == SmartParametersV2016.defaultDate)
                        {
                            utilityviewmodel.Hezbollah.bills_row.ACCOUNT_CREATED = SmartTimeV2016.ConvertDateTime(parameter);
                        }
                        if (utilityviewmodel.Hezbollah.bills_resource_row.ACCOUNT_CREATED == SmartParametersV2016.defaultDate)
                        {
                            utilityviewmodel.Hezbollah.bills_resource_row.ACCOUNT_CREATED = SmartTimeV2016.ConvertDateTime(parameter);
                        }
                        break;
                    case "STATEMENT_ID":
                        // Don't forget - this is the WEBPAGE Id (not the date of the bill)
                        if (string.IsNullOrEmpty(utilityviewmodel.Hezbollah.bills_row.STATEMENT_ID))
                        {
                            utilityviewmodel.Hezbollah.bills_row.STATEMENT_ID = parameter;
                        }
                        if (string.IsNullOrEmpty(utilityviewmodel.Hezbollah.bills_resource_row.STATEMENT_ID))
                        {
                            utilityviewmodel.Hezbollah.bills_resource_row.STATEMENT_ID = parameter;
                        }
                        break;
                    case "BILL_DATE":
                        if (utilityviewmodel.Hezbollah.bills_row.BILL_DATE == SmartParametersV2016.defaultDate)
                        {
                            utilityviewmodel.Hezbollah.bills_row.BILL_DATE = SmartTimeV2016.ConvertDateTime(parameter);
                        }
                        if (utilityviewmodel.Hezbollah.bills_resource_row.BILL_DATE == SmartParametersV2016.defaultDate)
                        {
                            utilityviewmodel.Hezbollah.bills_resource_row.BILL_DATE = SmartTimeV2016.ConvertDateTime(parameter);
                        }
                        break;
                    case "BILL_PERIOD_START":
                        // We haven't already done this bill in this current Read Meter
                        utilityviewmodel.Hezbollah.bills_row.BILL_PERIOD_START = SmartTimeV2016.ConvertDateTime(parameter);
                        break;
                    case "BILL_PERIOD_END":
                        // We haven't already done this bill in this current Read Meter
                        utilityviewmodel.Hezbollah.bills_row.BILL_PERIOD_END = SmartTimeV2016.ConvertDateTime(parameter);
                        break;
                    case "DISCOUNTS":
                        // This is a RUNNING TOTAL
                        utilityviewmodel.Hezbollah.bills_resource_row.RESOURCE_DISCOUNTS += Convert.ToInt32(parameter);
                        break;
                    case "DIRECT_DEBIT_DATE":
                        if (utilityviewmodel.Hezbollah.bills_row.DIRECT_DEBIT_DATE == SmartParametersV2016.defaultDate)
                        {
                            utilityviewmodel.Hezbollah.bills_row.DIRECT_DEBIT_DATE = SmartTimeV2016.ConvertDateTime(parameter);
                        }
                        break;
                    case "DISCOUNT_CREDIT_DATE":
                        if (utilityviewmodel.Hezbollah.bills_row.DISCOUNT_CREDIT_DATE == SmartParametersV2016.defaultDate)
                        {
                            utilityviewmodel.Hezbollah.bills_row.DISCOUNT_CREDIT_DATE = SmartTimeV2016.ConvertDateTime(parameter);
                        }
                        break;
                    case "RESOURCE_NEW_CHARGES":
                        // We haven't already done this bill in this current Read Meter
                        utilityviewmodel.Hezbollah.bills_resource_row.NEW_CHARGES += Convert.ToInt32(parameter);
                        break;
                    case "RESOURCE_VAT_AMOUNT":
                        // We haven't already done this bill in this current Read Meter
                        utilityviewmodel.Hezbollah.bills_resource_row.RESOURCE_VAT_AMOUNT += Convert.ToInt32(parameter);
                        break;
                    case "OUTSTANDING_BALANCE":
                        // We haven't already done this bill in this current Read Meter
                        utilityviewmodel.Hezbollah.bills_row.OUTSTANDING_BALANCE = Convert.ToInt32(parameter);
                        break;
                    case "RESOURCE_BALANCES":
                        // We haven't already done this bill in this current Read Meter
                        utilityviewmodel.Hezbollah.bills_row.RESOURCE_BALANCES = Convert.ToBoolean(parameter);
                        break;
                    case "OUTSTANDING_RESOURCE_BALANCE":
                        // We haven't already done this bill in this current Read Meter
                        utilityviewmodel.Hezbollah.bills_resource_row.OUTSTANDING_RESOURCE_BALANCE = Convert.ToInt32(parameter);
                        break;
                    case "PAYMENT_AMOUNT":
                        utilityviewmodel.PAYMENTS_BALANCE += Convert.ToInt32(parameter);
                        break;
                    case "PAYMENT_PLAN":
                        if (utilityviewmodel.Hezbollah.bills_row.PAYMENT_PLAN == SmartParametersV2016.defaultChar)
                        {
                            utilityviewmodel.Hezbollah.bills_row.PAYMENT_PLAN = Convert.ToChar(parameter);
                        }
                        break;
                    case "PAYMENT_DUE_DATE":
                        if (utilityviewmodel.Hezbollah.bills_row.PAYMENT_DUE_DATE == SmartParametersV2016.defaultDate)
                        {
                            utilityviewmodel.Hezbollah.bills_row.PAYMENT_DUE_DATE = SmartTimeV2016.ConvertDateTime(parameter);
                        }
                        break;
                    case "PAYMENTS_RECEIVED":
                        // We haven't already done this bill in this current Read Meter
                        utilityviewmodel.Hezbollah.bills_row.PAYMENTS_RECEIVED = Convert.ToInt32(parameter); // <= This is a TOTAL
                        break;
                    case "PAYMENT_TYPE":
                        // We haven't already done this bill in this current Read Meter
                        if (string.IsNullOrEmpty(utilityviewmodel.Hezbollah.bills_row.PAYMENT_TYPE))
                        {
                            utilityviewmodel.Hezbollah.bills_row.PAYMENT_TYPE = parameter.Substring(0, parameter.Length <= SmartParametersV2016.PAYMENTTYPESLENGTH ? parameter.Length : SmartParametersV2016.PAYMENTTYPESLENGTH);
                        }
                        break;
                    case "PREVIOUS_BALANCE":
                        // We haven't already done this bill in this current Read Meter
                        utilityviewmodel.Hezbollah.bills_row.PREVIOUS_BALANCE = Convert.ToInt32(parameter);
                        break;
                    case "PREVIOUS_RESOURCE_BALANCE":
                        // We haven't already done this bill in this current Read Meter
                        utilityviewmodel.Hezbollah.bills_resource_row.PREVIOUS_RESOURCE_BALANCE = Convert.ToInt32(parameter);
                        break;
                    case "RESOURCE_ACCOUNT_NO":
                        utilityviewmodel.Hezbollah.bills_resource_row.RESOURCE_ACCOUNT_NO = parameter;
                        break;
                    case "REWARDS":
                        // We haven't already done this bill in this current Read Meter
                        if (string.IsNullOrEmpty(utilityviewmodel.Hezbollah.bills_row.REWARDS))
                        {
                            utilityviewmodel.Hezbollah.bills_row.REWARDS = parameter;
                        }
                        break;
                    case "SUPPLY_CHARGES_CREDITS":
                        utilityviewmodel.Hezbollah.bills_row.SUPPLY_CHARGES_CREDITS += Convert.ToInt32(parameter); // This is a TOTAL
                        break;
                    case "SUPPLY_CHARGES_CREDITS_VAT_AMOUNT":
                        utilityviewmodel.Hezbollah.bills_row.SUPPLY_CHARGES_CREDITS_VAT_AMOUNT += Convert.ToInt32(parameter); // This is a TOTAL
                        break;
                    case "TARIFF_CODE":
                        // We haven't already done this bill in this current Read Meter
                        if (utilityviewmodel.Hezbollah.bills_resource_row.TARIFF_CODE == 0)
                        {
                            utilityviewmodel.Hezbollah.bills_resource_row.TARIFF_CODE = Convert.ToInt32(parameter);
                        }
                        break;
                    case "TOTAL_NOW_DUE":
                        // We haven't already done this bill in this current Read Meter
                        utilityviewmodel.Hezbollah.bills_row.TOTAL_NOW_DUE = Convert.ToInt32(parameter);
                        break;
                    case "MONTHLY_PAYMENT":
                        // We haven't already done this bill in this current Read Meter
                        utilityviewmodel.Hezbollah.bills_row.MONTHLY_PAYMENT = Convert.ToInt32(parameter);
                        break;
                    case "RESOURCE_VAT_CODE":
                        // We haven't already done this bill in this current Read Meter
                        utilityviewmodel.Hezbollah.bills_resource_row.RESOURCE_VAT_CODE = Convert.ToInt16(parameter);
                        break;
                    case "BILL_VAT_CODE":
                        // We haven't already done this bill in this current Read Meter
                        utilityviewmodel.Hezbollah.bills_row.BILL_VAT_CODE = Convert.ToInt16(parameter);
                        break;
                    case "BILL_VAT_AMOUNT":
                        // You can't have it both ways Ray!!!!
                        // British Gas have SEPARATE Bills so the VAT amount for Electricity
                        // can go in E_VAT_AMOUNT of Bills_Resource for 'E' and the VAT amount
                        // for Gas can go in Bills_Resource for 'G'
                        //
                        // First Utility have a COMBINED Bill but they split out the VAT for
                        // each of Gas and Electricity (For you) so like BG,
                        // the VAT amount for Electricity
                        // can go in E_VAT_AMOUNT of Bills_Resource for 'E' and the VAT amount
                        // for Gas can go in Bills_Resource for 'G'
                        //
                        // However ScottishPower paki fuckers have a COMBINED Bill and they
                        // DON'T split out the VAT for Electricity and Gas.  The fuckers combine it
                        // into one total (for the Bill) which we have to hold in the Bill.  We
                        // can calculate each of the Electricty and Gas VATs as we go through,
                        // but those are OUR CALCULATIONS and are not values we pick up from
                        // the Bill like we do for BG and FU.  WE have to do the split because
                        // these Foreigners are too stupid or lazy to do it for themselves.
                        // So ... when we process the Bill in SmartBills, we will have to check that
                        // our 'VAT split' into Electricity and Gas actually add up to the Bill
                        // VAT amount total and adjust accordingly.  What a fucking fag!  All becuase
                        // of a PENNY difference!!! 

                        // We haven't already done this bill in this current Read Meter
                        utilityviewmodel.Hezbollah.bills_row.BILL_VAT_AMOUNT = Convert.ToInt32(parameter); // This is a TOTAL VAT for a Bill
                        break;
                    case "UNITS_COST":
                        switch (utilityviewmodel.resource_code)
                        {
                            case SmartParametersV2016.Electricity:
                                List<SmartUtility.EUnitCharges> e_unit_charges_out_found = new List<SmartUtility.EUnitCharges>(from E_Unit_Charges
                                                                                in utilityviewmodel.Hezbollah.e_unit_charges_changesList
                                                                                                                               where ((E_Unit_Charges.USERNAME == ourviewmodel.UserName) &&
                                                                                                                                      (E_Unit_Charges.CUBEFACE_CODE == SmartParametersV2016.Utility) &&
                                                                                                                                      (E_Unit_Charges.SUPPLIER_CODE == temp_supplier_code) &&
                                                                                                                                      (E_Unit_Charges.BRAND_CODE == temp_brand_code) &&
                                                                                                                                      (E_Unit_Charges.ACCOUNT_NO == temp_account_no) &&
                                                                                                                                      (E_Unit_Charges.ACCOUNT_CREATED == temp_created) &&
                                                                                                                                      (E_Unit_Charges.STATEMENT_ID == temp_statement_id) &&
                                                                                                                                      (E_Unit_Charges.BILL_DATE == temp_bill_date))
                                                                                                                               select E_Unit_Charges);
                                if (e_unit_charges_out_found.Count > 0)
                                {
                                    foreach (SmartUtility.EUnitCharges e_unit_charges_row in e_unit_charges_out_found)
                                    {
                                        temp += e_unit_charges_row.UNITS_COST;
                                    }
                                    utilityviewmodel.Hezbollah.bills_resource_row.NEW_CHARGES += temp;
                                }
                                break;
                            case SmartParametersV2016.Gas:
                                List<SmartUtility.GUnitCharges> g_unit_charges_out_found = new List<SmartUtility.GUnitCharges>(from G_Unit_Charges
                                                                                in utilityviewmodel.Hezbollah.g_unit_charges_changesList
                                                                                                                               where ((G_Unit_Charges.USERNAME == ourviewmodel.UserName) &&
                                                                                                                                      (G_Unit_Charges.CUBEFACE_CODE == SmartParametersV2016.Utility) &&
                                                                                                                                      (G_Unit_Charges.SUPPLIER_CODE == temp_supplier_code) &&
                                                                                                                                      (G_Unit_Charges.BRAND_CODE == temp_brand_code) &&
                                                                                                                                       (G_Unit_Charges.ACCOUNT_NO == temp_account_no) &&
                                                                                                                                       (G_Unit_Charges.ACCOUNT_CREATED == temp_created) &&
                                                                                                                                       (G_Unit_Charges.STATEMENT_ID == temp_statement_id) &&
                                                                                                                                       (G_Unit_Charges.BILL_DATE == temp_bill_date))
                                                                                                                               select G_Unit_Charges);
                                if (g_unit_charges_out_found.Count > 0)
                                {
                                    foreach (SmartUtility.GUnitCharges g_unit_charges_row in g_unit_charges_out_found)
                                    {
                                        temp += g_unit_charges_row.UNITS_COST;
                                    }
                                    utilityviewmodel.Hezbollah.bills_resource_row.NEW_CHARGES += temp;
                                }
                                break;
                            default:
                                break;
                        }
                        break;
                    case "CHARGES_COST":
                        switch (utilityviewmodel.resource_code)
                        {
                            case SmartParametersV2016.Electricity:
                                List<SmartUtility.EStandingCharges> e_standing_charges_out_found = new List<SmartUtility.EStandingCharges>(from E_Standing_Charges
                                                                                        in utilityviewmodel.Hezbollah.e_standing_charges_changesList
                                                                                                                                           where (//(E_Standing_Charges.USERNAME == username) &&
                                                                                                                                                  (E_Standing_Charges.CUBEFACE_CODE == SmartParametersV2016.Utility) &&
                                                                                                                                                  (E_Standing_Charges.SUPPLIER_CODE == temp_supplier_code) &&
                                                                                                                                                  (E_Standing_Charges.BRAND_CODE == temp_brand_code) &&
                                                                                                                                                   (E_Standing_Charges.ACCOUNT_NO == temp_account_no) &&
                                                                                                                                                   (E_Standing_Charges.ACCOUNT_CREATED == temp_created) &&
                                                                                                                                                   (E_Standing_Charges.STATEMENT_ID == temp_statement_id) &&
                                                                                                                                                   (E_Standing_Charges.BILL_DATE == temp_bill_date))
                                                                                                                                           select E_Standing_Charges);
                                if (e_standing_charges_out_found.Count > 0)
                                {
                                    foreach (SmartUtility.EStandingCharges e_standing_charges_row in e_standing_charges_out_found)
                                    {
                                        temp += e_standing_charges_row.CHARGES_COST;
                                    }
                                    utilityviewmodel.Hezbollah.bills_resource_row.NEW_CHARGES += temp;
                                }
                                break;
                            case SmartParametersV2016.Gas:
                                List<SmartUtility.GStandingCharges> g_standing_charges_out_found = new List<SmartUtility.GStandingCharges>(from G_Standing_Charges
                                                                                        in utilityviewmodel.Hezbollah.g_standing_charges_changesList
                                                                                                                                           where (//(G_Standing_Charges.USERNAME == username) &&
                                                                                                                                                  (G_Standing_Charges.CUBEFACE_CODE == SmartParametersV2016.Utility) &&
                                                                                                                                                  (G_Standing_Charges.SUPPLIER_CODE == temp_supplier_code) &&
                                                                                                                                                  (G_Standing_Charges.BRAND_CODE == temp_brand_code) &&
                                                                                                                                                   (G_Standing_Charges.ACCOUNT_NO == temp_account_no) &&
                                                                                                                                                   (G_Standing_Charges.ACCOUNT_CREATED == temp_created) &&
                                                                                                                                                   (G_Standing_Charges.STATEMENT_ID == temp_statement_id) &&
                                                                                                                                                   (G_Standing_Charges.BILL_DATE == temp_bill_date))
                                                                                                                                           select G_Standing_Charges);
                                if (g_standing_charges_out_found.Count > 0)
                                {
                                    foreach (SmartUtility.GStandingCharges g_standing_charges_row in g_standing_charges_out_found)
                                    {
                                        temp += g_standing_charges_row.CHARGES_COST;
                                    }
                                    utilityviewmodel.Hezbollah.bills_resource_row.NEW_CHARGES += temp;
                                }
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

        internal static void Check_Vat_For_Pennies_Difference(MainViewModel ourviewmodel,
                                                                UtilityViewModel utilityviewmodel)
        {
            if (utilityviewmodel.check_vat)
            {
                int TEMP = utilityviewmodel.Hezbollah.bills_row.SUPPLY_CHARGES_CREDITS;
                if (TEMP != 0)
                {
                    // It IS there ... so what's the BILL_VAT_CODE and BILL_VAT_AMOUNT
                    // Ok lets see if the VAT is gonna 'add up'
                    // Look up the Bill Vat Rate
                    decimal bill_vat_rate = 0.0M;
                    if (!Lookup_Vat_Rate(utilityviewmodel.Hezbollah.bills_row.BILL_VAT_CODE,
                                         utilityviewmodel.Hezbollah.bills_row.BILL_PERIOD_START,
                                         utilityviewmodel.Hezbollah.bills_row.BILL_PERIOD_END,
                                         ourviewmodel.Blanche.vatRatesList,
                                         utilityviewmodel))
                    {
                        return;
                    }
                    else
                    {
                        bill_vat_rate = utilityviewmodel.VAT_RATE;
                    }
                    // Work out the Supply Charge VAT
                    int supply_vat_amount = Convert.ToInt32((TEMP * bill_vat_rate) / 100.0M);
                    int amount_to_split = utilityviewmodel.Hezbollah.bills_row.BILL_VAT_AMOUNT - supply_vat_amount;

                    // Find any Bill Resource already 'stored out'
                    char temp_cubeface_code = utilityviewmodel.Hezbollah.bills_row.CUBEFACE_CODE;
                    short temp_supplier_code = utilityviewmodel.Hezbollah.bills_row.SUPPLIER_CODE;
                    short temp_brand_code = utilityviewmodel.Hezbollah.bills_row.BRAND_CODE;
                    string temp_account_no = utilityviewmodel.Hezbollah.bills_row.ACCOUNT_NO;
                    DateTime temp_created = utilityviewmodel.Hezbollah.bills_row.ACCOUNT_CREATED;
                    string temp_statement_id = utilityviewmodel.Hezbollah.bills_row.STATEMENT_ID;
                    DateTime temp_bill_date = utilityviewmodel.Hezbollah.bills_row.BILL_DATE;
                    List<SmartUtility.BillsResource> bills_resource_found = new List<SmartUtility.BillsResource>(from Bill_Resource in utilityviewmodel.Hezbollah.bills_resource_changesList
                                                                                                                 where (Bill_Resource.CUBEFACE_CODE == temp_cubeface_code &&
                                                                                                                        Bill_Resource.SUPPLIER_CODE == temp_supplier_code &&
                                                                                                                        Bill_Resource.BRAND_CODE == temp_brand_code &&
                                                                                                                        Bill_Resource.ACCOUNT_NO == temp_account_no &&
                                                                                                                        Bill_Resource.ACCOUNT_CREATED == temp_created &&
                                                                                                                        Bill_Resource.STATEMENT_ID == temp_statement_id &&
                                                                                                                        Bill_Resource.BILL_DATE == temp_bill_date)
                                                                                                                 select Bill_Resource);
                    if (bills_resource_found.Count > 0)
                    {
                        foreach (SmartUtility.BillsResource bills_resource_row in bills_resource_found)
                        {
                            if ((bills_resource_row.NEW_CHARGES != 0) &&
                                (bills_resource_row.RESOURCE_VAT_CODE == utilityviewmodel.Hezbollah.bills_row.BILL_VAT_CODE) &&
                                (bills_resource_row.RESOURCE_VAT_AMOUNT != 0))
                            {
                                amount_to_split -= bills_resource_row.RESOURCE_VAT_AMOUNT;
                            }
                        }
                    }

                    if ((utilityviewmodel.Hezbollah.bills_resource_row.RESOURCE_VAT_CODE == utilityviewmodel.Hezbollah.bills_row.BILL_VAT_CODE) &&
                        (amount_to_split != utilityviewmodel.Hezbollah.bills_resource_row.RESOURCE_VAT_AMOUNT))
                    {
                        utilityviewmodel.Hezbollah.bills_resource_row.RESOURCE_VAT_AMOUNT = amount_to_split;
                    }
                }
            }
            return;
        }

        internal static bool Add_Minus_Sign(UtilityViewModel utilityviewmodel)
        {
            if (utilityviewmodel.value.IndexOf(SmartParametersV2016.minus) == -1)
            {
                utilityviewmodel.value = SmartParametersV2016.minus + utilityviewmodel.value;
                return true;
            }
            return false;
        }

        internal static void Remove_Minus_SignX(UtilityViewModel utilityviewmodel)
        {
            if (utilityviewmodel.value.IndexOf(SmartParametersV2016.minus) >= 0)
            {
                utilityviewmodel.value = utilityviewmodel.value.Replace(SmartParametersV2016.minus, "");
            }
            return;
        }
        //    // Sorry but the Tariff you are on has got NOTHING to do with the Readings!
        //    // YES, but the Tariff Details ARE related to the Readings, because there
        //    // could be several readings and the unit rate might change for each of them
        //    // Yes, the Unit Rate might well change but all we are interested in IS the UNIT_RATE
        //    // not what was read on that date (i.e. the Reading) All we are trying to build is
        //    // a Unit_Rates table 'line' which will allow us to check the real thing held
        //    // in SmartUtility.

        //    // The Problem with FU is a) the days they work out are WRONG (check 29 Apr 2015 11372987)
        //    // and b) they do the Standing charges in the WRONG ORDER (i.e. they do the first one last)
        //    // For example: the rates changed on1st Apr 2015 according to Energylinx and not on 31st March 2015
        //    // according to the bill.  Second, some of the readings are outside of the Bill Period .. but
        //    // what the fuck ...

        //    // Work down the list of Unit Charges until you have a Unique set of Rates
        //    // We need a Tariff Detail for each of these - if there is only ONE, then we have one Tariif_Detail
        //    // record, but if there are more than one, we have one for each.
        //    // Then work BACKWARD down the Standing Charges(!!) we should have a matching Standing
        //    // Charge for each set of unqie Unit Charges (if not => don;t record anything)

        //    // But it has everything to do with how much you are charged!!
        //    switch (utilityviewmodel.resource_code)
        //    {
        //        case SmartParametersV2016.Electricity:
        //            // Ewan McCoy instead of Ewan MCCOLL!!
        //            // and it was Ryan GOSLING you tit head not Ryan O-Neal!!
        //            // AND its a FUNICULAR not a fucking VERNACULAR!!!  You fucking bonehead


        // Aminated <= animated !!
        // The is the BG one and everyone elses
        internal static void Update_Tariff_Details(MainViewModel ourviewmodel,
                                                    UtilityViewModel utilityviewmodel)
        {
            short temp_supplier_code = utilityviewmodel.supplier_code;
            short temp_brand_code = utilityviewmodel.brand_code;
            string temp_account_no = utilityviewmodel.ACCOUNT_NO;
            DateTime temp_created = utilityviewmodel.CREATED;
            string temp_statement_id = utilityviewmodel.STATEMENT_ID;
            DateTime temp_bill_date = SmartTimeV2016.ConvertDateTime(utilityviewmodel.BILL_DATE);

            // Sorry but the Tariff you are on has got NOTHING to do with the Readings!
            // But it has everything to do with how much you are charged!!
            switch (utilityviewmodel.resource_code)
            {
                case SmartParametersV2016.Electricity:
                    // Ewan McCoy instead of Ewan MCCOLL!!
                    // and it was Ryan GOSLING you tit head not Ryan O-Neal!!
                    // AND its a FUNICULARE not a fucking VERNACULAR!!!  You fucking bonehead

                    // Now do the Unit Charges
                    List<SmartUtility.EUnitCharges> e_unit_charges_out_found = new List<SmartUtility.EUnitCharges>(from E_Unit_Charges
                                                                    in utilityviewmodel.Hezbollah.e_unit_charges_changesList
                                                                                                                   where ((E_Unit_Charges.CUBEFACE_CODE == SmartParametersV2016.Utility) &&
                                                                                                                           (E_Unit_Charges.SUPPLIER_CODE == temp_supplier_code) &&
                                                                                                                           (E_Unit_Charges.BRAND_CODE == temp_brand_code) &&
                                                                                                                           (E_Unit_Charges.ACCOUNT_NO == temp_account_no) &&
                                                                                                                           (E_Unit_Charges.ACCOUNT_CREATED == temp_created) &&
                                                                                                                           (E_Unit_Charges.STATEMENT_ID == temp_statement_id) &&
                                                                                                                           (E_Unit_Charges.BILL_DATE == temp_bill_date) &&
                                                                                                                           (E_Unit_Charges.UNITS_TIME == SmartParametersV2016.daytimeUnit))
                                                                                                                   select E_Unit_Charges);
                    foreach (SmartUtility.EUnitCharges e_unit_charges_out_row in e_unit_charges_out_found)
                    {
                        // Default the Standing Charge
                        string SC = "0.0";
                        // Set the Day Rate
                        string DR = e_unit_charges_out_row.UNITS_RATE.ToString();
                        // Default the Night Rate
                        string NR = "0.0";

                        List<SmartUtility.EUnitCharges> e_unit_charges_night_found = new List<SmartUtility.EUnitCharges>(from E_Unit_Charges
                                                                            in utilityviewmodel.Hezbollah.e_unit_charges_changesList
                                                                                                                         where ((E_Unit_Charges.CUBEFACE_CODE == SmartParametersV2016.Utility) &&
                                                                                                                                 (E_Unit_Charges.SUPPLIER_CODE == temp_supplier_code) &&
                                                                                                                                 (E_Unit_Charges.BRAND_CODE == temp_brand_code) &&
                                                                                                                                 (E_Unit_Charges.ACCOUNT_NO == temp_account_no) &&
                                                                                                                                 (E_Unit_Charges.ACCOUNT_CREATED == temp_created) &&
                                                                                                                                 (E_Unit_Charges.STATEMENT_ID == temp_statement_id) &&
                                                                                                                                 (E_Unit_Charges.BILL_DATE == temp_bill_date) &&
                                                                                                                                 (E_Unit_Charges.UNIT_CHARGES_PERIOD_START == e_unit_charges_out_row.UNIT_CHARGES_PERIOD_START) &&
                                                                                                                                 (E_Unit_Charges.UNITS_TIME == SmartParametersV2016.nighttimeUnit))
                                                                                                                         select E_Unit_Charges);
                        if (e_unit_charges_night_found.Count > 0)
                        {
                            foreach (SmartUtility.EUnitCharges e_unit_charges_night_row in e_unit_charges_night_found)
                            {
                                // Set the Night Rate
                                NR = e_unit_charges_night_row.UNITS_RATE.ToString();
                            }
                        }

                        // Do the Standing Charges first
                        List<SmartUtility.EStandingCharges> e_standing_charges_out_found = new List<SmartUtility.EStandingCharges>(from E_Standing_Charges
                                                                                in utilityviewmodel.Hezbollah.e_standing_charges_changesList
                                                                                                                                   where ((E_Standing_Charges.CUBEFACE_CODE == SmartParametersV2016.Utility) &&
                                                                                                                                           (E_Standing_Charges.SUPPLIER_CODE == temp_supplier_code) &&
                                                                                                                                           (E_Standing_Charges.BRAND_CODE == temp_brand_code) &&
                                                                                                                                           (E_Standing_Charges.ACCOUNT_NO == temp_account_no) &&
                                                                                                                                           (E_Standing_Charges.ACCOUNT_CREATED == temp_created) &&
                                                                                                                                           (E_Standing_Charges.STATEMENT_ID == temp_statement_id) &&
                                                                                                                                           (E_Standing_Charges.BILL_DATE == temp_bill_date) &&
                                                                                                                                           // You will never match the dates
                                                                                                                                           (E_Standing_Charges.STANDING_CHARGES_PERIOD_START >= e_unit_charges_out_row.UNIT_CHARGES_PERIOD_START) &&
                                                                                                                                           (E_Standing_Charges.STANDING_CHARGES_PERIOD_END <= e_unit_charges_out_row.UNIT_CHARGES_PERIOD_END) &&
                                                                                                                                           (E_Standing_Charges.CHARGES_ITEM == e_unit_charges_out_row.UNITS_BAND)) // But you should match the band/item
                                                                                                                                   select E_Standing_Charges);
                        // We may have 0 (for a Two-Tier) or 1 (for a Standing Charge) of these for each COST GROUP
                        switch (e_standing_charges_out_found.Count)
                        {
                            case 0:
                                break;
                            case 1:
                                SC = e_standing_charges_out_found[0].STANDING_CHARGE.ToString();
                                break;
                            default:
                                ourviewmodel.errorMessage = "Too many STANDING CHARGES: " + e_standing_charges_out_found.Count;
                                break;
                        }
                        // Add the Tariff details
                        SmartUtilityV2022.TariffDetailsList_Add(utilityviewmodel,

                                                                SmartParametersV2016.Utility,
                                                                utilityviewmodel.supplier_code,
                                                                utilityviewmodel.brand_code,
                                                                utilityviewmodel.ACCOUNT_NO,
                                                                utilityviewmodel.CREATED,
                                                                utilityviewmodel.STATEMENT_ID,
                                                                utilityviewmodel.BILL_DATE,
                                                                utilityviewmodel.sparks.MPAN,
                                                                e_unit_charges_out_row.UNIT_CHARGES_PERIOD_START,
                                                                e_unit_charges_out_row.UNIT_CHARGES_PERIOD_END,
                                                                utilityviewmodel.TARIFF_CODE,
                                                                utilityviewmodel.PAYMENT_PLAN,
                                                                e_unit_charges_out_row.UNITS_BAND,
                                                                utilityviewmodel.area_code,
                                                                SC,
                                                                DR,
                                                                NR,
                                                                utilityviewmodel.TCR);// New one - Tariff Comparison Rate
                    }
                    break;

                case SmartParametersV2016.Gas:
                    // Ewan McCoy instead of Ewan MCCOLL!!
                    // and it was Ryan GOSLING you tit head not Ryan O-Neal!!
                    // AND its a FUNICULARE not a fucking VERNACULAR!!!  You fucking bonehead

                    // Now do the Unit Charges
                    List<SmartUtility.GUnitCharges> g_unit_charges_out_found = new List<SmartUtility.GUnitCharges>(from G_Unit_Charges
                                                                    in utilityviewmodel.Hezbollah.g_unit_charges_changesList
                                                                                                                   where ((G_Unit_Charges.CUBEFACE_CODE == SmartParametersV2016.Utility) &&
                                                                                                                           (G_Unit_Charges.SUPPLIER_CODE == temp_supplier_code) &&
                                                                                                                           (G_Unit_Charges.BRAND_CODE == temp_brand_code) &&
                                                                                                                           (G_Unit_Charges.ACCOUNT_NO == temp_account_no) &&
                                                                                                                           (G_Unit_Charges.ACCOUNT_CREATED == temp_created) &&
                                                                                                                           (G_Unit_Charges.STATEMENT_ID == temp_statement_id) &&
                                                                                                                           (G_Unit_Charges.BILL_DATE == temp_bill_date))
                                                                                                                   select G_Unit_Charges);
                    foreach (SmartUtility.GUnitCharges g_unit_charges_out_row in g_unit_charges_out_found)
                    {
                        // Default the Standing Charge
                        string SC = "0.0";
                        // Set the Day Rate
                        string DR = g_unit_charges_out_row.UNITS_RATE.ToString();
                        // Default the Night Rate
                        string NR = "0.0";

                        // Do the Standing Charges first
                        List<SmartUtility.GStandingCharges> g_standing_charges_out_found = new List<SmartUtility.GStandingCharges>(from G_Standing_Charges
                                                                                in utilityviewmodel.Hezbollah.g_standing_charges_changesList
                                                                                                                                   where ((G_Standing_Charges.CUBEFACE_CODE == SmartParametersV2016.Utility) &&
                                                                                                                                           (G_Standing_Charges.SUPPLIER_CODE == temp_supplier_code) &&
                                                                                                                                           (G_Standing_Charges.BRAND_CODE == temp_brand_code) &&
                                                                                                                                           (G_Standing_Charges.ACCOUNT_NO == temp_account_no) &&
                                                                                                                                           (G_Standing_Charges.ACCOUNT_CREATED == temp_created) &&
                                                                                                                                           (G_Standing_Charges.STATEMENT_ID == temp_statement_id) &&
                                                                                                                                           (G_Standing_Charges.BILL_DATE == temp_bill_date) &&
                                                                                                                                           // You will never match the dates
                                                                                                                                           (G_Standing_Charges.STANDING_CHARGES_PERIOD_START >= g_unit_charges_out_row.UNIT_CHARGES_PERIOD_START) &&
                                                                                                                                           (G_Standing_Charges.STANDING_CHARGES_PERIOD_END <= g_unit_charges_out_row.UNIT_CHARGES_PERIOD_END) &&
                                                                                                                                           (G_Standing_Charges.CHARGES_ITEM == g_unit_charges_out_row.UNITS_BAND)) // But you should match the band/item
                                                                                                                                   select G_Standing_Charges);
                        // We may have 0 (for a Two-Tier) or 1 (for a Standing Charge) of these for each COST GROUP
                        switch (g_standing_charges_out_found.Count)
                        {
                            case 0:
                                break;
                            case 1:
                                SC = g_standing_charges_out_found[0].STANDING_CHARGE.ToString();
                                break;
                            default:
                                ourviewmodel.errorMessage = "Too many STANDING CHARGES: " + g_standing_charges_out_found.Count;
                                break;
                        }
                        // Add the Tariff details
                        SmartUtilityV2022.TariffDetailsList_Add(utilityviewmodel,

                                                               SmartParametersV2016.Utility,
                                                                utilityviewmodel.supplier_code,
                                                                utilityviewmodel.brand_code,
                                                                utilityviewmodel.ACCOUNT_NO,
                                                                utilityviewmodel.CREATED,
                                                                utilityviewmodel.STATEMENT_ID,
                                                                utilityviewmodel.BILL_DATE,
                                                                utilityviewmodel.smell.MPRN,
                                                                g_unit_charges_out_row.UNIT_CHARGES_PERIOD_START,
                                                                g_unit_charges_out_row.UNIT_CHARGES_PERIOD_END,
                                                                utilityviewmodel.TARIFF_CODE,
                                                                utilityviewmodel.PAYMENT_PLAN,
                                                                g_unit_charges_out_row.UNITS_BAND,
                                                                utilityviewmodel.area_code,
                                                                SC,
                                                                DR,
                                                                NR,
                                                                utilityviewmodel.TCR);// New one - Tariff Comparison Rate
                    }
                    break;
                default:
                    break;
            }
            return;
        }

        internal static string Find_Account(UtilityViewModel utilityviewmodel, int line_count, string[] lines)
        {
            while (line_count + 1 < lines.Length)
            {
                line_count++;
                if (!string.IsNullOrEmpty(lines[line_count]))
                {
                    string temp_account = "";
                    char[] characters = lines[line_count].Replace(SmartParametersV2016.space, "").Trim().ToCharArray();
                    foreach (char digit in characters)
                    {
                        if (Char.IsDigit(digit))
                        {
                            temp_account += digit.ToString();
                        }
                        else
                        {
                            temp_account = "";
                            break;
                        }
                    }
                    if (!string.IsNullOrEmpty(temp_account))
                    {
                        return temp_account;
                    }
                }
            }
            return "";
        }

#if WINFORMS
        internal static string find_next_string(UtilityViewModel utilityviewmodel, int line_count, string[] lines, int increment)
        {
            int target = 0;
            switch (increment)
            {
                case -1:
                    break;
                case 1:
                    target = lines.Length;
                    break;
                default:
                    break;
            }

            while (line_count + increment != target)
            {
                line_count = line_count + increment;
                if (!string.IsNullOrEmpty(lines[line_count]))
                {
                    return lines[line_count];
                }
            }
            return "";
        }
#endif

        internal static string Find_Bill_Date(UtilityViewModel utilityviewmodel, int line_count, string[] lines, string dates_null, string token)
        {
            if (!string.IsNullOrEmpty(token))
            {
                return token;
            }
            while (line_count + 1 < lines.Length)
            {
                line_count++;
                if (!string.IsNullOrEmpty(lines[line_count]))
                {
                    return lines[line_count].Trim();
                }
            }
            return dates_null;
        }

        internal static bool Currency_Strip(string bill_currency_symbol,
                                            char bill_currency_separator,
                                            char bill_thousands_separator,
                                            string token,
                                            UtilityViewModel utilityviewmodel)

        //rf string value,
        //rf string errorMessage)
        {

            // The type of accounting works like this:
            //      British Gas run Utility.Accounts on a 'credit' basis so 
            //              value CR    means ADD value to your balance
            //              value DR    means SUBTRACT value from your balance
            //
            //      However, the Foreigners at First Utility run Utility.Accounts on a 'debit' basis so
            //              value CR    means SUBTRACT the value from your balance
            //              value DR    means ADD the value to your balance
            //
            //      But whatever the Foreigners do, this routine works for ALL of them
            //      and it's TEN TIMES BETTER than what I had before!
            //
            if (string.IsNullOrEmpty(token))
            {
                return false;
            }

            int currency_index = token.IndexOf(bill_currency_symbol);
            if (currency_index == -1)
            {
                utilityviewmodel.errorMessage = "Currency symbol not found";
                return false;
            }
            token = token.Replace(bill_currency_symbol, "").Trim();

            if (token.IndexOf(SmartParametersV2016.minus) >= 0)
            {
                token = token.Replace(SmartParametersV2016.space, "").Trim();
                goto i_love_this_part;
            }
            if (token.IndexOf(SmartParametersV2016.plus) >= 0)
            {
                token = token.Replace(SmartParametersV2016.plus, "").Trim();
                goto i_love_this_part;
            }
            if (token.IndexOf("debit") >= 0)
            {
                token = token.Replace("in", "").Trim();   // Just in case
                token = token.Replace("debit", "").Trim();
                goto i_love_this_part;
            }
            if (token.IndexOf("DR") >= 0)
            {
                token = token.Replace("DR", "").Trim();
                goto i_love_this_part;
            }
            if (token.IndexOf("dr") >= 0)
            {
                token = token.Replace("dr", "").Trim();
                goto i_love_this_part;
            }
            if (token.IndexOf("credit") >= 0)
            {
                token = token.Replace("in", "").Trim();   // Just in case
                token = token.Replace("credit", "").Trim();
                token = SmartParametersV2016.minus + token;
                goto i_love_this_part;
            }
            if (token.IndexOf("CR") >= 0)
            {
                token = token.Replace("CR", "").Trim();
                token = SmartParametersV2016.minus + token;
                goto i_love_this_part;
            }
            if (token.IndexOf("cr") >= 0)
            {
                token = token.Replace("cr", "").Trim();
                token = SmartParametersV2016.minus + token;
                goto i_love_this_part;  // < Even more!
            }
        i_love_this_part:
            token = token.Replace(SmartParametersV2016.space, "").Trim();

            utilityviewmodel.value = token;
            utilityviewmodel.value = utilityviewmodel.value.Replace(bill_currency_separator.ToString(), "");
            utilityviewmodel.value = utilityviewmodel.value.Replace(bill_thousands_separator.ToString(), "");
            //try
            //{
            //    int new_value = Convert.ToInt32(value);
            //}
            //catch (Exception) // exception)
            //{
            //}
            return true;
        }

        // I don't need this shit anymore!!


        //    string minus = "";
        //    string dash = "-";
        //    if (!string.IsNullOrEmpty(token))
        //    {
        //        string trailer = "";
        //        int currency_index = token.IndexOf(bill_currency_symbol.ToString());
        //        if (currency_index >= 0)
        //        {
        //            // Was the character before the currency symbol a - ?
        //            if (currency_index >= 1)
        //            {
        //                if (token.Substring(currency_index - 1, 1) == dash)
        //                {
        //                    minus = dash;
        //                    currency_index = currency_index - 1;
        //                    goto adjustment;
        //                }
        //            }
        //            // Does 'in credit' appear before the 'currency symbol'?
        //            int credit_index = token.IndexOf("credit");
        //            if ((credit_index >= 0) &&
        //                (credit_index < currency_index))
        //            {
        //                // We found "credit" before the currency symbol
        //                trailer = " credit";
        //                // NOTE: the currency_index we return isn't adjusted!
        //            }

        //            // Does 'in debit' appear before the 'currency symbol'?
        //            int debit_index = token.IndexOf("debit");
        //            if ((debit_index >= 0) &&
        //                (debit_index < currency_index))
        //            {
        //                // We found "debit" before the currency symbol
        //                trailer = " debit";
        //                // NOTE: the currency_index we return isn't adjusted!
        //            }
        //   adjustment:
        //            token = token.Substring(currency_index).Replace(bill_currency_symbol.ToString(),
        //                                                            "").Trim() + trailer;
        //            // Did a - appear AFTER the currency symbol e.g. £-224.33??
        //            if (token.IndexOf(dash) == 0)
        //            {
        //                minus = dash;
        //                token = token.Substring(1);
        //            }
        //            if (!string.IsNullOrEmpty(token))
        //            {
        //                token = token.Replace("in", "");

        //                bool credit_found = false;
        //                // This logic assumes we never have a debit AND a credit signature ...
        //                string[] credits = new string[2] { "credit", "cr" };
        //                foreach (string credit in credits)
        //                {
        //                    if (token.IndexOf(credit) >= 0 ||
        //                        token.IndexOf(credit.ToUpper()) >= 0)
        //                    {
        //                        token = token.Replace(credit, "").Trim();
        //                        token = token.Replace(credit.ToUpper(), "").Trim();
        //                        credit_found = true;
        //                        switch (sub_or_add(analysis_names, analysis_code))
        //                        {
        //                            case "sub":
        //                                switch (minus)
        //                                {
        //                                    case "-":
        //                                        minus = ""; // -1.23CR becomes +1.23 so sub will be -1.23
        //                                        break;
        //                                    case "":
        //                                        minus = dash;         // 1.23CR becomes -1.23   so sub will be +1.23
        //                                        break;
        //                                    default:
        //                                        break;
        //                                }
        //                                break;
        //                            case "add":
        //                                switch (minus)
        //                                {
        //                                    case "-":
        //                                        minus = "";    // -1.23CR stays -1.23 so add will be -1.23
        //                                        break;
        //                                    case "":
        //                                        //minus = dash;           // 1.23CR stays 1.23   so add will be +1.23
        //                                        break;
        //                                    default:
        //                                        break;
        //                                }
        //                                break;
        //                            default:
        //                                break;
        //                        }
        //                        break;
        //                    }
        //                }
        //                if (!credit_found)
        //                {
        //                    // Look for a debit
        //                    string[] debits = new string[2] { "debit", "dr" };
        //                    foreach (string debit in debits)
        //                    {
        //                        if (token.IndexOf(debit) >= 0 ||
        //                            token.IndexOf(debit.ToUpper()) >= 0)
        //                        {
        //                            token = token.Replace(debit, "").Trim();
        //                            token = token.Replace(debit.ToUpper(), "").Trim();
        //                            switch (sub_or_add(analysis_names, analysis_code))
        //                            {
        //                                case "sub":
        //                                    switch (minus)
        //                                    {
        //                                        case "-":
        //                                            //minus = ""; // -1.23DR stays -1.23 so sub will be +1.23
        //                                            break;
        //                                        case "":
        //                                            //minus = dash;         // 1.23DR stays  1.23   so sub will be -1.23
        //                                            break;
        //                                        default:
        //                                            break;
        //                                    }
        //                                    break;
        //                                case "add":
        //                                    switch (minus)
        //                                    {
        //                                        case "-":
        //                                            //minus = "";    // -1.23DR becomes 1.23 so add will be +1.23
        //                                            break;
        //                                        case "":
        //                                            minus = dash;           // 1.23DR becomes   -1.23   so add will be -1.23
        //                                            break;
        //                                        default:
        //                                            break;
        //                                    }
        //                                    break;
        //                                default:
        //                                    break;
        //                            }
        //                            break;
        //                        }
        //                    }
        //                }
        //                value = minus + token.Replace(SmartParametersV2016.decimalPoint, "");
        //            }
        //            index = currency_index; // true;
        //            return true;
        //        }
        //    }
        //    errorMessage = "Currency symbol not found";
        //    return false;
        //}



        internal static bool Lookup_Vat_Code(string target_rate,
                                                DateTime target_date,
                                                List<SmartUsers.VatRates> vatRatesList,
                                                UtilityViewModel utilityviewmodel)
        {
            // Kludgy at the moment as we assume VAT is constant across the date range,
            // but it mightnot always be so ....
            foreach (SmartUsers.VatRates vat_rates_row in vatRatesList)
            {
                if (vat_rates_row.VAT_RATE == target_rate)
                {
                    DateTime valid_from = vat_rates_row.VALID_FROM;
                    DateTime valid_to = vat_rates_row.VALID_TO;
                    // Find a VAT range which covers 'from' and 'to'
                    if ((SmartRoutinesV2018.DateTimeCompare(valid_from, target_date) <= 0) &&
                        (SmartRoutinesV2018.DateTimeCompare(target_date, valid_to) <= 0))
                    {
                        utilityviewmodel.vat_code = Convert.ToInt16(vat_rates_row.VAT_CODE);
                        utilityviewmodel.errorMessage = "";
                        return true;
                    }
                }
            }
            utilityviewmodel.errorMessage = "Cannot find VAT rate for " + target_date.ToString(SmartParametersV2016.defaultCulture);
            return false;   // Can't find matching rate
        }

        internal static bool Generic_Parse_Datetime_Culture_enGB(string date_value, FinanceViewModel financeviewmodel)
        {
            //if (DateTime.TryParse(date_value, SmartParametersV2016.defaultCulture, DateTimeStyles.None, out target_date))
            //{
            //    return true;
            //}

            try
            {
                DateTime date = DateTime.ParseExact(date_value, "MM/dd/yyyy HH:mm:ss", SmartParametersV2016.cultureUSD);
                financeviewmodel.genericDateTime = date; //.ToString(SmartParametersV2016.militaryFormat, SmartParametersV2016.defaultCulture);
                return true;
            }
            catch (Exception ex)
            {
                financeviewmodel.errorMessage = "Cannot convert date: " + date_value + ex.Message;
            }
            return false;
        }

        //internal static bool Generic_Parse_Datetime_CultureX(string date_value, rf DateTime target_date, rf string message)
        //{
        //    if (DateTime.TryParse(date_value, CultureInfo.CurrentCulture, DateTimeStyles.None, out target_date))
        //    {
        //        return true;
        //    }
        //    message = "Cannot convert date: " + date_value;
        //    return false;
        //}

        internal static bool Generic_Parse_Datetime(string date_value, UtilityViewModel utilityviewmodel)
        {
            if (DateTime.TryParse(date_value, out utilityviewmodel.genericTargetDate))
            {
                return true;
            }
            utilityviewmodel.genericErrorMessage = "Cannot convert date: " + date_value;
            return false;
        }

        internal static bool Generic_Parse_Integer(string integer_value, UtilityViewModel utilityviewmodel)
        {
            if (Int32.TryParse(integer_value, out utilityviewmodel.genericTransactionValue))
            {
                return true;
            }
            utilityviewmodel.genericErrorMessage = "Cannot convert int: " + integer_value;
            return false;
        }

        internal static bool Generic_Parse_Decimal(string decimal_value, UtilityViewModel utilityviewmodel)
        {
            if (Decimal.TryParse(decimal_value, out utilityviewmodel.genericDecimalValue))
            {
                return true;
            }
            utilityviewmodel.genericErrorMessage = "Cannot convert decimal: " + decimal_value;
            return false;
        }

        internal static bool Generic_Parse_Digits(string value, UtilityViewModel utilityviewmodel)
        {
            if (string.IsNullOrEmpty(value))
            {
                return false;
            }
            else
            {
                char[] array = value.ToCharArray();
                foreach (char this_one in array)
                {
                    if (!Char.IsDigit(this_one))
                    {
                        utilityviewmodel.genericErrorMessage = "Cannot convert string: " + value;
                        return false;
                    }
                }
            }
            return true;
        }

        internal static bool Check_TempList(MainViewModel ourviewmodel,
                                            UtilityViewModel utilityviewmodel,
                                            DateTime PAYMENT_DATE,
                                            short payment_item,
                                            short PAYMENT_CODE,
                                            int PAYMENT_AMOUNT,
                                            int PAYMENT_BALANCE,
                                            List<SmartUtility.Payments> payments_tempList)
        {
            short supplier_code = utilityviewmodel.supplier_code;
            short brand_code = utilityviewmodel.brand_code;
            //char resource_code = utilityviewmodel.resource_code;
            string account_no = utilityviewmodel.account_no;     // We are not in a Bill yet for this to be uppercase!! You MORON
            DateTime created = utilityviewmodel.created;

            // Was it in a previous Bill?
            // I will have NO IDEA of the Item ...
            List<SmartUtility.Payments> payments_temp_found = new List<SmartUtility.Payments>
                (from Payment in payments_tempList
                 where ((Payment.USERNAME == ourviewmodel.UserName) &&
                         (Payment.CUBEFACE_CODE == SmartParametersV2016.Utility) &&
                         (Payment.SUPPLIER_CODE == supplier_code) &&
                         (Payment.BRAND_CODE == brand_code) &&
                         (Payment.ACCOUNT_NO == account_no) &&
                         (Payment.ACCOUNT_CREATED == created) &&
                         (Payment.PAYMENT_DATE == PAYMENT_DATE))
                 select Payment);
            // Have we already got this payment recorded?
            if (payments_temp_found.Count > 0)
            {
                // Found some - check 'em out
                foreach (SmartUtility.Payments payments_row in payments_temp_found)
                {
                    // Test on everything but WITHOUT the BILL field
                    if ((payments_row.CUBEFACE_CODE == SmartParametersV2016.Utility) &&
                        (payments_row.SUPPLIER_CODE == supplier_code) &&
                        (payments_row.BRAND_CODE == brand_code) &&
                        (payments_row.ACCOUNT_NO == account_no) &&
                        (payments_row.ACCOUNT_CREATED == created) &&
                        (payments_row.PAYMENT_DATE == PAYMENT_DATE) &&
                        (payments_row.PAYMENT_ITEM == payment_item) &&
                        (payments_row.PAYMENT_CODE == PAYMENT_CODE) &&
                        (payments_row.PAYMENT_AMOUNT == PAYMENT_AMOUNT) &&
                        (payments_row.PAYMENT_BALANCE == PAYMENT_BALANCE))
                    {
                        // It matches on these, so UPDATE THE RESOURCE_CODE and ignore it
                        // payments_row.RESOURCE_CODE = SmartParametersV2016.crossResourceCode;
                        return true;
                    }
                }
            }
            // WE can insert it - it is not in Payments Temp
            // to the best of my knowledge
            // MUST be good to go ...(!!!)
            return false;
        }

        internal static bool Check_Payment_Is_There(MainViewModel ourviewmodel,
                                                    UtilityViewModel utilityviewmodel)
        {
            short supplier_code = utilityviewmodel.supplier_code;
            short brand_code = utilityviewmodel.brand_code;
            //char resource_code = utilityviewmodel.resource_code;
            string account_no = utilityviewmodel.ACCOUNT_NO;     // Because THIS time its from a Bill, THAT'S WHY it can be in uppercase
            DateTime created = utilityviewmodel.CREATED;
            string statement_id = utilityviewmodel.STATEMENT_ID;
            DateTime bill_date = SmartTimeV2016.ConvertDateTime(utilityviewmodel.BILL_DATE);

            // Was it in a previous Bill?
            // I will have NO IDEA of the Item ...
            List<SmartUtility.Payments> payments_in_found = SmartSpikeUtilityV2017.Utility_PaymentsIn(ourviewmodel,
                                                                            utilityviewmodel,
                                                                            supplier_code,
                                                                            brand_code,
                                                                            account_no,
                                                                            created,
                                                                            statement_id,
                                                                            bill_date,
                                                                            Convert.ToDateTime(utilityviewmodel.PAYMENT_DATE));
            // Have we already got this payment recorded?
            if (payments_in_found.Count > 0)
            {
                // Found some - check 'em out
                foreach (SmartUtility.Payments payments_row in payments_in_found)
                {
                    // Test on everything but WITHOUT the BILL and ITEM fields
                    if ((payments_row.PAYMENT_CODE == Convert.ToInt16(utilityviewmodel.PAYMENT_CODE)) &&
                        (payments_row.PAYMENT_AMOUNT.ToString() == utilityviewmodel.PAYMENT_AMOUNT) &&
                        (payments_row.PAYMENT_BALANCE.ToString() == utilityviewmodel.PAYMENT_BALANCE))
                    {
                        // It matches on these, so UPDATE THE RESOURCE_CODE and ignore it
                        //payments_row.RESOURCE_CODE = SmartParametersV2016.crossResourceCode;
                        return true;
                    }
                }
            }

            // Now check Payments out
            List<SmartUtility.Payments> payments_out_found = SmartSpikeUtilityV2017.Utility_PaymentsOut(ourviewmodel,
                                                                                utilityviewmodel,
                                                                                supplier_code,
                                                                                brand_code,
                                                                                account_no,
                                                                                created,
                                                                                statement_id,
                                                                                bill_date,
                                                                                Convert.ToDateTime(utilityviewmodel.PAYMENT_DATE));
            // Have we already got this payment recorded?
            if (payments_out_found.Count == 0)
            {
                utilityviewmodel.payment_item = 1;
            }
            else
            {
                // Found some - check 'em out
                foreach (SmartUtility.Payments payments_row in payments_out_found)
                {
                    // I have re-done this .. because e.g. some Foreigners use the same bill
                    // for Gas as well as Electricity, so the Bill Number will be the same
                    // as well as the date etc.  But we only want to hold ONE entry for a 
                    // payment which applies to both of them.  But it could be that (for various
                    // reasons) we don't get the item number right, so we are only
                    // going to compare on these:
                    if ((payments_row.PAYMENT_CODE == Convert.ToUInt16(utilityviewmodel.PAYMENT_CODE)) &&
                         (payments_row.PAYMENT_AMOUNT.ToString() == utilityviewmodel.PAYMENT_AMOUNT) &&
                         (payments_row.PAYMENT_BALANCE.ToString() == utilityviewmodel.PAYMENT_BALANCE))
                    {
                        // Its there - it DEFINITELY matches on all the primary keys
                        // SO WE CAN NEVER, EVER INSERT IT, irrespective of the item
                        //payments_row.RESOURCE_CODE = SmartParametersV2016.crossResourceCode;
                        return true;
                    }
                }
                // We haven't returned, so fix the payment_item
                utilityviewmodel.payment_item = (short)(payments_out_found[0].PAYMENT_ITEM + 1);
            }
            // WE can insert it - it is neither in Payments IN nor Payments OUT
            // to the best of my knowledge
            // MUST be good to go ...(!!!)
            return false;
        }

        // 100,000 metres above sea level !!! 45 miles up!!! She is SUCH A GORMLESS dumb fucker
        internal static bool Check_Adjustment_Is_There(MainViewModel ourviewmodel,
                                                    UtilityViewModel utilityviewmodel,
                                                    DateTime adjustmentDate,
                                                    short adjustmentItem,
                                                    string ADJUSTMENTTYPE,
                                                    short ADJUSTMENTVATCODE,
                                                    int ADJUSTMENTAMOUNT)
        {
            // Sometimes .. there is a problem with this:
            // If we have the same bill (i.e. with the same Supplier, Account, Bill No, Account Date,
            // then the account Item might be the same if we read the same bill for both E and G
            // AND there are different items for E and G e.g. the E has transfer to G and the
            // G has transfer to E.  It might just happen, well, in fact - it does!!
            // So we need to be clever and search initially for everything up to but not 
            // including the Account Item, in ascending Account Item order (so we can get to
            // the last one easily).  Then we iterate throught the list and see if we match
            // on incoming item no and existing item no.
            // If we don't find an Account Item match, them we can return 'false'
            // and add in our incoming item no.
            // But if we DO find an Account Item  match, then before returning 'true' to say its there, we need to
            // look at the Account Type, Account Vat Code and Account Amount.  If the incoming
            // values for these three match the existing ones, then return true.
            // If the incoming values for these three DON'T match the existing ones, then make a
            // New Account Item and THEN return false.
            short supplier_code = utilityviewmodel.supplier_code;
            short brand_code = utilityviewmodel.brand_code;
            string account_no = utilityviewmodel.ACCOUNT_NO;
            DateTime created = utilityviewmodel.CREATED;
            string statement_id = utilityviewmodel.STATEMENT_ID;
            DateTime bill_date = SmartTimeV2016.ConvertDateTime(utilityviewmodel.BILL_DATE);

            // Is it already there in the Ins?
            List<SmartUtility.AccChargesCredits> account_charges_credits_found =
                            new List<SmartUtility.AccChargesCredits>(from Account_Charges_Credit
                                in utilityviewmodel.Hezbollah.account_charges_creditsList
                                                                     where ((Account_Charges_Credit.USERNAME == ourviewmodel.UserName) &&
                                                                             (Account_Charges_Credit.CUBEFACE_CODE == SmartParametersV2016.Utility) &&
                                                                             (Account_Charges_Credit.SUPPLIER_CODE == supplier_code) &&
                                                                             (Account_Charges_Credit.BRAND_CODE == brand_code) &&
                                                                             (Account_Charges_Credit.ACCOUNT_NO == account_no) &&
                                                                             (Account_Charges_Credit.ACCOUNT_CREATED == created) &&
                                                                             (Account_Charges_Credit.STATEMENT_ID == statement_id) &&
                                                                             (Account_Charges_Credit.BILL_DATE == bill_date) &&
                                                                             (Account_Charges_Credit.ACCOUNT_DATE == adjustmentDate))
                                                                     orderby Account_Charges_Credit.ACCOUNT_ITEM ascending
                                                                     select Account_Charges_Credit);
            // Have we already got this payment recorded?
            if (account_charges_credits_found.Count > 0)
            {
                // Found some - check 'em out
                foreach (SmartUtility.AccChargesCredits account_charges_credits_row in account_charges_credits_found)
                {
                    // Does the Item match?
                    if (account_charges_credits_row.ACCOUNT_ITEM == adjustmentItem)
                    {
                        if ((account_charges_credits_row.ACCOUNT_TYPE.ToString() == ADJUSTMENTTYPE) &&
                            (account_charges_credits_row.ACCOUNT_VAT_CODE == ADJUSTMENTVATCODE) &&
                            (account_charges_credits_row.ACCOUNT_AMOUNT == ADJUSTMENTAMOUNT))
                        {
                            // Its there - on the right date wiv the right type and the right balance
                            return true;
                        }
                    }
                }
            }

            // Is it already there in Outs?
            account_charges_credits_found = new List<SmartUtility.AccChargesCredits>(from Account_Charges_Credit
                                                in utilityviewmodel.Hezbollah.account_charges_credits_changesList
                                                                                     where ((Account_Charges_Credit.CUBEFACE_CODE == SmartParametersV2016.Utility) &&
                                                                                             (Account_Charges_Credit.SUPPLIER_CODE == supplier_code) &&
                                                                                             (Account_Charges_Credit.BRAND_CODE == brand_code) &&
                                                                                             (Account_Charges_Credit.ACCOUNT_NO == account_no) &&
                                                                                             (Account_Charges_Credit.ACCOUNT_CREATED == created) &&
                                                                                             (Account_Charges_Credit.STATEMENT_ID == statement_id) &&
                                                                                             (Account_Charges_Credit.BILL_DATE == bill_date) &&
                                                                                             (Account_Charges_Credit.ACCOUNT_DATE == adjustmentDate))
                                                                                     orderby Account_Charges_Credit.ACCOUNT_ITEM ascending
                                                                                     select Account_Charges_Credit);
            // Have we already got this payment recorded?
            if (account_charges_credits_found.Count > 0)
            {
                // Found some - check 'em out
                foreach (SmartUtility.AccChargesCredits account_charges_credits_row in account_charges_credits_found)
                {
                    // Does the Item match?
                    if (account_charges_credits_row.ACCOUNT_ITEM == adjustmentItem)
                    {
                        if ((account_charges_credits_row.ACCOUNT_TYPE.ToString() == ADJUSTMENTTYPE) &&
                            (account_charges_credits_row.ACCOUNT_VAT_CODE == ADJUSTMENTVATCODE) &&
                            (account_charges_credits_row.ACCOUNT_AMOUNT == ADJUSTMENTAMOUNT))
                        {
                            // Its there - on the right date wiv the right type and the right balance
                            return true;
                        }
                        else
                        {
                            short next_item = Convert.ToInt16(account_charges_credits_found[account_charges_credits_found.Count - 1].ACCOUNT_ITEM + 1);
                            utilityviewmodel.ACCOUNT_CHARGES_CREDITS_ITEM = next_item;
                            break;  // Time to go to the return false;
                        }
                    }
                }
            }
            return false;
        }

        internal static bool Check_Discount_Is_There(UtilityViewModel utilityviewmodel,
                                                    DateTime DISCOUNTDATE,
                                                    short DISCOUNTITEM,
                                                    string DISCOUNTTYPE,
                                                    short DISCOUNTVATCODE,
                                                    string DISCOUNTAMOUNT)
        {
            // H-ello!  How can I help you (such a sweetie voice!  Little do they know
            // she is SUCH A FUCKING BITCH

            // Is it already there?
            short supplier_code = utilityviewmodel.supplier_code;
            short brand_code = utilityviewmodel.brand_code;
            string account_no = utilityviewmodel.ACCOUNT_NO;
            DateTime created = utilityviewmodel.CREATED;
            string statement_id = utilityviewmodel.STATEMENT_ID;
            DateTime bill_date = SmartTimeV2016.ConvertDateTime(utilityviewmodel.BILL_DATE);
            switch (utilityviewmodel.resource_code)
            {
                case SmartParametersV2016.Electricity:
                    List<SmartUtility.EDiscounts> e_discounts_found = SmartSpikeUtilityV2017.Utility_EDiscountsFound(utilityviewmodel,
                                                                                            supplier_code,
                                                                                            brand_code,
                                                                                            account_no,
                                                                                            created,
                                                                                            statement_id,
                                                                                            bill_date,
                                                                                            DISCOUNTDATE,
                                                                                            DISCOUNTITEM);
                    // Have we already got this payment recorded?
                    if (e_discounts_found.Count > 0)
                    {
                        // Found some - check 'em out
                        foreach (SmartUtility.EDiscounts e_discounts_row in e_discounts_found)
                        {
                            if ((Convert.ToInt16(e_discounts_row.DISCOUNT_ITEM) == DISCOUNTITEM) &&
                                (e_discounts_row.DISCOUNT_TYPE == DISCOUNTTYPE) &&
                                (Convert.ToInt16(e_discounts_row.DISCOUNT_VAT_CODE) == DISCOUNTVATCODE) &&
                                (e_discounts_row.DISCOUNT_AMOUNT.ToString() == DISCOUNTAMOUNT))
                            {
                                // Its there - on the right date wiv the right amount
                                return true;
                            }
                        }
                    }
                    break;
                case SmartParametersV2016.Gas:
                    List<SmartUtility.GDiscounts> g_discounts_found = SmartSpikeUtilityV2017.Utility_GDiscountsFound(utilityviewmodel,
                                                                                            supplier_code,
                                                                                            brand_code,
                                                                                            account_no,
                                                                                            created,
                                                                                            statement_id,
                                                                                            bill_date,
                                                                                            DISCOUNTDATE,
                                                                                            DISCOUNTITEM);
                    // Have we already got this payment recorded?
                    if (g_discounts_found.Count > 0)
                    {
                        // Found some - check 'em out
                        foreach (SmartUtility.GDiscounts g_discounts_row in g_discounts_found)
                        {
                            if ((Convert.ToInt16(g_discounts_row.DISCOUNT_ITEM) == DISCOUNTITEM) &&
                                (g_discounts_row.DISCOUNT_TYPE == DISCOUNTTYPE) &&
                                (Convert.ToInt16(g_discounts_row.DISCOUNT_VAT_CODE) == DISCOUNTVATCODE) &&
                                (g_discounts_row.DISCOUNT_AMOUNT.ToString() == DISCOUNTAMOUNT))
                            {
                                // Its there - on the right date wiv the right amount
                                return true;
                            }
                        }
                    }
                    break;
                default:
                    break;
            }
            return false;
        }

        internal static bool Check_Supply_Is_There(MainViewModel ourviewmodel,
                                                    UtilityViewModel utilityviewmodel,
                                                    DateTime SUPPLYDATE,
                                                    short supplyItem,
                                                    string SUPPLYTYPE,
                                                    short SUPPLYVATCODE,
                                                    int SUPPLYAMOUNT)
        {
            // Is it already there in Ins?
            List<SmartUtility.SupChargesCredits> supply_charges_credits_found =
                                            new List<SmartUtility.SupChargesCredits>(from Supply_Charges_Credit
                                            in utilityviewmodel.Hezbollah.supply_charges_creditsList
                                                                                     where ((Supply_Charges_Credit.USERNAME == ourviewmodel.UserName) &&
                                                                                             (Supply_Charges_Credit.CUBEFACE_CODE == SmartParametersV2016.Utility) &&
                                                                                             (Supply_Charges_Credit.SUPPLIER_CODE == utilityviewmodel.supplier_code) &&
                                                                                             (Supply_Charges_Credit.BRAND_CODE == utilityviewmodel.brand_code) &&
                                                                                             (Supply_Charges_Credit.ACCOUNT_NO == utilityviewmodel.ACCOUNT_NO) &&
                                                                                             (Supply_Charges_Credit.ACCOUNT_CREATED == utilityviewmodel.CREATED) &&
                                                                                             (Supply_Charges_Credit.STATEMENT_ID == utilityviewmodel.STATEMENT_ID) &&
                                                                                             (Supply_Charges_Credit.BILL_DATE == SmartTimeV2016.ConvertDateTime(utilityviewmodel.BILL_DATE)) &&
                                                                                             (Supply_Charges_Credit.SUPPLY_DATE == SUPPLYDATE))
                                                                                     select Supply_Charges_Credit);
            // Have we already got this payment recorded?
            if (supply_charges_credits_found.Count > 0)
            {
                // Found some - check 'em out
                foreach (SmartUtility.SupChargesCredits supply_charges_credits_row in supply_charges_credits_found)
                {
                    if ((supply_charges_credits_row.SUPPLY_ITEM == supplyItem) &&
                        (supply_charges_credits_row.SUPPLY_TYPE == SUPPLYTYPE) &&
                        (supply_charges_credits_row.SUPPLY_VAT_CODE == SUPPLYVATCODE) &&
                        (supply_charges_credits_row.SUPPLY_AMOUNT == SUPPLYAMOUNT))
                    {
                        // Its there - on the right date wiv the right amount
                        return true;
                    }
                }
            }
            // Is it already there in Outs?
            supply_charges_credits_found = new List<SmartUtility.SupChargesCredits>(from Supply_Charges_Credit
                                            in utilityviewmodel.Hezbollah.supply_charges_credits_changesList
                                                                                    where ((Supply_Charges_Credit.CUBEFACE_CODE == SmartParametersV2016.Utility) &&
                                                                                            (Supply_Charges_Credit.SUPPLIER_CODE == utilityviewmodel.supplier_code) &&
                                                                                            (Supply_Charges_Credit.BRAND_CODE == utilityviewmodel.brand_code) &&
                                                                                            (Supply_Charges_Credit.ACCOUNT_NO == utilityviewmodel.ACCOUNT_NO) &&
                                                                                            (Supply_Charges_Credit.ACCOUNT_CREATED == utilityviewmodel.CREATED) &&
                                                                                            (Supply_Charges_Credit.STATEMENT_ID == utilityviewmodel.STATEMENT_ID) &&
                                                                                            (Supply_Charges_Credit.BILL_DATE == SmartTimeV2016.ConvertDateTime(utilityviewmodel.BILL_DATE)) &&
                                                                                            (Supply_Charges_Credit.SUPPLY_DATE == SUPPLYDATE))
                                                                                    select Supply_Charges_Credit);
            // Have we already got this payment recorded?
            if (supply_charges_credits_found.Count > 0)
            {
                // Found some - check 'em out
                foreach (SmartUtility.SupChargesCredits supply_charges_credits_row in supply_charges_credits_found)
                {
                    if ((supply_charges_credits_row.SUPPLY_ITEM == supplyItem) &&
                        (supply_charges_credits_row.SUPPLY_TYPE == SUPPLYTYPE) &&
                        (supply_charges_credits_row.SUPPLY_VAT_CODE == SUPPLYVATCODE) &&
                        (supply_charges_credits_row.SUPPLY_AMOUNT == SUPPLYAMOUNT))
                    {
                        // Its there - on the right date wiv the right amount
                        return true;
                    }
                }
            }
            return false;
        }

        internal static bool Determine_Vat_Code(MainViewModel ourviewmodel,
                                                UtilityViewModel utilityviewmodel,
                                                string token,
                                                string vat)
        {
            utilityviewmodel.vat_code = SmartParametersV2016.zeroRateVatCode;    // Default to 1
            string VAT_RATE = token.Replace(vat, "");
            int percent = VAT_RATE.IndexOf("%");
            if (percent >= 0)
            {
                VAT_RATE = VAT_RATE.Substring(0, percent).Trim();
                int has_decimal = VAT_RATE.IndexOf(SmartParametersV2016.decimalPoint);
                if (has_decimal < 0)
                {
                    VAT_RATE += ".00";
                }
                if (SmartParseV2016.Lookup_Vat_Code(VAT_RATE,
                                                    SmartTimeV2016.ConvertDateTime(utilityviewmodel.BILL_PERIOD_END),
                                                    ourviewmodel.Blanche.vatRatesList,
                                                    utilityviewmodel))
                //rf VAT_CODE, 
                //rf ourviewmodel.errorMessage))
                {
                    // Result is in utilityviewmodel.vat_code!!
                    //vatCode = VAT_CODE;
                    return true;
                }
            }
            return false;
        }

        internal static void No_Wonder_I_Cant_Fucking_Work(UtilityViewModel utilityviewmodel,
                                                            DateTime DISCOUNTDATE,
                                                            short DISCOUNTITEM,
                                                            string DISCOUNTTYPE,
                                                            short DISCOUNTVATCODE,
                                                            string DISCOUNTAMOUNT)
        {
            // We don't use the Discount Credit Bill here because these are Discounts
            // THAT HAVE ALREADY BEEN APPLIED
            switch (utilityviewmodel.resource_code)
            {
                case SmartParametersV2016.Electricity:
                    // Page 1 - Direct Debit discount
                    SmartUtilityV2022.E_DiscountsList_Add(utilityviewmodel,
                                                            SmartParametersV2016.Utility,
                                                            utilityviewmodel.supplier_code,
                                                            utilityviewmodel.brand_code,
                                                            utilityviewmodel.ACCOUNT_NO,
                                                            utilityviewmodel.CREATED,
                                                            utilityviewmodel.STATEMENT_ID,
                                                            utilityviewmodel.BILL_DATE,
                                                            utilityviewmodel.sparks.MPAN,
                                                            DISCOUNTDATE.ToString(SmartParametersV2016.defaultCulture),
                                                            DISCOUNTITEM.ToString(),
                                                            DISCOUNTTYPE,
                                                            DISCOUNTVATCODE.ToString(),
                                                            DISCOUNTAMOUNT);
                    break;
                case SmartParametersV2016.Gas:
                    SmartUtilityV2022.G_DiscountsList_Add(utilityviewmodel,
                                                            SmartParametersV2016.Utility,
                                                            utilityviewmodel.supplier_code,
                                                            utilityviewmodel.brand_code,
                                                            utilityviewmodel.ACCOUNT_NO,
                                                            utilityviewmodel.CREATED,
                                                            utilityviewmodel.STATEMENT_ID,
                                                            utilityviewmodel.BILL_DATE,
                                                            utilityviewmodel.smell.MPRN,
                                                            DISCOUNTDATE.ToString(SmartParametersV2016.defaultCulture),
                                                            DISCOUNTITEM.ToString(),
                                                            DISCOUNTTYPE,
                                                            DISCOUNTVATCODE.ToString(),
                                                            DISCOUNTAMOUNT);
                    break;
                default:
                    break;
            }
            // Note: DISCOUNT_AMOUNT has already been checked as an Integer
        }

        internal static bool Look_Backwards(UtilityViewModel utilityviewmodel, string search, string substitute)
        {
            int last_char = utilityviewmodel.token.LastIndexOf(search);
            if (last_char >= 0)
            {
                if (!string.IsNullOrEmpty(substitute))
                {
                    StringBuilder sb = new StringBuilder(utilityviewmodel.token);
                    sb[last_char] = Convert.ToChar(substitute);
                    utilityviewmodel.token = sb.ToString();
                    return true;
                }
            }
            return false;
        }


        internal static void Find_Bill_Periods(UtilityViewModel utilityviewmodel,
                                            int line_count,
                                            string[] lines,
                                            //rf string BILL_PERIOD_START,
                                            //rf string BILL_PERIOD_END,
                                            string defaultDates)
        {
            while (line_count + 1 < lines.Length)
            {
                line_count++;
                if (!string.IsNullOrEmpty(lines[line_count]))
                {
                    int dash_index = lines[line_count].IndexOf("-");
                    if (dash_index >= 0)
                    {
                        string[] components = lines[line_count].Split('-');
                        if (components.Length > 0)
                        {
                            utilityviewmodel.BILL_PERIOD_START = components[0].Trim();
                            if (components.Length > 1)
                            {
                                utilityviewmodel.BILL_PERIOD_END = components[1].Trim();
                                return;
                            }
                        }
                    }
                }
            }
            utilityviewmodel.BILL_PERIOD_START = utilityviewmodel.BILL_PERIOD_END = defaultDates;
        }

        //// Similar to the one above - can we combine them one day?

        internal static string Refactor_Unit_Rate(string unitRate)
        {
            string UNITS_RATE = "";

            // At this point, we know its a genuine decimal as a string
            int dec_point = unitRate.IndexOf(SmartParametersV2016.decimalPoint);
            if (dec_point >= 0)
            {
                // We found a dec point, now have we got 2 characters?
                if (unitRate.Length >= dec_point + 2)
                {
                    // Do the move
                    UNITS_RATE = unitRate.Replace(SmartParametersV2016.decimalPoint, ""); // Its gone
                    UNITS_RATE = UNITS_RATE.Substring(0, dec_point + 2) + SmartParametersV2016.decimalPoint + UNITS_RATE.Substring(0, dec_point + 2);
                    UNITS_RATE = UNITS_RATE.TrimStart('0');
                }
            }
            else
            {
                UNITS_RATE = unitRate;
            }
            return UNITS_RATE;
        }
    }
}