//using System.Windows.Media;     // This is in PresentationCore.dll <= FUCKING OBVIOUSLY!! Microshit fucking wanking idiots
//using System.Windows.Media.Imaging; // To get the fucking DNO lit up ...

//using System.Net.Mail;

#if DBSERVER
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Text;
#endif

#if WINFORMS
using System.Windows.Forms;
using System.Windows.Markup;
using System.Windows;
using System.IO;
using OxyPlot;
using System.Globalization;
using System.ComponentModel;
using System.Text;
using Microsoft.Web.WebView2.WinForms;
#endif

#if WPF
using System.Windows.Controls;
using System.Windows;
//using System.Windows.Media;
using System.Windows.Markup;
using System.Windows.Media.Imaging;
using System.Windows.Data;
using OxyPlot.Wpf;
using System.Windows.Threading;
using System.Windows.Media;
using Microsoft.Web.WebView2.Wpf;
using Microsoft.Web.WebView2.Core;
using System.IO;
using OxyPlot;
using System.ComponentModel;
using System.Globalization;
using System.Text;
#endif

#if WINUI
using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls.Primitives;
using System.Threading.Tasks;
using Microsoft.Web.WebView2.Core;
using System.IO;
using OxyPlot; // For the event
using Microsoft.UI.Xaml.Data;
using CommunityToolkit.WinUI.UI.Controls;
using Microsoft.UI.Xaml.Controls;
using System.Globalization;
using System.ComponentModel;
using System.Text;
using Microsoft.UI.Xaml.Media.Animation;
#endif

#if ANDROIDX
using OxyPlot.Xamarin.Android;
using AndroidX.ViewPager2.Widget;
using Android.Graphics;
using OxyPlot;
using Android.Views;
using System.Text;
using System.ComponentModel;
using System.Globalization;
#endif

#if SMARTMAUI
//using CommunityToolkit.Maui.Views;
using System.Text;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using Microsoft.Maui.Controls;
using System.Globalization;
using System.ComponentModel;
using CommunityToolkit.Maui.Views;
using System.Windows.Input;
#endif

namespace SmartCubeMobile
{
#if WPF
    public class FinanceDateConverter : IValueConverter
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
    }
#endif

#if WINUI

        public class FinanceDateConverter : IValueConverter
        {
            public object Convert(object value, Type targetType, object parameter, string language)
            {
                try
                {
                    var culture = CultureInfo.CurrentCulture;

                    var d = value as DateTime?;
                    if (!d.HasValue)
                        return value;

                    return d.Value.ToString("d", culture);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }

                return DateTime.Now.ToString("d", CultureInfo.CurrentCulture);
            }

            public object ConvertBack(object value, Type targetType, object parameter, string language)
            {
                try
                {
                    var culture = CultureInfo.CurrentCulture;

                    if (value is DateTime dt)
                        return dt;

                    if (value is DateTimeOffset dto)
                        return dto.DateTime;

                    if (value is string str &&
                        DateTime.TryParse(str, culture, DateTimeStyles.None, out var result))
                    {
                        return result;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                return Microsoft.UI.Xaml.DependencyProperty.UnsetValue;
            }
        }
#endif
#if SMARTMAUI

    public class FinanceDateConverter : IValueConverter
    {
        public object Convert(object value,
                              Type targetType,
                              object parameter,
                              CultureInfo culture)
        {
            if (value is DateTime date)
            {
                return date.ToString("d",
                       culture ?? CultureInfo.CurrentCulture);
            }

            return value;
        }

        public object ConvertBack(object value,
                                  Type targetType,
                                  object parameter,
                                  CultureInfo culture)
        {
            // MAUI DatePicker sends DateTime directly
            if (value is DateTime dt)
            {
                return dt;
            }

            // Text entry fallback
            if (value is string str &&
                DateTime.TryParse(
                    str,
                    culture ?? CultureInfo.CurrentCulture,
                    DateTimeStyles.None,
                    out var result))
            {
                return result;
            }

            return Binding.DoNothing;
        }
    }
#endif

#if WINFORMS || WPF  || WINUI || SMARTMAUI
    public class FinanceViewModel : INotifyPropertyChanged
    {
#endif
#if ANDROIDX
    public class FinanceViewModel : AndroidX.Lifecycle.ViewModel
    {
#endif
            
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string propName)
        {
            if (this.PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propName));
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
                    this.OnPropertyChanged(nameof(errorMessage));
                }
            }
        }





#if WINUI
        public bool IsLoginMethodLocked { get; set; }
        public bool ShowProviderText => !IsNew;
        public bool ShowProviderCombo => IsNew;

        public bool ShowLoginMethodText => !IsNew && IsLoginMethodLocked;
        public bool ShowLoginMethodCombo => !IsNew && !IsLoginMethodLocked;

        private bool _isNew;
        public bool IsNew
        {
            get => _isNew;
            set
            {
                _isNew = value;
                
                // 🔥 IMPORTANT: update dependent properties
                OnPropertyChanged(nameof(ShowProviderText));
                OnPropertyChanged(nameof(ShowProviderCombo));
                OnPropertyChanged(nameof(ShowLoginMethodText));
                OnPropertyChanged(nameof(ShowLoginMethodCombo));
            }
        }

#endif
#if SMARTMAUI
        private List<CurrencyTab> currencies = new List<CurrencyTab>();
        public List<CurrencyTab> Currencies
        {
            get
            {
                return currencies;
            }
            set
            {
                currencies = value;
                this.OnPropertyChanged(nameof(Currencies));
            }
        }
#endif

        public List<CurrencyValues> currencytotals = new List<CurrencyValues>();

        private string paidIn = "";
        public string PaidIn
        {
            get
            {
                return paidIn;
            }
            set
            {
                paidIn = value;
                this.OnPropertyChanged(nameof(PaidIn));
            }
        }

        private string paidOut = "";
        public string PaidOut
        {
            get
            {
                return paidOut;
            }
            set
            {
                paidOut = value;
                this.OnPropertyChanged(nameof(PaidOut));
            }
        }


        private string difference = "";
        public string Difference
        {
            get
            {
                return difference;
            }
            set
            {
                difference = value;
                this.OnPropertyChanged(nameof(Difference));
            }
        }

        internal string from = "";
        internal string to = "";

        internal bool suppressClose = false;

        private List<SmartFinance.CryptoLedgers> finance_cryptoledgerDefault = new List<SmartFinance.CryptoLedgers>();
        public List<SmartFinance.CryptoLedgers> FinanceCryptoLedger
        {
            get
            {
                return finance_cryptoledgerDefault;
            }
            set
            {
                finance_cryptoledgerDefault = value;
                this.OnPropertyChanged(nameof(FinanceCryptoLedger));
            }
        }

        private List<SmartFinance.CryptoWallets> finance_wallets = new List<SmartFinance.CryptoWallets>();
        public List<SmartFinance.CryptoWallets> FinanceWallets
        {
            get
            {
                return finance_wallets;
            }
            set
            {
                finance_wallets = value;
                this.OnPropertyChanged(nameof(FinanceWallets));
            }
        }

        internal int totalsIndex1 = 0;
        internal int totalsIndex2 = 0;
        internal bool displayFees = false;

        private int finance_feewidthDefault = 0;
        public int FeeWidth
        {
            get
            {
                return finance_feewidthDefault;
            }
            set
            {
                finance_feewidthDefault = value;
                this.OnPropertyChanged(nameof(FeeWidth));
            }
        }

#if CRYPTOS
        private List<SmartCrypto.LedgerTransactionView> finance_xrpcryptoledgerDefault = new List<SmartCrypto.LedgerTransactionView>();
        public List<SmartCrypto.LedgerTransactionView> FinanceXRPCryptoLedger
        {
            get
            {
                return finance_xrpcryptoledgerDefault;
            }
            set
            {
                finance_xrpcryptoledgerDefault = value;
                this.OnPropertyChanged(nameof(FinanceXRPCryptoLedger));
            }
        }

        private List<SmartCrypto.LedgerTransactionView> finance_ethcryptoledgerDefault = new List<SmartCrypto.LedgerTransactionView>();
        public List<SmartCrypto.LedgerTransactionView> FinanceETHCryptoLedger
        {
            get
            {
                return finance_ethcryptoledgerDefault;
            }
            set
            {
                finance_ethcryptoledgerDefault = value;
                this.OnPropertyChanged(nameof(FinanceETHCryptoLedger));
            }
        }
        

        private List<SmartCrypto.CurrencyTotals> finance_currtotalsDefault = new List<SmartCrypto.CurrencyTotals>();
        public List<SmartCrypto.CurrencyTotals> FinanceCurrencyTotals
        {
            get
            {
                return finance_currtotalsDefault;
            }
            set
            {
                finance_currtotalsDefault = value;
                this.OnPropertyChanged(nameof(FinanceCurrencyTotals));
            }
        }

        private List<SmartCrypto.CurrencyTotals> finance_currtotalsDefault1 = new List<SmartCrypto.CurrencyTotals>();
        public List<SmartCrypto.CurrencyTotals> FinanceCurrencyTotals1
        {
            get
            {
                return finance_currtotalsDefault1;
            }
            set
            {
                finance_currtotalsDefault1 = value;
                this.OnPropertyChanged(nameof(FinanceCurrencyTotals1));
            }
        }

        private List<SmartCrypto.CurrencyTotals> finance_currtotalsDefault2 = new List<SmartCrypto.CurrencyTotals>();
        public List<SmartCrypto.CurrencyTotals> FinanceCurrencyTotals2
        {
            get
            {
                return finance_currtotalsDefault2;
            }
            set
            {
                finance_currtotalsDefault2 = value;
                this.OnPropertyChanged(nameof(FinanceCurrencyTotals2));
            }
        }

        private List<SmartCrypto.V2CryptoTotals> finance_cryptocurrencyratesDefault = new List<SmartCrypto.V2CryptoTotals>();
        public List<SmartCrypto.V2CryptoTotals> FinanceCryptoCurrencyRates

        {
            get
            {
                return finance_cryptocurrencyratesDefault;
            }
            set
            {
                finance_cryptocurrencyratesDefault = value;
                this.OnPropertyChanged(nameof(FinanceCryptoCurrencyRates));
            }
        }


        private List<SmartFinance.CryptoWalletTotals> finance_wallettotals = new List<SmartFinance.CryptoWalletTotals>();
        public List<SmartFinance.CryptoWalletTotals> FinanceWalletTotals
        {
            get
            {
                return finance_wallettotals;
            }
            set
            {
                finance_wallettotals = value;
                this.OnPropertyChanged(nameof(FinanceWalletTotals));
            }
        }

        public int WalletTotalsIndex = -1;

        private List<SmartFinance.CryptoAccountsView> finance_cryptoaccountsDefault = new List<SmartFinance.CryptoAccountsView>();
        public List<SmartFinance.CryptoAccountsView> FinanceCryptoAccounts
        {
            get
            {
                return finance_cryptoaccountsDefault;
            }
            set
            {
                finance_cryptoaccountsDefault = value;
                this.OnPropertyChanged(nameof(FinanceCryptoAccounts));
            }
        }

        private List<SmartFinance.CryptoAddressesView> finance_cryptoaddressesDefault = new List<SmartFinance.CryptoAddressesView>();
        public List<SmartFinance.CryptoAddressesView> FinanceCryptoAddresses
        {
            get
            {
                return finance_cryptoaddressesDefault;
            }
            set
            {
                finance_cryptoaddressesDefault = value;
                this.OnPropertyChanged(nameof(FinanceCryptoAddresses));
            }
        }

        private List<SmartFinance.CryptoExchangeRatesView> finance_cryptoexchangeratesDefault = new List<SmartFinance.CryptoExchangeRatesView>();
        public List<SmartFinance.CryptoExchangeRatesView> FinanceCryptoExchangeRates
        {
            get
            {
                return finance_cryptoexchangeratesDefault;
            }
            set
            {
                finance_cryptoexchangeratesDefault = value;
                this.OnPropertyChanged(nameof(FinanceCryptoExchangeRates));
            }
        }

        private List<SmartFinance.CryptoTransactionsView> finance_cryptoethtransactionsDefault = new List<SmartFinance.CryptoTransactionsView>();
        public List<SmartFinance.CryptoTransactionsView> FinanceCryptoETHTransactions
        {
            get
            {
                return finance_cryptoethtransactionsDefault;
            }
            set
            {
                finance_cryptoethtransactionsDefault = value;
                this.OnPropertyChanged(nameof(FinanceCryptoETHTransactions));
            }
        }

        private List<SmartFinance.CryptoTransactionsView> finance_cryptotransactionsspecificDefault = new List<SmartFinance.CryptoTransactionsView>();
        public List<SmartFinance.CryptoTransactionsView> FinanceCryptoTransactionsSpecific
        {
            get
            {
                return finance_cryptotransactionsspecificDefault;
            }
            set
            {
                finance_cryptotransactionsspecificDefault = value;
                this.OnPropertyChanged(nameof(FinanceCryptoTransactionsSpecific));
            }
        }
#endif


#if WPF  || WINUI
        internal DataGrid Datagrid { get; set; }
#endif
#if SMARTMAUI
        internal CollectionView Datagrid { get; set; }
#endif

        internal string Progress = "";  // For CryptoDan
        public SmartFinanceList PLO = new SmartFinanceList();      // Initialized in DoWork - leave as null for SmartDashboard

