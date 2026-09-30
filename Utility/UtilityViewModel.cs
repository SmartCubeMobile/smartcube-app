using System.Text;
using System.Globalization;
//using System.Net.Mail;
using System.ComponentModel;
using System.IO;

#if DBSERVER
using System;
#endif
// In NuGET use 'Data Visualization Toolkit' to find it
//using Primitives;
//#endif

#if WINFORMS
using System.Windows;
using System.Windows.Forms;
using System.Drawing;
//using System.Windows.Data;
using OxyPlot.WindowsForms;
//using System.Windows.Data;
using System.Windows.Markup;
using OxyPlot;
#endif

#if WPF
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Markup;
using System.Windows.Media.Imaging;
using System.Windows;
using System.Windows.Data;
using OxyPlot.Wpf;
using OxyPlot;
#endif


#if WINUI
using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Controls;
using CommunityToolkit.WinUI.UI.Controls;
using OxyPlot;
using WinUIColor = Windows.UI.Color;
using WinUIColors = Microsoft.UI.Colors;
#endif

#if ANDROIDX
using Android.Graphics;
using OxyPlot.Xamarin.Android;
using Android.Content;
using OxyPlot;
#endif

#if SMARTMAUI
using Microsoft.Maui.Controls.Shapes;
using CommunityToolkit.Maui.Views;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
#endif

namespace SmartCubeMobile
{

#if WPF
    public class UtilityDateConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var date = value as DateTime?;
            if (!date.HasValue) return value;

            // Use the passed-in culture (from binding or fallback to default)
            return date.Value.ToString("d", culture ?? CultureInfo.CurrentCulture);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string str && DateTime.TryParse(str, culture, DateTimeStyles.None, out var result))
            {
                return result;
            }
            return DependencyProperty.UnsetValue;
        }
        //public object Convert(object value, Type targetType, object parameter, CultureInfo cultureinfo,
        //                        UtilityViewModel utilityviewmodel)
        //{
        //    var d = value as DateTime?;
        //    if (!d.HasValue) return value;
        //    return d.Value.ToString("d", utilityviewmodel.CultureINF);
        //}

        //public object ConvertBack(object value, Type targetType, object parameter, CultureInfo cultureInfo)
        //{
        //    DateTime dto = (DateTime)value;
        //    return dto;
        //}
    }
#endif
#if WINUI
    public class UtilityDateConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var d = value as DateTime?;
            if (!d.HasValue) return value;

            var culture = !string.IsNullOrEmpty(language)
                ? new CultureInfo(language)
                : CultureInfo.InvariantCulture;

            return d.Value.ToString("d", culture);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string something)
        {
            DateTime dto = (DateTime)value;
            return dto;
        }

        //public object Convert(object value, Type targetType, object parameter, string somethings,
        //                        UtilityViewModel utilityviewmodel)
        //{
        //    var d = value as DateTime?;
        //    if (!d.HasValue) return value;
        //    return d.Value.ToString("d", utilityviewmodel.CultureINF);
        //}

        //public object ConvertBack(object value, Type targetType, object parameter, string something)
        //{
        //    DateTime dto = (DateTime)value;
        //    return dto;
        //}
    }
#endif

#if SMARTMAUI
    public class UtilityDateConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var date = value as DateTime?;
            if (!date.HasValue) return value;

            // Use the passed-in culture (from binding or fallback to default)
            return date.Value.ToString("d", culture ?? CultureInfo.CurrentCulture);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string str && DateTime.TryParse(str, culture, DateTimeStyles.None, out var result))
            {
                return result;
            }
            return Binding.DoNothing;
        }
    }
#endif
    public class UtilityViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged(string propName)
        {
            if (this.PropertyChanged != null &&
                propName != null)                   // Just in case!!
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propName));
            }
        }

        private string errormessageDefault = "";
        public string errorMessage
        {
            get
            {
                return errormessageDefault;
            }
            set
            {
                if (errormessageDefault != value)
                {
                    errormessageDefault = value;
                    NotifyPropertyChanged(nameof(errorMessage));
                }
            }
        }
        internal SmartUtilityList Hezbollah = new SmartUtilityList();    // Initialized in DoWork leave as null for SmartDashboard
        
#if !DBSERVER

        // Sometimes .. we do Utility scrapes without the Cube face showing
        internal char cubeface_code = SmartParametersV2016.Utility;

        internal Decimal VAT_RATE = 0;

        internal DateTime genericTargetDate = SmartParametersV2016.defaultDate;
        internal string genericCultureTargetDate = "";
        internal string genericErrorMessage = "";
        internal int genericTransactionValue = 0;
        internal decimal genericDecimalValue = 0;
        internal short payment_item = 0;
        internal string exit_test = "";
        internal string yes_routine = "";
        internal string no_routine = "";
        internal string comments = "";
        internal string relative_pathname = "";
        internal string[][] sections = null;
        internal int matchingLine = 0;
        internal string token = "";
        internal string href = "";
        internal List<SmartUtility.Bills> bills_tempList = new List<SmartUtility.Bills>();
        internal List<SmartUtility.Payments> payments_tempList = new List<SmartUtility.Payments>();
        internal string tariffName = "";
        internal int currentIndex = 0;
        internal string DISCOUNTCREDITDATE = "";
        internal string AMOUNT = "";
        internal int this_resource = -1;
        internal short charges_item = 0;
        internal short units_band = 0;
        internal short discount_item = 0;
        internal string first_amount = "";
        internal string next_amount = "";
        internal string first_line = "";
        internal string next_line = "";

        internal string DISCOUNT_TYPE = "";
        internal string DISCOUNT_AMOUNT = "";
        internal DateTime DISCOUNT_DATE = SmartParametersV2016.defaultDate;

        internal string value = "";
        internal short vat_code = 0;
        internal string BILL_PERIOD_START = SmartParametersV2016.defaultDates;
        internal string BILL_PERIOD_END = SmartParametersV2016.defaultDates;
        internal string working_token = "";
        internal int temp_count = 0;
        internal string prfix = "";
        internal string READINGS_PERIOD_START = SmartParametersV2016.defaultDates;
        internal string READINGS_PERIOD_END = SmartParametersV2016.defaultDates;
        internal string PREVIOUS_BALANCE = "";

        internal int total_mantissa = 0;

        internal string STANDING_CHARGES_PERIOD_START = SmartParametersV2016.defaultDates;
        internal string STANDING_CHARGES_PERIOD_END = SmartParametersV2016.defaultDates;

        internal string UNIT_CHARGES_PERIOD_START = SmartParametersV2016.defaultDates;
        internal string UNIT_CHARGES_PERIOD_END = SmartParametersV2016.defaultDates;

        internal string CLOSING_DATE = "";
        internal string UNITS_COST = "0";
        internal string UNITS = "0";
        internal string UNITS_RATE = "0";
        internal string CUBIC_METRES_USED = "0";
        internal string VOLUMECORRECTION = "0";
        internal string KWHCONVERSION = "0";

        internal bool two_tier_meter = false;
        internal string kwh_line = "";
        internal int units_limit = 0;

        internal string METER_SERIAL_NO = "";
        internal string UNITS_TYPE = "";
        internal string METER_TYPE = "";
        internal string READ_DATE = "";
        internal string READ_TYPE = "";
        internal string THIS_READ_TYPE = "";
        internal string LAST_READ_TYPE = "";
        internal string D_THIS_READ = "";
        internal string D_LAST_READ = "";
        internal string D_UNITS_USED = "";
        internal string N_THIS_READ = "";
        internal string N_LAST_READ = "";
        internal string N_UNITS_USED = "";
        internal string N_UNITS_USED_KWH = "";
        internal string TOTAL_UNITS_USED = "";
        internal bool day_rate = false;
        internal string FROM_TYPE = "";
        internal string M3 = "";
        internal string D_UNITS_USED_M3 = "";
        internal string D_UNITS_USED_KWH = "";
        internal string UNIT_OF_MEASURE = "";
        internal string DISCOUNT_CREDIT_DATE = "";
        internal string OUTSTANDING_BALANCE = "";
        internal string TEMPS = "";
        internal string CHARGES_PERIOD = "";

        internal string CHARGES_DAYS = "";
        internal string STANDING_CHARGE = "";
        internal string CHARGES_COST = "";
        internal string CHARGES_TYPE = "";

        internal string PAYMENT_CODE = "";
        internal string PAYMENT_AMOUNT = "";
        internal string PAYMENT_METHOD = "";
        internal string PAYMENT_DATE = "";
        internal string PAYMENT_BALANCE = "";

        internal string ACCOUNT_AMOUNT = "";
        internal string ACCOUNT_TYPE = "";
        internal string ACCOUNT_DATE = "";

        internal string property_code = "";
        internal string qnb_quote_id = "";
        internal string form_id = "";             // Stays same at every step
        internal string form_build_id = "";
        internal string udprn = "";
        internal string House_No = "";
        internal string House_Name = "";
        internal string Street = "";
        internal string City_Town = "";
        internal string property_value = "";

        //internal int line_count1 = 0;
        internal string[] lines { get; set; }
        internal int main_tabview_index = -1;
        internal int charts_tabview_index = -1;
        internal string resource_description = "";
        internal string acct_type_tag = "";
        internal string transaction_date = "";
        internal string description = "";
        internal DateTime payment_date { get; set; }
        internal int payment_amount = 0; //string transaction_value,
        internal int payment_balance = 0; //string account_balance,

        // Scottish Power
        internal string url_addon = "";
        internal string sessionFormUID = "";
        internal string login_post_action = "";

        internal bool new_resource = false;
        internal string eventID = "";
        internal string post_action = "";

        internal string my_energy_usage_path = "";
        internal string view_my_balance_path = "";
        internal string edit_personal_details_path = "";
        internal string payment_plan_path = "";

        internal string gotomyaccount_pathname = "";
        internal string payment_plan_pathname = "";

        internal string first_name = "";
        internal string last_name = "";
        internal string middle_name = "";
        internal int readings_count = 0;
        //

        // SSE
        internal string your_products_pathname = "";
        internal string spare1 = "";    // Name
        internal string spare2 = "";    // Address
        internal string spare3 = "";    // Phone No
        internal string spare4 = "";    // E-mail
        internal string spare5 = "";    // DoB
        internal string SC = "";
        internal bool sc_tracker = false;
        //
        internal int inner_index = 0;
        internal int outer_index = 0;
        internal bool go_back_flag = false;

        internal bool rebuild = true;

        internal CancellationTokenSource utilityCts = new CancellationTokenSource();
        internal CancellationToken utilityToken { get; set; }
        internal CultureInfo CultureINF { get; set; }
        internal int LastCultureSelectedIndex = -1;
        internal string UtilityCultureCode = "";
        internal short CurrencyOrdinal { get; set; }
        internal string ConvertToSymbol { get; set; }

        internal List<SmartUtility.AnalysisBrandsView> analysisbrandsviewList = new List<SmartUtility.AnalysisBrandsView>();
        internal List<SmartUtility.AnalysisTariffsNameView> analysistariffsnameviewList = new List<SmartUtility.AnalysisTariffsNameView>();

#if WPF  || WINUI || SMARTMAUI
        internal ScaleTransform meter_scale = new ScaleTransform();
#endif
        internal char resource_code = SmartParametersV2016.defaultChar;// 'E', 'G' or 'D'
        // This for selection                                                                         
        internal string resourceCodes = "";
        internal string resource_type = "";
        //internal DateTime next_connection = SmartParametersV2016.defaultDate;

#if WPF  || WINUI
        internal DataGrid CostsDataGrid = new DataGrid();
        internal DataGrid BillsDataGrid = new DataGrid();
        internal DataGrid ReadingsDataGrid = new DataGrid();
        internal DataGrid BreakdownDataGrid = new DataGrid();
#endif
#if ANDROIDX
        //internal GridView CostsDataGrid;
        //internal GridView BillsDataGrid;
        //internal GridView ReadingsDataGrid;
        //internal GridView BreakdownDataGrid;
#endif
#if SMARTMAUI
        internal CollectionView CostsDataGrid = new CollectionView();
        internal CollectionView BillsDataGrid = new CollectionView();
        internal CollectionView ReadingsDataGrid = new CollectionView();
        internal CollectionView BreakdownDataGrid = new CollectionView();
#endif
        internal short tempBrandCode = 0;
        internal short tempSupplierCode = 0;


        internal short area_code = 0;   // Don't fink there IS an area code 0??
        internal short charts_supplier_code = 0;
        internal short charts_brand_code = 0;
        internal short age = 0;

        internal char payment_plan = SmartParametersV2016.defaultChar;

        internal DateTime unitrates_updatedx = SmartParametersV2016.defaultDate;

#if WINFORMS
        internal List<SmartUtility.EUnitCharges> e_unit_charges_found = new();
        internal List<SmartUtility.GUnitCharges> g_unit_charges_found = new();
        internal List<SmartUtility.EUnitCharges> d_unit_charges_found = new();
        internal List<SmartUtility.EStandingCharges> e_standing_charges_found = new();
        internal List<SmartUtility.GStandingCharges> g_standing_charges_found = new();
        internal List<SmartUtility.EStandingCharges> d_standing_charges_found = new();
        internal List<SmartUtility.EDiscounts> e_discounts_found = new();
        internal List<SmartUtility.GDiscounts> g_discounts_found = new();
        internal List<SmartUtility.EDiscounts> d_discounts_found = new();
        internal List<SmartUtility.EUsageView> e_usage_chart_found = new();
        internal List<SmartUtility.GUsageView> g_usage_chart_found = new();
        internal List<SmartUtility.EUsageView> d_usage_chart_found = new();



        internal Form ConnectionPage = new Form();
