using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;

#if WINFORMS
using System.Windows.Forms.VisualStyles;
//using System.Windows.Controls;
using System.Windows;
#endif

#if WPF
using System.Windows;
using Org.BouncyCastle.Crypto.Modes.Gcm;
#endif

#if WINUI
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;
#endif

#if ANDROIDX
using Android.Views;
using Android.Transitions;
#endif

namespace SmartCubeMobile
{
    public class SmartSwitchItem
    {
        public string User { get; set; }
        public string Group { get; set; }
        internal object SmartSwitchList { get; set; }        
    }
    public class BrandItem
    {
        public short InstitutionCode { get; set; }
        public short BRAND_CODE { get; set; }
        public string BrandName { get; set; }
    }

    public class Row
    {
        public List<BrandItem> AvailableBrands { get; set; }
        public BrandItem SelectedEntry { get; set; }
    }
    public class EntryItem
    {
        public short InstitutionCode { get; set; }
        public short BrandCode { get; set; }
        public int LoginMethod { get; set; }
        public string Parameter1 { get; set; }
    }

    public class CurrencyTab
    {
        public string Code { get; set; }
        public short Ordinal { get; set; }
        public string Label { get; set; }
        public bool IsVisible { get; set; }
    }

    public class CurrencyValues
    {
        public string Code { get; set; }
        public decimal PaidIn { get; set; }
        public decimal PaidOut { get; set; }
        public decimal Difference { get; set; }
    }

    public class LoginKeys
    {
        public string userName = "";    // Lowercase or uppercase - max 18 characters
        public string passwordHash = "";// Might turn this to <blank> .. no, might use Hash
        public bool trace = false;        // Sent as 'true' or 'false'                                
        public DateTime expirationDate1 = SmartParametersV2016.defaultDate; // sent in 'en-GB' format
        public DateTime expirationDate2 = SmartParametersV2016.defaultDate; // sent in 'en-GB' format
        public DateTime expirationDate3 = SmartParametersV2016.defaultDate; // sent in 'en-GB' format
        public bool subscriber = false;          // Sent as 'true' or 'false'
        public bool multimeter = false;          // Sent as 'true' or 'false'
        public bool administrator = false;       // Sent as 'true' or 'false'               
        public DateTime lastLoginTime = SmartParametersV2016.defaultDate; // Sent as a DateTime                                            
        public string userId = "";            // unique netcore-KeasdonEnergy.db Id      
        public DateTime previousLogonDate = SmartParametersV2016.defaultDate;
    }
    
    public class AccountLink
    {
        // Public ... or it doesn't Deserialize!!!
        public string Id { get; set; }
        public string Href { get; set; }
        public string Category { get; set; }
        public string Description { get; set; }
        public string AccountNumber { get; set; }
        public string Balance { get; set; }
    }

#if WINFORMS || WPF  || WINUI || ANDROIDX
    public class LanguageDictionary
    {
        public string languageCode;
#if WINFORMS
        public Dictionary<string, string> resourceDictionary;
#endif
#if WPF  || WINUI
        public Dictionary<string, string> resourceDictionary;
#endif
#if ANDROIDX
        public Dictionary<string, string> resourceDictionary;
#endif
    };
#endif

#if SMARTMAUI
    public class LanguageDictionary
    {
        public string languageCode;
        public Dictionary<string, string> resourceDictionary;
    };
#endif

    // Keep this class until we can get rid of looking for cookies in British Gas
    internal class AntiXsrfToken
    {
        internal string username = "";
        internal string cookie_value = "";
        internal string cookie_path = "";
        internal string cookie_domain = "";
        internal bool cookie_secure = false;
        internal DateTime cookie_timestamp = SmartParametersV2016.defaultDate;
        internal DateTime keepalive_timestamp = SmartParametersV2016.defaultDate;
        internal bool update = false;
    }

    internal class RequestVerificationToken
    {
        internal string username = "";
        internal string request_value = "";
        internal DateTime request_timestamp = SmartParametersV2016.defaultDate;
        internal DateTime keepalive_timestamp = SmartParametersV2016.defaultDate;
        internal List<SmartProfile.Groups> groupsList = new List<SmartProfile.Groups>();
        internal bool update = false;
    }

    internal class JsClickResult
    {
        public bool success { get; set; }
        public string message { get; set; }
        public bool visible { get; set; }
        public bool disabled { get; set; }
        public string display { get; set; }
        public string visibility { get; set; }
        public string opacity { get; set; }
    }

    // Message class to deserialize the JS object
    internal class WebMessage
    {
        // Names are case-sensitive - don't change these
        public string status { get; set; }
        public string text { get; set; }
    }

#if WINFORMS
    //public class CustomListView : ListView
    //{
    //    public string Content { get; set; }
    //    public object Value { get; set; }
    //    public Color Color { get; set; }
    //    public bool Selected { get; set; }
    //}
#endif

#if WINFORMS

    public struct Members
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public string ClearPassword { get; set; }
        public DateTime Expiration1 { get; set; }
        public DateTime Expiration2 { get; set; }
        public bool Subscriber { get; set; }
        public bool Trace { get; set; }
        public DateTime LastLoginDate { get; set; }
        public bool MultipleMeter { get; set; }
        public bool Administrator { get; set; }
        public string Id { get; set; }        
        public string PasswordHash { get; set; }
    }

    public struct ErrorLog
    {
        public DateTime LogDate { get; set; }
        public string ProcessInfo { get; set; }
        public string Text { get; set; }
    }
#endif
    //public static class Desperate
    //{
    //    public static string ConvertCurrency(int amount)
    //    {
    //        string amount_str = amount.ToString();
    //        int len = amount_str.Length;

    //        switch (len)
    //        {
    //            case 0:
    //                amount_str = "0.00";
    //                break;
    //            case 1:
    //                amount_str = "0.0" + amount_str;
    //                break;
    //            case 2:
    //                amount_str = "0." + amount_str;
    //                break;
    //            default:
    //                len = len - 2;
    //                amount_str = amount_str.Substring(0, len) + SmartParametersV2016.decimalPoint + amount_str.Substring(len);
    //                break;
    //        }
    //        return "£" + amount_str;
    //    }
    //}

    // List of Lists
    public class SmartUsersList
    {
        internal List<SmartUsers.ConsumersSQLite> sqliteConsumersList = new List<SmartUsers.ConsumersSQLite>();
        internal List<SmartUsers.Consumers> consumersList = new List<SmartUsers.Consumers>(); // Never used thus far <= yes now when FULL Screen
        internal List<SmartUsers.Consumers> consumersChangesList = new List<SmartUsers.Consumers>(); // Never used thus far <= yes now when FULL Screen
    }

    public class SmartProfileList
    {
        internal List<SmartProfile.CubefacesSQLite> sqlite_cubefacesList = new List<SmartProfile.CubefacesSQLite>();
        internal List<SmartProfile.Cubefaces> profilecubefacesList = new List<SmartProfile.Cubefaces>();
        internal List<SmartProfile.Cubefaces> profilecubefacesChangesList = new List<SmartProfile.Cubefaces>(); // Only used when switching screens?
        internal List<SmartProfile.GroupsSQLite> sqlite_groupsList = new List<SmartProfile.GroupsSQLite>();
        internal List<SmartProfile.Groups> profilegroupsList = new List<SmartProfile.Groups>();
        internal List<SmartProfile.Groups> profilegroupsChangesList = new List<SmartProfile.Groups>();
        internal List<SmartProfile.ProfilesSQLite> sqlite_profilesList = new List<SmartProfile.ProfilesSQLite>();
        internal List<SmartProfile.Profiles> profilesList = new List<SmartProfile.Profiles>();
        internal List<SmartProfile.Profiles> profilesChangesList = new List<SmartProfile.Profiles>();
        //internal List<SmartProfile.ProfilesCubeSQLite> sqlite_profilescubeList = new List<SmartProfile.ProfilesCubeSQLite>();
        //internal List<SmartProfile.ProfilesCube> profilescubeList = new List<SmartProfile.ProfilesCube>();
        //internal List<SmartProfile.ProfilesCube> profilescube_changesList = new List<SmartProfile.ProfilesCube>();
        internal List<SmartProfile.AddressesViewSQLite> sqlite_addressesviewList = new List<SmartProfile.AddressesViewSQLite>();
        internal List<SmartProfile.AddressesView> addressesviewList = new List<SmartProfile.AddressesView>();
        internal List<SmartProfile.AddressesView> addressesview_changesList = new List<SmartProfile.AddressesView>();

    };

    public class SmartMumList
    {
        internal List<SmartData.Buttons> buttonsList = new List<SmartData.Buttons>();
        internal List<SmartData.Cubefaces> cubefacesList = new List<SmartData.Cubefaces>();
        internal List<SmartData.Cultures> culturesList = new List<SmartData.Cultures>();
        internal List<SmartData.Currencies> currenciesList = new List<SmartData.Currencies>();
        internal List<SmartData.CultureView> cultureviewList = new List<SmartData.CultureView>();

        internal List<SmartData.Handlers> handlersList = new List<SmartData.Handlers>();
        internal List<SmartData.Schemas> schemasList = new List<SmartData.Schemas>();

        internal List<SmartData.SQLiteSchemas> sqliteschemasList = new List<SmartData.SQLiteSchemas>();
        internal List<SmartData.SQLiteTables> sqlitetablesList = new List<SmartData.SQLiteTables>();
        internal List<SmartData.SQLiteFields> sqlitefieldsList = new List<SmartData.SQLiteFields>();

        internal List<SmartData.Tooltips> tooltipsList = new List<SmartData.Tooltips>();

#if WINFORMS
        internal List<SmartData.SmartSwitchFields> smartswitchfieldsList = new List<SmartData.SmartSwitchFields>();
#endif
    }

    public class SmartJoyList
    {
        internal List<SmartUsers.ExchangeRates> exchangeRatesList = new List<SmartUsers.ExchangeRates>();
        internal List<SmartUsers.InternalPostcodes> postcodesList = new List<SmartUsers.InternalPostcodes>();
        // Read-Only/Update section
        internal List<SmartUsers.Working_Postcodes> workingPostcodesList = new List<SmartUsers.Working_Postcodes>();
        internal List<SmartUsers.VatRates> vatRatesList = new List<SmartUsers.VatRates>();
        internal List<SmartUsers.ExternalResources> externalResourcesList = new List<SmartUsers.ExternalResources>();
    }

    public class SmartFinanceList
    {
        // SmartFinance
        internal List<SmartFinance.BrandAccounts> brand_accountsList = new List<SmartFinance.BrandAccounts>();
        internal List<SmartFinance.BrandConnection> brand_connectionList = new List<SmartFinance.BrandConnection>();
        internal List<SmartFinance.BrandMatrix> brand_matrixList = new List<SmartFinance.BrandMatrix>();
        //internal List<SmartFinance.BrandOrdinals> brand_ordinalsList = new List<SmartFinance.BrandOrdinals>();
        internal List<SmartFinance.Brands> brandsList = new List<SmartFinance.Brands>();
        internal List<SmartFinance.BrandAreas> brand_areasList = new List<SmartFinance.BrandAreas>();
        internal List<SmartFinance.CategoryCodes> category_codesList = new List<SmartFinance.CategoryCodes>();
        internal List<SmartFinance.InstitutionInfo> institution_infoList = new List<SmartFinance.InstitutionInfo>();
        internal List<SmartFinance.Institutions> institutionsList = new List<SmartFinance.Institutions>();
        internal List<SmartFinance.Templates> templatesList = new List<SmartFinance.Templates>();
        //internal List<SmartFinance.Transaction_Flows> transaction_flowsList = new List<SmartFinance.Transaction_Flows>();
        internal List<SmartFinance.Transaction_Groups> transaction_groupsList = new List<SmartFinance.Transaction_Groups>();
        internal List<SmartFinance.Transaction_Types> transaction_typesList = new List<SmartFinance.Transaction_Types>();
        //SmartSwitch
        internal List<SmartFinance.AccountsSQLite> sqlite_finance_accountsList = new List<SmartFinance.AccountsSQLite>();
        internal List<SmartFinance.Accounts> finance_accountsList = new List<SmartFinance.Accounts>();
        internal List<SmartFinance.Accounts> finance_accounts_changesList = new List<SmartFinance.Accounts>();

        internal List<SmartFinance.CategoriesSQLite> sqlite_finance_categoriesList = new List<SmartFinance.CategoriesSQLite>();
        internal List<SmartFinance.Categories> finance_categoriesList = new List<SmartFinance.Categories>();
        internal List<SmartFinance.Categories> finance_categories_changesList = new List<SmartFinance.Categories>();
        //
        internal List<SmartFinance.CategoryTypesSQLite> sqlite_finance_categorytypesList = new List<SmartFinance.CategoryTypesSQLite>();
        internal List<SmartFinance.CategoryTypes> finance_categorytypesList = new List<SmartFinance.CategoryTypes>();
        internal List<SmartFinance.CategoryTypes> finance_categorytypes_changesList = new List<SmartFinance.CategoryTypes>();
        //
        internal List<SmartFinance.ConnectionsSQLite> sqlite_finance_connectionsList = new List<SmartFinance.ConnectionsSQLite>();
        internal List<SmartFinance.Connections> finance_connectionsList = new List<SmartFinance.Connections>();
        internal List<SmartFinance.Connections> finance_connections_changesList = new List<SmartFinance.Connections>();
        //
        internal List<SmartFinance.LoginsSQLite> sqlite_finance_loginsList = new List<SmartFinance.LoginsSQLite>();
        internal List<SmartFinance.Logins> finance_loginsList = new List<SmartFinance.Logins>();
        internal List<SmartFinance.Logins> finance_logins_changesList = new List<SmartFinance.Logins>();
        //
        internal List<SmartFinance.SwitchesSQLite> sqlite_finance_switchesList = new List<SmartFinance.SwitchesSQLite>();
        internal List<SmartFinance.Switches> finance_switchesList = new List<SmartFinance.Switches>();
        internal List<SmartFinance.Switches> finance_switches_changesList = new List<SmartFinance.Switches>();

        internal List<SmartFinance.CryptoAccountsSQLite> sqlite_crypto_accountsList = new List<SmartFinance.CryptoAccountsSQLite>();
        internal List<SmartFinance.CryptoAccounts> crypto_accountsList = new List<SmartFinance.CryptoAccounts>();
        internal List<SmartFinance.CryptoAccounts> crypto_accounts_changesList = new List<SmartFinance.CryptoAccounts>();

        internal List<SmartFinance.CryptoAddressesSQLite> sqlite_crypto_addressesList = new List<SmartFinance.CryptoAddressesSQLite>();
        internal List<SmartFinance.CryptoAddresses> crypto_addressesList = new List<SmartFinance.CryptoAddresses>();
        internal List<SmartFinance.CryptoAddresses> crypto_addresses_changesList = new List<SmartFinance.CryptoAddresses>();

        internal List<SmartFinance.CryptoCurrencyRatesSQLite> sqlite_crypto_currenciesList = new List<SmartFinance.CryptoCurrencyRatesSQLite>();
        internal List<SmartFinance.CryptoCurrencyRates> crypto_currenciesList = new List<SmartFinance.CryptoCurrencyRates>();
        internal List<SmartFinance.CryptoCurrencyRates> crypto_currencies_changesList = new List<SmartFinance.CryptoCurrencyRates>();

        internal List<SmartFinance.CryptoLedgersSQLite> sqlite_crypto_ledgersList = new List<SmartFinance.CryptoLedgersSQLite>();
        internal List<SmartFinance.CryptoLedgers> crypto_ledgersList = new List<SmartFinance.CryptoLedgers>();
        internal List<SmartFinance.CryptoLedgers> crypto_ledgers_changesList = new List<SmartFinance.CryptoLedgers>();

        //internal List<SmartFinance.CryptoTransactionsViewSQLite> sqlite_crypto_transactionsList = new List<SmartFinance.CryptoTransactionsViewSQLite>();
        //internal List<SmartFinance.CryptoTransactionsView> crypto_transactionsList = new List<SmartFinance.CryptoTransactionsView>();
        //internal List<SmartFinance.CryptoTransactionsView> crypto_transactions_changesList = new List<SmartFinance.CryptoTransactionsView>();

        internal List<SmartFinance.CryptoWalletsSQLite> sqlite_crypto_walletsList = new List<SmartFinance.CryptoWalletsSQLite>();
        internal List<SmartFinance.CryptoWallets> crypto_walletsList = new List<SmartFinance.CryptoWallets>();
        internal List<SmartFinance.CryptoWallets> crypto_wallets_changesList = new List<SmartFinance.CryptoWallets>();

        internal List<SmartFinance.CryptoWalletTotalsSQLite> sqlite_crypto_wallettotalsList = new List<SmartFinance.CryptoWalletTotalsSQLite>();
        internal List<SmartFinance.CryptoWalletTotals> crypto_wallettotalsList = new List<SmartFinance.CryptoWalletTotals>();
        internal List<SmartFinance.CryptoWalletTotals> crypto_wallettotals_changesList = new List<SmartFinance.CryptoWalletTotals>();

        internal List<SmartFinance.TransactionsSQLite> sqlite_transactionsList = new List<SmartFinance.TransactionsSQLite>();
        internal List<SmartFinance.Transactions> transactionsList = new List<SmartFinance.Transactions>();
        internal List<SmartFinance.Transactions> transactions_changesList = new List<SmartFinance.Transactions>();

        internal List<SmartFinance.TransactionsCategoriesSQLite> sqlite_transactionscategoriesList = new List<SmartFinance.TransactionsCategoriesSQLite>();
        internal List<SmartFinance.TransactionsCategories> transactionscategoriesList = new List<SmartFinance.TransactionsCategories>();
        internal List<SmartFinance.TransactionsCategories> transactionscategories_changesList = new List<SmartFinance.TransactionsCategories>();


    }

    public class SmartUtilityList
    {
        // SmartUtility Tables
        internal List<SmartUtility.ConditionsDates> conditions_datesList = new List<SmartUtility.ConditionsDates>();
        internal List<SmartUtility.ConditionsGroups> conditions_groupsList = new List<SmartUtility.ConditionsGroups>();
        internal List<SmartUtility.ConditionsLimits> conditions_limitsList = new List<SmartUtility.ConditionsLimits>();
        internal List<SmartUtility.ConditionsAreas> conditions_areasList = new List<SmartUtility.ConditionsAreas>();
        internal List<SmartUtility.ConditionsPlans> conditions_plansList = new List<SmartUtility.ConditionsPlans>();
        internal List<SmartUtility.PaymentMethods> payment_methodsList = new List<SmartUtility.PaymentMethods>();
        internal List<SmartUtility.PaymentPlans> payment_plansList = new List<SmartUtility.PaymentPlans>();
        internal List<SmartUtility.Post_Conditions> post_conditionsList = new List<SmartUtility.Post_Conditions>();
        internal List<SmartUtility.Post_Groupings> post_groupingsList = new List<SmartUtility.Post_Groupings>();
        internal List<SmartUtility.Post_Groups> post_groupsList = new List<SmartUtility.Post_Groups>();
        internal List<SmartUtility.Post_Limits> post_limitsList = new List<SmartUtility.Post_Limits>();
        internal List<SmartUtility.Post_Select> post_selectList = new List<SmartUtility.Post_Select>();
        internal List<SmartUtility.Post_Codes> post_codesList = new List<SmartUtility.Post_Codes>();
        internal List<SmartUtility.PreConditions> pre_conditionsList = new List<SmartUtility.PreConditions>();
        internal List<SmartUtility.BrandMatrix> brand_matrixList = new List<SmartUtility.BrandMatrix>();
        internal List<SmartUtility.BrandConnection> brand_connectionList = new List<SmartUtility.BrandConnection>();
        internal List<SmartUtility.Suppliers> suppliersList = new List<SmartUtility.Suppliers>();
        internal List<SmartUtility.Brands> brandsList = new List<SmartUtility.Brands>();
        internal List<SmartUtility.DistributorInfo> distributor_infoList = new List<SmartUtility.DistributorInfo>();
        internal List<SmartUtility.SupplierTypes> supplier_typesList = new List<SmartUtility.SupplierTypes>();
        internal List<SmartUtility.SupplierInfo> supplier_infoList = new List<SmartUtility.SupplierInfo>();
        internal List<SmartUtility.SupplyAreas> supply_areasList = new List<SmartUtility.SupplyAreas>();
        internal List<SmartUtility.Tariffs> tariffsList = new List<SmartUtility.Tariffs>();
        internal List<SmartUtility.TariffMatrix> tariff_matrixList = new List<SmartUtility.TariffMatrix>();
        internal List<SmartUtility.TariffCodesNames> tariff_codes_namesList = new List<SmartUtility.TariffCodesNames>();
        internal List<SmartUtility.TariffHistory> tariff_historyList = new List<SmartUtility.TariffHistory>();
        internal List<SmartUtility.TariffPlans> tariff_plansList = new List<SmartUtility.TariffPlans>();
        internal List<SmartUtility.WithdrawnDate> withdrawnDateList = new List<SmartUtility.WithdrawnDate>();
        internal List<SmartUtility.UnitRates> e_unit_ratesList = new List<SmartUtility.UnitRates>();
        internal List<SmartUtility.UnitRates> g_unit_ratesList = new List<SmartUtility.UnitRates>();
        internal List<SmartUtility.ResourceCodes> resource_codesList = new List<SmartUtility.ResourceCodes>();
        internal List<SmartUtility.ResourceTypes> resource_typesList = new List<SmartUtility.ResourceTypes>();
        internal List<SmartUtility.GasConversion> gas_conversionList = new List<SmartUtility.GasConversion>();
        internal List<SmartUtility.Templates> templateList = new List<SmartUtility.Templates>();

        // SmartSwitch Tables
        // Insert only - never Updated or Deleted
        internal List<SmartUtility.AccChargesCreditsSQLite> sqlite_account_charges_creditsList = new List<SmartUtility.AccChargesCreditsSQLite>();
        internal List<SmartUtility.AccChargesCredits> account_charges_creditsList = new List<SmartUtility.AccChargesCredits>();
        internal List<SmartUtility.AccChargesCredits> account_charges_credits_changesList = new List<SmartUtility.AccChargesCredits>();
        // Insert and Update possible - never Deleted
        internal List<SmartUtility.AccountsSQLite> sqlite_utility_accountsList = new List<SmartUtility.AccountsSQLite>();
        internal List<SmartUtility.Accounts> utility_accountsList = new List<SmartUtility.Accounts>();
        internal List<SmartUtility.Accounts> utility_accounts_changesList = new List<SmartUtility.Accounts>();
        // Insert and Update possible - never Deleted
        //
        internal List<SmartUtility.BankDetailsSQLite> sqlite_utility_bankdetailsList = new List<SmartUtility.BankDetailsSQLite>();
        internal List<SmartUtility.BankDetails> utility_bankdetailsList = new List<SmartUtility.BankDetails>();
        internal List<SmartUtility.BankDetails> utility_bankdetails_changesList = new List<SmartUtility.BankDetails>();
        // Insert only - never Updated or Deleted
        internal List<SmartUtility.BillsSQLite> sqlite_billsList = new List<SmartUtility.BillsSQLite>();
        internal List<SmartUtility.Bills> billsList = new List<SmartUtility.Bills>();
        internal List<SmartUtility.Bills> bills_changesList = new List<SmartUtility.Bills>();
        internal List<SmartUtility.Bills> working_billsList = new List<SmartUtility.Bills>();
        // Insert only - never Updated or Deleted
        internal List<SmartUtility.BillsResourceSQLite> sqlite_bills_resourceList = new List<SmartUtility.BillsResourceSQLite>();
        internal List<SmartUtility.BillsResource> bills_resourceList = new List<SmartUtility.BillsResource>();
        internal List<SmartUtility.BillsResource> bills_resource_changesList = new List<SmartUtility.BillsResource>();
        // Insert only - never Updated or Deleted
        internal List<SmartUtility.ECostsSQLite> sqlite_e_costsList = new List<SmartUtility.ECostsSQLite>();
        internal List<SmartUtility.ECosts> e_costsList = new List<SmartUtility.ECosts>();
#if WINFORMS
        internal List<SmartUtility.ECosts> e_costs_changesList = new List<SmartUtility.ECosts>();
#endif
        internal List<SmartUtility.AnalysisCosts> e_analysis_costsList = new List<SmartUtility.AnalysisCosts>();
        internal List<SmartUtility.AnalysisCosts> e_analysis_costs_changesList = new List<SmartUtility.AnalysisCosts>();
        // Insert only - never Updated or Deleted
        internal List<SmartUtility.EDiscountsSQLite> sqlite_e_discountsList = new List<SmartUtility.EDiscountsSQLite>();
        internal List<SmartUtility.EDiscounts> e_discountsList = new List<SmartUtility.EDiscounts>();
        internal List<SmartUtility.EDiscounts> e_discounts_changesList = new List<SmartUtility.EDiscounts>();
        // Insert only - never Updated or Deleted
        internal List<SmartUtility.EReadingsSQLite> sqlite_e_readingsList = new List<SmartUtility.EReadingsSQLite>();
        internal List<SmartUtility.EReadings> e_readingsList = new List<SmartUtility.EReadings>();
        internal List<SmartUtility.EReadings> e_readings_changesList = new List<SmartUtility.EReadings>();
        // Insert only - never Updated or Deleted
        internal List<SmartUtility.EStandingChargesSQLite> sqlite_e_standing_chargesList = new List<SmartUtility.EStandingChargesSQLite>();
        internal List<SmartUtility.EStandingCharges> e_standing_chargesList = new List<SmartUtility.EStandingCharges>();
        internal List<SmartUtility.EStandingCharges> e_standing_charges_changesList = new List<SmartUtility.EStandingCharges>();
        // Insert only - never Updated or Deleted
        internal List<SmartUtility.EUnitChargesSQLite> sqlite_e_unit_chargesList = new List<SmartUtility.EUnitChargesSQLite>();
        internal List<SmartUtility.EUnitCharges> e_unit_chargesList = new List<SmartUtility.EUnitCharges>();
        internal List<SmartUtility.EUnitCharges> e_unit_charges_changesList = new List<SmartUtility.EUnitCharges>();
        // Insert only - never Updated or Deleted
        internal List<SmartUtility.EUsageSQLite> sqlite_e_usageList = new List<SmartUtility.EUsageSQLite>();
        internal List<SmartUtility.EUsage> e_usageList = new List<SmartUtility.EUsage>();
        internal List<SmartUtility.EUsage> e_usage_changesList = new List<SmartUtility.EUsage>();
        // Insert only - never Updated or Deleted
        internal List<SmartUtility.GCostsSQLite> sqlite_g_costsList = new List<SmartUtility.GCostsSQLite>();
        internal List<SmartUtility.GCosts> g_costsList = new List<SmartUtility.GCosts>();
#if WINFORMS
        internal List<SmartUtility.GCosts> g_costs_changesList = new List<SmartUtility.GCosts>();
#endif
        internal List<SmartUtility.AnalysisCosts> g_analysis_costsList = new List<SmartUtility.AnalysisCosts>();
        internal List<SmartUtility.AnalysisCosts> g_analysis_costs_changesList = new List<SmartUtility.AnalysisCosts>();
        // Insert only - never Updated or Deleted
        internal List<SmartUtility.GDiscountsSQLite> sqlite_g_discountsList = new List<SmartUtility.GDiscountsSQLite>();
        internal List<SmartUtility.GDiscounts> g_discountsList = new List<SmartUtility.GDiscounts>();
        internal List<SmartUtility.GDiscounts> g_discounts_changesList = new List<SmartUtility.GDiscounts>();
        // Insert only - never Updated or Deleted
        internal List<SmartUtility.GReadingsSQLite> sqlite_g_readingsList = new List<SmartUtility.GReadingsSQLite>();
        internal List<SmartUtility.GReadings> g_readingsList = new List<SmartUtility.GReadings>();
        internal List<SmartUtility.GReadings> g_readings_changesList = new List<SmartUtility.GReadings>();
        // Insert only - never Updated or Deleted
        internal List<SmartUtility.GStandingChargesSQLite> sqlite_g_standing_chargesList = new List<SmartUtility.GStandingChargesSQLite>();
        internal List<SmartUtility.GStandingCharges> g_standing_chargesList = new List<SmartUtility.GStandingCharges>();
        internal List<SmartUtility.GStandingCharges> g_standing_charges_changesList = new List<SmartUtility.GStandingCharges>();
        // Insert only - never Updated or Deleted
        internal List<SmartUtility.GUnitChargesSQLite> sqlite_g_unit_chargesList = new List<SmartUtility.GUnitChargesSQLite>();
        internal List<SmartUtility.GUnitCharges> g_unit_chargesList = new List<SmartUtility.GUnitCharges>();
        internal List<SmartUtility.GUnitCharges> g_unit_charges_changesList = new List<SmartUtility.GUnitCharges>();
        // Insert only - never Updated or Deleted
        internal List<SmartUtility.GUsageSQLite> sqlite_g_usageList = new List<SmartUtility.GUsageSQLite>();
        internal List<SmartUtility.GUsage> g_usageList = new List<SmartUtility.GUsage>();
        internal List<SmartUtility.GUsage> g_usage_changesList = new List<SmartUtility.GUsage>();
        // Insert and Update possible - never Deleted
        internal List<SmartUtility.LoginsSQLite> sqlite_utility_loginsList = new List<SmartUtility.LoginsSQLite>();
        internal List<SmartUtility.Logins> utility_loginsList = new List<SmartUtility.Logins>();
        internal List<SmartUtility.Logins> utility_logins_changesList = new List<SmartUtility.Logins>();
        // Insert only - never Updated or Deleted
        internal List<SmartUtility.MetersSQLite> sqlite_utility_metersList = new List<SmartUtility.MetersSQLite>();
        internal List<SmartUtility.Meters> utility_metersList = new List<SmartUtility.Meters>();
        internal List<SmartUtility.Meters> utility_meters_changesList = new List<SmartUtility.Meters>();
        // Insert only - never Updated or Deleted
        internal List<SmartUtility.PaymentsSQLite> sqlite_paymentsList = new List<SmartUtility.PaymentsSQLite>();
        internal List<SmartUtility.Payments> paymentsList = new List<SmartUtility.Payments>();
        internal List<SmartUtility.Payments> payments_changesList = new List<SmartUtility.Payments>();
        // Insert and Update possible - never Deleted
        internal List<SmartUtility.ResourcesSQLite> sqlite_utility_resourcesList = new List<SmartUtility.ResourcesSQLite>();
        // https://makolyte.com/system-invalidoperationexception-collection-was-modified-enumeration-operation-may-not-execute/
        internal List<SmartUtility.Resources> utility_resourcesList = new List<SmartUtility.Resources>();
        internal List<SmartUtility.Resources> utility_resources_changesList = new List<SmartUtility.Resources>();
        // Insert and Update possible - never Deleted
        internal List<SmartUtility.ResourcesTypesSQLite> sqlite_utility_resources_typesList = new List<SmartUtility.ResourcesTypesSQLite>();
        internal List<SmartUtility.ResourcesTypes> utility_resources_typesList = new List<SmartUtility.ResourcesTypes>();
        internal List<SmartUtility.ResourcesTypes> utility_resources_types_changesList = new List<SmartUtility.ResourcesTypes>();
        // Insert only - never Updated or Deleted
        internal List<SmartUtility.SupChargesCreditsSQLite> sqlite_supply_charges_creditsList = new List<SmartUtility.SupChargesCreditsSQLite>();
        internal List<SmartUtility.SupChargesCredits> supply_charges_creditsList = new List<SmartUtility.SupChargesCredits>();
        internal List<SmartUtility.SupChargesCredits> supply_charges_credits_changesList = new List<SmartUtility.SupChargesCredits>();
        // Insert and Update possible - never Deleted
        internal List<SmartUtility.SwitchesSQLite> sqlite_utility_switchesList = new List<SmartUtility.SwitchesSQLite>();
        internal List<SmartUtility.Switches> utility_switchesList = new List<SmartUtility.Switches>();
        internal List<SmartUtility.Switches> utility_switches_changesList = new List<SmartUtility.Switches>();
        // Insert only - never Updated or Deleted
        internal List<SmartUtility.TariffDetailsSQLite> sqlite_tariff_detailsList = new List<SmartUtility.TariffDetailsSQLite>();
        internal List<SmartUtility.TariffDetails> tariff_detailsList = new List<SmartUtility.TariffDetails>();
        internal List<SmartUtility.TariffDetails> tariff_details_changesList = new List<SmartUtility.TariffDetails>();
        // Insert and Delete possible - never Updated
        internal List<SmartUtility.UnallocatedSQLite> sqlite_unallocatedList = new List<SmartUtility.UnallocatedSQLite>();
        internal List<SmartUtility.Unallocated> unallocatedList = new List<SmartUtility.Unallocated>();
        internal List<SmartUtility.Unallocated> unallocated_changesList = new List<SmartUtility.Unallocated>();


        // Utility
        internal List<SmartUtility.AnalysisCosts> d_analysis_costsList = new List<SmartUtility.AnalysisCosts>();
        internal List<SmartUtility.TariffEngine> e_tariff_engineList = new List<SmartUtility.TariffEngine>();
        internal List<SmartUtility.TariffEngine> g_tariff_engineList = new List<SmartUtility.TariffEngine>();
        internal List<SmartUtility.Simulate> simulateList = new List<SmartUtility.Simulate>();
        internal List<SmartUtility.TariffCosts> tariff_costsList = new List<SmartUtility.TariffCosts>();
        internal List<SmartUtility.TariffCodesNamesView> tariff_codes_names_tempList = new List<SmartUtility.TariffCodesNamesView>();

        internal SmartUtility.Bills bills_row = new SmartUtility.Bills();
        internal SmartUtility.BillsResource bills_resource_row = new SmartUtility.BillsResource();

        // Write-Only/Insert section
        internal List<Amelia> ameliaList = new List<Amelia>();

        internal List<SmartUtility.Bills> bills_tempList = new List<SmartUtility.Bills>();
        internal List<SmartUtility.BillsResource> bills_resource_tempList = new List<SmartUtility.BillsResource>();
    };

    public class Tracey            // Different levels of tracing ...
    {
        internal bool scrape = false;
#if WINFORMS
        internal bool decode = false;
#endif
    }