#if CRYPTOS
        internal SmartCrypto.V2User cryptouserV2 { get; set; }
        internal List<SmartCrypto.Wallet> Wallets = new List<SmartCrypto.Wallet>();

        internal List<SmartCrypto.V2User> v2userList = new List<SmartCrypto.V2User>();

        internal List<SmartCrypto.V2Accounts> v2accountsList =
                new List<SmartCrypto.V2Accounts>();
        internal List<SmartCrypto.V3Accounts> v3accountsList =
            new List<SmartCrypto.V3Accounts>();
        internal List<SmartCrypto.V3Accounts> v3tempaccountsList =
            new List<SmartCrypto.V3Accounts>();

        internal List<SmartCrypto.V2Addresses> v2addressesList =
            new List<SmartCrypto.V2Addresses>();

        internal List<SmartCrypto.V2Transactions> v2transactionsList =
            new List<SmartCrypto.V2Transactions>();
        // Doesn't appear to be needed
        //internal List<SmartCrypto.V2Currencies> v2currenciesList = new List<SmartCrypto.V2Currencies>();
        internal List<SmartCrypto.V2ExchangeRates> v2exchangeratesList = new List<SmartCrypto.V2ExchangeRates>();
#endif

        internal List<SmartFinance.Accounts> v2accounts_record = new List<SmartFinance.Accounts>();

#if !DBSERVER      // All the way to the end
        // Transactions
        internal bool transfiat = true;
        internal bool buy = true;
        internal bool sell = true;
        internal bool trade = true;
        internal bool send = true;
        internal bool interest = true;
        // Accounts
        internal bool acctsfiat = true;
        internal bool crypto = true;

        internal bool createdat = true;
        internal bool types = true;
        internal bool amount = true;

#if CRYPTOS

        internal string Exchange = "";
        internal string AccessId = "";
                                                                
#endif
        internal Random random = new Random();

        internal string cbversion = "2022-01-06";
#endif
        internal DateTime localTime = SmartParametersV2016.defaultDate;
        internal TimeSpan utcoffset = SmartParametersV2016.utcdefaultOffset;
        //internal bool rateEnabled = false;
        internal int rateCounter = 0;
        internal int rateLimit = 10;
        internal bool network = false;
        internal short DefaultInstitutionCode = 0;
        private string localtime = "";

        private List<SmartFinance.CryptoTransactionsView> finance_cryptotransactionsDefault = new List<SmartFinance.CryptoTransactionsView>();
        public List<SmartFinance.CryptoTransactionsView> FinanceCryptoTransactions
        {
            get
            {
                return finance_cryptotransactionsDefault;
            }
            set
            {
                finance_cryptotransactionsDefault = value;
                this.OnPropertyChanged(nameof(FinanceCryptoTransactions));
            }
        }
        public string LocalTime
        {
            get
            {
                return localtime;
            }
            set
            {
                if (localtime != value)
                {
                    localtime = value;
                    this.OnPropertyChanged(nameof(LocalTime));
                }
            }
        }


        private string username = "";
        public string DisplayName
        {
            get
            {
                return username;
            }
            set
            {
                if (username != value)
                {
                    username = value;
                    this.OnPropertyChanged(nameof(DisplayName));
                }
            }
        }
        private string utctime = "";
        public string UTCTime
        {
            get
            {
                return utctime;
            }
            set
            {
                if (utctime != value)
                {
                    utctime = value;
                    this.OnPropertyChanged(nameof(UTCTime));
                }
            }
        }

        private bool _showUsername;
        public bool ShowUsername
        {
            get => _showUsername;
            set
            {
                _showUsername = value;
                this.OnPropertyChanged(nameof(ShowUsername));
            }
        }

#if WINFORMS
        private System.Drawing.Color networkledDefault = System.Drawing.Color.Orange;
        public System.Drawing.Color NetworkLed
#endif
#if WPF  || WINUI || SMARTMAUI
        private Brush networkledDefault = new SolidColorBrush(Colors.Orange);
        public Brush NetworkLed
#endif
#if WINFORMS || WPF  || WINUI || SMARTMAUI
        {
            get
            {
                return networkledDefault;
            }
            set
            {
                if (networkledDefault != value)
                {
                    networkledDefault = value;
                    OnPropertyChanged(nameof(NetworkLed));
                }
            }
        }
#endif

        private bool tabcontrolenabled = false;
        public bool TabControlEnabled
        {
            get
            {
                return tabcontrolenabled;
            }
            set
            {
                if (tabcontrolenabled != value)
                {
                    tabcontrolenabled = value;
                    OnPropertyChanged(nameof(TabControlEnabled));
                }
            }
        }
        private bool transactivityenabled = false;
        public bool TransActivityEnabled
        {
            get
            {
                return transactivityenabled;
            }
            set
            {
                if (transactivityenabled != value)
                {
                    transactivityenabled = value;
                    OnPropertyChanged(nameof(TransActivityEnabled));
                }
            }
        }
        private bool acctsactivityenabled = false;
        public bool AcctsActivityEnabled
        {
            get
            {
                return acctsactivityenabled;
            }
            set
            {
                if (acctsactivityenabled != value)
                {
                    acctsactivityenabled = value;
                    OnPropertyChanged(nameof(AcctsActivityEnabled));
                }
            }
        }

#if WPF  || WINUI || SMARTMAUI
        private Visibility transribbonvisible = Visibility.Visible;
        public Visibility TransactionsRibbonVisible
#endif

#if ANDROIDX
        private ViewStates transribbonvisible = ViewStates.Visible;
        public ViewStates TransactionsRibbonVisible
#endif
#if WPF  || WINUI || ANDROIDX || SMARTMAUI
        {
            get
            {
                return transribbonvisible;
            }
            set
            {
                if (transribbonvisible != value)
                {
                    transribbonvisible = value;
                    OnPropertyChanged(nameof(TransactionsRibbonVisible));
                }
            }
        }
#endif

#if WPF  || WINUI || SMARTMAUI
        private Visibility acctsribbonvisible = Visibility.Collapsed;
        public Visibility AccountsRibbonVisible
#endif
#if ANDROIDX
        private ViewStates acctsribbonvisible = ViewStates.Invisible;
        public ViewStates AccountsRibbonVisible
#endif
#if WPF  || WINUI || ANDROIDX || SMARTMAUI
        {
            get
            {
                return acctsribbonvisible;
            }
            set
            {
                if (acctsribbonvisible != value)
                {
                    acctsribbonvisible = value;
                    OnPropertyChanged(nameof(AccountsRibbonVisible));
                }
            }
        }
#endif

        private string workingcurrency = "";
        public string WorkingCurrency
        {
            get
            {
                return workingcurrency;
            }
            set
            {
                if (workingcurrency != value)
                {
                    workingcurrency = value;
                    OnPropertyChanged(nameof(WorkingCurrency));
                }
            }
        }
        private string popupcurrency = "";
        public string PopupCurrency
        {
            get
            {
                return popupcurrency;
            }
            set
            {
                if (popupcurrency != value)
                {
                    popupcurrency = value;
                    OnPropertyChanged(nameof(PopupCurrency));
                }
            }
        }
        private string popuprate = "";
        public string PopupRate
        {
            get
            {
                return popuprate;
            }
            set
            {
                if (popuprate != value)
                {
                    popuprate = value;
                    OnPropertyChanged(nameof(PopupRate));
                }
            }
        }

        private string totalcost = "";
        public string TotalCost
        {
            get
            {
                return totalcost;
            }
            set
            {
                if (totalcost != value)
                {
                    totalcost = value;
                    OnPropertyChanged(nameof(TotalCost));
                }
            }
        }

#if WPF
        internal GridView FuckingGrid { get; set; }
        internal GridViewColumn FuckingGridColumn { get; set; }
#endif
        // All new Pain In My Arse stuff
        internal SmartFinance.Logins _lastAddedItem = null;
        
        public class InstitutionItem
        {
            public int InstitutionID { get; set; }
            public string Content { get; set; } // Public for binding to work

            public string Value { get; set; }
#if WINFORMS
            public System.Drawing.Color Colour { get; set; }
#endif
#if WPF  || WINUI
            public Brush Colour { get; set; }       // So Binding works
#endif
#if ANDROIDX
            public Android.Graphics.Color Colour { get; set; }   // Leave as Public for binding
#endif
#if SMARTMAUI
            public Brush Colour { get; set; }       // So Binding works
#endif
            public bool IsChecked { get; set; } // Never displayed
            public override string ToString()
            {
                return Content;
            }
        }

//        // Providers
//        public class ProviderItem
//        {
//            public int ProviderID { get; set; } // <= Make this generic
//            public string Content { get; set; }
//            public string Value { get; set; }
//#if WINFORMS
//            public System.Drawing.Color Colour { get; set; }
//#endif
//#if WPF  || WINUI
//            public Brush Colour { get; set; }           // So Binding works
//#endif
//#if ANDROIDX
//            public Android.Graphics.Color Colour { get; set; }           // So Binding works
//#endif
//#if SMARTMAUI
//            public Color Colour { get; set; }
//#endif
//            public bool IsChecked { get; set; }
//            public override string ToString()
//            {
//                return Content;
//            }
//        }

        // Wallets
        public class WalletItem
        {
            public int ProviderID { get; set; }  // <= Mage this generic
            public string Content { get; set; }
            public string Value { get; set; }
#if WINFORMS
            public System.Drawing.Color Colour { get; set; }
#endif
#if WPF  || WINUI
            public Brush Colour { get; set; }           // So Binding works
#endif
#if ANDROIDX
            public Android.Graphics.Color Colour { get; set; }           // So Binding works
#endif
#if SMARTMAUI

            public Color Colour { get; set; }
#endif
            public bool IsChecked { get; set; }
            public override string ToString()
            {
                return Content;
            }
        }

//        // Accounts
//        public class AccountItem
//        {
//            public int AccountID { get; set; }
//            public string Content { get; set; }
//            public string Value { get; set; }
//#if WINFORMS
//            public System.Drawing.Color Colour { get; set; }
//#endif
//#if WPF  || WINUI
//            public Brush Colour { get; set; }
//#endif
//#if ANDROIDX
//            public Android.Graphics.Color Colour { get; set; }
//#endif
//#if SMARTMAUI
//            public Color Colour { get; set; }
//#endif
//            public bool IsChecked { get; set; }

//            public override string ToString()
//            {
//                return Content;
//            }
//        }

        public class TransactionGroupItem
        {
            public int TransactionID { get; set; }
            public string Content { get; set; }
            public int CREDITDEBIT_INDICATOR { get; set; }
#if WINFORMS
            public System.Drawing.Color Colour { get; set; }           // So Binding works
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
            public bool IsChecked { get; set; }
            public override string ToString()
            {
                return Content;
            }
        }
        public class TransactionTypeItem
        {
            public int TransactionID { get; set; }
            public string Content { get; set; }
            public int CREDITDEBIT_INDICATOR { get; set; }
#if WINFORMS
            public System.Drawing.Color Colour { get; set; }           // So Binding works
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
            public bool IsChecked { get; set; }
            public override string ToString()
            {
                return Content;
            }
        }

        // DON'T CHANGE THESE TO INTERNAL!!!
        // YOU SCARED THE *SHIT* OUT OF ME ON SATURDAY COD
        // THEY DON'T SHOW UP UNLESS THEY ARE ==>PUBLIC<===
        public List<BrandItem> CreatedBrands { get; set; }
        public List<Row> Rows { get; set; }
        public List<EntryItem> CreatedEntries = new List<EntryItem>();
        public List<SmartFinance.Connections> ConnectionsViewList { get; set; }
        public List<SmartFinance.Logins> LoginsViewList { get; set; }


        
#if WPF  || WINUI || SMARTMAUI

        private Visibility _cryptovisible = Visibility.Collapsed;
        public Visibility CryptoVisible
#endif
#if ANDROIDX
        private ViewStates _cryptovisible = ViewStates.Invisible;
        public ViewStates CryptoVisible
#endif
#if WPF  || WINUI || ANDROIDX || SMARTMAUI
        {
            get
            {
                return _cryptovisible;
            }
            set
            {
                _cryptovisible = value;
                OnPropertyChanged(nameof(CryptoVisible));
            }
        }