#endif
        internal SmartUtility.Logins login_info = new SmartUtility.Logins();
        internal SmartUtility.Logins newlogin_info = new SmartUtility.Logins();
        internal bool found_it = false;


#if WINFORMS
        internal string drop_down_text = "";
        internal string selection_name = "";
        internal string payment_plans = "";
        internal short conditions_dates_code = 0;

        internal DateTime tariff_valid_from = SmartParametersV2016.defaultDate;
        internal string load_filename = "";

        internal int supplier_index = 0;
        internal string status_message = "";
        internal short version_code = SmartParametersV2016.defaultVersionCode;
        internal string tariff_name = "";
        internal short payment_code = 0;
        internal short conditions_plans_code = 0;
        internal short limit_code = 0;
        internal short tier_count = 0;
        internal DateTime prices_valid_from = SmartParametersV2016.defaultDate;
        internal short tier_level = 0;
        internal char propogate_tariffs = SmartParametersV2016.propogateTariffsDefault;
        internal short post_group_code = 0;
        internal short post_selection_code = 0;
        internal short post_limit_code = 0;
        internal short post_groupings_code = 0;

        internal TimeSpan offset = new TimeSpan(0, 0, 0, 0);
        internal DateTime expiration1 = SmartParametersV2016.defaultDate;
        internal DateTime expiration2 = SmartParametersV2016.defaultDate;
        internal DateTime expiration3 = SmartParametersV2016.defaultDate;
        internal bool subscriber = false;
        internal bool multimeter = false;
        internal bool administrator = false;
        internal DateTime last_login_time = SmartParametersV2016.defaultDate;
#endif
        //  dashboardmodel.area_code = -1;     // Default it to something - its as good as any?
        internal int tariff_code = 0;

        internal bool use_proxy = false;
        internal string bill_token = "";
        internal string bill_currency_symbol = SmartParametersV2016.defaultConvertToSymbol;
        internal char bill_denomination_symbol = SmartParametersV2016.defaultDenominationSymbol;
        internal char bill_currency_separator = SmartParametersV2016.defaultCurrencySeparator;
        internal char bill_thousands_separator = SmartParametersV2016.defaultThousandsSeparator;
        internal char bill_prfix = SmartParametersV2016.defaultBillprfix;
        internal bool download_bills = true;
        internal string prfix_xxx = "";
        internal string target_pathname = "";
        internal DateTime created = SmartParametersV2016.defaultDate;
        internal string account_no = "";
        internal string mpan_mprn = "";
        internal string unknown = "";
        internal decimal volumeCorrection = 0m;
        internal decimal kwhConversion = 0m;
        internal string ageString = "0";

        internal decimal firstReading = 0m;
        internal decimal lastReading = 0m;   // Should all
        internal decimal totalReadings = 0m; // be 0.0M
        internal decimal totalReadingsTemp = 0m; // be 0.0M
        internal List<SmartUtility.EReadings> e_readings_found;
        internal List<SmartUtility.GReadings> g_readings_found;
        internal List<SmartUtility.EReadings> d_readings_found;

        internal DateTime account_created = SmartParametersV2016.defaultDate;

        internal string target_supplier_prfix = "";
        internal string target_supplier_xxx = "";
        internal string target_bill_token = "";
        internal string target_bill_currency_symbol = "";
        internal char target_bill_denomination_symbol = SmartParametersV2016.defaultChar;
        internal char target_bill_currency_separator = SmartParametersV2016.defaultChar;
        internal char target_bill_thousands_separator = SmartParametersV2016.defaultChar;
        internal char target_bill_prfix = SmartParametersV2016.defaultChar;
        internal bool target_useProxy = false;

        internal bool update_account = false;
        internal bool found_resource = false;
        internal string bills_pathname = "";
        internal string payments_pathname = "";
        internal string details_pathname = "";

        internal string your_account_summary_path = ""; //EON!!!

        
        internal string startup_udprn = "";
        internal int returned_codes = 0;

        internal Stream dno_stream = Stream.Null;
        internal bool sucked = false;
        internal string supply_dno = "";

#if WINUI || SMARTMAUI
        internal Frame suppliers_frame = new Frame();

        //internal List<ProviderItem> SelecProviderItms = new List<ProviderItem>();
#endif
#if ANDROIDX        
        // Suppliers spinner dropdown
        internal Spinner Suppliers { get; set; }
        // Tariffs spinner dropdown
        internal Spinner Tariffs { get; set; }
        // PaymentPlans spinner dropdown
        internal Spinner PaymentPlans { get; set; }
        //internal List<ProviderItem> SelecProviderItms = new List<ProviderItem>();
        internal RadioButton RBElectricity { get; set; }
        internal RadioButton RBGas { get; set; }
        internal RadioButton RBDualFuel { get; set; }
        internal Spinner Addresses { get; set; }
        internal DatePicker StartDate { get; set; }
        internal DatePicker EndDate { get; set; }

        internal Spinner UtilityCultures { get; set; }

        internal RadioButton UtilityRBNON { get; set; }
        internal RadioButton UtilityRBGBP { get; set; }
        internal RadioButton UtilityRBEUR { get; set; }
        internal RadioButton UtilityRBUSD { get; set; }
        internal RadioButton UtilityRBJPY { get; set; }

        internal TextView GBPRate { get; set; }
        internal TextView EURRate { get; set; }
        internal TextView USDRate { get; set; }
        internal TextView JPYRate { get; set; }


#endif
        internal bool dual_fuel = false;

        internal string meterFormat = "";    // I HATE null(s)

        internal DateTime[] e_engine_dates = new DateTime[2] { SmartParametersV2016.defaultDate, SmartParametersV2016.defaultDate };
        internal DateTime[] g_engine_dates = new DateTime[2] { SmartParametersV2016.defaultDate, SmartParametersV2016.defaultDate };
        internal int[] analysisCost = new int[2] { 0, 0 };
        internal DateTime tariffs_last_loaded = SmartParametersV2016.defaultDate;
        internal DateTime withdrawn_date = SmartTimeV2016.ConvertDateTime(SmartParametersV2016.sensibleStartingDate);
        internal int e_total_cost = 0,
                        g_total_cost = 0;
        internal decimal volumecorrection = 0.0M;
        internal decimal kwhconversion = 0.0M;

        internal int simulate_pointer = 0;
        internal bool go_simulate = false;

        internal string failure_message = "";    // Shows up for Led6 being red

        internal SmartUtility.SwitchInfo XRAY = new SmartUtility.SwitchInfo();
        internal bool keep_looping = false;
        internal string graph_electricity_pathname = "";
        internal string graph_gas_pathname = "";
        internal string meter_serial_no = "";
        internal string fuelList = "";
        internal string view_your_statement_path = "";
        internal string meter_readings_path = "";
        internal bool clicked = false;

        internal string sessionID_name = "";
        internal string sessionID_value = "";

        internal SmartUtility.SwitchInfo switch_info = new SmartUtility.SwitchInfo();
        internal string switch_brand_name = "";
        internal string switch_tariff_name = "";

        internal int accountnumber_maxlength = 0;
        internal string youraccounts_pathname = "";

        internal string webpage_id = "";
        internal bool bills_out = false;
        internal bool bills_resource_out = false;

        internal int last_date_index = 0;
        internal int tariff_line = 0;

        internal string eon_account_info = "";
        internal string eon_account_name = "";
        internal string eon_tariff_name = "";

        internal string edf_tariff_name = "";
        internal string np_tariff_name = "";
        internal string np_resource_type = "";
#if ANDROIDX
        internal int meter_scale = 0;
#endif
        
        internal List<SmartUtility.TariffCodesNames> tariff_codes_names_subsetList = new List<SmartUtility.TariffCodesNames>();

        internal bool autoswitch_pending = false;
        internal string report = "";
        internal StringBuilder bollocks = new StringBuilder();

        internal List<KeyValuePair<string, string>> keyValues = new List<KeyValuePair<string, string>>();
        // #if FUCKING_DESPERATE <= I was!
        internal HtmlAgilityPack.HtmlDocument htmlDocument = new HtmlAgilityPack.HtmlDocument();

#if WINUI || SMARTMAUI
        internal char ChimpFuckers = SmartParametersV2016.defaultChar;
#endif
#if ANDROIDX
        internal char ChimpFuckers = SmartParametersV2016.defaultChar;
#endif

#if WINFORMS
        internal PlotView p1 = new PlotView();
        internal PlotView p2 = new PlotView();
        internal PlotView p3 = new PlotView();

#endif
        internal bool LoginDeleteClicked = false;

        internal int projectedCost = 0;

        internal List<SmartUtility.AnalysisConditions> already_doneList = new List<SmartUtility.AnalysisConditions>();

        // The definition is here, but each child window gets created when
        // need, otherwise all the RenderTransform.Origin settings go to rat shit.
        // This stuff is a fucking minefield ...
#if WPF  || WINUI || SMARTMAUI
        internal Grid tempGrid = new Grid();
#if WPF
        internal Window childWindow { get; set; }
        //internal TabItem tabItem;
#endif
#if WINUI || SMARTMAUI
        internal Popup childWindow;
        //internal TabViewItem tabItem;
#endif
#endif
#if ANDROIDX
        internal PopupWindow childWindow;
#endif

#if WINUI || SMARTMAUI
        internal int targetItem = 0;
#endif
#if ANDROIDX
        internal int targetItem = 0;
#endif
        internal int passcodeMaxlength = 0;

        private string _tariffdescriptionheaderDefault = "";
        public string TariffDescriptionTitle
        {
            get
            {
                return _tariffdescriptionheaderDefault;
            }
            set
            {
                if (_tariffdescriptionheaderDefault != value)
                {
                    _tariffdescriptionheaderDefault = value;
                    NotifyPropertyChanged(nameof(TariffDescriptionTitle));
                }
            }
        }

        private string _tariffstitleDefault = "";
        public string TariffsTitle
        {
            get
            {
                return _tariffstitleDefault;
            }
            set
            {
                if (_tariffstitleDefault != value)
                {
                    _tariffstitleDefault = value;
                    NotifyPropertyChanged(nameof(TariffsTitle));
                }
            }
        }

        public class TariffDescriptionList
        {
            public string Field { get; set; }
            public string Value { get; set; }
        }

        public List<TariffDescriptionList> tariffdes = new List<TariffDescriptionList>();
        public List<TariffDescriptionList> Tariff { get { return tariffdes; } }

        // To print the ListView
        //internal PrintDialog printdialog;
        private string _moneyDefault = "";
        public string Money
        {
            get
            {
                return _moneyDefault;
            }
            set
            {
                if (_moneyDefault != value)
                {
                    _moneyDefault = value;
                    this.NotifyPropertyChanged(nameof(Money));
                }
            }
        }

        private string _greetingDefault = "";
        public string Greeting
        {
            get
            {
                return _greetingDefault;
            }
            set
            {
                if (_greetingDefault != value)
                {
                    _greetingDefault = value;
                    this.NotifyPropertyChanged(nameof(Greeting));
                }
            }
        }

#if WINFORMS
        private Color autoswitchutilityDefault = Color.DarkRed;
        public Color AutoSwitchUtilityColour
#endif
#if WPF  || WINUI
        private Brush autoswitchutilityDefault = new SolidColorBrush(Colors.DarkRed);
        public Brush AutoSwitchUtilityColour
#endif
#if ANDROIDX
        private Color autoswitchutilityDefault = Color.DarkRed;
        public Color AutoSwitchUtilityColour
#endif
#if SMARTMAUI
        private Color autoswitchutilityDefault = Colors.DarkRed;
        public Color AutoSwitchUtilityColour

#endif
#if WINFORMS || WPF  || WINUI || ANDROIDX || SMARTMAUI
        {
            get
            {
                return autoswitchutilityDefault;
            }
            set
            {
                if (autoswitchutilityDefault != value)
                {
                    autoswitchutilityDefault = value;
                    this.NotifyPropertyChanged(nameof(AutoSwitchUtilityColour));
                }
            }
        }
#endif
        // We can SHOW the form but we disable the Open/Close
        // button unless we have a valid Username passed to us.
        // Take the Username and look it up in the Tariffs database.
        // First - find and open the Tariffs db
        private bool caischeckedDefault = false;
        public bool CAIsChecked
        {
            get
            {
                return caischeckedDefault;
            }
            set
            {
                if (caischeckedDefault != value)
                {
                    caischeckedDefault = value;
                    this.NotifyPropertyChanged(nameof(CAIsChecked));
                }
            }
        }

        private bool frischeckedDefault = false;
        public bool FRIsChecked
        {
            get
            {
                return frischeckedDefault;
            }
            set
            {
                if (frischeckedDefault != value)
                {
                    frischeckedDefault = value;
                    this.NotifyPropertyChanged(nameof(FRIsChecked));
                }
            }
        }

        private string _dateformatDefault = "d";
        public string DateFormat
        {
            get
            {
                return _dateformatDefault;
            }
            set
            {
                if (_dateformatDefault != value)
                {
                    _dateformatDefault = value;
                    this.NotifyPropertyChanged(nameof(DateFormat));
                }
            }
        }

#if WINFORMS || WPF  || WINUI || SMARTMAUI
#if WINFORMS
        private DateTime _startdateDefault = SmartParametersV2016.defaultDate;
        public DateTime StartDate
#endif
#if WPF
        // Careful!  These two were DateTimeOffset!!!
        private DateTime _startdateDefault = SmartParametersV2016.defaultDate;
        public DateTime StartDate
#endif
#if WINUI
        private DateTime _startdateDefault = SmartParametersV2016.defaultDate;
        public DateTime StartDate
