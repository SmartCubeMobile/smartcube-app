// https://stackoverflow.com/questions/78797531/cannot-use-javascriptexecutor-in-htmlunit-android-package
using System.Text.RegularExpressions;
using MoreLinq;


#if CRYPTOS
using System.Net.Http;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;
using System.Security.Cryptography;
using Jose;
using System.Text;
#endif

#if WINFORMS
using SmartDashboard;
using System.Windows.Threading;
using Windows.UI.Notifications.Management;
using Windows.UI.Notifications;
using OpenQA.Selenium;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json.Linq;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Jose;
using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.Tokens;
#endif

#if WPF
using Windows.UI.Notifications.Management;
using Windows.UI.Notifications;
using OpenQA.Selenium;
using System.Security.Policy;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using Windows.Media.Protection.PlayReady;
using System.Net.Http.Headers;
//using PythonNetWrapper;
using static SmartCubeMobile.SmartFinance;
using System.IO;
using System.Windows.Controls.Primitives;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows;
using Autofac.Diagnostics;
using Org.BouncyCastle.Crypto.Macs;
using Jose;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Org.BouncyCastle.Crypto;
using Windows.ApplicationModel.VoiceCommands;
#endif

#if UWP || WINUI
using System.Threading.Tasks;
using System.Collections.Generic;
using System;
using System.Threading;
using System.Linq;
using Windows.UI.Core;
using Windows.UI.Notifications.Management;
using Windows.UI.Notifications;
using OpenQA.Selenium;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json.Linq;
using iText.Layout.Splitting;
#endif

#if ANDROID
using Android.Content;
using Android.Provider;
//using OpenQA.Selenium;
#endif


namespace SmartCubeMobile
{
    public class CryptoBollocksV2024
    {
        private static readonly HttpClient client = new HttpClient();
        internal static async Task<bool> CryptoMate(//object sender, RoutedEventArgs e,
#if WINFORMS
                                                MainProcess components,
                                                RichTextBox textBoxConsole,
#endif
                                                MainViewModel ourviewmodel,
                                                FinanceViewModel financeviewmodel,
                                                char categoryCode,
                                                string start_url,
                                                short institution_code,
                                                short brand_code,
                                                List<SmartFinance.Transaction_Types> transaction_types_found,
                                                string AccessId,
                                                string Exchange,
                                                string ApiKey,
                                                string ApiSecret)
        {
            switch (brand_code)
            {
                case 20001:
                    // Coinbase
                    // 
                    //List<SmartFinance.DanApiKey> apikeys = new List<SmartFinance.DanApiKey>();
                    SmartFinance.DanApiKey danApiKey = new SmartFinance.DanApiKey();
                    danApiKey.Username = ourviewmodel.UserName;
                    danApiKey.Exchange = Exchange;
                    danApiKey.AccessId = AccessId;
                    danApiKey.ApiKey = ApiKey;
                    danApiKey.ApiSecret = ApiSecret;
                    //apikeys.Add(danApiKey);

                    financeviewmodel.v2accountsList = new List<SmartCrypto.V2Accounts>();
                    financeviewmodel.v3accountsList = new List<SmartCrypto.V3Accounts>();
                    financeviewmodel.v2addressesList = new List<SmartCrypto.V2Addresses>();
                    financeviewmodel.v2transactionsList = new List<SmartCrypto.V2Transactions>();
#if !SMARTMAUI
                    financeviewmodel.v2currenciesList = new List<SmartCrypto.V2Currencies>();
#endif
                    financeviewmodel.v2exchangeratesList = new List<SmartCrypto.V2ExchangeRates>();

                    if (!await User_Click(
#if WINFORMS
                                    components,
                                    textBoxConsole,
#endif
                                    ourviewmodel,
                                    financeviewmodel,
                                    categoryCode,
                                    institution_code,
                                    brand_code,
                                    danApiKey))
                    {
                        Console.WriteLine("Failure");
                        return false;
                    }
                    else
                    {
                        if (!await CryptoBollocksV2024.PostProcess(
#if WINFORMS
                                                        components, 
                                                        textBoxConsole,
#endif
                                                        ourviewmodel,
                                                        financeviewmodel,
                                                        categoryCode,
                                                        institution_code,
                                                        brand_code,
                                                        danApiKey,
                                                        transaction_types_found))
                        {
                            Console.WriteLine("Crypto post processing failed");
                            return false;
                        }
                    }
                    break;
                case 20002:
                    // Avalanche
                    break;
                case 20003:
                    // Binance
                    break;
                default:
                    break;
            }
            return true;
        }