#endif

        private List<SmartFinance.CommonTransactionsView> finance_transactionsDefault = new List<SmartFinance.CommonTransactionsView>();
        public List<SmartFinance.CommonTransactionsView> FinanceTransactions
        {
            get
            {
                return finance_transactionsDefault;
            }
            set
            {
                finance_transactionsDefault = value;
                OnPropertyChanged(nameof(FinanceTransactions));
            }
        }

        private string _connectionErrors = "";
        public string ConnectionErrors
        {
            get
            {
                return _connectionErrors;
            }
            set
            {
                if (_connectionErrors != value)
                {
                    _connectionErrors = value;
                    OnPropertyChanged(nameof(ConnectionErrors));
                }
            }
        }

        private string loginErrors = "";
        public string LoginErrors
        {
            get
            {
                return loginErrors;
            }
            set
            {
                if (loginErrors != value)
                {
                    loginErrors = value;
                    OnPropertyChanged(nameof(LoginErrors));
                }
            }
        }

        private string parameter1x;
        public string PARAMETER1
        {
            get
            {
                return parameter1x;
            }
            set
            {
                if (parameter1x != value)
                {
                    parameter1x = value;
                    OnPropertyChanged(nameof(PARAMETER1));
                }
            }
        }

        private string parameter2x;
        public string PARAMETER2
        {
            get
            {
                return parameter2x;
            }
            set
            {
                if (parameter2x != value)
                {
                    parameter2x = value;
                    OnPropertyChanged(nameof(PARAMETER2));
                }
            }
        }

        private string parameter3x;
        public string PARAMETER3
        {
            get
            {
                return parameter3x;
            }
            set
            {
                if (parameter3x != value)
                {
                    parameter3x = value;
                    OnPropertyChanged(nameof(PARAMETER3));
                }
            }
        }

        private SmartFinance.Connections selectedConnection;
        public SmartFinance.Connections SelectedConnection
        {
            get
            {
                return selectedConnection;
            }
            set
            {
                if (selectedConnection != value)
                {
                    selectedConnection = value;
                    OnPropertyChanged(nameof(SelectedConnection));
                }
            }
        }
        
        private BrandItem selectedBrand;
        public BrandItem SelectedBrand
        {
            get => selectedBrand;
            set
            {
                if (selectedBrand != value)
                {
                    selectedBrand = value;
                    OnPropertyChanged(nameof(SelectedBrand));
                }
            }
        }
        private string institutionschosen = "";
        public string FinanceInstitutionsText
        {
            get
            {
                return institutionschosen;
            }
            set
            {
                if (institutionschosen != value)
                {
                    institutionschosen = value;
                    OnPropertyChanged(nameof(FinanceInstitutionsText));
                }
            }
        }

        private SmartFinance.Logins selectedLogin;
        public SmartFinance.Logins SelectedLogin
        {
            get
            {
                return selectedLogin;
            }
            set
            {
                if (selectedLogin != value)
                {
                    selectedLogin = value;
                    OnPropertyChanged(nameof(SelectedLogin));
                }
            }
        }
        private List<InstitutionItem> institutionsList = new List<InstitutionItem>();
        public List<InstitutionItem> FinanceInstitutionsList
        {
            get
            {
                return institutionsList;
            }
            set
            {
                if (institutionsList != value)
                {
                    institutionsList = value;
                    this.OnPropertyChanged(nameof(FinanceInstitutionsList));
                }
            }
        }

        // Selenium
#if WINFORMS || WPF  || WINUI || ANDROIDX || SMARTMAUI
        internal List<Task> taskList = new List<Task>();
        // + For the Scrapers
#if !TEST
        internal readonly Dictionary<string, MainViewModel.TimerEntry> timers = new Dictionary<string, MainViewModel.TimerEntry>();
#endif
        internal int timerIdCounterX = 1;
#endif
#if ANDROIDX
        internal HtmlAgilityPack.HtmlDocument html_document { get; set; }
#endif
        internal List<KeyValuePair<string, string>> AccountLinks { get; set; }

        internal bool IDDisInitializing = false;
        internal bool IDDdropDownClicked = false;

        internal bool loggedIn = false;
        internal long sequence_no = 0;
        
        internal int SortDirection = 0;

        internal string owner = "";
        internal string baseUrl = "";

        // Sometimes .. we do Finance scrapes without the Cube face showing
        internal readonly char cubeface_code = SmartParametersV2016.Finance;

        internal List<AccountItem> accountItems = new List<AccountItem>();

        internal int account_id = 0;
        internal int transactions_count = 0;

#if WINFORMS || WPF  || WINUI || SMARTMAUI
        internal TaskCompletionSource<bool> notificationHandledSource { get; set; }
#endif

        internal string currentUrl = "";

        internal DateTime genericDateTime = SmartParametersV2016.defaultDate;
        internal string transaction_datetime = "";

        public string onetimepasscode { get; set; }

        internal List<SmartFinance.SelectedProviders> selectedProvidersList = new List<SmartFinance.SelectedProviders>();
        internal List<SmartFinance.SelectedAccounts> selectedAccountsList = new List<SmartFinance.SelectedAccounts>();
        //internal List<SmartFinance.SelectedTransactionGroups> selectedTransactiontypesList = new List<SmartFinance.SelectedTransactionGroups>();//Dr or Cr

#if ANDROIDX
        internal string accessStatus = "";
#endif
        internal string notificationContent = "";

#if !DBSERVER
        internal CancellationTokenSource financeCts { get; set; }// = new CancellationTokenSource();
        internal CancellationToken financeToken { get; set; }
#endif
        internal CultureInfo CultureINF { get; set; }
        internal int LastCultureSelectedIndex = -1;
        internal string FinanceCultureCode = "";
        internal short CurrencyOrdinal { get; set; }
        internal string ConvertToSymbol { get; set; }

        internal List<SmartFinance.Accounts> accountsFound = new List<SmartFinance.Accounts>();
        
        internal char category_code = SmartParametersV2016.defaultChar;// 'B', 'I' or 'S'
        // This for selection
        internal string categoryCodes = "";

        internal List<SmartFinance.TotalsView>[,] FinanceTotals { get; set; }
        internal List<(short Key, List<SmartFinance.TotalsView> Totals)> FinanceTotalsX = new List<(short Key, List<SmartFinance.TotalsView> Totals)>();

#if WINFORMS
        internal System.Windows.Forms.TabControl TabControlPanel  = new System.Windows.Forms.TabControl();
#endif
#if WPF
        internal TabControl TabControlPanel;
#endif
#if WINUI
        internal TabView TabControlPanel;
#endif
#if ANDROIDX
        internal List<string> TabTitles = new List<string>();
#endif
#if WPF  || WINUI || SMARTMAUI
        internal List<Grid> grids = new List<Grid>();
        internal Style CellStyle { get; set; }
#endif

        // Open Banking shit starts
        internal string obs_post_result = "";
        internal string rbs_access_token = "";
        internal string rbs_consentid = "";
        internal string rbs_authorization_code = "";
        internal string rbs_id_token = "";

        // Step 1
        internal string oauth_token_step1 = "";
        internal string oauth_token_secret1 = "";
        internal string oauth_callback_confirmed = "";
        // Step 2
        internal string oauth_token_step2 = "";
        internal string oauth_verifier = "";
        // Step3
        internal string oauth_token_step3 = "";
        internal string oauth_token_secret3 = "";
        internal string oauth_callback_confirmed3 = "";

        internal int step = 0;
        internal bool stepInProgress = false;
#if DEVELOPMENT
        //internal TextView SmsOTP { get; set; }

        //internal TextView Welcome { get; set; }
        //internal TextView UDPRNX { get; set; }
        //internal TextView PhoneNo { get; set; }
        //internal TextView Email { get; set; }
        //internal TextView TimeOffset { get; set; }

        //public System.Timers.Timer timerClockX = new System.Timers.Timer();

        // Open Banking shit ends

        //internal char test_code = SmartParametersV2016.default_test_code;
#endif
        
        internal string startup_udprn = "";

        internal char activeFlag = SmartParametersV2016.defaultChar;
        internal short area_code = -1;

        internal string brand_name = "";

        internal string meterFormat = "";    // I HATE null(s)

        internal int simulate_pointer = 0;
        internal bool go_simulate = false;

        internal bool autoswitch_pending = false;
        internal int returned_codes = 0;
#if WINFORMS
        internal Form FIPConnectionPage = new Form();
        internal Form FLDConnectionPage = new Form();
#endif
        internal string result = "";

#if WPF  || WINUI || SMARTMAUI
#if WPF 
        internal Window FCLWindow;
#endif
#if WINUI

        internal Popup FCLWindow;
#endif
#if SMARTMAUI

        internal ContentPage FCLWindow = new ContentPage();
#endif

        internal bool local_found_it = false;
        internal double HorizontalOffset = 0;
        internal double VerticalOffset = 0;
#endif
#if WINUI 
        internal XamlRoot xamlRoot;
#endif

        internal List<SmartFinance.TransactionsView> transactionsViewFound = new List<SmartFinance.TransactionsView>();

        internal List<SmartFinance.Transaction_Groups> transaction_groupsfound = new List<SmartFinance.Transaction_Groups>();

        internal char ChimpFuckers = SmartParametersV2016.defaultChar;

        internal string type = "";
        internal string response = "";

        internal bool bookingsort = true; // True Descending False=Ascending 

        internal List<FinanceViewModel.TransactionGroupItem> temp_groups = new List<FinanceViewModel.TransactionGroupItem>();

        internal int passcodeMaxlength = 0;
#if !DBSERVER
        internal Stream dno_stream = Stream.Null;
#endif
        internal bool sucked = false;

#if ANDROIDX
        internal Spinner FinanceInstitutions { get; set; }
        
        internal MultiSpinner FinanceProviders { get; set; }
        // Accounts spinner dropdown
        internal MultiSpinner FinanceAccounts { get; set; }
        // Transaction Groups spinner dropdown
        internal MultiSpinner FinanceTransactionGroups { get; set; }

        internal RadioButton RBBanks { get; set; }
        internal RadioButton RBSavings { get; set; }
        internal RadioButton RBInvestments { get; set; }
        internal RadioButton RBCryptos { get; set; }
        internal Spinner Addresses { get; set; }
        internal DatePicker StartDate { get; set; }
        internal DatePicker EndDate { get; set; }

        internal TextView ResultStartDate { get; set; }
        internal TextView ResultEndDate { get; set; }

        internal Spinner FinanceCultures { get; set; }

        internal RadioButton FinanceRBNON { get; set; }
        internal RadioButton FinanceRBGBP { get; set; }
        internal RadioButton FinanceRBEUR { get; set; }
        internal RadioButton FinanceRBUSD { get; set; }
        internal RadioButton FinanceRBJPY { get; set; }

        internal TextView GBPRate { get; set; }
        internal TextView EURRate { get; set; }
        internal TextView USDRate { get; set; }
        internal TextView JPYRate { get; set; }        

        internal PopupWindow FLDConnectionPage { get; set; }
        internal PopupWindow FIPConnectionPage { get; set; }
#endif

#if WINUI || SMARTMAUI
        //internal List<ProviderItem> SelecProviderItms = new List<ProviderItem>();
#endif
#if ANDROIDX
        internal List<ProviderItem> SelecProviderItms = new List<ProviderItem>();
#endif

        public string BookingDate { get; private set; } = "";
        public string SORTCODE { get; private set; } = "";
        public string ACCOUNT_NO { get; private set; } = "";
        public string DESCRIPTION { get; private set; } = "";
        public string CURRENCY { get; private set; } = "";
        public string TRANSGROUP_CODE { get; private set; } = "";
        public string TRANSACTION_CODE { get; private set; } = "";
        public string Credits { get; private set; } = "";
        public string Debits { get; private set; } = "";
        public string Balance { get; private set; } = "";

#if SMARTMAUI
        public string SubmitButton
        {
            get
            {
                return SignIn.BesetByChimps(SignIn.signinviewmodel,
                                            "FinanceSubmitTooltip");
            }
        }

        public string CancelButton
        {
            get
            {
                return SignIn.BesetByChimps(SignIn.signinviewmodel,
                                            "FinanceCancelTooltip");
            }
        }

        public string FinanceInstitutions
        {
            get
            {
                return SignIn.BesetByChimps(SignIn.signinviewmodel,
                                            "FinanceInstitutionsTooltip");
            }
        }

        public string FinanceProviders
        {
            get
            {
                return SignIn.BesetByChimps(SignIn.signinviewmodel,
                                            "FinanceProvidersTooltip");
            }
        }

        public string FinanceAccounts
        {
            get
            {
                return SignIn.BesetByChimps(SignIn.signinviewmodel,
                                            "FinanceAccountsTooltip");
            }
        }

        public string FinanceTransactionsGroups
        {
            get
            {
                return SignIn.BesetByChimps(SignIn.signinviewmodel,
                                            "FinanceTransactionsTooltip");
            }
        }

        public string FinanceBanks
        {
            get 
            {
                return SignIn.BesetByChimps(SignIn.signinviewmodel,
                                            "FinanceBanksTooltip"); 
            }
        }
        public string FinanceSavings
        {
            get
            {
                return SignIn.BesetByChimps(SignIn.signinviewmodel,
                                            "FinanceSavingsTooltip");
            }
        }
        public string FinanceInvestments
        {
            get
            {
                return SignIn.BesetByChimps(SignIn.signinviewmodel,
                                            "FinanceInvestmentsTooltip");
            }
        }

        public string FinanceCryptos
        {
            get
            {
                return SignIn.BesetByChimps(SignIn.signinviewmodel,
                                            "FinanceCryptosTooltip");
            }
        }

        public string FinanceAddresses
        {
            get
            {
                return SignIn.BesetByChimps(SignIn.signinviewmodel,
                                            "FinanceAddressesTooltip");
            }
        }

        public string ButtonAutoSwitch
        {
            get
            {
                return SignIn.BesetByChimps(SignIn.signinviewmodel,
                                            "FinanceAutoSwitchTooltip");
            }
        }

        public string StartingDate
        {
            get
            {
                return SignIn.BesetByChimps(SignIn.signinviewmodel,
                                            "FinanceStartDateTooltip");
            }
        }

        public string ResetDatesTooltip
        {
            get
            {
                return SignIn.BesetByChimps(SignIn.signinviewmodel,
                                            "FinanceResetDatesTooltip");
            }
        }

        public string EndingDate
        {
            get
            {
                return SignIn.BesetByChimps(SignIn.signinviewmodel,
                                            "FinanceEndDateTooltip");
            }
        }

        public string ResultsStartDate
        {
            get
            {
                return SignIn.BesetByChimps(SignIn.signinviewmodel,
                                            "FinanceResultsStartDateTooltip");
            }
        }

        public string ResultsEndDate
        {
            get
            {
                return SignIn.BesetByChimps(SignIn.signinviewmodel,
                                            "FinanceResultsEndDateTooltip");
            }
        }
        public string PostCodeTooltip
        {
            get
            {
                return SignIn.BesetByChimps(SignIn.signinviewmodel,
                                            "FinancePostCodeTooltip");
            }
        }

        public string Culture
        {
            get
            {
                return SignIn.BesetByChimps(SignIn.signinviewmodel,
                                            "FinanceCultureTooltip");
            }
        }

        public string ExchangeRates
        {
            get
            {
                return SignIn.BesetByChimps(SignIn.signinviewmodel,
                                            "FinanceExchangeRatesTooltip");
            }
        }