#endif
#if SMARTMAUI
        private DateTime _startdateDefault = SmartParametersV2016.defaultDate;
        public DateTime StartDate
#endif
        //#if ANDROIDX
        //        // Careful!  These two were DateTimeOffset!!!
        //        private DateTime _startdateDefault = SmartParametersV2016.defaultDate;
        //        public DateTime StartDate
        //#endif
        {
            get
            {
                return _startdateDefault;
            }
            set
            {
                if (_startdateDefault != value)
                {
                    _startdateDefault = value;
                    this.NotifyPropertyChanged(nameof(StartDate));
                }
            }
        }
#endif

        private bool _startenabledDefault = false;
        public bool StartDateEnabled
        {
            get
            {
                return _startenabledDefault;
            }
            set
            {
                if (_startenabledDefault != value)
                {
                    _startenabledDefault = value;
                    this.NotifyPropertyChanged(nameof(StartDateEnabled));
                }
            }
        }

        private bool _resetdatesenabledDefault = false;
        public bool ResetDatesEnabled
        {
            get
            {
                return _resetdatesenabledDefault;
            }
            set
            {
                if (_resetdatesenabledDefault != value)
                {
                    _resetdatesenabledDefault = value;
                    this.NotifyPropertyChanged(nameof(ResetDatesEnabled));
                }
            }
        }

        private Button resetdatesDefault { get; set; }
        public Button ResetDates
        {
            get
            {
                return resetdatesDefault;
            }
            set
            {
                if (resetdatesDefault != value)
                {
                    resetdatesDefault = value;
                    this.NotifyPropertyChanged(nameof(ResetDates));
                }
            }
        }

#if WINFORMS || WPF  || WINUI || SMARTMAUI
#if WINFORMS
        private DateTime _enddateDefault = SmartParametersV2016.defaultMaxdate;
        public DateTime EndDate
#endif
#if WPF
        // Careful!  These two were DateTimeOffset!!!
        private DateTime _enddateDefault = SmartParametersV2016.defaultMaxdate;
        public DateTime EndDate
#endif
#if WINUI
        private DateTime _enddateDefault = SmartParametersV2016.defaultMaxdate;
        public DateTime EndDate
#endif
#if SMARTMAUI
        private DateTime _enddateDefault = SmartParametersV2016.defaultMaxdate;
        public DateTime EndDate
#endif
        //#if ANDROIDX
        //        // Careful!  These two were DateTimeOffset!!!
        //        private DateTime _enddateDefault = SmartParametersV2016.defaultMaxdate;
        //        public DateTime EndDate
        //#endif
        {
            get
            {
                return _enddateDefault;
            }
            set
            {
                if (_enddateDefault != value)
                {
                    _enddateDefault = value;
                    this.NotifyPropertyChanged(nameof(EndDate));
                }
            }
        }
#endif

        private bool _enddateenabledDefault = false;
        public bool EndDateEnabled
        {
            get
            {
                return _enddateenabledDefault;
            }
            set
            {
                if (_enddateenabledDefault != value)
                {
                    _enddateenabledDefault = value;
                    this.NotifyPropertyChanged(nameof(EndDateEnabled));
                }
            }
        }

        // I don't know why we can't create a new one of these every time
        // This stuff is such a fucking bitch.  At least this method works
        //private string _utilitytext = "UtilityText";
        //public string UtilityText
        //{
        //    get
        //    {
        //        return _utilitytext;
        //    }
        //    set
        //    {
        //        if (_utilitytext != value)
        //        {
        //            _utilitytext = value;
        //            this.NotifyPropertyChanged(nameof(UtilityText));
        //        }
        //    }
        //}

        private string _name = "";
        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                if (_name != value)
                {
                    _name = value;
                    this.NotifyPropertyChanged(nameof(Name));
                }
            }
        }

#if WINFORMS
        //private Visibility _logincreate = Visibility.Hidden;
        //public Visibility LoginCreate
#endif
#if WPF
        private Visibility _logincreate = Visibility.Hidden;
        public Visibility LoginCreate
#endif
#if WINUI
        private Visibility _logincreate = Visibility.Collapsed;
        public Visibility LoginCreate
#endif
#if ANDROIDX
        private bool _logincreate = false;
        public bool LoginCreate
#endif
#if SMARTMAUI
        private bool _logincreate = false;
        public bool LoginCreate
#endif
#if WPF  || WINUI || ANDROIDX || SMARTMAUI
        {
            get
            {
                return _logincreate;
            }
            set
            {
                if (_logincreate != value)
                {
                    _logincreate = value;
                    this.NotifyPropertyChanged(nameof(LoginCreate));
                }
            }
        }
#endif

#if WINFORMS
        //private Visibility _loginamend = Visibility.Hidden;
        //public Visibility LoginAmend
#endif
#if WPF
        private Visibility _loginamend = Visibility.Hidden;
        public Visibility LoginAmend
#endif
#if WINUI
        private Visibility _loginamend = Visibility.Collapsed;
        public Visibility LoginAmend
#endif
#if ANDROIDX
        private bool _loginamend = false;
        public bool LoginAmend
#endif
#if SMARTMAUI
        private Visibility _loginamend = Visibility.Hidden;
        public Visibility LoginAmend
#endif
#if WPF  || WINUI || ANDROIDX || SMARTMAUI
        {
            get
            {
                return _loginamend;
            }
            set
            {
                if (_loginamend != value)
                {
                    _loginamend = value;
                    this.NotifyPropertyChanged(nameof(LoginAmend));
                }
            }
        }
#endif

        // Its not a rhododendron its a FUCKING HYDRANGEA for the 18th fucking time
#if WINFORMS
        //private Visibility _logindelete = Visibility.Hidden;
        //public Visibility LoginDelete
#endif
#if WPF
        private Visibility _logindelete = Visibility.Hidden;
        public Visibility LoginDelete
#endif
#if WINUI
        private Visibility _logindelete = Visibility.Collapsed;
        public Visibility LoginDelete
#endif
#if ANDROIDX
        private bool _logindelete = false;
        public bool LoginDelete
#endif
#if SMARTMAUI
        private Visibility _logindelete = Visibility.Hidden;
        public Visibility LoginDelete
#endif
#if WPF  || WINUI || ANDROIDX || SMARTMAUI
        {
            get
            {
                return _logindelete;
            }
            set
            {
                if (_logindelete != value)
                {
                    _logindelete = value;
                    this.NotifyPropertyChanged(nameof(LoginDelete));
                }
            }
        }
#endif

#if WPF
        private XmlLanguage _utilitylanguage;
        public XmlLanguage UtilityLanguage
#endif
#if WINUI
        private string _utilitylanguage = "";
        public string UtilityLanguage
#endif
#if ANDROIDX
        private string _utilitylanguage = "";
        public string UtilityLanguage
#endif
#if SMARTMAUI
        private string _utilitylanguage = "";
        public string UtilityLanguage
#endif
#if WPF  || WINUI || ANDROIDX || SMARTMAUI
        {
            get
            {
                return _utilitylanguage;
            }
            set
            {
                if (_utilitylanguage != value)
                {
                    _utilitylanguage = value;
                    this.NotifyPropertyChanged(nameof(UtilityLanguage));
                }
            }
        }
#endif
        private bool _utilityculturesenabled = false;
        public bool UtilityCulturesEnabled
        {
            get
            {
                return _utilityculturesenabled;
            }
            set
            {
                if (_utilityculturesenabled != value)
                {
                    _utilityculturesenabled = value;
                    this.NotifyPropertyChanged(nameof(UtilityCulturesEnabled));
                }
            }
        }

        private int utilitycultureselectedindex = -1;
        public int UtilityCultureSelectedIndex
        {
            get
            {
                return utilitycultureselectedindex;
            }
            set
            {
                if (utilitycultureselectedindex != value)
                {
                    utilitycultureselectedindex = value;
                    this.NotifyPropertyChanged(nameof(UtilityCultureSelectedIndex));
                }
            }
        }


        private bool _utilityconversionsenabled = false;
        public bool UtilityConversionsEnabled
        {
            get
            {
                return _utilityconversionsenabled;
            }
            set
            {
                if (_utilityconversionsenabled != value)
                {
                    _utilityconversionsenabled = value;
                    this.NotifyPropertyChanged(nameof(UtilityConversionsEnabled));
                }
            }
        }

#if WINFORMS || WPF  || WINUI || SMARTMAUI
        private bool _utilitynonchecked = false;
        public bool UtilityNONChecked
        {
            get
            {
                return _utilitynonchecked;
            }
            set
            {
                if (_utilitynonchecked != value)
                {
                    _utilitynonchecked = value;
                    this.NotifyPropertyChanged(nameof(UtilityNONChecked));
                }
            }
        }

        private bool _utilitygbpchecked = false;
        public bool UtilityGBPChecked
        {
            get
            {
                return _utilitygbpchecked;
            }
            set
            {
                if (_utilitygbpchecked != value)
                {
                    _utilitygbpchecked = value;
                    this.NotifyPropertyChanged(nameof(UtilityGBPChecked));
                }
            }
        }

        private bool _utilityeurchecked = false;
        public bool UtilityEURChecked
        {
            get
            {
                return _utilityeurchecked;
            }
            set
            {
                if (_utilityeurchecked != value)
                {
                    _utilityeurchecked = value;
                    this.NotifyPropertyChanged(nameof(UtilityEURChecked));
                }
            }
        }
        private bool _utilityusdchecked = false;
        public bool UtilityUSDChecked
        {
            get
            {
                return _utilityusdchecked;
            }
            set
            {
                if (_utilityusdchecked != value)
                {
                    _utilityusdchecked = value;
                    this.NotifyPropertyChanged(nameof(UtilityUSDChecked));
                }
            }
        }

        private bool _utilityjpychecked = false;
        public bool UtilityJPYChecked
        {
            get
            {
                return _utilityjpychecked;
            }
            set
            {
                if (_utilityjpychecked != value)
                {
                    _utilityjpychecked = value;
                    this.NotifyPropertyChanged(nameof(UtilityJPYChecked));
                }
            }
        }
#endif

#if WPF  || WINUI || SMARTMAUI
        public ImageSource electricbulb;
        public ImageSource ElectricLightBulb
        {
            get
            {
                return electricbulb;
            }
        }
#endif

#if WPF  || WINUI || SMARTMAUI
        public ImageSource gasflame;
        public ImageSource GasFlame
        {
            get
            {
                return gasflame;
            }
        }
#endif

        private string _accountname = "";
        public string AccountName
        {
            get
            {
                return _accountname;
            }
            set
            {
                if (_accountname != value && value.Length <= 35)
                {
                    _accountname = value;
                    this.NotifyPropertyChanged(nameof(AccountName));
                }
            }
        }

        private string _accountsortcode = "";
        public string AccountSortCode
        {
            get
            {
                return _accountsortcode;
            }
            set
            {
                if (_accountsortcode != value && value.Length <= 6)
                {
                    _accountsortcode = value;
                    this.NotifyPropertyChanged(nameof(AccountSortCode));
                }
            }
        }

        private string _accountnumber = "";
        public string AccountNumber
        {
            get
            {
                return _accountnumber;
            }
            set
            {
                if (_accountnumber != value && value.Length <= accountnumber_maxlength)
                {
                    _accountnumber = value;
                    this.NotifyPropertyChanged(nameof(AccountNumber));
                }
            }
        }

        private int _selectedtabindex = 0;
        public int SelectedTabIndex
        {
            get
            {
                return _selectedtabindex;
            }
            set
            {
                // For reasons unknown to me (..but perhaps because this entire XamarShit twaddle was written by fucking Chimps??)
                // The SelectedTabIndex doesn't reflect any changes when the tabs are scrolled through.
                // The BINDING works because the 'get' above works fine, its just the 'set' that doesn't work ...
                if (_selectedtabindex != value)
                {
                    _selectedtabindex = value;
                    this.NotifyPropertyChanged(nameof(SelectedTabIndex));
                }
            }
        }
#if ANDROIDX
        internal TextView NextConnectionMessage { get; set; }
#endif
#if WINFORMS || WPF  || WINUI || SMARTMAUI
        private string nextconnectionDefault = "                             ";
        public string NextConnectionMessage
        {
            get
            {
                return nextconnectionDefault;
            }
            set
            {
                if (nextconnectionDefault != value)
                {
                    nextconnectionDefault = value;
                    this.NotifyPropertyChanged(nameof(NextConnectionMessage));
                }
            }
        }
#endif

#if ANDROIDX
        private ImageView _arrowbutton;// = "expander_open.png";
        public ImageView ArrowButton
        {
            get
            {
                return _arrowbutton;
            }
            set
            {
                if (_arrowbutton != value)
                {
                    _arrowbutton = value;
                    this.NotifyPropertyChanged(nameof(ArrowButton));
                }
            }
        }
#endif
        public class SupplierItem
        {
            public string Content { get; set; } // Public for binding to work

            public int Supplier_ID { get; set; }

            public string Value { get; set; }
#if WINFORMS
            public Color Colour { get; set; }
#endif
#if WPF  || WINUI
            public Brush Colour { get; set; }       // So Binding works
#endif
#if ANDROIDX
            public Android.Graphics.Color Colour { get; set; }   // Leave as Public for binding
#endif
#if SMARTMAUI
            public Brush Colour { get; set; }
#endif
#if WINUI || SMARTMAUI
            public bool IsChecked { get; set; } // Never displayed
#endif
#if ANDROIDX
            public bool IsChecked { get; set; } // Never displayed
#endif
            public override string ToString()
            {
                return Content;
            }
        }

        public class TariffItem
        {
            public string Content { get; set; }
            public string Value { get; set; }
#if WINFORMS
            public System.Drawing.Color Colour { get; set; }
#endif
#if WPF  || WINUI
            public Brush Colour { get; set; }
#endif
#if ANDROIDX
            public Android.Graphics.Color Colour { get; set; }
#endif
#if SMARTMAUI
            public Color Colour { get; set; }
#endif
            public override string ToString()
            {
                return Content;
            }
        }

        public class PaymentPlanItem
        {
            public string Content { get; set; }
            public string Value { get; set; }
#if WINFORMS
            public System.Drawing.Color Colour { get; set; }
#endif
#if WPF  || WINUI
            public Brush Colour { get; set; }
#endif
#if ANDROIDX
            public Android.Graphics.Color Colour { get; set; }
#endif
#if SMARTMAUI
            public Color Colour { get; set; }
#endif
            public override string ToString()
            {
                return Content;
            }
        }

