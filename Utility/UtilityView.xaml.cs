#if WINFORMS
using SmartDashboard;
using System.Windows.Forms;
using System.Threading.Tasks;
#endif

#if WPF
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
#endif



#if WINUI
using CommunityToolkit.WinUI.UI.Controls;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using System;
using System.Reflection.Emit;
using System.Threading;
using static SmartCubeMobile.SmartUtility;
#endif

#if ANDROIDX
using Android.Content;
using Android.Views;
using AndroidX.AppCompat.App;
#endif

#if SMARTMAUI
using Microsoft.Maui.Controls;
#endif

namespace SmartCubeMobile
{
#if WINFORMS
    public partial class UtilityView : UserControl
    {
#endif
#if WPF  || WINUI
    public partial class UtilityView : UserControl
    {
#endif
#if ANDROIDX
    public partial class UtilityView : Android.Widget.RelativeLayout
    {
#endif
#if SMARTMAUI
    public partial class UtilityView : ContentView
    {
#endif
        private static SignInViewModel signinviewmodel;
        private MainViewModel ourviewmodel;
        private UtilityViewModel utilityviewmodel;
#if ANDROIDX
        public UtilityView(SignInViewModel signinvm, MainViewModel mainvm, UtilityViewModel utilityvm, Context context) : base(context)
        {
#else
        public UtilityView(SignInViewModel signinvm, MainViewModel mainvm, UtilityViewModel utilityvm)
        {
#endif       
            signinviewmodel = signinvm;
            ourviewmodel = mainvm;
            utilityviewmodel = utilityvm;
#if WPF  || WINUI || SMARTMAUI
            try
            {
                InitializeComponent();
#if WPF 
                // Because you CAN have the DataContext set on the UserControl itself
                DataContext = utilityviewmodel;
#endif
#if SMARTMAUI
                // Because you CAN have the DataContext set on the UserControl itself
                BindingContext = utilityviewmodel;
#endif
#if WINUI
                // Because you can't have the DataContext set on the UserControl itself <= Chimps!
                this.Frame.DataContext = utilityviewmodel;
#endif
                // Previously done in XAML but I couldn't
                // for the LIFE of me find out how I passed
                // extra model parameters to this bollocks ...

                //ButtonAutoSwitchUtility.Click += new RoutedEventHandler((s, e) => SmartUtilityV2022.ButtonAutoSwitchUtilityClick(s, e, ourviewmodel, utilityviewmodel));
#if WPF
                // See the Notes at the start of these
                // Event Handlers which describes
                // what complete and utter garbage this
                // Chimp-derived shite is ...
                UtilityBorder.KeyUp += (s, e) => OnKeyUp(s, e, signinviewmodel, ourviewmodel);

                StartingDate.SelectedDateChanged += new EventHandler<SelectionChangedEventArgs>((s, e) => StartDateChanged(s, e, ourviewmodel, utilityviewmodel));
                ResetDates.Click += new RoutedEventHandler((s, e) => Utility_ResetDates(s, e, ourviewmodel, utilityviewmodel));
                EndingDate.SelectedDateChanged += new EventHandler<SelectionChangedEventArgs>((s, e) => EndDateChanged(s, e, ourviewmodel, utilityviewmodel));

                SuppliersFrame.MouseEnter += new MouseEventHandler((s, e) => MouseEnterSuppliers(s, e, ourviewmodel, utilityviewmodel));
                SuppliersFrame.MouseLeave += new MouseEventHandler((s, e) => MouseLeaveSuppliers(s, e, ourviewmodel, utilityviewmodel));
                SuppliersFrame.MouseDoubleClick += new MouseButtonEventHandler((s, e) => Utility_SupplierMouseDoubleClick(s, e, ourviewmodel, utilityviewmodel));
                SuppliersFrame.MouseRightButtonUp += new MouseButtonEventHandler((s, e) => UtilityMouseRightButtonDown(s, e, ourviewmodel, utilityviewmodel));

                SubmitButton.Click += new RoutedEventHandler((s, e) => Utility_ButtonSubmitClick(s, e, ourviewmodel, utilityviewmodel));
                CancelButton.Click += new RoutedEventHandler((s, e) => Utility_ButtonCancelClick(s, e, ourviewmodel, utilityviewmodel));

                UtilityCostsTab.MouseDoubleClick += new MouseButtonEventHandler((s, e) => UtilityTabMouseDoubleClick(s, e, ourviewmodel, utilityviewmodel));
                UtilityBillsTab.MouseDoubleClick += new MouseButtonEventHandler((s, e) => UtilityTabMouseDoubleClick(s, e, ourviewmodel, utilityviewmodel));
                UtilityReadingsTab.MouseDoubleClick += new MouseButtonEventHandler((s, e) => UtilityTabMouseDoubleClick(s, e, ourviewmodel, utilityviewmodel));
                UtilityBreakdownTab.MouseDoubleClick += new MouseButtonEventHandler((s, e) => UtilityTabMouseDoubleClick(s, e, ourviewmodel, utilityviewmodel));
                UtilityChartsTab.MouseDoubleClick += new MouseButtonEventHandler((s, e) => UtilityTabMouseDoubleClick(s, e, ourviewmodel, utilityviewmodel));

                UtilityCostsGrid.SelectionChanged += new SelectionChangedEventHandler((s, e) => UtilityCostsMouseDoubleClick(s, e, ourviewmodel, utilityviewmodel));

                UtilityCostsChart.MouseDoubleClick += new MouseButtonEventHandler((s, e) => ChartsMouseDoubleClick(s, e, signinviewmodel, ourviewmodel, utilityviewmodel));
                UtilityReadingsChart.MouseDoubleClick += new MouseButtonEventHandler((s, e) => ChartsMouseDoubleClick(s, e, signinviewmodel, ourviewmodel, utilityviewmodel));
                UtilityUsageChart.MouseDoubleClick += new MouseButtonEventHandler((s, e) => ChartsMouseDoubleClick(s, e, signinviewmodel, ourviewmodel, utilityviewmodel));
#endif
#if WINUI
                StartingDate.SelectedDateChanged += (s, e) => StartDateChanged(s, e, ourviewmodel, utilityviewmodel);
                ResetDates.Click += new RoutedEventHandler((s, e) => Utility_ResetDates(s, e, ourviewmodel, utilityviewmodel));
                EndingDate.SelectedDateChanged += (s, e) => EndDateChanged(s, e, ourviewmodel, utilityviewmodel);

                SuppliersFrame.DoubleTapped += new DoubleTappedEventHandler((s, e) => Utility_SupplierMouseDoubleClick(s, e, ourviewmodel, utilityviewmodel));
                SuppliersFrame.RightTapped += new RightTappedEventHandler((s, e) => UtilityMouseRightButtonDown(s, e, ourviewmodel, utilityviewmodel));

                SubmitButton.Click += new RoutedEventHandler((s, e) => Utility_ButtonSubmitClick(s, e, ourviewmodel, utilityviewmodel));
                CancelButton.Click += new RoutedEventHandler((s, e) => Utility_ButtonCancelClick(s, e, ourviewmodel, utilityviewmodel));

                // WINUI does things differently so it appears
                //UtilityCostsTab.DoubleTapped += new DoubleTappedEventHandler((s, e) => UtilityTabMouseDoubleClick(s, e, ourviewmodel, utilityviewmodel));
                //UtilityBillsTab.MouseDoubleClick += new MouseButtonEventHandler((s, e) => UtilityTabMouseDoubleClick(s, e, ourviewmodel, utilityviewmodel));
                //UtilityReadingsTab.MouseDoubleClick += new MouseButtonEventHandler((s, e) => UtilityTabMouseDoubleClick(s, e, ourviewmodel, utilityviewmodel));
                //UtilityBreakdownTab.MouseDoubleClick += new MouseButtonEventHandler((s, e) => UtilityTabMouseDoubleClick(s, e, ourviewmodel, utilityviewmodel));
                //UtilityChartsTab.MouseDoubleClick += new MouseButtonEventHandler((s, e) => UtilityTabMouseDoubleClick(s, e, ourviewmodel, utilityviewmodel));

                UtilityCostsGrid.SelectionChanged += new SelectionChangedEventHandler((s, e) => UtilityCostsMouseDoubleClick(s, e, ourviewmodel, utilityviewmodel));


                // WINUI does things differently so it appears
                //UtilityCostsChart.MouseDoubleClick += new MouseButtonEventHandler((s, e) => ChartsMouseDoubleClick(s, e, ourviewmodel, utilityviewmodel));
                //UtilityReadingsChart.MouseDoubleClick += new MouseButtonEventHandler((s, e) => ChartsMouseDoubleClick(s, e, ourviewmodel, utilityviewmodel));
                //UtilityUsageChart.MouseDoubleClick += new MouseButtonEventHandler((s, e) => ChartsMouseDoubleClick(s, e, ourviewmodel, utilityviewmodel));
#endif
#if SMARTMAUI
                StartingDate.DateSelected += (s, e) => StartingDateChanged(s, e, ourviewmodel, utilityviewmodel);
                ResetDates.Clicked += (s, e) => Utility_ResetDates(s, e, ourviewmodel, utilityviewmodel);
                EndingDate.DateSelected += (s, e) => EndingDateChanged(s, e, ourviewmodel, utilityviewmodel);

                //SuppliersFrame.GestureRecognizers += new DoubleTappedEventHandler((s, e) => Utility_SupplierMouseDoubleClick(s, e, ourviewmodel, utilityviewmodel));
                //SuppliersFrame.RightTapped += (s, e) => UtilityMouseRightButtonDown(s, e, ourviewmodel, utilityviewmodel);

                SubmitButton.Clicked += (s, e) => Utility_ButtonSubmitClick(s, e, ourviewmodel, utilityviewmodel);
                CancelButton.Clicked += (s, e) => Utility_ButtonCancelClick(s, e, ourviewmodel, utilityviewmodel);

                CostsButton.Clicked += (s, e) => SideChartsClicked(s, e, ourviewmodel, utilityviewmodel);
                BillsButton.Clicked += (s, e) => SideChartsClicked(s, e, ourviewmodel, utilityviewmodel);
                ReadingsButton.Clicked += (s, e) => SideChartsClicked(s, e, ourviewmodel, utilityviewmodel);
                BreakdownButton.Clicked += (s, e) => SideChartsClicked(s, e, ourviewmodel, utilityviewmodel);
                ChartsButton.Clicked += (s, e) => SideChartsClicked(s, e, ourviewmodel, utilityviewmodel);

                UtilityCostsGrid.SelectionChanged += (s, e) => UtilityCostsMouseDoubleClick(s, e, ourviewmodel, utilityviewmodel);


                ChartsCostsButton.Clicked += (s, e) => TopChartsClicked(s, e, ourviewmodel, utilityviewmodel);
                ChartsReadingsButton.Clicked += (s, e) => TopChartsClicked(s, e, ourviewmodel, utilityviewmodel);
                ChartsUsageButton.Clicked += (s, e) => TopChartsClicked(s, e, ourviewmodel, utilityviewmodel);
#endif
#if WPF  || WINUI
                Addresses.SelectionChanged += (s, e) => Utility_AddressSelectionChanged(s, e, ourviewmodel, utilityviewmodel);
                CulturePicker.SelectionChanged += (s, e) => UtilityCultures_SelectionChanged(s, e, ourviewmodel, utilityviewmodel);
#endif
#if SMARTMAUI
                Addresses.SelectedIndexChanged += (s, e) => Utility_AddressSelectionChanged(s, e, ourviewmodel, utilityviewmodel);
                CulturePicker.SelectedIndexChanged += (s, e) => UtilityCultures_SelectionChanged(s, e, ourviewmodel, utilityviewmodel);
#endif
#if WPF  || WINUI
                SuppliersFrame.SelectionChanged += (s, e) => UtilitySupplier_SelectionChanged(s, e, ourviewmodel, utilityviewmodel);
                TariffsFrame.SelectionChanged += (s, e) => UtilityTariff_SelectionChanged(s, e, ourviewmodel, utilityviewmodel);
                PaymentPlansFrame.SelectionChanged += (s, e) => UtilityPaymentPlan_SelectionChanged(s, e, ourviewmodel, utilityviewmodel);
#endif
#endif
#if WPF  || WINUI || SMARTMAUI

                // **Haven't thought of a use for these two yet**
                // ** See the FinanceView implementation **
                // See the Notes at the start of these
                // Event Handlers which describes
                // what complete and utter garbage this
                // Chimp-derived shite is ...
#if WPF
                StartingDate.SelectedDateChanged += new EventHandler<SelectionChangedEventArgs>((s, e) => StartDateChanged(s, e, ourviewmodel, utilityviewmodel));
                ResetDates.Click += new RoutedEventHandler((s, e) => Utility_ResetDates(s, e, ourviewmodel, utilityviewmodel));
                EndingDate.SelectedDateChanged += new EventHandler<SelectionChangedEventArgs>((s, e) => EndDateChanged(s, e, ourviewmodel, utilityviewmodel));
#endif
#if WINUI
                StartingDate.SelectedDateChanged += (s, e) => StartDateChanged(s, e, ourviewmodel, utilityviewmodel);
                EndingDate.SelectedDateChanged += (s, e) => EndDateChanged(s, e, ourviewmodel, utilityviewmodel);
#endif
#if SMARTMAUI
                StartingDate.DateSelected += (s, e) => StartingDateChanged(s, e, ourviewmodel, utilityviewmodel);
                ResetDates.Clicked += (s, e) => Utility_ResetDates(s, e, ourviewmodel, utilityviewmodel);
                EndingDate.DateSelected += (s, e) => EndingDateChanged(s, e, ourviewmodel, utilityviewmodel);
#endif
#endif
#if WPF  || WINUI || SMARTMAUI
                //So we share the rates across ALL Views
                FrontEndGUI.SetLabelDataContext(ourviewmodel, GBP);
#if WPF
                GBP.SetBinding(Label.ContentProperty, new Binding()
#endif
#if WINUI
                GBP.SetBinding(TextBlock.TextProperty, new Binding()
#endif
#if WPF  || WINUI
                {
                    Path = new PropertyPath("GBPRate"),
                    Source = ourviewmodel
                });
#endif

#if SMARTMAUI
                GBP.SetBinding(Label.TextProperty, new Binding
                {
                    Path = "GBPRate",
                    Source = ourviewmodel
                });
#endif

                // So we share the rates across ALL Views
                FrontEndGUI.SetLabelDataContext(ourviewmodel, this.EUR);
#if WPF
                EUR.SetBinding(Label.ContentProperty, new Binding()
#endif
#if WINUI
                EUR.SetBinding(TextBlock.TextProperty, new Binding()
#endif
#if WPF  || WINUI
                {
                    Path = new PropertyPath("EURRate"),
                    Source = ourviewmodel
                });
#endif
#if SMARTMAUI
                EUR.SetBinding(Label.TextProperty, new Binding
                {
                    Path = "EURRate",
                    Source = ourviewmodel
                });
#endif

                // So we share the rates across ALL Views
                FrontEndGUI.SetLabelDataContext(ourviewmodel, this.USD);
#if WPF
                USD.SetBinding(Label.ContentProperty, new Binding()
#endif
#if WINUI
                USD.SetBinding(TextBlock.TextProperty, new Binding()
#endif
#if WPF  || WINUI
                {
                    Path = new PropertyPath("USDRate"),
                    Source = ourviewmodel
                });
#endif
#if SMARTMAUI
                USD.SetBinding(Label.TextProperty, new Binding
                {
                    Path = "USDRate",
                    Source = ourviewmodel
                });
#endif

                // So we share the rates across ALL Views
                FrontEndGUI.SetLabelDataContext(ourviewmodel, this.JPY);
#if WPF
                JPY.SetBinding(Label.ContentProperty, new Binding()
#endif
#if WINUI
                JPY.SetBinding(TextBlock.TextProperty, new Binding()
#endif
#if WPF  || WINUI
                {
                    Path = new PropertyPath("JPYRate"),
                    Source = ourviewmodel
                });
#endif
#if SMARTMAUI
                JPY.SetBinding(Label.TextProperty, new Binding
                {
                    Path = "JPYRate",
                    Source = ourviewmodel
                });
#endif
#if WPF
                // utilityviewmodel.tariffs_frame = (Frame)FindByName("TariffsFrame");
                // utilityviewmodel.paymentplans_frame = (Frame)FindByName("PaymentPlansFrame");
#endif
            }
            catch (Exception exception)
            {
                GiveUp(ourviewmodel, utilityviewmodel, exception.Message);
                return;
            }
#endif



                //#if Z!WINFORMS
                //            //// https://stackoverflow.com/questions/1483892/how-to-bind-to-a-passwordbox-in-mvvm
                //            //// Mike McKechnie


                //            if (utilityviewmodel.SupplierSelectedIndex == -1)
                //            {
                //                if (MySupplierPicker != null)
                //                {
                //#if WPF
                //                    MySupplierPicker.Focus();
                //#endif
                //                }
                //            }
                //#endif

#if WPF
            // https://stackoverflow.com/questions/1483892/how-to-bind-to-a-passwordbox-in-mvvm
            // Mike McKechnie
            if (this.SuppliersFrame == null)    // <= Was SelectedItem??? 
            {
                return;
            }
            this.SuppliersFrame.Focus();
#endif
                return;
        }

        

        internal void UtilityCleanup()
        {
#if WPF
            StartingDate.SelectedDateChanged -= new EventHandler<SelectionChangedEventArgs>((s, e) => UtilityView.StartDateChanged(s, e, ourviewmodel, utilityviewmodel));
            ResetDates.Click -= new RoutedEventHandler((s, e) => UtilityView.Utility_ResetDates(s, e, ourviewmodel, utilityviewmodel));
            EndingDate.SelectedDateChanged -= new EventHandler<SelectionChangedEventArgs>((s, e) => UtilityView.EndDateChanged(s, e, ourviewmodel, utilityviewmodel));
#endif
#if WINUI
            StartingDate.SelectedDateChanged -= (s, e) => StartDateChanged(s, e, ourviewmodel, utilityviewmodel);
            ResetDates.Click -= new RoutedEventHandler((s, e) => UtilityView.Utility_ResetDates(s, e, ourviewmodel, utilityviewmodel));
            EndingDate.SelectedDateChanged -= (s, e) => EndDateChanged(s, e, ourviewmodel, utilityviewmodel);
#endif
#if SMARTMAUI
            StartingDate.DateSelected -= (s, e) => StartingDateChanged(s, e, ourviewmodel, utilityviewmodel);
            EndingDate.DateSelected -= (s, e) => EndingDateChanged(s, e, ourviewmodel, utilityviewmodel);
#endif
        }
        internal static async void GiveUp(MainViewModel ourviewmodel, UtilityViewModel utilityviewmodel, string message)
        {
            ourviewmodel.errorMessage = message;
            if (!await SmartRoutinesV2018.CheckTrace(ourviewmodel, utilityviewmodel.utilityToken, 0, 0, SmartParametersV2016.Utility.ToString() + " " + ourviewmodel.errorMessage))
            {
                return;
            }
            FrontEndGUI.SetLedColour(ourviewmodel, 8, ourviewmodel.redColour);
            return;
        }

#if SMARTMAUI
        internal void SideChartsClicked(object sender, EventArgs e,
                                                    MainViewModel ourviewmodel,
                                                    UtilityViewModel utilityviewmodel)
        {
            Button abc = sender as Button;
            string compar = abc.CommandParameter.ToString();
            if (!string.IsNullOrEmpty(compar))
            {
                switch (compar)
                {
                    case "C":
                        utilityviewmodel.SideViewVisible = true;
                        utilityviewmodel.ChartsVisible = false;

                        utilityviewmodel.CostsVisible = true;
                        utilityviewmodel.BillsVisible = false;
                        utilityviewmodel.ReadingsVisible = false;
                        utilityviewmodel.BreakdownVisible = false;
                        break;
                    case "B":
                        utilityviewmodel.SideViewVisible = true;
                        utilityviewmodel.ChartsVisible = false;

                        utilityviewmodel.CostsVisible = false;
                        utilityviewmodel.BillsVisible = true;
                        utilityviewmodel.ReadingsVisible = false;
                        utilityviewmodel.BreakdownVisible = false;
                        break;
                    case "R":
                        utilityviewmodel.SideViewVisible = true;
                        utilityviewmodel.ChartsVisible = false;

                        utilityviewmodel.CostsVisible = false;
                        utilityviewmodel.BillsVisible = false;
                        utilityviewmodel.ReadingsVisible = true;
                        utilityviewmodel.BreakdownVisible = false;
                        break;
                    case "BK":
                        utilityviewmodel.SideViewVisible = true;
                        utilityviewmodel.ChartsVisible = false;

                        utilityviewmodel.CostsVisible = false;
                        utilityviewmodel.BillsVisible = false;
                        utilityviewmodel.ReadingsVisible = false;
                        utilityviewmodel.BreakdownVisible = true;
                        break;
                    case "CH":
                        utilityviewmodel.SideViewVisible = false;
                        utilityviewmodel.ChartsVisible = true;

                        utilityviewmodel.CostsGraphsVisible = true;
                        utilityviewmodel.ReadingsGraphsVisible = false;
                        utilityviewmodel.UsageGraphsVisible = false;
                        break;
                    default:
                        break;

                }
            }
            return;
        }

        internal void TopChartsClicked(object sender, EventArgs e,
                                                    MainViewModel ourviewmodel,
                                                    UtilityViewModel utilityviewmodel)
        {
            Button abc = sender as Button;
            string compar = abc.CommandParameter.ToString();
            if (!string.IsNullOrEmpty(compar))
            {
                switch (compar)
                {
                    case "CC":
                        utilityviewmodel.CostsGraphsVisible = true;
                        utilityviewmodel.ReadingsGraphsVisible = false;
                        utilityviewmodel.UsageGraphsVisible = false;
                        break;
                    case "CR":
                        utilityviewmodel.CostsGraphsVisible = false;
                        utilityviewmodel.ReadingsGraphsVisible = true;
                        utilityviewmodel.UsageGraphsVisible = false;
                        break;
                    case "CU":
                        utilityviewmodel.CostsGraphsVisible = false;
                        utilityviewmodel.ReadingsGraphsVisible = false;
                        utilityviewmodel.UsageGraphsVisible = true;
                        break;
                    default:
                        break;
                }
            }
            return;
        }
#endif

        // For reasons which escape me but the Microshit Chimpanzees will guess at,
        // this event is called even when there is nothing in the drop-down
#if WINFORMS
        internal static void Utility_AddressSelectionChanged(object sender, object e,
                                                        MainProcess process_components,
                                                        MainViewModel ourviewmodel,
                                                        UtilityViewModel utilityviewmodel)
#endif
#if WPF  || WINUI
        internal static void Utility_AddressSelectionChanged(object sender, RoutedEventArgs e,
                                                        MainViewModel ourviewmodel,
                                                        UtilityViewModel utilityviewmodel)
#endif
#if ANDROIDX
        internal static void Utility_AddressSelectionChanged(object sender, object e,
                                                            MainViewModel ourviewmodel,
                                                        UtilityViewModel utilityviewmodel)
#endif
#if SMARTMAUI
        internal static void Utility_AddressSelectionChanged(object sender, object e,
                                                        MainViewModel ourviewmodel,
                                                        UtilityViewModel utilityviewmodel)
#endif
        {
            if (sender != null &&
#if WINFORMS || WPF || SMARTMAUI
                FrontEndGUI.DecodeEventFlags(e) != null)
#endif
#if WINUI
                FrontEndGUI.DecodeSelectionChangedEventFlags(e) != null)
#endif
#if ANDROIDX
                FrontEndGUI.DecodeEventFlags(e) != null)
#endif
            {
                int index = FrontEndGUI.DecodeSenderIndex(sender);
                // I don't believe it... this bollocks is ACTUALLY WORKING
                if (index == utilityviewmodel.AddressSelectedIndex)
                {
                    // This just calls Transactions Subset with respect Dates
                    SmartUtilityV2022.Utility_AddressesChanged_Actual(
#if WINFORMS
                                                                    process_components,
#endif
                                                                    ourviewmodel,
                                                                    utilityviewmodel);
                }
#if WPF
                // Not sure what effect THIS has (stops it bubbling up)
                e.Handled = true;
#endif
            }
            return;
        }


#if WPF || SMARTMAUI
        // I haven't got the time, the energy OR the patience
        // to fuck around trying to defeat these OBVIOUS WPF bugs
        // to deal with when this event handler is called.  It seems
        // to be called EVEN WHEN its being defined right at the start
        // of the Event Handler being defined and it fucks about being
        // called every 5 seconds (of its own accord) thereafter.
        // Its a pile of crap .. but at least it seems to set the start
        // and end dates correctly when I change the Address to cover
        // different ranges.  I KNOW it's full of bugs, peculiarities
        // and Chimp-infested shit ...
#endif
#if WINFORMS
        internal static async void StartDateChanged(object sender, EventArgs e,
                                                MainProcess process_components,
                                                MainViewModel ourviewmodel,
                                                UtilityViewModel utilityviewmodel)
        {
#endif
#if WPF
        internal static async void StartDateChanged(object sender,
                                        SelectionChangedEventArgs e,
                                        MainViewModel ourviewmodel,
                                        UtilityViewModel utilityviewmodel)
        {
#endif
#if WINUI
        internal static async void StartDateChanged(object sender,
                                        DatePickerSelectedValueChangedEventArgs e,
                                        MainViewModel ourviewmodel,
                                        UtilityViewModel utilityviewmodel)
        {
#endif
#if ANDROIDX
        internal static async void StartDateChanged(object sender, EventArgs e,

                                        AppCompatActivity meterActivity,

                                        MainViewModel ourviewmodel,
                                        UtilityViewModel utilityviewmodel)
        {
#endif
#if SMARTMAUI
        internal static async void StartingDateChanged(object sender,
                                        DateChangedEventArgs e,
                                        MainViewModel ourviewmodel,
                                        UtilityViewModel utilityviewmodel)
        {
#endif
            // Sometimes (!!) we come here when we're least expecting it ...
            if (sender != null && e != null)
            {
#if WINFORMS
                DateTimePicker datepicker = (DateTimePicker)sender;
                if (datepicker.Enabled &&
                    datepicker.Value != SmartParametersV2016.defaultDate)
                {
                    // Has the date changed through the keyboard
                    if (SmartTimeV2016.ConvertDateTime(datepicker.Value.ToString()) != utilityviewmodel.StartDate)
                    {
#endif
#if ANDROIDX
                DateTime selectedDate = FrontEndGUI.DecodeDatePicker(sender);
                if (selectedDate != utilityviewmodel.StartDate.DateTime)
                {
                    utilityviewmodel.StartDate.DateTime = selectedDate;
#endif
#if WPF  || WINUI || SMARTMAUI
                DatePicker datepicker = (DatePicker)sender;

#if WPF  || WINUI
                if (datepicker.IsEnabled &&
                    datepicker.SelectedDate != SmartParametersV2016.defaultDate)
                {
#endif
#if SMARTMAUI
                if (datepicker.IsEnabled &&
                    datepicker.Date != SmartParametersV2016.defaultDate)
                {
#endif
                    // Has the date changed through the keyboard
#if WPF
                    if (datepicker.IsMouseOver)
                    {
#endif
#if WINUI
                    // The SelectedDate is TWO-WAY in WINUI in order to
                    // update utilityviewmodel ....
                    if (datepicker.IsEnabled)
                    {
#endif

#endif
#if ANDROIDX
                    if (true)
                    {
#endif
#if SMARTMAUI
                        if (true)
                    {
#endif
                    // Find which tab(s) to adjust - adjust them all?
                    SmartUtilityV2022.TurnOffUtilityStatus(
#if WINFORMS
                                                            process_components,
#endif
                                                            utilityviewmodel);
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
                            // Need return false here
                            Console.Write("here");
                        }
                        SmartUtilityV2022.TurnOnUtilityStatus(
#if WINFORMS
                                                        process_components,
#endif
                                                        utilityviewmodel);

#if WPF
                        // Not sure what effect THIS has - nothing!
                        e.Handled = true;
#endif
                    }
                }
            }
            return;
        }

#if WINFORMS
        internal static async void EndDateChanged(object sender, EventArgs e,
                                            MainProcess process_components,
                                            MainViewModel ourviewmodel,
                                            UtilityViewModel utilityviewmodel)
        {
#endif
#if WPF
        internal static async void EndDateChanged(object sender,
                                        SelectionChangedEventArgs e,
                                        MainViewModel ourviewmodel,
                                        UtilityViewModel utilityviewmodel)
        {
#endif
#if WINUI
        internal static async void EndDateChanged(object sender,
                                        DatePickerSelectedValueChangedEventArgs e,
                                        MainViewModel ourviewmodel,
                                        UtilityViewModel utilityviewmodel)
        {
#endif
#if ANDROIDX
        internal static async void EndDateChanged(object sender, EventArgs e,
                                        AppCompatActivity meterActivity,
                                        MainViewModel ourviewmodel,
                                        UtilityViewModel utilityviewmodel)
        {
#endif
#if SMARTMAUI
        internal static async void EndingDateChanged(object sender,
                                        DateChangedEventArgs e,
                                        MainViewModel ourviewmodel,
                                        UtilityViewModel utilityviewmodel)
        {
#endif

            // Sometimes (!!) we come here when we're least expecting it ...
            if (sender != null)
            {
#if WINFORMS
                DateTimePicker datepicker = (DateTimePicker)sender;
                if (datepicker.Enabled &&
                    datepicker.Value != SmartParametersV2016.defaultMaxdate)
                {
                    // Has the date changed through the keyboard
                    if (SmartTimeV2016.ConvertDateTime(datepicker.Value.ToString()) != utilityviewmodel.EndDate)
                    {
#endif
#if ANDROIDX
                DateTime selectedDate = FrontEndGUI.DecodeDatePicker(sender);
                if (selectedDate != utilityviewmodel.EndDate.DateTime)
                {
                    utilityviewmodel.EndDate.DateTime = selectedDate;
#endif
#if WPF  || WINUI
                DatePicker datepicker = (DatePicker)sender;
                if (datepicker.IsEnabled &&
                    datepicker.SelectedDate != SmartParametersV2016.defaultMaxdate)
                {
                    // Has the date changed through the keyboard
#if WPF
                    if (datepicker.IsMouseOver)
                    {
#endif
#if WINUI
                    // The SelectedDate is TWO-WAY in WINUI in order to
                    // update utilityviewmodel ....
                    if (datepicker.IsEnabled)
                    {
#endif
#endif
#if ANDROIDX
                    if (true)
                    {
#endif
#if SMARTMAUI
                DatePicker datepicker = (DatePicker)sender;
                if (datepicker.IsEnabled &&
                    datepicker.Date != SmartParametersV2016.defaultMaxdate)
                {
                    if (true)
                    {
#endif
                        // Do all tab(s)
                        // Find which tab(s) to adjust - adjust them all?
                        SmartUtilityV2022.TurnOffUtilityStatus(
#if WINFORMS
                                                        process_components,
#endif
                                                        utilityviewmodel);
                        if (!await SmartUtilityV2022.DisplayUtilityMeterAsync(
#if WINFORMS
                                            process_components,
#endif
#if ANDROIDX
                                            meterActivity,
#endif
                                            ourviewmodel, utilityviewmodel,
                                            SmartParametersV2016.activeFlag,
                                            utilityviewmodel.next_connection))
                        {
                            // Need return false here
                            Console.Write("here");
                        }
                        SmartUtilityV2022.TurnOnUtilityStatus(
#if WINFORMS
                                                        process_components,
#endif
                                                        utilityviewmodel);

#if WPF
                        // Not sure what effect THIS has - nothing!
                        e.Handled = true;
#endif
                    }
                }
            }
            return;
        }

#if WINFORMS
        internal static void Utility_ResetDates(object sender, EventArgs e,
                                                MainProcess process_components,
                                                MainViewModel ourviewmodel,
                                                UtilityViewModel utilityviewmodel)
        {
#endif
#if WPF
        internal static void Utility_ResetDates(object sender,
                                        EventArgs e,
                                        MainViewModel ourviewmodel,
                                        UtilityViewModel utilityviewmodel)
        {
#endif
#if WINUI
        internal static void Utility_ResetDates(object sender,
                                        RoutedEventArgs e,
                                        MainViewModel ourviewmodel,
                                        UtilityViewModel utilityviewmodel)
        {
#endif
#if ANDROIDX
        internal static void Utility_ResetDates(object sender, EventArgs e,
                                            AppCompatActivity meterActivity,

                                        MainViewModel ourviewmodel,
                                        UtilityViewModel utilityviewmodel)
        {
#endif
#if SMARTMAUI
        internal static void Utility_ResetDates(object sender,
                                        EventArgs e,
                                        MainViewModel ourviewmodel,
                                        UtilityViewModel utilityviewmodel)
        {
#endif
            // Sometimes (!!) we come here when we're least expecting it ...
            if (sender != null && e != null)
            {
#if WINFORMS
                Button resetdates = (Button)sender;
                if (resetdates.Enabled)
                {
#endif
#if WPF  || WINUI || SMARTMAUI
                Button resetdates = (Button)sender;
                if (resetdates.IsEnabled)
                {
#endif
#if ANDROIDX
                Android.Widget.Button resetdates = (Android.Widget.Button)sender;
                if (resetdates.Enabled)
                {
#endif
                    SmartUtilityV2022.TurnOffUtilityStatus(
#if WINFORMS
                                                        process_components,
#endif
                                                        utilityviewmodel);
                    SmartUtilityV2022.UtilityResetDatesActual(
#if WINFORMS
                                                                    process_components,
#endif
#if ANDROIDX
                                                                    meterActivity,
#endif
                                                                    ourviewmodel,
                                                                    utilityviewmodel);
                    SmartUtilityV2022.TurnOnUtilityStatus(
#if WINFORMS
                                                            process_components,
#endif
                                                            utilityviewmodel);

                }
            }
            return;
        }

#if WINFORMS || WPF  || WINUI || SMARTMAUI
#if WINFORMS
        internal static async void UtilitySupplier_SelectionChanged(object sender, object e,
                                                                    MainViewModel ourviewmodel,
                                                                    UtilityViewModel utilityviewmodel)
#endif
#if WPF  || WINUI || SMARTMAUI
        internal static async void UtilitySupplier_SelectionChanged(object sender, object e, //SelectionChangedEventArgs e)
                                                            MainViewModel ourviewmodel,
                                                            UtilityViewModel utilityviewmodel)
#endif
        {
            if (sender != null &&
                FrontEndGUI.DecodeSelectionChangedEventFlags(e) != null)
            {
                int selectedIndex = FrontEndGUI.DecodeSenderIndex(sender);
                if (selectedIndex != -1)
                {
                    if (utilityviewmodel.SupplierSelectedIndex != selectedIndex)
                    {
                        utilityviewmodel.SupplierSelectedIndex = selectedIndex;
                        // I don't believe it... this bollocks is ACTUALLY WORKING
                        int result = await SmartUtilityV2022.UtilitySuppliersChanged_Actual(ourviewmodel,
                                                                                            utilityviewmodel);
                        if (!string.IsNullOrEmpty(utilityviewmodel.errorMessage))
                        {
                            FrontEndGUI.SetLedColour(ourviewmodel, 8, ourviewmodel.redColour);
                        }
                    }
                }
#if WPF
                // Not sure what effect THIS has
                FrontEndGUI.DecodeSelectionChangedEventFlags(e).Handled = true;
#endif
            }
            return;
        }
#endif

        //internal void UtilitySupplier_SelectionChanged(object sender, EventArgs e)
        //{
        //    // For reasons which entirely elude me ... these MIcroshit chimpanzees have
        //    // decided to fire this event whenever I switch screens/UserControls etc. using
        //    // SwitchView.  Nothing is documented, nothing is explained and I can't trace where
        //    // this event is being fred from - I can only assume its the Microshit monkeys
        //    // and their usual, unrelenting, unremitting bollocks at work ...
        //    // Once I get the Android version of this working, I will dump those cunts in an INSTANT

#if WINFORMS
        internal static async void UtilityTariff_SelectionChanged(object sender, object e,
                                                                    MainViewModel ourviewmodel,
                                                                    UtilityViewModel utilityviewmodel)
#endif
#if WPF  || WINUI
        internal static async void UtilityTariff_SelectionChanged(object sender, object e,
                                                                    MainViewModel ourviewmodel,
                                                                    UtilityViewModel utilityviewmodel)
#endif
#if ANDROIDX
        internal static async void UtilityTariff_SelectionChanged(object sender, object e,
                                                                    MainViewModel ourviewmodel,
                                                                    UtilityViewModel utilityviewmodel)
#endif
#if SMARTMAUI
        internal static async void UtilityTariff_SelectionChanged(object sender, object e,
                                                                    MainViewModel ourviewmodel,
                                                                    UtilityViewModel utilityviewmodel)
#endif
        {
            if (sender != null &&
                FrontEndGUI.DecodeSelectionChangedEventFlags(e) != null)
            {
                int selectedIndex = FrontEndGUI.DecodeSenderIndex(sender);


                if ((utilityviewmodel.SupplierSelectedIndex != -1) &&
                    (selectedIndex != -1))
                {
#if WINFORMS
                    utilityviewmodel.TariffSelectedIndex = selectedIndex;
#endif
                    //I don't believe it... this bollocks is ACTUALLY WORKING
                    int tariffs_index = await SmartUtilityV2022.UtilityTariffsChanged_Actual(ourviewmodel,
                                                                                            utilityviewmodel);
                    if (!string.IsNullOrEmpty(utilityviewmodel.errorMessage))
                    {
                        FrontEndGUI.SetLedColour(ourviewmodel, 8, ourviewmodel.redColour);
                    }
                }

#if WPF
                // Not sure what effect THIS has
                FrontEndGUI.DecodeSelectionChangedEventFlags(e).Handled = true;
#endif
            }
            return;
        }

#if WINFORMS
        internal static async void UtilityPaymentPlan_SelectionChanged(object sender, object e,
                                                                        MainViewModel ourviewmodel,
                                                                        UtilityViewModel utilityviewmodel)
#endif
#if WPF  || WINUI
        internal static async void UtilityPaymentPlan_SelectionChanged(object sender, object e,
                                                                        MainViewModel ourviewmodel,
                                                                        UtilityViewModel utilityviewmodel)
#endif
#if ANDROIDX
        internal static async void UtilityPaymentPlan_SelectionChanged(object sender, object e, 
                                                                        MainViewModel ourviewmodel,
                                                                        UtilityViewModel utilityviewmodel)
#endif
#if SMARTMAUI
        internal static async void UtilityPaymentPlan_SelectionChanged(object sender, object e,
                                                                        MainViewModel ourviewmodel,
                                                                        UtilityViewModel utilityviewmodel)
#endif
        {
            if (sender != null &&
                FrontEndGUI.DecodeSelectionChangedEventFlags(e) != null)
            {
                int selectedIndex = FrontEndGUI.DecodeSenderIndex(sender);

                if ((utilityviewmodel.SupplierSelectedIndex != -1) &&
                    (utilityviewmodel.TariffSelectedIndex != -1) &&
                    (selectedIndex != -1))
                {
#if WINFORMS
                    utilityviewmodel.PaymentPlanSelectedIndex = selectedIndex;
#endif

                    // I don't believe it... this bollocks is ACTUALLY WORKING
                    int paymentplan_index = await SmartUtilityV2022.UtilityPaymentPlansChanged_Actual(ourviewmodel,
                                                                                                        utilityviewmodel);
                    if (!string.IsNullOrEmpty(utilityviewmodel.errorMessage))
                    {
                        FrontEndGUI.SetLedColour(ourviewmodel, 8, ourviewmodel.redColour);
                    }
                }
#if WPF
                // Not sure what effect THIS has
                FrontEndGUI.DecodeSelectionChangedEventFlags(e).Handled = true;
#endif
            }
            return;
        }

#if WINFORMS
        internal static async void Utility_CheckedChanged(object sender, MouseEventArgs e,
                                                            MainProcess components,
                                                            MainViewModel ourviewmodel,
                                                            UtilityViewModel utilityviewmodel)
#endif
#if WPF
        internal async void Utility_CheckedChanged(object sender, RoutedEventArgs e,
                                                            MainViewModel ourviewmodel,
                                                            UtilityViewModel utilityviewmodel)
#endif
#if WINUI
        internal async void Utility_CheckedChanged(object sender, RoutedEventArgs e,
                                                    MainViewModel ourviewmodel,
                                                    UtilityViewModel utilityviewmodel)
#endif
#if ANDROIDX
        internal static async void Utility_CheckedChanged(object sender, EventArgs e,
                                                    AppCompatActivity meterActivity,
                                                    MainViewModel ourviewmodel,
                                                    UtilityViewModel utilityviewmodel)
#endif
#if SMARTMAUI
        internal async void Utility_CheckedChanged(object sender, EventArgs e,
                                                            MainViewModel ourviewmodel,
                                                            UtilityViewModel utilityviewmodel)
#endif
        {
            // Even though CategoriesEnabled is FALSE here
            // (so - in theory - the button is DISABLED ...)
            // it STILL calls this routine!!!  What a pile
            // of shit this stuff is ....
            if (sender != null && e != null)
            {
#if WINFORMS || WPF  || WINUI || SMARTMAUI
                RadioButton radiobutton = sender as RadioButton;
#endif
#if ANDROIDX
                Android.Widget.RadioButton radiobutton = sender as Android.Widget.RadioButton;
#endif
#if WINFORMS
                if (radiobutton.Enabled)
#endif
#if WPF  || WINUI || SMARTMAUI
                if (radiobutton.IsEnabled)
#endif
#if ANDROIDX
                if (radiobutton.Enabled)
#endif
                {

#if WINFORMS
                        // Despite putting AutoClick to false this Chimp-deranged
                        // bollocks STILL raises and Event even when all
                        // events are off.  Its a pile of shite ...
                        if (!(bool)radiobutton.Checked)
                    {
#endif
#if ANDROIDX
                        // Despite putting AutoClick to false this Chimp-deranged
                        // bollocks STILL raises and Event even when all
                        // events are off.  Its a pile of shite ...
                        if (!(bool)radiobutton.Checked)
                        {
#endif
#if WPF || SMARTMAUI
                    // When the Preview Left Button is pressed the button ISN'T checked
                    // Completely different from WINUI ... but what else can you expect from Chimps??
                    if ((bool)radiobutton.IsChecked || !(bool)radiobutton.IsChecked)
                    {
#endif
#if WINUI
                    // When the LeftButton is pressed the button IS checked
                    // Completely different from WPF ... but what else can you expect from Chimps??
                    if ((bool)radiobutton.IsChecked)
                    {
#endif
#if WINFORMS
                        // Don't fuck about with the Events
                        // Otherwise the Chimps generate multiple events
                        //RadioButtonCheckedEvents(components, false); <= Leave this commented OUT!!
                        if (radiobutton.Tag != null)
                        {
                            char classid = Convert.ToChar(radiobutton.Tag.ToString());
#endif
#if WPF  || WINUI
                        if (radiobutton.Tag != null)
                        {
                            char classid = Convert.ToChar(radiobutton.Tag.ToString());
#endif
#if SMARTMAUI
                        if (radiobutton.Value != null)
                        {
                            char classid = Convert.ToChar(radiobutton.Value.ToString());
#endif
#if ANDROIDX
                        // Don't fuck about with the Events
                        // Otherwise the Chimps generate multiple events
                        //RadioButtonCheckedEvents(components, false); <= Leave this commented OUT!!
                        if (radiobutton.Tag != null)
                        {
                            char classid = Convert.ToChar(radiobutton.Tag.ToString());
#endif

                            utilityviewmodel.resource_code = classid;


                            foreach (SmartUtility.Resources resource_row in utilityviewmodel.Hezbollah.utility_resourcesList)
                            {
                                if (resource_row.RESOURCE_CODE == classid)
                                {
                                    switch (resource_row.RESOURCE_CODE)
                                    {
                                        case 'E':
#if WINFORMS
                                            if (!components.URElectricity.Checked)
                                            {
                                                components.URElectricity.Checked = true;
                                                components.URDualFuel.Checked = false;
                                                resource_row.CHECKED = SmartParametersV2016.lastChecked;
                                                resource_row.Updated = true;
                                            }
#endif
#if WPF  || WINUI || SMARTMAUI
                                            if (!(bool)this.URElectricity.IsChecked)
                                            {
                                                this.URElectricity.IsChecked = true;
                                                this.URDualFuel.IsChecked = false;
                                            }
                                            resource_row.CHECKED = SmartParametersV2016.lastChecked;
                                            resource_row.Updated = true;
#endif
#if ANDROIDX
                                            //RadioButton RBElectricity = FindViewById<RadioButton>(Resource.Id.RBElectricity);
                                            //RadioButton RBDualFuel = FindViewById<RadioButton>(Resource.Id.RBDualFuel);
                                            if (!(bool)utilityviewmodel.RBElectricity.Checked)
                                            {
                                                utilityviewmodel.RBElectricity.Checked = true;
                                                utilityviewmodel.RBDualFuel.Checked = false;
                                            }
                                            resource_row.CHECKED = SmartParametersV2016.lastChecked;
                                            resource_row.Updated = true;
#endif
                                            SmartUtilityV2022.FixResourceTypes(utilityviewmodel, resource_row);
                                            utilityviewmodel.resourceCodes = resource_row.RESOURCE_CODE.ToString();
                                            break;
                                        case 'G':
#if WINFORMS
                                            if (!components.URGas.Checked)
                                            {
                                                components.URGas.Checked = true;
                                                components.URDualFuel.Checked = false;
                                                resource_row.CHECKED = SmartParametersV2016.lastChecked;
                                                resource_row.Updated = true;
                                            }
#endif
#if WPF  || WINUI || SMARTMAUI
                                            if (!(bool)this.URGas.IsChecked)
                                            {
                                                this.URGas.IsChecked = true;
                                                this.URDualFuel.IsChecked = false;
                                            }
                                            resource_row.CHECKED = SmartParametersV2016.lastChecked;
                                            resource_row.Updated = true;
#endif
#if ANDROIDX
                                            //RadioButton RBGas = FindViewById<RadioButton>(Resource.Id.RBGas);
                                            //RadioButton RBDualFuel = FindViewById<RadioButton>(Resource.Id.RBDualFuel);
                                            if (!(bool)utilityviewmodel.RBGas.Checked)
                                            {
                                                utilityviewmodel.RBGas.Checked = true;
                                                utilityviewmodel.RBDualFuel.Checked = false;
                                            }
                                            resource_row.CHECKED = SmartParametersV2016.lastChecked;
                                            resource_row.Updated = true;
#endif
                                            SmartUtilityV2022.FixResourceTypes(utilityviewmodel, resource_row);
                                            utilityviewmodel.resourceCodes = resource_row.RESOURCE_CODE.ToString();
                                            break;
                                        default:    // No 'D'!! Ever!
                                            break;
                                    }
                                }
                                else
                                {
                                    switch (resource_row.RESOURCE_CODE)
                                    {
                                        case 'E':
#if WINFORMS
                                            if (components.URElectricity.Checked)
                                            {
                                                components.URElectricity.Checked = false;
                                                resource_row.CHECKED = SmartParametersV2016.unChecked;
                                                resource_row.Updated = true;
                                            }
#endif
#if WPF  || WINUI || SMARTMAUI
                                            if ((bool)this.URElectricity.IsChecked)
                                            {
                                                this.URElectricity.IsChecked = false;
                                                resource_row.CHECKED = SmartParametersV2016.unChecked;
                                                resource_row.Updated = true;
                                            }
#endif
#if ANDROIDX
                                            //RadioButton RBElectricity = FindViewById<RadioButton>(Resource.Id.RBElectricity);
                                            if ((bool)utilityviewmodel.RBElectricity.Checked)
                                            {
                                                utilityviewmodel.RBElectricity.Checked = false;
                                                resource_row.CHECKED = SmartParametersV2016.unChecked;
                                                resource_row.Updated = true;
                                            }
#endif
                                            SmartUtilityV2022.FixResourceTypes(utilityviewmodel, resource_row);
                                            break;
                                        case 'G':
#if WINFORMS
                                            if (components.URGas.Checked)
                                            {
                                                components.URGas.Checked = false;
                                                resource_row.CHECKED = SmartParametersV2016.unChecked;
                                                resource_row.Updated = true;
                                            }
#endif
#if WPF  || WINUI || SMARTMAUI
                                            if ((bool)this.URGas.IsChecked)
                                            {
                                                this.URGas.IsChecked = false;
                                                resource_row.CHECKED = SmartParametersV2016.unChecked;
                                                resource_row.Updated = true;
                                            }
#endif
#if ANDROIDX
                                            //RadioButton RBGas = FindViewById<RadioButton>(Resource.Id.RBGas);
                                            if ((bool)utilityviewmodel.RBGas.Checked)
                                            {
                                                utilityviewmodel.RBGas.Checked = false;
                                                resource_row.CHECKED = SmartParametersV2016.unChecked;
                                                resource_row.Updated = true;
                                            }
#endif
                                            SmartUtilityV2022.FixResourceTypes(utilityviewmodel, resource_row);
                                            break;
                                        default:    // No 'D' ever!
                                            break;
                                    }
                                }
                            }

                            // The 'D' test - 'D' will have turned both
                            // 'E' and 'G' OFF in the code above and
                            // both their CHECKED will = "".  So IF both
                            // are off then turn 'D' ON
                            if (classid == 'D')
                            {
#if WINFORMS
                                if (!components.URElectricity.Checked &&
                                    !components.URGas.Checked)
                                {
                                    components.URDualFuel.Checked = true;
                                    utilityviewmodel.resourceCodes = classid.ToString();
                                }
#endif
#if WPF  || WINUI || SMARTMAUI
                                if (!(bool)this.URElectricity.IsChecked &&
                                    !(bool)this.URGas.IsChecked)
                                {
                                    this.URDualFuel.IsChecked = true;
                                    utilityviewmodel.resourceCodes = classid.ToString();
                                }
#endif
#if ANDROIDX
                                //RadioButton RBElectricity = FindViewById<RadioButton>(Resource.Id.RBElectricity);
                                //RadioButton RBGas = FindViewById<RadioButton>(Resource.Id.RBGas);
                                //RadioButton RBDualFuel = FindViewById<RadioButton>(Resource.Id.RBDualFuel);
                                if (!(bool)utilityviewmodel.RBElectricity.Checked &&
                                    !(bool)utilityviewmodel.RBGas.Checked)
                                {
                                    utilityviewmodel.RBDualFuel.Checked = true;
                                    utilityviewmodel.resourceCodes = classid.ToString();
                                }
#endif
                            }

                            // I'm going to keep this next test in, just in case I ever encounter that situation again
                            // where the fucking event handler decides to call itself TWICE (or more) as the chimps seem 
                            // to insist it has to ..

                            // Despite everything and all the pain
                            // your program is ABSOLUTELY beautiful Ray, and its ALL YOURS!
                            if (!await SmartUtilityV2022.Utility_RadioButtonChecked_Actual(
#if WINFORMS
                                                                                            components,
#endif
#if ANDROIDX
                                                                                            meterActivity,
#endif
                                                                                            ourviewmodel,
                                                                                            utilityviewmodel))
                            {
                                GiveUp(ourviewmodel, utilityviewmodel, "RadioButton switch failed");
                            }
                            else
                            {
#if WINFORMS
                                // Don't turn this on otherwise
                                // the fuckingChimps generate multiple events!!!
                                //RadioButtonCheckedEvents(components, true); <= Leave this commented OUT!!
#endif

                                SmartRoutinesV2018.FocusOpenClose(ourviewmodel);
                            }
                        }
#if WPF
                        // Not sure what effect THIS has
                        // No WINUI
                        //                FrontEndGUI.DecodeRoutedEventFlags(e).Handled = true;
                        e.Handled = true;
#endif

                    }
                }
            }
            return;
        }

#if WINFORMS
        internal static void RadioButtonCheckedEvents(MainProcess comp, bool onoroff)
#endif
#if WPF  || WINUI
        internal static void RadioButtonCheckedEvents(UtilityView comp, 
                                                        MainViewModel ourviewmodel, 
                                                        UtilityViewModel utilityviewmodel, bool onoroff)
#endif
#if ANDROIDX
        internal static void RadioButtonCheckedEvents(View comp, AppCompatActivity meterActivity, MainViewModel ourviewmodel, UtilityViewModel utilityviewmodel, bool onoroff)
#endif
#if SMARTMAUI
        internal static void RadioButtonCheckedEvents(UtilityView comp, 
                                                        MainViewModel ourviewmodel, 
                                                        UtilityViewModel utilityviewmodel, bool onoroff)
#endif
        {
            // https://stackoverflow.com/questions/22813608/wpf-button-mouseleftbuttondown-doesnt-work-at-all
            // To describe why we use PreviewMouseLeftButtonDown and not MouseLeftButtonDown
#if WINFORMS
            if (onoroff)
            {
                // Traps both Left and Right buttons
                comp.URElectricity.MouseDown += new MouseEventHandler((s, e) => MainProcess.radioButtonUtility_CheckedChanged(s, e, comp));
                comp.URGas.MouseDown += new MouseEventHandler((s, e) => MainProcess.radioButtonUtility_CheckedChanged(s, e, comp));
                comp.URDualFuel.MouseDown += new MouseEventHandler((s, e) => MainProcess.radioButtonUtility_CheckedChanged(s, e, comp));

                comp.UtilityRBNON.MouseClick += new MouseEventHandler((s, e) => MainProcess.UtilityRBCheckedChanged(s, e, comp));
                comp.UtilityRBGBP.MouseClick += new MouseEventHandler((s, e) => MainProcess.UtilityRBCheckedChanged(s, e, comp));
                comp.UtilityRBEUR.MouseClick += new MouseEventHandler((s, e) => MainProcess.UtilityRBCheckedChanged(s, e, comp));
                comp.UtilityRBUSD.MouseClick += new MouseEventHandler((s, e) => MainProcess.UtilityRBCheckedChanged(s, e, comp));
                comp.UtilityRBJPY.MouseClick += new MouseEventHandler((s, e) => MainProcess.UtilityRBCheckedChanged(s, e, comp));
            }
            else
            {
                comp.URElectricity.MouseDown -= new MouseEventHandler((s, e) => MainProcess.radioButtonUtility_CheckedChanged(s, e, comp));
                comp.URGas.MouseDown -= new MouseEventHandler((s, e) => MainProcess.radioButtonUtility_CheckedChanged(s, e, comp));
                comp.URDualFuel.MouseDown -= new MouseEventHandler((s, e) => MainProcess.radioButtonUtility_CheckedChanged(s, e, comp));
            }
#endif
#if WPF
            // We only ever come here ONCE so - for WPF
            // its not necessary to keep disabling and
            // then re-enabling the event handlers ..
            if (onoroff)
            {
                comp.URElectricity.Checked += (s, e) => comp.Utility_CheckedChanged(s, e, ourviewmodel, utilityviewmodel);
                comp.URGas.Checked += (s, e) => comp.Utility_CheckedChanged(s, e, ourviewmodel, utilityviewmodel);
                comp.URDualFuel.Checked += (s, e) => comp.Utility_CheckedChanged(s, e, ourviewmodel, utilityviewmodel);

                comp.RBNON.Checked += (s, e) => RadioButtonRatesClicked(s, e, ourviewmodel, utilityviewmodel);
                comp.RBGBP.Checked += (s, e) => RadioButtonRatesClicked(s, e, ourviewmodel, utilityviewmodel);
                comp.RBEUR.Checked += (s, e) => RadioButtonRatesClicked(s, e, ourviewmodel, utilityviewmodel);
                comp.RBUSD.Checked += (s, e) => RadioButtonRatesClicked(s, e, ourviewmodel, utilityviewmodel);
                comp.RBJPY.Checked += (s, e) => RadioButtonRatesClicked(s, e, ourviewmodel, utilityviewmodel);

            }
#endif
#if WINUI
            // We only ever come here ONCE because it seems that the Chimps
            // never reduce the eventhandler count when you remove an eventhandler
            // so what happens is you get an event raised for every time you
            // define the eventhandler irrespective of the times you undefine it
            // Make sense? Of course not. The Chimp fuckers who wrote this bollocks
            // need fucking shooting
            if (onoroff)
            {
                comp.URElectricity.Tapped += new TappedEventHandler((s, e) => comp.Utility_CheckedChanged(s, e, ourviewmodel, utilityviewmodel));
                comp.URGas.Tapped += new TappedEventHandler((s, e) => comp.Utility_CheckedChanged(s, e, ourviewmodel, utilityviewmodel));
                comp.URDualFuel.Tapped += new TappedEventHandler((s, e) => comp.Utility_CheckedChanged(s, e, ourviewmodel, utilityviewmodel));

                comp.RBNON.Click += new RoutedEventHandler((s, e) => UtilityView.RadioButtonRatesClicked(s, e, ourviewmodel, utilityviewmodel));
                comp.RBGBP.Click += new RoutedEventHandler((s, e) => UtilityView.RadioButtonRatesClicked(s, e, ourviewmodel, utilityviewmodel));
                comp.RBEUR.Click += new RoutedEventHandler((s, e) => UtilityView.RadioButtonRatesClicked(s, e, ourviewmodel, utilityviewmodel));
                comp.RBUSD.Click += new RoutedEventHandler((s, e) => UtilityView.RadioButtonRatesClicked(s, e, ourviewmodel, utilityviewmodel));
                comp.RBJPY.Click += new RoutedEventHandler((s, e) => UtilityView.RadioButtonRatesClicked(s, e, ourviewmodel, utilityviewmodel));

            }
            //else
            //{
            //    comp.URElectricity.Tapped -= new TappedEventHandler((s, e) => comp.Utility_CheckedChanged(s, e, ourviewmodel, utilityviewmodel));
            //    comp.URGas.Tapped -= new TappedEventHandler((s, e) => comp.Utility_CheckedChanged(s, e, ourviewmodel, utilityviewmodel));
            //    comp.URDualFuel.Tapped -= new TappedEventHandler((s, e) => comp.Utility_CheckedChanged(s, e, ourviewmodel, utilityviewmodel));
            //}
#endif
#if ANDROIDX
            // We only ever come here ONCE because it seems that the Chimps
            // never reduce the eventhandler count when you remove an eventhandler
            // so what happens is you get an event raised for every time you
            // define the eventhandler irrespective of the times you undefine it
            // Make sense? Of course not. The Chimp fuckers who wrote this bollocks
            // need fucking shooting
            if (onoroff)
            {
                utilityviewmodel.RBElectricity.Click += new EventHandler((s, e) => UtilityView.Utility_CheckedChanged(s, e, meterActivity, ourviewmodel, utilityviewmodel));
                utilityviewmodel.RBGas.Click += new EventHandler((s, e) => UtilityView.Utility_CheckedChanged(s, e, meterActivity, ourviewmodel, utilityviewmodel));
                utilityviewmodel.RBDualFuel.Click += new EventHandler((s, e) => UtilityView.Utility_CheckedChanged(s, e, meterActivity, ourviewmodel, utilityviewmodel));

                utilityviewmodel.UtilityRBNON.Click += new EventHandler((s, e) => UtilityView.RadioButtonRatesClicked(s, e, meterActivity, ourviewmodel, utilityviewmodel));
                utilityviewmodel.UtilityRBGBP.Click += new EventHandler((s, e) => UtilityView.RadioButtonRatesClicked(s, e, meterActivity, ourviewmodel, utilityviewmodel));
                utilityviewmodel.UtilityRBEUR.Click += new EventHandler((s, e) => UtilityView.RadioButtonRatesClicked(s, e, meterActivity, ourviewmodel, utilityviewmodel));
                utilityviewmodel.UtilityRBUSD.Click += new EventHandler((s, e) => UtilityView.RadioButtonRatesClicked(s, e, meterActivity, ourviewmodel, utilityviewmodel));
                utilityviewmodel.UtilityRBJPY.Click += new EventHandler((s, e) => UtilityView.RadioButtonRatesClicked(s, e, meterActivity, ourviewmodel, utilityviewmodel));
            }
#endif
#if SMARTMAUI
            // We only ever come here ONCE so - for WPF
            // its not necessary to keep disabling and
            // then re-enabling the event handlers ..
            if (onoroff)
            {
                comp.URElectricity.CheckedChanged += (s, e) => comp.Utility_CheckedChanged(s, e, ourviewmodel, utilityviewmodel);
                comp.URGas.CheckedChanged += (s, e) => comp.Utility_CheckedChanged(s, e, ourviewmodel, utilityviewmodel);
                comp.URDualFuel.CheckedChanged += (s, e) => comp.Utility_CheckedChanged(s, e, ourviewmodel, utilityviewmodel);

                comp.RBNON.CheckedChanged += (s, e) => RadioButtonRatesClicked(s, e, ourviewmodel, utilityviewmodel);
                comp.RBGBP.CheckedChanged += (s, e) => RadioButtonRatesClicked(s, e, ourviewmodel, utilityviewmodel);
                comp.RBEUR.CheckedChanged += (s, e) => RadioButtonRatesClicked(s, e, ourviewmodel, utilityviewmodel);
                comp.RBUSD.CheckedChanged += (s, e) => RadioButtonRatesClicked(s, e, ourviewmodel, utilityviewmodel);
                comp.RBJPY.CheckedChanged += (s, e) => RadioButtonRatesClicked(s, e, ourviewmodel, utilityviewmodel);

            }
#endif


            return;
        }

        
#if WINFORMS
        internal void ChartsMouseDoubleClick(object sender, object e,
                                            MainViewModel ourviewmodel,
                                            UtilityViewModel utilityviewmodel)
#endif
#if WPF
        internal async void ChartsMouseDoubleClick(object sender, RoutedEventArgs e,
                                            SignInViewModel signinviewmodel,
                                            MainViewModel ourviewmodel,
                                            UtilityViewModel utilityviewmodel)
#endif
#if WINUI
        internal async void ChartsMouseDoubleClick(object sender, object e,
                                            MainViewModel ourviewmodel,
                                            UtilityViewModel utilityviewmodel)
#endif
#if SMARTMAUI
        internal async void ChartsMouseDoubleClick(object sender, object e,
                                            MainViewModel ourviewmodel,
                                            UtilityViewModel utilityviewmodel)
#endif
#if WINFORMS || WPF  || WINUI || SMARTMAUI
        {
            if (sender != null &&
#if WINFORMS || SMARTMAUI
                FrontEndGUI.DecodeEventFlags(e) != null)
#endif
#if WPF  || WINUI
                FrontEndGUI.DecodeRoutedEventFlags(e) != null)
#endif
            {
                if (1 == 1) //e.ClickCount == 1)
                {
                    string[] tabinfo = FrontEndGUI.DecodeTabControlUtility(this, sender);
                    if (tabinfo.Length == 2)
                    {
#if WINFORMS
                        SmartUtilityV2022.ChartsMouseDoubleClick_Actual(ourviewmodel,
                                                                        utilityviewmodel,
                                                                        tabinfo);
#endif
#if WPF || SMARTMAUI
                        await SmartUtilityV2022.ChartsMouseDoubleClick_Actual(signinviewmodel,
                                                                        ourviewmodel,
                                                                        utilityviewmodel,
                                                                        tabinfo);
#endif
#if WINUI
                        await SmartUtilityV2022.ChartsMouseDoubleClick_Actual(signinviewmodel, 
                                                                        ourviewmodel,
                                                                        utilityviewmodel,
                                                                        tabinfo);
#endif
                    }
                }
#if WPF
                // Not sure what effect THIS has (stops it bubbling up)
                e.Handled = true;
#endif
            }
            return;
        }
#endif

#if ANDROIDX
        // When you choose a Tab Label
        internal async void ChartsMouseDoubleClick(object sender, EventArgs e,
                                            AppCompatActivity meterActivity,
                                            SignInViewModel signinviewmodel,
                                            MainViewModel ourviewmodel,
                                            UtilityViewModel utilityviewmodel)
        {
            if (sender != null && FrontEndGUI.DecodeEventFlags(e) != null)
            {
                TextView tablabel = (TextView)sender;
                if (tablabel.Text != "")//.Count() == 2)
                {
                    string[] tabinfo = new string[2] { tablabel.Text, tablabel.Text };
                    await SmartUtilityV2022.ChartsMouseDoubleClick_Actual(
                                                                meterActivity,
                                                                signinviewmodel,
                                                                ourviewmodel,
                                                                utilityviewmodel,
                                                                tabinfo);
                }
            }
            return;
        }
#endif

#if WPF  || WINUI || SMARTMAUI
        internal static void Chart_MouseLeave(object sender, object e,
                                            UtilityViewModel utilityviewmodel)
        {
            if (sender != null &&
                FrontEndGUI.DecodeMouseEventFlags(e) != null)
            {
                // Remove the popup from the screen
#if WPF || SMARTMAUI
                utilityviewmodel.childWindow.Close();
#endif
#if WINUI
                utilityviewmodel.childWindow.IsOpen = false;
#endif
                // The chart is re-instated after in Chart_WindowClosed
#if WPF
                // No WINUI
                FrontEndGUI.DecodeMouseEventFlags(e).Handled = true;
#endif
            }
            return;
        }
#endif

#if WPF || SMARTMAUI
        internal static void Chart_MouseDoubleClick(object sender, object e,
                                            UtilityViewModel utilityviewmodel)
        {

            if (sender != null &&
                FrontEndGUI.DecodeMouseButtonEventFlags(e) != null)
            {
                int count = 0;
#if WPF
                count = FrontEndGUI.DecodeMouseButtonEventFlags(e).ClickCount;
#endif
                // This absolute fucking useless bollocks #1
                switch (count)
                {
                    case 0:
                    case 1:
                        // Remove the popup from the screen
                        utilityviewmodel.childWindow.Close();
                        // The chart is re-instated in Chart_WindowClosed - no its not anymore
                        break;
                    default:
                        break;
                }
#if WPF
                // Not sure what effect THIS has
                FrontEndGUI.DecodeMouseButtonEventFlags(e).Handled = true;
#endif
            }
            return;
        }
#endif

                //#if WPF  || WINUI
                //#if WPF
                //        internal static void Chart_WindowClosed(object sender, MouseEventArgs e)
                //#endif
                //#if WINUI
                //        internal static void Chart_WindowClosed(object sender, object e)
                //#endif
                //        {
                //            
                //            // Put it back where we found it
                //            // Re-instate the Tab grid, because Content is 'null' this should cause a refresh
                //            //utilityviewmodel.tabItem.Content = utilityviewmodel.tempGrid;
                //            return;
                //        }
                //#endif

                

#if ANDROIDX
        //                internal static void Chart_WindowClosed(object sender, EventArgs e)
        //        {
        //            // Put it back where we found it
        //            //utilityviewmodel.targetItem.Content = utilityviewmodel.target_viewbox;
        //            return;
        //        }
#endif

#if WINFORMS
        internal static async Task<bool> Utility_ButtonSubmitClick(object sender, EventArgs e, MainProcess components,
                                            MainViewModel ourviewmodel,
                                            UtilityViewModel utilityviewmodel)
#endif
#if WPF  || WINUI
        internal static async void Utility_ButtonSubmitClick(object sender, RoutedEventArgs e,
                                            MainViewModel ourviewmodel,
                                            UtilityViewModel utilityviewmodel)
#endif
#if ANDROIDX
        internal static async void Utility_ButtonSubmitClick(object sender, object e,
                                            AppCompatActivity meterActivity,
                                            MainViewModel ourviewmodel,
                                            UtilityViewModel utilityviewmodel)
#endif
#if SMARTMAUI
        internal static async void Utility_ButtonSubmitClick(object sender, object e,
                                            MainViewModel ourviewmodel,
                                            UtilityViewModel utilityviewmodel)
#endif
        {
#if WINFORMS
            bool status = true;
#endif
            // Are BOTH of these false? Yes? then carry on
            // Phase1 false means load sequence has finished
            // Phase3 true means Connect not in progress
            if (!ourviewmodel.Phase1 && ourviewmodel.Phase3)
            //if (sender != null && FrontEndGUI.DecodeEventFlags(e) != null)
            {
                // Set Phase 2 to say we are starting a scrape
                // This will block a Phase 3 Connect starting whilst we are in the middle
                FrontEndGUI.SetLedColour(ourviewmodel, 6, ourviewmodel.whiteColour);

                await SmartRoutinesV2018.TextBlockUpdate(
#if ANDROIDX
                                            meterActivity,
#endif
                                            ourviewmodel, SmartParametersV2016.Utility.ToString() + " Submit button - Utility");

                if (!await SmartRoutinesV2018.CheckTrace(ourviewmodel, utilityviewmodel.utilityToken, 0, 0, SmartParametersV2016.Utility.ToString() + " Submit button: Utility"))
                {
#if WINFORMS
                    return false;
#endif
#if WPF  || WINUI || SMARTMAUI

                    return;
#endif
                }

                if (!await SmartUtilityScrapeV2022.UtilityButtonSubmitClickActual(
#if WINFORMS
                                                                components,
#endif
#if ANDROIDX
                                                                meterActivity,
#endif
                                                                ourviewmodel,
                                                                utilityviewmodel,
                                                                ourviewmodel.Blanche.vatRatesList,
                                                                ourviewmodel.Blanche.exchangeRatesList,
                                                                ourviewmodel.Blanche.postcodesList))

                {
#if WINFORMS
                    status = false;
#endif
                    if (!await SmartRoutinesV2018.CheckTrace(ourviewmodel,
                                                            utilityviewmodel.utilityToken,
                                                            0, 0, "Problem 71: Utility Submit failed"))
                    {
#if WINFORMS
                        return false;
#endif
#if WPF  || WINUI || SMARTMAUI

                        return;
#endif
                    }
                }
#if WPF
                // No WINUI
                // Not sure what effect THIS has (stops it bubbling up)
                e.Handled = true;
#endif
            }
            // This will allow any Phase 3 Connects to happen
            FrontEndGUI.SetLedColour(ourviewmodel, 6, ourviewmodel.orangeColour);

#if WINFORMS
            return status;
#endif
        }

#if WINFORMS
        internal static async void Utility_ButtonCancelClick(object sender, EventArgs e,
                                            MainViewModel ourviewmodel,
                                            UtilityViewModel utilityviewmodel)
        {
#endif
#if WPF  || WINUI
        internal static async void Utility_ButtonCancelClick(object sender, RoutedEventArgs e,
                                            MainViewModel ourviewmodel,
                                            UtilityViewModel utilityviewmodel)
    {
#endif
#if SMARTMAUI
        internal static async void Utility_ButtonCancelClick(object sender, EventArgs e,
                                            MainViewModel ourviewmodel,
                                            UtilityViewModel utilityviewmodel)
    {
#endif
#if WINFORMS
            WebBrowser Scraper = new WebBrowser();
#endif
#if WINFORMS || WPF  || WINUI || SMARTMAUI
            // Find the category given the screen
            if ((sender != null) && (FrontEndGUI.DecodeEventFlags(e) != null))
            {
                if (!await SmartRoutinesV2018.CheckTrace(ourviewmodel, utilityviewmodel.utilityToken, 0, 0, "Problem 72: Utility Cancel chosen"))
                {
                    return;
                }
                //TextBlockUpdateUtility(ourviewmodel, "Utility Cancel chosen");
                if (utilityviewmodel.utilityToken != CancellationToken.None)
                {
                    if (utilityviewmodel.utilityCts != null)
                    {
                        utilityviewmodel.utilityCts.Cancel();
                        utilityviewmodel.utilityToken = CancellationToken.None;
                    }
                }
                if (!await SmartUtilityV2022.Utility_ButtonCancelClick_Actual(ourviewmodel,
                                                                                utilityviewmodel))

                {
                    if (!await SmartRoutinesV2018.CheckTrace(ourviewmodel,
                                                            utilityviewmodel.utilityToken,
                                                            0, 0, "Problem 73: Cancel failed"))
                    {
                        return;
                    }
                }
#if WPF
                // Not sure what effect THIS has (stops it bubbling up)
                e.Handled = true;
#endif
            }
            return;
        }
#endif

        internal static void MouseEnterSuppliers(object sender, object e,
                                            MainViewModel ourviewmodel,
                                            UtilityViewModel utilityviewmodel)
        {
#if WINFORMS
            if (sender != null && FrontEndGUI.DecodeEventFlags(e) != null)
#endif
#if WPF  || WINUI || SMARTMAUI
            if (sender != null && FrontEndGUI.DecodeMouseEventFlags(e) != null)
#endif
#if ANDROIDX
            if (sender != null && FrontEndGUI.DecodeEventFlags(e) != null)
#endif
            {
#if WINFORMS
                utilityviewmodel.SuppliersBorderColor = ourviewmodel.cyanColour;
                utilityviewmodel.SuppliersBorderThickness = true;
#endif
#if WPF  || WINUI || SMARTMAUI
                utilityviewmodel.SuppliersBorderColor = ourviewmodel.cyanColour;
                utilityviewmodel.SuppliersBorderThickness = FrontEndGUI.thickness1;// new Thickness(1);
#endif
#if ANDROIDX
                utilityviewmodel.SuppliersBorderColor = ourviewmodel.cyanColour;
                //utilityviewmodel.SuppliersBorderThickness = new Thickness(1);
#endif
#if WPF
                // No WINUI
                // Not sure what effect THIS has
                FrontEndGUI.DecodeMouseEventFlags(e).Handled = true;
#endif
            }
            return;
        }

        internal static void MouseLeaveSuppliers(object sender, object e,
                                            MainViewModel ourviewmodel,
                                            UtilityViewModel utilityviewmodel)
        {
#if WINFORMS
            if (sender != null && FrontEndGUI.DecodeEventFlags(e) != null)
#endif
#if WPF  || WINUI || SMARTMAUI
            if (sender != null && FrontEndGUI.DecodeMouseEventFlags(e) != null)
#endif
#if ANDROIDX
            if (sender != null && FrontEndGUI.DecodeEventFlags(e) != null)
#endif
            {
#if WINFORMS
                utilityviewmodel.SuppliersBorderColor = ourviewmodel.transparentColour;
                utilityviewmodel.SuppliersBorderThickness = false;
#endif
#if WPF  || WINUI
                utilityviewmodel.SuppliersBorderColor = ourviewmodel.transparentColour;
                utilityviewmodel.SuppliersBorderThickness = FrontEndGUI.thickness0;
#endif
#if ANDROIDX
                utilityviewmodel.SuppliersBorderColor = ourviewmodel.transparentColour;
                //utilityviewmodel.SuppliersBorderThickness = new Thickness(0);
#endif
#if SMARTMAUI
                utilityviewmodel.SuppliersBorderColor = ourviewmodel.transparentColour;
                utilityviewmodel.SuppliersBorderThickness = FrontEndGUI.thickness0;
#endif


                // https://stackoverflow.com/questions/27492401/how-to-change-the-stroke-width-of-a-shape-programmatically-in-android
#if WPF
                // No WINUI
                // Not sure what effect THIS has
                FrontEndGUI.DecodeMouseEventFlags(e).Handled = true;
#endif
            }
            return;
        }


#if ANDROIDX
        // When you choose a Main Tab Label
        internal void UtilityTabMouseSingleClick(object sender, object e,
                                            MainViewModel ourviewmodel,
                                            UtilityViewModel utilityviewmodel)
        {
            if (sender != null && FrontEndGUI.DecodeEventFlags(e) != null)
            {
                TextView tablabel = (TextView)sender;
                if (tablabel != null)
                {
                    TextView main_tabview = null;// FindViewById<Android.Widget.TabHost>(Resource.Id.tabhost);
                    if (main_tabview != null)
                    {
                        switch (tablabel.Text.ToString())
                        {
                            case "Costs":
                                utilityviewmodel.main_tabview_index = 0;
                                break;
                            case "Bills":
                                utilityviewmodel.main_tabview_index = 1;
                                break;
                            case "Readings":
                                utilityviewmodel.main_tabview_index = 2;
                                break;
                            case "Breakdown":
                                utilityviewmodel.main_tabview_index = 3;
                                break;
                            case "Charts":
                                utilityviewmodel.main_tabview_index = 4;
                                break;
                            default:
                                break;
                        }
                    }
                }
            }
            return;
        }
#endif

#if ANDROIDX
        // When you choose a Charts Tab Label
        internal void UtilityChartsMouseSingleClick(object sender, object e,
                                            MainViewModel ourviewmodel,
                                            UtilityViewModel utilityviewmodel)
        {
            if (sender != null && FrontEndGUI.DecodeEventFlags(e) != null)
            {
                TextView tablabel = (TextView)sender;
                if (tablabel != null)
                {
                    TextView charts_tabview = null;// FindViewById(Resource.Id.ChartsTabView) as TabHost;
                    if (charts_tabview != null)
                    {
                        switch (tablabel.Text.ToString())
                        {
                            case "Future Costs/Time":
                                utilityviewmodel.charts_tabview_index = 0;
                                break;
                            case "Readings/Time":
                                utilityviewmodel.charts_tabview_index = 1;
                                break;
                            case "Usage/Time":
                                utilityviewmodel.charts_tabview_index = 2;
                                break;
                            default:
                                break;
                        }
                    }
                }
            }
            return;
        }
#endif


        //#if not WINUI // Nothing for WINUI!! It does it 'differently'!!
#if WINFORMS
        // When you choose a Tab Label (Completely untested!!)
        internal static void UtilityTabMouseDoubleClick(object sender, EventArgs e,
                                                        int width,
                                                        int height,
                                                        DataGridView UtilityCostsGrid,
                                                        MainViewModel ourviewmodel,
                                                        UtilityViewModel utilityviewmodel)
        {
#endif
        // When you choose a Tab Label
#if WPF  || WINUI
        internal void UtilityTabMouseDoubleClick(object sender, object e,
                                            MainViewModel ourviewmodel,
                                            UtilityViewModel utilityviewmodel)
        {
#endif
#if ANDROIDX
        internal void UtilityTabMouseDoubleClick(object sender, object e,
                                            MainViewModel ourviewmodel,
                                            UtilityViewModel utilityviewmodel)
        {
#endif
#if SMARTMAUI
        internal void UtilityTabMouseDoubleClick(object sender, object e,
                                            MainViewModel ourviewmodel,
                                            UtilityViewModel utilityviewmodel)
        {
#endif

#if WINFORMS
            if (sender != null && FrontEndGUI.DecodeEventFlags(e) != null)
#endif
#if WPF     // Uh?? with  the one below as well??
            if (sender != null && FrontEndGUI.DecodeMouseButtonEventFlags(e) != null)
#endif
#if WPF  || WINUI || SMARTMAUI
            if (sender != null && FrontEndGUI.DecodeEventFlags(e) != null)
#endif
#if ANDROIDX
            if (sender != null && FrontEndGUI.DecodeEventFlags(e) != null)
#endif
                {
                    string tabLabel = FrontEndGUI.DecodeTabLabel(sender);

                    if (!string.IsNullOrEmpty(tabLabel))
                    {
#if WINFORMS
                    //List<DataGridViewColumn> popup_columns = new List<DataGridViewColumn>();
#endif
#if WPF  || WINUI || SMARTMAUI
                        var popupColumns = FrontEndGUI.SetPopupColumns();
                        //List<DataGridTextColumn> popup_columns = new List<DataGridTextColumn>();
#endif
#if ANDROIDX
                    //ListView popup_columns = new ListView();
#endif
                        bool carryon = false;
                        switch (tabLabel)
                        {
                            case "Costs":
                                if (utilityviewmodel.UtilityCosts.Count > 0)
                                {
                                    carryon = true;
                                }
#if WPF
                                popupColumns = SmartRoutinesV2018.CopyColumns(this.UtilityCostsGrid);
#endif
#if SMARTMAUI
                                List<SmartRoutinesV2018.ColumnDefinitionModel> costs =
                                SmartRoutinesV2018.GetColumnsFromCollectionView(this.UtilityCostsGrid);
                                popupColumns = SmartRoutinesV2018.CopyColumns(costs);
#endif
                                break;
                            case "Bills":
                                if (utilityviewmodel.UtilityBills.Count > 0)
                                {
                                    carryon = true;
                                }
#if WPF
                                popupColumns = SmartRoutinesV2018.CopyColumns(this.UtilityBillsGrid);
#endif
#if SMARTMAUI
                                List<SmartRoutinesV2018.ColumnDefinitionModel> bills =
                                SmartRoutinesV2018.GetColumnsFromCollectionView(this.UtilityBillsGrid);
                                popupColumns = SmartRoutinesV2018.CopyColumns(bills);
#endif
                                break;
                            case "Readings":
                                if (utilityviewmodel.UtilityReadings.Count > 0)
                                {
                                    carryon = true;
                                }
#if WPF
                                popupColumns = SmartRoutinesV2018.CopyColumns(this.UtilityReadingsGrid);
#endif
#if SMARTMAUI
                                List<SmartRoutinesV2018.ColumnDefinitionModel> readings =
                                SmartRoutinesV2018.GetColumnsFromCollectionView(this.UtilityReadingsGrid);
                                popupColumns = SmartRoutinesV2018.CopyColumns(readings);
#endif
                                break;
                            case "Breakdown":
                                if (utilityviewmodel.UtilityBreakdown.Count > 0)
                                {
                                    carryon = true;
                                }
#if WPF
                                popupColumns = SmartRoutinesV2018.CopyColumns(this.UtilityBreakdownGrid);
#endif
#if SMARTMAUI
                                List<SmartRoutinesV2018.ColumnDefinitionModel> breakdown =
                                SmartRoutinesV2018.GetColumnsFromCollectionView(this.UtilityBreakdownGrid);
                                popupColumns = SmartRoutinesV2018.CopyColumns(breakdown);
#endif
                                break;
                            default:
                                break;
                        }

                        if (carryon)
                        {
#if WINFORMS
                        SmartUtilityV2022.Utility_TabMouseDoubleClick_Actual(tabLabel,
                                                                ourviewmodel,
                                                                utilityviewmodel);
                        //width,
                        //height);
                        //popup_columns);
#endif
#if WPF  || WINUI || SMARTMAUI
                        SmartUtilityV2022.Utility_TabMouseDoubleClick_Actual(tabLabel,
                                                                    ourviewmodel,
                                                                    utilityviewmodel,
                                                                    popupColumns);
#endif
#if ANDROIDX
                        SmartUtilityV2022.Utility_TabMouseDoubleClick_Actual(//(ContentPage)this.Parent,
                                                                tabLabel,
                                                                ourviewmodel,
                                                                utilityviewmodel
                                                                 //#if WPF
                                                                 //                                                                , to_grid
                                                                 //#endif
                                                                 );
#endif
                        }
                    }
#if WPF
                    // No WINUI
                    // Not sure what effect THIS has
                    FrontEndGUI.DecodeMouseButtonEventFlags(e).Handled = true;
#endif
                }
            return;
        }

        
#if WINFORMS
        // Its ok to use async void on Event Handlers
        internal void UtilityCostsMouseDoubleClick(object sender, EventArgs e,
                                            MainViewModel ourviewmodel,
                                            UtilityViewModel utilityviewmodel)
        {
#endif
#if WPF
            // Its ok to use async void on Event Handlers
        internal void UtilityCostsMouseDoubleClick(object sender, RoutedEventArgs e,
                                            MainViewModel ourviewmodel,
                                            UtilityViewModel utilityviewmodel)
        {
#endif
#if WINUI
        // Its ok to use async void on Event Handlers
        internal void UtilityCostsMouseDoubleClick(object sender, RoutedEventArgs e,
                                            MainViewModel ourviewmodel,
                                            UtilityViewModel utilityviewmodel)
        {
#endif
#if ANDROIDX
            // Its ok to use async void on Event Handlers
            internal void UtilityCostsMouseDoubleClick(object sender, EventArgs e,
                                                MainViewModel ourviewmodel,
                                                UtilityViewModel utilityviewmodel)
            {
#endif
#if SMARTMAUI
        // Its ok to use async void on Event Handlers
        internal void UtilityCostsMouseDoubleClick(object sender, EventArgs e,
                                            MainViewModel ourviewmodel,
                                            UtilityViewModel utilityviewmodel)
        {
#endif
#if WINFORMS
            DataGridView viewList = FrontEndGUI.DecodeDataGrid(sender);
#endif
#if WPF
            DataGrid viewList = FrontEndGUI.DecodeDataGrid(sender);
#endif
#if WINUI
            DataGrid viewList = FrontEndGUI.DecodeDataGrid(sender);
#endif
#if ANDROIDX
            GridView viewList = FrontEndGUI.DecodeDataGrid(sender);
#endif
#if SMARTMAUI
            CollectionView viewList = FrontEndGUI.DecodeDataGrid(sender);
#endif
            if (viewList != null)
            {
#if WINFORMS || WPF  || WINUI || SMARTMAUI
                // Find the category given the screen
                SmartUtilityV2022.UtilityCostsMouseDoubleClick_Actual(viewList,
                                                    ourviewmodel,
                                                    utilityviewmodel,
                                                    this,
                                                    ourviewmodel.Blanche.vatRatesList,
                                                    ourviewmodel.Blanche.exchangeRatesList);
#if WPF
                // Not sure what effect THIS has (stops it bubbling up)
                e.Handled = true;
#endif
#endif
            }
            return;
        }

#if ANDROIDX
        internal void OnSupplierCellTapped(object sender, EventArgs e,
                                            MainViewModel ourviewmodel,
                                            UtilityViewModel utilityviewmodel)
        {
            //BuildMouseClick(sender, (PopupWindow)this.Parent, this, 0, ourviewmodel, utilityviewmodel);
            //Label label_cell = (Label)sender;
            //Grid gridrow = (Grid)label_cell.Parent;
            //var abc = gridrow.Children;
            //SmartUtility.AnalysisCostsView viewlist = new SmartUtility.AnalysisCostsView();
            //int count = 0;
            //foreach (Label lab in abc)
            //{
            //    switch (count)
            //    {
            //        case 0:
            //            viewlist.SUPPLIER_CODE = Convert.ToInt16(lab.Text);
            //            break;
            //        case 1:
            //            viewlist.BRAND_CODE = Convert.ToInt16(lab.Text);
            //            break;
            //        case 2:
            //            viewlist.SUPPLIER_NAME = lab.Text;
            //            break;
            //        case 3:
            //            viewlist.TARIFF_CODE = Convert.ToInt32(lab.Text);
            //            break;
            //        case 4:
            //            viewlist.TARIFF_NAME = lab.Text;
            //            break;
            //        case 5:
            //            viewlist.RESOURCE_TYPE = lab.Text;
            //            viewlist.TCR = 0.0M;
            //            break;
            //        case 6:
            //            viewlist.METER_TYPE = lab.Text;
            //            break;
            //        case 7:
            //            viewlist.PAYMENT_NAME = lab.Text;
            //            break;
            //        case 8:
            //            viewlist.TOTAL_AMOUNT = lab.Text;
            //            break;
            //    }
            //    count++;
            //}

            //Grid gridlist = (Grid)label_cell.Parent.Parent.Parent.Parent.LogicalChildren[0];
            //Label header = (Label)gridlist.Children[0];

            //// Find the category given the screen
            //SmartUtilityV2022.UtilityCostsMouseDoubleClick_Actual(viewlist,
            //                                    ourviewmodel,
            //                                    utilityviewmodel,
            //                                    this,
            //                                    header.Text,
            //                                    ourviewmodel.Blanche.vatRatesList,
            //                                    ourviewmodel.Blanche.exchangeRatesList);
            return;
        }

        internal void OnTariffCellTapped(object sender, EventArgs e)
        {
            //BuildMouseClick(sender, (PopupWindow)this.Parent, this, 0, ourviewmodel, utilityviewmodel);
            //Label label_cell = (Label)sender;
            //Grid gridrow = (Grid)label_cell.Parent;
            //var abc = gridrow.Children;
            //SmartUtility.AnalysisCostsView viewlist = new SmartUtility.AnalysisCostsView();
            //int count = 0;
            //foreach (Label lab in abc)
            //{
            //    switch (count)
            //    {
            //        case 0:
            //            viewlist.SUPPLIER_CODE = Convert.ToInt16(lab.Text);
            //            break;
            //        case 1:
            //            viewlist.BRAND_CODE = Convert.ToInt16(lab.Text);
            //            break;
            //        case 2:
            //            viewlist.SUPPLIER_NAME = lab.Text;
            //            break;
            //        case 3:
            //            viewlist.TARIFF_CODE = Convert.ToInt32(lab.Text);
            //            break;
            //        case 4:
            //            viewlist.TARIFF_NAME = lab.Text;
            //            break;
            //        case 5:
            //            viewlist.RESOURCE_TYPE = lab.Text;
            //            viewlist.TCR = 0.0M;
            //            break;
            //        case 6:
            //            viewlist.METER_TYPE = lab.Text;
            //            break;
            //        case 7:
            //            viewlist.PAYMENT_NAME = lab.Text;
            //            break;
            //        case 8:
            //            viewlist.TOTAL_AMOUNT = lab.Text;
            //            break;
            //    }
            //    count++;
            //}
            
            //Grid gridlist = (Grid)label_cell.Parent.Parent.Parent.Parent.LogicalChildren[0];
            //Label header = (Label)gridlist.Children[1];
            
            //// Find the category given the screen
            //SmartUtilityV2022.UtilityCostsMouseDoubleClick_Actual(viewlist,
            //                                 ourviewmodel,
            //                                    utilityviewmodel,
            //                                    this,
            //                                    header.Text,                                                
            //                                    ourviewmodel.Blanche.vatRatesList,
            //                                    ourviewmodel.Blanche.exchangeRatesList);
            return;
        }
#endif

#if WINUI
        private void UtilityCostsGrid_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Tapped += Row_Tapped;
        }

        private void Row_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (sender is FrameworkElement fe &&
                fe.DataContext is AnalysisCostsView item)
            {
                BuildMouseClick(item, ourviewmodel, utilityviewmodel);
            }
        }
#endif
        internal static void BuildMouseClick(
            SmartUtility.AnalysisCostsView item,
            MainViewModel ourviewmodel,
            UtilityViewModel utilityviewmodel)
                {
                    // You already have everything — no parsing needed
                    int supplierCode = item.SUPPLIER_CODE;
                    string supplierName = item.SUPPLIER_NAME;

                    // Do your logic here
        }
//#if WINFORMS
//        internal static void BuildMouseClick(object sender, Form contentpage,
//                                            UtilityView components,
//                                            int column_index,
//                                            MainViewModel ourviewmodel,
//                                            UtilityViewModel utilityviewmodel)
//#endif
//#if WPF  || WINUI
//        internal static void BuildMouseClick(object sender, Window contentpage, 
//                                            UtilityView components, 
//                                            int column_index,
//                                            MainViewModel ourviewmodel,
//                                            UtilityViewModel utilityviewmodel)
//#endif
//#if ANDROIDX
//        internal static void BuildMouseClick(object sender, PopupWindow contentpage, 
//                                            UtilityView components, 
//                                            int column_index,
//                                            MainViewModel ourviewmodel,
//                                            UtilityViewModel utilityviewmodel)
//#endif
//#if SMARTMAUI
//        internal static void BuildMouseClick(object sender, ContentPage contentpage, UtilityView components, int column_index)
//#endif
//        {
//#if WINFORMS || WPF  || WINUI || SMARTMAUI
//#if WPF || SMARTMAUI
//            Label label_cell = (Label)sender;
//            //Grid gridrow = (Grid)label_cell.Parent;
//            //var abc = gridrow.Children;
//#endif
//#if WINUI
//            Label abc = (Label)sender;
//#endif
//#if WINFORMS
//            Label[] abc = new Label[1];      
//#endif
//#endif
//#if ANDROIDX
//            TextView label_cell = (TextView)sender;
//            GridView gridrow = (GridView)label_cell.Parent;
//            //TextView abc = gridrow.GetChildAt(0) as TextView;

//            // This needs re-doing!!!!!!
//            List<TextView> abc = new List<TextView>();

//#endif
//            SmartUtility.AnalysisCostsView viewlist = new SmartUtility.AnalysisCostsView();
//            int count = 0;
//#if WINFORMS || WPF  || WINUI || SMARTMAUI
//            foreach (Label lab in abc)
//#endif
//#if ANDROIDX
//            foreach (TextView lab in abc)
//#endif
//            {

//                switch (count)
//                {
//                    case 0:
//#if WINFORMS
//                        viewlist.SUPPLIER_CODE = Convert.ToInt16(lab.Text);
//#endif
//#if WPF
//                        viewlist.SUPPLIER_CODE = Convert.ToInt16(lab.Content.ToString());
//#endif

//                        break;
//                    case 1:
//#if WINFORMS
//                        viewlist.BRAND_CODE = Convert.ToInt16(lab.Text);
//#endif
//#if WPF
//                        viewlist.BRAND_CODE = Convert.ToInt16(lab.Content.ToString());
//#endif
//                        break;
//                    case 2:
//#if WINFORMS
//                        viewlist.SUPPLIER_NAME = lab.Text;
//#endif
//#if WPF
//                        viewlist.SUPPLIER_NAME = lab.Content.ToString();
//#endif
//                        break;
//                    case 3:
//#if WINFORMS
//                        viewlist.TARIFF_CODE = Convert.ToInt32(lab.Text);
//#endif
//#if WPF
//                        viewlist.TARIFF_CODE = Convert.ToInt32(lab.Content.ToString());
//#endif
//                        break;
//                    case 4:
//#if WPF
//                        viewlist.TARIFF_NAME = lab.Content.ToString();
//#endif
//                        break;
//                    case 5:
//#if WINFORMS
//                        viewlist.RESOURCE_TYPE = lab.Text;
//#endif
//#if WPF
//                        viewlist.RESOURCE_TYPE = lab.Content.ToString();
//#endif
//                        viewlist.TCR = 0.0M;
//                        break;
//                    case 6:
//#if WINFORMS
//                        viewlist.METER_TYPE = lab.Text;
//#endif
//#if WPF
//                        viewlist.METER_TYPE = lab.Content.ToString();
//#endif
//                        break;
//                    case 7:
//#if WINFORMS
//                        viewlist.PAYMENT_NAME = lab.Text;
//#endif
//#if WPF
//                        viewlist.PAYMENT_NAME = lab.Content.ToString();
//#endif
//                        break;
//                    case 8:
//#if WINFORMS
//                        viewlist.TOTAL_AMOUNT = lab.Text;
//#endif
//#if WPF
//                        viewlist.TOTAL_AMOUNT = lab.Content.ToString();
//#endif
//                        break;
//                }
//                count++;
//            }
//#if WINFORMS || WPF  || WINUI || SMARTMAUI
//                        //Grid gridlist = (Grid)label_cell.Parent.Parent.Parent.Parent.LogicalChildren[0];
//                        //Label header = (Label)gridlist.Children[column_index];
//#endif
//#if ANDROIDX
//            // This needs re-doing as well!!!
//            Context xyz = Android.App.Application.Context;
//            TextView header = new TextView(xyz) { Text = "Something" };
//#endif
//                        // Find the category given the screen
//                        //SmartUtilityV2022.UtilityCostsMouseDoubleClick_Actual(gridlist,
//                        //                                    ourviewmodel,
//                        //                                    utilityviewmodel,
//                        //                                    components,
//                        //                                    //header.Content.ToString(),
//                        //                                    ourviewmodel.Blanche.vatRatesList,
//                        //                                    ourviewmodel.Blanche.exchangeRatesList);
//                        return;
//        }

#if WINFORMS
        internal static async void UtilityCultures_SelectionChanged(object sender, EventArgs e,
                                                                MainProcess components,
                                                                MainViewModel ourviewmodel,
                                                                UtilityViewModel utilityviewmodel)

        {
#endif
#if WPF
        internal static async void UtilityCultures_SelectionChanged(object sender, RoutedEventArgs e,
                                                            MainViewModel ourviewmodel,
                                                            UtilityViewModel utilityviewmodel)
        {
#endif
#if WINUI
        internal static async void UtilityCultures_SelectionChanged(object sender, SelectionChangedEventArgs e,
                                                                MainViewModel ourviewmodel,
                                                                UtilityViewModel utilityviewmodel)
        {
#endif
#if ANDROIDX
        internal async static void UtilityCultures_SelectionChanged(object sender, EventArgs e,
                                                            AppCompatActivity meterActivity,
                                                            MainViewModel ourviewmodel,
                                                            UtilityViewModel utilityviewmodel)
        {
#endif
#if SMARTMAUI
        internal static async void UtilityCultures_SelectionChanged(object sender, EventArgs e,
                                                            MainViewModel ourviewmodel,
                                                            UtilityViewModel utilityviewmodel)
        {
#endif
#if WINFORMS || WPF  || WINUI || SMARTMAUI
            // For WINFORMS, if you modify the comboBox items in here, you
            // get an infinite chimp-generated loop ... you couldn't make this shit up,
            // you really couldn't


#if WINFORMS || WPF  || WINUI
            ComboBox spinner = sender as ComboBox;
            // Well ...why is this Bollocks Chimp-trap here?  Because
            // whilst evrything works FINE with WPF, the WINUI bollocks
            // a) has to have the ComboBox Editable="true" and ALSO (!!!)
            // b) when you update using Notify in the utilityviewmodel,
            // it generates the "SelectionChanged" event for reasons best known
            // to its chimpy self.  So I trap the fact the SelectedIndex is -1
            // and return.  Then it comes BACK to the SelectionChanged routine
            // (so thats THREE Chimp-times) and exits gracefully because the
            // culture_mode hasn't changed ...  You couldn't make this chimp-fuck
            // shit up, you really couldn't ...
            if (spinner.SelectedIndex == -1)
            {
                return;
            }
#endif
#if SMARTMAUI
            Picker spinner = sender as Picker;
            // Well ...why is this Bollocks Chimp-trap here?  Because
            // whilst evrything works FINE with WPF, the WINUI bollocks
            // a) has to have the ComboBox Editable="true" and ALSO (!!!)
            // b) when you update using Notify in the utilityviewmodel,
            // it generates the "SelectionChanged" event for reasons best known
            // to its chimpy self.  So I trap the fact the SelectedIndex is -1
            // and return.  Then it comes BACK to the SelectionChanged routine
            // (so thats THREE Chimp-times) and exits gracefully because the
            // culture_mode hasn't changed ...  You couldn't make this chimp-fuck
            // shit up, you really couldn't ...
            if (spinner.SelectedIndex == -1)
            {
                return;
            }
#endif
#endif
#if ANDROIDX
            // By LUCK more than by anything else!!!!
            Spinner spinner = sender as Spinner;
#endif
#if WINFORMS
            if (spinner.Enabled)
#endif
#if WPF  || WINUI || SMARTMAUI
            if (spinner.IsEnabled)
#endif
#if ANDROIDX
            if (spinner.Enabled)
#endif
            {
#if WINFORMS
                if (FrontEndGUI.DecodeEventFlags(e) != null)
#endif
#if WPF  || WINUI || SMARTMAUI
                if (FrontEndGUI.DecodeRoutedEventFlags(e) != null)
#endif
#if ANDROIDX
                if (FrontEndGUI.DecodeCheckedChangedEventFlags(e) != null)
#endif
                {

#if WINFORMS || WPF  || WINUI || SMARTMAUI
                    MainViewModel.CultureItem citem = spinner.SelectedItem as MainViewModel.CultureItem;
                    string culture_code = citem.Content;
                    int position = spinner.SelectedIndex;
#endif
#if ANDROIDX
                    int position = spinner.SelectedItemPosition;
                    string culture_code = utilityviewmodel.UtilityCulturesList[position].Content;
#endif
                    if (culture_code != utilityviewmodel.UtilityCultureCode)
                    {
                        SmartData.CultureView cview = SmartSpikeV2017.Lookup_CULTURE_Info(ourviewmodel.cultureviewList,
                                                                                        culture_code);
                        // Time for a change
                        utilityviewmodel.UtilityCultureCode = culture_code;
                        utilityviewmodel.CultureINF = cview.CULTUREINFO;

                        // Adjust the drop down
                        if (utilityviewmodel.LastCultureSelectedIndex >= 0)
                        {
                            utilityviewmodel.UtilityCulturesList[utilityviewmodel.LastCultureSelectedIndex].Colour =
                                                ourviewmodel.blackColour;
                            utilityviewmodel.UtilityCulturesList[position].Colour = ourviewmodel.greenColour;
                            utilityviewmodel.LastCultureSelectedIndex = position;
                        }
                        // Adjust it here, as it is an index into the transactions
                        if (!await SmartUtilityV2022.Utility_CultureChanged_Actual(
#if WINFORMS
                                                                                components,
#endif
#if ANDROIDX
                                                                                    meterActivity,
#endif
                                                                                    ourviewmodel,
                                                                                    utilityviewmodel))
                        {
                            // Do nothing for the time being ...
                            utilityviewmodel.errorMessage = "Cannot change Utility culture";
                            return;
                        }
                    }
                }
            }
#if WINFORMS || WPF  || WINUI || SMARTMAUI
            else
            {
                utilityviewmodel.LastCultureSelectedIndex = spinner.SelectedIndex;
            }
#endif
            return;
        }

#if WINFORMS
        internal static async void RadioButtonRatesClicked(object sender, EventArgs e,
                                                    MainProcess components,
                                                    MainViewModel ourviewmodel,
                                                    UtilityViewModel utilityviewmodel)
#endif
#if WPF
        internal static async void RadioButtonRatesClicked(object sender, RoutedEventArgs e,
                                                    MainViewModel ourviewmodel,
                                                    UtilityViewModel utilityviewmodel)
#endif
#if WINUI
        internal static async void RadioButtonRatesClicked(object sender, RoutedEventArgs e,
                                                    MainViewModel ourviewmodel,
                                                    UtilityViewModel utilityviewmodel)
#endif
#if ANDROIDX
        internal async static void RadioButtonRatesClicked(object sender, EventArgs e,
                                                    AppCompatActivity meterActivity,
                                                    MainViewModel ourviewmodel,
                                                    UtilityViewModel utilityviewmodel)
#endif
#if SMARTMAUI
        internal static async void RadioButtonRatesClicked(object sender, EventArgs e,
                                                    MainViewModel ourviewmodel,
                                                    UtilityViewModel utilityviewmodel)
#endif

        {
            RadioButton radiobutton = sender as RadioButton;

            string targetCurrency =
#if WINFORMS
                radiobutton.Text.ToString();
#endif
#if WPF  || WINUI || SMARTMAUI
                radiobutton.Content.ToString();
#endif
#if ANDROIDX
                radiobutton.Tag.ToString();
#endif
            if (targetCurrency != "")
            {
                SmartData.CultureView cview = SmartSpikeV2017.Lookup_Currency_OrdinalNew(ourviewmodel.cultureviewList,
                                                                        ourviewmodel.currenciesList,
                                                                        targetCurrency);
                if (cview.CURRENCY_ORDINAL >= 0)
                {
                    if (utilityviewmodel.CurrencyOrdinal != cview.CURRENCY_ORDINAL)
                    {
                        utilityviewmodel.CurrencyOrdinal = cview.CURRENCY_ORDINAL;
                        utilityviewmodel.ConvertToSymbol = cview.ISOCURRENCYSYMBOL;
                        await SmartUtilityV2022.Utility_RateChanged_Actual(
#if WINFORMS
                                                            components,
#endif
#if ANDROIDX
                                                            meterActivity,
#endif
                                                            ourviewmodel,
                                                            utilityviewmodel,
                                                            cview.CURRENCY_ORDINAL,
                                                            targetCurrency);
#if ANDROIDX
                        if (radiobutton != utilityviewmodel.UtilityRBNON)
                        {
                            utilityviewmodel.UtilityRBNON.SetTextColor(ourviewmodel.blackColour);
                        }
                        else
                        {
                            utilityviewmodel.UtilityRBNON.SetTextColor(ourviewmodel.greenColour);
                        }
                        if (radiobutton != utilityviewmodel.UtilityRBGBP)
                        {
                            utilityviewmodel.UtilityRBGBP.SetTextColor(ourviewmodel.blackColour);
                        }
                        else
                        {
                            utilityviewmodel.UtilityRBGBP.SetTextColor(ourviewmodel.greenColour);
                        }
                        if (radiobutton != utilityviewmodel.UtilityRBEUR)
                        {
                            utilityviewmodel.UtilityRBEUR.SetTextColor(ourviewmodel.blackColour);
                        }
                        else
                        {
                            utilityviewmodel.UtilityRBEUR.SetTextColor(ourviewmodel.greenColour);
                        }
                        if (radiobutton != utilityviewmodel.UtilityRBUSD)
                        {
                            utilityviewmodel.UtilityRBUSD.SetTextColor(ourviewmodel.blackColour);
                        }
                        else
                        {
                            utilityviewmodel.UtilityRBUSD.SetTextColor(ourviewmodel.greenColour);
                        }
                        if (radiobutton != utilityviewmodel.UtilityRBJPY)
                        {
                            utilityviewmodel.UtilityRBJPY.SetTextColor(ourviewmodel.blackColour);
                        }
                        else
                        {
                            utilityviewmodel.UtilityRBJPY.SetTextColor(ourviewmodel.greenColour);
                        }
#endif
                    }
                }
#if WPF
                //e.Handled = true;
#endif
            }
            return;
        }

#if WINFORMS
        internal static void UtilityMouseRightButtonDown(object sender, object e,
                                            MainViewModel ourviewmodel,
                                            UtilityViewModel utilityviewmodel)
#endif
#if WPF  || WINUI
        internal void UtilityMouseRightButtonDown(object sender, object e,
                                            MainViewModel ourviewmodel,
                                            UtilityViewModel utilityviewmodel)
#endif
#if ANDROIDX
        internal void UtilityMouseRightButtonDown(object sender, object e,
                                            MainViewModel ourviewmodel,
                                            UtilityViewModel utilityviewmodel)
#endif
#if SMARTMAUI
        internal void UtilityMouseRightButtonDown(object sender, object e,
                                            MainViewModel ourviewmodel,
                                            UtilityViewModel utilityviewmodel)
#endif

        {
#if SMARTMAUI
            //var isRightClick = gestureRecognizer.Button == MouseButton.Right;
#endif


            if (sender != null &&


#if WINFORMS || WPF || SMARTMAUI
                FrontEndGUI.DecodeMouseEventFlags(e) != null)
#endif
#if WINUI
                FrontEndGUI.DecodeRightTappedRoutedEventFlags(e) != null)
#endif
#if ANDROIDX
                FrontEndGUI.DecodeMouseEventFlags(e) != null)
#endif
            {
                UtilityViewModel.SupplierItem supplier_item = FrontEndGUI.DecodeSupplierItem(sender);
                if (!string.IsNullOrEmpty(supplier_item.Content))
                {
#if WINFORMS
                    SmartUtilityV2022.Utility_Supplier_Selected_Actual(ourviewmodel,
                                                                        utilityviewmodel,
                                                                        supplier_item.Content);
#endif
#if WPF  || WINUI || SMARTMAUI
                    SmartUtilityV2022.Utility_Supplier_Selected_Actual(ourviewmodel,
                                                                        utilityviewmodel,
                                                                        this,
                                                                        supplier_item.Content);
#endif
#if ANDROIDX
                    SmartUtilityV2022.Utility_Supplier_Selected_Actual(//(ContentPage)this.Parent,
                                                                    ourviewmodel,
                                                                    utilityviewmodel,
                                                                    supplier_item.Content);
#endif
#if WPF
                    // Not sure what effect THIS has
                    FrontEndGUI.DecodeMouseEventFlags(e).Handled = true;
#endif
                }
            }
            return;
        }

#if WINFORMS
        internal static async void Utility_SupplierMouseDoubleClick(object sender, MouseEventArgs e, MainProcess process_components,
                                            MainViewModel ourviewmodel,
                                            UtilityViewModel utilityviewmodel)
#endif
#if WPF  || WINUI
        internal static async void Utility_SupplierMouseDoubleClick(object sender, object e,
                                            MainViewModel ourviewmodel,
                                            UtilityViewModel utilityviewmodel)
#endif
#if ANDROIDX
        internal static async void Utility_SupplierMouseDoubleClick(object sender, object e,
                                            MainViewModel ourviewmodel,
                                            UtilityViewModel utilityviewmodel)
#endif
#if SMARTMAUI
        internal static async void Utility_SupplierMouseDoubleClick(object sender, object e,
                                            MainViewModel ourviewmodel,
                                            UtilityViewModel utilityviewmodel)
#endif
        {
            if (sender != null &&
#if WINFORMS || WPF || SMARTMAUI
                FrontEndGUI.DecodeMouseEventFlags(e) != null)
#endif
#if WINUI
                FrontEndGUI.DecodeDoubleTappedRoutedEventFlags(e) != null)
#endif
#if ANDROIDX
                FrontEndGUI.DecodeEventFlags(e) != null)
#endif
            {
                bool status = false;

#if WINFORMS
                ComboBox supplier = (ComboBox)sender;
                utilityviewmodel.SupplierSelectedIndex = supplier.SelectedIndex;
#endif
                if (utilityviewmodel.SupplierSelectedIndex != -1)
                {
                    int selected_index = utilityviewmodel.SupplierSelectedIndex;
#if WINFORMS || WPF  || WINUI || SMARTMAUI
                    UtilityViewModel.SupplierItem supplier_item = utilityviewmodel.UtilitySuppliersList[selected_index];
#endif
#if ANDROIDX
                    // I don't believe it... this bollocks is ACTUALLY WORKING
                    UtilityViewModel.SupplierItem supplier_item = utilityviewmodel.UtilitySuppliersList[selected_index];
#endif
                    // Popup Connection Changed screen for WINFORMS
                    status = await SmartUtilityV2022.Utility_SuppliersChanged_Actual(
#if WINFORMS
                                                                                    process_components,

#endif
                                                                                        signinviewmodel,
                                                                                        ourviewmodel,
                                                                                        utilityviewmodel,
                                                                                        supplier_item);
                }
#if WPF
                // No WINUI
                // Not sure what effect THIS has
                FrontEndGUI.DecodeMouseEventFlags(e).Handled = true;
#endif
            }
            return;
        }

//#if ANDROIDX
//        // Here

//        // Thanks to Patil2421 https://forums.xamarShit.com/discussion/91188/handling-button-double-click-double-tap
//        //Declare a variable
//        public int ButtonCount = 0;

//        // Supplier Button Click Event:
//        public void SupplierButtonClick(object sender, EventArgs e)
//        {
//            if (sender != null && e != null)
//            {
//                if (ButtonCount < 1)
//                {
//                    TimeSpan tt = new TimeSpan(0, 0, 1);

//                }
//                ButtonCount++;
//            }
//            return;
//        }

//        public Func<bool> TestHandleFunction(object sender, EventArgs e)
//        {
//            if (ButtonCount > 1)
//            {
//                //Your action for Double Click here
//                //DisplayAlert("", "Two Clicks", "OK");

//                Utility_SupplierMouseDoubleClick(sender, e);
//            }
//            else
//            {
//                //Your action for Single Click here
//                //DisplayAlert("", "One Click", "OK");
//                SuppliersStartUp(); // (sender, e);
//            }
//            ButtonCount = 0;
//            return () => false;
//        }
//        public void Utility_SupplierMouseDClick(object sender, EventArgs e)
//        {
//            Utility_SupplierMouseDoubleClick(sender, e);
//            return;
//        }

//        public void SuppliersStartUp() // (object sender, EventArgs e)
//        {
//            utilityviewmodel.SupplierSelectedIndex = -1;
//            SuppliersStartUp_Actual(); // (sender, e);
//            return;
//        }
//#endif

//#if ANDROIDX
//        public static bool SuppliersStartUp_Actual()
//        {
//#if ANDROIDX
//            Android.Widget.LinearLayout parent = null;
//#endif

//            if (utilityviewmodel.UtilitySupplier_Selections.Length == 0)
//            {
//                utilityviewmodel.UtilitySupplier_Selections = (dinamic[])utilityviewmodel.UtilitySuppliersList.ToArray();
//            }
//            // Sometimes this element isn't XAML-ed up when we want to set the starting supplier index
//            if (utilityviewmodel.suppliers_frame.Width != -1)
//            {
//                utilityviewmodel.SupplierWidth = utilityviewmodel.suppliers_frame.Width;
//                utilityviewmodel.SupplierTranslationX = utilityviewmodel.suppliers_frame.TranslationX;
//                utilityviewmodel.SupplierTranslationY = utilityviewmodel.suppliers_frame.TranslationY;
//#if ANDROIDX
//                double screenCoordinateX = utilityviewmodel.suppliers_frame.GetX();
//                double screenCoordinateY = utilityviewmodel.suppliers_frame.GetY();
//#endif

//                // Get the view's parent (if it has one...)
//#if ANDROIDX
//                if (utilityviewmodel.suppliers_frame.Parent.GetType() != typeof(Android.Widget.FrameLayout))
//#endif
//                {
//                    // Loop back through all parents
//                    while (parent != null)
//                    {
//                        // Add in the coordinates of the parent with respect to ITS parent
//                        // If the parent of this parent isn't the app itself, get the parent's parent.
//#if ANDROIDX
//                        if (parent.Parent.GetType() == typeof(Android.Widget.FrameLayout))
//#endif
//                        {
//                            break;
//                        }
//                        else
//                        {
//                        }
//                    }
//                    // Return the final coordinates...which are the global SCREEN coordinates of the view
//                    utilityviewmodel.SupplierTranslationX = screenCoordinateX;
//                    utilityviewmodel.SupplierTranslationY = screenCoordinateY + utilityviewmodel.suppliers_frame.Height;                    
//                }
//            }

//            try
//            {
//                UtilitySupplierDropdown.OpenUtilitySuppliers(
//                                                                utilityviewmodel,
//                                                                SmartParametersV2016.supplierprompt,
//                                                                sb => utilityviewmodel.UtilitySuppliers_Text = sb,
//                                                                cb => utilityviewmodel.UtilitySuppliersColour = cb);
//                int id0 = (int)typeof(Resource.Drawable).GetField("expander_close").GetValue(null);
//                utilityviewmodel.ArrowButton.SetImageResource(id0);
//            }
//            catch (Exception ex)
//            {
//                Console.Write(ex.Message);
//            }  
//            return true;
//        }
//#endif


#if WINFORMS
        internal static void ExchangeRatesClicked(object sender, EventArgs e,
                                            MainViewModel ourviewmodel,
                                            UtilityViewModel utilityviewmodel)
        {
#endif
#if WPF
        internal static void ExchangeRatesClicked(object sender, RoutedEventArgs e,
                                            MainViewModel ourviewmodel,
                                            UtilityViewModel utilityviewmodel)
        {
#endif
#if WINUI
        internal async void ExchangeRatesClicked(object sender, RoutedEventArgs e,
                                            MainViewModel ourviewmodel,
                                            UtilityViewModel utilityviewmodel)
        {
#endif
#if ANDROIDX
        internal async void ExchangeRatesClicked(object sender, EventArgs e,
                                            MainViewModel ourviewmodel,
                                            UtilityViewModel utilityviewmodel)
        {
#endif
#if SMARTMAUI
        internal async void ExchangeRatesClicked(object sender, EventArgs e,
                                            MainViewModel ourviewmodel,
                                            UtilityViewModel utilityviewmodel)
        {
#endif
            // Make sure passed parameters are valid
            if ((sender != null) &&
                (FrontEndGUI.DecodeEventFlags(e) != null))
            {
                DateTime target_date = SmartTimeV2016.ConvertDateTime("2022-01-01");
                ourviewmodel.ExchangeRatesReduced = SmartSpikeV2017.ExchangeRates_Reduced(ourviewmodel,
                                                                target_date);
                if (ourviewmodel.ExchangeRatesReduced.Count > 0)
                {
                    // Here - the CHimps want me to turn UtilityView into a ContentPAGE
                    // instead of leaving it as a ContentVIEW and I'm not prepared to make
                    // that change as yet; they have FUCKED ME OVER ALREADY with this MEOOWI shit
                    // and I'm simply NOT IN THE MOOD

#if WINFORMS || WPF
                    if (!SmartUtilityV2022.Utility_ButtonExchangeRatesClick_Actual(
#endif
#if WINUI || SMARTMAUI
                    if (!await SmartUtilityV2022.Utility_ButtonExchangeRatesClick_Actual(
#endif
#if ANDROIDX
                    if (!SmartUtilityV2022.Utility_ButtonExchangeRatesClick_Actual(
#endif
#if WINUI
                                                                (ContentDialog)this.Parent,
#endif
#if ANDROIDX
                                                                (PopupWindow)this.Parent,
#endif
                                                                ourviewmodel,
                                                                utilityviewmodel))
                    {
                        if (ourviewmodel.trace)
                        {
                            GiveUp(ourviewmodel, utilityviewmodel, "ExchangeRates popup failed");
                        }
                    }
                    else
                    {
#if WINUI
                        await SmartRoutinesV2018.CheckTrace(ourviewmodel, utilityviewmodel.utilityToken, 0, 0, "Exchange rates clicked");
#endif
#if ANDROIDX
                        await SmartRoutinesV2018.CheckTrace(ourviewmodel, utilityviewmodel.utilityToken, 0, 0, "Exchange rates clicked");
#endif
                    }
                }
#if WPF
                // Not sure what effect THIS has (stops it bubbling up)
                e.Handled = true;
#endif
            }
            return;
        }

#if WPF
        internal async void OnKeyUp(object sender, object e,
                                    SignInViewModel signinviewmodel,
                                    MainViewModel ourviewmodel)
        {
            bool rightorleft = FrontEndGUI.DecodeKeyEventArgs(e);
            if (rightorleft)
            {
                // Go right
                await SmartRoutinesV2018.ButtonRightClickedActual(signinviewmodel, ourviewmodel);
            }
            else
            {
                if (!rightorleft)
                {
                    // Go Left
                    await SmartRoutinesV2018.ButtonLeftClickedActual(signinviewmodel, ourviewmodel);
                }
            }
            return;
        }
#endif
#if WINUI
        // https://stackoverflow.com/questions/46962632/use-swipe-gesture-in-WINUI
        // https://stackoverflow.com/questions/45550684/horizontal-swipe-gesture-on-WINUI
        // Thanks to Justin XL 

        private void SwipeableTextBlock_ManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
        {
            if (e.IsInertial)
            {
                var swipedDistance = e.Cumulative.Translation.X;

                if (Math.Abs(swipedDistance) <= 2) return;

                if (swipedDistance > 0)
                {
                    SmartRoutinesV2018.OnSwipedRightActual(sender, e, signinviewmodel, ourviewmodel);
                }
                else
                {
                    SmartRoutinesV2018.OnSwipedLeftActual(sender, e, signinviewmodel, ourviewmodel);
                }
            }
            return;
        }

        private void SwipeableTextBlock_ManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e)
        {
            if (sender != null &&
                e != null)
            {
                // Left in for future compatibility
            }
            return;
        }
#endif
#if SMARTMAUI
        private async void OnRightKey(object sender, EventArgs e)
        {
            await SmartRoutinesV2018.ButtonRightClickedActual(signinviewmodel, ourviewmodel);
        }

        private async void OnLeftKey(object sender, EventArgs e)
        {
            await SmartRoutinesV2018.ButtonLeftClickedActual(signinviewmodel, ourviewmodel);
        }
#endif
#if ANDROIDX
        public class CultureItemSelectedListener : Java.Lang.Object, AdapterView.IOnItemSelectedListener
        {
            private AppCompatActivity activity;
            private MainViewModel ourviewmodel;
            private UtilityViewModel utilityviewmodel;
            public CultureItemSelectedListener(AppCompatActivity meterActivity, MainViewModel mainvm, UtilityViewModel utilityvm)
            {
                this.activity = meterActivity;
                this.ourviewmodel = mainvm;
                this.utilityviewmodel = utilityvm;
            }
            public void OnItemSelected(AdapterView parent, View view, int position, long id)
            {
                // Change the selected item's color to GREEN
                ((TextView)view.FindViewById<TextView>(Resource.Id.textView)).SetTextColor(ourviewmodel.greenColour); //Change selected text color
                // And call the "SelectedItemChanged" routine directly
                EventArgs e = new EventArgs();
                UtilityView.UtilityCultures_SelectionChanged(utilityviewmodel.UtilityCultures, e,
                                                                    this.activity,
                                                                    ourviewmodel,
                                                                    utilityviewmodel);
                return;
            }

            public void OnNothingSelected(AdapterView parent)
            {
            }
        }
#endif
    }
}