#endif
#if ANDROIDX
        internal ViewPager2 financePager2 { get; set; }
#endif
#if WINFORMS
        internal Form childWindow = new Form();
#endif
#if WPF
        internal Window childWindow;
#endif
#if WINUI
        internal Popup childWindow;
#endif
#if ANDROIDX
        //internal PopupWindow childWindow;
#endif
#if SMARTMAUI
        internal Popup childWindow = new Popup();
#endif
#if WPF
        internal TabItem targetItem = new TabItem();
#endif

#if WINUI
        // WINUI is done by manipulating the DataGrid Column width in FrontEndGUI
#endif
#if !DBSERVER

        private string _providersName = SmartParametersV2016.BanksProvidersName; // Default to something??
        public string ProvidersName
        {
            get
            {
                return _providersName;
            }
            set
            {
                if (_providersName != value)
                {
                    _providersName = value;
                    this.OnPropertyChanged(nameof(ProvidersName));
                }
            }
        }
#endif

#if WPF || WINUI  || SMARTMAUI
        private GridLength usernamewidth = new GridLength(0);
#endif
#if ANDROIDX
        //private GridLength usernamewidth = 0;
#endif
#if WPF || WINUI  || SMARTMAUI
        public GridLength UsernameWidth
#endif
#if ANDROIDX
        //public GridLength UsernameWidth
#endif
#if WPF || WINUI  || SMARTMAUI
        {
            get
            {
                return usernamewidth;
            }
            set
            {
                if (usernamewidth != value)
                {
                    usernamewidth = value;
                    this.OnPropertyChanged(nameof(UsernameWidth));
                }
            }
        }
#endif

#if WPF || WINUI 
        private Visibility usernameVisible = Visibility.Collapsed;
#endif
#if ANDROIDX || SMARTMAUI
        private bool usernameVisible = false;
#endif
#if WPF || WINUI 
        public Visibility UsernameVisible
#endif
#if ANDROIDX || SMARTMAUI
        public bool UsernameVisible
#endif
#if WPF || WINUI  || ANDROIDX || SMARTMAUI
        {
            get
            {
                return usernameVisible;
            }
            set
            {
                if (usernameVisible != value)
                {
                    usernameVisible = value;
                    this.OnPropertyChanged(nameof(UsernameVisible));
                }
            }
        }
#endif

        // Bank Logo
#if WPF || WINUI  || SMARTMAUI
        private GridLength banklogowidth = new GridLength(0);
#endif
#if ANDROIDX
        //private GridLength banklogowidth = 0;
#endif
#if WPF || WINUI  || SMARTMAUI
        public GridLength BankLogoWidth
#endif
#if ANDROIDX
        //public GridLength BankLogoWidth
#endif
#if WPF || WINUI  || SMARTMAUI
        {
            get
            {
                return banklogowidth;
            }
            set
            {
                if (banklogowidth != value)
                {
                    banklogowidth = value;
                    this.OnPropertyChanged(nameof(BankLogoWidth));
                }
            }
        }
#endif

#if WPF || WINUI 
        private Visibility banklogovisible = Visibility.Collapsed;
#endif
#if ANDROIDX || SMARTMAUI
        private bool banklogovisible = false;
#endif
#if WPF || WINUI 
        public Visibility BankLogoVisible
#endif
#if ANDROIDX || SMARTMAUI
        public bool BankLogoVisible
#endif
#if WPF || WINUI  || ANDROIDX || SMARTMAUI
        {
            get
            {
                return banklogovisible;
            }
            set
            {
                if (banklogovisible != value)
                {
                    banklogovisible = value;
                    this.OnPropertyChanged(nameof(BankLogoVisible));
                }
            }
        }
#endif

#if WPF || WINUI 
        private Visibility sortaccountvisible = Visibility.Visible;
#endif
#if ANDROIDX || SMARTMAUI
        private bool sortaccountvisible = false;

#endif
#if WPF || WINUI 
        public Visibility SortAccountVisible
#endif
#if ANDROIDX || SMARTMAUI
        public bool SortAccountVisible
#endif
#if WPF || WINUI  || ANDROIDX || SMARTMAUI
        {
            get
            {
                return sortaccountvisible;
            }
            set
            {
                if (sortaccountvisible != value)
                {
                    sortaccountvisible = value;
                    this.OnPropertyChanged(nameof(SortAccountVisible));
                }
            }
        }
#endif

#if WINFORMS || WPF || WINUI  || SMARTMAUI
        private double _quantityWidth = 0;
#endif
#if ANDROIDX
        private double _quantityWidth = 0;
#endif
#if WINFORMS || WPF || WINUI  || SMARTMAUI
        public double QuantityWidth
#endif
#if ANDROIDX
        public double QuantityWidth
#endif
#if WINFORMS || WPF || WINUI  || ANDROIDX || SMARTMAUI
        {
            get
            {
                return _quantityWidth;
            }
            set
            {
                if (_quantityWidth != value)
                {
                    _quantityWidth = value;
                    this.OnPropertyChanged(nameof(QuantityWidth));
                }
            }
        }
#endif


#if WINUI
        private List<TabTitle> _tabTitle = new List<TabTitle>();
        public List<TabTitle> TabTitles
        {
            get
            {
                return _tabTitle;
            }
            set
            {
                if (_tabTitle != value)
                {
                    _tabTitle = value;
                    this.OnPropertyChanged(nameof(TabTitles));
                }
            }
        }
#endif

#if WINUI
        private SolidColorBrush _tabContentColour = new SolidColorBrush(Colors.Transparent);
        public SolidColorBrush TabContentColour
        {
            get
            {
                return _tabContentColour;
            }
            set
            {
                if (_tabContentColour != value)
                {
                    _tabContentColour = value;
                    this.OnPropertyChanged(nameof(TabContentColour));
                }
            }
        }
#endif
#if SMARTMAUI
        private Color _tabContentColour = Colors.Transparent;
        public Color TabContentColour
        {
            get
            {
                return _tabContentColour;
            }
            set
            {
                if (_tabContentColour != value)
                {
                    _tabContentColour = value;
                    this.OnPropertyChanged(nameof(TabContentColour));
                }
            }
        }
#endif
#if ANDROIDX
        private double popupheight = 0;
        public double PopupHeight
        {
            get
            {
                return popupheight;
            }
            set
            {
                if (popupheight != value)
                {
                    popupheight = value;
                    OnPropertyChanged(nameof(PopupHeight));
                }
            }
        }

        private double popupwidth = 0;

        public double PopupWidth
        {
            get
            {
                return popupwidth;
            }
            set
            {
                if (popupwidth != value)
                {
                    popupwidth = value;
                    OnPropertyChanged(nameof(PopupWidth));
                }
            }
        }
#endif
#if !DBSERVER
        private bool connectionIdReadOnly = false;
        public bool ConnectionIdReadOnly
        {
            get
            {
                return connectionIdReadOnly;
            }
            set
            {
                if (connectionIdReadOnly != value)
                {
                    connectionIdReadOnly = value;
                    OnPropertyChanged(nameof(ConnectionIdReadOnly));
                }
            }
        }

        private bool loginsBanksChecked = false;
        public bool LoginsBanksChecked
        {
            get
            {
                return loginsBanksChecked;
            }
            set
            {
                if (loginsBanksChecked != value)
                {
                    loginsBanksChecked = value;
                    OnPropertyChanged(nameof(LoginsBanksChecked));
                }
            }
        }

        public bool loginsSavingsChecked = false;
        public bool LoginsSavingsChecked
        {
            get
            {
                return loginsSavingsChecked;
            }
            set
            {
                if (loginsSavingsChecked != value)
                {
                    loginsSavingsChecked = value;
                    OnPropertyChanged(nameof(LoginsSavingsChecked));
                }
            }
        }

        public bool loginsInvestmentsChecked = false;
        public bool LoginsInvestmentsChecked
        {
            get
            {
                return loginsInvestmentsChecked;
            }
            set
            {
                if (loginsInvestmentsChecked != value)
                {
                    loginsInvestmentsChecked = value;
                    OnPropertyChanged(nameof(LoginsInvestmentsChecked));
                }
            }
        }

        public bool loginsCryptosChecked = false;
        public bool LoginsCryptosChecked
        {
            get
            {
                return loginsCryptosChecked;
            }
            set
            {
                if (loginsCryptosChecked != value)
                {
                    loginsCryptosChecked = value;
                    OnPropertyChanged(nameof(LoginsCryptosChecked));

                }
            }
        }
#endif
#if WPF
        private Visibility loginsBanksVisibility = Visibility.Hidden;
#endif
#if WINUI  || SMARTMAUI
        private Visibility loginsBanksVisibility = Visibility.Collapsed;
#endif
#if ANDROIDX
        private bool loginsBanksVisibility = false;
#endif
#if WPF || WINUI  || SMARTMAUI
        public Visibility LoginsBanksVisibility
#endif
#if ANDROIDX
        public bool LoginsBanksVisibility
#endif
#if WPF || WINUI  || ANDROIDX || SMARTMAUI
        {
            get
            {
                return loginsBanksVisibility;
            }
            set
            {
                if (loginsBanksVisibility != value)
                {
                    loginsBanksVisibility = value;
                    OnPropertyChanged(nameof(LoginsBanksVisibility));
                }
            }
        }
#endif

#if WPF
        private Visibility loginsSavingsVisibility = Visibility.Hidden;
#endif
#if WINUI  || SMARTMAUI
        private Visibility loginsSavingsVisibility = Visibility.Collapsed;
#endif
#if ANDROIDX
        private bool loginsSavingsVisibility = false;
#endif
#if WPF || WINUI  || SMARTMAUI
        public Visibility LoginsSavingsVisibility
#endif
#if ANDROIDX
        public bool LoginsSavingsVisibility
#endif
#if WPF || WINUI  || ANDROIDX || SMARTMAUI
        {
            get
            {
                return loginsSavingsVisibility;
            }
            set
            {
                if (loginsSavingsVisibility != value)
                {
                    loginsSavingsVisibility = value;
                    OnPropertyChanged(nameof(LoginsSavingsVisibility));
                }
            }
        }
#endif

#if WPF
        private Visibility loginsInvestmentsVisibility = Visibility.Hidden;
#endif
#if WINUI  || SMARTMAUI
        private Visibility loginsInvestmentsVisibility = Visibility.Collapsed;
#endif
#if ANDROIDX
        private bool loginsInvestmentsVisibility = false;
#endif
#if WPF || WINUI  || SMARTMAUI
        public Visibility LoginsInvestmentsVisibility
#endif
#if ANDROIDX
        public bool LoginsInvestmentsVisibility
#endif
#if WPF || WINUI  || ANDROIDX || SMARTMAUI
        {
            get
            {
                return loginsInvestmentsVisibility;
            }
            set
            {
                if (loginsInvestmentsVisibility != value)
                {
                    loginsInvestmentsVisibility = value;
                    OnPropertyChanged(nameof(LoginsInvestmentsVisibility));
                }
            }
        }
#endif
#if WPF
        private Visibility loginsCryptosVisibility = Visibility.Hidden;
#endif
#if WINUI  || SMARTMAUI
        private Visibility loginsCryptosVisibility = Visibility.Collapsed;
#endif
#if ANDROIDX
        private bool loginsCryptosVisibility = false;
#endif
#if WPF || WINUI  || SMARTMAUI
        public Visibility LoginsCryptosVisibility
#endif
#if ANDROIDX
        public bool LoginsCryptosVisibility
#endif
#if WPF || WINUI  || ANDROIDX || SMARTMAUI
        {
            get
            {
                return loginsCryptosVisibility;
            }
            set
            {
                if (loginsCryptosVisibility != value)
                {
                    loginsCryptosVisibility = value;
                    OnPropertyChanged(nameof(LoginsCryptosVisibility));
                }
            }
        }
#endif