#if WINFORMS || WPF  || WINUI || SMARTMAUI
        private List<SupplierItem> _supplieritems = new List<SupplierItem>();
        public List<SupplierItem> UtilitySuppliersList
#endif
#if ANDROIDX
        private List<SupplierItem> _supplieritems = new List<SupplierItem>();
        public List<SupplierItem> UtilitySuppliersList
#endif
        {
            get
            {
                return _supplieritems;
            }
            set
            {
                if (_supplieritems != value)
                {
                    _supplieritems = value;
                    this.NotifyPropertyChanged(nameof(UtilitySuppliersList));
                }
            }
        }

        private dynamic[] _utilitysupplierselections = Array.Empty<dynamic>();
        public dynamic[] UtilitySupplier_Selections
        {
            get
            {
                return _utilitysupplierselections;
            }
            set
            {
                if (_utilitysupplierselections != value)
                {
                    _utilitysupplierselections = value;
                    this.NotifyPropertyChanged(nameof(UtilitySupplier_Selections));
                }
            }
        }

        private string _supplierstext = SmartParametersV2016.supplierprompt;
        public string UtilitySuppliers_Text
        {
            get
            {
                return _supplierstext;
            }
            set
            {
                if (_supplierstext != value)
                {
                    _supplierstext = value;
                    this.NotifyPropertyChanged(nameof(UtilitySuppliers_Text));
                }
            }
        }

#if WINFORMS
        private Brush _supplierscolour = new SolidBrush(Color.Transparent);
        public Brush UtilitySuppliersColour
#endif
#if WPF  || WINUI
        private Brush _supplierscolour = new SolidColorBrush(Colors.Transparent);
        public Brush UtilitySuppliersColour
#endif
#if ANDROIDX
        private Android.Graphics.Color _supplierscolour = Android.Graphics.Color.Transparent;
        public Android.Graphics.Color UtilitySuppliersColour
#endif
#if SMARTMAUI
        private Color _supplierscolour = Colors.Transparent;
        public Color UtilitySuppliersColour
#endif
        {
            get
            {
                return _supplierscolour;
            }
            set
            {
                if (_supplierscolour != value)
                {
                    _supplierscolour = value;
                    this.NotifyPropertyChanged(nameof(UtilitySuppliersColour));
                }
            }
        }

        private bool _supplierpopupvisible = false;
        public bool SupplierPopupVisible
        {
            get
            {
                return _supplierpopupvisible;
            }
            set
            {
                if (_supplierpopupvisible != value)
                {
                    _supplierpopupvisible = value;
                    this.NotifyPropertyChanged(nameof(SupplierPopupVisible));
                }
            }
        }

#if WINUI || SMARTMAUI
        private double _supplierwidth = 0;
        public double SupplierWidth
        {
            get
            {
                return _supplierwidth;
            }
            set
            {
                if (_supplierwidth != value)
                {
                    _supplierwidth = value;
                    this.NotifyPropertyChanged(nameof(SupplierWidth));
                }
            }
        }

        private double _supplier_translationx = 0;
        public double SupplierTranslationX
        {
            get
            {
                return _supplier_translationx;
            }
            set
            {
                if (_supplier_translationx != value)
                {
                    _supplier_translationx = value;
                    this.NotifyPropertyChanged(nameof(SupplierTranslationX));
                }
            }
        }

        private double _supplier_translationy = 0;
        public double SupplierTranslationY
        {
            get
            {
                return _supplier_translationy;
            }
            set
            {
                if (_supplier_translationy != value)
                {
                    _supplier_translationy = value;
                    this.NotifyPropertyChanged(nameof(SupplierTranslationY));
                }
            }
        }
#endif

#if ANDROIDX
        private double _supplierwidth = 0;
        public double SupplierWidth
        {
            get
            {
                return _supplierwidth;
            }
            set
            {
                if (_supplierwidth != value)
                {
                    _supplierwidth = value;
                    this.NotifyPropertyChanged(nameof(SupplierWidth));
                }
            }
        }

        private double _supplier_translationx = 0;
        public double SupplierTranslationX
        {
            get
            {
                return _supplier_translationx;
            }
            set
            {
                if (_supplier_translationx != value)
                {
                    _supplier_translationx = value;
                    this.NotifyPropertyChanged(nameof(SupplierTranslationX));
                }
            }
        }

        private double _supplier_translationy = 0;
        public double SupplierTranslationY
        {
            get
            {
                return _supplier_translationy;
            }
            set
            {
                if (_supplier_translationy != value)
                {
                    _supplier_translationy = value;
                    this.NotifyPropertyChanged(nameof(SupplierTranslationY));
                }
            }
        }
#endif
#if WINFORMS || WPF  || WINUI || SMARTMAUI
        private List<TariffItem> _tariffView = new List<TariffItem>();
        public List<TariffItem> UtilityTariffsList
#endif
#if ANDROIDX
        private List<TariffItem> _tariffView = new List<TariffItem>();
        public List<TariffItem> UtilityTariffsList
#endif
        {
            get
            {
                return _tariffView;
            }
            set
            {
                if (_tariffView != value)
                {
                    _tariffView = value;
                    this.NotifyPropertyChanged(nameof(UtilityTariffsList));
                }
            }
        }

#if WINFORMS || WPF  || WINUI || SMARTMAUI
        private List<PaymentPlanItem> _paymentplansView = new List<PaymentPlanItem>();
        public List<PaymentPlanItem> UtilityPaymentPlansList
#endif
#if ANDROIDX
        private List<PaymentPlanItem> _paymentplansView = new List<PaymentPlanItem>();
        public List<PaymentPlanItem> UtilityPaymentPlansList
#endif
        {
            get
            {
                return _paymentplansView;
            }
            set
            {
                if (_paymentplansView != value)
                {
                    _paymentplansView = value;
                    this.NotifyPropertyChanged(nameof(UtilityPaymentPlansList));
                }
            }
        }

#if WINFORMS || WPF  || WINUI || SMARTMAUI
        private List<MainViewModel.AddressItem> _addressesView = new List<MainViewModel.AddressItem>();
        public List<MainViewModel.AddressItem> UtilityAddressesList
#endif
#if ANDROIDX
        private List<MainViewModel.AddressItem> _addressesView = new List<MainViewModel.AddressItem>();
        public List<MainViewModel.AddressItem> UtilityAddressesList
#endif
        {
            get
            {
                return _addressesView;
            }
            set
            {
                if (_addressesView != value)
                {
                    _addressesView = value;
                    this.NotifyPropertyChanged(nameof(UtilityAddressesList));
                }
            }
        }

        private List<MainViewModel.CultureItem> culturesView = new List<MainViewModel.CultureItem>();
        public List<MainViewModel.CultureItem> UtilityCulturesList
        {
            get
            {
                return culturesView;
            }
            set
            {
                if (culturesView != value)
                {
                    culturesView = value;
                    this.NotifyPropertyChanged(nameof(UtilityCulturesList));
                }
            }
        }

#if ANDROIDX
        internal List<string> UtilitySpinnerAddressesList { get; set; }
#endif

        private int supplierselected = -1;
        public int SupplierSelectedIndex
        {
            get
            {
                return supplierselected;
            }
            set
            {
                if (supplierselected != value)
                {
                    supplierselected = value;
                    this.NotifyPropertyChanged(nameof(SupplierSelectedIndex));
                }
            }
        }

        private int tariffselected = -1;
        public int TariffSelectedIndex
        {
            get
            {
                return tariffselected;
            }
            set
            {
                if (tariffselected != value)
                {
                    tariffselected = value;
                    this.NotifyPropertyChanged(nameof(TariffSelectedIndex));
                }
            }
        }

        private int paymentplansselected = -1;
        public int PaymentPlanSelectedIndex
        {
            get
            {
                return paymentplansselected;
            }
            set
            {
                if (paymentplansselected != value)
                {
                    paymentplansselected = value;
                    this.NotifyPropertyChanged(nameof(PaymentPlanSelectedIndex));
                }
            }
        }

        private int addressselected = -1;
        public int AddressSelectedIndex
        {
            get
            {
                return addressselected;
            }
            set
            {
                if (addressselected != value)
                {
                    addressselected = value;
                    this.NotifyPropertyChanged(nameof(AddressSelectedIndex));
                }
            }
        }

        private bool suppliersenabledDefault = false;   // Grid of Picker initially turned ON
        public bool SuppliersEnabled
        {
            get
            {
                return suppliersenabledDefault;
            }
            set
            {
                if (suppliersenabledDefault != value)
                {
                    suppliersenabledDefault = value;
                    this.NotifyPropertyChanged(nameof(SuppliersEnabled));
                }
            }
        }

        private bool tariffsenabledDefault = false;   // Grid of Picker initially turned ON
        public bool TariffsEnabled
        {
            get
            {
                return tariffsenabledDefault;
            }
            set
            {
                if (tariffsenabledDefault != value)
                {
                    tariffsenabledDefault = value;
                    this.NotifyPropertyChanged(nameof(TariffsEnabled));
                }
            }
        }

        private bool paymentplansenabledDefault = false;   // Grid of Picker initially turned ON
        public bool PaymentPlansEnabled
        {
            get
            {
                return paymentplansenabledDefault;
            }
            set
            {
                if (paymentplansenabledDefault != value)
                {
                    paymentplansenabledDefault = value;
                    this.NotifyPropertyChanged(nameof(PaymentPlansEnabled));
                }
            }
        }

        private bool addressessenabledDefault = false;   // Frame of Picker initially turned ON
        public bool AddressesEnabled
        {
            get
            {
                return addressessenabledDefault;
            }
            set
            {
                if (addressessenabledDefault != value)
                {
                    addressessenabledDefault = value;
                    this.NotifyPropertyChanged(nameof(AddressesEnabled));
                }
            }
        }


#if WINFORMS
        private System.Drawing.Color _supplierscolorDefault = System.Drawing.Color.Transparent;
        public System.Drawing.Color UtilitySuppliersColor
#endif
#if WPF  || WINUI
        private Brush _supplierscolorDefault = new SolidColorBrush(Colors.Transparent);
        public Brush UtilitySuppliersColor
#endif
#if ANDROIDX
        private Android.Graphics.Color _supplierscolorDefault = Android.Graphics.Color.Transparent;
        public Android.Graphics.Color UtilitySuppliersColor
#endif
#if SMARTMAUI
        private Color _supplierscolorDefault = Colors.Transparent;
        public Color UtilitySuppliersColor
#endif
        {
            get
            {
                return _supplierscolorDefault;
            }
            set
            {
                if (_supplierscolorDefault != value)
                {

                    _supplierscolorDefault = value;
                    this.NotifyPropertyChanged(nameof(UtilitySuppliersColor));
                }
            }
        }

#if WINFORMS
        private System.Drawing.Color _tariffscolorDefault = System.Drawing.Color.Transparent;
        public System.Drawing.Color UtilityTariffsColor
#endif
#if WPF  || WINUI
        private Brush _tariffscolorDefault = new SolidColorBrush(Colors.Transparent);
        public Brush UtilityTariffsColor
#endif
#if ANDROIDX
        private Android.Graphics.Color _tariffscolorDefault = Android.Graphics.Color.Transparent;
        public Android.Graphics.Color UtilityTariffsColor
#endif
#if SMARTMAUI
        private Color _tariffscolorDefault = Colors.Transparent;
        public Color UtilityTariffsColor
#endif
        {
            get
            {
                return _tariffscolorDefault;
            }
            set
            {
                if (_tariffscolorDefault != value)
                {

                    _tariffscolorDefault = value;
                    this.NotifyPropertyChanged(nameof(UtilityTariffsColor));
                }
            }
        }

        // #if MEEOWI You simply COULDN'T make this bollocks up!!

#if WINFORMS
        private System.Drawing.Color _paymentplanscolorDefault = System.Drawing.Color.Transparent;
        public System.Drawing.Color UtilityPaymentPlansColor
#endif
#if WPF  || WINUI
        private Brush _paymentplanscolorDefault = new SolidColorBrush(Colors.Transparent);
        public Brush UtilityPaymentPlansColor
#endif
#if ANDROIDX
        private Android.Graphics.Color _paymentplanscolorDefault = Android.Graphics.Color.Transparent;
        public Android.Graphics.Color UtilityPaymentPlansColor
#endif
#if SMARTMAUI
        private Color _paymentplanscolorDefault = Colors.Transparent;
        public Color UtilityPaymentPlansColor
#endif
        {
            get
            {
                return _paymentplanscolorDefault;
            }
            set
            {
                if (_paymentplanscolorDefault != value)
                {

                    _paymentplanscolorDefault = value;
                    this.NotifyPropertyChanged(nameof(UtilityPaymentPlansColor));
                }
            }
        }

#if WINFORMS
        private System.Drawing.Color _addressescolorDefault = System.Drawing.Color.Transparent;
        public System.Drawing.Color UtilityAddressesColor