        internal static async Task<bool> User_Click(
#if WINFORMS
                                            MainProcess components,
                                            System.Windows.Forms.RichTextBox textBoxConsole,
#endif

                                            MainViewModel ourviewmodel,
                                            FinanceViewModel financeviewmodel,
                                            char categoryCode,
                                            short institution_code,
                                            short brand_code,
                                            SmartFinance.DanApiKey danApiKey)
        {
            //string brokerageaccountsEndpoint = "api.coinbase.com/api/v3/brokerage/accounts";

            string V3accountsEndpoint = "api.coinbase.com/api/v3/brokerage/accounts";
            string V2accountsEndpoint = "api.coinbase.com/v2/accounts";
            string V2userEndpoint = "api.coinbase.com/v2/user";

            financeviewmodel.cryptouserV2 = new SmartCrypto.V2User();

            //foreach (SmartFinance.DanApiKey apikey in apikeys)
            //{
            // The way this works is that for every set of API keys, I get
            // a series of Accounts, and for each Account in THAT series
            // I get a list of Transactions.
            // So every time I start an Account in the series, I need to 'add'
            // in that Account and 'add' in the Transactions it finds
            // What's complicated about that???
            // ***Fucking EVERYTHING***
            if (danApiKey.Username == ourviewmodel.UserName)
            {
                if (danApiKey.ApiKey != null &&
                    danApiKey.ApiSecret != null)
                {
                    string cbPrivateKey = parseKey(danApiKey.ApiSecret);
                    // Get the User info
                    string userToken = GenerateToken(financeviewmodel, danApiKey.ApiKey, cbPrivateKey, $"GET {V2userEndpoint}");
                    if (await CallApiGET(financeviewmodel, $"https://{V2userEndpoint}", userToken, financeviewmodel.cbversion))
                    {
                        string userData = financeviewmodel.result;
                        financeviewmodel.cryptouserV2 = AnalyzeUserV2(userData, danApiKey.Username, danApiKey.AccessId);
                        if (financeviewmodel.cryptouserV2.EMAIL != danApiKey.AccessId)
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

                    // Get the V2Accounts for this User and AccessId
                    string V2accountToken = GenerateToken(financeviewmodel, danApiKey.ApiKey, cbPrivateKey, $"GET {V2accountsEndpoint}");
                    if (await CallApiGET(financeviewmodel, $"https://{V2accountsEndpoint}", V2accountToken, financeviewmodel.cbversion))
                    {
                        string accountsData = financeviewmodel.result;

                        financeviewmodel.v2accountsList = AnalyzeAccountV2(accountsData, danApiKey.Username, danApiKey.Exchange);
                    }
                    else
                    {
                        Console.WriteLine("V2Account ApiGET failed");
                        return false;
                    }


                    // Get the V3Accounts for this User and AccessId
                    string V3accountToken = GenerateToken(financeviewmodel, danApiKey.ApiKey, cbPrivateKey, $"GET {V3accountsEndpoint}");
                    if (await CallApiGET(financeviewmodel, $"https://{V3accountsEndpoint}", V3accountToken, financeviewmodel.cbversion))
                    {
                        string accountsData = financeviewmodel.result;

                        financeviewmodel.v3accountsList = AnalyzeAccountV3(accountsData, danApiKey.Username, danApiKey.Exchange);
                    }
                    else
                    {
                        Console.WriteLine("V2Account ApiGET failed");
                        return false;
                    }
                    foreach (SmartCrypto.V3Accounts account in financeviewmodel.v3accountsList)
                    {
                        // Addresses
                        string addressesEndpoint = V2accountsEndpoint + "/" + account.UUID + "/addresses";

                        string addressesToken = GenerateToken(financeviewmodel, danApiKey.ApiKey, cbPrivateKey, $"GET {addressesEndpoint}");
                        if (await CallApiGET(financeviewmodel, $"https://{addressesEndpoint}", addressesToken, financeviewmodel.cbversion))
                        {
                            string addressesData = financeviewmodel.result;

                            AnalyzeAddressV2(addressesData, financeviewmodel.v2addressesList, danApiKey.Username, danApiKey.Exchange, account.UUID);
                        }
                        else
                        {
                            Console.WriteLine("Address ApiGET failed");
                            return false;
                        }

                        // Transactions
                        string transactionsEndpoint = V2accountsEndpoint + "/" + account.UUID + "/transactions";
                        string transactionsToken = GenerateToken(financeviewmodel, danApiKey.ApiKey, cbPrivateKey, $"GET {transactionsEndpoint}");
                        if (await CallApiGET(financeviewmodel, $"https://{transactionsEndpoint}", transactionsToken, financeviewmodel.cbversion))
                        {
                            string transactionsData = financeviewmodel.result;
                            AnalyzeTransactionV2(transactionsData, financeviewmodel, danApiKey.Username, danApiKey.Exchange, account.UUID, account.NAME);
                            // Add in this Account
                            //financeviewmodel.V2accountsList.Add(account);
                        }
                        else
                        {
                            Console.WriteLine("Transaction ApiGET failed");
                            return false;
                        }
                    }
                    //financeviewmodel.V3accountsList.Clear();
                    Console.WriteLine(financeviewmodel.v2transactionsList.Count);
                }
            }
            return true; ;
        }
                
        internal static async Task<bool> PostProcess(
#if WINFORMS
                                                    MainProcess components,
                                                    RichTextBox textBoxConsole,
#endif
                                                    MainViewModel ourviewmodel,
                                                    FinanceViewModel financeviewmodel,
                                                    char categoryCode,
                                                    short institutionCode,
                                                    short brandCode,
                                                    SmartFinance.DanApiKey danApiKey,
                                                    List<SmartFinance.Transaction_Types> transaction_types_found)
        {
            // Addresses
#if !SMARTMAUI
            RebuildAddressesNew(ourviewmodel, financeviewmodel, danApiKey);
#endif

            // Categories
            RebuildCategories(ourviewmodel, financeviewmodel, categoryCode);

            // CategoryTypes
            RebuildCategoryTypes(ourviewmodel, financeviewmodel, categoryCode);
            
            // Accounts
            RebuildAccounts(financeviewmodel);
            
#if !SMARTMAUI
            foreach (SmartFinance.AccountsView accountView in financeviewmodel.FinanceCryptoAccounts)
            {
                // 0 = sortcode
                // 1 = account_no
                // 2 = udprn
                // 3 = currency
                short currencyOrdinal = SmartSpikeV2017.Lookup_Currency_OrdinalOld(ourviewmodel.currenciesList, accountView.CURRENCY);
                if (currencyOrdinal == 0)
                {

#if WPF || UWP || WINUI
                        await SmartBobV2017.ListenerAsync(ourviewmodel, financeviewmodel.financeToken, institutionCode, brandCode, "Crypto Ordinal Lookup", "Cant find: " + accountView.CURRENCY);
#endif
#if WINFORMS
                    await Task.Run(() => components.BeginInvoke(() => MainProcess.Output_Message(textBoxConsole, "Crypto Ordinal Lookup - Cant find: " + accountView.CURRENCY, false, false)));
#endif
                    //#if ANDROID
                    //                            await SmartBobV2017.ListenerAsync(ourviewmodel, financeviewmodel.financeToken, institution_code, brand_code, "Crypto ordinal Lookup", "Trans type  failed: " + transaction_type);
                    //#endif
                    // Carry on
                    return false;
                }

                string vaalue =
                            // Sort Code
                            accountView.EXCHANGE + SmartParametersV2016.fieldSeparator +
                            // Account Title
                            accountView.NAME + SmartParametersV2016.fieldSeparator +
                            // UDPRN
                            danApiKey.AccessId + SmartParametersV2016.fieldSeparator +
                            // Ordinal
                            currencyOrdinal;   // Assume we can find it in Banking
                vaalue = vaalue.Replace("&amp;", "&");                                                                           // Safety check
                string[] itemArray = vaalue.Split(SmartParametersV2016.fieldSeparator);

                financeviewmodel.account_id++;

                FinanceViewModel.AccountItem account_item = new FinanceViewModel.AccountItem
                {
                    AccountID = financeviewmodel.account_id,
                    Content = accountView.NAME,
                    // Here we have [0] = sort_code
                    //              [1] = account_no
                    //              [2] = udprn
                    //              [3] = currency ordinal
                    Value = string.Join(SmartParametersV2016.bar.ToString(), itemArray)
                };
                financeviewmodel.accountItems.Add(account_item);

                // Is it already in our PLO now, do we need to add it to Accounts?
                List<SmartFinance.Accounts> accounts_found =
                    SmartSpikeFinanceV2017.Finance_Lookup_AccountsCategory(ourviewmodel,
                                                                    financeviewmodel,
                                                                    institutionCode,
                                                                    brandCode,
                                                                    categoryCode,
                                                                    accountView.EXCHANGE,
                                                                    accountView.UUID,
                                                                    danApiKey.AccessId,
                                                                    currencyOrdinal);
                if (accounts_found.Count == 0)
                {
                    // Check to see if its in our changeList
                    accounts_found = new List<SmartFinance.Accounts>
                    (from Cubeface in SmartSpikeV2017.BasicCubeface(ourviewmodel, financeviewmodel.cubeface_code)
                    join Category in financeviewmodel.PLO.finance_accounts_changesList
                    on new { Cubeface.USERNAME, Cubeface.CUBEFACE_CODE }
                    equals new { Category.USERNAME, Category.CUBEFACE_CODE }
                    join CategoryType in financeviewmodel.PLO.finance_categorytypesList
                    on new { Category.USERNAME, Category.CUBEFACE_CODE, Category.CATEGORY_CODE }
                    equals new { CategoryType.USERNAME, CategoryType.CUBEFACE_CODE, CategoryType.CATEGORY_CODE }
                    join Account in financeviewmodel.PLO.finance_accountsList
                    on new { CategoryType.USERNAME, CategoryType.CUBEFACE_CODE, CategoryType.CATEGORY_CODE, CategoryType.CURRENCY_ORDINAL }
                    equals new { Account.USERNAME, Account.CUBEFACE_CODE, Account.CATEGORY_CODE, Account.CURRENCY_ORDINAL }
                    where Account.INSTITUTION_CODE == institutionCode &&
                            Account.BRAND_CODE == brandCode &&
                            Account.CATEGORY_CODE == categoryCode &&
                            Account.SORTCODE == accountView.EXCHANGE &&
                            Account.ACCOUNT_NO == accountView.UUID &&
                            Account.UDPRN == danApiKey.AccessId &&
                            Account.CURRENCY_ORDINAL == currencyOrdinal
                    select Account);
                    // Not there? Add it in
                    if (accounts_found.Count == 0)
                    {
                        // Dummy this one up
                        DateTime accountCreated = DateTime.UtcNow;  // UTC time
                                                                    // Its not in the Accounts List so add it in
                        SmartFinance.Accounts account_record =
                            SmartFinanceV2021.Account_Template(ourviewmodel,
                                                                financeviewmodel,
                                                                institutionCode,
                                                                brandCode,
                                                                categoryCode,
                                                                accountView.EXCHANGE,
                                                                accountView.UUID,
                                                                danApiKey.AccessId,
                                                                accountView.NAME,   // Account Title
                                                                currencyOrdinal,
                                                                Convert.ToInt32(accountView.AVAILABLE_BALANCE_VALUE.Replace(".", "")),
                                                                SmartParametersV2016.AccountStatus,
                                                                accountView.CREATED_AT);
                        account_record.Updated = false; // Its a new
                        financeviewmodel.PLO.finance_accounts_changesList.Add(account_record);
                    }
                }
            }
#endif
            
            // Get the exchange rates
            ExtractExchangeRates(financeviewmodel);

            // Build the Transaction list and the Currencies list
            await RebuildCryptoTransactions(
#if WINFORMS
                                                components,
                                                textBoxConsole,
#endif
                                            ourviewmodel,
                                            financeviewmodel,
                                            categoryCode,
                                            institutionCode,
                                            brandCode,
                                            financeviewmodel.cryptouserV2,
                                            danApiKey.Exchange,
                                            danApiKey.AccessId,
                                            transaction_types_found);
        

#if !SMARTMAUI
            financeviewmodel.rateEnabled = true;
#endif
            return true;
        }

        internal static async void ExtractExchangeRates(FinanceViewModel financeviewmodel)
        {
            string V2exchangeratesEndpoint = "api.coinbase.com/v2/exchange-rates?currency=" + financeviewmodel.WorkingCurrency;
            if (await CallApiGET(financeviewmodel, $"https://{V2exchangeratesEndpoint}", "", ""))
            {
                string exchangerateData = financeviewmodel.result;

                AnalyzeExchangeRates(financeviewmodel.WorkingCurrency, exchangerateData, financeviewmodel.v2exchangeratesList);
            }
            return;
        }

#if !SMARTMAUI
        internal static void RebuildAddressesNew(MainViewModel ourviewmodel,
                                                FinanceViewModel financeviewmodel,
                                                SmartFinance.DanApiKey danApiKey)
        {
            // Right at the start!
            // Is it in our PLO list
            List<SmartUsers.AddressesView> addresses_found = SmartSpikeV2017.Users_Lookup_AddressesUDPRN(ourviewmodel,
                                                        danApiKey.Username,
                                                        danApiKey.AccessId);
            if (addresses_found.Count == 0)
            {
#if !SMARTMAUI
                // Is it in our Changes list?
                addresses_found = new List<SmartUsers.AddressesView>
                    (from Address in ourviewmodel.Hamas.addressesview_changesList
                    where (Address.USERNAME == danApiKey.Username &&
                        Address.UDPRN == danApiKey.AccessId)
                    select Address);
                if (addresses_found.Count == 0)
                // i.e. we already know about it Just to be sure
                {
                    SmartUsers.AddressesView address = new SmartUsers.AddressesView()
                    {
                        USERNAME = danApiKey.Username,
                        UDPRN = danApiKey.AccessId,
                        RANDOM_KEY1 = SmartRoutinesV2018.GetNextRandomNew(ourviewmodel.randomR),
                        ADDRESS_CREATED = DateTime.UtcNow,
                        //AREA_CODE = 0,
                        BASIC = "",       // The one scraped from website (Bank, Utility, Water, Community Charge)
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
                        TOWN = "",
                        RANDOM_KEY2 = SmartRoutinesV2018.GetNextRandomNew(ourviewmodel.randomR),
                        CHECKED = false,
                        Updated = false     // Its new
                    };
                    ourviewmodel.Hamas.addressesview_changesList.Add(address);
                }
#endif
            }
            return;
        }
#endif

        internal static void RebuildCategories(MainViewModel ourviewmodel,
                                                    FinanceViewModel financeviewmodel,
                                                    char categoryCode)
        {            
            // Find the Category
            // Is it in the PLO list?
            List<SmartFinance.Categories> categoriesFound = SmartSpikeFinanceV2017.Finance_Find_CategoriesX(ourviewmodel,
                                                                                                            financeviewmodel,
                                                                                                            categoryCode);
            if (categoriesFound.Count == 0)
            {
                // Is it in our Changes list
                categoriesFound = new List<SmartFinance.Categories>
                    (from Cubeface in SmartSpikeV2017.BasicCubeface(ourviewmodel, financeviewmodel.cubeface_code)
                     join Category in financeviewmodel.PLO.finance_categories_changesList
                     on new { Cubeface.USERNAME, Cubeface.CUBEFACE_CODE }
                     equals new { Category.USERNAME, Category.CUBEFACE_CODE }
                     where Category.CATEGORY_CODE == categoryCode
                     select Category);
                if (categoriesFound.Count == 0)
                {
                    SmartFinance.Categories newCategory = new SmartFinance.Categories()
                    {
                        USERNAME = ourviewmodel.UserName,
                        CUBEFACE_CODE = SmartParametersV2016.Finance,
                        CATEGORY_CODE = categoryCode,
                        CHECKED = "X",
                        Updated = false // Its a new
                    };
                    financeviewmodel.PLO.finance_categories_changesList.Add(newCategory);
                }
            }
            return;
        }

        internal static void RebuildCategoryTypes(MainViewModel ourviewmodel,
                                                    FinanceViewModel financeviewmodel,
                                                    char categoryCode)
        {
            //List<SmartFinance.Accounts> accountsFound =
            //   new List<SmartFinance.Accounts>
            //   (from Accounts in financeviewmodel.
            //     orderby Accounts.ACCOUNT_CREATED
            //     select Accounts);
            if (financeviewmodel.v3accountsList.Count > 0)
            {
                foreach (SmartCrypto.V3Accounts acct in financeviewmodel.v3accountsList)
                {
                    // Find the CategoryType
                    // First find the account currency ordinal
                    short currencyOrdinal = SmartSpikeV2017.Lookup_Currency_OrdinalOld(ourviewmodel.currenciesList, acct.CURRENCY);
                    // Only do this if we have found a genuine currency
                    if (currencyOrdinal > 0)
                    {
                        // Is it in our PLO list?
                        List<SmartFinance.CategoryTypes> categorytypesFound = SmartSpikeFinanceV2017.Finance_Find_CategoryTypes(ourviewmodel,
                                                                                                                            financeviewmodel,
                                                                                                                            categoryCode,
                                                                                                                            currencyOrdinal);
                        if (categorytypesFound.Count == 0)
                        {
                            // Is it in our changes list?

                            List<SmartFinance.CategoryTypes> categoryTypesFound =
                             new List<SmartFinance.CategoryTypes>
                            (from CatTypes in financeviewmodel.PLO.finance_categorytypes_changesList
                             where CatTypes.USERNAME == ourviewmodel.UserName &&
                                CatTypes.CUBEFACE_CODE == SmartParametersV2016.Finance &&
                                CatTypes.CATEGORY_CODE == categoryCode &&
                                CatTypes.CURRENCY_ORDINAL == currencyOrdinal
                             select CatTypes);
                            if (categoryTypesFound.Count == 0)
                            {
                                // AND ...we haven't already added it to any changes!
                                SmartFinance.CategoryTypes newCategoryType = new SmartFinance.CategoryTypes()
                                {
                                    USERNAME = ourviewmodel.UserName,
                                    CUBEFACE_CODE = financeviewmodel.cubeface_code,
                                    CATEGORY_CODE = categoryCode,
                                    CURRENCY_ORDINAL = currencyOrdinal,
                                    Updated = false   // Its new
                                };
                                financeviewmodel.PLO.finance_categorytypes_changesList.Add(newCategoryType);
                            }
                        }
                    }
                }
            }
            return;
        }

        internal static void RebuildAccounts(FinanceViewModel financeviewmodel)
        {
            //financeviewmodel.accountsList =
            //    new List<SmartCrypto.V3Accounts>
            //    (from Accounts in financeviewmodel.accountsList
            //     orderby Accounts.CREATED_AT
            //     select Accounts);
#if !SMARTMAUI
            if (financeviewmodel.v2accountsList.Count > 0)
            {
                List<SmartFinance.AccountsView> tempAccounts = new List<SmartFinance.AccountsView>();
                foreach (SmartCrypto.V3Accounts acct in financeviewmodel.v3accountsList)
                {
                    bool doit = true;
                    switch (acct.TYPE)
                    {
                        case "ACCOUNT_TYPE_FIAT":
                            doit = financeviewmodel.acctsfiat;
                            break;
                        case "ACCOUNT_TYPE_CRYPTO":
                            doit = financeviewmodel.crypto;
                            break;
                        default:
                            break;
                    }
                    if (doit)
                    {
                        SmartFinance.AccountsView V2accountView = new SmartFinance.AccountsView()
                        {
                            USERNAME = acct.USERNAME,
                            EXCHANGE = acct.EXCHANGE,
                            UUID = acct.UUID,
                            ACTIVE = acct.ACTIVE,
                            AVAILABLE_BALANCE_CURRENCY = acct.AVAILABLE_BALANCE.CURRENCY,
                            AVAILABLE_BALANCE_VALUE = acct.AVAILABLE_BALANCE.VALUE,
                            CREATED_AT = acct.CREATED_AT,
                            CURRENCY = acct.CURRENCY,
                            DEFAULT = acct.DEFAULT,
                            DELETED_AT = acct.DELETED_AT,
                            HOLD_CURRENCY = acct.HOLD.CURRENCY,
                            HOLD_VALUE = acct.HOLD.VALUE,
                            NAME = acct.NAME,
                            PLATFORM = acct.PLATFORM,
                            READY = acct.READY,
                            RETAIL_PORTFOLIO_ID = acct.RETAIL_PORTFOLIO_ID,
                            TYPE = acct.TYPE,
                            UPDATED_AT = acct.UPDATED_AT
                        };
                        tempAccounts.Add(V2accountView);
                    }
                }
                financeviewmodel.FinanceCryptoAccounts = tempAccounts;
            }
#endif
            return;
        }
        internal static async Task RebuildCryptoTransactions(
#if WINFORMS
                                                    MainProcess components,
                                                    RichTextBox textBoxConsole,
#endif
                                                    MainViewModel ourviewmodel,
                                                    FinanceViewModel financeviewmodel,
                                                    char categoryCode,
                                                    short institutionCode,
                                                    short brandCode,
                                                    SmartCrypto.V2User cryptouserV2,
                                                    string Exchange,
                                                    string AccessId,
                                                    List<SmartFinance.Transaction_Types> transaction_types_found,
                                                    string header = "")
        {
            string fieldName = "CREATED_AT";
            if (header == "Activity")
            {
                fieldName = "TYPE";
            }

            //financeviewmodel.V2transactionsList =
            //    financeviewmodel.V2transactionsList
            //    .OrderBy(p => p.GetType().GetProperty(fieldName).GetValue(p, null))
            //    .ToList();

#if !SMARTMAUI
            if (financeviewmodel.v2transactionsList.Count > 0)
            {
                // Fix this then increment it to ensure uniqueness
                financeviewmodel.sequence_no = SmartFinanceV2021.UnixSequenceNo();
                //List<SmartFinance.TransactionsView> tempTransactions = new List<SmartFinance.TransactionsView>();
                financeviewmodel.FinanceCryptoCurrencyRates.Clear();

                // Find all the TransactionsCategories we haven't got into tc_changes
                // Then work through tc_changes building a distinct Transactions
                // Then work through tc_changes building a disctinct CategoryTypes
                // Make sure you have a distinct Accounts and
                // Also a ditinct Addresses ...

                // Form the TransactionsCategories tc_changes List
                foreach (SmartCrypto.V2Transactions tran in financeviewmodel.v2transactionsList)
                {
                    string descUpperCase = "";
                    bool isPaidIn = false;
                    switch (tran.TYPE)
                    {
                        case "fiat_deposit":
                            isPaidIn = true;
                            tran.TYPE = "fiat";
                            break;
                        case "buy":
                            break;
                        case "sell":
                            isPaidIn = true;
                            break;
                        case "trade":
                            break;
                        case "send":
                            break;
                        case "interest":
                            isPaidIn = true;
                            break;
                        default:
                            break;
                    }
                    if (tran.TYPE.Length > 0)
                    {
                        descUpperCase = Char.ToUpper(tran.TYPE[0]) + tran.TYPE.Substring(1);
                    }
                    // Work out the Header StatementDate and StatementNo
                    DateTime startDate = Convert.ToDateTime(cryptouserV2.CREATED_AT);   // Example start date
                    DateTime endDate = Convert.ToDateTime(tran.CREATED_AT);             // Example end date
                    int diff = SmartFinanceV2021.GetMonthsDifference(startDate, endDate);
                    short statementNo = Convert.ToInt16(diff + 1);
                    if (statementNo == 0)
                    {
                        Console.WriteLine("Stop!!");
                    }
                    DateTime statementDate = SmartFinanceV2021.GetFirstOfNextMonth(endDate);
                    // DONT DO THE HEADER HERE!! We now find DISTINCT HEADERS from tc_changes!

                    short[] transCodes = SmartSpikeFinanceV2017.Finance_Lookup_TransactionCode(financeviewmodel,
                                                                                    transaction_types_found,
                                                                                    isPaidIn,
                                                                                    descUpperCase,
                                                                                    false,
                                                                                    institutionCode,
                                                                                    brandCode,
                                                                                    categoryCode);
                    if (transCodes.Length != 2)
                    {
#if WPF || UWP || WINUI
                        await SmartBobV2017.ListenerAsync(ourviewmodel, financeviewmodel.financeToken, institutionCode, brandCode, "Crypto Trans Lookup", "Trans type  failed: " + tran.TYPE);
#endif
#if WINFORMS
                        await Task.Run(() => components.BeginInvoke(() => MainProcess.Output_Message(textBoxConsole, "Crypto Trans Lookup: Trans type  failed: " + tran.TYPE, false, false)));
#endif
                        //#if ANDROID
                        //                            await SmartBobV2017.ListenerAsync(ourviewmodel, financeviewmodel.financeToken, institution_code, brand_code, "NW Trans Lookup", "Trans type  failed: " + transaction_type);
                        //#endif
                        // Carry on
                    }
                    else
                    {
                        if (transCodes[0] == 0 || transCodes[1] == 0)
                        {
#if WPF || UWP || WINUI
                            await SmartBobV2017.ListenerAsync(ourviewmodel, financeviewmodel.financeToken, institutionCode, brandCode, "Crypto Trans Lookup", "Trans type  failed: " + tran.TYPE);
#endif
#if WINFORMS
                            await Task.Run(() => components.BeginInvoke(() => MainProcess.Output_Message(textBoxConsole, "NW Trans Lookup: Trans type  failed: " + tran.TYPE, false, false)));
#endif
                            //#if ANDROID
                            //                                await SmartBobV2017.ListenerAsync(ourviewmodel, financeviewmodel.financeToken, institution_code, brand_code, "NW Trans Lookup", "Trans type  failed: " + transaction_type);
                            //#endif

                            // Carry on - we have others to do
                        }
                    }
                    bool creditdebitIndicator = Convert.ToDouble(tran.AMOUNT.AMOUNT) < 0 ? false : true;                        
                    short currencyOrdinal = SmartSpikeV2017.Lookup_Currency_OrdinalOld(ourviewmodel.currenciesList, tran.AMOUNT.CURRENCY);
                    short nativecurrencyOrdinal = SmartSpikeV2017.Lookup_Currency_OrdinalOld(ourviewmodel.currenciesList, tran.NATIVE_AMOUNT.CURRENCY);

                    //                            
                    // Have we already got this one in PLO?
                    // (Ignore sequence_no)
                    List<SmartFinance.TransactionsCategories>
                        transactionscategories_found = SmartSpikeFinanceV2017.Finance_Lookup_BankTransaction(financeviewmodel,
                        tran.USERNAME,
                        SmartParametersV2016.Finance,
                        institutionCode,
                        brandCode,
                        tran.EXCHANGE,
                        tran.UUID,
                        AccessId,
                        statementDate,
                        statementNo,
                        tran.CREATED_AT,
                        creditdebitIndicator,     // Simple + or - amount for Crypto
                        transCodes,
                        descUpperCase,                                       // Description
                        Convert.ToDouble(tran.AMOUNT.AMOUNT),       // Amount
                        tran.TYPE,                                  // AmountType
                        Convert.ToInt32(tran.NATIVE_AMOUNT.AMOUNT.Replace(".", "")), // Value
                        currencyOrdinal,                            // Ordinal
                        0);                                         // Balance
                    // Only add it in if we can't find it
                    if (transactionscategories_found.Count == 0)
                    {
                        transactionscategories_found =
                        new List<SmartFinance.TransactionsCategories>
                        (from Transaction in financeviewmodel.PLO.transactionscategories_changesList
                         where
                             Transaction.USERNAME == tran.USERNAME &&
                             Transaction.CUBEFACE_CODE == SmartParametersV2016.Finance &&
                             Transaction.INSTITUTION_CODE == institutionCode &&
                             Transaction.BRAND_CODE == brandCode &&
                             Transaction.SORTCODE == tran.EXCHANGE &&
                             Transaction.ACCOUNT_NO == tran.UUID &&
                             Transaction.UDPRN == AccessId &&
                             Transaction.STATEMENT_DATE == statementDate &&
                             Transaction.STATEMENT_NO == statementNo &&
                             Transaction.TRANSACTION_DATE == tran.CREATED_AT &&
                             Transaction.CREDITDEBIT_INDICATOR == creditdebitIndicator &&
                             Transaction.TRANSGROUP_CODE == transCodes[0] && // Assumes everything HAS a code of course ...
                             Transaction.TRANSACTION_CODE == transCodes[1] && // Assumes everything HAS a code of course ...
                             Transaction.DESCRIPTION == descUpperCase &&
                             Transaction.AMOUNT == Convert.ToDouble(tran.AMOUNT.AMOUNT) &&
                             Transaction.AMOUNT_TYPE == tran.TYPE &&
                             Transaction.VALUE == Convert.ToInt32(tran.NATIVE_AMOUNT.AMOUNT.Replace(".", "")) &&
                             Transaction.CURRENCY_ORDINAL == currencyOrdinal &&
                             Transaction.BALANCE_AMOUNT == 0 && // Might the balance be different??
                             Transaction.BALANCE_CURRENCY_ORDINAL == currencyOrdinal
                         select Transaction);
                        if (transactionscategories_found.Count == 0)
                        {
                            // Still want to add it in even if we can't analyze the transaction ...
                            int random1 = SmartRoutinesV2018.GetNextRandomNew(ourviewmodel.randomR);
                            int random2 = SmartRoutinesV2018.GetNextRandomNew(ourviewmodel.randomR);

                            financeviewmodel.sequence_no++;
                            // Use common template
                            SmartFinance.TransactionsCategories transactionCategory =
                                SmartFinanceV2021.Transaction_Template(ourviewmodel,
                                                                        financeviewmodel,
                                                                        institutionCode,
                                                                        brandCode,
                                                                        tran.EXCHANGE,
                                                                        tran.UUID,
                                                                        AccessId,
                                                                        statementDate,
                                                                        statementNo,
                                                                        financeviewmodel.sequence_no,
                                                                        random1,
                                                                        tran.CREATED_AT,
                                                                        creditdebitIndicator,
                                                                        transCodes,
                                                                        descUpperCase,            // Description
                                                                        Convert.ToDouble(tran.AMOUNT.AMOUNT),
                                                                        tran.TYPE,
                                                                        Convert.ToInt32(tran.NATIVE_AMOUNT.AMOUNT.Replace(".", "")),
                                                                        nativecurrencyOrdinal,
                                                                        0, //balance,
                                                                        currencyOrdinal,
                                                                        creditdebitIndicator,// ? true : false,
                                                                        random2,
                                                                        false);             // Its new
                            financeviewmodel.PLO.transactionscategories_changesList.Add(transactionCategory);
                            financeviewmodel.transactions_count++;

                            

                        }
                    }
                }
                // No work through the tc_changes to do any Headers which are needed
                // Work out Distinct List
                List<SmartFinance.TransactionsCategories> tc_found =

                    new List<SmartFinance.TransactionsCategories>
                (financeviewmodel.PLO.transactionscategories_changesList.DistinctBy(key => new
                {
                    key.USERNAME,
                    key.CUBEFACE_CODE,
                    key.INSTITUTION_CODE,
                    key.BRAND_CODE,
                    key.SORTCODE,
                    key.ACCOUNT_NO,
                    key.UDPRN,
                    key.STATEMENT_DATE,
                    key.STATEMENT_NO
                }));

                foreach (SmartFinance.TransactionsCategories tran in tc_found)
                {
                    SmartFinance.Transactions transHeader = new SmartFinance.Transactions()
                    {
                        USERNAME = tran.USERNAME,
                        CUBEFACE_CODE = tran.CUBEFACE_CODE,
                        INSTITUTION_CODE = tran.INSTITUTION_CODE,
                        BRAND_CODE = tran.BRAND_CODE,
                        SORTCODE = tran.SORTCODE,
                        ACCOUNT_NO = tran.ACCOUNT_NO,
                        UDPRN = tran.UDPRN,
                        STATEMENT_DATE = tran.STATEMENT_DATE,
                        STATEMENT_NO = tran.STATEMENT_NO
                    };
                    // Try and find the Header in PLO
                    List<SmartFinance.Transactions> transheaders_found =
                           new List<SmartFinance.Transactions>
                    (from Header in financeviewmodel.PLO.transactionsList
                     where Header.USERNAME == transHeader.USERNAME &&
                            Header.CUBEFACE_CODE == transHeader.CUBEFACE_CODE &&
                            Header.INSTITUTION_CODE == transHeader.INSTITUTION_CODE &&
                            Header.BRAND_CODE == transHeader.BRAND_CODE &&
                            Header.SORTCODE == transHeader.SORTCODE &&
                            Header.ACCOUNT_NO == transHeader.ACCOUNT_NO &&
                            Header.STATEMENT_DATE == transHeader.STATEMENT_DATE &&
                            Header.STATEMENT_NO == transHeader.STATEMENT_NO &&
                            Header.UDPRN == transHeader.UDPRN
                     select Header).ToList();
                    if (transheaders_found.Count == 0)
                    {
                        // Not there? Is it in our changes?
                        transheaders_found =
                           new List<SmartFinance.Transactions>
                            (from Header in financeviewmodel.PLO.transactions_changesList
                             where Header.USERNAME == transHeader.USERNAME &&
                                    Header.CUBEFACE_CODE == transHeader.CUBEFACE_CODE &&
                                    Header.INSTITUTION_CODE == transHeader.INSTITUTION_CODE &&
                                    Header.BRAND_CODE == transHeader.BRAND_CODE &&
                                    Header.SORTCODE == transHeader.SORTCODE &&
                                    Header.ACCOUNT_NO == transHeader.ACCOUNT_NO &&
                                    Header.STATEMENT_DATE == transHeader.STATEMENT_DATE &&
                                    Header.STATEMENT_NO == transHeader.STATEMENT_NO &&
                                    Header.UDPRN == transHeader.UDPRN
                             select Header).ToList();
                        if (transheaders_found.Count == 0)
                        {
                            // Add it in
                            transHeader.RANDOMKEY1 = SmartRoutinesV2018.GetNextRandomNew(ourviewmodel.randomR);
                            transHeader.Updated = false; // Its new
                            financeviewmodel.PLO.transactions_changesList.Add(transHeader);
                        }
                    }
                }
                return;
            }
#endif
        }



        //public static void RebuildAddresses(FinanceViewModel financeviewmodel)
        //{
        //    financeviewmodel.addressesList =
        //        new List<SmartCrypto.V2Addresses>
        //        (from Addresses in financeviewmodel.addressesList
        //         orderby Addresses.CREATED_AT
        //         select Addresses);
        //    if (financeviewmodel.addressesList.Count > 0)
        //    {
        //        List<SmartFinance.AddressesView> tempAddresses = new List<SmartFinance.AddressesView>();
        //        financeviewmodel.FinanceCryptoCurrencyRates.Clear();

        //        foreach (SmartCrypto.V2Addresses addr in financeviewmodel.addressesList)
        //        {
        //            SmartFinance.AddressesView addressView = new SmartFinance.AddressesView()
        //            {
        //                USERNAME = addr.USERNAME,
        //                EXCHANGE = addr.EXCHANGE,
        //                UUID = addr.UUID,
        //                ADDRESS = addr.ADDRESS,
        //                ADDRESS_INFO_ADDRESS = addr.ADDRESS_INFO.ADDRESS,
        //                ADDRESS_LABEL = addr.ADDRESS_LABEL,
        //                CALLBACK_URL = addr.CALLBACK_URL,
        //                CREATED_AT = addr.CREATED_AT,
        //                DEFAULT_RECEIVE = addr.DEFAULT_RECEIVE,
        //                DEPOSIT_URI = addr.DEPOSIT_URI,
        //                DESTINATION_TAG = addr.DESTINATION_TAG,
        //                ID = addr.ID,
        //                INLINE_WARNING_TEXT = addr.INLINE_WARNING.TEXT,
        //                INLINE_WARNING_TOOLTIP = addr.INLINE_WARNING.TOOLTIP,
        //                NAME = addr.NAME,
        //                NETWORK = addr.NETWORK,
        //                QR_CODE_IMAGE_URL = addr.QR_CODE_IMAGE_URL,
        //                RECEIVE_SUBTITLE = addr.RECEIVE_SUBTITLE,
        //                RESOURCE = addr.RESOURCE,
        //                RESOURCE_PATH = addr.RESOURCE_PATH,
        //                SHARE_ADDRESS_COPY_LINE1 = addr.SHARE_ADDRESS_COPY.LINE1,
        //                SHARE_ADDRESS_COPY_LINE2 = addr.SHARE_ADDRESS_COPY.LINE2,
        //                UPDATED_AT = addr.UPDATED_AT,
        //                URI_SCHEME = addr.URI_SCHEME
        //            };
        //            tempAddresses.Add(addressView);
        //        }
        //        financeviewmodel.FinanceCryptoAddresses = tempAddresses;
        //    }
        //    return;
        //}

        public static void RebuildCurrencies(FinanceViewModel financeviewmodel)
        {
            if (financeviewmodel.v2exchangeratesList.Count > 0)
            {
                List<SmartCrypto.V2CryptoTotals> temp = new List<SmartCrypto.V2CryptoTotals>();

                foreach (SmartCrypto.V2CryptoTotals totals in financeviewmodel.FinanceCryptoCurrencyRates)
                {
                    List<SmartCrypto.V2ExchangeRates> rates_found =
                        new List<SmartCrypto.V2ExchangeRates>
                        (from Rate in financeviewmodel.v2exchangeratesList
                         where Rate.NAMEX == totals.CRYPTO_CURRENCY
                         select Rate);
                    if (rates_found.Count > 0)
                    {
                        totals.RATE = Convert.ToDouble(rates_found.First().RATE);
                        if (totals.RATE != 0)
                        {
                            totals.RATE = 1.0 / totals.RATE;
                        }
                    }
                    temp.Add(totals);
                }
                financeviewmodel.FinanceCryptoCurrencyRates = temp;
            }
            return;
        }

        public static async Task<bool> CallApiGET(FinanceViewModel financeviewmodel,
                                                    string url, string bearerToken = "",
                                                    string cbversion = "")
        {
            try
            {
                HttpClient client = new HttpClient();
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, url);

                if (bearerToken != "")
                    request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", bearerToken);
                if (cbversion != "")
                {
                    request.Headers.Add("CB-VERSION", cbversion);
                }
                HttpResponseMessage response = await client.SendAsync(request);
                if (response != null)
                {
                    financeviewmodel.result = response.Content.ReadAsStringAsync().Result.TrimEnd();
                    if (financeviewmodel.result != "Unauthorized")
                    {
                        return true;
                    }
                }
            }
            catch (Exception)
            {
                financeviewmodel.result = "";
            }
            return false;
        }