#if WINFORMS || WPF || WINUI  || ANDROIDX || SMARTMAUI
        private string financesubmitTooltipDefault = "FinanceSubmitTooltip";
        public string FinanceSubmitTooltip
        {
            get
            {
                return financesubmitTooltipDefault;
            }
            set
            {
                if (financesubmitTooltipDefault != value)
                {
                    financesubmitTooltipDefault = value;
                    OnPropertyChanged(nameof(FinanceSubmitTooltip));
                }
            }
        }

        private string financecancelTooltipDefault = "FinanceCancelTooltip";
        public string FinanceCancelTooltip
        {
            get
            {
                return financecancelTooltipDefault;
            }
            set
            {
                if (financecancelTooltipDefault != value)
                {
                    financecancelTooltipDefault = value;
                    OnPropertyChanged(nameof(FinanceCancelTooltip));
                }
            }
        }

        private string financebanksTooltipDefault = "FinanceBanksTooltip";
        public string FinanceBanksTooltip
        {
            get
            {
                return financebanksTooltipDefault;
            }
            set
            {
                if (financebanksTooltipDefault != value)
                {
                    financebanksTooltipDefault = value;
                    OnPropertyChanged(nameof(FinanceBanksTooltip));
                }
            }
        }

        private string financesavingsTooltipDefault = "FinanceSavingsTooltip";
        public string FinanceSavingsTooltip
        {
            get
            {
                return financesavingsTooltipDefault;
            }
            set
            {
                if (financesavingsTooltipDefault != value)
                {
                    financesavingsTooltipDefault = value;
                    OnPropertyChanged(nameof(FinanceSavingsTooltip));
                }
            }
        }

        private string financeinvestmentsTooltipDefault = "FinanceInvestmentsTooltip";
        public string FinanceInvestmentsTooltip
        {
            get
            {
                return financeinvestmentsTooltipDefault;
            }
            set
            {
                if (financeinvestmentsTooltipDefault != value)
                {
                    financeinvestmentsTooltipDefault = value;
                    OnPropertyChanged(nameof(FinanceInvestmentsTooltip));
                }
            }
        }

        private string financecryptosTooltipDefault = "FinanceCryptosTooltip";
        public string FinanceCryptosTooltip
        {
            get
            {
                return financecryptosTooltipDefault;
            }
            set
            {
                if (financecryptosTooltipDefault != value)
                {
                    financecryptosTooltipDefault = value;
                    OnPropertyChanged(nameof(FinanceCryptosTooltip));
                }
            }
        }
        private string financeprovidersTooltipDefault = "FinanceProvidersTooltip";
        public string FinanceProvidersTooltip
        {
            get
            {
                return financeprovidersTooltipDefault;
            }
            set
            {
                if (financeprovidersTooltipDefault != value)
                {
                    financeprovidersTooltipDefault = value;
                    OnPropertyChanged(nameof(FinanceProvidersTooltip));
                }
            }
        }

        private string financeaccountsTooltipDefault = "FinanceAccountsTooltip";
        public string FinanceAccountsTooltip
        {
            get
            {
                return financeaccountsTooltipDefault;
            }
            set
            {
                if (financeaccountsTooltipDefault != value)
                {
                    financeaccountsTooltipDefault = value;
                    OnPropertyChanged(nameof(FinanceAccountsTooltip));
                }
            }
        }

        private string financetransactionsTooltipDefault = "FinanceTransactionsTooltip";
        public string FinanceTransactionsTooltip
        {
            get
            {
                return financetransactionsTooltipDefault;
            }
            set
            {
                if (financetransactionsTooltipDefault != value)
                {
                    financetransactionsTooltipDefault = value;
                    OnPropertyChanged(nameof(FinanceTransactionsTooltip));
                }
            }
        }
#endif
#if !DBSERVER
        private string transactionstitleDefault = "";
        public string TransactionsTitle
        {
            get
            {
                return transactionstitleDefault;
            }
            set
            {
                if (transactionstitleDefault != value)
                {
                    transactionstitleDefault = value;
                    OnPropertyChanged(nameof(TransactionsTitle));
                }
            }
        }

        private string transactionslisttitleDefault = "";
        public string TransactionsListTitle
        {
            get
            {
                return transactionslisttitleDefault;
            }
            set
            {
                if (transactionslisttitleDefault != value)
                {
                    transactionslisttitleDefault = value;
                    OnPropertyChanged(nameof(TransactionsListTitle));
                }
            }
        }
#endif

#if !DBSERVER
        private string transactiondescriptionheaderDefault = "";
        public string TransactionDescriptionTitle
        {
            get
            {
                return transactiondescriptionheaderDefault;
            }
            set
            {
                if (transactiondescriptionheaderDefault != value)
                {
                    transactiondescriptionheaderDefault = value;
                    OnPropertyChanged(nameof(TransactionDescriptionTitle));
                }
            }
        }

        private bool transactiondescriptionvisibleDefault = false;
        public bool TransactionDescriptionVisible
        {
            get
            {
                return transactiondescriptionvisibleDefault;
            }
            set
            {
                if (transactiondescriptionvisibleDefault != value)
                {
                    transactiondescriptionvisibleDefault = value;
                    OnPropertyChanged(nameof(TransactionDescriptionVisible));
                }
            }
        }
        public class TransactionDescription
        {
            public string Field { get; set; }
            public string Value { get; set; }
        }

        public List<TransactionDescription> tranny = new List<TransactionDescription>();
        public List<TransactionDescription> Transaction { get { return tranny; } }

        private string moneyDefault = "";
        public string Money
        {
            get
            {
                return moneyDefault;
            }
            set
            {
                if (moneyDefault != value)
                {
                    moneyDefault = value;
                    this.OnPropertyChanged(nameof(Money));
                }
            }
        }

        private string greetingDefault = "";
        public string Greeting
        {
            get
            {
                return greetingDefault;
            }
            set
            {
                if (greetingDefault != value)
                {
                    greetingDefault = value;
                    this.OnPropertyChanged(nameof(Greeting));
                }
            }
        }

        private string usernameDefault = "";
        public string Username
        {
            get
            {
                return usernameDefault;
            }
            set
            {
                if (usernameDefault != value)
                {
                    usernameDefault = value;
                    this.OnPropertyChanged(nameof(Username));
                }
            }
        }
#endif

#if WINFORMS || WPF  || WINUI || SMARTMAUI
        private string sortcodeDefault = "";
        public string SortCode
        {
            get
            {
                return sortcodeDefault;
            }
            set
            {
                if (sortcodeDefault != value)
                {
                    sortcodeDefault = value;
                    this.OnPropertyChanged(nameof(SortCode));
                }
            }
        }
#endif

#if ANDROIDX
        internal TextView SortCode { get; set; }
#endif
#if !DBSERVER
        internal double SortCodeHeader = 0;
        internal double SortCodeColumn = 0;
#endif
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
                    this.OnPropertyChanged(nameof(AccountNo));
                }
            }
        }
#endif

#if ANDROIDX
        internal TextView AccountNo { get; set; }
#endif
#if !DBSERVER
        internal double AccountNoHeader = 0;
        internal double AccountNoColumn = 0;


        private bool fplv = false;
        public bool FinanceProvidersListVisible
        {
            get
            {
                return fplv;
            }
            set
            {
                if (fplv != value)
                {
                    fplv = value;
                    this.OnPropertyChanged(nameof(FinanceProvidersListVisible));
                }
            }
        }
#endif

#if ANDROIDX
        //internal List<InstitutionItem> FinanceSpinnerInstitutionsList { get; set; }
#endif

#if WPF  || WINUI || SMARTMAUI
        private dynamic[] financeinstitutionselections = Array.Empty<dynamic>();
        public dynamic[] FinanceInstitution_Selections
        {
            get
            {
                return financeinstitutionselections;
            }
            set
            {
                if (financeinstitutionselections != value)
                {
                    financeinstitutionselections = value;
                    this.OnPropertyChanged(nameof(FinanceInstitution_Selections));
                }
            }
        }

        private Brush institutionscolour = new SolidColorBrush(Colors.Transparent);
        public Brush FinanceInstitutionsColour
        {
            get
            {
                return institutionscolour;
            }
            set
            {
                if (institutionscolour != value)
                {
                    institutionscolour = value;
                    this.OnPropertyChanged(nameof(FinanceInstitutionsColour));
                }
            }
        }
#endif

#if ANDROIDX
        private Android.Graphics.Color labelcolour = Android.Graphics.Color.White;
        public Android.Graphics.Color LabelColour
        {
            get
            {
                return labelcolour;
            }
            set
            {
                if (labelcolour != value)
                {
                    labelcolour = value;
                    this.OnPropertyChanged(nameof(LabelColour));
                }
            }
        }
#endif
#if !DBSERVER
        private string providerschosen = SmartParametersV2016.providersprompt;
        public string FinanceProviders_Text
        {
            get
            {
                return providerschosen;
            }
            set
            {
                if (providerschosen != value)
                {
                    providerschosen = value;
                    this.OnPropertyChanged(nameof(FinanceProviders_Text));
                }
            }
        }
#endif
#if WPF  || WINUI || SMARTMAUI
        private double institutionwidth = 0;
        public double InstitutionWidth
        {
            get
            {
                return institutionwidth;
            }
            set
            {
                if (institutionwidth != value)
                {
                    institutionwidth = value;
                    this.OnPropertyChanged(nameof(InstitutionWidth));
                }
            }
        }

        private double institution_translationx = 0;
        public double InstitutionTranslationX
        {
            get
            {
                return institution_translationx;
            }
            set
            {
                if (institution_translationx != value)
                {
                    institution_translationx = value;
                    this.OnPropertyChanged(nameof(InstitutionTranslationX));
                }
            }
        }

        private double institution_translationy = 0;
        public double InstitutionTranslationY
        {
            get
            {
                return institution_translationy;
            }
            set
            {
                if (institution_translationy != value)
                {
                    institution_translationy = value;
                    this.OnPropertyChanged(nameof(InstitutionTranslationY));
                }
            }
        }
        private double providerwidth = 0;
        public double ProviderWidth
        {
            get
            {
                return providerwidth;
            }
            set
            {
                if (providerwidth != value)
                {
                    providerwidth = value;
                    this.OnPropertyChanged(nameof(ProviderWidth));
                }
            }
        }

        private double provider_translationx = 0;
        public double ProviderTranslationX
        {
            get
            {
                return provider_translationx;
            }
            set
            {
                if (provider_translationx != value)
                {
                    provider_translationx = value;
                    this.OnPropertyChanged(nameof(ProviderTranslationX));
                }
            }
        }

        private double provider_translationy = 0;
        public double ProviderTranslationY
        {
            get
            {
                return provider_translationy;
            }
            set
            {
                if (provider_translationy != value)
                {
                    provider_translationy = value;
                    this.OnPropertyChanged(nameof(ProviderTranslationY));
                }
            }
        }

        private bool institutionpopupvisible = false;
        public bool InstitutionPopupVisible
        {
            get
            {
                return institutionpopupvisible;
            }
            set
            {
                if (institutionpopupvisible != value)
                {
                    institutionpopupvisible = value;
                    this.OnPropertyChanged(nameof(InstitutionPopupVisible));
                }
            }
        }

        private bool providerpopupvisible = false;
        public bool ProviderPopupVisible
        {
            get
            {
                return providerpopupvisible;
            }
            set
            {
                if (providerpopupvisible != value)
                {
                    providerpopupvisible = value;
                    this.OnPropertyChanged(nameof(ProviderPopupVisible));
                }
            }
        }

        private double accountwidth = 0;
        public double AccountWidth
        {
            get
            {
                return accountwidth;
            }
            set
            {
                if (accountwidth != value)
                {
                    accountwidth = value;
                    this.OnPropertyChanged(nameof(AccountWidth));
                }
            }
        }

        private double accounttranslationx = 0;
        public double AccountTranslationX
        {
            get
            {
                return accounttranslationx;
            }
            set
            {
                if (accounttranslationx != value)
                {
                    accounttranslationx = value;
                    this.OnPropertyChanged(nameof(AccountTranslationX));
                }
            }
        }

        private double accounttranslationy = 0;
        public double AccountTranslationY
        {
            get
            {
                return accounttranslationy;
            }
            set
            {
                if (accounttranslationy != value)
                {
                    accounttranslationy = value;
                    this.OnPropertyChanged(nameof(AccountTranslationY));
                }
            }
        }

        private bool accountpopupvisible = false;
        public bool AccountPopupVisible
        {
            get
            {
                return accountpopupvisible;
            }
            set
            {
                if (accountpopupvisible != value)
                {
                    accountpopupvisible = value;
                    this.OnPropertyChanged(nameof(AccountPopupVisible));
                }
            }
        }
#endif
#if !DBSERVER        
        private List<WalletItem> financewalletslist = new List<WalletItem>();
        public List<WalletItem> FinanceWalletsList
        {
            get
            {
                return financewalletslist;
            }
            set
            {
                if (financewalletslist != value)
                {
                    financewalletslist = value;
                    this.OnPropertyChanged(nameof(FinanceWalletsList));
                }
            }
        }
#endif
#if WPF  || WINUI || SMARTMAUI
        private dynamic[] financeproviderselections = Array.Empty<dynamic>();
        public dynamic[] FinanceProvider_Selections
        {
            get
            {
                return financeproviderselections;
            }
            set
            {
                if (financeproviderselections != value)
                {
                    financeproviderselections = value;
                    this.OnPropertyChanged(nameof(FinanceProvider_Selections));
                }
            }
        }

        public ImageSource arrowbutton;
        public ImageSource ArrowButton
        {
            get
            {
                return arrowbutton;
            }
            set
            {
                if (arrowbutton != value)
                {
                    arrowbutton = value;
                    this.OnPropertyChanged(nameof(ArrowButton));
                }
            }
        }

        private string accountschosen = SmartParametersV2016.accountsprompt;
        public string FinanceAccounts_Text
        {
            get
            {
                return accountschosen;
            }
            set
            {
                if (accountschosen != value)
                {
                    accountschosen = value;
                    this.OnPropertyChanged(nameof(FinanceAccounts_Text));
                }
            }
        }
#endif
#if !DBSERVER


