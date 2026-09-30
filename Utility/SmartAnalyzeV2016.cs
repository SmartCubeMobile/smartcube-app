//
//  In memory of Susan Mason (nee Ryan) 1949 - 2014
//  who used to sit next to me in 4A at Derwentwater and was a kind girl with gorgeous
//  red hair and to whom I used to show my arithmetic answers when she got stuck.
//  RIP Susan ... you were a good girl
//

// Because of a bug .. SmartAnalyze DOES NOT do Placeholder tariffs hence the select
// criteria on tariffs (<== NO! Tariffs_Matrix!!!) always include STATUS_FLAG == ""

// **
// BUSINESS RULE:   TARIFFS are checked that STATUS_FLAG != Deleted
// BUSINESS RULE:   TARIFF_MATRIXs are checked that STATUS_FLAG != Deleted
//                  and PLACEHOLDER = 'N'
// **

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

namespace SmartCubeMobile
{
    internal class SmartAnalyzeV2016
    {
        internal static List<SmartUtility.AnalysisCostsView> Fill_Analysis_Costs(MainViewModel ourviewmodel,
                                                            UtilityViewModel utilityviewmodel,
                                                            char resource_code,
                                                            string displayName)
        {
            // We have ALWAYS done an analysis, we just may
            // not DISPLAY it if the expiration has been exceeded
            DateTime energy_expiration = SmartParametersV2016.defaultDate;

            switch (resource_code)
            {
                case SmartParametersV2016.Electricity:
                    energy_expiration = ourviewmodel.expiration[0];
                    break;
                case SmartParametersV2016.Gas:
                    energy_expiration = ourviewmodel.expiration[1];
                    break;
                case SmartParametersV2016.DualFuel:
                    // Only if its Dual Fuel do we get the minimum of the expirations
                    // If they are both null, then nothing changes
                    if (SmartRoutinesV2018.DateTimeCompare(ourviewmodel.expiration[0], ourviewmodel.expiration[1]) <= 0)
                    {
                        energy_expiration = ourviewmodel.expiration[0];
                    }
                    else
                    {
                        energy_expiration = ourviewmodel.expiration[1];
                    }
                    break;
                default:
                    break;
            }

            
            List<SmartUtility.AnalysisCostsView> analysis_costs_tempList = new List<SmartUtility.AnalysisCostsView>();

            string BRAND_NAME = "", //SUPPLIER_NAME = "",
                    TARIFF_TYPE = "";
            short last_supplier_code = 0,
                    last_brand_code = 0;
            char last_resource_code = SmartParametersV2016.defaultResourceCode;
            string last_resource_type = "";

            // These two put in to speed the lookups in SmartSpike
            utilityviewmodel.analysisbrandsviewList.Clear();// = new List<SmartUtility.AnalysisBrandsView>();
            utilityviewmodel.analysistariffsnameviewList.Clear(); // = new List<SmartUtility.AnalysisTariffsNameView>();

            // If there is no Expiration OR its within the limit then generate the costs for display
            if (//Convert.ToBoolean(ourviewmodel.LoginExpirationDate) || 
                (energy_expiration == SmartParametersV2016.defaultDate || SmartRoutinesV2018.DateTimeCompare((DateTime.Now + ourviewmodel.utcOffset), energy_expiration) <= 0)) // Local time and Local expiration
            {
                List<SmartUtility.AnalysisCosts> analysis_costsList = new List<SmartUtility.AnalysisCosts>();
                switch (resource_code)
                {
                    case SmartParametersV2016.Electricity:
                        utilityviewmodel.Hezbollah.e_analysis_costsList = SmartSpikeUtilityV2017.Utility_Find_AnalysisCosts_Resource(utilityviewmodel.Hezbollah.e_analysis_costsList);
                        analysis_costsList = utilityviewmodel.Hezbollah.e_analysis_costsList;
                        break;
                    case SmartParametersV2016.Gas:
                        utilityviewmodel.Hezbollah.g_analysis_costsList = SmartSpikeUtilityV2017.Utility_Find_AnalysisCosts_Resource(utilityviewmodel.Hezbollah.g_analysis_costsList);
                        analysis_costsList = utilityviewmodel.Hezbollah.g_analysis_costsList;
                        break;
                    case SmartParametersV2016.DualFuel:
                        // For this ... E is the Same as G as both are D
                        utilityviewmodel.Hezbollah.d_analysis_costsList = SmartSpikeUtilityV2017.Utility_Find_AnalysisCosts_Resource(utilityviewmodel.Hezbollah.d_analysis_costsList);
                        analysis_costsList = utilityviewmodel.Hezbollah.d_analysis_costsList;
                        break;
                    default:
                        break;
                }

                string RESOURCE_TYPE = "";
                //
                // Could probably speed this up by building 'quickie' lookup tables
                //

                foreach (SmartUtility.AnalysisCosts analysis_costs_row in analysis_costsList)
                {
                    //if (resource_code == SmartParametersV2016.defaultResourceCode)
                    //{
                    //    analysis_costs_row.RESOURCE_TYPE = "G:SR|E:VR";
                    //}
                    string payment_names = "";
                    string payment_plan = analysis_costs_row.PAYMENT_PLAN;
                    for (int i = 0; i < payment_plan.Length; i++)
                    {
                        List<SmartUtility.PaymentPlans> payment_plans_found = SmartSpikeUtilityV2017.Utility_Find_PaymentPlansY(ourviewmodel,
                                                                                                        utilityviewmodel,
                                                                                                        Convert.ToChar(payment_plan.Substring(i, 1)));
                        foreach (SmartUtility.PaymentPlans payment_plan_row in payment_plans_found)
                        {
                            if (i > 0)
                            {
                                payment_names += Environment.NewLine;
                            }
                            payment_names += payment_plan_row.ALTERNATE_NAME1;
                            break;
                        }
                    }

                    if ((analysis_costs_row.SUPPLIER_CODE != last_supplier_code) ||
                        (analysis_costs_row.BRAND_CODE != last_brand_code))
                    {
                        //SUPPLIER_NAME
                        BRAND_NAME = SmartSpikeUtilityV2017.Utility_Lookup_SupplierName(ourviewmodel,
                                                                        utilityviewmodel,
                                                                        analysis_costs_row.SUPPLIER_CODE,
                                                                        analysis_costs_row.BRAND_CODE);
                        last_supplier_code = analysis_costs_row.SUPPLIER_CODE;
                        last_brand_code = analysis_costs_row.BRAND_CODE;
                    }


                    // It should make any difference if we have E:VR|G:SR or G:SR||E:VR or E:SR|G:SR or G:SR|E:SR
                    // because when the d_analysisList is combined, it only combines with tariff (names) which are in
                    // both e_analysis_costs and g_analysis_costs lists.  So whether you come at this from the 'E'
                    // side or the 'G' side, it SHOULD pick up a tariff name (assuming that the tariff codes are the
                    // same tariff name for different resoures i.e. BG Standard SR = BG STandard VR = BG Standard SR  (Gas) = 1
                    // which we do check for in Integrity_Check
                    // Fingers crossed, eh?
                    if (!(analysis_costs_row.RESOURCE_CODE == last_resource_code &&
                        analysis_costs_row.RESOURCE_TYPE == last_resource_type))
                    {
                        //string[] resource_types = analysis_costs_row.RESOURCE_TYPE.Split(SmartParametersV2016.bar);

                        //string this_tariff_type = "";
                        //foreach (string tariff_type in resource_types)
                        //{
                        //    string[] further = tariff_type.Split(Convert.ToChar(SmartParametersV2016.colon));
                        //    char this_resource = Convert.ToChar(further[0]);
                        switch (analysis_costs_row.RESOURCE_CODE)
                        {
                            case SmartParametersV2016.Electricity:
                                // Returns Single-Rate or Variable-Rate
                                string E_TARIFF_TYPE = SmartSpikeUtilityV2017.Utility_Lookup_TariffType(ourviewmodel,
                                                utilityviewmodel,
                                                analysis_costs_row.RESOURCE_CODE,
                                                analysis_costs_row.RESOURCE_TYPE);
                                if (TARIFF_TYPE.IndexOf(E_TARIFF_TYPE) == -1)
                                {
                                    if (!string.IsNullOrEmpty(TARIFF_TYPE))
                                    {
                                        TARIFF_TYPE += "/";
                                    }
                                    TARIFF_TYPE += E_TARIFF_TYPE;
                                }
                                RESOURCE_TYPE = analysis_costs_row.RESOURCE_TYPE;
                                break;
                            case SmartParametersV2016.Gas:
                                // Returns Single-Rate
                                string G_TARIFF_TYPE = SmartSpikeUtilityV2017.Utility_Lookup_TariffType(ourviewmodel, utilityviewmodel,
                                                analysis_costs_row.RESOURCE_CODE,
                                                analysis_costs_row.RESOURCE_TYPE);
                                if (TARIFF_TYPE.IndexOf(G_TARIFF_TYPE) == -1)
                                {
                                    if (!string.IsNullOrEmpty(TARIFF_TYPE))
                                    {
                                        TARIFF_TYPE += "/";
                                    }
                                    TARIFF_TYPE += G_TARIFF_TYPE;
                                }
                                RESOURCE_TYPE = analysis_costs_row.RESOURCE_TYPE;
                                break;
                            case SmartParametersV2016.DualFuel:
                                // Returns Single-Rate or Variable-Rate and Single-Rate (Gas)
                                string[] rts = analysis_costs_row.RESOURCE_TYPE.Split('|');
                                if (rts.Length > 0)
                                {
                                    string E_TYPE = SmartSpikeUtilityV2017.Utility_Lookup_TariffType(ourviewmodel,
                                                                                    utilityviewmodel,
                                                                                    SmartParametersV2016.Electricity,
                                                                                    rts[0]);
                                    TARIFF_TYPE = E_TYPE;
                                    RESOURCE_TYPE = rts[0]; // Cos Gas is always 'SR' but Elec might not be
                                    string G_TYPE = SmartSpikeUtilityV2017.Utility_Lookup_TariffType(ourviewmodel,
                                                                                        utilityviewmodel,
                                                                                        SmartParametersV2016.Gas,
                                                                                        rts[1]);
                                    if (!string.IsNullOrEmpty(TARIFF_TYPE))
                                    {
                                        TARIFF_TYPE += "/";
                                    }
                                    TARIFF_TYPE += G_TYPE;
                                }
                                break;
                            default:
                                break;
                        }
                        last_resource_code = analysis_costs_row.RESOURCE_CODE;
                        last_resource_type = analysis_costs_row.RESOURCE_TYPE;
                    }
                    string TARIFF_NAME = SmartSpikeUtilityV2017.Utility_Lookup_TariffName(utilityviewmodel,
                                                                                analysis_costs_row.RESOURCE_CODE,
                                                                                analysis_costs_row.SUPPLIER_CODE,
                                                                                analysis_costs_row.BRAND_CODE,
                                                                                RESOURCE_TYPE,
                                                                                analysis_costs_row.TARIFF_CODE);
                    SmartUtility.AnalysisCostsView abc = new SmartUtility.AnalysisCostsView()
                    {
                        USERNAME = displayName,
                        SUPPLIER_CODE = analysis_costs_row.SUPPLIER_CODE,
                        BRAND_CODE = analysis_costs_row.BRAND_CODE,
                        SUPPLIER_NAME = BRAND_NAME, //SUPPLIER_NAME,
                        TARIFF_CODE = analysis_costs_row.TARIFF_CODE,
                        TARIFF_NAME = TARIFF_NAME,
                        TCR = analysis_costs_row.TCR,
                        RESOURCE_TYPE = RESOURCE_TYPE,
                        METER_TYPE = TARIFF_TYPE,
                        PAYMENT_PLAN = analysis_costs_row.PAYMENT_PLAN,
                        PAYMENT_NAME = payment_names,
                        //TOTAL_AMOUNT = (SmartRoutinesV2018.ReturnDecimal(analysis_costs_row.TOTAL_COST, true) * utilityviewmodel.exchange_rate).ToString("c", utilityviewmodel.utilityDisplayCulture)
                        TOTAL_AMOUNT = SmartUtilityV2022.DisplayValue(ourviewmodel,
                                                    utilityviewmodel,
                                                    utilityviewmodel.ConvertToSymbol,
                                                    ourviewmodel.exchangeRate,
                                                    analysis_costs_row.TOTAL_COST / 100.0m)
                        //((decimal)(analysis_costs_row.TOTAL_COST / 100.0) * ourviewmodel.exchangeRate[utilityviewmodel.currencyOrdinal]).ToString("c", utilityviewmodel.utilityDisplayCulture)
                    };
                    analysis_costs_tempList.Add(abc);
                }
            }
            // This will either contain something ... or not
            return analysis_costs_tempList;
        }

        internal static async Task<bool> Analyze_Costs(MainViewModel ourviewmodel,
                                            UtilityViewModel utilityviewmodel,
                                            bool wildcard,
                                            short displayedAreaCode,
                                            char displayed_resource_code,
                                            string displayed_resource_type,
                                            short displayed_brand_code,     // One in green display
                                            short displayed_supplier_code,  // One in green display
                                            short comparison_brand_code,    // One we might choose (wildcard or non-wildcard)
                                            short comparison_supplier_code, // One we might choose (wildcard or non-wildcard)
                                            int comparison_tariff_code,     // One we might choose (wildcard or non-wildcard)
                                            char comparison_payment_plan,
                                            short target_supplier_code,
                                            int target_tariff_code,
                                            char target_payment_plan,
                                            DateTime engine_from_date,
                                            int age,                        // Used in Pre-Condition
                                            DateTime withdrawn_date,
                                            List<SmartUtility.AnalysisConditions> already_doneList)
        {
            utilityviewmodel.analyzeMessage = "";

            //
            // So you are mimicking the Supplier's bills,
            // and using the Smart Meter usage (if any) at the end ...
            // they are the icing on the cake, so to speak -
            //
            // The engine table contain the apportioned (if applicable)
            // bands and charges for THIS supplier id (i.e. the one in the display)
            //


            if ((displayedAreaCode == 0) ||
                (displayed_supplier_code == 0))
            // || (displayed_tariff_code == "000")) Its simply too dangerous
            //                                    to leave this in - if we ever
            //                                    can't lookup the tariff then
            //                                    all analyses don't work .. too risky
            {
                return false;
            }
            // For the non-wildcard

            string process_codes = "";
            if (wildcard)
            {
                switch (displayed_resource_code)
                {
                    case SmartParametersV2016.Electricity:
                        utilityviewmodel.Hezbollah.e_analysis_costsList.Clear();
                        if (!string.IsNullOrEmpty(process_codes))
                        {
                            process_codes += SmartParametersV2016.bar;
                        }
                        process_codes += displayed_resource_code;
                        utilityviewmodel.analysisCost[0] = 0;

                        break;
                    case SmartParametersV2016.Gas:
                        utilityviewmodel.Hezbollah.g_analysis_costsList.Clear();
                        if (!string.IsNullOrEmpty(process_codes))
                        {
                            process_codes += SmartParametersV2016.bar;
                        }
                        process_codes += displayed_resource_code;
                        utilityviewmodel.analysisCost[1] = 0;
                        break;
                    case SmartParametersV2016.DualFuel:
                        utilityviewmodel.Hezbollah.e_analysis_costsList.Clear();
                        utilityviewmodel.Hezbollah.g_analysis_costsList.Clear();
                        process_codes = process_codes + SmartParametersV2016.Electricity +
                                                        SmartParametersV2016.bar +
                                                        SmartParametersV2016.Gas;
                        utilityviewmodel.analysisCost[0] = 0;
                        utilityviewmodel.analysisCost[1] = 0;
                        break;
                    default:
                        break;
                }
            }
            else
            {
                // NOW we clear the Tariff Costs down because we aren't wildcarding
                // Its the **ONLY ONE** we do clear down - all the others are READ-ONLY
                utilityviewmodel.Hezbollah.tariff_costsList.Clear();
                switch (displayed_resource_code)
                {
                    case SmartParametersV2016.Electricity:
                        if (!string.IsNullOrEmpty(process_codes))
                        {
                            process_codes += SmartParametersV2016.bar.ToString();
                        }
                        process_codes += displayed_resource_code.ToString();
                        utilityviewmodel.analysisCost[0] = 0;

                        break;
                    case SmartParametersV2016.Gas:
                        if (!string.IsNullOrEmpty(process_codes))
                        {
                            process_codes += SmartParametersV2016.bar.ToString();
                        }
                        process_codes += displayed_resource_code.ToString();
                        utilityviewmodel.analysisCost[1] = 0;

                        break;
                    case SmartParametersV2016.DualFuel:
                        process_codes += SmartParametersV2016.Electricity.ToString() +
                                                        SmartParametersV2016.bar.ToString() +
                                                        SmartParametersV2016.Gas.ToString();
                        utilityviewmodel.analysisCost[0] = 0;
                        utilityviewmodel.analysisCost[1] = 0;
                        break;
                    default:
                        break;
                }
            }

            
            string[] process_codesList = process_codes.Split(SmartParametersV2016.bar);

            foreach (string process_code in process_codesList)
            {
                char process_resource_code = Convert.ToChar(process_code);
                utilityviewmodel.uniqueNumber = 0;
                // Work through the resource we have on the Display  ... but if its 'D' then do BOTH 'E' and 'G'
                if (!Process_Resource(ourviewmodel,
                                        utilityviewmodel,
                                        wildcard,
                                        displayedAreaCode,
                                            process_resource_code,
                                            //displayed_brand_code,
                                            //displayed_supplier_code,
                                            //displayed_resource_type,
                                            comparison_brand_code,
                                            comparison_supplier_code,
                                            comparison_tariff_code,
                                            comparison_payment_plan,
                                            target_supplier_code,
                                            target_tariff_code,
                                            target_payment_plan,
                                            engine_from_date,
                                            age,
                                            withdrawn_date,
                                            already_doneList))
                {
                    // A failure of some kind, try and send a message back to HQ
                    string[] components = utilityviewmodel.analyzeMessage.Split(SmartParametersV2016.fieldSeparator);
                    if (components.Length >= 4)
                    {
                        await SmartBobV2017.ListenerAsync(ourviewmodel, utilityviewmodel.utilityToken, Convert.ToInt16(components[0]), Convert.ToInt16(components[1]), components[2], components[3]);
                    }
#if WINFORMS
                    await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, utilityviewmodel.analyzeMessage + Environment.NewLine.ToString());
#endif
                    return false;
                }
            }
            if (!wildcard)
            {
                // Sort whatever we got back - because the Post-Conditions amounts
                // have dates earlier in the sequence ...

                // These are for the BREAKDOWN
                utilityviewmodel.Hezbollah.tariff_costsList.Sort();  //  <= CHECK THIS COMES OUT IN ASC PERIOD_END !!!
            }
            // These two aren't needed anymore .. thanks & goodbye!
            return true;
        }