        //internal static string GenerateToken(MainViewModel ourviewmodel,
        //                                    FinanceViewModel financeviewmodel,
        //                                        string name,
        //                                        string secret,
        //                                        string uri)
        //{
        internal static string GenerateToken(FinanceViewModel financeviewmodel,
                                                string name,
                                                string secret,
                                                string uri)
        {
            var privateKeyBytes = Convert.FromBase64String(secret); // Assuming PEM is base64 encoded
            using var key = ECDsa.Create();
            key.ImportECPrivateKey(privateKeyBytes, out _);

            var payload = new Dictionary<string, object>
             {
                 { "sub", name },
                 { "iss", "coinbase-cloud" },
                 { "nbf", Convert.ToInt64((DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds) },
                 { "exp", Convert.ToInt64((DateTime.UtcNow.AddMinutes(1) - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds) },
                 { "uri", uri }
             };

            var extraHeaders = new Dictionary<string, object>
             {
                 { "kid", name },
                 // add nonce to prevent replay attacks with a random 10 digit number
                 { "nonce", randomHex(financeviewmodel, 10) },
                 { "typ", "JWT"}
             };

            var encodedToken = JWT.Encode(payload, key, JwsAlgorithm.ES256, extraHeaders);

            // print token
            //Console.WriteLine(encodedToken);
            return encodedToken;
        }

        internal static bool isTokenValid(MainViewModel ourviewmodel, string token, string tokenId, string secret)
        {
            string SecretKey = secret; // Replace with a strong secret key
            SymmetricSecurityKey SecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SecretKey));

            string tokenx = JwtHelper.GenerateToken(ourviewmodel.UserName);
            Console.WriteLine($"Generated Token: {tokenx}");

            if (JwtHelper.ValidateToken(tokenx, out ClaimsPrincipal principal))
            {
                Console.WriteLine($"Token is valid. User: {principal.Identity.Name}");
            }
            else
            {
                Console.WriteLine("Invalid token.");
            }
            if (token == null)
            {
                return false;
            }
            ECDsa key = ECDsa.Create();
            key.ImportECPrivateKey(Convert.FromBase64String(secret), out _);

            var securityKey = new ECDsaSecurityKey(key) { KeyId = tokenId };
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = securityKey,
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero
                }, out var validatedToken);

