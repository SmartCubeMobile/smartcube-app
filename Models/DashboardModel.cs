using OxyPlot.WindowsForms;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Threading;
using System.Windows.Forms;

namespace SmartCubeMobile
{
    public class DashboardModel
    {
        internal List<SqlTables> sql_tables_list;
        internal List<SqlKeys> sql_keys_list;
        internal List<SqlFields> sql_fields_list;

        internal System.Windows.Forms.TextBox Table_Name = new System.Windows.Forms.TextBox();

        internal List<string> forfucksake;

        internal string errorMessage = "";

        internal bool copy_success = false;

        internal TextBox Updates_Name = new System.Windows.Forms.TextBox();
        internal int read_counter = 0;
        internal int add_counter = 0;


        // SP Analyze
        internal string[] lines;
        internal string filePath_s = "";
        internal List<string> list_lines;
        internal bool supply_area = false;

        //

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



        internal Form connection_page = new Form();
#endif

#if WINFORMS
        internal List<SmartUtility.BrandMatrix> brand_matrix_found;
        internal SmartUtility.ConditionsPlans conditions_plans_row;
        internal string drop_down_text = string.Empty;
        internal string selection_name = string.Empty;
        internal string payment_plans = string.Empty;
        internal SmartUtility.ConditionsLimits conditions_limit_row;
        internal SmartUtility.Suppliers suppliers_row;
        internal SmartUtility.Brands brands_row;
        internal SmartUtility.Tariffs tariffs_row;
        internal SmartUtility.TariffHistory tariff_history_row;
        internal SmartUtility.TariffPlans tariff_plans_row;
        internal SmartUtility.ConditionsDates conditions_dates_row;
        internal SmartUtility.ConditionsGroups conditions_group_row;
        internal short conditions_dates_code = 0;

        internal List<SmartUtility.Brands> brands_found;


        internal DateTime Updated;
        internal string Status_Flag;
        internal DateTime Deactivated;

        internal DateTime tariff_valid_from = SmartParametersV2016.defaultDate;
        internal string load_filename = "";

        internal int supplier_index = 0;
        internal string status_message = "";
        internal short version_code = SmartParametersV2016.defaultVersionCode;
        internal string tariff_name = string.Empty;
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

#if WINFORMS
        //internal PlotView costsplotview = new PlotView();
        //internal PlotView readingsplotview = new PlotView();
        //internal PlotView usageplotview = new PlotView();
#endif

#if WINFORMS
        internal bool console = false;                          // 2 Log to console

        internal char resource_code = SmartParametersV2016.defaultChar;// 'E', 'G' or 'D'

        internal string resource_type = string.Empty;
        internal short supplier_code = 0;                           // 7 Supplier code (never overwritten)
        internal short area_code = 0;   // Don't fink there IS an area code 0??
        internal short brand_code = 0;                              // 6 Brand code (never overwritten)
        internal int tariff_code = 0;
        internal bool login_finished = false;                           // 40 Set when we enter the next routine after LOGIN

        internal int connection_timeout = 0;                        // 30
        internal HtmlAgilityPack.HtmlDocument htmlDocument = new HtmlAgilityPack.HtmlDocument();
        //internal string type;
        //internal string response;
        internal Decimal VAT_RATE = 0;

        internal CancellationTokenSource utilityCts = new CancellationTokenSource();
        internal CancellationToken utilityToken = new CancellationToken();

        // In th mud
        internal BindingList<Members> membersList;

        // This is only used in SmartDashboard (because its a struct, it doesn't use any space!)
        internal bool trace = false;
        internal bool hamas_loaded;
        internal string table_count;
        internal string ageString = "0";
        internal char payment_plan = SmartParametersV2016.defaultChar;

        internal bool use_proxy = false;
        internal string bill_token = string.Empty;
        internal string bill_currency_symbol = SmartParametersV2016.defaultISOConvertToSymbol;
        internal char bill_denomination_symbol = SmartParametersV2016.defaultDenominationSymbol;
        internal char bill_currency_separator = SmartParametersV2016.defaultCurrencySeparator;
        internal char bill_thousands_separator = SmartParametersV2016.defaultThousandsSeparator;
        internal char bill_prfix = SmartParametersV2016.defaultBillprfix;
        internal bool download_bills = true;
        internal string prfix_xxx = string.Empty;
        internal string target_pathname = string.Empty;


        // Energylinx
        internal int target_count;
        internal bool one_e_found;
        internal bool one_e7_found;

        internal DataTable energyupdates_table;
        internal DataTable newupdates_table;
        //

        // Finance
        internal string consumer_key = string.Empty;
        internal string consumer_secret = string.Empty;
        internal char category_code = SmartParametersV2016.defaultChar;// 'B', 'I' or 'S'


        //
#endif
    }
}
