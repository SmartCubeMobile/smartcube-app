using System;
using System.Collections.Generic;

namespace SmartCubeMobile
{
    public static class SmartEngineV2016
    {
        internal static bool Build_Tariff_EngineList(UtilityViewModel utilityviewmodel,
                                                    char resource_code,
                                                    DateTime comparison_date,
                                                    string VOLUMECORRECTION,
                                                    string KWHCONVERSION)
        {
            DateTime sc_months,
                        sc_quarters,
                        sc_years,
                        limit_months,
                        limit_quarters,
                        limit_years;

            utilityviewmodel.volumeCorrection = Convert.ToDecimal(VOLUMECORRECTION);
            utilityviewmodel.kwhConversion = Convert.ToDecimal(KWHCONVERSION);

            decimal units_total,
                        day_units = 0.0M,
                        night_units = 0.0M;

            int units_days,
                        readings_days;

            // Build our readings temp table
            List<Readings_Temp> readings_tempList = new List<Readings_Temp>();

            utilityviewmodel.e_readings_found = new List<SmartUtility.EReadings>();
            utilityviewmodel.g_readings_found = new List<SmartUtility.GReadings>();
            utilityviewmodel.d_readings_found = new List<SmartUtility.EReadings>();

            Find_All_Readings(utilityviewmodel,
                            resource_code);

            Build_Readings_Temp(utilityviewmodel,
                                resource_code,
                                utilityviewmodel.volumeCorrection,
                                utilityviewmodel.kwhConversion,
                                readings_tempList);

            if (readings_tempList.Count > 0)
            {
                units_total = 0.0M;

                List<SmartUtility.TariffEngine> tariff_tempList = new List<SmartUtility.TariffEngine>();
                sc_months = sc_quarters = sc_years = SmartParametersV2016.defaultDate;
                limit_months = limit_quarters = limit_years = SmartParametersV2016.defaultDate;
                foreach (Readings_Temp readings_row in readings_tempList)
                {
                    units_days = readings_row.UNITS_DAYS;
                    TimeSpan ts = new TimeSpan(units_days, 0, 0, 0, 0);
                    DateTime reading_date = readings_row.READ_DATE;
                    DateTime start_date = reading_date.Subtract(ts);
                    if ((sc_months == SmartParametersV2016.defaultDate) &&
                        (sc_quarters == SmartParametersV2016.defaultDate) &&
                        (sc_years == SmartParametersV2016.defaultDate))
                    {
                        sc_months = start_date;  // All of this fucking about for
                        sc_quarters = start_date;
                        sc_years = start_date;   // Monthly SCs and Yearly SCs
                    }
                    if ((limit_months == SmartParametersV2016.defaultDate) &&
                        (limit_quarters == SmartParametersV2016.defaultDate) &&
                        (limit_years == SmartParametersV2016.defaultDate))
                    {
                        limit_months = start_date.AddMonths(1);
                        limit_quarters = start_date.AddMonths(3);
                        limit_years = start_date.AddYears(1);
                    }

                    if (units_days > 0)
                    {
                        switch (resource_code)
                        {
                            case SmartParametersV2016.Electricity:
                                day_units = readings_row.D_UNITS_USED / units_days;
                                night_units = readings_row.N_UNITS_USED / units_days;
                                break;
                            case SmartParametersV2016.Gas:
                                day_units = readings_row.UNITS_USED / units_days;
                                night_units = 0.0M;
                                break;
                            default:
                                break;
                        }
                    }
                    else
                    {
                        day_units = night_units = 0.0M;
                    }

                    readings_days = 0;
                    while (readings_days < units_days)
                    {
                        SmartUtility.TariffEngine tariff_temp_row = new SmartUtility.TariffEngine()
                        {
                            USERNAME = readings_row.USERNAME,
                            CUBEFACE_CODE = readings_row.CUBEFACE_CODE,
                            SUPPLIER_CODE = readings_row.SUPPLIER_CODE,
                            BRAND_CODE = readings_row.BRAND_CODE, // brand_code; YES! I know! readings_row should be BRAND as well // This is a key
                            ACCOUNT_NO = readings_row.ACCOUNT_NO, // account_no;        // This is a key
                            ACCOUNT_CREATED = readings_row.ACCOUNT_CREATED, // brand_code; YES! I know! readings_row should be BRAND as well // This is a key

                            PERIOD_END = comparison_date,    // This is a key

                            D_UNITS = day_units,
                            N_UNITS = night_units,
                            UNITS = day_units + night_units
                        };
                        units_total = units_total + day_units + night_units;
                        tariff_temp_row.UNITS_TOTAL = units_total;
                        // Even though THIS one might not have any Standing Charges
                        // this DAYS = 1 is needed as a 'hook' for any other comparisons
                        // that might e.g.  0 < DAYS (where DAYS = 1)
                        tariff_temp_row.DAYS = true;            // Stops it getting sent as "True"

                        // Do the Standing Charge Months, Quarters and Years
                        if (SmartRoutinesV2018.DateTimeCompare(start_date, sc_months) >= 0)
                        {
                            tariff_temp_row.SC_MONTHS = true;
                            sc_months = sc_months.AddMonths(1);
                        }
                        else
                        {
                            tariff_temp_row.SC_MONTHS = false;
                        }
                        if (SmartRoutinesV2018.DateTimeCompare(start_date, sc_quarters) >= 0)
                        {
                            tariff_temp_row.SC_QUARTERS = true;
                            sc_quarters = sc_quarters.AddMonths(3);
                        }
                        else
                        {
                            tariff_temp_row.SC_QUARTERS = false;
                        }
                        if (SmartRoutinesV2018.DateTimeCompare(start_date, sc_years) >= 0)
                        {
                            tariff_temp_row.SC_YEARS = true;
                            sc_years = sc_years.AddYears(1);
                        }
                        else
                        {
                            tariff_temp_row.SC_YEARS = false;
                        }

                        // Do the Limits Months, Quarters and Years
                        if (SmartRoutinesV2018.DateTimeCompare(start_date, limit_months) >= 0)
                        {
                            tariff_temp_row.LIMIT_MONTHS = true;
                            limit_months = limit_months.AddMonths(1);
                        }
                        else
                        {
                            tariff_temp_row.LIMIT_MONTHS = false;
                        }

                        if (SmartRoutinesV2018.DateTimeCompare(start_date, limit_quarters) >= 0)
                        {
                            tariff_temp_row.LIMIT_QUARTERS = true;
                            limit_quarters = limit_quarters.AddMonths(3);
                        }
                        else
                        {
                            tariff_temp_row.LIMIT_QUARTERS = false;
                        }

                        if (SmartRoutinesV2018.DateTimeCompare(start_date, limit_years) >= 0)
                        {
                            tariff_temp_row.LIMIT_YEARS = true;
                            limit_years = limit_years.AddYears(1);
                        }
                        else
                        {
                            tariff_temp_row.LIMIT_YEARS = false;
                        }

                        tariff_temp_row.UNITS_COST = 0.0M;
                        tariff_temp_row.CHARGES_COST = 0.0M;
                        tariff_temp_row.TOTAL_MONTHLY_COST = 0.0M;
                        tariff_temp_row.TOTAL_QUARTERLY_COST = 0.0M;
                        tariff_temp_row.TOTAL_YEARLY_COST = 0.0M;

                        tariff_tempList.Add(tariff_temp_row);
                        start_date = start_date.AddDays(1);
                        comparison_date = comparison_date.AddDays(1);
                        readings_days++;
                    }
                    // Even I won't accept a different VAT rate for Day and Night ..

                    // Merge 'em all together - this DOES NOT do it in ascending
                    // Primary Key order !!!!
                    switch (resource_code)
                    {
                        case SmartParametersV2016.Electricity:
                            utilityviewmodel.Hezbollah.e_tariff_engineList.AddRange(tariff_tempList);
                            break;
                        case SmartParametersV2016.Gas:
                            utilityviewmodel.Hezbollah.g_tariff_engineList.AddRange(tariff_tempList);
                            break;
                        default:
                            break;
                    }
                    tariff_tempList.Clear();
                }
            }
            return true;
        }