                //return true;
            }
            catch
            {
                return false;
            }
            return true;
        }
        public class JwtHelper
        {
            private const string SecretKey = "your-256-bit-secret"; // Replace with a strong secret key
            private static readonly Microsoft.IdentityModel.Tokens.SymmetricSecurityKey SecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SecretKey));

            public static string GenerateToken(string username, int expireMinutes = 30)
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var credentials = new SigningCredentials(SecurityKey, SecurityAlgorithms.HmacSha256);

                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new[]
                    {
                        new Claim(ClaimTypes.Name, username),
                        new Claim(ClaimTypes.Role, "Admin") // Example role
                    }),
                    Expires = DateTime.UtcNow.AddMinutes(expireMinutes),
                    SigningCredentials = credentials
                };

                var token = tokenHandler.CreateToken(tokenDescriptor);
                return tokenHandler.WriteToken(token);
            }
            public static bool ValidateToken(string token, out ClaimsPrincipal principal)
            {
                principal = null;
                var tokenHandler = new JwtSecurityTokenHandler();
                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = SecurityKey,
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true, // Ensure the token has not expired
                    ClockSkew = TimeSpan.Zero
                };

                try
                {
                    principal = tokenHandler.ValidateToken(token, validationParameters, out _);
                    return true;
                }
                catch
                {
                    return false;
                }
            }
        }

        internal static string parseKey(string key)
        {
            List<string> keyLines = new List<string>();
            // For reasons which escape me (!!) I cannot split the
            // Secret key at \n characters when I'm reading it in from
            // a text file, but if I replace them with a 'bar' then I can
            // Life is too short to spend too much time wondering ... why??
            key = key.Replace("\\n", SmartParametersV2016.bar.ToString());
            keyLines.AddRange(key.Split(SmartParametersV2016.bar, StringSplitOptions.RemoveEmptyEntries));
            // Remove 'BEGIN' prefix
            keyLines.RemoveAt(0);
            // Remove 'END' suffix
            keyLines.RemoveAt(keyLines.Count - 1);
            return String.Join("", keyLines);
        }

        internal static string randomHex(FinanceViewModel financeviewmodel, int digits)
        {
            byte[] buffer = new byte[digits / 2];
            financeviewmodel.random.NextBytes(buffer);
            string result = String.Concat(buffer.Select(x => x.ToString("X2")).ToArray());
            if (digits % 2 == 0)
            {
                return result;
            }
            return result + financeviewmodel.random.Next(16).ToString("X");
        }

        internal static void AnalyzeExchangeRates(string workingcurrency, string exchangeratesData, List<SmartCrypto.V2ExchangeRates> exchangeratesList)
        {
            // These are all based to the USD!
            dynamic jsonResponse = JObject.Parse(exchangeratesData);
            foreach (dynamic statement in jsonResponse)
            {
                string statement_name = statement.Name;
                if (statement_name == "data")   // We're not interested in pagination as of Phase ONE
                {
                    // Clear down any previous
                    exchangeratesList.Clear();
                    foreach (dynamic transactions in statement)
                    {
                        foreach (dynamic transaction in transactions)
                        {
                            string transactionName = transaction.Name;
                            switch (transactionName)
                            {
                                case "currency":
                                    // These are all based to the USD!
                                    if (transaction.Value.ToString() != workingcurrency)
                                    {
                                        return; // !!
                                    }
                                    break;
                                case "rates":
                                    // These are all based to the USD!
                                    foreach (dynamic exchangerateItem in transaction.Value)
                                    {
                                        SmartCrypto.V2ExchangeRates v2exchangerate = new SmartCrypto.V2ExchangeRates();
                                        v2exchangerate.NAMEX = exchangerateItem.Name.ToString();
                                        v2exchangerate.RATE = exchangerateItem.Value.ToString();
                                        exchangeratesList.Add(v2exchangerate);
                                    }
                                    break;
                            }
                        }
                    }
                }
            }
            return;
        }

        internal static void AnalyzeCurrencies(string currenciesData, List<SmartCrypto.V2Currencies> currenciesList)
        {
            dynamic jsonResponse = JObject.Parse(currenciesData);
            foreach (dynamic statement in jsonResponse)
            {
                string statement_name = statement.Name;
                if (statement_name == "data")   // We're not interested in pagination as of Phase ONE
                {
                    foreach (dynamic transactions in statement)
                    {
                        foreach (dynamic transaction in transactions)
                        {
                            SmartCrypto.V2Currencies v2currency = new SmartCrypto.V2Currencies();
                            foreach (dynamic currencyItem in transaction)
                            {
                                string currencyName = currencyItem.Name;
                                switch (currencyName)
                                {
                                    case "id":
                                        v2currency.ID = currencyItem.Value.ToString();
                                        break;
                                    case "min_size":
                                        v2currency.MIN_SIZE = currencyItem.Value.ToString();
                                        break;
                                    case "name":
                                        v2currency.NAME = currencyItem.Value.ToString();
                                        break;
                                    default:
                                        break;
                                }
                            }
                            if (v2currency.ID != null)
                            {
                                currenciesList.Add(v2currency);
                            }
                        }
                    }
                }
            }
            return;
        }

        internal static SmartCrypto.V2User AnalyzeUserV2(string responseAddress,
                                                string Username,
                                                string AccessId)
        {
            SmartCrypto.V2User cryptoUser = new SmartCrypto.V2User();
            dynamic jsonResponse = JObject.Parse(responseAddress);
            foreach (dynamic statement in jsonResponse)
            {
                string statement_name = statement.Name;
                if (statement_name == "data")   // We're not interested in pagination as of Phase ONE
                {
                    foreach (dynamic transactions in statement)
                    {
                        foreach (dynamic individual in transactions)
                        {
                            string column = individual.Name;
                            switch (column)
                            {
                                case "bitcoin_unit":
                                    cryptoUser.BITCOIN_UNIT = individual.Value.ToString();
                                    break;
                                case "country":
                                    cryptoUser.COUNTRY = individual.Value.ToString();
                                    break;
                                case "created_at":
                                    if (individual.Value != null)
                                    {
                                        cryptoUser.CREATED_AT = Convert.ToDateTime(individual.Value);
                                    }
                                    else
                                    {
                                        cryptoUser.CREATED_AT = SmartParametersV2016.defaultDate;
                                    }
                                    break;
                                case "email":
                                    cryptoUser.EMAIL = individual.Value.ToString();
                                    break;
                                case "id":
                                    cryptoUser.ID = individual.Value.ToString();
                                    break;
                                case "name":
                                    cryptoUser.NAME = individual.Value.ToString();
                                    break;
                                case "native_currency":
                                    cryptoUser.NATIVE_CURRENCY = individual.Value.ToString();
                                    break;
                                case "resource":
                                    cryptoUser.RESOURCE = individual.Value.ToString();
                                    break;
                                case "resource_path":
                                    cryptoUser.RESOURCE_PATH = individual.Value.ToString();
                                    break;
                                case "time_zone":
                                    if (individual.Value != null)
                                        try
                                        {
                                            string tz = individual.Value.ToString();
                                            tz = tz.Replace("Atlantic Time (Canada)", "Atlantic Standard Time");
                                            cryptoUser.TIME_ZONE = TimeZoneInfo.FindSystemTimeZoneById(tz);
                                        }
                                        catch (Exception)
                                        {
                                            cryptoUser.TIME_ZONE = TimeZoneInfo.FindSystemTimeZoneById("Greenwich Standard Time");
                                        }
                                    break;
                                default:
                                    break;
                            }
                        }
                        if (cryptoUser.ID != null)
                        {
                            cryptoUser.USERNAME = Username;
                            cryptoUser.EMAIL = AccessId;
                        }
                    }
                }
            }
            return cryptoUser;
        }

        internal static List<SmartCrypto.V2Accounts> AnalyzeAccountV2(string accountsData,
                                                string Username,
                                                string Exchange)
        {
            List<SmartCrypto.V2Accounts> V2accountsList = new List<SmartCrypto.V2Accounts>();

            dynamic jsonResponse = JObject.Parse(accountsData);
            foreach (dynamic statement in jsonResponse)
            {
                string statement_name = statement.Name;
                if (statement_name == "data")
                {
                    foreach (dynamic transactions in statement)
                    {
                        foreach (dynamic transaction in transactions)
                        {
                            SmartCrypto.V2Accounts v2account = new SmartCrypto.V2Accounts();

                            foreach (dynamic individual in transaction)
                            {
                                string column = individual.Name;
                                switch (column)
                                {
                                    case "allow_deposits":
                                        v2account.ALLOW_DEPOSITS = individual.Value.ToString();
                                        break;
                                    case "allow_withdrawals":
                                        v2account.ALLOW_WITHDRAWALS = individual.Value.ToString();
                                        break;
                                    case "balance":
                                        var v2balance = v2account.BALANCE;
                                        foreach (dynamic balanceItem in individual.Value)
                                        {
                                            string balanceName = balanceItem.Name;
                                            switch (balanceName)
                                            {
                                                case "amount":
                                                    v2balance.AMOUNT = balanceItem.Value.ToString();
                                                    break;
                                                case "currency":
                                                    v2balance.CURRENCY = balanceItem.Value.ToString();
                                                    break;
                                                default:
                                                    break;
                                            }
                                        }
                                        v2account.BALANCE = v2balance;
                                        break;
                                    case "created_at":
                                        if (individual.Value != null)
                                        {
                                            v2account.CREATED_AT = Convert.ToDateTime(individual.Value);
                                        }
                                        else
                                        {
                                            v2account.CREATED_AT = SmartParametersV2016.defaultDate;
                                        }
                                        break;
                                    case "currency":
                                        var v2currency = v2account.CURRENCY;
                                        foreach (dynamic currencyItem in individual.Value)
                                        {
                                            string currencyName = currencyItem.Name;
                                            switch (currencyName)
                                            {
                                                case "asset_id":
                                                    v2currency.ASSET_ID = currencyItem.Value.ToString();
                                                    break;
                                                case "code":
                                                    v2currency.CODE = currencyItem.Value.ToString();
                                                    break;
                                                case "color":
                                                    v2currency.COLOR = currencyItem.Value.ToString();
                                                    break;
                                                case "exponent":
                                                    v2currency.EXPONENT = currencyItem.Value.ToString();
                                                    break;
                                                case "name":
                                                    v2currency.NAME = currencyItem.Value.ToString();
                                                    break;
                                                case "rewards":
                                                    if (currencyItem.Value != null)
                                                    {
                                                        var v2rewards = v2currency.REWARDS;
                                                        foreach (dynamic rewardItems in currencyItem)
                                                        {
                                                            foreach (dynamic rewardItem in rewardItems)
                                                            {
                                                                string rewardName = rewardItem.Name;
                                                                switch (rewardName)
                                                                {
                                                                    case "apy":
                                                                        v2rewards.APY = rewardItem.Value.ToString();
                                                                        break;
                                                                    case "formatted_apy":
                                                                        v2rewards.FORMATTED_APY = rewardItem.Value.ToString();
                                                                        break;
                                                                    case "label":
                                                                        v2rewards.LABEL = rewardItem.Value.ToString();
                                                                        break;
                                                                    default:
                                                                        break;
                                                                }
                                                            }
                                                        }
                                                        v2currency.REWARDS = v2rewards;
                                                    }
                                                    break;
                                                case "slug":
                                                    v2currency.SLUG = currencyItem.Value.ToString();
                                                    break;
                                                case "type":
                                                    v2currency.TYPE = currencyItem.Value.ToString();
                                                    break;
                                                default:
                                                    break;
                                            }
                                        }
                                        v2account.CURRENCY = v2currency;
                                        break;
                                    case "id":
                                        v2account.ID = individual.Value.ToString();
                                        break;
                                    case "name":
                                        v2account.NAME = individual.Value.ToString();
                                        break;
                                    case "primary":
                                        v2account.PRIMARY = individual.Value.ToString();
                                        break;
                                    case "resource":
                                        v2account.RESOURCE = individual.Value.ToString();
                                        break;
                                    case "resource_path":
                                        v2account.RESOURCE_PATH = individual.Value.ToString();
                                        break;
                                    case "type":
                                        v2account.TYPE = individual.Value.ToString();
                                        break;
                                    case "updated_at":
                                        if (individual.Value != null)
                                        {
                                            v2account.UPDATED_AT = Convert.ToDateTime(individual.Value);
                                        }
                                        else
                                        {
                                            v2account.UPDATED_AT = SmartParametersV2016.defaultDate;
                                        }
                                        break;
                                    default:
                                        Console.Write(column);
                                        break;
                                }
                            }
                            if (v2account.ID != null)
                            {
                                v2account.USERNAME = Username;
                                v2account.EXCHANGE = Exchange;
                                V2accountsList.Add(v2account);
                            }
                        }
                    }
                }
            }
            return V2accountsList;
        }

        internal static void AnalyzeAddressV2(string responseAddress,
                                                List<SmartCrypto.V2Addresses> addressesList,
                                                string Username,
                                                string Exchange,
                                                string Uuid)
        {
            dynamic jsonResponse = JObject.Parse(responseAddress);
            foreach (dynamic statement in jsonResponse)
            {
                string statement_name = statement.Name;
                if (statement_name == "data")   // We're not interested in pagination as of Phase ONE
                {
                    foreach (dynamic transactions in statement)
                    {
                        foreach (dynamic transaction in transactions)
                        {
                            SmartCrypto.V2Addresses addr = new SmartCrypto.V2Addresses();
                            foreach (dynamic individual in transaction)
                            {
                                string column = individual.Name;
                                switch (column)
                                {
                                    case "address":
                                        addr.ADDRESS = individual.Value.ToString();
                                        break;
                                    case "address_info":
                                        var addrInfo = addr.ADDRESS_INFO;
                                        foreach (dynamic addressItem in individual.Value)
                                        {
                                            string addressName = addressItem.Name;
                                            switch (addressName)
                                            {
                                                case "address":
                                                    addrInfo.ADDRESS = addressItem.Value.ToString();
                                                    break;
                                                default:
                                                    break;
                                            }
                                        }
                                        addr.ADDRESS_INFO = addrInfo;
                                        break;
                                    case "address_label":
                                        addr.ADDRESS_LABEL = individual.Value.ToString();
                                        break;
                                    case "callback_url":
                                        if (individual.Value != null)
                                        {
                                            addr.CALLBACK_URL = individual.Value.ToString();
                                        }
                                        break;
                                    case "created_at":  // UTC time
                                        if (individual.Value != null)
                                        {
                                            addr.CREATED_AT = Convert.ToDateTime(individual.Value);
                                        }
                                        else
                                        {
                                            addr.CREATED_AT = SmartParametersV2016.defaultDate;
                                        }
                                        break;
                                    case "default_receive":
                                        addr.DEFAULT_RECEIVE = individual.Value.ToString();
                                        break;
                                    case "deposit_uri":
                                        addr.DEPOSIT_URI = individual.Value.ToString();
                                        break;
                                    case "destination_tag":
                                        addr.DESTINATION_TAG = individual.Value.ToString();
                                        break;
                                    case "id":
                                        addr.ID = individual.Value.ToString();
                                        break;
                                    case "inline_warning":
                                        var inlineWarning = addr.INLINE_WARNING;
                                        foreach (dynamic inlineItem in individual.Value)
                                        {
                                            string inlineName = inlineItem.Name;
                                            switch (inlineName)
                                            {
                                                case "text":
                                                    inlineWarning.TEXT = inlineItem.Value.ToString();
                                                    break;
                                                case "tooltip":
                                                    inlineWarning.TOOLTIP = inlineItem.Value.ToString();
                                                    break;
                                                default:
                                                    break;
                                            }
                                        }
                                        addr.INLINE_WARNING = inlineWarning;
                                        break;
                                    case "name":
                                        if (individual.Value != null)
                                        {
                                            addr.NAME = individual.Value.ToString();
                                        }
                                        break;
                                    case "network":
                                        addr.NETWORK = individual.Value.ToString();
                                        break;
                                    case "qr_code_image_url":
                                        addr.QR_CODE_IMAGE_URL = individual.Value.ToString();
                                        break;
                                    case "receive_subtitle":
                                        if (individual.Value != null)
                                        {
                                            addr.RECEIVE_SUBTITLE = individual.Value.ToString();
                                        }
                                        break;
                                    case "resource":
                                        addr.RESOURCE = individual.Value.ToString();
                                        break;
                                    case "resource_path":
                                        addr.RESOURCE_PATH = individual.Value.ToString();
                                        break;
                                    case "share_address_copy":
                                        var shareAddr = addr.SHARE_ADDRESS_COPY;
                                        foreach (dynamic shareItem in individual.Value)
                                        {
                                            string shareName = shareItem.Name;
                                            switch (shareName)
                                            {
                                                case "line1":
                                                    shareAddr.LINE1 = shareItem.Value.ToString();
                                                    break;
                                                case "line2":
                                                    shareAddr.LINE2 = shareItem.Value.ToString();
                                                    break;
                                                default:
                                                    break;
                                            }
                                        }
                                        addr.SHARE_ADDRESS_COPY = shareAddr;
                                        break;
                                    case "updated_at":
                                        if (individual.Value != null)
                                        {
                                            addr.UPDATED_AT = Convert.ToDateTime(individual.Value);
                                        }
                                        else
                                        {
                                            addr.UPDATED_AT = SmartParametersV2016.defaultDate;
                                        }
                                        break;
                                    case "uri_scheme":
                                        addr.URI_SCHEME = individual.Value.ToString();
                                        break;
                                    default:
                                        // Don't deal with warnings as yet!
                                        if (!(column == "warnings" ||
                                             column == "launch_warning"))
                                        {
                                            Console.Write(column);
                                        }
                                        break;
                                }
                            }
                            if (addr.ID != null)
                            {
                                addr.USERNAME = Username;
                                addr.EXCHANGE = Exchange;
                                addr.UUID = Uuid;
                                addressesList.Add(addr);
                            }
                        }
                    }
                }
            }
            return;
        }
        internal static void AnalyzeTransactionV2(string transactionsData,
                                                FinanceViewModel financeviewmodel,
                                                string Username,
                                                string Exchange,
                                                string Uuid,
                                                string Name)
        {
            dynamic jsonResponse = JObject.Parse(transactionsData);
            foreach (dynamic statement in jsonResponse)
            {
                string statement_name = statement.Name;
                if (statement_name == "data")   // We're not interested in pagination as of Phase ONE
                {
                    foreach (dynamic transactions in statement)
                    {
                        foreach (dynamic transaction in transactions)
                        {
                            SmartCrypto.V2Transactions v2trans = new SmartCrypto.V2Transactions();

                            foreach (dynamic individual in transaction)
                            {
                                string column = individual.Name;
                                switch (column)
                                {
                                    case "amount":
                                        var transAmount = v2trans.AMOUNT;
                                        foreach (dynamic amountItem in individual.Value)
                                        {
                                            string amountName = amountItem.Name;
                                            switch (amountName)
                                            {
                                                case "amount":
                                                    transAmount.AMOUNT = amountItem.Value.ToString();
                                                    break;
                                                case "currency":
                                                    transAmount.CURRENCY = amountItem.Value.ToString();
                                                    break;
                                                default:
                                                    break;
                                            }

                                        }
                                        v2trans.AMOUNT = transAmount;
                                        break;
                                    case "buy":
                                    case "trade":
                                    case "sell":
                                        var transBuySell = v2trans.BUYSELL;
                                        foreach (dynamic amountItem in individual.Value)
                                        {
                                            string amountName = amountItem.Name;
                                            switch (amountName)
                                            {
                                                case "fee":
                                                    var transFee = transBuySell.FEE;
                                                    foreach (dynamic feeItem in amountItem.Value)
                                                    {
                                                        string feeName = feeItem.Name;
                                                        switch (feeName)
                                                        {
                                                            case "amount":
                                                                transFee.AMOUNT = feeItem.Value.ToString();
                                                                break;
                                                            case "currency":
                                                                transFee.CURRENCY = feeItem.Value.ToString();
                                                                break;
                                                            default:
                                                                break;
                                                        }
                                                    }
                                                    transBuySell.FEE = transFee;
                                                    break;
                                                case "id":
                                                    transBuySell.ID = amountItem.Value.ToString();
                                                    break;
                                                case "payment_method_name":
                                                    transBuySell.PAYMENT_METHOD_NAME = amountItem.Value.ToString();
                                                    break;
                                                case "subtotal":
                                                    var transSubtotal = transBuySell.SUBTOTAL;
                                                    foreach (dynamic subtotalItem in amountItem.Value)
                                                    {
                                                        string subtotalName = subtotalItem.Name;
                                                        switch (subtotalName)
                                                        {
                                                            case "amount":
                                                                transSubtotal.AMOUNT = subtotalItem.Value.ToString();
                                                                break;
                                                            case "currency":
                                                                transSubtotal.CURRENCY = subtotalItem.Value.ToString();
                                                                break;
                                                            default:
                                                                break;
                                                        }
                                                    }
                                                    transBuySell.SUBTOTAL = transSubtotal;
                                                    break;
                                                case "total":
                                                    var transTotal = transBuySell.TOTAL;
                                                    foreach (dynamic totalItem in amountItem.Value)
                                                    {
                                                        string totalName = totalItem.Name;
                                                        switch (totalName)
                                                        {
                                                            case "amount":
                                                                transTotal.AMOUNT = totalItem.Value.ToString();
                                                                break;
                                                            case "currency":
                                                                transTotal.CURRENCY = totalItem.Value.ToString();
                                                                break;
                                                            default:
                                                                break;
                                                        }
                                                    }
                                                    transBuySell.TOTAL = transTotal;
                                                    break;
                                                default:
                                                    break;
                                            }
                                        }
                                        v2trans.BUYSELL = transBuySell;
                                        break;
                                    case "created_at":
                                        if (individual.Value != null)
                                        {
                                            v2trans.CREATED_AT = Convert.ToDateTime(individual.Value);
                                        }
                                        else
                                        {
                                            v2trans.CREATED_AT = SmartParametersV2016.defaultDate;
                                        }
                                        break;
                                    case "id":
                                        v2trans.ID = individual.Value.ToString();
                                        break;
                                    case "idem":
                                        v2trans.IDEM = individual.Value.ToString();
                                        break;
                                    case "native_amount":
                                        var transNativeAmount = v2trans.NATIVE_AMOUNT;
                                        foreach (dynamic amountItem in individual.Value)
                                        {
                                            string amountName = amountItem.Name;
                                            switch (amountName)
                                            {
                                                case "amount":
                                                    transNativeAmount.AMOUNT = amountItem.Value.ToString();
                                                    break;
                                                case "currency":
                                                    transNativeAmount.CURRENCY = amountItem.Value.ToString();
                                                    break;
                                                default:
                                                    break;
                                            }
                                        }
                                        v2trans.NATIVE_AMOUNT = transNativeAmount;
                                        break;
                                    case "network":
                                        var transNetwork = v2trans.NETWORK;
                                        foreach (dynamic networkItem in individual.Value)
                                        {
                                            string networkName = networkItem.Name;
                                            switch (networkName)
                                            {
                                                case "hash":
                                                    transNetwork.HASH = networkItem.Value.ToString();
                                                    break;
                                                case "network_name":
                                                    transNetwork.NETWORK_NAME = networkItem.Value.ToString();
                                                    break;
                                                case "status":
                                                    transNetwork.STATUS = networkItem.Value.ToString();
                                                    break;
                                                case "status_description":
                                                    transNetwork.STATUS_DESCRIPTION = networkItem.Value.ToString();
                                                    break;
                                                case "transaction_url":
                                                    transNetwork.TRANSACTION_URL = networkItem.Value.ToString();
                                                    break;
                                                case "transaction_fee":
                                                    transNetwork.TRANSACTION_FEE = networkItem.Value.ToString();
                                                    break;
                                                default:
                                                    break;
                                            }
                                        }
                                        v2trans.NETWORK = transNetwork;
                                        break;
                                    case "resource":
                                        v2trans.RESOURCE = individual.Value.ToString();
                                        break;
                                    case "resource_path":
                                        v2trans.RESOURCE_PATH = individual.Value.ToString();
                                        break;
                                    case "status":
                                        v2trans.STATUS = individual.Value.ToString();
                                        break;
                                    case "to":
                                        var transTo = v2trans.TO;
                                        foreach (dynamic toItem in individual.Value)
                                        {
                                            string toName = toItem.Name;
                                            switch (toName)
                                            {
                                                case "address":
                                                    transTo.ADDRESS = toItem.Value.ToString();
                                                    break;
                                                case "resource":
                                                    transTo.RESOURCE = toItem.Value.ToString();
                                                    break;
                                                default:
                                                    break;
                                            }
                                        }
                                        v2trans.TO = transTo;
                                        break;
                                    case "type":
                                        v2trans.TYPE = individual.Value.ToString();
                                        break;
                                    default:
                                        break;
                                }
                            }
                            if (v2trans.ID != null)
                            {
                                v2trans.USERNAME = Username;
                                v2trans.EXCHANGE = Exchange;
                                v2trans.UUID = Uuid;
                                financeviewmodel.v2transactionsList.Add(v2trans);
                            }
                        }
                    }
                }
            }
            return;
        }

        internal static List<SmartCrypto.V3Accounts> AnalyzeAccountV3(string accountsData,
                                            string Username,
                                            string Exchange)
        {
            List<SmartCrypto.V3Accounts> V3accountList = new List<SmartCrypto.V3Accounts>();
            dynamic jsonResponse = JObject.Parse(accountsData);
            foreach (dynamic statement in jsonResponse)
            {
                string statement_name = statement.Name;
                if (statement_name == "accounts")
                {
                    foreach (dynamic transactions in statement)
                    {
                        foreach (dynamic transaction in transactions)
                        {
                            SmartCrypto.V3Accounts v3account = new SmartCrypto.V3Accounts();

                            foreach (dynamic individual in transaction)
                            {
                                string column = individual.Name;
                                switch (column)
                                {
                                    case "active":
                                        v3account.ACTIVE = individual.Value.ToString();
                                        break;
                                    case "available_balance":
                                        var v3availBalance = v3account.AVAILABLE_BALANCE;
                                        foreach (dynamic balanceItem in individual.Value)
                                        {
                                            string balanceName = balanceItem.Name;
                                            switch (balanceName)
                                            {
                                                case "value":
                                                    v3availBalance.VALUE = balanceItem.Value.ToString();
                                                    break;
                                                case "currency":
                                                    v3availBalance.CURRENCY = balanceItem.Value.ToString();
                                                    break;
                                                default:
                                                    break;
                                            }
                                        }
                                        v3account.AVAILABLE_BALANCE = v3availBalance;
                                        break;
                                    case "created_at":
                                        if (individual.Value != null)
                                        {
                                            v3account.CREATED_AT = Convert.ToDateTime(individual.Value);
                                        }
                                        else
                                        {
                                            v3account.CREATED_AT = SmartParametersV2016.defaultDate;
                                        }
                                        break;
                                    case "currency":
                                        v3account.CURRENCY = individual.Value.ToString();
                                        break;
                                    case "default":
                                        v3account.DEFAULT = individual.Value.ToString();
                                        break;
                                    case "deleted_at":
                                        if (individual.Value != null)
                                        {
                                            v3account.DELETED_AT = Convert.ToDateTime(individual.Value);
                                        }
                                        else
                                        {
                                            v3account.DELETED_AT = SmartParametersV2016.defaultDate;
                                        }
                                        break;
                                    case "hold":
                                        var v3hold = v3account.HOLD;
                                        foreach (dynamic holdItem in individual.Value)
                                        {
                                            string holdName = holdItem.Name;
                                            switch (holdName)
                                            {
                                                case "value":
                                                    v3hold.VALUE = holdItem.Value.ToString();
                                                    break;
                                                case "currency":
                                                    v3hold.CURRENCY = holdItem.Value.ToString();
                                                    break;
                                                default:
                                                    break;
                                            }
                                        }
                                        v3account.HOLD = v3hold;
                                        break;

                                    case "name":
                                        v3account.NAME = individual.Value.ToString();
                                        break;
                                    case "platform":
                                        v3account.PLATFORM = individual.Value.ToString();
                                        break;
                                    case "ready":
                                        v3account.READY = individual.Value.ToString();
                                        break;
                                    case "retail_portfolio_id":
                                        v3account.RETAIL_PORTFOLIO_ID = individual.Value.ToString();
                                        break;
                                    case "type":
                                        v3account.TYPE = individual.Value.ToString();
                                        break;
                                    case "updated_at":
                                        if (individual.Value != null)
                                        {
                                            v3account.UPDATED_AT = Convert.ToDateTime(individual.Value);
                                        }
                                        else
                                        {
                                            v3account.UPDATED_AT = SmartParametersV2016.defaultDate;
                                        }
                                        break;
                                    case "uuid":
                                        v3account.UUID = individual.Value.ToString();
                                        break;
                                    default:
                                        Console.Write(column);
                                        break;
                                }
                            }
                            if (v3account.UUID != null)
                            {
                                v3account.USERNAME = Username;
                                v3account.EXCHANGE = Exchange;
                                V3accountList.Add(v3account);
                            }
                        }
                    }
                }
            }
            return V3accountList;
        }
        