#endif
#if WPF  || WINUI
        private Brush _addressescolorDefault = new SolidColorBrush(Colors.Transparent);
        public Brush UtilityAddressesColor
#endif
#if ANDROIDX
        private Android.Graphics.Color _addressescolorDefault = Android.Graphics.Color.Transparent;
        public Android.Graphics.Color UtilityAddressesColor
#endif
#if SMARTMAUI
        private Color _addressescolorDefault = Colors.Transparent;
        public Color UtilityAddressesColor
#endif
        {
            get
            {
                return _addressescolorDefault;
            }
            set
            {
                if (_addressescolorDefault != value)
                {

                    _addressescolorDefault = value;
                    this.NotifyPropertyChanged(nameof(UtilityAddressesColor));
                }
            }
        }

        private int _costsposition = -1;
        public int UtilityCostsPosition
        {
            get
            {
                return _costsposition;
            }
            set
            {
                // Never seems to hit this for some reason ...??
                if (_costsposition != value)
                {
                    _costsposition = value;
                    this.NotifyPropertyChanged(nameof(UtilityCostsPosition));
                }
            }
        }

        private bool submitstatusDefault = false;
        public bool SubmitStatus
        {
            get
            {
                return submitstatusDefault;
            }
            set
            {
                if (submitstatusDefault != value)
                {
                    submitstatusDefault = value;
                    this.NotifyPropertyChanged(nameof(SubmitStatus));
                }

            }
        }

#if ANDROIDX
        private Button submitDefault { get; set; }
        public Button Submit
        {
            get
            {
                return submitDefault;
            }
            set
            {
                if (submitDefault != value)
                {
                    submitDefault = value;
                    this.NotifyPropertyChanged(nameof(Submit));
                }
            }
        }
#endif

        // The Cancel button works the opposite way to all the others
        private bool cancelstatusDefault = false;
        public bool CancelStatus
        {
            get
            {
                return cancelstatusDefault;
            }
            set
            {
                if (cancelstatusDefault != value)
                {
                    cancelstatusDefault = value;
                    this.NotifyPropertyChanged(nameof(CancelStatus));
                }
            }
        }

#if ANDROIDX
        private Button cancelDefault { get; set; }
        public Button Cancel

        {
            get
            {
                return cancelDefault;
            }
            set
            {
                if (cancelDefault != value)
                {
                    cancelDefault = value;
                    this.NotifyPropertyChanged(nameof(Cancel));
                }
            }
        }
#endif

        private bool utilityculturestatusDefault = false;
        public bool UtilityCultureStatus
        {
            get
            {
                return utilityculturestatusDefault;
            }
            set
            {
                if (utilityculturestatusDefault != value)
                {
                    utilityculturestatusDefault = value;
                    this.NotifyPropertyChanged(nameof(UtilityCultureStatus));
                }
            }
        }

#if WINFORMS
        private System.Drawing.Color utilityculturecolourDefault = System.Drawing.Color.Black;
        public System.Drawing.Color UtilityCultureColor
#endif
#if WPF  || WINUI
        private Brush utilityculturecolourDefault = new SolidColorBrush(Colors.Black);
        public Brush UtilityCultureColor
#endif
#if ANDROIDX
        private Android.Graphics.Color utilityculturecolourDefault = Android.Graphics.Color.Black;
        public Android.Graphics.Color UtilityCultureColor
#endif
#if SMARTMAUI
        private Color utilityculturecolourDefault = Colors.Black;
        public Color UtilityCultureColor
#endif
        {
            get
            {
                return utilityculturecolourDefault;
            }
            set
            {
                if (utilityculturecolourDefault != value)
                {
                    utilityculturecolourDefault = value;
                    this.NotifyPropertyChanged(nameof(UtilityCultureColor));
                }
            }
        }

        private string utilityculturecontentDefault = "Culture";
        public string UtilityCultureContent
        {
            get
            {
                return utilityculturecontentDefault;
            }
            set
            {
                if (utilityculturecontentDefault != value)
                {
                    utilityculturecontentDefault = value;
                    this.NotifyPropertyChanged(nameof(UtilityCultureContent));
                }
            }
        }

        private bool _resourcesenabled = false;
        public bool ResourcesEnabled
        {
            get
            {
                return _resourcesenabled;
            }
            set
            {
                if (_resourcesenabled != value)
                {
                    _resourcesenabled = value;
                    this.NotifyPropertyChanged(nameof(ResourcesEnabled));
                }
            }
        }

        private bool _electricitychecked = false;
        public bool ElectricityChecked
        {
            get
            {
                return _electricitychecked;
            }

            set
            {
                if (_electricitychecked != value)
                {
                    _electricitychecked = value;
                    this.NotifyPropertyChanged(nameof(ElectricityChecked));
                }
            }
        }

        private bool _gaschecked = false;
        public bool GasChecked
        {
            get
            {
                return _gaschecked;
            }
            set
            {
                if (_gaschecked != value)
                {
                    _gaschecked = value;
                    this.NotifyPropertyChanged(nameof(GasChecked));
                }
            }
        }

        private bool _dualfuelchecked = false;
        public bool DualFuelChecked
        {
            get
            {
                return _dualfuelchecked;
            }
            set
            {
                if (_dualfuelchecked != value)
                {
                    _dualfuelchecked = value;
                    this.NotifyPropertyChanged(nameof(DualFuelChecked));
                }
            }
        }

        public string ButtonGas
        {
            get
            {
                return SmartParametersV2016.Gas.ToString();
            }
        }

        public string ButtonElectricity // Public otherwise Tag doesn't get bound
        {
            get
            {
                return SmartParametersV2016.Electricity.ToString();
            }
        }

        public string ButtonDualFuel
        {
            get
            {
                return SmartParametersV2016.DualFuel.ToString();
            }
        }

        // This is the Day Rate LABEL
        private string dayrateDefault = "";
        public string DayRate
        {
            get
            {
                return dayrateDefault;
            }
            set
            {
                if (dayrateDefault != value)
                {
                    dayrateDefault = value;
                    this.NotifyPropertyChanged(nameof(DayRate));
                }
            }
        }

        // This is the Night Rate LABEL
        private string nightrateDefault = "";
        public string NightRate
        {
            get
            {
                return nightrateDefault;
            }
            set
            {
                if (nightrateDefault != value)
                {
                    nightrateDefault = value;
                    this.NotifyPropertyChanged(nameof(NightRate));
                }
            }
        }

        // This is the Standing Charge LABEL
        private string standingchargeDefault = "";
        public string StandingCharge
        {
            get
            {
                return standingchargeDefault;
            }
            set
            {
                if (standingchargeDefault != value)
                {
                    standingchargeDefault = value;
                    this.NotifyPropertyChanged(nameof(StandingCharge));
                }
            }
        }

#if WINFORMS || WPF  || WINUI || SMARTMAUI
        private string dayrate1Default = "";
        public string DayRate1
        {
            get
            {
                return dayrate1Default;
            }
            set
            {
                if (dayrate1Default != value)
                {
                    dayrate1Default = value;
                    this.NotifyPropertyChanged(nameof(DayRate1));
                }
            }
        }
#endif
#if ANDROIDX
        internal TextView DayRate1 { get; set; }
#endif

#if WINFORMS
        private Color dayrate1brushDefault = Color.Transparent;
        public Color DayRate1Brush
#endif
#if WPF  || WINUI
        private Brush dayrate1brushDefault = new SolidColorBrush(Colors.Transparent);
        public Brush DayRate1Brush
#endif
#if ANDROIDX
        private Android.Graphics.Color dayrate1brushDefault = Android.Graphics.Color.Transparent;
        public Android.Graphics.Color DayRate1Brush
#endif
#if SMARTMAUI
        private Color dayrate1brushDefault = Colors.Transparent;
        public Color DayRate1Brush
#endif
        {
            get
            {
                return dayrate1brushDefault;
            }
            set
            {
                if (dayrate1brushDefault != value)
                {
                    dayrate1brushDefault = value;
                    this.NotifyPropertyChanged(nameof(DayRate1Brush));
                }
            }
        }

#if WINFORMS || WPF  || WINUI || SMARTMAUI
        private string nightrate1Default = "";
        public string NightRate1
        {
            get
            {
                return nightrate1Default;
            }
            set
            {
                if (nightrate1Default != value)
                {
                    nightrate1Default = value;
                    this.NotifyPropertyChanged(nameof(NightRate1));
                }
            }
        }
#endif
#if ANDROIDX
        internal TextView NightRate1 { get; set; }
#endif

#if WINFORMS
        private Color nightrate1brushDefault = Color.Transparent;
        public Color NightRate1Brush
#endif
#if WPF  || WINUI
        private Brush nightrate1brushDefault = new SolidColorBrush(Colors.Transparent);
        public Brush NightRate1Brush
#endif
#if ANDROIDX
        private Android.Graphics.Color nightrate1brushDefault = Android.Graphics.Color.Transparent;
        public Android.Graphics.Color NightRate1Brush
#endif
#if SMARTMAUI
        private Color nightrate1brushDefault = Colors.Transparent;
        public Color NightRate1Brush
#endif
        {
            get
            {
                return nightrate1brushDefault;
            }
            set
            {
                if (nightrate1brushDefault != value)
                {
                    nightrate1brushDefault = value;
                    this.NotifyPropertyChanged(nameof(NightRate1Brush));
                }
            }
        }

#if WINFORMS || WPF  || WINUI || SMARTMAUI
        private string standingcharge1Default = "";
        public string StandingCharge1
        {
            get
            {
                return standingcharge1Default;
            }
            set
            {
                if (standingcharge1Default != value)
                {
                    standingcharge1Default = value;
                    this.NotifyPropertyChanged(nameof(StandingCharge1));
                }
            }
        }
#endif
#if ANDROIDX
        internal TextView StandingCharge1 { get; set; }
#endif
#if WINFORMS
        private Color standingcharge1brushDefault = Color.Transparent;
        public Color StandingCharge1Brush
#endif
#if WPF  || WINUI
        private Brush standingcharge1brushDefault = new SolidColorBrush(Colors.Transparent);
        public Brush StandingCharge1Brush
#endif
#if ANDROIDX
        private Android.Graphics.Color standingcharge1brushDefault = Android.Graphics.Color.Transparent;
        public Android.Graphics.Color StandingCharge1Brush
#endif
#if SMARTMAUI
        private Color standingcharge1brushDefault = Colors.Transparent;
        public Color StandingCharge1Brush
#endif
        {
            get
            {
                return standingcharge1brushDefault;
            }
            set
            {
                if (standingcharge1brushDefault != value)
                {
                    standingcharge1brushDefault = value;
                    this.NotifyPropertyChanged(nameof(StandingCharge1Brush));
                }
            }
        }

#if WINFORMS || WPF  || WINUI || SMARTMAUI
        private string dayrate2Default = "";
        public string DayRate2
        {
            get
            {
                return dayrate2Default;
            }
            set
            {
                if (dayrate2Default != value)
                {
                    dayrate2Default = value;
                    this.NotifyPropertyChanged(nameof(DayRate2));
                }
            }
        }
#endif
#if ANDROIDX
        internal TextView DayRate2 { get; set; }
#endif
#if WINFORMS
        private Color dayrate2brushDefault = Color.Transparent;
        public Color DayRate2Brush
#endif
#if WPF  || WINUI
        private Brush dayrate2brushDefault = new SolidColorBrush(Colors.Transparent);
        public Brush DayRate2Brush
#endif
#if ANDROIDX
        private Android.Graphics.Color dayrate2brushDefault = Android.Graphics.Color.Transparent;
        public Android.Graphics.Color DayRate2Brush
#endif
#if SMARTMAUI
        private Color dayrate2brushDefault = Colors.Transparent;
        public Color DayRate2Brush
#endif
        {
            get
            {
                return dayrate2brushDefault;
            }
            set
            {
                if (dayrate2brushDefault != value)
                {
                    dayrate2brushDefault = value;
                    this.NotifyPropertyChanged(nameof(DayRate2Brush));
                }
            }
        }

#if WINFORMS || WPF  || WINUI || SMARTMAUI
        private string nightrate2Default = "";
        public string NightRate2
        {
            get
            {
                return nightrate2Default;
            }
            set
            {
                if (nightrate2Default != value)
                {
                    nightrate2Default = value;
                    this.NotifyPropertyChanged(nameof(NightRate2));
                }
            }
        }
#endif
#if ANDROIDX
        internal TextView NightRate2 { get; set; }
#endif
#if WINFORMS
        private Color nightrate2brushDefault = Color.Transparent;
        public Color NightRate2Brush
#endif
#if WPF  || WINUI
        private Brush nightrate2brushDefault = new SolidColorBrush(Colors.Transparent);
        public Brush NightRate2Brush
#endif
#if ANDROIDX
        private Android.Graphics.Color nightrate2brushDefault = Android.Graphics.Color.Transparent;
        public Android.Graphics.Color NightRate2Brush
#endif
#if SMARTMAUI
        private Color nightrate2brushDefault = Colors.Transparent;
        public Color NightRate2Brush
#endif
        {
            get
            {
                return nightrate2brushDefault;
            }
            set
            {
                if (nightrate2brushDefault != value)
                {
                    nightrate2brushDefault = value;
                    this.NotifyPropertyChanged(nameof(NightRate2Brush));
                }
            }
        }

#if WINFORMS || WPF  || WINUI || SMARTMAUI
        private string standingcharge2Default = "";
        public string StandingCharge2
        {
            get
            {
                return standingcharge2Default;
            }
            set
            {
                if (standingcharge2Default != value)
                {
                    standingcharge2Default = value;
                    this.NotifyPropertyChanged(nameof(StandingCharge2));
                }
            }
        }
