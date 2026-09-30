//using System.Net.Http;        // No more of this absolute bollocks shit
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
//using System.Net;           // <== THIS IS DIFFERENT FOR LIGHSILVER than for Windows FORMS!!!
// What a complete pile of absolute fucking bollocks this shit is


using System.Text;
using System.Threading;
using System.Threading.Tasks;

//using System.Windows.Media;     // This is in PresentationCore.dll <= FUCKING OBVIOUSLY!! Microshit fucking wanking idiots



#if WINFORMS
using SmartDashboard;
#endif

#if WPF
using System.Windows.Media;
//using MoreLinq;
#endif

#if ANDROIDX
using AndroidX.AppCompat.App;
#endif

namespace SmartCubeMobile
{
    public class SmartUtilityScrapeV2022
    {
        internal static async Task<bool> UtilityButtonSubmitClickActual(
#if WINFORMS
                                                    MainProcess process_components,
#endif
#if ANDROIDX
                                                    AppCompatActivity meterActivity,
#endif
                                                    MainViewModel ourviewmodel,
                                                    UtilityViewModel utilityviewmodel,
                                                    List<SmartUsers.VatRates> vatRatesList,
                                                    List<SmartUsers.ExchangeRates> exchangeRatesList,
                                                    List<SmartUsers.InternalPostcodes> postcodesList)
        {
            SmartUtilityV2022.TurnOffUtilityStatus(
#if WINFORMS
                                        process_components,
#endif
                                        utilityviewmodel);
            bool status = false;
            
            if (utilityviewmodel.utilityToken == CancellationToken.None)
            {
                utilityviewmodel.utilityCts = new CancellationTokenSource();
                utilityviewmodel.utilityToken = utilityviewmodel.utilityCts.Token;
            }

            // Not sure if this is the right test, but leave it for the time being
            // It wasn't - THIS is the right test!
            if (utilityviewmodel.UtilitySuppliersList.Count > 0)
            {
                if (!FrontEndGUI.GetBorderVisible(ourviewmodel))
                {
                    FrontEndGUI.SetBorderScrollVisible(ourviewmodel);
                }

                // Now.  Do all Accounts within all Brands 
                foreach (UtilityViewModel.SupplierItem supplier_item in utilityviewmodel.UtilitySuppliersList)
                {
#if WINFORMS
                    if (supplier_item.Colour == ourviewmodel.greenColour)
#endif
#if WPF
                    // Ray!!! Check all this Color comparison shit actually works  
                    SolidColorBrush newBrush = (SolidColorBrush)ourviewmodel.greenColour;
                    if (supplier_item.Colour.ToString() == newBrush.Color.ToString())
#endif
#if ANDROIDX
                    // Because XamarShit chimps don't have a picker with colour that works - but I fucking DO!!
                    if (supplier_item.Colour == ourviewmodel.greenColour)
#endif
                    {
                        utilityviewmodel.supplier_code = 0;
                        utilityviewmodel.brand_code = 0;

                        SmartUtilityV2022.Decode_Tag_Supplier(utilityviewmodel, supplier_item.Value.ToString());
                        if (utilityviewmodel.supplier_code > 0 &&
                            utilityviewmodel.brand_code > 0)
                        {
                            string supplier_name = supplier_item.Content;
#if WINFORMS
                            supplier_name = supplier_name.TrimStart(SmartParametersV2016.defaultgreen).TrimEnd(SmartParametersV2016.defaultred);
#endif
                            List<SmartUtility.Logins> logins_found = SmartSpikeUtilityV2017.Utility_Lookup_Logins(ourviewmodel,
                                                                                                                utilityviewmodel,
                                                                                                                utilityviewmodel.supplier_code,
                                                                                                                utilityviewmodel.brand_code);
                            if (logins_found.Count == 0)
                            {
                                // This is a fatal error because you have an Supplier * <= checked
                                // which SHOULD have created the Resource record for it ... but that's not 
                                // there anymore ... so where has it gone??
                                if (!await SmartRoutinesV2018.CheckTrace(ourviewmodel, utilityviewmodel.utilityToken, utilityviewmodel.supplier_code, utilityviewmodel.brand_code, SmartParametersV2016.Utility.ToString() + " Cannot find resource for: " + supplier_name))
                                {
                                    return false;
                                }
                                // but carry on as there might be others to scrape - no for Utility there aren't
                            }
                            else
                            {
                                if (string.IsNullOrEmpty(logins_found.First().USER_ID) ||
                                    string.IsNullOrEmpty(logins_found.First().USER_PASSWORD))
                                {
                                    // This is a fatal error because you have an Supplier * <= checked
                                    // which has a created the Resource record for it ... but the connection  
                                    // info isn't there anymore ... so where has it gone??
                                    if (!await SmartRoutinesV2018.CheckTrace(ourviewmodel, utilityviewmodel.utilityToken, utilityviewmodel.supplier_code, utilityviewmodel.brand_code, SmartParametersV2016.Utility.ToString() + " Cannot find connection data for: " + supplier_name))
                                    {
                                        return false;
                                    }
                                    // but carry on as there might be others to scrape
                                }
                                else
                                {
                                    //List<FinanceViewModel.AccountItem> accountsList = new List<FinanceViewModel.AccountItem>();
                                    //financeviewmodel.owner = logins_found.First().OWNER;
                                    //financeviewmodel.challenge = "";

                                    //target_account_no = consumers_view_row.ACCOUNT_NO;




                                    // Its in HERE that we see if we have any HTTP connection info
                                    // YOU COULD DO the RBS Sandbox stuff?
                                    //                                    if (bank_name == "Royal Bank of Scotland")
                                    //                                    {
                                    if (await Utility_CheckMeter(
#if WINFORMS
                                                                  process_components,
                                                                  false,
                                                                //components.checkBoxStopOnError,
                                                                //components.checkBoxQuiet,
                                                                //components.checkBoxInsert_SmartSwitch,
                                                                //components.checkBoxDownload_Bills,
#endif
#if ANDROIDX
                                                                meterActivity,
#endif

                                                                ourviewmodel,
                                                                utilityviewmodel,
                                                                vatRatesList,
                                                                exchangeRatesList,
                                                                postcodesList,
                                                                true,   // Utility Submit button
                                                                logins_found.First().SUPPLIER_CODE,
                                                                logins_found.First().BRAND_CODE))






                                    {
                                        // This SHOULD get all the Inserts and Updates in one fell swoop
                                        if (await DoAllSmartUtility(ourviewmodel,
                                                    utilityviewmodel,
                                                    true,
                                                    SmartParametersV2016.sqliteformat,
                                                    utilityviewmodel.supplier_code,
                                                    utilityviewmodel.brand_code))
                                        {
                                            status = true;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                if (FrontEndGUI.GetBorderVisible(ourviewmodel))
                {
                    // Doo two at once
                    FrontEndGUI.SetBorderScrollInvisible(ourviewmodel);
                }

                SmartUtilityV2022.TurnOnUtilityStatus(
#if WINFORMS
                                                        process_components,
#endif
                                                        utilityviewmodel);
            }
            return status;
        }

        //            short local_supplier_code = 0,
        //                        local_brand_code = 0;
        //            string local_account_no = "";


        //            {
        //                // Not sure if this is the right test, but leave it for the time being
        //                // It wasn't - THIS is the right test!
        //                if (utilityviewmodel.SupplierSelectedIndex != -1)
        //                {

        //                    UtilityViewModel.SupplierItem supplier_item = (UtilityViewModel.SupplierItem)utilityviewmodel.UtilitySuppliersList[utilityviewmodel.SupplierSelectedIndex];

        //                    string brand_name = supplier_item.Content.ToString();
        //                    if (!Lookup_Brand_Item(ourviewmodel,
        //                                            utilityviewmodel,
        //                                            utilityviewmodel.resourceCodes,
        //                                            brand_name,
        //                                            rf local_supplier_code,
        //                                            rf local_brand_code))
        //                    {
        //                        if (!(ourviewmodel.quitCts.IsCancellationRequested ||
        //                                utilityviewmodel.utilityToken.IsCancellationRequested))
        //                        {
        //                            await SmartRoutinesV2018.CheckTrace(ourviewmodel, ourviewmodel.userToken, "Brand Item");
        //                        }
        //                        return false;
        //                    }
        //                    else
        //                    {
        //                        // Button Submit Utility
        //                        TurnOffUtilityStatus(
        //#if WINFORMS
        //                                             process_components,
        //#endif
        //                                            utilityviewmodel);
        //                        if (!await Utility_Check_Meter_Async(
        //#if WINFORMS
        //                                                process_components,
        //                                                false,
        //                                                //components.checkBoxStopOnError,
        //                                                //components.checkBoxQuiet,
        //                                                //components.checkBoxInsert_SmartSwitch,
        //                                                //components.checkBoxDownload_Bills,                      
        //#endif

        //                                                signinviewmodel,
        //                                                ourviewmodel,
        //                                                utilityviewmodel,
        //                                                SmartParametersV2016.Utility,        // <= Should be a 'U'
        //                                                DateTime.Now + outrviewmodel.utcOffset,    // Local time
        //                                                utilityviewmodel.resourceCodes,
        //                                                true,       // Utility Submit button
        //                                                SmartParametersV2016.defaultDates,
        //                                                SmartParametersV2016.defaultDate,
        //                                                local_supplier_code,        // Should definitely be known
        //                                                local_brand_code,           // Should definitely be known
        //                                                local_account_no,           // Might not be known (first scrape)
        //                                                DateTime.Now + ourviewmodel.utcOffset // Local time Today's date (might get over-written)
        //                                                ))
        //                        {
        //                            ourviewmodel.meterDisplay = false;
        //                            return false;
        //                        }
        //                        else
        //                        {
        //                            ourviewmodel.meterDisplay = true;
        //                            TurnOnUtilityStatus(
        //#if WINFORMS
        //                                                process_components,
        //#endif
        //                                                utilityviewmodel);
        //                        }
        //                    }
        //                }
        //            }

        //            return true;

        [RequiresUnreferencedCode("Calls SmartCubeMobile.SmartScraperV2016.General_Meter(AppCompatActivity, MainViewModel, UtilityViewModel, List<VatRates>, List<ExchangeRates>, List<InternalPostcodes>, DateTime, Int32[], Boolean, Int16, Int16, Boolean)")]
        internal static async Task<bool> Utility_CheckMeter(
#if WINFORMS
                                                    MainProcess process_components,
                                                    bool decode,
                                                    //CheckBox checkBoxStopOnError,
                                                    //CheckBox checkBoxQuiet,
                                                    //CheckBox checkBoxInsert_SmartSwitch,
                                                    //CheckBox checkBoxDownload_Bills,
#endif
#if ANDROIDX
                                                    AppCompatActivity meterActivity,
#endif

                                                    MainViewModel ourviewmodel,
                                                    UtilityViewModel utilityviewmodel,
                                                    List<SmartUsers.VatRates> vatRatesList,
                                                    List<SmartUsers.ExchangeRates> exchangeRatesList,
                                                    List<SmartUsers.InternalPostcodes> postcodesList,
                                                    bool utility_submit_button, // Which one though?
                                                    short supplier_code,
                                                    short brand_code)
        {
            //string urgent_message = "";

            //string target_supplier_prfix = "",
            //            target_supplier_xxx = "";
            //bool target_useProxy = false;
            //string target_user_id = "",
            //            target_password = "",
            //            target_udprn = SmartParametersV2016.udprnDefault,
            //            target_bill_token = "";
            //char target_bill_currency_symbol = SmartParametersV2016.defaultChar,
            //            target_bill_denomination_symbol = SmartParametersV2016.defaultChar,
            //            target_bill_currency_separator = SmartParametersV2016.defaultChar,
            //            target_bill_thousands_separator = SmartParametersV2016.defaultChar,
            //            target_bill_prfix = SmartParametersV2016.defaultChar;
            DateTime[] target_last_datetime = new DateTime[2],
                        last_datetime = new DateTime[2];

            //bool read_it = false;
            // This could be Default 

            //if (!Utility_Check_Primary(utility_submit_button,
            //                    rf last_datetime,
            //                    rf target_last_datetime,
            //                    rf read_it))
            //{
            //    return false;
            //}

            // If we have to skip the auto-scrape load then make
            // sure ... ==>
            // Look for updates to all the SMARTUTILITY Tariffs
            if (!await Utility_Check_Meter_Skip_Async(ourviewmodel,
                                                        utilityviewmodel))
            {
                // Hamas could be null or the consumers list could be 0
                // Or the updates to the Tariffs went wrong
                return false;
            }


            // This is all just in case the Last Update for Electricity is
            // different than the Last Update for Gas for the same Supplier

            // Form collection
            foreach (SmartUtility.Resources resources_row in utilityviewmodel.Hezbollah.utility_resourcesList)
            {
                // Set the 'other one' in
                target_last_datetime = Utility_Check_WhichResource(resources_row);
                //rf target_last_datetime,
                //rf utilityviewmodel.target_udprn,
                //rf utilityviewmodel.target_udprn);
                break;  // Only one
            }

            // Good to go ? Get all the bits and pieces
            // This routine will return FALSE if the selected supplier
            // has no URL or prfix (i.e. they are empty) so we will never
            // even call READ_GENERAL_METER ...
            if (Get_Bits(ourviewmodel,
                            utilityviewmodel,
                            supplier_code))
            {
                // We don't wait here so it goes banging on ...
                // But I can't take the chance that the Scrapers don't work ..

                bool download_bills = true;

                // Churn, churn, churn ..distracting witter ...
                // Notice there is no 'true' or 'false' check on this next call
                // This is because it is SO FUCKING COMPLICATED to check whether it succeeds or fails ...
//                if (!await SmartScraperV2016.General_Meter(
//#if WINFORMS
//                                                    decode,
//#endif
//#if ANDROIDX
//                                                    meterActivity,
//#endif
//                                                    ourviewmodel,
//                                                    utilityviewmodel,
//                                                    vatRatesList,
//                                                    exchangeRatesList,
//                                                    postcodesList,
//                                                    DateTime.Now + ourviewmodel.utcOffset, // Local time For LAST_UPDATE
//                                                    utilityviewmodel.analysisCost,
//                                                    utility_submit_button,
//                                                    supplier_code,   // <== Supplier
//                                                    brand_code,      // <== Brand
//                                                                     //target_account_no,      // <== Account No
//                                                                     //target_created,         // <== Created (from Accounts) ormaybe today's date                                                    
//                                                    download_bills)) // This is no longer an array       
//                {

//                    if (!(ourviewmodel.quitCts.IsCancellationRequested ||
//                            utilityviewmodel.utilityToken.IsCancellationRequested))
//                    {
//                        if (!await SmartRoutinesV2018.CheckTrace(ourviewmodel, utilityviewmodel.utilityToken, 0, 0, "Problem 60: General Meter"))
//                        {
//                            return false;
//                        }
//                    }
//                    return false;
//                }
            }
            //else
            //{
            // We're not doing a read ...
            // Is autoSwitch on and do we have to SWITCH OUR TARIFF??!!?
            // TO A NEW SUPPLIER????!!!!!!!
            if (utilityviewmodel.autoswitch_pending)
            {
                if (utilityviewmodel.UtilityCosts != null)
                {
                    if (utilityviewmodel.UtilityCosts.Count > 0)
                    {
                        utilityviewmodel.report = "";
                        utilityviewmodel.bollocks = new StringBuilder();

                        utilityviewmodel.switch_info = new SmartUtility.SwitchInfo();

                        SmartUtility.AnalysisCostsView analysis_costs_row = new SmartUtility.AnalysisCostsView();
                        if (utilityviewmodel.UtilityCostsPosition == -1)
                        {
                            analysis_costs_row = utilityviewmodel.UtilityCosts.First();
                        }
                        else
                        {
                            analysis_costs_row = utilityviewmodel.UtilityCosts[utilityviewmodel.UtilityCostsPosition];
                        }
                        // Reset it
                        utilityviewmodel.UtilityCostsPosition = -1;
#if WINFORMS
                        analysis_costs_row = utilityviewmodel.UtilityCosts.First();
                        //analysis_costs_row = (utilityviewmodel.dataGridCostsSource.ItemsSource as List<AnalysisCostsView>).First();
                        //analysis_costs_row = new AnalysisCostsView(Convert.ToInt16(columns[0].FormattedValue),
                        //                            Convert.ToInt16(columns[1].FormattedValue),
                        //                            columns[2].FormattedValue.ToString(),
                        //                            Convert.ToInt32(columns[3].FormattedValue),
                        //                            columns[4].FormattedValue.ToString(),
                        //                            ConvertDecimal(columns[5].FormattedValue),
                        //                            columns[6].FormattedValue.ToString(),
                        //                            columns[7].FormattedValue.ToString(),
                        //                            columns[8].FormattedValue.ToString(),
                        //                            columns[9].FormattedValue.ToString(),
                        //                            ConvertDecimal(columns[10].FormattedValue));
#endif

#if WINFORMS
                        // Need to convert this to analysis_costs_row
                        analysis_costs_row = utilityviewmodel.UtilityCosts.First(); // Fix this Ray Should be selected item
                                                                                    //DataGridViewCellCollection columns = utilityviewmodel.DataGridCosts.First();
                                                                                    //analysis_costs_row = (AnalysisCostsView)utilityviewmodel.dataGridCostsSource.Items[0];

                        //analysis_costs_row = new AnalysisCostsView(Convert.ToInt16(columns[0].FormattedValue),
                        //                        Convert.ToInt16(columns[1].FormattedValue),
                        //                        columns[2].FormattedValue.ToString(),
                        //                        Convert.ToInt32(columns[3].FormattedValue),
                        //                        columns[4].FormattedValue.ToString(),
                        //                        ConvertDecimal(columns[5].FormattedValue),
                        //                        columns[6].FormattedValue.ToString(),
                        //                        columns[7].FormattedValue.ToString(),
                        //                        columns[8].FormattedValue.ToString(),
                        //                        columns[9].FormattedValue.ToString(),
                        //                        ConvertDecimal(columns[10].FormattedValue));
#endif

                        //#if Andyroid
                        // Need to convert this to analysis_costs_row
                        // Couldn't have EVER down this without the help of Eric Schmeck
                        // https://forums.xamarShit.com/discussion/46785/how-to-cast-java-lang-object-to-specified-class-cant-convert-type-java-lang-object-tomyclass?
                        // var bob = utilityviewmodel.DataGridCosts; //  SelectedItem.GetType().GetProperty("Instance");
                        // analysis_costs_row = bob.GetValue(dutilityviewmodel.DataGridCosts.SelectedItem, null) as AnalysisCostsView;
                        //#endif
                        // Its not a problem if nothing wants to Switch ...
                        string err_message = "";
                        if (!await SmartSwitcherV2018.Utility_Check_Switch(
#if ANDROIDX
                                                                meterActivity,
#endif
                                                                ourviewmodel,
                                                                utilityviewmodel,
                                                                utilityviewmodel.resource_code,
                                                                analysis_costs_row,
                                                                utilityviewmodel.switch_info,
                                                                em => err_message = em))
                        {
                            if (!await SmartRoutinesV2018.CheckTrace(ourviewmodel, utilityviewmodel.utilityToken, analysis_costs_row.SUPPLIER_CODE, analysis_costs_row.BRAND_CODE, SmartParametersV2016.Utility.ToString() + " " + "Switch check failed to " + utilityviewmodel.switch_info.proposed_brand_name + "/" + utilityviewmodel.switch_info.proposed_tariff_name))
                            {
                                return false;
                            }
                        }
                        else
                        {
                            //bool status = false;
                            // We are going to get info? Better turn off some buttons first!
                            //

                            // Pop-up the Utility.Accounts window
                            // when we are using energylinx we have GOT to have all of these
                            if (string.IsNullOrEmpty(utilityviewmodel.switch_info.title) ||
                                string.IsNullOrEmpty(utilityviewmodel.switch_info.first_name) ||
                                string.IsNullOrEmpty(utilityviewmodel.switch_info.last_name) ||
                                string.IsNullOrEmpty(utilityviewmodel.switch_info.telephone) ||
                                string.IsNullOrEmpty(utilityviewmodel.switch_info.email) ||
                                string.IsNullOrEmpty(utilityviewmodel.switch_info.bank_account_name) ||
                                (utilityviewmodel.switch_info.bank_account_sort_code.Length != 6) ||
                                (utilityviewmodel.switch_info.bank_account_number.Length != 8) ||
                                (utilityviewmodel.switch_info.bank_account_number.IndexOf("*") >= 0))
                            {
#if WINFORMS
                                utilityviewmodel.switch_info.title = "Mrs";
                                utilityviewmodel.switch_info.birth_day = 29;
                                utilityviewmodel.switch_info.birth_month = 3;
                                utilityviewmodel.switch_info.birth_year = 1927;
                                utilityviewmodel.switch_info.email = "ts21nan@ntlworld.com";
                                utilityviewmodel.switch_info.bank_account_name = "Mrs I Slater"; // This has 18 character limit on the switch
                                utilityviewmodel.switch_info.bank_account_sort_code = "402126";  // Maximum 6
                                utilityviewmodel.switch_info.bank_account_number = "41038230";   // Maximum 8

                                //switch_info.account_no = components.textBoxFirstFourDigits.Text;
                                utilityviewmodel.switch_info.status = "";
#endif


                                utilityviewmodel.switch_brand_name = analysis_costs_row.SUPPLIER_NAME;
                                utilityviewmodel.switch_tariff_name = analysis_costs_row.TARIFF_NAME;
                                utilityviewmodel.XRAY.bank_account_name = utilityviewmodel.switch_info.bank_account_name;
                                utilityviewmodel.XRAY.bank_account_sort_code = utilityviewmodel.switch_info.bank_account_sort_code;
                                utilityviewmodel.XRAY.bank_account_number = utilityviewmodel.switch_info.bank_account_number;

                                //var taskResult = new TaskCompletionSource<bool>();
                                // We are GOOD TO GO!!! 11:00am on 3rd July 2017!!! 
                                if (!await SmartSwitcherV2018.General_Switcher(
#if ANDROIDX
                                                                meterActivity,
#endif
                                                                ourviewmodel,
                                                                utilityviewmodel,
                                                                utilityviewmodel.resource_code,
                                                                true,
#if WINFORMS
                                                                    decode,
#endif
                                                                    utilityviewmodel.switch_info))
                                {
                                    if (!await SmartRoutinesV2018.CheckTrace(ourviewmodel, utilityviewmodel.utilityToken, analysis_costs_row.SUPPLIER_CODE, analysis_costs_row.BRAND_CODE, SmartParametersV2016.Utility.ToString() + " " + "Switch failed from " + utilityviewmodel.switch_info.proposed_brand_code + "/" + utilityviewmodel.switch_info.proposed_tariff_code))
                                    {
                                        return false;
                                    }
                                }
                                else
                                {
                                    // Here - we have either switched E or G or both (Dual)
                                    // Send an e-mail to Anna
                                    if (!await SmartSwitcherV2018.Switch_Email_New(ourviewmodel,
                                                                            utilityviewmodel,
                                                                            utilityviewmodel.switch_info,
                                                                            utilityviewmodel.report,
                                                                            utilityviewmodel.bollocks))
                                    {
                                        if (!await SmartRoutinesV2018.CheckTrace(ourviewmodel, utilityviewmodel.utilityToken, analysis_costs_row.SUPPLIER_CODE, analysis_costs_row.BRAND_CODE, SmartParametersV2016.Utility.ToString() + " " + "Failed send to " + utilityviewmodel.switch_info.email + " for " + analysis_costs_row.SUPPLIER_NAME + "/" + analysis_costs_row.TARIFF_NAME))
                                        {
                                            return false;
                                        }
                                    }
                                    else
                                    {
                                        // There was a success
                                        FrontEndGUI.SetLedColour(ourviewmodel, 7, ourviewmodel.greenColour);
                                        // Clear down ready for an autoswitch on the next resource
                                        utilityviewmodel.autoswitch_pending = false;
                                    }
                                }
                            }

                        }
                    }
                }
                //}

                //if (last_withdrawn_date != utilityviewmodel.withdrawn_date)
                //{
#if WINFORMS || WPF
                //    ourviewmodel.WithdrawnDateMessage = "Tariffs and Prices as of: " + utilityviewmodel.withdrawn_date.ToString(utilityviewmodel.utilityDisplayCulture);
#endif
                //}

            }
            return true;
        }

        //internal static bool Utility_Check_Primary(bool submit_button,
        //                                        rf DateTime[] last_datetime,
        //                                        rf DateTime[] target_last_datetime,
        //                                        rf bool read_it)         // Comes in as false

        //{
        //    last_datetime[0] = SmartParametersV2016.defaultDate;
        //    last_datetime[1] = SmartParametersV2016.defaultDate;

        //    // The Submit button is only ever 'considered' when the network is available
        //    if (submit_button)
        //    {
        //        // Target Supplier Id set in 'Get_Bits'
        //        target_last_datetime[0] = SmartParametersV2016.defaultDate;
        //        target_last_datetime[1] = SmartParametersV2016.defaultDate;
        //        read_it = true;
        //    }
        //    // Even if read_it is false here we might have an scheduled connection pending
        //    return true;
        //}

        private static async Task<bool> Utility_Check_Meter_Skip_Async(MainViewModel ourviewmodel,
                                        UtilityViewModel utilityviewmodel)
        {
            if (ourviewmodel.Hamas.consumersList.Count == 0 ||
                SmartSpikeUtilityV2017.Utility_Lookup_Accounts(ourviewmodel,
                                                        utilityviewmodel,
                                                        SmartParametersV2016.defaultDate).Count == 0)
            {
                //  We can't do anything so return false
                return false; // Avoids 'race' condition
            }

            DateTime compare = utilityviewmodel.tariffs_last_loaded.AddDays(1);
            if (compare < DateTime.Now + ourviewmodel.utcOffset) // Local Time
            {
                // Set the fourth Led to Orange
                FrontEndGUI.SetLedColour(ourviewmodel, 4, ourviewmodel.orangeColour);
                // Fuck Me!  Was this COMPLICATED ... or what???
                if (!await SmartNibbyV2016.MiserableFuckingCow(ourviewmodel,
                                                            utilityviewmodel,
                                                            SmartParametersV2016.Utility,
                                                            SmartParametersV2016.TotalTables,
                                                            SmartParametersV2016.SmartUtilitySchema.ToUpper(),
                                                            SmartParametersV2016.wildcard))
                {
                    // Set the fourth Led to Red ... and carry on!
                    FrontEndGUI.SetLedColour(ourviewmodel, 4, ourviewmodel.redColour);
                    return false; // Avoids 'race' condition
                }
                else
                {
                    FrontEndGUI.SetLedColour(ourviewmodel, 4, ourviewmodel.greenColour);
                    utilityviewmodel.tariffs_last_loaded = DateTime.Now + ourviewmodel.utcOffset; // Local time
                    // Need to re-calculate these two fuckers
                    utilityviewmodel.Hezbollah.e_analysis_costsList.Clear();
                    utilityviewmodel.Hezbollah.g_analysis_costsList.Clear();
                }
            }
            return true;
        }

        //internal static bool Utility_Check_Connection(SmartUtility.ConsumersView consumers_view_row,
        //                                            bool utility_submit_button)
        //{
        //    return ((consumers_view_row.STATUS != 'N') &&
        //            (consumers_view_row.NEXT_CONNECTION != SmartParametersV2016.defaultDate) &&
        //            (SmartRoutinesV2018.DateTimeCompare(consumers_view_row.NEXT_CONNECTION, DateTime.Now + ourviewmodel.utcOffset) < 0) ||    // Local time
        //            utility_submit_button);
        //}



        internal static DateTime[] Utility_Check_WhichResource(SmartUtility.Resources resources_row)
        //rf string e_target_udprn,
        //rf string g_target_udprn)
        {
            // Obviously I could combine these two ...
            //string udprn = "";

            //List<SmartUtility.Meters> mt_found =
            //        SmartSpikeUtilityV2017.Utility_Lookup_Meters(ourviewmodel,
            //                                            utilityviewmodel,
            //                                            utilityviewmodel.resource_code,
            //                                            //utilityviewmodel.resource_type,
            //                                            "");
            //if (mt_found.Count > 0)
            //{
            //    udprn = mt_found.First().UDPRN;                
            //}
            DateTime[] target_last_datetime =
                new DateTime[2] { SmartParametersV2016.defaultDate, SmartParametersV2016.defaultDate };
            char local_resource_code = resources_row.RESOURCE_CODE;
            switch (local_resource_code)
            {
                case SmartParametersV2016.Electricity:
                    target_last_datetime[0] = resources_row.LAST_DATETIME;
                    //e_target_udprn = udprn;
                    break;
                case SmartParametersV2016.Gas:
                    target_last_datetime[1] = resources_row.LAST_DATETIME;
                    //g_target_udprn = udprn;
                    break;
                default:
                    break;
            }
            return target_last_datetime;
        }

        //internal static void Utility_Check_Resource(SmartUtility.ConsumersView consumers_view_row,
        //                                        rf DateTime[] target_last_datetime,
        //                                        rf string e_target_udprn,
        //                                        rf string g_target_udprn,
        //                                        rf char ce_resource_code,
        //                                        rf short ce_supplier_code,
        //                                        rf short ce_brand_code,
        //                                        rf string ce_user_id)
        //{
        //    // Again could combine these two
        //    char local_resource_code = consumers_view_row.RESOURCE_CODE;

        //    string udprn = consumers_view_row.UDPRN;
        //    switch (local_resource_code)
        //    {
        //        case SmartParametersV2016.Electricity:
        //            target_last_datetime[0] = consumers_view_row.LAST_DATETIME;
        //            e_target_udprn = udprn;

        //            // Try and find the corresponding SmartParametersV2016.Gas
        //            // (Both the UDPRN and the Account No may be different)
        //            ce_resource_code = SmartParametersV2016.Gas;
        //            ce_supplier_code = consumers_view_row.SUPPLIER_CODE;
        //            ce_brand_code = consumers_view_row.BRAND_CODE;
        //            ce_user_id = consumers_view_row.USER_ID;
        //            break;
        //        case SmartParametersV2016.Gas:
        //            target_last_datetime[1] = consumers_view_row.LAST_DATETIME;
        //            g_target_udprn = udprn;

        //            // Try and find the corresponding 'E'
        //            // (Both the UDPRN and the Account No may be different)
        //            ce_resource_code = SmartParametersV2016.Electricity;
        //            ce_supplier_code = consumers_view_row.SUPPLIER_CODE;
        //            ce_brand_code = consumers_view_row.BRAND_CODE;
        //            ce_user_id = consumers_view_row.USER_ID;
        //            break;
        //        default:
        //            break;
        //    }
        //    return;
        //}

        internal static bool Get_Bits(MainViewModel ourviewmodel,
                                    UtilityViewModel utilityviewmodel,
                                    short supplier_code)
        {
            utilityviewmodel.target_bill_token = "";

            // Look for the Supplier info
            List<SmartUtility.Suppliers> suppliers_found = SmartSpikeUtilityV2017.Utility_Just_Lookup_Supplier(ourviewmodel,
                                                            utilityviewmodel,
                                                            utilityviewmodel.resource_code,
                                                            supplier_code);
            if (suppliers_found.Count > 0)
            {
                foreach (SmartUtility.Suppliers supplier_row in suppliers_found)
                {
                    //target_supplier_code = supplier_row.SUPPLIER_CODE;
                    //target_brand_code = current_brand_code; // <== KLUDGE City
                    utilityviewmodel.target_supplier_prfix = supplier_row.URL_PREFIX;
                    utilityviewmodel.target_supplier_xxx = supplier_row.URL_TARGET;
                    utilityviewmodel.target_bill_token = supplier_row.BILL_TOKEN;
                    utilityviewmodel.target_bill_currency_symbol = supplier_row.BILL_CURRENCY_SYMBOL;
                    utilityviewmodel.target_bill_denomination_symbol = supplier_row.BILL_DENOMINATION_SYMBOL;
                    utilityviewmodel.target_bill_currency_separator = supplier_row.BILL_CURRENCY_SEPARATOR;
                    utilityviewmodel.target_bill_thousands_separator = supplier_row.BILL_THOUSANDS_SEPARATOR;
                    utilityviewmodel.target_bill_prfix = supplier_row.BILL_PREFIX;
                    if (string.IsNullOrEmpty(supplier_row.USE_PROXY))
                    {
                        utilityviewmodel.target_useProxy = false;
                    }
                    else
                    {
                        utilityviewmodel.target_useProxy = false;   // Always turn it off!!!!
                    }
                    break;
                }
                if (!string.IsNullOrEmpty(utilityviewmodel.target_supplier_xxx))
                {
                    return true;
                }
            }
            // Means we never call READ_GENERAL_METER
            return false;
        }

        internal static async Task<bool> DoAllSmartUtility(MainViewModel ourviewmodel,
                                                        UtilityViewModel utilityviewmodel,
                                                        bool full_or_view,
                                                        string sqliteformat,
                                                        short supplier_code = 0,
                                                        short brand_code = 0,
                                                        bool delete_unallocated_payments = true)

        {
            bool status = false;
            // Now do all the Utility.Accounts etc which may be needed for keys?
            //
            // So we can store it in the fucking SQL Server Express datanase in
            // fucking stupid US "English" format
            //
            utilityviewmodel.bollocks = new StringBuilder();

            
            //These are NEVER updated -only inserted <= Not true - FULL_SCREEN
            DoAllUtility_Resources(utilityviewmodel,
                                    sqliteformat);
            DoAllUtility_ResourcesTypes(utilityviewmodel,
                                    sqliteformat);
            // These can be updated and inserted
            DoAllUtility_Accounts(ourviewmodel,        // <= Means encrypted
                                    utilityviewmodel,
                                    sqliteformat);
            DoAllUtility_Logins(ourviewmodel,          // <= Means encrypted
                                    utilityviewmodel,
                                    sqliteformat);
            DoAllUtility_Switches(ourviewmodel,         // <= Means encrypted
                                    utilityviewmodel,
                                    sqliteformat);
            DoAllBank_Details(ourviewmodel,             // <= Means encrypted
                                    utilityviewmodel,
                                    sqliteformat);
            DoAllMeters(ourviewmodel,             // <= Means encrypted
                                    utilityviewmodel,
                                    sqliteformat);

            // Not done these yet!
            if (full_or_view)
            {
                // These can be only be inserted
                DoAllBillsOut(ourviewmodel,
                            utilityviewmodel,
                            sqliteformat,
                            supplier_code,
                            brand_code);
                // These can be only be inserted
                DoAllBillsResource(ourviewmodel,
                                    utilityviewmodel,
                                    sqliteformat,
                                    supplier_code,
                                    brand_code);
                // Payments MOSTLY depend on Bills
                // We are Displaying not Simulating
                // THIS MAY NOT BE SUFFICIENT FOR THOSE SUPPLIERS WHO TAKE **ONE** PAYMENT
                DoAllPayments(ourviewmodel,
                                utilityviewmodel,
                                sqliteformat,
                                supplier_code,
                                brand_code);
                DoAllUnallocated(ourviewmodel,
                                utilityviewmodel,
                                delete_unallocated_payments,
                                sqliteformat,
                                supplier_code,
                                brand_code);
                // These can be only be inserted
                DoAllAccountCharges(ourviewmodel,
                                utilityviewmodel,
                                sqliteformat,
                                supplier_code,
                                brand_code);
                // These can be only be inserted
                DoAllSupplyCharges(ourviewmodel,
                                utilityviewmodel,
                                sqliteformat,
                                supplier_code,
                                brand_code);
                // These can be only be inserted
                DoAllTariffDetails(ourviewmodel,
                                utilityviewmodel,
                                sqliteformat,
                                supplier_code,
                                brand_code);

                // Now do all the E Stuff and G stuff
                // We are taking a chance here that this all works, because the
                // CONSUMER_ENERGY record has either been Created OE updated with its
                // Info ... SO it had better work ...
                char ar_resource = SmartParametersV2016.Electricity;

                switch (ar_resource) //melia_row.RESOURCE_CODE)
                {
                    case SmartParametersV2016.Electricity:
                        // Do the e stuff - we have all the First and Last dates and Readings
                        // These can be only be inserted
                        DoAllECosts(ourviewmodel, utilityviewmodel, sqliteformat);
                        // These can be only be inserted
                        DoAllEDiscounts(ourviewmodel, utilityviewmodel, sqliteformat);
                        // These can be only be inserted
                        DoAllEReadings(ourviewmodel, utilityviewmodel, sqliteformat);
                        // These can be only be inserted
                        DoAllEUnitCharges(ourviewmodel, utilityviewmodel, sqliteformat);
                        // These can be only be inserted
                        DoAllEStandingCharges(ourviewmodel, utilityviewmodel, sqliteformat);
                        break;

                    case SmartParametersV2016.Gas:
                        // Do the g stuff - we should have all the first and last Dates and Readings ... ?
                        // These can be only be inserted
                        DoAllGCosts(ourviewmodel, utilityviewmodel, sqliteformat);
                        // These can be only be inserted
                        DoAllGDiscounts(ourviewmodel, utilityviewmodel, sqliteformat);
                        // These can be only be inserted
                        DoAllGReadings(ourviewmodel, utilityviewmodel, sqliteformat);
                        // These can be only be inserted
                        DoAllGUnitCharges(ourviewmodel, utilityviewmodel, sqliteformat);
                        // These can be only be inserted
                        DoAllGStandingCharges(ourviewmodel, utilityviewmodel, sqliteformat);
                        break;
                    default:
                        break;
                }
                switch (ar_resource) //amelia_row.RESOURCE_CODE)
                {
                    case SmartParametersV2016.Electricity:
                        // These can be only be inserted
                        DoAllEUsage(ourviewmodel, utilityviewmodel, sqliteformat);
                        break;
                    case SmartParametersV2016.Gas:
                        // These can be only be inserted
                        DoAllGUsage(ourviewmodel, utilityviewmodel, sqliteformat); // Yes, no times on Gas Usage
                        break;
                    default:
                        break;
                }
            }

            // If we haven't anything to send ... then that's good because any inserts or updates won't fail!
            if (utilityviewmodel.bollocks.Length == 0 ||
                ourviewmodel.UserNameColour != ourviewmodel.greenColour)
            {
                return true;
            }

            // This might return true or false
            if (!ourviewmodel.localOnly)
            {
                status = await SmartBobV2017.Insert_COMMON_Async(ourviewmodel,
                                                        utilityviewmodel.utilityToken, //"SmartSwitch", 
                                                        utilityviewmodel.bollocks);
            }
            if (status || ourviewmodel.localOnly)
            {
                // Update the internal DB
                status = await Update_SQLite_Utility(ourviewmodel,
                                    utilityviewmodel,
                                    SmartParametersV2016.SmartUtilitySchema,
                                    ourviewmodel.UDEK);
            }
            // THank FUCK for that!!
            return status;
        }

        private static async Task<bool> ProcessSQLiteListAsync<T1, T2>(
                                        List<T1> sqliteList,
                                        List<T2> domainList,
                                        string tableName,
                                        Func<T1, bool> isUpdated,
                                        MainViewModel ourviewmodel,
                                        UtilityViewModel utilityviewmodel,
                                        string schemaName,
                                        string userName,
                                        string udek)
        {
            if (sqliteList.Count > 0)
            {
                foreach (T1 item in sqliteList)
                {
                    if (isUpdated(item))
                    {
                        if (!await SmartPhyllV2020.UpdateSQLite(ourviewmodel, schemaName, tableName, item))
                            return false;
                    }
                    else
                    {
                        if (!await SmartPhyllV2020.InsertSQLite(ourviewmodel, schemaName, tableName, item))
                            return false;
                    }
                }

                sqliteList.Clear();
                domainList.Clear();

                if (!await SmartPhyllV2020.LoadCommonUtility(
                        ourviewmodel,
                        utilityviewmodel,
                        true,
                        schemaName,
                        tableName,
                        userName,
                        userName,
                        userName,
                        udek,
                        false))
                {
                    return false;
                }
            }
            return true;
        }

        internal static async Task<bool> Update_SQLite_Utility(MainViewModel ourviewmodel,
                                                UtilityViewModel utilityviewmodel,
                                                string schema_name,
                                                string UDEK)
        {
            // Here we successfully updated the remote SERVER db
            // Now we have to UPDATE the SQLite record already in Profiles
            // or INSERT in the new SQLite record into Profiles
            // We never DELETE records btw
            // But we have to do that with the ENCRYPTED record
            // because WPF.db3 holds ENCRYPTED records

            // Don't really need primary, 'cos we only ever store primary!!
            if (schema_name == "")
            {
                return false;
            }

            // Process Account Charges Credits
            if (!await ProcessSQLiteListAsync(
                utilityviewmodel.Hezbollah.sqlite_account_charges_creditsList,
                utilityviewmodel.Hezbollah.account_charges_creditsList,
                "AccChargesCredits",
                row => row.Updated,
                ourviewmodel,
                utilityviewmodel,
                schema_name,
                ourviewmodel.UserName,
                UDEK))
            {
                return false;
            }

            // Process Accounts
            if (!await ProcessSQLiteListAsync(
                utilityviewmodel.Hezbollah.sqlite_utility_accountsList,
                utilityviewmodel.Hezbollah.utility_accountsList,
                "Accounts",
                row => row.Updated,
                ourviewmodel,
                utilityviewmodel,
                schema_name,
                ourviewmodel.UserName,
                UDEK))
            {
                return false;
            }

            // Process BankDetails
            if (!await ProcessSQLiteListAsync(
                utilityviewmodel.Hezbollah.sqlite_utility_bankdetailsList,
                utilityviewmodel.Hezbollah.utility_bankdetailsList,
                "BankDetails",
                row => row.Updated,
                ourviewmodel,
                utilityviewmodel,
                schema_name,
                ourviewmodel.UserName,
                UDEK))
            {
                return false;
            }

            // Process Bills
            if (!await ProcessSQLiteListAsync(
                utilityviewmodel.Hezbollah.sqlite_billsList,
                utilityviewmodel.Hezbollah.billsList,
                "Bills",
                row => row.Updated,
                ourviewmodel,
                utilityviewmodel,
                schema_name,
                ourviewmodel.UserName,
                UDEK))
            {
                return false;
            }

            // Process BillsResource
            if (!await ProcessSQLiteListAsync(
                utilityviewmodel.Hezbollah.sqlite_bills_resourceList,
                utilityviewmodel.Hezbollah.bills_resourceList,
                "BillsResource",
                row => row.Updated,
                ourviewmodel,
                utilityviewmodel,
                schema_name,
                ourviewmodel.UserName,
                UDEK))
            {
                return false;
            }

            // ECosts
            if (!await ProcessSQLiteListAsync(
                utilityviewmodel.Hezbollah.sqlite_e_costsList,
                utilityviewmodel.Hezbollah.e_costsList,
                "ECosts",
                row => row.Updated,
                ourviewmodel,
                utilityviewmodel,
                schema_name,
                ourviewmodel.UserName,
                UDEK))
            {
                return false;
            }

            // EDiscounts
            if (!await ProcessSQLiteListAsync(
                utilityviewmodel.Hezbollah.sqlite_e_discountsList,
                utilityviewmodel.Hezbollah.e_discountsList,
                "EDiscounts",
                row => row.Updated,
                ourviewmodel,
                utilityviewmodel,
                schema_name,
                ourviewmodel.UserName,
                UDEK))
            {
                return false;
            }

            // EReadings
            if (!await ProcessSQLiteListAsync(
                utilityviewmodel.Hezbollah.sqlite_e_readingsList,
                utilityviewmodel.Hezbollah.e_readingsList,
                "EReadings",
                row => row.Updated,
                ourviewmodel,
                utilityviewmodel,
                schema_name,
                ourviewmodel.UserName,
                UDEK))
            {
                return false;
            }

            // EUnitCharges
            if (!await ProcessSQLiteListAsync(
                utilityviewmodel.Hezbollah.sqlite_e_unit_chargesList,
                utilityviewmodel.Hezbollah.e_unit_chargesList,
                "EUnitCharges",
                row => row.Updated,
                ourviewmodel,
                utilityviewmodel,
                schema_name,
                ourviewmodel.UserName,
                UDEK))
            {
                return false;
            }

            // EStandingCharges
            if (!await ProcessSQLiteListAsync(
                utilityviewmodel.Hezbollah.sqlite_e_standing_chargesList,
                utilityviewmodel.Hezbollah.e_standing_chargesList,
                "EStandingCharges",
                row => row.Updated,
                ourviewmodel,
                utilityviewmodel,
                schema_name,
                ourviewmodel.UserName,
                UDEK))
            {
                return false;
            }

            // EUsage
            if (!await ProcessSQLiteListAsync(
                utilityviewmodel.Hezbollah.sqlite_e_usageList,
                utilityviewmodel.Hezbollah.e_usageList,
                "EUsage",
                row => row.Updated,
                ourviewmodel,
                utilityviewmodel,
                schema_name,
                ourviewmodel.UserName,
                UDEK))
            {
                return false;
            }

            // GCosts
            if (!await ProcessSQLiteListAsync(
                utilityviewmodel.Hezbollah.sqlite_g_costsList,
                utilityviewmodel.Hezbollah.g_costsList,
                "GCosts",
                row => row.Updated,
                ourviewmodel,
                utilityviewmodel,
                schema_name,
                ourviewmodel.UserName,
                UDEK))
            {
                return false;
            }

            // GDiscounts
            if (!await ProcessSQLiteListAsync(
                utilityviewmodel.Hezbollah.sqlite_g_discountsList,
                utilityviewmodel.Hezbollah.g_discountsList,
                "GDiscounts",
                row => row.Updated,
                ourviewmodel,
                utilityviewmodel,
                schema_name,
                ourviewmodel.UserName,
                UDEK))
            {
                return false;
            }

            // GReadings
            if (!await ProcessSQLiteListAsync(
                utilityviewmodel.Hezbollah.sqlite_g_readingsList,
                utilityviewmodel.Hezbollah.g_readingsList,
                "GReadings",
                row => row.Updated,
                ourviewmodel,
                utilityviewmodel,
                schema_name,
                ourviewmodel.UserName,
                UDEK))
            {
                return false;
            }

            // GUnitCharges
            if (!await ProcessSQLiteListAsync(
                utilityviewmodel.Hezbollah.sqlite_g_unit_chargesList,
                utilityviewmodel.Hezbollah.g_unit_chargesList,
                "GUnitCharges",
                row => row.Updated,
                ourviewmodel,
                utilityviewmodel,
                schema_name,
                ourviewmodel.UserName,
                UDEK))
            {
                return false;
            }

            // GStandingCharges
            if (!await ProcessSQLiteListAsync(
                utilityviewmodel.Hezbollah.sqlite_g_standing_chargesList,
                utilityviewmodel.Hezbollah.g_standing_chargesList,
                "GStandingCharges",
                row => row.Updated,
                ourviewmodel,
                utilityviewmodel,
                schema_name,
                ourviewmodel.UserName,
                UDEK))
            {
                return false;
            }

            // GUsage
            if (!await ProcessSQLiteListAsync(
                utilityviewmodel.Hezbollah.sqlite_g_usageList,
                utilityviewmodel.Hezbollah.g_usageList,
                "GUsage",
                row => row.Updated,
                ourviewmodel,
                utilityviewmodel,
                schema_name,
                ourviewmodel.UserName,
                UDEK))
            {
                return false;
            }

            // Logins
            if (!await ProcessSQLiteListAsync(
                utilityviewmodel.Hezbollah.sqlite_utility_loginsList,
                utilityviewmodel.Hezbollah.utility_loginsList,
                "Login",
                row => row.Updated,
                ourviewmodel,
                utilityviewmodel,
                schema_name,
                ourviewmodel.UserName,
                UDEK))
            {
                return false;
            }

            // Meters
            if (!await ProcessSQLiteListAsync(
                utilityviewmodel.Hezbollah.sqlite_utility_metersList,
                utilityviewmodel.Hezbollah.utility_metersList,
                "Meters",
                row => row.Updated,
                ourviewmodel,
                utilityviewmodel,
                schema_name,
                ourviewmodel.UserName,
                UDEK))
            {
                return false;
            }

            // Payments
            if (!await ProcessSQLiteListAsync(
                utilityviewmodel.Hezbollah.sqlite_paymentsList,
                utilityviewmodel.Hezbollah.paymentsList,
                "Payments",
                row => row.Updated,
                ourviewmodel,
                utilityviewmodel,
                schema_name,
                ourviewmodel.UserName,
                UDEK))
            {
                return false;
            }

            // Resources
            if (!await ProcessSQLiteListAsync(
                utilityviewmodel.Hezbollah.sqlite_utility_resourcesList,
                utilityviewmodel.Hezbollah.utility_resourcesList,
                "Resources",
                row => row.Updated,
                ourviewmodel,
                utilityviewmodel,
                schema_name,
                ourviewmodel.UserName,
                UDEK))
            {
                return false;
            }

            // ResourcesTypes
            if (!await ProcessSQLiteListAsync(
                utilityviewmodel.Hezbollah.sqlite_utility_resources_typesList,
                utilityviewmodel.Hezbollah.utility_resources_typesList,
                "ResourcesTypes",
                row => row.Updated,
                ourviewmodel,
                utilityviewmodel,
                schema_name,
                ourviewmodel.UserName,
                UDEK))
            {
                return false;
            }

            // SupplyChargesCredits
            if (!await ProcessSQLiteListAsync(
                utilityviewmodel.Hezbollah.sqlite_supply_charges_creditsList,
                utilityviewmodel.Hezbollah.supply_charges_creditsList,
                "SupChargesCredits",
                row => row.Updated,
                ourviewmodel,
                utilityviewmodel,
                schema_name,
                ourviewmodel.UserName,
                UDEK))
            {
                return false;
            }

            // Switches
            if (!await ProcessSQLiteListAsync(
                utilityviewmodel.Hezbollah.sqlite_utility_switchesList,
                utilityviewmodel.Hezbollah.utility_switchesList,
                "Switches",
                row => row.Updated,
                ourviewmodel,
                utilityviewmodel,
                schema_name,
                ourviewmodel.UserName,
                UDEK))
            {
                return false;
            }

            // Michael Keaton  <= Billy Crystal!!

            // TariffDetails
            if (!await ProcessSQLiteListAsync(
                utilityviewmodel.Hezbollah.sqlite_tariff_detailsList,
                utilityviewmodel.Hezbollah.tariff_detailsList,
                "TariffDetails",
                row => row.Updated,
                ourviewmodel,
                utilityviewmodel,
                schema_name,
                ourviewmodel.UserName,
                UDEK))
            {
                return false;
            }

            // Unallocated - one of the VERY FEW?? where we DELETE??
            if (utilityviewmodel.Hezbollah.sqlite_unallocatedList.Count > 0)
            {
                string sql_delete = "DELETE FROM [SmartUtility.Unallocated] " +
                        " WHERE " +
                        "USERNAME = '" + ourviewmodel.UserName + "';";
                if (!await SmartPhyllV2020.ExecuteSQLite(ourviewmodel, sql_delete, em => ourviewmodel.errorMessage = em))
                {
                    return false;
                }
                // Unallocated
                if (!await ProcessSQLiteListAsync(
                    utilityviewmodel.Hezbollah.sqlite_unallocatedList,
                    utilityviewmodel.Hezbollah.unallocatedList,
                    "Unallocated",
                    row => row.Updated,
                    ourviewmodel,
                    utilityviewmodel,
                    schema_name,
                    ourviewmodel.UserName,
                    UDEK))
                {
                    return false;
                }                
            }
            // All done!
            return true;
        }

        internal static void DoAllUtility_Resources(UtilityViewModel utilityviewmodel,
                                                    string sqliteformat)
        {
            utilityviewmodel.Hezbollah.sqlite_utility_resourcesList.Clear();
            if (utilityviewmodel.Hezbollah.utility_resources_changesList.Count > 0)
            {
                FieldInfo[] myFields = typeof(SmartUtility.ResourcesSQLite).GetFields(SmartParametersV2016.bindingFlags);

                string rowAsString = "";
                foreach (SmartUtility.Resources resources_row in utilityviewmodel.Hezbollah.utility_resources_changesList)
                {
                    if (resources_row.Updated == false)
                    {
                        if (!string.IsNullOrEmpty(rowAsString))
                        {
                            // Separate each record line with this
                            rowAsString += SmartParametersV2016.recordSeparator;
                        }
                        SmartUtility.ResourcesSQLite abc = ConvertUtilityResources(resources_row);
                        utilityviewmodel.Hezbollah.sqlite_utility_resourcesList.Add(abc);
                        rowAsString += SmartNibbyV2016.Build_StringNew<SmartUtility.ResourcesSQLite>(myFields, abc, sqliteformat);
                    }
                }
                if (!string.IsNullOrEmpty(rowAsString))
                {
                    // You have to have a case "Resources": followed by update_common in SmartDBServer 
                    // for this to work
                    utilityviewmodel.bollocks.AppendLine(SmartParametersV2016.SmartUtilitySchema + "." + "Resources" +
                                            SmartParametersV2016.groupSeparator +
                                            "I" + SmartParametersV2016.groupSeparator +
                                            rowAsString);
                }

                rowAsString = "";
                foreach (SmartUtility.Resources resources_row in utilityviewmodel.Hezbollah.utility_resources_changesList)
                {
                    if (resources_row.Updated == true)
                    {
                        if (!string.IsNullOrEmpty(rowAsString))
                        {
                            // Separate each record line with this
                            rowAsString += SmartParametersV2016.recordSeparator;
                        }
                        SmartUtility.ResourcesSQLite abc = ConvertUtilityResources(resources_row);
                        utilityviewmodel.Hezbollah.sqlite_utility_resourcesList.Add(abc);
                        rowAsString += SmartNibbyV2016.Build_StringNew<SmartUtility.ResourcesSQLite>(myFields, abc, sqliteformat);
                    }
                }
                if (!string.IsNullOrEmpty(rowAsString))
                {
                    // You have to have a case "Resources": followed by update_common in SmartDBServer 
                    // for this to work
                    utilityviewmodel.bollocks.AppendLine(SmartParametersV2016.SmartUtilitySchema + "." + "Resources" +
                                            SmartParametersV2016.groupSeparator +
                                            "U" + SmartParametersV2016.groupSeparator +
                                            rowAsString);
                }
                utilityviewmodel.Hezbollah.utility_resources_changesList.Clear();
            }
            return;
        }

        internal static SmartUtility.ResourcesSQLite ConvertUtilityResources(SmartUtility.Resources resources_row)
        {
            SmartUtility.ResourcesSQLite resources_enc = new SmartUtility.ResourcesSQLite()
            {
                USERNAME = resources_row.USERNAME,
                CUBEFACE_CODE = resources_row.CUBEFACE_CODE.ToString(),
                RESOURCE_CODE = resources_row.RESOURCE_CODE.ToString(),
                CHECKED = resources_row.CHECKED.ToString(),
                TOTAL_USAGE = resources_row.TOTAL_USAGE,
                UNITRATES_UPDATED = resources_row.UNITRATES_UPDATED,
                LAST_DATETIME = resources_row.LAST_DATETIME,
                LAST_UPDATE = resources_row.LAST_UPDATE,
                EXPIRY_DATE = resources_row.EXPIRY_DATE,
                AUTOSWITCH = resources_row.AUTOSWITCH.ToString(),
                Updated = resources_row.Updated
            };
            return resources_enc;
        }
        internal static void DoAllUtility_ResourcesTypes(UtilityViewModel utilityviewmodel,
                                                    string sqliteformat)
        {
            utilityviewmodel.Hezbollah.sqlite_utility_resources_typesList.Clear();
            if (utilityviewmodel.Hezbollah.utility_resources_types_changesList.Count > 0)
            {
                FieldInfo[] myFields = typeof(SmartUtility.ResourcesTypesSQLite).GetFields(SmartParametersV2016.bindingFlags);

                string rowAsString = "";
                foreach (SmartUtility.ResourcesTypes resourcestypes_row in utilityviewmodel.Hezbollah.utility_resources_types_changesList)
                {
                    if (resourcestypes_row.Updated == false)
                    {
                        if (!string.IsNullOrEmpty(rowAsString))
                        {
                            // Separate each record line with this
                            rowAsString += SmartParametersV2016.recordSeparator;
                        }
                        SmartUtility.ResourcesTypesSQLite abc = ConvertUtilityResourcesTypes(resourcestypes_row);
                        utilityviewmodel.Hezbollah.sqlite_utility_resources_typesList.Add(abc);
                        rowAsString += SmartNibbyV2016.Build_StringNew<SmartUtility.ResourcesTypesSQLite>(myFields, abc, sqliteformat);
                    }
                }
                if (!string.IsNullOrEmpty(rowAsString))
                {
                    // You have to have a case "ResourcesTypes": followed by update_common in SmartDBServer 
                    // for this to work
                    utilityviewmodel.bollocks.AppendLine(SmartParametersV2016.SmartUtilitySchema + "." + "ResourcesTypes" +
                                            SmartParametersV2016.groupSeparator +
                                            "I" + SmartParametersV2016.groupSeparator +
                                            rowAsString);
                }

                rowAsString = "";
                foreach (SmartUtility.ResourcesTypes resourcestypes_row in utilityviewmodel.Hezbollah.utility_resources_types_changesList)
                {
                    if (resourcestypes_row.Updated == true)
                    {
                        if (!string.IsNullOrEmpty(rowAsString))
                        {
                            // Separate each record line with this
                            rowAsString += SmartParametersV2016.recordSeparator;
                        }
                        SmartUtility.ResourcesTypesSQLite abc = ConvertUtilityResourcesTypes(resourcestypes_row);
                        utilityviewmodel.Hezbollah.sqlite_utility_resources_typesList.Add(abc);
                        rowAsString += SmartNibbyV2016.Build_StringNew<SmartUtility.ResourcesTypesSQLite>(myFields, abc, sqliteformat);
                    }
                }
                if (!string.IsNullOrEmpty(rowAsString))
                {
                    // You have to have a case "ResourcesTypes": followed by update_common in SmartDBServer 
                    // for this to work
                    utilityviewmodel.bollocks.AppendLine(SmartParametersV2016.SmartUtilitySchema + "." + "ResourcesTypes" +
                                            SmartParametersV2016.groupSeparator +
                                            "U" + SmartParametersV2016.groupSeparator +
                                            rowAsString);
                }
                utilityviewmodel.Hezbollah.utility_resources_types_changesList.Clear();
            }
            return;
        }

        internal static SmartUtility.ResourcesTypesSQLite ConvertUtilityResourcesTypes(SmartUtility.ResourcesTypes resourcestypes_row)
        {
            SmartUtility.ResourcesTypesSQLite resourcestypes_enc = new SmartUtility.ResourcesTypesSQLite()
            {
                USERNAME = resourcestypes_row.USERNAME,
                CUBEFACE_CODE = resourcestypes_row.CUBEFACE_CODE.ToString(),
                RESOURCE_CODE = resourcestypes_row.RESOURCE_CODE.ToString(),
                RESOURCE_TYPE = resourcestypes_row.RESOURCE_TYPE,
                Updated = resourcestypes_row.Updated
            };
            return resourcestypes_enc;
        }
        internal static void DoAllUtility_Accounts(MainViewModel ourviewmodel,
                                                    UtilityViewModel utilityviewmodel,
                                                    string sqliteformat)
        {
            string deriveds = "SmartUtility.Accounts";
            utilityviewmodel.Hezbollah.sqlite_utility_accountsList.Clear();
            if (utilityviewmodel.Hezbollah.utility_accounts_changesList.Count > 0)
            {
                FieldInfo[] myFields = typeof(SmartUtility.AccountsSQLite).GetFields(SmartParametersV2016.bindingFlags);

                string rowAsString = "";
                foreach (SmartUtility.Accounts accounts_row in utilityviewmodel.Hezbollah.utility_accounts_changesList)
                {
                    if (accounts_row.Updated == false)
                    {
                        if (!string.IsNullOrEmpty(rowAsString))
                        {
                            // Separate each record line with this
                            rowAsString += SmartParametersV2016.recordSeparator;
                        }
                        SmartUtility.AccountsSQLite abc = ConvertUtilityAccounts(ourviewmodel, accounts_row, deriveds.Length);
                        utilityviewmodel.Hezbollah.sqlite_utility_accountsList.Add(abc);
                        rowAsString += SmartNibbyV2016.Build_StringNew<SmartUtility.AccountsSQLite>(myFields, abc, sqliteformat);
                    }
                }
                if (!string.IsNullOrEmpty(rowAsString))
                {
                    // You have to have a case "Utility.Accounts": followed by update_common in SmartDBServer 
                    // for this to work
                    utilityviewmodel.bollocks.AppendLine(deriveds +
                                            SmartParametersV2016.groupSeparator +
                                            "I" + SmartParametersV2016.groupSeparator +
                                            rowAsString);
                }

                rowAsString = "";
                foreach (SmartUtility.Accounts accounts_row in utilityviewmodel.Hezbollah.utility_accounts_changesList)
                {
                    if (accounts_row.Updated == true)
                    {
                        if (!string.IsNullOrEmpty(rowAsString))
                        {
                            // Separate each record line with this
                            rowAsString += SmartParametersV2016.recordSeparator;
                        }
                        SmartUtility.AccountsSQLite abc = ConvertUtilityAccounts(ourviewmodel, accounts_row, deriveds.Length);
                        utilityviewmodel.Hezbollah.sqlite_utility_accountsList.Add(abc);
                        rowAsString += SmartNibbyV2016.Build_StringNew<SmartUtility.AccountsSQLite>(myFields, abc, sqliteformat);
                    }
                }
                if (!string.IsNullOrEmpty(rowAsString))
                {
                    // You have to have a case "Utility.Accounts": followed by update_common in SmartDBServer 
                    // for this to work
                    utilityviewmodel.bollocks.AppendLine(deriveds +
                                            SmartParametersV2016.groupSeparator +
                                            "U" + SmartParametersV2016.groupSeparator +
                                            rowAsString);
                }
                utilityviewmodel.Hezbollah.utility_accounts_changesList.Clear();
            }
            return;
        }

        internal static SmartUtility.AccountsSQLite ConvertUtilityAccounts(MainViewModel ourviewmodel,
                                                                SmartUtility.Accounts accounts_row,
                                                                int derived)
        {
            // Encrypt all of these into DETAILS with randomkey
            string key_details_unencrypted = accounts_row.ACCOUNT_NO +
                                            SmartParametersV2016.fieldSeparator +
                                            accounts_row.UDPRN;
            string key_details = (ourviewmodel.UDEK == "" ?
                key_details_unencrypted : SmartEncryptionV2016.DoTheBiz(key_details_unencrypted, SmartPhyllV2020.BuildEncryptKey(ourviewmodel.UDEK, accounts_row.RANDOMKEY1, derived), "", true));

            string details_unencrypted = accounts_row.STATUS.ToString() +
                                SmartParametersV2016.fieldSeparator +
                                accounts_row.CURRENCY_ORDINAL +
                                SmartParametersV2016.fieldSeparator +
                                accounts_row.TARIFF_CODE.ToString() +
                                SmartParametersV2016.fieldSeparator +
                                accounts_row.PAYMENT_PLAN.ToString();

            // Time for a new RandomKey2
            accounts_row.RANDOMKEY2 = SmartRoutinesV2018.GetNextRandomNew(ourviewmodel.randomR);
            string details = (ourviewmodel.UDEK == "" ?
                details_unencrypted : SmartEncryptionV2016.DoTheBiz(details_unencrypted, SmartPhyllV2020.BuildEncryptKey(ourviewmodel.UDEK, accounts_row.RANDOMKEY2, derived), "", true));

            SmartUtility.AccountsSQLite accounts_enc = new SmartUtility.AccountsSQLite()
            {
                USERNAME = accounts_row.USERNAME,
                CUBEFACE_CODE = accounts_row.CUBEFACE_CODE.ToString(),
                RESOURCE_CODE = accounts_row.RESOURCE_CODE.ToString(),
                SUPPLIER_CODE = accounts_row.SUPPLIER_CODE,
                BRAND_CODE = accounts_row.BRAND_CODE,
                ACCOUNT_CREATED = accounts_row.ACCOUNT_CREATED,
                KEY_DETAILS = key_details,
                RANDOMKEY1 = accounts_row.RANDOMKEY1,
                DETAILS = details,
                RANDOMKEY2 = accounts_row.RANDOMKEY2,
                Updated = accounts_row.Updated
            };
            return accounts_enc;
        }

        internal static void DoAllUtility_Logins(MainViewModel ourviewmodel,
                                                UtilityViewModel utilityviewmodel,
                                                string sqliteformat)
        {
            string deriveds = "SmartUtility.Logins";
            utilityviewmodel.Hezbollah.sqlite_utility_loginsList.Clear();
            if (utilityviewmodel.Hezbollah.utility_logins_changesList.Count > 0)
            {
                FieldInfo[] myFields = typeof(SmartUtility.LoginsSQLite).GetFields(SmartParametersV2016.bindingFlags);

                string rowAsString = "";
                foreach (SmartUtility.Logins logins_row in utilityviewmodel.Hezbollah.utility_logins_changesList)
                {
                    // What is this saying?  Its saying you can't update the CATEGORY, SUPPLIER, BRAND or CREATED
                    if (logins_row.Updated == false)
                    {
                        if (!string.IsNullOrEmpty(rowAsString))
                        {
                            // Separate each record line with this
                            rowAsString += SmartParametersV2016.recordSeparator;
                        }
                        SmartUtility.LoginsSQLite abc = ConvertUtilityLogins(ourviewmodel, logins_row, deriveds.Length);
                        utilityviewmodel.Hezbollah.sqlite_utility_loginsList.Add(abc);
                        rowAsString += SmartNibbyV2016.Build_StringNew<SmartUtility.LoginsSQLite>(myFields, abc, sqliteformat);
                    }
                }
                if (!string.IsNullOrEmpty(rowAsString))
                {
                    // You have to have a case "Logins:": followed by update_common in SmartDBServer 
                    // for this to work
                    utilityviewmodel.bollocks.AppendLine(deriveds +
                                            SmartParametersV2016.groupSeparator +
                                            "I" + SmartParametersV2016.groupSeparator +
                                            rowAsString);
                }

                rowAsString = "";
                foreach (SmartUtility.Logins logins_row in utilityviewmodel.Hezbollah.utility_logins_changesList)
                {
                    // What is this saying?  Its saying you can't update the CATEGORY, SUPPLIER, BRAND or CREATED
                    if (logins_row.Updated == true)
                    {
                        if (!string.IsNullOrEmpty(rowAsString))
                        {
                            // Separate each record line with this
                            rowAsString += SmartParametersV2016.recordSeparator;
                        }
                        SmartUtility.LoginsSQLite abc = ConvertUtilityLogins(ourviewmodel, logins_row, deriveds.Length);
                        utilityviewmodel.Hezbollah.sqlite_utility_loginsList.Add(abc);
                        rowAsString += SmartNibbyV2016.Build_StringNew<SmartUtility.LoginsSQLite>(myFields, abc, sqliteformat);
                    }
                }
                if (!string.IsNullOrEmpty(rowAsString))
                {
                    // You have to have a case "Logins": followed by update_common in SmartDBServer 
                    // for this to work
                    utilityviewmodel.bollocks.AppendLine(deriveds +
                                            SmartParametersV2016.groupSeparator +
                                            "U" + SmartParametersV2016.groupSeparator +
                                            rowAsString);
                }
                utilityviewmodel.Hezbollah.utility_logins_changesList.Clear();
            }

            // We don't do it like this anymore ... we don't 'Delete' anything

            //if (utilityviewmodel.Hezbollah.utility_logins_deletesList.Count > 0)
            //{
            //    // Delete all Logins in list
            //    foreach (SmartUtility.Logins logins_row in utilityviewmodel.Hezbollah.utility_logins_deletesList)
            //    {
            //        string sql = "DELETE FROM " + "Logins" +          // Same as above - might need a schema if not added auto ..think it will ALWAYS need a schema!
            //                    " WHERE USERNAME = '" + ourviewmodel.UserName + "'" +
            //                    " AND CUBEFACE_CODE = '" + logins_row.CUBEFACE_CODE + "'" +
            //                    " AND SUPPLIER_CODE = " + logins_row.SUPPLIER_CODE +
            //                    " AND BRAND_CODE = " + logins_row.BRAND_CODE +
            //                    " AND LOGIN_CREATED = '" + logins_row.LOGIN_CREATED.ToString(sqliteformat) + "'";
            //        utilityviewmodel.bollocks.AppendLine(SmartParametersV2016.smartutilitySchema + "." +
            //                            "Logins" + SmartParametersV2016.groupSeparator +
            //                            "D" + SmartParametersV2016.groupSeparator +
            //                            sql);
            //    }
            //}
            return;
        }

        internal static SmartUtility.LoginsSQLite ConvertUtilityLogins(MainViewModel ourviewmodel,
                                                                SmartUtility.Logins logins_row,
                                                                int derived)
        {
            // Encrypt all of these into DETAILS with randomkey
            string details_unencrypted = logins_row.USER_ID +
                                SmartParametersV2016.fieldSeparator +
                                logins_row.USER_PASSWORD;
            // Time for a new RandomKey
            logins_row.RANDOMKEY = SmartRoutinesV2018.GetNextRandomNew(ourviewmodel.randomR);
            string details = (ourviewmodel.UDEK == "" ?
                details_unencrypted : SmartEncryptionV2016.DoTheBiz(details_unencrypted, SmartPhyllV2020.BuildEncryptKey(ourviewmodel.UDEK, logins_row.RANDOMKEY, derived), "", true));

            SmartUtility.LoginsSQLite logins_enc = new SmartUtility.LoginsSQLite()
            {
                CUBEFACE_CODE = logins_row.CUBEFACE_CODE.ToString(),
                //RESOURCE_CODE = logins_row.RESOURCE_CODE,
                SUPPLIER_CODE = logins_row.SUPPLIER_CODE,
                BRAND_CODE = logins_row.BRAND_CODE,
                LOGIN_CREATED = logins_row.LOGIN_CREATED,
                DETAILS = details,
                RANDOMKEY = logins_row.RANDOMKEY,
                Updated = logins_row.Updated
            };
            return logins_enc;
        }

        internal static void DoAllUtility_Switches(MainViewModel ourviewmodel,
                                                    UtilityViewModel utilityviewmodel,
                                                    string sqliteformat)
        {
            string deriveds = "SmartUtility.Switches";
            utilityviewmodel.Hezbollah.sqlite_utility_switchesList.Clear();
            if (utilityviewmodel.Hezbollah.utility_switches_changesList.Count > 0)
            {
                FieldInfo[] myFields = typeof(SmartUtility.SwitchesSQLite).GetFields(SmartParametersV2016.bindingFlags);

                string rowAsString = "";
                foreach (SmartUtility.Switches switches_row in utilityviewmodel.Hezbollah.utility_switches_changesList)
                {
                    if (switches_row.Updated == false)
                    {
                        if (!string.IsNullOrEmpty(rowAsString))
                        {
                            // Separate each record line with this
                            rowAsString += SmartParametersV2016.recordSeparator;
                        }
                        SmartUtility.SwitchesSQLite abc = ConvertUtilitySwitches(ourviewmodel, switches_row, deriveds.Length);
                        utilityviewmodel.Hezbollah.sqlite_utility_switchesList.Add(abc);
                        rowAsString += SmartNibbyV2016.Build_StringNew<SmartUtility.SwitchesSQLite>(myFields, abc, sqliteformat);
                    }
                }
                if (!string.IsNullOrEmpty(rowAsString))
                {
                    // You have to have a case "Switches": followed by update_common in SmartDBServer 
                    // for this to work
                    utilityviewmodel.bollocks.AppendLine(deriveds +
                                            SmartParametersV2016.groupSeparator +
                                            "I" + SmartParametersV2016.groupSeparator +
                                            rowAsString);
                }

                rowAsString = "";
                foreach (SmartUtility.Switches switches_row in utilityviewmodel.Hezbollah.utility_switches_changesList)
                {
                    if (switches_row.Updated == true)
                    {
                        if (!string.IsNullOrEmpty(rowAsString))
                        {
                            // Separate each record line with this
                            rowAsString += SmartParametersV2016.recordSeparator;
                        }
                        SmartUtility.SwitchesSQLite abc = ConvertUtilitySwitches(ourviewmodel, switches_row, deriveds.Length);
                        utilityviewmodel.Hezbollah.sqlite_utility_switchesList.Add(abc);
                        rowAsString += SmartNibbyV2016.Build_StringNew<SmartUtility.SwitchesSQLite>(myFields, abc, sqliteformat);
                    }
                }
                if (!string.IsNullOrEmpty(rowAsString))
                {
                    // You have to have a case "Switches": followed by update_common in SmartDBServer 
                    // for this to work
                    utilityviewmodel.bollocks.AppendLine(deriveds +
                                            SmartParametersV2016.groupSeparator +
                                            "U" + SmartParametersV2016.groupSeparator +
                                            rowAsString);
                }
                utilityviewmodel.Hezbollah.utility_switches_changesList.Clear();
            }
            return;
        }

        internal static SmartUtility.SwitchesSQLite ConvertUtilitySwitches(MainViewModel ourviewmodel,
                                                                SmartUtility.Switches switches_row,
                                                                int derived)
        {
            // Encrypt all of these into DETAILS with randomkey
            string details_unencrypted = switches_row.ACCOUNT_NO +
                                SmartParametersV2016.fieldSeparator +
                                switches_row.MPAN_MPRN;
            string details = (ourviewmodel.UDEK == "" ?
                details_unencrypted : SmartEncryptionV2016.DoTheBiz(details_unencrypted, SmartPhyllV2020.BuildEncryptKey(ourviewmodel.UDEK, switches_row.RANDOMKEY, derived), "", true));


            SmartUtility.SwitchesSQLite switches_enc = new SmartUtility.SwitchesSQLite()
            {
                USERNAME = switches_row.USERNAME,
                CUBEFACE_CODE = switches_row.CUBEFACE_CODE.ToString(),
                SUPPLIER_CODE = switches_row.SUPPLIER_CODE,
                BRAND_CODE = switches_row.BRAND_CODE,
                RESOURCE_CODE = switches_row.RESOURCE_CODE.ToString(),
                SWITCH_CREATED = switches_row.SWITCH_CREATED,
                KEY_DETAILS = details,
                RANDOMKEY = switches_row.RANDOMKEY,
                AGE_60PLUS = switches_row.AGE_60PLUS,
                AUTOSWITCH_DESTINATION = switches_row.AUTOSWITCH_DESTINATION,
                AUTOSWITCH_EMAIL_SENT = DateTime.Now + ourviewmodel.utcOffset,  // Local time
                AUTOSWITCH_SUPPLIER_CODE = switches_row.AUTOSWITCH_SUPPLIER_CODE,
                AUTOSWITCH_BRAND_CODE = switches_row.AUTOSWITCH_BRAND_CODE,
                AUTOSWITCH_TARIFF_CODE = switches_row.AUTOSWITCH_TARIFF_CODE,
                Updated = switches_row.Updated
            };
            return switches_enc;
        }

        internal static void DoAllBank_Details(MainViewModel ourviewmodel,
                                                    UtilityViewModel utilityviewmodel,
                                                    string sqliteformat)
        {
            string deriveds = "SmartUtility.BankDetails";
            utilityviewmodel.Hezbollah.sqlite_utility_bankdetailsList.Clear();
            if (utilityviewmodel.Hezbollah.utility_bankdetails_changesList.Count > 0)
            {
                FieldInfo[] myFields = typeof(SmartUtility.BankDetailsSQLite).GetFields(SmartParametersV2016.bindingFlags);

                string rowAsString = "";
                foreach (SmartUtility.BankDetails bankdetails_row in utilityviewmodel.Hezbollah.utility_bankdetails_changesList)
                {
                    if (bankdetails_row.Updated == false)
                    {
                        if (!string.IsNullOrEmpty(rowAsString))
                        {
                            // Separate each record line with this
                            rowAsString += SmartParametersV2016.recordSeparator;
                        }
                        SmartUtility.BankDetailsSQLite abc = ConvertUtilityBankDetails(ourviewmodel, bankdetails_row, deriveds.Length);
                        utilityviewmodel.Hezbollah.sqlite_utility_bankdetailsList.Add(abc);
                        rowAsString += SmartNibbyV2016.Build_StringNew<SmartUtility.BankDetailsSQLite>(myFields, abc, sqliteformat);
                    }
                }
                if (!string.IsNullOrEmpty(rowAsString))
                {
                    // You have to have a case "BankDetails": followed by update_common in SmartDBServer 
                    // for this to work
                    utilityviewmodel.bollocks.AppendLine(deriveds +
                                            SmartParametersV2016.groupSeparator +
                                            "I" + SmartParametersV2016.groupSeparator +
                                            rowAsString);
                }

                rowAsString = "";
                foreach (SmartUtility.BankDetails bankdetails_row in utilityviewmodel.Hezbollah.utility_bankdetails_changesList)
                {
                    if (bankdetails_row.Updated == true)
                    {
                        if (!string.IsNullOrEmpty(rowAsString))
                        {
                            // Separate each record line with this
                            rowAsString += SmartParametersV2016.recordSeparator;
                        }
                        SmartUtility.BankDetailsSQLite abc = ConvertUtilityBankDetails(ourviewmodel, bankdetails_row, deriveds.Length);
                        utilityviewmodel.Hezbollah.sqlite_utility_bankdetailsList.Add(abc);
                        rowAsString += SmartNibbyV2016.Build_StringNew<SmartUtility.BankDetailsSQLite>(myFields, abc, sqliteformat);
                    }
                }
                if (!string.IsNullOrEmpty(rowAsString))
                {
                    // You have to have a case "BankDetails": followed by update_common in SmartDBServer 
                    // for this to work
                    utilityviewmodel.bollocks.AppendLine(deriveds +
                                            SmartParametersV2016.groupSeparator +
                                            "U" + SmartParametersV2016.groupSeparator +
                                            rowAsString);
                }
                utilityviewmodel.Hezbollah.utility_bankdetails_changesList.Clear();
            }
            return;
        }

        internal static SmartUtility.BankDetailsSQLite ConvertUtilityBankDetails(MainViewModel ourviewmodel,
                                                                SmartUtility.BankDetails bankdetails_row,
                                                                int derived)
        {
            // Look at the rationale in Structures to see why
            // SORTCODE, ACCOUNT_NO and PAYDAY shouldn't be part
            // of the key for this table
            string key_details_unencrypted = bankdetails_row.ACCOUNT_NO;
            string key_details = (ourviewmodel.UDEK == "" ?
                key_details_unencrypted : SmartEncryptionV2016.DoTheBiz(key_details_unencrypted, SmartPhyllV2020.BuildEncryptKey(ourviewmodel.UDEK, bankdetails_row.RANDOMKEY1, derived), "", true));

            // Encrypt all of these into DETAILS with randomkey
            string details_unencrypted = bankdetails_row.FINANCE_INSTITUTION_CODE.ToString() +
                                SmartParametersV2016.fieldSeparator +
                                bankdetails_row.FINANCE_BRAND_CODE.ToString() +
                                SmartParametersV2016.fieldSeparator +
                                bankdetails_row.FINANCE_SORTCODE +
                                SmartParametersV2016.fieldSeparator +
                                bankdetails_row.FINANCE_ACCOUNT_NO +
                                SmartParametersV2016.fieldSeparator +
                                bankdetails_row.BANK_PAYDAY.ToString();

            // Time for a new RandomKey
            bankdetails_row.RANDOMKEY2 = SmartRoutinesV2018.GetNextRandomNew(ourviewmodel.randomR);
            string details = (ourviewmodel.UDEK == "" ?
                details_unencrypted : SmartEncryptionV2016.DoTheBiz(details_unencrypted, SmartPhyllV2020.BuildEncryptKey(ourviewmodel.UDEK, bankdetails_row.RANDOMKEY2, derived), "", true));

            SmartUtility.BankDetailsSQLite bank_details_enc = new SmartUtility.BankDetailsSQLite()
            {
                USERNAME = bankdetails_row.USERNAME,
                CUBEFACE_CODE = bankdetails_row.CUBEFACE_CODE.ToString(),
                SUPPLIER_CODE = bankdetails_row.SUPPLIER_CODE,
                BRAND_CODE = bankdetails_row.BRAND_CODE,
                ACCOUNT_CREATED = bankdetails_row.ACCOUNT_CREATED,
                KEY_DETAILS = key_details,
                RANDOMKEY1 = bankdetails_row.RANDOMKEY1,
                DETAILS = details,
                RANDOMKEY2 = bankdetails_row.RANDOMKEY2,
                Updated = bankdetails_row.Updated
            };
            return bank_details_enc;
        }

        internal static void DoAllMeters(MainViewModel ourviewmodel,
                                                    UtilityViewModel utilityviewmodel,
                                                    string sqliteformat)
        {
            string deriveds = "SmartUtility.Meters";
            utilityviewmodel.Hezbollah.sqlite_utility_metersList.Clear();
            if (utilityviewmodel.Hezbollah.utility_meters_changesList.Count > 0)
            {
                FieldInfo[] myFields = typeof(SmartUtility.Meters).GetFields(SmartParametersV2016.bindingFlags);

                string rowAsString = "";
                foreach (SmartUtility.Meters meters_row in utilityviewmodel.Hezbollah.utility_meters_changesList)
                {
                    if (meters_row.Updated == false)
                    {
                        if (!string.IsNullOrEmpty(rowAsString))
                        {
                            // Separate each record line with this
                            rowAsString += SmartParametersV2016.recordSeparator;
                        }
                        SmartUtility.MetersSQLite abc = ConvertUtilityMeters(ourviewmodel, meters_row, deriveds.Length);
                        utilityviewmodel.Hezbollah.sqlite_utility_metersList.Add(abc);
                        rowAsString += SmartNibbyV2016.Build_StringNew<SmartUtility.MetersSQLite>(myFields, abc, sqliteformat);
                    }
                }
                if (!string.IsNullOrEmpty(rowAsString))
                {
                    // You have to have a case "Meters": followed by update_common in SmartDBServer 
                    // for this to work
                    utilityviewmodel.bollocks.AppendLine(deriveds +
                                            SmartParametersV2016.groupSeparator +
                                            "I" + SmartParametersV2016.groupSeparator +
                                            rowAsString);
                }

                rowAsString = "";
                foreach (SmartUtility.Meters meters_row in utilityviewmodel.Hezbollah.utility_meters_changesList)
                {
                    if (meters_row.Updated == true)
                    {
                        if (!string.IsNullOrEmpty(rowAsString))
                        {
                            // Separate each record line with this
                            rowAsString += SmartParametersV2016.recordSeparator;
                        }
                        SmartUtility.MetersSQLite abc = ConvertUtilityMeters(ourviewmodel, meters_row, deriveds.Length);
                        utilityviewmodel.Hezbollah.sqlite_utility_metersList.Add(abc);
                        rowAsString += SmartNibbyV2016.Build_StringNew<SmartUtility.MetersSQLite>(myFields, abc, sqliteformat);
                    }
                }
                if (!string.IsNullOrEmpty(rowAsString))
                {
                    // You have to have a case "Meters": followed by update_common in SmartDBServer 
                    // for this to work
                    utilityviewmodel.bollocks.AppendLine(deriveds +
                                            SmartParametersV2016.groupSeparator +
                                            "U" + SmartParametersV2016.groupSeparator +
                                            rowAsString);
                }
                utilityviewmodel.Hezbollah.utility_meters_changesList.Clear();
            }
            return;
        }

        internal static SmartUtility.MetersSQLite ConvertUtilityMeters(MainViewModel ourviewmodel,
                                                                SmartUtility.Meters meters_row,
                                                                int derived)
        {
            // Encrypt all of these into DETAILS with randomkey
            string key_details_unencrypted = meters_row.MPAN_MPRN;
            string key_details = (ourviewmodel.UDEK == "" ?
                key_details_unencrypted : SmartEncryptionV2016.DoTheBiz(key_details_unencrypted, SmartPhyllV2020.BuildEncryptKey(ourviewmodel.UDEK, meters_row.RANDOMKEY1, derived), "", true));

            string details_unencrypted = meters_row.METER_SERIAL_NO;// +
                                                                    //SmartParametersV2016.fieldSeparator +
                                                                    //meters_row.UDPRN;
                                                                    // Time for a new RandomKey
            meters_row.RANDOMKEY2 = SmartRoutinesV2018.GetNextRandomNew(ourviewmodel.randomR);
            string details = (ourviewmodel.UDEK == "" ?
                details_unencrypted : SmartEncryptionV2016.DoTheBiz(details_unencrypted, SmartPhyllV2020.BuildEncryptKey(ourviewmodel.UDEK, meters_row.RANDOMKEY2, derived), "", true));

            SmartUtility.MetersSQLite meters_enc = new SmartUtility.MetersSQLite()
            {
                USERNAME = meters_row.USERNAME,
                CUBEFACE_CODE = meters_row.CUBEFACE_CODE.ToString(),
                RESOURCE_CODE = meters_row.RESOURCE_CODE.ToString(),
                RESOURCE_TYPE = meters_row.RESOURCE_TYPE,
                KEY_DETAILS = key_details,
                RANDOMKEY1 = meters_row.RANDOMKEY1,
                DETAILS = details,
                RANDOMKEY2 = meters_row.RANDOMKEY2,
                Updated = meters_row.Updated
            };
            return meters_enc;
        }

        internal static void DoAllBillsOut(MainViewModel ourviewmodel,
                                            UtilityViewModel utilityviewmodel,
                                            string sqliteformat,
                                            short supplier_code,
                                            short brand_code)
        {
            string deriveds = "SmartUtility.Bills";

            utilityviewmodel.Hezbollah.sqlite_billsList.Clear();
            List<SmartUtility.Bills> bills_temp = new List<SmartUtility.Bills>();
            // All these depend on the Bill (or Bills going in)
            if (utilityviewmodel.Hezbollah.bills_changesList.Count == 0)
            {
                // We have no Bills Out going out ... but we may have had some Bills In which we might need
                // later on in the next routine for the Bills Resources
                // Clear this down
                bills_temp.Clear();

                foreach (SmartUtility.Bills bills_row in utilityviewmodel.Hezbollah.billsList)
                {
                    if ((bills_row.CUBEFACE_CODE == SmartParametersV2016.Utility) &&
                        (bills_row.SUPPLIER_CODE == supplier_code) &&
                        (bills_row.BRAND_CODE == brand_code) &&
                        (bills_row.ACCOUNT_NO == utilityviewmodel.account_no) &&
                        (bills_row.ACCOUNT_CREATED == utilityviewmodel.account_created))
                    {
                        bills_temp.Add(bills_row);
                    }
                }
                utilityviewmodel.Hezbollah.bills_tempList.Clear();
                utilityviewmodel.Hezbollah.bills_tempList.AddRange(bills_temp);
                utilityviewmodel.Hezbollah.bills_changesList.Clear();
            }
            else
            {
                FieldInfo[] myFields = typeof(SmartUtility.BillsSQLite).GetFields(SmartParametersV2016.bindingFlags);
                string rowAsString = "";

                // Clear this down
                bills_temp.Clear();

                foreach (SmartUtility.Bills bills_row in utilityviewmodel.Hezbollah.bills_changesList)
                {
                    if ((bills_row.CUBEFACE_CODE == SmartParametersV2016.Utility) &&
                        (bills_row.SUPPLIER_CODE == supplier_code) &&
                        (bills_row.BRAND_CODE == brand_code) &&
                        (bills_row.ACCOUNT_NO == utilityviewmodel.account_no) &&
                        (bills_row.ACCOUNT_CREATED == utilityviewmodel.account_created))
                    {
                        if (!string.IsNullOrEmpty(rowAsString))
                        {
                            // Separate each record with this
                            rowAsString += SmartParametersV2016.recordSeparator;
                        }

                        SmartUtility.BillsSQLite abc = ConvertUtilityBills(ourviewmodel, bills_row, deriveds.Length);
                        utilityviewmodel.Hezbollah.sqlite_billsList.Add(abc);
                        rowAsString += SmartNibbyV2016.Build_StringNew<SmartUtility.BillsSQLite>(myFields, abc, sqliteformat);
                        bills_temp.Add(bills_row);
                        utilityviewmodel.Hezbollah.bills_changesList.Remove(bills_row);
                    }
                }
                if (!string.IsNullOrEmpty(rowAsString))
                {
                    // Results have been passed in to be updated from local table
                    utilityviewmodel.bollocks.AppendLine(deriveds +
                                            SmartParametersV2016.groupSeparator +
                                            "I" + SmartParametersV2016.groupSeparator +
                                            rowAsString);
                    foreach (SmartUtility.Bills temp_row in bills_temp)
                    {
                        utilityviewmodel.Hezbollah.billsList.Add(temp_row);//  Range(bills_temp);
                    }
                    // !! Can't use RemoveRange here but .. what the fuck!!?!!
                    utilityviewmodel.Hezbollah.bills_tempList.Clear();
                    utilityviewmodel.Hezbollah.bills_tempList.AddRange(bills_temp);
                }
                utilityviewmodel.Hezbollah.bills_changesList.Clear();
            }
            return;
        }

        internal static SmartUtility.BillsSQLite ConvertUtilityBills(MainViewModel ourviewmodel,
                                                                SmartUtility.Bills bills_row,
                                                                int derived)
        {
            // Encrypt all of these into DETAILS with randomkey
            string key_details_unencrypted = bills_row.ACCOUNT_NO +
                                        SmartParametersV2016.fieldSeparator +
                                        bills_row.STATEMENT_ID +
                                        SmartParametersV2016.fieldSeparator +
                                        bills_row.BILL_DATE.ToString(SmartParametersV2016.sqliteformat);

            string key_details = (ourviewmodel.UDEK == "" ?
                key_details_unencrypted : SmartEncryptionV2016.DoTheBiz(key_details_unencrypted, SmartPhyllV2020.BuildEncryptKey(ourviewmodel.UDEK, bills_row.RANDOMKEY1, derived), "", true));

            string details_unencrypted = bills_row.BILL_PERIOD_START.ToString(SmartParametersV2016.sqliteformat) +
                                        SmartParametersV2016.fieldSeparator +
                                        bills_row.BILL_PERIOD_END.ToString(SmartParametersV2016.sqliteformat) +
                                        SmartParametersV2016.fieldSeparator +
                                        bills_row.PAYMENT_PLAN.ToString() +
                                        SmartParametersV2016.fieldSeparator +
                                        bills_row.RESOURCE_BALANCES.ToString() +
                                        SmartParametersV2016.fieldSeparator +
                                        bills_row.PREVIOUS_BALANCE.ToString() +       // Well, we can (in theory) calculate these
                                        SmartParametersV2016.fieldSeparator +
                                        bills_row.PAYMENTS_RECEIVED.ToString() +      // OB = OB - PAYMENTS_RECEIVED three from all the data we have from a Bill
                                        SmartParametersV2016.fieldSeparator +
                                        bills_row.ACCOUNT_CHARGES_CREDITS.ToString() + // Rebates go in here and are credited to your account
                                        SmartParametersV2016.fieldSeparator +
                                        bills_row.ACCOUNT_CHARGES_CREDITS_VAT_AMOUNT.ToString() +
                                        SmartParametersV2016.fieldSeparator +
                                        bills_row.SUPPLY_CHARGES_CREDITS.ToString() +
                                        SmartParametersV2016.fieldSeparator +
                                        bills_row.SUPPLY_CHARGES_CREDITS_VAT_AMOUNT.ToString() +
                                        SmartParametersV2016.fieldSeparator +
                                        bills_row.BILL_VAT_AMOUNT.ToString() +        // For the entire Bill (if we can find it) e.g. for SP so we can split it up later because of ONE FUCKING PENNY
                                        SmartParametersV2016.fieldSeparator +
                                        bills_row.BILL_VAT_CODE.ToString() +
                                        SmartParametersV2016.fieldSeparator +
                                        bills_row.OUTSTANDING_BALANCE.ToString() +    // But I will leave them in as a check
                                        SmartParametersV2016.fieldSeparator +
                                        bills_row.TOTAL_NOW_DUE.ToString() +          // Which may be different from OB
                                        SmartParametersV2016.fieldSeparator +
                                        bills_row.MONTHLY_PAYMENT.ToString() +       // May only be used by Scottish Power??
                                        SmartParametersV2016.fieldSeparator +
                                        bills_row.PAYMENT_DUE_DATE.ToString(SmartParametersV2016.sqliteformat) +       // Latest date due
                                        SmartParametersV2016.fieldSeparator +
                                        bills_row.PAYMENT_TYPE +           // This text may not match the Payment Plan, but it may be relevant
                                        SmartParametersV2016.fieldSeparator +
                                        bills_row.DIRECT_DEBIT_DATE.ToString(SmartParametersV2016.sqliteformat) +
                                        SmartParametersV2016.fieldSeparator +
                                        bills_row.LOYALTY_BONUS.ToString() +
                                        SmartParametersV2016.fieldSeparator +
                                        bills_row.DISCOUNT_CREDIT_DATE.ToString(SmartParametersV2016.sqliteformat) +
                                        SmartParametersV2016.fieldSeparator +
                                        bills_row.FIRST_YEARS_DISCOUNT +
                                        SmartParametersV2016.fieldSeparator +
                                        bills_row.REWARDS;                // SSE Only I think
            // Time for a new RandomKey
            bills_row.RANDOMKEY2 = SmartRoutinesV2018.GetNextRandomNew(ourviewmodel.randomR);
            string details = (ourviewmodel.UDEK == "" ?
                details_unencrypted : SmartEncryptionV2016.DoTheBiz(details_unencrypted, SmartPhyllV2020.BuildEncryptKey(ourviewmodel.UDEK, bills_row.RANDOMKEY2, derived), "", true));

            SmartUtility.BillsSQLite bills_enc = new SmartUtility.BillsSQLite()
            {
                USERNAME = bills_row.USERNAME,
                CUBEFACE_CODE = bills_row.CUBEFACE_CODE.ToString(),
                SUPPLIER_CODE = bills_row.SUPPLIER_CODE,
                BRAND_CODE = bills_row.BRAND_CODE,
                ACCOUNT_CREATED = bills_row.ACCOUNT_CREATED,
                KEY_DETAILS = key_details,
                RANDOMKEY1 = bills_row.RANDOMKEY1,
                DETAILS = details,
                RANDOMKEY2 = bills_row.RANDOMKEY2,
                Updated = bills_row.Updated
            };
            return bills_enc;
        }

        internal static void DoAllBillsResource(MainViewModel ourviewmodel,
                                                UtilityViewModel utilityviewmodel,
                                                string sqliteformat,
                                                short supplier_code,
                                                short brand_code)
        {
            string deriveds = "SmartUtility.BillsResource";
            utilityviewmodel.Hezbollah.sqlite_bills_resourceList.Clear();
            List<SmartUtility.BillsResource> bills_resource_temp = new List<SmartUtility.BillsResource>();

            // All these depend on the Bill (or Bills going in)
            if (utilityviewmodel.Hezbollah.bills_resource_changesList.Count > 0)
            {
                FieldInfo[] myFields = typeof(SmartUtility.BillsResourceSQLite).GetFields(SmartParametersV2016.bindingFlags);
                string rowAsString = "";

                // Clear this down
                bills_resource_temp.Clear();

                foreach (SmartUtility.BillsResource bills_resource_row in utilityviewmodel.Hezbollah.bills_resource_changesList)
                {
                    if ((bills_resource_row.CUBEFACE_CODE == SmartParametersV2016.Utility) &&
                        (bills_resource_row.SUPPLIER_CODE == supplier_code) &&
                        (bills_resource_row.BRAND_CODE == brand_code) &&
                        (bills_resource_row.ACCOUNT_NO == utilityviewmodel.account_no) &&
                        (bills_resource_row.ACCOUNT_CREATED == utilityviewmodel.account_created) &&
                        (bills_resource_row.MPAN_MPRN == utilityviewmodel.mpan_mprn))
                    {
                        if (!string.IsNullOrEmpty(rowAsString))
                        {
                            // Separate each record with this
                            rowAsString += SmartParametersV2016.recordSeparator;
                        }
                        SmartUtility.BillsResourceSQLite abc = ConvertUtilityBillsResource(ourviewmodel, bills_resource_row, deriveds.Length);
                        utilityviewmodel.Hezbollah.sqlite_bills_resourceList.Add(abc);
                        rowAsString += SmartNibbyV2016.Build_StringNew<SmartUtility.BillsResourceSQLite>(myFields, abc, sqliteformat);
                        bills_resource_temp.Add(bills_resource_row);
                        utilityviewmodel.Hezbollah.bills_resource_changesList.Remove(bills_resource_row);
                    }
                }
                if (!string.IsNullOrEmpty(rowAsString))
                {
                    // Results have been passed in to be updated from local table
                    utilityviewmodel.bollocks.AppendLine(deriveds +
                                            SmartParametersV2016.groupSeparator +
                                            "I" + SmartParametersV2016.groupSeparator +
                                            rowAsString);
                    foreach (SmartUtility.BillsResource temp_row in bills_resource_temp)
                    {
                        utilityviewmodel.Hezbollah.bills_resourceList.Add(temp_row); // Range(bills_resource_temp);
                    }
                    // !! Can't use RemoveRange here but .. what the fuck!!?!!
                    utilityviewmodel.Hezbollah.bills_resource_tempList.Clear();
                    utilityviewmodel.Hezbollah.bills_resource_tempList.AddRange(bills_resource_temp);
                }
                utilityviewmodel.Hezbollah.bills_resource_changesList.Clear();
            }
            return;
        }

        internal static SmartUtility.BillsResourceSQLite ConvertUtilityBillsResource(MainViewModel ourviewmodel,
                                                                SmartUtility.BillsResource billsresource_row,
                                                                int derived)
        {
            // Encrypt all of these into DETAILS with randomkey
            string key_details_unencrypted = billsresource_row.ACCOUNT_NO +
                                        SmartParametersV2016.fieldSeparator +
                                        billsresource_row.STATEMENT_ID +
                                        SmartParametersV2016.fieldSeparator +
                                        billsresource_row.BILL_DATE.ToString(SmartParametersV2016.sqliteformat) +
                                        SmartParametersV2016.fieldSeparator +
                                        billsresource_row.MPAN_MPRN;

            string key_details = (ourviewmodel.UDEK == "" ?
                key_details_unencrypted : SmartEncryptionV2016.DoTheBiz(key_details_unencrypted, SmartPhyllV2020.BuildEncryptKey(ourviewmodel.UDEK, billsresource_row.RANDOMKEY1, derived), "", true));

            string details_unencrypted = billsresource_row.RESOURCE_ACCOUNT_NO +
                                        SmartParametersV2016.fieldSeparator +
                                        billsresource_row.TARIFF_CODE.ToString() +
                                        SmartParametersV2016.fieldSeparator +
                                        billsresource_row.NEW_CHARGES.ToString() +
                                        SmartParametersV2016.fieldSeparator +
                                        billsresource_row.RESOURCE_DISCOUNTS.ToString() +
                                        SmartParametersV2016.fieldSeparator +
                                        billsresource_row.RESOURCE_VAT_AMOUNT.ToString() +
                                        SmartParametersV2016.fieldSeparator +
                                        billsresource_row.RESOURCE_VAT_CODE.ToString() +
                                        SmartParametersV2016.fieldSeparator +
                                        billsresource_row.PREVIOUS_RESOURCE_BALANCE.ToString() +
                                        SmartParametersV2016.fieldSeparator +
                                        billsresource_row.OUTSTANDING_RESOURCE_BALANCE.ToString();
            // Time for a new RandomKey
            billsresource_row.RANDOMKEY2 = SmartRoutinesV2018.GetNextRandomNew(ourviewmodel.randomR);
            string details = (ourviewmodel.UDEK == "" ?
                details_unencrypted : SmartEncryptionV2016.DoTheBiz(details_unencrypted, SmartPhyllV2020.BuildEncryptKey(ourviewmodel.UDEK, billsresource_row.RANDOMKEY2, derived), "", true));

            SmartUtility.BillsResourceSQLite billsresource_enc = new SmartUtility.BillsResourceSQLite()
            {
                USERNAME = billsresource_row.USERNAME,
                CUBEFACE_CODE = billsresource_row.CUBEFACE_CODE.ToString(),
                SUPPLIER_CODE = billsresource_row.SUPPLIER_CODE,
                BRAND_CODE = billsresource_row.BRAND_CODE,
                ACCOUNT_CREATED = billsresource_row.ACCOUNT_CREATED,
                KEY_DETAILS = key_details,
                RANDOMKEY1 = billsresource_row.RANDOMKEY1,
                DETAILS = details,
                RANDOMKEY2 = billsresource_row.RANDOMKEY2,
                Updated = billsresource_row.Updated
            };
            return billsresource_enc;
        }
        internal static void DoAllPayments(MainViewModel ourviewmodel,
                                                UtilityViewModel utilityviewmodel,
                                                string sqliteformat,
                                                short supplier_code,
                                                short brand_code)
        {
            string deriveds = "SmartUtility.Payments";
            utilityviewmodel.Hezbollah.sqlite_paymentsList.Clear();
            // Put any new Payments (found on Bills) and unallocated Payments (not found in a Bill) back into the table
            if (utilityviewmodel.Hezbollah.payments_changesList.Count > 0)
            {
                // Now insert all the new payments
                FieldInfo[] myFields = typeof(SmartUtility.PaymentsSQLite).GetFields(SmartParametersV2016.bindingFlags);
                string rowAsString = "";
                List<SmartUtility.Payments> payments_temp = new List<SmartUtility.Payments>();
                foreach (SmartUtility.Payments payments_row in utilityviewmodel.Hezbollah.payments_changesList)
                {
                    if ((payments_row.CUBEFACE_CODE == SmartParametersV2016.Utility) &&
                        (payments_row.SUPPLIER_CODE == supplier_code) &&
                        (payments_row.BRAND_CODE == brand_code) &&
                        (payments_row.ACCOUNT_CREATED == utilityviewmodel.account_created) &&
                        (payments_row.ACCOUNT_NO == utilityviewmodel.account_no))
                    {
                        if (!string.IsNullOrEmpty(rowAsString))
                        {
                            // Separate each record with this
                            rowAsString += SmartParametersV2016.recordSeparator;
                        }
                        SmartUtility.PaymentsSQLite abc = ConvertUtilityPayments(ourviewmodel, payments_row, deriveds.Length);
                        utilityviewmodel.Hezbollah.sqlite_paymentsList.Add(abc);
                        rowAsString += SmartNibbyV2016.Build_StringNew<SmartUtility.PaymentsSQLite>(myFields, abc, sqliteformat);
                        payments_temp.Add(payments_row);
                    }
                }
                if (!string.IsNullOrEmpty(rowAsString))
                {
                    utilityviewmodel.bollocks.AppendLine(deriveds +
                                        SmartParametersV2016.groupSeparator +
                                       "I" + SmartParametersV2016.groupSeparator +
                                       rowAsString);
                    foreach (SmartUtility.Payments temp_row in payments_temp)
                    {
                        utilityviewmodel.Hezbollah.paymentsList.Add(temp_row); // Range(payments_temp);
                    }
                    // !! Can't use RemoveRange here but .. what the fuck!!?!!
                    //foreach (SmartUtility.Payments payments_row in payments_temp)
                    //{
                    //    utilityviewmodel.Hezbollah.payments_changesList.Remove(payments_row);
                    //}
                }
                utilityviewmodel.Hezbollah.payments_changesList.Clear();
            }
            return;
        }

        internal static SmartUtility.PaymentsSQLite ConvertUtilityPayments(MainViewModel ourviewmodel,
                                                                SmartUtility.Payments payments_row,
                                                                int derived)
        {
            // Encrypt all of these into DETAILS with randomkey
            string key_details_unencrypted = payments_row.ACCOUNT_NO +
                                        SmartParametersV2016.fieldSeparator +
                                        payments_row.STATEMENT_ID +
                                        SmartParametersV2016.fieldSeparator +
                                        payments_row.BILL_DATE.ToString(SmartParametersV2016.sqliteformat) +
                                        SmartParametersV2016.fieldSeparator +
                                        payments_row.PAYMENT_DATE.ToString(SmartParametersV2016.sqliteformat) +
                                        SmartParametersV2016.fieldSeparator +
                                        payments_row.PAYMENT_ITEM.ToString();

            string key_details = (ourviewmodel.UDEK == "" ?
                key_details_unencrypted : SmartEncryptionV2016.DoTheBiz(key_details_unencrypted, SmartPhyllV2020.BuildEncryptKey(ourviewmodel.UDEK, payments_row.RANDOMKEY1, derived), "", true));

            string details_unencrypted = payments_row.PAYMENT_CODE.ToString() +
                                        SmartParametersV2016.fieldSeparator +
                                        payments_row.PAYMENT_AMOUNT.ToString() +
                                        SmartParametersV2016.fieldSeparator +
                                        payments_row.PAYMENT_BALANCE.ToString();
            // Time for a new RandomKey
            payments_row.RANDOMKEY2 = SmartRoutinesV2018.GetNextRandomNew(ourviewmodel.randomR);
            string details = (ourviewmodel.UDEK == "" ?
                details_unencrypted : SmartEncryptionV2016.DoTheBiz(details_unencrypted, SmartPhyllV2020.BuildEncryptKey(ourviewmodel.UDEK, payments_row.RANDOMKEY2, derived), "", true));

            SmartUtility.PaymentsSQLite payments_enc = new SmartUtility.PaymentsSQLite()
            {
                USERNAME = payments_row.USERNAME,
                CUBEFACE_CODE = payments_row.CUBEFACE_CODE.ToString(),
                SUPPLIER_CODE = payments_row.SUPPLIER_CODE,
                BRAND_CODE = payments_row.BRAND_CODE,
                ACCOUNT_CREATED = payments_row.ACCOUNT_CREATED,
                KEY_DETAILS = key_details,
                RANDOMKEY1 = payments_row.RANDOMKEY1,
                DETAILS = details,
                RANDOMKEY2 = payments_row.RANDOMKEY2,
                Updated = payments_row.Updated
            };
            return payments_enc;
        }

        internal static void DoAllUnallocated(MainViewModel ourviewmodel,
                                            UtilityViewModel utilityviewmodel,
                                            bool deleteUnallocatedPayments,
                                            string sqliteformat,
                                            short supplier_code,
                                            short brand_code)
        {
            string deriveds = "SmartUtility.Unallocated";
            utilityviewmodel.Hezbollah.sqlite_unallocatedList.Clear();
            // Put any new Payments (found on Bills) and unallocated Payments (not found in a Bill) back into the table
            if (utilityviewmodel.Hezbollah.unallocated_changesList.Count > 0)
            {
                // Delete all Payments not allocated to a Bill
                if (deleteUnallocatedPayments)
                {
                    string sql = "DELETE FROM " + "Unallocated" +          // Same as above - might need a schema if not added auto ..think it will ALWAYS need a schema!
                                " WHERE USERNAME = '" + ourviewmodel.UserName + "'" +
                                " AND CUBEFACE_CODE = '" + SmartParametersV2016.Utility + "'" +
                                " AND SUPPLIER_CODE = " + supplier_code +
                                " AND BRAND_CODE = " + brand_code +
                                " AND CREATED = '" + utilityviewmodel.account_created.ToString(sqliteformat) + "'" +
                                " AND ACCOUNT_NO = '" + utilityviewmodel.account_no + "'" +
                                " AND STATEMENT_ID = ''";
                    utilityviewmodel.bollocks.AppendLine(deriveds +
                                        SmartParametersV2016.groupSeparator +
                                        "D" + SmartParametersV2016.groupSeparator +
                                        sql);
                }

                // Now insert all the new unallocated payments
                FieldInfo[] myFields = typeof(SmartUtility.UnallocatedSQLite).GetFields(SmartParametersV2016.bindingFlags);
                string rowAsString = "";
                List<SmartUtility.Unallocated> unallocated_temp = new List<SmartUtility.Unallocated>();
                foreach (SmartUtility.Unallocated unallocated_row in utilityviewmodel.Hezbollah.unallocated_changesList)
                {
                    if ((unallocated_row.USERNAME == ourviewmodel.UserName) &&
                        (unallocated_row.CUBEFACE_CODE == SmartParametersV2016.Utility) &&
                        (unallocated_row.SUPPLIER_CODE == supplier_code) &&
                        (unallocated_row.BRAND_CODE == brand_code) &&
                        (unallocated_row.ACCOUNT_CREATED == utilityviewmodel.account_created) &&
                        (unallocated_row.ACCOUNT_NO == utilityviewmodel.account_no))
                    {
                        if (!string.IsNullOrEmpty(rowAsString))
                        {
                            // Separate each recolrd with this
                            rowAsString += SmartParametersV2016.recordSeparator;
                        }
                        SmartUtility.UnallocatedSQLite abc = ConvertUtilityUnallocated(ourviewmodel, unallocated_row, deriveds.Length);
                        utilityviewmodel.Hezbollah.sqlite_unallocatedList.Add(abc);
                        rowAsString += SmartNibbyV2016.Build_StringNew<SmartUtility.UnallocatedSQLite>(myFields, abc, sqliteformat);
                        unallocated_temp.Add(unallocated_row);
                    }
                }
                if (!string.IsNullOrEmpty(rowAsString))
                {
                    utilityviewmodel.bollocks.AppendLine(deriveds +
                                        SmartParametersV2016.groupSeparator +
                                       "I" + SmartParametersV2016.groupSeparator +
                                       rowAsString);
                    foreach (SmartUtility.Unallocated my_row in unallocated_temp)
                    {
                        utilityviewmodel.Hezbollah.unallocatedList.Add(my_row); // AddRange(unallocated_temp);
                    }
                    //!! Can't use RemoveRange here but .. what the fuck!!?!!
                    //foreach (SmartUtility.Unallocated unallocated_row in unallocated_temp)
                    //{
                    //    utilityviewmodel.Hezbollah.unallocated_changesList.Remove(unallocated_row);
                    //}
                }
                utilityviewmodel.Hezbollah.unallocated_changesList.Clear();
            }
            return;
        }

        internal static SmartUtility.UnallocatedSQLite ConvertUtilityUnallocated(MainViewModel ourviewmodel,
                                                                SmartUtility.Unallocated unallocated_row,
                                                                int derived)
        {
            // Encrypt all of these into DETAILS with randomkey
            string key_details_unencrypted = unallocated_row.ACCOUNT_NO +
                                        SmartParametersV2016.fieldSeparator +
                                        unallocated_row.STATEMENT_ID +
                                        SmartParametersV2016.fieldSeparator +
                                        unallocated_row.BILL_DATE.ToString(SmartParametersV2016.sqliteformat) +
                                        SmartParametersV2016.fieldSeparator +
                                        unallocated_row.PAYMENT_DATE.ToString(SmartParametersV2016.sqliteformat) +
                                        SmartParametersV2016.fieldSeparator +
                                        unallocated_row.PAYMENT_ITEM.ToString();

            string key_details = (ourviewmodel.UDEK == "" ?
                key_details_unencrypted : SmartEncryptionV2016.DoTheBiz(key_details_unencrypted, SmartPhyllV2020.BuildEncryptKey(ourviewmodel.UDEK, unallocated_row.RANDOMKEY1, derived), "", true));

            string details_unencrypted = unallocated_row.PAYMENT_CODE.ToString() +
                                        SmartParametersV2016.fieldSeparator +
                                        unallocated_row.PAYMENT_AMOUNT.ToString() +
                                        SmartParametersV2016.fieldSeparator +
                                        unallocated_row.PAYMENT_BALANCE.ToString();
            // Time for a new RandomKey
            unallocated_row.RANDOMKEY2 = SmartRoutinesV2018.GetNextRandomNew(ourviewmodel.randomR);
            string details = (ourviewmodel.UDEK == "" ?
                details_unencrypted : SmartEncryptionV2016.DoTheBiz(details_unencrypted, SmartPhyllV2020.BuildEncryptKey(ourviewmodel.UDEK, unallocated_row.RANDOMKEY2, derived), "", true));

            SmartUtility.UnallocatedSQLite unallocated_enc = new SmartUtility.UnallocatedSQLite()
            {
                USERNAME = unallocated_row.USERNAME,
                CUBEFACE_CODE = unallocated_row.CUBEFACE_CODE.ToString(),
                SUPPLIER_CODE = unallocated_row.SUPPLIER_CODE,
                BRAND_CODE = unallocated_row.BRAND_CODE,
                ACCOUNT_CREATED = unallocated_row.ACCOUNT_CREATED,
                KEY_DETAILS = key_details,
                RANDOMKEY1 = unallocated_row.RANDOMKEY1,
                DETAILS = details,
                RANDOMKEY2 = unallocated_row.RANDOMKEY2,
                Updated = unallocated_row.Updated
            };
            return unallocated_enc;
        }

        internal static void DoAllAccountCharges(MainViewModel ourviewmodel,
                                            UtilityViewModel utilityviewmodel,
                                            //bool delete_unallocated_payments,
                                            string sqliteformat,
                                            short supplier_code,
                                            short brand_code)
        {
            string deriveds = "SmartUtility.AccChargesCredits";

            utilityviewmodel.Hezbollah.sqlite_account_charges_creditsList.Clear();
            // Account charges and credits (depends on Bills)
            if (utilityviewmodel.Hezbollah.account_charges_credits_changesList.Count > 0)
            {
                FieldInfo[] myFields = typeof(SmartUtility.AccChargesCreditsSQLite).GetFields(SmartParametersV2016.bindingFlags);
                string rowAsString = "";

                foreach (SmartUtility.Bills bills_row in utilityviewmodel.Hezbollah.bills_tempList)    // We don't have bills_out anymore
                {
                    foreach (SmartUtility.AccChargesCredits account_charges_credits_row in utilityviewmodel.Hezbollah.account_charges_credits_changesList)
                    {
                        if ((account_charges_credits_row.USERNAME == bills_row.USERNAME) &&
                            (account_charges_credits_row.CUBEFACE_CODE == SmartParametersV2016.Utility) &&
                            (account_charges_credits_row.SUPPLIER_CODE == supplier_code) &&
                            (account_charges_credits_row.BRAND_CODE == brand_code) &&
                            (account_charges_credits_row.ACCOUNT_NO == utilityviewmodel.account_no) &&
                            (account_charges_credits_row.ACCOUNT_CREATED == utilityviewmodel.account_created) &&
                            (account_charges_credits_row.STATEMENT_ID == bills_row.STATEMENT_ID) &&
                            (account_charges_credits_row.BILL_DATE == bills_row.BILL_DATE))
                        {
                            if (!string.IsNullOrEmpty(rowAsString))
                            {
                                // Separate each record with this
                                rowAsString += SmartParametersV2016.recordSeparator;
                            }
                            SmartUtility.AccChargesCreditsSQLite abc = ConvertUtilityAccChargesCredits(ourviewmodel, account_charges_credits_row, deriveds.Length);
                            utilityviewmodel.Hezbollah.sqlite_account_charges_creditsList.Add(abc);
                            rowAsString += SmartNibbyV2016.Build_StringNew<SmartUtility.AccChargesCreditsSQLite>(myFields, abc, sqliteformat);
                        }
                    }
                }
                if (!string.IsNullOrEmpty(rowAsString))
                {
                    utilityviewmodel.bollocks.AppendLine(deriveds +
                                        SmartParametersV2016.groupSeparator +
                                        "I" + SmartParametersV2016.groupSeparator +
                                        rowAsString);
                }
                utilityviewmodel.Hezbollah.account_charges_credits_changesList.Clear();
            }
            return;
        }

        internal static SmartUtility.AccChargesCreditsSQLite ConvertUtilityAccChargesCredits(MainViewModel ourviewmodel,
                                                        SmartUtility.AccChargesCredits accchargescredits_row,
                                                        int derived)
        {
            // Encrypt all of these into DETAILS with randomkey
            string key_details_unencrypted = accchargescredits_row.ACCOUNT_NO +
                                        SmartParametersV2016.fieldSeparator +
                                        accchargescredits_row.STATEMENT_ID +
                                        SmartParametersV2016.fieldSeparator +
                                        accchargescredits_row.BILL_DATE.ToString(SmartParametersV2016.sqliteformat) +
                                        SmartParametersV2016.fieldSeparator +
                                        accchargescredits_row.ACCOUNT_CREATED.ToString(SmartParametersV2016.sqliteformat) +
                                        SmartParametersV2016.fieldSeparator +
                                        accchargescredits_row.ACCOUNT_ITEM.ToString();

            string key_details = (ourviewmodel.UDEK == "" ?
                key_details_unencrypted : SmartEncryptionV2016.DoTheBiz(key_details_unencrypted, SmartPhyllV2020.BuildEncryptKey(ourviewmodel.UDEK, accchargescredits_row.RANDOMKEY1, derived), "", true));

            string details_unencrypted = accchargescredits_row.ACCOUNT_TYPE +
                                        SmartParametersV2016.fieldSeparator +
                                        accchargescredits_row.ACCOUNT_VAT_CODE.ToString() +
                                        SmartParametersV2016.fieldSeparator +
                                        accchargescredits_row.ACCOUNT_AMOUNT.ToString() +
                                        SmartParametersV2016.fieldSeparator +
                                        accchargescredits_row.INCLUDE_BILLS;
            // Time for a new RandomKey
            accchargescredits_row.RANDOMKEY2 = SmartRoutinesV2018.GetNextRandomNew(ourviewmodel.randomR);
            string details = (ourviewmodel.UDEK == "" ?
                details_unencrypted : SmartEncryptionV2016.DoTheBiz(details_unencrypted, SmartPhyllV2020.BuildEncryptKey(ourviewmodel.UDEK, accchargescredits_row.RANDOMKEY2, derived), "", true));

            SmartUtility.AccChargesCreditsSQLite accchargescredits_enc = new SmartUtility.AccChargesCreditsSQLite()
            {
                USERNAME = accchargescredits_row.USERNAME,
                CUBEFACE_CODE = accchargescredits_row.CUBEFACE_CODE.ToString(),
                SUPPLIER_CODE = accchargescredits_row.SUPPLIER_CODE,
                BRAND_CODE = accchargescredits_row.BRAND_CODE,
                ACCOUNT_CREATED = accchargescredits_row.ACCOUNT_CREATED,
                KEY_DETAILS = key_details,
                RANDOMKEY1 = accchargescredits_row.RANDOMKEY1,
                DETAILS = details,
                RANDOMKEY2 = accchargescredits_row.RANDOMKEY2,
                Updated = accchargescredits_row.Updated
            };
            return accchargescredits_enc;
        }

        internal static void DoAllSupplyCharges(MainViewModel ourviewmodel,
                                            UtilityViewModel utilityviewmodel,
                                            string sqliteformat,
                                            short supplier_code,
                                            short brand_code)
        {
            string deriveds = "SmartUtility.SupChargesCredits";

            utilityviewmodel.Hezbollah.sqlite_supply_charges_creditsList.Clear();
            // Supply charges and credits (depends on Bills)
            if (utilityviewmodel.Hezbollah.supply_charges_credits_changesList.Count > 0)
            {
                FieldInfo[] myFields = typeof(SmartUtility.SupChargesCredits).GetFields(SmartParametersV2016.bindingFlags);
                string rowAsString = "";
                List<SmartUtility.SupChargesCredits> supply_charges_credits_temp = new List<SmartUtility.SupChargesCredits>();
                foreach (SmartUtility.Bills bills_row in utilityviewmodel.Hezbollah.bills_tempList)  // We don't have bills_out anymore
                {
                    foreach (SmartUtility.SupChargesCredits supply_charges_credits_row in utilityviewmodel.Hezbollah.supply_charges_credits_changesList)
                    {
                        if (//(supply_charges_credits_row.USERNAME == bills_row.USERNAME) &&
                            (supply_charges_credits_row.CUBEFACE_CODE == SmartParametersV2016.Utility) &&
                            (supply_charges_credits_row.SUPPLIER_CODE == supplier_code) &&
                            (supply_charges_credits_row.BRAND_CODE == brand_code) &&
                            (supply_charges_credits_row.ACCOUNT_NO == utilityviewmodel.account_no) &&
                            (supply_charges_credits_row.ACCOUNT_CREATED == utilityviewmodel.account_created) &&
                            (supply_charges_credits_row.STATEMENT_ID == bills_row.STATEMENT_ID) &&
                            (supply_charges_credits_row.BILL_DATE == bills_row.BILL_DATE))
                        {
                            if (!string.IsNullOrEmpty(rowAsString))
                            {
                                // Separate each record with this
                                rowAsString += SmartParametersV2016.recordSeparator;
                            }
                            SmartUtility.SupChargesCreditsSQLite abc = ConvertUtilitySupChargesCredits(ourviewmodel, supply_charges_credits_row, deriveds.Length);
                            utilityviewmodel.Hezbollah.sqlite_supply_charges_creditsList.Add(abc);
                            rowAsString += SmartNibbyV2016.Build_StringNew<SmartUtility.SupChargesCreditsSQLite>(myFields, abc, sqliteformat);
                            supply_charges_credits_temp.Add(supply_charges_credits_row);
                        }
                    }
                }
                if (!string.IsNullOrEmpty(rowAsString))
                {
                    utilityviewmodel.bollocks.AppendLine(deriveds +
                                        SmartParametersV2016.groupSeparator +
                                        "I" + SmartParametersV2016.groupSeparator +
                                        rowAsString);
                    foreach (SmartUtility.SupChargesCredits my_row in supply_charges_credits_temp)
                    {
                        utilityviewmodel.Hezbollah.supply_charges_creditsList.Add(my_row); // Range(supply_charges_credits_temp);
                    }
                    // !!
                    //foreach (SmartUtility.SupChargesCredits supply_charges_credits_row in supply_charges_credits_temp)
                    //{
                    //    utilityviewmodel.Hezbollah.supply_charges_credits_changesList.Remove(supply_charges_credits_row);
                    //}
                }
                utilityviewmodel.Hezbollah.supply_charges_credits_changesList.Clear();
            }
            return;
        }

        internal static SmartUtility.SupChargesCreditsSQLite ConvertUtilitySupChargesCredits(MainViewModel ourviewmodel,
                                                        SmartUtility.SupChargesCredits supchargescredits_row,
                                                        int derived)
        {
            // Encrypt all of these into DETAILS with randomkey
            string key_details_unencrypted = supchargescredits_row.ACCOUNT_NO +
                                        SmartParametersV2016.fieldSeparator +
                                        supchargescredits_row.STATEMENT_ID +
                                        SmartParametersV2016.fieldSeparator +
                                        supchargescredits_row.BILL_DATE.ToString(SmartParametersV2016.sqliteformat) +
                                        SmartParametersV2016.fieldSeparator +
                                        supchargescredits_row.SUPPLY_DATE.ToString(SmartParametersV2016.sqliteformat) +
                                        SmartParametersV2016.fieldSeparator +
                                        supchargescredits_row.SUPPLY_ITEM.ToString();

            string key_details = (ourviewmodel.UDEK == "" ?
                key_details_unencrypted : SmartEncryptionV2016.DoTheBiz(key_details_unencrypted, SmartPhyllV2020.BuildEncryptKey(ourviewmodel.UDEK, supchargescredits_row.RANDOMKEY1, derived), "", true));

            string details_unencrypted = supchargescredits_row.SUPPLY_TYPE +
                                        SmartParametersV2016.fieldSeparator +
                                        supchargescredits_row.SUPPLY_VAT_CODE.ToString() +
                                        SmartParametersV2016.fieldSeparator +
                                        supchargescredits_row.SUPPLY_AMOUNT.ToString() +
                                        SmartParametersV2016.fieldSeparator +
                                        supchargescredits_row.SUPPLY_DUE_DATE.ToString(SmartParametersV2016.sqliteformat) +
                                        SmartParametersV2016.fieldSeparator +
                                        supchargescredits_row.SUPPLY_AMOUNT.ToString() +
                                        SmartParametersV2016.fieldSeparator +
                                        supchargescredits_row.SUPPLY_CREDIT_BILL;
            // Time for a new RandomKey
            supchargescredits_row.RANDOMKEY2 = SmartRoutinesV2018.GetNextRandomNew(ourviewmodel.randomR);
            string details = (ourviewmodel.UDEK == "" ?
                details_unencrypted : SmartEncryptionV2016.DoTheBiz(details_unencrypted, SmartPhyllV2020.BuildEncryptKey(ourviewmodel.UDEK, supchargescredits_row.RANDOMKEY2, derived), "", true));

            SmartUtility.SupChargesCreditsSQLite supchargescredits_enc = new SmartUtility.SupChargesCreditsSQLite()
            {
                USERNAME = supchargescredits_row.USERNAME,
                CUBEFACE_CODE = supchargescredits_row.CUBEFACE_CODE.ToString(),
                SUPPLIER_CODE = supchargescredits_row.SUPPLIER_CODE,
                BRAND_CODE = supchargescredits_row.BRAND_CODE,
                ACCOUNT_CREATED = supchargescredits_row.ACCOUNT_CREATED,
                KEY_DETAILS = key_details,
                RANDOMKEY1 = supchargescredits_row.RANDOMKEY1,
                DETAILS = details,
                RANDOMKEY2 = supchargescredits_row.RANDOMKEY2,
                Updated = supchargescredits_row.Updated
            };
            return supchargescredits_enc;
        }

        internal static void DoAllTariffDetails(MainViewModel ourviewmodel,
                                            UtilityViewModel utilityviewmodel,
                                            string sqliteformat,
                                            short supplier_code,
                                            short brand_code)
        {
            string deriveds = "SmartUtility.TariffDetails";
            utilityviewmodel.Hezbollah.sqlite_tariff_detailsList.Clear();
            // Tariff details (depends on Bills)
            if (utilityviewmodel.Hezbollah.tariff_details_changesList.Count > 0)
            {
                FieldInfo[] myFields = typeof(SmartUtility.TariffDetailsSQLite).GetFields(SmartParametersV2016.bindingFlags);
                string rowAsString = "";
                List<SmartUtility.TariffDetails> TariffDetails_temp = new List<SmartUtility.TariffDetails>();
                foreach (SmartUtility.TariffDetails tariffdetails_row in utilityviewmodel.Hezbollah.tariff_details_changesList)
                {
                    if ((tariffdetails_row.CUBEFACE_CODE == SmartParametersV2016.Utility) &&
                        (tariffdetails_row.SUPPLIER_CODE == supplier_code) &&
                        (tariffdetails_row.BRAND_CODE == brand_code) &&
                        (tariffdetails_row.ACCOUNT_NO == utilityviewmodel.account_no) &&
                        (tariffdetails_row.ACCOUNT_CREATED == utilityviewmodel.account_created) &&
                        (!string.IsNullOrEmpty(tariffdetails_row.STATEMENT_ID)) &&
                        (tariffdetails_row.BILL_DATE != SmartParametersV2016.defaultDate) &&
                        (tariffdetails_row.MPAN_MPRN == utilityviewmodel.mpan_mprn) &&
                        (tariffdetails_row.PRICES_VALID_FROM != SmartParametersV2016.defaultDate) &&
                        (tariffdetails_row.PRICES_VALID_TO != SmartParametersV2016.defaultDate) &&
                        (tariffdetails_row.TARIFF_CODE > 0) &&
                        (tariffdetails_row.PAYMENT_PLAN != SmartParametersV2016.defaultChar) &&
                        (tariffdetails_row.TIER_LEVEL > 0) &&
                        (tariffdetails_row.AREA_CODE > 0) &&
                        (!string.IsNullOrEmpty(tariffdetails_row.SC)) &&
                        (!string.IsNullOrEmpty(tariffdetails_row.DR)) &&
                        (!string.IsNullOrEmpty(tariffdetails_row.NR)) &&
                        (!string.IsNullOrEmpty(tariffdetails_row.TCR)))
                    {
                        if (!string.IsNullOrEmpty(rowAsString))
                        {
                            // Separate each record with this
                            rowAsString += SmartParametersV2016.recordSeparator;
                        }
                        SmartUtility.TariffDetailsSQLite abc = ConvertUtilityTariffDetails(ourviewmodel, tariffdetails_row, deriveds.Length);
                        utilityviewmodel.Hezbollah.sqlite_tariff_detailsList.Add(abc);
                        rowAsString += SmartNibbyV2016.Build_StringNew<SmartUtility.TariffDetailsSQLite>(myFields, abc, sqliteformat);
                        TariffDetails_temp.Add(tariffdetails_row);
                    }
                }
                if (!string.IsNullOrEmpty(rowAsString))
                {
                    utilityviewmodel.bollocks.AppendLine(deriveds +
                                        SmartParametersV2016.groupSeparator +
                                        "I" + SmartParametersV2016.groupSeparator +
                                        rowAsString);
                    foreach (SmartUtility.TariffDetails my_row in TariffDetails_temp)
                    {
                        utilityviewmodel.Hezbollah.tariff_detailsList.Add(my_row); // Range(TariffDetails_temp);
                    }
                    // !!
                    //foreach (SmartUtility.TariffDetails TariffDetails_row in TariffDetails_temp)
                    //{
                    //    utilityviewmodel.Hezbollah.tariff_details_changesList.Remove(TariffDetails_row);
                    //}
                }
                utilityviewmodel.Hezbollah.tariff_details_changesList.Clear();
            }
            return;
        }

        internal static SmartUtility.TariffDetailsSQLite ConvertUtilityTariffDetails(MainViewModel ourviewmodel,
                                                        SmartUtility.TariffDetails tariffdetails_row,
                                                        int derived)
        {
            // Encrypt all of these into DETAILS with randomkey
            string key_details_unencrypted = tariffdetails_row.ACCOUNT_NO +
                                        SmartParametersV2016.fieldSeparator +
                                        tariffdetails_row.STATEMENT_ID +
                                        SmartParametersV2016.fieldSeparator +
                                        tariffdetails_row.BILL_DATE.ToString(SmartParametersV2016.sqliteformat) +
                                        SmartParametersV2016.fieldSeparator +
                                        tariffdetails_row.MPAN_MPRN +
                                        SmartParametersV2016.fieldSeparator +
                                        tariffdetails_row.PRICES_VALID_FROM.ToString(SmartParametersV2016.sqliteformat) +
                                        SmartParametersV2016.fieldSeparator +
                                        tariffdetails_row.PRICES_VALID_TO.ToString(SmartParametersV2016.sqliteformat) +
                                        SmartParametersV2016.fieldSeparator +
                                        tariffdetails_row.TARIFF_CODE.ToString() +
                                        SmartParametersV2016.fieldSeparator +
                                        tariffdetails_row.PAYMENT_PLAN.ToString() +
                                        SmartParametersV2016.fieldSeparator +
                                        tariffdetails_row.TIER_LEVEL.ToString();

            string key_details = (ourviewmodel.UDEK == "" ?
                key_details_unencrypted : SmartEncryptionV2016.DoTheBiz(key_details_unencrypted, SmartPhyllV2020.BuildEncryptKey(ourviewmodel.UDEK, tariffdetails_row.RANDOMKEY1, derived), "", true));

            string details_unencrypted = tariffdetails_row.AREA_CODE.ToString() +
                                        SmartParametersV2016.fieldSeparator +
                                        tariffdetails_row.SC +
                                        SmartParametersV2016.fieldSeparator +
                                        tariffdetails_row.DR +
                                        SmartParametersV2016.fieldSeparator +
                                        tariffdetails_row.NR +
                                        SmartParametersV2016.fieldSeparator +
                                        tariffdetails_row.TCR;
            // Time for a new RandomKey
            tariffdetails_row.RANDOMKEY2 = SmartRoutinesV2018.GetNextRandomNew(ourviewmodel.randomR);
            string details = (ourviewmodel.UDEK == "" ?
                details_unencrypted : SmartEncryptionV2016.DoTheBiz(details_unencrypted, SmartPhyllV2020.BuildEncryptKey(ourviewmodel.UDEK, tariffdetails_row.RANDOMKEY2, derived), "", true));

            SmartUtility.TariffDetailsSQLite tariffdetails_enc = new SmartUtility.TariffDetailsSQLite()
            {
                USERNAME = tariffdetails_row.USERNAME,
                CUBEFACE_CODE = tariffdetails_row.CUBEFACE_CODE.ToString(),
                SUPPLIER_CODE = tariffdetails_row.SUPPLIER_CODE,
                BRAND_CODE = tariffdetails_row.BRAND_CODE,
                ACCOUNT_CREATED = tariffdetails_row.ACCOUNT_CREATED,
                KEY_DETAILS = key_details,
                RANDOMKEY1 = tariffdetails_row.RANDOMKEY1,
                DETAILS = details,
                RANDOMKEY2 = tariffdetails_row.RANDOMKEY2,
                Updated = tariffdetails_row.Updated
            };
            return tariffdetails_enc;
        }

        internal static void DoAllECosts(MainViewModel ourviewmodel,
                                            UtilityViewModel utilityviewmodel,
                                            string sqliteformat)
        {
            string deriveds = "SmartUtility.ECosts";
            utilityviewmodel.Hezbollah.sqlite_e_costsList.Clear();
            if (utilityviewmodel.Hezbollah.e_costsList.Count > 0)
            {
                string sql = "DELETE FROM " + "ECosts" +          // Same as above - might need a schema if not added auto
                                " WHERE USERNAME = '" + ourviewmodel.UserName + "'";
                utilityviewmodel.bollocks.AppendLine(deriveds +
                                    SmartParametersV2016.groupSeparator +
                                    "D" + SmartParametersV2016.groupSeparator +
                                    sql);


                string rowAsString = "";
                FieldInfo[] myFields = typeof(SmartUtility.ECostsSQLite).GetFields(SmartParametersV2016.bindingFlags);

                //string last_usage_date = "";
                foreach (SmartUtility.ECosts e_costs_row in utilityviewmodel.Hezbollah.e_costsList)
                {
                    if (!string.IsNullOrEmpty(rowAsString))
                    {
                        // Separate each record with this
                        rowAsString += SmartParametersV2016.recordSeparator;
                    }
                    SmartUtility.ECostsSQLite abc = ConvertUtilityECosts(e_costs_row);
                    utilityviewmodel.Hezbollah.sqlite_e_costsList.Add(abc);
                    rowAsString += SmartNibbyV2016.Build_StringNew<SmartUtility.ECostsSQLite>(myFields, abc, sqliteformat);
                    // Done slightly differently than the others
                }

                // Clear this table down
                if (!string.IsNullOrEmpty(rowAsString))
                {
                    utilityviewmodel.bollocks.AppendLine(SmartParametersV2016.SmartUtilitySchema + "." + "ECosts" +
                                            SmartParametersV2016.groupSeparator +
                                            "I" + SmartParametersV2016.groupSeparator +
                                            rowAsString);
                }
                utilityviewmodel.Hezbollah.e_costsList = new List<SmartUtility.ECosts>();
            }
            return;
        }

        internal static SmartUtility.ECostsSQLite ConvertUtilityECosts(SmartUtility.ECosts ecosts_row)
        {
            SmartUtility.ECostsSQLite ecosts_enc = new SmartUtility.ECostsSQLite()
            {
                USERNAME = ecosts_row.USERNAME,
                CUBEFACE_CODE = ecosts_row.CUBEFACE_CODE.ToString(),
                RESOURCE_CODE = ecosts_row.RESOURCE_CODE.ToString(),
                UNIQUE_NUMBER = ecosts_row.UNIQUE_NUMBER,
                SUPPLIER_CODE = ecosts_row.SUPPLIER_CODE,
                BRAND_CODE = ecosts_row.BRAND_CODE,
                TARIFF_CODE = ecosts_row.TARIFF_CODE,
                TCR = ecosts_row.TCR,
                RESOURCE_TYPE = ecosts_row.RESOURCE_TYPE,
                PAYMENT_PLAN = ecosts_row.PAYMENT_PLAN.ToString(),
                TOTAL_COST = ecosts_row.TOTAL_COST,
                Updated = ecosts_row.Updated
            };
            return ecosts_enc;
        }
        internal static void DoAllEDiscounts(MainViewModel ourviewmodel,
                                                UtilityViewModel utilityviewmodel,
                                                string sqliteformat)
        {
            string deriveds = "SmartUtility.EDiscounts";
            utilityviewmodel.Hezbollah.sqlite_e_discountsList.Clear();
            if (utilityviewmodel.Hezbollah.e_discounts_changesList.Count > 0)
            {
                string rowAsString = "";
                FieldInfo[] myFields = typeof(SmartUtility.EDiscountsSQLite).GetFields(SmartParametersV2016.bindingFlags);
                List<SmartUtility.EDiscounts> e_discounts_temp = new List<SmartUtility.EDiscounts>();
                // Discounts depend on Bills
                foreach (SmartUtility.BillsResource bill_resource_row in utilityviewmodel.Hezbollah.bills_resource_tempList)
                {
                    List<SmartUtility.EDiscounts> e_discounts_found = SmartSpikeUtilityV2017.Utility_Find_EDiscounts(ourviewmodel,
                                                                                            bill_resource_row.CUBEFACE_CODE,
                                                                                            bill_resource_row.SUPPLIER_CODE,
                                                                                            bill_resource_row.BRAND_CODE,
                                                                                            bill_resource_row.ACCOUNT_NO,
                                                                                            bill_resource_row.ACCOUNT_CREATED,
                                                                                            bill_resource_row.STATEMENT_ID,
                                                                                            bill_resource_row.BILL_DATE,
                                                                                            bill_resource_row.MPAN_MPRN,
                                                                                            utilityviewmodel.Hezbollah.e_discounts_changesList);
                    foreach (SmartUtility.EDiscounts e_discounts_row in e_discounts_found)
                    {
                        if (!string.IsNullOrEmpty(rowAsString))
                        {
                            // Separate each record with this
                            rowAsString += SmartParametersV2016.recordSeparator;
                        }
                        SmartUtility.EDiscountsSQLite abc = ConvertUtilityEDiscounts(ourviewmodel, e_discounts_row, deriveds.Length);
                        utilityviewmodel.Hezbollah.sqlite_e_discountsList.Add(abc);
                        rowAsString += SmartNibbyV2016.Build_StringNew<SmartUtility.EDiscountsSQLite>(myFields, abc, sqliteformat);
                        e_discounts_temp.Add(e_discounts_row);
                    }
                }
                if (!string.IsNullOrEmpty(rowAsString))
                {
                    utilityviewmodel.bollocks.AppendLine(deriveds +
                                        SmartParametersV2016.groupSeparator +
                                        "I" + SmartParametersV2016.groupSeparator +
                                        rowAsString);
                    foreach (SmartUtility.EDiscounts temp_row in e_discounts_temp)
                    {
                        utilityviewmodel.Hezbollah.e_discountsList.Add(temp_row); // Range(e_discounts_temp);
                    }
                    // Can't use RemoveRange here, but what the fuck!
                    //foreach (SmartUtility.EDiscounts e_discounts_row in e_discounts_temp)
                    //{
                    //    utilityviewmodel.Hezbollah.e_discounts_changesList.Remove(e_discounts_row);
                    //}
                }
                utilityviewmodel.Hezbollah.e_discounts_changesList.Clear();
            }
            return;
        }

        internal static SmartUtility.EDiscountsSQLite ConvertUtilityEDiscounts(MainViewModel ourviewmodel,
                                                        SmartUtility.EDiscounts ediscounts_row,
                                                        int derived)
        {
            // Encrypt all of these into DETAILS with randomkey
            string key_details_unencrypted = ediscounts_row.ACCOUNT_NO +
                                        SmartParametersV2016.fieldSeparator +
                                        ediscounts_row.STATEMENT_ID +
                                        SmartParametersV2016.fieldSeparator +
                                        ediscounts_row.BILL_DATE.ToString(SmartParametersV2016.sqliteformat) +
                                        SmartParametersV2016.fieldSeparator +
                                        ediscounts_row.MPAN_MPRN +
                                        SmartParametersV2016.fieldSeparator +
                                        ediscounts_row.DISCOUNT_DATE.ToString(SmartParametersV2016.sqliteformat) +
                                        SmartParametersV2016.fieldSeparator +
                                        ediscounts_row.DISCOUNT_ITEM.ToString();

            string key_details = (ourviewmodel.UDEK == "" ?
                key_details_unencrypted : SmartEncryptionV2016.DoTheBiz(key_details_unencrypted, SmartPhyllV2020.BuildEncryptKey(ourviewmodel.UDEK, ediscounts_row.RANDOMKEY, derived), "", true));

            //string details_unencrypted = ediscounts_row.DISCOUNT_TYPE +
            //                            SmartParametersV2016.fieldSeparator +
            //                            ediscounts_row.DISCOUNT_VAT_CODE.ToString() +
            //                            SmartParametersV2016.fieldSeparator +
            //                            ediscounts_row.DISCOUNT_AMOUNT.ToString();
            //// Time for a new RandomKey
            //ediscounts_row.RANDOMKEY2 = SmartRoutinesV2018.GetNextRandomNew(ourviewmodel.randomR);
            //string details = (ourviewmodel.UDEK == "" ?
            //    details_unencrypted : SmartEncryptionV2016.DoTheBiz(details_unencrypted, SmartPhyllV2020.BuildEncryptKey(ourviewmodel.UDEK, ediscounts_row.RANDOMKEY2), "", true));

            SmartUtility.EDiscountsSQLite ediscounts_enc = new SmartUtility.EDiscountsSQLite()
            {
                USERNAME = ediscounts_row.USERNAME,
                CUBEFACE_CODE = ediscounts_row.CUBEFACE_CODE.ToString(),
                SUPPLIER_CODE = ediscounts_row.SUPPLIER_CODE,
                BRAND_CODE = ediscounts_row.BRAND_CODE,
                ACCOUNT_CREATED = ediscounts_row.ACCOUNT_CREATED,
                KEY_DETAILS = key_details,
                RANDOMKEY = ediscounts_row.RANDOMKEY,
                DISCOUNT_TYPE = ediscounts_row.DISCOUNT_TYPE,
                DISCOUNT_VAT_CODE = ediscounts_row.DISCOUNT_VAT_CODE,
                DISCOUNT_AMOUNT = ediscounts_row.DISCOUNT_AMOUNT,
                Updated = ediscounts_row.Updated
            };
            return ediscounts_enc;
        }

        internal static void DoAllEReadings(MainViewModel ourviewmodel,
                                            UtilityViewModel utilityviewmodel,
                                            string sqliteformat)
        {
            string deriveds = "SmartUtility.EReadings";

            utilityviewmodel.Hezbollah.sqlite_e_readingsList.Clear();
            // Do the e stuff - we have all the First and Last dates and Readings
            if (utilityviewmodel.Hezbollah.e_readings_changesList.Count > 0)
            {
                string rowAsString = "";
                FieldInfo[] myFields = typeof(SmartUtility.EReadings).GetFields(SmartParametersV2016.bindingFlags);
                List<SmartUtility.EReadings> e_readings_temp = new List<SmartUtility.EReadings>();
                // Readings depend on Bills
                foreach (SmartUtility.BillsResource bill_resource_row in utilityviewmodel.Hezbollah.bills_resource_tempList)
                {
                    List<SmartUtility.EReadings> e_readings_found = SmartSpikeUtilityV2017.Utility_Find_EReadings(ourviewmodel,
                                                                                        bill_resource_row.CUBEFACE_CODE,
                                                                                        bill_resource_row.SUPPLIER_CODE,
                                                                                        bill_resource_row.BRAND_CODE,
                                                                                        bill_resource_row.ACCOUNT_NO,
                                                                                        bill_resource_row.ACCOUNT_CREATED,
                                                                                        bill_resource_row.STATEMENT_ID,
                                                                                        bill_resource_row.BILL_DATE,
                                                                                        bill_resource_row.MPAN_MPRN,
                                                                                        utilityviewmodel.Hezbollah.e_readings_changesList);
                    // These don't come back in PERIOD_END order ...
                    foreach (SmartUtility.EReadings e_readings_row in e_readings_found)
                    {
                        // Let duplicate readings bounce off as
                        // sqlexceptions in Insert_Common.  This is a kludge
                        // and I know it.  However ... there will be very few 'normal'
                        // duplicate readings (i.e. when the same one appears on two
                        // different bills) and all the readings returned from
                        // Scottish Power because I cannot read their bills.  I don't
                        // expect this to last forever!  ONE DAY!! SCottish Power will
                        // have readable bills, and I can get more or less unique
                        // readings every time and I won't be trapping exceptions
                        // Live with it Ray, you have done wonders so far ...
                        if (!string.IsNullOrEmpty(rowAsString))
                        {
                            // Separate each record with this
                            rowAsString += SmartParametersV2016.recordSeparator;
                        }
                        SmartUtility.EReadingsSQLite abc = ConvertUtilityEReadings(ourviewmodel, e_readings_row, deriveds.Length);
                        utilityviewmodel.Hezbollah.sqlite_e_readingsList.Add(abc);
                        rowAsString += SmartNibbyV2016.Build_StringNew<SmartUtility.EReadingsSQLite>(myFields, abc, sqliteformat);
                        e_readings_temp.Add(e_readings_row);
                    }
                }
                if (!string.IsNullOrEmpty(rowAsString))
                {
                    utilityviewmodel.bollocks.AppendLine(deriveds +
                                        SmartParametersV2016.groupSeparator +
                                        "I" + SmartParametersV2016.groupSeparator +
                                        rowAsString);
                    foreach (SmartUtility.EReadings temp_row in e_readings_temp)
                    {
                        utilityviewmodel.Hezbollah.e_readingsList.Add(temp_row); // Range(e_readings_temp);
                    }
                    // Can't use RemoveRange here, but what the fuck!
                    //foreach (SmartUtility.EReadings e_readings_row in e_readings_temp)
                    //{
                    //    utilityviewmodel.Hezbollah.e_readings_changesList.Remove(e_readings_row);
                    //}
                }
                utilityviewmodel.Hezbollah.e_readings_changesList.Clear();
            }
            return;
        }

        internal static SmartUtility.EReadingsSQLite ConvertUtilityEReadings(MainViewModel ourviewmodel,
                                                        SmartUtility.EReadings ereadings_row,
                                                        int derived)
        {
            // Encrypt all of these into DETAILS with randomkey
            string key_details_unencrypted = ereadings_row.ACCOUNT_NO +
                                        SmartParametersV2016.fieldSeparator +
                                        ereadings_row.STATEMENT_ID +
                                        SmartParametersV2016.fieldSeparator +
                                        ereadings_row.BILL_DATE.ToString(SmartParametersV2016.sqliteformat) +
                                        SmartParametersV2016.fieldSeparator +
                                        ereadings_row.MPAN_MPRN +
                                        SmartParametersV2016.fieldSeparator +
                                        ereadings_row.READINGS_PERIOD_START.ToString(SmartParametersV2016.sqliteformat) +
                                        SmartParametersV2016.fieldSeparator +
                                        ereadings_row.READINGS_PERIOD_END.ToString(SmartParametersV2016.sqliteformat);

            string key_details = (ourviewmodel.UDEK == "" ?
                key_details_unencrypted : SmartEncryptionV2016.DoTheBiz(key_details_unencrypted, SmartPhyllV2020.BuildEncryptKey(ourviewmodel.UDEK, ereadings_row.RANDOMKEY1, derived), "", true));

            string details_unencrypted = ereadings_row.METER_SERIAL_NO +
                                        SmartParametersV2016.fieldSeparator +
                                        ereadings_row.READ_TYPE +
                                        SmartParametersV2016.fieldSeparator +
                                        ereadings_row.D_LAST_READ.ToString() +
                                        SmartParametersV2016.fieldSeparator +
                                        ereadings_row.D_THIS_READ.ToString() +
                                        SmartParametersV2016.fieldSeparator +
                                        ereadings_row.D_UNITS_USED.ToString() +
                                        SmartParametersV2016.fieldSeparator +
                                        ereadings_row.N_LAST_READ.ToString() +
                                        SmartParametersV2016.fieldSeparator +
                                        ereadings_row.N_THIS_READ.ToString() +
                                        SmartParametersV2016.fieldSeparator +
                                        ereadings_row.N_UNITS_USED.ToString() +
                                        SmartParametersV2016.fieldSeparator +
                                        ereadings_row.UNIT_OF_MEASURE;
            // Time for a new RandomKey
            ereadings_row.RANDOMKEY2 = SmartRoutinesV2018.GetNextRandomNew(ourviewmodel.randomR);
            string details = (ourviewmodel.UDEK == "" ?
                details_unencrypted : SmartEncryptionV2016.DoTheBiz(details_unencrypted, SmartPhyllV2020.BuildEncryptKey(ourviewmodel.UDEK, ereadings_row.RANDOMKEY2, derived), "", true));

            SmartUtility.EReadingsSQLite ereadings_enc = new SmartUtility.EReadingsSQLite()
            {
                USERNAME = ereadings_row.USERNAME,
                CUBEFACE_CODE = ereadings_row.CUBEFACE_CODE.ToString(),
                SUPPLIER_CODE = ereadings_row.SUPPLIER_CODE,
                BRAND_CODE = ereadings_row.BRAND_CODE,
                ACCOUNT_CREATED = ereadings_row.ACCOUNT_CREATED,
                KEY_DETAILS = key_details,
                RANDOMKEY1 = ereadings_row.RANDOMKEY1,
                DETAILS = details,
                RANDOMKEY2 = ereadings_row.RANDOMKEY2,
                Updated = ereadings_row.Updated
            };
            return ereadings_enc;
        }

        internal static void DoAllEStandingCharges(MainViewModel ourviewmodel,
                                        UtilityViewModel utilityviewmodel,
                                        string sqliteformat)
        {
            string deriveds = "SmartUtility.EStandingCharges";
            utilityviewmodel.Hezbollah.sqlite_e_standing_chargesList.Clear();
            if (utilityviewmodel.Hezbollah.e_standing_chargesList.Count > 0)
            {
                string rowAsString = "";
                FieldInfo[] myFields = typeof(SmartUtility.EStandingCharges).GetFields(SmartParametersV2016.bindingFlags);
                List<SmartUtility.EStandingCharges> e_standing_charges_temp = new List<SmartUtility.EStandingCharges>();
                // Standing Charges depend on Bills
                foreach (SmartUtility.BillsResource bill_resource_row in utilityviewmodel.Hezbollah.bills_resource_tempList)
                {
                    List<SmartUtility.EStandingCharges> e_standing_charges_found = SmartSpikeUtilityV2017.Utility_Find_EStandingCharges(ourviewmodel,
                                                                                                                bill_resource_row.CUBEFACE_CODE,
                                                                                                                bill_resource_row.SUPPLIER_CODE,
                                                                                                                bill_resource_row.BRAND_CODE,
                                                                                                                bill_resource_row.ACCOUNT_NO,
                                                                                                                bill_resource_row.ACCOUNT_CREATED,
                                                                                                                bill_resource_row.STATEMENT_ID,
                                                                                                                bill_resource_row.BILL_DATE,
                                                                                                                bill_resource_row.MPAN_MPRN,
                                                                                                                utilityviewmodel.Hezbollah.e_standing_charges_changesList);
                    foreach (SmartUtility.EStandingCharges e_standing_charges_row in e_standing_charges_found)
                    {
                        if (!string.IsNullOrEmpty(rowAsString))
                        {
                            // Separate each record with this
                            rowAsString += SmartParametersV2016.recordSeparator;
                        }
                        SmartUtility.EStandingChargesSQLite abc = ConvertUtilityEStandingCharges(ourviewmodel, e_standing_charges_row, deriveds.Length);
                        utilityviewmodel.Hezbollah.sqlite_e_standing_chargesList.Add(abc);
                        rowAsString += SmartNibbyV2016.Build_StringNew<SmartUtility.EStandingCharges>(myFields, e_standing_charges_row, sqliteformat);
                        e_standing_charges_temp.Add(e_standing_charges_row);
                    }
                }
                if (!string.IsNullOrEmpty(rowAsString))
                {
                    utilityviewmodel.bollocks.AppendLine(deriveds +
                                        SmartParametersV2016.groupSeparator +
                                        "I" + SmartParametersV2016.groupSeparator +
                                        rowAsString);
                    foreach (SmartUtility.EStandingCharges temp_row in e_standing_charges_temp)
                    {
                        utilityviewmodel.Hezbollah.e_standing_chargesList.Add(temp_row); // Range(e_standing_charges_temp);
                    }
                    // Can't use RemoveRange here, but what the fuck!
                    //foreach (SmartUtility.EStandingCharges e_standing_charges_row in e_standing_charges_temp)
                    //{
                    //    utilityviewmodel.Hezbollah.e_standing_charges_changesList.Remove(e_standing_charges_row);
                    //}
                }
                utilityviewmodel.Hezbollah.e_standing_charges_changesList.Clear();
            }
            return;
        }

        internal static SmartUtility.EStandingChargesSQLite ConvertUtilityEStandingCharges(MainViewModel ourviewmodel,
                                                        SmartUtility.EStandingCharges estandingcharges_row,
                                                        int derived)
        {
            // Encrypt all of these into DETAILS with randomkey
            string key_details_unencrypted = estandingcharges_row.ACCOUNT_NO +
                                        SmartParametersV2016.fieldSeparator +
                                        estandingcharges_row.STATEMENT_ID +
                                        SmartParametersV2016.fieldSeparator +
                                        estandingcharges_row.BILL_DATE.ToString(SmartParametersV2016.sqliteformat) +
                                        SmartParametersV2016.fieldSeparator +
                                        estandingcharges_row.MPAN_MPRN +
                                        SmartParametersV2016.fieldSeparator +
                                        estandingcharges_row.STANDING_CHARGES_PERIOD_START.ToString(SmartParametersV2016.sqliteformat) +
                                        SmartParametersV2016.fieldSeparator +
                                        estandingcharges_row.STANDING_CHARGES_PERIOD_END.ToString(SmartParametersV2016.sqliteformat) +
                                        SmartParametersV2016.fieldSeparator +
                                        estandingcharges_row.CHARGES_ITEM.ToString();

            string key_details = (ourviewmodel.UDEK == "" ?
                key_details_unencrypted : SmartEncryptionV2016.DoTheBiz(key_details_unencrypted, SmartPhyllV2020.BuildEncryptKey(ourviewmodel.UDEK, estandingcharges_row.RANDOMKEY, derived), "", true));
            SmartUtility.EStandingChargesSQLite estandingcharges_enc = new SmartUtility.EStandingChargesSQLite()
            {
                USERNAME = estandingcharges_row.USERNAME,
                CUBEFACE_CODE = estandingcharges_row.CUBEFACE_CODE.ToString(),
                SUPPLIER_CODE = estandingcharges_row.SUPPLIER_CODE,
                BRAND_CODE = estandingcharges_row.BRAND_CODE,
                ACCOUNT_CREATED = estandingcharges_row.ACCOUNT_CREATED,
                KEY_DETAILS = key_details,
                RANDOMKEY = estandingcharges_row.RANDOMKEY,
                CHARGES_TYPE = estandingcharges_row.CHARGES_TYPE,
                STANDING_CHARGE = estandingcharges_row.STANDING_CHARGE,
                CHARGES_DAYS = estandingcharges_row.CHARGES_DAYS,
                CHARGES_COST = estandingcharges_row.CHARGES_COST,
                Updated = estandingcharges_row.Updated
            };
            return estandingcharges_enc;
        }

        internal static void DoAllEUnitCharges(MainViewModel ourviewmodel,
                                            UtilityViewModel utilityviewmodel,
                                            string sqliteformat)
        {
            string deriveds = "SmartUtility.EUnitCharges";
            utilityviewmodel.Hezbollah.sqlite_e_unit_chargesList.Clear();
            if (utilityviewmodel.Hezbollah.e_unit_chargesList.Count > 0)
            {
                string rowAsString = "";
                FieldInfo[] myFields = typeof(SmartUtility.EUnitCharges).GetFields(SmartParametersV2016.bindingFlags);
                List<SmartUtility.EUnitCharges> e_unit_charges_temp = new List<SmartUtility.EUnitCharges>();
                // Charges depend on Bills
                foreach (SmartUtility.BillsResource bill_resource_row in utilityviewmodel.Hezbollah.bills_resource_tempList)
                {
                    List<SmartUtility.EUnitCharges> e_unit_charges_found = SmartSpikeUtilityV2017.Utility_Find_EUnitCharges(ourviewmodel,
                                                                                                    bill_resource_row.CUBEFACE_CODE,
                                                                                                    bill_resource_row.SUPPLIER_CODE,
                                                                                                    bill_resource_row.BRAND_CODE,
                                                                                                    bill_resource_row.ACCOUNT_NO,
                                                                                                    bill_resource_row.ACCOUNT_CREATED,
                                                                                                    bill_resource_row.STATEMENT_ID,
                                                                                                    bill_resource_row.BILL_DATE,
                                                                                                    bill_resource_row.MPAN_MPRN,
                                                                                                    utilityviewmodel.Hezbollah.e_unit_charges_changesList);
                    foreach (SmartUtility.EUnitCharges e_unit_charges_row in e_unit_charges_found)
                    {
                        if (!string.IsNullOrEmpty(rowAsString))
                        {
                            // Separate each record with this
                            rowAsString += SmartParametersV2016.recordSeparator;
                        }
                        SmartUtility.EUnitChargesSQLite abc = ConvertUtilityEUnitCharges(ourviewmodel, e_unit_charges_row, deriveds.Length);
                        utilityviewmodel.Hezbollah.sqlite_e_unit_chargesList.Add(abc);
                        rowAsString += SmartNibbyV2016.Build_StringNew<SmartUtility.EUnitCharges>(myFields, e_unit_charges_row, sqliteformat);
                        e_unit_charges_temp.Add(e_unit_charges_row);
                    }
                }
                if (!string.IsNullOrEmpty(rowAsString))
                {
                    utilityviewmodel.bollocks.AppendLine(deriveds +
                                        SmartParametersV2016.groupSeparator +
                                        "I" + SmartParametersV2016.groupSeparator +
                                        rowAsString);
                    foreach (SmartUtility.EUnitCharges temp_row in e_unit_charges_temp)
                    {
                        utilityviewmodel.Hezbollah.e_unit_chargesList.Add(temp_row); // Range(e_unit_charges_temp);
                    }
                    // Can't use RemoveRange here, but what the fuc.unk!
                    //foreach (SmartUtility.EUnitCharges e_unit_charges_row in e_unit_charges_temp)
                    //{
                    //    utilityviewmodel.Hezbollah.e_unit_charges_changesList.Remove(e_unit_charges_row);
                    //}
                }
                utilityviewmodel.Hezbollah.e_unit_charges_changesList.Clear();
            }
            return;
        }

        internal static SmartUtility.EUnitChargesSQLite ConvertUtilityEUnitCharges(MainViewModel ourviewmodel,
                                                        SmartUtility.EUnitCharges eunitcharges_row,
                                                        int derived)
        {
            // Encrypt all of these into DETAILS with randomkey
            string key_details_unencrypted = eunitcharges_row.ACCOUNT_NO +
                                        SmartParametersV2016.fieldSeparator +
                                        eunitcharges_row.STATEMENT_ID +
                                        SmartParametersV2016.fieldSeparator +
                                        eunitcharges_row.BILL_DATE.ToString(SmartParametersV2016.sqliteformat) +
                                        SmartParametersV2016.fieldSeparator +
                                        eunitcharges_row.MPAN_MPRN +
                                        SmartParametersV2016.fieldSeparator +
                                        eunitcharges_row.UNIT_CHARGES_PERIOD_START.ToString(SmartParametersV2016.sqliteformat) +
                                        SmartParametersV2016.fieldSeparator +
                                        eunitcharges_row.UNIT_CHARGES_PERIOD_END.ToString(SmartParametersV2016.sqliteformat) +
                                        SmartParametersV2016.fieldSeparator +
                                        eunitcharges_row.UNITS_TIME.ToString() +
                                        SmartParametersV2016.fieldSeparator +
                                        eunitcharges_row.UNITS_BAND.ToString();

            string key_details = (ourviewmodel.UDEK == "" ?
                key_details_unencrypted : SmartEncryptionV2016.DoTheBiz(key_details_unencrypted, SmartPhyllV2020.BuildEncryptKey(ourviewmodel.UDEK, eunitcharges_row.RANDOMKEY, derived), "", true));
            SmartUtility.EUnitChargesSQLite eunitcharges_enc = new SmartUtility.EUnitChargesSQLite()
            {
                USERNAME = eunitcharges_row.USERNAME,
                CUBEFACE_CODE = eunitcharges_row.CUBEFACE_CODE.ToString(),
                SUPPLIER_CODE = eunitcharges_row.SUPPLIER_CODE,
                BRAND_CODE = eunitcharges_row.BRAND_CODE,
                ACCOUNT_CREATED = eunitcharges_row.ACCOUNT_CREATED,
                KEY_DETAILS = key_details,
                RANDOMKEY = eunitcharges_row.RANDOMKEY,
                UNITS_TYPE = eunitcharges_row.UNITS_TYPE,
                UNITS = eunitcharges_row.UNITS,
                UNITS_RATE = eunitcharges_row.UNITS_RATE,
                UNIT_OF_MEASURE = eunitcharges_row.UNIT_OF_MEASURE,
                UNITS_COST = eunitcharges_row.UNITS_COST,
                Updated = eunitcharges_row.Updated
            };
            return eunitcharges_enc;
        }

        internal static void DoAllEUsage(MainViewModel ourviewmodel,
                                            UtilityViewModel utilityviewmodel,
                                            string sqliteformat)
        {
            string deriveds = "SmartUtility.EUsage";
            utilityviewmodel.Hezbollah.sqlite_e_usageList.Clear();
            if (utilityviewmodel.Hezbollah.e_usage_changesList.Count > 0)
            {
                SmartUtility.EUsage last_row;
                int count;

                // Trim off the last reading if its 0.00  this is because sometimes The supplier
                // hasn't updated the reading for the previous day, and as we record units AND dates
                // of reading them, there is a chance we would miss a 'true' value by recording a 'false' 0.00 instead
                count = utilityviewmodel.Hezbollah.e_usage_changesList.Count;
                while (count > 0)
                {
                    last_row = utilityviewmodel.Hezbollah.e_usage_changesList[count - 1];
                    if (last_row.USAGE_TOTAL == 0)
                    {
                        count--;
                    }
                    else
                    {
                        break;
                    }
                }
                string rowAsString = "";
                FieldInfo[] myFields = typeof(SmartUtility.EUsage).GetFields(SmartParametersV2016.bindingFlags);

                //string last_usage_date = "";
                foreach (SmartUtility.EUsage e_usage_row in utilityviewmodel.Hezbollah.e_usage_changesList)
                {
                    if (count == 0)
                    {
                        break;
                    }
                    // The reading comes in as Wh so we must divide by 1000 to give Kwh

                    // For smart meter readings, we have to work out the UNITS
                    // but we do that LATER - its too fraught to do it here =:-(
                    if (!string.IsNullOrEmpty(rowAsString))
                    {
                        // Separate each record with this
                        rowAsString += SmartParametersV2016.recordSeparator;
                    }
                    SmartUtility.EUsageSQLite abc = ConvertUtilityEUsage(ourviewmodel, e_usage_row, deriveds.Length);
                    utilityviewmodel.Hezbollah.sqlite_e_usageList.Add(abc);
                    rowAsString += SmartNibbyV2016.Build_StringNew<SmartUtility.EUsage>(myFields, e_usage_row, sqliteformat);
                    // Done slightly differently than the others
                    utilityviewmodel.Hezbollah.e_usageList.Add(e_usage_row);
                    count--;
                }

                // Clear this table down
                if (!string.IsNullOrEmpty(rowAsString))
                {

                    utilityviewmodel.bollocks.AppendLine(deriveds +
                                            SmartParametersV2016.groupSeparator +
                                            "I" + SmartParametersV2016.groupSeparator +
                                            rowAsString);
                }
                utilityviewmodel.Hezbollah.e_usage_changesList.Clear();
            }
            return;
        }

        internal static SmartUtility.EUsageSQLite ConvertUtilityEUsage(MainViewModel ourviewmodel,
                                                        SmartUtility.EUsage eusage_row,
                                                        int derived)
        {
            // Encrypt all of these into DETAILS with randomkey
            string key_details_unencrypted = eusage_row.MPAN_MPRN +
                                        SmartParametersV2016.fieldSeparator +
                                        eusage_row.UNIQUE_INDEX.ToString();

            string key_details = (ourviewmodel.UDEK == "" ?
                key_details_unencrypted : SmartEncryptionV2016.DoTheBiz(key_details_unencrypted, SmartPhyllV2020.BuildEncryptKey(ourviewmodel.UDEK, eusage_row.RANDOMKEY, derived), "", true));
            SmartUtility.EUsageSQLite eusage_enc = new SmartUtility.EUsageSQLite()
            {
                USERNAME = eusage_row.USERNAME,
                CUBEFACE_CODE = eusage_row.CUBEFACE_CODE.ToString(),
                KEY_DETAILS = key_details,
                RANDOMKEY = eusage_row.RANDOMKEY,
                USAGE_DATETIME = eusage_row.USAGE_DATETIME,
                USAGE_TOTAL = eusage_row.USAGE_TOTAL,
                USAGE_VALUE = eusage_row.USAGE_VALUE,
                Updated = eusage_row.Updated
            };
            return eusage_enc;
        }

        internal static void DoAllGCosts(MainViewModel ourviewmodel,
                                            UtilityViewModel utilityviewmodel,
                                            string sqliteformat)
        {
            string deriveds = "SmartUtility.GCosts";
            utilityviewmodel.Hezbollah.sqlite_g_costsList.Clear();
            if (utilityviewmodel.Hezbollah.g_costsList.Count > 0)
            {
                string sql = "DELETE FROM " + "GCosts" +          // Same as above - might need a schema if not added auto
                                " WHERE USERNAME = '" + ourviewmodel.UserName + "'";
                utilityviewmodel.bollocks.AppendLine(SmartParametersV2016.SmartUtilitySchema + "." +
                                    "GCosts" + SmartParametersV2016.groupSeparator +
                                    "D" + SmartParametersV2016.groupSeparator +
                                    sql);

                string rowAsString = "";
                FieldInfo[] myFields = typeof(SmartUtility.GCosts).GetFields(SmartParametersV2016.bindingFlags);

                //string last_usage_date = "";
                foreach (SmartUtility.GCosts g_costs_row in utilityviewmodel.Hezbollah.g_costsList)
                {
                    if (!string.IsNullOrEmpty(rowAsString))
                    {
                        // Separate each record with this
                        rowAsString += SmartParametersV2016.recordSeparator;
                    }
                    SmartUtility.GCostsSQLite abc = ConvertUtilityGCosts(g_costs_row);
                    utilityviewmodel.Hezbollah.sqlite_g_costsList.Add(abc);
                    rowAsString += SmartNibbyV2016.Build_StringNew<SmartUtility.GCosts>(myFields, g_costs_row, sqliteformat);
                    // Done slightly differently than the others
                }

                // Clear this table down
                if (!string.IsNullOrEmpty(rowAsString))
                {
                    utilityviewmodel.bollocks.AppendLine(deriveds +
                                            SmartParametersV2016.groupSeparator +
                                            "I" + SmartParametersV2016.groupSeparator +
                                            rowAsString);
                }
                utilityviewmodel.Hezbollah.g_costsList = new List<SmartUtility.GCosts>();
            }
            return;
        }

        internal static SmartUtility.GCostsSQLite ConvertUtilityGCosts(SmartUtility.GCosts gcosts_row)
        {
            SmartUtility.GCostsSQLite gcosts_enc = new SmartUtility.GCostsSQLite()
            {
                USERNAME = gcosts_row.USERNAME,
                CUBEFACE_CODE = gcosts_row.CUBEFACE_CODE.ToString(),
                RESOURCE_CODE = gcosts_row.RESOURCE_CODE.ToString(),
                UNIQUE_NUMBER = gcosts_row.UNIQUE_NUMBER,
                SUPPLIER_CODE = gcosts_row.SUPPLIER_CODE,
                BRAND_CODE = gcosts_row.BRAND_CODE,
                TARIFF_CODE = gcosts_row.TARIFF_CODE,
                TCR = gcosts_row.TCR,
                RESOURCE_TYPE = gcosts_row.RESOURCE_TYPE,
                PAYMENT_PLAN = gcosts_row.PAYMENT_PLAN.ToString(),
                TOTAL_COST = gcosts_row.TOTAL_COST,
                Updated = gcosts_row.Updated
            };
            return gcosts_enc;
        }

        internal static void DoAllGDiscounts(MainViewModel ourviewmodel,
                                                UtilityViewModel utilityviewmodel,
                                                string sqliteformat)
        {
            string deriveds = "SmartUtility.GDiscounts";
            utilityviewmodel.Hezbollah.sqlite_g_discountsList.Clear();
            if (utilityviewmodel.Hezbollah.g_discounts_changesList.Count > 0)
            {
                string rowAsString = "";
                FieldInfo[] myFields = typeof(SmartUtility.GDiscounts).GetFields(SmartParametersV2016.bindingFlags);
                List<SmartUtility.GDiscounts> g_discounts_temp = new List<SmartUtility.GDiscounts>();
                // G Discounts depend on Bills
                foreach (SmartUtility.BillsResource bill_resource_row in utilityviewmodel.Hezbollah.bills_resource_tempList)
                {
                    List<SmartUtility.GDiscounts> g_discounts_found = SmartSpikeUtilityV2017.Utility_Find_GDiscounts(ourviewmodel,
                                                                                            bill_resource_row.CUBEFACE_CODE,
                                                                                            bill_resource_row.SUPPLIER_CODE,
                                                                                            bill_resource_row.BRAND_CODE,
                                                                                            bill_resource_row.ACCOUNT_NO,
                                                                                            bill_resource_row.ACCOUNT_CREATED,
                                                                                            bill_resource_row.STATEMENT_ID,
                                                                                            bill_resource_row.BILL_DATE,
                                                                                            bill_resource_row.MPAN_MPRN,
                                                                                            utilityviewmodel.Hezbollah.g_discounts_changesList);
                    foreach (SmartUtility.GDiscounts g_discounts_row in g_discounts_found)
                    {
                        if (!string.IsNullOrEmpty(rowAsString))
                        {
                            // Separate each record with this
                            rowAsString += SmartParametersV2016.recordSeparator;
                        }
                        SmartUtility.GDiscountsSQLite abc = ConvertUtilityGDiscounts(ourviewmodel, g_discounts_row, deriveds.Length);
                        utilityviewmodel.Hezbollah.sqlite_g_discountsList.Add(abc);
                        rowAsString += SmartNibbyV2016.Build_StringNew<SmartUtility.GDiscounts>(myFields, g_discounts_row, sqliteformat);
                        g_discounts_temp.Add(g_discounts_row);
                    }
                }
                if (!string.IsNullOrEmpty(rowAsString))
                {
                    utilityviewmodel.bollocks.AppendLine(deriveds +
                                        SmartParametersV2016.groupSeparator +
                                        "I" + SmartParametersV2016.groupSeparator +
                                        rowAsString);
                    foreach (SmartUtility.GDiscounts temp_row in g_discounts_temp)
                    {
                        utilityviewmodel.Hezbollah.g_discountsList.Add(temp_row); // Range(g_discounts_temp);
                    }
                    // Can't use RemoveRange here, but what the fuck!
                    //foreach (SmartUtility.GDiscounts g_discounts_row in g_discounts_temp)
                    //{
                    //    utilityviewmodel.Hezbollah.g_discounts_changesList.Remove(g_discounts_row);
                    //}
                }
                utilityviewmodel.Hezbollah.g_discounts_changesList.Clear();
            }
            return;
        }

        internal static SmartUtility.GDiscountsSQLite ConvertUtilityGDiscounts(MainViewModel ourviewmodel,
                                                        SmartUtility.GDiscounts gdiscounts_row,
                                                        int derived)
        {
            // Encrypt all of these into DETAILS with randomkey
            string key_details_unencrypted = gdiscounts_row.ACCOUNT_NO +
                                        SmartParametersV2016.fieldSeparator +
                                        gdiscounts_row.STATEMENT_ID +
                                        SmartParametersV2016.fieldSeparator +
                                        gdiscounts_row.BILL_DATE.ToString(SmartParametersV2016.sqliteformat) +
                                        SmartParametersV2016.fieldSeparator +
                                        gdiscounts_row.MPAN_MPRN +
                                        SmartParametersV2016.fieldSeparator +
                                        gdiscounts_row.DISCOUNT_DATE.ToString(SmartParametersV2016.sqliteformat) +
                                        SmartParametersV2016.fieldSeparator +
                                        gdiscounts_row.DISCOUNT_ITEM.ToString();

            string key_details = (ourviewmodel.UDEK == "" ?
                key_details_unencrypted : SmartEncryptionV2016.DoTheBiz(key_details_unencrypted, SmartPhyllV2020.BuildEncryptKey(ourviewmodel.UDEK, gdiscounts_row.RANDOMKEY, derived), "", true));

            //string details_unencrypted = gdiscounts_row.DISCOUNT_TYPE +
            //                            SmartParametersV2016.fieldSeparator +
            //                            gdiscounts_row.DISCOUNT_VAT_CODE.ToString() +
            //                            SmartParametersV2016.fieldSeparator +
            //                            gdiscounts_row.DISCOUNT_AMOUNT.ToString();
            //// Time for a new RandomKey
            //gdiscounts_row.RANDOMKEY2 = SmartRoutinesV2018.GetNextRandomNew(ourviewmodel.randomR);
            //string details = (ourviewmodel.UDEK == "" ?
            //    details_unencrypted : SmartEncryptionV2016.DoTheBiz(details_unencrypted, SmartPhyllV2020.BuildEncryptKey(ourviewmodel.UDEK, gdiscounts_row.RANDOMKEY2), "", true));

            SmartUtility.GDiscountsSQLite gdiscounts_enc = new SmartUtility.GDiscountsSQLite()
            {
                USERNAME = gdiscounts_row.USERNAME,
                CUBEFACE_CODE = gdiscounts_row.CUBEFACE_CODE.ToString(),
                SUPPLIER_CODE = gdiscounts_row.SUPPLIER_CODE,
                BRAND_CODE = gdiscounts_row.BRAND_CODE,
                ACCOUNT_CREATED = gdiscounts_row.ACCOUNT_CREATED,
                KEY_DETAILS = key_details,
                RANDOMKEY = gdiscounts_row.RANDOMKEY,
                DISCOUNT_TYPE = gdiscounts_row.DISCOUNT_TYPE,
                DISCOUNT_VAT_CODE = gdiscounts_row.DISCOUNT_VAT_CODE,
                DISCOUNT_AMOUNT = gdiscounts_row.DISCOUNT_AMOUNT,
                Updated = gdiscounts_row.Updated
            };
            return gdiscounts_enc;
        }

        internal static void DoAllGReadings(MainViewModel ourviewmodel,
                                            UtilityViewModel utilityviewmodel,
                                            string sqliteformat)
        {
            string depriveds = "SmartUtility.GReadings";
            utilityviewmodel.Hezbollah.sqlite_g_readingsList.Clear();
            if (utilityviewmodel.Hezbollah.g_readings_changesList.Count > 0)
            {
                string rowAsString = "";
                FieldInfo[] myFields = typeof(SmartUtility.GReadings).GetFields(SmartParametersV2016.bindingFlags);
                List<SmartUtility.GReadings> g_readings_temp = new List<SmartUtility.GReadings>();
                // G Readings depend on Bills
                foreach (SmartUtility.BillsResource bill_resource_row in utilityviewmodel.Hezbollah.bills_resource_tempList)
                {
                    // These don't come back in PERIOD_END order ...
                    List<SmartUtility.GReadings> g_readings_found = SmartSpikeUtilityV2017.Utility_Find_GReadings(ourviewmodel,
                                                                                        bill_resource_row.CUBEFACE_CODE,
                                                                                        bill_resource_row.SUPPLIER_CODE,
                                                                                        bill_resource_row.BRAND_CODE,
                                                                                        bill_resource_row.ACCOUNT_NO,
                                                                                        bill_resource_row.ACCOUNT_CREATED,
                                                                                        bill_resource_row.STATEMENT_ID,
                                                                                        bill_resource_row.BILL_DATE,
                                                                                        bill_resource_row.MPAN_MPRN,
                                                                                        utilityviewmodel.Hezbollah.g_readings_changesList);
                    foreach (SmartUtility.GReadings g_readings_row in g_readings_found)
                    {
                        if (!string.IsNullOrEmpty(rowAsString))
                        {
                            // Separate each record with this
                            rowAsString += SmartParametersV2016.recordSeparator;
                        }
                        SmartUtility.GReadingsSQLite abc = ConvertUtilityGReadings(ourviewmodel, g_readings_row, depriveds.Length);
                        utilityviewmodel.Hezbollah.sqlite_g_readingsList.Add(abc);
                        rowAsString += SmartNibbyV2016.Build_StringNew<SmartUtility.GReadings>(myFields, g_readings_row, sqliteformat);
                        g_readings_temp.Add(g_readings_row);
                    }
                }
                if (!string.IsNullOrEmpty(rowAsString))
                {
                    utilityviewmodel.bollocks.AppendLine(depriveds +
                                        SmartParametersV2016.groupSeparator +
                                        "I" + SmartParametersV2016.groupSeparator +
                                        rowAsString);
                    foreach (SmartUtility.GReadings temp_row in g_readings_temp)
                    {
                        utilityviewmodel.Hezbollah.g_readingsList.Add(temp_row); // Range(g_readings_temp);
                    }
                    // Can't use RemoveRange here, but what the fuck!
                    //foreach (SmartUtility.GReadings g_readings_row in g_readings_temp)
                    //{
                    //    utilityviewmodel.Hezbollah.g_readings_changesList.Remove(g_readings_row);
                    //}
                }
                utilityviewmodel.Hezbollah.g_readings_changesList.Clear();
            }
            return;
        }

        internal static SmartUtility.GReadingsSQLite ConvertUtilityGReadings(MainViewModel ourviewmodel,
                                                        SmartUtility.GReadings greadings_row,
                                                        int deprived)
        {
            // Encrypt all of these into DETAILS with randomkey
            string key_details_unencrypted = greadings_row.ACCOUNT_NO +
                                        SmartParametersV2016.fieldSeparator +
                                        greadings_row.STATEMENT_ID +
                                        SmartParametersV2016.fieldSeparator +
                                        greadings_row.BILL_DATE.ToString(SmartParametersV2016.sqliteformat) +
                                        SmartParametersV2016.fieldSeparator +
                                        greadings_row.MPAN_MPRN +
                                        SmartParametersV2016.fieldSeparator +
                                        greadings_row.READINGS_PERIOD_START.ToString(SmartParametersV2016.sqliteformat) +
                                        SmartParametersV2016.fieldSeparator +
                                        greadings_row.READINGS_PERIOD_END.ToString(SmartParametersV2016.sqliteformat);

            string key_details = (ourviewmodel.UDEK == "" ?
                key_details_unencrypted : SmartEncryptionV2016.DoTheBiz(key_details_unencrypted, SmartPhyllV2020.BuildEncryptKey(ourviewmodel.UDEK, greadings_row.RANDOMKEY1, deprived), "", true));

            string details_unencrypted = greadings_row.METER_SERIAL_NO +
                                        SmartParametersV2016.fieldSeparator +
                                        greadings_row.READ_TYPE +
                                        SmartParametersV2016.fieldSeparator +
                                        greadings_row.D_LAST_READ.ToString() +
                                        SmartParametersV2016.fieldSeparator +
                                        greadings_row.D_THIS_READ.ToString() +
                                        SmartParametersV2016.fieldSeparator +
                                        greadings_row.D_UNITS_USED_M3.ToString() +
                                        SmartParametersV2016.fieldSeparator +
                                        greadings_row.UNIT_OF_MEASURE +
                                        SmartParametersV2016.fieldSeparator +
                                        greadings_row.D_UNITS_USED_KWH.ToString() +
                                        SmartParametersV2016.fieldSeparator +
                                        greadings_row.CALORIFIC_VALUE.ToString();
            // Time for a new RandomKey
            greadings_row.RANDOMKEY2 = SmartRoutinesV2018.GetNextRandomNew(ourviewmodel.randomR);
            string details = (ourviewmodel.UDEK == "" ?
                details_unencrypted : SmartEncryptionV2016.DoTheBiz(details_unencrypted, SmartPhyllV2020.BuildEncryptKey(ourviewmodel.UDEK, greadings_row.RANDOMKEY2, deprived), "", true));

            SmartUtility.GReadingsSQLite greadings_enc = new SmartUtility.GReadingsSQLite()
            {
                USERNAME = greadings_row.USERNAME,
                CUBEFACE_CODE = greadings_row.CUBEFACE_CODE.ToString(),
                SUPPLIER_CODE = greadings_row.SUPPLIER_CODE,
                BRAND_CODE = greadings_row.BRAND_CODE,
                ACCOUNT_CREATED = greadings_row.ACCOUNT_CREATED,
                KEY_DETAILS = key_details,
                RANDOMKEY1 = greadings_row.RANDOMKEY1,
                DETAILS = details,
                RANDOMKEY2 = greadings_row.RANDOMKEY2,
                Updated = greadings_row.Updated
            };
            return greadings_enc;
        }

        internal static void DoAllGStandingCharges(MainViewModel ourviewmodel,
                                            UtilityViewModel utilityviewmodel,
                                            string sqliteformat)
        {
            string deriveds = "SmartUtility.GStandingCharges";
            utilityviewmodel.Hezbollah.sqlite_g_standing_chargesList.Clear();
            if (utilityviewmodel.Hezbollah.g_standing_charges_changesList.Count > 0)
            {
                string rowAsString = "";
                FieldInfo[] myFields = typeof(SmartUtility.GStandingCharges).GetFields(SmartParametersV2016.bindingFlags);
                List<SmartUtility.GStandingCharges> g_standing_charges_temp = new List<SmartUtility.GStandingCharges>();
                // G Standing Charges depend on Bills
                foreach (SmartUtility.BillsResource bill_resource_row in utilityviewmodel.Hezbollah.bills_resource_tempList)
                {
                    List<SmartUtility.GStandingCharges> g_standing_charges_found = SmartSpikeUtilityV2017.Utility_Find_GStandingCharges(ourviewmodel,
                                                                                                                bill_resource_row.CUBEFACE_CODE,
                                                                                                                bill_resource_row.SUPPLIER_CODE,
                                                                                                                bill_resource_row.BRAND_CODE,
                                                                                                                bill_resource_row.ACCOUNT_NO,
                                                                                                                bill_resource_row.ACCOUNT_CREATED,
                                                                                                                bill_resource_row.STATEMENT_ID,
                                                                                                                bill_resource_row.BILL_DATE,
                                                                                                                bill_resource_row.MPAN_MPRN,
                                                                                                                utilityviewmodel.Hezbollah.g_standing_charges_changesList);
                    foreach (SmartUtility.GStandingCharges g_standing_charges_row in g_standing_charges_found)
                    {
                        if (!string.IsNullOrEmpty(rowAsString))
                        {
                            // Separate each record with this
                            rowAsString += SmartParametersV2016.recordSeparator;
                        }
                        SmartUtility.GStandingChargesSQLite abc = ConvertUtilityGStandingCharges(ourviewmodel, g_standing_charges_row, deriveds.Length);
                        utilityviewmodel.Hezbollah.sqlite_g_standing_chargesList.Add(abc);
                        rowAsString += SmartNibbyV2016.Build_StringNew<SmartUtility.GStandingCharges>(myFields, g_standing_charges_row, sqliteformat);
                        g_standing_charges_temp.Add(g_standing_charges_row);
                    }
                }
                if (!string.IsNullOrEmpty(rowAsString))
                {
                    utilityviewmodel.bollocks.AppendLine(deriveds +
                                        SmartParametersV2016.groupSeparator +
                                        "I" + SmartParametersV2016.groupSeparator +
                                        rowAsString);
                    foreach (SmartUtility.GStandingCharges temp_row in g_standing_charges_temp)
                    {
                        utilityviewmodel.Hezbollah.g_standing_chargesList.Add(temp_row); // Range(g_standing_charges_temp);
                    }
                    // Can't use RemoveRange here, but what the fuck!
                    //foreach (SmartUtility.GStandingCharges g_standing_charges_row in g_standing_charges_temp)
                    //{
                    //    utilityviewmodel.Hezbollah.g_standing_charges_changesList.Remove(g_standing_charges_row);
                    //}
                }
                utilityviewmodel.Hezbollah.g_standing_charges_changesList.Clear();
            }
            return;
        }

        internal static SmartUtility.GStandingChargesSQLite ConvertUtilityGStandingCharges(MainViewModel ourviewmodel,
                                                        SmartUtility.GStandingCharges gstandingcharges_row,
                                                        int derived)
        {
            // Encrypt all of these into DETAILS with randomkey
            string key_details_unencrypted = gstandingcharges_row.ACCOUNT_NO +
                                        SmartParametersV2016.fieldSeparator +
                                        gstandingcharges_row.STATEMENT_ID +
                                        SmartParametersV2016.fieldSeparator +
                                        gstandingcharges_row.BILL_DATE.ToString(SmartParametersV2016.sqliteformat) +
                                        SmartParametersV2016.fieldSeparator +
                                        gstandingcharges_row.MPAN_MPRN +
                                        SmartParametersV2016.fieldSeparator +
                                        gstandingcharges_row.STANDING_CHARGES_PERIOD_START.ToString(SmartParametersV2016.sqliteformat) +
                                        SmartParametersV2016.fieldSeparator +
                                        gstandingcharges_row.STANDING_CHARGES_PERIOD_END.ToString(SmartParametersV2016.sqliteformat) +
                                        SmartParametersV2016.fieldSeparator +
                                        gstandingcharges_row.CHARGES_ITEM.ToString();

            string key_details = (ourviewmodel.UDEK == "" ?
                key_details_unencrypted : SmartEncryptionV2016.DoTheBiz(key_details_unencrypted, SmartPhyllV2020.BuildEncryptKey(ourviewmodel.UDEK, gstandingcharges_row.RANDOMKEY, derived), "", true));
            SmartUtility.GStandingChargesSQLite gstandingcharges_enc = new SmartUtility.GStandingChargesSQLite()
            {
                USERNAME = gstandingcharges_row.USERNAME,
                CUBEFACE_CODE = gstandingcharges_row.CUBEFACE_CODE.ToString(),
                SUPPLIER_CODE = gstandingcharges_row.SUPPLIER_CODE,
                BRAND_CODE = gstandingcharges_row.BRAND_CODE,
                ACCOUNT_CREATED = gstandingcharges_row.ACCOUNT_CREATED,
                KEY_DETAILS = key_details,
                RANDOMKEY = gstandingcharges_row.RANDOMKEY,
                CHARGES_TYPE = gstandingcharges_row.CHARGES_TYPE,
                STANDING_CHARGE = gstandingcharges_row.STANDING_CHARGE,
                CHARGES_DAYS = gstandingcharges_row.CHARGES_DAYS,
                CHARGES_COST = gstandingcharges_row.CHARGES_COST,
                Updated = gstandingcharges_row.Updated
            };
            return gstandingcharges_enc;
        }

        internal static void DoAllGUnitCharges(MainViewModel ourviewmodel,
                                            UtilityViewModel utilityviewmodel,
                                            string sqliteformat)
        {
            string deriveds = "SmartUtility.GUnitCharges";
            utilityviewmodel.Hezbollah.sqlite_g_unit_chargesList.Clear();
            if (utilityviewmodel.Hezbollah.g_unit_charges_changesList.Count > 0)
            {
                string rowAsString = "";
                FieldInfo[] myFields = typeof(SmartUtility.GUnitCharges).GetFields(SmartParametersV2016.bindingFlags);
                List<SmartUtility.GUnitCharges> g_unit_charges_temp = new List<SmartUtility.GUnitCharges>();
                // G Unit Charges depend on Bills
                foreach (SmartUtility.BillsResource bill_resource_row in utilityviewmodel.Hezbollah.bills_resource_tempList)
                {
                    List<SmartUtility.GUnitCharges> g_unit_charges_found = SmartSpikeUtilityV2017.Utility_Find_GUnitCharges(ourviewmodel,
                                                                                                    bill_resource_row.CUBEFACE_CODE,
                                                                                                    bill_resource_row.SUPPLIER_CODE,
                                                                                                    bill_resource_row.BRAND_CODE,
                                                                                                    bill_resource_row.ACCOUNT_NO,
                                                                                                    bill_resource_row.ACCOUNT_CREATED,
                                                                                                    bill_resource_row.STATEMENT_ID,
                                                                                                    bill_resource_row.BILL_DATE,
                                                                                                    bill_resource_row.MPAN_MPRN,
                                                                                                    utilityviewmodel.Hezbollah.g_unit_charges_changesList);
                    foreach (SmartUtility.GUnitCharges g_unit_charges_row in g_unit_charges_found)
                    {
                        if (!string.IsNullOrEmpty(rowAsString))
                        {
                            // Separate each record with this
                            rowAsString += SmartParametersV2016.recordSeparator;
                        }
                        SmartUtility.GUnitChargesSQLite abc = ConvertUtilityGUnitCharges(ourviewmodel, g_unit_charges_row, deriveds.Length);
                        utilityviewmodel.Hezbollah.sqlite_g_unit_chargesList.Add(abc);
                        rowAsString += SmartNibbyV2016.Build_StringNew<SmartUtility.GUnitCharges>(myFields, g_unit_charges_row, sqliteformat);
                        g_unit_charges_temp.Add(g_unit_charges_row);
                    }
                }
                if (!string.IsNullOrEmpty(rowAsString))
                {
                    utilityviewmodel.bollocks.AppendLine(deriveds +
                                        SmartParametersV2016.groupSeparator +
                                        "I" + SmartParametersV2016.groupSeparator +
                                        rowAsString);
                    foreach (SmartUtility.GUnitCharges temp_row in g_unit_charges_temp)
                    {
                        utilityviewmodel.Hezbollah.g_unit_chargesList.Add(temp_row); // Range(g_unit_charges_temp);
                    }
                    // Can't use RemoveRange here, but what the fuck!
                    //foreach (SmartUtility.GUnitCharges g_unit_charges_row in g_unit_charges_temp)
                    //{
                    //    utilityviewmodel.Hezbollah.g_unit_charges_changesList.Remove(g_unit_charges_row);
                    //}
                }
                utilityviewmodel.Hezbollah.g_unit_charges_changesList.Clear();
            }
            return;
        }

        internal static SmartUtility.GUnitChargesSQLite ConvertUtilityGUnitCharges(MainViewModel ourviewmodel,
                                                        SmartUtility.GUnitCharges gunitcharges_row,
                                                        int derived)
        {
            // Encrypt all of these into DETAILS with randomkey
            string key_details_unencrypted = gunitcharges_row.ACCOUNT_NO +
                                        SmartParametersV2016.fieldSeparator +
                                        gunitcharges_row.STATEMENT_ID +
                                        SmartParametersV2016.fieldSeparator +
                                        gunitcharges_row.BILL_DATE.ToString(SmartParametersV2016.sqliteformat) +
                                        SmartParametersV2016.fieldSeparator +
                                        gunitcharges_row.MPAN_MPRN +
                                        SmartParametersV2016.fieldSeparator +
                                        gunitcharges_row.UNIT_CHARGES_PERIOD_START.ToString(SmartParametersV2016.sqliteformat) +
                                        SmartParametersV2016.fieldSeparator +
                                        gunitcharges_row.UNIT_CHARGES_PERIOD_END.ToString(SmartParametersV2016.sqliteformat) +
                                        SmartParametersV2016.fieldSeparator +
                                        gunitcharges_row.UNITS_BAND.ToString();

            string key_details = (ourviewmodel.UDEK == "" ?
                key_details_unencrypted : SmartEncryptionV2016.DoTheBiz(key_details_unencrypted, SmartPhyllV2020.BuildEncryptKey(ourviewmodel.UDEK, gunitcharges_row.RANDOMKEY, derived), "", true));
            SmartUtility.GUnitChargesSQLite gunitcharges_enc = new SmartUtility.GUnitChargesSQLite()
            {
                USERNAME = gunitcharges_row.USERNAME,
                CUBEFACE_CODE = gunitcharges_row.CUBEFACE_CODE.ToString(),
                SUPPLIER_CODE = gunitcharges_row.SUPPLIER_CODE,
                BRAND_CODE = gunitcharges_row.BRAND_CODE,
                ACCOUNT_CREATED = gunitcharges_row.ACCOUNT_CREATED,
                KEY_DETAILS = key_details,
                RANDOMKEY = gunitcharges_row.RANDOMKEY,
                UNITS_TYPE = gunitcharges_row.UNITS_TYPE,
                UNITS = gunitcharges_row.UNITS,
                UNITS_RATE = gunitcharges_row.UNITS_RATE,
                UNIT_OF_MEASURE = gunitcharges_row.UNIT_OF_MEASURE,
                UNITS_COST = gunitcharges_row.UNITS_COST,
                Updated = gunitcharges_row.Updated
            };
            return gunitcharges_enc;
        }

        internal static void DoAllGUsage(MainViewModel ourviewmodel,
                                            UtilityViewModel utilityviewmodel,
                                            string sqliteformat)
        {
            string deriveds = "SmartUtility.GUsage";
            utilityviewmodel.Hezbollah.sqlite_g_usageList.Clear();
            if (utilityviewmodel.Hezbollah.g_usage_changesList.Count > 0)
            {
                // Store away everything from g_usageList
                SmartUtility.GUsage last_row;
                int count;

                // Trim off the last reading if its 0.00  this is because sometimes The supplier
                // hasn't updated the reading for the previous day, and as we record units AND dates
                // of reading them, there is a chance we would miss a 'true' value by recording a 'false' 0.00 instead
                count = utilityviewmodel.Hezbollah.g_usage_changesList.Count;
                while (count > 0)
                {
                    last_row = utilityviewmodel.Hezbollah.g_usage_changesList[count - 1];
                    if (last_row.USAGE_TOTAL == 0M)
                    {
                        count--;
                    }
                    else
                    {
                        break;
                    }
                }
                string rowAsString = "";
                FieldInfo[] myFields = typeof(SmartUtility.GUsage).GetFields(SmartParametersV2016.bindingFlags);

                foreach (SmartUtility.GUsage g_usage_row in utilityviewmodel.Hezbollah.g_usage_changesList)
                {
                    if (count == 0)
                    {
                        break;
                    }
                    //                          3
                    // The reading comes in as M  which needs to be convert to Kwh
                    // .. but at least it is to 3 decimal places

                    // For smart meter readings, we have to work out the UNITS
                    // but its too difficult to do it here - do it later =:-(

                    if (!string.IsNullOrEmpty(rowAsString))
                    {
                        // Separate each record with this
                        rowAsString += SmartParametersV2016.recordSeparator;
                    }
                    SmartUtility.GUsageSQLite abc = ConvertUtilityGUsage(ourviewmodel, g_usage_row, deriveds.Length);
                    utilityviewmodel.Hezbollah.sqlite_g_usageList.Add(abc);
                    rowAsString += SmartNibbyV2016.Build_StringNew<SmartUtility.GUsage>(myFields, g_usage_row, sqliteformat);
                    // Done slightly differently than the others
                    utilityviewmodel.Hezbollah.g_usageList.Add(g_usage_row);
                    count--;
                }
                if (!string.IsNullOrEmpty(rowAsString))
                {
                    utilityviewmodel.bollocks.AppendLine(deriveds +
                                        SmartParametersV2016.groupSeparator +
                                        "I" + SmartParametersV2016.groupSeparator +
                                        rowAsString);
                    utilityviewmodel.Hezbollah.g_usage_changesList.Clear();
                }
                utilityviewmodel.Hezbollah.g_usage_changesList.Clear();
            }
            return;
        }

        internal static SmartUtility.GUsageSQLite ConvertUtilityGUsage(MainViewModel ourviewmodel,
                                                        SmartUtility.GUsage gusage_row,
                                                        int derived)
        {
            // Encrypt all of these into DETAILS with randomkey
            string key_details_unencrypted = gusage_row.MPAN_MPRN +
                                        SmartParametersV2016.fieldSeparator +
                                        gusage_row.UNIQUE_INDEX.ToString();

            string key_details = (ourviewmodel.UDEK == "" ?
                key_details_unencrypted : SmartEncryptionV2016.DoTheBiz(key_details_unencrypted, SmartPhyllV2020.BuildEncryptKey(ourviewmodel.UDEK, gusage_row.RANDOMKEY, derived), "", true));
            SmartUtility.GUsageSQLite eusage_enc = new SmartUtility.GUsageSQLite()
            {
                USERNAME = gusage_row.USERNAME,
                CUBEFACE_CODE = gusage_row.CUBEFACE_CODE.ToString(),
                KEY_DETAILS = key_details,
                RANDOMKEY = gusage_row.RANDOMKEY,
                USAGE_DATETIME = gusage_row.USAGE_DATETIME,
                USAGE_TOTAL = gusage_row.USAGE_TOTAL,
                USAGE_VALUE = gusage_row.USAGE_VALUE,
                Updated = gusage_row.Updated
            };
            return eusage_enc;
        }
    }
}