#if WPF


        //internal void TabItemChanged(object sender, SelectionChangedEventArgs e, FinanceViewModel financeviewmodel)
        //{
        //    if (sender != null &&
        //        e != null)
        //    {
        //        TabControl abc = sender as TabControl;
        //        if (abc != null)
        //        {
        //            TabItem titem = abc.SelectedItem as TabItem;
        //            string header = titem.Header.ToString();
        //            switch (header)
        //            {
        //                case "Transactions":
        //                    financeviewmodel.AccountsRibbonVisible = Visibility.Hidden;
        //                    financeviewmodel.TransactionsRibbonVisible = Visibility.Visible;
        //                    financeviewmodel.TransActivityEnabled = true;
        //                    financeviewmodel.AcctsActivityEnabled = false;
        //                    break;
        //                case "Accounts":
        //                    financeviewmodel.TransactionsRibbonVisible = Visibility.Hidden;
        //                    financeviewmodel.AccountsRibbonVisible = Visibility.Visible;
        //                    financeviewmodel.TransActivityEnabled = false;
        //                    financeviewmodel.AcctsActivityEnabled = true;
        //                    break;
        //                case "Addresses":
        //                    financeviewmodel.TransactionsRibbonVisible = Visibility.Hidden;
        //                    financeviewmodel.AccountsRibbonVisible = Visibility.Hidden;
        //                    financeviewmodel.TransActivityEnabled = false;
        //                    financeviewmodel.AcctsActivityEnabled = false;
        //                    break;
        //                default:
        //                    break;
        //            }

        //        }
        //    }
        //    return;
        //}
        internal void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e, FinanceViewModel financeviewmodel)
        {
            //if (CryptoTotals.SelectedItem != null)
            //{
            //    // Retrieve selected item data
            //    SmartCrypto.V2CryptoTotals selectedItem = CryptoTotals.SelectedItem as SmartCrypto.V2CryptoTotals;
            //    string currency = selectedItem.CRYPTO_CURRENCY.ToString();
            //    string rate = selectedItem.RATE.ToString();

            //    financeviewmodel.FinanceCryptoTransactionsSpecific =
            //        new List<SmartFinance.CryptoTransactionsView>
            //        (from Currencies in financeviewmodel.FinanceCryptoTransactions
            //         where Currencies.CRYPTO_CURRENCY == currency
            //         orderby Currencies.CREATED_AT
            //         select Currencies);
            //    financeviewmodel.PopupCurrency = currency + "(" +
            //    financeviewmodel.FinanceCryptoTransactionsSpecific.Count.ToString() +
            //                                                          ")";
            //    financeviewmodel.PopupRate = "Rate now: " + Convert.ToDouble(rate).ToString("F3");

            //    //CurrencyPopup currency_popup = new CurrencyPopup()
            //    //{
            //    //    // Took me ALL DAY to get this fucker working ...
            //    //    PlacementTarget = this,     // components
            //    //    Placement = PlacementMode.Center,
            //    //    IsOpen = true
            //    //};
            //    //e.Handled = true;
            //    //CryptoTotals.SelectedItem = null;
            //}
            return;
        }