#if WINFORMS
    public class DashBored
    {
        // Read-Only section
        internal Tracey examine = new Tracey();                 // 1 Log to trace file
        internal bool console;                                // 2 Log to console
    }
#endif


    internal class Electricity
    {
        internal string resource_type = "",                         // 44 Set to "SR" usually for E
                                                                    //udprn,                  // 8 numeric chars
                            MPAN = "",                               // 45
                            meter_serial_no = "",
                            contact_end_date = "",
                            energy_used = "",
                            personal_projection = "",
                            TCR = "";
        internal int tariff_code = 0;
        internal char payment_method = SmartParametersV2016.defaultChar;
    }

    internal class Gas
    {
        internal string resource_type = "",                         // 44 Set to "SR" usually for E
                                                                    //udprn,          // 8 numeric chars
                            MPRN = "",                               // 45
                            meter_serial_no = "",
                            contact_end_date = "",
                            energy_used = "",
                            personal_projection = "",
                            TCR = "";
        internal int tariff_code = 0;
        internal char payment_method = SmartParametersV2016.defaultChar;
    }

    public class GenericAddress
    {
        internal string value = "";
        internal string text = "";
        internal string organization = "";
        internal string sub_building_name = "";
        internal string building_number = "";
        internal string building_name = "";
        internal string thoroughfare = "";
        internal string dependant_thoroughfare = "";
        internal string dependant_locality = "";
        internal string double_dependant_locality = "";
        internal string code = "";
        internal string postcode = "";
        internal string pobox = "";
        internal string town = "";
        internal string county = "";
        internal string country = "";
        internal string udprn = "";       // 8 numeric chars
        internal Electricity electricity = new Electricity();
        internal Gas gas = new Gas();
    }


    //#if WINFORMS
    public class Aspnet_View
    {
        public string USERNAME
        {
            get;
            set;
        }
        public string EMAIL
        {
            get;
            set;
        }
        public string PASSWORD
        {
            get;
            set;
        }
        public DateTime EXPIRATION1
        {
            get;
            set;
        }
        public DateTime EXPIRATION2
        {
            get;
            set;
        }
        public bool SUBSCRIBER
        {
            get;
            set;
        }
        public bool TRACE
        {
            get;
            set;
        }
        public DateTime LASTLOGINDATE
        {
            get;
            set;
        }
        public bool MULTIPLEMETER
        {
            get;
            set;
        }
        public bool ADMINISTRATOR
        {
            get;
            set;
        }
        public string ID
        {
            get;
            set;
        }
        public string PASSWORDHASH
        {
            get;
            set;
        }
        public Aspnet_View(string Username,
                                string Email,
                                string ClearPassword,
                                DateTime Expiration1,
                                DateTime Expiration2,
                                bool Subscriber,
                                bool Trace,
                                DateTime LastLogon,
                                bool MultipleMeter,
                                bool Administrator,
                                string Id,
                                string PasswordHash
                                )
        {
            USERNAME = Username;
            EMAIL = Email;
            PASSWORD = ClearPassword;
            EXPIRATION1 = Expiration1;
            EXPIRATION2 = Expiration2;
            SUBSCRIBER = Subscriber;
            TRACE = Trace;
            LASTLOGINDATE = LastLogon;
            MULTIPLEMETER = MultipleMeter;
            ADMINISTRATOR = Administrator;
            ID = Id;
            PASSWORDHASH = PasswordHash;
        }
    }

#if WINFORMS
    public class Listener_View
    {
        public string USERNAME
        {
            get;
            set;
        }
        public string DATE
        {
            get;
            set;
        }
        public string BRAND_CODE
        {
            get;
            set;
        }
        public string SUPPLIER_CODE
        {
            get;
            set;
        }
        public string ROUTINE
        {
            get;
            set;
        }
        public string MESSAGE
        {
            get;
            set;
        }

        public Listener_View(string Username,
                                string Date,
                                string Brand_Code,
                                string Supplier_Code,
                                string Routine,
                                string Message)
        {
            USERNAME = Username;
            DATE = Date;
            BRAND_CODE = Brand_Code;
            SUPPLIER_CODE = Supplier_Code;
            ROUTINE = Routine;
            MESSAGE = Message;
        }
    }

    public class DatabaseView
    {
        public string SCHEMA_NAME
        { get; set; }
        public string TABLE_NAME
        { get; set; }
        public string MATCHES
        { get; set; }
        public string SOURCE_COUNT
        { get; set; }
        public string TARGET_COUNT
        { get; set; }

        public DatabaseView(string Schema_Name,
                                string Table_Name,
                                string Matches,
                                string Source_Count,
                                string Target_Count)
        {
            SCHEMA_NAME = Schema_Name;
            TABLE_NAME = Table_Name;
            MATCHES = Matches;
            SOURCE_COUNT = Source_Count;
            TARGET_COUNT = Target_Count;
        }
    }