        private static bool Process_Resource(MainViewModel ourviewmodel,
                                        UtilityViewModel utilityviewmodel,
                                        bool wildcard,
                                        short displayedAreaCode,
                                        char displayed_resource_code,
                                        //short displayed_brand_code,
                                        //short displayed_supplier_code,
                                        //string displayed_resource_type,
                                        short comparison_brand_code,
                                        short comparison_supplier_code,
                                        int comparison_tariff_code,
                                        char comparison_payment_plan,
                                        short target_supplier_code,
                                        int target_tariff_code,
                                        char target_payment_plan,
                                        DateTime engine_from_date,
                                        int age,
                                        DateTime withdrawn_date,
                                        List<SmartUtility.AnalysisConditions> already_doneList,
                                        [System.Runtime.CompilerServices.CallerMemberName] string routine = "")
        {
            FieldInfo[] myFields = typeof(SmartUtility.BrandMatrix).GetFields(SmartParametersV2016.bindingFlags);

            List<SmartUtility.BrandsView> brandsview_found = SmartSpikeUtilityV2017.Utility_Determine_Brands(ourviewmodel,
                                                            utilityviewmodel,
                                                            wildcard,
                                                            displayed_resource_code,
                                                            comparison_supplier_code,
                                                            withdrawn_date);

            foreach (SmartUtility.BrandsView brandview_row in brandsview_found)

            {
                List<SmartUtility.BrandMatrix> brand_matrix_found = SmartSpikeUtilityV2017.Utility_Find_Brand_Matrix(ourviewmodel,
                                                                                            utilityviewmodel,
                                                                                            brandview_row.BRAND_CODE);

                foreach (SmartUtility.BrandMatrix brand_matrix_row in brand_matrix_found)
                {
                    int brand_field_index = SmartNibbyV2016.Area_Matrix_Index(displayedAreaCode, myFields);
                    // Check to see if the Supplier by means of their Brand actually DOES the Area under consideration
                    // If they don't, then we can *safely assume* they have no applicable Tariffs
                    // for that Area (!!)
                    if (brand_field_index == -1)
                    {
                        utilityviewmodel.analyzeMessage = Build_Analyze_Message(
                                                                comparison_brand_code,
                                                                comparison_supplier_code,
                                                                comparison_payment_plan,
                                                                brandview_row.RESOURCE_CODE,
                                                                brandview_row.RESOURCE_TYPE,
                                                                "AnalyzeBrandMatrix" +
                                                                SmartParametersV2016.space +
                                                                displayedAreaCode.ToString(),
                                                                routine);
                        return false;
                    }

                    if (Convert.ToChar(myFields[brand_field_index].GetValue(brand_matrix_row).ToString()) == SmartParametersV2016.brandMatrixValid)
                    {
                        //string supplier_name = supplier_row.SUPPLIER_NAME;  // For old time's sake..
                        //if (!string.IsNullOrEmpty(supplier_name))
                        string brand_name = brandview_row.BRAND_NAME;  // For old time's sake..
                        if (!string.IsNullOrEmpty(brand_name))
                        {
                            if (!Process_Area_Brand(ourviewmodel,
                                                        utilityviewmodel,
                                                        wildcard,
                                                        displayedAreaCode,
                                                        brandview_row.RESOURCE_CODE,
                                                        brandview_row.RESOURCE_TYPE,
                                                        brandview_row.SUPPLIER_CODE,
                                                        brandview_row.BRAND_CODE,
                                                        comparison_supplier_code,
                                                        comparison_brand_code,
                                                        comparison_tariff_code,
                                                        comparison_payment_plan,
                                                        withdrawn_date,
                                                        target_supplier_code,
                                                        target_tariff_code,
                                                        target_payment_plan,
                                                        engine_from_date,
                                                        age,
                                                        already_doneList))
                            {
                                return false;
                            }
                        }
                    }
                }
            }
            return true;
        }

        private static string Build_Analyze_Message(short brand_code,
                                                    short supplier_code,
                                                    char payment_plan,
                                                    char resource_code,
                                                    string resource_type,
                                                    string message,
                                                    string routine)
        {
            return brand_code.ToString() + SmartParametersV2016.fieldSeparator +
                    supplier_code.ToString() + SmartParametersV2016.fieldSeparator +
                    payment_plan.ToString() + SmartParametersV2016.space + resource_code.ToString() +
                    SmartParametersV2016.space + resource_type +
                    SmartParametersV2016.space + message +
                    SmartParametersV2016.fieldSeparator +
                    routine;
        }