#endif
#if WPF
        //internal void TransActivityChecked(object sender, EventArgs e, FinanceViewModel financeviewmodel)
        //{
        //    if (sender != null &&
        //        e != null)
        //    {
        //        if (financeviewmodel.TransActivityEnabled)
        //        {
        //            CheckBox abc = sender as CheckBox;
        //            string tag = abc.Tag.ToString();
        //            switch (tag)
        //            {
        //                case "F":
        //                    financeviewmodel.transfiat = (bool)(abc.IsChecked);
        //                    break;
        //                case "B":
        //                    financeviewmodel.buy = (bool)(abc.IsChecked);
        //                    break;
        //                case "S":
        //                    financeviewmodel.sell = (bool)(abc.IsChecked);
        //                    break;
        //                case "T":
        //                    financeviewmodel.trade = (bool)(abc.IsChecked);
        //                    break;
        //                case "X":
        //                    financeviewmodel.send = (bool)(abc.IsChecked);
        //                    break;
        //                default:
        //                    break;
        //            }
        //            SmartFinanceV2021.RebuildTransactions(financeviewmodel);
        //        }
        //    }
        //    return;
        //}

        //internal void AcctsActivityChecked(object sender, EventArgs e, FinanceViewModel financeviewmodel)
        //{
        //    if (sender != null &&
        //        e != null)
        //    {
        //        if (financeviewmodel.AcctsActivityEnabled)
        //        {
        //            CheckBox abc = sender as CheckBox;
        //            string tag = abc.Tag.ToString();
        //            switch (tag)
        //            {
        //                case "F":
        //                    financeviewmodel.acctsfiat = (bool)(abc.IsChecked);
        //                    break;
        //                case "C":
        //                    financeviewmodel.crypto = (bool)(abc.IsChecked);
        //                    break;
        //                default:
        //                    break;
        //            }
        //            SmartFinanceV2021.RebuildAccounts(financeviewmodel);
        //        }
        //    }
        //    return;
        //}