#endif
#if ANDROIDX
        internal TextView StandingCharge2 { get; set; }
#endif

#if WINFORMS
        private Color standingcharge2brushDefault = Color.Transparent;
        public Color StandingCharge2Brush
#endif
#if WPF  || WINUI
        private Brush standingcharge2brushDefault = new SolidColorBrush(Colors.Transparent);
        public Brush StandingCharge2Brush
#endif
#if ANDROIDX
        private Android.Graphics.Color standingcharge2brushDefault = Android.Graphics.Color.Transparent;
        public Android.Graphics.Color StandingCharge2Brush
#endif
#if SMARTMAUI
        private Color standingcharge2brushDefault = Colors.Transparent;
        public Color StandingCharge2Brush
#endif
        {
            get
            {
                return standingcharge2brushDefault;
            }
            set
            {
                if (standingcharge2brushDefault != value)
                {
                    standingcharge2brushDefault = value;
                    this.NotifyPropertyChanged(nameof(StandingCharge2Brush));
                }
            }
        }

#if WINFORMS || WPF  || WINUI || SMARTMAUI
        private string areaidDefault = "";
        public string AreaId
        {
            get
            {
                return areaidDefault;
            }
            set
            {
                if (areaidDefault != value)
                {
                    areaidDefault = value;
                    this.NotifyPropertyChanged(nameof(AreaId));
                }
            }
        }
#endif

#if ANDROIDX
        internal TextView AreaId { get; set; }
#endif

#if WINFORMS || WPF  || WINUI || SMARTMAUI
        private string postcodeDefault = "";
        public string PostCode
        {
            get
            {
                return postcodeDefault;
            }
            set
            {
                if (postcodeDefault != value)
                {
                    postcodeDefault = value;
                    this.NotifyPropertyChanged(nameof(PostCode));
                }
            }
        }
#endif

#if ANDROIDX
        internal TextView PostCode { get; set; }
#endif

#if WINFORMS
        private Color suppliersborderbrushDefault = Color.Transparent;
        public Color SuppliersBorderBrush
#endif
#if WPF  || WINUI
        private Brush suppliersborderbrushDefault = new SolidColorBrush(Colors.Transparent);
        public Brush SuppliersBorderBrush
#endif
#if ANDROIDX
        private Android.Graphics.Color suppliersborderbrushDefault = Android.Graphics.Color.Transparent;
        public Android.Graphics.Color SuppliersBorderBrush
#endif
#if SMARTMAUI
        private Color suppliersborderbrushDefault = Colors.Transparent;
        public Color SuppliersBorderBrush
#endif
        {
            get
            {
                return suppliersborderbrushDefault;
            }
            set
            {
                if (suppliersborderbrushDefault != value)
                {
                    suppliersborderbrushDefault = value;
                    this.NotifyPropertyChanged(nameof(SuppliersBorderBrush));
                }
                ;
            }
        }

#if WINFORMS
        private Color suppliersbordercolorDefault = Color.Transparent;
        public Color SuppliersBorderColor
#endif
#if WPF  || WINUI
        private Brush suppliersbordercolorDefault = new SolidColorBrush(Colors.Transparent);
        public Brush SuppliersBorderColor
#endif
#if ANDROIDX
        private Android.Graphics.Color suppliersbordercolorDefault = Android.Graphics.Color.Transparent;
        public Android.Graphics.Color SuppliersBorderColor
#endif
#if SMARTMAUI
        private Color suppliersbordercolorDefault = Colors.Transparent;
        public Color SuppliersBorderColor
#endif
        {
            get
            {
                return suppliersbordercolorDefault;
            }
            set
            {
                if (suppliersbordercolorDefault != value)
                {
                    suppliersbordercolorDefault = value;
                    this.NotifyPropertyChanged(nameof(SuppliersBorderColor));
                }
            }
        }

#if WINFORMS
        private bool suppliersborderthicknessDefault;
        public bool SuppliersBorderThickness
#endif
#if WPF  || WINUI || SMARTMAUI
        private Thickness suppliersborderthicknessDefault;
        public Thickness SuppliersBorderThickness
#endif
#if WINFORMS || WPF  || WINUI || SMARTMAUI
        {
            get
            {
                return suppliersborderthicknessDefault;
            }
            set
            {
                if (suppliersborderthicknessDefault != value)
                {
                    suppliersborderthicknessDefault = value;
                    this.NotifyPropertyChanged(nameof(SuppliersBorderThickness));
                }
            }
        }
#endif

        //#if WINFORMS
        //        internal string Password { get; set; }
        //#endif
        //#if WPF
        //        private Func<string> _abc;
        //        public Func<string> PasswordHandler
        //        {
        //            get
        //            {
        //                return _abc;
        //            }
        //            set
        //            {
        //                if (_abc != value)
        //                {
        //                    _abc = value;
        //                    this.NotifyPropertyChanged(nameof(PasswordHandler));
        //                }
        //            }
        //        }
        //#endif

        private string _userid = "";
        public string UserId
        {
            get
            {
                return _userid;
            }
            set
            {
                if (_userid != value)
                {
                    _userid = value;
                    this.NotifyPropertyChanged(nameof(UserId));
                }
            }
        }

        private string _password = "";
        public string Password
        {
            get
            {
                return _password;
            }
            set
            {
                if (_password != value)
                {
                    _password = value;
                    this.NotifyPropertyChanged(nameof(Password));
                }
            }
        }

#if WINFORMS || WPF  || WINUI || SMARTMAUI
        private string accountnoDefault = "";
        public string AccountNo
        {
            get
            {
                return accountnoDefault;
            }
            set
            {
                if (accountnoDefault != value)
                {
                    accountnoDefault = value;
                    this.NotifyPropertyChanged(nameof(AccountNo));
                }
            }
        }
#endif

#if ANDROIDX
        internal TextView AccountNo { get; set; }
#endif
#if WINFORMS
        private Color passwordbordercolorDefault = Color.Transparent;
        public Color PasswordBorderColor
#endif
#if WPF  || WINUI
        private Brush passwordbordercolorDefault = new SolidColorBrush(Colors.Transparent);
        public Brush PasswordBorderColor
#endif
#if ANDROIDX
        private Android.Graphics.Color passwordbordercolorDefault = Android.Graphics.Color.Transparent;
        public Android.Graphics.Color PasswordBorderColor
#endif
#if SMARTMAUI
        private Color passwordbordercolorDefault = Colors.Transparent;
        public Color PasswordBorderColor
#endif
        {
            get
            {
                return passwordbordercolorDefault;
            }
            set
            {
                if (passwordbordercolorDefault != value)
                {
                    passwordbordercolorDefault = value;
                    this.NotifyPropertyChanged(nameof(PasswordBorderColor));
                }
            }
        }

#if WINFORMS
        private bool passwordborderthicknessDefault;
        public bool PasswordBorderThickness
#endif
#if WPF  || WINUI || SMARTMAUI
        private Thickness passwordborderthicknessDefault;
        public Thickness PasswordBorderThickness
#endif
#if WINFORMS || WPF  || WINUI || SMARTMAUI
        {
            get
            {
                return passwordborderthicknessDefault;
            }
            set
            {
                if (passwordborderthicknessDefault != value)
                {
                    passwordborderthicknessDefault = value;
                    this.NotifyPropertyChanged(nameof(PasswordBorderThickness));
                }
                ;
            }
        }
#endif

#if WINFORMS
        private bool ledswitchedDefault = false;
        public bool LedSwitched
#endif
#if WPF
        private Visibility ledswitchedDefault = Visibility.Hidden;
        public Visibility LedSwitched
#endif
#if WINUI || SMARTMAUI
        private Visibility ledswitchedDefault = Visibility.Collapsed;
        public Visibility LedSwitched
#endif
#if WINFORMS || WPF  || WINUI || SMARTMAUI
        {
            get
            {
                return ledswitchedDefault;
            }
            set
            {
                if (ledswitchedDefault != value)
                {
                    ledswitchedDefault = value;
                    this.NotifyPropertyChanged(nameof(LedSwitched));
                }
            }
        }
#endif

#if ANDROIDX
        internal TextView LedSwitched { get; set; }
#endif

#if WINFORMS
        private bool economy7Default = false;
        public bool Economy7State
#endif
#if WPF
        private Visibility economy7Default = Visibility.Hidden;
        public Visibility Economy7State
#endif
#if WINUI || SMARTMAUI
        private Visibility economy7Default = Visibility.Collapsed;
        public Visibility Economy7State
#endif
#if ANDROIDX
        private bool economy7Default = false;  // <= There is no 'Hidden'
        public bool Economy7State
#endif
        {
            get
            {
                return economy7Default;
            }
            set
            {
                if (economy7Default != value)
                {
                    economy7Default = value;
                    this.NotifyPropertyChanged(nameof(Economy7State));
                }
            }
        }

#if WINFORMS || WPF  || WINUI || SMARTMAUI
        private bool checkbox60Default = false;
        public bool CheckBox60
        {
            get
            {
                return checkbox60Default;
            }
            set
            {
                if (checkbox60Default != value)
                {
                    checkbox60Default = value;
                    this.NotifyPropertyChanged(nameof(CheckBox60));
                }
            }
        }
#endif
#if ANDROIDX
        internal CheckBox CheckBox60 { get; set; }
#endif

        private bool checkboxsmoothDefault = false;
        public bool CheckBoxSmooth
        {
            get
            {
                return checkboxsmoothDefault;
            }
            set
            {
                if (checkboxsmoothDefault != value)
                {
                    checkboxsmoothDefault = value;
                    this.NotifyPropertyChanged(nameof(CheckBoxSmooth));
                }
            }
        }

#if WINFORMS
        private Image pictureboxDefault; //"keasdon_energy_small.jpg";
        public Image PictureBoxDNO
#endif
#if WPF  || WINUI || SMARTMAUI
        private ImageSource pictureboxDefault;
        public ImageSource PictureBoxDNO
#endif
#if WINFORMS || WPF  || WINUI || SMARTMAUI
        {
            get
            {
                return pictureboxDefault;
            }
            set
            {
                if (pictureboxDefault != value)
                {
                    pictureboxDefault = value;
                    this.NotifyPropertyChanged(nameof(PictureBoxDNO));
                }
            }
        }
#endif
#if ANDROIDX
        private ImageView pictureboxDnoDefault; // Needs context
        public ImageView PictureBoxDNO
        {
            get
            {
                return pictureboxDnoDefault;
            }
            set
            {
                if (pictureboxDnoDefault != value)
                {
                    pictureboxDnoDefault = value;
                    this.NotifyPropertyChanged(nameof(PictureBoxDNO));
                }
            }
        }
#endif

        private List<SmartUtility.AnalysisCostsView> utilitycostsDefault = new List<SmartUtility.AnalysisCostsView>();
        public List<SmartUtility.AnalysisCostsView> UtilityCosts
        {
            get
            {
                return utilitycostsDefault;
            }
            set
            {
                utilitycostsDefault = value;
                this.NotifyPropertyChanged(nameof(UtilityCosts));
            }
        }

        private List<SmartUtility.AnalysisBillsView> utilitybillsDefault = new List<SmartUtility.AnalysisBillsView>();
        public List<SmartUtility.AnalysisBillsView> UtilityBills
        {
            get
            {
                return utilitybillsDefault;
            }
            set
            {
                utilitybillsDefault = value;
                this.NotifyPropertyChanged(nameof(UtilityBills));
            }
        }

        private List<SmartUtility.AnalysisReadingsView> utilityreadingsDefault = new List<SmartUtility.AnalysisReadingsView>();
        public List<SmartUtility.AnalysisReadingsView> UtilityReadings
        {
            get
            {
                return utilityreadingsDefault;
            }
            set
            {
                utilityreadingsDefault = value;
                this.NotifyPropertyChanged(nameof(UtilityReadings));
            }
        }

        private List<SmartUtility.AnalysisBreakdownView> utilitybreakdownDefault = new List<SmartUtility.AnalysisBreakdownView>();
        public List<SmartUtility.AnalysisBreakdownView> UtilityBreakdown
        {
            get
            {
                return utilitybreakdownDefault;
            }
            set
            {
                utilitybreakdownDefault = value;
                this.NotifyPropertyChanged(nameof(UtilityBreakdown));
            }
        }

#if ANDROIDX
        internal TextView TotalkWh { get; set; }
#endif
#if WINFORMS || WPF  || WINUI || SMARTMAUI
        private string totalkwhDefault = "";
        public string TotalkWh
        {
            get
            {
                return totalkwhDefault;
            }
            set
            {
                if (totalkwhDefault != value)
                {
                    totalkwhDefault = value;
                    this.NotifyPropertyChanged(nameof(TotalkWh));
                }
            }
        }
#endif

#if ANDROIDX
        internal TextView TotalBilled { get; set; }
#endif
#if WINFORMS || WPF  || WINUI || SMARTMAUI
        private string totalbilledDefault = "";
        public string TotalBilled
        {
            get
            {
                return totalbilledDefault;
            }
            set
            {
                if (totalbilledDefault != value)
                {
                    totalbilledDefault = value;
                    this.NotifyPropertyChanged(nameof(TotalBilled));
                }
            }
        }
#endif

#if ANDROIDX
        internal TextView ProjectedCost { get; set; }
#endif
#if WINFORMS || WPF  || WINUI || SMARTMAUI
        private string _projectedcost = "";
        public string ProjectedCost
        {
            get
            {
                return _projectedcost;
            }
            set
            {
                if (_projectedcost != value)
                {
                    _projectedcost = value;
                    this.NotifyPropertyChanged(nameof(ProjectedCost));
                }
            }
        }