        internal static void Build_Readings_Temp(UtilityViewModel utilityviewmodel,
                                            char resource_code,
                                            decimal volumeCorrection,
                                            decimal kwhConversion,
                                            List<Readings_Temp> readings_tempList)
        {
            switch (resource_code)
            {
                case SmartParametersV2016.Electricity:
                    if (utilityviewmodel.e_readings_found.Count > 0)
                    {
                        // Populate our temp table
                        decimal d_residue = 0.0M,
                                n_residue = 0.0M;
                        bool skip_one = false;
                        DateTime LAST_FROM_DATE = SmartParametersV2016.defaultDate;
                        foreach (SmartUtility.EReadings readings_row in utilityviewmodel.e_readings_found)
                        {
                            Readings_Temp readings_temp_row = new Readings_Temp()
                            {
                                USERNAME = readings_row.USERNAME,
                                CUBEFACE_CODE = readings_row.CUBEFACE_CODE,
                                SUPPLIER_CODE = readings_row.SUPPLIER_CODE,
                                BRAND_CODE = readings_row.BRAND_CODE,
                                ACCOUNT_NO = readings_row.ACCOUNT_NO,
                                ACCOUNT_CREATED = readings_row.ACCOUNT_CREATED,

                                READ_DATE = readings_row.READINGS_PERIOD_END,
                                // D_UNITS_USED Electricity has a different storage ...
                                D_UNITS_USED = readings_row.D_UNITS_USED,
                                N_UNITS_USED = readings_row.N_UNITS_USED
                            };
                            if (skip_one &&
                                (readings_row.READINGS_PERIOD_START == LAST_FROM_DATE))
                            {
                                readings_temp_row.D_UNITS_USED += d_residue;
                                readings_temp_row.N_UNITS_USED += n_residue;
                                skip_one = false;
                            }
                            readings_temp_row.UNITS_DAYS = (short)readings_row.READINGS_PERIOD_END.Subtract(readings_row.READINGS_PERIOD_START).Days;

                            if (readings_row.READINGS_PERIOD_START == readings_row.READINGS_PERIOD_END)
                            {
                                d_residue = readings_row.D_UNITS_USED;
                                n_residue = readings_row.N_UNITS_USED;
                                LAST_FROM_DATE = readings_row.READINGS_PERIOD_START;
                                skip_one = true;
                            }
                            else
                            {
                                readings_tempList.Add(readings_temp_row);
                                // Update the fuckers
                                utilityviewmodel.totalReadingsTemp = utilityviewmodel.totalReadingsTemp + readings_temp_row.D_UNITS_USED + readings_temp_row.N_UNITS_USED;
                            }
                        }
                        // Should really check if skip_one still set to true here ...
                        int temp_count = readings_tempList.Count - 1;
                        if (temp_count > 0)
                        {
                            utilityviewmodel.firstReading = readings_tempList[0].D_UNITS_USED + readings_tempList[0].N_UNITS_USED;
                            utilityviewmodel.lastReading = readings_tempList[temp_count].D_UNITS_USED + readings_tempList[temp_count].N_UNITS_USED;
                        }
                        else
                        {
                            utilityviewmodel.firstReading = 0.0M;
                            utilityviewmodel.lastReading = 0.0M;
                        }
                    }
                    break;
                case SmartParametersV2016.Gas:
                    if (utilityviewmodel.g_readings_found.Count > 0)
                    {
                        // Populate our temp table
                        decimal d_residue = 0.0M;
                        bool skip_one = false;
                        DateTime LAST_FROM_DATE = SmartParametersV2016.defaultDate;

                        foreach (SmartUtility.GReadings readings_row in utilityviewmodel.g_readings_found)
                        {
                            Readings_Temp readings_temp_row = new Readings_Temp()
                            {
                                USERNAME = readings_row.USERNAME,
                                CUBEFACE_CODE = readings_row.CUBEFACE_CODE,
                                SUPPLIER_CODE = readings_row.SUPPLIER_CODE,
                                BRAND_CODE = readings_row.BRAND_CODE,
                                ACCOUNT_NO = readings_row.ACCOUNT_NO,
                                ACCOUNT_CREATED = readings_row.ACCOUNT_CREATED,

                                READ_DATE = readings_row.READINGS_PERIOD_END,
                                UNITS_DAYS = (short)readings_row.READINGS_PERIOD_END.Subtract(readings_row.READINGS_PERIOD_START).Days,
                                UNITS_USED = readings_row.D_UNITS_USED_KWH
                            };
                            // Only if we can't find a kWh and we have an m3
                            if (readings_temp_row.UNITS_USED == 0)
                            {
                                if (readings_row.D_UNITS_USED_M3 != -0)
                                {
                                    readings_temp_row.UNITS_USED = Convert_To_Kwh(readings_row.UNIT_OF_MEASURE,
                                                                readings_row.D_UNITS_USED_M3,
                                                                volumeCorrection,
                                                                readings_row.CALORIFIC_VALUE,
                                                                kwhConversion);
                                }
                            }

                            if (skip_one &&
                                (readings_row.READINGS_PERIOD_START == LAST_FROM_DATE))
                            {
                                readings_temp_row.UNITS_USED += d_residue;
                                skip_one = false;
                            }
                            if (readings_row.READINGS_PERIOD_START == readings_row.READINGS_PERIOD_END)
                            {
                                d_residue = readings_temp_row.UNITS_USED;
                                LAST_FROM_DATE = readings_row.READINGS_PERIOD_START;
                                skip_one = true;
                            }
                            else
                            {
                                readings_tempList.Add(readings_temp_row);
                                // Update these fuckers
                                utilityviewmodel.totalReadingsTemp += readings_temp_row.UNITS_USED;
                            }
                        }
                        int temp_count = readings_tempList.Count - 1;
                        if (temp_count > 0)
                        {
                            utilityviewmodel.firstReading = readings_tempList[0].UNITS_USED;
                            utilityviewmodel.lastReading = readings_tempList[temp_count].UNITS_USED;
                        }
                        else
                        {
                            utilityviewmodel.firstReading = 0.0M;
                            utilityviewmodel.lastReading = 0.0M;
                        }
                    }
                    break;
                default:
                    break;
            }
            return;
        }