#endif

        internal static async Task<bool> ExtractExchangeRates(
#if WINFORMS
                                                             MainProcess components,
                                                            RichTextBox textBoxConsole,
#endif
                                                            MainViewModel ourviewmodel,
                                                            FinanceViewModel financeviewmodel)
        {
            string V2exchangeratesEndpoint = "api.coinbase.com/v2/exchange-rates?currency=" + financeviewmodel.ConvertToSymbol;
            string exchangerateData = "";
            if (!await CallApiGET(financeviewmodel, $"https://{V2exchangeratesEndpoint}", "", ""))
            {
                return false;
            }
            exchangerateData = financeviewmodel.result;
            AnalyzeExchangeRates(financeviewmodel.ConvertToSymbol, exchangerateData, financeviewmodel.v2exchangeratesList);
#if WINFORMS
            MainProcess.Output_Message(textBoxConsole, "Exchange rate: " + financeviewmodel.ConvertToSymbol, false, false);
#endif
#if WPF || UWP || WINUI
            await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Exchange rate: " + financeviewmodel.ConvertToSymbol);
#endif
#if ANDROID
            await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Exchange rate: " + financeviewmodel.ConvertToSymbol);
#endif
            return true;
            
        }

#if WPF
        internal void Leave_Click(object sender, RoutedEventArgs e, FinanceViewModel financeviewmodel)
        {
            if (sender != null &&
                e != null)
            {
                financeviewmodel.timerClock.Stop();
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

        internal static void TransactionsSelectionChanged(object sender, SelectionChangedEventArgs e,
                                                    FinanceViewModel financeviewmodel)
        {
            return;
        }

        internal static void FinanceTransactionsMouseDoubleClick(object sender, MouseAction e,
                                                    FinanceViewModel financeviewmodel)
        {
            return;
        }
#endif
    }
}    