#endif

    // We need Amelia because this structure feeds into Consumer Energy ... 
    // this has all the bits and pieces we need to do that ....
    internal class Amelia
    {
        internal char
            CUBEFACE_CODE = SmartParametersV2016.defaultChar;
        internal char
            RESOURCE_CODE = SmartParametersV2016.defaultChar;
        internal string
            RESOURCE_TYPE = "";
        internal short
            SUPPLIER_CODE = 0;
        internal short
            BRAND_CODE = 0;
        internal DateTime
            ACCOUNT_CREATED = SmartParametersV2016.defaultDate;
        internal string
            ACCOUNT_NO = "";
        internal string
            POSTCODE = "";       // <= This is a MUST ... or nothing works ...
        internal short
            AREA_CODE = 0;
        internal string
            MPAN_MPRN = "";
        internal string
            METER_SERIAL_NO = "";
        internal string
            UDPRN = "";           // 8 numeric chars
        internal short
            BANK_INSTITUTION_CODE = 0,
            BANK_BRAND_CODE = 0;
    }

    
    public class SmartUsers
    {
        internal class ExternalResources
        {
            internal short
                EXTERNAL_ID = 0;
            internal string
                EXTERNAL_NAME = "";
            internal string
                EXTERNAL_URL = "";
            internal string
                API_KEY = "";
            internal string
                API_QUERY = "";
#if WINFORMS
            internal string
                Description = "";     // Sometime ...
            internal DateTime
                Created = SmartParametersV2016.defaultDate,
                Updated = SmartParametersV2016.defaultDate;
            internal string
                Status_Flag = SmartParametersV2016.activeStatus;
            internal DateTime
                Deactivated = SmartParametersV2016.defaultDate;
#endif
        }


        public class ExchangeRates
        {
            internal DateTime
                TRANSACTION_DATE = SmartParametersV2016.defaultDate;
            internal int
                ECB = 0;                            // Did we load it from ECB (true) or derive it (false)
            internal decimal
                CURRENCY_RATE_01 = 0;                  // Pound sterling
            internal decimal
                CURRENCY_RATE_02 = 0;                  // Euro
            internal decimal
                CURRENCY_RATE_03 = 0;                  // USD
                                                       //internal decimal
                                                       //    CURRENCY_RATE_04 = 0;                  // CAD
            internal decimal
                CURRENCY_RATE_04 = 0;                  // JPY
#if WINFORMS
            internal DateTime
                Created = SmartParametersV2016.defaultDate,
                Updated = SmartParametersV2016.defaultDate;
            internal string
                Status_Flag = SmartParametersV2016.activeStatus;
            internal DateTime
                Deactivated = SmartParametersV2016.defaultDate;
#endif
        }

        public class ExchangeRatesView
        {
            public string TRANSACTION_DATE     // Valid date Oooh maybe Dan can see his Crypto in Euros?!?!
            { get; set; }
            public string ECB       // Did we load it from ECB (true/yes) or not (false/no)
            { get; set; }
            public decimal CURRENCY_RATE_01    // Pound sterling
            { get; set; }
            public decimal CURRENCY_RATE_02    // Euro
            { get; set; }
            public decimal CURRENCY_RATE_03    // USD
            { get; set; }
            //public decimal CURRENCY_RATE_04    // CAD
            //{ get; set; }
            public decimal CURRENCY_RATE_04    // JPY
            { get; set; }
        }

        internal class Postcodes
        {
            internal string
                POSTCODE = "";
            internal short
                AREA_CODE = 0;
#if WINFORMS
            internal string
                Location = "";     // Sometime ...
            internal DateTime
                Created = SmartParametersV2016.defaultDate,
                Updated = SmartParametersV2016.defaultDate;
            internal string
                Status_Flag = SmartParametersV2016.activeStatus;
            internal DateTime
                Deactivated = SmartParametersV2016.defaultDate;
#endif
        }

        internal class Working_Postcodes
        {
            internal string
                POSTCODE = "";
            internal short
                AREA_CODE = 0;
            internal Working_Postcodes(string Postcode,
                                        short Area_Code)
            {
                POSTCODE = Postcode;
                AREA_CODE = Area_Code;
            }
        }

        internal class InternalPostcodes
        {
            internal string
                POSTCODE = "";
            internal short
                AREA_CODE = 0;
        }

        internal class VatRates
        {
            internal short
                VAT_CODE = 0;       // Always 1 for the time being
            internal DateTime
                VALID_FROM = SmartParametersV2016.defaultDate,
                VALID_TO = SmartParametersV2016.defaultDate;
            internal string
                VAT_RATE = "";
#if WINFORMS
            internal DateTime
                Created = SmartParametersV2016.defaultDate,
                Updated = SmartParametersV2016.defaultDate;
            internal string
                Status_Flag = SmartParametersV2016.activeStatus;
            internal DateTime
                Deactivated = SmartParametersV2016.defaultDate;
#endif
        }

        public class ConsumerViews
        {
            public char CUBEFACE_CODE;
#if WINFORMS
            public System.Windows.Forms.UserControl View;
#endif
#if WPF
            public System.Windows.Controls.UserControl View;
#endif
#if WINUI
            public UserControl View;
#endif
#if ANDROIDX
            public View View;
#endif
#if SMARTMAUI
            public ContentView View;
#endif
        }
        public class ConsumersSQLite
        {
            //  I am tempted to taked the USERNAME out for Production,
            //  but am holding fire until I get Multi-Meters - who can manipulate
            //  lots of different Usernames sorted out ....


            // It might seem light you have a 'Tables' record for
            // each entry in COnsumers ... BUT you never ever log
            // in more than once at any one time with a Consumers
            // Primary key entry (even if you log in from a different
            // device).  So its perfectly acceptable to have
            // these entries

            public string
                USERNAME                               // Key to link to all of SmartSwitch
            { get; set; }
            public int
                LOCAL_ONLY
            { get; set; }
            public int
                FULL_SCREEN
            { get; set; }
            public int
                MULTIMETER_ACTIVE
            { get; set; }
            public DateTime
                CONSUMER_CREATED    // = SmartParametersV2016.defaultDate;                       // No links,no joins, just because we can
            { get; set; }
            public DateTime
                SmartUsersConsumers             // 1
            { get; set; }
            public DateTime
                SmartProfileCubefaces             // 2
            { get; set; }
            public DateTime
                SmartProfileGroups                // 3
            { get; set; }
            public DateTime
                SmartProfileProfiles              // 4
            { get; set; }
            public DateTime
                SmartProfileAddresses             // 5
            { get; set; }
            public DateTime
                SmartFinanceAccounts            // 6
            { get; set; }
            public DateTime
                SmartFinanceCryptoAccounts      // 7
            { get; set; }
            public DateTime
                SmartFinanceCryptoAddresses         // 8
            { get; set; }
            public DateTime
            SmartFinanceCryptoCurrencyRates      // 9
            { get; set; }
            public DateTime
            SmartFinanceCryptoLedgers               // 10
            { get; set; }
            public DateTime
            SmartFinanceCryptoTransactions          // 11
            { get; set; }
            public DateTime
            SmartFinanceCryptoWallets               // 12
            { get; set; }
            public DateTime
            SmartFinanceCryptoWalletTotals          // 13
            { get; set; }
            public DateTime
            SmartFinanceCategories                  // 14
            { get; set; }
            public DateTime
            SmartFinanceCategoryTypes               // 15
            { get; set; }
            public DateTime
            SmartFinanceConnections                 // 16
            { get; set; }
            public DateTime
            SmartFinanceLogins                      // 17
            { get; set; }
            public DateTime
            SmartFinanceSwitches                // 18
            { get; set; }
            public DateTime
            SmartFinanceTransactions            // 19
            { get; set; }
            public DateTime
            SmartFinanceTransactionsCategories  // 20
            { get; set; }
            public DateTime
            SmartUtilityAccChargesCredits       // 21
            { get; set; }
            public DateTime
            SmartUtilityAccounts                // 22
            { get; set; }
            public DateTime
            SmartUtilityBankDetails             // 23
            { get; set; }
            public DateTime
                SmartUtilityBills               // 24
            { get; set; }
            public DateTime
                SmartUtilityBillsResource       // 25
            { get; set; }
            public DateTime
                SmartUtilityECosts              // 26
            { get; set; }
            public DateTime
                SmartUtilityEDiscounts          // 27
            { get; set; }
            public DateTime
                SmartUtilityEReadings           // 28
            { get; set; }
            public DateTime
                SmartUtilityEStandingCharges    // 29
            { get; set; }
            public DateTime
                SmartUtilityEUnitCharges        // 30
            { get; set; }
            public DateTime
                SmartUtilityEUsage              // 31
            { get; set; }
            public DateTime
                SmartUtilityGCosts              // 32
            { get; set; }
            public DateTime
                SmartUtilityGDiscounts          // 33
            { get; set; }
            public DateTime
                SmartUtilityGReadings           // 34
            { get; set; }
            public DateTime
                SmartUtilityGStandingCharges    // 35
            { get; set; }
            public DateTime
                SmartUtilityGUnitCharges        // 36
            { get; set; }
            public DateTime
                SmartUtilityGUsage              // 37
            { get; set; }
            public DateTime
                SmartUtilityLogins              // 38
            { get; set; }
            public DateTime
                SmartUtilityMeters              // 39
            { get; set; }
            public DateTime
                SmartUtilityPayments            // 40
            { get; set; }
            public DateTime
                SmartUtilityResources           // 41
            { get; set; }
            public DateTime
                SmartUtilityResourcesTypes      // 42
            { get; set; }
            public DateTime
                SmartUtilitySupChargesCredits   // 43
            { get; set; }
            public DateTime
                SmartUtilitySwitches            // 44
            { get; set; }
            public DateTime
                SmartUtilityTariffDetails       // 45
            { get; set; }
            public DateTime
                SmartUtilityUnallocated         // 46
            { get; set; }
            public bool
                Updated = false;    // Lower case so it doesn't go out to the table
        }

        internal class Consumers
        {
            //  I am tempted to taked the USERNAME out for Production,
            //  but am holding fire until I get Multi-Meters - who can manipulate
            //  lots of different Usernames sorted out ....
            internal string
                USERNAME = "";                               // Key to link to all of SmartSwitch
            internal bool
                LOCAL_ONLY = false;
            internal bool
                FULL_SCREEN = false; 
            internal bool
                MULTIMETER_ACTIVE = false;
            internal DateTime
                CONSUMER_CREATED = SmartParametersV2016.defaultDate;                       // No links,no joins, just because we can
            internal DateTime
                SmartUsersConsumers = SmartParametersV2016.defaultDate;
            internal DateTime
                SmartProfileCubefaces = SmartParametersV2016.defaultDate;
            internal DateTime
                SmartProfileGroups = SmartParametersV2016.defaultDate;
            internal DateTime
                SmartProfileProfiles = SmartParametersV2016.defaultDate;
            //internal DateTime
            //    SmartProfileProfilesCube = SmartParametersV2016.defaultDate;
            internal DateTime
                SmartProfileAddresses = SmartParametersV2016.defaultDate;
            internal DateTime
                SmartFinanceAccounts = SmartParametersV2016.defaultDate;
            internal DateTime
                SmartFinanceCryptoAccounts = SmartParametersV2016.defaultDate;
            internal DateTime
                SmartFinanceCryptoAddresses = SmartParametersV2016.defaultDate;
            internal DateTime
                SmartFinanceCryptoCurrencyRates = SmartParametersV2016.defaultDate;
            internal DateTime
                SmartFinanceCryptoLedgers = SmartParametersV2016.defaultDate;
            internal DateTime
                SmartFinanceCryptoTransactions = SmartParametersV2016.defaultDate;
            internal DateTime
                SmartFinanceCryptoWallets = SmartParametersV2016.defaultDate;
            internal DateTime
                SmartFinanceCryptoWalletTotals = SmartParametersV2016.defaultDate;
            internal DateTime
                SmartFinanceCategories = SmartParametersV2016.defaultDate;
            internal DateTime
                SmartFinanceCategoryTypes = SmartParametersV2016.defaultDate;
            internal DateTime
                SmartFinanceConnections = SmartParametersV2016.defaultDate;
            internal DateTime
                SmartFinanceLogins = SmartParametersV2016.defaultDate;
            internal DateTime
                SmartFinanceSwitches = SmartParametersV2016.defaultDate;
            internal DateTime
                SmartFinanceTransactions = SmartParametersV2016.defaultDate;
            internal DateTime
                SmartFinanceTransactionsCategories = SmartParametersV2016.defaultDate;
            internal DateTime
                SmartUtilityAccChargesCredits = SmartParametersV2016.defaultDate;
            internal DateTime
                SmartUtilityAccounts = SmartParametersV2016.defaultDate;
            internal DateTime
                SmartUtilityBankDetails = SmartParametersV2016.defaultDate;
            internal DateTime
                SmartUtilityBills = SmartParametersV2016.defaultDate;
            internal DateTime
                SmartUtilityBillsResource = SmartParametersV2016.defaultDate;
            internal DateTime
                SmartUtilityECosts = SmartParametersV2016.defaultDate;
            internal DateTime
                SmartUtilityEDiscounts = SmartParametersV2016.defaultDate;
            internal DateTime
                SmartUtilityEReadings = SmartParametersV2016.defaultDate;
            internal DateTime
                SmartUtilityEStandingCharges = SmartParametersV2016.defaultDate;
            internal DateTime
                SmartUtilityEUnitCharges = SmartParametersV2016.defaultDate;
            internal DateTime
                SmartUtilityEUsage = SmartParametersV2016.defaultDate;
            internal DateTime
                SmartUtilityGCosts = SmartParametersV2016.defaultDate;
            internal DateTime
                SmartUtilityGDiscounts = SmartParametersV2016.defaultDate;
            internal DateTime
                SmartUtilityGReadings = SmartParametersV2016.defaultDate;
            internal DateTime
                SmartUtilityGStandingCharges = SmartParametersV2016.defaultDate;
            internal DateTime
                SmartUtilityGUnitCharges = SmartParametersV2016.defaultDate;
            internal DateTime
                SmartUtilityGUsage = SmartParametersV2016.defaultDate;
            internal DateTime
                SmartUtilityLogins = SmartParametersV2016.defaultDate;
            internal DateTime
                SmartUtilityMeters = SmartParametersV2016.defaultDate;
            internal DateTime
                SmartUtilityPayments = SmartParametersV2016.defaultDate;
            internal DateTime
                SmartUtilityResources = SmartParametersV2016.defaultDate;
            internal DateTime
                SmartUtilityResourcesTypes = SmartParametersV2016.defaultDate;
            internal DateTime
                SmartUtilitySupChargesCredits = SmartParametersV2016.defaultDate;
            internal DateTime
                SmartUtilitySwitches = SmartParametersV2016.defaultDate;
            internal DateTime
                SmartUtilityTariffDetails = SmartParametersV2016.defaultDate;
            internal DateTime
                SmartUtilityUnallocated = SmartParametersV2016.defaultDate;
            internal bool
                Include = true;        // You have to MAKE it true, Sunbeam!!
                                       // You can't just assume it to be so!
                                       // Yes you can assume it
                                       // What's the point of it unless its seen?

            internal bool
                Updated = false;    // Lower case so it doesn't go out to the table
        }

        public class SmartView
        {
            // All the rest have get/sets so the DataGrid Groups
            // can display them!  Yay!!
            public string USERNAME
            { get; set; }
            public List<object> BOLLOCKS
            { get; set; }
            public bool Delete { get; set; }

            public SmartView(string username,
                             List<object> bollocks)
            {
                this.USERNAME = username;
                this.BOLLOCKS = bollocks;
            }
            public SmartView() { }
        }

    }

    public class SmartProfile
    {
        public class CubefacesSQLite
        {
            //  I am tempted to taked the USERNAME out for Production,
            //  but am holding fire until I get Multi-Meters - who can manipulate
            //  lots of different Usernames sorted out ....
            public string
                USERNAME        // KEY to link to all of SmartSwitch
            { get; set; }
            public string
                CUBEFACE_CODE   // KEY to link to everything
            { get; set; }
            // This next field is the Consent Date which we need to check and
            // renew every 90 days; if the Local UTC date and time exceeds this field
            // by 90 days, then flag the FACE_ACTIVE = "" so that nothing can be done with Finance
            // When we make FACE_ACTIVE = "X" again then create a new record
            // so we have a history of consents ...
            public int
                FACE_ACTIVE         // Need this as bool to turn off Utility
                                    // when it hasn't been sent by ANNA
            { get; set; }
            // This is *NOT* a Key field!!!
            public DateTime
                CUBEFACE_CREATED    // No links,no joins, just because we can
            { get; set; }
            public string
                FACE_LAST_DISPLAY
            { get; set; }
            public string
                FACE_CULTURE_CODE
            { get; set; }
            public string
                FACE_AUTOSWITCH     // 'R'ed  'O'range or 'G'reen
            { get; set; }
            public short
                FACE_CURRENCY     // 0,1,2 or 3
            { get; set; }
            public DateTime
                NEXT_CONNECTION     // Every Cubeface has a Connection ... so we need
            { get; set; }       // just ONE connection for all Banks, Investments and
                                // Savings and - really - one for both Gas and Electricity
            public bool
                Updated;
        }

        public class Cubefaces
        {
            //  I am tempted to take the USERNAME out for Production,
            //  but am holding fire until I get Multi-Meters - who can manipulate
            //  lots of different Usernames sorted out ....
            public string
                USERNAME = "";                            // KEY to link to all of SmartSwitch
            public char
                CUBEFACE_CODE   // KEY to link to everything
            { get; set; }
            // This next field is the Consent Date which we need to check and
            // renew every 90 days; if the Local UTC date and time exceeds this field
            // by 90 days, then flag the FACE_ACTIVE = "" so that nothing can be done with Finance
            // When we make FACE_ACTIVE = "X" again then create a new record
            // so we have a history of consents ...

            public bool       // Because it is NEVER Empty!
                FACE_ACTIVE   // Need this as bool to turn off Utility
            { get; set; }             // when it hasn't been sent by ANNA
            // This is *NOT* a Key field!!!
            public DateTime
                CUBEFACE_CREATED = SmartParametersV2016.defaultDate;   // No links,no joins, just because we can
            public string     // Because its sometimes Empty!
                FACE_LAST_DISPLAY = "";
            public string
                FACE_CULTURE_CODE = "UK";
            public char     // Because its never Empty!
                FACE_AUTOSWITCH = SmartParametersV2016.defaultAutoswitch;     // 'R'ed  'O'range or 'G'reen
            public short
                FACE_CURRENCY = 0;  // Default 'No conversion'
            public DateTime
                NEXT_CONNECTION = SmartParametersV2016.defaultDate;
            // Every Cubeface has a Connection ... so we need
            // just ONE connection for all Banks, Investments and                                                                          //       // Savings and - really - one for both Gas and Electricity            
            internal bool
                Updated = false;
        }

        // So there will be one record for ANNA and one for MARIA
        public class Groups
        {
            //  I am tempted to taked the USERNAME out for Production,
            //  but am holding fire until I get 'MultiMeter' - who can manipulate
            //  lots of different Usernames sorted out ....
            public string
                USERNAME = "";    // Key to link to all of SmartSwitch
            public string
                GROUPNAME
            { get; set; }
            public bool
                ACTIVEFLAG
            { get; set; }
            public DateTime
                MARKER
            { get; set; }
            public string
                PDEK = "";
            public bool
                SENDF
            { get; set; } // List of schemas
            public string
                FDEK = "";
            public bool
                SENDU
            { get; set; } // List of schemas
            public string
                UDEK = "";
            public bool
                RECEIVEALL          // This used to be 'RECEIVE' but for some CHIMP-REASON, Sql Server didn't like that as a field name!!!!
            { get; set; }           // Usually *
            public string           // NEED TO BE **PUBLIC** TO BIND TO YOU CLUNKHEAD!!!
                DISPLAYNAME       // For some obscure reason its null in WPF            
            { get; set; }
            internal bool
                Updated = false;    // Lower case so it doesn't go out to the table
        }

        public class GroupsSQLite
        {
            //  I am tempted to taked the USERNAME out for Production,
            //  but am holding fire until I get Multi-Meters - who can manipulate
            //  lots of different Usernames sorted out ....
            public string
                USERNAME            // Key to link to all of SmartSwitch
            { get; set; }
            public string
                GROUPNAME
            { get; set; }
            public int
                ACTIVEFLAG
            { get; set; }
            public DateTime
                MARKER
            { get; set; }
            public string
                PDEK
            { get; set; }
            public int
                SENDF
            { get; set; }
            public string
                FDEK
            { get; set; }
            public int
                SENDU
            { get; set; }
            public string
                UDEK
            { get; set; }
            public int
                RECEIVEALL  // All because fucking SQL Server couldn't deal with a column called 'RECEIVE'!!!
            { get; set; }
            public string
                DISPLAYNAME
            { get; set; }
            public bool
                Updated = false;
        }


        public class AddressesViewSQLite
        {
            public string
                USERNAME
            { get; set; }
            public string
                KEY_DETAILS
            { get; set; }
            public int
                RANDOMKEY1
            { get; set; }
            public DateTime
                ADDRESS_CREATED
            { get; set; }
            public string
                DETAILS
            { get; set; }
            public int
                RANDOMKEY2
            { get; set; }
            public bool
                Updated = false;
        }
        internal class AddressesView
        {
            internal string
                USERNAME = "";
            internal string
                UDPRN = "";       // KEY 8 numeric chars OR email (may not be unique!)
            internal int
                RANDOMKEY1 = 0;
            internal DateTime
                ADDRESS_CREATED = SmartParametersV2016.defaultDate;
            internal short
                AREA_CODE = 0;    // Should be moved into AddressesView (it is)
            internal string
                BASIC = "";       // The one scraped from website (Bank, Utility, Water, Community Charge)
            internal string
                BUILDING_NAME = "",
                BUILDING_NUMBER = "",
                COUNTY = "",
                DOUBLE_DEPENDANT_LOCALITY = "",
                DEPENDANT_THOROUGHFARE = "",
                DEPENDANT_LOCALITY = "",
                ORGANIZATION = "",
                POSTCODE_OUTWARD = "",
                POSTCODE = "",
                POBOX = "",
                SUB_BUILDING_NAME = "",
                THOROUGHFARE = "",
                TOWN = "";
            internal int
                RANDOMKEY2 = 0;
            internal bool
                CHECKED = true;
            internal bool
                Updated = false;    // Lower case so it doesn't go out to the table
        }

        internal class Profiles
        {
            internal string
                USERNAME = "";
            internal DateTime
                PROFILE_CREATED = SmartParametersV2016.defaultDate;
            internal string
                LASTNAME = "",         // Entered by User
                FIRSTNAME = "",       //      "
                MIDDLENAME = "",      //      "
                DATE_OF_BIRTH = "";   //      "
            internal int RANDOMKEY1 = 0;
            internal string TITLE = "", //      "
                GENDER = "",          //      "
                CONTACT_NO = "",      //      "
                EMAIL_ADDRESS = "",   //      "
                DISPLAYNAME = "";
            internal int RANDOMKEY2 = 0;
            internal bool
                Updated = false;    // Lower case so it doesn't go out to the table
        }

        public class ProfilesSQLite
        {
            public string
                USERNAME
            { get; set; }
            public DateTime
                PROFILE_CREATED
            { get; set; }
            public string
                KEY_DETAILS
            { get; set; }
            public int
                RANDOMKEY1
            { get; set; }
            public string
                DETAILS
            { get; set; }
            public int
                RANDOMKEY2
            { get; set; }
            public bool
                Updated = false;
        }

        //internal class ProfilesCube
        //{
        //    internal string
        //        USERNAME = "";
        //    internal DateTime
        //        PROFILE_CREATED = SmartParametersV2016.defaultDate;
        //    internal char
        //        CUBEFACE_CODE = SmartParametersV2016.defaultChar;       // KEY to link to everything
        //    internal string
        //        NAME = "",            //      Scraped by us
        //        DOB = "",             //          "
        //        PHONE_NO = "",        //          "
        //        EMAIL_ADDRESS = "";   //
        //    internal int
        //        RANDOMKEY = 0;
        //    internal bool
        //        Updated = false;    // Lower case so it doesn't go out to the table"
        //}

        //public class ProfilesCubeSQLite
        //{
        //    public string
        //        USERNAME
        //    { get; set; }
        //    public DateTime
        //        PROFILE_CREATED
        //    { get; set; }
        //    public string
        //        CUBEFACE_CODE
        //    { get; set; }
        //    public string
        //        DETAILS
        //    { get; set; }
        //    public int
        //        RANDOMKEY
        //    { get; set; }
        //    public bool
        //        Updated = false;
        //}

        //internal class ProfilesView
        //{
        //    internal string
        //        USERNAME = "";
        //    internal DateTime
        //        PROFILE_CREATED = SmartParametersV2016.defaultDate;
        //    internal char
        //        CUBEFACE_CODE = SmartParametersV2016.defaultChar;       // KEY to link to everything
        //    internal string
        //        LASTNAME = "",         // Entered by User
        //        FIRSTNAME = "",       //      "
        //        MIDDLENAME = "",      //      "
        //        DATE_OF_BIRTH = "",   //      "
        //        TITLE = "",           //      "
        //        GENDER = "",          //      "
        //        CONTACT_NO = "",      //      "
        //        EMAIL_ADDRESS = "",   //      "
        //        DISPLAY_NAME = "";
        //    internal string
        //       NAME = "",            //      Scraped by us
        //       DOB = "",             //          "
        //       PHONE_NO = "",        //          "
        //       EMAIL = "";           //
        //}
    }

    public class TableName      // Used in SmartPhyll
    {
        public TableName() { }
        public string Name { get; set; }
        public DateTime Updated { get; set; }
    }

    public class SmartFinance   // public for XML Parser
    {
        //internal struct Transaction
        //{
        //    internal string bank_id;         // Bank id (key)
        //    internal string account_id;
        //    internal string transaction_id;
        //
        //    internal string creditdebit_indicator;
        //    internal string transaction_status;
        //}

        internal class SelectedProviders
        {
            // These come from the USER
            // This is only used in SmartDashboard (because its a struct, it doesn't use any space!)
            public string USERNAME = "";
            public char CUBEFACE_CODE = SmartParametersV2016.defaultChar;
            public short INSTITUTION_CODE = 0;
            public short BRAND_CODE = 0;
        }

        internal class SelectedAccounts
        {
            public string USERNAME = "";
            public char CUBEFACE_CODE = SmartParametersV2016.defaultChar;
            public short INSTITUTION_CODE = 0;
            public short BRAND_CODE = 0;
            public string SORTCODE = "";
            public string ACCOUNT_NO = "";
            //public char CATEGORY_CODE = SmartParametersV2016.defaultChar;
            public string UDPRN = "";
            public short CURRENCY_ORDINAL = 0;
            //public string ACCOUNT_TITLE = ""; / Dont need this? Are we ever going to select based on title?
        }

        internal class SelectedTransactionGroups
        {
            internal string TYPE = "";
            //public bool ISCHECKED = false;
        }
        internal class SelectedTransactionTypes
        {
            // These come from SMARTCUBEMOBILE
            public int CREDITDEBIT_INDICATOR = 0;
            public bool ISCHECKED = false;
        }

        public class AccountsSQLite
        {
            public string
                USERNAME            // KEY
            { get; set; }
            public string
                CUBEFACE_CODE       // KEY
            { get; set; }
            public short
                INSTITUTION_CODE    // KEY
            { get; set; }
            public short
                BRAND_CODE          // KEY
            { get; set; }
            public string
                KEY_DETAILS         // KEY (SortCode and Account_No AND UDPRN)
            { get; set; }
            public DateTime
                ACCOUNT_CREATED     //  KEY
            { get; set; }
            public string
                CATEGORY_CODE       // KEY
            { get; set; }
            public int
                RANDOMKEY1
            { get; set; }
            public string
                DETAILS             // Contains Account_Title, Currency, Account_Balance, Status
            { get; set; }
            public int
                RANDOMKEY2
            { get; set; }
            public bool
                Updated = false;
        }

        internal class Accounts
        {
            internal string
                USERNAME = "";                                          // KEY
            internal char
                CUBEFACE_CODE = SmartParametersV2016.defaultChar;       // KEY
            internal short
                INSTITUTION_CODE = 0;                                   // KEY
            internal short
                BRAND_CODE = 0;                                         // KEY
            internal string
                SORTCODE = "";                                          // KEY
            internal string
                ACCOUNT_NO = "";                                        // KEY
            //  The fucking UDPRN ....
            //    This IS the address where the Payer the bank account lives (in theory! Ray?!?)
            //      This DOESN'T go on the Account per se, it goes on the Provider (cos the same address
            //      applies to lots of Accounts)  and we are going to have to scrape a statement in order
            //      to extract the address, cos the cunts don't show it anywhere else)
            //    No, it appears we DON'T have to scrape a statement 'cos (well, at least
            //      for NW) the address appears in the 'Change Address' screen!
            //      But the first statement still applies; the Address is allocated to
            //      the Provider ... 'cos when we log in to NW they only have ONE
            //      'address field' which apples to all accounts
            //    Nope.
            //       Its possible for a Provider to accept that a User
            //       has more than one address! They may have two accounts
            //       with the same provider, but at different addresses!
            //       All you can be guaranteed is that an Account is linked
            //       to AN address, so the Account has to have room for a UDPRN
            //       (even though it might be the same for all Accounts)
            //
            //   And the same account can have MORE THAN ONE Address (change of address)
            internal string
                UDPRN = "";                                             // KEY!!
            internal DateTime
                ACCOUNT_CREATED = SmartParametersV2016.defaultDate;     // KEY
            internal int
                RANDOMKEY1 = 0;
            internal char
                CATEGORY_CODE = SmartParametersV2016.defaultChar;       // *NOT* in Details
            internal short
                CURRENCY_ORDINAL = 0;                                   // In Details
            internal string
                ACCOUNT_TITLE = "";                                     // In Details       
            internal int
                ACCOUNT_BALANCE = 0;                                    // In Details
            internal char
                STATUS = SmartParametersV2016.defaultChar;              // In Details Open = 'O', Switching = 'S', Closed = 'N'
            // ALL of this has gone in the Crypt Accounts as a trailer
            //internal string EXCHANGE = "";      // SORTCODE
            //internal DateTime CREATED_AT = SmartParametersV2016.defaultDate;    // ACCOUNT_CREATED
            //internal string ACTIVE = "";
            //internal string AVAILABLE_BALANCE_VALUE = "0";
            //internal string AVAILABLE_BALANCE_CURRENCY = "0";
            //internal string CURRENCY = "0";     // CURRENCY_ORDINAL
            //internal string DEFAULT = "0";
            //internal DateTime DELETED_AT = SmartParametersV2016.defaultDate;
            //internal string HOLD_VALUE = "0";
            //internal string HOLD_CURRENCY = "0";
            //internal string NAME = "";
            //internal string PLATFORM = "";
            //internal string READY = "";
            //internal string RETAIL_PORTFOLIO_ID = "";
            //internal string TYPE = "";
            //internal DateTime UPDATED_AT = SmartParametersV2016.defaultDate;
            //internal string UUID = "";          // ACCOUNT_NO
            internal int RANDOMKEY2 = 0;                                // For Details
            internal bool Updated = false;
        }

        // These is no CREATION_DATE on each of these, simply
        // because I can't think of a need for it!  Yes, we can
        // Update the PTC_CODE (Personal Transaction Code) but ...
        // .. that's all? Don't ever need to sort these by
        // 'creation_date' so why include it??
        public class TransactionsSQLite     // Header
        {
            public string
                USERNAME        // KEY
            { get; set; }
            public string
                CUBEFACE_CODE   // KEY
            { get; set; }
            public short
                INSTITUTION_CODE    // KEY                          
            { get; set; }
            public short
                BRAND_CODE      // KEY
            { get; set; }
            public string
                KEY_DETAILS     // KEY
            { get; set; }
            public int
                RANDOMKEY1      // KEY contains SORTCODE and ACCOUNT_NO
            { get; set; }
            public bool
                Updated = false;
        }

        internal class TransactionsView // Header
        {
            internal string USERNAME = "";                                      // KEY
            internal char CUBEFACE_CODE = SmartParametersV2016.defaultChar;   // KEY
            internal short INSTITUTION_CODE = 0;                               // KEY                         
            internal short BRAND_CODE = 0;                                     // KEY
            internal string SORTCODE = "";                                      // KEY Wrong? => NO needed because different account nos on a statement
            internal string ACCOUNT_NO = "";                                    // KEY
            internal string ACCOUNT_NAME = "";
            internal string UDPRN = "";                                         // KEY
            internal DateTime STATEMENT_DATE = SmartParametersV2016.defaultDate;  // KEY
            internal short STATEMENT_NO = 0;                                   // KEY
        }
        internal class Transactions // Header
        {
            internal string
                USERNAME = "";                                      // KEY
            internal char
                CUBEFACE_CODE = SmartParametersV2016.defaultChar;   // KEY
            internal short
                INSTITUTION_CODE = 0;                               // KEY                         
            internal short
                BRAND_CODE = 0;                                     // KEY
            internal string
                SORTCODE = "";                                      // KEY Wrong? => NO needed because different account nos on a statement
            internal string
                ACCOUNT_NO = "";                                    // KEY
            internal string
                UDPRN = "";                                         // KEY
            internal DateTime
                STATEMENT_DATE = SmartParametersV2016.defaultDate;  // KEY
            internal short
                STATEMENT_NO = 0;                                   // KEY
            internal int
                RANDOMKEY1 = 0;
            internal bool
                Updated = false;
        }

        public class TransactionsCategoriesSQLite
        {
            public string
                USERNAME            // KEY
            { get; set; }
            public string
                CUBEFACE_CODE       // KEY
            { get; set; }
            public short
                INSTITUTION_CODE    // KEY                          
            { get; set; }
            public short
                BRAND_CODE          // KEY
            { get; set; }
            public string
                KEY_DETAILS         // KEY contains SORTCODE, ACCOUNT_NO and UDPRN
            { get; set; }
            public long              // ChatGPT says SQLite will use this to handle 64-bit longs
                SEQUENCE_NO        // KEY
            { get; set; }
            public int
                RANDOMKEY1
            { get; set; }
            public string
                DETAILS             // Encrypted information
            { get; set; }
            public int
                RANDOMKEY2
            { get; set; }
            public bool
                Updated = false;
        }
        internal class TransactionsCategories : IComparable<TransactionsCategories>
        {
            internal string
                USERNAME = "";                                      // KEY
            internal char
                CUBEFACE_CODE = SmartParametersV2016.defaultChar;   // KEY
            internal short
                INSTITUTION_CODE = 0;                               // KEY                         
            internal short
                BRAND_CODE = 0;                                     // KEY
            internal string
                SORTCODE = "";                                      // KEY
            internal string
                ACCOUNT_NO = "";                                    // KEY Needed because you have different account nos on a statement
            internal string
                UDPRN = "";                                         // KEY Yes - ACCOUNT_NO doesn't change but UDPRN might
            internal DateTime
                STATEMENT_DATE = SmartParametersV2016.defaultDate;  // KEY
            internal short
                STATEMENT_NO = 0;                                   // KEY
            internal long           // Ticks
                SEQUENCE_NO = 0;                                    // KEY
            internal int
                RANDOMKEY1 = 0;
            internal DateTime
                TRANSACTION_DATE = SmartParametersV2016.defaultDate;
            internal int
                CREDITDEBIT_INDICATOR = 0;
            internal int
                PTC_CODE = 0;       // See the notes: Personal Transaction Categorization Code
                                    // This code allows the Transaction to be updated
                                    // and then acts as a 'permanent' PTC for the user
            internal short
                TRANSGROUP_CODE = 0;
            internal short
                TRANSACTION_CODE = 0;
            internal string
                DESCRIPTION = "";
            internal string
                TYPE = "";                          // Crypto
            internal double
                CRYPTO_AMOUNT = 0;
            internal short
                CRYPTO_CURRENCY_ORDINAL = 0;
            internal double
                AMOUNT = 0;
            internal short
                AMOUNT_CURRENCY_ORDINAL = 0;
            internal double
                BALANCE_AMOUNT = 0;
            internal short
                BALANCE_CURRENCY_ORDINAL = 0;
            internal int
                BALANCE_CREDITDEBIT_INDICATOR = 0;
            internal int
                RANDOMKEY2 = 0;
            internal decimal[]
                EXCHANGE_RATES = new decimal[4] { 0, 0, 0, 0 };
            // Crypto shit
            //internal string ACTIVITY = "";  // Crypto
            //internal string EXCHANGE = "";
            //internal string CRYPTO_RATE = "";
            //internal string CRYPTO_CURRENCY_DISPLAY = "";  // The corresponding GBP or whatever
            //internal double NATIVE_AMOUNT = 0;
            //internal string NATIVE_CURRENCY = "";
            //internal double FEE_AMOUNT = 0;
            //internal string FEE_CURRENCY = "";
            //internal int CRYPTO_BALANCE = 0;
            //internal string TOTAL_COST = "";
            //internal string TO_ADDRESS = "";
            internal bool Updated = false;

            #region Icomparable<TransactionsCategories> Members
            public int CompareTo(TransactionsCategories other)
            {
                // Need to test this breaking change!!!
                int compareResult = 0;

                if (other != null)
                {
                    if (TRANSACTION_DATE < other.TRANSACTION_DATE)
                    {
                        compareResult = -1;
                    }
                    else
                    {
                        if (TRANSACTION_DATE > other.TRANSACTION_DATE)
                        {
                            compareResult = 1;
                        }
                        else
                        {
                            // If the Transaction Dates are equal then compare the CREATED_DATEs
                            //if (TRANSACTION_CREATED < other.TRANSACTION_CREATED)
                            //{
                            //    compareResult = -1;
                            //}
                            //else
                            //{
                            //    if (TRANSACTION_CREATED > other.TRANSACTION_CREATED)
                            //    {
                            //        compareResult = 1;
                            //    }
                            //    else
                            //    {
                            // The Transaction Dates are equal so are the Created Dates then
                            // compare the Sequence Nos ...
                            if (SEQUENCE_NO < other.SEQUENCE_NO)
                            {
                                compareResult = -1;
                            }
                            else
                            {
                                if (SEQUENCE_NO > other.SEQUENCE_NO)
                                {
                                    compareResult = 1;
                                }
                            }
                        }
                    }
                }
                return compareResult;
            }

            public static int Compare(TransactionsCategories left, TransactionsCategories right)
            {
                if (object.ReferenceEquals(left, right))
                {
                    return 0;
                }
                if (left is null)
                {
                    return -1;
                }
                return left.CompareTo(right);
            }

            public override bool Equals(object obj)
            {
                TransactionsCategories other = (TransactionsCategories)obj; //avoid double casting
                if (other is null)
                {
                    return false;
                }
                return this.CompareTo(other) == 0;
            }

            public static bool operator ==(TransactionsCategories left, TransactionsCategories right)
            {
                if (left is null)
                {
                    return right is null;
                }
                return left.Equals(right);
            }
            public static bool operator !=(TransactionsCategories left, TransactionsCategories right)
            {
                return !(left == right);
            }
            public static bool operator <(TransactionsCategories left, TransactionsCategories right)
            {
                return (Compare(left, right) < 0);
            }
            public static bool operator >(TransactionsCategories left, TransactionsCategories right)
            {
                return (Compare(left, right) > 0);
            }
            public override int GetHashCode()
            {
                //Get hash code for the TRANSACTION_DATE field.
                int hashTransaction = TRANSACTION_DATE.GetHashCode();
                //Get hash code for Unix Milliseconds;
                int hashSequence = SEQUENCE_NO.GetHashCode();
                //Calculate the hash code for the product.
                return hashTransaction ^
                        hashSequence;
            }
            #endregion
        }

        public class CategoriesSQLite
        {
            //  I am tempted to taked the USERNAME out for Production,
            //  but am holding fire until I get Multi-Meters - who can manipulate
            //  lots of different Usernames sorted out ....
            public string
                USERNAME    // = "";                                    // KEY to link to all of SmartSwitch
            { get; set; }
            public string
                CUBEFACE_CODE   // = SmartParametersV2016.defaultChar;           // KEY to link to everything
            { get; set; }
            public string
                CATEGORY_CODE   // = SmartParametersV2016.defaultChar; // KEY to link to generated RADIOBUTTON
            { get; set; }
            public string
                CHECKED
            { get; set; }
            public bool
                Updated = false;
        }

        internal class Categories
        {
            //  I am tempted to taked the USERNAME out for Production,
            //  but am holding fire until I get Multi-Meters - who can manipulate
            //  lots of different Usernames sorted out ....
            internal string
                USERNAME = "";                                    // KEY to link to all of SmartSwitch
            internal char
                CUBEFACE_CODE = SmartParametersV2016.defaultChar;           // KEY to link to everything
            internal char
                CATEGORY_CODE = SmartParametersV2016.defaultChar; // KEY to link to generated RADIOBUTTON
            internal string     // Because it might be EMpty!!
                CHECKED = "";
            internal bool
                Updated = false;
        }

        public class CategoryTypesSQLite
        {
            //  I am tempted to taked the USERNAME out for Production,
            //  but am holding fire until I get Multi-Meters - who can manipulate
            //  lots of different Usernames sorted out ....
            public string
                USERNAME    // = "";                                    // KEY to link to all of SmartSwitch
            { get; set; }
            public string
                CUBEFACE_CODE   // = SmartParametersV2016.defaultChar;           // KEY to link to everything
            { get; set; }
            public string
                CATEGORY_CODE   // = SmartParametersV2016.defaultChar; // KEY to link to generated RADIOBUTTON
            { get; set; }
            public short
                CURRENCY_ORDINAL
            { get; set; }
            public bool
                Updated = false;
        }

        internal class CategoryTypes
        {
            //  I am tempted to taked the USERNAME out for Production,
            //  but am holding fire until I get Multi-Meters - who can manipulate
            //  lots of different Usernames sorted out ....
            internal string
                USERNAME = "";                                    // KEY to link to all of SmartSwitch
            internal char
                CUBEFACE_CODE = SmartParametersV2016.defaultChar;           // KEY to link to everything
            internal char
                CATEGORY_CODE = SmartParametersV2016.defaultChar; // KEY to link to generated RADIOBUTTON
            internal short     // Because it might be Empty!!
                CURRENCY_ORDINAL = 0;
            internal bool
                Updated = false;
        }

        public class ConnectionsSQLite
        {
            //  I am tempted to taked the USERNAME out for Production,
            //  but am holding fire until I get Multi-Meters - who can manipulate
            //  lots of different Usernames sorted out ....
            public string
                USERNAME        // KEY to link to all of SmartSwitch      
            { get; set; }
            public string
                CUBEFACE_CODE       // KEY to link to everything
            { get; set; }
            public short
                INSTITUTION_CODE    // Key - should be unique
            { get; set; }
            public short
                BRAND_CODE          // Key - should be unique
            { get; set; }
            public int
                LOGIN_METHOD        // Key - should be unique
            { get; set; }           // but may not be consecutive (i.e. it will have gaps)
            public string
                ACTIVE_FLAG
            { get; set; }
            public string
                DETAILS        // Connection set
            { get; set; }
            public int
                RANDOMKEY
            { get; set; }
            public bool
                Updated = false;                  // Lower case so it doesn't go out to the table
            public bool
                Delete = false;                  // Lower case so it doesn't go out to the table
        }
        public class Connections
        {
            //  I am tempted to taked the USERNAME out for Production,
            //  but am holding fire until I get Multi-Meters - who can manipulate
            //  lots of different Usernames sorted out ....
            internal string
                USERNAME = "";              // KEY to link to all of SmartSwitch
            internal char
                CUBEFACE_CODE = SmartParametersV2016.defaultChar;              // KEY to link to everything
            internal short
                INSTITUTION_CODE            // Key - should be unique
            { get; set; }
            internal short
                BRAND_CODE                  // Key - should be unique
            { get; set; }
            public int
                LOGIN_METHOD                // Key - should be unique
            { get; set; }                   // but may not be consecutive (i.e. it will have gaps)
            internal char
                ACTIVE_FLAG = SmartParametersV2016.defaultChar;
            internal string
                DETAILS
            { get; set; }
            internal int
                RANDOMKEY = 0;
            internal bool
               Updated = false;                  // Lower case so it doesn't go out to the table
            internal bool
               Delete = false;                  // Lower case so it doesn't go out to the table
                                                // 🔴 THIS IS THE FIX
                                                // Prevents Spinner ItemSelected firing during bind
            public int SelectedBrandIndex { get; set; } = -1;
            public int SelectedLengthIndex { get; set; } = -1;
            public int SelectedLoginMethod { get; set; } = -1;
            public bool SpinnerInitialized { get; set; }
            public bool ProviderSelectionInitialized { get; set; } = false; public int ProviderIndex { get; set; } = -1;
            public int LoginMethodIndex { get; set; } = -1;
            
            public bool HasSingleProvider { get; set; }
            public string SingleProviderName { get; set; }
            public List<BrandItem> AvailableBrands { get; set; }
            public List<int> AvailableLoginMethods { get; set; } = new List<int>();
            public string InstitutionName { get; set; }
            public string BrandName { get; set; }
            public BrandItem SelectedBrand { get; set; }
            public bool IsPendingCompletion { get; set; }
            public bool IsNew { get; set; }
            public bool IsLoginMethodLocked { get; set; }
            public bool UserFinalizedLoginMethod { get; set; }
            public string Parameter1 { get; set; } // E-mail possibly
            public string Parameter2 { get; set; } // APIKey Public This the User can change
            public string Parameter3 { get; set; } // APIKey Secret not if its an APISECRET
#if WPF  || WINUI || SMARTMAUI
            public Visibility Parameter1Visibility { get; set; }
            public Visibility Parameter2Visibility { get; set; }
            public Visibility Parameter3Visibility { get; set; }
#endif
#if ANDROIDX
            public ViewStates Parameter1Visibility { get; set; }
            public ViewStates Parameter2Visibility { get; set; }
            public ViewStates Parameter3Visibility { get; set; }
#endif
        }

        public class LoginsSQLite
        {
            //  I am tempted to taked the USERNAME out for Production,
            //  but am holding fire until I get Multi-Meters - who can manipulate
            //  lots of different Usernames sorted out ....
            public string
                USERNAME            // KEY to link to all of SmartSwitch
            { get; set; }
            public string
                CUBEFACE_CODE       // KEY to link to everything
            { get; set; }
            public short
                INSTITUTION_CODE    // KEY
            { get; set; }
            public short
                BRAND_CODE          // KEY                                            // KEY
            { get; set; }
            public int
                LOGIN_METHOD          // KEY
            { get; set; }
            public string
                ACTIVE_FLAG
            { get; set; }
            public string
                DETAILS             // Gets decrypted - Contains Owner, Contact phone no(s) and Contact e-mail address
            { get; set; }
            public int
                RANDOMKEY
            { get; set; }
            public int
                BANKSCHECKED
            { get; set; }
            public int
                SAVINGSCHECKED
            { get; set; }
            public int
                INVESTMENTSCHECKED
            { get; set; }
            public int
                CRYPTOSCHECKED
            { get; set; }
            public bool
                Updated = false;    // Lower case so it doesn't go out to the table
            public bool
                Delete = false;    // Lower case so it doesn't go out to the table
        }
        public class Logins
        {
            //  I am tempted to taked the USERNAME out for Production,
            //  but am holding fire until I get Multi-Meters - who can manipulate
            //  lots of different Usernames sorted out ....
            internal string USERNAME = "";                                      // KEY to link to all of SmartSwitch
            internal char CUBEFACE_CODE = SmartParametersV2016.defaultChar;   // KEY to link to everything
            internal short INSTITUTION_CODE
            { get; set; }                   // KEY
            internal short BRAND_CODE
            { get; set; }                   // KEY
            public int LOGIN_METHOD         // KEY
            { get; set; }
            internal char ACTIVE_FLAG = SmartParametersV2016.defaultChar;
            
            public bool BANKSCHECKED { get; set; }
            public bool SAVINGSCHECKED { get; set; }
            public bool INVESTMENTSCHECKED { get; set; }
            public bool CRYPTOSCHECKED { get; set; }
            internal string
                DETAILS
            { get; set; }
            // Returned from the scrape!
            internal string OWNER = "";             // Returned from scrape!             // This is fixed by the Bank (?)
            internal string CONTACT_EMAIL = "";     // This can change. Why would this change?  Because they change their e-mail address!
            internal string CONTACT_PHONENO = "";   // This can change. Why would this change?  Because they change their phone no!
            internal int RANDOMKEY = 0;
            internal string EXCHANGE = "";

            internal bool Updated = false;              // Lower case so it doesn't go out to the table
            internal bool Delete = false;               // Lower case so it doesn't go out to the table
            public int LoginMethodIndex { get; set; }

            public string Parameter1 { get; set; }
            public string BrandName { get; set; }
            public string InstitutionName { get; set; }

#if WPF  || WINUI || SMARTMAUI
            public Visibility BanksVisibility { get; set; }
            public Visibility SavingsVisibility { get; set; }
            public Visibility InvestmentsVisibility { get; set; }
            public Visibility CryptosVisibility { get; set; }
#endif
#if ANDROIDX
            public ViewStates BanksVisibility { get; set; }
            public ViewStates SavingsVisibility { get; set; }
            public ViewStates InvestmentsVisibility { get; set; }
            public ViewStates CryptosVisibility { get; set; }
#endif
            public List<DanApiKey> apiKeys { get; set; }
            public List<BrandItem> AvailableBrands { get; set; }
            public BrandItem SelectedBrand { get; set; }
            public bool IsNew { get; set; }
            public bool IsLoginMethodLocked { get; set; }
            public bool UserFinalizedLoginMethod { get; set; }
            public List<int> AvailableLoginMethods { get; set; } = new(); // list for the ComboBox
            public List<EntryItem> AvailableConnections { get; set; }
        }

        public class Parent
        {
            public Logins A { get; set; }
            public Brands B { get; set; }
            public List<SmartFinance.DanApiKey> ApiKeys { get; set; }
            public Parent()
            {
                A = new Logins();
                B = new Brands();
                ApiKeys = new List<DanApiKey>();
            }
        }
        public class SwitchesSQLite
        {
            //  I am tempted to taked the USERNAME out for Production,
            //  but am holding fire until I get Multi-Meters - who can manipulate
            //  lots of different Usernames sorted out ....
            public string USERNAME            // KEY to link to all of SmartSwitch
            { get; set; }
            public string CUBEFACE_CODE       // KEY to link to everything
            { get; set; }
            public short INSTITUTION_CODE    // KEY
            { get; set; }
            public short BRAND_CODE          // KEY
            { get; set; }
            public string CATEGORY_CODE       // KEY to link to all of SmartSwitch
            { get; set; }
            public DateTime SWITCH_CREATED      // as it doesn't appear in any JOINs?? Think its a KEY now tho
            { get; set; }
            public string KEY_DETAILS         // KEY contains ACCOUNT_TYPE, SORTCODE and ACCOUNT_NO                                                           
            { get; set; }
            public int RANDOMKEY
            { get; set; }
            public string AUTOSWITCH
            { get; set; }
            public DateTime LAST_DATETIME
            { get; set; }
            public DateTime LAST_UPDATE
            { get; set; }
            public DateTime EXPIRY_DATE
            { get; set; }
            public string AUTOSWITCH_DESTINATION      // Where we sent the e-mail
            { get; set; }
            public short AUTOSWITCH_INSTITUTION_CODE // Which supplier we chose to switch to
            { get; set; }
            public short AUTOSWITCH_BRAND_CODE       // Which brand code we chose to switch to
            { get; set; }
            public DateTime AUTOSWITCH_EMAIL_SENT       // When we sent the e-mail
            { get; set; }
            public bool Updated = false;
        }

        internal class Switches
        {
            //  I am tempted to taked the USERNAME out for Production,
            //  but am holding fire until I get Multi-Meters - who can manipulate
            //  lots of different Usernames sorted out ....
            internal string
                USERNAME = "";                                      // KEY to link to all of SmartSwitch
            internal char
                CUBEFACE_CODE = SmartParametersV2016.defaultChar;   // KEY to link to everything
            internal short
                INSTITUTION_CODE = 0;                               // KEY
            internal short
                BRAND_CODE = 0;                                     // KEY
            internal char
                CATEGORY_CODE = SmartParametersV2016.defaultChar;   // KEY?
            internal DateTime
                SWITCH_CREATED = SmartParametersV2016.defaultDate;  // KEY? *not* a Key! as it doesn't appear in any JOINs            
            internal string
                SORTCODE = "";                                      // KEY
            internal string
                ACCOUNT_NO = "";                                    // KEY
            internal string
                UDPRN = "";        // 8 numeric chars Which will - in all probability - change
                                   // This IS the address where the Payer the bank account lives (in theory! Ray?!?)
            internal short
                CURRENCY_ORDINAL = 0;
            internal int
                RANDOMKEY = 0;
            internal char       // Because its never Empty
                AUTOSWITCH = SmartParametersV2016.defaultChar;
            internal DateTime
                LAST_DATETIME = SmartParametersV2016.defaultDate;
            internal DateTime
                LAST_UPDATE = SmartParametersV2016.defaultDate;
            internal DateTime
                EXPIRY_DATE = SmartParametersV2016.defaultDate;
            internal string
                AUTOSWITCH_DESTINATION = "";                // Where we sent the e-mail
            internal short
                AUTOSWITCH_INSTITUTION_CODE = 0;            // Which supplier we chose to switch to
            internal short
                AUTOSWITCH_BRAND_CODE = 0;                  // Which brand code we chose to switch to
            internal DateTime
                AUTOSWITCH_EMAIL_SENT = SmartParametersV2016.defaultDate;     // When we sent the e-mail
            internal bool
                Updated = false;
        }

        internal class CategoryCodes
        {
            internal char
                CUBEFACE_CODE = SmartParametersV2016.defaultChar;       // KEY Usually 'F' for Finance
            internal char
                CATEGORY_CODE = SmartParametersV2016.defaultChar;     // KEY This is 'B', 'I', 'S' or 'C'
            internal short
                ORDINAL = 0;
            internal string
                DESCRIPTION = "";
#if WINFORMS
            internal DateTime
                Created = SmartParametersV2016.defaultDate,
                Updated = SmartParametersV2016.defaultDate;
            internal string
                Status_Flag = SmartParametersV2016.activeStatus;
            internal DateTime
                Deactivated = SmartParametersV2016.defaultDate;
#endif
        }

        // Crypto begins
        public class DanApiKey
        {
            public string Username { get; set; }
            public string Exchange { get; set; }
            public string AccessId { get; set; }
            public string ApiKey { get; set; }
            public string ApiSecret { get; set; }
        }

        public class CryptoAccountsView
        {
            // All the rest have get/sets so the ListView Accounts
            // can display them!  Yay!!
            public string USERNAME              // Key
            { get; set; }
            public string EXCHANGE              // Key
            { get; set; }
            public DateTime CREATED_AT          // Key
            { get; set; }
            public string ACTIVE                // Key
            { get; set; }
            public string CURRENCY              // Key
            { get; set; }
            public string TYPE                  // Key
            { get; set; }
            public string UUID
            { get; set; }
            public string AVAILABLE_BALANCE_VALUE
            { get; set; }
            public string AVAILABLE_BALANCE_CURRENCY
            { get; set; }
            public string DEFAULT
            { get; set; }
            public DateTime DELETED_AT
            { get; set; }
            public string HOLD_VALUE
            { get; set; }
            public string HOLD_CURRENCY
            { get; set; }
            public string NAME
            { get; set; }
            public string PLATFORM
            { get; set; }
            public string READY
            { get; set; }
            public string RETAIL_PORTFOLIO_ID
            { get; set; }
            public DateTime UPDATED_AT
            { get; set; }
            public string UDPRN
            { get; set; }

        }

        public class CryptoAddressesView
        {
            public string USERNAME
            { get; set; }
            public string EXCHANGE
            { get; set; }
            public string UUID
            { get; set; }
            public string ADDRESS
            { get; set; }
            public string ADDRESS_INFO_ADDRESS
            { get; set; }
            public string ADDRESS_LABEL
            { get; set; }
            public string CALLBACK_URL
            { get; set; }
            public DateTime CREATED_AT
            { get; set; }
            public string DEFAULT_RECEIVE
            { get; set; }
            public string DEPOSIT_URI
            { get; set; }
            public string DESTINATION_TAG
            { get; set; }
            public string ID
            { get; set; }
            public string INLINE_WARNING_TEXT
            { get; set; }
            public string INLINE_WARNING_TOOLTIP
            { get; set; }
            public string NAME
            { get; set; }
            public string NETWORK
            { get; set; }
            public string QR_CODE_IMAGE_URL
            { get; set; }
            public string RECEIVE_SUBTITLE
            { get; set; }
            public string RESOURCE
            { get; set; }
            public string RESOURCE_PATH
            { get; set; }
            public string SHARE_ADDRESS_COPY_LINE1
            { get; set; }
            public string SHARE_ADDRESS_COPY_LINE2
            { get; set; }
            public DateTime UPDATED_AT
            { get; set; }
            public string URI_SCHEME
            { get; set; }
        }

        public class CryptoExchangeRatesView
        {
            public string USERNAME
            { get; set; }
            public DateTime TRANSACTION_DATE
            { get; set; }
            public short CURRENCY_ORDINAL
            { get; set; }
            public string CRYPTO_CURRENCY
            { get; set; }
            public double CRYPTO_RATE
            { get; set; }

        }

        public class CryptoLedgersSQLite
        {
            // All the rest have get/sets so the DataGridView Transactions
            // can display them!  Yay!!
            public string USERNAME
            { get; set; }
            public string CUBEFACE_CODE
            { get; set; }
            public short INSTITUTION_CODE
            { get; set; }
            public short BRAND_CODE
            { get; set; }
            public string LEDGER_NAME
            { get; set; }
            public string WALLET
            { get; set; }
            public string ACCOUNT
            { get; set; }
            public DateTime TRANSACTION_DATE
            { get; set; }
            public double AMOUNT
            { get; set; }
            public string SOURCE
            { get; set; }
            public string DESTINATION
            { get; set; }
            public string CURRENCY_DISPLAY
            { get; set; }
            public string FEE
            { get; set; }
            public string TRANSACTION_TYPE
            { get; set; }
            public string TO_ADDRESS
            { get; set; }
            public string NETWORK_NAME
            { get; set; }
            public string HASH
            { get; set; }

            //public string Flags { get; set; }
            //public string Hash { get; set; }
            //public string InLedger { get; set; }
            //public string LasLedgerSequence { get; set; }
            //public string LedgerIndex { get; set; }
            //public string Memos { get; set; }
            //public string Sequence { get; set; }
            //public string SigningPubKey { get; set; }

            public bool Updated;
        }

        public class CryptoLedgers
        {
            // All the rest have get/sets so the DataGridView Transactions
            // can display them!  Yay!!
            public string USERNAME = "";                                    // Key
            public char CUBEFACE_CODE = SmartParametersV2016.defaultChar;   // Key
            public short INSTITUTION_CODE;                                  // Key
            public short BRAND_CODE;                                        // Key
            public string LEDGER_NAME  // As Network                                     // Key
            { get; set; }
            public string WALLET                                            // Key
            { get; set; }
            public string ACCOUNT = "";                                 // Key
            public DateTime TRANSACTION_DATE                                // Key
            { get; set; }
            public double AMOUNT                                       // Key
            { get; set; }
            public string SOURCE                                      // Key
            { get; set; }
            public string DESTINATION                                 // Key
            { get; set; }
            public string CURRENCY_DISPLAY
            { get; set; }
            public string FEE = "";
            public string TRANSACTION_TYPE
            { get; set; }
            public string TO_ADDRESS = "";
            public string NETWORK_NAME = "";
            public string HASH = "";
            //public string Flags { get; set; }
            //public string Hash { get; set; }
            //public string InLedger { get; set; }
            //public string LasLedgerSequence { get; set; }
            //public string LedgerIndex { get; set; }
            //public string Memos { get; set; }
            //public string Sequence { get; set; }
            //public string SigningPubKey { get; set; }
            public bool Updated = false;
            public CryptoLedgers Clone()
            {
                return new CryptoLedgers
                {
                    USERNAME = this.USERNAME,
                    CUBEFACE_CODE = this.CUBEFACE_CODE,
                    INSTITUTION_CODE = this.INSTITUTION_CODE,
                    BRAND_CODE = this.BRAND_CODE,
                    LEDGER_NAME = this.LEDGER_NAME,
                    WALLET = this.WALLET,
                    ACCOUNT = this.ACCOUNT,
                    TRANSACTION_DATE = this.TRANSACTION_DATE,
                    AMOUNT = this.AMOUNT,
                    SOURCE = this.SOURCE,
                    DESTINATION = this.DESTINATION,
                    CURRENCY_DISPLAY = this.CURRENCY_DISPLAY,
                    FEE = this.FEE,
                    TRANSACTION_TYPE = this.TRANSACTION_TYPE,
                    TO_ADDRESS = this.TO_ADDRESS,
                    NETWORK_NAME = this.NETWORK_NAME,
                    HASH = this.HASH
                };
            }
        }

        public class CryptoTransactionsViewSQLite
        {
            // All the rest have get/sets so the DataGridView Transactions
            // can display them!  Yay!!
            public string USERNAME
            { get; set; }
            public string CUBEFACE_CODE
            { get; set; }
            public short INSTITUTION_CODE
            { get; set; }
            public short BRAND_CODE
            { get; set; }
            public string KEY_DETAILS
            { get; set; }
            public long SEQUENCE_NO
            { get; set; }
            public int RANDOMKEY1
            { get; set; }
            public string DETAILS
            { get; set; }
            public int RANDOMKEY2
            { get; set; }
            public bool Updated;
        }

        public class CryptoTransactionsView
        {
            // All the rest have get/sets so the DataGridView Transactions
            // can display them!  Yay!!
            public string USERNAME
            { get; set; }
            public char CUBEFACE_CODE = SmartParametersV2016.defaultChar;
            public short INSTITUTION_CODE;
            public short BRAND_CODE;
            public string EXCHANGE //SORTCODE
            { get; set; }
            public string UUID //ACCOUNT_NO
            { get; set; }
            public string UDPRN
            { get; set; }
            public DateTime CREATED_AT
            { get; set; }
            public long SEQUENCE_NO
            { get; set; }
            public int RANDOMKEY1 = 0;
            public string TYPE
            { get; set; }
            public string CRYPTO_CURRENCY
            { get; set; }
            public short CRYPTO_CURRENCY_ORDINAL;          // This is the ordinal
            //public string EXCHANGE
            //{ get; set; }
            //public int PTC_CODE;
            public double CRYPTO_AMOUNT
            { get; set; }
            public double CRYPTO_RATE
            { get; set; }
            public string CURRENCY_DISPLAY  // The corresponding GBP or whatever
            { get; set; }
            //public short TRANSGROUP_CODE
            //{ get; set; }
            //public short TRANSACTION_CODE
            //{ get; set; }
            public int CREDITDEBIT_INDICATOR;
            public double NATIVE_AMOUNT
            { get; set; }
            public string NATIVE_CURRENCY
            { get; set; }
            public double FEE_AMOUNT   // All because format "C;C;;" or "'C';'C';;" doesn't work
            { get; set; }
            public string FEE_CURRENCY      // All because format "C;C;;" or "'C';'C';;" doesn't work
            { get; set; }
            //public int BALANCE;
            public string TOTAL_COST
            { get; set; }
            public string TO_ADDRESS
            { get; set; }
            public string NETWORK_NAME
            { get; set; }
            public int RANDOMKEY2 = 0;
            public string HASH
            { get; set; }

            public DateTime ACCOUNT_CREATED = SmartParametersV2016.defaultDate;
            public bool Processed = false;
            public bool Updated = false;
        }

        public class CryptoWalletsSQLite
        {
            // All the rest have get/sets so the DataGridView Transactions
            // can display them!  Yay!!
            public string USERNAME          // Key
            { get; set; }
            public string CUBEFACE_CODE     // Key
            { get; set; }
            public string CRYPTO_DESTINATION        // Key
            { get; set; }
            public string CRYPTO_NAME
            { get; set; }
            public double BALANCE
            { get; set; }
            public bool Updated;
        }

        public class CryptoWallets
        {
            // All the rest have get/sets so the DataGridView Transactions
            // can display them!  Yay!!
            public string USERNAME
            { get; set; }
            public char CUBEFACE_CODE
            { get; set; }
            public string CRYPTO_DESTINATION
            { get; set; }
            public string CRYPTO_NAME
            { get; set; }
            public double BALANCE
            { get; set; }
            internal bool Updated = false;
        }

        public class CryptoWalletTotalsSQLite
        {
            // All the rest have get/sets so the DataGridView Transactions
            // can display them!  Yay!!
            public string USERNAME          // Key
            { get; set; }
            public string CUBEFACE_CODE     // Key
            { get; set; }
            public short INSTITUTION_CODE   // Key
            { get; set; }
            public short BRAND_CODE         // Key
            { get; set; }
            public string ADDRESS           // Key
            { get; set; }
            public string CRYPTO_WALLET_NAME        // Key
            { get; set; }
            public short CRYPTO_CURRENCY_ORDINAL
            { get; set; }
            public string CRYPTO_CURRENCY
            { get; set; }
            public double CRYPTO_AMOUNT
            { get; set; }
            public double RATE
            { get; set; }
            public string VALUE
            { get; set; }
            public bool Updated;
        }
        public class CryptoWalletTotals
        {
            // All the rest have get/sets so the DataGridView Transactions
            // can display them!  Yay!!
            public string USERNAME          // Key
            { get; set; }
            public char CUBEFACE_CODE       // Key
            { get; set; }
            public short INSTITUTION_CODE   // Key
            { get; set; }
            public short BRAND_CODE         // Key
            { get; set; }
            public string ADDRESS           // Key
            { get; set; }
            public string CRYPTO_WALLET_NAME        // Key
            { get; set; }
            public short CRYPTO_CURRENCY_ORDINAL
            { get; set; }
            public string CRYPTO_CURRENCY
            { get; set; }
            public double CRYPTO_AMOUNT
            { get; set; }
            public double RATE
            { get; set; }
            public string VALUE
            { get; set; }
            internal bool Updated = false;
        }

        public class CryptoAccountsSQLite
        {
            // All the rest have get/sets so the DataGridView Transactions
            // can display them!  Yay!!
            public string USERNAME          // Key
            { get; set; }
            public string CUBEFACE_CODE     // Key
            { get; set; }
            public short INSTITUTION_CODE   // Key
            { get; set; }
            public short BRAND_CODE         // Key
            { get; set; }
            public string SORTCODE          // Key EXCHANGE
            { get; set; }
            public string ACCOUNT_NO        // Key UUID
            { get; set; }
            public string UDPRN             // Key
            { get; set; }
            //public short CURRENCY_ORDINAL   // Key
            //{ get; set; }
            public DateTime CREATED_AT
            { get; set; }
            public string DETAILS
            { get; set; }

            public bool Updated;
        }
        internal class CryptoAccounts
        {
            // All the rest have get/sets so the DataGridView Transactions
            // can display them!  Yay!!
            internal string USERNAME = "";
            internal char CUBEFACE_CODE = SmartParametersV2016.defaultChar;
            internal short INSTITUTION_CODE = 0;
            internal short BRAND_CODE = 0;
            internal string SORTCODE = "";              // Key EXCHNAGE
            internal string ACCOUNT_NO = "";            // Key UUID
            internal string UDPRN = "";                 // Key UDPRN
            //internal short CURRENCY_ORDINAL = 0;        // Key CURRENCY_ORDINAL
            internal DateTime CREATED_AT = SmartParametersV2016.defaultDate; // Key ACCOUNT_CREATED
            // Extra Crypto Account trailer
            //internal string DETAILS = "";
            //internal string ACTIVE = "";
            internal string TYPE = "";
            internal string AVAILABLE_BALANCE_VALUE = "0";
            internal string AVAILABLE_BALANCE_CURRENCY = "0";
            internal string DEFAULT = "0";
            internal DateTime DELETED_AT = SmartParametersV2016.defaultDate;
            internal string HOLD_VALUE = "0";
            internal string HOLD_CURRENCY = "0";
            internal string NAME = "";
            internal string Platform = "";
            internal string READY = "";
            internal string RETAIL_PORTFOLIO_ID = "";
            internal DateTime UPDATED_AT = SmartParametersV2016.defaultDate;
            internal bool Updated = false;
        }

        public class CryptoAddressesSQLite
        {
            // All the rest have get/sets so the DataGridView Transactions
            // can display them!  Yay!!
            public string USERNAME
            { get; set; }
            public string CUBEFACE_CODE
            { get; set; }
            public short INSTITUTION_CODE
            { get; set; }
            public short BRAND_CODE
            { get; set; }
            public string SORTCODE
            { get; set; }
            public string ACCOUNT_NO
            { get; set; }
            public string UDPRN
            { get; set; }
            //public short CURRENCY_ORDINAL
            //{ get; set; }
            public string DETAILS
            { get; set; }
            public bool Updated;
        }
        internal class CryptoAddresses
        {
            // All the rest have get/sets so the DataGridView Transactions
            // can display them!  Yay!!
            internal string USERNAME = "";
            internal char CUBEFACE_CODE = SmartParametersV2016.defaultChar;
            internal short INSTITUTION_CODE = 0;
            internal short BRAND_CODE = 0;
            internal string SORTCODE = "";
            internal string ACCOUNT_NO = "";
            internal string UDPRN = "";
            //internal short CURRENCY_ORDINAL = 0;
            internal string ADDRESS = "";
            internal string ADDRESS_INFO_ADDRESS = "";
            internal string ADDRESS_LABEL = "";
            internal string CALLBACK_URL = "";
            internal DateTime CREATED_AT = SmartParametersV2016.defaultDate;
            internal string DEFAULT_RECEIVE = "";
            internal string DEPOSIT_URI = "";
            internal string DESTINATION_TAG = "";
            internal string ID = "";
            internal string INLINE_WARNING_TEXT = "";
            internal string INLINE_WARNING_TOOLTIP = "";
            internal string NAME = "";
            internal string NETWORK = "";
            internal string QR_CODE_IMAGE_URL = "";
            internal string RECEIVE_SUBTITLE = "";
            internal string RESOURCE = "";
            internal string RESOURCE_PATH = "";
            internal string SHARE_ADDRESS_COPY_LINE1 = "";
            internal string SHARE_ADDRESS_COPY_LINE2 = "";
            internal DateTime UPDATED_AT = SmartParametersV2016.defaultDate;
            internal string URI_SCHEME = "";
            internal bool Updated = false;
        }

        public class CryptoCurrencyRatesSQLite
        {
            public string USERNAME
            { get; set; }
            public string CUBEFACE_CODE
            { get; set; }
            public short CRYPTO_ORDINAL
            { get; set; }
            public double CRYPTO_RATE
            { get; set; }
            public DateTime CRYPTO_RATE_DATE
            { get; set; }
            public bool Updated;
        }
        internal class CryptoCurrencyRates
        {
            internal string USERNAME = "";
            internal char CUBEFACE_CODE = SmartParametersV2016.defaultChar;
            internal short CRYPTO_ORDINAL = 0;
            internal double CRYPTO_RATE = 0;
            internal DateTime CRYPTO_RATE_DATE = SmartParametersV2016.defaultDate;
            internal bool Updated = false;
        }

        // Crypto ends

        internal class BrandAccounts
        {
            internal char
                CUBEFACE_CODE = SmartParametersV2016.defaultChar;       // KEY Usually 'F' for Finance
            internal short
                INSTITUTION_CODE = 0,                                   // KEY
                BRAND_CODE = 0;                                         // KEY
            internal char
                CATEGORY_CODE = SmartParametersV2016.defaultChar;       // KEY This is 'B', 'I', 'S' or 'C'
#if WINFORMS
            internal DateTime
                Created = SmartParametersV2016.defaultDate,
                Updated = SmartParametersV2016.defaultDate;
            internal string
                Status_Flag = SmartParametersV2016.activeStatus;
            internal DateTime
                Deactivated = SmartParametersV2016.defaultDate;
#endif
        }

        internal class BrandConnection
        {
            internal char
                CUBEFACE_CODE = SmartParametersV2016.defaultChar;    // KEY  Usually 'F' for Finances
            internal short
                INSTITUTION_CODE = 0; // KEY
            // Think this is needed so I can have different
            // number of Login_Methods for each brand
            internal short
                BRAND_CODE = 0;     // Key
            internal int
                LOGIN_METHOD = 0;                                         // KEY Because Barclays have more than one way of signing in =:-{
            internal short
                ORDINAL = 0;                                        // KEY
            internal string
                PARAMETER_PROMPT = "";
            internal int
                PARAMETER_LENGTH = 0;
            internal string
                PARAMETER_TYPE = "";
            internal string
                PARAMETER_HINT = "";
#if WINFORMS
            internal DateTime
                  Created = SmartParametersV2016.defaultDate,
                  Updated = SmartParametersV2016.defaultDate;
            internal string
                   Status_Flag = SmartParametersV2016.activeStatus;
            internal DateTime
                   Deactivated = SmartParametersV2016.defaultDate;
#endif
        }

        internal class BrandMatrix
        {
            internal char
               CUBEFACE_CODE = SmartParametersV2016.defaultChar;    // KEY  Usually '
            internal short
                INSTITUTION_CODE = 0;                               // KEY
            internal short
                BRAND_CODE = 0;                                     // KEY
            internal char
                AREA_01 = SmartParametersV2016.defaultChar,
                AREA_02 = SmartParametersV2016.defaultChar,
                AREA_03 = SmartParametersV2016.defaultChar,
                AREA_04 = SmartParametersV2016.defaultChar,
                AREA_05 = SmartParametersV2016.defaultChar;
#if WINFORMS
            internal DateTime
                Created = SmartParametersV2016.defaultDate,
                Updated = SmartParametersV2016.defaultDate;
            internal string
                Status_Flag = SmartParametersV2016.activeStatus;
            internal DateTime
                Deactivated = SmartParametersV2016.defaultDate;
#endif
        }

//        internal class BrandOrdinals
//        {
//            internal char
//               CUBEFACE_CODE = SmartParametersV2016.defaultChar;    // KEY  Usually 'F' for Finances
//            internal short
//                INSTITUTION_CODE = 0,                               // KEY
//                BRAND_CODE = 0;                                     // KEY
//            internal short
//                ORDINAL = 0;                                        // KEY            
//#if WINFORMS

//            internal DateTime
//                  Created = SmartParametersV2016.defaultDate,
//                  Updated = SmartParametersV2016.defaultDate;
//            internal string
//                   Status_Flag = SmartParametersV2016.activeStatus;
//            internal DateTime
//                   Deactivated = SmartParametersV2016.defaultDate;
//#endif
//        }

        public class Brands
        {
            internal char
                CUBEFACE_CODE = SmartParametersV2016.defaultChar;   // KEY  Usually 'F' for Finances
            internal short
                INSTITUTION_CODE = 0,                               // KEY
                BRAND_CODE = 0;                                     // KEY
            internal char
                ACTIVE_FLAG = SmartParametersV2016.activeDefault;
            internal string
                BRAND_NAME = "";
            internal string
                DESCRIPTION = "",
                ALTERNATIVE_NAME = "",
                DASHBOARD_NAME = "";
            internal string
                URL_PREFIX = "",
                NOTIFICATION_TITLE = "",
                NOTIFICATION_PREFIX = "";
            internal short
                NOTIFICATION_TAGLENGTH = 0;
            internal char
                TWOFACTOR_FLAG = SmartParametersV2016.defaultChar;
            internal DateTime
                VALID_FROM = SmartParametersV2016.defaultDate,
                VALID_TO = SmartParametersV2016.defaultDate;
            //internal string
            //    CLIENT_ID = "",
            //    CLIENT_SECRET = "",
            //    URL_BASE_SANDBOX = "",
            //    URL_BASE_PRODUCTION = "";
            //internal char
            //    BROWSER_FLAG = SmartParametersV2016.defaultChar;   // Used for WebDriver or API
#if WINFORMS
            internal DateTime
                  Created = SmartParametersV2016.defaultDate,
                  Updated = SmartParametersV2016.defaultDate;
            internal string
                   Status_Flag = SmartParametersV2016.activeStatus;
            internal DateTime
                   Deactivated = SmartParametersV2016.defaultDate;
#endif
        }

        public class BrandsView
        {
            public char
                CUBEFACE_CODE   // KEY  Usually 'F' for Finances
            { get; set; }
            public short
                INSTITUTION_CODE
            { get; set; }       // KEY
            public short
                BRAND_CODE
            { get; set; }// KEY
            public char
                ACTIVE_FLAG
            { get; set; }
            public string
                BRAND_NAME
            { get; set; }
            public char
                CATEGORY_CODE
            { get; set; }
            public short
                ORDINAL
            { get; set; }
            public string
               DESCRIPTION
            { get; set; }
        }

        // These are defined by SmartSwitch and are INSERTED depending on which
        // TYPE of information is being recorded - they are *NOT* defined by the User
        internal class BrandAreas
        {
            internal char
                CUBEFACE_CODE = SmartParametersV2016.defaultChar;   // KEY
            internal short
                AREA_CODE = 0;                                      // KEY
            internal string
                AREA_NAME = "";
#if WINFORMS
            internal string
                Comment = "";
            internal DateTime
                Created = SmartParametersV2016.defaultDate,
                Updated = SmartParametersV2016.defaultDate;
            internal string
                    Status_Flag = SmartParametersV2016.activeStatus;
            internal DateTime
                    Deactivated = SmartParametersV2016.defaultDate;
#endif
        }

        internal class InstitutionInfo
        {
            internal char
                CUBEFACE_CODE = SmartParametersV2016.defaultChar;   // KEY Usually 'F' for Finances
            internal short
                INSTITUTION_CODE = 0;                               // KEY
            internal string
                INSTITUTION_NAME = "",
                DETAILS = "";
            internal string
                OWNER = "",
                OWNER_COUNTRY = "";
#if WINFORMS
            internal DateTime
                Created = SmartParametersV2016.defaultDate,
                Updated = SmartParametersV2016.defaultDate;
            internal string
                Status_Flag = SmartParametersV2016.activeStatus;
            internal DateTime
                Deactivated = SmartParametersV2016.defaultDate;
#endif
        }

        internal class Institutions
        {
            internal char
                CUBEFACE_CODE = SmartParametersV2016.defaultChar;   // KEY Usually 'F' for Finances
            internal short
                INSTITUTION_CODE = 0;                               // KEY
            internal char
                ACTIVE_FLAG = SmartParametersV2016.activeDefault;
            internal string
                CULTURE_CODE = "";
            //internal bool
            //    TWOFACTOR = false;
            //internal string
            //    URL_PREFIX = "",
            //    NOTIFICATION_TITLE = "",
            //    NOTIFICATION_PREFIX = "";
            //internal short
            //    NOTIFICATION_TAG_LENGTH = 0;
            internal float
                BROWSER_TIMEOUT = 0;
            internal string
                USE_PROXY = "";
            internal DateTime
                VALID_FROM = SmartParametersV2016.defaultDate,
                VALID_TO = SmartParametersV2016.defaultDate;
            internal string
                CONTACT_EMAIL = "",
                CONTACT_TEL_NO = "";
#if WINFORMS
            internal string Comment = "";
            internal DateTime
                Created = SmartParametersV2016.defaultDate,
                Updated = SmartParametersV2016.defaultDate;
            internal string
                Status_Flag = SmartParametersV2016.activeStatus;
            internal DateTime
                Deactivated = SmartParametersV2016.defaultDate;
#endif
        }

        //        internal class Banking
        //        {
        //            internal string
        //                ORGANIZATION_ID = "";                     // KEY
        //            internal string
        //                ORGANIZATION_NAME = "";
        //            internal string
        //                REDIRECT_URI = "";
        //#if WINFORMS
        //            internal DateTime
        //                Created = SmartParametersV2016.defaultDate,
        //                Updated = SmartParametersV2016.defaultDate;
        //            internal string
        //                Status_Flag = SmartParametersV2016.activeStatus;
        //            internal DateTime
        //                Deactivated = SmartParametersV2016.defaultDate;
        //#endif
        //        }

        internal class Templates
        {
            internal char
                CUBEFACE_CODE = SmartParametersV2016.defaultChar;   // KEY Usually 'F' for Finance
            internal string
                WHAT = "",                                // KEY
                TEXT = "";
#if WINFORMS
            internal DateTime
                Created = SmartParametersV2016.defaultDate,
                Updated = SmartParametersV2016.defaultDate;
            internal string
                   Status_Flag = SmartParametersV2016.activeStatus;
            internal DateTime
                   Deactivated = SmartParametersV2016.defaultDate;
#endif
        }

        internal class Transaction_Flows
        {
            internal char
               CUBEFACE_CODE = SmartParametersV2016.defaultChar;    // KEY Usually 'F' for Finances
                                                                    // Transaction Flows transcend these next two/three?
                                                                    //internal short
                                                                    //    INSTITUTION_CODE = 0,                               // KEY
                                                                    //    BRAND_CODE = 0;                                     // KEY
                                                                    //    CATEGORY_CODE = ''                                // KEY

            internal char
               CATEGORY_CODE = SmartParametersV2016.defaultChar;
            internal int
                CREDITDEBIT_INDICATOR = 0;                      // KEY
            internal string
                DESCRIPTION = "";
            internal string
                COMMENT = "";
#if WINFORMS
            internal DateTime
                Created = SmartParametersV2016.defaultDate,
                Updated = SmartParametersV2016.defaultDate;
            internal string
                   Status_Flag = SmartParametersV2016.activeStatus;
            internal DateTime
                   Deactivated = SmartParametersV2016.defaultDate;
#endif
        }

        internal class Transaction_Groups
        {
            internal char
               CUBEFACE_CODE = SmartParametersV2016.defaultChar;    // KEY Usually 'F' for Finances
            internal short
                INSTITUTION_CODE = 0,                               // KEY
                BRAND_CODE = 0;                                     // KEY
            internal char
                CATEGORY_CODE = SmartParametersV2016.defaultChar;   // KEY Usually 'B' for Bank Transactions
            internal short
                TRANSGROUP_CODE = 0;                                // KEY
            internal int
                CREDITDEBIT_INDICATOR = 0;
            internal string
                AMOUNT_TYPE = "";
            internal string
                COMMENT = "";
#if WINFORMS
            internal DateTime
                Created = SmartParametersV2016.defaultDate,
                Updated = SmartParametersV2016.defaultDate;
            internal string
                   Status_Flag = SmartParametersV2016.activeStatus;
            internal DateTime
                   Deactivated = SmartParametersV2016.defaultDate;
#endif
        }

        internal class Transaction_Types
        {
            internal char
               CUBEFACE_CODE = SmartParametersV2016.defaultChar;    // KEY Usually 'F' for Finances
            internal short
                INSTITUTION_CODE = 0,                               // KEY
                BRAND_CODE = 0;                                     // KEY
            internal char
                CATEGORY_CODE = SmartParametersV2016.defaultChar;   // KEY Usually 'B' for Bank Transactions
            internal short
                TRANSGROUP_CODE = 0;                               // KEY
            internal short
                TRANSACTION_CODE = 0;                               // KEY
            internal string
                TRANSACTION_TYPE = "";                    // *not* a Key!
            internal int
                CREDITDEBIT_INDICATOR = 0;
            internal string
                DESCRIPTION = "";
            internal string
                COMMENT = "";
#if WINFORMS
            internal DateTime
                Created = SmartParametersV2016.defaultDate,
                Updated = SmartParametersV2016.defaultDate;
            internal string
                Status_Flag = SmartParametersV2016.activeStatus;
            internal DateTime
                Deactivated = SmartParametersV2016.defaultDate;
#endif
        }

        public class TotalsView
        {
            public string DESCRIPTION { get; set; }  // Which currency
            public string AMOUNT { get; set; }       // Total of the above currency
        }

        public class CommonTransactionsView
        {
            // All the rest have get/sets so the DataGridView Transactions
            // can display them!  Yay!!
            public string USERNAME
            { get; set; }
            public char CUBEFACE_CODE;
#if WINFORMS
            public System.Drawing.Image BANKLOGO { get; set; }
#endif
#if WPF
            public System.Windows.Media.ImageSource BANKLOGO { get; set; }
#endif
#if WINUI || SMARTMAUI
            public ImageSource BANKLOGO { get; set; }
#endif
            public short INSTITUTION_CODE;
            public short BRAND_CODE;
            public char CATEGORY_CODE;
            public int PTC_CODE;
            public DateTime TRANSACTION_DATE
            { get; set; }
            public long SEQUENCE_NO;
            public string SORTCODE
            { get; set; }
            public string ACCOUNT_NO
            { get; set; }
            public string DESCRIPTION
            { get; set; }
            public short CURRENCY_ORDINAL;  // This is the ordinal
            public string CURRENCY_DISPLAY = "";  // The corresponding GBP or whatever
            public short TRANSGROUP_CODE
            { get; set; }
            public short TRANSACTION_CODE
            { get; set; }
            public int CREDITDEBIT_INDICATOR;
            public string AMOUNT_TYPE = "";
            public double AMOUNT;
            public string CREDITS      // All because format "C;C;;" or "'C';'C';;" doesn't work
            { get; set; }
            public string DEBITS    // All because format "C;C;;" or "'C';'C';;" doesn't work
            { get; set; }
            public double BALANCE = 0;
            public string BALANCE_DISPLAY
            { get; set; }
            public string ACTIVITY = "";  // Crypto Shit start
            public string CRYPTO_CURRENCY = "";
            public string EXCHANGE = "";
            public double CRYPTO_AMOUNT = 0;
            public string CRYPTO_AMOUNT_DISPLAY;
            public double CRYPTO_RATE = 0;
            public string CRYPTO_CURRENCY_DISPLAY = "";  // The corresponding GBP or whatever
            public double NATIVE_AMOUNT = 0;
            public string NATIVE_CURRENCY = "";
            public double FEE_AMOUNT = 0;
            public string FEE_CURRENCY = "";
            public int CRYPTO_BALANCE = 0;
            public string TOTAL_COST = "";
            public string TO_ADDRESS = "";    // Crypto shit end
            public string NETWORK_NAME = "";    // Crypto shit end
            public string HASH = "";    // Crypto shit end
                                        //{ get; set; }
#if !(WINFORMS || DBSERVER || ANDROIDX)
            public Visibility UsernameVisible;
            public Visibility SortAccountVisible;
#endif
        }


        internal class DebitsCreditsByMonth : IComparable<DebitsCreditsByMonth>
        {
            internal DateTime
                PERIOD_END = SmartParametersV2016.defaultDate;
            internal double
                DEBITS = 0;
            internal double
                CREDITS = 0;

            internal DebitsCreditsByMonth(DateTime Read_Date,
                                    double Debits,
                                    double Credits)
            {
                PERIOD_END = Read_Date;
                DEBITS = Debits;
                CREDITS = Credits;
            }

            #region Icomparable<DebitsCreditsByMonth> Members
            public int CompareTo(DebitsCreditsByMonth other)
            {
                // Need to check this breaking change!!
                int compareResult = 0;

                if (other != null)
                {
                    if (PERIOD_END < other.PERIOD_END)
                    {
                        compareResult = -1;
                    }
                    else
                    {
                        if (this.PERIOD_END > other.PERIOD_END)
                        {
                            compareResult = 1;
                        }
                        //else
                        //{
                        //    compareResult = 0;
                        //}
                    }
                }
                return compareResult;
            }

            public static int Compare(DebitsCreditsByMonth left, DebitsCreditsByMonth right)
            {
                if (object.ReferenceEquals(left, right))
                {
                    return 0;
                }
                if (left is null)
                {
                    return -1;
                }
                return left.CompareTo(right);
            }

            public override bool Equals(object obj)
            {
                DebitsCreditsByMonth other = (DebitsCreditsByMonth)obj; //avoid double casting
                if (other is null)
                {
                    return false;
                }
                return this.CompareTo(other) == 0;
            }

            public static bool operator ==(DebitsCreditsByMonth left, DebitsCreditsByMonth right)
            {
                if (left is null)
                {
                    return right is null;
                }
                return left.Equals(right);
            }
            public static bool operator !=(DebitsCreditsByMonth left, DebitsCreditsByMonth right)
            {
                return !(left == right);
            }
            public static bool operator <(DebitsCreditsByMonth left, DebitsCreditsByMonth right)
            {
                return (Compare(left, right) < 0);
            }
            public static bool operator >(DebitsCreditsByMonth left, DebitsCreditsByMonth right)
            {
                return (Compare(left, right) > 0);
            }
            public override int GetHashCode()
            {
                int date = this.PERIOD_END.GetHashCode();
                return (int)date;
            }
            #endregion
        }
    }

    public class SmartData
    {
        public class Buttons
        {
            public char
                CUBEFACE_CODE = SmartParametersV2016.defaultChar;
            public char
                BUTTON_CODE = SmartParametersV2016.defaultChar;
            public string
                DESCRIPTION = "";
#if WINFORMS
            public DateTime
                  Created = SmartParametersV2016.defaultDate,
                  Updated = SmartParametersV2016.defaultDate;
            public string
                   Status_Flag = SmartParametersV2016.activeStatus;
            public DateTime
                   Deactivated = SmartParametersV2016.defaultDate;
#endif
        }

        public class Cubefaces
        {
            public char
                CUBEFACE_CODE = SmartParametersV2016.defaultChar;
            public short
                SCREEN_CODE = -1;
            public string
                DESCRIPTION = "";
            public bool
                ACTIVEFLAG = false;
#if WINFORMS
            public string
                COMMENT = "";
            public DateTime
                  Created = SmartParametersV2016.defaultDate,
                  Updated = SmartParametersV2016.defaultDate;
            public string
                   Status_Flag = SmartParametersV2016.activeStatus;
            public DateTime
                   Deactivated = SmartParametersV2016.defaultDate;
#endif
        }

        public class Cultures
        {
            public string
                CULTURE_CODE = "";
            public string
                CULTUREINFO = "";
            public string
                DESCRIPTION = "";
#if WINFORMS
            public DateTime
                          Created = SmartParametersV2016.defaultDate,
                          Updated = SmartParametersV2016.defaultDate;
            public string
                   Status_Flag = SmartParametersV2016.activeStatus;
            public DateTime
                  Deactivated = SmartParametersV2016.defaultDate;
#endif
        }

        public class CultureView
        {
            //  I am tempted to taked the USERNAME out for Production,
            //  but am holding fire until I get Multi-Meters - who can manipulate
            //  lots of different Usernames sorted out ....
            public string
                CULTURE_CODE = "";
            public CultureInfo
                CULTUREINFO = SmartParametersV2016.defaultCulture;     // Key to link to everything
            public string
                ISOCURRENCYSYMBOL = "";
            public string
                SYMBOL = "";
            public short
                CURRENCY_ORDINAL = 0;
            public string
                DESCRIPTION = "";
        }


        public class Currencies
        {
            public string
                ISOCURRENCYSYMBOL = "";       // Always empty for the time being
            public short
                ORDINAL = 0;
            public string
                DESCRIPTION = String.Empty;                  // This is based on the £ sterling
            public bool
                ACTIVEFLAG = false;
            //public short
            //    TOTALSINDEX = 0;
#if WINFORMS
            public DateTime
                Created = SmartParametersV2016.defaultDate,
                Updated = SmartParametersV2016.defaultDate;
            public string
                Status_Flag = SmartParametersV2016.activeStatus;
            public DateTime
                Deactivated = SmartParametersV2016.defaultDate;
#endif
        }

        public class Handlers
        {
            public string
                PATH = "";       // Always empty for the time being
            public string
                DESCRIPTION = String.Empty;                  // This is based on the £ sterling
#if WINFORMS
            public DateTime
                Created = SmartParametersV2016.defaultDate,
                Updated = SmartParametersV2016.defaultDate;
            public string
                Status_Flag = SmartParametersV2016.activeStatus;
            public DateTime
                Deactivated = SmartParametersV2016.defaultDate;
#endif
        }
        public class Schemas
        {
            public short
                INDEX = 0;
            public string
                SCHEMA_NAME = "";
#if WINFORMS
            public DateTime
                  Created = SmartParametersV2016.defaultDate,
                  Updated = SmartParametersV2016.defaultDate;
            public string
                   Status_Flag = SmartParametersV2016.activeStatus;
            public DateTime
                   Deactivated = SmartParametersV2016.defaultDate;
#endif
        }

        public class SQLiteSchemas
        {
            public string
                SCHEMA_NAME = "";
            public string
                DESCRIPTION = "";
        }

        public class SQLiteTables
        {
            public string
                SCHEMA_NAME = "";
            public string
                TABLE_NAME = "";
            public short
                ORDINAL = 0;
            public string
                PRODUCTION = "";
            public string
                DESCRIPTION = "";

        }

        public class SQLiteFields
        {
            public string
                SCHEMA_NAME = "";
            public string
                TABLE_NAME = "";
            public short
                ORDINAL = 0;
            public char
                KEY_FIELD = SmartParametersV2016.defaultChar;
            public string
                FIELD_NAME = "";
            public string
                FIELD_TYPE = "";
            public short
                FIELD_LENGTH = 0;
        }

        public class SmartSwitchFields
        {
            public string
                SCHEMA_NAME = "";
            public string
                TABLE_NAME = "";
            public short
                ORDINAL = 0;
            public char
                KEY_FIELD = SmartParametersV2016.defaultChar;
            public string
                FIELD_NAME = "";
            public string
                FIELD_TYPE = "";
            public short
                FIELD_LENGTH = 0;
        }

        public class Tooltips
        {
            public char
                CUBEFACE_CODE = SmartParametersV2016.defaultChar;
            public string
                KEY_NAME = "";
            public string
                LANGUAGE = "";
            public string
                TOOLTIP_TEXT = "";
#if WINFORMS
            public DateTime
                  Created = SmartParametersV2016.defaultDate,
                  Updated = SmartParametersV2016.defaultDate;
            public string
                   Status_Flag = SmartParametersV2016.activeStatus;
            public DateTime
                   Deactivated = SmartParametersV2016.defaultDate;
#endif
        }
    }



    public class SmartUtility
    {
        public class TariffEngine
        {
            // No USERNAME because this table is only ever used inside SmartSwitch
            // It is never retrieved from the database, nor stored (unlike ANALYSIS_COSTS)
            // so there is no need for a Username field
            internal string
                USERNAME = "";
            internal char
                CUBEFACE_CODE = SmartParametersV2016.defaultChar;
            internal short
                SUPPLIER_CODE = 0,
                BRAND_CODE = 0;
            internal DateTime
                ACCOUNT_CREATED = SmartParametersV2016.defaultDate;
            internal string
                ACCOUNT_NO = "";
            internal DateTime
                PERIOD_END = SmartParametersV2016.defaultDate;
#if WINFORMS
            internal string
                CODE = "";
            internal DateTime
                READING_DATE = SmartParametersV2016.defaultDate;
            internal string
                PAYMENT_PLAN = "";
            internal bool
                PAPERLESS_BILLING = false;
#endif
            internal decimal
                D_UNITS = 0.0M;
#if WINFORMS
            internal decimal
                D_UNITS_RATE = 0.0M;
#endif
            internal decimal
                N_UNITS = 0.0M;
#if WINFORMS
            internal decimal
                N_UNITS_RATE = 0.0M;
#endif
            internal decimal
                UNITS = 0.0M,
                UNITS_TOTAL = 0.0M;
#if WINFORMS
            internal decimal
                STANDING_CHARGE = 0.0M;
#endif
            internal bool
                DAYS = false,
                SC_MONTHS = false,
                SC_QUARTERS = false,
                SC_YEARS = false,
                LIMIT_MONTHS = false,
                LIMIT_QUARTERS = false,
                LIMIT_YEARS = false;
            internal decimal
                UNITS_COST = 0.0M,
                CHARGES_COST = 0.0M;
#if WINFORMS
            internal decimal
                TOTAL_DAILY_COST = 0.0M;
#endif
            internal decimal
                TOTAL_MONTHLY_COST = 0.0M,
                TOTAL_QUARTERLY_COST = 0.0M,
                TOTAL_YEARLY_COST = 0.0M;
        }

        public class AnalysisBrandsView
        {
            // All the rest have get/sets so the DataGridView Costs
            // can display them!  Yay!!
            public char CUBEFACE_CODE;
            public char RESOURCE_CODE;
            public string RESOURCE_TYPE;
            public short SUPPLIER_CODE;
            public short BRAND_CODE;
            public string BRAND_NAME;

        }

        public class AnalysisTariffsNameView
        {
            // All the rest have get/sets so the DataGridView Costs
            // can display them!  Yay!!
            public char CUBEFACE_CODE;
            public char RESOURCE_CODE;
            public string RESOURCE_TYPE;
            public short SUPPLIER_CODE;
            public int TARIFF_CODE;
            public short BRAND_CODE;
            public short VERSION_CODE;
            public string TARIFF_NAME;
        }

        public class AnalysisCostsView
        {
            // All the rest have get/sets so the DataGridView Costs
            // can display them!  Yay!!
            public string USERNAME
            { get; set; }
            public short SUPPLIER_CODE;
            public short BRAND_CODE;
            public string SUPPLIER_NAME
            { get; set; }
            public int TARIFF_CODE;
            public string TARIFF_NAME
            { get; set; }
            public decimal TCR;
            public string RESOURCE_TYPE;
            public string METER_TYPE
            { get; set; }
            public string PAYMENT_PLAN;
            public string PAYMENT_NAME
            { get; set; }
            public string TOTAL_AMOUNT
            { get; set; }
            public decimal[] EXCHANGE_RATES = new decimal[4] { 0, 0, 0, 0 };

        }

        public class AnalysisBillsView
        {
            // All the rest have get/sets so the DataGridView Bills
            // can display them!  Yay!!
            public string USERNAME
            { get; set; }
            public string ACCOUNT_NO
            { get; set; }
            public DateTime BILL_DATE
            { get; set; }
            public string STATEMENT_ID
            { get; set; }
            public DateTime DATE
            { get; set; }
            public string CODE
            { get; set; }
            public string DESCRIPTION
            { get; set; }
            public string AMOUNT
            { get; set; }
            public string BALANCE
            { get; set; }
            public decimal[] EXCHANGE_RATES = new decimal[4] { 0, 0, 0, 0 };

        }

        public class AnalysisReadingsView
        {
            // Well .. whilst I can't see the need for the CUBEFACE
            // to be displayed (because we are already showing Utility)
            // I can see the need for the Username IF we are combining
            // Readings as per Multi-User. So lets see if I can get it
            // back in as 'no-visible'
            // Visible is being IGNORED! No surprise there Chimps RULE!!
            public string USERNAME
            { get; set; }
            public string ACCOUNT_NO
            { get; set; }
            public string STATEMENT_ID
            { get; set; }
            public DateTime READINGS_PERIOD_END
            { get; set; }
            public string METER_SERIAL_NO
            { get; set; }
            public string READ_TYPE
            { get; set; }
            public string THIS_READ
            { get; set; }
            public string LAST_READ
            { get; set; }
            public string UNITS_USED
            { get; set; }
            public string UNIT_OF_MEASURE
            { get; set; }
        }

        // This is a FUTURE prediction of COSTS and therfore
        // we cannot pin any Exchange Rate to these 'predicitions'
        // The only option we have is to convert them at 'today's'
        // exchange rate as that's the best we can do
        public class AnalysisBreakdownView
        {
            public string USERNAME
            { get; set; }
            public DateTime FROM_DATE
            { get; set; }
            public DateTime TO_DATE
            { get; set; }
            public string CODE
            { get; set; }
            public string DESCRIPTION
            { get; set; }
            public string ITEMS
            { get; set; }
            public string CHARGES_DISCOUNTS
            { get; set; }
            public string DAY_UNITS
            { get; set; }
            public string DAY_RATE
            { get; set; }
            public string NIGHT_UNITS
            { get; set; }
            public string NIGHT_RATE
            { get; set; }
            public string TOTAL_UNITS
            { get; set; }
            public string TOTAL
            { get; set; }
        }

        internal class Analysis_Bills
        {
            internal string
                USERNAME = "";
            internal string
                ACCOUNT_NO = "";
            internal string
                STATEMENT_ID = "";
            internal DateTime
                BILL_DATE = SmartParametersV2016.defaultDate;
            internal DateTime
                DATE = SmartParametersV2016.defaultDate;
            internal short
                ITEM = 0;
            internal string
                CODE = "",
                DESCRIPTION = "";
            internal int
                AMOUNT = 0,
                BALANCE = 0;
        }

        internal class AnalysisCosts
        {
            internal string USERNAME = "";
            internal char CUBEFACE_CODE = SmartParametersV2016.defaultChar;
            internal char RESOURCE_CODE = SmartParametersV2016.defaultChar;
            internal short UNIQUE_NUMBER = 0;   // Just so we can put a key on the table
            internal short SUPPLIER_CODE = 0;
            internal short BRAND_CODE = 0;
            internal int TARIFF_CODE = 0;
            internal decimal TCR = 0.0M;
            internal string RESOURCE_TYPE = "";
            internal string PAYMENT_PLAN = "";
            internal int TOTAL_COST = 0;
        }

        // To speed up the Analysis calculation process.
        internal class AnalysisConditions
        {
            internal char displayed_resource_code = SmartParametersV2016.defaultChar;
            internal string displayed_resource_type = "";
            internal DateTime withdrawnDate = SmartParametersV2016.defaultDate;
            internal int fallback_tariff_code = 0;
            internal string SC_UNITS = "";
            internal string SC_DESCRIPTION = "";
            internal List<UnitRates> rates_found1 = new List<UnitRates>();
            internal List<UnitRates> rates_found2 = new List<UnitRates>();
            internal List<UnitRates> rates_found3 = new List<UnitRates>();
            internal List<UnitRates> fallback_rates_found1 = new List<UnitRates>();
            internal List<UnitRates> fallback_rates_found2 = new List<UnitRates>();
            internal List<UnitRates> fallback_rates_found3 = new List<UnitRates>();
            internal List<SmartUtility.ConditionsLimits> restrict_usageList = new List<SmartUtility.ConditionsLimits>();
            internal decimal condition_costs = 0.0M;
        }

        internal class SwitchInfo
        {
            // Read-Only section
            internal string status = "";
            internal string postcode = "";
            internal string address = "";
            internal string title = "";
            internal string first_name = "";
            internal string last_name = "";
            internal string telephone = "";
            internal string email = "";
            internal string date_of_birth = "";
            internal short birth_day = 0;
            internal short birth_month = 0;
            internal short birth_year = 0;
            internal string new_account_password = "";
            internal string bank_account_name = "";
            internal string bank_account_sort_code = "";
            internal string bank_account_number = "";
            internal short bank_account_payday = 0;
            internal short present_supplier_code = 0;
            internal short present_brand_code = 0;
            internal int present_tariff_code = 0;
            internal char present_payment_plan = SmartParametersV2016.defaultChar;
            internal string present_resource_type = "";
            internal string proposed_brand_name = "";
            internal string proposed_tariff_name = "";
            internal string proposed_payment_name = "";
            internal short proposed_supplier_code = 0;
            internal short proposed_brand_code = 0;
            internal int proposed_tariff_code = 0;
            internal char proposed_payment_plan = SmartParametersV2016.defaultChar;
            internal string url_prfix = "";
            internal string username = "";
            internal char cubeface_code = SmartParametersV2016.defaultChar;
            internal DateTime created = SmartParametersV2016.defaultDate;
            internal string account_no = "";
            internal string udprn = "";           // 8 numeric chars
            internal string mpan = "";
            internal string e_meter_serial_no = "";
            internal string mprn = "";
            internal string g_meter_serial_no = "";
            internal char resource_code = SmartParametersV2016.defaultResourceCode;
            internal string resource_type = "";
        }

        public class AccChargesCreditsSQLite
        {
            public string
                USERNAME    // So its never 'null' ... SmartSwitch never really
            { get; set; }                            // deals with USERNAMES (they are removed by SmartDBserver coming
                                                     // in and set by SmartDBServer on the way out. However (!)
                                                     // sometimes we created records within SmartDashboard and THEN
                                                     // we need this to be 'something' even if its only 'empty' ...
            public string
                CUBEFACE_CODE
            { get; set; }
            public short
                SUPPLIER_CODE
            { get; set; }
            public short
                BRAND_CODE
            { get; set; }
            public DateTime
                ACCOUNT_CREATED
            { get; set; }
            public string
                KEY_DETAILS
            { get; set; }
            public int
                RANDOMKEY1
            { get; set; }
            public string
                DETAILS
            { get; set; }
            public int
                RANDOMKEY2
            { get; set; }
            public bool
                Updated = false;
        }

        internal class AccChargesCredits
        {
            internal string
                USERNAME = "";    // So its never 'null' ... SmartSwitch never really
                                  // deals with USERNAMES (they are removed by SmartDBserver coming
                                  // in and set by SmartDBServer on the way out. However (!)
                                  // sometimes we created records within SmartDashboard and THEN
                                  // we need this to be 'something' even if its only 'empty' ...
            internal char
                CUBEFACE_CODE = SmartParametersV2016.defaultChar;
            internal short
                SUPPLIER_CODE = 0;
            internal short
                BRAND_CODE = 0;
            internal DateTime
                ACCOUNT_CREATED = SmartParametersV2016.defaultDate;
            internal string
                ACCOUNT_NO = "";
            internal string
                STATEMENT_ID = "";
            internal DateTime
                BILL_DATE = SmartParametersV2016.defaultDate;
            internal DateTime
                ACCOUNT_DATE = SmartParametersV2016.defaultDate;
            internal short
                ACCOUNT_ITEM = 0;
            internal int
                RANDOMKEY1 = 0;
            internal string
                ACCOUNT_TYPE = "";
            internal short
                ACCOUNT_VAT_CODE = 0;       // Either 01 or 02
            internal int
                ACCOUNT_AMOUNT = 0;        // Exc VAT
            internal string
                INCLUDE_BILLS = "";          // Because "Cancelled VAT" from SP ISN'T
            internal int
                RANDOMKEY2 = 0;
            internal bool
                Updated = false;
        }

        public class AccountsSQLite
        {
            // Accounts DOESN'T have a RESOURCE_CODE because for a Utility Supplier
            // supplying BOTH Resources of Electricity and Gas - the Account would
            // be the same i.e. it would cross RESOURCES.  You would never have
            // British Gas with a separate Account for Electricity and another for Gas ...

            //
            // What bollocks!! Oh yes you fucking would!!  Look at Bob's account!!!
            //
            //
            public string
                USERNAME
            { get; set; }
            public string
                CUBEFACE_CODE
            { get; set; }
            public string
                RESOURCE_CODE       // <=  It's in here!!
            { get; set; }
            public short
                SUPPLIER_CODE
            { get; set; }
            public short
                BRAND_CODE
            { get; set; }
            public DateTime
                ACCOUNT_CREATED
            { get; set; }
            public string
                KEY_DETAILS
            { get; set; }
            public int
                RANDOMKEY1
            { get; set; }
            public string
                DETAILS
            { get; set; }
            public int
                RANDOMKEY2
            { get; set; }
            public bool
                Updated = false;
        }

        internal class Accounts
        {
            // Accounts DOESN'T have a RESOURCE_CODE because for a Utility Supplier
            // supplying BOTH Resources of Electricity and Gas - the Account would
            // be the same i.e. it would cross RESOURCES.  You would never have
            // British Gas with a separate Account for Electricity and another for Gas ...

            //
            // Oh yes you would!!  Look at Bob's account!!!
            //  So ... where the fuck is it???
            //  Its the third one down knucklehead ....
            internal string
                USERNAME = "";
            internal char
                CUBEFACE_CODE = SmartParametersV2016.defaultChar;
            internal char
                RESOURCE_CODE = SmartParametersV2016.defaultChar;  // <= HERE!
            internal short
                SUPPLIER_CODE = 0;
            internal short
                BRAND_CODE = 0;
            internal DateTime
                ACCOUNT_CREATED = SmartParametersV2016.defaultDate;
            internal string
                ACCOUNT_NO = "";
            /// The above is now all redundant because as soon as we have logged in to
            // an account we hae the ability to find the address AND the MPAN/MPRN using
            // the lookup JSON table from:
            // https://quote.powershop.co.uk/promo/ws/index.html?callback=none&fields=SK8+3PD&wsFunction=GetAddressListJson HTTP/1.1
            // This URL also allows us to find the UDPRN for the address which we can also store
            // and use in the main screen dropdown
            internal string
                UDPRN = "";           // Each account may be at a different address
            internal int
                RANDOMKEY1 = 0;
            internal char
                STATUS = SmartParametersV2016.defaultChar;
            // Open = 'Y', Switching = 'S', Closed = 'N'
            // Why are we putting TARIFF_CODE and PAYMENT_PLAN in here?
            // So that when an account has no Bills OR we can't read the Bills
            // we can keep a record of their actual Tariff and Plan (which they
            // must have in order to get an account created)
            // We are not putting these in TariffDetails becuase of the enormous
            // complexity of having a record there which constantly needs updating...
            // We do that shit already with Utility.Accounts ...
            internal short
                CURRENCY_ORDINAL = 0;        // Our own 'category type'!!
            // What do we really use the MPAN/MPRN for?  For identification on the screen
            // and for making sure that people don't use TWO different Usernames to try
            // and circumvent the Expiry Date restrictions.  That's all.  We only ever
            // use it for on-screen information, never for analysis.  Its just a unique
            // number after all, but we ALWAYS need to know it..
            internal int
                TARIFF_CODE = 0;
            internal char
                PAYMENT_PLAN = SmartParametersV2016.defaultChar;
            internal int
                RANDOMKEY2 = 0;
            internal bool
                Updated = false;    // Lower case so it doesn't go out to the table
        }

        public class BankDetailsSQLite
        {
            public string
                USERNAME
            { get; set; }
            public string
                CUBEFACE_CODE
            { get; set; }
            public short
                SUPPLIER_CODE
            { get; set; }
            public short
                BRAND_CODE
            { get; set; }
            public DateTime
                ACCOUNT_CREATED
            { get; set; }
            public string
                KEY_DETAILS
            { get; set; }
            public int
                RANDOMKEY1
            { get; set; }
            public string
                DETAILS
            { get; set; }
            public int
                RANDOMKEY2
            { get; set; }
            public bool
                Updated = false;
        }

        internal class BankDetails
        {
            internal string
                USERNAME = "";
            internal char
                CUBEFACE_CODE = SmartParametersV2016.defaultChar;
            internal short
                SUPPLIER_CODE = 0;
            internal short
                BRAND_CODE = 0;
            internal DateTime
                ACCOUNT_CREATED = SmartParametersV2016.defaultDate;
            internal string
                ACCOUNT_NO = "";
            internal int
                RANDOMKEY1 = 0;
            // I am taking SORTCODE, ACCOUNT_NO and BANK_PAYDAY
            // OUT from being a key in this table. This is because
            // it's common sense for a Utility customer to change
            // their bank/account with any energy supplier thus
            // making it a candidate *not* to be a key (keys don't change!!)            
            internal short
                FINANCE_INSTITUTION_CODE = 0;
            internal short
                FINANCE_BRAND_CODE = 0;
            internal string
                FINANCE_SORTCODE = "";
            internal string
                FINANCE_ACCOUNT_NO = "";
            internal short
                BANK_PAYDAY = 0;
            internal int
                RANDOMKEY2 = 0;
            internal bool
                Updated = false;    // Lower case so it doesn't go out to the table
        }

        public class BillsSQLite
        {
            public string
                USERNAME
            { get; set; }
            public string
                CUBEFACE_CODE
            { get; set; }
            public short
                SUPPLIER_CODE
            { get; set; }
            public short
                BRAND_CODE
            { get; set; }
            public DateTime
                ACCOUNT_CREATED
            { get; set; }
            public string
                KEY_DETAILS
            { get; set; }
            public int
                RANDOMKEY1
            { get; set; }
            public string
                DETAILS
            { get; set; }
            public int
                RANDOMKEY2
            { get; set; }
            public bool
                Updated = false;
        }


        internal class Bills : IComparable<SmartUtility.Bills>
        {
            internal string
                USERNAME = "";
            internal char
                CUBEFACE_CODE = SmartParametersV2016.defaultChar;
            internal short
                SUPPLIER_CODE = 0;
            internal short
                BRAND_CODE = 0;
            internal DateTime
                ACCOUNT_CREATED = SmartParametersV2016.defaultDate;
            internal string
                ACCOUNT_NO = "";
            internal string
                STATEMENT_ID = "";
            internal DateTime
                BILL_DATE = SmartParametersV2016.defaultDate;              // The MES version is a STRING
            internal int
                RANDOMKEY1 = 0;
            internal DateTime
                BILL_PERIOD_START = SmartParametersV2016.defaultDate,
                BILL_PERIOD_END = SmartParametersV2016.defaultDate;
            internal char
                PAYMENT_PLAN = SmartParametersV2016.defaultChar;
            internal bool
                RESOURCE_BALANCES = false;
            internal int
                PREVIOUS_BALANCE = 0,       // Well, we can (in theory) calculate these
                PAYMENTS_RECEIVED = 0,      // OB = OB - PAYMENTS_RECEIVED three from all the data we have from a Bill
                ACCOUNT_CHARGES_CREDITS = 0,// Rebates go in here and are credited to your account
                ACCOUNT_CHARGES_CREDITS_VAT_AMOUNT = 0,
                SUPPLY_CHARGES_CREDITS = 0,
                SUPPLY_CHARGES_CREDITS_VAT_AMOUNT = 0,
                BILL_VAT_AMOUNT = 0;        // For the entire Bill (if we can find it) e.g. for SP so we can split it up later because of ONE FUCKING PENNY
            internal short
                BILL_VAT_CODE = 0;
            internal int
                OUTSTANDING_BALANCE = 0,    // But I will leave them in as a check
                TOTAL_NOW_DUE = 0,          // Which may be different from OB
                MONTHLY_PAYMENT = 0;        // May only be used by Scottish Power??
            internal DateTime
                PAYMENT_DUE_DATE = SmartParametersV2016.defaultDate;       // Latest date due
            internal string
                PAYMENT_TYPE = "";           // This text may not match the Payment Plan, but it may be relevant
            internal DateTime
                DIRECT_DEBIT_DATE = SmartParametersV2016.defaultDate;
            internal decimal
                LOYALTY_BONUS = 0.0M;
            internal DateTime
                DISCOUNT_CREDIT_DATE = SmartParametersV2016.defaultDate;
            internal string
                FIRST_YEARS_DISCOUNT = "",
                REWARDS = "";                // SSE Only I think
            internal int
                RANDOMKEY2 = 0;
            internal bool
                Updated = false;
            #region Icomparable<SmartUtility.Bills> Members
            public int CompareTo(Bills other)
            {
                // Meed to check this breaking change!!!
                int compareResult = 0;

                if (other != null)
                {
                    if (BILL_PERIOD_END < other.BILL_PERIOD_END)
                    {
                        compareResult = -1;
                    }
                    else
                    {
                        if (this.BILL_PERIOD_END > other.BILL_PERIOD_END)
                        {
                            compareResult = 1;
                        }
                        //else
                        //{
                        //    compareResult = 0;
                        //}
                    }
                }
                return compareResult;
            }
            public static int Compare(Bills left, Bills right)
            {
                if (object.ReferenceEquals(left, right))
                {
                    return 0;
                }
                if (left is null)
                {
                    return -1;
                }
                return left.CompareTo(right);
            }
            public override bool Equals(object obj)
            {
                Bills other = (Bills)obj; //avoid double casting
                if (other is null)
                {
                    return false;
                }
                return this.CompareTo(other) == 0;
            }

            public static bool operator ==(Bills left, Bills right)
            {
                if (left is null)
                {
                    return right is null;
                }
                return left.Equals(right);
            }
            public static bool operator !=(Bills left, Bills right)
            {
                return !(left == right);
            }
            public static bool operator <(Bills left, Bills right)
            {
                return (Compare(left, right) < 0);
            }
            public static bool operator >(Bills left, Bills right)
            {
                return (Compare(left, right) > 0);
            }

            public override int GetHashCode()
            {
                int date = this.BILL_PERIOD_END.GetHashCode();
                return (int)date;
            }
            #endregion
        }

        public class BillsResourceSQLite
        {
            public string
                USERNAME
            { get; set; }
            public string
                CUBEFACE_CODE
            { get; set; }
            public short
                SUPPLIER_CODE
            { get; set; }
            public short
                BRAND_CODE
            { get; set; }
            public DateTime
                ACCOUNT_CREATED
            { get; set; }
            public string
                KEY_DETAILS
            { get; set; }
            public int
                RANDOMKEY1
            { get; set; }
            public string
                DETAILS
            { get; set; }
            public int
                RANDOMKEY2
            { get; set; }
            public bool
                Updated = false;
        }

        internal class BillsResource
        {
            internal string
                USERNAME = "";
            internal char
                CUBEFACE_CODE = SmartParametersV2016.defaultChar;
            internal short
                SUPPLIER_CODE = 0;
            internal short
                BRAND_CODE = 0;
            internal DateTime
                ACCOUNT_CREATED = SmartParametersV2016.defaultDate;
            internal string
                ACCOUNT_NO = "";
            internal string
                STATEMENT_ID = "";
            internal DateTime
                BILL_DATE = SmartParametersV2016.defaultDate;          // The MES version is a STRING
            internal string
                MPAN_MPRN = "";
            internal int
                RANDOMKEY1 = 0;
            internal string
                RESOURCE_ACCOUNT_NO = "";
            internal int
                TARIFF_CODE = 0;
            internal int
                NEW_CHARGES = 0,
                RESOURCE_DISCOUNTS = 0,
                RESOURCE_VAT_AMOUNT = 0;
            internal short
                RESOURCE_VAT_CODE = 0;
            internal int
                PREVIOUS_RESOURCE_BALANCE = 0,
                OUTSTANDING_RESOURCE_BALANCE = 0;
            internal int
                RANDOMKEY2 = 0;
            internal bool
                Updated = false;
        }

        public class ECostsSQLite
        {
            public string
                USERNAME
            { get; set; }
            public string
                CUBEFACE_CODE
            { get; set; }
            public string
                RESOURCE_CODE
            { get; set; }
            public short
                UNIQUE_NUMBER
            { get; set; }
            public short
                SUPPLIER_CODE
            { get; set; }
            public short
                BRAND_CODE
            { get; set; }
            public int
                TARIFF_CODE
            { get; set; }
            public decimal
                TCR
            { get; set; }
            public string
                RESOURCE_TYPE
            { get; set; }
            public string
                PAYMENT_PLAN
            { get; set; }
            public int
                TOTAL_COST
            { get; set; }
            public bool
                Updated = false;
        }

        internal class ECosts
        {
            internal string
                USERNAME = "";
            internal char
                CUBEFACE_CODE = SmartParametersV2016.defaultChar;
            internal char
                RESOURCE_CODE = SmartParametersV2016.defaultChar;
            internal short
                UNIQUE_NUMBER = 0;
            internal short
                SUPPLIER_CODE = 0;
            internal short
                BRAND_CODE = 0;
            internal int
                TARIFF_CODE = 0;
            internal decimal
                TCR = 0.0M;
            internal string
                RESOURCE_TYPE = "";
            internal char
                PAYMENT_PLAN = SmartParametersV2016.defaultChar;
            internal int
                TOTAL_COST = 0;
            internal bool
                Updated = false;
        }

        public class EDiscountsSQLite
        {
            public string
                USERNAME
            { get; set; }
            public string
                CUBEFACE_CODE
            { get; set; }
            public short
                SUPPLIER_CODE
            { get; set; }
            public short
                BRAND_CODE
            { get; set; }
            public DateTime
                ACCOUNT_CREATED
            { get; set; }
            public string
                KEY_DETAILS
            { get; set; }
            public int
                RANDOMKEY
            { get; set; }
            public string
                DISCOUNT_TYPE
            { get; set; }
            public short
                DISCOUNT_VAT_CODE
            { get; set; }
            public int
                DISCOUNT_AMOUNT
            { get; set; }
            public bool
                Updated = false;
        }

        internal class EDiscounts
        {
            internal string
                USERNAME = "";
            internal char
                CUBEFACE_CODE = SmartParametersV2016.defaultChar;
            internal short
                SUPPLIER_CODE = 0;
            internal short
                BRAND_CODE = 0;
            internal DateTime
                ACCOUNT_CREATED = SmartParametersV2016.defaultDate;
            internal string
                ACCOUNT_NO = "";
            internal string
                STATEMENT_ID = "";
            internal DateTime
                BILL_DATE = SmartParametersV2016.defaultDate;
            internal string
                MPAN_MPRN = "";
            internal DateTime
                DISCOUNT_DATE = SmartParametersV2016.defaultDate;          // This is the BILL_PERIOD_END surely?
            internal short
                DISCOUNT_ITEM = 0;
            internal int
                RANDOMKEY = 0;
            internal string
                DISCOUNT_TYPE = "";
            internal short
                DISCOUNT_VAT_CODE = 0;
            internal int
                DISCOUNT_AMOUNT = 0;        // Exc VAT
            internal bool
                Updated = false;
        }

        public class EReadingsSQLite
        {
            public string
                USERNAME
            { get; set; }
            public string
                CUBEFACE_CODE
            { get; set; }
            public short
                SUPPLIER_CODE
            { get; set; }
            public short
                BRAND_CODE
            { get; set; }
            public DateTime
                ACCOUNT_CREATED
            { get; set; }
            public string
                KEY_DETAILS
            { get; set; }
            public int
                RANDOMKEY1
            { get; set; }
            public string
                DETAILS            // <= Why MAKE UP a fucking key???? This is need because we sort on it
            { get; set; }
            public int
                RANDOMKEY2
            { get; set; }
            public string
                READ_TYPE
            { get; set; }
            public decimal          // Because we will never have any 'number' problems if its like this
                D_LAST_READ
            { get; set; }
            public decimal
                D_THIS_READ
            { get; set; }
            public decimal
                D_UNITS_USED
            { get; set; }
            public decimal          // Because we will never have any 'number' problems if its like this
               N_LAST_READ
            { get; set; }
            public decimal
                N_THIS_READ
            { get; set; }
            public decimal
                N_UNITS_USED
            { get; set; }
            public string
                UNIT_OF_MEASURE
            { get; set; }
            public bool
                Updated = false;
        }

        internal class EReadings
        {
            internal string
                USERNAME = "";
            internal char
                CUBEFACE_CODE = SmartParametersV2016.defaultChar;
            internal short
                SUPPLIER_CODE = 0;
            internal short
                BRAND_CODE = 0;
            internal DateTime
                ACCOUNT_CREATED = SmartParametersV2016.defaultDate;
            internal string
                ACCOUNT_NO = "";
            internal string
                STATEMENT_ID = "";                // <= We cannot sort on this
            internal DateTime
                BILL_DATE = SmartParametersV2016.defaultDate;              // <= Why MAKE UP a fucking key???? This is need because we sort on it
            internal string
                MPAN_MPRN = "";
            internal DateTime
                READINGS_PERIOD_START = SmartParametersV2016.defaultDate,      // <= when you HAVE ONE GIVEN TO YOU????
                READINGS_PERIOD_END = SmartParametersV2016.defaultDate;        // This is the CLOSING date the 'TO' date
            internal int
                RANDOMKEY1 = 0;
            internal string
                METER_SERIAL_NO = "";
            internal int
                RANDOMKEY2 = 0;
            internal string
                READ_TYPE = "";
            internal decimal          // Because we will never have any 'number' problems if its like this
                D_LAST_READ = 0.0M,
                D_THIS_READ = 0.0M,
                D_UNITS_USED = 0.0M,
                N_LAST_READ = 0.0M,
                N_THIS_READ = 0.0M,
                N_UNITS_USED = 0.0M;
            internal string
                UNIT_OF_MEASURE = "";
            internal bool
                Updated = false;
        }

        public class EStandingChargesSQLite
        {
            public string
                USERNAME
            { get; set; }
            public string
                CUBEFACE_CODE
            { get; set; }
            public short
                SUPPLIER_CODE
            { get; set; }
            public short
                BRAND_CODE
            { get; set; }
            public DateTime
                ACCOUNT_CREATED
            { get; set; }
            public string
                KEY_DETAILS
            { get; set; }
            public int
                RANDOMKEY
            { get; set; }
            public string
                CHARGES_TYPE
            { get; set; }
            public decimal
                STANDING_CHARGE
            { get; set; }
            public short
                CHARGES_DAYS
            { get; set; }
            public int
                CHARGES_COST       // Exc VAT
            { get; set; }
            public bool
                Updated = false;
        }

        internal class EStandingCharges
        {
            internal string
                USERNAME = "";
            internal char
                CUBEFACE_CODE = SmartParametersV2016.defaultChar;
            internal short
                SUPPLIER_CODE = 0;
            internal short
                BRAND_CODE = 0;
            internal DateTime
                ACCOUNT_CREATED = SmartParametersV2016.defaultDate;
            internal string
                ACCOUNT_NO = "";
            internal string
                STATEMENT_ID = "";
            internal DateTime
                BILL_DATE = SmartParametersV2016.defaultDate;                          // <= Why MAKE UP a fucking key
            internal string
                MPAN_MPRN = "";
            internal DateTime
                STANDING_CHARGES_PERIOD_START = SmartParametersV2016.defaultDate,      // When you can use theirs???
                STANDING_CHARGES_PERIOD_END = SmartParametersV2016.defaultDate;
            internal short
                CHARGES_ITEM = 0;           // Because you could get more than one Standing Charge for any date range
            internal int
                RANDOMKEY = 0;
            internal string
                CHARGES_TYPE = "";
            internal decimal
                STANDING_CHARGE = 0.0M;
            internal short
                CHARGES_DAYS = 0;
            internal int
                CHARGES_COST = 0;       // Exc VAT
            internal bool
                Updated = false;
        }
        public class EUnitChargesSQLite
        {
            // There is no PERIOD_END because ... which one would we put it against?
            // when we have two readings on two different dates against one rate?
            public string
                USERNAME
            { get; set; }
            public string
                CUBEFACE_CODE
            { get; set; }
            public short
                SUPPLIER_CODE
            { get; set; }
            public short
                BRAND_CODE
            { get; set; }
            public DateTime
                ACCOUNT_CREATED
            { get; set; }
            public string
                KEY_DETAILS
            { get; set; }
            public int
                RANDOMKEY
            { get; set; }
            public string
                UNITS_TYPE
            { get; set; }
            public decimal                    // described as numeric in DB (they are interchangeable)
                UNITS
            { get; set; }
            public decimal
                UNITS_RATE
            { get; set; }
            public string
                UNIT_OF_MEASURE
            { get; set; }
            public int
                UNITS_COST         //  Exc VAT
            { get; set; }
            public bool
                Updated = false;

        }

        internal class EUnitCharges : IEquatable<SmartUtility.EUnitCharges>
        {
            // There is no PERIOD_END because ... which one would we put it against?
            // when we have two readings on two different dates against one rate?
            internal string
                USERNAME = "";
            internal char
                CUBEFACE_CODE = SmartParametersV2016.defaultChar;
            internal short
                SUPPLIER_CODE = 0;
            internal short
                BRAND_CODE = 0;
            internal DateTime
                ACCOUNT_CREATED = SmartParametersV2016.defaultDate;
            internal string
                ACCOUNT_NO = "";
            internal string
                STATEMENT_ID = "";
            internal DateTime
                BILL_DATE = SmartParametersV2016.defaultDate;                      // <= Why MAKE UP a fucking key
            internal string
                MPAN_MPRN = "";
            internal DateTime
                UNIT_CHARGES_PERIOD_START = SmartParametersV2016.defaultDate,      // When you can use theirs???
                UNIT_CHARGES_PERIOD_END = SmartParametersV2016.defaultDate;
            internal char
                UNITS_TIME = SmartParametersV2016.defaultChar;
            internal short
                UNITS_BAND = 0;
            internal int
                RANDOMKEY = 0;
            internal string
                UNITS_TYPE = "";
            internal decimal                    // described as numeric in DB (they are interchangeable)
                UNITS = 0.0M,
                UNITS_RATE = 0.0M;
            internal string
                UNIT_OF_MEASURE = "";
            internal int
                UNITS_COST = 0;         //  Exc VAT
            internal bool
                Updated = false;

            // If Equals() returns true for a pair of objects 
            // then GetHashCode() must return the same value for these objects.
            public override int GetHashCode()
            {
                //Get hash code for the Username field if it is not null.
                int hashUsername = USERNAME.GetHashCode();
                //Get hash code for the Category Code field.
                int hashCategoryCode = CUBEFACE_CODE.GetHashCode();
                //Get hash code for the Supplier Code field.
                int hashSupplierCode = SUPPLIER_CODE.GetHashCode();
                //Get hash code for the Brand Code field.
                int hashBrandCode = BRAND_CODE.GetHashCode();
                //Get hash code for the Created field if it is not null.
                int hashCreated = ACCOUNT_CREATED.GetHashCode();
                //Get hash code for the AccountNo field if it is not null.
                int hashAccountNo = ACCOUNT_NO.GetHashCode();
                //Get hash code for the BillNumber field if it is not null.
                int hashBillNumber = STATEMENT_ID.GetHashCode();
                //Get hash code for the BillDate field if it is not null.
                int hashBillDate = BILL_DATE.GetHashCode();
                //Get hash code for the MPAN_MPRN field.
                int hashMpanMprn = MPAN_MPRN.GetHashCode();
                //Get hash code for the UNits Time field if it is not null.
                int hashUnitsTime = UNITS_TIME.GetHashCode();
                //Get hash code for the UNits Time field if it is not null.
                int hashUnitsBand = UNITS_BAND.GetHashCode();
                //Get hash code for the Units Rate field if it is not null.
                int hashUnitsRate = UNITS_RATE.GetHashCode();

                //Calculate the hash code for the product.
                return hashUsername ^
                        hashCategoryCode ^
                        hashSupplierCode ^
                        hashBrandCode ^
                        hashCreated ^
                        hashAccountNo ^
                        hashBillNumber ^
                        hashBillDate ^
                        hashMpanMprn ^
                        hashUnitsTime ^
                        hashUnitsBand ^
                        hashUnitsRate;
            }

            // Records are equal if the fieldsin this module are equal
            public bool Equals(EUnitCharges other)
            {
                // Check whether any of the compared objects is null.
                if (other is null)
                {
                    return false;
                }
                // Check whether the records fields are equal.
                if (USERNAME == other.USERNAME &&
                    CUBEFACE_CODE == other.CUBEFACE_CODE &&
                    SUPPLIER_CODE == other.SUPPLIER_CODE &&
                    BRAND_CODE == other.BRAND_CODE &&
                    ACCOUNT_CREATED == other.ACCOUNT_CREATED &&
                    ACCOUNT_NO == other.ACCOUNT_NO &&
                    STATEMENT_ID == other.STATEMENT_ID &&
                    BILL_DATE == other.BILL_DATE &&
                    MPAN_MPRN == other.MPAN_MPRN &&
                    UNITS_TIME == other.UNITS_TIME &&
                    UNITS_BAND == other.UNITS_BAND &&
                    UNITS_RATE == other.UNITS_RATE)
                {
                    return true;
                }
                return false;
            }

            public override bool Equals(object obj)
            {
                return Equals((EUnitCharges)obj);
            }
        }

        public class EUsageSQLite
        {
            public string
                USERNAME
            { get; set; }
            public string
                CUBEFACE_CODE
            { get; set; }
            public string
                KEY_DETAILS
            { get; set; }
            public int
                RANDOMKEY
            { get; set; }
            public DateTime
                USAGE_DATETIME
            { get; set; }
            public decimal
                USAGE_TOTAL
            { get; set; }
            public decimal
                USAGE_VALUE
            { get; set; }
            public bool
                Updated = false;
        }

        internal class EUsage
        {
            internal string
                USERNAME = "";
            internal char
                CUBEFACE_CODE = SmartParametersV2016.defaultChar;
            internal string
                MPAN_MPRN = "";
            internal short
                UNIQUE_INDEX = 0;
            internal int
                RANDOMKEY = 0;
            internal DateTime
                USAGE_DATETIME = SmartParametersV2016.defaultDate;
            internal decimal
                USAGE_TOTAL = 0.0M;
            internal decimal
                USAGE_VALUE = 0;
            internal bool
                Updated = false;
        }

        internal class EUsageView
        {
            internal string
                USERNAME = "";    // For completeness
            internal char
                CUBEFACE_CODE = SmartParametersV2016.defaultChar;
            internal short
                SUPPLIER_CODE = 0;
            internal DateTime
                ACCOUNT_CREATED = SmartParametersV2016.defaultDate;
            internal string
                ACCOUNT_NO = "";
            internal string
                MPAN_MPRN = "";
            internal string
                METER_SERIAL_NO = "";
            internal DateTime
                USAGE_DATETIME = SmartParametersV2016.defaultDate;
            internal decimal
                USAGE_TOTAL = 0.0M;
            internal decimal
                USAGE_VALUE = 0.0M;
        }

        internal class DUsageView
        {
            internal string
                USERNAME = "";    // For completeness
            internal char
                CUBEFACE_CODE = SmartParametersV2016.defaultChar;
            internal DateTime
                USAGE_DATETIME = SmartParametersV2016.defaultDate;
            internal decimal
                USAGE_TOTAL = 0.0M;
        }

        public class GCostsSQLite
        {
            public string
                USERNAME
            { get; set; }
            public string
                CUBEFACE_CODE
            { get; set; }
            public string
                RESOURCE_CODE
            { get; set; }
            public short
                UNIQUE_NUMBER
            { get; set; }
            public short
                SUPPLIER_CODE
            { get; set; }
            public short
                BRAND_CODE
            { get; set; }
            public int
                TARIFF_CODE
            { get; set; }
            public decimal
                TCR
            { get; set; }
            public string
                RESOURCE_TYPE
            { get; set; }
            public string
                PAYMENT_PLAN
            { get; set; }
            public int
                TOTAL_COST
            { get; set; }
            public bool
                Updated = false;
        }

        internal class GCosts
        {
            internal string
                USERNAME = "";
            internal char
                CUBEFACE_CODE = SmartParametersV2016.defaultChar;
            internal char
                RESOURCE_CODE = SmartParametersV2016.defaultChar;
            internal short
                UNIQUE_NUMBER = 0;
            internal short
                SUPPLIER_CODE = 0;
            internal short
                BRAND_CODE = 0;
            internal int
                TARIFF_CODE = 0;
            internal decimal
                TCR = 0.0M;
            internal string
                RESOURCE_TYPE = "";
            internal char
                PAYMENT_PLAN = SmartParametersV2016.defaultChar;
            internal int
                TOTAL_COST = 0;
            internal bool
                Updated = false;
        }

        public class GDiscountsSQLite
        {
            public string
                USERNAME
            { get; set; }
            public string
                CUBEFACE_CODE
            { get; set; }
            public short
                SUPPLIER_CODE
            { get; set; }
            public short
                BRAND_CODE
            { get; set; }
            public DateTime
                ACCOUNT_CREATED
            { get; set; }
            public string
                KEY_DETAILS
            { get; set; }
            public int
                RANDOMKEY
            { get; set; }
            public string
                DISCOUNT_TYPE
            { get; set; }
            public short
                DISCOUNT_VAT_CODE
            { get; set; }
            public int
                DISCOUNT_AMOUNT
            { get; set; }
            public bool
                Updated = false;
        }

        internal class GDiscounts
        {
            internal string
                USERNAME = "";
            internal char
                CUBEFACE_CODE = SmartParametersV2016.defaultChar;
            internal short
                SUPPLIER_CODE = 0;
            internal short
                BRAND_CODE = 0;
            internal DateTime
                ACCOUNT_CREATED = SmartParametersV2016.defaultDate;
            internal string
                ACCOUNT_NO = "";
            internal string
                STATEMENT_ID = "";
            internal DateTime
                BILL_DATE = SmartParametersV2016.defaultDate;
            internal string
                MPAN_MPRN = "";
            internal DateTime
                DISCOUNT_DATE = SmartParametersV2016.defaultDate;          // This is the BILL_PERIOD_END surely?
            internal short
                DISCOUNT_ITEM = 0;
            internal int
                RANDOMKEY = 0;
            internal string
                DISCOUNT_TYPE = "";
            internal short
                DISCOUNT_VAT_CODE = 0;
            internal int
                DISCOUNT_AMOUNT = 0;        // Exc VAT
            internal bool
                Updated = false;
        }

        public class GReadingsSQLite
        {
            public string
                USERNAME
            { get; set; }
            public string
                CUBEFACE_CODE
            { get; set; }
            public short
                SUPPLIER_CODE
            { get; set; }
            public short
                BRAND_CODE
            { get; set; }
            public DateTime
                ACCOUNT_CREATED
            { get; set; }
            public string
                KEY_DETAILS
            { get; set; }
            public int
                RANDOMKEY1
            { get; set; }
            public string
                DETAILS                // <= Why MAKE UP a fucking key
            { get; set; }
            public int
                RANDOMKEY2
            { get; set; }
            public string
                READ_TYPE
            { get; set; }
            public decimal          // Because we will never have any 'number' problems if its like this
                D_LAST_READ
            { get; set; }
            public decimal
                D_THIS_READ
            { get; set; }
            public decimal
                D_UNITS_USED_M3
            { get; set; }
            public string
                UNIT_OF_MEASURE
            { get; set; }
            public decimal
                D_UNITS_USED_KWH
            { get; set; }
            public decimal
                CALORIFIC_VALUE
            { get; set; }
            public bool
                Updated = false;
        }

        internal class GReadings
        {
            internal string
                USERNAME = "";
            internal char
                CUBEFACE_CODE = SmartParametersV2016.defaultChar;
            internal short
                SUPPLIER_CODE = 0;
            internal short
                BRAND_CODE = 0;
            internal DateTime
                ACCOUNT_CREATED = SmartParametersV2016.defaultDate;
            internal string
                ACCOUNT_NO = "";
            internal string
                STATEMENT_ID = "";
            internal DateTime
                BILL_DATE = SmartParametersV2016.defaultDate;                // <= Why MAKE UP a fucking key
            internal string
                MPAN_MPRN = "";
            internal DateTime
                READINGS_PERIOD_START = SmartParametersV2016.defaultDate,      // When you can use theirs???
                READINGS_PERIOD_END = SmartParametersV2016.defaultDate;        // Used in FU to allocate consumption values to readings
            internal int
                RANDOMKEY1 = 0;
            internal string
                METER_SERIAL_NO = "";            // Max size 14
            internal int
                RANDOMKEY2 = 0;
            internal string
                READ_TYPE = "";
            internal decimal                // We should never have any trouble now, should we ??!?
                D_LAST_READ = 0.0M,
                D_THIS_READ = 0.0M;
            internal decimal
                D_UNITS_USED_M3 = 0.0M;
            internal string
                UNIT_OF_MEASURE = "";
            internal decimal
                D_UNITS_USED_KWH = 0.0M,
                CALORIFIC_VALUE = 0.0M;
            internal bool
                Updated = false;
        }

        internal class DReadingsCharts : IComparable<DReadingsCharts>
        {
            internal DateTime
                PERIOD_END = SmartParametersV2016.defaultDate;
            internal decimal
                UNITS_USED = 0.0M;
            internal string
                METER_SERIAL_NO = "";

            internal DReadingsCharts(DateTime Read_Date,
                                    decimal Units_Used,
                                    string Meter_Serial_No)
            {
                PERIOD_END = Read_Date;
                UNITS_USED = Units_Used;
                METER_SERIAL_NO = Meter_Serial_No;
            }

            #region Icomparable<DReadingsCharts> Members
            public int CompareTo(DReadingsCharts other)
            {
                // Need to check this breaking change!!
                int compareResult = 0;

                if (other != null)
                {
                    if (PERIOD_END < other.PERIOD_END)
                    {
                        compareResult = -1;
                    }
                    else
                    {
                        if (this.PERIOD_END > other.PERIOD_END)
                        {
                            compareResult = 1;
                        }
                        //else
                        //{
                        //    compareResult = 0;
                        //}
                    }
                }
                return compareResult;
            }

            public static int Compare(DReadingsCharts left, DReadingsCharts right)
            {
                if (object.ReferenceEquals(left, right))
                {
                    return 0;
                }
                if (left is null)
                {
                    return -1;
                }
                return left.CompareTo(right);
            }

            public override bool Equals(object obj)
            {
                DReadingsCharts other = (DReadingsCharts)obj; //avoid double casting
                if (other is null)
                {
                    return false;
                }
                return this.CompareTo(other) == 0;
            }

            public static bool operator ==(DReadingsCharts left, DReadingsCharts right)
            {
                if (left is null)
                {
                    return right is null;
                }
                return left.Equals(right);
            }
            public static bool operator !=(DReadingsCharts left, DReadingsCharts right)
            {
                return !(left == right);
            }
            public static bool operator <(DReadingsCharts left, DReadingsCharts right)
            {
                return (Compare(left, right) < 0);
            }
            public static bool operator >(DReadingsCharts left, DReadingsCharts right)
            {
                return (Compare(left, right) > 0);
            }
            public override int GetHashCode()
            {
                int date = this.PERIOD_END.GetHashCode();
                return (int)date;
            }
            #endregion
        }

        public class GStandingChargesSQLite
        {
            public string
                USERNAME
            { get; set; }
            public string
                CUBEFACE_CODE
            { get; set; }
            public short
                SUPPLIER_CODE
            { get; set; }
            public short
                BRAND_CODE
            { get; set; }
            public DateTime
                ACCOUNT_CREATED
            { get; set; }
            public string
                KEY_DETAILS
            { get; set; }
            public int
                RANDOMKEY
            { get; set; }
            public string
                CHARGES_TYPE
            { get; set; }
            public decimal
                STANDING_CHARGE
            { get; set; }
            public short
                CHARGES_DAYS
            { get; set; }
            public int
                CHARGES_COST       // Exc VAT
            { get; set; }
            public bool
                Updated = false;
        }

        internal class GStandingCharges
        {
            internal string
                USERNAME = "";
            internal char
                CUBEFACE_CODE = SmartParametersV2016.defaultChar;
            internal short
                SUPPLIER_CODE = 0;
            internal short
                BRAND_CODE = 0;
            internal DateTime
                ACCOUNT_CREATED = SmartParametersV2016.defaultDate;
            internal string
                ACCOUNT_NO = "";
            internal string
                STATEMENT_ID = "";
            internal DateTime
                BILL_DATE = SmartParametersV2016.defaultDate;                      // <= Why MAKE UP a fucking key
            internal string
                MPAN_MPRN = "";
            internal DateTime
                STANDING_CHARGES_PERIOD_START = SmartParametersV2016.defaultDate,  // <= when you can use theirs?
                STANDING_CHARGES_PERIOD_END = SmartParametersV2016.defaultDate;
            internal short
                CHARGES_ITEM = 0;           // Because you could get more than one Standing Charge for any date range
            internal int
                RANDOMKEY = 0;
            internal string
                CHARGES_TYPE = "";
            internal decimal
                STANDING_CHARGE = 0.0M;
            internal short
                CHARGES_DAYS = 0;
            internal int
                CHARGES_COST = 0;           // Exc VAT
            internal bool
                Updated = false;
        }

        public class GUnitChargesSQLite
        {
            // There is no PERIOD_END because ... which one would we put it against?
            // when we have two readings on two different dates against one rate?
            public string
                USERNAME
            { get; set; }
            public string
                CUBEFACE_CODE
            { get; set; }
            public short
                SUPPLIER_CODE
            { get; set; }
            public short
                BRAND_CODE
            { get; set; }
            public DateTime
                ACCOUNT_CREATED
            { get; set; }
            public string
                KEY_DETAILS
            { get; set; }
            public int
                RANDOMKEY
            { get; set; }
            public string
                UNITS_TYPE
            { get; set; }
            public decimal                    // described as numeric in DB (they are interchangeable)
                UNITS
            { get; set; }
            public decimal
                UNITS_RATE
            { get; set; }
            public string
                UNIT_OF_MEASURE
            { get; set; }
            public int
                UNITS_COST         //  Exc VAT
            { get; set; }
            public bool
                Updated = false;
        }
        internal class GUnitCharges : IEquatable<SmartUtility.GUnitCharges>
        {
            // There is no PERIOD_END because ... which one would we put it against?
            // when we have two readings on two different dates against one rate?
            internal string
                USERNAME = "";
            internal char
                CUBEFACE_CODE = SmartParametersV2016.defaultChar;
            internal short
                SUPPLIER_CODE = 0;
            internal short
                BRAND_CODE = 0;
            internal DateTime
                ACCOUNT_CREATED = SmartParametersV2016.defaultDate;
            internal string
                ACCOUNT_NO = "";
            internal string
                STATEMENT_ID = "";
            internal DateTime
                BILL_DATE = SmartParametersV2016.defaultDate;                     // <= Why MAKE UP a fucking key
            internal string
                MPAN_MPRN = "";
            internal DateTime
                UNIT_CHARGES_PERIOD_START = SmartParametersV2016.defaultDate,      // <= when you can use theirs?
                UNIT_CHARGES_PERIOD_END = SmartParametersV2016.defaultDate;
            internal short
                UNITS_BAND = 0;
            internal int
                RANDOMKEY = 0;
            internal string
                UNITS_TYPE = "";
            internal decimal                    // described as numeric in DB (they are interchangeable)
                UNITS = 0.0M,
                UNITS_RATE = 0.0M;
            internal string
                UNIT_OF_MEASURE = "";
            internal int
                UNITS_COST = 0;         // Exc VAT            
            internal bool
                Updated = false;

            // If Equals() returns true for a pair of objects 
            // then GetHashCode() must return the same value for these objects.
            public override int GetHashCode()
            {
                //Get hash code for the Username field if it is not null.
                int hashUsername = USERNAME.GetHashCode();
                //Get hash code for the Category Code field.
                int hashCategoryCode = CUBEFACE_CODE.GetHashCode();
                //Get hash code for the Supplier Code field.
                int hashSupplierCode = SUPPLIER_CODE.GetHashCode();
                //Get hash code for the Supplier Code field.
                int hashBrandCode = BRAND_CODE.GetHashCode();
                //Get hash code for the Created field if it is not null.
                int hashCreated = ACCOUNT_CREATED.GetHashCode();
                //Get hash code for the AccountNo field if it is not null.
                int hashAccountNo = ACCOUNT_NO.GetHashCode();
                //Get hash code for the BillNumber field if it is not null.
                int hashBillNumber = STATEMENT_ID.GetHashCode();
                //Get hash code for the BillDate field if it is not null.
                int hashBillDate = BILL_DATE.GetHashCode();
                //Get hash code for the Mpan_Mprn field.
                int hashMpanMprn = MPAN_MPRN.GetHashCode();
                //Get hash code for the UNits Time field if it is not null.
                int hashUnitsBand = UNITS_BAND.GetHashCode();
                //Get hash code for the Units Rate field if it is not null.
                int hashUnitsRate = UNITS_RATE.GetHashCode();

                //Calculate the hash code for the product.
                return hashUsername ^
                        hashCategoryCode ^
                        hashSupplierCode ^
                        hashBrandCode ^
                        hashCreated ^
                        hashAccountNo ^
                        hashBillNumber ^
                        hashBillDate ^
                        hashMpanMprn ^
                        hashUnitsBand ^
                        hashUnitsRate;
            }

            // Records are equal if the fields in this module are equal
            public bool Equals(GUnitCharges other)
            {
                // Check whether any of the compared objects is null.
                if (other is null)
                {
                    return false;
                }
                // Check whether the records fields are equal.
                if (USERNAME == other.USERNAME &&
                    CUBEFACE_CODE == other.CUBEFACE_CODE &&
                    SUPPLIER_CODE == other.SUPPLIER_CODE &&
                    BRAND_CODE == other.BRAND_CODE &&
                    ACCOUNT_CREATED == other.ACCOUNT_CREATED &&
                    ACCOUNT_NO == other.ACCOUNT_NO &&
                    STATEMENT_ID == other.STATEMENT_ID &&
                    BILL_DATE == other.BILL_DATE &&
                    MPAN_MPRN == other.MPAN_MPRN &&
                    UNITS_BAND == other.UNITS_BAND &&
                    UNITS_RATE == other.UNITS_RATE)
                {
                    return true;
                }
                return false;
            }
            public override bool Equals(object obj)
            {
                return Equals((GUnitCharges)obj);
            }
        }

        public class GUsageSQLite
        {
            public string
                USERNAME
            { get; set; }
            public string
                CUBEFACE_CODE
            { get; set; }
            public string
                KEY_DETAILS
            { get; set; }
            public int
                RANDOMKEY
            { get; set; }
            public DateTime
                USAGE_DATETIME
            { get; set; }
            public decimal
                USAGE_TOTAL
            { get; set; }
            public decimal
                USAGE_VALUE
            { get; set; }
            public bool
                Updated = false;
        }

        internal class GUsage
        {
            internal string
                USERNAME = "";
            internal char
                CUBEFACE_CODE = SmartParametersV2016.defaultChar;
            internal string
                MPAN_MPRN = "";
            internal short
                UNIQUE_INDEX = 0;
            internal int
                RANDOMKEY = 0;
            internal DateTime
                USAGE_DATETIME = SmartParametersV2016.defaultDate;
            internal decimal
                USAGE_TOTAL = 0.0M;
            internal decimal
                USAGE_VALUE = 0;
            internal bool
                Updated = false;
        }

        internal class GUsageView
        {
            internal string
                USERNAME = "";        // For completeness
            internal char
                CUBEFACE_CODE = SmartParametersV2016.defaultChar;
            internal short
                SUPPLIER_CODE = 0;
            internal DateTime
                ACCOUNT_CREATED = SmartParametersV2016.defaultDate;
            internal string
                ACCOUNT_NO = "";
            internal string
                MPAN_MPRN = "";
            internal string
                METER_SERIAL_NO = "";
            internal DateTime
                USAGE_DATETIME = SmartParametersV2016.defaultDate;
            internal decimal
                USAGE_TOTAL = 0.0M;
            internal decimal
                USAGE_VALUE = 0.0M;
        }

        public class LoginsSQLite
        {
            //  I am tempted to take the USERNAME out for Production,
            //  but am holding fire until I get Multi-Meters - who can manipulate
            //  lots of different Usernames sorted out ....
            public string
                USERNAME                              // Key to link to all of SmartSwitch
            { get; set; }
            public string
                CUBEFACE_CODE     // Key to link to everything
            { get; set; }
            //public string
            //RESOURCE_CODE
            //{ get; set; }
            public short
                SUPPLIER_CODE
            { get; set; }
            public short
                BRAND_CODE
            { get; set; }
            public DateTime
                LOGIN_CREATED
            { get; set; }
            public string
                DETAILS
            { get; set; }
            public int
                RANDOMKEY
            { get; set; }
            public bool
                Updated = false;    // Lower case so it doesn't go out to the table
        }

        // See the rationale in Things Done for 28th June 2020 as to why we don't need
        // RESOURCE_CODES and RESOURCE_TYPEs in this table
        internal class Logins
        {
            //  I am tempted to taked the USERNAME out for Production,
            //  but am holding fire until I get Multi-Meters - who can manipulate
            //  lots of different Usernames sorted out ....
            internal string
                USERNAME = "";                                // Key to link to all of SmartSwitch
            internal char
                CUBEFACE_CODE = SmartParametersV2016.defaultChar;     // Key to link to everything
            //internal char
            //    RESOURCE_CODE = SmartParametersV2016.defaultUtilityResourceCode; // Its a "D" !!
            internal short
                SUPPLIER_CODE = 0,
                BRAND_CODE = 0;
            internal DateTime
                LOGIN_CREATED = SmartParametersV2016.defaultDate;
            internal string
                USER_ID = "",
                USER_PASSWORD = "";
            internal int
                RANDOMKEY = 0;
            internal bool
                Updated = false;    // Lower case so it doesn't go out to the table
        }

        public class MetersSQLite
        {
            public string
                USERNAME
            { get; set; }
            public string
                CUBEFACE_CODE            // Key
            { get; set; }
            public string
                RESOURCE_CODE              // Key
            { get; set; }
            public string
                RESOURCE_TYPE              // Key
            { get; set; }
            public string
                KEY_DETAILS                // Key  // Which SHOULD never change
            { get; set; }
            public int
                RANDOMKEY1
            { get; set; }
            public string
                DETAILS            // Which might well change
            { get; set; }
            public int
                RANDOMKEY2                      // 8 numeric chars no, might be bigger
            { get; set; }                                          // AS might this? Why? Why would the address change?  Because they move house!
            public bool
                Updated = false;    // Lower case so it doesn't go out to the table
        }

        internal class Meters
        {
            internal string
                USERNAME = "";
            internal char
                CUBEFACE_CODE = SmartParametersV2016.defaultChar;              // Key
            internal char
                RESOURCE_CODE = SmartParametersV2016.defaultChar;              // Key
            internal string
                RESOURCE_TYPE = "";              // Key
            internal string
                MPAN_MPRN = "";                 // Key  // Which SHOULD never change
            internal int
                RANDOMKEY1 = 0;
            internal string
                METER_SERIAL_NO = "";            // Which might well change
            //internal string                                 // AS might this? Why? Why would the address change?  Because they move house!
            //    UDPRN = "";                      // 8 numeric chars
            internal int
                RANDOMKEY2 = 0;
            internal bool
                Updated = false;    // Lower case so it doesn't go out to the table
        }

        public class PaymentsSQLite
        {
            public string
                USERNAME
            { get; set; }
            public string
                CUBEFACE_CODE
            { get; set; }
            public short
                SUPPLIER_CODE
            { get; set; }
            public short
                BRAND_CODE
            { get; set; }
            public DateTime
                ACCOUNT_CREATED
            { get; set; }
            public string
                KEY_DETAILS
            { get; set; }
            public int
                RANDOMKEY1
            { get; set; }
            public string
                DETAILS
            { get; set; }
            public int
                RANDOMKEY2
            { get; set; }
            public bool
                Updated = false;
        }
        internal class Payments : IComparable<Payments>
        {
            internal string
                USERNAME = "";
            internal char
                CUBEFACE_CODE = SmartParametersV2016.defaultChar;
            internal short
                SUPPLIER_CODE = 0;
            internal short
                BRAND_CODE = 0;
            internal DateTime
                ACCOUNT_CREATED = SmartParametersV2016.defaultDate;
            internal string
                ACCOUNT_NO = "";
            internal string
                STATEMENT_ID = "";
            internal DateTime
                BILL_DATE = SmartParametersV2016.defaultDate;
            internal DateTime
                PAYMENT_DATE = SmartParametersV2016.defaultDate;
            internal short
                PAYMENT_ITEM = 0;
            internal int
                RANDOMKEY1 = 0;
            internal short
                PAYMENT_CODE = 0;
            internal int
                PAYMENT_AMOUNT = 0,         // Exc VAT!
                PAYMENT_BALANCE = 0;
            internal int
                RANDOMKEY2 = 0;
            internal bool
                Updated = false;

            #region Icomparable<SmartUtility.Payments> Members
            public int CompareTo(Payments other)
            {
                // Need yo check this breaking change!!!
                int compareResult = 0;

                if (other != null)
                {
                    if (PAYMENT_DATE < other.PAYMENT_DATE)
                    {
                        compareResult = -1;
                    }
                    else
                    {
                        if (PAYMENT_DATE > other.PAYMENT_DATE)
                        {
                            compareResult = 1;
                        }
                        //else
                        //{
                        //    compareResult = 0;
                        //}
                    }
                }
                return compareResult;
            }

            public static int Compare(Payments left, Payments right)
            {
                if (object.ReferenceEquals(left, right))
                {
                    return 0;
                }
                if (left is null)
                {
                    return -1;
                }
                return left.CompareTo(right);
            }

            public override bool Equals(object obj)
            {
                Payments other = (Payments)obj; //avoid double casting
                if (other is null)
                {
                    return false;
                }
                return this.CompareTo(other) == 0;
            }
            public override int GetHashCode()
            {
                int date = this.PAYMENT_DATE.GetHashCode();
                return (int)date;
            }

            public static bool operator ==(Payments left, Payments right)
            {
                if (left is null)
                {
                    return right is null;
                }
                return left.Equals(right);
            }
            public static bool operator !=(Payments left, Payments right)
            {
                return !(left == right);
            }
            public static bool operator <(Payments left, Payments right)
            {
                return (Compare(left, right) < 0);
            }
            public static bool operator >(Payments left, Payments right)
            {
                return (Compare(left, right) > 0);
            }

            #endregion
        }

        public class ResourcesSQLite
        {
            public string
                USERNAME                                // Key to link to all of SmartSwitch
            { get; set; }
            public string
                CUBEFACE_CODE      // Key to link to everything
            { get; set; }
            public string
                RESOURCE_CODE
            { get; set; }
            //public string
            //    RESOURCE_TYPE
            // { get; set; }
            public string
                CHECKED
            { get; set; }
            public decimal
                 TOTAL_USAGE
            { get; set; }
            public DateTime
                UNITRATES_UPDATED
            { get; set; }
            public DateTime
               LAST_DATETIME
            { get; set; }
            public DateTime
                LAST_UPDATE
            { get; set; }
            public DateTime
                EXPIRY_DATE
            { get; set; }
            public string
                AUTOSWITCH
            { get; set; }
            public bool
                Updated = false;    // Lower case so it doesn't go out to the table
        }

        internal class Resources
        {
            internal string
                USERNAME = "";                                // Key to link to all of SmartSwitch
            internal char
                CUBEFACE_CODE = SmartParametersV2016.defaultChar;       // Key to link to everything
            internal char
                RESOURCE_CODE = SmartParametersV2016.defaultResourceCode;
            //internal string
            //    RESOURCE_TYPE = "";
            internal string     // Because it might be Empty!
                CHECKED = "";
            internal decimal
                TOTAL_USAGE = 0m;
            internal DateTime
                UNITRATES_UPDATED = SmartParametersV2016.defaultDate;
            internal DateTime
                LAST_DATETIME = SmartParametersV2016.defaultDate,
                LAST_UPDATE = SmartParametersV2016.defaultDate,
                EXPIRY_DATE = SmartParametersV2016.defaultDate;
            internal char
                AUTOSWITCH = SmartParametersV2016.defaultAutoSwitch;
            internal bool
                Updated = false;    // Lower case so it doesn't go out to the table
        }

        public class ResourcesTypesSQLite
        {
            public string
                USERNAME                               // Key to link to all of SmartSwitch
            { get; set; }
            public string
                CUBEFACE_CODE       // Key to link to everything
            { get; set; }
            public string
                RESOURCE_CODE
            { get; set; }
            public string
                RESOURCE_TYPE
            { get; set; }
            public bool
                Updated = false;    // Lower case so it doesn't go out to the table
        }

        internal class ResourcesTypes
        {
            internal string
                USERNAME = "";                                // Key to link to all of SmartSwitch
            internal char
                CUBEFACE_CODE = SmartParametersV2016.defaultChar;       // Key to link to everything
            internal char
                RESOURCE_CODE = SmartParametersV2016.defaultResourceCode;
            internal string
                RESOURCE_TYPE = "";
            internal bool
                Updated = false;    // Lower case so it doesn't go out to the table
        }

        public class SupChargesCreditsSQLite
        {
            public string
                USERNAME
            { get; set; }
            public string
                CUBEFACE_CODE
            { get; set; }
            public short
                SUPPLIER_CODE
            { get; set; }
            public short
                BRAND_CODE
            { get; set; }
            public DateTime
                ACCOUNT_CREATED
            { get; set; }
            public string
                KEY_DETAILS
            { get; set; }
            public int
                RANDOMKEY1
            { get; set; }
            public string
                DETAILS
            { get; set; }
            public int
                RANDOMKEY2
            { get; set; }
            public bool
                Updated = false;
        }
        internal class SupChargesCredits
        {
            internal string
                USERNAME = "";
            internal char
                CUBEFACE_CODE = SmartParametersV2016.defaultChar;
            internal short
                SUPPLIER_CODE = 0;
            internal short
                BRAND_CODE = 0;
            internal DateTime
                ACCOUNT_CREATED = SmartParametersV2016.defaultDate;
            internal string
                ACCOUNT_NO = "";
            internal string
                STATEMENT_ID = "";
            internal DateTime
                BILL_DATE = SmartParametersV2016.defaultDate;
            internal DateTime
                SUPPLY_DATE = SmartParametersV2016.defaultDate;
            internal short
                SUPPLY_ITEM = 0;
            internal int
                RANDOMKEY1 = 0;
            internal string
                SUPPLY_TYPE = "";
            internal short
                SUPPLY_VAT_CODE = 0;        // Either 01 or 02 but usually 01 (0.00%)
            internal int
                SUPPLY_AMOUNT = 0;          // Exc VAT
            internal DateTime
                SUPPLY_DUE_DATE = SmartParametersV2016.defaultDate;
            internal string
                SUPPLY_CREDIT_BILL = "";
            internal int
                RANDOMKEY2 = 0;
            internal bool
                Updated = false;
        }

        public class SwitchesSQLite
        {
            //  I am tempted to taked the USERNAME out for Production,
            //  but am holding fire until I get Multi-Meters - who can manipulate
            //  lots of different Usernames sorted out ....
            public string
                USERNAME                               // Key to link to all of SmartSwitch
            { get; set; }
            public string
                CUBEFACE_CODE    // Key to link to everything
            { get; set; }
            public short
                SUPPLIER_CODE                                      // Key to link to Utility.Accounts
            { get; set; }
            public short
                BRAND_CODE                                         // Key
            { get; set; }
            public string
                RESOURCE_CODE
            { get; set; }
            public DateTime
                SWITCH_CREATED
            { get; set; }
            public string
                KEY_DETAILS                              // Key to link to Utility.Accounts
            { get; set; }
            public int
                RANDOMKEY
            { get; set; }
            public string
                AGE_60PLUS
            { get; set; }
            public string
                AUTOSWITCH_DESTINATION     // Where we sent the e-mail
            { get; set; }
            public short
                AUTOSWITCH_SUPPLIER_CODE   // Which Supplier we chose to switch to
            { get; set; }
            public short
                AUTOSWITCH_BRAND_CODE   // Which Brand we chose to switch to
            { get; set; }
            public int
                AUTOSWITCH_TARIFF_CODE     // Which tariff code we chose to switch to
            { get; set; }
            public DateTime
                AUTOSWITCH_EMAIL_SENT      // When we sent the e-mail
            { get; set; }
            public bool
                Updated = false;
        }

        internal class Switches
        {
            //  I am tempted to taked the USERNAME out for Production,
            //  but am holding fire until I get Multi-Meters - who can manipulate
            //  lots of different Usernames sorted out ....
            internal string
                USERNAME = "";                                // Key to link to all of SmartSwitch
            internal char
                CUBEFACE_CODE = SmartParametersV2016.defaultChar;     // Key to link to everything
            internal short
                SUPPLIER_CODE = 0,                                      // Key to link to Utility.Accounts
                BRAND_CODE = 0;                                         // Key
            internal char
                RESOURCE_CODE = SmartParametersV2016.defaultChar;
            internal DateTime
                SWITCH_CREATED = SmartParametersV2016.defaultDate;
            internal string
                ACCOUNT_NO = "";                              // Key to link to Utility.Accounts
            internal string
                MPAN_MPRN = "";
            internal int
                RANDOMKEY = 0;
            internal string     // Because its sometimes Empty!
                AGE_60PLUS = "";
            internal string
                AUTOSWITCH_DESTINATION = "";     // Where we sent the e-mail
            internal short
                AUTOSWITCH_SUPPLIER_CODE = 0;   // Which Supplier we chose to switch to
            internal short
                AUTOSWITCH_BRAND_CODE = 0;   // Which Brand we chose to switch to
            internal int
                AUTOSWITCH_TARIFF_CODE = 0;     // Which tariff code we chose to switch to
            internal DateTime
                AUTOSWITCH_EMAIL_SENT = SmartParametersV2016.defaultDate;      // When we sent the e-mail
            internal bool
                Updated = false;    // Lower case so it doesn't go out to the table
        }

        public class TariffDetailsSQLite
        {
            public string
                USERNAME    // Key
            { get; set; }
            public string
                CUBEFACE_CODE              // Key
            { get; set; }
            public short
                SUPPLIER_CODE              // Key
            { get; set; }
            public short
                BRAND_CODE                 // Key
            { get; set; }
            public DateTime
                ACCOUNT_CREATED                    // Key
            { get; set; }
            public string
                KEY_DETAILS                 // Key
            { get; set; }
            public int
                RANDOMKEY1               // Key
            { get; set; }
            public string
                DETAILS                  // Key
            { get; set; }
            public int
                RANDOMKEY2                  // Key
            { get; set; }
            public bool
                Updated = false;
        }

        internal class TariffDetails
        {
            internal string
                USERNAME = "";    // Key
            internal char
                CUBEFACE_CODE = SmartParametersV2016.defaultChar;              // Key
            internal short
                SUPPLIER_CODE = 0;              // Key
            internal short
                BRAND_CODE = 0;                 // Key
            internal DateTime
                ACCOUNT_CREATED = SmartParametersV2016.defaultDate;                    // Key
            internal string
                ACCOUNT_NO = "";                 // Key
            internal string
                STATEMENT_ID = "";               // Key
            internal DateTime
                BILL_DATE = SmartParametersV2016.defaultDate;                  // Key
            internal string
                MPAN_MPRN = "";                  // Key
            internal DateTime
                PRICES_VALID_FROM = SmartParametersV2016.defaultDate,          // Key
                PRICES_VALID_TO = SmartParametersV2016.defaultDate;            // Key
            internal int
                TARIFF_CODE = 0;                // Key
            internal char
                PAYMENT_PLAN = SmartParametersV2016.defaultChar;               // Key
            internal short
                TIER_LEVEL = 0;                 // Key
            internal int
                RANDOMKEY1 = 0;
            internal short
                AREA_CODE = 0;
            internal string
                SC = "",
                DR = "",
                NR = "";
            internal string
                TCR = "";        // <= New one should, be in there without trailing 'p'
            internal int
                RANDOMKEY2 = 0;
            internal bool
                Updated = false;
        }

        public class UnallocatedSQLite
        {
            public string
                USERNAME
            { get; set; }
            public string
                CUBEFACE_CODE
            { get; set; }
            public short
                SUPPLIER_CODE
            { get; set; }
            public short
                BRAND_CODE = 0;
            public DateTime
                ACCOUNT_CREATED
            { get; set; }
            public string
                KEY_DETAILS
            { get; set; }
            public int
                RANDOMKEY1
            { get; set; }
            public string
                DETAILS
            { get; set; }
            public int
                RANDOMKEY2
            { get; set; }
            public bool
                Updated = false;
        }

        internal class Unallocated : IComparable<Unallocated>
        {
            internal string
                USERNAME = "";
            internal char
                CUBEFACE_CODE = SmartParametersV2016.defaultChar;
            internal short
                SUPPLIER_CODE = 0;
            internal short
                BRAND_CODE = 0;
            internal DateTime
                ACCOUNT_CREATED = SmartParametersV2016.defaultDate;
            internal string
                ACCOUNT_NO = "";
            internal string
                STATEMENT_ID = "";
            internal DateTime
                BILL_DATE = SmartParametersV2016.defaultDate;
            internal DateTime
                PAYMENT_DATE = SmartParametersV2016.defaultDate;
            internal short
                PAYMENT_ITEM = 0;
            internal int
                RANDOMKEY1 = 0;
            internal short
                PAYMENT_CODE = 0;
            internal int
                PAYMENT_AMOUNT = 0,         // Exc VAT!
                PAYMENT_BALANCE = 0;
            internal int
                RANDOMKEY2 = 0;
            internal bool
                Updated = false;
            #region Icomparable<SmartUtility.Unallocated> Members
            public int CompareTo(Unallocated other)
            {
                // Need to test this breaking change!!!

                int compareResult = 0;

                if (other != null)
                {
                    if (PAYMENT_DATE < other.PAYMENT_DATE)
                    {
                        compareResult = -1;
                    }
                    else
                    {
                        if (PAYMENT_DATE > other.PAYMENT_DATE)
                        {
                            compareResult = 1;
                        }
                        //else
                        //{
                        //    compareResult = 0;
                        //}
                    }
                }
                return compareResult;
            }

            public static int Compare(Unallocated left, Unallocated right)
            {
                if (object.ReferenceEquals(left, right))
                {
                    return 0;
                }
                if (left is null)
                {
                    return -1;
                }
                return left.CompareTo(right);
            }

            public override bool Equals(object obj)
            {
                Unallocated other = (Unallocated)obj; //avoid double casting
                if (other is null)
                {
                    return false;
                }
                return this.CompareTo(other) == 0;
            }
            public override int GetHashCode()
            {
                int date = this.PAYMENT_DATE.GetHashCode();
                return (int)date;
            }

            public static bool operator ==(Unallocated left, Unallocated right)
            {
                if (left is null)
                {
                    return right is null;
                }
                return left.Equals(right);
            }
            public static bool operator !=(Unallocated left, Unallocated right)
            {
                return !(left == right);
            }
            public static bool operator <(Unallocated left, Unallocated right)
            {
                return (Compare(left, right) < 0);
            }
            public static bool operator >(Unallocated left, Unallocated right)
            {
                return (Compare(left, right) > 0);
            }

            #endregion
        }

        internal class TariffCodesNames
        {
            internal char
                CUBEFACE_CODE = SmartParametersV2016.defaultChar;
            internal short
                BRAND_CODE = 0;
            internal char
                RESOURCE_CODE = SmartParametersV2016.defaultResourceCode;
            internal string
                RESOURCE_TYPE = "";
            internal int
                TARIFF_CODE = 0;
            internal short
                VERSION_CODE = 0;
            internal string
                PLACEHOLDER = "";
            internal string
                TARIFF_NAME = "",
                TARIFF_PREVIOUS_NAME = "";
            internal char
                AREA_10 = SmartParametersV2016.defaultChar,
                AREA_11 = SmartParametersV2016.defaultChar,
                AREA_12 = SmartParametersV2016.defaultChar,
                AREA_13 = SmartParametersV2016.defaultChar,
                AREA_14 = SmartParametersV2016.defaultChar,
                AREA_15 = SmartParametersV2016.defaultChar,
                AREA_16 = SmartParametersV2016.defaultChar,
                AREA_17 = SmartParametersV2016.defaultChar,
                AREA_18 = SmartParametersV2016.defaultChar,
                AREA_19 = SmartParametersV2016.defaultChar,
                AREA_20 = SmartParametersV2016.defaultChar,
                AREA_21 = SmartParametersV2016.defaultChar,
                AREA_22 = SmartParametersV2016.defaultChar,
                AREA_23 = SmartParametersV2016.defaultChar;
        }

        internal class BrandsView
        {
            internal char
                 CUBEFACE_CODE = SmartParametersV2016.defaultChar;
            internal char
                 RESOURCE_CODE = SmartParametersV2016.defaultChar;
            internal string
                 RESOURCE_TYPE = "";
            internal short
                SUPPLIER_CODE = 0;
            internal short
                BRAND_CODE = 0;
            internal string
                BRAND_NAME = "";
        }

        internal class TariffCodesNamesView
        {
            internal char
                CUBEFACE_CODE = SmartParametersV2016.defaultChar;
            internal short
                BRAND_CODE = 0;
            internal char
                RESOURCE_CODE = SmartParametersV2016.defaultResourceCode;
            internal int
                TARIFF_CODE = 0;
            internal short
                VERSION_CODE = 0;
            internal string
                TARIFF_NAME = "",
                MES_NAME = "";
        }

        internal class AmeliasView
        {
            //  I am tempted to taked the USERNAME out for Production,
            //  but am holding fire until I get Multi-Meters - who can manipulate
            //  lots of different Usernames sorted out ....
            internal string
                USERNAME = "";                                // Key to link to all of SmartSwitch
            internal char
                CUBEFACE_CODE = SmartParametersV2016.defaultChar;     // Key to link to everything
            internal char
                RESOURCE_CODE = SmartParametersV2016.defaultChar;
            internal string
                RESOURCE_TYPE = "";
            internal short
                SUPPLIER_CODE = 0,
                BRAND_CODE = 0;
            internal DateTime
                ACCOUNT_CREATED = SmartParametersV2016.defaultDate;
            internal DateTime
                NEXT_CONNECTION = SmartParametersV2016.defaultDate;
            internal string
                ACCOUNT_NO = "",
                MPAN_MPRN = "",
                METER_SERIAL_NO = "",
                UDPRN = "";           // 8 numeric chars
            internal decimal
                TOTAL_USAGE = 0.0M;
            internal DateTime
                UNITRATES_UPDATED = SmartParametersV2016.defaultDate;
            internal short
                AREA_CODE = 0;
            internal string
                POSTCODE = "";
            internal DateTime
                LOGIN_CREATED = SmartParametersV2016.defaultDate;
            internal string
                USER_ID = "",            // Mixed case so it gets decrypted <= NO LONGER APPLIES!
                USER_PASSWORD = "";           // Mixed case so it gets decrypted <= NO LONGER APPLIES!
            internal string     // May be Empty sometimes!!
                CHECKED = "";
            internal DateTime
                LAST_DATETIME = SmartParametersV2016.defaultDate;
            internal DateTime
                LAST_UPDATE = SmartParametersV2016.defaultDate;
            internal DateTime
                EXPIRY_DATE = SmartParametersV2016.defaultDate;
            internal int
                TARIFF_CODE = 0;
            internal char
                PAYMENT_PLAN = SmartParametersV2016.defaultChar;
            internal char
                STATUS = SmartParametersV2016.defaultChar;                 // Open = 'Y', Switching = 'S', Closed = 'N'            
            internal char     // Because its never Empty!
                AUTOSWITCH = SmartParametersV2016.defaultChar;
        }

        internal class TariffCosts : IComparable<TariffCosts>
        {
            internal string
                USERNAME = "";
            internal char
                CUBEFACE_CODE = SmartParametersV2016.defaultChar;
            internal short
                SUPPLIER_CODE = 0;
            internal string
                ACCOUNT_NO = "";
            internal DateTime
                PERIOD_END = SmartParametersV2016.defaultDate;
            internal string
                CODE = "";
            internal DateTime
                UPPER_DATE = SmartParametersV2016.defaultDate;
            internal short
                ITEM_COUNT = 0;
            internal decimal
                D_UNITS = 0.0M,
                D_UNITS_RATE = 0.0M,
                N_UNITS = 0.0M,
                N_UNITS_RATE = 0.0M,
                STANDING_CHARGE = 0.0M,
                //LEFTOVER,
                FIGURE = 0.0M;
            internal string
                DESCRIPTION = "";
            internal decimal
                VAT_RATE = 0.0M;


            #region Icomparable<TariffCosts> Members
            public int CompareTo(TariffCosts other)
            {
                // Need to test this breaking change!!!
                int compareResult = 0;

                if (other != null)
                {
                    if (PERIOD_END < other.PERIOD_END)
                    {
                        compareResult = -1;
                    }
                    else
                    {
                        if (PERIOD_END > other.PERIOD_END)
                        {
                            compareResult = 1;
                        }
                        //else
                        //{
                        //    // The Period Ends are equal so compare the Codes ...
                        //    if (Convert.ToInt16(CODE) > Convert.ToInt16(other.CODE))
                        //    {
                        //        compareResult = -1;
                        //    }
                        //    else
                        //    {
                        //        if (Convert.ToInt16(CODE) < Convert.ToInt16(other.CODE))
                        //        {
                        //            compareResult = -1;
                        //        }
                        //    }
                        //}
                    }
                }
                return compareResult;
            }

            public static int Compare(TariffCosts left, TariffCosts right)
            {
                if (object.ReferenceEquals(left, right))
                {
                    return 0;
                }
                if (left is null)
                {
                    return -1;
                }
                return left.CompareTo(right);
            }

            public override bool Equals(object obj)
            {
                TariffCosts other = (TariffCosts)obj; //avoid double casting
                if (other is null)
                {
                    return false;
                }
                return this.CompareTo(other) == 0;
            }

            public static bool operator ==(TariffCosts left, TariffCosts right)
            {
                if (left is null)
                {
                    return right is null;
                }
                return left.Equals(right);
            }
            public static bool operator !=(TariffCosts left, TariffCosts right)
            {
                return !(left == right);
            }
            public static bool operator <(TariffCosts left, TariffCosts right)
            {
                return (Compare(left, right) < 0);
            }
            public static bool operator >(TariffCosts left, TariffCosts right)
            {
                return (Compare(left, right) > 0);
            }
            public override int GetHashCode()
            {
                //Get hash code for the PERIOD_END field.
                int date = PERIOD_END.GetHashCode();

                //Calculate the hash code for the product.
                return (int)date;
            }
            #endregion
        }

        internal class Simulate
        {
            internal string
                USERNAME = "";
            internal DateTime
                DATE = SmartParametersV2016.defaultDate;
            internal decimal
                TOTAL_READINGS = 0.0M;
            internal int
                TOTAL_COSTS = 0;
            internal Simulate(string Username,
                            DateTime date,
                            decimal totalReadings,
                            int totalCosts)
            {
                USERNAME = Username;
                DATE = date;
                TOTAL_READINGS = totalReadings;
                TOTAL_COSTS = totalCosts;
            }
        }

        internal class GasConversion
        {
            internal char
                CUBEFACE_CODE = SmartParametersV2016.defaultChar;
            internal DateTime
                VALID_FROM = SmartParametersV2016.defaultDate,
                VALID_TO = SmartParametersV2016.defaultDate;
            internal decimal
                CORRECTION_FACTOR = 0.0M,
                KWH_CONVERSION = 0.0M;
#if WINFORMS
            internal DateTime
                  Created = SmartParametersV2016.defaultDate,
                  Updated = SmartParametersV2016.defaultDate;
            internal string
                   Status_Flag = SmartParametersV2016.activeStatus;
            internal DateTime
                  Deactivated = SmartParametersV2016.defaultDate;
#endif
        }

        internal class ConditionsDates
        {
            internal char
                CUBEFACE_CODE = SmartParametersV2016.defaultChar;
            internal short
                SUPPLIER_CODE = 0;
            internal char
                RESOURCE_CODE = SmartParametersV2016.defaultResourceCode;
            internal string
                RESOURCE_TYPE = "";
            internal int
                TARIFF_CODE = 0;
            internal short
                VERSION_CODE = 0,
                PAYMENT_CODE = 0;
            internal DateTime
                PRICES_VALID_FROM = SmartParametersV2016.defaultDate;
            internal short
                TIER_COUNT = 0;
#if WINFORMS
            internal string
                GROUP_NAME = "";
            internal DateTime
                  Created = SmartParametersV2016.defaultDate,
                  Updated = SmartParametersV2016.defaultDate;
            internal string
                   Status_Flag = SmartParametersV2016.activeStatus;
            internal DateTime
                  Deactivated = SmartParametersV2016.defaultDate;
#endif
        }

        internal class ConditionsGroups
        {
            internal char
                CUBEFACE_CODE = SmartParametersV2016.defaultChar;
            internal short
                SUPPLIER_CODE = 0;
            internal char
                RESOURCE_CODE = SmartParametersV2016.defaultResourceCode;
            internal string
                RESOURCE_TYPE = "";
            internal int
                TARIFF_CODE = 0;
            internal short
                VERSION_CODE = 0,
                PAYMENT_CODE = 0,
                TIER_COUNT = 0,
                TIER_LEVEL = 0,
                LIMIT_CODE = 0;
            //internal string         // How THE FUCK did it ever work with this in?????
            //    PAYMENT_PLANS = "";
#if WINFORMS
            internal DateTime
                  Created = SmartParametersV2016.defaultDate,
                  Updated = SmartParametersV2016.defaultDate;
            internal string
                   Status_Flag = SmartParametersV2016.activeStatus;
            internal DateTime
                  Deactivated = SmartParametersV2016.defaultDate;
#endif
        }

        internal class ConditionsView
        {
            internal char
                CUBEFACE_CODE = SmartParametersV2016.defaultChar;
            internal short
                SUPPLIER_CODE = 0;
            internal char
                RESOURCE_CODE = SmartParametersV2016.defaultResourceCode;
            internal string
                RESOURCE_TYPE = "";
            internal int
                TARIFF_CODE = 0;
            internal short
                VERSION_CODE = 0,
                PAYMENT_CODE = 0;
            internal DateTime
                PRICES_VALID_FROM = SmartParametersV2016.defaultDate;
            internal short
                TIER_COUNT = 0,
                TIER_LEVEL = 0;
        }

        internal class ConditionsLimits
        {
            internal char
                CUBEFACE_CODE = SmartParametersV2016.defaultChar;
            internal short
                SUPPLIER_CODE = 0;
            internal char
                RESOURCE_CODE = SmartParametersV2016.defaultResourceCode;
            internal string
                RESOURCE_TYPE = "";
            internal short
                LIMIT_CODE = 0;
            internal string
                LIMIT_TYPE = "",
                LOWER_LIMIT = "",
                LOWER_SYMBOL = "",
                FIELD_NAME = "",
                UPPER_SYMBOL = "",
                UPPER_LIMIT = "",
                UNITS = "",
                LIMIT_NAME = "";
#if WINFORMS
            internal string
                Comments = "";
            internal DateTime
                Created = SmartParametersV2016.defaultDate,
                Updated = SmartParametersV2016.defaultDate;
            internal string
                 Status_Flag = SmartParametersV2016.activeStatus;
            internal DateTime
                 Deactivated = SmartParametersV2016.defaultDate;
#endif
        }

        internal class ConditionsAreas
        {
            internal char
                CUBEFACE_CODE = SmartParametersV2016.defaultChar;
            internal short
                SUPPLIER_CODE = 0;
            internal char
                RESOURCE_CODE = SmartParametersV2016.defaultResourceCode;
            internal string
                RESOURCE_TYPE = "";
            internal short
                LIMIT_CODE = 0,
                AREA_CODE = 0;
            internal string
                LOWER_LIMIT = "",
                UPPER_LIMIT = "";
#if WINFORMS
            internal string
                LIMIT_NAME = "";
            internal DateTime
                    Created = SmartParametersV2016.defaultDate,
                    Updated = SmartParametersV2016.defaultDate;
            internal string
                   Status_Flag = SmartParametersV2016.activeStatus;
            internal DateTime
                   Deactivated = SmartParametersV2016.defaultDate;
#endif
        }

        internal class ConditionsPlans
        {
            internal char
                CUBEFACE_CODE = SmartParametersV2016.defaultChar;
            internal short
                SUPPLIER_CODE = 0;
            internal char
                RESOURCE_CODE = SmartParametersV2016.defaultResourceCode;
            internal string
                RESOURCE_TYPE = "";
            internal int
                TARIFF_CODE = 0;
            internal short
                VERSION_CODE = 0;
            internal string
                PAYMENT_PLANS = "";
            internal short
                PAYMENT_CODE = 0;
#if WINFORMS
            internal string
                SELECTION_NAME = "";
            internal DateTime
                  Created = SmartParametersV2016.defaultDate,
                  Updated = SmartParametersV2016.defaultDate;
            internal string
                   Status_Flag = SmartParametersV2016.activeStatus;
            internal DateTime
                  Deactivated = SmartParametersV2016.defaultDate;
#endif
        }

        internal class ConditionsPlansView
        {
            internal char
                CUBEFACE_CODE = SmartParametersV2016.defaultChar;
            internal short
                SUPPLIER_CODE = 0;
            internal char
                RESOURCE_CODE = SmartParametersV2016.defaultResourceCode;
            internal string
                RESOURCE_TYPE = "";
            internal int
                TARIFF_CODE = 0;
            internal short
                VERSION_CODE = 0;
            internal string
                PAYMENT_PLANS = "";  // Could be more than one ...
            internal short
                PAYMENT_CODE = 0;
        }

        internal class DistributorInfo
        {
            internal char
                CUBEFACE_CODE = SmartParametersV2016.defaultChar;
            internal string
                MARKET_ID = "",
                DNO_LOGO = "",
                DISTRIBUTION_COMPANY = "",
                LOSS_OF_SUPPLY_PHONE = "";
#if WINFORMS
            internal DateTime
                  Created = SmartParametersV2016.defaultDate,
                  Updated = SmartParametersV2016.defaultDate;
            internal string
                   Status_Flag = SmartParametersV2016.activeStatus;
            internal DateTime
                  Deactivated = SmartParametersV2016.defaultDate;
#endif
        }

        internal class PaymentMethods
        {
            internal char
                CUBEFACE_CODE = SmartParametersV2016.defaultChar;
            internal short
                PAYMENT_CODE = 0;
            internal string
                PAYMENT_METHOD1 = "",
                PAYMENT_METHOD2 = "";
#if WINFORMS
            internal DateTime
                  Created = SmartParametersV2016.defaultDate,
                  Updated = SmartParametersV2016.defaultDate;
            internal string
                   Status_Flag = SmartParametersV2016.activeStatus;
            internal DateTime
                   Deactivated = SmartParametersV2016.defaultDate;
#endif
        }

        internal class PaymentPlans
        {
            internal char
                CUBEFACE_CODE = SmartParametersV2016.defaultChar;
            internal char
                PAYMENT_PLAN = SmartParametersV2016.defaultChar;
            internal string
                PAYMENT_NAME = "",
                ALTERNATE_NAME1 = "",
                ALTERNATE_NAME2 = "";
#if WINFORMS
            internal string
                ALTERNATE_NAME3 = "",   // Only for SP Analyze
                PAYMENT_CODE = "";       // <= Only used in energylinx
            internal DateTime
                  Created = SmartParametersV2016.defaultDate,
                  Updated = SmartParametersV2016.defaultDate;
            internal string
                   Status_Flag = SmartParametersV2016.activeStatus;
            internal DateTime
                   Deactivated = SmartParametersV2016.defaultDate;
#endif
        }

        internal class Post_Conditions
        {
            internal char
                CUBEFACE_CODE = SmartParametersV2016.defaultChar;
            internal short
                SUPPLIER_CODE = 0;
            internal char
                RESOURCE_CODE = SmartParametersV2016.defaultResourceCode;
            internal short
                SELECTION_CODE = 0;
            internal string
                SELECTION_NAME = "",
                FIELD_NAME = "",
                PRICE_EXC_VAT = "",
                MAXIMUM_DISCOUNT = "",
                ONCE_ONLY = "";
#if WINFORMS
            internal DateTime
                  Created = SmartParametersV2016.defaultDate,
                  Updated = SmartParametersV2016.defaultDate;
            internal string
                   Status_Flag = SmartParametersV2016.activeStatus;
            internal DateTime
                  Deactivated = SmartParametersV2016.defaultDate;
#endif
        }

        internal class Post_Codes
        {
            internal char
                CUBEFACE_CODE = SmartParametersV2016.defaultChar;
            internal short
                SUPPLIER_CODE = 0;
            internal char
                RESOURCE_CODE = SmartParametersV2016.defaultResourceCode;
            internal int
                TARIFF_CODE = 0;
            internal short
                GROUP_CODE = 0;
            internal string
                PAYMENT_PLANS = "";
#if WINFORMS
            internal DateTime
                  Created = SmartParametersV2016.defaultDate,
                  Updated = SmartParametersV2016.defaultDate;
            internal string
                   Status_Flag = SmartParametersV2016.activeStatus;
            internal DateTime
                  Deactivated = SmartParametersV2016.defaultDate;
#endif
        }

        internal class Post_Groupings
        {
            internal char
                CUBEFACE_CODE = SmartParametersV2016.defaultChar;
            internal short
                SUPPLIER_CODE = 0,
                GROUP_CODE = 0,
                SELECTION_CODE = 0;
#if WINFORMS
            internal DateTime
                  Created = SmartParametersV2016.defaultDate,
                  Updated = SmartParametersV2016.defaultDate;
            internal string
                   Status_Flag = SmartParametersV2016.activeStatus;
            internal DateTime
                  Deactivated = SmartParametersV2016.defaultDate;
#endif
        }

        internal class Post_Groups
        {
            internal char
                CUBEFACE_CODE = SmartParametersV2016.defaultChar;
            internal short
                SUPPLIER_CODE = 0;
            internal char
                RESOURCE_CODE = SmartParametersV2016.defaultResourceCode;
            internal short
                GROUP_CODE = 0;
#if WINFORMS
            internal string
               GROUP_NAME = "";
            internal DateTime
                  Created = SmartParametersV2016.defaultDate,
                  Updated = SmartParametersV2016.defaultDate;
            internal string
                   Status_Flag = SmartParametersV2016.activeStatus;
            internal DateTime
                  Deactivated = SmartParametersV2016.defaultDate;
#endif
        }

        internal class Post_Limits
        {
            internal char
                CUBEFACE_CODE = SmartParametersV2016.defaultChar;
            // How THE FUCK did this ever work with the Limit Name in the wrong position???
            internal short
                SUPPLIER_CODE = 0,
                LIMIT_CODE = 0;
            internal string
                LIMIT_TYPE = "",
                LOWER_LIMIT = "",
                LOWER_SYMBOL = "",
                FIELD_NAME = "",
                UPPER_SYMBOL = "",
                UPPER_LIMIT = "",
                UNITS = "";
#if WINFORMS
            internal string
                LIMIT_NAME = "";
            internal DateTime
                  Created = SmartParametersV2016.defaultDate,
                  Updated = SmartParametersV2016.defaultDate;
            internal string
                   Status_Flag = SmartParametersV2016.activeStatus;
            internal DateTime
                   Deactivated = SmartParametersV2016.defaultDate;
#endif
        }

        internal class Post_Select
        {
            internal char
                CUBEFACE_CODE = SmartParametersV2016.defaultChar;
            internal short
                SUPPLIER_CODE = 0,
                SELECTION_CODE = 0,
                LIMIT_CODE = 0;
#if WINFORMS
            internal DateTime
                  Created = SmartParametersV2016.defaultDate,
                  Updated = SmartParametersV2016.defaultDate;
            internal string
                   Status_Flag = SmartParametersV2016.activeStatus;
            internal DateTime
                  Deactivated = SmartParametersV2016.defaultDate;
#endif
        }

        internal class PreConditions
        {
            internal char
                CUBEFACE_CODE = SmartParametersV2016.defaultChar;
            internal short
                SUPPLIER_CODE = 0;
            internal char
                RESOURCE_CODE = SmartParametersV2016.defaultResourceCode;
            internal int
                TARIFF_CODE = 0;
            internal string
                FIELD_NAME = "",
                PRE_CONDITION = "";
#if WINFORMS
            internal string
                Comments = "";
            internal DateTime
                Created = SmartParametersV2016.defaultDate,
                Updated = SmartParametersV2016.defaultDate;
            internal string
                Status_Flag = SmartParametersV2016.activeStatus;
            internal DateTime
                Deactivated = SmartParametersV2016.defaultDate;
#endif
        }

        internal class BrandMatrix
        {
            internal char
                CUBEFACE_CODE = SmartParametersV2016.defaultChar;
            internal short
                BRAND_CODE = 0;
            internal char
                AREA_10 = SmartParametersV2016.defaultChar,
                AREA_11 = SmartParametersV2016.defaultChar,
                AREA_12 = SmartParametersV2016.defaultChar,
                AREA_13 = SmartParametersV2016.defaultChar,
                AREA_14 = SmartParametersV2016.defaultChar,
                AREA_15 = SmartParametersV2016.defaultChar,
                AREA_16 = SmartParametersV2016.defaultChar,
                AREA_17 = SmartParametersV2016.defaultChar,
                AREA_18 = SmartParametersV2016.defaultChar,
                AREA_19 = SmartParametersV2016.defaultChar,
                AREA_20 = SmartParametersV2016.defaultChar,
                AREA_21 = SmartParametersV2016.defaultChar,
                AREA_22 = SmartParametersV2016.defaultChar,
                AREA_23 = SmartParametersV2016.defaultChar;
#if WINFORMS
            internal DateTime
                Created = SmartParametersV2016.defaultDate,
                Updated = SmartParametersV2016.defaultDate;
            internal string
                   Status_Flag = SmartParametersV2016.activeStatus;
            internal DateTime
                  Deactivated = SmartParametersV2016.defaultDate;
#endif
        }

        internal class Suppliers
        {
            internal char
                CUBEFACE_CODE = SmartParametersV2016.defaultChar;
            internal short
                SUPPLIER_CODE = 0;
            internal char
                ACTIVE_FLAG = SmartParametersV2016.activeDefault;
            internal string
                BILL_TOKEN = "",
                BILL_CURRENCY_SYMBOL = "";  // Not a char, cos sometimes the symbol is more than just ONE!
            internal char
                BILL_DENOMINATION_SYMBOL = SmartParametersV2016.defaultChar,
                BILL_CURRENCY_SEPARATOR = SmartParametersV2016.defaultChar,
                BILL_THOUSANDS_SEPARATOR = SmartParametersV2016.defaultChar,
                BILL_PREFIX = SmartParametersV2016.defaultChar;
            internal string
                URL_PREFIX = "",
                URL_TARGET = "",
                URL_SWITCH = "",
                USE_PROXY = "";
            internal DateTime
                VALID_FROM = SmartParametersV2016.defaultDate,
                VALID_TO = SmartParametersV2016.defaultDate;
            internal string
                CONTACT_EMAIL = "",
                CONTACT_TEL_NO = "",
                SMELL_GAS_TEL_NO = "",
                POWER_CUT_TEL_NO = "";
#if WINFORMS
            internal string COMMENT = "";
            internal DateTime
                  Created = SmartParametersV2016.defaultDate,
                  Updated = SmartParametersV2016.defaultDate;
            internal string
                   Status_Flag = SmartParametersV2016.activeStatus;
            internal DateTime
                   Deactivated = SmartParametersV2016.defaultDate;
#endif
        }

        internal class Brands
        {
            internal char
                CUBEFACE_CODE = SmartParametersV2016.defaultChar;
            internal short
                SUPPLIER_CODE = 0,
                BRAND_CODE = 0;
            internal string
                BRAND_NAME = "";
            internal string
                DESCRIPTION = "";
#if WINFORMS
            internal string
                ALTERNATIVE_NAME = "",
                DASHBOARD_NAME = "",
                USWITCH_NAME = "",
                USWITCH_SCRAPER = "",
                HREF = "";
            internal char
                PROPOGATE_TARIFFS = SmartParametersV2016.defaultChar;
            internal DateTime
                  Created = SmartParametersV2016.defaultDate,
                  Updated = SmartParametersV2016.defaultDate;
            internal string
                   Status_Flag = SmartParametersV2016.activeStatus;
            internal DateTime
                   Deactivated = SmartParametersV2016.defaultDate;
#endif
        }

        internal class BrandConnection
        {
            internal char
               CUBEFACE_CODE = SmartParametersV2016.defaultChar;  // Usually 'U' for Utility
            internal short
                SUPPLIER_CODE = 0,
                BRAND_CODE = 0;
            internal short
                ORDINAL = 0;
            internal string
                PARAMETER_PROMPT = "";
            internal int
                PARAMETER_LENGTH = 0;
            internal string
                PARAMETER_TYPE = "";
#if WINFORMS

            internal DateTime
                  Created = SmartParametersV2016.defaultDate,
                  Updated = SmartParametersV2016.defaultDate;
            internal string
                   Status_Flag = SmartParametersV2016.activeStatus;
            internal DateTime
                   Deactivated = SmartParametersV2016.defaultDate;
#endif
        }

        internal class SupplierTypes
        {
            internal char
                CUBEFACE_CODE = SmartParametersV2016.defaultChar;
            internal char
                RESOURCE_CODE = SmartParametersV2016.defaultResourceCode;
            internal string
                RESOURCE_TYPE = "";
            internal short
                SUPPLIER_CODE = 0;
#if WINFORMS
            internal DateTime
                  Created = SmartParametersV2016.defaultDate,
                  Updated = SmartParametersV2016.defaultDate;
            internal string
                   Status_Flag = SmartParametersV2016.activeStatus;
            internal DateTime
                   Deactivated = SmartParametersV2016.defaultDate;
#endif
        }

        internal class Templates
        {
            internal char
                CUBEFACE_CODE = SmartParametersV2016.defaultChar;
            internal string
                WHAT = "",
                TEXT = "";
#if WINFORMS
            internal DateTime
                          Created = SmartParametersV2016.defaultDate,
                          Updated = SmartParametersV2016.defaultDate;
            internal string
                   Status_Flag = SmartParametersV2016.activeStatus;
            internal DateTime
                   Deactivated = SmartParametersV2016.defaultDate;
#endif
        }

        internal class SupplierInfo
        {
            internal char
                CUBEFACE_CODE = SmartParametersV2016.defaultChar;
            internal string
                SUPPLIER_ID = "",
                SUPPLIER_NAME = "",
                DETAILS = "";
#if WINFORMS
            internal string
                OWNER = "",
                OWNER_COUNTRY = "";
            internal DateTime
                          Created = SmartParametersV2016.defaultDate,
                          Updated = SmartParametersV2016.defaultDate;
            internal string
                   Status_Flag = SmartParametersV2016.activeStatus;
            internal DateTime
                   Deactivated = SmartParametersV2016.defaultDate;
#endif
        }

        internal class ResourceCodes
        {
            internal char
                CUBEFACE_CODE = SmartParametersV2016.defaultChar;
            internal char
                RESOURCE_CODE = SmartParametersV2016.defaultResourceCode;
            internal short
                ORDINAL = 0;
            internal string
                DESCRIPTION = "";
#if WINFORMS
            internal DateTime
                    Created = SmartParametersV2016.defaultDate,
                    Updated = SmartParametersV2016.defaultDate;
            internal string
                    Status_Flag = SmartParametersV2016.activeStatus;
            internal DateTime
                    Deactivated = SmartParametersV2016.defaultDate;
#endif
        }

        internal class ResourceTypes // As distinct from ResourceSTypes
        {
            internal char
                CUBEFACE_CODE = SmartParametersV2016.defaultChar;
            internal char
                RESOURCE_CODE = SmartParametersV2016.defaultResourceCode;
            internal string
                RESOURCE_TYPE = "";
            internal string
                DESCRIPTION = "";
#if WINFORMS
            internal DateTime
                    Created = SmartParametersV2016.defaultDate,
                    Updated = SmartParametersV2016.defaultDate;
            internal string
                    Status_Flag = SmartParametersV2016.activeStatus;
            internal DateTime
                    Deactivated = SmartParametersV2016.defaultDate;
#endif
        }
        internal class SupplyAreas
        {
            internal char
                CUBEFACE_CODE = SmartParametersV2016.defaultChar;
            internal short
                AREA_CODE = 0;
            internal string
                AREA_NAME = "",
                DNO = "",
                MARKET_ID = "";
#if WINFORMS
            internal string
                
                SUPPLIER = "",
                POSTCODE = "",
                E_ADDRESS = "",
                E7_ADDRESS = "",
                G_ADDRESS = "",
                USWITCH_REGION = "",
                USWITCH_DNO = "",
                Comment = "";
            internal DateTime
                Created = SmartParametersV2016.defaultDate,
                Updated = SmartParametersV2016.defaultDate;
            internal string
                    Status_Flag = SmartParametersV2016.activeStatus;
            internal DateTime
                    Deactivated = SmartParametersV2016.defaultDate;
#endif
        }

        internal class TariffHistory
        {
            internal char
                CUBEFACE_CODE = SmartParametersV2016.defaultChar;
            internal short
                BRAND_CODE = 0;
            internal char
                RESOURCE_CODE = SmartParametersV2016.defaultResourceCode;
            internal string
                RESOURCE_TYPE = "";
            internal int
                TARIFF_CODE = 0;
            internal short
                VERSION_CODE = 0;
            internal string
                TARIFF_PREVIOUS_NAME = "";
#if WINFORMS
            internal DateTime
                LAUNCHED = SmartParametersV2016.defaultDate,
                ISSUED = SmartParametersV2016.defaultDate,
                PRICES_VALID_FROM = SmartParametersV2016.defaultDate,
                VALID_TO = SmartParametersV2016.defaultDate,
                WITHDRAWN = SmartParametersV2016.defaultDate;
            internal DateTime
                            Created = SmartParametersV2016.defaultDate,
                            Updated = SmartParametersV2016.defaultDate;
            internal string
                    Status_Flag = SmartParametersV2016.activeStatus;
            internal DateTime
                    Deactivated = SmartParametersV2016.defaultDate;
#endif
        }

        internal class TariffMatrix
        {
            internal char
                CUBEFACE_CODE = SmartParametersV2016.defaultChar;
            internal short
                BRAND_CODE = 0;
            internal char
                RESOURCE_CODE = SmartParametersV2016.defaultResourceCode;
            internal string
                RESOURCE_TYPE = "";
            internal int
                TARIFF_CODE = 0;
            internal short
                VERSION_CODE = 0;
            internal string
                PLACEHOLDER = "";     // This was a key and now its not
            internal string
                TARIFF_NAME = "";
            internal char
                AREA_10 = SmartParametersV2016.defaultChar,
                AREA_11 = SmartParametersV2016.defaultChar,
                AREA_12 = SmartParametersV2016.defaultChar,
                AREA_13 = SmartParametersV2016.defaultChar,
                AREA_14 = SmartParametersV2016.defaultChar,
                AREA_15 = SmartParametersV2016.defaultChar,
                AREA_16 = SmartParametersV2016.defaultChar,
                AREA_17 = SmartParametersV2016.defaultChar,
                AREA_18 = SmartParametersV2016.defaultChar,
                AREA_19 = SmartParametersV2016.defaultChar,
                AREA_20 = SmartParametersV2016.defaultChar,
                AREA_21 = SmartParametersV2016.defaultChar,
                AREA_22 = SmartParametersV2016.defaultChar,
                AREA_23 = SmartParametersV2016.defaultChar;
#if WINFORMS
            internal DateTime
                            Created = SmartParametersV2016.defaultDate,
                            Updated = SmartParametersV2016.defaultDate;
            internal string
                    Status_Flag = SmartParametersV2016.activeStatus;
            internal DateTime
                    Deactivated = SmartParametersV2016.defaultDate;
#endif

        }

        internal class TariffPlans
        {
            internal char
                CUBEFACE_CODE = SmartParametersV2016.defaultChar;
            internal short
                SUPPLIER_CODE = 0;
            internal string
                PAYMENT_PLANS = "";
#if WINFORMS
            internal DateTime
                            Created = SmartParametersV2016.defaultDate,
                            Updated = SmartParametersV2016.defaultDate;
            internal string
                    Status_Flag = SmartParametersV2016.activeStatus;
            internal DateTime
                    Deactivated = SmartParametersV2016.defaultDate;
#endif
        }

        internal class Tariffs
        {
            internal char
                CUBEFACE_CODE = SmartParametersV2016.defaultChar;
            internal short
                SUPPLIER_CODE = 0;  // This isn't BRAND_CODE for the following reason:
                                    // Tariffs are determined by Supplier so if these
                                    // Suppliers DO have separate 'brands' and separate
                                    // tariffs for each 'brand' then add them to the
                                    // Supplier and determine which are seen by which
                                    // 'brand' using the Tariff Area Matrix because
                                    // a'brand' will cover a particular Area. No 'brand'
                                    // must (can) ever be multi-area ....
            internal char
                RESOURCE_CODE = SmartParametersV2016.defaultResourceCode;
            internal string
                RESOURCE_TYPE = "";
            internal int
                TARIFF_CODE = 0;
            internal string
                FALLBACK = "",
                TARIFF_TYPE = "",
                AVAILABILITY = "",
                AGE = "",
                CUSTOMER = "";
            internal DateTime
                LAUNCHED = SmartParametersV2016.defaultDate,
                ISSUED = SmartParametersV2016.defaultDate,
                PRICES_VALID_FROM = SmartParametersV2016.defaultDate,
                VALID_TO = SmartParametersV2016.defaultDate;
#if WINFORMS
            internal DateTime
                WITHDRAWN = SmartParametersV2016.defaultDate;
            internal string
                ONLINE_OPTION = "",
                DUAL_FUEL = "",
                FEED_IN = "";
            internal byte
                PAYDAYS = 0;
            internal short
                TIL_TEO = 0,
                TIL_PGU = 0,
                TIL_EF = 0,
                TIL_DAC = 0,
                TIL_APSI = 0;
            internal DateTime
                    Created = SmartParametersV2016.defaultDate,
                    Updated = SmartParametersV2016.defaultDate;
            internal string
                    Status_Flag = SmartParametersV2016.activeStatus;
            internal DateTime
                    Deactivated = SmartParametersV2016.defaultDate;
#endif
        }

        internal class TIL_APSI
        {
            internal char
                CUBEFACE_CODE = SmartParametersV2016.defaultChar;
            internal short
                APSI_TIL = 0;
            internal string
                APSI_INFO = "";
#if WINFORMS
            internal DateTime
                    Created = SmartParametersV2016.defaultDate,
                    Updated = SmartParametersV2016.defaultDate;
            internal string
                    Status_Flag = SmartParametersV2016.activeStatus;
            internal DateTime
                    Deactivated = SmartParametersV2016.defaultDate;
#endif
        }

        internal class TIL_DAC
        {
            internal char
                CUBEFACE_CODE = SmartParametersV2016.defaultChar;
            internal short
                DAC_TIL = 0;
            internal string
                DAC_INFO = "";
#if WINFORMS
            internal DateTime
                    Created = SmartParametersV2016.defaultDate,
                    Updated = SmartParametersV2016.defaultDate;
            internal string
                    Status_Flag = SmartParametersV2016.activeStatus;
            internal DateTime
                    Deactivated = SmartParametersV2016.defaultDate;
#endif
        }

        internal class TIL_EF
        {
            internal char
                CUBEFACE_CODE = SmartParametersV2016.defaultChar;
            internal short
                EF_TIL = 0;
            internal string
                EF_INFO = "";
#if WINFORMS
            internal DateTime
                    Created = SmartParametersV2016.defaultDate,
                    Updated = SmartParametersV2016.defaultDate;
            internal string
                    Status_Flag = SmartParametersV2016.activeStatus;
            internal DateTime
                    Deactivated = SmartParametersV2016.defaultDate;
#endif
        }

        internal class TIL_PGU
        {
            internal char
                CUBEFACE_CODE = SmartParametersV2016.defaultChar;
            internal short
                PGU_TIL = 0;
            internal string
                PGU_INFO = "";
#if WINFORMS

            internal DateTime
                    Created = SmartParametersV2016.defaultDate,
                    Updated = SmartParametersV2016.defaultDate;
            internal string
                    Status_Flag = SmartParametersV2016.activeStatus;
            internal DateTime
                    Deactivated = SmartParametersV2016.defaultDate;
#endif
        }

        internal class TIL_TEO
        {
            internal char
                CUBEFACE_CODE = SmartParametersV2016.defaultChar;
            internal short
                TEO_TIL = 0;
            internal string
                TEO_INFO = "";
#if WINFORMS

            internal DateTime
                    Created = SmartParametersV2016.defaultDate,
                    Updated = SmartParametersV2016.defaultDate;
            internal string
                    Status_Flag = SmartParametersV2016.activeStatus;
            internal DateTime
                    Deactivated = SmartParametersV2016.defaultDate;
#endif
        }

        internal class WithdrawnDate
        {
            internal DateTime
                WITHDRAWN_DATE = SmartParametersV2016.defaultDate;
#if WINFORMS
            internal DateTime
                Created = SmartParametersV2016.defaultDate,
                Updated = SmartParametersV2016.defaultDate;
            internal string
                Status_Flag = SmartParametersV2016.activeStatus;
#endif
        }

        internal class UnitRates
        {
            // Really ... when we execute LOAD_UNIT_RATES we don't need to return
            // RESOURCE_CODE, RESOURCE_TYPE and AREA_CODE because these are selection keys
            // but we would only be saving 1 + 2 + 2 bytes ...5 bytes?? Wtf its not worth it..
            internal char
                CUBEFACE_CODE = SmartParametersV2016.defaultChar;
            internal short
                SUPPLIER_CODE = 0;
            internal char
                RESOURCE_CODE = SmartParametersV2016.defaultResourceCode;
            internal string
                RESOURCE_TYPE = "";
            internal int
                TARIFF_CODE = 0;
            internal short
                PAYMENT_CODE = 0;
            internal DateTime
                PRICES_VALID_FROM = SmartParametersV2016.defaultDate;
            internal short
                TIER_COUNT = 0,
                TIER_LEVEL = 0,
                AREA_CODE = 0;
            internal string
                SC = "",
                DR = "",
                NR = "",
                TCR = "";            // <= New one
#if WINFORMS
            internal DateTime
                Updated = SmartParametersV2016.defaultDate,
                Created = SmartParametersV2016.defaultDate;
            internal string
                Status_Flag = SmartParametersV2016.activeStatus;
#endif
            // <= Because of Ba! flag we don't attempt
            // to remove DEACTIVATED from this table as it doesn't exist
        }
    }
}