#endif

#if WINFORMS || WPF  || WINUI || SMARTMAUI
        private string totalcostDefault = "";
        public string TotalCost
        {
            get
            {
                return totalcostDefault;
            }
            set
            {
                if (totalcostDefault != value)
                {
                    totalcostDefault = value;
                    this.NotifyPropertyChanged(nameof(TotalCost));
                }
            }
        }
#endif

#if ANDROIDX
        internal TextView TotalCost { get; set; }
#endif
#if WINFORMS || WPF  || WINUI || SMARTMAUI
        private string totalreadingsDefault = "";
        public string TotalReadings
        {
            get
            {
                return totalreadingsDefault;
            }
            set
            {
                if (totalreadingsDefault != value)
                {
                    totalreadingsDefault = value;
                    NotifyPropertyChanged(nameof(TotalReadings));
                }
            }
        }
#endif
#if ANDROIDX
        internal TextView TotalReadings { get; set; }
#endif
#if WINFORMS
        private Color areaidbackgroundDefault = Color.Transparent;
        public Color AreaIdBackground
#endif
#if WPF  || WINUI
        private Brush areaidbackgroundDefault = new SolidColorBrush(Colors.Transparent);
        public Brush AreaIdBackground
#endif
#if ANDROIDX
        private Android.Graphics.Color areaidbackgroundDefault = Android.Graphics.Color.Transparent;
        public Android.Graphics.Color AreaIdBackground
#endif
#if SMARTMAUI
        private Color areaidbackgroundDefault = Colors.Transparent;
        public Color AreaIdBackground
#endif
        {
            get
            {
                return areaidbackgroundDefault;
            }
            set
            {
                if (areaidbackgroundDefault != value)
                {
                    areaidbackgroundDefault = value;
                    this.NotifyPropertyChanged(nameof(AreaIdBackground));
                }
            }
        }

#if WINFORMS
        private Color accountnobackgroundDefault = Color.Transparent;
        public Color AccountNoBackground
#endif
#if WPF  || WINUI
        private Brush accountnobackgroundDefault = new SolidColorBrush(Colors.Transparent);
        public Brush AccountNoBackground
#endif
#if ANDROIDX
        private Android.Graphics.Color accountnobackgroundDefault = Android.Graphics.Color.Transparent;
        public Android.Graphics.Color AccountNoBackground
#endif
#if SMARTMAUI
        private Color accountnobackgroundDefault = Colors.Transparent;
        public Color AccountNoBackground
#endif
        {
            get
            {
                return accountnobackgroundDefault;
            }
            set
            {
                if (accountnobackgroundDefault != value)
                {
                    accountnobackgroundDefault = value;
                    this.NotifyPropertyChanged(nameof(AccountNoBackground));
                }
            }
        }

#if WINFORMS
        private Color postcodebackgroundDefault = Color.Transparent;
        public Color PostCodeBackground
#endif
#if WPF  || WINUI
        private Brush postcodebackgroundDefault = new SolidColorBrush(Colors.Transparent);
        public Brush PostCodeBackground
#endif
#if ANDROIDX
        private Android.Graphics.Color postcodebackgroundDefault = Android.Graphics.Color.Transparent;
        public Android.Graphics.Color PostCodeBackground
#endif
#if SMARTMAUI
        private Color postcodebackgroundDefault = Colors.Transparent;
        public Color PostCodeBackground
#endif
        {
            get
            {
                return postcodebackgroundDefault;
            }
            set
            {
                if (postcodebackgroundDefault != value)
                {
                    postcodebackgroundDefault = value;
                    this.NotifyPropertyChanged(nameof(PostCodeBackground));
                }
            }
        }

#if WINFORMS
        private Color datetimestartbrushDefault = Color.Transparent;
        public Color DateTimeStartBrush
#endif
#if WPF  || WINUI
        private Brush datetimestartbrushDefault = new SolidColorBrush(Colors.Transparent);
        public Brush DateTimeStartBrush
#endif
#if ANDROIDX
        private Android.Graphics.Color datetimestartbrushDefault = Android.Graphics.Color.Transparent;
        public Android.Graphics.Color DateTimeStartBrush
#endif
#if SMARTMAUI
        private Color datetimestartbrushDefault = Colors.Transparent;
        public Color DateTimeStartBrush
#endif
        {
            get
            {
                return datetimestartbrushDefault;
            }
            set
            {
                if (datetimestartbrushDefault != value)
                {

                    datetimestartbrushDefault = value;
                    this.NotifyPropertyChanged(nameof(DateTimeStartBrush));
                }
            }
        }

#if WINFORMS
        private Color datetimeendbrushDefault = Color.Transparent;
        public Color DateTimeEndBrush
#endif
#if WPF  || WINUI
        private Brush datetimeendbrushDefault = new SolidColorBrush(Colors.Transparent);
        public Brush DateTimeEndBrush
#endif
#if ANDROIDX
        private Android.Graphics.Color datetimeendbrushDefault = Android.Graphics.Color.Transparent;
        public Android.Graphics.Color DateTimeEndBrush
#endif
#if SMARTMAUI
        private Color datetimeendbrushDefault = Colors.Transparent;
        public Color DateTimeEndBrush
#endif
        {
            get
            {
                return datetimeendbrushDefault;
            }
            set
            {
                if (datetimeendbrushDefault != value)
                {

                    datetimeendbrushDefault = value;
                    this.NotifyPropertyChanged(nameof(DateTimeEndBrush));
                }
            }
        }

        private string cheapesttitleDefault = "";
        public string CheapestTitle
        {
            get
            {
                return cheapesttitleDefault;
            }
            set
            {
                if (cheapesttitleDefault != value)
                {

                    cheapesttitleDefault = value;
                    this.NotifyPropertyChanged(nameof(CheapestTitle));
                }
            }
        }

        private string dearesttitleDefault = "";
        public string DearestTitle
        {
            get
            {
                return dearesttitleDefault;
            }
            set
            {
                if (dearesttitleDefault != value)
                {

                    dearesttitleDefault = value;
                    this.NotifyPropertyChanged(nameof(DearestTitle));
                }
            }
        }

        private string ourtitleDefault = "";
        public string OurTitle
        {
            get
            {
                return ourtitleDefault;
            }
            set
            {
                if (ourtitleDefault != value)
                {

                    ourtitleDefault = value;
                    this.NotifyPropertyChanged(nameof(OurTitle));
                }
            }
        }

        private List<KeyValuePair<DateTime, double>> columnseriesDefault = new List<KeyValuePair<DateTime, double>>();
        public List<KeyValuePair<DateTime, double>> ColumnSeriesData
        {
            get
            {
                return columnseriesDefault;
            }
            set
            {
                if (columnseriesDefault != value)
                {

                    columnseriesDefault = value;
                    this.NotifyPropertyChanged(nameof(ColumnSeriesData));
                }
            }
        }

        private List<KeyValuePair<DateTime, KeyValuePair<decimal, string>>> usageseriesDefault = new List<KeyValuePair<DateTime, KeyValuePair<decimal, string>>>();
        public List<KeyValuePair<DateTime, KeyValuePair<decimal, string>>> UsageSeriesData
        {
            get
            {
                return usageseriesDefault;
            }
            set
            {
                if (usageseriesDefault != value)
                {

                    usageseriesDefault = value;
                    this.NotifyPropertyChanged(nameof(UsageSeriesData));
                }
            }
        }

        private List<KeyValuePair<DateTime, double>> sinewaveseriesDefault = new List<KeyValuePair<DateTime, double>>();
        public List<KeyValuePair<DateTime, double>> SineWaveSeriesData
        {
            get
            {
                return sinewaveseriesDefault;
            }
            set
            {
                if (sinewaveseriesDefault != value)
                {

                    sinewaveseriesDefault = value;
                    this.NotifyPropertyChanged(nameof(SineWaveSeriesData));
                }
            }
        }

#if WINFORMS
        internal System.Windows.Forms.ToolTip SubmitToolTip { get; set; }
        internal System.Windows.Forms.ToolTip CancelToolTip { get; set; }
        internal System.Windows.Forms.ToolTip SuppliersToolTip { get; set; }
        internal System.Windows.Forms.ToolTip UserIdToolTip { get; set; }
        internal System.Windows.Forms.ToolTip PasswordToolTip { get; set; }
        internal System.Windows.Forms.ToolTip DualFuelToolTip { get; set; }
        internal System.Windows.Forms.ToolTip Age60ToolTip { get; set; }
        internal System.Windows.Forms.ToolTip SmoothToolTip { get; set; }
#endif

        // Nicola Surgeon !!!
        // encrouch <= enCROACH you dumb fucker
        internal string plot_code = "";

        public double PopupHeight { get; set; }
        public double PopupWidth { get; set; }

#if SMARTMAUI

        public ISeries[] ChartSeries { get; set; }

        //public Axis[] XAxes { get; set; }

        //public Axis[] YAxes { get; set; }

        public Axis[] CostsXAxes { get; set; }
        public Axis[] CostsYAxes { get; set; }

        public Axis[] ReadingsXAxes { get; set; }
        public Axis[] ReadingsYAxes { get; set; }

        public Axis[] UsageXAxes { get; set; }
        public Axis[] UsageYAxes { get; set; }

        private ISeries[] chartSeries1;
        public ISeries[] ChartSeries1
        {
            get => chartSeries1;
            set
            {
                chartSeries1 = value;
                NotifyPropertyChanged(nameof(ChartSeries1));
            }
        }
        private ISeries[] chartSeries2;
        public ISeries[] ChartSeries2
        {
            get => chartSeries2;
            set
            {
                chartSeries2 = value;
                NotifyPropertyChanged(nameof(ChartSeries2));
            }
        }
        private ISeries[] chartSeries3;
        public ISeries[] ChartSeries3
        {
            get => chartSeries3;
            set
            {
                chartSeries3 = value;
                NotifyPropertyChanged(nameof(ChartSeries3));
            }
        }

        private bool sideViewVisible = true;
        public bool SideViewVisible
        {
            get => sideViewVisible;
            set
            {
                if (sideViewVisible != value)
                {
                    sideViewVisible = value;
                    NotifyPropertyChanged(nameof(SideViewVisible));
                }
            }
        }
        private bool costsdefault = false;
        public bool CostsVisible
        {
            get
            {
                return costsdefault;
            }
            set
            {
                if (costsdefault != value)
                {
                    costsdefault = value;
                    NotifyPropertyChanged(nameof(CostsVisible));
                }
            }
        }

        private bool billsdefault = false;
        public bool BillsVisible
        {
            get
            {
                return billsdefault;
            }
            set
            {
                if (billsdefault != value)
                {
                    billsdefault = value;
                    NotifyPropertyChanged(nameof(BillsVisible));
                }
            }
        }
        private bool readingsdefault = false;
        public bool ReadingsVisible
        {
            get
            {
                return readingsdefault;
            }
            set
            {
                if (readingsdefault != value)
                {
                    readingsdefault = value;
                    NotifyPropertyChanged(nameof(ReadingsVisible));
                }
            }
        }
        private bool breakdowndefault = false;
        public bool BreakdownVisible
        {
            get
            {
                return breakdowndefault;
            }
            set
            {
                if (breakdowndefault != value)
                {
                    breakdowndefault = value;
                    NotifyPropertyChanged(nameof(BreakdownVisible));
                }
            }
        }

        private bool chartsVisible;
        public bool ChartsVisible
        {
            get => chartsVisible;
            set
            {
                if (chartsVisible != value)
                {
                    chartsVisible = value;
                    NotifyPropertyChanged(nameof(ChartsVisible));
                }
            }
        }
        private bool costsgraphsdefault = false;
        public bool CostsGraphsVisible
        {
            get
            {
                return costsgraphsdefault;
            }
            set
            {
                if (costsgraphsdefault != value)
                {
                    costsgraphsdefault = value;
                    NotifyPropertyChanged(nameof(CostsGraphsVisible));
                }
            }
        }

        private bool readingsgraphsdefault = false;
        public bool ReadingsGraphsVisible
        {
            get
            {
                return readingsgraphsdefault;
            }
            set
            {
                if (readingsgraphsdefault != value)
                {
                    readingsgraphsdefault = value;
                    NotifyPropertyChanged(nameof(ReadingsGraphsVisible));
                }
            }
        }

        private bool usagegraphsdefault = false;
        public bool UsageGraphsVisible
        {
            get
            {
                return usagegraphsdefault;
            }
            set
            {
                if (usagegraphsdefault != value)
                {
                    usagegraphsdefault = value;
                    NotifyPropertyChanged(nameof(UsageGraphsVisible));
                }
            }
        }
#endif

#if WINFORMS || WPF  || WINUI || ANDROIDX
        private PlotModel plotModel1;
        public PlotModel PlotModel1
        {
            get => plotModel1;
            set
            {
                if (plotModel1 != value)
                {
                    plotModel1 = value;
                    NotifyPropertyChanged(nameof(PlotModel1));
                }
            }
        }

        private PlotModel plotModel2;
        public PlotModel PlotModel2
        {
            get => plotModel2;
            set
            {
                if (plotModel2 != value)
                {
                    plotModel2 = value;
                    NotifyPropertyChanged(nameof(PlotModel2));
                }
            }
        }

        private PlotModel plotModel3;
        public PlotModel PlotModel3
        {
            get => plotModel3;
            set
            {
                if (plotModel3 != value)
                {
                    plotModel3 = value;
                    NotifyPropertyChanged(nameof(PlotModel3));
                }
            }
        }
