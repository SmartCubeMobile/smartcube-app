//using System.Net;           // <== THIS IS DIFFERENT FOR LIGHSILVER than for Windows FORMS!!!
// What a complete pile of absolute fucking bollocks this shit is


using System.Reflection;
//using System.Net.Http;        // No more of this absolute bollocks shit
using System.Globalization;



//using System.Windows.Media;     // This is in PresentationCore.dll <= FUCKING OBVIOUSLY!! Microshit fucking wanking idiots

#if WINFORMS
using System.IO;
using SmartDashboard;
using MoreLinq;
using OxyPlot.WindowsForms;
using OxyPlot;
#endif

#if WPF
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Controls;
using System.Windows;
using OxyPlot.Wpf;
using OxyPlot;
using MoreLinq;
using System.IO;
using iText.Layout.Element;
#endif

#if WINUI
using Windows.Devices.Input;
using OxyPlot;
using MoreLinq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;
using System.Linq;
using System;
using CommunityToolkit.Helpers;
using Microsoft.UI.Xaml;
using CommunityToolkit.WinUI.UI.Controls;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.UI.Xaml.Media;
using Windows.Storage.Streams;
using System.Reflection.Emit;
using System.IO;
using Microsoft.UI.Xaml.Data;
#endif

#if ANDROIDX
using Android.Graphics;
using OxyPlot;
using Android.Content;
using OxyPlot.Xamarin.Android;
using Android.Views;
using AndroidX.ViewPager2.Adapter;
using AndroidX.ViewPager2.Widget;
using Google.Android.Material.Tabs;
using static Google.Android.Material.Tabs.TabLayoutMediator;
using AndroidX.AppCompat.App;
#endif

#if SMARTMAUI
using CommunityToolkit.Maui.Views;
using SkiaSharp.Views.Maui.Controls;
using CommunityToolkit.Maui.Core;
using System.Collections.Generic;
using LiveChartsCore;
#endif

namespace SmartCubeMobile
{
    public static class ListExtensions
    {
        public static void AddRange<T>(this List<T> lista, IEnumerable<T> items)
        {
            foreach (var item in items) // Checked
            {
                lista.Add(item);
            }
        }
    }

#if ANDROIDX
    public static class UtilityViewModelStore
    {
        private static readonly Dictionary<string, (MainViewModel main, UtilityViewModel utility)> store = new();
        public static void Add(string key, MainViewModel main, UtilityViewModel utility)
        {
            store[key] = (main, utility);
        }
        public static void Add(string key, UtilityViewModel utility)
        {
            store[key] = (null, utility);
        }
        public static (MainViewModel main, UtilityViewModel utility)? Get(string key)
        {
            return store.TryGetValue(key, out var value) ? value : null;
        }
        public static void Remove(string key)
        {
            store.Remove(key);
        }
    }
    public class UtilityStrategy : Java.Lang.Object, ITabConfigurationStrategy
    {
        internal static List<string> utilityfragmentTitles = new List<string>()
            { "Costs", "Charts", "Bills", "Readings", "Breakdown" };

        public void OnConfigureTab(TabLayout.Tab p0, int p1)
        {
            p0.SetText(utilityfragmentTitles[p1]);
        }
    }

    public class UtilityViewPager2Adapter : FragmentStateAdapter
    {
        private readonly int itemCount;
        private MainViewModel ourviewmodel;
        private UtilityViewModel utilityviewmodel;
        public UtilityViewPager2Adapter(AndroidX.Fragment.App.FragmentManager fragmentManager, AndroidX.Lifecycle.Lifecycle lifecycle, int itemCount, MainViewModel mainvm, UtilityViewModel utilityvm) : base(fragmentManager, lifecycle)
        {
            this.itemCount = itemCount;
            this.ourviewmodel = mainvm;
            this.utilityviewmodel = utilityvm;
        }
        public override int ItemCount => itemCount;

        public override AndroidX.Fragment.App.Fragment CreateFragment(int position)
        {
            AndroidX.Fragment.App.Fragment utilityfragment = new AndroidX.Fragment.App.Fragment();

            switch (position)
            {
                case 0:
                    utilityfragment = UtilityCostsFragment.NewInstance(utilityviewmodel);
                    break;
                case 1:
                    utilityfragment = UtilityChartsFragment.NewInstance(ourviewmodel, utilityviewmodel);
                    break;
                case 2:
                    utilityfragment = UtilityBillsFragment.NewInstance(utilityviewmodel);
                    break;
                case 3:
                    utilityfragment = UtilityReadingsFragment.NewInstance(utilityviewmodel);
                    break;
                case 4:
                    utilityfragment = UtilityBreakdownFragment.NewInstance(utilityviewmodel);
                    break;
            }
            return utilityfragment;
        }
    }
#endif

    public class SmartUtilityV2022
    {
        internal static async void ButtonAutoSwitchUtilityClick(object sender, object e,
                                                        SignInViewModel signinviewmodel,
                                                        MainViewModel ourviewmodel,
                                                        UtilityViewModel utilityviewmodel)
        {
            if (sender != null &&
#if WINFORMS || WPF || SMARTMAUI
                (FrontEndGUI.DecodeEventFlags(e) != null))
#endif
#if WINUI
                (FrontEndGUI.DecodeRoutedEventFlags(e) != null))
#endif
#if ANDROIDX
                (FrontEndGUI.DecodeEventFlags(e) != null))
#endif
            {
                if (!await SmartUtilityV2022.ButtonAutoSwitchUtilityClickActual(signinviewmodel,
                                                                        ourviewmodel,
                                                                        utilityviewmodel))
                {
                    if (!await SmartRoutinesV2018.CheckTrace(ourviewmodel, ourviewmodel.quitCts.Token, 0, 0, "Problem 24: AutoSwitch Utility failed"))
                    {
                        return;
                    }
                }
            }
            return;
        }

#if ANDROIDX
        //        internal async void ButtonAutoSwitchUtilityClick(object sender, EventArgs e)
        //       {
        //            if (sender != null && e != null)
        //            {
        //                if (!await SmartUtilityV2022.ButtonAutoSwitchUtilityClickActual(SignIn.signinviewmodel,
        //                                                                        ourviewmodel,
        //                                                                        utilityviewmodel))
        //                {
        //                    await SmartRoutinesV2018.CheckTrace(ourviewmodel, ourviewmodel.userToken, 0, 0, "AutoSwitch Utility failed");
        //                }
        //            }
        //            return;
        //        }
#endif

#if ANDROIDX
        public static void SetUpUtilityViewPager(View thisView, AndroidX.Fragment.App.FragmentManager fm, AndroidX.Lifecycle.Lifecycle lfc, MainViewModel ourviewmodel, UtilityViewModel utilityviewmodel)
        {
            TabLayout utilitytabLayout = thisView.FindViewById<TabLayout>(Resource.Id.utilitytabLayout);
            ViewPager2 utilitypager2 = thisView.FindViewById<ViewPager2>(Resource.Id.utilityviewPager);
            UtilityViewPager2Adapter utilityAdapter = new UtilityViewPager2Adapter(fm, lfc, UtilityStrategy.utilityfragmentTitles.Count, ourviewmodel, utilityviewmodel);
            utilitypager2.Adapter = utilityAdapter;
            utilitypager2.UserInputEnabled = false;
            utilityAdapter.NotifyDataSetChanged();
            new TabLayoutMediator(utilitytabLayout, utilitypager2, new UtilityStrategy()).Attach();

            return;
        }
#endif

        //        internal static async Task<bool> Utility_Check_Meter_Async(
        //#if WINFORMS
        //                                                    MainProcess process_components,
        //                                                    bool decode,
        //                                                    //CheckBox checkBoxStopOnError,
        //                                                    //CheckBox checkBoxQuiet,
        //                                                    //CheckBox checkBoxInsert_SmartSwitch,
        //                                                    //CheckBox checkBoxDownload_Bills,
        //#endif

        //                                                    SignInViewModel signinviewmodel,
        //                                                    MainViewModel ourviewmodel,
        //                                                    UtilityViewModel utilityviewmodel,
        //                                                    char cubeface_code,
        //                                                    DateTime time_now,
        //                                                    char utility_resource_code,
        //                                                    bool utility_submit_button, // WHich one though
        //                                                    string defaultDate_string,
        //                                                    DateTime defaultDate,
        //                                                    //char target_category_code,                 // Should either be F, U or default
        //                                                    short target_supplier_code,         // Should be known
        //                                                    short target_brand_code,            // Should be known
        //                                                    string target_account_no,           // Might not be known
        //                                                    DateTime target_created)            // Comes in as today'sdate
        //        {
        //            string urgent_message = "";

        //            string target_supplier_prfix = "",
        //                        target_supplier_xxx = "";
        //            bool target_use_proxy = false;
        //            string target_user_id = "",
        //                        target_password = "",
        //                        target_udprn = SmartParametersV2016.udprnDefault,
        //                        target_bill_token = "";
        //            char target_bill_currency_symbol = SmartParametersV2016.defaultChar,
        //                        target_bill_denomination_symbol = SmartParametersV2016.defaultChar,
        //                        target_bill_currency_separator = SmartParametersV2016.defaultChar,
        //                        target_bill_thousands_separator = SmartParametersV2016.defaultChar,
        //                        target_bill_prfix = SmartParametersV2016.defaultChar;
        //            DateTime[] target_last_datetime = new DateTime[2],
        //                        last_datetime = new DateTime[2];
        //            DateTime next_connection;


        //            // We have to have a Network before we can do anything ...
        //            if (FrontEndGUI.CompareLedColour(6, ourviewmodel.redColour) ||
        //                FrontEndGUI.CompareLedColour(8, ourviewmodel.redColour))
        //            {
        //                // If either Led6 is not White or Led8 is Red, then we just don't go here ... (very rare)
        //                return false;
        //            }
        //            if (FrontEndGUI.CompareLedColour(5, ourviewmodel.greenColour))
        //            {
        //                // If Led5 is Green, then we just don't go here ... because we already are!
        //                return false;
        //            }
        //
        //            // The Submit button is only ever 'considered' when the network is available
        //            if (!SmartNibbyV2016.Network_Availability())
        //            {
        //                FrontEndGUI.SetLedColour(ourviewmodel, 5, ourviewmodel.orangeColour);
        //                // We can get to here because both Led6 and Led8 are either Green or Orange
        //                // But we have no network so its pointless checking for last connection
        //                // dates and tiutilityviewmodel.
        //                return false;
        //            }

        //            //if (target_resource_code == SmartParametersV2016.defaultResourceCode ||
        //            //        (target_resource_code != SmartParametersV2016.defaultResourceCode &&
        //            //         target_cubeface_code == cubeface_code)) // <= These will all be 'Active'
        //            //    {

        //            bool read_it = false;
        //            // This could be Default 

        //            if (!Utility_Check_Primary(utility_submit_button,
        //                                defaultDate,
        //                                rf last_datetime,
        //                                rf target_last_datetime,
        //                                rf read_it))
        //            {
        //                // We have Led6 on Red OR Led8 on Red OR we have no network
        //                return false;
        //            }

        //            // If we have to skip the auto-scrape load then make
        //            // sure ... ==>
        //            // Look for updates to all the SMARTUTILITY Tariffs
        //            if (!await Utility_Check_Meter_Skip_Async(ourviewmodel,
        //                                                        utilityviewmodel,
        //                                                        cubeface_code,
        //                                                        time_now))
        //            {
        //                // Hamas could be null or the consumers list could be 0
        //                // Or the updates to the Tariffs went wrong
        //                return false;
        //            }

        //            // Read_it may be true or false here - true if the Submit button
        //            // has been presed, false otherwise.  But we might have a scheduled
        //            // connection so even if read-it is false, we press on ...

        //            // What if we are Submitting with stuff already there?
        //            char ce_resource_code = SmartParametersV2016.defaultResourceCode;
        //            short ce_supplier_code = 0,
        //                    ce_brand_code = 0;
        //            string ce_user_id = "";

        //            DateTime last_withdrawn_date = utilityviewmodel.withdrawn_date;


        //            // The BIG TWO!! or THREE!! or FOur
        //            foreach (SmartUtility.ConsumersView consumers_view_row in viewlst)
        //            {
        //                // Is Connection time after 'now'?
        //                if (Utility_Check_Connection(consumers_view_row,
        //                                        defaultDate,
        //                                        time_now,
        //                                        utility_submit_button))
        //                {
        //                    // Next connection is behind 'now' so DO THIS ONE and update its time
        //                    // so if it succeeds we don't do it again

        //                    // We have a CONSUMERS_ROW because we are inside the loop!  Its pointless
        //                    // not to use the ACCOUNT_NO and CREATED if they are there!!

        //                    target_supplier_code = consumers_view_row.SUPPLIER_CODE;
        //                    target_brand_code = consumers_view_row.BRAND_CODE;
        //                    target_account_no = consumers_view_row.ACCOUNT_NO;
        //                    //target_created = consumers_view_row.CREATED;

        //                    // But perhaps the UserId/Password has been changed and we need to use 
        //                    // the one sent in on a SUBMIT??
        //                    if (!utility_submit_button)
        //                    {
        //                        // Its an auto-connection - go with what we've got
        //                        target_user_id = consumers_view_row.USER_ID;
        //                        target_password = consumers_view_row.USER_PASSWORD;
        //                    }
        //                    Utility_Check_Resource(consumers_view_row,
        //                                                rf target_last_datetime,
        //                                                rf target_udprn,
        //                                                rf target_udprn,
        //                                                rf ce_resource_code,
        //                                                rf ce_supplier_code,
        //                                                rf ce_brand_code,
        //                                                rf ce_user_id);
        //                    read_it = true;
        //                }
        //                if (read_it)
        //                {
        //                    break;  // Only one initiator at any one time (but two might be scraped)
        //                }
        //            }
        //
        //            if (read_it)
        //            {
        //                // NOW! we flag up the Meter
        //                FrontEndGUI.SetLedColour(ourviewmodel, 5, ourviewmodel.greenColour);
        //
        //                // This is all just in case the Last Update for Electricity is
        //                // different than the Last Update for Gas for the same Supplier
        //
        //                // Form collection
        //                foreach (SmartUtility.ConsumersView consumers_view_row in SmartSpikeUtilityV2017.Consumers_UserList(utilityviewmodel, ce_user_id))
        //                {
        //                    // Set the 'other one' in
        //                    Utility_Check_WhichResource(consumers_view_row,
        //                                    rf target_last_datetime,
        //                                    rf target_udprn,
        //                                    rf target_udprn);
        //                    break;  // Only one
        //                }

        //                next_connection = time_now.AddDays(1);
        //                // Good to go ? Get all the bits and pieces
        //                // This routine will return FALSE if the selected supplier
        //                // has no URL or prfix (i.e. they are empty) so we will never
        //                // even call READ_GENERAL_METER ...
        //                if (Get_Bits(ourviewmodel,
        //                                utilityviewmodel,
        //                                target_supplier_code,
        //                                utility_resource_code,
        //                                rf target_supplier_prfix,
        //                                rf target_supplier_xxx,
        //                                rf target_bill_token,
        //                                rf target_bill_currency_symbol,
        //                                rf target_bill_denomination_symbol,
        //                                rf target_bill_currency_separator,
        //                                rf target_bill_thousands_separator,
        //                                rf target_bill_prfix,
        //                                rf target_use_proxy))
        //                {
        //                    // Turn off the Quit and Submit buttons until we have finished scraping
        //                    // as well as the Electricity, Gas and Dual Fuel radio buttons
        //                    // ... but not the Cancel button
        //                    TurnOffUtilityStatus(
        //#if WINFORMS
        //                                        process_components,
        //#endif
        //                                        utilityviewmodel);
        //                    // If White then set to Green
        //                    // i.e. Red always stays showing if it happened
        //                    if (FrontEndGUI.CompareLedColour(6, ourviewmodel.whiteColour);)
        //                    {
        //                      FrontEndGUI.SetLedColour(ourviewmodel, 5, ourviewmodel.greenColour);
        //                    }
        //                    // We don't wait here so it goes banging on ...
        //                    // But I can't take the chance that the Scrapers don't work ..
        //                    bool download_bills = true;

        //                    // Churn, churn, churn ..distracting witter ...
        //                    // Notice there is no 'true' or 'false' check on this next call
        //                    // This is because it is SO FUCKING COMPLICATED to check whether it succeeds or fails ...
        //                    await SmartScraperV2016.General_Meter(
        //#if WINFORMS
        //                                                        decode,
        //#endif
        //                                                        signinviewmodel,
        //                                                        ourviewmodel,
        //                                                        utilityviewmodel,
        //                                                        //checkBoxStopOnError,
        //                                                        //checkBoxQuiet,
        //                                                        //checkBoxInsert_SmartSwitch,
        //                                                        //checkBoxDownload_Bills,                        
        //                                                        time_now,           // For LAST_UPDATE
        //                                                        utilityviewmodel.analysis_cost,
        //                                                        utility_submit_button,
        //                                                        defaultDate_string,
        //                                                        defaultDate,
        //                                                        target_supplier_prfix,
        //                                                        target_supplier_xxx,
        //                                                        target_supplier_code,   // <== Supplier
        //                                                        target_brand_code,      // <== Brand
        //                                                        target_account_no,      // <== Account No
        //                                                        target_created,         // <== Created (from Accounts) ormaybe today's date
        //                                                        target_bill_token,
        //                                                        target_bill_currency_symbol,
        //                                                        target_bill_denomination_symbol,
        //                                                        target_bill_currency_separator,
        //                                                        target_bill_thousands_separator,
        //                                                        target_bill_prfix,
        //                                                        target_use_proxy,
        //                                                        download_bills,
        //                                                        target_user_id,
        //                                                        target_password,
        //                                                        target_udprn,
        //                                                        next_connection);        // This is no longer an array



        //                    // No ... if either of the LEDs are Red, then there has been a problem so DO update the
        //                    // next connection label to ensure we don't cycle round scraping away after a failure ...
        //                    utilityviewmodel.NextConnectionMessage = next_connection.ToString(utilityviewmodel.utilityDisplayCulture);
        //                    // These can all be turned on now
        //                    TurnOnUtilityStatus(
        //#if WINFORMS
        //                                        process_components,
        //#endif
        //                                        utilityviewmodel);
        //                }
        //            }
        //            else
        //            {
        //                // We're not doing a read ...
        //                // Is autoSwitch on and do we have to SWITCH OUR TARIFF??!!?
        //                // TO A NEW SUPPLIER????!!!!!!!
        //                if (utilityviewmodel.autoswitch_pending)
        //                {
        //                    if (utilityviewmodel.UtilityCosts != null)
        //                    {
        //                        if (utilityviewmodel.UtilityCosts.Count > 0)
        //                        {
        //                            utilityviewmodel.report = "";
        //                            utilityviewmodel.bollocks = new StringBuilder();

        //                            utilityviewmodel.switch_info = new SmartUtility.SwitchInfo();

        //                            SmartUtility.AnalysisCostsView analysis_costs_row = new SmartUtility.AnalysisCostsView();
        //                            if (utilityviewmodel.UtilityCostsPosition == -1)
        //                            {
        //                                analysis_costs_row = utilityviewmodel.UtilityCosts.First();
        //                            }
        //                            else
        //                            {
        //                                analysis_costs_row = utilityviewmodel.UtilityCosts[utilityviewmodel.UtilityCostsPosition];
        //                            }
        //                            // Reset it
        //                            utilityviewmodel.UtilityCostsPosition = -1;
        //#if WINFORMS
        //                            analysis_costs_row = utilityviewmodel.UtilityCosts.First();
        //                            //analysis_costs_row = (utilityviewmodel.dataGridCostsSource.ItemsSource as List<AnalysisCostsView>).First();
        //                            //analysis_costs_row = new AnalysisCostsView(Convert.ToInt16(columns[0].FormattedValue),
        //                            //                            Convert.ToInt16(columns[1].FormattedValue),
        //                            //                            columns[2].FormattedValue.ToString(),
        //                            //                            Convert.ToInt32(columns[3].FormattedValue),
        //                            //                            columns[4].FormattedValue.ToString(),
        //                            //                            ConvertDecimal(columns[5].FormattedValue),
        //                            //                            columns[6].FormattedValue.ToString(),
        //                            //                            columns[7].FormattedValue.ToString(),
        //                            //                            columns[8].FormattedValue.ToString(),
        //                            //                            columns[9].FormattedValue.ToString(),
        //                            //                            ConvertDecimal(columns[10].FormattedValue));
        //#endif

        //#if WINFORMS
        //                            // Need to convert this to analysis_costs_row
        //                            analysis_costs_row = utilityviewmodel.UtilityCosts.First(); // Fix this Ray Should be selected item
        //                                                                                        //DataGridViewCellCollection columns = utilityviewmodel.DataGridCosts.First();
        //                                                                                        //analysis_costs_row = (AnalysisCostsView)utilityviewmodel.dataGridCostsSource.Items[0];

        //                            //analysis_costs_row = new AnalysisCostsView(Convert.ToInt16(columns[0].FormattedValue),
        //                            //                        Convert.ToInt16(columns[1].FormattedValue),
        //                            //                        columns[2].FormattedValue.ToString(),
        //                            //                        Convert.ToInt32(columns[3].FormattedValue),
        //                            //                        columns[4].FormattedValue.ToString(),
        //                            //                        ConvertDecimal(columns[5].FormattedValue),
        //                            //                        columns[6].FormattedValue.ToString(),
        //                            //                        columns[7].FormattedValue.ToString(),
        //                            //                        columns[8].FormattedValue.ToString(),
        //                            //                        columns[9].FormattedValue.ToString(),
        //                            //                        ConvertDecimal(columns[10].FormattedValue));
        //#endif

        //#if Andyroid
        //                          // Need to convert this to analysis_costs_row
        //                          // Couldn't have EVER down this without the help of Eric Schmeck
        //                          // https://forums.xamarShit.com/discussion/46785/how-to-cast-java-lang-object-to-specified-class-cant-convert-type-java-lang-object-tomyclass?
        //                          var bob = utilityviewmodel.DataGridCosts; //  SelectedItem.GetType().GetProperty("Instance");
        //                          analysis_costs_row = bob.GetValue(dutilityviewmodel.DataGridCosts.SelectedItem, null) as AnalysisCostsView;
        //#endif
        //                            // Its not a problem if nothing wants to Switch ...
        //                            string err_message = "";
        //                            if (!await SmartSwitcherV2018.Utility_Check_Switch(ourviewmodel,
        //                                                                    utilityviewmodel,
        //                                                                    utility_resource_code,
        //                                                                    analysis_costs_row,
        //                                                                    utilityviewmodel.switch_info,
        //                                                                    em => err_message = em))
        //                            {
        //                                await SmartBobV2017.ListenerAsync(ourviewmodel, ourviewmodel.userToken, analysis_costs_row.BrandCode, analysis_costs_row.SupplierCode, "Switch check failed to " + utilityviewmodel.switch_info.proposed_brand_name + "/" + utilityviewmodel.switch_info.proposed_tariff_name);
        //                            }
        //                            else
        //                            {
        //                                //bool status = false;
        //                                // We are going to get info? Better turn off some buttons first!
        //                                //
        //                                // These can all be turned off now, except the Cancel button
        //                                TurnOffUtilityStatus(
        //#if WINFORMS
        //                                                            process_components,
        //#endif
        //                                                                utilityviewmodel);
        //                                // Pop-up the Utility.Accounts window
        //                                // when we are using energylinx we have GOT to have all of these
        //                                if (string.IsNullOrEmpty(utilityviewmodel.switch_info.title) ||
        //                                    string.IsNullOrEmpty(utilityviewmodel.switch_info.first_name) ||
        //                                    string.IsNullOrEmpty(utilityviewmodel.switch_info.last_name) ||
        //                                    string.IsNullOrEmpty(utilityviewmodel.switch_info.telephone) ||
        //                                    string.IsNullOrEmpty(utilityviewmodel.switch_info.email) ||
        //                                    string.IsNullOrEmpty(utilityviewmodel.switch_info.bank_account_name) ||
        //                                    (utilityviewmodel.switch_info.bank_account_sort_code.Length != 6) ||
        //                                    (utilityviewmodel.switch_info.bank_account_number.Length != 8) ||
        //                                    (utilityviewmodel.switch_info.bank_account_number.IndexOf("*") >= 0))
        //                                {
        //#if WINFORMS
        //                                    utilityviewmodel.switch_info.title = "Mrs";
        //                                    utilityviewmodel.switch_info.birth_day = 29;
        //                                    utilityviewmodel.switch_info.birth_month = 3;
        //                                    utilityviewmodel.switch_info.birth_year = 1927;
        //                                    utilityviewmodel.switch_info.email = "ts21nan@ntlworld.com";
        //                                    utilityviewmodel.switch_info.bank_account_name = "Mrs I Slater"; // This has 18 character limit on the switch
        //                                    utilityviewmodel.switch_info.bank_account_sort_code = "402126";  // Maximum 6
        //                                    utilityviewmodel.switch_info.bank_account_number = "41038230";   // Maximum 8

        //                                    //switch_info.account_no = components.textBoxFirstFourDigits.Text;
        //                                    utilityviewmodel.switch_info.status = "";
        //#endif


        //                                    utilityviewmodel.switch_brand_name = analysis_costs_row.SupplierName;
        //                                    utilityviewmodel.switch_tariff_name = analysis_costs_row.TariffName;
        //                                    utilityviewmodel.XRAY.bank_account_name = utilityviewmodel.switch_info.bank_account_name;
        //                                    utilityviewmodel.XRAY.bank_account_sort_code = utilityviewmodel.switch_info.bank_account_sort_code;
        //                                    utilityviewmodel.XRAY.bank_account_number = utilityviewmodel.switch_info.bank_account_number;

        //                                    //var taskResult = new TaskCompletionSource<bool>();
        //                                    // We are GOOD TO GO!!! 11:00am on 3rd July 2017!!! 
        //                                    if (!await SmartSwitcherV2018.General_Switcher(


        //                                                                    ourviewmodel,
        //                                                                    utilityviewmodel,
        //                                                                    utility_resource_code,
        //                                                                    true,
        //#if WINFORMS
        //                                                                    decode,
        //#endif
        //                                                                    utilityviewmodel.switch_info))
        //                                    {
        //                                        await SmartBobV2017.ListenerAsync(ourviewmodel, ourviewmodel.userToken, analysis_costs_row.BrandCode, analysis_costs_row.SupplierCode, "Switch failed from " + utilityviewmodel.switch_info.proposed_brand_code + "/" + utilityviewmodel.switch_info.proposed_tariff_code);

        //                                    }
        //                                    else
        //                                    {
        //                                        // Here - we have either switched E or G or both (Dual)
        //                                        // Send an e-mail to Anna
        //                                        if (!await SmartSwitcherV2018.Switch_Email_New(ourviewmodel,
        //                                                                                utilityviewmodel,
        //                                                                                utilityviewmodel.switch_info,
        //                                                                                utilityviewmodel.report,
        //                                                                                utilityviewmodel.bollocks))
        //                                        {
        //                                            await SmartBobV2017.ListenerAsync(ourviewmodel, ourviewmodel.userToken, analysis_costs_row.BrandCode, analysis_costs_row.SupplierCode, "Failed send to " + utilityviewmodel.switch_info.email + " for " + analysis_costs_row.SupplierName + "/" + analysis_costs_row.TariffName);

        //                                        }
        //                                        else
        //                                        {
        //                                            // There was a success
        //                                            FrontEndGUI.SetLedColour(ourviewmodel, 7, ourviewmodel.greenColour);
        //                                            // Clear down ready for an autoswitch on the next resource
        //                                            utilityviewmodel.autoswitch_pending = false;
        //                                        }
        //                                    }
        //                                }
        //                                // These can all be turned on now, except Cancel which is never turned off
        //                                TurnOnUtilityStatus(
        //#if WINFORMS
        //                                                            process_components,
        //#endif
        //                                                            utilityviewmodel);
        //                            }
        //                        }
        //                    }
        //                }

        //                if (last_withdrawn_date != utilityviewmodel.withdrawn_date)
        //                {
        //#if WINFORMS || WPF
        //                    ourviewmodel.WithdrawnDateMessage = "Tariffs and Prices as of: " + utilityviewmodel.withdrawn_date.ToString(utilityviewmodel.utilityDisplayCulture);
        //#endif
        //                }
        //
        //            }
        //
        //            // Turn Off the Network Led
        //            if (FrontEndGUI.CompareLedColour(5, ourviewmodel.greenColour))
        //            {
        //               FrontEndGUI.SetLedColour(ourviewmodel, 5, ourviewmodel.orangeColour);
        //            }
        //            return true;
        //        }

        internal static async Task<bool> First_Throw(MainViewModel ourviewmodel, UtilityViewModel utilityviewmodel)
        {
#if WINFORMS
            if (utilityviewmodel.console)
            {
                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel,
                    (DateTime.Now + ourviewmodel.utcOffset).ToString(SmartParametersV2016.sqliteformat) +   // Local time 
                                                    SmartParametersV2016.space + utilityviewmodel.brand_code + SmartParametersV2016.space + utilityviewmodel.supplier_code + SmartParametersV2016.space + utilityviewmodel.next_routine);
            }
#endif
#if WPF  || WINUI || SMARTMAUI
            if (utilityviewmodel.examine.scrape)
            {
                if (!await SmartRoutinesV2018.CheckTrace(ourviewmodel, utilityviewmodel.utilityToken, utilityviewmodel.supplier_code, utilityviewmodel.brand_code, SmartParametersV2016.Utility.ToString() + " " + utilityviewmodel.next_routine))
                {
                    return false;
                }
            }
#endif
#if ANDROIDX
            if (utilityviewmodel.examine.scrape)
            {
                if (!await SmartRoutinesV2018.CheckTrace(ourviewmodel, utilityviewmodel.utilityToken, utilityviewmodel.supplier_code, utilityviewmodel.brand_code, SmartParametersV2016.Utility.ToString() + " " + utilityviewmodel.next_routine))
                {
                    return false;
                }
            }
#endif
            return true;
        }

        internal static async Task<bool> Prelude(MainViewModel ourviewmodel,
                                                UtilityViewModel utilityviewmodel,
                                                List<KeyValuePair<string, string>> keyValues)
        {
            if (!string.IsNullOrEmpty(ourviewmodel.errorMessage) ||
                    utilityviewmodel.next_routine == "LOGOUT")
            {
                utilityviewmodel.target_pathname = utilityviewmodel.logout_pathname;
                utilityviewmodel.next_routine = "LOGOUT";
                return false;
            }
            if (utilityviewmodel.next_routine != "HOME")
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
                    utilityviewmodel.htmlDocument = await SmartBobV2017.Scraper_Generic_Post(ourviewmodel, utilityviewmodel.utilityToken, keyValues, utilityviewmodel);
                    if (!string.IsNullOrEmpty(ourviewmodel.errorMessage))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        internal static async Task<bool> Last_Throw(
#if ANDROIDX
                                                    AppCompatActivity meterActivity,
#endif
                                                    MainViewModel ourviewmodel,
                                                    UtilityViewModel utilityviewmodel)
        {
            if (!utilityviewmodel.login_attempted)
            {
                // We can assume the login failed
#if WINFORMS
                if (utilityviewmodel.console)
                {
#endif
                await SmartRoutinesV2018.TextBlockUpdate(
#if ANDROIDX
                                                        meterActivity,
#endif
                                                        ourviewmodel, " Login failed");
#if WINFORMS
                }
#endif
            }
            else
            {
                // We can assume the login succeeded
#if WINFORMS
                if (utilityviewmodel.console)
                {
#endif
                await SmartRoutinesV2018.TextBlockUpdate(
#if ANDROIDX
                                                        meterActivity,
#endif
                                                        ourviewmodel, " Logged out");
#if WINFORMS
                }
#endif
            }
            return true;
        }

        internal static async Task<bool> Common_Logout(
#if ANDROIDX
                                                    AppCompatActivity meterActivity,
#endif
                                                    MainViewModel ourviewmodel,
                                                    UtilityViewModel utilityviewmodel,
                                                    bool TextBox_Active)
        {
            // This is THE place to check if we have any Bills In or Bills Out
            // We may have neither BUT we might have a TARIFF_CODE which we will lose
            // if we don't store it somewhere.  The obvious place is TariffDetails_out
            // so we must look 

            // This just tidies up so we
            // drop out of the loop as we have nothing more to do
            if (TextBox_Active)
            {
                await SmartRoutinesV2018.TextBlockUpdate(
#if ANDROIDX
                                                             meterActivity,
#endif
                                                            ourviewmodel,
                                                            "LoggingOut");
            }
            utilityviewmodel.keep_looping = false;
            return true;
        }

        internal static async Task<bool> Check_Examine(MainViewModel ourviewmodel,
                                                    UtilityViewModel utilityviewmodel)
        {
            if (utilityviewmodel.examine.scrape)
            {
                if (!await SmartRoutinesV2018.CheckTrace(ourviewmodel, utilityviewmodel.utilityToken, utilityviewmodel.supplier_code, utilityviewmodel.brand_code, SmartParametersV2016.Utility.ToString() + " " + ourviewmodel.errorMessage))
                {
                    return false;
                }
                return true;
            }
            return false;
        }

        //internal static void Set_TARIFF_PAYMENT(UtilityViewModel utilityviewmodel)
        //                                        //rf int tariff_code,
        //                                        //rf char payment_plan)
        //{
        //    switch (utilityviewmodel.resource_code)
        //    {
        //        case SmartParametersV2016.Electricity:
        //            utilityviewmodel.tariff_code = utilityviewmodel.sparks.tariff_code;
        //            utilityviewmodel.payment_plan = utilityviewmodel.sparks.payment_method;
        //            break;
        //        case SmartParametersV2016.Gas:
        //            utilityviewmodel.tariff_code = utilityviewmodel.smell.tariff_code;
        //            utilityviewmodel.payment_plan = utilityviewmodel.smell.payment_method;
        //            break;
        //        default:
        //            break;
        //    }
        //    return;
        //}

        internal static void Set_UDPRN_MPAN_MPRN(UtilityViewModel utilityviewmodel)
        {
            switch (utilityviewmodel.resource_code)
            {
                case SmartParametersV2016.Electricity:
                    //udprn = utilityviewmodel.sparks.udprn;
                    utilityviewmodel.mpan_mprn = utilityviewmodel.sparks.MPAN;
                    utilityviewmodel.unknown = SmartParametersV2016.unknownMPAN;
                    break;
                case SmartParametersV2016.Gas:
                    //udprn = utilityviewmodel.smell.udprn;
                    utilityviewmodel.mpan_mprn = utilityviewmodel.smell.MPRN;
                    utilityviewmodel.unknown = SmartParametersV2016.unknownMPRN;
                    break;
                default:
                    break;
            }
            return;
        }

        //internal static void Set_UDPRN(UtilityViewModel utilityviewmodel,
        //                                  rf string udprn)
        //{
        //    switch (utilityviewmodel.resource_code)
        //    {
        //        case SmartParametersV2016.Electricity:
        //            udprn = utilityviewmodel.sparks.udprn;
        //            break;
        //        case SmartParametersV2016.Gas:
        //            udprn = utilityviewmodel.smell.udprn;
        //            break;
        //        case SmartParametersV2016.DualFuel:
        //            udprn = "";
        //            break;
        //        default:
        //            break;
        //    }
        //    return;
        //}

        //internal static void Set_RESOURCES_METER_SERIAL_NO(UtilityViewModel utilityviewmodel)
        //                                        //rf char resource_code,
        //                                        //rf string resource_type,
        //                                        //rf string meter_serial_no)
        //{
        //    switch (utilityviewmodel.resource_code)
        //    {
        //        case SmartParametersV2016.Electricity:
        //            utilityviewmodel.resource_code = utilityviewmodel.resource_code;
        //            utilityviewmodel.resource_type = utilityviewmodel.sparks.resource_type;
        //            utilityviewmodel.meter_serial_no = utilityviewmodel.sparks.meter_serial_no;
        //            break;
        //        case SmartParametersV2016.Gas:
        //            utilityviewmodel.resource_code = utilityviewmodel.resource_code;
        //            utilityviewmodel.resource_type = utilityviewmodel.smell.resource_type;
        //            utilityviewmodel.meter_serial_no = utilityviewmodel.smell.meter_serial_no;
        //            break;
        //        default:
        //            break;
        //    }
        //    return;
        //}

        internal static async Task<bool> Determine_UDPRNZ(MainViewModel ourviewmodel,
                                                            UtilityViewModel utilityviewmodel,
                                                            string account_address)
        {
            // However we do need a POSTCODE and an ACCOUNT_ADDRESS to make this happen
            if (string.IsNullOrEmpty(utilityviewmodel.postcode) ||
                string.IsNullOrEmpty(account_address))
            {
                ourviewmodel.errorMessage = "Cannot determine UDPRN with: " + utilityviewmodel.postcode + SmartParametersV2016.space + account_address;
                return false;
            }

            utilityviewmodel.ADDRESS = new GenericAddress();


            string target_postcode = utilityviewmodel.postcode;
            string target_address = account_address;
            utilityviewmodel.udprn = "";
            utilityviewmodel.property_value = "";
            utilityviewmodel.House_No = "";
            utilityviewmodel.House_Name = "";
            utilityviewmodel.Street = "";
            utilityviewmodel.City_Town = "";

            string old_prfix = utilityviewmodel.prfix_xxx;
            string old_target_pathname = utilityviewmodel.target_pathname;

            //int switch_now = switch_now_path.IndexOf("promo");
            //if (switch_now >= 0)
            //{
            //    switch_now_path = switch_now_path.Substring(switch_now).Replace(@"/", "-");
            //}
            utilityviewmodel.target_pathname = "ws/index.html?callback=none&fields=" + Uri.EscapeDataString(utilityviewmodel.postcode) + "&wsFunction=GetAddressListJson";
            string json = await SmartBobV2017.Scraper_Generic_Get_Json(ourviewmodel, utilityviewmodel.utilityToken, utilityviewmodel);
            if (!string.IsNullOrEmpty(ourviewmodel.errorMessage))
            {
                return false;
            }
            utilityviewmodel.prfix_xxx = old_prfix;
            utilityviewmodel.target_pathname = old_target_pathname;

            // Find the UDPRN cos we have the address and Post Code
            if (!SmartNibbyV2016.PS_GetAddress(utilityviewmodel,
                                            json,
                                            target_postcode,
                                            target_address,
                                            utilityviewmodel.ADDRESS))
            {
                ourviewmodel.errorMessage = "Cannot determine address from UDPRN: " + utilityviewmodel.udprn;
                return false;
            }

            switch (utilityviewmodel.resource_code)
            {
                case SmartParametersV2016.Electricity:
                    if (string.IsNullOrEmpty(utilityviewmodel.ADDRESS.electricity.MPAN))
                    {
                        utilityviewmodel.ADDRESS.electricity.MPAN = SmartParametersV2016.unknownMPAN;      // Has to be different from MPRN so we can find it in Resources
                    }
                    if (string.IsNullOrEmpty(utilityviewmodel.sparks.MPAN) ||
                        ((utilityviewmodel.sparks.MPAN == SmartParametersV2016.unknownMPAN) &&
                         (utilityviewmodel.ADDRESS.electricity.MPAN != SmartParametersV2016.unknownMPAN)))
                    {
                        utilityviewmodel.sparks.MPAN = utilityviewmodel.ADDRESS.electricity.MPAN;
                    }
                    if (string.IsNullOrEmpty(utilityviewmodel.ADDRESS.electricity.resource_type))
                    {
                        utilityviewmodel.ADDRESS.electricity.resource_type = "SR";
                    }
                    utilityviewmodel.sparks.resource_type = utilityviewmodel.ADDRESS.electricity.resource_type;
                    if (string.IsNullOrEmpty(utilityviewmodel.ADDRESS.electricity.meter_serial_no))
                    {
                        utilityviewmodel.ADDRESS.electricity.meter_serial_no = "";
                    }
                    utilityviewmodel.sparks.meter_serial_no = utilityviewmodel.ADDRESS.electricity.meter_serial_no;
                    //utilityviewmodel.sparks.udprn = udprn;
                    break;
                case SmartParametersV2016.Gas:
                    if (string.IsNullOrEmpty(utilityviewmodel.ADDRESS.gas.MPRN))
                    {
                        utilityviewmodel.ADDRESS.gas.MPRN = SmartParametersV2016.unknownMPRN;      // Has to be different from MPRN so we can find it in Resources
                    }
                    if (string.IsNullOrEmpty(utilityviewmodel.smell.MPRN) ||
                        ((utilityviewmodel.smell.MPRN == SmartParametersV2016.unknownMPRN) &&
                         (utilityviewmodel.ADDRESS.gas.MPRN != SmartParametersV2016.unknownMPRN)))
                    {
                        utilityviewmodel.smell.MPRN = utilityviewmodel.ADDRESS.gas.MPRN;
                    }
                    if (string.IsNullOrEmpty(utilityviewmodel.ADDRESS.gas.resource_type))
                    {
                        utilityviewmodel.ADDRESS.gas.resource_type = "SR";
                    }
                    utilityviewmodel.smell.resource_type = utilityviewmodel.ADDRESS.gas.resource_type;
                    if (string.IsNullOrEmpty(utilityviewmodel.ADDRESS.gas.meter_serial_no))
                    {
                        utilityviewmodel.ADDRESS.gas.meter_serial_no = "";
                    }
                    utilityviewmodel.smell.meter_serial_no = utilityviewmodel.ADDRESS.gas.meter_serial_no;
                    //utilityviewmodel.smell.udprn = udprn;
                    break;
                default:
                    break;
            }

            utilityviewmodel.account_address = account_address;
            return true;
        }

        internal static void Account_Charges_CreditsList_Add(UtilityViewModel utilityviewmodel,
                                                            char cubeface_code,
                                                            short supplier_code,
                                                            short brand_code,
                                                            string ACCOUNT_NO,
                                                            DateTime created,
                                                            string STATEMENT_ID,
                                                            string BILL_DATE,
                                                            DateTime ACCOUNT_DATE,
                                                            short account_item,
                                                            string ACCOUNT_TYPE,
                                                            short ACCOUNT_VAT_CODE,
                                                            int ACCOUNT_AMOUNT,
                                                            string INCLUDE_BILLS)
        {
            //
            // To have a Account Charges - you must
            //  Have an RESOURCE_CODE
            //  Have a SUPPLIER_code
            //  Have an ACCOUNT_NO to which the Bill can be applied
            //  Have a date (ACCOUNT_DATE) for that Bill
            //  Have an ACCOUNT_ITEM for that bill
            //
            SmartUtility.AccChargesCredits account_charges_credits_row = new SmartUtility.AccChargesCredits()
            {
                CUBEFACE_CODE = cubeface_code,                  // Key
                SUPPLIER_CODE = supplier_code,                  // Key
                BRAND_CODE = brand_code,                        // Key
                ACCOUNT_NO = ACCOUNT_NO,                        // Key
                ACCOUNT_CREATED = created,                      // Key
                STATEMENT_ID = STATEMENT_ID,                    // Key
                BILL_DATE = SmartTimeV2016.ConvertDateTime(BILL_DATE),      // Key
                ACCOUNT_DATE = ACCOUNT_DATE,                    // Key
                ACCOUNT_ITEM = account_item,                    // Key
                ACCOUNT_TYPE = ACCOUNT_TYPE,
                ACCOUNT_VAT_CODE = ACCOUNT_VAT_CODE,
                ACCOUNT_AMOUNT = ACCOUNT_AMOUNT,
                INCLUDE_BILLS = INCLUDE_BILLS
            };
            utilityviewmodel.Hezbollah.account_charges_creditsList.Add(account_charges_credits_row);
            return;
        }

        internal static void Bills_Row_Update(SmartUtility.Bills bills_row,
                                            char cubeface_code,
                                            short supplier_code,
                                            short brand_code,
                                            string ACCOUNT_NO,
                                            DateTime created,
                                            string STATEMENT_ID,
                                            string BILL_DATE,
                                            string BILL_PERIOD_START,
                                            string BILL_PERIOD_END,
                                            char PAYMENT_PLAN,
                                            bool RESOURCE_BALANCES,
                                            string PREVIOUS_BALANCE,
                                            string PAYMENTS_RECEIVED,                   // Total for this bill
                                            string ACCOUNT_CHARGES_CREDITS,             // Total for this bill
                                            string ACCOUNT_CHARGES_CREDITS_VAT_AMOUNT,  // Total for this bill
                                            string SUPPLY_CHARGES_CREDITS,              // Total for this bill
                                            string SUPPLY_CHARGES_CREDITS_VAT_AMOUNT,   // Total for this bill
                                            string BILL_VAT_AMOUNT,
                                            short BILL_VAT_CODE,
                                            string OUTSTANDING_BALANCE,
                                            string TOTAL_NOW_DUE,
                                            string MONTHLY_PAYMENT,
                                            string PAYMENT_DUE_DATE,
                                            string PAYMENT_TYPE,
                                            string DIRECT_DEBIT_DATE,
                                            string LOYALTY_BONUS,
                                            string DISCOUNT_CREDIT_DATE,
                                            string FIRST_YEAR_DISCOUNT,
                                            string REWARDS)
        {
            DateTime year_one = SmartRoutinesV2018.DateTimeParse("01/01/0001 00:00:00");
            //
            // To have a bill - you must
            //  Have a SUPPLIER_code
            //  Have an ACCOUNT_NO to which the Bill can be applied
            //  Have a date for that Bill
            //  Have a Statement Id (bill number) or identification of some kind for that Bill
            //  which has been derived externally
            if (bills_row.CUBEFACE_CODE == '\0')       // Key
            {
                bills_row.CUBEFACE_CODE = cubeface_code;                  // Key
            }
            if (bills_row.SUPPLIER_CODE == 0)                   // Key
            {
                bills_row.SUPPLIER_CODE = supplier_code;        // Key
            }
            if (bills_row.BRAND_CODE == 0)                   // Key
            {
                bills_row.BRAND_CODE = brand_code;        // Key
            }
            if (string.IsNullOrEmpty(bills_row.ACCOUNT_NO))
            {
                bills_row.ACCOUNT_NO = ACCOUNT_NO;                  // Key
            }
            if (bills_row.ACCOUNT_CREATED == year_one)                      // Key
            {
                bills_row.ACCOUNT_CREATED = created;                        // Key
            }
            if (string.IsNullOrEmpty(bills_row.STATEMENT_ID))
            {
                bills_row.STATEMENT_ID = STATEMENT_ID;              // Key
            }
            if (bills_row.BILL_DATE == year_one)                    // Key
            {
                bills_row.BILL_DATE = SmartTimeV2016.ConvertDateTime(BILL_DATE);                    // Key
            }
            if (bills_row.PAYMENT_PLAN == SmartParametersV2016.defaultChar)
            {
                bills_row.PAYMENT_PLAN = PAYMENT_PLAN;
            }
            if (bills_row.BILL_PERIOD_START == year_one)
            {
                bills_row.BILL_PERIOD_START = SmartRoutinesV2018.DateTimeParse(BILL_PERIOD_START);
            }
            if (bills_row.BILL_PERIOD_END == year_one)
            {
                bills_row.BILL_PERIOD_END = SmartRoutinesV2018.DateTimeParse(BILL_PERIOD_END);
            }
            if (bills_row.RESOURCE_BALANCES == false)
            {
                bills_row.RESOURCE_BALANCES = RESOURCE_BALANCES;
            }
            if (bills_row.PREVIOUS_BALANCE == 0)
            {
                bills_row.PREVIOUS_BALANCE = Convert.ToInt32(PREVIOUS_BALANCE);
            }
            if (bills_row.PAYMENTS_RECEIVED == 0)
            {
                bills_row.PAYMENTS_RECEIVED = Convert.ToInt32(PAYMENTS_RECEIVED);
            }
            if (bills_row.ACCOUNT_CHARGES_CREDITS == 0)
            {
                bills_row.ACCOUNT_CHARGES_CREDITS = Convert.ToInt32(ACCOUNT_CHARGES_CREDITS);
            }
            if (bills_row.ACCOUNT_CHARGES_CREDITS_VAT_AMOUNT == 0)
            {
                bills_row.ACCOUNT_CHARGES_CREDITS_VAT_AMOUNT = Convert.ToInt32(ACCOUNT_CHARGES_CREDITS_VAT_AMOUNT);
            }
            if (bills_row.SUPPLY_CHARGES_CREDITS == 0)
            {
                bills_row.SUPPLY_CHARGES_CREDITS = Convert.ToInt32(SUPPLY_CHARGES_CREDITS);
            }
            if (bills_row.SUPPLY_CHARGES_CREDITS_VAT_AMOUNT == 0)
            {
                bills_row.SUPPLY_CHARGES_CREDITS_VAT_AMOUNT = Convert.ToInt32(SUPPLY_CHARGES_CREDITS_VAT_AMOUNT);
            }
            if (bills_row.BILL_VAT_AMOUNT == 0)
            {
                bills_row.BILL_VAT_AMOUNT = Convert.ToInt32(BILL_VAT_AMOUNT);
            }
            if (bills_row.BILL_VAT_CODE == 0)
            {
                bills_row.BILL_VAT_CODE = BILL_VAT_CODE;
            }
            if (bills_row.OUTSTANDING_BALANCE == 0)
            {
                bills_row.OUTSTANDING_BALANCE = Convert.ToInt32(OUTSTANDING_BALANCE);
            }
            if (bills_row.TOTAL_NOW_DUE == 0)
            {
                bills_row.TOTAL_NOW_DUE = Convert.ToInt32(TOTAL_NOW_DUE);
            }
            if (bills_row.MONTHLY_PAYMENT == 0)
            {
                bills_row.MONTHLY_PAYMENT = Convert.ToInt32(MONTHLY_PAYMENT);
            }
            if (bills_row.PAYMENT_DUE_DATE == year_one)
            {
                bills_row.PAYMENT_DUE_DATE = SmartRoutinesV2018.DateTimeParse(PAYMENT_DUE_DATE);
            }
            if (string.IsNullOrEmpty(bills_row.PAYMENT_TYPE))
            {
                // Embedded commas fuck up SmartDBServer on the way back <= Not any more
                bills_row.PAYMENT_TYPE = PAYMENT_TYPE;
            }
            if (bills_row.DIRECT_DEBIT_DATE == year_one)
            {
                bills_row.DIRECT_DEBIT_DATE = SmartRoutinesV2018.DateTimeParse(DIRECT_DEBIT_DATE);
            }
            if (bills_row.LOYALTY_BONUS == 0.0M)
            {
                bills_row.LOYALTY_BONUS = SmartRoutinesV2018.ConvertDecimal(LOYALTY_BONUS);
            }
            if (bills_row.DISCOUNT_CREDIT_DATE == year_one)
            {
                bills_row.DISCOUNT_CREDIT_DATE = SmartRoutinesV2018.DateTimeParse(DISCOUNT_CREDIT_DATE);
            }
            if (string.IsNullOrEmpty(bills_row.FIRST_YEARS_DISCOUNT))
            {
                bills_row.FIRST_YEARS_DISCOUNT = FIRST_YEAR_DISCOUNT;
            }
            if (string.IsNullOrEmpty(bills_row.REWARDS))
            {
                bills_row.REWARDS = REWARDS;
            }
            return;
        }

        internal static void Bills_Resource_Row_Update(SmartUtility.BillsResource bills_resource_row,
                                                    char cubeface_code,
                                                    short supplier_code,
                                                    short brand_code,
                                                    string ACCOUNT_NO,
                                                    DateTime created,
                                                    string STATEMENT_ID,
                                                    string BILL_DATE,
                                                    string MPAN_MPRN,
                                                    string RESOURCE_ACCOUNT_NO,
                                                    int TARIFF_CODE,
                                                    int NEW_CHARGES,
                                                    int RESOURCE_DISCOUNTS,
                                                    int RESOURCE_VAT_AMOUNT,
                                                    short RESOURCE_VAT_CODE)
        {
            DateTime year_one = SmartRoutinesV2018.DateTimeParse("01/01/0001 00:00:00");
            //
            // To have a bill - you must
            //  Have a SUPPLIER_code
            //  Have an ACCOUNT_NO to which the Bill can be applied
            //  Have a date for that Bill
            //  Have a number or identification of some kind for that Bill
            //
            if (bills_resource_row.CUBEFACE_CODE == '\0')       // Key
            {
                bills_resource_row.CUBEFACE_CODE = cubeface_code;                  // Key
            }
            if (bills_resource_row.SUPPLIER_CODE == 0)                   // Key
            {
                bills_resource_row.SUPPLIER_CODE = supplier_code;        // Key
            }
            if (bills_resource_row.BRAND_CODE == 0)                   // Key
            {
                bills_resource_row.BRAND_CODE = brand_code;        // Key
            }
            if (string.IsNullOrEmpty(bills_resource_row.ACCOUNT_NO))
            {
                bills_resource_row.ACCOUNT_NO = ACCOUNT_NO;                 // Key
            }
            if (bills_resource_row.ACCOUNT_CREATED == year_one)                     // Key
            {
                bills_resource_row.ACCOUNT_CREATED = created;    // Key
            }
            if (string.IsNullOrEmpty(bills_resource_row.STATEMENT_ID))
            {
                bills_resource_row.STATEMENT_ID = STATEMENT_ID;              // Key
            }
            if (bills_resource_row.BILL_DATE == year_one)                    // Key
            {
                bills_resource_row.BILL_DATE = SmartTimeV2016.ConvertDateTime(BILL_DATE);                    // Key
            }
            if (string.IsNullOrEmpty(bills_resource_row.MPAN_MPRN))
            {
                bills_resource_row.MPAN_MPRN = MPAN_MPRN;
            }
            if (string.IsNullOrEmpty(bills_resource_row.RESOURCE_ACCOUNT_NO))
            {
                bills_resource_row.RESOURCE_ACCOUNT_NO = RESOURCE_ACCOUNT_NO;
            }
            if (bills_resource_row.TARIFF_CODE == 0)
            {
                bills_resource_row.TARIFF_CODE = TARIFF_CODE;
            }
            if (bills_resource_row.NEW_CHARGES == 0)
            {
                bills_resource_row.NEW_CHARGES = NEW_CHARGES;
            }
            if (bills_resource_row.RESOURCE_DISCOUNTS == 0)
            {
                bills_resource_row.RESOURCE_DISCOUNTS = RESOURCE_DISCOUNTS;
            }
            if (bills_resource_row.RESOURCE_VAT_AMOUNT == 0)
            {
                bills_resource_row.RESOURCE_VAT_AMOUNT = RESOURCE_VAT_AMOUNT;
            }
            if (bills_resource_row.RESOURCE_VAT_CODE == 0)
            {
                bills_resource_row.RESOURCE_VAT_CODE = RESOURCE_VAT_CODE;
            }
            return;
        }

        internal static void E_DiscountsList_Add(UtilityViewModel utilityviewmodel,
                                                char cubeface_code,
                                                short supplier_code,
                                                short brand_code,
                                                string ACCOUNT_NO,
                                                DateTime created,
                                                string STATEMENT_ID,
                                                string BILL_DATE,
                                                string MPAN_MPRN,
                                                string DISCOUNT_DATE,
                                                string DISCOUNT_ITEM,
                                                string DISCOUNT_TYPE,
                                                string DISCOUNT_VAT_CODE,
                                                string DISCOUNT_AMOUNT)
        {
            SmartUtility.EDiscounts discounts_row = new SmartUtility.EDiscounts()
            {
                CUBEFACE_CODE = cubeface_code,                      // Key
                SUPPLIER_CODE = supplier_code,                     // Key
                BRAND_CODE = brand_code,                            // Key
                ACCOUNT_NO = ACCOUNT_NO,                            // Key
                ACCOUNT_CREATED = created,                                  // Key
                STATEMENT_ID = STATEMENT_ID,                        // Key
                BILL_DATE = SmartTimeV2016.ConvertDateTime(BILL_DATE),                              // Another fucking Key
                MPAN_MPRN = MPAN_MPRN,                              // Key
                DISCOUNT_DATE = SmartRoutinesV2018.DateTimeParse(DISCOUNT_DATE),    // Key
                DISCOUNT_ITEM = Convert.ToInt16(DISCOUNT_ITEM),   // Key
                DISCOUNT_TYPE = DISCOUNT_TYPE,
                DISCOUNT_VAT_CODE = Convert.ToInt16(DISCOUNT_VAT_CODE),
                DISCOUNT_AMOUNT = Convert.ToInt32(DISCOUNT_AMOUNT),
                Updated = false
            };
            utilityviewmodel.Hezbollah.e_discounts_changesList.Add(discounts_row);
            return;
        }

        internal static void G_DiscountsList_Add(UtilityViewModel utilityviewmodel,
                                                char cubeface_code,
                                                short supplier_code,
                                                short brand_code,
                                                string ACCOUNT_NO,
                                                DateTime created,
                                                string STATEMENT_ID,
                                                string BILL_DATE,
                                                string MPAN_MPRN,
                                                string DISCOUNT_DATE,
                                                string DISCOUNT_ITEM,
                                                string DISCOUNT_TYPE,
                                                string DISCOUNT_VAT_CODE,
                                                string DISCOUNT_AMOUNT)
        {
            SmartUtility.GDiscounts discounts_row = new SmartUtility.GDiscounts()
            {
                CUBEFACE_CODE = cubeface_code,                      // Key
                SUPPLIER_CODE = supplier_code,                      // Key
                BRAND_CODE = brand_code,                            // Key
                ACCOUNT_NO = ACCOUNT_NO,                            // Key
                ACCOUNT_CREATED = created,                                  // Key
                STATEMENT_ID = STATEMENT_ID,                        // Key
                BILL_DATE = SmartTimeV2016.ConvertDateTime(BILL_DATE),                              // Another fucking Key
                MPAN_MPRN = MPAN_MPRN,                               // Key
                DISCOUNT_DATE = SmartRoutinesV2018.DateTimeParse(DISCOUNT_DATE),    // Key
                DISCOUNT_ITEM = Convert.ToInt16(DISCOUNT_ITEM),   // Key
                DISCOUNT_TYPE = DISCOUNT_TYPE,
                DISCOUNT_VAT_CODE = Convert.ToInt16(DISCOUNT_VAT_CODE),
                DISCOUNT_AMOUNT = Convert.ToInt32(DISCOUNT_AMOUNT),
                Updated = false
            };
            utilityviewmodel.Hezbollah.g_discounts_changesList.Add(discounts_row);
            return;
        }

        internal static void E_ReadingsList_Add(UtilityViewModel utilityviewmodel,
                                                char cubeface_code,
                                                short supplier_code,
                                                short brand_code,
                                                string ACCOUNT_NO,
                                                DateTime created,
                                                string STATEMENT_ID,
                                                string BILL_DATE,
                                                string MPAN_MPRN,
                                                string READINGS_PERIOD_START,
                                                string READINGS_PERIOD_END,
                                                string METER_SERIAL_NO,
                                                string READ_TYPE,
                                                string D_LAST_READ,
                                                string D_THIS_READ,
                                                string D_UNITS_USED,
                                                string N_LAST_READ,
                                                string N_THIS_READ,
                                                string N_UNITS_USED,
                                                string UNIT_OF_MEASURE)
        {
            SmartUtility.EReadings e_readings_row = new SmartUtility.EReadings()
            {
                CUBEFACE_CODE = cubeface_code,                      // Key
                SUPPLIER_CODE = supplier_code,                     // Key
                BRAND_CODE = brand_code,                            // Key
                ACCOUNT_NO = ACCOUNT_NO,                            // Key
                ACCOUNT_CREATED = created,                                  // Key
                STATEMENT_ID = STATEMENT_ID,                        // Key
                BILL_DATE = SmartTimeV2016.ConvertDateTime(BILL_DATE),                              // Yet another Key
                MPAN_MPRN = MPAN_MPRN,                              // Keys
                READINGS_PERIOD_START = SmartTimeV2016.ConvertDateTime(READINGS_PERIOD_START),       // Key
                READINGS_PERIOD_END = SmartTimeV2016.ConvertDateTime(READINGS_PERIOD_END),       // Key
                METER_SERIAL_NO = METER_SERIAL_NO,
                READ_TYPE = READ_TYPE,
                D_LAST_READ = SmartRoutinesV2018.ConvertDecimal(D_LAST_READ),
                D_THIS_READ = SmartRoutinesV2018.ConvertDecimal(D_THIS_READ),
                D_UNITS_USED = SmartRoutinesV2018.ConvertDecimal(D_UNITS_USED),
                N_LAST_READ = SmartRoutinesV2018.ConvertDecimal(N_LAST_READ),
                N_THIS_READ = SmartRoutinesV2018.ConvertDecimal(N_THIS_READ),
                N_UNITS_USED = SmartRoutinesV2018.ConvertDecimal(N_UNITS_USED),
                UNIT_OF_MEASURE = UNIT_OF_MEASURE,
                Updated = false
            };
            utilityviewmodel.Hezbollah.e_readings_changesList.Add(e_readings_row);
            return;
        }

        internal static void E_Unit_ChargesList_Add(UtilityViewModel utilityviewmodel,
                                                    char cubeface_code,
                                                    short supplier_code,
                                                    short brand_code,
                                                    string ACCOUNT_NO,
                                                    DateTime created,
                                                    string STATEMENT_ID,
                                                    string BILL_DATE,
                                                    string MPAN_MPRN,
                                                    string UNIT_CHARGES_PERIOD_START,
                                                    string UNIT_CHARGES_PERIOD_END,
                                                    char UNITS_TIME,
                                                    string UNITS_BAND,
                                                    string UNITS_TYPE,
                                                    string UNITS,
                                                    string UNITS_RATE,
                                                    string UNIT_OF_MEASURE,
                                                    string UNITS_COST)
        {
            SmartUtility.EUnitCharges e_unit_charges_row = new SmartUtility.EUnitCharges()
            {
                CUBEFACE_CODE = cubeface_code,                      // Key 
                SUPPLIER_CODE = supplier_code,                      // Key
                BRAND_CODE = brand_code,                            // Key
                ACCOUNT_NO = ACCOUNT_NO,                            // Key
                ACCOUNT_CREATED = created,                          // Key
                STATEMENT_ID = STATEMENT_ID,                        // Key
                BILL_DATE = SmartTimeV2016.ConvertDateTime(BILL_DATE),          // yet anothe fucking Key
                MPAN_MPRN = MPAN_MPRN,                              // Again more fucking keys
                UNIT_CHARGES_PERIOD_START = SmartTimeV2016.ConvertDateTime(UNIT_CHARGES_PERIOD_START),   // Key
                UNIT_CHARGES_PERIOD_END = SmartTimeV2016.ConvertDateTime(UNIT_CHARGES_PERIOD_END),   // Key
                UNITS_TIME = UNITS_TIME,                            // Key
                UNITS_BAND = Convert.ToInt16(UNITS_BAND),           // Key
                //if (string.IsNullOrEmpty(e_unit_charges_row.UNITS_TYPE))
                //{
                UNITS_TYPE = UNITS_TYPE,
                //}
                UNITS = SmartRoutinesV2018.ConvertDecimal(UNITS),
                UNITS_RATE = SmartRoutinesV2018.ConvertDecimal(UNITS_RATE),
                UNIT_OF_MEASURE = UNIT_OF_MEASURE,
                UNITS_COST = Convert.ToInt32(UNITS_COST),
                Updated = false
            };
            utilityviewmodel.Hezbollah.e_unit_charges_changesList.Add(e_unit_charges_row);
            return;
        }

        internal static void E_Standing_ChargesList_Add(UtilityViewModel utilityviewmodel,
                                                        char cubeface_code,
                                                        short supplier_code,
                                                        short brand_code,
                                                        string ACCOUNT_NO,
                                                        DateTime created,
                                                        string STATEMENT_ID,
                                                        string BILL_DATE,
                                                        string MPAN_MPRN,
                                                        string STANDING_CHARGES_PERIOD_START,
                                                        string STANDING_CHARGES_PERIOD_END,
                                                        string CHARGES_ITEM,
                                                        string CHARGES_TYPE,
                                                        string STANDING_CHARGE,
                                                        string CHARGES_DAYS,
                                                        string CHARGES_COST)
        {
            //
            // To have Standing Charges - you must
            //  Have a SUPPLIER_code
            //  Have an ACCOUNT_NO to which the Charge can be applied
            //  Have a STATEMENT_ID
            //  Have a PERIOD_START for multiple Charges on the same date to the same Account
            //  Have a DATE_DATE for multiple Charges on the same date to the same Account
            //  Have a CHARGES_ITEM for multiple Charges on the same date to the same Account

            SmartUtility.EStandingCharges e_standing_charges_row = new SmartUtility.EStandingCharges()
            {
                CUBEFACE_CODE = cubeface_code,                      // Key
                SUPPLIER_CODE = supplier_code,                      // Key
                BRAND_CODE = brand_code,                            // Key
                ACCOUNT_NO = ACCOUNT_NO,                            // Key
                ACCOUNT_CREATED = created,                          // Key
                STATEMENT_ID = STATEMENT_ID,                        // Key
                BILL_DATE = SmartTimeV2016.ConvertDateTime(BILL_DATE),          // Another fucking Key
                MPAN_MPRN = MPAN_MPRN,                              // Sigh again
                STANDING_CHARGES_PERIOD_START = SmartTimeV2016.ConvertDateTime(STANDING_CHARGES_PERIOD_START),       // Key
                STANDING_CHARGES_PERIOD_END = SmartTimeV2016.ConvertDateTime(STANDING_CHARGES_PERIOD_END),       // Key
                CHARGES_ITEM = Convert.ToInt16(CHARGES_ITEM),    // Key
                // Embedded commas fuck up SmartDBServer on the way back - Not any more!
                //if (string.IsNullOrEmpty(e_standing_charges_row.CHARGES_TYPE))
                //{
                CHARGES_TYPE = CHARGES_TYPE,
                //}
                STANDING_CHARGE = SmartRoutinesV2018.ConvertDecimal(STANDING_CHARGE),
                CHARGES_DAYS = Convert.ToInt16(CHARGES_DAYS),
                CHARGES_COST = Convert.ToInt16(CHARGES_COST),
                Updated = false
            };
            utilityviewmodel.Hezbollah.e_standing_charges_changesList.Add(e_standing_charges_row);
            return;
        }

        internal static void G_ReadingsList_Add(UtilityViewModel utilityviewmodel,
                                                char cubeface_code,
                                                short supplier_code,
                                                short brand_code,
                                                string ACCOUNT_NO,
                                                DateTime created,
                                                string STATEMENT_ID,
                                                string BILL_DATE,
                                                string MPAN_MPRN,
                                                string READINGS_PERIOD_START,
                                                string READINGS_PERIOD_END,
                                                string METER_SERIAL_NO,
                                                string READ_TYPE,
                                                string D_LAST_READ,
                                                string D_THIS_READ,
                                                string D_UNITS_USED_M3,
                                                string UNIT_OF_MEASURE,
                                                string D_UNITS_USED_KWH,
                                                string CALORIFIC_VALUE)
        {
            SmartUtility.GReadings g_readings_row = new SmartUtility.GReadings()
            {
                CUBEFACE_CODE = cubeface_code,                  // Key
                SUPPLIER_CODE = supplier_code,                  // Key
                BRAND_CODE = brand_code,                        // Key
                ACCOUNT_NO = ACCOUNT_NO,                        // Key
                ACCOUNT_CREATED = created,                      // Key
                STATEMENT_ID = STATEMENT_ID,                    // Key
                BILL_DATE = SmartTimeV2016.ConvertDateTime(BILL_DATE),      // Yet another Key
                MPAN_MPRN = MPAN_MPRN,                          // Key
                READINGS_PERIOD_START = SmartRoutinesV2018.DateTimeParse(READINGS_PERIOD_START),       // Key
                READINGS_PERIOD_END = SmartRoutinesV2018.DateTimeParse(READINGS_PERIOD_END),       // Key
                METER_SERIAL_NO = METER_SERIAL_NO,
                // Embedded commas fuck up SmartDBServer on the way back
                READ_TYPE = READ_TYPE,
                D_LAST_READ = SmartRoutinesV2018.ConvertDecimal(D_LAST_READ),
                D_THIS_READ = SmartRoutinesV2018.ConvertDecimal(D_THIS_READ),
                D_UNITS_USED_M3 = SmartRoutinesV2018.ConvertDecimal(D_UNITS_USED_M3),
                UNIT_OF_MEASURE = UNIT_OF_MEASURE,
                D_UNITS_USED_KWH = SmartRoutinesV2018.ConvertDecimal(D_UNITS_USED_KWH),
                CALORIFIC_VALUE = SmartRoutinesV2018.ConvertDecimal(CALORIFIC_VALUE),
                Updated = false
            };
            utilityviewmodel.Hezbollah.g_readings_changesList.Add(g_readings_row);
            return;
        }

        internal static void G_Standing_ChargesList_Add(UtilityViewModel utilityviewmodel,
                                                        char cubeface_code,
                                                        short supplier_code,
                                                        short brand_code,
                                                        string ACCOUNT_NO,
                                                        DateTime created,
                                                        string STATEMENT_ID,
                                                        string BILL_DATE,
                                                        string MPAN_MPRN,
                                                        string STANDING_CHARGES_PERIOD_START,
                                                        string STANDING_CHARGES_PERIOD_END,
                                                        string CHARGES_ITEM,
                                                        string CHARGES_TYPE,
                                                        string STANDING_CHARGE,
                                                        string CHARGES_DAYS,
                                                        string CHARGES_COST)
        {
            //
            // To have Standing Charges - you must
            //  Have a SUPPLIER_code
            //  Have an ACCOUNT_NO to which the Charge can be applied
            //  Have a STATEMENT_ID
            //  Have a PERIOD_START for multiple Charges on the same date to the same Account
            //  Have a PERIOD_END for multiple Charges on the same date to the same Account
            //  Have a CHARGES_ITEM for multiple Charges on the same date to the same Account

            SmartUtility.GStandingCharges g_standing_charges_row = new SmartUtility.GStandingCharges()
            {
                CUBEFACE_CODE = cubeface_code,                      // Key
                SUPPLIER_CODE = supplier_code,                      // Key
                BRAND_CODE = brand_code,                            // Key
                ACCOUNT_NO = ACCOUNT_NO,                            // Key
                ACCOUNT_CREATED = created,                          // Key
                STATEMENT_ID = STATEMENT_ID,                        // Key
                BILL_DATE = SmartTimeV2016.ConvertDateTime(BILL_DATE),          // Another effing Key
                MPAN_MPRN = MPAN_MPRN,                              // ditto above
                STANDING_CHARGES_PERIOD_START = SmartTimeV2016.ConvertDateTime(STANDING_CHARGES_PERIOD_START),       // Key
                STANDING_CHARGES_PERIOD_END = SmartTimeV2016.ConvertDateTime(STANDING_CHARGES_PERIOD_END),       // Key
                CHARGES_ITEM = Convert.ToInt16(CHARGES_ITEM),    // Key
                // Embedded commas fuck up SmartDBServer on the way back
                //if (string.IsNullOrEmpty(g_standing_charges_row.CHARGES_TYPE))
                //{
                CHARGES_TYPE = CHARGES_TYPE,
                //}
                STANDING_CHARGE = SmartRoutinesV2018.ConvertDecimal(STANDING_CHARGE),
                CHARGES_DAYS = Convert.ToInt16(CHARGES_DAYS),
                CHARGES_COST = Convert.ToInt16(CHARGES_COST),
                Updated = false
            };
            utilityviewmodel.Hezbollah.g_standing_charges_changesList.Add(g_standing_charges_row);
            return;
        }

        internal static void G_Unit_ChargesList_Add(UtilityViewModel utilityviewmodel,
                                                    char cubeface_code,
                                                    short supplier_code,
                                                    short brand_code,
                                                    string ACCOUNT_NO,
                                                    DateTime created,
                                                    string STATEMENT_ID,
                                                    string BILL_DATE,
                                                    string MPAN_MPRN,
                                                    string UNIT_CHARGES_PERIOD_START,
                                                    string UNIT_CHARGES_PERIOD_END,
                                                    string UNITS_BAND,
                                                    string UNITS_TYPE,
                                                    string UNITS,
                                                    string UNITS_RATE,
                                                    string UNIT_OF_MEASURE,
                                                    string UNITS_COST)
        {
            SmartUtility.GUnitCharges g_unit_charges_row = new SmartUtility.GUnitCharges()
            {
                CUBEFACE_CODE = cubeface_code,                      // Key
                SUPPLIER_CODE = supplier_code,                      // Key
                BRAND_CODE = brand_code,                            // Key
                ACCOUNT_NO = ACCOUNT_NO,                            // Key
                ACCOUNT_CREATED = created,                          // Key
                STATEMENT_ID = STATEMENT_ID,                        // Key
                BILL_DATE = SmartTimeV2016.ConvertDateTime(BILL_DATE),          // yet another fucking Key
                MPAN_MPRN = MPAN_MPRN,                              // and ANOTHER fucking key
                UNIT_CHARGES_PERIOD_START = SmartTimeV2016.ConvertDateTime(UNIT_CHARGES_PERIOD_START),  // Key
                UNIT_CHARGES_PERIOD_END = SmartTimeV2016.ConvertDateTime(UNIT_CHARGES_PERIOD_END),   // Key
                UNITS_BAND = Convert.ToInt16(UNITS_BAND),           // Key
                UNITS_TYPE = UNITS_TYPE,
                UNITS = SmartRoutinesV2018.ConvertDecimal(UNITS),
                UNITS_RATE = SmartRoutinesV2018.ConvertDecimal(UNITS_RATE),
                UNIT_OF_MEASURE = UNIT_OF_MEASURE,
                UNITS_COST = Convert.ToInt32(UNITS_COST),    // I wish you were ...
                Updated = false
            };
            utilityviewmodel.Hezbollah.g_unit_charges_changesList.Add(g_unit_charges_row);
            return;
        }

        internal static void TariffDetailsList_Add(UtilityViewModel utilityviewmodel,
                                            char cubeface_code,
                                            short supplier_code,
                                            short brand_code,
                                            string ACCOUNT_NO,
                                            DateTime created,
                                            string STATEMENT_ID,
                                            string BILL_DATE,
                                            string MPAN_MPRN,
                                            DateTime PRICES_VALID_FROM,
                                            DateTime PRICES_VALID_TO,
                                            int TARIFF_CODE,
                                            char PAYMENT_PLAN,
                                            short TIER_LEVEL,
                                            short AREA_CODE,
                                            string SC,
                                            string DR,
                                            string NR,
                                            string TCR)
        {
            SmartUtility.TariffDetails TariffDetails_row = new SmartUtility.TariffDetails()
            {
                CUBEFACE_CODE = cubeface_code,                      // Key
                SUPPLIER_CODE = supplier_code,                      // Key
                BRAND_CODE = brand_code,                            // Key
                ACCOUNT_NO = ACCOUNT_NO,                            // Key
                ACCOUNT_CREATED = created,                          // Key
                STATEMENT_ID = STATEMENT_ID,                        // Key
                BILL_DATE = SmartTimeV2016.ConvertDateTime(BILL_DATE),          // Key
                MPAN_MPRN = MPAN_MPRN,                              // Key
                PRICES_VALID_FROM = PRICES_VALID_FROM,              // Key
                PRICES_VALID_TO = PRICES_VALID_TO,                  // Key
                TARIFF_CODE = TARIFF_CODE,                          // Key
                PAYMENT_PLAN = PAYMENT_PLAN,                        // Key
                TIER_LEVEL = TIER_LEVEL,                            // Key
                AREA_CODE = AREA_CODE,
                SC = SC,
                DR = DR,
                NR = NR,
                TCR = TCR,
                Updated = false
            };
            utilityviewmodel.Hezbollah.tariff_details_changesList.Add(TariffDetails_row);
            return;
        }

        internal static void PaymentsList_Add(UtilityViewModel utilityviewmodel,
                                            char cubeface_code,
                                            short supplier_code,
                                            short brand_code,
                                            string ACCOUNT_NO,
                                            DateTime CREATED,
                                            string STATEMENT_ID,
                                            string BILL_DATE,
                                            DateTime PAYMENT_DATE,
                                            short PAYMENT_ITEM,
                                            short PAYMENT_CODE,
                                            int PAYMENT_AMOUNT,
                                            int PAYMENT_BALANCE)
        {
            //
            // To have a Payment and hold details - you must
            //  Have a SUPPLIER_code
            //  Have an ACCOUNT_NO to which the Payment can be applied
            //  Have a PAYMENT_DATE for when that Payment took place
            //  Have a PAYMENT_ITEM for multiple payments on the same date
            //  too the same Account
            //
            //  The ACCOUNT_NO will either be JOINT for both Electricity and Gas
            //  or INDIVIDUAL and therfore show for each resource
            //  (Don't necessarily need a Bill to have made a payment, you
            //  typically pay a Supplier and quote the Account Number
            //
            SmartUtility.Payments payments_row = new SmartUtility.Payments()
            {
                CUBEFACE_CODE = cubeface_code,                      // Key
                SUPPLIER_CODE = supplier_code,                      // Key
                BRAND_CODE = brand_code,                            // Key
                ACCOUNT_NO = ACCOUNT_NO,                            // Key
                ACCOUNT_CREATED = CREATED,                          // Key
                STATEMENT_ID = STATEMENT_ID,                        // This is a must have for a payment in a bill but a 'nice to have' for a payment made but not shown in a bill
                BILL_DATE = SmartTimeV2016.ConvertDateTime(BILL_DATE),          // Key
                PAYMENT_DATE = PAYMENT_DATE,                        // Key
                PAYMENT_ITEM = PAYMENT_ITEM,                        // Key
                PAYMENT_CODE = PAYMENT_CODE,
                PAYMENT_AMOUNT = PAYMENT_AMOUNT,
                PAYMENT_BALANCE = PAYMENT_BALANCE,
                Updated = false
            };
            utilityviewmodel.Hezbollah.payments_changesList.Add(payments_row);
            return;
        }

        internal static void UnallocatedList_Add(UtilityViewModel utilityviewmodel,
                                            char cubeface_code,
                                            short supplier_code,
                                            short brand_code,
                                            string ACCOUNT_NO,
                                            DateTime created,
                                            string STATEMENT_ID,
                                            string BILL_DATE,
                                            DateTime PAYMENT_DATE,
                                            short PAYMENT_ITEM,
                                            short PAYMENT_CODE,
                                            int PAYMENT_AMOUNT,
                                            int PAYMENT_BALANCE)
        {
            //
            // To have a Unallocated and hold details - you must
            //  Have a SUPPLIER_code
            //  Have an ACCOUNT_NO to which the Payment can be applied
            //  Have a PAYMENT_DATE for when that Payment took place
            //  Have a PAYMENT_ITEM for multiple payments on the same date
            //  too the same Account
            //
            //  The ACCOUNT_NO will either be JOINT for both Electricity and Gas
            //  or INDIVIDUAL and therfore show for each resource
            //  (Don't necessarily need a Bill to have made a payment, you
            //  typically pay a Supplier and quote the Account Number
            //
            SmartUtility.Unallocated unallocated_row = new SmartUtility.Unallocated()
            {
                CUBEFACE_CODE = cubeface_code,                      // Key
                SUPPLIER_CODE = supplier_code,                      // Key
                BRAND_CODE = brand_code,                            // Key
                ACCOUNT_NO = ACCOUNT_NO,                            // Key
                ACCOUNT_CREATED = created,                          // Key
                STATEMENT_ID = STATEMENT_ID,                        // Key Should be empty
                BILL_DATE = SmartTimeV2016.ConvertDateTime(BILL_DATE),          // Key Should be default
                PAYMENT_DATE = PAYMENT_DATE,                        // Key
                PAYMENT_ITEM = PAYMENT_ITEM,                        // Key
                PAYMENT_CODE = PAYMENT_CODE,
                PAYMENT_AMOUNT = PAYMENT_AMOUNT,
                PAYMENT_BALANCE = PAYMENT_BALANCE,
                Updated = false
            };
            utilityviewmodel.Hezbollah.unallocated_changesList.Add(unallocated_row);
            return;
        }

        internal static void Supply_Charges_CreditsList_Add(UtilityViewModel utilityviewmodel,
                                                            char cubeface_code,
                                                            short supplier_code,
                                                            short brand_code,
                                                            string ACCOUNT_NO,
                                                            DateTime created,
                                                            string STATEMENT_ID,
                                                            string BILL_DATE,
                                                            DateTime SUPPLY_DATE,
                                                            short supply_item,
                                                            string SUPPLY_TYPE,
                                                            short SUPPLY_VAT_CODE,
                                                            int SUPPLY_AMOUNT,
                                                            DateTime SUPPLY_DUE_DATE,
                                                            string SUPPLY_CREDIT_BILL)
        {
            SmartUtility.SupChargesCredits supply_charges_credits_row = new SmartUtility.SupChargesCredits()
            {
                CUBEFACE_CODE = cubeface_code,              // Key
                SUPPLIER_CODE = supplier_code,              // Key
                BRAND_CODE = brand_code,                    // Key
                ACCOUNT_NO = ACCOUNT_NO,                    // Key
                ACCOUNT_CREATED = created,                  // Key
                STATEMENT_ID = STATEMENT_ID,                // Key
                BILL_DATE = SmartTimeV2016.ConvertDateTime(BILL_DATE),  // Key
                SUPPLY_DATE = SUPPLY_DATE,                  // Key
                SUPPLY_ITEM = supply_item,                  // Key
                SUPPLY_TYPE = SUPPLY_TYPE,
                SUPPLY_VAT_CODE = SUPPLY_VAT_CODE,
                SUPPLY_AMOUNT = SUPPLY_AMOUNT,
                SUPPLY_DUE_DATE = SUPPLY_DUE_DATE,
                SUPPLY_CREDIT_BILL = SUPPLY_CREDIT_BILL,
                Updated = false
            };
            utilityviewmodel.Hezbollah.supply_charges_credits_changesList.Add(supply_charges_credits_row);
            return;
        }

        internal static List<Amelia> Amelia_Add_Record(SmartUtility.AmeliasView consumers_view_row)
        {
            List<Amelia> ameliaList = new List<Amelia>();
            Amelia amelia_row = new Amelia()
            {
                CUBEFACE_CODE = consumers_view_row.CUBEFACE_CODE,
                RESOURCE_CODE = consumers_view_row.RESOURCE_CODE,
                RESOURCE_TYPE = consumers_view_row.RESOURCE_TYPE,
                SUPPLIER_CODE = consumers_view_row.SUPPLIER_CODE,
                BRAND_CODE = consumers_view_row.BRAND_CODE,
                ACCOUNT_CREATED = consumers_view_row.ACCOUNT_CREATED,
                ACCOUNT_NO = consumers_view_row.ACCOUNT_NO,
                POSTCODE = consumers_view_row.POSTCODE,
                AREA_CODE = consumers_view_row.AREA_CODE,
                MPAN_MPRN = consumers_view_row.MPAN_MPRN,
                METER_SERIAL_NO = consumers_view_row.METER_SERIAL_NO,
                UDPRN = consumers_view_row.UDPRN//,
                //BANK_INSTITUTION_CODE = consumers_view_row.BANK_INSTITUTION_CODE,
                //BANK_BRAND_CODE = consumers_view_row.BANK_BRAND_CODE
            };
            ameliaList.Add(amelia_row);
            return ameliaList;
        }

        internal static void Amelia_Add(UtilityViewModel utilityviewmodel)
        {
            switch (utilityviewmodel.resource_code)
            {
                case SmartParametersV2016.Electricity:
                    utilityviewmodel.Hezbollah.ameliaList.Add(new Amelia()
                    {
                        CUBEFACE_CODE = SmartParametersV2016.Utility,
                        RESOURCE_CODE = utilityviewmodel.resource_code,
                        RESOURCE_TYPE = utilityviewmodel.sparks.resource_type,
                        SUPPLIER_CODE = utilityviewmodel.supplier_code,
                        BRAND_CODE = utilityviewmodel.brand_code,
                        ACCOUNT_CREATED = utilityviewmodel.created,
                        ACCOUNT_NO = utilityviewmodel.account_no,
                        POSTCODE = utilityviewmodel.postcode,
                        AREA_CODE = utilityviewmodel.area_code,
                        MPAN_MPRN = utilityviewmodel.sparks.MPAN,
                        METER_SERIAL_NO = utilityviewmodel.sparks.meter_serial_no,
                        //UDPRN = utilityviewmodel.sparks.udprn,
                        BANK_INSTITUTION_CODE = utilityviewmodel.supplier_code,
                        BANK_BRAND_CODE = utilityviewmodel.brand_code
                    });
                    break;
                case SmartParametersV2016.Gas:
                    utilityviewmodel.Hezbollah.ameliaList.Add(new Amelia()
                    {
                        CUBEFACE_CODE = SmartParametersV2016.Utility,
                        RESOURCE_CODE = utilityviewmodel.resource_code,
                        RESOURCE_TYPE = utilityviewmodel.smell.resource_type,
                        SUPPLIER_CODE = utilityviewmodel.supplier_code,
                        BRAND_CODE = utilityviewmodel.brand_code,
                        ACCOUNT_CREATED = utilityviewmodel.created,
                        ACCOUNT_NO = utilityviewmodel.account_no,
                        POSTCODE = utilityviewmodel.postcode,
                        AREA_CODE = utilityviewmodel.area_code,
                        MPAN_MPRN = utilityviewmodel.smell.MPRN,
                        METER_SERIAL_NO = utilityviewmodel.smell.meter_serial_no,
                        //UDPRN = utilityviewmodel.smell.udprn,
                        BANK_INSTITUTION_CODE = utilityviewmodel.supplier_code,
                        BANK_BRAND_CODE = utilityviewmodel.brand_code
                    });
                    break;
                case SmartParametersV2016.Banks:
                    utilityviewmodel.Hezbollah.ameliaList.Add(new Amelia()
                    {
                        CUBEFACE_CODE = SmartParametersV2016.Utility,
                        RESOURCE_CODE = utilityviewmodel.resource_code,
                        RESOURCE_TYPE = utilityviewmodel.smell.resource_type,
                        SUPPLIER_CODE = utilityviewmodel.supplier_code,
                        BRAND_CODE = utilityviewmodel.brand_code,
                        ACCOUNT_CREATED = utilityviewmodel.created,
                        ACCOUNT_NO = utilityviewmodel.account_no,
                        POSTCODE = utilityviewmodel.postcode,
                        AREA_CODE = utilityviewmodel.area_code,
                        MPAN_MPRN = utilityviewmodel.smell.MPRN,
                        METER_SERIAL_NO = utilityviewmodel.smell.meter_serial_no,
                        //UDPRN = utilityviewmodel.smell.udprn,
                        BANK_INSTITUTION_CODE = utilityviewmodel.bank_institution_code,
                        BANK_BRAND_CODE = utilityviewmodel.bank_brand_code
                    });
                    break;
                default:
                    break;
            }
            return;
        }

        internal static List<Amelia> Amelia_Lookup(MainViewModel ourviewmodel,
                                                                    UtilityViewModel utilityviewmodel, bool include_udprn)
        {
            List<Amelia> amelia_found = new List<Amelia>();
            if (include_udprn)
            {
                switch (utilityviewmodel.resource_code)
                {
                    case SmartParametersV2016.Electricity:
                        amelia_found = SmartSpikeUtilityV2017.Utility_Find_Amelia(ourviewmodel,
                                                    utilityviewmodel,
                                                    utilityviewmodel.resource_code,
                                                    utilityviewmodel.sparks.resource_type,
                                                    utilityviewmodel.account_no,
                                                    utilityviewmodel.sparks.MPAN);
                        break;
                    case SmartParametersV2016.Gas:
                        amelia_found = SmartSpikeUtilityV2017.Utility_Find_Amelia(ourviewmodel,
                                                    utilityviewmodel,
                                                    utilityviewmodel.resource_code,
                                                    utilityviewmodel.smell.resource_type,
                                                    utilityviewmodel.account_no,
                                                    utilityviewmodel.smell.MPRN);
                        break;
                    default:
                        break;
                }
            }
            else
            {
                // Doesn't matter about the UDPRN - only ever do one of each 'E' or 'G'
                // AND its the Account No which changes in a household - not the UDPRN (which IS the houshold)
                amelia_found = SmartSpikeUtilityV2017.Utility_Find_Amelia(ourviewmodel,
                                                    utilityviewmodel,
                                                    utilityviewmodel.resource_code,
                                                    Determine_Resource_Type(utilityviewmodel),
                                                    utilityviewmodel.account_no,
                                                    "");
            }
            return amelia_found;
        }

        internal static string Determine_Resource_Type(UtilityViewModel utilityviewmodel)
        {
            string resource_type = "";
            switch (utilityviewmodel.resource_code)
            {
                case SmartParametersV2016.Electricity:
                    resource_type = utilityviewmodel.sparks.resource_type;
                    break;
                case SmartParametersV2016.Gas:
                    resource_type = utilityviewmodel.smell.resource_type;
                    break;
                default:
                    break;
            }
            return resource_type;
        }

        internal static void FixResourceTypes(UtilityViewModel utilityviewmodel,
                                SmartUtility.Resources resource_row)
        {
            foreach (SmartUtility.ResourcesTypes restype in utilityviewmodel.Hezbollah.utility_resources_typesList) // Checked
            {
                if (resource_row.USERNAME == restype.USERNAME &&
                    resource_row.CUBEFACE_CODE == restype.CUBEFACE_CODE &&
                    resource_row.RESOURCE_CODE == restype.RESOURCE_CODE)
                {
                    // Do nothing! Maybe 'Active' at some point?
                    // restype.Include = resource_row.Include;
                }
            }
            return;
        }

        internal static void Fix_Resource_Type(UtilityViewModel utilityviewmodel, string resource_type)
        {
            switch (utilityviewmodel.resource_code)
            {
                case SmartParametersV2016.Electricity:
                    utilityviewmodel.sparks.resource_type = resource_type;
                    break;
                case SmartParametersV2016.Gas:
                    utilityviewmodel.smell.resource_type = resource_type;
                    break;
                default:
                    break;
            }
        }

        //internal static string Determine_UDPRN(UtilityViewModel utilityviewmodel)
        //{
        //    string udprn = "";
        //    switch (utilityviewmodel.resource_code)
        //    {
        //        case SmartParametersV2016.Electricity:
        //            udprn = utilityviewmodel.sparks.udprn;
        //            break;
        //        case SmartParametersV2016.Gas:
        //            udprn = utilityviewmodel.smell.udprn;
        //            break;
        //        default:
        //            break;
        //    }
        //    return udprn;
        //}

        internal static string Determine_MPAN_MPRN(UtilityViewModel utilityviewmodel)
        {
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
            return mpan_mprn;
        }
        // Live like me, do like me, always ask me I KNOW BEST
        // Bryan Cranston = BRANSON!!
        // Different fractions  <= Different FACTIONS you knucklehead
        // Live like me, do like me, always ask me I KNOW BEST
        internal static void Amelia_Update_Postcode_Area(MainViewModel ourviewmodel, UtilityViewModel utilityviewmodel)
        {
            List<Amelia> amelia_found = SmartSpikeUtilityV2017.Utility_Find_Amelia(ourviewmodel,
                                                    utilityviewmodel,
                                                    utilityviewmodel.resource_code,
                                                    Determine_Resource_Type(utilityviewmodel),
                                                    utilityviewmodel.account_no,
                                                    "");
            if (amelia_found.Count > 0)
            {
                foreach (Amelia amelia in amelia_found) // Checked
                {
                    switch (utilityviewmodel.resource_code)
                    {
                        case SmartParametersV2016.Electricity:
                            amelia.POSTCODE = utilityviewmodel.postcode;
                            amelia.AREA_CODE = utilityviewmodel.area_code;
                            amelia.MPAN_MPRN = utilityviewmodel.sparks.MPAN;
                            break;
                        case SmartParametersV2016.Gas:
                            amelia.POSTCODE = utilityviewmodel.postcode;
                            amelia.AREA_CODE = utilityviewmodel.area_code;
                            amelia.MPAN_MPRN = utilityviewmodel.smell.MPRN;
                            break;
                        default:
                            break;
                    }
                }
            }
            return;
        }

        //    // ALL OF THIS IS REDUNDANT
        //    // The MPAN/MPRN may or may not be "Unknown".  Whether it is or not
        //    // if we can match on the RESOURCE_CODE, SUPPLIER_CODE and ACCOUNT_NO
        //    // then ... leave it alone, as the MPAN/MPRN will be update before
        //    // we logout (i.e. if MPAN = S123445 it will be left alone, if it is
        //    // "Unknown" then it will be replaced by S123445).
        //    // So the MPAN/MPRN is irrelevent on both look-ups .  Agreed?
        //    // ALL OF THIS IS REDUNDANT

#if WINFORMS
        internal static string Check_Economy7(bool labelEconomy7)
        {
            if (labelEconomy7)
#endif
#if WPF  || WINUI || SMARTMAUI
        internal static string Check_Economy7(Visibility labelEconomy7)
        {
            if (labelEconomy7 == Visibility.Visible)
#endif
#if ANDROIDX
        internal static string Check_Economy7(bool labelEconomy7)
        {
            if (labelEconomy7)
#endif
            {
                return "VR";
            }
            else
            {
                return "SR";
            }
        }

        internal static string Filter_Tariffname(bool html_flag,
                                                string tariff_name)
        {
            // Check if the string is 'replaceable"
            if (!string.IsNullOrEmpty(tariff_name))
            {
                if (html_flag)
                {
                    // Leave any '&' in there for the time being but get rid of html shit
                    tariff_name = tariff_name.Replace("amp;", "");
                    tariff_name = tariff_name.Replace("\r\n", "");
                    tariff_name = tariff_name.Replace("\t", "");
                }
                else
                {
                    // Replace any '&' in there with $
                    tariff_name = tariff_name.Replace("&", "$");
                }
            }
            return tariff_name.Trim();
        }

        internal static bool Bill_Already_Done(UtilityViewModel utilityviewmodel, string mpan_mprn)
        {
            // Assume it has been bills_out and bills_resource_out should both come in as 'true'
            // The account has been taken out because the 'current account' in consumer_energy
            // may not match the account inside the bill!  In the case of changes in account
            // (e.g. Npower) we may be looking a bill up on the wrong account.
            // We can only truly find the Bill's account (not the current account)
            // by looking INSIDE the Bill i.e. by decoding the Pdf.
            // We are assuming here that for two different suppliers with two different
            // account numbers, we wouldn't have a Bill with the same date.... (famous last words!)
            // However FOR THE SAME SUPPLIER with two different account numbers
            // we can EASILY have a (i.e. two) Bill(s) with the same date!!!

            // Have we already done this Bill/Resource in a previous Read Meter?

            //char temp_category_code = SmartParametersV2016.Utility;

            //char temp_resource_code = utilityviewmodel.resource_code;
            //string resource_type = "";
            //switch (utilityviewmodel.resource_code)
            //{
            //    case SmartParametersV2016.Electricity:
            //        resource_type = utilityviewmodel.sparks.resource_type;
            //        break;
            //    case SmartParametersV2016.Gas:
            //        resource_type = utilityviewmodel.smell.resource_type;
            //        break;
            //    default:
            //        break;
            //}

            short temp_supplier_code = utilityviewmodel.supplier_code;
            short temp_brand_code = utilityviewmodel.brand_code;
            string temp_account_no = utilityviewmodel.account_no;
            DateTime temp_created = utilityviewmodel.created;
            string temp_statement_id = utilityviewmodel.statement_id;
            DateTime temp_bill_date = utilityviewmodel.bill_date;

            List<SmartUtility.Bills> bills_in_found =
                        SmartSpikeUtilityV2017.Utility_BillsList(utilityviewmodel,
                                                                    temp_supplier_code,
                                                                    temp_brand_code,
                                                                    temp_account_no,
                                                                    temp_created,
                                                                    temp_statement_id,
                                                                    temp_bill_date);
            switch (bills_in_found.Count)
            {
                case 0:
                    // Not in Bills In - could it be a new Bill already in Bills out?
                    List<SmartUtility.Bills> bills_out_found =
                                    SmartSpikeUtilityV2017.Utility_BillsList(utilityviewmodel,
                                                                                temp_supplier_code,
                                                                                temp_brand_code,
                                                                                temp_account_no,
                                                                                temp_created,
                                                                                temp_statement_id,
                                                                                temp_bill_date);
                    switch (bills_out_found.Count)
                    {
                        case 0:
                            // Not in Bills Out either - make a new bill and resource
                            // Leave bills_out set to true
                            utilityviewmodel.Hezbollah.bills_row = new SmartUtility.Bills();
                            // Leave bills_resource_out set to true
                            utilityviewmodel.Hezbollah.bills_resource_row = new SmartUtility.BillsResource();
                            return false;
                        default:
                            // Its in Bills Out - but ... which resource?
                            utilityviewmodel.bills_out = false;     // So clear this down
                            foreach (SmartUtility.Bills bills_out_row in bills_out_found) // Checked
                            {
                                utilityviewmodel.Hezbollah.bills_row = bills_out_row;
                                temp_account_no = bills_out_row.ACCOUNT_NO;
                                temp_created = bills_out_row.ACCOUNT_CREATED;
                                temp_bill_date = bills_out_row.BILL_DATE;
                                temp_statement_id = bills_out_row.STATEMENT_ID;

                                List<SmartUtility.BillsResource> bills_resource_out_found =
                                            SmartSpikeUtilityV2017.Utility_Bills_ResourceListNew(utilityviewmodel,
                                                                                                                    mpan_mprn,
                                                                                                                    bills_out_row);
                                switch (bills_resource_out_found.Count)
                                {
                                    case 0:
                                        // Leave bills_resource_out set to true
                                        utilityviewmodel.Hezbollah.bills_resource_row = new SmartUtility.BillsResource();
                                        return false;
                                    case 1:
                                        utilityviewmodel.Hezbollah.bills_resource_row = bills_resource_out_found[0];
                                        utilityviewmodel.bills_resource_out = false;    // So Clear this down
                                        return false;
                                    default:
                                        // Aaargh - problem on Bills Out - too many!  Don't process anything
                                        utilityviewmodel.bills_resource_out = false;
                                        break;
                                }
                            }
                            break;
                            //default:
                            //    // Aaargh - problem on Bills Out - too many!  Don't process anything
                            //    bills_out = false;
                            //    bills_resource_out = false;
                            //    break;
                    }
                    break;
                default:
                    foreach (SmartUtility.Bills bills_in_row in bills_in_found) // Checked
                    {
                        temp_account_no = bills_in_row.ACCOUNT_NO;
                        temp_created = bills_in_row.ACCOUNT_CREATED;
                        temp_bill_date = bills_in_row.BILL_DATE;
                        temp_statement_id = bills_in_row.STATEMENT_ID;
                        List<SmartUtility.BillsResource> bills_resource_in_found =
                                SmartSpikeUtilityV2017.Utility_Bills_ResourceListNew(utilityviewmodel,
                                                                                                            mpan_mprn,
                                                                                                            bills_in_row);


                        switch (bills_resource_in_found.Count)
                        {
                            case 0:
                                // Should never be here, should we? We have half a Bills In
                                // i.e. a Bills In with no Bills Resource???
                                // NO WE CAN BE HERE **IF** WE HAVE an E 05-Sep-2014 and then 
                                // a G 05-Sep-2014 ***WITH DIFFERENT ACCOUNT NUMBERS!!!***
                                // Leave bills_out set to true <= NO
                                // Leave bills_resource_out set to true
                                if (bills_in_row.ACCOUNT_NO != utilityviewmodel.account_no ||
                                    bills_in_row.ACCOUNT_CREATED != utilityviewmodel.created)
                                {
                                    utilityviewmodel.Hezbollah.bills_row = new SmartUtility.Bills();
                                }
                                else
                                {
                                    utilityviewmodel.bills_out = false;
                                }
                                utilityviewmodel.Hezbollah.bills_resource_row = new SmartUtility.BillsResource();
                                return false;
                            case 1:
                                // Already in Bills Resource in - don't process anything
                                utilityviewmodel.bills_out = false;
                                utilityviewmodel.bills_resource_out = false;    // Clear this down
                                break;
                            default:
                                // Aaargh - problem on Bills Resource In - too many!  Don't process anything
                                utilityviewmodel.bills_out = false;
                                utilityviewmodel.bills_resource_out = false;    // Clear this down
                                break;
                        }
                    }
                    break;
                    //default:
                    //    // Aaargh - problem on Bills In - too many!  Don't process anything
                    //    utilityviewmodel.bills_out = false;
                    //    utilityviewmodel.bills_resource_out = false;
                    //    break;
            }
            return true;
        }

        internal static async Task<bool> Lookup_Remote_Tariff_Code(MainViewModel ourviewmodel,
                                                                UtilityViewModel utilityviewmodel)
        {
            bool status = true;     // We live in hope ...

            if (utilityviewmodel.TARIFF_CODE == 0)
            {
                if (!SmartSpikeUtilityV2017.Utility_Lookup_TariffCode(utilityviewmodel,
                                                                utilityviewmodel.brand_code,
                                                                utilityviewmodel.resource_code,
                                                                Determine_Resource_Type(utilityviewmodel)))
                //utilityviewmodel.TARIFF_NAME,
                //utilityviewmodel.TARIFF_CODE))
                {
                    if (utilityviewmodel.Hezbollah.tariff_codes_namesList.Count > 0)
                    {
                        foreach (SmartUtility.TariffCodesNamesView tariff_codes_names_view_row in utilityviewmodel.Hezbollah.tariff_codes_names_tempList) // Checked
                        {
                            if (utilityviewmodel.resource_code == tariff_codes_names_view_row.RESOURCE_CODE &&
                                utilityviewmodel.TARIFF_NAME == tariff_codes_names_view_row.MES_NAME)
                            {
                                utilityviewmodel.TARIFF_CODE = tariff_codes_names_view_row.TARIFF_CODE;
                                //utilityviewmodel.VERSION_CODE = tariff_codes_names_view_row.VERSION_CODE;

                                utilityviewmodel.TARIFF_NAME = tariff_codes_names_view_row.TARIFF_NAME;
                                break;
                            }
                        }
                    }
                }
                if ((utilityviewmodel.area_code > 0) &&  // Stops the remote procedure crashing on AREA_? lookup
                    (utilityviewmodel.TARIFF_CODE == 0) &&
                    !string.IsNullOrEmpty(utilityviewmodel.TARIFF_NAME))
                {
                    // Its not in the REDUCED (current) list - is it in the FULL list
                    string P1 = SmartParametersV2016.Utility.ToString() + SmartParametersV2016.unitSeparator +
                                utilityviewmodel.area_code.ToString() + SmartParametersV2016.unitSeparator +
                                utilityviewmodel.brand_code.ToString() + SmartParametersV2016.unitSeparator +
                                utilityviewmodel.resource_code.ToString() + SmartParametersV2016.unitSeparator +
                                Determine_Resource_Type(utilityviewmodel) + SmartParametersV2016.unitSeparator +
                                utilityviewmodel.TARIFF_NAME + SmartParametersV2016.unitSeparator +
                                utilityviewmodel.TARIFF_NAME.Replace(SmartParametersV2016.space, "");

                    // The GUI should **NEVER** have to worry about the Schema - SmartDBServer should sort that out
                    List<SmartUtility.TariffCodesNamesView> tariff_codes_names_viewList =
                        await SmartBobV2017.Load_SingleList_Async<SmartUtility.TariffCodesNamesView>(ourviewmodel,
                                                                                            utilityviewmodel,
                                                                                            utilityviewmodel.utilityToken,
                                                                                            "SMARTUTILITY",
                                                                                            "LOOKUP_TARIFF_NAMES",
                                                                                            "P",
                                                                                            P1);
                    if (!string.IsNullOrEmpty(ourviewmodel.errorMessage))
                    {
                        return false;
                    }
                    else
                    {
                        if (tariff_codes_names_viewList.Count > 0)
                        {
                            foreach (SmartUtility.TariffCodesNamesView tariff_codes_names_view_row in tariff_codes_names_viewList) // Checked
                            {
                                SmartUtility.TariffCodesNamesView tariff_view = new SmartUtility.TariffCodesNamesView()
                                {
                                    BRAND_CODE = tariff_codes_names_view_row.BRAND_CODE,
                                    RESOURCE_CODE = tariff_codes_names_view_row.RESOURCE_CODE,
                                    TARIFF_CODE = tariff_codes_names_view_row.TARIFF_CODE,
                                    VERSION_CODE = tariff_codes_names_view_row.VERSION_CODE,
                                    TARIFF_NAME = tariff_codes_names_view_row.TARIFF_NAME,
                                    MES_NAME = utilityviewmodel.TARIFF_NAME
                                };
                                utilityviewmodel.Hezbollah.tariff_codes_names_tempList.Add(tariff_view);

                                utilityviewmodel.TARIFF_CODE = tariff_codes_names_view_row.TARIFF_CODE;
                                //utilityviewmodel.VERSION_CODE = tariff_codes_names_view_row.VERSION_CODE;
                                utilityviewmodel.TARIFF_NAME = tariff_codes_names_view_row.TARIFF_NAME;   // The real name from SmartUtility
                                break;
                            }
                        }
                        else
                        {
                            // One of the very few occasions when we need to tell HQ something -
                            // (make sure NONE of these have embedded commas, cos this will fuck up SmartDbserver!
                            ourviewmodel.errorMessage = utilityviewmodel.area_code + SmartParametersV2016.space + utilityviewmodel.supplier_code + SmartParametersV2016.space + utilityviewmodel.resource_code + SmartParametersV2016.space + utilityviewmodel.TARIFF_NAME + " not found";
                            if (!await SmartRoutinesV2018.CheckTrace(ourviewmodel, utilityviewmodel.utilityToken, utilityviewmodel.supplier_code, utilityviewmodel.brand_code, SmartParametersV2016.Utility.ToString() + " " + ourviewmodel.errorMessage))
                            {
                                return false;
                            }
                            status = false;
                        }
                    }
                }
            }
            // Fixup MES before we return with wahtever status
            return status;
        }

        
#if WINFORMS
        internal static void Generic_Payment_Plan(MainViewModel ourviewmodel,
                                                UtilityViewModel utilityviewmodel,
                                                string username, char PAYMENT_PLAN)
        {
            foreach (SmartUtility.Bills bills_row in SmartSpikeUtilityV2017.Utility_Find_Bill(ourviewmodel,
                                                                        utilityviewmodel,
                                                                        utilityviewmodel.supplier_code,
                                                                        utilityviewmodel.brand_code,
                                                                        utilityviewmodel.account_no,
                                                                        utilityviewmodel.created))
            {
                bills_row.PAYMENT_PLAN = PAYMENT_PLAN;
            }
            return;
        }
#endif
        internal static void Generic_Payment_Done(UtilityViewModel utilityviewmodel,
                                                    List<SmartUtility.Payments> payments_tempList)
        {
            // Yes, this will work fine IF you have put the fucking payments you found
            // in the Bills you have scraped INTO the fucking database ....
            DateTime last_bill_date = SmartParametersV2016.defaultDate;

            // Combine the Bills_In and Bills_Out lists
            List<SmartUtility.Bills> bills_tempList = new List<SmartUtility.Bills>();
            bills_tempList.AddRange(utilityviewmodel.Hezbollah.billsList);
            bills_tempList.AddRange(utilityviewmodel.Hezbollah.bills_changesList);
            bills_tempList.Sort();
            if (bills_tempList.Count > 0)
            {
                last_bill_date = bills_tempList[bills_tempList.Count - 1].BILL_PERIOD_END;
            }
            foreach (SmartUtility.Payments payments_row in payments_tempList) // Checked
            {
                if (payments_row.PAYMENT_DATE > last_bill_date)
                {
                    if (string.IsNullOrEmpty(payments_row.STATEMENT_ID))
                    {

                        UnallocatedList_Add(utilityviewmodel,
                                                           payments_row.CUBEFACE_CODE,
                                                           payments_row.SUPPLIER_CODE,              // Could be BRAND
                                                           payments_row.BRAND_CODE,
                                                           payments_row.ACCOUNT_NO,                 // Should not be empty
                                                           payments_row.ACCOUNT_CREATED,            // Should not be empty
                                                           payments_row.STATEMENT_ID,               // Which SHOULD BE empty
                                                           payments_row.BILL_DATE.ToString(utilityviewmodel.CultureINF),
                                                           payments_row.PAYMENT_DATE,
                                                           payments_row.PAYMENT_ITEM,
                                                           payments_row.PAYMENT_CODE,
                                                           payments_row.PAYMENT_AMOUNT,
                                                           payments_row.PAYMENT_BALANCE);
                    }
                    else
                    {
                        PaymentsList_Add(utilityviewmodel,
                                                            payments_row.CUBEFACE_CODE,
                                                            payments_row.SUPPLIER_CODE,             // Could be BRAND
                                                            payments_row.BRAND_CODE,
                                                            payments_row.ACCOUNT_NO,
                                                            payments_row.ACCOUNT_CREATED,
                                                            payments_row.STATEMENT_ID,              // Which MUST NOT be empty
                                                            payments_row.BILL_DATE.ToString(utilityviewmodel.CultureINF),
                                                            payments_row.PAYMENT_DATE,
                                                            payments_row.PAYMENT_ITEM,
                                                            payments_row.PAYMENT_CODE,
                                                            payments_row.PAYMENT_AMOUNT,
                                                            payments_row.PAYMENT_BALANCE);
                    }
                }
            }
            return;
        }

        internal static bool Check_Bill_Date(UtilityViewModel utilityviewmodel)
        {
            if ((utilityviewmodel.resource_code != SmartParametersV2016.defaultResourceCode) &&
                (utilityviewmodel.brand_code != 0) &&
                !string.IsNullOrEmpty(utilityviewmodel.account_no) &&
                !string.IsNullOrEmpty(utilityviewmodel.statement_id) &&
                (utilityviewmodel.bill_date != SmartParametersV2016.defaultDate))
            {
                return true;
            }
            return false;
        }

        internal static List<SmartUtility.Bills> Working_BillsX(MainViewModel ourviewmodel,
                                                UtilityViewModel utilityviewmodel,
                                                DateTime defaultDate,
                                                bool remove = true) // <= Remove unnecessary bills
        {
            // Well ... its not JUST Bills we want, because sometimes Bills 'overlap' each other
            // and give us multiple info for the same date period.
            // What we REALLY what to do is to realize that only READINGS have any sense because
            // they are done in ascending date order and if there are TWO readings for the same
            // date then we must have multiple Bills.  Only if a reading takes place is money
            // required so we need to start from READINGS and not BILLS ... and this manifests
            // itself into BILL_PERIOD_START and BILL_PEROD_END so we reduce the bills accordingly
            // If we have a Bill which 'encompasses' another Bill with regard to its
            // Start and End dates, then we use the encompassing (probably later) Bill.
            // The chimps at ScottishPower are responsible for this type of shit  ...

            // Now we have to pre-check out Bills in case we have overlapping BILL_PERIOD_START
            // and BILL_PERIOD_END dates ... e.g. for SP

            List<SmartUtility.Bills> working_billsList = new List<SmartUtility.Bills>();

            // But of course, the loadedList is based on the RESOURCE you dick-head
            List<SmartUtility.Bills> loadedList_found = SmartSpikeUtilityV2017.Utility_Loaded_BillList(ourviewmodel,
                                                                            utilityviewmodel,
                                                                            "");   // MPAN_MPRN


            if (loadedList_found.Count > 0)
            {
                DateTime running_period_end = defaultDate;
                short last_supplier_code = 0;
                short last_brand_code = 0;
                string last_account_no = "";
                DateTime last_created = SmartParametersV2016.defaultDate;
                foreach (SmartUtility.Bills bill in loadedList_found) // Checked
                {
                    // We are going up the list in 'Bill Date' order
                    if (working_billsList.Count == 0 ||
                        (last_supplier_code != bill.SUPPLIER_CODE) ||
                        (last_brand_code != bill.BRAND_CODE) ||
                        (last_account_no != bill.ACCOUNT_NO) ||
                        (last_created != bill.ACCOUNT_CREATED))
                    {
                        // Add in the first bill for this supplier
                        working_billsList.Add(bill);
                        running_period_end = bill.BILL_PERIOD_END;
                        last_supplier_code = bill.SUPPLIER_CODE;
                        last_brand_code = bill.BRAND_CODE;
                        last_account_no = bill.ACCOUNT_NO;
                        last_created = bill.ACCOUNT_CREATED;
                    }
                    else
                    {
                        // From now on ... they are the SAME Supplier and SAME Account No ...
                        DateTime BILL_PERIOD_START = bill.BILL_PERIOD_START;
                        DateTime BILL_PERIOD_END = bill.BILL_PERIOD_END;

                        // Is the Bill's Period Start Greater than than the List's Period End
                        if (BILL_PERIOD_START > running_period_end)
                        {
                            // Yes, ok to add it in
                            working_billsList.Add(bill);     // Add the Bill in
                            // Set the List Period End to be this Bill's Period End
                            running_period_end = BILL_PERIOD_END;
                        }
                        else
                        {
                            // Oh there is an overlap
                            // Start at the beginning of the list
                            bool sorted = false;
                            foreach (SmartUtility.Bills working in working_billsList.ToList())  // <= needed very important
                            {
                                // Does this Bill replace a single entry entirely?
                                if ((BILL_PERIOD_START == working.BILL_PERIOD_START) &&
                                    (BILL_PERIOD_END == working.BILL_PERIOD_END))
                                {
                                    // Update the Bill No and the issued date
                                    // b_ps and b_pe area a LATER_BILL
                                    working.STATEMENT_ID = bill.STATEMENT_ID;
                                    working.BILL_DATE = bill.BILL_DATE;
                                    sorted = true;
                                }
                                else
                                {
                                    // Is it a partial replacement for a single Bill
                                    // (i.e. the original Bill needs adjusting?)
                                    if ((BILL_PERIOD_START > working.BILL_PERIOD_START) &&
                                        (BILL_PERIOD_END == working.BILL_PERIOD_END))
                                    {
                                        // b_ps and b_pe are a LATER BILL
                                        working.BILL_PERIOD_END = BILL_PERIOD_START.AddDays(-1);
                                        working_billsList.Add(bill);     // Add the Bill in
                                        sorted = true;
                                    }
                                    else
                                    {
                                        if ((BILL_PERIOD_START == working.BILL_PERIOD_START) &&
                                            (BILL_PERIOD_END < working.BILL_PERIOD_END))
                                        {
                                            // b_ps and b_pe are a LATER BILL
                                            working.BILL_PERIOD_START = BILL_PERIOD_END.AddDays(1);
                                            working_billsList.Add(bill);     // Add the Bill in
                                            sorted = true;
                                        }
                                    }
                                }
                                if (sorted)
                                {
                                    break;
                                }
                                // Oh.  May be the new bill covers several previous bills
                                // Does this Bill replace a single entry entirely?
                                if (((BILL_PERIOD_START == working.BILL_PERIOD_START) &&
                                    (BILL_PERIOD_END > working.BILL_PERIOD_END)) ||
                                    ((BILL_PERIOD_START < working.BILL_PERIOD_START) &&
                                    (BILL_PERIOD_END > working.BILL_PERIOD_END)) ||
                                    ((BILL_PERIOD_START < working.BILL_PERIOD_START) &&
                                    (BILL_PERIOD_END == working.BILL_PERIOD_END)))
                                {
                                    working_billsList.Remove(working);
                                }
                                if ((BILL_PERIOD_START > working.BILL_PERIOD_START) &&
                                    (BILL_PERIOD_END < working.BILL_PERIOD_END))
                                {
                                    // Sometimes ... you get 'Bills' which are just summaries
                                    // of several real bills which have already occurred
                                    // We just need to filter these out ...
                                    sorted = true;
                                    if (remove)
                                    {
                                        working_billsList.Remove(working);
                                    }
                                    //working.BILL_DATE = SmartParametersV2016.defaultDate;   // To reflect loss of this Bill
                                }
                            }
                            if (!sorted)
                            {
                                working_billsList.Add(bill);     // Add the Bill in
                            }
                        }
                    }
                }
            }
            return working_billsList;
        }

#if WINFORMS
        internal static bool Do_We_Read(SmartProfile.Cubefaces cubeface_row,
                                        DateTime defaultDate,
                                        DateTime time_now)
        {
            //foreach (SmartUtility.Switches switches_row in switchesList)
            //{
            // Is Connection time on or before 'now'?
            if ((cubeface_row.NEXT_CONNECTION != defaultDate) &&
                (SmartRoutinesV2018.DateTimeCompare(cubeface_row.NEXT_CONNECTION, time_now) < 0))
            {
                // Next connection is behind 'now' so THIS ONE is going to cause a scrape
                // So don't bother to load SmartUtility, it will be done on the scrape
                // from within Check_Meter
                // We don't need to scrape at start-up
                return true;
            }
            //}
            // We need to scrape at startup
            return false;
        }
#endif

        // At last - after SEVEN years ... find out whether we display the DUAL FUEL button
        // or not
        // For a Consumer to be eligible to have the Dual Fuel button enabled, the must
        // have an Account (which isn't closed) for both Elextricity and Gas.  Doesn't
        // matter if they haven't had // a Bill (!).  The fact that they have two open Utility.Accounts
        // for different Resources is good enough to qualify
        internal static bool Dual_Fuel(MainViewModel ourviewmodel,
                                        UtilityViewModel utilityviewmodel)
        //List<SmartUtility.ConsumersView> consumers_viewList,
        //   List<SmartUtility.Resources> resourcesList)
        {
            // This routines assumes resourcesList JUST contains our Username
            //List<char> resource_codeList = new List<char>() { };
            //foreach (Buttons resources_row in buttonsList)
            // {
            //     if (resources_row.CUBE_CODE == 'A')
            //     {
            //         resource_codeList.Add(resources_row.RESOURCE_CODE); // Gives us 2 entries
            //     }
            // }

            // Count of 0 or 1 means no dual fuel
            return SmartSpikeUtilityV2017.Utility_Find_OpenAccounts(ourviewmodel, utilityviewmodel);
            //if (utilityviewmodel.Hezbollah.utility_accountsList.Count > 0)
            //{
            //    // This has a 'Distinct' in the Select
            //    List<SmartUtility.Resources> resources_found = SmartSpikeUtilityV2017.Lookup_Resources(utilityviewmodel,
            //                                                                        SmartSpikeUtilityV2017.Find_Open_Accounts(utilityviewmodel)
            //                                                                        resourcesList);
            //    if (resources_found.Count > 1)
            //    {
            //        return true;
            //    }
            //}
            //// Count of 0 or 1 means no dual fuel
            //return false;
        }

        internal static void UtilityUpdateCulture(UtilityViewModel utilityviewmodel)
        {
#if WINFORMS
            // When I'm not so tired!!!

            var picker = utilityviewmodel.StartDate;

            // Reapply format to force culture refresh
            //utilityviewmodel.StartDate. 
            //picker.Format = DateTimePickerFormat.Long;
            //picker.Format = DateTimePickerFormat.Short;

            //System.Windows.Forms.DateTimePicker temp = utilityviewmodel.StartDate;
            //utilityviewmodel.StartDate = null;
            //utilityviewmodel.StartDate = temp;
            //temp = utilityviewmodel.EndDate;
            //utilityviewmodel.EndDate = null;
            //utilityviewmodel.EndDate = temp;
            
            //DateTimeOffset temp_date = utilityviewmodel.StartDate.SelectedDate;
            //utilityviewmodel.StartDate = SmartParametersV2016.defaultDate;
            //utilityviewmodel.StartDate = temp_date.DateTime;
            //temp_date = utilityviewmodel.EndDate;
            //utilityviewmodel.EndDate = SmartParametersV2016.defaultDate;
            //utilityviewmodel.EndDate = temp_date.DateTime;
#endif
#if WPF  || WINUI || SMARTMAUI
            // Force the Start and End dates to change so the new format is revealed
            DateTime temp = utilityviewmodel.StartDate;
            //utilityviewmodel.StartDate = null;
            utilityviewmodel.StartDate = temp;
            temp = utilityviewmodel.EndDate;
            //utilityviewmodel.EndDate = null;
            utilityviewmodel.EndDate = temp;            
#endif
#if ANDROIDX
            // Force the Start and End dates to change so the new format is revealed
            DateTimeOffset temp_date = utilityviewmodel.StartDate.DateTime;
            utilityviewmodel.StartDate.DateTime = SmartParametersV2016.defaultDate;
            utilityviewmodel.StartDate.DateTime = temp_date.DateTime;
            temp_date = utilityviewmodel.EndDate.DateTime;
            utilityviewmodel.EndDate.DateTime = SmartParametersV2016.defaultDate;
            utilityviewmodel.EndDate.DateTime = temp_date.DateTime;
#endif
        }

        internal static void Build_WhereList(MainViewModel ourviewmodel,
                                                UtilityViewModel utilityviewmodel,
                                                char local_resource_code,
                                                bool buttonDualFuel)
        {
            List<SmartUtility.Meters> mpan_mprnList = new List<SmartUtility.Meters>();
            if ((local_resource_code == SmartParametersV2016.Electricity) ||
            (buttonDualFuel))
            {
                mpan_mprnList = SmartSpikeUtilityV2017.Utility_Lookup_MetersByResource(ourviewmodel,
                                                                                utilityviewmodel,
                                                                                local_resource_code);
            }
            if ((local_resource_code == SmartParametersV2016.Gas) ||
                (buttonDualFuel == true))
            {
                mpan_mprnList = SmartSpikeUtilityV2017.Utility_Lookup_MetersByResource(ourviewmodel,
                                                                                utilityviewmodel,
                                                                                local_resource_code);
            }
            foreach (SmartUtility.Meters meter_row in mpan_mprnList) // Checked
            {
                utilityviewmodel.resource_codeList.Add(meter_row.MPAN_MPRN);
            }
            return;
        }

        internal static async Task<bool> Utility_CultureChanged_Actual(
#if WINFORMS
                                                      MainProcess process_components,
#endif
#if ANDROIDX
                                                      AppCompatActivity meterActivity,
#endif
                                                      MainViewModel ourviewmodel,
                                                      UtilityViewModel utilityviewmodel)
        {
            // We are going to get info? Better turn off some buttons first!
            // These can all be turned off now, except the Cancel button
            TurnOffUtilityStatus(
#if WINFORMS
                                 process_components,
#endif
                                 utilityviewmodel);
            bool status = false;
            // Despite everything and all the pain
            // your program is ABSOLUTELY beautiful Ray, and its ALL YOURS!
            if (!await DisplayUtilityMeterAsync(
#if WINFORMS
                                                    process_components,
#endif
#if ANDROIDX
                                                    meterActivity,
#endif
                                                     ourviewmodel,
                                                     utilityviewmodel,
                                                     SmartParametersV2016.activeFlag,
                                                     SmartParametersV2016.defaultDate))
            {
                // We failed because of a Cancellation ... but which one? Check
                if (!(ourviewmodel.quitCts.IsCancellationRequested ||
                        utilityviewmodel.utilityToken.IsCancellationRequested))
                {
                    // If we DIDN'T request a cancellation
                    // We do nothing if there is a failure .. !  SmartSwitch DOESN'T fail!!
                    FrontEndGUI.SetLedColour(ourviewmodel, 8, ourviewmodel.redColour);
                }
            }
            else
            {
                foreach (SmartProfile.Cubefaces cubeface_row in ourviewmodel.SmartProfile.profilecubefacesList) // Checked
                {
                    if (cubeface_row.CUBEFACE_CODE == SmartParametersV2016.Utility)
                    {
                        cubeface_row.FACE_CULTURE_CODE = utilityviewmodel.UtilityCultureCode;
                        cubeface_row.Updated = true;
                        ourviewmodel.SmartProfile.profilecubefacesChangesList.Add(cubeface_row);
                        break;
                    }
                }

                if (!await SmartRoutinesV2018.DoAllSmartProfile(ourviewmodel,
                                                                SmartParametersV2016.sqliteformat))
                {
                    FrontEndGUI.SetLedColour(ourviewmodel, 6, ourviewmodel.redColour);
                    if (!await SmartRoutinesV2018.CheckTrace(ourviewmodel, utilityviewmodel.utilityToken, 0, 0, "Problem 61: Utility Culture Switch"))
                    {
                        return false;
                    }
                }
                else
                {
                    // Tell the console we have switched
                    if (await SmartRoutinesV2018.TextBlockUpdate(
#if ANDROIDX
                        meterActivity,
#endif
                        ourviewmodel, "Utility Culture switched to: " + utilityviewmodel.UtilityCultureCode))
                    {
                        status = true;
                    }
                }
            }
            // We are going to get info? Better turn off some buttons first!
            // These can all be turned off now, except the Cancel button
            TurnOnUtilityStatus(
#if WINFORMS
                                 process_components,
#endif
                                 utilityviewmodel);
            return status;    // Either nothing was done OR something was done successfully
        }

        internal static async Task<bool> Utility_RateChanged_Actual(
#if WINFORMS
                                                                      MainProcess process_components,
#endif
#if ANDROIDX
                                                                      AppCompatActivity meterActivity,
#endif
                                                                      MainViewModel ourviewmodel,
                                                                      UtilityViewModel utilityviewmodel,
                                                                      short targetOrdinal,
                                                                      string targetCurrency)
        {
            // We are going to get info? Better turn off some buttons first!
            // These can all be turned off now, except the Cancel button
            TurnOffUtilityStatus(
#if WINFORMS
                                     process_components,
#endif
                                 utilityviewmodel);
            bool status = false;
            // Despite everything and all the pain
            // your program is ABSOLUTELY beautiful Ray, and its ALL YOURS!
            //UtilityUpdateRate(utilityviewmodel);

            if (!await DisplayUtilityMeterAsync(
#if WINFORMS
                                                    process_components,
#endif
#if ANDROIDX
                                                    meterActivity,
#endif
                                                     ourviewmodel,
                                                     utilityviewmodel,
                                                     SmartParametersV2016.activeFlag,
                                                     SmartParametersV2016.defaultDate))
            {
                // We failed because of a Cancellation ... but which one? Check
                if (!(ourviewmodel.quitCts.IsCancellationRequested ||
                        utilityviewmodel.utilityToken.IsCancellationRequested))
                {
                    // If we DIDN'T request a cancellation
                    // We do nothing if there is a failure .. !  SmartSwitch DOESN'T fail!!
                    FrontEndGUI.SetLedColour(ourviewmodel, 8, ourviewmodel.redColour);
                }
            }
            else
            {
                foreach (SmartProfile.Cubefaces cubeface_row in ourviewmodel.SmartProfile.profilecubefacesList) // Checked
                {
                    if (cubeface_row.CUBEFACE_CODE == SmartParametersV2016.Utility)
                    {
                        cubeface_row.FACE_CURRENCY = targetOrdinal;
                        cubeface_row.Updated = true;
                        ourviewmodel.SmartProfile.profilecubefacesChangesList.Add(cubeface_row);
                        break;
                    }
                }

                if (!await SmartRoutinesV2018.DoAllSmartProfile(ourviewmodel,
                                                                SmartParametersV2016.sqliteformat))
                {
                    FrontEndGUI.SetLedColour(ourviewmodel, 6, ourviewmodel.redColour);
                    if (!await SmartRoutinesV2018.CheckTrace(ourviewmodel, utilityviewmodel.utilityToken, 0, 0, "Problem 62: Utility Currency Switch"))
                    {
                        return false;
                    }
                }
                else
                {
                    // Tell the console we have switched
                    if (await SmartRoutinesV2018.TextBlockUpdate(
#if ANDROIDX
                                                         meterActivity,
#endif
                                                        ourviewmodel, "Utility Currency switched to: " + targetCurrency))
                    {
                        status = true;
                    }
                }
            }
            // We are going to get info? Better turn off some buttons first!
            // These can all be turned off now, except the Cancel button
            TurnOnUtilityStatus(
#if WINFORMS
                                     process_components,
#endif
                                 utilityviewmodel);
            return status;    // Either nothing was done OR something was done successfully
        }


        // Slagging off her poor dead sister again ... never lived like me, never did like me, 
        // never took my advice etc. etc. etc.
        // Let the poor woman rest in peace ...
        internal static void UndisplayMeter(MainViewModel ourviewmodel,
                                                UtilityViewModel utilityviewmodel,
                                                char local_resource_code,
                                                string meterFormat,
                                                List<SmartUtility.Switches> switchesList,
                                                List<SmartUtility.Meters> metersList)
        {
            // Costs and Readings
            double total_reading = 0.0,
                    billed_amount = 0.0,
                    projectedCost = 0.0;

            string none = "          <none>           ";

            utilityviewmodel.resource_codeList = new List<string>() { };

            Build_WhereList(ourviewmodel, utilityviewmodel, local_resource_code, utilityviewmodel.DualFuelChecked);

            // Do we need to clear the colours as well?
            List<SmartUtility.Resources> resources_found = SmartSpikeUtilityV2017.Utility_Find_Switches_LAST_CHECKED_ANY(ourviewmodel,
                                                                                                                    utilityviewmodel);

            if (resources_found.Count == 0)
            {
#if WINFORMS || WPF  || WINUI || SMARTMAUI
                utilityviewmodel.TotalBilled = "0";
                utilityviewmodel.TotalkWh = "0";
                utilityviewmodel.ProjectedCost = "0";
                utilityviewmodel.NextConnectionMessage = none;
#endif
#if ANDROIDX
                utilityviewmodel.TotalBilled.Text = "0";
                utilityviewmodel.TotalkWh.Text = "0";
                utilityviewmodel.ProjectedCost.Text = "0";
                utilityviewmodel.NextConnectionMessage.Text = none;
#endif
                utilityviewmodel.UtilitySuppliersList.Clear();
                utilityviewmodel.UtilityTariffsList.Clear();
                utilityviewmodel.UtilityPaymentPlansList.Clear();
                utilityviewmodel.UtilityAddressesList.Clear();

#if WINFORMS || WPF  || WINUI || SMARTMAUI

                utilityviewmodel.AreaId = "";
                utilityviewmodel.AccountNo = "";
                utilityviewmodel.PostCode = "";

                utilityviewmodel.DayRate1 = "";
                utilityviewmodel.NightRate1 = "";
                utilityviewmodel.StandingCharge1 = "";
#endif
#if ANDROIDX
                utilityviewmodel.AreaId.Text = "";
                utilityviewmodel.AccountNo.Text = "";
                utilityviewmodel.PostCode.Text = "";

                //utilityviewmodel.DayRate1.Text = "";
                //utilityviewmodel.NightRate1.Text = "";
                //utilityviewmodel.StandingCharge1.Text = "";
#endif
#if WPF  || WINUI || SMARTMAUI
                utilityviewmodel.PictureBoxDNO = null;
#endif
                utilityviewmodel.UtilityCosts = new List<SmartUtility.AnalysisCostsView>();
                utilityviewmodel.UtilityBills = new List<SmartUtility.AnalysisBillsView>();
                utilityviewmodel.UtilityReadings = new List<SmartUtility.AnalysisReadingsView>();
                utilityviewmodel.UtilityBreakdown = new List<SmartUtility.AnalysisBreakdownView>();
#if WINFORMS
                utilityviewmodel.Password = "";
#endif
#if WPF  || WINUI || SMARTMAUI
                //utilityviewmodel.PasswordHandler = () => "";
                utilityviewmodel.Password = "";
#endif
#if WINFORMS || WPF  || WINUI || SMARTMAUI
                utilityviewmodel.DayRate2 = "";
                utilityviewmodel.NightRate2 = "";
                utilityviewmodel.StandingCharge2 = "";
#endif
#if ANDROIDX
                utilityviewmodel.DayRate2.Text = "";
                utilityviewmodel.NightRate2.Text = "";
                utilityviewmodel.StandingCharge2.Text = "";
#endif

#if WINFORMS
                utilityviewmodel.Economy7State = false;
#endif
#if WPF || SMARTMAUI
                utilityviewmodel.Economy7State = Visibility.Hidden;
#endif
#if WINUI 
                utilityviewmodel.Economy7State = Visibility.Collapsed;
#endif
#if ANDROIDX
                utilityviewmodel.Economy7State = false;
#endif
#if WINFORMS || WPF  || WINUI || SMARTMAUI
                utilityviewmodel.CheckBox60 = false;
#endif
#if ANDROIDX
                utilityviewmodel.CheckBox60.Checked = false;
#endif
#if WINFORMS
                // Costs and Readings
                //utilityviewmodel.TotalkWh = string.Format(meterFormat, total_reading);
                //utilityviewmodel.TotalBilled = ((decimal)(billed_amount / 100.0) * ourviewmodel.exchangeRate[utilityviewmodel.currencyOrdinal]).ToString("c", utilityviewmodel.utilityDisplayCulture);
                // This is for the likes of Scottish Power .. twats
                //utilityviewmodel.ProjectedCost = ((decimal)(projectedCost / 100.0) * ourviewmodel.exchangeRate[utilityviewmodel.currencyOrdinal]).ToString("c", utilityviewmodel.utilityDisplayCulture);
#endif
#if WINFORMS || WPF  || WINUI || SMARTMAUI
                // Costs and Readings
                utilityviewmodel.TotalkWh = total_reading.ToString(meterFormat, utilityviewmodel.CultureINF);
                utilityviewmodel.TotalBilled = DisplayValue(ourviewmodel,
                                                            utilityviewmodel,
                                                            utilityviewmodel.ConvertToSymbol,
                                                            ourviewmodel.exchangeRate,
                                                            (decimal)(billed_amount / 100.0));

                //((decimal)(billed_amount / 100.0) * ourviewmodel.exchangeRate[utilityviewmodel.currencyOrdinal]).ToString("c", utilityviewmodel.utilityDisplayCulture);
                // This is for the likes of Scottish Power .. twats
                utilityviewmodel.ProjectedCost = DisplayValue(ourviewmodel,
                                                            utilityviewmodel,
                                                            utilityviewmodel.ConvertToSymbol,
                                                            ourviewmodel.exchangeRate,
                                                            (decimal)(projectedCost / 100.0));

                //((decimal)(projectedCost / 100.0) * ourviewmodel.exchangeRate[utilityviewmodel.currencyOrdinal]).ToString("c", utilityviewmodel.utilityDisplayCulture);
#endif
#if ANDROIDX
                // Costs and Readings
                utilityviewmodel.TotalkWh.Text = total_reading.ToString(meterFormat, utilityviewmodel.CultureINF);
                utilityviewmodel.TotalBilled.Text = DisplayValue(ourviewmodel,
                                                            utilityviewmodel,
                                                            utilityviewmodel.ConvertToSymbol,
                                                            ourviewmodel.exchangeRate,
                                                            (decimal)(billed_amount / 100.0));

                //((decimal)(billed_amount / 100.0) * ourviewmodel.exchangeRate[utilityviewmodel.currencyOrdinal]).ToString("c", utilityviewmodel.utilityDisplayCulture);
                // This is for the likes of Scottish Power .. twats
                utilityviewmodel.ProjectedCost.Text = DisplayValue(ourviewmodel,
                                                            utilityviewmodel,
                                                            utilityviewmodel.ConvertToSymbol,
                                                            ourviewmodel.exchangeRate,
                                                            (decimal)(projectedCost / 100.0));

                //((decimal)(projectedCost / 100.0) * ourviewmodel.exchangeRate[utilityviewmodel.currencyOrdinal]).ToString("c", utilityviewmodel.utilityDisplayCulture);
#endif
            }
            return;
        }

        internal static int Utility_Address_DropDown(MainViewModel ourviewmodel,
                                                UtilityViewModel utilityviewmodel)
        {
            int address_index = -1;
            switch (utilityviewmodel.resource_code)
            {
                case SmartParametersV2016.Electricity:
                    List<SmartProfile.AddressesView> electricity_addresses_found =
                        SmartSpikeV2017.Users_Lookup_AddressesUDPRN(ourviewmodel,
                                                                    ourviewmodel.UserName,
                                                                    utilityviewmodel.startup_udprn);
                    foreach (SmartProfile.AddressesView address_row in electricity_addresses_found)
                    {
                        if (!Build_Utility_UDPRN_Dropdown(ourviewmodel,
                                                    utilityviewmodel,
                                                    utilityviewmodel.startup_udprn,
                                                    address_row.BASIC))
                        {
                            utilityviewmodel.UtilityAddressesList.Clear();
                        }
                        else
                        {
                            address_index = utilityviewmodel.udprn_index;
                        }
                    }
                    break;
                case SmartParametersV2016.Gas:
                    List<SmartProfile.AddressesView> gas_addresses_found =
                        SmartSpikeV2017.Users_Lookup_AddressesUDPRN(ourviewmodel,
                                                                    ourviewmodel.UserName,
                                                                    utilityviewmodel.startup_udprn);
                    foreach (SmartProfile.AddressesView address_row in gas_addresses_found)
                    {
                        if (!Build_Utility_UDPRN_Dropdown(ourviewmodel,
                                                    utilityviewmodel,
                                                    utilityviewmodel.startup_udprn,
                                                    address_row.BASIC))
                        {
                            utilityviewmodel.UtilityAddressesList.Clear();
                        }
                        else
                        {
                            address_index = utilityviewmodel.udprn_index;
                        }
                    }
                    break;
                default:
                    break;
            }
            return address_index;
        }

        //internal static void Utility_Update_Pending(UtilityViewModel utilityviewmodel,
        //                                            char resource_code, 
        //                                            bool value)
        //{
        //    switch (resource_code)
        //    {
        //        case SmartParametersV2016.Electricity:
        //            utilityviewmodel.autoswitchToggle[0] = value;
        //            break;
        //        case SmartParametersV2016.Gas:
        //            utilityviewmodel.autoswitchToggle[1] = value;
        //            break;
        //        case SmartParametersV2016.DualFuel:
        //            utilityviewmodel.autoswitchToggle[0] = value;
        //            utilityviewmodel.autoswitchToggle[1] = value;
        //            break;
        //        default:
        //            break;
        //    }
        //    return;
        //}

        internal static bool Utility_Get_Pending(UtilityViewModel utilityviewmodel,
                                                char resource_code)
        {
            switch (resource_code)
            {
                case SmartParametersV2016.Electricity:
                    return utilityviewmodel.autoswitchToggle[0];
                case SmartParametersV2016.Gas:
                    return utilityviewmodel.autoswitchToggle[1];
                case SmartParametersV2016.DualFuel:
                    return utilityviewmodel.autoswitchToggle[0] && utilityviewmodel.autoswitchToggle[1];
                default:
                    break;
            }
            return false;
        }

        internal static async Task<bool> Start_Utility_Display(
#if WINFORMS
                                                MainProcess components,
#endif
#if ANDROIDX
                                                AppCompatActivity meterActivity,
#endif
                                                MainViewModel ourviewmodel,
                                                UtilityViewModel utilityviewmodel,
                                                DateTime next_connection,
                                                char cubeface_code,
                                                string face_last_display)
        {
            //
            // In Utility we have two Resources - Electricity and Gas (Water sometime in the future!)
            //
            try
            {
                utilityviewmodel.autoswitchToggle = new bool[2] { false, false };

                int defaultDates_count = SmartSpikeUtilityV2017.Utility_Lookup_Accounts(ourviewmodel,
                                                                                utilityviewmodel,
                                                                                SmartParametersV2016.defaultDate).Count;
                UtilityDisplaySwitch(
#if WINFORMS
                                    components,
#endif
#if ANDROIDX
                                    meterActivity,
#endif
                                    ourviewmodel,
                                    utilityviewmodel);
                //rf defaultDates_count);
                if (utilityviewmodel.resource_code != SmartParametersV2016.defaultResourceCode)  // Was local_resource_code
                {
                    if (Utility_Get_Pending(utilityviewmodel, utilityviewmodel.resource_code))
                    {
                        utilityviewmodel.AutoSwitchUtilityColour = ourviewmodel.darkorangeColour;
#if ANDROIDX
                        ourviewmodel.AutoSwitch0 = ourviewmodel.openred0;
                        ourviewmodel.AutoSwitch25 = ourviewmodel.openred25;
                        ourviewmodel.AutoSwitch50 = ourviewmodel.openred50;
                        ourviewmodel.AutoSwitch75 = ourviewmodel.openred75;
#endif
                    }
                    else
                    {
                        utilityviewmodel.AutoSwitchUtilityColour = ourviewmodel.darkredColour;
#if ANDROIDX
                        ourviewmodel.AutoSwitch0 = ourviewmodel.autoswitch0;
                        ourviewmodel.AutoSwitch25 = ourviewmodel.autoswitch25;
                        ourviewmodel.AutoSwitch50 = ourviewmodel.autoswitch50;
                        ourviewmodel.AutoSwitch75 = ourviewmodel.autoswitch75;
#endif
                    }

                    
                    FrontEndGUI.SetLedColour(ourviewmodel, 8, ourviewmodel.orangeColour);
                    // Now ... can we set the Dual Fuel button?
                    utilityviewmodel.dual_fuel = Dual_Fuel(ourviewmodel, utilityviewmodel);
                    //if (ourviewmodel.meterDisplay)
                    //{
                    // Before you wonder why this isn't an 'async void' routine, check out
                    // https://msdn.microsoft.com/en-us/magazine/jj991977.aspx
                    if (!await DisplayUtilityMeterAsync(
#if WINFORMS
                                                        components,
#endif
#if ANDROIDX
                                                    meterActivity,
#endif
                                                    ourviewmodel,
                                                    utilityviewmodel,
                                                    SmartParametersV2016.activeFlag,
                                                    next_connection))
                    {
                        // If we DIDN'T request a cancellation
                        // We do nothing if there is a failure .. !  SmartSwitch DOES NOT fail!!
                        // We failed because of a Cancellation ... but which one? Check
                        return false;
                    }
                    // Turn on the buttons
#if WINFORMS
                    // Lets not make life difficult for ourselves
                    UtilityView.RadioButtonCheckedEvents(components, true); // MouseClick
#endif
#if WPF  || WINUI || SMARTMAUI
                    UtilityView.RadioButtonCheckedEvents((UtilityView)SmartRoutinesV2018.CubefaceView(ourviewmodel, SmartParametersV2016.Utility), ourviewmodel, utilityviewmodel, true);

#endif
                    SmartRoutinesV2018.FocusOpenClose(ourviewmodel);

                    //switch (utilityviewmodel.ChimpFuckers)
                    //{
                    //    case SmartParametersV2016.Electricity:
                    //        utilityviewmodel.ElectricityChecked = true;
                    //        break;
                    //    case SmartParametersV2016.Gas:
                    //        utilityviewmodel.GasChecked = true;
                    //        break;
                    //    case SmartParametersV2016.DualFuel:
                    //        utilityviewmodel.DualFuelChecked = true;
                    //        break;
                    //}

                    // Even enabling the drop-downs HERE ... the SelectedIndex event
                    // still gets called (what a pile of bollocks)
                    SmartUtilityV2022.TurnOnUtilityStatus(
#if WINFORMS
                                                        components,
#endif
                                                        utilityviewmodel);

                    //if (face_last_display != "")
                    //{
                    //    ourviewmodel.screenCode = screen_code;
                    //}
                    await SmartRoutinesV2018.TextBlockUpdate(
#if ANDROIDX
                                            meterActivity,
#endif
                                            ourviewmodel,
                                            "Cubeface: " + cubeface_code + " display");
                    //else
                    //{
                    //    // If the last Supplier is 'set' (possibly as the result of a failed login)
                    //    // then don't clear it down, so it remains visible for a subsequent CANCEL
                    //    if (utilityviewmodel.SupplierSelectedIndex == -1)
                    //    {
                    //        // If its the inital run through, then build the list
                    //        int utility_brand_index = -1;
                    //        if (!Rebuild_USD(ourviewmodel,
                    //                                            utilityviewmodel,
                    //                                            "",
                    //                                            (utilityviewmodel.resource_code == SmartParametersV2016.defaultResourceCode ? SmartParametersV2016.activeFlag : SmartParametersV2016.activeDefault),  // This might be an initial setup
                    //                                            0,      // Brand code
                    //                                            rf utility_brand_index))
                    //        {
                    //            utilityviewmodel.UtilitySuppliersList.Clear();
                    //        }
                    //        else
                    //        {
                    //            if (utility_brand_index != -1)
                    //            {
                    //                UtilityViewModel.SupplierItem supplier_item = utilityviewmodel.UtilitySuppliersList[utility_brand_index];
                    //            }
                    //        }
                    //    }
                }
            }
            catch (ArgumentNullException exception)
            {
                // Nothing ever turns this back from Red
                utilityviewmodel.errorMessage = exception.Message;
            }
            catch (NullReferenceException exception)
            {
                // Nothing ever turns this back from Red
                utilityviewmodel.errorMessage = exception.Message;
            }
            catch (Exception exception)
            {
                utilityviewmodel.errorMessage = exception.Message;
            }
            if (!string.IsNullOrEmpty(utilityviewmodel.errorMessage))
            {
                // Nothing ever turns this back from Red
                // Try and tell HQ
                if (!await SmartRoutinesV2018.CheckTrace(ourviewmodel, utilityviewmodel.utilityToken, 0, 0, "Problem " + " " + utilityviewmodel.errorMessage))
                {
                    return false;
                }
                return false;
            }
            return true;
        }

        internal static void UtilityDisplaySwitch(
#if WINFORMS
                                                    MainProcess components,
#endif
#if ANDROIDX
                                                    AppCompatActivity meterActivity,
#endif
                                                    MainViewModel ourviewmodel,
                                                    UtilityViewModel utilityviewmodel)
        {
            foreach (SmartUtility.Resources resource_row in utilityviewmodel.Hezbollah.utility_resourcesList)
            {
                // DualFuel 'D' will never be in resourceList
                if (resource_row.CHECKED == SmartParametersV2016.lastChecked)
                {
                    utilityviewmodel.resource_code = resource_row.RESOURCE_CODE;

                    switch (utilityviewmodel.resource_code)
                    {
                        case SmartParametersV2016.Electricity:
#if WINFORMS
                            components.URElectricity.Checked = true;
#endif
#if WPF  || WINUI || SMARTMAUI
                            utilityviewmodel.ElectricityChecked = true;
#endif
#if ANDROIDX
                            utilityviewmodel.ElectricityChecked = true;
                            utilityviewmodel.RBElectricity.Checked = true;
#endif
                            utilityviewmodel.meterFormat = SmartParametersV2016.format0Places;

                            break;
                        case SmartParametersV2016.Gas:
#if WINFORMS
                            components.URGas.Checked = true;
#endif
#if WPF  || WINUI || SMARTMAUI
                            utilityviewmodel.GasChecked = true;
#endif
#if ANDROIDX
                            utilityviewmodel.GasChecked = true;
                            utilityviewmodel.RBGas.Checked = true;
#endif
                            utilityviewmodel.meterFormat = SmartParametersV2016.format3Places;

                            break;
                        default:
                            break;

                    }
                    break;
                }
            }

            // Now ... if NONE of the resourceList result in a
            // 'CHECKED' resource, we can only assume that NEITHER
            // 'E' nor 'G' was 'CHECKED' ... so that must mean 'D'!!
            if (utilityviewmodel.resource_code == SmartParametersV2016.defaultResourceCode)
            {
                utilityviewmodel.resource_code = SmartParametersV2016.DualFuel;
#if WINFORMS
                components.URDualFuel.Checked = true;
#endif
#if WPF  || WINUI || SMARTMAUI
                utilityviewmodel.DualFuelChecked = true;
#endif
#if ANDROIDX
                utilityviewmodel.DualFuelChecked = true;
                utilityviewmodel.RBDualFuel.Checked = true;
#endif
                utilityviewmodel.meterFormat = SmartParametersV2016.format3Places;
            }

            // ALWAYS set it to SOMETHING just in case
            // *Everyone has Electricity*
            if (utilityviewmodel.resource_code == SmartParametersV2016.defaultResourceCode)
            {
                utilityviewmodel.resource_code = SmartParametersV2016.Electricity;
#if WINFORMS
                components.URElectricity.Checked = true;
#endif
#if WPF  || WINUI || SMARTMAUI
                utilityviewmodel.ElectricityChecked = true;
#endif
#if ANDROIDX
                utilityviewmodel.ElectricityChecked = true;
                utilityviewmodel.RBElectricity.Checked = true;
#endif
            }

            //if (consumers_view_row.AUTOSWITCH == Convert.ToChar(SmartParametersV2016.yesFlag))
            //{
            //    Utility_Update_Pending(consumers_view_row.CATEGORY_CODE, true, rf autoswitch);
            //}

#if ANDROIDX
            utilityviewmodel.UtilitySpinnerAddressesList = new List<string>();
#endif

            utilityviewmodel.UtilityAddressesList.Clear();
            List<MainViewModel.AddressItem> tempList = new List<MainViewModel.AddressItem>();

            List<SmartProfile.AddressesView> addressList = SmartSpikeUtilityV2017.Utility_Configure_Addresses(ourviewmodel, utilityviewmodel);
            foreach (SmartProfile.AddressesView address_row in addressList)
            {
                // What are we saying here is that IF we have an AccountNo then we MUST have an address
                // to hook it to, because Suppliers work like that!  We can say 'if you have an
                // BANK ACCOUNT then you must have an address to hook it to 'cos a bank won't open an
                // account for you without an address
                // We won't have ANY Utility Accounts without a UDPRN which
                // can be joined back to Addresses.  Likewise we will never
                // have ANY Utility transactions which don't can't be joined
                // back to a Utility Account and hence to an Address.

                MainViewModel.AddressItem addressItem = new MainViewModel.AddressItem()
                {
                    Content = address_row.BASIC,
                    Value = address_row.UDPRN,
                    Colour = ourviewmodel.blackColour
                };
                tempList.Add(addressItem);
#if ANDROIDX
                utilityviewmodel.UtilitySpinnerAddressesList.Add(addressItem.Content);
#endif
            }

            if (tempList.Count > 0)
            {
                utilityviewmodel.UtilityAddressesList = tempList;
#if ANDROIDX
                ArrayAdapter<string> addressAdapter = new ArrayAdapter<string>(meterActivity, Android.Resource.Layout.SimpleSpinnerItem, utilityviewmodel.UtilitySpinnerAddressesList);
                addressAdapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
                utilityviewmodel.Addresses.Adapter = addressAdapter;
                utilityviewmodel.Addresses.ItemSelected += new EventHandler<AdapterView.ItemSelectedEventArgs>((s, e) => UtilityView.Utility_AddressSelectionChanged(s, e, ourviewmodel, utilityviewmodel));
#endif
                //utilityviewmodel.AddressSelectedIndex = 0;

            }

            List<MainViewModel.CultureItem> temp =
                new List<MainViewModel.CultureItem>();
            int index = -1;
            int selectedIndex = 0;
            foreach (SmartData.CultureView cv in ourviewmodel.cultureviewList)
            {
                index++;
                MainViewModel.CultureItem cultureItem = new MainViewModel.CultureItem()
                {
                    CultureID = index,
                    Content = cv.CULTURE_CODE,
                    Source = cv.DESCRIPTION
                };
                if (cv.CULTURE_CODE == utilityviewmodel.UtilityCultureCode)
                {
                    // So the first default item appears 'green' in the dropdown
                    cultureItem.Colour = ourviewmodel.greenColour;
                    selectedIndex = index;
                }
                else
                {
                    cultureItem.Colour = ourviewmodel.blackColour;
                }
                temp.Add(cultureItem);
            }
            utilityviewmodel.UtilityCulturesList = temp;
            utilityviewmodel.LastCultureSelectedIndex = selectedIndex;

#if WINFORMS
            components.comboBoxGUIUtilityCultures.DataSource = utilityviewmodel.UtilityCulturesList;
            components.utilityCulturesListBindingSource.DataMember = "UtilityCulturesList";
            components.utilityCulturesListBindingSource.DataSource = components.UtilityBindingSource;
            components.comboBoxGUIUtilityCultures.DataBindings.Add(new("SelectedIndex", components.utilityCulturesListBindingSource, "Content", true, DataSourceUpdateMode.OnPropertyChanged));
            if (components.comboBoxGUIUtilityCultures.Items.Count > 0)
            {
                components.comboBoxGUIUtilityCultures.SelectedIndex = selectedIndex;
            }
            else
            {
                components.comboBoxGUIUtilityCultures.SelectedIndex = -1;
            }
            components.comboBoxGUIUtilityCultures.SelectedIndexChanged += (s, e) => components.UtilityCultures_SelectedIndexChanged(s, e, ourviewmodel, utilityviewmodel); 
#endif

#if ANDROIDX
            SmartUtilityV2022.UtilityCulturesAdapter adapter = new SmartUtilityV2022.UtilityCulturesAdapter(meterActivity, utilityviewmodel.UtilityCulturesList);
            utilityviewmodel.UtilityCultures.Adapter = adapter;
            // Otherwise 'CAN' appears in the first slot
            utilityviewmodel.UtilityCultures.SetSelection(utilityviewmodel.LastCultureSelectedIndex);
#endif
            // I can't for the life of me think why I put this stuff here ...
            // Yes I can, but it was Cultures and not Currency . Silly me.
            switch (utilityviewmodel.ConvertToSymbol)
            {
                case "GBP":
#if WINFORMS
                    components.UtilityRBGBP.Checked = true;
#endif
#if WPF  || WINUI || SMARTMAUI
                    utilityviewmodel.UtilityGBPChecked = true;
#endif
#if ANDROIDX
                    utilityviewmodel.UtilityRBGBP.Checked = true;
                    utilityviewmodel.UtilityRBGBP.SetTextColor(ourviewmodel.greenColour);
#endif
                    break;
                case "EUR":
#if WINFORMS
                    components.UtilityRBEUR.Checked = true;
#endif
#if WPF  || WINUI || SMARTMAUI
                    utilityviewmodel.UtilityEURChecked = true;
#endif
#if ANDROIDX
                    utilityviewmodel.UtilityRBEUR.Checked = true;
                    utilityviewmodel.UtilityRBEUR.SetTextColor(ourviewmodel.greenColour);
#endif
                    break;
                case "USD":
#if WINFORMS
                    components.UtilityRBUSD.Checked = true;
#endif
#if WPF  || WINUI || SMARTMAUI
                    utilityviewmodel.UtilityUSDChecked = true;
#endif
#if ANDROIDX
                    utilityviewmodel.UtilityRBUSD.Checked = true;
                    utilityviewmodel.UtilityRBUSD.SetTextColor(ourviewmodel.greenColour);
#endif
                    break;
                case "JPY":
#if WINFORMS
                    components.UtilityRBJPY.Checked = true;
#endif
#if WPF  || WINUI || SMARTMAUI
                    utilityviewmodel.UtilityJPYChecked = true;
#endif
#if ANDROIDX
                    utilityviewmodel.UtilityRBJPY.Checked = true;
                    utilityviewmodel.UtilityRBJPY.SetTextColor(ourviewmodel.greenColour);
#endif
                    break;
                default:
#if WINFORMS
                    components.UtilityRBNON.Checked = true;
#endif
#if WPF  || WINUI || SMARTMAUI
                    utilityviewmodel.UtilityNONChecked = true;
#endif
#if ANDROIDX
                    utilityviewmodel.UtilityRBNON.Checked = true;
                    utilityviewmodel.UtilityRBNON.SetTextColor(ourviewmodel.greenColour);
#endif
                    break;
            }
            // Not sure why this is here - shouldn't it be in MainMeter?
            ourviewmodel.exchangeRate = SmartSpikeV2017.Lookup_Todays_Exchange_Rate(ourviewmodel.Blanche.exchangeRatesList);

            return;
        }

        //
        // Before you wonder why this isn't an 'async void' routine, check out
        // https://msdn.microsoft.com/en-us/magazine/jj991977.aspx
        //
        internal static async Task<bool> DisplayUtilityMeterAsync(
#if WINFORMS
                                                    MainProcess process_components,
#endif
#if ANDROIDX
                                                    AppCompatActivity meterActivity,
#endif
                                                    MainViewModel ourviewmodel,
                                                    UtilityViewModel utilityviewmodel,
                                                    char activeFlag,
                                                    DateTime next_connection)
        {
            string urgent_message = "";
            short supplier_code = 0;
            short brand_code = 0;


            List<SmartUtility.Accounts> accounts_found =
                    SmartSpikeUtilityV2017.Utility_Lookup_Accounts(ourviewmodel,
                                                        utilityviewmodel,
                                                        SmartParametersV2016.defaultDate);
            if (accounts_found.Count == 0)
            {
                ClearDownEverythingUtility(ourviewmodel, utilityviewmodel, SmartParametersV2016.none);
                // This IS a show-stopper because we should ALWAYS have an Resource even if we have no Readings, no Usage and/or no Bills
                if (!await SmartRoutinesV2018.CheckTrace(ourviewmodel, utilityviewmodel.utilityToken, supplier_code, brand_code, SmartParametersV2016.Utility.ToString() + " " + utilityviewmodel.resourceCodes + " Has no Accounts"))
                {
                    return false;
                }
                // Think this is the only place where we return a false - No it isn't
                goto dropdowns;
            }
            else
            {
                // Readings
                utilityviewmodel.UtilityReadings.Clear();
                // Bills
                utilityviewmodel.UtilityBills.Clear();
                // Charts
                utilityviewmodel.projectedCost = 0;
                // Breakdown
                utilityviewmodel.UtilityBreakdown.Clear();

                // Mundae ...!
                // SURELY this isn't necessary??
                //utilityviewmodel.resourceCodes = accounts_found.First().RESOURCE_CODE;

                utilityviewmodel.tariff_code = accounts_found.First().TARIFF_CODE;
                utilityviewmodel.payment_plan = accounts_found.First().PAYMENT_PLAN;

                supplier_code = accounts_found.First().SUPPLIER_CODE;
                brand_code = accounts_found.First().BRAND_CODE;
                if (utilityviewmodel.CurrencyOrdinal == 0)
                {
                    foreach (SmartData.Currencies abcd in ourviewmodel.currenciesList)
                    {
                        if (abcd.ORDINAL == accounts_found.First().CURRENCY_ORDINAL)
                        {
                            //indexpos = abcd.ORDINAL;
                            //indexpos--;
                            utilityviewmodel.CurrencyOrdinal = abcd.ORDINAL;
                            utilityviewmodel.ConvertToSymbol = abcd.ISOCURRENCYSYMBOL;
                            break;
                        }
                    }
                }

                // Look up the Utility Addresses JUST to get the Area Code and the
                // Post Code to put in the display - that's all
                // (The addresses dropdown is built later)
                string postcode = "";
                //List<SmartUsers.Addresses> ad_found =
                //        SmartSpikeUtilityV2017.Utility_Find_Addresses(ourviewmodel,
                //                                            utilityviewmodel);
                //if (ad_found.Count == 0)
                //{
                //    // This IS a show-stopper because the Area code should never be 0
                //    // If it is ever ZERO, then its probably because there
                //    // Is NO ADDRESS assigned to an account, and that's fatal
                //    // No Utility supplier is going to set up an account for
                //    // equipment at a location for which it doesn't have an address...
                //    if (!await SmartRoutinesV2018.CheckTrace(ourviewmodel, utilityviewmodel.utilityToken, supplier_code, brand_code, SmartParametersV2016.Utility.ToString() + " " +
                //                            utilityviewmodel.resourceCodes +
                //                            " Account has no address: " +
                //                            accounts_found.First().ACCOUNT_NO))
                //    {
                //       return false;
                //    }
                //    // Think this is the only place where we return a false - No it isn't
                //    return false;
                //}
                //else
                //{

                string udprn = accounts_found.First().UDPRN;
                List<SmartProfile.AddressesView> addressViewFound =
                            SmartSpikeV2017.Users_Lookup_AddressesUDPRN(ourviewmodel,
                                                        ourviewmodel.UserName,
                                                        udprn);
                if (addressViewFound.Count > 0)
                {
                    utilityviewmodel.area_code = addressViewFound.First().AREA_CODE; // Might be 0?
                }
                if (utilityviewmodel.area_code == 0)
                {
                    // This IS a show-stopper because the Area code should never be 0
                    // If it is ever ZERO, then its probably because there
                    // Is NO ADDRESS assigned to an account, and that's fatal
                    // No Utility supplier is going to set up an account for
                    // equipment at a location for which it doesn't have an address...
                    if (!await SmartRoutinesV2018.CheckTrace(ourviewmodel, utilityviewmodel.utilityToken, supplier_code, brand_code, SmartParametersV2016.Utility.ToString() + " " +
                                                utilityviewmodel.resourceCodes +
                                                " Account " +
                                                accounts_found.First().ACCOUNT_NO +
                                                " has Area code: " + utilityviewmodel.area_code))
                    {
                        return false;
                    }
                    // Think this is the only place where we return a false - No it isn't
                    return false;
                }
                postcode = addressViewFound.First().POSTCODE;

#if WINFORMS || WPF  || WINUI || SMARTMAUI
                utilityviewmodel.AreaId = utilityviewmodel.area_code.ToString("00");
#endif
#if ANDROIDX
                utilityviewmodel.AreaId.Text = utilityviewmodel.area_code.ToString("00");
#endif
                utilityviewmodel.AreaIdBackground = ourviewmodel.whiteColour;// <== DON'T PUT ANY TEXT IN THIS FIELD, JUST NUMBERS

                if (!string.IsNullOrEmpty(postcode))
                {   // Sort of 'center' this - there are approximately 2 spaces for each char
#if WINFORMS || WPF  || WINUI || SMARTMAUI
                    utilityviewmodel.PostCode = SmartRoutinesV2018.Check_Postcodes(postcode);
#endif
#if ANDROIDX
                    utilityviewmodel.PostCode.Text = SmartRoutinesV2018.Check_Postcodes(postcode);
#endif
                    utilityviewmodel.PostCodeBackground = ourviewmodel.whiteColour;
                }
#if WINFORMS || WPF  || WINUI || SMARTMAUI
                utilityviewmodel.NextConnectionMessage = SmartRoutinesV2018.Check_Next_Connection(utilityviewmodel.CultureINF,
                                                                                                    SmartParametersV2016.none,
                                                                                                    next_connection,
                                                                                                    SmartParametersV2016.defaultDate);
#endif
#if ANDROIDX
                utilityviewmodel.NextConnectionMessage.Text = SmartRoutinesV2018.Check_Next_Connection(utilityviewmodel.CultureINF,
                                                                                                                    SmartParametersV2016.none,
                                                                                                                    next_connection,
                                                                                                                    SmartParametersV2016.defaultDate);
#endif

#if WINFORMS || WPF  || WINUI || SMARTMAUI
                utilityviewmodel.AccountNo = accounts_found.First().ACCOUNT_NO;
#endif
#if ANDROIDX
                utilityviewmodel.AccountNo.Text = accounts_found.First().ACCOUNT_NO;
#endif
                utilityviewmodel.AccountNoBackground = ourviewmodel.whiteColour;

                // Do the first - if we LOOP, then UNIT_RATES gets done more than once !!!
                // 'cos that is how AWAIT works ...

                // Re-do this RAY!!
                List<SmartUtility.Switches> switches_found =
                    SmartSpikeUtilityV2017.Utility_Lookup_Switches(ourviewmodel,
                                                                   utilityviewmodel,
                                                                   supplier_code,
                                                                  brand_code);
                if (switches_found.Count > 0)
                {
                    if (switches_found.First().AUTOSWITCH_EMAIL_SENT != SmartParametersV2016.defaultDate)
                    {
#if WINFORMS
                        utilityviewmodel.LedSwitched = true;
#endif
#if WPF  || WINUI || SMARTMAUI
                        utilityviewmodel.LedSwitched = Visibility.Visible;
#endif
#if ANDROIDX
                        utilityviewmodel.LedSwitched.Text = "o";
#endif
                    }
                    else
                    {
#if WINFORMS
                        utilityviewmodel.LedSwitched = false;
#endif
#if WPF || SMARTMAUI
                        utilityviewmodel.LedSwitched = Visibility.Hidden;
#endif
#if WINUI
                        utilityviewmodel.LedSwitched = Visibility.Collapsed;
#endif
#if ANDROIDX
                        utilityviewmodel.LedSwitched.Text = "";
#endif
                    }
                }

                // Needed in Build Rates
                // G is always SR but E might be SR OR VR .. so we store the E RESOURCE_TYPE in every
                // case because we can always rely on the G. If we ever return TWO 
                // consumer_utility_view records then we have an E AND a G .. but the E
                // will (should?) come before the G (because we have an ORDERBY on it)
                // therfore we can always take the FIRST record ...

                List<SmartUtility.ResourcesTypes> resourcetypes_found =
                    SmartSpikeUtilityV2017.Utility_Find_ResourcesTypes(ourviewmodel,
                                                                            utilityviewmodel,
                                                                            accounts_found.First().RESOURCE_CODE);
                utilityviewmodel.resource_type = resourcetypes_found.First().RESOURCE_TYPE; // Will be (should be) SR or VR

                //utilityviewmodel.Economy7State = Check_Economy(utilityviewmodel.resource_type);

                // Do it this way until we learn how to store .PNG files in the Database ...
                // If we don't have a picture, then go and get it. Even when we switch
                // resources, we need the SAME picture because its not possible to
                // have ONE postcode with Area Suppliers in TWO different areas is it?

                // Just await this - doesn't matter if it returns true or false,
                // 'cos we continue anyway.  Can't stop SmartSwitch for the sake of a fucking picture!
                //  What a fucking balls-ache all of this was ... cross-thread shit and all
                // How ... why??? the FUCK does this work???
                // http://stackoverflow.com/questions/11771223/loading-the-source-of-a-bitmapimage-in-wpf
                // Wouln't have got this in a 1000 years without Habib (unfortunately)
                //                

                utilityviewmodel.dno_stream = Stream.Null;

                // Go and find the image
                // Look in the table first
                // I'm not 1000% sure this 'caching' is needed ...
                // but I'm going to leave it in for the time being
                // Go and find the image
                // Look in the table first
                // I'm not 1000% sure this 'caching' is needed ...
                // but I'm going to leave it in for the time being
                // in case I need it for the Transaction items .. which I'm doing now
                string marketId = "";
                List<SmartUtility.SupplyAreas> supply_areas_found = SmartSpikeUtilityV2017.Utility_Find_SupplyAreas(ourviewmodel, utilityviewmodel);
                if (supply_areas_found.Count > 0)
                {
                    marketId = supply_areas_found.First().MARKET_ID;

                    List<SmartUtility.DistributorInfo> distributorInfo_found = SmartSpikeUtilityV2017.Utility_Find_DistributorInfo(ourviewmodel,
                                                                                                            utilityviewmodel,
                                                                                                            marketId);
                    if (distributorInfo_found.Count > 0)
                    {
                        utilityviewmodel.PictureBoxDNO = Lookup_LOGO(
#if ANDROIDX
                                                             meterActivity,
#endif
                                                                ourviewmodel,
                                                               utilityviewmodel,
                                                               SmartParametersV2016.Utility,
                                                               distributorInfo_found.First().DNO_LOGO);
                        if (utilityviewmodel.PictureBoxDNO == null)
                        {
                            utilityviewmodel.PictureBoxDNO = await Download_LOGOAsync(
#if ANDROIDX
                                                             meterActivity,
#endif
                                                            ourviewmodel,
                                                            utilityviewmodel,
                                                            SmartParametersV2016.Utility,
                                                            distributorInfo_found.First().DNO_LOGO);
                            // It may still be blank here ...! But it shouldn't be!!
                        }
                    }
                }

                // Build rates
                if (!await Build_Rates_Async(
#if ANDROIDX
                                                             meterActivity,
#endif
                                                ourviewmodel,
                                                utilityviewmodel,
                                                utilityviewmodel.resource_code,
                                                utilityviewmodel.tariff_code,
                                                utilityviewmodel.payment_plan,
                                                utilityviewmodel.resource_type))
                {
                    if (!await SmartRoutinesV2018.CheckTrace(ourviewmodel, utilityviewmodel.utilityToken, supplier_code, brand_code, SmartParametersV2016.Utility.ToString() + " " + utilityviewmodel.resourceCodes + " No Rates found for: " + utilityviewmodel.area_code))
                    {
                        return false;
                    }
                    return false;
                }

#if WINFORMS
                utilityviewmodel.DayRate1Brush = ourviewmodel.transparentColour;
                utilityviewmodel.NightRate1Brush = ourviewmodel.transparentColour;
                utilityviewmodel.StandingCharge1Brush = ourviewmodel.transparentColour;
#endif
#if WPF  || WINUI || SMARTMAUI
                utilityviewmodel.DayRate1Brush = ourviewmodel.transparentColour;
                utilityviewmodel.NightRate1Brush = ourviewmodel.transparentColour;
                utilityviewmodel.StandingCharge1Brush = ourviewmodel.transparentColour;
#endif
#if ANDROIDX
                utilityviewmodel.DayRate1Brush = ourviewmodel.transparentColour;
                utilityviewmodel.NightRate1Brush = ourviewmodel.transparentColour;
                utilityviewmodel.StandingCharge1Brush = ourviewmodel.transparentColour;
#endif
#if WINFORMS || WPF  || WINUI || SMARTMAUI
                utilityviewmodel.DayRate1 = "";
                utilityviewmodel.NightRate1 = "";
                utilityviewmodel.StandingCharge1 = "";
#endif
#if ANDROIDX
                utilityviewmodel.DayRate1.Text = "";
                utilityviewmodel.NightRate1.Text = "";
                utilityviewmodel.StandingCharge1.Text = "";
#endif
#if WINFORMS
                utilityviewmodel.DayRate2Brush = ourviewmodel.transparentColour;
                utilityviewmodel.NightRate2Brush = ourviewmodel.transparentColour;
                utilityviewmodel.StandingCharge2Brush = ourviewmodel.transparentColour;
#endif
#if WPF  || WINUI || SMARTMAUI
                utilityviewmodel.DayRate2Brush = ourviewmodel.transparentColour;
                utilityviewmodel.NightRate2Brush = ourviewmodel.transparentColour;
                utilityviewmodel.StandingCharge2Brush = ourviewmodel.transparentColour;
#endif
#if ANDROIDX
                utilityviewmodel.DayRate2Brush = ourviewmodel.transparentColour;
                utilityviewmodel.NightRate2Brush = ourviewmodel.transparentColour;
                utilityviewmodel.StandingCharge2Brush = ourviewmodel.transparentColour;
#endif
#if WINFORMS || WPF  || WINUI || SMARTMAUI
                utilityviewmodel.DayRate2 = "";
                utilityviewmodel.NightRate2 = "";
                utilityviewmodel.StandingCharge2 = "";
#endif
#if ANDROIDX
                utilityviewmodel.DayRate2.Text = "";
                utilityviewmodel.NightRate2.Text = "";
                utilityviewmodel.StandingCharge2.Text = "";
#endif

                Fill_Rates(ourviewmodel,
                            utilityviewmodel,
                            utilityviewmodel.resource_code,
                            supplier_code,
                            utilityviewmodel.tariff_code,
                            utilityviewmodel.payment_plan,
                            utilityviewmodel.resource_type);

                //utilityviewmodel.DayRate = dayrate;     // The labels not the rates themselves
                //utilityviewmodel.NightRate = nightrate; // The labels not the rates themselves
                //utilityviewmodel.DayRate1Brush = DayRate1Brush;
                //utilityviewmodel.NightRate1Brush = NightRate1Brush;
                //utilityviewmodel.StandingCharge1Brush = StandingCharge1Brush;
                //utilityviewmodel.DayRate1 = DayRate1;
                //utilityviewmodel.NightRate1 = NightRate1;
                //utilityviewmodel.StandingCharge1 = StandingCharge1;
                //utilityviewmodel.DayRate2Brush = DayRate2Brush;
                //utilityviewmodel.NightRate2Brush = NightRate2Brush;
                //utilityviewmodel.StandingCharge2Brush = StandingCharge2Brush;
                //utilityviewmodel.DayRate2 = DayRate2;
                //utilityviewmodel.NightRate2 = NightRate2;
                //utilityviewmodel.StandingCharge2 = StandingCharge2;

                // Age 60+?
#if WINFORMS || WPF  || WINUI || SMARTMAUI
                if (utilityviewmodel.CheckBox60)
#endif
#if ANDROIDX
                if (utilityviewmodel.CheckBox60.Checked)
#endif
                {
                    utilityviewmodel.age = 60;
                }
                else
                {
                    utilityviewmodel.age = 0;
                }

                // Do all this setup bollocks -
                // which depends on utilityviewmodel.resourceCodes being set
                if (utilityviewmodel.Hezbollah.billsList.Count > 0)
                {
                    utilityviewmodel.Hezbollah.working_billsList = SmartUtilityV2022.Working_BillsX(ourviewmodel,
                                                                            utilityviewmodel,
                                                                            SmartParametersV2016.defaultDate);
                    if (utilityviewmodel.Hezbollah.working_billsList.Count > 0)
                    {
                        // We are interested in the PERIOD_START and PERIOD_END
                        // Dates **NOT** the BILL_DATE!!!!
#if WINFORMS
                        utilityviewmodel.StartDate = utilityviewmodel.Hezbollah.working_billsList.First().BILL_PERIOD_START;
                        utilityviewmodel.EndDate = utilityviewmodel.Hezbollah.working_billsList.Last().BILL_PERIOD_END;
#endif
#if WPF  || WINUI || SMARTMAUI
                        utilityviewmodel.StartDate = utilityviewmodel.Hezbollah.working_billsList.First().BILL_PERIOD_START;
                        utilityviewmodel.EndDate = utilityviewmodel.Hezbollah.working_billsList.Last().BILL_PERIOD_END;
#endif
#if ANDROIDX
                        utilityviewmodel.StartDate.DateTime = utilityviewmodel.Hezbollah.working_billsList.First().BILL_PERIOD_START;
                        utilityviewmodel.EndDate.DateTime = utilityviewmodel.Hezbollah.working_billsList.Last().BILL_PERIOD_END;
#endif
                    }
                    else
                    {
#if WINFORMS
                        utilityviewmodel.StartDate = SmartParametersV2016.defaultDate;
                        utilityviewmodel.EndDate = SmartParametersV2016.defaultMaxdate;
#endif
#if WPF  || WINUI || SMARTMAUI
                        utilityviewmodel.StartDate = SmartParametersV2016.defaultDate;
                        utilityviewmodel.EndDate = SmartParametersV2016.defaultMaxdate;
#endif
#if ANDROIDX
                        utilityviewmodel.StartDate.DateTime = SmartParametersV2016.defaultDate;
                        utilityviewmodel.EndDate.DateTime = SmartParametersV2016.defaultMaxdate;
#endif
                    }
                }

                utilityviewmodel.tempBrandCode = brand_code;// Use charts?
                utilityviewmodel.tempSupplierCode = supplier_code;// Use charts??
                if (!await UtilityUpdateTabs(
#if WINFORMS
                                            process_components,
#endif
                                            ourviewmodel,
                                            utilityviewmodel,
                                            utilityviewmodel.tempBrandCode,
                                            utilityviewmodel.tempSupplierCode))
                {
                    if (!await SmartRoutinesV2018.CheckTrace(ourviewmodel, utilityviewmodel.utilityToken, supplier_code, brand_code, SmartParametersV2016.Utility.ToString() + " " + utilityviewmodel.errorMessage))
                    {
                        return false;
                    }
                    return false;
                }

#if ANDROIDX
                SmartUtilityV2022.SetUpUtilityViewPager(SmartRoutinesV2018.CubefaceView(ourviewmodel, SmartParametersV2016.Utility), ourviewmodel.Fm, ourviewmodel.Lfc, ourviewmodel, utilityviewmodel);
#endif
            }
        dropdowns:
            // Build dropdowns
            // This now ONLY Suppliers (??)
            //int utility_brand_index = -1;

            // Data for building dropdowns
#if WINFORMS || WPF  || WINUI || SMARTMAUI
            urgent_message = Build_Utility_Dropdowns(
#endif
#if ANDROIDX
            urgent_message = Build_Utility_Dropdowns(
#endif
#if WINFORMS
                                                            process_components,
#endif
                                                            ourviewmodel,
                                                            utilityviewmodel);
            if (!string.IsNullOrEmpty(urgent_message))
            {
                if (!await SmartRoutinesV2018.CheckTrace(ourviewmodel, utilityviewmodel.utilityToken, supplier_code, brand_code, SmartParametersV2016.Utility.ToString() + " " + urgent_message))
                {
                    return false;
                }
                return false;
            }
            return true;
        }

        internal static string DisplayValue(MainViewModel ourviewmodel,
                                            UtilityViewModel utilityviewmodel,
                                            string ConvertToSymbol,
                                            decimal[] exchangeRate,
                                            decimal value)
        {
            NumberFormatInfo nfi;
            if (utilityviewmodel.CurrencyOrdinal > 0)
            {
                if (exchangeRate[utilityviewmodel.CurrencyOrdinal - 1] != 1.0m)
                {
                    value = exchangeRate[utilityviewmodel.CurrencyOrdinal - 1] * value;
                }
            }
            List<SmartData.CultureView> cviews_found =
             new List<SmartData.CultureView>(from cview in ourviewmodel.cultureviewList
                                             where cview.ISOCURRENCYSYMBOL == ConvertToSymbol  //currentCurrency.ISOConvertToSymbol
                                             select cview);
            if (cviews_found.Count > 0)
            {
                nfi = utilityviewmodel.CultureINF.NumberFormat.Clone() as NumberFormatInfo;
                nfi.CurrencySymbol = cviews_found.First().SYMBOL;
                return (value).ToString("c", nfi);
            }
            return "";
        }

        internal static async Task<bool> UtilityUpdateTabs(
#if WINFORMS
                                                    MainProcess process_components,
#endif
                                                    MainViewModel ourviewmodel,
                                                    UtilityViewModel utilityviewmodel,
                                                    short brand_code,
                                                    short supplier_code)
        {
            // Readings
            utilityviewmodel.totalReadings = 0.0M;
            // Bills
            utilityviewmodel.payments_made = 0;
            utilityviewmodel.billed_amount = 0;
            // Charts
            utilityviewmodel.projectedCost = 0;
            // Breakdown

            // Bills
            utilityviewmodel.UtilityBills = Fill_Bills(ourviewmodel,
                                                        utilityviewmodel);
            if (utilityviewmodel.errorMessage == "" &&
                utilityviewmodel.UtilityBills.Count > 0)
            {
                // We have something in the Username field, but
                // do we display it??
#if WPF
                if (ourviewmodel.SmartProfile.profilegroupsList.Count > 0)
                {
                    FrontEndGUI.DecodeDataGridViewUsername(utilityviewmodel.BillsDataGrid, true);
                }
#endif


                // Readings
                utilityviewmodel.UtilityReadings = Fill_Readings(ourviewmodel,
                                                                //utilityviewmodel.utilityDisplayCulture.DateTimeFormat.ShortDatePattern,
                                                                utilityviewmodel,
                                                                utilityviewmodel.resource_code,
                                                                SmartParametersV2016.format3Places);
                if (utilityviewmodel.UtilityReadings.Count > 0)
                {
                    // We have something in the Username field, but
                    // do we display it??
#if WPF
                    //if (ourviewmodel.SmartProfile.profilegroupsList.Count > 0)
                    //{
                    //    FrontEndGUI.DecodeDataGridViewUsername(utilityviewmodel.ReadingsDataGrid, true);
                    //}
#endif


                    string displayName = "";
                    List<SmartUsers.Consumers> consumers_temp
                        = SmartSpikeV2017.Find_ValidCubefaces(ourviewmodel, SmartParametersV2016.Utility);
                    foreach (SmartUsers.Consumers consumer_row in consumers_temp)
                    {
                        displayName = SmartRoutinesV2018.GetDisplayName(ourviewmodel, consumer_row.USERNAME);

                        // Big test ... but worth it in the long run...
                        // Test 1 Costs table for this resource is empty
                        List<SmartUtility.Resources> resources_found =
                                SmartSpikeUtilityV2017.UtilityFindResourcesList(ourviewmodel,
                                                                                utilityviewmodel,
                                                                                utilityviewmodel.resource_code);
                        if (resources_found.Count > 0)
                        {
                            bool recalculate = false;
                            if (SmartRoutinesV2018.DateTimeCompare(resources_found.First().UNITRATES_UPDATED,
                                                                    utilityviewmodel.unitrates_updatedx) < 0)
                            {
                                recalculate = true;
                            }
                            // Test 2 Readings for this Account have changed
                            if (!recalculate)
                            {
                                if (utilityviewmodel.totalReadings != resources_found.First().TOTAL_USAGE)
                                {
                                    recalculate = true;
                                }
                            }

                            if (recalculate)
                            {
                                switch (utilityviewmodel.resource_code)
                                {
                                    case SmartParametersV2016.Electricity:
                                        utilityviewmodel.Hezbollah.e_analysis_costsList = new List<SmartUtility.AnalysisCosts>();
                                        break;
                                    case SmartParametersV2016.Gas:
                                        utilityviewmodel.Hezbollah.g_analysis_costsList = new List<SmartUtility.AnalysisCosts>();
                                        break;
                                    default:
                                        break;
                                }
                            }
                            utilityviewmodel.already_doneList.Clear();
                            // Single costs
                            if (!await Fill_Costs_Datagrid(ourviewmodel,
                                                            utilityviewmodel,
                                                            utilityviewmodel.area_code,
                                                            utilityviewmodel.resource_code,
                                                            utilityviewmodel.resource_type,
                                                            brand_code,
                                                            supplier_code,
                                                            utilityviewmodel.age,
                                                            utilityviewmodel.already_doneList))
                            {
                                if (!await SmartRoutinesV2018.CheckTrace(ourviewmodel,
                                                                    utilityviewmodel.utilityToken,
                                                                    supplier_code,
                                                                    brand_code,
                                                                    SmartParametersV2016.Utility.ToString() + " " + utilityviewmodel.resourceCodes + " Cannot build Costs grid for: " + utilityviewmodel.area_code))
                                {
                                    return false;
                                }
                                return false;
                            }

                            if (recalculate)
                            {
                                switch (utilityviewmodel.resource_code)
                                {
                                    case SmartParametersV2016.Electricity:
                                        utilityviewmodel.Hezbollah.e_costsList =
                                        new List<SmartUtility.ECosts>(from key in
                                        utilityviewmodel.Hezbollah.e_analysis_costsList
                                                                      select new SmartUtility.ECosts
                                                                      {
                                                                          USERNAME = key.USERNAME,
                                                                          CUBEFACE_CODE = key.CUBEFACE_CODE,
                                                                          RESOURCE_CODE = key.RESOURCE_CODE,
                                                                          UNIQUE_NUMBER = key.UNIQUE_NUMBER,
                                                                          SUPPLIER_CODE = key.SUPPLIER_CODE,
                                                                          BRAND_CODE = key.BRAND_CODE,
                                                                          TARIFF_CODE = key.TARIFF_CODE,
                                                                          TCR = key.TCR,
                                                                          RESOURCE_TYPE = key.RESOURCE_TYPE,
                                                                          PAYMENT_PLAN = Convert.ToChar(key.PAYMENT_PLAN),
                                                                          TOTAL_COST = key.TOTAL_COST
                                                                      });
                                        break;
                                    case SmartParametersV2016.Gas:
                                        utilityviewmodel.Hezbollah.g_costsList =
                                        new List<SmartUtility.GCosts>(from key in
                                        utilityviewmodel.Hezbollah.g_analysis_costsList
                                                                      select new SmartUtility.GCosts
                                                                      {
                                                                          USERNAME = key.USERNAME,
                                                                          CUBEFACE_CODE = key.CUBEFACE_CODE,
                                                                          RESOURCE_CODE = key.RESOURCE_CODE,
                                                                          UNIQUE_NUMBER = key.UNIQUE_NUMBER,
                                                                          SUPPLIER_CODE = key.SUPPLIER_CODE,
                                                                          BRAND_CODE = key.BRAND_CODE,
                                                                          TARIFF_CODE = key.TARIFF_CODE,
                                                                          TCR = key.TCR,
                                                                          RESOURCE_TYPE = key.RESOURCE_TYPE,
                                                                          PAYMENT_PLAN = Convert.ToChar(key.PAYMENT_PLAN),
                                                                          TOTAL_COST = key.TOTAL_COST
                                                                      });
                                        break;
                                    default:
                                        break;
                                }
                                //consumers_utilityview_found.First().SUPPLIER_CODE,
                                //consumers_utilityview_found.First().BRAND_CODE);
                                //consumers_utilityview_found.First().ACCOUNT_CREATED,
                                //consumers_utilityview_found.First().ACCOUNT_NO,
                                //consumers_utilityview_found.First().MPAN_MPRN);
                                //if (!string.IsNullOrEmpty(resources_row.ToString()))
                                //{
                                // Despite everything and all the pain
                                // your program is ABSOLUTELY beautiful Ray, and its ALL YOURS!
                                // Update the Switches and Costs tables

                                resources_found.First().TOTAL_USAGE = utilityviewmodel.totalReadings;
                                resources_found.First().UNITRATES_UPDATED = utilityviewmodel.unitrates_updatedx;
                                resources_found.First().Updated = true;
                                utilityviewmodel.Hezbollah.utility_resources_changesList.Add(resources_found.First());
                                if (!await SmartUtilityScrapeV2022.DoAllSmartUtility(ourviewmodel,
                                                                                    utilityviewmodel,
                                                                                    false,
                                                                                    SmartParametersV2016.sqliteformat,
                                                                                    supplier_code,
                                                                                    brand_code))
                                {
                                    if (!await SmartRoutinesV2018.CheckTrace(ourviewmodel, utilityviewmodel.utilityToken, 0, 0, ourviewmodel.errorMessage))
                                    {
                                        return false;
                                    }
                                    //  Led8 turned to Red on return
                                    return false;
                                }
                                //consumers_utilityview_found.First().TOTAL_USAGE = total_readings;
                                //consumers_utilityview_found.First().UNITRATES_UPDATED = utilityviewmodel.unitrates_updatedx;
                            }
                        }

                        // Well ... the resource_code should never ever be set to ' ' (space)!!!
                        // so this following line would never ever be true!  I have set it to DualFuel
                        // which is what I think it should be ...  (we'll see!!)
                        if (utilityviewmodel.resource_code == SmartParametersV2016.DualFuel) //  defaultResourceCode)
                        {
                            Combine_CostsLists(utilityviewmodel.resource_code, utilityviewmodel);
                        }

                        List<SmartUtility.AnalysisCostsView> temp = SmartAnalyzeV2016.Fill_Analysis_Costs(ourviewmodel,
                                                                                    utilityviewmodel,
                                                                                    utilityviewmodel.resource_code,
                                                                                    displayName);
                        utilityviewmodel.UtilityCosts = temp;

                        if (utilityviewmodel.UtilityCosts.Count > 0)
                        {
                            // We have something in the Username field, but
                            // do we display it??
#if WPF
                            if (ourviewmodel.SmartProfile.profilegroupsList.Count > 0)
                            {
                                // Show Costs Username
                                FrontEndGUI.DecodeDataGridViewUsername(utilityviewmodel.CostsDataGrid, true);
                            }
#endif
                        }
                    }

                    // Start off with the Engine Dates
                    utilityviewmodel.DateTimeStartBrush = SmartRoutinesV2018.WhiteBackgroundDate(utilityviewmodel.DateTimeStartBrush, utilityviewmodel.DateTimeStartBrush, ourviewmodel);
                    utilityviewmodel.DateTimeEndBrush = SmartRoutinesV2018.WhiteBackgroundDate(utilityviewmodel.DateTimeEndBrush, utilityviewmodel.DateTimeEndBrush, ourviewmodel);

                    //switch (utilityviewmodel.resource_code)
                    //{
                    //    case SmartParametersV2016.Electricity:
                    //        // This needs to be the Engine From and To Dates
                    //        //utilityviewmodel.StartDate = utilityviewmodel.e_engine_dates[0];
                    //        //utilityviewmodel.EndDate = utilityviewmodel.e_engine_dates[1];
                    //        break;
                    //    case SmartParametersV2016.Gas:
                    //        // This needs to be the Engine From and To Dates
                    //        //utilityviewmodel.StartDate = utilityviewmodel.g_engine_dates[0];
                    //        //utilityviewmodel.EndDate = utilityviewmodel.g_engine_dates[1];
                    //        break;
                    //    case SmartParametersV2016.DualFuel:
                    //        // This needs to be the Engine From and To Dates
                    //        // Find a band where one is within the other? Fingers crossed ....
                    //        //utilityviewmodel.StartDate = (utilityviewmodel.e_engine_dates[0] > utilityviewmodel.g_engine_dates[0] ? utilityviewmodel.e_engine_dates[0] : utilityviewmodel.g_engine_dates[0]);
                    //        //utilityviewmodel.EndDate = (utilityviewmodel.e_engine_dates[1] < utilityviewmodel.g_engine_dates[1] ? utilityviewmodel.e_engine_dates[1] : utilityviewmodel.g_engine_dates[1]);
                    //        break;
                    //    default:
                    //        break;
                    //}

                    utilityviewmodel.charts_supplier_code = supplier_code;
                    utilityviewmodel.charts_brand_code = brand_code;

                    if (!await BuildCostsChart(
#if WINFORMS
                                            process_components,
#endif
                                            ourviewmodel, utilityviewmodel))
                    {
                        return false;
                    }
#if SMARTMAUI
                    utilityviewmodel.CostsVisible = true;
#endif
                    BuildReadingsChart(
#if WINFORMS
                                            process_components,
#endif

                                            ourviewmodel, utilityviewmodel);
                    BuildUsageChart(
#if WINFORMS
                                            process_components,
#endif

                                            ourviewmodel, utilityviewmodel);

                    // Has to come after the Charts as its the middle one which
                    // Fills up the Breakdown table
                    utilityviewmodel.UtilityBreakdown = Fill_Breakdown(ourviewmodel,
                                                                        utilityviewmodel,
                                                                        //utilityviewmodel.utilityDisplayCulture.DateTimeFormat.ShortDatePattern,
                                                                        //SmartParametersV2016.format2Places,
                                                                        SmartParametersV2016.format3Places);

                    if (utilityviewmodel.UtilityBreakdown.Count > 0)
                    {
                        // We have something in the Username field, but
                        // do we display it??
#if WPF
                        if (ourviewmodel.SmartProfile.profilegroupsList.Count > 0)
                        {
                            FrontEndGUI.DecodeDataGridViewUsername(utilityviewmodel.BreakdownDataGrid, true);
                        }
#endif
                    }
                    // Costs and Readings
#if WINFORMS || WPF  || WINUI || SMARTMAUI
                    utilityviewmodel.TotalkWh = string.Format(utilityviewmodel.CultureINF, utilityviewmodel.meterFormat, utilityviewmodel.totalReadings);
                    utilityviewmodel.TotalBilled = DisplayValue(ourviewmodel,
                                                                utilityviewmodel,
                                                                utilityviewmodel.ConvertToSymbol,
                                                                ourviewmodel.exchangeRate,
                                                                (decimal)(-utilityviewmodel.billed_amount / 100.0));
                    utilityviewmodel.ProjectedCost = DisplayValue(ourviewmodel,
                                                                utilityviewmodel,
                                                                utilityviewmodel.ConvertToSymbol,
                                                                ourviewmodel.exchangeRate,
                                                                (decimal)(utilityviewmodel.projectedCost / 100.0));
                    utilityviewmodel.TotalCost = DisplayValue(ourviewmodel,
                                                                utilityviewmodel,
                                                                utilityviewmodel.ConvertToSymbol,
                                                                ourviewmodel.exchangeRate,
                                                                (decimal)(utilityviewmodel.payments_made / 100.0));
                    utilityviewmodel.TotalReadings = utilityviewmodel.totalReadings.ToString(utilityviewmodel.CultureINF);
#endif
#if ANDROIDX
                    utilityviewmodel.TotalkWh.Text = string.Format(utilityviewmodel.CultureINF, utilityviewmodel.meterFormat, utilityviewmodel.totalReadings);
                    utilityviewmodel.TotalBilled.Text = DisplayValue(ourviewmodel,
                                                                utilityviewmodel,
                                                                utilityviewmodel.ConvertToSymbol,
                                                                ourviewmodel.exchangeRate,
                                                                (decimal)(-utilityviewmodel.billed_amount / 100.0));
                    utilityviewmodel.ProjectedCost.Text = DisplayValue(ourviewmodel,
                                                                utilityviewmodel,
                                                                utilityviewmodel.ConvertToSymbol,
                                                                ourviewmodel.exchangeRate,
                                                                (decimal)(utilityviewmodel.projectedCost / 100.0));
                    utilityviewmodel.TotalCost.Text = DisplayValue(ourviewmodel,
                                                                utilityviewmodel,
                                                                utilityviewmodel.ConvertToSymbol,
                                                                ourviewmodel.exchangeRate,
                                                                (decimal)(utilityviewmodel.payments_made / 100.0));
                    utilityviewmodel.TotalReadings.Text = utilityviewmodel.totalReadings.ToString(utilityviewmodel.CultureINF);
#endif
                }
            }
            return true;
        }


        internal static async Task<bool> BuildCostsChart(
#if WINFORMS
                                                        MainProcess components,
#endif
                                                        MainViewModel ourviewmodel, 
                                                        UtilityViewModel utilityviewmodel)
        {
#if SMARTMAUI
            utilityviewmodel.ChartSeries1 = (await SmartLiveCharts2.CreateLineSeries("", ourviewmodel, utilityviewmodel)).ToArray();
#else

            utilityviewmodel.PlotModel1 = await SmartChartsV2016.CreateLineSeries("", ourviewmodel, utilityviewmodel);
#endif
#if WINFORMS
            components.UtilityPlotView1.Model = utilityviewmodel.PlotModel1;
#endif
            return true;
        }
        
        internal static void BuildReadingsChart(
#if WINFORMS
                                                MainProcess components,
#endif
                                                MainViewModel ourviewmodel,
                                                UtilityViewModel utilityviewmodel)
        {
#if SMARTMAUI
            utilityviewmodel.ChartSeries2 = SmartLiveCharts2.CreateReadingsSeries("", ourviewmodel, utilityviewmodel);
#else
            utilityviewmodel.PlotModel2 = SmartChartsV2016.CreateReadingsSeries("",ourviewmodel,utilityviewmodel);
#endif
#if WINFORMS
            components.UtilityPlotView2.Model = utilityviewmodel.PlotModel2;
#endif
            return;
        }
        
        internal static void BuildUsageChart(
#if WINFORMS
                                             MainProcess components,
#endif
                                            MainViewModel ourviewmodel,
                                            UtilityViewModel utilityviewmodel)
        {
#if SMARTMAUI
            utilityviewmodel.ChartSeries3 = SmartLiveCharts2.CreateUsageSeries("", ourviewmodel, utilityviewmodel);
#else
            utilityviewmodel.PlotModel3 = SmartChartsV2016.CreateUsageSeries("", ourviewmodel, utilityviewmodel);
#endif
#if WINFORMS
            components.UtilityPlotView3.Model = utilityviewmodel.PlotModel3;
#endif
            return;
        }
        
#if WINFORMS
        internal static bool Check_Economy(
#endif
#if WPF  || WINUI || SMARTMAUI
        internal static Visibility Check_Economy(
#endif
#if ANDROIDX
        internal static bool Check_Economy(
#endif
                                            string resource_type)
        {
            if (resource_type == "VR")
            {
#if WINFORMS
                return true;
#endif
#if WPF  || WINUI || SMARTMAUI
                return Visibility.Visible;
#endif
#if ANDROIDX
                return true;
#endif
            }
            else
            {
#if WINFORMS
                return false;
#endif
#if WPF || SMARTMAUI
                return Visibility.Hidden;
#endif
#if WINUI
                return Visibility.Collapsed;
#endif
#if ANDROIDX
                return false;
#endif
            }
        }

        internal static async Task<bool> Build_Rates_Async(
#if ANDROIDX
                                                            AppCompatActivity meterActivity,
#endif
                                                            MainViewModel ourviewmodel,
                                                            UtilityViewModel utilityviewmodel,
                                                            char resource_code,
                                                            int tariff_code,
                                                            char payment_plan,
                                                            string resource_type)
        {
            // Rates - these are all keys into LOAD_UNIT_RATES procedure so we might as well check 'em now ..
            if ((tariff_code != 0) &&
                (payment_plan != SmartParametersV2016.defaultChar) &&
                (!string.IsNullOrEmpty(resource_type)) &&
                (utilityviewmodel.area_code > 0) &&
                (resource_code != SmartParametersV2016.defaultResourceCode))
            {
                // PRICES can be fixed now there is a good AREA_CODE
                // The 'DISTINCT' keyword is used because some SUPPLIER_CODES are common
                // across BRANDS, so the Tariffs are the same?

                // But you don't need DISTINCT if you don't JOIN on SUPPLIERS
                // So its quicker??
                // Build rates

                // NewER version - store current withdrawal date
                DateTime comparison_date = utilityviewmodel.withdrawn_date;
                //DateTime latest_date = comparison_date;
                switch (resource_code)
                {
                    case SmartParametersV2016.Electricity:
                        if (utilityviewmodel.Hezbollah.e_unit_ratesList.Count == 0)
                        {

                            string load_unit_rates = SmartParametersV2016.Utility.ToString() + SmartParametersV2016.unitSeparator +
                                            utilityviewmodel.area_code.ToString() + SmartParametersV2016.unitSeparator +
                                            resource_code.ToString() + SmartParametersV2016.unitSeparator +
                                            resource_type + SmartParametersV2016.unitSeparator +
                                            comparison_date.ToString(SmartParametersV2016.sqliteformat);
                            // Tell the console we have switched
                            await SmartRoutinesV2018.TextBlockUpdate(
#if ANDROIDX
                                                             meterActivity,
#endif
                                                            ourviewmodel, "Requesting LOAD_UNIT_RATES: " + utilityviewmodel.resource_code);
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
                            // Tell the console we have switched
                            await SmartRoutinesV2018.TextBlockUpdate(
#if ANDROIDX
                                                            meterActivity,
#endif
                                                            ourviewmodel, "Completed LOAD_UNIT_RATES: " + utilityviewmodel.resource_code);
                        }
                        break;
                    case SmartParametersV2016.Gas:
                        if (utilityviewmodel.Hezbollah.g_unit_ratesList.Count == 0)
                        {
                            string load_gas_unit_rates = SmartParametersV2016.Utility.ToString() + SmartParametersV2016.unitSeparator +
                                                utilityviewmodel.area_code.ToString() + SmartParametersV2016.unitSeparator +
                                                resource_code.ToString() + SmartParametersV2016.unitSeparator +
                                                resource_type + SmartParametersV2016.unitSeparator +
                                                comparison_date.ToString(SmartParametersV2016.sqliteformat);
                            // Tell the console we have switched
                            await SmartRoutinesV2018.TextBlockUpdate(
#if ANDROIDX
                                                             meterActivity,
#endif
                                                             ourviewmodel, "Requesting LOAD_UNIT_RATES: " + utilityviewmodel.resource_code);

                            utilityviewmodel.Hezbollah.g_unit_ratesList = await SmartBobV2017.Load_SingleList_Async<SmartUtility.UnitRates>(ourviewmodel,
                                                                                                                utilityviewmodel,
                                                                                                                utilityviewmodel.utilityToken,
                                                                                                                "SMARTUTILITY",
                                                                                                                "LOAD_UNIT_RATES",
                                                                                                                "P",
                                                                                                                load_gas_unit_rates);

                            if (!string.IsNullOrEmpty(ourviewmodel.errorMessage))
                            {
                                return false;
                            }
                            // Tell the console we have switched
                            await SmartRoutinesV2018.TextBlockUpdate(
#if ANDROIDX
                                                             meterActivity,
#endif
                                                            ourviewmodel, "Completed LOAD_UNIT_RATES: " + utilityviewmodel.resource_code);
                        }
                        break;
                    case SmartParametersV2016.DualFuel:
                        if (utilityviewmodel.Hezbollah.e_unit_ratesList.Count == 0)
                        {
                            string load_elec_unit_rates = SmartParametersV2016.Utility.ToString() + SmartParametersV2016.unitSeparator +
                                            utilityviewmodel.area_code.ToString() + SmartParametersV2016.unitSeparator +
                                            SmartParametersV2016.Electricity.ToString() + SmartParametersV2016.unitSeparator +
                                            resource_type + SmartParametersV2016.unitSeparator +
                                            comparison_date.ToString(SmartParametersV2016.sqliteformat);

                            // Tell the console we have switched
                            await SmartRoutinesV2018.TextBlockUpdate(
#if ANDROIDX
                                                            meterActivity,
#endif
                                                            ourviewmodel, "Requesting LOAD_UNIT_RATES: " + SmartParametersV2016.Electricity);
                            utilityviewmodel.Hezbollah.e_unit_ratesList = await SmartBobV2017.Load_SingleList_Async<SmartUtility.UnitRates>(ourviewmodel,
                                                                                                                utilityviewmodel,
                                                                                                                utilityviewmodel.utilityToken,
                                                                                                                "SMARTUTILITY",
                                                                                                                "LOAD_UNIT_RATES",
                                                                                                                "P",
                                                                                                                load_elec_unit_rates);

                            if (!string.IsNullOrEmpty(ourviewmodel.errorMessage))
                            {
                                return false;
                            }
                            // Tell the console we have switched
                            await SmartRoutinesV2018.TextBlockUpdate(
#if ANDROIDX
                                                             meterActivity,
#endif
                                                            ourviewmodel, "Completed LOAD_UNIT_RATES: " + SmartParametersV2016.Electricity);
                        }
                        if (utilityviewmodel.Hezbollah.g_unit_ratesList.Count == 0)
                        {
                            string load_gas_unit_rates = SmartParametersV2016.Utility.ToString() + SmartParametersV2016.unitSeparator +
                                            utilityviewmodel.area_code.ToString() + SmartParametersV2016.unitSeparator +
                                            SmartParametersV2016.Gas.ToString() + SmartParametersV2016.unitSeparator +
                                            resource_type + SmartParametersV2016.unitSeparator +
                                            comparison_date.ToString(SmartParametersV2016.sqliteformat);
                            // Tell the console we have switched
                            await SmartRoutinesV2018.TextBlockUpdate(
#if ANDROIDX
                                                             meterActivity,
#endif
                                                            ourviewmodel, "Requesting LOAD_UNIT_RATES: " + SmartParametersV2016.Gas);

                            utilityviewmodel.Hezbollah.g_unit_ratesList = await SmartBobV2017.Load_SingleList_Async<SmartUtility.UnitRates>(ourviewmodel,
                                                                                                                utilityviewmodel,
                                                                                                                utilityviewmodel.utilityToken,
                                                                                                                "SMARTUTILITY",
                                                                                                                "LOAD_UNIT_RATES",
                                                                                                                "P",
                                                                                                                load_gas_unit_rates);

                            if (!string.IsNullOrEmpty(ourviewmodel.errorMessage))
                            {
                                return false;
                            }
                            // Tell the console we have switched
                            await SmartRoutinesV2018.TextBlockUpdate(
#if ANDROIDX
                                                             meterActivity,
#endif
                                                            ourviewmodel, "Completed LOAD_UNIT_RATES: " + SmartParametersV2016.Gas);
                        }
                        break;
                    default:
                        break;
                }
                // In case the date returned is earlier??  Shouldn't ever occur
                if (SmartRoutinesV2018.DateTimeCompare(comparison_date, utilityviewmodel.withdrawn_date) > 0)
                {
                    utilityviewmodel.withdrawn_date = comparison_date; // ?? Shouldn't ever occur??
                }
                return true;
            }
            return false;
        }

#if WINFORMS || WPF  || WINUI || SMARTMAUI
        internal static string Build_Utility_Dropdowns(
#endif
#if ANDROIDX
        internal static string Build_Utility_Dropdowns(
#endif
#if WINFORMS
                                                                    MainProcess components,
#endif
                                                                    MainViewModel ourviewmodel,
                                                                    UtilityViewModel utilityviewmodel)
        {

            string urgent_message = "";
            // There is no resource_type passed into this routine ... Yes there is

            string target_supplier_name = "";
            foreach (SmartUtility.Accounts account_row in utilityviewmodel.Hezbollah.utility_accountsList)
            {
                target_supplier_name = SmartSpikeUtilityV2017.Utility_Lookup_SupplierName(ourviewmodel,
                                                    utilityviewmodel,
                                                    account_row.SUPPLIER_CODE,
                                                    account_row.BRAND_CODE);
                break;
            }

            // We are doing Institutions within Areas and we need to do Areas within Suppliers
            // (because we may have a Supplier with no Areas, but we will never have an Area with no Suppliers)
            List<UtilityViewModel.SupplierItem> temp_suppliers = new List<UtilityViewModel.SupplierItem>();

            List<SmartUtility.Brands> brands_found = new List<SmartUtility.Brands>();
            if (utilityviewmodel.Hezbollah.suppliersList.Count > 0)
            {
                int supplier_id = -1;   // So that Supplier Ids start from 0
                // Get 'em all - institution_code is only used to match the return index
                //brand_expression = "BRAND_code LIKE '%'";

                // Check this  ?????  its supposed to be the same as LIKE %
                brands_found = SmartSpikeUtilityV2017.Utility_Lookup_Brands(ourviewmodel,
                                                            utilityviewmodel,
                                                            utilityviewmodel.resource_code,
                                                            utilityviewmodel.resource_type,
                                                            SmartParametersV2016.activeFlag,
                                                            0,
                                                            0);
                if (brands_found.Count == 0)
                {
                    utilityviewmodel.UtilitySuppliersList = new List<UtilityViewModel.SupplierItem>();
                }
                else
                {
                    foreach (SmartUtility.Brands brand_row in brands_found)
                    {
                        string supplier_name = brand_row.BRAND_NAME;
                        // First Big Test

#if WINFORMS
                        Color tempColour = ourviewmodel.blackColour;
#endif
#if WPF  || WINUI || SMARTMAUI
                        Brush tempColour = ourviewmodel.blackColour;
#endif
#if ANDROIDX
                        Color tempColour = ourviewmodel.blackColour;
#endif
                        List<SmartUtility.Logins> logins_found = SmartSpikeUtilityV2017.Utility_Lookup_Logins(ourviewmodel,
                                                                                                        utilityviewmodel,
                                                                                                        brand_row.SUPPLIER_CODE,
                                                                                                        brand_row.BRAND_CODE);
                        string header = "";
                        if (logins_found.Count > 0)
                        {
                            tempColour = ourviewmodel.greenColour;
#if WINFORMS
                            if (!string.IsNullOrEmpty(logins_found.First().USER_ID) &&
                                !string.IsNullOrEmpty(logins_found.First().USER_PASSWORD))
                            {
                                // Because in Windows WINUI our chimp developed picker cannot show Colours, we have to resort
                                // to this crude indication

                                header = SmartParametersV2016.defaultgreen.ToString();
                            }
#endif
                        }

                        UtilityViewModel.SupplierItem supplier_item = new UtilityViewModel.SupplierItem()
                        {
                            Supplier_ID = supplier_id + 1,
                            Content = header + brand_row.BRAND_NAME,
                            Value = Build_Tag_Supplier(brand_row.SUPPLIER_CODE, brand_row.BRAND_CODE),
                            Colour = tempColour
                        };

                        if (supplier_name == target_supplier_name)
                        {
                            utilityviewmodel.SupplierSelectedIndex = supplier_item.Supplier_ID;
                        }
                        // She is a MINE of mangled distortions, distractions, muddled facts, mis-represntations
                        // and mis-information regarding names
                        // dates, times, places peopla and events AND all of them put together!

                        temp_suppliers.Add(supplier_item);
                        supplier_id++;
                    }
                }
                // There is a selected Institution which is the first
                // You can choose any of them by double-clicking to set up or change the connection info 
                utilityviewmodel.UtilitySuppliersList = temp_suppliers;
                if (utilityviewmodel.UtilitySuppliersList.Count > 0)
                {
                    // How do you find your current Supplier?  
                    // Its the one who has control of your Meter (Elect or Gas)
                    // You could have TWO Current Suppliers - one for E and one for Gas
                    // So your supplier could change if you switch resources ....
                    // Lets have a look in SmartSpike this evening for Meter selects ...                    
                    if (utilityviewmodel.SupplierSelectedIndex != -1)
                    {
#if WINFORMS
                        if (components.comboBoxGUIUtilitySuppliers.Items.Count > 0)
                        {
                            components.comboBoxGUIUtilitySuppliers.SelectedIndex = utilityviewmodel.SupplierSelectedIndex;
                            components.comboBoxGUIUtilitySuppliers.SelectedIndexChanged += (s, e) => components.comboBoxUtilitySuppliers_SelectedIndexChanged(s, e, ourviewmodel, utilityviewmodel);
                        }
#endif
#if ANDROIDX

                        //await UtilityView.SuppliersStartUp_Actual();
#endif
                    }
                }
            }
            return urgent_message;
        }

        internal static bool Rebuild_USD(MainViewModel ourviewmodel,
                                                            UtilityViewModel utilityviewmodel,
                                                            string resource_type,
                                                            char activeFlag,
                                                            short brand_code)
        {
            // There is no resource_type passed into this routine ...
            // It is UNHEARD OF that any Supplier JUST does Economy7!!!
            // They might do Gas or they might do Electricity without E7,
            // but none of them do Electricity with ONLY E7!!
            int brand_index = -1;

            // We are doing Suppliers within Areas and we need to do Areas within Suppliers
            // (because we may have a Supplier with no Areas, but we will never have an Area with no Suppliers)

            utilityviewmodel.UtilitySuppliersList.Clear();

            // Not sure why this has just started working ...?
            if (utilityviewmodel.Hezbollah.suppliersList.Count > 0)
            {
                // Get 'em all - supplier_code is only used to match the return index
                //brand_expression = "BRAND_code LIKE '%'";

                // Check this  ?????  its supposed to be the same as LIKE %
                List<SmartUtility.Brands> brand_found = SmartSpikeUtilityV2017.Utility_Lookup_Brands(ourviewmodel,
                                                                                        utilityviewmodel,
                                                                                        utilityviewmodel.resource_code,
                                                                                        resource_type,
                                                                                        activeFlag,
                                                                                        0, //supplier_code,
                                                                                        0); //brand_code,

                Locate_Utility_Supplier_Entry(ourviewmodel,
                                        utilityviewmodel,
                                        utilityviewmodel.resource_code,
                                        brand_code,
                                        brand_found);
            }
            if (SmartRoutinesV2018.Check_These_Two_Determine_UDPRN(utilityviewmodel, utilityviewmodel.area_code, brand_code, brand_index))
            {
                // utility_brand_index is now set 
                return true;
            }
            return false;
        }

        internal static void Locate_Utility_Supplier_Entry(MainViewModel ourviewmodel,
                                                            UtilityViewModel utilityviewmodel,
                                                            char resource_code,
                                                            short brand_code,
                                                            List<SmartUtility.Brands> brand_found)
        {
            int loop = 0;

            List<UtilityViewModel.SupplierItem> temp_suppliers = new List<UtilityViewModel.SupplierItem>();

            if (brand_found.Count > 0)
            {
                List<SmartUtility.AnalysisCosts> analysis_costs_found = new List<SmartUtility.AnalysisCosts>();
                switch (resource_code)
                {
                    case SmartParametersV2016.Electricity:
                        analysis_costs_found = utilityviewmodel.Hezbollah.e_analysis_costsList;
                        break;
                    case SmartParametersV2016.Gas:
                        analysis_costs_found = utilityviewmodel.Hezbollah.g_analysis_costsList;
                        break;
                    case SmartParametersV2016.DualFuel:
                        // Need a combination here ...
                        analysis_costs_found = utilityviewmodel.Hezbollah.d_analysis_costsList;
                        break;
                    default:
                        break;
                }
                int analysis_costs_count = analysis_costs_found.Count - 1;

                string CHEAPEST_BRAND_NAME = "",
                    DEAREST_BRAND_NAME = "";

                if (analysis_costs_count > 0)
                {
                    if (utilityviewmodel.area_code == 0)
                    {
                        utilityviewmodel.utility_brand_index = loop;
                    }
                    CHEAPEST_BRAND_NAME = SmartSpikeUtilityV2017.Utility_Lookup_SupplierName(ourviewmodel,
                                                                                utilityviewmodel,
                                                                                analysis_costs_found.First().SUPPLIER_CODE,
                                                                                analysis_costs_found.First().BRAND_CODE);

                    DEAREST_BRAND_NAME = SmartSpikeUtilityV2017.Utility_Lookup_SupplierName(ourviewmodel,
                                                                            utilityviewmodel,
                                                                            analysis_costs_found.Last().SUPPLIER_CODE,
                                                                            analysis_costs_found.Last().BRAND_CODE);
                }

                foreach (SmartUtility.Brands brand_row in brand_found)
                {
                    // The Supplier(s) exist(s) but what if we required those
                    // for a specific Area?
                    if (utilityviewmodel.Hezbollah.brand_matrixList.Count > 0)
                    {
                        FieldInfo[] myFields = typeof(SmartUtility.BrandMatrix).GetFields(SmartParametersV2016.bindingFlags);

                        utilityviewmodel.brand_code_temp = 0;
                        if (Check_Utility_Supplier_Area(ourviewmodel,
                                                utilityviewmodel,
                                                brand_row,
                                                myFields))
                        {
                            
                            // Area_Code = "00" so we need all Suppliers, regardless
                            // OR the Supplier is in the Area if this parameter is passed in

                            // Do we have any tariffs for this Area for this Brand or Supplier?
                            // Why don't we check for this?  Well, lets take Daligas which is a gas
                            // supplier only.  We build the 'E' dropdown to include Daligas because
                            // historically, Daligas MAY have supplied electricity, stopped and just
                            // supplied Gas for several years and thenstarted supplying electricity
                            // again. (It doesn't, but this is a hypothetical scenario).  So in this
                            // hypothetical case, how would we ever know whether to include it in the
                            // dropdown or not?  If we exclude it, then we might not see historical
                            // tariffs it has, if we include it - and it has no tariffs for electricity
                            // then we won't see anything in the tariffs dropdown... and that is NOT
                            // a big deal.  It just means 'no tariffs for Daligas under electricity.

                            UtilityViewModel.SupplierItem supplier_item = new UtilityViewModel.SupplierItem()
                            {
                                Content = brand_row.BRAND_NAME,
                                Value = "",
                                Colour = ourviewmodel.blackColour
                            };

                            if (analysis_costs_count > 0)
                            {
                                if (brand_row.BRAND_NAME == CHEAPEST_BRAND_NAME)
                                {
#if WINFORMS
                                    // supplier_item.Content = SmartParametersV2016.defaultgreen.ToString() + supplier_item.Content;
#endif
                                    supplier_item.Colour = ourviewmodel.greenColour;
                                }
                                else
                                {
                                    if (brand_row.BRAND_NAME == DEAREST_BRAND_NAME)
                                    {
#if WINFORMS
                                        // supplier_item.Content = supplier_item.Content + SmartParametersV2016.defaultred;
#endif
                                        supplier_item.Colour = ourviewmodel.redColour;
                                    }
                                }
                            }

                            temp_suppliers.Add(supplier_item);

                            // WOW!  I never thought THIS FUCKER would work first time!!!!!
                            if (brand_row.BRAND_CODE == brand_code)
                            {
                                utilityviewmodel.utility_brand_index = loop;
                            }
                            loop++;
                            
                        }
                    }
                }
                utilityviewmodel.UtilitySuppliersList = temp_suppliers;
            }
            return;
        }

        internal static void Fill_Rates(MainViewModel ourviewmodel,
                                        UtilityViewModel utilityviewmodel,
                                        char resource_code,
                                        short supplier_code,
                                        int tariff_code,
                                        char payment_plan,
                                        string resource_type)
        {
            switch (resource_code)
            {
                case SmartParametersV2016.Electricity:
                    if (resource_type == "VR")
                    {
                        utilityviewmodel.DayRate = "Day Rate";         // The labels not the rates themselves
                        utilityviewmodel.NightRate = "Night Rate";     // The labels not the rates themselves
                    }
                    else
                    {
                        utilityviewmodel.DayRate = "Day Rate1";    // The labels not the rates themselves
                        utilityviewmodel.NightRate = "Day Rate2"; // The labels not the rates themselves
                    }
                    // Fix em in
                    utilityviewmodel.DayRateBrush = utilityviewmodel.DayRate1Brush;
                    utilityviewmodel.NightRateBrush = utilityviewmodel.NightRate1Brush;
                    utilityviewmodel.StandingChargeBrush = utilityviewmodel.StandingCharge1Brush;
#if WINFORMS || WPF  || WINUI || SMARTMAUI
                    utilityviewmodel.DayRate = utilityviewmodel.DayRate1;
                    utilityviewmodel.NightRate = utilityviewmodel.NightRate1;
                    utilityviewmodel.StandingCharge = utilityviewmodel.StandingCharge1;
#endif
#if ANDROIDX
                    utilityviewmodel.DayRate = utilityviewmodel.DayRate1.Text;
                    utilityviewmodel.NightRate = utilityviewmodel.NightRate1.Text;
                    utilityviewmodel.StandingCharge = utilityviewmodel.StandingCharge1.Text;
#endif
                    Lookup_Prices(ourviewmodel,
                                    utilityviewmodel,
                                    resource_code,
                                    supplier_code,
                                    tariff_code,
                                    payment_plan,
                                    resource_type);
                    // Fix em out
                    utilityviewmodel.DayRate1Brush = utilityviewmodel.DayRateBrush;
                    utilityviewmodel.NightRate1Brush = utilityviewmodel.NightRateBrush;
                    utilityviewmodel.StandingCharge1Brush = utilityviewmodel.StandingChargeBrush;
#if WINFORMS || WPF  || WINUI || SMARTMAUI
                    utilityviewmodel.DayRate1 = utilityviewmodel.DayRate;
                    utilityviewmodel.NightRate1 = utilityviewmodel.NightRate;
                    utilityviewmodel.StandingCharge1 = utilityviewmodel.StandingCharge;
#endif
#if ANDROIDX
                    utilityviewmodel.DayRate1.Text = utilityviewmodel.DayRate;
                    utilityviewmodel.NightRate1.Text = utilityviewmodel.NightRate;
                    utilityviewmodel.StandingCharge1.Text = utilityviewmodel.StandingCharge;
#endif
                    break;

                case SmartParametersV2016.Gas:

                    utilityviewmodel.DayRate = "Day Rate1";
                    utilityviewmodel.NightRate = "Day Rate2";    // No night rates for Gas

                    // Fix em in
                    utilityviewmodel.DayRateBrush = utilityviewmodel.DayRate2Brush;
                    utilityviewmodel.NightRateBrush = utilityviewmodel.NightRate2Brush;
                    utilityviewmodel.StandingChargeBrush = utilityviewmodel.StandingCharge2Brush;
#if WINFORMS || WPF  || WINUI || SMARTMAUI
                    utilityviewmodel.DayRate = utilityviewmodel.DayRate2;
                    utilityviewmodel.NightRate = utilityviewmodel.NightRate2;
                    utilityviewmodel.StandingCharge = utilityviewmodel.StandingCharge2;
#endif
#if ANDROIDX
                    utilityviewmodel.DayRate = utilityviewmodel.DayRate2.Text;
                    utilityviewmodel.NightRate = utilityviewmodel.NightRate2.Text;
                    utilityviewmodel.StandingCharge = utilityviewmodel.StandingCharge2.Text;
#endif
                    Lookup_Prices(ourviewmodel,
                                    utilityviewmodel,
                                    resource_code,
                                    supplier_code,
                                    tariff_code,
                                    payment_plan,
                                    resource_type);
                    // Fix em out
                    utilityviewmodel.DayRate2Brush = utilityviewmodel.DayRateBrush;
                    utilityviewmodel.NightRate2Brush = utilityviewmodel.NightRateBrush;
                    utilityviewmodel.StandingCharge2Brush = utilityviewmodel.StandingChargeBrush;
#if WINFORMS || WPF  || WINUI || SMARTMAUI
                    utilityviewmodel.DayRate2 = utilityviewmodel.DayRate;
                    utilityviewmodel.NightRate2 = utilityviewmodel.NightRate;
                    utilityviewmodel.StandingCharge2 = utilityviewmodel.StandingCharge;
#endif
#if ANDROIDX
                    utilityviewmodel.DayRate2.Text = utilityviewmodel.DayRate;
                    utilityviewmodel.NightRate2.Text = utilityviewmodel.NightRate;
                    utilityviewmodel.StandingCharge2.Text = utilityviewmodel.StandingCharge;
#endif
                    break;
                case SmartParametersV2016.DualFuel:
                    // Do something here for DUal
                    if (resource_type == "VR")
                    {
                        utilityviewmodel.DayRate = "Day Rate";
                        utilityviewmodel.NightRate = "Night Rate";
                    }
                    else
                    {
                        utilityviewmodel.DayRate = "Day Rate1";
                        utilityviewmodel.NightRate = "Day Rate2";
                    }

                    // Fix em in
                    utilityviewmodel.DayRateBrush = utilityviewmodel.DayRate1Brush;
                    utilityviewmodel.NightRateBrush = utilityviewmodel.NightRate1Brush;
                    utilityviewmodel.StandingChargeBrush = utilityviewmodel.StandingCharge1Brush;
#if WINFORMS || WPF  || WINUI || SMARTMAUI
                    utilityviewmodel.DayRate = utilityviewmodel.DayRate1;
                    utilityviewmodel.NightRate = utilityviewmodel.NightRate1;
                    utilityviewmodel.StandingCharge = utilityviewmodel.StandingCharge1;
#endif
#if ANDROIDX
                    utilityviewmodel.DayRate = utilityviewmodel.DayRate1.Text;
                    utilityviewmodel.NightRate = utilityviewmodel.NightRate1.Text;
                    utilityviewmodel.StandingCharge = utilityviewmodel.StandingCharge1.Text;
#endif
                    Lookup_Prices(ourviewmodel,
                                    utilityviewmodel,
                                    SmartParametersV2016.Electricity,
                                    supplier_code,
                                    tariff_code,
                                    payment_plan,
                                    resource_type);
                    // Fix em out
                    utilityviewmodel.DayRate1Brush = utilityviewmodel.DayRateBrush;
                    utilityviewmodel.NightRate1Brush = utilityviewmodel.NightRateBrush;
                    utilityviewmodel.StandingCharge1Brush = utilityviewmodel.StandingChargeBrush;
#if WINFORMS || WPF  || WINUI || SMARTMAUI
                    utilityviewmodel.DayRate1 = utilityviewmodel.DayRate;
                    utilityviewmodel.NightRate1 = utilityviewmodel.NightRate;
                    utilityviewmodel.StandingCharge1 = utilityviewmodel.StandingCharge;
#endif
#if ANDROIDX
                    utilityviewmodel.DayRate1.Text = utilityviewmodel.DayRate;
                    utilityviewmodel.NightRate1.Text = utilityviewmodel.NightRate;
                    utilityviewmodel.StandingCharge1.Text = utilityviewmodel.StandingCharge;
#endif

                    // Fix em in
                    utilityviewmodel.DayRateBrush = utilityviewmodel.DayRate2Brush;
                    utilityviewmodel.NightRateBrush = utilityviewmodel.NightRate2Brush;
                    utilityviewmodel.StandingChargeBrush = utilityviewmodel.StandingCharge2Brush;
#if WINFORMS || WPF  || WINUI || SMARTMAUI
                    utilityviewmodel.DayRate = utilityviewmodel.DayRate2;
                    utilityviewmodel.NightRate = utilityviewmodel.NightRate2;
                    utilityviewmodel.StandingCharge = utilityviewmodel.StandingCharge2;
#endif
#if ANDROIDX
                    utilityviewmodel.DayRate = utilityviewmodel.DayRate2.Text;
                    utilityviewmodel.NightRate = utilityviewmodel.NightRate2.Text;
                    utilityviewmodel.StandingCharge = utilityviewmodel.StandingCharge2.Text;
#endif

                    Lookup_Prices(ourviewmodel,
                                    utilityviewmodel,
                                    SmartParametersV2016.Gas,
                                    supplier_code,
                                    tariff_code,
                                    payment_plan,
                                    resource_type);
                    // Fix em out
                    utilityviewmodel.DayRate2Brush = utilityviewmodel.DayRateBrush;
                    utilityviewmodel.NightRate2Brush = utilityviewmodel.NightRateBrush;
                    utilityviewmodel.StandingCharge2Brush = utilityviewmodel.StandingChargeBrush;
#if WINFORMS || WPF  || WINUI || SMARTMAUI
                    utilityviewmodel.DayRate2 = utilityviewmodel.DayRate;
                    utilityviewmodel.NightRate2 = utilityviewmodel.NightRate;
                    utilityviewmodel.StandingCharge2 = utilityviewmodel.StandingCharge;
#endif
#if ANDROIDX
                    utilityviewmodel.DayRate2.Text = utilityviewmodel.DayRate;
                    utilityviewmodel.NightRate2.Text = utilityviewmodel.NightRate;
                    utilityviewmodel.StandingCharge2.Text = utilityviewmodel.StandingCharge;
#endif
                    break;
                default:
                    break;
            }
            return;
        }

        internal static void Lookup_Prices(MainViewModel ourviewmodel,
                                            UtilityViewModel utilityviewmodel,
                                            char resource_code,
                                            short local_supplier_code,
                                            int local_tariff_code,
                                            char local_payment_plan,
                                            string local_resource_type)
        {
            if (local_supplier_code > 0)
            {
                List<SmartUtility.ConditionsPlans> conditions_plans_found = SmartSpikeUtilityV2017.Utility_ConditionsPlans(ourviewmodel,
                                                                                                    utilityviewmodel,
                                                                                                    local_supplier_code,
                                                                                                    resource_code,
                                                                                                    local_resource_type,
                                                                                                    local_tariff_code,
                                                                                                    local_payment_plan);
                if (conditions_plans_found.Count > 0)
                {
                    foreach (SmartUtility.ConditionsPlans conditions_plans_row in conditions_plans_found)
                    {
                        List<SmartUtility.ConditionsView> conditions_view_found = SmartSpikeUtilityV2017.Utility_ConditionsView(ourviewmodel,
                                                                                                        utilityviewmodel,
                                                                                                        local_supplier_code,
                                                                                                        resource_code,
                                                                                                        local_resource_type,
                                                                                                        local_tariff_code,
                                                                                                        conditions_plans_row.PAYMENT_CODE,
                                                                                                        conditions_plans_row.VERSION_CODE);
                        // We only ever do Tier 1 (SINGLE RATE/STANDING CHARGE), or Two-Tier with this
                        // We don't do > Two-Tier, althought the logic should be able to cope with it
                        // because we use Tier_Level

                        // This should be today's date, as we are (Usually) interested in the latest prices
                        DateTime prices_valid_from = DateTime.Now + ourviewmodel.utcOffset;  // Local time

                        // I have ABSOLUTELY no idea what I am trying to do here!!!
                        // However ... I need to fill in some prices - relative to today - and I
                        // only have three slots to put them in; Day Rate, Night Rate and Standing Charge
                        // For Tier1: Day Rate / Standing Charge or Day Rate : Night Rate : Standing Charge 
                        // I am going to take the FIRST conditions_view_row that comes along!!
                        // (Assumiing they are sorted in descending date order (i.e. latest comes first)


                        bool display_something = false;
                        foreach (SmartUtility.ConditionsView conditions_view_row in conditions_view_found)
                        {
                            // The dates found COULD be in the future ... so we want the first one in the past
                            if (SmartRoutinesV2018.DateTimeCompare(prices_valid_from, conditions_view_row.PRICES_VALID_FROM) >= 0)
                            {
                                display_something = true;
                                short tier_level = 0;
                                short tier_count = 0;
                                int row_count = 0;
                                //for (int loop = lower_limit; loop < upper_limit; loop++)
                                // Only do 1 line or Rates
                                for (int loop = 0; loop < 1; loop++)
                                {
                                    tier_count = conditions_view_row.TIER_COUNT;
                                    tier_level = conditions_view_row.TIER_LEVEL;
                                    // New Rates apply with each Grouping
                                    prices_valid_from = conditions_view_row.PRICES_VALID_FROM;        // Should make it 24-hour format?


                                    short payment_code = conditions_plans_row.PAYMENT_CODE;

                                    List<SmartUtility.UnitRates> columns_found =
                                            SmartSpikeUtilityV2017.Utility_UnitRates(utilityviewmodel,
                                                                            local_supplier_code,
                                                                            resource_code,
                                                                            local_resource_type,
                                                                            local_tariff_code,
                                                                            payment_code,
                                                                            prices_valid_from,
                                                                            tier_count,
                                                                            tier_level,
                                                                            utilityviewmodel.area_code);
                                    if (columns_found.Count == 1)
                                    {
                                        if (local_resource_type == "VR")
                                        {
                                            VR_Row(ourviewmodel,
                                                    utilityviewmodel,
                                                    row_count,
                                                    columns_found);
                                        }
                                        else
                                        {
                                            SR_Row(ourviewmodel,
                                                    utilityviewmodel,
                                                    row_count,
                                                    columns_found);
                                        }
                                    }
                                    row_count++;
                                }
                            }
                        }
                        if (!display_something)
                        {
                            // Nothing was found OR all the prices were in the future
                            // Date was before our range - send back 0.00 for all values
                            // Yes, I know I could write this better, but its logical the way it IS done
                            utilityviewmodel.DayRate = "0.000";
                            utilityviewmodel.DayRateBrush = ourviewmodel.whiteColour;
                            utilityviewmodel.NightRate = "0.000";
                            utilityviewmodel.NightRateBrush = ourviewmodel.whiteColour;
                            utilityviewmodel.StandingCharge = "0.000";
                            utilityviewmodel.StandingChargeBrush = ourviewmodel.whiteColour;
                        }
                        break;  // Only one Condition Plan
                    }
                }
            }
            return;
        }

        internal static void SR_Row(MainViewModel ourviewmodel,
                                UtilityViewModel utilityviewmodel,
                                int row_count,
                                List<SmartUtility.UnitRates> columns_found)
        {
            // Use the first
            switch (row_count)
            {
                case 0:
                    utilityviewmodel.DayRate = columns_found[0].DR;
                    utilityviewmodel.DayRateBrush = ourviewmodel.whiteColour;
                    utilityviewmodel.NightRate = columns_found[0].NR;
                    utilityviewmodel.NightRateBrush = ourviewmodel.whiteColour;
                    utilityviewmodel.StandingCharge = columns_found[0].SC;
                    utilityviewmodel.StandingChargeBrush = ourviewmodel.whiteColour;
                    break;
                case 1:
                    utilityviewmodel.DayRate = "";
                    utilityviewmodel.DayRateBrush = ourviewmodel.whiteColour;
                    utilityviewmodel.NightRate = columns_found[0].DR;
                    utilityviewmodel.NightRateBrush = ourviewmodel.whiteColour;
                    utilityviewmodel.StandingCharge = "";
                    utilityviewmodel.StandingChargeBrush = ourviewmodel.whiteColour;
                    break;
                default:
                    break;
            }
            return;
        }

        internal static void VR_Row(MainViewModel ourviewmodel,
                              UtilityViewModel utilityviewmodel,
                              int row_count,
                                List<SmartUtility.UnitRates> columns_found)
        {
            switch (row_count)
            {
                // Use the first
                case 0:
                    // Day Rate1
                    utilityviewmodel.DayRate = columns_found[0].DR;
                    utilityviewmodel.DayRateBrush = ourviewmodel.whiteColour;
                    utilityviewmodel.NightRate = "";
                    utilityviewmodel.NightRateBrush = ourviewmodel.whiteColour;
                    utilityviewmodel.StandingCharge = columns_found[0].SC;
                    utilityviewmodel.StandingChargeBrush = ourviewmodel.whiteColour;
                    break;
                case 1:
                    // Day Rate2 or Night Rate
                    utilityviewmodel.DayRate = "";
                    utilityviewmodel.DayRateBrush = ourviewmodel.whiteColour;
                    utilityviewmodel.NightRate = columns_found[0].DR;
                    utilityviewmodel.NightRateBrush = ourviewmodel.whiteColour;
                    utilityviewmodel.StandingCharge = "";
                    utilityviewmodel.StandingChargeBrush = ourviewmodel.whiteColour;
                    break;
                default:
                    break;
            }
            return;
        }

        internal static async Task<bool> Fill_Costs_Datagrid(MainViewModel ourviewmodel,
                                                UtilityViewModel utilityviewmodel,
                                                short area_code,
                                                char local_resource_code,
                                                string resource_type,
                                                short brand_code,
                                                short supplier_code,
                                                int age,
                                                List<SmartUtility.AnalysisConditions> already_doneList)
        {
            
            DateTime[] engine_dates = new DateTime[2]
            {
                SmartParametersV2016.defaultDate,
                SmartParametersV2016.defaultDate
            };
            //utilityviewmodel.first_reading = 0.0M;
            //utilityviewmodel.last_reading = 0.0M;
            //utilityviewmodel.total_readings = 0.0M;
            // Tab "Costs"
            // Costs - this isn't perfect ... but I'm gonna go with it
            if ((local_resource_code == SmartParametersV2016.Electricity &&
                utilityviewmodel.Hezbollah.e_unit_ratesList.Count > 0) ||
                (local_resource_code == SmartParametersV2016.Gas &&
                utilityviewmodel.Hezbollah.g_unit_ratesList.Count > 0) ||
                (local_resource_code == SmartParametersV2016.DualFuel &&
                utilityviewmodel.Hezbollah.e_unit_ratesList.Count > 0 &&
                utilityviewmodel.Hezbollah.g_unit_ratesList.Count > 0))
            {
                DateTime engine_from_date = SmartParametersV2016.defaultDate;
                switch (local_resource_code)
                {
                    case SmartParametersV2016.Electricity:
                        if (utilityviewmodel.Hezbollah.e_tariff_engineList.Count == 0)
                        {
                            DateTime e_engine_from_date = DateTime.Now + ourviewmodel.utcOffset; // Local time
                            TimeSpan ts = new TimeSpan(12, 0, 0);
                            e_engine_from_date = e_engine_from_date.Date.Add(ts);

                            SmartEngineV2016.Build_Tariff_EngineList(utilityviewmodel,
                                                                    local_resource_code,
                                                                    e_engine_from_date,
                                                                    SmartParametersV2016.VOLUMECORRECTION,
                                                                    SmartParametersV2016.KWHCONVERSION);
                        }
                        if (utilityviewmodel.Hezbollah.e_tariff_engineList.Count > 0)
                        {
                            engine_from_date = utilityviewmodel.Hezbollah.e_tariff_engineList.First().PERIOD_END;
                            engine_dates[0] = engine_from_date;
                            engine_dates[1] = utilityviewmodel.Hezbollah.e_tariff_engineList.Last().PERIOD_END;
                            utilityviewmodel.e_engine_dates = engine_dates;
                        }

                        break;
                    case SmartParametersV2016.Gas:
                        if (utilityviewmodel.Hezbollah.g_tariff_engineList.Count == 0)
                        {
                            DateTime g_engine_from_date = DateTime.Now + ourviewmodel.utcOffset; // Local time
                            TimeSpan ts = new TimeSpan(12, 0, 0);
                            g_engine_from_date = g_engine_from_date.Date.Add(ts);

                            //DateTime g_EngineToDate = g_engine_from_date;
                            SmartEngineV2016.Build_Tariff_EngineList(utilityviewmodel,
                                                                        local_resource_code,
                                                                        g_engine_from_date,
                                                                        SmartParametersV2016.VOLUMECORRECTION,
                                                                        SmartParametersV2016.KWHCONVERSION);
                        }
                        if (utilityviewmodel.Hezbollah.g_tariff_engineList.Count > 0)
                        {
                            engine_from_date = utilityviewmodel.Hezbollah.g_tariff_engineList.First().PERIOD_END;
                            engine_dates[0] = engine_from_date;
                            engine_dates[1] = utilityviewmodel.Hezbollah.g_tariff_engineList.Last().PERIOD_END;
                            utilityviewmodel.g_engine_dates = engine_dates;
                        }
                        break;
                    case SmartParametersV2016.DualFuel:
                        if (utilityviewmodel.Hezbollah.e_tariff_engineList.Count == 0)
                        {
                            DateTime e_engine_from_date = DateTime.Now + ourviewmodel.utcOffset; // Local time
                            TimeSpan ts = new TimeSpan(12, 0, 0);
                            e_engine_from_date = e_engine_from_date.Date.Add(ts);

                            SmartEngineV2016.Build_Tariff_EngineList(utilityviewmodel,
                                                                    SmartParametersV2016.Electricity,
                                                                    e_engine_from_date,
                                                                    SmartParametersV2016.VOLUMECORRECTION,
                                                                    SmartParametersV2016.KWHCONVERSION);
                        }
                        if (utilityviewmodel.Hezbollah.e_tariff_engineList.Count > 0)
                        {
                            // Take the E dates for the time being for Dual Fuel
                            engine_from_date = utilityviewmodel.Hezbollah.e_tariff_engineList.First().PERIOD_END;
                            engine_dates[0] = engine_from_date;
                            engine_dates[1] = utilityviewmodel.Hezbollah.e_tariff_engineList.Last().PERIOD_END;
                        }
                        utilityviewmodel.e_engine_dates = engine_dates;
                        if (utilityviewmodel.Hezbollah.g_tariff_engineList.Count == 0)
                        {
                            DateTime g_engine_from_date = DateTime.Now + ourviewmodel.utcOffset; // Local time
                            TimeSpan ts = new TimeSpan(12, 0, 0);
                            g_engine_from_date = g_engine_from_date.Date.Add(ts);

                            SmartEngineV2016.Build_Tariff_EngineList(utilityviewmodel,
                                                                        SmartParametersV2016.Gas,
                                                                        g_engine_from_date,
                                                                        SmartParametersV2016.VOLUMECORRECTION,
                                                                        SmartParametersV2016.KWHCONVERSION);
                        }
                        if (utilityviewmodel.Hezbollah.g_tariff_engineList.Count > 0)
                        {
                            // Take the G dates for the time being for Dual Fuel
                            engine_from_date = utilityviewmodel.Hezbollah.g_tariff_engineList.First().PERIOD_END;
                            engine_dates[0] = engine_from_date;
                            engine_dates[1] = utilityviewmodel.Hezbollah.g_tariff_engineList.Last().PERIOD_END;
                        }
                        utilityviewmodel.g_engine_dates = engine_dates;
                        break;
                    default:
                        break;
                }

                short target_supplier_code = 0;             // For debugging
                int target_tariff_code = 0;                 // For debugging
                char target_payment_plan = SmartParametersV2016.defaultChar;  // For debugging

                switch (utilityviewmodel.resource_code)
                {
                    case SmartParametersV2016.Electricity:
                        utilityviewmodel.analysisCost[0] = 0;
                        break;
                    case SmartParametersV2016.Gas:
                        utilityviewmodel.analysisCost[1] = 0;
                        break;
                    default:
                        break;
                }

                switch (local_resource_code)
                {
                    case SmartParametersV2016.Electricity:
                        if (utilityviewmodel.Hezbollah.e_analysis_costsList.Count == 0)
                        {
                            if (!await SmartAnalyzeV2016.Analyze_Costs(ourviewmodel,
                                                            utilityviewmodel,
                                                            true,     // Will delete from AC
                                                            area_code,
                                                            local_resource_code,
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
                                return false;
                            }
                        }
                        break;
                    case SmartParametersV2016.Gas:
                        if (utilityviewmodel.Hezbollah.g_analysis_costsList.Count == 0)
                        {
                            if (!await SmartAnalyzeV2016.Analyze_Costs(ourviewmodel,
                                                            utilityviewmodel,
                                                            true,     // Will delete from AC
                                                            area_code,
                                                            local_resource_code,
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
                                return false;
                            }
                        }
                        break;
                    case SmartParametersV2016.DualFuel:
                        //if (utilityviewmodel.Hezbollah.e_analysis_costsList.Count == 0)
                        //{
                        if (!await SmartAnalyzeV2016.Analyze_Costs(ourviewmodel,
                                                        utilityviewmodel,
                                                        true,     // Will delete from AC
                                                        area_code,
                                                        SmartParametersV2016.Electricity,
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
                            return false;
                        }
                        //}

                        //if (utilityviewmodel.Hezbollah.g_analysis_costsList.Count == 0)
                        //{
                        if (!await SmartAnalyzeV2016.Analyze_Costs(ourviewmodel,
                                                        utilityviewmodel,
                                                        true,     // Will delete from AC
                                                        area_code,
                                                        SmartParametersV2016.Gas,
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
                            return false;
                        }
                        //}
                        break;
                    default:
                        break;


                }

                //if (local_resource_code == SmartParametersV2016.defaultResourceCode)
                //{
                //    Combine_CostsLists(local_resource_code, utilityviewmodel);
                //}
                //utilityviewmodel.UtilityCosts = SmartAnalyzeV2016.Fill_Analysis_Costs(ourviewmodel,
                //                                                utilityviewmodel,
                //                                                local_resource_code,
                //                                                defaultDate);
            }
            return true;
        }

        internal static void Combine_CostsLists(char local_resource_code,
                                                    UtilityViewModel utilityviewmodel)
        {
            List<SmartUtility.AnalysisCosts> analysis_costsList = new List<SmartUtility.AnalysisCosts>(); ;
            List<SmartUtility.AnalysisCosts> opposite_costsList = new List<SmartUtility.AnalysisCosts>();
            switch (local_resource_code)
            {
                case SmartParametersV2016.Electricity:
                    analysis_costsList = utilityviewmodel.Hezbollah.e_analysis_costsList;
                    opposite_costsList = utilityviewmodel.Hezbollah.g_analysis_costsList;
                    break;
                case SmartParametersV2016.Gas:
                    analysis_costsList = utilityviewmodel.Hezbollah.g_analysis_costsList;
                    opposite_costsList = utilityviewmodel.Hezbollah.e_analysis_costsList;
                    break;
                case SmartParametersV2016.DualFuel:
                    analysis_costsList = utilityviewmodel.Hezbollah.e_analysis_costsList;
                    opposite_costsList = utilityviewmodel.Hezbollah.g_analysis_costsList;
                    break;
                default:
                    break;
            }
            utilityviewmodel.Hezbollah.d_analysis_costsList = SmartSpikeUtilityV2017.Utility_Find_DualFuel(opposite_costsList,
                                                                                                    analysis_costsList);
            return;
        }

        internal static List<SmartUtility.AnalysisBillsView> Fill_Bills(MainViewModel ourviewmodel,
                                                                        UtilityViewModel utilityviewmodel)
        {
            // Tab "Bills
            // Does this stop it being 'null' ? Yes
            List<SmartUtility.AnalysisBillsView> analysis_bills_temp = new List<SmartUtility.AnalysisBillsView>();

            utilityviewmodel.analysis_bills_found = new List<SmartUtility.Analysis_Bills>();
            if (SmartBillsV2016.Analyze_Bills(ourviewmodel,
                                                utilityviewmodel))
            {
                if (utilityviewmodel.analysis_bills_found.Count > 0)
                {
                    // But it takes less than a second to fill it anyway
                    // and there is NO speed difference between using ExecuteReader and SqlCe ResultSet
                    // This code is a lot easier to understand and maintain ...

                    // Get rid of all the old Ba! in the ListView ...
                    // AND MAKE SURE YOU CLEAR THE **LISTVIEW** AND NOT THE ITEMS!!

                    // Gets everything out in descending order
                    int bills_index = utilityviewmodel.analysis_bills_found.Count;

                    while (bills_index > 0)
                    {
                        SmartUtility.Analysis_Bills analysis_bills_row = utilityviewmodel.analysis_bills_found[bills_index - 1];
                        string username = SmartRoutinesV2018.GetDisplayName(ourviewmodel, analysis_bills_row.USERNAME);

                        SmartUtility.AnalysisBillsView analysis_billsview_row = new SmartUtility.AnalysisBillsView()
                        {
                            // Fields in UPPERCASE have their Heading text set in SmartTest
                            USERNAME = username,
                            ACCOUNT_NO = analysis_bills_row.ACCOUNT_NO,
                            BILL_DATE = analysis_bills_row.BILL_DATE,
                            STATEMENT_ID = analysis_bills_row.STATEMENT_ID,
                            DATE = analysis_bills_row.DATE,
                            CODE = analysis_bills_row.CODE,
                            DESCRIPTION = analysis_bills_row.DESCRIPTION,
                            //AMOUNT = (SmartRoutinesV2018.ReturnDecimal(analysis_bills_row.AMOUNT, true) * utilityviewmodel.exchange_rate).ToString("c", utilityviewmodel.utilityDisplayCulture),
                            AMOUNT = DisplayValue(ourviewmodel,
                                                    utilityviewmodel,
                                                    utilityviewmodel.ConvertToSymbol,
                                                    ourviewmodel.exchangeRate,
                                                    (decimal)(analysis_bills_row.AMOUNT / 100.0)),

                            //((decimal)(analysis_bills_row.AMOUNT / 100.0) * ourviewmodel.exchangeRate[utilityviewmodel.currencyOrdinal]).ToString("c", utilityviewmodel.utilityDisplayCulture),
                            //BALANCE = (SmartRoutinesV2018.ReturnDecimal(analysis_bills_row.BALANCE, true) * utilityviewmodel.exchange_rate).ToString("c", utilityviewmodel.utilityDisplayCulture)
                            BALANCE = DisplayValue(ourviewmodel,
                                                    utilityviewmodel,
                                                    utilityviewmodel.ConvertToSymbol,
                                                    ourviewmodel.exchangeRate,
                                                    (decimal)(analysis_bills_row.BALANCE / 100.0))

                            //((decimal)(analysis_bills_row.BALANCE / 100.0) * ourviewmodel.exchangeRate[utilityviewmodel.currencyOrdinal]).ToString("c", utilityviewmodel.utilityDisplayCulture)
                        };
                        analysis_bills_temp.Add(analysis_billsview_row);
                        switch (analysis_bills_row.CODE)
                        {
                            case "PAY":    // Payment
                                utilityviewmodel.payments_made += analysis_bills_row.AMOUNT;
                                break;
                            case "BBF":    // Balance brought forward
                                break;
                            case "BCF":    // Balance carried forward
                                break;
                            default:                            // Everything else
                                utilityviewmodel.billed_amount += analysis_bills_row.AMOUNT;
                                break;
                        }
                        bills_index--;
                    }
                }
            }
            return analysis_bills_temp;
        }

        internal static List<SmartUtility.AnalysisReadingsView> Fill_Readings(MainViewModel ourviewmodel,
                                            //string ddmmyyyy_Format,
                                            UtilityViewModel utilityviewmodel,
                                            char local_resource_code,
                                            string format_3places)
        {
            List<SmartUtility.AnalysisReadingsView> readings_viewList = new List<SmartUtility.AnalysisReadingsView>();

            utilityviewmodel.e_readings_found = new List<SmartUtility.EReadings>();
            utilityviewmodel.g_readings_found = new List<SmartUtility.GReadings>();
            utilityviewmodel.d_readings_found = new List<SmartUtility.EReadings>();
            //Tab "Readings" (across ALL Account Nos (e.g. Pat's Npower)
            switch (local_resource_code)
            {
                case SmartParametersV2016.Electricity:
                    SmartSpikeUtilityV2017.Utility_Find_Readings(utilityviewmodel,
                                                    local_resource_code);
                    readings_viewList = Load_Readings(ourviewmodel,
                                                        utilityviewmodel,
                                                        local_resource_code,
                                                        format_3places,
                                                        utilityviewmodel.e_readings_found,
                                                        utilityviewmodel.g_readings_found);
                    break;
                case SmartParametersV2016.Gas:
                    SmartSpikeUtilityV2017.Utility_Find_Readings(utilityviewmodel,
                                                    local_resource_code);
                    readings_viewList = Load_Readings(ourviewmodel,
                                                        utilityviewmodel,
                                                        local_resource_code,
                                                        format_3places,
                                                        utilityviewmodel.e_readings_found,
                                                        utilityviewmodel.g_readings_found);
                    break;
                case SmartParametersV2016.DualFuel:
                    SmartSpikeUtilityV2017.Utility_Find_Readings(utilityviewmodel,
                                                    local_resource_code);
                    readings_viewList = Load_Readings(ourviewmodel,
                                                    utilityviewmodel,
                                                    local_resource_code,
                                                    format_3places,
                                                    utilityviewmodel.e_readings_found,
                                                    utilityviewmodel.g_readings_found);
                    break;
                default:
                    break;
            }
            readings_viewList = new List<SmartUtility.AnalysisReadingsView>(from Temp in readings_viewList
                                                                            orderby Temp.USERNAME,
                                                                                    Temp.STATEMENT_ID ascending
                                                                            select Temp);
            return readings_viewList;
        }

        internal static List<SmartUtility.AnalysisReadingsView> Load_Readings(MainViewModel ourviewmodel,
                                    UtilityViewModel utilityviewmodel,
                                    char local_resource_code,
                                    string format_3places,
                                    List<SmartUtility.EReadings> e_readings_found,
                                    List<SmartUtility.GReadings> g_readings_found)
        {
            List<SmartUtility.AnalysisReadingsView> readings_viewList = new List<SmartUtility.AnalysisReadingsView>();

            if (local_resource_code == SmartParametersV2016.Electricity ||
                local_resource_code == SmartParametersV2016.DualFuel)
            {
                if (e_readings_found.Count > 0)
                {
                    foreach (SmartUtility.EReadings e_readings_row in e_readings_found)
                    {
                        SmartUtility.AnalysisReadingsView readings_view = new SmartUtility.AnalysisReadingsView()
                        {
                            USERNAME = SmartRoutinesV2018.GetDisplayName(ourviewmodel, e_readings_row.USERNAME),
                            ACCOUNT_NO = e_readings_row.ACCOUNT_NO,
                            STATEMENT_ID = e_readings_row.STATEMENT_ID,
                            READINGS_PERIOD_END = e_readings_row.READINGS_PERIOD_END,
                            METER_SERIAL_NO = e_readings_row.METER_SERIAL_NO.Substring(0, e_readings_row.METER_SERIAL_NO.Length <= 14 ? e_readings_row.METER_SERIAL_NO.Length : 14), //Enforce 14 char max
                            READ_TYPE = e_readings_row.READ_TYPE,
                            THIS_READ = (e_readings_row.D_THIS_READ + e_readings_row.N_THIS_READ).ToString(),       // This Read
                            LAST_READ = (e_readings_row.D_LAST_READ + e_readings_row.N_LAST_READ).ToString(),      // Last Read
                            UNITS_USED = (e_readings_row.D_UNITS_USED + e_readings_row.N_UNITS_USED).ToString(),    // Units Used
                            UNIT_OF_MEASURE = e_readings_row.UNIT_OF_MEASURE
                        };
                        readings_viewList.Add(readings_view);
                        utilityviewmodel.totalReadings += e_readings_row.D_UNITS_USED + e_readings_row.N_UNITS_USED;
                    }
                }
                // The Readings List is no longer needed    
            }
            if (local_resource_code == SmartParametersV2016.Gas ||
                local_resource_code == SmartParametersV2016.DualFuel)
            {
                if (g_readings_found.Count > 0)
                {
                    foreach (SmartUtility.GReadings g_readings_row in g_readings_found)
                    {
                        SmartUtility.AnalysisReadingsView readings_view = new SmartUtility.AnalysisReadingsView()
                        {
                            USERNAME = SmartRoutinesV2018.GetDisplayName(ourviewmodel, g_readings_row.USERNAME),
                            ACCOUNT_NO = g_readings_row.ACCOUNT_NO,
                            STATEMENT_ID = g_readings_row.STATEMENT_ID,
                            READINGS_PERIOD_END = g_readings_row.READINGS_PERIOD_END,
                            METER_SERIAL_NO = g_readings_row.METER_SERIAL_NO.Substring(0, g_readings_row.METER_SERIAL_NO.Length <= 14 ? g_readings_row.METER_SERIAL_NO.Length : 14), //Enforce 14 char max
                            READ_TYPE = g_readings_row.READ_TYPE,
                            THIS_READ = g_readings_row.D_THIS_READ.ToString(),       // This Read
                            LAST_READ = g_readings_row.D_LAST_READ.ToString(),      // Last Read
                            UNITS_USED = string.Format(format_3places, g_readings_row.D_UNITS_USED_M3),    // Units Used
                            UNIT_OF_MEASURE = g_readings_row.UNIT_OF_MEASURE
                        };
                        readings_viewList.Add(readings_view);
                        utilityviewmodel.totalReadings += g_readings_row.D_UNITS_USED_KWH;
                    }
                }
            }

            readings_viewList = new List<SmartUtility.AnalysisReadingsView>(from Reading in readings_viewList
                                                                            orderby Reading.READINGS_PERIOD_END descending,
                                                                                    Reading.ACCOUNT_NO ascending,
                                                                                    Reading.STATEMENT_ID descending
                                                                            select Reading);
            return readings_viewList;
        }

        internal static List<SmartUtility.AnalysisBreakdownView> Fill_Breakdown(
                                            MainViewModel ourviewmodel,
                                            UtilityViewModel utilityviewmodel,
                                            //string ddmmmyyyy_format,
                                            //string format_2places,
                                            string format_3places)
        {
            // As long as Tariff Costs contains something sensible ...we
            // don't need to go to a Web Service to get this one!

            // Try and do this on a month by month basis ..... =&-O(

            DateTime lower_date = SmartParametersV2016.defaultDate,
                    limit_date = SmartParametersV2016.defaultDate;

            utilityviewmodel.total_units = 0.0M;
            utilityviewmodel.total_value = 0.0M;
            utilityviewmodel.total_item_count = 0;

            // But of course then we have to re-create the fucking headings
            // God this stuff is SHIT
            // Get rid of all the old Ba! in the ListView and not just the fucking Items!!

            List<SmartUtility.AnalysisBreakdownView> breakdown_temp = new List<SmartUtility.AnalysisBreakdownView>();

            List<SmartUtility.TariffCosts> tariff_costs_monthList = new List<SmartUtility.TariffCosts>();
            List<SmartUtility.TariffCosts> tariff_costs_years = SmartSpikeUtilityV2017.Utility_Find_Tariff_Costs_Years(ourviewmodel,
                                                                                                                        utilityviewmodel);

            foreach (SmartUtility.TariffCosts tariff_costs_row in tariff_costs_years)
            {
                if (lower_date == SmartParametersV2016.defaultDate)
                {
                    lower_date = tariff_costs_row.PERIOD_END;
                    limit_date = lower_date.AddMonths(1);
                }

                if (SmartRoutinesV2018.DateTimeCompare(tariff_costs_row.PERIOD_END, limit_date) < 0)
                {
                    tariff_costs_monthList.Add(tariff_costs_row);
                }
                else
                {
                    Process_Month(ourviewmodel,
                                    utilityviewmodel,
                                    breakdown_temp,
                                    //ddmmmyyyy_format,
                                    //format_2places,
                                    format_3places,
                                    tariff_costs_monthList);
                    // Do the whole thing again for the next month            
                    tariff_costs_monthList.Clear();
                    lower_date = tariff_costs_row.PERIOD_END;
                    limit_date = lower_date.AddMonths(1);
                    tariff_costs_monthList.Add(tariff_costs_row);
                }
            }
            
            // Any left in the month to do?
            if (tariff_costs_monthList.Count > 0)
            {
                // Flush them out
                Process_Month(ourviewmodel,
                                utilityviewmodel,
                                breakdown_temp,
                                //ddmmmyyyy_format,
                                //format_2places,
                                format_3places,
                                tariff_costs_monthList);
            }

            // Now do the totals
            SmartUtility.AnalysisBreakdownView breakdown_total = new SmartUtility.AnalysisBreakdownView()
            {
                USERNAME = "",                       // Who does it belong to?                
                FROM_DATE = SmartParametersV2016.defaultDate,                       // From Date
                TO_DATE = SmartParametersV2016.defaultDate,                       // To Date
                CODE = "Total",                        // Code
                DESCRIPTION = "",            // Description (not be shown until drillsown)
                ITEMS = utilityviewmodel.total_item_count.ToString(), // Item count
                CHARGES_DISCOUNTS = "",                // Charges and Discounts
                DAY_UNITS = "",                        // Day Units
                DAY_RATE = "",                // Day Rate
                NIGHT_UNITS = "",                      // Night Units
                NIGHT_RATE = "",              // Night Rate
                TOTAL_UNITS = string.Format(format_3places, utilityviewmodel.total_units), // Total Units
                TOTAL = DisplayValue(ourviewmodel,
                                    utilityviewmodel,
                                    utilityviewmodel.ConvertToSymbol,
                                    ourviewmodel.exchangeRate,
                                    utilityviewmodel.total_value)

                //(utilityviewmodel.total_value * ourviewmodel.exchangeRate[utilityviewmodel.currencyOrdinal]).ToString("c", utilityviewmodel.utilityDisplayCulture)
            };  // Total Value
            breakdown_temp.Add(breakdown_total);

            return breakdown_temp;
        }

        internal static void Process_Month(MainViewModel ourviewmodel,
                                        UtilityViewModel utilityviewmodel,
                                        List<SmartUtility.AnalysisBreakdownView> breakdown_temp,
                                        //string ddmmmyyyy_format,
                                        //string format_2places,
                                        string format_3places,
                                        List<SmartUtility.TariffCosts> tariff_costs_monthList)
        {
            string username = "";
            DateTime lower_date = SmartParametersV2016.defaultDate,
                    vat_lower_date = SmartParametersV2016.defaultDate,
                    upper_date = SmartParametersV2016.defaultDate,
                    vat_upper_date = SmartParametersV2016.defaultDate;
            short last_supplier_code = 0,
                    supplier_code;
            string account_no,
                    last_account_no = "",
                    code,
                    last_code = "",
                    description,
                    last_description = "";
            decimal vat_rate = 0.0M,
                    last_vat_rate = 0.0M,
                    total_month,
                    standing_charge,
                    d_units,
                    d_units_rate,
                    n_units,
                    n_units_rate,
                    figure,
                    last_standing_charge = 0.0M,
                    last_d_units = 0.0M,
                    last_d_units_rate = 0.0M,
                    last_n_units = 0.0M,
                    last_n_units_rate = 0.0M,
                    last_figure = 0.0M;
            int item_count = 0;

            utilityviewmodel.total_line = 0.0M;
            utilityviewmodel.total_units_x_items = 0.0M;

            List<SmartUtility.TariffCosts> tariff_month_totalList = new List<SmartUtility.TariffCosts>();
            List<SmartUtility.TariffCosts> tariff_costs_month = SmartSpikeUtilityV2017.Utility_Find_Tariff_Costs_Months(ourviewmodel,
                                                                                                            utilityviewmodel,
                                                                                                            tariff_costs_monthList);

            foreach (SmartUtility.TariffCosts tariff_month_row in tariff_costs_month)
            {
                username = SmartRoutinesV2018.GetDisplayName(ourviewmodel, tariff_month_row.USERNAME);
                supplier_code = tariff_month_row.SUPPLIER_CODE;
                account_no = tariff_month_row.ACCOUNT_NO;
                code = tariff_month_row.CODE;
                description = tariff_month_row.DESCRIPTION;
                d_units = tariff_month_row.D_UNITS;
                d_units_rate = tariff_month_row.D_UNITS_RATE;
                n_units = tariff_month_row.N_UNITS;
                n_units_rate = tariff_month_row.N_UNITS_RATE;
                standing_charge = tariff_month_row.STANDING_CHARGE;
                figure = tariff_month_row.FIGURE;

                vat_rate = tariff_month_row.VAT_RATE;

                // Big compare! Might need USERNAME in here??
                if (Big_Compare(code, last_code,
                                standing_charge, last_standing_charge,
                                d_units, last_d_units,
                                d_units_rate, last_d_units_rate,
                                n_units, last_n_units,
                                n_units_rate, last_n_units_rate,
                                figure, last_figure,
                                vat_rate, last_vat_rate))
                {
                    if (item_count == 0)
                    {
                        lower_date = tariff_month_row.PERIOD_END;
                        vat_lower_date = tariff_month_row.PERIOD_END;
                    }
                    upper_date = tariff_month_row.PERIOD_END;
                    vat_upper_date = tariff_month_row.PERIOD_END;
                    item_count++;
                }
                else
                {
                    SmartUtility.TariffCosts tariffcosts = new SmartUtility.TariffCosts()
                    {
                        USERNAME = username,
                        CUBEFACE_CODE = utilityviewmodel.cubeface_code,
                        SUPPLIER_CODE = last_supplier_code,
                        ACCOUNT_NO = last_account_no,
                        PERIOD_END = lower_date,
                        CODE = last_code,
                        UPPER_DATE = upper_date,
                        ITEM_COUNT = (short)item_count,
                        D_UNITS = last_d_units,
                        D_UNITS_RATE = last_d_units_rate,
                        N_UNITS = n_units,
                        N_UNITS_RATE = n_units_rate,
                        STANDING_CHARGE = last_standing_charge,
                        // LEFTOVER = 0.0M,     // Don't care
                        FIGURE = last_figure,   // FIGURE - DO care its a Discount you numbskull!
                        DESCRIPTION = last_description,
                        VAT_RATE = last_vat_rate
                    };
                    tariff_month_totalList.Add(tariffcosts);
                    // Reset these
                    item_count = 1;
                    lower_date = tariff_month_row.PERIOD_END;
                    upper_date = lower_date;
                }
                last_supplier_code = supplier_code;
                last_account_no = account_no;
                last_code = code;
                last_standing_charge = standing_charge;
                last_d_units = d_units;
                last_d_units_rate = d_units_rate;
                last_n_units = n_units;
                last_n_units_rate = n_units_rate;
                last_figure = figure;
                last_description = description;
                last_vat_rate = vat_rate;
            }
            // Do last range if needs be
            if (item_count > 0)
            {
                SmartUtility.TariffCosts tariffcosts = new SmartUtility.TariffCosts()
                {
                    USERNAME = username,
                    CUBEFACE_CODE = utilityviewmodel.cubeface_code,
                    SUPPLIER_CODE = last_supplier_code,
                    ACCOUNT_NO = last_account_no,
                    PERIOD_END = lower_date,
                    CODE = last_code,
                    UPPER_DATE = upper_date,
                    ITEM_COUNT = (short)item_count,
                    D_UNITS = last_d_units,
                    D_UNITS_RATE = last_d_units_rate,
                    N_UNITS = last_n_units,
                    N_UNITS_RATE = last_n_units_rate,
                    STANDING_CHARGE = last_standing_charge,
                    // LEFTOVER = 0.0M,     // Don't care
                    FIGURE = last_figure,   // FIGURE DO care its a fucking discount
                    DESCRIPTION = last_description,
                    VAT_RATE = last_vat_rate
                };
                tariff_month_totalList.Add(tariffcosts);
            }

            total_month = 0.0M;

            // Sort them by Lower Date, Upper Date and then Code ...fingers crossed, eh Ray?
            tariff_costs_month = SmartSpikeUtilityV2017.Utility_Find_Tariff_Costs_Months_Total(ourviewmodel,
                                                                                                utilityviewmodel,
                                                                                                tariff_month_totalList);

            foreach (SmartUtility.TariffCosts tariff_month_row in tariff_costs_month)
            {
                username = tariff_month_row.USERNAME;
                lower_date = tariff_month_row.PERIOD_END;
                upper_date = tariff_month_row.UPPER_DATE;
                code = tariff_month_row.CODE;
                description = tariff_month_row.DESCRIPTION;
                item_count = tariff_month_row.ITEM_COUNT;
                d_units = tariff_month_row.D_UNITS;
                d_units_rate = tariff_month_row.D_UNITS_RATE;
                n_units = tariff_month_row.N_UNITS;
                n_units_rate = tariff_month_row.N_UNITS_RATE;
                standing_charge = tariff_month_row.STANDING_CHARGE;
                figure = tariff_month_row.FIGURE;

                // Add the list items to the ListView
                if (code == "001" || code == "002" || code == "003")
                {
                    breakdown_temp.Add(Make_Discount_Record(ourviewmodel,
                                                            utilityviewmodel,
                                                            //ddmmmyyyy_format,
                                                            username,
                                                            lower_date,
                                                            upper_date,
                                                            //format_2places,
                                                            code,
                                                            description,
                                                            item_count,
                                                            figure));
                }
                else
                {
                    breakdown_temp.Add(Make_Units_Record(ourviewmodel,
                                                        utilityviewmodel,
                                                        //ddmmmyyyy_format,
                                                        username,
                                                        lower_date,
                                                        upper_date,
                                                        //format_2places,
                                                        format_3places,
                                                        code,
                                                        description,
                                                        item_count,
                                                        standing_charge,
                                                        d_units,
                                                        d_units_rate,
                                                        n_units,
                                                        n_units_rate));
                    //figure));
                }
                // Update the totals
                utilityviewmodel.total_item_count += item_count;
                utilityviewmodel.total_units += utilityviewmodel.total_units_x_items;
                total_month += utilityviewmodel.total_line;
                utilityviewmodel.total_value += utilityviewmodel.total_line;
            }
            // Add the VAT for this month           
            breakdown_temp.Add(Make_Vat_Record(ourviewmodel,
                                                utilityviewmodel,
                                                //ddmmmyyyy_format,
                                                username,
                                                vat_lower_date,
                                                vat_upper_date,
                                                "999",
                                                "VAT @ " + vat_rate.ToString(),
                                                1,
                                                vat_rate,
                                                total_month));
            utilityviewmodel.total_value += utilityviewmodel.total_line;
            return;
        }

        internal static bool Big_Compare(string code,
                                            string last_code,
                                            decimal standing_charge,
                                            decimal last_standing_charge,
                                            decimal d_units,
                                            decimal last_d_units,
                                            decimal d_units_rate,
                                            decimal last_d_units_rate,
                                            decimal n_units,
                                            decimal last_n_units,
                                            decimal n_units_rate,
                                            decimal last_n_units_rate,
                                            decimal figure,
                                            decimal last_figure,
                                            decimal vat_rate,
                                            decimal last_vat_rate)
        {
            if (((code == last_code) || string.IsNullOrEmpty(last_code)) &&
                    ((standing_charge == last_standing_charge) || (last_standing_charge == 0.0M)) && // This contains leftovers
                    ((d_units == last_d_units) || (last_d_units == 0.0M)) &&
                    ((d_units_rate == last_d_units_rate) || (last_d_units_rate == 0.0M)) &&
                    ((n_units == last_n_units) || (last_n_units == 0.0M)) &&
                    ((n_units_rate == last_n_units_rate) || (last_n_units_rate == 0.0M)) &&
                    ((figure == last_figure) || (last_figure == 0.0M)) &&
                    ((vat_rate == last_vat_rate) || (last_vat_rate == 0.0M)))
            {
                return true;
            }
            return false;
        }

        internal static SmartUtility.AnalysisBreakdownView Make_Discount_Record(MainViewModel ourviewmodel,
                                            UtilityViewModel utilityviewmodel,
                                            //string ddmmmyyyy_format,
                                            string username,
                                            DateTime lower_date,
                                            DateTime upper_date,
                                            //string format_2places,
                                            string code,
                                            string description,
                                            int item_count,
                                            decimal figure)
        {
            utilityviewmodel.total_line = (item_count * figure) / 100;
            //string username = SmartRoutinesV2018.GetUsername(ourviewmodel);
            SmartUtility.AnalysisBreakdownView breakdown_view =
                    new SmartUtility.AnalysisBreakdownView()
                    {
                        USERNAME = username,
                        FROM_DATE = lower_date,
                        TO_DATE = upper_date,
                        CODE = code,
                        DESCRIPTION = description,
                        ITEMS = item_count.ToString(),
                        //CHARGES_DISCOUNTS = string.Format(format_2places, (item_count * figure)),
                        CHARGES_DISCOUNTS = DisplayValue(ourviewmodel,
                                                        utilityviewmodel,
                                                        utilityviewmodel.ConvertToSymbol,
                                                        ourviewmodel.exchangeRate,
                                                        (item_count * figure)),

                        //((item_count * figure) * ourviewmodel.exchangeRate[utilityviewmodel.currencyOrdinal]).ToString("c", utilityviewmodel.utilityDisplayCulture),
                        DAY_UNITS = "",
                        DAY_RATE = "",
                        NIGHT_UNITS = "",
                        NIGHT_RATE = "",
                        TOTAL_UNITS = "",
                        //TOTAL = total_line.ToString("c", utilityviewmodel.utilityDisplayCulture)
                        TOTAL = DisplayValue(ourviewmodel,
                                                        utilityviewmodel,
                                                        utilityviewmodel.ConvertToSymbol,
                                                        ourviewmodel.exchangeRate,
                                                        utilityviewmodel.total_line)
                        //(utilityviewmodel.total_line * ourviewmodel.exchangeRate[utilityviewmodel.currencyOrdinal]).ToString("c", utilityviewmodel.utilityDisplayCulture)
                    };
            return breakdown_view;
        }

        internal static SmartUtility.AnalysisBreakdownView Make_Vat_Record(MainViewModel ourviewmodel,
                                            UtilityViewModel utilityviewmodel,
                                            //string ddmmmyyyy_format,
                                            string username,
                                            DateTime lower_date,
                                            DateTime upper_date,
                                            string code,
                                            string description,
                                            int item_count,
                                            decimal vat_rate,
                                            decimal total_month)
        {
            // Define the list items
            utilityviewmodel.total_line = (item_count * total_month * vat_rate) / 100;
            SmartUtility.AnalysisBreakdownView breakdown_view =
                new SmartUtility.AnalysisBreakdownView()
                {
                    USERNAME = username,
                    FROM_DATE = lower_date,                             // 1 From date
                    TO_DATE = upper_date,                               // 2 To Date
                    CODE = code,                                        // 3 Code
                    DESCRIPTION = description,                          // 4 Description
                    ITEMS = item_count.ToString(),               // 5 Item Count
                    CHARGES_DISCOUNTS = "",                   // 6 Charges and Discounts
                    DAY_UNITS = "",                           // 7 Day Units
                    DAY_RATE = "",                            // 8 Day Rate
                    NIGHT_UNITS = "",                         // 9 Night Units
                    NIGHT_RATE = "",                          // 10 Night Rate
                    TOTAL_UNITS = "",                         // 11 Total Units
                    //TOTAL = total_line.ToString("c", utilityviewmodel.utilityDisplayCulture)
                    TOTAL = DisplayValue(ourviewmodel,
                                        utilityviewmodel,
                                        utilityviewmodel.ConvertToSymbol,
                                        ourviewmodel.exchangeRate,
                                        utilityviewmodel.total_line)
                    //(utilityviewmodel.total_line * ourviewmodel.exchangeRate[utilityviewmodel.currencyOrdinal]).ToString("c", utilityviewmodel.utilityDisplayCulture)
                };// 12 Total Line (VAT)                                                                                                                     // Add the list items to the ListView
            return breakdown_view;
        }

        internal static SmartUtility.AnalysisBreakdownView Make_Units_Record(MainViewModel ourviewmodel,
                                                UtilityViewModel utilityviewmodel,
                                                //string ddmmmyyyy_format,
                                                string username,
                                                DateTime lower_date,
                                                DateTime upper_date,
                                                //string format_2places,
                                                string format_3places,
                                                string code,
                                                string description,
                                                int item_count,
                                                decimal StandingCharge,
                                                decimal d_units,
                                                decimal d_units_rate,
                                                decimal n_units,
                                                decimal n_units_rate)
        {
            // Define the list items
            string tot_un;
            if (StandingCharge == 0.0M)
            {
                utilityviewmodel.total_units_x_items = item_count * (d_units + n_units);
                tot_un = string.Format(format_3places, utilityviewmodel.total_units_x_items);// Total Units
                utilityviewmodel.total_line = (item_count * ((d_units * d_units_rate) +
                                                (n_units * n_units_rate))) / 100;
            }
            else
            {
                utilityviewmodel.total_units_x_items = 0.0M;
                tot_un = "";
                // Used for leftovers as well
                utilityviewmodel.total_line = (item_count * StandingCharge) / 100;
            }

            SmartUtility.AnalysisBreakdownView breakdown_view = new SmartUtility.AnalysisBreakdownView()
            {
                USERNAME = username,
                FROM_DATE = lower_date,                                     // From date
                TO_DATE = upper_date,                                       // To Date
                CODE = code,                                                // Code
                DESCRIPTION = description,                                  // Description
                ITEMS = item_count.ToString(),                       // Item count
                // Why is this 3 DECIMAL PLACES??
                //CHARGES_DISCOUNTS = string.Format(format_3places, StandingCharge),// Charges and Discounts
                CHARGES_DISCOUNTS = DisplayValue(ourviewmodel,
                                                utilityviewmodel,
                                                utilityviewmodel.ConvertToSymbol,
                                                ourviewmodel.exchangeRate,
                                                StandingCharge),

                //(StandingCharge * ourviewmodel.exchangeRate[utilityviewmodel.currencyOrdinal]).ToString("c", utilityviewmodel.utilityDisplayCulture),
                DAY_UNITS = string.Format(format_3places, d_units),                 // Day Units
                //DAY_RATE = string.Format(format_2places, d_units_rate),           // Day Rate
                DAY_RATE = DisplayValue(ourviewmodel,
                                        utilityviewmodel,
                                        utilityviewmodel.ConvertToSymbol,
                                        ourviewmodel.exchangeRate,
                                        d_units_rate),

                //(d_units_rate * ourviewmodel.exchangeRate[utilityviewmodel.currencyOrdinal]).ToString("c", utilityviewmodel.utilityDisplayCulture),
                NIGHT_UNITS = string.Format(format_3places, n_units),               // Night Units
                //NIGHT_RATE = string.Format(format_2places, n_units_rate),         // Night Rate
                NIGHT_RATE = DisplayValue(ourviewmodel,
                                        utilityviewmodel,
                                        utilityviewmodel.ConvertToSymbol,
                                        ourviewmodel.exchangeRate,
                                        n_units_rate),
                //
                //(n_units_rate * ourviewmodel.exchangeRate[utilityviewmodel.currencyOrdinal]).ToString("c", utilityviewmodel.utilityDisplayCulture),
                TOTAL_UNITS = tot_un,
                // Add the list items to the ListView
                //TOTAL = total_line.ToString("c", utilityviewmodel.utilityDisplayCulture)
                TOTAL = DisplayValue(ourviewmodel,
                                    utilityviewmodel,
                                    utilityviewmodel.ConvertToSymbol,
                                    ourviewmodel.exchangeRate,
                                    utilityviewmodel.total_line),

                //(utilityviewmodel.total_line * ourviewmodel.exchangeRate[utilityviewmodel.currencyOrdinal]).ToString("c", utilityviewmodel.utilityDisplayCulture)
            };    // Total Line (Items x Units x Rate
            return breakdown_view;
        }

        internal static bool Check_Utility_Supplier_Area(MainViewModel ourviewmodel,
                                                UtilityViewModel utilityviewmodel,
                                                SmartUtility.Brands brand_row,
                                                FieldInfo[] myFields)
        {
            // Is it really necessary to return a tariff_name list when the tariff code
            // only ever appears once for a Brand or Supplier in Tariff Area Matrix??
            int field_index = SmartNibbyV2016.Area_Matrix_Index(utilityviewmodel.area_code,
                                                                myFields);
            if (utilityviewmodel.area_code == 0 ||     // Initial condition to get Supplier list
                field_index >= 0)
            {
                List<SmartUtility.BrandMatrix> brand_matrix_found = SmartSpikeUtilityV2017.Utility_Find_Brand_Matrix(ourviewmodel,
                                                                                            utilityviewmodel,
                                                                                            brand_row.BRAND_CODE);
                foreach (SmartUtility.BrandMatrix brand_matrix_row in brand_matrix_found)
                {
                    if (utilityviewmodel.area_code == 0 || // Initial condition to get Supplier List
                        Convert.ToChar(myFields[field_index].GetValue(brand_matrix_row).ToString()) == SmartParametersV2016.tariffMatrixValid)
                    {
                        utilityviewmodel.brand_code_temp = brand_matrix_row.BRAND_CODE;
                        return true;  // Only the first on this Supplier
                    }
                }
            }
            return false;
        }

        internal static async Task<int> Build_Tariff_Dropdown(MainViewModel ourviewmodel,
                                            UtilityViewModel utilityviewmodel,
                                            DateTime defaultDate,
                                            List<SmartUtility.AnalysisCosts> e_analysis_costsList,
                                            List<SmartUtility.AnalysisCosts> g_analysis_costsList,
                                            List<SmartUtility.AnalysisCosts> d_analysis_costsList,
                                            char resource_code,
                                            string resource_type,
                                            short supplier_code,
                                            short brand_code,
                                            int tariff_code)
        {
            int tariff_index = -1,
                    loop = 0;

            if (utilityviewmodel.TariffSelectedIndex == -1)
            {
                utilityviewmodel.UtilityTariffsList.Clear();
                List<UtilityViewModel.TariffItem> temp_tariffs = new List<UtilityViewModel.TariffItem>();

                // Sanity check the Area to avoid crashes
                if (utilityviewmodel.area_code != 0)
                {
                    // Anything in the Tariff table?
                    if (utilityviewmodel.Hezbollah.tariffsList.Count > 0)
                    {
                        List<SmartUtility.AnalysisCosts> analysis_costs_found = new List<SmartUtility.AnalysisCosts>();
                        switch (resource_code)
                        {
                            case SmartParametersV2016.Electricity:
                                analysis_costs_found = SmartSpikeUtilityV2017.Utility_Find_AnalysisCosts_Brand(ourviewmodel,
                                                                                                    utilityviewmodel,
                                                                                                    supplier_code,
                                                                                                    brand_code,
                                                                                                    e_analysis_costsList);
                                break;
                            case SmartParametersV2016.Gas:
                                analysis_costs_found = SmartSpikeUtilityV2017.Utility_Find_AnalysisCosts_Brand(ourviewmodel,
                                                                                                    utilityviewmodel,
                                                                                                    supplier_code,
                                                                                                    brand_code,
                                                                                                    g_analysis_costsList);
                                break;
                            case SmartParametersV2016.DualFuel:
                                // Fingers crossed this works ...
                                analysis_costs_found = SmartSpikeUtilityV2017.Utility_Find_AnalysisCosts_Brand(ourviewmodel,
                                                                                                    utilityviewmodel,
                                                                                                    supplier_code,
                                                                                                    brand_code,
                                                                                                    d_analysis_costsList);
                                break;
                            default:
                                break;
                        }
                        if (analysis_costs_found.Count > 0)
                        {
                            // Lookup the supplier from the brand
                            List<SmartUtility.Brands> brands_found = SmartSpikeUtilityV2017.Utility_Find_Supplier(ourviewmodel,
                                                                            utilityviewmodel,
                                                                            resource_code,
                                                                            supplier_code,
                                                                            brand_code);
                            if (brands_found.Count == 1)
                            {
                                foreach (SmartUtility.Brands brand_row in brands_found)
                                {
                                    List<SmartUtility.Tariffs> tariff_found = SmartSpikeUtilityV2017.Utility_Find_Tariffs(ourviewmodel,
                                                                                                utilityviewmodel,
                                                                                                brand_row.SUPPLIER_CODE,
                                                                                                resource_code,
                                                                                                resource_type);
                                    if (tariff_found.Count > 0)
                                    {
                                        FieldInfo[] myFields = typeof(SmartUtility.TariffMatrix).GetFields(SmartParametersV2016.bindingFlags);


                                        string CHEAPEST_TARIFF_NAME = SmartSpikeUtilityV2017.Utility_Lookup_TariffName(utilityviewmodel,
                                                                                                                resource_code,
                                                                                                                analysis_costs_found.First().SUPPLIER_CODE,
                                                                                                                analysis_costs_found.First().BRAND_CODE,
                                                                                                                analysis_costs_found.First().RESOURCE_TYPE,
                                                                                                                analysis_costs_found.First().TARIFF_CODE);
                                        int last = analysis_costs_found.Count - 1;
                                        string DEAREST_TARIFF_NAME = SmartSpikeUtilityV2017.Utility_Lookup_TariffName(utilityviewmodel,
                                                                                                                resource_code,
                                                                                                                analysis_costs_found.Last().SUPPLIER_CODE,
                                                                                                                analysis_costs_found.Last().BRAND_CODE,
                                                                                                                analysis_costs_found.Last().RESOURCE_TYPE,
                                                                                                                analysis_costs_found.Last().TARIFF_CODE);

                                        foreach (SmartUtility.Tariffs tariff_row in tariff_found)
                                        {
                                            string tariff_name = Check_Tariff_Area(ourviewmodel,
                                                                                    utilityviewmodel,
                                                                                    resource_code,
                                                                                    brand_code,
                                                                                    resource_type,
                                                                                    tariff_row,
                                                                                    myFields);
                                            // It could be that the tariff isn't applicable to out
                                            // area, so the name will be returned empty to indicate that fact
                                            if (!string.IsNullOrEmpty(tariff_name))
                                            {
                                                UtilityViewModel.TariffItem tariff_item = new UtilityViewModel.TariffItem()
                                                {
                                                    Content = tariff_name,
                                                    Value = "",
                                                    Colour = ourviewmodel.blackColour

                                                };
                                                if (analysis_costs_found.Count > 0)
                                                {
                                                    if (tariff_name == CHEAPEST_TARIFF_NAME)
                                                    {
#if WINFORMS
                                                        //                                                        tariff_item.Content = SmartParametersV2016.defaultgreen.ToString() + tariff_item.Content;
#endif
                                                        tariff_item.Colour = ourviewmodel.greenColour;
                                                    }
                                                    else
                                                    {
                                                        if (tariff_name == DEAREST_TARIFF_NAME)
                                                        {
#if WINFORMS
                                                            //                                                            tariff_item.Content = SmartParametersV2016.defaultred + tariff_item.Content;
#endif
                                                            tariff_item.Colour = ourviewmodel.redColour;
                                                        }
                                                    }
                                                }
                                                temp_tariffs.Add(tariff_item);

                                                // To cater for the inevitable typo
                                                if ((tariff_code == 0) &&
                                                    (loop == 0))
                                                {
                                                    tariff_index = loop;
                                                }
                                                else
                                                {
                                                    if (tariff_row.TARIFF_CODE == tariff_code)
                                                    {
                                                        tariff_index = loop;
                                                    }
                                                }
                                                loop++;
                                            }
                                        }
                                    }
                                }

                                if (tariff_index < 0)
                                {
                                    string P1 = SmartParametersV2016.Utility.ToString() + SmartParametersV2016.unitSeparator +
                                        utilityviewmodel.area_code.ToString() + SmartParametersV2016.unitSeparator +
                                        brand_code.ToString() + SmartParametersV2016.unitSeparator +
                                        resource_code.ToString() + SmartParametersV2016.unitSeparator +
                                        resource_type + SmartParametersV2016.unitSeparator +
                                        tariff_code.ToString();

                                    // The GUI should **NEVER** have to worry about the Schema - SmartDBServer should sort that out
                                    List<SmartUtility.TariffCodesNamesView> tariff_codes_names_viewList =
                                    await SmartBobV2017.Load_SingleList_Async<SmartUtility.TariffCodesNamesView>(ourviewmodel,
                                                                                                            utilityviewmodel,
                                                                                                            utilityviewmodel.utilityToken,
                                                                                                            "SMARTUTILITY",
                                                                                                            "LOOKUP_TARIFF_CODE",
                                                                                                            "P",
                                                                                                            P1);
                                    if (!string.IsNullOrEmpty(ourviewmodel.errorMessage))
                                    {
                                        return -1;
                                    }
                                    else
                                    {
                                        if (tariff_codes_names_viewList.Count > 0)
                                        {
                                            UtilityViewModel.TariffItem tariff_item = new UtilityViewModel.TariffItem()
                                            {
                                                Content = tariff_codes_names_viewList.First().TARIFF_NAME,
                                                Value = "",
                                                Colour = ourviewmodel.orangeColour
                                            };

                                            temp_tariffs.Add(tariff_item);
                                        }
                                    }
                                }
                                utilityviewmodel.UtilityTariffsList = temp_tariffs;
                                return tariff_index;
                            }
                        }
                    }
                }
            }
            return -1;
        }

        // BBB is at it again, huffing and puffing
        internal static string Check_Tariff_Area(MainViewModel ourviewmodel,
                                UtilityViewModel utilityviewmodel,
                                char local_resource_code,
                                short local_brand_code,
                                string local_resource_type,
                                SmartUtility.Tariffs tariff_row,
                                FieldInfo[] myFields)
        {
            string tariff_name = "";
            // Is it really necessary to return a tariff_name list when the tariff code
            // only ever appears once for a Brand or Supplier in Tariff Area Matrix??
            int field_index = SmartNibbyV2016.Area_Matrix_Index(utilityviewmodel.area_code, myFields);
            if (field_index >= 0)
            {
                List<SmartUtility.TariffMatrix> tariff_matrix_found = SmartSpikeUtilityV2017.Utility_Find_Tariff_Matrix(ourviewmodel,
                                                                                                utilityviewmodel,
                                                                                                local_brand_code,
                                                                                                local_resource_code,
                                                                                                local_resource_type,
                                                                                                tariff_row.TARIFF_CODE);
                int row_count = 0;
                foreach (SmartUtility.TariffMatrix tariff_matrix_row in tariff_matrix_found)
                {
                    if (Convert.ToChar(myFields[field_index].GetValue(tariff_matrix_row).ToString()) == SmartParametersV2016.tariffMatrixValid)
                    {
                        if (string.IsNullOrEmpty(tariff_name))
                        {
                            tariff_name = tariff_matrix_row.TARIFF_NAME;
                        }
                        row_count++;
                    }
                }
                if (row_count != tariff_matrix_found.Count)
                {
                    tariff_name = "";
                }
            }
            return tariff_name;
        }

        internal static async Task<int> Build_Payment_Plan_Dropdown(MainViewModel ourviewmodel,
                                                                UtilityViewModel utilityviewmodel,
                                                                short brand_code,
                                                                short supplier_code,
                                                                char resource_code,
                                                                string resource_type,
                                                                int tariff_code,
                                                                char target_payment_plan)
        {
            int payment_plan_index = -1,
                    loop = 0;

            if (utilityviewmodel.PaymentPlanSelectedIndex == -1)
            {
                utilityviewmodel.UtilityPaymentPlansList.Clear();
                List<UtilityViewModel.PaymentPlanItem> temp_payment_plans = new List<UtilityViewModel.PaymentPlanItem>();

                if (utilityviewmodel.Hezbollah.conditions_plansList.Count > 0)
                {
                    List<SmartUtility.AnalysisCosts> analysis_costs_found = new List<SmartUtility.AnalysisCosts>();
                    switch (resource_code)
                    {
                        case SmartParametersV2016.Electricity:
                            analysis_costs_found = utilityviewmodel.Hezbollah.e_analysis_costsList;
                            break;
                        case SmartParametersV2016.Gas:
                            analysis_costs_found = utilityviewmodel.Hezbollah.g_analysis_costsList;
                            break;
                        case SmartParametersV2016.DualFuel:
                            analysis_costs_found = utilityviewmodel.Hezbollah.d_analysis_costsList;
                            break;
                        default:
                            break;
                    }

                    // REDO THIS - WHICH DROPDOWN PAYMENTS YOU CHOOSE ACTUALLY DEPENDS ON
                    // WHICH TARIFFS YOU CHOOSE FROM WHICH SUPPLIER
                    // Yes - its now been totally and utterly re-done - including colour!

                    // This list returns 'A' for Tier2 and 'A' for Standing Charge (Tier1) hence the 'Distinct;
                    List<string> conditions_plans_found = SmartSpikeUtilityV2017.Utility_Find_PaymentPlansX(ourviewmodel,
                                                                                                utilityviewmodel,
                                                                                                supplier_code,
                                                                                                resource_code,
                                                                                                resource_type,
                                                                                                tariff_code);
                    if (conditions_plans_found.Count > 0)
                    {
                        foreach (string conditions_plans_row in conditions_plans_found)
                        {
                            
                            string payment_plans = conditions_plans_row;
                            for (int i = 0; i < payment_plans.Length; i++)
                            {
                                List<SmartUtility.PaymentPlans> payment_names_found = SmartSpikeUtilityV2017.Utility_Find_PaymentPlansY(ourviewmodel,
                                                                                                                utilityviewmodel,
                                                                                                                Convert.ToChar(payment_plans.Substring(i, 1)));
                                foreach (SmartUtility.PaymentPlans payment_name_row in payment_names_found)
                                {
                                    // Default this in even though it is irrelevant
                                    UtilityViewModel.PaymentPlanItem payment_name = new UtilityViewModel.PaymentPlanItem()
                                    {
                                        Content = payment_name_row.ALTERNATE_NAME1,
                                        Value = "",
                                        Colour = ourviewmodel.blackColour
                                    };

                                    if (analysis_costs_found.Count > 0)
                                    {
                                        if (analysis_costs_found.First().PAYMENT_PLAN.Contains(payment_name_row.PAYMENT_PLAN.ToString()))
                                        {
                                            payment_name.Colour = ourviewmodel.greenColour;
                                        }
                                        else
                                        {
                                            if (analysis_costs_found.Last().PAYMENT_PLAN.Contains(payment_name_row.PAYMENT_PLAN.ToString()))
                                            {
                                                if (analysis_costs_found.First().TOTAL_COST < analysis_costs_found[analysis_costs_found.Count - 1].TOTAL_COST)
                                                {
                                                    payment_name.Colour = ourviewmodel.redColour;
                                                }
                                            }
                                        }
                                    }
                                    temp_payment_plans.Add(payment_name);

                                    // Did we have a target?
                                    if (target_payment_plan != SmartParametersV2016.defaultChar)
                                    {
                                        // Yes - test it
                                        if (payment_name_row.PAYMENT_PLAN == target_payment_plan)
                                        {
                                            payment_plan_index = loop;
                                        }
                                    }
                                    else
                                    {
                                        // No - are we on the first?
                                        if (loop == 0)
                                        {
                                            // Yes
                                            payment_plan_index = loop;
                                        }
                                    }
                                    break;
                                }
                                loop++;
                            }
                        }
                    }
                    else
                    {
                        char remote_payment_plan = await SmartSpikeUtilityV2017.Utility_Lookup_RemotePaymentPlan(ourviewmodel,
                                                                    utilityviewmodel,
                                                                    brand_code,
                                                                    supplier_code,
                                                                    resource_code,
                                                                    resource_type,
                                                                    tariff_code);
                        if (!string.IsNullOrEmpty(ourviewmodel.errorMessage))
                        {
                            return -1;
                        }
                        List<SmartUtility.PaymentPlans> payment_names_found = SmartSpikeUtilityV2017.Utility_Find_PaymentPlansY(ourviewmodel,
                                                                                                        utilityviewmodel,
                                                                                                        remote_payment_plan);
                        foreach (SmartUtility.PaymentPlans payment_name_row in payment_names_found)
                        {
                            UtilityViewModel.PaymentPlanItem payment_name = new UtilityViewModel.PaymentPlanItem()
                            {
                                Content = payment_name_row.PAYMENT_NAME,
                                Value = "",
                                Colour = ourviewmodel.orangeColour
                            };

                            temp_payment_plans.Add(payment_name);
                            payment_plan_index = utilityviewmodel.UtilityPaymentPlansList.Count - 1;

                            break;  // Only the first
                        }
                    }
                }
                utilityviewmodel.UtilityPaymentPlansList = temp_payment_plans;
                return payment_plan_index;
            }
            return payment_plan_index;
        }

        internal static bool Lookup_Tariff_Item(MainViewModel ourviewmodel,
                                                UtilityViewModel utilityviewmodel,
                                                string tariff_item,
                                                short brand_code,
                                                char resource_code,
                                                string resource_type)
        {
            if (!string.IsNullOrEmpty(tariff_item))
            {
                utilityviewmodel.tariff_codes_names_subsetList = SmartSpikeUtilityV2017.Utility_Find_Tariff_Codes_Names(ourviewmodel,
                                                                            utilityviewmodel,
                                                                            brand_code,
                                                                            resource_code,
                                                                            resource_type);
                if (SmartSpikeUtilityV2017.Utility_Lookup_TariffCode(utilityviewmodel,
                                                        brand_code,
                                                        resource_code,
                                                        resource_type))
                {
                    //tariff_item = utilityviewmodel.TARIFF_NAME;
                    //tariff_code = utilityviewmodel.TARIFF_CODE;
                    return true;
                }
            }
            return false;
        }

        internal static bool Lookup_Brand_Item(MainViewModel ourviewmodel,
                                                UtilityViewModel utilityviewmodel,
                                                char resource_code,
                                                string brand_name)
        {
            if (!string.IsNullOrEmpty(brand_name))
            {
                if (Lookup_Brand_Code(ourviewmodel,
                                                utilityviewmodel,
                                                resource_code,
                                                brand_name))
                {
                    return true;
                }
            }
            return false;
        }

        internal static bool Lookup_Brand_Code(MainViewModel ourviewmodel,
                                                UtilityViewModel utilityviewmodel,
                                                char resource_code,
                                                string supplier_name)
        {
            // Only if its not empty and we have something in the table
            if ((!string.IsNullOrEmpty(supplier_name)) &&
                (utilityviewmodel.Hezbollah.suppliersList.Count > 0))
            {
                List<SmartUtility.Brands> brands_found = SmartSpikeUtilityV2017.Utility_Lookup_BrandName(ourviewmodel,
                                                                                        utilityviewmodel,
                                                                                        resource_code,
                                                                                        supplier_name);
                if (brands_found.Count > 0)
                {
                    utilityviewmodel.supplier_code = brands_found.First().SUPPLIER_CODE;
                    utilityviewmodel.brand_code = brands_found.First().BRAND_CODE;
                    return true;
                }
            }
            return false;
        }

        internal static bool Lookup_Consumer_Info(MainViewModel ourviewmodel,
                                                UtilityViewModel utilityviewmodel,
                                                string brand_name)
        {
            utilityviewmodel.supplier_code = 0;
            utilityviewmodel.brand_code = 0;
            // If we are simulating, then stop ...
            char local_resource_code = Utility_Find_Resource_Code(utilityviewmodel);
            utilityviewmodel.resource_code = local_resource_code;

            if (utilityviewmodel.SupplierSelectedIndex != -1)
            {
                if (Lookup_Brand_Item(ourviewmodel,
                                    utilityviewmodel,
                                    local_resource_code,
                                    brand_name))
                {
                    // Find the UDPRN
                    switch (local_resource_code)
                    {
                        case SmartParametersV2016.Electricity:
                            if (utilityviewmodel.UtilityAddressesList.Count > 0)
                            {
                                foreach (MainViewModel.AddressItem address_item in utilityviewmodel.UtilityAddressesList)
                                {
                                    utilityviewmodel.udprn = address_item.Value;
                                    break;
                                }
                            }
                            break;
                        case SmartParametersV2016.Gas:
                            if (utilityviewmodel.UtilityAddressesList.Count > 0)
                            {
                                foreach (MainViewModel.AddressItem address_item in utilityviewmodel.UtilityAddressesList)
                                {
                                    utilityviewmodel.udprn = address_item.Value;
                                    break;
                                }
                            }
                            break;
                        case SmartParametersV2016.DualFuel:
                            if (utilityviewmodel.UtilityAddressesList.Count > 0)
                            {
                                foreach (MainViewModel.AddressItem address_item in utilityviewmodel.UtilityAddressesList)
                                {
                                    utilityviewmodel.udprn = address_item.Value;
                                    break;
                                }
                            }
                            break;
                        default:
                            break;
                    }
                    char temp_resource_code = local_resource_code;
                    List<SmartUtility.Switches> switches_found =
                            SmartSpikeUtilityV2017.Utility_Find_Switches_UDPRN(ourviewmodel,
                                                                               utilityviewmodel,
                                                                                temp_resource_code);
                    if (switches_found.Count > 0)
                    {
                        foreach (SmartUtility.Switches switches_row in switches_found)
                        {
                            utilityviewmodel.supplier_code = switches_row.SUPPLIER_CODE;
                            utilityviewmodel.brand_code = switches_row.BRAND_CODE;

                            // Re-do this RAY!!

                            //last_date = switches_row.LAST_DATETIME;

                            //if (switches_row.AUTOSWITCH == SmartParametersV2016.activeFlag)
                            //{
                            //    autoswitch = true;
                            //}
                            return true;  // only the first
                        }
                    }
                }
            }
            return false;
        }

        internal static bool Manipulate_Test(bool on_or_off)
        {
            switch (on_or_off)
            {
                case true:
                    break;
                case false:
                    return false;
            }
            return true;
        }

        // This is a REAL fucking kludge!
        internal static async Task<bool> Cancel_Utility_Meter_Async(MainViewModel ourviewmodel,
                                                                    UtilityViewModel utilityviewmodel,
                                                                    char first_resource_code,
                                                                    SmartProfile.Cubefaces cubeface_row)
        {
            // So - write this assuming you have two scenarios:
            // 1. Different Suppliers / User Ids and Passwords
            //    which implies different LAST_DATETIMEs and NEXT_CONNECTIONs
            //
            //    For each Supplier pull out the User Id, Password and
            //    Next Connection.
            //
            // 2. Same Suppliers which implies Same User Ids and Passwords
            //    which also implies same NEXT_CONNECTIONs (the LAST_DATETIMEs might
            //    still indeed be different for Electricity and Gas
            //
            // How are you going to code this?
            // For all the Consumer Energy records, check the Next Connections
            //
            //  If you find none that needs SUBMITting then exit
            //  If you find one that needs SUBMITting, then check its Supplier with the
            //  other one (which doesn't need SUBMITting).
            //      If the Suppliers are the same adjust the non-SUBMIT down to the SUBMIT
            //      so that both will get processed and updated
            //      If the Suppliers aren't the same, then leave the non-SUBMIT alone
            //  If you find both need SUBMITting then check the Suppliers.
            //      If the are equal, then its a classic dual-tariff and do both in one
            //      read of the Supplier
            //      If they are different, then do them in order of Consumer Energy record
            //      sequence
            int returned_codes;

            short area_code = 0,
                    brand_code = 0;
            char second_resource_code;
            string //first_mpan_mprn,
                    second_mpan_mprn;

            // We never come in from the Clock we always come in from a Cancel
            // From the Cancel - we can do this with ZERO Consumer Energy Records

            List<SmartUtility.Meters> first_mpan_mprnList;
            // Examine all we can find
            // Always a CANCEL?
            switch (first_resource_code)
            {
                case SmartParametersV2016.Electricity:
                    first_mpan_mprnList = SmartSpikeUtilityV2017.Utility_Lookup_MetersByResource(ourviewmodel,
                                                                        utilityviewmodel,
                                                                        first_resource_code);
                    break;
                case SmartParametersV2016.Gas:
                    first_mpan_mprnList = SmartSpikeUtilityV2017.Utility_Lookup_MetersByResource(ourviewmodel,
                                                                        utilityviewmodel,
                                                                        first_resource_code);
                    break;
                default:
                    break;
            }

            List<SmartUtility.Switches> switches_found = SmartSpikeUtilityV2017.Utility_Lookup_Switches(ourviewmodel, utilityviewmodel);

            // Make sure we do Suppliers and their Utility.Accounts one after the other
            if (switches_found.Count > 0)
            {
                DateTime next_conn = (DateTime.Now + ourviewmodel.utcOffset).AddDays(1);   // Local time for when we fix next connections
                foreach (SmartUtility.Switches switches_row in switches_found)
                {
                    // Should be doing the latest here ...
                    //area_code = utilityviewmodel.Hezbollah.List.First().AREA_CODE;
                    brand_code = switches_found.First().BRAND_CODE;

                    // Despite everything and all the pain
                    // your program is ABSOLUTELY beautiful Ray, and its ALL YOURS!
                    //string urgent_message = "";
                    //switches_row.NEXT_CONNECTION = next_conn;
                    if (!string.IsNullOrEmpty(switches_row.ToString()))
                    {
                        switches_row.Updated = true;
                        utilityviewmodel.Hezbollah.utility_switches_changesList.Add(switches_row);
                        if (!await SmartUtilityScrapeV2022.DoAllSmartUtility(ourviewmodel,
                                                        utilityviewmodel,
                                                        false,
                                                        SmartParametersV2016.sqliteformat,
                                                        switches_found.First().SUPPLIER_CODE,
                                                        switches_found.First().BRAND_CODE))
                        {
                            if (!await SmartRoutinesV2018.CheckTrace(ourviewmodel, utilityviewmodel.utilityToken, switches_found.First().SUPPLIER_CODE, switches_found.First().BRAND_CODE, ourviewmodel.errorMessage))
                            {
                                return false;
                            }
                            // Led8 turned to Red on return
                            return false;
                        }
#if WINFORMS || WPF  || WINUI || SMARTMAUI
                        utilityviewmodel.NextConnectionMessage = cubeface_row.NEXT_CONNECTION.ToString(utilityviewmodel.CultureINF);
#endif
#if ANDROIDX
                        utilityviewmodel.NextConnectionMessage.Text = cubeface_row.NEXT_CONNECTION.ToString(utilityviewmodel.CultureINF);
#endif
                        // Try and find the same User Id for this Supplier
                        // on the opposite Energy Code.  Even if the SUpplier is
                        // used on more than one record, its the User Id access
                        // we want to Cancel
                        List<SmartUtility.Meters> second_mpan_mprnList = new List<SmartUtility.Meters>();
                        switch (first_resource_code)
                        {
                            case SmartParametersV2016.Electricity:
                                second_resource_code = SmartParametersV2016.Gas;
                                second_mpan_mprnList = first_mpan_mprnList = SmartSpikeUtilityV2017.Utility_Lookup_MetersByResource(ourviewmodel,
                                                                            utilityviewmodel,
                                                                            second_resource_code);
                                break;
                            case SmartParametersV2016.Gas:
                                second_resource_code = SmartParametersV2016.Electricity;
                                second_mpan_mprnList = first_mpan_mprnList = SmartSpikeUtilityV2017.Utility_Lookup_MetersByResource(ourviewmodel,
                                                                            utilityviewmodel,
                                                                            second_resource_code);
                                break;
                            default:
                                break;
                        }

                        foreach (SmartUtility.Meters m_row in second_mpan_mprnList)
                        {
                            second_mpan_mprn = m_row.MPAN_MPRN;
                            break;
                        }
                        List<SmartUtility.Switches> utility2_found = SmartSpikeUtilityV2017.Utility_Consumer_CancelList(ourviewmodel,
                                                                                                                    utilityviewmodel);


                        if (utility2_found.Count > 0)
                        {
                            foreach (SmartUtility.Switches utility2_row in utility2_found)
                            {
                                cubeface_row.NEXT_CONNECTION = next_conn;
                                if (!string.IsNullOrEmpty(switches_row.ToString()))
                                {
                                    utility2_row.Updated = true;
                                    utilityviewmodel.Hezbollah.utility_switches_changesList.Add(utility2_row);
                                    if (!await SmartUtilityScrapeV2022.DoAllSmartUtility(ourviewmodel,
                                                                    utilityviewmodel,
                                                                    false,
                                                                    SmartParametersV2016.sqliteformat,
                                                                    switches_row.SUPPLIER_CODE,
                                                                    switches_row.BRAND_CODE))
                                    {
                                        if (!await SmartRoutinesV2018.CheckTrace(ourviewmodel, utilityviewmodel.utilityToken, switches_row.SUPPLIER_CODE, switches_row.BRAND_CODE, ourviewmodel.errorMessage))
                                        {
                                            return false;
                                        }
                                        return false;
                                    }
                                }
                                break;  // Only the latest
                            }
                        }
                    }
                    break; // Only the latest
                }
            }
            //}
            // Led6 turned to Orange on return
            returned_codes = brand_code + (area_code * 256);
            utilityviewmodel.returned_codes = returned_codes;
            return true;
        }

        internal static bool Lookup_Utility_Supplier_Code(MainViewModel ourviewmodel,
                                                UtilityViewModel utilityviewmodel,
                                                char resource_code,
                                                short brand_code)
        {
            List<SmartUtility.Brands> brands_found = SmartSpikeUtilityV2017.Utility_Lookup_BrandName(ourviewmodel,
                                                            utilityviewmodel,
                                                            resource_code,
                                                            "");
            if (brands_found.Count > 0)
            {
                foreach (SmartUtility.Brands brand_row in brands_found)
                {
                    if (brand_row.BRAND_CODE == brand_code)
                    {
                        utilityviewmodel.supplier_code = brand_row.SUPPLIER_CODE; // Only the first - only ONE Supplier per Brand *Brands are unique!!)
                        return true;
                    }
                }
            }
            return false;
        }

#if WINFORMS
        internal static System.Drawing.Image Lookup_LOGO(MainViewModel ourviewmodel,
                                                                    UtilityViewModel utilityviewmodel,
                                                                    char cubefaceCode,
                                                                    string supply_dno)

#endif
#if WPF  || WINUI
        internal static ImageSource Lookup_LOGO(MainViewModel ourviewmodel,
                                                                    UtilityViewModel utilityviewmodel,
                                                                    char cubefaceCode,
                                                                    string supply_dno)
#endif
#if ANDROIDX
        internal static ImageView Lookup_LOGO(
                                                AppCompatActivity meterActivity,
                                                MainViewModel ourviewmodel,
                                                                    UtilityViewModel utilityviewmodel,
                                                                    char cubefaceCode,
                                                                    string supply_dno)
#endif
#if SMARTMAUI
        internal static ImageSource Lookup_LOGO(MainViewModel ourviewmodel,
                                                                    UtilityViewModel utilityviewmodel,
                                                                    char cubefaceCode,
                                                                    string supply_dno)
#endif
        {
#if WINFORMS
            // This is needed to initialize it properly
            System.Drawing.Image logo = new Bitmap(1, 1);
#endif
#if WPF  || WINUI
            BitmapImage logo = new BitmapImage();
#endif
#if ANDROIDX
            ImageView logo = new ImageView(meterActivity);
#endif
#if SMARTMAUI
            ImageSource logo = null;
#endif
            if (!string.IsNullOrEmpty(supply_dno))
            {
                foreach (MainViewModel.ImageItem itemView in ourviewmodel.ImageList)
                {
                    if (itemView.Cubeface == cubefaceCode &&
                        itemView.ImageName == supply_dno)
                    {
                        //
                        // My God in Heaven, this stuff is SHIT
                        //                                 ====
                        // For reasons best know to the Chimps, when you
                        // re-read a Stream the position is at the END!!
#if WINFORMS
                        logo = Image.FromStream(itemView.StreamBits);
#endif
#if WPF 

#if WPF
                        itemView.StreamBits.Position = 0;
                        logo.BeginInit();
                        logo.StreamSource = itemView.StreamBits;//   new Uri("your-image.jpg", UriKind.Relative);
                        logo.DecodePixelWidth = 100; // match display size
                        logo.CacheOption = BitmapCacheOption.OnLoad;
                        logo.EndInit();
#endif
#endif
#if WINUI
                        // Conversion from one stream to another (yawn)
                        InMemoryRandomAccessStream randomaccessStream = new InMemoryRandomAccessStream();
                        using (System.IO.Stream outputStream = randomaccessStream.AsStreamForWrite())
                        {
                            itemView.StreamBits.CopyTo(outputStream);
                            outputStream.FlushAsync();
                        }
                        randomaccessStream.Seek(0); // Reset position
                        logo.SetSource(randomaccessStream);
#endif

#if ANDROIDX
                        Android.Graphics.Bitmap bmap = BitmapFactory.DecodeStream(itemView.StreamBits);
                        logo.SetImageBitmap(bmap);
#endif
#if SMARTMAUI
                        logo = ImageSource.FromStream(() =>
                        {
                            var ms = new MemoryStream();
                            itemView.StreamBits.Position = 0;
                            itemView.StreamBits.CopyTo(ms);
                            ms.Position = 0;
                            return ms;
                        });
#endif
                        return logo;
                    }
                }
            }
            return null;
        }

#if WINFORMS
        internal static async Task<System.Drawing.Image> Download_LOGOAsync(MainViewModel ourviewmodel,
                                                                    UtilityViewModel utilityviewmodel,
                                                                    char cubefaceCode,
                                                                    string supply_dno)

#endif
#if WPF  || WINUI
        internal static async Task<ImageSource> Download_LOGOAsync(MainViewModel ourviewmodel,
                                                                    UtilityViewModel utilityviewmodel,
                                                                    char cubefaceCode,
                                                                    string supply_dno)
#endif
#if ANDROIDX
        internal static async Task<ImageView> Download_LOGOAsync(
                                                                AppCompatActivity meterActivity,
                                                                MainViewModel ourviewmodel,
                                                                    UtilityViewModel utilityviewmodel,
                                                                    char cubefaceCode,
                                                                    string supply_dno)
#endif
#if SMARTMAUI
        internal static async Task<ImageSource> Download_LOGOAsync(MainViewModel ourviewmodel,
                                                                    UtilityViewModel utilityviewmodel,
                                                                    char cubefaceCode,
                                                                    string supply_dno)
#endif

        {
#if WINFORMS
            // This is needed to initialize it properly
            System.Drawing.Image logo = new Bitmap(1, 1);
#endif
#if WPF  || WINUI
            BitmapImage logo = new BitmapImage();
#endif
#if ANDROIDX
            ImageView logo = new ImageView(meterActivity);
#endif
#if SMARTMAUI
            ImageSource logo = null;
#endif
            // This SHOULD never fail ...
            if (!string.IsNullOrEmpty(supply_dno))
            {
                if (await SmartBobV2017.Load_DNO_Async(ourviewmodel,
                                                            utilityviewmodel,
                                                            utilityviewmodel.utilityToken,
                                                            cubefaceCode,
                                                            supply_dno))
                {
                    if (utilityviewmodel.dno_stream != null)
                    {
                        MainViewModel.ImageItem itemView = new MainViewModel.ImageItem()
                        {
                            Cubeface = cubefaceCode,
                            ImageName = supply_dno,
                            StreamBits = utilityviewmodel.dno_stream
                        };

                        ourviewmodel.ImageList.Add(itemView);
                        itemView.StreamBits.Position = 0;
#if WINFORMS
                        logo = System.Drawing.Image.FromStream(utilityviewmodel.dno_stream);

#endif
#if WPF

                        logo.BeginInit();
                        logo.StreamSource = itemView.StreamBits;//   new Uri("your-image.jpg", UriKind.Relative);
                        logo.DecodePixelWidth = 100; // match display size
                        logo.CacheOption = BitmapCacheOption.OnLoad;
                        logo.EndInit();
#endif
#if WINUI
                        // Conversion from one stream to another (yawn)
                        InMemoryRandomAccessStream randomaccessStream = new InMemoryRandomAccessStream();
                        using (System.IO.Stream outputStream = randomaccessStream.AsStreamForWrite())
                        {
                            await itemView.StreamBits.CopyToAsync(outputStream);
                            await outputStream.FlushAsync();
                        }
                        randomaccessStream.Seek(0); // Reset position
                        await logo.SetSourceAsync(randomaccessStream);
#endif
#if ANDROIDX
                        Android.Graphics.Bitmap bmap = BitmapFactory.DecodeStream(itemView.StreamBits);
                        logo.SetImageBitmap(bmap);
#endif
#if SMARTMAUI
                        logo = ImageSource.FromStream(() =>
                        {
                            var ms = new MemoryStream();
                            itemView.StreamBits.Position = 0;
                            itemView.StreamBits.CopyTo(ms);
                            ms.Position = 0;
                            return ms;
                        });
#endif
                    }
                }
            }
            return logo;
        }

        internal static bool Build_Utility_UDPRN_Dropdown(MainViewModel ourviewmodel,
                                                UtilityViewModel utilityviewmodel,
                                                string udprn,
                                                string address)
        {
            utilityviewmodel.udprn_index = -1;

            List<MainViewModel.AddressItem> temp_addresses = new List<MainViewModel.AddressItem>();

            if (!string.IsNullOrEmpty(udprn))
            {

                if (utilityviewmodel.UtilityAddressesList.Count > 0)
                {
                    int loop = 0;

                    foreach (MainViewModel.AddressItem address_item in utilityviewmodel.UtilityAddressesList) // <== The paki is right for the first time
                    {
                        if (udprn == address_item.Value.ToString())
                        {
                            utilityviewmodel.udprn_index = loop;
                            break;
                        }
                        loop++;
                    }
                    if (utilityviewmodel.udprn_index == -1)
                    {
                        MainViewModel.AddressItem udprn_item = new MainViewModel.AddressItem()
                        {
                            Content = address,
                            Value = udprn,
                            Colour = ourviewmodel.blackColour
                        };

                        temp_addresses.Add(udprn_item);
                        utilityviewmodel.udprn_index = temp_addresses.Count - 1;
                    }
                }
                else
                {
                    MainViewModel.AddressItem udprn_item = new MainViewModel.AddressItem()
                    {
                        Content = address,
                        Value = udprn,
                        Colour = ourviewmodel.blackColour
                    };

                    temp_addresses.Add(udprn_item);
                    utilityviewmodel.udprn_index = 0;
                }
                utilityviewmodel.UtilityAddressesList = temp_addresses;
                //udprn_index_new = udprn_index;
                return true;
            }
            return false;
        }

        internal static char Utility_Find_Resource_Code(UtilityViewModel utilityviewmodel)
        {
            if (utilityviewmodel != null)
            {
                if (utilityviewmodel.ElectricityChecked)
                {
                    return Convert.ToChar(utilityviewmodel.ButtonElectricity);
                }
                else
                {
                    if (utilityviewmodel.GasChecked)
                    {
                        return Convert.ToChar(utilityviewmodel.ButtonGas);
                    }
                    else
                    {
                        if (utilityviewmodel.DualFuelChecked)
                        {
                            return Convert.ToChar(utilityviewmodel.ButtonDualFuel);
                        }
                    }
                }
            }
            return SmartParametersV2016.defaultUtilityResourceCode;
        }

        internal static async Task<bool> ButtonAutoSwitchUtilityClickActual(SignInViewModel signinviewmodel,
                                                                        MainViewModel ourviewmodel,
                                                                        UtilityViewModel utilityviewmodel)
        {
            // One day .... 
            //         ... well THAT DAY is 28-May-2017!!!
            //string urgent_message = "";
            if (ourviewmodel.SmartProfile.profilecubefacesList.Count > 0)
            {
                char toggle = SmartParametersV2016.activeDefault;
                // Set the toggle switch to 'On'

#if WINFORMS || WPF  || WINUI || SMARTMAUI
                if (utilityviewmodel.AutoSwitchUtilityColour == ourviewmodel.darkorangeColour)
#endif
#if ANDROIDX
                if (utilityviewmodel.AutoSwitchUtilityColour == ourviewmodel.darkorangeColour)
#endif
                {
                    toggle = SmartParametersV2016.activeFlag;
                }

                foreach (SmartProfile.Cubefaces cubeface_row in ourviewmodel.SmartProfile.profilecubefacesList)
                {
                    bool do_something = Toggle_AutoSwitch(cubeface_row, toggle);
                    if (do_something)
                    {
                        SmartUtility.Resources resource_row = SmartSpikeUtilityV2017.Utility_Extract_MatchingResources(ourviewmodel,
                                                                                                                      utilityviewmodel);//,
                                                                                                                                        //ourviewmodel.UserName);
                                                                                                                                        //consumers_view_row.SUPPLIER_CODE,
                                                                                                                                        //consumers_view_row.BRAND_CODE);
                                                                                                                                        //consumers_view_row.ACCOUNT_CREATED,
                                                                                                                                        //consumers_view_row.ACCOUNT_NO,
                                                                                                                                        //consumers_view_row.MPAN_MPRN);
                        if (!string.IsNullOrEmpty(resource_row.ToString()))
                        {
                            resource_row.AUTOSWITCH = cubeface_row.FACE_AUTOSWITCH;
                            resource_row.Updated = true;
                            utilityviewmodel.Hezbollah.utility_resources_changesList.Add(resource_row);
                            if (!await SmartUtilityScrapeV2022.DoAllSmartUtility(ourviewmodel,
                                                            utilityviewmodel,
                                                            false,
                                                            SmartParametersV2016.sqliteformat))
                            {
                                if (!(ourviewmodel.quitCts.IsCancellationRequested ||
                                        utilityviewmodel.utilityToken.IsCancellationRequested))
                                {
                                    // Nothing ever turns this back from Red
                                    FrontEndGUI.SetLedColour(ourviewmodel, 8, ourviewmodel.redColour);
                                    if (!await SmartRoutinesV2018.CheckTrace(ourviewmodel, utilityviewmodel.utilityToken, 0, 0, "Problem 63: Updating Resource"))
                                    {
                                        return false;
                                    }
                                }
                                return false;
                            }
                        }
                    }
                }
                // DarkRed turned to darkorange
                if (toggle == SmartParametersV2016.activeFlag)
                {
                    utilityviewmodel.AutoSwitchUtilityColour = ourviewmodel.darkorangeColour;
#if ANDROIDX
                    ourviewmodel.AutoSwitch0 = ourviewmodel.openred0;
                    ourviewmodel.AutoSwitch25 = ourviewmodel.openred25;
                    ourviewmodel.AutoSwitch50 = ourviewmodel.openred50;
                    ourviewmodel.AutoSwitch75 = ourviewmodel.openred75;
#endif
                }
                else
                {
                    utilityviewmodel.AutoSwitchUtilityColour = ourviewmodel.darkgreenColour;
#if ANDROIDX
                    ourviewmodel.AutoSwitch0 = ourviewmodel.autoswitch0;
                    ourviewmodel.AutoSwitch25 = ourviewmodel.autoswitch25;
                    ourviewmodel.AutoSwitch50 = ourviewmodel.autoswitch50;
                    ourviewmodel.AutoSwitch75 = ourviewmodel.autoswitch75;
#endif
                }
            }
            return true;
        }

        internal static bool Toggle_AutoSwitch(SmartProfile.Cubefaces cubeface_row,
                                                char toggle)
        {
            cubeface_row.FACE_AUTOSWITCH = toggle;
            return true;
        }

#if WINFORMS || WPF
        internal static bool Utility_ButtonExchangeRatesClick_Actual(MainViewModel ourviewmodel,
                                                                    UtilityViewModel utilityviewmodel)
#endif
#if WINUI
        internal static async Task<bool> Utility_ButtonExchangeRatesClick_Actual(ContentDialog contentpage,
                                                                    MainViewModel ourviewmodel,
                                                                    UtilityViewModel utilityviewmodel)
#endif
#if ANDROIDX
        internal static bool Utility_ButtonExchangeRatesClick_Actual(PopupWindow contentpage,
                                                                    MainViewModel ourviewmodel,
                                                                    UtilityViewModel utilityviewmodel)
#endif
#if SMARTMAUI
        internal static async Task<bool> Utility_ButtonExchangeRatesClick_Actual(MainViewModel ourviewmodel,
                                                                    UtilityViewModel utilityviewmodel)
#endif
        {
            try
            {
                utilityviewmodel.PopupHeight = ourviewmodel.currentHeight - ourviewmodel.currentHeight / 10;
                //#if Z!(WPF || WINFORMS)    // For the time being until we build a popup
                //                UtilityExchangeRatesPopup exchangerates_popup = new UtilityExchangeRatesPopup();
                //#endif

#if WINUI
                await contentpage.ShowAsync();
#endif
#if ANDROIDX
                Context context = null; // Yes I know this wont work
                View popupView = LayoutInflater.From(context)
                               .Inflate(Resource.Layout.UtilityExchangeRates, null);

                PopupWindow popupWindow = new PopupWindow(
                    popupView,
                    ViewGroup.LayoutParams.WrapContent,
                    ViewGroup.LayoutParams.WrapContent,
                    true);

                popupWindow.ShowAtLocation(
                    popupView,  // Should be parent view
                    GravityFlags.Center,
                    0,
                    0);

                // For the time being until we build a popup
                //UtilityExchangeRatesPopup exchangerates_popup = new UtilityExchangeRatesPopup();
                // Needs fixing
                //await exchangerates_popup.ShowAtLocation(this);
#endif
#if SMARTMAUI
                UtilityExchangeRatesPopup exchangerates_popup = new UtilityExchangeRatesPopup();
                await Shell.Current.ShowPopupAsync(exchangerates_popup);
#endif
                // What an absolute fucking bitch all this DataGridView stuff is ..
                // When you copy the ItemSource, the Rows are copied ... but the Columns aren't!!                            
            }
            catch (Exception ex)
            {
                utilityviewmodel.errorMessage = ex.Message;
                return false;
            }
            return true;
        }

        internal static void Cycle_Round(MainViewModel ourviewmodel,
                                UtilityViewModel utilityviewmodel,
                                char resource_code,
                                DateTime lower_date_time,
                                DateTime upper_date_time,
                                DateTime target_date_time)
        {
            DateTime read_date;

            List<SmartUtility.TariffEngine> tariff_engineList = new List<SmartUtility.TariffEngine>();
            switch (resource_code)
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

            foreach (SmartUtility.TariffEngine tariff_row in tariff_engineList)
            {
                read_date = tariff_row.PERIOD_END;
                if (read_date >= lower_date_time && read_date <= upper_date_time)
                {
                    // We are range and therfore in business
                    int analysis_cost = Convert.ToInt32(tariff_row.UNITS_COST + tariff_row.CHARGES_COST);
                    // Date is within range - and a target has been established?
                    // So check it....
                    switch (SmartRoutinesV2018.DateTimeCompare(read_date, target_date_time))
                    {
                        case -1:
                            // Read date is less than target date
                            utilityviewmodel.total_simulate_readings = tariff_row.UNITS_TOTAL;  // Already accumulated
                            utilityviewmodel.total_simulate_cost += analysis_cost;
                            break;
                        case 0:
                            // Read date equals target date
                            utilityviewmodel.Hezbollah.simulateList.Add(new SmartUtility.Simulate(ourviewmodel.UserName,
                                                                        read_date,
                                                                        utilityviewmodel.total_simulate_readings,
                                                                        utilityviewmodel.total_simulate_cost));
                            // Update the target

                            //if (utilityviewmodel.RadioOneDayChecked == true)
                            //{
                            //    target_date_time = target_date_time.AddDays(1);
                            //}
                            //if (utilityviewmodel.RadioOneMonthChecked == true)
                            //{
                            //    target_date_time = target_date_time.AddMonths(1);
                            //}
                            //if (utilityviewmodel.RadioOneYearChecked == true)
                            //{
                            //    target_date_time = target_date_time.AddYears(1);
                            //}

                            // Start counting anew
                            utilityviewmodel.total_simulate_readings = tariff_row.UNITS_TOTAL;   // Already accumulated
                            utilityviewmodel.total_simulate_cost += analysis_cost;
                            break;
                        case 1:
                            // Read date is greater than target date
                            break;
                        default:
                            break;
                    }
                }
            }
            return;
        }

        internal static async Task<bool> Utility_ButtonCancelClick_Actual(MainViewModel ourviewmodel,
                                                                            UtilityViewModel utilityviewmodel)
        {
            if (!FrontEndGUI.CompareLedColour(ourviewmodel, 6, ourviewmodel.redColour))
            {
                utilityviewmodel.returned_codes = 0;

                //AddressItem abc = (AddressItem)utilityviewmodel.UtilityAddressesList[utilityviewmodel.AddressSelectedIndex];

                SmartProfile.Cubefaces cubeface_row = new SmartProfile.Cubefaces(); // <= Fix this Ray
                if (!await Cancel_Utility_Meter_Async(ourviewmodel,
                                                utilityviewmodel,
                                                utilityviewmodel.resource_code,
                                                cubeface_row))
                {
                    if (!(ourviewmodel.quitCts.IsCancellationRequested ||
                            utilityviewmodel.utilityToken.IsCancellationRequested))
                    {
                        FrontEndGUI.SetLedColour(ourviewmodel, 8, ourviewmodel.redColour);
                        if (!await SmartRoutinesV2018.CheckTrace(ourviewmodel, utilityviewmodel.utilityToken, 0, 0, "CancelMeter"))
                        {
                            return false;
                        }
                    }
                    return false;
                }
                else
                {
                    // Set this
                    FrontEndGUI.SetLedColour(ourviewmodel, 6, ourviewmodel.whiteColour);
                    // Re-enable this
                    utilityviewmodel.SubmitStatus = true;

                    // This is a real kludge ...
                    int area_code = utilityviewmodel.returned_codes / 256;
                    int brand_code = utilityviewmodel.returned_codes - (area_code * 256);
                    utilityviewmodel.supplier_code = 0;
                    Lookup_Utility_Supplier_Code(ourviewmodel,
                                                        utilityviewmodel,
                                                        utilityviewmodel.resource_code,
                                                        (short)brand_code);
                    utilityviewmodel.utility_brand_index = -1;

                    string resource_type = Check_Economy7(utilityviewmodel.Economy7State);
                    if (!Rebuild_USD(ourviewmodel,
                                    utilityviewmodel,
                                    resource_type,
                                    (utilityviewmodel.resource_code == SmartParametersV2016.defaultResourceCode ? SmartParametersV2016.activeFlag : SmartParametersV2016.activeDefault),  // This might be an initial setup

                                    (short)brand_code))
                    {
                        utilityviewmodel.UtilitySuppliersList.Clear();
                    }
                    else
                    {
                        // Inserted by RAY as part of the 'cleanup'
                        utilityviewmodel.SupplierSelectedIndex = utilityviewmodel.utility_brand_index;
                    }
                }
            }

            if (!FrontEndGUI.CompareLedColour(ourviewmodel, 7, ourviewmodel.redColour))
            {
                // Reset this
                FrontEndGUI.SetLedColour(ourviewmodel, 7, ourviewmodel.orangeColour);
            }
            // Last but not least let the popup happen
            utilityviewmodel.autoswitch_pending = false;
            return true;
        }

        //internal static bool Toggle_AutoSwitch(SmartUtility.Resources resources_row,
        //                                char toggle)
        //{
        //    resources_row.AUTOSWITCH = toggle;
        //    return true;
        //}

        // Despite everything and all the pain
        // your program is ABSOLUTELY beautiful Ray, and its ALL YOURS!
        // But RESOURCE CODE MUST NEVER BE <blank> OR NOTHING FUCKING WORKS!!

        internal static bool Utility_Some_Checks(MainViewModel ourviewmodel,
                                        UtilityViewModel utilityviewmodel,
                                        short comparison_supplier_code,
                                        short comparison_brand_code,
                                        int comparison_tariff_code)
        //string comparison_resource_type,
        {
            // This is BETTER than the original which never checked for
            // consumer_energy_found == 0 !!!
            if ((comparison_supplier_code > 0) &&
                (comparison_brand_code > 0) &&
                (comparison_tariff_code > 0))
            {
                // Very important for Smart_Analyze we get the CORRECT displayed supplier id
                List<SmartUtility.Resources> resources_found = SmartSpikeUtilityV2017.Utility_Find_View_LAST_CHECKED(ourviewmodel,
                                                                                                                                    utilityviewmodel);
                if (resources_found.Count > 0)
                {
                    // Do the first - if we LOOP, then UNIT_RATES gets done more than once !!!
                    // 'cos that is how AWAIT works ...
                    //utilityviewmodel.area_code = consumers_view_found.First().AREA_CODE;
                    //if (utilityviewmodel.area_code > 0)
                    //{
                    // Find which resource code is in the display
                    utilityviewmodel.local_resource_code = resources_found.First().RESOURCE_CODE; // SmartSpikeUtilityV2017.Lookup_Meters_Resource_Code(resources_row.CATEGORY_CODE,
                                                                                                  //       resources_row.MPAN_MPRN,
                                                                                                  //       Hamas.metersList);
                                                                                                  //supplier_code = resources_found.First().SUPPLIER_CODE;
                                                                                                  //brand_code = resources_found.First().BRAND_CODE;

                    // Re-do this RAY!!

                    //if (!string.IsNullOrEmpty(consumers_view_found.First().AGE_60PLUS))
                    //{
                    //    age = 60;
                    //}
                    //resource_type = resources_found.First().RESOURCE_TYPE;
                    return true;
                    //}
                }
            }
            return false;
        }

        internal static void Utility_Tariff_Switch(MainViewModel ourviewmodel,
                                            UtilityViewModel utilityviewmodel,
                                            char local_resource_code,
                                            string comparison_supplier_name,
                                            short comparison_supplier_code,
                                            int comparison_tariff_code,
                                            string comparison_resource_type)
        {
            switch (local_resource_code)
            {
                case SmartParametersV2016.Electricity:
#if WINFORMS
                    DateTime estartdate = utilityviewmodel.StartDate;//.GetValueOrDefault(SmartParametersV2016.defaultDate);

#endif
#if WPF  || WINUI || SMARTMAUI
                    DateTime estartdate = utilityviewmodel.StartDate;//.SelectedDate.GetValueOrDefault(SmartParametersV2016.defaultDate);

#endif
                    utilityviewmodel.tariff_report = SmartReportV2016.Fill_Tariff_Report(ourviewmodel,
                                                                        utilityviewmodel,
                                                                        local_resource_code,
                                                                        comparison_supplier_name,
                                                                        comparison_supplier_code,
                                                                        comparison_tariff_code,
                                                                        comparison_resource_type,
#if WINFORMS || WPF  || WINUI || SMARTMAUI
                                                                        estartdate
#endif
#if ANDROIDX
                                                                        utilityviewmodel.StartDate.DateTime
#endif
                                                                        );
                    break;
                case SmartParametersV2016.Gas:
#if WINFORMS
                    DateTime gstartdate = utilityviewmodel.StartDate;//.SelectedDate.GetValueOrDefault(SmartParametersV2016.defaultDate);

#endif
#if WPF  || WINUI || SMARTMAUI
                    DateTime gstartdate = utilityviewmodel.StartDate;//.SelectedDate.GetValueOrDefault(SmartParametersV2016.defaultDate);

#endif
                    utilityviewmodel.tariff_report = SmartReportV2016.Fill_Tariff_Report(ourviewmodel,
                                                                        utilityviewmodel,
                                                                        local_resource_code,
                                                                        comparison_supplier_name,
                                                                        comparison_supplier_code,
                                                                        comparison_tariff_code,
                                                                        comparison_resource_type,
#if WINFORMS || WPF  || WINUI || SMARTMAUI
                                                                        gstartdate
#endif
#if ANDROIDX
                                                                        utilityviewmodel.StartDate.DateTime
#endif
                                                                        );
                    break;
                default:
                    break;
            }
            return;
        }

        internal static bool Cellsdc_Analyze_Check(UtilityViewModel utilityviewmodel,
                                        char local_resource_code,
                                        DateTime[] e_engine_dates,
                                        DateTime[] g_engine_dates)
        {
            switch (local_resource_code)
            {
                case SmartParametersV2016.Electricity:
                    // We have done DISPLAY_METER so we SHOULD ALWAYS HAVE THE RATES??
                    // Not always - why should we?
                    if (utilityviewmodel.Hezbollah.e_unit_ratesList.Count > 0)
                    {
                        utilityviewmodel.from_date = e_engine_dates[0];
                        utilityviewmodel.to_date = e_engine_dates[1];
                        return true;
                    }
                    break;
                case SmartParametersV2016.Gas:
                    // We have done DISPLAY_METER so we SHOULD ALWAYS HAVE THE RATES??
                    if (utilityviewmodel.Hezbollah.g_unit_ratesList.Count > 0)
                    {
                        utilityviewmodel.from_date = g_engine_dates[0];
                        utilityviewmodel.to_date = g_engine_dates[1];
                        return true;
                    }
                    break;
                default:
                    break;
            }
            return false;
        }

        internal static bool Utility_Check_Analysis_Costs(SmartUtility.AnalysisCostsView analysis_costs_row)
        {
            if (analysis_costs_row.SUPPLIER_CODE > 0 &&
                analysis_costs_row.BRAND_CODE > 0 &&
                !string.IsNullOrEmpty(analysis_costs_row.SUPPLIER_NAME) &&
                analysis_costs_row.TARIFF_CODE > 0 &&
                !string.IsNullOrEmpty(analysis_costs_row.TARIFF_NAME) &&
                !string.IsNullOrEmpty(analysis_costs_row.RESOURCE_TYPE) &&
                analysis_costs_row.TCR >= 0.0M &&
                !string.IsNullOrEmpty(analysis_costs_row.METER_TYPE) &&
                !string.IsNullOrEmpty(analysis_costs_row.PAYMENT_NAME) &&
                !string.IsNullOrEmpty(analysis_costs_row.TOTAL_AMOUNT))
            {
                return true;
            }
            return false;
        }

        internal static bool Utility_Do_We_Do_Something(SmartUtility.Resources resources_row,
                                            char displayed_resource_code,
                                            string displayed_udprn,
                                            string account_udprn)
        {
            //  Rules:
            //  If it doesn't match and X turn it to blank
            //  If it doesn't match and blank leave it alone
            //  If it matches and X leave it alone
            //  If it matches and blank turn it to X

            if (account_udprn != displayed_udprn)
            {
                if (displayed_resource_code != resources_row.RESOURCE_CODE)
                {
                    if (resources_row.CHECKED == SmartParametersV2016.lastChecked)
                    {
                        resources_row.CHECKED = "";
                        return true;
                    }
                }
                else
                {
                    if (resources_row.CHECKED != SmartParametersV2016.lastChecked)
                    {
                        resources_row.CHECKED = "";
                        return true;
                    }
                }
            }
            return false;
        }

#if WPF  || WINUI
        internal static DataGrid Breakdown_Data_Grid(MainViewModel ourviewmodel,
                                                        UtilityViewModel utilityviewmodel,
                                                        double maxHeight,
                                                        double maxWidth)
        {
            // Make a new DataGrid allowing a double-click (DataGrid)
            DataGrid child_datagrid = new DataGrid
            {
                // Don't make any columns ourselves
                AutoGenerateColumns = true,
                IsReadOnly = true,
                // Turn on scrolling
                VerticalScrollBarVisibility = ScrollBarVisibility.Visible,
                // Fix the height
                MaxHeight = maxHeight,
                MaxWidth = maxWidth
            };

            // What an absolute fucking bitch all this DataGridView stuff is ..
            // This next line is an OUT-OF-BROWSER function which DOESN'T WORK In-Browser (its left in so I can rememeber how to do it ...)
            //DataGrid fuckers = (DataGrid)Application.Current.Content.FindName("dataGridBreakdown");

            Fill_Breakdown(ourviewmodel,
                            utilityviewmodel,
                            //SmartParametersV2016.ddmmmyyyyFormat,
                            //SmartParametersV2016.format2Places,
                            SmartParametersV2016.format3Places);

            // This used to be BEFORE the previous line, but now it makes sense to put it HERE, because we should
            // (should - that's the fucking theory with asll this shit) copy everything into the child_Datagrid
            // What an absolute fucking NIGHTMARE all this shit is ... Microshit fucking chimpanzees wrote this bollocks ..
            //CopyColumns_New<BreakdownView>(BreakdownGrid, child_datagrid, ""); // Seems to make DESCRIPTION visible all by itself 

            child_datagrid.ItemsSource = utilityviewmodel.UtilityBreakdown;
            return child_datagrid;
        }



        internal static void CopyColumns_New<T>(DataGrid from_grid, DataGrid to_grid)//, string header)
        {
            //foreach (T item in from_grid)
            //{
            //    ICloneable cloneable = item as ICloneable;
            //    if (cloneable != null)
            //    {
            //        to_grid.Columns.Add((DataGridColumn)cloneable.Clone());
            //    }
            //    else
            //    {
            //        to_grid.Items.Add(item);
            //    }
            //}

            FieldInfo[] myFields = typeof(T).GetFields(SmartParametersV2016.bindingFlags);
            //T target_row = Activator.CreateInstance<T>();
            //FieldInfo[] myFields = typeof(T).GetFields(SmartParametersV2016.bindingFlags);

            //FieldInfo[] fields = t.GetFields(SmartParametersV2016.bindingFlags);
            foreach (FieldInfo field in myFields)
            {
                object fieldV = field.GetValue(from_grid);
                if (fieldV != null)
                {
                    field.SetValue(to_grid, fieldV);
                }
            }


            //foreach (var col in from_grid)
            //{
            //    DataGridTextColumn newcol = new DataGridTextColumn
            //    {
            //        Header = col.Header
            //    };

            //    string fuckers = col.Header.ToString();

            //    newcol.Binding = col.Binding;
            //    if (!string.IsNullOrEmpty(col.Binding.StringFormat) &&
            //        string.IsNullOrEmpty(newcol.Binding.StringFormat))
            //    {
            //        newcol.Binding.StringFormat = col.Binding.StringFormat;
            //    }
            //    if (col.CellStyle != null)
            //    {
            //        newcol.CellStyle = col.CellStyle;
            //    }
            //    if (!string.IsNullOrEmpty(header))
            //    {
            //        if (header == fuckers)
            //        {
            //            to_grid.Columns.Add(newcol);
            //            break;
            //        }
            //    }
            //    else
            //    {
            //        to_grid.Columns.Add(newcol);
            //    }
            //}
            return;
        }

        internal static List<DataGridColumn> CopyColumns(DataGrid from_grid)
        {
            List<DataGridColumn> to_grid = new List<DataGridColumn>();
            foreach (DataGridColumn col in from_grid.Columns)
            {
                DataGridTextColumn textcol = col as DataGridTextColumn;
                DataGridTextColumn newcol = new DataGridTextColumn
                {
                    Header = textcol.Header,
                    Binding = textcol.Binding
                };
#if !WINUI
                if (!string.IsNullOrEmpty(textcol.Binding.StringFormat) &&
                    string.IsNullOrEmpty(newcol.Binding.StringFormat))
                {
                    newcol.Binding.StringFormat = textcol.Binding.StringFormat;
                }
#endif
                

                if (col.CellStyle != null)
                {
                    newcol.CellStyle = textcol.CellStyle;
                }
                to_grid.Add(newcol);
                //if (!string.IsNullOrEmpty(header))
                //{
                //    if (header == fuckers)
                //    {
                //        to_grid.Columns.Add(newcol);
                //        break;
                //    }
                //}
                //else
                //{
                //    to_grid.Columns.Add(newcol);
                //}
            }
            return to_grid;
        }
#endif
#if SMARTMAUI
        internal static CollectionView Breakdown_Data_Grid(MainViewModel ourviewmodel,
                                                        UtilityViewModel utilityviewmodel,
                                                        double maxHeight,
                                                        double maxWidth)
        {
            // Make a new DataGrid allowing a double-click (DataGrid)
            CollectionView child_datagrid = new CollectionView
            {
                // Don't make any columns ourselves
                //AutoGenerateColumns = true,
                //IsReadOnly = true,
                // Turn on scrolling
                //VerticalScrollBarVisibility = ScrollBarVisibility.Visible,
                // Fix the height
                //MaxHeight = maxHeight,
                //MaxWidth = maxWidth
            };

            // What an absolute fucking bitch all this DataGridView stuff is ..
            // This next line is an OUT-OF-BROWSER function which DOESN'T WORK In-Browser (its left in so I can rememeber how to do it ...)
            //DataGrid fuckers = (DataGrid)Application.Current.Content.FindName("dataGridBreakdown");

            Fill_Breakdown(ourviewmodel,
                            utilityviewmodel,
                            //SmartParametersV2016.ddmmmyyyyFormat,
                            //SmartParametersV2016.format2Places,
                            SmartParametersV2016.format3Places);

            // This used to be BEFORE the previous line, but now it makes sense to put it HERE, because we should
            // (should - that's the fucking theory with asll this shit) copy everything into the child_Datagrid
            // What an absolute fucking NIGHTMARE all this shit is ... Microshit fucking chimpanzees wrote this bollocks ..
            //CopyColumns_New<BreakdownView>(BreakdownGrid, child_datagrid, ""); // Seems to make DESCRIPTION visible all by itself 

            child_datagrid.ItemsSource = utilityviewmodel.UtilityBreakdown;
            return child_datagrid;
        }

#endif

#if ANDROIDX
        //internal static XamarShit.Forms.DataGrid.DataGrid Breakdown_Data_Grid(SignInViewModel signinviewmodel,
        //                                            MainViewModel ourviewmodel,
        //                                            UtilityViewModel utilityviewmodel,
        //                                            DateTime defaultDate)
        //{
        //    // Make a new DataGrid allowing a double-click (DataGrid)
        //    XamarShit.Forms.DataGrid.PaletteCollection textpc = new XamarShit.Forms.DataGrid.PaletteCollection()
        //    {
        //        Color.Black
        //    };
        //    XamarShit.Forms.DataGrid.PaletteCollection backpc = new XamarShit.Forms.DataGrid.PaletteCollection()
        //    {
        //        Color.White
        //    };

        //    XamarSHit.Forms.DataGrid.DataGrid child_datagrid = new XamarShit.Forms.DataGrid.DataGrid()
        //    {
        //        HorizontalOptions = LayoutOptions.FillAndExpand,
        //        VerticalOptions = LayoutOptions.FillAndExpand,
        //        RowHeight = 20,
        //        RowSpacing = 0,
        //        BackgroundColor = Color.White,
        //        RowsTextColorPalette = textpc,
        //        RowsBackgroundColorPalette = backpc,
        //        IsVisible = false
        //    };
        //    child_datagrid.ItemsSource = new List<SmartUtility.AnalysisBreakdownView>();

        //    // What an absolute fucking bitch all this DataGridView stuff is ..
        //    // This next line is an OUT-OF-BROWSER function which DOESN'T WORK In-Browser (its left in so I can rememeber how to do it ...)
        //    //DataGrid fuckers = (DataGrid)Application.Current.Content.FindName("dataGridBreakdown");
        //    Fill_Breakdown(signinviewmodel,
        //                    ourviewmodel,
        //                    utilityviewmodel,
        //                    defaultDate,
        //                    SmartParametersV2016.ddmmmyyyyFormat,
        //                    SmartParametersV2016.format2Places,
        //                    SmartParametersV2016.format3Places);
        //    child_datagrid.ItemsSource = utilityviewmodel.UtilityBreakdown;
        //    return child_datagrid;
        //}

        //internal static void CopyColumns(XamarShit.Forms.DataGrid.DataGrid from_grid, XamarShit.Forms.DataGrid.DataGrid to_grid, string header)
        //{
        //    foreach (XamarShit.Forms.DataGrid.DataGridColumn col in from_grid.Columns)
        //    {
        //        XamarShit.Forms.DataGrid.DataGridColumn newcol = new XamarShit.Forms.DataGrid.DataGridColumn
        //        {
        //            Title = col.Title
        //        };

        //        string fuckers = col.Title.ToString();

        //        newcol.PropertyName = col.PropertyName;
        //        if (!string.IsNullOrEmpty(col.StringFormat) &&
        //            string.IsNullOrEmpty(newcol.StringFormat))
        //        {
        //            newcol.StringFormat = col.StringFormat;
        //        }
        //        //if (col.CellStyle != null)
        //        //{
        //        //   newcol.CellStyle = col.CellStyle;
        //        // }
        //        if (!string.IsNullOrEmpty(header))
        //        {
        //            if (header == fuckers)
        //            {
        //                to_grid.Columns.Add(newcol);
        //                break;
        //            }
        //        }
        //        else
        //        {
        //            to_grid.Columns.Add(newcol);
        //        }
        //    }
        //    return;
        //}
#endif

#if WINFORMS
        internal static void Utility_PopupMouseLeave(object sender, System.Windows.Forms.MouseEventArgs e, UtilityViewModel utilityviewmodel)
        {
#endif
#if WPF  || WINUI
        internal static void Utility_PopupMouseLeave(object sender, MouseEventArgs e, UtilityViewModel utilityviewmodel)
        {
#endif
#if SMARTMAUI
        internal static void Utility_PopupMouseLeave(object sender, PopupClosedEventArgs e, UtilityViewModel utilityviewmodel)
        {
#endif
#if WPF  || WINUI || SMARTMAUI
            if (sender != null && e != null)
            {
                if (utilityviewmodel.childWindow != null)
                {
#if WINUI
                    if (utilityviewmodel.childWindow.IsOpen)
#endif
#if WPF
                    if (utilityviewmodel.childWindow.IsVisible)
#endif
#if SMARTMAUI
                    if (utilityviewmodel.childWindow != null) // Only checks it exists not if its open
#endif
                    {
#if WINUI
                        utilityviewmodel.childWindow.IsOpen = false;
#endif
#if WPF || SMARTMAUI
                        utilityviewmodel.childWindow.Close();
#endif
                    }
                }
            }
#endif
#if WINFORMS || WPF  || WINUI || SMARTMAUI
            return;
        }
#endif

#if WINFORMS
        internal static void Utility_PopupMouseLeftButtonDown(object sender, System.Windows.Forms.MouseEventArgs e, UtilityViewModel utilityviewmodel)
        {
#endif
#if WPF
        internal static void Utility_PopupMouseLeftButtonDown(object sender, MouseButtonEventArgs e, UtilityViewModel utilityviewmodel)
        {
#endif
#if WINUI
        internal static void Utility_PopupMouseLeftButtonDown(object sender, EventArgs e, UtilityViewModel utilityviewmodel)
        {
#endif
#if ANDROIDX
        internal static void Utility_PopupMouseLeftButtonDown(object sender, EventArgs e, UtilityViewModel utilityviewmodel)
        {
#endif
#if SMARTMAUI
        internal static void Utility_PopupMouseLeftButtonDown(object sender, PointerEventArgs e, UtilityViewModel utilityviewmodel)
        {
#endif
            if (sender != null && e != null)
            {
                // This absolute fucking useless bollocks #1
                // THANK YOU Doguhan Uluca - you are a star ...
#if WINFORMS
                //int ClickCount = 1;
                //switch (ClickCount)
#endif
#if WPF
                switch (e.ClickCount)
                {
                    case 0:
                    case 1:
                        // Cancel this handler because this appears to get called as well
                        // when the window is removed ... and we only want to close it ONCE, DON'T WE???
                        utilityviewmodel.childWindow.MouseLeave -= new MouseEventHandler((s, e1) => Utility_PopupMouseLeave(s, e1, utilityviewmodel));
                        //utilityviewmodel.childWindow.MouseLeave -= new MouseEventHandler(Utility_PopupMouseLeave(sender, e, utilityviewmodel));
                        // Remove the popup grid from the screen
                        utilityviewmodel.childWindow.Close();
                        break;
                    default:
                        break;
                }
#endif
#if SMARTMAUI
                // Not needed as the Popup can be close by tapping outside it
#endif
            }
            return;
        }

        internal static void ClearDownEverythingUtility(MainViewModel ourviewmodel,
                                                    UtilityViewModel utilityviewmodel,
                                                    string none)
        {
            if (ourviewmodel != null &&
                utilityviewmodel != null)
            {

#if WINFORMS || WPF  || WINUI || SMARTMAUI
                utilityviewmodel.NextConnectionMessage = none;
                utilityviewmodel.AreaId = "";
                utilityviewmodel.AccountNo = "";
                utilityviewmodel.PostCode = "";
                utilityviewmodel.UserId = "";
#endif
#if ANDROIDX
                utilityviewmodel.NextConnectionMessage.Text = none;
                utilityviewmodel.AreaId.Text = "";
                utilityviewmodel.AccountNo.Text = "";
                utilityviewmodel.PostCode.Text = "";
                utilityviewmodel.UserId = "";
#endif
#if WINFORMS
                utilityviewmodel.Password = "";
#endif
#if WPF  || WINUI || SMARTMAUI
                //utilityviewmodel.PasswordHandler = () => "";
                utilityviewmodel.Password = "";
#endif
#if ANDROIDX
                utilityviewmodel.Password = "";
#endif
#if WINFORMS || WPF  || WINUI || SMARTMAUI
                utilityviewmodel.DayRate1 = "";
                utilityviewmodel.NightRate1 = "";
                utilityviewmodel.StandingCharge1 = "";
                utilityviewmodel.DayRate2 = "";
                utilityviewmodel.NightRate2 = "";
                utilityviewmodel.StandingCharge2 = "";
#endif
#if ANDROIDX
                utilityviewmodel.DayRate1.Text = "";
                utilityviewmodel.NightRate1.Text = "";
                utilityviewmodel.StandingCharge1.Text = "";
                utilityviewmodel.DayRate2.Text = "";
                utilityviewmodel.NightRate2.Text = "";
                utilityviewmodel.StandingCharge2.Text = "";
#endif
#if WINFORMS
                utilityviewmodel.LedSwitched = false;
                utilityviewmodel.Economy7State = false;
#endif
#if WPF || SMARTMAUI
                utilityviewmodel.LedSwitched = Visibility.Hidden;
                utilityviewmodel.Economy7State = Visibility.Hidden;
#endif
#if WINUI
                utilityviewmodel.LedSwitched = Visibility.Collapsed;
                utilityviewmodel.Economy7State = Visibility.Collapsed;
#endif
#if ANDROIDX
                utilityviewmodel.LedSwitched.Text = "";
                utilityviewmodel.Economy7State = false;
#endif
#if WINFORMS || WPF  || WINUI || SMARTMAUI
                utilityviewmodel.CheckBox60 = false;
#endif
#if ANDROIDX
                utilityviewmodel.CheckBox60.Checked = false;
#endif
#if WPF  || WINUI || SMARTMAUI
                utilityviewmodel.PictureBoxDNO = null;
#endif
#if ANDROIDX
                Bitmap bitMapImageKESmall = BitmapFactory.DecodeFile(@"Images/keasdon_energy_small.jpg");
                utilityviewmodel.PictureBoxDNO.SetImageBitmap(bitMapImageKESmall);
#endif
                utilityviewmodel.UtilityCosts = new List<SmartUtility.AnalysisCostsView>();
                utilityviewmodel.UtilityBills = new List<SmartUtility.AnalysisBillsView>();
                utilityviewmodel.UtilityReadings = new List<SmartUtility.AnalysisReadingsView>();
                utilityviewmodel.UtilityBreakdown = new List<SmartUtility.AnalysisBreakdownView>();
            }
            return;
        }

        //     Its ok to use async void on Event Handlers
#if WINFORMS
        internal static void Utility_AddressesChanged_Actual(MainProcess components,
                                                                    MainViewModel ourviewmodel,
                                                                    UtilityViewModel utilityviewmodel)
#endif
#if WPF  || WINUI
        internal static void Utility_AddressesChanged_Actual(MainViewModel ourviewmodel,
                                                                    UtilityViewModel utilityviewmodel)
#endif
#if ANDROIDX
        internal static void Utility_AddressesChanged_Actual(MainViewModel ourviewmodel,
                                                                    UtilityViewModel utilityviewmodel)
#endif
#if SMARTMAUI
        internal static void Utility_AddressesChanged_Actual(MainViewModel ourviewmodel,
                                                                    UtilityViewModel utilityviewmodel)
#endif
        {
            // Is happens on initial setup AND if a Consumer has multiple addresses so they can look at past usage
            // What are we doing here?  We have changed Address so we want to use the CURRENT resource code (either 'E'
            // or 'G') as a 'base' and see if we have a resource which matches it.
            //  Address    Displayed Resource  Before  After   Action 
            //  123456        E         E        X       X     Do nothing (E matches E and X is on)
            //  123456        E         G      blank   blank   Do nothing (G doesn't match E and blank is off)
            //  789012        E         G        X     blank   Switch X to blank (G doesn't match E)
            //
            //  789012        G         G        X       X     Do nothing (G matches G and X is on)
            //  123456        G         E        X     blank   Switch X to blank (E doesn't match G)
            //  123456        G         G      blank     X     Switch blank to X (G matches G and switch is blank)
            //
            //  Rules:
            //  If it matches and X leave it alone
            //  If it matches and blank turn it to X
            //  If it doesn't match and X turn it to blank
            //  If it doesn't match and blank leave it alone

            // Sanity Check
            if (utilityviewmodel.AddressSelectedIndex != -1)
            {

                MainViewModel.AddressItem addresses_item = (MainViewModel.AddressItem)utilityviewmodel.UtilityAddressesList[utilityviewmodel.AddressSelectedIndex];
                string displayed_udprn = addresses_item.Value.ToString();

                // Will return resources_found.Count = 0 for DualFuel 'D'
                List<SmartUtility.Resources> resources_found = SmartSpikeUtilityV2017.UtilityFindResourcesList(ourviewmodel,
                                                                                        utilityviewmodel,
                                                                                        utilityviewmodel.resource_code);

                //// If the current one shown now IS NOT the 'local energy code' then
                //// convert any
                //// 'X' in the LAST_DISPLAY column 
                //if (resources_found.Count > 0)
                //{
                //    foreach (SmartUtility.Resources resources_row in utilityviewmodel.Hezbollah.utility_resourcesList)
                //    {
                //        // Find the account row for this resource ... which should have the
                //        // UDPRN inside it
                //        List<SmartUtility.Accounts> accounts_found = SmartSpikeUtilityV2017.Utility_Lookup_Resources(ourviewmodel,
                //                                      utilityviewmodel,
                //                                      resources_row.RESOURCE_CODE);
                //        if (accounts_found.Count > 0)
                //        {
                //            if (Utility_Do_We_Do_Something(resources_row,
                //                            utilityviewmodel.resourceCodes,
                //                            displayed_udprn,
                //                            accounts_found[0].UDPRN))
                //            {
                //                resources_row.Updated = true;
                //                utilityviewmodel.Hezbollah.utility_resources_changesList.Add(resources_row);
                //                if (!await SmartUtilityScrapeV2022.DoAll_SmartUtility(ourviewmodel,
                //                                                utilityviewmodel,
                //                                                false,
                //                                                SmartParametersV2016.sqliteformat,
                //                                                em => utilityviewmodel.errorMessage = em,
                //                                                0,
                //                                                0))
                //                {
                //                    if (!(ourviewmodel.quitCts.IsCancellationRequested ||
                //                            utilityviewmodel.utilityToken.IsCancellationRequested))
                //                    {
                //                        // Nothing ever turns this back from Red
                //                        FrontEndGUI.SetLedColour(ourviewmodel, 8, ourviewmodel.redColour);
                //                        if (!await SmartRoutinesV2018.CheckTrace(ourviewmodel, utilityviewmodel.utilityToken, 0, 0, "Problem 64: DoAll_SmartUtility"))
                //                        {
                //                           return false;
                //                        }
                //                    }
                //                    return;
                //                }
                //            }
                //        }
                //    }
                //}

            }
            return;
        }

        // Its ok to use async void on Event Handlers
#if WINFORMS
        internal static async Task<int> UtilitySuppliersChanged_Actual(MainViewModel ourviewmodel,
                                                                    UtilityViewModel utilityviewmodel)
#endif
#if WPF  || WINUI
        internal static async Task<int> UtilitySuppliersChanged_Actual(MainViewModel ourviewmodel,
                                                                    UtilityViewModel utilityviewmodel)
#endif
#if ANDROIDX
        internal static async Task<int> UtilitySuppliersChanged_Actual(MainViewModel ourviewmodel,
                                                                        UtilityViewModel utilityviewmodel)
#endif
#if SMARTMAUI
        internal static async Task<int> UtilitySuppliersChanged_Actual(MainViewModel ourviewmodel,
                                                                    UtilityViewModel utilityviewmodel)
#endif
        {
            int tariffs_index = -1;

            utilityviewmodel.UtilityTariffsList.Clear();
            utilityviewmodel.TariffSelectedIndex = -1;
            utilityviewmodel.UtilityPaymentPlansList.Clear();
            utilityviewmodel.PaymentPlanSelectedIndex = -1;

            // Continual running commentary ... continual churning, churning, churning ...
            UtilityViewModel.SupplierItem supplier_item = (UtilityViewModel.SupplierItem)utilityviewmodel.UtilitySuppliersList[utilityviewmodel.SupplierSelectedIndex];

            string brand_name = supplier_item.Content;
#if WINFORMS
            brand_name = brand_name.TrimStart(SmartParametersV2016.defaultgreen).TrimEnd(SmartParametersV2016.defaultred);
#endif
#if WINFORMS || WPF  || WINUI || SMARTMAUI
            if (!string.IsNullOrEmpty(utilityviewmodel.AreaId) &&
                !string.IsNullOrEmpty(brand_name))
#endif
#if ANDROIDX
            if (!string.IsNullOrEmpty(utilityviewmodel.AreaId.Text) &&
                !string.IsNullOrEmpty(brand_name))
#endif
            {
                // Set the ComboBox itself to match the Item's colour (whatever that happens to be)

                //utilityviewmodel.UtilitySuppliersColor = supplier_item.Colour;

                utilityviewmodel.area_code = Convert.ToInt16(utilityviewmodel.AreaId);

                utilityviewmodel.supplier_code = 0;
                utilityviewmodel.brand_code = 0;

                if (!Lookup_Brand_Item(ourviewmodel,
                                        utilityviewmodel,
                                        utilityviewmodel.resource_code,
                                        brand_name))
                {
                    if (!(ourviewmodel.quitCts.IsCancellationRequested ||
                            utilityviewmodel.utilityToken.IsCancellationRequested))
                    {
                        FrontEndGUI.SetLedColour(ourviewmodel, 8, ourviewmodel.redColour);
                        if (!await SmartRoutinesV2018.CheckTrace(ourviewmodel, utilityviewmodel.utilityToken, utilityviewmodel.supplier_code, utilityviewmodel.brand_code, "Problem 65: Lookup BrandItem"))
                        {
                            return -1;
                        }
                    }
                    //return tariffs_index;
                }
                else
                {
                    string resource_type = Check_Economy7(utilityviewmodel.Economy7State);

                    // Should display nothing id Tariff code = -1
                    tariffs_index = await Build_Tariff_Dropdown(ourviewmodel,
                                                                utilityviewmodel,
                                                                SmartParametersV2016.defaultDate,
                                                                utilityviewmodel.Hezbollah.e_analysis_costsList,
                                                                utilityviewmodel.Hezbollah.g_analysis_costsList,
                                                                utilityviewmodel.Hezbollah.d_analysis_costsList,
                                                                utilityviewmodel.resource_code,
                                                                resource_type,
                                                                utilityviewmodel.supplier_code,
                                                                utilityviewmodel.brand_code,
                                                                0);
                    //if (!string.IsNullOrEmpty(ourviewmodel.errorMessage))
                    //{
                    //    return -1;
                    //}                    
                }
            }
            return tariffs_index;
        }

        // Its ok to use async void on Event Handlers
#if WINFORMS
        internal static async Task<int> UtilityTariffsChanged_Actual(MainViewModel ourviewmodel,
                                                                    UtilityViewModel utilityviewmodel)
#endif
#if WPF  || WINUI
        internal static async Task<int> UtilityTariffsChanged_Actual(MainViewModel ourviewmodel,
                                                                    UtilityViewModel utilityviewmodel)
#endif
#if ANDROIDX
        internal static async Task<int> UtilityTariffsChanged_Actual(MainViewModel ourviewmodel,
                                                                    UtilityViewModel utilityviewmodel)
#endif
#if SMARTMAUI
        internal static async Task<int> UtilityTariffsChanged_Actual(MainViewModel ourviewmodel,
                                                                    UtilityViewModel utilityviewmodel)
#endif
        {
            // Sanity Check
            int tariffs_index = -1;

            utilityviewmodel.UtilityPaymentPlansList.Clear();
            utilityviewmodel.PaymentPlanSelectedIndex = -1;

            UtilityViewModel.TariffItem tariff_item = (UtilityViewModel.TariffItem)utilityviewmodel.UtilityTariffsList[utilityviewmodel.TariffSelectedIndex];

            string tariff_name = tariff_item.Content;
#if WINFORMS
            tariff_name = tariff_name.TrimStart(SmartParametersV2016.defaultgreen).TrimEnd(SmartParametersV2016.defaultred);
#endif
#if WINFORMS || WPF  || WINUI || SMARTMAUI
            if (!string.IsNullOrEmpty(utilityviewmodel.AreaId) &&
            !string.IsNullOrEmpty(tariff_name))
#endif
#if ANDROIDX
            if (!string.IsNullOrEmpty(utilityviewmodel.AreaId.Text) &&
            !string.IsNullOrEmpty(tariff_name))
#endif
            {
                // Set the ComboBox itself to match the Item's colour (whatever that happens to be)
                utilityviewmodel.UtilityTariffsColor = tariff_item.Colour;

                utilityviewmodel.area_code = Convert.ToInt16(utilityviewmodel.AreaId);

                utilityviewmodel.supplier_code = 0;
                utilityviewmodel.brand_code = 0;

                UtilityViewModel.SupplierItem supplier_item = (UtilityViewModel.SupplierItem)utilityviewmodel.UtilitySuppliersList[utilityviewmodel.SupplierSelectedIndex];

                string brand_name = supplier_item.Content;
#if WINFORMS
                brand_name = brand_name.TrimStart(SmartParametersV2016.defaultgreen).TrimEnd(SmartParametersV2016.defaultred);
#endif
                if (!Lookup_Brand_Item(ourviewmodel,
                                        utilityviewmodel,
                                        utilityviewmodel.resource_code,
                                        brand_name))
                {
                    if (!(ourviewmodel.quitCts.IsCancellationRequested ||
                            utilityviewmodel.utilityToken.IsCancellationRequested))
                    {
                        FrontEndGUI.SetLedColour(ourviewmodel, 8, ourviewmodel.redColour);
                        if (!await SmartRoutinesV2018.CheckTrace(ourviewmodel, utilityviewmodel.utilityToken, utilityviewmodel.supplier_code, utilityviewmodel.brand_code, "Problem 66: Lookup BrandItem"))
                        {
                            return -1;
                        }
                    }
                    //return tariffs_index;
                }
                else
                {
                    string resource_type = Check_Economy7(utilityviewmodel.Economy7State);
                    utilityviewmodel.TARIFF_CODE = 0;
                    if (Lookup_Tariff_Item(ourviewmodel,
                                            utilityviewmodel,
                                            tariff_name,
                                            utilityviewmodel.brand_code,
                                            utilityviewmodel.resource_code,
                                            resource_type))
                    {
                        // Should display nothing if Payment Plan is empty
                        tariffs_index = await Build_Payment_Plan_Dropdown(ourviewmodel,
                                                                utilityviewmodel,
                                                                utilityviewmodel.brand_code,
                                                                utilityviewmodel.supplier_code,
                                                                utilityviewmodel.resource_code,
                                                                resource_type,
                                                                utilityviewmodel.TARIFF_CODE,
                                                                SmartParametersV2016.defaultChar);
                        //if (!string.IsNullOrEmpty(ourviewmodel.errorMessage))
                        //{
                        //    return -1;
                        //}
                        //return tariffs_index;
                    }
                }
            }
            return tariffs_index;
        }

        // Its ok to use async void on Event Handlers
#if WINFORMS
        internal static async Task<int> UtilityPaymentPlansChanged_Actual(MainViewModel ourviewmodel,
                                                                        UtilityViewModel utilityviewmodel)
#endif
#if WPF  || WINUI
        internal static async Task<int> UtilityPaymentPlansChanged_Actual(MainViewModel ourviewmodel,
                                                                        UtilityViewModel utilityviewmodel)
#endif
#if ANDROIDX
        internal static async Task<int> UtilityPaymentPlansChanged_Actual(MainViewModel ourviewmodel,
                                                                        UtilityViewModel utilityviewmodel)
#endif
#if SMARTMAUI
        internal static async Task<int> UtilityPaymentPlansChanged_Actual(MainViewModel ourviewmodel,
                                                                        UtilityViewModel utilityviewmodel)
#endif
        {
            int status = -1;
            UtilityViewModel.PaymentPlanItem payment_plan_item = (UtilityViewModel.PaymentPlanItem)utilityviewmodel.UtilityPaymentPlansList[utilityviewmodel.PaymentPlanSelectedIndex];

            string payment_plan_name = payment_plan_item.Content;
#if WINFORMS
            payment_plan_name = payment_plan_name.TrimStart(SmartParametersV2016.defaultgreen).TrimEnd(SmartParametersV2016.defaultred);
#endif
#if WINFORMS || WPF  || WINUI
            if (!string.IsNullOrEmpty(utilityviewmodel.AreaId) &&
                !string.IsNullOrEmpty(payment_plan_name))
#endif
#if ANDROIDX
            if (!string.IsNullOrEmpty(utilityviewmodel.AreaId.Text) &&
                !string.IsNullOrEmpty(payment_plan_name))
#endif
            {
                // Set the ComboBox itself to match the Item's colour (whatever that happens to be)
                utilityviewmodel.UtilityPaymentPlansColor = payment_plan_item.Colour;

                //char payment_plan = SmartParametersV2016.defaultChar;
                string PAYMENT_TYPE = payment_plan_name;
                if (!SmartSpikeUtilityV2017.Utility_Lookup_PaymentPlan(ourviewmodel,
                                                        utilityviewmodel,
                                                        PAYMENT_TYPE))
                {
                    if (!(ourviewmodel.quitCts.IsCancellationRequested ||
                            utilityviewmodel.utilityToken.IsCancellationRequested))
                    {
                        FrontEndGUI.SetLedColour(ourviewmodel, 8, ourviewmodel.redColour);
                        if (!await SmartRoutinesV2018.CheckTrace(ourviewmodel, utilityviewmodel.utilityToken,
                                0, 0, "PaymentPlan"
                                + SmartParametersV2016.space + ourviewmodel.warningMessage))
                        {
                            return -1;
                        }
                    }
                    //return -1;
                }
                else
                {
                    utilityviewmodel.area_code = Convert.ToInt16(utilityviewmodel.AreaId);
                    utilityviewmodel.supplier_code = 0;
                    utilityviewmodel.brand_code = 0;

                    UtilityViewModel.SupplierItem supplier_item = (UtilityViewModel.SupplierItem)utilityviewmodel.UtilitySuppliersList[utilityviewmodel.SupplierSelectedIndex];

                    string brand_name = supplier_item.Content;
#if WINFORMS
                    brand_name = brand_name.TrimStart(SmartParametersV2016.defaultgreen).TrimEnd(SmartParametersV2016.defaultred);
#endif
                    if (!Lookup_Brand_Item(ourviewmodel,
                                            utilityviewmodel,
                                            utilityviewmodel.resource_code,
                                            brand_name))
                    {
                        if (!(ourviewmodel.quitCts.IsCancellationRequested ||
                                utilityviewmodel.utilityToken.IsCancellationRequested))
                        {
                            FrontEndGUI.SetLedColour(ourviewmodel, 8, ourviewmodel.redColour);
                            if (!await SmartRoutinesV2018.CheckTrace(ourviewmodel, utilityviewmodel.utilityToken,
                                    0, 0, "BrandItem"))
                            {
                                return -1;
                            }
                        }
                        //return -1;
                    }
                    else
                    {

                        string resource_type = Check_Economy7(utilityviewmodel.Economy7State);
                        utilityviewmodel.TARIFF_CODE = 0;

                        UtilityViewModel.TariffItem tariff_item = (UtilityViewModel.TariffItem)utilityviewmodel.UtilityTariffsList[utilityviewmodel.TariffSelectedIndex];

                        string tariff_name = tariff_item.Content;
#if WINFORMS
                        tariff_name = tariff_name.TrimStart(SmartParametersV2016.defaultgreen).TrimEnd(SmartParametersV2016.defaultred);
#endif
                        if (!Lookup_Tariff_Item(ourviewmodel,
                                                utilityviewmodel,
                                                tariff_name,
                                                utilityviewmodel.brand_code,
                                                utilityviewmodel.resource_code,
                                                resource_type))
                        {
                            if (!(ourviewmodel.quitCts.IsCancellationRequested ||
                                    utilityviewmodel.utilityToken.IsCancellationRequested))
                            {
                                FrontEndGUI.SetLedColour(ourviewmodel, 8, ourviewmodel.redColour);
                                if (!await SmartRoutinesV2018.CheckTrace(ourviewmodel, utilityviewmodel.utilityToken,
                                                0, 0, "Problem 67: TariffItem"))
                                {
                                    return -1;
                                }
                            }
                            //return -1;
                        }
                        else
                        {
#if WINFORMS
                            utilityviewmodel.DayRate1Brush = ourviewmodel.transparentColour;
                            utilityviewmodel.NightRate1Brush = ourviewmodel.transparentColour;
                            utilityviewmodel.StandingCharge1Brush = ourviewmodel.transparentColour;
#endif
#if WPF  || WINUI || SMARTMAUI
                            utilityviewmodel.DayRate1Brush = ourviewmodel.transparentColour;
                            utilityviewmodel.NightRate1Brush = ourviewmodel.transparentColour;
                            utilityviewmodel.StandingCharge1Brush = ourviewmodel.transparentColour;
#endif
#if ANDROIDX
                            utilityviewmodel.DayRate1Brush = ourviewmodel.transparentColour;
                            utilityviewmodel.NightRate1Brush = ourviewmodel.transparentColour;
                            utilityviewmodel.StandingCharge1Brush = ourviewmodel.transparentColour;
#endif
#if WINFORMS || WPF  || WINUI || SMARTMAUI
                            utilityviewmodel.DayRate1 = "";
                            utilityviewmodel.NightRate1 = "";
                            utilityviewmodel.StandingCharge1 = "";
#endif
#if ANDROIDX
                            utilityviewmodel.DayRate1.Text = "";
                            utilityviewmodel.NightRate1.Text = "";
                            utilityviewmodel.StandingCharge1.Text = "";
#endif
#if WINFORMS
                            utilityviewmodel.DayRate2Brush = ourviewmodel.transparentColour;
                            utilityviewmodel.NightRate2Brush = ourviewmodel.transparentColour;
                            utilityviewmodel.StandingCharge2Brush = ourviewmodel.transparentColour;
#endif
#if WPF  || WINUI || SMARTMAUI
                            utilityviewmodel.DayRate2Brush = ourviewmodel.transparentColour;
                            utilityviewmodel.NightRate2Brush = ourviewmodel.transparentColour;
                            utilityviewmodel.StandingCharge2Brush = ourviewmodel.transparentColour;
#endif
#if ANDROIDX
                            utilityviewmodel.DayRate2Brush = ourviewmodel.transparentColour;
                            utilityviewmodel.NightRate2Brush = ourviewmodel.transparentColour;
                            utilityviewmodel.StandingCharge2Brush = ourviewmodel.transparentColour;
#endif
#if WINFORMS || WPF  || WINUI || SMARTMAUI
                            utilityviewmodel.DayRate2 = "";
                            utilityviewmodel.NightRate2 = "";
                            utilityviewmodel.StandingCharge2 = "";
#endif
#if ANDROIDX
                            utilityviewmodel.DayRate2.Text = "";
                            utilityviewmodel.NightRate2.Text = "";
                            utilityviewmodel.StandingCharge2.Text = "";
#endif

                            switch (utilityviewmodel.resource_code)
                            {
                                case SmartParametersV2016.Electricity:
                                    if (utilityviewmodel.Hezbollah.e_unit_ratesList.Count > 0)
                                    {
                                        Lookup_Prices(ourviewmodel,
                                                        utilityviewmodel,
                                                        utilityviewmodel.resource_code,
                                                        utilityviewmodel.supplier_code,
                                                        utilityviewmodel.TARIFF_CODE,
                                                        utilityviewmodel.PAYMENT_PLAN,
                                                        resource_type);
                                    }
                                    break;
                                case SmartParametersV2016.Gas:
                                    if (utilityviewmodel.Hezbollah.g_unit_ratesList.Count > 0)
                                    {
                                        Lookup_Prices(ourviewmodel,
                                                        utilityviewmodel,
                                                        utilityviewmodel.resource_code,
                                                        utilityviewmodel.supplier_code,
                                                        utilityviewmodel.TARIFF_CODE,
                                                        utilityviewmodel.PAYMENT_PLAN,
                                                        resource_type);
                                    }
                                    break;
                                default:
                                    break;
                            }
                            status = 0;
                        }
                    }
                }
            }
            return status;
        }

#if WINFORMS
        internal async static void UtilityResetDatesActual(MainProcess process_components,
                                                             MainViewModel ourviewmodel,
                                                             UtilityViewModel utilityviewmodel)
#endif
#if WPF  || WINUI || SMARTMAUI
        internal async static void UtilityResetDatesActual(MainViewModel ourviewmodel,
                                                             UtilityViewModel utilityviewmodel)
#endif
#if ANDROIDX
        // I still don't know what to do with this fucker?  What do I do
        // when the address changes???
        internal async static void UtilityResetDatesActual(
                                                            AppCompatActivity meterActivity,
                                                            MainViewModel ourviewmodel,
                                                             UtilityViewModel utilityviewmodel)
#endif
        {
            // Find earliest and latest Dates across all tab(s)
            if (utilityviewmodel.Hezbollah.working_billsList.Count > 0)
            {
                // We are interested in the PERIOD_START and PERIOD_END
                // Dates **NOT** the BILL_DATE!!!!
#if WINFORMS
                utilityviewmodel.StartDate = utilityviewmodel.Hezbollah.working_billsList.First().BILL_PERIOD_START;
                utilityviewmodel.EndDate = utilityviewmodel.Hezbollah.working_billsList.Last().BILL_PERIOD_END;
#endif
#if WPF  || WINUI || SMARTMAUI
                utilityviewmodel.StartDate = utilityviewmodel.Hezbollah.working_billsList.First().BILL_PERIOD_START;
                utilityviewmodel.EndDate = utilityviewmodel.Hezbollah.working_billsList.Last().BILL_PERIOD_END;
#endif
#if ANDROIDX
                utilityviewmodel.StartDate.DateTime = utilityviewmodel.Hezbollah.working_billsList.First().BILL_PERIOD_START;
                utilityviewmodel.EndDate.DateTime = utilityviewmodel.Hezbollah.working_billsList.Last().BILL_PERIOD_END;
#endif
            }
            else
            {
#if WINFORMS
                utilityviewmodel.StartDate = SmartParametersV2016.defaultDate;
                utilityviewmodel.EndDate = SmartParametersV2016.defaultMaxdate;
#endif
#if WPF  || WINUI || SMARTMAUI
                utilityviewmodel.StartDate = SmartParametersV2016.defaultDate;
                utilityviewmodel.EndDate = SmartParametersV2016.defaultMaxdate;
#endif
#if ANDROIDX
                utilityviewmodel.StartDate.DateTime = SmartParametersV2016.defaultDate;
                utilityviewmodel.EndDate.DateTime = SmartParametersV2016.defaultMaxdate;
#endif
            }

            if (!await SmartUtilityV2022.DisplayUtilityMeterAsync(
#if WINFORMS
                                            process_components,
#endif
#if ANDROIDX
                                            meterActivity,
#endif
                                            ourviewmodel,
                                            utilityviewmodel,
                                            SmartParametersV2016.activeFlag,
                                            utilityviewmodel.next_connection))
            {
                // Need a return false here really
                Console.Write("here");
            }

            return;
        }

        internal static void TurnOffUtilityStatus(
#if WINFORMS
                                                    MainProcess process_components,
#endif
                                                    UtilityViewModel utilityviewmodel)
        {
            if (utilityviewmodel != null)
            {
                utilityviewmodel.SuppliersEnabled =
                utilityviewmodel.TariffsEnabled =
                utilityviewmodel.PaymentPlansEnabled =
                utilityviewmodel.AddressesEnabled =
                utilityviewmodel.ResourcesEnabled =
                utilityviewmodel.UtilityCulturesEnabled =
                utilityviewmodel.UtilityConversionsEnabled =
                utilityviewmodel.StartDateEnabled =
                utilityviewmodel.ResetDatesEnabled =
                utilityviewmodel.EndDateEnabled =
                utilityviewmodel.UtilityCultureStatus =
                utilityviewmodel.SubmitStatus = false;
                // Cancel button works the opposite way
                utilityviewmodel.CancelStatus = true;
            }
            return;
        }

        internal static void TurnOnUtilityStatus(
#if WINFORMS
                                                    MainProcess process_components,
#endif
                                                    UtilityViewModel utilityviewmodel)
        {
            if (utilityviewmodel != null)
            {
                utilityviewmodel.SuppliersEnabled =
                utilityviewmodel.TariffsEnabled =
                utilityviewmodel.PaymentPlansEnabled =
                utilityviewmodel.AddressesEnabled =
                utilityviewmodel.ResourcesEnabled =
                utilityviewmodel.UtilityCulturesEnabled = // Main button?
                utilityviewmodel.UtilityConversionsEnabled =
                utilityviewmodel.StartDateEnabled =
                utilityviewmodel.ResetDatesEnabled =
                utilityviewmodel.EndDateEnabled =
                utilityviewmodel.UtilityCultureStatus =
                utilityviewmodel.SubmitStatus = true;
                // Cancel button works the opposite way
                utilityviewmodel.CancelStatus = false;
            }
            return;
        }

        // Its ok to use async void on Event Handlers
        internal static async Task<bool> Utility_RadioButtonChecked_Actual(
#if WINFORMS
                                                                        MainProcess components,
#endif
#if ANDROIDX
                                                                        AppCompatActivity meterActivity,
#endif
                                                                        MainViewModel ourviewmodel,
                                                                        UtilityViewModel utilityviewmodel)
        {
            // Before you wonder why this isn't an 'async void' routine, check out
            // https://msdn.microsoft.com/en-us/magazine/jj991977.aspx


            // We are going to get info? Better turn off some buttons first!
            // These can all be turned off now, except the Cancel button
            TurnOffUtilityStatus(
#if WINFORMS
                                 components,
#endif
                                 utilityviewmodel);
            bool status = false;
            if (!await DisplayUtilityMeterAsync(
#if WINFORMS
                                                    components,
#endif
#if ANDROIDX
                                                    meterActivity,
#endif
                                                    ourviewmodel,
                                                    utilityviewmodel,
                                                    SmartParametersV2016.activeFlag,
                                                    SmartParametersV2016.defaultDate))
            {
                // We failed because of a Cancellation ... but which one? Check
                if (!(ourviewmodel.quitCts.IsCancellationRequested ||
                        utilityviewmodel.utilityToken.IsCancellationRequested))
                {
                    // If we DIDN'T request a cancellation ...
                    // We do nothing if there is a failure .. !  SmartSwitch DOES NOT fail!!
                    FrontEndGUI.SetLedColour(ourviewmodel, 8, ourviewmodel.redColour);
                }
            }
            else
            {
                // Despite everything and all the pain
                // your program is ABSOLUTELY beautiful Ray, and its ALL YOURS!
                if (await Utility_Switchover(
#if ANDROIDX
                                            meterActivity,
#endif
                                            ourviewmodel,
                                            utilityviewmodel,
                                            false))             // Reset all

                {
                    status = true;
                }
                else
                {
                    FrontEndGUI.SetLedColour(ourviewmodel, 6, ourviewmodel.redColour);
                    if (!await SmartRoutinesV2018.CheckTrace(ourviewmodel, utilityviewmodel.utilityToken, 0, 0, "Problem 68: Utility Switchover"))
                    {
                        return false;
                    }
                }
                // She is such a pompous sanctimonious bitch ...
                // Conservatory <= Observatory    You couldn't make it up!!!
                // excrement housing <= excreable housing
            }
            // We are going to get info? Better turn off some buttons first!
            // These can all be turned off now, except the Cancel button
            TurnOnUtilityStatus(
#if WINFORMS
                                 components,
#endif
                                 utilityviewmodel);
            return status;
        }

#if WPF  || WINUI || SMARTMAUI
        internal static void Get_Switch_Information(SmartUtility.AnalysisCostsView analysis_costs_row,
                                                    MainViewModel ourviewmodel,
                                                    UtilityViewModel utilityviewmodel)
        {
#if WPF
            utilityviewmodel.childWindow = new Window
#endif
#if WINUI
            utilityviewmodel.childWindow = new Popup
#endif
#if SMARTMAUI
            utilityviewmodel.childWindow = new Popup
#endif
            {
#if WPF
                SizeToContent = SizeToContent.WidthAndHeight,
                WindowStartupLocation = WindowStartupLocation.CenterScreen,
                Topmost = true,
                Title = "Email - " + analysis_costs_row.SUPPLIER_NAME + " (" + ")"
#endif
#if WINUI
                IsOpen = true
                //Title = "Email - " + analysis_costs_row.SUPPLIER_NAME + " (" + ")"
#endif
#if SMARTMAUI
                // ALL THE STUFF FROM CHARTS!!
                //SizeToContent = SizeToContent.WidthAndHeight,
                //WindowStartupLocation = WindowStartupLocation.CenterScreen,
                //Topmost = true,
                //Title = "Email - " + analysis_costs_row.SUPPLIER_NAME + " (" + ")"
#endif
            };
#if WPF 
            DockPanel target_dockpanel = new DockPanel();
#endif
#if WINUI
            StackPanel target_dockpanel = new StackPanel();
#endif
#if SMARTMAUI
            StackLayout target_dockpanel = new StackLayout();
#endif

            //components.dataGridReadings.TextColumn("Description", "description");

            //DataGridTextColumn textcol = new DataGridTextColumn;
            //textcol.Header = "Description";
            //textcol. = "description";
            //target_dockpanel.Children.Add(components.DataGrid(DataGridTextColumn("Description", "description"), email_report.ToString().Trim()));
            //target_dockpanel.Children.Add(components.DataGrid(textcol, email_report.ToString().Trim()));


            //This needs fixing!!!!
            //ApplyWindowMouseButtons(components.childWindow);
#if WPF
            utilityviewmodel.childWindow.Content = target_dockpanel;
            utilityviewmodel.childWindow.Show();
#endif
#if WINUI
            utilityviewmodel.childWindow.Child = target_dockpanel;
            utilityviewmodel.childWindow.IsOpen = true;
#endif
#if SMARTMAUI
            utilityviewmodel.childWindow.Content = target_dockpanel;
            Shell.Current.ShowPopupAsync(utilityviewmodel.childWindow);
#endif
            return;
        }
#endif

#if ANDROIDX
        internal static void Get_Switch_Information(SmartUtility.AnalysisCostsView analysis_costs_row,
                                                    MainViewModel ourviewmodel,
                                                    UtilityViewModel utilityviewmodel)
        {
            Console.Write(ourviewmodel.accessStatus);    // Just to avoid Analysis error

            Console.Write(utilityviewmodel.AccountName);    // Just to avoid Analysis error
            //var rootFrame = new XamarShit.Forms.Frame();
            //SmartUtility.SwitchInfo XRAY;// = new SmartUtility.SwitchInfo();
            //await GetSwitchDetails_Async(ourviewmodel, "ABC", "DEF", XRAY);

            //rootFrame.Navigation.PushAsync(new SwitchDetails());

            // Place the frame in the current Window and ensure that it is active
            //Window.Current.Content = rootFrame;

            //{
            //    SizeToContent = SizeToContent.WidthAndHeight,
            //    WindowStartupLocation = WindowStartupLocation.CenterScreen,
            //    Topmost = true,
            //    Title = "Email - " + analysis_costs_row.SupplierName + " (" + ")"
            //};

            //DockPanel target_dockpanel = new DockPanel();

            //components.dataGridReadings.TextColumn("Description", "description");

            //DataGridTextColumn textcol = new DataGridTextColumn;
            //textcol.Header = "Description";
            //textcol. = "description";
            //target_dockpanel.Children.Add(components.DataGrid(DataGridTextColumn("Description", "description"), email_report.ToString().Trim()));
            //target_dockpanel.Children.Add(components.DataGrid(textcol, email_report.ToString().Trim()));


            //This needs fixing!!!!
            //ApplyWindowMouseButtons(components.childWindow);

            //Window.Current.Content = target_dockpanel;
            //Window.Current.Activate();

            return;
        }
#endif


        //#if WINFORMS
        //        internal static async Task<bool> SliderValueChangedActual(MainProcess components,
        //                                                                 SignInViewModel signinviewmodel,
        //                                                                 MainViewModel ourviewmodel,
        //                                                                 UtilityViewModel utilityviewmodel,
        //                                                                 List<SmartUsers.VatRates> vatRatesList,
        //                                                                 List<SmartUsers.ExchangeRates> exchangeRatesList)

        //#endif

#if WPF
        internal static async Task<bool> SliderValueChangedActual(Slider slider,
                                                                SignInViewModel signinviewmodel,
                                                                 MainViewModel ourviewmodel,
                                                                UtilityViewModel utilityviewmodel,
                                                                List<SmartUsers.VatRates> vatRatesList,
                                                                List<SmartUsers.ExchangeRates> exchangeRatesList)

        //#endif                                                                    List<SmartUsers.ExchangeRates> exchangeRatesList)

        //#endif
        //#if WINFORMS || WPF  || WINUI
        {
            // We are going to get info? Better turn off some buttons first!
            // These can all be turned off now, except the Cancel button
            TurnOffUtilityStatus(
#if WINFORMS
                                 components,
#endif
                                 utilityviewmodel);
            bool status = false;
            utilityviewmodel.supplier_code = 0;
            utilityviewmodel.brand_code = 0;
            utilityviewmodel.udprn = "";
            bool autoswitch_on = false;
            DateTime first_date = SmartParametersV2016.defaultDate,
                    last_date = SmartParametersV2016.defaultDate;

#if WINFORMS
            if (true) // Is this the case??
#endif
#if WPF
            if (slider.IsFocused == true) // Is this the case??
#endif
            //#if ANDROIDX
            //if (true) // Is this the case??
            //#endif
            {
                // If we are simulating, then stop ...
                UtilityViewModel.SupplierItem supplier_item = (UtilityViewModel.SupplierItem)utilityviewmodel.UtilitySuppliersList[utilityviewmodel.SupplierSelectedIndex];

                string brand_name = supplier_item.Content;
#if WINFORMS
                brand_name = brand_name.TrimStart(SmartParametersV2016.defaultgreen).TrimEnd(SmartParametersV2016.defaultred);
#endif
                Lookup_Consumer_Info(ourviewmodel,
                                    utilityviewmodel,
                                    brand_name);
                //rf autoswitch_on) ;
                // This gives 'E' or 'G'
                bool simulate = utilityviewmodel.go_simulate; // bool simulate = components.simulate.Checked;
                if (simulate)
                {
                    simulate = Manipulate_Test(false);
                    //#if WINFORMS || WPF  || WINUI
                    //utilityviewmodel.StartDate,
                    //utilityviewmodel.EndDate,
                    //#Xelse
                    //utilityviewmodel.StartDate.Date,
                    //utilityviewmodel.EndDate.Date,
                    //#endif
                    //first_date,
                    //last_date);
                }

                // Despite everything and all the pain
                // your program is ABSOLUTELY beautiful Ray, and its ALL YOURS!
                if (!await Utility_Switchover(ourviewmodel, utilityviewmodel, false))
                {
                    FrontEndGUI.SetLedColour(ourviewmodel, 6, ourviewmodel.redColour);
                    if (!await SmartRoutinesV2018.CheckTrace(ourviewmodel, utilityviewmodel.utilityToken,
                    0, 0, "Problem 69: Switchover"))
                    {
                        return false;
                    }
                    //return false;
                }
                else
                {
                    if (!await DisplayUtilityMeterAsync(
#if WINFORMS
                                                    components,
#endif
                                                    ourviewmodel,
                                                    utilityviewmodel,
                                                    SmartParametersV2016.activeFlag,
                                                    SmartParametersV2016.defaultDate))
                    {
                        // We failed because of a Cancellation ... but which one? Check
                        if (!(ourviewmodel.quitCts.IsCancellationRequested ||
                            utilityviewmodel.utilityToken.IsCancellationRequested))
                        {
                            // If we DIDN'T request a cancellation
                            // We do nothing if there is a failure .. !  SmartSwitch DOESN'T fail!!
                            FrontEndGUI.SetLedColour(ourviewmodel, 8, ourviewmodel.redColour);
                        }
                        //return false;
                    }
                    else
                    {
                        if (autoswitch_on)
                        {
                            utilityviewmodel.AutoSwitchUtilityColour = ourviewmodel.redColour;
                            //#if ANDROIDX
                            //                            ourviewmodel.AutoSwitch0 = ourviewmodel.openred0;
                            //                            ourviewmodel.AutoSwitch25 = ourviewmodel.openred25;
                            //                            ourviewmodel.AutoSwitch50 = ourviewmodel.openred50;
                            //                            ourviewmodel.AutoSwitch75 = ourviewmodel.openred75;
                            //#endif
                        }
                        else
                        {
                            utilityviewmodel.AutoSwitchUtilityColour = ourviewmodel.magentaColour;
                            //#if ANDROIDX
                            //                            ourviewmodel.AutoSwitch0 = ourviewmodel.autoswitch0;
                            //                            ourviewmodel.AutoSwitch25 = ourviewmodel.autoswitch25;
                            //                            ourviewmodel.AutoSwitch50 = ourviewmodel.autoswitch50;
                            //                            ourviewmodel.AutoSwitch75 = ourviewmodel.autoswitch75;
                            //#endif
                        }
                        SmartRoutinesV2018.FocusOpenClose(ourviewmodel);
                        status = true;
                    }
                }
            }
            // We are going to get info? Better turn off some buttons first!
            // These can all be turned off now, except the Cancel button
            TurnOnUtilityStatus(
#if WINFORMS
                                 components,
#endif
                                 utilityviewmodel);
            return status;
        }
#endif

#if WINFORMS
        // Don't really do double-clic
        internal static bool ChartsMouseDoubleClick_Actual(MainViewModel ourviewmodel,
                                                            UtilityViewModel utilityviewmodel,
                                                            string[] tabinfo)
        {

            // Did we get here on a Double Click?
            if (tabinfo.Length == 2)   // <= This needs fixing
            {
                return true;
            }
            return false;
        }
#endif
        
#if WINFORMS
        internal static async Task<bool> ChartsMouseDoubleClick_ActualX(MainProcess process_components,
#endif
#if WPF
        internal static async Task<bool> ChartsMouseDoubleClick_Actual(
#endif
#if WINUI
        internal static async Task<bool> ChartsMouseDoubleClick_Actual(
#endif
#if ANDROIDX
        internal static async Task<bool> ChartsMouseDoubleClick_Actual(
                                                        AppCompatActivity meterActivity,
#endif
#if SMARTMAUI
        internal static async Task<bool> ChartsMouseDoubleClick_Actual(
#endif
                                                        SignInViewModel signinviewmodel,
                                                        MainViewModel ourviewmodel,
                                                        UtilityViewModel utilityviewmodel,
                                                        string[] tabinfo)
        {
            // We are going to get info? Better turn off some buttons first!
            // These can all be turned off now, except the Cancel button
            TurnOffUtilityStatus(
#if WINFORMS
                                     process_components,
#endif
                                    utilityviewmodel);
            bool status = false;
            if (!string.IsNullOrEmpty(tabinfo[0]))
            {
                // Pretty sure the Label doesn't have a parent?
                // However we can find out which tab item it is by working forward
                // from a TabControl statement which is underneath the Charts tab
                // and then down through all the TabItems below the TabControl ...
                // A kludge but .. can YOU work out the TabItem from the label??
                // No? Then fuck off, smart arse

                try
                {
#if WPF
                    utilityviewmodel.PopupHeight = Application.Current.MainWindow.Height;
                    utilityviewmodel.PopupWidth = Application.Current.MainWindow.Width;
                    utilityviewmodel.childWindow = new Window
                    {
                        WindowStartupLocation = WindowStartupLocation.CenterScreen,
                        Topmost = true,
                        Title = tabinfo[1],
                    };
                    Label titleLabel = new Label()
                    {
                        Content = tabinfo[1]
                    };
#endif
#if WINUI
                    utilityviewmodel.PopupHeight = ourviewmodel.currentHeight - ourviewmodel.currentHeight / 10;
                    utilityviewmodel.PopupWidth = ourviewmodel.currentWidth;
                    utilityviewmodel.childWindow = new Popup
                    {

                        Height = utilityviewmodel.PopupHeight,
                        Width = utilityviewmodel.PopupWidth//,
                        //DesiredPlacement = PopupPlacementMode.Auto
                        //Title = tablabel
                    };
                    TextBlock titleLabel = new TextBlock
                    {
                        Text = tabinfo[1],
                        FontWeight = Microsoft.UI.Text.FontWeights.Bold,
                        HorizontalAlignment = Microsoft.UI.Xaml.HorizontalAlignment.Center
                    };
#endif
#if SMARTMAUI
                    Double height = DeviceDisplay.Current.MainDisplayInfo.Height;
                    Double width = DeviceDisplay.Current.MainDisplayInfo.Width;
                    Double density = DeviceDisplay.Current.MainDisplayInfo.Density;

                    // Convert to device-independent units (like WPF)
                    Double heightDp = height / density;
                    Double widthDp = width / density;
                    utilityviewmodel.childWindow = new CommunityToolkit.Maui.Views.Popup
                    {
                        Size = new Size(widthDp, heightDp),
                        CanBeDismissedByTappingOutsideOfPopup = true
                    };

                    Label titleLabel = new Label
                    {
                        FontAttributes = FontAttributes.Bold,
                        HorizontalOptions = LayoutOptions.Center,
                        Text = tabinfo[1]
                    };
#endif
//Grid content = new Grid
//{
//    RowDefinitions =
//    {
//        new RowDefinition { Height = GridLength.Auto }, // title
//        new RowDefinition { Height = GridLength.Star }  // body
//    }
//};

//content.Add(titleLabel);
//Grid.SetRow(titleLabel, 0);

#if WPF  || WINUI || SMARTMAUI
                    Grid content = new Grid
                    {
                        RowDefinitions =
                        {
                            new RowDefinition { Height = GridLength.Auto }, // title
                            new RowDefinition { Height = new GridLength(1, GridUnitType.Star) }  // body
                        }
                    };

                    content.Children.Add(titleLabel);
                    Grid.SetRow(titleLabel, 0);
#endif
                    // Your existing popup content goes in row 1
#if WINFORMS
                    // This needs fixing
                    PlotView popup_plotview = new PlotView(); 
#endif
#if WPF
                    PlotView popup_plotview = new PlotView();
#endif
#if WINUI
#if OXYPLOT
                    PlotView popup_plotview = new PlotView();

#else
                    PlotModel popup_plotview = null;
#endif
#endif
#if WPF
                    utilityviewmodel.childWindow.Content = popup_plotview;
                    // Create some events so we can escape from the popup
                    utilityviewmodel.childWindow.MouseDoubleClick += new MouseButtonEventHandler((s, e) => UtilityView.Chart_MouseDoubleClick(s, e, utilityviewmodel));
                    utilityviewmodel.childWindow.MouseLeave += new MouseEventHandler((s, e) => UtilityView.Chart_MouseLeave(s, e, utilityviewmodel));
#endif
#if WINUI
                    // Have no idea how to do this

                    //utilityviewmodel.childWindow.Child = popup_plotview;

                    // Create some events so we can escape from the popup
                    //utilityviewmodel.childWindow.MouseDoubleClick += new MouseButtonEventHandler(UtilityView.Chart_MouseDoubleClick);
                    utilityviewmodel.childWindow.Tapped +=
                        new TappedEventHandler((s, e) => ChildWindowTapped(s, e, utilityviewmodel)); // += new MouseEventHandler(UtilityView.Chart_MouseLeave); 
                    //utilityviewmodel.childWindow.MouseLeave += new MouseEventHandler(UtilityView.Chart_MouseLeave);
#endif
#if ANDROIDX
                    //utilityviewmodel.PopupHeight = Application.Current.MainPage.Height;
                    //utilityviewmodel.PopupWidth = Application.Current.MainPage.Width;
                    UtilityChartsPopup charts_popup = new UtilityChartsPopup();
#endif
#if ANDROIDX
                    // This needs fixing
                    PlotView popup_plotview = new PlotView(meterActivity); // (PlotView)charts_popup.Resource.Id. FindByName("PopupPlotView");
#endif
#if SMARTMAUI
                    // What a fucking mess trying to get the Model in a Popup..
                    var canvas = new SKCanvasView
                    {
                        HeightRequest = 300,
                        WidthRequest = 300
                    };

                    canvas.PaintSurface += (s, e) =>
                    {
                        //using var rc = new SkiaRenderContext(e.Surface.Canvas);
                        //rc.Render(utilityviewmodel.PlotModel1, e.Info.Width, e.Info.Height);
                    };

                    //Content = new Border
                    //{
                    //    BackgroundColor = Colors.White,
                    //    Stroke = Colors.Black,
                    //    StrokeThickness = 1,
                    //    Padding = 10,
                    //    Content = canvas
                    //};
                    var tap = new TapGestureRecognizer();
                    tap.Tapped += (s, e) =>
                    {
                        utilityviewmodel.childWindow.Close();
                    };

                    //popup_plotview.GestureRecognizers.Add(tap);
                    //PlotView popup_plotview = new PlotView();
#endif
                    if (true)//popup_plotview != null)
                    {
#if !SMARTMAUI
                        PlotModel pmodel = new PlotModel();
#endif
                        // So we can change the header title in the XAML file
                        switch (tabinfo[0])
                        {
                            case "1":
                                await BuildCostsChart(
#if WINFORMS
                                    process_components,
#endif
                                    ourviewmodel, utilityviewmodel);
                                break;
                            case "2":
                                BuildReadingsChart(
#if WINFORMS
                                    process_components,
#endif
                                    ourviewmodel, utilityviewmodel);
                                break;
                            case "3":
                                BuildUsageChart(
#if WINFORMS
                                    process_components,
#endif
                                    ourviewmodel, utilityviewmodel);
                                break;
                            default:
                                break;
                        }

                        if (true)//popup_plotview != null)
                        {
#if OXYPLOT
                            popup_plotview.Model = pmodel;
#endif

                            string desc = SmartSpikeV2017.Lookup_Button_Description(signinviewmodel.Fatah.buttonsList,
                                                         SmartParametersV2016.Utility,
                                                         utilityviewmodel.resource_code); // <= BUTTON_CODE
                            utilityviewmodel.UtilityChartsPlotTitle = desc + " - " + (DateTime.Now + ourviewmodel.utcOffset).ToString(SmartParametersV2016.militaryFormat); // Local time

#if OXYPLOT                 // When plotview is REALLY a plotview
                            content.Add(popup_plotview);
                            Grid.SetRow(popup_plotview, 1);
                            utilityviewmodel.childWindow.Content = content;
#endif
#if WPF
                            // Show the pop-up window
                            utilityviewmodel.childWindow.Show();
#endif
#if WINUI
                            // Show the pop-up window
                            utilityviewmodel.childWindow.IsOpen = true;
#endif
#if ANDROIDX
                            // This needs fixing
                            GravityFlags gflags = new GravityFlags();
                            charts_popup.ShowAtLocation(popup_plotview, gflags, 0, 0);
#endif
#if SMARTMAUI
                            // Show the pop-up window
                            await Application.Current.MainPage.ShowPopupAsync(utilityviewmodel.childWindow);
#endif
                        }
                    }

                    // What an absolute fucking bitch all this PlotView/PlotModel stuff is ..
                    // When you copy the ItemSource, the Rows are copied ... but the Columns aren't!!                            
                    status = true;
                }
                catch (Exception ex)
                {
                    utilityviewmodel.errorMessage = ex.Message;
                    status = false;
                }
            }
            // We are going to get info? Better turn off some buttons first!
            // These can all be turned off now, except the Cancel button
            TurnOnUtilityStatus(
#if WINFORMS
                                  process_components,
#endif
                                 utilityviewmodel);
            return status;
        }

#if WINUI
        private static void ChildWindowTapped(object sender, TappedRoutedEventArgs e, UtilityViewModel utilityviewmodel)
        {
            if (sender != null && e != null)
            {
                utilityviewmodel.childWindow.IsOpen = false;
                //throw new NotImplementedException();
            }
            return;
        }
#endif

#if WPF
        internal static void Utility_ApplyWindowMouseButtons(System.Windows.UIElement glaze, UtilityViewModel utilityviewmodel)
        {
            if (glaze != null)
            {
                glaze.MouseLeftButtonDown += new MouseButtonEventHandler((s, e1) => Utility_PopupMouseLeftButtonDown(s, e1, utilityviewmodel));
                glaze.MouseLeave += new MouseEventHandler((s, e2) => Utility_PopupMouseLeave(s, e2, utilityviewmodel));
                //glaze.MouseLeftButtonDown += new MouseButtonEventHandler(Utility_PopupMouseLeftButtonDown);
                //glaze.MouseLeave += new System.Windows.Input.MouseEventHandler(Utility_PopupMouseLeave);
            }
            return;
        }
#endif

#if WPF
        internal static void Utility_ApplyGridMouseButtons(System.Windows.UIElement square, UtilityViewModel utilityviewmodel)
        {
            if (square != null)
            {
                square.MouseLeftButtonDown += new MouseButtonEventHandler((s, e1) => Utility_PopupMouseLeftButtonDown(s, e1, utilityviewmodel));
                //square.MouseLeftButtonDown += new MouseButtonEventHandler(Utility_PopupMouseLeftButtonDown);
            }
            return;
        }
#endif

#if ANDROIDX
        internal static void Utility_ApplyWindowMouseButtons(PopupWindow glaze)
        {
            if (glaze != null)
            {
                //glaze.PointerPressed += new PointerEventHandler(SmartRoutinesV2018.Utility_PopupMouseLeftButtonDown);
                //glaze.PointerExited += new PointerEventHandler(SmartRoutinesV2018.Utility_PopupMouseLeave);
            }
            return;
        }
#endif

#if ANDROIDX
        internal static void Utility_ApplyGridMouseButtons(Element square)
        {
            if (square != null)
            {
                //square.PointerPressed += new PointerEventHandler(SmartRoutinesV2018.Utility_PopupMouseLeftButtonDown);
            }
            return;
        }
#endif
#if SMARTMAUI
        internal static void Utility_ApplyGridMouseButtons(Microsoft.Maui.Controls.Element square)
        {
            if (square != null)
            {
                //square.PointerPressed += new PointerEventHandler(SmartRoutinesV2018.Utility_PopupMouseLeftButtonDown);
            }
            return;
        }
#endif


#if WINFORMS
        internal static bool Utility_TabMouseDoubleClick_Actual(string tabLabel,
                                                        MainViewModel ourviewmodel,
                                                        UtilityViewModel utilityviewmodel)
        {
#endif
#if WPF
        internal static bool Utility_TabMouseDoubleClick_Actual(string tabLabel,
                                                            MainViewModel ourviewmodel,
                                                            UtilityViewModel utilityviewmodel,
                                                            List<DataGridTextColumn> to_grid)
        {
#endif
#if WINUI
        internal static bool Utility_TabMouseDoubleClick_Actual(string tabLabel,
                                                            MainViewModel ourviewmodel,
                                                            UtilityViewModel utilityviewmodel,
                                                            List<DataGridTextColumn> to_grid)
        {
#endif
#if ANDROIDX
        internal static bool Utility_TabMouseDoubleClick_Actual(string tabLabel,
                                                                MainViewModel ourviewmodel,
                                                                UtilityViewModel utilityviewmodel)
        {
#endif
#if SMARTMAUI
        internal static bool Utility_TabMouseDoubleClick_Actual(string tabLabel,
                                                            MainViewModel ourviewmodel,
                                                            UtilityViewModel utilityviewmodel,
                                                            List<SmartRoutinesV2018.ColumnDefinitionModel> to_grid)
        {
#endif

            bool status = false;
            if (!string.IsNullOrEmpty(tabLabel))
            {
                switch (tabLabel)
                {
                    case "Charts":
                        break;
                    default:
#if WPF
                        utilityviewmodel.childWindow = new Window
                        {
                            SizeToContent = SizeToContent.WidthAndHeight,
                            WindowStartupLocation = WindowStartupLocation.CenterScreen,
                            Topmost = true,
                            Title = tabLabel
                        };
#endif
#if WINUI
                        utilityviewmodel.childWindow = new Popup
                        {
                            //SizeToContent = SizeToContent.WidthAndHeight,
                            //WindowStartupLocation = WindowStartupLocation.CenterScreen,
                            //Topmost = true,
                            //Title = tabLabel
                        };
#endif

#if SMARTMAUI
                        utilityviewmodel.childWindow = new Popup
                        {
                            //SizeToContent = SizeToContent.WidthAndHeight,
                            //WindowStartupLocation = WindowStartupLocation.CenterScreen,
                            //Topmost = true,
                            //Title = tabLabel
                            CanBeDismissedByTappingOutsideOfPopup = true
                        };
#endif
#if WPF  || WINUI
                        // Make a new DataGrid with double-click trapping
                        DataGrid child_datagrid = new DataGrid
                        {
                            AutoGenerateColumns = false,
                            IsReadOnly = true
                        };
                        foreach (DataGridColumn col in to_grid)
                        {
                            child_datagrid.Columns.Add(col);
                        }
#endif
#if SMARTMAUI
                        // Make a new DataGrid with double-click trapping
                        CollectionView child_datagrid =
                            SmartRoutinesV2018.CollectionViewFactory.BuildCollectionView(to_grid);
#endif
#if WPF
                        DockPanel target_dockpanel = new DockPanel();
#endif
#if WINUI
                        StackPanel target_dockpanel = new StackPanel();
#endif
#if ANDROIDX
                        // Just to get it out of the error list!!!
                        utilityviewmodel.childWindow = new PopupWindow();
                        Context con = Application.Context;
                        GridView child_datagrid = new GridView(con);
#endif
#if SMARTMAUI
                        VerticalStackLayout target_dockpanel = new VerticalStackLayout();
#endif
#if ANDROIDX
                        //ListView child_datagrid;
#endif
                        switch (tabLabel)
                        {
                            case "Costs":
                                // What an absolute fucking bitch all this DataGridView stuff is ..
                                // When you copy the ItemSource, the Rows are copied ... but the Columns aren't!!
                                // Make a new DataGrid with double-click trapping
#if WPF  || WINUI || SMARTMAUI
                                child_datagrid.ItemsSource = utilityviewmodel.UtilityCosts;
                                // What an absolute fucking bitch all this DataGridView stuff is ..
                                // More tea!!
#endif
#if ANDROIDX
                                con = Application.Context;
                                List<View> abc = new List<View>();
                                foreach (SmartUtility.AnalysisCostsView xyz in utilityviewmodel.UtilityCosts)
                                {
                                    TextView def = new TextView(con)
                                    {
                                        Text = xyz.BRAND_CODE.ToString()
                                    };
                                    abc.Add(def);
                                };
                                child_datagrid.AddChildrenForAccessibility(abc);
#endif
#if ANDROIDX
                                try
                                {
                                    utilityviewmodel.PopupHeight = ourviewmodel.currentHeight - ourviewmodel.currentHeight / 10;
                                    UtilityCostsPopup costs_popup = new UtilityCostsPopup();

                                    // Wait until we can get SHowPopup working from a ContentVIEW

                                    GravityFlags gflagsx = new GravityFlags();
                                    costs_popup.ShowAtLocation(child_datagrid, gflagsx, 0, 0);

                                    // What an absolute fucking bitch all this DataGridView stuff is ..
                                    // When you copy the ItemSource, the Rows are copied ... but the Columns aren't!!                            
                                    status = true;
                                }
                                catch (Exception ex)
                                {
                                    utilityviewmodel.errorMessage = ex.Message;
                                    //return false;
                                }
#endif
                                break;
                            case "Bills":
#if WPF  || WINUI || SMARTMAUI
                                child_datagrid.ItemsSource = utilityviewmodel.UtilityBills;
#endif
#if ANDROIDX
                                try
                                {
                                    utilityviewmodel.PopupHeight = ourviewmodel.currentHeight - ourviewmodel.currentHeight / 10;
                                    UtilityBillsPopup bills_popup = new UtilityBillsPopup();

                                    GravityFlags gflagsy = new GravityFlags();
                                    bills_popup.ShowAtLocation(child_datagrid, gflagsy, 0, 0);

                                    // What an absolute fucking bitch all this DataGridView stuff is ..
                                    // When you copy the ItemSource, the Rows are copied ... but the Columns aren't!!                            
                                    status = true;
                                }
                                catch (Exception ex)
                                {
                                    utilityviewmodel.errorMessage = ex.Message;
                                    //return false;
                                }
#endif
                                break;
                            case "Readings":
#if WPF  || WINUI || SMARTMAUI
                                child_datagrid.ItemsSource = utilityviewmodel.UtilityReadings;
#endif
#if ANDROIDX
                                try
                                {
                                    utilityviewmodel.PopupHeight = ourviewmodel.currentHeight - ourviewmodel.currentHeight / 10;
                                    UtilityReadingsPopup readings_popup = new UtilityReadingsPopup();

                                    GravityFlags gflagsz = new GravityFlags();
                                    readings_popup.ShowAtLocation(child_datagrid, gflagsz, 0, 0);
                                    // What an absolute fucking bitch all this DataGridView stuff is ..
                                    // When you copy the ItemSource, the Rows are copied ... but the Columns aren't!!                            
                                    status = true;
                                }
                                catch (Exception ex)
                                {
                                    utilityviewmodel.errorMessage = ex.Message;
                                    //return false;
                                }
#endif
                                break;
                            case "Breakdown":


#if WPF  || WINUI || SMARTMAUI
                                child_datagrid.ItemsSource = utilityviewmodel.UtilityBreakdown;
#endif
#if ANDROIDX
                                try
                                {
                                    utilityviewmodel.PopupHeight = ourviewmodel.currentHeight - ourviewmodel.currentHeight / 10;
                                    UtilityBreakdownPopup breakdown_popup = new UtilityBreakdownPopup();


                                    // Wait until we can get SHowPopup working from a ContentVIEW

                                    GravityFlags gflagsw = new GravityFlags();
                                    breakdown_popup.ShowAtLocation(child_datagrid, gflagsw, 0, 0);

                                    // What an absolute fucking bitch all this DataGridView stuff is ..
                                    // When you copy the ItemSource, the Rows are copied ... but the Columns aren't!!                            
                                    status = true;
                                }
                                catch (Exception ex)
                                {
                                    utilityviewmodel.errorMessage = ex.Message;
                                    //return false;
                                }
#endif
                                break;
                            // What an absolute bitch this stuff is.  Its a fucking minefield ..
                            default:
                                break;
                        }

#if WPF  || WINUI || SMARTMAUI

#if WPF  || WINUI
                        // Turn on scrolling
                        child_datagrid.VerticalScrollBarVisibility = ScrollBarVisibility.Visible;
                        // Fix the height
                        child_datagrid.MaxHeight = ourviewmodel.actualHeight;
                        child_datagrid.MaxWidth = ourviewmodel.actualWidth;
                        // Configure the escape routes
#endif
#if SMARTMAUI

                        child_datagrid.VerticalScrollBarVisibility = ScrollBarVisibility.Always;
#endif
#endif
#if WPF
                        child_datagrid.MouseLeftButtonDown += new MouseButtonEventHandler((s, e1) => Utility_PopupMouseLeftButtonDown(s, e1, utilityviewmodel));
                        utilityviewmodel.childWindow.MouseLeave += new MouseEventHandler((s, e2) => Utility_PopupMouseLeave(s, e2, utilityviewmodel));
                        //child_datagrid.MouseLeftButtonDown += new MouseButtonEventHandler(Utility_PopupMouseLeftButtonDown);
                        //utilityviewmodel.childWindow.MouseLeave += new MouseEventHandler(Utility_PopupMouseLeave);
#endif
#if SMARTMAUI
                        //child_datagrid.MouseLeftButtonDown += new MouseButtonEventHandler((s, e1) => Utility_PopupMouseLeftButtonDown(s, e1, utilityviewmodel));
                        utilityviewmodel.childWindow.Closed += ((s, e2) => Utility_PopupMouseLeave(s, e2, utilityviewmodel));
                        //child_datagrid.MouseLeftButtonDown += new MouseButtonEventHandler(Utility_PopupMouseLeftButtonDown);
                        //utilityviewmodel.childWindow.MouseLeave += new MouseEventHandler(Utility_PopupMouseLeave);
#endif
                        // And show the popup with the DataGrid inside
#if WPF
                        // What an absolute bitch this stuff is.  Its a fucking minefield ..
                        target_dockpanel.Children.Add(child_datagrid);
                        utilityviewmodel.childWindow.Content = target_dockpanel;
                        utilityviewmodel.childWindow.Show();
#endif
#if WINUI
                        // What an absolute bitch this stuff is.  Its a fucking minefield ..
                        target_dockpanel.Children.Add(child_datagrid);
                        utilityviewmodel.childWindow.Child = target_dockpanel;
                        utilityviewmodel.childWindow.IsOpen = true;
#endif
#if ANDROIDX
                        GravityFlags gflags = new GravityFlags();
                        utilityviewmodel.childWindow.ShowAtLocation(child_datagrid, gflags, 0, 0);
#endif
#if SMARTMAUI
                        // What an absolute bitch this stuff is.  Its a fucking minefield ..
                        target_dockpanel.Children.Add(child_datagrid);
                        utilityviewmodel.childWindow.Content = target_dockpanel;
                        Shell.Current.ShowPopupAsync(utilityviewmodel.childWindow);
#endif
                        status = true;
                        break;
                }
            }
            return status;
        }

#if WINFORMS
        // Its ok to use async void on Event Handlers
        internal static async void UtilityCostsMouseDoubleClick_Actual(DataGridView datagrid,
                                                                MainViewModel ourviewmodel,
                                                                UtilityViewModel utilityviewmodel,
                                                                UtilityView utilityview,
                                                                List<SmartUsers.VatRates> vatRatesList,
                                                                List<SmartUsers.ExchangeRates> exchangeRatesList)
#endif
#if WPF  || WINUI
        internal static async void UtilityCostsMouseDoubleClick_Actual(DataGrid viewlist,
                                                                MainViewModel ourviewmodel,
                                                                UtilityViewModel utilityviewmodel,
                                                                UtilityView utilityview,
                                                                List<SmartUsers.VatRates> vatRatesList,
                                                                List<SmartUsers.ExchangeRates> exchangeRatesList)
#endif
#if ANDROIDX
        internal static async void UtilityCostsMouseDoubleClick_Actual(PopupWindow contentpage,
                                                        SmartUtility.AnalysisCostsView viewlist,
                                                        MainViewModel ourviewmodel,
                                                        UtilityViewModel utilityviewmodel,
                                                        UtilityView utilityview,
                                                        string col_header,
                                                        List<SmartUsers.VatRates> vatRatesList,
                                                        List<SmartUsers.ExchangeRates> exchangeRatesList)
#endif
#if SMARTMAUI
        internal static async void UtilityCostsMouseDoubleClick_Actual(CollectionView viewlist,
                                                                MainViewModel ourviewmodel,
                                                                UtilityViewModel utilityviewmodel,
                                                                UtilityView utilityview,
                                                                List<SmartUsers.VatRates> vatRatesList,
                                                                List<SmartUsers.ExchangeRates> exchangeRatesList)
#endif
        {
            utilityviewmodel.local_resource_code = utilityviewmodel.resource_code;
            string resource_type = "";
            char comparison_payment_plan = SmartParametersV2016.defaultChar;
            //short supplier_code = 0,
            //        brand_code = 0;
            int age = 0;
            //var abc = ourviewmodel.administrator;
            // Sometimes? this shit is null ... hence the test

#if WINFORMS
            SmartUtility.AnalysisCostsView analysis_costs_row = new SmartUtility.AnalysisCostsView();
#endif
#if WPF  || WINUI || SMARTMAUI

            SmartUtility.AnalysisCostsView analysis_costs_row = (SmartUtility.AnalysisCostsView)viewlist.SelectedItem;
            // Couldn't have EVER down this without the help of Eric Schmeck
            // https://forums.xamarshit.com/discussion/46785/how-to-cast-java-lang-object-to-specified-class-cant-convert-type-java-lang-object-tomyclass?
            //PropertyInfo bob = datagrid.SelectedItem.GetType().GetProperty("Instance");
            //AnalysisCostsView analysis_costs_row = bob.GetValue(datagrid.SelectedItem, null) as AnalysisCostsView;
#endif
#if ANDROIDX
            SmartUtility.AnalysisCostsView analysis_costs_row = viewlist;

#endif
            if (analysis_costs_row != null)
            {
                if (Utility_Check_Analysis_Costs(analysis_costs_row))
                {
                    if (!Utility_Some_Checks(ourviewmodel,
                                            utilityviewmodel,
                                            analysis_costs_row.SUPPLIER_CODE,
                                            analysis_costs_row.BRAND_CODE,
                                            analysis_costs_row.TARIFF_CODE))
                    //analysis_costs_row.RESOURCE_TYPE,
                    {
                        return;
                    }
#if WINFORMS
                    string column_header = "Something";
#endif

#if WPF  || WINUI || SMARTMAUI
#if WPF
                    utilityviewmodel.childWindow = new Window
                    {
                        SizeToContent = SizeToContent.WidthAndHeight,
                        WindowStartupLocation = WindowStartupLocation.CenterScreen,
                        Topmost = true
                    };
#endif
#if WINUI
                    utilityviewmodel.childWindow = new Popup
                    {
                        //SizeToContent = SizeToContent.WidthAndHeight,
                        //WindowStartupLocation = WindowStartupLocation.CenterScreen,
                        //Topmost = true
                    };
#endif
#if SMARTMAUI
                    utilityviewmodel.childWindow = new Popup();
                    // ALL THE STUFF FROM CHARTS!!
                    //{
                    //    SizeToContent = SizeToContent.WidthAndHeight,
                    //    WindowStartupLocation = WindowStartupLocation.CenterScreen,
                    //    Topmost = true
                    //};
#endif
#if WPF
                    DockPanel target_dockpanel = new DockPanel();
#endif
#if WINUI
                    StackPanel target_dockpanel = new StackPanel();
#endif
                    // I have no idea how to find out which column its under!!

                    //SmartUtility.AnalysisCostsView item = viewlist.SelectedItem as SmartUtility.AnalysisCostsView;
                    //string column_header = viewlist.CurrentColumn.Header.ToString();

                    string column_header = "Supplier"; // Fix it for now 
#endif
#if ANDROIDX
                    //var rootFrame = new Frame();

                    // Place the frame in the current Window and ensure that it is active
                    //Window.Current.Content = rootFrame;

                    //{
                    //SizeToContent = SizeToContent.WidthAndHeight,
                    //WindowStartupLocation = WindowStartupLocation.CenterScreen,
                    //Topmost = true
                    //};
                    //DockPanel target_dockpanel = new DockPanel();

                    string column_header = col_header;
                    //Window.Current.Content = target_dockpanel;
                    //Window.Current.Activate();
#endif
#if SMARTMAUI
                    StackLayout target_dockpanel = new StackLayout();
#endif
                    switch (column_header)
                    {
                        case "Supplier":
#if WPF  || WINUI || SMARTMAUI
                            // Was this fucking painful, or what?  Couldn't get the fucking
                            // text to clean up its lines before sending ....
#if WPF
                            utilityviewmodel.childWindow.Title = "Report - " + analysis_costs_row.SUPPLIER_NAME;


                            // God - was THIS fucking painful or what???  ALl this just to get the
                            // title fully shown at the top of the box. Fucking , fucking Microshit
                            FontFamily FontFamily = new FontFamily("Arial");
                            FontWeight FontWeight = System.Windows.FontWeights.Bold;
                            FontStyle FontStyle = new FontStyle();
                            int FontSize = 12;
                            Typeface Typeface = new Typeface(FontFamily, FontStyle, FontWeight, FontStretches.Normal);

                            //FormattedText ft = new FormattedText(utilityviewmodel.childWindow.Title,
                            //                        utilityviewmodel.utilityDisplayCulture,
                            //                        FlowDirection.LeftToRight,
                            //                        Typeface, FontSize, Brushes.Black);
                            FormattedText ft = new FormattedText(utilityviewmodel.childWindow.Title,
                                            CultureInfo.GetCultureInfo("en-GB"),
                                            FlowDirection.LeftToRight,
                                            Typeface,
                                            FontSize,
                                            Brushes.Black,
                                            VisualTreeHelper.GetDpi(utilityview).PixelsPerDip);
                            utilityviewmodel.childWindow.Width = ft.Width;
#endif
#if SMARTMAUI
                            var label = new Label
                            {
                                Text = column_header,
                                FontFamily = "Arial",
                                FontAttributes = FontAttributes.Bold,
                                FontSize = 12,
                                TextColor = Colors.Black
                            };

                            var formatted = new FormattedString();

                            formatted.Spans.Add(new Span
                            {
                                Text = column_header,
                                FontAttributes = FontAttributes.Bold,
                                FontSize = 12
                            });

                            label.FormattedText = formatted;

                            

                            var font = Microsoft.Maui.Font.OfSize("Arial", 12);

                            //var size = TextMeasurer.Default.Measure(
                            //    column_header,
                            //    font,
                            //    0 // no width constraint
                            //);
                            var size = MeasureText(column_header, 12);

#endif
                            string supplier_report = SmartReportV2016.Fill_Supplier_Report(utilityviewmodel,
                                                                                                analysis_costs_row.SUPPLIER_NAME);

#if WPF 
                            target_dockpanel.Children.Add(SmartRoutinesV2018.CreateDataGrid(SmartRoutinesV2018.TextColumn("Description", "description"), utilityviewmodel.childWindow.Title.ToString().Trim()));
#endif

#if SMARTMAUI
                            target_dockpanel.Children.Add(SmartRoutinesV2018.CreateDataGrid("Description", supplier_report.ToString().Trim()));
#endif


#if WPF
                            Utility_ApplyWindowMouseButtons(utilityviewmodel.childWindow, utilityviewmodel);

                            utilityviewmodel.childWindow.Content = target_dockpanel;
                            utilityviewmodel.childWindow.Show();
#endif
#if WINUI
                            //Utility_ApplyWindowMouseButtons(utilityviewmodel.childWindow);

                            utilityviewmodel.childWindow.Child = target_dockpanel;
                            utilityviewmodel.childWindow.IsOpen = true;
#endif
#endif
#if ANDROIDX
                            // Was this fucking painful, or what?  Couldn't get the fucking
                            // text to clean up its lines before sending ....

                            //UtilityView.childWindow.Title = "Report - " + analysis_costs_row.SupplierName;
                            string supplier_report = SmartReportV2016.Fill_Supplier_Report(utilityviewmodel,
                                                                                            analysis_costs_row.SUPPLIER_NAME);


                            //target_dockpanel.Children.Add(SmartRoutinesV2018.CreateDataGrid(SmartRoutinesV2018.TextColumn("Description", "description"), supplier_report.ToString().Trim()));

                            //target_dockpanel.Children.Add(DataGrid(TextColumn("Description", "description"), supplier_report.ToString().Trim()));

                            //ApplyWindowMouseButtons(utilityviewmodel.childWindow);

                            //utilityviewmodel.childWindow.Content = target_dockpanel;
                            //utilityviewmodel.childWindow.Activate();
                            //TransactionDescription transactiondescription_popup = new TransactionDescription();
                            //await PopupNavigation.Instance.PushAsync(transactiondescription_popup);

                            SupplierDetails supplierdetails_popup = new SupplierDetails();

                            Android.Widget.ListView abc = null;
                            foreach (string xyz in supplier_report.Split('\n'))
                            {
                                Android.Widget.TextView ray = null;
                                ray.Text = xyz;
                                abc.AddChildrenForAccessibility((IList<Android.Views.View>)ray);
                            }
                            GravityFlags gflags = new GravityFlags();
                            supplierdetails_popup.ShowAtLocation(abc, gflags, 0, 0);
#endif
                            break;

                        case "Tariff":
#if WPF  || WINUI || SMARTMAUI
#if WPF
                            //utilityviewmodel.childWindow.Title = "Report - " + analysis_costs_row.SUPPLIER_NAME + ": " + analysis_costs_row.TARIFF_NAME;
                            utilityviewmodel.childWindow.Title = "Report - " + analysis_costs_row.SUPPLIER_NAME + ": " + analysis_costs_row.TARIFF_NAME;
#endif
#if SMARTMAUI
                            string title = "Report - " + analysis_costs_row.SUPPLIER_NAME + ": " + analysis_costs_row.TARIFF_NAME;
#endif
                            utilityviewmodel.tariff_report = "";

                            Utility_Tariff_Switch(ourviewmodel,
                                                    utilityviewmodel,
                                                    utilityviewmodel.local_resource_code,
                                                    analysis_costs_row.SUPPLIER_NAME,
                                                    analysis_costs_row.SUPPLIER_CODE,
                                                    analysis_costs_row.TARIFF_CODE,
                                                    analysis_costs_row.RESOURCE_TYPE);

                            //var abc = SmartRoutinesV2018.TextColumn("Description", "description");
#if WPF 
                            target_dockpanel.Children.Add(SmartRoutinesV2018.CreateDataGrid(SmartRoutinesV2018.TextColumn("Description", "description"), utilityviewmodel.childWindow.Title.ToString().Trim()));
#endif
#if SMARTMAUI
                            target_dockpanel.Children.Add(SmartRoutinesV2018.CreateDataGrid("Description", utilityviewmodel.tariff_report.Trim()));
#endif
#if WPF
                            Utility_ApplyWindowMouseButtons(utilityviewmodel.childWindow, utilityviewmodel);

                            utilityviewmodel.childWindow.Content = target_dockpanel;
                            utilityviewmodel.childWindow.Show();
#endif
#if WINUI
                            //Utility_ApplyWindowMouseButtons(utilityviewmodel.childWindow);

                            utilityviewmodel.childWindow.Child = target_dockpanel;
                            utilityviewmodel.childWindow.IsOpen = true;
#endif
#endif
#if ANDROIDX
                            utilityviewmodel.TariffDescriptionTitle = "Report - " + viewlist.SUPPLIER_NAME + " - " + viewlist.TARIFF_NAME;
                            utilityviewmodel.tariff_report = "";

                            Utility_Tariff_Switch(ourviewmodel,
                                                    utilityviewmodel,
                                                    utilityviewmodel.local_resource_code,
                                                    analysis_costs_row.SUPPLIER_NAME,
                                                    analysis_costs_row.SUPPLIER_CODE,
                                                    analysis_costs_row.TARIFF_CODE,
                                                    analysis_costs_row.RESOURCE_TYPE);

                            //SmartRoutinesV2018.TextColumn("Description", "description");

                            string[] lines = utilityviewmodel.tariff_report.Split(SmartParametersV2016.newline);
                            utilityviewmodel.Tariff.Clear();
                            int count = 0;
                            foreach (string line in lines)
                            {
                                string trimmed = line.Trim();
                                string[] bits = trimmed.Split(SmartParametersV2016.colon);
                                string fields = "";
                                string values = "";
                                if (bits.Length > 0)
                                {
                                    if (bits.Length == 1)
                                    {
                                        if (count == 0)
                                        {
                                            fields = bits[0].Trim();
                                            values = String.Empty;
                                        }
                                        else
                                        {
                                            fields = "";
                                            values = bits[0].Trim();
                                        }
                                    }
                                    else
                                    {
                                        fields = bits[0].Trim();
                                        values = SmartParametersV2016.colon + SmartParametersV2016.space + bits[1].Trim();
                                    }
                                }
                                UtilityViewModel.TariffDescriptionList abcde = new UtilityViewModel.TariffDescriptionList()
                                {
                                    Field = fields,
                                    Value = values
                                };
                                utilityviewmodel.Tariff.Add(abcde);
                                count++;
                            }

                            TariffDescription tariffdescription_popup = new TariffDescription();

                            Android.Widget.ListView abcd = null;
                            foreach (UtilityViewModel.TariffDescriptionList xyz in utilityviewmodel.Tariff)
                            {
                                // This needs fixing
                                //Android.Widget.ListView ray = (Android.Widget.ListView)xyz;
                                //abcd.AddChildrenForAccessibility((IList<Android.Views.View>)ray);
                            }
                            GravityFlags gflagsv = new GravityFlags();
                            // This needs fixing
                            tariffdescription_popup.ShowAtLocation(abcd, gflagsv, 0, 0);
#endif
                            break;

                        case "Total Cost":
                            if (!string.IsNullOrEmpty(analysis_costs_row.PAYMENT_PLAN))
                            {
                                // Extract first plan (its all we need)
                                comparison_payment_plan = Convert.ToChar(analysis_costs_row.PAYMENT_PLAN);
                            }

                            short target_supplier_code = 0;           // For debugging
                            int target_tariff_code = 0;             // For debugging
                            char target_payment_plan = SmartParametersV2016.defaultChar;  // For debugging

                            utilityviewmodel.from_date = SmartParametersV2016.defaultDate;
                            utilityviewmodel.to_date = SmartParametersV2016.defaultDate;
                            if (Cellsdc_Analyze_Check(utilityviewmodel,
                                                    utilityviewmodel.local_resource_code,
                                                    utilityviewmodel.e_engine_dates,
                                                    utilityviewmodel.g_engine_dates))
                            {
                                List<SmartUtility.AnalysisConditions> already_doneList = new List<SmartUtility.AnalysisConditions>();
                                if (!await SmartAnalyzeV2016.Analyze_Costs(ourviewmodel,
                                                            utilityviewmodel,
                                                            false,                              // Will not delete from AC
                                                            utilityviewmodel.area_code,
                                                            utilityviewmodel.local_resource_code,
                                                            resource_type,
                                                            analysis_costs_row.BRAND_CODE,       // Brand
                                                            analysis_costs_row.SUPPLIER_CODE,    // Supplier
                                                            analysis_costs_row.SUPPLIER_CODE,    // Supplier
                                                            analysis_costs_row.BRAND_CODE,       // Brand
                                                            analysis_costs_row.TARIFF_CODE,
                                                            comparison_payment_plan,
                                                            target_supplier_code,
                                                            target_tariff_code,
                                                            target_payment_plan,
                                                            utilityviewmodel.from_date, // These are global
                                                            age,
                                                            utilityviewmodel.withdrawn_date,
                                                            already_doneList))
                                {
                                    if (!(ourviewmodel.quitCts.IsCancellationRequested ||
                                            utilityviewmodel.utilityToken.IsCancellationRequested))
                                    {
                                        FrontEndGUI.SetLedColour(ourviewmodel, 8, ourviewmodel.redColour);
                                        if (!await SmartRoutinesV2018.CheckTrace(ourviewmodel, utilityviewmodel.utilityToken, 0, 0, "Problem 70: AnalyzeCosts"))
                                        {
                                            return;
                                        }
                                    }
                                    return;
                                }
                            }

#if WPF  || WINUI || SMARTMAUI
                            // We have to be 'good' to have got here, otherwise we would have return(ed)
#if WPF
                            //utilityviewmodel.childWindow.Title = "Breakdown - " + analysis_costs_row.SUPPLIER_NAME + ": " + analysis_costs_row.TARIFF_NAME;

                            utilityviewmodel.childWindow.Title = "Breakdown - " + analysis_costs_row.SUPPLIER_NAME + ": " + analysis_costs_row.TARIFF_NAME; ;
#endif
#if SMARTMAUI
                            string Title = "Breakdown - " + analysis_costs_row.SUPPLIER_NAME + ": " + analysis_costs_row.TARIFF_NAME; ;
#endif
                            //// What an absolute fucking bitch all this DataGridView stuff is ..
                            //// This next line is an OUT-OF-BROWSER function which DOESN'T WORK In-Browser (its left in so I can rememeber how to do it ...)
                            ////DataGrid fuckers = (DataGrid)Application.Current.Content.FindName("dataGridBreakdown");


                            // Add the new datagrid to the Child window
                            target_dockpanel.Children.Add(Breakdown_Data_Grid(ourviewmodel,
                                                                                utilityviewmodel,
                                                                                ourviewmodel.actualHeight,
                                                                                ourviewmodel.actualWidth));
                            //SmartParametersV2016.defaultDate));
#if WPF || SMARTMAUI
#if WPF
                            // Configure the escape routes
                            Utility_ApplyWindowMouseButtons(utilityviewmodel.childWindow, utilityviewmodel);
#endif
                            utilityviewmodel.childWindow.Content = target_dockpanel;



#endif
#if WPF
                            utilityviewmodel.childWindow.Show();
#endif
#if SMARTMAUI
                            await Shell.Current.ShowPopupAsync(utilityviewmodel.childWindow);
#endif
#if WINUI
                            // Configure the escape routes
                            //Utility_ApplyWindowMouseButtons(utilityviewmodel.childWindow);
                            utilityviewmodel.childWindow.Child = target_dockpanel;
                            utilityviewmodel.childWindow.IsOpen = true;
#endif
#endif
#if ANDROIDX
                            // We have to be 'good' to have got here, otherwise we would have return(ed)
                            //utilityviewmodel.childWindow.SetTitleBar("Breakdown - " + analysis_costs_row.SupplierName + ": " + analysis_costs_row.TariffName);


                            // Configure the escape routes
                            //UtilityView.ApplyWindowMouseButtons(UtilityView.childWindow);

                            //  FIX THIS ===>>> //TurnOnGridMouseButtons(child_datagrid);

                            // Add the new datagrid to the Child window
                            //target_dockpanel.Children.Add(Breakdown_Data_Grid(utilityviewmodel,
                            //                                                    SmartParametersV2016.defaultDate));
                            //utilityviewmodel.childWindow.Content = target_dockpanel;
                            //utilityviewmodel.childWindow.Activate();
#endif
                            break;
                        default:
                            break;

                    }
                }
            }
            return;
        }

#if WPF
        internal static Size MeasureText(string text, double fontSize, string fontFamily = "Arial")
        {
            var textBox = new TextBox
            {
                Text = text,
                FontSize = fontSize,
                FontFamily = new FontFamily(fontFamily)
            };

            textBox.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));

            return textBox.DesiredSize;
        }
#endif
#if SMARTMAUI
        internal static Size MeasureText(string text, double fontSize, string fontFamily = null)
        {
            var label = new Label
            {
                Text = text,
                FontSize = fontSize,
                FontFamily = fontFamily
            };

            SizeRequest result = label.Measure(double.PositiveInfinity, double.PositiveInfinity);
            return result.Request;
        }
#endif
        internal static async Task<bool> Utility_Switchover(
#if ANDROIDX
                                            AppCompatActivity meterActivity,
#endif
                                            MainViewModel ourviewmodel,
                                            UtilityViewModel utilityviewmodel,
                                            bool reset_all)
        {
            // We are going to get info? Better turn off some buttons first!
            // These can all be turned off now, except the Cancel button

            bool status = false;
            // Only do switches between Electricity, Gas and Dual            
            if (reset_all)
            {
                foreach (SmartUtility.Resources resource_row in utilityviewmodel.Hezbollah.utility_resourcesList)
                {
                    resource_row.CHECKED = "";
                    resource_row.Updated = true;
                    utilityviewmodel.Hezbollah.utility_resources_changesList.Add(resource_row);
                }
            }
            else
            {
                foreach (SmartUtility.Resources resource_row in utilityviewmodel.Hezbollah.utility_resourcesList)
                {
                    if (resource_row.Updated)
                    {
                        utilityviewmodel.Hezbollah.utility_resources_changesList.Add(resource_row);
                    }
                }
            }

            if (utilityviewmodel.Hezbollah.utility_resources_changesList.Count > 0)
            {
                // Record the switch ...and ..
                // Update the internal DB
                if (await SmartUtilityScrapeV2022.DoAllSmartUtility(ourviewmodel,
                                                utilityviewmodel,
                                                false,
                                                SmartParametersV2016.sqliteformat))
                {
                    // Tell the console we have switched
                    await SmartRoutinesV2018.TextBlockUpdate(
#if ANDROIDX
                        meterActivity,
#endif
                        ourviewmodel, "Utility Type switched to: " + utilityviewmodel.resourceCodes);
                        status = true;
                }
            }
            return status;
        }

        internal static async Task<bool> Utility_LoginSetup(
#if WINFORMS
                                                    MainProcess process_components,
#endif
#if ANDROIDX
                                                    AppCompatActivity meterActivity,
#endif
                                                    MainViewModel ourviewmodel,
                                                    UtilityViewModel utilityviewmodel,
                                                    bool local_found_it,
                                                    bool delete_selected,
                                                    Action<string> set_message)
        {
            // We are going to get info? Better turn off some buttons first!
            // These can all be turned off now, except the Cancel button
            TurnOffUtilityStatus(
#if WINFORMS
                                     process_components,
#endif
                                 utilityviewmodel);
            bool status = false;
            try
            {
                if (delete_selected)
                {
                    // Delete? Its all in Utility_Logins_ChangesList
                    //utilityviewmodel.Hezbollah.utility_logins_deletesList.Add(utilityviewmodel.login_info);
                }
                else
                {
                    if (string.IsNullOrEmpty(utilityviewmodel.newlogin_info.USER_ID.Trim()) ||
                        string.IsNullOrEmpty(utilityviewmodel.newlogin_info.USER_PASSWORD.Trim()))
                    {
                        set_message("Logindetailsincomplete");
                        status = true;
                    }
                    if ((utilityviewmodel.newlogin_info.USER_ID.Trim() == utilityviewmodel.login_info.USER_ID) &&
                        (utilityviewmodel.newlogin_info.USER_PASSWORD.Trim() == utilityviewmodel.login_info.USER_PASSWORD))
                    {
                        // They are both the same - do nothing - for now
                        set_message("No changes made");
                        status = true;
                    }
                }
                if (!delete_selected && !status)    // status still false
                {
                    int randomkey = SmartRoutinesV2018.GetNextRandomNew(ourviewmodel.randomR);
                    SmartUtility.Logins logins_row = new SmartUtility.Logins()
                    {
                        USERNAME = utilityviewmodel.login_info.USERNAME,
                        CUBEFACE_CODE = utilityviewmodel.login_info.CUBEFACE_CODE,
                        SUPPLIER_CODE = utilityviewmodel.login_info.SUPPLIER_CODE,
                        BRAND_CODE = utilityviewmodel.login_info.BRAND_CODE,
                        // Login Created done in a mo ...
                        USER_ID = utilityviewmodel.newlogin_info.USER_ID.Trim(),
                        USER_PASSWORD = utilityviewmodel.newlogin_info.USER_PASSWORD.Trim(),
                        RANDOMKEY = randomkey
                    };
                    // Login Created done here (a mo later ...)
                    if (!local_found_it)
                    {
                        logins_row.LOGIN_CREATED = DateTime.UtcNow;   // UTC time
                        logins_row.Updated = false;
                    }
                    else
                    {
                        logins_row.LOGIN_CREATED = utilityviewmodel.login_info.LOGIN_CREATED;
                        logins_row.Updated = true;
                    }
                    ;
                    // Insert or Update? Its all in Utility_Logins_ChangesList
                    utilityviewmodel.Hezbollah.utility_logins_changesList.Add(logins_row);

                    // At about this point we CAN'T pass the login_info BACK because
                    // this Popup has been called and the calling routine has moved on!
                    // If Updated = true, then the record already exists
                    // If Updated = false then we need to create a new record ..
                }

                // Record the event
                if (await SmartUtilityScrapeV2022.DoAllSmartUtility(ourviewmodel,
                                            utilityviewmodel,
                                            false,
                                            SmartParametersV2016.sqliteformat,
                                            utilityviewmodel.login_info.SUPPLIER_CODE,
                                            utilityviewmodel.login_info.BRAND_CODE))
                {

            // We've added (or deleted) a new connection - how do we turn British Gas green or black?
            //int brand_index = 0;
#if WINFORMS || WPF  || WINUI || SMARTMAUI
            utilityviewmodel.errorMessage = Build_Utility_Dropdowns(
#endif
#if ANDROIDX
                    utilityviewmodel.errorMessage = Build_Utility_Dropdowns(
#endif
#if WINFORMS
                                                                                        process_components,
#endif
                                                                                        ourviewmodel,
                                                                                        utilityviewmodel);
                    //rf brand_index);
                    if (string.IsNullOrEmpty(utilityviewmodel.errorMessage))
                    {
                        // Tell the console we have switched
                        await SmartRoutinesV2018.TextBlockUpdate(
#if ANDROIDX
                            meterActivity,
#endif
                            ourviewmodel, "Login info changed for: " + utilityviewmodel.login_info.SUPPLIER_CODE);
                        status = true;
                    }
                    else
                    {
                        await SmartRoutinesV2018.TextBlockUpdate(
#if ANDROIDX
                            meterActivity,
#endif
                            ourviewmodel, "Utility Build Suppliers failed: " + utilityviewmodel.login_info.SUPPLIER_CODE);
                        set_message("Problem 74: Utility_Build_Suppliers");
                        //return false;
                    }
                }
            }
            catch (Exception ex)
            {
                set_message(ex.Message);
                status = false;
            }
            TurnOnUtilityStatus(
#if WINFORMS
                                     process_components,
#endif
                                 utilityviewmodel);
            return status;
        }

#if WINFORMS
        internal static void UtilityLoginParams(System.Windows.Forms.TextBox UserId,
                                    System.Windows.Forms.TextBox Password,
                                    SmartUtility.Logins oldlogin_info,
                                    List<SmartUtility.BrandConnection> params_found)
#endif
#if WPF  || WINUI
        internal static void UtilityLoginParams(TextBox UserId,
                                    TextBox Password,
                                    SmartUtility.Logins oldlogin_info,
                                    List<SmartUtility.BrandConnection> params_found)
#endif
#if ANDROIDX
        internal static void UtilityLoginParams(EditText UserId,
                                    EditText Password,
                                    SmartUtility.Logins oldlogin_info,
                                    List<SmartUtility.BrandConnection> params_found)
#endif
#if SMARTMAUI
        internal static void UtilityLoginParams(Entry UserId,
                                    Entry Password,
                                    SmartUtility.Logins oldlogin_info,
                                    List<SmartUtility.BrandConnection> params_found)
#endif
        {
            // Do the Params
            UserId.Text = oldlogin_info.USER_ID;
            if (string.IsNullOrEmpty(UserId.Text) && params_found.Count >= 1)
            {
#if WINFORMS || WPF  || WINUI || SMARTMAUI
                UserId.Text = params_found[0].PARAMETER_PROMPT;
#endif
#if ANDROIDX
                // This should be a Placeholder/Hint? But
                // I don't know how to do that yet
                UserId.Text = params_found[0].PARAMETER_PROMPT;
#endif
            }

            Password.Text = oldlogin_info.USER_PASSWORD;
            if (string.IsNullOrEmpty(Password.Text) && params_found.Count >= 4)
            {
#if WINFORMS || WPF  || WINUI || SMARTMAUI
                Password.Text = params_found[1].PARAMETER_PROMPT;
#endif
#if ANDROIDX
                // This should be a Placeholder/Hint? But
                // I don't know how to do that yet
                Password.Text = params_found[1].PARAMETER_PROMPT;
#endif
            }
            return;
        }

#if WINFORMS
        internal static bool Utility_Supplier_Selected_Actual(MainViewModel ourviewmodel,
                                                UtilityViewModel utilityviewmodel,
                                                string supplier_text)
#endif
#if WPF  || WINUI
        internal static bool Utility_Supplier_Selected_Actual(MainViewModel ourviewmodel,
                                                UtilityViewModel utilityviewmodel,
                                                UtilityView utilityview,
                                                string supplier_text)
#endif
#if ANDROIDX
        internal static bool Utility_Supplier_Selected_Actual(MainViewModel ourviewmodel,
                                                UtilityViewModel utilityviewmodel,
                                                string supplier_text)
#endif
#if SMARTMAUI
        internal static bool Utility_Supplier_Selected_Actual(MainViewModel ourviewmodel,
                                                UtilityViewModel utilityviewmodel,
                                                UtilityView utilityview,
                                                string supplier_text)
#endif
        {
#if WPF  || WINUI || SMARTMAUI
#if WPF
            utilityviewmodel.childWindow = new Window
            {
                SizeToContent = SizeToContent.WidthAndHeight,
                WindowStartupLocation = WindowStartupLocation.CenterScreen,
                Topmost = true
            };
#endif
#if WINUI
            utilityviewmodel.childWindow = new Popup
            {
                //SizeToContent = SizeToContent.WidthAndHeight,
                //WindowStartupLocation = WindowStartupLocation.CenterScreen,
                //Topmost = true
            };
#endif
#if SMARTMAUI
            utilityviewmodel.childWindow = new Popup();
            // Then all the CHARTS Title shit
            //{
            //    SizeToContent = SizeToContent.WidthAndHeight,
            //    WindowStartupLocation = WindowStartupLocation.CenterScreen,
            //    Topmost = true
            //};
#endif
#if WPF
            DockPanel target_dockpanel = new DockPanel();
#endif
#if WINUI
            StackPanel target_dockpanel = new StackPanel();
#endif
#if SMARTMAUI
            StackLayout target_dockpanel = new StackLayout();
#endif
            // Was this fucking painful, or what?  Couldn't get the fucking
            // text to clean up its lines before sending ....
#if WPF
            utilityviewmodel.childWindow.Title = "Report - " + supplier_text;
#endif

            // God - was THIS fucking painful or what???  ALl this just to get the
            // title fully shown at the top of the box. Fucking , fucking Microshit
#if WPF
            FontFamily FontFamily = new FontFamily("Arial");

            FontWeight FontWeight = System.Windows.FontWeights.Bold;
            FontStyle FontStyle = new FontStyle();
            int FontSize = 12;
            Typeface Typeface = new Typeface(FontFamily, FontStyle, FontWeight, FontStretches.Normal);

            FormattedText ft = new FormattedText(utilityviewmodel.childWindow.Title,
                                            CultureInfo.GetCultureInfo("en-GB"),
                                            FlowDirection.LeftToRight,
                                            Typeface,
                                            FontSize,
                                            Brushes.Black,
                                            VisualTreeHelper.GetDpi(utilityview).PixelsPerDip);
            utilityviewmodel.childWindow.Width = ft.Width;
#endif
#if SMARTMAUI
            // Convert this AS BEFORE
#endif
            string supplier_report = SmartReportV2016.Fill_Supplier_Report(utilityviewmodel,
                supplier_text);
#if WPF 
            target_dockpanel.Children.Add(SmartRoutinesV2018.CreateDataGrid(SmartRoutinesV2018.TextColumn("Description", "description"), utilityviewmodel.childWindow.Title.ToString().Trim()));
#endif
#if SMARTMAUI
            target_dockpanel.Children.Add(SmartRoutinesV2018.CreateDataGrid("Description", supplier_report.Trim()));
#endif
#if WPF
            Utility_ApplyWindowMouseButtons(utilityviewmodel.childWindow, utilityviewmodel);

            utilityviewmodel.childWindow.Content = target_dockpanel;
            utilityviewmodel.childWindow.Show();
#endif
#if WINUI
            //Utility_ApplyWindowMouseButtons(utilityviewmodel.childWindow);

            utilityviewmodel.childWindow.Child = target_dockpanel;
            utilityviewmodel.childWindow.IsOpen = true;
#endif
#endif
#if ANDROIDX
            SupplierDetails supplier_page = new SupplierDetails();

            // This needs fixing and testing
            string supplier_report = SmartReportV2016.Fill_Supplier_Report(utilityviewmodel,
                                                                                supplier_text);

            Android.Widget.ListView abcd = null;
            foreach (string xyz in supplier_report.Split("\n"))
            {
                // This needs fixing
                Android.Widget.ListView ray = (Android.Widget.ListView)xyz;
                abcd.AddChildrenForAccessibility((IList<Android.Views.View>)ray);
            }
            GravityFlags gflags = new GravityFlags();
            // This needs fixing
            supplier_page.ShowAtLocation(abcd, gflags, 0, 0);
#endif
            return true;
        }

#if WPF  || WINUI || SMARTMAUI
        internal static UtilityLoginDetails ConnectionPage { get; set; }
#endif
#if ANDROIDX
        internal static UtilityLoginDetails ConnectionPage { get; set; }
#endif
        internal static async Task<bool> Utility_SuppliersChanged_Actual(
#if WINFORMS
                                                    MainProcess process_components,
#endif
                                                    SignInViewModel signinviewmodel,
                                                    MainViewModel ourviewmodel,
                                                    UtilityViewModel utilityviewmodel,
                                                    UtilityViewModel.SupplierItem supplier_item)
        {
            //string urgent_message = "";

            utilityviewmodel.supplier_code = 0;
            utilityviewmodel.brand_code = 0;

            // We are going to get info? Better turn off some buttons first!
            // These can all be turned off now, except the Cancel button
            TurnOffUtilityStatus(
#if WINFORMS
                                     process_components,
#endif
                                 utilityviewmodel);
            bool status = false;
            if (supplier_item != null)
            {
                string supplier_name = supplier_item.Content;
#if WINFORMS
                supplier_name = supplier_name.TrimStart(SmartParametersV2016.defaultgreen).TrimEnd(SmartParametersV2016.defaultred);
#endif

                Decode_Tag_Supplier(utilityviewmodel, supplier_item.Value); // rf supplier_code, rf brand_code);


                // We are GOOD TO GO!!! 11:00am on 3rd July 2017!!! 
                // This is where we store the data
                // Open the SmartCube file for RAY
                // Create it if it doesn't exist
                try
                {
                    // We are trying to find the connection info for supplier name
                    utilityviewmodel.login_info = new SmartUtility.Logins();
                    List<SmartUtility.Logins> logins_found = SmartSpikeUtilityV2017.Utility_Lookup_Logins(ourviewmodel,
                                                                                    utilityviewmodel,   // Created latest
                                                                                    utilityviewmodel.supplier_code,
                                                                                    utilityviewmodel.brand_code);

                    if (logins_found.Count == 0)
                    {
                        utilityviewmodel.login_info.USERNAME = ourviewmodel.UserName;
                        utilityviewmodel.login_info.CUBEFACE_CODE = SmartParametersV2016.Utility;
                        utilityviewmodel.login_info.SUPPLIER_CODE = utilityviewmodel.supplier_code;
                        utilityviewmodel.login_info.BRAND_CODE = utilityviewmodel.brand_code;
                        //utilityviewmodel.login_info.LOGIN_CREATED = SmartParametersV2016.defaultDate;
                        // Note: no RANDOMKEY !!
                    }
                    else
                    {
                        utilityviewmodel.login_info = logins_found.First();
                        //utilityviewmodel.login_info.USERNAME = logins_found.First().USERNAME;
                        //utilityviewmodel.login_info.CUBEFACE_CODE = logins_found.First().CUBEFACE_CODE;
                        //utilityviewmodel.login_info.SUPPLIER_CODE = logins_found.First().SUPPLIER_CODE;
                        //utilityviewmodel.login_info.BRAND_CODE = logins_found.First().BRAND_CODE;
                        //utilityviewmodel.login_info.LOGIN_CREATED = logins_found.First().LOGIN_CREATED;
                        //utilityviewmodel.login_info.USER_ID = logins_found.First().USER_ID;
                        //utilityviewmodel.login_info.USER_PASSWORD = logins_found.First().USER_PASSWORD;
                        // Note: no RANDOMKEY !!
                    }

                    // Previous FindUtilityConnection COULD have returned false
                    // If it returned TRUE then prompt for the Username password
                    // If it returned FALSE then there is no data and no need for a Username password prompt

                    // Pop-up the Utility Login Details window ... whether the info
                    // was found - or not.  If it wasn't found you need to collect it
                    // if it was found, you will be interested in changing it.

                    List<SmartUtility.BrandConnection> params_found = SmartSpikeUtilityV2017.Utility_FindBrandConnections(ourviewmodel,
                                                                                                                    utilityviewmodel,
                                                                                                                    utilityviewmodel.login_info.SUPPLIER_CODE,
                                                                                                                    utilityviewmodel.login_info.BRAND_CODE);
                    if (logins_found.Count == 0)
                    {
                        utilityviewmodel.found_it = false;
                    }
                    else
                    {
                        utilityviewmodel.found_it = true;
                    }//= logins_found.Count == 0 ? false : true;
#if WINFORMS
                    utilityviewmodel.ConnectionPage = new UtilityLoginDetails(
                                                                    params_found,
                                                                    utilityviewmodel,
                                                                    supplier_name,
                                                                    signinviewmodel.PasswordHash);//ourviewmodel.PassWord);
                    utilityviewmodel.ConnectionPage.Text = supplier_name;

                    DialogResult dialogresult = utilityviewmodel.ConnectionPage.ShowDialog();
                    if (dialogresult == DialogResult.Yes)
                    {
                        string errorMessage;
                        await Utility_LoginSetup(
#if WINFORMS
                                                    process_components,
#endif
                                                    ourviewmodel,
                                                    utilityviewmodel,
                                                    // One day we will need this to show we are the same person who started the porgram
                                                    //ourviewmodel.PassWord, 
                                                    (logins_found.Count == 0 ? false : true),
                                                    utilityviewmodel.LoginDeleteClicked,
                                                    em => errorMessage = em);
                    }
                    else
                    {
                        if (dialogresult == DialogResult.Cancel)
                        {
                            // Do nothing
                        }
                    }
                    // Popup is already closed here
                    utilityviewmodel.ConnectionPage.Dispose();
#endif




#if WPF  || WINUI || SMARTMAUI
                    //  THIS NEEDS TESTING AGAIN!!!
                    ConnectionPage = new UtilityLoginDetails(params_found,
                                                                    utilityviewmodel,
                                                                    supplier_name,
                                                                    supplier_name);
#endif
#if ANDROIDX
                    //  THIS NEEDS TESTING AGAIN!!!
                    ConnectionPage = new UtilityLoginDetails(params_found,
                                                                    utilityviewmodel,
                                                                    supplier_name,
                                                                    supplier_name);
#endif
#if WPF
                    ConnectionPage.IsOpen = true;
#endif
#if ANDROIDX
                    // This needs fixing
                    //utilityviewmodel.ConnectionPage.ShowAtLocation();
#endif
#if SMARTMAUI
                    //await Shell.Current.ShowPopupAsync(ConnectionPage);
#endif
                    status = true;
                }
                catch (Exception exception)
                {
                    if (!await SmartRoutinesV2018.CheckTrace(ourviewmodel, utilityviewmodel.utilityToken, utilityviewmodel.login_info.SUPPLIER_CODE, utilityviewmodel.login_info.BRAND_CODE, SmartParametersV2016.Utility.ToString() + " " + "Login details failed from " + supplier_name + " " + exception.Message))
                    {
                        return false;
                    }
                }
            }
            // These can all be turned off now, except the Cancel button
            TurnOnUtilityStatus(
#if WINFORMS
                                                    process_components,
#endif
                                                utilityviewmodel);
            return status;
        }

        internal static void Decode_Tag_Supplier(UtilityViewModel utilityviewmodel,
                                                    string tag)
        {
            string[] Bouncer = tag.Split(SmartParametersV2016.fieldSeparator);
            if (Bouncer.Length > 1)
            {
                utilityviewmodel.supplier_code = Convert.ToInt16(Bouncer[0].ToString());
                utilityviewmodel.brand_code = Convert.ToInt16(Bouncer[1].ToString());
            }
            return;
        }

        internal static string Build_Tag_Supplier(short supplier_code, short brand_code)
        {
            return supplier_code.ToString() + SmartParametersV2016.fieldSeparator +
                    brand_code.ToString() + SmartParametersV2016.fieldSeparator;
        }

#if ANDROIDX
        internal class UtilityCulturesAdapter : BaseAdapter<string>
        {
            private Activity activity;
            private List<MainViewModel.CultureItem> cultures;
            public UtilityCulturesAdapter(Activity myactivity, List<MainViewModel.CultureItem> cultures)
            {
                this.activity = myactivity;
                this.cultures = cultures;
            }
            public override string this[int position] => cultures[position].Content;
            public override int Count => cultures.Count;
            public override Java.Lang.Object GetItem(int position)
            {
                //.Lang.Object abc = institutions[position].ToString() as Java.Lang.Object;
                return null;
            }
            public override long GetItemId(int position)
            {
                return position;
            }
            public override View GetView(int position, View convertView, ViewGroup parent)
            {
                MainViewModel.CultureItem item = cultures[position];
                View view = activity.LayoutInflater.Inflate(Resource.Layout.SpinnerLayout, null);
                //ImageView icon = view.FindViewById<ImageView>(Resource.Id.imageView);
                TextView textView = view.FindViewById<TextView>(Resource.Id.textView);
                textView.SetTextColor(item.Colour);
                textView.Text = item.Content;
                // Oh and make sure the FUCKING IMAGE exists, of course ...
                //icon.SetImageResource(SetImageId(item.Source));
                return view;
            }

            internal static int SetImageId(string source)
            {
                // Make sure all the Images are 'AndroidResource' !!!!
                return (int)typeof(Resource.Drawable).GetField(source).GetValue(null);
            }
        }
#endif
    }
}