        private static bool Process_Area_Brand(MainViewModel ourviewmodel,
                                                UtilityViewModel utilityviewmodel,
                                                bool wildcard,
                                                short displayedAreaCode,
                                                char displayed_resource_code,
                                                string displayed_resource_type,
                                                short displayed_supplier_code,
                                                short displayed_brand_code,
                                                short comparison_supplier_code,
                                                short comparison_brand_code,
                                                int comparison_tariff_code,
                                                char comparison_payment_plan,
                                                DateTime withdrawn_date,
                                                short target_supplier_code,
                                                int target_tariff_code,
                                                char target_payment_plan,
                                                DateTime engine_from_date,
                                                int age,
                                                List<SmartUtility.AnalysisConditions> already_doneList,
                                                [System.Runtime.CompilerServices.CallerMemberName] string routine = "")
        {
            //short comparison_supplier_code = 0;
            int fallback_tariff_code;

            // This Supplier 'does' the Area ...
            // Now we have to find out for which Resource do they have a Tariff
            // available.  It could well be, that they have - say - a Gas
            // Tariff for one Area which they don't sell in another

            // So which Brand is it?
            //char active_code = SmartParametersV2016.defaultChar;  // We want all brands
            //List<SmartUtility.Brands> brand_found = SmartSpikeUtilityV2017.Lookup_Utility_Brands(ourviewmodel,
            //                                                utilityviewmodel,
            //                                                displayed_resource_code,
            //                                                displayed_resource_type,
            //                                                active_code,
            //                                                comparison_supplier_code,
            //                                                comparison_brand_code);

            // Suppliers should be in Area once only but for SOuthern Electric there are 4
            //if (brand_found.Count == 1)
            //{
            // We should cycle through here only ONCE
            // Does this Supplier to the Area have any
            // Tariffs which match the Resource for this Area?

            // CHANGE THIS TO BE A BRAND_code !!!!
            fallback_tariff_code = 0;
            comparison_supplier_code = displayed_supplier_code; // brand_found[0].SUPPLIER_CODE;

            List<SmartUtility.Tariffs> fallback_found;
            // Lookup the fallback tariff id

            // **
            // BUSINESS RULE: A FALLBACK TARIFF can never be a PLACEHOLDER
            // **
            fallback_found = SmartSpikeUtilityV2017.Utility_Find_Tariffs_Fallback(ourviewmodel,
                                                                    utilityviewmodel,
                                                                    comparison_supplier_code,
                                                                    displayed_resource_code,
                                                                    displayed_resource_type,
                                                                    comparison_tariff_code);

            //(from Tariff
            //                    in Hezbollah.tariffsList
            //                where ((Tariff.SUPPLIER_CODE == comparison_supplier_code) &&
            //                    (Tariff.RESOURCE_CODE == displayed_resource_code) &&
            //                    (Tariff.RESOURCE_TYPE == displayed_resource_type) &&
            //                    (Tariff.TARIFF_CODE == comparison_tariff_code) &&
            //                    (Tariff.FALLBACK == "Y"))
            //                select Tariff);
            if (fallback_found.Count > 0)
            {
                foreach (SmartUtility.Tariffs fallback_row in fallback_found)
                {
                    fallback_tariff_code = fallback_row.TARIFF_CODE;
                    break;
                }
            }

            List<SmartUtility.TariffMatrix> tariff_matrix_found;// = new List<SmartUtility.TariffMatrix>();
            if (wildcard)
            {
                tariff_matrix_found = SmartSpikeUtilityV2017.Utility_TariffMatrixList(ourviewmodel,
                                                                                utilityviewmodel,
                                                                                displayed_resource_code,
                                                                                displayed_resource_type,
                                                                                displayed_brand_code,
                                                                                0);

                if (comparison_supplier_code == 0)
                {
                    comparison_supplier_code = displayed_supplier_code;
                }
                if (comparison_brand_code == 0 && displayed_brand_code != 0)
                {
                    comparison_brand_code = displayed_brand_code;
                }
            }
            else
            {
                tariff_matrix_found = SmartSpikeUtilityV2017.Utility_TariffMatrixList(ourviewmodel,
                                                                                utilityviewmodel,
                                                                                displayed_resource_code,
                                                                                displayed_resource_type,
                                                                                comparison_brand_code,
                                                                                comparison_tariff_code);
            }
            if (tariff_matrix_found.Count == 0)
            {
                // It may WELL BE that a Supplier is valid but has WITHDRAWN all their Tariffs
                // and by the WITHDRAWN_DATE has nothing to switch to.
                // We can do NO MORE than simply ignore them ...
                return true;
            }
            else
            {
                FieldInfo[] myFields = typeof(SmartUtility.TariffMatrix).GetFields(SmartParametersV2016.bindingFlags);
                int tariff_field_index = SmartNibbyV2016.Area_Matrix_Index(displayedAreaCode, myFields);
                if (tariff_field_index == -1)
                {
                    utilityviewmodel.analyzeMessage = Build_Analyze_Message(comparison_brand_code,
                                                comparison_supplier_code,
                                                comparison_payment_plan,
                                                displayed_resource_code,
                                                displayed_resource_type,
                                                "AnalyzeTariffMatrix" +
                                                SmartParametersV2016.space +
                                                displayedAreaCode.ToString(),
                                                routine);
                    return false;
                }
                else
                {
                    // Yes - they have one or more Tariffs for this Area and Resource
                    foreach (SmartUtility.TariffMatrix tariff_matrix_row in tariff_matrix_found)
                    {
                        if (Convert.ToChar(myFields[tariff_field_index].GetValue(tariff_matrix_row).ToString()) == SmartParametersV2016.tariffMatrixValid)
                        {
                            List<SmartUtility.Tariffs> tariffs_found = SmartSpikeUtilityV2017.Utility_TariffsList(ourviewmodel,
                                                                                        utilityviewmodel,
                                                                                        comparison_supplier_code,
                                                                                        tariff_matrix_row.RESOURCE_CODE,
                                                                                        tariff_matrix_row.RESOURCE_TYPE,
                                                                                        tariff_matrix_row.TARIFF_CODE);
                            if (tariffs_found.Count > 0)
                            {
                                // The check for DEACTIVATED is done before we get into here

                                // Find the ranges:
                                //                   19-Sep-2011  ....  25-Oct-2011
                                // From 01-Jan-2011 to 31-Dec-2011 is good because 01-Jan < 25-Oct and 31-Dec > 19-Sep
                                //      01-Feb-2011 to 31-Mar-2011 will fail because 31-Mar is not >= 19-Sep
                                //      01-Nov-2011 to 31-Dec-2011 will fail because 01-Nov is not <= 25-Oct
                                //      01-Mar-2011 to 30-Sep-2011 is good because 30-Sep >= 19-Sep and 01-Mar <= 25-Oct
                                //      01-Oct-2011 to 31-Dec-2011 is good because 01-Oct <= 25-Oct and 31-Dec >= 19-Sep

                                //tariffs_expression = "SUPPLIER_code = '" + compare_supplier_code + "'" +
                                //                            " AND RESOURCE_CODE = '" + displayed_resource_code.ToString() + "'" +
                                //                            " AND TARIFF_code = '" + compare_tariff_code + "'" +
                                //    //" AND VALID_FROM " + "<= '" + to_date + "'" +
                                //    //" AND VALID_TO " + ">= '" + from_date + "'";
                                //                            " AND VALID_FROM " + "<= '" + from_date + "'" +
                                //                            " AND VALID_TO " + ">= '" + from_date + "'";

                                // There should only be ONE record for each Code ...
                                if ((tariffs_found[0].PRICES_VALID_FROM <= engine_from_date) &&
                                    (tariffs_found[0].VALID_TO >= engine_from_date))
                                {
                                    //customer = false;
                                    // Find out if this Tariff is "Customer Only" i.e. it only is
                                    // worth checking IF you are already a Customer of this Supplier
                                    if ((tariffs_found[0].CUSTOMER == "Y") &&
                                        (comparison_supplier_code != displayed_supplier_code))
                                    {
                                        continue;   // Skip this one
                                    }
                                    else
                                    {
                                        //customer = true;
                                    }
                                    if (!Pre_Condition(ourviewmodel,
                                                        utilityviewmodel,
                                                        wildcard,
                                                        displayed_resource_code,
                                                        displayed_supplier_code,
                                                        comparison_brand_code,
                                                        comparison_supplier_code,
                                                        tariff_matrix_row.TARIFF_CODE,
                                                        tariff_matrix_row.VERSION_CODE,
                                                        comparison_payment_plan,
                                                        withdrawn_date,
                                                        target_supplier_code,
                                                        target_tariff_code,
                                                        target_payment_plan,
                                                        fallback_tariff_code,
                                                        displayed_resource_type,
                                                        age,
                                                        already_doneList))
                                    {
                                        return false;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            //}
            return true;
        }

        private static bool Pre_Condition(MainViewModel ourviewmodel,
                                            UtilityViewModel utilityviewmodel,
                                            bool wildcard,
                                            char displayed_resource_code,
                                            short displayed_supplier_code,
                                            short comparison_brand_code,
                                            short comparison_supplier_code,
                                            int comparison_tariff_code,
                                            short comparison_version_code,
                                            char comparison_payment_plan,
                                            DateTime withdrawn_date,
                                            short target_supplier_code,
                                            int target_tariff_code,
                                            char target_payment_plan,
                                            int fallback_tariff_code,
                                            string displayed_resource_type,
                                            int age,
                                            List<SmartUtility.AnalysisConditions> already_doneList,
                                            [System.Runtime.CompilerServices.CallerMemberName] string routine = "")
        {
            string FIELD_NAME,
                    PRE_CONDITION;

            // Does this Tariff have any Pre-Conditions about Age or whether
            // it is only allowed for existing Customers??
            List<SmartUtility.PreConditions> pre_conditions_found = SmartSpikeUtilityV2017.Utility_Find_PreConditions(ourviewmodel,
                                                                utilityviewmodel,
                                                                comparison_supplier_code,
                                                                displayed_resource_code,
                                                                comparison_tariff_code);

            //(from Pre_Condition
            //                                        in Hezbollah.pre_conditionsList
            //                                        where ((Pre_Condition.SUPPLIER_CODE == comparison_supplier_code) &&
            //                                             (Pre_Condition.RESOURCE_CODE == displayed_resource_code) &&
            //                                             (Pre_Condition.TARIFF_CODE == comparison_tariff_code))
            //                                        select Pre_Condition);
            if (pre_conditions_found.Count > 0)
            {
                foreach (SmartUtility.PreConditions pre_condition_row in pre_conditions_found)
                {
                    FIELD_NAME = pre_condition_row.FIELD_NAME;
                    switch (FIELD_NAME)
                    {
                        case "AGE":
                            PRE_CONDITION = pre_condition_row.PRE_CONDITION;
                            if (age < Convert.ToInt16(PRE_CONDITION))
                            {
                                return true; // This Tariff's condition not met
                            }
                            break;
                        case "CUSTOMER":
                            PRE_CONDITION = pre_condition_row.PRE_CONDITION;
                            switch (PRE_CONDITION)
                            {
                                case "DISPLAYED_SUPPLIER_ID":
                                    if (comparison_supplier_code != displayed_supplier_code)
                                    {
                                        return true; // This Tariff's condition not met
                                    }
                                    break;
                                default:
                                    utilityviewmodel.analyzeMessage = Build_Analyze_Message(comparison_brand_code,
                                                                            comparison_supplier_code,
                                                                            comparison_payment_plan,
                                                                            displayed_resource_code,
                                                                            displayed_resource_type,
                                                                            "AnalyzePreConditions" +
                                                                            SmartParametersV2016.space +
                                                                            PRE_CONDITION,
                                                                            routine);
                                    return false;     // Avoid race condition
                            }
                            break;
                        default:
                            utilityviewmodel.analyzeMessage = Build_Analyze_Message(comparison_brand_code,
                                                                    comparison_supplier_code,
                                                                    comparison_payment_plan,
                                                                    displayed_resource_code,
                                                                    displayed_resource_type,
                                                                    "AnalyzePreFieldName" +
                                                                    SmartParametersV2016.space +
                                                                    FIELD_NAME,
                                                                    routine);
                            return false;     // Avoid race condition
                    }
                }
            }

            // Now this Tariff is linked to certain Tariff Plans
            // so we need to work our way through the links

            // Does this Supplier have any Tariff Plan codes?
            // If not, then none of the Conditions and/or Post-Condition look-ups are going to work!
            List<SmartUtility.TariffPlans> tariff_plans_found = SmartSpikeUtilityV2017.Utility_Find_Tariff_Plans(ourviewmodel,
                                                                utilityviewmodel,
                                                                wildcard,
                                                                comparison_supplier_code,
                                                                comparison_payment_plan);
            //if (!wildcard)
            //{
            //    tariff_plans_found = (from Tariff_Plan
            //                          in Hezbollah.tariff_plansList
            //                          where ((Tariff_Plan.SUPPLIER_CODE == comparison_supplier_code) &&
            //                                 (Tariff_Plan.PAYMENT_PLANS.Contains(comparison_payment_plan)))
            //                          select Tariff_Plan);
            //}
            //else
            //{
            //    tariff_plans_found = (from Tariff_Plan
            //                          in Hezbollah.tariff_plansList
            //                          where (Tariff_Plan.SUPPLIER_CODE == comparison_supplier_code)
            //                          select Tariff_Plan);
            //}
            if (tariff_plans_found.Count > 0)
            {
                // We found our COMPARISON_PAYMENT_PLAN or ALL OF THEM
                if (!Process_Tariff(ourviewmodel,
                                        utilityviewmodel,
                                        wildcard,
                                        displayed_resource_code,
                                        comparison_brand_code,
                                        comparison_supplier_code,
                                        comparison_tariff_code,
                                        comparison_version_code,
                                        comparison_payment_plan,
                                        withdrawn_date,
                                        target_supplier_code,
                                        target_tariff_code,
                                        target_payment_plan,
                                        fallback_tariff_code,
                                        displayed_resource_type,
                                        already_doneList))
                {
                    return false;
                }
            }
            return true;
        }

        private static bool Process_Tariff(MainViewModel ourviewmodel,
                                            UtilityViewModel utilityviewmodel,
                                            bool wildcard,
                                            char displayed_resource_code,
                                            short comparison_brand_code,
                                            short comparison_supplier_code,
                                            int comparison_tariff_code,
                                            short comparison_version_code,
                                            char comparison_payment_plan,
                                            DateTime withdrawn_date,
                                            short target_supplier_code,
                                            int target_tariff_code,
                                            char target_payment_plan,
                                            int fallback_tariff_code,
                                            string displayed_resource_type,
                                            List<SmartUtility.AnalysisConditions> already_doneList,
                                            [System.Runtime.CompilerServices.CallerMemberName] string routine = "")
        {
            short group_code,
                   version_code,
                   payment_code,
                   selection_code;

            utilityviewmodel.condition_costs = 0.0M;
            utilityviewmodel.post_condition_costs = 0.0M;
            utilityviewmodel.tcr = "";

            // Move this shit OUT one day ..... that day is NOW!!

            // Now each link points to a Payment Method which we need to 
            // a) Find the details about and then
            // b) Find the conditions pertaining to
            // (should only be ONE of each Payment Method Codes in table)

            List<SmartUtility.ConditionsPlans> conditions_plans_found;
            if (!wildcard)
            {
                // We only look for one comparison
                //  conditions_plans_expression =
                //    conditions_plans_expression +
                //        " AND PAYMENT_PLANS LIKE '%" + comparison_payment_plan + "%'";

                // Ok .. we have an 'A' (Direct Debit) showing in the Payment Plan combox which is how
                // Bob pays ...   And we can see the Analysis Costs of all the cheaper costs.
                // Supposing one of these is an 'A', then we are fine because '.Contains' will find it.
                // But what if the cheaper method is a 'B' (standing Order) *OR* the Supplier does not
                // 'do' plans of type 'A'?  We need to give a breakdown of whatever is showing, be it
                // 'B', 'C', 'D' or whateever.
                //
                // Try for an 'A' first ('A' comes from the Payment Plan drop down)

                conditions_plans_found = SmartSpikeUtilityV2017.Utility_ConditionsPlansList(ourviewmodel,
                                                                                utilityviewmodel,
                                                                                displayed_resource_code,
                                                                                displayed_resource_type,
                                                                                comparison_supplier_code,
                                                                                comparison_tariff_code,
                                                                                comparison_version_code,
                                                                                comparison_payment_plan);
                if (conditions_plans_found.Count == 0)
                {
                    // We didn't find an 'A', so look for ANYTHING which matches the one selected ..
                    conditions_plans_found = SmartSpikeUtilityV2017.Utility_ConditionsPlansList(ourviewmodel,
                                                                                utilityviewmodel,
                                                                                displayed_resource_code,
                                                                                displayed_resource_type,
                                                                                comparison_supplier_code,
                                                                                comparison_tariff_code,
                                                                                comparison_version_code,
                                                                                SmartParametersV2016.defaultChar);
                }
            }
            else
            {
                conditions_plans_found = SmartSpikeUtilityV2017.Utility_ConditionsPlansList(ourviewmodel,
                                                                                utilityviewmodel,
                                                                                displayed_resource_code,
                                                                                displayed_resource_type,
                                                                                comparison_supplier_code,
                                                                                comparison_tariff_code,
                                                                                comparison_version_code,
                                                                                SmartParametersV2016.defaultChar);

            }
            if (conditions_plans_found.Count > 0)
            {
                foreach (SmartUtility.ConditionsPlans conditions_plans_row in conditions_plans_found)
                {
                    utilityviewmodel.condition_costs = 0.0M; // Total costs to be returned
                    utilityviewmodel.tcr = "";

                    version_code = conditions_plans_row.VERSION_CODE;
                    payment_code = conditions_plans_row.PAYMENT_CODE;
                    comparison_payment_plan = Convert.ToChar(conditions_plans_row.PAYMENT_PLANS);
                    //tier_count = conditions_plans_row.TIER_COUNT;
                    // Clear down the Costs table for each Tariff and Payment Type!
                    // I don't understand this, the tariff_costs table is
                    // only ever used for non-wildcard operations on a cell double-click?
                    // I am commenting it out ... it only ever needs to be cleared ONCE
                    // right at the start when we are doing a non-wildcard ...?? Yes???   

                    // Set the Condition Costs
                    bool task_pc = Process_Conditions(ourviewmodel,
                                                    utilityviewmodel,
                                                    wildcard,
                                                    displayed_resource_code,
                                                    displayed_resource_type,
                                                    comparison_brand_code,
                                                    comparison_supplier_code,
                                                    comparison_tariff_code,
                                                    comparison_payment_plan,
                                                    withdrawn_date,
                                                    target_supplier_code,
                                                    target_tariff_code,
                                                    target_payment_plan,
                                                    fallback_tariff_code,
                                                    version_code,
                                                    payment_code,
                                                    already_doneList);
                    if (!task_pc)
                    {
                        return task_pc;
                    }
                    else
                    {
                        // The way it works ....  <=====***********YOU NEED TO IMPLEMENT THIS******

                        // We may not have found any Condition_Dates for a Payment Plan
                        if (utilityviewmodel.condition_costs != 0.0M)
                        {
                            //
                            // POST_GROUPS
                            //  Has a Supplier (5) a resource (E) and a Group Code (1) which defines the resource for
                            //  the Group Code of the Supplier
                            //
                            // POST_CODES
                            // Has a Supplier (5) a Tariff (2) and a Group Code which defines the Group Code for
                            // the Tariff for the SUpplier
                            //
                            // POST_GROUPINGS
                            // Has a Supplier (5) a Group Code (1) and a Selection Code (1) which defines
                            // the Selections for the Group for the SUpplier 

                            // So ... given a Supplier, a Tariff and a Resource
                            // Look up the SUPPLIERS
                            // Look up the TARIFFS
                            // Look up the POST_GROUPS (cross PCG.RESOURCE with TARIFFS.RESOURCE)
                            //  Look up Post_Codes (cross TC.TARIFF with TARIFFS.TARIFF and
                            //				TC.GROUP with PCH.Group


                            // Now each Tariff Code contains two essential elements:
                            //    The first is a set of Tariff Plans to which the POST CONDITIONS apply
                            //    The second is the Group Code which points into the POST_CONDITIONS tables

                            //  There is NO LINK between the Post_Codes PAYMENT_PLANS
                            //  and the Conditions PAYMENT_PLANS.  Why?  Because you could have
                            //  a Payment Plan with Conditions but NO Post Condiitons
                            //
                            //  There IS a link between the Post_Codes and the Post-Conditions table
                            //  This link is the GROUP_CODE

                            // First of all, try and match up the type of Comparison plan we have now, eg 'A'
                            // and see if the TARIFF_PLANS_TABLE field PAYMENT_PLANS has a match - if it does,
                            // then we can go and find the Conditions which apply

                            //tariff_code_expression = "SUPPLIER_code = '" + comparison_supplier_code + "'" +
                            //                    " AND TARIFF_code = '" + comparison_tariff_code + "'";
                            if (!wildcard)
                            {
                                //tariff_code_expression = tariff_code_expression  +
                                //                        " AND TARIFF_PLAN LIKE '%" + comparison_tariff_plan + "'";
                            }
                            List<SmartUtility.Post_Codes> post_codes_found = SmartSpikeUtilityV2017.Utility_Find_PostCodes(ourviewmodel,
                                                                utilityviewmodel,
                                                                displayed_resource_code,
                                                                comparison_supplier_code,
                                                                comparison_tariff_code);
                            //(from Tariff_Code
                            //                                in Hezbollah.Post_CodesList
                            //                                                   where ((Tariff_Code.SUPPLIER_CODE == comparison_supplier_code) &&
                            //                                                               (Tariff_Code.RESOURCE_CODE == displayed_resource_code) &&
                            //                                                               (Tariff_Code.TARIFF_CODE == comparison_tariff_code))
                            //                                                   select Tariff_Code);

                            utilityviewmodel.post_condition_costs = 0.0M;
                            foreach (SmartUtility.Post_Codes post_conditions_code_row in post_codes_found)
                            {
                                group_code = post_conditions_code_row.GROUP_CODE;
                                if (group_code != 0)
                                {
                                    List<SmartUtility.Post_Groupings> post_groupings_found = SmartSpikeUtilityV2017.Utility_Find_PostGroupings(ourviewmodel,
                                                                                                                    utilityviewmodel,
                                                                                                                    comparison_supplier_code,
                                                                                                                    group_code);
                                    //(from Post_Conditions_Grouping
                                    //                                        in Hezbollah.post_groupingsList
                                    //                                        where ((Post_Conditions_Grouping.SUPPLIER_CODE == comparison_supplier_code) &&
                                    //                                                (Post_Conditions_Grouping.GROUP_CODE == group_code))
                                    //                                        select Post_Conditions_Grouping);
                                    if (post_groupings_found.Count > 0)
                                    {
                                        foreach (SmartUtility.Post_Groupings post_groupings_row in post_groupings_found)
                                        {
                                            selection_code = post_groupings_row.SELECTION_CODE;
                                            // Set the Post Condition Costs
                                            if (!Process_Post_Conditions(ourviewmodel,
                                                                        utilityviewmodel,
                                                                        wildcard,
                                                                        displayed_resource_code,
                                                                        displayed_resource_type,
                                                                        comparison_brand_code,
                                                                        comparison_supplier_code,
                                                                        comparison_payment_plan,
                                                                        selection_code))
                                            {
                                                return false;
                                            }
                                        }
                                    }
                                }
                            }

                            // Now we have (or we SHOULD fucking have after all this pain)
                            // all the facts and figures we need in the tariff_engine_table
                            // and tariff_costs_table.
                            // Just add the Condition costs to the Post-Condition costs:
                            switch (displayed_resource_code) // <=  FUCKING IDIOT!!! 'utilityviewmodel' DOH!
                            {
                                case SmartParametersV2016.Electricity:
                                    utilityviewmodel.analysisCost[0] = Convert.ToInt32(utilityviewmodel.condition_costs + utilityviewmodel.post_condition_costs);
                                    break;
                                case SmartParametersV2016.Gas:
                                    utilityviewmodel.analysisCost[1] = Convert.ToInt32(utilityviewmodel.condition_costs + utilityviewmodel.post_condition_costs);
                                    break;
                                default:
                                    break;
                            }
                            if (wildcard)
                            {
                                // No DateTimes in here!!
                                utilityviewmodel.uniqueNumber = (short)(utilityviewmodel.uniqueNumber + 1);
                                SmartUtility.AnalysisCosts analysis_costs_row = new SmartUtility.AnalysisCosts()
                                {
                                    USERNAME = ourviewmodel.UserName,
                                    CUBEFACE_CODE = SmartParametersV2016.Utility,
                                    RESOURCE_CODE = displayed_resource_code,
                                    UNIQUE_NUMBER = utilityviewmodel.uniqueNumber,
                                    SUPPLIER_CODE = comparison_supplier_code,
                                    BRAND_CODE = comparison_brand_code,          // So 'Atlantic' shows up not SSE
                                    TARIFF_CODE = comparison_tariff_code,
                                    RESOURCE_TYPE = //displayed_resource_code + SmartParametersV2016.colon + 
                                                    displayed_resource_type,
                                    PAYMENT_PLAN = conditions_plans_row.PAYMENT_PLANS,
                                };
                                if (!string.IsNullOrEmpty(utilityviewmodel.tcr))
                                {
                                    analysis_costs_row.TCR = SmartRoutinesV2018.ConvertDecimal(utilityviewmodel.tcr);
                                }
                                else
                                {
                                    analysis_costs_row.TCR = SmartRoutinesV2018.ConvertDecimal("0.0");
                                }
                                analysis_costs_row.TOTAL_COST = Convert.ToInt32(utilityviewmodel.condition_costs + utilityviewmodel.post_condition_costs);
                                switch (displayed_resource_code)
                                {
                                    case SmartParametersV2016.Electricity:
                                        utilityviewmodel.Hezbollah.e_analysis_costsList.Add(analysis_costs_row);
                                        break;
                                    case SmartParametersV2016.Gas:
                                        utilityviewmodel.Hezbollah.g_analysis_costsList.Add(analysis_costs_row);
                                        break;
                                    default:
                                        break;
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                utilityviewmodel.analyzeMessage = Build_Analyze_Message(comparison_brand_code,
                                                        comparison_supplier_code,
                                                        comparison_payment_plan,
                                                        displayed_resource_code,
                                                        displayed_resource_type,
                                                        "AnalyzeConditionsPlans" +
                                                        SmartParametersV2016.space +
                                                        comparison_tariff_code.ToString() +
                                                        SmartParametersV2016.space +
                                                        comparison_payment_plan,
                                                        routine);
                return false;     // Avoid race condition
            }
            return true;
        }


        internal static bool Special_Date_Compare(bool how, DateTime date1, DateTime date2)
        {
            switch (how)
            {
                case false:
                    if (SmartRoutinesV2018.DateTimeCompare(date1, date2) >= 0)
                    {
                        return true;
                    }
                    break;
                case true:
                    if (SmartRoutinesV2018.DateTimeCompare(date1, date2) == 0)
                    {
                        return true;
                    }
                    break;
            }
            return false;
        }

        private static bool Process_Conditions(MainViewModel ourviewmodel,
                                                UtilityViewModel utilityviewmodel,
                                                bool wildcard,
                                                char displayed_resource_code,
                                                string displayed_resource_type,    // We may turn this on or off (still)
                                                short comparison_brand_code,
                                                short comparison_supplier_code,
                                                int comparison_tariff_code,
                                                char comparison_payment_plan,
                                                DateTime withdrawn_date,
                                                short target_supplier_code,
                                                int target_tariff_code,
                                                char target_payment_plan,
                                                int fallback_tariff_code,
                                                short version_code,
                                                short payment_code,
                                                List<SmartUtility.AnalysisConditions> already_doneList,
                                                [System.Runtime.CompilerServices.CallerMemberName] string routine = "")
        {
            //
            //  There is NO LINK between the Post_Codes PAYMENT_PLANS
            //  and the Conditions PAYMENT_PLANS.  Why?  Because you could have
            //  a Payment Plan with NO Post Condiitons, but WITH Conditions
            //

            // Test out the Conditions Limits Areas part

            List<SmartUtility.UnitRates>[] rates_found = new List<SmartUtility.UnitRates>[3]
                                                                            {
                                                                              new List<SmartUtility.UnitRates>(),
                                                                              new List<SmartUtility.UnitRates>(),
                                                                              new List<SmartUtility.UnitRates>()
                                                                            };
            List<SmartUtility.UnitRates>[] fallback_rates_found = new List<SmartUtility.UnitRates>[3]
            {
                new List<SmartUtility.UnitRates>(),
                new List<SmartUtility.UnitRates>(),
                new List<SmartUtility.UnitRates>()
            };


            // Only Utility Warehouse ever gets this many?
            //string[] time_limit = new string[3] { "", "", "" };
            utilityviewmodel.SC_UNITS = "";
            utilityviewmodel.SC_DESCRIPTION = "";

            // Only Utility Warehouse ever gets this many?
            //decimal[] units_limit = new decimal[3] { 0.0M, 0.0M, 0.0M };
            //string[] description_limit = new string[3] { "", "", "" };
            //decimal tier_remainder = 0.0M,
            //            units,
            //            total_monthly_costs = 0.0M,
            //            total_quarterly_costs = 0.0M,
            //            total_yearly_costs = 0.0M;
            short tier_count;// = 0;
            short tier_level;// = 0;
            utilityviewmodel.add_tier = false;

            
            // You have done ABSOLUTELY BRILLIANTLY so far Ray, that I
            // wouldn't worry too much about this ... some of this stuff is STUNNING!
            //#if Z!PRODUCTION
            try
            {
                if (Are_We_In(target_supplier_code,
                                        target_tariff_code,
                                        target_payment_plan,
                                        comparison_supplier_code,
                                        comparison_tariff_code,
                                        comparison_payment_plan))
                //#endif
                {
                    List<SmartUtility.ConditionsDates> conditions_dates_found = SmartSpikeUtilityV2017.Utility_Find_ConditionsDates(ourviewmodel,
                                                                    utilityviewmodel,
                                                                    comparison_supplier_code,
                                                                    comparison_tariff_code,
                                                                    displayed_resource_code,
                                                                    displayed_resource_type,
                                                                    version_code,
                                                                    payment_code);

                    // May eventually have Group Code = 0 to deal with ? Maybe but that doesn't matter
                    // Conditions_Dates MAY have a Group Code of 0, but that just means there are
                    // no Conditions_Groups for it and therfore no Conditions_Limits.
                    // We should ALWAYS find AT LEAST ONE COnditions_Dates so to find ZERO *IS* an error ...
                    if (conditions_dates_found.Count == 0)
                    {
                        utilityviewmodel.analyzeMessage = Build_Analyze_Message(comparison_brand_code,
                                                        comparison_supplier_code,
                                                        comparison_payment_plan,
                                                        displayed_resource_code,
                                                        displayed_resource_type,
                                                        "AnalyzeConditionsDates" +
                                                        SmartParametersV2016.space +
                                                        comparison_tariff_code.ToString() +
                                                        SmartParametersV2016.space +
                                                        payment_code.ToString(),
                                                        routine);
                        return false;     // Avoid race condition
                    }
                    else
                    {
                        
                        // ALWAYS USE THE LAST GROUPINGS DATE AS A YARDSTICK
                        DateTime prices_valid_from = withdrawn_date;

                        bool price_already_found = false;
                        List<SmartUtility.ConditionsLimits> restrict_usageList = new List<SmartUtility.ConditionsLimits>();
                        foreach (SmartUtility.ConditionsDates conditions_dates_row in conditions_dates_found)
                        {
                            if (Special_Date_Compare(price_already_found, prices_valid_from, conditions_dates_row.PRICES_VALID_FROM))
                            {
                                price_already_found = true;
                                prices_valid_from = conditions_dates_row.PRICES_VALID_FROM;

                                // New Rates apply with each Grouping
                                tier_count = conditions_dates_row.TIER_COUNT;
                                List<SmartUtility.ConditionsGroups> conditions_groups_found = SmartSpikeUtilityV2017.Utility_Find_ConditionsGroups(ourviewmodel,
                                                                    utilityviewmodel,
                                                                    comparison_supplier_code,
                                                                    comparison_tariff_code,
                                                                    displayed_resource_code,
                                                                    displayed_resource_type,
                                                                    version_code,
                                                                    payment_code,
                                                                    tier_count);
                                // If there are STILL No Groups found, then this ISN'T a catastrophe -
                                // it just means there are no subsequent Limits etc.
                                // therfore just 'carry on' and pick up the Tier 1 rates
                                if (conditions_groups_found.Count > 0)
                                {
                                    // and what's worse .. she actually PROUD of her stupidity?
                                    // She's not embarassed, or abashed or ashamed of her thickness,
                                    // she's actually PROUD that she's stupid!!
                                    // We sometimes - legitimately - get more than one Group found ...
                                    foreach (SmartUtility.ConditionsGroups conditions_groups_row in conditions_groups_found)
                                    {
                                        tier_level = Convert.ToInt16(conditions_groups_row.TIER_LEVEL);
                                        // But this can SURELY be speeded up as we only ever have ONE Limit
                                        // per Group Row ???
                                        List<SmartUtility.ConditionsLimits> conditions_limits_found = SmartSpikeUtilityV2017.Utility_Find_ConditionsLimits(ourviewmodel,
                                                                                                                                utilityviewmodel,
                                                                                                                                conditions_groups_row.SUPPLIER_CODE,
                                                                                                                                conditions_groups_row.RESOURCE_CODE,
                                                                                                                                conditions_groups_row.RESOURCE_TYPE,
                                                                                                                                conditions_groups_row.LIMIT_CODE);

                                        if (conditions_limits_found.Count > 0)
                                        {
                                            if (!Work_Through_Conditions_Limits(ourviewmodel,
                                                            utilityviewmodel,
                                                            conditions_limits_found,
                                                            comparison_brand_code,
                                                            comparison_supplier_code,
                                                            comparison_payment_plan,
                                                            displayed_resource_code,
                                                            displayed_resource_type,
                                                            conditions_groups_row,
                                                            restrict_usageList))
                                            {
                                                return false;
                                            }
                                        }
                                        else
                                        {
                                            // This IS a catastrophe because we said there would be
                                            // some Limits - and there aren't  =:&-(
                                            utilityviewmodel.analyzeMessage = Build_Analyze_Message(comparison_brand_code,
                                                                                    comparison_supplier_code,
                                                                                    comparison_payment_plan,
                                                                                    displayed_resource_code,
                                                                                    displayed_resource_type,
                                                                                    "AnalyzeLimitCode" +
                                                                                    SmartParametersV2016.space +
                                                                                    conditions_groups_row.LIMIT_CODE.ToString(),
                                                                                    routine);
                                            return false;
                                        }



                                        // Look up all the fields for this Area Id
                                        switch (displayed_resource_code)
                                        {
                                            case SmartParametersV2016.Electricity:
                                                rates_found[tier_level - 1] = SmartSpikeUtilityV2017.Utility_Find_UnitRates(ourviewmodel,
                                                     utilityviewmodel,
                                                     comparison_supplier_code,
                                                     displayed_resource_code,
                                                     displayed_resource_type,
                                                     comparison_tariff_code,
                                                     payment_code,
                                                     prices_valid_from,
                                                     tier_count,
                                                     tier_level,
                                                     utilityviewmodel.Hezbollah.e_unit_ratesList);
                                                if (rates_found[tier_level - 1].Count > 0)
                                                {
                                                    if (string.IsNullOrEmpty(utilityviewmodel.tcr))
                                                    {
                                                        utilityviewmodel.tcr = rates_found[tier_level - 1][0].TCR;
                                                    }
                                                }
                                                break;
                                            case SmartParametersV2016.Gas:
                                                rates_found[tier_level - 1] = SmartSpikeUtilityV2017.Utility_Find_UnitRates(ourviewmodel,
                                                    utilityviewmodel,
                                                    comparison_supplier_code,
                                                    displayed_resource_code,
                                                    displayed_resource_type,
                                                    comparison_tariff_code,
                                                    payment_code,
                                                    prices_valid_from,
                                                    tier_count,
                                                    tier_level,
                                                    utilityviewmodel.Hezbollah.g_unit_ratesList);
                                                if (rates_found[tier_level - 1].Count > 0)
                                                {
                                                    if (string.IsNullOrEmpty(utilityviewmodel.tcr))
                                                    {
                                                        utilityviewmodel.tcr = rates_found[tier_level - 1][0].TCR;
                                                    }
                                                }
                                                break;
                                            default:
                                                break;
                                        }
                                        // Find the Fallback tariff All Big 6 should have one of these
                                        if (fallback_tariff_code != 0)
                                        {
                                            // Find the Fallback rates just in case - for the time being these
                                            // Standard rates are assumed never to expire.....
                                            //List<SmartUtility.UnitRates> fallback_found = new List<SmartUtility.UnitRates>();
                                            switch (displayed_resource_code)
                                            {
                                                case SmartParametersV2016.Electricity:
                                                    fallback_rates_found[tier_level - 1] = SmartSpikeUtilityV2017.Utility_Find_UnitRates(ourviewmodel,
                                                                                                    utilityviewmodel,
                                                                                                    comparison_supplier_code,
                                                                                                    displayed_resource_code,
                                                                                                    displayed_resource_type,
                                                                                                    fallback_tariff_code,
                                                                                                    payment_code,
                                                                                                    prices_valid_from,
                                                                                                    tier_count,
                                                                                                    tier_level,
                                                                                                    utilityviewmodel.Hezbollah.e_unit_ratesList);
                                                    if (fallback_rates_found[tier_level - 1].Count > 0)
                                                    {
                                                        if (string.IsNullOrEmpty(utilityviewmodel.tcr))
                                                        {
                                                            utilityviewmodel.tcr = fallback_rates_found[tier_level - 1][0].TCR;
                                                        }
                                                    }
                                                    break;
                                                case SmartParametersV2016.Gas:
                                                    fallback_rates_found[tier_level - 1] = SmartSpikeUtilityV2017.Utility_Find_UnitRates(ourviewmodel,
                                                                                                    utilityviewmodel,
                                                                                                    comparison_supplier_code,
                                                                                                    displayed_resource_code,
                                                                                                    displayed_resource_type,
                                                                                                    fallback_tariff_code,
                                                                                                    payment_code,
                                                                                                    prices_valid_from,
                                                                                                    tier_count,
                                                                                                    tier_level,
                                                                                                    utilityviewmodel.Hezbollah.g_unit_ratesList);
                                                    if (fallback_rates_found[tier_level - 1].Count > 0)
                                                    {
                                                        if (string.IsNullOrEmpty(utilityviewmodel.tcr))
                                                        {
                                                            utilityviewmodel.tcr = fallback_rates_found[tier_level - 1][0].TCR;
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

                        // Did we find a price??
                        if (price_already_found)
                        {
                            // Um ... well ... if we have a 'K' plan
                            // (which has been already done with an 'A' plan)
                            // then we DON'T do the tariff_costsList when
                            // it is required for a Chart. Hmmm ... suppose
                            // if tariff_costsLists.Length is 0 then we MUST
                            // re-calculate them ... Hmmmm ....
                            utilityviewmodel.temp_decimal = 0.0M;
                            if (Check_Already_Done(utilityviewmodel,
                                            already_doneList,
                                            displayed_resource_code,
                                            displayed_resource_type,
                                            utilityviewmodel.SC_UNITS,
                                            utilityviewmodel.SC_DESCRIPTION,
                                            rates_found[0],
                                            rates_found[1],
                                            rates_found[2],
                                            fallback_rates_found[0],
                                            fallback_rates_found[1],
                                            fallback_rates_found[2],
                                            restrict_usageList) &&
                                            utilityviewmodel.Hezbollah.tariff_costsList.Count > 0)
                            {
                                utilityviewmodel.condition_costs = utilityviewmodel.temp_decimal;
                            }
                            else
                            {
                                if (!Price_Found(ourviewmodel,
                                                utilityviewmodel,
                                                wildcard,
                                                displayed_resource_code,
                                                displayed_resource_type,    // We may turn this on or off (still)
                                                comparison_brand_code,
                                                comparison_supplier_code,
                                                comparison_payment_plan,
                                                withdrawn_date,
                                                fallback_tariff_code,
                                                utilityviewmodel.SC_UNITS,
                                                utilityviewmodel.SC_DESCRIPTION,
                                                rates_found[0],
                                                rates_found[1],
                                                rates_found[2],
                                                fallback_rates_found[0],
                                                fallback_rates_found[1],
                                                fallback_rates_found[2],
                                                restrict_usageList))
                                {
                                    return false;
                                }
                                else
                                {
                                    // Add it in to already_doneList
                                    SmartUtility.AnalysisConditions abc = new SmartUtility.AnalysisConditions()
                                    {
                                        displayed_resource_code = displayed_resource_code,
                                        displayed_resource_type = displayed_resource_type,
                                        withdrawnDate = withdrawn_date,
                                        fallback_tariff_code = fallback_tariff_code,
                                        SC_UNITS = utilityviewmodel.SC_UNITS,
                                        SC_DESCRIPTION = utilityviewmodel.SC_DESCRIPTION,
                                        rates_found1 = rates_found[0],
                                        rates_found2 = rates_found[1],
                                        rates_found3 = rates_found[2],
                                        fallback_rates_found1 = fallback_rates_found[0],
                                        fallback_rates_found2 = fallback_rates_found[1],
                                        fallback_rates_found3 = fallback_rates_found[2],
                                        restrict_usageList = restrict_usageList,
                                        condition_costs = utilityviewmodel.condition_costs
                                    };
                                    already_doneList.Add(abc);
                                }
                            }
                        }
                    }
                }
                //#if Z!PRODUCTION
            }
            catch (ArgumentNullException exception)
            {
                utilityviewmodel.analyzeMessage = exception.Message;
                return false;
            }
            catch (FormatException exception)
            {
                utilityviewmodel.analyzeMessage = exception.Message;
                return false;
            }
            catch (ArgumentException exception)
            {
                utilityviewmodel.analyzeMessage = exception.Message;
                return false;
            }
            //#endif
            return true;
        }

        private static bool Are_We_In(short target_supplier_code,
                                        int target_tariff_code,
                                        char target_payment_plan,
                                        short comparison_supplier_code,
                                        int comparison_tariff_code,
                                        char comparison_payment_plan)
        {
            if (((target_supplier_code == 0) &&
                    (target_tariff_code == 0) &&
                    (target_payment_plan == SmartParametersV2016.defaultChar)) ||
                    ((comparison_supplier_code == target_supplier_code) &&
                    (comparison_tariff_code == target_tariff_code) &&
                    (comparison_payment_plan == target_payment_plan)))
            {
                return true;
            }
            return false;
        }

        private static bool Work_Through_Conditions_Limits(MainViewModel ourviewmodel,
                                                            UtilityViewModel utilityviewmodel,
                                                            List<SmartUtility.ConditionsLimits> conditions_limits_found,
                                                            short comparison_brand_code,
                                                            short comparison_supplier_code,
                                                            char comparison_payment_plan,
                                                            char displayed_resource_code,
                                                            string displayed_resource_type,
                                                            SmartUtility.ConditionsGroups conditions_groups_row,
                                                            List<SmartUtility.ConditionsLimits> restrict_usageList,
                                                            [System.Runtime.CompilerServices.CallerMemberName] string routine = "")
        {
            foreach (SmartUtility.ConditionsLimits conditions_limits_row in conditions_limits_found)
            {
                switch (conditions_limits_row.LIMIT_TYPE)
                {
                    case "BL": //"Band Limit":
                        utilityviewmodel.add_tier = true;
                        // Test for special code for different Area thresholds on Tier Bands
                        if ((conditions_limits_row.LOWER_LIMIT == "<AREA_CODE>") ||
                            (conditions_limits_row.UPPER_LIMIT == "<AREA_CODE>"))
                        {
                            List<SmartUtility.ConditionsAreas> conditions_areas_found = SmartSpikeUtilityV2017.Utility_Find_ConditionsAreas(ourviewmodel,
                                                                    utilityviewmodel,
                                                                    conditions_limits_row.SUPPLIER_CODE,
                                                                    conditions_limits_row.RESOURCE_CODE,
                                                                    conditions_limits_row.RESOURCE_TYPE,
                                                                    conditions_limits_row.LIMIT_CODE);


                            //(from Conditions_Limits_Area
                            //                                in Hezbollah.conditions_areasList
                            //                                            where ((Conditions_Limits_Area.SUPPLIER_CODE == conditions_limits_row.SUPPLIER_CODE) &&
                            //                                                   (Conditions_Limits_Area.RESOURCE_CODE == conditions_limits_row.RESOURCE_CODE) &&
                            //                                                   (Conditions_Limits_Area.RESOURCE_TYPE == conditions_limits_row.RESOURCE_TYPE) &&
                            //                                                   (Conditions_Limits_Area.LIMIT_CODE == conditions_limits_row.LIMIT_CODE) &&
                            //                                                   (Conditions_Limits_Area.AREA_CODE == displayed_area_code))
                            //                                            select Conditions_Limits_Area);
                            if (conditions_areas_found.Count == 0)
                            {
                                // Attempt to look for a '00' Area Id
                                conditions_areas_found = SmartSpikeUtilityV2017.Utility_Find_ConditionsAreas(ourviewmodel,
                                                                    utilityviewmodel,
                                                                    conditions_limits_row.SUPPLIER_CODE,
                                                                    conditions_limits_row.RESOURCE_CODE,
                                                                    conditions_limits_row.RESOURCE_TYPE,
                                                                    conditions_limits_row.LIMIT_CODE);


                                //(from Conditions_Limits_Area
                                //                                 in Hezbollah.conditions_areasList
                                //                          where ((Conditions_Limits_Area.SUPPLIER_CODE == conditions_limits_row.SUPPLIER_CODE) &&
                                //                                 (Conditions_Limits_Area.RESOURCE_CODE == conditions_limits_row.RESOURCE_CODE) &&
                                //                                 (Conditions_Limits_Area.RESOURCE_TYPE == conditions_limits_row.RESOURCE_TYPE) &&
                                //                                 (Conditions_Limits_Area.LIMIT_CODE == conditions_limits_row.LIMIT_CODE) &&
                                //                                 (Conditions_Limits_Area.AREA_CODE == 0))
                                //                          select Conditions_Limits_Area);
                            }
                            if (conditions_areas_found.Count > 0)
                            {
                                foreach (SmartUtility.ConditionsAreas conditions_areas_row in conditions_areas_found)
                                {
                                    // I am not 100% sure why I should do LOWER_LIMIT because I don't think I use
                                    // it anywhere?  But this seems intrinsicly wrong to leave it out, so I am
                                    // including it for completeness i.e. my gut-feeling tells me it should be included ...
                                    conditions_limits_row.LOWER_LIMIT = conditions_areas_row.LOWER_LIMIT;
                                    conditions_limits_row.UPPER_LIMIT = conditions_areas_row.UPPER_LIMIT;
                                    // Only do first!
                                    break;
                                }
                            }
                            else
                            {
                                // Default to ignore the limit
                                utilityviewmodel.analyzeMessage = Build_Analyze_Message(comparison_brand_code,
                                                                        comparison_supplier_code,
                                                                        comparison_payment_plan,
                                                                        displayed_resource_code,
                                                                        displayed_resource_type,
                                                                        "AnalyzeFixedArea",
                                                                        routine);
                                
                                return false; // Avoid race condition add_tier = false;
                            }
                        }
                        break;
                    case "TL": //"Time Limit":
                        utilityviewmodel.add_tier = true;
                        break;
                    case "SC": //"Standing Charge":
                               // Lets get this straight ... what is a Standing Charge?
                               // A Standing Charge is simple a flag to say 'add in the standing charge rate'
                               // whatever that is.  The only question is - WHEN does it get added in?
                               // It gets added in whenever the SC_UNITS (Day, Month, Quarter, Year) matches the
                               // flag in the Tariff Engine row. Its got NOTHING TO DO WITH Tiers and Tier Levels
                               // NOTHING!!! So it gets passed (as a flag along with Tariff Engine row) into
                               // DO_ROWS_FOUND.  It shouldn't be passed (and it isn't) into CONDITIONS_ROW_VALID.
                        utilityviewmodel.SC_UNITS = conditions_limits_row.UNITS;
                        utilityviewmodel.SC_DESCRIPTION = conditions_limits_row.LIMIT_NAME;
                        // No need to add this because as a Tier restriction
                        // its units are never compared ...
                        utilityviewmodel.add_tier = false;
                        break;
                    default:
                        utilityviewmodel.analyzeMessage = Build_Analyze_Message(comparison_brand_code,
                                                                comparison_supplier_code,
                                                                comparison_payment_plan,
                                                                displayed_resource_code,
                                                                displayed_resource_type,
                                                                "AnalyzeLimitType" +
                                                                SmartParametersV2016.space +
                                                                conditions_limits_row.LIMIT_TYPE.ToString(),
                                                                routine);
                        return false;
                }
                if (utilityviewmodel.add_tier)
                {
                    SmartUtility.ConditionsLimits restrict_usage_row = new SmartUtility.ConditionsLimits()
                    {
                        // Keys
                        SUPPLIER_CODE = conditions_limits_row.SUPPLIER_CODE,
                        RESOURCE_CODE = conditions_limits_row.RESOURCE_CODE,
                        RESOURCE_TYPE = conditions_limits_row.RESOURCE_TYPE,

                        LIMIT_CODE = conditions_limits_row.LIMIT_CODE,
                        LIMIT_TYPE = conditions_groups_row.TIER_LEVEL.ToString(),
                        // Nice to haves
                        // Yes ... But where do I ever use it? .. in the DESCRIPTION you tart
                        LIMIT_NAME = conditions_limits_row.LIMIT_NAME,
                        LOWER_SYMBOL = conditions_limits_row.LOWER_SYMBOL,
                        LOWER_LIMIT = conditions_limits_row.LOWER_LIMIT,
                        // Must haves
                        FIELD_NAME = conditions_limits_row.FIELD_NAME,
                        UPPER_SYMBOL = conditions_limits_row.UPPER_SYMBOL,
                        UPPER_LIMIT = conditions_limits_row.UPPER_LIMIT,
                        UNITS = conditions_limits_row.UNITS
                    };
                    restrict_usageList.Add(restrict_usage_row);
                }
            }
            return true;
        }

        private static bool Check_Already_Done(UtilityViewModel utilityviewmodel,
                                        List<SmartUtility.AnalysisConditions> already_doneList,
                                        char displayed_resource_code,
                                        string displayed_resource_type,
                                        string SC_UNITS,
                                        string SC_DESCRIPTION,
                                        List<SmartUtility.UnitRates> rates_found1,
                                        List<SmartUtility.UnitRates> rates_found2,
                                        List<SmartUtility.UnitRates> rates_found3,
                                        List<SmartUtility.UnitRates> fallback_rates_found1,
                                        List<SmartUtility.UnitRates> fallback_rates_found2,
                                        List<SmartUtility.UnitRates> fallback_rates_found3,
                                        List<SmartUtility.ConditionsLimits> restrict_usageList)
        {
            // We are far more likely to find a match going DOWN rather than UP
            // We don't check on supplier and brand because there are several brands who
            // floow the Big 6 i.e. Big 6 price adjustments are replicated by the smaller brands
            // so its worth checking EVERYTHING in the list!
            int total = already_doneList.Count;
            while (total > 0)
            {
                SmartUtility.AnalysisConditions ac_record = already_doneList[total - 1];
                if (displayed_resource_code == ac_record.displayed_resource_code &&
                    displayed_resource_type == ac_record.displayed_resource_type &&
                    SC_UNITS == ac_record.SC_UNITS &&
                    SC_DESCRIPTION == ac_record.SC_DESCRIPTION)
                {
                    if (!Check_Rates(rates_found1, ac_record.rates_found1))
                    {
                        return false;
                    }
                    if (!Check_Rates(rates_found2, ac_record.rates_found2))
                    {
                        return false;
                    }
                    if (!Check_Rates(rates_found3, ac_record.rates_found3))
                    {
                        return false;
                    }
                    if (!Check_Rates(fallback_rates_found1, ac_record.fallback_rates_found1))
                    {
                        return false;
                    }
                    if (!Check_Rates(fallback_rates_found2, ac_record.fallback_rates_found2))
                    {
                        return false;
                    }
                    if (!Check_Rates(fallback_rates_found3, ac_record.fallback_rates_found3))
                    {
                        return false;
                    }

                    if (restrict_usageList.Count == ac_record.restrict_usageList.Count)
                    {
                        utilityviewmodel.temp_decimal = ac_record.condition_costs;
                        return true;
                    }
                }
                --total;
            }
            return false;
        }

        private static bool Check_Rates(List<SmartUtility.UnitRates> rates_found,
                                        List<SmartUtility.UnitRates> ac_record_rates_found)
        {
            if (rates_found.Count == ac_record_rates_found.Count &&
                        rates_found.Count > 0)
            {
                for (int i = 0; i < rates_found.Count; i++)
                {
                    if (rates_found[i].PRICES_VALID_FROM != ac_record_rates_found[i].PRICES_VALID_FROM ||
                        rates_found[i].SC != ac_record_rates_found[i].SC ||
                        rates_found[i].DR != ac_record_rates_found[i].DR ||
                        rates_found[i].NR != ac_record_rates_found[i].NR ||
                        rates_found[i].TIER_COUNT != ac_record_rates_found[i].TIER_COUNT ||
                        rates_found[i].TIER_LEVEL != ac_record_rates_found[i].TIER_LEVEL)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        private static bool Price_Found(MainViewModel ourviewmodel,
                                        UtilityViewModel utilityviewmodel,
                                        bool wildcard,
                                        char displayed_resource_code,
                                        string displayed_resource_type,    // We may turn this on or off (still)
                                        short comparison_brand_code,
                                        short comparison_supplier_code,
                                        char comparison_payment_plan,
                                        DateTime withdrawn_date,
                                        int fallback_tariff_code,
                                        string SC_UNITS,
                                        string SC_DESCRIPTION,
                                        List<SmartUtility.UnitRates> rates_found1,
                                        List<SmartUtility.UnitRates> rates_found2,
                                        List<SmartUtility.UnitRates> rates_found3,
                                        List<SmartUtility.UnitRates> fallback_rates_found1,
                                        List<SmartUtility.UnitRates> fallback_rates_found2,
                                        List<SmartUtility.UnitRates> fallback_rates_found3,
                                        List<SmartUtility.ConditionsLimits> restrict_usageList,
                                        [System.Runtime.CompilerServices.CallerMemberName] string routine = "")
        {

            // Only Utility Warehouse ever gets this many?
            string[] time_limit = new string[3] { "", "", "" };
            decimal[] units_limit = new decimal[3] { 0.0M, 0.0M, 0.0M };

            utilityviewmodel.pricetierRemainder = 0m;
            utilityviewmodel.priceUnits = 0m;
            utilityviewmodel.pricebumpedIndex = false;
            utilityviewmodel.total_monthly_costs = 0m;
            utilityviewmodel.total_quarterly_costs = 0m;
            utilityviewmodel.total_yearly_costs = 0m;
            utilityviewmodel.doRowsFoundMessage = "";

            string[] description_limit = new string[3] { "", "", "" };
            short tier_count = 0,
                    tier_level;
            int row_count = 0;
            bool tier1_valid,
                        tier2_valid = false,
                        tier3_valid = false;

            //Yes
            foreach (SmartUtility.ConditionsLimits restrict_usage_row in restrict_usageList)
            {
                // This IS a catastrophe because have 'planned'
                // only these limits - and there are more of them  =:&-(
                // Bug! 6/2 = 3 which is NOT > 3 its EQUAL to 3 you TART!!
                // You need '>=' here not '>' you tosser
                if ((tier_count / 2) >= units_limit.Length)
                {
                    utilityviewmodel.analyzeMessage = Build_Analyze_Message(comparison_brand_code,
                                                            comparison_supplier_code,
                                                            comparison_payment_plan,
                                                            displayed_resource_code,
                                                            displayed_resource_type,
                                                            "AnalyzeTierCount" +
                                                            SmartParametersV2016.space +
                                                            (tier_count / 2).ToString(),
                                                            routine);
                    return false;
                }
                else
                {
                    switch (restrict_usage_row.FIELD_NAME)
                    {
                        case "UNITS_TOTAL":
                            units_limit[tier_count / 2] = SmartRoutinesV2018.ConvertDecimal(restrict_usage_row.UPPER_LIMIT);
                            description_limit[tier_count / 2] = restrict_usage_row.LIMIT_NAME;
                            break;
                        case "READ_DATE":
                            time_limit[tier_count / 2] = restrict_usage_row.UNITS;
                            break;
                        default:
                            utilityviewmodel.analyzeMessage = Build_Analyze_Message(comparison_brand_code,
                                                                    comparison_supplier_code,
                                                                    comparison_payment_plan,
                                                                    displayed_resource_code,
                                                                    displayed_resource_type,
                                                                    "AnalyzeFieldName" +
                                                                    SmartParametersV2016.space +
                                                                    restrict_usage_row.FIELD_NAME,
                                                                    routine);
                            return false;     // Avoid race condition
                    }
                    tier_count = (short)(tier_count + 1);
                }
            }

            tier_count = (short)((tier_count + 1) / 2);
            if (tier_count > 3)
            {
                tier_count = 3;
            }

            utilityviewmodel.priceUnits = 0.0M;
            // These rows are all built in Build Engine

            List<SmartUtility.TariffEngine> tariff_engineList = new List<SmartUtility.TariffEngine>();
            switch (displayed_resource_code)
            {
                case SmartParametersV2016.Electricity:
                    tariff_engineList = utilityviewmodel.Hezbollah.e_tariff_engineList;
                    break;
                case SmartParametersV2016.Gas:
                    tariff_engineList = utilityviewmodel.Hezbollah.g_tariff_engineList;
                    break;
                default:
                    break;
            }
            foreach (SmartUtility.TariffEngine tariff_engine_row in tariff_engineList)
            {
                // Store and clear these when necessary
                if (tariff_engine_row.LIMIT_MONTHS)
                {
                    // These costs are excluding VAT (which is correct)
                    tariff_engine_row.TOTAL_MONTHLY_COST = utilityviewmodel.total_monthly_costs;
                    utilityviewmodel.total_monthly_costs = 0.0M;
                }
                if (tariff_engine_row.LIMIT_QUARTERS)
                {
                    // These costs are excluding VAT (which is correct)
                    tariff_engine_row.TOTAL_QUARTERLY_COST = utilityviewmodel.total_quarterly_costs;
                    utilityviewmodel.total_quarterly_costs = 0.0M;
                }
                if (tariff_engine_row.LIMIT_YEARS)
                {
                    // These costs are excluding VAT (which is correct)
                    tariff_engine_row.TOTAL_YEARLY_COST = utilityviewmodel.total_yearly_costs;
                    utilityviewmodel.total_yearly_costs = 0.0M;
                }

                tier_level = 0;
                tier1_valid = Conditions_Row_Valid(utilityviewmodel,
                                                    tariff_engine_row,
                                                    time_limit[tier_level],
                                                    units_limit[tier_level]); // Can initalize it
                if (tier1_valid)
                {
                    tier2_valid = false;
                    tier3_valid = false;
                }
                else
                {
                    switch (tier_count)
                    {
                        case 1:
                            tier1_valid = true;
                            break;
                        case 2:
                        case 3:
                            tier_level = (short)(tier_level + 1);
                            tier2_valid = Conditions_Row_Valid(utilityviewmodel,
                                                                tariff_engine_row,
                                                                time_limit[tier_level],
                                                                units_limit[tier_level]);// Can initalize it
                            if (tier2_valid)
                            {
                                tier3_valid = false;
                            }
                            else
                            {
                                switch (tier_count)
                                {
                                    case 1:
                                    case 2:
                                        tier2_valid = true;
                                        break;
                                    case 3:
                                        tier_level = (short)(tier_level + 1);
                                        tier3_valid = Conditions_Row_Valid(utilityviewmodel,
                                                                            tariff_engine_row,
                                                                            time_limit[tier_level],
                                                                            units_limit[tier_level]);// Can initalize it
                                        if (!tier3_valid)
                                        {
                                            utilityviewmodel.analyzeMessage = Build_Analyze_Message(comparison_brand_code,
                                                                                    comparison_supplier_code,
                                                                                    comparison_payment_plan,
                                                                                    displayed_resource_code,
                                                                                    displayed_resource_type,
                                                                                    "AnalyzeTierThree" +
                                                                                    SmartParametersV2016.space +
                                                                                    tier3_valid.ToString(),
                                                                                    routine);
                                            return false;     // Avoid race condition
                                        }
                                        break;
                                    default:
                                        break;
                                }
                            }
                            break;
                        default:
                            utilityviewmodel.analyzeMessage = Build_Analyze_Message(comparison_brand_code,
                                                                    comparison_supplier_code,
                                                                    comparison_payment_plan,
                                                                    displayed_resource_code,
                                                                    displayed_resource_type,
                                                                    "AnalyzeTierCount" +
                                                                    SmartParametersV2016.space +
                                                                    tier_count.ToString(),
                                                                    routine);
                            return false;     // Avoid race condition
                    }
                }
                if (!tier1_valid && !tier2_valid && !tier3_valid)
                {
                    utilityviewmodel.analyzeMessage = Build_Analyze_Message(comparison_brand_code,
                                                            comparison_supplier_code,
                                                            comparison_payment_plan,
                                                            displayed_resource_code,
                                                            displayed_resource_type,
                                                            "AnalyzeTier" +
                                                            SmartParametersV2016.space +
                                                            tier1_valid.ToString() +
                                                            SmartParametersV2016.space +
                                                            tier2_valid.ToString() +
                                                            SmartParametersV2016.space +
                                                            tier3_valid.ToString(),
                                                            routine);
                    return false;     // Avoid race condition
                }
                else
                {
                    //if (displayed_brand_code != tariff_engine_row.BRAND_CODE)
                    //{
                    //    analyze_message = Build_Analyze_Message(comparison_brand_code,
                    //                                            comparison_supplier_code,
                    //                                            comparison_payment_plan,
                    //                                            displayed_resource_code,
                    //                                            displayed_resource_type,
                    //                                            "AnalyzeTariffEngine" +
                    //                                            SmartParametersV2016.space +
                    //                                            displayed_supplier_code.ToString() +
                    //                                            SmartParametersV2016.space +
                    //                                            tariff_engine_row.BRAND_CODE.ToString(),
                    //                                            routine);
                    //    return false;     // Avoid race condition
                    //}
                    DateTime read_date = tariff_engine_row.PERIOD_END;
                    if (!SmartParseV2016.Lookup_Vat_Rate(SmartParametersV2016.currentVatCode,
                                                        read_date,
                                                        read_date,
                                                        ourviewmodel.Blanche.vatRatesList,
                                                        utilityviewmodel))
                    {
                        utilityviewmodel.analyzeMessage = Build_Analyze_Message(comparison_brand_code,
                                                                comparison_supplier_code,
                                                                comparison_payment_plan,
                                                                displayed_resource_code,
                                                                displayed_resource_type,
                                                                "AnalyzeVatRates" +
                                                                SmartParametersV2016.space +
                                                                tariff_engine_row.PERIOD_END.ToString(),
                                                                routine);

                        return false;     // Avoid race condition
                    }
                    DoRowsFound(ourviewmodel,
                                    utilityviewmodel,
                                    wildcard,
                                    tier1_valid,
                                    tier2_valid,
                                    tier3_valid,
                                    withdrawn_date,
                                    displayed_resource_code,
                                    displayed_resource_type,
                                    tariff_engine_row,
                                    SC_UNITS,
                                    SC_DESCRIPTION,
                                    fallback_tariff_code,
                                    comparison_supplier_code,
                                    description_limit[tier_level],
                                    utilityviewmodel.VAT_RATE,
                                    rates_found1,
                                    rates_found2,
                                    rates_found3,
                                    fallback_rates_found1,
                                    fallback_rates_found2,
                                    fallback_rates_found3);
                    //rf utilityviewmodel.condition_costs,        // Can update it
                    //rf utilityviewmodel.total_monthly_costs,    // Can update it
                    //rf utilityviewmodel.total_quarterly_costs,  // Can update it
                    //rf utilityviewmodel.total_yearly_costs,     // Can update it
                    //rf utilityviewmodel.pricetierRemainder,     // Can set it to 0.0
                    //rf do_rows_found_messsage);
                    if (!string.IsNullOrEmpty(utilityviewmodel.doRowsFoundMessage))
                    {
                        utilityviewmodel.analyzeMessage = Build_Analyze_Message(comparison_brand_code,
                                                                comparison_supplier_code,
                                                                comparison_payment_plan,
                                                                displayed_resource_code,
                                                                displayed_resource_type,
                                                                utilityviewmodel.doRowsFoundMessage,
                                                                routine);
                        return false;
                    }
                }
                row_count++;
            }
            return true;
        }
        private static bool Conditions_Row_Valid(UtilityViewModel utilityviewmodel,
                                                SmartUtility.TariffEngine tariff_engine_row,
                                                string LIMIT_UNITS,
                                                // rf decimal units
                                                decimal units_limit)
        {
            bool inside_range = true;

            switch (LIMIT_UNITS)
            {
                case "Days":
                    if (tariff_engine_row.DAYS)
                    {
                        inside_range = true;
                        utilityviewmodel.priceUnits = 0.0M;
                        utilityviewmodel.pricebumpedIndex = false;
                    }
                    break;
                case "Months":
                    if (tariff_engine_row.LIMIT_MONTHS)
                    {
                        inside_range = true;
                        utilityviewmodel.priceUnits = 0.0M;
                        utilityviewmodel.pricebumpedIndex = false;
                    }
                    break;
                case "Quarters":
                    if (tariff_engine_row.LIMIT_QUARTERS)
                    {
                        inside_range = true;
                        utilityviewmodel.priceUnits = 0.0M;
                        utilityviewmodel.pricebumpedIndex = false;
                    }
                    break;
                case "Years":
                    if (tariff_engine_row.LIMIT_YEARS)
                    {
                        inside_range = true;
                        utilityviewmodel.priceUnits = 0.0M;
                        utilityviewmodel.pricebumpedIndex = false;
                    }
                    break;
                default:
                    break;
            }
            if (inside_range)
            {
                // Can probably speed this up slightly
                if (units_limit > 0)
                {
                    utilityviewmodel.priceUnits += tariff_engine_row.UNITS;
                    if (utilityviewmodel.priceUnits > units_limit)
                    {
                        // Beyond Limit
                        if (!utilityviewmodel.pricebumpedIndex)
                        {
                            // Excuse first transgression
                            utilityviewmodel.pricebumpedIndex = true;
                            utilityviewmodel.pricetierRemainder = utilityviewmodel.priceUnits - units_limit;
                        }
                        else
                        {
                            // But not all subsequent crimes
                            inside_range = false;
                        }
                    }
                }
            }
            return inside_range;
        }

        private static void DoRowsFound(MainViewModel ourviewmodel,
                                            UtilityViewModel utilityviewmodel,
                                            bool wildcard,
                                            bool tier1_valid,
                                            bool tier2_valid,
                                            bool tier3_valid,
                                            DateTime withdrawn_date,
                                            char displayed_resource_code,
                                            string displayed_resource_type,                // Use Economy7 rates for calculation
                                            SmartUtility.TariffEngine amelia,               // tariff_engine_row
                                            string SC_UNITS,
                                            string SC_DESCRIPTION,
                                            int fallback_tariff_code,
                                            short comparison_supplier_code,
                                            string description,
                                            decimal vat_rate,
                                            List<SmartUtility.UnitRates> rates_found1,
                                            List<SmartUtility.UnitRates> rates_found2,
                                            List<SmartUtility.UnitRates> rates_found3,
                                            List<SmartUtility.UnitRates> fallback_rates_found1,
                                            List<SmartUtility.UnitRates> fallback_rates_found2,
                                            List<SmartUtility.UnitRates> fallback_rates_found3)
        //rf decimal condition_costs,
        //rf decimal total_monthly_costs,
        //rf decimal total_quarterly_costs,
        //rf decimal total_yearly_costs,
        //rf decimal priceTierRemainder,
        //rf string do_rows_found_messsage)
        {
            // If WE are an Economy7 then we have day and night rates
            // and can compare against both Economy7 and non-Economy7
            //
            // If WE are NOT an Economy7 then we have no night rates
            // only 'day' rates so it only makes sense to compare against
            // non-Economy7 others like us
            // (because we don't know the day/night usage split)
            //
            // However from the calling routine we MAY be examing Economy7
            // Day and Night Units with a Non-Economy7 Day Rate so
            // ECONOMY7 must **ONLY** but used to test RATES not UNITS
            // in this routine

            DateTime read_date;

            utilityviewmodel.sc_code = "";
            utilityviewmodel.sc_units_days = 0;
            utilityviewmodel.sc_units_months = 0;
            utilityviewmodel.sc_units_quarters = 0;
            utilityviewmodel.sc_units_years = 0;

            decimal figure,// = 0.0M,
                        figure1,// = 0.0M,
                        figure2,// = 0.0M,
                        day_units,// = 0.0M,
                        night_units;// = 0.0M,

            utilityviewmodel.leftover_cost = 0m;
            utilityviewmodel.day_units_rate = 0m;
            utilityviewmodel.night_units_rate = 0m;
            utilityviewmodel.tariff_comparison_rate = 0m;
            utilityviewmodel.standing_charge1 = 0m;
            utilityviewmodel.standing_charge2 = 0m; // <= Fucking Utility Whorehouse cunts

            utilityviewmodel.doRowsFoundMessage = "";

            read_date = amelia.PERIOD_END;

            // Is the date less than the withdrawn date of the tariff?
            // Yes - find the rate (for this tariff) that applies for this date
            // No  - find the rate (for this fallback tariff) that applies for this date
            if ((SmartRoutinesV2018.DateTimeCompare(read_date, withdrawn_date) > 0) &&
                (fallback_tariff_code != 0))
            {
                Process_Fallback_Rates(utilityviewmodel,
                                        tier1_valid,
                                        tier2_valid,
                                        tier3_valid,
                                        fallback_rates_found1,
                                        fallback_rates_found2,
                                        fallback_rates_found3,
                                        read_date);
                //rf day_units_rate,
                //rf night_units_rate,
                //rf standing_charge1,
                //rf standing_charge2,
                //rf tariff_comparison_rate);

                //List<SmartUtility.UnitRates> fallback_rates_found = new List<SmartUtility.UnitRates>();
                //if (tier1_valid)
                //{
                //    fallback_rates_found = fallback_rates_found1;
                //}
                //if (tier2_valid)
                //{
                //    fallback_rates_found = fallback_rates_found2;
                //}
                //if (tier3_valid)
                //{
                //    fallback_rates_found = fallback_rates_found3;
                //}
                //// If we are out of time and we have a fallback, then use it
                //foreach (Unit_Rates fallback_rates_row in fallback_rates_found)
                //{
                //    valid_from = fallback_rates_row.PRICES_VALID_FROM;
                //    if (SmartRoutinesV2018.DateTimeCompare(read_date, valid_from) >= 0)
                //    {
                //        // Our Date is greater than the one from the table
                //        day_units_rate = ConvertDecimal(fallback_rates_row.DR, SmartParametersV2016.defaultCulture);
                //        // This needs sorting out - is there a DUAL FUEL Rate per se? Or is it a discount?
                //        // This WAS sorted we DID have a dual fuel rate, but not anymore
                //        night_units_rate = ConvertDecimal(fallback_rates_row.NR, SmartParametersV2016.defaultCulture);
                //        // Common
                //        if (fallback_rates_row.SC.IndexOf("+") >= 0)
                //        {
                //            standing_charge = fallback_rates_row.SC.Split('+');
                //            standing_charge1 = ConvertDecimal(standing_charge[0], SmartParametersV2016.defaultCulture);
                //            standing_charge2 = ConvertDecimal(standing_charge[1], SmartParametersV2016.defaultCulture);
                //        }
                //        else
                //        {
                //            standing_charge1 = ConvertDecimal(fallback_rates_row.SC, SmartParametersV2016.defaultCulture);
                //        }
                //        tariff_comparison_rate = ConvertDecimal(fallback_rates_row.TCR, SmartParametersV2016.defaultCulture);
                //    }
                //    else
                //    {
                //        // Our date is less than the one from the table
                //        break;
                //    }
                //}
            }
            else
            {
                Process_Normal_Rates(utilityviewmodel,
                                    tier1_valid,
                                    tier2_valid,
                                    tier3_valid,
                                    rates_found1,
                                    rates_found2,
                                    rates_found3,
                                    read_date);
                //rf day_units_rate,
                //rf night_units_rate,
                //rf standing_charge1,
                //rf standing_charge2,
                //rf tariff_comparison_rate);
                //List<SmartUtility.UnitRates> rates_found = new List<SmartUtility.UnitRates>();
                //if (tier1_valid)
                //{
                //    rates_found = rates_found1;
                //}
                //if (tier2_valid)
                //{
                //    rates_found = rates_found2;
                //}
                //if (tier3_valid)
                //{
                //    rates_found = rates_found3;
                //}

                //// We are not out of time OR there is no fallback, so ignore
                //// any Tariff out of validity date
                //// I suppose rates_found could well be 'empty' ....
                //foreach (Unit_Rates rates_row in rates_found)
                //{
                //    valid_from = rates_row.PRICES_VALID_FROM;
                //    if (SmartRoutinesV2018.DateTimeCompare(read_date, valid_from) >= 0)
                //    {
                //        // Our Date is greater than the one from the table
                //        day_units_rate = ConvertDecimal(rates_row.DR, SmartParametersV2016.defaultCulture);
                //        // Sort this out - we did have a dual_fuel_rate but not anymore
                //        // dual_fuel_rate = ConvertDecimal(rates_row.NR, SmartParametersV2016.defaultCulture);
                //        night_units_rate = ConvertDecimal(rates_row.NR, SmartParametersV2016.defaultCulture);

                //        // Common
                //        if (rates_row.SC.IndexOf("+") >= 0)
                //        {
                //            standing_charge = rates_row.SC.Split('+');
                //            standing_charge1 = ConvertDecimal(standing_charge[0], SmartParametersV2016.defaultCulture);
                //            standing_charge2 = ConvertDecimal(standing_charge[1], SmartParametersV2016.defaultCulture);
                //        }
                //        else
                //        {
                //            standing_charge1 = ConvertDecimal(rates_row.SC, SmartParametersV2016.defaultCulture);
                //        }
                //        tariff_comparison_rate = ConvertDecimal(rates_row.TCR, SmartParametersV2016.defaultCulture);
                //        // We have some prices .. that's all - we are OUT OF HERE?
                //        break;
                //    }
                //}
            }

            utilityviewmodel.sc_code = "";

            if ((utilityviewmodel.standing_charge1 != 0.0M) ||
                (utilityviewmodel.standing_charge2 != 0.0M))
            {
                utilityviewmodel.sc_units_days = 0;
                utilityviewmodel.sc_units_months = 0;
                utilityviewmodel.sc_units_quarters = 0;
                utilityviewmodel.sc_units_years = 0;
                Process_SC_Units(utilityviewmodel,
                                    SC_UNITS,
                                    amelia);
                //rf sc_units_years,
                //rf sc_units_quarters,
                //rf sc_units_months,
                //rf sc_units_days,
                //rf sc_code);

                //switch (SC_UNITS)
                //{
                //    case "Years":
                //        sc_units_years = Convert.ToByte(amelia.SC_YEARS);
                //        sc_code = "Y";
                //        break;
                //    case "Quarters":
                //        sc_units_quarters = Convert.ToByte(amelia.SC_QUARTERS);
                //        sc_code = "Q";
                //        break;
                //    case "Months":
                //        sc_units_months = Convert.ToByte(amelia.SC_MONTHS);
                //        sc_code = "M";
                //        break;
                //    default:
                //        // Days
                //        sc_units_days = Convert.ToByte(amelia.DAYS);
                //        sc_code = "D";
                //        break;
                //}

                if (!string.IsNullOrEmpty(utilityviewmodel.sc_code))
                {
                    int sc_total = utilityviewmodel.sc_units_days +
                                    utilityviewmodel.sc_units_months +
                                    utilityviewmodel.sc_units_quarters +
                                    utilityviewmodel.sc_units_years;
                    figure1 = sc_total * utilityviewmodel.standing_charge1;
                    if (!wildcard &&
                        (sc_total > 0))
                    {
                        // Somtimes (fucking Utility Whorehouse) you have to show a 0 Monthly charge
                        SmartUtility.TariffCosts tariffcosts = new SmartUtility.TariffCosts()
                        {
                            USERNAME = ourviewmodel.UserName,
                            CUBEFACE_CODE = SmartParametersV2016.Utility,
                            SUPPLIER_CODE = comparison_supplier_code,
                            ACCOUNT_NO = amelia.ACCOUNT_NO,
                            PERIOD_END = read_date,
                            CODE = "940",
                            UPPER_DATE = read_date,
                            ITEM_COUNT = 0,
                            D_UNITS = 0.0M,
                            D_UNITS_RATE = 0.0M,
                            N_UNITS = 0.0M,
                            N_UNITS_RATE = 0.0M,
                            STANDING_CHARGE = utilityviewmodel.standing_charge1,
                            //LEFTOVER = 0.0M,  // Don't care
                            FIGURE = figure1,
                            DESCRIPTION = SC_DESCRIPTION,
                            VAT_RATE = vat_rate //  Vat Rate on the day
                        };
                        utilityviewmodel.Hezbollah.tariff_costsList.Add(tariffcosts);
                    }
                    if (utilityviewmodel.standing_charge2 != 0.0M)
                    {
                        figure2 = sc_total * utilityviewmodel.standing_charge2;
                        if (!wildcard &&
                             (sc_total > 0))
                        {
                            SmartUtility.TariffCosts tariffcosts = new SmartUtility.TariffCosts()
                            {
                                USERNAME = ourviewmodel.UserName,
                                CUBEFACE_CODE = SmartParametersV2016.Utility,
                                SUPPLIER_CODE = comparison_supplier_code,
                                ACCOUNT_NO = amelia.ACCOUNT_NO,
                                PERIOD_END = read_date,
                                CODE = "941",
                                UPPER_DATE = read_date,
                                ITEM_COUNT = 0,
                                D_UNITS = 0.0M,
                                D_UNITS_RATE = 0.0M,
                                N_UNITS = 0.0M,
                                N_UNITS_RATE = 0.0M,
                                STANDING_CHARGE = utilityviewmodel.standing_charge2,
                                //LEFTOVER = 0.0M,  // Don't care
                                FIGURE = figure2,
                                DESCRIPTION = SC_DESCRIPTION,
                                VAT_RATE = vat_rate //  Vat Rate on the day
                            };
                            utilityviewmodel.Hezbollah.tariff_costsList.Add(tariffcosts);
                        }
                        figure1 += figure2;
                    }
                    figure = figure1 + (figure1 * vat_rate) / 100;
                    utilityviewmodel.condition_costs += figure;

                    utilityviewmodel.total_monthly_costs += figure;
                    utilityviewmodel.total_quarterly_costs += figure;
                    utilityviewmodel.total_yearly_costs += figure;
                    if (wildcard)
                    {
                        amelia.CHARGES_COST = figure;
                    }
                }
            }

            day_units = amelia.D_UNITS;
            night_units = amelia.N_UNITS;

            utilityviewmodel.leftover_cost = 0.0M;
            // Adjust the first one if there is any remainder
            if (utilityviewmodel.pricetierRemainder > 0.0M)
            {
                foreach (SmartUtility.UnitRates rates_row in rates_found2)
                {
                    //valid_from = rates_row.PRICES_VALID_FROM;
                    if (SmartRoutinesV2018.DateTimeCompare(read_date, rates_row.PRICES_VALID_FROM) >= 0)
                    {
                        // Our Date is greater than the one from the table
                        Process_Leftover_Cost(utilityviewmodel,
                                                displayed_resource_type,
                                                rates_row,
                                                utilityviewmodel.pricetierRemainder);

                        //switch (displayed_resource_type)
                        //{
                        //    case "SR":
                        //        leftover_cost = remainder * ConvertDecimal(rates_row.DR, SmartParametersV2016.defaultCulture); // Was day_units_rate for some fucking reason?
                        //        break;
                        //    case "VR":
                        //        // We do all split rates at the day rate, not the night rate
                        //        leftover_cost = remainder * ConvertDecimal(rates_row.DR, SmartParametersV2016.defaultCulture);// Was day_units_rate for some fucking reason?
                        //        //night_units_rate = ConvertDecimal(rates_row.NR, SmartParametersV2016.defaultCulture);
                        //        break;
                        //    default:
                        //        // Can never happen
                        //        break;
                        //}
                        if (!wildcard)
                        {
                            SmartUtility.TariffCosts tariffcosts = new SmartUtility.TariffCosts()
                            {
                                USERNAME = ourviewmodel.UserName,
                                CUBEFACE_CODE = SmartParametersV2016.Utility,
                                SUPPLIER_CODE = comparison_supplier_code,
                                ACCOUNT_NO = amelia.ACCOUNT_NO,
                                PERIOD_END = read_date,
                                CODE = "920",
                                UPPER_DATE = read_date,
                                ITEM_COUNT = 0,
                                D_UNITS = utilityviewmodel.pricetierRemainder,
                                D_UNITS_RATE = SmartRoutinesV2018.ConvertDecimal(rates_row.DR),
                                N_UNITS = 0.0M,
                                N_UNITS_RATE = 0.0M,
                                STANDING_CHARGE = 0.0M,
                                //LEFTOVER = 0.0M,          // Don't care
                                FIGURE = utilityviewmodel.leftover_cost,
                                DESCRIPTION = "", // No description for 'Normal' Units @ Day Rate
                                VAT_RATE = vat_rate         //  Vat Rate on the day
                            };
                            utilityviewmodel.Hezbollah.tariff_costsList.Add(tariffcosts);
                        }
                        // We have a price (we hope) for the remainder - that's it!
                        break;
                    }
                    if (utilityviewmodel.leftover_cost == 0.0M)
                    {
                        // Trap no rates for leftovers
                        utilityviewmodel.doRowsFoundMessage = "No rates found for: " + utilityviewmodel.pricetierRemainder;
                    }
                }

                // Now we have a remainder to 'dispose' of ...
                // Theoretically we should apportion it to both the
                // Day Units AND the Night Units (for Economy7 meters).
                // However (for Economy7 Meters) we reduce the Night Units first
                // because the Night Rate is always (?) cheaper
                // Then for the sake of simplicity; we are going to let the Day
                // Units have any remainder left because the Day Rate is higher
                // and we want to err on the side of caution.

                // Here we DON'T check the ECONOMY7 flag because we
                // are comparing UNITS and not RATES
                // if there are any night units (which will be zero for Non-E7
                // anyway and may (possibly) be zero for E7 (!!) then adjust them accordingly
                // So we just TEST for night_units (its enough!)
                if (night_units > 0.0M)
                {
                    if (utilityviewmodel.pricetierRemainder >= night_units)
                    {
                        utilityviewmodel.pricetierRemainder -= night_units;
                        night_units = 0.0M;
                    }
                    else
                    {
                        night_units -= utilityviewmodel.pricetierRemainder;
                        utilityviewmodel.pricetierRemainder = 0.0M;
                    }
                }
                // Now reduce the Day Units with what's left
                day_units -= utilityviewmodel.pricetierRemainder;
                if (day_units < 0.0M)
                {
                    // In THEORY, this should never happen ...
                    utilityviewmodel.doRowsFoundMessage = "Day Units set to 0.0: " + day_units.ToString();
                    day_units = 0.0M;
                }
                utilityviewmodel.pricetierRemainder = 0.0M;
            }

            if (utilityviewmodel.night_units_rate == 0.0M)
            {
                // The one we are comparing against is not Economy7
                // Treat all units at the Day Rate
                figure = utilityviewmodel.leftover_cost + (day_units + night_units) * utilityviewmodel.day_units_rate;
                if (!wildcard)
                {
                    if (string.IsNullOrEmpty(description))
                    {
                        switch (displayed_resource_code)
                        {
                            case SmartParametersV2016.Electricity:
                                description = "Electricity units";
                                break;
                            case SmartParametersV2016.Gas:
                                description = "Gas units";
                                break;
                            default:
                                break;
                        }
                    }
                    SmartUtility.TariffCosts tariffcosts = new SmartUtility.TariffCosts()
                    {
                        USERNAME = ourviewmodel.UserName,
                        CUBEFACE_CODE = SmartParametersV2016.Utility,
                        SUPPLIER_CODE = comparison_supplier_code,
                        ACCOUNT_NO = amelia.ACCOUNT_NO,
                        PERIOD_END = read_date,
                        CODE = "910",
                        UPPER_DATE = read_date,
                        ITEM_COUNT = 0,
                        D_UNITS = day_units + night_units,
                        D_UNITS_RATE = utilityviewmodel.day_units_rate,
                        N_UNITS = 0.0M,
                        N_UNITS_RATE = 0.0M,
                        STANDING_CHARGE = 0.0M,
                        //LEFTOVER = 0.0M,          // Don't care
                        FIGURE = figure,
                        DESCRIPTION = description,
                        VAT_RATE = vat_rate         //  Vat Rate on the day
                    };
                    utilityviewmodel.Hezbollah.tariff_costsList.Add(tariffcosts);
                }
            }
            else
            {
                // We are comparing against an Economy7 so it has a night rate
                figure = utilityviewmodel.leftover_cost +
                                        (day_units * utilityviewmodel.day_units_rate) +
                                        (night_units * utilityviewmodel.night_units_rate);
                // ALL RATES MUST NOT INCLUDE VAT - because its added on HERE
                // This is because VAT rates could change! We need to know what the VAT rate is!
                if (!wildcard)
                {
                    SmartUtility.TariffCosts tariffcosts = new SmartUtility.TariffCosts()
                    {
                        USERNAME = ourviewmodel.UserName,
                        CUBEFACE_CODE = SmartParametersV2016.Utility,
                        SUPPLIER_CODE = comparison_supplier_code,
                        ACCOUNT_NO = amelia.ACCOUNT_NO,
                        PERIOD_END = read_date,
                        CODE = "930",
                        UPPER_DATE = read_date,
                        ITEM_COUNT = 0,
                        D_UNITS = day_units,
                        D_UNITS_RATE = utilityviewmodel.day_units_rate,
                        N_UNITS = night_units,
                        N_UNITS_RATE = utilityviewmodel.night_units_rate,
                        STANDING_CHARGE = 0.0M,
                        //LEFTOVER = 0.0M,          // Don't care
                        FIGURE = figure,
                        DESCRIPTION = description,
                        VAT_RATE = vat_rate         //  Vat Rate on the day
                    };
                    utilityviewmodel.Hezbollah.tariff_costsList.Add(tariffcosts);
                }
            }
            // This is because VAT rates could change! We need to know what the VAT rate is!
            figure += (figure * vat_rate) / 100;
            utilityviewmodel.condition_costs += figure;

            utilityviewmodel.total_monthly_costs += figure;
            utilityviewmodel.total_quarterly_costs += figure;
            utilityviewmodel.total_yearly_costs += figure;
            if (wildcard)
            {
                amelia.UNITS_COST = figure;
            }
            return;
        }

        private static void Process_Leftover_Cost(UtilityViewModel utilityviewmodel,
                                                    string displayed_resource_type,
                                                    SmartUtility.UnitRates rates_row,
                                                    decimal remainder)
        {
            // Our Date is greater than the one from the table
            switch (displayed_resource_type)
            {
                case "SR":
                    utilityviewmodel.leftover_cost = remainder * SmartRoutinesV2018.ConvertDecimal(rates_row.DR); // Was day_units_rate for some fucking reason?
                    break;
                case "VR":
                    // We do all split rates at the day rate, not the night rate
                    utilityviewmodel.leftover_cost = remainder * SmartRoutinesV2018.ConvertDecimal(rates_row.DR);// Was day_units_rate for some fucking reason?
                                                                                                                 //night_units_rate = ConvertDecimal(rates_row.NR, SmartParametersV2016.defaultCulture);
                    break;
                default:
                    // Can never happen
                    break;
            }
            return;
        }
        private static void Process_SC_Units(UtilityViewModel utilityviewmodel,
                                            string SC_UNITS,
                                            SmartUtility.TariffEngine amelia)
        //rf int sc_units_years,
        //rf int sc_units_quarters,
        //rf int sc_units_months,
        //rf int sc_units_days,
        //rf string sc_code)
        {
            switch (SC_UNITS)
            {
                case "Years":
                    utilityviewmodel.sc_units_years = Convert.ToByte(amelia.SC_YEARS);
                    utilityviewmodel.sc_code = "Y";
                    break;
                case "Quarters":
                    utilityviewmodel.sc_units_quarters = Convert.ToByte(amelia.SC_QUARTERS);
                    utilityviewmodel.sc_code = "Q";
                    break;
                case "Months":
                    utilityviewmodel.sc_units_months = Convert.ToByte(amelia.SC_MONTHS);
                    utilityviewmodel.sc_code = "M";
                    break;
                default:
                    // Days
                    utilityviewmodel.sc_units_days = Convert.ToByte(amelia.DAYS);
                    utilityviewmodel.sc_code = "D";
                    break;
            }
            return;
        }
        private static void Process_Normal_Rates(UtilityViewModel utilityviewmodel,
                                                    bool tier1_valid,
                                                    bool tier2_valid,
                                                    bool tier3_valid,
                                                    List<SmartUtility.UnitRates> rates_found1,
                                                    List<SmartUtility.UnitRates> rates_found2,
                                                    List<SmartUtility.UnitRates> rates_found3,
                                                    DateTime read_date)
        //rf decimal day_units_rate,
        //rf decimal night_units_rate,
        //rf decimal standing_charge1,
        //rf decimal standing_charge2,
        //rf decimal tariff_comparison_rate)
        {
            List<SmartUtility.UnitRates> rates_found = new List<SmartUtility.UnitRates>();
            if (tier1_valid)
            {
                rates_found = rates_found1;
            }
            if (tier2_valid)
            {
                rates_found = rates_found2;
            }
            if (tier3_valid)
            {
                rates_found = rates_found3;
            }

            // We are not out of time OR there is no fallback, so ignore
            // any Tariff out of validity date
            // I suppose rates_found could well be 'empty' ....
            foreach (SmartUtility.UnitRates rates_row in rates_found)
            {
                if (SmartRoutinesV2018.DateTimeCompare(read_date, rates_row.PRICES_VALID_FROM) >= 0)
                {
                    // Our Date is greater than the one from the table
                    utilityviewmodel.day_units_rate = SmartRoutinesV2018.ConvertDecimal(rates_row.DR);
                    // Sort this out - we did have a dual_fuel_rate but not anymore
                    // dual_fuel_rate = SmartRoutinesV2018.ConvertDecimal(rates_row.NR);
                    utilityviewmodel.night_units_rate = SmartRoutinesV2018.ConvertDecimal(rates_row.NR);

                    // Common
                    if (rates_row.SC.IndexOf("+") >= 0)
                    {
                        string[] standing_charge = rates_row.SC.Split('+');
                        utilityviewmodel.standing_charge1 = SmartRoutinesV2018.ConvertDecimal(standing_charge[0]);
                        utilityviewmodel.standing_charge2 = SmartRoutinesV2018.ConvertDecimal(standing_charge[1]);
                    }
                    else
                    {
                        utilityviewmodel.standing_charge1 = SmartRoutinesV2018.ConvertDecimal(rates_row.SC);
                    }
                    utilityviewmodel.tariff_comparison_rate = SmartRoutinesV2018.ConvertDecimal(rates_row.TCR);
                    // We have some prices .. that's all - we are OUT OF HERE?
                    break;
                }
            }
            return;
        }
        private static void Process_Fallback_Rates(UtilityViewModel utilityviewmodel,
                                                    bool tier1_valid,
                                                    bool tier2_valid,
                                                    bool tier3_valid,
                                                    List<SmartUtility.UnitRates> fallback_rates_found1,
                                                    List<SmartUtility.UnitRates> fallback_rates_found2,
                                                    List<SmartUtility.UnitRates> fallback_rates_found3,
                                                    DateTime read_date)
        //rf decimal day_units_rate,
        //rf decimal night_units_rate,
        //rf decimal standing_charge1,
        //rf decimal standing_charge2,
        //rf decimal tariff_comparison_rate)
        {
            List<SmartUtility.UnitRates> fallback_rates_found = new List<SmartUtility.UnitRates>();
            if (tier1_valid)
            {
                fallback_rates_found = fallback_rates_found1;
            }
            if (tier2_valid)
            {
                fallback_rates_found = fallback_rates_found2;
            }
            if (tier3_valid)
            {
                fallback_rates_found = fallback_rates_found3;
            }
            // If we are out of time and we have a fallback, then use it
            foreach (SmartUtility.UnitRates fallback_rates_row in fallback_rates_found)
            {
                if (SmartRoutinesV2018.DateTimeCompare(read_date, fallback_rates_row.PRICES_VALID_FROM) >= 0)
                {
                    // Our Date is greater than the one from the table
                    utilityviewmodel.day_units_rate = SmartRoutinesV2018.ConvertDecimal(fallback_rates_row.DR);
                    // This needs sorting out - is there a DUAL FUEL Rate per se? Or is it a discount?
                    // This WAS sorted we DID have a dual fuel rate, but not anymore
                    utilityviewmodel.night_units_rate = SmartRoutinesV2018.ConvertDecimal(fallback_rates_row.NR);
                    // Common
                    if (fallback_rates_row.SC.IndexOf("+") >= 0)
                    {
                        string[] standing_charge = fallback_rates_row.SC.Split('+');
                        utilityviewmodel.standing_charge1 = SmartRoutinesV2018.ConvertDecimal(standing_charge[0]);
                        utilityviewmodel.standing_charge2 = SmartRoutinesV2018.ConvertDecimal(standing_charge[1]);
                    }
                    else
                    {
                        utilityviewmodel.standing_charge1 = SmartRoutinesV2018.ConvertDecimal(fallback_rates_row.SC);
                    }
                    utilityviewmodel.tariff_comparison_rate = SmartRoutinesV2018.ConvertDecimal(fallback_rates_row.TCR);
                }
                else
                {
                    // Our date is less than the one from the table
                    break;
                }
            }
            return;
        }

        private static bool Process_Post_Conditions(MainViewModel ourviewmodel,
                                                    UtilityViewModel utilityviewmodel,
                                                    bool wildcard,
                                                    char displayed_resource_code,
                                                    string displayed_resource_type,
                                                    short comparison_brand_code,
                                                    short comparison_supplier_code,
                                                    char comparison_payment_plan,
                                                    short selection_code)
        {
            string SELECTION_NAME = "",
                        FIELD_NAME = "",
                        CHARGE = "",
                        MAXIMUM_DISCOUNT = "",
                        ONCE_ONLY = "";

            bool tier1_valid;

            utilityviewmodel.time_limit = "";
            utilityviewmodel.dunits_limit = 0m;

            utilityviewmodel.post_period_costs = 0m;
            utilityviewmodel.units = 0m;
            utilityviewmodel.bumped_index = false;
            utilityviewmodel.tier1_remainder = 0m;

            utilityviewmodel.discount = 0m;
            utilityviewmodel.vat_rate = 0m;

            // You have done ABSOLUTELY BRILLIANTLY so far Ray, that I
            // wouldn't worry too much about this ..
            List<SmartUtility.Post_Conditions> post_conditions_found = SmartSpikeUtilityV2017.Utility_Find_PostConditions(ourviewmodel,
                                                                utilityviewmodel,
                                                                comparison_supplier_code,
                                                                displayed_resource_code,
                                                                selection_code);
            //(from Post_Condition
            //                                            in Hezbollah.post_conditionsList
            //                                           where ((Post_Condition.SUPPLIER_CODE == comparison_supplier_code) &&
            //                                                   (Post_Condition.RESOURCE_CODE == displayed_resource_code) &&
            //                                                   (Post_Condition.SELECTION_CODE == selection_code))
            //                                           select Post_Condition);
            if (post_conditions_found.Count > 0)
            {
                foreach (SmartUtility.Post_Conditions post_conditions_row in post_conditions_found)
                {
                    SELECTION_NAME = post_conditions_row.SELECTION_NAME;
                    FIELD_NAME = post_conditions_row.FIELD_NAME;
                    CHARGE = post_conditions_row.PRICE_EXC_VAT;
                    MAXIMUM_DISCOUNT = post_conditions_row.MAXIMUM_DISCOUNT;
                    ONCE_ONLY = post_conditions_row.ONCE_ONLY;
                    break;
                }
                List<SmartUtility.Post_Limits> restrict_usageList = new List<SmartUtility.Post_Limits>();
                //foreach (Post_Conditions post_conditions_row in post_conditions_found)
                //{

                if (!Post_Select_Shit(ourviewmodel,
                                        utilityviewmodel,
                                        displayed_resource_code,
                                        displayed_resource_type,
                                        comparison_brand_code,
                                        comparison_supplier_code,
                                        comparison_payment_plan,
                                        selection_code,
                                        restrict_usageList))
                {
                    return false;
                }

                if (!Post_Limits_Shit(utilityviewmodel,
                                        displayed_resource_code,
                                        displayed_resource_type,
                                        comparison_brand_code,
                                        comparison_supplier_code,
                                        comparison_payment_plan,
                                        restrict_usageList))
                {
                    return false;
                }

                // Post Conditions
                utilityviewmodel.units = 0.0M;
                List<SmartUtility.TariffEngine> tariff_engineList = new List<SmartUtility.TariffEngine>();
                switch (displayed_resource_code)
                {
                    case SmartParametersV2016.Electricity:
                        tariff_engineList = utilityviewmodel.Hezbollah.e_tariff_engineList;
                        break;
                    case SmartParametersV2016.Gas:
                        tariff_engineList = utilityviewmodel.Hezbollah.g_tariff_engineList;
                        break;
                    default:
                        break;
                }
                // These are built in Build Engine
                foreach (SmartUtility.TariffEngine tariff_engine_row in tariff_engineList)
                {
                    tier1_valid =
                        Post_Conditions_Row_Valid(utilityviewmodel,
                                                tariff_engine_row,
                                                FIELD_NAME,
                                                utilityviewmodel.time_limit);
                    //rf post_period_costs,
                    //rf bumped_index, // Not sure we need this
                    //rf units,
                    //rf dunits_limit,
                    //rf tier1_remainder);
                    if (tier1_valid)
                    {
                        utilityviewmodel.discount = 0.0M;
                        utilityviewmodel.vat_rate = 0.0M;
                        if (!Update_Discounts(ourviewmodel,
                                                utilityviewmodel,
                                                wildcard,
                                                comparison_brand_code,
                                                comparison_supplier_code,
                                                comparison_payment_plan,
                                                displayed_resource_code,
                                                displayed_resource_type,
                                                ONCE_ONLY,
                                                CHARGE,
                                                SELECTION_NAME,
                                                FIELD_NAME,
                                                MAXIMUM_DISCOUNT,
                                                selection_code,
                                                utilityviewmodel.tier1_remainder,
                                                tariff_engine_row))
                        //rf post_period_costs,
                        //rf discount,
                        //rf vat_rate))
                        {
                            return false;
                        }
                        else
                        {
                            utilityviewmodel.post_condition_costs += utilityviewmodel.discount;
                            utilityviewmodel.post_condition_costs += (utilityviewmodel.discount * utilityviewmodel.vat_rate) / 100;
                        }
                    }
                }
                //}
            }
            return true;
        }

        internal static bool Post_Select_Shit(MainViewModel ourviewmodel,
                                                UtilityViewModel utilityviewmodel,
                                                char displayed_resource_code,
                                                string displayed_resource_type,
                                                short comparison_brand_code,
                                                short comparison_supplier_code,
                                                char comparison_payment_plan,
                                                short selection_code,
                                                List<SmartUtility.Post_Limits> restrict_usageList,
                                                [System.Runtime.CompilerServices.CallerMemberName] string routine = "")
        {
            List<SmartUtility.Post_Select> post_select_found = SmartSpikeUtilityV2017.Utility_Find_PostSelect(ourviewmodel,
                                                                                    utilityviewmodel,
                                                                                    comparison_supplier_code,
                                                                                    selection_code);
            //(from Post_Conditions_Select_Group
            //                    in Hezbollah.post_selectList
            //                where ((Post_Conditions_Select_Group.SUPPLIER_CODE == comparison_supplier_code) &&
            //                    (Post_Conditions_Select_Group.SELECTION_CODE == selection_code))
            //                select Post_Conditions_Select_Group);
            if (post_select_found.Count > 0)
            {
                foreach (SmartUtility.Post_Select post_select_row in post_select_found)
                {
                    SmartUtility.Post_Limits restrict_usage = new SmartUtility.Post_Limits();
                    List<SmartUtility.Post_Limits> post_limits_found = SmartSpikeUtilityV2017.Utility_Find_PostLimits(ourviewmodel,
                                                                utilityviewmodel,
                                                                comparison_supplier_code,
                                                                post_select_row.LIMIT_CODE);
                    //(from Post_Limit
                    //                                    in Hezbollah.post_limitsList
                    //                                   where ((Post_Limit.SUPPLIER_CODE == comparison_supplier_code) &&
                    //                                          (Post_Limit.LIMIT_CODE == post_select_row.LIMIT_CODE))
                    //                                   select Post_Limit);
                    if (post_limits_found.Count > 0)
                    {
                        foreach (SmartUtility.Post_Limits post_limits_row in post_limits_found)
                        {
                            switch (post_limits_row.LIMIT_TYPE)
                            {
                                case "BL": //"Band Limit"
                                case "TL": //"Time Limit" 
                                    restrict_usage = post_limits_row;
                                    restrict_usageList.Add(restrict_usage);
                                    break;
                                case "TF": //"Dual Fuel Flag/Online Flag"
                                case "TP": //"Tariff Plan"
                                    break;
                                default:
                                    // Anything else?
                                    utilityviewmodel.analyzeMessage = Build_Analyze_Message(comparison_brand_code,
                                                                        comparison_supplier_code,
                                                                        comparison_payment_plan,
                                                                        displayed_resource_code,
                                                                        displayed_resource_type,
                                                                        "AnalyzeLimitType" +
                                                                        SmartParametersV2016.space +
                                                                        post_limits_row.LIMIT_TYPE,
                                                                        routine);
                                    return false;     // Avoid race condition
                            }
                        }
                    }
                }
            }
            return true;
        }

        internal static bool Post_Limits_Shit(UtilityViewModel utilityviewmodel,
                                                char displayed_resource_code,
                                                string displayed_resource_type,
                                                short comparison_brand_code,
                                                short comparison_supplier_code,
                                                char comparison_payment_plan,
                                                List<SmartUtility.Post_Limits> restrict_usageList,
                                                [System.Runtime.CompilerServices.CallerMemberName] string routine = "")
        {
            foreach (SmartUtility.Post_Limits restrict_usage_row in restrict_usageList)
            {
                switch (restrict_usage_row.FIELD_NAME)
                {
                    case "UNITS_TOTAL":
                        utilityviewmodel.dunits_limit = SmartRoutinesV2018.ConvertDecimal(restrict_usage_row.UPPER_LIMIT);
                        break;
                    case "READ_DATE":
                        utilityviewmodel.time_limit = restrict_usage_row.UNITS;
                        break;
                    default:
                        
                        utilityviewmodel.analyzeMessage = Build_Analyze_Message(comparison_brand_code,
                                                                comparison_supplier_code,
                                                                comparison_payment_plan,
                                                                displayed_resource_code,
                                                                displayed_resource_type,
                                                                "AnalyzePostFieldName" +
                                                                SmartParametersV2016.space +
                                                                restrict_usage_row.FIELD_NAME,
                                                                routine);
                        return false;     // Avoid race condition
                }
            }
            return true;
        }

        private static bool Post_Conditions_Row_Valid(UtilityViewModel utilityviewmodel,
                                                        SmartUtility.TariffEngine tariff_engine_row,
                                                        string FIELD_NAME,
                                                        string LIMIT_UNITS)
        //rf decimal post_period_costs,
        //rf bool bumped_index,
        //rf decimal units,
        //rf decimal dunits_limit,
        //rf decimal tier1_remainder)
        {
            // Assume failure on both tests
            bool inside_range1 = false,
                 inside_range2;// = false;

            switch (LIMIT_UNITS)
            {
                case "Days":
                    if (tariff_engine_row.DAYS)
                    {
                        inside_range1 = true;
                    }
                    break;
                case "Months":
                    if (tariff_engine_row.LIMIT_MONTHS)
                    {
                        inside_range1 = true;
                        utilityviewmodel.post_period_costs = tariff_engine_row.TOTAL_MONTHLY_COST;
                    }
                    break;
                case "Quarters":
                    if (tariff_engine_row.LIMIT_QUARTERS)
                    {
                        inside_range1 = true;
                        utilityviewmodel.post_period_costs = tariff_engine_row.TOTAL_QUARTERLY_COST;
                    }
                    break;
                case "Years":
                    if (tariff_engine_row.LIMIT_YEARS)
                    {
                        inside_range1 = true;
                        utilityviewmodel.post_period_costs = tariff_engine_row.TOTAL_YEARLY_COST;
                    }
                    break;
                default:
                    break;
            }

            // Here - we have either hit a Time Limit and got a TRUE or not and got a FALSE
            // Do we have a limit range to consider as well whether we got either?
            if (utilityviewmodel.dunits_limit > 0.0M)
            {
                // If inside range has been set TRUE by a Time Limit, then clear these down
                if (inside_range1)
                {
                    utilityviewmodel.units = 0.0M;
                    utilityviewmodel.bumped_index = false;
                }
                if (utilityviewmodel.units + tariff_engine_row.UNITS > utilityviewmodel.dunits_limit)
                {
                    // Are we beyond the Units Limit?
                    if (!utilityviewmodel.bumped_index)
                    {
                        // Yes - excuse only the first transgression
                        utilityviewmodel.bumped_index = true;
                        utilityviewmodel.tier1_remainder = utilityviewmodel.dunits_limit - utilityviewmodel.units;
                        inside_range2 = true;
                        // This is a kludge
                        if (FIELD_NAME == "TOTAL_UNITS")
                        {
                            inside_range1 = true;
                        }
                    }
                    else
                    {
                        // No - reject all subsequent crimes
                        inside_range2 = false;
                    }
                }
                else
                {
                    utilityviewmodel.units += tariff_engine_row.UNITS;
                    // Its within range
                    inside_range2 = true;
                    // This is a kludge
                    if (FIELD_NAME == "TOTAL_UNITS")
                    {
                        inside_range1 = true;
                    }
                }
            }
            else
            {
                inside_range2 = true;
            }
            return inside_range1 && inside_range2;
        }

        private static bool Update_Discounts(MainViewModel ourviewmodel,
                                                UtilityViewModel utilityviewmodel,
                                                bool wildcard,
                                                 short brand_code,
                                                 short supplier_code,
                                                 char payment_plan,
                                                 char displayed_resource_code,
                                                 string displayed_resource_type,
                                                 string ONCE_ONLY,
                                                 string CHARGE,
                                                 string SELECTION_NAME,
                                                 string FIELD_NAME,
                                                 string MAXIMUM_DISCOUNT,
                                                 short selection_code,
                                                 decimal tier1_remainder,
                                                 SmartUtility.TariffEngine tariff_engine_row,
                                                 //rf decimal post_period_costs,
                                                 //rf decimal discount,
                                                 //rf decimal vat_rate,
                                                 [System.Runtime.CompilerServices.CallerMemberName] string routine = "")
        {
            DateTime discount_date;
            string description = "";
            decimal units,
                        amount,
                        maximum,
                        percentage;

            if (ONCE_ONLY == "Y")
            {
                utilityviewmodel.discount = SmartRoutinesV2018.ConvertDecimal(SmartReportV2016.Report_Sterling_Format(CHARGE, false));
                description = SELECTION_NAME + string.Format(SmartParametersV2016.currencyFormat, SmartReportV2016.Report_Sterling_Format(CHARGE, true)) + " applied";
                utilityviewmodel.post_period_costs += utilityviewmodel.discount;
                // 'Discount' could be +ve (a charge) or -ve (a discount)
                discount_date = tariff_engine_row.PERIOD_END;
            }
            else
            {
                // Work out charges
                switch (FIELD_NAME)
                {
                    case "TOTAL_COSTS":
                        if (!string.IsNullOrEmpty(CHARGE))
                        {
                            if (CHARGE.IndexOf("%") >= 0)
                            {
                                percentage = SmartRoutinesV2018.ConvertDecimal(CHARGE.Replace("%", ""));
                                utilityviewmodel.discount = (utilityviewmodel.post_period_costs * percentage) / 100;
                                description = SELECTION_NAME + " of " +
                                                CHARGE + " (" +
                                                string.Format(SmartParametersV2016.currencyFormat, utilityviewmodel.discount / 100) + ") " +
                                                " applied to " +
                                                string.Format(SmartParametersV2016.currencyFormat, utilityviewmodel.post_period_costs / 100);
                            }
                            else
                            {
                                amount = SmartRoutinesV2018.ConvertDecimal(SmartReportV2016.Report_Sterling_Format(CHARGE, false));
                                utilityviewmodel.discount = amount;
                                description = SELECTION_NAME + " of " + string.Format(SmartParametersV2016.currencyFormat, SmartReportV2016.Report_Sterling_Format(CHARGE, true)) + " applied";
                            }
                        }
                        break;
                    case "TOTAL_UNITS":
                        // Price Inc Vat is the maximum discount for Price Exc VAT
                        if (!string.IsNullOrEmpty(MAXIMUM_DISCOUNT))
                        {
                            // Don't forget - maximum is NEGATIVE value as is post Condition costs
                            maximum = SmartRoutinesV2018.ConvertDecimal(SmartReportV2016.Report_Sterling_Format(MAXIMUM_DISCOUNT, false));
                            if (utilityviewmodel.post_period_costs >= maximum)
                            {
                                if (tier1_remainder > 0.0M)
                                {
                                    units = tier1_remainder;
                                }
                                else
                                {
                                    units = tariff_engine_row.D_UNITS + tariff_engine_row.N_UNITS;
                                }
                                amount = SmartRoutinesV2018.ConvertDecimal(SmartReportV2016.Report_Sterling_Format(CHARGE, false));
                                utilityviewmodel.post_period_costs += (units * amount);
                                if (utilityviewmodel.post_period_costs > maximum)
                                {
                                    utilityviewmodel.discount = units * amount;
                                }
                                else
                                {
                                    utilityviewmodel.discount = utilityviewmodel.post_period_costs - maximum;
                                }
                                description = SELECTION_NAME + " of " + string.Format(SmartParametersV2016.currencyFormat, utilityviewmodel.discount) + " applied";
                            }
                        }
                        break;
                    default:
                        utilityviewmodel.analyzeMessage = Build_Analyze_Message(brand_code,
                                                                supplier_code,
                                                                payment_plan,
                                                                displayed_resource_code,
                                                                displayed_resource_type,
                                                                "AnalyzeFieldName" +
                                                                SmartParametersV2016.space +
                                                                FIELD_NAME,
                                                                routine);
                        return false;     // Avoid race condition
                }
                TimeSpan ts = SmartParametersV2016.oneDay;
                discount_date = tariff_engine_row.PERIOD_END.Add(ts); //.AddDays(1);
            }
            if (utilityviewmodel.discount != 0.0M)
            {
                if (!wildcard)
                {
                    // 01 is the Standard Rate
                    utilityviewmodel.vat_rate = 0.0M;
                    if (!SmartParseV2016.Lookup_Vat_Rate(SmartParametersV2016.currentVatCode, discount_date, discount_date, ourviewmodel.Blanche.vatRatesList, utilityviewmodel)) // vate_rate
                    {
                        utilityviewmodel.analyzeMessage = Build_Analyze_Message(brand_code,
                                                                supplier_code,
                                                                payment_plan,
                                                                displayed_resource_code,
                                                                displayed_resource_type,
                                                                "AnalyzeVatRates" +
                                                                SmartParametersV2016.space +
                                                                discount_date.ToString(SmartParametersV2016.defaultCulture),
                                                                routine);
                        return false;
                    }
                    // 'Discount' could be +ve (a charge) or -ve (a discount)
                    SmartUtility.TariffCosts tariffcosts = new SmartUtility.TariffCosts()
                    {
                        USERNAME = ourviewmodel.UserName,
                        CUBEFACE_CODE = SmartParametersV2016.Utility,
                        SUPPLIER_CODE = tariff_engine_row.BRAND_CODE,
                        ACCOUNT_NO = tariff_engine_row.ACCOUNT_NO,
                        PERIOD_END = discount_date,
                        CODE = selection_code.ToString("000"),
                        UPPER_DATE = discount_date,
                        ITEM_COUNT = 0,
                        D_UNITS = 0.0M,
                        D_UNITS_RATE = 0.0M,
                        N_UNITS = 0.0M,
                        N_UNITS_RATE = 0.0M,
                        STANDING_CHARGE = 0.0M,
                        //LEFTOVER = 0.0M,          // Don't care
                        FIGURE = utilityviewmodel.discount,
                        DESCRIPTION = description,
                        VAT_RATE = utilityviewmodel.vat_rate         //  Vat Rate on the day
                    };
                    utilityviewmodel.Hezbollah.tariff_costsList.Add(tariffcosts);
                }
            }
            return true;
        }
    }
}