        internal static void Find_All_Readings(UtilityViewModel utilityviewmodel,
                                            char resource_code)
        {
            switch (resource_code)
            {
                case SmartParametersV2016.Electricity:
                    utilityviewmodel.Hezbollah.e_tariff_engineList.Clear();

                    // select A.SUPPLIER_CODE, A.ACCOUNT_NO,
                    // E_R.FROM_DATE,E_R.READ_DATE,E_R.D_UNITS_USED,E_R.N_UNITS_USED,E_R.UNIT_OF_MEASURE,
                    // B.PAYMENT_PLAN
                    // from SmartUsers.consumer_energy AS C
                    // join SmartUsers.Utility.Accounts AS A
                    // -- The Join on USERNAME is cos when we use SQL Management we may have others with this Supplier
                    // on (A.USERNAME = C.USERNAME AND A.CATEGORY_CODE = C.CATEGORY_CODE AND A.RESOURCE_CODE = C.RESOURCE_CODE AND A.SUPPLIER_CODE = C.SUPPLIER_CODE)
                    // join SmartUsers.Bills AS B
                    // on (B.CATEGORY_CODE = A.CATEGORY_CODE AND B.SUPPLIER_CODE = A.SUPPLIER_CODE AND B.ACCOUNT_NO = A.ACCOUNT_NO)
                    // join SmartUsers.Bills_Resource AS BR
                    // on (BR.CATEGORY_CODE = B.CATEGORY_CODE AND BR.SUPPLIER_CODE = B.SUPPLIER_CODE AND BR.ACCOUNT_NO = B.ACCOUNT_NO AND BR.STATEMENT_ID = B.STATEMENT_ID AND BR.BILL_DATE = B.BILL_DATE AND BR.RESOURCE_CODE = A.RESOURCE_CODE)
                    // join SmartUsers.E_Readings AS E_R
                    // on (E_R.SUPPLIER_CODE = BR.SUPPLIER_CODE AND E_R.ACCOUNT_NO = BR.ACCOUNT_NO AND E_R.STATEMENT_ID = BR.STATEMENT_ID)
                    // where C.USERNAME = 'MARIA' AND
                    // C.CATEGORY_CODE = 'U' AND
                    //C.RESOURCE_CODE = 'E' AND
                    // C.MPAN_MPRN = 'S1610028353603' AND
                    // E_R.UNIT_OF_MEASURE = 'kWh'
                    // order by E_R.FROM_DATE, E_R.READ_DATE

                    // Note: This is ACROSS all Suppliers....
                    SmartSpikeUtilityV2017.Utility_Find_Readings(utilityviewmodel,
                                                                resource_code);
                    break;
                case SmartParametersV2016.Gas:
                    utilityviewmodel.Hezbollah.g_tariff_engineList.Clear();

                    //select A.SUPPLIER_CODE, A.ACCOUNT_NO,
                    // G_R.FROM_DATE,G_R.READ_DATE,G_R.D_UNITS_USED,G_R.UNIT_OF_MEASURE,
                    // B.PAYMENT_PLAN,
                    // G_R.VOLUMECORRECTION,G_R.CALORIFIC_VALUE,G_R.KWHCONVERSION
                    // from SmartUsers.consumer_energy AS C
                    // join SmartUsers.Utility.Accounts AS A
                    // -- The Join on USERNAME is cos when we use SQL Management we may have others with this Supplier
                    // on (A.USERNAME = C.USERNAME AND A.CATEGORY_CODE = C.CATEGORY_CODE AND A.RESOURCE_CODE = C.RESOURCE_CODE AND A.SUPPLIER_CODE = C.SUPPLIER_CODE)
                    // join SmartUsers.Bills AS B
                    // on (B.CATEGORY_CODE = A.CATEGORY_CODE AND B.SUPPLIER_CODE = A.SUPPLIER_CODE AND B.ACCOUNT_NO = A.ACCOUNT_NO)
                    // join SmartUsers.Bills_Resource AS BR
                    // on (BR.CATEGORY_CODE = B.CATEGORY_CODE AND BR.SUPPLIER_CODE = B.SUPPLIER_CODE AND BR.ACCOUNT_NO = B.ACCOUNT_NO AND BR.STATEMENT_ID = B.STATEMENT_ID AND BR.BILL_DATE = B.BILL_DATE AND BR.RESOURCE_CODE = A.RESOURCE_CODE)
                    // join SmartUsers.G_Readings AS G_R
                    // on (G_R.SUPPLIER_CODE = BR.SUPPLIER_CODE AND G_R.ACCOUNT_NO = BR.ACCOUNT_NO AND G_R.STATEMENT_ID = BR.STATEMENT_ID)
                    // where C.USERNAME = 'MARIA' AND
                    // C.CATEGORY_CODE = 'U' AND
                    // C.RESOURCE_CODE = 'G' AND
                    // C.MPAN_MPRN = '1577463803'
                    // order by G_R.FROM_DATE, G_R.READ_DATE

                    // Note: This is ACROSS all Suppliers ...
                    SmartSpikeUtilityV2017.Utility_Find_Readings(utilityviewmodel,
                                                        resource_code);
                    break;
                default:
                    SmartSpikeUtilityV2017.Utility_Find_Readings(utilityviewmodel,
                                                        resource_code);
                    break;
            }
            return;
        }