#endif
        private string _utilitychartstitle = "";
        public string UtilityChartsTitle
        {
            get
            {
                return _utilitychartstitle;
            }
            set
            {
                if (_utilitychartstitle != value)
                {

                    _utilitychartstitle = value;
                    this.NotifyPropertyChanged(nameof(UtilityChartsTitle));
                }
            }
        }

        private string _utilitychartsplottitle = "";
        public string UtilityChartsPlotTitle
        {
            get
            {
                return _utilitychartsplottitle;
            }
            set
            {
                if (_utilitychartsplottitle != value)
                {

                    _utilitychartsplottitle = value;
                    this.NotifyPropertyChanged(nameof(UtilityChartsPlotTitle));
                }
            }
        }

        private string _utilitycoststitle = "";
        public string UtilityCostsTitle
        {
            get
            {
                return _utilitycoststitle;
            }
            set
            {
                if (_utilitycoststitle != value)
                {

                    _utilitycoststitle = value;
                    this.NotifyPropertyChanged(nameof(UtilityCostsTitle));
                }
            }
        }

        private string _utilitybillstitle = "";
        public string UtilityBillsTitle
        {
            get
            {
                return _utilitybillstitle;
            }
            set
            {
                if (_utilitybillstitle != value)
                {

                    _utilitybillstitle = value;
                    this.NotifyPropertyChanged(nameof(UtilityBillsTitle));
                }
            }
        }

        private string _utilityreadingstitle = "";
        public string UtilityReadingsTitle
        {
            get
            {
                return _utilityreadingstitle;
            }
            set
            {
                if (_utilityreadingstitle != value)
                {

                    _utilityreadingstitle = value;
                    this.NotifyPropertyChanged(nameof(UtilityReadingsTitle));
                }
            }
        }

        private string _utilitybreakdowntitle = "";
        public string UtilityBreakdownTitle
        {
            get
            {
                return _utilitybreakdowntitle;
            }
            set
            {
                if (_utilitybreakdowntitle != value)
                {

                    _utilitybreakdowntitle = value;
                    this.NotifyPropertyChanged(nameof(UtilityBreakdownTitle));
                }
            }
        }

        private string _utilityexchangeratestitle = "";
        public string UtilityExchangeRatesTitle
        {
            get
            {
                return _utilityexchangeratestitle;
            }
            set
            {
                if (_utilityexchangeratestitle != value)
                {

                    _utilityexchangeratestitle = value;
                    this.NotifyPropertyChanged(nameof(UtilityExchangeRatesTitle));
                }
            }
        }

        private string _utilityexchangeratesinfo = "";
        public string UtilityExchangeRatesInfo
        {
            get
            {
                return _utilityexchangeratesinfo;
            }
            set
            {
                if (_utilityexchangeratesinfo != value)
                {

                    _utilityexchangeratesinfo = value;
                    this.NotifyPropertyChanged(nameof(UtilityExchangeRatesInfo));
                }
            }
        }

        // Read-Only section
        internal Tracey examine = new Tracey();                 // 1 Log to trace file
#if WINFORMS
        internal bool console = false;                          // 2 Log to console
#endif
        internal string user_id = "";                                // 4 Userid for login
        internal string password = "";                               // 5 Password for login
        internal short brand_code = 0;                              // 6 Brand code (never overwritten)
        internal short supplier_code = 0;                           // 7 Supplier code (never overwritten)
        internal Guid guid = new Guid();                                     // 20
        internal string current_routine = "";                        // 22
        internal string next_routine = "";                           // 23
        internal string pdf_message = "";                            // 25
        internal string logout_pathname = "";                        // 27   // Remove one day
        internal List<string> headers = new List<string>();                         // For extra HTTP headers
        internal int current_page = 0;                              // 29
        internal int connection_timeout = 0;                        // 30                       // 31
        internal short[] last_usage_index = new short[2] { 0, 0 };                      // 36
        internal DateTime[] last_usage_date = new DateTime[2] { SmartParametersV2016.defaultDate, SmartParametersV2016.defaultDate };                    // 37
                                                                                                                                                         // Read/Write internally section        
        internal char account_status = 'Y';                     // 38 returned in Utility.Accounts - Open = 'Y', Switching = 'S', Closed = 'N'
        internal bool login_attempted = false;                          // 39 So we don't go round LOGIN more than once
        internal bool login_finished = false;                           // 40 Set when we enter the next routine after LOGIN
        internal bool check_vat = false;                                // 41 For Sp mainly?
        internal char face_active = SmartParametersV2016.activeDefault;
        internal string face_last_display = "";
        internal GenericAddress ADDRESS = new GenericAddress();
        internal Electricity sparks = new Electricity();
        internal Gas smell = new Gas();
        internal string postcode = "";                               // 48
        internal string account_type = "";                           // 49
        internal string supplier_name = "";
        internal string payment_name = "";                           // 51

        // Finance
        internal short bank_institution_code = 0;                               // To look up Bank ccounts in BAnk_DEtails
        internal short bank_brand_code = 0;                                 // To look up Bank ccounts in BAnk_DEtails
        internal string bank_sort_code = "";                               // To look up Bank ccounts in BAnk_DEtails
        internal string bank_account_number = "";                               // To look up Bank ccounts in BAnk_DEtails
        internal string bank_account_title = "";
        internal string bank_account_other = "";
        internal char bank_status = SmartParametersV2016.defaultChar;
        internal char bank_account_type = SmartParametersV2016.defaultChar;
        internal string bank_account_name = "";         // Bank account name max 35 chars
        internal short bank_payment_day = 0;
        internal string account_name = "";
        internal string account_address = "";
        internal string account_phone_no = "";
        internal string account_email = "";
        internal string account_dob = "";
        internal char autoswitch = SmartParametersV2016.defaultChar;
        internal string last_display = "";
        internal string age_60plus = "";
        internal DateTime last_datetime = SmartParametersV2016.defaultDate;
        internal DateTime next_connection = SmartParametersV2016.defaultDate;
        internal DateTime expiry_date = SmartParametersV2016.defaultDate;
        internal DateTime last_update = SmartParametersV2016.defaultDate;
        internal string autoswitch_destination = "";
        internal short autoswitch_supplier_code = 0;
        internal short autoswitch_brand_code = 0;
        internal int autoswitch_tariff_code = 0;
        internal DateTime autoswitch_email_sent = SmartParametersV2016.defaultDate;
        internal DateTime bill_date = SmartParametersV2016.defaultDate;                            // 53
        internal string statement_id = "";                           // 54
        internal short bill_version = 0;                            // 55 e.g. 0 = old, 1 - new etc.
                                                                    // Odds and sods
        internal string ACCOUNT_NO = "";                     // 94 The Account No shouldn't change through the Bills ... but it might!
        internal DateTime CREATED = SmartParametersV2016.defaultDate;
        internal string STATEMENT_ID = "";                    // 95 Must match what is inside the Bill ..
        internal string BILL_DATE = "";                      // 96
        internal int TARIFF_CODE = 0;                    // 99
        internal string TARIFF_NAME = "";                    // 101
        internal char PAYMENT_PLAN = SmartParametersV2016.defaultChar;                   // 102
        internal string TCR = "";                            // 104
        internal int PAYMENTS_BALANCE = 0;               // 105
        internal short PAYMENTS_ITEM = 0;                  // 106
        internal short SUPPLY_CHARGES_CREDITS_ITEM = 0;    // 107
        internal short ACCOUNT_CHARGES_CREDITS_ITEM = 0;   // 108
        internal string CALORIFIC_VALUE = "";                // 109

        //internal DateTime PAYMENT_DATE;
        //internal short transaction_code =0;
        //internal int transaction_balance = 0;
        //internal int transaction_value = 0;
        internal bool postcode_found = false;

        internal List<SmartUtility.EUnitCharges> bill_e_unit_charges { get; set; }
        internal List<SmartUtility.GUnitCharges> bill_g_unit_charges { get; set; }
        internal List<SmartUtility.EUnitCharges> bill_d_unit_charges { get; set; }

        internal List<SmartUtility.EStandingCharges> bill_e_standing_charges { get; set; }
        internal List<SmartUtility.GStandingCharges> bill_g_standing_charges { get; set; }
        internal List<SmartUtility.EStandingCharges> bill_d_standing_charges { get; set; }


        internal List<SmartUtility.EDiscounts> bill_e_discounts { get; set; }
        internal List<SmartUtility.GDiscounts> bill_g_discounts { get; set; }
        internal List<SmartUtility.EDiscounts> bill_d_discounts { get; set; }

        internal string payment_method = "";
        internal int payments_made = 0;
        internal int billed_amount = 0;
        internal int utility_brand_index = 0;
        internal short brand_code_temp = 0;

        internal List<string> resource_codeList;

        internal bool[] autoswitchToggle = new bool[2] { false, false };


#if WINFORMS
        internal Color DayRateBrush;
        internal Color NightRateBrush;
        internal Color StandingChargeBrush;
#endif
#if WPF  || WINUI
        internal Brush DayRateBrush {get; set; }
        internal Brush NightRateBrush {get; set; }
        internal Brush StandingChargeBrush {get; set; }
#endif
#if ANDROIDX
        internal Color DayRateBrush;
        internal Color NightRateBrush;
        internal Color StandingChargeBrush;
#endif
#if SMARTMAUI
        internal Color DayRateBrush;
        internal Color NightRateBrush;
        internal Color StandingChargeBrush;

        private bool _showUsername;
        public bool ShowUsernameCosts
        {
            get => _showUsername;
            set
            {
                _showUsername = value;
                this.NotifyPropertyChanged(nameof(ShowUsernameCosts));
            }
        }

#endif

        internal string tariff_report = "";

        internal int udprn_index = -1;

        internal List<SmartUtility.Analysis_Bills> analysis_bills_found;

        internal decimal total_units = 0.0M;
        internal decimal total_value = 0.0M;
        internal int total_item_count = 0;

        internal decimal total_line = 0.0M;
        internal decimal total_units_x_items = 0.0M;

        internal decimal total_simulate_readings = 0m;  // Don't think this is used
        internal int total_simulate_cost = 0;           // Nor this either

        internal char local_resource_code;
        internal DateTime from_date = SmartParametersV2016.defaultDate;
        internal DateTime to_date = SmartParametersV2016.defaultDate;

        // Analyze start
        internal string analyzeMessage = "";
        internal short uniqueNumber = 0;
        internal decimal condition_costs = 0m;
        internal string tcr = "";
        internal decimal post_condition_costs = 0;
        internal decimal temp_decimal = 0m;

        internal string time_limit = "";
        internal decimal dunits_limit = 0m;

        internal decimal post_period_costs = 0m;
        internal bool bumped_index = false; // Not sure we need this
        internal decimal units = 0m;
        internal decimal tier1_remainder = 0m;

        internal decimal discount = 0m;
        internal decimal vat_rate = 0m;

        internal bool add_tier = false;
        internal string SC_UNITS = "";
        internal string SC_DESCRIPTION = "";

        internal decimal pricetierRemainder = 0m;
        internal decimal priceUnits = 0m;
        internal bool pricebumpedIndex = false;

        internal decimal total_monthly_costs = 0m;
        internal decimal total_quarterly_costs = 0m;
        internal decimal total_yearly_costs = 0m;

        internal string doRowsFoundMessage = "";

        internal decimal day_units_rate = 0m;
        internal decimal night_units_rate = 0m;
        internal decimal tariff_comparison_rate = 0m;
        internal decimal standing_charge1 = 0m;
        internal decimal standing_charge2 = 0m; // <= Fucking Utility Whorehouse cunts
        internal decimal leftover_cost = 0m;

        internal string sc_code = "";
        internal int sc_units_days = 0;
        internal int sc_units_months = 0;
        internal int sc_units_quarters = 0;
        internal int sc_units_years = 0;

        // Analyze end

        // Report start
        internal string report_box = "";
        internal bool reportBandLimit = false;
        internal string reportPaymentPlans = "";
        internal string reportStandingCharge = "";
        internal string reportTcr = "";
        internal string reportPlansRange = "";
        // Report end

        // Charts
        internal DateTime ChartsEngineFromDate { get; set; }
        internal DateTime ChartsEngineToDate { get; set; }
        internal DateTime ChartsMinimum { get; set; }
        internal DateTime ChartsMaximum { get; set; }
        internal double chartsmax = 0;    // For sine wave fitting
        internal double chartsmin = 0;    // For sine wave fitting
        internal int ChartsXValue = 0;

        internal DateTime ChartsStartDate { get; set; }
#if WPF || WINUI || ANDROIDX
        internal OxyColor ChartsBackground;
#endif
        internal List<SmartUtility.EUsageView> e_usage_view_found = new List<SmartUtility.EUsageView>();
        internal List<SmartUtility.GUsageView> g_usage_view_found = new List<SmartUtility.GUsageView>();
        internal List<SmartUtility.EUsageView> d_usage_view_found = new List<SmartUtility.EUsageView>();

        // Charts

        // Scraper
        internal DateTime EFirstDate { get; set; }
        internal DateTime ELastDate { get; set; }
        internal Decimal EFirstRead { get; set; }
        internal Decimal ELastRead { get; set; }

        internal DateTime GFirstDate { get; set; }
        internal DateTime GLastDate { get; set; }
        internal Decimal GFirstRead = 0m;
        internal Decimal GLastRead = 0m;

        internal DateTime EngineToDate { get; set; }

        internal DateTime[] scraperlast_usage_date = new DateTime[2];
        internal short[] scraperlast_usage_index = new short[2];

        internal string scraperlast_display = "";
        //
        // Spike
        internal string temp_payment_name = "";
        //
        // Switchers
        internal string wskey = "";
        internal string wskey_name = "";
        internal string switcherdone = "";
        internal string proceed_online_path = "";
        // Switcher
#endif  // End of DBSERVER
    }
}