#if WINFORMS
using System.Net.Http;
using System.Windows;
using System.Windows.Input;
#endif


#if WPF
using System.ComponentModel;
using System.IO;
using System.Net.Http;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Linq;
using MoreLinq;
#endif

namespace SmartCubeMobile
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public class CryptoBollocksV2025
    {
        internal static string wherewereat = "";

        private static readonly HttpClient client = new HttpClient();
        internal static async Task<bool> CryptoMate(//object sender, RoutedEventArgs e,
#if WINFORMS
                                                MainMeter components,
                                                //System.Windows.Forms.RichTextBox textBoxConsole,
#endif
                                                MainViewModel ourviewmodel,
                                                FinanceViewModel financeviewmodel,
                                                char categoryCode,
                                                string start_url,
                                                short institutionCode,
                                                short brandCode,
                                                List<SmartFinance.Transaction_Types> transaction_types_found,
                                                List<SmartFinance.DanApiKey> apikeys)
                                                
        {
            switch (brandCode)
            {
                case 20001:
                    // Coinbase
                    
                    financeviewmodel.v2accountsList = new List<SmartFinance.V2Accounts>();
                    financeviewmodel.v3accountsList = new List<SmartFinance.V3Accounts>();
                    financeviewmodel.v2addressesList = new List<SmartFinance.V2Addresses>();
                    financeviewmodel.v2transactionsList = new List<SmartFinance.V2Transactions>();
                    financeviewmodel.v2currenciesList = new List<SmartFinance.V2Currencies>();
                    financeviewmodel.v2exchangeratesList = new List<SmartFinance.V2ExchangeRates>();


                    //herewereat = "GetGoing";
                    //financeviewmodel.TransActivityEnabled = 
                    //financeviewmodel.AcctsActivityEnabled = false;

                    if (!await User_Click(
#if WINFORMS
                        components,
                        //textBoxConsole,
#endif
                                ourviewmodel,
                                financeviewmodel,
                                categoryCode,
                                institutionCode,
                                brandCode,
                                apikeys,
                                transaction_types_found))
                    {
                        Console.WriteLine("Failure");
                        return false;
                    }
                    else
                    {

                    }
                    break;
                default:
                    break;
            }
            return true;
        }

        internal static async Task<bool> User_Click(
#if WINFORMS
                                        MainMeter components,
                                        //System.Windows.Forms.RichTextBox textBoxConsole,
#endif
                                        MainViewModel ourviewmodel,
                                        FinanceViewModel financeviewmodel,
                                        char categoryCode,
                                        short institutionCode, 
                                        short brandCode,
                                        List<SmartFinance.DanApiKey> apikeys,
                                        List<SmartFinance.Transaction_Types> transaction_types_found)
        {
            wherewereat = "User clicked";
            //string brokerageaccountsEndpoint = "api.coinbase.com/api/v3/brokerage/accounts";

            string V3accountsEndpoint = "api.coinbase.com/api/v3/brokerage/accounts";
            string V2accountsEndpoint = "api.coinbase.com/v2/accounts";
            string V2userEndpoint = "api.coinbase.com/v2/user";

            financeviewmodel.cryptouserV2 = new SmartFinance.V2User();

            foreach (SmartFinance.DanApiKey danApiKey in apikeys)
            {
                // The way this works is that for every set of API keys, I get
                // a series of Accounts, and for each Account in THAT series
                // I get a list of Transactions.
                // So every time I start an Account in the series, I need to 'add'
                // in that Account and 'add' in the Transactions it finds
                // What's complicated about that???
                if (danApiKey.Username == ourviewmodel.UserName)
                {
                    if (danApiKey.ApiKey != null &&
                        danApiKey.ApiSecret != null)
                    {
                        financeviewmodel.v2userList.Clear();
                        string cbPrivateKey = SmartFinanceV2021.parseKey(danApiKey.ApiSecret);
                        // Get the User info
                        string userToken = SmartFinanceV2021.generateToken(financeviewmodel, danApiKey.ApiKey, cbPrivateKey, $"GET {V2userEndpoint}");
                        string userData = await SmartFinanceV2021.CallApiGET($"https://{V2userEndpoint}", userToken, financeviewmodel.cbversion);
                        SmartFinanceV2021.AnalyzeUserV2(userData, financeviewmodel.v2userList, danApiKey.Username, danApiKey.AccessId);
                        if (financeviewmodel.v2userList.Count == 1)
                        {
                            if (financeviewmodel.v2userList[0].EMAIL != danApiKey.AccessId)
                            {
                                Console.WriteLine("No email match");
                                return false;
                            }
                        }
                        else
                        {
                            Console.WriteLine("Lookup user failed");
                            return false;
                        }
                        wherewereat = danApiKey.AccessId;

                        // Get the Accounts for this User and AccessId
                        string accountToken = SmartFinanceV2021.generateToken(financeviewmodel, danApiKey.ApiKey, cbPrivateKey, $"GET {V3accountsEndpoint}");
                        string accountsData = await SmartFinanceV2021.CallApiGET($"https://{V3accountsEndpoint}", accountToken, financeviewmodel.cbversion);
                        SmartFinanceV2021.AnalyzeAccountV3(accountsData, financeviewmodel.v3accountsList, danApiKey.Username, danApiKey.Exchange);

                        wherewereat = "Get accounts";

                        foreach (SmartFinance.V3Accounts account in financeviewmodel.v3accountsList)
                        {
                            // Addresses
                            string addressesEndpoint = V2accountsEndpoint + "/" + account.UUID + "/addresses";

                            string addressesToken = SmartFinanceV2021.generateToken(financeviewmodel, danApiKey.ApiKey, cbPrivateKey, $"GET {addressesEndpoint}");
                            string addressesData = await SmartFinanceV2021.CallApiGET($"https://{addressesEndpoint}", addressesToken, financeviewmodel.cbversion);
                            SmartFinanceV2021.AnalyzeAddressV2(addressesData, financeviewmodel.v2addressesList, danApiKey.Username, danApiKey.Exchange, account.UUID);


                            // Transactions
                            string transactionsEndpoint = V2accountsEndpoint + "/" + account.UUID + "/transactions";

                            string transactionsToken = SmartFinanceV2021.generateToken(financeviewmodel, danApiKey.ApiKey, cbPrivateKey, $"GET {transactionsEndpoint}");
                            string transactionsData = await SmartFinanceV2021.CallApiGET($"https://{transactionsEndpoint}", transactionsToken, financeviewmodel.cbversion);
                            SmartFinanceV2021.AnalyzeTransactionV2(transactionsData, financeviewmodel.v2transactionsList, danApiKey.Username, danApiKey.Exchange, account.UUID);

                            // Add in this Account
                            financeviewmodel.v3tempaccountsList.Add(account);
                        }
                        financeviewmodel.v3accountsList.Clear();
                    }

                    // Do PLO addresses
                    wherewereat = "Addresses";
                    SmartFinanceV2021.RebuildAddresses(ourviewmodel, danApiKey);
                    wherewereat = "Transactions";
                    await SmartFinanceV2021.RebuildTransactions(ourviewmodel,
                                                            financeviewmodel,
                                                            categoryCode,
                                                            institutionCode,
                                                            brandCode,
                                                            financeviewmodel.v2userList[0],
                                                            danApiKey.Exchange,
                                                            danApiKey.AccessId,
                                                            transaction_types_found);
                }
            }
            wherewereat = "Rebuilding";

            // Accounts
            SmartFinanceV2021.RebuildCryptoAccounts(financeviewmodel);

            // Addresses
            SmartFinanceV2021.RebuildCryptoAddresses(financeviewmodel);

            // Get the exchange rates
            if (await ExtractExchangeRates(financeviewmodel))

            {
                // Build the Transaction list and the Currencies list
                SmartFinanceV2021.RebuildCryptoTransactions(financeviewmodel);
            }

            financeviewmodel.rateEnabled = true;

            // SmartSwitch Addresses
            SmartFinanceV2021.RebuildCategories(ourviewmodel,
                                                           financeviewmodel,
                                                           categoryCode);

            SmartFinanceV2021.RebuildCategoryTypes(ourviewmodel,
                                                           financeviewmodel,
                                                           categoryCode);
            SmartFinanceV2021.RebuildCategoryTypes(ourviewmodel,
                                                    financeviewmodel,
                                                    categoryCode);

            wherewereat = "PLO Transactions";
            SmartFinanceV2021.RebuildPLOTransactions(financeviewmodel);
            
            financeviewmodel.rateEnabled = true;
            wherewereat = "Finished";
            return true;
        }

        internal void GridViewColumnHeader_Click(object sender, RoutedEventArgs e)
        {
#if WPF
            var headerClicked = e.OriginalSource as GridViewColumnHeader;
            if (headerClicked.Column != null)
            {
                GridViewColumnHeader gvch = headerClicked.Column.Header as GridViewColumnHeader;
                string sortBy = gvch.Content.ToString();
                SmartFinanceV2021.RebuildCryptoTransactions(MainMeter.financeviewmodel, sortBy);
              
            }
#endif
            return;
        }

#if WPF
        internal void TabItemChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender != null &&
                e != null)
            {
                TabControl abc = sender as TabControl;
                if (abc != null)
                {
                    if (abc.IsEnabled)
                    {
                        TabItem titem = abc.SelectedItem as TabItem;
                        string header = titem.Header.ToString();
                        switch (header)
                        {
                            case "Transactions":
                                MainMeter.financeviewmodel.AccountsRibbonVisible = Visibility.Hidden;
                                MainMeter.financeviewmodel.TransactionsRibbonVisible = Visibility.Visible;
                                MainMeter.financeviewmodel.TransActivityEnabled = true;
                                MainMeter.financeviewmodel.AcctsActivityEnabled = false;
                                break;
                            case "Accounts":
                                MainMeter.financeviewmodel.TransactionsRibbonVisible = Visibility.Hidden;
                                MainMeter.financeviewmodel.AccountsRibbonVisible = Visibility.Visible;
                                MainMeter.financeviewmodel.TransActivityEnabled = false;
                                MainMeter.financeviewmodel.AcctsActivityEnabled = true;
                                break;
                            case "Addresses":
                                MainMeter.financeviewmodel.TransactionsRibbonVisible = Visibility.Hidden;
                                MainMeter.financeviewmodel.AccountsRibbonVisible = Visibility.Hidden;
                                MainMeter.financeviewmodel.TransActivityEnabled = false;
                                MainMeter.financeviewmodel.AcctsActivityEnabled = false;
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

        //internal void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{            
        //    if (CryptoTotals.SelectedItem != null)
        //    {
        //        // Retrieve selected item data
        //        SmartFinance.V2CryptoTotals selectedItem = CryptoTotals.SelectedItem as SmartFinance.V2CryptoTotals;
        //        string currency = selectedItem.CRYPTO_CURRENCY.ToString();
        //        string rate = selectedItem.RATE.ToString();

        //        MainMeter.financeviewmodel.FinanceCryptoTransactionsSpecific =
        //            new List<SmartFinance.CryptoTransactionsView>
        //            (from Currencies in MainMeter.financeviewmodel.FinanceCryptoTransactions
        //             where Currencies.CRYPTO_CURRENCY == currency
        //             orderby Currencies.CREATED_AT
        //             select Currencies);
        //        MainMeter.financeviewmodel.PopupCurrency = currency + "(" +
        //            MainMeter.financeviewmodel.FinanceCryptoTransactionsSpecific.Count.ToString() +
        //                                                              ")";                                                            
        //        MainMeter.financeviewmodel.PopupRate = "Rate now: " + Convert.ToDouble(rate).ToString("F3");

        //        //CurrencyPopup currency_popup = new CurrencyPopup()
        //        //{
        //        //    // Took me ALL DAY to get this fucker working ...
        //        //    PlacementTarget = this,     // components
        //        //    Placement = PlacementMode.Center,
        //        //    IsOpen = true
        //        //};
        //        //e.Handled = true;
        //        //CryptoTotals.SelectedItem = null;
        //    }
        //    return;
        //}

        internal void TransActivityChecked(object sender, EventArgs e)
        {
            if (sender != null &&
                e != null)
            {
                if (MainMeter.financeviewmodel.TransActivityEnabled)
                {
                    CheckBox abc = sender as CheckBox;
                    string tag = abc.Tag.ToString();
                    switch (tag)
                    {
                        case "F":
                            MainMeter.financeviewmodel.transfiat = (bool)(abc.IsChecked);
                            break;
                        case "B":
                            MainMeter.financeviewmodel.buy = (bool)(abc.IsChecked);
                            break;
                        case "S":
                            MainMeter.financeviewmodel.sell = (bool)(abc.IsChecked);
                            break;
                        case "T":
                            MainMeter.financeviewmodel.trade = (bool)(abc.IsChecked);
                            break;
                        case "X":
                            MainMeter.financeviewmodel.send = (bool)(abc.IsChecked);
                            break;
                        default:
                            break;
                    }
                    SmartFinanceV2021.RebuildCryptoTransactions(MainMeter.financeviewmodel);
                }
            }
            return;
        }

        internal void AcctsActivityChecked(object sender, EventArgs e)
        {
            if (sender != null &&
                e != null)
            {
                if (MainMeter.financeviewmodel.AcctsActivityEnabled)
                {
                    CheckBox abc = sender as CheckBox;
                    string tag = abc.Tag.ToString();
                    switch (tag)
                    {
                        case "F":
                            MainMeter.financeviewmodel.acctsfiat = (bool)(abc.IsChecked);
                            break;
                        case "C":
                            MainMeter.financeviewmodel.crypto = (bool)(abc.IsChecked);
                            break;                        
                        default:
                            break;
                    }
                    SmartFinanceV2021.RebuildCryptoAccounts(MainMeter.financeviewmodel);
                }
            }
            return;
        }

        internal static async Task<bool> ExtractExchangeRates(FinanceViewModel financeviewmodel)
        {
            string V2exchangeratesEndpoint = "api.coinbase.com/v2/exchange-rates?currency=" + financeviewmodel.WorkingCurrency;
            string exchangerateData = await SmartFinanceV2021.CallApiGET($"https://{V2exchangeratesEndpoint}", "", "");
            SmartFinanceV2021.AnalyzeExchangeRates(financeviewmodel.WorkingCurrency, exchangerateData, financeviewmodel.v2exchangeratesList);

            return true;
        }
        internal void Leave_Click(object sender, RoutedEventArgs e)
        {
            if (sender != null &&
                e != null)
            {
                MainMeter.financeviewmodel.timerClock.Stop();
                App.Current.Shutdown();
            }
        }

        internal void OnLoaded(object sender, EventArgs e)
        {
            return;
        }

        internal void OnClosed(object sender, EventArgs e)
        {
            return;
        }

#if WPF
        internal static void TransactionsSelectionChanged(object sender, SelectionChangedEventArgs e,
                                                    FinanceViewModel financeviewmodel)
        {
            return;
        }
#endif
        internal static void FinanceTransactionsMouseDoubleClick(object sender, MouseAction e,
                                                    FinanceViewModel financeviewmodel)
        {
            return;
        }
    }
}