#if SMARTMAUI
        public class ProviderItem : IMultiSelectItem, INotifyPropertyChanged
#else
        public class ProviderItem

#endif
        {
            private bool _isChecked;

            public bool IsChecked
            {
                get => _isChecked;
                set
                {
                    if (_isChecked == value)
                        return;

                    _isChecked = value;
                    PropertyChanged?.Invoke(
                        this,
                        new PropertyChangedEventArgs(nameof(IsChecked)));
                }
            }

            public event PropertyChangedEventHandler PropertyChanged;

            public int ProviderID { get; set; } // <= Make this generic

            public string Content { get; set; }
            public string Value { get; set; }
#if WPF  || WINUI
            public Brush Colour { get; set; }
#else
            public Color Colour { get; set; }
#endif
            public override string ToString()
            {
                return Content;
            }

        }


        private List<AccountItem> financeaccounts = new List<AccountItem>();
        public List<AccountItem> FinanceAccountsList
        {
            get
            {
                return financeaccounts;
            }
            set
            {
                if (financeaccounts != value)
                {
                    financeaccounts = value;
                    this.OnPropertyChanged(nameof(FinanceAccountsList));
                }
            }
        }

        private List<ProviderItem> financeproviders = new List<ProviderItem>();
        public List<ProviderItem> FinanceProvidersList
        {
            get
            {
                return financeproviders;
            }
            set
            {
                if (financeproviders != value)
                {
                    financeproviders = value;
                    this.OnPropertyChanged(nameof(FinanceProvidersList));
                }
            }
        }

#if SMARTMAUI
        public class AccountItem : IMultiSelectItem, INotifyPropertyChanged
#else
        public class AccountItem
#endif
        {
            private bool _isChecked;

            public bool IsChecked
            {
                get => _isChecked;
                set
                {
                    if (_isChecked == value)
                        return;

                    _isChecked = value;
                    PropertyChanged?.Invoke(
                        this,
                        new PropertyChangedEventArgs(nameof(IsChecked)));
                }
            }

            public event PropertyChangedEventHandler PropertyChanged;

            public int AccountID { get; set; }
            public string Content { get; set; }
            public string Value { get; set; }
#if WPF  || WINUI
            public Brush Colour { get; set; }
#else
            public Color Colour { get; set; }
#endif

            public override string ToString()
            {
                return Content;
            }
        }

        private List<TransactionGroupItem> financetransactiongroups = new List<TransactionGroupItem>();
        public List<TransactionGroupItem> FinanceTransactionGroupsList
        {
            get
            {
                return financetransactiongroups;
            }
            set
            {
                if (financetransactiongroups != value)
                {
                    financetransactiongroups = value;
                    this.OnPropertyChanged(nameof(FinanceTransactionGroupsList));
                }
            }
        }

        private bool transactionGroupsEnabled = false;
        public bool TransactionGroupsEnabled
        {
            get
            {
                return transactionGroupsEnabled;
            }
            set
            {
                if (transactionGroupsEnabled != value)
                {
                    transactionGroupsEnabled = value;
                    this.OnPropertyChanged(nameof(TransactionGroupsEnabled));
                }
            }
        }

        private string transactionschosen = SmartParametersV2016.transactionsprompt;
        public string FinanceTransactionsText
        {
            get
            {
                return transactionschosen;
            }
            set
            {
                if (transactionschosen != value)
                {
                    transactionschosen = value;
                    OnPropertyChanged(nameof(FinanceTransactionsText));
                }
            }
        }

        private dynamic[] financeaccountselections = Array.Empty<dynamic>();
        public dynamic[] FinanceAccount_Selections
        {
            get
            {
                return financeaccountselections;
            }
            set
            {
                if (financeaccountselections != value)
                {
                    financeaccountselections = value;
                    OnPropertyChanged(nameof(FinanceAccount_Selections));
                }
            }
        }

#if !TEST
        private List<MainViewModel.AddressItem> addressesView = new List<MainViewModel.AddressItem>();
        public List<MainViewModel.AddressItem> FinanceAddressesList
        {
            get
            {
                return addressesView;
            }
            set
            {
                if (addressesView != value)
                {
                    addressesView = value;
                    OnPropertyChanged(nameof(FinanceAddressesList));
                }
            }
        }
#endif

#if !TEST
        private List<MainViewModel.CultureItem> culturesView = new List<MainViewModel.CultureItem>();
        public List<MainViewModel.CultureItem> FinanceCulturesList
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
                    OnPropertyChanged(nameof(FinanceCulturesList));
                }
            }
        }
#endif
#endif
#if ANDROIDX
        internal List<string> FinanceSpinnerAddressesList { get; set; }
#endif
#if WINFORMS
        private string financelanguage = "";
        public string FinanceLanguage
#endif
#if WPF
        private XmlLanguage financelanguage;
        public XmlLanguage FinanceLanguage
#endif
#if WINUI || SMARTMAUI
        private string financelanguage = "";
        public string FinanceLanguage
#endif
#if ANDROIDX
        private string financelanguage = "";
        public string FinanceLanguage
#endif
#if WINFORMS || WPF || WINUI  || ANDROIDX || SMARTMAUI
        {
            get
            {
                return financelanguage;
            }
            set
            {
                if (financelanguage != value)
                {
                    financelanguage = value;
                    OnPropertyChanged(nameof(FinanceLanguage));
                }
            }
        }
#endif

#if WINFORMS || WPF  || WINUI || SMARTMAUI
        private bool financenonchecked = false;
        public bool FinanceNONChecked
        {
            get
            {
                return financenonchecked;
            }
            set
            {
                if (financenonchecked != value)
                {
                    financenonchecked = value;
                    this.OnPropertyChanged(nameof(FinanceNONChecked));
                }
            }
        }

        private bool financegbpchecked = false;
        public bool FinanceGBPChecked
        {
            get
            {
                return financegbpchecked;
            }
            set
            {
                if (financegbpchecked != value)
                {
                    financegbpchecked = value;
                    OnPropertyChanged(nameof(FinanceGBPChecked));
                }
            }
        }

        private bool financeeurchecked = false;
        public bool FinanceEURChecked
        {
            get
            {
                return financeeurchecked;
            }
            set
            {
                if (financeeurchecked != value)
                {
                    financeeurchecked = value;
                    OnPropertyChanged(nameof(FinanceEURChecked));
                }
            }
        }

        private bool financeusdchecked = false;
        public bool FinanceUSDChecked
        {
            get
            {
                return financeusdchecked;
            }
            set
            {
                if (financeusdchecked != value)
                {
                    financeusdchecked = value;
                    OnPropertyChanged(nameof(FinanceUSDChecked));
                }
            }
        }

        private bool financejpychecked = false;
        public bool FinanceJPYChecked
        {
            get
            {
                return financejpychecked;
            }
            set
            {
                if (financejpychecked != value)
                {
                    financejpychecked = value;
                    OnPropertyChanged(nameof(FinanceJPYChecked));
                }
            }
        }
#endif

#if WPF  || WINUI || SMARTMAUI
        public ImageSource cashbag;
        public ImageSource CashBag
        {
            get
            {
                return cashbag;
            }
        }
#endif

#if WPF  || WINUI || SMARTMAUI
        public ImageSource piggybank;
        public ImageSource PiggyBank
        {
            get
            {
                return piggybank;
            }
        }
#endif

#if WPF  || WINUI || SMARTMAUI
        public ImageSource stockmarket;
        public ImageSource StockMarket
        {
            get
            {
                return stockmarket;
            }
        }
#endif

#if WPF  || WINUI || SMARTMAUI
        public ImageSource bitcoin;
        public ImageSource Bitcoin
        {
            get
            {
                return bitcoin;
            }
        }
#endif

#if !DBSERVER
        private string customernumber = "";
        public string CustomerNumber
        {
            get
            {
                return customernumber;
            }
            set
            {
                if (customernumber != value && value.Length <= 35)
                {
                    customernumber = value;
                    this.OnPropertyChanged(nameof(CustomerNumber));
                }
            }
        }

        private string parameter1 = "";
        public string Parameter1
        {
            get
            {
                return parameter1;
            }
            set
            {
                if (parameter1 != value)
                {
                    parameter1 = value;
                    OnPropertyChanged(nameof(Parameter1));
                }
            }
        }

        private string passcode = "";
        public string Passcode
        {
            get
            {
                return passcode;
            }
            set
            {
                if (passcode != value && value.Length <= passcodeMaxlength)
                {
                    passcode = value;
                    this.OnPropertyChanged(nameof(Passcode));
                }
            }
        }
#endif
#if WINFORMS
        private System.Drawing.Color placeholdercolour = System.Drawing.Color.Transparent;
        public System.Drawing.Color PlaceholderColour
#endif
#if WPF  || WINUI || SMARTMAUI
        private Brush placeholdercolour = new SolidColorBrush(Colors.Transparent);
        public Brush PlaceholderColour
#endif
#if ANDROIDX
        private Android.Graphics.Color placeholdercolour = Android.Graphics.Color.Black;
        public Android.Graphics.Color PlaceholderColour
#endif
#if WINFORMS || WPF || WINUI  || ANDROIDX || SMARTMAUI
        {
            get
            {
                return placeholdercolour;
            }
            set
            {
                if (placeholdercolour != value)
                {
                    placeholdercolour = value;
                    OnPropertyChanged(nameof(PlaceholderColour));
                }
            }
        }
#endif
#if !DBSERVER
        // Its a wonder I have EVER finished this program with this goon wittering away...
        private int datagrid_screen = -1;
        public int DataGridScreen
        {
            get
            {
                return datagrid_screen;
            }
            set
            {
                if (datagrid_screen != value)
                {
                    datagrid_screen = value;
                    this.OnPropertyChanged(nameof(DataGridScreen));
                }
            }
        }

        private string whatyouselected = "";
        public string WhatYouSelected
        {
            get
            {
                return whatyouselected;
            }
            set
            {
                if (whatyouselected != value)
                {
                    whatyouselected = value;
                    OnPropertyChanged(nameof(WhatYouSelected));
                }
            }
        }

        private int selectedtabindex = 0;
        public int SelectedTabIndex
        {
            get
            {
                return selectedtabindex;
            }
            set
            {
                // For reasons unknown to me (..but perhaps because this entire WINUI twaddle was written by chimps??)
                // The SelectedTabIndex doesn't reflect any changes when the tabs are scrolled through.
                // The BINDING works because the 'get' above works fine, its just the 'set' that doesn't work ...
                if (selectedtabindex != value)
                {
                    selectedtabindex = value;
                    this.OnPropertyChanged(nameof(SelectedTabIndex));
                }
            }
        }
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
                    this.OnPropertyChanged(nameof(NextConnectionMessage));
                }
            }
        }
#endif

#if ANDROIDX
        internal TextView NextConnectionMessage { get; set; }
#endif
#if !DBSERVER
        private int transactionsposition = -1;
        public int FinanceTransactionsPosition
        {
            get
            {
                return transactionsposition;
            }
            set
            {
                // Never seems to hit this for some reason ...??
                if (transactionsposition != value)
                {
                    transactionsposition = value;
                    this.OnPropertyChanged(nameof(FinanceTransactionsPosition));
                }
            }
        }
#endif
#if WINFORMS
        private System.Drawing.Color addressescolorDefault = System.Drawing.Color.Transparent;
        public System.Drawing.Color FinanceAddressesColor
#endif
#if WPF  || WINUI || SMARTMAUI
        private Brush addressescolorDefault = new SolidColorBrush(Colors.Transparent);
        public Brush FinanceAddressesColor
#endif
#if ANDROIDX
        private Android.Graphics.Color addressescolorDefault = Android.Graphics.Color.Transparent;
        public Android.Graphics.Color FinanceAddressesColor
#endif
#if WINFORMS || WPF || WINUI  || ANDROIDX || SMARTMAUI
        {
            get
            {
                return addressescolorDefault;
            }
            set
            {
                if (addressescolorDefault != value)
                {
                    addressescolorDefault = value;
                    this.OnPropertyChanged(nameof(FinanceAddressesColor));
                }
            }
        }