        internal static decimal Convert_To_Kwh(string unit_of_measure,
                                                decimal d_units_used,
                                                decimal volumeCorrection,
                                                decimal calorific_value,
                                                decimal kwhConversion)
        {
            if (kwhConversion == 0.0M)
            {
                // Safety net to stop divide by zero on the conversion
                kwhConversion = 3.6M;
            }
            switch (unit_of_measure)
            {
                case "ft3":
                    // Convert to m3
                    d_units_used *= 2.83M;
                    // Convert from cubic metres to kWh
                    d_units_used = (d_units_used * calorific_value * volumeCorrection) / kwhConversion;
                    break;
                case "m3":
                    // Convert from cubic metres to kWh
                    d_units_used = (d_units_used * calorific_value * volumeCorrection) / kwhConversion;
                    break;
                default:
                    // kWh - do nothing
                    break;
            }
            return d_units_used;
        }
    }

    internal class Readings_Temp
    {
        internal string
            USERNAME;
        internal char
            CUBEFACE_CODE;
        internal short
            SUPPLIER_CODE,
            BRAND_CODE;
        internal string
            ACCOUNT_NO;
        internal DateTime
            ACCOUNT_CREATED;
        internal DateTime
            READ_DATE;
        internal decimal
            D_UNITS_USED,
            N_UNITS_USED;
        internal decimal
            UNITS_USED;
        internal short
            UNITS_DAYS;
    }
}