#endif
#if !DBSERVER
        private int institutionselectedindex = -1;
        public int InstitutionSelectedIndex
        {
            get
            {
                return institutionselectedindex;
            }

            set
            {
                if (institutionselectedindex != value)
                {
                    institutionselectedindex = value;
                    OnPropertyChanged(nameof(InstitutionSelectedIndex));
                }
            }
        }

        private bool institutionsenabledDefault = false;
        public bool InstitutionsEnabled
        {
            get
            {
                return institutionsenabledDefault;
            }
            set
            {
                if (institutionsenabledDefault != value)
                {
                    institutionsenabledDefault = value;
                    this.OnPropertyChanged(nameof(InstitutionsEnabled));
                }
            }
        }

        private int financecultureelectedindex = -1;
        public int FinanceCultureSelectedIndex
        {
            get
            {
                return financecultureelectedindex;
            }
            set
            {
                if (financecultureelectedindex != value)
                {
                    financecultureelectedindex = value;
                    this.OnPropertyChanged(nameof(FinanceCultureSelectedIndex));
                }
            }
        }
        private bool culturesenabledDefault = false;
        public bool FinanceCulturesEnabled
        {
            get
            {
                return culturesenabledDefault;
            }
            set
            {
                if (culturesenabledDefault != value)
                {
                    culturesenabledDefault = value;
                    this.OnPropertyChanged(nameof(FinanceCulturesEnabled));
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
                    this.OnPropertyChanged(nameof(AddressSelectedIndex));
                }
            }
        }

        private int providerselectedindex = -1;
        public int ProviderSelectedIndex
        {
            get
            {
                return providerselectedindex;
            }

            set
            {
                if (providerselectedindex != value)
                {
                    providerselectedindex = value;
                    OnPropertyChanged(nameof(ProviderSelectedIndex));
                }
            }
        }

        private bool providersenabledDefault = false;
        public bool ProvidersEnabled
        {
            get
            {
                return providersenabledDefault;
            }
            set
            {
                if (providersenabledDefault != value)
                {
                    providersenabledDefault = value;
                    this.OnPropertyChanged(nameof(ProvidersEnabled));
                }
            }
        }

        private int accountselectedindex = -1;
        public int AccountSelectedIndex
        {
            get
            {
                return accountselectedindex;
            }

            set
            {
                if (accountselectedindex != value)
                {
                    accountselectedindex = value;
                    OnPropertyChanged(nameof(AccountSelectedIndex));
                }
            }
        }
        
        private bool accountsenabledDefault = false;
        public bool AccountsEnabled
        {
            get
            {
                return accountsenabledDefault;
            }
            set
            {
                if (accountsenabledDefault != value)
                {
                    accountsenabledDefault = value;
                    this.OnPropertyChanged(nameof(AccountsEnabled));
                }
            }
        }

        private bool transactionsenabledDefault = false;
        public bool TransactionsEnabled
        {
            get
            {
                return transactionsenabledDefault;
            }
            set
            {
                if (transactionsenabledDefault != value)
                {
                    transactionsenabledDefault = value;
                    this.OnPropertyChanged(nameof(TransactionsEnabled));
                }
            }
        }
        private bool addressessenabledDefault = false;
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
                    this.OnPropertyChanged(nameof(AddressesEnabled));
                }
            }
        }

        private bool financeconversionsenabled = false;
        public bool FinanceConversionsEnabled
        {
            get
            {
                return financeconversionsenabled;
            }
            set
            {
                if (financeconversionsenabled != value)
                {
                    financeconversionsenabled = value;
                    this.OnPropertyChanged(nameof(FinanceConversionsEnabled));
                }
            }
        }

        private bool printbuttonenabled = true;
        public bool PrintButtonEnabled
        {
            get
            {
                return printbuttonenabled;
            }
            set
            {
                if (printbuttonenabled != value)
                {
                    printbuttonenabled = value;
                    this.OnPropertyChanged(nameof(PrintButtonEnabled));
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
                    this.OnPropertyChanged(nameof(SubmitStatus));
                }
            }
        }
#endif
#if WINFORMS
        private System.Windows.Forms.Button submitDefault { get; set; }
        public System.Windows.Forms.Button Submit
#endif
#if WPF
        private Button submitDefault { get; set; }
        public Button Submit
#endif
#if WINFORMS || WPF
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
                    this.OnPropertyChanged(nameof(Submit));
                }
            }
        }
#endif

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
                    this.OnPropertyChanged(nameof(Submit));
                }
            }
        }
#endif

#if !DBSERVER
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
                    this.OnPropertyChanged(nameof(CancelStatus));
                }
            }
        }

#endif
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
                    this.OnPropertyChanged(nameof(Cancel));
                }
            }
        }
#endif

#if !DBSERVER
        private bool financeculturestatusDefault = false;
        public bool FinanceCultureStatus
        {
            get
            {
                return financeculturestatusDefault;
            }
            set
            {
                if (financeculturestatusDefault != value)
                {
                    financeculturestatusDefault = value;
                    this.OnPropertyChanged(nameof(FinanceCultureStatus));
                }
            }
        }
#endif

#if WINFORMS
        private System.Drawing.Color financeculturecolourDefault = System.Drawing.Color.Black;
        public System.Drawing.Color FinanceCultureColor
#endif
#if WPF  || WINUI || SMARTMAUI
        private Brush financeculturecolourDefault = new SolidColorBrush(Colors.Black);
        public Brush FinanceCultureColor
#endif
#if ANDROIDX
        private Android.Graphics.Color financeculturecolourDefault = Android.Graphics.Color.Black;
        public Android.Graphics.Color FinanceCultureColor
#endif
#if WINFORMS || WPF || WINUI  || ANDROIDX || SMARTMAUI
        {
            get
            {
                return financeculturecolourDefault;
            }
            set
            {
                if (financeculturecolourDefault != value)
                {
                    financeculturecolourDefault = value;
                    this.OnPropertyChanged(nameof(FinanceCultureColor));
                }
            }
        }
#endif

#if !DBSERVER
        public bool contrastColour = true;

        internal short exchange_index = 0;

        private string financeculturecontentDefault = "Culture";
        public string FinanceCultureContent
        {
            get
            {
                return financeculturecontentDefault;
            }
            set
            {
                if (financeculturecontentDefault != value)
                {
                    financeculturecontentDefault = value;
                    this.OnPropertyChanged(nameof(FinanceCultureContent));
                }
            }
        }
#endif

#if WINFORMS
        private System.Drawing.Color institutionsborderDefault = System.Drawing.Color.Transparent;
        public System.Drawing.Color InstitutionsBorderBrush
#endif
#if WPF  || WINUI || SMARTMAUI
        private Brush institutionsborderDefault = new SolidColorBrush(Colors.Transparent);
        public Brush InstitutionsBorderBrush
#endif
#if ANDROIDX
        private Android.Graphics.Color institutionsborderDefault = Android.Graphics.Color.Transparent;
        public Android.Graphics.Color InstitutionsBorderBrush
#endif
#if WINFORMS || WPF || WINUI  || ANDROIDX || SMARTMAUI
        {
            get
            {
                return institutionsborderDefault;
            }
            set
            {
                if (institutionsborderDefault != value)
                {
                    institutionsborderDefault = value;
                    this.OnPropertyChanged(nameof(InstitutionsBorderBrush));
                }
            }
        }
#endif
#if WPF  || WINUI || SMARTMAUI
        private Thickness institutionsborderthicknessDefault;
        public Thickness InstitutionsBorderThickness
#endif
#if WPF  || WINUI || SMARTMAUI
        {
            get
            {
                return institutionsborderthicknessDefault;
            }
            set
            {
                if (institutionsborderthicknessDefault != value)
                {
                    institutionsborderthicknessDefault = value;
                    this.OnPropertyChanged(nameof(InstitutionsBorderThickness));
                }
            }
        }
#endif


#if WINFORMS
        private System.Drawing.Color providersborderDefault = System.Drawing.Color.Transparent;
        public System.Drawing.Color ProvidersBorderBrush
#endif
#if WPF  || WINUI || SMARTMAUI
        private Brush providersborderDefault = new SolidColorBrush(Colors.Transparent);
        public Brush ProvidersBorderBrush
#endif
#if ANDROIDX
        private Android.Graphics.Color providersborderDefault = Android.Graphics.Color.Transparent;
        public Android.Graphics.Color ProvidersBorderBrush
#endif
#if WINFORMS || WPF || WINUI  || ANDROIDX || SMARTMAUI
        {
            get
            {
                return providersborderDefault;
            }
            set
            {
                if (providersborderDefault != value)
                {
                    providersborderDefault = value;
                    this.OnPropertyChanged(nameof(ProvidersBorderBrush));
                }
            }
        }
#endif
#if WPF  || WINUI || SMARTMAUI
        private Thickness providersborderthicknessDefault;
        public Thickness ProvidersBorderThickness
        {
            get
            {
                return providersborderthicknessDefault;
            }
            set
            {
                if (providersborderthicknessDefault != value)
                {
                    providersborderthicknessDefault = value;
                    this.OnPropertyChanged(nameof(ProvidersBorderThickness));
                }
            }
        }
#endif

#if WINFORMS
        private System.Drawing.Color accountsborderDefault = System.Drawing.Color.Transparent;
        public System.Drawing.Color AccountsBorderBrush
#endif
#if WPF  || WINUI || SMARTMAUI
        private Brush accountsborderDefault = new SolidColorBrush(Colors.Transparent);
        public Brush AccountsBorderBrush
#endif
#if ANDROIDX
        private Android.Graphics.Color accountsborderDefault = Android.Graphics.Color.Transparent;
        public Android.Graphics.Color AccountsBorderBrush
#endif
#if WINFORMS || WPF || WINUI  || ANDROIDX || SMARTMAUI
        {
            get
            {
                return accountsborderDefault;
            }
            set
            {
                if (accountsborderDefault != value)
                {
                    accountsborderDefault = value;
                    this.OnPropertyChanged(nameof(AccountsBorderBrush));
                }
            }
        }
#endif
#if WPF  || WINUI || SMARTMAUI
        private Thickness accountsborderthicknessDefault;
        public Thickness AccountsBorderThickness
        {
            get
            {
                return accountsborderthicknessDefault;
            }
            set
            {
                if (accountsborderthicknessDefault != value)
                {
                    accountsborderthicknessDefault = value;
                    this.OnPropertyChanged(nameof(AccountsBorderThickness));
                }
            }
        }
#endif

#if WINFORMS
        private System.Drawing.Color transactionsborderDefault = System.Drawing.Color.Transparent;
        public System.Drawing.Color TransactionsBorderBrush
#endif
#if WPF  || WINUI || SMARTMAUI
        private Brush transactionsborderDefault = new SolidColorBrush(Colors.Transparent);
        public Brush TransactionsBorderBrush
#endif
#if ANDROIDX
        private Android.Graphics.Color transactionsborderDefault = Android.Graphics.Color.Transparent;
        public Android.Graphics.Color TransactionsBorderBrush
#endif
#if WINFORMS || WPF || WINUI  || ANDROIDX || SMARTMAUI
        {
            get
            {
                return transactionsborderDefault;
            }
            set
            {
                if (transactionsborderDefault != value)
                {
                    transactionsborderDefault = value;
                    this.OnPropertyChanged(nameof(TransactionsBorderBrush));
                }
            }
        }
#endif
#if WPF  || WINUI || SMARTMAUI
        private Thickness transactionsborderthicknessDefault;
        public Thickness TransactionsBorderThickness
        {
            get
            {
                return transactionsborderthicknessDefault;
            }
            set
            {
                if (transactionsborderthicknessDefault != value)
                {
                    transactionsborderthicknessDefault = value;
                    this.OnPropertyChanged(nameof(TransactionsBorderThickness));
                }
            }
        }
#endif

#if !DBSERVER
        private bool categoriesenabled = false;
        public bool CategoriesEnabled
        {
            get
            {
                return categoriesenabled;
            }
            set
            {
                if (categoriesenabled != value)
                {
                    categoriesenabled = value;
                    this.OnPropertyChanged(nameof(CategoriesEnabled));
                }
            }
        }

        private bool bankschecked = false;
        public bool BanksChecked
        {
            get
            {
                return bankschecked;
            }
            set
            {
                if (bankschecked != value)
                {
                    bankschecked = value;
                    this.OnPropertyChanged(nameof(BanksChecked));
                }
            }
        }

        private bool savingschecked = false;
        public bool SavingsChecked
        {
            get
            {
                return savingschecked;
            }
            set
            {
                if (savingschecked != value)
                {
                    savingschecked = value;
                    this.OnPropertyChanged(nameof(SavingsChecked));
                }
            }
        }

        private bool investmentschecked = false;
        public bool InvestmentsChecked
        {
            get
            {
                return investmentschecked;
            }
            set
            {
                if (investmentschecked == value)
                {
                    return;
                }
                investmentschecked = value;
                this.OnPropertyChanged(nameof(InvestmentsChecked));
            }
        }

        private bool cryptoschecked = false;
        public bool CryptosChecked
        {
            get
            {
                return cryptoschecked;
            }
            set
            {
                if (cryptoschecked == value)
                {
                    return;
                }
                cryptoschecked = value;
                this.OnPropertyChanged(nameof(CryptosChecked));
            }
        }
        public string ButtonBanks   // Its 'public' so Tag binding works
        {
            get
            {
                return SmartParametersV2016.Banks.ToString();
            }
        }
        public string ButtonSavings
        {
            get
            {
                return SmartParametersV2016.Savings.ToString();
            }
        }
        public string ButtonInvestments
        {
            get
            {
                return SmartParametersV2016.Investments.ToString();
            }
        }
        public string ButtonCryptos
        {
            get
            {
                return SmartParametersV2016.Cryptos.ToString();
            }
        }
#endif

#if WINFORMS
        private Color autoswitchfinanceDefault = Color.DarkRed;
        public Color AutoSwitchFinanceColour
#endif
#if WPF  || WINUI
        private Brush autoswitchfinanceDefault = new SolidColorBrush(Colors.DarkRed);
        public Brush AutoSwitchFinanceColour
#endif
#if ANDROIDX
        private Color autoswitchfinanceDefault = Color.DarkRed;
        public Color AutoSwitchFinanceColour
#endif
#if SMARTMAUI
        private Color autoswitchfinanceDefault = Colors.DarkRed;
        public Color AutoSwitchFinanceColour
#endif
#if WINFORMS || WPF  || WINUI || ANDROIDX || SMARTMAUI
        {
            get
            {
                return autoswitchfinanceDefault;
            }
            set
            {
                if (autoswitchfinanceDefault != value)
                {
                    autoswitchfinanceDefault = value;
                    this.OnPropertyChanged(nameof(AutoSwitchFinanceColour));
                }
            }
        }
#endif

#if WINFORMS || WPF  || WINUI || ANDROIDX || SMARTMAUI

        private string dateformatDefault = "d";
        public string DateFormat
        {
            get
            {
                return dateformatDefault;
            }
            set
            {
                if (dateformatDefault != value)
                {
                    dateformatDefault = value;
                    this.OnPropertyChanged(nameof(DateFormat));
                }
            }
        }

        private bool ukischeckedDefault;
        public bool UKIsChecked
        {
            get
            {
                return ukischeckedDefault;
            }
            set
            {
                if (ukischeckedDefault != value)
                {
                    ukischeckedDefault = value;
                    this.OnPropertyChanged(nameof(UKIsChecked));
                }
            }
        }

        private bool usischeckedDefault = false;
        public bool USIsChecked
        {
            get
            {
                return usischeckedDefault;
            }
            set
            {
                if (usischeckedDefault != value)
                {
                    usischeckedDefault = value;
                    this.OnPropertyChanged(nameof(USIsChecked));
                }
            }
        }
#endif

#if WINFORMS || WPF  || WINUI || SMARTMAUI
#if WINFORMS
        private DateTime startdateDefault = SmartParametersV2016.defaultDate;
        public DateTime StartDate
#endif
#if WPF
        // Careful!  These two were DateTimeOffset!!!
        private DateTime startdateDefault = SmartParametersV2016.defaultDate;
        public DateTime StartDate
#endif
#if WINUI
        private DateTimeOffset startdateDefault = SmartParametersV2016.defaultDate;
        public DateTimeOffset StartDate
#endif
#if SMARTMAUI
        private DateTime startdateDefault = SmartParametersV2016.defaultDate;
        public DateTime StartDate
#endif
        {
            get
            {
                return startdateDefault;
            }
            set
            {
                if (startdateDefault != value)
                {
                    startdateDefault = value;
                    this.OnPropertyChanged(nameof(StartDate));
                }
            }
        }
#endif

#if WINFORMS || WPF  || WINUI || ANDROIDX || SMARTMAUI

        private bool startdateenabledDefault = false;
        public bool StartDateEnabled
        {
            get
            {
                return startdateenabledDefault;
            }
            set
            {
                if (startdateenabledDefault != value)
                {
                    startdateenabledDefault = value;
                    this.OnPropertyChanged(nameof(StartDateEnabled));
                }
            }
        }

        private bool resetdatesenabledDefault = false;
        public bool ResetDatesEnabled
        {
            get
            {
                return resetdatesenabledDefault;
            }
            set
            {
                if (resetdatesenabledDefault != value)
                {
                    resetdatesenabledDefault = value;
                    this.OnPropertyChanged(nameof(ResetDatesEnabled));
                }
            }
        }
#endif

#if WINFORMS
        private System.Windows.Forms.Button resetdatesDefault { get; set; }
        public System.Windows.Forms.Button ResetDatesButton
#endif
#if WPF  || WINUI || SMARTMAUI
        private Button resetdatesDefault { get; set; }
        public Button ResetDatesButton
#endif
#if ANDROIDX
        private Button resetdatesDefault { get; set; }
        public Button ResetDatesButton
#endif
#if WINFORMS || WPF  || WINUI || ANDROIDX || SMARTMAUI
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
                    this.OnPropertyChanged(nameof(ResetDatesButton));
                }
            }
        }
#endif


#if WINFORMS || WPF  || WINUI || SMARTMAUI
#if WINFORMS
        private DateTime enddateDefault = SmartParametersV2016.defaultMaxdate;
        public DateTime EndDate
#endif
#if WPF
        // Careful!  These two were DateTimeOffset!!!
        private DateTime enddateDefault = SmartParametersV2016.defaultMaxdate;
        public DateTime EndDate
#endif
#if WINUI
        private DateTimeOffset enddateDefault = SmartParametersV2016.defaultMaxdate;
        public DateTimeOffset EndDate
#endif
#if SMARTMAUI
        private DateTime enddateDefault = SmartParametersV2016.defaultMaxdate;
        public DateTime EndDate
#endif
        {
            get
            {
                return enddateDefault;
            }
            set
            {
                if (enddateDefault != value)
                {
                    enddateDefault = value;
                    this.OnPropertyChanged(nameof(EndDate));
                }
            }
        }
#endif

        private bool enddateenabledDefault = false;
        public bool EndDateEnabled
        {
            get
            {
                return enddateenabledDefault;
            }
            set
            {
                if (enddateenabledDefault != value)
                {
                    enddateenabledDefault = value;
                    this.OnPropertyChanged(nameof(EndDateEnabled));
                }
            }
        }

#if WINFORMS || WPF  || WINUI || SMARTMAUI
#if WINFORMS
        private string resultstartdateDefault = "";
        public string ResultStartDate
#endif
#if WPF
        // Careful!  These two were DateTimeOffset!!!
        private string resultstartdateDefault = "";
        public string ResultStartDate
#endif
#if WINUI || SMARTMAUI
        private string resultstartdateDefault = "";
        public string ResultStartDate
#endif
        {
            get
            {
                return resultstartdateDefault;
            }
            set
            {
                if (resultstartdateDefault != value)
                {
                    resultstartdateDefault = value;
                    this.OnPropertyChanged(nameof(ResultStartDate));
                }
            }
        }
#endif

#if WINFORMS || WPF  || WINUI || SMARTMAUI
#if WINFORMS
        private string resultenddateDefault = "";
        public string ResultEndDate
#endif
#if WPF
        // Careful!  These two were DateTimeOffset!!!
        private string resultenddateDefault = "";
        public string ResultEndDate
#endif
#if WINUI || SMARTMAUI
        private string resultenddateDefault = "";
        public string ResultEndDate
#endif
        {
            get
            {
                return resultenddateDefault;
            }
            set
            {
                if (resultenddateDefault != value)
                {
                    resultenddateDefault = value;
                    this.OnPropertyChanged(nameof(ResultEndDate));
                }
            }
        }
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
                    this.OnPropertyChanged(nameof(PostCode));
                }
            }
        }
#endif

#if ANDROIDX
        internal TextView PostCode { get; set; }
#endif

        private int paidinfontsize = 18;
        public int PaidInFontSize

        {
            get
            {
                return paidinfontsize;
            }
            set
            {
                if (paidinfontsize != value)
                {
                    paidinfontsize = value;
                    this.OnPropertyChanged(nameof(PaidInFontSize));
                }
            }
        }

        private int paidoutfontsize = 18;
        public int PaidOutFontSize

        {
            get
            {
                return paidoutfontsize;
            }
            set
            {
                if (paidoutfontsize != value)
                {
                    paidoutfontsize = value;
                    this.OnPropertyChanged(nameof(PaidOutFontSize));
                }
            }
        }

        private int differencefontsize = 18;
        public int DifferenceFontSize

        {
            get
            {
                return differencefontsize;
            }
            set
            {
                if (differencefontsize != value)
                {
                    differencefontsize = value;
                    this.OnPropertyChanged(nameof(DifferenceFontSize));
                }
            }
        }

#if WINFORMS || WPF  || WINUI || SMARTMAUI
        private string totaltransDefault = "";
        public string TotalTransactions
        {
            get
            {
                return totaltransDefault;
            }
            set
            {
                if (totaltransDefault != value)
                {
                    totaltransDefault = value;
                    this.OnPropertyChanged(nameof(TotalTransactions));
                }
            }
        }
#endif

#if ANDROIDX
        internal TextView TotalTransactions { get; set; }
#endif

#if WINFORMS || WPF  || WINUI || SMARTMAUI
        private string totalvalueDefault = "";
        public string TotalValue
        {
            get
            {

                return totalvalueDefault;
            }
            set
            {
                if (totalvalueDefault != value)
                {
                    totalvalueDefault = value;
                    this.OnPropertyChanged(nameof(TotalValue));
                }
            }
        }
#endif

#if ANDROIDX
        internal TextView TotalValue { get; set; }
#endif


#if WINFORMS
        private bool ledswitchedDefault = false;
        public bool LedSwitched
#endif
#if WPF  || WINUI || SMARTMAUI
#if WPF
        private Visibility ledswitchedDefault = Visibility.Hidden;
#endif
#if WINUI || SMARTMAUI
        private Visibility ledswitchedDefault = Visibility.Collapsed;
#endif
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
                    this.OnPropertyChanged(nameof(LedSwitched));
                }
            }
        }
#endif

#if ANDROIDX
        internal TextView LedSwitched { get; set; }
#endif



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
                    this.OnPropertyChanged(nameof(AreaId));
                }
            }
        }
#endif

#if ANDROIDX
        internal TextView AreaId { get; set; }
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
                    this.OnPropertyChanged(nameof(AreaIdBackground));
                }
            }
        }

#if WINFORMS
        private System.Drawing.Image pictureboxDefault; //"keasdon_energy_small.jpg";
        public System.Drawing.Image PictureBoxLOGO
#endif
#if WPF  || WINUI || SMARTMAUI
        private ImageSource pictureboxDefault;
        public ImageSource PictureBoxLOGO
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
                    this.OnPropertyChanged(nameof(PictureBoxLOGO));
                }
            }
        }
#endif

#if ANDROIDX
        private ImageView pictureboxLogoDefault;
        public ImageView PictureBoxLOGO
        {
            get
            {
                return pictureboxLogoDefault;
            }
            set
            {
                if (pictureboxLogoDefault != value)
                {
                    pictureboxLogoDefault = value;
                    this.OnPropertyChanged(nameof(PictureBoxLOGO));
                }
            }
        }
#endif

        private string statusmessage = "";
        public string StatusMessage
        {
            get
            {
                return statusmessage;
            }
            set
            {
                if (statusmessage != value)
                {
                    statusmessage = value;
                    this.OnPropertyChanged(nameof(StatusMessage));
                }
            }
        }

        private string financeexchangeratestitle = "";
        public string FinanceExchangeRatesTitle
        {
            get
            {
                return financeexchangeratestitle;
            }
            set
            {
                if (financeexchangeratestitle != value)
                {

                    financeexchangeratestitle = value;
                    this.OnPropertyChanged(nameof(FinanceExchangeRatesTitle));
                }
            }
        }

        private string financeexchangeratesinfo = "";
        public string FinanceExchangeRatesInfo
        {
            get
            {
                return financeexchangeratesinfo;
            }
            set
            {
                if (financeexchangeratesinfo != value)
                {

                    financeexchangeratesinfo = value;
                    this.OnPropertyChanged(nameof(FinanceExchangeRatesInfo));
                }
            }
        }

#if WPF  || WINUI || SMARTMAUI
        public double PopupHeight { get; set; }
        public double PopupWidth { get; set; }
#endif


#if SMARTMAUI

        public ISeries[] ChartSeries { get; set; }

        public Axis[] XAxes { get; set; }

        public Axis[] YAxes { get; set; }

        
        private bool webviewdefault = false;
        public bool WebViewVisible
        {
            get
            {
                return webviewdefault;
            }
            set
            {
                if (webviewdefault != value)
                {
                    webviewdefault = value;
                    this.OnPropertyChanged(nameof(WebViewVisible));
                }
            }
        }

        private bool cryptosdefault = false;
        public bool CryptosVisible
        {
            get
            {
                return cryptosdefault;
            }
            set
            {
                if (cryptosdefault != value)
                {
                    cryptosdefault = value;
                    this.OnPropertyChanged(nameof(CryptosVisible));
                }
            }
        }
        private bool debitscreditsdefault = false;
        public bool DebitsCreditsVisible
        {
            get
            {
                return debitscreditsdefault;
            }
            set
            {
                if (debitscreditsdefault != value)
                {
                    debitscreditsdefault = value;
                    this.OnPropertyChanged(nameof(DebitsCreditsVisible));
                }
            }
        }

        private bool graphsdefault = false;
        public bool GraphsVisible
        {
            get
            {
                return graphsdefault;
            }
            set
            {
                if (graphsdefault != value)
                {
                    graphsdefault = value;
                    this.OnPropertyChanged(nameof(GraphsVisible));
                }
            }
        }
#endif

#if WINFORMS || WPF  || WINUI || ANDROIDX
        private PlotModel plotmodel2 = null;
        public PlotModel PlotModel2
        {
            get
            {
                return plotmodel2;
            }
            set
            {
                if (plotmodel2 != value)
                {
                    plotmodel2 = value;
                    this.OnPropertyChanged(nameof(PlotModel2));
                }
            }
        }

        private string financechartstitle = "";
        public string FinanceChartsTitle
        {
            get
            {
                return financechartstitle;
            }
            set
            {
                if (financechartstitle != value)
                {

                    financechartstitle = value;
                    this.OnPropertyChanged(nameof(FinanceChartsTitle));
                }
            }
        }

        private string financechartsplottitle = "";
        public string FinanceChartsPlotTitle
        {
            get
            {
                return financechartsplottitle;
            }
            set
            {
                if (financechartsplottitle != value)
                {

                    financechartsplottitle = value;
                    this.OnPropertyChanged(nameof(FinanceChartsPlotTitle));
                }
            }
        }
#endif
        internal string udprn = "";
        internal bool[] autoswitchToggle = new bool[3] { false, false, false };
        internal StringBuilder bollocks { get; set; }

        // NW
        internal int[] zzz = new int[3] { 0, 0, 0 };

        // Spike
        internal string Client_Id = "";
        internal string Client_Secret = "";
        // Crypto ends
    }
}